/// @file
/// @brief This file contains the
/// ::ColorShapeLinks.TextBased.Lib.IMatchSubject interface.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;
using System.Collections.Generic;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;
using ColorShapeLinks.Common.Session;

namespace ColorShapeLinks.TextBased.Lib
{
    /// <summary>
    /// Interface defining the events raised in a match of ColorShapeLinks.
    /// </summary>
    public interface IMatchSubject
    {
        /// <summary>
        /// Event raised when the match is about to start.
        /// </summary>
        /// <remarks>
        /// * The ::ColorShapeLinks.Common.Session.IMatchConfig type parameter
        ///   contains the match configuration.
        /// * The `IList<string>` type parameter contains a list of player
        ///   names, with White player at index 0, and Red player at index 1.
        /// </remarks>
        event Action<IMatchConfig, IList<string>> MatchStart;

        /// <summary>
        /// Event raised when the board is updated.
        /// </summary>
        /// <remarks>
        /// * The ::ColorShapeLinks.Common.Board type parameter represents the
        ///   game board.
        /// </remarks>
        event Action<Board> BoardUpdate;

        /// <summary>
        /// Event raised when the next turn is about to start.
        /// </summary>
        /// <remarks>
        /// * The ::ColorShapeLinks.Common.PColor type parameter is the color
        ///   of the player that is playing in the next turn.
        /// * The `string` type parameter is the name of the player that is
        ///   playing in the next turn.
        /// </remarks>
        event Action<PColor, string> NextTurn;

        /// <summary>
        /// Event raised when a given player performed an invalid play, for
        /// example taking too long to play, returning an invalid move or
        /// causing or throwing an exception.
        /// </summary>
        /// <remarks>
        /// * The ::ColorShapeLinks.Common.PColor type parameter is the color
        ///   of the player that made an invalid play.
        /// * The first `string` type parameter is the name of the player that
        ///   made an invalid play.
        /// * The second `string` type parameter is the description of the
        ///   invalid play.
        /// </remarks>
        event Action<PColor, string, string> InvalidPlay;

        /// <summary>
        /// Event raised when a given player makes a move.
        /// </summary>
        /// <remarks>
        /// * The ::ColorShapeLinks.Common.PColor type parameter is the color
        ///   of the player that made a move.
        /// * The `string` type parameter is the name of the player that
        ///   made a move.
        /// * The ::ColorShapeLinks.Common.AI.FutureMove type parameter is the
        ///   move performed.
        /// * The `int` time parameter is the move thinking time in
        ///   milliseconds.
        /// </remarks>
        event Action<PColor, string, FutureMove, int> MovePerformed;

        /// <summary>
        /// Event raised when the match is over.
        /// </summary>
        /// <remarks>
        /// * The ::ColorShapeLinks.Common.Winner type parameter contains the
        ///   match result.
        /// * The `ICollection<::ColorShapeLinks.Common.Pos>` type parameter
        ///   contains the winning solution, if any.
        /// * The `IList<string>` type parameter contains a list of player
        ///   names, with White player at index 0, and Red player at index 1.
        /// </remarks>
        event Action<Winner, ICollection<Pos>, IList<string>> MatchOver;
    }
}
