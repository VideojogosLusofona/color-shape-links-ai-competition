/// @file
/// @brief This file contains the ::AIPlayer class.
///
/// @author Nuno Fachada
/// @date 2019
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using UnityEngine;

/// <summary>Base class for AI player configuration classes.</summary>
/// <remarks>
/// Concrete AIs should extend this class and implement its abstract members.
/// The child class should then be added as a component of the
/// `SessionConfiguration` game object in the Unity editor.
/// </remarks>
public abstract class AIPlayer : MonoBehaviour, IPlayer
{
    /// <summary>
    /// Unity Editor variable which defines if the is active.
    /// </summary>
    [SerializeField] private bool isActive = true;

    /// <summary> Is the AI active?</summary>
    /// <value>`true` if the AI is active, `false` otherwise.</value>
    public bool IsActive => isActive;

    /// <summary>The time in seconds available for the AI to play.</summary>
    /// <value>A time interval in seconds.</value>
    protected float AITimeLimit { get; private set; }

    /// <summary>Is the player human?</summary>
    /// <value>Always `false`.</value>
    /// <seealso cref="IPlayer.IsHuman"/>
    public bool IsHuman => false;

    /// <summary>The player's name.</summary>
    /// <value>A string representing the player's name.</value>
    /// <seealso cref="IPlayer.PlayerName"/>
    public abstract string PlayerName { get; }

    /// <summary>The player's thinker.</summary>
    /// <value>An instance of <see cref="IThinker"/>.</value>
    /// <seealso cref="IPlayer.Thinker"/>
    public abstract IThinker Thinker { get; }

    /// <summary>
    /// This method will be called before a match starts.
    /// </summary>
    /// <remarks>
    /// Extending classes must override this method and use it for
    /// instantianting their own implementation of <see cref="IThinker"/>.
    /// </remarks>
    public abstract void Setup();

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
    protected virtual void Awake()
    {
        AITimeLimit = GetComponentInParent<SessionController>().AITimeLimit;
    }

    /// <summary>
    /// A string representation of this player. Equivalent to
    /// <see cref="AIPlayer.PlayerName"/>.
    /// </summary>
    /// <returns>A string representation of this player.</returns>
    public override string ToString() => PlayerName;

}
