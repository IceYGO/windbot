using OCGWrapper.Enums;
using System.Collections.Generic;

namespace WindBot.Game.AI.Decks
{
    [Deck("Horus", "AI_Horus")]
    public class HorusExecutor : DefaultExecutor
    {
        public enum CardId
        {
            AlexandriteDragon = 43096270,
            LusterDragon = 11091375,
            WhiteNightDragon = 79473793,
            HorusTheBlackFlameDragonLv8 = 48229808,
            HorusTheBlackFlameDragonLv6 = 11224103,
            CyberDragon = 70095154,
            AxeDragonute = 84914462,
            DodgerDragon = 47013502,
            GolemDragon = 9666558,
            Raigeki = 12580477,
            HammerShot = 26412047,
            DarkHole = 53129443,
            Fissure = 66788016,
            StampingDestruction = 81385346,
            FoolishBurial = 81439173,
            MonsterReborn = 83764718,
            MysticalSpaceTyphoon = 5318639,
            BellowOfTheSilverDragon = 80600103,
            Mountain = 50913601,
            DragonsRebirth = 20638610,
            MirrorForce = 44095762,
            DimensionalPrison = 70342110
        }

        public HorusExecutor(GameAI ai, Duel duel) : base(ai, duel)
        {
            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);

            AddExecutor(ExecutorType.Activate, (int)CardId.HorusTheBlackFlameDragonLv6);
            AddExecutor(ExecutorType.Activate, (int)CardId.StampingDestruction, DefaultStampingDestruction);
            AddExecutor(ExecutorType.Activate, (int)CardId.MysticalSpaceTyphoon, DefaultMysticalSpaceTyphoon);
            AddExecutor(ExecutorType.Activate, (int)CardId.FoolishBurial, FoolishBurial);
            AddExecutor(ExecutorType.Activate, (int)CardId.Mountain, DefaultField);
            AddExecutor(ExecutorType.Activate, (int)CardId.DarkHole, DefaultDarkHole);
            AddExecutor(ExecutorType.Activate, (int)CardId.Raigeki, DefaultRaigeki);
            AddExecutor(ExecutorType.Activate, (int)CardId.HammerShot, DefaultHammerShot);
            AddExecutor(ExecutorType.Activate, (int)CardId.Fissure);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.CyberDragon);
            AddExecutor(ExecutorType.Activate, (int)CardId.BellowOfTheSilverDragon, BellowOfTheSilverDragon);
            AddExecutor(ExecutorType.Activate, (int)CardId.MonsterReborn, MonsterReborn);

            AddExecutor(ExecutorType.Summon, (int)CardId.WhiteNightDragon, WhiteNightDragon);
            AddExecutor(ExecutorType.Summon, (int)CardId.HorusTheBlackFlameDragonLv6, DefaultTributeSummon);
            AddExecutor(ExecutorType.Summon, (int)CardId.AlexandriteDragon);
            AddExecutor(ExecutorType.SummonOrSet, (int)CardId.AxeDragonute);
            AddExecutor(ExecutorType.SummonOrSet, (int)CardId.DodgerDragon);
            AddExecutor(ExecutorType.MonsterSet, (int)CardId.GolemDragon);
            AddExecutor(ExecutorType.SummonOrSet, (int)CardId.LusterDragon);

            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);

            AddExecutor(ExecutorType.Activate, (int)CardId.HorusTheBlackFlameDragonLv8, HorusTheBlackFlameDragonLv8);
            AddExecutor(ExecutorType.Activate, (int)CardId.MirrorForce, DefaultTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.DimensionalPrison, DefaultTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.DragonsRebirth, DragonsRebirth);
        }

        private bool FoolishBurial()
        {
            if (Duel.Fields[0].HasInGraveyard((int)CardId.WhiteNightDragon))
                return false;
            if (Duel.Fields[0].HasInHand((int)CardId.WhiteNightDragon))
                return false;
            int remaining = 2;
            foreach (ClientCard card in Duel.Fields[0].Banished)
                if (card.Id == (int)CardId.WhiteNightDragon)
                    remaining--;
            if (remaining > 0)
            {
                AI.SelectCard((int)CardId.WhiteNightDragon);
                return true;
            }
            return false;
        }

        private bool BellowOfTheSilverDragon()
        {
            if (Duel.Player == 0 && (Duel.Phase == DuelPhase.Draw || Duel.Phase == DuelPhase.Standby))
                return false;
            if (Duel.Player == 1 && Duel.Phase == DuelPhase.End)
                return false;

            List<ClientCard> cards = new List<ClientCard>(Duel.Fields[0].Graveyard);
            cards.Sort(AIFunctions.CompareCardAttack);
            for (int i = cards.Count - 1; i >= 0; --i)
            {
                ClientCard card = cards[i];
                if (card.Attack < 1000)
                    return false;
                if (card.IsMonster() && card.HasType(CardType.Normal))
                {
                    AI.SelectCard(card);
                    return true;
                }
            }
            return false;
        }

        private bool MonsterReborn()
        {
            List<ClientCard> cards = new List<ClientCard>(Duel.Fields[0].Graveyard);
            cards.Sort(AIFunctions.CompareCardAttack);
            ClientCard selectedCard = null;
            for (int i = cards.Count - 1; i >= 0; --i)
            {
                ClientCard card = cards[i];
                if (card.Attack < 1000)
                    break;
                if (card.IsMonster())
                {
                    selectedCard = card;
                    break;
                }
            }
            cards = new List<ClientCard>(Duel.Fields[1].Graveyard);
            cards.Sort(AIFunctions.CompareCardAttack);
            for (int i = cards.Count - 1; i >= 0; --i)
            {
                ClientCard card = cards[i];
                if (card.Attack < 1000)
                    break;
                if (card.IsMonster() && card.HasType(CardType.Normal) && (selectedCard == null || card.Attack > selectedCard.Attack))
                {
                    selectedCard = card;
                    break;
                }
            }
            if (selectedCard != null)
            {
                AI.SelectCard(selectedCard);
                return true;
            }
            return false;
        }

        private bool WhiteNightDragon()
        {
            // We should summon Horus the Black Flame Dragon LV6 if he can lvlup.
            if (Duel.Fields[1].GetMonsterCount() != 0 && !AI.Utils.IsAllEnnemyBetterThanValue(2300 - 1, false))
                foreach (ClientCard card in Main.SummonableCards)
                    if (card.Id == 11224103)
                        return false;

            return DefaultTributeSummon();
        }

        private bool HorusTheBlackFlameDragonLv8()
        {
            return LastChainPlayer == 1;
        }

        private bool DragonsRebirth()
        {
            List<ClientCard> cards = new List<ClientCard>(Duel.Fields[0].GetMonsters());
            if (cards.Count == 0)
                return false;
            cards.Sort(AIFunctions.CompareCardAttack);
            ClientCard tributeCard = null;
            foreach (ClientCard monster in cards)
            {
                if (monster.Attack > 2000)
                    return false;
                if (!monster.IsFacedown() && monster.Race == (int)CardRace.Dragon)
                {
                    tributeCard = monster;
                    break;
                }
            }

            if (tributeCard == null)
                return false;

            cards = new List<ClientCard>(Duel.Fields[0].Hand);
            cards.AddRange(Duel.Fields[0].Graveyard);
            if (cards.Count == 0)
                return false;
            cards.Sort(AIFunctions.CompareCardAttack);
            ClientCard summonCard = null;
            for (int i = cards.Count - 1; i >= 0; --i)
            {
                ClientCard monster = cards[i];
                if (monster.Attack < 2300)
                    return false;
                if (monster.Race == (int)CardRace.Dragon && monster.Id != (int)CardId.HorusTheBlackFlameDragonLv8)
                {
                    summonCard = monster;
                    break;
                }
            }

            if (summonCard == null)
                return false;

            AI.SelectCard(tributeCard);
            AI.SelectNextCard(summonCard);

            return true;
        }
    }
}