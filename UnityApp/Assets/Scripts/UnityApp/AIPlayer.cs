/// @file
/// @brief This file contains the ::ColorShapeLinks.UnityApp.AIPlayer class.
///
/// @author Nuno Fachada
/// @date 2019, 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using UnityEngine;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;

namespace ColorShapeLinks.UnityApp
{
    /// <summary>Component which represents an AI thinker.</summary>
    /// <remarks>
    /// Each AI thinker to be used in a match or tournament should be
    /// represented by an instance of this component.
    /// </remarks>
    public sealed class AIPlayer : MonoBehaviour, IPlayer
    {
        /// <summary>
        /// Unity Editor variable which defines if the AI thinker is active.
        /// </summary>
        [SerializeField] private bool isActive = true;

        /// <summary>
        /// Selected AI thinker.
        /// </summary>
        [SerializeField] [AIList] private string selectedAI = null;

        /// <summary>
        /// String containing AI thinker-specific parameters.
        /// </summary>
        [SerializeField] private string aiConfig = "";

        /// <summary>Is the AI thinker active?</summary>
        /// <value>`true` if the AI is active, `false` otherwise.</value>
        public bool IsActive => isActive;

        /// <summary>The AI thinker instance.</summary>
        /// <value>
        /// An instance of <see cref="ColorShapeLinks.Common.AI.IThinker"/>.
        /// </value>
        /// <seealso cref="IPlayer.Thinker"/>
        public IThinker Thinker
        {
            get
            {
                // The underlying thinker is lazy instantiated
                if (thinker == null)
                {
                    // Obtain the component holding the game configuration
                    IGameConfig gameConfig = GetComponent<IGameConfig>();

                    // Instantiate the thinker
                    AbstractThinker aThinker = AIManager.Instance.NewThinker(
                        selectedAI, gameConfig, aiConfig);

                    // Thinking information is printed in the Unity console
                    aThinker.ThinkingInfo += Debug.Log;

                    // Keep the thinker in the auto property
                    thinker = aThinker;
                }

                // Return the underlying thinker
                return thinker;
            }
        }

        // Private reference to the underlying thinker instance
        private IThinker thinker;

        /// <summary>
        /// A string representation of the underlying AI thinker.
        /// </summary>
        /// <returns>
        /// A string representation of the underlying AI thinker.
        /// </returns>
        public override string ToString() => Thinker.ToString();
    }
}
