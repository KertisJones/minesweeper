// System
using System;

// Unity
using UnityEngine;
using UnityEngine.Internal;

namespace GUPS.AntiCheat.Protected.Prefs
{
    /// <summary>
    /// Protected version of the unity PlayerPrefs. Contains also additional save and load able types.
    /// </summary>
    public sealed class ProtectedPlayerPrefs
    {
        /// <summary>
        /// Returns true if key exists in the preferences.
        /// </summary>
        /// <param name="_Key"></param>
        /// <returns></returns>
        public static bool HasKey(String _Key)
        {
            return PlayerPrefs.HasKey(_Key + "_Protected");
        }

        /// <summary>
        ///   <para>Sets the _Value of the preference identified by _Key.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_Value"></param>
        public static void SetInt(string _Key, int _Value)
        {
            // Init Protected with value _Value.
            ProtectedInt32 var_Protected = new ProtectedInt32(_Value);
            // Serialize the protected.
            var_Protected.Serialize(out Int32 var_ObfuscatedValue, out Int32 var_Secret);
            // Set intern value as value for _Key
            PlayerPrefs.SetInt(_Key + "_Protected", var_ObfuscatedValue);
            // Save under the _Key+_ProtectedHash, the secret.
            PlayerPrefs.SetInt(_Key + "_ProtectedHash", var_Secret);

            // Auto save if activated.
            if(AutoSave)
            {
                Save();
            }
        }

        /// <summary>
        ///   <para>Returns the value corresponding to _Key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_DefaultValue"></param>
        public static int GetInt(string _Key, [DefaultValue("0")] int _DefaultValue)
        {
            if (PlayerPrefs.HasKey(_Key + "_ProtectedHash"))
            {
                // Create empty protected by not using the empty constructor. The empty constructor would initialize the struct with the default / empty values.
                ProtectedInt32 var_Protected = new ProtectedInt32(0);

                // Load obuscated value.
                int var_ObfuscatedValue = PlayerPrefs.GetInt(_Key + "_Protected");

                // Load secret.
                int var_Secret = PlayerPrefs.GetInt(_Key + "_ProtectedHash");

                // Deserialize the protected.
                var_Protected.Deserialize(var_ObfuscatedValue, var_Secret);

                return var_Protected.Value;
            }

            return PlayerPrefs.GetInt(_Key, _DefaultValue);
        }

        /// <summary>
        ///   <para>Returns the value corresponding to _Key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        public static int GetInt(string _Key)
        {
            return ProtectedPlayerPrefs.GetInt(_Key, 0);
        }

        /// <summary>
        ///   <para>Sets the _Value of the preference identified by _Key.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_Value"></param>
        public static void SetFloat(string _Key, float _Value)
        {
            unchecked
            {
                // Init Protected with value _Value.
                ProtectedFloat var_Protected = new ProtectedFloat(_Value);
                // Serialize the protected.
                var_Protected.Serialize(out UInt32 var_ObfuscatedValue, out UInt32 var_Secret);
                //Set intern value as value for _Key
                PlayerPrefs.SetInt(_Key + "_Protected", (int)var_ObfuscatedValue);
                //Save under the _Key+_ProtectedHash, the secret.
                PlayerPrefs.SetInt(_Key + "_ProtectedHash", (int)var_Secret);
            }

            // Auto save if activated.
            if (AutoSave)
            {
                Save();
            }
        }

        /// <summary>
        ///   <para>Returns the value corresponding to _Key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_DefaultValue"></param>
        public static float GetFloat(string _Key, [DefaultValue("0.0F")] float _DefaultValue)
        {
            if (PlayerPrefs.HasKey(_Key + "_ProtectedHash"))
            {
                unchecked
                {
                    // Create empty protected by not using the empty constructor. The empty constructor would initialize the struct with the default / empty values.
                    ProtectedFloat var_Protected = new ProtectedFloat(0);

                    // Load obfuscated value.
                    int var_ObfuscatedValue = PlayerPrefs.GetInt(_Key + "_Protected");

                    // Load secret
                    int var_Secret = PlayerPrefs.GetInt(_Key + "_ProtectedHash");

                    // Deserialize the protected.
                    var_Protected.Deserialize((UInt32)var_ObfuscatedValue, (UInt32)var_Secret);

                    return var_Protected.Value;
                }
            }

            return PlayerPrefs.GetFloat(_Key, _DefaultValue);
        }

        /// <summary>
        ///   <para>Returns the value corresponding to _Key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        public static float GetFloat(string _Key)
        {
            return ProtectedPlayerPrefs.GetFloat(_Key, 0);
        }

        /// <summary>
        ///   <para>Sets the value of the preference identified by key.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_Value"></param>
        public static void SetString(string _Key, string _Value)
        {
            //Init Protected with value _Value
            ProtectedString var_Protected = new ProtectedString(_Value);
            // Serialize the protected.
            var_Protected.Serialize(out String var_ObfuscatedValue, out Int32 var_Secret);
            //Set intern value as value for _Key
            PlayerPrefs.SetString(_Key + "_Protected", var_ObfuscatedValue);
            //Save under the _Key+_ProtectedHash, the secret.
            PlayerPrefs.SetInt(_Key + "_ProtectedHash", var_Secret);

            // Auto save if activated.
            if (AutoSave)
            {
                Save();
            }
        }

        /// <summary>
        ///   <para>Returns the value corresponding to key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_DefaultValue"></param>
        public static string GetString(string _Key, [DefaultValue("\"\"")] string _DefaultValue)
        {
            if (PlayerPrefs.HasKey(_Key + "_ProtectedHash"))
            {
                // Create empty protected by not using the empty constructor. The empty constructor would initialize the struct with the default / empty values.
                ProtectedString var_Protected = new ProtectedString(String.Empty);

                // Load intern Value
                String var_ObfuscatedValue = PlayerPrefs.GetString(_Key + "_Protected");

                // Load Key
                int var_Secret = PlayerPrefs.GetInt(_Key + "_ProtectedHash");

                // Deserialize the protected.
                var_Protected.Deserialize(var_ObfuscatedValue, var_Secret);

                return var_Protected.Value;
            }

            return PlayerPrefs.GetString(_Key, _DefaultValue);
        }

        /// <summary>
        ///   <para>Returns the value corresponding to _Key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        public static string GetString(string _Key)
        {
            return ProtectedPlayerPrefs.GetString(_Key, "");
        }

        ////////////////////// CUSTOM ////////////////////////////

        /// <summary>
        ///   <para>Sets the _Value of the preference identified by _Key.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_Value"></param>
        public static void SetBool(string _Key, bool _Value)
        {
            // Init Protected with value _Value.
            ProtectedBool var_Protected = new ProtectedBool(_Value);
            // Serialize the protected.
            var_Protected.Serialize(out byte var_ObfuscatedValue, out int var_Secret);
            // Set intern value as value for _Key.
            PlayerPrefs.SetString(_Key + "_Protected", var_ObfuscatedValue.ToString());
            // Save under the _Key+_ProtectedHash, the secret.
            PlayerPrefs.SetInt(_Key + "_ProtectedHash", var_Secret);

            // Auto save if activated.
            if (AutoSave)
            {
                Save();
            }
        }

        /// <summary>
        ///   <para>Returns the value corresponding to _Key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_DefaultValue"></param>
        public static bool GetBool(string _Key, [DefaultValue("false")] bool _DefaultValue)
        {
            if (PlayerPrefs.HasKey(_Key + "_ProtectedHash"))
            {
                // Create empty protected by not using the empty constructor. The empty constructor would initialize the struct with the default / empty values.
                ProtectedBool var_Protected = new ProtectedBool(false);
                // Load obfuscated value.
                String var_ObfuscatedValueString = PlayerPrefs.GetString(_Key + "_Protected");
                // Load Secret
                int var_Secret = PlayerPrefs.GetInt(_Key + "_ProtectedHash");
                // Deserialize the protected.
                var_Protected.Deserialize(byte.Parse(var_ObfuscatedValueString), var_Secret);

                // Return the unobfuscated value.
                return var_Protected.Value;
            }

            return _DefaultValue;
        }

        /// <summary>
        ///   <para>Returns the value corresponding to _Key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        public static bool GetBool(string _Key)
        {
            return ProtectedPlayerPrefs.GetBool(_Key, false);
        }

        /// <summary>
        ///   <para>Sets the value of the preference identified by key.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_Value"></param>
        public static void SetVector2(string _Key, Vector2 _Value)
        {
            //Init Protected with value _Value
            ProtectedVector2 var_Protected = new ProtectedVector2(_Value);
            // Serialize the protected.
            var_Protected.Serialize(out UInt32 var_ObfuscatedValueX, out UInt32 var_ObfuscatedValueY, out UInt32 var_Secret);
            //Set intern value as value for _Key
            PlayerPrefs.SetString(_Key + "_Protected", var_ObfuscatedValueX + "|" + var_ObfuscatedValueY);
            //Save under the _Key+_ProtectedHash, the secret.
            PlayerPrefs.SetInt(_Key + "_ProtectedHash", (int)var_Secret);

            // Auto save if activated.
            if (AutoSave)
            {
                Save();
            }
        }

        /// <summary>
        ///   <para>Returns the value corresponding to key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_DefaultValue"></param>
        public static Vector2 GetVector2(string _Key, Vector2 _DefaultValue)
        {
            if (PlayerPrefs.HasKey(_Key + "_ProtectedHash"))
            {
                // Create empty protected by not using the empty constructor. The empty constructor would initialize the struct with the default / empty values.
                ProtectedVector2 var_Protected = new ProtectedVector2(Vector2.zero);

                // Load intern Value
                string var_ObfuscatedValueString = PlayerPrefs.GetString(_Key + "_Protected");
                string[] var_ObfuscatedValueStringSplit = var_ObfuscatedValueString.Split('|');

                // Load Key
                int var_Secret = PlayerPrefs.GetInt(_Key + "_ProtectedHash");

                // Deserialize the protected.
                var_Protected.Deserialize(UInt32.Parse(var_ObfuscatedValueStringSplit[0]), UInt32.Parse(var_ObfuscatedValueStringSplit[1]), (UInt32)var_Secret);

                return var_Protected.Value;
            }

            return _DefaultValue;
        }

        /// <summary>
        ///   <para>Returns the value corresponding to _Key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        public static Vector2 GetVector2(string _Key)
        {
            return ProtectedPlayerPrefs.GetVector2(_Key, Vector2.zero);
        }

        /// <summary>
        ///   <para>Sets the value of the preference identified by key.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_Value"></param>
        public static void SetVector3(string _Key, Vector3 _Value)
        {
            //Init Protected with value _Value
            ProtectedVector3 var_Protected = new ProtectedVector3(_Value);
            // Serialize the protected.
            var_Protected.Serialize(out UInt32 var_ObfuscatedValueX, out UInt32 var_ObfuscatedValueY, out UInt32 var_ObfuscatedValueZ, out UInt32 var_Secret);
            //Set intern value as value for _Key
            PlayerPrefs.SetString(_Key + "_Protected", var_ObfuscatedValueX + "|" + var_ObfuscatedValueY + "|" + var_ObfuscatedValueZ);
            //Save under the _Key+_ProtectedHash, the secret.
            PlayerPrefs.SetInt(_Key + "_ProtectedHash", (int)var_Secret);

            // Auto save if activated.
            if (AutoSave)
            {
                Save();
            }
        }

        /// <summary>
        ///   <para>Returns the value corresponding to key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_DefaultValue"></param>
        public static Vector3 GetVector3(string _Key, Vector3 _DefaultValue)
        {
            if (PlayerPrefs.HasKey(_Key + "_ProtectedHash"))
            {
                // Create empty protected by not using the empty constructor. The empty constructor would initialize the struct with the default / empty values.
                ProtectedVector3 var_Protected = new ProtectedVector3(Vector3.zero);

                // Load obfuscated value.
                string var_ObfuscatedValueString = PlayerPrefs.GetString(_Key + "_Protected");
                string[] var_ObfuscatedValueStringSplit = var_ObfuscatedValueString.Split('|');

                // Load secret.
                int var_Secret = PlayerPrefs.GetInt(_Key + "_ProtectedHash");

                // Deserialize the protected.
                var_Protected.Deserialize(UInt32.Parse(var_ObfuscatedValueStringSplit[0]), UInt32.Parse(var_ObfuscatedValueStringSplit[1]), UInt32.Parse(var_ObfuscatedValueStringSplit[2]), (UInt32)var_Secret);

                return var_Protected.Value;
            }

            return _DefaultValue;
        }

        /// <summary>
        ///   <para>Returns the value corresponding to _Key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        public static Vector3 GetVector3(string _Key)
        {
            return ProtectedPlayerPrefs.GetVector3(_Key, Vector3.zero);
        }

        /// <summary>
        ///   <para>Sets the value of the preference identified by key.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_Value"></param>
        public static void SetVector4(string _Key, Vector4 _Value)
        {
            // Init Protected with value _Value
            ProtectedVector4 var_Protected = new ProtectedVector4(_Value);
            // Serialize the protected.
            var_Protected.Serialize(out UInt32 var_ObfuscatedValueX, out UInt32 var_ObfuscatedValueY, out UInt32 var_ObfuscatedValueZ, out UInt32 var_ObfuscatedValueW, out UInt32 var_Secret);
            // Set intern value as value for _Key
            PlayerPrefs.SetString(_Key + "_Protected", var_ObfuscatedValueX + "|" + var_ObfuscatedValueY + "|" + var_ObfuscatedValueZ + "|" + var_ObfuscatedValueW);
            // Save under the _Key+_ProtectedHash, the secret.
            PlayerPrefs.SetInt(_Key + "_ProtectedHash", (int)var_Secret);

            // Auto save if activated.
            if (AutoSave)
            {
                Save();
            }
        }

        /// <summary>
        ///   <para>Returns the value corresponding to key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_DefaultValue"></param>
        public static Vector4 GetVector4(string _Key, Vector4 _DefaultValue)
        {
            if (PlayerPrefs.HasKey(_Key + "_ProtectedHash"))
            {
                // Create empty protected by not using the empty constructor. The empty constructor would initialize the struct with the default / empty values.
                ProtectedVector4 var_Protected = new ProtectedVector4(Vector4.zero);

                // Load obfuscated value.
                string var_ObfuscatedValueString = PlayerPrefs.GetString(_Key + "_Protected");
                string[] var_ObfuscatedValueStringSplit = var_ObfuscatedValueString.Split('|');

                // Load secret.
                int var_Secret = PlayerPrefs.GetInt(_Key + "_ProtectedHash");

                // Deserialize the protected.
                var_Protected.Deserialize(UInt32.Parse(var_ObfuscatedValueStringSplit[0]), UInt32.Parse(var_ObfuscatedValueStringSplit[1]), UInt32.Parse(var_ObfuscatedValueStringSplit[2]), UInt32.Parse(var_ObfuscatedValueStringSplit[3]), (UInt32)var_Secret);

                return var_Protected.Value;
            }

            return _DefaultValue;
        }

        /// <summary>
        ///   <para>Returns the value corresponding to _Key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        public static Vector4 GetVector4(string _Key)
        {
            return ProtectedPlayerPrefs.GetVector4(_Key, Vector4.zero);
        }
        
        /// <summary>
        ///   <para>Sets the value of the preference identified by key.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_Value"></param>
        public static void SetQuaternion(string _Key, Quaternion _Value)
        {
            Vector4 var_Vector = new Vector4(_Value.x, _Value.y, _Value.z, _Value.w);
            SetVector4(_Key, var_Vector);
        }

        /// <summary>
        ///   <para>Returns the value corresponding to _Key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        public static Quaternion GetQuaternion(string _Key, Quaternion _Default)
        {
            Vector4 var_Vector = ProtectedPlayerPrefs.GetVector4(_Key, new Vector4(_Default.x, _Default.y, _Default.z, _Default.w));
            return new Quaternion(var_Vector.x, var_Vector.y, var_Vector.z, var_Vector.w);
        }

        /// <summary>
        ///   <para>Returns the value corresponding to _Key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        public static Quaternion GetQuaternion(string _Key)
        {
            return GetQuaternion(_Key, Quaternion.identity);
        }

        /// <summary>
        /// Activate or deactivate force autosaving of modified preferences to the disk.
        /// </summary>
        public static bool AutoSave = false;

        /// <summary>
        /// Writes all modified preferences to disk.
        /// </summary>
        public static void Save()
        {
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Removes key and its corresponding value from the preferences.
        /// </summary>
        public static void DeleteKey(String _Key)
        {
            PlayerPrefs.DeleteKey(_Key + "_Protected");

            PlayerPrefs.DeleteKey(_Key + "_ProtectedHash");         
        }
    }
}
