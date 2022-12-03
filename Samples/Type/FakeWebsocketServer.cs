namespace AsyncEventHandlers.Samples.Type;

public class MessageData
{
    public string? Message { get; set; }
}

public class ClientConnectedData
{
    public int ClientId { get; set; }
}

public class FakeWebsocketServer
{
    public AsyncEventHandler Started = default;
    private readonly AsyncEventHandler<ClientConnectedData> clientConnected = new AsyncEventHandler<ClientConnectedData>();
    public event AsyncEvent<ClientConnectedData> ClientConnected
    {
        add { clientConnected.Register(value); }
        remove { clientConnected.Unregister(value); }
    }
    public AsyncEventHandler<MessageData> MessageReceived = new AsyncEventHandler<MessageData>();

    public void Run(CancellationToken cancellationToken)
    {
        _ = Task.Run(() => SimulateTest(cancellationToken), cancellationToken);
    }

    private async Task SimulateTest(CancellationToken cancellationToken)
    {
        try
        {
            await Started!.InvokeAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Started event threw an exception: {ex}");
        }

        // Simulate client connecting
        await Task.Delay(1000);
        await clientConnected.InvokeAsync(new ClientConnectedData { ClientId = 1 }, cancellationToken);

        // Simulate client message
        await Task.Delay(1000);
        try
        {
            await MessageReceived.InvokeAsync(new MessageData { Message = "Hello!" }, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Token was cancelled");
        }
    }
}
