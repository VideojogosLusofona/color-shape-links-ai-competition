/// @file
/// @brief This file contains the ::Piece enum.
///
/// @author Nuno Fachada
/// @date 2019
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;

public struct Piece
{
    public readonly PColor color;
    public readonly PShape shape;

    public Winner Player
    {
        get
        {
            if (color == PColor.White) return Winner.White;
            if (color == PColor.Red) return Winner.Red;

            // If we get here, there is a bug
            throw new InvalidOperationException("Piece has invalid color");
        }
    }

    public Piece(PColor color, PShape shape)
    {
        this.color = color;
        this.shape = shape;
    }

    public bool Is(PColor color, PShape shape) =>
        this.color == color && this.shape == shape;

    public override string ToString() => $"{color}{shape}";
}
