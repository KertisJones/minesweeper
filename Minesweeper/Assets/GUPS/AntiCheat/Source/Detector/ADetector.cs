// System
using System;
using System.Collections.Generic;

// Unity
using UnityEngine;

// GUPS - AntiCheat - Core
using GUPS.AntiCheat.Core.Detector;
using GUPS.AntiCheat.Core.Watch;

namespace GUPS.AntiCheat.Detector
{
    /// <summary>
    /// Abstract base class for detectors that can watch for possible cheating and notify observers about detected threats.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <para>
    /// The <see cref="ADetector"/> class acts as bsae class for detectors to identify potential cheating activities.
    /// Inherite from this class to watch for suspicious behavior and notify observers through the observer pattern when threats are detected.
    /// </para>
    /// 
    /// <para>
    /// The interface extends once interface, providing additional functionality and specifications:
    /// - <see cref="IDetector"/>: Allows the detector to observe and notify observers about possible detected threats.
    /// </para>
    /// </remarks>
    public abstract class ADetector : MonoBehaviour, IDetector
    {
        // Name
        #region Name

        /// <summary>
        /// The name of the detector.
        /// </summary>
        public abstract String Name { get; }

        #endregion

        // Platform
        #region Platform

        /// <summary>
        /// Gets whether the detector is supported on the current platform.
        /// </summary>
        public abstract bool IsSupported { get; }

        /// <summary>
        /// Gets or sets whether the detector is active and watching for possible cheating (Default: true).
        /// </summary>
        [SerializeField]
        [Header("General - Settings")]
        [Tooltip("Gets or sets whether the detector is active and watching for possible cheating (Default: true).")]
        private bool isActive = true;

        /// <summary>
        /// Gets or sets whether the detector is active and watching for possible cheating (Default: true).
        /// </summary>
        public bool IsActive { get => this.isActive; set => this.isActive = value; }

        #endregion

        // Threat Rating
        #region Threat Rating

        /// <summary>
        /// Gets the threat rating of the detector, indicating the assessed level of potential threat.
        /// </summary>
        /// <remarks>
        /// A higher values denote greater perceived threats.
        /// </remarks>
        public abstract uint ThreatRating { get; protected set; }

        /// <summary>
        /// Get whether the detector has detected a possible cheating activity.
        /// </summary>
        public abstract bool PossibleCheatingDetected { get; protected set; }

        #endregion

        // Observable
        #region Observable

        /// <summary>
        /// The list of observers subscribed to the monitor.
        /// </summary>
        private List<IObserver<IDetectorStatus>> observers = new List<IObserver<IDetectorStatus>>();

        /// <summary>
        /// Subscribes an observer to receive notifications of the monitor.
        /// </summary>
        /// <param name="_Observer">The observer to subscribe.</param>
        /// <returns>An <see cref="IDisposable"/> object that can be used to unsubscribe the observer.</returns>
        public IDisposable Subscribe(IObserver<IDetectorStatus> _Observer)
        {
            if (!this.observers.Contains(_Observer))
            {
                this.observers.Add(_Observer);
            }

            return new Unsubscriber(this.observers, _Observer);
        }

        /// <summary>
        /// Notifies the observers of the monitor of the detected status and possible cheating.
        /// </summary>
        /// <param name="_Subject"></param>
        public void Notify(IDetectorStatus _Subject)
        {
            foreach (var var_Observer in this.observers)
            {
                if (var_Observer == null)
                {
                    continue;
                }

                var_Observer.OnNext(_Subject);
            }
        }

        /// <summary>
        /// Represents a disposable object used to unsubscribe an observer from the monitor.
        /// </summary>
        private class Unsubscriber : IDisposable
        {
            private List<IObserver<IDetectorStatus>> observers;
            private IObserver<IDetectorStatus> observer;

            /// <summary>
            /// Initializes a new instance of the <see cref="Unsubscriber"/> class.
            /// </summary>
            /// <param name="observers">The list of observers to manage.</param>
            /// <param name="observer">The observer to unsubscribe.</param>
            public Unsubscriber(List<IObserver<IDetectorStatus>> observers, IObserver<IDetectorStatus> observer)
            {
                this.observers = observers;
                this.observer = observer;
            }

            /// <summary>
            /// Disposes of the observer, removing it from the list of observers.
            /// </summary>
            public void Dispose()
            {
                if (this.observer != null && this.observers.Contains(this.observer))
                {
                    this.observers.Remove(this.observer);
                }
            }
        }

        /// <summary>
        /// Notifies the observers that the monitor has completed.
        /// </summary>
        public void Dispose()
        {
            foreach (var var_Observer in this.observers)
            {
                if (var_Observer == null)
                {
                    continue;
                }

                var_Observer.OnCompleted();
            }

            this.observers.Clear();
        }

        #endregion

        // Observer
        #region Observer

        /// <summary>
        /// Called when the observed monitor has completed.
        /// </summary>
        public abstract void OnCompleted();

        /// <summary>
        /// Called when an error occurs in the observed monitor.
        /// </summary>
        /// <param name="_Error">The error that occurred.</param>
        public abstract void OnError(Exception _Error);

        /// <summary>
        /// Called when the observed monitor has a new status.
        /// </summary>
        /// <param name="_Subject">The new status of the monitor.</param>
        public abstract void OnNext(IWatchedSubject _Subject);

        #endregion

        // Lifecycle
        #region Lifecycle

        /// <summary>
        /// On awake, check if the detector is supported and disable it if it is not.
        /// </summary>
        protected virtual void Awake()
        {
            // If the detector is not supported, disable it.
            if (!this.IsSupported)
            {
                // Disable the detector.
                this.isActive = false;
                this.enabled = false;
            }
        }

        #endregion
    }
}
