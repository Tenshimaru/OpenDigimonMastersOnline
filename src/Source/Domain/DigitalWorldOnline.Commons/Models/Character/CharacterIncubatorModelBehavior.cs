using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Utils;
using System.Drawing;

namespace DigitalWorldOnline.Commons.Models.Character
{
    public sealed partial class CharacterIncubatorModel
    {
        /// <summary>
        /// Inserts a new egg into the incubator.
        /// </summary>
        public void InsertEgg(int eggId)
        { 
            EggId = eggId;
            HatchLevel = 0;
        }

        /// <summary>
        /// Removes the current egg from the incubator.
        /// </summary>
        public void RemoveEgg()
        { 
            EggId = 0;
            HatchLevel = 0;
        }
        
        /// <summary>
        /// Inserts a new backup disk into the incubator.
        /// </summary>
        public void InsertBackupDisk(int backupDiskId) => BackupDiskId = backupDiskId;

        /// <summary>
        /// Removes the current backup disk from the incubator.
        /// </summary>
        public void RemoveBackupDisk() => BackupDiskId = 0;
        
        /// <summary>
        /// Increases the current incubator egg level.
        /// </summary>
        public void IncreaseLevel() => HatchLevel += 1;
        
        /// <summary>
        /// Returns the hatch size based on current egg level;
        /// </summary>
        public short GetLevelSize()
        {
            return HatchLevel switch
            {
                3 => UtilitiesFunctions.RandomShort(8200, 10000),
                4 => UtilitiesFunctions.RandomShort(11000, 12500),
                5 => UtilitiesFunctions.RandomShort(11800, 13000),
                _ => 0,
            };
        }

        /// <summary>
        /// Returns the flag for perfect size based on the hatch grade.
        /// </summary>
        /// <param name="grade">Hatch grade enumeration</param>
        /// <param name="size">Hatch size</param>
        public bool PerfectSize(DigimonHatchGradeEnum grade, short size)
        {
            return grade switch
            {
                DigimonHatchGradeEnum.Default => size == 10000,
                DigimonHatchGradeEnum.High => size == 12500,
                DigimonHatchGradeEnum.Perfect => size == 13000,
                _ => false
            };
        }

        /// <summary>
        /// Flag for recent inserted egg.
        /// </summary>
        public bool NotDevelopedEgg => EggId > 0 && HatchLevel == 0;
    }
}
