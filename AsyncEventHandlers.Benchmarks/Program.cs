using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace AsyncEventHandlers.Benchmarks
{

    public class Program
    {
        public static void Main()
        {
            BenchmarkRunner.Run<Program>();
        }

        private static AsyncEventHandler<AsyncEventArgs> Struct_AsyncEventHandler = new AsyncEventHandler<AsyncEventArgs>();
        private static event AsyncEventHandlerDelegate<AsyncEventArgs> Delegate_AsyncEventHandler;

        public Program()
        {
            for (int i = 0; i < 100; i++)
            {
                Struct_AsyncEventHandler += Struct_AsyncEventHandler_Event;
                Delegate_AsyncEventHandler += Delegate_AsyncEventHandler_Event;
            }
        }

        private Task Struct_AsyncEventHandler_Event(object? sender, AsyncEventArgs e)
        {
            return Task.CompletedTask;
        }

        private Task Delegate_AsyncEventHandler_Event(object? sender, AsyncEventArgs e)
        {
            return Task.CompletedTask;
        }


        private Task Struct_Call()
        {
            var args = new AsyncEventArgs();
            return Struct_AsyncEventHandler.InvokeAsync(this, args);
        }

        private ValueTask Delegate_Call()
        {
            var args = new AsyncEventArgs();
            return Delegate_AsyncEventHandler.InvokeAsync(this, args);
        }

        [Benchmark]
        public async Task Struct_Call_1()
        {
            await Struct_Call();
        }

        [Benchmark]
        public async Task Delegate_Call_1()
        {
            await Delegate_Call();
        }

        [Benchmark]
        public async Task Struct_Call_100()
        {
            for (int i = 0; i < 100; i++)
                await Struct_Call();
        }

        [Benchmark]
        public async Task Delegate_Call_100()
        {
            for (int i = 0; i < 100; i++)
                await Delegate_Call();
        }
    }

}