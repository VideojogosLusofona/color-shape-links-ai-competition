/// @file
/// @brief This file contains the ::RandomAIThinker class.
///
/// @author Nuno Fachada
/// @date 2019
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;
using System.Threading;

/// <summary>
/// Implementation of an AI that will play randomly.
/// </summary>
public class RandomAIThinker : IThinker
{
    // A random number generator instance
    private Random random;

    /// <summary>
    /// Create a new instance of RandomIAThinker.
    /// </summary>
    public RandomAIThinker()
    {
        random = new Random();
    }

    /// @copydoc IThinker.Think
    /// <seealso cref="IThinker.Think"/>
    public FutureMove Think(Board board, CancellationToken ct)
    {
        // Check how many pieces current player has
        int roundPieces = board.PieceCount(board.Turn, PShape.Round);
        int squarePieces = board.PieceCount(board.Turn, PShape.Square);

        // Chose a random piece
        int pieceRand = random.Next(roundPieces + squarePieces);
        PShape shape = pieceRand < roundPieces ? PShape.Round : PShape.Square;

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
