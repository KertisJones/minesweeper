// GUPS - AntiCheat - Core
using GUPS.AntiCheat.Core.Watch;

namespace GUPS.AntiCheat.Core.HoneyPot
{
    /// <summary>
    /// Represents an interface for classes implementing a honey pot mechanism to detect and counteract unauthorized activities.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="IHoneyPot"/> interface outlines the contract for classes that implement a honey pot, which is a deceptive element used to identify 
    /// and counteract unauthorized or malicious actions.
    /// </para>
    /// 
    /// <para>
    /// The interface extends <see cref="IWatchAble{IWatchedSubject}"/>, allowing honey pot implementations to be observed and monitored for changes by external entities.
    /// </para>
    /// </remarks>
    public interface IHoneyPot : IWatchAble<IWatchedSubject>
    {
        /// <summary>
        /// Places a honey pot, a deceptive element designed to attract and identify unauthorized or malicious actions.
        /// </summary>
        void PlaceHoneyPot();
    }
}
