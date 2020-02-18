/// @file
/// @brief This file contains the ::ColorShapeLinks.UnityApp.HumanThinker class.
///
/// @author Nuno Fachada
/// @date 2019, 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;
using System.Threading;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;

namespace ColorShapeLinks.UnityApp
{
    /// <summary>
    /// Represents a human thinker, for testing purposes.
    /// </summary>
    public class HumanThinker : IThinker
    {
        /// <summary>
        /// Name of the human player.
        /// </summary>
        public const string Name = "Human";

        /// <summary>
        /// Name of the human player.
        /// </summary>
        /// <returns>The string "Human".</returns>
        public override string ToString() => Name;

        /// @copydoc ColorShapeLinks.Common.AI.IThinker.Think
        /// <seealso cref="ColorShapeLinks.Common.AI.IThinker.Think"/>
        /// <exception cref="InvalidOperationException">
        /// Thrown when this method is invoked.
        /// </exception>
        public FutureMove Think(Board board, CancellationToken ct)
        {
            ThinkingInfo?.Invoke("An exception will be thrown in a moment.");
            throw new InvalidOperationException(
                "Humans should think by themselves");
        }

        /// @copydoc ColorShapeLinks.Common.AI.IThinker.ThinkingInfo
        /// <seealso cref="ColorShapeLinks.Common.AI.IThinker.ThinkingInfo"/>
        public event Action<string> ThinkingInfo;
    }
}
