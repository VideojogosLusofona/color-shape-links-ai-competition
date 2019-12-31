/// @file
/// @brief This file contains the ::WinnerExtensions class.
///
/// @author Nuno Fachada
/// @date 2019
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

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
