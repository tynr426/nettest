using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace FrameworkConsole
{
    public class TimerTest
    {
        private static System.Timers.Timer aTimer;

        public static void Main3()
        {
            string tid = Thread.CurrentThread.ManagedThreadId.ToString();
            Console.WriteLine($"Main tid {tid}");

            SetTimer();

            Console.WriteLine("\nPress the Enter key to exit the application...\n");
            Console.WriteLine("The application started at {0:HH:mm:ss.fff}", DateTime.Now);
            Console.ReadLine();
            aTimer.Stop();
            aTimer.Dispose();

            Console.WriteLine("Terminating the application...");
        }

        private static void SetTimer()
        {
            // Create a timer with a two second interval.
            aTimer = new System.Timers.Timer(2000);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            string tid = Thread.CurrentThread.ManagedThreadId.ToString();
            Console.WriteLine($"OnTimedEvent tid {tid}");
            Cal();
            Thread.Sleep(1000);
            Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}",
                              e.SignalTime);
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
    }
}
