/// @file
/// @brief This file contains the
/// ::ColorShapeLinks.TextBased.App.MarkdownSessionListener class.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;
using System.Collections.Generic;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.Session;
using ColorShapeLinks.TextBased.Lib;

namespace ColorShapeLinks.TextBased.App
{
    /// <summary>
    /// Session event listener which outputs session information in Markdown
    /// format.
    /// </summary>
    public class MarkdownSessionListener : ISessionListener
    {
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
        // Methods for listening to sessions //
        // ///////////////////////////////// //

        // Lists matches to play before the session starts
        private void BeforeSession(ISessionDataProvider sessionData)
        {
            Console.WriteLine("Matches to play:");
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