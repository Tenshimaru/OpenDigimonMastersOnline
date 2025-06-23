namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class GetGameMapConfigForAdminQueryDto
    {
        public long Id { get; set; }
        public long MapId { get; set; }
        public string Name { get; set; }
        public int Mobs { get; set; }
    }
}