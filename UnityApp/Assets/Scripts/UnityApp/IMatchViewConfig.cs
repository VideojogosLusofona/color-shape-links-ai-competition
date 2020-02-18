/// @file
/// @brief This file contains the ::ColorShapeLinks.UnityApp.IMatchViewConfig
/// interface.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

namespace ColorShapeLinks.UnityApp
{
    /// <summary>
    /// Interface defining configuration options for match views.
    /// </summary>
    public interface IMatchViewConfig
    {
        /// <summary>Last move animation length in seconds.</summary>
        /// <value>Last move animation length in seconds.</value>
        float LastMoveAnimLength { get; }
    }
}
