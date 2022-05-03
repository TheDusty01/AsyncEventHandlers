using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncEventHandlers
{
    /// <summary>
    /// The "action" delegate which should be supplied to the
    /// <see cref="AsyncEventHandler{TEventArgs}.Register(AsyncEvent{TEventArgs})"/> and <see cref="AsyncEventHandler{TEventArgs}.Unregister(AsyncEvent{TEventArgs})"/>
    /// methods.
    /// <para/>
    /// This shouldn't be used with the <see langword="event"/> keyword.
    /// </summary>
    /// <typeparam name="TEventArgs"><see cref="AsyncEventArgs"/> or a derived type</typeparam>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An object that contains the event data.</param>
    /// <returns></returns>
    public delegate Task AsyncEvent<in TEventArgs>(object? sender, TEventArgs e)
        where TEventArgs : AsyncEventArgs;


    /// <summary>
    /// A thread-safe asynchronus event handler.
    /// </summary>
    /// <typeparam name="TEventArgs">The generic type which holds the event data.</typeparam>
    public struct AsyncEventHandler<TEventArgs>
        where TEventArgs : AsyncEventArgs
    {
        /// <summary>
        /// All registered events.
        /// </summary>
        public HashSet<AsyncEvent<TEventArgs>> Callbacks { get; private set; } = new HashSet<AsyncEvent<TEventArgs>>();

        public AsyncEventHandler()
        {
        }

        /// <summary>
        /// Subscribe to the event.
        /// </summary>
        /// <param name="callback">The action which should get executed when the event fires.</param>
        public void Register(AsyncEvent<TEventArgs> callback)
        {
            if (Callbacks is null)
                Callbacks = new HashSet<AsyncEvent<TEventArgs>>();

            lock (Callbacks)
                Callbacks.Add(callback);
        }

        /// <summary>
        /// Unsubscribe from the event.
        /// </summary>
        /// <param name="callback">The action which shouldn't get executed anymore when the event fires.</param>
        public void Unregister(AsyncEvent<TEventArgs> callback)
        {
            lock (Callbacks)
                Callbacks.Remove(callback);
        }

        public static AsyncEventHandler<TEventArgs> operator +(AsyncEventHandler<TEventArgs> asyncEventHandler, AsyncEvent<TEventArgs> callback)
        {
            asyncEventHandler.Register(callback);

            return asyncEventHandler;
        }

        public static AsyncEventHandler<TEventArgs>? operator -(AsyncEventHandler<TEventArgs> asyncEventHandler, AsyncEvent<TEventArgs> callback)
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
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An object that contains the event data.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to communicate cancellation of the async operation.</param>
        /// <exception cref="OperationCanceledException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="Exception"></exception>
        /// <returns>A <see cref="Task"/> which represents the completion of all registered events.</returns>
        public async Task InvokeAsync(object? sender, TEventArgs e, CancellationToken? cancellationToken = null)
        {
            cancellationToken ??= CancellationToken.None;
            e.CancellationToken = cancellationToken.Value;

            cancellationToken.Value.ThrowIfCancellationRequested();

            Task[] tasks;
            lock (Callbacks)
            {
                tasks = new Task[Callbacks.Count];
                int i = 0;
                foreach (var callback in Callbacks)
                {
                    cancellationToken.Value.ThrowIfCancellationRequested();
                    tasks[i] = callback.Invoke(sender, e);
                    i++;
                }
            }

            await Task.WhenAll(tasks);
        }
    }


    //public static class AsyncEventHandlerExtensions
    //{
    //    /// <summary>
    //    /// Invokes all registered events.
    //    /// <para/>
    //    /// Throws <see cref="OperationCanceledException"/> if the supplied <paramref name="cancellationToken"/> was cancelled or
    //    /// <see cref="ObjectDisposedException"/> if the supplied <paramref name="cancellationToken"/> was disposed or
    //    /// <see cref="Exception"/> if any of the registered event handlers threw an exception.
    //    /// </summary>
    //    /// <typeparam name="TEventArgs">The generic type which holds the event data.</typeparam>
    //    /// <param name="sender">The source of the event.</param>
    //    /// <param name="e">An object that contains the event data.</param>
    //    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to communicate cancellation of the async operation.</param>
    //    /// <exception cref="OperationCanceledException"></exception>
    //    /// <exception cref="ObjectDisposedException"></exception>
    //    /// <exception cref="Exception"></exception>
    //    /// <returns>A <see cref="Task"/> which represents the completion of all registered events.</returns>
    //    public static async Task InvokeAsync<TEventArgs>(this AsyncEventHandler<TEventArgs> asyncEventHandler, object? sender, TEventArgs e, CancellationToken? cancellationToken = null)
    //        where TEventArgs : AsyncEventArgs
    //    {
    //        cancellationToken ??= CancellationToken.None;
    //        e.CancellationToken = cancellationToken.Value;

    //        cancellationToken.Value.ThrowIfCancellationRequested();

    //        Task[] tasks;
    //        lock (asyncEventHandler.Callbacks)
    //        {
    //            tasks = new Task[asyncEventHandler.Callbacks.Count];
    //            for (int i = 0; i < asyncEventHandler.Callbacks.Count; i++)
    //            {
    //                cancellationToken.Value.ThrowIfCancellationRequested();
    //                tasks[i] = asyncEventHandler.Callbacks[i].Invoke(sender, e);
    //            }
    //        }

    //        await Task.WhenAll(tasks);
    //    }

    //}
}