# Changelog

## Current (latest)

 - Refresh the bot level assignments
 - Rebalance TimeThief for Level 2
 - Fix `ActivateDescription` propagation
 - Reduce unnecessary combos under Maxx "C" and Mulcharmy based on the summon location
 - Refactor SkyStriker
 - Update Level8 and Orcust
 - Add `DefaultGetDisableMonsterTarget`
 - Update PureWinds
 - Fix and update Lightsworn
 - Add life-point safety checks for Predaplant Verte Anaconda effects
 - Add the `UsePreErrataEffects` configuration option
 - Add life-point checks to ThunderDragon effects
 - Update ThunderDragon's Union Carrier handling
 - Update Rank5 and Rank8
 - Update Chaos408 and Monarch506
 - Add Any CPU as the default build platform
 - Include the Mono.Data.Sqlite sources instead of the prebuilt managed DLL
 - Update the native SQLite library to 3.53.4
 - New deck: RadiantTyphoon
 - Fix Trickstar negation checks
 - Add `DefaultCheckWhetherNumber41IsActive`
 - Fix place-selection callbacks
 - Add `Executor.OnSelectBattleDirectAttack`
 - Fix direct-attack, attack-target, and battle-replay selection
 - Fix Salamangreat attack-target handling
 - Fix executor logic that incorrectly relied on the global `Card` context
 - Fix `OnSelectPendulumSummon`
 - Add `Executor.OnSpSummoning`
 - Improve SacredBeast card selection
 - Add `Bot.GetCardCountInDeck` and `Bot.HasInDeck` backed by tracked deck counts
 - Update equip-card and target-card relationship tracking
 - Fix alternative-artwork cards support
 - Improve error logging
 - Add the `DefenseAttackMonster` known-card enum, fix related battle phase actions
 - Improve place selection and field-zone prompts
 - Fix overlay-material movement synchronization
 - Fix deck-count and field-zone synchronization after field updates
 - Improve SacredBeast card selection against Dogmatika Maximus
 - De-duplicate and centralize list-shuffling logic
 - Fix player-name display in Tag Duel
 - Add server-mode deck existence validation
 - Add Mono build validation
 - Fix a SuperheavySamurai card-selection regression
 - Improve selection-count diagnostics
 - Prefer `ChainInfo` activation snapshots while a chain is resolving
 - New deck: Enneacraft
 - New deck: Pumpking
 - Recover when a deck AI returns an invalid number of selected cards
 - Improve Yubel behavior
 - Fix Voiceless compilation
 - Fix Yubel targeting of the bot's monsters
 - Improve select/unselect handling
 - Refactor TimeThief and fix Time Thief Redoer behavior
 - Refactor select-sum handling
 - Validate card-selection results before sending a response
 - Clear stale preselected choices between selection requests
 - Allow Main Phase 2 after battle when appropriate
 - Fix draw and hint handling
 - Fix startup errors and improve runtime diagnostics
 - Fix FamiliarPossessed place selection
 - Fix Dragunity's Dragon Ravine handling
 - Update Altergeist, HeroBeat1103, and ThunderDragon
 - Rewrite Lightsworn
 - New deck: Monarch506
 - Update Chaos408
 - Add `Executor.OnSummoning`
 - Add chain targets to `ChainInfo`
 - Fix mandatory-effect card selection
 - Fix position selection
 - Fix Salamangreat's Borrelsword handling
 - Fix MekkKnight behavior
 - Update the default monster-positioning, Solemn Judgment, and Solemn Warning executors
 - New deck: Archfiend
 - New deck: Chaos408
 - New deck: Rank8
 - New deck: HeroBeat1103
 - Add `AGENTS.md` contributor guidance for creating and modifying deck AIs
 - Fix the S:P Little Knight card ID in Apophis
 - Update the known card enums
 - New deck: SacredBeast
 - Include deck executors and related resources through project-file wildcards
 - Add BotWrapper load-once configuration for YGOPro
 - New deck: Neko
 - New deck: BE2025
 - Fix server-mode query-string parsing
 - Improve exception handling and debugging behavior
 - Fix preferred-card selection
 - Fix Yubel card selection
 - Fix Maliss in the Mirror handling
 - Fix SuperheavySamurai's Soulpiercer equip handling
 - New deck: Apophis
 - New deck: Maliss
 - New deck: MalissOCG
 - New deck: Yubel

## v0x1362 (2025-06-21)

 - Update YGOPro protocol to 0x1362
 - New decks: Albaz, Ryzeal
 - Upgrade the main project to .NET Framework 4.8 and the Windows CI image to Visual Studio 2022
 - Add `ChainInfo` activation snapshots and `Duel.CurrentChainInfo`
 - Add `Executor.OnSpSummoned` and improve special-summon state tracking
 - Update chain, confirmation, sum, and select/unselect handling for the newer protocol
 - Add automatic-length Unicode packet writing and `CtosMessage.ExternalAddress`
 - Fix JoinGame packet length, overlay material handling, card ordering, and several deck AIs

## v0x1361 (2024-06-03)

 - Update YGOPro protocol to 0x1361
 - New decks: Dogmatika, Labrynth, SuperheavySamurai, Swordsoul, Zefra
 - Include the YGOSharp.Network and YGOSharp.OCGWrapper sources instead of prebuilt DLLs
 - Add `ExecutorType.Surrender` and teammate-surrender handling
 - Update SQLite to 3.45.2
 - Wait for the server connection to close at duel end and allow empty dialog sets
 - Improve default executors, card enums, selection logic, attack targeting, and the AI of several decks

## v0x1360 (2023-05-27)

 - Update YGOPro protocol to 0x1360
 - New decks: Exosister, Kashtira, Tearlaments, ThunderDragon
 - Update the known card enums and the default Vaylantz Worlds executor
 - Improve card selection de-duplication and fix several existing deck AIs

## v0x1354 (2022-09-29)

 - Update YGOPro protocol to 0x1354
 - New deck: Brave
 - Add YGOPro 2 database-path support
 - Add GitHub Actions CI automated build validation
 - Add basic field-zone support to `OnSelectPlace`
 - Implement `GameMessage.SwapGraveDeck`
 - Improve material selection, chain selection after a card becomes Xyz material, and target handling

## v0x1353 (2021-09-06)

 - Update YGOPro protocol to 0x1353
 - New deck: FamiliarPossessed; add the experimental Lucky executor
 - Add the `DeckFile` option and hostname resolution with IPv4 selection
 - Fully implement announce-card handling and make `OnAnnounceCard` overrideable
 - Add `HintMsg` handling and `Executor.OnPreActivate`
 - Improve action-loop prevention, summon/set and position decisions, direct attacks, and card-controller tracking
 - Rewrite and update the Time Thief AI and update default executors

## v0x1352 (2020-09-30)

 - Update YGOPro protocol to 0x1352
 - Fix repeated activation in Witchcraft and other deck AIs
 - Fix Unending Nightmare activation and update Witchcraft behavior

## v0x1351 (2020-05-01)

 - Update YGOPro protocol to 0x1351; `STOC_DECK_COUNT` is not implemented
 - New decks: PureWinds, TimeThief, Witchcraft
 - Update default executors and the known card enums
 - Fix chaining from an unknown card in the hand and several deck-specific behaviors

## v0x1350 (2020-01-23)

 - Update YGOPro protocol to 0x1350 and support the 2020 Master Rule revision
 - New deck: Dragun
 - Add `ClientCard.IsOriginalCode`
 - Prefer Main Monster Zones where appropriate
 - Improve Dark Magician, Gren Maju Thunder Boarder, Salamangreat, and Dragun behavior
 - Update the known card enums and fix repeated activation in several decks

## v0x134B (2019-07-07)

 - Update YGOPro protocol to 0x134B

## v0x134A (2019-05-30)

 - Update YGOPro protrol to 0x134A
 - New decks: Altergeist, BlueEyesMaxDragon, GrenMajuThunderBoarder, Level8, Orcust, Phantasm, Salamangreat
 - Use LINQ in codes
 - Add random bot feature to BotWrapper
 - Add `ClientCard.Sequence`, `ClientCard.ProcCompleted`, `ClientCard.IsSpecialSummoned`
 - Add `ClientCard.EquipCards`, `ClientCard.OwnTargets`, handle equip cards
 - Add `ClientCard.IsCode`, handle card alias
 - Add `ClientCard.GetLinkedZones`, `ClientCard.HasSetcode`
 - Add `ClientField.UnderAttack`, `ClientField.GetLinkedZones`, `ClientField.GetFieldSpellCard`
 - Add `Duel.SummoningCards`, `Duel.LastSummonedCards`
 - Add `Util.GetTotalAttackingMonsterAttack`, `Util.GetBotAvailZonesFromExtraDeck`
 - Add `GetMatchingCards`, `GetFirstMatchingCard`, `IsExistingMatchingCard`
 - Add `ExecutorType.GoToBattlePhase`, `ExecutorType.GoToMainPhase2`, `ExecutorType.GoToEndPhase`
 - Add `DefaultScapegoat`, `DefaultMaxxC`, `DefaultAshBlossomAndJoyousSpring`, `DefaultGhostOgreAndSnowRabbit`, `DefaultGhostBelleAndHauntedMansion`, `DefaultEffectVeiler`, `DefaultCalledByTheGrave`, `DefaultInfiniteImpermanence`
 - Rename `AIFunctions` to `AIUtil` (Usage: `AI.Utils.` -> `Util.`)
 - Rename `AIFunctions.CompareCardAttack` to `CardContainer.CompareCardAttack`
 - Update `Util.SelectPreferredCards` and `Util.CheckSelectCount` to return the result
 - Update `ClientField.HasInMonstersZone` to support check face-up card
 - Update `AI.SelectCard` [\#59](https://github.com/IceYGO/windbot/pull/59)
 - Handle swap control of cards
 - Change some `int location` to `CardLocation location`
 - Update default `OnPreBattleBetween` to recognize more cards
 - Misc updates to default executors
 - Misc updates to the AI of some decks
 - Update the known card enums
 - Fix `CardSelector.Select`
 - Fix `OnSelectEffectYn` didn't have `ActivateDescription`
 - Fix `ClientCard.Attacked`
 - Fix infinite activation of ZexalWeapons

## v0x1344 (2018-06-05)

 - Update YGOPro protrol to 0x1344
 - New decks: DarkMagician, SkyStriker
 - Add param to turn chat off
 - Add param to print verbose log
 - Add part of `Zones` enum and `AI.SelectPlace`
 - Add `ClientCard.IsTuner`, `ClientCard.LinkMarker`, `ClientCard.HasLinkMarker`
 - Add `ShouldNotBeTarget` and `ShouldBeDisabledBeforeItUseEffectMonster` enum
 - Add `AI.Utils.GetBestBotMonster`, `AI.Utils.GetWorstBotMonster` and `AI.Utils.ChainContainPlayer`
 - Add `Executor.OnCardSorting` and `Executor.OnDraw`
 - Add `ClientField.GetColumnCount` and `ClientField.HasInHandOrInSpellZone` etc.
 - Misc updates to LightswornShaddoldinosour and ChainBurn deck
 - Misc updates to default executors
 - Fix OnSelectUnselectCard
 - Fix OnMove to keep card data when moving

## v0x1343 (2018-04-11)

 - Update YGOPro protrol to 0x1343
 - New decks: Trickstar, LightswornShaddoldinosour, ChainBurn
 - Update `OnBattle`, add `Executor.OnSelectAttacker` and `Executor.OnSelectAttackTarget`
 - Add `Executor.OnSelectPosition`, `Executor.OnSelectBattleReplay`
 - Add `Bot.BattlingMonster`
 - Add and update some default executors
 - Change `Duel.LifePoints[0]` to `Bot.LifePoints`
 - Change `LastChainPlayer` and `CurrentChain` to `Duel` class
 - Change `ChainContainsCard` and `GetLastChainCard` etc. to `AI.Utils` class
 - Fix turn count in match duel
 - Fix don't turn 0 atk monster to atk pos

## v0x1342 (2017-12-26)

 - Update YGOPro protrol to 0x1342
 - Add Linux BotWrapper (a simple bash script)

## v0x1341 (2017-11-27)

 - Update YGOPro protrol to 0x1341
 - Change the program to x86 only
 - Add BotWrapper for YGOPro bot mode
 - Add `AI.SelectMaterials`, `OnSelectFusionMaterial`, `OnSelectPendulumSummon`, `AI.Utils.SelectPreferredCards` etc.
 - Fix `AI.Utils.GetBestEnemySpell` to not return normal spell currently activating
 - Fix AI don't attack defense Crystal Wing or S39
 - Fix ZexalWeapons AI don't change defense S39 back
 - Minor updates

## v0x1340 (2017-11-06)

 - Update YGOPro protrol to 0x1340
 - Add support for the New Master Rule
 - Decks update
 - New commandline parameters
 - Add support for Match and TAG duel
 - Add server mode
 - Bot dialogs now customable
 - Only use normal deck when random picking decks
 - Send sorry when the AI did something wrong that make the duel can't continue (for example, selected illegal card)
 - Send info when the deck of the AI is illegal (for example, lflist dismatch)
 - Fix the issue that the bot will attack _Dupe Frog_ with low attack monster when there is monster next to _Dupe Frog_
 - Fix the issue that synchro summon stuck in some condition [\#7](https://github.com/IceYGO/windbot/issues/7)
 - Fix C#6.0 (VS2015) support
 - Fix `OnUpdateData`
 - New and updated `DefaultExecutor`
 - New and updated `AI.Utils`, `ClientCard`, `ClientField` functions
 - Add `OnNewTurn`, `AI.SelectYesNo`, `AI.SelectThirdCard`, `Duel.ChainTargets`, `Duel.LastSummonPlayer`
 - Shortcut `Bot` for `Duel.Fields[0]`, `Enemy` for `Duel.Fields[1]`
 - `CardId` is now class instead of enum so `(int)` is no longer needed
 - Update the known card enums, add `Floodgate`, `OneForXyz`, `FusionSpell`, `MonsterHasPreventActivationEffectInBattle`
 - Update `OnPreBattleBetween` to calculate the ATK of cards like _Number S39: Utopia the Lightning_
 - Update direct attack handling

## v0x133D (2017-09-24)

 - Update YGOPro protrol to 0x133D
 - Use the latest YGOSharp.Network to improve performances
 - Update the namespace of `YGOSharp.OCGWrapper`
 - Fix the default trap cards not always activating
