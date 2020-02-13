/// @file
/// @brief This file contains the ::PlainRenderer class.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;
using System.Collections.Generic;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;
using ColorShapeLinks.ConsoleAppLib;

namespace ColorShapeLinks.ConsoleApp
{
    public class SimpleRenderingListener : IMatchListener
    {
        private int turn;
        private bool turnInfoShown;
        private int vertCursorPos;

        private string PlayerString(PColor playerColor, string playerName) =>
            $"Player {(int)playerColor + 1} ({playerColor}, {playerName})";

        public SimpleRenderingListener()
        {
            turn = 0;
            turnInfoShown = false;
        }

        public void ListenTo(IMatchSubject subject)
        {
            subject.BoardUpdate += UpdateBoard;
            subject.NextTurn += NextTurn;
            subject.TooLong += TooLong;
            subject.MovePerformed += Move;
            subject.MatchOver += MatchOver;
            subject.TurnInfo += TurnInfo;
        }

        private void UpdateBoard(Board board)
        {
            for (int r = board.rows - 1; r >= 0; r--)
            {
                for (int c = 0; c < board.cols; c++)
                {
                    char pc = '.';
                    Piece? p = board[r, c];
                    if (p.HasValue)
                    {
                        if (p.Value.Is(PColor.White, PShape.Round))
                            pc = 'w';
                        else if (p.Value.Is(PColor.White, PShape.Square))
                            pc = 'W';
                        else if (p.Value.Is(PColor.Red, PShape.Round))
                            pc = 'r';
                        else if (p.Value.Is(PColor.Red, PShape.Square))
                            pc = 'R';
                        else
                            Console.Error.WriteLine($"Invalid piece {p.Value}");
                    }
                    Console.Write(pc);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        private void NextTurn(PColor playerColor, string playerName)
        {
            Console.WriteLine($"{PlayerString(playerColor, playerName)} turn");
            turn++;
            turnInfoShown = false;
        }

        private void TooLong(PColor playerColor, string playerName)
        {
            Console.WriteLine(PlayerString(playerColor, playerName)
                + " took too long to play!");
        }

        private void Move(PColor playerColor, string playerName, FutureMove move)
        {
            Console.WriteLine(PlayerString(playerColor, playerName)
                + $" placed a {move.shape} piece at column {move.column}");
        }
        private void MatchOver(
            Winner winner, ICollection<Pos> solution, IList<string> playerNames)
        {
            if (winner == Winner.Draw)
            {
                Console.WriteLine("Game ended in a draw");
            }
            else
            {
                PColor winnerColor = winner.ToPColor();
                int winnerIdx = (int)winnerColor;
                string winnerName = playerNames[winnerIdx];

                Console.WriteLine(
                    $"Winner is {PlayerString(winnerColor, winnerName)}");
                if (solution != null)
                {
                    Console.Write("Solution=");
                    foreach (Pos pos in solution)
                    {
                        Console.Write(pos);
                    }
                    Console.WriteLine();
                }
            }
        }

        private void TurnInfo(ICollection<string> turnInfo)
        {
            if (turnInfoShown)
            {
                Console.SetCursorPosition(0, vertCursorPos - turnInfo.Count);
            }

            foreach (string info in turnInfo)
            {
                Console.WriteLine(info);
            }

            if (!turnInfoShown)
            {
                vertCursorPos = Console.CursorTop;
                turnInfoShown = true;
            }
        }
    }
}
