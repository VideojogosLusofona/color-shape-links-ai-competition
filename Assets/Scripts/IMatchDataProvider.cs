/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

public interface IMatchDataProvider
{
    Board Board { get; }
    IPlayer CurrentPlayer { get; }
    float AITimeLimit { get; }
    float TimeBetweenAIMoves { get; }
    IPlayer GetPlayer(PColor player);
}
