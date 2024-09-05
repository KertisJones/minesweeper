// System
using System;

namespace GUPS.AntiCheat.Protected.Prefs
{
    /// <summary>
    /// Provides an interface for accessing protected player preferences via properties, offering a more structured approach than 
    /// interacting directly with the static ProtectedPlayerPrefs class. Also allows to easily assign the protected player preferences
    /// in the unity inspector.
    /// </summary>
    /// <typeparam name="T">The type of the value stored in the protected player preferences.</typeparam>
    /// <remarks>
    /// <para>
    /// Implementing classes should handle the mapping between the generic type and the actual storage and retrieval of values in the player preferences.
    /// </para>
    /// </remarks>
    public interface IProtectedPref<T>
    {
        /// <summary>
        /// Gets the unique key associated with the player preference.
        /// </summary>
        String Key { get; }

        /// <summary>
        /// Gets or sets the value of the player preference.
        /// </summary>
        T Value { get; set; }
    }
}
