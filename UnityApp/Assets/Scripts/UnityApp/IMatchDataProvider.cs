/// @file
/// @brief This file contains the
/// ::ColorShapeLinks.UnityApp.IMatchDataProvider interface.
///
/// @author Nuno Fachada
/// @date 2019, 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;

namespace ColorShapeLinks.UnityApp
{
    /// <summary>
    /// Defines a data provider for *ColorShapeLinks* matches.
    /// </summary>
    public interface IMatchDataProvider
    {
        /// <summary>The game board.</summary>
        /// <value>The game board.</value>
        Board Board { get; }

        /// <summary>The current thinker.</summary>
        /// <value>The current thinker.</value>
        IThinker CurrentThinker { get; }

        /// <summary>
        /// Maximum real time in seconds that AI can take to play.
        /// </summary>
        /// <value>
        /// Maximum real time in seconds that AI can take to play.
        /// </value>
        float AITimeLimit { get; }

        /// <summary>
        /// Even if the AI plays immediately, this time (in seconds) gives the
        /// illusion that the AI took some minimum time to play.
        /// </summary>
        /// <value>Minimum apparent AI play time.</value>
        float MinAIGameMoveTime { get; }

        /// <summary>Last move animation length in seconds.</summary>
        /// <value>Last move animation length in seconds.</value>
        float LastMoveAnimLength { get; }

        /// <summary>Get thinker of the specified color.</summary>
        /// <param name="thinkerColor">Color of the thinker to get.</param>
        /// <returns>Thinker of the specified color.</returns>
        IThinker GetThinker(PColor thinkerColor);
    }
}
