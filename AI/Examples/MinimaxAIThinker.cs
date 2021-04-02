/// @file
/// @brief This file contains the
/// ::ColorShapeLinks.Common.AI.Examples.MinimaxAIThinker class.
///
/// @author Nuno Fachada
/// @date 2020, 2021
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;
using System.Threading;

namespace ColorShapeLinks.Common.AI.Examples
{
    /// <summary>
    /// Sample AI thinker using a basic Minimax algorithm with a naive
    /// heuristic which previledges center board positions.
    /// </summary>
    /// <remarks>
    /// This is the same implementation used in the @ref minimax tutorial.
    /// </remarks>
    public class MinimaxAIThinker : AbstractThinker
    {
        // Maximum Minimax search depth.
        private int maxDepth;

        /// <summary>
        /// The default maximum search depth.
        /// </summary>
        public const int defaultMaxDepth = 3;

        /// <summary>
        /// Setups up this thinker's maximum search depth.
        /// </summary>
        /// <param name="str">
        /// A string which should be convertible to a positive `int`.
        /// </param>
        /// <remarks>
        /// If <paramref name="str"/> is not convertible to a positive `int`,
        /// the maximum search depth is set to <see cref="defaultMaxDepth"/>.
        /// </remarks>
        /// <seealso cref="ColorShapeLinks.Common.AI.AbstractThinker.Setup"/>
        public override void Setup(string str)
        {
            // Try to get the maximum depth from the parameters
            if (!int.TryParse(str, out maxDepth))
            {
                // If not possible, set it to the default
                maxDepth = defaultMaxDepth;
            }

            // If a non-positive integer was provided, reset it to the default
            if (maxDepth < 1) maxDepth = defaultMaxDepth;
        }

        /// <summary>
        /// Returns the name of this AI thinker which will include the
        /// maximum search depth.
        /// </summary>
        /// <returns>The name of this AI thinker.</returns>
        public override string ToString()
        {
            return base.ToString() + "D" + maxDepth;
        }

        /// @copydoc IThinker.Think
        /// <seealso cref="IThinker.Think"/>
        public override FutureMove Think(Board board, CancellationToken ct)
        {
            // Invoke minimax, starting with zero depth
            (FutureMove move, float score) decision =
                Minimax(board, ct, board.Turn, board.Turn, 0);

            // Return best move
            return decision.move;
        }

        /// <summary>
        /// A basic implementation of the Minimax algorithm.
        /// </summary>
        /// <param name="board">The game board.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <param name="player">
        /// Color of the AI controlling this thinker.
        /// </param>
        /// <param name="turn">
        /// Color of the player playing in this turn.
        /// </param>
        /// <param name="depth">Current search depth.</param>
        /// <returns>
        /// A value tuple with:
        /// <list type="bullet">
        /// <item>
        /// <term><c>move</c></term>
        /// <description>
        /// The best move from the perspective of who's playing in this turn.
        /// </description>
        /// </item>
        /// <item>
        /// <term><c>score</c></term>
        /// <description>
        /// The heuristic score associated with <c>move</c>.
        /// </description>
        /// </item>
        /// </list>
        /// </returns>
        private (FutureMove move, float score) Minimax(
            Board board, CancellationToken ct,
            PColor player, PColor turn, int depth)
        {
            // Move to return and its heuristic value
            (FutureMove move, float score) selectedMove;

            // Current board state
            Winner winner;

            // If a cancellation request was made...
            if (ct.IsCancellationRequested)
            {
                // ...set a "no move" and skip the remaining part of
                // the algorithm
                selectedMove = (FutureMove.NoMove, float.NaN);
            }
            // Otherwise, if it's a final board, return the appropriate
            // evaluation
            else if ((winner = board.CheckWinner()) != Winner.None)
            {
                if (winner.ToPColor() == player)
                {
                    // AI player wins, return highest possible score
                    selectedMove = (FutureMove.NoMove, float.PositiveInfinity);
                }
                else if (winner.ToPColor() == player.Other())
                {
                    // Opponent wins, return lowest possible score
                    selectedMove = (FutureMove.NoMove, float.NegativeInfinity);
                }
                else
                {
                    // A draw, return zero
                    selectedMove = (FutureMove.NoMove, 0f);
                }
            }
            // If we're at maximum depth and don't have a final board, use
            // the heuristic
            else if (depth == maxDepth)
            {
                selectedMove = (FutureMove.NoMove, Heuristic(board, player));
            }
            else // Board not final and depth not at max...
            {
                //...so let's test all possible moves and recursively call
                // Minimax() for each one of them, maximizing or minimizing
                // depending on who's turn it is

                // Initialize the selected move...
                selectedMove = turn == player
                    // ...with negative infinity if it's the AI's turn and
                    // we're maximizing (so anything except defeat will be
                    // better than this)
                    ? (FutureMove.NoMove, float.NegativeInfinity)
                    // ...or with positive infinity if it's the opponent's
                    // turn and we're minimizing (so anything except victory
                    // will be worse than this)
                    : (FutureMove.NoMove, float.PositiveInfinity);

                // Test each column
                for (int i = 0; i < Cols; i++)
                {
                    // Skip full columns
                    if (board.IsColumnFull(i)) continue;

                    // Test shapes
                    for (int j = 0; j < 2; j++)
                    {
                        // Get current shape
                        PShape shape = (PShape)j;

                        // Use this variable to keep the current board's score
                        float eval;

                        // Skip unavailable shapes
                        if (board.PieceCount(turn, shape) == 0) continue;

                        // Test move, call minimax and undo move
                        board.DoMove(shape, i);
                        eval = Minimax(
                            board, ct, player, turn.Other(), depth + 1).score;
                        board.UndoMove();

                        // If we're maximizing, is this the best move so far?
                        if (turn == player
                            && eval >= selectedMove.score)
                        {
                            // If so, keep it
                            selectedMove = (new FutureMove(i, shape), eval);
                        }
                        // Otherwise, if we're minimizing, is this the worst
                        // move so far?
                        else if (turn == player.Other()
                            && eval <= selectedMove.score)
                        {
                            // If so, keep it
                            selectedMove = (new FutureMove(i, shape), eval);
                        }
                    }
                }
            }

            // Return movement and its heuristic value
            return selectedMove;
        }

        /// <summary>
        /// Naive heuristic function which previledges center board positions.
        /// </summary>
        /// <param name="board">The game board.</param>
        /// <param name="color">
        /// Perspective from which the board will be evaluated.
        /// </param>
        /// <returns>
        /// The heuristic value of the given <paramref name="board"/> from
        /// the perspective of the specified <paramref name="color"/.
        /// </returns>
        private float Heuristic(Board board, PColor color)
        {
            // Distance between two points
            float Dist(float x1, float y1, float x2, float y2)
            {
                return (float)Math.Sqrt(
                    Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
            }

            // Determine the center row
            float centerRow = board.rows / 2;
            float centerCol = board.cols / 2;

            // Maximum points a piece can be awarded when it's at the center
            float maxPoints = Dist(centerRow, centerCol, 0, 0);

            // Current heuristic value
            float h = 0;

            // Loop through the board looking for pieces
            for (int i = 0; i < board.rows; i++)
            {
                for (int j = 0; j < board.cols; j++)
                {
                    // Get piece in current board position
                    Piece? piece = board[i, j];

                    // Is there any piece there?
                    if (piece.HasValue)
                    {
                        // If the piece is of our color, increment the
                        // heuristic inversely to the distance from the center
                        if (piece.Value.color == color)
                            h += maxPoints - Dist(centerRow, centerCol, i, j);
                        // Otherwise decrement the heuristic value using the
                        // same criteria
                        else
                            h -= maxPoints - Dist(centerRow, centerCol, i, j);
                        // If the piece is of our shape, increment the
                        // heuristic inversely to the distance from the center
                        if (piece.Value.shape == color.Shape())
                            h += maxPoints - Dist(centerRow, centerCol, i, j);
                        // Otherwise decrement the heuristic value using the
                        // same criteria
                        else
                            h -= maxPoints - Dist(centerRow, centerCol, i, j);
                    }
                }
            }
            // Return the final heuristic score for the given board
            return h;
        }
    }
}
