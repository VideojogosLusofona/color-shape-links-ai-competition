/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;

public class HumanPlayer : IPlayer
{
    public bool IsHuman => true;
    public string PlayerName => "Human";

    public IThinker Thinker =>
        throw new InvalidOperationException("Humans don't need an AI thinker");
}
