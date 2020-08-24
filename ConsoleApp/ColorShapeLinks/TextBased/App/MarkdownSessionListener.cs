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
    /// Session event listener which outputs session information to the
    /// standard output in Markdown format.
    /// </summary>
    public class MarkdownSessionListener : ISessionListener
    {
        /// @copydoc ColorShapeLinks.TextBased.Lib.ISessionListener.ListenTo
        /// <seealso cref="ColorShapeLinks.TextBased.Lib.ISessionListener.ListenTo"/>
        public void ListenTo(ISessionSubject subject)
        {
            subject.AfterSession += AfterSession;
        }

        // ///////////////////////////////// //
        // Methods for listening to sessions //
        // ///////////////////////////////// //

        // Outputs the final standings and results after the session is over
        private void AfterSession(ISessionDataProvider sessionData)
        {
            int i = 0;

            // Header and update time
            Console.WriteLine("# Standings");
            Console.WriteLine();
            Console.WriteLine($"Last update: {DateTime.Now.ToString("R")}");
            Console.WriteLine();

            // Classification
            Console.WriteLine("## Classification");
            Console.WriteLine();
            Console.WriteLine("| Pos. | AI Thinker | Points |");
            Console.WriteLine("|:----:| ---------- | -----: |");
            foreach (KeyValuePair<string, int> tp in sessionData.Standings)
            {
                Console.WriteLine($"| {++i} | {tp.Key} | {tp.Value} |");
            }
            Console.WriteLine();

            // Results
            Console.WriteLine("## Results");
            Console.WriteLine();
            Console.WriteLine("_Winner, if any, shown in bold_");
            Console.WriteLine();
            Console.WriteLine("| White |   Red   |");
            Console.WriteLine("| -----:|:------- |");
            foreach (KeyValuePair<Match, Winner> mw in sessionData.Results)
            {
                string white = $"`{mw.Key.thinkerWhite}`";
                string red = $"`{mw.Key.thinkerRed}`";
                if (mw.Value == Winner.White) white = $"**{white}**";
                else if (mw.Value == Winner.Red) red = $"**{red}**";
                Console.WriteLine($"| {white} | {red} |");
            }
        }
    }
}
