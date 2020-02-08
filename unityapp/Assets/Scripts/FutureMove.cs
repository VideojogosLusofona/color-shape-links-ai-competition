/// @file
/// @brief This file contains the ::FutureMove struct.
///
/// @author Nuno Fachada
/// @date 2019
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

/// <summary>
/// Represents a move to be performed in the future.
/// </summary>
public struct FutureMove
{
    /// <summary>
    /// The column where to drop the piece.
    /// </summary>
    public readonly int column;

    /// <summary>
    /// The piece to use in the move.
    /// </summary>
    public readonly PShape shape;

    /// <summary>
    /// Represent a "no move" decision, when an IA is unable to decide which
    /// move to perform, due to a timeout or exception.
    /// </summary>
    /// <returns>A "no move" decision.</returns>
    public static FutureMove NoMove => new FutureMove(-1, (PShape)(-1));

    /// <summary>
    /// Create a future move.
    /// </summary>
    /// <param name="column">The column where to drop the piece.</param>
    /// <param name="shape">The piece to use in the move.</param>
    public FutureMove(int column, PShape shape)
    {
        this.column = column;
        this.shape = shape;
    }
}
