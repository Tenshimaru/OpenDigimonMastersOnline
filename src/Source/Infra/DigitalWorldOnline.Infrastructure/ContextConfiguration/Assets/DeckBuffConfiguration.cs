using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class DeckBuffConfiguration : IEntityTypeConfiguration<DeckBuffAssetDTO>
    {
        public void Configure(EntityTypeBuilder<DeckBuffAssetDTO> builder)
        {
            builder
                .ToTable("DeckBuff", "Asset")
                .HasKey(x => x.Id);
            
            builder
                .Property(e => e.GroupIdX)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.GroupName)
                .HasMaxLength(250)
                .HasColumnType("varchar")
                .IsRequired();

            builder
                .Property(e => e.Explain)
                .HasColumnType("varchar(max)")
                .IsRequired();

            // Add unique index
            builder
                .HasIndex(e => e.GroupIdX)
                .IsUnique();
            
            builder
                .HasMany(x => x.Options)
                .WithOne(x => x.DeckBuff)
                .HasForeignKey(x => x.GroupIdX)
                .HasPrincipalKey(x => x.GroupIdX);
            
            builder
                .HasMany(x => x.Characters)
                .WithOne(x => x.DeckBuff)
                .HasForeignKey(x => x.DeckBuffId)
                .HasPrincipalKey(x => x.GroupIdX);
            
            builder
                .HasData(
                    new DeckBuffAssetDTO
                    {
                        Id = 1,
                        GroupIdX = 1001,
                        GroupName = "Four Holy Beasts",
                        Explain =
                            "The Four Holy Beasts are four Ultimate digimons that reign over the north, south, east, and west of the digital world. Qinglongmon(Lightning), Xuanwumon(Water), Zhuqiaomon(Fire), and Baihumon(Iron), the Four Holy Beasts have, unlike other normal digimons, twelve cores and four eyes respectively.",
                    },
                    new DeckBuffAssetDTO
                    {
                        Id = 2,
                        GroupIdX = 1002,
                        GroupName = "Fusion to evolve into the higher",
                        Explain =
                            "Two or more Jogress digimons can be combined to evolve into an entirely different digimon. A digimon successfully jogressed has great power and an essential role in the digital world."
                    },
                    new DeckBuffAssetDTO
                    {
                        Id = 3,
                        GroupIdX = 1004,
                        GroupName = "Three Archangels",
                        Explain =
                            "They are the three great archangels who are the leaders of all the angelic digimons in the digital world. They are Seraphimon who executes god's law, Cherubimon who protects wisdom and god's area, Ophanimon who delivers god's love. Also, they lead angelic army consists of man-type, animal-type, and woman-type angelic digimons. Seraphimon is the highest of the three, and thus of all the angelic digimons."
                    },
                    new DeckBuffAssetDTO
                    {
                        Id = 4,
                        GroupIdX = 1008,
                        GroupName = "Main characters of the adventure",
                        Explain =
                            "Digimon adventure's 8 main characters' partner digimons. Tai's partner Agumon, Matthew's partner Gabumon, Izzy's partner Tentomon, Kido Jou's partner Gomamon, T.K.'s partner Patamon, Mimi's Palmon, Sora's partner Biyomon, Kari's partner Gatomon are the members."
                    },
                    new DeckBuffAssetDTO
                    {
                        Id = 5,
                        GroupIdX = 1009,
                        GroupName = ">Burst Ultimate Wing",
                        Explain =
                            "This is the group of the digimons with wings who have become Burst Mode digimons overcoming the power of Mega digimons.\nTheir wings influence all their actions from battle to moving.\nThe wings can be the shield of the Digital World full of good will,\nbut sometimes they can also be the blade raising the wind of darkness."
                    },
                    new DeckBuffAssetDTO
                    {
                        Id = 6,
                        GroupIdX = 1010,
                        GroupName = "Seafood Stew",
                        Explain =
                            "The digimon looks like the food one can easily see in daily life.\nMushroom, clam, fish, squid,\nand pumpkin can make really great seafood stew if mixed harmoniously.\n※Jogress Food※\nEvolve into the best seafood stew!"
                    },
                    new DeckBuffAssetDTO
                    {
                        Id = 7,
                        GroupIdX = 1011,
                        GroupName = "Sharp Blue Sword Wave",
                        Explain =
                            "This is the group of the digimons using big and nice blades to lead the battle.\nThey are usually masters of swords so that it's very beautiful to watch they play with their\nswords.\nAs the digimons walk only on the sword's way,\nthey commonly hold fast to their justice just like sharp blades."
                    },
                    new DeckBuffAssetDTO
                    {
                        Id = 8,
                        GroupIdX = 1012,
                        GroupName = "Instant Red Gun Fire",
                        Explain =
                            "This is the group of the digimons using firearms like guns and cannons to win battles.\nSome of them unleash stylish attacks with pistols,\nthe others deal heavy damage with huge cannon installed on their body.\nGun using digimons are usually cheerful, whereas\nCannon using digimons are taciturn."
                    },
                    new DeckBuffAssetDTO
                    {
                        Id = 9,
                        GroupIdX = 1013,
                        GroupName = "So cute♥",
                        Explain =
                            "This is the group of the digimons that you want to be with.\nWhen you come back home,  if one of the digimons from the deck welcomes you,\nit will be your happy day.\nWhen you are uncomfortable, you can be calmed if you touch their shaggy hair."
                    },
                    new DeckBuffAssetDTO
                    {
                        Id = 10,
                        GroupIdX = 1016,
                        GroupName = "Four Dark Masters",
                        Explain =
                            "Appearing at the latter part of the digimon adventure, they are powerful dark forces who can compress all the areas into one place. Four dark masters have Ultimate digimons as servants."
                    },
                    new DeckBuffAssetDTO
                    {
                        Id = 11,
                        GroupIdX = 1017,
                        GroupName = "Ego of the Darkness",
                        Explain =
                            "It is the final boss of Digimon Adventure\nand came from beyond the Wall of Fire to have the final fight with the Digidestined.\nIt wanted to help the world but it is a collective agent of the resentment of the Digimons that\ndied out and are left behind by evolution.\nThe egos of resentment disappeared in the darkness\nand decided to attack the Digital World to give the Digidestined and the other Digimons in the\nworld of light the same pain it suffered."
                    },
                    new DeckBuffAssetDTO
                    {
                        Id = 12,
                        GroupIdX = 1019,
                        GroupName = "Hyper Spirit Evolution",
                        Explain = "The Final Sprit Evolusion\nHyper Spirit Evolusion Digimon"
                    },
                    new DeckBuffAssetDTO
                    {
                        Id = 13,
                        GroupIdX = 1020,
                        GroupName = "Ancient Spirit Evolution",
                        Explain =
                            "Digimon Group that Told of Oriental legends, strongest destructive god and the god which governs over regeneration, Susanoomon and Hyper Spirit Evolution Digimons."
                    },
                    new DeckBuffAssetDTO
                    {
                        Id = 14,
                        GroupIdX = 1021,
                        GroupName = "Legendary Knights of Vaccine",
                        Explain =
                            "A group of legendary vaccine knights that declared to be natural enemy of evil virus Digimons."
                    },
                    new DeckBuffAssetDTO
                    {
                        Id = 15,
                        GroupIdX = 1022,
                        GroupName = "OMEGA",
                        Explain = "Community of 3 different Holy Knights that are alike."
                    },
                    new DeckBuffAssetDTO
                    {
                        Id = 16,
                        GroupIdX = 1023,
                        GroupName = "MEGA-OMEGA",
                        Explain =
                            "Community of 3 different Holy Knights that are alike and Digimon after transcendence."
                    }
                );
        }
    }
}