/// @file
/// @brief This file contains the ::Game class.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;
using ColorShapeLinks.ConsoleAppLib;

namespace ColorShapeLinks.ConsoleApp
{
    public class Game : IMatchSubject
    {
        private int timeLimitMillis;
        private int minMoveTimeMillis;
        private Board board;
        private IThinker[] thinkers;

        public Game(Options options)
        {
            timeLimitMillis = options.TimeLimitMillis;
            minMoveTimeMillis = options.MinMoveTimeMillis;

            thinkers = new IThinker[2];

            thinkers[0] = AIManager.Instance.NewThinker(
                options.Player1, options, options.Player1Params);
            thinkers[1] = AIManager.Instance.NewThinker(
                options.Player2, options, options.Player2Params);

            foreach (IThinker t in thinkers) t.ThinkingInfo += OnThinkingInfo;

            board = new Board(options.Rows, options.Cols,
                options.WinSequence, options.RoundPiecesPerPlayer,
                options.SquarePiecesPerPlayer);
        }

        public ExitStatus Run()
        {
            Winner winner = Winner.None;
            Pos[] solution;

            while (true)
            {
                (winner, solution) = Play(PColor.White);
                if (winner != Winner.None) break;
                (winner, solution) = Play(PColor.Red);
                if (winner != Winner.None) break;
            }

            BoardUpdate?.Invoke(board);

            MatchOver?.Invoke(
                winner,
                solution,
                new string[] { thinkers[0].ToString(), thinkers[1].ToString() }
            );

            return winner.ToExitStatus();
        }

        private (Winner, Pos[]) Play(PColor color)
        {
            int row;
            FutureMove move;
            CancellationTokenSource ts = new CancellationTokenSource();
            IThinker thinker = thinkers[(int)color];
            Winner winner = Winner.None;
            Pos[] solution = new Pos[board.piecesInSequence];
            DateTime startTime = DateTime.Now;
            int timeLeftMillis;

            // Task to execute the thinker in a separate thread
            Task<FutureMove> thinkTask;

            // Update board view
            BoardUpdate?.Invoke(board);

            NextTurn?.Invoke(color, thinker.ToString());

            thinkTask = Task.Run(
                    () => thinker.Think(board.Copy(), ts.Token));

            if (!thinkTask.Wait(timeLimitMillis))
            {
                ts.Cancel();
                winner = color == PColor.Red ? Winner.White : Winner.Red;
                solution = null;
                TooLong?.Invoke(color, thinker.ToString());
            }
            else
            {
                // Get move for current player
                move = thinkTask.Result;

                // Perform move in game board, get column where move was
                // performed
                row = board.DoMove(move.shape, move.column);

                // If the column had space for the move...
                if (row >= 0)
                {
                    MovePerformed?.Invoke(color, thinker.ToString(), move);

                    // Get possible winner and solution
                    winner = board.CheckWinner(solution);
                }
                else // If we get here, column didn't have space for the move
                {
                    throw new InvalidOperationException("Invalid move");
                }
            }

            timeLeftMillis = minMoveTimeMillis
                - (int)(DateTime.Now - startTime).TotalMilliseconds;
            if (timeLeftMillis > 0)
            {
                Thread.Sleep(timeLeftMillis);
            }

            return (winner, solution);
        }

        private void OnThinkingInfo(ICollection<string> info)
        {
            TurnInfo?.Invoke(info);
        }

        public event Action<Board> BoardUpdate;
        public event Action<PColor, string> NextTurn;
        public event Action<ICollection<string>> TurnInfo;
        public event Action<PColor, string> TooLong;
        public event Action<PColor, string, FutureMove> MovePerformed;
        public event Action<Winner, ICollection<Pos>, IList<string>> MatchOver;

    }
}
