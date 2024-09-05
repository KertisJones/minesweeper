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
    /// Represents a protected DateTime, designed to enhance integrity and security by obfuscating its value and incorporating anti-cheat measures.
    /// In most cases, this protected DateTime can be used as a drop-in replacement for the default DateTime type.
    /// </summary>
    [Serializable]
    public struct ProtectedDateTime : IProtected, IDisposable, ISerializationCallbackReceiver
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
        /// The encrypted true value represented by a ProtectedInt64.
        /// </summary>
        private ProtectedInt64 obfuscatedInt64;

        /// <summary>
        /// A honeypot pretending to be the original value. If a user attempts to change this value via a cheat/hack engine, you will be notified.
        /// The protected value will retain its true value.
        /// </summary>
        [SerializeField]
        private Int64 fakeValue;

        /// <summary>
        /// Unity serialization hook. Ensures the correct values are serialized.
        /// </summary>
        public void OnBeforeSerialize()
        {
            this.fakeValue = Value.Ticks;
        }

        /// <summary>
        /// Unity deserialization hook. Ensures the correct values are deserialized.
        /// </summary>
        public void OnAfterDeserialize()
        {
            this = new DateTime(this.fakeValue);
        }

        /// <summary>
        /// Creates a new protected DateTime with the specified initial value.
        /// </summary>
        /// <param name="_Value">The initial value of the protected DateTime.</param>
        public ProtectedDateTime(DateTime _Value)
        {
            // Initialization
            this.isInitialized = true;
            this.obfuscatedInt64 = 0;
            this.fakeValue = 0;
            this.HasIntegrity = true;

            // Obfuscate the value.
            this.Obfuscate(_Value);
        }

        /// <summary>
        /// Gets and sets the true unencrypted field value.
        /// </summary>
        public DateTime Value
        {
            get
            {
                if (!this.isInitialized)
                {
                    return new DateTime();
                }

                if (!this.CheckIntegrity())
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
        private void Obfuscate(DateTime _Value)
        {
            // Obfuscate the value.
            this.obfuscatedInt64.Value = _Value.Ticks;

            // Assign the fake value.
            this.fakeValue = _Value.Ticks;
        }

        /// <summary>
        /// Unobfuscates the secured value and returns the true unencrypted value.
        /// </summary>
        /// <returns>The true unencrypted value.</returns>
        private DateTime UnObfuscate()
        {
            // Get the unobfuscated value.
            return new DateTime(this.obfuscatedInt64.Value);
        }

        /// <summary>
        /// Obfuscates the current value, generating a new random secret key.
        /// </summary>
        public void Obfuscate()
        {
            // Obfuscate the value.
            this.obfuscatedInt64.Obfuscate();
        }

        /// <summary>
        /// Checks the integrity of the protected DateTime, detecting if an attacher changed the honeypot fake value.
        /// </summary>
        /// <returns>True if the protected DateTime has integrity; otherwise, false.</returns>
        public bool CheckIntegrity()
        {
            // Unobfuscate the secured value.
            DateTime var_UnobfuscatedValue = this.UnObfuscate();

            // Check if an attacher changed the honeypot fake value.
            if (this.fakeValue != var_UnobfuscatedValue.Ticks)
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
            this.obfuscatedInt64.Dispose();
        }

        /// <summary>
        /// Converts the protected DateTime to its string representation.
        /// </summary>
        /// <returns>The string representation of the true value.</returns>
        public override string ToString()
        {
            return this.Value.ToString();
        }

        /// <summary>
        /// Gets the hash code of the protected DateTime's true value.
        /// </summary>
        /// <returns>The hash code of the true value.</returns>
        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        #region Implicit operator

        /// <summary>
        /// Implicitly converts a DateTime value to a protected DateTime.
        /// </summary>
        /// <param name="_Value">The DateTime value to convert.</param>
        /// <returns>The corresponding protected DateTime.</returns>
        public static implicit operator ProtectedDateTime(DateTime _Value)
        {
            return new ProtectedDateTime(_Value);
        }

        /// <summary>
        /// Implicitly converts a protected DateTime to its DateTime value.
        /// </summary>
        /// <param name="_Value">The protected DateTime to convert.</param>
        /// <returns>The DateTime value of the protected DateTime.</returns>
        public static implicit operator DateTime(ProtectedDateTime _Value)
        {
            return _Value.Value;
        }

        #endregion

        #region Equality operator

        /// <summary>
        /// Checks if two protected DateTimes are equal based on their true values.
        /// </summary>
        /// <param name="v1">The first protected DateTime.</param>
        /// <param name="v2">The second protected DateTime.</param>
        /// <returns>True if the true values are equal; otherwise, false.</returns>
        public static bool operator ==(ProtectedDateTime v1, ProtectedDateTime v2)
        {
            return v1.Value == v2.Value;
        }

        /// <summary>
        /// Checks if two protected DateTimes are not equal based on their true values.
        /// </summary>
        /// <param name="v1">The first protected DateTime.</param>
        /// <param name="v2">The second protected DateTime.</param>
        /// <returns>True if the true values are not equal; otherwise, false.</returns>
        public static bool operator !=(ProtectedDateTime v1, ProtectedDateTime v2)
        {
            return v1.Value != v2.Value;
        }

        /// <summary>
        /// Checks if the protected DateTime is equal to another object based on their true values.
        /// </summary>
        /// <param name="obj">The object to compare with the protected DateTime.</param>
        /// <returns>True if the true values are equal; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is ProtectedDateTime)
            {
                return this.Value == ((ProtectedDateTime)obj).Value;
            }
            return this.Value.Equals(obj);
        }

        #endregion
    }
}