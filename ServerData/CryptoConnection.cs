using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;

namespace ServerData
{
    public class CryptoConnection : SocketConnection
    {
        private Socket socket;

        byte[] key;
        byte[] iv;

        public CryptoConnection(Socket socket, byte[] key, byte[] iv)
            : base(socket, false)
        {
            this.key = key;
            this.iv = iv;

            // The listening is not started directly to avoid a race condition
            // of setting the key and iv with the listener.
            StartListening();
        }

        protected override void OnMessageReceived(MessageReceivedEventArgs e)
        {
            // Decrypt message.
            byte[] msg = CryptoProvider.DecryptData(e.RawMessage, key, iv);
            base.OnMessageReceived(new MessageReceivedEventArgs(msg));
        }

        public override bool SendMessage(byte[] msg)
        {
            if (msg == null)
            {
                throw new ArgumentNullException();
            }

            // Encrypt message.
            byte[] message = CryptoProvider.EncryptData(msg, key, iv);
            return base.SendMessage(message);
        }

    }
}
