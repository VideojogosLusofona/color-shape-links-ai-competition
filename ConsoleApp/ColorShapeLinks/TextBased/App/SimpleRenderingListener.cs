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
using ColorShapeLinks.Common.Session;
using ColorShapeLinks.TextBased.Lib;

namespace ColorShapeLinks.TextBased.App
{
    /// <summary>
    /// Simple event listener which renders thinker, match and session
    /// information on the console.
    /// </summary>
    public class SimpleRenderingListener :
        IThinkerListener, IMatchListener, ISessionListener
    {
        /// @copydoc ColorShapeLinks.TextBased.Lib.IThinkerListener.ListenTo
        /// <seealso cref="ColorShapeLinks.TextBased.Lib.IThinkerListener.ListenTo"/>
        public void ListenTo(IThinker subject)
        {
            subject.ThinkingInfo += ThinkingInfo;
        }

        /// @copydoc ColorShapeLinks.TextBased.Lib.IMatchListener.ListenTo
        /// <seealso cref="ColorShapeLinks.TextBased.Lib.IMatchListener.ListenTo"/>
        public void ListenTo(IMatchSubject subject)
        {
            subject.MatchStart += MatchStart;
            subject.BoardUpdate += BoardUpdate;
            subject.NextTurn += NextTurn;
            subject.InvalidPlay += InvalidMove;
            subject.MovePerformed += MovePerformed;
            subject.MatchOver += MatchOver;
        }

        /// @copydoc ColorShapeLinks.TextBased.Lib.ISessionListener.ListenTo
        /// <seealso cref="ColorShapeLinks.TextBased.Lib.ISessionListener.ListenTo"/>
        public void ListenTo(ISessionSubject subject)
        {
            subject.BeforeSession += BeforeSession;
            subject.AfterSession += AfterSession;
            subject.BeforeMatch += BeforeMatch;
            subject.AfterMatch += AfterMatch;
        }

        // ///////////////////////////////// //
        // Methods for listening to thinkers //
        // ///////////////////////////////// //

        // Show thinking info
        private void ThinkingInfo(string thinkingInfo)
        {
            // Show thinking info
            Console.WriteLine(thinkingInfo);
        }

        // //////////////////////////////// //
        // Methods for listening to matches //
        // //////////////////////////////// //

        // Renders information about the match about to start
        private void MatchStart(
            IMatchConfig matchConfig, IList<string> thinkerNames)
        {
            // Show who's playing
            Console.WriteLine(
                $"=> {thinkerNames[0]} (White) vs {thinkerNames[1]} (Red) <=\n");

            // Show piece legend
            Console.WriteLine("\tw - White Round");
            Console.WriteLine("\tW - White Square");
            Console.WriteLine("\tr - Red   Round");
            Console.WriteLine("\tR - Red   Square");
            Console.WriteLine();
        }

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
            Console.WriteLine($"{thinkerColor.FormatName(thinkerName)} turn");
        }

        // Displays notification that the specified thinker lost due to an
        // invalid play
        private void InvalidMove(
            PColor thinkerColor, string thinkerName, string reason)
        {
            Console.WriteLine(String.Format("{0} loses match! Reason: {1}",
                thinkerColor.FormatName(thinkerName), reason));
        }

        // Displays the move performed by the specified thinker
        private void MovePerformed(
            PColor thinkerColor, string thinkerName,
            FutureMove move, int thinkingTime)
        {
            Console.WriteLine(thinkerColor.FormatName(thinkerName)
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
                    $"Winner is {winnerColor.FormatName(winnerName)}");

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

        // ///////////////////////////////// //
        // Methods for listening to sessions //
        // ///////////////////////////////// //

        // Lists matches to play before the session starts
        private void BeforeSession(ISessionDataProvider sessionData)
        {
            Console.WriteLine("Matches to play:");
            foreach (Match match in sessionData.Matches)
            {
                Console.WriteLine($"\t{match}");
            }
            Console.WriteLine();
        }

        // Shows the final standings after the session is over
        private void AfterSession(ISessionDataProvider sessionData)
        {
            int i = 0;
            Console.WriteLine("\nFinal standings:");
            foreach (KeyValuePair<string, int> tp in sessionData.Standings)
            {
                Console.WriteLine($"\t{++i}. {tp.Key,-20} {tp.Value,8}");
            }
        }

        // Shows what match is about to be played
        private void BeforeMatch(Match match)
        {
            Console.WriteLine($"* {match} now playing...");
        }

        // Shows match result after match is over
        private void AfterMatch(Match match, ISessionDataProvider sessionData)
        {
            string resultStr;
            if (sessionData.LastMatchResult == Winner.Draw)
                resultStr = "It's a draw";
            else
                resultStr = $"Winner is {sessionData.WinnerString}";
            Console.WriteLine($"  - {resultStr}");
        }
    }
}
