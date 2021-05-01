using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

using ServerData;

namespace Client
{
    public partial class ClientForm : Form
    {
        CryptoClient client;

        private bool logActive = true;
        private delegate void LogDelegate(string text);

        public ClientForm()
        {
            InitializeComponent();
        }

        private void Log(string text)
        {
            if (!logActive) return;

            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new LogDelegate(Log), new object[] { text });
            }
            else
            {
                txtLog.AppendText(text + Environment.NewLine);
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (client != null)
            {
                client.Close();
            }

            client = new CryptoClient();
            client.MessageReceived += Socket_MessageReceived;
            client.ConnectionEnded += Socket_ConnectionEnded;

            bool connected = client.Connect(new IPEndPoint(IPAddress.Parse(txtIP.Text), 4242));
            if (connected)
            {
                Log("Connected to server.");
            }
            else
            {
                Log("Could not connect to server.");
            }
        }

        private void Socket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            string msg = Encoding.Default.GetString(e.RawMessage);
            Log("Server: " + msg);
        }

        private void Socket_ConnectionEnded(object sender, EventArgs e)
        {
            Log("Server Disconnected.");
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                if (client != null)
                {
                    string msg = txtMsg.Text;
                    byte[] data = Encoding.Default.GetBytes(msg);

                    if (!client.SendMessage(data))
                    {
                        Log("Unable to send message.");
                    }
                    else
                    {
                        txtMsg.Text = string.Empty;
                    }
                }
            }
            catch (SocketException)
            {
                Log("Unable to send message.");
            }
        }

        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            logActive = false;
            if (client != null)
            {
                client.Close();
            }
        }
    }
}
