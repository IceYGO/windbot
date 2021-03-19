﻿using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using System.Linq;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("Lucky", "AI_Test", "Test")]
    public class LuckyExecutor : DefaultExecutor
    {
        public LuckyExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, ImFeelingLucky);
            AddExecutor(ExecutorType.SpSummon, ImFeelingLucky);

            AddExecutor(ExecutorType.SpSummon, ImFeelingUnlucky);
            AddExecutor(ExecutorType.Activate, ImFeelingUnlucky);

            AddExecutor(ExecutorType.SummonOrSet, ImFeelingLazy);
            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);
            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);

            AddExecutor(ExecutorType.Activate, _CardId.MysticalSpaceTyphoon, DefaultMysticalSpaceTyphoon);
            AddExecutor(ExecutorType.Activate, _CardId.CosmicCyclone, DefaultCosmicCyclone);
            AddExecutor(ExecutorType.Activate, _CardId.GalaxyCyclone, DefaultGalaxyCyclone);
            AddExecutor(ExecutorType.Activate, _CardId.BookOfMoon, DefaultBookOfMoon);
            AddExecutor(ExecutorType.Activate, _CardId.CompulsoryEvacuationDevice, DefaultCompulsoryEvacuationDevice);
            AddExecutor(ExecutorType.Activate, _CardId.CallOfTheHaunted, DefaultCallOfTheHaunted);
            AddExecutor(ExecutorType.Activate, _CardId.Scapegoat, DefaultScapegoat);
            AddExecutor(ExecutorType.Activate, _CardId.MaxxC, DefaultMaxxC);
            AddExecutor(ExecutorType.Activate, _CardId.AshBlossom, DefaultAshBlossomAndJoyousSpring);
            AddExecutor(ExecutorType.Activate, _CardId.GhostOgreAndSnowRabbit, DefaultGhostOgreAndSnowRabbit);
            AddExecutor(ExecutorType.Activate, _CardId.GhostBelle, DefaultGhostBelleAndHauntedMansion);
            AddExecutor(ExecutorType.Activate, _CardId.EffectVeiler, DefaultEffectVeiler);
            AddExecutor(ExecutorType.Activate, _CardId.CalledByTheGrave, DefaultCalledByTheGrave);
            AddExecutor(ExecutorType.Activate, _CardId.InfiniteImpermanence, DefaultInfiniteImpermanence);
            AddExecutor(ExecutorType.Activate, _CardId.BreakthroughSkill, DefaultBreakthroughSkill);
            AddExecutor(ExecutorType.Activate, _CardId.SolemnJudgment, DefaultSolemnJudgment);
            AddExecutor(ExecutorType.Activate, _CardId.SolemnWarning, DefaultSolemnWarning);
            AddExecutor(ExecutorType.Activate, _CardId.SolemnStrike, DefaultSolemnStrike);
            AddExecutor(ExecutorType.Activate, _CardId.TorrentialTribute, DefaultTorrentialTribute);
            AddExecutor(ExecutorType.Activate, _CardId.HeavyStorm, DefaultHeavyStorm);
            AddExecutor(ExecutorType.Activate, _CardId.HarpiesFeatherDuster, DefaultHarpiesFeatherDusterFirst);
            AddExecutor(ExecutorType.Activate, _CardId.HammerShot, DefaultHammerShot);
            AddExecutor(ExecutorType.Activate, _CardId.DarkHole, DefaultDarkHole);
            AddExecutor(ExecutorType.Activate, _CardId.Raigeki, DefaultRaigeki);
            AddExecutor(ExecutorType.Activate, _CardId.SmashingGround, DefaultSmashingGround);
            AddExecutor(ExecutorType.Activate, _CardId.PotOfDesires, DefaultPotOfDesires);
            AddExecutor(ExecutorType.Activate, _CardId.AllureofDarkness, DefaultAllureofDarkness);
            AddExecutor(ExecutorType.Activate, _CardId.DimensionalBarrier, DefaultDimensionalBarrier);
            AddExecutor(ExecutorType.Activate, _CardId.InterruptedKaijuSlumber, DefaultInterruptedKaijuSlumber);

            AddExecutor(ExecutorType.SpSummon, _CardId.JizukirutheStarDestroyingKaiju, DefaultKaijuSpsummon);
            AddExecutor(ExecutorType.SpSummon, _CardId.GadarlatheMysteryDustKaiju, DefaultKaijuSpsummon);
            AddExecutor(ExecutorType.SpSummon, _CardId.GamecieltheSeaTurtleKaiju, DefaultKaijuSpsummon);
            AddExecutor(ExecutorType.SpSummon, _CardId.RadiantheMultidimensionalKaiju, DefaultKaijuSpsummon);
            AddExecutor(ExecutorType.SpSummon, _CardId.KumongoustheStickyStringKaiju, DefaultKaijuSpsummon);
            AddExecutor(ExecutorType.SpSummon, _CardId.ThunderKingtheLightningstrikeKaiju, DefaultKaijuSpsummon);
            AddExecutor(ExecutorType.SpSummon, _CardId.DogorantheMadFlameKaiju, DefaultKaijuSpsummon);
            AddExecutor(ExecutorType.SpSummon, _CardId.SuperAntiKaijuWarMachineMechaDogoran, DefaultKaijuSpsummon);

            AddExecutor(ExecutorType.SpSummon, _CardId.EvilswarmExcitonKnight, DefaultEvilswarmExcitonKnightSummon);
            AddExecutor(ExecutorType.Activate, _CardId.EvilswarmExcitonKnight, DefaultEvilswarmExcitonKnightEffect);

            AddExecutor(ExecutorType.Summon, _CardId.SandaionTheTimelord);
            AddExecutor(ExecutorType.Summon, _CardId.GabrionTheTimelord);
            AddExecutor(ExecutorType.Summon, _CardId.MichionTheTimelord);
            AddExecutor(ExecutorType.Summon, _CardId.ZaphionTheTimelord);
            AddExecutor(ExecutorType.Summon, _CardId.HailonTheTimelord);
            AddExecutor(ExecutorType.Summon, _CardId.RaphionTheTimelord);
            AddExecutor(ExecutorType.Summon, _CardId.SadionTheTimelord);
            AddExecutor(ExecutorType.Summon, _CardId.MetaionTheTimelord);
            AddExecutor(ExecutorType.Summon, _CardId.KamionTheTimelord);
            AddExecutor(ExecutorType.Summon, _CardId.LazionTheTimelord);

            AddExecutor(ExecutorType.Summon, _CardId.LeftArmofTheForbiddenOne, JustDontIt);
            AddExecutor(ExecutorType.Summon, _CardId.RightLegofTheForbiddenOne, JustDontIt);
            AddExecutor(ExecutorType.Summon, _CardId.LeftLegofTheForbiddenOne, JustDontIt);
            AddExecutor(ExecutorType.Summon, _CardId.RightArmofTheForbiddenOne, JustDontIt);
            AddExecutor(ExecutorType.Summon, _CardId.ExodiaTheForbiddenOne, JustDontIt);
        }

        private List<int> HintMsgForRemove = new List<int>
        {
            HintMsg.Release, HintMsg.Destroy, HintMsg.Remove, HintMsg.ToGrave, HintMsg.ReturnToHand, HintMsg.ToDeck,
            HintMsg.FusionMaterial, HintMsg.SynchroMaterial, HintMsg.XyzMaterial, HintMsg.LinkMaterial, HintMsg.Disable
        };

        public override IList<ClientCard> OnSelectCard(IList<ClientCard> _cards, int min, int max, int hint, bool cancelable)
        {
            if (Duel.Phase == DuelPhase.BattleStart)
                return null;
            if (AI.HaveSelectedCards())
                return null;

            IList<ClientCard> selected = new List<ClientCard>();
            IList<ClientCard> cards = new List<ClientCard>(_cards);
            if (max > cards.Count)
                max = cards.Count;

            if (HintMsgForRemove.Contains(hint))
            {
                IList<ClientCard> enemyCards = cards.Where(card => card.Controller == 1).ToList();

                // select enemy's card first
                while (enemyCards.Count > 0 && selected.Count < max)
                {
                    ClientCard card = enemyCards[Program.Rand.Next(enemyCards.Count)];
                    selected.Add(card);
                    enemyCards.Remove(card);
                    cards.Remove(card);
                }
            }

            // select random cards
            while (selected.Count < min)
            {
                ClientCard card = cards[Program.Rand.Next(cards.Count)];
                selected.Add(card);
                cards.Remove(card);
            }

            return selected;
        }

        public override int OnSelectOption(IList<int> options)
        {
            return Program.Rand.Next(options.Count);
        }

        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            YGOSharp.OCGWrapper.NamedCard cardData = YGOSharp.OCGWrapper.NamedCard.Get(cardId);
            if (cardData != null)
            {
                if (cardData.Attack < 0)
                    return CardPosition.FaceUpAttack;
                if (cardData.Attack <= 1000)
                    return CardPosition.FaceUpDefence;
            }
            return 0;
        }

        private bool ImFeelingLucky()
        {
            return Program.Rand.Next(10) >= 5 && DefaultDontChainMyself();
        }

        private bool ImFeelingUnlucky()
        {
            return DefaultDontChainMyself();
        }

        private bool ImFeelingLazy()
        {
            if (Executors.Any(exec => (exec.Type == ExecutorType.SummonOrSet || exec.Type == ExecutorType.Summon || exec.Type == ExecutorType.MonsterSet) && exec.CardId == Card.Id))
                return false;
            return DefaultMonsterSummon();
        }

        private bool JustDontIt()
        {
            return false;
        }
    }
}