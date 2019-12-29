/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

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

    protected virtual void Awake()
    {
        AITimeLimit = GetComponentInParent<SessionController>().AITimeLimit;
    }
}
