/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

public struct FutureMove
{
    public readonly int column;
    public readonly PShape shape;

    public static FutureMove NoMove => new FutureMove(-1, (PShape)(-1));

    public FutureMove(int column, PShape shape)
    {
        this.column = column;
        this.shape = shape;
    }
}
