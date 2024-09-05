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
    /// Represents a protected UInt64, designed to enhance integrity and security by obfuscating its value and incorporating anti-cheat measures.
    /// In most cases, this protected UInt64 can be used as a drop-in replacement for the default UInt64 type.
    /// </summary>
    [Serializable]
    public struct ProtectedUInt64 : IProtected, IDisposable, ISerializationCallbackReceiver
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
        /// The encrypted true value.
        /// </summary>
        private UInt64 obfuscatedValue;

        /// <summary>
        /// A secret key the true value gets encrypted with.
        /// </summary>
        private UInt64 secret;

        /// <summary>
        /// A honeypot pretending to be the original value. If some user tries to change this value via a cheat/hack engine, you will get notified.
        /// The protected value will keep its true value.
        /// </summary>
        [SerializeField]
        private UInt64 fakeValue;

        /// <summary>
        /// Unity serialization hook. Ensures the correct values will be serialized.
        /// </summary>
        public void OnBeforeSerialize()
        {
            this.fakeValue = Value;
        }

        /// <summary>
        /// Unity deserialization hook. Ensures the correct values will be deserialized.
        /// </summary>
        public void OnAfterDeserialize()
        {
            this = this.fakeValue;
        }

        /// <summary>
        /// Creates a new protected UInt64 with the specified initial value.
        /// </summary>
        /// <param name="_Value">The initial value of the protected UInt64.</param>
        public ProtectedUInt64(UInt64 _Value = 0)
        {
            // Initialization
            this.isInitialized = true;
            this.obfuscatedValue = 0;
            this.secret = (UInt32)GlobalSettings.RandomProvider.RandomInt32(1, Int32.MaxValue);
            this.fakeValue = 0;
            this.HasIntegrity = true;

            // Obfuscate the value.
            this.Obfuscate(_Value);
        }

        /// <summary>
        /// Gets and sets the true unencrypted field value.
        /// </summary>
        public UInt64 Value
        {
            get
            {
                if (!this.isInitialized)
                {
                    return 0;
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
        private void Obfuscate(UInt64 _Value)
        {
            // Obfuscate the value.
            this.obfuscatedValue = _Value ^ this.secret;

            // Assign the fake value.
            this.fakeValue = _Value;
        }

        /// <summary>
        /// Unobfuscates the secured value and returns the true unencrypted value.
        /// </summary>
        /// <returns>The true unencrypted value.</returns>
        private UInt64 UnObfuscate()
        {
            // Get the unobfuscated value.
            return (UInt64)(this.obfuscatedValue ^ this.secret);
        }

        /// <summary>
        /// Obfuscates the current value, generating a new random secret key.
        /// </summary>
        public void Obfuscate()
        {
            // Unobfuscate the secured value.
            UInt64 var_UnobfuscatedValue = this.UnObfuscate();

            // Create a new random secret.
            this.secret = (UInt32)GlobalSettings.RandomProvider.RandomInt32(1, Int32.MaxValue);

            // Obfuscate the value.
            this.Obfuscate(var_UnobfuscatedValue);
        }

        /// <summary>
        /// Checks the integrity of the protected value, detecting if an attacher changed the honeypot fake value.
        /// </summary>
        /// <returns>True if the protected value has integrity; otherwise, false.</returns>
        public bool CheckIntegrity()
        {
            // Unobfuscate the secured value.
            UInt64 var_UnobfuscatedValue = this.UnObfuscate();

            // Check if an attacher changed the honeypot fake value.
            if (this.fakeValue != var_UnobfuscatedValue)
            {
                this.HasIntegrity = false;
            }

            // Return the integrity status.
            return this.HasIntegrity;
        }

        /// <summary>
        /// Disposes of the secured and secret values.
        /// </summary>
        public void Dispose()
        {
            this.obfuscatedValue = 0;
            this.secret = 0;
        }

        /// <summary>
        /// Converts the protected UInt64 to its string representation.
        /// </summary>
        /// <returns>The string representation of the true value.</returns>
        public override string ToString()
        {
            return this.Value.ToString();
        }

        /// <summary>
        /// Gets the hash code of the protected UInt64's true value.
        /// </summary>
        /// <returns>The hash code of the true value.</returns>
        public override int GetHashCode()
        {
            return (int)this.Value;
        }

        #region Implicit operator

        /// <summary>
        /// Implicitly converts an UInt64 value to a protected UInt64.
        /// </summary>
        /// <param name="_Value">The UInt64 value to convert.</param>
        /// <returns>The corresponding protected UInt64.</returns>
        public static implicit operator ProtectedUInt64(UInt64 _Value)
        {
            return new ProtectedUInt64(_Value);
        }

        /// <summary>
        /// Implicitly converts a protected UInt64 to its UInt64 value.
        /// </summary>
        /// <param name="_Value">The protected UInt64 to convert.</param>
        /// <returns>The UInt64 value of the protected UInt64.</returns>
        public static implicit operator UInt64(ProtectedUInt64 _Value)
        {
            return _Value.Value;
        }

        #endregion

        #region Calculation operator

        /// <summary>
        /// Adds two protected UInt64 values.
        /// </summary>
        /// <param name="v1">The first protected UInt64.</param>
        /// <param name="v2">The second protected UInt64.</param>
        /// <returns>The result of the addition.</returns>
        public static ProtectedUInt64 operator +(ProtectedUInt64 v1, ProtectedUInt64 v2)
        {
            return new ProtectedUInt64(v1.Value + v2.Value);
        }

        /// <summary>
        /// Subtracts the second protected UInt64 from the first.
        /// </summary>
        /// <param name="v1">The first protected UInt64.</param>
        /// <param name="v2">The second protected UInt64.</param>
        /// <returns>The result of the subtraction.</returns>
        public static ProtectedUInt64 operator -(ProtectedUInt64 v1, ProtectedUInt64 v2)
        {
            return new ProtectedUInt64(v1.Value - v2.Value);
        }

        /// <summary>
        /// Multiplies two protected UInt64 values.
        /// </summary>
        /// <param name="v1">The first protected UInt64.</param>
        /// <param name="v2">The second protected UInt64.</param>
        /// <returns>The result of the multiplication.</returns>
        public static ProtectedUInt64 operator *(ProtectedUInt64 v1, ProtectedUInt64 v2)
        {
            return new ProtectedUInt64(v1.Value * v2.Value);
        }

        /// <summary>
        /// Divides the first protected UInt64 by the second.
        /// </summary>
        /// <param name="v1">The first protected UInt64.</param>
        /// <param name="v2">The second protected UInt64.</param>
        /// <returns>The result of the division.</returns>
        public static ProtectedUInt64 operator /(ProtectedUInt64 v1, ProtectedUInt64 v2)
        {
            return new ProtectedUInt64(v1.Value / v2.Value);
        }

        #endregion

        #region Equality operator

        /// <summary>
        /// Checks if two protected UInt64 values are equal based on their true values.
        /// </summary>
        /// <param name="v1">The first protected UInt64.</param>
        /// <param name="v2">The second protected UInt64.</param>
        /// <returns>True if the true values are equal; otherwise, false.</returns>
        public static bool operator ==(ProtectedUInt64 v1, ProtectedUInt64 v2)
        {
            return v1.Value == v2.Value;
        }

        /// <summary>
        /// Checks if two protected UInt64 values are not equal based on their true values.
        /// </summary>
        /// <param name="v1">The first protected UInt64.</param>
        /// <param name="v2">The second protected UInt64.</param>
        /// <returns>True if the true values are not equal; otherwise, false.</returns>
        public static bool operator !=(ProtectedUInt64 v1, ProtectedUInt64 v2)
        {
            return v1.Value != v2.Value;
        }

        /// <summary>
        /// Checks if the protected UInt64 is equal to another object based on their true values.
        /// </summary>
        /// <param name="obj">The object to compare with the protected UInt64.</param>
        /// <returns>True if the true values are equal; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is ProtectedUInt64)
            {
                return this.Value == ((ProtectedUInt64)obj).Value;
            }
            return this.Value.Equals(obj);
        }

        /// <summary>
        /// Compares two protected UInt64 values.
        /// </summary>
        /// <param name="v1">The first protected UInt64.</param>
        /// <param name="v2">The second protected UInt64.</param>
        /// <returns>True if the first value is less than the second; otherwise, false.</returns>
        public static bool operator <(ProtectedUInt64 v1, ProtectedUInt64 v2)
        {
            return v1.Value < v2.Value;
        }

        /// <summary>
        /// Checks if the first protected UInt64 is less than or equal to the second.
        /// </summary>
        /// <param name="v1">The first protected UInt64.</param>
        /// <param name="v2">The second protected UInt64.</param>
        /// <returns>True if the first value is less than or equal to the second; otherwise, false.</returns>
        public static bool operator <=(ProtectedUInt64 v1, ProtectedUInt64 v2)
        {
            return v1.Value <= v2.Value;
        }

        /// <summary>
        /// Checks if the first protected UInt64 is greater than the second.
        /// </summary>
        /// <param name="v1">The first protected UInt64.</param>
        /// <param name="v2">The second protected UInt64.</param>
        /// <returns>True if the first value is greater than the second; otherwise, false.</returns>
        public static bool operator >(ProtectedUInt64 v1, ProtectedUInt64 v2)
        {
            return v1.Value > v2.Value;
        }

        /// <summary>
        /// Checks if the first protected UInt64 is greater than or equal to the second.
        /// </summary>
        /// <param name="v1">The first protected UInt64.</param>
        /// <param name="v2">The second protected UInt64.</param>
        /// <returns>True if the first value is greater than or equal to the second; otherwise, false.</returns>
        public static bool operator >=(ProtectedUInt64 v1, ProtectedUInt64 v2)
        {
            return v1.Value >= v2.Value;
        }

        #endregion
    }
}