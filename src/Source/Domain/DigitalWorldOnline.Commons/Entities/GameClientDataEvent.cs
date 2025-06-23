namespace DigitalWorldOnline.Commons.Entities
{
    public sealed class GameClientDataEvent : GameClientEvent
    {
        public byte[] Data { get; private set; }

        public GameClientDataEvent(GameClient client, byte[] data) : base(client)
        {
            Data = data ?? Array.Empty<byte>();
        }
    }
}
