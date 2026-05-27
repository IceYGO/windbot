using System;
using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using System.Linq;

namespace WindBot.Game.AI.Decks
{
    [Deck("Dragun", "AI_Dragun")]
    class DragunExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int DarkMagician = 46986414;
            public const int RedEyesBDragon = 74677422;
            public const int RedEyesWyvern = 67300516;
            public const int TourGuideFromTheUnderworld = 10802915;
            public const int Sangan = 26202165;
            public const int CrusadiaArboria = 91646304;
            public const int AshBlossomJoyousSpring = 14558127;
            public const int MechaPhantomBeastOLion = 72291078;
            public const int MechaPhantomBeastOLionToken = 72291079;
            public const int MaxxC = 23434538;
            public const int MagiciansSouls = 97631303;

            public const int InstantFusion = 1845204;
            public const int RedEyesFusion = 6172122;
            public const int MagicalizedFusion = 11827244;
            public const int HarpiesFeatherDuster = 18144506;
            public const int FoolishBurial = 81439173;
            public const int MonsterReborn = 83764718;
            public const int RedEyesInsight = 92353449;
            public const int CalledbyTheGrave = 24224830;
            public const int InfiniteImpermanence = 10045474;
            public const int SolemnStrike = 40605147;

            public const int DragunofRedEyes = 37818794;
            public const int SeaMonsterofTheseus = 96334243;
            public const int ThousandEyesRestrict = 63519819;
            public const int CrystronHalqifibrax = 50588353;
            public const int PredaplantVerteAnaconda = 70369116;
            public const int LinkSpider = 98978921;
            public const int ImdukTheWorldChaliceDragon = 31226177;
            public const int SalamangreatAlmiraj = 60303245;
        }

        public DragunExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // counter
            AddExecutor(ExecutorType.Activate, CardId.AshBlossomJoyousSpring, DefaultAshBlossomAndJoyousSpring);
            AddExecutor(ExecutorType.Activate, CardId.CalledbyTheGrave, DefaultCalledByTheGrave);
            AddExecutor(ExecutorType.Activate, CardId.InfiniteImpermanence, DefaultInfiniteImpermanence);
            AddExecutor(ExecutorType.Activate, CardId.SolemnStrike, DefaultSolemnStrike);
            AddExecutor(ExecutorType.Activate, CardId.DragunofRedEyes, DragunofRedEyesCounter);

            AddExecutor(ExecutorType.Activate, CardId.MaxxC, DefaultMaxxC);
            AddExecutor(ExecutorType.Activate, CardId.HarpiesFeatherDuster);

            AddExecutor(ExecutorType.Activate, CardId.DragunofRedEyes, DragunofRedEyesDestroy);
            AddExecutor(ExecutorType.Activate, CardId.ThousandEyesRestrict, ThousandEyesRestrictEffect);

            AddExecutor(ExecutorType.Activate, CardId.RedEyesInsight, RedEyesInsightEffect);

            AddExecutor(ExecutorType.Activate, CardId.RedEyesFusion, RedEyesFusionEffect);

            AddExecutor(ExecutorType.Repos, MonsterRepos);

            AddExecutor(ExecutorType.Summon, CardId.TourGuideFromTheUnderworld, TourGuideFromTheUnderworldSummon);
            AddExecutor(ExecutorType.Activate, CardId.TourGuideFromTheUnderworld, TourGuideFromTheUnderworldEffect);
            AddExecutor(ExecutorType.Summon, CardId.Sangan, SanganSummon);
            AddExecutor(ExecutorType.Activate, CardId.Sangan, SanganEffect);

            AddExecutor(ExecutorType.Summon, CardId.MechaPhantomBeastOLion);
            AddExecutor(ExecutorType.Activate, CardId.MechaPhantomBeastOLion, MechaPhantomBeastOLionEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.SalamangreatAlmiraj, SalamangreatAlmirajSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.ImdukTheWorldChaliceDragon, ImdukTheWorldChaliceDragonSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.LinkSpider, LinkSpiderSummon);

            AddExecutor(ExecutorType.SpSummon, CardId.CrusadiaArboria);

            AddExecutor(ExecutorType.Activate, CardId.InstantFusion, InstantFusionEffect);

            AddExecutor(ExecutorType.Summon, CardId.RedEyesWyvern);
            AddExecutor(ExecutorType.Summon, CardId.CrusadiaArboria, SummonForMaterial);
            AddExecutor(ExecutorType.Summon, CardId.AshBlossomJoyousSpring, SummonForMaterial);
            AddExecutor(ExecutorType.Summon, CardId.MaxxC, SummonForMaterial);

            AddExecutor(ExecutorType.Activate, CardId.FoolishBurial, FoolishBurialEffect);
            AddExecutor(ExecutorType.Activate, CardId.MonsterReborn, MonsterRebornEffect);

            AddExecutor(ExecutorType.Activate, CardId.MagiciansSouls, MagiciansSoulsEffect);
            AddExecutor(ExecutorType.Summon, CardId.MagiciansSouls, SummonForMaterial);

            AddExecutor(ExecutorType.SpSummon, CardId.CrystronHalqifibrax, CrystronNeedlefiberSummon);
            AddExecutor(ExecutorType.Activate, CardId.CrystronHalqifibrax, CrystronNeedlefiberEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.PredaplantVerteAnaconda, PredaplantVerteAnacondaSummon);

            AddExecutor(ExecutorType.Activate, CardId.MagicalizedFusion, MagicalizedFusionEffect);

            AddExecutor(ExecutorType.Activate, CardId.PredaplantVerteAnaconda, PredaplantVerteAnacondaEffect);

            AddExecutor(ExecutorType.SpellSet, CardId.InfiniteImpermanence, TrapSet);
            AddExecutor(ExecutorType.SpellSet, CardId.SolemnStrike, TrapSet);

            AddExecutor(ExecutorType.MonsterSet, CardId.Sangan);

        }

        private bool BeastOLionUsed = false;
        private bool RedEyesFusionUsed = false;
        public override bool OnSelectHand()
        {
            // go first
            return true;
        }

        public override void OnNewTurn()
        {
            BeastOLionUsed = false;
            RedEyesFusionUsed = false;
            base.OnNewTurn();
        }

        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            YGOSharp.OCGWrapper.NamedCard cardData = YGOSharp.OCGWrapper.NamedCard.Get(cardId);
            if (cardData != null)
            {
                if (cardData.Attack <= 1000)
                    return CardPosition.FaceUpDefence;
            }
            return 0;
        }

        public override int OnSelectPlace(int cardId, int player, CardLocation location, int available)
        {
            if (location == CardLocation.MonsterZone)
            {
                return available & ~Bot.GetLinkedZones();
            }
            return 0;
        }

        private bool DragunofRedEyesCounter()
        {
            if (ActivateDescription != -1 && ActivateDescription != Util.GetStringId(CardId.DragunofRedEyes, 1))
                return false;
            if (Duel.LastChainPlayer != 1)
                return false;
            AI.SelectCard(new[] {
                CardId.RedEyesWyvern,
                CardId.MechaPhantomBeastOLion
            });
            return true;
        }

        private bool DragunofRedEyesDestroy()
        {
            if (ActivateDescription == -1 || ActivateDescription == Util.GetStringId(CardId.DragunofRedEyes, 1))
                return false;
            AI.SelectCard(Util.GetBestEnemyMonster());
            return true;
        }

        private bool ThousandEyesRestrictEffect()
        {
            AI.SelectCard(Util.GetBestEnemyMonster());
            return true;
        }

        private bool RedEyesInsightEffect()
        {
            if (Bot.HasInHand(CardId.RedEyesFusion))
                return false;
            if (Bot.GetRemainingCount(CardId.RedEyesWyvern, 1) == 0 && Bot.GetRemainingCount(CardId.RedEyesBDragon, 2) == 1 && !Bot.HasInHand(CardId.RedEyesBDragon))
                return false;
            AI.SelectCard(CardId.RedEyesWyvern);
            return true;
        }

        private bool RedEyesFusionEffect()
        {
            if (Bot.HasInMonstersZone(new[] { CardId.DragunofRedEyes, CardId.RedEyesBDragon }))
            { // you don't want to use DragunofRedEyes which is treated as RedEyesBDragon as fusion material
                if (Util.GetBotAvailZonesFromExtraDeck() == 0)
                    return false;
                if (Bot.GetRemainingCount(CardId.RedEyesBDragon, 2) == 0 && !Bot.HasInHand(CardId.RedEyesBDragon))
                    return false;
            }
            AI.SelectMaterials(CardLocation.Deck);
            RedEyesFusionUsed = true;
            return true;
        }

        private bool TourGuideFromTheUnderworldSummon()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            if (Bot.GetRemainingCount(CardId.TourGuideFromTheUnderworld, 2) == 0 && Bot.GetRemainingCount(CardId.Sangan, 2) == 0)
                return false;
            return true;
        }

        private bool TourGuideFromTheUnderworldEffect()
        {
            AI.SelectCard(CardId.Sangan);
            return true;
        }

        private bool SanganSummon()
        {
            return true;
        }

        private bool SanganEffect()
        {
            if (Bot.HasInMonstersZone(CardId.SalamangreatAlmiraj) && !Bot.HasInHand(CardId.CrusadiaArboria))
                AI.SelectCard(CardId.CrusadiaArboria);
            else if (!Bot.HasInHand(CardId.MaxxC))
                AI.SelectCard(CardId.MaxxC);
            else if (!Bot.HasInHand(CardId.AshBlossomJoyousSpring))
                AI.SelectCard(CardId.AshBlossomJoyousSpring);
            else if (!Bot.HasInHand(CardId.MagiciansSouls))
                AI.SelectCard(CardId.MagiciansSouls);
            else if (!Bot.HasInHand(CardId.CrusadiaArboria))
                AI.SelectCard(CardId.CrusadiaArboria);
            else
                AI.SelectCard(new[] {
                    CardId.AshBlossomJoyousSpring,
                    CardId.MaxxC,
                    CardId.CrusadiaArboria
                });
            return true;
        }

        private bool SalamangreatAlmirajSummon()
        {
            int[] materials = new[] {
                CardId.Sangan,
                CardId.MechaPhantomBeastOLion
            };
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials) && !card.IsSpecialSummoned) == 0)
                return false;
            AI.SelectMaterials(materials);
            return true;
        }

        private bool ImdukTheWorldChaliceDragonSummon()
        {
            if (Bot.HasInMonstersZone(CardId.PredaplantVerteAnaconda, true) || !Bot.HasInExtra(CardId.PredaplantVerteAnaconda))
                return false;
            if (Bot.Graveyard.GetMatchingCardsCount(card => (card.Race & (int)CardRace.Dragon) > 0) >= 0)
                return false;
            if (Bot.GetMonsterCount() == 1 && Bot.Hand.GetMatchingCardsCount(card => card.Level <= 4) == 0 && !Util.IsTurn1OrMain2())
                return false;
            if (Bot.GetMonsterCount() >= 2 && Bot.MonsterZone.GetMatchingCardsCount(card => card.Level >= 8) > 0)
                return false;
            return true;
        }

        private bool LinkSpiderSummon()
        {
            if (!Bot.HasInMonstersZone(CardId.MechaPhantomBeastOLionToken))
                return false;
            AI.SelectMaterials(CardId.MechaPhantomBeastOLionToken);
            return true;
        }

        private bool NeedMonster()
        {
            if (Bot.HasInMonstersZone(CardId.PredaplantVerteAnaconda, true) || !Bot.HasInExtra(CardId.PredaplantVerteAnaconda))
                return false;
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.Level >= 8) > 0)
                return false;
            if (Bot.GetMonsterCount() == 0 && Bot.Hand.GetMatchingCardsCount(card => card.Level <= 4) == 0)
                return false;
            if (Bot.GetMonsterCount() >= 2)
                return false;

            return true;
        }

        private bool InstantFusionEffect()
        {
            if (!NeedMonster())
                return false;

            if (Enemy.GetMonsterCount() > 0)
                AI.SelectCard(CardId.ThousandEyesRestrict);
            else
                AI.SelectCard(CardId.SeaMonsterofTheseus);
            return true;
        }

        private bool SummonForMaterial()
        {
            if (Bot.HasInMonstersZone(CardId.PredaplantVerteAnaconda, true) || !Bot.HasInExtra(CardId.PredaplantVerteAnaconda))
                return false;
            if (Bot.MonsterZone.GetMatchingCardsCount(card => (card.HasType(CardType.Effect) || card.IsTuner()) && card.Level < 8) == 1)
                return true;
            if (Bot.HasInHand(CardId.MagiciansSouls))
                return true;
            return false;
        }

        private bool MagiciansSoulsEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            if (Card.Location == CardLocation.Hand)
            {
                if (RedEyesFusionUsed)
                    return false;
                if (Bot.GetMonsterCount() >= 2)
                    return false;
                AI.SelectOption(1);
                AI.SelectYesNo(true);
                return true;
            }
            else
            {
                int[] costs = new[] {
                    CardId.RedEyesInsight,
                    CardId.RedEyesFusion
                };
                if (Bot.HasInHand(costs))
                {
                    AI.SelectCard(costs);
                    return true;
                }
                return false;
            }
        }

        private bool PredaplantVerteAnacondaSummon()
        {
            if (Bot.HasInMonstersZone(CardId.PredaplantVerteAnaconda, true))
                return false;

            int[] materials = new[] {
                CardId.ImdukTheWorldChaliceDragon,
                CardId.Sangan,
                CardId.TourGuideFromTheUnderworld,
                CardId.CrusadiaArboria,
                CardId.MechaPhantomBeastOLion,
                CardId.MagiciansSouls,
                CardId.SalamangreatAlmiraj,
                CardId.LinkSpider,
                CardId.ThousandEyesRestrict,
                CardId.AshBlossomJoyousSpring,
                CardId.MaxxC,
                CardId.RedEyesWyvern,
                CardId.CrystronHalqifibrax
            };
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials)) >= 2)
            {
                AI.SelectMaterials(materials);
                return true;
            }
            return false;
        }

        private bool MagicalizedFusionEffect()
        {
            if (Bot.HasInMonstersZone(new[] { CardId.DragunofRedEyes, CardId.RedEyesBDragon }))
            { // you don't want to use DragunofRedEyes which is treated as RedEyesBDragon as fusion material
                if (Util.GetBotAvailZonesFromExtraDeck() == 0)
                    return false;
                if (Bot.Graveyard.GetMatchingCardsCount(card => (card.Race & (int)CardRace.Dragon) > 0) == 0)
                    return false;
            }
            AI.SelectMaterials(CardLocation.Grave);
            return true;
        }

        private bool PredaplantVerteAnacondaEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            if (ActivateDescription == Util.GetStringId(CardId.PredaplantVerteAnaconda, 0))
                return false;
            AI.SelectCard(CardId.RedEyesFusion);
            AI.SelectMaterials(CardLocation.Deck);
            return true;
        }

        private bool FoolishBurialEffect()
        {
            if (RedEyesFusionUsed)
                return false;

            if (Bot.HasInHand(CardId.MagicalizedFusion))
            {
                if (Bot.HasInGraveyard(CardId.DarkMagician) && Bot.Graveyard.GetMatchingCardsCount(card => (card.Race & (int)CardRace.Dragon) > 0) == 0)
                {
                    AI.SelectCard(new[]
                    {
                        CardId.RedEyesWyvern,
                        CardId.RedEyesBDragon
                    });
                    return true;
                }
                if (!Bot.HasInGraveyard(CardId.DarkMagician) && Bot.Graveyard.GetMatchingCardsCount(card => (card.Race & (int)CardRace.Dragon) > 0) > 0)
                {
                    AI.SelectCard(CardId.DarkMagician);
                    return true;
                }
            }

            if (!NeedMonster())
                return false;

            AI.SelectCard(new[] {
                CardId.MechaPhantomBeastOLion
            });
            return true;
        }

        private bool MonsterRebornEffect()
        {
            if (Bot.HasInGraveyard(CardId.DragunofRedEyes))
            {
                AI.SelectCard(CardId.DragunofRedEyes);
                return true;
            }
            else
            {
                if (!NeedMonster())
                    return false;

                AI.SelectCard(new[] {
                    CardId.PredaplantVerteAnaconda,
                    CardId.Sangan,
                    CardId.ThousandEyesRestrict,
                    CardId.MechaPhantomBeastOLion,
                    CardId.CrusadiaArboria,
                    CardId.AshBlossomJoyousSpring
                });
                return true;
            }
        }

        private bool MechaPhantomBeastOLionEffect()
        {
            if (ActivateDescription == -1)
            {
                BeastOLionUsed = true;
                return true;
            }
            return !BeastOLionUsed;
        }


        private bool CrystronNeedlefiberSummon()
        {
            if (Bot.HasInMonstersZone(CardId.PredaplantVerteAnaconda, true))
                return false;

            int[] materials = new[] {
                CardId.CrusadiaArboria,
                CardId.MechaPhantomBeastOLion,
                CardId.AshBlossomJoyousSpring,
                CardId.SeaMonsterofTheseus,
                CardId.MechaPhantomBeastOLionToken,
                CardId.DarkMagician,
                CardId.ImdukTheWorldChaliceDragon,
                CardId.Sangan,
                CardId.TourGuideFromTheUnderworld,
                CardId.MagiciansSouls,
                CardId.SalamangreatAlmiraj,
                CardId.LinkSpider,
                CardId.ThousandEyesRestrict,
                CardId.SeaMonsterofTheseus,
                CardId.MaxxC,
                CardId.RedEyesWyvern
            };
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials)) >= 2)
            {
                AI.SelectMaterials(materials);
                return true;
            }
            return false;
        }

        private bool CrystronNeedlefiberEffect()
        {
            if (Duel.Player == 0)
            {
                AI.SelectCard(CardId.MechaPhantomBeastOLion);
                return true;
            }
            else
            {
                return true;
            }
        }

        private bool TrapSet()
        {
            if (Bot.HasInMonstersZone(new[] { CardId.DragunofRedEyes, CardId.RedEyesBDragon }) && Bot.GetHandCount() == 1)
                return false;
            AI.SelectPlace(Zones.z0 + Zones.z1 + Zones.z3 + Zones.z4);
            return true;
        }

        private bool MonsterRepos()
        {
            if (Card.IsFacedown())
                return true;
            return DefaultMonsterRepos();
        }
    }
}
