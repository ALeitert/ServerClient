using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;

namespace ServerData
{
    [Serializable]
    public class Packet
    {
        public List<string> Gdata;
        public int packetInt;
        public bool packetBool;
        public string senderId;
        public PacketType packetType;

        public Packet(PacketType type, string senderId)
        {
            Gdata = new List<string>();
            this.senderId = senderId;
            this.packetType = type;
        }

        public Packet(byte[] packetBytes)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(packetBytes);

            Packet p = (Packet)bf.Deserialize(ms);
            ms.Close();
            Gdata = p.Gdata;
            packetInt = p.packetInt;
            packetBool = p.packetBool;
            senderId = p.senderId;
            packetType = p.packetType;
        }

        public byte[] toBytes()
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, this);
            byte[] bytes = ms.ToArray();
            ms.Close();
            return bytes;
        }


        public static string getIp4Address()
        {
            IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress i in ips)
            {
                if (i.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return i.ToString();
                }
            }
            return "127.0.0.1";
        }
    }

    public enum PacketType
    {
        Registration,
        chat
    }
}
