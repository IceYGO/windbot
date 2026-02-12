using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using YGOSharp.OCGWrapper.Enums;

namespace WindBot.Game.AI.Decks
{
    [Deck("Dragunity", "AI_Dragunity")]
    public class DragunityExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int StardustDragonAssaultMode = 61257789;
            public const int DragunityArmaMysletainn = 876330;
            public const int AssaultBeast = 3431737;
            public const int DragunityDux = 28183605;
            public const int DragunityPhalanx = 59755122;
            public const int AssaultTeleport = 29863101;
            public const int CardsOfConsonance = 39701395;
            public const int UpstartGoblin = 70368879;
            public const int DragonsMirror = 71490127;
            public const int Terraforming = 73628505;
            public const int FoolishBurial = 81439173;
            public const int MonsterReborn = 83764718;
            public const int MysticalSpaceTyphoon = 5318639;
            public const int FireFormationTenki = 57103969;
            public const int DragunitySpearOfDestiny = 60004971;
            public const int DragonRavine = 62265044;
            public const int MirrorForce = 44095762;
            public const int StarlightRoad = 58120309;
            public const int DimensionalPrison = 70342110;
            public const int AssaultModeActivate = 80280737;
            public const int FiveHeadedDragon = 99267150;
            public const int CrystalWingSynchroDragon = 50954680;
            public const int ScrapDragon = 76774528;
            public const int StardustDragon = 44508094;
            public const int DragunityKnightGaeDearg = 34116027;
            public const int DragunityKnightVajrayana = 21249921;
        }

        public DragunityExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // Set traps
            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);

            // Execute spells
            AddExecutor(ExecutorType.Activate, CardId.MysticalSpaceTyphoon, DefaultMysticalSpaceTyphoon);
            AddExecutor(ExecutorType.Activate, CardId.AssaultTeleport);
            AddExecutor(ExecutorType.Activate, CardId.UpstartGoblin);
            AddExecutor(ExecutorType.Activate, CardId.DragonRavine, DragonRavineField);
            AddExecutor(ExecutorType.Activate, CardId.Terraforming, Terraforming);
            AddExecutor(ExecutorType.Activate, CardId.FoolishBurial, FoolishBurial);
            AddExecutor(ExecutorType.Activate, CardId.MonsterReborn, MonsterReborn);

            // Execute monsters
            AddExecutor(ExecutorType.Activate, CardId.ScrapDragon, ScrapDragonEffect);
            AddExecutor(ExecutorType.Activate, CardId.CrystalWingSynchroDragon, CrystalWingSynchroDragonEffect);
            AddExecutor(ExecutorType.Activate, CardId.DragunityPhalanx);
            AddExecutor(ExecutorType.Activate, CardId.DragunityKnightVajrayana);
            AddExecutor(ExecutorType.Activate, CardId.DragunityArmaMysletainn, DragunityArmaMysletainnEffect);
            AddExecutor(ExecutorType.Activate, CardId.DragunityDux);

            // Summon
            AddExecutor(ExecutorType.Activate, CardId.DragonsMirror, DragonsMirror);
            AddExecutor(ExecutorType.SpSummon, CardId.ScrapDragon, ScrapDragonSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.CrystalWingSynchroDragon, CrystalWingSynchroDragonSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.StardustDragon);
            AddExecutor(ExecutorType.SpSummon, CardId.DragunityKnightVajrayana);
            AddExecutor(ExecutorType.SpSummon, CardId.DragunityKnightGaeDearg);
            AddExecutor(ExecutorType.Summon, CardId.DragunityPhalanx, DragunityPhalanxSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.DragunityArmaMysletainn, DragunityArmaMysletainn);
            AddExecutor(ExecutorType.Summon, CardId.DragunityArmaMysletainn, DragunityArmaMysletainnTribute);

            // Use draw effects if we can't do anything else
            AddExecutor(ExecutorType.Activate, CardId.CardsOfConsonance);
            AddExecutor(ExecutorType.Activate, CardId.DragonRavine, DragonRavineEffect);
            AddExecutor(ExecutorType.Activate, CardId.FireFormationTenki, FireFormationTenki);
            AddExecutor(ExecutorType.Activate, CardId.DragunitySpearOfDestiny);

            // Summon
            AddExecutor(ExecutorType.Summon, CardId.DragunityDux, DragunityDux);
            AddExecutor(ExecutorType.MonsterSet, CardId.DragunityPhalanx, DragunityPhalanxSet);
            AddExecutor(ExecutorType.SummonOrSet, CardId.AssaultBeast);

            // Draw assault mode if we don't have one
            AddExecutor(ExecutorType.Activate, CardId.AssaultBeast, AssaultBeast);

            // Set useless cards
            AddExecutor(ExecutorType.SpellSet, CardId.DragonsMirror, SetUselessCards);
            AddExecutor(ExecutorType.SpellSet, CardId.Terraforming, SetUselessCards);
            AddExecutor(ExecutorType.SpellSet, CardId.AssaultTeleport, SetUselessCards);
            AddExecutor(ExecutorType.SpellSet, CardId.CardsOfConsonance, SetUselessCards);

            // Chain traps and monsters
            AddExecutor(ExecutorType.Activate, CardId.StardustDragonAssaultMode, DefaultStardustDragonEffect);
            AddExecutor(ExecutorType.Activate, CardId.StardustDragon, DefaultStardustDragonEffect);
            AddExecutor(ExecutorType.Activate, CardId.StarlightRoad, DefaultTrap);
            AddExecutor(ExecutorType.Activate, CardId.MirrorForce, DefaultTrap);
            AddExecutor(ExecutorType.Activate, CardId.DimensionalPrison, DefaultTrap);
            AddExecutor(ExecutorType.Activate, CardId.AssaultModeActivate, AssaultModeActivate);

            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
        }

        private bool DragonRavineField()
        {
            if (Card.Location == CardLocation.Hand)
                return DefaultField();
            return false;
        }

        private bool DragonRavineEffect()
        {
            if (Card.Location != CardLocation.SpellZone)
                return false;

            int tributeId = -1;
            if (Bot.HasInHand(CardId.DragunityPhalanx))
                tributeId = CardId.DragunityPhalanx;
            else if (Bot.HasInHand(CardId.FireFormationTenki))
                tributeId = CardId.FireFormationTenki;
            else if (Bot.HasInHand(CardId.Terraforming))
                tributeId = CardId.Terraforming;
            else if (Bot.HasInHand(CardId.DragonRavine))
                tributeId = CardId.DragonRavine;
            else if (Bot.HasInHand(CardId.AssaultTeleport))
                tributeId = CardId.AssaultTeleport;
            else if (Bot.HasInHand(CardId.AssaultBeast))
                tributeId = CardId.AssaultBeast;
            else if (Bot.HasInHand((int) CardId.DragunityArmaMysletainn))
                tributeId = CardId.DragunityArmaMysletainn;
            else
            {
                int count = 0;
                foreach (ClientCard card in Bot.Hand)
                {
                    if (card.IsCode(CardId.DragunityDux))
                        ++count;
                }
                if (count >= 2)
                    tributeId = CardId.DragunityDux;
            }
            if (tributeId == -1 && Bot.HasInHand(CardId.StardustDragonAssaultMode))
                tributeId = CardId.StardustDragonAssaultMode;
            if (tributeId == -1 && Bot.HasInHand(CardId.DragunitySpearOfDestiny))
                tributeId = CardId.StardustDragonAssaultMode;
            if (tributeId == -1 && Bot.HasInHand(CardId.DragonsMirror)
                && Bot.GetMonsterCount() == 0)
                tributeId = CardId.StardustDragonAssaultMode;

            if (tributeId == -1)
                return false;

            int needId = -1;
            if (!Bot.HasInMonstersZone(CardId.DragunityPhalanx) &&
                !Bot.HasInGraveyard(CardId.DragunityPhalanx))
                needId = CardId.DragunityPhalanx;
            else if (Bot.GetMonsterCount() == 0)
                needId = CardId.DragunityDux;
            else
            {
                /*bool hasRealMonster = false;
                foreach (ClientCard card in Bot.GetMonsters())
                {
                    if (!card.IsCode(CardId.AssaultBeast))
                    {
                        hasRealMonster = true;
                        break;
                    }
                }
                if (!hasRealMonster || Util.GetProblematicCard() != null)*/
                needId = CardId.DragunityDux;
            }

            if (needId == -1)
                return false;

            int option;

            if (tributeId == CardId.DragunityPhalanx)
                needId = CardId.DragunityDux;

            int remaining = 3;
            foreach (ClientCard card in Bot.Hand)
                if (card.IsCode(needId))
                    remaining--;
            foreach (ClientCard card in Bot.Graveyard)
                if (card.IsCode(needId))
                    remaining--;
            foreach (ClientCard card in Bot.Banished)
                if (card.IsCode(needId))
                    remaining--;
            if (remaining <= 0)
                return false;

            if (needId == CardId.DragunityPhalanx)
                option = 2;
            else
                option = 1;

            if (ActivateDescription != Util.GetStringId(CardId.DragonRavine, option))
                return false;

            AI.SelectCard(tributeId);
            AI.SelectNextCard(needId);

            return true;
        }

        private bool Terraforming()
        {
            if (Bot.HasInHand(CardId.DragonRavine))
                return false;
            if (Bot.SpellZone[5] != null)
                return false;
            return true;
        }

        private bool SetUselessCards()
        {
            ClientField field = Bot;

            if (field.HasInSpellZone(CardId.FireFormationTenki))
                return false;
            if (field.HasInSpellZone(CardId.AssaultTeleport))
                return false;
            if (field.HasInSpellZone(CardId.CardsOfConsonance))
                return false;
            if (field.HasInSpellZone(CardId.DragonsMirror))
                return false;

            return Bot.GetSpellCountWithoutField() < 4;
        }

        private bool FireFormationTenki()
        {
            if (Card.Location == CardLocation.Hand)
                return Bot.GetSpellCountWithoutField() < 4;
            return true;
        }

        private bool FoolishBurial()
        {
            AI.SelectCard(
                CardId.DragunityPhalanx,
                CardId.AssaultBeast,
                CardId.StardustDragonAssaultMode
                );
            return true;
        }

        private bool MonsterReborn()
        {
            List<ClientCard> cards = new List<ClientCard>(Bot.Graveyard.GetMatchingCards(card => card.IsCanRevive()));
            cards.Sort(CardContainer.CompareCardAttack);
            ClientCard selectedCard = null;
            for (int i = cards.Count - 1; i >= 0; --i)
            {
                ClientCard card = cards[i];
                if (card.Attack < 2000)
                    break;
                if (card.IsCode(CardId.StardustDragonAssaultMode, CardId.FiveHeadedDragon))
                    continue;
                if (card.IsMonster())
                {
                    selectedCard = card;
                    break;
                }
            }
            cards = new List<ClientCard>(Enemy.Graveyard.GetMatchingCards(card => card.IsCanRevive()));
            cards.Sort(CardContainer.CompareCardAttack);
            for (int i = cards.Count - 1; i >= 0; --i)
            {
                ClientCard card = cards[i];
                if (card.Attack < 2000)
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

        private bool DragonsMirror()
        {
            IList<ClientCard> tributes = new List<ClientCard>();
            int phalanxCount = 0;
            foreach (ClientCard card in Bot.Graveyard)
            {
                if (card.IsCode(CardId.DragunityPhalanx))
                {
                    phalanxCount++;
                    break;
                }
                if (card.Race == (int) CardRace.Dragon)
                    tributes.Add(card);
                if (tributes.Count == 5)
                    break;
            }

            // We can tribute one or two phalanx if needed, but only
            // if we have more than one in the graveyard.
            if (tributes.Count < 5 && phalanxCount > 1)
            {
                foreach (ClientCard card in Bot.Graveyard)
                {
                    if (card.IsCode(CardId.DragunityPhalanx))
                    {
                        phalanxCount--;
                        tributes.Add(card);
                        if (phalanxCount <= 1)
                            break;
                    }
                }
            }

            if (tributes.Count < 5)
                return false;

            AI.SelectCard(CardId.FiveHeadedDragon);
            AI.SelectNextCard(tributes);
            return true;
        }

        private bool ScrapDragonSummon()
        {
            //if (Util.IsOneEnemyBetterThanValue(2500, true))
            //    return true;
            ClientCard invincible = Util.GetProblematicEnemyCard(3000);
            return invincible != null;
        }

        private bool ScrapDragonEffect()
        {
            ClientCard invincible = Util.GetProblematicEnemyCard(3000);
            if (invincible == null && !Util.IsOneEnemyBetterThanValue(2800 - 1, false))
                return false;

            int tributeId = -1;
            if (Bot.HasInSpellZone(CardId.FireFormationTenki))
                tributeId = CardId.FireFormationTenki;
            else if (Bot.HasInSpellZone(CardId.Terraforming))
                tributeId = CardId.Terraforming;
            else if (Bot.HasInSpellZone(CardId.DragonsMirror))
                tributeId = CardId.DragonsMirror;
            else if (Bot.HasInSpellZone(CardId.CardsOfConsonance))
                tributeId = CardId.CardsOfConsonance;
            else if (Bot.HasInSpellZone(CardId.AssaultTeleport))
                tributeId = CardId.AssaultTeleport;
            else if (Bot.HasInSpellZone(CardId.AssaultModeActivate))
                tributeId = CardId.AssaultModeActivate;
            else if (Bot.HasInSpellZone(CardId.DragonRavine))
                tributeId = CardId.DragonRavine;

            List<ClientCard> monsters = Enemy.GetMonsters();
            monsters.Sort(CardContainer.CompareCardAttack);

            ClientCard destroyCard = invincible;
            if (destroyCard == null)
            {
                for (int i = monsters.Count - 1; i >= 0; --i)
                {
                    if (monsters[i].IsAttack())
                    {
                        destroyCard = monsters[i];
                        break;
                    }
                }
            }

            if (destroyCard == null)
                return false;

            AI.SelectCard(tributeId);
            AI.SelectNextCard(destroyCard);

            return true;
        }

        private bool CrystalWingSynchroDragonSummon()
        {
            return !Bot.HasInHand(CardId.AssaultModeActivate)
                && !Bot.HasInHand(CardId.AssaultBeast)
                && !Bot.HasInSpellZone(CardId.AssaultModeActivate);
        }

        private bool CrystalWingSynchroDragonEffect()
        {
            return Duel.LastChainPlayer != 0;
        }

        private bool DragunityPhalanxSummon()
        {
            return Bot.HasInHand(CardId.DragunityArmaMysletainn);
        }

        private bool DragunityArmaMysletainn()
        {
            if (Bot.HasInMonstersZone(CardId.DragunityPhalanx))
            {
                AI.SelectCard(CardId.DragunityPhalanx);
                return true;
            }
            if (Bot.HasInMonstersZone(CardId.DragunityDux))
            {
                AI.SelectCard(CardId.DragunityDux);
                return true;
            }
            return false;
        }

        private bool DragunityArmaMysletainnEffect()
        {
            AI.SelectCard(CardId.DragunityPhalanx);
            return true;
        }

        private bool DragunityArmaMysletainnTribute()
        {
            if ((Bot.HasInMonstersZone(CardId.AssaultBeast)
                && Bot.HasInGraveyard(CardId.DragunityPhalanx))
                || Bot.HasInMonstersZone(CardId.DragunityPhalanx)
                || Bot.HasInHand(CardId.DragunitySpearOfDestiny))
            {
                List<ClientCard> monster_sorted = Bot.GetMonsters();
                monster_sorted.Sort(CardContainer.CompareCardAttack);
                foreach (ClientCard monster in monster_sorted)
                {
                    AI.SelectMaterials(monster);
                    return true;
                }
            }
            return false;
        }

        private bool DragunityDux()
        {
            return Bot.HasInGraveyard(CardId.DragunityPhalanx) ||
                (Bot.GetMonsterCount() == 0 && Bot.HasInHand(CardId.DragunityArmaMysletainn) ||
                Bot.HasInHand(CardId.DragunitySpearOfDestiny));
        }

        private bool DragunityPhalanxSet()
        {
            return Bot.GetMonsterCount() == 0 || !Bot.HasInGraveyard(CardId.DragunityPhalanx);
        }

        private bool AssaultBeast()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            if (!Bot.HasInSpellZone(CardId.AssaultModeActivate))
                return true;
            return false;
        }

        private bool AssaultModeActivate()
        {
            if (Duel.Player == 0 && Duel.Phase == DuelPhase.BattleStart)
            {
                List<ClientCard> monsters = Bot.GetMonsters();
                foreach (ClientCard monster in monsters)
                {
                    if (monster.IsCode(CardId.StardustDragon) && monster.Attacked)
                    {
                        AI.SelectCard(monster);
                        return true;
                    }
                }
            }
            return Duel.Player == 1;
        }
    }
}