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
            panel1.Visible = false;
            label2.Visible = false;
            textBox3.Visible = false;
            label3.Visible = false;
            textBox4.Visible = false;
            button5.Visible = false;
            var LocalIP = GetLocalIP();
            this.Text = "Ode Chat - Serveur - " + GetLocalIP();
            
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
            try
            {
                server.BroadCast(R.Name + " est connecté !");
                ChangeListBox(listBox1, R.Name, false);
                ChangeListBox(listBox2, R.Ip, false);
            }
            catch
            {

            }
            
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

        private void button4_Click(object sender, EventArgs e)
        {
            var LocalIP = GetLocalIP();

            if (radioButton1.Checked)
            {
                if (textBox3.Text != "")
                {
                    try
                    {
                        server = new Server(LocalIP, textBox3.Text);
                        this.Text = "Ode Chat - Serveur - " + GetLocalIP() + ":" + textBox3.Text;
                        server.OnClientConnected += new OnConnectedDelegate(server_OnClientConnected);
                        server.OnClientDisconnected += new OnDisconnectedDelegate(server_OnClientDisconnected);
                        server.OnDataReceived += new OnReceivedDelegate(server_OnDataReceived);
                        server.OnServerError += new OnErrorDelegate(server_OnServerError);
                        server.Start();
                        panel1.Visible = true;
                        panel2.Visible = false;
                    }
                    catch
                    {

                    }
                    

                }
                else
                {
                    MessageBox.Show("Vous n'avez pas renseigné de port !");
                    panel2.Visible = true;
                }
                
            }
            else
            {
                server = new Server(LocalIP, "5842");
                this.Text = "Ode Chat - Serveur - " + GetLocalIP() + ":5842";
                server.OnClientConnected += new OnConnectedDelegate(server_OnClientConnected);
                server.OnClientDisconnected += new OnDisconnectedDelegate(server_OnClientDisconnected);
                server.OnDataReceived += new OnReceivedDelegate(server_OnDataReceived);
                server.OnServerError += new OnErrorDelegate(server_OnServerError);
                server.Start();
                panel1.Visible = true;
                panel2.Visible = false;
            }
            if (radioButton4.Checked)
            {
                MessageBox.Show("test2");
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            label2.Visible = true;
            textBox3.Visible = true;
            button5.Visible = true;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            label2.Visible = false;
            textBox3.Visible = false;
            button5.Visible = false;
            textBox3.Clear();
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            label3.Visible = true;
            textBox4.Visible = true;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            label3.Visible = false;
            textBox4.Visible = false;
            textBox4.Clear();
        }


        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsControl(e.KeyChar) || !Char.IsNumber(e.KeyChar))
            {
                e.Handled = true;
                return;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                textBox3.Text = textBox3.Text.Remove(textBox3.Text.Length - 1);

            }
            catch
            {

            }
            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Environment.Exit(System.Environment.ExitCode);
        }
    }
}
