/// @file
/// @brief This file contains the ::Game class.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;
using ColorShapeLinks.ConsoleAppLib;

namespace ColorShapeLinks.ConsoleApp
{
    /// <summary>
    /// This class runs a game-engine independent match of ColorShapeLinks.
    /// </summary>
    public class Match : IMatchSubject
    {
        // Maximum time a thinker has to think
        private int timeLimitMillis;

        // Minimum apparent time a thinker will take to think
        private int minMoveTimeMillis;

        // The game board
        private Board board;

        // An array containing the solution, in the game doesn't end in a draw
        private Pos[] solution;

        // List containing the two thinkers
        private IList<IThinker> thinkers;

        // A cancellation token so thinker threads can be ordered to stop
        private CancellationTokenSource ts;


        /// <summary>
        /// Sets up a new match.
        /// </summary>
        /// <param name="options">Match options.</param>
        public Match(Options options)
        {
            // Initialize the solution array
            solution = new Pos[options.WinSequence];

            // Initialize the cancellation token
            ts = new CancellationTokenSource();

            // Fetch times from match options
            timeLimitMillis = options.TimeLimitMillis;
            minMoveTimeMillis = options.MinMoveTimeMillis;

            // Create an array of thinkers
            thinkers = new IThinker[2];

            // Instantiate the two thinkers
            thinkers[0] = AIManager.Instance.NewThinker(
                options.Player1, options, options.Player1Params);
            thinkers[1] = AIManager.Instance.NewThinker(
                options.Player2, options, options.Player2Params);

            // Listen to the thinking info produced by each thinker
            foreach (IThinker t in thinkers) t.ThinkingInfo += OnThinkingInfo;

            // Instantiate a new game board
            board = new Board(options.Rows, options.Cols,
                options.WinSequence, options.RoundPiecesPerPlayer,
                options.SquarePiecesPerPlayer);
        }

        /// <summary>
        /// Runs the match.
        /// </summary>
        /// <returns>The match result.</returns>
        public Winner Run()
        {
            // Initially there's no winner
            Winner winner = Winner.None;

            // Notify listeners that we have a new empty board
            BoardUpdate?.Invoke(board);

            // Game loop
            while (true)
            {
                // White plays
                winner = Play(PColor.White);
                // Break loop if a winner is found
                if (winner != Winner.None) break;

                // Red plays
                winner = Play(PColor.Red);
                // Break loop if a winner is found
                if (winner != Winner.None) break;
            }

            // Notify listeners that match is over
            MatchOver?.Invoke(
                winner,
                solution,
                new string[] { thinkers[0].ToString(), thinkers[1].ToString() }
            );

            // Return match result
            return winner;
        }

        /// <summary>
        /// Player of given <paramref name="color"/> plays its move.
        /// </summary>
        /// <param name="color">Player color.</param>
        /// <returns>
        /// A tuple containing (a) the current match result, and (b) the
        /// solution in case one of the players won.
        /// </returns>
        private Winner Play(PColor color)
        {
            // Get a reference to the current thinker
            IThinker thinker = thinkers[(int)color];

            // Match result so far
            Winner winner = Winner.None;

            // Thinking start time
            DateTime startTime = DateTime.Now;

            // Apparent thinking time left
            int timeLeftMillis;

            // Task to execute the thinker in a separate thread
            Task<FutureMove> thinkTask;

            // Notify listeners that next turn is about to start
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
                FutureMove move = thinkTask.Result;

                // Perform move in game board, get column where move was
                // performed
                int row = board.DoMove(move.shape, move.column);

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

            // Notify listeners that the board was updated
            BoardUpdate?.Invoke(board);

            return winner;
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
