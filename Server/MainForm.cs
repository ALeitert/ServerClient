using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Server
{
    public partial class MainForm : Form
    {
        private delegate void LogDelegate(string text);

        private Socket listenerSocket;
        private List<ClientHandler> clientList; // ToDo: HashTable

        private IPAddress selectedIP;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // List IP adresses
            IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress i in ips)
            {
                if (i.AddressFamily == AddressFamily.InterNetwork)
                {
                    selectedIP = i;
                }
                Log(i.ToString() + "  " + i.AddressFamily.ToString());
            }

            if (selectedIP == null)
            {
                selectedIP = IPAddress.Parse("127.0.0.1");
            }

        }

        private void btnStartServer_Click(object sender, EventArgs e)
        {
            Log("Starting Server...");
            listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientList = new List<ClientHandler>();

            try
            {
                IPEndPoint ip = new IPEndPoint(selectedIP, 4242); // ToDo: Port
                listenerSocket.Bind(ip);

                Thread listenThread = new Thread(ListenThread);
                listenThread.Start();
                Log("Success... Listening IP: " + selectedIP + ":4242");
            }
            catch (Exception ex)
            {
                Log("Unable to start server.");
                Log(ex.GetType().ToString());
                Log(ex.Message);
            }

        }

        private void ListenThread()
        {
            while (true)
            {
                try
                {
                    listenerSocket.Listen(0);

                    ClientHandler ch = new ClientHandler(listenerSocket.Accept());
                    ch.DataRecieved += Client_DataRecieved;
                    clientList.Add(ch);
                }
                catch (SocketException)
                {
                    Log("Listener closed.");
                    return;
                }
            }
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

        private void btnStopServer_Click(object sender, EventArgs e)
        {
            if (listenerSocket != null)
            {
                listenerSocket.Close();
            }
        }

        private void Client_DataRecieved(object sender, DataRecievedArgs e)
        {
            string msg = Encoding.Default.GetString(e.Data);
            Log(msg);
        }
    }
}
