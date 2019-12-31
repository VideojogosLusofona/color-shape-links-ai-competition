/// @file
/// @brief This file contains the ::Move struct.
///
/// @author Nuno Fachada
/// @date 2019
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

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
