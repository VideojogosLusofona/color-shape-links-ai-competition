/// @file
/// @brief This file contains the ::HumanThinker class.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;
using System.Threading;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;

namespace ColorShapeLinks.ConsoleApp
{
    public class HumanThinker : AbstractThinker
    {
        private int selectedCol;
        private PShape selectedShape;

        public override void Setup(string str)
        {
            selectedCol = Cols / 2;
            selectedShape = PShape.Round;
        }

        /// @copydoc IThinker.Think
        /// <seealso cref="IThinker.Think"/>
        public override FutureMove Think(Board board, CancellationToken ct)
        {

            FutureMove move = FutureMove.NoMove;
            DateTime timeLimit =
                DateTime.Now + TimeSpan.FromMilliseconds(TimeLimitMillis);
            DateTime lastUpdateTime = DateTime.MinValue;
            TimeSpan frameUpdate = TimeSpan.FromMilliseconds(20);

            if (board.PieceCount(board.Turn, PShape.Round) == 0)
                selectedShape = PShape.Square;
            if (board.PieceCount(board.Turn, PShape.Square) == 0)
                selectedShape = PShape.Round;

            while (board.IsColumnFull(selectedCol))
            {
                selectedCol++;
                if (selectedCol >= Cols)
                    selectedCol = 0;
            }

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKey key = Console.ReadKey().Key;
                    if (key == ConsoleKey.RightArrow
                        || key == ConsoleKey.D
                        || key == ConsoleKey.NumPad6)
                    {
                        do
                        {
                            selectedCol++;
                            if (selectedCol >= Cols)
                                selectedCol = 0;
                        } while (board.IsColumnFull(selectedCol));
                    }
                    else if (key == ConsoleKey.LeftArrow
                        || key == ConsoleKey.A
                        || key == ConsoleKey.NumPad4)
                    {
                        do
                        {
                            selectedCol--;
                            if (selectedCol < 0)
                                selectedCol = Cols - 1;
                        } while (board.IsColumnFull(selectedCol));
                    }
                    else if (key == ConsoleKey.T)
                    {
                        if (selectedShape == PShape.Round
                            && board.PieceCount(board.Turn, PShape.Square) > 0)
                        {
                            selectedShape = PShape.Square;
                        }
                        else if (selectedShape == PShape.Square
                            && board.PieceCount(board.Turn, PShape.Round) > 0)
                        {
                            selectedShape = PShape.Round;
                        }
                    }
                    else if (key == ConsoleKey.Enter)
                    {
                        move = new FutureMove(selectedCol, selectedShape);
                        break;
                    }
                }

                if (ct.IsCancellationRequested)
                {
                    break;
                }

                if (DateTime.Now > lastUpdateTime + frameUpdate)
                {
                    OnThinkingInfo(new string[] {
                        $"< > : Column [{selectedCol,8}] selected   ",
                        $" T  : Piece  [{selectedShape,8}] selected  ",
                        $"    : Time to play: {timeLimit - DateTime.Now, 14}"
                    });

                    lastUpdateTime = DateTime.Now;
                }
            }

            return move;
        }

    }
}
