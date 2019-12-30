/* This Source Code Form is subject to the terms of the Mozilla Public
* License, v. 2.0. If a copy of the MPL was not distributed with this
* file, You can obtain one at http://mozilla.org/MPL/2.0/.
*
* Author: Nuno Fachada
* */

using System;

public struct Match : IComparable<Match>
{
    private static int nextId = 0;
    private readonly int id;
    public readonly IPlayer player1;
    public readonly IPlayer player2;
    public IPlayer this[PColor color] => color == PColor.White ? player1
            : color == PColor.Red ? player2
                : throw new InvalidOperationException(
                    $"Invalid player color");

    public Match Swapped => new Match(player2, player1);
    public Match(IPlayer player1, IPlayer player2)
    {
        id = nextId++;
        this.player1 = player1;
        this.player2 = player2;
    }
    public override string ToString() => $"{player1} vs {player2}";
    public int CompareTo(Match other) => Equals(other) ? 0 : id - other.id;
    public override int GetHashCode() =>
        (player1.ToString() + player2.ToString()).GetHashCode();
    public override bool Equals(object obj)
    {
        Match other;
        if (obj == null || !(obj is Match)) return false;
        other = (Match)obj;
        return player1 == other.player1 && player2 == other.player2;
    }
}
