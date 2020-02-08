/// @file
/// @brief This file contains the ::Piece enum.
///
/// @author Nuno Fachada
/// @date 2019
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;

/// <summary>Represents a board piece.</summary>
public struct Piece
{
    /// <summary>The piece color.</summary>
    public readonly PColor color;

    /// <summary>The piece shape.</summary>
    public readonly PShape shape;

    /// <summary>Create a new piece.</summary>
    /// <param name="color">The piece color.</param>
    /// <param name="shape">The piece shape.</param>
    public Piece(PColor color, PShape shape)
    {
        this.color = color;
        this.shape = shape;
    }

    /// <summary>Is the piece of the specified color and shape?</summary>
    /// <param name="color">The piece color.</param>
    /// <param name="shape">The piece shape.</param>
    /// <returns>
    /// `true` if the piece has the specified color and shape, `false`
    /// otherwise.
    /// </returns>
    public bool Is(PColor color, PShape shape) =>
        this.color == color && this.shape == shape;

    /// <summary>
    /// Provides a string representation of the piece in the form "ColorShape".
    /// </summary>
    /// <returns>A string representation of the piece.</returns>
    public override string ToString() => $"{color}{shape}";
}
