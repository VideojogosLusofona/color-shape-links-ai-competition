/// @file
/// @brief This file contains the ::ColorShapeLinks.UnityApp.IPlayer interface.
///
/// @author Nuno Fachada
/// @date 2019, 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using ColorShapeLinks.Common.AI;

namespace ColorShapeLinks.UnityApp
{
    /// <summary>
    /// Defines a player for the ColorShapeLinks board game.
    /// </summary>
    public interface IPlayer
    {
        /// <summary>The player's thinker.</summary>
        /// <value>
        /// An instance of <see cref="ColorShapeLinks.Common.AI.IThinker"/>.
        /// </value>
        IThinker Thinker { get; }
    }
}
