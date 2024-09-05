// GUPS - AntiCheat - Core
using GUPS.AntiCheat.Core.Integrity;
using GUPS.AntiCheat.Core.Obfuscate;
using GUPS.AntiCheat.Core.Watch;

namespace GUPS.AntiCheat.Core.Protected
{
    /// <summary>
    /// Represents an interface that combines features for obfuscation, data integrity, and subject monitoring.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="IProtected"/> interface provides a unified contract for classes that integrate obfuscation, data integrity verification, and 
    /// subject monitoring. Implementing classes are expected to provide mechanisms for obfuscating their content, ensuring data integrity.
    /// </para>
    /// 
    /// <para>
    /// Implementing classes are expected to inherit features from the following interfaces:
    /// - <see cref="IObfuscated"/>: Allows the class to apply obfuscation to its content.
    /// - <see cref="IDataIntegrity"/>: Provides mechanisms for ensuring and verifying data integrity.
    /// - <see cref="IWatchedSubject"/>: Enables the class to be observed and monitored by external entities.
    /// </para>
    /// </remarks>
    public interface IProtected : IObfuscated, IDataIntegrity, IWatchedSubject
    {
        // Members from IObfuscated interface:
        // - void Obfuscate()

        // Members from IDataIntegrity interface:
        // - bool HasIntegrity { get; }
        // - bool CheckIntegrity()
    }
}
