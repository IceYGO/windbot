using OCGWrapper.Enums;
using System;
using System.Collections.Generic;

namespace WindBot.Game.AI
{
    public abstract class DefaultExecutor : Executor
    {
        protected DefaultExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
        }

        private enum CardId
        {
            MysticalSpaceTyphoon = 5318639
        }

        protected bool DefaultMysticalSpaceTyphoon()
        {
            foreach (ClientCard card in CurrentChain)
                if (card.Id == (int)CardId.MysticalSpaceTyphoon)
                    return false;

            return DefaultStampingDestruction();
        }

        protected bool DefaultStampingDestruction()
        {
            List<ClientCard> spells = Duel.Fields[1].GetSpells();
            if (spells.Count == 0)
                return false;

            ClientCard selected = null;
            foreach (ClientCard card in spells)
            {
                if (card.IsSpellNegateAttack())
                {
                    selected = card;
                    break;
                }
            }

            if (selected == null)
            {
                foreach (ClientCard card in spells)
                {
                    if (Duel.Player == 1 && !card.HasType(CardType.Continuous))
                        continue;
                    selected = card;
                    if (Duel.Player == 0 && card.IsFacedown())
                        break;
                }
            }

            if (selected == null)
                return false;
            AI.SelectCard(selected);
            return true;
        }

        protected bool DefaultBookOfMoon()
        {
            if (AI.Utils.IsEnnemyBetter(true, true))
            {
                ClientCard monster = Duel.Fields[1].GetMonsters().GetHighestAttackMonster();
                if (monster != null && monster.HasType(CardType.Effect) && (monster.HasType(CardType.Xyz) || monster.Level > 4))
                {
                    AI.SelectCard(monster);
                    return true;
                }
            }
            return false;
        }

        protected bool DefaultTorrentialTribute()
        {
            return (AI.Utils.IsEnnemyBetter(true, true));
        }

        protected bool DefaultHeavyStorm()
        {
            return Duel.Fields[0].GetSpellCount() < Duel.Fields[1].GetSpellCount();
        }

        protected bool DefaultHammerShot()
        {
            return AI.Utils.IsEnnemyBetter(true, false);
        }

        protected bool DefaultDarkHole()
        {
            return AI.Utils.IsEnnemyBetter(false, false);
        }

        protected bool DefaultRaigeki()
        {
            return AI.Utils.IsEnnemyBetter(false, false);
        }

        protected bool DefaultSpellSet()
        {
            return Card.IsTrap() && Duel.Fields[0].GetSpellCountWithoutField() < 4;
        }

        protected bool DefaultTributeSummon()
        {
            int tributecount = (int)Math.Ceiling((Card.Level - 4.0d) / 2.0d);
            for (int j = 0; j < 5; ++j)
            {
                ClientCard tributeCard = Duel.Fields[0].MonsterZone[j];
                if (tributeCard == null) continue;
                if (tributeCard.Attack < Card.Attack)
                    tributecount--;
            }
            return tributecount <= 0;
        }

        protected bool DefaultField()
        {
            return Duel.Fields[0].SpellZone[5] == null;
        }

        protected bool DefaultMonsterRepos()
        {
            bool ennemyBetter = AI.Utils.IsEnnemyBetter(true, true);

            if (Card.IsAttack() && ennemyBetter)
                return true;
            if (Card.IsDefense() && !ennemyBetter && Card.Attack >= Card.Defense)
                return true;
            return false;
        }

        protected bool DefaultTrap()
        {
            return LastChainPlayer == -1 || LastChainPlayer == 1;
        }

        protected bool DefaultUniqueTrap()
        {
            if (HasChainedTrap(0))
                return false;

            foreach (ClientCard card in Duel.Fields[0].SpellZone)
            {
                if (card != null &&
                    card.Id == Card.Id &&
                    card.HasPosition(CardPosition.FaceUp))
                    return false;
            }

            return true;
        }
    }
}
