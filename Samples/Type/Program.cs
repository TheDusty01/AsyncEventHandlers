namespace AsyncEventHandlers.Samples.Type;

public class Program
{

    private static readonly CancellationTokenSource cts = new();

    public static void Main()
    {
        var ws = new FakeWebsocketServer();

        // Automatically creates an instance for ws.Started if null, even thought the compiler warns you
        ws.Started += Ws_Started;
        ws.Started += Ws_Started_Second;

        ws.Started.Register(e =>
        {
            Console.WriteLine("Adding the started event via the register method also works!");
            return Task.CompletedTask;
        });

        ws.ClientConnected += Ws_ClientConnected;
        ws.MessageReceived += Ws_MessageReceived;
      
        ws.Run(cts.Token);

        Console.WriteLine("Press any key to exit");
        Console.ReadKey(true);
    }

    private static Task Ws_Started(CancellationToken cancellationToken)
    {
        //throw new Exception("Something went wrong!");
        return Task.CompletedTask;
    }

    private static async Task Ws_Started_Second(CancellationToken cancellationToken)
    {
        // Simulate long running request (e.g. sending something to an api)
        await Task.Delay(1000, cancellationToken);
        Console.WriteLine("Second Started event done!");
    }

    private static Task Ws_ClientConnected(ClientConnectedData e, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Client connected: {e.ClientId}");

        // Cancel cts, the MessageRecieved shouldn't get executed afterwards
        cts.Cancel();

        return Task.CompletedTask;
    }

    private static Task Ws_MessageReceived(MessageData e, CancellationToken cancellationToken)
    {
        Console.WriteLine($"New message: {e.Message}");
        return Task.CompletedTask;
    }
}