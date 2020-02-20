/// @file
/// @brief This file contains the
/// ::ColorShapeLinks.Common.Session.ISessionDataProvider interface.
///
/// @author Nuno Fachada
/// @date 2019, 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System.Collections.Generic;

namespace ColorShapeLinks.Common.Session
{
    /// <summary>
    /// Defines a data provider for ColorShapeLinks sessions, which include
    /// one or more matches.
    /// </summary>
    public interface ISessionDataProvider
    {
        /// <summary>State of the current session.</summary>
        /// <value>
        /// One of the values defined in the <see cref="SessionState"/>
        /// enumeration.
        /// </value>
        SessionState State { get; }

        /// <summary>Session configuration.</summary>
        /// <value>
        /// Session configuration consists of points per win, per loss and per
        /// draw.
        /// </value>
        ISessionConfig SessionConfig { get; }

        /// <summary>Match configuration.</summary>
        /// <value>
        /// Match configuration consists of number of board rows and columns,
        /// how many pieces in a row to win, initial number of pieces, etc.
        /// </value>
        IMatchConfig MatchConfig { get; }

        /// <summary>The match currently being played.</summary>
        Match CurrentMatch { get; }

        /// <summary>
        /// All matches played or to be played in current session.
        /// </summary>
        /// <value>Collection of matches.</value>
        IEnumerable<Match> Matches { get; }

        /// <summary>
        /// Results of matches played so far in current session.
        /// </summary>
        /// <value>Collection of match-winner pairs.</value>
        IEnumerable<KeyValuePair<Match, Winner>> Results { get; }

        /// <summary>
        /// Standings (classification, ranking) of thinkers in current session.
        /// </summary>
        /// <value>
        /// Descending ordered collection of thinker-points pairs.
        /// </value>
        IEnumerable<KeyValuePair<string, int>> Standings { get; }

        /// <summary>Result of last match.</summary>
        /// <value>
        /// One of the values of the <see cref="Winner"/> enumeration.
        /// </value>
        Winner LastMatchResult { get; }

        /// <summary>The winner's name and color.</summary>
        /// <value>A string containing the winner's name and color.</value>
        string WinnerString { get; }
    }
}
