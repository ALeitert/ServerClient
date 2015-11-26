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
        public static byte[] ExampleKey = new byte[] { 180, 167, 65, 132, 32, 91, 239, 188, 85, 245, 222, 175, 79, 113, 96, 95, 121, 16, 155, 219, 1, 127, 131, 1, 244, 3, 181, 3, 217, 123, 151, 32 };
        public static byte[] ExampleIV = new byte[] { 210, 128, 172, 109, 54, 118, 164, 237, 44, 160, 102, 145, 83, 121, 237, 190 };

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
