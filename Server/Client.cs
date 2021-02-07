using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Collections.Concurrent;
using Packets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;


namespace Server
{
    class Client
    {
        Socket m_Socket;
        NetworkStream m_Stream;
        private BinaryReader m_Reader;
        private BinaryWriter m_Writer;
        private BinaryFormatter m_Formatter;
        public IPEndPoint m_UdpEndPoint;
        private object m_Readlock;
        private object m_Writelock;
        public string m_Name;

        private RSACryptoServiceProvider m_RSAProvider;
        private RSAParameters m_PublicKey;
        private RSAParameters m_PrivateKey;
        private RSAParameters m_ServerKey;

        Encoding u8 = Encoding.UTF8;
      



        public Client(Socket socket)
        {
            m_Writelock = new object();
            m_Readlock = new object();
            m_Socket = socket;
            m_Stream = new NetworkStream(socket);
            m_Reader = new BinaryReader(m_Stream, Encoding.UTF8);
            m_Writer = new BinaryWriter(m_Stream, Encoding.UTF8);
            m_Formatter = new BinaryFormatter();
            m_RSAProvider = new RSACryptoServiceProvider(1024);
            m_PublicKey = m_RSAProvider.ExportParameters(false);
            m_PrivateKey = m_RSAProvider.ExportParameters(true);

        }
        private byte[] Encrypt(byte[] data)
        {
            lock (m_RSAProvider)
            {
                m_RSAProvider.ImportParameters(m_ServerKey);
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
            m_Stream.Close();
            m_Reader.Close();
            m_Writer.Close();
            m_Socket.Close();
        }

        public Packet Read()
        {
            lock (m_Readlock)
            {
                int numberOfBytes;
                if ((numberOfBytes = m_Reader.ReadInt32()) != -1)
                {
                    byte[] buffer = m_Reader.ReadBytes(numberOfBytes);
                   MemoryStream mem = new MemoryStream(buffer);
                    return m_Formatter.Deserialize(mem) as Packet;
                }
                return null;
            }
        }

        public void Send(Packet message)
        {
            lock (m_Writelock)
            {
                // m_Writer.WriteLine("" + message);
                // m_Writer.Flush();
                MemoryStream memoryStream = new MemoryStream();
                m_Formatter.Serialize(memoryStream, message);
                byte[] buffer = memoryStream.GetBuffer();
                m_Writer.Write(buffer.Length);
                m_Writer.Write(buffer);
                m_Writer.Flush();
            }
        }
    }
}
