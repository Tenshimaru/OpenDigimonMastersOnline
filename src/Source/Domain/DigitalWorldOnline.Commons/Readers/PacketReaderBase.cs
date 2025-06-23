using System.Text;

namespace DigitalWorldOnline.Commons.Readers
{
    public abstract class PacketReaderBase : IDisposable
    {
        public MemoryStream Packet { get; set; }
        public int Length {get; set; }
        public int Type { get; set; }

        public const int CheckSumValidation = 6716;

        protected PacketReaderBase() => Packet = new();

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

        #region [Read Data]
        public virtual byte ReadByte()
        {
            byte[] buffer = new byte[1];
            Packet.Read(buffer, 0, 1);

            return buffer[0];
        }

        public byte[] ReadBytes(int len)
        {
            byte[] buffer = new byte[len];
            Packet.Read(buffer, 0, len);

            return buffer;
        }

        public virtual short ReadShort()
        {
            byte[] buffer = new byte[2];
            Packet.Read(buffer, 0, 2);

            return BitConverter.ToInt16(buffer, 0);
        }

        public virtual ushort ReadUShort()
        {
            byte[] buffer = new byte[2];
            Packet.Read(buffer, 0, 2);

            return BitConverter.ToUInt16(buffer, 0);
        }

        public virtual int ReadInt()
        {
            byte[] buffer = new byte[4];
            Packet.Read(buffer, 0, 4);

            return BitConverter.ToInt32(buffer, 0);
        }

        public virtual uint ReadUInt()
        {
            byte[] buffer = new byte[4];
            Packet.Read(buffer, 0, 4);

            return BitConverter.ToUInt32(buffer, 0);
        }

        public virtual long ReadInt64()
        {
            byte[] buffer = new byte[8];
            Packet.Read(buffer, 0, 8);

            return BitConverter.ToInt64(buffer, 0);
        }

        public int ReadScan()
        {
            byte[] buffer = new byte[15];
            Packet.Read(buffer, 0, 15);

            return BitConverter.ToInt32(buffer, 0);
        }

        public virtual float ReadFloat()
        {
            byte[] buffer = new byte[4];
            Packet.Read(buffer, 0, 4);

            return BitConverter.ToSingle(buffer, 0);
        }

        public string ReadString()
        {
            int len = Packet.ReadByte();

            byte[] buffer = new byte[len];
            Packet.Read(buffer, 0, len);
            
            return Encoding.ASCII.GetString(buffer).Trim();
        }

        public string ReadZString()
        {
            StringBuilder sb = new();

            while (Packet.CanRead)
            {
                int data = Packet.ReadByte();

                if (data == 0)
                    break;

                sb.Append((char)data);
            }

            return sb.ToString();
        }

        public byte[] ToArray()
        {
            return Packet.ToArray();
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