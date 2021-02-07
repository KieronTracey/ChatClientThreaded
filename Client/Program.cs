using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    class Program
    {
        private const string m_ServerIP = "127.0.0.1";
        private const int m_ServerPort = 4444;

        static void Main(string[] args)
        {
            Client client = new Client();

            if (client.Connect(m_ServerIP, m_ServerPort))
            {
                Console.WriteLine("Connected... ");

                try
                {
                    client.Run();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
            else
            {
                Console.WriteLine("Failed to connect to: " + m_ServerIP + " : " + m_ServerPort);
            }
            Console.Read();
        }
    }
}
