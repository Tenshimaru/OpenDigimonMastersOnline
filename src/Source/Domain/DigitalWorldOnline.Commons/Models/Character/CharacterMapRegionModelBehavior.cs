namespace DigitalWorldOnline.Commons.Models.Character
{
    public sealed partial class CharacterMapRegionModel
    {
        /// <summary>
        /// Unlocks the region.
        /// </summary>
        public void Unlock() => Unlocked = 128;

        /// <summary>
        /// Serializes the map region object.
        /// </summary>
        public byte[] ToArray()
        {
            using (MemoryStream m = new ())
            {
                m.Write(BitConverter.GetBytes((short)Unlocked), 0, 1); 
               
                return m.ToArray();
            }
        }
    }
}