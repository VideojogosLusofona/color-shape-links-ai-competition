/// @file
/// @brief This file contains the ::ISessionDataProvider interface.
///
/// @author Nuno Fachada
/// @date 2019
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System.Collections.Generic;

/// <summary>
/// Defines a data provider for *ColorShapeLinks* sessions, which include one
/// or more matches.
/// </summary>
public interface ISessionDataProvider
{
    /// <summary>State of the current session.</summary>
    /// <value>
    /// One of the values defined in the <see cref="SessionState"/> enumeration.
    /// </value>
    SessionState State { get; }

    /// <summary>Name of player playing as white.</summary>
    /// <value>A string containing the player's name.</value>
    string PlayerWhite { get; }

    /// <summary>Name of player playing as red.</summary>
    /// <value>A string containing the player's name.</value>
    string PlayerRed { get; }

    /// <summary>
    /// All matches played or to be played in current session.
    /// </summary>
    /// <value>Collection of matches.</value>
    IEnumerable<Match> Matches { get; }

    /// <summary>Results of matches played so far in current session.</summary>
    /// <value>Collection of match-winner pairs.</value>
    IEnumerable<KeyValuePair<Match, Winner>> Results { get; }

    /// <summary>
    /// Standings (classification, ranking) of players in current session.
    /// </summary>
    /// <value>Descending ordered collection of player-points pairs.</value>
    IEnumerable<KeyValuePair<IPlayer, int>> Standings { get; }

    /// <summary>Result of last match.</summary>
    /// <value>
    /// One of the values of the <see cref="Winner"/> enumeration.
    /// </value>
    Winner LastMatchResult { get; }

    /// <summary>The winner's name and color.</summary>
    /// <value>A string containing the winner's name and color.</value>
    string WinnerString { get; }

    /// <summary>Ask who plays first?</summary>
    /// <value>
    /// `true` if UI is to ask who plays first, `false` otherwise.
    /// </value>
    bool WhoPlaysFirst { get; }

    /// <summary>
    /// Start of next match screen needs to be unlocked with a button?
    /// </summary>
    /// <value>
    /// `true` if start of next match screen needs to be unlocked with a
    /// button, `false` otherwise.
    /// </value>
    bool BlockStartNextMatch { get; }

    /// <summary>
    /// Show result screen needs to be unlocked with a button?
    /// </summary>
    /// <value>
    /// `true` if show result screen needs to be unlocked with a button,
    /// `false` otherwise.
    /// </value>
    bool BlockShowResult { get; }

    /// <summary>
    /// Duration of unblocked screens (start of next match, show result).
    /// </summary>
    /// <value>Duration in seconds.</value>
    float UnblockedScreenDuration { get; }
}
