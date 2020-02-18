/// @file
/// @brief This file contains the ::ColorShapeLinks.Common.PColorExtensions
/// class.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

namespace ColorShapeLinks.Common
{
    /// <summary>Extension methods for the <see cref="PColor"/> enum.</summary>
    public static class PColorExtensions
    {
        /// <summary>
        /// Provides a consistent way to get a formatted thinker name which
        /// includes the color with which he's playing.
        /// </summary>
        /// <param name="color">Thinker's color.</param>
        /// <param name="name">Thinker's name.</param>
        /// <returns>A formatted thinker name with color.</returns>
        public static string FormatName(this PColor color, string name) =>
            $"{name} ({color})";
    }
}
