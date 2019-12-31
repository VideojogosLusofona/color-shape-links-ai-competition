/// @file
/// @brief This file contains the ::AIPlayer class.
///
/// @author Nuno Fachada
/// @date 2019
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using UnityEngine;

public abstract class AIPlayer : MonoBehaviour, IPlayer
{
    [SerializeField] private bool isActive = true;

    public bool IsActive => isActive;
    protected float AITimeLimit { get; private set; }
    public bool IsHuman => false;
    public override string ToString() => PlayerName;
    public abstract string PlayerName { get; }
    public abstract IThinker Thinker { get; }

    public abstract void Setup();

    protected virtual void Awake()
    {
        AITimeLimit = GetComponentInParent<SessionController>().AITimeLimit;
    }
}
