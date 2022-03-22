using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncEventHandlers
{
    /// <summary>
    /// Represents the method that will handle an event when the event provides data.
    /// </summary>
    /// <typeparam name="TEventArgs"></typeparam>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    public delegate Task AsyncEventHandlerDelegate<TEventArgs>(object? sender, TEventArgs e) where TEventArgs : AsyncEventArgs;

    public static class AsyncEventHandlerExtensions
    {
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
        /// <returns>A <see cref="Task"/> which represents the completion of all registered events.</returns>
        public static async Task InvokeAsync<TEventArgs>(this AsyncEventHandlerDelegate<TEventArgs> asyncEventHandler, object? sender, TEventArgs e, CancellationToken? cancellationToken = null)
            where TEventArgs : AsyncEventArgs
        {
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
