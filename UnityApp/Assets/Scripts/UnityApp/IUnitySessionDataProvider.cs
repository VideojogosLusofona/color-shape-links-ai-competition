/// @file
/// @brief This file contains the
/// ::ColorShapeLinks.UnityApp.IUnitySessionDataProvider interface.
///
/// @author Nuno Fachada
/// @date 2019, 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using ColorShapeLinks.Common.Session;

namespace ColorShapeLinks.UnityApp
{
    /// <summary>
    /// Defines a data provider for ColorShapeLinks Unity sessions, which
    /// include one or more matches.
    /// </summary>
    public interface IUnitySessionDataProvider : ISessionDataProvider
    {
        /// <summary>Ask who plays first?</summary>
        /// <value>
        /// `true` if UI is to ask who plays first, `false` otherwise.
        /// </value>
        bool WhoPlaysFirst { get; }

        /// <summary>
        /// Start of next match screen needs to be unlocked with a button?
        /// </summary>
        /// <value>
        /// `true` if start of next match screen needs to be unlocked with a
        /// button, `false` otherwise.
        /// </value>
        bool BlockStartNextMatch { get; }

        /// <summary>
        /// Show result screen needs to be unlocked with a button?
        /// </summary>
        /// <value>
        /// `true` if show result screen needs to be unlocked with a button,
        /// `false` otherwise.
        /// </value>
        bool BlockShowResult { get; }

        /// <summary>
        /// Duration of unblocked screens (start of next match, show result).
        /// </summary>
        /// <value>Duration in seconds.</value>
        float UnblockedScreenDuration { get; }
    }
}
