﻿using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;

namespace ServerData
{
    public class CryptoConnection
    {
        private Socket socket;
        private NetworkStream netStream;

        byte[] key;
        byte[] iv;

        private Thread listenThread;

        public event MessageReceivedEventHandler MessageReceived;

        public CryptoConnection(Socket socket, byte[] key, byte[] iv)
        {
            if (socket == null)
            {
                throw new ArgumentNullException();
            }

            // ToDo: Verify socket.

            this.key = key;
            this.iv = iv;

            this.socket = socket;
            netStream = new NetworkStream(socket);

            listenThread = new Thread(Listen);
            listenThread.Start();

        }

        public bool SendMessage(byte[] msg)
        {
            if (msg == null)
            {
                throw new ArgumentNullException();
            }

            int msgLength = msg.Length;

            if (msgLength <= 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            try
            {
                byte[] message = CryptoProvider.EncryptData(msg, key, iv);
                msgLength = message.Length;

                // Send message length.
                byte[] header = BitConverter.GetBytes(msgLength);
                for (int bytesSend = 0; bytesSend < 4;)
                {
                    bytesSend += socket.Send(header, bytesSend, 4 - bytesSend, SocketFlags.None);
                }

                // Send message.
                for (int bytesSend = 0; bytesSend < msgLength;)
                {
                    bytesSend += socket.Send(message, bytesSend, msgLength - bytesSend, SocketFlags.None);
                }

                return true;
            }
            catch (SocketException)
            {
                // ToDo: Handle exception.
                return false;
            }
        }

        private void Listen()
        {
            while (true)
            {
                try
                {

                    // Read a message-length
                    byte[] header = new byte[4];

                    for (int bytesRead = 0; bytesRead < 4;)
                    {
                        bytesRead += socket.Receive(header, bytesRead, 4 - bytesRead, SocketFlags.None);
                    }

                    int msgLength = BitConverter.ToInt32(header, 0);

                    // Read message
                    byte[] message = new byte[msgLength];
                    for (int bytesRead = 0; bytesRead < msgLength;)
                    {
                        bytesRead += socket.Receive(message, bytesRead, msgLength - bytesRead, SocketFlags.None);
                    }

                    byte[] msg = CryptoProvider.DecryptData(message, key, iv);

                    if (MessageReceived != null)
                    {
                        MessageReceived(this, new MessageReceivedEventArgs(msg));
                    }
                }
                catch (SocketException)
                {
                    // ToDo: Handle exception.
                    break;
                }
            }
        }

    }
}
