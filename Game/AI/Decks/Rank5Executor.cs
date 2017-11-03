using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using YGOSharp.OCGWrapper.Enums;

namespace WindBot.Game.AI.Decks
{
    [Deck("Rank V", "AI_Rank5")]
    public class Rank5Executor : DefaultExecutor
    {
        public enum CardId
        {
            MistArchfiend = 28601770,
            CyberDragon = 70095154,
            ZWEagleClaw = 29353756,
            SolarWindJammer = 33911264,
            QuickdrawSynchron = 20932152,
            WindUpSoldier = 12299841,
            StarDrawing = 24610207,
            ChronomalyGoldenJet = 88552992,

            InstantFusion = 1845204,
            DoubleSummon = 43422537,
            MysticalSpaceTyphoon = 5318639,
            BookOfMoon = 14087893,
            XyzUnit = 13032689,
            XyzReborn = 26708437,
            MirrorForce = 44095762,
            TorrentialTribute = 53582587,
            XyzVeil = 96457619,

            PanzerDragon = 72959823,
            GaiaDragonTheThunderCharger = 91949988,
            CyberDragonInfinity = 10443957,
            TirasKeeperOfGenesis = 31386180,
            Number61Volcasaurus = 29669359,
            SharkFortress = 50449881,
            CyberDragonNova = 58069384
        }

        private bool NormalSummoned = false;
        private bool InstantFusionUsed = false;
        private bool DoubleSummonUsed = false;
        private bool CyberDragonInfinitySummoned = false;
        private bool Number61VolcasaurusUsed = false;

        public Rank5Executor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // Quick spells
            AddExecutor(ExecutorType.Activate, (int)CardId.BookOfMoon, DefaultBookOfMoon);
            AddExecutor(ExecutorType.Activate, (int)CardId.MysticalSpaceTyphoon, DefaultMysticalSpaceTyphoon);

            // Cyber Dragon Infinity first
            AddExecutor(ExecutorType.SpSummon, (int)CardId.CyberDragonNova, CyberDragonNovaSummon);
            AddExecutor(ExecutorType.Activate, (int)CardId.CyberDragonNova, CyberDragonNovaEffect);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.CyberDragonInfinity, CyberDragonInfinitySummon);
            AddExecutor(ExecutorType.Activate, (int)CardId.CyberDragonInfinity, CyberDragonInfinityEffect);

            // Level 5 monsters without side effects
            AddExecutor(ExecutorType.SpSummon, (int)CardId.CyberDragon);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.SolarWindJammer, SolarWindJammerSummon);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.ZWEagleClaw);
            AddExecutor(ExecutorType.Summon, (int)CardId.ChronomalyGoldenJet, NormalSummon);
            AddExecutor(ExecutorType.Activate, (int)CardId.ChronomalyGoldenJet, ChronomalyGoldenJetEffect);
            AddExecutor(ExecutorType.Summon, (int)CardId.StarDrawing, NormalSummon);
            AddExecutor(ExecutorType.Summon, (int)CardId.WindUpSoldier, NormalSummon);
            AddExecutor(ExecutorType.Activate, (int)CardId.WindUpSoldier, WindUpSoldierEffect);

            // XYZ Monsters: Summon
            AddExecutor(ExecutorType.SpSummon, (int)CardId.Number61Volcasaurus, Number61VolcasaurusSummon);
            AddExecutor(ExecutorType.Activate, (int)CardId.Number61Volcasaurus, Number61VolcasaurusEffect);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.TirasKeeperOfGenesis);
            AddExecutor(ExecutorType.Activate, (int)CardId.TirasKeeperOfGenesis, TirasKeeperOfGenesisEffect);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.SharkFortress);
            AddExecutor(ExecutorType.Activate, (int)CardId.SharkFortress);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.GaiaDragonTheThunderCharger, GaiaDragonTheThunderChargerSummon);


            // Level 5 monsters with side effects
            AddExecutor(ExecutorType.SpSummon, (int)CardId.QuickdrawSynchron, QuickdrawSynchronSummon);
            AddExecutor(ExecutorType.Summon, (int)CardId.MistArchfiend, MistArchfiendSummon);
            AddExecutor(ExecutorType.Activate, (int)CardId.InstantFusion, InstantFusionEffect);

            // Useful spells
            AddExecutor(ExecutorType.Activate, (int)CardId.DoubleSummon, DoubleSummonEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.XyzUnit, XyzUnitEffect);


            AddExecutor(ExecutorType.Activate, (int)CardId.XyzReborn, XyzRebornEffect);

            AddExecutor(ExecutorType.Activate, (int)CardId.PanzerDragon, PanzerDragonEffect);

            // Reposition
            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);

            // Set and activate traps
            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);

            AddExecutor(ExecutorType.Activate, (int)CardId.XyzVeil, XyzVeilEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.TorrentialTribute, DefaultTorrentialTribute);
            AddExecutor(ExecutorType.Activate, (int)CardId.MirrorForce, DefaultTrap);
        }

        public override bool OnSelectHand()
        {
            return false;
        }

        public override void OnNewTurn()
        {
            NormalSummoned = false;
            InstantFusionUsed = false;
            DoubleSummonUsed = false;
            CyberDragonInfinitySummoned = false;
            Number61VolcasaurusUsed = false;
        }

        private bool NormalSummon()
        {
            NormalSummoned = true;
            return true;
        }

        private bool SolarWindJammerSummon()
        {
            AI.SelectPosition(CardPosition.FaceUpDefence);
            return true;
        }

        private bool QuickdrawSynchronSummon()
        {
            if (!needLV5())
                return false;
            AI.SelectCard(new[]
                {
                    (int)CardId.QuickdrawSynchron,
                    (int)CardId.ZWEagleClaw,
                    (int)CardId.SolarWindJammer,
                    (int)CardId.CyberDragon,
                    (int)CardId.MistArchfiend,
                    (int)CardId.WindUpSoldier,
                    (int)CardId.StarDrawing,
                    (int)CardId.ChronomalyGoldenJet
                });
            return true;
        }

        private bool MistArchfiendSummon()
        {
            if (!needLV5())
                return false;
            AI.SelectOption(1);
            NormalSummoned = true;
            return true;
        }

        private bool InstantFusionEffect()
        {
            if (!needLV5())
                return false;
            InstantFusionUsed = true;
            return true;
        }

        private bool needLV5()
        {
            if (HaveLV5OnField())
                return true;
            int lv5Count = 0;
            IList<ClientCard> hand = Bot.Hand;
            foreach (ClientCard card in hand)
            {
                if (card.Id == (int)CardId.InstantFusion && !InstantFusionUsed)
                    ++lv5Count;
                if (card.Id == (int)CardId.QuickdrawSynchron && Bot.Hand.ContainsMonsterWithLevel(4))
                    ++lv5Count;
                if (card.Id == (int)CardId.MistArchfiend && !NormalSummoned)
                    ++lv5Count;
                if (card.Id == (int)CardId.DoubleSummon && DoubleSummonEffect())
                    ++lv5Count;
            }
            if (lv5Count >= 2)
                return true;
            return false;
        }

        private bool WindUpSoldierEffect()
        {
            return HaveLV5OnField();
        }

        private bool ChronomalyGoldenJetEffect()
        {
            return Card.Level == 4;
        }

        private bool DoubleSummonEffect()
        {
            if (!NormalSummoned || DoubleSummonUsed)
                return false;
            IList<ClientCard> hand = Bot.Hand;
            foreach (ClientCard card in hand)
            {
                if (card.Id == (int)CardId.MistArchfiend ||
                    card.Id == (int)CardId.WindUpSoldier ||
                    card.Id == (int)CardId.StarDrawing ||
                    card.Id == (int)CardId.ChronomalyGoldenJet)
                {
                    NormalSummoned = false;
                    DoubleSummonUsed = true;
                    return true;
                }
            }
            return false;
        }

        private bool CyberDragonNovaSummon()
        {
            return !CyberDragonInfinitySummoned;
        }

        private bool CyberDragonNovaEffect()
        {
            if (ActivateDescription == AI.Utils.GetStringId((int)CardId.CyberDragonNova, 0))
            {
                return true;
            }
            else if (Card.Location == CardLocation.Grave)
            {
                AI.SelectPosition(CardPosition.FaceUpDefence);
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CyberDragonInfinitySummon()
        {
            CyberDragonInfinitySummoned = true;
            return true;
        }

        private bool CyberDragonInfinityEffect()
        {
            if (CurrentChain.Count > 0)
            {
                return LastChainPlayer == 1;
            }
            else
            {
                List<ClientCard> monsters = Enemy.GetMonsters();
                ClientCard bestmonster = null;
                foreach (ClientCard monster in monsters)
                {
                    if (monster.IsAttack() && (bestmonster == null || monster.Attack >= bestmonster.Attack))
                        bestmonster = monster;
                }
                if (bestmonster != null)
                {
                    AI.SelectCard(bestmonster);
                    return true;
                }
            }
            return false;
        }

        private bool Number61VolcasaurusSummon()
        {
            return AI.Utils.IsOneEnemyBetterThanValue(2000, false);
        }

        private bool Number61VolcasaurusEffect()
        {
            ClientCard target = AI.Utils.GetProblematicEnemyMonster(2000);
            if (target != null)
            {
                AI.SelectCard((int)CardId.CyberDragon);
                AI.SelectNextCard(target);
                Number61VolcasaurusUsed = true;
                return true;
            }
            return false;
        }

        private bool TirasKeeperOfGenesisEffect()
        {
            ClientCard target = AI.Utils.GetProblematicEnemyCard();
            if (target != null)
            {
                AI.SelectCard(target);
            }
            return true;
        }

        private bool GaiaDragonTheThunderChargerSummon()
        {
            if (Number61VolcasaurusUsed && Bot.HasInMonstersZone((int)CardId.Number61Volcasaurus))
            {
                AI.SelectCard((int)CardId.Number61Volcasaurus);
                return true;
            }
            List<ClientCard> monsters = Bot.GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                if (monster.HasType(CardType.Xyz) && !monster.HasXyzMaterial())
                {
                    AI.SelectCard(monster);
                    return true;
                }
            }
            return false;
        }

        private bool XyzRebornEffect()
        {
            foreach (ClientCard card in Bot.SpellZone)
            {
                if (card != null &&
                    card.Id == Card.Id &&
                    card.HasPosition(CardPosition.FaceUp))
                    return false;
            }
            AI.SelectCard(new[]
                {
                    (int)CardId.CyberDragonInfinity,
                    (int)CardId.CyberDragonNova,
                    (int)CardId.TirasKeeperOfGenesis,
                    (int)CardId.SharkFortress,
                    (int)CardId.Number61Volcasaurus
                });
            return true;
        }

        private bool XyzUnitEffect()
        {
            List<ClientCard> monsters = Bot.GetMonsters();
            return monsters.Exists(p => p.HasType(CardType.Xyz));
        }

        private bool PanzerDragonEffect()
        {
            ClientCard target = AI.Utils.GetBestEnemyCard();
            if (target != null)
            {
                AI.SelectCard(target);
                return true;
            }
            return false;
        }

        private bool XyzVeilEffect()
        {
            List<ClientCard> spells = Bot.GetSpells();
            foreach (ClientCard spell in spells)
            {
                if (spell.Id == (int)CardId.XyzVeil && !spell.IsFacedown())
                    return false;
            }
            List<ClientCard> monsters = Bot.GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                if (monster.HasType(CardType.Xyz))
                    return true;
            }
            return false;
        }

        private bool HaveLV5OnField()
        {
            List<ClientCard> monsters = Bot.GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                if (monster.HasType(CardType.Monster) &&
                    !monster.HasType(CardType.Xyz) &&
                    (monster.Level == 5
                    || monster.Id == (int)CardId.StarDrawing
                    || (monster.Id == (int)CardId.WindUpSoldier) && !monster.Equals(Card)))
                    return true;
            }
            return false;
        }
    }
}
