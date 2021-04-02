/// @file
/// @brief This file contains the
/// ::ColorShapeLinks.Common.AI.UncooperativeThinkerException class.
///
/// @author Nuno Fachada
/// @date 2020, 2021
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;

namespace ColorShapeLinks.Common.AI
{
    /// <summary>
    /// Exception thrown when a thinker refuses to terminate, even after
    /// being requested to do so.
    /// </summary>
    public class UncooperativeThinkerException : Exception
    {
        /// <summary>
        /// Hard time limit in millisseconds that the app should wait (after
        /// notifying the thinker to terminate) before it shuts down.
        /// </summary>
        public const int HardThinkingLimitMs = 5000;

        /// <summary>
        /// Create a new uncooperative thinker exception.
        /// </summary>
        /// <param name="thinker">
        /// The thinker that is being uncooperative.
        /// </param>
        public UncooperativeThinkerException(IThinker thinker)
            : base($"{thinker} refused to terminate execution")
        { }
    }
}
