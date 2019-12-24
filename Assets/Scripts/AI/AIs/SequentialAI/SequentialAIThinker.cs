/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;
using System.Threading;

public class SequentialAIThinker : IThinker
{
    private int lastCol = -1;

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
