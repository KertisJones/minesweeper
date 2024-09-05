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
    /// Represents a protected float. In almost all cases, you can replace your default type with the protected one for added security.
    /// </summary>
    [Serializable]
    public struct ProtectedFloat : IProtected, IDisposable, ISerializationCallbackReceiver
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
        private UIntFloat obfuscatedValue;

        /// <summary>
        /// Used for calculation of the int/float values for the secured value.
        /// </summary>
        private UIntFloat manager;

        /// <summary>
        /// A secret key used to obfuscate the true value.
        /// </summary>
        private UInt32 secret;

        /// <summary>
        /// A honeypot pretending to be the original value. If some user tries to change this value via a cheat/hack engine, you will get notified.
        /// The protected value will keep its true value.
        /// </summary>
        [SerializeField]
        private float fakeValue;

        /// <summary>
        /// Unity serialization hook. So the right values will be serialized.
        /// </summary>
        public void OnBeforeSerialize()
        {
            this.fakeValue = Value;
        }

        /// <summary>
        /// Unity deserialization hook. So the right values will be deserialized.
        /// </summary>
        public void OnAfterDeserialize()
        {
            this = this.fakeValue;
        }

        /// <summary>
        /// Create a new protected float with _Value.
        /// </summary>
        /// <param name="_Value">The initial value for the protected float.</param>
        public ProtectedFloat(float value = 0)
        {
            // Initialization
            this.isInitialized = true;
            this.secret = (UInt32)GlobalSettings.RandomProvider.RandomInt32(1, Int32.MaxValue);

            //
            this.obfuscatedValue.intValue = 0;
            this.obfuscatedValue.floatValue = value;
            this.obfuscatedValue.intValue = this.obfuscatedValue.intValue ^ this.secret;

            //
            this.manager.intValue = 0;
            this.manager.floatValue = 0;

            //
            this.HasIntegrity = true;

            //
            this.fakeValue = value;
        }

        /// <summary>
        /// Gets and sets the true unencrypted field value.
        /// </summary>
        public float Value
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
        private void Obfuscate(float _Value)
        {
            // Obfuscate the value.
            this.manager.floatValue = _Value;
            this.manager.intValue = this.manager.intValue ^ this.secret;
            this.obfuscatedValue.floatValue = this.manager.floatValue;

            // Assign the fake value.
            this.fakeValue = _Value;
        }

        /// <summary>
        /// Unobfuscates the secured value and returns the true unencrypted value.
        /// </summary>
        /// <returns>The true unencrypted value.</returns>
        private float UnObfuscate()
        {
            // Get the unobfuscated value.
            this.manager.intValue = this.obfuscatedValue.intValue ^ this.secret;

            // Return the unobfuscated value.
            return this.manager.floatValue;
        }

        /// <summary>
        /// Obfuscates the current value, generating a new random secret key.
        /// </summary>
        public void Obfuscate()
        {
            // Unobfuscate the secured value.
            float var_UnobfuscatedValue = this.UnObfuscate();

            // Create a new random secret.
            this.secret = (UInt32)GlobalSettings.RandomProvider.RandomInt32(1, Int32.MaxValue);

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
            float var_UnobfuscatedValue = this.UnObfuscate();

            // Check if an attacker changed the honeypot fake value.
            if (this.fakeValue != var_UnobfuscatedValue)
            {
                this.HasIntegrity = false;
            }

            // Return the integrity status.
            return this.HasIntegrity;
        }

        /// <summary>
        /// Disposes of the resources associated with the protected float.
        /// </summary>
        public void Dispose()
        {
            this.obfuscatedValue.intValue = 0;
            this.manager.intValue = 0;
            this.secret = 0;
        }

        /// <summary>
        /// Returns a string representation of the protected float.
        /// </summary>
        /// <returns>A string representation of the protected float.</returns>
        public override string ToString()
        {
            return this.Value.ToString();
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current <see cref="ProtectedFloat"/>.</returns>
        public override int GetHashCode()
        {
            return this.obfuscatedValue.floatValue.GetHashCode();
        }

        #region Serialization

        /// <summary>
        /// Used to serialize the protected to the player prefs.
        /// </summary>
        /// <param name="_ObfuscatedValue">The obfuscated value of the protected.</param>
        /// <param name="_Secret">The secret key used to obfuscate the true value.</param>
        internal void Serialize(out uint _ObfuscatedValue, out uint _Secret)
        {
            _ObfuscatedValue = this.obfuscatedValue.intValue;
            _Secret = this.secret;
        }

        /// <summary>
        /// Used to deserialize the protected from the player prefs.
        /// </summary>
        /// <param name="_ObfuscatedValue">The obfuscated value of the protected.</param>
        /// <param name="_Secret">The secret key used to obfuscate the true value.</param>
        internal void Deserialize(uint _ObfuscatedValue, uint _Secret)
        {
            this.obfuscatedValue.intValue = _ObfuscatedValue;
            this.secret = _Secret;
            this.fakeValue = this.UnObfuscate();
        }

        #endregion

        #region Implicit operators

        /// <summary>
        /// Implicitly converts a float to a ProtectedFloat.
        /// </summary>
        /// <param name="_Value">The float value to be converted.</param>
        /// <returns>A new instance of ProtectedFloat with the converted value.</returns>
        public static implicit operator ProtectedFloat(float _Value)
        {
            return new ProtectedFloat(_Value);
        }

        /// <summary>
        /// Implicitly converts a ProtectedFloat to a float.
        /// </summary>
        /// <param name="_Value">The ProtectedFloat to be converted.</param>
        /// <returns>The unencrypted float value.</returns>
        public static implicit operator float(ProtectedFloat _Value)
        {
            return _Value.Value;
        }

        /// <summary>
        /// Implicitly converts a ProtectedFloat to a ProtectedInt16.
        /// </summary>
        /// <param name="_Value">The ProtectedFloat to be converted.</param>
        /// <returns>A new instance of ProtectedInt16 with the converted value.</returns>
        public static implicit operator ProtectedInt16(ProtectedFloat _Value)
        {
            return new ProtectedInt16((Int16)_Value.Value);
        }

        /// <summary>
        /// Implicitly converts a ProtectedInt16 to a ProtectedFloat.
        /// </summary>
        /// <param name="_Value">The ProtectedInt16 to be converted.</param>
        /// <returns>A new instance of ProtectedFloat with the converted value.</returns>
        public static implicit operator ProtectedFloat(ProtectedInt16 _Value)
        {
            return new ProtectedFloat((float)_Value.Value);
        }

        /// <summary>
        /// Implicitly converts a ProtectedFloat to a ProtectedInt32.
        /// </summary>
        /// <param name="_Value">The ProtectedFloat to be converted.</param>
        /// <returns>A new instance of ProtectedInt32 with the converted value.</returns>
        public static implicit operator ProtectedInt32(ProtectedFloat _Value)
        {
            return new ProtectedInt32((Int32)_Value.Value);
        }

        /// <summary>
        /// Implicitly converts a ProtectedInt32 to a ProtectedFloat.
        /// </summary>
        /// <param name="_Value">The ProtectedInt32 to be converted.</param>
        /// <returns>A new instance of ProtectedFloat with the converted value.</returns>
        public static implicit operator ProtectedFloat(ProtectedInt32 _Value)
        {
            return new ProtectedFloat((float)_Value.Value);
        }

        /// <summary>
        /// Implicitly converts a ProtectedFloat to a ProtectedInt64.
        /// </summary>
        /// <param name="_Value">The ProtectedFloat to be converted.</param>
        /// <returns>A new instance of ProtectedInt64 with the converted value.</returns>
        public static implicit operator ProtectedInt64(ProtectedFloat _Value)
        {
            return new ProtectedInt64((Int64)_Value.Value);
        }

        /// <summary>
        /// Implicitly converts a ProtectedInt64 to a ProtectedFloat.
        /// </summary>
        /// <param name="_Value">The ProtectedInt64 to be converted.</param>
        /// <returns>A new instance of ProtectedFloat with the converted value.</returns>
        public static implicit operator ProtectedFloat(ProtectedInt64 _Value)
        {
            return new ProtectedFloat((float)_Value.Value);
        }

        #endregion

        #region Calculation operators

        /// <summary>
        /// Adds two ProtectedFloat instances.
        /// </summary>
        /// <param name="v1">The first ProtectedFloat instance.</param>
        /// <param name="v2">The second ProtectedFloat instance.</param>
        /// <returns>A new instance of ProtectedFloat representing the sum of the two instances.</returns>
        public static ProtectedFloat operator +(ProtectedFloat v1, ProtectedFloat v2)
        {
            return new ProtectedFloat(v1.Value + v2.Value);
        }

        /// <summary>
        /// Subtracts one ProtectedFloat instance from another.
        /// </summary>
        /// <param name="v1">The first ProtectedFloat instance.</param>
        /// <param name="v2">The second ProtectedFloat instance.</param>
        /// <returns>A new instance of ProtectedFloat representing the result of the subtraction.</returns>
        public static ProtectedFloat operator -(ProtectedFloat v1, ProtectedFloat v2)
        {
            return new ProtectedFloat(v1.Value - v2.Value);
        }

        /// <summary>
        /// Multiplies two ProtectedFloat instances.
        /// </summary>
        /// <param name="v1">The first ProtectedFloat instance.</param>
        /// <param name="v2">The second ProtectedFloat instance.</param>
        /// <returns>A new instance of ProtectedFloat representing the product of the two instances.</returns>
        public static ProtectedFloat operator *(ProtectedFloat v1, ProtectedFloat v2)
        {
            return new ProtectedFloat(v1.Value * v2.Value);
        }

        /// <summary>
        /// Divides one ProtectedFloat instance by another.
        /// </summary>
        /// <param name="v1">The numerator ProtectedFloat instance.</param>
        /// <param name="v2">The denominator ProtectedFloat instance.</param>
        /// <returns>A new instance of ProtectedFloat representing the result of the division.</returns>
        public static ProtectedFloat operator /(ProtectedFloat v1, ProtectedFloat v2)
        {
            return new ProtectedFloat(v1.Value / v2.Value);
        }

        #endregion

        #region Equality operators

        /// <summary>
        /// Determines whether two ProtectedFloat instances are equal.
        /// </summary>
        /// <param name="v1">The first ProtectedFloat instance.</param>
        /// <param name="v2">The second ProtectedFloat instance.</param>
        /// <returns>True if the values of the two instances are equal; otherwise, false.</returns>
        public static bool operator ==(ProtectedFloat v1, ProtectedFloat v2)
        {
            return v1.Value == v2.Value;
        }

        /// <summary>
        /// Determines whether two ProtectedFloat instances are not equal.
        /// </summary>
        /// <param name="v1">The first ProtectedFloat instance.</param>
        /// <param name="v2">The second ProtectedFloat instance.</param>
        /// <returns>True if the values of the two instances are not equal; otherwise, false.</returns>
        public static bool operator !=(ProtectedFloat v1, ProtectedFloat v2)
        {
            return v1.Value != v2.Value;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current ProtectedFloat instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>True if the specified object is equal to the current instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is ProtectedFloat)
            {
                return this.Value == ((ProtectedFloat)obj).Value;
            }
            return this.Value.Equals(obj);
        }

        /// <summary>
        /// Compares two ProtectedFloat instances for less than.
        /// </summary>
        /// <param name="v1">The first ProtectedFloat instance.</param>
        /// <param name="v2">The second ProtectedFloat instance.</param>
        /// <returns>True if the value of the first instance is less than the value of the second instance; otherwise, false.</returns>
        public static bool operator <(ProtectedFloat v1, ProtectedFloat v2)
        {
            return v1.Value < v2.Value;
        }

        /// <summary>
        /// Compares two ProtectedFloat instances for less than or equal.
        /// </summary>
        /// <param name="v1">The first ProtectedFloat instance.</param>
        /// <param name="v2">The second ProtectedFloat instance.</param>
        /// <returns>True if the value of the first instance is less than or equal to the value of the second instance; otherwise, false.</returns>
        public static bool operator <=(ProtectedFloat v1, ProtectedFloat v2)
        {
            return v1.Value <= v2.Value;
        }

        /// <summary>
        /// Compares two ProtectedFloat instances for greater than.
        /// </summary>
        /// <param name="v1">The first ProtectedFloat instance.</param>
        /// <param name="v2">The second ProtectedFloat instance.</param>
        /// <returns>True if the value of the first instance is greater than the value of the second instance; otherwise, false.</returns>
        public static bool operator >(ProtectedFloat v1, ProtectedFloat v2)
        {
            return v1.Value > v2.Value;
        }

        /// <summary>
        /// Compares two ProtectedFloat instances for greater than or equal.
        /// </summary>
        /// <param name="v1">The first ProtectedFloat instance.</param>
        /// <param name="v2">The second ProtectedFloat instance.</param>
        /// <returns>True if the value of the first instance is greater than or equal to the value of the second instance; otherwise, false.</returns>
        public static bool operator >=(ProtectedFloat v1, ProtectedFloat v2)
        {
            return v1.Value >= v2.Value;
        }

        #endregion
    }
}