// System
using System;
using System.Collections.Generic;
using System.Linq;

// Unity
using UnityEngine;

// GUPS - AnitCheat - Core
using GUPS.AntiCheat.Core.Detector;
using GUPS.AntiCheat.Core.Punisher;
using GUPS.AntiCheat.Core.Watch;

// GUPS - AnitCheat
using GUPS.AntiCheat.Singleton;

namespace GUPS.AntiCheat
{
    /// <summary>
    /// The heart of the anti cheat system. The monitor watches all detectors and punishes cheaters. It is a persistent singleton, once created it will exist throughout 
    /// the whole application.
    /// </summary>
    /// <remarks>
    /// On awake, the monitor registers all detectors and punishers in the children gameobjects of the monitor. It then subscribes to all detectors and watches for threats. 
    /// If a threat is detected, the monitor will punish the cheater. The monitor also slowly reduces the threat level over time. The monitor is a thread-safe singleton and 
    /// is persistent throughout the whole application.
    /// </remarks>
    public class AntiCheatMonitor : Singleton<AntiCheatMonitor>, IWatcher<IDetectorStatus>
    {
        // Singleton
        #region Singleton

        /// <summary>
        /// The persistent singleton instance of the anti cheat monitor.
        /// </summary>
        public override bool IsPersistent => true;

        #endregion

        // Threat Rating
        #region Threat Rating

        /// <summary>
        /// The sensitive level of the monitor. Manage the reaction sensitivity of the monitor to possible detected threats. Ratings of detected threats are scaled by the 
        /// sensitive level beginning at 0 (NOT_SENSITIVE) up to 4 (VERY_SENSITIVE). The higher the sensitive level, the earlier the monitor will react and punish the cheater.
        /// </summary>
        [SerializeField]
        [Tooltip("The sensitive level of the monitor. Manage the reaction sensitivity of the monitor to possible detected threats. Ratings of detected threats are scaled by the sensitive level beginning at 0 (NOT_SENSITIVE) up to 4 (VERY_SENSITIVE). The higher the sensitive level, the earlier the monitor will react and punish the cheater.")]
        private ESensitiveLevel sensitiveLevel = ESensitiveLevel.MODERATE;

        /// <summary>
        /// The sensitive level of the monitor. Manage the reaction sensitivity of the monitor to possible detected threats. Ratings of detected threats are scaled by the 
        /// sensitive level beginning at 0 (NOT_SENSITIVE) up to 4 (VERY_SENSITIVE). The higher the sensitive level, the earlier the monitor will react and punish the cheater.
        /// </summary>
        public ESensitiveLevel SensitiveLevel { get => this.sensitiveLevel; }

        /// <summary>
        /// The current threat level of the monitor.
        /// </summary>
        private float threatLevel = 0.0f;

        /// <summary>
        /// The current threat level of the monitor.
        /// </summary>
        public float ThreatLevel => this.threatLevel;

        #endregion

        // Detector
        #region Detector

        /// <summary>
        /// A list of all detectors that the monitor is subscribed to.
        /// </summary>
        private List<IDetector> detectors = new List<IDetector>();

        /// <summary>
        /// Register a detector to the monitor.
        /// </summary>
        /// <param name="_Detector">The detector to register.</param>
        private void RegisterDetector(IDetector _Detector)
        {
            // Add the detector if it is not already in the list.
            if (!this.detectors.Contains(_Detector))
            {
                // Add the detector to the list.
                this.detectors.Add(_Detector);

                // Subscribe the monitor to the detector.
                _Detector.Subscribe(this);
            }
        }

        /// <summary>
        /// Get the first detector of the specified type.
        /// </summary>
        /// <typeparam name="TDetector">The type of the detector to get.</typeparam>
        /// <returns>The first detector of the specified type.</returns>
        public TDetector GetDetector<TDetector>() 
            where TDetector : IDetector
        {
            // Find the first detector that is of the specified type.
            foreach (var var_Detector in this.detectors)
            {
                if (var_Detector is TDetector var_TypedDetector)
                {
                    return var_TypedDetector;
                }
            }

            return default;
        }

        /// <summary>
        /// Remove a detector from the monitor.
        /// </summary>
        /// <param name="_Detector">The detector to remove.</param>
        private void RemoveDetector(IDetector _Detector)
        {
            // Remove the detector if it is in the list.
            if (this.detectors.Contains(_Detector))
            {
                this.detectors.Remove(_Detector);
            }
        }

        /// <summary>
        /// The monitor is notified of a threat by a detector.
        /// </summary>
        /// <param name="value">The detected threat status.</param>
        public void OnNext(IDetectorStatus value)
        {
            // Add the threat rating to the threat level scaled by the sensitive level and change of false positives.
            this.threatLevel += (1f - value.PossibilityOfFalsePositive) * value.ThreatRating * (int)this.SensitiveLevel;

            // Get a list of punisher for the threat level (ascending).
            var var_Punisher = this.GetPunisher((UInt32)this.threatLevel);

            // Iterate through the list of punishers and punish them.
            var_Punisher.ForEach(p => p.Punish());
        }

        /// <summary>
        /// The monitor is notified of an error by a detector.
        /// </summary>
        /// <param name="_Error">The error that occurred.</param>
        public void OnError(Exception _Error)
        {
            // Do nothing.
        }

        /// <summary>
        /// The monitor is notified of the completion of a detector.
        /// </summary>
        public void OnCompleted()
        {
            // Do nothing.
        }

        /// <summary>
        /// On dispose, do nothing.
        /// </summary>
        public void Dispose()
        {
            // Do nothing.
        }

        #endregion

        // Punisher
        #region Punisher

        /// <summary>
        /// The list of all punishers that the monitor can use.
        /// </summary>
        private List<IPunisher> punishers = new List<IPunisher>();

        /// <summary>
        /// Register a punisher to the monitor.
        /// </summary>
        /// <param name="_Punisher">The punisher to register.</param>
        private void RegisterPunisher(IPunisher _Punisher)
        {
            // Add the punisher if it is not already in the list.
            if (!this.punishers.Contains(_Punisher))
            {
                // Add the punisher to the list.
                this.punishers.Add(_Punisher);
            }
        }

        /// <summary>
        /// Returns a list of all punishers that have a threat rating lower than the threat level and can punish multiple times or have not punished yet. 
        /// </summary>
        /// <param name="_ThreatLevel">The current threat level.</param>
        /// <returns>A list of punishable punishers, sorted by threat rating ascending.</returns>
        public List<IPunisher> GetPunisher(UInt32 _ThreatLevel)
        {
            // A result list of all active punishers that have a threat rating lower than the passed threat level sorted by threat rating ascending (is done on add).
            List<IPunisher> var_Punisher = this.punishers.Where(p => p.IsActive && p.ThreatRating <= _ThreatLevel && (!p.PunishOnce || p.PunishOnce && !p.HasPunished)).ToList();

            // Return the result list.
            return var_Punisher;
        }

        /// <summary>
        /// Remove a punisher from the monitor.
        /// </summary>
        /// <param name="_Punisher">The punisher to remove.</param>
        private void RemovePunisher(IPunisher _Punisher)
        {
            // Remove the punisher if it is in the list.
            if (this.punishers.Contains(_Punisher))
            {
                this.punishers.Remove(_Punisher);
            }
        }

        #endregion

        // Lifecycle
        #region Lifecycle

        /// <summary>
        /// On start of the monitor, register all detectors and punishers in the children of the monitor.
        /// </summary>
        protected override void Awake()
        {
            // Call the base method.
            base.Awake();

            // If the Behaviour or GameObject was destroyed, because of duplicate singletons, return.
            if(this == null || this.gameObject == null)
            {
                return;
            }

            // Find all detectors in the children of the monitor.
            foreach (var var_Detector in this.GetComponentsInChildren<IDetector>())
            {
                // Check if the detector is supported.
                if (var_Detector.IsSupported)
                {
                    // Then register the detector.
                    this.RegisterDetector(var_Detector);
                }
            }

            // Find all punishers in the children of the monitor.
            foreach (var var_Punisher in this.GetComponentsInChildren<IPunisher>())
            {
                // Check if the punisher is supported.
                if (var_Punisher.IsSupported)
                {
                    // Then register the punisher.
                    this.RegisterPunisher(var_Punisher);
                }
            }

            // Sort the punishers by their threat rating ascending.
            this.punishers.Sort((a, b) => a.ThreatRating.CompareTo(b.ThreatRating));
        }

        /// <summary>
        /// On the fixed update of the monitor, reduce the threat level over time.
        /// </summary>
        protected virtual void FixedUpdate()
        {
            // Reduce the threat level over time by a factor based on the sensitive level.
            float var_Reduction = UnityEngine.Time.fixedUnscaledDeltaTime / 5.5f * ((int)ESensitiveLevel.VERY_SENSITIVE + 1 - (int)this.SensitiveLevel);

            // Reduce the threat level.
            this.threatLevel = Mathf.Max(0, this.threatLevel - var_Reduction);
        }

        #endregion
    }
}
