using DigitalWorldOnline.Commons.Enums.ClientEnums;

namespace DigitalWorldOnline.Commons.Models.Digimon
{
    public partial class DigimonDigicloneHistoryModel
    {
        internal void ResetEntries(DigicloneTypeEnum type)
        {
            switch (type)
            {
                case DigicloneTypeEnum.AT:
                    {
                        ATValues = new int[15] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    }
                    break;

                case DigicloneTypeEnum.BL:
                    {
                        BLValues = new int[15] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    }
                    break;

                case DigicloneTypeEnum.CT:
                    {
                        CTValues = new int[15] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    }
                    break;

                case DigicloneTypeEnum.HP:
                    {
                        HPValues = new int[15] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    }
                    break;

                case DigicloneTypeEnum.EV:
                    {
                        EVValues = new int[15] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    }
                    break;
            }
        }

        internal void AddEntry(DigicloneTypeEnum type, short value)
        {
            switch (type)
            {
                case DigicloneTypeEnum.AT:
                    {
                        int index = Array.IndexOf(ATValues, 0);
                        if (index != -1)
                            ATValues[index] = value;
                    }
                    break;

                case DigicloneTypeEnum.BL:
                    {
                        int index = Array.IndexOf(BLValues, 0);
                        if (index != -1)
                            BLValues[index] = value;
                    }
                    break;

                case DigicloneTypeEnum.CT:
                    {
                        int index = Array.IndexOf(CTValues, 0);
                        if (index != -1)
                            CTValues[index] = value;
                    }
                    break;

                case DigicloneTypeEnum.HP:
                    {
                        int index = Array.IndexOf(HPValues, 0);
                        if (index != -1)
                            HPValues[index] = value;
                    }
                    break;

                case DigicloneTypeEnum.EV:
                    {
                        int index = Array.IndexOf(EVValues, 0);
                        if (index != -1)
                            EVValues[index] = value;
                    }
                    break;
            }
        }
    }
}