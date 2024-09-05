// System
using System;
using System.Collections.Generic;

// Unity
using UnityEngine;

// GUPS - AntiCheat - Core
using GUPS.AntiCheat.Core.Monitor;
using GUPS.AntiCheat.Core.Watch;

namespace GUPS.AntiCheat.Monitor
{
    /// <summary>
    /// An abstract base class that defines the common functionality for monitors in a Unity environment. 
    /// Implements the <see cref="IMonitor"/> and <see cref="IWatchAble{IWatchedSubject}"/> interfaces.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="AMonitor"/> class serves as an abstract base for implementing monitors in Unity applications. 
    /// It provides methods to control the lifecycle of a monitoring process and supports the observer pattern 
    /// through the <see cref="IWatchAble{IWatchedSubject}"/> interface. Monitors can be started, paused, resumed, and stopped, 
    /// and they notify subscribed observers of relevant events during their lifecycle.
    /// </para>
    /// </remarks>
    public abstract class AMonitor : MonoBehaviour, IMonitor, IWatchAble<IWatchedSubject>
    {
        // Name
        #region Name

        /// <summary>
        /// The name of the monitor.
        /// </summary>
        public abstract String Name { get; }

        #endregion

        // Lifecycle
        #region Lifecycle

        /// <summary>
        /// A value indicating whether the monitor is active and should be running (Default: true).
        /// </summary>
        [SerializeField]
        [Header("General - Settings")]
        [Tooltip("A value indicating whether the monitor is active and should be running (Default: true).")]
        private bool isActive = true;

        /// <summary>
        /// A value indicating whether the monitor is active and should be running.
        /// </summary>
        public bool IsActive => this.isActive;

        /// <summary>
        /// Gets a value indicating whether the monitor has been started and is currently running.
        /// </summary>
        public bool IsStarted { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the monitor has been paused and is currently suspended.
        /// </summary>
        public bool IsPaused { get; private set; }

        /// <summary>
        /// Initiates the monitoring process, enabling the monitor to start collecting and processing data.
        /// </summary>
        public void Start()
        {
            // Check if the monitor is already started, if so return.
            if (this.IsStarted)
            {
                return;
            }

            // Set the monitor as started.
            this.IsStarted = true;

            // Notify.
            this.OnStart();
        }

        /// <summary>
        /// Executed when the monitor is started. Override this method to provide custom start behavior.
        /// </summary>
        protected virtual void OnStart()
        {

        }

        /// <summary>
        /// Pauses the monitoring process, temporarily suspending data collection and processing without terminating the monitor.
        /// </summary>
        public void Pause()
        {
            // Check if the monitor is paused, if so return.
            if (this.IsPaused)
            {
                return;
            }

            // Check if the monitor not started, if so return.
            if (!this.IsStarted)
            {
                return;
            }

            // Set the monitor as paused.
            this.IsPaused = true;

            // Notify.
            this.OnPause();
        }

        /// <summary>
        /// Executed when the monitor is paused. Override this method to provide custom pause behavior.
        /// </summary>
        protected virtual void OnPause()
        {

        }

        /// <summary>
        /// Resumes the monitoring process after a pause, allowing the monitor to continue collecting and processing data.
        /// </summary>
        public void Resume()
        {
            // Check if the monitor is not paused, if so return.
            if (!this.IsPaused)
            {
                return;
            }

            // Check if the monitor not started, if so return.
            if (!this.IsStarted)
            {
                return;
            }

            // Set the monitor as not paused.
            this.IsPaused = false;

            // Notify.
            this.OnResume();
        }

        /// <summary>
        /// Executed when the monitor is resumed after a pause. Override this method to provide custom resume behavior.
        /// </summary>
        protected virtual void OnResume()
        {

        }

        /// <summary>
        /// Stops and terminates the monitoring process, concluding data collection and finalizing any necessary cleanup operations.
        /// </summary>
        public void Stop()
        {
            // Check if the monitor is not started, if so return.
            if (!this.IsStarted)
            {
                return;
            }

            // Set the monitor as not started.
            this.IsStarted = false;

            // Notify.
            this.OnStop();
        }

        /// <summary>
        /// Executed when the monitor is stopped. Override this method to provide custom stop behavior.
        /// </summary>
        protected virtual void OnStop()
        {

        }

        /// <summary>
        /// Executed on each Unity update cycle when the monitor is started and not paused.
        /// </summary>
        private void Update()
        {
            // Check if the monitor is started and not paused.
            if (!this.IsStarted || this.IsPaused)
            {
                return;
            }

            // Check if the monitor is active.
            if (!this.isActive)
            {
                return;
            }

            // Notify.
            this.OnUpdate();
        }

        /// <summary>
        /// Executed on each Unity update cycle when the monitor is started and not paused.
        /// Override this method to implement custom update behavior.
        /// </summary>
        protected virtual void OnUpdate()
        {

        }

        /// <summary>
        /// When the monitor is destroyed, stop the monitor.
        /// </summary>
        protected virtual void OnDestroy()
        {
            // Stop the monitor.
            this.Stop();
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

        // Observable
        #region Observable

        /// <summary>
        /// The list of observers subscribed to the monitor.
        /// </summary>
        private List<IObserver<IWatchedSubject>> observers = new List<IObserver<IWatchedSubject>>();

        /// <summary>
        /// Subscribes an observer to receive notifications of the monitor.
        /// </summary>
        /// <param name="_Observer">The observer to subscribe.</param>
        /// <returns>An <see cref="IDisposable"/> object that can be used to unsubscribe the observer.</returns>
        public IDisposable Subscribe(IObserver<IWatchedSubject> _Observer)
        {
            if (!this.observers.Contains(_Observer))
            {
                this.observers.Add(_Observer);
            }

            return new Unsubscriber(this.observers, _Observer);
        }

        /// <summary>
        /// Notifies all subscribed observers with the provided watched monitor subject.
        /// </summary>
        /// <param name="_Subject">The watched subject to notify observers about.</param>
        public void Notify(IWatchedSubject _Subject)
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
            private List<IObserver<IWatchedSubject>> observers;
            private IObserver<IWatchedSubject> observer;

            /// <summary>
            /// Initializes a new instance of the <see cref="Unsubscriber"/> class.
            /// </summary>
            /// <param name="observers">The list of observers to manage.</param>
            /// <param name="observer">The observer to unsubscribe.</param>
            public Unsubscriber(List<IObserver<IWatchedSubject>> observers, IObserver<IWatchedSubject> observer)
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

        #endregion
    }
}
