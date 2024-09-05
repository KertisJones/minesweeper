# Version History:

## 2024.3.1: Protected Primitives - Fix
- Fix: Protected Primitives without any assigned values, threw a detection event on access (like .ToString()).

## 2024.3: Android Security Update
After two months of development, it's finally here: the Android Security update! Packed with numerous security features related to Android apps, it ensures your mobile gaming experience is safer than ever before.

Frequently mobile apps become targets for unauthorized redistribution, whether by disabling Digital Rights Management (DRM), modifying the app, bypassing payment methods, or rebranding it under a different name. This is a major problem for developers. That's why AntiCheat introduces new features that secure your mobile Android apps!

Android:
- Feature (Pro): Validate Installation Source - Validate the installation source, to check whether your app was installed by an official app stores and not by third parties.
- Feature (Pro): Validate Hash - Validate the entire app hash to determine whether the app has been modified in any way. Be it a different package name or changed code or other resources. 
- Feature (Pro): Validate Certificate Fingerprint - Validate your apps certificate fingerprint to make sure the app is shipped by you and no one else.
- Feature (Pro): Validate Libraries - A common cheat method in Unity Android apps is to insert custom libraries into your app instead of modifying the existing code. Validate against whitelisted and blacklisted libraries.
- Feature (Pro): Validate Installed Apps - Not only can a user modify or manipulate your game or app, but they can also try to gain an advantage by making changes to their device. Validate the installed apps!

iOS + Android:
- Feature: Validate Package Name - Validate the package name of the shipped app and make sure it is your app and not a rebranding.

## 2024.2.2: Protected Player Prefs - Fix
- Fix: Protected Player Prefs had an issue throwing detected cheating when reading from Protected Player Prefs.

## 2024.2.1: Support of Unity 2019 & 2020
- Support: Supports now Unity 2019 and 2020.
- Feature (Pro): Blockchain: A blockchain class has been introduced that inherits from a datachain that can be synchronized with a remote source.

## 2024.2: Datachain & Collection - Update
- QoL: Unity events can now also be attached to detectors via the inspector in order to react to cheating events.
- Feature (Pro): ProtectedList - A protected list is similar to the normal generic list you would use, with the special feature that its integrity is checked.
- Feature (Pro): ProtectedQueue - A protected queue is similar to the normal generic queue you would use, with the special feature that its integrity is checked.
- Feature (Pro): ProtectedStack - A protected stack is similar to the normal generic stack you would use, with the special feature that its integrity is checked.
- Feature (Pro): Datachain - A datachain is similar to a linked list consisting of a sequence of elements arranged in a specific order. It is used to maintain the order of these elements while keeping its integrity.

## 2024.1: Official release of AntiCheat 2024
- Refactored the code base to tackle the security issues in gaming 2024!
