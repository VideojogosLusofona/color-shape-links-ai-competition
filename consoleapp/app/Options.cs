/// @file
/// @brief This file contains the ::Options class.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System.Collections.Generic;
using ColorShapeLinks.Common;
using CommandLine;

namespace ColorShapeLinks.ConsoleApp
{
    /// <summary>
    /// Command line match options.
    /// </summary>
    public class Options : IGameConfig
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

        // Full class name of thinker 1
        private readonly string thinker1;

        // Full class name of thinker 2
        private readonly string thinker2;

        // Parameters for setting up thinker 1 instance
        private readonly string thinker1params;

        // Parameters for setting up thinker 2 instance
        private readonly string thinker2params;

        // Match listeners
        private readonly IEnumerable<string> listeners;

        // Third-party assemblies
        private readonly IEnumerable<string> assemblies;

        // Show debug information and exit?
        private readonly bool showDebugInfoAndExit;

        /// <summary>
        /// Create a new instance of match options.
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
        /// <param name="thinker1">Full class name of thinker 1.</param>
        /// <param name="thinker2">Full class name of thinker 2.</param>
        /// <param name="thinker1params">
        /// Parameters for setting up thinker 1 instance.
        /// </param>
        /// <param name="thinker2params">
        /// Parameters for setting up thinker 2 instance.
        /// </param>
        /// <param name="listeners">Match listeners.</param>
        /// <param name="assemblies">Third-party assemblies.</param>
        /// <param name="showDebugInfoAndExit">
        /// Show debug information and exit?
        /// </param>
        public Options(int rows, int cols, int winSequence,
            int roundPiecesPerPlayer, int squarePiecesPerPlayer,
            int timeLimitMillis, int minMoveTimeMillis,
            string thinker1, string thinker2,
            string thinker1params, string thinker2params,
            IEnumerable<string> listeners, IEnumerable<string> assemblies,
            bool showDebugInfoAndExit)
        {
            this.rows = rows;
            this.cols = cols;
            this.winSequence = winSequence;
            this.roundPiecesPerPlayer = roundPiecesPerPlayer;
            this.squarePiecesPerPlayer = squarePiecesPerPlayer;
            this.timeLimitMillis = timeLimitMillis;
            this.minMoveTimeMillis = minMoveTimeMillis;
            this.thinker1 = thinker1;
            this.thinker2 = thinker2;
            this.thinker1params = thinker1params;
            this.thinker2params = thinker2params;
            this.listeners = listeners;
            this.assemblies = assemblies;
            this.showDebugInfoAndExit = showDebugInfoAndExit;
        }

        /// <summary>
        /// Number of board rows.
        /// </summary>
        [Option('r', "rows", Default = 6, SetName = "game",
            HelpText = "Number of rows in game board")]
        public int Rows => rows;

        /// <summary>
        /// Number of board columns.
        /// </summary>
        [Option('c', "cols", Default = 7, SetName = "game",
            HelpText = "Number of columns in game board")]
        public int Cols => cols;

        /// <summary>
        /// Number of pieces in sequence for winning the game.
        /// </summary>
        [Option('w', "win-sequence", Default = 4, SetName = "game",
            HelpText = "How many pieces in sequence to win")]
        public int WinSequence => winSequence;

        /// <summary>
        /// Number of initial round pieces per player.
        /// </summary>
        [Option('o', "round-pieces", Default = 10, SetName = "game",
            HelpText = "Number of initial round pieces per player")]
        public int RoundPiecesPerPlayer => roundPiecesPerPlayer;

        /// <summary>
        /// Number of initial square pieces per player.
        /// </summary>
        [Option('s', "square-pieces", Default = 11, SetName = "game",
            HelpText = "Number of initial square pieces per player")]
        public int SquarePiecesPerPlayer => squarePiecesPerPlayer;

        /// <summary>
        /// Time limit for thinking in milliseconds.
        /// </summary>
        [Option('t', "time-limit", Default = int.MaxValue / 2, SetName = "game",
            HelpText = "Time limit (ms) for making move")]
        public int TimeLimitMillis => timeLimitMillis;

        /// <summary>
        /// Minimum apparent move time in milliseconds.
        /// </summary>
        [Option('m', "min-time", Default = 0, SetName = "game",
            HelpText = "Minimum time (ms) between moves")]
        public int MinMoveTimeMillis => minMoveTimeMillis;

        /// <summary>
        /// Full class name of thinker 1.
        /// </summary>
        [Option('W', "white",
            Default = "ColorShapeLinks.ConsoleApp.HumanThinker",
            SetName = "game",
            HelpText = "Fully qualified name of player 1 thinker class")]
        public string Thinker1 => thinker1;

        /// <summary>
        /// Full class name of thinker 2.
        /// </summary>
        [Option('R', "red",
            Default = "ColorShapeLinks.ConsoleApp.HumanThinker",
            SetName = "game",
            HelpText = "Fully qualified name of player 2 thinker class")]
        public string Thinker2 => thinker2;

        /// <summary>
        /// Parameters for setting up thinker 1 instance.
        /// </summary>
        [Option("white-params", Default = "", SetName = "game",
            HelpText = "Parameters for setting up player 1 thinker instance")]
        public string Thinker1Params => thinker1params;

        /// <summary>
        /// Parameters for setting up thinker 2 instance.
        /// </summary>
        [Option("red-params", Default = "", SetName = "game",
            HelpText = "Parameters for setting up player 2 thinker instance")]
        public string Thinker2Params => thinker2params;

        /// <summary>
        /// Match listeners.
        /// </summary>
        [Option('l', "listeners",
            Default = new string[] {
                "ColorShapeLinks.ConsoleApp.SimpleRenderingListener" },
            SetName = "game",
            HelpText = "Match event listeners (space separated)")]
        public IEnumerable<string> Listeners => listeners;

        /// <summary>
        /// Third-party assemblies.
        /// </summary>
        [Option('a', "assemblies",
            HelpText = ".NET Standard 2.0 DLLs containing thinkers and/or "
                + "listeners (space separated)")]
        public IEnumerable<string> Assemblies => assemblies;

        /// <summary>
        /// Show debug information and exit?
        /// </summary>
        [Option('d', "debug", Default = false, SetName = "debug",
            HelpText = "Show debug info (known assemblies, thinkers and "
                + "listeners) and exit")]
        public bool ShowDebugInfoAndExit => showDebugInfoAndExit;
    }
}
