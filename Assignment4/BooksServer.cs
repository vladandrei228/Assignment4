using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using Assignment_1;

namespace Assignment4
{
    class BooksServer
    {
        private static List<Book> books = new List<Book>()
        {
            new Book("Heidi", "Johanna Spyri", 365, "1-1234567-001"),
            new Book("Rich Dad, Poor Dad", "Robert Kiyosaki", 215, "1-1234567-093"),
            new Book("Increase your finaincial IQ", "Robert Kiyosaki", 202, "1-1234567-094"),
            new Book("Reach Kid, Smart Kid", "Robert Kiyosaki", 165, "1-1234567-095"),
            new Book("CashFlow Quadrant", "Robert Kiyosaki", 175, "1-1234567-097"),
            new Book("ABCs of real estate investing", "Robert Kiyosaki", 305, "1-1234567-098"),
        };

        private TcpClient connectionSocket;
        private TcpListener serverSocket;

        public BooksServer(TcpClient connectionSocket)
        {
            this.connectionSocket = connectionSocket;
        }

        public BooksServer(ref TcpClient connectionSocket, ref TcpListener serverSocket)
        {
            this.connectionSocket = connectionSocket;
            this.serverSocket = serverSocket;
        }

        internal void DoIt()
        {
            Stream _stream = connectionSocket.GetStream();
            StreamReader _streamReader = new StreamReader(_stream);
            StreamWriter _streamWriter = new StreamWriter(new BufferedStream(_stream));
            _streamWriter.AutoFlush = true;

            string message = _streamReader.ReadLine();
            string answer, first;
            while (message!=null && message != "")
            {
                Console.WriteLine("Client: " + message);
                string[] list = message.Split(' ');
                first = list[0].ToUpper();
                if (first.Equals("STOP"))
                {
                    Console.WriteLine("Wants to stop");
                    _stream.Close();
                    connectionSocket.Close();
                    while (serverSocket.Pending())
                    Thread.Sleep(100);
                    Console.WriteLine("Shutdown sys; Server is stopped");

                    serverSocket.Stop();
                    break;
                }

                if (first.Equals("GET") && list.Length == 2 )
                {
                    string stringJson = JsonSerializer.Serialize(GetBook(list[1]));
                    _streamWriter.Write(stringJson);
                }

                if (first.Equals("SAVE")&& list.Length == 2)
                {
                    SaveBook(JsonSerializer.Deserialize<Book>(list[1]));
                }

                if (first.Equals("GETALL") && list.Length==2)
                {
                    string stringJson = JsonSerializer.Serialize(GetAllBooks());
                    _streamWriter.Write((stringJson));
                }
                else
                {
                    answer = message.ToUpper();
                    _streamWriter.WriteLine(answer);
                    message = _streamReader.ReadLine();
                }


            }

            _streamWriter.Write("\r\n");
            _streamWriter.Flush();
            _streamWriter.BaseStream.Flush();
            _streamWriter.Close();
            _stream.Close();
            connectionSocket.Close();


        }

        private Book GetBook(string Isbn13)
        {
            return books.FirstOrDefault(t => t.isbn13 == Isbn13);
        }

        private List<Book> getAllBooks()
        {
            return books;
        }

        private void SaveBook(Book newBook)
        {
            books.Add(newBook);
        }



    }
}
