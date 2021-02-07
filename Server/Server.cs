using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Collections.Concurrent;
using Packets;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

namespace Server

{
    class Server
    {
        //variables for hangman game
        static int m_Lives = 6;
        static List<char> m_IncorrectLetters;
        static List<char> m_CorrectLetters;
        static string[] m_Words = { "block", "magnitude", "flu", "topple", "vegetarian", "econobox", "glove", "deport", "cupboard", "ratio" };
        static string m_Word;
        static bool m_Won = false;
        bool PlayHangman = false;
        string hangman1, hangman2, hangman3, hangman4, hangman5, hangman6, hangman7;
        char userInput;
        bool correct = false;
        string incorrectGuess;


        TcpListener m_TcpListener;
        UdpClient m_UdpListener;

        private ConcurrentDictionary<int, Client> m_clients;

        public Server(string ipAdress, int port)
        {
            IPAddress ip = IPAddress.Parse(ipAdress);
            m_TcpListener = new TcpListener(ip, port);
            m_UdpListener = new UdpClient(port);

            Thread udpListen = new Thread(() => { UDPListen(); });
            udpListen.Start();
        }

        public void Start()
        {
            m_clients = new ConcurrentDictionary<int, Client>();
            m_TcpListener.Start();

            Timer m_ClientListTimer = new Timer(SendClientList, "", TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10));

            int clientIndex = 0;

            Console.WriteLine("Listening...");
            while (true)
            {
                int index = clientIndex;
                clientIndex++;

                Socket socket = m_TcpListener.AcceptSocket();
                Console.WriteLine("Connection Made");

                Client client = new Client(socket);
                m_clients.TryAdd(index, client);

                Thread thread = new Thread(() => { ClientMethod(index); });
                thread.Start();
            }
        }

        public void Stop()
        {
            m_TcpListener.Stop();
            m_UdpListener.Close();
            
        }

        private void ClientMethod(int index)
        {
           
            try
            {
                Packet packet;

               

                while ((packet = m_clients[index].Read()) != null)
                {
                    if (packet != null)
                    {
                        Console.WriteLine("Recieved...");

                        switch (packet.m_PacketType)
                        {
                            case PacketType.EMPTY:
                                break;

                            case PacketType.CHATMESSAGE:
                                ChatMessagePacket inChatPacket = (ChatMessagePacket)packet;
                                string message = inChatPacket.m_Message;

                                ChatMessagePacket outChatPacket = new ChatMessagePacket(m_clients[index].m_Name + " says: " + inChatPacket.m_Message);
                                foreach (KeyValuePair<int, Client> c in m_clients)
                                {
                                    if (c.Value != m_clients[index])
                                        c.Value.Send(outChatPacket);

                                }
                                break;


                            case PacketType.PRIVATEMESSAGE:
                                PrivateMessagePacket privateMessagePacket = (PrivateMessagePacket)packet;
                                foreach (KeyValuePair<int, Client> c in m_clients)
                                {
                                    if (c.Value.m_Name == privateMessagePacket.m_Name)
                                        c.Value.Send(new PrivateMessagePacket(m_clients[index].m_Name, privateMessagePacket.m_Message));
                                }
                                break;

                            case PacketType.NICKNAME:
                                NickNamePacket namePacket = (NickNamePacket)packet;
                                string NickName = namePacket.m_Name;
                                m_clients[index].m_Name = NickName;
                                m_clients[index].Send( new ChatMessagePacket("Hello! " + m_clients[index].m_Name + " Your nickname has been changed!"));
                                break;

                            case PacketType.LOGIN:
                                LoginPacket loginPacket = (LoginPacket)packet;
                                m_clients[index].m_Name = loginPacket.m_Name;
                                m_clients[index].m_UdpEndPoint = loginPacket.m_EndPoint;
                                break;

                            case PacketType.PLAYHANGMAN:
                                PlayHangmanPacket playHangmanPacket = (PlayHangmanPacket)packet;

                                PlayHangman = true;
                                m_Lives = 6;
                                m_IncorrectLetters = new List<char>();
                                m_CorrectLetters = new List<char>();
                                Random rand = new Random();
                                m_Word = m_Words[rand.Next(0, m_Words.Length)];
                                foreach (KeyValuePair<int, Client> c in m_clients)
                                {

                                    if (c.Value != m_clients[index])
                                    {

                                        c.Value.Send(new PlayHangmanPacket(PlayHangman));

                                    }
                                }


                                HangmanState();

                                m_clients[index].Send(new ChatMessagePacket(hangman1));
                                m_clients[index].Send(new ChatMessagePacket(hangman2));
                                m_clients[index].Send(new ChatMessagePacket(hangman3));
                                m_clients[index].Send(new ChatMessagePacket(hangman4));
                                m_clients[index].Send(new ChatMessagePacket(hangman5));
                                m_clients[index].Send(new ChatMessagePacket(hangman6));
                                m_clients[index].Send(new ChatMessagePacket(hangman7));

                                foreach (KeyValuePair<int, Client> c in m_clients)
                                {
                             
                                    if (c.Value != m_clients[index])
                                    {

                                        c.Value.Send(new ChatMessagePacket(hangman1)); 
                                        c.Value.Send(new ChatMessagePacket(hangman2)); 
                                        c.Value.Send(new ChatMessagePacket(hangman3)); 
                                        c.Value.Send(new ChatMessagePacket(hangman4)); 
                                        c.Value.Send(new ChatMessagePacket(hangman5)); 
                                        c.Value.Send(new ChatMessagePacket(hangman6)); 
                                        c.Value.Send(new ChatMessagePacket(hangman7)); 
                                    } 
                                }
                                break;

                            case PacketType.HANMANGUESS:
                                HangmanGuessPacket hangmanGuessPacket = (HangmanGuessPacket)packet;
                                userInput = hangmanGuessPacket.m_HangmanGuess;

                                correct = false;
                                char guess = userInput;
                                if (!m_Won && m_Lives > 0)
                                {


                                    if (m_IncorrectLetters.Contains(userInput))
                                    {
                                        incorrectGuess = guess + " : has already been guessed";
                                        m_clients[index].Send(new ChatMessagePacket(incorrectGuess));
                                        foreach (KeyValuePair<int, Client> c in m_clients)
                                        {

                                            if (c.Value != m_clients[index])
                                            {

                                                c.Value.Send(new ChatMessagePacket(incorrectGuess));
                                           
                                            }
                                        }
                                    }

                                    for (int i = 0; i < m_Word.Length; ++i)
                                    {
                                        if (Char.ToLower(m_Word[i]) == userInput)
                                        {
                                            correct = true;
                                            m_CorrectLetters.Add(userInput);
                                            break;
                                        }
                                    }

                                    if (!correct)
                                    {
                                        m_Lives--;
                                        m_IncorrectLetters.Add(userInput);
                                    }

                                    //clear
                                    HangmanState();
                                    m_clients[index].Send(new ChatMessagePacket(hangman1));
                                    m_clients[index].Send(new ChatMessagePacket(hangman2));
                                    m_clients[index].Send(new ChatMessagePacket(hangman3));
                                    m_clients[index].Send(new ChatMessagePacket(hangman4));
                                    m_clients[index].Send(new ChatMessagePacket(hangman5));
                                    m_clients[index].Send(new ChatMessagePacket(hangman6));
                                    m_clients[index].Send(new ChatMessagePacket(hangman7));
                                    foreach (KeyValuePair<int, Client> c in m_clients)
                                    {

                                        if (c.Value != m_clients[index])
                                        {

                                            c.Value.Send(new ChatMessagePacket(hangman1));
                                            c.Value.Send(new ChatMessagePacket(hangman2));
                                            c.Value.Send(new ChatMessagePacket(hangman3));
                                            c.Value.Send(new ChatMessagePacket(hangman4));
                                            c.Value.Send(new ChatMessagePacket(hangman5));
                                            c.Value.Send(new ChatMessagePacket(hangman6));
                                            c.Value.Send(new ChatMessagePacket(hangman7));
                                        }
                                    }

                                    // ShowWord();
                                    m_Won = true;
                                    for (int i = 0; i < m_Word.Length; ++i)
                                    {
                                        if (m_CorrectLetters.Contains(Char.ToLower(m_Word[i])))
                                        {
                                            m_clients[index].Send(new ChatMessagePacket(char.ToString(m_Word[i])));
                                            foreach (KeyValuePair<int, Client> c in m_clients)
                                            {

                                                if (c.Value != m_clients[index])
                                                {

                                                    c.Value.Send(new ChatMessagePacket(char.ToString(m_Word[i])));

                                                }
                                            }
                                        }
                                        else
                                        {
                                            m_Won = false;
                                            m_clients[index].Send(new ChatMessagePacket(" _"));
                                            foreach (KeyValuePair<int, Client> c in m_clients)
                                            {

                                                if (c.Value != m_clients[index])
                                                {

                                                    c.Value.Send(new ChatMessagePacket(" _"));

                                                }
                                            }
                                        }
                                    }

                                    m_clients[index].Send(new ChatMessagePacket("Wrong Answers: "));
                                    foreach (KeyValuePair<int, Client> c in m_clients)
                                    {

                                        if (c.Value != m_clients[index])
                                        {

                                            c.Value.Send(new ChatMessagePacket("Wrong Answers: "));

                                        }
                                    }

                                    for (int i = 0; i < m_IncorrectLetters.Count; ++i)
                                    {
                                        m_clients[index].Send(new ChatMessagePacket(char.ToString(m_IncorrectLetters[i]) + " "));
                                        foreach (KeyValuePair<int, Client> c in m_clients)
                                        {

                                            if (c.Value != m_clients[index])
                                            {

                                                c.Value.Send(new ChatMessagePacket(char.ToString(m_IncorrectLetters[i]) + " "));

                                            }
                                        }
                                        Console.Write(m_IncorrectLetters[i] + " ");
                                    }

                                    // ShowWrongAnswers();
                                }
                            
                                if (m_Won)
                                {
                                    m_clients[index].Send(new ChatMessagePacket("You win!"));
                                    foreach (KeyValuePair<int, Client> c in m_clients)
                                    {

                                        if (c.Value != m_clients[index])
                                        {

                                            c.Value.Send(new ChatMessagePacket("You win!"));

                                        }
                                    }

                                    PlayHangman = false;

                                    m_clients[index].Send(new PlayHangmanPacket(PlayHangman));
                                    foreach (KeyValuePair<int, Client> c in m_clients)
                                    {

                                        if (c.Value != m_clients[index])
                                        {

                                            c.Value.Send(new PlayHangmanPacket(PlayHangman));

                                        }
                                    }
                                }

                                if (m_Lives == 0)
                                {
                                    m_clients[index].Send(new ChatMessagePacket("You lose! The correct answer was - " + m_Word));
                                    foreach (KeyValuePair<int, Client> c in m_clients)
                                    {

                                        if (c.Value != m_clients[index])
                                        {

                                            c.Value.Send(new ChatMessagePacket("You lose! The correct answer was - " + m_Word));

                                        }
                                    }
                                    PlayHangman = false;

                                    m_clients[index].Send(new PlayHangmanPacket(PlayHangman));
                                    foreach (KeyValuePair<int, Client> c in m_clients)
                                    {

                                        if (c.Value != m_clients[index])
                                        {

                                            c.Value.Send(new PlayHangmanPacket(PlayHangman));

                                        }
                                    }

                                }
                                

                                break;

                        }

                       
                    }


                }

            }

           catch (Exception e)
            {
                Console.WriteLine("Error occured: " + e.Message);
            }
            finally
            {
                m_clients[index].Close();
                Client c;
                m_clients.TryRemove(index, out c);
            }
        }

        private void UDPListen()
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
            while(true)
            {
                byte[] bytes = m_UdpListener.Receive(ref endPoint);

                MemoryStream mem = new MemoryStream(bytes);
                Packet packet = new BinaryFormatter().Deserialize(mem) as Packet;
                Console.WriteLine("UDP Recieved");

                foreach (KeyValuePair<int, Client> c in m_clients)
                {
                    if(endPoint.ToString() == c.Value.m_UdpEndPoint.ToString())
                    {
                        //stuff for games graphics
                        
                    }
                }

                m_UdpListener.Send(bytes, bytes.Length, endPoint);
            }
        }

        void SendClientList(object state)
        {
            List<string> names = new List<string>();

            foreach (KeyValuePair<int, Client> c in m_clients)
            {
                names.Add(c.Value.m_Name);
            }

            ClientListPacket packet = new ClientListPacket(names.ToArray());

            MemoryStream memStream = new MemoryStream();
            new BinaryFormatter().Serialize(memStream, packet);
            byte[] buffer = memStream.GetBuffer();

            foreach (KeyValuePair<int, Client> c in m_clients)
            {
                m_UdpListener.Send(buffer, buffer.Length, c.Value.m_UdpEndPoint);
            }

        }


        void HangmanState()
        {
           
            hangman1 = ("     |------+  ");
            hangman2 = ("     |      |  ");
            hangman3 = ("     |      " + (m_Lives < 6 ? "O" : ""));
            hangman4 = ("     |     " + (m_Lives < 4 ? "/" : "") + (m_Lives < 5 ? "|" : "") + (m_Lives < 3 ? @"\" : ""));
            hangman5 = ("     |     " + (m_Lives < 2 ? "/" : "") + " " + (m_Lives < 1 ? @"\" : ""));
            hangman6 = ("     |         ");
            hangman7 = ("===============");

        }



    }
}
