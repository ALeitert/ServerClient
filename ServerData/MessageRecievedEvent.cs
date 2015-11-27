using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerData
{
    public delegate void MessageReceivedEventHandler(object sender, MessageReceivedEventArgs e);

    public class MessageReceivedEventArgs : EventArgs
    {
        public byte[] RawMessage { get; protected set; }

        public MessageReceivedEventArgs(byte[] message)
        {
            RawMessage = message;
        }
    }

}
