// Unity
using UnityEngine;

namespace GUPS.AntiCheat.Editor.Helper
{
    /// <summary>
    /// Helper class for gui styles.
    /// </summary>
    public static class StyleHelper
    {
        /// <summary>
        /// Returns the style for a dark background.
        /// </summary>
        public static GUIStyle DarkBackground
        {
            get
            {
                GUIStyle var_GUIStyle = new GUIStyle();
                var_GUIStyle.normal.background = TextureHelper.MakeTexture(1, 1, new Color(0.1f, 0.1f, 0.1f, 0.25f));
                return var_GUIStyle;
            }
        }
    }
}
