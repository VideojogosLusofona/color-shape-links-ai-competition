/// @file
/// @brief This file contains the ::AIManager class.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace ColorShapeLinks.Common.AI
{
    /// <summary>
    /// Singleton class used for finding and keeping a record of existing AIs.
    /// </summary>
    public class AIManager
    {
        // Unique instance of this class, instantiated lazily
        private static readonly Lazy<AIManager> instance =
            new Lazy<AIManager>(() => new AIManager());

        // Table of known AIs
        private IDictionary<string, Type> aiTable;

        /// <summary>
        /// Returns the singleton instance of this class.
        /// </summary>
        /// <value>The singleton instance of this class.</value>
        public static AIManager Instance => instance.Value;

        /// <summary>
        /// Array of AI names.
        /// </summary>
        /// <value>Names of known AI.</value>
        public string[] AIs => aiTable.Keys.ToArray();

        // Private constructor
        private AIManager()
        {
            // Get a reference to the IThinker type
            Type type = typeof(AbstractThinker);

            // Get known AIs, i.e. AIs which implement IThinker, are not
            // abstract and have an empty constructor
            aiTable = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => type.IsAssignableFrom(t)
                    && !t.IsAbstract
                    && t.GetConstructor(Type.EmptyTypes) != null)
                .ToDictionary(t => t.FullName, t => t);
        }

        /// <summary>
        /// Instantiate a new thinker.
        /// </summary>
        /// <param name="aiName">Fully qualified name of the AI thinker.</param>
        /// <param name="gameConfig">Game configuration.</param>
        /// <param name="aiConfig">AI thinker configuration.</param>
        /// <returns>A new AI thinker instance.</returns>
        public AbstractThinker NewThinker(
            string aiName, IGameConfig gameConfig, string aiConfig)
        {
            // Variable where to place the thinker instance
            AbstractThinker thinker;

            // Flags to find the fields to initialize in the thinker instance
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;

            // If AI is not known, thrown an exception
            if (!aiTable.ContainsKey(aiName))
            {
                throw new InvalidOperationException(
                    $"No AI named '{aiName}' was found");
            }

            // Instantiate the AI thinker
            thinker =
                (AbstractThinker)Activator.CreateInstance(aiTable[aiName]);

            // Initialize the fields of the AI thinker instance
            typeof(AbstractThinker)
                .GetField("rows", flags)
                .SetValue(thinker, gameConfig.Rows);
            typeof(AbstractThinker)
                .GetField("cols", flags)
                .SetValue(thinker, gameConfig.Cols);
            typeof(AbstractThinker)
                .GetField("winSequence", flags)
                .SetValue(thinker, gameConfig.WinSequence);
            typeof(AbstractThinker)
                .GetField("roundPiecesPerPlayer", flags)
                .SetValue(thinker, gameConfig.RoundPiecesPerPlayer);
            typeof(AbstractThinker)
                .GetField("squarePiecesPerPlayer", flags)
                .SetValue(thinker, gameConfig.SquarePiecesPerPlayer);
            typeof(AbstractThinker)
                .GetField("timeLimitMillis", flags)
                .SetValue(thinker, gameConfig.TimeLimitMillis);

            // Setup the AI thinker instance, implementation dependent
            thinker.Setup(aiConfig);

            // Return the new AI thinker instance
            return thinker;
        }
    }
}
