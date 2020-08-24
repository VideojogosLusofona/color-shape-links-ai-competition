/// @file
/// @brief This file contains the
/// ::ColorShapeLinks.TextBased.Lib.RankingSessionListener class.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System.IO;
using System.Collections.Generic;
using ColorShapeLinks.Common.Session;

namespace ColorShapeLinks.TextBased.Lib
{
    /// <summary>
    /// Session event listener which outputs session final results into a
    /// TSV file.
    /// </summary>
    /// <remarks>
    /// The main purpose of this listener is to save session results in a
    /// simple file which can be opened by a third-party tool, e.g. to be used
    /// in an optimization context.
    /// </remarks>
    public class RankingSessionListener : ISessionListener
    {
        /// <summary>
        /// File to where results will be saved.
        /// </summary>
        public const string RankingFile = "results.tsv";

        /// @copydoc ColorShapeLinks.TextBased.Lib.ISessionListener.ListenTo
        /// <seealso cref="ColorShapeLinks.TextBased.Lib.ISessionListener.ListenTo"/>
        public void ListenTo(ISessionSubject subject)
        {
            subject.AfterSession += AfterSession;
        }

        // ///////////////////////////////// //
        // Methods for listening to sessions //
        // ///////////////////////////////// //

        // Outputs the final ranking after the session is over
        private void AfterSession(ISessionDataProvider sessionData)
        {
            using (StreamWriter outFile = new StreamWriter(RankingFile))
            {
                // Classification
                foreach (KeyValuePair<string, int> tp in sessionData.Standings)
                {
                    outFile.WriteLine($"{tp.Key}\t{tp.Value}");
                }
            }
        }
    }
}
