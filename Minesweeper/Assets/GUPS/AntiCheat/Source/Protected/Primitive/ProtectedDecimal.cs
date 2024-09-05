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
    /// Represents a protected decimal designed to enhance data integrity and security by obfuscating its value and incorporating anti-cheat measures.
    /// In most cases, this protected decimal can be used as a drop-in replacement for the default decimal type.
    /// </summary>
    [Serializable]
    public struct ProtectedDecimal : IProtected, IDisposable, ISerializationCallbackReceiver
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
        /// The encrypted true value represented as an array of integers.
        /// </summary>
        private int[] obfuscatedValues;

        /// <summary>
        /// A honeypot pretending to be the original value. If a user attempts to change this value via a cheat/hack engine, notifications will be triggered.
        /// The protected value will retain its true value.
        /// </summary>
        [SerializeField]
        private double fakeValue;

        /// <summary>
        /// Unity serialization hook. Ensures the correct values will be serialized.
        /// </summary>
        public void OnBeforeSerialize()
        {
            this.fakeValue = (double)this.Value;
        }

        /// <summary>
        /// Unity deserialization hook. Ensures the correct values will be deserialized.
        /// </summary>
        public void OnAfterDeserialize()
        {
            this = (decimal)this.fakeValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtectedDecimal"/> struct with the specified initial value.
        /// </summary>
        /// <param name="_Value">The initial value of the protected decimal.</param>
        public ProtectedDecimal(decimal _Value = 0)
        {
            // Initialization
            this.isInitialized = true;
            this.obfuscatedValues = decimal.GetBits(_Value);
            this.HasIntegrity = true;

            // Setup fake value.
            this.fakeValue = (double)_Value;
        }

        /// <summary>
        /// Gets and sets the true unencrypted field value.
        /// </summary>
        public decimal Value
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
        /// <param name="_Value">The value to be obfuscated.</param>
        private void Obfuscate(decimal _Value)
        {
            // Obfuscate the value.
            this.obfuscatedValues = decimal.GetBits(_Value);

            // Assign the fake value.
            this.fakeValue = (double)_Value;
        }

        /// <summary>
        /// Unobfuscates the secured value and returns the true unencrypted value.
        /// </summary>
        /// <returns>The true unencrypted value.</returns>
        private decimal UnObfuscate()
        {
            // Get the unobfuscated value.
            return new decimal(this.obfuscatedValues);
        }

        /// <summary>
        /// Obfuscates the current value, generating a new random secret key.
        /// </summary>
        public void Obfuscate()
        {
            // Unobfuscate the secured value.
            decimal var_UnobfuscatedValue = this.UnObfuscate();

            // Obfuscate the value.
            this.Obfuscate(var_UnobfuscatedValue);
        }

        /// <summary>
        /// Checks the integrity of the protected value, detecting if an attacker changed the honeypot fake value.
        /// </summary>
        /// <returns>True if the protected value has integrity; otherwise, false.</returns>
        public bool CheckIntegrity()
        {
            // Unobfuscate the secured value.
            decimal var_UnobfuscatedValue = this.UnObfuscate();

            // Check if an attacker changed the honeypot fake value.
            if (this.fakeValue != (double)var_UnobfuscatedValue)
            {
                this.HasIntegrity = false;
            }

            // Return the integrity status.
            return this.HasIntegrity;
        }

        /// <summary>
        /// Disposes of the secured values array.
        /// </summary>
        public void Dispose()
        {
            this.obfuscatedValues = null;
        }

        /// <summary>
        /// Returns a string representation of the protected decimal's true value.
        /// </summary>
        /// <returns>The string representation of the true value.</returns>
        public override string ToString()
        {
            return this.Value.ToString();
        }

        /// <summary>
        /// Gets the hash code of the protected decimal's true value.
        /// </summary>
        /// <returns>The hash code of the true value.</returns>
        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        #region Implicit operator

        /// <summary>
        /// Implicitly converts a decimal to a <see cref="ProtectedDecimal"/>.
        /// </summary>
        /// <param name="_Value">The decimal value to be converted.</param>
        /// <returns>A new instance of <see cref="ProtectedDecimal"/> with the specified decimal value.</returns>
        public static implicit operator ProtectedDecimal(decimal _Value)
        {
            return new ProtectedDecimal(_Value);
        }

        /// <summary>
        /// Implicitly converts a <see cref="ProtectedDecimal"/> to a decimal.
        /// </summary>
        /// <param name="_Value">The <see cref="ProtectedDecimal"/> value to be converted.</param>
        /// <returns>The decimal value of the specified <see cref="ProtectedDecimal"/>.</returns>
        public static implicit operator decimal(ProtectedDecimal _Value)
        {
            return _Value.Value;
        }

        /// <summary>
        /// Implicitly converts a <see cref="ProtectedDecimal"/> to a <see cref="ProtectedDouble"/>.
        /// </summary>
        /// <param name="_Value">The <see cref="ProtectedDecimal"/> value to be converted.</param>
        /// <returns>A new instance of <see cref="ProtectedDouble"/> with the converted value.</returns>
        public static implicit operator ProtectedDouble(ProtectedDecimal _Value)
        {
            return new ProtectedDouble((double)_Value.Value);
        }

        /// <summary>
        /// Implicitly converts a <see cref="ProtectedDouble"/> to a <see cref="ProtectedDecimal"/>.
        /// </summary>
        /// <param name="_Value">The <see cref="ProtectedDouble"/> value to be converted.</param>
        /// <returns>A new instance of <see cref="ProtectedDecimal"/> with the converted value.</returns>
        public static implicit operator ProtectedDecimal(ProtectedDouble _Value)
        {
            return new ProtectedDecimal((decimal)_Value.Value);
        }

        /// <summary>
        /// Implicitly converts a <see cref="ProtectedDecimal"/> to a <see cref="ProtectedInt16"/>.
        /// </summary>
        /// <param name="_Value">The <see cref="ProtectedDecimal"/> value to be converted.</param>
        /// <returns>A new instance of <see cref="ProtectedInt16"/> with the converted value.</returns>
        public static implicit operator ProtectedInt16(ProtectedDecimal _Value)
        {
            return new ProtectedInt16((Int16)_Value.Value);
        }

        /// <summary>
        /// Implicitly converts a <see cref="ProtectedInt16"/> to a <see cref="ProtectedDecimal"/>.
        /// </summary>
        /// <param name="_Value">The <see cref="ProtectedInt16"/> value to be converted.</param>
        /// <returns>A new instance of <see cref="ProtectedDecimal"/> with the converted value.</returns>
        public static implicit operator ProtectedDecimal(ProtectedInt16 _Value)
        {
            return new ProtectedDecimal((decimal)_Value.Value);
        }

        /// <summary>
        /// Implicitly converts a <see cref="ProtectedDecimal"/> to a <see cref="ProtectedInt32"/>.
        /// </summary>
        /// <param name="_Value">The <see cref="ProtectedDecimal"/> value to be converted.</param>
        /// <returns>A new instance of <see cref="ProtectedInt32"/> with the converted value.</returns>
        public static implicit operator ProtectedInt32(ProtectedDecimal _Value)
        {
            return new ProtectedInt32((Int32)_Value.Value);
        }

        /// <summary>
        /// Implicitly converts a <see cref="ProtectedInt32"/> to a <see cref="ProtectedDecimal"/>.
        /// </summary>
        /// <param name="_Value">The <see cref="ProtectedInt32"/> value to be converted.</param>
        /// <returns>A new instance of <see cref="ProtectedDecimal"/> with the converted value.</returns>
        public static implicit operator ProtectedDecimal(ProtectedInt32 _Value)
        {
            return new ProtectedDecimal((decimal)_Value.Value);
        }

        /// <summary>
        /// Implicitly converts a <see cref="ProtectedDecimal"/> to a <see cref="ProtectedInt64"/>.
        /// </summary>
        /// <param name="_Value">The <see cref="ProtectedDecimal"/> value to be converted.</param>
        /// <returns>A new instance of <see cref="ProtectedInt64"/> with the converted value.</returns>
        public static implicit operator ProtectedInt64(ProtectedDecimal _Value)
        {
            return new ProtectedInt64((Int64)_Value.Value);
        }

        /// <summary>
        /// Implicitly converts a <see cref="ProtectedInt64"/> to a <see cref="ProtectedDecimal"/>.
        /// </summary>
        /// <param name="_Value">The <see cref="ProtectedInt64"/> value to be converted.</param>
        /// <returns>A new instance of <see cref="ProtectedDecimal"/> with the converted value.</returns>
        public static implicit operator ProtectedDecimal(ProtectedInt64 _Value)
        {
            return new ProtectedDecimal((decimal)_Value.Value);
        }

        /// <summary>
        /// Implicitly converts a <see cref="ProtectedDecimal"/> to a <see cref="ProtectedFloat"/>.
        /// </summary>
        /// <param name="_Value">The <see cref="ProtectedDecimal"/> value to be converted.</param>
        /// <returns>A new instance of <see cref="ProtectedFloat"/> with the converted value.</returns>
        public static implicit operator ProtectedFloat(ProtectedDecimal _Value)
        {
            return new ProtectedFloat((float)_Value.Value);
        }

        /// <summary>
        /// Implicitly converts a <see cref="ProtectedFloat"/> to a <see cref="ProtectedDecimal"/>.
        /// </summary>
        /// <param name="_Value">The <see cref="ProtectedFloat"/> value to be converted.</param>
        /// <returns>A new instance of <see cref="ProtectedDecimal"/> with the converted value.</returns>
        public static implicit operator ProtectedDecimal(ProtectedFloat _Value)
        {
            return new ProtectedDecimal((decimal)_Value.Value);
        }

        #endregion

        #region Equality operator

        /// <summary>
        /// Checks if two <see cref="ProtectedDecimal"/> instances are equal.
        /// </summary>
        /// <param name="v1">The first <see cref="ProtectedDecimal"/> instance.</param>
        /// <param name="v2">The second <see cref="ProtectedDecimal"/> instance.</param>
        /// <returns>True if the values are equal; otherwise, false.</returns>
        public static bool operator ==(ProtectedDecimal v1, ProtectedDecimal v2)
        {
            return v1.Value == v2.Value;
        }

        /// <summary>
        /// Checks if two <see cref="ProtectedDecimal"/> instances are not equal.
        /// </summary>
        /// <param name="v1">The first <see cref="ProtectedDecimal"/> instance.</param>
        /// <param name="v2">The second <see cref="ProtectedDecimal"/> instance.</param>
        /// <returns>True if the values are not equal; otherwise, false.</returns>
        public static bool operator !=(ProtectedDecimal v1, ProtectedDecimal v2)
        {
            return v1.Value != v2.Value;
        }

        /// <summary>
        /// Checks if the current <see cref="ProtectedDecimal"/> is equal to the specified object.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>True if the object is a <see cref="ProtectedDecimal"/> and has the same value; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is ProtectedDecimal)
            {
                return this.Value == ((ProtectedDecimal)obj).Value;
            }
            return this.Value.Equals(obj);
        }

        /// <summary>
        /// Checks if the value of the current <see cref="ProtectedDecimal"/> is less than the value of another.
        /// </summary>
        /// <param name="v1">The first <see cref="ProtectedDecimal"/> instance.</param>
        /// <param name="v2">The second <see cref="ProtectedDecimal"/> instance.</param>
        /// <returns>True if the value of the first instance is less than the value of the second instance; otherwise, false.</returns>
        public static bool operator <(ProtectedDecimal v1, ProtectedDecimal v2)
        {
            return v1.Value < v2.Value;
        }

        /// <summary>
        /// Checks if the value of the current <see cref="ProtectedDecimal"/> is less than or equal to the value of another.
        /// </summary>
        /// <param name="v1">The first <see cref="ProtectedDecimal"/> instance.</param>
        /// <param name="v2">The second <see cref="ProtectedDecimal"/> instance.</param>
        /// <returns>True if the value of the first instance is less than or equal to the value of the second instance; otherwise, false.</returns>
        public static bool operator <=(ProtectedDecimal v1, ProtectedDecimal v2)
        {
            return v1.Value <= v2.Value;
        }

        /// <summary>
        /// Checks if the value of the current <see cref="ProtectedDecimal"/> is greater than the value of another.
        /// </summary>
        /// <param name="v1">The first <see cref="ProtectedDecimal"/> instance.</param>
        /// <param name="v2">The second <see cref="ProtectedDecimal"/> instance.</param>
        /// <returns>True if the value of the first instance is greater than the value of the second instance; otherwise, false.</returns>
        public static bool operator >(ProtectedDecimal v1, ProtectedDecimal v2)
        {
            return v1.Value > v2.Value;
        }

        /// <summary>
        /// Checks if the value of the current <see cref="ProtectedDecimal"/> is greater than or equal to the value of another.
        /// </summary>
        /// <param name="v1">The first <see cref="ProtectedDecimal"/> instance.</param>
        /// <param name="v2">The second <see cref="ProtectedDecimal"/> instance.</param>
        /// <returns>True if the value of the first instance is greater than or equal to the value of the second instance; otherwise, false.</returns>
        public static bool operator >=(ProtectedDecimal v1, ProtectedDecimal v2)
        {
            return v1.Value >= v2.Value;
        }

        #endregion
    }
}