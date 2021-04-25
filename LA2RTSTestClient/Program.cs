using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LA2RTSTestClient
{
    class Program
    {

        private const int port = 30512;
        private const string server = "127.0.0.1";

        static void Main(string[] args)
        {
            try
            {
                TcpClient client = new TcpClient();
                client.Connect(server, port);

                byte header = 0x01;
                byte[] data = new byte[256];
                data[0] = header;

                NetworkStream stream = client.GetStream();

                while (true)
                {
                    stream.Write(data, 0, 1);
                    Thread.Sleep(5000);
                }

                // Закрываем потоки
                stream.Close();
                client.Close();
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
            }

            Console.WriteLine("Запрос завершен...");
            Console.Read();
        }
    }
}
