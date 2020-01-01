/// @file
/// @brief This file contains the ::LoserSleeperAIThinker class.
///
/// @author Nuno Fachada
/// @date 2019
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System.Threading;

/// <summary>
/// Implementation of an AI that will always lose because it takes too long to
/// play.
/// </summary>
public class LoserSleeperAIThinker : IThinker
{
    /// @copydoc IThinker.Think
    /// <seealso cref="IThinker.Think"/>
    public FutureMove Think(Board board, CancellationToken ct)
    {
        // Is this task to be cancelled?
        while (true)
        {
            // Wait a few millisseconds
            Thread.Sleep(100);

            // The task will eventually be cancelled due to a timeout
            if (ct.IsCancellationRequested) break;
        }

        // Eventually return a "no move"
        return FutureMove.NoMove;
    }
}
