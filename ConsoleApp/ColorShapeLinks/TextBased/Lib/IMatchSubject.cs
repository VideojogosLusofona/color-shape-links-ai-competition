/// @file
/// @brief This file contains the ::IMatchSubject interface.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;
using System.Collections.Generic;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;

namespace ColorShapeLinks.TextBased.Lib
{
    /// <summary>
    /// Interface defining the events raised in a match of the ColorShapeLinks
    /// console app.
    /// </summary>
    public interface IMatchSubject
    {
        /// <summary>
        /// Event raised when the board is updated.
        /// </summary>
        /// <remarks>
        /// * The ::Board type parameter represents the game board.
        /// </remarks>
        event Action<Board> BoardUpdate;

        /// <summary>
        /// Event raised when the next turn is about to start.
        /// </summary>
        /// <remarks>
        /// * The ::PColor type parameter is the color of the player that
        ///   is playing in the next turn.
        /// * The `string` type parameter is the name of the player that is
        ///   playing in the next turn.
        /// </remarks>
        event Action<PColor, string> NextTurn;

        /// <summary>
        /// Event raised when there is new information regarding the current
        /// turn.
        /// </summary>
        /// <remarks>
        /// * The `ICollection<string>` type parameter contains a collection of
        ///   strings providing new information about the current turn.
        /// </remarks>
        event Action<ICollection<string>> TurnInfo;

        /// <summary>
        /// Event raised when a given player took too long to play,
        /// and will therefore lose the match.
        /// </summary>
        /// <remarks>
        /// * The ::PColor type parameter is the color of the player that
        ///   took too long to play.
        /// * The `string` type parameter is the name of the player that
        ///   took too long to play.
        /// </remarks>
        event Action<PColor, string> Timeout;

        /// <summary>
        /// Event raised when a given player makes a move.
        /// </summary>
        /// <remarks>
        /// * The ::PColor type parameter is the color of the player that
        ///   made a move.
        /// * The `string` type parameter is the name of the player that
        ///   made a move.
        /// * The ::FutureMove type parameter is the move performed.
        /// </remarks>
        event Action<PColor, string, FutureMove> MovePerformed;

        /// <summary>
        /// Event raised when the match is over.
        /// </summary>
        /// <remarks>
        /// * The ::Winner type parameter contains the match result.
        /// * The `ICollection<::Pos>` type parameter contains the winning
        ///   solution, if any.
        /// * The `IList<string>` type parameter contains a list of player
        ///   names, with White player at index 0, and Red player at index 1.
        /// </remarks>
        event Action<Winner, ICollection<Pos>, IList<string>> MatchOver;
    }
}
