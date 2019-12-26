/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

public static class WinnerExtensions
{
    public static PColor ToPColor(this Winner winner)
    {
        // Player color by default
        PColor color;

        // Check what color is winner
        if (winner == Winner.White)
            color = PColor.White;
        else if (winner == Winner.Red)
            color = PColor.Red;
        else
            // If winner is not of valid color, return an invalid color
            color = (PColor)(-1);

        return color;
    }
}
