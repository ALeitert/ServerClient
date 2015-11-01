using System;
using ServerData;
using System.Net;
using System.Net.Sockets;
using System.Threading;



namespace ConsoleClient
{
    class Program
    {
        public static Socket master;
        public static string name;
        public static string id;

        static void Main(string[] args)
        {
            Console.WriteLine("Enter Your Name: ");
            name = Console.ReadLine();

        A: Console.Clear();
            Console.WriteLine("Gimme ip");
            string ip = Console.ReadLine();

            master = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(ip), 4242);

            try
            {
                master.Connect(ipe);
            }
            catch
            {
                Console.WriteLine("Could not connect to server");
                Thread.Sleep(1000);
                goto A;
            }

            Thread t = new Thread(data_IN);
            t.Start();

            for (;;)
            {
                Console.Write("::>");
                string input = Console.ReadLine();

                Packet p = new Packet(PacketType.chat, id);
                p.Gdata.Add(name);
                p.Gdata.Add(input);
                master.Send(p.toBytes());
            }
        }

        static void data_IN()
        {
            byte[] buffer;
            int readBytes;

            for (;;)
            {
                try
                {
                    buffer = new byte[master.SendBufferSize];
                    readBytes = master.Receive(buffer);

                    if (readBytes > 0)
                    {
                        DataManager(new Packet(buffer));
                    }
                }
                catch (SocketException ex)
                {
                    Console.WriteLine("Server Lost!");
                    Console.ReadLine();
                    Environment.Exit(0);
                }
            }

        }

        static void DataManager(Packet p)
        {
            switch (p.packetType)
            {
                case PacketType.Registration:
                    Console.WriteLine("Connected to Server!");
                    id = p.Gdata[0];
                    break;
                case PacketType.chat:
                    Console.WriteLine(p.Gdata[0] + ": " + p.Gdata[1]);
                    break;
            }
        }
    }
}
