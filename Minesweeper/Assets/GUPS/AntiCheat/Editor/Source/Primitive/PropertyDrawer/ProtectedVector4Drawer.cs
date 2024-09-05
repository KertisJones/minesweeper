// Unity
using UnityEditor;
using UnityEngine;

namespace GUPS.AntiCheat.Editor
{
    /// <summary>
    /// Custom property drawer for vector4.
    /// </summary>
    [CustomPropertyDrawer(typeof(GUPS.AntiCheat.Protected.ProtectedVector4), true)]
    public class ProtectedVector4Drawer : ProtectedPropertyDrawer
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
            Vector4 var_Value = UnityEditor.EditorGUI.Vector4Field(_Position, _Label, var_FakeValue.vector4Value);

            // End check.
            if (UnityEditor.EditorGUI.EndChangeCheck())
            {
                var_FakeValue.vector4Value = var_Value;

                _Property.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
