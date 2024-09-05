// GUPS - AntiCheat - Core
using GUPS.AntiCheat.Core.Threat;

namespace GUPS.AntiCheat.Core.Punisher
{
    /// <summary>
    /// Represents an interface for entities equipped to administer punitive actions in response to perceived threats, enhancing threat assessment capabilities.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="IPunisher"/> interface defines the contract for entities that possess the capability to administer punitive actions based on perceived threat levels.
    /// </para>
    /// 
    /// <para>
    /// The interface inherits the <see cref="IThreatRated"/> interface, providing access to the threat rating property to decide when to administer punitive actions.
    /// </para>
    /// </remarks>
    public interface IPunisher : IThreatRated
    {
        /// <summary>
        /// The name of the punisher.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Returns if the punisher is supported on the current platform.
        /// </summary>
        bool IsSupported { get; }

        /// <summary>
        /// Gets a value indicating whether the punisher is active and can administer punitive actions.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Returns if the punisher should only administer punitive actions once or any time the threat level exceeds the threat rating.
        /// </summary>
        bool PunishOnce { get; }

        /// <summary>
        /// Returns if the punisher has administered punitive actions.
        /// </summary>
        bool HasPunished { get; }

        /// <summary>
        /// Administers punitive actions based on the perceived threat level as indicated by the threat rating.
        /// </summary>
        void Punish();

        // Members from IThreatRated:
        // - UInt32 ThreatRating { get; }
    }
}
