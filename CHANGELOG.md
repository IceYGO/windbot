# Changelog

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
