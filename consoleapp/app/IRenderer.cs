using System.Collections.Generic;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;

namespace ColorShapeLinks.ConsoleApp
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
