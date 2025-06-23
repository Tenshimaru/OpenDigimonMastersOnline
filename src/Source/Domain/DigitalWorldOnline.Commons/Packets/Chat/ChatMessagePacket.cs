using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Writers;
using System.Net.Sockets;
using System.Reflection;

namespace DigitalWorldOnline.Commons.Packets.Chat
{
    public class ChatMessagePacket : PacketWriter
    {
        private const int PacketNumber = 1006;

        /// <summary>
        /// Sends the message to the target chat.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="chatType">The target chat type.</param>
        /// <param name="sourceHandler">The origin of the message.</param>
        public ChatMessagePacket(string message, ChatTypeEnum chatType, uint sourceHandler)
        {
            Type(PacketNumber);
            WriteByte((byte)chatType);
            WriteByte(1);

            WriteUInt(sourceHandler);
            WriteString(message);
            WriteByte(0);
        }
        
        public ChatMessagePacket(string message, ChatTypeEnum chatType, WhisperResultEnum whisperResult, string senderName, string receiverName)
        {
            Type(PacketNumber);
            WriteByte((byte)chatType);
            WriteByte(1);

            WriteByte((byte)whisperResult);
            WriteString(senderName);
            WriteString(receiverName);
            WriteString(message);
            WriteByte(0);
        }
        
        public ChatMessagePacket(string message, ChatTypeEnum chatType, string senderName)
        {
            Type(PacketNumber);
            WriteByte((byte)chatType);
            WriteByte(1);

            WriteString(senderName);
            WriteString(message);
            WriteByte(0);
        }
        
        public ChatMessagePacket(string message, ChatTypeEnum chatType, string senderName, int itemId, byte senderLevel)
        {
            Type(PacketNumber);
            WriteByte((byte)chatType);
            WriteByte(1);

            WriteString(senderName);
            WriteString(message);
            WriteInt(itemId);
            WriteShort(senderLevel);
        }
    }
}