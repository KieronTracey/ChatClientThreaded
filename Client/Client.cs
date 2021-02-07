using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Net;
using Packets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing;
using System.Security.Cryptography;

namespace Client
{
    class Client
    {
        private TcpClient m_TcpClient;
        private UdpClient m_udpClient;
        private NetworkStream m_Stream;
        private BinaryWriter m_Writer;
        private BinaryReader m_Reader;
        private BinaryFormatter m_Formatter;
        private Thread m_ReadingThread;
        private Thread m_TcpReadingThread;
        private Thread m_UdpReadingThread;
        private ClientForm m_clientForm;
        Socket m_Socket;
        private RSACryptoServiceProvider m_RSAProvider;
        private RSAParameters m_PublicKey;
        private RSAParameters m_PrivateKey;
        private RSAParameters m_ClientKey;


        public Client()
        {
            m_Formatter = new BinaryFormatter();
            m_RSAProvider = new RSACryptoServiceProvider(1024);
            m_PublicKey = m_RSAProvider.ExportParameters(false);
            m_PrivateKey = m_RSAProvider.ExportParameters(true);
        }

        private byte[] Encrypt(byte[] data)
        {
            lock (m_RSAProvider)
            {
                m_RSAProvider.ImportParameters(m_ClientKey);
                return m_RSAProvider.Encrypt(data, true);
            }
        }

        private byte[] Decrypt(byte[] data)
        {
            lock (m_RSAProvider)
            {
                m_RSAProvider.ImportParameters(m_PrivateKey);
                return m_RSAProvider.Decrypt(data, true);
            }
        }

        private byte[] EncryptString(string message)
        {
            byte[] msg = UTF8Encoding.UTF8.GetBytes(message);
            Encrypt(msg);
            return msg;
        }

        private string DecryptString(byte[] message)
        {
            byte[] newmessage = Decrypt(message);
            string msg = UTF8Encoding.UTF8.GetString(newmessage);
            return msg;
        }


        public void Close()
        {
            if (m_TcpClient.Connected)
            {
                m_TcpClient.GetStream().Close();
                m_TcpClient.Close();
            }

            m_udpClient.Close();
        }

        public void SendMessage(Packet packet)
        {
            MemoryStream memStream = new MemoryStream();
            m_Formatter.Serialize(memStream, packet);
            byte[] buffer = memStream.GetBuffer();
            m_Writer.Write(buffer.Length);
            m_Writer.Write(buffer);
            m_Writer.Flush();
        }

        public void Login(String name)
        {
            SendMessage(new LoginPacket((IPEndPoint)m_udpClient.Client.LocalEndPoint, name));
        }

        public void UdpSendMessage(Packet packet)
        {
            MemoryStream memStream = new MemoryStream();
            m_Formatter.Serialize(memStream, packet);
            byte[] buffer = memStream.GetBuffer();
            m_udpClient.Send(buffer, buffer.Length);
        }

        private void TcpProcessServerResponse()
        {
            try
            {
                int numberOfBytes;
                while ((numberOfBytes = m_Reader.ReadInt32()) != 0)
                {
                    byte[] buffer = m_Reader.ReadBytes(numberOfBytes);
                    MemoryStream memStream = new MemoryStream(buffer);
                    Packet packet = m_Formatter.Deserialize(memStream) as Packet;

                    switch (packet.m_PacketType)
                    {
                        case PacketType.EMPTY:
                            break;

                        case PacketType.CHATMESSAGE:
                            ChatMessagePacket chatPacket = packet as ChatMessagePacket;
                            m_clientForm.UpdateChatWindow(chatPacket.m_Message, Color.Black);
                            break;

                        case PacketType.PRIVATEMESSAGE:
                            PrivateMessagePacket privateChatPacket = packet as PrivateMessagePacket;
                            m_clientForm.UpdateChatWindow("Whisper from " + privateChatPacket.m_Name + ": " + privateChatPacket.m_Message, Color.PaleVioletRed);
                            break;

                        case PacketType.NICKNAME:
                            NickNamePacket nickNamePacket = packet as NickNamePacket; 
                            break;

                        case PacketType.PLAYHANGMAN:
                            PlayHangmanPacket playHangmanPacket = packet as PlayHangmanPacket;
                            bool PlayingHangman = playHangmanPacket.m_PlayHangman;

                            if (PlayingHangman)
                            {
                                m_clientForm.YesHangMan();
                            }
                            else
                            {
                                m_clientForm.NoHangMan();
                            }                       
                           
                            break;  

                        default:
                            break;
                    }
                }
            }
            catch(IOException e)
            {
                Console.WriteLine("Tcp Read error on Client: " + e.Message);
            }
        }

        private void UdpProcessServerResponse()
        {
            try
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
                while(true)
                {
                    byte[] bytes = m_udpClient.Receive(ref endPoint);

                    MemoryStream mem = new MemoryStream(bytes);
                    Packet packet = new BinaryFormatter().Deserialize(mem) as Packet;

                    switch (packet.m_PacketType)
                    {
                        case PacketType.CLIENTLIST:
                            ClientListPacket clientPacket = packet as ClientListPacket;
                            string clientList = "Connected Clients: ";

                            for (int i = 0; i < clientPacket.m_ClientList.Length; i++)
                            {
                                clientList += Environment.NewLine;
                                clientList += (i + 1) + " - " + clientPacket.m_ClientList[i];
                            }

                            m_clientForm.UpdateClientListWindow(clientList, Color.Turquoise);
                            break;

                        
                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("UDP Read error on client: " + e.Message);
            }
        }
        

        public bool Connect(string ip, int port)
        {
            try
            {
                if (m_TcpClient == null)
                {
                    m_TcpClient = new TcpClient();
                    m_TcpClient.Connect(ip, port);
                    m_Stream = m_TcpClient.GetStream();
                    m_Writer = new BinaryWriter(m_Stream, Encoding.UTF8);
                    m_Reader = new BinaryReader(m_Stream, Encoding.UTF8);
                    m_Formatter = new BinaryFormatter();

                    m_udpClient = new UdpClient();
                    m_udpClient.Connect(ip, port);

                    // m_ReadingThread = new Thread(() => { ProcessServerResponse(); });
                    // m_ReadingThread.Start();

                    m_TcpReadingThread = new Thread(() => { TcpProcessServerResponse(); });
                    m_TcpReadingThread.Start();

                    m_UdpReadingThread = new Thread(() => { UdpProcessServerResponse(); });
                    m_UdpReadingThread.Start();

                    Console.WriteLine("Connection Made");
                    Console.WriteLine("The IP Address of the server is" + ((IPEndPoint)m_TcpClient.Client.LocalEndPoint).Address
                        + " on port" + ((IPEndPoint)m_TcpClient.Client.LocalEndPoint).Port);
                    Console.WriteLine("The IP Address of the new connection is" + ((IPEndPoint)m_TcpClient.Client.RemoteEndPoint).Address
                    + " on port" + ((IPEndPoint)m_TcpClient.Client.RemoteEndPoint).Port);

                    return true;
                }
                else
                {
                    Close();
                    m_TcpClient = null;
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                return false;
            }
        }

        public void Run()
        {
            if (!m_TcpClient.Connected)
            {
                throw new Exception();
            }

            try
            {
               // string userInput;

                m_clientForm = new ClientForm(this);
                m_clientForm.ShowDialog();

                Console.Write("enter the data to be sent; ");

               // while ((userInput = Console.ReadLine()) != null)
               // {
                 //   m_Writer.WriteLine(userInput);
                 //   m_Writer.Flush();

                   // ProcessServerResponse();

                  //  if (userInput.ToLower() == "bye")
                 //   {
                        //break;
                  //  }

                 //   Console.Write("Enter the data to be sent: ");
               // }
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected Error: " + e.Message);
            }
            finally
            {
                m_TcpClient.Close();
            }
            Console.Read();
        }

        private void ProcessServerResponse()
        {
            try
            {
                string message;
                while ((message = m_Reader.ReadString()) != null)
                {
                    m_clientForm.UpdateChatWindow(message, Color.Black);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

    }
}
