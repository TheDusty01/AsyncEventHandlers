# AsyncEventHandlers
This library provides true thread-safe asynchronus event handlers for .NET.\

## Setup
Just install the latest release from [NuGet](https://www.nuget.org/packages/AsyncEventHandlers). Alternatively you can download it from the [Releases tab](https://github.com/TheDusty01/AsyncEventHandlers/releases) aswell.

## Usage
Note that when registering/unregistering a lot from different threads, should consider using ``AsyncEventHandler`` instead of ``AsyncEventHandlerDelegate`` as the latter uses the built-in non thread-safe registering/unregistering mechanism.

### Using the delegate
The ``AsyncEventHandlerDelegate`` delegate is basically being used like the built-in ``EventHandler`` type apart from the invocation. See below.

Declaring an event:
```cs
public event AsyncEventHandlerDelegate<AsyncEventArgs>? MyEvent;

// Or with custom event args:
public class MyAsyncEventArgs : AsyncEventArgs
{
    public string? Message { get; set; }
}

public event AsyncEventHandlerDelegate<MyAsyncEventArgs>? MyCustomEvent;
```

Invoking the event:
```cs
try
{
    if (MyEvent is not null)
    {
        await MyEvent.InvokeAsync(this, new AsyncEventArgs());
        // or with a cancellation token
        var cts = new CancellationTokenSource();
        await MyEvent.InvokeAsync(this, new AsyncEventArgs(), cts.Token);
    }
}
catch (OperationCanceledException)
{
    // Cancellation token was cancelled
}
catch (ObjectDisposedException)
{
    // Cancellation token was disposed
}
catch (Exception)
{
    // Some registered event(s) have thrown an exception
}
```

Registering/Unregistering to the event:
```cs
class WebsocketServer
{
    public event AsyncEventHandlerDelegate<AsyncEventArgs>? MyEvent;
}

var ws = new WebsocketServer();
ws.MyEvent += Ws_MyEvent;   // Register
ws.MyEvent -= Ws_MyEvent;   // Unregister

static Task Ws_MyEvent(object? sender, AsyncEventArgs e)
{
    if (e.CancellationToken.IsCancellationRequested)
    {
        Console.WriteLine("The cancellation token was cancelled!");
    }

    return Task.CompletedTask;
}
```

### Using the ``AsyncEventHandler`` class
The  ``AsyncEventHandler`` type is completely thread-safe, no matter how many events are registered/unregistered from different threads. It can almost be used as the delegate described before apart from the declaration.

Declaring an event:
```cs
public AsyncEventHandler<AsyncEventArgs> MyEvent = new AsyncEventHandler<AsyncEventArgs>();

// Or with custom event args:
public class MyAsyncEventArgs : AsyncEventArgs
{
    public string? Message { get; set; }
}

public AsyncEventHandlerDelegate<MyAsyncEventArgs> MyCustomEvent = new AsyncEventHandler<MyAsyncEventArgs>();
```

Registering/Unregistering to the event:
```cs
class WebsocketServer
{
    public AsyncEventHandler<AsyncEventArgs> MyEvent = new AsyncEventHandler<AsyncEventArgs>();
}

var ws = new WebsocketServer();
ws.MyEvent += Ws_MyEvent;   // Register
ws.MyEvent -= Ws_MyEvent;   // Unregister
// Or use the methods
ws.MyEvent.Register(Ws_MyEvent);
ws.MyEvent.Unregister(Ws_MyEvent);

static Task Ws_MyEvent(object? sender, AsyncEventArgs e)
{
    if (e.CancellationToken.IsCancellationRequested)
    {
        Console.WriteLine("The cancellation token was cancelled!");
    }

    return Task.CompletedTask;
}
```

### Sample projects
More samples can be found in the [Samples directory](/Samples).


## License
AsyncEventHandlers is licensed under The Unlicense, see [LICENSE.txt](/LICENSE.txt) for more information.