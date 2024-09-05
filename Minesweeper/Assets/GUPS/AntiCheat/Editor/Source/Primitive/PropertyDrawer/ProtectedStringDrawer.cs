// System
using System;

// Unity
using UnityEditor;
using UnityEngine;

namespace GUPS.AntiCheat.Editor
{
    /// <summary>
    /// Custom property drawer for string.
    /// </summary>
    [CustomPropertyDrawer(typeof(GUPS.AntiCheat.Protected.ProtectedString), true)]
    public class ProtectedStringDrawer : ProtectedPropertyDrawer
    {
        /// <summary>
        /// Overrides the custom gui property method to render the bool.
        /// </summary>
        /// <param name="_Position"></param>
        /// <param name="_Property"></param>
        /// <param name="_Label"></param>
        protected override void OnGUIProperty(Rect _Position, SerializedProperty _Property, GUIContent _Label)
        {
            // Begin check.
            UnityEditor.EditorGUI.BeginChangeCheck();

            // Find the fake value property.
            SerializedProperty var_FakeValue = _Property.FindPropertyRelative("fakeValue");

            // Render.
            String var_Value = UnityEditor.EditorGUI.TextField(_Position, _Label, var_FakeValue.stringValue);

            // End check.
            if (UnityEditor.EditorGUI.EndChangeCheck())
            {
                var_FakeValue.stringValue = var_Value;

                _Property.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
