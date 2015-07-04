using WindBot.Game.Enums;

namespace WindBot.Game.Data
{
    public class CardData
    {
        public int Id { get; private set; }
        public int AliasId { get; set; }
        public int Type { get; set; }
        public int Level { get; set; }
        public int Attribute { get; set; }
        public int Race { get; set; }
        public int Atk { get; set; }
        public int Def { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public CardData(int id)
        {
            Id = id;
        }

        public bool HasType(CardType type)
        {
            return ((Type & (int)type) != 0);
        }

        public bool IsExtraCard()
        {
            return (HasType(CardType.Fusion) || HasType(CardType.Synchro) || HasType(CardType.Xyz));
        }
    }
}