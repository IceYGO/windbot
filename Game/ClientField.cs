using System.Collections.Generic;
using YGOSharp.OCGWrapper.Enums;

namespace WindBot.Game
{
    public class ClientField
    {
        public IList<ClientCard> Hand { get; private set; }
        public ClientCard[] MonsterZone { get; private set; }
        public ClientCard[] SpellZone { get; private set; }
        public IList<ClientCard> Graveyard { get; private set; }
        public IList<ClientCard> Banished { get; private set; }
        public IList<ClientCard> Deck { get; private set; }
        public IList<ClientCard> ExtraDeck { get; private set; }

        public int LifePoints;
        public ClientCard BattlingMonster;

        public ClientField()
        {
        }

        public void Init(int deck, int extra)
        {
            Hand = new List<ClientCard>();
            MonsterZone = new ClientCard[7];
            SpellZone = new ClientCard[8];
            Graveyard = new List<ClientCard>();
            Banished = new List<ClientCard>();
            Deck = new List<ClientCard>();
            ExtraDeck = new List<ClientCard>();

            for (int i = 0; i < deck; ++i)
                Deck.Add(new ClientCard(0, CardLocation.Deck));
            for (int i = 0; i < extra; ++i)
                ExtraDeck.Add(new ClientCard(0, CardLocation.Extra));
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
            List<ClientCard> cards = new List<ClientCard>();
            if (MonsterZone[5] != null)
                cards.Add(MonsterZone[5]);
            if (MonsterZone[6] != null)
                cards.Add(MonsterZone[6]);
            return cards;
        }

        public List<ClientCard> GetMonstersInMainZone()
        {
            List<ClientCard> cards = new List<ClientCard>();
            for (int i = 0; i < 5; i++)
            {
                if (MonsterZone[i] != null)
                    cards.Add(MonsterZone[i]);
            }
            return cards;
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

        public bool HasAttackingMonster()
        {
            IList<ClientCard> monsters = GetMonsters();
            foreach (ClientCard card in monsters)
            {
                if (card.IsAttack())
                    return true;
            }
            return false;
        }

        public bool HasDefendingMonster()
        {
            IList<ClientCard> monsters = GetMonsters();
            foreach (ClientCard card in monsters)
            {
                if (card.IsDefense())
                    return true;
            }
            return false;
        }

        public bool HasInMonstersZone(int cardId, bool notDisabled = false, bool hasXyzMaterial = false)
        {
            return HasInCards(MonsterZone, cardId, notDisabled, hasXyzMaterial);
        }

        public bool HasInMonstersZone(IList<int> cardId, bool notDisabled = false, bool hasXyzMaterial = false)
        {
            return HasInCards(MonsterZone, cardId, notDisabled, hasXyzMaterial);
        }

        public bool HasInSpellZone(int cardId, bool notDisabled = false)
        {
            return HasInCards(SpellZone, cardId, notDisabled);
        }

        public bool HasInSpellZone(IList<int> cardId, bool notDisabled = false)
        {
            return HasInCards(SpellZone, cardId, notDisabled);
        }

        public int GetRemainingCount(int cardId, int initialCount)
        {
            int remaining = initialCount;
            foreach (ClientCard card in Hand)
                if (card != null && card.Id == cardId)
                    remaining--;
            foreach (ClientCard card in SpellZone)
                if (card != null && card.Id == cardId)
                    remaining--;
            foreach (ClientCard card in Graveyard)
                if (card != null && card.Id == cardId)
                    remaining--;
            foreach (ClientCard card in Banished)
                if (card != null && card.Id == cardId)
                    remaining--;
            return (remaining < 0) ? 0 : remaining;
        }

        private static int GetCount(IEnumerable<ClientCard> cards)
        {
            int count = 0;
            foreach (ClientCard card in cards)
            {
                if (card != null)
                    count++;
            }
            return count;
        }

        public int GetCountCardInZone(IEnumerable<ClientCard> cards, int cardId)
        {
            int count = 0;
            foreach (ClientCard card in cards)
            {
                if (card != null && card.Id == cardId)
                    count++;
            }
            return count;
        }

        public int GetCountCardInZone(IEnumerable<ClientCard> cards, List<int> cardId)
        {
            int count = 0;
            foreach (ClientCard card in cards)
            {
                if (card != null && cardId.Contains(card.Id))
                    count++;
            }
            return count;
        }

        private static List<ClientCard> GetCards(IEnumerable<ClientCard> cards, CardType type)
        {
            List<ClientCard> nCards = new List<ClientCard>();
            foreach (ClientCard card in cards)
            {
                if (card != null && card.HasType(type))
                    nCards.Add(card);
            }
            return nCards;
        }

        private static List<ClientCard> GetCards(IEnumerable<ClientCard> cards)
        {
            List<ClientCard> nCards = new List<ClientCard>();
            foreach (ClientCard card in cards)
            {
                if (card != null)
                    nCards.Add(card);
            }
            return nCards;
        }

        private static bool HasInCards(IEnumerable<ClientCard> cards, int cardId, bool notDisabled = false, bool hasXyzMaterial = false)
        {
            foreach (ClientCard card in cards)
            {
                if (card != null && card.Id == cardId && !(notDisabled && card.IsDisabled()) && !(hasXyzMaterial && !card.HasXyzMaterial()))
                    return true;
            }
            return false;
        }

        private static bool HasInCards(IEnumerable<ClientCard> cards, IList<int> cardId, bool notDisabled = false, bool hasXyzMaterial = false)
        {
            foreach (ClientCard card in cards)
            {
                if (card != null && cardId.Contains(card.Id) && !(notDisabled && card.IsDisabled()) && !(hasXyzMaterial && !card.HasXyzMaterial()))
                    return true;
            }
            return false;
        }
    }
}