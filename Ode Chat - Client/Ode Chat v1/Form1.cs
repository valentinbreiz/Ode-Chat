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
            textBox5.Visible = false;
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
            Connexion();
        }

        private void Connexion()
        {
            if (textBox3.Text != "" && textBox4.Text != "")
            {
                try
                {
                    if (checkBox1.Checked == true)
                    {
                        if (textBox5.Text != "")
                        {
                            client = new Client();
                            client.ClientName = textBox3.Text;
                            client.ServerIp = textBox4.Text;
                            client.ServerPort = textBox5.Text;
                            client.OnClientConnected += new OnClientConnectedDelegate(client_OnClientConnected);
                            client.OnClientConnecting += new OnClientConnectingDelegate(client_OnClientConnecting);
                            client.OnClientDisconnected += new OnClientDisconnectedDelegate(client_OnClientDisconnected);
                            client.OnClientError += new OnClientErrorDelegate(client_OnClientError);
                            client.OnClientFileSending += new OnClientFileSendingDelegate(client_OnClientFileSending);
                            client.OnDataReceived += new OnClientReceivedDelegate(client_OnDataReceived);
                            client.Connect();
                            groupBox1.Enabled = false;
                            groupBox2.Enabled = true;
                            textBox2.Focus();

                        }
                        else
                        {
                            MessageBox.Show("Vous devez remplir les champs de texte !");
                        }
                        
                    }
                    else
                    {
                        client = new Client();
                        client.ClientName = textBox3.Text;
                        client.ServerIp = textBox4.Text;
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
                        textBox2.Focus();
                    }
                    
                }
                catch
                {
                    MessageBox.Show("erreur inconnue ;-;");
                }
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
            //peut être après...
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
            try
            {
                client.Disconnect();
                System.Environment.Exit(System.Environment.ExitCode);
            }
            catch
            {
                System.Environment.Exit(System.Environment.ExitCode);
            }
            
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Visible)
            {
                textBox1.SelectionStart = textBox1.TextLength;
                textBox1.ScrollToCaret();
            }
        }

        private void textBox4_KeyDown(object sender, KeyEventArgs e)
        {
            if  (e.KeyCode == Keys.Enter)
            {
                Connexion();
            }
        }


        private void Form1_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            MessageBox.Show("Ode Chat version 0.0.1 \nCréé par valentinbreiz\nCe logiciel utilise l'API NetworksAPI.\nLes sources sont disponibles sur github.com/valentinbreiz", "À propos",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                textBox5.Visible = true;
            }
            else
            {
                textBox5.Visible = false;
            }
        }
    }
}
