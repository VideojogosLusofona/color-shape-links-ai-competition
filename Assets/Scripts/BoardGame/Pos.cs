/// @file
/// @brief This file contains the ::Pos struct.
///
/// @author Nuno Fachada
/// @date 2019
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

// Struct for representing a board position
public struct Pos
{
    public readonly int row;
    public readonly int col;
    public Pos(int row, int col)
    {
        this.row = row;
        this.col = col;
    }

    public override string ToString() => $"({row},{col})";
}
