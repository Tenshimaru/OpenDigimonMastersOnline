using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Extensions;

namespace DigitalWorldOnline.Commons.Models.Mechanics
{
    public sealed partial class GuildAuthorityModel
    {
        /// <summary>
        /// Creates a new guild authority object.
        /// </summary>
        /// <param name="memberClass">Authority type enumeration</param>
        public static GuildAuthorityModel Create(GuildAuthorityTypeEnum memberClass)
        {
            return new GuildAuthorityModel()
            {
                Class = memberClass,
                Duty = memberClass.GetDescription(),
                Title = memberClass.GetDescription()
            };
        }

        /// <summary>
        /// Updates the target guild authority title and duty.
        /// </summary>
        /// <param name="newTitle">New title</param>
        /// <param name="newDuty">New duty</param>
        public GuildAuthorityModel Update(string newTitle, string newDuty)
        {
            Title = newTitle;
            Duty = newDuty;

            return this;
        }
    }
}