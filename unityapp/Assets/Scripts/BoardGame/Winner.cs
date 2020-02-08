/// @file
/// @brief This file contains the ::Winner enum.
///
/// @author Nuno Fachada
/// @date 2019, 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

namespace ColorShapeLinks.BoardGame
{
    /// <summary>Winner or result of match.</summary>
    public enum Winner
    {
        /// <summary>No result yet.</summary>
        None,

        /// <summary>Draw.</summary>
        Draw,

        /// <summary>White wins.</summary>
        White,

        /// <summary>Red wins.</summary>
        Red
    }
}
