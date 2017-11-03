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
        public enum CardId
        {
            KrisTheCrackOfDawn = 81105204,
            SiroccoTheDawn = 75498415,
            ShuraTheBlueFlame = 58820853,
            BoraTheSpear = 49003716,
            KalutTheMoonShadow = 85215458,
            GaleTheWhirlwind = 2009101,
            BlizzardTheFarNorth = 22835145,
            MistralTheSilverShield = 46710683,
            Raigeki = 12580477,
            DarkHole = 53129443,
            MysticalSpaceTyphoon = 5318639,
            BlackWhirlwind = 91351370,
            MirrorForce = 44095762,
            DeltaCrowAntiReverse = 59839761,
            DimensionalPrison = 70342110,
            SilverwindTheAscendant = 33236860,
            BlackWingedDragon = 9012916,
            ArmorMaster = 69031175,
            ArmedWing = 76913983,
            GramTheShiningStar = 17377751
        }

        public BlackwingExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);

            AddExecutor(ExecutorType.Activate, (int)CardId.MysticalSpaceTyphoon, DefaultMysticalSpaceTyphoon);
            AddExecutor(ExecutorType.Activate, (int)CardId.DarkHole, DefaultDarkHole);
            AddExecutor(ExecutorType.Activate, (int)CardId.Raigeki, DefaultRaigeki);
            AddExecutor(ExecutorType.Activate, (int)CardId.BlackWhirlwind, BlackWhirlwindEffect);
            
            AddExecutor(ExecutorType.SpSummon, (int)CardId.KrisTheCrackOfDawn);
            AddExecutor(ExecutorType.SummonOrSet, (int)CardId.KrisTheCrackOfDawn);
            AddExecutor(ExecutorType.Summon, (int)CardId.SiroccoTheDawn, SiroccoTheDawnSummon);
            AddExecutor(ExecutorType.Summon, (int)CardId.ShuraTheBlueFlame, ShuraTheBlueFlameSummon);
            AddExecutor(ExecutorType.SummonOrSet, (int)CardId.ShuraTheBlueFlame);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.BoraTheSpear);
            AddExecutor(ExecutorType.SummonOrSet, (int)CardId.BoraTheSpear);
            AddExecutor(ExecutorType.SummonOrSet, (int)CardId.KalutTheMoonShadow, KalutTheMoonShadowSummon);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.GaleTheWhirlwind);
            AddExecutor(ExecutorType.SummonOrSet, (int)CardId.GaleTheWhirlwind);
            AddExecutor(ExecutorType.Summon, (int)CardId.BlizzardTheFarNorth, BlizzardTheFarNorthSummon);
            AddExecutor(ExecutorType.MonsterSet, (int)CardId.MistralTheSilverShield);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.SilverwindTheAscendant);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.ArmorMaster);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.GramTheShiningStar);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.ArmedWing);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.BlackWingedDragon);

            AddExecutor(ExecutorType.Activate, (int)CardId.MirrorForce, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.DimensionalPrison, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.DeltaCrowAntiReverse, DeltaCrowAntiReverseEffect);

            AddExecutor(ExecutorType.Activate, (int)CardId.BlizzardTheFarNorth);
            AddExecutor(ExecutorType.Activate, (int)CardId.ShuraTheBlueFlame);
            AddExecutor(ExecutorType.Activate, (int)CardId.BoraTheSpear, BoraTheSpearEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.KalutTheMoonShadow, AttackUpEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.SiroccoTheDawn, AttackUpEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.GaleTheWhirlwind, GaleTheWhirlwindEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.SilverwindTheAscendant);
            AddExecutor(ExecutorType.Activate, (int)CardId.BlackWingedDragon);
            AddExecutor(ExecutorType.Activate, (int)CardId.ArmorMaster);
            AddExecutor(ExecutorType.Activate, (int)CardId.ArmedWing);
            AddExecutor(ExecutorType.Activate, (int)CardId.GramTheShiningStar);

            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
        }

        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            if (defender.IsMonsterInvincible() && !defender.IsMonsterDangerous() && attacker.Id == 83104731)
                return true;
            return base.OnPreBattleBetween(attacker, defender);
        }

        private bool ShuraTheBlueFlameSummon()
        {
            if (Bot.HasInMonstersZone((int)CardId.SiroccoTheDawn) && Bot.GetMonsters().GetHighestAttackMonster().Attack < 3800)
                return true;
            return false;
        }

        private bool BlackWhirlwindEffect()
        {
            if (Card.Location == CardLocation.Hand && Bot.HasInSpellZone(Card.Id))
                return false;
            if (ActivateDescription == AI.Utils.GetStringId((int)Card.Id,0))
                AI.SelectCard((int)CardId.GaleTheWhirlwind);
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
                if (card != null && card.Id == (int)CardId.KrisTheCrackOfDawn || card.Id == (int)CardId.KalutTheMoonShadow || card.Id == (int)CardId.GaleTheWhirlwind || card.Id == (int)CardId.BoraTheSpear || card.Id == (int)CardId.SiroccoTheDawn || card.Id == (int)CardId.ShuraTheBlueFlame || card.Id == (int)CardId.BlizzardTheFarNorth)
                    return true;
            return false;
        }

        private bool KalutTheMoonShadowSummon()
        {
            foreach (ClientCard card in Bot.Hand)
                if (card != null && card.Id == (int)CardId.KrisTheCrackOfDawn || card.Id == (int)CardId.GaleTheWhirlwind || card.Id == (int)CardId.BoraTheSpear || card.Id == (int)CardId.SiroccoTheDawn || card.Id == (int)CardId.ShuraTheBlueFlame || card.Id == (int)CardId.BlizzardTheFarNorth)
                    return false;
            return true;
        }

        private bool BlizzardTheFarNorthSummon()
        {
            foreach (ClientCard card in Bot.Graveyard)
                if (card != null && card.Id == (int)CardId.KalutTheMoonShadow || card.Id == (int)CardId.BoraTheSpear || card.Id == (int)CardId.ShuraTheBlueFlame || card.Id == (int)CardId.KrisTheCrackOfDawn)
                    return true;
            return false;
        }

        private bool DeltaCrowAntiReverseEffect()
        {
            int Count = 0;

            List<ClientCard> monster = Bot.GetMonsters();
            foreach (ClientCard card in monster)
                if (card != null && card.Id == (int)CardId.KrisTheCrackOfDawn || card.Id == (int)CardId.KalutTheMoonShadow || card.Id == (int)CardId.GaleTheWhirlwind || card.Id == (int)CardId.BoraTheSpear || card.Id == (int)CardId.SiroccoTheDawn || card.Id == (int)CardId.ShuraTheBlueFlame || card.Id == (int)CardId.BlizzardTheFarNorth)
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