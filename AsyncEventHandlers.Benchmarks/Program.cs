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

        private static AsyncEventHandler<AsyncEventArgs> Class_AsyncEventHandler = new AsyncEventHandler<AsyncEventArgs>();
        private static event AsyncEventHandlerDelegate<AsyncEventArgs> Delegate_AsyncEventHandler;

        public Program()
        {
            for (int i = 0; i < 100; i++)
            {
                Class_AsyncEventHandler += Class_AsyncEventHandler_Event;
                Delegate_AsyncEventHandler += Delegate_AsyncEventHandler_Event;
            }
        }

        private Task Class_AsyncEventHandler_Event(object? sender, AsyncEventArgs e)
        {
            return Task.CompletedTask;
        }

        private Task Delegate_AsyncEventHandler_Event(object? sender, AsyncEventArgs e)
        {
            return Task.CompletedTask;
        }


        private Task Class_Call()
        {
            var args = new AsyncEventArgs();
            return Class_AsyncEventHandler.InvokeAsync(this, args);
        }

        private Task Delegate_Call()
        {
            var args = new AsyncEventArgs();
            return Delegate_AsyncEventHandler.InvokeAsync(this, args);
        }

        [Benchmark]
        public async Task Class_Call_1()
        {
            await Class_Call();
        }

        [Benchmark]
        public async Task Delegate_Call_1()
        {
            await Delegate_Call();
        }


        //[Benchmark]
        //public async Task Class_Call_100()
        //{
        //    for (int i = 0; i < 100; i++)
        //        await Class_Call();
        //}

        //[Benchmark]
        //public async Task Delegate_Call_100()
        //{
        //    for (int i = 0; i < 100; i++)
        //        await Delegate_Call();
        //}
    }

}