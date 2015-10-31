using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerData;

using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Net;

namespace Server
{
    class Server
    {

        static Socket listenerSocket;
        static List<ClientData> _clients;


        static void Main(string[] args)
        {
            Console.WriteLine("Starting Server...");
            listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _clients = new List<ClientData>();

            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(Packet.getIp4Address()), 4242);
            listenerSocket.Bind(ip);

            Thread listenThread = new Thread(ListenThread);
            listenThread.Start();
            Console.WriteLine("Success... Listening IP: " + Packet.getIp4Address() + ":4242");
        }

        static void ListenThread()
        {
            for (; ; )
            {
                listenerSocket.Listen(0);
                _clients.Add(new ClientData(listenerSocket.Accept()));
            }
        }

        public static void Data_IN(object cSocket)
        {
            Socket clientSocket = (Socket)cSocket;

            byte[] Buffer;
            int readBytes;

            for (;;)
            {
                try
                {
                    Buffer = new byte[clientSocket.SendBufferSize];
                    readBytes = clientSocket.Receive(Buffer); //  https://www.youtube.com/watch?v=X66hFZG5p3A

                    if (readBytes > 0)
                    {
                        Packet p = new Packet(Buffer);
                        dataManager(p);
                    }
                }
                catch (SocketException ex)
                {
                    Console.WriteLine("Client Disconnected.");
                }
            }
        }

        public static void dataManager(Packet p)
        {
            switch (p.packetType)
            {
                case PacketType.chat:
                    foreach (ClientData c in _clients)
                    {
                        c.clientSocket.Send(p.toBytes());
                    }
                    break;
            }
        }

        class ClientData
        {
            public Socket clientSocket;
            public Thread clientThread;
            public string id;

            public ClientData()
            {
                id = Guid.NewGuid().ToString();
                clientThread = new Thread(Server.Data_IN);
                clientThread.Start(clientSocket);
                sendRegistrationPacket();
            }

            public ClientData(Socket clientSocket)
            {
                this.clientSocket = clientSocket;
                id = Guid.NewGuid().ToString();
                clientThread = new Thread(Server.Data_IN);
                clientThread.Start(clientSocket);
                sendRegistrationPacket();
            }

            public void sendRegistrationPacket()
            {
                Packet p = new Packet(PacketType.Registration, "server");
                p.Gdata.Add(id);
                clientSocket.Send(p.toBytes());

               
            }
        }
    }
}
