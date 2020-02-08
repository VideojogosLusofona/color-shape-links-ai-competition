/// @file
/// @brief This file contains the ::Match struct.
///
/// @author Nuno Fachada
/// @date 2019
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;

/// <summary>
/// Represents one match in a session.
/// </summary>
public struct Match : IComparable<Match>
{
    // Used for getting the next match ID
    private static int nextId = 0;

    // The match ID
    private readonly int id;

    /// <summary>First player in this match (white).</summary>
    public readonly IPlayer player1;

    /// <summary>Second player in this match (ref).</summary>
    public readonly IPlayer player2;

    /// <summary>Indexer for getting a player based on his color.</summary>
    /// <param name="color">Player color.</param>
    /// <returns>The player associated with the given color.</returns>
    /// <exception cref="System.InvalidOperationException">
    /// Thrown when an invalid color is given.
    /// </exception>
    public IPlayer this[PColor color] => color == PColor.White ? player1
            : color == PColor.Red ? player2
                : throw new InvalidOperationException(
                    $"Invalid player color");

    /// <summary>A match with the players swapped.</summary>
    /// <value>A new match instance.</value>
    public Match Swapped => new Match(player2, player1);

    /// <summary>Create a new match.</summary>
    /// <param name="player1">First player in match (white).</param>
    /// <param name="player2">Second player in match (red).</param>
    public Match(IPlayer player1, IPlayer player2)
    {
        // Each new match will have increasing IDs
        id = nextId++;
        // Keep references to players
        this.player1 = player1;
        this.player2 = player2;
    }

    /// <summary>A string representation of this match.</summary>
    /// <returns>A string representation of this match.</returns>
    public override string ToString() => $"{player1} vs {player2}";

    /// <summary>
    /// Compares this match with another match. Comparisons are made using an
    /// internal ID, except if the players are the same and in the same order,
    /// in which case matches are considered equal, even if they have different
    /// internal IDs.
    /// </summary>
    /// <param name="other">The match to compare the current match with.</param>
    /// <returns>
    /// * Zero if players are the same and are in the same order.
    /// * Negative value if current match was created before match given in
    /// <paramref name="other"/>.
    /// * Positive value if current match was created after match given in
    /// <paramref name="other"/>.
    /// </returns>
    public int CompareTo(Match other) => Equals(other) ? 0 : id - other.id;

    /// <summary>Returns a hash code for this match.</summary>
    /// <returns>
    /// Hash code obtain from the string concatenation of the player's names.
    /// </returns>
    public override int GetHashCode() =>
        (player1.ToString() + player2.ToString()).GetHashCode();

    /// <summary>
    /// Is the current match equal to the given object?
    /// Equality is verified if <paramref name="obj"/> is a <see cref="Match"/>
    /// object, and if it contains the same players in the same positions.
    /// </summary>
    /// <param name="obj">Object to check for equality with this match.</param>
    /// <returns>
    /// `true` if this match is equal to <paramref name="obj"/>, `false`
    /// otherwise.
    /// </returns>
    public override bool Equals(object obj)
    {
        Match other;
        if (obj == null || !(obj is Match)) return false;
        other = (Match)obj;
        return player1 == other.player1 && player2 == other.player2;
    }
}
