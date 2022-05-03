# AsyncEventHandlers
This library provides faster and true thread-safe asynchronus event handlers for .NET.

Using the ``AsyncEventHandler`` struct for events is about ~96% faster than using the built-in event handlers. For more details check out the **Benchmarks** section.

## Setup
Just install the latest release from [NuGet](https://www.nuget.org/packages/AsyncEventHandlers) or from the [Packages tab](https://github.com/TheDusty01/AsyncEventHandlers/packages). Alternatively you can download it from the [Releases tab](https://github.com/TheDusty01/AsyncEventHandlers/releases) aswell.

## Usage
Note that when registering/unregistering a lot from different threads and using many events, consider using ``AsyncEventHandler`` instead of ``AsyncEventHandlerDelegate`` as the first one is about ~96% faster than the latter and doesn't use the built-in non thread-safe registering/unregistering mechanism.

### Using the ``AsyncEventHandler`` struct
The  ``AsyncEventHandler`` type is completely thread-safe, no matter how many events are registered/unregistered from different threads. It can almost be used as the delegate described before apart from the declaration.

Declaring an event:
```cs
public AsyncEventHandler<AsyncEventArgs> MyEvent = new AsyncEventHandler();
public AsyncEventHandler<AsyncEventArgs> AnotherEvent;  // You can even do this!

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

Invoking the event:
```cs
try
{
    await MyEvent.InvokeAsync(this, new AsyncEventArgs());
    // or with a cancellation token
    var cts = new CancellationTokenSource();
    await MyEvent.InvokeAsync(this, new AsyncEventArgs(), cts.Token);
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


### Using the delegate (not recommended)
The ``AsyncEventHandlerDelegate`` delegate is basically being used like the built-in ``EventHandler`` type apart from the invocation. See below.

Declaring an event:
```cs
public event AsyncEventHandlerDelegate? MyEvent;

// Or with custom event args:
public class MyAsyncEventArgs : AsyncEventArgs
{
    public string? Message { get; set; }
}

public event AsyncEventHandlerDelegate<MyAsyncEventArgs>? MyCustomEvent;
```

Registering/Unregistering to the event:
```cs
class WebsocketServer
{
    public event AsyncEventHandlerDelegate? MyEvent;
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

Invoking the event:
```cs
try
{
    await MyEvent.InvokeAsync(this, new AsyncEventArgs());
    // or with a cancellation token
    var cts = new CancellationTokenSource();
    await MyEvent.InvokeAsync(this, new AsyncEventArgs(), cts.Token);
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

### Sample projects
More samples can be found in the [Samples directory](/Samples).


## Benchmarks
The benchmarks can be found in the [AsyncEventHandlers.Benchmarks directory](/AsyncEventHandlers.Benchmarks).

### Data
```
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1645 (21H1/May2021Update)
Intel Core i7-7700K CPU 4.20GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.100
  [Host]     : .NET 6.0.0 (6.0.21.52210), X64 RyuJIT  [AttachedDebugger]
  DefaultJob : .NET 6.0.0 (6.0.21.52210), X64 RyuJIT


|            Method |         Mean |       Error |      StdDev |
|------------------ |-------------:|------------:|------------:|
|     Struct_Call_1 |     107.6 ns |     1.20 ns |     1.12 ns |
|   Delegate_Call_1 |   3,242.8 ns |    34.12 ns |    30.25 ns |
|   Struct_Call_100 |   9,466.6 ns |   114.86 ns |   101.82 ns |
| Delegate_Call_100 | 344,683.2 ns | 2,627.63 ns | 2,457.89 ns |
```

### Explanation
The ``Struct_Call_1`` and ``Struct_Call_100`` methods use the ``AsyncEventHandler`` struct for async events. The other methods use the delegate approach.

``Call_1`` invokes all 100 registered events async, ``Call_100`` does the same with the only difference being that the method calls ``InvokeAsync`` 100 times.

### Speed improvements
Struct_Call_1 vs. Delegate_Call_1:\
``100 - ((107.6/3242.8) * 100) = 96.6818799 = 96.68%``

Struct_Call_100 vs. Delegate_Call_100:\
``100 - ((9466.6/344683.2) * 100) = 97.253536 = 97,25%``

As you can see, the struct approach is about **96%-97% faster**!


## License
AsyncEventHandlers is licensed under The Unlicense, see [LICENSE.txt](/LICENSE.txt) for more information.