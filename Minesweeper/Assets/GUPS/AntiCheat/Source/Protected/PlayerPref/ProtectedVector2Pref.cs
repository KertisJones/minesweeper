// System
using System;

// Unity
using UnityEngine;

namespace GUPS.AntiCheat.Protected.Prefs
{
    /// <summary>
    /// Provides an class for accessing protected vector2 player preferences via properties, offering a more structured approach than 
    /// interacting directly with the static ProtectedPlayerPrefs class. Also allows to easily assign the protected player preferences
    /// in the unity inspector.
    /// </summary>
    [Serializable]
    public class ProtectedVector2Pref : IProtectedPref<Vector2>
    {
        /// <summary>
        /// Gets the unique key associated with the player preference.
        /// </summary>
        [SerializeField]
        [Tooltip("The unique key associated with the player preference.")]
        private string key;

        /// <summary>
        /// Gets the unique key associated with the player preference.
        /// </summary>
        public String Key => this.Key;

        /// <summary>
        /// The default value if the player preference is not set.
        /// </summary>
        [SerializeField]
        [Tooltip("The default value if the player preference is not set.")]
        private Vector2 defaultValue;

        /// <summary>
        /// Gets or sets the value of the player preference.
        /// </summary>
        public Vector2 Value
        {
            get
            {
                return ProtectedPlayerPrefs.GetVector2(key, this.defaultValue);
            }
            set
            {
                ProtectedPlayerPrefs.SetVector2(key, value);
            }
        }
    }
}
