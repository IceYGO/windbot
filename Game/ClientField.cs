using System.Collections.Generic;
using System.Linq;
using WindBot.Game.AI;
using YGOSharp.OCGWrapper;
using YGOSharp.OCGWrapper.Enums;

namespace WindBot.Game
{
    public class ClientField
    {
        private int _player;
        private ClientField _opponent;
        private IDictionary<int, int> _initialDeckCounts;

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
            _player = player;
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

        internal void SetOpponent(ClientField opponent)
        {
            _opponent = opponent;
        }

        public void SetInitialDeck(IEnumerable<NamedCard> cards)
        {
            _initialDeckCounts = new Dictionary<int, int>();
            foreach (NamedCard card in cards)
            {
                IncrementInitialDeckCount(card.Id);
                if (card.Alias != 0 && System.Math.Abs(card.Alias - card.Id) < 20)
                    IncrementInitialDeckCount(card.Alias);
            }
        }

        private void IncrementInitialDeckCount(int cardId)
        {
            int count = 0;
            _initialDeckCounts.TryGetValue(cardId, out count);
            _initialDeckCounts[cardId] = count + 1;
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

        /// <summary>
        /// Checks if the deck contains a specific card.
        /// The bot can only check this by counting the appearances of the card outside the deck.
        /// </summary>
        public bool HasInDeck(int cardId)
        {
            if (!CanQueryDeck()) return false;
            return GetRemainingCount(cardId) > 0;
        }

        /// <summary>
        /// Checks if the deck contains specific cards.
        /// The bot can only check this by counting the appearances of the card outside the deck.
        /// </summary>
        public bool HasInDeck(params int[] cardIds)
        {
            if (!CanQueryDeck()) return false;
            return cardIds.Any(id => GetRemainingCount(id) > 0);
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
        /// Use GetRemainingCount(int cardId) instead, which counts based on the initial deck.
        /// </summary>
        /// <param name="initalCount_deprecated">The param is ignored.</param>
        public int GetRemainingCount(int cardId, int initalCount_deprecated)
        {
            return GetRemainingCount(cardId);
        }

        public int GetRemainingCount(int cardId)
        {
            int remaining = 0;
            if (_initialDeckCounts?.TryGetValue(cardId, out remaining) != true)
                Logger.DebugWriteLine($"GetRemainingCount: cardId {cardId} not found in the deck being used.");

            // Known limitation: In tag duels, Owner identifies only the team. Cards left by a teammate in
            // shared zones may therefore be subtracted from this bot's initial deck count.
            remaining -= Hand.Count(card => card != null && card.Owner == _player && card.IsOriginalCode(cardId));
            remaining -= SpellZone.Count(card => card != null && card.Owner == _player && card.IsOriginalCode(cardId));
            remaining -= MonsterZone.Count(card => card != null && card.Owner == _player && card.IsOriginalCode(cardId));
            remaining -= Graveyard.Count(card => card != null && card.Owner == _player && card.IsOriginalCode(cardId));
            remaining -= Banished.Count(card => card != null && card.Owner == _player && card.IsOriginalCode(cardId));
            remaining -= ExtraDeck.Count(card => card != null && card.Owner == _player && card.IsFaceup() && card.IsOriginalCode(cardId));
            remaining -= MonsterZone.Where(card => card != null).Sum(card => CountMatchingOverlays(card, cardId));

            // Known limitation: Cards moved into an opponent's hand or deck cannot be matched to stable slots
            // after a shuffle, so those known copies are not included here.
            if (_opponent != null && _opponent.MonsterZone != null)
            {
                remaining -= _opponent.SpellZone.Count(card => card != null && card.Owner == _player && card.IsOriginalCode(cardId));
                remaining -= _opponent.MonsterZone.Count(card => card != null && card.Owner == _player && card.IsOriginalCode(cardId));
                remaining -= _opponent.MonsterZone.Where(card => card != null).Sum(card => CountMatchingOverlays(card, cardId));
            }
            return (remaining < 0) ? 0 : remaining;
        }

        public int GetRemainingCount(IList<int> cardIds) // params int[] will conflict with deprecated initalCount
        {
            return cardIds.Sum(id => GetRemainingCount(id));
        }

        private int CountMatchingOverlays(ClientCard card, int cardId)
        {
            int count = 0;
            for (int i = 0; i < card.Overlays.Count; ++i)
            {
                if (i >= card.OverlayOwners.Count || card.OverlayOwners[i] != _player)
                    continue;

                int overlayId = card.Overlays[i];
                NamedCard overlayData = NamedCard.Get(overlayId);
                if (overlayId == cardId || (overlayData != null && System.Math.Abs(overlayData.Alias - overlayId) < 20 && overlayData.Alias == cardId))
                    count++;
            }
            return count;
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

        private bool CanQueryDeck()
        {
            if (_initialDeckCounts != null) return true;
            Logger.WriteErrorLine("Enemy.HasInDeck cannot be used because the opponent's deck is hidden to AI.");
            return false;
        }
    }
}
