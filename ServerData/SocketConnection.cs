using System;
using System.Net.Sockets;
using System.Threading;

namespace ServerData
{
    public class SocketConnection
    {
        private Socket socket;

        private Thread listenThread;

        public event MessageReceivedEventHandler MessageReceived;
        public event EventHandler ConnectionEnded;

        public SocketConnection(Socket socket)
            : this(socket, true)
        {
        }

        protected SocketConnection(Socket socket, bool startListening)
        {
            if (socket == null)
            {
                throw new ArgumentNullException();
            }

            // ToDo: Verify socket.

            this.socket = socket;

            if (startListening)
            {
                StartListening();
            }
        }

        public virtual bool SendMessage(byte[] message)
        {
            if (message == null)
            {
                throw new ArgumentNullException();
            }

            int msgLength = message.Length;

            if (msgLength <= 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            try
            {
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

        public void Close()
        {
            if (socket != null)
            {
                socket.Close();
            }
        }

        protected void StartListening()
        {
            listenThread = new Thread(Listen);
            listenThread.Start();
        }

        protected virtual void OnMessageReceived(MessageReceivedEventArgs e)
        {
            if (MessageReceived != null)
            {
                MessageReceived(this, e);
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

                    OnMessageReceived(new MessageReceivedEventArgs(message));
                }
                catch (SocketException)
                {
                    // ToDo: Handle exception.
                    socket.Close();
                    if (ConnectionEnded != null)
                    {
                        ConnectionEnded(this, new EventArgs());
                    }
                    break;
                }
            }
        }

    }
}
