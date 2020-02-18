/// @file
/// @brief This file contains the ::ColorShapeLinks.TextBased.Lib.ExitStatus
/// enum.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

namespace ColorShapeLinks.TextBased.Lib
{
    /// <summary>
    /// Possible exit status of the ColorShapeLinks console app.
    /// </summary>
    public enum ExitStatus
    {
        /// <summary>
        /// Exit status returned by app when match ends in a draw.
        /// </summary>
        Draw = 0,

        /// <summary>
        /// Exit status returned by app when white (player 1) wins match.
        /// </summary>
        WhiteWins = 1,

        /// <summary>
        /// Exit status returned by app when red (player 2) wins match.
        /// </summary>
        RedWins = 2,

        /// <summary>
        /// Exit status returned by app when a session (i.e., a tournament),
        /// was successfully completed.
        /// </summary>
        Session = 3,

        /// <summary>
        /// Exit status returned by app when debug info was requested and no
        /// game was played.
        /// </summary>
        Info = 4,

        /// <summary>
        /// Exit status returned by app when an unrecoverable exception is
        /// thrown.
        /// </summary>
        Exception = 5
    }

}
