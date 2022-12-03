using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncEventHandlers;

/// <summary>
/// A thread-safe asynchronus event handler.
/// </summary>
public struct AsyncEventHandler
{
    /// <summary>
    /// All registered events.
    /// Do not manually add items without locking.
    /// </summary>
    public HashSet<AsyncEvent> Callbacks { get; private set; } = new HashSet<AsyncEvent>();

    public AsyncEventHandler()
    {
    }

    /// <summary>
    /// Subscribe to the event.
    /// </summary>
    /// <param name="callback">The action which should get executed when the event fires.</param>
    public void Register(AsyncEvent callback)
    {
        Callbacks ??= new HashSet<AsyncEvent>();

        lock (Callbacks)
            Callbacks.Add(callback);
    }

    /// <summary>
    /// Unsubscribe from the event.
    /// </summary>
    /// <param name="callback">The action which shouldn't get executed anymore when the event fires.</param>
    public void Unregister(AsyncEvent callback)
    {
        Callbacks ??= new HashSet<AsyncEvent>();

        lock (Callbacks)
            Callbacks.Remove(callback);
    }

    public static AsyncEventHandler operator +(AsyncEventHandler asyncEventHandler, AsyncEvent callback)
    {
        asyncEventHandler.Register(callback);

        return asyncEventHandler;
    }

    public static AsyncEventHandler? operator -(AsyncEventHandler asyncEventHandler, AsyncEvent callback)
    {
        asyncEventHandler.Unregister(callback);

        return asyncEventHandler;
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
        Task[] tasks;
        lock (Callbacks)
        {
            tasks = new Task[Callbacks.Count];
            int i = 0;
            foreach (var callback in Callbacks)
            {
                cancellationToken.ThrowIfCancellationRequested();
                tasks[i] = callback(cancellationToken);
                i++;
            }
        }

        return Task.WhenAll(tasks);
    }
}

/// <summary>
/// A thread-safe asynchronus event handler.
/// </summary>
/// <typeparam name="TEventData">The generic type which holds the event data.</typeparam>
public struct AsyncEventHandler<TEventData>
{
    /// <summary>
    /// All registered events.
    /// Do not manually add items without locking.
    /// </summary>
    public HashSet<AsyncEvent<TEventData>> Callbacks { get; private set; } = new HashSet<AsyncEvent<TEventData>>();

    public AsyncEventHandler()
    {
    }

    /// <summary>
    /// Subscribe to the event.
    /// </summary>
    /// <param name="callback">The action which should get executed when the event fires.</param>
    public void Register(AsyncEvent<TEventData> callback)
    {
        Callbacks ??= new HashSet<AsyncEvent<TEventData>>();

        lock (Callbacks)
            Callbacks.Add(callback);
    }

    /// <summary>
    /// Unsubscribe from the event.
    /// </summary>
    /// <param name="callback">The action which shouldn't get executed anymore when the event fires.</param>
    public void Unregister(AsyncEvent<TEventData> callback)
    {
        Callbacks ??= new HashSet<AsyncEvent<TEventData>>();

        lock (Callbacks)
            Callbacks.Remove(callback);
    }

    public static AsyncEventHandler<TEventData> operator +(AsyncEventHandler<TEventData> asyncEventHandler, AsyncEvent<TEventData> callback)
    {
        asyncEventHandler.Register(callback);

        return asyncEventHandler;
    }

    public static AsyncEventHandler<TEventData>? operator -(AsyncEventHandler<TEventData> asyncEventHandler, AsyncEvent<TEventData> callback)
    {
        asyncEventHandler.Unregister(callback);

        return asyncEventHandler;
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
    public async Task InvokeAsync(TEventData data, CancellationToken cancellationToken = default)
    {
        Task[] tasks;
        lock (Callbacks)
        {
            tasks = new Task[Callbacks.Count];
            int i = 0;
            foreach (var callback in Callbacks)
            {
                cancellationToken.ThrowIfCancellationRequested();
                tasks[i] = callback(data, cancellationToken);
                i++;
            }
        }

        await Task.WhenAll(tasks);
    }
}