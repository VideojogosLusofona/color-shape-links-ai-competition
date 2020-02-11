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
    public interface IRenderer
    {
        void UpdateBoard(Board board);
        void NextTurn(PColor playerColor, string playerName);
        void TooLong(PColor playerColor, string playerName);
        void Move(PColor playerColor, string playerName, FutureMove move);
        void Result(
            Winner winner, ICollection<Pos> solution, IList<string> playerNames);
        void UpdateTurnInfo(ICollection<string> turnInfo);
    }
}
