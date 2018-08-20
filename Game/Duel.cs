using System.Collections.Generic;
using YGOSharp.OCGWrapper.Enums;

namespace WindBot.Game
{
    public class Duel
    {
        public bool IsFirst { get; set; }
        public bool IsNewRule { get; set; }

        public ClientField[] Fields { get; private set; }

        public int Turn { get; set; }
        public int Player { get; set; }
        public DuelPhase Phase { get; set; }
        public MainPhase MainPhase { get; set; }
        public BattlePhase BattlePhase { get; set; }

        public int LastChainPlayer { get; set; }
        public IList<ClientCard> CurrentChain { get; set; }
        public IList<ClientCard> ChainTargets { get; set; }
        public int LastSummonPlayer { get; set; }

        public Duel()
        {
            Fields = new ClientField[2];
            Fields[0] = new ClientField();
            Fields[1] = new ClientField();
            LastChainPlayer = -1;
            CurrentChain = new List<ClientCard>();
            ChainTargets = new List<ClientCard>();
            LastSummonPlayer = -1;
        }

        public ClientCard GetCard(int player, CardLocation loc, int index)
        {
            return GetCard(player, (int)loc, index, 0);
        }

        public ClientCard GetCard(int player, int loc, int index, int subindex)
        {
            if (player < 0 || player > 1)
                return null;

            bool isXyz = (loc & 0x80) != 0;
            CardLocation location = (CardLocation)(loc & 0x7f);

            IList<ClientCard> cards = null;
            switch (location)
            {
                case CardLocation.Deck:
                    cards = Fields[player].Deck;
                    break;
                case CardLocation.Hand:
                    cards = Fields[player].Hand;
                    break;
                case CardLocation.MonsterZone:
                    cards = Fields[player].MonsterZone;
                    break;
                case CardLocation.SpellZone:
                    cards = Fields[player].SpellZone;
                    break;
                case CardLocation.Grave:
                    cards = Fields[player].Graveyard;
                    break;
                case CardLocation.Removed:
                    cards = Fields[player].Banished;
                    break;
                case CardLocation.Extra:
                    cards = Fields[player].ExtraDeck;
                    break;
            }
            if (cards == null)
                return null;

            if (index >= cards.Count)
                return null;

            if (isXyz)
            {
                ClientCard card = cards[index];
                if (card == null || subindex >= card.Overlays.Count)
                    return null;
                return null; // TODO card.Overlays[subindex]
            }

            return cards[index];
        }

        public void AddCard(CardLocation loc, int cardId, int player, int zone, int pos)
        {
            switch (loc)
            {
                case CardLocation.Hand:
                    Fields[player].Hand.Add(new ClientCard(cardId, loc, pos));
                    break;
                case CardLocation.Grave:
                    Fields[player].Graveyard.Add(new ClientCard(cardId, loc, pos));
                    break;
                case CardLocation.Removed:
                    Fields[player].Banished.Add(new ClientCard(cardId, loc, pos));
                    break;
                case CardLocation.MonsterZone:
                    Fields[player].MonsterZone[zone] = new ClientCard(cardId, loc, pos);
                    break;
                case CardLocation.SpellZone:
                    Fields[player].SpellZone[zone] = new ClientCard(cardId, loc, pos);
                    break;
                case CardLocation.Deck:
                    Fields[player].Deck.Add(new ClientCard(cardId, loc, pos));
                    break;
                case CardLocation.Extra:
                    Fields[player].ExtraDeck.Add(new ClientCard(cardId, loc, pos));
                    break;
            }
        }

        public void AddCard(CardLocation loc, ClientCard card, int player, int zone, int pos, int id)
        {
            card.Location = loc;
            card.Position = pos;
            card.SetId(id);
            switch (loc)
            {
                case CardLocation.Hand:
                    Fields[player].Hand.Add(card);
                    break;
                case CardLocation.Grave:
                    Fields[player].Graveyard.Add(card);
                    break;
                case CardLocation.Removed:
                    Fields[player].Banished.Add(card);
                    break;
                case CardLocation.MonsterZone:
                    Fields[player].MonsterZone[zone] = card;
                    break;
                case CardLocation.SpellZone:
                    Fields[player].SpellZone[zone] = card;
                    break;
                case CardLocation.Deck:
                    Fields[player].Deck.Add(card);
                    break;
                case CardLocation.Extra:
                    Fields[player].ExtraDeck.Add(card);
                    break;
            }
        }

        public void RemoveCard(CardLocation loc, ClientCard card, int player, int zone)
        {
            switch (loc)
            {
                case CardLocation.Hand:
                    Fields[player].Hand.Remove(card);
                    break;
                case CardLocation.Grave:
                    Fields[player].Graveyard.Remove(card);
                    break;
                case CardLocation.Removed:
                    Fields[player].Banished.Remove(card);
                    break;
                case CardLocation.MonsterZone:
                    Fields[player].MonsterZone[zone] = null;
                    break;
                case CardLocation.SpellZone:
                    Fields[player].SpellZone[zone] = null;
                    break;
                case CardLocation.Deck:
                    Fields[player].Deck.Remove(card);
                    break;
                case CardLocation.Extra:
                    Fields[player].ExtraDeck.Remove(card);
                    break;
            }
        }

        public int GetLocalPlayer(int player)
        {
            return IsFirst ? player : 1 - player;
        }
    }
}