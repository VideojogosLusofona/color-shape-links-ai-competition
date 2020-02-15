/// @file
/// @brief This file contains the ::ColorShapeLinks.TextBased.App.Match class.
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
using ColorShapeLinks.TextBased.Lib;

namespace ColorShapeLinks.TextBased.App
{
    /// <summary>
    /// This class runs a game-engine independent match of ColorShapeLinks.
    /// </summary>
    public class Match : IMatchSubject
    {
        // Maximum time a thinker has to think
        private readonly int timeLimitMillis;

        // Minimum apparent time a thinker will take to think
        private readonly int minMoveTimeMillis;

        // The game board
        private readonly Board board;

        // An array containing the solution, in the game doesn't end in a draw
        private readonly Pos[] solution;

        // List containing the two thinkers
        private readonly IList<IThinker> thinkers;

        // A cancellation token so thinker threads can be ordered to stop
        private readonly CancellationTokenSource ts;

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
                options.Thinker1, options, options.Thinker1Params);
            thinkers[1] = AIManager.Instance.NewThinker(
                options.Thinker2, options, options.Thinker2Params);

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

            // Ask thinker to think about its next move
            thinkTask = Task.Run(
                    () => thinker.Think(board.Copy(), ts.Token));

            // Wait for thinker to think... until the allowed time limit
            if (thinkTask.Wait(timeLimitMillis))
            {
                // Thinker successfully made a move within the time limit

                // Get the move selected by the thinker
                FutureMove move = thinkTask.Result;

                // Perform move in game board, get column where move was
                // performed
                int row = board.DoMove(move.shape, move.column);

                // If the column had space for the move...
                if (row >= 0)
                {
                    // Notify listeners of the move performed
                    MovePerformed?.Invoke(color, thinker.ToString(), move);

                    // Get possible winner and solution
                    winner = board.CheckWinner(solution);
                }
                else
                {
                    // If we get here, column didn't have space for the move,
                    // which means that thinker made an invalid move
                    throw new InvalidOperationException("Invalid move");
                }
            }
            else // Did the time limit expired?
            {
                // Notify thinker to voluntarily stop thinking
                ts.Cancel();

                // Set the other thinker as the winner of the match
                winner = color == PColor.Red ? Winner.White : Winner.Red;

                // Notify listeners that thinker took too long to play
                Timeout?.Invoke(color, thinker.ToString());
            }

            // How much time is left for the minimum apparent move time?
            timeLeftMillis = minMoveTimeMillis
                - (int)(DateTime.Now - startTime).TotalMilliseconds;

            // Was the minimum apparent move time reached
            if (timeLeftMillis > 0)
            {
                // If not, wait until it is reached
                Thread.Sleep(timeLeftMillis);
            }

            // Notify listeners that the board was updated
            BoardUpdate?.Invoke(board);

            // Return winner
            return winner;
        }

        /// <summary>
        /// This method listens for thinker's thinking info and in turn
        /// forwards this information to match listeners.
        /// </summary>
        /// <param name="info">Thinking info.</param>
        private void OnThinkingInfo(string info)
        {
            ThinkingInfo?.Invoke(info);
        }

        /// @copydoc ColorShapeLinks.TextBased.Lib.IMatchSubject.BoardUpdate
        /// <seealso cref="ColorShapeLinks.TextBased.Lib.IMatchSubject.BoardUpdate"/>
        public event Action<Board> BoardUpdate;

        /// @copydoc ColorShapeLinks.TextBased.Lib.IMatchSubject.NextTurn
        /// <seealso cref="ColorShapeLinks.TextBased.Lib.IMatchSubject.NextTurn"/>
        public event Action<PColor, string> NextTurn;

        /// @copydoc ColorShapeLinks.TextBased.Lib.IMatchSubject.TurnInfo
        /// <seealso cref="ColorShapeLinks.TextBased.Lib.IMatchSubject.ThinkingInfo"/>
        public event Action<string> ThinkingInfo;

        /// @copydoc ColorShapeLinks.TextBased.Lib.IMatchSubject.Timeout
        /// <seealso cref="ColorShapeLinks.TextBased.Lib.IMatchSubject.Timeout"/>
        public event Action<PColor, string> Timeout;

        /// @copydoc ColorShapeLinks.TextBased.Lib.IMatchSubject.MovePerformed
        /// <seealso cref="ColorShapeLinks.TextBased.Lib.IMatchSubject.MovePerformed"/>
        public event Action<PColor, string, FutureMove> MovePerformed;

        /// @copydoc ColorShapeLinks.TextBased.Lib.IMatchSubject.MatchOver
        /// <seealso cref="ColorShapeLinks.TextBased.Lib.IMatchSubject.MatchOver"/>
        public event Action<Winner, ICollection<Pos>, IList<string>> MatchOver;
    }
}
