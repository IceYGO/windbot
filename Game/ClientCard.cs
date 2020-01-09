using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public int Sequence { get; set; }
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
        public int LinkCount { get; private set; }
        public int LinkMarker { get; private set; }
        public int BaseAttack { get; private set; }
        public int BaseDefense { get; private set; }
        public int RealPower { get; set; }
        public List<int> Overlays { get; private set; }
        public int Owner { get; private set; }
        public int Controller { get; private set; }
        public int Disabled { get; private set; }
        public int ProcCompleted { get; private set; }
        public int SelectSeq { get; set; }
        public int OpParam1 { get; set; }
        public int OpParam2 { get; set; }

        public List<ClientCard> EquipCards { get; set; }
        public ClientCard EquipTarget;
        public List<ClientCard> OwnTargets { get; set; }
        public List<ClientCard> TargetCards { get; set; }

        public bool CanDirectAttack { get; set; }
        public bool ShouldDirectAttack { get; set; }
        public bool Attacked { get; set; }
        public bool IsLastAttacker { get; set; }
        public bool IsSpecialSummoned { get; set; }

        public int[] ActionIndex { get; set; }
        public IDictionary<long, int> ActionActivateIndex { get; private set; }

        public ClientCard(int id, CardLocation loc, int sequence)
            : this(id, loc, -1 , 0)
        {
        }

        public ClientCard(int id, CardLocation loc, int sequence, int position)
        {
            SetId(id);
            Sequence = sequence;
            Position = position;
            Overlays = new List<int>();
            EquipCards = new List<ClientCard>();
            OwnTargets = new List<ClientCard>();
            TargetCards = new List<ClientCard>();
            ActionIndex = new int[16];
            ActionActivateIndex = new Dictionary<long, int>();
            Location = loc;
        }

        public void SetId(int id)
        {
            if (Id == id) return;
            Id = id;
            Data = NamedCard.Get(Id);
            if (Data != null)
            {
                Name = Data.Name;
                if (Data.Alias != 0)
                    Alias = Data.Alias;
            }
        }

        public long Update(BinaryReader packet, Duel duel)
        {
            long pos = packet.BaseStream.Position;
            while (true)
            {
                int size = packet.ReadInt16();
                if (size == 0)
                    return packet.BaseStream.Position - pos;
                uint flag = packet.ReadUInt32();
                switch (flag)
                {
                    case (uint)Query.Code:
                        {
                            SetId(packet.ReadInt32());
                            break;
                        }
                    case (uint)Query.Position:
                        {
                            Position = packet.ReadInt32();
                            break;
                        }
                    case (uint)Query.Alias:
                        {
                            Alias = packet.ReadInt32();
                            break;
                        }
                    case (uint)Query.Type:
                        {
                            Type = packet.ReadInt32();
                            break;
                        }
                    case (uint)Query.Level:
                        {
                            Level = packet.ReadInt32();
                            break;
                        }
                    case (uint)Query.Rank:
                        {
                            Rank = packet.ReadInt32();
                            break;
                        }
                    case (uint)Query.Attribute:
                        {
                            Attribute = packet.ReadInt32();
                            break;
                        }
                    case (uint)Query.Race:
                        {
                            Race = packet.ReadInt32();
                            break;
                        }
                    case (uint)Query.Attack:
                        {
                            Attack = packet.ReadInt32();
                            break;
                        }
                    case (uint)Query.Defence:
                        {
                            Defense = packet.ReadInt32();
                            break;
                        }
                    case (uint)Query.BaseAttack:
                        {
                            BaseAttack = packet.ReadInt32();
                            break;
                        }
                    case (uint)Query.BaseDefence:
                        {
                            BaseDefense = packet.ReadInt32();
                            break;
                        }
                    case (uint)Query.OverlayCard:
                        {
                            Overlays.Clear();
                            int count = packet.ReadInt32();
                            for (int i = 0; i < count; ++i)
                                Overlays.Add(packet.ReadInt32());
                        }
                        break;
                    case (uint)Query.Owner:
                        {
                            Owner = duel.GetLocalPlayer(packet.ReadByte());
                            break;
                        }
                    case (uint)Query.Status:
                        {
                            int status = packet.ReadInt32();
                            const int STATUS_DISABLED = 0x0001;
                            const int STATUS_PROC_COMPLETE = 0x0008;
                            Disabled = status & STATUS_DISABLED;
                            ProcCompleted = status & STATUS_PROC_COMPLETE;
                            break;
                        }
                    case (uint)Query.LScale:
                        {
                            LScale = packet.ReadInt32();
                            break;
                        }
                    case (uint)Query.RScale:
                        {
                            RScale = packet.ReadInt32();
                            break;
                        }
                    case (uint)Query.Link:
                        {
                            LinkCount = packet.ReadInt32();
                            LinkMarker = packet.ReadInt32();
                            break;
                        }
                    case 0x80000000: //Query.End
                        return packet.BaseStream.Position - pos;
                    default:
                        {
                            packet.ReadChars(size - sizeof(uint));
                            break;
                        }
                }
            }
        }

        public void ClearCardTargets()
        {
            foreach (ClientCard card in TargetCards)
            {
                card.OwnTargets.Remove(this);
            }
            foreach (ClientCard card in OwnTargets)
            {
                card.TargetCards.Remove(this);
            }
            OwnTargets.Clear();
            TargetCards.Clear();
        }

        public bool HasLinkMarker(int dir)
        {
            return (LinkMarker & dir) != 0;
        }

        public bool HasLinkMarker(CardLinkMarker dir)
        {
            return (LinkMarker & (int)dir) != 0;
        }

        public int GetLinkedZones()
        {
            if (!HasType(CardType.Link) || Location != CardLocation.MonsterZone)
                return 0;
            int zones = 0;
            if (Sequence > 0 && Sequence <= 4 && HasLinkMarker(CardLinkMarker.Left))
                zones |= 1 << (Sequence - 1);
            if (Sequence <= 3 && HasLinkMarker(CardLinkMarker.Right))
                zones |= 1 << (Sequence + 1);
            if (Sequence == 0 && HasLinkMarker(CardLinkMarker.TopRight)
                || Sequence == 1 && HasLinkMarker(CardLinkMarker.Top)
                || Sequence == 2 && HasLinkMarker(CardLinkMarker.TopLeft))
                zones |= (1 << 5) | (1 << (16 + 6));
            if (Sequence == 2 && HasLinkMarker(CardLinkMarker.TopRight)
                || Sequence == 3 && HasLinkMarker(CardLinkMarker.Top)
                || Sequence == 4 && HasLinkMarker(CardLinkMarker.TopLeft))
                zones |= (1 << 6) | (1 << (16 + 5));
            if (Sequence == 5)
            {
                if (HasLinkMarker(CardLinkMarker.BottomLeft))
                    zones |= 1 << 0;
                if (HasLinkMarker(CardLinkMarker.Bottom))
                    zones |= 1 << 1;
                if (HasLinkMarker(CardLinkMarker.BottomRight))
                    zones |= 1 << 2;
                if (HasLinkMarker(CardLinkMarker.TopLeft))
                    zones |= 1 << (16 + 4);
                if (HasLinkMarker(CardLinkMarker.Top))
                    zones |= 1 << (16 + 3);
                if (HasLinkMarker(CardLinkMarker.TopRight))
                    zones |= 1 << (16 + 2);
            }
            if (Sequence == 6)
            {
                if (HasLinkMarker(CardLinkMarker.BottomLeft))
                    zones |= 1 << 2;
                if (HasLinkMarker(CardLinkMarker.Bottom))
                    zones |= 1 << 3;
                if (HasLinkMarker(CardLinkMarker.BottomRight))
                    zones |= 1 << 4;
                if (HasLinkMarker(CardLinkMarker.TopLeft))
                    zones |= 1 << (16 + 2);
                if (HasLinkMarker(CardLinkMarker.Top))
                    zones |= 1 << (16 + 1);
                if (HasLinkMarker(CardLinkMarker.TopRight))
                    zones |= 1 << (16 + 0);
            }
            return zones;
        }

        public bool HasType(CardType type)
        {
            return (Type & (int)type) != 0;
        }

        public bool HasPosition(CardPosition position)
        {
            return (Position & (int)position) != 0;
        }

        public bool HasAttribute(CardAttribute attribute)
        {
            return (Attribute & (int)attribute) != 0;
        }

        public bool HasSetcode(int setcode)
        {
            if (Data == null) return false;
            long setcodes = Data.Setcode;
            int settype = setcode & 0xfff;
            int setsubtype = setcode & 0xf000;
            while (setcodes > 0)
            {
                long check_setcode = setcodes & 0xffff;
                setcodes >>= 16;
                if ((check_setcode & 0xfff) == settype && (check_setcode & 0xf000 & setsubtype) == setsubtype) return true;
            }
            return false;
        }

        public bool IsMonster()
        {
            return HasType(CardType.Monster);
        }

        public bool IsTuner()
        {
            return HasType(CardType.Tuner);
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
            return HasType(CardType.Fusion) || HasType(CardType.Synchro) || HasType(CardType.Xyz) || HasType(CardType.Link);
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
            return Disabled != 0;
        }

        public bool IsCanRevive()
        {
            return ProcCompleted != 0 || !(IsExtraCard() || HasType(CardType.Ritual) || HasType(CardType.SpSummon));
        }

        public bool IsCode(int id)
        {
            return Id == id || Alias != 0 && Alias == id;
        }

        public bool IsCode(IList<int> ids)
        {
            return ids.Contains(Id) || Alias != 0 && ids.Contains(Alias);
        }

        public bool IsCode(params int[] ids)
        {
            return ids.Contains(Id) || Alias != 0 && ids.Contains(Alias);
        }

        public bool IsOriginalCode(int id)
        {
            return Id == id || Alias - Id < 10 && Alias == id;
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