using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NetworksApi.TCP.SERVER;
using System.Net;
using System.Net.Sockets;


namespace Ode_Chat___Server
{
    public delegate void UpdateChatLog(string txt);
    public delegate void UpdateListBox(ListBox box, string value, bool Remove);

    public partial class Form1 : Form
    {
        Server server;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var LocalIP = GetLocalIP();
            this.Text = "Ode Chat - Serveur - " + GetLocalIP();
            server = new Server(LocalIP, "5842");
            server.OnClientConnected += new OnConnectedDelegate(server_OnClientConnected);
            server.OnClientDisconnected += new OnDisconnectedDelegate(server_OnClientDisconnected);
            server.OnDataReceived += new OnReceivedDelegate(server_OnDataReceived);
            server.OnServerError += new OnErrorDelegate(server_OnServerError);
            server.Start();
        }

        private string GetLocalIP()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "127.0.0.1";
        }

        private void ChangeChatLog(string txt)
        {
            if (textBox1.InvokeRequired)
            {
                Invoke(new UpdateChatLog(ChangeChatLog), new object[] { txt });
            }
            else
            {
                textBox1.Text += txt + "\r\n";
            }
        }
        private void ChangeListBox(ListBox box, string value, bool Remove)
        {
            if (box.InvokeRequired)
            {
                Invoke(new UpdateListBox(ChangeListBox), new object[] { box, value, Remove});
            }
            else
            {
                if (Remove)
                {
                    box.Items.Remove(value);
                }
                else
                {
                    box.Items.Add(value);
                }
                
            }
        }

        private void server_OnServerError(object Sender, ErrorArguments R)
        {
            MessageBox.Show(R.ErrorMessage);
        }

        private void server_OnDataReceived(object Sender, ReceivedArguments R)
        {
            ChangeChatLog(R.ReceivedData);
            server.BroadCast(R.Name + "> " + R.ReceivedData);
        }

        private void server_OnClientDisconnected(object Sender, DisconnectedArguments R)
        {
            server.BroadCast(R.Name + " est déconnecté !");
            ChangeListBox(listBox1, R.Name, true);
            ChangeListBox(listBox2, R.Ip, true);
        }

        private void server_OnClientConnected(object Sender, ConnectedArguments R)
        {
            server.BroadCast(R.Name + " est connecté !");
            ChangeListBox(listBox1, R.Name, false);
            ChangeListBox(listBox2, R.Ip, false);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            server.SendTo((string)listBox1.SelectedItem, "serveur> " + textBox2.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            server.BroadCast("serveur> " + textBox2.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            server.DisconnectClient((string)listBox1.SelectedItem);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Environment.Exit(System.Environment.ExitCode);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox2.SelectedIndex = listBox1.SelectedIndex;
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.SelectedIndex = listBox2.SelectedIndex;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Visible)
            {
                textBox1.SelectionStart = textBox1.TextLength;
                textBox1.ScrollToCaret();
            }
        }
    }
}
