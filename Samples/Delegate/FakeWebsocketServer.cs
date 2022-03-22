using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncEventHandlers.Samples.Delegate
{
    public class MessageAsyncEventArgs : AsyncEventArgs
    {
        public string? Message { get; set; }
    }

    public class ClientConnectedAsyncEventArgs : AsyncEventArgs
    {
        public int ClientId { get; set; }
    }

    public class FakeWebsocketServer
    {
        public event AsyncEventHandlerDelegate? Started;
        public event AsyncEventHandlerDelegate<ClientConnectedAsyncEventArgs>? ClientConnected;
        public event AsyncEventHandlerDelegate<MessageAsyncEventArgs>? MessageReceived;

        public void Run(CancellationToken cancellationToken)
        {
            _ = Task.Run(() => SimulateTest(cancellationToken), cancellationToken);
        }

        private async Task SimulateTest(CancellationToken cancellationToken)
        {
            try
            {
                await Started!.InvokeAsync(this, new AsyncEventArgs(), cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Started event threw an exception: {ex}");
            }

            // Simulate client connecting
            await Task.Delay(1000);
            await ClientConnected!.InvokeAsync(this, new ClientConnectedAsyncEventArgs { ClientId = 1 }, cancellationToken);

            // Simulate client message
            await Task.Delay(1000);
            try
            {
                await MessageReceived!.InvokeAsync(this, new MessageAsyncEventArgs { Message = "Hello!" }, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Token was cancelled");
            }
        }
    }
}
