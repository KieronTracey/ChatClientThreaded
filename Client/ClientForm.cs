using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Packets;


namespace Client
{
    partial class ClientForm : Form

    {
        Client m_Client;
        bool playhangman;

 


        public ClientForm(Client client)
        {
            InitializeComponent();
            m_Client = client;
            InputField.Select();
            string name;
            name = WhisperBox1.Text;
            m_Client.Login(name);
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            if (playhangman)
            {
                char guess = char.Parse(InputField.Text);
                m_Client.SendMessage(new HangmanGuessPacket(guess));
                InputField.Clear();
            }
            else
            {
                string message = InputField.Text;

                m_Client.SendMessage(new ChatMessagePacket(message));
                InputField.Clear();
                UpdateChatWindow(message, Color.Black);
            }
            
        }

        public void UpdateChatWindow(string message, Color color)
        {
            if (MessageWindow.InvokeRequired)

            {

                Invoke(new Action(() =>

                {
                    UpdateChatWindow(message, color);
                }));

            }

            else

            {
                MessageWindow.Text += message + Environment.NewLine;
                MessageWindow.SelectionStart = MessageWindow.Text.Length;
                MessageWindow.ScrollToCaret();
            }
        }

        private void MessageWindow_TextChanged(object sender, EventArgs e)
        {
            MessageWindow.ReadOnly = true;
        }

        private void DisconnectButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void InputField_TextChanged(object sender, EventArgs e)
        {

        }

        private void NickNameBox_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void ChangeNicknameButton_Click(object sender, EventArgs e)
        {
            string name = NickNameBox.Text;
            m_Client.SendMessage(new NickNamePacket(name));
            
        }

        private void WhisperBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void WhisperButton_Click(object sender, EventArgs e)
        {
            string name = WhisperBox1.Text;
            string message = InputField.Text;

            m_Client.SendMessage(new PrivateMessagePacket(name, message));
            InputField.Clear();
            UpdateChatWindow(message, Color.PaleVioletRed);
        }

        private void ConnectionsLabel_Click(object sender, EventArgs e)
        {

        }

        private void ClientList_TextChanged(object sender, EventArgs e)
        {
            ClientList.ReadOnly = true;
        }

        public void UpdateClientListWindow(string message, Color color)
        {
            if (MessageWindow.InvokeRequired)

            {

                Invoke(new Action(() =>

                {
                    UpdateClientListWindow(message, color);
                }));

            }

            else

            {
                ClientList.Clear();
                ClientList.Text += message + Environment.NewLine;
                MessageWindow.SelectionStart = MessageWindow.Text.Length;
                MessageWindow.ScrollToCaret();
            }
        }

        private void PlayHangmanButton_Click(object sender, EventArgs e)
        {
            playhangman = true;
            bool playHangman = true;
            m_Client.SendMessage(new PlayHangmanPacket(playHangman));
        }

        public void NoHangMan()
        {
            playhangman = false;
        }

        public void YesHangMan()
        {
            playhangman = true;
        }
    }
}
