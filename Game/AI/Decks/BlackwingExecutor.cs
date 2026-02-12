using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    // NOT FINISHED YET
    [Deck("Blackwing", "AI_Blackwing", "NotFinished")]
    public class BlackwingExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int KrisTheCrackOfDawn = 81105204;
            public const int SiroccoTheDawn = 75498415;
            public const int ShuraTheBlueFlame = 58820853;
            public const int BoraTheSpear = 49003716;
            public const int KalutTheMoonShadow = 85215458;
            public const int GaleTheWhirlwind = 2009101;
            public const int BlizzardTheFarNorth = 22835145;
            public const int MistralTheSilverShield = 46710683;
            public const int Raigeki = 12580477;
            public const int DarkHole = 53129443;
            public const int MysticalSpaceTyphoon = 5318639;
            public const int BlackWhirlwind = 91351370;
            public const int MirrorForce = 44095762;
            public const int DeltaCrowAntiReverse = 59839761;
            public const int DimensionalPrison = 70342110;
            public const int SilverwindTheAscendant = 33236860;
            public const int BlackWingedDragon = 9012916;
            public const int ArmorMaster = 69031175;
            public const int ArmedWing = 76913983;
            public const int GramTheShiningStar = 17377751;
        }

        public BlackwingExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);

            AddExecutor(ExecutorType.Activate, CardId.MysticalSpaceTyphoon, DefaultMysticalSpaceTyphoon);
            AddExecutor(ExecutorType.Activate, CardId.DarkHole, DefaultDarkHole);
            AddExecutor(ExecutorType.Activate, CardId.Raigeki, DefaultRaigeki);
            AddExecutor(ExecutorType.Activate, CardId.BlackWhirlwind, BlackWhirlwindEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.KrisTheCrackOfDawn);
            AddExecutor(ExecutorType.SummonOrSet, CardId.KrisTheCrackOfDawn);
            AddExecutor(ExecutorType.Summon, CardId.SiroccoTheDawn, SiroccoTheDawnSummon);
            AddExecutor(ExecutorType.Summon, CardId.ShuraTheBlueFlame, ShuraTheBlueFlameSummon);
            AddExecutor(ExecutorType.SummonOrSet, CardId.ShuraTheBlueFlame);
            AddExecutor(ExecutorType.SpSummon, CardId.BoraTheSpear);
            AddExecutor(ExecutorType.SummonOrSet, CardId.BoraTheSpear);
            AddExecutor(ExecutorType.SummonOrSet, CardId.KalutTheMoonShadow, KalutTheMoonShadowSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.GaleTheWhirlwind);
            AddExecutor(ExecutorType.SummonOrSet, CardId.GaleTheWhirlwind);
            AddExecutor(ExecutorType.Summon, CardId.BlizzardTheFarNorth, BlizzardTheFarNorthSummon);
            AddExecutor(ExecutorType.MonsterSet, CardId.MistralTheSilverShield);

            AddExecutor(ExecutorType.SpSummon, CardId.SilverwindTheAscendant);
            AddExecutor(ExecutorType.SpSummon, CardId.ArmorMaster);
            AddExecutor(ExecutorType.SpSummon, CardId.GramTheShiningStar);
            AddExecutor(ExecutorType.SpSummon, CardId.ArmedWing);
            AddExecutor(ExecutorType.SpSummon, CardId.BlackWingedDragon);

            AddExecutor(ExecutorType.Activate, CardId.MirrorForce, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, CardId.DimensionalPrison, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, CardId.DeltaCrowAntiReverse, DeltaCrowAntiReverseEffect);

            AddExecutor(ExecutorType.Activate, CardId.BlizzardTheFarNorth);
            AddExecutor(ExecutorType.Activate, CardId.ShuraTheBlueFlame);
            AddExecutor(ExecutorType.Activate, CardId.BoraTheSpear, BoraTheSpearEffect);
            AddExecutor(ExecutorType.Activate, CardId.KalutTheMoonShadow, AttackUpEffect);
            AddExecutor(ExecutorType.Activate, CardId.SiroccoTheDawn, AttackUpEffect);
            AddExecutor(ExecutorType.Activate, CardId.GaleTheWhirlwind, GaleTheWhirlwindEffect);
            AddExecutor(ExecutorType.Activate, CardId.SilverwindTheAscendant);
            AddExecutor(ExecutorType.Activate, CardId.BlackWingedDragon);
            AddExecutor(ExecutorType.Activate, CardId.ArmorMaster);
            AddExecutor(ExecutorType.Activate, CardId.ArmedWing);
            AddExecutor(ExecutorType.Activate, CardId.GramTheShiningStar);

            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
        }

        private bool ShuraTheBlueFlameSummon()
        {
            if (Bot.HasInMonstersZone(CardId.SiroccoTheDawn) && Bot.GetMonsters().GetHighestAttackMonster().Attack < 3800)
                return true;
            return false;
        }

        private bool BlackWhirlwindEffect()
        {
            if (Card.Location == CardLocation.Hand && Bot.HasInSpellZone(Card.Id))
                return false;
            if (ActivateDescription == Util.GetStringId((int)Card.Id,0))
                AI.SelectCard(CardId.GaleTheWhirlwind);
            return true;
        }

        private bool SiroccoTheDawnSummon()
        {
            int OpponentMonster = Enemy.GetMonsterCount();
            int AIMonster = Bot.GetMonsterCount();
            if (OpponentMonster != 0 && AIMonster == 0)
                return true;
            return false;
        }

        private bool BoraTheSpearEffect()
        {
            List<ClientCard> monster = Bot.GetMonsters();
            foreach (ClientCard card in monster)
                if (card != null && card.IsCode(CardId.KrisTheCrackOfDawn, CardId.KalutTheMoonShadow, CardId.GaleTheWhirlwind, CardId.BoraTheSpear, CardId.SiroccoTheDawn, CardId.ShuraTheBlueFlame, CardId.BlizzardTheFarNorth))
                    return true;
            return false;
        }

        private bool KalutTheMoonShadowSummon()
        {
            foreach (ClientCard card in Bot.Hand)
                if (card != null && card.IsCode(CardId.KrisTheCrackOfDawn, CardId.GaleTheWhirlwind, CardId.BoraTheSpear, CardId.SiroccoTheDawn, CardId.ShuraTheBlueFlame, CardId.BlizzardTheFarNorth))
                    return false;
            return true;
        }

        private bool BlizzardTheFarNorthSummon()
        {
            foreach (ClientCard card in Bot.Graveyard)
                if (card != null && card.IsCode(CardId.KalutTheMoonShadow, CardId.BoraTheSpear, CardId.ShuraTheBlueFlame, CardId.KrisTheCrackOfDawn))
                    return true;
            return false;
        }

        private bool DeltaCrowAntiReverseEffect()
        {
            int Count = 0;

            List<ClientCard> monster = Bot.GetMonsters();
            foreach (ClientCard card in monster)
                if (card != null && card.IsCode(CardId.KrisTheCrackOfDawn, CardId.KalutTheMoonShadow, CardId.GaleTheWhirlwind, CardId.BoraTheSpear, CardId.SiroccoTheDawn, CardId.ShuraTheBlueFlame, CardId.BlizzardTheFarNorth))
                    Count++;

            if (Count == 3)
                return true;
            return false;
        }

        private bool GaleTheWhirlwindEffect()
        {
            if (Card.Position == (int)CardPosition.FaceUp)
            {
                AI.SelectCard(Enemy.GetMonsters().GetHighestAttackMonster());
                return true;
            }
            return false;
        }

        private bool AttackUpEffect()
        {
            ClientCard bestMy = Bot.GetMonsters().GetHighestAttackMonster();
            ClientCard bestEnemyATK = Enemy.GetMonsters().GetHighestAttackMonster();
            ClientCard bestEnemyDEF = Enemy.GetMonsters().GetHighestDefenseMonster();
            if (bestMy == null || (bestEnemyATK == null && bestEnemyDEF == null))
                return false;
            if (bestEnemyATK != null && bestMy.Attack < bestEnemyATK.Attack)
                return true;
            if (bestEnemyDEF != null && bestMy.Attack < bestEnemyDEF.Defense)
                return true;
            return false;
        }
    }
}