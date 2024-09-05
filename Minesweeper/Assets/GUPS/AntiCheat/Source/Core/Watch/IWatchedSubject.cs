namespace GUPS.AntiCheat.Core.Watch
{
    /// <summary>
    /// Represents an interface for subjects that are watched or monitored.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The IWatchedSubject interface serves as the foundation for implementing the subject side of the observer pattern.
    /// Classes that implement this interface act as subjects that can be watched or monitored by observers.
    /// </para>
    /// 
    /// <para>
    /// Observers can subscribe to receive notifications about changes or events in the watched subject.
    /// Implementing classes should provide mechanisms for observers to register, unregister, and notify them of relevant changes.
    /// </para>
    /// 
    /// <para>
    /// This interface itself does not contain any specific members but serves as a marker or contract for classes to adhere to
    /// when implementing the observer pattern.
    /// </para>
    /// </remarks>
    public interface IWatchedSubject
    {
        // This interface does not declare any members.
    }
}
