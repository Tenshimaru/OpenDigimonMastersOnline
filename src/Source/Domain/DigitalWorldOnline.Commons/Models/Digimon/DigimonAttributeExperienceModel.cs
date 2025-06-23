namespace DigitalWorldOnline.Commons.Models.Digimon
{
    public sealed partial class DigimonAttributeExperienceModel
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Reference to the digimon owner of this clone.
        /// </summary>
        public long DigimonId { get; private set; }

        /// <summary>
        /// None attribute experience.
        /// </summary>
        public short None { get; private set; }
        
        /// <summary>
        /// Data attribute experience.
        /// </summary>
        public short Data { get; private set; }

        /// <summary>
        /// Vaccine attribute experience.
        /// </summary>
        public short Vaccine { get; private set; }

        /// <summary>
        /// Virus attribute experience.
        /// </summary>
        public short Virus { get; private set; }
        
        /// <summary>
        /// Unknown attribute experience.
        /// </summary>
        public short Unknown { get; private set; }
        
        /// <summary>
        /// Ice attribute experience.
        /// </summary>
        public short Ice { get; private set; }
        
        /// <summary>
        /// Water attribute experience.
        /// </summary>
        public short Water { get; private set; }
        
        /// <summary>
        /// Fire attribute experience.
        /// </summary>
        public short Fire { get; private set; }
        
        /// <summary>
        /// Land attribute experience.
        /// </summary>
        public short Land { get; private set; }
        
        /// <summary>
        /// Wind attribute experience.
        /// </summary>
        public short Wind { get; private set; }
        
        /// <summary>
        /// Wood attribute experience.
        /// </summary>
        public short Wood { get; private set; }
        
        /// <summary>
        /// Light attribute experience.
        /// </summary>
        public short Light { get; private set; }
        
        /// <summary>
        /// Dark attribute experience.
        /// </summary>
        public short Dark { get; private set; }
        
        /// <summary>
        /// Thunder attribute experience.
        /// </summary>
        public short Thunder { get; private set; }
        
        /// <summary>
        /// Steel attribute experience.
        /// </summary>
        public short Steel { get; private set; }


        public bool CurrentAttributeExperience  => Data >= 10000 || Vaccine >= 10000 || Virus >= 10000;
        public bool CurrentElementExperience => Unknown >= 10000 || Ice >= 10000 || Water >= 10000 || Fire >= 10000 || Land >= 10000 || Wind >= 10000 || Wood >= 10000 || Light >= 10000 || Dark >= 10000 || Thunder >= 10000 || Steel >= 10000;
    }
}
