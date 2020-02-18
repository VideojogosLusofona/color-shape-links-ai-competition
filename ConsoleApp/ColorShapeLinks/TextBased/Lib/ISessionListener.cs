/// @file
/// @brief This file contains the
/// ::ColorShapeLinks.TextBased.Lib.ISessionListener interface.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

namespace ColorShapeLinks.TextBased.Lib
{
    /// <summary>
    /// Interface to be implemented by classes who wish to listen to session
    /// events.
    /// </summary>
    public interface ISessionListener
    {
        /// <summary>
        /// Register listener with a session event producer.
        /// </summary>
        /// <param name="subject">
        /// The session event producer which this listener will be listen to.
        /// </param>
        void ListenTo(ISessionSubject subject);
    }
}
