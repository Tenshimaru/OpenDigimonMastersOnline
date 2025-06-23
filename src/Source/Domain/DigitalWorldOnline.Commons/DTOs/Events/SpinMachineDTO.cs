namespace DigitalWorldOnline.Commons.DTOs.Events
{
    public class SpinMachineDTO
    {
        public long SpinMachineId { get; set; }
        public int NormalItensRemaining { get; set; }
        public List<SpinMachineRareRewardDTO> RareRewards { get; set; }
    }
}
