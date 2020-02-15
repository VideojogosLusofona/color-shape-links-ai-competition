/// @file
/// @brief This file contains the ::ColorShapeLinks.Common.AI.IThinker
/// interface.
///
/// @author Nuno Fachada
/// @date 2019, 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;
using System.Threading;
using System.Collections.Generic;

namespace ColorShapeLinks.Common.AI
{
    /// <summary>
    /// The actual AI code of specific AIs should be placed in classes which
    /// implement this interface.
    /// </summary>
    public interface IThinker
    {
        /// <summary>Perform a move.</summary>
        /// <param name="board">The game board.</param>
        /// <param name="ct">A cancellation token.</param>
        /// <returns>The move to be performed.</returns>
        /// <remarks>
        /// The main thread may ask the AI to stop *thinking*. As such, this
        /// method should frequently test if a cancellation request was made to
        /// the cancellation token (<paramref name="ct"/>). If so, it should
        /// return immediately with no move performed, as exemplified in the
        /// following code:
        ///
        /// ```cs
        /// if (ct.IsCancellationRequested) return FutureMove.NoMove;
        /// ```
        /// </remarks>
        FutureMove Think(Board board, CancellationToken ct);

        /// <summary>
        /// Event raised when thinkers produce information while thinking.
        /// </summary>
        /// <remarks>
        /// * Listeners receive a string containing the thinking information.
        /// * It is not mandatory that thinkers produce any information while
        ///   thinking.
        /// </remarks>
        event Action<string> ThinkingInfo;
    }
}
