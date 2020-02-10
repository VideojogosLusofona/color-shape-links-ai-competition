/// @file
/// @brief This file contains the ::LoserSleeperAIThinker class.
///
/// @author Nuno Fachada
/// @date 2019, 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System.Threading;

namespace ColorShapeLinks.Common.AI.Examples
{
    /// <summary>
    /// Implementation of an AI that will always lose because it takes too long
    /// to play.
    /// </summary>
    public class LoserSleeperAIThinker : AbstractThinker
    {
        /// @copydoc IThinker.Think
        /// <seealso cref="IThinker.Think"/>
        public override FutureMove Think(Board board, CancellationToken ct)
        {
            // Is this task to be cancelled?
            while (true)
            {
                // Wait enough millisseconds to lose
                Thread.Sleep(TimeLimitMillis + 1);

                // The task will eventually be cancelled due to a timeout
                if (ct.IsCancellationRequested) break;
            }

            // Eventually return a "no move"
            return FutureMove.NoMove;
        }
    }
}
