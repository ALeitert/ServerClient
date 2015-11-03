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

        private SocketConnection connection;

        private delegate void LogDelegate(string text);

        public ClientForm()
        {
            InitializeComponent();
        }

        private void Log(string text)
        {
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
            }

            connection = new SocketConnection(master);

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
    }
}
