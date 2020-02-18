/// @file
/// @brief This file contains the ::ColorShapeLinks.TextBased.App.MatchOptions
/// class.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System.Collections.Generic;
using ColorShapeLinks.Common.AI;
using CommandLine;

namespace ColorShapeLinks.TextBased.App
{
    /// <summary>
    /// Command line match options, available only to the "match" verb command.
    /// </summary>
    [Verb("match", HelpText = "Run a single match between two thinkers")]
    public class MatchOptions : GameOptions
    {
        // Full class name of thinker 1
        private readonly string thinker1;

        // Full class name of thinker 2
        private readonly string thinker2;

        // Parameters for setting up thinker 1 instance
        private readonly string thinker1params;

        // Parameters for setting up thinker 2 instance
        private readonly string thinker2params;

        // A sequence of thinker prototypes
        private IEnumerable<IThinkerPrototype> thinkerPrototypes;

        /// <summary>
        /// Create a new instance of match options.
        /// </summary>
        /// <param name="thinker1">Full class name of thinker 1.</param>
        /// <param name="thinker2">Full class name of thinker 2.</param>
        /// <param name="thinker1params">
        /// Parameters for setting up thinker 1 instance.
        /// </param>
        /// <param name="thinker2params">
        /// Parameters for setting up thinker 2 instance.
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
        /// </param>
        /// <param name="assemblies">Third-party assemblies.</param>
        /// <param name="debugMode">
        /// Show debug information (exception stack traces)?
        /// </param>
        public MatchOptions(
            string thinker1, string thinker2,
            string thinker1params, string thinker2params,
            int rows, int cols, int winSequence,
            int roundPiecesPerPlayer, int squarePiecesPerPlayer,
            int timeLimitMillis, int minMoveTimeMillis,
            IEnumerable<string> thinkerListeners,
            IEnumerable<string> matchListeners,
            IEnumerable<string> assemblies, bool debugMode)
                : base(rows, cols, winSequence,
                    roundPiecesPerPlayer, squarePiecesPerPlayer,
                    timeLimitMillis, minMoveTimeMillis,
                    thinkerListeners, matchListeners,
                    assemblies, debugMode)
        {
            this.thinker1 = thinker1;
            this.thinker2 = thinker2;
            this.thinker1params = thinker1params;
            this.thinker2params = thinker2params;
        }

        /// <summary>
        /// Full class name of thinker 1.
        /// </summary>
        [Option('W', "white",
            Default = "ColorShapeLinks.TextBased.App.HumanThinker",
            HelpText = "Fully qualified name of player 1 thinker class")]
        public string Thinker1 => thinker1;

        /// <summary>
        /// Full class name of thinker 2.
        /// </summary>
        [Option('R', "red",
            Default = "ColorShapeLinks.TextBased.App.HumanThinker",
            HelpText = "Fully qualified name of player 2 thinker class")]
        public string Thinker2 => thinker2;

        /// <summary>
        /// Parameters for setting up thinker 1 instance.
        /// </summary>
        [Option("white-params", Default = "",
            HelpText = "Parameters for setting up player 1 thinker instance")]
        public string Thinker1Params => thinker1params;

        /// <summary>
        /// Parameters for setting up thinker 2 instance.
        /// </summary>
        [Option("red-params", Default = "",
            HelpText = "Parameters for setting up player 2 thinker instance")]
        public string Thinker2Params => thinker2params;

        /// <summary>
        /// A sequence of thinker prototypes.
        /// </summary>
        public override IEnumerable<IThinkerPrototype> ThinkerPrototypes
        {
            get
            {
                // The thinker prototypes are lazily instantiated
                if (thinkerPrototypes == null)
                {
                    thinkerPrototypes = new IThinkerPrototype[]
                    {
                        new ThinkerPrototype(thinker1, thinker1params, this),
                        new ThinkerPrototype(thinker2, thinker2params, this),
                    };
                }
                return thinkerPrototypes;
            }
        }
    }
}
