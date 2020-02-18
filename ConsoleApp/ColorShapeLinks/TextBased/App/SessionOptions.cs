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
        private readonly IEnumerable<IThinkerPrototype> thinkerPrototypes;

        /// <summary>
        /// Create a new instance of session options.
        /// </summary>
        /// <param name="assemblies">Third-party assemblies.</param>
        /// <param name="debugMode">
        /// Show debug information (exception stack traces)?
        /// </param>
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
        /// <param name="pointsPerWin">Points per win.</param>
        /// <param name="pointsPerLoss">Points per loss.</param>
        /// <param name="pointsPerDraw">Points per draw.</param>
        /// <param name="configFile">Session configuration file.</param>
        /// <param name="sessionListeners">Session listeners.</param>
        public SessionOptions(IEnumerable<string> assemblies, bool debugMode,
            int rows, int cols, int winSequence,
            int roundPiecesPerPlayer, int squarePiecesPerPlayer,
            int timeLimitMillis, int minMoveTimeMillis,
            IEnumerable<string> thinkerListeners,
            IEnumerable<string> matchListeners,
            int pointsPerWin, int pointsPerLoss, int pointsPerDraw,
            string configFile, IEnumerable<string> sessionListeners)
                : base(assemblies, debugMode, rows, cols, winSequence,
                    roundPiecesPerPlayer, squarePiecesPerPlayer,
                    timeLimitMillis, minMoveTimeMillis,
                    thinkerListeners, matchListeners)
        {
            ICollection<IThinkerPrototype> prototypes =
                new List<IThinkerPrototype>();

            this.pointsPerWin = pointsPerWin;
            this.pointsPerLoss = pointsPerLoss;
            this.pointsPerDraw = pointsPerDraw;
            this.configFile = configFile;
            this.sessionListeners = sessionListeners;

            using (StreamReader sr = new StreamReader(configFile))
            {
                char[] seps = { ' ' };
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] parsed = line.Split(seps, 2);
                    string thinkerFQN = parsed.Length > 0 ? parsed[0] : "";
                    string thinkerParams = parsed.Length > 1 ? parsed[1] : "";
                    prototypes.Add(
                        new ThinkerPrototype(thinkerFQN, thinkerParams, this));
                }
            }

            thinkerPrototypes = prototypes;
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
        [Option('g', "config",
            HelpText = "Session configuration file, each line contains fully "
                + "qualified name of thinker, space, thinker options")]
        public string ConfigFile => configFile;

        /// <summary>
        /// Session listeners.
        /// </summary>
        [Option("session-listeners",
            Default = new string[] {
                "ColorShapeLinks.TextBased.App.SimpleSessionListener" },
            HelpText = "Session event listeners (space separated)")]
        public IEnumerable<string> SessionListeners => sessionListeners;

        /// <summary>
        /// A sequence of thinker prototypes.
        /// </summary>
        public override IEnumerable<IThinkerPrototype> ThinkerPrototypes =>
            thinkerPrototypes;

    }
}
