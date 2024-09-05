// GUPS - AntiCheat - Core
using GUPS.AntiCheat.Core.Threat;
using GUPS.AntiCheat.Core.Watch;

namespace GUPS.AntiCheat.Core.Detector
{
    /// <summary>
    /// Represents an interface for detector subjects that are watched or monitored, enhancing threat assessment capabilities by incorporating a possibility of false positives.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="IDetectorStatus"/> interface defines the contract for objects that act as status subjects for detectors, providing information about their current state.
    /// Detectors observing objects implementing this interface gain access to threat assessment capabilities, including the possibility of false positives.
    /// </para>
    /// 
    /// <para>
    /// The interface extends two other interfaces, providing additional functionality and specifications:
    /// - <see cref="IWatchedSubject"/>: Allows the subject to be observed and monitored by detectors.
    /// - <see cref="IThreatRated"/>: Specifies the capability to rate the severity of threats.
    /// </para>
    /// </remarks>
    public interface IDetectorStatus : IWatchedSubject, IThreatRated
    {
        /// <summary>
        /// Gets a value indicating the possibility of a false positive when assessing threats for the implementing subject from 0.0 to 1.0.
        /// </summary>
        /// <remarks>
        /// The value is represented as a positive float value ranging from 0.0 to 1.0, where 0.0 indicates no possibility of a false positive,
        /// and 1.0 denotes a 100% possibility of a false positive.
        /// </remarks>
        float PossibilityOfFalsePositive { get; }
    }
}
