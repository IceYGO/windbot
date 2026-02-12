using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("ST1732", "AI_ST1732")]
    public class ST1732Executor : DefaultExecutor
    {
        public class CardId
        {
            public const int Digitron = 32295838;
            public const int Bitron = 36211150;
            public const int DualAssembloom = 7445307;
            public const int BootStagguard = 70950698;
            public const int Linkslayer = 35595518;
            public const int RAMClouder = 9190563;
            public const int ROMCloudia = 44956694;
            public const int BalancerLord = 8567955;
            public const int Backlinker = 71172240;
            public const int Kleinant = 45778242;
            public const int Draconnet = 62706865;
            public const int DotScaper = 18789533;

            public const int MindControl = 37520316;
            public const int DarkHole = 53129443;
            public const int MonsterReborn = 83764718;
            public const int MysticalSpaceTyphoon = 5318639;
            public const int CosmicCyclone = 8267140;
            public const int BookOfMoon = 14087893;
            public const int CynetBackdoor = 43839002;
            public const int MoonMirrorShield = 19508728;
            public const int CynetUniverse = 61583217;
            public const int BottomlessTrapHole = 29401950;
            public const int MirrorForce = 44095762;
            public const int TorrentialTribute = 53582587;
            public const int RecodedAlive = 70238111;
            public const int DimensionalBarrier = 83326048;
            public const int CompulsoryEvacuationDevice = 94192409;
            public const int SolemnStrike = 40605147;

            public const int DecodeTalker = 1861629;
            public const int EncodeTalker = 6622715;
            public const int TriGateWizard = 32617464;
            public const int Honeybot = 34472920;
            public const int BinarySorceress = 79016563;
            public const int LinkSpider = 98978921;

            public const int StagToken = 70950699;
        }

        bool BalancerLordUsed = false;

        public ST1732Executor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, CardId.CosmicCyclone, DefaultCosmicCyclone);
            AddExecutor(ExecutorType.Activate, CardId.MysticalSpaceTyphoon, DefaultMysticalSpaceTyphoon);
            AddExecutor(ExecutorType.Activate, CardId.DarkHole, DefaultDarkHole);
            AddExecutor(ExecutorType.Activate, CardId.BookOfMoon, DefaultBookOfMoon);

            AddExecutor(ExecutorType.Activate, CardId.CynetUniverse, CynetUniverseEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.Linkslayer);
            AddExecutor(ExecutorType.Activate, CardId.Linkslayer, LinkslayerEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.LinkSpider);
            AddExecutor(ExecutorType.Activate, CardId.LinkSpider);

            AddExecutor(ExecutorType.Activate, CardId.MindControl, MindControlEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.Backlinker);
            AddExecutor(ExecutorType.Activate, CardId.Backlinker, BacklinkerEffect);

            AddExecutor(ExecutorType.Activate, CardId.BootStagguard, BootStagguardEffect);

            AddExecutor(ExecutorType.Activate, CardId.MonsterReborn, MonsterRebornEffect);
            AddExecutor(ExecutorType.Activate, CardId.MoonMirrorShield, MoonMirrorShieldEffect);

            AddExecutor(ExecutorType.Activate, CardId.CynetBackdoor, CynetBackdoorEffect);
            AddExecutor(ExecutorType.Activate, CardId.RecodedAlive);

            AddExecutor(ExecutorType.Summon, CardId.BalancerLord, BalancerLordSummon);

            AddExecutor(ExecutorType.Summon, CardId.ROMCloudia, ROMCloudiaSummon);
            AddExecutor(ExecutorType.Activate, CardId.ROMCloudia, ROMCloudiaEffect);

            AddExecutor(ExecutorType.Summon, CardId.Draconnet, DraconnetSummon);
            AddExecutor(ExecutorType.Activate, CardId.Draconnet, DraconnetEffect);

            AddExecutor(ExecutorType.Summon, CardId.Kleinant);
            AddExecutor(ExecutorType.Activate, CardId.Kleinant, KleinantEffect);

            AddExecutor(ExecutorType.Summon, CardId.RAMClouder);
            AddExecutor(ExecutorType.Activate, CardId.RAMClouder, RAMClouderEffect);

            AddExecutor(ExecutorType.SummonOrSet, CardId.DotScaper);
            AddExecutor(ExecutorType.Activate, CardId.DotScaper, DotScaperEffect);

            AddExecutor(ExecutorType.Summon, CardId.BalancerLord);
            AddExecutor(ExecutorType.Summon, CardId.ROMCloudia);
            AddExecutor(ExecutorType.Summon, CardId.Draconnet);
            AddExecutor(ExecutorType.SummonOrSet, CardId.Backlinker);
            AddExecutor(ExecutorType.SummonOrSet, CardId.Digitron);
            AddExecutor(ExecutorType.SummonOrSet, CardId.Bitron);

            AddExecutor(ExecutorType.Activate, CardId.BalancerLord, BalancerLordEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.DecodeTalker, LinkSummon);
            AddExecutor(ExecutorType.Activate, CardId.DecodeTalker);

            AddExecutor(ExecutorType.SpSummon, CardId.TriGateWizard, LinkSummon);
            AddExecutor(ExecutorType.Activate, CardId.TriGateWizard);

            AddExecutor(ExecutorType.SpSummon, CardId.EncodeTalker, LinkSummon);
            AddExecutor(ExecutorType.Activate, CardId.EncodeTalker);

            AddExecutor(ExecutorType.SpSummon, CardId.Honeybot, LinkSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.BinarySorceress, LinkSummon);
            AddExecutor(ExecutorType.Activate, CardId.BinarySorceress);

            AddExecutor(ExecutorType.SpellSet, CardId.CynetBackdoor, DefaultSpellSet);
            AddExecutor(ExecutorType.SpellSet, CardId.RecodedAlive, DefaultSpellSet);

            AddExecutor(ExecutorType.SpellSet, CardId.SolemnStrike, DefaultSpellSet);
            AddExecutor(ExecutorType.SpellSet, CardId.CompulsoryEvacuationDevice, DefaultSpellSet);
            AddExecutor(ExecutorType.SpellSet, CardId.DimensionalBarrier, DefaultSpellSet);
            AddExecutor(ExecutorType.SpellSet, CardId.TorrentialTribute, DefaultSpellSet);
            AddExecutor(ExecutorType.SpellSet, CardId.MirrorForce, DefaultSpellSet);
            AddExecutor(ExecutorType.SpellSet, CardId.BottomlessTrapHole, DefaultSpellSet);
            AddExecutor(ExecutorType.SpellSet, CardId.BookOfMoon, DefaultSpellSet);
            AddExecutor(ExecutorType.SpellSet, CardId.CosmicCyclone, DefaultSpellSet);
            AddExecutor(ExecutorType.SpellSet, CardId.MysticalSpaceTyphoon, DefaultSpellSet);

            AddExecutor(ExecutorType.Activate, CardId.SolemnStrike, DefaultSolemnStrike);
            AddExecutor(ExecutorType.Activate, CardId.CompulsoryEvacuationDevice, DefaultCompulsoryEvacuationDevice);
            AddExecutor(ExecutorType.Activate, CardId.DimensionalBarrier, DefaultDimensionalBarrier);
            AddExecutor(ExecutorType.Activate, CardId.TorrentialTribute, DefaultTorrentialTribute);
            AddExecutor(ExecutorType.Activate, CardId.MirrorForce, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, CardId.BottomlessTrapHole, DefaultUniqueTrap);

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
            base.OnNewTurn();
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
                AI.SelectCard(
                    CardId.DualAssembloom,
                    CardId.Bitron,
                    CardId.Digitron,
                    CardId.RecodedAlive
                    );
                AI.SelectNextCard(targets);
                return true;
            }
            return false;
        }

        private bool MindControlEffect()
        {
            ClientCard target = Util.GetBestEnemyMonster();
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
            IList<int> targets = new[] {
                    CardId.DecodeTalker,
                    CardId.EncodeTalker,
                    CardId.TriGateWizard,
                    CardId.BinarySorceress,
                    CardId.Honeybot,
                    CardId.DualAssembloom,
                    CardId.BootStagguard,
                    CardId.BalancerLord,
                    CardId.ROMCloudia,
                    CardId.Linkslayer,
                    CardId.RAMClouder,
                    CardId.Backlinker,
                    CardId.Kleinant
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
            foreach (ClientCard monster in Bot.GetMonsters())
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
            foreach (ClientCard card in Enemy.Graveyard)
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
            if (!UniqueFaceupSpell())
                return false;
            bool selected = false;
            foreach (ClientCard monster in Bot.GetMonstersInExtraZone())
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
                List<ClientCard> monsters = Bot.GetMonsters();
                foreach (ClientCard monster in monsters)
                {
                    if (monster.IsCode(CardId.BalancerLord))
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
                AI.SelectNextCard(
                    CardId.ROMCloudia,
                    CardId.BalancerLord,
                    CardId.Kleinant,
                    CardId.Draconnet,
                    CardId.Backlinker
                    );
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
            bool hastarget = Bot.HasInHand(new[] {
                    CardId.Draconnet,
                    CardId.Kleinant,
                    CardId.BalancerLord,
                    CardId.ROMCloudia,
                    CardId.RAMClouder,
                    CardId.DotScaper
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
            return Bot.HasInGraveyard(new[] {
                    CardId.BootStagguard,
                    CardId.BalancerLord,
                    CardId.Kleinant,
                    CardId.Linkslayer,
                    CardId.Draconnet,
                    CardId.RAMClouder
                });
        }

        private bool ROMCloudiaEffect()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {
                AI.SelectCard(
                    CardId.BootStagguard,
                    CardId.BalancerLord,
                    CardId.Kleinant,
                    CardId.Linkslayer,
                    CardId.Draconnet,
                    CardId.RAMClouder
                    );
                return true;
            }
            else
            {
                AI.SelectCard(
                    CardId.BalancerLord,
                    CardId.Kleinant,
                    CardId.RAMClouder,
                    CardId.DotScaper
                    );
                return true;
            }
        }

        private bool DraconnetSummon()
        {
            return Bot.GetRemainingCount(CardId.Digitron, 1) > 0
                || Bot.GetRemainingCount(CardId.Bitron, 1) > 0;
        }

        private bool DraconnetEffect()
        {
            AI.SelectCard(CardId.Bitron);
            return true;
        }

        private bool KleinantEffect()
        {
            IList<int> targets = new[] {
                CardId.DualAssembloom,
                CardId.Bitron,
                CardId.Digitron,
                CardId.DotScaper
            };
            foreach (ClientCard monster in Bot.Hand)
            {
                if (monster.IsCode(targets))
                {
                    AI.SelectCard(targets);
                    return true;
                }
            }
            IList<int> targets2 = new[] {
                CardId.StagToken,
                CardId.Bitron,
                CardId.Digitron,
                CardId.DotScaper
            };
            foreach (ClientCard monster in Bot.GetMonsters())
            {
                if (monster.IsCode(targets2))
                {
                    AI.SelectCard(targets2);
                    return true;
                }
            }
            return false;
        }

        private bool RAMClouderEffect()
        {
            AI.SelectCard(
                CardId.StagToken,
                CardId.Bitron,
                CardId.Digitron,
                CardId.DotScaper,
                CardId.Draconnet,
                CardId.Backlinker,
                CardId.RAMClouder
                );
            AI.SelectNextCard(
                CardId.DecodeTalker,
                CardId.EncodeTalker,
                CardId.TriGateWizard,
                CardId.BinarySorceress,
                CardId.Honeybot,
                CardId.DualAssembloom,
                CardId.BootStagguard,
                CardId.BalancerLord,
                CardId.ROMCloudia,
                CardId.Linkslayer,
                CardId.RAMClouder
                );
            return true;
        }

        private bool DotScaperEffect()
        {
            AI.SelectPosition(CardPosition.FaceUpDefence);
            return true;
        }

        private bool LinkSummon()
        {
            return (Util.IsTurn1OrMain2() || Util.IsOneEnemyBetter())
                && Util.GetBestAttack(Bot) < Card.Attack;
        }
    }
}