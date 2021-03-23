/// @file
/// @brief This file contains the
/// ::ColorShapeLinks.Common.AI.Examples.SequentialAIThinker class.
///
/// @author Nuno Fachada
/// @date 2019, 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System.Threading;

namespace ColorShapeLinks.Common.AI.Examples
{
    /// <summary>
    /// Implementation of an AI that will always play in sequence, from the
    /// first to the last column. It will start by using pieces with its winning
    /// shape, and when these are over, it continues by playing pieces with the
    /// losing shape.
    /// </summary>
    public class SequentialAIThinker : AbstractThinker
    {
        // Last column played
        private int lastCol = -1;

        /// @copydoc IThinker.Think
        /// <seealso cref="IThinker.Think"/>
        public override FutureMove Think(Board board, CancellationToken ct)
        {
            // The move to perform
            FutureMove move;

            // Find next free column where to play
            do
            {
                // Get next column
                lastCol++;
                if (lastCol >= board.cols) lastCol = 0;
                // Is this task to be cancelled?
                if (ct.IsCancellationRequested) return FutureMove.NoMove;
            }
            while (board.IsColumnFull(lastCol));

            // Try to use the winning shape first
            if (board.PieceCount(board.Turn, board.Turn.Shape()) > 0)
            {
                move = new FutureMove(lastCol, board.Turn.Shape());
            }
            // If there's no pieces with the winning shape left, try and use
            // the other shape
            else if (board.PieceCount(board.Turn, board.Turn.Other().Shape()) > 0)
            {
                move = new FutureMove(lastCol, board.Turn.Other().Shape());
            }
            // Otherwise return a "no move" (this should never happen)
            else
            {
                move = FutureMove.NoMove;
            }

            // Return move
            return move;
        }
    }
}
