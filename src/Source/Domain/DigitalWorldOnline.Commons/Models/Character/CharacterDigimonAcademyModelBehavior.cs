namespace DigitalWorldOnline.Commons.Models.Character
{
    public partial class CharacterDigimonAcademyModel
    {
        public void AddSlot()
        {
            DigimonAcademys.Add(new CharacterDigimonAcademyItemModel(DigimonAcademys.Max(x => x.Slot) + 1));
            Slots++;
        }
    }
}
