/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

public struct Piece
{
    public readonly Color color;
    public readonly Shape shape;

    public Piece(Color color, Shape shape)
    {
        this.color = color;
        this.shape = shape;
    }
}
