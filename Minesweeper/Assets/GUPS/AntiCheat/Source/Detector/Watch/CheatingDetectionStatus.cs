// GUPS - AntiCheat - Core
using GUPS.AntiCheat.Core.Detector;

namespace GUPS.AntiCheat.Detector
{
    /// <summary>
    /// Represents a default implementation of the <see cref="IDetectorStatus"/> interface, providing a possibility of false positive and threat rating.
    /// </summary>
    public struct CheatingDetectionStatus : IDetectorStatus
    {
        /// <summary>
         /// Gets a value indicating the possibility of a false positive when assessing threats for the implementing subject from 0.0 to 1.0.
         /// </summary>
         /// <remarks>
         /// The value is represented as a positive float value ranging from 0.0 to 1.0, where 0.0 indicates no possibility of a false positive,
         /// and 1.0 denotes a 100% possibility of a false positive.
         /// </remarks>
        public float PossibilityOfFalsePositive { get; private set; }

        /// <summary>
        /// Gets the threat rating associated with the detected cheating, indicating the assessed level of a potential threat.
        /// </summary>
        /// <remarks>
        /// The threat rating is represented as a positive 32-bit integer (UInt32), where higher values denote greater perceived threats.
        /// </remarks>
        public uint ThreatRating { get; private set; }

        /// <summary>
        /// Creates a new instance of the <see cref="CheatingDetectionStatus"/> struct with the specified possibility of false positive and threat rating.
        /// </summary>
        /// <param name="_PossibilityOfFalsePositive">The possibility of a false positive ranging from 0.0 to 1.0.</param>
        /// <param name="_ThreatRating">The threat rating.</param>
        public CheatingDetectionStatus(float _PossibilityOfFalsePositive, uint _ThreatRating)
        {
            this.PossibilityOfFalsePositive = _PossibilityOfFalsePositive;
            this.ThreatRating = _ThreatRating;
        }
    }
}
