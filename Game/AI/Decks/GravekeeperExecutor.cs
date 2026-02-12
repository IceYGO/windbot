using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    // NOT FINISHED YET
    [Deck("Gravekeeper", "AI_Gravekeeper", "NotFinished")]
    public class GravekeeperExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int GravekeepersOracle = 25524823;
            public const int MaleficStardustDragon = 36521459;
            public const int GravekeepersVisionary = 3825890;
            public const int GravekeepersChief = 62473983;
            public const int ThunderKingRaiOh = 71564252;
            public const int GravekeepersCommandant = 17393207;
            public const int GravekeepersAssailant = 25262697;
            public const int GravekeepersDescendant = 30213599;
            public const int GravekeepersSpy = 24317029;
            public const int GravekeepersRecruiter = 93023479;
            public const int AllureOfDarkness = 1475311;
            public const int DarkHole = 53129443;
            public const int RoyalTribute = 72405967;
            public const int GravekeepersStele = 99523325;
            public const int MysticalSpaceTyphoon = 5318639;
            public const int BookofMoon = 14087893;
            public const int HiddenTemplesOfNecrovalley = 70000776;
            public const int Necrovalley = 47355498;
            public const int BottomlessTrapHole = 29401950;
            public const int RiteOfSpirit = 30450531;
            public const int TorrentialTribute = 53582587;
            public const int DimensionalPrison = 70342110;
            public const int SolemnWarning = 84749824;
            public const int ImperialTombsOfNecrovalley = 90434657;
        }

        public GravekeeperExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);

            AddExecutor(ExecutorType.Activate, CardId.AllureOfDarkness);
            AddExecutor(ExecutorType.Activate, CardId.DarkHole, DefaultDarkHole);
            AddExecutor(ExecutorType.Activate, CardId.RoyalTribute);
            AddExecutor(ExecutorType.Activate, CardId.GravekeepersStele);
            AddExecutor(ExecutorType.Activate, CardId.MysticalSpaceTyphoon, DefaultMysticalSpaceTyphoon);
            AddExecutor(ExecutorType.Activate, CardId.BookofMoon, DefaultBookOfMoon);
            AddExecutor(ExecutorType.Activate, CardId.HiddenTemplesOfNecrovalley, HiddenTemplesOfNecrovalleyEffect);
            AddExecutor(ExecutorType.Activate, CardId.Necrovalley, NecrovalleyActivate);

            AddExecutor(ExecutorType.Activate, CardId.BottomlessTrapHole, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, CardId.SolemnWarning, DefaultSolemnWarning);
            AddExecutor(ExecutorType.Activate, CardId.DimensionalPrison, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, CardId.RiteOfSpirit, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, CardId.ImperialTombsOfNecrovalley, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, CardId.TorrentialTribute, DefaultTorrentialTribute);

            AddExecutor(ExecutorType.Summon, CardId.GravekeepersOracle);
            AddExecutor(ExecutorType.SpSummon, CardId.MaleficStardustDragon, MaleficStardustDragonSummon);
            AddExecutor(ExecutorType.Summon, CardId.GravekeepersVisionary);
            AddExecutor(ExecutorType.Summon, CardId.GravekeepersChief);
            AddExecutor(ExecutorType.Summon, CardId.ThunderKingRaiOh);
            AddExecutor(ExecutorType.Summon, CardId.GravekeepersCommandant, GravekeepersCommandantSummon);
            AddExecutor(ExecutorType.Summon, CardId.GravekeepersAssailant);
            AddExecutor(ExecutorType.Summon, CardId.GravekeepersDescendant);
            AddExecutor(ExecutorType.MonsterSet, CardId.GravekeepersSpy);
            AddExecutor(ExecutorType.MonsterSet, CardId.GravekeepersRecruiter);

            AddExecutor(ExecutorType.Activate, CardId.GravekeepersOracle);
            AddExecutor(ExecutorType.Activate, CardId.GravekeepersVisionary);
            AddExecutor(ExecutorType.Activate, CardId.GravekeepersChief);
            AddExecutor(ExecutorType.Activate, CardId.GravekeepersCommandant, GravekeepersCommandantEffect);
            AddExecutor(ExecutorType.Activate, CardId.GravekeepersAssailant, GravekeepersAssailantEffect);
            AddExecutor(ExecutorType.Activate, CardId.GravekeepersDescendant, GravekeepersDescendantEffect);
            AddExecutor(ExecutorType.Activate, CardId.GravekeepersSpy, SearchForDescendant);
            AddExecutor(ExecutorType.Activate, CardId.GravekeepersRecruiter, SearchForDescendant);

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
            if (!Bot.HasInHand(CardId.Necrovalley) && !Bot.HasInSpellZone(CardId.Necrovalley))
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
            if (Util.IsOneEnemyBetterThanValue(bestatk, true))
            {
                AI.SelectCard(Enemy.GetMonsters().GetHighestAttackMonster());
                return true;
            }
            return false;
        }

        private bool SearchForDescendant()
        {
            AI.SelectCard(CardId.GravekeepersDescendant);
            return true;
        }
    }
}