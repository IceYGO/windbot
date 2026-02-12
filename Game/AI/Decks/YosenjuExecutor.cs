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
        public class CardId
        {
            public const int YosenjuKama1 = 65247798;
            public const int YosenjuKama2 = 92246806;
            public const int YosenjuKama3 = 28630501;
            public const int YosenjuTsujik = 25244515;

            public const int HarpiesFeatherDuster = 18144507;
            public const int DarkHole = 53129443;
            public const int CardOfDemise = 59750328;
            public const int PotOfDuality = 98645731;
            public const int CosmicCyclone = 8267140;
            public const int QuakingMirrorForce = 40838625;
            public const int DrowningMirrorForce = 47475363;
            public const int StarlightRoad = 58120309;
            public const int VanitysEmptiness = 5851097;
            public const int MacroCosmos = 30241314;
            public const int SolemnStrike = 40605147;
            public const int SolemnWarning = 84749824;
            public const int SolemnJudgment = 41420027;
            public const int MagicDrain = 59344077;

            public const int StardustDragon = 44508094;
            public const int NumberS39UtopiatheLightning = 56832966;
            public const int NumberS39UtopiaOne = 86532744;
            public const int DarkRebellionXyzDragon = 16195942;
            public const int Number39Utopia = 84013237;
            public const int Number103Ragnazero = 94380860;
            public const int BrotherhoodOfTheFireFistTigerKing = 96381979;
            public const int Number106GiantHand = 63746411;
            public const int CastelTheSkyblasterMusketeer = 82633039;
            public const int DiamondDireWolf = 95169481;
            public const int LightningChidori = 22653490;
            public const int EvilswarmExcitonKnight = 46772449;
            public const int AbyssDweller = 21044178;
            public const int GagagaCowboy = 12014404;
        }

        bool CardOfDemiseUsed = false;

        public YosenjuExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // do the end phase effect of Card Of Demise before Yosenjus return to hand
            AddExecutor(ExecutorType.Activate, CardId.CardOfDemise, CardOfDemiseEPEffect);

            // burn if enemy's LP is below 800
            AddExecutor(ExecutorType.SpSummon, CardId.GagagaCowboy, GagagaCowboySummon);
            AddExecutor(ExecutorType.Activate, CardId.GagagaCowboy);

            AddExecutor(ExecutorType.Activate, CardId.HarpiesFeatherDuster, DefaultHarpiesFeatherDusterFirst);
            AddExecutor(ExecutorType.Activate, CardId.CosmicCyclone, DefaultCosmicCyclone);
            AddExecutor(ExecutorType.Activate, CardId.HarpiesFeatherDuster);
            AddExecutor(ExecutorType.Activate, CardId.DarkHole, DefaultDarkHole);

            AddExecutor(ExecutorType.Activate, CardId.PotOfDuality, PotOfDualityEffect);

            AddExecutor(ExecutorType.Summon, CardId.YosenjuKama1, HaveAnotherYosenjuWithSameNameInHand);
            AddExecutor(ExecutorType.Summon, CardId.YosenjuKama2, HaveAnotherYosenjuWithSameNameInHand);
            AddExecutor(ExecutorType.Summon, CardId.YosenjuKama3, HaveAnotherYosenjuWithSameNameInHand);
            AddExecutor(ExecutorType.Summon, CardId.YosenjuKama1);
            AddExecutor(ExecutorType.Summon, CardId.YosenjuKama2);
            AddExecutor(ExecutorType.Summon, CardId.YosenjuKama3);
            AddExecutor(ExecutorType.Summon, CardId.YosenjuTsujik);

            AddExecutor(ExecutorType.Activate, CardId.YosenjuKama1, YosenjuEffect);
            AddExecutor(ExecutorType.Activate, CardId.YosenjuKama2, YosenjuEffect);
            AddExecutor(ExecutorType.Activate, CardId.YosenjuKama3, YosenjuEffect);
            AddExecutor(ExecutorType.Activate, CardId.YosenjuTsujik, YosenjuEffect);

            AddExecutor(ExecutorType.SpellSet, CardId.SolemnJudgment, TrapSetUnique);
            AddExecutor(ExecutorType.SpellSet, CardId.SolemnStrike, TrapSetUnique);
            AddExecutor(ExecutorType.SpellSet, CardId.SolemnWarning, TrapSetUnique);
            AddExecutor(ExecutorType.SpellSet, CardId.MacroCosmos, TrapSetUnique);
            AddExecutor(ExecutorType.SpellSet, CardId.VanitysEmptiness, TrapSetUnique);
            AddExecutor(ExecutorType.SpellSet, CardId.MagicDrain, TrapSetUnique);
            AddExecutor(ExecutorType.SpellSet, CardId.DrowningMirrorForce, TrapSetUnique);
            AddExecutor(ExecutorType.SpellSet, CardId.QuakingMirrorForce, TrapSetUnique);
            AddExecutor(ExecutorType.SpellSet, CardId.StarlightRoad, TrapSetUnique);

            AddExecutor(ExecutorType.SpellSet, CardId.SolemnJudgment, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, CardId.SolemnStrike, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, CardId.SolemnWarning, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, CardId.MacroCosmos, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, CardId.VanitysEmptiness, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, CardId.MagicDrain, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, CardId.DrowningMirrorForce, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, CardId.QuakingMirrorForce, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, CardId.StarlightRoad, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, CardId.HarpiesFeatherDuster, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, CardId.DarkHole, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, CardId.PotOfDuality, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, CardId.CosmicCyclone, TrapSetWhenZoneFree);

            AddExecutor(ExecutorType.SpellSet, CardId.CardOfDemise);
            AddExecutor(ExecutorType.Activate, CardId.CardOfDemise, CardOfDemiseEffect);

            AddExecutor(ExecutorType.SpellSet, CardId.SolemnJudgment, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, CardId.SolemnStrike, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, CardId.SolemnWarning, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, CardId.MacroCosmos, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, CardId.VanitysEmptiness, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, CardId.MagicDrain, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, CardId.DrowningMirrorForce, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, CardId.QuakingMirrorForce, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, CardId.StarlightRoad, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, CardId.HarpiesFeatherDuster, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, CardId.DarkHole, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, CardId.PotOfDuality, CardOfDemiseAcivated);
            AddExecutor(ExecutorType.SpellSet, CardId.CosmicCyclone, CardOfDemiseAcivated);

            AddExecutor(ExecutorType.SpSummon, CardId.EvilswarmExcitonKnight, DefaultEvilswarmExcitonKnightSummon);
            AddExecutor(ExecutorType.Activate, CardId.EvilswarmExcitonKnight, DefaultEvilswarmExcitonKnightEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.DarkRebellionXyzDragon, DarkRebellionXyzDragonSummon);
            AddExecutor(ExecutorType.Activate, CardId.DarkRebellionXyzDragon, DarkRebellionXyzDragonEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.Number39Utopia, DefaultNumberS39UtopiaTheLightningSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.NumberS39UtopiaOne);
            AddExecutor(ExecutorType.SpSummon, CardId.NumberS39UtopiatheLightning);
            AddExecutor(ExecutorType.Activate, CardId.NumberS39UtopiatheLightning, DefaultNumberS39UtopiaTheLightningEffect);

            AddExecutor(ExecutorType.Activate, CardId.StardustDragon, DefaultStardustDragonEffect);

            AddExecutor(ExecutorType.Activate, CardId.StarlightRoad, DefaultTrap);
            AddExecutor(ExecutorType.Activate, CardId.MagicDrain);
            AddExecutor(ExecutorType.Activate, CardId.SolemnWarning, DefaultSolemnWarning);
            AddExecutor(ExecutorType.Activate, CardId.SolemnStrike, DefaultSolemnStrike);
            AddExecutor(ExecutorType.Activate, CardId.SolemnJudgment, DefaultSolemnJudgment);
            AddExecutor(ExecutorType.Activate, CardId.MacroCosmos, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, CardId.VanitysEmptiness, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, CardId.DrowningMirrorForce, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, CardId.QuakingMirrorForce, DefaultUniqueTrap);

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
            base.OnNewTurn();
        }

        public override bool OnSelectYesNo(int desc)
        {
            // Yosenju Kama 2 shouldn't attack directly at most times
            if (Card == null)
                return true;
            // Logger.DebugWriteLine(Card.Name);
            if (Card.IsCode(CardId.YosenjuKama2))
                return Card.ShouldDirectAttack;
            else
                return true;
        }

        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            if (!defender.IsMonsterHasPreventActivationEffectInBattle())
            {
                if (attacker.Attribute == (int)CardAttribute.Wind && Bot.HasInHand(CardId.YosenjuTsujik))
                    attacker.RealPower = attacker.RealPower + 1000;
            }
            return base.OnPreBattleBetween(attacker, defender);
        }

        public override IList<ClientCard> OnSelectXyzMaterial(IList<ClientCard> cards, int min, int max)
        {
            IList<ClientCard> result = Util.SelectPreferredCards(CardId.YosenjuTsujik, cards, min, max);
            return Util.CheckSelectCount(result, cards, min, max);
        }

        private bool PotOfDualityEffect()
        {
            if (CardOfDemiseUsed)
            {
                AI.SelectCard(
                    CardId.StarlightRoad,
                    CardId.MagicDrain,
                    CardId.SolemnJudgment,
                    CardId.VanitysEmptiness,
                    CardId.HarpiesFeatherDuster,
                    CardId.DrowningMirrorForce,
                    CardId.QuakingMirrorForce,
                    CardId.SolemnStrike,
                    CardId.SolemnWarning,
                    CardId.MacroCosmos,
                    CardId.CardOfDemise
                    );
            }
            else
            {
                AI.SelectCard(
                    CardId.YosenjuKama3,
                    CardId.YosenjuKama1,
                    CardId.YosenjuKama2,
                    CardId.StarlightRoad,
                    CardId.MagicDrain,
                    CardId.VanitysEmptiness,
                    CardId.HarpiesFeatherDuster,
                    CardId.DrowningMirrorForce,
                    CardId.QuakingMirrorForce,
                    CardId.SolemnStrike,
                    CardId.SolemnJudgment,
                    CardId.SolemnWarning,
                    CardId.MacroCosmos,
                    CardId.CardOfDemise
                    );
            }
            return true;
        }

        private bool CardOfDemiseEffect()
        {
            if (Util.IsTurn1OrMain2())
            {
                CardOfDemiseUsed = true;
                return true;
            }
            return false;
        }

        private bool HaveAnotherYosenjuWithSameNameInHand()
        {
            foreach (ClientCard card in Bot.Hand.GetMonsters())
            {
                if (!card.Equals(Card) && card.IsCode(Card.Id))
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

        private bool YosenjuEffect()
        {
            // Don't activate the return to hand effect first
            if (Duel.Phase == DuelPhase.End)
                return false;
            AI.SelectCard(CardId.YosenjuKama1, CardId.YosenjuKama2, CardId.YosenjuKama3);
            return true;
        }

        private bool CardOfDemiseEPEffect()
        {
            // do the end phase effect of Card Of Demise before Yosenjus return to hand
            return Duel.Phase == DuelPhase.End;
        }

        private bool GagagaCowboySummon()
        {
            if (Enemy.LifePoints <= 800 || (Bot.GetMonsterCount()>=4 && Enemy.LifePoints <= 1600))
            {
                AI.SelectPosition(CardPosition.FaceUpDefence);
                return true;
            }
            return false;
        }

        private bool DarkRebellionXyzDragonSummon()
        {
            int selfBestAttack = Util.GetBestAttack(Bot);
            int oppoBestAttack = Util.GetBestAttack(Enemy);
            return selfBestAttack <= oppoBestAttack;
        }

        private bool DarkRebellionXyzDragonEffect()
        {
            int oppoBestAttack = Util.GetBestAttack(Enemy);
            ClientCard target = Util.GetOneEnemyBetterThanValue(oppoBestAttack, true);
            if (target != null)
            {
                AI.SelectCard(0);
                AI.SelectNextCard(target);
            }
            return true;
        }
    }
}