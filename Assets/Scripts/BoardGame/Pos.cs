/// @file
/// @brief This file contains the ::Pos struct.
///
/// @author Nuno Fachada
/// @date 2019
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

/// <summary>Represents a board position.</summary>
public struct Pos
{
    /// <summary>Board row.</summary>
    public readonly int row;

    /// <summary>Board column.</summary>
    public readonly int col;

    /// <summary>
    /// Creates a new board position.
    /// </summary>
    /// <param name="row">Board row.</param>
    /// <param name="col">Board column.</param>
    public Pos(int row, int col)
    {
        this.row = row;
        this.col = col;
    }

    /// <summary>
    /// Provides a string representation of the board position in the form
    /// "(row,col)".
    /// </summary>
    /// <returns>A string representation of the board position.</returns>
    public override string ToString() => $"({row},{col})";
}
