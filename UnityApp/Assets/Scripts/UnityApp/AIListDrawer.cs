using System;
using UnityEngine;
using UnityEditor;

namespace ColorShapeLinks.UnityApp
{
    [CustomPropertyDrawer(typeof(AIList))]
    public class AIListDrawer : PropertyDrawer
    {
        // Draw the property inside the given rect
        public override void OnGUI(
            Rect position, SerializedProperty property, GUIContent label)
        {
            AIList aiList = attribute as AIList;
            string[] aiNamesList = aiList.AIs;

            int index = Mathf.Max(
                0, Array.IndexOf(aiNamesList, property.stringValue));
            index = EditorGUI.Popup(
                position, property.displayName, index, aiNamesList);

            property.stringValue = aiNamesList[index];
        }
    }
}
