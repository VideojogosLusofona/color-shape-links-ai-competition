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
    /// <summary>Base class for AI player configuration classes.</summary>
    /// <remarks>
    /// Concrete AIs should extend this class and implement its abstract members.
    /// The child class should then be added as a component of the
    /// `SessionConfiguration` game object in the Unity editor.
    /// </remarks>
    public class AIPlayer : MonoBehaviour, IPlayer
    {
        /// <summary>
        /// Unity Editor variable which defines if the is active.
        /// </summary>
        [SerializeField] private bool isActive = true;

        /// <summary>
        /// Selected AI thinker.
        /// </summary>
        [SerializeField][AIList] private string selectedAI = null;

        /// <summary>
        /// String containing AI thinker-specific parameters.
        /// </summary>
        [SerializeField] private string aiConfig = "";

        /// <summary> Is the AI active?</summary>
        /// <value>`true` if the AI is active, `false` otherwise.</value>
        public bool IsActive => isActive;

        /// <summary>Is the player human?</summary>
        /// <value>Always `false`.</value>
        /// <seealso cref="IPlayer.IsHuman"/>
        public bool IsHuman => false;

        /// <summary>The player's thinker.</summary>
        /// <value>An instance of <see cref="IThinker"/>.</value>
        /// <seealso cref="IPlayer.Thinker"/>
        public IThinker Thinker { get; private set; }

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        /// <remarks>
        /// Extending classes may override this method for their own awake-time
        /// configuration, but in doing so they must invoke the parent `Awake()`
        /// (this method) as follows:
        ///
        /// ```cs
        /// base.Awake();
        /// ```
        /// </remarks>
        protected void Awake()
        {
            IGameConfig gameConfig = GetComponent<IGameConfig>();

            Thinker = AIManager.Instance.NewThinker(
                selectedAI, gameConfig, aiConfig);
        }

        /// <summary>
        /// A string representation of this player.
        /// </summary>
        /// <returns>A string representation of this player.</returns>
        public override string ToString() => Thinker.ToString();
    }
}
