/// @file
/// @brief This file contains the
/// ::ColorShapeLinks.Common.Session.IMatchDataProvider interface.
///
/// @author Nuno Fachada
/// @date 2019, 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using ColorShapeLinks.Common.AI;

namespace ColorShapeLinks.Common.Session
{
    /// <summary>
    /// Defines a data provider for ColorShapeLinks matches.
    /// </summary>
    public interface IMatchDataProvider
    {
        /// <summary>The game board.</summary>
        /// <value>The game board.</value>
        Board Board { get; }

        /// <summary>The current thinker.</summary>
        /// <value>The current thinker.</value>
        IThinker CurrentThinker { get; }

        /// <summary>Get thinker of the specified color.</summary>
        /// <param name="thinkerColor">Color of the thinker to get.</param>
        /// <returns>Thinker of the specified color.</returns>
        IThinker GetThinker(PColor thinkerColor);
    }
}
