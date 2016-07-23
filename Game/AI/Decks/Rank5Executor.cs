using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot.Game;
using WindBot.Game.AI;

namespace DevBot.Game.AI.Decks
{
    [Deck("Rank V", "AI_Rank5")]
    public class Rank5Executor : DefaultExecutor
    {
        public enum CardId
        {
            MistArchfiend = 28601770,
            PowerInvader = 18842395,
            CyberDragon = 70095155,
            ViceDragon = 54343893,
            TheTricky = 14778250,
            WindUpSoldier = 12299841,
            StarDrawing = 24610207,
            GagagaMagician = 26082117,
            InstantFusion = 1845204,
            DoubleSummon = 43422537,
            MysticalSpaceTyphoon = 5318639,
            BookOfMoon = 14087893,
            XyzUnit = 13032689,
            XyzReborn = 26708437,
            MirrorForce = 44095762,
            TorrentialTribute = 53582587,
            SakuretsuArmor = 56120475,
            XyzEffect = 58628539,
            XyzVeil = 96457619,

            FlameSwordsman = 45231177,
            DigvorzhakKingOfHeavyIndustry = 29515122,
            TirasKeeperOfGenesis = 31386180,
            AdreusKeeperOfArmageddon = 94119480,
            Number61Volcasaurus = 29669359,
            Number19Freezerdon = 55067058
        }

        public Rank5Executor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // Quick spells
            AddExecutor(ExecutorType.Activate, (int)CardId.BookOfMoon, DefaultBookOfMoon);
            AddExecutor(ExecutorType.Activate, (int)CardId.MysticalSpaceTyphoon, DefaultMysticalSpaceTyphoon);

            // XYZ Monsters: Effect
            AddExecutor(ExecutorType.Activate, (int)CardId.DigvorzhakKingOfHeavyIndustry);
            AddExecutor(ExecutorType.Activate, (int)CardId.TirasKeeperOfGenesis);
            AddExecutor(ExecutorType.Activate, (int)CardId.AdreusKeeperOfArmageddon);
            AddExecutor(ExecutorType.Activate, (int)CardId.Number61Volcasaurus);
            AddExecutor(ExecutorType.Activate, (int)CardId.Number19Freezerdon);

            // Summon LV.5 Monsters
            AddExecutor(ExecutorType.SpSummon, (int)CardId.CyberDragon);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.ViceDragon);
            AddExecutor(ExecutorType.Summon, (int)CardId.PowerInvader, PowerInvader);
            AddExecutor(ExecutorType.Summon, (int)CardId.WindUpSoldier);
            AddExecutor(ExecutorType.Summon, (int)CardId.StarDrawing);
            AddExecutor(ExecutorType.Summon, (int)CardId.GagagaMagician);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.TheTricky, TheTricky);

            AddExecutor(ExecutorType.Summon, (int)CardId.MistArchfiend, MistArchfiend);
            AddExecutor(ExecutorType.Activate, (int)CardId.InstantFusion, IsAnotherRank5Available);

            AddExecutor(ExecutorType.Activate, (int)CardId.GagagaMagician, GagagaMagicianEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.WindUpSoldier, WindUpSoldierEffect);

            // Useful spells
            AddExecutor(ExecutorType.Activate, (int)CardId.DoubleSummon, DoubleSummon);
            AddExecutor(ExecutorType.Activate, (int)CardId.XyzUnit, XyzUnit);

            // XYZ Monsters: Summon
            AddExecutor(ExecutorType.SpSummon, (int)CardId.DigvorzhakKingOfHeavyIndustry, SummonXYZ);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.TirasKeeperOfGenesis, SummonXYZ);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.AdreusKeeperOfArmageddon, SummonXYZ);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.Number61Volcasaurus, SummonXYZ);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.Number19Freezerdon, SummonXYZ);

            // Xyz Reborn
            AddExecutor(ExecutorType.Activate, (int)CardId.XyzReborn);

            // Reposition
            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);

            // Set and activate traps
            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);

            AddExecutor(ExecutorType.Activate, (int)CardId.XyzVeil, XyzVeil);
            AddExecutor(ExecutorType.Activate, (int)CardId.XyzEffect, XyzEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.TorrentialTribute, DefaultTorrentialTribute);
            AddExecutor(ExecutorType.Activate, (int)CardId.MirrorForce, DefaultTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.SakuretsuArmor, DefaultTrap);
        }

        public override bool OnSelectHand()
        {
            return false;
        }

        private bool SummonXYZ()
        {
            List<ClientCard> monsters = Duel.Fields[0].GetMonsters();
            foreach (ClientCard monster in monsters)
                if (monster.Id == Card.Id)
                    return false;
            return true;
        }

        private bool PowerInvader()
        {
            if (Duel.Fields[1].GetMonsterCount() >= 2)
            {
                AI.SelectOption(1);
                return true;
            }
            return false;
        }

        private bool TheTricky()
        {
            IList<ClientCard> hand = Duel.Fields[0].Hand;
            foreach (ClientCard card in hand)
            {
                if (card.Id == (int)CardId.TheTricky && card != Card)
                {
                    AI.SelectCard(card);
                    return true;
                }
            }
            if (Duel.Fields[0].HasInHand((int)CardId.PowerInvader))
            {
                AI.SelectCard((int)CardId.PowerInvader);
                return true;
            }
            if (Duel.Fields[0].HasInHand((int)CardId.CyberDragon))
            {
                AI.SelectCard((int)CardId.CyberDragon);
                return true;
            }
            if (Duel.Fields[0].HasInHand((int)CardId.ViceDragon))
            {
                AI.SelectCard((int)CardId.ViceDragon);
                return true;
            }
            if (Duel.Fields[0].HasInHand((int)CardId.XyzVeil))
            {
                AI.SelectCard((int)CardId.XyzVeil);
                return true;
            }
            if (Duel.Fields[0].HasInHand((int)CardId.XyzEffect))
            {
                AI.SelectCard((int)CardId.XyzEffect);
                return true;
            }
            if (Duel.Fields[0].HasInHand((int)CardId.MysticalSpaceTyphoon))
            {
                AI.SelectCard((int)CardId.MysticalSpaceTyphoon);
                return true;
            }
            return false;
        }

        private bool MistArchfiend()
        {
            if (!IsAnotherRank5Available())
                return false;
            AI.SelectOption(1);
            return true;
        }

        private bool IsAnotherRank5Available()
        {
            List<ClientCard> monsters = Duel.Fields[0].GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                if (monster.HasType(CardType.Monster) &&
                    !monster.HasType(CardType.Xyz) &&
                    (monster.Level == 5 ||
                        monster.Id == (int)CardId.WindUpSoldier) ||
                        monster.Id == (int)CardId.StarDrawing ||
                        monster.Id == (int)CardId.GagagaMagician)
                    return true;
            }
            int mistCount = 0;
            IList<ClientCard> hand = Duel.Fields[0].Hand;
            foreach (ClientCard card in hand)
            {
                if (card.Id == (int)CardId.InstantFusion && Card.Id != card.Id)
                    return true;
                if (card.Id == (int)CardId.MistArchfiend && Card.Id != card.Id)
                    return true;
                if (card.Id == (int)CardId.MistArchfiend)
                    ++mistCount;
            }
            if (mistCount >= 2)
                return true;
            return false;
        }

        private bool WindUpSoldierEffect()
        {
            List<ClientCard> monsters = Duel.Fields[0].GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                if (monster.HasType(CardType.Monster) &&
                    !monster.HasType(CardType.Xyz) &&
                    (monster.Level == 5 ||
                        monster.Id == (int)CardId.StarDrawing) ||
                        monster.Id == (int)CardId.GagagaMagician)
                    return true;
            }
            return false;
        }

        private bool GagagaMagicianEffect()
        {
            AI.SelectNumber(5);
            return true;
        }

        private bool DoubleSummon()
        {
            IList<ClientCard> hand = Duel.Fields[0].Hand;
            foreach (ClientCard card in hand)
            {
                if (card.Id == (int)CardId.PowerInvader && Duel.Fields[1].GetMonsterCount() >= 2)
                    return true;
                if (card.Id == (int)CardId.WindUpSoldier ||
                    card.Id == (int)CardId.StarDrawing ||
                    card.Id == (int)CardId.GagagaMagician)
                    return true;
            }
            return false;
        }

        private bool XyzUnit()
        {
            List<ClientCard> monsters = Duel.Fields[0].GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                if (monster.HasType(CardType.Xyz))
                {
                    AI.SelectCard(monster);
                    return true;
                }
            }
            return false;
        }

        private bool XyzVeil()
        {
            List<ClientCard> spells = Duel.Fields[0].GetSpells();
            foreach (ClientCard spell in spells)
            {
                if (spell.Id == (int)CardId.XyzVeil && spell != Card)
                    return false;
            }
            List<ClientCard> monsters = Duel.Fields[0].GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                if (monster.HasType(CardType.Xyz))
                    return true;
            }
            return true;
        }

        private bool XyzEffect()
        {
            ClientCard card = GetBestEnnemyCard();
            if (card != null)
            {
                AI.SelectCard(card);
                return true;
            }
            return false;
        }

        private ClientCard GetProblematicCard()
        {
            ClientCard card = Duel.Fields[1].MonsterZone.GetInvincibleMonster();
            if (card != null)
                return card;
            card = Duel.Fields[1].SpellZone.GetNegateAttackSpell();
            if (card != null)
                return card;
            return null;
        }

        private ClientCard GetBestEnnemyCard()
        {
            ClientCard card = GetProblematicCard();
            if (card != null)
                return card;
            card = Duel.Fields[1].MonsterZone.GetHighestAttackMonster();
            if (card != null)
                return card;
            List<ClientCard> spells = Duel.Fields[1].GetSpells();
            if (spells.Count > 0)
                return spells[0];
            return null;
        }
    }
}
