using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("Yosenju", "AI_Yosenju")]
    public class YosenjuExecutor : DefaultExecutor
    {
        public enum CardId
        {
            YosenjuKama1 = 65247798,
            YosenjuKama2 = 92246806,
            YosenjuKama3 = 28630501,
            YosenjuTsujik = 25244515,

            HarpiesFeatherDuster = 18144507,
            DarkHole = 53129443,
            CardOfDemise = 59750328,
            PotOfDuality = 98645731,
            CosmicCyclone = 8267140,
            QuakingMirrorForce = 40838625,
            DrowningMirrorForce = 47475363,
            StarlightRoad = 58120309,
            VanitysEmptiness = 5851097,
            MacroCosmos = 30241314,
            SolemnStrike = 40605147,
            SolemnWarning = 84749824,
            SolemnJudgment = 41420027,
            MagicDrain = 59344077,

            StardustDragon = 44508094,
            NumberS39UtopiatheLightning = 56832966,
            NumberS39UtopiaOne = 86532744,
            DarkRebellionXyzDragon = 16195942,
            Number39Utopia = 84013237,
            Number103Ragnazero = 94380860,
            BrotherhoodOfTheFireFistTigerKing = 96381979,
            Number106GiantHand = 63746411,
            CastelTheSkyblasterMusketeer = 82633039,
            DiamondDireWolf = 95169481,
            LightningChidori = 22653490,
            EvilswarmExcitonKnight = 46772449,
            AbyssDweller = 21044178,
            GagagaCowboy = 12014404
        }

        bool CardOfDemiseUsed = false;

        public YosenjuExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // do the end phase effect of Card Of Demise before Yosenjus return to hand
            AddExecutor(ExecutorType.Activate, (int)CardId.CardOfDemise, CardOfDemiseEPEffect);

            // burn if enemy's LP is below 800
            AddExecutor(ExecutorType.SpSummon, (int)CardId.GagagaCowboy, GagagaCowboySummon);
            AddExecutor(ExecutorType.Activate, (int)CardId.GagagaCowboy);

            AddExecutor(ExecutorType.Activate, (int)CardId.HarpiesFeatherDuster, DefaultHarpiesFeatherDusterFirst);
            AddExecutor(ExecutorType.Activate, (int)CardId.CosmicCyclone, DefaultCosmicCyclone);
            AddExecutor(ExecutorType.Activate, (int)CardId.HarpiesFeatherDuster);
            AddExecutor(ExecutorType.Activate, (int)CardId.DarkHole, DefaultDarkHole);

            AddExecutor(ExecutorType.Activate, (int)CardId.PotOfDuality, PotOfDualityEffect);

            AddExecutor(ExecutorType.Summon, (int)CardId.YosenjuKama1, HaveAnotherYosenjuWithSameNameInHand);
            AddExecutor(ExecutorType.Summon, (int)CardId.YosenjuKama2, HaveAnotherYosenjuWithSameNameInHand);
            AddExecutor(ExecutorType.Summon, (int)CardId.YosenjuKama3, HaveAnotherYosenjuWithSameNameInHand);
            AddExecutor(ExecutorType.Summon, (int)CardId.YosenjuKama1);
            AddExecutor(ExecutorType.Summon, (int)CardId.YosenjuKama2);
            AddExecutor(ExecutorType.Summon, (int)CardId.YosenjuKama3);
            AddExecutor(ExecutorType.Summon, (int)CardId.YosenjuTsujik);

            AddExecutor(ExecutorType.Activate, (int)CardId.YosenjuKama1, YosenjuEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.YosenjuKama2, YosenjuEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.YosenjuKama3, YosenjuEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.YosenjuTsujik, YosenjuEffect);

            AddExecutor(ExecutorType.SpellSet, (int)CardId.SolemnJudgment, TrapSetUnique);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.SolemnStrike, TrapSetUnique);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.SolemnWarning, TrapSetUnique);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.MacroCosmos, TrapSetUnique);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.VanitysEmptiness, TrapSetUnique);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.MagicDrain, TrapSetUnique);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.DrowningMirrorForce, TrapSetUnique);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.QuakingMirrorForce, TrapSetUnique);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.StarlightRoad, TrapSetUnique);

            AddExecutor(ExecutorType.SpellSet, (int)CardId.SolemnJudgment, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.SolemnStrike, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.SolemnWarning, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.MacroCosmos, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.VanitysEmptiness, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.MagicDrain, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.DrowningMirrorForce, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.QuakingMirrorForce, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.StarlightRoad, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.HarpiesFeatherDuster, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.DarkHole, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.PotOfDuality, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.CosmicCyclone, TrapSetWhenZoneFree);

            AddExecutor(ExecutorType.SpellSet, (int)CardId.CardOfDemise);
            AddExecutor(ExecutorType.Activate, (int)CardId.CardOfDemise, CardOfDemiseEffect);

            AddExecutor(ExecutorType.SpellSet, (int)CardId.SolemnJudgment, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.SolemnStrike, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.SolemnWarning, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.MacroCosmos, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.VanitysEmptiness, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.MagicDrain, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.DrowningMirrorForce, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.QuakingMirrorForce, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.StarlightRoad, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.HarpiesFeatherDuster, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.DarkHole, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.PotOfDuality, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.CosmicCyclone, CardOfDemiseAcivated);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.EvilswarmExcitonKnight, EvilswarmExcitonKnightSummon);
            AddExecutor(ExecutorType.Activate, (int)CardId.EvilswarmExcitonKnight, EvilswarmExcitonKnightEffect);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.DarkRebellionXyzDragon, DarkRebellionXyzDragonSummon);
            AddExecutor(ExecutorType.Activate, (int)CardId.DarkRebellionXyzDragon, DarkRebellionXyzDragonEffect);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.Number39Utopia, NumberS39UtopiatheLightningSummon);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.NumberS39UtopiaOne);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.NumberS39UtopiatheLightning);
            AddExecutor(ExecutorType.Activate, (int)CardId.NumberS39UtopiatheLightning);

            AddExecutor(ExecutorType.Activate, (int)CardId.StardustDragon, StardustDragonEffect);

            AddExecutor(ExecutorType.Activate, (int)CardId.StarlightRoad, DefaultTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.MagicDrain);
            AddExecutor(ExecutorType.Activate, (int)CardId.SolemnWarning, DefaultSolemnWarning);
            AddExecutor(ExecutorType.Activate, (int)CardId.SolemnStrike, DefaultSolemnStrike);
            AddExecutor(ExecutorType.Activate, (int)CardId.SolemnJudgment, DefaultSolemnJudgment);
            AddExecutor(ExecutorType.Activate, (int)CardId.MacroCosmos, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.VanitysEmptiness, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.DrowningMirrorForce, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.QuakingMirrorForce, DefaultUniqueTrap);

            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
        }

        public override bool OnSelectHand()
        {
            // go first
            return true;
        }

        public override void OnNewTurn()
        {
            CardOfDemiseUsed = false;
        }

        public override bool OnSelectYesNo(int desc)
        {
            // Yosenju Kama 2 shouldn't attack directly at most times
            if (Card == null)
                return true;
            // Logger.DebugWriteLine(Card.Name);
            if (Card.Id == (int)CardId.YosenjuKama2)
                return Card.ShouldDirectAttack;
            else
                return true;
        }

        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            if (defender.IsMonsterInvincible())
            {
                if (defender.IsMonsterDangerous() || defender.IsDefense())
                    return false;
            }
            if (!(defender.Id == (int)CardId.NumberS39UtopiatheLightning))
            {
                if (attacker.Attribute == (int)CardAttribute.Wind && Bot.HasInHand((int)CardId.YosenjuTsujik))
                    attacker.RealPower = attacker.RealPower + 1000;
                if (attacker.Id == (int)CardId.NumberS39UtopiatheLightning && !attacker.IsDisabled() && attacker.HasXyzMaterial(2, (int)CardId.Number39Utopia))
                    attacker.RealPower = 5000;
            }
            return attacker.RealPower > defender.GetDefensePower();
        }

        private bool PotOfDualityEffect()
        {
            if (CardOfDemiseUsed)
            {
                AI.SelectCard(new[]
                    {
                    (int)CardId.StarlightRoad,
                    (int)CardId.MagicDrain,
                    (int)CardId.SolemnJudgment,
                    (int)CardId.VanitysEmptiness,
                    (int)CardId.HarpiesFeatherDuster,
                    (int)CardId.DrowningMirrorForce,
                    (int)CardId.QuakingMirrorForce,
                    (int)CardId.SolemnStrike,
                    (int)CardId.SolemnWarning,
                    (int)CardId.MacroCosmos,
                    (int)CardId.CardOfDemise
                });
            }
            else
            {
                AI.SelectCard(new[]
                    {
                    (int)CardId.YosenjuKama3,
                    (int)CardId.YosenjuKama1,
                    (int)CardId.YosenjuKama2,
                    (int)CardId.StarlightRoad,
                    (int)CardId.MagicDrain,
                    (int)CardId.VanitysEmptiness,
                    (int)CardId.HarpiesFeatherDuster,
                    (int)CardId.DrowningMirrorForce,
                    (int)CardId.QuakingMirrorForce,
                    (int)CardId.SolemnStrike,
                    (int)CardId.SolemnJudgment,
                    (int)CardId.SolemnWarning,
                    (int)CardId.MacroCosmos,
                    (int)CardId.CardOfDemise,
                });
            }
            return true;
        }

        private bool CardOfDemiseEffect()
        {
            if (AI.Utils.IsTurn1OrMain2())
            {
                CardOfDemiseUsed = true;
                return true;
            }
            return false;
        }

        private bool HaveAnotherYosenjuWithSameNameInHand()
        {
            foreach (ClientCard card in Bot.Hand)
            {
                if (card != null && !card.Equals(Card) && card.Id == Card.Id)
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

        private bool YosenjuEffect()
        {
            // Don't activate the return to hand effect first
            if (Duel.Phase == DuelPhase.End)
                return false;
            AI.SelectCard(new[]
                {
                    (int)CardId.YosenjuKama1,
                    (int)CardId.YosenjuKama2,
                    (int)CardId.YosenjuKama3
                });
            return true;
        }

        private bool CardOfDemiseEPEffect()
        {
            // do the end phase effect of Card Of Demise before Yosenjus return to hand
            return Duel.Phase == DuelPhase.End;
        }

        private bool GagagaCowboySummon()
        {
            if (Duel.LifePoints[1] <= 800 || (Bot.GetMonsterCount()>=4 && Duel.LifePoints[1] <= 1600))
            {
                AI.SelectPosition(CardPosition.FaceUpDefence);
                return true;
            }
            return false;
        }

        private bool EvilswarmExcitonKnightSummon()
        {
            int selfCount = Bot.GetMonsterCount() + Bot.GetSpellCount() + Bot.GetHandCount();
            int oppoCount = Enemy.GetMonsterCount() + Enemy.GetSpellCount() + Enemy.GetHandCount();
            return (selfCount - 1 < oppoCount) && EvilswarmExcitonKnightEffect();
        }

        private bool EvilswarmExcitonKnightEffect()
        {
            int selfCount = Bot.GetMonsterCount() + Bot.GetSpellCount();
            int oppoCount = Enemy.GetMonsterCount() + Enemy.GetSpellCount();
            return selfCount < oppoCount;
        }

        private bool DarkRebellionXyzDragonSummon()
        {
            int selfBestAttack = AI.Utils.GetBestAttack(Bot);
            int oppoBestAttack = AI.Utils.GetBestAttack(Enemy);
            return selfBestAttack <= oppoBestAttack;
        }

        private bool DarkRebellionXyzDragonEffect()
        {
            int oppoBestAttack = AI.Utils.GetBestAttack(Enemy);
            ClientCard target = AI.Utils.GetOneEnemyBetterThanValue(oppoBestAttack, true);
            if (target != null)
            {
                AI.SelectNextCard(target);
            }
            return true;
        }

        private bool NumberS39UtopiatheLightningSummon()
        {
            int selfBestAttack = AI.Utils.GetBestAttack(Bot);
            int oppoBestAttack = AI.Utils.GetBestPower(Enemy);
            return selfBestAttack <= oppoBestAttack;
        }

        private bool StardustDragonEffect()
        {
            return LastChainPlayer == 1;
        }

    }
}