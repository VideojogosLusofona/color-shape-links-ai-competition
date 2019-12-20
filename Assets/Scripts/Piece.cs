/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

 using System;

public struct Piece
{
    public readonly Color color;
    public readonly Shape shape;

    public Winner Player
    {
        get
        {
            if (color == Color.White) return Winner.Player1;
            if (color == Color.Red) return Winner.Player2;

            // If we get here, there is a bug
            throw new InvalidOperationException("Piece has invalid color");
        }
    }

    public Piece(Color color, Shape shape)
    {
        this.color = color;
        this.shape = shape;
    }

    public bool Is(Color color, Shape shape) =>
        this.color == color && this.shape == shape;

    public override string ToString() => $"{color}{shape}";
}
