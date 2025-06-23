namespace DigitalWorldOnline.Commons.Entities
{
    public class GameClientEvent : EventArgs
    {
        public GameClient Client { get; private set; }

        public GameClientEvent(GameClient client)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
        }
    }
}