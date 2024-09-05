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
    /// Represents a protected Quaternion, enhancing security for sensitive vector data. 
    /// In most scenarios, it is recommended to replace the default Quaternion type with this protected variant.
    /// </summary>
    [Serializable]
    public struct ProtectedQuaternion : IProtected, IDisposable, ISerializationCallbackReceiver
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
        private UIntFloat obfuscatedValueX;

        /// <summary>
        /// The encrypted true value for the y-component.
        /// </summary>
        private UIntFloat obfuscatedValueY;

        /// <summary>
        /// The encrypted true value for the z-component.
        /// </summary>
        private UIntFloat obfuscatedValueZ;

        /// <summary>
        /// The encrypted true value for the w-component.
        /// </summary>
        private UIntFloat obfuscatedValueW;

        /// <summary>
        /// Used for calculation of int/float values for the secured value.
        /// </summary>
        private UIntFloat manager;

        /// <summary>
        /// A secret key the true value gets encrypted with.
        /// </summary>
        private UInt32 secret;

        /// <summary>
        /// A honeypot pretending to be the original value. If a user attempts to change this value via a cheat/hack engine, notifications will be triggered.
        /// The protected value will maintain its true representation.
        /// </summary>
        [SerializeField]
        private Quaternion fakeValue;

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
        /// Creates a new protected Quaternion with the specified value.
        /// </summary>
        /// <param name="_Value">The initial value of the protected Quaternion.</param>
        public ProtectedQuaternion(Quaternion _Value)
        {
            // Initialization
            this.isInitialized = true;

            // Initialization - Secured values.
            this.obfuscatedValueX.intValue = 0;
            this.obfuscatedValueX.floatValue = 0;
            this.obfuscatedValueY.intValue = 0;
            this.obfuscatedValueY.floatValue = 0;
            this.obfuscatedValueZ.intValue = 0;
            this.obfuscatedValueZ.floatValue = 0;
            this.obfuscatedValueW.intValue = 0;
            this.obfuscatedValueW.floatValue = 0;

            // Initialization - Manager.
            this.manager.intValue = 0;
            this.manager.floatValue = 0;

            // Initialization - Random secret.
            this.secret = 0;

            // Initialization - Fake value.
            this.fakeValue = Quaternion.identity;

            // Initialization - Integrity.
            this.HasIntegrity = true;

            // Obfuscate the value.
            this.Obfuscate(_Value);
        }

        /// <summary>
        /// Gets and sets the true unencrypted field value.
        /// </summary>
        public Quaternion Value
        {
            get
            {
                if (!this.isInitialized)
                {
                    return new Quaternion();
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
        private void Obfuscate(Quaternion _Value)
        {
            // Obfuscate the value.
            this.manager.floatValue = _Value.x;
            this.manager.intValue = this.manager.intValue ^ this.secret;
            this.obfuscatedValueX.floatValue = this.manager.floatValue;

            this.manager.floatValue = _Value.y;
            this.manager.intValue = this.manager.intValue ^ this.secret;
            this.obfuscatedValueY.floatValue = this.manager.floatValue;

            this.manager.floatValue = _Value.z;
            this.manager.intValue = this.manager.intValue ^ this.secret;
            this.obfuscatedValueZ.floatValue = this.manager.floatValue;

            this.manager.floatValue = _Value.w;
            this.manager.intValue = this.manager.intValue ^ this.secret;
            this.obfuscatedValueW.floatValue = this.manager.floatValue;

            // Assign the fake value.
            this.fakeValue = _Value;
        }

        /// <summary>
        /// Unobfuscates the secured value and returns the true unencrypted value.
        /// </summary>
        /// <returns>The true unencrypted value.</returns>
        private Quaternion UnObfuscate()
        {
            // Get the unobfuscated value.
            Quaternion var_RealValue = new Quaternion();

            this.manager.intValue = this.obfuscatedValueX.intValue ^ this.secret;
            var_RealValue.x = this.manager.floatValue;

            this.manager.intValue = this.obfuscatedValueY.intValue ^ this.secret;
            var_RealValue.y = this.manager.floatValue;

            this.manager.intValue = this.obfuscatedValueZ.intValue ^ this.secret;
            var_RealValue.z = this.manager.floatValue;

            this.manager.intValue = this.obfuscatedValueW.intValue ^ this.secret;
            var_RealValue.w = this.manager.floatValue;

            // Return the unobfuscated value.
            return var_RealValue;
        }

        /// <summary>
        /// Obfuscates the current value, generating a new random secret key.
        /// </summary>
        public void Obfuscate()
        {
            // Unobfuscate the secured value.
            Quaternion var_UnobfuscatedValue = this.UnObfuscate();

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
            Quaternion var_UnobfuscatedValue = this.UnObfuscate();

            // Check if an attacker changed the honeypot fake value.
            if (this.fakeValue != var_UnobfuscatedValue)
            {
                this.HasIntegrity = false;
            }

            // Return the integrity status.
            return this.HasIntegrity;
        }

        /// <summary>
        /// Releases the resources used by the protected Quaternion.
        /// </summary>
        public void Dispose()
        {
            this.obfuscatedValueX.intValue = 0;
            this.obfuscatedValueY.intValue = 0;
            this.obfuscatedValueZ.intValue = 0;
            this.obfuscatedValueW.intValue = 0;
            this.manager.intValue = 0;
            this.secret = 0;
        }

        /// <summary>
        /// Returns a string representation of the protected Quaternion.
        /// </summary>
        /// <returns>A string representation of the protected Quaternion.</returns>
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
        /// Implicitly converts a regular Quaternion to a protected Quaternion.
        /// </summary>
        /// <param name="_Value">The regular Quaternion to be converted.</param>
        /// <returns>A new instance of the protected Quaternion.</returns>
        public static implicit operator ProtectedQuaternion(Quaternion _Value)
        {
            return new ProtectedQuaternion(_Value);
        }

        /// <summary>
        /// Implicitly converts a protected Quaternion to a regular Quaternion.
        /// </summary>
        /// <param name="_Value">The protected Quaternion to be converted.</param>
        /// <returns>The true unencrypted value of the protected Quaternion.</returns>
        public static implicit operator Quaternion(ProtectedQuaternion _Value)
        {
            return _Value.Value;
        }

        #endregion

        #region Equality operator

        /// <summary>
        /// Checks if two protected Quaternion instances are equal.
        /// </summary>
        /// <param name="v1">The first protected Quaternion.</param>
        /// <param name="v2">The second protected Quaternion.</param>
        /// <returns>True if the values of the protected Quaternion instances are equal; otherwise, false.</returns>
        public static bool operator ==(ProtectedQuaternion v1, ProtectedQuaternion v2)
        {
            return v1.Value == v2.Value;
        }

        /// <summary>
        /// Checks if two protected Quaternion instances are not equal.
        /// </summary>
        /// <param name="v1">The first protected Quaternion.</param>
        /// <param name="v2">The second protected Quaternion.</param>
        /// <returns>True if the values of the protected Quaternion instances are not equal; otherwise, false.</returns>
        public static bool operator !=(ProtectedQuaternion v1, ProtectedQuaternion v2)
        {
            return v1.Value != v2.Value;
        }

        /// <summary>
        /// Checks if the protected Quaternion is equal to the specified object.
        /// </summary>
        /// <param name="obj">The object to compare with the protected Quaternion.</param>
        /// <returns>True if the values are equal; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is ProtectedQuaternion)
            {
                return this.Value == ((ProtectedQuaternion)obj).Value;
            }
            return this.Value.Equals(obj);
        }

        #endregion
    }
}