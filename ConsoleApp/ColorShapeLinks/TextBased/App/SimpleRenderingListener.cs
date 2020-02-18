/// @file
/// @brief This file contains the
/// ::ColorShapeLinks.TextBased.App.SimpleRenderingListener class.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;
using System.Collections.Generic;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;
using ColorShapeLinks.TextBased.Lib;

namespace ColorShapeLinks.TextBased.App
{
    /// <summary>
    /// Simple match event listener which renders match information on the
    /// console.
    /// </summary>
    public class SimpleRenderingListener :
        IThinkerListener, IMatchListener, ISessionListener
    {
        // Who's playing?
        private PColor turn = PColor.White;

        /// @copydoc ColorShapeLinks.TextBased.Lib.IThinkerListener.ListenTo
        /// <seealso cref="ColorShapeLinks.TextBased.Lib.IThinkerListener.ListenTo"/>
        public void ListenTo(AbstractThinker subject)
        {
            subject.ThinkingInfo += ThinkingInfo;
        }

        /// @copydoc ColorShapeLinks.TextBased.Lib.IMatchListener.ListenTo
        /// <seealso cref="ColorShapeLinks.TextBased.Lib.IMatchListener.ListenTo"/>
        public void ListenTo(IMatchSubject subject)
        {
            subject.BoardUpdate += BoardUpdate;
            subject.NextTurn += NextTurn;
            subject.Timeout += Timeout;
            subject.MovePerformed += MovePerformed;
            subject.MatchOver += MatchOver;
        }

        /// @copydoc ColorShapeLinks.TextBased.Lib.ISessionListener.ListenTo
        /// <seealso cref="ColorShapeLinks.TextBased.Lib.ISessionListener.ListenTo"/>
        public void ListenTo(ISessionSubject subject)
        {

        }

        // Helper method to create a consistent thinker description string
        private string ThinkerDesc(PColor thinkerColor, string thinkerName) =>
            $"Thinker {(int)thinkerColor + 1} ({thinkerColor}, {thinkerName})";

        // Renders the match board as follows:
        // w - Round white pieces
        // W - Square white pieces
        // r - Round red pieces
        // R - Square red pieces
        private void BoardUpdate(Board board)
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
                            throw new ArgumentException(
                                $"Invalid piece '{p.Value}'");
                    }
                    Console.Write(pc);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        // Renders info about the next turn
        private void NextTurn(PColor thinkerColor, string thinkerName)
        {
            Console.WriteLine($"{ThinkerDesc(thinkerColor, thinkerName)} turn");
            turn = thinkerColor;
        }

        // Displays notification that the specified thinker took too long to
        // play
        private void Timeout(PColor thinkerColor, string thinkerName)
        {
            Console.WriteLine(ThinkerDesc(thinkerColor, thinkerName)
                + " took too long to play!");
        }

        // Displays the move performed by the specified thinker
        private void MovePerformed(
            PColor thinkerColor, string thinkerName,
            FutureMove move, int thinkingTime)
        {
            Console.WriteLine(ThinkerDesc(thinkerColor, thinkerName)
                + $" placed a {move.shape} piece at column {move.column}"
                + $" after {thinkingTime}ms");
        }

        // Renders the match over screen, showing the final result
        private void MatchOver(
            Winner winner,
            ICollection<Pos> solution,
            IList<string> thinkerNames)
        {
            // Did the match end in a draw?
            if (winner == Winner.Draw)
            {
                // If so, show that information
                Console.WriteLine("Game ended in a draw");
            }
            else
            {
                // Otherwise, show match results
                PColor winnerColor = winner.ToPColor();
                int winnerIdx = (int)winnerColor;
                string winnerName = thinkerNames[winnerIdx];

                // Show who won
                Console.WriteLine(
                    $"Winner is {ThinkerDesc(winnerColor, winnerName)}");

                // Show the solution, if available
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

        // Show thinking info
        private void ThinkingInfo(string thinkingInfo)
        {
            // Show thinking info
            Console.WriteLine($"{turn} thinker says: {thinkingInfo}");
        }
    }
}
