using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YGOSharp.OCGWrapper;
using YGOSharp.OCGWrapper.Enums;

namespace WindBot.Game
{
    public class ChainInfo
    {
        public ClientCard RelatedCard { get; private set; }
        public int ActivatePlayer { get; private set; }
        public int ActivateId { get; private set; }
        public int ActivateController { get; private set; }
        public int ActivatePosition { get; private set; }
        public int ActivateSequence { get; private set; }
        public CardLocation ActivateLocation { get; private set; }
        public int ActivateLevel { get; private set; }
        public int ActivateRank { get; private set; }
        public int ActivateType { get; private set; }
        public int ActivateRace { get; private set; }
        public int ActivateAttack { get; private set; }
        public int ActivateDefense { get; private set; }
        public bool IsSpecialSummoned { get; private set; }
        public int ActivateDescription { get; private set; }

        public ChainInfo(ClientCard card)
            : this(card, card.Controller, 0)
        {
        }

        public ChainInfo(ClientCard card, int player, int desc)
        {
            RelatedCard = card;
            ActivatePlayer = player;
            ActivateId = card.Id;
            ActivateController = card.Controller;
            ActivatePosition = card.Position;
            ActivateSequence = card.Sequence;
            ActivateLocation = card.Location;
            ActivateLevel = card.Level;
            ActivateRank = card.Rank;
            ActivateType = card.Type;
            ActivateRace = card.Race;
            ActivateAttack = card.Attack;
            ActivateDefense = card.Defense;
            ActivateAttack = card.Attack;
            ActivateDefense = card.Defense;
            IsSpecialSummoned = card.IsSpecialSummoned;
            ActivateDescription = desc;
        }

        public bool HasPosition(CardPosition position)
        {
            return (ActivatePosition & (int)position) != 0;
        }

        public bool HasLocation(CardLocation location)
        {
            return ((int)ActivateLocation & (int)location) != 0;
        }

        public bool IsCode(int id)
        {
            return RelatedCard != null && RelatedCard.IsCode(id);
        }

        public bool IsCode(IList<int> ids)
        {
            return RelatedCard != null && RelatedCard.IsCode(ids);
        }

        public bool IsCode(params int[] ids)
        {
            return RelatedCard != null && RelatedCard.IsCode(ids);
        }

        public bool HasType(CardType type)
        {
            return RelatedCard != null && (RelatedCard.Type & (int)type) != 0;
        }
        public bool IsSpell()
        {
            return HasType(CardType.Spell);
        }

        public bool IsTrap()
        {
            return HasType(CardType.Trap);
        }
    }
}
