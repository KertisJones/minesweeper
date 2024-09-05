// System
using System;

// Unity
using UnityEngine;

namespace GUPS.AntiCheat.Protected.Prefs
{
    /// <summary>
    /// Provides an class for accessing protected int player preferences via properties, offering a more structured approach than 
    /// interacting directly with the static ProtectedPlayerPrefs class. Also allows to easily assign the protected player preferences
    /// in the unity inspector.
    /// </summary>
    [Serializable]
    public class ProtectedIntPref : IProtectedPref<Int32>
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
        private Int32 defaultValue;

        /// <summary>
        /// Gets or sets the value of the player preference.
        /// </summary>
        public Int32 Value
        {
            get
            {
                return ProtectedPlayerPrefs.GetInt(key, this.defaultValue);
            }
            set
            {
                ProtectedPlayerPrefs.SetInt(key, value);
            }
        }
    }
}
