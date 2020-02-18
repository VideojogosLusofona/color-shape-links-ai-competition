/// @file
/// @brief This file contains the ::ColorShapeLinks.TextBased.App.BaseOptions
/// class.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using CommandLine;
using System.Collections.Generic;

namespace ColorShapeLinks.TextBased.App
{
    /// <summary>
    /// Command-line base options, available to all verb commands.
    /// </summary>
    [Verb("info", HelpText =
        "Show environment info (known assemblies, thinkers and "
        + "listeners) and exit")]
    public class BaseOptions
    {
        // Third-party assemblies
        private readonly IEnumerable<string> assemblies;

        // Show debug information (exception stack traces)?
        private readonly bool debugMode;

        /// <summary>
        /// Create a new instance of BaseOptions.
        /// </summary>
        /// <param name="assemblies">Third-party assemblies.</param>
        /// <param name="debugMode">
        /// Show debug information (exception stack traces)?
        /// </param>
        public BaseOptions(IEnumerable<string> assemblies, bool debugMode)
        {
            this.assemblies = assemblies;
            this.debugMode = debugMode;
        }

        /// <summary>
        /// Third-party assemblies.
        /// </summary>
        [Option('a', "assemblies",
            HelpText = "Load .NET Standard 2.0 DLLs containing thinkers "
                + "and/or listeners (space separated)")]
        public IEnumerable<string> Assemblies => assemblies;

        /// <summary>
        /// Enable debug mode.
        /// </summary>
        [Option('d', "debug", Default = false,
            HelpText = "Enable debug mode (shows exception stack traces)")]
        public bool DebugMode => debugMode;
    }
}
