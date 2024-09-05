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
    /// Represents a protected string that provides an additional layer of security for sensitive values.
    /// In most cases, it is recommended to replace the default string type with the protected one, considering the overhead introduced by complex encryption and encoding.
    /// </summary>
    [Serializable]
    public struct ProtectedString : IProtected, IDisposable, ISerializationCallbackReceiver
    {
        /// <summary>
        /// A struct does not have a default constructor that is called when the structure is created. Therefore, the protected primitive must return 
        /// a default value if it does not have an assigned value.
        /// </summary>
        private bool isInitialized;

        /// <summary>
        /// Encrypts the specified string with the given secret and encodes it to UTF-8.
        /// </summary>
        /// <param name="_String">The string to be protected.</param>
        /// <param name="_Secret">The secret key for protection.</param>
        /// <returns>The encrypted and UTF-8 encoded representation of the input string.</returns>
        private static string EncryptToUTF8(string _String, int _Secret)
        {
            if (_String == null)
            {
                return null;
            }

            if (_String.Length == 0)
            {
                return "";
            }

            uint key1 = 0x45435345 + (uint)_Secret;
            uint key2 = 0x95656543;

            byte[] buff1 = System.Text.UTF8Encoding.UTF8.GetBytes(_String);
            byte[] buff = new byte[buff1.Length + 1];

            buff[0] = (byte)(GlobalSettings.RandomProvider.RandomInt32(1, Int32.MaxValue) % 256);

            byte d = buff[0];

            for (int i = 1; i < buff.Length; i++)
            {
                buff[i] = buff1[i - 1];
                key1 = (key1 * 4343255 + d + 5235457) % 0xFFFFFFFE;
                key2 = (key2 * 5354354 + d + 22646641) % 0xFFFFFFFE;

                d = buff[i];

                buff[i] = (byte)((uint)buff[i] ^ key1);
                buff[i] = (byte)((byte)buff[i] + (byte)key2);
            }

            return System.Convert.ToBase64String(buff);
        }

        /// <summary>
        /// Decrypts the protected string with the given secret and decodes it from UTF-8.
        /// </summary>
        /// <param name="_String">The string to be unprotected.</param>
        /// <param name="_Secret">The secret key for protection.</param>
        /// <returns>The decrypted and UTF-8 decoded representation of the input string.</returns>
        private static string DecryptFromUTF8(string _String, int _Secret)
        {
            if (_String == null)
            {
                return null;
            }

            if (_String.Length == 0)
            {
                return "";
            }

            uint key1 = 0x45435345 + (uint)_Secret;
            uint key2 = 0x95656543;

            byte[] buff1 = System.Convert.FromBase64String(_String);
            byte[] buff = new byte[buff1.Length - 1];

            byte d = buff1[0];

            for (int i = 0; i < buff.Length; i++)
            {
                buff[i] = buff1[i + 1];
                key1 = (key1 * 4343255 + d + 5235457) % 0xFFFFFFFE;
                key2 = (key2 * 5354354 + d + 22646641) % 0xFFFFFFFE;

                buff[i] = (byte)((byte)buff[i] - (byte)key2);
                buff[i] = (byte)((uint)buff[i] ^ key1);
                d = buff[i];
            }

            return System.Text.UTF8Encoding.UTF8.GetString(buff, 0, buff.Length);
        }

        /// <summary>
        /// Gets a value indicating whether the protected value has integrity, i.e., whether it has maintained its original state.
        /// </summary>
        public bool HasIntegrity { get; private set; }

        /// <summary>
        /// The obfuscated value of the protected.
        /// </summary>
        private string obfuscatedValue;

        /// <summary>
        /// A secret key used to obfuscate the true value.
        /// </summary>
        private Int32 secret;

        /// <summary>
        /// A honeypot pretending to be the original value. If a user attempts to change this value via a cheat/hack engine, you will get notified.
        /// The protected value will keep its true value.
        /// </summary>
        [SerializeField]
        private string fakeValue;

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
        /// Creates a new protected string with the specified value.
        /// </summary>
        /// <param name="_Value">The initial value of the protected string.</param>
        public ProtectedString(string _Value = null)
        {
            // Initialization
            this.isInitialized = true;
            this.secret = GlobalSettings.RandomProvider.RandomInt32(1, +5432);
            this.obfuscatedValue = EncryptToUTF8(_Value, this.secret);

            //
            this.HasIntegrity = true;

            //
            this.fakeValue = _Value;
        }

        /// <summary>
        /// Gets and sets the true unencrypted field value.
        /// </summary>
        public string Value
        {
            get
            {
                if (!this.isInitialized)
                {
                    return null;
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
        private void Obfuscate(string _Value)
        {
            // Obfuscate the value.
            this.obfuscatedValue = EncryptToUTF8(_Value, this.secret);

            // Assign the fake value.
            this.fakeValue = _Value;
        }

        /// <summary>
        /// Unobfuscates the secured value and returns the true unencrypted value.
        /// </summary>
        /// <returns>The true unencrypted value.</returns>
        private string UnObfuscate()
        {
            // Get the unobfuscated value.
            return DecryptFromUTF8(this.obfuscatedValue, this.secret);
        }

        /// <summary>
        /// Obfuscates the current value, generating a new random secret key.
        /// </summary>
        public void Obfuscate()
        {
            // Unobfuscate the secured value.
            string var_UnobfuscatedValue = this.UnObfuscate();

            // Create a new random secret.
            this.secret = GlobalSettings.RandomProvider.RandomInt32(1, +5432);

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
            string var_UnobfuscatedValue = this.UnObfuscate();

            // Check if an attacker changed the honeypot fake value.
            if (this.fakeValue != var_UnobfuscatedValue)
            {
                this.HasIntegrity = false;
            }

            // Return the integrity status.
            return this.HasIntegrity;
        }

        /// <summary>
        /// Releases the resources used by the protected string.
        /// </summary>
        public void Dispose()
        {
            this.secret = 0;
            this.obfuscatedValue = null;
        }

        /// <summary>
        /// Returns a string representation of the protected string.
        /// </summary>
        /// <returns>A string representation of the protected string.</returns>
        public override string ToString()
        {
            return this.Value;
        }

        /// <summary>
        /// Gets the hash code for the protected value.
        /// </summary>
        /// <returns>The hash code for the protected value.</returns>
        public override int GetHashCode()
        {
            if (this.Value == null)
            {
                return 0;
            }

            return this.Value.GetHashCode();
        }

        #region Serialization

        /// <summary>
        /// Used to serialize the protected to the player prefs.
        /// </summary>
        /// <param name="_ObfuscatedValue">The obfuscated value of the protected.</param>
        /// <param name="_Secret">The secret key used to obfuscate the true value.</param>
        internal void Serialize(out String _ObfuscatedValue, out int _Secret)
        {
            _ObfuscatedValue = this.obfuscatedValue;
            _Secret = this.secret;
        }

        /// <summary>
        /// Used to deserialize the protected from the player prefs.
        /// </summary>
        /// <param name="_ObfuscatedValue">The obfuscated value of the protected.</param>
        /// <param name="_Secret">The secret key used to obfuscate the true value.</param>
        internal void Deserialize(String _ObfuscatedValue, int _Secret)
        {
            this.obfuscatedValue = _ObfuscatedValue;
            this.secret = _Secret;
            this.fakeValue = this.UnObfuscate();
        }

        #endregion

        #region Implicit operator

        /// <summary>
        /// Implicitly converts a regular string to a protected string.
        /// </summary>
        /// <param name="_Value">The regular string to be converted.</param>
        /// <returns>A new instance of the protected string.</returns>
        public static implicit operator ProtectedString(string _Value)
        {
            return new ProtectedString(_Value);
        }

        /// <summary>
        /// Implicitly converts a protected string to a regular string.
        /// </summary>
        /// <param name="_Value">The protected string to be converted.</param>
        /// <returns>The true unencrypted value of the protected string.</returns>
        public static implicit operator string(ProtectedString _Value)
        {
            return _Value.Value;
        }

        #endregion

        #region Calculation operator

        /// <summary>
        /// Concatenates two protected strings.
        /// </summary>
        /// <param name="v1">The first protected string.</param>
        /// <param name="v2">The second protected string.</param>
        /// <returns>A new protected string representing the concatenation of the input strings.</returns>
        public static ProtectedString operator +(ProtectedString v1, ProtectedString v2)
        {
            return new ProtectedString(v1.Value + v2.Value);
        }

        #endregion

        #region Equality operator

        /// <summary>
        /// Checks if two protected strings are equal.
        /// </summary>
        /// <param name="v1">The first protected string.</param>
        /// <param name="v2">The second protected string.</param>
        /// <returns>True if the values of the protected strings are equal; otherwise, false.</returns>
        public static bool operator ==(ProtectedString v1, ProtectedString v2)
        {
            return v1.Value == v2.Value;
        }

        /// <summary>
        /// Checks if two protected strings are not equal.
        /// </summary>
        /// <param name="v1">The first protected string.</param>
        /// <param name="v2">The second protected string.</param>
        /// <returns>True if the values of the protected strings are not equal; otherwise, false.</returns>
        public static bool operator !=(ProtectedString v1, ProtectedString v2)
        {
            return v1.Value != v2.Value;
        }

        /// <summary>
        /// Checks if the protected string is equal to the specified object.
        /// </summary>
        /// <param name="obj">The object to compare with the protected string.</param>
        /// <returns>True if the values are equal; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is ProtectedString)
            {
                return this.Value == ((ProtectedString)obj).Value;
            }

            if (this.Value == null && obj == null)
            {
                return true;
            }

            return this.Value.Equals(obj);
        }

        #endregion
    }
}