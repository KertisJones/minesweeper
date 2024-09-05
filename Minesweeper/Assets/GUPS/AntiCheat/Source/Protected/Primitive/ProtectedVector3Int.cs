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
    /// Represents a protected Vector3Int, enhancing security for sensitive vector data. 
    /// In most scenarios, it is recommended to replace the default Vector3Int type with this protected variant.
    /// </summary>
    [Serializable]
    public struct ProtectedVector3Int : IProtected, IDisposable, ISerializationCallbackReceiver
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
        /// A secret key the true value gets encrypted with.
        /// </summary>
        private Int32 secret;

        /// <summary>
        /// A honeypot pretending to be the original value. If a user attempts to change this value via a cheat/hack engine, notifications will be triggered.
        /// The protected value will maintain its true representation.
        /// </summary>
        [SerializeField]
        private Vector3Int fakeValue;

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
        /// Creates a new protected Vector3Int with the specified value.
        /// </summary>
        /// <param name="_Value">The initial value of the protected Vector3Int.</param>
        public ProtectedVector3Int(Vector3Int _Value)
        {
            // Initialization
            this.isInitialized = true;

            // Initialization - Secured values.
            this.obfuscatedValueX = 0;
            this.obfuscatedValueY = 0;
            this.obfuscatedValueZ = 0;

            // Initialization - Random secret.
            this.secret = 0;

            // Initialization - Fake value.
            this.fakeValue = Vector3Int.zero;

            // Initialization - Integrity.
            this.HasIntegrity = true;

            // Obfuscate the value.
            this.Obfuscate(_Value);
        }

        /// <summary>
        /// Gets and sets the true unencrypted field value.
        /// </summary>
        public Vector3Int Value
        {
            get
            {
                if (!this.isInitialized)
                {
                    return new Vector3Int();
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
        private void Obfuscate(Vector3Int _Value)
        {
            // Obfuscate the value.
            this.obfuscatedValueX = (int)_Value.x ^ this.secret;
            this.obfuscatedValueY = (int)_Value.y ^ this.secret;
            this.obfuscatedValueZ = (int)_Value.z ^ this.secret;

            // Assign the fake value.
            this.fakeValue = _Value;
        }

        /// <summary>
        /// Unobfuscates the secured value and returns the true unencrypted value.
        /// </summary>
        /// <returns>The true unencrypted value.</returns>
        private Vector3Int UnObfuscate()
        {
            // Get the unobfuscated value.
            Vector3Int var_RealValue = new Vector3Int();

            var_RealValue.x = this.obfuscatedValueX ^ this.secret;
            var_RealValue.y = this.obfuscatedValueY ^ this.secret;
            var_RealValue.z = this.obfuscatedValueZ ^ this.secret;

            // Return the unobfuscated value.
            return var_RealValue;
        }

        /// <summary>
        /// Obfuscates the current value, generating a new random secret key.
        /// </summary>
        public void Obfuscate()
        {
            // Unobfuscate the secured value.
            Vector3Int var_UnobfuscatedValue = this.UnObfuscate();

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
            Vector3Int var_UnobfuscatedValue = this.UnObfuscate();

            // Check if an attacker changed the honeypot fake value.
            if (this.fakeValue != var_UnobfuscatedValue)
            {
                this.HasIntegrity = false;
            }

            // Return the integrity status.
            return this.HasIntegrity;
        }

        /// <summary>
        /// Releases the resources used by the protected Vector3Int.
        /// </summary>
        public void Dispose()
        {
            this.obfuscatedValueX = 0;
            this.obfuscatedValueY = 0;
            this.obfuscatedValueZ = 0;
            this.secret = 0;
        }

        /// <summary>
        /// Returns a string representation of the protected Vector3Int.
        /// </summary>
        /// <returns>A string representation of the protected Vector3Int.</returns>
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
        /// Implicitly converts a regular Vector3Int to a protected Vector3Int.
        /// </summary>
        /// <param name="_Value">The regular Vector3Int to be converted.</param>
        /// <returns>A new instance of the protected Vector3Int.</returns>
        public static implicit operator ProtectedVector3Int(Vector3Int _Value)
        {
            return new ProtectedVector3Int(_Value);
        }

        /// <summary>
        /// Implicitly converts a protected Vector3Int to a regular Vector3Int.
        /// </summary>
        /// <param name="_Value">The protected Vector3Int to be converted.</param>
        /// <returns>The true unencrypted value of the protected Vector3Int.</returns>
        public static implicit operator Vector3Int(ProtectedVector3Int _Value)
        {
            return _Value.Value;
        }

        /// <summary>
        /// Implicitly converts a protected Vector3Int to a protected Quaternion.
        /// </summary>
        /// <param name="_Value">The protected Vector3Int to be converted.</param>
        /// <returns>The encrypted value as protected Quaternion.</returns>
        public static implicit operator ProtectedQuaternion(ProtectedVector3Int _Value)
        {
            return new ProtectedQuaternion(new Quaternion(_Value.Value.x, _Value.Value.y, _Value.Value.z, 0));
        }

        /// <summary>
        /// Implicitly converts a protected Quaternion to a protected Vector3Int.
        /// </summary>
        /// <param name="_Value">The protected Quaternion to be converted.</param>
        /// <returns>The encrypted value as protected Vector3Int.</returns>
        public static implicit operator ProtectedVector3Int(ProtectedQuaternion _Value)
        {
            return new ProtectedVector3Int(new Vector3Int((int)_Value.Value.x, (int)_Value.Value.y, (int)_Value.Value.z));
        }

        #endregion

        #region Equality operator

        /// <summary>
        /// Checks if two protected Vector3Int instances are equal.
        /// </summary>
        /// <param name="v1">The first protected Vector3Int.</param>
        /// <param name="v2">The second protected Vector3Int.</param>
        /// <returns>True if the values of the protected Vector3Int instances are equal; otherwise, false.</returns>
        public static bool operator ==(ProtectedVector3Int v1, ProtectedVector3Int v2)
        {
            return v1.Value == v2.Value;
        }

        /// <summary>
        /// Checks if two protected Vector3Int instances are not equal.
        /// </summary>
        /// <param name="v1">The first protected Vector3Int.</param>
        /// <param name="v2">The second protected Vector3Int.</param>
        /// <returns>True if the values of the protected Vector3Int instances are not equal; otherwise, false.</returns>
        public static bool operator !=(ProtectedVector3Int v1, ProtectedVector3Int v2)
        {
            return v1.Value != v2.Value;
        }

        /// <summary>
        /// Checks if the protected Vector3Int is equal to the specified object.
        /// </summary>
        /// <param name="obj">The object to compare with the protected Vector3Int.</param>
        /// <returns>True if the values are equal; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is ProtectedVector3Int)
            {
                return this.Value == ((ProtectedVector3Int)obj).Value;
            }
            return this.Value.Equals(obj);
        }

        #endregion
    }
}