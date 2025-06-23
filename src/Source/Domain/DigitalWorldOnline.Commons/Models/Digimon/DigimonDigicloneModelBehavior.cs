using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.ClientEnums;

namespace DigitalWorldOnline.Commons.Models.Digimon
{
    public sealed partial class DigimonDigicloneModel
    {
        /// <summary>
        /// Returns the total clone level of all attributes.
        /// </summary>
        public short CloneLevel => (short)(1 + ATLevel + 
            BLLevel + CTLevel + EVLevel + HPLevel);

        /// <summary>
        /// Flag for max clone level.
        /// </summary>
        public bool MaxCloneLevel => CloneLevel == 76;

        /// <summary>
        /// Gets the current level of the target clone.
        /// </summary>
        /// <param name="type">Clone type enumeration.</param>
        public byte GetCurrentLevel(DigicloneTypeEnum type)
        {
            return type switch
            {
                DigicloneTypeEnum.AT => ATLevel,
                DigicloneTypeEnum.BL => BLLevel,
                DigicloneTypeEnum.CT => CTLevel,
                DigicloneTypeEnum.HP => HPLevel,
                DigicloneTypeEnum.EV => EVLevel,
                _ => 0,
            };
        }

        /// <summary>
        /// Increases target clone level and value.
        /// </summary>
        /// <param name="type">Target clone type.</param>
        /// <param name="value">Value to increase.</param>
        public void IncreaseCloneLevel(DigicloneTypeEnum type, short value)
        {
            switch (type)
            {
                case DigicloneTypeEnum.AT:
                    {
                        ATLevel++;
                        ATValue += value;
                    }
                    break;

                case DigicloneTypeEnum.BL:
                    {
                        BLLevel++;
                        BLValue += value;
                    }
                    break;

                case DigicloneTypeEnum.CT:
                    {
                        CTLevel++;
                        CTValue += value;
                    }
                    break;

                case DigicloneTypeEnum.HP:
                    {
                        HPLevel++;
                        HPValue += value;
                    }
                    break;

                case DigicloneTypeEnum.EV:
                    {
                        EVLevel++;
                        EVValue += value;
                    }
                    break;
            }
            
            Console.WriteLine($"Digiclone increased {type} level to {CloneLevel}.");
            History.AddEntry(type, value);
        }

        public void Break(DigicloneTypeEnum cloneType)
        {
            switch (cloneType)
            {
                case DigicloneTypeEnum.AT:
                    {
                        var lastValue = History.ATValues.LastOrDefault(x => x > 0);
                        if (lastValue > 0)
                        {
                            ATValue -= (short)lastValue;

                            int index = Array.LastIndexOf(History.ATValues, lastValue);
                            if (index != -1)
                                History.ATValues[index] = 0;
                        }
                        else
                            ATValue = 0;

                        ATLevel--;
                    }
                    break;

                case DigicloneTypeEnum.BL:
                    {
                        var lastValue = History.BLValues.LastOrDefault(x => x > 0);
                        if (lastValue > 0)
                        {
                            BLValue -= (short)lastValue;

                            int index = Array.LastIndexOf(History.BLValues, lastValue);
                            if (index != -1)
                                History.BLValues[index] = 0;
                        }
                        else
                            BLValue = 0;

                        BLLevel--;
                    }
                    break;

                case DigicloneTypeEnum.CT:
                    {
                        var lastValue = History.CTValues.LastOrDefault(x => x > 0);
                        if (lastValue > 0)
                        {
                            CTValue -= (short)lastValue;

                            int index = Array.LastIndexOf(History.CTValues, lastValue);
                            if (index != -1)
                                History.CTValues[index] = 0;
                        }
                        else
                            CTValue = 0;

                        CTLevel--;
                    }
                    break;

                case DigicloneTypeEnum.HP:
                    {
                        var lastValue = History.HPValues.LastOrDefault(x => x > 0);
                        if (lastValue > 0)
                        {
                            HPValue -= (short)lastValue;

                            int index = Array.LastIndexOf(History.HPValues, lastValue);
                            if (index != -1)
                                History.HPValues[index] = 0;
                        }
                        else
                            HPValue = 0;

                        HPLevel--;
                    }
                    break;

                case DigicloneTypeEnum.EV:
                    {
                        var lastValue = History.EVValues.LastOrDefault(x => x > 0);
                        if (lastValue > 0)
                        {
                            EVValue -= (short)lastValue;

                            int index = Array.LastIndexOf(History.EVValues, lastValue);
                            if (index != -1)
                                History.EVValues[index] = 0;
                        }
                        else
                            EVValue = 0;

                        EVLevel--;
                    }
                    break;
            }
        }
        
        public void ResetOne(DigicloneTypeEnum cloneType)
        {
            switch (cloneType)
            {
                case DigicloneTypeEnum.AT:
                    {
                        var lastValue = History.ATValues.LastOrDefault(x => x > 0);
                        if (lastValue > 0)
                        {
                            ATValue -= (short)lastValue;

                            int index = Array.LastIndexOf(History.ATValues, lastValue);
                            if (index != -1)
                                History.ATValues[index] = 0;
                        }
                        else
                            ATValue = 0;

                        ATLevel--;
                    }
                    break;

                case DigicloneTypeEnum.BL:
                    {
                        var lastValue = History.BLValues.LastOrDefault(x => x > 0);
                        if (lastValue > 0)
                        {
                            BLValue -= (short)lastValue;

                            int index = Array.LastIndexOf(History.BLValues, lastValue);
                            if (index != -1)
                                History.BLValues[index] = 0;
                        }
                        else
                            BLValue = 0;

                        BLLevel--;
                    }
                    break;

                case DigicloneTypeEnum.CT:
                    {
                        var lastValue = History.CTValues.LastOrDefault(x => x > 0);
                        if (lastValue > 0)
                        {
                            CTValue -= (short)lastValue;

                            int index = Array.LastIndexOf(History.CTValues, lastValue);
                            if (index != -1)
                                History.CTValues[index] = 0;
                        }
                        else
                            CTValue = 0;

                        CTLevel--;
                    }
                    break;

                case DigicloneTypeEnum.HP:
                    {
                        var lastValue = History.HPValues.LastOrDefault(x => x > 0);
                        if (lastValue > 0)
                        {
                            HPValue -= (short)lastValue;

                            int index = Array.LastIndexOf(History.HPValues, lastValue);
                            if (index != -1)
                                History.HPValues[index] = 0;
                        }
                        else
                            HPValue = 0;

                        HPLevel--;
                    }
                    break;

                case DigicloneTypeEnum.EV:
                    {
                        var lastValue = History.EVValues.LastOrDefault(x => x > 0);
                        if (lastValue > 0)
                        {
                            EVValue -= (short)lastValue;

                            int index = Array.LastIndexOf(History.EVValues, lastValue);
                            if (index != -1)
                                History.EVValues[index] = 0;
                        }
                        else
                            EVValue = 0;

                        EVLevel--;
                    }
                    break;
            }
        }
        
        public void ResetAll(DigicloneTypeEnum cloneType)
        {
            switch (cloneType)
            {
                case DigicloneTypeEnum.AT:
                    {
                        ATValue = 0;
                        ATLevel = 0;
                    }
                    break;

                case DigicloneTypeEnum.BL:
                    {
                        BLValue = 0;
                        BLLevel = 0;
                    }
                    break;

                case DigicloneTypeEnum.CT:
                    {
                        CTValue = 0;
                        CTLevel = 0;
                    }
                    break;

                case DigicloneTypeEnum.HP:
                    {
                        HPValue = 0;
                        HPLevel = 0;
                    }
                    break;

                case DigicloneTypeEnum.EV:
                    {
                        EVValue = 0;
                        EVLevel = 0;
                    }
                    break;
            }

            History.ResetEntries(cloneType);
        }
    }
}