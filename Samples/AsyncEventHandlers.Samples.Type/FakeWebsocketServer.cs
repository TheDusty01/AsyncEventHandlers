using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncEventHandlers.Samples.Type
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
        public AsyncEventHandler<AsyncEventArgs>? Started;
        public AsyncEventHandler<ClientConnectedAsyncEventArgs> ClientConnected = new AsyncEventHandler<ClientConnectedAsyncEventArgs>();
        public AsyncEventHandler<MessageAsyncEventArgs> MessageReceived = new AsyncEventHandler<MessageAsyncEventArgs>();

        public void Run(CancellationToken cancellationToken)
        {
            _ = Task.Run(() => SimulateTest(cancellationToken), cancellationToken);
        }

        private async Task SimulateTest(CancellationToken cancellationToken)
        {
            try
            {
                if (Started is not null)
                    await Started.InvokeAsync(this, new AsyncEventArgs(), cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Started event threw an exception: {ex}");
            }

            // Simulate client connecting
            await Task.Delay(1000);
            if (ClientConnected is not null)
                await ClientConnected.InvokeAsync(this, new ClientConnectedAsyncEventArgs { ClientId = 1 }, cancellationToken);

            // Simulate client message
            await Task.Delay(1000);
            try
            {
                if (MessageReceived is not null)
                    await MessageReceived.InvokeAsync(this, new MessageAsyncEventArgs { Message = "Hello!" }, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Token was cancelled");
            }
        }
    }
}
