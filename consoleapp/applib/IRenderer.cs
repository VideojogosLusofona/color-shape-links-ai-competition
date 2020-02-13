/// @file
/// @brief This file contains the ::IRenderer interface.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System.Collections.Generic;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;

namespace ColorShapeLinks.ConsoleAppLib
{
    /// <summary>
    /// Interface to be implemented by renderers of the ColorShapeLinks console
    /// app.
    /// </summary>
    public interface IRenderer
    {
        /// <summary>
        /// Notifies the renderer that the board has been updated.
        /// </summary>
        /// <param name="board">The game board.</param>
        void UpdateBoard(Board board);

        /// <summary>
        /// Notifies the renderer that the next turn is about to start.
        /// </summary>
        /// <param name="playerColor">
        /// Color of the player that is playing in the next turn.
        /// </param>
        /// <param name="playerName">
        /// Name of the player that is playing in the next turn.
        /// </param>
        void NextTurn(PColor playerColor, string playerName);

        /// <summary>
        /// Notifies the renderer that a given player took too long to play,
        /// and will therefore lose the match.
        /// </summary>
        /// <param name="playerColor">
        /// Color of the player that took too long to play.
        /// </param>
        /// <param name="playerName">
        /// Name of the player that took too long to play.
        /// </param>
        void TooLong(PColor playerColor, string playerName);

        /// <summary>
        /// Notifies the renderer that a given player made a move.
        /// </summary>
        /// <param name="playerColor">
        /// Color of the player that made a move.
        /// </param>
        /// <param name="playerName">
        /// Name of the player that made a move.
        /// </param>
        /// <param name="move">
        /// Move performed.
        /// </param>
        void Move(PColor playerColor, string playerName, FutureMove move);

        /// <summary>
        /// Notifies the renderer that the match is over.
        /// </summary>
        /// <param name="winner">Match result.</param>
        /// <param name="solution">Winning solution, if any.</param>
        /// <param name="playerNames">
        /// List with player names, with White player at index 0, and Red
        /// player at index 1.
        /// </param>
        void MatchOver(
            Winner winner, ICollection<Pos> solution,
            IList<string> playerNames);

        /// <summary>
        /// Notify renderer of new information regarding current turn.
        /// </summary>
        /// <param name="turnInfo">
        /// A collection of strings providing new information about the current
        /// turn.
        /// </param>
        void UpdateTurnInfo(ICollection<string> turnInfo);
    }
}
