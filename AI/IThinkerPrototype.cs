/// @file
/// @brief This file contains the ::ColorShapeLinks.Common.AI.IThinkerPrototype
/// interface.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

namespace ColorShapeLinks.Common.AI
{
    /// <summary>
    /// Interface for all thinker prototypes.
    /// </summary>
    public interface IThinkerPrototype
    {
        /// <summary>
        /// Name of the underlying thinker.
        /// </summary>
        string ThinkerName { get; }

        /// <summary>
        /// Instantiate a new thinker from this prototype.
        /// </summary>
        /// <returns>A new thinker instance.</returns>
        IThinker Create();
    }
}
