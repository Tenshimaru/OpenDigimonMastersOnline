using System.Text;

namespace DigitalWorldOnline.Commons.Writers
{
    public abstract class PacketWriterBase : IDisposable
    {
        public MemoryStream Packet { get; set; }
        public int Length => (int)Packet.Length;

        public const int CheckSumValidation = 6716;

        public PacketWriterBase()
        {
            Packet = new ();
        }

        #region [Position]
        public virtual void Seek(long position)
        {
            Packet.Seek(position, SeekOrigin.Begin);
        }

        public virtual void Skip(long bytes)
        {
            Packet.Seek(bytes, SeekOrigin.Current);
        }
        #endregion

        #region [Write Data]
        public void Type(int type)
        {
            Packet.Write(BitConverter.GetBytes(type), 0, 2);
        }

        public void WriteByte(byte value)
        {
            Packet.Write(BitConverter.GetBytes((short)value), 0, 1);
        }

        public void WriteBytes(byte[] buffer)
        {
            Packet.Write(buffer, 0, buffer.Length);
        }

        public void WriteShort(short value)
        {
            Packet.Write(BitConverter.GetBytes(value), 0, 2);
        }

        public void WriteUShort(ushort value)
        {
            Packet.Write(BitConverter.GetBytes(value), 0, 2);
        }

        public void WriteInt(int value)
        {
            Packet.Write(BitConverter.GetBytes(value), 0, 4);
        }
        
        public void WriteInt(int value, int pos)
        {
            Packet.Seek(pos, SeekOrigin.Begin);
            Packet.Write(BitConverter.GetBytes(value), 0, 4);
        }

        public void WriteUInt(uint value)
        {
            Packet.Write(BitConverter.GetBytes(value), 0, 4);
        }
        
        public void WriteUInt(uint value, int pos)
        {
            Packet.Seek(pos, SeekOrigin.Begin);
            Packet.Write(BitConverter.GetBytes(value), 0, 4);
        }

        public void WriteInt64(long value)
        {
            Packet.Write(BitConverter.GetBytes(value), 0, 8);
        }

        public void WriteUInt64(ulong value)
        {
            Packet.Write(BitConverter.GetBytes(value), 0, 8);
        }

        public void WriteString(string value)
        {
            if (value == null) value = String.Empty;
            byte[] buffer = Encoding.ASCII.GetBytes(value);
            Packet.WriteByte((byte)buffer.Length);
            Packet.Write(buffer, 0, buffer.Length);
            Packet.WriteByte(0);
        }

        public void WriteString(string value, int pos)
        {
            Packet.Seek(pos, SeekOrigin.Begin);
            byte[] buffer = Encoding.ASCII.GetBytes(value);
            Packet.WriteByte((byte)value.Length);
            Packet.Write(buffer, 0, buffer.Length);
            Packet.WriteByte(0);
        }

        public void WriteFloat(float value)
        {
            Packet.Write(BitConverter.GetBytes(value), 0, 4);
        }

        #endregion

        public void Dispose()
        {
            Packet.Close();
            Packet.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}