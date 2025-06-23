namespace DigitalWorldOnline.Commons.Writers
{
    public class PacketWriter : PacketWriterBase
    {
        public byte[]? Buffer { get; set; }

        public PacketWriter() : base()
        {
            Packet = new ();
            Packet.Write(new byte[] { 0, 0 }, 0, 2);
        }

        public byte[] Serialize()
        {
            if (Buffer == null)
            {
                WriteShort(0);

                byte[] buffer = Packet.ToArray();
                byte[] len = BitConverter.GetBytes((short)buffer.Length);
                byte[] checksum = BitConverter.GetBytes((short)(buffer.Length ^ CheckSumValidation));

                len.CopyTo(buffer, 0);
                checksum.CopyTo(buffer, buffer.Length - 2);

                Packet.Close();

                Buffer = buffer;
            }

            return Buffer;
        }
    }
}