using ServerData;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace Server
{
    public partial class MainForm : Form
    {
        private bool logActive = true;
        private delegate void LogDelegate(string text);

        private CryptoServer server;
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
                    break;
                }
            }

            if (selectedIP == null)
            {
                selectedIP = IPAddress.Parse("127.0.0.1");
            }

            Log("Selected IP: " + selectedIP.ToString());
        }

        private void btnStartServer_Click(object sender, EventArgs e)
        {
            Log("Starting Server...");
            server = new CryptoServer();

            try
            {
                IPEndPoint ip = new IPEndPoint(selectedIP, 4242); // ToDo: Port

                server.Start(ip);

                server.MessageFromClient += Server_MessageFromClient;
                server.ClientConnected += Server_ClientConnected;
                server.ClientDisconnected += Server_ClientDisconnected;
                server.ListenerClosed += Server_ListenerClosed;

                Log("Success... Listening IP: " + selectedIP + ":4242");
            }
            catch (Exception ex)
            {
                Log("Unable to start server.");
                Log(ex.GetType().ToString());
                Log(ex.Message);
            }

        }

        private void Server_MessageFromClient(object sender, MessageFromClientEventArgs e)
        {
            int clientId = e.ClientId;
            string msg = Encoding.Default.GetString(e.RawMessage);
            Log("Client " + e.ClientId.ToString() + ": " + msg);
        }

        private void Server_ClientConnected(object sender, ClientConnectedEventArgs e)
        {
            int clientId = e.ClientId;
            byte[] message = Encoding.Default.GetBytes("Hallo, ich bin der Server.");

            server.SendMessage(clientId, message);
            Log("Client " + e.ClientId.ToString() + " connected.");
        }

        private void Server_ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            Log("Client " + e.ClientId.ToString() + " Disconnected");
        }

        private void Server_ListenerClosed(object sender, EventArgs e)
        {
            Log("Listener closed.");
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

        private void btnStopServer_Click(object sender, EventArgs e)
        {
            StopServer();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            logActive = false;
            StopServer();
        }

        private void StopServer()
        {
            server?.Stop();
        }
    }
}
