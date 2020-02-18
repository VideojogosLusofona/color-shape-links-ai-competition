/// @file
/// @brief This file contains the ::ColorShapeLinks.UnityApp.AIPlayer class.
///
/// @author Nuno Fachada
/// @date 2019, 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using ColorShapeLinks.Common.AI;
using ColorShapeLinks.Common.Session;
using UnityEngine;

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
        [SerializeField] [ThinkerList] private string selectedThinker = null;

        /// <summary>
        /// String containing AI thinker-specific configuration parameters.
        /// </summary>
        [SerializeField] private string thinkerParams = "";

        // A reference to the thinker's prototype
        private ThinkerPrototype thinkerPrototype;

        /// <summary>Is the AI thinker active?</summary>
        /// <value>`true` if the AI is active, `false` otherwise.</value>
        public bool IsActive => isActive;

        /// @copydoc IPlayer.ThinkerFQN
        /// <seealso cref="IPlayer.ThinkerFQN"/>
        public string ThinkerFQN => selectedThinker;

        /// <summary>
        /// A reference to the thinker's prototype.
        /// </summary>
        public ThinkerPrototype ThinkerPrototype
        {
            get
            {
                // The underlying thinker prototype is lazy instantiated
                if (thinkerPrototype == null)
                {
                    // Obtain the component holding the game configuration
                    IMatchConfig matchConfig = GetComponent<IMatchConfig>();

                    thinkerPrototype = new ThinkerPrototype(
                        selectedThinker, thinkerParams, matchConfig);
                }

                // Return the underlying thinker prototype
                return thinkerPrototype;
            }
        }

        /// <summary>
        /// A string representation of the underlying AI thinker.
        /// </summary>
        /// <returns>
        /// In practice, it's the fully qualified name of the thinker class.
        /// </returns>
        public override string ToString() => selectedThinker;
    }
}
