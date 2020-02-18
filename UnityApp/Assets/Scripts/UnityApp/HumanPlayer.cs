/// @file
/// @brief This file contains the ::ColorShapeLinks.UnityApp.HumanPlayer class.
///
/// @author Nuno Fachada
/// @date 2019, 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

namespace ColorShapeLinks.UnityApp
{
    /// <summary>
    /// Represents a human player, for testing purposes.
    /// </summary>
    public class HumanPlayer : IPlayer
    {
        // The fully qualified name of the human thinker class
        private string thinker = typeof(HumanThinker).FullName;

        /// <summary>
        /// Name of the human player.
        /// </summary>
        /// <returns>
        /// The fully qualified name of the human thinker class.
        /// </returns>
        public override string ToString() => thinker;

        /// <summary>
        /// The fully qualified name of the human thinker class.
        /// </summary>
        public string ThinkerFQN => thinker;
    }
}
