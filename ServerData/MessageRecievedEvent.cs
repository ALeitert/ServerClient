using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerData
{
    public delegate void MessageReceivedEventHandler(object sender, MessageReceivedArgs e);

    public class MessageReceivedArgs : EventArgs
    {
        public byte[] RawMessage { get; protected set; }

        public MessageReceivedArgs(byte[] message)
        {
            RawMessage = message;
        }
    }

}
