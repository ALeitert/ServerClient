using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

using ServerData;

namespace Client
{
    public partial class ClientForm : Form
    {
        private Socket master;

        private CryptoConnection connection;

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
            if (master != null)
            {
                master.Close();
                master.Dispose();
            }

            master = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(txtIP.Text), 4242);

            try
            {
                master.Connect(ipe);
                Log("Connected to server.");
            }
            catch
            {
                Log("Could not connect to server.");
                return;
            }

            connection = new CryptoConnection(master, CryptoProvider.ExampleKey, CryptoProvider.ExampleIV);
            connection.MessageReceived += Socket_MessageReceived;
            connection.ConnectionEnded += Socket_ConnectionEnded;
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
                if (connection != null)
                {
                    string msg = txtMsg.Text;
                    byte[] data = Encoding.Default.GetBytes(msg);

                    if (!connection.SendMessage(data))
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
            if (connection != null)
            {
                connection.Close();
            }
        }
    }
}
