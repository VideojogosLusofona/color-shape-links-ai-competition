/// @file
/// @brief This file contains the ::ColorShapeLinks.Common.AI.IThinker
/// interface.
///
/// @author Nuno Fachada
/// @date 2019-2021
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;
using System.Threading;

namespace ColorShapeLinks.Common.AI
{
    /// <summary>
    /// The actual AI code of specific thinkers should be placed in classes
    /// which implement this interface.
    /// </summary>
    public interface IThinker
    {
        /// <summary>Perform a move.</summary>
        /// <param name="board">The game board.</param>
        /// <param name="ct">A cancellation token.</param>
        /// <returns>The move to be performed.</returns>
        /// <remarks>
        /// The main thread may ask the thinker to stop *thinking*. As such,
        /// this method should frequently test if a cancellation request was
        /// made to the cancellation token (<paramref name="ct"/>). If so,
        /// it should return immediately with no move performed, as
        /// exemplified in the following code:
        /// <code>
        /// if (ct.IsCancellationRequested) return FutureMove.NoMove;
        /// </code>
        /// </remarks>
        FutureMove Think(Board board, CancellationToken ct);

        /// <summary>
        /// Event raised when thinkers produce information while thinking.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item><description>
        /// Listeners receive a string containing the thinking information.
        /// </description></item>
        /// <item><description>
        /// It is not mandatory that thinkers produce any information while
        /// thinking.
        /// </description></item>
        /// </list>
        /// </remarks>
        event Action<string> ThinkingInfo;
    }
}
