using ECF.Sockets;
using ECF.Sockets.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkConsole
{
    class SocketClientTest
    {
        static void Mainn(string[] args)
        {
            //SendCommand("Clone", "data.clone.push.command2222", new object[] { 44, 2 });
            //bool sendCmd =SendCommand("Clone", "data.clone.push.command2222", new object[] { 44, 2 });
            //if (!sendCmd)
            //{
            //    Console.WriteLine("命令已发送失败");
            //}

            SocketCommand cmd = new SocketCommand()
            {
                Plugin = "Clone",
                Action = "data.clone.push.command2222",
                Parameters = new object[] { 44, 2 },
                IsReceipt = false
            };
            SocketClient tcpClient = new SocketClient();

            var endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5555);
            Task<bool> connect = tcpClient.Connect(endPoint);
            //等待连接成功
            connect.ContinueWith((result) =>
            {
                Console.WriteLine(result.IsCompleted);
                Console.WriteLine(tcpClient.IsConnected);
                if (tcpClient.IsConnected)
                {
                    Task<bool> command = tcpClient.SendCommand(cmd);

                    //command.Wait();使用这个命令iis都要崩
                    command.ContinueWith((t) =>
                    {
                        //命令执行完后执行关闭
                        tcpClient.Dispose();
                    });
                }
            });

            Console.ReadLine();
        }
        static async void SendCommand(string plugin, string action, object[] parameters)
        {
            try
            {
                SocketCommand cmd = new SocketCommand()
                {
                    Plugin = plugin,
                    Action = action,
                    Parameters = parameters,
                    IsReceipt = true
                };
                SocketClient tcpClient = new SocketClient();

                var endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5555);
                var state = await tcpClient.Connect(endPoint);
                if (state)
                {
                    //等待连接成功
                    Task<bool> command = tcpClient.SendCommand(cmd);
                    command.ContinueWith((t) =>
                    {
                        //命令执行完后执行关闭
                        tcpClient.Dispose();
                    });
                    tcpClient.Dispose();
                }
            }
            catch (Exception ex)
            {
               // return false;
            }
        }
    }
}
