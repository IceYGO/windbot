using System.Collections.Generic;
using System.IO;
using YGOSharp.OCGWrapper;
using YGOSharp.OCGWrapper.Enums;

namespace WindBot.Game
{
    public class ClientCard
    {
        public int Id { get; private set; }
        public NamedCard Data { get; private set; }
        public string Name { get; private set; }

        public int Position { get; set; }
        public CardLocation Location { get; set; }
        public int Alias { get; private set; }
        public int Level { get; private set; }
        public int Rank { get; private set; }
        public int Type { get; private set; }
        public int Attribute { get; private set; }
        public int Race { get; private set; }
        public int Attack { get; private set; }
        public int Defense { get; private set; }
        public int LScale { get; private set; }
        public int RScale { get; private set; }
        public int BaseAttack { get; private set; }
        public int BaseDefense { get; private set; }
        public int RealPower { get; set; }
        public List<int> Overlays { get; private set; }
        public int Owner { get; private set; }
        public int Controller { get; private set; }
        public int Disabled { get; private set; }
        public int SelectSeq { get; set; }
        public int OpParam1 { get; set; }
        public int OpParam2 { get; set; }
        public bool CanDirectAttack { get; set; }
        public bool ShouldDirectAttack { get; set; }
        public bool Attacked { get; set; }
        public bool IsLastAttacker { get; set; }

        public int[] ActionIndex { get; set; }
        public IDictionary<int, int> ActionActivateIndex { get; private set; }

        public ClientCard(int id, CardLocation loc)
            : this(id, loc, 0)
        {
        }

        public ClientCard(int id, CardLocation loc, int position)
        {
            SetId(id);
            Position = position;
            Overlays = new List<int>();
            ActionIndex = new int[16];
            ActionActivateIndex = new Dictionary<int, int>();
            Location = loc;
        }

        public void SetId(int id)
        {
            if (Id == id) return;
            Id = id;
            Data = NamedCard.Get(Id);
            if (Data != null)
                Name = Data.Name;
        }

        public void Update(BinaryReader packet, Duel duel)
        {
            int flag = packet.ReadInt32();
            if ((flag & (int)Query.Code) != 0)
                SetId(packet.ReadInt32());
            if ((flag & (int)Query.Position) != 0)
            {
                Controller = duel.GetLocalPlayer(packet.ReadByte());
                packet.ReadByte();
                packet.ReadByte();
                Position = packet.ReadByte();
            }
            if ((flag & (int)Query.Alias) != 0)
                Alias = packet.ReadInt32();
            if ((flag & (int)Query.Type) != 0)
                Type = packet.ReadInt32();
            if ((flag & (int)Query.Level) != 0)
                Level = packet.ReadInt32();
            if ((flag & (int)Query.Rank) != 0)
                Rank = packet.ReadInt32();
            if ((flag & (int)Query.Attribute) != 0)
                Attribute = packet.ReadInt32();
            if ((flag & (int)Query.Race) != 0)
                Race = packet.ReadInt32();
            if ((flag & (int)Query.Attack) != 0)
                Attack = packet.ReadInt32();
            if ((flag & (int)Query.Defence) != 0)
                Defense = packet.ReadInt32();
            if ((flag & (int)Query.BaseAttack) != 0)
                BaseAttack = packet.ReadInt32();
            if ((flag & (int)Query.BaseDefence) != 0)
                BaseDefense = packet.ReadInt32();
            if ((flag & (int)Query.Reason) != 0)
                packet.ReadInt32();
            if ((flag & (int)Query.ReasonCard) != 0)
                packet.ReadInt32(); // Int8 * 4
            if ((flag & (int)Query.EquipCard) != 0)
                packet.ReadInt32(); // Int8 * 4
            if ((flag & (int)Query.TargetCard) != 0)
            {
                int count = packet.ReadInt32();
                for (int i = 0; i < count; ++i)
                    packet.ReadInt32(); // Int8 * 4
            }
            if ((flag & (int)Query.OverlayCard) != 0)
            {
                Overlays.Clear();
                int count = packet.ReadInt32();
                for (int i = 0; i < count; ++i)
                    Overlays.Add(packet.ReadInt32());
            }
            if ((flag & (int)Query.Counters) != 0)
            {
                int count = packet.ReadInt32();
                for (int i = 0; i < count; ++i)
                    packet.ReadInt32(); // Int16 * 2
            }
            if ((flag & (int)Query.Owner) != 0)
                Owner = duel.GetLocalPlayer(packet.ReadInt32());
            if ((flag & (int)Query.IsDisabled) != 0)
                Disabled = packet.ReadInt32();
            if ((flag & (int)Query.IsPublic) != 0)
                packet.ReadInt32();
            if ((flag & (int)Query.LScale) != 0)
                LScale = packet.ReadInt32();
            if ((flag & (int)Query.RScale) != 0)
                RScale = packet.ReadInt32();
        }

        public bool HasType(CardType type)
        {
            return ((Type & (int)type) != 0);
        }

        public bool HasPosition(CardPosition position)
        {
            return ((Position & (int)position) != 0);
        }

        public bool HasAttribute(CardAttribute attribute)
        {
            return ((Attribute & (int)attribute) != 0);
        }

        public bool IsMonster()
        {
            return HasType(CardType.Monster);
        }

        public bool IsSpell()
        {
            return HasType(CardType.Spell);
        }

        public bool IsTrap()
        {
            return HasType(CardType.Trap);
        }

        public bool IsExtraCard()
        {
            return (HasType(CardType.Fusion) || HasType(CardType.Synchro) || HasType(CardType.Xyz));
        }

        public bool IsFaceup()
        {
            return HasPosition(CardPosition.FaceUp);
        }

        public bool IsFacedown()
        {
            return HasPosition(CardPosition.FaceDown);
        }

        public bool IsAttack()
        {
            return HasPosition(CardPosition.Attack);
        }

        public bool IsDefense()
        {
            return HasPosition(CardPosition.Defence);
        }

        public bool IsDisabled()
        {
            return (Disabled != 0);
        }

        public bool HasXyzMaterial()
        {
            return Overlays.Count > 0;
        }

        public bool HasXyzMaterial(int count)
        {
            return Overlays.Count >= count;
        }

        public bool HasXyzMaterial(int count, int cardid)
        {
            return Overlays.Count >= count && Overlays.Contains(cardid);
        }

        public int GetDefensePower()
        {
            return IsAttack() ? Attack : Defense;
        }

        public bool Equals(ClientCard card)
        {
            return ReferenceEquals(this, card);
        }
    }
}