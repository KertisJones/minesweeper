// Unity
using UnityEditor;
using UnityEngine;

namespace GUPS.AntiCheat.Editor
{
    /// <summary>
    /// Custom property drawer for float.
    /// </summary>
    [CustomPropertyDrawer(typeof(GUPS.AntiCheat.Protected.ProtectedFloat), true)]
    public class ProtectedFloatDrawer : ProtectedPropertyDrawer
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
            float var_Value = UnityEditor.EditorGUI.FloatField(_Position, _Label, var_FakeValue.floatValue);

            // End check.
            if (UnityEditor.EditorGUI.EndChangeCheck())
            {
                var_FakeValue.floatValue = var_Value;

                _Property.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
