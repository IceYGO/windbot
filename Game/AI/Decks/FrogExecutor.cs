using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot.Game;
using WindBot.Game.AI;

namespace DevBot.Game.AI.Decks
{
    [Deck("Frog", "AI_Frog")]
    public class FrogExecutor : DefaultExecutor
    {
        public enum CardId
        {
            CryomancerOfTheIceBarrier = 23950192,
            DewdarkOfTheIceBarrier = 90311614,
            SubmarineFrog = 63948258,
            SwapFrog = 9126351,
            FlipFlopFrog = 81278754,
            Unifrog = 56052205,
            Ronintoadin = 1357146,
            DupeFrog = 46239604,
            Tradetoad = 23408872,
            TreebornFrog = 12538374,
            DarkHole = 53129443,
            Raigeki = 12580477,
            Terraforming = 73628505,
            PotOfDuality = 98645731,
            Solidarity = 86780027,
            Wetlands = 2084239,
            FroggyForcefield = 34351849,
            GravityBind = 85742772,
            TheHugeRevolutionIsOver = 99188141
        }

        public FrogExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);

            AddExecutor(ExecutorType.Activate, (int)CardId.Solidarity, Solidarity);
            AddExecutor(ExecutorType.Activate, (int)CardId.Terraforming, Terraforming);
            AddExecutor(ExecutorType.Activate, (int)CardId.Wetlands, DefaultField);
            AddExecutor(ExecutorType.Activate, (int)CardId.DarkHole, DefaultDarkHole);
            AddExecutor(ExecutorType.Activate, (int)CardId.Raigeki, DefaultRaigeki);
            AddExecutor(ExecutorType.Activate, (int)CardId.PotOfDuality, PotOfDuality);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.SwapFrog, SwapFrogSummon);
            AddExecutor(ExecutorType.Activate, (int)CardId.SwapFrog, SwapFrogActivate);
            AddExecutor(ExecutorType.Activate, (int)CardId.DupeFrog, DupeFrog);
            AddExecutor(ExecutorType.Activate, (int)CardId.FlipFlopFrog, FlipFlopFrog);
            AddExecutor(ExecutorType.Activate, (int)CardId.Ronintoadin, Ronintoadin);
            AddExecutor(ExecutorType.Activate, (int)CardId.TreebornFrog);
            AddExecutor(ExecutorType.Activate, (int)CardId.Unifrog);

            AddExecutor(ExecutorType.Summon, (int)CardId.CryomancerOfTheIceBarrier, SummonFrog);
            AddExecutor(ExecutorType.Summon, (int)CardId.DewdarkOfTheIceBarrier, SummonFrog);
            AddExecutor(ExecutorType.Summon, (int)CardId.SubmarineFrog, SummonFrog);
            AddExecutor(ExecutorType.Summon, (int)CardId.SwapFrog, SummonFrog);
            AddExecutor(ExecutorType.Summon, (int)CardId.Unifrog, SummonFrog);
            AddExecutor(ExecutorType.Summon, (int)CardId.Ronintoadin, SummonFrog);
            AddExecutor(ExecutorType.Summon, (int)CardId.DupeFrog, SummonFrog);
            AddExecutor(ExecutorType.Summon, (int)CardId.Tradetoad, SummonFrog);
            AddExecutor(ExecutorType.Summon, (int)CardId.TreebornFrog, SummonFrog);
            AddExecutor(ExecutorType.Summon, (int)CardId.FlipFlopFrog, SummonFrog);

            AddExecutor(ExecutorType.MonsterSet, (int)CardId.FlipFlopFrog);
            AddExecutor(ExecutorType.MonsterSet, (int)CardId.DupeFrog);
            AddExecutor(ExecutorType.MonsterSet, (int)CardId.Tradetoad);
            AddExecutor(ExecutorType.MonsterSet, (int)CardId.Ronintoadin);
            AddExecutor(ExecutorType.MonsterSet, (int)CardId.TreebornFrog);
            AddExecutor(ExecutorType.MonsterSet, (int)CardId.Unifrog);
            AddExecutor(ExecutorType.MonsterSet, (int)CardId.SwapFrog);
            AddExecutor(ExecutorType.MonsterSet, (int)CardId.SubmarineFrog);
            AddExecutor(ExecutorType.MonsterSet, (int)CardId.DewdarkOfTheIceBarrier);
            AddExecutor(ExecutorType.MonsterSet, (int)CardId.CryomancerOfTheIceBarrier);

            AddExecutor(ExecutorType.Repos, FrogMonsterRepos);

            AddExecutor(ExecutorType.Activate, (int)CardId.FroggyForcefield, DefaultTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.TheHugeRevolutionIsOver, DefaultTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.GravityBind, GravityBind);
        }

        private int m_swapFrogSummoned;
        private int m_flipFlopFrogSummoned;

        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            if (attacker.Id == (int)CardId.Unifrog || attacker.Id == (int)CardId.DewdarkOfTheIceBarrier)
                return true;
            if (defender.IsMonsterInvincible() && !defender.IsMonsterDangerous() && attacker.Id == (int)CardId.SubmarineFrog)
                return true;
            return base.OnPreBattleBetween(attacker, defender);
        }

        private bool SwapFrogSummon()
        {
            int atk = Card.Attack + GetSpellBonus();
            if (AI.Utils.IsAllEnemyBetterThanValue(atk, true))
                return false;

            AI.SelectCard((int)CardId.Ronintoadin);
            m_swapFrogSummoned = Duel.Turn;
            return true;
        }

        private bool SwapFrogActivate()
        {
            if (m_swapFrogSummoned != Duel.Turn)
                return false;
            m_swapFrogSummoned = -1;

            if (Bot.GetRemainingCount((int)CardId.Ronintoadin, 2) == 0)
                return false;

            AI.SelectCard((int)CardId.Ronintoadin);
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
                monsters.Sort(AIFunctions.CompareCardAttack);
                monsters.Reverse();
                AI.SelectCard(monsters);
                return true;
            }
            return false;
        }

        private bool Ronintoadin()
        {
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

            if (AI.Utils.IsOneEnemyBetterThanValue(atk, true))
                return false;

            if (Card.Id == (int)CardId.SwapFrog)
                m_swapFrogSummoned = Duel.Turn;
            return true;
        }

        private bool PotOfDuality()
        {
            List<int> cards = new List<int>();
            
            if (AI.Utils.IsEnemyBetter(false, false))
            {
                cards.Add((int)CardId.FlipFlopFrog);
            }

            if (Bot.SpellZone[5] == null)
            {
                cards.Add((int)CardId.Terraforming);
                cards.Add((int)CardId.Wetlands);
            }

            cards.Add((int)CardId.DarkHole);
            cards.Add((int)CardId.SwapFrog);
            cards.Add((int)CardId.GravityBind);

            if (cards.Count > 0)
            {
                AI.SelectCard(cards);
                return true;
            }

            return false;
        }

        private bool DarkHole()
        {
            return AI.Utils.IsEnemyBetter(false, false);
        }

        private bool Terraforming()
        {
            if (Bot.HasInHand((int)CardId.Wetlands))
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
                if (spell.Id == (int)CardId.GravityBind && !spell.IsFacedown())
                    return false;
            }
            return true;
        }

        private bool FrogMonsterRepos()
        {
            if (Card.Id == (int)CardId.Unifrog)
                return Card.IsDefense();
            if (Card.Id == (int)CardId.DewdarkOfTheIceBarrier)
                return Card.IsDefense();

            bool enemyBetter = AI.Utils.IsOneEnemyBetterThanValue(Card.Attack + (Card.IsFacedown() ? GetSpellBonus() : 0), true);
            if (Card.Attack < 800)
                enemyBetter = true;
            bool result = false;
            if (Card.IsAttack() && enemyBetter)
                result =  true;
            if (Card.IsDefense() && !enemyBetter)
                result = true;

            if (!result && Card.Id == (int)CardId.FlipFlopFrog && Enemy.GetMonsterCount() > 0 && Card.IsFacedown())
                result = true;

            if (Card.Id == (int)CardId.FlipFlopFrog && Card.IsFacedown() && result)
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
                    if (card.Id == (int)CardId.Solidarity)
                        atk += 800;
                }
            }

            return atk;
        }
    }
}
