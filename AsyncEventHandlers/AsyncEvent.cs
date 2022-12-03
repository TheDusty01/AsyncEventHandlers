using System.Threading.Tasks;
using System.Threading;

namespace AsyncEventHandlers;

/// <summary>
/// The "action" delegate which should be supplied to the
/// <see cref="AsyncEventHandler{TEventData}.Register(AsyncEvent{TEventData})"/> and <see cref="AsyncEventHandler{TEventData}.Unregister(AsyncEvent{TEventData})"/>
/// methods.
/// <para/>
/// This shouldn't be used with the <see langword="event"/> keyword.
/// </summary>
/// <typeparam name="TEventData">Event data to pass to each subscriber.</typeparam>
/// <param name="data">An object that contains the event data.</param>
/// <param name="cancellationToken">The <see cref="CancellationToken"/> to communicate cancellation of the async operation.</param>
/// <returns></returns>
public delegate Task AsyncEvent<in TEventData>(TEventData data, CancellationToken cancellationToken);

/// <summary>
/// The "action" delegate which should be supplied to the
/// <see cref="AsyncEventHandler.Register(AsyncEvent)"/> and <see cref="AsyncEventHandler.Unregister(AsyncEvent)"/>
/// methods.
/// <para/>
/// This shouldn't be used with the <see langword="event"/> keyword.
/// </summary>
/// <typeparam name="TEventData">Event data to pass to each subscriber.</typeparam>
/// <param name="cancellationToken">The <see cref="CancellationToken"/> to communicate cancellation of the async operation.</param>
/// <returns></returns>
public delegate Task AsyncEvent(CancellationToken cancellationToken);