namespace GUPS.AntiCheat.Core.Monitor
{
    /// <summary>
    /// Defines an interface for implementing a monitor, which provides methods to control the lifecycle of a monitoring process.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The IMonitor interface outlines the contract for objects that serve as monitors, responsible for managing the lifecycle
    /// of a monitoring process. Monitors are used to collect and process data related to a specific aspect of the operating system 
    /// or the game / application.
    /// </para>
    /// </remarks>
    public interface IMonitor
    {
        /// <summary>
        /// The name of the monitor.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a value indicating whether the monitor has been started and is currently running.
        /// </summary>
        bool IsStarted { get; }

        /// <summary>
        /// Gets a value indicating whether the monitor has been paused and is currently suspended.
        /// </summary>
        bool IsPaused { get; }

        /// <summary>
        /// Initiates the monitoring process, enabling the monitor to start collecting and processing data.
        /// </summary>
        void Start();

        /// <summary>
        /// Pauses the monitoring process, temporarily suspending data collection and processing without terminating the monitor.
        /// </summary>
        void Pause();

        /// <summary>
        /// Resumes the monitoring process after a pause, allowing the monitor to continue collecting and processing data.
        /// </summary>
        void Resume();

        /// <summary>
        /// Stops and terminates the monitoring process, concluding data collection and finalizing any necessary cleanup operations.
        /// </summary>
        void Stop();
    }
}
