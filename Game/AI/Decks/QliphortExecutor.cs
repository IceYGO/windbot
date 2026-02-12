using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("Qliphort", "AI_Qliphort")]
    public class QliphortExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int Scout = 65518099;
            public const int Stealth = 13073850;
            public const int Shell = 90885155;
            public const int Helix = 37991342;
            public const int Carrier = 91907707;

            public const int DarkHole = 53129443;
            public const int CardOfDemise = 59750328;
            public const int SummonersArt = 79816536;
            public const int PotOfDuality = 98645731;
            public const int Saqlifice = 17639150;

            public const int MirrorForce = 44095762;
            public const int TorrentialTribute = 53582587;
            public const int DimensionalBarrier = 83326048;
            public const int CompulsoryEvacuationDevice = 94192409;
            public const int VanitysEmptiness = 5851097;
            public const int SkillDrain = 82732705;
            public const int SolemnStrike = 40605147;
            public const int TheHugeRevolutionIsOver = 99188141;
        }

        bool CardOfDemiseUsed = false;

        IList<int> LowScaleCards = new[]
        {
            CardId.Stealth,
            CardId.Carrier
        };
        IList<int> HighScaleCards = new[]
        {
            CardId.Scout,
            CardId.Shell,
            CardId.Helix
        };

        public QliphortExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {

            AddExecutor(ExecutorType.Activate, CardId.DarkHole, DefaultDarkHole);
            AddExecutor(ExecutorType.Activate, CardId.SummonersArt);

            AddExecutor(ExecutorType.Activate, CardId.Scout, ScoutActivate);
            AddExecutor(ExecutorType.Activate, CardId.Scout, ScoutEffect);

            AddExecutor(ExecutorType.Activate, CardId.Stealth, ScaleActivate);
            AddExecutor(ExecutorType.Activate, CardId.Shell, ScaleActivate);
            AddExecutor(ExecutorType.Activate, CardId.Helix, ScaleActivate);
            AddExecutor(ExecutorType.Activate, CardId.Carrier, ScaleActivate);

            AddExecutor(ExecutorType.Summon, NormalSummon);
            AddExecutor(ExecutorType.SpSummon);

            AddExecutor(ExecutorType.Activate, CardId.Saqlifice, SaqlificeEffect);

            AddExecutor(ExecutorType.Activate, CardId.Stealth, StealthEffect);
            AddExecutor(ExecutorType.Activate, CardId.Helix, HelixEffect);
            AddExecutor(ExecutorType.Activate, CardId.Carrier, CarrierEffect);

            AddExecutor(ExecutorType.SpellSet, CardId.SkillDrain, TrapSetUnique);
            AddExecutor(ExecutorType.SpellSet, CardId.VanitysEmptiness, TrapSetUnique);
            AddExecutor(ExecutorType.SpellSet, CardId.DimensionalBarrier, TrapSetUnique);
            AddExecutor(ExecutorType.SpellSet, CardId.TorrentialTribute, TrapSetUnique);
            AddExecutor(ExecutorType.SpellSet, CardId.SolemnStrike, TrapSetUnique);
            AddExecutor(ExecutorType.SpellSet, CardId.MirrorForce, TrapSetUnique);
            AddExecutor(ExecutorType.SpellSet, CardId.CompulsoryEvacuationDevice, TrapSetUnique);
            AddExecutor(ExecutorType.SpellSet, CardId.TheHugeRevolutionIsOver, TrapSetUnique);

            AddExecutor(ExecutorType.SpellSet, CardId.Saqlifice, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, CardId.SkillDrain, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, CardId.VanitysEmptiness, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, CardId.DimensionalBarrier, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, CardId.TorrentialTribute, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, CardId.SolemnStrike, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, CardId.MirrorForce, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, CardId.CompulsoryEvacuationDevice, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, CardId.TheHugeRevolutionIsOver, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, CardId.DarkHole, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, CardId.SummonersArt, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, CardId.PotOfDuality, TrapSetWhenZoneFree);

            AddExecutor(ExecutorType.Activate, CardId.PotOfDuality, PotOfDualityEffect);
            AddExecutor(ExecutorType.SpellSet, CardId.CardOfDemise);
            AddExecutor(ExecutorType.Activate, CardId.CardOfDemise, CardOfDemiseEffect);

            AddExecutor(ExecutorType.SpellSet, CardId.Saqlifice, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, CardId.SkillDrain, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, CardId.VanitysEmptiness, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, CardId.DimensionalBarrier, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, CardId.TorrentialTribute, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, CardId.SolemnStrike, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, CardId.MirrorForce, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, CardId.CompulsoryEvacuationDevice, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, CardId.TheHugeRevolutionIsOver, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, CardId.DarkHole, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, CardId.SummonersArt, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, CardId.PotOfDuality, CardOfDemiseAcivated);

            AddExecutor(ExecutorType.Activate, CardId.TheHugeRevolutionIsOver, DefaultTrap);
            AddExecutor(ExecutorType.Activate, CardId.SolemnStrike, DefaultSolemnStrike);
            AddExecutor(ExecutorType.Activate, CardId.SkillDrain, SkillDrainEffect);
            AddExecutor(ExecutorType.Activate, CardId.VanitysEmptiness, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, CardId.CompulsoryEvacuationDevice, DefaultCompulsoryEvacuationDevice);
            AddExecutor(ExecutorType.Activate, CardId.DimensionalBarrier, DefaultDimensionalBarrier);
            AddExecutor(ExecutorType.Activate, CardId.MirrorForce, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, CardId.TorrentialTribute, DefaultTorrentialTribute);

            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
        }

        public override bool OnSelectHand()
        {
            return true;
        }

        public override void OnNewTurn()
        {
            CardOfDemiseUsed = false;
            base.OnNewTurn();
        }

        public override IList<ClientCard> OnSelectPendulumSummon(IList<ClientCard> cards, int max)
        {
            Logger.DebugWriteLine("OnSelectPendulumSummon");
            // select the last cards

            IList<ClientCard> selected = new List<ClientCard>();
            for (int i = 1; i <= max; ++i)
            {
                ClientCard card = cards[cards.Count - i];
                if (!card.IsCode(CardId.Scout) || (card.Location == CardLocation.Extra && !Duel.IsNewRule))
                    selected.Add(card);
            }
            if (selected.Count == 0)
                selected.Add(cards[cards.Count - 1]);

            return selected;
        }

        private bool NormalSummon()
        {
            if (Card.IsCode(CardId.Scout))
                return false;
            if (Card.Level < 8)
                AI.SelectOption(1);
            return true;
        }

        private bool SkillDrainEffect()
        {
            return (Bot.LifePoints > 1000) && DefaultUniqueTrap();
        }

        private bool PotOfDualityEffect()
        {
            AI.SelectCard(
                CardId.Scout,
                CardId.SkillDrain,
                CardId.VanitysEmptiness,
                CardId.DimensionalBarrier,
                CardId.Stealth,
                CardId.Shell,
                CardId.Helix,
                CardId.Carrier,
                CardId.SolemnStrike,
                CardId.CardOfDemise
                );
            return !ShouldPendulum();
        }

        private bool CardOfDemiseEffect()
        {
            if (Util.IsTurn1OrMain2() && !ShouldPendulum())
            {
                CardOfDemiseUsed = true;
                return true;
            }
            return false;
        }

        private bool TrapSetUnique()
        {
            foreach (ClientCard card in Bot.GetSpells())
            {
                if (card.IsCode(Card.Id))
                    return false;
            }
            return TrapSetWhenZoneFree();
        }

        private bool TrapSetWhenZoneFree()
        {
            return Bot.GetSpellCountWithoutField() < 4;
        }

        private bool CardOfDemiseAcivated()
        {
            return CardOfDemiseUsed;
        }

        private bool SaqlificeEffect()
        {
            if (Card.Location == CardLocation.Grave)
            {
                ClientCard l = Util.GetPZone(0, 0);
                ClientCard r = Util.GetPZone(0, 1);
                if (l == null && r == null)
                    AI.SelectCard(CardId.Scout);
            }
            return true;
        }

        private bool ScoutActivate()
        {
            if (Card.Location != CardLocation.Hand)
                return false;
            ClientCard l = Util.GetPZone(0, 0);
            ClientCard r = Util.GetPZone(0, 1);
            if (l == null && r == null)
                return true;
            if (l == null && r.RScale != Card.LScale)
                return true;
            if (r == null && l.LScale != Card.RScale)
                return true;
            return false;
        }

        private bool ScaleActivate()
        {
            if (!Card.HasType(CardType.Pendulum) || Card.Location != CardLocation.Hand)
                return false;
            int count = 0;
            foreach (ClientCard card in Bot.Hand.GetMonsters())
            {
                if (!Card.Equals(card))
                    count++;
            }
            foreach (ClientCard card in Bot.ExtraDeck.GetFaceupPendulumMonsters())
            {
                count++;
            }
            ClientCard l = Util.GetPZone(0, 0);
            ClientCard r = Util.GetPZone(0, 1);
            if (l == null && r == null)
            {
                if (CardOfDemiseUsed)
                    return true;
                bool pair = false;
                foreach (ClientCard card in Bot.Hand.GetMonsters())
                {
                    if (card.RScale != Card.LScale)
                    {
                        pair = true;
                        count--;
                        break;
                    }
                }
                return pair && count>1;
            }
            if (l == null && r.RScale != Card.LScale)
                return count > 1 || CardOfDemiseUsed;
            if (r == null && l.LScale != Card.RScale)
                return count > 1 || CardOfDemiseUsed;
            return false;
        }

        private bool ScoutEffect()
        {
            if (Card.Location == CardLocation.Hand)
                return false;
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            int count = 0;
            int handcount = 0;
            int fieldcount = 0;
            foreach (ClientCard card in Bot.Hand.GetMonsters())
            {
                count++;
                handcount++;
            }
            foreach (ClientCard card in Bot.MonsterZone.GetMonsters())
            {
                fieldcount++;
            }
            foreach (ClientCard card in Bot.ExtraDeck.GetFaceupPendulumMonsters())
            {
                count++;
            }
            if (count>0 && !Bot.HasInHand(LowScaleCards))
            {
                AI.SelectCard(LowScaleCards);
            }
            else if (handcount>0 || fieldcount>0)
            {
                AI.SelectCard(CardId.Saqlifice, CardId.Shell, CardId.Helix);
            }
            else
            {
                AI.SelectCard(HighScaleCards);
            }
            return Bot.LifePoints > 800;
        }

        private bool StealthEffect()
        {
            if (Card.Location == CardLocation.Hand)
                return false;
            ClientCard target = Util.GetBestEnemyCard();
            if (target != null)
            {
                AI.SelectCard(target);
                return true;
            }
            return false;
        }

        private bool CarrierEffect()
        {
            if (Card.Location == CardLocation.Hand)
                return false;
            ClientCard target = Util.GetBestEnemyMonster();
            if (target != null)
            {
                AI.SelectCard(target);
                return true;
            }
            return false;
        }

        private bool HelixEffect()
        {
            if (Card.Location == CardLocation.Hand)
                return false;
            ClientCard target = Util.GetBestEnemySpell();
            if (target != null)
            {
                AI.SelectCard(target);
                return true;
            }
            return false;
        }

        private bool ShouldPendulum()
        {
            ClientCard l = Util.GetPZone(0, 0);
            ClientCard r = Util.GetPZone(0, 1);
            if (l != null && r != null && l.LScale != r.RScale)
            {
                int count = 0;
                foreach (ClientCard card in Bot.Hand.GetMonsters())
                {
                    count++;
                }
                foreach (ClientCard card in Bot.ExtraDeck.GetFaceupPendulumMonsters())
                {
                    count++;
                }
                return count > 1;
            }
            return false;
        }

    }
}