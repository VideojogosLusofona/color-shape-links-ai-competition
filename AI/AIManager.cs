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
        private static readonly Lazy<AIManager> instance =
            new Lazy<AIManager>(() => new AIManager());

        public static AIManager Instance => instance.Value;

        private AIManager()
        {
            aiTable = new Dictionary<string, Type>();

            Type type = typeof(IThinker);

            aiTable = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => type.IsAssignableFrom(t)
                    && !t.IsAbstract
                    && t.GetConstructor(Type.EmptyTypes) != null)
                .ToDictionary(t => t.FullName, t => t);
        }

        public string[] AIs => aiTable.Keys.ToArray();

        private IDictionary<string, Type> aiTable;

        public AbstractThinker NewInstance(
            string aiName, IGameConfig gameConfig, string aiConfig)
        {
            AbstractThinker thinker;
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;

            if (!aiTable.ContainsKey(aiName))
            {
                throw new InvalidOperationException(
                    $"No AI named '{aiName}' was found");
            }

            thinker =
                (AbstractThinker)Activator.CreateInstance(aiTable[aiName]);

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

            thinker.Setup(aiConfig);

            return thinker;
        }
    }
}
