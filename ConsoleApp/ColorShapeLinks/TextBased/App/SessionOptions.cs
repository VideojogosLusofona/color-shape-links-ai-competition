/// @file
/// @brief This file contains the
/// ::ColorShapeLinks.TextBased.App.SessionOptions class.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System.IO;
using System.Collections.Generic;
using ColorShapeLinks.Common.AI;
using ColorShapeLinks.Common.Session;
using CommandLine;

namespace ColorShapeLinks.TextBased.App
{
    /// <summary>
    /// Command line session options, available only to the "session"
    /// verb command.
    /// </summary>
    [Verb("session",
        HelpText = "Run a complete session (tournament) between all thinkers")]
    public class SessionOptions : GameOptions, ISessionConfig
    {
        // Points per win
        private readonly int pointsPerWin;

        // Points per loss
        private readonly int pointsPerLoss;

        // Points per draw
        private readonly int pointsPerDraw;

        // Session configuration file
        private readonly string configFile;

        // Session listeners
        private readonly IEnumerable<string> sessionListeners;

        // A sequence of thinker prototypes
        private IEnumerable<IThinkerPrototype> thinkerPrototypes;

        /// <summary>
        /// Create a new instance of session options.
        /// </summary>
        /// <param name="pointsPerWin">Points per win.</param>
        /// <param name="pointsPerLoss">Points per loss.</param>
        /// <param name="pointsPerDraw">Points per draw.</param>
        /// <param name="configFile">Session configuration file.</param>
        /// <param name="thinkerListeners">Thinker listeners.</param>
        /// <param name="matchListeners">Match listeners.</param>
        /// <param name="sessionListeners">Session listeners.</param>
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
        /// <param name="assemblies">Third-party assemblies.</param>
        /// <param name="debugMode">
        /// Show debug information (exception stack traces)?
        /// </param>
        public SessionOptions(
            int pointsPerWin, int pointsPerLoss, int pointsPerDraw,
            string configFile,
            IEnumerable<string> thinkerListeners,
            IEnumerable<string> matchListeners,
            IEnumerable<string> sessionListeners,
            int rows, int cols, int winSequence,
            int roundPiecesPerPlayer, int squarePiecesPerPlayer,
            int timeLimitMillis, int minMoveTimeMillis,
            IEnumerable<string> assemblies, bool debugMode)
                : base(rows, cols, winSequence,
                    roundPiecesPerPlayer, squarePiecesPerPlayer,
                    timeLimitMillis, minMoveTimeMillis,
                    thinkerListeners, matchListeners,
                    assemblies, debugMode)
        {
            this.pointsPerWin = pointsPerWin;
            this.pointsPerLoss = pointsPerLoss;
            this.pointsPerDraw = pointsPerDraw;
            this.configFile = configFile;
            this.sessionListeners = sessionListeners;
        }

        /// <summary>
        /// Points per win.
        /// </summary>
        [Option("points-per-win", Default = 3, HelpText = "Points per win")]
        public int PointsPerWin => pointsPerWin;

        /// <summary>
        /// Points per loss.
        /// </summary>
        [Option("points-per-loss", Default = 0, HelpText = "Points per loss")]
        public int PointsPerLoss => pointsPerLoss;

        /// <summary>
        /// Points per draw.
        /// </summary>
        [Option("points-per-draw", Default = 1, HelpText = "Points per draw")]
        public int PointsPerDraw => pointsPerDraw;

        /// <summary>
        /// Session configuration file.
        /// </summary>
        /// <remarks>
        /// Each line of this file contains the fully qualified name of a
        /// thinker, a space, and the thinker options.
        /// </remarks>
        [Option('g', "config", Required = true,
            HelpText = "Session configuration file, each line contains fully "
                + "qualified name of thinker, space, thinker options")]
        public string ConfigFile => configFile;

        /// <summary>
        /// Thinker listeners.
        /// </summary>
        [Option("thinker-listeners",
            Default = new string[] { },
            HelpText = "Thinker event listeners (space separated)")]
        public override IEnumerable<string> ThinkerListeners =>
            base.ThinkerListeners;

        /// <summary>
        /// Match listeners.
        /// </summary>
        [Option("match-listeners",
            Default = new string[] { },
            HelpText = "Match event listeners (space separated)")]
        public override IEnumerable<string> MatchListeners =>
            base.MatchListeners;

        /// <summary>
        /// Session listeners.
        /// </summary>
        [Option("session-listeners",
            Default = new string[] {
                "ColorShapeLinks.TextBased.App.SimpleRenderingListener" },
            HelpText = "Session event listeners (space separated)")]
        public IEnumerable<string> SessionListeners => sessionListeners;

        /// <summary>
        /// A sequence of thinker prototypes.
        /// </summary>
        public override IEnumerable<IThinkerPrototype> ThinkerPrototypes
        {
            get
            {
                // Thinker prototypes are lazily instantiated
                if (thinkerPrototypes == null)
                {
                    // Collection of thinker prototypes
                    ICollection<IThinkerPrototype> prototypes =
                        new List<IThinkerPrototype>();

                    // Open file listing the thinkers and their configurations
                    using (StreamReader sr = new StreamReader(configFile))
                    {
                        // Each line specifies a thinker and its configuration
                        char[] seps = { ' ' };
                        string line;

                        // Loop through all the lines
                        while ((line = sr.ReadLine()) != null)
                        {
                            // Trim line
                            line = line.Trim();

                            // Only process line if it's not empty or is
                            // commented out
                            if (line.Length > 0 && line[0] != '#'
                                && line[0] != '%' && line[0] != ';')
                            {
                                // Split the line in two using the first space
                                string[] parsed = line.Split(seps, 2);

                                // Get the thinker fully qualified name
                                string thinkerFQN =
                                    parsed.Length > 0 ? parsed[0] : "";

                                // Get the thinker parameters
                                string thinkerParams =
                                    parsed.Length > 1 ? parsed[1] : "";

                                // Add prototype to collection
                                prototypes.Add(
                                    new ThinkerPrototype(
                                        thinkerFQN, thinkerParams, this));
                            }
                        }
                    }
                    // Place collection in instance variable
                    thinkerPrototypes = prototypes;
                }

                // Return the thinker prototypes
                return thinkerPrototypes;
            }
        }
    }
}
