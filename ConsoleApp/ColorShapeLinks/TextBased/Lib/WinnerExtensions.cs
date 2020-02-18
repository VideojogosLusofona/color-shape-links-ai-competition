/// @file
/// @brief This file contains the
/// ::ColorShapeLinks.TextBased.Lib.WinnerExtensions class.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using ColorShapeLinks.Common;

namespace ColorShapeLinks.TextBased.Lib
{
    /// <summary>Extension methods for the <see cref="Winner"/> enum.</summary>
    public static class WinnerExtensions
    {
        /// <summary>
        /// Converts a <see cref="Winner"/> instance into an
        /// <see cref="ExitStatus"/> for the console application.
        /// </summary>
        /// <param name="winner">A <see cref="Winner"/> instance.</param>
        /// <returns>
        /// An <see cref="ExitStatus"/> for the console application.
        /// </returns>
        public static ExitStatus ToExitStatus(this Winner winner)
        {
            // The exit status
            ExitStatus exitStatus;

            // Find an exit status compatible with the match winner
            switch (winner)
            {
                case Winner.Draw:
                    exitStatus = ExitStatus.Draw;
                    break;
                case Winner.White:
                    exitStatus = ExitStatus.WhiteWins;
                    break;
                case Winner.Red:
                    exitStatus = ExitStatus.RedWins;
                    break;
                default:
                    exitStatus= ExitStatus.Exception;
                    break;
            }

            return exitStatus;
        }
    }
}
