using DigitalWorldOnline.Commons.DTOs.Events;
using DigitalWorldOnline.Commons.Models.Character;

namespace DigitalWorldOnline.Commons.Models.Mechanics
{
    public class ArenaRankingCompetitorModel
    {
        public Guid Id { get; set; }
        public long TamerId { get; set; }
        public DateTime InsertDate { get; set; }
        public int Points { get; set; }
        public byte Position { get; set; }
        public bool New { get; set; }
        public string TamerName { get; set; }
        public string GuildName { get; set; }

        public void AddPoints(int points)
        {
            Points += points;
            InsertDate = DateTime.Now;       
        }

        public void SetTamerAndGuildName(string tamerName,string guildName)
        {
            TamerName = tamerName;
            GuildName = guildName;
        }
        public void SetPosition(byte position) => Position = position;
        
        public ArenaRankingCompetitorModel(long tamerId,int points)
        {
            Id = Guid.NewGuid();
            TamerId = tamerId;
            Points = points;
            InsertDate = DateTime.Now;
            New = true;
        }
    }
}
