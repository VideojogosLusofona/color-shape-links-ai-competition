/// @file
/// @brief This file contains the
/// ::ColorShapeLinks.UnityApp.ThinkerListAttribute class.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using UnityEngine;
using ColorShapeLinks.Common.AI;

namespace ColorShapeLinks.UnityApp
{
    /// <summary>
    /// Attribute which provides a custom attribute for script variables
    /// containing all the known thinkers at runtime.
    /// </summary>
    public class ThinkerListAttribute : PropertyAttribute
    {
        /// <summary>
        /// Property containing all the known thinkers at runtime.
        /// </summary>
        public string[] Thinkers => ThinkerManager.Instance.ThinkerNames;
    }
}
