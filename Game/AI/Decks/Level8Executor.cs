using System;
using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using System.Linq;

namespace WindBot.Game.AI.Decks
{
    [Deck("Level VIII", "AI_Level8")]
    class Level8Executor : DefaultExecutor
    {
        public class CardId
        {
            public const int AngelTrumpeter = 87979586;
            public const int ScrapGolem = 82012319;
            public const int PhotonThrasher = 65367484;
            public const int WorldCarrotweightChampion = 44928016;
            public const int RaidenHandofTheLightsworn = 77558536;
            public const int ScrapBeast = 19139516;
            public const int PerformageTrickClown = 67696066;
            public const int MaskedChameleon = 53573406;
            public const int Goblindbergh = 25259669;
            public const int WhiteRoseDragon = 12213463;
            public const int RedRoseDragon = 26118970;
            public const int ScrapRecycler = 4334811;
            public const int MechaPhantomBeastOLion = 72291078;
            public const int MechaPhantomBeastOLionToken = 72291079;
            public const int JetSynchron = 9742784;

            public const int UnexpectedDai = 911883;
            public const int Raigeki = 12580477;
            public const int HarpiesFeatherDuster = 18144506;
            public const int ReinforcementofTheArmy = 32807846;
            public const int FoolishBurial = 81439173;
            public const int MonsterReborn = 83764718;
            public const int ChargeofTheLightBrigade = 94886282;
            public const int CalledbyTheGrave = 24224830;
            public const int SolemnStrike = 40605147;

            public const int WhiteAuraBihamut = 89907227;
            public const int BorreloadSavageDragon = 27548199;
            public const int CrystalWingSynchroDragon = 50954680;
            public const int ScarlightRedDragonArchfiend = 80666118;
            public const int PSYFramelordOmega = 74586817;
            public const int ScrapDragon = 76774528;
            public const int BlackRoseMoonlightDragon = 33698022;
            public const int ShootingRiserDragon = 68431965;
            public const int CoralDragon = 42566602;
            public const int GardenRoseMaiden = 53325667;
            public const int Number41BagooskaTheTerriblyTiredTapir = 90590303;
            public const int MekkKnightCrusadiaAstram = 21887175;
            public const int ScrapWyvern = 47363932;
            public const int CrystronNeedlefiber = 50588353;
        }

        public Level8Executor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, CardId.CalledbyTheGrave, DefaultCalledByTheGrave);
            AddExecutor(ExecutorType.Activate, CardId.Raigeki);
            AddExecutor(ExecutorType.Activate, CardId.HarpiesFeatherDuster);

            AddExecutor(ExecutorType.Repos, CardId.Number41BagooskaTheTerriblyTiredTapir, MonsterRepos);

            AddExecutor(ExecutorType.Activate, CardId.CrystalWingSynchroDragon, CrystalWingSynchroDragonEffect);
            AddExecutor(ExecutorType.Activate, CardId.BorreloadSavageDragon, BorreloadSavageDragonEffect);
            AddExecutor(ExecutorType.Activate, CardId.ScrapGolem, ScrapGolemEffect);

            // empty field
            AddExecutor(ExecutorType.Activate, CardId.UnexpectedDai, UnexpectedDaiFirst);
            AddExecutor(ExecutorType.SpSummon, CardId.PhotonThrasher, PhotonThrasherSummonFirst);
            AddExecutor(ExecutorType.Activate, CardId.UnexpectedDai);
            AddExecutor(ExecutorType.SpSummon, CardId.PhotonThrasher);

            // 
            AddExecutor(ExecutorType.Activate, CardId.ReinforcementofTheArmy, ReinforcementofTheArmyEffect);
            AddExecutor(ExecutorType.Activate, CardId.FoolishBurial, FoolishBurialEffect);
            AddExecutor(ExecutorType.Activate, CardId.MonsterReborn, DefaultCallOfTheHaunted);

            // scrap combo
            AddExecutor(ExecutorType.Summon, CardId.ScrapRecycler, ScrapRecyclerSummonFirst);
            AddExecutor(ExecutorType.Activate, CardId.ScrapRecycler, ScrapRecyclerEffect);
            AddExecutor(ExecutorType.Activate, CardId.MechaPhantomBeastOLion, MechaPhantomBeastOLionEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.ScrapWyvern, ScrapWyvernSummon);
            AddExecutor(ExecutorType.Activate, CardId.ScrapWyvern, ScrapWyvernEffect);

            AddExecutor(ExecutorType.Activate, CardId.JetSynchron, JetSynchronEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.CrystronNeedlefiber, CrystronNeedlefiberSummon);
            AddExecutor(ExecutorType.Activate, CardId.CrystronNeedlefiber, CrystronNeedlefiberEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.MekkKnightCrusadiaAstram, MekkKnightCrusadiaAstramSummon);
            AddExecutor(ExecutorType.Activate, CardId.MekkKnightCrusadiaAstram, MekkKnightCrusadiaAstramEffect);

            //
            AddExecutor(ExecutorType.Activate, CardId.ChargeofTheLightBrigade);

            // other summon
            AddExecutor(ExecutorType.Summon, CardId.Goblindbergh, GoblindberghSummonFirst);
            AddExecutor(ExecutorType.Activate, CardId.Goblindbergh, GoblindberghEffect);
            AddExecutor(ExecutorType.Summon, CardId.MaskedChameleon, MaskedChameleonSummonFirst);
            AddExecutor(ExecutorType.Activate, CardId.MaskedChameleon, MaskedChameleonEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.WhiteRoseDragon);
            AddExecutor(ExecutorType.Summon, CardId.WhiteRoseDragon, WhiteRoseDragonSummonFirst);
            AddExecutor(ExecutorType.Activate, CardId.WhiteRoseDragon, WhiteRoseDragonEffect);

            //
            AddExecutor(ExecutorType.Summon, CardId.RaidenHandofTheLightsworn, L4TunerSummonFirst);
            AddExecutor(ExecutorType.Summon, CardId.ScrapBeast, L4TunerSummonFirst);
            AddExecutor(ExecutorType.Summon, CardId.AngelTrumpeter, L4TunerSummonFirst);
            AddExecutor(ExecutorType.Summon, CardId.MaskedChameleon, L4TunerSummonFirst);

            AddExecutor(ExecutorType.Summon, CardId.PerformageTrickClown, L4NonTunerSummonFirst);
            AddExecutor(ExecutorType.Summon, CardId.Goblindbergh, L4NonTunerSummonFirst);
            AddExecutor(ExecutorType.Summon, CardId.WorldCarrotweightChampion, L4NonTunerSummonFirst);
            AddExecutor(ExecutorType.Summon, CardId.WhiteRoseDragon, L4NonTunerSummonFirst);

            AddExecutor(ExecutorType.Summon, CardId.RedRoseDragon, OtherTunerSummonFirst);
            AddExecutor(ExecutorType.Summon, CardId.JetSynchron, OtherTunerSummonFirst);
            AddExecutor(ExecutorType.Summon, CardId.MechaPhantomBeastOLion, OtherTunerSummonFirst);

            AddExecutor(ExecutorType.Summon, CardId.RaidenHandofTheLightsworn);
            AddExecutor(ExecutorType.Summon, CardId.Goblindbergh);
            AddExecutor(ExecutorType.Summon, CardId.ScrapBeast);
            AddExecutor(ExecutorType.Summon, CardId.PerformageTrickClown);
            AddExecutor(ExecutorType.Summon, CardId.AngelTrumpeter);
            AddExecutor(ExecutorType.Summon, CardId.WorldCarrotweightChampion);
            AddExecutor(ExecutorType.Summon, CardId.MaskedChameleon);
            AddExecutor(ExecutorType.Summon, CardId.WhiteRoseDragon);

            AddExecutor(ExecutorType.Summon, CardId.RedRoseDragon);
            AddExecutor(ExecutorType.Summon, CardId.JetSynchron);
            AddExecutor(ExecutorType.Summon, CardId.MechaPhantomBeastOLion);

            AddExecutor(ExecutorType.Summon, CardId.ScrapRecycler);

            AddExecutor(ExecutorType.Activate, CardId.RedRoseDragon);
            AddExecutor(ExecutorType.Activate, CardId.RaidenHandofTheLightsworn);
            AddExecutor(ExecutorType.Activate, CardId.PerformageTrickClown, PerformageTrickClownEffect);

            AddExecutor(ExecutorType.Activate, CardId.WorldCarrotweightChampion, WorldCarrotweightChampionEffect);

            // extra monsters
            AddExecutor(ExecutorType.SpSummon, CardId.BorreloadSavageDragon, BorreloadSavageDragonSummon);

            AddExecutor(ExecutorType.SpSummon, CardId.ScrapDragon, ScrapDragonSummon);
            AddExecutor(ExecutorType.Activate, CardId.ScrapDragon, ScrapDragonEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.CrystalWingSynchroDragon);

            AddExecutor(ExecutorType.SpSummon, CardId.ScarlightRedDragonArchfiend, DefaultScarlightRedDragonArchfiendSummon);
            AddExecutor(ExecutorType.Activate, CardId.ScarlightRedDragonArchfiend, DefaultScarlightRedDragonArchfiendEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.PSYFramelordOmega);
            AddExecutor(ExecutorType.Activate, CardId.PSYFramelordOmega, PSYFramelordOmegaEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.WhiteAuraBihamut);
            AddExecutor(ExecutorType.Activate, CardId.WhiteAuraBihamut);

            AddExecutor(ExecutorType.SpSummon, CardId.GardenRoseMaiden);
            AddExecutor(ExecutorType.Activate, CardId.GardenRoseMaiden);

            AddExecutor(ExecutorType.SpSummon, CardId.CoralDragon);
            AddExecutor(ExecutorType.Activate, CardId.CoralDragon, CoralDragonEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.ShootingRiserDragon, ShootingRiserDragonSummon);
            AddExecutor(ExecutorType.Activate, CardId.ShootingRiserDragon, ShootingRiserDragonEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.BlackRoseMoonlightDragon);

            AddExecutor(ExecutorType.SpSummon, CardId.Number41BagooskaTheTerriblyTiredTapir, Number41BagooskaTheTerriblyTiredTapirSummon);

            AddExecutor(ExecutorType.Summon, CardId.ScrapGolem, ScrapGolemSummon);

            AddExecutor(ExecutorType.SpellSet, CardId.CalledbyTheGrave);
            AddExecutor(ExecutorType.SpellSet, CardId.SolemnStrike);
            AddExecutor(ExecutorType.Activate, CardId.SolemnStrike, DefaultSolemnStrike);
            AddExecutor(ExecutorType.Repos, MonsterRepos);
        }

        private bool BeastOLionUsed = false;
        private bool JetSynchronUsed = false;
        private bool ScrapWyvernUsed = false;
        private bool MaskedChameleonUsed = false;

        private int[] HandCosts = new[]
        {
            CardId.PerformageTrickClown,
            CardId.JetSynchron,
            CardId.MechaPhantomBeastOLion,
            CardId.ScrapGolem,
            CardId.WorldCarrotweightChampion
        };

        private int[] L4NonTuners = new[]
        {
            CardId.PhotonThrasher,
            CardId.WorldCarrotweightChampion,
            CardId.PerformageTrickClown,
            CardId.Goblindbergh,
            CardId.WhiteRoseDragon
        };

        private int[] L4Tuners = new[]
        {
            CardId.RaidenHandofTheLightsworn,
            CardId.ScrapBeast,
            CardId.AngelTrumpeter,
            CardId.MaskedChameleon
        };

        public override void OnNewTurn()
        {
            BeastOLionUsed = false;
            JetSynchronUsed = false;
            ScrapWyvernUsed = false;
            MaskedChameleonUsed = false;
            base.OnNewTurn();
        }

        public override void OnChainEnd()
        {
            base.OnChainEnd();
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
            if (location == CardLocation.SpellZone)
            {
                if (cardId == CardId.MekkKnightCrusadiaAstram || cardId == CardId.ScrapWyvern || cardId == CardId.CrystronNeedlefiber)
                {
                    ClientCard b = Bot.MonsterZone.GetFirstMatchingCard(card => card.Id == CardId.BorreloadSavageDragon);
                    int zone = (1 << (b?.Sequence ?? 0)) & available;
                    if (zone > 0)
                        return zone;
                }
                if ((available & Zones.z4) > 0)
                    return Zones.z4;
                if ((available & Zones.z3) > 0)
                    return Zones.z3;
                if ((available & Zones.z2) > 0)
                    return Zones.z2;
                if ((available & Zones.z1) > 0)
                    return Zones.z1;
                if ((available & Zones.z0) > 0)
                    return Zones.z0;
            }
            if (location == CardLocation.MonsterZone)
            {
                if ((available & Zones.z6) > 0)
                    return Zones.z6;
                if ((available & Zones.z5) > 0)
                    return Zones.z5;
                if ((available & Zones.z0) > 0)
                    return Zones.z0;
                if ((available & Zones.z2) > 0)
                    return Zones.z2;
                if ((available & Zones.z4) > 0)
                    return Zones.z4;
                if ((available & Zones.z1) > 0)
                    return Zones.z1;
                if ((available & Zones.z3) > 0)
                    return Zones.z3;
            }
            return 0;
        }

        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            if (!defender.IsMonsterHasPreventActivationEffectInBattle())
            {
                if (!attacker.IsDisabled() && (attacker.IsCode(CardId.MekkKnightCrusadiaAstram) && defender.IsSpecialSummoned
                                            || attacker.IsCode(CardId.CrystalWingSynchroDragon) && defender.Level>=5))
                {
                    attacker.RealPower = attacker.RealPower + defender.Attack;
                }
            }
            return base.OnPreBattleBetween(attacker, defender);
        }

        private bool UnexpectedDaiFirst()
        {
            if (Bot.HasInHand(CardId.ScrapRecycler))
                return true;
            if (Bot.HasInHand(new[] {
                CardId.WorldCarrotweightChampion,
                CardId.PerformageTrickClown,
                CardId.Goblindbergh,
                CardId.WhiteRoseDragon
            }))
                return true;
            return false;
        }

        private bool PhotonThrasherSummonFirst()
        {
            if (Bot.HasInHand(CardId.ScrapRecycler))
                return true;
            if (Bot.HasInHand(L4Tuners))
                return true;
            return false;
        }

        private bool ReinforcementofTheArmyEffect()
        {
            if (Bot.GetMonsterCount() == 0 && PhotonThrasherSummonFirst() && !Bot.HasInHand(CardId.PhotonThrasher))
            {
                AI.SelectCard(CardId.PhotonThrasher);
                return true;
            }
            if (GoblindberghSummonFirst() && !Bot.HasInHand(CardId.Goblindbergh))
            {
                AI.SelectCard(CardId.Goblindbergh);
                return true;
            }
            AI.SelectCard(new[] {
                CardId.Goblindbergh,
                CardId.RaidenHandofTheLightsworn,
                CardId.PhotonThrasher
            });
            return true;
        }

        private bool FoolishBurialEffect()
        {
            if (!Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.ScrapRecycler))
            {
                AI.SelectCard(CardId.ScrapRecycler);
                return true;
            }
            if (L4NonTunerSummonFirst() && Bot.GetRemainingCount(CardId.PerformageTrickClown, 1) > 0)
            {
                AI.SelectCard(CardId.PerformageTrickClown);
                return true;
            }
            AI.SelectCard(new[] {
                CardId.JetSynchron,
                CardId.MechaPhantomBeastOLion,
                CardId.ScrapBeast,
                CardId.PhotonThrasher
            });
            return true;
        }

        private bool ScrapRecyclerSummonFirst()
        {
            return Bot.GetRemainingCount(CardId.ScrapGolem, 2) > 0 && Bot.GetRemainingCount(CardId.MechaPhantomBeastOLion, 2) > 0 && Bot.GetRemainingCount(CardId.JetSynchron, 2) > 0;
        }

        private bool ScrapRecyclerEffect()
        {
            if ((Bot.HasInMonstersZone(CardId.ScrapGolem) && !JetSynchronUsed) || BeastOLionUsed)
            {
                AI.SelectCard(new[] { CardId.JetSynchron, CardId.MechaPhantomBeastOLion });
            }
            else
            {
                AI.SelectCard(new[] { CardId.MechaPhantomBeastOLion, CardId.JetSynchron });
            }
            return true;
        }

        private bool MechaPhantomBeastOLionEffect()
        {
            if (ActivateDescription == -1)
            {
                BeastOLionUsed = true;
                return true;
            }
            // todo: need tuner check
            return !BeastOLionUsed;
        }

        private bool ScrapWyvernSummon()
        {
            if (ScrapWyvernUsed || MaskedChameleonUsed || Bot.HasInMonstersZone(CardId.ScrapWyvern))
                return false;
            if (!Bot.HasInMonstersZone(new[] {
                CardId.ScrapBeast,
                CardId.ScrapGolem,
                CardId.ScrapRecycler,
            }) || !Bot.HasInMonstersZoneOrInGraveyard(CardId.ScrapRecycler))
                return false;

            AI.SelectMaterials(new[] {
                CardId.MechaPhantomBeastOLionToken,
                CardId.PhotonThrasher,
                CardId.Goblindbergh,
                CardId.AngelTrumpeter,
                CardId.PerformageTrickClown,
                CardId.WorldCarrotweightChampion,
                CardId.WhiteRoseDragon,
                CardId.ScrapBeast,
                CardId.ScrapGolem,
                CardId.ScrapRecycler
            });
            return true;
        }

        private bool ScrapWyvernEffect()
        {
            if(ActivateDescription != -1)
            {
                int[] targets = new[]
                {
                    CardId.ScrapRecycler,
                    CardId.ScrapBeast,
                    CardId.ScrapGolem,
                    CardId.ScrapDragon
                };
                AI.SelectCard(targets);
                AI.SelectNextCard(targets);
                ScrapWyvernUsed = true;
                return true;
            }
            else
            {
                AI.SelectCard(new[]
                {
                    CardId.ScrapGolem,
                    CardId.ScrapBeast
                });
                ClientCard target = Util.GetBestEnemyCard();
                if (target != null)
                    AI.SelectNextCard(target);
                else
                    AI.SelectNextCard(new[]
                    {
                        CardId.CalledbyTheGrave,
                        CardId.PhotonThrasher,
                        CardId.PerformageTrickClown,
                        CardId.MechaPhantomBeastOLionToken,
                        CardId.WorldCarrotweightChampion,
                        CardId.WhiteRoseDragon,
                        CardId.Goblindbergh,
                        CardId.AngelTrumpeter,
                        CardId.ScrapWyvern
                    });
                return true;
            }
        }

        private bool ScrapGolemEffect()
        {
            if (Bot.GetMonstersInMainZone().Count == 5)
                return false;
            AI.SelectCard(CardId.ScrapRecycler);
            AI.SelectOption(0);
            return true;
        }

        private bool JetSynchronEffect()
        {
            if (!Bot.HasInMonstersZone(CardId.BlackRoseMoonlightDragon)
                && Bot.MonsterZone.GetMatchingCardsCount(card => card.IsFaceup() && card.Level >= 2 && card.Level <= 5) < 2)
                return false;
            AI.SelectCard(HandCosts);
            return true;
        }

        private bool CrystronNeedlefiberSummon()
        {
            if (MaskedChameleonUsed)
                return false;
            int nonTunerCount = Bot.MonsterZone.GetMatchingCardsCount(card => card.IsFaceup() && !card.IsTuner());
            if (Bot.GetMonsterCount() < 3 || nonTunerCount == 0)
                return false;
            if (nonTunerCount == 1)
            {
                AI.SelectMaterials(new[] {
                    CardId.JetSynchron,
                    CardId.MechaPhantomBeastOLion,
                    CardId.AngelTrumpeter,
                    CardId.RaidenHandofTheLightsworn,
                    CardId.ScrapBeast,
                    CardId.MaskedChameleon,

                    CardId.PerformageTrickClown,
                    CardId.MechaPhantomBeastOLionToken,
                    CardId.ScrapRecycler,
                    CardId.WhiteRoseDragon,
                    CardId.PhotonThrasher,
                    CardId.Goblindbergh,
                    CardId.WorldCarrotweightChampion
                });
            }
            else
            {
                AI.SelectMaterials(new[] {
                    CardId.MechaPhantomBeastOLionToken,
                    CardId.ScrapRecycler,
                    CardId.WhiteRoseDragon,
                    CardId.PhotonThrasher,
                    CardId.Goblindbergh,
                    CardId.WorldCarrotweightChampion,
                    CardId.PerformageTrickClown,

                    CardId.JetSynchron,
                    CardId.MechaPhantomBeastOLion,
                    CardId.AngelTrumpeter,
                    CardId.RaidenHandofTheLightsworn,
                    CardId.ScrapBeast,
                    CardId.MaskedChameleon
                });
            }
            return true;
        }

        private bool CrystronNeedlefiberEffect()
        {
            if (Duel.Player == 0)
            {
                AI.SelectCard(CardId.RedRoseDragon);
                return true;
            }
            else
            {
                if (Bot.HasInExtra(CardId.ShootingRiserDragon) && Bot.MonsterZone.IsExistingMatchingCard(card => card.Level >= 3 && card.Level <= 5 && card.IsFaceup() && !card.IsTuner()))
                {
                    AI.SelectCard(CardId.ShootingRiserDragon);
                    return true;
                }
                if (Util.IsOneEnemyBetterThanValue(1500, true) || DefaultOnBecomeTarget())
                {
                    AI.SelectCard(CardId.CoralDragon);
                    return true;
                }
                return false;
            }
        }

        private bool MekkKnightCrusadiaAstramSummon()
        {
            int[] matcodes = new[] {
                CardId.ScrapWyvern,
                CardId.CrystronNeedlefiber
            };
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(matcodes)) < 2)
                return false;
            AI.SelectMaterials(matcodes);
            return true;
        }

        private bool MekkKnightCrusadiaAstramEffect()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {
                return true;
            }
            else
            {
                ClientCard target = Util.GetBestEnemyCard();
                if (target == null)
                    return false;
                AI.SelectCard(target);
                return true;
            }
        }

        private bool GoblindberghSummonFirst()
        {
            if (Bot.HasInHand(L4Tuners))
                return true;
            return false;
        }

        private bool GoblindberghEffect()
        {
            AI.SelectCard(L4Tuners);
            return true;
        }

        private bool MaskedChameleonSummonFirst()
        {
            if (Bot.HasInGraveyard(new[] {
                CardId.PhotonThrasher,
                CardId.WorldCarrotweightChampion,
                CardId.Goblindbergh
            }))
                return true;
            return false;
        }

        private bool MaskedChameleonEffect()
        {
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsFaceup() && !card.IsTuner()) == 0)
            {
                MaskedChameleonUsed = true;
                AI.SelectCard(L4NonTuners);
                return true;
            }
            return false;
        }

        private bool WhiteRoseDragonSummonFirst()
        {
            if (Bot.HasInGraveyard(new[] {
                CardId.RedRoseDragon
            }))
                return true;
            return false;
        }

        private bool WhiteRoseDragonEffect()
        {
            if (Card.Location == CardLocation.Grave)
            {
                if (Bot.GetRemainingCount(CardId.WorldCarrotweightChampion, 1) > 0)
                {
                    AI.SelectCard(CardId.WorldCarrotweightChampion);
                    return true;
                }
                return false;
            }
            return true;
        }

        private bool L4TunerSummonFirst()
        {
            return Bot.HasInMonstersZone(L4NonTuners, faceUp: true);
        }

        private bool L4NonTunerSummonFirst()
        {
            return Bot.HasInMonstersZone(L4Tuners, faceUp: true);
        }

        private bool OtherTunerSummonFirst()
        {
            return Bot.HasInMonstersZone(L4NonTuners, faceUp: true);
        }

        private bool PerformageTrickClownEffect()
        {
            if (Bot.LifePoints <= 1000)
                return false;
            AI.SelectPosition(CardPosition.FaceUpDefence);
            return true;
        }

        private bool WorldCarrotweightChampionEffect()
        {
            return !Bot.HasInMonstersZone(L4NonTuners);
        }

        private bool ScrapGolemSummon()
        {
            return Bot.GetMonsterCount() <= 2 && Bot.HasInMonstersZoneOrInGraveyard(CardId.ScrapRecycler);
        }

        private bool BorreloadSavageDragonSummon()
        {
            if (!Bot.HasInGraveyard(new[] {
                CardId.ScrapWyvern,
                CardId.CrystronNeedlefiber,
                CardId.MekkKnightCrusadiaAstram
            }))
                return false;
            return true;
        }

        private bool BorreloadSavageDragonEffect()
        {
            if (ActivateDescription == -1)
            {
                AI.SelectCard(new[] { CardId.MekkKnightCrusadiaAstram, CardId.CrystronNeedlefiber, CardId.ScrapWyvern });
                return true;
            }
            else
            {
                return true;
            }
        }

        private bool ScrapDragonSummon()
        {
            if (Util.GetProblematicEnemyCard(3000) != null)
            {
                return true;
            }
            if (Bot.HasInGraveyard(new[] { CardId.ScrapBeast, CardId.ScrapRecycler, CardId.ScrapGolem, CardId.ScrapWyvern }))
            {
                return true;
            }
            return false;
        }

        private bool ScrapDragonEffect()
        {
            ClientCard invincible = Util.GetProblematicEnemyCard(3000);
            if (invincible == null && !Util.IsOneEnemyBetterThanValue(2800 - 1, false))
                return false;

            List<ClientCard> monsters = Enemy.GetMonsters();
            monsters.Sort(CardContainer.CompareCardAttack);

            ClientCard destroyCard = invincible;
            if (destroyCard == null)
            {
                for (int i = monsters.Count - 1; i >= 0; --i)
                {
                    if (monsters[i].IsAttack())
                    {
                        destroyCard = monsters[i];
                        break;
                    }
                }
            }

            if (destroyCard == null)
                return false;

            AI.SelectCard(new[] {
                CardId.CalledbyTheGrave,
                CardId.MechaPhantomBeastOLionToken,
                CardId.ScrapRecycler,
                CardId.WhiteRoseDragon,
                CardId.PhotonThrasher,
                CardId.Goblindbergh,
                CardId.WorldCarrotweightChampion,
                CardId.PerformageTrickClown,
                CardId.JetSynchron,
                CardId.MechaPhantomBeastOLion,
                CardId.AngelTrumpeter,
                CardId.RaidenHandofTheLightsworn,
                CardId.ScrapBeast,
                CardId.MaskedChameleon
            });
            AI.SelectNextCard(destroyCard);

            return true;
        }

        private bool CrystalWingSynchroDragonEffect()
        {
            return Duel.LastChainPlayer != 0;
        }

        private bool PSYFramelordOmegaEffect()
        {
            if (Card.Location == CardLocation.Grave)
            {
                // todo
                return false;
            }
            if (Duel.Player == 0)
            {
                return DefaultOnBecomeTarget();
            }
            if (Duel.Player == 1)
            {
                if (Duel.Phase == DuelPhase.Standby)
                {
                    if (Bot.HasInBanished(CardId.JetSynchron) && !Bot.HasInGraveyard(CardId.JetSynchron))
                    {
                        AI.SelectCard(CardId.JetSynchron);
                        return true;
                    }
                    if (Bot.HasInBanished(CardId.CrystronNeedlefiber))
                    {
                        AI.SelectCard(CardId.CrystronNeedlefiber);
                        return true;
                    }
                }
                else
                {
                    if (Enemy.MonsterZone.GetMatchingCards(card => card.IsAttack()).Sum(card => card.Attack) >= Bot.LifePoints)
                        return false;
                    return true;// DefaultOnBecomeTarget() || Util.IsOneEnemyBetterThanValue(2800, true);
                }
            }
            return false;
        }

        private bool CoralDragonEffect()
        {
            if (Card.Location == CardLocation.Grave)
                return true;

            ClientCard target = Util.GetProblematicEnemyCard(canBeTarget: true);
            if (target != null)
            {
                AI.SelectCard(HandCosts);
                AI.SelectNextCard(target);
                return true;
            }
            return false;
        }

        private bool ShootingRiserDragonSummon()
        {
            return Bot.MonsterZone.GetMatchingCardsCount(card => card.IsFaceup() && !card.IsTuner()) >= 2;
        }

        private bool ShootingRiserDragonEffect()
        {
            if (ActivateDescription == -1 || (ActivateDescription == Util.GetStringId(CardId.ShootingRiserDragon, 0)))
            {
                int targetLevel = 8;

                if (Bot.MonsterZone.IsExistingMatchingCard(card => card.Level == targetLevel - 5 && card.IsFaceup() && !card.IsTuner()) && Bot.GetRemainingCount(CardId.MechaPhantomBeastOLion, 2) > 0)
                {
                    AI.SelectCard(CardId.MechaPhantomBeastOLion);
                }
                else if (Bot.MonsterZone.IsExistingMatchingCard(card => card.Level == targetLevel - 4 && card.IsFaceup() && !card.IsTuner()))
                {
                    AI.SelectCard(new[] {
                        CardId.ScrapRecycler,
                        CardId.RedRoseDragon
                    });
                }
                else if (Bot.MonsterZone.IsExistingMatchingCard(card => card.Level == targetLevel - 3 && card.IsFaceup() && !card.IsTuner()))
                {
                    AI.SelectCard(new[] {
                        CardId.ScrapBeast,
                        CardId.PhotonThrasher,
                        CardId.Goblindbergh,
                        CardId.WorldCarrotweightChampion,
                        CardId.WhiteRoseDragon,
                        CardId.RaidenHandofTheLightsworn,
                        CardId.AngelTrumpeter,
                        CardId.PerformageTrickClown,
                        CardId.MaskedChameleon
                    });
                }
                else
                {
                    FoolishBurialEffect();
                }
                return true;
            }
            else
            {
                if (Duel.LastChainPlayer == 0)
                    return false;
                AI.SelectCard(new[] {
                    CardId.BlackRoseMoonlightDragon,
                    CardId.ScrapDragon,
                    CardId.PSYFramelordOmega
                });
                return true;
            }
        }

        private bool Number41BagooskaTheTerriblyTiredTapirSummon()
        {
            if (!Util.IsTurn1OrMain2())
                return false;
            if (Bot.GetMonsterCount() > 3)
                return false;
            AI.SelectPosition(CardPosition.FaceUpDefence);
            return true;
        }

        private bool MonsterRepos()
        {
            if (Card.IsFacedown())
                return true;
            if (Card.IsCode(CardId.Number41BagooskaTheTerriblyTiredTapir) && Card.IsDefense())
                return Card.Overlays.Count == 0;
            return DefaultMonsterRepos();
        }
    }
}
