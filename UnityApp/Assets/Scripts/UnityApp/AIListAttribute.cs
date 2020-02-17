/// @file
/// @brief This file contains the
/// ::ColorShapeLinks.UnityApp.AIListAttribute class.
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
    /// containing all the known AI thinkers at runtime.
    /// </summary>
    public class AIListAttribute : PropertyAttribute
    {
        /// <summary>
        /// Property containing all the known AI thinkers at runtime.
        /// </summary>
        public string[] AIs => ThinkerManager.Instance.AIs;
    }
}
