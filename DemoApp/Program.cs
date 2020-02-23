using System;
using System.Threading.Tasks;

namespace DemoApp
{
    class Program
    {
        static void Main(string[] args)
        {
            DiContainer.Initialize();

            Task.Run(() => DoAsync()).GetAwaiter().GetResult();

            Console.ReadLine();
        }

        private static async Task DoAsync()
        {
            // Create an object with all registered interceptors.
            IBusiness business = DiContainer.Resolve<IBusiness>();

            Console.WriteLine($"Doing sync...");
            business.Do(1);
            Console.WriteLine();

            Console.WriteLine($"Counting sync...");
            Console.WriteLine($"Result: {business.Count(2)}");
            Console.WriteLine();

            Console.WriteLine($"Doing async...");
            await business.DoAsync(3);
            Console.WriteLine();

            Console.WriteLine($"Counting async...");
            Console.WriteLine($"Result: {await business.CountAsync(4)}");
            Console.WriteLine();

            Console.WriteLine($"Doing something that fails...");
            await business.DoAsync(-1);
            Console.WriteLine();

            Console.WriteLine($"Doing something that is expected to return a value but fails...");
            Console.WriteLine($"Result of the failed operation: {business.Count(-1)}");
            Console.WriteLine();
        }
    }
}
