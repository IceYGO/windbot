using System;
using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using System.Linq;

namespace WindBot.Game.AI.Decks
{
    [Deck("Orcust", "AI_Orcust")]
    class OrcustExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int OrcustKnightmare = 4055337;
            public const int OrcustHarpHorror = 57835716;
            public const int OrcustCymbalSkeleton = 21441617;
            public const int WorldLegacyWorldWand = 93920420;
            public const int ThePhantomKnightsofAncientCloak = 90432163;
            public const int ThePhantomKnightsofSilentBoots = 36426778;

            public const int TrickstarCarobein = 98169343;
            public const int TrickstarCandina = 61283655;
            public const int ArmageddonKnight = 28985331;
            public const int ScrapRecycler = 4334811;
            public const int DestrudoTheLostDragonsFrisson = 5560911;
            public const int JetSynchron = 9742784;

            public const int AshBlossomJoyousSpring = 14558127;
            public const int GhostBelleHauntedMansion = 73642296;
            public const int MaxxC = 23434538;

            public const int SkyStrikerMobilizeEngage = 63166095;
            public const int SkyStrikerMechaEagleBooster = 25733157;
            public const int SkyStrikerMechaHornetDrones = 52340444;
            public const int SkyStrikerMechaHornetDronesToken = 52340445;
            public const int TrickstarLightStage = 35371948;
            public const int OrcustratedBabel = 90351981;

            public const int ReinforcementofTheArmy = 32807846;
            public const int Terraforming = 73628505;
            public const int FoolishBurial = 81439173;
            public const int CalledbyTheGrave = 24224830;

            public const int ThePhantomKnightsofShadeBrigandine = 98827725;
            public const int PhantomKnightsFogBlade = 25542642;
            public const int OrcustratedClimax = 703897;

            public const int BorreloadSavageDragon = 27548199;
            public const int ShootingRiserDragon = 68431965;
            public const int SheorcustDingirsu = 93854893;
            public const int BorrelswordDragon = 85289965;
            public const int LongirsuTheOrcustOrchestrator = 76145142;
            public const int ThePhantomKnightsofRustyBardiche = 26692769;
            public const int KnightmarePhoenix = 2857636;
            public const int GalateaTheOrcustAutomaton = 30741503;
            public const int CrystronNeedlefiber = 50588353;
            public const int SkyStrikerAceKagari = 63288573;
            public const int KnightmareMermaid = 3679218;
            public const int SalamangreatAlmiraj = 60303245;
        }

        public OrcustExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, CardId.SkyStrikerMechaEagleBooster, EagleBoosterEffect);
            AddExecutor(ExecutorType.Activate, CardId.OrcustratedClimax, ClimaxEffect);

            AddExecutor(ExecutorType.Activate, CardId.MaxxC, DefaultMaxxC);
            AddExecutor(ExecutorType.Activate, CardId.AshBlossomJoyousSpring, DefaultAshBlossomAndJoyousSpring);
            AddExecutor(ExecutorType.Activate, CardId.GhostBelleHauntedMansion, DefaultGhostBelleAndHauntedMansion);
            AddExecutor(ExecutorType.Activate, CardId.CalledbyTheGrave, DefaultCalledByTheGrave);

            AddExecutor(ExecutorType.Activate, CardId.Terraforming, TerraformingEffect);
            AddExecutor(ExecutorType.Activate, CardId.ReinforcementofTheArmy, ReinforcementofTheArmyEffect);
            AddExecutor(ExecutorType.Activate, CardId.FoolishBurial, FoolishBurialEffect);

            AddExecutor(ExecutorType.Activate, CardId.TrickstarLightStage, LightStageEffect);

            AddExecutor(ExecutorType.Activate, CardId.SkyStrikerMobilizeEngage, EngageEffect);
            AddExecutor(ExecutorType.Activate, CardId.SkyStrikerMechaHornetDrones, DronesEffectFirst);
            AddExecutor(ExecutorType.SpSummon, CardId.SkyStrikerAceKagari);
            AddExecutor(ExecutorType.Activate, CardId.SkyStrikerAceKagari);

            AddExecutor(ExecutorType.SpSummon, CardId.KnightmareMermaid, KnightmareMermaidSummon);
            AddExecutor(ExecutorType.Activate, CardId.KnightmareMermaid, KnightmareMermaidEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.TrickstarCarobein, CarobeinSummon);
            AddExecutor(ExecutorType.Activate, CardId.TrickstarCarobein);

            AddExecutor(ExecutorType.SpellSet, CardId.ThePhantomKnightsofShadeBrigandine);

            AddExecutor(ExecutorType.Summon, CardId.ArmageddonKnight, ArmageddonKnightSummon);
            AddExecutor(ExecutorType.Activate, CardId.ArmageddonKnight, ArmageddonKnightEffect);

            AddExecutor(ExecutorType.Summon, CardId.ScrapRecycler, ScrapRecyclerSummon);
            AddExecutor(ExecutorType.Activate, CardId.ScrapRecycler, ScrapRecyclerEffect);

            AddExecutor(ExecutorType.Activate, CardId.SkyStrikerMechaHornetDrones, DronesEffect);

            AddExecutor(ExecutorType.Summon, CardId.JetSynchron, JetSynchronSummon);

            AddExecutor(ExecutorType.Activate, CardId.DestrudoTheLostDragonsFrisson, DestrudoSummon);

            AddExecutor(ExecutorType.SpSummon, CardId.CrystronNeedlefiber, NeedlefiberSummonFirst);
            AddExecutor(ExecutorType.Activate, CardId.CrystronNeedlefiber, NeedlefiberEffect);

            AddExecutor(ExecutorType.Activate, CardId.ShootingRiserDragon, ShootingRiserDragonEffect);

            AddExecutor(ExecutorType.Summon, CardId.TrickstarCandina, CandinaSummon);
            AddExecutor(ExecutorType.Activate, CardId.TrickstarCandina, CandinaEffect);

            AddExecutor(ExecutorType.Summon, CardId.JetSynchron, OneCardComboSummon);
            AddExecutor(ExecutorType.Summon, CardId.ThePhantomKnightsofAncientCloak, OneCardComboSummon);
            AddExecutor(ExecutorType.Summon, CardId.ThePhantomKnightsofSilentBoots, OneCardComboSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.SalamangreatAlmiraj, AlmirajSummon);

            AddExecutor(ExecutorType.Activate, CardId.ThePhantomKnightsofShadeBrigandine, ShadeBrigandineSummonFirst);

            AddExecutor(ExecutorType.SpSummon, CardId.KnightmarePhoenix, KnightmarePhoenixSummon);
            AddExecutor(ExecutorType.Activate, CardId.KnightmarePhoenix, KnightmarePhoenixEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.GalateaTheOrcustAutomaton, GalateaSummonFirst);

            AddExecutor(ExecutorType.Activate, CardId.JetSynchron, JetSynchronEffect);

            AddExecutor(ExecutorType.Activate, CardId.OrcustKnightmare, OrcustKnightmareEffect);

            AddExecutor(ExecutorType.Activate, CardId.OrcustHarpHorror, HarpHorrorEffect);

            AddExecutor(ExecutorType.Activate, CardId.WorldLegacyWorldWand, WorldWandEffect);

            AddExecutor(ExecutorType.Activate, CardId.ThePhantomKnightsofAncientCloak, AncientCloakEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.ThePhantomKnightsofRustyBardiche, RustyBardicheSummon);
            AddExecutor(ExecutorType.Activate, CardId.ThePhantomKnightsofRustyBardiche, RustyBardicheEffect);

            AddExecutor(ExecutorType.Activate, CardId.OrcustCymbalSkeleton, CymbalSkeletonEffect);

            AddExecutor(ExecutorType.Activate, CardId.GalateaTheOrcustAutomaton, GalateaEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.SheorcustDingirsu, SheorcustDingirsuSummon);
            AddExecutor(ExecutorType.Activate, CardId.SheorcustDingirsu, SheorcustDingirsuEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.ThePhantomKnightsofSilentBoots, SilentBootsSummon);
            AddExecutor(ExecutorType.Activate, CardId.ThePhantomKnightsofShadeBrigandine, ShadeBrigandineSummonSecond);

            AddExecutor(ExecutorType.SpSummon, CardId.BorreloadSavageDragon);
            AddExecutor(ExecutorType.Activate, CardId.BorreloadSavageDragon, BorreloadSavageDragonEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.GalateaTheOrcustAutomaton, GalateaSummonSecond);

            AddExecutor(ExecutorType.Activate, CardId.ThePhantomKnightsofSilentBoots, SilentBootsEffect);

            AddExecutor(ExecutorType.Summon, CardId.GhostBelleHauntedMansion, TunerSummon);
            AddExecutor(ExecutorType.Summon, CardId.AshBlossomJoyousSpring, TunerSummon);
            AddExecutor(ExecutorType.Summon, CardId.OrcustCymbalSkeleton, OtherSummon);
            AddExecutor(ExecutorType.Summon, CardId.OrcustHarpHorror, OtherSummon);
            AddExecutor(ExecutorType.Summon, CardId.ThePhantomKnightsofAncientCloak, LinkMaterialSummon);
            AddExecutor(ExecutorType.Summon, CardId.MaxxC, LinkMaterialSummon);
            AddExecutor(ExecutorType.Summon, CardId.ThePhantomKnightsofSilentBoots, LinkMaterialSummon);

            AddExecutor(ExecutorType.SpSummon, CardId.CrystronNeedlefiber, NeedlefiberSummonSecond);

            AddExecutor(ExecutorType.SpSummon, CardId.BorrelswordDragon, BorrelswordDragonSummon);
            AddExecutor(ExecutorType.Activate, CardId.BorrelswordDragon, BorrelswordDragonEffect);

            AddExecutor(ExecutorType.SpellSet, CardId.PhantomKnightsFogBlade);
            AddExecutor(ExecutorType.Activate, CardId.PhantomKnightsFogBlade, FogBladeEffect);
            AddExecutor(ExecutorType.SpellSet, CardId.OrcustratedClimax);

            AddExecutor(ExecutorType.Activate, CardId.OrcustratedBabel, BabelEffect);

            AddExecutor(ExecutorType.Repos, MonsterRepos);
        }

        private bool NormalSummoned = false;
        private bool SheorcustDingirsuSummoned = false;
        private bool HarpHorrorUsed = false;
        private bool CymbalSkeletonUsed = false;
        private bool BorrelswordDragonUsed = false;
        private ClientCard RustyBardicheTarget = null;

        private int[] HandCosts = new[]
        {
            CardId.OrcustCymbalSkeleton,
            CardId.OrcustKnightmare,
            CardId.DestrudoTheLostDragonsFrisson,
            CardId.WorldLegacyWorldWand,
            CardId.OrcustHarpHorror,
            CardId.ThePhantomKnightsofAncientCloak,
            CardId.ThePhantomKnightsofSilentBoots,
            CardId.JetSynchron,
            CardId.TrickstarLightStage,
            CardId.SkyStrikerMobilizeEngage,
            CardId.Terraforming,
            CardId.ReinforcementofTheArmy,
            CardId.MaxxC,
            CardId.GhostBelleHauntedMansion
        };

        public override bool OnSelectHand()
        {
            // go first
            return true;
        }

        public override void OnNewTurn()
        {
            NormalSummoned = false;
            SheorcustDingirsuSummoned = false;
            HarpHorrorUsed = false;
            CymbalSkeletonUsed = false;
            BorrelswordDragonUsed = false;
            RustyBardicheTarget = null;
            base.OnNewTurn();
        }

        public override void OnChainEnd()
        {
            RustyBardicheTarget = null;
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
                if (cardId == CardId.KnightmarePhoenix || cardId == CardId.CrystronNeedlefiber)
                {
                    ClientCard b = Bot.MonsterZone.GetFirstMatchingCard(card => card.Id == CardId.BorreloadSavageDragon);
                    int zone = (1 << (b?.Sequence ?? 0)) & available;
                    if (zone > 0)
                        return zone;
                }
                if ((available & Zones.z0) > 0)
                    return Zones.z0;
                if ((available & Zones.z1) > 0)
                    return Zones.z1;
                if ((available & Zones.z2) > 0)
                    return Zones.z2;
                if ((available & Zones.z3) > 0)
                    return Zones.z3;
                if ((available & Zones.z4) > 0)
                    return Zones.z4;
            }
            if (location == CardLocation.MonsterZone)
            {
                if (cardId == CardId.SheorcustDingirsu)
                {
                    ClientCard l = Bot.MonsterZone.GetFirstMatchingCard(card => card.Id == CardId.ThePhantomKnightsofRustyBardiche);
                    int zones = (l?.GetLinkedZones() ?? 0) & available;
                    if ((zones & Zones.z4) > 0)
                        return Zones.z4;
                    if ((zones & Zones.z3) > 0)
                        return Zones.z3;
                    if ((zones & Zones.z2) > 0)
                        return Zones.z2;
                    if ((zones & Zones.z1) > 0)
                        return Zones.z1;
                    if ((zones & Zones.z0) > 0)
                        return Zones.z0;
                }
                if (cardId == CardId.GalateaTheOrcustAutomaton)
                {
                    int zones = Bot.GetLinkedZones() & available;
                    if ((zones & Zones.z0) > 0)
                        return Zones.z0;
                    if ((zones & Zones.z2) > 0)
                        return Zones.z2;
                    if ((zones & Zones.z1) > 0)
                        return Zones.z1;
                    if ((zones & Zones.z3) > 0)
                        return Zones.z3;
                    if ((zones & Zones.z4) > 0)
                        return Zones.z4;
                }
                if (cardId == CardId.KnightmarePhoenix)
                {
                    if ((Enemy.MonsterZone[5]?.HasLinkMarker(CardLinkMarker.Top) ?? false) && (available & Zones.z3) > 0)
                        return Zones.z3;
                    if ((Enemy.MonsterZone[6]?.HasLinkMarker(CardLinkMarker.Top) ?? false) && (available & Zones.z1) > 0)
                        return Zones.z1;
                }

                if ((available & Zones.z6) > 0)
                    return Zones.z6;
                if ((available & Zones.z5) > 0)
                    return Zones.z5;
                if ((available & Zones.z1) > 0)
                    return Zones.z1;
                if ((available & Zones.z3) > 0)
                    return Zones.z3;
                if ((available & Zones.z0) > 0)
                    return Zones.z0;
                if ((available & Zones.z4) > 0)
                    return Zones.z4;
                if ((available & Zones.z2) > 0)
                    return Zones.z2;
            }
            return 0;
        }

        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            if (!defender.IsMonsterHasPreventActivationEffectInBattle())
            {
                if (attacker.IsCode(CardId.TrickstarCandina) && Bot.HasInHand(CardId.TrickstarCarobein))
                    attacker.RealPower = attacker.RealPower + 1800;

                if (attacker.IsCode(CardId.BorrelswordDragon) && !attacker.IsDisabled() && !BorrelswordDragonUsed)
                {
                    attacker.RealPower = attacker.RealPower + defender.GetDefensePower() / 2;
                    defender.RealPower = defender.RealPower - defender.GetDefensePower() / 2;
                }
            }
            return base.OnPreBattleBetween(attacker, defender);
        }

        private bool TerraformingEffect()
        {
            AI.SelectCard(CardId.TrickstarLightStage);
            return true;
        }

        private bool ReinforcementofTheArmyEffect()
        {
            AI.SelectCard(CardId.ArmageddonKnight);
            return true;
        }

        private bool FoolishBurialEffect()
        {
            AI.SelectCard(new[] {
                CardId.DestrudoTheLostDragonsFrisson,
                CardId.JetSynchron,
                CardId.OrcustHarpHorror,
                CardId.OrcustCymbalSkeleton
            });
            return true;
        }

        private bool LightStageEffect()
        {
            if (Card.Location == CardLocation.Hand || Card.IsFacedown())
            {
                ClientCard field = Bot.GetFieldSpellCard();
                if ((field?.IsCode(CardId.OrcustratedBabel) ?? false) && Bot.GetMonsterCount() > 1)
                    return false;
                if ((field?.IsCode(CardId.TrickstarLightStage) ?? false) && Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.TrickstarCandina) && Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.TrickstarCarobein))
                    return false;
                AI.SelectYesNo(true);
                if (Bot.HasInHandOrHasInMonstersZone(CardId.TrickstarCandina))
                    AI.SelectCard(CardId.TrickstarCarobein);
                else
                    AI.SelectCard(CardId.TrickstarCandina);
                return true;
            }
            ClientCard target = Enemy.SpellZone.GetFirstMatchingCard(card => card.IsFacedown());
            AI.SelectCard(target);
            return true;
        }

        private bool CarobeinSummon()
        {
            if (Bot.HasInMonstersZone(CardId.TrickstarCandina))
            {
                // TODO: beat mode
                return Bot.HasInExtra(CardId.KnightmarePhoenix);
            }
            else
            {
                return !NormalSummoned && Bot.Hand.IsExistingMatchingCard(card => card.Level <= 4);
            }
        }

        private bool EngageEffect()
        {
            bool needProtect = false;
            if (Bot.HasInHand(CardId.ArmageddonKnight))
                needProtect = true;
            else if (Bot.HasInHandOrInGraveyard(CardId.DestrudoTheLostDragonsFrisson) && Bot.Hand.IsExistingMatchingCard(card => card.Level <= 4))
                needProtect = true;
            else if (Bot.HasInHand(CardId.TrickstarCandina))
                needProtect = true;
            if (needProtect)
                AI.SelectCard(CardId.SkyStrikerMechaEagleBooster);
            else
                AI.SelectCard(CardId.SkyStrikerMechaHornetDrones);
            AI.SelectYesNo(true);
            return true;
        }

        private bool DronesEffectFirst()
        {
            return Bot.GetMonsterCount() == 0;
        }

        private bool DronesEffect()
        {
            return !Bot.HasInHand(CardId.ArmageddonKnight) && !Bot.HasInHand(CardId.TrickstarCandina);
        }

        private bool CandinaSummon()
        {
            NormalSummoned = true;
            return true;
        }

        private bool CandinaEffect()
        {
            AI.SelectCard(CardId.TrickstarLightStage);
            return true;
        }

        private bool ArmageddonKnightSummon()
        {
            NormalSummoned = true;
            return true;
        }

        private bool ArmageddonKnightEffect()
        {
            AI.SelectCard(new[] {
                CardId.DestrudoTheLostDragonsFrisson,
                CardId.OrcustHarpHorror
            });
            return true;
        }

        private bool ScrapRecyclerSummon()
        {
            NormalSummoned = true;
            return true;
        }

        private bool ScrapRecyclerEffect()
        {
            AI.SelectCard(new[] {
                CardId.JetSynchron,
                CardId.OrcustHarpHorror
            });
            return true;
        }

        private bool JetSynchronSummon()
        {
            if (Bot.GetMonsterCount() > 0)
            {
                NormalSummoned = true;
                return true;
            }
            return false;
        }

        private bool JetSynchronEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            AI.SelectCard(HandCosts);
            return true;
        }

        private bool AlmirajSummon()
        {
            if (Bot.GetMonsterCount() > 1)
                return false;
            ClientCard mat = Bot.GetMonsters().First();
            if (mat.IsCode(new[] {
                CardId.JetSynchron,
                CardId.ThePhantomKnightsofAncientCloak,
                CardId.ThePhantomKnightsofSilentBoots
            }))
            {
                AI.SelectMaterials(mat);
                return true;
            }
            return false;
        }

        private bool DestrudoSummon()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            return Bot.GetMonsterCount() < 3 && Bot.HasInExtra(new[] { CardId.CrystronNeedlefiber, CardId.KnightmarePhoenix });
        }

        private bool NeedlefiberSummonFirst()
        {
            if (!Bot.HasInExtra(CardId.BorreloadSavageDragon))
                return false;
            if (!Bot.HasInHand(CardId.JetSynchron) && Bot.GetRemainingCount(CardId.JetSynchron, 1) == 0)
                return false;

            int[] matids = new[] {
                CardId.DestrudoTheLostDragonsFrisson,
                CardId.AshBlossomJoyousSpring,
                CardId.GhostBelleHauntedMansion,
                CardId.SkyStrikerMechaHornetDronesToken,
                CardId.TrickstarCarobein,
                CardId.SkyStrikerAceKagari,
                CardId.ScrapRecycler,
                CardId.ArmageddonKnight,
                CardId.TrickstarCandina,
                CardId.OrcustHarpHorror,
                CardId.OrcustCymbalSkeleton,
                CardId.ThePhantomKnightsofAncientCloak,
                CardId.ThePhantomKnightsofSilentBoots
            };
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(matids)) >= 2)
            {
                AI.SelectMaterials(matids);
                return true;
            }
            return false;
        }

        private bool NeedlefiberSummonSecond()
        {
            IList<ClientCard> selected = new List<ClientCard>();

            ClientCard tuner = Bot.MonsterZone.GetFirstMatchingFaceupCard(card => card.IsCode(new[]
            {
                CardId.DestrudoTheLostDragonsFrisson,
                CardId.AshBlossomJoyousSpring,
                CardId.GhostBelleHauntedMansion,
                CardId.JetSynchron
            }));
            if (tuner != null)
                selected.Add(tuner);

            int[] matids = new[] {
                CardId.SkyStrikerMechaHornetDronesToken,
                CardId.ThePhantomKnightsofShadeBrigandine,
                CardId.SkyStrikerAceKagari,
                CardId.ScrapRecycler,
                CardId.ArmageddonKnight,
                CardId.OrcustHarpHorror,
                CardId.OrcustCymbalSkeleton,
                CardId.ThePhantomKnightsofAncientCloak,
                CardId.ThePhantomKnightsofSilentBoots
            };

            IList<ClientCard> mats = Bot.MonsterZone.GetMatchingCards(card => card.Attack <= 1700);

            for (int i = 0; i < matids.Length && selected.Count < 2; i++)
            {
                ClientCard c = mats.GetFirstMatchingFaceupCard(card => card.IsCode(matids[i]));
                if (c != null)
                {
                    selected.Add(c);
                    if (selected.Count == 2 && Util.GetBotAvailZonesFromExtraDeck(selected) == 0)
                        selected.Remove(c);
                }
            }

            if (selected.Count == 2)
            {
                AI.SelectMaterials(selected);
                return true;
            }
            return false;
        }

        private bool NeedlefiberEffect()
        {
            AI.SelectCard(CardId.JetSynchron);
            return true;
        }

        private bool ShootingRiserDragonEffect()
        {
            if (ActivateDescription == -1 || (ActivateDescription == Util.GetStringId(CardId.ShootingRiserDragon, 0)))
            {
                if (Bot.MonsterZone.IsExistingMatchingCard(card => card.Level == 3 && card.IsFaceup() && !card.IsTuner()) && Bot.GetRemainingCount(CardId.MaxxC, 3) > 0)
                {
                    AI.SelectCard(CardId.MaxxC);
                }
                else if (Bot.MonsterZone.IsExistingMatchingCard(card => card.Level == 4 && card.IsFaceup() && !card.IsTuner()))
                {
                    AI.SelectCard(new[] {
                        CardId.ThePhantomKnightsofAncientCloak,
                        CardId.ThePhantomKnightsofSilentBoots,
                        CardId.ScrapRecycler,
                        CardId.OrcustCymbalSkeleton,
                        CardId.AshBlossomJoyousSpring,
                        CardId.GhostBelleHauntedMansion
                    });
                }
                else if (Bot.MonsterZone.IsExistingMatchingCard(card => card.Level == 5 && card.IsFaceup() && !card.IsTuner()))
                {
                    AI.SelectCard(new[] {
                        CardId.OrcustHarpHorror,
                        CardId.ArmageddonKnight,
                        CardId.TrickstarCandina
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
                return Duel.LastChainPlayer != 0;
            }
        }

        private bool KnightmarePhoenixSummon()
        {
            if (!KnightmareMermaidSummon())
                return false;
            if (!Bot.HasInExtra(CardId.KnightmareMermaid))
                return false;

            int[] firstMats = new[] {
                CardId.JetSynchron,
                CardId.CrystronNeedlefiber,
                CardId.SkyStrikerMechaHornetDronesToken,
                CardId.ThePhantomKnightsofShadeBrigandine,
                CardId.ScrapRecycler,
                CardId.SkyStrikerAceKagari,
                CardId.ArmageddonKnight,
                CardId.TrickstarCandina,
                CardId.TrickstarCarobein
            };
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(firstMats)) >= 2)
            {
                AI.SelectMaterials(firstMats);
                return true;
            }
            int[] secondMats = new[] {
                CardId.OrcustCymbalSkeleton,
                CardId.OrcustHarpHorror,
                CardId.DestrudoTheLostDragonsFrisson,
                CardId.JetSynchron,
                CardId.AshBlossomJoyousSpring,
                CardId.GhostBelleHauntedMansion,
                CardId.ThePhantomKnightsofSilentBoots,
                CardId.ThePhantomKnightsofAncientCloak,
                CardId.MaxxC,
                CardId.SalamangreatAlmiraj
            };
            int[] mats = firstMats.Concat(secondMats).ToArray();
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(mats)) >= 2)
            {
                AI.SelectMaterials(mats);
                return true;
            }
            return false;
        }

        private bool KnightmarePhoenixEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            int costcount = Bot.Hand.GetMatchingCardsCount(card => card.IsCode(HandCosts));
            ClientCard target = Enemy.SpellZone.GetFloodgate();
            ClientCard anytarget = Enemy.SpellZone.GetFirstMatchingCard(card => !card.OwnTargets.Any(cont => cont.IsCode(CardId.TrickstarLightStage)));
            if ((costcount > 1 && anytarget != null) || (Bot.GetHandCount() > 1 && target != null))
            {
                AI.SelectCard(HandCosts);
                if (target == null)
                    target = anytarget;
                AI.SelectNextCard(target);
                return true;
            }
            return false;
        }

        private bool KnightmareMermaidSummon()
        {
            if (Bot.GetHandCount() == 0)
                return false;
            if (Bot.GetRemainingCount(CardId.OrcustKnightmare, 2) == 0)
                return false;
            AI.SelectPlace(Zones.ExtraMonsterZones);
            return true;
        }

        private bool KnightmareMermaidEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            AI.SelectCard(HandCosts);
            return true;
        }

        private bool GalateaSummonFirst()
        {
            // only summon with Mermaid and Orcust Knightmare
            IList<ClientCard> mats = Bot.MonsterZone.GetMatchingCards(card => card.IsCode(CardId.KnightmareMermaid, CardId.OrcustKnightmare));
            if (mats.Count >= 2)
            {
                AI.SelectMaterials(mats);
                return true;
            }
            return false;
        }

        private bool OrcustKnightmareEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            if (!Bot.HasInGraveyard(CardId.OrcustHarpHorror))
            {
                AI.SelectCard(Util.GetBestBotMonster());
                AI.SelectNextCard(CardId.OrcustHarpHorror);
                return true;
            }
            else if (!Bot.HasInGraveyard(CardId.WorldLegacyWorldWand) && Bot.GetRemainingCount(CardId.WorldLegacyWorldWand, 1) > 0)
            {
                AI.SelectCard(CardId.GalateaTheOrcustAutomaton);
                AI.SelectNextCard(CardId.WorldLegacyWorldWand);
                return true;
            }
            else if (!Bot.HasInGraveyard(CardId.OrcustCymbalSkeleton) && Bot.GetRemainingCount(CardId.OrcustCymbalSkeleton, 1) > 0 && Bot.HasInGraveyard(CardId.SheorcustDingirsu) && !SheorcustDingirsuSummoned)
            {
                AI.SelectCard(CardId.GalateaTheOrcustAutomaton);
                AI.SelectNextCard(CardId.OrcustCymbalSkeleton);
                return true;
            }
            return false;
        }

        private bool HarpHorrorEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            HarpHorrorUsed = true;
            AI.SelectCard(CardId.OrcustCymbalSkeleton);
            return true;
        }

        private bool WorldWandEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            AI.SelectCard(CardId.OrcustCymbalSkeleton);
            return true;
        }

        private bool RustyBardicheSummon()
        {
            //if (Bot.GetRemainingCount(CardId.ThePhantomKnightsofAncientCloak, 1) == 0 && Bot.GetRemainingCount(CardId.ThePhantomKnightsofSilentBoots, 1) == 0)
            //    return false;
            //if (Bot.GetRemainingCount(CardId.ThePhantomKnightsofShadeBrigandine, 1) == 0 && Bot.GetRemainingCount(CardId.PhantomKnightsFogBlade, 2) == 0)
            //    return false;
            IList<ClientCard> mats = Bot.MonsterZone.GetMatchingCards(card => card.IsCode(CardId.GalateaTheOrcustAutomaton));
            ClientCard mat2 = Bot.MonsterZone.GetMatchingCards(card => card.IsCode(CardId.OrcustCymbalSkeleton)).FirstOrDefault();
            if (mat2 != null)
                mats.Add(mat2);
            AI.SelectMaterials(mats);
            AI.SelectPlace(Zones.ExtraMonsterZones);
            return true;
        }

        private bool RustyBardicheEffect()
        {
            if (ActivateDescription == -1 || ActivateDescription == Util.GetStringId(CardId.ThePhantomKnightsofRustyBardiche, 0))
            {
                ClientCard target = GetFogBladeTarget();
                if (target == null)
                    target = Util.GetBestEnemyCard(false, true);
                if (target == null)
                    return false;
                RustyBardicheTarget = target;
                AI.SelectCard(target);
                return true;
            }
            else
            {
                AI.SelectCard(CardId.ThePhantomKnightsofAncientCloak);
                if (Bot.HasInMonstersZone(CardId.JetSynchron) && !Bot.MonsterZone.IsExistingMatchingCard(card => card.Level == 4))
                    AI.SelectNextCard(CardId.ThePhantomKnightsofShadeBrigandine);
                else
                    AI.SelectNextCard(CardId.PhantomKnightsFogBlade);
                return true;
            }
        }

        private ClientCard GetFogBladeTarget()
        {
            return Enemy.MonsterZone.GetFirstMatchingCard(card => card.OwnTargets.Any(cont => cont.IsCode(CardId.PhantomKnightsFogBlade)));
        }

        private bool CymbalSkeletonEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            int[] botTurnTargets = new[] { CardId.GalateaTheOrcustAutomaton, CardId.SheorcustDingirsu };
            int[] emenyTurnTargets = new[] { CardId.SheorcustDingirsu, CardId.GalateaTheOrcustAutomaton };
            if (Duel.Player == 0 && Bot.HasInGraveyard(CardId.GalateaTheOrcustAutomaton) && !Bot.HasInMonstersZone(CardId.GalateaTheOrcustAutomaton) && Bot.HasInExtra(CardId.SheorcustDingirsu) && !SheorcustDingirsuSummoned)
            {
                AI.SelectCard(botTurnTargets);
                CymbalSkeletonUsed = true;
                return true;
            }
            else if (Duel.Player == 0 && Bot.HasInGraveyard(CardId.SheorcustDingirsu) && !SheorcustDingirsuSummoned)
            {
                AI.SelectCard(emenyTurnTargets);
                SheorcustDingirsuSummoned = true;
                CymbalSkeletonUsed = true;
                return true;
            }
            if (Duel.Player == 1 && Bot.HasInGraveyard(CardId.SheorcustDingirsu) && !SheorcustDingirsuSummoned &&
                (Util.GetProblematicEnemyCard() != null || Duel.Phase == DuelPhase.End))
            {
                AI.SelectCard(emenyTurnTargets);
                CymbalSkeletonUsed = true;
                SheorcustDingirsuSummoned = true;
                return true;
            }
            return false;
        }

        private bool SheorcustDingirsuSummon()
        {
            SheorcustDingirsuSummoned = true;
            return true;
        }

        private bool SheorcustDingirsuEffect()
        {
            if (ActivateDescription == 96)
            {
                // TODO: more FogBlade lost target
                if ((Duel.Phase == DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2) && Duel.CurrentChain.Count == 0)
                    return false;
                AI.SelectCard(CardId.OrcustCymbalSkeleton);
                return true;
            }
            ClientCard target;
            target = GetFogBladeTarget();
            if (target != null && target != RustyBardicheTarget)
            {
                AI.SelectOption(0);
                AI.SelectCard(target);
                return true;
            }
            target = Util.GetProblematicEnemyMonster();
            if (target != null && target != RustyBardicheTarget)
            {
                AI.SelectOption(0);
                AI.SelectCard(target);
                return true;
            }
            target = Util.GetProblematicEnemySpell();
            if (target != null && target != RustyBardicheTarget)
            {
                AI.SelectOption(0);
                AI.SelectCard(target);
                return true;
            }
            if (Bot.HasInBanished(CardId.OrcustCymbalSkeleton))
            {
                AI.SelectOption(1);
                AI.SelectCard(CardId.OrcustCymbalSkeleton);
                return true;
            }
            target = Enemy.MonsterZone.GetFirstMatchingCard(card => card != RustyBardicheTarget) ?? Enemy.SpellZone.GetFirstMatchingCard(card => card != RustyBardicheTarget);
            if (target != null)
            {
                AI.SelectOption(0);
                AI.SelectCard(target);
                return true;
            }
            AI.SelectOption(1);
            //AI.SelectCard(); any card
            return true;
        }

        private bool AncientCloakEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            if (Bot.HasInMonstersZone(CardId.SalamangreatAlmiraj) && Bot.HasInExtra(CardId.KnightmarePhoenix))
                AI.SelectCard(CardId.ThePhantomKnightsofShadeBrigandine);
            else
                AI.SelectCard(CardId.ThePhantomKnightsofSilentBoots);
            return true;
        }

        private bool SilentBootsSummon()
        {
            return true;
        }

        private bool SilentBootsEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            if (Bot.HasInMonstersZone(CardId.SalamangreatAlmiraj) && Bot.HasInExtra(CardId.KnightmarePhoenix))
                AI.SelectCard(CardId.ThePhantomKnightsofShadeBrigandine);
            else
                AI.SelectCard(CardId.PhantomKnightsFogBlade);
            return true;
        }

        private bool ShadeBrigandineSummonSecond()
        {
            if (DefaultOnBecomeTarget())
                return true;
            return (Bot.HasInMonstersZone(CardId.SalamangreatAlmiraj) && Bot.HasInExtra(CardId.KnightmarePhoenix)) ||
                (Bot.HasInMonstersZone(CardId.JetSynchron) && Bot.HasInMonstersZone(CardId.ThePhantomKnightsofSilentBoots));
        }

        private bool GalateaSummonSecond()
        {
            if (!Util.IsTurn1OrMain2())
                return false;

            if (Bot.HasInMonstersZone(CardId.GalateaTheOrcustAutomaton))
                return false;

            IList<ClientCard> selected = new List<ClientCard>();

            if (!Bot.HasInGraveyard(CardId.SheorcustDingirsu))
            {
                ClientCard sheorcustDingirsu = Bot.MonsterZone.GetFirstMatchingFaceupCard(card => card.IsCode(CardId.SheorcustDingirsu));
                if (sheorcustDingirsu != null)
                    selected.Add(sheorcustDingirsu);
            }

            int[] matids = new[] {
                CardId.OrcustKnightmare,
                CardId.ThePhantomKnightsofSilentBoots,
                CardId.ThePhantomKnightsofAncientCloak,
                CardId.OrcustCymbalSkeleton,
                CardId.OrcustHarpHorror,
                CardId.ScrapRecycler,
                CardId.CrystronNeedlefiber,
                CardId.SkyStrikerAceKagari,
                CardId.KnightmareMermaid,
                CardId.ArmageddonKnight
            };

            IList<ClientCard> mats = Bot.MonsterZone.GetMatchingCards(card => card.Level > 0 && card.Level <= 7);

            for (int i = 0; i < matids.Length && selected.Count < 2; i++)
            {
                ClientCard c = mats.GetFirstMatchingFaceupCard(card => card.IsCode(matids[i]));
                if (c != null)
                {
                    selected.Add(c);
                    if (selected.Count == 2 && Util.GetBotAvailZonesFromExtraDeck(selected) == 0)
                        selected.Remove(c);
                }
            }

            if (selected.Count == 2)
            {
                AI.SelectMaterials(selected);
                return true;
            }

            return false;
        }

        private bool GalateaEffect()
        {
            if (Duel.Player == 0)
            {
                AI.SelectCard(CardId.OrcustKnightmare);
                AI.SelectNextCard(CardId.OrcustratedBabel);
            }
            if (Duel.Player == 1)
            {
                AI.SelectCard(CardId.OrcustKnightmare);
                AI.SelectNextCard(CardId.OrcustratedClimax);
            }
            return true;
        }

        private bool BorrelswordDragonSummon()
        {
            if (Util.IsTurn1OrMain2())
                return false;

            List<ClientCard> mats = Bot.MonsterZone.GetMatchingCards(card => card.IsFaceup() && card.HasType(CardType.Effect) && card.Attack <= 2000).ToList();
            mats.Sort(CardContainer.CompareCardAttack);
            mats.Reverse();

            int link = 0;
            bool doubleused = false;
            IList<ClientCard> selected = new List<ClientCard>();
            foreach (ClientCard card in mats)
            {
                selected.Add(card);
                if (!doubleused && card.LinkCount == 2)
                {
                    doubleused = true;
                    link += 2;
                }
                else
                    link++;
                if (link >= 4)
                    break;
            }

            if (link >= 4 && Util.GetBotAvailZonesFromExtraDeck(selected) > 0)
            {
                AI.SelectMaterials(selected);
                return true;
            }
            return false;
        }

        private bool BorrelswordDragonEffect()
        {
            if (ActivateDescription == -1 || ActivateDescription == Util.GetStringId(CardId.BorrelswordDragon, 1))
            {
                BorrelswordDragonUsed = true;
                return true;
            }
            else
            {
                if (Duel.Player == 0 && (Duel.Turn == 1 || Duel.Phase >= DuelPhase.Main2))
                {
                    return false;
                }
                ClientCard target = Bot.MonsterZone.GetFirstMatchingCard(card => card.IsAttack() && !card.HasType(CardType.Link) && card.Attacked && !card.IsShouldNotBeTarget());
                if (target != null)
                {
                    AI.SelectCard(target);
                    return true;
                }
                if (!Bot.MonsterZone.IsExistingMatchingCard(card => card.IsAttack() && !card.HasType(CardType.Link)))
                {
                    target = Enemy.MonsterZone.GetFirstMatchingCard(card => card.IsAttack() && !card.HasType(CardType.Link) && !card.IsShouldNotBeTarget());
                    if (target != null)
                    {
                        AI.SelectCard(target);
                        return true;
                    }
                }
                return false;
            }
        }

        private bool BabelEffect()
        {
            if (Card.Location == CardLocation.Grave)
            {
                if (DefaultCheckWhetherCardIsNegated(Card)) return false;
                IList<ClientCard> costCards = Bot.Hand.GetMatchingCards(card => card.IsCode(HandCosts));
                if (costCards.Count > 0)
                {
                    AI.SelectCard(HandCosts);
                    return true;
                }
                return false;
            }
            return Bot.HasInMonstersZoneOrInGraveyard(new[] {
                CardId.OrcustCymbalSkeleton,
                CardId.OrcustHarpHorror,
                CardId.OrcustKnightmare,
                CardId.GalateaTheOrcustAutomaton,
                CardId.LongirsuTheOrcustOrchestrator,
                CardId.SheorcustDingirsu
            });
        }

        private bool ShadeBrigandineSummonFirst()
        {
            return Bot.GetMonsterCount() < 2;
        }

        private bool OneCardComboSummon()
        {
            if (Bot.HasInExtra(CardId.SalamangreatAlmiraj) && Bot.HasInExtra(new[] { CardId.CrystronNeedlefiber, CardId.KnightmarePhoenix }) && Bot.GetMonsterCount() < 3)
            {
                NormalSummoned = true;
                return true;
            }
            return false;
        }

        private bool LinkMaterialSummon()
        {
            if (Bot.HasInExtra(CardId.KnightmarePhoenix) && Bot.GetMonsterCount() > 0 && Bot.GetMonsterCount() < 3)
            {
                NormalSummoned = true;
                return true;
            }
            return false;
        }

        private bool TunerSummon()
        {
            if (Bot.HasInExtra(new[] { CardId.CrystronNeedlefiber, CardId.KnightmarePhoenix }) && Bot.GetMonsterCount() > 0 && Bot.GetMonsterCount() < 3)
            {
                NormalSummoned = true;
                return true;
            }
            return false;
        }

        private bool OtherSummon()
        {
            NormalSummoned = true;
            return true;
        }

        private bool BorreloadSavageDragonEffect()
        {
            if (Duel.CurrentChain.Count == 0)
            {
                AI.SelectCard(new[] { CardId.KnightmarePhoenix, CardId.CrystronNeedlefiber });
                return true;
            }
            else
            {
                return true;
            }
        }

        private bool FogBladeEffect()
        {
            if (Card.Location == CardLocation.SpellZone)
            {
                return !Util.HasChainedTrap(0) && DefaultDisableMonster();
            }
            else if (Bot.HasInGraveyard(CardId.ThePhantomKnightsofRustyBardiche) || Bot.GetMonsterCount() < 2)
            {
                if (DefaultCheckWhetherCardIsNegated(Card)) return false;
                AI.SelectCard(CardId.ThePhantomKnightsofRustyBardiche);
                return true;
            }
            return false;
        }

        private bool ClimaxEffect()
        {
            if (Card.Location == CardLocation.SpellZone)
            {
                return Duel.LastChainPlayer == 1;
            }
            else if (Duel.Phase == DuelPhase.End)
            {
                ClientCard target = null;
                target = Bot.Banished.GetFirstMatchingFaceupCard(card=>card.IsCode(CardId.OrcustCymbalSkeleton));
                if (target == null)
                    target = Bot.Banished.GetFirstMatchingFaceupCard(card => card.IsCode(CardId.OrcustHarpHorror));
                if (target != null)
                {
                    AI.SelectCard(target);
                    return true;
                }
                if(!Bot.HasInHand(CardId.OrcustHarpHorror) && Bot.GetRemainingCount(CardId.OrcustHarpHorror, 2) > 1)
                {
                    AI.SelectCard(CardId.OrcustHarpHorror);
                    return true;
                }
            }
            return false;
        }

        private bool EagleBoosterEffect()
        {
            if (Duel.LastChainPlayer != 1)
                return false;
            ClientCard target = Bot.GetMonstersInExtraZone().GetFirstMatchingCard(
                card => Duel.CurrentChain.Contains(card) || card.IsCode(CardId.KnightmareMermaid));
            if (target != null)
            {
                AI.SelectCard(target);
                return true;
            }
            return false;
        }

        private bool MonsterRepos()
        {
            if (Card.IsFacedown())
                return true;
            return DefaultMonsterRepos();
        }
    }
}
