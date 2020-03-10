/// @file
/// @brief This file contains the ::ColorShapeLinks.Common.Session.MatchConfig
/// class.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

namespace ColorShapeLinks.Common.Session
{
    /// @copybrief ColorShapeLinks.Common.Session.IMatchConfig
    /// <remarks>
    /// This is a helper class which can used for testing AI thinkers in
    /// isolation, simplifying the instantiation of thinkers using
    /// @ref ColorShapeLinks.Common.AI.ThinkerPrototype "ThinkerPrototype"s,
    /// outside the context of matches and sessions.
    /// </remarks>
    /// <seealso cref="ColorShapeLinks.Common.Session.IMatchConfig"/>
    public class MatchConfig : IMatchConfig
    {
        /// @copydoc ColorShapeLinks.Common.Session.IMatchConfig.Rows
        /// <seealso cref="ColorShapeLinks.Common.Session.IMatchConfig.Rows"/>
        public int Rows { get; }

        /// @copydoc ColorShapeLinks.Common.Session.IMatchConfig.Cols
        /// <seealso cref="ColorShapeLinks.Common.Session.IMatchConfig.Cols"/>
        public int Cols { get; }

        /// @copydoc ColorShapeLinks.Common.Session.IMatchConfig.WinSequence
        /// <seealso cref="ColorShapeLinks.Common.Session.IMatchConfig.WinSequence"/>
        public int WinSequence { get; }

        /// @copydoc ColorShapeLinks.Common.Session.IMatchConfig.RoundPiecesPerPlayer
        /// <seealso cref="ColorShapeLinks.Common.Session.IMatchConfig.RoundPiecesPerPlayer"/>
        public int RoundPiecesPerPlayer { get; }

        /// @copydoc ColorShapeLinks.Common.Session.IMatchConfig.SquarePiecesPerPlayer
        /// <seealso cref="ColorShapeLinks.Common.Session.IMatchConfig.SquarePiecesPerPlayer"/>
        public int SquarePiecesPerPlayer { get; }

        /// @copydoc ColorShapeLinks.Common.Session.IMatchConfig.TimeLimitMillis
        /// <seealso cref="ColorShapeLinks.Common.Session.IMatchConfig.TimeLimitMillis"/>
        public int TimeLimitMillis { get; }

        /// @copydoc ColorShapeLinks.Common.Session.IMatchConfig.TimeLimitSeconds
        /// <seealso cref="ColorShapeLinks.Common.Session.IMatchConfig.TimeLimitSeconds"/>
        public float TimeLimitSeconds { get; }

        /// @copydoc ColorShapeLinks.Common.Session.IMatchConfig.MinMoveTimeSeconds
        /// <seealso cref="ColorShapeLinks.Common.Session.IMatchConfig.MinMoveTimeSeconds"/>
        public float MinMoveTimeSeconds { get; }

        /// @copydoc ColorShapeLinks.Common.Session.IMatchConfig.MinMoveTimeMillis
        /// <seealso cref="ColorShapeLinks.Common.Session.IMatchConfig.MinMoveTimeMillis"/>
        public int MinMoveTimeMillis { get; }

        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        /// <param name="rows">
        /// @copybrief Rows
        /// </param>
        /// <param name="cols">
        /// @copybrief Cols
        /// </param>
        /// <param name="winSequence">
        /// @copybrief WinSequence
        /// </param>
        /// <param name="roundPiecesPerPlayer">
        /// @copybrief RoundPiecesPerPlayer
        /// </param>
        /// <param name="squarePiecesPerPlayer">
        /// @copybrief SquarePiecesPerPlayer
        /// </param>
        /// <param name="timeLimitMillis">
        /// @copybrief TimeLimitMillis
        /// </param>
        /// <param name="minMoveTimeMillis">
        /// @copybrief MinMoveTimeMillis
        /// </param>
        public MatchConfig(int rows = 6, int cols = 7, int winSequence = 4,
            int roundPiecesPerPlayer = 10, int squarePiecesPerPlayer = 11,
            int timeLimitMillis = 3600000, int minMoveTimeMillis = 0)
        {
            Rows = rows;
            Cols = cols;
            WinSequence = winSequence;
            RoundPiecesPerPlayer = roundPiecesPerPlayer;
            SquarePiecesPerPlayer = squarePiecesPerPlayer;
            TimeLimitMillis = timeLimitMillis;
            TimeLimitSeconds = timeLimitMillis / 1000.0f;
            MinMoveTimeMillis = minMoveTimeMillis;
            MinMoveTimeSeconds = minMoveTimeMillis / 1000.0f;
        }
    }
}
