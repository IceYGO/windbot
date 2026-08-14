using System;
using System.Collections.Generic;
using System.Linq;
using WindBot.Game.AI;
using YGOSharp.OCGWrapper;
using YGOSharp.OCGWrapper.Enums;

namespace WindBot.Game
{
    public class ClientField
    {
        private IDictionary<int, int> _deckCardCounts;
        public bool DeckTrackingActive;

        public IList<ClientCard> Hand { get; private set; }
        public ClientCard[] MonsterZone { get; private set; }
        public ClientCard[] SpellZone { get; private set; }
        public IList<ClientCard> Graveyard { get; private set; }
        public IList<ClientCard> Banished { get; private set; }
        public IList<ClientCard> Deck { get; private set; }
        public IList<ClientCard> ExtraDeck { get; private set; }

        public int LifePoints;
        public ClientCard BattlingMonster;
        public bool UnderAttack;

        public HashSet<int> HintDescriptions { get; private set; }

        public ClientField()
        {
        }

        public void Init(int deck, int extra, int player)
        {
            Hand = new List<ClientCard>();
            MonsterZone = new ClientCard[7];
            SpellZone = new ClientCard[8];
            Graveyard = new List<ClientCard>();
            Banished = new List<ClientCard>();
            Deck = new List<ClientCard>();
            ExtraDeck = new List<ClientCard>();
            HintDescriptions = new HashSet<int>();

            for (int i = 0; i < deck; ++i)
            {
                ClientCard card = new ClientCard(0, CardLocation.Deck, i, (int)CardPosition.FaceDownDefence);
                card.Owner = player;
                card.Controller = player;
                Deck.Add(card);
            }
            for (int i = 0; i < extra; ++i)
            {
                ClientCard card = new ClientCard(0, CardLocation.Extra, i, (int)CardPosition.FaceDownDefence);
                card.Owner = player;
                card.Controller = player;
                ExtraDeck.Add(card);
            }
        }

        public void SetInitialDeck(IEnumerable<NamedCard> cards)
        {
            _deckCardCounts = new Dictionary<int, int>();
            DeckTrackingActive = true;
            foreach (NamedCard card in cards)
                IncrementDeckCardCount(card.Id);
        }

        private void IncrementDeckCardCount(int cardId)
        {
            int count;
            _deckCardCounts.TryGetValue(cardId, out count);
            _deckCardCounts[cardId] = count + 1;
        }

        private int GetTrackedDeckCount(int cardId)
        {
            int remaining = 0;
            bool found = false;
            foreach (KeyValuePair<int, int> pair in _deckCardCounts)
            {
                NamedCard card = NamedCard.Get(pair.Key);
                if (pair.Key != cardId && (card == null || card.Alias != cardId || !NamedCard.IsAltartAlias(card.Id, card.Alias)))
                    continue;
                found = true;
                remaining += pair.Value;
            }
            if (!found)
            {
                // at most cases we don't want to log this
                // Logger.DebugWriteLine($"GetTrackedDeckCount: cardId {cardId} not found in the deck being used.");
            }
            return remaining;
        }

        internal void TrackAddToDeck(int cardId)
        {
            if (!DeckTrackingActive || _deckCardCounts == null)
                return;

            if (cardId == 0)
            {
                Logger.WriteErrorLine("Deck tracking: an unknown card entered the deck.");
                return;
            }
            IncrementDeckCardCount(cardId);
        }

        internal void TrackRemoveFromDeck(int cardId)
        {
            if (!DeckTrackingActive || _deckCardCounts == null)
                return;

            int count = 0;
            if (cardId != 0 && _deckCardCounts.TryGetValue(cardId, out count) && count > 0)
            {
                _deckCardCounts[cardId] = count - 1;
                return;
            }

            // No currently OCG card moves a card directly from one player's Deck to another zone hidden to that player,
            // so an unknown or untracked departure indicates a protocol parsing or state-sync error.
            Logger.WriteErrorLine("Deck tracking: an unknown or untracked card left the deck.");
        }

        internal void TrackReplaceDeck(IEnumerable<ClientCard> cards)
        {
            if (!DeckTrackingActive || _deckCardCounts == null)
                return;

            _deckCardCounts.Clear();
            foreach (ClientCard card in cards)
            {
                if (card == null || card.Id == 0)
                {
                    Logger.WriteErrorLine("Deck tracking: an unknown card was found while replacing the deck.");
                    continue;
                }
                IncrementDeckCardCount(card.Id);
            }
        }

        internal void ValidateTrackedDeckCount(int actualCount)
        {
            if (!DeckTrackingActive || _deckCardCounts == null)
                return;

            int trackedCount = _deckCardCounts.Values.Sum();
            if (trackedCount == actualCount)
                return;

            Logger.WriteErrorLine("Deck tracking count mismatch: tracked=" + trackedCount + ", actual=" + actualCount + ".");
        }

        private bool CanQueryDeck()
        {
            if (_deckCardCounts != null && DeckTrackingActive) return true;
            if (_deckCardCounts != null)
            {
                Logger.DebugWriteLine("Deck contents cannot be queried while this bot's deck is inactive in a tag duel.");
                return false;
            }
            Logger.WriteErrorLine("Deck contents cannot be queried because the opponent's deck is hidden to AI.");
            return false;
        }

        public int GetMonstersExtraZoneCount()
        {
            int count = 0;
            if (MonsterZone[5] != null)
                count++;
            if (MonsterZone[6] != null)
                count++;
            return count;
        }
        public int GetMonsterCount()
        {
            return GetCount(MonsterZone);
        }

        public int GetSpellCount()
        {
            return GetCount(SpellZone);
        }

        public int GetHandCount()
        {
            return GetCount(Hand);
        }

        public int GetSpellCountWithoutField()
        {
            int count = 0;
            for (int i = 0; i < 5; ++i)
            {
                if (SpellZone[i] != null)
                    ++count;
            }
            return count;
        }

        /// <summary>
        /// Count Column
        /// </summary>
        /// <param zone>range of zone 0-4</param>
        public int GetColumnCount(int zone, bool IncludeExtraMonsterZone = true)
        {
            int count = 0;
            if (SpellZone[zone] != null)
                count++;
            if (MonsterZone[zone] != null)
                count++;
            if(zone == 1 && IncludeExtraMonsterZone)
            {
                if (MonsterZone[5] != null)
                    count++;
            }
            if (zone == 3 && IncludeExtraMonsterZone)
            {
                if (MonsterZone[6] != null)
                    count++;
            }
            return count;
        }

        public int GetFieldCount()
        {
            return GetSpellCount() + GetMonsterCount();
        }

        public int GetFieldHandCount()
        {
            return GetSpellCount() + GetMonsterCount() + GetHandCount();
        }

        public bool IsFieldEmpty()
        {
            return GetMonsters().Count == 0 && GetSpells().Count == 0;
        }

        public int GetLinkedZones()
        {
            int zones = 0;
            for (int i = 0; i < 7; i++)
            {
                zones |= MonsterZone[i]?.GetLinkedZones() ?? 0;
            }
            return zones;
        }

        public List<ClientCard> GetMonsters()
        {
            return GetCards(MonsterZone);
        }

        public List<ClientCard> GetGraveyardMonsters()
        {
            return GetCards(Graveyard, CardType.Monster);
        }

        public List<ClientCard> GetGraveyardSpells()
        {
            return GetCards(Graveyard, CardType.Spell);
        }

        public List<ClientCard> GetGraveyardTraps()
        {
            return GetCards(Graveyard, CardType.Trap);
        }

        public List<ClientCard> GetSpells()
        {
            return GetCards(SpellZone);
        }

        public List<ClientCard> GetMonstersInExtraZone()
        {
            return GetMonsters().Where(card => card.Sequence >= 5).ToList();
        }

        public List<ClientCard> GetMonstersInMainZone()
        {
            return GetMonsters().Where(card => card.Sequence < 5).ToList();
        }

        public ClientCard GetFieldSpellCard()
        {
            return SpellZone[5];
        }

        public bool HasInDeck(int cardId)
        {
            if (!CanQueryDeck()) return false;
            return GetTrackedDeckCount(cardId) > 0;
        }

        public bool HasInDeck(params int[] cardIds)
        {
            if (!CanQueryDeck()) return false;
            return cardIds.Any(id => GetTrackedDeckCount(id) > 0);
        }

        public bool HasInHand(int cardId)
        {
            return HasInCards(Hand, cardId);
        }

        public bool HasInHand(IList<int> cardId)
        {
            return HasInCards(Hand, cardId);
        }

        public bool HasInGraveyard(int cardId)
        {
            return HasInCards(Graveyard, cardId);
        }
    
        public bool HasInGraveyard(IList<int> cardId)
        {
            return HasInCards(Graveyard, cardId);
        }

        public bool HasInBanished(int cardId)
        {
            return HasInCards(Banished, cardId);
        }

        public bool HasInBanished(IList<int> cardId)
        {
            return HasInCards(Banished, cardId);
        }

        public bool HasInExtra(int cardId)
        {
            return HasInCards(ExtraDeck, cardId);
        }

        public bool HasInExtra(IList<int> cardId)
        {
            return HasInCards(ExtraDeck, cardId);
        }

        public bool HasAttackingMonster(bool strict = false)
        {
            return GetMonsters().Any(card => card.IsAttack() || (!strict && card.IsMonsterAttackWhileInDefPos()));
        }

        public bool HasDefendingMonster()
        {
            return GetMonsters().Any(card => card.IsDefense());
        }

        public bool HasInMonstersZone(int cardId, bool notDisabled = false, bool hasXyzMaterial = false, bool faceUp = false)
        {
            return HasInCards(MonsterZone, cardId, notDisabled, hasXyzMaterial, faceUp);
        }

        public bool HasInMonstersZone(IList<int> cardId, bool notDisabled = false, bool hasXyzMaterial = false, bool faceUp = false)
        {
            return HasInCards(MonsterZone, cardId, notDisabled, hasXyzMaterial, faceUp);
        }

        public bool HasInSpellZone(int cardId, bool notDisabled = false, bool faceUp = false)
        {
            return HasInCards(SpellZone, cardId, notDisabled, false, faceUp);
        }

        public bool HasInSpellZone(IList<int> cardId, bool notDisabled = false, bool faceUp = false)
        {
            return HasInCards(SpellZone, cardId, notDisabled, false, faceUp);
        }

        public bool HasInHandOrInSpellZone(int cardId)
        {
            return HasInHand(cardId) || HasInSpellZone(cardId);
        }

        public bool HasInHandOrHasInMonstersZone(int cardId)
        {
            return HasInHand(cardId) || HasInMonstersZone(cardId);
        }

        public bool HasInHandOrInGraveyard(int cardId)
        {
            return HasInHand(cardId) || HasInGraveyard(cardId);
        }

        public bool HasInGraveyardOrInBanished(int cardId)
        {
            return HasInBanished(cardId) || HasInGraveyard(cardId);
        }

        public bool HasInMonstersZoneOrInGraveyard(int cardId)
        {
            return HasInMonstersZone(cardId) || HasInGraveyard(cardId);
        }

        public bool HasInSpellZoneOrInGraveyard(int cardId)
        {
            return HasInSpellZone(cardId) || HasInGraveyard(cardId);
        }

        public bool HasInHandOrInMonstersZoneOrInGraveyard(int cardId)
        {
            return HasInHand(cardId) || HasInMonstersZone(cardId) || HasInGraveyard(cardId);
        }

        public bool HasInHandOrInSpellZoneOrInGraveyard(int cardId)
        {
            return HasInHand(cardId) || HasInSpellZone(cardId) || HasInGraveyard(cardId);
        }

        public bool HasInHandOrInSpellZone(IList<int> cardId)
        {
            return HasInHand(cardId) || HasInSpellZone(cardId);
        }

        public bool HasInHandOrHasInMonstersZone(IList<int> cardId)
        {
            return HasInHand(cardId) || HasInMonstersZone(cardId);
        }

        public bool HasInHandOrInGraveyard(IList<int> cardId)
        {
            return HasInHand(cardId) || HasInGraveyard(cardId);
        }

        public bool HasInMonstersZoneOrInGraveyard(IList<int> cardId)
        {
            return HasInMonstersZone(cardId) || HasInGraveyard(cardId);
        }

        public bool HasInSpellZoneOrInGraveyard(IList<int> cardId)
        {
            return HasInSpellZone(cardId) || HasInGraveyard(cardId);
        }

        public bool HasInHandOrInMonstersZoneOrInGraveyard(IList<int> cardId)
        {
            return HasInHand(cardId) || HasInMonstersZone(cardId) || HasInGraveyard(cardId);
        }

        public bool HasInHandOrInSpellZoneOrInGraveyard(IList<int> cardId)
        {
            return HasInHand(cardId) || HasInSpellZone(cardId) || HasInGraveyard(cardId);
        }

        /// <summary>
        /// Deprecated. Will be removed in the future.
        /// </summary>
        /// <param name="initialCount">The param is ignored now.</param>
        [Obsolete("Use GetCardCountInDeck instead.")]
        public int GetRemainingCount(int cardId, int initialCount = 0)
        {
            return GetCardCountInDeck(cardId);
        }

        public int GetCardCountInDeck(int cardId)
        {
            if (!CanQueryDeck()) return 0;
            return GetTrackedDeckCount(cardId);
        }

        public int GetCardCountInDeck(IList<int> cardIds)
        {
            if (!CanQueryDeck()) return 0;
            return cardIds.Sum(id => GetTrackedDeckCount(id));
        }

        private static int GetCount(IEnumerable<ClientCard> cards)
        {
            return cards.Count(card => card != null);
        }

        public int GetCountCardInZone(IEnumerable<ClientCard> cards, int cardId)
        {
            return cards.Count(card => card != null && card.IsCode(cardId));
        }

        public int GetCountCardInZone(IEnumerable<ClientCard> cards, List<int> cardId)
        {
            return cards.Count(card => card != null && card.IsCode(cardId));
        }

        private static List<ClientCard> GetCards(IEnumerable<ClientCard> cards, CardType type)
        {
            return cards.Where(card => card != null && card.HasType(type)).ToList();
        }

        private static List<ClientCard> GetCards(IEnumerable<ClientCard> cards)
        {
            return cards.Where(card => card != null).ToList();
        }

        private static bool HasInCards(IEnumerable<ClientCard> cards, int cardId, bool notDisabled = false, bool hasXyzMaterial = false, bool faceUp = false)
        {
            return cards.Any(card => card != null && card.IsCode(cardId) && !(notDisabled && card.IsDisabled()) && !(hasXyzMaterial && !card.HasXyzMaterial()) && !(faceUp && card.IsFacedown()));
        }

        private static bool HasInCards(IEnumerable<ClientCard> cards, IList<int> cardId, bool notDisabled = false, bool hasXyzMaterial = false, bool faceUp = false)
        {
            return cards.Any(card => card != null && card.IsCode(cardId) && !(notDisabled && card.IsDisabled()) && !(hasXyzMaterial && !card.HasXyzMaterial()) && !(faceUp && card.IsFacedown()));
        }
    }
}
