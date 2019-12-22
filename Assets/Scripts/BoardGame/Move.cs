/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

public struct Move
{
    public readonly int row;
    public readonly int col;
    public readonly Piece piece;

    public Move(int row, int col, Piece piece)
    {
        this.row = row;
        this.col = col;
        this.piece = piece;
    }
}
