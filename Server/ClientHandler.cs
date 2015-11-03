using System;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    public delegate void DataReceivedEventHandler(object sender, DataReceivedArgs e);

    public class DataReceivedArgs : EventArgs
    {
        public byte[] Data { get; protected set; }
        public int Length { get; protected set; }

        public DataReceivedArgs(byte[] data, int length)
        {
            Data = data;
            Length = length;
        }
    }

    class ClientHandler
    {
        // ToDo: Other IDs
        public Socket clientSocket;
        public Thread clientThread;
        public string id;

        public event DataReceivedEventHandler DataReceived;

        public ClientHandler(Socket clientSocket)
        {
            if (clientSocket == null)
            {
                throw new ArgumentNullException();
            }
            this.clientSocket = clientSocket;
            id = Guid.NewGuid().ToString();
            clientThread = new Thread(ListenToClient);
            clientThread.Start();
        }

        private void ListenToClient()
        {
            byte[] buffer;
            int readBytes;

            while (true)
            {
                try
                {
                    // ToDo: Ensure that all data was read. See: https://stackoverflow.com/a/5934816/559144
                    buffer = new byte[clientSocket.SendBufferSize];
                    readBytes = clientSocket.Receive(buffer);
                    //NetworkStream nstr = new NetworkStream()

                    if (readBytes > 0)
                    {
                        if (DataReceived != null)
                        {
                            DataReceived(this, new DataReceivedArgs(buffer, readBytes));
                        }
                    }
                }
                catch (SocketException ex)
                {
                    Console.WriteLine("Client Disconnected.");
                    break;
                }
            }
        }

    }
}
