// System
using System;

namespace GUPS.AntiCheat.Core.Threat
{
    /// <summary>
    /// Represents an interface for classes that assess and convey threat levels by providing a threat rating.
    /// </summary>
    public interface IThreatRated
    {
        /// <summary>
        /// Gets the threat rating associated with the implementing class, indicating the assessed level of potential threat.
        /// </summary>
        /// <remarks>
        /// The threat rating is represented as a positive 32-bit integer (UInt32), where higher values denote greater perceived threats.
        /// </remarks>
        UInt32 ThreatRating { get; }
    }
}
