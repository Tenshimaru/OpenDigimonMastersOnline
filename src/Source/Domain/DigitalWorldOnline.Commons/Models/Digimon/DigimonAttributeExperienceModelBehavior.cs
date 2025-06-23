using DigitalWorldOnline.Commons.Enums.ClientEnums;

namespace DigitalWorldOnline.Commons.Models.Digimon
{
    public sealed partial class DigimonAttributeExperienceModel
    {
        /// <summary>
        /// Increases element experience.
        /// </summary>
        /// <param name="value">Value to increase.</param>
        /// <param name="element">Target element.</param>
        public void IncreaseElementExperience(short value, DigimonElementEnum element)
        {
            //TODO: eliminar o switch via Reflection
            switch (element)
            {
                case DigimonElementEnum.Neutral:
                default:
                    break;

                case DigimonElementEnum.Ice:
                    {
                        Ice += value;
                        if (Ice > 10000)
                            Ice = 10000;
                    }
                    break;

                case DigimonElementEnum.Water:
                    {
                        Water += value;
                        if (Water > 10000)
                            Water = 10000;
                    }
                    break;

                case DigimonElementEnum.Fire:
                    {
                        Fire += value;
                        if (Fire > 10000)
                            Fire = 10000;
                    }
                    break;

                case DigimonElementEnum.Land:
                    {
                        Land += value;
                        if (Land > 10000)
                            Land = 10000;
                    }
                    break;

                case DigimonElementEnum.Wind:
                    {
                        Wind += value;
                        if (Wind > 10000)
                            Wind = 10000;
                    }
                    break;

                case DigimonElementEnum.Wood:
                    {
                        Wood += value;
                        if (Wood > 10000)
                            Wood = 10000;
                    }
                    break;

                case DigimonElementEnum.Light:
                    {
                        Light += value;
                        if (Light > 10000)
                            Light = 10000;
                    }
                    break;

                case DigimonElementEnum.Dark:
                    {
                        Dark += value;
                        if (Dark > 10000)
                            Dark = 10000;
                    }
                    break;

                case DigimonElementEnum.Thunder:
                    {
                        Thunder += value;
                        if (Thunder > 10000)
                            Thunder = 10000;
                    }
                    break;

                case DigimonElementEnum.Steel:
                    {
                        Steel += value;
                        if (Steel > 10000)
                            Steel = 10000;
                    }
                    break;
            }
        }
        
        /// <summary>
        /// Reduces element experience.
        /// </summary>
        /// <param name="value">Value to decrease.</param>
        /// <param name="element">Target element.</param>
        public void DecreaseElementExperience(short value, DigimonElementEnum element)
        {
            //TODO: eliminar o switch via Reflection
            switch (element)
            {
                case DigimonElementEnum.Neutral:
                default:
                    break;

                case DigimonElementEnum.Ice:
                    Ice -= value;
                    break;

                case DigimonElementEnum.Water:
                    Water -= value;
                    break;

                case DigimonElementEnum.Fire:
                    Fire -= value;
                    break;

                case DigimonElementEnum.Land:
                    Land -= value;
                    break;

                case DigimonElementEnum.Wind:
                    Wind -= value;
                    break;

                case DigimonElementEnum.Wood:
                    Wood -= value;
                    break;

                case DigimonElementEnum.Light:
                    Light -= value;
                    break;

                case DigimonElementEnum.Dark:
                    Dark -= value;
                    break;

                case DigimonElementEnum.Thunder:
                    Thunder -= value;
                    break;

                case DigimonElementEnum.Steel:
                    Steel -= value;
                    break;
            }
        }

        /// <summary>
        /// Increases attribute experience.
        /// </summary>
        /// <param name="value">Value to increase.</param>
        /// <param name="attribute">Target attribute.</param>
        public void IncreaseAttributeExperience(short value, DigimonAttributeEnum attribute)
        {
            switch (attribute)
            {
                case DigimonAttributeEnum.None:
                    None = 0;
                    break;

                case DigimonAttributeEnum.Data:
                    {
                        Data += value;
                        if (Data > 10000)
                            Data = 10000;
                    }
                    break;

                case DigimonAttributeEnum.Vaccine:
                    {
                        Vaccine += value;
                        if (Vaccine > 10000)
                            Vaccine = 10000;
                    }
                    break;

                case DigimonAttributeEnum.Virus:
                    {
                        Virus += value;
                        if (Virus > 10000)
                            Virus = 10000;
                    }
                    break;

                case DigimonAttributeEnum.Unknown:
                    //Fixed by current form
                    break;
            }
        }

        /// <summary>
        /// Reduces attribute experience.
        /// </summary>
        /// <param name="value">Value to decrease.</param>
        /// <param name="attribute">Target attribute.</param>
        public void DecreaseAttributeExperience(short value, DigimonAttributeEnum attribute)
        {
            switch (attribute)
            {
                case DigimonAttributeEnum.None:
                    None = 0;
                    break;

                case DigimonAttributeEnum.Data:
                    Data -= value;
                    break;

                case DigimonAttributeEnum.Vaccine:
                    Vaccine -= value;
                    break;

                case DigimonAttributeEnum.Virus:
                    Virus -= value;
                    break;

                case DigimonAttributeEnum.Unknown:
                    //Fixed by current form
                    break;
            }
        }
    }
}
