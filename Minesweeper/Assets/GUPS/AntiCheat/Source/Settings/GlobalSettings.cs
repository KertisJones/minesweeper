// System
using System;
using System.Collections.Generic;

// Unity
using UnityEngine;

// GUPS - AntiCheat - Core
using GUPS.AntiCheat.Core.Hash;
using GUPS.AntiCheat.Core.Random;

// GUPS - AntiCheat
using GUPS.AntiCheat.Monitor.Android;

namespace GUPS.AntiCheat.Settings
{
    /// <summary>
    /// The global settings for the anti cheat monitor.
    /// </summary>
    public class GlobalSettings : ScriptableObject
    {
        // Instance
        #region Instance

        /// <summary>
        /// The relative unity resource path to the settings file.
        /// </summary>
        public const String RELATIVE_SETTINGS_PATH = "GUPS/AntiCheat/Settings/GlobalSettings";

        /// <summary>
        /// The absolute unity resource path in the project to the settings file.
        /// </summary>
        public const String SETTINGS_PATH = "Assets/GUPS/AntiCheat/Resources/" + RELATIVE_SETTINGS_PATH;

        /// <summary>
        /// The runtime settings singleton instance.
        /// </summary>
        private static GlobalSettings instance;

        /// <summary>
        /// Get or load the runtime settings singleton instance.
        /// </summary>
        public static GlobalSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = LoadAsset();
                }

                return instance;
            }
        }

        /// <summary>
        /// Load the settings asset from the resources.
        /// </summary>
        /// <returns></returns>
        public static GlobalSettings LoadAsset()
        {
            return Resources.Load<GlobalSettings>(RELATIVE_SETTINGS_PATH);
        }

        #endregion

        // Primitive
        #region Primitive

        /// <summary>
        /// A shared random provider used to generate random values.
        /// </summary>
        public static IRandomProvider RandomProvider { get; } = new PseudoRandom();

        #endregion

        // Android
        #region Android

        /// <summary>
        /// Set to true to validate (Appstore, Libraries, Applications, Signature, ...) the android app on development builds too. Set to false to not 
        /// validate the android app on development builds. Recommended: false.
        /// </summary>
        [SerializeField]
        public bool Android_Enable_Development = false;

        /// <summary>
        /// Set true to allow all package installation sources for your app. Set to false to allow only the package installation sources in the list 
        /// of allowed app stores.
        /// </summary>
        [SerializeField]
        public bool Android_AllowAllAppStores = true;

        /// <summary>
        /// A list of allowed package installation sources for the application. If the app is installed from a source not in the list, you will get a
        /// notification. You can react to those notifications and decide what you want to do from there.
        /// </summary>
        [SerializeField]
        public List<EAppStore> Android_AllowedAppStores = new List<EAppStore>();

        /// <summary>
        /// A list of allowed custom package installation sources for the application, if the store you wish to allow installation from is not in the 
        /// list of allowed app stores. Enter here the package names. For example for GooglePlayStore it is com.android.vending.
        /// </summary>
        [SerializeField]
        public List<String> Android_AllowedCustomAppStores = new List<String>();

        /// <summary>
        /// Set to true to verify the hash of the app with a remote source. Set to false to not verify the app hash. After you have built your app, calculate 
        /// the hash of the whole app (apk / aab). Store this hash somewhere on a server in the web, but accessible to your app. When the app starts, it can
        /// download the hash from the server and compares it with the hash of the app. If the hashes do not match, the app is not the original app and you 
        /// can react.
        /// </summary>
        [SerializeField]
        public bool Android_VerifyAppHash = false;

        /// <summary>
        /// The algorithm used to generate and validate the app hash. Recommend: SHA-256.
        /// </summary>
        [SerializeField]
        public EHashAlgorithm Android_AppHashAlgorithm = EHashAlgorithm.SHA256;

        /// <summary>
        /// The server get endpoint to read the app hash from. The server should return the hash of the whole app (apk / aab) as string to verify the app's 
        /// identity and ensure that it is not tampered with or shipped through an unauthorized source. The path can contain a placeholder '{version}' which will be
        /// replaced with the Application.version. For example: https://yourserver.com/yourapp/hash/{version} or https://yourserver.com/yourapp/hash?version={version}.
        /// Application.version returns the current version of the Application. To set the version number in Unity, go to Edit > Project Settings > Player. This is the 
        /// same as PlayerSettings.bundleVersion.
        /// </summary>
        [SerializeField]
        public String Android_AppHashEndpoint = "";

        /// <summary>
        /// Set to true to verify the app fingerprint. Set to false to not check the app fingerprint. The fingerprint or signature of the app is a unique 
        /// identifier. It is used to verify the app's identity and ensure that it is not tampered with. You can get the fingerprint directly from the app
        /// or you can use the following command on your keystore to get the fingerprint: keytool -list -v -keystore yourapp.keystore -alias youralias.
        /// </summary>
        [SerializeField]
        public bool Android_VerifyAppFingerprint = false;

        /// <summary>
        /// The algorithm used to generate and validate the app fingerprint. Recommend: SHA-256.
        /// </summary>
        [SerializeField]
        public EHashAlgorithm Android_AppFingerprintAlgorithm = EHashAlgorithm.SHA256;

        /// <summary>
        /// The actual app fingerprint used to verify the app's identity and ensure that it is not tampered with or shipped through an unauthorized source.
        /// </summary>
        [SerializeField]
        public String Android_AppFingerprint = "";

        /// <summary>
        /// Set true to use whitelisting and blacklisting for libraries. Set to false to allow all libraries to be used in the application.
        /// </summary>
        [SerializeField]
        public bool Android_UseWhitelistingForLibraries = false;

        /// <summary>
        /// A list of whitelisted libraries that are allowed to be used in the application. If the application uses a library that is not in the list,
        /// you will get a notification. You can react to those notifications and decide what you want to do from there. A very common modding process
        /// is to add libraries to the application, which contain cheats.
        /// </summary>
        [SerializeField]
        public List<String> Android_WhitelistedLibraries = new List<String>();

        /// <summary>
        /// A list of blacklisted libraries that are not allowed to be used in the application. If the application uses a library that is in the list,
        /// you will get a notification. You can react to those notifications and decide what you want to do from there. A very common modding process
        /// is to add libraries to the application, which contain cheats.
        /// </summary>
        [SerializeField]
        public List<String> Android_BlacklistedLibraries = new List<String>();

        /// <summary>
        /// Set to true to use blacklisting for apps on the device. Set to false to allow all apps to be used on the device. If the user as an app on
        /// their device that is blacklisted, you will get a notification. You can react to those notifications and decide what you want to do from
        /// there.
        /// </summary>
        [SerializeField]
        public bool Android_UseBlacklistingforApplication = false;

        /// <summary>
        /// A list of blacklisted applications that are not allowed to be used on the device. If the user as an app on their device that is blacklisted,
        /// you will get a notification. You can react to those notifications and decide what you want to do from there.
        /// </summary>
        [SerializeField]
        public List<String> Android_BlacklistedApplications = new List<String>();

        #endregion

        // Editor
        #region Editor

#if UNITY_EDITOR

        public static GlobalSettings CreateAsset()
        {
            // Create asset directory if it does not exist...
            if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(SETTINGS_PATH)))
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(SETTINGS_PATH));
            }

            // ... then create the asset.
            GlobalSettings var_Asset = CreateInstance<GlobalSettings>();
            UnityEditor.AssetDatabase.CreateAsset(var_Asset, SETTINGS_PATH + ".asset");
            UnityEditor.AssetDatabase.SaveAssets();
            return var_Asset;
        }

        public static GlobalSettings LoadOrCreateAsset()
        {
            GlobalSettings var_Asset = LoadAsset();
            if (var_Asset == null)
            {
                var_Asset = CreateAsset();
            }
            return var_Asset;
        }

        public static UnityEditor.SerializedObject GetSerializedAsset()
        {
            return new UnityEditor.SerializedObject(LoadOrCreateAsset());
        }

#endif

        #endregion
    }
}
