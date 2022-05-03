using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncEventHandlers
{
    /// <summary>
    /// Represents the method that will handle an event when the event provides data.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <returns>A <see cref="Task"/> that represents the event.</returns>
    public delegate Task AsyncEventHandlerDelegate(object? sender, AsyncEventArgs e);

    /// <summary>
    /// Represents the method that will handle an event when the event provides data.
    /// </summary>
    /// <typeparam name="TEventArgs"></typeparam>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <returns>A <see cref="Task"/> that represents the event.</returns>
    public delegate Task AsyncEventHandlerDelegate<TEventArgs>(object? sender, TEventArgs e) where TEventArgs : AsyncEventArgs;

    public static class AsyncEventHandlerDelegateExtensions
    {
        /// <summary>
        /// Invokes all registered events.
        /// <para/>
        /// Throws <see cref="OperationCanceledException"/> if the supplied <paramref name="cancellationToken"/> was cancelled or
        /// <see cref="ObjectDisposedException"/> if the supplied <paramref name="cancellationToken"/> was disposed or
        /// <see cref="Exception"/> if any of the registered event handlers threw an exception.
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An object that contains the event data.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to communicate cancellation of the async operation.</param>
        /// <exception cref="OperationCanceledException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="Exception"></exception>
        /// <returns>A <see cref="ValueTask"/> which represents the completion of all registered events.</returns>
        public static async ValueTask InvokeAsync(this AsyncEventHandlerDelegate asyncEventHandler, object? sender, AsyncEventArgs e, CancellationToken? cancellationToken = null)
        {
            if (asyncEventHandler is null)
            {
                return;
            }

            cancellationToken ??= CancellationToken.None;
            e.CancellationToken = cancellationToken.Value;

            cancellationToken.Value.ThrowIfCancellationRequested();

            Delegate[]? callbacks = asyncEventHandler.GetInvocationList();
            Task[] tasks = new Task[callbacks.Length];

            for (int i = 0; i < callbacks.Length; i++)
            {
                cancellationToken.Value.ThrowIfCancellationRequested();
                AsyncEventHandlerDelegate cb = (AsyncEventHandlerDelegate)callbacks[i];
                tasks[i] = cb.Invoke(sender, e);
            }

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Invokes all registered events.
        /// <para/>
        /// Throws <see cref="OperationCanceledException"/> if the supplied <paramref name="cancellationToken"/> was cancelled or
        /// <see cref="ObjectDisposedException"/> if the supplied <paramref name="cancellationToken"/> was disposed or
        /// <see cref="Exception"/> if any of the registered event handlers threw an exception.
        /// </summary>
        /// <typeparam name="TEventArgs">The generic type which holds the event data.</typeparam>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An object that contains the event data.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to communicate cancellation of the async operation.</param>
        /// <exception cref="OperationCanceledException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="Exception"></exception>
        /// <returns>A <see cref="ValueTask"/> which represents the completion of all registered events.</returns>
        public static async ValueTask InvokeAsync<TEventArgs>(this AsyncEventHandlerDelegate<TEventArgs> asyncEventHandler, object? sender, TEventArgs e, CancellationToken? cancellationToken = null)
            where TEventArgs : AsyncEventArgs
        {
            if (asyncEventHandler is null)
            {
                return;
            }

            cancellationToken ??= CancellationToken.None;
            e.CancellationToken = cancellationToken.Value;

            cancellationToken.Value.ThrowIfCancellationRequested();

            Delegate[]? callbacks = asyncEventHandler.GetInvocationList();
            Task[] tasks = new Task[callbacks.Length];     
            
            for (int i = 0; i < callbacks.Length; i++)
            {
                cancellationToken.Value.ThrowIfCancellationRequested();
                AsyncEventHandlerDelegate<TEventArgs> cb = (AsyncEventHandlerDelegate<TEventArgs>)callbacks[i];
                tasks[i] = cb.Invoke(sender, e);
            }

            await Task.WhenAll(tasks);
        }

    }
}
