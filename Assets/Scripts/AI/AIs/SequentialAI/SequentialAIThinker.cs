/// @file
/// @brief This file contains the ::SequentialAIThinker class.
///
/// @author Nuno Fachada
/// @date 2019
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System.Threading;

/// <summary>
/// Implementation of an AI that will always play in sequence, from the first
/// to the last column. It will start by spending all round pieces, and only
/// then start using the square pieces.
/// </summary>
public class SequentialAIThinker : IThinker
{
    // Last column played
    private int lastCol = -1;

    /// @copydoc IThinker.Think
    /// <seealso cref="IThinker.Think"/>
    public FutureMove Think(Board board, CancellationToken ct)
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

        // Try to use a round piece first
        if (board.PieceCount(board.Turn, PShape.Round) > 0)
        {
            move = new FutureMove(lastCol, PShape.Round);
        }
        // If there's no round pieces left, let's try a square pieces
        else if (board.PieceCount(board.Turn, PShape.Square) > 0)
        {
            move = new FutureMove(lastCol, PShape.Square);
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
