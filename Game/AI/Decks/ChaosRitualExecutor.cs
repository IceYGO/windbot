using System;
using System.Collections.Generic;
using System.Linq;
using YGOSharp.OCGWrapper;
using YGOSharp.OCGWrapper.Enums;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("ChaosRitual", "AI_ChaosRitual")]
    class ChaosRitualExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int TheWorldsGreatestGallantThief = 24203749;
            public const int BlackSkullDragonTheArchfiendDragonOfUnity = 97818130;
            public const int BlackChaos = 98684220;
            public const int FydraulisHarmonia = 70088809;
            public const int SkullArchfiendOfChaos = 24088928;
            public const int FallenOfTheWhiteDragon = 73819701;
            public const int IncredibleEcclesiaTheVirtuous = 55273560;
            public const int CelticMystic = 50073633;
            public const int MulcharmyPurulia = 84192580;
            public const int AshBlossomJoyousSpring = 14558127;
            public const int MaxxC = 23434538;
            public const int Griffoh = 97462632;
            public const int DrollLockBird = 94145021;
            public const int BlackLusterSoldierSoldierOfLightAndDarkness = 70405001;
            public const int MagicianOfDarkChaosBlackChaos = 44001993;
            public const int SpatialTrunade = 2729965;
            public const int RaggedRecordsOfRites = 24461358;
            public const int CrimsonCall = 99398682;
            public const int LightAndDarknessRitual = 33599853;
            public const int TheFallenTheVirtuous = 30271097;
            public const int SpellShatteringSword = 77456448;
            public const int MindShuffle = 24749710;
            public const int BlackRoseDragon = 73580471;
            public const int HarpiesFeatherDuster = 18144506;
            public const int Raigeki = 12580477;
            public const int LightningStorm = 14532163;

            public const int AlbaLenatusTheAbyssDragon = 3410461;
            public const int TitanikladTheAshDragon = 41373230;
            public const int AlbionTheBrandedDragon = 87746184;
            public const int RedNovaDragonBurningSoul = 65541655;
            public const int PsychicEndPunisher = 60465049;
            public const int ChaosAngel = 22850702;
            public const int FiendsmithsRequiem = 2463794;
            public const int TheCrimsonKing = 67809530;
            public const int RedDragonArchfiend = 70902743;
            public const int EnigmasterPackbit = 72444406;
            public const int EcclesiaAndTheDarkDragon = 78397661;
            public const int StardustDragonVictimSanctuary = 76636978;
            public const int WindPegasusIgnister = 98506199;
            public const int GoldenCloudBeastMalong = 93125329;
            public const int HeraldOfTheArcLight = 79606837;
        }

        private bool _selectingGallantThiefTributes;
        private bool _ownMonsterReleasedToGraveThisTurn;
        private bool _mindShufflePriorityOverrideApplied;
        private int _pendingMindShuffleSummonId;
        private readonly HashSet<ClientCard> _pendingMindShuffleReturnCards =
            new HashSet<ClientCard>();
        private bool _performedRitualSummonThisTurn;
        private readonly HashSet<ClientCard> _pendingReleaseCards =
            new HashSet<ClientCard>();
        private bool _enemyMaxxCResolved;
        private int _ritualSummonCountThisTurn;
        // Optional trigger prompts can arrive with a generic description. Keep
        // the successful normal-summon window separate from Celtic Mystic's
        // later ignition effect so the two effects cannot borrow each other's
        // strategic conditions.
        private bool _celticMysticDrawTriggerPending;
        // When Celtic Mystic is Special Summoned while another chain is
        // resolving, its summon-success trigger is offered only after that
        // chain ends. Preserve the pending flag across exactly that one chain
        // end; later stale windows may still clear it normally.
        private bool _preserveCelticMysticDrawTriggerAtChainEnd;
        // SelectUnselect asks for one material at a time. Once the current
        // selection is finishable, returning an empty selection confirms it;
        // without this state the executor can keep adding Graveyard monsters
        // after Kuriboh Guardian has already fulfilled the entire requirement.
        private bool _selectingLightAndDarknessRitualMaterials;
        // Black Chaos's Special Summon procedure is chainless. Restrict its
        // ToDeck selector to the action which actually armed it, otherwise an
        // unrelated chainless return-to-Deck prompt can consume this policy.
        private bool _selectingBlackChaosSpecialSummonReturn;
        // Spell Shattering Sword must directly follow the opponent's latest
        // chain link. Preserve that exact field monster through its option and
        // target prompts instead of scanning backwards through older links.
        private ClientCard _pendingSpellShatteringSwordMonsterTarget;
        private bool _pendingSpellShatteringSwordSpellDestroy;
        // Fydraulis reveals first and sends one of those revealed Synchros
        // later. Keep the intended payload name stable across both prompts.
        private int _pendingFydraulisSynchroToGraveId;
        // Fydraulis chooses the monster only while its non-targeting effect is
        // resolving. Reserve its best current target strategically when the
        // activation is accepted, so a later copy of The Fallen in the same
        // chain does not destroy the only useful monster first.
        private ClientCard _pendingFydraulisDestructionTarget;
        // Light and Darkness Ritual's Graveyard effect selects its second
        // return card while the card is still in the Graveyard. Keep that
        // selection reserved until the chain ends so Skull Archfiend cannot
        // activate its Graveyard effect in response and Special Summon itself
        // before the return resolves.
        private readonly HashSet<ClientCard> _pendingLightAndDarknessReturnCards =
            new HashSet<ClientCard>();
        private readonly HashSet<ClientCard> _reservedOpponentTargets =
            new HashSet<ClientCard>();
        // A search/set selection is made before the selected card necessarily
        // reaches the Hand or Spell Zone. Keep the card name reserved until the
        // current chain ends so another resolving effect cannot select the same
        // support card from the stale deck view.
        private readonly HashSet<int> _pendingDeckSearchIds =
            new HashSet<int>();
        private bool _pendingBlackChaosSupportSearch;
        // Keep a local count for Mind Shuffle copies that have entered our
        // Spell Zone. A few server/client message sequences leave the local
        // SpellZone card object temporarily stale after the card resolves;
        // the count prevents a later search effect from selecting a second
        // copy during that window.
        private int _mindShuffleFieldCount;
        private readonly HashSet<int> _activatedFirstEffectCardIdsThisTurn =
            new HashSet<int>();
        private readonly List<int> _mindShuffleSummonOrder = new List<int>
        {
            CardId.MagicianOfDarkChaosBlackChaos,
            CardId.BlackSkullDragonTheArchfiendDragonOfUnity,
            CardId.BlackLusterSoldierSoldierOfLightAndDarkness,
            CardId.BlackChaos
        };
        private readonly List<int> _mindShuffleReturnOrder = new List<int>
        {
            CardId.FydraulisHarmonia,
            CardId.BlackSkullDragonTheArchfiendDragonOfUnity,
            CardId.BlackLusterSoldierSoldierOfLightAndDarkness,
            CardId.MagicianOfDarkChaosBlackChaos,
            CardId.BlackChaos
        };

        public ChaosRitualExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // The main route is to establish a ritual spell and a ritual
            // monster before spending the normal summon on a value monster.
            AddExecutor(ExecutorType.Summon, CardId.TheWorldsGreatestGallantThief,
                GallantThiefSummon);
            AddExecutor(ExecutorType.Summon, CardId.CelticMystic,
                CelticMysticSummon);
            // Alba Lenatus can only use our White Dragon and opposing Dragons.
            // When that exact material route is available, use it before the
            // other optional Extra Deck routes.
            AddExecutor(ExecutorType.SpSummon, CardId.AlbaLenatusTheAbyssDragon,
                AlbaLenatusSpecialSummon);
            // Spatial Trunade is a non-targeting field reset and has priority
            // over Ecclesia's optional special summon.
            AddExecutor(ExecutorType.Activate, CardId.SpatialTrunade,
                SpatialTrunadeActivate);
            // These Synchro routes are preferred whenever the server reports
            // that their own summon requirements are currently legal.
            AddExecutor(ExecutorType.SpSummon, CardId.ChaosAngel,
                PriorityExtraDeckSpecialSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.RedNovaDragonBurningSoul,
                PriorityExtraDeckSpecialSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.StardustDragonVictimSanctuary,
                StardustDragonSpecialSummon);
            AddExecutor(ExecutorType.Activate, CardId.StardustDragonVictimSanctuary,
                StardustDragonActivate);
            // Optional effects of the remaining Extra Deck monsters. The
            // server has already verified the exact timing and legal targets;
            // these callbacks only decide whether the effect is worth taking.
            AddExecutor(ExecutorType.Activate, CardId.TitanikladTheAshDragon,
                TitanikladActivate);
            AddExecutor(ExecutorType.Activate, CardId.AlbionTheBrandedDragon,
                AlbionActivate);
            AddExecutor(ExecutorType.Activate, CardId.RedNovaDragonBurningSoul,
                RedNovaDragonActivate);
            AddExecutor(ExecutorType.Activate, CardId.PsychicEndPunisher,
                PsychicEndPunisherActivate);
            AddExecutor(ExecutorType.Activate, CardId.ChaosAngel,
                ChaosAngelActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.TheCrimsonKing,
                CrimsonKingSpecialSummon);
            AddExecutor(ExecutorType.Activate, CardId.TheCrimsonKing,
                CrimsonKingActivate);
            AddExecutor(ExecutorType.Activate, CardId.EnigmasterPackbit,
                EnigmasterPackbitActivate);
            AddExecutor(ExecutorType.Activate, CardId.EcclesiaAndTheDarkDragon,
                EcclesiaAndTheDarkDragonActivate);
            AddExecutor(ExecutorType.Activate, CardId.WindPegasusIgnister,
                WindPegasusIgnisterActivate);
            AddExecutor(ExecutorType.Activate, CardId.GoldenCloudBeastMalong,
                GoldenCloudBeastMalongActivate);
            AddExecutor(ExecutorType.Activate, CardId.HeraldOfTheArcLight,
                HeraldOfTheArcLightActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.IncredibleEcclesiaTheVirtuous,
                EcclesiaSpecialSummon);

            // Hand traps and effects which can interrupt without consuming the
            // ritual engine.
            AddExecutor(ExecutorType.Activate, CardId.MaxxC, MaxxCActivate);
            AddExecutor(ExecutorType.Activate, CardId.MulcharmyPurulia, PuruliaActivate);
            AddExecutor(ExecutorType.Activate, CardId.AshBlossomJoyousSpring,
                AshBlossomActivate);
            AddExecutor(ExecutorType.Activate, CardId.DrollLockBird, DrollLockBirdActivate);
            AddExecutor(ExecutorType.Activate, CardId.TheWorldsGreatestGallantThief,
                GallantThiefActivate);

            // Fydraulis is a hand response to an opponent field monster effect.
            AddExecutor(ExecutorType.Activate, CardId.FydraulisHarmonia,
                FydraulisHarmoniaActivate);
            AddExecutor(ExecutorType.Activate, CardId.BlackSkullDragonTheArchfiendDragonOfUnity,
                BlackSkullDragonFieldActivate);
            // Psychic End Punisher should be accepted as a Synchro summon
            // before the lower-priority hand discard effect of Black Chaos.
            AddExecutor(ExecutorType.SpSummon, CardId.PsychicEndPunisher,
                PsychicEndPunisherSpecialSummon);
            // Celtic Mystic's draw-three effect is intentionally checked
            // before Black Chaos's hand discard effect. Its normal summon is
            // also protected by ShouldSummonCelticMysticFirst().
            AddExecutor(ExecutorType.Activate, CardId.CelticMystic,
                CelticMysticSearchActivate);
            // Keep Black Chaos's field effect after Griffoh so only the
            // requested hand-effect priority is changed.
            AddExecutor(ExecutorType.Activate, CardId.BlackChaos,
                BlackChaosHandActivate);
            AddExecutor(ExecutorType.Activate, CardId.Griffoh, GriffohActivate);
            AddExecutor(ExecutorType.Activate, CardId.BlackChaos,
                BlackChaosFieldActivate);
            AddExecutor(ExecutorType.Activate, CardId.BlackLusterSoldierSoldierOfLightAndDarkness,
                BlackLusterSoldierActivate);

            // Ritual monsters and secondary resources.
            AddExecutor(ExecutorType.Activate, CardId.IncredibleEcclesiaTheVirtuous,
                EcclesiaActivate);
            // Ecclesia's field/grave effects are preferred before the White
            // Dragon route; her hand special summon has its own earlier
            // executor and is intentionally not moved by this ordering.
            AddExecutor(ExecutorType.Activate, CardId.FallenOfTheWhiteDragon,
                FallenOfTheWhiteDragonActivate);
            // A legal Graveyard effect of Light and Darkness Ritual must be
            // considered before Magician of Dark Chaos can retrieve it. This
            // matters after Celtic Mystic has entered the Graveyard and made
            // the Ritual's second recovery card legal. Only the Graveyard
            // route is moved; the hand/field Ritual route keeps its old order.
            AddExecutor(ExecutorType.Activate, CardId.LightAndDarknessRitual,
                LightAndDarknessRitualGraveActivate);
            AddExecutor(ExecutorType.Activate, CardId.MagicianOfDarkChaosBlackChaos,
                MagicianOfDarkChaosActivate);
            AddExecutor(ExecutorType.Activate, CardId.TheFallenTheVirtuous,
                TheFallenTheVirtuousActivate);
            AddExecutor(ExecutorType.Activate, CardId.SpellShatteringSword,
                SpellShatteringSwordActivate);
            // The hand special summon of Black Skull Dragon deliberately comes
            // after Spell Shattering Sword, so it is not spent too early.
            AddExecutor(ExecutorType.Activate, CardId.BlackSkullDragonTheArchfiendDragonOfUnity,
                BlackSkullDragonHandActivate);
            // Black Chaos's own special-summon procedure is deliberately later
            // than the Black Skull Dragon route.
            AddExecutor(ExecutorType.SpSummon, CardId.BlackChaos,
                BlackChaosSpecialSummon);
            // Search and draw engine.
            AddExecutor(ExecutorType.Activate, CardId.RaggedRecordsOfRites,
                RaggedRecordsOfRitesActivate);
            AddExecutor(ExecutorType.Activate, CardId.MindShuffle, MindShuffleActivate);
            AddExecutor(ExecutorType.Activate, CardId.CrimsonCall, CrimsonCallActivate);
            // Hand and Spell/Trap Zone effects retain their existing priority.
            AddExecutor(ExecutorType.Activate, CardId.LightAndDarknessRitual,
                LightAndDarknessRitualActivate);

            AddExecutor(ExecutorType.Activate, CardId.SkullArchfiendOfChaos,
                SkullArchfiendOfChaosActivate);
            // The tribute effect that Special Summons a Ritual Monster must
            // remain later than Light and Darkness Ritual. It is intentionally
            // separate from Celtic Mystic's earlier draw-three effect.
            AddExecutor(ExecutorType.Activate, CardId.CelticMystic,
                CelticMysticRitualSpecialSummonActivate);

            AddExecutor(ExecutorType.SpellSet, ChaosSpellSet);
            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
        }

        public override bool OnSelectHand()
        {
            return true;
        }

        public override bool OnSelectYesNo(int desc)
        {
            return base.OnSelectYesNo(desc);
        }

        public override void OnNewTurn()
        {
            _selectingGallantThiefTributes = false;
            _ownMonsterReleasedToGraveThisTurn = false;
            _mindShufflePriorityOverrideApplied = false;
            _pendingMindShuffleSummonId = 0;
            _pendingMindShuffleReturnCards.Clear();
            _performedRitualSummonThisTurn = false;
            _enemyMaxxCResolved = false;
            _ritualSummonCountThisTurn = 0;
            _celticMysticDrawTriggerPending = false;
            _preserveCelticMysticDrawTriggerAtChainEnd = false;
            _selectingLightAndDarknessRitualMaterials = false;
            _selectingBlackChaosSpecialSummonReturn = false;
            _pendingSpellShatteringSwordMonsterTarget = null;
            _pendingSpellShatteringSwordSpellDestroy = false;
            _pendingFydraulisSynchroToGraveId = 0;
            _pendingFydraulisDestructionTarget = null;
            _pendingReleaseCards.Clear();
            _pendingLightAndDarknessReturnCards.Clear();
            _reservedOpponentTargets.Clear();
            _pendingDeckSearchIds.Clear();
            _pendingBlackChaosSupportSearch = false;
            _mindShuffleFieldCount = 0;
            _activatedFirstEffectCardIdsThisTurn.Clear();
            ResetMindShuffleSummonOrder();
            ResetMindShuffleReturnOrder();
            _mindShuffleFieldCount = Bot.SpellZone.Count(c => c != null &&
                c.IsCode(CardId.MindShuffle));
            base.OnNewTurn();
        }

        public override ClientCard OnSelectAttacker(IList<ClientCard> attackers, IList<ClientCard> defenders)
        {
            ClientCard blackLusterSoldier = attackers.FirstOrDefault(c =>
                c != null && c.IsCode(CardId.BlackLusterSoldierSoldierOfLightAndDarkness));
            if (blackLusterSoldier != null)
                return blackLusterSoldier;

            return base.OnSelectAttacker(attackers, defenders);
        }

        public override BattlePhaseAction OnBattle(IList<ClientCard> attackers, IList<ClientCard> defenders)
        {
            // GameAI normally attacks with the lowest-power monster first when
            // the opponent has no monsters. Keep the Light and Darkness Soldier
            // as the first attacker for direct attacks as well.
            if (defenders.Count == 0)
            {
                ClientCard blackLusterSoldier = attackers.FirstOrDefault(c =>
                    c != null && c.IsCode(CardId.BlackLusterSoldierSoldierOfLightAndDarkness));
                if (blackLusterSoldier != null && blackLusterSoldier.CanDirectAttack)
                    return AI.Attack(blackLusterSoldier, null);
            }

            return base.OnBattle(attackers, defenders);
        }

        public override BattlePhaseAction OnSelectAttackTarget(ClientCard attacker,
            IList<ClientCard> defenders)
        {
            if (attacker != null &&
                attacker.IsCode(CardId.BlackLusterSoldierSoldierOfLightAndDarkness))
            {
                foreach (ClientCard defender in defenders)
                {
                    attacker.RealPower = attacker.GetAttackPower();
                    defender.RealPower = defender.GetDefensePower();
                    if (!OnPreBattleBetween(attacker, defender))
                        continue;

                    // The Soldier is allowed to attack an attack-position
                    // monster with equal ATK. Strictly stronger attacks keep
                    // the normal behaviour for face-down/defence-position cards.
                    if (attacker.RealPower > defender.RealPower ||
                        attacker.RealPower == defender.RealPower && defender.IsAttack())
                    {
                        return AI.Attack(attacker, defender);
                    }
                }

                if (attacker.CanDirectAttack)
                    return AI.Attack(attacker, null);
                return null;
            }

            return base.OnSelectAttackTarget(attacker, defenders);
        }

        public override void OnChaining(int player, ClientCard card)
        {
            if (player == 0 && card != null && card.IsCode(CardId.CelticMystic) &&
                _celticMysticDrawTriggerPending)
            {
                // The summon-success effect has now actually been activated.
                // Track it immediately even if the protocol used description
                // -1, then release the pending trigger context.
                _activatedFirstEffectCardIdsThisTurn.Add(card.Id);
                _celticMysticDrawTriggerPending = false;
                _preserveCelticMysticDrawTriggerAtChainEnd = false;
            }

            if (player == 0 && card != null && card.Id != 0 &&
                !IsMultiEffectCard(card))
            {
                // Single-effect cards can be recorded at activation time.
                _activatedFirstEffectCardIdsThisTurn.Add(card.Id);
            }

            base.OnChaining(player, card);
        }

        public override void OnChainSolved(int chainIndex)
        {
            ChainInfo currentChain = Duel.GetCurrentSolvingChainInfo();
            if (currentChain != null && currentChain.ActivatePlayer == 0 &&
                IsFirstEffect(currentChain))
            {
                // Multi-effect cards are resolved here because the activation
                // callback does not expose the packet's effect description.
                // This remains activation tracking: a negated activation still
                // counts as having been used this turn.
                _activatedFirstEffectCardIdsThisTurn.Add(currentChain.ActivateId);
            }

            if (currentChain != null && !Duel.IsCurrentSolvingChainNegated())
            {
                if (currentChain.ActivatePlayer == 1 &&
                    currentChain.IsActivateCode(CardId.MaxxC))
                {
                    // Maxx "C" is only considered active after its chain link
                    // has actually resolved. This must not be set merely when
                    // the opponent activates the card, because the effect may
                    // still be negated later in the same chain.
                    _enemyMaxxCResolved = true;
                }
            }

            base.OnChainSolved(chainIndex);
        }

        public override void OnChainEnd()
        {
            if (_preserveCelticMysticDrawTriggerAtChainEnd)
                _preserveCelticMysticDrawTriggerAtChainEnd = false;
            else
                _celticMysticDrawTriggerPending = false;
            _selectingLightAndDarknessRitualMaterials = false;
            _pendingSpellShatteringSwordMonsterTarget = null;
            _pendingSpellShatteringSwordSpellDestroy = false;
            _pendingFydraulisSynchroToGraveId = 0;
            _pendingFydraulisDestructionTarget = null;
            _reservedOpponentTargets.Clear();
            _pendingDeckSearchIds.Clear();
            _pendingBlackChaosSupportSearch = false;
            _pendingLightAndDarknessReturnCards.Clear();
            base.OnChainEnd();
        }

        public override void OnMove(ClientCard card, int previousControler, int previousLocation,
            int currentControler, int currentLocation)
        {
            if (card != null && currentControler == 0 &&
                (currentLocation & (int)CardLocation.Grave) != 0)
            {
                if (_pendingReleaseCards.Contains(card))
                {
                    _ownMonsterReleasedToGraveThisTurn = true;
                    _pendingReleaseCards.Remove(card);
                }
            }

            if (card != null && card.IsCode(CardId.MindShuffle) &&
                currentControler == 0)
            {
                bool wasInSpellZone = (previousLocation & (int)CardLocation.SpellZone) != 0;
                bool isInSpellZone = (currentLocation & (int)CardLocation.SpellZone) != 0;
                if (!wasInSpellZone && isInSpellZone)
                    ++_mindShuffleFieldCount;
                else if (wasInSpellZone && !isInSpellZone &&
                    _mindShuffleFieldCount > 0)
                    --_mindShuffleFieldCount;
            }

            if (card != null && _pendingMindShuffleSummonId != 0 &&
                card.IsCode(_pendingMindShuffleSummonId) && currentControler == 0 &&
                (currentLocation & (int)CardLocation.MonsterZone) != 0)
            {
                MoveMindShuffleSummonToEnd(card.Id);
                _pendingMindShuffleSummonId = 0;
            }

            if (card != null && _pendingMindShuffleReturnCards.Contains(card) &&
                currentControler == 0 &&
                (previousLocation & (int)CardLocation.MonsterZone) != 0 &&
                (currentLocation & (int)CardLocation.Hand) != 0)
            {
                MoveMindShuffleReturnToEnd(card.Id);
                _pendingMindShuffleReturnCards.Remove(card);
            }

            if (card != null && card.IsCode(CardId.CelticMystic) &&
                (previousLocation & (int)CardLocation.MonsterZone) != 0 &&
                (currentLocation & (int)CardLocation.MonsterZone) == 0)
            {
                _celticMysticDrawTriggerPending = false;
                _preserveCelticMysticDrawTriggerAtChainEnd = false;
            }

            base.OnMove(card, previousControler, previousLocation, currentControler,
                currentLocation);
        }

        public override void OnSummoning()
        {
            ClientCard celticMystic = Duel.SummoningCards.FirstOrDefault(c => c != null &&
                c.Controller == 0 && c.IsCode(CardId.CelticMystic));
            _celticMysticDrawTriggerPending = celticMystic != null &&
                Bot.Hand.Any(c => c != null && c != celticMystic &&
                    IsRitualRelatedCard(c));
            _preserveCelticMysticDrawTriggerAtChainEnd = false;
            base.OnSummoning();
        }

        public override void OnNewPhase()
        {
            _celticMysticDrawTriggerPending = false;
            _preserveCelticMysticDrawTriggerAtChainEnd = false;
            _selectingBlackChaosSpecialSummonReturn = false;
            base.OnNewPhase();
        }

        public override void OnSpSummoned()
        {
            ChainInfo solvingChain = Duel.GetCurrentSolvingChainInfo();
            if (solvingChain != null &&
                solvingChain.IsActivateCode(CardId.LightAndDarknessRitual) &&
                Duel.LastSummonedCards.Any(c => c != null && c.Controller == 0 &&
                    IsRitualMonster(c)))
            {
                _performedRitualSummonThisTurn = true;
                ++_ritualSummonCountThisTurn;
            }

            _selectingLightAndDarknessRitualMaterials = false;

            if (Duel.LastSummonedCards.Any(c => c != null &&
                c.Controller == 0 && c.IsCode(CardId.CelticMystic)))
            {
                _celticMysticDrawTriggerPending =
                    Bot.Hand.Any(IsRitualRelatedCard);
                _preserveCelticMysticDrawTriggerAtChainEnd =
                    _celticMysticDrawTriggerPending &&
                    Duel.CurrentChain.Count > 0;
            }

            base.OnSpSummoned();
        }

        public override IList<ClientCard> OnSelectTribute(IList<ClientCard> cards,
            int min, int max, int hint, bool cancelable)
        {
            // Some server/script combinations expose Kuriboh Guardian through
            // MSG_SELECT_TRIBUTE without encoding its Level 8 replacement in
            // the operation parameter. The card is still a legal single-card
            // material for either Level 8 Ritual Monster, so accept the Grave
            // candidate directly before applying the generic sum selector.
            IList<ClientCard> griffohSelection =
                SelectGriffohAsFullRitualMaterial(cards, min, max);
            if (griffohSelection != null)
            {
                TrackReleaseSelection(griffohSelection);
                return griffohSelection;
            }

            // Some script/server versions expose Light and Darkness Ritual's
            // material selection through MSG_SELECT_TRIBUTE instead of
            // MSG_SELECT_SUM. In that route the generic selector was sorting
            // by attack and could ignore the intended Graveyard preference.
            // MSG_SELECT_TRIBUTE does not always preserve the solving-chain
            // snapshot. A Graveyard monster in this candidate list is the
            // reliable protocol signature for Light and Darkness Ritual's
            // special material route; normal tribute selection only offers
            // cards that can actually be tributed from the field/hand.
            if (cards.Any(IsGraveCard))
            {
                IList<ClientCard> ritualSelected = SelectGraveyardRitualTribute(
                    cards, min, max);
                if (ritualSelected == null)
                {
                    List<ClientCard> ordered = cards.Where(IsAllowedRitualMaterial)
                        .OrderBy(GetRitualTributePriority).ToList();
                    ritualSelected = AI.FindTributeSelection(ordered, min, max);
                }
                if (ritualSelected != null)
                {
                    TrackReleaseSelection(ritualSelected);
                    return ritualSelected;
                }
            }

            if (!_selectingGallantThiefTributes)
                return null;

            _selectingGallantThiefTributes = false;
            List<ClientCard> enemyTributes = cards.Where(c => c != null && c.Controller == 1)
                .OrderByDescending(GetGallantThiefTributePriority).ToList();
            List<ClientCard> ownTributes = cards.Where(c => c != null && c.Controller == 0)
                .OrderBy(GetGallantThiefTributePriority).ToList();

            // Prefer the opponent's monsters. Own monsters are only a fallback
            // when the server cannot provide enough opposing tributes.
            List<ClientCard> selected = new List<ClientCard>();
            selected.AddRange(enemyTributes.Take(max));
            if (selected.Count < min)
                selected.AddRange(ownTributes.Take(max - selected.Count));

            if (selected.Count < min)
            {
                if (cancelable)
                    return new List<ClientCard>();
                return null;
            }

            IList<ClientCard> result = selected.Take(Math.Min(max, selected.Count)).ToList();
            TrackReleaseSelection(result);
            return result;
        }

        public override IList<ClientCard> OnSelectRitualTribute(IList<ClientCard> cards,
            IList<ClientCard> mandatoryCards, int sum, int min, int max, bool exactEqual)
        {
            // Kuriboh Guardian is a special Level 1 exception: when the
            // server presents it as a legal Level 8 ritual material, it can
            // satisfy the ritual by itself. The local selector normally only
            // sees its printed Level 1 operation value, so normalize the
            // selected candidate to the requested ritual sum before GameAI
            // validates the response.
            IList<ClientCard> griffohSelection =
                SelectGriffohAsFullRitualMaterial(cards, mandatoryCards, sum, min, max);
            if (griffohSelection != null)
            {
                TrackReleaseSelection(griffohSelection);
                return griffohSelection;
            }

            IList<ClientCard> selected = SelectGraveyardRitualSumSelection(
                cards, mandatoryCards, sum, min, max, exactEqual);
            if (selected == null)
            {
                List<ClientCard> ordered = cards.Where(IsAllowedRitualMaterial)
                    .OrderBy(GetRitualMaterialPriority).ToList();
                selected = AI.FindSumSelection(ordered, mandatoryCards,
                    sum, min, max, exactEqual);
            }
            TrackReleaseSelection(selected);
            return selected;
        }

        public override IList<ClientCard> OnSelectSum(IList<ClientCard> cards,
            IList<ClientCard> mandatoryCards, int sum, int min, int max, int hint,
            bool exactEqual)
        {
            // Some protocol/script combinations reach OnSelectSum directly
            // without subsequently dispatching to OnSelectRitualTribute.
            // Handle the one-card Graveyard Kuriboh Guardian route here too,
            // so it cannot fall back to the generic level-1 calculation.
            // A few server versions do not preserve HINTMSG_RELEASE on this
            // packet. The presence of a Graveyard monster is the reliable
            // signature for this card's special ritual-material route.
            bool hasGraveMaterial = (cards != null && cards.Any(IsGraveCard)) ||
                (mandatoryCards != null && mandatoryCards.Any(IsGraveCard));
            if (hint == HintMsg.Release || hasGraveMaterial)
            {
                IList<ClientCard> griffohSelection =
                    SelectGriffohAsFullRitualMaterial(cards, mandatoryCards, sum, min, max);
                if (griffohSelection != null)
                {
                    TrackReleaseSelection(griffohSelection);
                    return griffohSelection;
                }

                IList<ClientCard> selected = SelectGraveyardRitualSumSelection(
                    cards, mandatoryCards, sum, min, max, exactEqual);
                if (selected == null)
                {
                    List<ClientCard> ordered = cards.Where(IsAllowedRitualMaterial)
                        .OrderBy(GetRitualMaterialPriority).ToList();
                    selected = AI.FindSumSelection(ordered, mandatoryCards,
                        sum, min, max, exactEqual);
                }
                TrackReleaseSelection(selected);
                return selected;
            }

            return null;
        }

        public override int OnSelectPlace(int cardId, int player, CardLocation location,
            int available)
        {
            if (player == 0 && location == CardLocation.MonsterZone &&
                IsExtraDeckMonster(cardId))
            {
                // Prefer an Extra Monster Zone for every Extra Deck monster;
                // use the first available one deterministically.
                if ((available & Zones.z5) != 0)
                    return Zones.z5;
                if ((available & Zones.z6) != 0)
                    return Zones.z6;
            }

            return base.OnSelectPlace(cardId, player, location, available);
        }

        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards,
            int min, int max, int hint, bool cancelable)
        {
            ChainInfo chain = Duel.GetCurrentSolvingChainInfo();
            if (chain == null)
                chain = GetCurrentActivationChainInfo();
            ClientCard solvingCard = Duel.GetCurrentSolvingChainCard();

            // Black Chaos's own Special Summon procedure returns one legal
            // Ritual Monster from the hand or Graveyard to the Deck. This
            // request is outside a normal chain, so prefer Graveyard cards
            // before hand cards here.
            if (chain == null && hint == HintMsg.ToDeck &&
                _selectingBlackChaosSpecialSummonReturn)
            {
                IList<ClientCard> blackChaosReturn =
                    SelectBlackChaosSpecialSummonReturn(cards, min, max);
                _selectingBlackChaosSpecialSummonReturn = false;
                if (blackChaosReturn != null)
                    return blackChaosReturn;
            }

            // Some server/core versions expose the resolving card but not a
            // ChainInfo snapshot while the effect is asking for its second
            // target. Keep Ecclesia's field-target selection card-specific in
            // that case; otherwise the generic selector may choose one of our
            // monsters when the opponent only has facedown back row cards.
            if (chain == null && solvingCard != null &&
                solvingCard.Controller == 0 &&
                solvingCard.IsCode(CardId.EcclesiaAndTheDarkDragon) &&
                (hint == HintMsg.Target || hint == HintMsg.ToDeck))
            {
                IList<ClientCard> ecclesiaTarget =
                    SelectEcclesiaAndTheDarkDragonTarget(cards, min, max);
                if (ecclesiaTarget != null)
                    return ecclesiaTarget;
            }

            bool selectingLightAndDarknessRitualMaterials = hint == HintMsg.Release &&
                (_selectingLightAndDarknessRitualMaterials ||
                 chain != null && chain.ActivatePlayer == 0 &&
                    chain.IsActivateCode(CardId.LightAndDarknessRitual) ||
                 cards.Any(IsGraveCard));
            if (selectingLightAndDarknessRitualMaterials)
            {
                _selectingLightAndDarknessRitualMaterials = true;

                // In MSG_SELECT_UNSELECT_CARD, min == 0 means the materials
                // already selected by earlier responses form a legal set.
                // Confirm that set immediately instead of adding another card.
                if (min == 0)
                    return new List<ClientCard>();

                // Some script/core combinations expose the ritual material
                // request through MSG_SELECT_CARD instead of MSG_SELECT_SUM
                // or MSG_SELECT_TRIBUTE. Keep the same Graveyard-first rule
                // on that route as well.
                IList<ClientCard> griffoh = SelectGriffohAsFullRitualMaterial(
                    cards, min, max);
                if (griffoh != null)
                {
                    TrackReleaseSelection(griffoh);
                    return griffoh;
                }

                List<ClientCard> graveMaterials = cards
                    .Where(IsPreferredGraveRitualMaterial)
                    .OrderBy(GetRitualTributePriority).ToList();
                if (graveMaterials.Count >= min)
                {
                    IList<ClientCard> selectedGrave = Util.CheckSelectCount(
                        graveMaterials, cards, min, max);
                    if (selectedGrave != null)
                    {
                        TrackReleaseSelection(selectedGrave);
                        return selectedGrave;
                    }
                }
            }

            IList<ClientCard> selected = null;
            if (chain != null && chain.ActivatePlayer == 0)
            {
                selected = SelectResolutionCard(chain, cards, min, max, hint);
            }

            // During chain construction the activation snapshot can be
            // unavailable on some server message sequences. These two cards
            // have an especially important Extra Deck cost, so identify the
            // live chain card directly as a safe second route rather than
            // allowing the generic selector to choose an unrelated card.
            if (selected == null && hint == HintMsg.ToGrave)
            {
                ClientCard current = Duel.GetCurrentChainCard();
                if (current != null && current.Controller == 0 &&
                    current.IsCode(CardId.FallenOfTheWhiteDragon,
                        CardId.TheFallenTheVirtuous))
                {
                    selected = SelectFallenExtraDeckCost(cards, min, max);
                }
            }

            if (selected == null)
                selected = base.OnSelectCard(cards, min, max, hint, cancelable);

            if (chain != null && chain.ActivatePlayer == 0 &&
                IsDeckSearchSelectionHint(hint))
                ReservePendingDeckSearch(selected);

            if (selected != null && chain != null &&
                chain.ActivatePlayer == 0 &&
                chain.IsActivateCode(CardId.LightAndDarknessRitual) &&
                chain.HasLocation(CardLocation.Grave) &&
                hint == HintMsg.AddToHand)
            {
                foreach (ClientCard card in selected)
                {
                    if (card != null && card.IsCode(CardId.SkullArchfiendOfChaos))
                        _pendingLightAndDarknessReturnCards.Add(card);
                }
            }

            if (IsOpponentTargetSelectionHint(hint) && IsValidTargetSelection(
                selected, cards, min, max))
                ReserveOpponentTargets(selected);

            if (hint == HintMsg.Release)
                TrackReleaseSelection(selected);
            return selected;
        }

        public override int OnSelectOption(IList<int> options)
        {
            int selected;

            ChainInfo solvingChain = Duel.GetCurrentSolvingChainInfo();
            if (solvingChain != null &&
                solvingChain.IsActivateCode(CardId.AlbionTheBrandedDragon))
            {
                // Albion's options are Set (1153) and Add to hand (1190).
                // Prefer Set whenever the server offers it, then fall back
                // to Add to hand if Set is unavailable.
                selected = options.IndexOf(1153);
                if (selected >= 0)
                    return selected;
                selected = options.IndexOf(1190);
                if (selected >= 0)
                    return selected;
            }

            // The destruction branch is the primary use of The Fallen in this
            // deck; the revival branch is deliberately not selected here.
            selected = GetOptionIndex(options, CardId.TheFallenTheVirtuous, 1);
            if (selected >= 0 && GetOptionIndex(options, CardId.TheFallenTheVirtuous, 2) >= 0)
                return selected;

            selected = GetOptionIndex(options, CardId.Griffoh, 2);
            if (selected >= 0 && CanSetGriffohSupportCard())
                return selected;
            selected = GetOptionIndex(options, CardId.Griffoh, 1);
            if (selected >= 0 && CanReceiveDamage())
                return selected;

            selected = GetOptionIndex(options, CardId.SpellShatteringSword, 1);
            if (selected >= 0 && CanUseSpellShatteringSwordSpellDestroy())
            {
                _pendingSpellShatteringSwordMonsterTarget = null;
                return selected;
            }
            selected = GetOptionIndex(options, CardId.SpellShatteringSword, 2);
            if (selected >= 0 && CanUseSpellShatteringSwordMonsterNegate())
                return selected;

            return base.OnSelectOption(options);
        }

        private bool GallantThiefSummon()
        {
            if (!CanSummonGallantThief(Card))
                return false;

            _selectingGallantThiefTributes = true;
            return true;
        }

        private bool CanSummonGallantThief(ClientCard gallantThief)
        {
            if (gallantThief == null ||
                !gallantThief.IsCode(CardId.TheWorldsGreatestGallantThief) ||
                !Bot.Hand.Contains(gallantThief) ||
                Bot.GetMonsterCount() != 0 || Enemy.GetMonsterCount() == 0)
                return false;

            bool hasOwnFallback = Bot.Hand.Any(c => c != null && c.IsMonster() &&
                c != gallantThief);
            return Enemy.GetMonsterCount() >= 2 || hasOwnFallback;
        }

        private bool ShouldWaitForGallantThief()
        {
            ClientCard gallantThief = Bot.Hand.FirstOrDefault(c => c != null &&
                c.IsCode(CardId.TheWorldsGreatestGallantThief));
            return Duel.Player == 0 &&
                (Duel.Phase == DuelPhase.Draw || Duel.Phase == DuelPhase.Standby) &&
                CanSummonGallantThief(gallantThief);
        }

        private bool MaxxCActivate()
        {
            return !ShouldWaitForGallantThief() && DefaultMaxxC();
        }

        private bool PuruliaActivate()
        {
            return !ShouldWaitForGallantThief() && DefaultMaxxC();
        }

        private bool AshBlossomActivate()
        {
            return !ShouldWaitForGallantThief() && DefaultAshBlossomAndJoyousSpring();
        }

        private bool GallantThiefActivate()
        {
            if (Card.Location != CardLocation.MonsterZone)
                return false;

            ClientCard chainCard = GetCurrentOpponentChainCard();
            bool canNegateRemoteEffect = chainCard != null &&
                (chainCard.Location == CardLocation.Hand ||
                 chainCard.Location == CardLocation.Grave ||
                 chainCard.Location == CardLocation.Removed);
            bool canChangeBattlePositions = Duel.Phase >= DuelPhase.BattleStart &&
                Duel.Phase < DuelPhase.Main2 &&
                Enemy.GetMonsters().Any(c => c.IsAttack());

            if (IsDescription(CardId.TheWorldsGreatestGallantThief, 1))
                return canNegateRemoteEffect;

            if (IsDescription(CardId.TheWorldsGreatestGallantThief, 2))
                return canChangeBattlePositions;

            // A generic optional-effect description is not assigned to either
            // branch blindly. Accept it only when the live chain or battle
            // context independently proves that at least one effect is useful.
            if (ActivateDescription == -1)
                return canNegateRemoteEffect || canChangeBattlePositions;

            return false;
        }

        private bool FydraulisHarmoniaActivate()
        {
            if (ShouldWaitForGallantThief() || Card.Location != CardLocation.Hand ||
                DefaultCheckWhetherCardIsNegated(Card))
                return false;

            // Use the activation snapshot instead of the mutable live card.
            // The opponent's monster may already have left the field as cost,
            // while its effect was still legally activated from the field.
            ChainInfo latest = GetLatestChainInfo();
            if (!IsOpponentMonsterEffectChain(latest))
                return false;

            ClientCard source = GetChainSourceCard(latest);
            if (source != null && source.IsDisabled())
                return false;

            List<ClientCard> synchros = Bot.ExtraDeck.Where(c => c != null &&
                c.HasType(CardType.Synchro)).ToList();
            if (synchros.Count >= 5)
            {
                // Executor conditions may be queried more than once for the
                // same response. Reuse the first accepted reservation instead
                // of moving it to the next monster on every query.
                if (_pendingFydraulisDestructionTarget != null &&
                    Enemy.GetMonsters().Contains(_pendingFydraulisDestructionTarget))
                    return true;

                ClientCard target = GetOrderedFydraulisTargets(Enemy.GetMonsters())
                    .FirstOrDefault(c => !IsOpponentTargetReserved(c));
                if (target == null)
                    return false;

                _pendingFydraulisDestructionTarget = target;
                _reservedOpponentTargets.Add(target);
                return true;
            }

            // Three revealed Synchros still unlock the send-to-Graveyard
            // payload. Spend the card only when that payload has an immediate
            // strategic purpose; with one or two Synchros, reserve it for an
            // emergency body on an otherwise empty field.
            if (synchros.Count >= 3)
                return GetPreferredFydraulisSynchroToGraveId(synchros) != 0;
            return synchros.Count > 0 && Bot.GetMonsterCount() == 0 &&
                Enemy.GetMonsterCount() > 0;
        }

        private bool GriffohActivate()
        {
            if (ShouldWaitForGallantThief() || Card.Location != CardLocation.Hand ||
                DefaultCheckWhetherCardIsNegated(Card))
                return false;

            return !ShouldDelayForHigherPriorityHandStarter(CardId.Griffoh) &&
                (CanSetGriffohSupportCard() || CanReceiveDamage());
        }

        private bool BlackChaosHandActivate()
        {
            if (ShouldWaitForGallantThief() || Card.Location != CardLocation.Hand ||
                DefaultCheckWhetherCardIsNegated(Card))
                return false;

            // This hand effect can only place the continuous Trap Mind
            // Shuffle. A copy already on our field means the support is
            // established, but a copy in our hand does not: it cannot be
            // activated immediately this turn and must not block this effect.
            if (HasMindShuffleOnField())
                return false;

            bool accept = !ShouldSummonCelticMysticFirst() &&
                !ShouldDelayForHigherPriorityHandStarter(CardId.BlackChaos) &&
                CanSearchBlackChaosSupportCard();
            if (accept)
                _pendingBlackChaosSupportSearch = true;
            return accept;
        }

        private bool BlackSkullDragonFieldActivate()
        {
            return !ShouldWaitForGallantThief() &&
                Card.Location == CardLocation.MonsterZone &&
                !DefaultCheckWhetherCardIsNegated(Card);
        }

        private bool BlackSkullDragonHandActivate()
        {
            if (ShouldWaitForGallantThief() || Card.Location != CardLocation.Hand ||
                DefaultCheckWhetherCardIsNegated(Card))
                return false;

            // Do not discard Light and Darkness Ritual for this summon while
            // the same Ritual Spell is already a legal immediate action.
            return !HasHigherPriorityLightAndDarknessRitualCandidate() &&
                CanUseBlackSkullDragonHandSpecialSummon();
        }

        private bool BlackChaosFieldActivate()
        {
            if (ShouldWaitForGallantThief() || Card.Location != CardLocation.MonsterZone ||
                DefaultCheckWhetherCardIsNegated(Card))
                return false;

            return Duel.Phase < DuelPhase.End && Enemy.GetSpellCount() +
                Enemy.GetMonsterCount() >= 2;
        }

        private bool BlackChaosSpecialSummon()
        {
            // The server already supplies only legal candidates for this
            // special-summon procedure. This callback only delays it while the
            // accepted Gallant Thief normal summon is still pending, or while
            // Celtic Mystic is a legal normal-summon candidate. GameAI checks
            // SpecialSummon before Summon, so this local guard is required to
            // make Celtic Mystic's requested normal-summon priority real.
            if (HasHigherPriorityLightAndDarknessRitualCandidate())
                return false;

            bool accept = !ShouldWaitForGallantThief() &&
                !ShouldSummonCelticMysticFirst() &&
                (Card.Location != CardLocation.Hand ||
                    !ShouldDelayForHigherPriorityHandStarter(CardId.BlackChaos));
            if (accept)
                _selectingBlackChaosSpecialSummonReturn = true;
            return accept;
        }

        private bool CelticMysticSummon()
        {
            return !ShouldWaitForGallantThief() &&
                !DefaultCheckWhetherCardIsNegated(Card) &&
                (Bot.Hand.Any(c => c != null && c != Card &&
                     IsRitualRelatedCard(c)) ||
                 Bot.Hand.Any(IsRitualMonster));
        }

        private bool BlackLusterSoldierActivate()
        {
            if (ShouldWaitForGallantThief() || Card.Location != CardLocation.MonsterZone ||
                DefaultCheckWhetherCardIsNegated(Card))
                return false;

            // Offset 0 is the summon-success facedown banish and needs an
            // opponent field card. Offset 1 is the post-battle ATK/extra-attack
            // effect and must not be rejected merely because no field target
            // remains after the destroyed monster has left the field.
            if (IsDescription(CardId.BlackLusterSoldierSoldierOfLightAndDarkness, 0))
            {
                return HasFreshOpponentTarget(
                    Enemy.GetMonsters().Concat(Enemy.GetSpells()));
            }

            if (IsDescription(CardId.BlackLusterSoldierSoldierOfLightAndDarkness, 1))
                return true;

            if (ActivateDescription == -1)
            {
                if (Duel.Phase >= DuelPhase.BattleStart && Duel.Phase < DuelPhase.Main2)
                    return true;
                return HasFreshOpponentTarget(
                    Enemy.GetMonsters().Concat(Enemy.GetSpells()));
            }

            return false;
        }

        private bool CanActivateCelticMystic()
        {
            if (ShouldWaitForGallantThief() || Card.Location != CardLocation.MonsterZone ||
                DefaultCheckWhetherCardIsNegated(Card))
                return false;

            return true;
        }

        private bool CelticMysticSearchActivate()
        {
            if (!CanActivateCelticMystic())
                return false;

            // aux.Stringid uses zero-based offsets: the draw-three effect is
            // offset 0 and the Ritual Monster effect is offset 1.
            if (IsDescription(CardId.CelticMystic, 0))
                return Bot.Hand.Any(IsRitualRelatedCard);

            if (IsDescription(CardId.CelticMystic, 1))
                return false;

            // A generic description is accepted here only while the successful
            // normal-summon trigger is still pending. It must not make the
            // later ignition effect inherit the draw-three condition.
            return ActivateDescription == -1 &&
                _celticMysticDrawTriggerPending &&
                Bot.Hand.Any(IsRitualRelatedCard);
        }

        private bool CelticMysticRitualSpecialSummonActivate()
        {
            if (!CanActivateCelticMystic())
                return false;

            // The draw-three effect above is a summon-success trigger and must
            // not be delayed by a stale SpecialSummonableCards snapshot. Only
            // the separate Ritual special-summon effect keeps this strategic
            // priority behind an already legal Black Chaos summon.
            if (Duel.CurrentChain.Count == 0 &&
                Duel.MainPhase.SpecialSummonableCards.Any(c => c != null &&
                    c.IsCode(CardId.BlackChaos)))
                return false;

            // Keep Celtic Mystic's Ritual special summon behind a legal
            // hand/field Light and Darkness Ritual activation as well. The
            // same priority is used by Black Chaos; otherwise Celtic Mystic
            // could release itself immediately after the Ritual was placed.
            if (HasHigherPriorityLightAndDarknessRitualCandidate())
                return false;

            // If the server has already offered the Graveyard recovery effect
            // of Light and Darkness Ritual, let that effect resolve before
            // releasing Celtic Mystic to Ritual Summon. The explicit guard
            // keeps this priority correct even if the server's candidate
            // enumeration order changes.
            if (Duel.CurrentChain.Count == 0 &&
                Duel.MainPhase.ActivableCards.Any(c => c != null &&
                    c.IsCode(CardId.LightAndDarknessRitual) &&
                    c.Location == CardLocation.Grave))
                return false;

            if (IsDescription(CardId.CelticMystic, 0))
                return false;

            if (IsDescription(CardId.CelticMystic, 1))
            {
                return Bot.Hand.Any(IsRitualMonster);
            }

            // A generic description can reach this executor only after the
            // summon-success trigger context has ended. This prevents the two
            // effects from accepting the same unknown-description candidate.
            return ActivateDescription == -1 &&
                !_celticMysticDrawTriggerPending &&
                Bot.Hand.Any(IsRitualMonster);
        }

        private bool RaggedRecordsOfRitesActivate()
        {
            if (ShouldWaitForGallantThief() || DefaultCheckWhetherCardIsNegated(Card))
                return false;

            if (Card.Location == CardLocation.Grave)
            {
                // Effect 2 only returns this card to the hand. It does not
                // search the Deck, so the availability of a monster for effect
                // 1 must not suppress this delayed Graveyard trigger.
                // Facedown monsters do not expose a non-Ritual type and must
                // not suppress this Graveyard recovery trigger.
                return !Bot.GetMonsters().Any(c => c != null && c.IsFaceup() &&
                    !c.HasType(CardType.Ritual));
            }

            return HasRitualMonsterSearchTarget() &&
                (Card.Location == CardLocation.Hand ||
                 Card.Location == CardLocation.SpellZone);
        }

        private bool MindShuffleActivate()
        {
            if (ShouldWaitForGallantThief() || DefaultCheckWhetherCardIsNegated(Card) ||
                Card.Location != CardLocation.SpellZone)
                return false;

            if (IsDescription(CardId.MindShuffle, 0))
                return HasRitualMonsterSearchTarget();

            if (IsDescription(CardId.MindShuffle, 1))
                return HasLatestOpponentChain() && CanUseMindShuffleSummon() &&
                    HasMindShuffleBreakthroughReason();

            if (HasLatestOpponentChain() &&
                CanUseMindShuffleSummon() && HasMindShuffleBreakthroughReason())
                return true;
            return HasRitualMonsterSearchTarget();
        }

        private bool CrimsonCallActivate()
        {
            if (ShouldWaitForGallantThief() || DefaultCheckWhetherCardIsNegated(Card))
                return false;

            if (Card.Location == CardLocation.Grave)
            {
                return Duel.Phase >= DuelPhase.BattleStart &&
                    Bot.HasInMonstersZone(CardId.RedDragonArchfiend);
            }

            // The hand and Spell/Trap Zone effects do not need an additional
            // local target/availability check. The server has already supplied
            // the legal activation candidate.
            return Card.Location == CardLocation.Hand ||
                Card.Location == CardLocation.SpellZone;
        }

        private bool LightAndDarknessRitualGraveActivate()
        {
            return Card != null && Card.Location == CardLocation.Grave &&
                LightAndDarknessRitualActivate();
        }

        private bool LightAndDarknessRitualActivate()
        {
            if (ShouldWaitForGallantThief())
                return false;

            if (Card.Location == CardLocation.Grave)
            {
                // The server only includes this card when the Graveyard
                // recovery effect has a legal resolution. Do not rebuild its
                // return-card filter from the partial client-side Graveyard
                // state: that used to reject a legal recovery effect and let
                // Celtic Mystic's Ritual special-summon effect win first.
                return true;
            }

            if (DefaultCheckWhetherCardIsNegated(Card))
                return false;

            if (Card.Location != CardLocation.Hand && Card.Location != CardLocation.SpellZone)
                return false;

            // On the first turn, perform at most one actual Ritual Summon.
            // The restriction is deliberately applied only to the Ritual
            // Summon route; the Graveyard recovery effect above remains
            // available because it does not perform a Ritual Summon.
            if (Duel.Turn == 1 && _ritualSummonCountThisTurn >= 1)
                return false;

            // The server has already supplied this card as a legal activation
            // candidate, including the Ritual Monster and material checks.
            // Do not re-simulate those checks from the partial client state:
            // Graveyard substitution effects (especially Griffoh and the
            // one-card Kuriboh route) can make the local material estimate
            // disagree with the server and incorrectly let Celtic Mystic win.
            return true;
        }

        private bool FallenOfTheWhiteDragonActivate()
        {
            if (ShouldWaitForGallantThief() ||
                ShouldSuppressWhiteDragonRouteAfterMaxxC())
                return false;

            if (Card.Location == CardLocation.Hand)
            {
                return !DefaultCheckWhetherCardIsNegated(Card) &&
                    !ShouldDelayForHigherPriorityHandStarter(CardId.FallenOfTheWhiteDragon) &&
                    Bot.HasInExtra(new int[] { CardId.AlbionTheBrandedDragon,
                        CardId.EcclesiaAndTheDarkDragon,
                        CardId.TitanikladTheAshDragon, CardId.AlbaLenatusTheAbyssDragon });
            }

            if (Card.Location == CardLocation.MonsterZone)
                return !DefaultCheckWhetherCardIsNegated(Card);

            return false;
        }

        private bool EcclesiaSpecialSummon()
        {
            return !ShouldWaitForGallantThief() && Card.Location == CardLocation.Hand &&
                !ShouldSuppressWhiteDragonRouteAfterMaxxC() &&
                Enemy.GetMonsterCount() > Bot.GetMonsterCount() &&
                Duel.Phase < DuelPhase.End &&
                !DefaultCheckWhetherCardIsNegated(Card) &&
                !ShouldDelayForHigherPriorityHandStarter(
                    CardId.IncredibleEcclesiaTheVirtuous);
        }

        private bool AlbaLenatusSpecialSummon()
        {
            if (ShouldWaitForGallantThief())
                return false;

            ClientCard whiteDragon = Bot.GetMonsters().FirstOrDefault(c =>
                c != null && c.IsCode(CardId.FallenOfTheWhiteDragon));
            if (whiteDragon == null)
                return false;

            List<ClientCard> enemyDragons = Enemy.GetMonsters().Where(c =>
                c != null && c.IsFaceup() && c.HasRace(CardRace.Dragon)).ToList();
            if (enemyDragons.Count == 0)
                return false;

            List<ClientCard> materials = new List<ClientCard> { whiteDragon };
            materials.AddRange(enemyDragons);
            AI.SelectMaterials(materials);
            return true;
        }

        private bool PriorityExtraDeckSpecialSummon()
        {
            return !ShouldWaitForGallantThief();
        }

        private bool StardustDragonSpecialSummon()
        {
            return !ShouldWaitForGallantThief() &&
                (HasChaosRitualStarterInHand() ||
                 HasMindShuffleOnField() ||
                 !Bot.HasInDeck(CardId.CrimsonCall));
        }

        private bool StardustDragonActivate()
        {
            return !ShouldWaitForGallantThief() &&
                !DefaultCheckWhetherCardIsNegated(Card);
        }

        private bool TitanikladActivate()
        {
            // The third effect is an End Phase search or Special Summon after
            // this card was sent to the Graveyard. This deck deliberately uses
            // it only to recover White Dragon.
            return !ShouldWaitForGallantThief() &&
                Card.Location == CardLocation.Grave &&
                Duel.Phase == DuelPhase.End &&
                !DefaultCheckWhetherCardIsNegated(Card) &&
                Bot.HasInDeck(CardId.FallenOfTheWhiteDragon);
        }

        private bool AlbionActivate()
        {
            // Albion is not summoned by this deck. Only its End Phase recovery
            // effect from the Graveyard is used.
            return !ShouldWaitForGallantThief() &&
                Card.Location == CardLocation.Grave && Duel.Phase == DuelPhase.End &&
                !DefaultCheckWhetherCardIsNegated(Card);
        }

        private bool RedNovaDragonActivate()
        {
            // The on-summon effect recovers a card and grants this monster
            // additional ATK. Its destruction immunity is continuous.
            return !ShouldWaitForGallantThief() &&
                Card.Location == CardLocation.MonsterZone &&
                !DefaultCheckWhetherCardIsNegated(Card);
        }

        private bool PsychicEndPunisherActivate()
        {
            if (ShouldWaitForGallantThief() ||
                Card.Location != CardLocation.MonsterZone ||
                DefaultCheckWhetherCardIsNegated(Card))
                return false;

            // The deck's policy is to use every legal effect immediately;
            // do not add a separate LP threshold here.
            return true;
        }

        private bool PsychicEndPunisherSpecialSummon()
        {
            return !ShouldWaitForGallantThief();
        }

        private bool ChaosAngelActivate()
        {
            return !ShouldWaitForGallantThief() &&
                Card.Location == CardLocation.MonsterZone &&
                !DefaultCheckWhetherCardIsNegated(Card) &&
                HasFreshOpponentTarget(Enemy.GetMonsters().Concat(Enemy.GetSpells()));
        }

        private bool CrimsonKingActivate()
        {
            // The search effect and the reactive replacement effect are both
            // accepted whenever the server reports them as legal.
            return !ShouldWaitForGallantThief() &&
                Card.Location == CardLocation.MonsterZone &&
                !DefaultCheckWhetherCardIsNegated(Card);
        }

        private bool EnigmasterPackbitActivate()
        {
            // This deck only uses the Graveyard trigger. The Continuous Trap
            // Special Summon effect is deliberately left unused.
            return !ShouldWaitForGallantThief() &&
                Card.Location == CardLocation.Grave &&
                !DefaultCheckWhetherCardIsNegated(Card) &&
                HasFreshOpponentTarget(Enemy.GetMonsters().Where(c =>
                    c != null && c.IsFaceup()));
        }

        private bool EcclesiaAndTheDarkDragonActivate()
        {
            // This deck does not summon Dark Ecclesia from the field. Only its
            // Graveyard recycling effect is accepted.
            return !ShouldWaitForGallantThief() &&
                Card.Location == CardLocation.Grave &&
                !DefaultCheckWhetherCardIsNegated(Card) &&
                HasFreshOpponentTarget(Enemy.GetMonsters().Concat(Enemy.GetSpells()));
        }

        private bool WindPegasusIgnisterActivate()
        {
            if (ShouldWaitForGallantThief() || DefaultCheckWhetherCardIsNegated(Card))
                return false;

            // This card is not expected to be summoned in this deck; use only
            // its Graveyard reactive effect.
            return Card.Location == CardLocation.Grave &&
                HasFreshOpponentTarget(Enemy.GetMonsters().Concat(Enemy.GetSpells()));
        }

        private bool GoldenCloudBeastMalongActivate()
        {
            // This card is not expected to be summoned in this deck; use only
            // the Graveyard effect that returns an opposing face-up card.
            return !ShouldWaitForGallantThief() &&
                Card.Location == CardLocation.Grave &&
                !DefaultCheckWhetherCardIsNegated(Card) &&
                HasFreshOpponentTarget(Enemy.GetMonsters().Concat(Enemy.GetSpells()));
        }

        private bool HeraldOfTheArcLightActivate()
        {
            // This card is not expected to be summoned in this deck; use only
            // its Graveyard Ritual search.
            return !ShouldWaitForGallantThief() &&
                Card.Location == CardLocation.Grave &&
                !DefaultCheckWhetherCardIsNegated(Card);
        }

        private bool CrimsonKingSpecialSummon()
        {
            // Stardust Dragon is the preferred 8-Synchro route when Mind
            // Shuffle is already established, or when Crimson Call is no
            // longer available for Crimson King's follow-up search. Do not
            // accept Crimson King in those cases, even if the server also
            // reports its summon as legal.
            return !ShouldWaitForGallantThief() &&
                !HasMindShuffleOnField() &&
                Bot.HasInDeck(CardId.CrimsonCall) &&
                !HasChaosRitualStarterInHand();
        }

        private bool HasChaosRitualStarterInHand()
        {
            return GetHandStarterOrder().Any(IsHandStarterAvailable);
        }

        private IList<int> GetHandStarterOrder()
        {
            return new List<int>
            {
                CardId.RaggedRecordsOfRites,
                CardId.IncredibleEcclesiaTheVirtuous,
                CardId.FallenOfTheWhiteDragon,
                CardId.BlackChaos,
                CardId.Griffoh
            };
        }

        private bool ShouldDelayForHigherPriorityHandStarter(int cardId)
        {
            // The order is intended for selecting the next main-phase action.
            // An opponent chain must not suppress a valid hand response.
            if (Duel.CurrentChain.Count != 0)
                return false;

            IList<int> order = GetHandStarterOrder();
            int currentIndex = order.IndexOf(cardId);
            if (currentIndex < 0)
                return false;

            for (int i = 0; i < currentIndex; ++i)
            {
                if (IsHandStarterAvailable(order[i]))
                    return true;
            }
            return false;
        }

        private bool ShouldSuppressWhiteDragonRouteAfterMaxxC()
        {
            if (Duel.Player != 0 || !_enemyMaxxCResolved)
                return false;

            // Once the opponent's Maxx "C" has resolved on our turn, keep
            // White Dragon and Ecclesia in reserve when another recognized
            // starter is already in hand. This is intentionally based on the
            // cards being present in hand; the server still decides whether a
            // specific effect is legally activatable.
            return Bot.Hand.Any(c => c != null && c.IsCode(
                CardId.BlackChaos,
                CardId.RaggedRecordsOfRites,
                CardId.Griffoh,
                CardId.CelticMystic));
        }

        private bool ShouldSummonCelticMysticFirst()
        {
            return Duel.CurrentChain.Count == 0 &&
                Duel.MainPhase.SummonableCards.Any(c => c != null &&
                    c.IsCode(CardId.CelticMystic) &&
                    !DefaultCheckWhetherCardIsNegated(c) &&
                    (Bot.Hand.Any(handCard => handCard != null && handCard != c &&
                         IsRitualRelatedCard(handCard)) ||
                     Bot.Hand.Any(IsRitualMonster)));
        }

        private bool IsHandStarterAvailable(int cardId)
        {
            ClientCard card = Bot.Hand.FirstOrDefault(c => c != null && c.IsCode(cardId));
            if (card == null || DefaultCheckWhetherCardIsNegated(card))
                return false;

            if (cardId == CardId.RaggedRecordsOfRites)
                return HasRitualMonsterSearchTarget();

            if (cardId == CardId.IncredibleEcclesiaTheVirtuous)
            {
                return Enemy.GetMonsterCount() > Bot.GetMonsterCount() &&
                    Duel.Phase < DuelPhase.End;
            }

            if (cardId == CardId.FallenOfTheWhiteDragon)
            {
                return HasFallenExtraDeckCost();
            }

            if (cardId == CardId.Griffoh)
                return CanSetGriffohSupportCard() || CanReceiveDamage();

            if (cardId == CardId.BlackChaos)
                return CanSearchBlackChaosSupportCard();

            return false;
        }

        private bool EcclesiaActivate()
        {
            if (ShouldWaitForGallantThief() || DefaultCheckWhetherCardIsNegated(Card))
                return false;

            if (Card.Location == CardLocation.MonsterZone)
            {
                if (ShouldSuppressWhiteDragonRouteAfterMaxxC())
                    return false;

                // The server already supplies the legal hand/deck targets for
                // White Dragon's Fallen. The strategic restriction is only
                // that no copy is currently on our field.
                return !Bot.HasInMonstersZone(CardId.FallenOfTheWhiteDragon);
            }

            if (Card.Location == CardLocation.Grave)
            {
                // The script records whether a Fusion Monster was sent to our
                // Graveyard this turn. That monster does not need to remain
                // there at the End Phase; the server candidate already proves
                // that the event condition was met.
                return Duel.Phase == DuelPhase.End;
            }

            return false;
        }

        private bool SkullArchfiendOfChaosActivate()
        {
            if (ShouldWaitForGallantThief() || DefaultCheckWhetherCardIsNegated(Card))
                return false;

            // The deck deliberately does not use the hand version of the
            // recycle/special-summon effect. Only accept that route from the
            // Graveyard, and only when at least four post-filter candidates
            // remain. The Graveyard search effect remains a separate legal
            // route when its ritual-spell condition is met.
            if (Card.Location == CardLocation.Hand)
                return false;

            if (Card.Location == CardLocation.Grave)
            {
                // Effect 2 is the separate Ritual Monster search after this
                // card is sent to the Graveyard. The server already offers
                // this activation only when its send/search requirements are
                // legal, so accept it without applying effect 1's timing
                // restriction. Trigger prompts with description 0/221 are
                // normalized to -1 by GameBehavior; for this card that
                // generic trigger prompt is the Graveyard search effect.
                if (IsDescription(CardId.SkullArchfiendOfChaos, 1) ||
                    ActivateDescription == -1)
                    return true;

                // Do not treat an unexpected description as effect 1. Its
                // three-card subgroup selection has strict script constraints
                // and must only be entered for the explicit offset 0 prompt.
                if (!IsDescription(CardId.SkullArchfiendOfChaos, 0))
                    return false;

                // Effect 1 is the explicitly described return-and-special-
                // summon effect. Its timing and inability to chain are already
                // enforced by the server's legal candidate list. Keep only the
                // local guard that prevents it from returning to the field
                // before Light and Darkness Ritual's return has resolved.
                if (IsSkullArchfiendReturnPending(Card))
                    return false;

                List<ClientCard> recycleCandidates = GetRitualRecycleCandidates();
                return recycleCandidates.Count >= 4 &&
                    recycleCandidates.Any(IsRitualRelatedCard);
            }

            return false;
        }

        private bool MagicianOfDarkChaosActivate()
        {
            if (ShouldWaitForGallantThief() || Card.Location != CardLocation.MonsterZone ||
                DefaultCheckWhetherCardIsNegated(Card))
                return false;

            // The facedown-banish effect is independent of the Graveyard
            // recovery effect. Its server-provided description is offset 1.
            if (IsDescription(CardId.MagicianOfDarkChaosBlackChaos, 1))
            {
                return HasFreshOpponentTarget(
                    Enemy.GetMonsters().Concat(Enemy.GetSpells()));
            }

            // The summon-success recovery effect is offset 0 and may also be
            // exposed as a generic optional trigger. Any other explicit
            // description must not inherit its Graveyard-spell condition.
            if (IsDescription(CardId.MagicianOfDarkChaosBlackChaos, 0) ||
                ActivateDescription == -1)
                return Bot.Graveyard.Any(c => c.IsSpell());

            return false;
        }

        private bool TheFallenTheVirtuousActivate()
        {
            if (ShouldWaitForGallantThief() || DefaultCheckWhetherCardIsNegated(Card) ||
                !HasFallenExtraDeckCost())
                return false;

            return HasFreshOpponentTarget(GetOrderedFallenTargets(
                Enemy.GetMonsters().Concat(Enemy.GetSpells())));
        }

        private bool SpellShatteringSwordActivate()
        {
            if (ShouldWaitForGallantThief() || DefaultCheckWhetherCardIsNegated(Card))
                return false;

            // This is a delayed trigger from the Graveyard. The activating
            // chain has already ended by the time the server asks for it, so
            // it must not be rejected by the current-chain guard used by the
            // hand/field effects.
            if (Card.Location == CardLocation.Grave ||
                Card.Location == CardLocation.Removed)
                return IsDescription(CardId.SpellShatteringSword, 3) ||
                    ActivateDescription == -1;

            if (Duel.CurrentChain.Count == 0)
                return false;

            ChainInfo latest = GetLatestChainInfo();
            if (latest == null || latest.ActivatePlayer != 1)
                return false;

            // Both branches are evaluated only against the immediately
            // preceding opponent link. Older opponent links in the same chain
            // are not valid reasons to activate this card now.
            _pendingSpellShatteringSwordSpellDestroy =
                CanUseSpellShatteringSwordSpellDestroy(latest);
            _pendingSpellShatteringSwordMonsterTarget =
                GetSpellShatteringSwordMonsterTarget(latest);
            bool canClearSpells = _pendingSpellShatteringSwordSpellDestroy;
            bool canDisableMonster =
                _pendingSpellShatteringSwordMonsterTarget != null;
            return canClearSpells || canDisableMonster;
        }

        private bool CanUseSpellShatteringSwordSpellDestroy()
        {
            if (_pendingSpellShatteringSwordSpellDestroy)
                return true;

            return CanUseSpellShatteringSwordSpellDestroy(GetLatestChainInfo());
        }

        private bool CanUseSpellShatteringSwordSpellDestroy(ChainInfo chain)
        {
            if (chain == null || chain.ActivatePlayer != 1 ||
                !chain.HasLocation(CardLocation.SpellZone))
                return false;

            bool isSpell = (chain.ActivateType & (int)CardType.Spell) != 0;
            bool isContinuousOrField = (chain.ActivateType &
                (int)(CardType.Continuous | CardType.Field)) != 0;
            return isSpell && isContinuousOrField && HasFreshOpponentTarget(
                Enemy.GetSpells());
        }

        private bool CanUseSpellShatteringSwordMonsterNegate()
        {
            if (_pendingSpellShatteringSwordMonsterTarget != null &&
                _pendingSpellShatteringSwordMonsterTarget.Controller == 1 &&
                _pendingSpellShatteringSwordMonsterTarget.IsOnField() &&
                _pendingSpellShatteringSwordMonsterTarget.IsFaceup())
                return true;

            _pendingSpellShatteringSwordMonsterTarget =
                GetSpellShatteringSwordMonsterTarget(GetLatestChainInfo());
            return _pendingSpellShatteringSwordMonsterTarget != null;
        }

        private ClientCard GetSpellShatteringSwordMonsterTarget(ChainInfo chain)
        {
            if (!Bot.Hand.Concat(Bot.Graveyard).Any(c => c.IsCode(
                CardId.LightAndDarknessRitual)))
                return null;

            ClientCard source = GetChainSourceCard(chain);
            if (source == null || source.IsDisabled() ||
                !HasFreshOpponentTarget(new ClientCard[] { source }))
                return null;
            return source;
        }

        private bool SpatialTrunadeActivate()
        {
            if (ShouldWaitForGallantThief() || DefaultCheckWhetherCardIsNegated(Card))
                return false;

            return CanUseSpatialTrunadeOnEnemyField();
        }

        private bool DrollLockBirdActivate()
        {
            if (ShouldWaitForGallantThief() || DefaultCheckWhetherCardIsNegated(Card) ||
                Duel.Player != 1 || Duel.LastChainPlayer != 1)
                return false;

            return true;
        }

        private bool ChaosSpellSet()
        {
            if (!(Card.IsTrap() || Card.HasType(CardType.QuickPlay) ||
                DefaultSpellMustSetFirst()))
                return false;

            int reservedZones = 1;
            bool albionMaySetFallen = Bot.HasInGraveyard(
                CardId.AlbionTheBrandedDragon) &&
                Bot.HasInDeck(CardId.TheFallenTheVirtuous);
            bool handEngineMaySetSupport =
                Bot.HasInHand(CardId.Griffoh) && CanSetGriffohSupportCard() ||
                Bot.HasInHand(CardId.BlackChaos) &&
                    CanSearchBlackChaosSupportCard();
            if (albionMaySetFallen || handEngineMaySetSupport)
                reservedZones = 2;

            return Bot.GetSpellCountWithoutField() < 5 - reservedZones;
        }

        private IList<ClientCard> SelectResolutionCard(ChainInfo chain,
            IList<ClientCard> cards, int min, int max, int hint)
        {
            if (chain.IsActivateCode(CardId.RaggedRecordsOfRites))
            {
                if (hint == HintMsg.Confirm)
                    return SelectPreferredIds(cards, min, max, CardId.LightAndDarknessRitual);
                if (hint == HintMsg.AddToHand)
                {
                    IList<ClientCard> selected = SelectRaggedRecordsSearchCard(
                        cards, min, max);
                    if (selected != null)
                    {
                        // The first effect has now reached its search
                        // selection. Register only effect 1; the Graveyard
                        // recovery effect remains independently usable this
                        // turn because the card has two separate once-per-
                        // turn effects.
                        _activatedFirstEffectCardIdsThisTurn.Add(
                            CardId.RaggedRecordsOfRites);
                    }
                    return selected;
                }
            }

            if (chain.IsActivateCode(CardId.CelticMystic) && hint == HintMsg.Discard)
                return SelectCelticMysticDiscardCards(cards, min, max);

            if (chain.IsActivateCode(CardId.MindShuffle))
            {
                if (hint == HintMsg.AddToHand)
                    return SelectMindShuffleSearchCard(cards, min, max);
                if (hint == HintMsg.SpSummon)
                    return SelectMindShuffleSummonMonster(cards, min, max);
                if (hint == HintMsg.Discard)
                    return SelectMindShuffleDiscardCards(cards, min, max);
                if (hint == HintMsg.ReturnToHand)
                {
                    List<ClientCard> returnCandidates = cards
                        .Where(IsMindShuffleReturnCandidate).ToList();
                    bool magicianTargeted = returnCandidates.Any(c =>
                        c.IsCode(CardId.MagicianOfDarkChaosBlackChaos) &&
                        IsOpponentTargetedByCurrentChain(c));
                    if (IsMindShuffleProtectionThreat() && !magicianTargeted)
                    {
                        returnCandidates = returnCandidates.Where(c =>
                            !c.IsCode(CardId.MagicianOfDarkChaosBlackChaos)).ToList();
                    }
                    returnCandidates = returnCandidates
                        .OrderByDescending(IsOpponentTargetedByCurrentChain)
                        .ThenByDescending(WasCardActivatedThisTurn)
                        .ThenBy(GetMindShuffleReturnPriority)
                        .ToList();
                    IList<ClientCard> selected = SelectCount(returnCandidates, cards,
                        min, max, 1);
                    _pendingMindShuffleReturnCards.Clear();
                    if (selected != null)
                    {
                        foreach (ClientCard selectedCard in selected)
                        {
                            if (selectedCard != null)
                                _pendingMindShuffleReturnCards.Add(selectedCard);
                        }
                    }
                    return selected;
                }
            }

            if (chain.IsActivateCode(CardId.CrimsonCall) && hint == HintMsg.AddToHand)
                return SelectPreferredIds(cards, min, max, CardId.Griffoh);

            if (chain.IsActivateCode(CardId.LightAndDarknessRitual) &&
                hint == HintMsg.SpSummon)
                return SelectRitualMonster(cards, min, max);

            if (chain.IsActivateCode(CardId.LightAndDarknessRitual) &&
                hint == HintMsg.AddToHand)
                return SelectLightAndDarknessGraveCard(cards, min, max);

            if (chain.IsActivateCode(CardId.TitanikladTheAshDragon))
            {
                if (hint == HintMsg.AddToHand || hint == HintMsg.SpSummon)
                    return SelectPreferredIds(cards, min, max,
                        CardId.FallenOfTheWhiteDragon);
            }

            if (chain.IsActivateCode(CardId.AlbionTheBrandedDragon) &&
                (hint == HintMsg.AddToHand || hint == HintMsg.Set))
            {
                // Prefer placing The Fallen directly on the field. The Set
                // branch gives this deck immediate interaction for the next
                // turn; Add to hand is only the fallback when Set is not
                // available or the server only offers the hand candidate.
                IList<ClientCard> fallen = SelectPreferredIds(cards, min, max,
                    CardId.TheFallenTheVirtuous);
                if (fallen != null)
                    return fallen;

                IList<ClientCard> branded = SelectCount(cards.Where(c => c != null &&
                    c.HasSetcode(0x15d)), cards, min, max, 1);
                if (branded != null)
                    return branded;
            }

            if (chain.IsActivateCode(CardId.RedNovaDragonBurningSoul) &&
                hint == HintMsg.AddToHand)
            {
                return SelectPreferredIds(cards, min, max,
                    CardId.LightAndDarknessRitual, CardId.RaggedRecordsOfRites,
                    CardId.MindShuffle, CardId.CrimsonCall);
            }

            if (chain.IsActivateCode(CardId.TheCrimsonKing) &&
                hint == HintMsg.AddToHand)
            {
                return SelectPreferredIds(cards, min, max,
                    CardId.CrimsonCall, CardId.RedDragonArchfiend,
                    CardId.TheCrimsonKing);
            }

            if (chain.IsActivateCode(CardId.HeraldOfTheArcLight) &&
                hint == HintMsg.AddToHand)
            {
                IList<ClientCard> ritualMonster = SelectRitualMonster(cards, min, max);
                if (ritualMonster != null)
                    return ritualMonster;
                return SelectPreferredIds(cards, min, max,
                    CardId.LightAndDarknessRitual);
            }

            if (chain.IsActivateCode(CardId.ChaosAngel) &&
                (hint == HintMsg.Target || hint == HintMsg.Remove))
                return SelectEffectTarget(cards, min, max, false);

            if (chain.IsActivateCode(CardId.PsychicEndPunisher) &&
                (hint == HintMsg.Target || hint == HintMsg.Remove))
            {
                List<ClientCard> ownMonsters = cards.Where(c => c != null &&
                    c.Controller == 0 && c.IsMonster()).OrderBy(c => c.Attack).ToList();
                if (ownMonsters.Count > 0)
                    return SelectCount(ownMonsters, cards, min, max, 1);
                return SelectEffectTarget(cards, min, max, false);
            }

            if (chain.IsActivateCode(CardId.EnigmasterPackbit) &&
                hint == HintMsg.Target)
            {
                return SelectEffectTarget(cards.Where(c => c != null &&
                    c.Controller == 1 && c.IsMonster() && c.IsFaceup()).ToList(),
                    min, max, false);
            }

            if (chain.IsActivateCode(CardId.EcclesiaAndTheDarkDragon) &&
                (hint == HintMsg.Target || hint == HintMsg.ToDeck))
                return SelectEcclesiaAndTheDarkDragonTarget(cards, min, max);

            if ((chain.IsActivateCode(CardId.WindPegasusIgnister) ||
                chain.IsActivateCode(CardId.GoldenCloudBeastMalong)) &&
                hint == HintMsg.Target)
                return SelectEffectTarget(cards, min, max, false);

            if (chain.IsActivateCode(CardId.FydraulisHarmonia) && hint == HintMsg.Confirm)
            {
                List<ClientCard> candidates = cards.Where(c => c != null &&
                    c.HasType(CardType.Synchro)).ToList();
                _pendingFydraulisSynchroToGraveId =
                    GetPreferredFydraulisSynchroToGraveId(candidates);

                List<ClientCard> synchros = new List<ClientCard>();
                if (_pendingFydraulisSynchroToGraveId != 0)
                {
                    ClientCard payload = candidates.FirstOrDefault(c =>
                        c.IsCode(_pendingFydraulisSynchroToGraveId));
                    if (payload != null)
                        synchros.Add(payload);
                }
                synchros.AddRange(candidates.Where(c => !synchros.Contains(c))
                    .OrderBy(c => c.Attack));
                synchros = synchros.Take(Math.Min(5, max)).ToList();
                if (synchros.Count >= min)
                    return Util.CheckSelectCount(synchros, cards, min, max);
            }

            if (chain.IsActivateCode(CardId.FydraulisHarmonia) && hint == HintMsg.ToGrave)
                return SelectFydraulisSynchroToGrave(cards, min, max);

            if (chain.IsActivateCode(CardId.FydraulisHarmonia) &&
                (hint == HintMsg.Destroy || hint == HintMsg.Target))
            {
                List<ClientCard> ordered = GetOrderedFydraulisTargets(
                    cards.Where(c => c.Controller == 1 && c.IsMonster()));
                IEnumerable<ClientCard> fallback = cards.Where(c => c != null &&
                    c.Controller == 1 && c.IsMonster() && !ordered.Contains(c))
                    .OrderByDescending(GetFydraulisTargetPriority)
                    .ThenByDescending(c => c.GetDefensePower());

                // Our own strategic reservation remains selectable here.
                // Reservations made by The Fallen or another interaction are
                // still excluded. If the original monster left the field,
                // fall back to the best currently legal unreserved monster.
                List<ClientCard> candidates = new List<ClientCard>();
                if (_pendingFydraulisDestructionTarget != null &&
                    cards.Contains(_pendingFydraulisDestructionTarget))
                {
                    candidates.Add(_pendingFydraulisDestructionTarget);
                }
                candidates.AddRange(ordered.Concat(fallback).Where(c =>
                    c != _pendingFydraulisDestructionTarget &&
                    !IsOpponentTargetReserved(c)));
                return SelectCount(candidates, cards, min, max, 1);
            }

            if (chain.IsActivateCode(CardId.Griffoh) && hint == HintMsg.Set)
                return SelectGriffohSetCard(cards, min, max);

            if (chain.IsActivateCode(CardId.BlackSkullDragonTheArchfiendDragonOfUnity) &&
                hint == HintMsg.Discard)
                return SelectBlackSkullDragonDiscardCard(cards, min, max);

            if (chain.IsActivateCode(CardId.FallenOfTheWhiteDragon))
            {
                if (hint == HintMsg.ToGrave)
                    return SelectFallenExtraDeckCost(cards, min, max);
                if (hint == HintMsg.SpSummon)
                    return SelectPreferredIds(cards, min, max,
                        CardId.IncredibleEcclesiaTheVirtuous);
            }

            if (chain.IsActivateCode(CardId.BlackSkullDragonTheArchfiendDragonOfUnity) &&
                hint == HintMsg.Set)
            {
                return SelectBlackSkullDragonSupportCard(cards, min, max);
            }

            if (chain.IsActivateCode(CardId.BlackChaos) &&
                (hint == HintMsg.Set || hint == HintMsg.ToField ||
                    hint == HintMsg.AddToHand))
            {
                IList<ClientCard> selected = SelectPreferredIds(cards, min, max,
                    CardId.MindShuffle);
                _pendingBlackChaosSupportSearch = false;
                if (selected != null)
                    ReservePendingDeckSearch(selected);
                return selected;
            }

            if (chain.IsActivateCode(CardId.SkullArchfiendOfChaos))
            {
                if (hint == HintMsg.ToGrave)
                    return SelectPreferredIds(cards, min, max, CardId.LightAndDarknessRitual);
                if (hint == HintMsg.AddToHand)
                    return SelectRitualMonster(cards, min, max);
                if (hint == HintMsg.ToDeck)
                    return SelectSkullArchfiendRecycleCards(cards, min, max);
            }

            if (chain.IsActivateCode(CardId.MagicianOfDarkChaosBlackChaos) &&
                hint == HintMsg.AddToHand)
            {
                return SelectPreferredIds(cards, min, max,
                    CardId.TheFallenTheVirtuous, CardId.SpatialTrunade,
                    CardId.CrimsonCall, CardId.RaggedRecordsOfRites,
                    CardId.SpellShatteringSword, CardId.LightAndDarknessRitual);
            }

            if (chain.IsActivateCode(CardId.MagicianOfDarkChaosBlackChaos) &&
                (hint == HintMsg.Remove || hint == HintMsg.Target))
                return SelectEffectTarget(cards, min, max, false);

            if (chain.IsActivateCode(CardId.BlackLusterSoldierSoldierOfLightAndDarkness) &&
                (hint == HintMsg.Remove || hint == HintMsg.Target))
            {
                ClientCard problematic = Util.GetProblematicEnemyCard(0, true);
                if (problematic != null && cards.Contains(problematic))
                    return SelectNonOverlappingTarget(cards, min, max,
                        new List<ClientCard> { problematic });
                return SelectNonOverlappingTarget(cards, min, max,
                    cards.Where(c => c.Controller == 1)
                        .OrderByDescending(c => c.IsFaceup())
                        .ThenByDescending(c => c.GetDefensePower()));
            }

            if (chain.IsActivateCode(CardId.TheFallenTheVirtuous))
            {
                if (hint == HintMsg.ToGrave)
                    return SelectFallenExtraDeckCost(cards, min, max);
                if (hint == HintMsg.Destroy || hint == HintMsg.Target)
                {
                    List<ClientCard> ordered = GetOrderedFallenTargets(
                        cards.Where(c => c.Controller == 1));
                    IEnumerable<ClientCard> fallback = cards.Where(c => c != null &&
                        c.Controller == 1 && c.IsFaceup() && !ordered.Contains(c))
                        .OrderByDescending(GetFallenTargetPriority)
                        .ThenByDescending(c => Math.Max(c.Attack, c.Defense));
                    return SelectNonOverlappingTarget(cards, min, max,
                        ordered.Concat(fallback));
                }
            }

            if (chain.IsActivateCode(CardId.SpellShatteringSword))
            {
                if (hint == HintMsg.Confirm)
                    return SelectPreferredIds(cards, min, max, CardId.LightAndDarknessRitual);
                if (hint == HintMsg.Target || hint == HintMsg.Disable)
                    return SelectSpellShatteringSwordMonsterTarget(cards, min, max);
                if (hint == HintMsg.Destroy)
                    return SelectEffectTarget(cards, min, max, false, false);
            }

            if (chain.IsActivateCode(CardId.SpatialTrunade) && hint == HintMsg.ReturnToHand)
            {
                List<ClientCard> enemyCards = cards.Where(c => c.Controller == 1 &&
                        !IsOpponentTargetReserved(c))
                    .OrderByDescending(GetSpatialTrunadePriority)
                    .ThenByDescending(c => c.GetDefensePower()).ToList();
                if (enemyCards.Count > 0)
                    return SelectCount(enemyCards, cards, min, max, Math.Min(2, max));
            }

            return null;
        }

        private IList<ClientCard> SelectRitualMonster(IList<ClientCard> cards,
            int min, int max)
        {
            List<ClientCard> ordered = cards.Where(IsRitualMonster)
                .OrderBy(GetRitualMonsterPriority).ToList();
            return SelectCount(ordered, cards, min, max, 1);
        }

        private IList<ClientCard> SelectLightAndDarknessGraveCard(
            IList<ClientCard> cards, int min, int max)
        {
            return SelectPreferredIds(cards, min, max,
                CardId.BlackSkullDragonTheArchfiendDragonOfUnity,
                CardId.CelticMystic,
                CardId.SkullArchfiendOfChaos,
                CardId.MagicianOfDarkChaosBlackChaos,
                CardId.BlackLusterSoldierSoldierOfLightAndDarkness,
                CardId.LightAndDarknessRitual);
        }

        private IList<ClientCard> SelectMindShuffleSummonMonster(IList<ClientCard> cards,
            int min, int max)
        {
            ClientCard returnTarget = GetMindShuffleReturnTarget();
            bool allowUsedTargets = returnTarget != null &&
                IsOpponentTargetedByCurrentChain(returnTarget);
            List<int> summonOrder = GetMindShuffleSummonOrderForCurrentChain();
            List<ClientCard> ordered = new List<ClientCard>();
            foreach (int id in summonOrder)
            {
                ordered.AddRange(cards.Where(c => c != null &&
                    c.Location == CardLocation.Hand && c.IsCode(id) &&
                    (allowUsedTargets || !WasCardActivatedThisTurn(c)) &&
                    (returnTarget == null || !c.IsCode(returnTarget.Id)) &&
                    !ordered.Contains(c)));
            }
            ordered.AddRange(cards.Where(c => c != null && c.Location == CardLocation.Hand &&
                c.IsMonster() && (allowUsedTargets || !WasCardActivatedThisTurn(c)) &&
                (returnTarget == null || !c.IsCode(returnTarget.Id)) &&
                !ordered.Contains(c)));

            IList<ClientCard> selected = SelectCount(ordered, cards, min, max, 1);
            if (selected != null && selected.Count > 0)
                _pendingMindShuffleSummonId = selected[0].Id;
            return selected;
        }

        private int GetSpatialTrunadePriority(ClientCard card)
        {
            if (card == null || card.Controller != 1)
                return -1;

            ClientCard problematic = Util.GetProblematicEnemyCard(0, false);
            if (card == problematic)
                return 300;
            if (card.IsExtraCard())
                return 200;
            if (card.IsFacedown())
                return 100;
            return 0;
        }

        private IList<ClientCard> SelectFydraulisSynchroToGrave(IList<ClientCard> cards,
            int min, int max)
        {
            int preferredId = _pendingFydraulisSynchroToGraveId;
            if (preferredId == 0 || !cards.Any(c => c != null &&
                c.IsCode(preferredId)))
            {
                preferredId = GetPreferredFydraulisSynchroToGraveId(cards);
            }

            _pendingFydraulisSynchroToGraveId = 0;
            if (preferredId != 0)
            {
                IList<ClientCard> selected = SelectPreferredIds(cards, min, max,
                    preferredId);
                if (selected != null)
                    return selected;
            }
            return SelectCount(cards.Where(c => c != null && c.HasType(CardType.Synchro))
                .OrderBy(c => c.Attack), cards, min, max, 1);
        }

        private int GetPreferredFydraulisSynchroToGraveId(
            IEnumerable<ClientCard> available)
        {
            List<ClientCard> candidates = available.Where(c => c != null &&
                c.HasType(CardType.Synchro)).ToList();
            List<int> preferredIds = new List<int>();
            int enemyMonsterCount = Enemy.GetMonsterCount();
            if (enemyMonsterCount >= 2 &&
                (HasDuplicateCardInHand() || !CanActivatePurulia()))
            {
                preferredIds.Add(CardId.EnigmasterPackbit);
            }
            if (enemyMonsterCount >= 2)
                preferredIds.Add(CardId.GoldenCloudBeastMalong);
            if (_ownMonsterReleasedToGraveThisTurn)
                preferredIds.Add(CardId.StardustDragonVictimSanctuary);
            if (!Bot.Hand.Any(IsRitualMonster))
                preferredIds.Add(CardId.HeraldOfTheArcLight);
            if (enemyMonsterCount == 1)
                preferredIds.Add(CardId.WindPegasusIgnister);

            return preferredIds.FirstOrDefault(id =>
                candidates.Any(c => c.IsCode(id)));
        }

        private IList<ClientCard> SelectSkullArchfiendRecycleCards(IList<ClientCard> cards,
            int min, int max)
        {
            List<ClientCard> candidates = LimitSkullArchfiendRitualCopies(
                cards.Where(IsSkullArchfiendRecycleCandidate));
            if (candidates.Count < min)
                return null;

            List<ClientCard> ordered = candidates
                .OrderBy(GetSkullArchfiendRecycleLocationPriority)
                .ThenBy(GetSkullArchfiendRecycleCardPriority)
                .ThenByDescending(c => IsDuplicateInGrave(c, cards))
                .ToList();

            // The script's gcheck requires at least one of the three selected
            // cards to have "Light and Darkness Ritual" in its card text.
            // SelectSubGroup cannot roll back an invalid preselection, so do
            // not rely on sorting alone: explicitly place the highest-priority
            // qualifying card inside the selected prefix. The activation check
            // already requires one known qualifying card; returning null here
            // only protects against an unexpected candidate-snapshot mismatch.
            ClientCard ritualRelated = ordered.FirstOrDefault(IsRitualRelatedCard);
            if (ritualRelated == null)
                return null;
            ordered.Remove(ritualRelated);
            ordered.Insert(0, ritualRelated);
            int desired = Math.Min(max, Math.Max(min, 3));
            return SelectCount(ordered, cards, min, max, desired);
        }

        private int GetSkullArchfiendRecycleLocationPriority(ClientCard card)
        {
            if (card != null && card.IsCode(CardId.MindShuffle) &&
                card.Location == CardLocation.Removed)
                return 0;
            if (card != null && card.IsCode(CardId.MindShuffle) &&
                card.Location == CardLocation.Grave)
                return 1;
            if (card != null && card.Location == CardLocation.Removed)
                return 2;
            return 3;
        }

        private int GetSkullArchfiendRecycleCardPriority(ClientCard card)
        {
            if (card == null)
                return 99;
            if (card.IsCode(CardId.BlackChaos))
                return 0;
            if (card.IsCode(CardId.BlackSkullDragonTheArchfiendDragonOfUnity))
                return 1;
            if (card.IsCode(CardId.TheFallenTheVirtuous))
                return 2;
            if (card.IsCode(CardId.MaxxC))
                return 3;
            if (card.IsCode(CardId.FallenOfTheWhiteDragon))
                return 4;
            if (card.IsCode(CardId.Griffoh))
                return 5;
            if (card.IsCode(CardId.CelticMystic))
                return 6;
            if (card.IsCode(CardId.AshBlossomJoyousSpring))
                return 7;
            if (card.IsCode(CardId.SkullArchfiendOfChaos))
                return 8;
            return 9;
        }

        private bool IsOwnGraveyardRitualMonster(ClientCard card)
        {
            return card != null && card.Controller == 0 &&
                card.Location == CardLocation.Grave &&
                HasKnownCardType(card, CardType.Monster) &&
                HasKnownCardType(card, CardType.Ritual);
        }

        private bool IsDuplicateInGrave(ClientCard card, IList<ClientCard> cards)
        {
            return card != null && card.Id != 0 && cards.Count(c => c != null &&
                c.Location == CardLocation.Grave && c.Id == card.Id) > 1;
        }

        private IList<ClientCard> SelectGriffohSetCard(IList<ClientCard> cards,
            int min, int max)
        {
            // Griffoh's hand effect is specifically intended to establish
            // Mind Shuffle.  Having one copy in hand is not a reason to
            // skip the search: a second copy can be set from the Deck while
            // the hand copy remains available.  Only an already established
            // field copy, or no remaining Deck copy, allows the fallback
            // support cards to move ahead of Mind Shuffle.
            if (!HasPendingBlackChaosSupportSearch() &&
                !HasMindShuffleOnField() && Bot.HasInDeck(CardId.MindShuffle))
            {
                IList<ClientCard> mindShuffle = SelectCount(cards.Where(c =>
                    c != null && c.IsCode(CardId.MindShuffle) &&
                    !IsPendingDeckSearch(c)), cards, min, max, 1);
                if (mindShuffle != null)
                {
                    ReservePendingDeckSearch(mindShuffle);
                    return mindShuffle;
                }
            }

            if (HasPendingBlackChaosSupportSearch())
            {
                // Black Chaos was activated earlier in this same chain and
                // still needs Mind Shuffle for its own resolution. Let
                // Griffoh consume another legal support card first.
                int[] alternatives =
                {
                    CardId.SpellShatteringSword
                };
                foreach (int id in alternatives)
                {
                    if (IsSearchTargetUnavailable(id))
                        continue;

                    IList<ClientCard> selected = SelectPreferredIds(cards,
                        min, max, id);
                    if (selected != null)
                    {
                        ReservePendingDeckSearch(selected);
                        return selected;
                    }
                }

                // Black Chaos will consume the available Mind Shuffle when it
                // resolves later in this chain. If no different legal support
                // card remains, do not preselect the same Deck copy for both
                // effects.
                return null;
            }

            return SelectCommonRitualSupportCard(cards, min, max);
        }

        private bool HasPendingBlackChaosSupportSearch()
        {
            return _pendingBlackChaosSupportSearch;
        }

        private IList<ClientCard> SelectBlackSkullDragonSupportCard(
            IList<ClientCard> cards, int min, int max)
        {
            if (ShouldPrioritizeLightAndDarknessRitual())
            {
                IList<ClientCard> ritual = SelectPreferredIdPreferGrave(cards, min,
                    max, CardId.LightAndDarknessRitual);
                if (ritual != null)
                    return ritual;
            }

            int[] preferredIds =
            {
                CardId.MindShuffle,
                CardId.SpellShatteringSword,
                CardId.LightAndDarknessRitual
            };
            foreach (int id in preferredIds)
            {
                if (IsSearchTargetUnavailable(id))
                    continue;

                IList<ClientCard> selected = SelectPreferredIdPreferGrave(cards,
                    min, max, id);
                if (selected != null)
                    return selected;
            }

            return null;
        }

        private IList<ClientCard> SelectRaggedRecordsSearchCard(IList<ClientCard> cards,
            int min, int max)
        {
            int[] preferredIds =
            {
                CardId.Griffoh,
                CardId.BlackChaos,
                CardId.CelticMystic,
                CardId.BlackSkullDragonTheArchfiendDragonOfUnity
            };

            // Prefer a card that is both unused this turn and absent from the
            // hand. If that combination is unavailable, keep the same two
            // criteria as the next tie-breakers before using the original
            // card-purpose order.
            List<ClientCard> ordered = cards.Where(c => c != null &&
                    preferredIds.Contains(c.Id) && !IsPendingDeckSearch(c))
                .OrderBy(c =>
                {
                    bool unused = !WasCardActivatedThisTurn(c);
                    bool missingFromHand = !Bot.HasInHand(c.Id);
                    if (unused && missingFromHand)
                        return 0;
                    if (unused)
                        return 1;
                    if (missingFromHand)
                        return 2;
                    return 3;
                })
                .ThenBy(c => Array.IndexOf(preferredIds, c.Id))
                .ToList();
            if (ordered.Count > 0)
                return Util.CheckSelectCount(ordered, cards, min, max);

            return SelectRitualMonster(cards, min, max);
        }

        private IList<ClientCard> SelectMindShuffleSearchCard(IList<ClientCard> cards,
            int min, int max)
        {
            if (!ShouldUseMindShuffleFallback())
            {
                IList<ClientCard> skull = SelectHandMissingIds(cards, min, max,
                    CardId.SkullArchfiendOfChaos);
                if (skull != null)
                    return skull;
            }

            IList<ClientCard> fallback = SelectHandMissingIds(cards, min, max,
                CardId.BlackSkullDragonTheArchfiendDragonOfUnity,
                CardId.MagicianOfDarkChaosBlackChaos,
                CardId.BlackLusterSoldierSoldierOfLightAndDarkness,
                CardId.BlackChaos, CardId.CelticMystic, CardId.Griffoh);
            if (fallback != null)
                return fallback;

            return SelectRitualMonster(cards, min, max);
        }

        private IList<ClientCard> SelectBlackSkullDragonDiscardCard(
            IList<ClientCard> cards, int min, int max)
        {
            List<ClientCard> ordered = new List<ClientCard>();
            ordered.AddRange(cards.Where(c => c != null &&
                c.IsCode(CardId.LightAndDarknessRitual)));
            ordered.AddRange(cards.Where(IsDuplicateSpellInHand)
                .Where(c => !ordered.Contains(c)));
            ordered.AddRange(cards.Where(c => c != null &&
                c.IsCode(CardId.SpatialTrunade) &&
                !CanUseSpatialTrunadeOnEnemyField())
                .Where(c => !ordered.Contains(c)));
            ordered.AddRange(cards.Where(c => IsBlackSkullDiscardSpell(c) &&
                WasCardActivatedThisTurn(c)).Where(c => !ordered.Contains(c)));

            if (ordered.Count == 0)
                return null;
            return Util.CheckSelectCount(ordered, cards, min, max);
        }

        private IList<ClientCard> SelectMindShuffleDiscardCards(IList<ClientCard> cards,
            int min, int max)
        {
            List<ClientCard> ordered = new List<ClientCard>();
            if (!ShouldUseMindShuffleFallback())
            {
                ordered.AddRange(cards.Where(c => c != null &&
                    c.IsCode(CardId.SkullArchfiendOfChaos)));
                ordered.AddRange(GetOrderedDiscardCards(cards)
                    .Where(c => !ordered.Contains(c)));
            }
            else
            {
                ordered.AddRange(cards.Where(c => c != null &&
                    c.IsCode(CardId.MulcharmyPurulia)));
                ordered.AddRange(cards.Where(c => IsDuplicateInSelection(c, cards))
                    .Where(c => !ordered.Contains(c)));
                ordered.AddRange(cards.Where(c => c != null &&
                    c.IsCode(CardId.TheWorldsGreatestGallantThief))
                    .Where(c => !ordered.Contains(c)));
                ordered.AddRange(GetOrderedDiscardCards(cards)
                    .Where(c => !ordered.Contains(c)));
            }

            if (ordered.Count == 0)
                return null;
            return Util.CheckSelectCount(ordered, cards, min, max);
        }

        private IList<ClientCard> SelectCelticMysticDiscardCards(IList<ClientCard> cards,
            int min, int max)
        {
            List<ClientCard> ordered = new List<ClientCard>();
            ordered.AddRange(cards.Where(c => c != null && c.IsCode(
                CardId.MulcharmyPurulia, CardId.SkullArchfiendOfChaos,
                CardId.TheWorldsGreatestGallantThief)));
            ordered.AddRange(cards.Where(c => IsDuplicateInSelection(c, cards))
                .Where(c => !ordered.Contains(c)));
            ordered.AddRange(cards.Where(WasCardActivatedThisTurn)
                .Where(c => !ordered.Contains(c)));
            ordered.AddRange(cards.Where(c => !ordered.Contains(c))
                .OrderBy(GetDiscardPriority));

            if (ordered.Count == 0)
                return null;
            return Util.CheckSelectCount(ordered, cards, min, max);
        }

        private IList<ClientCard> SelectCommonRitualSupportCard(IList<ClientCard> cards,
            int min, int max)
        {
            int[] preferredIds =
            {
                CardId.MindShuffle,
                CardId.SpellShatteringSword
            };

            foreach (int id in preferredIds)
            {
                if (IsSearchTargetUnavailable(id))
                    continue;

                IList<ClientCard> selected = SelectPreferredIds(cards, min, max, id);
                if (selected != null)
                {
                    ReservePendingDeckSearch(selected);
                    return selected;
                }
            }

            return null;
        }

        private IList<ClientCard> SelectHandMissingIds(IList<ClientCard> cards,
            int min, int max, params int[] ids)
        {
            List<ClientCard> ordered = new List<ClientCard>();
            foreach (int id in ids)
            {
                if (Bot.HasInHand(id))
                    continue;
                ordered.AddRange(cards.Where(c => c != null && c.IsCode(id) &&
                    !IsPendingDeckSearch(c) &&
                    !ordered.Contains(c)));
            }

            if (ordered.Count == 0)
                return null;
            return Util.CheckSelectCount(ordered, cards, min, max);
        }

        private List<ClientCard> GetOrderedDiscardCards(IList<ClientCard> cards)
        {
            return cards.OrderBy(c => IsDuplicateInSelection(c, cards) ? 0 : 1)
                .ThenBy(c => WasCardActivatedThisTurn(c) ? 0 : 1)
                .ThenBy(GetDiscardPriority).ToList();
        }

        private IList<ClientCard> SelectEffectTarget(IList<ClientCard> cards,
            int min, int max, bool allowOwnFallback, bool targeted = true)
        {
            ClientCard problematic = Util.GetProblematicEnemyCard(0, true);
            List<ClientCard> enemyCards = cards.Where(c => c != null &&
                    c.Controller == 1)
                .OrderByDescending(c => GetGeneralRemovalPriority(c,
                    problematic, targeted))
                .ThenByDescending(c => Math.Max(c.Attack, c.Defense)).ToList();
            if (enemyCards.Count > 0)
            {
                IList<ClientCard> enemySelection =
                    SelectNonOverlappingTarget(cards, min, max, enemyCards);
                if (enemySelection != null)
                    return enemySelection;
            }

            if (!allowOwnFallback)
                return null;

            return SelectCount(cards.Where(c => c != null && c.Controller == 0)
                .OrderBy(c => c.GetDefensePower()), cards, min, max, 1);
        }

        private int GetGeneralRemovalPriority(ClientCard card,
            ClientCard problematic, bool targeted)
        {
            if (card == null || card.Controller != 1)
                return -1;

            int priority = card == problematic ? 1200 : 0;
            if (card.IsMonster())
            {
                if (card.IsFloodgate())
                    priority = Math.Max(priority, 1100);
                else if (card.IsMonsterDangerous())
                    priority = Math.Max(priority, 1050);
                else if (card.IsMonsterShouldBeDisabledBeforeItUseEffect())
                    priority = Math.Max(priority, 1000);
                else if (IsValuableFallenExtraDeckMonster(card))
                    priority = Math.Max(priority, 850);
                else if (card.IsFaceup())
                    priority = Math.Max(priority, 500);

                if (card.IsDisabled())
                    priority -= 300;
                if (targeted && CanLikelyEscapeFallenTarget(card))
                    priority -= 450;
                return priority;
            }

            if (card.IsFloodgate())
                priority = Math.Max(priority, 1100);
            else if (card.IsFacedown())
                priority = Math.Max(priority, 800);
            else if (card.HasType(CardType.Field))
                priority = Math.Max(priority, 750);
            else if (card.HasType(CardType.Continuous))
                priority = Math.Max(priority, 700);
            else
                priority = Math.Max(priority, 400);
            return priority;
        }

        private IList<ClientCard> SelectEcclesiaAndTheDarkDragonTarget(
            IList<ClientCard> cards, int min, int max)
        {
            // Dark Ecclesia's Graveyard effect is intended to recycle only by
            // returning an opponent's card. Never use the generic target
            // helper's own-card fallback here.
            List<ClientCard> enemyCards = cards.Where(c => c != null &&
                c.Controller == 1 && c.IsOnField()).ToList();
            if (enemyCards.Count == 0)
                return null;

            ClientCard problematic = Util.GetProblematicEnemyCard(0, true);
            List<ClientCard> ordered = enemyCards
                .OrderBy(GetEcclesiaTargetPriority)
                .ThenByDescending(c => c.GetDefensePower())
                .ToList();

            // Keep the common threat detector as the first tie-breaker, but
            // only when that card is one of the server-provided legal targets.
            if (problematic != null && ordered.Contains(problematic))
            {
                ordered.Remove(problematic);
                ordered.Insert(0, problematic);
            }

            return SelectNonOverlappingTarget(cards, min, max, ordered);
        }

        private int GetEcclesiaTargetPriority(ClientCard card)
        {
            if (card == null)
                return 99;

            if (card.IsFloodgate() || card.IsMonsterDangerous() ||
                card.IsMonsterInvincible() ||
                card.IsMonsterShouldBeDisabledBeforeItUseEffect())
                return 0;

            if ((card.IsSpell() || card.IsTrap()) && card.IsFacedown())
                return 1;

            if (card.IsMonster())
                return 2;

            if (card.IsSpell() || card.IsTrap())
                return 3;

            return 4;
        }

        private IList<ClientCard> SelectPreferredIds(IList<ClientCard> cards,
            int min, int max, params int[] ids)
        {
            List<ClientCard> ordered = new List<ClientCard>();
            foreach (int id in ids)
                ordered.AddRange(cards.Where(c => c != null && c.IsCode(id) &&
                    !IsPendingDeckSearch(c) &&
                    !ordered.Contains(c)));
            if (ordered.Count == 0)
                return null;
            return Util.CheckSelectCount(ordered, cards, min, max);
        }

        private IList<ClientCard> SelectPreferredIdPreferGrave(
            IList<ClientCard> cards, int min, int max, int id)
        {
            List<ClientCard> ordered = cards.Where(c => c != null &&
                    c.IsCode(id) && !IsPendingDeckSearch(c))
                .OrderBy(c => IsGraveCard(c) ? 0 : 1)
                .ToList();
            if (ordered.Count == 0)
                return null;

            return Util.CheckSelectCount(ordered, cards, min, max);
        }

        private IList<ClientCard> SelectFallenExtraDeckCost(IList<ClientCard> cards,
            int min, int max)
        {
            // Both White Dragon and The Fallen send one Extra Deck card as
            // cost. Keep the same explicit order for both effects so the
            // activation-stage selector cannot fall back to an unrelated
            // Extra Deck card.
            return SelectPreferredIds(cards, min, max,
                CardId.AlbionTheBrandedDragon,
                CardId.EcclesiaAndTheDarkDragon,
                CardId.TitanikladTheAshDragon,
                CardId.AlbaLenatusTheAbyssDragon);
        }

        private IList<ClientCard> SelectBlackChaosSpecialSummonReturn(
            IList<ClientCard> cards, int min, int max)
        {
            List<ClientCard> graveCards = cards.Where(c => c != null &&
                IsGraveCard(c)).ToList();
            List<ClientCard> handCards = cards.Where(c => c != null &&
                (((int)c.Location & (int)CardLocation.Hand) != 0)).ToList();

            List<ClientCard> ordered = graveCards.Concat(handCards).ToList();
            if (ordered.Count < min)
                return null;

            return Util.CheckSelectCount(ordered, cards, min, max);
        }

        private IList<ClientCard> SelectNonOverlappingTarget(IList<ClientCard> cards,
            int min, int max, IEnumerable<ClientCard> preferred)
        {
            List<ClientCard> legal = preferred.Where(c => c != null &&
                cards.Contains(c)).Distinct().ToList();
            if (legal.Count == 0)
                return null;

            // Only targets committed by our own effects are reserved. Opponent
            // effects targeting their own cards must not make those cards look
            // unavailable to our interaction.
            List<ClientCard> unused = legal.Where(c => !IsOpponentTargetReserved(c))
                .ToList();
            if (unused.Count < min)
                return null;
            return SelectCount(unused, cards, min, max, 1);
        }

        private IList<ClientCard> SelectCount(IEnumerable<ClientCard> preferred,
            IList<ClientCard> cards, int min, int max, int desired)
        {
            List<ClientCard> ordered = preferred.Where(c => c != null && cards.Contains(c))
                .Distinct().Take(Math.Min(max, desired)).ToList();
            if (ordered.Count < min)
                return null;
            return Util.CheckSelectCount(ordered, cards, min, max);
        }

        private bool IsOpponentTargetSelectionHint(int hint)
        {
            return hint == HintMsg.Target || hint == HintMsg.Destroy ||
                hint == HintMsg.Remove || hint == HintMsg.ReturnToHand ||
                hint == HintMsg.Disable;
        }

        private bool IsValidTargetSelection(IList<ClientCard> selected,
            IList<ClientCard> cards, int min, int max)
        {
            return selected != null && selected.Count >= min && selected.Count <= max &&
                selected.Distinct().Count() == selected.Count &&
                selected.All(c => c != null && cards.Contains(c));
        }

        private void ReserveOpponentTargets(IList<ClientCard> selected)
        {
            foreach (ClientCard card in selected)
            {
                if (card != null && card.Controller == 1)
                    _reservedOpponentTargets.Add(card);
            }
        }

        private bool IsOpponentTargetReserved(ClientCard card)
        {
            if (card == null)
                return false;
            if (_reservedOpponentTargets.Contains(card))
                return true;

            return Duel.CurrentChainInfo != null &&
                Duel.CurrentChainInfo.Any(chain => chain != null &&
                    chain.ActivatePlayer == 0 && chain.Targets != null &&
                    chain.Targets.Contains(card));
        }

        private bool HasRitualMonsterSearchTarget()
        {
            return Bot.HasInDeck(CardId.BlackLusterSoldierSoldierOfLightAndDarkness) ||
                Bot.HasInDeck(CardId.MagicianOfDarkChaosBlackChaos) ||
                Bot.HasInDeck(CardId.SkullArchfiendOfChaos) ||
                Bot.HasInDeck(CardId.BlackSkullDragonTheArchfiendDragonOfUnity) ||
                Bot.HasInDeck(CardId.BlackChaos) ||
                Bot.HasInDeck(CardId.CelticMystic) ||
                Bot.HasInDeck(CardId.Griffoh);
        }

        private List<ClientCard> GetRitualRecycleCandidates()
        {
            return LimitSkullArchfiendRitualCopies(Bot.Graveyard.Concat(Bot.Banished)
                .Concat(Enemy.Graveyard).Concat(Enemy.Banished)
                .Where(IsSkullArchfiendRecycleCandidate)
                );
        }

        private List<ClientCard> LimitSkullArchfiendRitualCopies(
            IEnumerable<ClientCard> source)
        {
            List<ClientCard> candidates = source.Where(c => c != null).ToList();
            ClientCard ritual = candidates.FirstOrDefault(c => c.IsCode(
                CardId.LightAndDarknessRitual));

            if (ritual == null)
                return candidates;

            // Skull Archfiend may recover three cards, but this strategy never
            // returns more than one copy of Light and Darkness Ritual in the
            // same activation. This is especially important when two copies
            // are both in our Graveyard.
            return candidates.Where(c => !c.IsCode(CardId.LightAndDarknessRitual) ||
                c == ritual).ToList();
        }

        private bool IsSkullArchfiendRecycleCandidate(ClientCard card)
        {
            if (card == null || card.Id == 0 ||
                card.IsCode(CardId.SkullArchfiendOfChaos) ||
                IsOwnGraveyardRitualMonster(card))
                return false;

            // Preserve the last copy of Light and Darkness Ritual in the
            // Graveyard. It may be returned by Skull Archfiend only when two
            // copies are already present there.
            if (card.IsCode(CardId.LightAndDarknessRitual) &&
                Bot.Graveyard.Count(c => c != null &&
                    c.IsCode(CardId.LightAndDarknessRitual)) < 2)
                return false;

            return true;
        }

        private bool IsSkullArchfiendReturnPending(ClientCard card)
        {
            if (card == null)
                return false;

            if (_pendingLightAndDarknessReturnCards.Contains(card))
                return true;

            // The return card is selected during resolution, but Skull
            // Archfiend can be offered its Graveyard effect while the chain
            // is still being built. Until the selection is known, conservatively
            // reserve Skull whenever our Graveyard Ritual effect is pending.
            return Duel.CurrentChainInfo.Any(chain => chain != null &&
                chain.ActivatePlayer == 0 &&
                chain.IsActivateCode(CardId.LightAndDarknessRitual) &&
                chain.HasLocation(CardLocation.Grave));
        }

        private bool CanUseMindShuffleSummon()
        {
            ClientCard returnTarget = GetMindShuffleReturnTarget();
            ClientCard summonTarget = GetMindShuffleSummonTarget();
            if (returnTarget == null || summonTarget == null)
                return false;

            // Once every hand monster that Mind Shuffle could summon has
            // already used its relevant effect this turn, do not keep
            // returning and resummoning cards for marginal value. The only
            // exception is when the selected return monster is itself being
            // targeted by the opponent and must be moved away.
            if (!IsOpponentTargetedByCurrentChain(returnTarget) &&
                !HasFreshMindShuffleSummonCandidate(returnTarget))
                return false;

            if (IsMindShuffleTargetedByOpponentChain() &&
                returnTarget.IsCode(CardId.MagicianOfDarkChaosBlackChaos) &&
                !HasNonMagicianMindShuffleReturnCandidate())
                return false;

            return !summonTarget.IsCode(returnTarget.Id);
        }

        private bool HasMindShuffleBreakthroughReason()
        {
            if (IsMindShuffleProtectionThreat())
                return true;

            if (Enemy.GetMonsterCount() > 0)
                return true;

            if (Enemy.GetSpells().Any(c => c.IsFacedown() || c.HasType(CardType.Continuous)))
                return true;

            return Bot.GetMonsters().Any(c => c.Level >= 7 && Util.IsChainTarget(c));
        }

        private bool HasLatestOpponentChain()
        {
            ClientCard latest = Duel.GetCurrentChainCard();
            return latest != null && latest.Controller == 1;
        }

        private bool IsMindShuffleTargetedByOpponentChain()
        {
            return Bot.GetSpells().Any(c => c != null &&
                c.IsCode(CardId.MindShuffle) && c.IsFaceup() &&
                IsOpponentTargetedByCurrentChain(c));
        }

        private bool IsMindShuffleProtectionThreat()
        {
            return IsMindShuffleTargetedByOpponentChain() ||
                IsOpponentNonTargetingMassDestructionChain();
        }

        private bool IsOpponentNonTargetingMassDestructionChain()
        {
            ChainInfo chain = GetLatestOpponentChainInfo();
            if (chain == null || chain.ActivatePlayer != 1 ||
                (chain.Targets != null && chain.Targets.Count > 0))
                return false;

            // These are the common non-targeting board-wipe effects. The
            // server does not expose a general "destroy all" flag, so keep
            // the card list explicit instead of guessing from an unrelated
            // non-targeting effect.
            return chain.IsActivateCode(CardId.BlackRoseDragon,
                    CardId.HarpiesFeatherDuster, CardId.Raigeki,
                    CardId.LightningStorm);
        }

        private ClientCard GetMindShuffleSummonTarget()
        {
            ClientCard returnTarget = GetMindShuffleReturnTarget();
            foreach (int id in GetMindShuffleSummonOrderForCurrentChain())
            {
                ClientCard target = Bot.Hand.FirstOrDefault(c => c != null && c.IsCode(id) &&
                    !WasCardActivatedThisTurn(c) &&
                    (returnTarget == null || !c.IsCode(returnTarget.Id)));
                if (target != null)
                    return target;
            }

            if (returnTarget != null && IsOpponentTargetedByCurrentChain(returnTarget))
            {
                foreach (int id in GetMindShuffleSummonOrderForCurrentChain())
                {
                    ClientCard target = Bot.Hand.FirstOrDefault(c => c != null &&
                        c.IsCode(id) && c.IsMonster() &&
                        (returnTarget == null || !c.IsCode(returnTarget.Id)));
                    if (target != null)
                        return target;
                }
            }
            return null;
        }

        private List<int> GetMindShuffleSummonOrderForCurrentChain()
        {
            EnsureMindShufflePriorityOverride();
            List<int> order = _mindShuffleSummonOrder.ToList();
            if (IsMindShuffleProtectionThreat())
            {
                order.Remove(CardId.MagicianOfDarkChaosBlackChaos);
                order.Insert(0, CardId.MagicianOfDarkChaosBlackChaos);
            }
            return order;
        }

        private bool HasNonMagicianMindShuffleReturnCandidate()
        {
            return Bot.GetMonsters().Any(c => IsMindShuffleReturnCandidate(c) &&
                !c.IsCode(CardId.MagicianOfDarkChaosBlackChaos));
        }

        private ClientCard GetMindShuffleReturnTarget()
        {
            List<ClientCard> candidates = Bot.GetMonsters()
                .Where(IsMindShuffleReturnCandidate).ToList();

            bool magicianTargeted = candidates.Any(c =>
                c.IsCode(CardId.MagicianOfDarkChaosBlackChaos) &&
                IsOpponentTargetedByCurrentChain(c));

            // When the opponent is removing Mind Shuffle itself, or is using
            // a non-targeting board wipe, never spend the Magician as the
            // return cost. Return another eligible monster if possible; if it
            // is the only candidate, returning null makes the activation
            // decline and leaves the Magician on the field.
            if (IsMindShuffleProtectionThreat() && !magicianTargeted)
            {
                candidates = candidates.Where(c =>
                    !c.IsCode(CardId.MagicianOfDarkChaosBlackChaos)).ToList();
                if (candidates.Count == 0)
                    return null;
            }

            ClientCard selected = candidates
                .OrderByDescending(IsOpponentTargetedByCurrentChain)
                .ThenByDescending(WasCardActivatedThisTurn)
                .ThenBy(GetMindShuffleReturnPriority)
                .FirstOrDefault();

            // If the Magician is the only normal return target, do not keep
            // cycling it once both hand monsters that Mind Shuffle could
            // summon have already used their effects this turn. A direct
            // opponent target still overrides this resource-saving rule.
            if (selected != null &&
                selected.IsCode(CardId.MagicianOfDarkChaosBlackChaos) &&
                !IsOpponentTargetedByCurrentChain(selected) &&
                !candidates.Any(c => !c.IsCode(CardId.MagicianOfDarkChaosBlackChaos)) &&
                HaveUsedMindShuffleHandTargets())
                return null;

            return selected;
        }

        private bool HaveUsedMindShuffleHandTargets()
        {
            return Bot.Hand.Any(c => c != null && c.IsCode(
                    CardId.BlackLusterSoldierSoldierOfLightAndDarkness)) &&
                Bot.Hand.Any(c => c != null && c.IsCode(
                    CardId.BlackSkullDragonTheArchfiendDragonOfUnity)) &&
                _activatedFirstEffectCardIdsThisTurn.Contains(
                    CardId.BlackLusterSoldierSoldierOfLightAndDarkness) &&
                _activatedFirstEffectCardIdsThisTurn.Contains(
                    CardId.BlackSkullDragonTheArchfiendDragonOfUnity);
        }

        private bool HasFreshMindShuffleSummonCandidate(ClientCard returnTarget)
        {
            return Bot.Hand.Any(c => c != null && c.IsMonster() &&
                c.IsCode(CardId.MagicianOfDarkChaosBlackChaos,
                    CardId.BlackSkullDragonTheArchfiendDragonOfUnity,
                    CardId.BlackLusterSoldierSoldierOfLightAndDarkness,
                    CardId.BlackChaos) &&
                !WasCardActivatedThisTurn(c) &&
                (returnTarget == null || !c.IsCode(returnTarget.Id)));
        }

        private bool IsOpponentTargetedByCurrentChain(ClientCard card)
        {
            if (card == null || card.Controller != 0 ||
                Duel.CurrentChainInfo == null)
                return false;

            // Prefer the per-chain target snapshot. This covers every
            // opponent effect that targeted our monster, regardless of the
            // monster's card name.
            if (Duel.CurrentChainInfo.Any(chain => chain != null &&
                chain.ActivatePlayer == 1 && chain.Targets != null &&
                chain.Targets.Contains(card)))
                return true;

            // Some protocol sequences update the shared target lists before
            // the ChainInfo target snapshot. Keep both lists as fallbacks,
            // but only while an opponent link is part of the current chain.
            return Duel.CurrentChainInfo.Any(chain => chain != null &&
                chain.ActivatePlayer == 1) &&
                (Duel.ChainTargets.Contains(card) ||
                    Duel.LastChainTargets.Contains(card));
        }

        private bool IsMindShuffleReturnCandidate(ClientCard card)
        {
            if (card == null || card.Controller != 0 || !card.IsFaceup())
                return false;

            // These are the only monsters that Mind Shuffle is allowed to
            // return in this deck. In particular, Gallant Thief and other
            // Extra Deck monsters must never be chosen. Fydraulis is the
            // intentional Extra Deck exception in the requested priority.
            return card.IsCode(CardId.FydraulisHarmonia,
                CardId.BlackSkullDragonTheArchfiendDragonOfUnity,
                CardId.BlackLusterSoldierSoldierOfLightAndDarkness,
                CardId.MagicianOfDarkChaosBlackChaos,
                CardId.BlackChaos);
        }

        private int GetMindShuffleReturnPriority(ClientCard card)
        {
            if (card == null)
                return 99;
            int priority = _mindShuffleReturnOrder.IndexOf(card.Id);
            return priority >= 0 ? priority : 99;
        }

        private void EnsureMindShufflePriorityOverride()
        {
            if (_mindShufflePriorityOverrideApplied || Duel.Player != 0 ||
                Duel.Turn == 1 || !Bot.Graveyard.Any(c => c.IsCode(CardId.LightAndDarknessRitual)))
                return;

            _mindShuffleSummonOrder.Remove(CardId.BlackChaos);
            _mindShuffleSummonOrder.Insert(0, CardId.BlackChaos);
            _mindShufflePriorityOverrideApplied = true;
        }

        private void ResetMindShuffleSummonOrder()
        {
            _mindShuffleSummonOrder.Clear();
            _mindShuffleSummonOrder.Add(CardId.MagicianOfDarkChaosBlackChaos);
            _mindShuffleSummonOrder.Add(CardId.BlackSkullDragonTheArchfiendDragonOfUnity);
            _mindShuffleSummonOrder.Add(CardId.BlackLusterSoldierSoldierOfLightAndDarkness);
            _mindShuffleSummonOrder.Add(CardId.BlackChaos);
        }

        private void MoveMindShuffleSummonToEnd(int cardId)
        {
            if (!_mindShuffleSummonOrder.Contains(cardId))
                return;
            _mindShuffleSummonOrder.Remove(cardId);
            _mindShuffleSummonOrder.Add(cardId);
        }

        private void ResetMindShuffleReturnOrder()
        {
            _mindShuffleReturnOrder.Clear();
            _mindShuffleReturnOrder.Add(CardId.FydraulisHarmonia);
            _mindShuffleReturnOrder.Add(CardId.BlackSkullDragonTheArchfiendDragonOfUnity);
            _mindShuffleReturnOrder.Add(CardId.BlackLusterSoldierSoldierOfLightAndDarkness);
            _mindShuffleReturnOrder.Add(CardId.MagicianOfDarkChaosBlackChaos);
            _mindShuffleReturnOrder.Add(CardId.BlackChaos);
        }

        private void MoveMindShuffleReturnToEnd(int cardId)
        {
            if (!_mindShuffleReturnOrder.Contains(cardId))
                return;
            _mindShuffleReturnOrder.Remove(cardId);
            _mindShuffleReturnOrder.Add(cardId);
        }

        private bool HasDuplicateCardInHand()
        {
            return Bot.Hand.Where(c => c != null && c.Id != 0)
                .GroupBy(c => c.Id).Any(group => group.Count() > 1);
        }

        private bool CanActivatePurulia()
        {
            return Bot.HasInHand(CardId.MulcharmyPurulia) &&
                Bot.GetFieldCount() == 0 &&
                !_activatedFirstEffectCardIdsThisTurn.Contains(
                    CardId.MulcharmyPurulia);
        }

        private bool CanSearchBlackChaosSupportCard()
        {
            return !HasMindShuffleOnField() &&
                (Bot.HasInDeck(CardId.MindShuffle) ||
                    Bot.HasInGraveyard(CardId.MindShuffle)) &&
                !_pendingDeckSearchIds.Contains(CardId.MindShuffle);
        }

        private bool IsSearchTargetUnavailable(int cardId)
        {
            return (cardId == CardId.MindShuffle && HasMindShuffleOnField()) ||
                HasCardInHandOrField(cardId) ||
                _pendingDeckSearchIds.Contains(cardId);
        }

        private bool HasMindShuffleOnField()
        {
            // Scan the actual SpellZone so a face-down copy is also treated
            // as already established, and supplement it with the move-tracked
            // references for the short period in which the local zone array
            // may still contain the pre-resolution view.
            return Bot.HasInSpellZone(CardId.MindShuffle) ||
                _mindShuffleFieldCount > 0;
        }

        private bool IsDeckSearchSelectionHint(int hint)
        {
            return hint == HintMsg.AddToHand || hint == HintMsg.Set ||
                hint == HintMsg.ToField || hint == HintMsg.SpSummon;
        }

        private bool IsPendingDeckSearch(ClientCard card)
        {
            return card != null && card.Id != 0 &&
                (card.Location & CardLocation.Deck) != 0 &&
                _pendingDeckSearchIds.Contains(card.Id);
        }

        private void ReservePendingDeckSearch(IList<ClientCard> selected)
        {
            if (selected == null)
                return;

            foreach (ClientCard card in selected)
            {
                if (card != null && card.Id != 0 &&
                    (card.Location & CardLocation.Deck) != 0)
                    _pendingDeckSearchIds.Add(card.Id);
            }
        }

        private bool ShouldPrioritizeLightAndDarknessRitual()
        {
            return Duel.Player == 0 &&
                Bot.Hand.Any(IsRitualMonster) &&
                !_performedRitualSummonThisTurn &&
                !Bot.HasInHandOrInSpellZone(CardId.LightAndDarknessRitual);
        }

        private bool HasHigherPriorityLightAndDarknessRitualCandidate()
        {
            if (Duel.CurrentChain.Count != 0 || Duel.MainPhase == null)
                return false;

            // A hand/Spell Zone Ritual candidate must resolve before Black
            // Chaos returns a Ritual Monster to the Deck for its own summon.
            // Graveyard recovery is handled by its separate earlier executor.
            return Duel.MainPhase.ActivableCards.Any(c => c != null &&
                c.IsCode(CardId.LightAndDarknessRitual) &&
                (((int)c.Location & (int)(CardLocation.Hand |
                    CardLocation.SpellZone)) != 0));
        }

        private bool HasFreeSpellTrapZone()
        {
            // SpellZone[0..4] are the five normal Spell/Trap zones.
            // Field and Pendulum zones cannot be used as ordinary set zones.
            return Bot.GetSpellCountWithoutField() < 5;
        }

        private bool CanSetGriffohSupportCard()
        {
            if (!HasFreeSpellTrapZone())
                return false;

            if (HasPendingBlackChaosSupportSearch())
            {
                return Bot.HasInDeck(CardId.SpellShatteringSword) &&
                    !IsSearchTargetUnavailable(CardId.SpellShatteringSword);
            }

            // Griffoh sets directly from the Deck. A copy already in hand
            // cannot be activated this turn, so it must not block setting a
            // second copy from the Deck.
            return (Bot.HasInDeck(CardId.MindShuffle) &&
                        !HasMindShuffleOnField() &&
                        !_pendingDeckSearchIds.Contains(CardId.MindShuffle)) ||
                (Bot.HasInDeck(CardId.SpellShatteringSword) &&
                    !IsSearchTargetUnavailable(CardId.SpellShatteringSword));
        }

        private bool CanReceiveDamage()
        {
            if (Bot.UnderAttack || Enemy.BattlingMonster != null)
                return true;

            if (Duel.Player == 1 && Duel.Phase >= DuelPhase.BattleStart &&
                Duel.Phase <= DuelPhase.Battle &&
                Enemy.GetMonsters().Any(c => c != null && c.IsAttack() &&
                    !c.IsDisabled()))
                return true;

            ChainInfo latest = GetLatestChainInfo();
            if (latest != null && latest.ActivatePlayer == 1 &&
                HasDamageText(NamedCard.Get(latest.ActivateId)))
                return true;

            ClientCard latestCard = GetCurrentOpponentChainCard();
            return latestCard != null && HasDamageText(latestCard.Data);
        }

        private bool HasDamageText(NamedCard card)
        {
            if (card == null || String.IsNullOrEmpty(card.Description))
                return false;

            return card.Description.IndexOf("傷害", StringComparison.Ordinal) >= 0 ||
                card.Description.IndexOf("伤害", StringComparison.Ordinal) >= 0 ||
                card.Description.IndexOf("damage", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private bool HasCardInHandOrField(int cardId)
        {
            return Bot.Hand.Any(c => c != null && c.IsCode(cardId)) ||
                Bot.GetSpells().Any(c => c != null && c.IsCode(cardId)) ||
                Bot.GetMonsters().Any(c => c != null && c.IsCode(cardId));
        }

        private bool CanUseBlackSkullDragonHandSpecialSummon()
        {
            return Bot.Hand.Any(c => c != null &&
                    c.IsCode(CardId.LightAndDarknessRitual)) ||
                Bot.Hand.Any(IsDuplicateSpellInHand) ||
                (Bot.Hand.Any(c => c != null && c.IsCode(CardId.SpatialTrunade)) &&
                    !CanUseSpatialTrunadeOnEnemyField()) ||
                Bot.Hand.Any(c => IsBlackSkullDiscardSpell(c) &&
                    WasCardActivatedThisTurn(c));
        }

        private bool CanUseSpatialTrunadeOnEnemyField()
        {
            List<ClientCard> enemyCards = Enemy.GetMonsters().Concat(Enemy.GetSpells())
                .Where(c => c != null && !IsOpponentTargetReserved(c)).ToList();
            if (enemyCards.Count == 0)
                return false;

            ClientCard problematic = Util.GetProblematicEnemyCard(0, false);
            return enemyCards.Count >= 2 ||
                problematic != null && enemyCards.Contains(problematic) ||
                enemyCards.Any(c => c.IsExtraCard() || c.IsFacedown() ||
                    c.IsFaceup() && c.IsFloodgate());
        }

        private bool IsDuplicateSpellInHand(ClientCard card)
        {
            return IsBlackSkullDiscardSpell(card) &&
                Bot.Hand.Count(c => c != null && c.IsCode(card.Id)) > 1;
        }

        private bool IsBlackSkullDiscardSpell(ClientCard card)
        {
            return card != null && card.Id != 0 && card.IsSpell() &&
                (card.HasType(CardType.QuickPlay) ||
                    card.IsCode(CardId.LightAndDarknessRitual));
        }

        private bool ShouldUseMindShuffleFallback()
        {
            return !Bot.HasInDeck(CardId.LightAndDarknessRitual) ||
                !Bot.HasInDeck(CardId.SkullArchfiendOfChaos);
        }

        private void TrackReleaseSelection(IList<ClientCard> selected)
        {
            if (selected == null)
                return;
            foreach (ClientCard card in selected)
            {
                // A material selected from the Graveyard is banished by the
                // ritual spell; it is not a monster released to the Graveyard.
                if (card != null && card.Controller == 0 && card.IsMonster() &&
                    !IsGraveMonster(card))
                    _pendingReleaseCards.Add(card);
            }
        }

        private bool HasFallenExtraDeckCost()
        {
            return Bot.HasInExtra(CardId.AlbionTheBrandedDragon) ||
                Bot.HasInExtra(CardId.EcclesiaAndTheDarkDragon) ||
                Bot.HasInExtra(CardId.TitanikladTheAshDragon) ||
                Bot.HasInExtra(CardId.AlbaLenatusTheAbyssDragon);
        }

        private List<ClientCard> GetOrderedFallenTargets(IEnumerable<ClientCard> source)
        {
            return source.Where(IsWorthwhileFallenTarget).Distinct()
                .OrderByDescending(GetFallenTargetPriority)
                .ThenByDescending(c => Math.Max(c.Attack, c.Defense))
                .ToList();
        }

        private int GetFallenTargetPriority(ClientCard card)
        {
            if (card == null || card.Controller != 1 || !card.IsFaceup())
                return -1;

            if (!card.IsMonster())
            {
                if (card.IsFloodgate())
                    return 1000;
                if (card.HasType(CardType.Field))
                    return 850;
                if (card.HasType(CardType.Continuous))
                    return 800;
                return 0;
            }

            int priority = GetDestructionMonsterPriority(card);
            // The Fallen targets when it is activated. A monster that can
            // publicly remove itself in response is less reliable, but remains
            // a fallback because the opponent's hidden resources are unknown.
            if (CanLikelyEscapeFallenTarget(card))
                priority -= 450;
            return priority;
        }

        private List<ClientCard> GetOrderedFydraulisTargets(IEnumerable<ClientCard> source)
        {
            return source.Where(IsWorthwhileDestructionMonster).Distinct()
                .OrderByDescending(GetFydraulisTargetPriority)
                .ThenByDescending(c => Math.Max(c.Attack, c.Defense))
                .ToList();
        }

        private int GetFydraulisTargetPriority(ClientCard card)
        {
            if (card == null || card.Controller != 1 || !card.IsMonster())
                return -1;

            // Fydraulis chooses the monster while its non-targeting effect is
            // resolving, so target-dodging effects do not receive The Fallen's
            // priority penalty here.
            return GetDestructionMonsterPriority(card);
        }

        private int GetDestructionMonsterPriority(ClientCard card)
        {
            if (card == null || !card.IsMonster())
                return -1;

            int priority;
            if (card.IsFloodgate())
                priority = 1000;
            else if (card.IsMonsterDangerous())
                priority = 950;
            else if (card.IsMonsterShouldBeDisabledBeforeItUseEffect())
                priority = 900;
            else if (IsValuableFallenExtraDeckMonster(card))
                priority = 750;
            else if (MeetsFallenMonsterStatThreshold(card))
                priority = 500;
            else
                priority = 0;

            // A disabled monster can still be material or a battle threat, so
            // demote it instead of excluding it outright.
            if (card.IsDisabled())
                priority -= 350;
            return priority;
        }

        private bool IsWorthwhileFallenTarget(ClientCard card)
        {
            if (card == null || card.Controller != 1 || !card.IsFaceup())
            {
                return false;
            }

            if (card.IsMonster())
                return IsWorthwhileDestructionMonster(card);

            return card.HasType(CardType.Field | CardType.Continuous);
        }

        private bool IsWorthwhileDestructionMonster(ClientCard card)
        {
            return card != null && card.Controller == 1 && card.IsMonster() &&
                card.IsFaceup() &&
                (IsValuableFallenExtraDeckMonster(card) || card.IsFloodgate() ||
                    card.IsMonsterDangerous() ||
                    card.IsMonsterShouldBeDisabledBeforeItUseEffect() ||
                    MeetsFallenMonsterStatThreshold(card));
        }

        private bool MeetsFallenMonsterStatThreshold(ClientCard card)
        {
            return card != null && card.Attack + card.Defense >= 2700 &&
                (card.Attack >= 1800 || card.Defense >= 2100);
        }

        private bool CanLikelyEscapeFallenTarget(ClientCard card)
        {
            return card != null && card.IsCode(CardId.FiendsmithsRequiem) &&
                !card.IsDisabled();
        }

        private bool IsValuableFallenExtraDeckMonster(ClientCard card)
        {
            return card != null && card.IsMonster() && card.IsExtraCard() &&
                (!card.HasType(CardType.Link) || card.LinkCount >= 2);
        }

        private bool IsRitualMonster(ClientCard card)
        {
            return card != null && card.IsCode(
                CardId.MagicianOfDarkChaosBlackChaos,
                CardId.BlackLusterSoldierSoldierOfLightAndDarkness);
        }

        private bool IsRitualRelatedCard(ClientCard card)
        {
            // Celtic Mystic's draw-three effect requires the hand to contain
            // a card whose text mentions Light and Darkness Ritual. This is
            // the complete set of such cards in this deck, not merely the
            // cards that can search or set the Ritual Spell.
            return card != null && card.IsCode(
                CardId.SkullArchfiendOfChaos,
                CardId.MindShuffle,
                CardId.LightAndDarknessRitual,
                CardId.MagicianOfDarkChaosBlackChaos,
                CardId.CelticMystic,
                CardId.BlackLusterSoldierSoldierOfLightAndDarkness,
                CardId.SpellShatteringSword,
                CardId.Griffoh,
                CardId.BlackSkullDragonTheArchfiendDragonOfUnity,
                CardId.BlackChaos);
        }

        private int GetRitualMonsterPriority(ClientCard card)
        {
            if (card.IsCode(CardId.MagicianOfDarkChaosBlackChaos))
                return 0;
            if (card.IsCode(CardId.BlackLusterSoldierSoldierOfLightAndDarkness))
                return 1;
            return 2;
        }

        private int GetRitualMaterialPriority(ClientCard card)
        {
            if (IsGraveCard(card))
            {
                if (HasKnownCardType(card, CardType.Fusion))
                    return 30;
                if (card.IsCode(CardId.Griffoh))
                    return 0;
                if (card.IsCode(CardId.BlackChaos))
                    return 1;
                return 2;
            }

            if (card.IsCode(CardId.Griffoh))
                return 10;
            if (card.IsCode(CardId.BlackChaos))
                return 11;
            if (card.IsCode(CardId.CelticMystic))
                return 20;
            if (card.IsCode(CardId.FallenOfTheWhiteDragon))
                return 21;
            if (card.IsCode(CardId.AshBlossomJoyousSpring,
                CardId.MaxxC, CardId.DrollLockBird, CardId.MulcharmyPurulia))
                return 23;
            if (card.IsCode(CardId.BlackLusterSoldierSoldierOfLightAndDarkness))
                return 30;
            return 25;
        }

        private int GetRitualTributePriority(ClientCard card)
        {
            // Light and Darkness Ritual can banish monsters from the Graveyard
            // instead of releasing cards from the hand/field. Preserve those
            // resources by placing non-Fusion Graveyard monsters first.
            if (IsGraveCard(card))
            {
                if (HasKnownCardType(card, CardType.Fusion))
                    return 30;
                if (card.IsCode(CardId.Griffoh))
                    return 0;
                if (card.IsCode(CardId.BlackChaos))
                    return 1;
                return 2;
            }

            // If the special one-card Kuriboh route was not legal, its normal
            // material value is still better than spending most hand cards.
            if (card.IsCode(CardId.Griffoh))
                return 10;
            if (card.IsCode(CardId.BlackChaos))
                return 11;
            if (card.IsCode(CardId.CelticMystic))
                return 20;
            if (card.IsCode(CardId.FallenOfTheWhiteDragon))
                return 21;
            if (card.IsCode(CardId.AshBlossomJoyousSpring,
                CardId.MaxxC, CardId.DrollLockBird, CardId.MulcharmyPurulia))
                return 23;
            return 25;
        }

        private bool IsGraveMonster(ClientCard card)
        {
            return card != null && HasKnownCardType(card, CardType.Monster) &&
                (((int)card.Location & (int)CardLocation.Grave) != 0);
        }

        private bool HasKnownCardType(ClientCard card, CardType type)
        {
            return card != null && (card.HasType(type) ||
                card.Data != null && card.Data.HasType(type));
        }

        private bool IsGraveCard(ClientCard card)
        {
            return card != null &&
                (((int)card.Location & (int)CardLocation.Grave) != 0);
        }

        private bool IsAllowedRitualMaterial(ClientCard card)
        {
            return card != null && !card.IsCode(CardId.SkullArchfiendOfChaos);
        }

        private bool IsPreferredGraveRitualMaterial(ClientCard card)
        {
            return IsAllowedRitualMaterial(card) && IsGraveCard(card) &&
                !HasKnownCardType(card, CardType.Fusion);
        }

        private bool CanUseOnlyGraveyardMaterials(IList<ClientCard> mandatoryCards)
        {
            return mandatoryCards == null || mandatoryCards.All(IsGraveCard);
        }

        private IList<ClientCard> SelectGraveyardRitualSumSelection(
            IList<ClientCard> cards, IList<ClientCard> mandatoryCards, int sum,
            int min, int max, bool exactEqual)
        {
            if (!CanUseOnlyGraveyardMaterials(mandatoryCards))
                return null;

            List<ClientCard> graveCards = cards
                .Where(IsPreferredGraveRitualMaterial)
                .OrderBy(GetRitualMaterialPriority).ToList();
            if (graveCards.Count == 0)
                return null;

            return AI.FindSumSelection(graveCards, mandatoryCards, sum, min, max,
                exactEqual);
        }

        private IList<ClientCard> SelectGraveyardRitualTribute(
            IList<ClientCard> cards, int min, int max)
        {
            List<ClientCard> graveCards = cards
                .Where(IsPreferredGraveRitualMaterial)
                .OrderBy(GetRitualTributePriority).ToList();
            if (graveCards.Count == 0)
                return null;

            return AI.FindTributeSelection(graveCards, min, max);
        }

        private IList<ClientCard> SelectGriffohAsFullRitualMaterial(
            IList<ClientCard> cards, int min, int max)
        {
            ClientCard griffoh = cards.FirstOrDefault(c => c != null &&
                c.IsCode(CardId.Griffoh) && IsGraveCard(c));
            if (griffoh == null)
                return null;

            // SelectTribute encodes the ordinary one-card value in OpParam1
            // and the replacement value in OpParam2. Kuriboh Guardian can
            // represent both values, so keep both branches available to the
            // local validator. The response only contains the candidate
            // index; the server still applies the card script's real value.
            griffoh.OpParam1 = 1;
            griffoh.OpParam2 = 8;
            return new List<ClientCard> { griffoh };
        }

        private IList<ClientCard> SelectGriffohAsFullRitualMaterial(
            IList<ClientCard> cards, IList<ClientCard> mandatoryCards, int sum,
            int min, int max)
        {
            if (min > 1 || max < 1)
                return null;

            // Depending on the script/core combination, the same replacement
            // may arrive as a sum of 8 or as the ordinary one-card value 1.
            // Both are valid representations of Kuriboh Guardian here.
            if (sum != 0 && sum != 1 && sum != 8)
                return null;

            ClientCard mandatoryGriffoh = mandatoryCards == null ? null :
                mandatoryCards.FirstOrDefault(c => c != null &&
                    c.IsCode(CardId.Griffoh) && IsGraveCard(c));
            if (mandatoryGriffoh != null)
            {
                mandatoryGriffoh.OpParam1 = 1;
                mandatoryGriffoh.OpParam2 = 8;
                return new List<ClientCard>();
            }

            if (mandatoryCards != null && mandatoryCards.Count > 0)
                return null;

            ClientCard griffoh = cards.FirstOrDefault(c => c != null &&
                c.IsCode(CardId.Griffoh) && IsGraveCard(c));
            if (griffoh == null)
                return null;

            griffoh.OpParam1 = 1;
            griffoh.OpParam2 = 8;
            return new List<ClientCard> { griffoh };
        }

        private bool IsExtraDeckMonster(int cardId)
        {
            NamedCard card = NamedCard.Get(cardId);
            return card != null && card.IsExtraCard();
        }

        private int GetDiscardPriority(ClientCard card)
        {
            if (card.IsCode(CardId.SpatialTrunade))
                return 0;
            if (card.IsCode(CardId.TheFallenTheVirtuous,
                CardId.SpellShatteringSword, CardId.CrimsonCall))
                return 1;
            if (card.IsCode(CardId.RaggedRecordsOfRites,
                CardId.MindShuffle))
                return 2;
            if (card.IsCode(CardId.LightAndDarknessRitual) &&
                Bot.Hand.Count(c => c.IsCode(CardId.LightAndDarknessRitual)) > 1)
                return 3;
            if (card.IsCode(CardId.FallenOfTheWhiteDragon,
                CardId.CelticMystic, CardId.Griffoh))
                return 5;
            if (card.IsCode(CardId.BlackChaos,
                CardId.BlackLusterSoldierSoldierOfLightAndDarkness))
                return 20;
            return 10;
        }

        private bool WasCardActivatedThisTurn(ClientCard card)
        {
            return card != null && card.Id != 0 &&
                _activatedFirstEffectCardIdsThisTurn.Contains(card.Id);
        }

        private bool IsMultiEffectCard(ClientCard card)
        {
            return card != null && card.IsCode(
                CardId.TheWorldsGreatestGallantThief,
                CardId.BlackSkullDragonTheArchfiendDragonOfUnity,
                CardId.BlackChaos,
                CardId.CelticMystic,
                CardId.RaggedRecordsOfRites,
                CardId.MindShuffle,
                CardId.CrimsonCall,
                CardId.LightAndDarknessRitual,
                CardId.FallenOfTheWhiteDragon,
                CardId.IncredibleEcclesiaTheVirtuous,
                CardId.SkullArchfiendOfChaos,
                CardId.MagicianOfDarkChaosBlackChaos,
                CardId.TheFallenTheVirtuous,
                CardId.SpellShatteringSword,
                CardId.BlackLusterSoldierSoldierOfLightAndDarkness,
                CardId.FydraulisHarmonia);
        }

        private bool IsFirstEffect(ChainInfo chain)
        {
            if (chain == null)
                return false;
            if (!chain.IsActivateCode(
                CardId.TheWorldsGreatestGallantThief,
                CardId.BlackSkullDragonTheArchfiendDragonOfUnity,
                CardId.BlackChaos,
                CardId.CelticMystic,
                CardId.RaggedRecordsOfRites,
                CardId.MindShuffle,
                CardId.CrimsonCall,
                CardId.LightAndDarknessRitual,
                CardId.FallenOfTheWhiteDragon,
                CardId.IncredibleEcclesiaTheVirtuous,
                CardId.SkullArchfiendOfChaos,
                CardId.MagicianOfDarkChaosBlackChaos,
                CardId.TheFallenTheVirtuous,
                CardId.SpellShatteringSword,
                CardId.BlackLusterSoldierSoldierOfLightAndDarkness,
                CardId.FydraulisHarmonia))
                return true;

            // Gallant Thief's offset 0 belongs to its Summon procedure. Its
            // first activatable monster effect is offset 1; all other tracked
            // multi-effect cards use offset 0 for their first effect.
            int offset = chain.IsActivateCode(
                CardId.TheWorldsGreatestGallantThief) ? 1 : 0;
            return chain.ActivateDescription ==
                Util.GetStringId(chain.ActivateId, offset);
        }

        private bool IsDuplicateInSelection(ClientCard card, IList<ClientCard> cards)
        {
            return card != null && card.Id != 0 &&
                cards.Count(c => c != null && c.Id == card.Id) > 1;
        }

        private int GetGallantThiefTributePriority(ClientCard card)
        {
            if (card.Controller == 1)
            {
                ClientCard problematic = Util.GetProblematicEnemyMonster(0, true);
                if (card == problematic)
                    return 100;
                return card.IsFacedown() ? 80 : card.Attack;
            }

            if (card.IsCode(CardId.CelticMystic, CardId.FallenOfTheWhiteDragon,
                CardId.Griffoh))
                return 1;
            return 0;
        }

        private bool IsDescription(int cardId, int offset)
        {
            return ActivateDescription == Util.GetStringId(cardId, offset);
        }

        private int GetOptionIndex(IList<int> options, int cardId, int offset)
        {
            return options.IndexOf(Util.GetStringId(cardId, offset));
        }

        private bool IsOpponentMonsterEffectChain(ChainInfo chain)
        {
            return chain != null && chain.ActivatePlayer == 1 &&
                (chain.ActivateType & (int)CardType.Monster) != 0 &&
                chain.HasLocation(CardLocation.MonsterZone);
        }

        private ClientCard GetChainSourceCard(ChainInfo chain)
        {
            if (!IsOpponentMonsterEffectChain(chain))
                return null;

            ClientCard source = Enemy.GetMonsters().FirstOrDefault(c => c != null &&
                c.Location == chain.ActivateLocation &&
                c.Sequence == chain.ActivateSequence && c.IsFaceup());
            if (source != null)
                return source;

            return Enemy.GetMonsters().FirstOrDefault(c => c != null &&
                c == chain.RelatedCard && c.IsFaceup());
        }

        private ChainInfo GetLatestOpponentChainInfo()
        {
            if (Duel.CurrentChainInfo == null)
                return null;

            for (int i = Duel.CurrentChainInfo.Count - 1; i >= 0; --i)
            {
                ChainInfo chain = Duel.CurrentChainInfo[i];
                if (chain != null && chain.ActivatePlayer == 1)
                    return chain;
            }

            return null;
        }

        private ClientCard FindMatchingCard(IList<ClientCard> cards,
            ClientCard source)
        {
            if (source == null)
                return null;

            ClientCard exact = cards.FirstOrDefault(c => c == source);
            if (exact != null)
                return exact;

            return cards.FirstOrDefault(c => c != null &&
                c.Controller == source.Controller &&
                c.Location == source.Location &&
                c.Sequence == source.Sequence);
        }

        private IList<ClientCard> SelectSpellShatteringSwordMonsterTarget(
            IList<ClientCard> cards, int min, int max)
        {
            ClientCard target = FindMatchingCard(cards,
                _pendingSpellShatteringSwordMonsterTarget);
            if (target == null || target.Controller != 1 || !target.IsOnField() ||
                !target.IsMonster() || !target.IsFaceup())
                return null;

            _pendingSpellShatteringSwordMonsterTarget = null;
            return SelectCount(new List<ClientCard> { target }, cards, min, max, 1);
        }

        private ChainInfo GetLatestChainInfo()
        {
            if (Duel.CurrentChainInfo == null || Duel.CurrentChainInfo.Count == 0)
                return null;
            return Duel.CurrentChainInfo[Duel.CurrentChainInfo.Count - 1];
        }

        private bool HasFreshOpponentTarget(IEnumerable<ClientCard> candidates)
        {
            return candidates != null && candidates.Any(c => c != null &&
                c.Controller == 1 && !IsOpponentTargetReserved(c));
        }

        private ChainInfo GetCurrentActivationChainInfo()
        {
            ClientCard current = Duel.GetCurrentChainCard();
            if (current == null)
                return null;

            for (int i = Duel.CurrentChainInfo.Count - 1; i >= 0; --i)
            {
                ChainInfo chain = Duel.CurrentChainInfo[i];
                if (chain == null || chain.ActivatePlayer != 0)
                    continue;

                if (chain.RelatedCard == current ||
                    (chain.ActivateId == current.Id &&
                        chain.ActivateController == current.Controller &&
                        chain.ActivateSequence == current.Sequence))
                    return chain;
            }

            return null;
        }

        private ClientCard GetCurrentOpponentChainCard()
        {
            ClientCard current = Duel.GetCurrentChainCard();
            if (current != null && current.Controller == 1)
                return current;

            ClientCard last = Util.GetLastChainCard();
            return last != null && last.Controller == 1 ? last : null;
        }
    }
}
