/// @file
/// @brief This file contains the
/// ::ColorShapeLinks.UnityApp.HumanThinkerPrototype class.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using ColorShapeLinks.Common.AI;

namespace ColorShapeLinks.UnityApp
{
    /// <summary>
    /// Human thinker prototype.
    /// </summary>
    public class HumanThinkerPrototype : IThinkerPrototype
    {
        /// <summary>
        /// Name of the underlying thinker.
        /// </summary>
        public string ThinkerName => HumanThinker.Name;

        /// <summary>
        /// Instantiate a new human thinker.
        /// </summary>
        /// <returns>A new human thinker instance.</returns>
        public IThinker Create() => new HumanThinker();
    }
}
