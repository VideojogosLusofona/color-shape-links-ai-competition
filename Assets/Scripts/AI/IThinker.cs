/// @file
/// @brief This file contains the ::IThinker interface.
///
/// @author Nuno Fachada
/// @date 2019
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System.Threading;

public interface IThinker
{
    FutureMove Think(Board board, CancellationToken ct);
}
