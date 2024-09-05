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
    /// Represents a protected double value that is encrypted and includes mechanisms to maintain integrity.
    /// In most scenarios, this type can be used as a drop-in replacement for the default double type.
    /// </summary>
    [Serializable]
    public struct ProtectedDouble : IProtected, IDisposable, ISerializationCallbackReceiver
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
        private ULongDouble obfuscatedValue;

        /// <summary>
        /// Used for calculation of the long/double values for the secured value.
        /// </summary>
        private ULongDouble manager;

        /// <summary>
        /// A secret key the true value gets encrypted with.
        /// </summary>
        private UInt64 secret;

        /// <summary>
        /// A honeypot pretending to be the original value. If a user attempts to change this value via a cheat/hack engine,
        /// you will be notified. The protected value will retain its true value.
        /// </summary>
        [SerializeField]
        private double fakeValue;

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
        /// Creates a new protected double with the specified value.
        /// </summary>
        /// <param name="value">The initial value of the protected double.</param>
        public ProtectedDouble(double value = 0)
        {
            // Initialization
            this.isInitialized = true;
            this.secret = (ulong)GlobalSettings.RandomProvider.RandomInt32(1, Int32.MaxValue);

            //
            this.obfuscatedValue.longValue = 0;
            this.obfuscatedValue.doubleValue = value;
            this.obfuscatedValue.longValue = this.obfuscatedValue.longValue ^ this.secret;

            //
            this.manager.longValue = 0;
            this.manager.doubleValue = 0;

            //
            this.HasIntegrity = true;

            //
            this.fakeValue = value;
        }

        /// <summary>
        /// Gets and sets the true unencrypted field value.
        /// </summary>
        public double Value
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
        private void Obfuscate(double _Value)
        {
            // Obfuscate the value.
            this.manager.doubleValue = _Value;
            this.manager.longValue = this.manager.longValue ^ this.secret;
            this.obfuscatedValue.doubleValue = this.manager.doubleValue;

            // Assign the fake value.
            this.fakeValue = _Value;
        }

        /// <summary>
        /// Unobfuscates the secured value and returns the true unencrypted value.
        /// </summary>
        /// <returns>The true unencrypted value.</returns>
        private double UnObfuscate()
        {
            // Get the unobfuscated value.
            this.manager.longValue = this.obfuscatedValue.longValue ^ this.secret;

            // Return the unobfuscated value.
            return this.manager.doubleValue;
        }

        /// <summary>
        /// Obfuscates the current value, generating a new random secret key.
        /// </summary>
        public void Obfuscate()
        {
            // Unobfuscate the secured value.
            double var_UnobfuscatedValue = this.UnObfuscate();

            // Create a new random secret.
            this.secret = (ulong)GlobalSettings.RandomProvider.RandomInt32(1, Int32.MaxValue);

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
            double var_UnobfuscatedValue = this.UnObfuscate();

            // Check if an attacker changed the honeypot fake value.
            if (this.fakeValue != var_UnobfuscatedValue)
            {
                this.HasIntegrity = false;
            }

            // Return the integrity status.
            return this.HasIntegrity;
        }

        /// <summary>
        /// Releases the resources used by the <see cref="ProtectedDouble"/>.
        /// </summary>
        public void Dispose()
        {
            this.obfuscatedValue.longValue = 0;
            this.manager.longValue = 0;
            this.secret = 0;
        }

        /// <summary>
        /// Returns a string that represents the current <see cref="ProtectedDouble"/>.
        /// </summary>
        /// <returns>A string representation of the protected double value.</returns>
        public override string ToString()
        {
            return this.Value.ToString();
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current <see cref="ProtectedDouble"/>.</returns>
        public override int GetHashCode()
        {
            return this.obfuscatedValue.doubleValue.GetHashCode();
        }

        #region Implicit operators

        /// <summary>
        /// Implicitly converts a double to a <see cref="ProtectedDouble"/>.
        /// </summary>
        /// <param name="_Value">The double value to be converted.</param>
        /// <returns>A new instance of <see cref="ProtectedDouble"/> with the converted value.</returns>
        public static implicit operator ProtectedDouble(double _Value)
        {
            return new ProtectedDouble(_Value);
        }

        /// <summary>
        /// Implicitly converts a <see cref="ProtectedDouble"/> to a double.
        /// </summary>
        /// <param name="_Value">The <see cref="ProtectedDouble"/> to be converted.</param>
        /// <returns>The unencrypted double value.</returns>
        public static implicit operator double(ProtectedDouble _Value)
        {
            return _Value.Value;
        }

        /// <summary>
        /// Implicitly converts a <see cref="ProtectedDouble"/> to a <see cref="ProtectedInt16"/>.
        /// </summary>
        /// <param name="_Value">The <see cref="ProtectedDouble"/> to be converted.</param>
        /// <returns>A new instance of <see cref="ProtectedInt16"/> with the converted value.</returns>
        public static implicit operator ProtectedInt16(ProtectedDouble _Value)
        {
            return new ProtectedInt16((Int16)_Value.Value);
        }

        /// <summary>
        /// Implicitly converts a <see cref="ProtectedInt16"/> to a <see cref="ProtectedDouble"/>.
        /// </summary>
        /// <param name="_Value">The <see cref="ProtectedInt16"/> to be converted.</param>
        /// <returns>A new instance of <see cref="ProtectedDouble"/> with the converted value.</returns>
        public static implicit operator ProtectedDouble(ProtectedInt16 _Value)
        {
            return new ProtectedDouble((double)_Value.Value);
        }

        /// <summary>
        /// Implicitly converts a <see cref="ProtectedDouble"/> to a <see cref="ProtectedInt32"/>.
        /// </summary>
        /// <param name="_Value">The <see cref="ProtectedDouble"/> to be converted.</param>
        /// <returns>A new instance of <see cref="ProtectedInt32"/> with the converted value.</returns>
        public static implicit operator ProtectedInt32(ProtectedDouble _Value)
        {
            return new ProtectedInt32((Int32)_Value.Value);
        }

        /// <summary>
        /// Implicitly converts a <see cref="ProtectedInt32"/> to a <see cref="ProtectedDouble"/>.
        /// </summary>
        /// <param name="_Value">The <see cref="ProtectedInt32"/> to be converted.</param>
        /// <returns>A new instance of <see cref="ProtectedDouble"/> with the converted value.</returns>
        public static implicit operator ProtectedDouble(ProtectedInt32 _Value)
        {
            return new ProtectedDouble((double)_Value.Value);
        }

        /// <summary>
        /// Implicitly converts a <see cref="ProtectedDouble"/> to a <see cref="ProtectedInt64"/>.
        /// </summary>
        /// <param name="_Value">The <see cref="ProtectedDouble"/> to be converted.</param>
        /// <returns>A new instance of <see cref="ProtectedInt64"/> with the converted value.</returns>
        public static implicit operator ProtectedInt64(ProtectedDouble _Value)
        {
            return new ProtectedInt64((Int64)_Value.Value);
        }

        /// <summary>
        /// Implicitly converts a <see cref="ProtectedInt64"/> to a <see cref="ProtectedDouble"/>.
        /// </summary>
        /// <param name="_Value">The <see cref="ProtectedInt64"/> to be converted.</param>
        /// <returns>A new instance of <see cref="ProtectedDouble"/> with the converted value.</returns>
        public static implicit operator ProtectedDouble(ProtectedInt64 _Value)
        {
            return new ProtectedDouble((double)_Value.Value);
        }

        /// <summary>
        /// Implicitly converts a <see cref="ProtectedDouble"/> to a <see cref="ProtectedFloat"/>.
        /// </summary>
        /// <param name="_Value">The <see cref="ProtectedDouble"/> to be converted.</param>
        /// <returns>A new instance of <see cref="ProtectedFloat"/> with the converted value.</returns>
        public static implicit operator ProtectedFloat(ProtectedDouble _Value)
        {
            return new ProtectedFloat((float)_Value.Value);
        }

        /// <summary>
        /// Implicitly converts a <see cref="ProtectedFloat"/> to a <see cref="ProtectedDouble"/>.
        /// </summary>
        /// <param name="_Value">The <see cref="ProtectedFloat"/> to be converted.</param>
        /// <returns>A new instance of <see cref="ProtectedDouble"/> with the converted value.</returns>
        public static implicit operator ProtectedDouble(ProtectedFloat _Value)
        {
            return new ProtectedDouble((double)_Value.Value);
        }

        #endregion

        #region Calculation operations

        /// <summary>
        /// Adds two <see cref="ProtectedDouble"/> instances.
        /// </summary>
        /// <param name="v1">The first <see cref="ProtectedDouble"/> instance.</param>
        /// <param name="v2">The second <see cref="ProtectedDouble"/> instance.</param>
        /// <returns>A new <see cref="ProtectedDouble"/> instance representing the sum of the two values.</returns>
        public static ProtectedDouble operator +(ProtectedDouble v1, ProtectedDouble v2)
        {
            return new ProtectedDouble(v1.Value + v2.Value);
        }

        /// <summary>
        /// Subtracts one <see cref="ProtectedDouble"/> instance from another.
        /// </summary>
        /// <param name="v1">The first <see cref="ProtectedDouble"/> instance.</param>
        /// <param name="v2">The second <see cref="ProtectedDouble"/> instance.</param>
        /// <returns>A new <see cref="ProtectedDouble"/> instance representing the difference of the two values.</returns>
        public static ProtectedDouble operator -(ProtectedDouble v1, ProtectedDouble v2)
        {
            return new ProtectedDouble(v1.Value - v2.Value);
        }

        /// <summary>
        /// Multiplies two <see cref="ProtectedDouble"/> instances.
        /// </summary>
        /// <param name="v1">The first <see cref="ProtectedDouble"/> instance.</param>
        /// <param name="v2">The second <see cref="ProtectedDouble"/> instance.</param>
        /// <returns>A new <see cref="ProtectedDouble"/> instance representing the product of the two values.</returns>
        public static ProtectedDouble operator *(ProtectedDouble v1, ProtectedDouble v2)
        {
            return new ProtectedDouble(v1.Value * v2.Value);
        }

        /// <summary>
        /// Divides one <see cref="ProtectedDouble"/> instance by another.
        /// </summary>
        /// <param name="v1">The dividend <see cref="ProtectedDouble"/> instance.</param>
        /// <param name="v2">The divisor <see cref="ProtectedDouble"/> instance.</param>
        /// <returns>A new <see cref="ProtectedDouble"/> instance representing the quotient of the division.</returns>
        public static ProtectedDouble operator /(ProtectedDouble v1, ProtectedDouble v2)
        {
            return new ProtectedDouble(v1.Value / v2.Value);
        }

        #endregion

        #region Equality operations

        /// <summary>
        /// Checks if two <see cref="ProtectedDouble"/> instances are equal.
        /// </summary>
        /// <param name="v1">The first <see cref="ProtectedDouble"/> instance.</param>
        /// <param name="v2">The second <see cref="ProtectedDouble"/> instance.</param>
        /// <returns>True if the values of the two instances are equal; otherwise, false.</returns>
        public static bool operator ==(ProtectedDouble v1, ProtectedDouble v2)
        {
            return v1.Value == v2.Value;
        }

        /// <summary>
        /// Checks if two <see cref="ProtectedDouble"/> instances are not equal.
        /// </summary>
        /// <param name="v1">The first <see cref="ProtectedDouble"/> instance.</param>
        /// <param name="v2">The second <see cref="ProtectedDouble"/> instance.</param>
        /// <returns>True if the values of the two instances are not equal; otherwise, false.</returns>
        public static bool operator !=(ProtectedDouble v1, ProtectedDouble v2)
        {
            return v1.Value != v2.Value;
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to the current <see cref="ProtectedDouble"/>.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>True if the specified object is equal to the current instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is ProtectedDouble)
            {
                return this.Value == ((ProtectedDouble)obj).Value;
            }
            return this.Value.Equals(obj);
        }

        /// <summary>
        /// Compares two <see cref="ProtectedDouble"/> instances for less than.
        /// </summary>
        /// <param name="v1">The first <see cref="ProtectedDouble"/> instance.</param>
        /// <param name="v2">The second <see cref="ProtectedDouble"/> instance.</param>
        /// <returns>True if the value of the first instance is less than the value of the second instance; otherwise, false.</returns>
        public static bool operator <(ProtectedDouble v1, ProtectedDouble v2)
        {
            return v1.Value < v2.Value;
        }

        /// <summary>
        /// Compares two <see cref="ProtectedDouble"/> instances for less than or equal.
        /// </summary>
        /// <param name="v1">The first <see cref="ProtectedDouble"/> instance.</param>
        /// <param name="v2">The second <see cref="ProtectedDouble"/> instance.</param>
        /// <returns>True if the value of the first instance is less than or equal to the value of the second instance; otherwise, false.</returns>
        public static bool operator <=(ProtectedDouble v1, ProtectedDouble v2)
        {
            return v1.Value <= v2.Value;
        }

        /// <summary>
        /// Compares two <see cref="ProtectedDouble"/> instances for greater than.
        /// </summary>
        /// <param name="v1">The first <see cref="ProtectedDouble"/> instance.</param>
        /// <param name="v2">The second <see cref="ProtectedDouble"/> instance.</param>
        /// <returns>True if the value of the first instance is greater than the value of the second instance; otherwise, false.</returns>
        public static bool operator >(ProtectedDouble v1, ProtectedDouble v2)
        {
            return v1.Value > v2.Value;
        }

        /// <summary>
        /// Compares two <see cref="ProtectedDouble"/> instances for greater than or equal.
        /// </summary>
        /// <param name="v1">The first <see cref="ProtectedDouble"/> instance.</param>
        /// <param name="v2">The second <see cref="ProtectedDouble"/> instance.</param>
        /// <returns>True if the value of the first instance is greater than or equal to the value of the second instance; otherwise, false.</returns>
        public static bool operator >=(ProtectedDouble v1, ProtectedDouble v2)
        {
            return v1.Value >= v2.Value;
        }

        #endregion
    }
}