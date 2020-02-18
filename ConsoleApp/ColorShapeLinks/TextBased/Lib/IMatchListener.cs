/// @file
/// @brief This file contains the
/// ::ColorShapeLinks.TextBased.Lib.IMatchListener interface.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

namespace ColorShapeLinks.TextBased.Lib
{
    /// <summary>
    /// Interface to be implemented by classes who wish to listen to match
    /// events.
    /// </summary>
    public interface IMatchListener
    {
        /// <summary>
        /// Register listener with a match event producer.
        /// </summary>
        /// <param name="subject">
        /// The match event producer which this listener will be listen to.
        /// </param>
        /// <seealso cref="IMatchListener.ListenTo"/>
        void ListenTo(IMatchSubject subject);
    }
}
