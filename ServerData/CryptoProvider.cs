using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ServerData
{
    public class CryptoProvider
    {
        public static int KeySize = 32;
        public static int IVSize = 16;

        public static byte[] EncryptData(byte[] input, byte[] key, byte[] iv)
        {
            RijndaelManaged managed = new RijndaelManaged();
            var enc = managed.CreateEncryptor(key, iv);
            return enc.TransformFinalBlock(input, 0, input.Length);
        }

        public static byte[] DecryptData(byte[] input, byte[] key, byte[] iv)
        {
            RijndaelManaged managed = new RijndaelManaged();
            var dec = managed.CreateDecryptor(key, iv);
            return dec.TransformFinalBlock(input, 0, input.Length);
        }
    }
}
