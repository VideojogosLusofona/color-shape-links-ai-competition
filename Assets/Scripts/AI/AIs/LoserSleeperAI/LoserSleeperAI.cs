/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

public class LoserSleeperAI : AIPlayer
{
    public override string PlayerName => "LoserSleeperAI";
    public override IThinker Thinker => thinker;

    private IThinker thinker;
    protected override void Awake()
    {
        base.Awake();
        thinker = new LoserSleeperAIThinker();
    }

}
