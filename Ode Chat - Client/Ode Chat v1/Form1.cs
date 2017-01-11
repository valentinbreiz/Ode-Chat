using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NetworksApi.TCP.CLIENT;

namespace Ode_Chat_v1
{
    public delegate void UpdateText(string txt);

    public partial class Form1 : Form
    {
        Client client;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            groupBox2.Enabled = false;
        }

        private void ChangeTextBox(string txt)
        {
            if (textBox1.InvokeRequired)
            {
                Invoke(new UpdateText(ChangeTextBox), new object[] { txt });
            }
                else
            {
                textBox1.Text += txt + "\r\n";
            }
        }
        

        private void button3_Click(object sender, EventArgs e)
        {
            if(textBox3.Text !="" && textBox4.Text !="")
            {
                client = new Client();
                client.ClientName = textBox4.Text;
                client.ServerIp = textBox3.Text;
                client.ServerPort = "5842";
                client.OnClientConnected += new OnClientConnectedDelegate(client_OnClientConnected);
                client.OnClientConnecting += new OnClientConnectingDelegate(client_OnClientConnecting);
                client.OnClientDisconnected += new OnClientDisconnectedDelegate(client_OnClientDisconnected);
                client.OnClientError += new OnClientErrorDelegate(client_OnClientError);
                client.OnClientFileSending += new OnClientFileSendingDelegate(client_OnClientFileSending);
                client.OnDataReceived += new OnClientReceivedDelegate(client_OnDataReceived);
                client.Connect();
                groupBox1.Enabled = false;
                groupBox2.Enabled = true;
            }
            else
            {
                MessageBox.Show("Vous devez remplir les champs de texte !");
            }
        }

        private void client_OnDataReceived(object Sender, ClientReceivedArguments R)
        {
            ChangeTextBox(R.ReceivedData);
        }

        private void client_OnClientFileSending(object Sender, ClientFileSendingArguments R)
        {
            
        }

        private void client_OnClientError(object Sender, ClientErrorArguments R)
        {
            ChangeTextBox(R.ErrorMessage);
        }

        private void client_OnClientDisconnected(object Sender, ClientDisconnectedArguments R)
        {
            ChangeTextBox(R.EventMessage);
        }

        private void client_OnClientConnecting(object Sender, ClientConnectingArguments R)
        {
            ChangeTextBox(R.EventMessage);

        }

        private void client_OnClientConnected(object Sender, ClientConnectedArguments R)
        {
            ChangeTextBox(R.EventMessage);

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Environment.Exit(System.Environment.ExitCode);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(client != null && client.IsConnected)
            {
                client.Send(textBox2.Text);
                textBox2.Clear();
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (client != null && client.IsConnected && e.KeyCode == Keys.Enter)
            {
                client.Send(textBox2.Text);
                textBox2.Clear();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            groupBox1.Enabled = true;
            groupBox2.Enabled = false;
            client.Disconnect();
        }
    }
}
