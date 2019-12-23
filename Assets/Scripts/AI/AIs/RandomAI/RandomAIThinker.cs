/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using System.Threading;

public class RandomAIThinker : IThinker
{
    private Random random;

    public RandomAIThinker()
    {
        random = new Random();
    }

    public FutureMove Think(Board board, CancellationToken ct)
    {
        // Is this task to be cancelled?
        if (ct.IsCancellationRequested) return FutureMove.NoMove;

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
            col = random.Next(board.cols);
        }
        while (board.IsColumnFull(col));

        // Return it
        return new FutureMove(col, shape);
    }
}
