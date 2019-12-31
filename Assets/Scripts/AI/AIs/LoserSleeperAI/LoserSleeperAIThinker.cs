/// @file
/// @brief This file contains the ::LoserSleeperAIThinker class.
///
/// @author Nuno Fachada
/// @date 2019
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System.Threading;

public class LoserSleeperAIThinker : IThinker
{

    public FutureMove Think(Board board, CancellationToken ct)
    {
        // Is this task to be cancelled?
        while (true)
        {
            Thread.Sleep(100);
            if (ct.IsCancellationRequested) break;
        }
        return FutureMove.NoMove;
    }
}
