using System;
using System.Data;

namespace YGOSharp.OCGWrapper
{
    public class NamedCard : Card
    {
        public string Name { get; private set; }
        public string Description { get; private set; }

        internal NamedCard(IDataRecord reader) : base(reader)
        {
            Name = reader.GetString(10);
            Description = reader.GetString(11);
        }

        public static new NamedCard Get(int id)
        {
            return NamedCardsManager.GetCard(id);
        }

        /// <summary>
        /// Returns whether a card ID and its alias represent alternate artwork.
        /// Aliases with an ID difference of 20 or more are rule-name aliases, not alternate artwork.
        /// </summary>
        public static bool IsAltartAlias(int cardId, int alias)
        {
            // YGOPro treats this normal-monster printing as a rule-name alias of the ritual monster.
            if (cardId == 5405695)
                return false;
            return cardId > 0 && alias > 0 && Math.Abs(cardId - alias) < 20;
        }
    }
}
