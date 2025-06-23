

namespace DigitalWorldOnline.Commons.Enums.ClientEnums
{
    public enum InventorySlotTypeEnum
    {

        TabInven = 0,   // nLimit::Inven
        TabEquip,       // nLimit::Equip
        TabWarehouse,   // nLimit::Warehouse

        // Abaixo de digivice
        TabSkill,       // nLimit::Skill,	 Itens de habilidade que podem ser montados em um digital
        TabChipset,     // nLimit::Chipset,	 Itens de chipset que podem ser montados em um dígito
        TabDigivice,    // nLimit::Digivice, Slots que podem ser montados no Digiest ==> Espaço

        TabCashShop,    // nLimit::CashShop, Compre Armazém de Armazenamento de Item de Dinheiro
        TabGiftShop,    // nLimit::GiftShop, Itens armazenados por presentes ou recuperação, etc.
        TabPCBang,      // Somente inventário da sala de PC

        TabShareStash,  // nLimit::ShareStash
                        // 아래는 inven common item tab
        TabCrossWars,   // nLimit::DigimonCrossWars, Cartão Cross Wars?Relógio item

        MaxSlotType
    }
}
