/// @file
/// @brief This file contains the
/// ::ColorShapeLinks.TextBased.Lib.ISessionSubject interface.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;
using System.Collections.Generic;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.Session;

namespace ColorShapeLinks.TextBased.Lib
{
    /// <summary>
    /// Interface defining the events raised in a session of ColorShapeLinks.
    /// </summary>
    public interface ISessionSubject
    {
        /// <summary>
        /// Event raised when the session is about to start.
        /// </summary>
        /// <remarks>
        /// * The `IEnumerable<Match>` type parameter represents the set of
        ///   matches to be played.
        /// </remarks>
        event Action<IEnumerable<Match>> BeforeSession;

        event Action<ISessionDataProvider> AfterSession;

        event Action<Match> BeforeMatch;

        event Action<Match, ISessionDataProvider> AfterMatch;

    }
}
