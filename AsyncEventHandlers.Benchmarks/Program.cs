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

        private static AsyncEventHandler Struct_AsyncEventHandler = new AsyncEventHandler();
        private static WeakAsyncEventHandler WeakAsyncEventHandler = new WeakAsyncEventHandler();
        private static event AsyncEventHandlerDelegate<IAsyncEventArgs> Delegate_AsyncEventHandler;

        private readonly List<object> eventSources = new List<object>();

        public Program()
        {
            for (int i = 0; i < 100; i++)
            {
                eventSources.Add(new object());


                Struct_AsyncEventHandler += Struct_AsyncEventHandler_Event;
                WeakAsyncEventHandler.Register(eventSources[i], WeakAsyncEventHandler_Event);
                Delegate_AsyncEventHandler += Delegate_AsyncEventHandler_Event;
            }
        }

        private Task Struct_AsyncEventHandler_Event(CancellationToken ct)
        {
            return Task.CompletedTask;
        }

        private Task WeakAsyncEventHandler_Event(CancellationToken ct)
        {
            return Task.CompletedTask;
        }

        private Task Delegate_AsyncEventHandler_Event(object? sender, IAsyncEventArgs e)
        {
            return Task.CompletedTask;
        }


        private Task Struct_Call()
        {
            return Struct_AsyncEventHandler.InvokeAsync();
        }

        private Task Weak_Call()
        {
            return WeakAsyncEventHandler.InvokeAsync();
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
        public async Task Weak_Call_1()
        {
            await Weak_Call();
        }

        [Benchmark]
        public async Task Delegate_Call_1()
        {
            await Delegate_Call();
        }

        //[Benchmark]
        //public async Task Struct_Call_100()
        //{
        //    for (int i = 0; i < 100; i++)
        //        await Struct_Call();
        //}

        //[Benchmark]
        //public async Task Delegate_Call_100()
        //{
        //    for (int i = 0; i < 100; i++)
        //        await Delegate_Call();
        //}
    }

}