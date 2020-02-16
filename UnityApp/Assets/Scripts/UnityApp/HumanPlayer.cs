/// @file
/// @brief This file contains the ::ColorShapeLinks.UnityApp.HumanPlayer class.
///
/// @author Nuno Fachada
/// @date 2019, 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using ColorShapeLinks.Common.AI;

namespace ColorShapeLinks.UnityApp
{
    /// <summary>
    /// Represents a human player, for testing purposes.
    /// </summary>
    public class HumanPlayer : IPlayer
    {
        // Reference to a human thinker
        private IThinker thinker;

        /// <summary>
        /// Is the player human?
        /// </summary>
        /// <value>
        /// Always evaluates to `true`, since this is a human player.
        /// </value>
        public bool IsHuman => true;

        /// <summary>
        /// Name of the human player.
        /// </summary>
        /// <returns>The string "Human".</returns>
        public override string ToString() => "Human";


        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        public HumanPlayer()
        {
            thinker = new HumanThinker();
        }

        /// <summary>
        /// An instance of <cref="HumanThinker"/>.
        /// </summary>
        public IThinker Thinker => thinker;
    }
}
