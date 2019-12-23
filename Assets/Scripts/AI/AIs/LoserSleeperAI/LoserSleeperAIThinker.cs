/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

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
