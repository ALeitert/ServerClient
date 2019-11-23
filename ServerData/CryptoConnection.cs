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
        byte[] aesKey;
        byte[] ivSend;
        byte[] ivListen;

        ECDiffieHellmanCng keyExchange;
        byte[] publicKey;

        public CryptoConnection(Socket socket, byte[] key, byte[] iv)
            : base(socket, false)
        {
            this.aesKey = key;
            this.ivSend = iv;
            this.ivListen = iv;

            // The listening is not started directly to avoid a race condition
            // of setting the key and iv with the listener.
            StartListening();
        }

        public CryptoConnection(Socket socket)
            : base(socket, false)
        {
            keyExchange = new ECDiffieHellmanCng();
            keyExchange.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
            keyExchange.HashAlgorithm = CngAlgorithm.Sha256;

            publicKey = keyExchange.PublicKey.ToByteArray();
            base.SendMessage(publicKey);

            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
            {
                ivSend = new byte[CryptoProvider.IVSize];
                rngCsp.GetBytes(ivSend);
            }
            base.SendMessage(ivSend);

            StartListening();
        }

        protected override void OnMessageReceived(MessageReceivedEventArgs e)
        {
            if (aesKey == null)
            {
                byte[] pubKey = e.RawMessage;
                CngKey cngKey = CngKey.Import(pubKey, CngKeyBlobFormat.EccPublicBlob);
                aesKey = keyExchange.DeriveKeyMaterial(cngKey);
            }
            else if (ivListen == null)
            {
                ivListen = e.RawMessage;
            }
            else
            {
                // Decrypt message.
                byte[] msg = CryptoProvider.DecryptData(e.RawMessage, aesKey, ivListen);
                base.OnMessageReceived(new MessageReceivedEventArgs(msg));
            }
        }

        public override bool SendMessage(byte[] msg)
        {
            if (msg == null)
            {
                throw new ArgumentNullException();
            }

            // Encrypt message.
            byte[] message = CryptoProvider.EncryptData(msg, aesKey, ivSend);
            return base.SendMessage(message);
        }
    }
}
