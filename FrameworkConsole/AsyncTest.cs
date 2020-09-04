using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FrameworkConsole
{
    class AsyncTest
    {
        static void Main2(string[] args)
        {
            string tid = Thread.CurrentThread.ManagedThreadId.ToString();
            Console.WriteLine($"Main1 tid {tid}");
            //Task<int> t = CalAsync();
            //CalAsync2();
            Cal2();
            Console.WriteLine($"Main after CalAsync");
            Console.Read();
        }
        public static long Cal()
        {
            string tid = Thread.CurrentThread.ManagedThreadId.ToString();
            Console.WriteLine($"Cal tid {tid}");
            long sum = 0;
            for (int i = 0; i < 9999999; i++)
            {
                sum = sum + i;
            }
            Console.WriteLine($"sum={sum}");
            return sum;
        }
        public static async Task<long> Cal2()
        {
            string tid = Thread.CurrentThread.ManagedThreadId.ToString();
            Console.WriteLine($"Cal2 tid {tid}");
            long sum = await CalAsync();
            Console.WriteLine($"sum={sum}");
            return sum;
        }
        public static async Task<long> CalAsync()
        {
            string tid = Thread.CurrentThread.ManagedThreadId.ToString();
            Console.WriteLine($"CalAsync1 tid {tid}");
            long result = await Task.Run(new Func<long>(Cal));

            tid = Thread.CurrentThread.ManagedThreadId.ToString();
            Console.WriteLine($"CalAsync2 tid {tid}, result={result}");

           
            return result;
        }
        public static long CalAsync2()
        {
            string tid = Thread.CurrentThread.ManagedThreadId.ToString();
            Console.WriteLine($"CalAsync2 tid {tid}");
            long result = Task.Run(new Func<long>(Cal)).Result;

            tid = Thread.CurrentThread.ManagedThreadId.ToString();
            Console.WriteLine($"CalAsync2 tid {tid}, result={result}");

     
            return result;
        }
    }
}
