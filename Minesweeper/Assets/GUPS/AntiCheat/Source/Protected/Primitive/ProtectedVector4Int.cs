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
    /// Represents a protected Vector4Int, enhancing security for sensitive vector data. 
    /// Unity does not have a regular Vector4Int type, so this is a custom implementation.
    /// </summary>
    [Serializable]
    public struct ProtectedVector4Int : IProtected, IDisposable, ISerializationCallbackReceiver
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
        /// The encrypted true value for the x-component.
        /// </summary>
        private Int32 obfuscatedValueX;

        /// <summary>
        /// The encrypted true value for the y-component.
        /// </summary>
        private Int32 obfuscatedValueY;

        /// <summary>
        /// The encrypted true value for the z-component.
        /// </summary>
        private Int32 obfuscatedValueZ;

        /// <summary>
        /// The encrypted true value for the w-component.
        /// </summary>
        private Int32 obfuscatedValueW;

        /// <summary>
        /// A secret key the true value gets encrypted with.
        /// </summary>
        private Int32 secret;

        /// <summary>
        /// A honeypot pretending to be the original value. If a user attempts to change this value via a cheat/hack engine, notifications will be triggered.
        /// The protected value will maintain its true representation.
        /// </summary>
        [SerializeField]
        private Vector4 fakeValue;

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
        /// Creates a new protected Vector4 with the specified value.
        /// </summary>
        /// <param name="_Value">The initial value of the protected Vector4.</param>
        public ProtectedVector4Int(Vector4 _Value)
        {
            // Initialization
            this.isInitialized = true;

            // Initialization - Secured values.
            this.obfuscatedValueX = 0;
            this.obfuscatedValueY = 0;
            this.obfuscatedValueZ = 0;
            this.obfuscatedValueW = 0;

            // Initialization - Random secret.
            this.secret = 0;

            // Initialization - Fake value.
            this.fakeValue = Vector4.zero;

            // Initialization - Integrity.
            this.HasIntegrity = true;

            // Obfuscate the value.
            this.Obfuscate(_Value);
        }

        /// <summary>
        /// Gets and sets the true unencrypted field value.
        /// </summary>
        public Vector4 Value
        {
            get
            {
                if (!this.isInitialized)
                {
                    return new Vector4();
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
        private void Obfuscate(Vector4 _Value)
        {
            // Obfuscate the value.
            this.obfuscatedValueX = (int)_Value.x ^ this.secret;
            this.obfuscatedValueY = (int)_Value.y ^ this.secret;
            this.obfuscatedValueZ = (int)_Value.z ^ this.secret;
            this.obfuscatedValueW = (int)_Value.w ^ this.secret;

            // Assign the fake value.
            this.fakeValue = _Value;
        }

        /// <summary>
        /// Unobfuscates the secured value and returns the true unencrypted value.
        /// </summary>
        /// <returns>The true unencrypted value.</returns>
        private Vector4 UnObfuscate()
        {
            // Get the unobfuscated value.
            Vector4 var_RealValue = new Vector4();

            var_RealValue.x = this.obfuscatedValueX ^ this.secret;
            var_RealValue.y = this.obfuscatedValueY ^ this.secret;
            var_RealValue.z = this.obfuscatedValueZ ^ this.secret;
            var_RealValue.w = this.obfuscatedValueW ^ this.secret;

            // Return the unobfuscated value.
            return var_RealValue;
        }

        /// <summary>
        /// Obfuscates the current value, generating a new random secret key.
        /// </summary>
        public void Obfuscate()
        {
            // Unobfuscate the secured value.
            Vector4 var_UnobfuscatedValue = this.UnObfuscate();

            // Create a new random secret.
            this.secret = GlobalSettings.RandomProvider.RandomInt32(1, Int32.MaxValue);

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
            Vector4 var_UnobfuscatedValue = this.UnObfuscate();

            // Check if an attacker changed the honeypot fake value.
            if (this.fakeValue != var_UnobfuscatedValue)
            {
                this.HasIntegrity = false;
            }

            // Return the integrity status.
            return this.HasIntegrity;
        }

        /// <summary>
        /// Releases the resources used by the protected Vector4.
        /// </summary>
        public void Dispose()
        {
            this.obfuscatedValueX = 0;
            this.obfuscatedValueY = 0;
            this.obfuscatedValueZ = 0;
            this.obfuscatedValueW = 0;
            this.secret = 0;
        }

        /// <summary>
        /// Returns a string representation of the protected Vector4.
        /// </summary>
        /// <returns>A string representation of the protected Vector4.</returns>
        public override string ToString()
        {
            return this.Value.ToString();
        }

        /// <summary>
        /// Gets the hash code for the protected value.
        /// </summary>
        /// <returns>The hash code for the protected value.</returns>
        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        #region Implicit operator

        /// <summary>
        /// Implicitly converts a regular Vector4 to a protected Vector4Int.
        /// </summary>
        /// <param name="_Value">The regular Vector4 to be converted.</param>
        /// <returns>A new instance of the protected Vector4Int.</returns>
        public static implicit operator ProtectedVector4Int(Vector4 _Value)
        {
            return new ProtectedVector4Int(_Value);
        }

        /// <summary>
        /// Implicitly converts a protected Vector4Int to a regular Vector4.
        /// </summary>
        /// <param name="_Value">The protected Vector4Int to be converted.</param>
        /// <returns>The true unencrypted value of the protected Vector4.</returns>
        public static implicit operator Vector4(ProtectedVector4Int _Value)
        {
            return _Value.Value;
        }

        /// <summary>
        /// Implicitly converts a protected Vector4Int to a protected Quaternion.
        /// </summary>
        /// <param name="_Value">The protected Vector4Int to be converted.</param>
        /// <returns>The encrypted value as protected Quaternion.</returns>
        public static implicit operator ProtectedQuaternion(ProtectedVector4Int _Value)
        {
            return new ProtectedQuaternion(new Quaternion(_Value.Value.x, _Value.Value.y, _Value.Value.z, _Value.Value.w));
        }

        /// <summary>
        /// Implicitly converts a protected Quaternion to a protected Vector4Int.
        /// </summary>
        /// <param name="_Value">The protected Quaternion to be converted.</param>
        /// <returns>The encrypted value as protected Vector4Int.</returns>
        public static implicit operator ProtectedVector4Int(ProtectedQuaternion _Value)
        {
            return new ProtectedVector4Int(new Vector4(_Value.Value.x, _Value.Value.y, _Value.Value.z, _Value.Value.w));
        }

        #endregion

        #region Equality operator

        /// <summary>
        /// Checks if two protected Vector4Int instances are equal.
        /// </summary>
        /// <param name="v1">The first protected Vector4Int.</param>
        /// <param name="v2">The second protected Vector4Int.</param>
        /// <returns>True if the values of the protected Vector4Int instances are equal; otherwise, false.</returns>
        public static bool operator ==(ProtectedVector4Int v1, ProtectedVector4Int v2)
        {
            return v1.Value == v2.Value;
        }

        /// <summary>
        /// Checks if two protected Vector4Int instances are not equal.
        /// </summary>
        /// <param name="v1">The first protected Vector4Int.</param>
        /// <param name="v2">The second protected Vector4Int.</param>
        /// <returns>True if the values of the protected Vector4Int instances are not equal; otherwise, false.</returns>
        public static bool operator !=(ProtectedVector4Int v1, ProtectedVector4Int v2)
        {
            return v1.Value != v2.Value;
        }

        /// <summary>
        /// Checks if the protected Vector4Int is equal to the specified object.
        /// </summary>
        /// <param name="obj">The object to compare with the protected Vector4Int.</param>
        /// <returns>True if the values are equal; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is ProtectedVector4Int)
            {
                return this.Value == ((ProtectedVector4Int)obj).Value;
            }
            return this.Value.Equals(obj);
        }

        #endregion
    }
}