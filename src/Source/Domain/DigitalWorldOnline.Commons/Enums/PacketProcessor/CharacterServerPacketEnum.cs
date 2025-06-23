namespace DigitalWorldOnline.Commons.Enums.PacketProcessor
{
    public enum CharacterServerPacketEnum
    {
        /// <summary>
        /// Unknown packet
        /// </summary>
        Unknown = -99,

        /// <summary>
        /// To avoid connection break/interrupt, the client sends this often.
        /// </summary>
        KeepConnection = -3,

        /// <summary>
        /// Request connection with the server.
        /// </summary>
        Connection = -1,

        /// <summary>
        /// Checks if the selected character name is available.
        /// </summary>
        CheckNameDuplicity = 1302,

        /// <summary>
        /// Creates a new character and partner.
        /// </summary>
        CreateCharacter = 1303,

        /// <summary>
        /// Removes the selected character and digimons.
        /// </summary>
        DeleteCharacter = 1304,

        GetCharacterPosition = 1305,

        /// <summary>
        /// Requests the existent characters of the account.
        /// </summary>
        RequestCharacters = 1706,

        /// <summary>
        /// Connects to the Game server.
        /// </summary>
        ConnectGameServer = 1703
    }
}
