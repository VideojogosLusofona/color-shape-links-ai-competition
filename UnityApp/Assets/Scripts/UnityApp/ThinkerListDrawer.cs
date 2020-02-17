/// @file
/// @brief This file contains the
/// ::ColorShapeLinks.UnityApp.ThinkerListDrawer class.
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
    /// Custom property drawer for selecting an thinker from all
    /// known thinkers at runtime.
    /// </summary>
    [CustomPropertyDrawer(typeof(ThinkerListAttribute))]
    public class ThinkerListDrawer : PropertyDrawer
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
            // and convert it to a ThinkerListAttribute
            ThinkerListAttribute thinkerList =
                attribute as ThinkerListAttribute;

            // Get the known thinkers from the ThinkerListAttribute instance
            string[] thinkerNamesList = thinkerList.Thinkers;

            // Get the index of the previously selected thinker
            int index = Mathf.Max(
                0, Array.IndexOf(thinkerNamesList, property.stringValue));

            // Update index in case it changed
            index = EditorGUI.Popup(
                position, property.displayName, index, thinkerNamesList);

            // Update selected thinker in case index changed
            property.stringValue = thinkerNamesList[index];
        }
    }
}
