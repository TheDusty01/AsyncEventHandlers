namespace AsyncEventHandlers.Samples.Type
{
    public class Program
    {

        private static readonly CancellationTokenSource cts = new();

        public static void Main()
        {
            var ws = new FakeWebsocketServer();


#pragma warning disable CS8604 // Possible nullreference
            // Automatically creates an instance for ws.Started if null, even thought the compiler warns you
            ws.Started += Ws_Started;
#pragma warning restore CS8604 // Possible nullreference
            ws.Started += Ws_Started_Second;

            ws.Started.Register((s, e) =>
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

        private static Task Ws_Started(object? sender, AsyncEventArgs e)
        {
            //throw new Exception("Something went wrong!");
            return Task.CompletedTask;
        }

        private static async Task Ws_Started_Second(object? sender, AsyncEventArgs e)
        {
            // Simulate long running request (e.g. sending something to an api)
            await Task.Delay(1000, e.CancellationToken);
            Console.WriteLine("Second Started event done!");
        }

        private static Task Ws_ClientConnected(object? sender, ClientConnectedAsyncEventArgs e)
        {
            Console.WriteLine($"Client connected: {e.ClientId}");

            // Cancel cts, the MessageRecieved shouldn't get executed afterwards
            cts.Cancel();

            return Task.CompletedTask;
        }

        private static Task Ws_MessageReceived(object? sender, MessageAsyncEventArgs e)
        {
            Console.WriteLine($"New message: {e.Message}");
            return Task.CompletedTask;
        }
    }
}