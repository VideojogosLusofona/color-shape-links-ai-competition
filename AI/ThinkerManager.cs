/// @file
/// @brief This file contains the ::ColorShapeLinks.Common.AI.ThinkerManager
/// class.
///
/// @author Nuno Fachada
/// @date 2020, 2021
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;
using System.Linq;
using System.Collections.Generic;

namespace ColorShapeLinks.Common.AI
{
    /// <summary>
    /// Singleton class used for finding and keeping a record of existing
    /// thinkers.
    /// </summary>
    public class ThinkerManager
    {
        // Unique instance of this class, instantiated lazily
        private static readonly Lazy<ThinkerManager> instance =
            new Lazy<ThinkerManager>(() => new ThinkerManager());

        // Known thinkers
        private readonly IDictionary<string, Type> thinkerTable;

        /// <summary>
        /// Returns the singleton instance of this class.
        /// </summary>
        /// <value>The singleton instance of this class.</value>
        public static ThinkerManager Instance => instance.Value;

        /// <summary>
        /// Array of thinker names.
        /// </summary>
        /// <value>Names of known thinkers.</value>
        public string[] ThinkerNames => thinkerTable.Keys.ToArray();

        /// <summary>
        /// Does the given thinker FQN correspond to a known thinker?
        /// </summary>
        /// <param name="thinkerFQN">
        /// Fully qualified name of thinker.
        /// </param>
        /// <returns>
        /// <c>true</c> if the thinker class exists in the loaded
        /// assemblies, <c>false</c> otherwise.
        /// </returns>
        public bool IsKnown(string thinkerFQN) =>
            thinkerTable.ContainsKey(thinkerFQN);

        /// <summary>
        /// Get thinker type from its fully qualified name.
        /// </summary>
        /// <param name="thinkerFQN">
        /// Fully qualified name of thinker.
        /// </param>
        /// <returns>
        /// The thinker's type.
        /// </returns>
        public Type GetTypeFromFQN(string thinkerFQN) =>
            thinkerTable[thinkerFQN];

        // Private constructor
        private ThinkerManager()
        {
            // Get a reference to the AbstractThinker type
            Type type = typeof(AbstractThinker);

            // Get known thinkers, i.e. thinkers which extend AbstractThinker,
            // are not abstract and have an empty constructor
            thinkerTable = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => type.IsAssignableFrom(t)
                    && !t.IsAbstract
                    && t.GetConstructor(Type.EmptyTypes) != null)
                .ToDictionary(t => t.FullName, t => t);
        }
    }
}
