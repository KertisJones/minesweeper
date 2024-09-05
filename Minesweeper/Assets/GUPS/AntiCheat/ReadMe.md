**NOTE: Check out the online docs for the latest version plus images: https://docs.guardingpearsoftware.com/manual/AntiCheat/Description.html**

# How To Start

This guide will assist you in obtaining AntiCheat, setting it up, and preparing your game to protect against memory, storage, and time manipulation, while ensuring that your game maintains its integrity.

## Step 1 - Get & Install AntiCheat

The very first step is to get either [AntiCheat Free](https://assetstore.unity.com/packages/slug/140341) or [AntiCheat Pro](https://assetstore.unity.com/packages/slug/117539) from the Unity Asset Store. Then download it into your current project.

The AntiCheat project structure 'Assets/GUPS/AntiCheat' will look like the following.

You find the following directories and files in the root directory of AntiCheat:
- **Demos**: Contains various demos to simplify your start.
- **Editor:** Contains all resources and sources for the editor.
- **Plugins:** Contains platform specific plugins.
- **Resources:** Contains Prefabs like the AntiCheat-Monitor and the global AntiCheat-Settings.
- **Source:** Contains all runtime source code.
- **Tests:** Contains unit tests for all runtime components.
- **ReadMe.md:** A readme file that contains the same content as this page, but locally.
- **VersionHistory.md:** This file contains a detailed record of all the changes made in each version of the asset.

## Step 2 - Setup Project Settings

AntiCheat has a centralized configuration which applies global settings at runtime and in the editor. You can find it at 'Edit -> Project settings -> GuardingPearSoftware -> AntiCheat'. AntiCheat has currently only global settings for the Android platform. These settings work in combination with the Android Monitors and Detectors, check out the chapter 'Protect Android'.

## Step 3 - Add AntiCheat-Monitor

AntiCheat by GuardingPearSoftware protects your game memory, storage, time and detects and prevents tampering attempts. It ensures the integrity of the gaming experience for you and your honest players. Todo so it is built on four pillars: *Protection, Monitoring, Detection and Punishment*.

1. **Protection:** To protect against cheaters and hackers, encrypt or obfuscate your data so that it is protected against attacks. Countermeasures are taken to protect the integrity of the data and trap attackers, for example through honey pots.
2. **Monitoring:** Observe and track the state of the game or device and the behavior of players to detect potential cheating or unusual behavior.
3. **Detection:** Monitoring itself does not report cheating activity, but only tracks the state of the game or device and monitors deviations. These deviations are recorded by a detector. The detector itself checks the monitored deviations and informs the 'AntiCheat-Monitor' about possible cases of tampering or cheating.
4. **Punishment:** React to detected cheating attempts with punishers when a calculated threat level is reached and can impose various penalties.

The 'AntiCheat-Monitor' serves as the heart of the system and orchestrates the *Protection, Monitoring, Detection, and Punishment* features. As singleton instance it spans the entire application and carefully monitors both the game and the device to detect potential cheating attempts. Once detected, appropriate measures can be taken to impose consequences on the cheater or hacker.

You can either add the 'AntiCheat-Monitor' manually or using a prepared prefab.

**Manual**

To add the 'AntiCheat-Monitor' create a new GameObject in your Scene Hierarchy. Add the 'AntiCheatMonitor' MonoBehavior from the 'GUPS.AntiCheat' namespace to the GameObject.

**Prefab**

There is also a prefab for the 'AntiCheat-Monitor' which you can directly attach as a GameObject to your scene. You can find the 'AntiCheat-Monitor' under 'GUPS/AntiCheat/Resources/Prefabs'. As a persistent singleton, it will remain throughout the game. You can add Monitors, Detectors and Punisher as components, or use the predefined prefabs and attach them as children of the 'AntiCheat-Monitor'.

## Step 4 - Protect Memory

Sensitive information, such as the position of the player character or health data, is often stored in the runtime memory. However, this data is vulnerable to manipulation by cheat software or data sniffers. It is therefore advisable to protect this data. 

AntiCheat offers protected data types for all data types that you will use in your daily work with Unity. You only need to replace your currently used types with the protected types. The protected types provide the same functionality as the standard types, so no code modifications are required.

**Data Types**

| \#                | Free | Pro |
| ----------------- | ---- | --- |
| Primitives        | +    | +   |
| Collections       |      | +   |
| DataChain         |      | +   |
| BlockChain        |      | +   |

A brief overview over the available protected data types:

- **Primitives:** AntiCheat provides protected data type for all primitives. Primitives are fundamental data type. This includes default value types like int, float, string, ... and all Unity value types like Vector, Quaternion, ...

- **Collections:** Important information can be not only within basic data types such as integer or float, but also within collections. AntiCheat provides protected alternatives for commonly used collections such as List, Queue and Stack, allowing you to monitor changes and check their integrity.

- **DataChain:** A data chain is similar to a linked list consisting of a sequence of elements arranged in a specific order. It is used to maintain the order of these elements while preserving their integrity. Data chain can be useful in scenarios where you want to manage e.g. "Digital Assets", "Achievements", "Virtual Currencies" etc.

- **BlockChain:** A blockchain can be compared to a datachain that connects blocks in a continuous sequence. Each block within this chain records transactions, containing the actual data. And this is where it differs from a datachain. The datachain is used as a local in memory data store. But the blockchain can synchronize its transactions with a remote source to retrieve and upload data while maintaining the integrity of the blockchain.

**Detector**

| Detector - Universal        | Free | Pro |
| --------------------------  | ---- | --- |
| PrimitiveCheatingDetector   | +    | +   |

Each protected data type is not only encrypted but also includes a honeypot. This way, if a cheater attempts to tamper with the protected data, they'll get caught in the honeypot trap. Every time a protected data type is accessed, whether through reading or writing, the its integrity will be checked. If someone trid to manipulate the data and triggers the honeypot, the 'PrimitiveCheatingDetector' is notified. 

The 'PrimitiveCheatingDetector' helps identify unauthorized changes to protected data, typically caused by memory manipulation or cheating. Add the detector to your 'AntiCheat-Monitor' (more details are provided within the detector's documentation). To react to detection look in the last chapter 'React To Detected Cheating'.

## Step 5 - Protect PlayerPrefs

Unity's PlayerPrefs serve as a convenient means of saving user settings and data. However, they lack protection or encryption, which makes them vulnerable to easy modification. The AntiCheat Protected PlayerPrefs provide a solution to this security problem. Similar to the protected data types, it is just plug-n-play and simple replacement.

**Protected Prefs**

| \#                             | Free | Pro |
| ------------------------       | ---- | --- |
| ProtectedPlayerPrefs           | +    | +   |
| ProtectedFileBasedPlayerPrefs  | +    | +   |

A brief overview over the available protected player prefs:

- **ProtectedPlayerPrefs:** This option replaces the default Unity PlayerPrefs features while introducing additional features. The protected prefs are stored in the known default location.

- **ProtectedFileBasedPlayerPrefs:** This is a customised implementation of Unity PlayerPrefs that enables the storage of protected player preferences under a specific file path (default: Application.persistentDataPath). To use a custom file path, set ProtectedFileBasedPlayerPrefs.FilePath. Then use ProtectedFileBasedPlayerPrefs in the same way as the standard Unity PlayerPrefs.

**Detector**

The protected PlayerPrefs have no active detector watching the PlayerPrefs. The protected PlayerPrefs are stored encrypted. If a cheater tries to modify them, the data is simple no longer valid, when reading it.

## Step 6 - Protect Game Time

All aspects of Unity, whether scripts, animations, or physics updates, depend on passed time between frames, typically referenced through deltaTime or fixedDeltaTime. This is a common target for cheaters, who often attempt to manipulate Unity's time to accelerate, decelerate or interrupt the game. 

AntiCheat introduced a ProtectedTime static class which you can use to replace your UnityEngine.Time static class usage. It gives you direct access to a protected time instance that prevents possible changes and ensures the integrity of your application's time-based calculations.

**Protected Time**

| \#                     | Free | Pro |
| ------------           | ---- | --- |
| Protected Game Time    |      | +   |

The ProtectedTime class offers a replacement for the default Time class. But the ProtectedTime does not know itself when a player cheats and modifies the game time. This is the task of the monitor and detector. Based on the AntiCheat four pillars: *Protection, Monitoring, Detection, and Punishment*.

**Monitor**

| Monitor - Universal | Free | Pro |
| -----------------   | ---- | --- |
| GameTimeMonitor     |      | +   |

To monitor the game time, AntiCheat has introduced the 'GameTimeMonitor'. The monitor tracks the game time and notifies the 'GameTimeCheatingDetector' of time deviations (paused, slowed down, speed up), that may be caused by a cheater.

Add the 'GameTimeMonitor' to your 'AntiCheat-Monitor' to start monitoring the game time (more details are provided within the monitor's documentation). 

**Detector**

| Detector - Universal        | Free | Pro |
| --------------------------  | ---- | --- |
| GameTimeCheatingDetector    |      | +   |

The 'GameTimeCheatingDetector' is used to detect actual game time cheating. It observes the 'GameTimeMonitor' and subscribes to time deviations, based on this it calculates the possibility of cheating and notifies observers of the detected cheating. Additionally, it starts doing counter measures by calculating the game time based on system ticks, if a cheating got detected. So even if cheated, the game time will be calculated correctly and applied in the ProtectedTime static class.

Add the 'GameTimeCheatingDetector' to your 'AntiCheat-Monitor' (more details are provided within the detector's documentation). To react to detection look in the last chapter 'React To Detected Cheating'.

## Step 7 - Protect Device Time

Relying on the user's device clock for time-sensitive applications poses risks due to potential manipulation, which cheaters might exploit to gain advantages and skip time related event or extend trial periods. 

To counter this, AntiCheat offers a secure way in the ProtectedTime static class to obtain Coordinated Universal Time (UTC), using a reference that can be either the device's local time or a trusted Internet source. This approach prevents users from altering time to cheat in your Unity games.

**Protected Time**

| \#                     | Free | Pro |
| ------------           | ---- | --- |
| Protected Device Time  |      | +   |

Similar to the game time, the device time in the AntiCheat ProtectedTime static class does not calculate a reliable Coordinated Universal Time (UTC) itself. This is the task of the monitor and detector. Based on the AntiCheat four pillars: *Protection, Monitoring, Detection, and Punishment*.

**Monitor**

| Monitor - Universal | Free | Pro |
| -----------------   | ---- | --- |
| DeviceTimeMonitor   |      | +   |

To monitor the device time, AntiCheat has introduced the 'DeviceTimeMonitor'. The monitor tracks the device time and notifies the 'DeviceTimeCheatingDetector' of time deviations (different date time than expected), that may be caused by a cheater.

The monitor tracks the device time for deviations each time the application is resumed by unpausing or focusing. During this, the actual device time is compared with the time that has elapsed between the suspending and the resuming.

Add the 'DeviceTimeMonitor' to your 'AntiCheat-Monitor' to start monitoring the device time (more details are provided within the monitor's documentation). 

**Detector**

| Detector - Universal        | Free | Pro |
| --------------------------  | ---- | --- |
| DeviceTimeCheatingDetector  |      | +   |

The 'DeviceTimeCheatingDetector' is used to detect device or system time manipulation. It observes the 'DeviceTimeMonitor' and subscribes to time deviations, based on this it calculates the possibility of cheating and notifies observers of the detected cheating. It also provides a trustworthy DateTime.UtcNow either calculated based on the internet time or device time, provided in the ProtectedTime class.

Add the 'DeviceTimeCheatingDetector' to your 'AntiCheat-Monitor' (more details are provided within the detector's documentation). To react to detection look in the last chapter 'React To Detected Cheating'.

## Step 8 - Protect Mobile

Mobile apps are often targets for unauthorized redistribution, with cheaters modifying them to gain advantages, bypass payment systems or even rebranding and republishing them. But AntiCheat helps you to stop that!

### Genuine Validation

AntiCheat introduces an integrity detector to validate if a mobile app (Android and iOS) is still genuine. It validates the package name of the build app against the running app. On Android, it should be combined with the AntiCheat solutions specially developed for Android apps, as the general genuine validation check does not detect all possible manipulations.

**Detector**

| Detector - Mobile        | Free | Pro |
| ------------------------ | ---- | --- |
| MobileGenuineDetector    | +    | +   |

The genuine validation comes without a monitor. The whole check happens in the genuine detector itself. Add the 'MobileGenuineDetector' to your 'AntiCheat-Monitor' (more details are provided within the detector's documentation). To react to detection look in the last chapter 'React To Detected Cheating'.

## Step 9 - Protect Android

Mobile iOS apps are generally more secure, as Apple itself does not allow installation from any store or simple sideloading. It's a little different with Android. So AntiCheat introduced multiple ways to validate and secure your apps.

### Validate Installation Source

By validating the installation source, you can check whether your app was installed by official app stores and not by third parties. Cheater often offer a manipulated app as a direct download so that users can install it directly.

**Monitor**

| Monitor - Android                  | Free | Pro |
| -----------------                  | ---- | --- |
| AndroidPackageSourceMonitor        |      | +   |

To validate the installation source you first need a 'AndroidPackageSourceMonitor'. It is used to find the installation source of the Android app. For example, if it was installed via the Google Play Store. The 'AndroidPackageTamperingDetector' subscribes to this and checks whether the source belongs to the allowed installation sources.

Add the 'AndroidPackageSourceMonitor' to your 'AntiCheat-Monitor' to start monitoring the installation source (more details are provided within the monitor's documentation). 

**Project Settings**

To assign the allowed installation sources, go to 'Edit -> Project settings -> GuardingPearSoftware -> AntiCheat'. Go to the 'Android - App Store - Settings' section.

In the settings you can find the following options:
- **Allow all installation sources:** Check to allow all package installation sources for your app. Uncheck to allow only the package installation sources in the list of allowed app stores.

- **Allow following sources:** If not all installation sources are allowed. You can assign here a list of allowed package installation sources. If the app is installed from a source not in the list, the detector will allow you to react to it.

- **Allow custom sources:** A list of allowed custom package installation sources for the application, if the store you wish to allow installation from is not in the list of allowed app stores. Enter here the package names. For example for the Google Play Store it is com.android.vending.

**Detector**

| Detector - Android                 | Free | Pro |
| -----------------                  | ---- | --- |
| AndroidPackageTamperingDetector    |      | +   |

The 'AndroidPackageTamperingDetector' is an aggregated detector that observes multiple Android monitors and validates their output for possible cheating. It validates the allowed installation source set in the global AntiCheat project settings.

Add the 'AndroidPackageTamperingDetector' to your 'AntiCheat-Monitor' (more details are provided within the detector's documentation). To react to detection look in the last chapter 'React To Detected Cheating'.

### Validate App Hash

Validating the entire app hash is a good way to determine whether the app has been modified in any way. Be it a different package name or changed code or other resources. 

**Monitor**

| Monitor - Android                  | Free | Pro |
| -----------------                  | ---- | --- |
| AndroidPackageHashMonitor          |      | +   |

To monitor the hash of the app, AntiCheat has introduced the 'AndroidPackageHashMonitor'. The monitor calculates the hash of the entire app (APK/AAB) itself and notifies the detector about the calculated hash. This hash can be compared with a remote source to detect if the app is in it original state or was modified or tampered with.

Add the 'AndroidPackageHashMonitor' to your 'AntiCheat-Monitor' to calculate your apps hash at runtime (more details are provided within the monitor's documentation). 

**Project Settings**

To activate hash validation and assigning the remote source, go to 'Edit -> Project Settings -> GuardingPearSoftware -> AntiCheat'. Go to the 'Android - App Hash - Settings' section.

In the settings you can find the following options:
- **Verify app hash:** Enable to verify the hash of the app with a remote source. Deactivate to not verify the app hash. After you have built your app, AntiCheat calculates the hash of the enite app (apk / aab) and displays it in the log. Store 
this hash somewhere on a server in the web, but accessible to your app. When the app starts, it can download the hash from the server and compares it with the hash of the app. If the hashes do not match, the app is not the original app.

- **Used hash algorithm:** The algorithm used to generate and validate the app hash. Recommend: SHA-256.

- **Remote hash location:** The server get endpoint to read the app hash from. The server should return the hash of the whole app (apk / aab) as string. The path can contain a placeholder '{version}' which will be replaced with the Application.version. For example: https://yourserver.com/yourapp/hash/{version} or https://yourserver.com/yourapp/hash?version={version}. Application.version returns the current version of the Application. To set the version number in Unity, go to 'Edit -> Project Settings -> Player'. This is the same as PlayerSettings.bundleVersion.

When you build your Android app, AntiCheat calculates the hash of your app and logs it in your editor console at the end of the build. Of course, you can also calculate the hash manually.

You can copy paste this hash hex string to a remote location, for example your server and make it available through a get request. 

> [!NOTE]
> The calculated hash changes with every build, even if you have not changed anything. So make sure that you return and validate the correct hash. To differ the hash for different build versions, you can use the placeholder '{version}' in your request URL. This will then be replaced by the 'Application.version' on request.

Assign your build version in the 'Edit -> Project Settings -> Player'.

The remote source or server get response should only return the hex string of the app hash. For example, here is a small node.js express server script of what this could look like:

```javascript
import express from 'express';

const app = express();

app.get('/hash', (req, res) => {
    if(req.query.version === '0.2') {
      res.send('00:E4:C4:13:2F:09:91:4A:B5:A0:D6:64:AC:38:FD:50:82:02:3C:45:5E:64:69:B1:F7:0E:43:04:14:1C:1A:3A');
      return;
    }
    res.send('Unknown version');
});

app.listen(4000, () => {
  console.log(`server running on port 4000`);
});
```

**Detector**

| Detector - Android                 | Free | Pro |
| -----------------                  | ---- | --- |
| AndroidPackageTamperingDetector    |      | +   |

The 'AndroidPackageTamperingDetector' is an aggregated detector that observes multiple Android monitors and validates their output for possible cheating. It compares the  calculated local hash with the one returned from the remote source set in the AntiCheat project settings. If it is not equals to the one returned from the remote source, the detector will detect this as cheating. 

Add the 'AndroidPackageTamperingDetector' to your 'AntiCheat-Monitor' (more details are provided within the detector's documentation). To react to detection look in the last chapter 'React To Detected Cheating'.

### Validate App Fingerprint

By validating your apps certificate fingerprint you can make sure the app is shipped by you and no one else.

**Monitor**

| Monitor - Android                  | Free | Pro |
| -----------------                  | ---- | --- |
| AndroidPackageFingerprintMonitor   |      | +   |

To monitor the certificate fingerprint of the app, AntiCheat has introduced the 'AndroidPackageFingerprintMonitor'. The monitor reads the signed fingerprint of the app and hashs it using a hash algorithm to make it more human readable, because the fingerprint is stored as binary. Once the fingerprint is read, it notifies the detector.

Add the 'AndroidPackageFingerprintMonitor' to your 'AntiCheat-Monitor' to start monitoring the installation source (more details are provided within the monitor's documentation). 

**Project Settings**

To activate fingerprint validation and also set the fingerprint itself, go to 'Edit -> Project Settings -> GuardingPearSoftware -> AntiCheat'. Go to the 'Android - App Fingerprint - Settings' section.

In the settings you can find the following options:
- **Verify app fingerprint:** Enable to verify the app fingerprint. Disable to not check the app fingerprint. The fingerprint or signature of the app is a unique identifier. It is used to verify the app's identity and ensure that it is not tampered with.

- **Used hash algorithm:** The algorithm used to generate and validate the app fingerprint. Recommend: SHA-256.

- **Fingerprint:** The actual app fingerprint used to verify the app's identity and ensure that it is not tampered with or shipped through an unauthorized source. Enter as hex string.

The fingerprint is the public part of a certificate to validate its authenticity. A certificate is digitally applied to an app by using a private key (stored in a key store). It is created by a developer or an organization and is unique to their apps.

The key store is assigned by you through the 'Edit -> Project Settings -> Player -> Publishing Settings'.

You can get the fingerprint directly from the app or from your keystore:

```
// 1. Open your cmd / terminal.
// 2. Go to your jdk location (for example):
cd "C:\Program Files\Java\jdk-17\bin"

// 3. Get the fingerprint from you keystore:
keytool.exe -list -v -keystore "[Project]\user.keystore"
```

Enter the fingerprint (recommended at least SHA256) in the AntiCheat 'Android - App Fingerprint - Settings'. As long as you use the same key store and project key for you app, you only have to enter the fingerprint once, because it does not change.

**Detector**

| Detector - Android                 | Free | Pro |
| -----------------                  | ---- | --- |
| AndroidPackageTamperingDetector    |      | +   |

The 'AndroidPackageTamperingDetector' is an aggregated detector that observes multiple Android monitors and validates their output for possible cheating. It validates the fingerprint of the app certificate. If it is not equals to the one stored in the settings, the detector will detect this as cheating

Add the 'AndroidPackageTamperingDetector' to your 'AntiCheat-Monitor' (more details are provided within the detector's documentation). To react to detection look in the last chapter 'React To Detected Cheating'.

### Validate App Libraries

A common cheat method in Unity Android apps is to insert custom libraries into your app instead of modifying the existing code. These libraries contain cheats that then give the player advantages.

**Monitor**

| Monitor - Android                  | Free | Pro |
| -----------------                  | ---- | --- |
| AndroidPackageLibraryMonitor       |      | +   |

To monitor the library files found in the app, AntiCheat has introduced the 'AndroidPackageLibraryMonitor'. The monitor reads all files located in the library directory, for example '{APK}\lib\armeabi-v7a\'. As soon as the libraries have been read, it notifies the detector.

Add the 'AndroidPackageLibraryMonitor' to your 'AntiCheat-Monitor' to start monitoring the installation source (more details are provided within the monitor's documentation). 

**Project Settings**

To activate whitelisting or blacklisting of libraries, go to 'Edit -> Project Settings -> GuardingPearSoftware -> AntiCheat'. Go to the 'Android - App Library - Settings' section.

In the settings you can find the following options:
- White-/Blacklist libraries: Enable to use whitelisting and blacklisting for libraries. Disable to allow all libraries to be used in the app.

- Whitelisted libraries: A list of whitelisted libraries that are allowed to be used in the application. If the application uses a library that is not in the list, you will get a notification. You can react to those notifications and decide what you want to do from there. A very common modding process is to add libraries to the application, which contain cheats.

- Blacklisted libraries: A list of blacklisted libraries that are not allowed to be used in the application. If the application uses a library that is in the list, you will get a notification. You can react to those notifications and decide what you want to do from there. A very common modding process is to add libraries to the application, which contain cheats.

To find the libraries of your app, open your app with a zip-filemanager (for example 7Zip), go to the library directory inside the opened app (for example 'lib\armeabi-v7a\') and enter all found libraries into the 'Android - App Library - Settings'. Enter the fullname including the extension.

**Detector**

| Detector - Android                 | Free | Pro |
| -----------------                  | ---- | --- |
| AndroidPackageTamperingDetector    |      | +   |

The 'AndroidPackageTamperingDetector' is an aggregated detector that observes multiple Android monitors and validates their output for possible cheating. It validates the monitored libraries and compares them with the ones from the AntiCheat project settings. If there are libraries found that are not whitelisted or are blacklisted, the detector will detect this as cheating

Add the 'AndroidPackageTamperingDetector' to your 'AntiCheat-Monitor' (more details are provided within the detector's documentation). To react to detection look in the last chapter 'React To Detected Cheating'.

### Validate Installed Apps

Not only can a user modify or manipulate your game or app, but they can also try to gain an advantage by making changes to their device. For example, by installing cheat engine apps that try to manipulate the device's or game's memory and gain advantages.

**Monitor**

| Monitor - Android                  | Free | Pro |
| -----------------                  | ---- | --- |
| AndroidInstalledApplicationMonitor |      | +   |

To monitor the installed apps on the users device, AntiCheat has introduced the 'AndroidInstalledApplicationMonitor'. The monitor reads and provides information about the apps installed on the device (system apps are ignored) and passes the package name of the apps found to the detector.

Add the 'AndroidPackageSourceMonitor' to your 'AntiCheat-Monitor' (more details are provided within the monitor's documentation). To react to detection look in the last chapter 'React To Detected Cheating'.

**Project Settings**

To define the unwanted apps you have to enter their package names in the global 'AntiCheat-Project Settings'. Do do so, go to 'Project Settings -> GuardingPearSoftware -> AntiCheat'. Go to the section 'Android - Device App - Settings'. Activate the 'Blacklist device apps' checkbox and enter the package names of the apps you want to detect.

In the image shown, you can see two examples of package names:
1. "com.cheating.app": Some demo package name to show possible input values.
2. "catch_.me_.if_.you_.can_": The package name of the general cheating app "GameGuardian". 

So if a user runs your app, the detector will react to any installed app with the entered package names. In the image case, this would be "com.cheating.app" and "catch_.me_.if_.you_.can_".

**Detector**

| Detector - Android                 | Free | Pro |
| -----------------                  | ---- | --- |
| AndroidDeviceCheatingDetector      |      | +   |

Use the 'AndroidDeviceCheatingDetector' to detect unwanted installed apps. It monitors the 'AndroidInstalledApplicationMonitor' and subscribes to the installed apps found. The detector enables you to do a comparison with a number of unwanted apps and notifies the observers of the detection. This gives you the opportunity to react to those.

Add the 'AndroidDeviceCheatingDetector' to your 'AntiCheat-Monitor' and respond to detections in various ways (more details are provided within the detector's documentation).

## Step 10 - React To Detected Cheating

When everything is setup you sure want to react to detected cheating. To do so there are various ways.

**Punisher**

In general, any cheat detected is forwarded to the 'AntiCheat-Monitor', which calculates an overall threat level. Based on the threat level, you can apply punishments by using Punisher components added to a child GameObject of the 'AntiCheat-Monitor'. There are some built-in punishers that you can find here as prefabs:

**Inspector**

You can set a callback in the Unity Inspector view of the detector. This callback is invoken as soon as the specific cheating is detected.

**Code**

If you would like to write a custom listener to the detector, you can attach an observer:

```cs
// Get the detector. For example the 'MobileGenuineDetector'.
var detector = AntiCheatMonitor.Instance
      .GetDetector<MobileGenuineDetector>();

// Subscribe as observer and get notified on inconsistency.
detector.Subscribe(myObserver);
```

Also the detectors have a property called 'PossibleCheatingDetected' which is set to true (and stays true) once a cheating got detected.