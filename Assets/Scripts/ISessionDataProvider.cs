/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System.Collections.Generic;

public interface ISessionDataProvider
{
    SessionState State { get; }
    string PlayerWhite { get; }
    string PlayerRed { get; }
    IEnumerable<KeyValuePair<Match, Winner>> Matches { get; }
    IEnumerable<KeyValuePair<IPlayer, int>> Standings { get; }
    Winner LastMatchResult { get; }
    string WinnerString { get; }
    bool ShowListOfMatches { get; }
    bool ShowTournamentStandings { get; }
    bool WhoPlaysFirst { get; }
    bool BlockStartNextMatch { get; }
    bool BlockShowResult { get; }
    float NoMatchScreenDuration { get; }
}
