// System
using System;

// Unity
using UnityEngine;

// GUPS - AntiCheat - Core
using GUPS.AntiCheat.Core.Protected;

// GUPS - AntiCheat
using GUPS.AntiCheat.Detector;
using GUPS.AntiCheat.Settings;

namespace GUPS.AntiCheat.Protected
{
    /// <summary>
    /// Represents a protected boolean, designed to enhance integrity and security by obfuscating its value and incorporating anti-cheat measures.
    /// In most cases, this protected boolean can be used as a drop-in replacement for the default boolean type.
    /// </summary>
    [Serializable]
    public struct ProtectedBool : IProtected, IDisposable, ISerializationCallbackReceiver
    {
        /// <summary>
        /// A struct does not have a default constructor that is called when the structure is created. Therefore, the protected primitive must return 
        /// a default value if it does not have an assigned value.
        /// </summary>
        private bool isInitialized;

        /// <summary>
        /// Gets a value indicating whether the protected value has integrity, i.e., whether it has maintained its original state.
        /// </summary>
        public bool HasIntegrity { get; private set; }

        /// <summary>
        /// The obfuscated value of the protected.
        /// </summary>
        private byte obfuscatedValue;

        /// <summary>
        /// A secret key used to obfuscate the true value.
        /// </summary>
        private Int32 secret;

        /// <summary>
        /// A honeypot pretending to be the original value. If a user attempts to change this value via a cheat/hack engine, you will be notified.
        /// The protected value will retain its true value.
        /// </summary>
        [SerializeField]
        private bool fakeValue;

        /// <summary>
        /// Unity serialization hook. Ensures the correct values are serialized.
        /// </summary>
        public void OnBeforeSerialize()
        {
            this.fakeValue = Value;
        }

        /// <summary>
        /// Unity deserialization hook. Ensures the correct values are deserialized.
        /// </summary>
        public void OnAfterDeserialize()
        {
            this = this.fakeValue;
        }

        /// <summary>
        /// Creates a new protected boolean with the specified initial value.
        /// </summary>
        /// <param name="_Value">The initial value of the protected boolean.</param>
        public ProtectedBool(bool _Value = false)
        {
            // Initialization
            this.isInitialized = true;
            this.obfuscatedValue = 0;
            this.secret = GlobalSettings.RandomProvider.RandomInt32(1, Int32.MaxValue);
            this.fakeValue = _Value;
            this.HasIntegrity = true;

            // Obfuscate the value.
            this.Obfuscate(_Value);
        }

        /// <summary>
        /// Gets and sets the true unencrypted field value.
        /// </summary>
        public bool Value
        {
            get 
            {
                if(!this.isInitialized)
                {
                    return false;
                }

                if(!this.CheckIntegrity())
                {
                    AntiCheatMonitor.Instance.GetDetector<PrimitiveCheatingDetector>()?.OnNext(this);
                }

                return this.UnObfuscate(); 
            }
            set { this.Obfuscate(value); }
        }

        /// <summary>
        /// Obfuscates the specified value, encrypting it with the secret key.
        /// </summary>
        private void Obfuscate(bool _Value)
        {
            // Obfuscate the value.
            byte var_BoolAsByte = (_Value) ? (byte)1 : (byte)0;
            this.obfuscatedValue = (byte)(var_BoolAsByte ^ this.secret);

            // Assign the fake value.
            this.fakeValue = _Value;
        }

        /// <summary>
        /// Unobfuscates the secured value and returns the true unencrypted value.
        /// </summary>
        /// <returns>The true unencrypted value.</returns>
        private bool UnObfuscate()
        {
            // Get the unobfuscated value.
            byte var_BoolAsByte = (byte)(this.obfuscatedValue ^ this.secret);
            return (var_BoolAsByte == 1);
        }

        /// <summary>
        /// Obfuscates the current value, generating a new random secret key.
        /// </summary>
        public void Obfuscate()
        {
            // Unobfuscate the secured value.
            bool var_UnobfuscatedValue = this.UnObfuscate();

            // Create a new random secret.
            this.secret = GlobalSettings.RandomProvider.RandomInt32(1, Int32.MaxValue);

            // Obfuscate the value.
            this.Obfuscate(var_UnobfuscatedValue);
        }

        /// <summary>
        /// Checks the integrity of the protected boolean, detecting if an attacher changed the honeypot fake value.
        /// </summary>
        /// <returns>True if the protected boolean has integrity; otherwise, false.</returns>
        public bool CheckIntegrity()
        {
            // Unobfuscate the secured value.
            bool var_UnobfuscatedValue = this.UnObfuscate();

            // Check if an attacher changed the honeypot fake value.
            if (this.fakeValue != var_UnobfuscatedValue)
            {
                this.HasIntegrity = false;
            }

            // Return the integrity status.
            return this.HasIntegrity;
        }

        /// <summary>
        /// Disposes of the secure and secret values.
        /// </summary>
        public void Dispose()
        {
            this.obfuscatedValue = 0;
            this.secret = 0;
        }

        /// <summary>
        /// Gets the hash code of the protected boolean's true value.
        /// </summary>
        /// <returns>The hash code of the true value.</returns>
        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        /// <summary>
        /// Converts the protected boolean to its string representation.
        /// </summary>
        /// <returns>The string representation of the true value.</returns>
        public override string ToString()
        {
            return this.Value.ToString();
        }

        #region Serialization

        /// <summary>
        /// Used to serialize the protected to the player prefs.
        /// </summary>
        /// <param name="_ObfuscatedValue">The obfuscated value of the protected.</param>
        /// <param name="_Secret">The secret key used to obfuscate the true value.</param>
        internal void Serialize(out byte _ObfuscatedValue, out int _Secret)
        {
            _ObfuscatedValue = this.obfuscatedValue;
            _Secret = this.secret;
        }

        /// <summary>
        /// Used to deserialize the protected from the player prefs.
        /// </summary>
        /// <param name="_ObfuscatedValue">The obfuscated value of the protected.</param>
        /// <param name="_Secret">The secret key used to obfuscate the true value.</param>
        internal void Deserialize(byte _ObfuscatedValue, int _Secret)
        {
            this.obfuscatedValue = _ObfuscatedValue;
            this.secret = _Secret;
            this.fakeValue = this.UnObfuscate();
        }

        #endregion

        #region Implicit operator

        /// <summary>
        /// Implicitly converts a boolean value to a protected boolean.
        /// </summary>
        /// <param name="_Value">The boolean value to convert.</param>
        /// <returns>The corresponding protected boolean.</returns>
        public static implicit operator ProtectedBool(bool _Value)
        {
            return new ProtectedBool(_Value);
        }

        /// <summary>
        /// Implicitly converts a protected boolean to its boolean value.
        /// </summary>
        /// <param name="_Value">The protected boolean to convert.</param>
        /// <returns>The boolean value of the protected boolean.</returns>
        public static implicit operator bool(ProtectedBool _Value)
        {
            return _Value.Value;
        }

        #endregion

        #region Equality operator

        /// <summary>
        /// Checks if two protected booleans are equal based on their true values.
        /// </summary>
        /// <param name="v1">The first protected boolean.</param>
        /// <param name="v2">The second protected boolean.</param>
        /// <returns>True if the true values are equal; otherwise, false.</returns>
        public static bool operator ==(ProtectedBool v1, ProtectedBool v2)
        {
            return v1.Value == v2.Value;
        }

        /// <summary>
        /// Checks if two protected booleans are not equal based on their true values.
        /// </summary>
        /// <param name="v1">The first protected boolean.</param>
        /// <param name="v2">The second protected boolean.</param>
        /// <returns>True if the true values are not equal; otherwise, false.</returns>
        public static bool operator !=(ProtectedBool v1, ProtectedBool v2)
        {
            return v1.Value != v2.Value;
        }

        /// <summary>
        /// Checks if the protected boolean is equal to another object based on their true values.
        /// </summary>
        /// <param name="obj">The object to compare with the protected boolean.</param>
        /// <returns>True if the true values are equal; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is ProtectedBool)
            {
                return this.Value == ((ProtectedBool)obj).Value;
            }
            return this.Value.Equals(obj);
        }

        #endregion
    }
}