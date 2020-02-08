/// @file
/// @brief This file contains the ::IThinker interface.
///
/// @author Nuno Fachada
/// @date 2019
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System.Threading;

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
    /// The main thread may ask the AI to stop *thinking*. As such, this method
    /// should frequently test if a cancellation request was made to the
    /// cancellation token (<paramref name="ct"/>). If so, it should return
    /// immediately with no move performed, as exemplified in the following
    /// code:
    ///
    /// ```cs
    /// if (ct.IsCancellationRequested) return FutureMove.NoMove;
    /// ```
    /// </remarks>
    FutureMove Think(Board board, CancellationToken ct);
}
