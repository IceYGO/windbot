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
            public const int EvenlyMatched = 15693423;
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
            public const int GhostMournerMoonlitChill = 52038441;
            public const int ArtifactLancea = 34267821;
            public const int DimensionShifter = 91800273;
            public const int NibiruThePrimalBeing = 27204311;
            public const int MulcharmyPurulia = 84192580;
            public const int MulcharmyFuwalos = 42141493;
            public const int MulcharmyNyalus = 87126721;

            public const int CalledByTheGrave = 24224830;
            public const int CrossoutDesignator = 65681983;
            public const int InfiniteImpermanence = 10045474;
            public const int GalaxySoldier = 46659709;
            public const int MacroCosmos = 30241314;
            public const int UpstartGoblin = 70368879;
            public const int CyberEmergency = 60600126;
            public const int TheAgentOfCreationVenus = 64734921;

            public const int EaterOfMillions = 63845230;

            public const int InvokedPurgatrio = 12307878;
            public const int ChaosAncientGearGiant = 51788412;
            public const int UltimateAncientGearGolem = 12652643;

            public const int RedDragonArchfiend = 70902743;

            public const int ImperialOrder = 61740673;
            public const int RoyalDecreel = 51452091;
            public const int NaturalExterio = 99916754;
            public const int NaturiaBeast = 33198837;
            public const int SwordsmanLV7 = 37267041;
            public const int AntiSpellFragrance = 58921041;
            public const int Number41BagooskatheTerriblyTiredTapir = 90590303;
            public const int SkillDrain = 82732705;

            public const int DimensionalFissure = 81674782;
            public const int BanisheroftheRadiance = 94853057;
            public const int BanisheroftheLight = 61528025;
            public const int KashtiraAriseHeart = 48626373;
            public const int MaskedHERODarkLaw = 58481572;

            public const int VaylantzWorld_ShinraBansho = 49568943;
            public const int VaylantzWorld_KonigWissen = 75952542;
            public const int DivineArsenalAAZEUS_SkyThunder = 90448279;
            public const int LightningStorm = 14532163;

            public const int BelialMarquisOfDarkness = 33655493;
            public const int ChirubiméPrincessOfAutumnLeaves = 87294988;
            public const int PerformapalBarokuriboh = 19050066;
            public const int LabrynthArchfiend = 48745395;
            public const int HarpiesPetDragonFearsomeFireBlast = 4991081;
            public const int DynaHeroFurHire = 25123713;
            public const int Hieracosphinx = 82260502;
            public const int SpeedroidPassinglider = 26420373;
            public const int TyrOfTheNordicChampions = 2333365;
            public const int ValkyrianKnight = 99348756;
            public const int Victoria = 75162696;
            public const int MadolcheChouxvalier = 75363626;
            public const int LadyOfD = 67511500;
            public const int MermailAbysslung = 95466842;
            public const int HarpiesPetBabyDragon = 6924874;
            public const int HandHoldingGenie = 94535485;
            public const int GolemDragon = 9666558;
            public const int TwilightRoseKnight = 2986553;
            public const int PerformapalThunderhino = 70458081;
            public const int MiracleFlipper = 131182;
            public const int Decoyroid = 25034083;
            public const int AltergeistFifinellag = 12977245;
            public const int BatterymanD = 55401221;
            public const int Watthopper = 61380658;
            public const int EgyptianGodSlime = 42166000;
            public const int DinowrestlerChimeraTWrextle = 22900219;
            public const int DinowrestlerGigaSpinosavate = 58672736;
            public const int ScarredWarrior = 45298492;
            public const int SharkFortress = 50449881;
            public const int HeroicChampionClaivesolish = 97453744;
            public const int GhostrickAlucard = 75367227;
            public const int DinowrestlerKingTWrextle = 77967790;
            public const int NumberF0UtopicFutureZexal = 41522092;

            public const int PerformapalMissDirector = 92932860;
            public const int AncientWarriorsMasterfulSunMou = 40140448;
            public const int AncientWarriorsVirtuousLiuXuan = 40428851;
            public const int CommandKnight = 10375182;
            public const int HunterOwl = 51962254;
            public const int RokketRecharger = 5969957;
            public const int EmissaryOfTheOasis = 6103294;
            public const int Zuttomozaurus = 24454387;
            public const int Otoshidamashi = 14957440;
            public const int NaturiaMosquito = 17285476;
            public const int RescueACEHydrant = 37617348;
            public const int MeizenTheBattleNinja = 11825276;
            public const int VindikiteRGenex = 73483491;
            public const int PrincessCologne = 75574498;
            public const int Number48ShadowLich = 1426714;
            public const int PhantomToken = 1426715;
            public const int DuelLinkDragonTheDuelDragon = 60025883;
            public const int DuelDragonToken = 60025884;
            public const int SeleneQueenOfTheMasterMagicians = 45819647;
            public const int TheWingedDragonofRaSphereMode = 10000080;
            public const int SelettriceVaalmonica = 23093373;
            public const int PerformageTrapezeWitch = 33206889;
            public const int PoseidraTheStormingAtlantean = 99193444;

            public const int RockOfTheVanquisher = 28168628;
            public const int SpiralDischarge = 29477860;
            public const int GaiaTheDragonChampion = 66889139;
            public const int CrusadiaVanguard = 55312487;
            public const int GladiatorBeastDomitianus = 33652635;
            public const int PatricianOfDarkness = 19153634;
            public const int DictatorOfD = 66961194;

            public const int LoThePrayersOfTheVoicelessVoice = 25801745;
            public const int BarrierOfTheVoicelessVoice = 98477480;

            public const int DiabellzeOfTheOriginalSin = 53765052;
            public const int PotOfExtravagance = 49238328;
        }

        protected class _Setcode
        {
            public const int Watt = 0xe;
            public const int Speedroid = 0x2016;
            public const int EarthboundImmortal = 0x1021;
            public const int Naturia = 0x2a;
            public const int Nordic = 0x42;
            public const int Harpie = 0x64;
            public const int Madolche = 0x71;
            public const int Ghostrick = 0x8d;
            public const int OddEyes = 0x99;
            public const int Performapal = 0x9f;
            public const int Performage = 0xc6;
            public const int BlueEyes = 0xdd;
            public const int FurHire = 0x114;
            public const int Altergeist = 0x103;
            public const int Crusadia = 0x116;
            public const int Danger = 0x11e;
            public const int Endymion = 0x12a;
            public const int AncientWarriors = 0x137;
            public const int RescueACE = 0x18b;
            public const int VanquishSoul = 0x195;
            public const int Horus = 0x19d;
        }

        protected DefaultExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, _CardId.ChickenGame, DefaultChickenGame);
            AddExecutor(ExecutorType.Activate, _CardId.VaylantzWorld_ShinraBansho, DefaultVaylantzWorld_ShinraBansho);
            AddExecutor(ExecutorType.Activate, _CardId.VaylantzWorld_KonigWissen, DefaultVaylantzWorld_KonigWissen);
            AddExecutor(ExecutorType.Activate, _CardId.SantaClaws);
            AddExecutor(ExecutorType.SpellSet, DefaultSetForDiabellze);
        }

        protected int lightningStormOption = -1;
        Dictionary<int, int> calledbytheGraveIdCountMap = new Dictionary<int, int>();
        List<int> crossoutDesignatorIdList = new List<int>();

        /// <summary>
        /// Defined:
        /// if monster with code as KEY, other monsters with rules as VALUE won't be targeted for attack.
        /// </summary>
        protected Dictionary<int, Func<ClientCard, bool>> DefenderProtectRule = new Dictionary<int, Func<ClientCard, bool>> {
            {_CardId.BelialMarquisOfDarkness, defender => defender.IsFaceup()},
            {_CardId.ChirubiméPrincessOfAutumnLeaves, defender => defender.HasRace(CardRace.Plant)},
            {_CardId.PerformapalBarokuriboh, defender => true},
            {_CardId.LabrynthArchfiend, defender => defender.HasRace(CardRace.Fiend) && !defender.IsCode(_CardId.LabrynthArchfiend)},
            {_CardId.HarpiesPetDragonFearsomeFireBlast, defender => defender.Level <= 6 && defender.HasSetcode(_Setcode.Harpie)},
            {_CardId.DynaHeroFurHire, defender => defender.HasSetcode(_Setcode.FurHire)},
            {_CardId.Hieracosphinx, defender => defender.IsFacedown()},
            {_CardId.SpeedroidPassinglider, defender => defender.HasSetcode(_Setcode.Speedroid)},
            {_CardId.TyrOfTheNordicChampions, defender => defender.HasSetcode(_Setcode.Nordic)},
            {_CardId.ValkyrianKnight, defender => defender.HasRace(CardRace.Warrior) && !defender.IsCode(_CardId.ValkyrianKnight)},
            {_CardId.Victoria, defender => defender.HasRace(CardRace.Fairy)},
            {_CardId.MadolcheChouxvalier, defender => defender.HasSetcode(_Setcode.Madolche) && !defender.IsCode(_CardId.MadolcheChouxvalier)},
            {_CardId.LadyOfD, defender => defender.HasRace(CardRace.Dragon)},
            {_CardId.MermailAbysslung, defender => defender.HasAttribute(CardAttribute.Water)},
            {_CardId.HarpiesPetBabyDragon, defender => defender.HasSetcode(_Setcode.Harpie) && !defender.IsCode(_CardId.HarpiesPetBabyDragon)},
            {_CardId.HandHoldingGenie, defender => true},
            {_CardId.GolemDragon, defender => defender.HasRace(CardRace.Dragon)},
            {_CardId.MaraudingCaptain, defender => defender.HasRace(CardRace.Warrior)},
            {_CardId.TwilightRoseKnight, defender => defender.HasRace(CardRace.Plant)},
            {_CardId.PerformapalThunderhino, defender => defender.HasSetcode(_Setcode.Performapal)},
            {_CardId.MiracleFlipper, defender => defender.IsFaceup()},
            {_CardId.Decoyroid, defender => defender.IsFaceup()},
            {_CardId.DupeFrog, defender => true},
            {_CardId.AltergeistFifinellag, defender => defender.HasSetcode(_Setcode.Altergeist)},
            {_CardId.BatterymanD, defender => defender.HasRace(CardRace.Thunder) && !defender.IsCode(_CardId.BatterymanD)},
            {_CardId.Watthopper, defender => defender.HasSetcode(_Setcode.Watt) && defender.IsFaceup()},

            {_CardId.EgyptianGodSlime, defender => true},
            {_CardId.DinowrestlerChimeraTWrextle, defender => true},
            {_CardId.DinowrestlerGigaSpinosavate, defender => true},
            {_CardId.ScarredWarrior, defender => defender.HasRace(CardRace.Warrior) && defender.IsFaceup()},
            {_CardId.SharkFortress, defender => true},
            {_CardId.HeroicChampionClaivesolish, defender => true},
            {_CardId.GhostrickAlucard, defender => defender.HasSetcode(_Setcode.Ghostrick) || defender.IsFacedown()},
            {_CardId.MekkKnightCrusadiaAstram, defender => true},
            {_CardId.DinowrestlerKingTWrextle, defender => true},
            {_CardId.NumberF0UtopicFutureZexal, defender => true}
        };

        /// <summary>
        /// Defined:
        /// if monster with KEY on field, and meet VALUE(monster, all monster), it cannot be targeted for attack.
        /// </summary>
        protected Dictionary<int, Func<ClientCard, List<ClientCard>, bool>> DefenderInvisbleRule = new Dictionary<int, Func<ClientCard, List<ClientCard>, bool>> {
            {_CardId.UltimayaTzolkin, (defender, list) => list.Any(monster => !monster.Equals(defender) && monster.HasType(CardType.Synchro))},
            {_CardId.PerformapalMissDirector, (defender, list) => list.Any(monster => monster.HasSetcode(_Setcode.OddEyes))},
            {_CardId.AncientWarriorsMasterfulSunMou, (defender, list) => list.Any(monster => !monster.Equals(defender) && monster.HasSetcode(_Setcode.AncientWarriors))},
            {_CardId.AncientWarriorsVirtuousLiuXuan, (defender, list) => list.Any(monster => !monster.Equals(defender) && monster.HasSetcode(_Setcode.AncientWarriors))},
            {_CardId.CommandKnight, (defender, list) => list.Any(monster => !monster.Equals(defender))},
            {_CardId.HunterOwl, (defender, list) => list.Any(monster => !monster.Equals(defender) && monster.HasAttribute(CardAttribute.Wind))},
            {_CardId.RokketRecharger, (defender, list) => list.Any(monster => monster.IsExtraCard() && monster.HasAttribute(CardAttribute.Dark))},
            {_CardId.EmissaryOfTheOasis, (defender, list) => list.Any(monster => monster.HasType(CardType.Normal) && monster.Level <= 3)},
            {_CardId.Zuttomozaurus, (defender, list) => list.Any(monster => !monster.Equals(defender) && monster.HasRace(CardRace.Dinosaur))},
            {_CardId.Otoshidamashi, (defender, list) => list.Any(monster => !monster.HasType(CardType.Tuner))},
            {_CardId.NaturiaMosquito, (defender, list) => list.Any(monster => !monster.Equals(defender) && monster.HasSetcode(_Setcode.Naturia))},
            {_CardId.RescueACEHydrant, (defender, list) => list.Any(monster => !monster.IsCode(_CardId.RescueACEHydrant) && monster.HasSetcode(_Setcode.RescueACE))},

            {_CardId.MeizenTheBattleNinja, (defender, list) => list.Any(monster => monster.IsFacedown())},
            {_CardId.VindikiteRGenex, (defender, list) => true},
            {_CardId.PrincessCologne, (defender, list) => list.Any(monster => !monster.Equals(defender))},
            {_CardId.Number48ShadowLich, (defender, list) => list.Any(monster => monster.IsCode(_CardId.PhantomToken))},
            {_CardId.DuelLinkDragonTheDuelDragon, (defender, list) => list.Any(monster => monster.IsCode(_CardId.DuelDragonToken))},
            {_CardId.SeleneQueenOfTheMasterMagicians, (defender, list) => list.Any(monster => monster.HasSetcode(_Setcode.Endymion))},

            {_CardId.TheWingedDragonofRaSphereMode, (defender, list) => true},
            {_CardId.SelettriceVaalmonica, (defender, list) => list.Any(monster => !monster.IsCode(_CardId.SelettriceVaalmonica))},
            {_CardId.PerformageTrapezeWitch, (defender, list) => list.Any(monster => !monster.IsCode(_CardId.PerformageTrapezeWitch) && monster.HasSetcode(_Setcode.Performage))},
            {_CardId.PoseidraTheStormingAtlantean, (defender, list) => list.Any(monster => !monster.IsCode(_CardId.PoseidraTheStormingAtlantean))}
        };

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

                if (defender.EquipCards.Any(equip => equip.IsCode(_CardId.MoonMirrorShield) && !equip.IsDisabled()))
                    return false;

                if (!defender.IsDisabled())
                {
                    if (defender.IsCode(_CardId.MekkKnightCrusadiaAstram) && defender.IsAttack() && attacker.IsSpecialSummoned)
                        defender.RealPower += attacker.Attack;

                    if (defender.IsCode(_CardId.CrystalWingSynchroDragon) && defender.IsAttack() && attacker.Level >= 5)
                        defender.RealPower += attacker.Attack;

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

                if (attacker.EquipCards.Any(equip => equip.IsCode(_CardId.MoonMirrorShield) && !equip.IsDisabled()))
                    attacker.RealPower = defender.RealPower + 100;
            }

            foreach (ClientCard protecter in Enemy.GetMonsters())
            {
                if (!protecter.IsDisabled() && protecter != defender)
                {
                    Func<ClientCard, bool> defenderRule = card => false;
                    if (DefenderProtectRule.TryGetValue(protecter.Id, out defenderRule))
                    {
                        if (defenderRule(defender)) return false;
                    }
                }
            }

            if (attacker.EquipCards.Any(equip => equip.IsCode(_CardId.MoonMirrorShield) && !equip.IsDisabled()))
                attacker.RealPower = defender.RealPower + 100;

            if (!defender.IsDisabled())
            {
                Func<ClientCard, List<ClientCard>, bool> defenderRule = (card, monsterList) => false;
                if (DefenderInvisbleRule.TryGetValue(defender.Id, out defenderRule))
                {
                    if (defenderRule(defender, Enemy.GetMonsters())) return false;
                }
            }

            if (Enemy.GetMonsters().Any(monster => !monster.Equals(defender) && monster.IsCode(_CardId.HamonLordofStrikingThunder) && !monster.IsDisabled() && monster.IsDefense()))
                return false;

            if (defender.OwnTargets.Any(card => card.IsCode(_CardId.PhantomKnightsFogBlade) && !card.IsDisabled()))
                return false;

            if (defender.HasSetcode(_Setcode.EarthboundImmortal) && !defender.IsDisabled())
                return false;

            bool attackHighestMonster =
                Enemy.HasInMonstersZone(_CardId.RockOfTheVanquisher, true) && Enemy.GetMonsters().Any(card => card.HasSetcode(_Setcode.VanquishSoul)) ||
                Enemy.HasInMonstersZone(_CardId.GladiatorBeastDomitianus, true) || Enemy.HasInMonstersZone(_CardId.PatricianOfDarkness) ||
                Enemy.HasInMonstersZone(_CardId.DictatorOfD, true) && Enemy.GetMonsters().Any(card => card.HasSetcode(_Setcode.BlueEyes));
            if (attackHighestMonster)
            {
                if (defender.HasPosition(CardPosition.FaceDown))
                    return false;
                if (Enemy.GetMonsters().Any(card => card.IsFaceup() && card.Attack > defender.Attack))
                    return false;
            }

            if (Enemy.HasInSpellZone(_CardId.SpiralDischarge, true) && Enemy.HasInMonstersZone(_CardId.GaiaTheDragonChampion) && !defender.IsCode(_CardId.GaiaTheDragonChampion))
                return false;

            if (Enemy.HasInSpellZone(_CardId.CrusadiaVanguard, true) && Enemy.GetMonsters().Any(card => card.HasSetcode(_Setcode.Crusadia) && card.HasType(CardType.Link)) && !defender.HasType(CardType.Link))
                return false;

            if (defender.IsCode(_CardId.RescueACEHydrant) && !defender.IsDisabled() && Enemy.GetMonsters().Any(monster => monster.HasSetcode(_Setcode.RescueACE) && !monster.IsCode(_CardId.RescueACEHydrant)))
                return false;

            if (Enemy.HasInSpellZone(_CardId.BarrierOfTheVoicelessVoice, true) && Enemy.HasInMonstersZone(_CardId.LoThePrayersOfTheVoicelessVoice, faceUp: true)
                && Enemy.GetMonsters().Any(card => card.HasType(CardType.Ritual) && card.IsFaceup()) && !defender.HasType(CardType.Ritual))
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
        /// Called when the AI has to select one or more cards.
        /// </summary>
        /// <param name="cards">List of available cards.</param>
        /// <param name="min">Minimal quantity.</param>
        /// <param name="max">Maximal quantity.</param>
        /// <param name="hint">The hint message of the select.</param>
        /// <param name="cancelable">True if you can return an empty list.</param>
        /// <returns>A new list containing the selected cards.</returns>
        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, int hint, bool cancelable)
        {
            // wordaround for Dogmatika Alba Zoa
            int albaZoaCount = Bot.ExtraDeck.Count / 2;
            if (!cancelable && min == albaZoaCount && max == albaZoaCount
                && Duel.Player == 1 && (Duel.Phase == DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2) && cards.All(card =>
                card.Controller == 0 && (card.Location == CardLocation.Hand || card.Location == CardLocation.Extra)))
            {
                Logger.DebugWriteLine("Dogmatika Alba Zoa solved");
                List<ClientCard> extraDeck = new List<ClientCard>(Bot.ExtraDeck);
                int shuffleCount = extraDeck.Count;
                while (shuffleCount-- > 1)
                {
                    int index = Program.Rand.Next(extraDeck.Count);
                    ClientCard tempCard = extraDeck[shuffleCount];
                    extraDeck[shuffleCount] = extraDeck[index];
                    extraDeck[index] = tempCard;
                }

                return Util.CheckSelectCount(extraDeck, cards, min, max);
            }

            return null;
        }

        public override void OnReceivingAnnouce(int player, int data)
        {
            if (player == 1 && data == Util.GetStringId(_CardId.LightningStorm, 0) || data == Util.GetStringId(_CardId.LightningStorm, 1))
            {
                lightningStormOption = data - Util.GetStringId(_CardId.LightningStorm, 0);
            }

            base.OnReceivingAnnouce(player, data);
        }

        public override void OnChainEnd()
        {
            lightningStormOption = -1;
            base.OnChainEnd();
        }

        /// <summary>
        /// Reset variables for new turn.
        /// </summary>
        public override void OnNewTurn()
        {
            if (Duel.Turn <= 1) calledbytheGraveIdCountMap.Clear();
            List<int> keyList = calledbytheGraveIdCountMap.Keys.ToList();
            foreach (int dic in keyList)
            {
                if (calledbytheGraveIdCountMap[dic] > 0)
                {
                    calledbytheGraveIdCountMap[dic] -= 1;
                }
            }
            crossoutDesignatorIdList.Clear();

            base.OnNewTurn();
        }

        public override void OnMove(ClientCard card, int previousControler, int previousLocation, int currentControler, int currentLocation)
        {
            if (card != null)
            {
                ClientCard currentSolvingChain = Duel.GetCurrentSolvingChainCard();
                if (currentSolvingChain != null && currentLocation == (int)CardLocation.Removed)
                {
                    int originId = card.Id;
                    if (card.Data != null)
                    {
                        if (card.Data.Alias > 0) originId = card.Data.Alias;
                        else originId = card.Id;
                    }
                    if (currentSolvingChain.IsCode(_CardId.CalledByTheGrave))
                    {
                        calledbytheGraveIdCountMap[originId] = 2;
                    }
                    if (currentSolvingChain.IsCode(_CardId.CrossoutDesignator))
                    {
                        crossoutDesignatorIdList.Add(originId);
                    }
                }
            }
            base.OnMove(card, previousControler, previousLocation, currentControler, currentLocation);
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
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            return Duel.Player == 1;
        }
        /// <summary>
        /// Always disable opponent's effect except some cards like UpstartGoblin
        /// </summary>
        protected bool DefaultAshBlossomAndJoyousSpring()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            int[] ignoreList = {
                _CardId.MacroCosmos,
                _CardId.UpstartGoblin,
                _CardId.CyberEmergency,
                _CardId.TheAgentOfCreationVenus
            };
            if (Util.GetLastChainCard().IsCode(ignoreList))
                return false;
            if (Util.GetLastChainCard().HasSetcode(_Setcode.Danger) && Util.GetLastChainCard().Location == CardLocation.Hand) // Danger! archtype hand effect
                return false;
            return Duel.LastChainPlayer == 1;
        }
        /// <summary>
        /// Always activate unless the activating card is disabled
        /// </summary>
        protected bool DefaultGhostOgreAndSnowRabbit()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            if (Util.GetLastChainCard() != null && Util.GetLastChainCard().IsDisabled())
                return false;
            return DefaultTrap();
        }
        /// <summary>
        /// Always disable opponent's effect
        /// </summary>
        protected bool DefaultGhostBelleAndHauntedMansion()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            return DefaultTrap();
        }
        /// <summary>
        /// Same as DefaultBreakthroughSkill
        /// </summary>
        protected bool DefaultEffectVeiler()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            ClientCard LastChainCard = Util.GetLastChainCard();
            if (LastChainCard != null && (LastChainCard.IsCode(_CardId.GalaxySoldier) && Enemy.Hand.Count >= 3
                                    || LastChainCard.IsCode(_CardId.EffectVeiler, _CardId.InfiniteImpermanence)))
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
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            // TODO: disable s & t
            ClientCard LastChainCard = Util.GetLastChainCard();
            if (LastChainCard != null && (LastChainCard.IsCode(_CardId.GalaxySoldier) && Enemy.Hand.Count >= 3
                                    || LastChainCard.IsCode(_CardId.EffectVeiler, _CardId.InfiniteImpermanence)))
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
            return !Util.IsChainTargetOnly(Card) && !(Duel.Player == 0 && Duel.LastChainPlayer == -1) && !DefaultOnlyHorusSpSummoning() && DefaultTrap();
        }

        /// <summary>
        /// Activate only except we summon monsters.
        /// </summary>
        protected bool DefaultSolemnWarning()
        {
            return (Bot.LifePoints > 2000) && !(Duel.Player == 0 && Duel.LastChainPlayer == -1) && !DefaultOnlyHorusSpSummoning() && DefaultTrap();
        }

        /// <summary>
        /// Activate only except we summon monsters.
        /// </summary>
        protected bool DefaultSolemnStrike()
        {
            return (Bot.LifePoints > 1500) && !(Duel.Player == 0 && Duel.LastChainPlayer == -1) && !DefaultOnlyHorusSpSummoning() && DefaultTrap();
        }

        /// <summary>
        /// Check whether only Horus monster is special summoning.
        /// If returning true, should not negate the special summon since it can be special summoned again.
        /// </summary>
        /// <returns></returns>
        protected bool DefaultOnlyHorusSpSummoning()
        {
            if (Duel.SummoningCards.Count != 0)
            {
                bool notOnlyHorusFlag = false;
                foreach (ClientCard card in Duel.SummoningCards)
                {
                    if (!card.HasSetcode(_Setcode.Horus) || card.LastLocation != CardLocation.Grave)
                    {
                        notOnlyHorusFlag = true;
                        break;
                    }
                }
                return !notOnlyHorusFlag;
            }
            return false;
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
            if (Card.IsMonsterInvincible())
                return Card.IsDefense();

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
            return (Bot.HasInSpellZone(_CardId.ImperialOrder, true, true) || Enemy.HasInSpellZone(_CardId.ImperialOrder, true)) && !Util.ChainContainsCard(_CardId.ImperialOrder)
                || DefaultCheckWhetherCardIsNegated(Card);
        }

        /// <summary>
        /// If trap will be negated
        /// </summary>
        protected bool DefaultTrapWillBeNegated()
        {
            return (Bot.HasInSpellZone(_CardId.RoyalDecreel, true, true) || Enemy.HasInSpellZone(_CardId.RoyalDecreel, true)) && !Util.ChainContainsCard(_CardId.RoyalDecreel)
                || DefaultCheckWhetherCardIsNegated(Card);
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
                _CardId.TopologicTrisbaena,
                _CardId.EvenlyMatched,
                _CardId.DivineArsenalAAZEUS_SkyThunder
            };
            int[] destroyAllMonsterList =
            {
                _CardId.DarkHole,
                _CardId.InterruptedKaijuSlumber
            };
            int[] destroyAllOpponentMonsterList =
            {
                _CardId.Raigeki
            };
            int[] destroyAllOpponentSpellList =
            {
                _CardId.HarpiesFeatherDuster,
                _CardId.DarkMagicAttack
            };

            if (Util.ChainContainsCard(destroyAllList)) return true;
            if (Enemy.HasInSpellZone(destroyAllOpponentSpellList, true) && Card.Location == CardLocation.SpellZone) return true;
            if (Util.ChainContainsCard(destroyAllMonsterList) && Card.Location == CardLocation.MonsterZone) return true;
            if (Duel.CurrentChain.Any(c => c.Controller == 1 && c.IsCode(destroyAllOpponentMonsterList)) && Card.Location == CardLocation.MonsterZone) return true;
            if (lightningStormOption == 0 && Card.Location == CardLocation.MonsterZone && Card.IsAttack()) return true;
            if (lightningStormOption == 1 && Card.Location == CardLocation.SpellZone) return true;
            // TODO: ChainContainsCard(id, player)
            return false;
        }
        /// <summary>
        /// Chain enemy activation or summon.
        /// </summary>
        protected bool DefaultTrap()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
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
                    if (!monster.HasType(CardType.Xyz | CardType.Link))
                    {
                        if (monster.HasType(CardType.Tuner)) tuner = true;
                        else nontuner = true;
                        if (!monster.HasType(CardType.Token)) levels[monster.Level] = levels[monster.Level] + 1;
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
            if (Duel.LastChainPlayer == 1 && lastchaincard != null && !lastchaincard.IsDisabled()
                && (lastchaincard.HasType(CardType.Spell | CardType.Trap) || lastchaincard.Location == CardLocation.MonsterZone))
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
                if (DefaultCheckWhetherCardIsNegated(Card)) return false;
                return Bot.BattlingMonster.IsAttack() &&
                    ((Bot.BattlingMonster.Attack < Enemy.BattlingMonster.Attack) || Bot.BattlingMonster.Attack >= Enemy.LifePoints
                    || ((Bot.BattlingMonster.Attack < Enemy.BattlingMonster.Defense) && (Bot.BattlingMonster.Attack + Enemy.BattlingMonster.Attack > Enemy.BattlingMonster.Defense)));
            }

            return Util.IsTurn1OrMain2();
        }

        /// <summary>
        /// Always activate
        /// </summary>
        protected bool DefaultVaylantzWorld_ShinraBansho()
        {
            if (DefaultSpellWillBeNegated()) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Select enemy's best monster
        /// </summary>
        protected bool DefaultVaylantzWorld_KonigWissen()
        {
            if (DefaultSpellWillBeNegated()) {
                return false;
            }

            List<ClientCard> monsters = Enemy.GetMonsters();
            if (monsters.Count == 0) {
                return false;
            }

            List<ClientCard> targetList = new List<ClientCard>();
            List<ClientCard> floodgateCards = monsters
                .Where(card => card?.Data != null && card.IsFloodgate() && card.IsFaceup() && !card.IsShouldNotBeTarget())
                .OrderByDescending(card => card.Attack).ToList();
            List<ClientCard> dangerousCards = monsters
                .Where(card => card?.Data != null && card.IsMonsterDangerous() && card.IsFaceup() && !card.IsShouldNotBeTarget())
                .OrderByDescending(card => card.Attack).ToList();
            List<ClientCard> attackOrderedCards = monsters
                .Where(card => card?.Data != null && card.HasType(CardType.Monster) && card.IsFaceup() && card.IsShouldNotBeTarget())
                .OrderByDescending(card => card.Attack).ToList();

            targetList.AddRange(floodgateCards);
            targetList.AddRange(dangerousCards);
            targetList.AddRange(attackOrderedCards);

            if (targetList?.Count > 0)
            {
                AI.SelectCard(targetList);
                return true;
            }

            return false;
        }

        protected bool DefaultCheckWhetherCardIsNegated(ClientCard card)
        {
            if (card == null) return true;
            if (card.Data == null) return card.IsDisabled();
            int originId = card.Data.Alias;
            if (originId == 0) originId = card.Data.Id;
            return crossoutDesignatorIdList.Contains(originId)
                || (calledbytheGraveIdCountMap.ContainsKey(originId) && calledbytheGraveIdCountMap[originId] > 0)
                || (card.IsDisabled() && ((int)card.Location & (int)CardLocation.Onfield) > 0);
        }
        
        protected bool DefaultCheckWhetherCardIdIsNegated(int cardId)
        {
            return crossoutDesignatorIdList.Contains(cardId)
                || (calledbytheGraveIdCountMap.ContainsKey(cardId) && calledbytheGraveIdCountMap[cardId] > 0);
        }

        protected int GetCalledbytheGraveIdCount(int cardId)
        {
            if (!calledbytheGraveIdCountMap.ContainsKey(cardId)) return 0;
            return calledbytheGraveIdCountMap[cardId];
        }


        protected virtual bool DefaultSetForDiabellze()
        {
            if (Card == null) return false;
            if (Card.Id == _CardId.PotOfExtravagance) return false;
            if (Enemy.HasInMonstersZone(_CardId.DiabellzeOfTheOriginalSin, true, faceUp: true) && Card.HasType(CardType.Spell) && !Card.HasType(CardType.QuickPlay))
            {
                if (Bot.SpellZone.Any(c => c != null && Duel.MainPhase.ActivableCards.Contains(c) && c.HasType(CardType.Spell) && !Card.HasType(CardType.QuickPlay) && c.IsFacedown()))
                {
                    return false;
                }
                foreach (CardExecutor exec in Executors)
                {
                    if (exec.Type == ExecutorType.Activate && exec.CardId == Card.Id)
                    {
                        if (exec.Func == null || exec.Func())
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
