/// @file
/// @brief This file contains the
/// ::ColorShapeLinks.TextBased.App.MatchController class.
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
using ColorShapeLinks.Common.Session;

namespace ColorShapeLinks.TextBased.Lib
{
    /// <summary>
    /// This class runs a game-engine independent match of ColorShapeLinks.
    /// </summary>
    public class MatchController : IMatchSubject
    {
        // Maximum time a thinker has to think
        private readonly int timeLimitMillis;

        // Minimum apparent time a thinker will take to think
        private readonly int minMoveTimeMillis;

        // An array containing the solution, in the game doesn't end in a draw
        private readonly Pos[] solution;

        // Match data
        private readonly IMatchDataProvider matchData;

        // A cancellation token so thinker threads can be ordered to stop
        private readonly CancellationTokenSource ts;

        // A reference to the game board
        private readonly Board board;

        /// <summary>
        /// Sets up a new match.
        /// </summary>
        /// <param name="matchConfig">Match configuration.</param>
        public MatchController(
            IMatchConfig matchConfig, IMatchDataProvider matchData)
        {
            // Keep a reference to the match data
            this.matchData = matchData;

            // Initialize the solution array
            solution = new Pos[matchConfig.WinSequence];

            // Initialize the cancellation token
            ts = new CancellationTokenSource();

            // Get a reference to the game board
            board = matchData.Board;

            // Fetch times from match options
            timeLimitMillis = matchConfig.TimeLimitMillis;
            minMoveTimeMillis = matchConfig.MinMoveTimeMillis;
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
                // Next player plays
                winner = Play();
                // Break loop if a winner is found
                if (winner != Winner.None) break;
            }

            // Notify listeners that match is over
            MatchOver?.Invoke(
                winner,
                solution,
                new string[] {
                    matchData.GetThinker(PColor.White).ToString(),
                    matchData.GetThinker(PColor.Red).ToString() }
            );

            // Return match result
            return winner;
        }

        /// Current thinker makes its move
        private Winner Play()
        {
            // Get a reference to the current thinker
            IThinker thinker = matchData.CurrentThinker;

            // Determine the color of the current thinker
            PColor color = board.Turn;

            // Match result so far
            Winner winner = Winner.None;

            // Thinking start time
            DateTime startTime = DateTime.Now;

            // Real think time in milliseconds
            int thinkTimeMillis;

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
                    // Obtain thinking end time
                    thinkTimeMillis = (int)(DateTime.Now - startTime)
                        .TotalMilliseconds;

                    // How much time left for the minimum apparent move time?
                    timeLeftMillis = minMoveTimeMillis - thinkTimeMillis;

                    // Was the minimum apparent move time reached
                    if (timeLeftMillis > 0)
                    {
                        // If not, wait until it is reached
                        Thread.Sleep(timeLeftMillis);
                    }

                    // Notify listeners of the move performed
                    MovePerformed?.Invoke(
                        color, thinker.ToString(), move, thinkTimeMillis);

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

            // Notify listeners that the board was updated
            BoardUpdate?.Invoke(board);

            // Return winner
            return winner;
        }

        /// @copydoc ColorShapeLinks.TextBased.Lib.IMatchSubject.BoardUpdate
        /// <seealso cref="ColorShapeLinks.TextBased.Lib.IMatchSubject.BoardUpdate"/>
        public event Action<Board> BoardUpdate;

        /// @copydoc ColorShapeLinks.TextBased.Lib.IMatchSubject.NextTurn
        /// <seealso cref="ColorShapeLinks.TextBased.Lib.IMatchSubject.NextTurn"/>
        public event Action<PColor, string> NextTurn;

        /// @copydoc ColorShapeLinks.TextBased.Lib.IMatchSubject.Timeout
        /// <seealso cref="ColorShapeLinks.TextBased.Lib.IMatchSubject.Timeout"/>
        public event Action<PColor, string> Timeout;

        /// @copydoc ColorShapeLinks.TextBased.Lib.IMatchSubject.MovePerformed
        /// <seealso cref="ColorShapeLinks.TextBased.Lib.IMatchSubject.MovePerformed"/>
        public event Action<PColor, string, FutureMove, int> MovePerformed;

        /// @copydoc ColorShapeLinks.TextBased.Lib.IMatchSubject.MatchOver
        /// <seealso cref="ColorShapeLinks.TextBased.Lib.IMatchSubject.MatchOver"/>
        public event Action<Winner, ICollection<Pos>, IList<string>> MatchOver;
    }
}
