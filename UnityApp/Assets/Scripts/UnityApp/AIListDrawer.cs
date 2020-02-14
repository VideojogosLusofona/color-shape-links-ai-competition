/// @file
/// @brief This file contains the ::AIListDrawer class.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;
using UnityEngine;
using UnityEditor;

namespace ColorShapeLinks.UnityApp
{
    /// <summary>
    /// Custom property drawer for selecting an AI thinker from all
    /// known AI thinkers at runtime.
    /// </summary>
    [CustomPropertyDrawer(typeof(AIListAttribute))]
    public class AIListDrawer : PropertyDrawer
    {

        /// <summary>
        /// Draw the property inside the given rect.
        /// </summary>
        /// <param name="position">
        /// Rectangle on the screen to use for the property GUI.
        /// </param>
        /// <param name="property">
        /// The SerializedProperty to make the custom GUI for.
        /// </param>
        /// <param name="label">
        /// The label of this property.
        /// </param>
        public override void OnGUI(
            Rect position, SerializedProperty property, GUIContent label)
        {
            // Get the PropertyAttribute for the property from the base class
            // and convert it to an AIListAttribute
            AIListAttribute aiList = attribute as AIListAttribute;

            // Get the known AI thinkers from the AIListAttribute instance
            string[] aiNamesList = aiList.AIs;

            // Get the index of the previously selected AI thinker
            int index = Mathf.Max(
                0, Array.IndexOf(aiNamesList, property.stringValue));

            // Update index in case it changed
            index = EditorGUI.Popup(
                position, property.displayName, index, aiNamesList);

            // Update selected AI thinker in case index changed
            property.stringValue = aiNamesList[index];
        }
    }
}
