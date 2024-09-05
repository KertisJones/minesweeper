// System
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

// Unity
using UnityEngine;
using UnityEngine.Internal;

namespace GUPS.AntiCheat.Protected.Prefs
{
    /// <summary>
    /// Thread safe file based protected player prefs.
    /// </summary>
    public static class ProtectedFileBasedPlayerPrefs
    {
        /// <summary>
        /// Custom file path.
        /// Default is: Application.persistentDataPath + System.IO.Path.PathSeparator + "playerprefs.dat" 
        /// </summary>
        public static String FilePath { get; set; } = Application.persistentDataPath + System.IO.Path.PathSeparator + "playerprefs.dat";

        /// <summary>
        /// Lock for thread safety.
        /// </summary>
        private static object lockHandle = new object();

        /// <summary>
        /// Data structure of a single PlayerPrefs entry.
        /// </summary>
        private struct DataStruct
        {
            public EPlayerPrefsType PlayerPrefsType { get; }

            public String Key { get; }

            public String Value { get; }

            public String Hash { get; }

            public DataStruct(EPlayerPrefsType _PlayerPrefsType, String _Key, String _Value, String _Hash)
            {
                this.PlayerPrefsType = _PlayerPrefsType;
                this.Key = _Key;
                this.Value = _Value;
                this.Hash = _Hash;
            }
        }

        /// <summary>
        /// Key to DataStruct Dictionary.
        /// </summary>
        private static Dictionary<String, DataStruct> currentDataStructMapping;

        /// <summary>
        /// Returns true if key exists in the preferences.
        /// </summary>
        /// <param name="_Key"></param>
        /// <returns></returns>
        public static bool HasKey(String _Key)
        {
            // Load the mapping if not already loaded.
            Load();

            lock (lockHandle)
            {
                return currentDataStructMapping.ContainsKey(_Key);
            }
        }

        /// <summary>
        ///   <para>Sets the _Value of the preference identified by _Key.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_Value"></param>
        public static void SetInt(string _Key, int _Value)
        {
            // Load the mapping if not already loaded.
            Load();

            lock (lockHandle)
            {
                // Init Protected with value _Value
                ProtectedInt32 var_Protected = new ProtectedInt32(_Value);
                // Serialize the protected.
                var_Protected.Serialize(out Int32 var_ObfuscatedValue, out Int32 var_Secret);
                // Create new DataStruct
                DataStruct var_DataStruct = new DataStruct(EPlayerPrefsType.INT, _Key, var_ObfuscatedValue.ToString(), var_Secret.ToString());
                // Assign to mapping
                currentDataStructMapping[var_DataStruct.Key] = var_DataStruct;
            }

            // Save the mapping.
            Save();
        }

        /// <summary>
        ///   <para>Returns the value corresponding to _Key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_DefaultValue"></param>
        public static int GetInt(string _Key, [DefaultValue("0")] int _DefaultValue)
        {
            // Load the mapping if not already loaded.
            Load();

            lock (lockHandle)
            {
                if (currentDataStructMapping.ContainsKey(_Key))
                {
                    // Create empty protected by not using the empty constructor. The empty constructor would initialize the struct with the default / empty values.
                    ProtectedInt32 var_Protected = new ProtectedInt32(0);

                    // Load secured value
                    if (!int.TryParse(currentDataStructMapping[_Key].Value, out int var_Value))
                    {
                        return PlayerPrefs.GetInt(_Key, _DefaultValue);
                    }

                    // Load hash
                    if (!int.TryParse(currentDataStructMapping[_Key].Hash, out int var_Secret))
                    {
                        return PlayerPrefs.GetInt(_Key, _DefaultValue);
                    }

                    // Deserialize the protected.
                    var_Protected.Deserialize(var_Value, var_Secret);

                    return var_Protected.Value;
                }
            }

            return PlayerPrefs.GetInt(_Key, _DefaultValue);
        }

        /// <summary>
        ///   <para>Returns the value corresponding to _Key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        public static int GetInt(string _Key)
        {
            return GetInt(_Key, 0);
        }

        /// <summary>
        ///   <para>Sets the _Value of the preference identified by _Key.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_Value"></param>
        public static void SetFloat(string _Key, float _Value)
        {
            // Load the mapping if not already loaded.
            Load();

            lock (lockHandle)
            {
                unchecked
                {
                    // Init Protected with value _Value
                    ProtectedFloat var_Protected = new ProtectedFloat(_Value);
                    // Serialize the protected.
                    var_Protected.Serialize(out UInt32 var_ObfuscatedValue, out UInt32 var_Secret);
                    // Create new DataStruct
                    DataStruct var_DataStruct = new DataStruct(EPlayerPrefsType.FLOAT, _Key, var_ObfuscatedValue.ToString(), var_Secret.ToString());
                    // Assign to mapping
                    currentDataStructMapping[var_DataStruct.Key] = var_DataStruct;
                }
            }

            // Save the mapping.
            Save();
        }

        /// <summary>
        ///   <para>Returns the value corresponding to _Key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_DefaultValue"></param>
        public static float GetFloat(string _Key, [DefaultValue("0.0F")] float _DefaultValue)
        {
            // Load the mapping if not already loaded.
            Load();

            lock (lockHandle)
            {
                if (currentDataStructMapping.ContainsKey(_Key))
                {
                    unchecked
                    {
                        // Create empty protected by not using the empty constructor. The empty constructor would initialize the struct with the default / empty values.
                        ProtectedFloat var_Protected = new ProtectedFloat(0);

                        // Load secured value
                        if (!int.TryParse(currentDataStructMapping[_Key].Value, out int var_Value))
                        {
                            return PlayerPrefs.GetFloat(_Key, _DefaultValue);
                        }

                        // Load hash
                        if (!int.TryParse(currentDataStructMapping[_Key].Hash, out int var_Secret))
                        {
                            return PlayerPrefs.GetFloat(_Key, _DefaultValue);
                        }

                        // Deserialize the protected.
                        var_Protected.Deserialize((UInt32)var_Value, (UInt32)var_Secret);

                        return var_Protected.Value;
                    }
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
            return GetFloat(_Key, 0);
        }

        /// <summary>
        ///   <para>Sets the value of the preference identified by key.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_Value"></param>
        public static void SetString(string _Key, string _Value)
        {
            // Load the mapping if not already loaded.
            Load();

            lock (lockHandle)
            {
                //Init Protected with value _Value
                ProtectedString var_Protected = new ProtectedString(_Value);
                // Serialize the protected.
                var_Protected.Serialize(out String var_ObfuscatedValue, out Int32 var_Secret);
                // Create new DataStruct
                DataStruct var_DataStruct = new DataStruct(EPlayerPrefsType.STRING, _Key, var_ObfuscatedValue.ToString(), var_Secret.ToString());
                // Assign to mapping
                currentDataStructMapping[var_DataStruct.Key] = var_DataStruct;
            }

            // Save the mapping.
            Save();
        }

        /// <summary>
        ///   <para>Returns the value corresponding to key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_DefaultValue"></param>
        public static string GetString(string _Key, [DefaultValue("\"\"")] string _DefaultValue)
        {
            // Load the mapping if not already loaded.
            Load();

            lock (lockHandle)
            {
                if (currentDataStructMapping.ContainsKey(_Key))
                {
                    // Create empty protected by not using the empty constructor. The empty constructor would initialize the struct with the default / empty values.
                    ProtectedString var_Protected = new ProtectedString(String.Empty);

                    // Load intern Value
                    String var_Value = currentDataStructMapping[_Key].Value;

                    // Load hash
                    if (!int.TryParse(currentDataStructMapping[_Key].Hash, out int var_Secret))
                    {
                        return PlayerPrefs.GetString(_Key, _DefaultValue);
                    }

                    // Deserialize the protected.
                    var_Protected.Deserialize(var_Value, var_Secret);

                    return var_Protected.Value;
                }
            }

            return PlayerPrefs.GetString(_Key, _DefaultValue);
        }

        /// <summary>
        ///   <para>Returns the value corresponding to _Key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        public static string GetString(string _Key)
        {
            return GetString(_Key, "");
        }

        ////////////////////// CUSTOM ////////////////////////////

        /// <summary>
        ///   <para>Sets the _Value of the preference identified by _Key.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_Value"></param>
        public static void SetBool(string _Key, bool _Value)
        {
            // Load the mapping if not already loaded.
            Load();

            lock (lockHandle)
            {
                // Init Protected with value _Value
                ProtectedBool var_Protected = new ProtectedBool(_Value);
                // Serialize the protected.
                var_Protected.Serialize(out byte var_ObfuscatedValue, out int var_Secret);
                // Create new DataStruct
                DataStruct var_DataStruct = new DataStruct(EPlayerPrefsType.BOOL, _Key, var_ObfuscatedValue.ToString(), var_Secret.ToString());
                // Assign to mapping
                currentDataStructMapping[var_DataStruct.Key] = var_DataStruct;
            }

            // Save the mapping.
            Save();
        }

        /// <summary>
        ///   <para>Returns the value corresponding to _Key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_DefaultValue"></param>
        public static bool GetBool(string _Key, [DefaultValue("false")] bool _DefaultValue)
        {
            // Load the mapping if not already loaded.
            Load();

            lock (lockHandle)
            {
                if (currentDataStructMapping.ContainsKey(_Key))
                {
                    // Create empty protected by not using the empty constructor. The empty constructor would initialize the struct with the default / empty values.
                    ProtectedBool var_Protected = new ProtectedBool(false);

                    // Load secured value
                    if (!byte.TryParse(currentDataStructMapping[_Key].Value, out byte var_Value))
                    {
                        return _DefaultValue;
                    }

                    // Load hash
                    if (!int.TryParse(currentDataStructMapping[_Key].Hash, out int var_Secret))
                    {
                        return _DefaultValue;
                    }

                    // Deserialize the protected.
                    var_Protected.Deserialize(var_Value, var_Secret);

                    return var_Protected.Value;
                }
            }

            return _DefaultValue;
        }

        /// <summary>
        ///   <para>Returns the value corresponding to _Key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        public static bool GetBool(string _Key)
        {
            return GetBool(_Key, false);
        }

        /// <summary>
        ///   <para>Sets the value of the preference identified by key.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_Value"></param>
        public static void SetVector2(string _Key, Vector2 _Value)
        {
            // Load the mapping if not already loaded.
            Load();

            lock (lockHandle)
            {
                //Init Protected with value _Value
                ProtectedVector2 var_Protected = new ProtectedVector2(_Value);
                // Serialize the protected.
                var_Protected.Serialize(out UInt32 var_ObfuscatedValueX, out UInt32 var_ObfuscatedValueY, out UInt32 var_Secret);
                // Create new DataStruct
                DataStruct var_DataStruct = new DataStruct(EPlayerPrefsType.VECTOR2, _Key, var_ObfuscatedValueX + "|" + var_ObfuscatedValueY, var_Secret.ToString());
                // Assign to mapping
                currentDataStructMapping[var_DataStruct.Key] = var_DataStruct;
            }

            // Save the mapping.
            Save();
        }

        /// <summary>
        ///   <para>Returns the value corresponding to key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_DefaultValue"></param>
        public static Vector2 GetVector2(string _Key, Vector2 _DefaultValue)
        {
            // Load the mapping if not already loaded.
            Load();

            lock (lockHandle)
            {
                if (currentDataStructMapping.ContainsKey(_Key))
                {
                    // Create empty protected by not using the empty constructor. The empty constructor would initialize the struct with the default / empty values.
                    ProtectedVector2 var_Protected = new ProtectedVector2(Vector2.zero);

                    // Load value
                    String var_Value = currentDataStructMapping[_Key].Value;
                    String[] var_ValueSplit = var_Value.Split('|');

                    // Load hash
                    if (!int.TryParse(currentDataStructMapping[_Key].Hash, out int var_Secret))
                    {
                        return _DefaultValue;
                    }

                    // Deserialize the protected.
                    var_Protected.Deserialize(UInt32.Parse(var_ValueSplit[0]), UInt32.Parse(var_ValueSplit[1]), (UInt32)var_Secret);

                    return var_Protected.Value;
                }
            }

            return _DefaultValue;
        }

        /// <summary>
        ///   <para>Returns the value corresponding to _Key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        public static Vector2 GetVector2(string _Key)
        {
            return GetVector2(_Key, Vector2.zero);
        }

        /// <summary>
        ///   <para>Sets the value of the preference identified by key.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_Value"></param>
        public static void SetVector3(string _Key, Vector3 _Value)
        {
            // Load the mapping if not already loaded.
            Load();

            lock (lockHandle)
            {
                // Init Protected with value _Value
                ProtectedVector3 var_Protected = new ProtectedVector3(_Value);
                // Serialize the protected.
                var_Protected.Serialize(out UInt32 var_ObfuscatedValueX, out UInt32 var_ObfuscatedValueY, out UInt32 var_ObfuscatedValueZ, out UInt32 var_Secret);
                // Create new DataStruct
                DataStruct var_DataStruct = new DataStruct(EPlayerPrefsType.VECTOR3, _Key, var_ObfuscatedValueX + "|" + var_ObfuscatedValueY + "|" + var_ObfuscatedValueZ, var_Secret.ToString());
                // Assign to mapping
                currentDataStructMapping[var_DataStruct.Key] = var_DataStruct;
            }

            // Save the mapping.
            Save();
        }

        /// <summary>
        ///   <para>Returns the value corresponding to key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_DefaultValue"></param>
        public static Vector3 GetVector3(string _Key, Vector3 _DefaultValue)
        {
            // Load the mapping if not already loaded.
            Load();

            lock (lockHandle)
            {
                if (currentDataStructMapping.ContainsKey(_Key))
                {
                    // Create empty protected by not using the empty constructor. The empty constructor would initialize the struct with the default / empty values.
                    ProtectedVector3 var_Protected = new ProtectedVector3(Vector3.zero);

                    // Load value
                    String var_ObfuscatedValueString = currentDataStructMapping[_Key].Value;
                    String[] var_ObfuscatedValueStringSplit = var_ObfuscatedValueString.Split('|');

                    // Load hash
                    if (!int.TryParse(currentDataStructMapping[_Key].Hash, out int var_Secret))
                    {
                        return _DefaultValue;
                    }

                    // Deserialize the protected.
                    var_Protected.Deserialize(UInt32.Parse(var_ObfuscatedValueStringSplit[0]), UInt32.Parse(var_ObfuscatedValueStringSplit[1]), UInt32.Parse(var_ObfuscatedValueStringSplit[2]), (UInt32)var_Secret);

                    return var_Protected.Value;
                }
            }

            return _DefaultValue;
        }

        /// <summary>
        ///   <para>Returns the value corresponding to _Key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        public static Vector3 GetVector3(string _Key)
        {
            return GetVector3(_Key, Vector3.zero);
        }

        /// <summary>
        ///   <para>Sets the value of the preference identified by key.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_Value"></param>
        public static void SetVector4(string _Key, Vector4 _Value)
        {
            // Load the mapping if not already loaded.
            Load();

            lock (lockHandle)
            {
                // Init Protected with value _Value
                ProtectedVector4 var_Protected = new ProtectedVector4(_Value);
                // Serialize the protected.
                var_Protected.Serialize(out UInt32 var_ObfuscatedValueX, out UInt32 var_ObfuscatedValueY, out UInt32 var_ObfuscatedValueZ, out UInt32 var_ObfuscatedValueW, out UInt32 var_Secret);
                // Create new DataStruct
                DataStruct var_DataStruct = new DataStruct(EPlayerPrefsType.VECTOR4, _Key, var_ObfuscatedValueX + "|" + var_ObfuscatedValueY + "|" + var_ObfuscatedValueZ + "|" + var_ObfuscatedValueW, var_Secret.ToString());
                // Assign to mapping
                currentDataStructMapping[var_DataStruct.Key] = var_DataStruct;
            }

            // Save the mapping.
            Save();
        }

        /// <summary>
        ///   <para>Returns the value corresponding to key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_DefaultValue"></param>
        public static Vector4 GetVector4(string _Key, Vector4 _DefaultValue)
        {
            // Load the mapping if not already loaded.
            Load();

            lock (lockHandle)
            {
                if (currentDataStructMapping.ContainsKey(_Key))
                {
                    // Create empty protected by not using the empty constructor. The empty constructor would initialize the struct with the default / empty values.
                    ProtectedVector4 var_Protected = new ProtectedVector4(Vector4.zero);

                    // Load value
                    String var_ObfuscatedValueString = currentDataStructMapping[_Key].Value;
                    String[] var_ObfuscatedValueStringSplit = var_ObfuscatedValueString.Split('|');

                    // Load hash
                    if (!int.TryParse(currentDataStructMapping[_Key].Hash, out int var_Secret))
                    {
                        return _DefaultValue;
                    }

                    // Deserialize the protected.
                    var_Protected.Deserialize(UInt32.Parse(var_ObfuscatedValueStringSplit[0]), UInt32.Parse(var_ObfuscatedValueStringSplit[1]), UInt32.Parse(var_ObfuscatedValueStringSplit[2]), UInt32.Parse(var_ObfuscatedValueStringSplit[3]), (UInt32)var_Secret);


                    return var_Protected.Value;
                }
            }

            return _DefaultValue;
        }

        /// <summary>
        ///   <para>Returns the value corresponding to _Key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        public static Vector4 GetVector4(string _Key)
        {
            return GetVector4(_Key, Vector4.zero);
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
        public static Quaternion GetQuaternion(string _Key)
        {
            Vector4 var_Vector = GetVector4(_Key, Vector4.zero);
            return new Quaternion(var_Vector.x, var_Vector.y, var_Vector.z, var_Vector.w);
        }

        /// <summary>
        /// Removes the PlayerPrefs at _Key.
        /// </summary>
        /// <param name="_Key"></param>
        public static void DeleteKey(String _Key)
        {
            // Load the mapping if not already loaded.
            Load();

            lock (lockHandle)
            {
                currentDataStructMapping.Remove(_Key);
            }

            // Save the mapping.
            Save();
        }

        /// <summary>
        /// Loads the data struct mapping, if not already loaded.
        /// </summary>
        private static void Load()
        {
            lock (lockHandle)
            {
                if (currentDataStructMapping == null)
                {
                    currentDataStructMapping = new Dictionary<string, DataStruct>();

                    if (System.IO.File.Exists(FilePath))
                    {
                        using (FileStream var_FileStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            using (BinaryReader var_Reader = new BinaryReader(var_FileStream))
                            {
                                // Read the count.
                                int var_Count = var_Reader.ReadInt32();

                                // Read each element.
                                for (int i = 0; i < var_Count; i++)
                                {
                                    DataStruct var_DataStruct = ReadDataStruct(var_Reader);

                                    currentDataStructMapping[var_DataStruct.Key] = var_DataStruct;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Saves the data struct mapping if there is one.
        /// </summary>
        private static void Save()
        {
            lock (lockHandle)
            {
                if (currentDataStructMapping == null)
                {
                    return;
                }

                using (FileStream var_FileStream = new FileStream(FilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                {
                    using (BinaryWriter var_Writer = new BinaryWriter(var_FileStream))
                    {
                        // Write the count.
                        var_Writer.Write(currentDataStructMapping.Count);

                        // Write each element.
                        foreach(var var_Pair in currentDataStructMapping)
                        {
                            WriteDataStruct(var_Writer, currentDataStructMapping[var_Pair.Key]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Read a DataStruct from the _Reader.
        /// </summary>
        /// <param name="_Reader"></param>
        /// <returns></returns>
        private static DataStruct ReadDataStruct(BinaryReader _Reader)
        {
            EPlayerPrefsType var_PlayerPrefsType = (EPlayerPrefsType)_Reader.ReadByte();

            String var_Key = ReadString(_Reader);

            String var_Value = ReadString(_Reader);

            String var_Hash = ReadString(_Reader);

            return new DataStruct(var_PlayerPrefsType, var_Key, var_Value, var_Hash);
        }

        /// <summary>
        /// Writes a DataStruct to _Writer.
        /// </summary>
        /// <param name="_Writer"></param>
        /// <param name="_DataStruct"></param>
        private static void WriteDataStruct(BinaryWriter _Writer, DataStruct _DataStruct)
        {
            _Writer.Write((byte)_DataStruct.PlayerPrefsType);

            WriteString(_Writer, _DataStruct.Key);

            WriteString(_Writer, _DataStruct.Value);

            WriteString(_Writer, _DataStruct.Hash);
        }

        /// <summary>
        /// Read a string with length.
        /// </summary>
        /// <param name="_Reader"></param>
        /// <returns></returns>
        private static string ReadString(BinaryReader _Reader)
        {
            int var_Length = _Reader.ReadInt32();
            if (var_Length > 0 && var_Length <= _Reader.BaseStream.Length - _Reader.BaseStream.Position)
            {
                byte[] var_StringData = _Reader.ReadBytes(var_Length);
                String var_String = Encoding.UTF8.GetString(var_StringData);

                return var_String;
            }
            return "";
        }

        /// <summary>
        /// Write string and write the length.
        /// </summary>
        /// <param name="_Writer"></param>
        /// <param name="_String"></param>
        private static void WriteString(BinaryWriter _Writer, String _String)
        {
            byte[] var_Bytes = Encoding.UTF8.GetBytes(_String);
            _Writer.Write(var_Bytes.Length);
            _Writer.Write(var_Bytes);
        }
    }
}