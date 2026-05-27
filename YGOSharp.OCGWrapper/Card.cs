using YGOSharp.OCGWrapper.Enums;
using System.Data;

namespace YGOSharp.OCGWrapper
{
    public class Card
    {
        public struct CardData
        {
            public int Code;
            public int Alias;
            public long Setcode;
            public int Type;
            public int Level;
            public int Attribute;
            public int Race;
            public int Attack;
            public int Defense;
            public int LScale;
            public int RScale;
            public int LinkMarker;
        }

        public int Id { get; private set; }
        public int Ot { get; private set; }
        public int Alias { get; private set; }
        public long Setcode { get; private set; }
        public int Type { get; private set; }

        public int Level { get; private set; }
        public int LScale { get; private set; }
        public int RScale { get; private set; }
        public int LinkMarker { get; private set; }

        public int Attribute { get; private set; }
        public int Race { get; private set; }
        public int Attack { get; private set; }
        public int Defense { get; private set; }

        internal CardData Data { get; private set; }

        public static Card Get(int id)
        {
            return CardsManager.GetCard(id);
        }

        public bool HasType(CardType type)
        {
            return ((Type & (int)type) != 0);
        }

        public bool HasSetcode(int setcode)
        {
            long setcodes = Setcode;
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

        public bool IsExtraCard()
        {
            return (HasType(CardType.Fusion) || HasType(CardType.Synchro) || HasType(CardType.Xyz) || HasType(CardType.Link));
        }

        internal Card(IDataRecord reader)
        {
            Id = reader.GetInt32(0);
            Ot = reader.GetInt32(1);
            Alias = reader.GetInt32(2);
            Setcode = reader.GetInt64(3);
            Type = reader.GetInt32(4);

            int levelInfo = reader.GetInt32(5);
            Level = levelInfo & 0xff;
            LScale = (levelInfo >> 24) & 0xff;
            RScale = (levelInfo >> 16) & 0xff;

            Race = reader.GetInt32(6);
            Attribute = reader.GetInt32(7);
            Attack = reader.GetInt32(8);
            Defense = reader.GetInt32(9);

            if (HasType(CardType.Link))
            {
                LinkMarker = Defense;
                Defense = 0;
            }

            Data = new CardData()
            {
                Code = Id,
                Alias = Alias,
                Setcode = Setcode,
                Type = Type,
                Level = Level,
                Attribute = Attribute,
                Race = Race,
                Attack = Attack,
                Defense = Defense,
                LScale = LScale,
                RScale = RScale,
                LinkMarker = LinkMarker
            };
        }
    }
}