// GUPS - AntiCheat - Core
using GUPS.AntiCheat.Core.Watch;
using GUPS.AntiCheat.Core.Threat;

namespace GUPS.AntiCheat.Core.Detector
{
    /// <summary>
    /// Represents an interface for detectors that can watch for possible cheating and notify observers about detected threats.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="IDetector"/> interface defines the contract for objects that act as detectors to identify potential cheating activities.
    /// Detectors implementing this interface can watch for suspicious behavior and notify observers through the observer pattern when threats are detected.
    /// </para>
    /// 
    /// <para>
    /// The interface extends three other interfaces, providing additional functionality and specifications:
    /// - <see cref="IWatcher{IWatchedSubject}"/>: Allows the detector to observe and notify observers about changes in watched subjects.
    /// - <see cref="IWatchAble{IDetectorStatus}"/>: Enables the detector to be observed for changes in its status, providing information about its current state.
    /// - <see cref="IThreatRated"/>: Specifies the capability to rate the severity of detected threats.
    /// </para>
    /// </remarks>
    public interface IDetector : IWatcher<IWatchedSubject>, IWatchAble<IDetectorStatus>, IThreatRated
    {
        /// <summary>
        /// The name of the detector.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a value indicating whether the detector is supported on the current platform.
        /// </summary>
        bool IsSupported { get; }

        /// <summary>
        /// Gets a value indicating whether the detector is active and watching for possible cheating.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Gets a value indicating whether the detector has detected a possible cheating activity.
        /// </summary>
        bool PossibleCheatingDetected { get; }
    }
}
