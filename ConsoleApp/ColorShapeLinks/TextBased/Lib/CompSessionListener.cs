/// @file
/// @brief This file contains the
/// ::ColorShapeLinks.TextBased.Lib.CompSessionListener class.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;
using System.IO;
using System.Collections.Generic;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.Session;

namespace ColorShapeLinks.TextBased.Lib
{
    /// <summary>
    /// Session event listener which outputs complete session information in
    /// Markdown format with links text files containing the play outs of each
    /// match.
    /// </summary>
    /// <remarks>
    /// This listener is used to produce the daily standing/results of the
    /// ColorShapeLink competition, since the output can be easily or
    /// automatically converted to HTML.
    /// </remarks>
    public class CompSessionListener : ISessionListener
    {
        /// <summary>
        /// The folder where complete match play outs will be placed.
        /// </summary>
        public const string PlayoutsFolder = "results";

        /// <summary>
        /// The name of the Markdown file containing the standings.
        /// </summary>
        public const string StandingsFile = "standings.md";

        // Current play out file
        private StreamWriter currPlayoutFile;

        // Reference to the standard output
        private TextWriter stdOutput;

        /// @copydoc ColorShapeLinks.TextBased.Lib.ISessionListener.ListenTo
        /// <seealso cref="ColorShapeLinks.TextBased.Lib.ISessionListener.ListenTo"/>
        public void ListenTo(ISessionSubject subject)
        {
            // Register listeners
            subject.BeforeSession += BeforeSession;
            subject.BeforeMatch += BeforeMatch;
            subject.AfterMatch += AfterMatch;
            subject.AfterSession += AfterSession;
        }

        // Derive a unique match play out filename given the match object
        private string MatchPlayoutFileName(Match match)
        {
            string[] separated = match.ToString().Split();
            string joined = string.Join("", separated);
            return $"{joined}.txt";
        }

        // ///////////////////////////////// //
        // Methods for listening to sessions //
        // ///////////////////////////////// //

        // Creates the folder where the match play out files will be placed
        // Called before the session starts
        private void BeforeSession(ISessionDataProvider sessionData)
        {
            Directory.CreateDirectory(PlayoutsFolder);
        }

        // Create a new match play out file and redirects the standard output
        // to that file
        // Called before a match starts
        private void BeforeMatch(Match match)
        {
            currPlayoutFile = new StreamWriter(
                Path.Combine(PlayoutsFolder, MatchPlayoutFileName(match)));
            stdOutput = Console.Out;
            Console.SetOut(currPlayoutFile);
        }

        // Closes the currently open match play out file and resets the
        // standard output to what it was previously
        // Called after a match finishes
        private void AfterMatch(Match match, ISessionDataProvider sessionData)
        {
            Console.SetOut(stdOutput);
            currPlayoutFile.Close();
        }

        // Outputs the final standings and results in Markdown format
        // Called after the session is over
        private void AfterSession(ISessionDataProvider sessionData)
        {
            using (StreamWriter outFile = new StreamWriter(StandingsFile))
            {
                int i = 0;
                IMatchConfig cfg = sessionData.MatchConfig;

                // Header and update time
                outFile.WriteLine("# Standings");
                outFile.WriteLine();
                outFile.WriteLine($"Last update: {DateTime.Now.ToString("R")}");
                outFile.WriteLine();
                outFile.WriteLine("## Configuration");
                outFile.WriteLine();
                outFile.WriteLine("| Parameter      | Value             |");
                outFile.WriteLine("|:-------------- | ----------------: |");
                outFile.WriteLine($"| Rows          | {cfg.Rows}        |");
                outFile.WriteLine($"| Cols          | {cfg.Cols}        |");
                outFile.WriteLine($"| Win sequence  | {cfg.WinSequence} |");
                outFile.WriteLine($"| Round pieces  | {cfg.RoundPiecesPerPlayer}  |");
                outFile.WriteLine($"| Square pieces | {cfg.SquarePiecesPerPlayer} |");
                outFile.WriteLine($"| Time limit    | {cfg.TimeLimitMillis}ms     |");
                outFile.WriteLine();

                // Classification
                outFile.WriteLine("## Classification");
                outFile.WriteLine();
                outFile.WriteLine("| Pos. | AI Thinker | Points |");
                outFile.WriteLine("|:----:| ---------- | -----: |");
                foreach (KeyValuePair<string, int> tp in sessionData.Standings)
                {
                    outFile.WriteLine($"| {++i} | {tp.Key} | {tp.Value} |");
                }
                outFile.WriteLine();

                // Results
                outFile.WriteLine("## Results");
                outFile.WriteLine();
                outFile.WriteLine("_Winner, if any, shown in bold_");
                outFile.WriteLine();
                outFile.WriteLine("| White |   Red   | Details |");
                outFile.WriteLine("| -----:|:------- | :-----: |");
                foreach (KeyValuePair<Match, Winner> mw in sessionData.Results)
                {
                    string white = $"`{mw.Key.thinkerWhite}`";
                    string red = $"`{mw.Key.thinkerRed}`";
                    if (mw.Value == Winner.White) white = $"**{white}**";
                    else if (mw.Value == Winner.Red) red = $"**{red}**";
                    outFile.WriteLine("| {0} | {1} | [+]({2}/{3}) |",
                        white, red, PlayoutsFolder,
                        MatchPlayoutFileName(mw.Key));
                }
            }
        }
    }
}
