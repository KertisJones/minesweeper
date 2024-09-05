// System
using System;

namespace GUPS.AntiCheat.Monitor.Android
{
    /// <summary>
    /// Enum representing various Android app stores.
    /// </summary>
    [Serializable]
    public enum EAppStore : byte
    {
        /// <summary>
        /// Unknown. The default value.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Package Installer. The installation of apps outside of stores is done by a system app that is integrated into every Android device. 
        /// This system app, known as the package installer, is responsible for installing applications that originate from apk files downloaded 
        /// from various locations.
        /// </summary>
        AndroidPackageInstaller = 1,

        /// <summary>
        /// Amazon Appstore. Amazon's digital application distribution platform.
        /// </summary>
        AmazonAppstore = 2,

        /// <summary>
        /// Aptoide. An open-source independent Android app store.
        /// </summary>
        Aptoide = 3,

        /// <summary>
        /// Cafe Bazaar. An Iranian Android marketplace.
        /// </summary>
        CafeBazaar = 4,

        /// <summary>
        /// F-Droid. An open-source software repository for Android.
        /// </summary>
        FDroid = 5,

        /// <summary>
        /// Google Play Store. Google's official app store.
        /// </summary>
        GooglePlayStore = 6,

        /// <summary>
        /// Huawei AppGallery. Huawei's official app distribution platform.
        /// </summary>
        HuaweiAppGallery = 7,

        /// <summary>
        /// Myket. A popular Android app store.
        /// </summary>
        Myket = 8,

        /// <summary>
        /// Oppo App Market. Oppo's official app store.
        /// </summary>
        OppoAppMarket = 9,

        /// <summary>
        /// Samsung Galaxy Store. Samsung's official app store.
        /// </summary>
        SamsungGalaxyStore = 10,

        /// <summary>
        /// TapTap. A Chinese app store for mobile games.
        /// </summary>
        TapTap = 11,

        /// <summary>
        /// Vivo App Store. Vivo's official app distribution platform.
        /// </summary>
        VivoAppStore = 12,

        /// <summary>
        /// Xiaomi Mi GetApps. Xiaomi's official app store.
        /// </summary>
        XiaomiMiGetApps = 13,

        /// <summary>
        /// XDA Labs. A platform for mobile development projects.
        /// </summary>
        XDALabs = 14,
    }
}
