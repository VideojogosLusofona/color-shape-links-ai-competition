/// @file
/// @brief This file contains the ::ColorShapeLinks.TextBased.App.GameOptions
/// class.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System.Collections.Generic;
using ColorShapeLinks.Common.AI;
using ColorShapeLinks.Common.Session;
using CommandLine;

namespace ColorShapeLinks.TextBased.App
{
    /// <summary>
    /// Command line game options, available to both the "match" and "session"
    /// verb commands.
    /// </summary>
    public abstract class GameOptions : BaseOptions, IMatchConfig
    {
        // Number of board rows
        private readonly int rows;

        // Number of board cols
        private readonly int cols;

        // Number of pieces in sequence for winning the game
        private readonly int winSequence;

        // Number of initial round pieces per player
        private readonly int roundPiecesPerPlayer;

        // Number of initial square pieces per player
        private readonly int squarePiecesPerPlayer;

        // Time limit for thinking in milliseconds
        private readonly int timeLimitMillis;

        // Minimum apparent move time in milliseconds
        private readonly int minMoveTimeMillis;

        // Thinker listeners
        private readonly IEnumerable<string> thinkerListeners;

        // Match listeners
        private readonly IEnumerable<string> matchListeners;

        /// <summary>
        /// Create a new instance of game options.
        /// </summary>
        /// <param name="rows">Number of board rows.</param>
        /// <param name="cols">Number of board columns.</param>
        /// <param name="winSequence">
        /// Number of pieces in sequence for winning the game.
        /// </param>
        /// <param name="roundPiecesPerPlayer">
        /// Number of initial round pieces per player.
        /// </param>
        /// <param name="squarePiecesPerPlayer">
        /// Number of initial square pieces per player.
        /// </param>
        /// <param name="timeLimitMillis">
        /// Time limit for thinking in milliseconds.
        /// </param>
        /// <param name="minMoveTimeMillis">
        /// Minimum apparent move time in milliseconds.
        /// </param>
        /// <param name="thinkerListeners">Thinker listeners.</param>
        /// <param name="matchListeners">Match listeners.</param>
        /// <param name="assemblies">Third-party assemblies.</param>
        /// <param name="debugMode">
        /// Show debug information (exception stack traces)?
        /// </param>
        public GameOptions(
            int rows, int cols, int winSequence,
            int roundPiecesPerPlayer, int squarePiecesPerPlayer,
            int timeLimitMillis, int minMoveTimeMillis,
            IEnumerable<string> thinkerListeners,
            IEnumerable<string> matchListeners,
            IEnumerable<string> assemblies, bool debugMode)
                : base(assemblies, debugMode)
        {
            this.rows = rows;
            this.cols = cols;
            this.winSequence = winSequence;
            this.roundPiecesPerPlayer = roundPiecesPerPlayer;
            this.squarePiecesPerPlayer = squarePiecesPerPlayer;
            this.timeLimitMillis = timeLimitMillis;
            this.minMoveTimeMillis = minMoveTimeMillis;
            this.thinkerListeners = thinkerListeners;
            this.matchListeners = matchListeners;
        }

        /// <summary>
        /// Number of board rows.
        /// </summary>
        [Option('r', "rows", Default = 6,
            HelpText = "Number of rows in game board")]
        public int Rows => rows;

        /// <summary>
        /// Number of board columns.
        /// </summary>
        [Option('c', "cols", Default = 7,
            HelpText = "Number of columns in game board")]
        public int Cols => cols;

        /// <summary>
        /// Number of pieces in sequence for winning the game.
        /// </summary>
        [Option('w', "win-sequence", Default = 4,
            HelpText = "How many pieces in sequence to win")]
        public int WinSequence => winSequence;

        /// <summary>
        /// Number of initial round pieces per player.
        /// </summary>
        [Option('o', "round-pieces", Default = 10,
            HelpText = "Number of initial round pieces per player")]
        public int RoundPiecesPerPlayer => roundPiecesPerPlayer;

        /// <summary>
        /// Number of initial square pieces per player.
        /// </summary>
        [Option('s', "square-pieces", Default = 11,
            HelpText = "Number of initial square pieces per player")]
        public int SquarePiecesPerPlayer => squarePiecesPerPlayer;

        /// <summary>
        /// Time limit for thinking in milliseconds.
        /// </summary>
        [Option('t', "time-limit", Default = 3600000,
            HelpText = "Time limit (ms) for making move")]
        public int TimeLimitMillis => timeLimitMillis;

        /// <summary>
        /// Time limit for thinking in seconds.
        /// </summary>
        /// <remarks>
        /// This value is derived from <see cref="TimeLimitMillis"/>.
        /// </remarks>
        public float TimeLimitSeconds => timeLimitMillis / 1000.0f;

        /// <summary>
        /// Minimum apparent move time in milliseconds.
        /// </summary>
        [Option('m', "min-time", Default = 0,
            HelpText = "Minimum time (ms) between moves")]
        public int MinMoveTimeMillis => minMoveTimeMillis;

        /// <summary>
        /// Minimum apparent move time in seconds.
        /// </summary>
        /// <remarks>
        /// This value is derived from <see cref="MinMoveTimeMillis"/>.
        /// </remarks>
        public float MinMoveTimeSeconds => minMoveTimeMillis / 1000.0f;

        /// <summary>
        /// Thinker listeners.
        /// </summary>
        [Option("thinker-listeners",
            Default = new string[] {
                "ColorShapeLinks.TextBased.App.SimpleRenderingListener" },
            HelpText = "Thinker event listeners (space separated)")]
        public virtual IEnumerable<string> ThinkerListeners => thinkerListeners;

        /// <summary>
        /// Match listeners.
        /// </summary>
        [Option("match-listeners",
            Default = new string[] {
                "ColorShapeLinks.TextBased.App.SimpleRenderingListener" },
            HelpText = "Match event listeners (space separated)")]
        public virtual IEnumerable<string> MatchListeners => matchListeners;

        /// <summary>
        /// A sequence of thinker prototypes.
        /// </summary>
        public abstract IEnumerable<IThinkerPrototype> ThinkerPrototypes
        {
            get;
        }
    }
}
