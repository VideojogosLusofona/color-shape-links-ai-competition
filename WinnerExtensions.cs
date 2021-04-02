/// @file
/// @brief This file contains the ::ColorShapeLinks.Common.WinnerExtensions
/// class.
///
/// @author Nuno Fachada
/// @date 2019-2021
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

namespace ColorShapeLinks.Common
{
    /// <summary>Extension methods for the <see cref="Winner"/> enum.</summary>
    public static class WinnerExtensions
    {
        /// <summary>
        /// Converts a <see cref="Winner"/> instance into a <see cref="PColor"/>
        /// instance. If conversion is not possible (i.e., if winner not
        /// <see cref="Winner.White"/> or <see cref="Winner.Red"/>), returns
        /// <c>(PColor)(-1)</c>, which is an invalid color.
        /// </summary>
        /// <param name="winner">A <see cref="Winner"/> instance.</param>
        /// <returns>A <see cref="PColor"/> instance.</returns>
        public static PColor ToPColor(this Winner winner)
        {
            // Player color
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
}
