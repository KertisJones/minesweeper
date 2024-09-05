// System
using System;
using System.Collections;

// Unity
using UnityEngine;

// GUPS - AntiCheat - Core
using GUPS.AntiCheat.Core.Watch;

namespace GUPS.AntiCheat.Detector.Mobile
{
    /// <summary>
    /// This detector is used to detect if the build mobile app (Android and iOS) is genuine, i.e. if it is not modified or tampered with.
    /// </summary>
    public class MobileGenuineDetector : ADetector
    {
        // Name
        #region Name

        /// <summary>
        /// The name of the detector.
        /// </summary>
        public override String Name => "Mobile Genuine Check Detector";

        #endregion

        // Platform
        #region Platform

        /// <summary>
        /// Is supported only for Android and iOS platforms.
        /// </summary>
#if UNITY_ANDROID || UNITY_IOS
        public override bool IsSupported => true;
#else
        public override bool IsSupported => false;
#endif

        #endregion

        // Threat Rating
        #region Threat Rating

        /// <summary>
        /// The possibility of a false positive is very low.
        /// </summary>
        public float PossibilityOfFalsePositive => 0.01f;

        /// <summary>
        /// The threat rating of this detector. It is set to a very high value, because false positives are very unlikely and the impact of cheating is very high (Recommended: 500).
        /// </summary>
        [SerializeField]
        [Header("Threat Rating - Settings")]
        [Tooltip("The threat rating of this detector. It is set to a very high value, because false positives are very unlikely and the impact of cheating is very high (Recommended: 500).")]
        private uint threatRating = 500;

        /// <summary>
        /// The threat rating of this detector. It is set to a very high value, because false positives are very unlikely and the impact of cheating is very high (Recommended: 500).
        /// </summary>
        public override uint ThreatRating { get => this.threatRating; protected set => this.threatRating = value; }

        /// <summary>
        /// Stores whether a cheating got detected.
        /// </summary>
        public override bool PossibleCheatingDetected { get; protected set; } = false;

        #endregion

        // Observable
        #region Observable

        /// <summary>
        /// A unity event that is used to subscribe to the cheating detection events. It is useful if you do not want to write custom observers to subscribe to the detectors and 
        /// simply attach a callback to the detector event through the inspector.
        /// </summary>
        [Header("Observable - Settings")]
        [Tooltip("A unity event that is used to subscribe to the cheating detection events. It is useful if you do not want to write custom observers to subscribe to the detectors and simply attach a callback to the detector event through the inspector.")]
        public CheatingDetectionEvent<CheatingDetectionStatus> OnCheatingDetectionEvent = new CheatingDetectionEvent<CheatingDetectionStatus>();

        #endregion

        // Observer
        #region Observer

        /// <summary>
        /// Does not observe any subjects.
        /// </summary>
        /// <param name="_Subject"></param>
        public override void OnNext(IWatchedSubject _Subject)
        {
            // Does not observe any subjects.
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <param name="_Error">Error to handle.</param>
        public override void OnError(Exception _Error)
        {
            // Does nothing.
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        public override void OnCompleted()
        {
            // Does nothing.
        }

        #endregion

        // Genuine
        #region Genuine

        /// <summary>
        /// Enable to check if the application is genuine only on detector start. The genuine check can be resource intensive. Disable to check in a define interval.
        /// </summary>
        [Header("Genuine - Settings")]
        [Tooltip("Enable to check if the application is genuine only on detector start. The genuine check can be resource intensive. Disable to check in a define interval. Recommended: True")]
        public bool CheckGenuineOnlyOnGameStart = true;

        /// <summary>
        /// Interval in seconds in which to check the genuine of the application.
        /// </summary>
        [Tooltip("Interval in seconds in which to check the genuine of the application. Recommended: 60")]
        [Range(0.001f, 600f)]
        public float RecheckIntervalForPossibleCheating = 60f;

        /// <summary>
        /// Runs in the interval of the recheck field.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CheckGenuine()
        {
            while (true)
            {
                // Check genuine only if the detector is active.
                if (this.IsActive)
                {
                    // Check genuine if not already checked on game start.
                    if (!this.CheckGenuineOnlyOnGameStart)
                    {
                        if (!Application.genuine)
                        {
                            // Possible cheating detected.
                            this.PossibleCheatingDetected = true;

                            // Notify observers (mostly the AntiCheatMonitor) of the detected modification.
                            this.Notify(new CheatingDetectionStatus(this.PossibilityOfFalsePositive, this.ThreatRating));

                            // Notify event listeners of the detected modification.
                            this.OnCheatingDetectionEvent?.Invoke(new CheatingDetectionStatus(this.PossibilityOfFalsePositive, this.ThreatRating));
                        }
                    }
                }

                yield return new WaitForSecondsRealtime(this.RecheckIntervalForPossibleCheating);
            }
        }

        /// <summary>
        /// Manually check for genuine and return if tampering was detected.
        /// </summary>
        /// <returns></returns>
        public bool ManualGenuineCheck()
        {
            if (Application.genuineCheckAvailable)
            {
                if (!Application.genuine)
                {
                    // Possible cheating detected.
                    this.PossibleCheatingDetected = true;

                    // Notify observers (mostly the AntiCheatMonitor) of the detected modification.
                    this.Notify(new CheatingDetectionStatus(this.PossibilityOfFalsePositive, this.ThreatRating));

                    // Notify event listeners of the detected modification.
                    this.OnCheatingDetectionEvent?.Invoke(new CheatingDetectionStatus(this.PossibilityOfFalsePositive, this.ThreatRating));

                    return false;
                }
            }

            return true;
        }

        #endregion

        // Lifecycle
        #region Lifecycle

        /// <summary>
        /// Check the genuine of the application on start or check in a defined interval.
        /// </summary>
        protected virtual void Start()
        {
            // Check genuine only if the detector is active.
            if (this.IsActive)
            {
                // If active, manually check for genuine on game start once.
                if (this.CheckGenuineOnlyOnGameStart)
                {
                    this.ManualGenuineCheck();
                }
            }

            // Repeating genuine check - Start only when genuine check is available!
            if (Application.genuineCheckAvailable)
            {
                // Start checking coroutine.
                this.StartCoroutine(this.CheckGenuine());
            }
        }

        #endregion
    }
}