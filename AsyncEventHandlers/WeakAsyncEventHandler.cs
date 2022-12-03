using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncEventHandlers;

/// <summary>
/// A thread-safe, weak and asynchronus event handler.
/// </summary>
public struct WeakAsyncEventHandler
{
    /// <summary>
    /// All registered events.
    /// </summary>
    public ConditionalWeakTable<object, AsyncEvent> Callbacks { get; private set; } = new ConditionalWeakTable<object, AsyncEvent>();

    public WeakAsyncEventHandler()
    {
    }

    /// <summary>
    /// Subscribe to the event.
    /// </summary>
    /// <param name="instance">The source of the event.</param>
    /// <param name="callback">The action which should get executed when the event fires.</param>
    public void Register(object instance, AsyncEvent callback)
    {
        if (Callbacks is null)
            Callbacks = new ConditionalWeakTable<object, AsyncEvent>();

        Callbacks.AddOrUpdate(instance, callback);
    }

    /// <summary>
    /// Unsubscribe from the event.
    /// </summary>
    /// <param name="instance">The instance which contains the events shouldn't get executed anymore when the event fires.</param>
    public bool Unregister(object instance)
    {
        return Callbacks.Remove(instance);
    }

    /// <summary>
    /// Invokes all registered events.
    /// <para/>
    /// Throws <see cref="OperationCanceledException"/> if the supplied <paramref name="cancellationToken"/> was cancelled or
    /// <see cref="ObjectDisposedException"/> if the supplied <paramref name="cancellationToken"/> was disposed or
    /// <see cref="Exception"/> if any of the registered event handlers threw an exception.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to communicate cancellation of the async operation.</param>
    /// <exception cref="OperationCanceledException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="Exception"></exception>
    /// <returns>A <see cref="Task"/> which represents the completion of all registered events.</returns>
    public Task InvokeAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        HashSet<Task> tasks = new HashSet<Task>();
        foreach ((var instance, var callback) in Callbacks)
        {
            cancellationToken.ThrowIfCancellationRequested();
            tasks.Add(callback.Invoke(cancellationToken));
        }

        return Task.WhenAll(tasks);
    }
}

/// <summary>
/// A thread-safe, weak and asynchronus event handler.
/// </summary>
/// <typeparam name="TEventData">The generic type which holds the event data.</typeparam>
public struct WeakAsyncEventHandler<TEventData>
{
    /// <summary>
    /// All registered events.
    /// </summary>
    public ConditionalWeakTable<object, AsyncEvent<TEventData>> Callbacks { get; private set; } = new ConditionalWeakTable<object, AsyncEvent<TEventData>>();

    public WeakAsyncEventHandler()
    {
    }

    /// <summary>
    /// Subscribe to the event.
    /// </summary>
    /// <param name="instance">The source of the event.</param>
    /// <param name="callback">The action which should get executed when the event fires.</param>
    public void Register(object instance, AsyncEvent<TEventData> callback)
    {
        if (Callbacks is null)
            Callbacks = new ConditionalWeakTable<object, AsyncEvent<TEventData>>();

        Callbacks.AddOrUpdate(instance, callback);
    }

    /// <summary>
    /// Unsubscribe from the event.
    /// </summary>
    /// <param name="instance">The instance which contains the events shouldn't get executed anymore when the event fires.</param>
    public bool Unregister(object instance)
    {
        return Callbacks.Remove(instance);
    }

    /// <summary>
    /// Invokes all registered events.
    /// <para/>
    /// Throws <see cref="OperationCanceledException"/> if the supplied <paramref name="cancellationToken"/> was cancelled or
    /// <see cref="ObjectDisposedException"/> if the supplied <paramref name="cancellationToken"/> was disposed or
    /// <see cref="Exception"/> if any of the registered event handlers threw an exception.
    /// </summary>
    /// <param name="data">An object that contains the event data.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to communicate cancellation of the async operation.</param>
    /// <exception cref="OperationCanceledException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="Exception"></exception>
    /// <returns>A <see cref="Task"/> which represents the completion of all registered events.</returns>
    public Task InvokeAsync(TEventData data, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        HashSet<Task> tasks = new HashSet<Task>();
        foreach ((var instance, var callback) in Callbacks)
        {
            cancellationToken.ThrowIfCancellationRequested();
            tasks.Add(callback.Invoke(data, cancellationToken));
        }

        return Task.WhenAll(tasks);
    }
}
