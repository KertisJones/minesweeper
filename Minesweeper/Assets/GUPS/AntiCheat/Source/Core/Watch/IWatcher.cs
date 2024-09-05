// System
using System;

namespace GUPS.AntiCheat.Core.Watch
{
    /// <summary>
    /// Represents an interface for entities that act as observers, receiving notifications about changes in subjects implementing the <see cref="IWatchedSubject"/> interface.
    /// </summary>
    /// <typeparam name="TWatchedSubject">The type of the subject being observed, which must implement the IWatchedSubject interface.</typeparam>
    /// <remarks>
    /// <para>
    /// The IWatcher interface is designed for entities that play the role of observers in the observer pattern.
    /// Observers implementing this interface can subscribe to and receive notifications about changes or events in subjects
    /// that adhere to the IWatchedSubject interface.
    /// </para>
    /// 
    /// <para>
    /// The generic type parameter TWatchedSubject represents the type of the subject being observed, and it must implement the IWatchedSubject interface.
    /// </para>
    /// 
    /// <para>
    /// This interface does not introduce any additional members beyond those inherited.
    /// </para>
    /// 
    /// <para>
    /// Members inherited from IObserver<TWatchedSubject>:
    /// - void OnCompleted(): Notifies the observer that the provider has finished sending push-based notifications.
    /// - void OnError(Exception error): Notifies the observer that the provider has experienced an error condition.
    /// - void OnNext(TWatchedSubject value): Provides the observer with new data.
    /// </para>
    /// 
    /// <para>
    /// Members inherited from IDisposable:
    /// - void Dispose(): Performs application-defined tasks associated with freeing, releasing, or resetting resources.
    /// </para>
    /// </remarks>
    public interface IWatcher<in TWatchedSubject> : IObserver<TWatchedSubject>, IDisposable
        where TWatchedSubject : IWatchedSubject
    {
        // This interface does not declare any additional members.
        // It inherits the members from IObserver<T> where T : IWatchedSubject.

        // Members from IObserver<T>:
        // - void OnCompleted()
        // - void OnError(Exception error)
        // - void OnNext(T value)

        // Members from IDisposable:
        // - void Dispose()
    }
}
