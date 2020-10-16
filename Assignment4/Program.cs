using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Assignment4
{
    class Program
    {
        static void Main(string[] args)
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            TcpListener serverSocket = new TcpListener(ip, 4646);

            serverSocket.Start();
            try
            {
                while (true)
                {
                    TcpClient connectionSocket = serverSocket.AcceptTcpClient();
                    Console.WriteLine("Server is now activated!");
                    BooksServer server = new BooksServer(ref connectionSocket, ref serverSocket);

                    Task.Factory.StartNew((() => server.DoIt()));

                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (serverSocket != null) serverSocket.Stop();
                

            }
        }
    }
}
