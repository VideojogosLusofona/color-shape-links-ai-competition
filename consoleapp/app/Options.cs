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
    public class Options : IGameConfig
    {
        private readonly int rows;
        private readonly int cols;
        private readonly int winSequence;
        private readonly int roundPiecesPerPlayer;
        private readonly int squarePiecesPerPlayer;
        private readonly int timeLimitMillis;
        private readonly int minMoveTimeMillis;
        private readonly string player1;
        private readonly string player2;
        private readonly string player1params;
        private readonly string player2params;
        private readonly IEnumerable<string> listeners;
        private readonly IEnumerable<string> assemblies;
        private readonly bool showDebugInfoAndExit;

        public Options(int rows, int cols, int winSequence,
            int roundPiecesPerPlayer, int squarePiecesPerPlayer,
            int timeLimitMillis, int minMoveTimeMillis,
            string player1, string player2,
            string player1params, string player2params,
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
            this.player1 = player1;
            this.player2 = player2;
            this.player1params = player1params;
            this.player2params = player2params;
            this.listeners = listeners;
            this.assemblies = assemblies;
            this.showDebugInfoAndExit = showDebugInfoAndExit;
        }

        [Option('r', "rows", Default = 6, SetName = "game",
            HelpText = "Number of rows in game board")]
        public int Rows => rows;

        [Option('c', "cols", Default = 7, SetName = "game",
            HelpText = "Number of columns in game board")]
        public int Cols => cols;

        [Option('w', "win-sequence", Default = 4, SetName = "game",
            HelpText = "How many pieces in sequence to win")]
        public int WinSequence => winSequence;

        [Option('o', "round-pieces", Default = 10, SetName = "game",
            HelpText = "Number of initial round pieces per player")]
        public int RoundPiecesPerPlayer => roundPiecesPerPlayer;

        [Option('s', "square-pieces", Default = 11, SetName = "game",
            HelpText = "Number of initial square pieces per player")]
        public int SquarePiecesPerPlayer => squarePiecesPerPlayer;

        [Option('t', "time-limit", Default = int.MaxValue / 2, SetName = "game",
            HelpText = "Time limit (ms) for making move")]
        public int TimeLimitMillis => timeLimitMillis;

        [Option('m', "min-time", Default = 0, SetName = "game",
            HelpText = "Minimum time (ms) between moves")]
        public int MinMoveTimeMillis => minMoveTimeMillis;

        [Option('W', "white",
            Default = "ColorShapeLinks.ConsoleApp.HumanThinker",
            SetName = "game",
            HelpText = "Fully qualified name of player 1 thinker class")]
        public string Player1 => player1;

        [Option('R', "red",
            Default = "ColorShapeLinks.ConsoleApp.HumanThinker",
            SetName = "game",
            HelpText = "Fully qualified name of player 2 thinker class")]
        public string Player2 => player2;

        [Option("white-params", Default = "", SetName = "game",
            HelpText = "Parameters for setting up player 1 thinker instance")]
        public string Player1Params => player1params;

        [Option("red-params", Default = "", SetName = "game",
            HelpText = "Parameters for setting up player 2 thinker instance")]
        public string Player2Params => player2params;

        [Option('l', "listeners",
            Default = new string[] {
                "ColorShapeLinks.ConsoleApp.SimpleRenderingListener" },
            SetName = "game",
            HelpText = "Match event listeners (space separated)")]
        public IEnumerable<string> Listeners => listeners;

        [Option('a', "assemblies",
            HelpText = ".NET Standard 2.0 DLLs containing thinkers and/or "
                + "listeners (space separated)")]
        public IEnumerable<string> Assemblies => assemblies;

        [Option('d', "debug", Default = false, SetName = "debug",
            HelpText = "Show debug info (known assemblies, thinkers and "
                + "listeners) and exit")]
        public bool ShowDebugInfoAndExit => showDebugInfoAndExit;

    }
}
