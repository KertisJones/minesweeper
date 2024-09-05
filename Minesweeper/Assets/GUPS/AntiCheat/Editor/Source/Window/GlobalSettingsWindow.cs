// Microsoft
using System;
using System.Collections.Generic;

// Unity
using UnityEngine;
using UnityEditor;

// GUPS - AnitCheat
using GUPS.AntiCheat.Monitor.Android;
using GUPS.AntiCheat.Settings;

// GUPS - AnitCheat - Editor
using GUPS.AntiCheat.Editor.Helper;

namespace GUPS.AntiCheat.Editor.Window
{
    /// <summary>
    /// Configuration window for AntiCheat Settings in the project settings.
    /// </summary>
    public static class GlobalSettingsWindow
    {
        /// <summary>
        /// The scroll position of the window.
        /// </summary>
        private static Vector2 scrollPosition;

        /// <summary>
        /// A static method to return a SettingsProvider for the AntiCheat settings, displayed in the project settings.
        /// </summary>
        /// <returns>The created SettingsProvider.</returns>
        [SettingsProvider]
        public static SettingsProvider CreateAntiCheatSettingsProvider()
        {
            // Create provider and initialize.
            SettingsProvider var_Provider = new SettingsProvider("Project/GuardingPearSoftware/AntiCheat", SettingsScope.Project);

            // Assign the name of the window.
            var_Provider.label = "AntiCheat";

            // Populate the search keywords to enable smart search filtering and label highlighting:
            var_Provider.keywords = new HashSet<string>(new[] { "Cheat", "Hack", "AntiCheat", "Protect", "Secure" });

            // Register a callback that draws the GUI and handles the interaction with the underlying serialized object.
            var_Provider.guiHandler = GetGui;

            // Return the provider.
            return var_Provider;
        }

        /// <summary>
        /// The GUI for the AntiCheat settings.
        /// </summary>
        /// <param name="_SearchContext">User search context.</param>
        private static void GetGui(String _SearchContext)
        {
            // Get the serialized object for the global settings.
            SerializedObject var_GlobalSettingsObject = GlobalSettings.GetSerializedAsset();

            // Update the serialized object.
            var_GlobalSettingsObject.Update();

            // Display the gui content.
            EditorGUILayout.LabelField("Centralized configuration for global AntiCheat-Settings. The settings apply at runtime and in the editor.", EditorStyles.wordWrappedLabel);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            // Draw message box that pro is required.
            EditorGUILayout.HelpBox("This is a Pro feature. You need to have the AntiCheat Pro version to use these features.", MessageType.Warning);

            // Disable the gui.
            GUI.enabled = false;

            EditorGUIUtility.labelWidth = 200;

            GetGeneralGui(var_GlobalSettingsObject);

            GetAppStoreGui(var_GlobalSettingsObject);

            GetAppHashGui(var_GlobalSettingsObject);

            GetAppFingerprintGui(var_GlobalSettingsObject);

            GetAppLibraryGui(var_GlobalSettingsObject);

            GetDeviceAppGui(var_GlobalSettingsObject);

            EditorGUILayout.Space(5);

            EditorGUIUtility.labelWidth = 0;

            // Enable the gui.
            GUI.enabled = true;

            EditorGUILayout.EndScrollView();

            // Apply changes.
            if (var_GlobalSettingsObject.ApplyModifiedProperties())
            {
            }

            // Dispose the object at the end, after all changes are applied. Required to prevent memory leaks.
            var_GlobalSettingsObject.Dispose();
        }

        /// <summary>
        /// Display the general settings for the AntiCheat.
        /// </summary>
        /// <param name="_GlobalSettingsObject">The serialized object of the global settings.</param>
        private static void GetGeneralGui(SerializedObject _GlobalSettingsObject)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField(new GUIContent("Android - Settings", "In this section you can configure the security settings for the Android build target."), EditorStyles.boldLabel);

            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(_GlobalSettingsObject.FindProperty("Android_Enable_Development"), new GUIContent("Verify development builds", "Check to validate (Appstore, Libraries, Applications, Signature, ...) the android app on development builds too. Uncheck to not validate the android app on development builds. Recommended: false."));
        }

        /// <summary>
        /// Display the app store settings for the AntiCheat.
        /// </summary>
        /// <param name="_GlobalSettingsObject">The serialized object of the global settings.</param>
        private static void GetAppStoreGui(SerializedObject _GlobalSettingsObject)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField(new GUIContent("Android - App Store - Settings", "In this subsection you can configure the app installation sources."), EditorStyles.boldLabel);

            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(_GlobalSettingsObject.FindProperty("Android_AllowAllAppStores"), new GUIContent("Allow all installation sources", "Check to allow all package installation sources for your app. Uncheck to allow only the package installation sources in the list of allowed app stores."));

            if (_GlobalSettingsObject.FindProperty("Android_AllowAllAppStores").boolValue == false)
            {
                // Get the property for the whitelisted app stores.
                var var_AllowedAppStoresProperty = _GlobalSettingsObject.FindProperty("Android_AllowedAppStores");

                // Create a list of the current checked app stores.
                List<EAppStore> var_CheckedAppStores = new List<EAppStore>();

                for (int i = 0; i < var_AllowedAppStoresProperty.arraySize; i++)
                {
                    var var_AppStore = (EAppStore)var_AllowedAppStoresProperty.GetArrayElementAtIndex(i).enumValueIndex;
                    var_CheckedAppStores.Add(var_AppStore);
                }

                // Create a list of the new checked app stores.
                List<EAppStore> var_NewCheckedAppStores = new List<EAppStore>();

                // Draw the label for the whitelisted app stores.
                EditorGUILayout.LabelField(new GUIContent("Allow following sources:", "A list of allowed package installation sources for the application. If the app is installed from a source not in the list, you will get a notification. You can react to those notifications and decide what you want to do from there."));

                // Darken the background color for the app stores.
                EditorGUILayout.BeginVertical(StyleHelper.DarkBackground);

                // AndroidPackageInstaller
                if (EditorGUILayout.Toggle(new GUIContent("Android Package Installer", "Package Installer (com.android.packageinstaller or com.google.android.packageinstaller). The installation of apps outside of stores is done by a system app that is integrated into every Android device. This system app, known as the package installer, is responsible for installing applications that originate from apk files downloaded from various locations."), var_CheckedAppStores.Contains(EAppStore.AndroidPackageInstaller)))
                {
                    var_NewCheckedAppStores.Add(EAppStore.AndroidPackageInstaller);
                }

                // AmazonAppstore
                if (EditorGUILayout.Toggle(new GUIContent("Amazon Appstore", "Amazon's digital application distribution platform (com.amazon.venezia)."), var_CheckedAppStores.Contains(EAppStore.AmazonAppstore)))
                {
                    var_NewCheckedAppStores.Add(EAppStore.AmazonAppstore);
                }

                // Aptoide
                if (EditorGUILayout.Toggle(new GUIContent("Aptoide", "An open-source independent Android app store (cm.aptoide.pt)."), var_CheckedAppStores.Contains(EAppStore.Aptoide)))
                {
                    var_NewCheckedAppStores.Add(EAppStore.Aptoide);
                }

                // CafeBazaar
                if (EditorGUILayout.Toggle(new GUIContent("Cafe Bazaar", "An Iranian Android marketplace (com.farsitel.bazaar)."), var_CheckedAppStores.Contains(EAppStore.CafeBazaar)))
                {
                    var_NewCheckedAppStores.Add(EAppStore.CafeBazaar);
                }

                // FDroid
                if (EditorGUILayout.Toggle(new GUIContent("F-Droid", "An open-source software repository for Android (org.fdroid.fdroid)."), var_CheckedAppStores.Contains(EAppStore.FDroid)))
                {
                    var_NewCheckedAppStores.Add(EAppStore.FDroid);
                }

                // GooglePlayStore
                if (EditorGUILayout.Toggle(new GUIContent("Google Play Store", "Google's official app store (com.android.vending)."), var_CheckedAppStores.Contains(EAppStore.GooglePlayStore)))
                {
                    var_NewCheckedAppStores.Add(EAppStore.GooglePlayStore);
                }

                // HuaweiAppGallery
                if (EditorGUILayout.Toggle(new GUIContent("Huawei AppGallery", "Huawei's official app distribution platform (com.huawei.appmarket)."), var_CheckedAppStores.Contains(EAppStore.HuaweiAppGallery)))
                {
                    var_NewCheckedAppStores.Add(EAppStore.HuaweiAppGallery);
                }

                // Myket
                if (EditorGUILayout.Toggle(new GUIContent("Myket", "A popular Android app store (ir.mservices.market)."), var_CheckedAppStores.Contains(EAppStore.Myket)))
                {
                    var_NewCheckedAppStores.Add(EAppStore.Myket);
                }

                // OppoAppMarket
                if (EditorGUILayout.Toggle(new GUIContent("Oppo App Market", "Oppo's official app store (com.oppo.market)."), var_CheckedAppStores.Contains(EAppStore.OppoAppMarket)))
                {
                    var_NewCheckedAppStores.Add(EAppStore.OppoAppMarket);
                }

                // SamsungGalaxyStore
                if (EditorGUILayout.Toggle(new GUIContent("Samsung Galaxy Store", "Samsung's official app store (com.sec.android.app.samsungapps)."), var_CheckedAppStores.Contains(EAppStore.SamsungGalaxyStore)))
                {
                    var_NewCheckedAppStores.Add(EAppStore.SamsungGalaxyStore);
                }

                // TapTap
                if (EditorGUILayout.Toggle(new GUIContent("TapTap", "A Chinese app store for mobile games (com.taptap)."), var_CheckedAppStores.Contains(EAppStore.TapTap)))
                {
                    var_NewCheckedAppStores.Add(EAppStore.TapTap);
                }

                // VivoAppStore
                if (EditorGUILayout.Toggle(new GUIContent("Vivo App Store", "Vivo's official app distribution platform (com.bbk.appstore)."), var_CheckedAppStores.Contains(EAppStore.VivoAppStore)))
                {
                    var_NewCheckedAppStores.Add(EAppStore.VivoAppStore);
                }

                // XiaomiMiGetApps
                if (EditorGUILayout.Toggle(new GUIContent("Xiaomi Mi GetApps", "Xiaomi's official app store (com.xiaomi.market)."), var_CheckedAppStores.Contains(EAppStore.XiaomiMiGetApps)))
                {
                    var_NewCheckedAppStores.Add(EAppStore.XiaomiMiGetApps);
                }

                // XDALabs
                if (EditorGUILayout.Toggle(new GUIContent("XDA Labs", "A platform for mobile development projects (com.xda.labs.play)."), var_CheckedAppStores.Contains(EAppStore.XDALabs)))
                {
                    var_NewCheckedAppStores.Add(EAppStore.XDALabs);
                }

                // Unknown
                if (EditorGUILayout.Toggle(new GUIContent("Unknown", "Unknown installation source. If it is neither of the above sources."), var_CheckedAppStores.Contains(EAppStore.Unknown)))
                {
                    var_NewCheckedAppStores.Add(EAppStore.Unknown);
                }

                // Add all or remove all.
                EditorGUILayout.BeginHorizontal();

                // A button to add all app stores.
                if (GUILayout.Button("Add All", EditorStyles.miniButtonLeft, GUILayout.Width(100), GUILayout.Height(20)))
                {
                    // Add all app stores.
                    var_NewCheckedAppStores.Clear();

                    foreach (EAppStore var_Store in Enum.GetValues(typeof(EAppStore)))
                    {
                        var_NewCheckedAppStores.Add(var_Store);
                    }
                }

                // A button to remove all app stores.
                if (GUILayout.Button("Remove All", EditorStyles.miniButtonRight, GUILayout.Width(100), GUILayout.Height(20)))
                {
                    // Clear all app stores.
                    var_NewCheckedAppStores.Clear();
                }

                // End the horizontal group.
                EditorGUILayout.EndHorizontal();

                // End the vertical group.
                EditorGUILayout.EndVertical();

                // Apply the new checked app stores to the property.
                var_AllowedAppStoresProperty.ClearArray();

                for (int i = 0; i < var_NewCheckedAppStores.Count; i++)
                {
                    var_AllowedAppStoresProperty.InsertArrayElementAtIndex(i);
                    var_AllowedAppStoresProperty.GetArrayElementAtIndex(i).enumValueIndex = (int)var_NewCheckedAppStores[i];
                }

                // Display the custom package installation sources.
                EditorGUILayout.PropertyField(_GlobalSettingsObject.FindProperty("Android_AllowedCustomAppStores"), new GUIContent("Allow custom sources", "A list of allowed custom package installation sources for the application, if the store you wish to allow installation from is not in the list of allowed app stores. Enter here the package names. \n\nFor example for GooglePlayStore it is com.android.vending."));
            }
        }

        /// <summary>
        /// Display the app hash settings for the AntiCheat.
        /// </summary>
        /// <param name="_GlobalSettingsObject">The serialized object of the global settings.</param>
        private static void GetAppHashGui(SerializedObject _GlobalSettingsObject)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField(new GUIContent("Android - App Hash - Settings", "In this subsection you can configure the validation of the app hash."), EditorStyles.boldLabel);

            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(_GlobalSettingsObject.FindProperty("Android_VerifyAppHash"), new GUIContent("Verify app hash", "Check to verify the hash of the app with a remote source. Uncheck to not verify the app hash. After you have built your app, AntiCheat calculates the hash of the enite app (apk / aab) and displays it in the log. Store this hash somewhere on a server in the web, but accessible to your app. When the app starts, it can download the hash from the server and compares it with the hash of the app. If the hashes do not match, the app is not the original app and you can react."));

            if (_GlobalSettingsObject.FindProperty("Android_VerifyAppHash").boolValue == true)
            {
                EditorGUILayout.PropertyField(_GlobalSettingsObject.FindProperty("Android_AppHashAlgorithm"), new GUIContent("Used hash algorithm", "The algorithm used to generate and validate the app hash. Recommend: SHA-256."));

                EditorGUILayout.PropertyField(_GlobalSettingsObject.FindProperty("Android_AppHashEndpoint"), new GUIContent("Remote hash location", "The server get endpoint to read the app hash from. The server should return the hash of the whole app (apk / aab) as string to verify the app's identity and ensure that it is not tampered with or shipped through an unauthorized source. The path can contain a placeholder '{version}' which will be replaced with the Application.version.\n\nFor example: https://yourserver.com/yourapp/hash/{version} or https://yourserver.com/yourapp/hash?version={version}.\n\nApplication.version returns the current version of the Application. To set the version number in Unity, go to Edit>Project Settings>Player. This is the same as PlayerSettings.bundleVersion."));
            }
        }

        /// <summary>
        /// Display the app fingerprint settings for the AntiCheat.
        /// </summary>
        /// <param name="_GlobalSettingsObject">The serialized object of the global settings.</param>
        private static void GetAppFingerprintGui(SerializedObject _GlobalSettingsObject)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField(new GUIContent("Android - App Fingerprint - Settings", "In this subsection you can configure the validation of the app fingerprint or signature."), EditorStyles.boldLabel);

            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(_GlobalSettingsObject.FindProperty("Android_VerifyAppFingerprint"), new GUIContent("Verify app fingerprint", "Check to verify the app fingerprint. Uncheck to not check the app fingerprint. The fingerprint or signature of the app is a unique identifier. It is used to verify the app's identity and ensure that it is not tampered with. \n\nYou can get the fingerprint directly from the app or you can use for example the following command on your keystore to get the fingerprint: keytool -list -v -keystore yourapp.keystore -alias youralias."));

            if (_GlobalSettingsObject.FindProperty("Android_VerifyAppFingerprint").boolValue == true)
            {
                EditorGUILayout.PropertyField(_GlobalSettingsObject.FindProperty("Android_AppFingerprintAlgorithm"), new GUIContent("Used hash algorithm", "The algorithm used to generate and validate the app fingerprint. Recommend: SHA-256."));

                EditorGUILayout.PropertyField(_GlobalSettingsObject.FindProperty("Android_AppFingerprint"), new GUIContent("Fingerprint", "The actual app fingerprint used to verify the app's identity and ensure that it is not tampered with or shipped through an unauthorized source. Enter as hex-string."));
            }
        }

        /// <summary>
        /// Display the app library settings for the AntiCheat.
        /// </summary>
        /// <param name="_GlobalSettingsObject">The serialized object of the global settings.</param>
        private static void GetAppLibraryGui(SerializedObject _GlobalSettingsObject)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField(new GUIContent("Android - App Library - Settings", "In this subsection you can configure the validation of the app libraries."), EditorStyles.boldLabel);

            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(_GlobalSettingsObject.FindProperty("Android_UseWhitelistingForLibraries"), new GUIContent("White-/Blacklist libraries", "Check to use whitelisting and blacklisting for libraries. Uncheck to allow all libraries to be used in the app."));

            if (_GlobalSettingsObject.FindProperty("Android_UseWhitelistingForLibraries").boolValue == true)
            {
                EditorGUILayout.PropertyField(_GlobalSettingsObject.FindProperty("Android_WhitelistedLibraries"), new GUIContent("Whitelisted libraries", "A list of whitelisted libraries that are allowed to be used in the application. If the application uses a library that is not in the list, you will get a notification. You can react to those notifications and decide what you want to do from there. A very common modding process is to add libraries to the application, which contain cheats."));

                EditorGUILayout.PropertyField(_GlobalSettingsObject.FindProperty("Android_BlacklistedLibraries"), new GUIContent("Blacklisted libraries", "A list of blacklisted libraries that are not allowed to be used in the application. If the application uses a library that is in the list, you will get a notification. You can react to those notifications and decide what you want to do from there. A very common modding process is to add libraries to the application, which contain cheats."));
            }
        }

        /// <summary>
        /// Display the device app settings for the AntiCheat.
        /// </summary>
        /// <param name="_GlobalSettingsObject">The serialized object of the global settings.</param>
        private static void GetDeviceAppGui(SerializedObject _GlobalSettingsObject)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField(new GUIContent("Android - Device App - Settings", "In this subsection you can configure the validation of apps on the device."), EditorStyles.boldLabel);

            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(_GlobalSettingsObject.FindProperty("Android_UseBlacklistingforApplication"), new GUIContent("Blacklist device apps", "Check to use blacklisting for apps on the device. Uncheck to allow all apps to be used on the device. If the user as an app on their device that is blacklisted, you will get a notification. You can react to those notifications and decide what you want to do from there."));

            if (_GlobalSettingsObject.FindProperty("Android_UseBlacklistingforApplication").boolValue == true)
            {
                EditorGUILayout.PropertyField(_GlobalSettingsObject.FindProperty("Android_BlacklistedApplications"), new GUIContent("Blacklisted apps", "A list of blacklisted applications that are not allowed to be used on the device. If the user as an app on their device that is blacklisted, you will get a notification. You can react to those notifications and decide what you want to do from there."));
            }
        }
    }
}