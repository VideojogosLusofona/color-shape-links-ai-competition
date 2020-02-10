/// @file
/// @brief This file contains the ::IGameConfig interface.
///
/// @author Nuno Fachada
/// @date 2019, 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

namespace ColorShapeLinks.Common.AI
{
    public interface IGameConfig
    {
        int Rows { get; }
        int Cols { get; }
        int WinSequence { get; }
        int RoundPiecesPerPlayer { get; }
        int SquarePiecesPerPlayer { get; }
        int TimeLimitMillis { get; }
    }
}
