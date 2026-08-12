using System.Collections.Generic;
using YGOSharp.OCGWrapper.Enums;

namespace WindBot.Game
{
    public class Duel
    {
        public bool IsFirst { get; set; }
        public bool IsNewRule { get; set; }
        public bool IsNewRule2020 { get; set; }
        public bool DeckReversed { get; set; }

        public ClientField[] Fields { get; private set; }

        public int Turn { get; set; }
        public int Player { get; set; }
        public DuelPhase Phase { get; set; }
        public MainPhase MainPhase { get; set; }
        public BattlePhase BattlePhase { get; set; }

        public int LastChainPlayer { get; set; }
        public CardLocation LastChainLocation { get; set; }
        public IList<ClientCard> CurrentChain { get; set; }
        public IList<ChainInfo> CurrentChainInfo { get; set; }
        public IList<ClientCard> ChainTargets { get; set; }
        public IList<ClientCard> LastChainTargets { get; set; }
        public IList<ClientCard> ChainTargetOnly { get; set; }
        public int LastSummonPlayer { get; set; }
        public IList<ClientCard> SummoningCards { get; set; }
        public IList<ClientCard> LastSummonedCards { get; set; }
        public int SolvingChainIndex { get; set; }
        public IList<int> NegatedChainIndexList { get; set; }

        public Duel()
        {
            Fields = new ClientField[2];
            Fields[0] = new ClientField();
            Fields[1] = new ClientField();
            Fields[0].SetOpponent(Fields[1]);
            Fields[1].SetOpponent(Fields[0]);
            LastChainPlayer = -1;
            LastChainLocation = 0;
            CurrentChain = new List<ClientCard>();
            CurrentChainInfo = new List<ChainInfo>();
            ChainTargets = new List<ClientCard>();
            LastChainTargets = new List<ClientCard>();
            ChainTargetOnly = new List<ClientCard>();
            LastSummonPlayer = -1;
            SummoningCards = new List<ClientCard>();
            LastSummonedCards = new List<ClientCard>();
            SolvingChainIndex = 0;
            NegatedChainIndexList = new List<int>();
            MainPhase = new MainPhase();
            BattlePhase = new BattlePhase();
            DeckReversed = false;
        }

        public ClientCard GetCard(int player, CardLocation loc, int seq)
        {
            return GetCard(player, (int)loc, seq, 0);
        }

        public ClientCard GetCard(int player, int loc, int seq, int subSeq)
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

            if (seq >= cards.Count)
                return null;

            if (isXyz)
            {
                ClientCard card = cards[seq];
                if (card == null || subSeq >= card.Overlays.Count)
                    return null;
                ClientCard overlay = new ClientCard(card.Overlays[subSeq], CardLocation.Overlay, 0, 0);
                if (subSeq < card.OverlayOwners.Count)
                    overlay.Owner = card.OverlayOwners[subSeq];
                return overlay;
            }

            return cards[seq];
        }

        public void AddCard(CardLocation loc, int cardId, int player, int seq, int pos)
        {
            ClientCard card = new ClientCard(cardId, loc, seq, pos);
            AddCard(loc, card, player, seq, pos, cardId);
        }

        public void AddCard(CardLocation loc, ClientCard card, int player, int seq, int pos, int id)
        {
            card.Location = loc;
            card.Sequence = seq;
            card.Position = pos;
            card.Controller = player;
            card.SetId(id);
            switch (loc)
            {
                case CardLocation.Hand:
                    Fields[player].Hand.Add(card);
                    ResetSequence(Fields[player].Hand);
                    break;
                case CardLocation.Grave:
                    Fields[player].Graveyard.Add(card);
                    ResetSequence(Fields[player].Graveyard);
                    break;
                case CardLocation.Removed:
                    Fields[player].Banished.Add(card);
                    ResetSequence(Fields[player].Banished);
                    break;
                case CardLocation.MonsterZone:
                    Fields[player].MonsterZone[seq] = card;
                    break;
                case CardLocation.SpellZone:
                    Fields[player].SpellZone[seq] = card;
                    break;
                case CardLocation.Deck:
                    if (seq == 0 && Fields[player].Deck.Count > 0)
                        Fields[player].Deck.Insert(0, card);
                    else
                        Fields[player].Deck.Add(card);
                    ResetSequence(Fields[player].Deck);
                    break;
                case CardLocation.Extra:
                    if (seq >= 0 && seq < Fields[player].ExtraDeck.Count)
                        Fields[player].ExtraDeck.Insert(seq, card);
                    else
                        Fields[player].ExtraDeck.Add(card);
                    ResetSequence(Fields[player].ExtraDeck);
                    break;
            }
        }

        public void RemoveCard(CardLocation loc, ClientCard card, int player, int seq)
        {
            switch (loc)
            {
                case CardLocation.Hand:
                    Fields[player].Hand.Remove(card);
                    ResetSequence(Fields[player].Hand);
                    break;
                case CardLocation.Grave:
                    Fields[player].Graveyard.Remove(card);
                    ResetSequence(Fields[player].Graveyard);
                    break;
                case CardLocation.Removed:
                    Fields[player].Banished.Remove(card);
                    ResetSequence(Fields[player].Banished);
                    break;
                case CardLocation.MonsterZone:
                    Fields[player].MonsterZone[seq] = null;
                    break;
                case CardLocation.SpellZone:
                    Fields[player].SpellZone[seq] = null;
                    break;
                case CardLocation.Deck:
                    Fields[player].Deck.Remove(card);
                    ResetSequence(Fields[player].Deck);
                    break;
                case CardLocation.Extra:
                    Fields[player].ExtraDeck.Remove(card);
                    ResetSequence(Fields[player].ExtraDeck);
                    break;
            }
        }

        private static void ResetSequence(IList<ClientCard> cards)
        {
            for (int i = 0; i < cards.Count; ++i)
                cards[i].Sequence = i;
        }

        public int GetLocalPlayer(int player)
        {
            return IsFirst ? player : 1 - player;
        }

        /// <summary>
        /// Returns the newest chain link while the chain is being built, including
        /// while that link is selecting its activation cost or targets.
        /// Unlike GetCurrentSolvingChainCard, this returns null once chain resolution
        /// has started.
        /// </summary>
        public ClientCard GetCurrentChainCard()
        {
            if (SolvingChainIndex != 0 || CurrentChain.Count == 0) return null;
            return CurrentChain[CurrentChain.Count - 1];
        }

        /// <summary>
        /// Returns the chain link currently being resolved. This is null before the
        /// engine starts resolving the chain, including during activation target selection.
        /// </summary>
        public ClientCard GetCurrentSolvingChainCard()
        {
            if (SolvingChainIndex == 0 || SolvingChainIndex > CurrentChain.Count) return null;
            return CurrentChain[SolvingChainIndex - 1];
        }

        public ChainInfo GetCurrentSolvingChainInfo()
        {
            if (SolvingChainIndex == 0 || SolvingChainIndex > CurrentChainInfo.Count) return null;
            return CurrentChainInfo[SolvingChainIndex - 1];
        }

        public bool IsCurrentSolvingChainNegated()
        {
            return SolvingChainIndex > 0 && NegatedChainIndexList.Contains(SolvingChainIndex);
        }
    }
}
