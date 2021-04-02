/// @file
/// @brief This file contains the
/// ::ColorShapeLinks.Common.AI.Examples.RandomAIThinker class.
///
/// @author Nuno Fachada
/// @date 2019-2021
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;
using System.Threading;

namespace ColorShapeLinks.Common.AI.Examples
{
    /// <summary>
    /// Implementation of an AI that will play randomly.
    /// </summary>
    public class RandomAIThinker : AbstractThinker
    {
        // A random number generator instance
        private Random random;

        /// <summary>
        /// Initialize the random number generator used for this random
        /// thinker.
        /// </summary>
        /// <param name="str">
        /// If the string is convertible to <c>int</c>, this <c>int</c> value
        /// is sed as a seed for the random number generator. Otherwise, a
        /// random seed value is used instead.
        /// </param>
        /// <seealso cref="AbstractThinker.Setup"/>
        public override void Setup(string str)
        {
            if (int.TryParse(str, out int seed))
                random = new Random(seed);
            else
                random = new Random();
        }

        /// <summary>
        /// This method will simply return a random valid move.
        /// </summary>
        /// <param name="board">The game board.</param>
        /// <param name="ct">A cancellation token (ignored).</param>
        /// <returns>The move to be performed.</returns>
        /// <seealso cref="IThinker.Think"/>
        public override FutureMove Think(Board board, CancellationToken ct)
        {
            // Check how many pieces current player has
            int roundPieces = board.PieceCount(board.Turn, PShape.Round);
            int squarePieces = board.PieceCount(board.Turn, PShape.Square);

            // Chose a random piece
            int pieceRand = random.Next(roundPieces + squarePieces);
            PShape shape = pieceRand < roundPieces
                ? PShape.Round : PShape.Square;

            // Chose a random free position
            int col;
            do
            {
                // Get a random position
                col = random.Next(board.cols);
                // Is this task to be cancelled?
                if (ct.IsCancellationRequested) return FutureMove.NoMove;
            }
            while (board.IsColumnFull(col));

            // Return the random move
            return new FutureMove(col, shape);
        }
    }
}
