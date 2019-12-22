/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;

public abstract class AIPlayer : MonoBehaviour, IPlayer
{
    public bool IsHuman => false;
    public abstract string PlayerName { get; }
    public abstract IThinker Thinker { get; }
}
