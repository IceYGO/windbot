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
        public enum CardId
        {
            Scout = 65518099,
            Stealth = 13073850,
            Shell = 90885155,
            Helix = 37991342,
            Carrier = 91907707,
            DarkHole = 53129443,
            CardOfDemise = 59750328,
            SummonersArt = 79816536,
            PotOfDuality = 98645731,
            Saqlifice = 17639150,
            MirrorForce = 44095762,
            TorrentialTribute = 53582587,
            DimensionalBarrier = 83326048,
            CompulsoryEvacuationDevice = 94192409,
            VanitysEmptiness = 5851097,
            SkillDrain = 82732705,
            SolemnStrike = 40605147,
            TheHugeRevolutionIsOver = 99188141
        }

        bool CardOfDemiseUsed = false;

        List<int> LowScaleCards = new List<int>
        {
            (int)CardId.Stealth,
            (int)CardId.Carrier
        };
        List<int> HighScaleCards = new List<int>
        {
            (int)CardId.Scout,
            (int)CardId.Shell,
            (int)CardId.Helix
        };

        public QliphortExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {

            AddExecutor(ExecutorType.Activate, (int)CardId.DarkHole, DefaultDarkHole);
            AddExecutor(ExecutorType.Activate, (int)CardId.SummonersArt);

            AddExecutor(ExecutorType.Activate, (int)CardId.Scout, ScoutActivate);
            AddExecutor(ExecutorType.Activate, (int)CardId.Scout, ScoutEffect);

            AddExecutor(ExecutorType.Activate, (int)CardId.Stealth, ScaleActivate);
            AddExecutor(ExecutorType.Activate, (int)CardId.Shell, ScaleActivate);
            AddExecutor(ExecutorType.Activate, (int)CardId.Helix, ScaleActivate);
            AddExecutor(ExecutorType.Activate, (int)CardId.Carrier, ScaleActivate);

            AddExecutor(ExecutorType.Summon, NormalSummon);
            AddExecutor(ExecutorType.SpSummon);

            AddExecutor(ExecutorType.Activate, (int)CardId.Saqlifice, SaqlificeEffect);

            AddExecutor(ExecutorType.Activate, (int)CardId.Stealth, StealthEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.Helix, HelixEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.Carrier, CarrierEffect);

            AddExecutor(ExecutorType.SpellSet, (int)CardId.SkillDrain, TrapSetUnique);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.VanitysEmptiness, TrapSetUnique);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.DimensionalBarrier, TrapSetUnique);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.TorrentialTribute, TrapSetUnique);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.SolemnStrike, TrapSetUnique);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.MirrorForce, TrapSetUnique);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.CompulsoryEvacuationDevice, TrapSetUnique);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.TheHugeRevolutionIsOver, TrapSetUnique);

            AddExecutor(ExecutorType.SpellSet, (int)CardId.Saqlifice, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.SkillDrain, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.VanitysEmptiness, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.DimensionalBarrier, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.TorrentialTribute, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.SolemnStrike, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.MirrorForce, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.CompulsoryEvacuationDevice, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.TheHugeRevolutionIsOver, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.DarkHole, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.SummonersArt, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.PotOfDuality, TrapSetWhenZoneFree);

            AddExecutor(ExecutorType.Activate, (int)CardId.PotOfDuality, PotOfDualityEffect);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.CardOfDemise);
            AddExecutor(ExecutorType.Activate, (int)CardId.CardOfDemise, CardOfDemiseEffect);

            AddExecutor(ExecutorType.SpellSet, (int)CardId.Saqlifice, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.SkillDrain, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.VanitysEmptiness, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.DimensionalBarrier, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.TorrentialTribute, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.SolemnStrike, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.MirrorForce, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.CompulsoryEvacuationDevice, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.TheHugeRevolutionIsOver, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.DarkHole, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.SummonersArt, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.PotOfDuality, CardOfDemiseAcivated);

            AddExecutor(ExecutorType.Activate, (int)CardId.TheHugeRevolutionIsOver, DefaultTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.SolemnStrike, DefaultSolemnStrike);
            AddExecutor(ExecutorType.Activate, (int)CardId.SkillDrain, SkillDrainEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.VanitysEmptiness, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.CompulsoryEvacuationDevice, DefaultCompulsoryEvacuationDevice);
            AddExecutor(ExecutorType.Activate, (int)CardId.DimensionalBarrier, DefaultDimensionalBarrier);
            AddExecutor(ExecutorType.Activate, (int)CardId.MirrorForce, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.TorrentialTribute, DefaultTorrentialTribute);

            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
        }

        public override bool OnSelectHand()
        {
            return true;
        }

        public override void OnNewTurn()
        {
            CardOfDemiseUsed = false;
        }

        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, bool cancelable)
        {
            if (max <= min)
            {
                return null;
            }
            IList<ClientCard> selected = new List<ClientCard>();

            // select the last cards
            for (int i = 1; i <= max; ++i)
                selected.Add(cards[cards.Count - i]);

            return selected;
        }

        private bool NormalSummon()
        {
            if (Card.Id == (int)CardId.Scout)
                return false;
            if (Card.Level < 8)
                AI.SelectOption(1);
            return true;
        }

        private bool SkillDrainEffect()
        {
            return (Duel.LifePoints[0] > 1000) && DefaultUniqueTrap();
        }

        private bool PotOfDualityEffect()
        {
            AI.SelectCard(new[]
                    {
                    (int)CardId.Scout,
                    (int)CardId.SkillDrain,
                    (int)CardId.VanitysEmptiness,
                    (int)CardId.DimensionalBarrier,
                    (int)CardId.Stealth,
                    (int)CardId.Shell,
                    (int)CardId.Helix,
                    (int)CardId.Carrier,
                    (int)CardId.SolemnStrike,
                    (int)CardId.CardOfDemise
                });
            return !ShouldPendulum();
        }

        private bool CardOfDemiseEffect()
        {
            if (AI.Utils.IsTurn1OrMain2() && !ShouldPendulum())
            {
                CardOfDemiseUsed = true;
                return true;
            }
            return false;
        }
        
        private bool TrapSetUnique()
        {
            foreach (ClientCard card in Bot.SpellZone)
            {
                if (card != null && card.Id == Card.Id)
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
                ClientCard l = AI.Utils.GetPZone(0, 0);
                ClientCard r = AI.Utils.GetPZone(0, 1);
                if (l == null && r == null)
                    AI.SelectCard((int)CardId.Scout);
            }
            return true;
        }

        private bool ScoutActivate()
        {
            if (Card.Location != CardLocation.Hand)
                return false;
            ClientCard l = AI.Utils.GetPZone(0, 0);
            ClientCard r = AI.Utils.GetPZone(0, 1);
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
            foreach (ClientCard card in Bot.ExtraDeck.GetMonsters())
            {
                if (card.HasType(CardType.Pendulum) && card.IsFaceup())
                    count++;
            }
            ClientCard l = AI.Utils.GetPZone(0, 0);
            ClientCard r = AI.Utils.GetPZone(0, 1);
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
            foreach (ClientCard card in Bot.ExtraDeck.GetMonsters())
            {
                if (card.HasType(CardType.Pendulum) && card.IsFaceup())
                    count++;
            }
            if (count>0 && !Bot.HasInHand(LowScaleCards))
            {
                AI.SelectCard(LowScaleCards);
            }
            else if (handcount>0 || fieldcount>0)
            {
                AI.SelectCard(new[]
                {
                    (int)CardId.Saqlifice,
                    (int)CardId.Shell,
                    (int)CardId.Helix
                });
            }
            else
            {
                AI.SelectCard(HighScaleCards);
            }
            return Duel.LifePoints[0] > 800;
        }

        private bool StealthEffect()
        {
            if (Card.Location == CardLocation.Hand)
                return false;
            ClientCard target = AI.Utils.GetProblematicCard();
            if (target != null)
            {
                AI.SelectCard(target);
                return true;
            }
            List<ClientCard> monsters = Enemy.GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                AI.SelectCard(monster);
                return true;
            }
            List<ClientCard> spells = Enemy.GetSpells();
            foreach (ClientCard spell in spells)
            {
                if (spell.IsFacedown())
                {
                    AI.SelectCard(spell);
                    return true;
                }
            }
            foreach (ClientCard spell in spells)
            {
                AI.SelectCard(spell);
                return true;
            }
            return false;
        }

        private bool CarrierEffect()
        {
            if (Card.Location == CardLocation.Hand)
                return false;
            ClientCard target = AI.Utils.GetProblematicMonsterCard();
            if (target != null)
            {
                AI.SelectCard(target);
                return true;
            }
            List<ClientCard> monsters = Enemy.GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                AI.SelectCard(monster);
                return true;
            }
            return false;
        }

        private bool HelixEffect()
        {
            if (Card.Location == CardLocation.Hand)
                return false;
            ClientCard target = AI.Utils.GetProblematicSpellCard();
            if (target != null)
            {
                AI.SelectCard(target);
                return true;
            }
            List<ClientCard> spells = Enemy.GetSpells();
            foreach (ClientCard spell in spells)
            {
                if (spell.IsFacedown())
                {
                    AI.SelectCard(spell);
                    return true;
                }
            }
            foreach (ClientCard spell in spells)
            {
                AI.SelectCard(spell);
                return true;
            }
            return false;
        }

        private bool ShouldPendulum()
        {
            ClientCard l = AI.Utils.GetPZone(0, 0);
            ClientCard r = AI.Utils.GetPZone(0, 1);
            if (l != null && r != null && l.LScale != r.RScale)
            {
                int count = 0;
                foreach (ClientCard card in Bot.Hand.GetMonsters())
                {
                    count++;
                }
                foreach (ClientCard card in Bot.ExtraDeck.GetMonsters())
                {
                    if (card.HasType(CardType.Pendulum) && card.IsFaceup())
                        count++;
                }
                return count > 1;
            }
            return false;
        }

    }
}