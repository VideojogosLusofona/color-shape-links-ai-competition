/// @file
/// @brief This file contains the
/// ::ColorShapeLinks.TextBased.Lib.IThinkerListener interface.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using ColorShapeLinks.Common.AI;

namespace ColorShapeLinks.TextBased.Lib
{
    /// <summary>
    /// Interface to be implemented by classes who wish to listen to thinker
    /// events.
    /// </summary>
    public interface IThinkerListener
    {
        /// <summary>
        /// Register listener with a thinker event producer.
        /// </summary>
        /// <param name="subject">
        /// The thinker event producer which this listener will be listen to.
        /// </param>
        void ListenTo(IThinker subject);
    }
}
