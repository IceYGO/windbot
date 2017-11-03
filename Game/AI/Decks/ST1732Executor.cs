using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("ST1732", "AI_ST1732", "Normal")]
    public class ST1732Executor : DefaultExecutor
    {
        public enum CardId
        {
            Digitron = 32295838,
            Bitron = 36211150,
            DualAssembloom = 7445307,
            BootStagguard = 70950698,
            Linkslayer = 35595518,
            RAMClouder = 9190563,
            ROMCloudia = 44956694,
            BalancerLord = 8567955,
            Backlinker = 71172240,
            Kleinant = 45778242,
            Draconnet = 62706865,
            DotScaper = 18789533,

            MindControl = 37520316,
            DarkHole = 53129443,
            MonsterReborn = 83764718,
            MysticalSpaceTyphoon = 5318639,
            CosmicCyclone = 8267140,
            BookOfMoon = 14087893,
            CynetBackdoor = 43839002,
            MoonMirrorShield = 19508728,
            CynetUniverse = 61583217,
            BottomlessTrapHole = 29401950,
            MirrorForce = 44095762,
            TorrentialTribute = 53582587,
            RecodedAlive = 70238111,
            DimensionalBarrier = 83326048,
            CompulsoryEvacuationDevice = 94192409,
            SolemnStrike = 40605147,

            DecodeTalker = 1861629,
            EncodeTalker = 6622715,
            TriGateWizard = 32617464,
            Honeybot = 34472920,
            BinarySorceress = 79016563,
            LinkSpider = 98978921,

            StagToken = 70950699
        }

        bool BalancerLordUsed = false;

        public ST1732Executor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, (int)CardId.CosmicCyclone, DefaultCosmicCyclone);
            AddExecutor(ExecutorType.Activate, (int)CardId.MysticalSpaceTyphoon, DefaultMysticalSpaceTyphoon);
            AddExecutor(ExecutorType.Activate, (int)CardId.DarkHole, DefaultDarkHole);
            AddExecutor(ExecutorType.Activate, (int)CardId.BookOfMoon, DefaultBookOfMoon);

            AddExecutor(ExecutorType.Activate, (int)CardId.CynetUniverse, CynetUniverseEffect);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.Linkslayer);
            AddExecutor(ExecutorType.Activate, (int)CardId.Linkslayer, LinkslayerEffect);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.LinkSpider);
            AddExecutor(ExecutorType.Activate, (int)CardId.LinkSpider);

            AddExecutor(ExecutorType.Activate, (int)CardId.MindControl, MindControlEffect);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.Backlinker);
            AddExecutor(ExecutorType.Activate, (int)CardId.Backlinker, BacklinkerEffect);

            AddExecutor(ExecutorType.Activate, (int)CardId.BootStagguard, BootStagguardEffect);

            AddExecutor(ExecutorType.Activate, (int)CardId.MonsterReborn, MonsterRebornEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.MoonMirrorShield, MoonMirrorShieldEffect);

            AddExecutor(ExecutorType.Activate, (int)CardId.CynetBackdoor, CynetBackdoorEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.RecodedAlive);

            AddExecutor(ExecutorType.Summon, (int)CardId.BalancerLord, BalancerLordSummon);

            AddExecutor(ExecutorType.Summon, (int)CardId.ROMCloudia, ROMCloudiaSummon);
            AddExecutor(ExecutorType.Activate, (int)CardId.ROMCloudia, ROMCloudiaEffect);

            AddExecutor(ExecutorType.Summon, (int)CardId.Draconnet, DraconnetSummon);
            AddExecutor(ExecutorType.Activate, (int)CardId.Draconnet, DraconnetEffect);

            AddExecutor(ExecutorType.Summon, (int)CardId.Kleinant);
            AddExecutor(ExecutorType.Activate, (int)CardId.Kleinant, KleinantEffect);

            AddExecutor(ExecutorType.Summon, (int)CardId.RAMClouder);
            AddExecutor(ExecutorType.Activate, (int)CardId.RAMClouder, RAMClouderEffect);

            AddExecutor(ExecutorType.SummonOrSet, (int)CardId.DotScaper);
            AddExecutor(ExecutorType.Activate, (int)CardId.DotScaper, DotScaperEffect);

            AddExecutor(ExecutorType.Summon, (int)CardId.BalancerLord);
            AddExecutor(ExecutorType.Summon, (int)CardId.ROMCloudia);
            AddExecutor(ExecutorType.Summon, (int)CardId.Draconnet);
            AddExecutor(ExecutorType.SummonOrSet, (int)CardId.Backlinker);
            AddExecutor(ExecutorType.SummonOrSet, (int)CardId.Digitron);
            AddExecutor(ExecutorType.SummonOrSet, (int)CardId.Bitron);

            AddExecutor(ExecutorType.Activate, (int)CardId.BalancerLord, BalancerLordEffect);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.DecodeTalker, LinkSummon);
            AddExecutor(ExecutorType.Activate, (int)CardId.DecodeTalker);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.TriGateWizard, LinkSummon);
            AddExecutor(ExecutorType.Activate, (int)CardId.TriGateWizard);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.EncodeTalker, LinkSummon);
            AddExecutor(ExecutorType.Activate, (int)CardId.EncodeTalker);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.Honeybot, LinkSummon);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.BinarySorceress, LinkSummon);
            AddExecutor(ExecutorType.Activate, (int)CardId.BinarySorceress);

            AddExecutor(ExecutorType.SpellSet, (int)CardId.CynetBackdoor, DefaultSpellSet);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.RecodedAlive, DefaultSpellSet);

            AddExecutor(ExecutorType.SpellSet, (int)CardId.SolemnStrike, DefaultSpellSet);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.CompulsoryEvacuationDevice, DefaultSpellSet);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.DimensionalBarrier, DefaultSpellSet);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.TorrentialTribute, DefaultSpellSet);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.MirrorForce, DefaultSpellSet);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.BottomlessTrapHole, DefaultSpellSet);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.BookOfMoon, DefaultSpellSet);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.CosmicCyclone, DefaultSpellSet);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.MysticalSpaceTyphoon, DefaultSpellSet);

            AddExecutor(ExecutorType.Activate, (int)CardId.SolemnStrike, DefaultSolemnStrike);
            AddExecutor(ExecutorType.Activate, (int)CardId.CompulsoryEvacuationDevice, DefaultCompulsoryEvacuationDevice);
            AddExecutor(ExecutorType.Activate, (int)CardId.DimensionalBarrier, DefaultDimensionalBarrier);
            AddExecutor(ExecutorType.Activate, (int)CardId.TorrentialTribute, DefaultTorrentialTribute);
            AddExecutor(ExecutorType.Activate, (int)CardId.MirrorForce, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.BottomlessTrapHole, DefaultUniqueTrap);

            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
        }

        public override bool OnSelectHand()
        {
            // go second
            return false;
        }

        public override void OnNewTurn()
        {
            // reset
            BalancerLordUsed = false;
        }

        public override int OnSelectOption(IList<int> options)
        {
            // put Moon Mirror Shield to the bottom of deck
            return options.Count == 2 ? 1 : 0;
        }

        public override bool OnSelectYesNo(int desc)
        {
            if (desc == 210) // Continue selecting? (Link Summoning)
                return false;
            if (desc == 31) // Direct Attack?
                return true;
            return base.OnSelectYesNo(desc);
        }

        private bool LinkslayerEffect()
        {
            IList<ClientCard> targets = Enemy.GetSpells();
            if (targets.Count > 0)
            {
                AI.SelectCard(new[]{
                    (int)CardId.DualAssembloom,
                    (int)CardId.Bitron,
                    (int)CardId.Digitron,
                    (int)CardId.RecodedAlive
                });
                AI.SelectNextCard(targets);
                return true;
            }
            return false;
        }

        private bool MindControlEffect()
        {
            ClientCard target = AI.Utils.GetBestEnemyMonster();
            if (target != null)
            {
                AI.SelectCard(target);
                return true;
            }
            return false;
        }

        private bool BacklinkerEffect()
        {
            return (Bot.MonsterZone[5] == null) && (Bot.MonsterZone[6] == null);
        }

        private bool BootStagguardEffect()
        {
            if (Card.Location != CardLocation.Hand)
                AI.SelectPosition(CardPosition.FaceUpDefence);
            return true;
        }

        private bool MonsterRebornEffect()
        {
            List<int> targets = new List<int> {
                    (int)CardId.DecodeTalker,
                    (int)CardId.EncodeTalker,
                    (int)CardId.TriGateWizard,
                    (int)CardId.BinarySorceress,
                    (int)CardId.Honeybot,
                    (int)CardId.DualAssembloom,
                    (int)CardId.BootStagguard,
                    (int)CardId.BalancerLord,
                    (int)CardId.ROMCloudia,
                    (int)CardId.Linkslayer,
                    (int)CardId.RAMClouder,
                    (int)CardId.Backlinker,
                    (int)CardId.Kleinant
                };
            if (!Bot.HasInGraveyard(targets))
            {
                return false;
            }
            AI.SelectCard(targets);
            return true;
        }

        private bool MoonMirrorShieldEffect()
        {
            List<ClientCard> monsters = Bot.GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                AI.SelectCard(monster);
                return true;
            }
            return false;
        }

        private bool CynetUniverseEffect()
        {
            if (Card.Location == CardLocation.Hand)
                return DefaultField();
            IList<ClientCard> cards = Enemy.Graveyard;
            foreach (ClientCard card in cards)
            {
                if (card.IsMonster())
                {
                    AI.SelectCard(card);
                    return true;
                }
            }
            return false;
        }

        private bool CynetBackdoorEffect()
        {
            if (!(Duel.Player == 0 && Duel.Phase == DuelPhase.Main2) &&
                !(Duel.Player == 1 && (Duel.Phase == DuelPhase.BattleStart || Duel.Phase == DuelPhase.End)))
            {
                return false;
            }
            foreach (ClientCard card in Bot.SpellZone)
            {
                if (card != null &&
                    card.Id == Card.Id &&
                    card.HasPosition(CardPosition.FaceUp))
                    return false;
            }
            bool selected = false;
            List<ClientCard> monsters = Bot.GetMonstersInExtraZone();
            foreach (ClientCard monster in monsters)
            {
                if (monster.Attack > 1000)
                {
                    AI.SelectCard(monster);
                    selected = true;
                    break;
                }
            }
            if (!selected)
            {
                monsters = Bot.GetMonsters();
                foreach (ClientCard monster in monsters)
                {
                    if (monster.Id == (int)CardId.BalancerLord)
                    {
                        AI.SelectCard(monster);
                        selected = true;
                        break;
                    }
                }
                if (!selected)
                {
                    foreach (ClientCard monster in monsters)
                    {
                        if (monster.Attack >= 1700)
                        {
                            AI.SelectCard(monster);
                            selected = true;
                            break;
                        }
                    }
                }
            }
            if (selected)
            {
                AI.SelectNextCard(new[]
                {
                    (int)CardId.ROMCloudia,
                    (int)CardId.BalancerLord,
                    (int)CardId.Kleinant,
                    (int)CardId.Draconnet,
                    (int)CardId.Backlinker
                });
                return true;
            }
            return false;
        }

        private bool BalancerLordSummon()
        {
            return !BalancerLordUsed;
        }

        private bool BalancerLordEffect()
        {
            if (Card.Location == CardLocation.Removed)
                return true;
            bool hastarget = Bot.HasInHand(new List<int> {
                    (int)CardId.Draconnet,
                    (int)CardId.Kleinant,
                    (int)CardId.BalancerLord,
                    (int)CardId.ROMCloudia,
                    (int)CardId.RAMClouder,
                    (int)CardId.DotScaper
                });
            if (hastarget && !BalancerLordUsed)
            {
                BalancerLordUsed = true;
                return true;
            }
            return false;
        }

        private bool ROMCloudiaSummon()
        {
            return Bot.HasInGraveyard(new List<int> {
                    (int)CardId.BootStagguard,
                    (int)CardId.BalancerLord,
                    (int)CardId.Kleinant,
                    (int)CardId.Linkslayer,
                    (int)CardId.Draconnet,
                    (int)CardId.RAMClouder
                });
        }

        private bool ROMCloudiaEffect()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {
                AI.SelectCard(new[]{
                    (int)CardId.BootStagguard,
                    (int)CardId.BalancerLord,
                    (int)CardId.Kleinant,
                    (int)CardId.Linkslayer,
                    (int)CardId.Draconnet,
                    (int)CardId.RAMClouder
                });
                return true;
            }
            else
            {
                AI.SelectCard(new[]{
                    (int)CardId.BalancerLord,
                    (int)CardId.Kleinant,
                    (int)CardId.RAMClouder,
                    (int)CardId.DotScaper
                });
                return true;
            }
        }

        private bool DraconnetSummon()
        {
            return Bot.GetRemainingCount((int)CardId.Digitron, 1) > 0
                || Bot.GetRemainingCount((int)CardId.Bitron, 1) > 0;
        }

        private bool DraconnetEffect()
        {
            AI.SelectCard((int)CardId.Bitron);
            return true;
        }

        private bool KleinantEffect()
        {
            IList<int> targets = new[] {
                (int)CardId.DualAssembloom,
                (int)CardId.Bitron,
                (int)CardId.Digitron,
                (int)CardId.DotScaper
            };
            foreach (ClientCard monster in Bot.Hand)
            {
                if (targets.Contains(monster.Id))
                {
                    AI.SelectCard(targets);
                    return true;
                }
            }
            IList<int> targets2 = new[] {
                (int)CardId.StagToken,
                (int)CardId.Bitron,
                (int)CardId.Digitron,
                (int)CardId.DotScaper
            };
            foreach (ClientCard monster in Bot.GetMonsters())
            {
                if (targets2.Contains(monster.Id))
                {
                    AI.SelectCard(targets2);
                    return true;
                }
            }
            return false;
        }

        private bool RAMClouderEffect()
        {
            AI.SelectCard(new[]{
                    (int)CardId.StagToken,
                    (int)CardId.Bitron,
                    (int)CardId.Digitron,
                    (int)CardId.DotScaper,
                    (int)CardId.Draconnet,
                    (int)CardId.Backlinker,
                    (int)CardId.RAMClouder
                });
            AI.SelectNextCard(new[]{
                    (int)CardId.DecodeTalker,
                    (int)CardId.EncodeTalker,
                    (int)CardId.TriGateWizard,
                    (int)CardId.BinarySorceress,
                    (int)CardId.Honeybot,
                    (int)CardId.DualAssembloom,
                    (int)CardId.BootStagguard,
                    (int)CardId.BalancerLord,
                    (int)CardId.ROMCloudia,
                    (int)CardId.Linkslayer,
                    (int)CardId.RAMClouder
                });
            return true;
        }

        private bool DotScaperEffect()
        {
            AI.SelectPosition(CardPosition.FaceUpDefence);
            return true;
        }

        private bool LinkSummon()
        {
            return (AI.Utils.IsTurn1OrMain2() || AI.Utils.IsOneEnemyBetter())
                && AI.Utils.GetBestAttack(Bot) < Card.Attack;
        }
    }
}