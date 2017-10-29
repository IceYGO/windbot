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
        public enum CardId
        {
            StardustDragonAssaultMode = 61257789,
            DragunityArmaMysletainn = 876330,
            AssaultBeast = 3431737,
            DragunityDux = 28183605,
            DragunityPhalanx = 59755122,
            AssaultTeleport = 29863101,
            CardsOfConsonance = 39701395,
            UpstartGoblin = 70368879,
            DragonsMirror = 71490127,
            Terraforming = 73628505,
            FoolishBurial = 81439173,
            MonsterReborn = 83764718,
            MysticalSpaceTyphoon = 5318639,
            FireFormationTenki = 57103969,
            DragunitySpearOfDestiny = 60004971,
            DragonRavine = 62265044,
            MirrorForce = 44095762,
            StarlightRoad = 58120309,
            DimensionalPrison = 70342110,
            AssaultModeActivate = 80280737,
            FiveHeadedDragon = 99267150,
            CrystalWingSynchroDragon = 50954680,
            ScrapDragon = 76774528,
            StardustDragon = 44508094,
            DragunityKnightGaeDearg = 34116027,
            DragunityKnightVajrayana = 21249921
        }

        public DragunityExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // Set traps
            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);

            // Execute spells
            AddExecutor(ExecutorType.Activate, (int)CardId.MysticalSpaceTyphoon, DefaultMysticalSpaceTyphoon);
            AddExecutor(ExecutorType.Activate, (int)CardId.AssaultTeleport);
            AddExecutor(ExecutorType.Activate, (int)CardId.UpstartGoblin);
            AddExecutor(ExecutorType.Activate, (int)CardId.DragonRavine, DragonRavineField);
            AddExecutor(ExecutorType.Activate, (int)CardId.Terraforming, Terraforming);
            AddExecutor(ExecutorType.Activate, (int)CardId.FoolishBurial, FoolishBurial);
            AddExecutor(ExecutorType.Activate, (int)CardId.MonsterReborn, MonsterReborn);

            // Execute monsters
            AddExecutor(ExecutorType.Activate, (int)CardId.ScrapDragon, ScrapDragonEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.CrystalWingSynchroDragon, CrystalWingSynchroDragonEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.DragunityPhalanx);
            AddExecutor(ExecutorType.Activate, (int)CardId.DragunityKnightVajrayana);
            AddExecutor(ExecutorType.Activate, (int)CardId.DragunityArmaMysletainn, DragunityArmaMysletainnEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.DragunityDux);

            // Summon
            AddExecutor(ExecutorType.Activate, (int)CardId.DragonsMirror, DragonsMirror);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.ScrapDragon, ScrapDragonSummon);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.CrystalWingSynchroDragon, CrystalWingSynchroDragonSummon);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.StardustDragon);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.DragunityKnightVajrayana);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.DragunityKnightGaeDearg);
            AddExecutor(ExecutorType.Summon, (int)CardId.DragunityPhalanx, DragunityPhalanxSummon);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.DragunityArmaMysletainn, DragunityArmaMysletainn);
            AddExecutor(ExecutorType.Summon, (int)CardId.DragunityArmaMysletainn, DragunityArmaMysletainnTribute);

            // Use draw effects if we can't do anything else
            AddExecutor(ExecutorType.Activate, (int)CardId.CardsOfConsonance);
            AddExecutor(ExecutorType.Activate, (int)CardId.DragonRavine, DragonRavineEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.FireFormationTenki, FireFormationTenki);
            AddExecutor(ExecutorType.Activate, (int)CardId.DragunitySpearOfDestiny);

            // Summon
            AddExecutor(ExecutorType.Summon, (int)CardId.DragunityDux, DragunityDux);
            AddExecutor(ExecutorType.MonsterSet, (int)CardId.DragunityPhalanx, DragunityPhalanxSet);
            AddExecutor(ExecutorType.SummonOrSet, (int)CardId.AssaultBeast);

            // Draw assault mode if we don't have one
            AddExecutor(ExecutorType.Activate, (int)CardId.AssaultBeast, AssaultBeast);

            // Set useless cards
            AddExecutor(ExecutorType.SpellSet, (int)CardId.DragonsMirror, SetUselessCards);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.Terraforming, SetUselessCards);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.AssaultTeleport, SetUselessCards);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.CardsOfConsonance, SetUselessCards);

            // Chain traps and monsters
            AddExecutor(ExecutorType.Activate, (int)CardId.StardustDragonAssaultMode, StardustDragon);
            AddExecutor(ExecutorType.Activate, (int)CardId.StardustDragon, StardustDragon);
            AddExecutor(ExecutorType.Activate, (int)CardId.StarlightRoad, DefaultTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.MirrorForce, DefaultTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.DimensionalPrison, DefaultTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.AssaultModeActivate, AssaultModeActivate);

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
            if (Bot.HasInHand((int)CardId.DragunityPhalanx))
                tributeId = (int)CardId.DragunityPhalanx;
            else if (Bot.HasInHand((int)CardId.FireFormationTenki))
                tributeId = (int)CardId.FireFormationTenki;
            else if (Bot.HasInHand((int)CardId.Terraforming))
                tributeId = (int)CardId.Terraforming;
            else if (Bot.HasInHand((int)CardId.DragonRavine))
                tributeId = (int)CardId.DragonRavine;
            else if (Bot.HasInHand((int)CardId.AssaultTeleport))
                tributeId = (int)CardId.AssaultTeleport;
            else if (Bot.HasInHand((int)CardId.AssaultBeast))
                tributeId = (int)CardId.AssaultBeast;
            else if (Bot.HasInHand((int) CardId.DragunityArmaMysletainn))
                tributeId = (int)CardId.DragunityArmaMysletainn;
            else
            {
                int count = 0;
                foreach (ClientCard card in Bot.Hand)
                {
                    if (card.Id == (int)CardId.DragunityDux)
                        ++count;
                }
                if (count >= 2)
                    tributeId = (int)CardId.DragunityDux;
            }
            if (tributeId == -1 && Bot.HasInHand((int)CardId.StardustDragonAssaultMode))
                tributeId = (int)CardId.StardustDragonAssaultMode;
            if (tributeId == -1 && Bot.HasInHand((int)CardId.DragunitySpearOfDestiny))
                tributeId = (int)CardId.StardustDragonAssaultMode;
            if (tributeId == -1 && Bot.HasInHand((int)CardId.DragonsMirror)
                && Bot.GetMonsterCount() == 0)
                tributeId = (int)CardId.StardustDragonAssaultMode;

            if (tributeId == -1)
                return false;

            int needId = -1;
            if (!Bot.HasInMonstersZone((int)CardId.DragunityPhalanx) &&
                !Bot.HasInGraveyard((int)CardId.DragunityPhalanx))
                needId = (int)CardId.DragunityPhalanx;
            else if (Bot.GetMonsterCount() == 0)
                needId = (int)CardId.DragunityDux;
            else
            {
                /*bool hasRealMonster = false;
                foreach (ClientCard card in Bot.GetMonsters())
                {
                    if (card.Id != (int)CardId.AssaultBeast)
                    {
                        hasRealMonster = true;
                        break;
                    }
                }
                if (!hasRealMonster || AI.Utils.GetProblematicCard() != null)*/
                needId = (int)CardId.DragunityDux;
            }

            if (needId == -1)
                return false;

            int option;

            if (tributeId == (int)CardId.DragunityPhalanx)
                needId = (int)CardId.DragunityDux;

            int remaining = 3;
            foreach (ClientCard card in Bot.Hand)
                if (card.Id == needId)
                    remaining--;
            foreach (ClientCard card in Bot.Graveyard)
                if (card.Id == needId)
                    remaining--;
            foreach (ClientCard card in Bot.Banished)
                if (card.Id == needId)
                    remaining--;
            if (remaining <= 0)
                return false;

            if (needId == (int)CardId.DragunityPhalanx)
                option = 2;
            else
                option = 1;

            if (ActivateDescription != (int)CardId.DragonRavine*16 + option)
                return false;

            AI.SelectCard(tributeId);
            AI.SelectNextCard(needId);

            return true;
        }

        private bool Terraforming()
        {
            if (Bot.HasInHand((int)CardId.DragonRavine))
                return false;
            if (Bot.SpellZone[5] != null)
                return false;
            return true;
        }

        private bool SetUselessCards()
        {
            ClientField field = Bot;

            if (field.HasInSpellZone((int)CardId.FireFormationTenki))
                return false;
            if (field.HasInSpellZone((int)CardId.AssaultTeleport))
                return false;
            if (field.HasInSpellZone((int)CardId.CardsOfConsonance))
                return false;
            if (field.HasInSpellZone((int)CardId.DragonsMirror))
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
            if (Bot.HasInGraveyard((int)CardId.DragunityPhalanx))
                return false;
            if (Bot.HasInHand((int)CardId.DragunityPhalanx))
                return false;
            int remaining = 3;
            foreach (ClientCard card in Bot.Banished)
                if (card.Id == (int)CardId.DragunityPhalanx)
                    remaining--;
            if (remaining > 0)
            {
                AI.SelectCard((int)CardId.DragunityPhalanx);
                return true;
            }
            return false;
        }

        private bool MonsterReborn()
        {
            List<ClientCard> cards = new List<ClientCard>(Bot.Graveyard);
            cards.Sort(AIFunctions.CompareCardAttack);
            ClientCard selectedCard = null;
            for (int i = cards.Count - 1; i >= 0; --i)
            {
                ClientCard card = cards[i];
                if (card.Attack < 2000)
                    break;
                if (card.Id == (int) CardId.StardustDragonAssaultMode ||
                    card.Id == (int) CardId.FiveHeadedDragon)
                    continue;
                if (card.IsMonster())
                {
                    selectedCard = card;
                    break;
                }
            }
            cards = new List<ClientCard>(Enemy.Graveyard);
            cards.Sort(AIFunctions.CompareCardAttack);
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
                if (card.Id == (int) CardId.DragunityPhalanx)
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
                    if (card.Id == (int) CardId.DragunityPhalanx)
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

            AI.SelectCard((int)CardId.FiveHeadedDragon);
            AI.SelectNextCard(tributes);
            return true;
        }

        private bool ScrapDragonSummon()
        {
            //if (AI.Utils.IsOneEnemyBetterThanValue(2500, true))
            //    return true;
            ClientCard invincible = AI.Utils.GetProblematicCard();
            return invincible != null;
        }

        private bool ScrapDragonEffect()
        {
            ClientCard invincible = AI.Utils.GetProblematicCard();
            if (invincible == null && !AI.Utils.IsOneEnemyBetterThanValue(2800 - 1, false))
                return false;

            ClientField field = Bot;

            int tributeId = -1;
            if (field.HasInSpellZone((int)CardId.FireFormationTenki))
                tributeId = (int)CardId.FireFormationTenki;
            else if (field.HasInSpellZone((int)CardId.Terraforming))
                tributeId = (int)CardId.Terraforming;
            else if (field.HasInSpellZone((int)CardId.DragonsMirror))
                tributeId = (int)CardId.DragonsMirror;
            else if (field.HasInSpellZone((int)CardId.CardsOfConsonance))
                tributeId = (int)CardId.CardsOfConsonance;
            else if (field.HasInSpellZone((int)CardId.AssaultTeleport))
                tributeId = (int)CardId.AssaultTeleport;
            else if (field.HasInSpellZone((int)CardId.AssaultModeActivate))
                tributeId = (int)CardId.AssaultModeActivate;
            else if (field.HasInSpellZone((int)CardId.DragonRavine))
                tributeId = (int)CardId.DragonRavine;

            List<ClientCard> monsters = Enemy.GetMonsters();
            monsters.Sort(AIFunctions.CompareCardAttack);

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

            if (tributeId != -1)
                AI.SelectCard(tributeId);

            AI.SelectNextCard(destroyCard);

            return true;
        }

        private bool CrystalWingSynchroDragonSummon()
        {
            return !Bot.HasInHand((int)CardId.AssaultModeActivate)
                && !Bot.HasInHand((int)CardId.AssaultBeast)
                && !Bot.HasInSpellZone((int)CardId.AssaultModeActivate);
        }

        private bool CrystalWingSynchroDragonEffect()
        {
            return LastChainPlayer != 0;
        }

        private bool DragunityPhalanxSummon()
        {
            return Bot.HasInHand((int)CardId.DragunityArmaMysletainn);
        }

        private bool DragunityArmaMysletainn()
        {
            if (Bot.HasInMonstersZone((int)CardId.DragunityPhalanx))
            {
                AI.SelectCard((int)CardId.DragunityPhalanx);
                return true;
            }
            if (Bot.HasInMonstersZone((int)CardId.DragunityDux))
            {
                AI.SelectCard((int)CardId.DragunityDux);
                return true;
            }
            return false;
        }

        private bool DragunityArmaMysletainnEffect()
        {
            AI.SelectCard((int)CardId.DragunityPhalanx);
            return true;
        }

        private bool DragunityArmaMysletainnTribute()
        {
            if ((Bot.HasInMonstersZone((int)CardId.AssaultBeast)
                && Bot.HasInGraveyard((int)CardId.DragunityPhalanx))
                || Bot.HasInMonstersZone((int)CardId.DragunityPhalanx)
                || Bot.HasInHand((int)CardId.DragunitySpearOfDestiny))
                return true;
            return false;
        }

        private bool DragunityDux()
        {
            return Bot.HasInGraveyard((int)CardId.DragunityPhalanx) ||
                (Bot.GetMonsterCount() == 0 && Bot.HasInHand((int)CardId.DragunityArmaMysletainn) ||
                Bot.HasInHand((int)CardId.DragunitySpearOfDestiny));
        }

        private bool DragunityPhalanxSet()
        {
            return Bot.GetMonsterCount() == 0 || !Bot.HasInGraveyard((int)CardId.DragunityPhalanx);
        }

        private bool AssaultBeast()
        {
            if (!Bot.HasInSpellZone((int)CardId.AssaultModeActivate))
                return true;
            return false;
        }

        private bool StardustDragon()
        {
            if (Card.Location == CardLocation.Grave)
                return true;
            return LastChainPlayer == 1;
        }

        private bool AssaultModeActivate()
        {
            if (Duel.Player == 0 && Duel.Phase == DuelPhase.BattleStart)
            {
                List<ClientCard> monsters = Bot.GetMonsters();
                foreach (ClientCard monster in monsters)
                {
                    if (monster.Id == (int)CardId.StardustDragon && monster.Attacked)
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