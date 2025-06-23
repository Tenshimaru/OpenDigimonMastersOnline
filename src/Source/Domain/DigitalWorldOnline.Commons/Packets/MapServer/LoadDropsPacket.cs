using DigitalWorldOnline.Commons.Models.Map;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.MapServer
{
    public class LoadDropsPacket : PacketWriter
    {
        private const int PacketNumber = 1006;
                
        /// <summary>
        /// Loads the target drop.
        /// </summary>
        /// <param name="drop">The drop to be loaded.</param>
        public LoadDropsPacket(Drop drop)
        {
            Type(PacketNumber);
            WriteByte(3);
            WriteShort(1);
            WriteInt(drop.Location.X);
            WriteInt(drop.Location.Y);
            WriteUInt((uint)drop.GeneralHandler);
            WriteInt(drop.DropInfo.ItemId);
            WriteInt(drop.OwnerHandler);
            WriteByte(0);
            WriteInt(0);
        }
        
        public LoadDropsPacket(Drop drop, int viewHandler)
        {
            Type(PacketNumber);
            WriteByte(3);
            WriteShort(1);
            WriteInt(drop.Location.X);
            WriteInt(drop.Location.Y);
            WriteUInt((uint)drop.GeneralHandler);
            WriteInt(drop.DropInfo.ItemId);
            WriteInt(drop.NoOwner ? viewHandler : drop.OwnerHandler);
            WriteByte(0); //Forma (Somente Impar/Par)
            WriteInt(0);
        }
    }
}