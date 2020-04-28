using System;
using System.Net.Sockets;
using System.Text;
using System.Net;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleClient
{
    class Program
    {
            static string userName;
            private const string host = "127.0.0.1";
            private const int port = 8888;
            static TcpClient client;
            static NetworkStream stream;

            static void Main(string[] args)
            {
                Console.Write("Введите свое имя: ");
                userName = Console.ReadLine();
                client = new TcpClient();
                try
                {
                    client.Connect(host, port); 
                    stream = client.GetStream(); 

                    string message = userName;
                    byte[] data = Encoding.Unicode.GetBytes(message);
                    stream.Write(data, 0, data.Length);

                    Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                    receiveThread.Start(); 
                    SendMessage();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    Disconnect();
                }
            }
            static void SendMessage()
            {
                Console.WriteLine("Введите сообщение: ");

                while (true)
                {   
                    string message = Console.ReadLine();
               
                    Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop - 1);
                    Console.WriteLine("Я: " + message);
                    byte[] data = Encoding.Unicode.GetBytes(message);
                    stream.Write(data, 0, data.Length);
              
                if (message == ".quit")
                {
                    Disconnect();
                }
            }

            }
            static void ReceiveMessage()
            {
                while (true)
                {
              try
                    {
                        byte[] data = new byte[64];
                        StringBuilder builder = new StringBuilder();
                        int bytes = 0;
                        do
                        {
                            bytes = stream.Read(data, 0, data.Length);
                            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        }
                        while (stream.DataAvailable);

                        string message = builder.ToString();
                        Console.WriteLine(message);
                    }
                    catch
                    {
                        Console.WriteLine("Подключение прервано!"); 
                        Console.ReadLine();
                        Disconnect();
                    }
                }
            }

            static void Disconnect()
            {
                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();
                Environment.Exit(0); 
            }
        }
    public class ServerObject
    {
        static TcpListener tcpListener; 
        List<ClientObject> clients = new List<ClientObject>();

        protected internal void AddConnection(ClientObject clientObject)
        {
            clients.Add(clientObject);
        }
        protected internal void RemoveConnection(string id)
        {
            ClientObject client = clients.FirstOrDefault(c => c.Id == id);
            if (client != null)
                clients.Remove(client);
        }
        protected internal void Disconnect()
        {
            tcpListener.Stop(); 

            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close(); 
                
            }
            Environment.Exit(0); 
        }
    }
    public class ClientObject
        {
            protected internal string Id { get; private set; }
            protected internal NetworkStream Stream { get; private set; }
            TcpClient client;

            public ClientObject(TcpClient tcpClient, ServerObject serverObject)
            {
                Id = DateTime.Now.Ticks.ToString();
                client = tcpClient;
                serverObject.AddConnection(this);
            }

 
            protected internal void Close()
            {
                if (Stream != null)
                    Stream.Close();
                if (client != null)
                    client.Close();
            }
        }
    }