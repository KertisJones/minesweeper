// Unity
using UnityEditor;
using UnityEngine;

namespace GUPS.AntiCheat.Editor
{
    /// <summary>
    /// Custom property drawer for quaternion.
    /// </summary>
    [CustomPropertyDrawer(typeof(GUPS.AntiCheat.Protected.ProtectedQuaternion), true)]
    public class ProtectedQuaternionDrawer : ProtectedPropertyDrawer
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
            Vector4 var_Value = UnityEditor.EditorGUI.Vector4Field(_Position, _Label, Helper_QuaternionToVector4(var_FakeValue.quaternionValue));

            // End check.
            if (UnityEditor.EditorGUI.EndChangeCheck())
            {
                var_FakeValue.quaternionValue = Helper_Vector4ToQuaternion(var_Value);

                _Property.serializedObject.ApplyModifiedProperties();
            }
        }

        /// <summary>
        /// Helper class to convert a Vector4 to a Quaternion.
        /// </summary>
        /// <param name="_Value"></param>
        /// <returns></returns>
        private static Quaternion Helper_Vector4ToQuaternion(Vector4 _Value)
        {
            return new Quaternion(_Value.x, _Value.y, _Value.z, _Value.w);
        }

        /// <summary>
        /// Helper class to convert a Quaternion to a Vector4.
        /// </summary>
        /// <param name="_Value"></param>
        /// <returns></returns>
        private static Vector4 Helper_QuaternionToVector4(Quaternion _Value)
        {
            return new Vector4(_Value.x, _Value.y, _Value.z, _Value.w);
        }
    }
}
