using System;
using System.Net;
using System.Security.Cryptography;

namespace Packets
{
    public enum PacketType
    {
        EMPTY,
        CHATMESSAGE,
        PRIVATEMESSAGE,
        NICKNAME,
        LOGIN,
        CLIENTLIST,
        PLAYHANGMAN,
        CLEARCHATWINDOW,
        HANMANGUESS
    }

    [Serializable]
    public abstract class Packet
    {
        public PacketType m_PacketType { get; protected set; }
    }

    [Serializable]
    public class ChatMessagePacket : Packet
    {
        public string m_Message { get; private set; }
        public ChatMessagePacket(string message)
        {
            m_Message = message;
            m_PacketType = PacketType.CHATMESSAGE;
        }
    }

    [Serializable]
    public class NickNamePacket : Packet
    {
        public string m_Name { get; private set; }
        public NickNamePacket(string name)
        {
            m_Name = name;
            m_PacketType = PacketType.NICKNAME;
        }
    }

    [Serializable]
    public class PrivateMessagePacket : Packet
    {
        
        public string m_Name { get; private set; }
        public string m_Message { get; private set; }
        public PrivateMessagePacket(string name, string privateMessage)
        {
            m_Name = name;
            m_Message = privateMessage;
            m_PacketType = PacketType.PRIVATEMESSAGE;
        }
    }

    [Serializable]
    public class LoginPacket : Packet
    {
        public string m_Name { get; private set; }
        public IPEndPoint m_EndPoint { get; private set; }      
        public LoginPacket(IPEndPoint EndPoint, string name)
        {
            m_EndPoint = EndPoint;
            m_PacketType = PacketType.LOGIN;
        }
    }

    [Serializable]
    public class ClientListPacket : Packet
    {
        public string[] m_ClientList { get; private set; }
        public ClientListPacket(string[] clientList)
        {
            m_ClientList = clientList;
            m_PacketType = PacketType.CLIENTLIST;
        }
    }

    [Serializable]
    public class PlayHangmanPacket : Packet
    {
        public bool m_PlayHangman { get; private set; }
        public PlayHangmanPacket(bool playHangman)
        {
            m_PlayHangman = playHangman;
            m_PacketType = PacketType.PLAYHANGMAN;
        }
    }


    [Serializable]
    public class HangmanGuessPacket : Packet
    {
        public char m_HangmanGuess { get; private set; }
        public HangmanGuessPacket(char hangmanGuess)
        {
            m_HangmanGuess = hangmanGuess;
            m_PacketType = PacketType.HANMANGUESS;
        }
    }

    [Serializable]
    public class ClearChatWindowPacket : Packet
    {
        public ClearChatWindowPacket()
        {
        }
    }


}


 

