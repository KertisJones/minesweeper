// System
using System;

namespace GUPS.AntiCheat.Core.Watch
{
    /// <summary>
    /// Represents an interface for entities that can be watched or monitored, extending the capabilities of the observer pattern.
    /// This interface extends the standard observer pattern by incorporating disposable resources.
    /// </summary>
    /// <typeparam name="TWatchedSubject">The type of the subject being watched, which must implement the IWatchedSubject interface.</typeparam>
    /// <remarks>
    /// <para>
    /// This interface inherits from IObservable{TWatchedSubject} and IDisposable. It is designed for entities that can be observed or monitored.
    /// The generic type parameter TWatchedSubject represents the type of the subject being watched, and it must implement the IWatchedSubject interface.
    /// </para>
    /// 
    /// <para>
    /// The interface does not declare any additional members beyond those inherited.
    /// </para>
    /// 
    /// <para>
    /// Members inherited from IObservable{TWatchedSubject}:
    /// - IDisposable Subscribe(IObserver{TWatchedSubject} observer): Subscribes an observer to receive notifications.
    /// - IDisposable Subscribe(Action{TWatchedSubject} onNext, Action{Exception} onError, Action onCompleted): Subscribes actions to handle notifications.
    /// </para>
    /// 
    /// <para>
    /// Members inherited from IDisposable:
    /// - void Dispose(): Performs application-defined tasks associated with freeing, releasing, or resetting resources.
    /// </para>
    /// </remarks>
    public interface IWatchAble<out TWatchedSubject> : IObservable<TWatchedSubject>, IDisposable
        where TWatchedSubject : IWatchedSubject
    {
        // This interface does not declare any additional members.

        // Members from IObservable<T>:
        // - IDisposable Subscribe(IObserver<T> observer)
        // - IDisposable Subscribe(Action<T> onNext, Action<Exception> onError, Action onCompleted)

        // Members from IDisposable:
        // - void Dispose()
    }

}
