/// @file
/// @brief This file contains the ::ColorShapeLinks.Common.AI.ThinkerPrototype
/// class.
///
/// @author Nuno Fachada
/// @date 2020, 2021
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;
using System.Reflection;
using ColorShapeLinks.Common.Session;

namespace ColorShapeLinks.Common.AI
{
    /// <summary>
    /// Represents a thinker prototype from which to obtain new instances of
    /// the given thinker.
    /// </summary>
    public class ThinkerPrototype : IThinkerPrototype
    {
        // Thinker's fully qualified name
        private string thinkerFQN;

        // Thinker's configuration parameters
        private string thinkerParams;

        // The configuration for matches where created thinkers will play
        private IMatchConfig matchConfig;

        // Name of the underlying thinker
        private string thinkerName;

        /// <summary>
        /// Name of the underlying thinker.
        /// </summary>
        public string ThinkerName
        {
            get
            {
                // If name is not set, create a temporary instance which will
                // set the name
                if (thinkerName == null) Create();
                return thinkerName;
            }
        }

        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        /// <param name="thinkerFQN">Thinker's fully qualified name.</param>
        /// <param name="thinkerParams">
        /// Thinker's configuration parameters.
        /// </param>
        /// <param name="matchConfig">
        /// The configuration for matches where created thinkers will play.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown when the <paramref name="thinkerFQN"/> is empty or does not
        /// correspond to a known thinker.
        /// </exception>
        public ThinkerPrototype(
            string thinkerFQN, string thinkerParams, IMatchConfig matchConfig)
        {
            // Check if thinkerFQN string contains anything
            if (thinkerFQN == null || thinkerFQN.Length == 0)
            {
                throw new ArgumentException(
                    $"A ThinkerPrototype requires a non-empty Thinker FQN.");
            }

            // If AI is not known, thrown an exception
            if (!ThinkerManager.Instance.IsKnown(thinkerFQN))
            {
                throw new ArgumentException(
                    $"No Thinker named '{thinkerFQN}' was found");
            }

            // Keep the parameters
            this.thinkerFQN = thinkerFQN;
            this.thinkerParams = thinkerParams;
            this.matchConfig = matchConfig;
        }

        /// <summary>
        /// Instantiate a new thinker from this prototype.
        /// </summary>
        /// <returns>A new thinker instance.</returns>
        public IThinker Create()
        {
            // Variable where to place the thinker instance
            AbstractThinker thinker;

            // Flags to find the fields to initialize in the thinker instance
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;

            // Get the thinker's type
            Type type = ThinkerManager.Instance.GetTypeFromFQN(thinkerFQN);

            // Instantiate the AI thinker
            thinker = (AbstractThinker)Activator.CreateInstance(type);

            // Initialize the fields of the AI thinker instance
            typeof(AbstractThinker)
                .GetField("rows", flags)
                .SetValue(thinker, matchConfig.Rows);
            typeof(AbstractThinker)
                .GetField("cols", flags)
                .SetValue(thinker, matchConfig.Cols);
            typeof(AbstractThinker)
                .GetField("winSequence", flags)
                .SetValue(thinker, matchConfig.WinSequence);
            typeof(AbstractThinker)
                .GetField("roundPiecesPerPlayer", flags)
                .SetValue(thinker, matchConfig.RoundPiecesPerPlayer);
            typeof(AbstractThinker)
                .GetField("squarePiecesPerPlayer", flags)
                .SetValue(thinker, matchConfig.SquarePiecesPerPlayer);
            typeof(AbstractThinker)
                .GetField("timeLimitMillis", flags)
                .SetValue(thinker, matchConfig.TimeLimitMillis);

            // Setup the AI thinker instance, implementation dependent
            thinker.Setup(thinkerParams);

            // Keep the thinker's name
            thinkerName = thinker.ToString();

            // Return the new AI thinker instance
            return thinker;
        }

        /// <summary>
        /// Returns name of the underlying thinker.
        /// </summary>
        /// <returns>Name of the underlying thinker.</returns>
        public override string ToString() => ThinkerName;
    }
}
