/// @file
/// @brief This file contains the ::ColorShapeLinks.Common.Board class.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System.Collections.Generic;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.Session;
using Xunit;

namespace Tests.Common
{

    public class BoardTests
    {
        private static readonly PShape o = PShape.Round;
        private static readonly PShape s = PShape.Square;

        private static readonly object[][] boardData =
        {
            // Winner is red, standard board
            new object[] {
                new MatchConfig(),
                new (int, PShape)[] {
                    (3, o), (3, o), (1, o), (2, o), (0, o), (2, o), (3, o),
                    (1, o), (4, o), (1, s), (1, s), (0, o), (0, s),
                    (0, s) },
                new (int, PShape)[] {
                    (3, s), (3, s), (2, s), (2, s), (2, s), (3, s), (1, s),
                    (1, s), (6, s), (4, s), (5, o), (4, s), (2, o) },
                Winner.Red,
                new Pos[] {
                    new Pos(3, 0), new Pos(3, 1), new Pos(3, 2), new Pos(3, 3) }
            },

            // Very small board, winner is white on the last possible move
            // with a full board
            new object[] {
                new MatchConfig(rows: 2, cols: 3, winSequence: 3),
                new (int, PShape)[] {
                    (0, o), (1, o), (2, o) },
                new (int, PShape)[] {
                    (1, s), (2, s), (0, o) },
                Winner.White,
                new Pos[] {
                    new Pos(1, 0), new Pos(1, 1), new Pos(1, 2) }
            },

            // Very small board, final result is a tie
            new object[] {
                new MatchConfig(rows: 2, cols: 3, winSequence: 3),
                new (int, PShape)[] {
                    (0, o), (1, o), (2, o) },
                new (int, PShape)[] {
                    (1, s), (2, s), (0, s) },
                Winner.Draw,
                new Pos[3] }
        };

        public static IEnumerable<object[]> GetBoardsNoSolution()
        {
            foreach (object[] bDatum in boardData)
            {
                yield return new object[] {
                    bDatum[0], bDatum[1], bDatum[2], bDatum[3] };
            }
        }

        public static IEnumerable<object[]> GetBoardsWithSolution()
        {
            foreach (object[] bDatum in boardData)
            {
                yield return bDatum;
            }
        }

        [Theory]
        [MemberData(nameof(GetBoardsNoSolution))]
        public void CheckWinner_WithSolution_No(
            MatchConfig mc,
            (int, PShape)[] wMov, (int, PShape)[] rMov,
            Winner win)
        {
            Board board = new Board(mc.Rows, mc.Cols, mc.WinSequence,
                mc.RoundPiecesPerPlayer, mc.SquarePiecesPerPlayer);

            int w = 0;
            int r = 0;

            while (true)
            {
                Assert.Equal(Winner.None, board.CheckWinner());
                if (board.Turn == PColor.White)
                {
                    board.DoMove(wMov[w].Item2, wMov[w].Item1);
                    w++;
                }
                else
                {
                    board.DoMove(rMov[r].Item2, rMov[r].Item1);
                    r++;
                }
                if (w + r == wMov.Length + rMov.Length) break;
            }
            Assert.Equal(win, board.CheckWinner());
        }

        [Theory]
        [MemberData(nameof(GetBoardsWithSolution))]
        public void CheckWinner_WithSolution_Yes(
            MatchConfig mc,
            (int, PShape)[] wMov, (int, PShape)[] rMov,
            Winner win, Pos[] sol)
        {
            Board board = new Board(mc.Rows, mc.Cols, mc.WinSequence,
                mc.RoundPiecesPerPlayer, mc.SquarePiecesPerPlayer);

            int w = 0;
            int r = 0;
            Pos[] testSol = new Pos[mc.WinSequence];

            while (true)
            {
                Assert.Equal(Winner.None, board.CheckWinner(testSol));
                if (board.Turn == PColor.White)
                {
                    board.DoMove(wMov[w].Item2, wMov[w].Item1);
                    w++;
                }
                else
                {
                    board.DoMove(rMov[r].Item2, rMov[r].Item1);
                    r++;
                }
                if (w + r == wMov.Length + rMov.Length) break;
            }
            Assert.Equal(win, board.CheckWinner(testSol));

            foreach (Pos p in sol)
            {
                Assert.Contains(p, testSol);
            }
        }
    }
}
