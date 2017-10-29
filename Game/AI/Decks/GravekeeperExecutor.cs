using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    // NOT FINISHED YET
    [Deck("Gravekeeper", "AI_Gravekeeper")]
    public class GravekeeperExecutor : DefaultExecutor
    {
        public enum CardId
        {
            GravekeepersOracle = 25524823,
            MaleficStardustDragon = 36521459,
            GravekeepersVisionary = 3825890,
            GravekeepersChief = 62473983,
            ThunderKingRaiOh = 71564252,
            GravekeepersCommandant = 17393207,
            GravekeepersAssailant = 25262697,
            GravekeepersDescendant = 30213599,
            GravekeepersSpy = 24317029,
            GravekeepersRecruiter = 93023479,
            AllureOfDarkness = 1475311,
            DarkHole = 53129443,
            RoyalTribute = 72405967,
            GravekeepersStele = 99523325,
            MysticalSpaceTyphoon = 5318639,
            BookofMoon = 14087893,
            HiddenTemplesOfNecrovalley = 70000776,
            Necrovalley = 47355498,
            BottomlessTrapHole = 29401950,
            RiteOfSpirit = 30450531,
            TorrentialTribute = 53582587,
            DimensionalPrison = 70342110,
            SolemnWarning = 84749824,
            ImperialTombsOfNecrovalley = 90434657
        }

        public GravekeeperExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);

            AddExecutor(ExecutorType.Activate, (int)CardId.AllureOfDarkness);
            AddExecutor(ExecutorType.Activate, (int)CardId.DarkHole, DefaultDarkHole);
            AddExecutor(ExecutorType.Activate, (int)CardId.RoyalTribute);
            AddExecutor(ExecutorType.Activate, (int)CardId.GravekeepersStele);
            AddExecutor(ExecutorType.Activate, (int)CardId.MysticalSpaceTyphoon, DefaultMysticalSpaceTyphoon);
            AddExecutor(ExecutorType.Activate, (int)CardId.BookofMoon, DefaultBookOfMoon);
            AddExecutor(ExecutorType.Activate, (int)CardId.HiddenTemplesOfNecrovalley, HiddenTemplesOfNecrovalleyEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.Necrovalley, NecrovalleyActivate);

            AddExecutor(ExecutorType.Activate, (int)CardId.BottomlessTrapHole, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.SolemnWarning, DefaultSolemnWarning);
            AddExecutor(ExecutorType.Activate, (int)CardId.DimensionalPrison, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.RiteOfSpirit, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.ImperialTombsOfNecrovalley, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.TorrentialTribute, DefaultTorrentialTribute);

            AddExecutor(ExecutorType.Summon, (int)CardId.GravekeepersOracle);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.MaleficStardustDragon, MaleficStardustDragonSummon);
            AddExecutor(ExecutorType.Summon, (int)CardId.GravekeepersVisionary);
            AddExecutor(ExecutorType.Summon, (int)CardId.GravekeepersChief);
            AddExecutor(ExecutorType.Summon, (int)CardId.ThunderKingRaiOh);
            AddExecutor(ExecutorType.Summon, (int)CardId.GravekeepersCommandant, GravekeepersCommandantSummon);
            AddExecutor(ExecutorType.Summon, (int)CardId.GravekeepersAssailant);
            AddExecutor(ExecutorType.Summon, (int)CardId.GravekeepersDescendant);
            AddExecutor(ExecutorType.MonsterSet, (int)CardId.GravekeepersSpy);
            AddExecutor(ExecutorType.MonsterSet, (int)CardId.GravekeepersRecruiter);

            AddExecutor(ExecutorType.Activate, (int)CardId.GravekeepersOracle);
            AddExecutor(ExecutorType.Activate, (int)CardId.GravekeepersVisionary);
            AddExecutor(ExecutorType.Activate, (int)CardId.GravekeepersChief);
            AddExecutor(ExecutorType.Activate, (int)CardId.GravekeepersCommandant, GravekeepersCommandantEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.GravekeepersAssailant, GravekeepersAssailantEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.GravekeepersDescendant, GravekeepersDescendantEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.GravekeepersSpy, SearchForDescendant);
            AddExecutor(ExecutorType.Activate, (int)CardId.GravekeepersRecruiter, SearchForDescendant);

            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
        }

        private bool HiddenTemplesOfNecrovalleyEffect()
        {
            if (Card.Location == CardLocation.Hand && Bot.HasInSpellZone((int)Card.Id))
                return false;
            return true;
        }

        private bool NecrovalleyActivate()
        {
            if (Bot.SpellZone[5] != null)
                return false;
            return true;
        }

        private bool MaleficStardustDragonSummon()
        {
            if (Bot.SpellZone[5] != null)
                return true;
            return false;
        }

        private bool GravekeepersCommandantEffect()
        {
            if (!Bot.HasInHand((int)CardId.Necrovalley) && !Bot.HasInSpellZone((int)CardId.Necrovalley))
                return true;
            return false;
        }

        private bool GravekeepersCommandantSummon()
        {
            return !GravekeepersCommandantEffect();
        }

        private bool GravekeepersAssailantEffect()
        {
            if (!Card.IsAttack())
                return false;
            foreach (ClientCard card in Enemy.GetMonsters())
            {
                if (card.IsDefense() && card.Defense > 1500 && card.Attack < 1500 || card.Attack > 1500 && card.Defense < 1500)
                    return true;
            }
            return false;
        }

        private bool GravekeepersDescendantEffect()
        {
            int bestatk = Bot.GetMonsters().GetHighestAttackMonster().Attack;
            if (AI.Utils.IsOneEnemyBetterThanValue(bestatk, true))
            {
                AI.SelectCard(Enemy.GetMonsters().GetHighestAttackMonster());
                return true;
            }
            return false;
        }

        private bool SearchForDescendant()
        {
            AI.SelectCard((int)CardId.GravekeepersDescendant);
            return true;
        }
    }
}