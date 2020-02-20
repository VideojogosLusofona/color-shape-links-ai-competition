/// @file
/// @brief This file contains the
/// ::ColorShapeLinks.TextBased.Lib.ISessionSubject interface.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;
using System.Collections.Generic;
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
        /// * The `ISessionDataProvider` type parameter provides data about the
        ///   session that is about to start.
        /// </remarks>
        event Action<ISessionDataProvider> BeforeSession;

        /// <summary>
        /// Event raised when the session is over.
        /// </summary>
        /// <remarks>
        /// * The `ISessionDataProvider` type parameter provides data about the
        ///   finished session.
        /// </remarks>
        event Action<ISessionDataProvider> AfterSession;

        /// <summary>
        /// Event raised before a match starts in the context of a session.
        /// </summary>
        /// <remarks>
        /// * The `Match` type parameter provides information about the match
        ///   that is about to start.
        /// </remarks>
        event Action<Match> BeforeMatch;

        /// <summary>
        /// Event raised after a match takes place in the context of a session.
        /// </summary>
        /// <remarks>
        /// * The `Match` type parameter provides information about the match
        ///   that just finished.
        /// * The `ISessionDataProvider` type parameter provides data about the
        ///   current state of the session.
        /// </remarks>
        event Action<Match, ISessionDataProvider> AfterMatch;
    }
}
