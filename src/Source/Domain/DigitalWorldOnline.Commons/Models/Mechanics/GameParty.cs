using DigitalWorldOnline.Commons.Enums.Party;
using DigitalWorldOnline.Commons.Models.Character;
using System.Reflection;

namespace DigitalWorldOnline.Commons.Models.Mechanics
{
    public class GameParty
    {
        private Dictionary<byte,CharacterModel> _members;
        public int Id { get; private set; }
        public PartyLootShareTypeEnum LootType { get; private set; }
        public PartyLootShareRarityEnum LootFilter { get; private set; }
        public int LeaderSlot { get;  set; }
        public long LeaderId { get; private set; }
        public DateTime CreateDate { get; }

        public Dictionary<byte,CharacterModel> Members
        {
            get
            {
                return _members.OrderBy(x => x.Key).ToDictionary(x => x.Key,x => x.Value);
            }

            private set
            {
                _members = value;
            }
        }

        public KeyValuePair<byte,CharacterModel> this[long memberId] => Members.First(x => x.Value.Id == memberId);
        public KeyValuePair<byte,CharacterModel> this[string memberName] => Members.First(x => x.Value.Name == memberName);

        private GameParty(int id,CharacterModel leader,CharacterModel member)
        {
            Id = id;
            CreateDate = DateTime.Now;
            LootType = PartyLootShareTypeEnum.Normal;
            LootFilter = PartyLootShareRarityEnum.Lv1;
            LeaderSlot = 0;
            LeaderId = leader.Id;

            Members = new()
            {
                { 0, leader },
                { 1, member }
            };
        }

        public static GameParty Create(int id,CharacterModel leader,CharacterModel member)
        {
            return new GameParty
            (
                id,
                leader,
                member
            );
        }
        public void AddMember(CharacterModel member)
        {
            var last = _members.Keys.Last();

            byte newKey;

            if (last != null)
            {
                newKey = (byte)(last + 1);
            }
            else
            {
                newKey = (byte)(_members.Count);
            }

            _members.Add(newKey,member);
        }

        public void ChangeLeader(byte newLeaderSlot)
        {
            if (_members.TryGetValue(newLeaderSlot,out var newLeader))
            {
                LeaderSlot = newLeaderSlot;
                LeaderId = newLeader.Id;
            }
        }

        public void UpdateMember(KeyValuePair<byte,CharacterModel> member,CharacterModel newData)
        {
            if (_members.ContainsKey(member.Key))
                _members[member.Key] = newData;
        }
        public KeyValuePair<byte,CharacterModel>? GetMemberById(long memberId)
        {
            return Members.FirstOrDefault(x => x.Value.Id == memberId);
        }
        public void ChangeLootType(PartyLootShareTypeEnum lootType,PartyLootShareRarityEnum rareType)
        {
            LootType = lootType;
            LootFilter = rareType;
        }

        public List<long> GetMembersIdList() => _members.Values.Select(x => x.Id).ToList();

        public object Clone() => MemberwiseClone();
    

    public void RemoveMember(byte memberSlot)
        {
            if (_members.ContainsKey(memberSlot))
            {
                var removedMember = _members[memberSlot];

                _members.Remove(memberSlot);
                ReorderMembers();

                if (LeaderId == removedMember.Id)
                {
                    AssignNewLeader();
                }
            }
        }

        private void ReorderMembers()
        {
            var updatedMembers = _members.OrderBy(pair => pair.Key)
                .ToDictionary(pair => pair.Key,pair => pair.Value);

            _members.Clear();
            foreach (var kvp in updatedMembers)
            {
                _members.Add(kvp.Key,kvp.Value);
            }

            ValidateLeader();
        }

        private void ValidateLeader()
        {
            if (!_members.Values.Any(member => member.Id == LeaderId))
            {
                AssignNewLeader();
            }
        }

        private void AssignNewLeader()
        {
            if (_members.Count > 0)
            {
                var newLeaderEntry = _members.First();
                LeaderSlot = newLeaderEntry.Key;
                LeaderId = newLeaderEntry.Value.Id;
                
            }
           
        }
    }
}
