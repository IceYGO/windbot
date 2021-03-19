using System;
using System.Collections.Generic;
using System.Linq;
using YGOSharp.OCGWrapper.Enums;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI
{
    public abstract class DefaultExecutor : Executor
    {
        protected class _CardId
        {
            public const int JizukirutheStarDestroyingKaiju = 63941210;
            public const int ThunderKingtheLightningstrikeKaiju = 48770333;
            public const int DogorantheMadFlameKaiju = 93332803;
            public const int RadiantheMultidimensionalKaiju = 28674152;
            public const int GadarlatheMysteryDustKaiju = 36956512;
            public const int KumongoustheStickyStringKaiju = 29726552;
            public const int GamecieltheSeaTurtleKaiju = 55063751;
            public const int SuperAntiKaijuWarMachineMechaDogoran = 84769941;

            public const int SandaionTheTimelord = 33015627;
            public const int GabrionTheTimelord = 6616912;
            public const int MichionTheTimelord = 7733560;
            public const int ZaphionTheTimelord = 28929131;
            public const int HailonTheTimelord = 34137269;
            public const int RaphionTheTimelord = 60222213;
            public const int SadionTheTimelord = 65314286;
            public const int MetaionTheTimelord = 74530899;
            public const int KamionTheTimelord = 91712985;
            public const int LazionTheTimelord = 92435533;

            public const int LeftArmofTheForbiddenOne = 7902349;
            public const int RightLegofTheForbiddenOne = 8124921;
            public const int LeftLegofTheForbiddenOne = 44519536;
            public const int RightArmofTheForbiddenOne = 70903634;
            public const int ExodiaTheForbiddenOne = 33396948;

            public const int UltimateConductorTytanno = 18940556;
            public const int ElShaddollConstruct = 20366274;
            public const int AllyOfJusticeCatastor = 26593852;

            public const int DupeFrog = 46239604;
            public const int MaraudingCaptain = 2460565;

            public const int BlackRoseDragon = 73580471;
            public const int JudgmentDragon = 57774843;
            public const int TopologicTrisbaena = 72529749;
            public const int EvilswarmExcitonKnight = 46772449;
            public const int HarpiesFeatherDuster = 18144506;
            public const int DarkMagicAttack = 2314238;
            public const int MysticalSpaceTyphoon = 5318639;
            public const int CosmicCyclone = 8267140;
            public const int GalaxyCyclone = 5133471;
            public const int BookOfMoon = 14087893;
            public const int CompulsoryEvacuationDevice = 94192409;
            public const int CallOfTheHaunted = 97077563;
            public const int Scapegoat = 73915051;
            public const int BreakthroughSkill = 78474168;
            public const int SolemnJudgment = 41420027;
            public const int SolemnWarning = 84749824;
            public const int SolemnStrike = 40605147;
            public const int TorrentialTribute = 53582587;
            public const int HeavyStorm = 19613556;
            public const int HammerShot = 26412047;
            public const int DarkHole = 53129443;
            public const int Raigeki = 12580477;
            public const int SmashingGround = 97169186;
            public const int PotOfDesires = 35261759;
            public const int AllureofDarkness = 1475311;
            public const int DimensionalBarrier = 83326048;
            public const int InterruptedKaijuSlumber = 99330325;

            public const int ChickenGame = 67616300;
            public const int SantaClaws = 46565218;

            public const int CastelTheSkyblasterMusketeer = 82633039;
            public const int CrystalWingSynchroDragon = 50954680;
            public const int NumberS39UtopiaTheLightning = 56832966;
            public const int Number39Utopia = 84013237;
            public const int UltimayaTzolkin = 1686814;
            public const int MekkKnightCrusadiaAstram = 21887175;
            public const int HamonLordofStrikingThunder = 32491822;

            public const int MoonMirrorShield = 19508728;
            public const int PhantomKnightsFogBlade = 25542642;

            public const int VampireFraeulein = 6039967;
            public const int InjectionFairyLily = 79575620;

            public const int BlueEyesChaosMAXDragon = 55410871;

            public const int AshBlossom = 14558127;
            public const int MaxxC = 23434538;
            public const int LockBird = 94145021;
            public const int GhostOgreAndSnowRabbit = 59438930;
            public const int GhostBelle = 73642296;
            public const int EffectVeiler = 97268402;
            public const int ArtifactLancea = 34267821;

            public const int CalledByTheGrave = 24224830;
            public const int InfiniteImpermanence = 10045474;
            public const int GalaxySoldier = 46659709;
            public const int MacroCosmos = 30241314;
            public const int UpstartGoblin = 70368879;
            public const int CyberEmergency = 60600126;

            public const int EaterOfMillions = 63845230;

            public const int InvokedPurgatrio = 12307878;
            public const int ChaosAncientGearGiant = 51788412;
            public const int UltimateAncientGearGolem = 12652643;

            public const int RedDragonArchfiend = 70902743;

            public const int ImperialOrder = 61740673;
            public const int RoyalDecreel = 51452091;
            public const int NaturiaBeast = 33198837;
            public const int AntiSpellFragrance = 58921041;
        }

        protected DefaultExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, _CardId.ChickenGame, DefaultChickenGame);
            AddExecutor(ExecutorType.Activate, _CardId.SantaClaws);
        }

        /// <summary>
        /// Decide which card should the attacker attack.
        /// </summary>
        /// <param name="attacker">Card that attack.</param>
        /// <param name="defenders">Cards that defend.</param>
        /// <returns>BattlePhaseAction including the target, or null (in this situation, GameAI will check the next attacker)</returns>
        public override BattlePhaseAction OnSelectAttackTarget(ClientCard attacker, IList<ClientCard> defenders)
        {
            foreach (ClientCard defender in defenders)
            {
                attacker.RealPower = attacker.Attack;
                defender.RealPower = defender.GetDefensePower();
                if (!OnPreBattleBetween(attacker, defender))
                    continue;

                if (attacker.RealPower > defender.RealPower || (attacker.RealPower >= defender.RealPower && attacker.IsLastAttacker && defender.IsAttack()))
                    return AI.Attack(attacker, defender);
            }

            if (attacker.CanDirectAttack)
                return AI.Attack(attacker, null);

            return null;
        }

        /// <summary>
        /// Decide whether to declare attack between attacker and defender.
        /// Can be overrided to update the RealPower of attacker for cards like Honest.
        /// </summary>
        /// <param name="attacker">Card that attack.</param>
        /// <param name="defender">Card that defend.</param>
        /// <returns>false if the attack shouldn't be done.</returns>
        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            if (!attacker.IsMonsterHasPreventActivationEffectInBattle())
            {
                if (defender.IsMonsterInvincible() && defender.IsDefense())
                    return false;

                if (defender.IsMonsterDangerous())
                {
                    bool canIgnoreIt = !attacker.IsDisabled() && (
                        attacker.IsCode(_CardId.UltimateConductorTytanno) && defender.IsDefense() || 
                        attacker.IsCode(_CardId.ElShaddollConstruct) && defender.IsSpecialSummoned ||
                        attacker.IsCode(_CardId.AllyOfJusticeCatastor) && !defender.HasAttribute(CardAttribute.Dark));
                    if (!canIgnoreIt)
                        return false;
                }

                foreach (ClientCard equip in defender.EquipCards)
                {
                    if (equip.IsCode(_CardId.MoonMirrorShield) && !equip.IsDisabled())
                    {
                        return false;
                    }
                }

                if (!defender.IsDisabled())
                {
                    if (defender.IsCode(_CardId.MekkKnightCrusadiaAstram) && defender.IsAttack() && attacker.IsSpecialSummoned)
                        return false;

                    if (defender.IsCode(_CardId.CrystalWingSynchroDragon) && defender.IsAttack() && attacker.Level >= 5)
                        return false;

                    if (defender.IsCode(_CardId.AllyOfJusticeCatastor) && !attacker.HasAttribute(CardAttribute.Dark))
                        return false;

                    if (defender.IsCode(_CardId.NumberS39UtopiaTheLightning) && defender.IsAttack() && defender.HasXyzMaterial(2, _CardId.Number39Utopia))
                        defender.RealPower = 5000;

                    if (defender.IsCode(_CardId.VampireFraeulein))
                        defender.RealPower += (Enemy.LifePoints > 3000) ? 3000 : (Enemy.LifePoints - 100);

                    if (defender.IsCode(_CardId.InjectionFairyLily) && Enemy.LifePoints > 2000)
                        defender.RealPower += 3000;
                }
            }

            if (!defender.IsMonsterHasPreventActivationEffectInBattle())
            {
                if (attacker.IsCode(_CardId.NumberS39UtopiaTheLightning) && !attacker.IsDisabled() && attacker.HasXyzMaterial(2, _CardId.Number39Utopia))
                    attacker.RealPower = 5000;

                if (attacker.IsCode(_CardId.EaterOfMillions) && !attacker.IsDisabled())
                    attacker.RealPower = 9999;

                if (attacker.IsMonsterInvincible())
                    attacker.RealPower = 9999;

                foreach (ClientCard equip in attacker.EquipCards)
                {
                    if (equip.IsCode(_CardId.MoonMirrorShield) && !equip.IsDisabled())
                    {
                        attacker.RealPower = defender.RealPower + 100;
                    }
                }
            }

            if (Enemy.HasInMonstersZone(_CardId.MekkKnightCrusadiaAstram, true) && !(defender).IsCode(_CardId.MekkKnightCrusadiaAstram))
                return false;

            if (Enemy.HasInMonstersZone(_CardId.DupeFrog, true) && !(defender).IsCode(_CardId.DupeFrog))
                return false;

            if (Enemy.HasInMonstersZone(_CardId.MaraudingCaptain, true) && !defender.IsCode(_CardId.MaraudingCaptain) && defender.Race == (int)CardRace.Warrior)
                return false;

            if (defender.IsCode(_CardId.UltimayaTzolkin) && !defender.IsDisabled() && Enemy.GetMonsters().Any(monster => !monster.Equals(defender) && monster.HasType(CardType.Synchro)))
                return false;

            if (Enemy.GetMonsters().Any(monster => !monster.Equals(defender) && monster.IsCode(_CardId.HamonLordofStrikingThunder) && !monster.IsDisabled() && monster.IsDefense()))
                return false;

            if (defender.OwnTargets.Any(card => card.IsCode(_CardId.PhantomKnightsFogBlade) && !card.IsDisabled()))
                return false;

            return true;
        }

        public override bool OnPreActivate(ClientCard card)
        {
            ClientCard LastChainCard = Util.GetLastChainCard();
            if (LastChainCard != null && Duel.Phase == DuelPhase.Standby &&
                LastChainCard.IsCode(
                    _CardId.SandaionTheTimelord,
                    _CardId.GabrionTheTimelord,
                    _CardId.MichionTheTimelord,
                    _CardId.ZaphionTheTimelord,
                    _CardId.HailonTheTimelord,
                    _CardId.RaphionTheTimelord,
                    _CardId.SadionTheTimelord,
                    _CardId.MetaionTheTimelord,
                    _CardId.KamionTheTimelord,
                    _CardId.LazionTheTimelord
                    ))
                return false;
            if ((card.Location == CardLocation.Hand || card.Location == CardLocation.SpellZone && card.IsFacedown()) &&
                (card.IsSpell() && DefaultSpellWillBeNegated() || card.IsTrap() && DefaultTrapWillBeNegated()))
                return false;
            return true;
        }

        /// <summary>
        /// Called when the AI has to select a card position.
        /// </summary>
        /// <param name="cardId">Id of the card to position on the field.</param>
        /// <param name="positions">List of available positions.</param>
        /// <returns>Selected position, or 0 if no position is set for this card.</returns>
        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            YGOSharp.OCGWrapper.NamedCard cardData = YGOSharp.OCGWrapper.NamedCard.Get(cardId);
            if (cardData != null)
            {
                if (cardData.Attack == 0)
                    return CardPosition.FaceUpDefence;
            }
            return 0;
        }

        public override bool OnSelectBattleReplay()
        {
            if (Bot.BattlingMonster == null)
                return false;
            List<ClientCard> defenders = new List<ClientCard>(Duel.Fields[1].GetMonsters());
            defenders.Sort(CardContainer.CompareDefensePower);
            defenders.Reverse();
            BattlePhaseAction result = OnSelectAttackTarget(Bot.BattlingMonster, defenders);
            if (result != null && result.Action == BattlePhaseAction.BattleAction.Attack)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Set when this card can't beat the enemies
        /// </summary>
        public override bool OnSelectMonsterSummonOrSet(ClientCard card)
        {
            return card.Level <= 4 && Bot.GetMonsters().Count(m => m.IsFaceup()) == 0 && Util.IsAllEnemyBetterThanValue(card.Attack, true);
        }

        /// <summary>
        /// Destroy face-down cards first, in our turn.
        /// </summary>
        protected bool DefaultMysticalSpaceTyphoon()
        {
            if (Duel.CurrentChain.Any(card => card.IsCode(_CardId.MysticalSpaceTyphoon)))
            {
                return false;
            }

            List<ClientCard> spells = Enemy.GetSpells();
            if (spells.Count == 0)
                return false;

            ClientCard selected = Enemy.SpellZone.GetFloodgate();

            if (selected == null)
            {
                if (Duel.Player == 0)
                    selected = spells.FirstOrDefault(card => card.IsFacedown());
                if (Duel.Player == 1)
                    selected = spells.FirstOrDefault(card => card.HasType(CardType.Continuous) || card.HasType(CardType.Equip) || card.HasType(CardType.Field));
            }

            if (selected == null)
                return false;
            AI.SelectCard(selected);
            return true;
        }

        /// <summary>
        /// Destroy face-down cards first, in our turn.
        /// </summary>
        protected bool DefaultCosmicCyclone()
        {
            foreach (ClientCard card in Duel.CurrentChain)
                if (card.IsCode(_CardId.CosmicCyclone))
                    return false;
            return (Bot.LifePoints > 1000) && DefaultMysticalSpaceTyphoon();
        }

        /// <summary>
        /// Activate if avail.
        /// </summary>
        protected bool DefaultGalaxyCyclone()
        {
            List<ClientCard> spells = Enemy.GetSpells();
            if (spells.Count == 0)
                return false;

            ClientCard selected = null;

            if (Card.Location == CardLocation.Grave)
            {
                selected = Util.GetBestEnemySpell(true);
            }
            else
            {
                selected = spells.FirstOrDefault(card => card.IsFacedown());
            }

            if (selected == null)
                return false;

            AI.SelectCard(selected);
            return true;
        }

        /// <summary>
        /// Set the highest ATK level 4+ effect enemy monster.
        /// </summary>
        protected bool DefaultBookOfMoon()
        {
            if (Util.IsAllEnemyBetter(true))
            {
                ClientCard monster = Enemy.GetMonsters().GetHighestAttackMonster(true);
                if (monster != null && monster.HasType(CardType.Effect) && !monster.HasType(CardType.Link) && (monster.HasType(CardType.Xyz) || monster.Level > 4))
                {
                    AI.SelectCard(monster);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Return problematic monster, and if this card become target, return any enemy monster.
        /// </summary>
        protected bool DefaultCompulsoryEvacuationDevice()
        {
            ClientCard target = Util.GetProblematicEnemyMonster(0, true);
            if (target != null)
            {
                AI.SelectCard(target);
                return true;
            }
            if (Util.IsChainTarget(Card))
            {
                ClientCard monster = Util.GetBestEnemyMonster(false, true);
                if (monster != null)
                {
                    AI.SelectCard(monster);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Revive the best monster when we don't have better one in field.
        /// </summary>
        protected bool DefaultCallOfTheHaunted()
        {
            if (!Util.IsAllEnemyBetter(true))
                return false;
            ClientCard selected = Bot.Graveyard.GetMatchingCards(card => card.IsCanRevive()).OrderByDescending(card => card.Attack).FirstOrDefault();
            AI.SelectCard(selected);
            return true;
        }

        /// <summary>
        /// Default Scapegoat effect
        /// </summary>
        protected bool DefaultScapegoat()
        {
            if (DefaultSpellWillBeNegated()) return false;
            if (Duel.Player == 0) return false;
            if (Duel.Phase == DuelPhase.End) return true;
            if (DefaultOnBecomeTarget()) return true;
            if (Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2)
            {
                if (Enemy.HasInMonstersZone(new[]
                {
                    _CardId.UltimateConductorTytanno,
                    _CardId.InvokedPurgatrio,
                    _CardId.ChaosAncientGearGiant,
                    _CardId.UltimateAncientGearGolem,
                    _CardId.RedDragonArchfiend
                }, true)) return false;
                if (Util.GetTotalAttackingMonsterAttack(1) >= Bot.LifePoints) return true;
            }
            return false;
        }
        /// <summary>
        /// Always active in opponent's turn.
        /// </summary>
        protected bool DefaultMaxxC()
        {
            return Duel.Player == 1;
        }
        /// <summary>
        /// Always disable opponent's effect except some cards like UpstartGoblin
        /// </summary>
        protected bool DefaultAshBlossomAndJoyousSpring()
        {
            int[] ignoreList = {
                _CardId.MacroCosmos,
                _CardId.UpstartGoblin,
                _CardId.CyberEmergency
            };
            if (Util.GetLastChainCard().IsCode(ignoreList))
                return false;
            if (Util.GetLastChainCard().HasSetcode(0x11e) && Util.GetLastChainCard().Location == CardLocation.Hand) // Danger! archtype hand effect
                return false;
            return Duel.LastChainPlayer == 1;
        }
        /// <summary>
        /// Always activate unless the activating card is disabled
        /// </summary>
        protected bool DefaultGhostOgreAndSnowRabbit()
        {
            if (Util.GetLastChainCard() != null && Util.GetLastChainCard().IsDisabled())
                return false;
            return DefaultTrap();
        }
        /// <summary>
        /// Always disable opponent's effect
        /// </summary>
        protected bool DefaultGhostBelleAndHauntedMansion()
        {
            return DefaultTrap();
        }
        /// <summary>
        /// Same as DefaultBreakthroughSkill
        /// </summary>
        protected bool DefaultEffectVeiler()
        {
            if (Util.GetLastChainCard() != null && Util.GetLastChainCard().IsCode(_CardId.GalaxySoldier) && Enemy.Hand.Count >= 3) return false;
            if (Util.ChainContainsCard(_CardId.EffectVeiler))
                return false;
            return DefaultBreakthroughSkill();
        }
        /// <summary>
        /// Chain common hand traps
        /// </summary>
        protected bool DefaultCalledByTheGrave()
        {
            int[] targetList =
            {
                _CardId.MaxxC,
                _CardId.LockBird,
                _CardId.GhostOgreAndSnowRabbit,
                _CardId.AshBlossom,
                _CardId.GhostBelle,
                _CardId.EffectVeiler,
                _CardId.ArtifactLancea
            };
            if (Duel.LastChainPlayer == 1)
            {
                foreach (int id in targetList)
                {
                    if (Util.GetLastChainCard().IsCode(id))
                    {
                        AI.SelectCard(id);
                        return UniqueFaceupSpell();
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Default InfiniteImpermanence effect
        /// </summary>
        protected bool DefaultInfiniteImpermanence()
        {
            // TODO: disable s & t
            if (!DefaultUniqueTrap())
                return false;
            return DefaultDisableMonster();
        }
        /// <summary>
        /// Chain the enemy monster, or disable monster like Rescue Rabbit.
        /// </summary>
        protected bool DefaultBreakthroughSkill()
        {
            if (!DefaultUniqueTrap())
                return false;
            return DefaultDisableMonster();
        }
        /// <summary>
        /// Chain the enemy monster, or disable monster like Rescue Rabbit.
        /// </summary>
        protected bool DefaultDisableMonster()
        {
            if (Duel.Player == 1)
            {
                ClientCard target = Enemy.MonsterZone.GetShouldBeDisabledBeforeItUseEffectMonster();
                if (target != null)
                {
                    AI.SelectCard(target);
                    return true;
                }
            }

            ClientCard LastChainCard = Util.GetLastChainCard();

            if (LastChainCard != null && LastChainCard.Controller == 1 && LastChainCard.Location == CardLocation.MonsterZone &&
                !LastChainCard.IsDisabled() && !LastChainCard.IsShouldNotBeTarget() && !LastChainCard.IsShouldNotBeSpellTrapTarget())
            {
                AI.SelectCard(LastChainCard);
                return true;
            }

            if (Bot.BattlingMonster != null && Enemy.BattlingMonster != null)
            {
                if (!Enemy.BattlingMonster.IsDisabled() && Enemy.BattlingMonster.IsCode(_CardId.EaterOfMillions))
                {
                    AI.SelectCard(Enemy.BattlingMonster);
                    return true;
                }
            }

            if (Duel.Phase == DuelPhase.BattleStart && Duel.Player == 1 &&
                Enemy.HasInMonstersZone(_CardId.NumberS39UtopiaTheLightning, true))
            {
                AI.SelectCard(_CardId.NumberS39UtopiaTheLightning);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Activate only except this card is the target or we summon monsters.
        /// </summary>
        protected bool DefaultSolemnJudgment()
        {
            return !Util.IsChainTargetOnly(Card) && !(Duel.Player == 0 && Duel.LastChainPlayer == -1) && DefaultTrap();
        }

        /// <summary>
        /// Activate only except we summon monsters.
        /// </summary>
        protected bool DefaultSolemnWarning()
        {
            return (Bot.LifePoints > 2000) && !(Duel.Player == 0 && Duel.LastChainPlayer == -1) && DefaultTrap();
        }

        /// <summary>
        /// Activate only except we summon monsters.
        /// </summary>
        protected bool DefaultSolemnStrike()
        {
            return (Bot.LifePoints > 1500) && !(Duel.Player == 0 && Duel.LastChainPlayer == -1) && DefaultTrap();
        }

        /// <summary>
        /// Activate when all enemy monsters have better ATK.
        /// </summary>
        protected bool DefaultTorrentialTribute()
        {
            return !Util.HasChainedTrap(0) && Util.IsAllEnemyBetter(true);
        }

        /// <summary>
        /// Activate enemy have more S&T.
        /// </summary>
        protected bool DefaultHeavyStorm()
        {
            return Bot.GetSpellCount() < Enemy.GetSpellCount();
        }

        /// <summary>
        /// Activate before other winds, if enemy have more than 2 S&T.
        /// </summary>
        protected bool DefaultHarpiesFeatherDusterFirst()
        {
            return Enemy.GetSpellCount() >= 2;
        }

        /// <summary>
        /// Activate when one enemy monsters have better ATK.
        /// </summary>
        protected bool DefaultHammerShot()
        {
            return Util.IsOneEnemyBetter(true);
        }

        /// <summary>
        /// Activate when one enemy monsters have better ATK or DEF.
        /// </summary>
        protected bool DefaultDarkHole()
        {
            return Util.IsOneEnemyBetter();
        }

        /// <summary>
        /// Activate when one enemy monsters have better ATK or DEF.
        /// </summary>
        protected bool DefaultRaigeki()
        {
            return Util.IsOneEnemyBetter();
        }

        /// <summary>
        /// Activate when one enemy monsters have better ATK or DEF.
        /// </summary>
        protected bool DefaultSmashingGround()
        {
            return Util.IsOneEnemyBetter();
        }

        /// <summary>
        /// Activate when we have more than 15 cards in deck.
        /// </summary>
        protected bool DefaultPotOfDesires()
        {
            return Bot.Deck.Count > 15;
        }

        /// <summary>
        /// Set traps only and avoid block the activation of other cards.
        /// </summary>
        protected bool DefaultSpellSet()
        {
            return (Card.IsTrap() || Card.HasType(CardType.QuickPlay) || DefaultSpellMustSetFirst()) && Bot.GetSpellCountWithoutField() < 4;
        }

        /// <summary>
        /// Summon with no tribute, or with tributes ATK lower.
        /// </summary>
        protected bool DefaultMonsterSummon()
        {
            if (Card.Level <= 4)
                return true;

            if (!UniqueFaceupMonster())
                return false;
            int tributecount = (int)Math.Ceiling((Card.Level - 4.0d) / 2.0d);
            for (int j = 0; j < 7; ++j)
            {
                ClientCard tributeCard = Bot.MonsterZone[j];
                if (tributeCard == null) continue;
                if (tributeCard.GetDefensePower() < Card.Attack)
                    tributecount--;
            }
            return tributecount <= 0;
        }

        /// <summary>
        /// Activate when we have no field.
        /// </summary>
        protected bool DefaultField()
        {
            return Bot.SpellZone[5] == null;
        }

        /// <summary>
        /// Turn if all enemy is better.
        /// </summary>
        protected bool DefaultMonsterRepos()
        {
            if (Card.Attack == 0)
            {
                if (Card.IsFaceup() && Card.IsAttack())
                    return true;
                if (Card.IsFaceup() && Card.IsDefense())
                    return false;
            }

            if (Enemy.HasInMonstersZone(_CardId.BlueEyesChaosMAXDragon, true) &&
                Card.IsAttack() && (4000 - Card.Defense) * 2 > (4000 - Card.Attack))
                return false;
            if (Enemy.HasInMonstersZone(_CardId.BlueEyesChaosMAXDragon, true) &&
                Card.IsDefense() && Card.IsFaceup() &&
                (4000 - Card.Defense) * 2 > (4000 - Card.Attack))
                return true;

            bool enemyBetter = Util.IsAllEnemyBetter();
            if (Card.IsAttack() && enemyBetter)
                return true;
            if (Card.IsDefense() && !enemyBetter && (Card.Attack >= Card.Defense || Card.Attack >= Util.GetBestPower(Enemy)))
                return true;

            return false;
        }

        /// <summary>
        /// If spell will be negated
        /// </summary>
        protected bool DefaultSpellWillBeNegated()
        {
            return (Bot.HasInSpellZone(_CardId.ImperialOrder, true, true) || Enemy.HasInSpellZone(_CardId.ImperialOrder, true)) && !Util.ChainContainsCard(_CardId.ImperialOrder);
        }

        /// <summary>
        /// If trap will be negated
        /// </summary>
        protected bool DefaultTrapWillBeNegated()
        {
            return (Bot.HasInSpellZone(_CardId.RoyalDecreel, true, true) || Enemy.HasInSpellZone(_CardId.RoyalDecreel, true)) && !Util.ChainContainsCard(_CardId.RoyalDecreel);
        }

        /// <summary>
        /// If spell must set first to activate
        /// </summary>
        protected bool DefaultSpellMustSetFirst()
        {
            return Bot.HasInSpellZone(_CardId.AntiSpellFragrance, true, true) || Enemy.HasInSpellZone(_CardId.AntiSpellFragrance, true);
        }

        /// <summary>
        /// if spell/trap is the target or enermy activate HarpiesFeatherDuster
        /// </summary>
        protected bool DefaultOnBecomeTarget()
        {
            if (Util.IsChainTarget(Card)) return true;
            int[] destroyAllList =
            {
                _CardId.EvilswarmExcitonKnight,
                _CardId.BlackRoseDragon,
                _CardId.JudgmentDragon,
                _CardId.TopologicTrisbaena
            };
            int[] destroyAllOpponentList =
            {
                _CardId.HarpiesFeatherDuster,
                _CardId.DarkMagicAttack
            };

            if (Util.ChainContainsCard(destroyAllList)) return true;
            if (Enemy.HasInSpellZone(destroyAllOpponentList, true)) return true;
            // TODO: ChainContainsCard(id, player)
            return false;
        }
        /// <summary>
        /// Chain enemy activation or summon.
        /// </summary>
        protected bool DefaultTrap()
        {
            return (Duel.LastChainPlayer == -1 && Duel.LastSummonPlayer != 0) || Duel.LastChainPlayer == 1;
        }

        /// <summary>
        /// Activate when avail and no other our trap card in this chain or face-up.
        /// </summary>
        protected bool DefaultUniqueTrap()
        {
            if (Util.HasChainedTrap(0))
                return false;

            return UniqueFaceupSpell();
        }

        /// <summary>
        /// Check no other our spell or trap card with same name face-up.
        /// </summary>
        protected bool UniqueFaceupSpell()
        {
            return !Bot.GetSpells().Any(card => card.IsCode(Card.Id) && card.IsFaceup());
        }

        /// <summary>
        /// Check no other our monster card with same name face-up.
        /// </summary>
        protected bool UniqueFaceupMonster()
        {
            return !Bot.GetMonsters().Any(card => card.IsCode(Card.Id) && card.IsFaceup());
        }

        /// <summary>
        /// Dumb way to avoid the bot chain in mess.
        /// </summary>
        protected bool DefaultDontChainMyself()
        {
            if (Type != ExecutorType.Activate)
                return true;
            if (Executors.Any(exec => exec.Type == Type && exec.CardId == Card.Id))
                return false;
            return Duel.LastChainPlayer != 0;
        }

        /// <summary>
        /// Draw when we have lower LP, or destroy it. Can be overrided.
        /// </summary>
        protected bool DefaultChickenGame()
        {
            if (Executors.Count(exec => exec.Type == Type && exec.CardId == Card.Id) > 1)
                return false;
            if (Card.IsFacedown())
                return true;
            if (Bot.LifePoints <= 1000)
                return false;
            if (Bot.LifePoints <= Enemy.LifePoints && ActivateDescription == Util.GetStringId(_CardId.ChickenGame, 0))
                return true;
            if (Bot.LifePoints > Enemy.LifePoints && ActivateDescription == Util.GetStringId(_CardId.ChickenGame, 1))
                return true;
            return false;
        }

        /// <summary>
        /// Draw when we have Dark monster in hand,and banish random one. Can be overrided.
        /// </summary>
        protected bool DefaultAllureofDarkness()
        {
            ClientCard target = Bot.Hand.FirstOrDefault(card => card.HasAttribute(CardAttribute.Dark));
            return target != null;
        }

        /// <summary>
        /// Clever enough.
        /// </summary>
        protected bool DefaultDimensionalBarrier()
        {
            const int RITUAL = 0;
            const int FUSION = 1;
            const int SYNCHRO = 2;
            const int XYZ = 3;
            const int PENDULUM = 4;
            if (Duel.Player != 0)
            {
                List<ClientCard> monsters = Enemy.GetMonsters();
                int[] levels = new int[13];
                bool tuner = false;
                bool nontuner = false;
                foreach (ClientCard monster in monsters)
                {
                    if (monster.HasType(CardType.Tuner))
                        tuner = true;
                    else if (!monster.HasType(CardType.Xyz) && !monster.HasType(CardType.Link))
                    {
                        nontuner = true;
                        levels[monster.Level] = levels[monster.Level] + 1;
                    }

                    if (monster.IsOneForXyz())
                    {
                        AI.SelectOption(XYZ);
                        return true;
                    }
                }
                if (tuner && nontuner)
                {
                    AI.SelectOption(SYNCHRO);
                    return true;
                }
                for (int i=1; i<=12; i++)
                {
                    if (levels[i]>1)
                    {
                        AI.SelectOption(XYZ);
                        return true;
                    }
                }
                ClientCard l = Enemy.SpellZone[6];
                ClientCard r = Enemy.SpellZone[7];
                if (l != null && r != null && l.LScale != r.RScale)
                {
                    AI.SelectOption(PENDULUM);
                    return true;
                }
            }
            ClientCard lastchaincard = Util.GetLastChainCard();
            if (Duel.LastChainPlayer == 1 && lastchaincard != null && !lastchaincard.IsDisabled())
            {
                if (lastchaincard.HasType(CardType.Ritual))
                {
                    AI.SelectOption(RITUAL);
                    return true;
                }
                if (lastchaincard.HasType(CardType.Fusion))
                {
                    AI.SelectOption(FUSION);
                    return true;
                }
                if (lastchaincard.HasType(CardType.Synchro))
                {
                    AI.SelectOption(SYNCHRO);
                    return true;
                }
                if (lastchaincard.HasType(CardType.Xyz))
                {
                    AI.SelectOption(XYZ);
                    return true;
                }
                if (lastchaincard.IsFusionSpell())
                {
                    AI.SelectOption(FUSION);
                    return true;
                }
            }
            if (Util.IsChainTarget(Card))
            {
                AI.SelectOption(XYZ);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Clever enough
        /// </summary>
        protected bool DefaultInterruptedKaijuSlumber()
        {
            if (Card.Location == CardLocation.Grave)
            {
                AI.SelectCard(
                    _CardId.GamecieltheSeaTurtleKaiju,
                    _CardId.KumongoustheStickyStringKaiju,
                    _CardId.GadarlatheMysteryDustKaiju,
                    _CardId.RadiantheMultidimensionalKaiju,
                    _CardId.DogorantheMadFlameKaiju,
                    _CardId.ThunderKingtheLightningstrikeKaiju,
                    _CardId.JizukirutheStarDestroyingKaiju
                    );
                return true;
            }

            if (DefaultDarkHole())
            {
                AI.SelectCard(
                    _CardId.JizukirutheStarDestroyingKaiju,
                    _CardId.ThunderKingtheLightningstrikeKaiju,
                    _CardId.DogorantheMadFlameKaiju,
                    _CardId.RadiantheMultidimensionalKaiju,
                    _CardId.GadarlatheMysteryDustKaiju,
                    _CardId.KumongoustheStickyStringKaiju,
                    _CardId.GamecieltheSeaTurtleKaiju
                    );
                AI.SelectNextCard(
                    _CardId.SuperAntiKaijuWarMachineMechaDogoran,
                    _CardId.GamecieltheSeaTurtleKaiju,
                    _CardId.KumongoustheStickyStringKaiju,
                    _CardId.GadarlatheMysteryDustKaiju,
                    _CardId.RadiantheMultidimensionalKaiju,
                    _CardId.DogorantheMadFlameKaiju,
                    _CardId.ThunderKingtheLightningstrikeKaiju
                    );
                return true;
            }

            return false;
        }

        /// <summary>
        /// Clever enough.
        /// </summary>
        protected bool DefaultKaijuSpsummon()
        {
            IList<int> kaijus = new[] {
                _CardId.JizukirutheStarDestroyingKaiju,
                _CardId.GadarlatheMysteryDustKaiju,
                _CardId.GamecieltheSeaTurtleKaiju,
                _CardId.RadiantheMultidimensionalKaiju,
                _CardId.KumongoustheStickyStringKaiju,
                _CardId.ThunderKingtheLightningstrikeKaiju,
                _CardId.DogorantheMadFlameKaiju,
                _CardId.SuperAntiKaijuWarMachineMechaDogoran
            };
            foreach (ClientCard monster in Enemy.GetMonsters())
            {
                if (monster.IsCode(kaijus))
                    return Card.GetDefensePower() > monster.GetDefensePower();
            }
            ClientCard card = Enemy.MonsterZone.GetFloodgate();
            if (card != null)
            {
                AI.SelectCard(card);
                return true;
            }
            card = Enemy.MonsterZone.GetDangerousMonster();
            if (card != null)
            {
                AI.SelectCard(card);
                return true;
            }
            card = Util.GetOneEnemyBetterThanValue(Card.GetDefensePower());
            if (card != null)
            {
                AI.SelectCard(card);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Summon when we don't have monster attack higher than enemy's.
        /// </summary>
        protected bool DefaultNumberS39UtopiaTheLightningSummon()
        {
            int bestBotAttack = Util.GetBestAttack(Bot);
            return Util.IsOneEnemyBetterThanValue(bestBotAttack, false);
        }

        /// <summary>
        /// Activate if the card is attack pos, and its attack is below 5000, when the enemy monster is attack pos or not useless faceup defense pos
        /// </summary>
        protected bool DefaultNumberS39UtopiaTheLightningEffect()
        {
            return Card.IsAttack() && Card.Attack < 5000 && (Enemy.BattlingMonster.IsAttack() || Enemy.BattlingMonster.IsFacedown() || Enemy.BattlingMonster.GetDefensePower() >= Card.Attack);
        }

        /// <summary>
        /// Summon when it can and should use effect.
        /// </summary>
        protected bool DefaultEvilswarmExcitonKnightSummon()
        {
            int selfCount = Bot.GetMonsterCount() + Bot.GetSpellCount() + Bot.GetHandCount();
            int oppoCount = Enemy.GetMonsterCount() + Enemy.GetSpellCount() + Enemy.GetHandCount();
            return (selfCount - 1 < oppoCount) && DefaultEvilswarmExcitonKnightEffect();
        }

        /// <summary>
        /// Activate when we have less cards than enemy's, or the atk sum of we is lower than enemy's.
        /// </summary>
        protected bool DefaultEvilswarmExcitonKnightEffect()
        {
            int selfCount = Bot.GetMonsterCount() + Bot.GetSpellCount();
            int oppoCount = Enemy.GetMonsterCount() + Enemy.GetSpellCount();

            if (selfCount < oppoCount)
                return true;

            int selfAttack = Bot.GetMonsters().Sum(monster => (int?)monster.GetDefensePower()) ?? 0;
            int oppoAttack = Enemy.GetMonsters().Sum(monster => (int?)monster.GetDefensePower()) ?? 0;

            return selfAttack < oppoAttack;
        }

        /// <summary>
        /// Summon in main2, or when the attack of we is lower than enemy's, but not when enemy have monster higher than 2500.
        /// </summary>
        protected bool DefaultStardustDragonSummon()
        {
            int selfBestAttack = Util.GetBestAttack(Bot);
            int oppoBestAttack = Util.GetBestPower(Enemy);
            return (selfBestAttack <= oppoBestAttack && oppoBestAttack <= 2500) || Util.IsTurn1OrMain2();
        }

        /// <summary>
        /// Negate enemy's destroy effect, and revive from grave.
        /// </summary>
        protected bool DefaultStardustDragonEffect()
        {
            return (Card.Location == CardLocation.Grave) || Duel.LastChainPlayer == 1;
        }

        /// <summary>
        /// Summon when enemy have card which we must solve.
        /// </summary>
        protected bool DefaultCastelTheSkyblasterMusketeerSummon()
        {
            return Util.GetProblematicEnemyCard() != null;
        }

        /// <summary>
        /// Bounce the problematic enemy card. Ignore the 1st effect.
        /// </summary>
        protected bool DefaultCastelTheSkyblasterMusketeerEffect()
        {
            if (ActivateDescription == Util.GetStringId(_CardId.CastelTheSkyblasterMusketeer, 0))
                return false;
            ClientCard target = Util.GetProblematicEnemyCard();
            if (target != null)
            {
                AI.SelectCard(0);
                AI.SelectNextCard(target);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Summon when it should use effect, or when the attack of we is lower than enemy's, but not when enemy have monster higher than 3000.
        /// </summary>
        protected bool DefaultScarlightRedDragonArchfiendSummon()
        {
            int selfBestAttack = Util.GetBestAttack(Bot);
            int oppoBestAttack = Util.GetBestPower(Enemy);
            return (selfBestAttack <= oppoBestAttack && oppoBestAttack <= 3000) || DefaultScarlightRedDragonArchfiendEffect();
        }

        protected bool DefaultTimelordSummon()
        {
            return Bot.GetMonsterCount() == 0;
        }

        /// <summary>
        /// Activate when we have less monsters than enemy, or when enemy have more than 3 monsters.
        /// </summary>
        protected bool DefaultScarlightRedDragonArchfiendEffect()
        {
            int selfCount = Bot.GetMonsters().Count(monster => !monster.Equals(Card) && monster.IsSpecialSummoned && monster.HasType(CardType.Effect) && monster.Attack <= Card.Attack);
            int oppoCount = Enemy.GetMonsters().Count(monster => monster.IsSpecialSummoned && monster.HasType(CardType.Effect) && monster.Attack <= Card.Attack);
            return selfCount <= oppoCount && oppoCount > 0 || oppoCount >= 3;
        }

        /// <summary>
        /// Clever enough.
        /// </summary>
        protected bool DefaultHonestEffect()
        {
            if (Card.Location == CardLocation.Hand)
            {
                return Bot.BattlingMonster.IsAttack() &&
                    (((Bot.BattlingMonster.Attack < Enemy.BattlingMonster.Attack) || Bot.BattlingMonster.Attack >= Enemy.LifePoints)
                    || ((Bot.BattlingMonster.Attack < Enemy.BattlingMonster.Defense) && (Bot.BattlingMonster.Attack + Enemy.BattlingMonster.Attack > Enemy.BattlingMonster.Defense)));
            }

            return Util.IsTurn1OrMain2();
        }
    }
}
