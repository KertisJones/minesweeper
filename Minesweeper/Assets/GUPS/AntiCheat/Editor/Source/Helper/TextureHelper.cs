// Unity
using UnityEngine;

namespace GUPS.AntiCheat.Editor.Helper
{
    /// <summary>
    /// A helper class for creating textures.
    /// </summary>
    public static class TextureHelper
    {
        /// <summary>
        /// Creates a texture with the given width, height and color.
        /// </summary>
        /// <param name="_Width">The width of the texture.</param>
        /// <param name="_Height">The height of the texture.</param>
        /// <param name="_Color">The color of the texture.</param>
        /// <returns>Returns the created texture.</returns>
        public static Texture2D MakeTexture(int _Width, int _Height, Color _Color)
        {
            Color[] pix = new Color[_Width * _Height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = _Color;

            Texture2D result = new Texture2D(_Width, _Height, TextureFormat.ARGB32, false);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }
    }
}
