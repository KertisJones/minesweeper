// Unity
using UnityEditor;
using UnityEngine;

namespace GUPS.AntiCheat.Editor
{
    /// <summary>
    /// Custom property drawers for protected fields.
    /// </summary>
    [CustomPropertyDrawer(typeof(GUPS.AntiCheat.Core.Protected.IProtected))]
    public class ProtectedPropertyDrawer : PropertyDrawer
    {
        /// <summary>
        /// Shared gui method for the drawer.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Calculate rects
            var amountRect = new Rect(position.x, position.y, position.width, position.height);

            // Draw fields
            this.OnGUIProperty(amountRect, property, label);

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        /// <summary>
        /// Custom drawer for a property.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        protected virtual void OnGUIProperty(Rect position, SerializedProperty property, GUIContent label)
        {
        }
    }
}