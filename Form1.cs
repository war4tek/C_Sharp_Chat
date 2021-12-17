using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace Chat
{
    public partial class Form1 : Form
    {
        private TcpClient client;
        public StreamReader str;
        public StreamWriter stw;
        public string receive;
        public string message;

        public Form1()
        {
            InitializeComponent();

            IPAddress[] localIp = Dns.GetHostAddresses(Dns.GetHostName());
            foreach(IPAddress address in localIp)
            { 
                if(address.AddressFamily == AddressFamily.InterNetwork)
                {
                    serverIpTextBox.Text = address.ToString();
                }
            }
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, int.Parse(portTextBox.Text));
            listener.Start();
            client = listener.AcceptTcpClient();
            str = new StreamReader(client.GetStream());
            stw = new StreamWriter(client.GetStream());
            stw.AutoFlush = true;

            backgroundWorker1.RunWorkerAsync();
            backgroundWorker2.WorkerSupportsCancellation = true;
        }

        private void ClientConnectBtn_Click(object sender, EventArgs e)
        {
            client = new TcpClient();
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(clientIPTextBox.Text), int.Parse(clientPortTextBox.Text));

            try
            {
                client.Connect(endPoint);
                if(client.Connected)
                {
                    chatWindowTextBox.AppendText("Connected to Server" + Environment.NewLine);
                    str = new StreamReader(client.GetStream());
                    stw = new StreamWriter(client.GetStream());

                    stw.AutoFlush = true;
                    backgroundWorker1.RunWorkerAsync();
                    backgroundWorker2.WorkerSupportsCancellation = true;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while(client.Connected)
            {
                try
                {
                    receive = str.ReadLine();
                    this.chatWindowTextBox.Invoke(new MethodInvoker(delegate ()
                    {
                        chatWindowTextBox.AppendText("Julian:" + receive + Environment.NewLine);
                    }));
                    receive = "";
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void BackgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            if(client.Connected)
            {
                stw.WriteLine(message);
                this.chatWindowTextBox.Invoke(new MethodInvoker(delegate ()
                {
                    chatWindowTextBox.AppendText("Angelo:" + message + Environment.NewLine);
                }));
           
            }
            else
            {
                MessageBox.Show("Sending failed");
            }
            backgroundWorker2.CancelAsync();
        }

        private void SendBtn_Click(object sender, EventArgs e)
        {
            if (messageTextBox.Text != "")
            {
                message = messageTextBox.Text.Trim();
                backgroundWorker2.RunWorkerAsync();
            }
            messageTextBox.Text = "";
        }
    }
}
