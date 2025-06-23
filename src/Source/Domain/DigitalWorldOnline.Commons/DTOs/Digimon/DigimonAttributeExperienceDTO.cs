namespace DigitalWorldOnline.Commons.DTOs.Digimon
{
    public sealed partial class DigimonAttributeExperienceDTO
    {
        //TODO: separar nature de element
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Data attribute experience.
        /// </summary>
        public short Data { get; set; }

        /// <summary>
        /// Vaccine attribute experience.
        /// </summary>
        public short Vaccine { get; set; }

        /// <summary>
        /// Virus attribute experience.
        /// </summary>
        public short Virus { get; set; }
        
        /// <summary>
        /// Ice attribute experience.
        /// </summary>
        public short Ice { get; set; }
        
        /// <summary>
        /// Water attribute experience.
        /// </summary>
        public short Water { get; set; }
        
        /// <summary>
        /// Fire attribute experience.
        /// </summary>
        public short Fire { get; set; }
        
        /// <summary>
        /// Land attribute experience.
        /// </summary>
        public short Land { get; set; }
        
        /// <summary>
        /// Wind attribute experience.
        /// </summary>
        public short Wind { get; set; }
        
        /// <summary>
        /// Wood attribute experience.
        /// </summary>
        public short Wood { get; set; }
        
        /// <summary>
        /// Light attribute experience.
        /// </summary>
        public short Light { get; set; }
        
        /// <summary>
        /// Dark attribute experience.
        /// </summary>
        public short Dark { get; set; }
        
        /// <summary>
        /// Thunder attribute experience.
        /// </summary>
        public short Thunder { get; set; }
        
        /// <summary>
        /// Metal attribute experience.
        /// </summary>
        public short Steel { get; set; }

        /// <summary>
        /// Reference to the digimon owner of this clone.
        /// </summary>
        public long DigimonId { get; set; }
        public DigimonDTO Digimon { get; set; }
    }
}
