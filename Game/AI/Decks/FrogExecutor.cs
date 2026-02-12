using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using YGOSharp.OCGWrapper.Enums;

namespace WindBot.Game.AI.Decks
{
    [Deck("Frog", "AI_Frog", "Easy")]
    public class FrogExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int CryomancerOfTheIceBarrier = 23950192;
            public const int DewdarkOfTheIceBarrier = 90311614;
            public const int SubmarineFrog = 63948258;
            public const int SwapFrog = 9126351;
            public const int FlipFlopFrog = 81278754;
            public const int Unifrog = 56052205;
            public const int Ronintoadin = 1357146;
            public const int DupeFrog = 46239604;
            public const int Tradetoad = 23408872;
            public const int TreebornFrog = 12538374;
            public const int DarkHole = 53129443;
            public const int Raigeki = 12580477;
            public const int Terraforming = 73628505;
            public const int PotOfDuality = 98645731;
            public const int Solidarity = 86780027;
            public const int Wetlands = 2084239;
            public const int FroggyForcefield = 34351849;
            public const int GravityBind = 85742772;
            public const int TheHugeRevolutionIsOver = 99188141;
        }

        public FrogExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);

            AddExecutor(ExecutorType.Activate, CardId.Solidarity, Solidarity);
            AddExecutor(ExecutorType.Activate, CardId.Terraforming, Terraforming);
            AddExecutor(ExecutorType.Activate, CardId.Wetlands, DefaultField);
            AddExecutor(ExecutorType.Activate, CardId.DarkHole, DefaultDarkHole);
            AddExecutor(ExecutorType.Activate, CardId.Raigeki, DefaultRaigeki);
            AddExecutor(ExecutorType.Activate, CardId.PotOfDuality, PotOfDuality);

            AddExecutor(ExecutorType.SpSummon, CardId.SwapFrog, SwapFrogSummon);
            AddExecutor(ExecutorType.Activate, CardId.SwapFrog, SwapFrogActivate);
            AddExecutor(ExecutorType.Activate, CardId.DupeFrog, DupeFrog);
            AddExecutor(ExecutorType.Activate, CardId.FlipFlopFrog, FlipFlopFrog);
            AddExecutor(ExecutorType.Activate, CardId.Ronintoadin, Ronintoadin);
            AddExecutor(ExecutorType.Activate, CardId.TreebornFrog, TreebornFrog);
            AddExecutor(ExecutorType.Activate, CardId.Unifrog);

            AddExecutor(ExecutorType.Summon, CardId.CryomancerOfTheIceBarrier, SummonFrog);
            AddExecutor(ExecutorType.Summon, CardId.DewdarkOfTheIceBarrier, SummonFrog);
            AddExecutor(ExecutorType.Summon, CardId.SubmarineFrog, SummonFrog);
            AddExecutor(ExecutorType.Summon, CardId.SwapFrog, SummonFrog);
            AddExecutor(ExecutorType.Summon, CardId.Unifrog, SummonFrog);
            AddExecutor(ExecutorType.Summon, CardId.Ronintoadin, SummonFrog);
            AddExecutor(ExecutorType.Summon, CardId.DupeFrog, SummonFrog);
            AddExecutor(ExecutorType.Summon, CardId.Tradetoad, SummonFrog);
            AddExecutor(ExecutorType.Summon, CardId.TreebornFrog, SummonFrog);
            AddExecutor(ExecutorType.Summon, CardId.FlipFlopFrog, SummonFrog);

            AddExecutor(ExecutorType.MonsterSet, CardId.FlipFlopFrog);
            AddExecutor(ExecutorType.MonsterSet, CardId.DupeFrog);
            AddExecutor(ExecutorType.MonsterSet, CardId.Tradetoad);
            AddExecutor(ExecutorType.MonsterSet, CardId.Ronintoadin);
            AddExecutor(ExecutorType.MonsterSet, CardId.TreebornFrog);
            AddExecutor(ExecutorType.MonsterSet, CardId.Unifrog);
            AddExecutor(ExecutorType.MonsterSet, CardId.SwapFrog);
            AddExecutor(ExecutorType.MonsterSet, CardId.SubmarineFrog);
            AddExecutor(ExecutorType.MonsterSet, CardId.DewdarkOfTheIceBarrier);
            AddExecutor(ExecutorType.MonsterSet, CardId.CryomancerOfTheIceBarrier);

            AddExecutor(ExecutorType.Repos, FrogMonsterRepos);

            AddExecutor(ExecutorType.Activate, CardId.FroggyForcefield, DefaultTrap);
            AddExecutor(ExecutorType.Activate, CardId.TheHugeRevolutionIsOver, DefaultTrap);
            AddExecutor(ExecutorType.Activate, CardId.GravityBind, GravityBind);
        }

        private int m_swapFrogSummoned;
        private int m_flipFlopFrogSummoned;

        private bool TreebornFrog()
        {
            return true;
        }

        private bool SwapFrogSummon()
        {
            int atk = Card.Attack + GetSpellBonus();
            if (Util.IsAllEnemyBetterThanValue(atk, true))
                return false;

            AI.SelectCard(CardId.Ronintoadin);
            m_swapFrogSummoned = Duel.Turn;
            return true;
        }

        private bool SwapFrogActivate()
        {
            if (m_swapFrogSummoned != Duel.Turn)
                return false;
            m_swapFrogSummoned = -1;

            if (Bot.GetRemainingCount(CardId.Ronintoadin, 2) == 0)
                return false;

            AI.SelectCard(CardId.Ronintoadin);
            return true;
        }

        private bool DupeFrog()
        {
            AI.SelectCard(CardLocation.Deck);
            return true;
        }

        private bool FlipFlopFrog()
        {
            if (Card.IsDefense() || m_flipFlopFrogSummoned == Duel.Turn || Duel.Phase == DuelPhase.Main2)
            {
                m_flipFlopFrogSummoned = -1;
                List<ClientCard> monsters = Enemy.GetMonsters();
                monsters.Sort(CardContainer.CompareCardAttack);
                monsters.Reverse();
                AI.SelectCard(monsters);
                return true;
            }
            return false;
        }

        private bool Ronintoadin()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            List<ClientCard> monsters = Bot.GetGraveyardMonsters();
            if (monsters.Count > 2)
            {
                if (GetSpellBonus() == 0)
                    AI.SelectPosition(CardPosition.FaceUpDefence);
                return true;
            }
            return false;
        }

        private bool SummonFrog()
        {
            int atk = Card.Attack + GetSpellBonus();

            if (Util.IsOneEnemyBetterThanValue(atk, true))
                return false;

            if (Card.IsCode(CardId.SwapFrog))
                m_swapFrogSummoned = Duel.Turn;
            return true;
        }

        private bool PotOfDuality()
        {
            List<int> cards = new List<int>();
            
            if (Util.IsOneEnemyBetter())
            {
                cards.Add(CardId.FlipFlopFrog);
            }

            if (Bot.SpellZone[5] == null)
            {
                cards.Add(CardId.Terraforming);
                cards.Add(CardId.Wetlands);
            }

            cards.Add(CardId.DarkHole);
            cards.Add(CardId.SwapFrog);
            cards.Add(CardId.GravityBind);

            if (cards.Count > 0)
            {
                AI.SelectCard(cards);
                return true;
            }

            return false;
        }

        private bool Terraforming()
        {
            if (Bot.HasInHand(CardId.Wetlands))
                return false;
            if (Bot.SpellZone[5] != null)
                return false;
            return true;
        }

        private bool Solidarity()
        {
            List<ClientCard> monsters = Bot.GetGraveyardMonsters();
            return monsters.Count != 0;
        }

        private bool GravityBind()
        {
            List<ClientCard> spells = Bot.GetSpells();
            foreach (ClientCard spell in spells)
            {
                if (spell.IsCode(CardId.GravityBind) && !spell.IsFacedown())
                    return false;
            }
            return true;
        }

        private bool FrogMonsterRepos()
        {
            if (Card.IsCode(CardId.Unifrog))
                return Card.IsDefense();
            if (Card.IsCode(CardId.DewdarkOfTheIceBarrier))
                return Card.IsDefense();

            bool enemyBetter = Util.IsOneEnemyBetterThanValue(Card.Attack + (Card.IsFacedown() ? GetSpellBonus() : 0), true);
            if (Card.Attack < 800)
                enemyBetter = true;
            bool result = false;
            if (Card.IsAttack() && enemyBetter)
                result =  true;
            if (Card.IsDefense() && !enemyBetter)
                result = true;

            if (!result && Card.IsCode(CardId.FlipFlopFrog) && Enemy.GetMonsterCount() > 0 && Card.IsFacedown())
                result = true;

            if (Card.IsCode(CardId.FlipFlopFrog) && Card.IsFacedown() && result)
                m_flipFlopFrogSummoned = Duel.Turn;

            return result;
        }

        private int GetSpellBonus()
        {
            int atk = 0;
            if (Bot.SpellZone[5] != null)
                atk += 1200;

            List<ClientCard> monsters = Bot.GetGraveyardMonsters();
            if (monsters.Count != 0)
            {
                foreach (ClientCard card in Bot.GetSpells())
                {
                    if (card.IsCode(CardId.Solidarity))
                        atk += 800;
                }
            }

            return atk;
        }
    }
}
