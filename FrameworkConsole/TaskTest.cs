using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FrameworkConsole
{
    class TaskTest
    {
        static void Main11(string[] args)
        {
            Task t = new Task(() =>
            {
                Console.WriteLine("任务开始工作……");
                //模拟工作过程
                Thread.Sleep(5000);
                Console.WriteLine("任务开始工作2……");
            });
            t.Start();
 
            t.ContinueWith((task) =>
            {
                Console.WriteLine("任务完成，完成时候的状态为：");
                Console.WriteLine("IsCanceled={0}\tIsCompleted={1}\tIsFaulted={2}", task.IsCanceled, task.IsCompleted, task.IsFaulted);
            });
            Console.WriteLine("task测试");
            Task.WaitAll(t);
            Console.WriteLine("task测试2");
            Console.ReadKey();
        }
    }
}
