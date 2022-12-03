using System.Threading;

namespace AsyncEventHandlers;

/// <summary>
/// The <see cref="CancellationToken"/> is automatically set.
/// </summary>
public interface IAsyncEventArgs
{
    CancellationToken CancellationToken { get; internal set; }
}

/// <inheritdoc cref="IAsyncEventArgs" />
public class AsyncEventArgs : IAsyncEventArgs
{
    public CancellationToken CancellationToken { get; set; }
}