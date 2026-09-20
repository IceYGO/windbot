using YGOSharp.OCGWrapper;
using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using System.Linq;
using System;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("Elfnote", "AI_Elfnote")]
    public class ElfnoteExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int MixedHellGod = 70488851;
            public const int GreatRighteousThief = 24203749;
            public const int WhitePendulum = 10266279;
            public const int ElvenotesRed = 13597785;
            public const int ElvenotesBlue = 59581480;
            public const int ElvenotesYellow = 85976588;
            public const int ElvenotesWind = 56651978;
            public const int MediusTheInnocent = 97556336;
            public const int JailChicken = 12375297;
            public const int JailGodGate = 25661743;
            public const int InnocentArt = 37279096;
            public const int GreenField = 64491754;
            public const int RedField = 24092792;

            public const int ChaosAngel = 22850702;
            public const int White10 = 5559570;
            public const int BaronneDeFleur = 84815190;
            public const int AssaultBlackwing = 31114334;
            public const int ThousandSpearDragon = 65424481;
            public const int AncientFishDragon = 87188910;
            public const int CrystalWing = 50954680;
            public const int PSYFramelordOmega = 74586817;
            public const int AccelSynchroStardust = 30983281;
            public const int StardustDragon = 44508094;
            public const int HelldiveBomber = 66122213;
            public const int BlackRoseDragon = 73580471;
            public const int FormulaAthleteLightning = 33158448;
            public const int White7 = 42302563;
            public const int SuperLibrarian = 90953320;
        }

        public ElfnoteExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, _CardId.EffectVeiler, EffectVeilerActivate);
            AddExecutor(ExecutorType.Activate, _CardId.AshBlossom, AshBlossomActivate);
            AddExecutor(ExecutorType.Activate, CardId.BaronneDeFleur, BaronneNegateActivate);
            AddExecutor(ExecutorType.Activate, CardId.CrystalWing, CrystalWingActivate);
            AddExecutor(ExecutorType.Activate, CardId.FormulaAthleteLightning, FormulaAthleteActivate);
            AddExecutor(ExecutorType.Activate, CardId.GreatRighteousThief, GreatRighteousThiefNegateActivate);
            AddExecutor(ExecutorType.Activate, CardId.AccelSynchroStardust, AccelStardustActivate);
            AddExecutor(ExecutorType.Activate, CardId.StardustDragon, StardustDragonActivate);
            AddExecutor(ExecutorType.Activate, CardId.PSYFramelordOmega, PSYOmegaActivate);
            AddExecutor(ExecutorType.Activate, CardId.ChaosAngel, ChaosAngelActivate);
            AddExecutor(ExecutorType.Activate, CardId.RedField, RedFieldActivate);
            AddExecutor(ExecutorType.Activate, CardId.White10, White10Activate);
            AddExecutor(ExecutorType.Activate, CardId.White7, White7Activate);
            AddExecutor(ExecutorType.Activate, CardId.BaronneDeFleur, BaronneDestroyActivate);

            AddExecutor(ExecutorType.Activate, _CardId.MaxxC, MaxxCActivate);
            AddExecutor(ExecutorType.Activate, _CardId.MulcharmyFuwalos, MulcharmyFuwalosActivate);
            AddExecutor(ExecutorType.Activate, _CardId.MulcharmyPurulia, MulcharmyPuruliaActivate);
            AddExecutor(ExecutorType.Activate, _CardId.LockBird, LockBirdActivate);

            // Tribute opponent monsters before our own Activate / SpSummon, so they cannot negate the combo.
            AddExecutor(ExecutorType.Summon, CardId.GreatRighteousThief, GreatRighteousThiefSummon);

            AddExecutor(ExecutorType.Activate, CardId.InnocentArt, InnocentArtActivate);
            AddExecutor(ExecutorType.Activate, CardId.JailGodGate, JailGodGateActivate);
            AddExecutor(ExecutorType.Activate, CardId.MixedHellGod, MixedHellGodActivate);
            // NS chicken first so a later hand-SS 6-star can be +3 synchro'd if the opponent targets it.
            AddExecutor(ExecutorType.Summon, CardId.JailChicken, JailChickenSummonForWhite10Dodge);
            AddExecutor(ExecutorType.SpSummon, CardId.ElvenotesRed, ElvenotesRedSpSummon);
            AddExecutor(ExecutorType.Activate, CardId.ElvenotesRed, ElvenotesRedActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.ElvenotesBlue, ElvenotesBlueSpSummon);
            AddExecutor(ExecutorType.Activate, CardId.ElvenotesBlue, ElvenotesBlueActivate);
            AddExecutor(ExecutorType.Activate, CardId.GreenField, GreenFieldActivateFromHand);
            AddExecutor(ExecutorType.SpSummon, CardId.ElvenotesYellow, ElvenotesYellowSpSummon);
            AddExecutor(ExecutorType.Activate, CardId.ElvenotesYellow, ElvenotesYellowActivate);
            AddExecutor(ExecutorType.Activate, CardId.ElvenotesWind, ElvenotesWindActivate);
            AddExecutor(ExecutorType.Activate, CardId.MediusTheInnocent, MediusActivate);
            AddExecutor(ExecutorType.Activate, CardId.WhitePendulum, WhitePendulumActivate);
            // Same-window triggers: White P draw → Chicken GY search → White P recycle
            // → Black Rose ① → Wind GY recycle (Wind last so a negate hits Wind, not Black Rose).
            AddExecutor(ExecutorType.Activate, CardId.WhitePendulum, WhitePendulumDrawActivate);
            AddExecutor(ExecutorType.Activate, CardId.JailChicken, JailChickenGySearchActivate);
            AddExecutor(ExecutorType.Activate, CardId.WhitePendulum, WhitePendulumRecycleActivate);
            AddExecutor(ExecutorType.Activate, CardId.BlackRoseDragon, BlackRoseActivate);
            AddExecutor(ExecutorType.Activate, CardId.ElvenotesWind, ElvenotesWindGyRecycleActivate);

            // Going-second wipe before White 7. Turn 1 still skips Black Rose and makes White 7.
            AddExecutor(ExecutorType.SpSummon, CardId.BlackRoseDragon, BlackRoseSpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.White7, White7SpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.SuperLibrarian, SuperLibrarianSpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.AccelSynchroStardust, AccelStardustSpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.CrystalWing, CrystalWingSpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.White10, White10SpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.BaronneDeFleur, BaronneSpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.FormulaAthleteLightning, FormulaAthleteSpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.PSYFramelordOmega, PSYOmegaSpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.AncientFishDragon, AncientFishSpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.ThousandSpearDragon, ThousandSpearSpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.AssaultBlackwing, AssaultBlackwingSpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.HelldiveBomber, HelldiveBomberSpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.ChaosAngel, ChaosAngelSpSummon);

            AddExecutor(ExecutorType.Summon, CardId.MediusTheInnocent, MediusSummon);
            AddExecutor(ExecutorType.Summon, CardId.JailChicken, JailChickenSummon);
            AddExecutor(ExecutorType.Summon, _CardId.EffectVeiler, EffectVeilerSummon);

            AddExecutor(ExecutorType.Activate, CardId.GreenField, GreenFieldActivate);
            AddExecutor(ExecutorType.Activate, CardId.JailChicken, JailChickenActivate);
            AddExecutor(ExecutorType.Activate, CardId.ThousandSpearDragon, ThousandSpearActivate);
            AddExecutor(ExecutorType.Activate, CardId.HelldiveBomber, HelldiveBomberActivate);
            AddExecutor(ExecutorType.Activate, CardId.AncientFishDragon, AncientFishActivate);
            AddExecutor(ExecutorType.Activate, CardId.GreatRighteousThief, GreatRighteousThiefBattleActivate);
            AddExecutor(ExecutorType.Activate, CardId.SuperLibrarian, SuperLibrarianActivate);

            AddExecutor(ExecutorType.Repos, ElfnoteMonsterRepos);
            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
            AddExecutor(ExecutorType.SpellSet, SpellSetCheck);
        }

        const int SetcodeElvenotes = 0x1d8;
        const int SetcodeJailGod = 0x1ce;
        const int SetcodeTimeLord = 0x4a;
        const int SetcodeOrcust = 0x11b;
        const int SetcodePhantomKnight = 0xdb;
        const int SetcodeHorus = 0x19d;
        const int HintTimingMainEnd = 0x4;

        List<int> NotToNegateIdList = new List<int>
        {
            58699500, 20343502, 25451383, 19403423
        };
        List<int> NotToDestroySpellTrap = new List<int> { 50005218, 6767771 };
        List<int> targetNegateIdList = new List<int>
        {
            _CardId.EffectVeiler, _CardId.InfiniteImpermanence, _CardId.GhostMournerMoonlitChill, _CardId.BreakthroughSkill, 74003290, 67037924,
            9753964, 66192538, 23204029, 73445448, 35103106, 30286474, 45002991, 5795980, 38511382, 53742162, 30430448
        };

        int summonCount = 1;
        // Innocent Art search is once per copy. Track it for cost / Gate wait only; OnMove
        // resets when our copy relocates. InnocentArtActivate must not gate on this flag.
        bool artSearched = false;
        bool white10GyWhite7LineThisChain = false;
        bool white10TwoZoneWhite7ChickenThisChain = false;
        bool white10PickedGyChickenThisChain = false;
        List<int> white10EnemyPickedCodesThisChain = new List<int>();
        // White 10 4-body Wind line: Wind ② later fills this missing RBY from deck.
        // Survives OnChainEnd because Wind ② is a new chain after 解体.
        int white10EnemyWindMissingDeckId = 0;
        // Opening missing-place White 10: set on chicken +3, kept across OnChainEnd so ②
        // can still force GY chicken + deck Yellow/Blue after the 6-star left the field.
        bool white10MissingPlaceLineThisChain = false;
        // Red Field negate is chosen at resolution. Keep the intended target here, not in
        // currentNegateCardList, so Ancient Fish can skip it without Red Field YesNo/Disable skipping it too.
        ClientCard redFieldPendingNegateTarget = null;
        // Veiler: remember the intended target at activation, commit to currentNegateCardList only after select.
        ClientCard preferNegateCard = null;
        // Rule 1: this-turn NS chicken stays off z2 so Red/Blue/Yellow can take center.
        bool nsChickenOffCenterForWhite10 = false;
        // Rule 2: this chain, chicken +3 is eating a targeted center Elvenotes.
        bool chickenDodgeSynchroThisChain = false;
        int chickenDodgeSynchroCardId = 0;
        // Accel ② picked Formula/White7: synchro materials must be chicken+Wind from the live candidate list.
        bool accel2PreferWindChicken = false;
        // Jail Gate spell resolved in Main Phase (not negated). Attack lock; activation still uses activatedCardIdList.
        bool jailGateResolvedThisTurn = false;

        List<ClientCard> currentNegateCardList = new List<ClientCard>();
        List<ClientCard> currentDestroyCardList = new List<ClientCard>();
        List<int> activatedCardIdList = new List<int>();
        // Red/Blue/Yellow inherent hand SS is once per name (oath). Not a full this-turn SS log.
        List<int> spSummonedCardIdList = new List<int>();
        List<ClientCard> enemyPlaceThisTurn = new List<ClientCard>();
        // Medius SS'd by its own GY ② leaves the field to banish unless Iron Wall / Lancea.
        List<ClientCard> mediusSsByOwnGyEffect = new List<ClientCard>();

        #region Helpers

        public void ElfnoteLog(string message)
        {
            Logger.DebugWriteLine("[Elfnote] " + message);
        }

        public string CardStr(ClientCard card)
        {
            if (card == null) return "null";
            string name = card.Name ?? card.Id.ToString();
            return name + "@" + card.Location.ToString() + "#" + card.Sequence;
        }

        /// <summary>
        /// Same on-field card: live zone object vs chain snapshot / select candidate.
        /// ClientCard.Equals is reference-only, so Id+controller+location+sequence.
        /// </summary>
        public bool SameFieldCard(ClientCard a, ClientCard b)
        {
            if (a == null || b == null) return false;
            if (a == b) return true;
            return a.Id == b.Id && a.Controller == b.Controller
                && a.Location == b.Location && a.Sequence == b.Sequence;
        }

        public bool CheckCanBeTargeted(ClientCard card, bool canBeTarget, CardType selfType)
        {
            if (card == null) return true;
            if (canBeTarget)
            {
                if (card.IsShouldNotBeTarget()) return false;
                if (((int)selfType & (int)CardType.Monster) > 0 && card.IsShouldNotBeMonsterTarget()) return false;
                if (((int)selfType & (int)CardType.Spell) > 0 && card.IsShouldNotBeSpellTrapTarget()) return false;
                if (((int)selfType & (int)CardType.Trap) > 0
                    && (card.IsShouldNotBeSpellTrapTarget() && !card.IsDisabled())) return false;
            }
            return true;
        }

        public bool CheckWhetherNegated(bool disablecheck = true, bool toFieldCheck = false, CardType type = 0, bool ignore41 = false)
        {
            bool isMonster = type == 0 && Card.IsMonster();
            isMonster |= ((int)type & (int)CardType.Monster) != 0;
            bool isSpellOrTrap = type == 0 && (Card.IsSpell() || Card.IsTrap());
            isSpellOrTrap |= (((int)type & (int)CardType.Spell) != 0) || (((int)type & (int)CardType.Trap) != 0);
            bool isCounter = ((int)type & (int)CardType.Counter) != 0;
            if (isSpellOrTrap && toFieldCheck && CheckSpellWillBeNegate(isCounter))
                return true;
            if (DefaultCheckWhetherCardIsNegated(Card)) return true;
            if (isMonster && (toFieldCheck || Card.Location == CardLocation.MonsterZone))
            {
                if (!ignore41 && ((toFieldCheck && (((int)type & (int)CardType.Link) == 0)) || Card.IsDefense()))
                {
                    if (DefaultCheckWhetherNumber41IsActive()) return true;
                }
                if (Enemy.HasInSpellZone(_CardId.SkillDrain, true, true)) return true;
            }
            if (disablecheck) return (Card.Location == CardLocation.MonsterZone || Card.Location == CardLocation.SpellZone) && Card.IsDisabled() && Card.IsFaceup();
            return false;
        }

        public bool CheckSpellWillBeNegate(bool isCounter = false, ClientCard target = null)
        {
            if (target == null) target = Card;
            if (target.Location != CardLocation.SpellZone && target.Location != CardLocation.Hand) return false;
            if (Enemy.HasInMonstersZone(_CardId.NaturalExterio, true) && !isCounter) return true;
            if (target.IsSpell())
            {
                if (Enemy.HasInMonstersZone(_CardId.NaturiaBeast, true)) return true;
                if (Enemy.HasInSpellZone(_CardId.ImperialOrder, true) || Bot.HasInSpellZone(_CardId.ImperialOrder, true)) return true;
                if (Enemy.HasInMonstersZone(_CardId.SwordsmanLV7, true) || Bot.HasInMonstersZone(_CardId.SwordsmanLV7, true)) return true;
            }
            if (target.IsTrap() && (Enemy.HasInSpellZone(_CardId.RoyalDecreel, true) || Bot.HasInSpellZone(_CardId.RoyalDecreel, true))) return true;
            return DefaultCheckWhetherSpellActivateWillBeNegated(target);
        }

        public bool CheckLastChainShouldNegated()
        {
            ClientCard lastcard = Util.GetLastChainCard();
            if (lastcard == null || lastcard.Controller != 1) return false;
            return CheckCardShouldNegate(lastcard);
        }

        public bool CheckCardShouldNegate(ClientCard card)
        {
            if (card == null) return false;
            if (currentNegateCardList.Contains(card)) return false;
            if (card.IsMonster() && card.HasSetcode(SetcodeTimeLord) && Duel.Phase == DuelPhase.Standby) return false;
            if (NotToNegateIdList.Contains(card.Id)) return false;
            if (card.HasSetcode(_Setcode.Danger) && card.Location == CardLocation.Hand) return false;
            if (card.IsMonster() && card.Location == CardLocation.MonsterZone && card.HasPosition(CardPosition.Defence))
            {
                if (DefaultCheckWhetherNumber41IsActive()) return false;
            }
            if (DefaultCheckWhetherCardIsNegated(card)) return false;
            if (Duel.Player == 1 && card.IsCode(_CardId.MulcharmyPurulia, _CardId.MulcharmyFuwalos, _CardId.MulcharmyNyalus)) return false;
            if (card.IsDisabled()) return false;
            if (CheckAtAdvantage() && Duel.Player == 0 && card.IsCode(_CardId.MulcharmyPurulia, _CardId.MulcharmyFuwalos, _CardId.MaxxC))
                return false;
            return true;
        }

        public bool CheckAtAdvantage()
        {
            if (GetProblematicEnemyMonster() != null) return false;
            if (!(Duel.Player == 0 || Bot.GetMonsterCount() > 0)) return false;
            // G stop: empty opponent field is not enough; need an Elfnote stop board (1/2/3).
            return CheckHasStopBoard();
        }

        public bool CheckHasStopBoard()
        {
            bool extraWhite10 = Bot.HasInExtra(CardId.White10);
            bool windCanJumpCenter = CanWindJumpToCenter();
            if (windCanJumpCenter && Bot.HasInDeck(CardId.JailChicken) && extraWhite10)
                return true;
            if (HasChickenAndLevel6OnField() && extraWhite10)
                return true;
            bool hasSynchro = Bot.GetMonsters().Any(c => c != null && c.HasType(CardType.Synchro) && c.IsFaceup());
            bool gyWhite = Bot.HasInGraveyard(CardId.White7) || Bot.HasInGraveyard(CardId.White10);
            bool hasRedField = Bot.HasInSpellZone(CardId.RedField, true, true) || Bot.HasInHand(CardId.RedField) || Bot.HasInDeck(CardId.RedField);
            if ((hasSynchro || gyWhite) && hasRedField)
                return true;
            return false;
        }

        public bool CheckShouldNoMoreSpSummon()
        {
            if (CheckAtAdvantage() && enemyResolvedEffectIdList.Contains(_CardId.MaxxC) && DefaultCheckWhetherEnemyCanDraw())
                return true;
            return false;
        }

        public bool CheckShouldNoMoreSpSummon(CardLocation loc)
        {
            if (CheckShouldNoMoreSpSummon()) return true;
            if (!DefaultCheckWhetherEnemyCanDraw() || (Duel.Turn > 1 && Duel.Phase < DuelPhase.Main2)) return false;
            if (!CheckAtAdvantage() && Duel.Player == 0 && Duel.Turn > 1)
            {
                if (GetProblematicEnemyCardList(false, false, 0).Count > 0)
                    return false;
            }
            if (enemyResolvedEffectIdList.Contains(_CardId.MulcharmyPurulia) && (loc & CardLocation.Hand) != 0) return true;
            if (enemyResolvedEffectIdList.Contains(_CardId.MulcharmyFuwalos) && (loc & (CardLocation.Deck | CardLocation.Extra)) != 0) return true;
            if (enemyResolvedEffectIdList.Contains(_CardId.MulcharmyNyalus) && (loc & (CardLocation.Grave | CardLocation.Removed)) != 0) return true;
            return false;
        }

        public bool CheckWhetherCanSummon()
        {
            return Duel.Player == 0 && Duel.Phase < DuelPhase.End && summonCount > 0;
        }

        /// <summary>
        /// First combo turn: going-first turn 1, or going-second first turn after opponent
        /// passed (empty field). Use going-first expansion (6+1 White 7 before chicken +3).
        /// Going second can still attack; do not reuse this for Battle / Black Rose / Assault.
        /// </summary>
        public bool IsOpeningComboTurn()
        {
            if (Duel.Player != 0) return false;
            if (Duel.Turn == 1) return true;
            if (Duel.Turn == 2 && !Duel.IsFirst
                && Enemy.GetMonsterCount() == 0 && Enemy.GetSpellCount() == 0)
                return true;
            return false;
        }

        /// <summary>
        /// EFFECT_CANNOT_REMOVE: Artifact Lancea until end of the turn it resolved,
        /// Imperial Iron Wall while face-up and not disabled.
        /// </summary>
        public bool BanishRedirectPrevented()
        {
            return resolvedEffectIdList.Contains(_CardId.ArtifactLancea)
                || Bot.HasInSpellZone(_CardId.ImperialIronWall, true, true)
                || Enemy.HasInSpellZone(_CardId.ImperialIronWall, true, true);
        }

        /// <summary>
        /// Wrap Default GY-redirect checks, then Medius GY ② leave-field redirect.
        /// Iron Wall / Lancea are checked first so even that Medius can still go to the GY.
        /// </summary>
        public bool CheckWhetherBotWillBeBanished(ClientCard card = null)
        {
            if (BanishRedirectPrevented()) return false;
            if (DefaultCheckWhetherBotWillBeBanished(card)) return true;
            if (card != null && mediusSsByOwnGyEffect.Contains(card)) return true;
            return false;
        }

        public bool CheckWhetherBotWillBeBanished(CardType type, CardLocation location)
        {
            if (BanishRedirectPrevented()) return false;
            return DefaultCheckWhetherBotWillBeBanished(type, location);
        }

        public bool MonstersLeavingFieldWillBeBanished()
        {
            return CheckWhetherBotWillBeBanished(CardType.Monster, CardLocation.MonsterZone);
        }

        /// <summary>
        /// Own turn, chicken + 6-star on field, monsters sent to GY would be banished:
        /// do not make White 7 / Crystal / Accel / White 10.
        /// </summary>
        public bool ShouldRerouteChickenSixUnderBanish()
        {
            if (Duel.Player != 0) return false;
            if (!HasChickenAndLevel6OnField()) return false;
            return MonstersLeavingFieldWillBeBanished();
        }

        /// <summary>
        /// Opening combo turn, opponent resolved Fuwalos only (no Purulia / Maxx C) and can still draw.
        /// Short Baronne / Accel lines instead of hard-stopping all Extra/Deck SS.
        /// </summary>
        public bool IsFuwalosOnlyOpeningCompromise()
        {
            if (!IsOpeningComboTurn()) return false;
            if (!enemyResolvedEffectIdList.Contains(_CardId.MulcharmyFuwalos)) return false;
            if (enemyResolvedEffectIdList.Contains(_CardId.MulcharmyPurulia)) return false;
            if (enemyResolvedEffectIdList.Contains(_CardId.MaxxC)) return false;
            if (!DefaultCheckWhetherEnemyCanDraw()) return false;
            return true;
        }

        /// <summary>
        /// Non-whitelist Extra synchros stay off during the Fuwalos short line (Turn 2 opening
        /// does not Extra-gate, so White 7 / Librarian / Crystal must skip here).
        /// </summary>
        public bool ShouldSkipExtraSynchroForFuwalosCompromise()
        {
            return IsFuwalosOnlyOpeningCompromise();
        }

        /// <summary>
        /// Chicken +3 on Medius (or self in center with leftover Medius) makes 8 for Accel.
        /// </summary>
        public bool CanChickenPlus3Accel()
        {
            if (activatedCardIdList.Contains(CardId.JailChicken + 1)) return false;
            if (!Bot.HasInExtra(CardId.AccelSynchroStardust))
                return false;
            if (!Bot.GetMonsters().Any(c => c != null && c.IsCode(CardId.JailChicken) && c.IsFaceup()))
                return false;
            ClientCard center = GetCenterMonster();
            if (center == null) return false;
            if (center.IsCode(CardId.MediusTheInnocent) && center.Level + 3 + 1 == 8)
                return true;
            if (center.IsCode(CardId.JailChicken)
                && Bot.GetMonsters().Any(c => c != null && c != center && c.IsCode(CardId.MediusTheInnocent) && c.IsFaceup()))
                return true;
            return false;
        }

        /// <summary>
        /// After chicken +3, idle Baronne 9+1 (chicken YesNo refused so White 10 is not made).
        /// </summary>
        public bool ShouldFuwalosCompromiseIdleBaronne()
        {
            if (!IsFuwalosOnlyOpeningCompromise()) return false;
            if (!activatedCardIdList.Contains(CardId.JailChicken + 1)) return false;
            if (!Bot.HasInExtra(CardId.BaronneDeFleur))
                return false;
            return CanSynchroLevel(10);
        }

        /// <summary>
        /// After chicken +3, idle Accel 7+1 or 4+4 with Medius.
        /// </summary>
        public bool ShouldFuwalosCompromiseIdleAccel()
        {
            if (!IsFuwalosOnlyOpeningCompromise()) return false;
            if (!activatedCardIdList.Contains(CardId.JailChicken + 1)) return false;
            if (!Bot.HasInExtra(CardId.AccelSynchroStardust))
                return false;
            if (!Bot.GetMonsters().Any(c => c != null && c.IsCode(CardId.JailChicken) && c.IsFaceup()))
                return false;
            ClientCard center = GetCenterMonster();
            if (center == null) return false;
            if (center.IsCode(CardId.MediusTheInnocent))
                return true;
            if (center.IsCode(CardId.JailChicken)
                && Bot.GetMonsters().Any(c => c != null && c != center && c.IsCode(CardId.MediusTheInnocent) && c.IsFaceup()))
                return true;
            return false;
        }

        /// <summary>
        /// Opening combo: chicken GY search already used, no RBY/Wind in hand, field chicken +
        /// spent 6-star, Blue or Yellow still needs to place. +3 White 10 instead of idle 6+1.
        /// Abort if Fuwalos already resolved (Extra SS would draw).
        /// </summary>
        public bool OpeningYellowPlaceMissing()
        {
            if (Bot.HasInSpellZone(CardId.RedField, true, true)) return false;
            return Bot.HasInDeck(CardId.RedField) || Bot.HasInHand(CardId.RedField);
        }

        public bool OpeningBluePlaceMissing()
        {
            bool needGreen = Bot.HasInDeck(CardId.GreenField)
                && !Bot.HasInSpellZone(CardId.GreenField) && !Bot.HasInHand(CardId.GreenField);
            if (needGreen) return true;
            if (!Bot.HasInDeck(CardId.InnocentArt)) return false;
            if (Bot.HasInHand(CardId.MediusTheInnocent)) return false;
            if (!DefaultCheckWhetherBotCanSearch()) return false;
            return true;
        }

        public bool ShouldOpeningWhite10ForMissingPlace()
        {
            if (!IsOpeningComboTurn()) return false;
            if (IsFuwalosOnlyOpeningCompromise()) return false;
            if (enemyResolvedEffectIdList.Contains(_CardId.MulcharmyFuwalos)) return false;
            if (!activatedCardIdList.Contains(CardId.JailChicken)) return false;
            if (Bot.HasInHand(CardId.ElvenotesRed) || Bot.HasInHand(CardId.ElvenotesBlue)
                || Bot.HasInHand(CardId.ElvenotesYellow) || Bot.HasInHand(CardId.ElvenotesWind))
                return false;
            if (!HasChickenAndLevel6OnField()) return false;
            if (CenterElvenotesStillNeedsIgnition()) return false;
            if (!OpeningYellowPlaceMissing() && !OpeningBluePlaceMissing()) return false;
            if (!White10EffectStillAvailable()) return false;
            if (GetChickenPlus3SynchroLevel() != 10) return false;
            if (CheckShouldNoMoreSpSummon(CardLocation.Extra)) return false;
            if (ShouldRerouteChickenSixUnderBanish()) return false;
            return true;
        }

        /// <summary>
        /// Blue placed Green Field: send used Blue, SS Red from deck, then the Baronne line.
        /// </summary>
        public bool ShouldFuwalosGreenFieldSummonRed()
        {
            if (!IsFuwalosOnlyOpeningCompromise()) return false;
            if (activatedCardIdList.Contains(CardId.GreenField)) return false;
            if (!activatedCardIdList.Contains(CardId.ElvenotesBlue)) return false;
            if (!Bot.HasInDeck(CardId.ElvenotesRed)) return false;
            ClientCard center = GetCenterMonster();
            if (center == null || !center.IsCode(CardId.ElvenotesBlue)) return false;
            if (CheckWhetherBotWillBeBanished(center)) return false;
            return IsGreenFieldAcceptableCost(center, CardAttribute.Fire);
        }

        /// <summary>
        /// Medius opening under Fuwalos: SS chicken from deck. Gate line (GY chicken + Mixed
        /// still available) keeps adding Mixed instead.
        /// </summary>
        public bool ShouldFuwalosMediusSsChickenFromDeck()
        {
            if (!IsFuwalosOnlyOpeningCompromise()) return false;
            if (Bot.HasInGraveyard(CardId.JailChicken) && !activatedCardIdList.Contains(CardId.MixedHellGod))
            {
                if (Bot.HasInDeck(CardId.MixedHellGod) || Bot.HasInHand(CardId.MixedHellGod)
                    || Bot.HasInSpellZone(CardId.MixedHellGod))
                    return false;
            }
            return Bot.HasInDeck(CardId.JailChicken);
        }

        /// <summary>
        /// Chicken +3 on a 6-star makes 10 with chicken; Baronne is still in Extra.
        /// Hand chicken that can still Normal Summon counts (same geometry as keep-six).
        /// </summary>
        public bool CanChickenPlus3Baronne()
        {
            if (activatedCardIdList.Contains(CardId.JailChicken + 1)) return false;
            if (!Bot.HasInExtra(CardId.BaronneDeFleur))
                return false;
            ClientCard center = GetCenterMonster();
            if (center == null) return false;
            if (center.IsCode(CardId.JailChicken))
                return Bot.GetMonsters().Any(c => c != null && c != center && IsElvenotesSixStar(c) && c.IsFaceup());
            if (IsElvenotesSixStar(center) && center.Level + 3 + 1 == 10)
            {
                if (Bot.GetMonsters().Any(c => c != null && c.IsCode(CardId.JailChicken) && c.IsFaceup()))
                    return true;
                if (Bot.HasInHand(CardId.JailChicken) && CheckWhetherCanSummon()) return true;
            }
            return false;
        }

        public ClientCard GetCenterMonster()
        {
            return Bot.MonsterZone[2];
        }

        public bool IsCenterEmpty()
        {
            return Bot.MonsterZone[2] == null;
        }

        public bool IsElvenotesSixStar(ClientCard card)
        {
            if (card == null) return false;
            return card.IsCode(CardId.ElvenotesRed, CardId.ElvenotesBlue, CardId.ElvenotesYellow, CardId.ElvenotesWind);
        }

        public bool IsPendulumZoneCard(ClientCard card)
        {
            if (card == null) return false;
            if (card.Location == CardLocation.PendulumZone) return true;
            if (card.Location == CardLocation.SpellZone && (card.Sequence == 0 || card.Sequence == 4)) return true;
            return false;
        }

        public bool HasChickenAndLevel6OnField()
        {
            bool chicken = Bot.GetMonsters().Any(c => c != null && c.IsCode(CardId.JailChicken) && c.IsFaceup());
            bool six = Bot.GetMonsters().Any(c => c != null && IsElvenotesSixStar(c) && c.IsFaceup());
            return chicken && six;
        }

        public int CountFaceupChickenOnField()
        {
            return Bot.GetMonsters().Count(c => c != null && c.IsCode(CardId.JailChicken) && c.IsFaceup());
        }

        /// <summary>
        /// Keep the 6-star that chicken +3 will turn into White 10 (or Baronne under GY redirect).
        /// Chicken in center: leftover 6-star on the side. 6-star in center: chicken on field
        /// (or Normal Summonable from hand) about to +3 it. GY chicken alone cannot +3 this turn.
        /// If a hand Red/Blue/Yellow can still SS, we dump Yellow instead to free z2.
        /// </summary>
        public bool ShouldKeepSixStarForChickenPlus3()
        {
            if (activatedCardIdList.Contains(CardId.JailChicken + 1)) return false;
            if (HandHasSixStarWaitingToSs()) return false;
            // Own turn + monster GY redirect: keep the 6-star for +3 Baronne even before chicken is NS'd.
            if (Duel.Player == 0 && MonstersLeavingFieldWillBeBanished())
                return CanChickenPlus3Baronne();
            if (IsFuwalosOnlyOpeningCompromise())
                return CanChickenPlus3Baronne();
            if (!White10EffectStillAvailable())
                return false;
            ClientCard center = GetCenterMonster();
            if (center == null) return false;
            if (center.IsCode(CardId.JailChicken))
                return Bot.GetMonsters().Any(c => c != null && c != center && IsElvenotesSixStar(c) && c.IsFaceup());
            if (IsElvenotesSixStar(center) && center.Level + 3 + 1 == 10)
            {
                if (Bot.HasInMonstersZone(CardId.JailChicken)) return true;
                // Hand chicken can NS for +3; GY chicken needs revive and must not block Green Field ②.
                if (Bot.HasInHand(CardId.JailChicken) && CheckWhetherCanSummon()) return true;
            }
            return false;
        }

        public bool White7StillWantsGyChicken()
        {
            return Bot.HasInMonstersZone(CardId.White7)
                && !activatedCardIdList.Contains(CardId.White7)
                && Bot.HasInGraveyard(CardId.JailChicken);
        }

        public bool CanWindJumpToCenter()
        {
            if (!Bot.HasInHand(CardId.ElvenotesWind)) return false;
            if (activatedCardIdList.Contains(CardId.ElvenotesWind) && Bot.Hand.Count(c => c.IsCode(CardId.ElvenotesWind)) == 0)
                return false;
            ClientCard cost = GetWindCostCard();
            if (cost == null) return false;
            if (IsCenterEmpty()) return true;
            ClientCard center = GetCenterMonster();
            return center != null && center == cost;
        }

        public bool IsUsedElvenotesBody(ClientCard card)
        {
            if (card == null || !card.IsFaceup()) return false;
            if (card.IsCode(CardId.ElvenotesBlue) && activatedCardIdList.Contains(CardId.ElvenotesBlue)) return true;
            if (card.IsCode(CardId.ElvenotesRed) && !CanElvenotesRedSearch()) return true;
            if (card.IsCode(CardId.ElvenotesYellow) && activatedCardIdList.Contains(CardId.ElvenotesYellow)) return true;
            return false;
        }

        /// <summary>
        /// Red ② searches a different 0x1d8 monster from the Main Deck (not another Red).
        /// </summary>
        public bool HasElvenotesSearchTargetInDeck()
        {
            return Bot.HasInDeck(CardId.ElvenotesBlue)
                || Bot.HasInDeck(CardId.ElvenotesYellow)
                || Bot.HasInDeck(CardId.ElvenotesWind)
                || Bot.HasInDeck(CardId.JailChicken)
                || Bot.HasInDeck(CardId.WhitePendulum);
        }

        public bool CanElvenotesRedSearch()
        {
            if (activatedCardIdList.Contains(CardId.ElvenotesRed)) return false;
            if (!DefaultCheckWhetherBotCanSearch()) return false;
            return HasElvenotesSearchTargetInDeck();
        }

        public bool ShouldKeepGyChickenForMixedHellGod()
        {
            if (activatedCardIdList.Contains(CardId.MixedHellGod)) return false;
            if (!Bot.HasInGraveyard(CardId.JailChicken)) return false;
            // Mixed P option 1 needs a free main zone; otherwise Red Field may revive the chicken.
            if (CountFreeMainMonsterZones() < 1) return false;
            return Bot.HasInHand(CardId.MixedHellGod) || Bot.HasInSpellZone(CardId.MixedHellGod);
        }

        public bool HasEightSynchroOnField()
        {
            return Bot.GetMonsters().Any(c => c != null && c.IsFaceup()
                && c.IsCode(CardId.CrystalWing, CardId.AccelSynchroStardust));
        }

        /// <summary>
        /// After Crystal Wing / Accel Stardust, Yellow in hand should take center first:
        /// place Red Field, then dump Yellow to revive chicken so leftover Red/Blue can SS.
        /// Wind cannot SS Yellow from deck while it sits in hand.
        /// </summary>
        public bool ShouldSsYellowFirstAfterEightSynchro()
        {
            if (Duel.Player != 0) return false;
            if (!Bot.HasInHand(CardId.ElvenotesYellow)) return false;
            if (HasEightSynchroOnField()) return true;
            if (IsFuwalosOnlyOpeningCompromise()
                && Bot.GetMonsters().Any(c => c != null && c.IsFaceup() && c.IsCode(CardId.BaronneDeFleur)))
                return true;
            return false;
        }

        /// <summary>
        /// Red Field can still be placed (not on field, copy in deck/hand, Yellow ② unused)
        /// but is not down yet. Hand-SS Yellow first when we can follow with a tuner:
        /// unused NS plus a 1-star in hand (chicken / Veiler), or chicken already in GY.
        /// </summary>
        public bool ShouldSsYellowFirstToPlaceRedField()
        {
            if (Duel.Player != 0) return false;
            if (!Bot.HasInHand(CardId.ElvenotesYellow)) return false;
            if (spSummonedCardIdList.Contains(CardId.ElvenotesYellow)) return false;
            if (activatedCardIdList.Contains(CardId.ElvenotesYellow)) return false;
            if (Bot.HasInSpellZone(CardId.RedField, true, true)) return false;
            if (!Bot.HasInDeck(CardId.RedField) && !Bot.HasInHand(CardId.RedField)) return false;
            bool nsWithOneStar = CheckWhetherCanSummon()
                && Bot.Hand.Any(c => c != null && c.IsCode(CardId.JailChicken, _CardId.EffectVeiler));
            bool gyChicken = Bot.HasInGraveyard(CardId.JailChicken);
            return nsWithOneStar || gyChicken;
        }

        /// <summary>
        /// Red/Blue/Yellow hand SS is once per name (oath). A leftover copy in hand cannot take center
        /// if that name already Special Summoned this turn.
        /// </summary>
        public bool HandHasSixStarWaitingToSs()
        {
            if (Bot.HasInHand(CardId.ElvenotesRed) && !spSummonedCardIdList.Contains(CardId.ElvenotesRed))
                return true;
            if (Bot.HasInHand(CardId.ElvenotesBlue) && !spSummonedCardIdList.Contains(CardId.ElvenotesBlue))
                return true;
            if (Bot.HasInHand(CardId.ElvenotesYellow) && !spSummonedCardIdList.Contains(CardId.ElvenotesYellow))
                return true;
            return false;
        }

        /// <summary>
        /// Chicken GY ③ has not resolved yet (White 7 just used chicken as material). Search order
        /// matches OnSelectCard: Blue then Yellow then Red, if that name can still inherent-SS.
        /// </summary>
        public bool ChickenGySearchWillAddHandSixStar()
        {
            if (Duel.Player != 0) return false;
            if (!DefaultCheckWhetherBotCanSearch()) return false;
            if (activatedCardIdList.Contains(CardId.JailChicken)) return false;
            if (!Bot.HasInGraveyard(CardId.JailChicken)) return false;
            if (Bot.HasInDeck(CardId.ElvenotesBlue) && !spSummonedCardIdList.Contains(CardId.ElvenotesBlue)
                && !Bot.HasInHand(CardId.ElvenotesBlue) && !activatedCardIdList.Contains(CardId.ElvenotesBlue))
                return true;
            if (Bot.HasInDeck(CardId.ElvenotesYellow) && !spSummonedCardIdList.Contains(CardId.ElvenotesYellow)
                && !Bot.HasInHand(CardId.ElvenotesYellow) && !activatedCardIdList.Contains(CardId.ElvenotesYellow))
                return true;
            if (Bot.HasInDeck(CardId.ElvenotesRed) && !spSummonedCardIdList.Contains(CardId.ElvenotesRed)
                && !Bot.HasInHand(CardId.ElvenotesRed))
                return true;
            return false;
        }

        /// <summary>
        /// Other lines that should run instead of spending the Normal Summon on Veiler.
        /// </summary>
        public bool HasOtherElfnoteExtensionWithoutVeiler()
        {
            if (Bot.HasInHand(CardId.JailChicken)
                && Bot.GetMonsters().Any(c => c != null && IsElvenotesSixStar(c) && c.IsFaceup()))
                return true;
            if (HasChickenAndLevel6OnField())
                return true;
            if (CanWindJumpToCenter())
                return true;
            if (CanMixedHellGodPendulumSs())
                return true;
            if (IsCenterEmpty() && HandHasSixStarWaitingToSs())
                return true;
            if (Bot.HasInSpellZone(CardId.GreenField) && !CheckShouldNoMoreSpSummon(CardLocation.Deck))
            {
                ClientCard center = GetCenterMonster();
                bool canEnterCenter = IsCenterEmpty() || IsUsedElvenotesBody(center);
                if (canEnterCenter && (WantGreenFieldSummonWind() || Bot.HasInDeck(CardId.JailChicken)
                    || Bot.HasInDeck(CardId.ElvenotesYellow) || Bot.HasInDeck(CardId.ElvenotesBlue)
                    || Bot.HasInDeck(CardId.ElvenotesRed)))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// White P in PZONE (or still placeable from hand) and its draw has not fired.
        /// </summary>
        public bool WhitePendulumWaitingForElvenotesSs()
        {
            if (!DefaultCheckWhetherBotCanDraw()) return false;
            if (activatedCardIdList.Contains(CardId.WhitePendulum)) return false;
            if (Bot.HasInSpellZone(CardId.WhitePendulum)) return true;
            return Bot.HasInHand(CardId.WhitePendulum) && NeedPendulumScale();
        }

        /// <summary>
        /// NS Veiler for 6+1 White 7 when there is no other extension, Extra still has a White 7
        /// whose ② is unused, and either White P needs an Elvenotes SS, or a hand 6-star is blocked
        /// by the center 6-star.
        /// </summary>
        public bool ShouldNormalSummonVeilerForWhite7()
        {
            if (!CheckWhetherCanSummon()) return false;
            if (CheckShouldNoMoreSpSummon(CardLocation.Extra)) return false;
            if (!White7EffectStillAvailable())
                return false;
            if (activatedCardIdList.Contains(CardId.White7)) return false;
            if (!Bot.GetMonsters().Any(c => c != null && IsElvenotesSixStar(c) && c.IsFaceup()))
                return false;
            if (HasOtherElfnoteExtensionWithoutVeiler()) return false;
            ClientCard center = GetCenterMonster();
            bool blockedHandSix = HandHasSixStarWaitingToSs()
                && center != null && IsElvenotesSixStar(center);
            return WhitePendulumWaitingForElvenotesSs() || blockedHandSix;
        }

        /// <summary>
        /// Unused Yellow in center can still ignition-place Red Field. Chicken +3 is Quick and
        /// would synchro that Yellow away before the place can fire.
        /// </summary>
        public bool CenterYellowStillNeedsToPlaceRedField()
        {
            if (Duel.Player != 0) return false;
            ClientCard center = GetCenterMonster();
            if (center == null || !center.IsCode(CardId.ElvenotesYellow)) return false;
            if (activatedCardIdList.Contains(CardId.ElvenotesYellow)) return false;
            if (Bot.HasInSpellZone(CardId.RedField, true, true)) return false;
            return Bot.HasInDeck(CardId.RedField) || Bot.HasInHand(CardId.RedField);
        }

        /// <summary>
        /// Face-up enemy monster with the floodgate tag only (not Extra type / ATK).
        /// </summary>
        public bool EnemyHasFloodgateMonster()
        {
            return Enemy.GetMonsters().Any(c => c != null && c.IsFaceup() && c.IsFloodgate());
        }

        /// <summary>
        /// Center Red/Blue/Yellow still has a useful own-turn ignition ② (search / place).
        /// Chicken +3 is Quick and would synchro that body away in the summon window.
        /// </summary>
        public bool CenterElvenotesStillNeedsIgnition()
        {
            if (Duel.Player != 0) return false;
            ClientCard center = GetCenterMonster();
            if (center == null || !center.IsFaceup()) return false;
            if (DefaultCheckWhetherCardIsNegated(center)) return false;
            if (Enemy.HasInSpellZone(_CardId.SkillDrain, true, true)) return false;
            if (center.IsDefense() && DefaultCheckWhetherNumber41IsActive()) return false;
            if (center.IsCode(CardId.ElvenotesRed))
                return CanElvenotesRedSearch();
            if (center.IsCode(CardId.ElvenotesBlue))
            {
                if (activatedCardIdList.Contains(CardId.ElvenotesBlue)) return false;
                if (Bot.GetSpellCountWithoutField() >= 5) return false;
                bool needGreen = Bot.HasInDeck(CardId.GreenField)
                    && !Bot.HasInSpellZone(CardId.GreenField) && !Bot.HasInHand(CardId.GreenField);
                if (!DefaultCheckWhetherBotCanSearch() && !needGreen) return false;
                return Bot.HasInDeck(CardId.GreenField) || Bot.HasInDeck(CardId.InnocentArt);
            }
            if (center.IsCode(CardId.ElvenotesYellow))
                return CenterYellowStillNeedsToPlaceRedField();
            return false;
        }

        /// <summary>
        /// Our-turn Red Field should revive chicken (or a missing White 7/10) to extend synchro,
        /// not dump used Yellow to revive Blue while Wind is already a 6-star on field.
        /// White 7 ② can revive chicken for free — do not spend Red Field first.
        /// After eight-synchro + Yellow placed Red Field, dump Yellow to revive chicken if Red/Blue/Yellow still need the center.
        /// </summary>
        public bool ShouldActivateRedFieldOnOurTurn()
        {
            bool white7OnChain = false;
            if (Duel.CurrentChain != null)
            {
                foreach (ClientCard card in Duel.CurrentChain)
                {
                    if (card != null && card.Controller == 0 && card.IsCode(CardId.White7))
                    {
                        white7OnChain = true;
                        break;
                    }
                }
            }
            // White7Activate flags the card when the chain is built, not when it resolves.
            // Treat a White 7 still on the chain as unused so Red Field does not stack on it.
            bool white7CanReviveChicken = Bot.HasInMonstersZone(CardId.White7)
                && (!activatedCardIdList.Contains(CardId.White7) || white7OnChain)
                && (Bot.HasInGraveyard(CardId.JailChicken) || Bot.HasInHand(CardId.JailChicken));
            if (white7CanReviveChicken)
                return false;

            // White 10 ② returns itself and SS up to 3. Reviving first fills the zone White 10 needs.
            if (White10OnFieldSecondEffectReady())
                return false;

            ClientCard center = GetCenterMonster();
            if (CenterYellowStillNeedsToPlaceRedField())
                return false;

            bool gyChicken = Bot.HasInGraveyard(CardId.JailChicken) && !Bot.HasInMonstersZone(CardId.JailChicken);
            bool gyWhite7 = Bot.HasInGraveyard(CardId.White7) && !Bot.HasInMonstersZone(CardId.White7);
            bool gyWhite10 = Bot.HasInGraveyard(CardId.White10) && !Bot.HasInMonstersZone(CardId.White10);
            bool hasSix = Bot.GetMonsters().Any(c => c != null && IsElvenotesSixStar(c) && c.IsFaceup());
            bool extraOk = !CheckShouldNoMoreSpSummon(CardLocation.Extra);

            if (center != null && center.IsCode(CardId.ElvenotesYellow) && activatedCardIdList.Contains(CardId.ElvenotesYellow))
            {
                if (gyChicken && HandHasSixStarWaitingToSs())
                {
                    ElfnoteLog("Red Field: dump used Yellow to revive chicken, Red/Blue/Yellow can still SS to center");
                    return true;
                }
                // Keep Yellow as the 6-star. Fall through: revive chicken with a non-Yellow cost for +3 White 10.
            }

            if (gyChicken && extraOk)
            {
                if (hasSix && White7EffectStillAvailable())
                    return true;
                if (Bot.HasInMonstersZone(CardId.White7)
                    && (Bot.HasInExtra(CardId.AccelSynchroStardust) || Bot.HasInExtra(CardId.CrystalWing)))
                    return true;
                if (!activatedCardIdList.Contains(CardId.JailChicken + 1) && White10EffectStillAvailable()
                    && center != null && IsElvenotesSixStar(center) && center.Level + 3 + 1 == 10)
                {
                    ElfnoteLog("Red Field: revive chicken, keep 6-star for +3 White 10");
                    return true;
                }
                if (center != null && center.Level == 9 && White10EffectStillAvailable())
                    return true;
                if (Bot.GetMonsters().Any(c => c != null && c.IsFaceup() && c.Level == 4)
                    && Bot.HasInExtra(CardId.SuperLibrarian))
                    return true;
            }

            // Tuner already on field: revive GY White 7 for 7+1 (Crystal Wing / Accel).
            if (gyWhite7 && extraOk && Bot.HasInMonstersZone(CardId.JailChicken)
                && (Bot.HasInExtra(CardId.AccelSynchroStardust) || Bot.HasInExtra(CardId.CrystalWing)))
            {
                ElfnoteLog("Red Field: revive White 7 for 7+1, chicken already on field");
                return true;
            }

            if (gyWhite7 || gyWhite10)
                return true;

            return false;
        }

        /// <summary>
        /// Reviving chicken with Red Field would immediately enable a synchro
        /// (6+1 White 7, 6+1 Formula after White 7 is already made, 7+1, +3 White 10, 9+1, or Librarian).
        /// Empty field does not.
        /// </summary>
        public bool RedFieldChickenEnablesSynchro()
        {
            if (CheckShouldNoMoreSpSummon(CardLocation.Extra)) return false;
            ClientCard center = GetCenterMonster();
            bool hasSix = Bot.GetMonsters().Any(c => c != null && IsElvenotesSixStar(c) && c.IsFaceup());
            if (hasSix && White7EffectStillAvailable())
                return true;
            if (hasSix && Bot.HasInExtra(CardId.FormulaAthleteLightning))
                return true;
            if (Bot.HasInMonstersZone(CardId.White7)
                && (Bot.HasInExtra(CardId.AccelSynchroStardust) || Bot.HasInExtra(CardId.CrystalWing)))
                return true;
            if (!activatedCardIdList.Contains(CardId.JailChicken + 1) && White10EffectStillAvailable()
                && center != null && IsElvenotesSixStar(center) && center.Level + 3 + 1 == 10)
                return true;
            if (center != null && center.Level == 9 && White10EffectStillAvailable())
                return true;
            if (Bot.GetMonsters().Any(c => c != null && c.IsFaceup() && c.Level == 4)
                && Bot.HasInExtra(CardId.SuperLibrarian))
                return true;
            return false;
        }

        public bool ShouldPreferRedFieldGyWhite7()
        {
            if (!Bot.HasInGraveyard(CardId.White7) || Bot.HasInMonstersZone(CardId.White7))
                return false;
            bool chickenOnField = Bot.GetMonsters().Any(c => c != null && c.IsCode(CardId.JailChicken) && c.IsFaceup());
            bool canEight = Bot.HasInExtra(new List<int> { CardId.CrystalWing, CardId.AccelSynchroStardust, CardId.PSYFramelordOmega });
            if (chickenOnField && canEight)
                return true;
            return !RedFieldChickenEnablesSynchro();
        }

        /// <summary>
        /// Mixed P spending the hand chicken is worse than letting a 6-star SS first, then NS chicken.
        /// Also skip when a 6/4-star is already out, or Wind can still jump to center.
        /// </summary>
        public bool ShouldNsChickenInsteadOfMixedHandSs()
        {
            if (!CheckWhetherCanSummon()) return false;
            if (!Bot.HasInHand(CardId.JailChicken) || Bot.HasInMonstersZone(CardId.JailChicken))
                return false;
            if (Bot.GetMonsters().Any(c => c != null && (IsElvenotesSixStar(c) || c.Level == 4)))
                return true;
            if (HandHasSixStarWaitingToSs() && IsCenterEmpty())
                return true;
            if (CanWindJumpToCenter())
                return true;
            return false;
        }

        public bool CanMixedHellGodPendulumSs()
        {
            if (Bot.HasInGraveyard(CardId.JailChicken) && !CheckShouldNoMoreSpSummon(CardLocation.Grave))
                return true;
            if (Bot.HasInHand(CardId.JailChicken) && !Bot.HasInMonstersZone(CardId.JailChicken)
                && !CheckShouldNoMoreSpSummon(CardLocation.Hand))
            {
                bool skipForNs = ShouldNsChickenInsteadOfMixedHandSs();
                ElfnoteLog("Mixed P hand chicken canSummon=" + CheckWhetherCanSummon()
                    + " skipForNs=" + skipForNs
                    + " botMon=" + Bot.GetMonsterCount()
                    + " enemyMon=" + Enemy.GetMonsterCount()
                    + " handSix=" + HandHasSixStarWaitingToSs()
                    + " handRed=" + Bot.HasInHand(CardId.ElvenotesRed)
                    + " handBlue=" + Bot.HasInHand(CardId.ElvenotesBlue)
                    + " handYellow=" + Bot.HasInHand(CardId.ElvenotesYellow)
                    + " handWind=" + Bot.HasInHand(CardId.ElvenotesWind)
                    + " emptyFieldThief=" + (Bot.GetMonsterCount() == 0 && Enemy.GetMonsterCount() > 0));
                if (skipForNs)
                {
                    ElfnoteLog("skip Mixed P hand chicken: NS chicken after 6-star SS");
                    return false;
                }
                return true;
            }
            return false;
        }

        public bool InnocentArtCanStillSearchMedius()
        {
            if (artSearched) return false;
            if (!DefaultCheckWhetherBotCanSearch()) return false;
            if (!Bot.HasInDeck(CardId.MediusTheInnocent)) return false;
            return Bot.HasInHand(CardId.InnocentArt) || Bot.HasInSpellZone(CardId.InnocentArt);
        }

        public bool JailGatePredictedMillWouldBeBanished()
        {
            bool chickenAlready = Bot.HasInMonstersZone(CardId.JailChicken) || Bot.HasInGraveyard(CardId.JailChicken);
            bool deckBanish = CheckWhetherBotWillBeBanished(CardType.Monster, CardLocation.Deck);
            bool extraBanish = CheckWhetherBotWillBeBanished(CardType.Monster, CardLocation.Extra);
            bool extraMill = Bot.HasInExtra(CardId.WhitePendulum) || Bot.HasInExtra(CardId.MixedHellGod);
            bool deckMill = Bot.HasInDeck(CardId.JailChicken) || Bot.HasInDeck(CardId.MixedHellGod)
                || Bot.HasInDeck(CardId.WhitePendulum);
            // Mill must land in GY or Gate's optional search does not resolve.
            if (!chickenAlready && Bot.HasInDeck(CardId.JailChicken) && !deckBanish)
                return false;
            if (extraMill && !extraBanish)
                return false;
            if (deckMill && !deckBanish)
                return false;
            return true;
        }

        /// <summary>
        /// Mixed P option 2: destroy self to Extra, ① searches Jail Gate, then Gate searches Medius.
        /// Requires Gate unused and not already in hand/field. Innocent Art searches Medius first.
        /// Skip when Medius is already in hand or on field: Gate is for searching Medius, not a leftover mill.
        /// </summary>
        public bool CanMixedHellGodPendulumAtkBuff()
        {
            if (activatedCardIdList.Contains(CardId.JailGodGate)) return false;
            if (Bot.HasInHand(CardId.JailGodGate) || Bot.HasInSpellZone(CardId.JailGodGate)) return false;
            if (!DefaultCheckWhetherBotCanSearch()) return false;
            if (!Bot.HasInDeck(CardId.JailGodGate)) return false;
            if (!Bot.HasInDeck(CardId.MediusTheInnocent)) return false;
            if (Bot.HasInHand(CardId.MediusTheInnocent)) return false;
            if (Bot.HasInMonstersZone(CardId.MediusTheInnocent)) return false;
            if (ShouldSkipJailGodGateForAttack()) return false;
            if (InnocentArtCanStillSearchMedius()) return false;
            if (CheckWhetherBotWillBeBanished()) return false;
            if (JailGatePredictedMillWouldBeBanished()) return false;
            return true;
        }

        public int CountFreeMainMonsterZones()
        {
            return 5 - Bot.GetMonstersInMainZone().Count;
        }

        public int CountChickenInHandDeckGrave()
        {
            int count = Bot.GetCardCountInDeck(CardId.JailChicken);
            foreach (ClientCard card in Bot.Hand)
            {
                if (card != null && card.IsCode(CardId.JailChicken))
                    count++;
            }
            foreach (ClientCard card in Bot.Graveyard)
            {
                if (card != null && card.IsCode(CardId.JailChicken))
                    count++;
            }
            return count;
        }

        /// <summary>
        /// Field still has tuner + non-tuner and Extra has a synchro we would try,
        /// so Mixed P should wait for that synchro to free a main zone.
        /// </summary>
        public bool CanSynchroToFreeMainZone()
        {
            if (Duel.Player != 0) return false;
            if (Duel.Phase != DuelPhase.Main1 && Duel.Phase != DuelPhase.Main2) return false;
            if (CheckShouldNoMoreSpSummon(CardLocation.Extra)) return false;
            bool tuner = false;
            bool nonTuner = false;
            foreach (ClientCard card in Bot.GetMonsters())
            {
                if (card == null || !card.IsFaceup() || card.Level <= 0) continue;
                if (card.HasType(CardType.Xyz | CardType.Link)) continue;
                if (card.IsTuner()) tuner = true;
                else nonTuner = true;
            }
            if (!tuner || !nonTuner) return false;
            return Bot.HasInExtra(CardId.White7) || Bot.HasInExtra(CardId.SuperLibrarian)
                || Bot.HasInExtra(CardId.AccelSynchroStardust) || Bot.HasInExtra(CardId.CrystalWing)
                || Bot.HasInExtra(CardId.White10) || Bot.HasInExtra(CardId.BaronneDeFleur)
                || Bot.HasInExtra(CardId.FormulaAthleteLightning) || Bot.HasInExtra(CardId.PSYFramelordOmega)
                || Bot.HasInExtra(CardId.ChaosAngel) || Bot.HasInExtra(CardId.BlackRoseDragon)
                || Bot.HasInExtra(CardId.AncientFishDragon) || Bot.HasInExtra(CardId.ThousandSpearDragon)
                || Bot.HasInExtra(CardId.AssaultBlackwing);
        }

        /// <summary>
        /// Face-up Continuous / Equip / Field S/T can return to hand.
        /// Skip pendulum, disabled, and cards already marked to negate this chain.
        /// Normal Spells (and similar one-use cards) cannot, even while activating.
        /// </summary>
        public bool CanYellowBounceToHand(ClientCard card)
        {
            if (card == null || !card.IsFaceup()) return false;
            if (card.IsDisabled()) return false;
            if (card.HasType(CardType.Pendulum)) return false;
            if (!card.IsSpell() && !card.IsTrap()) return false;
            return card.HasType(CardType.Continuous | CardType.Equip | CardType.Field);
        }

        public bool EnemyHasYellowBounceTarget()
        {
            foreach (ClientCard card in Enemy.GetSpells())
            {
                if (CanYellowBounceToHand(card) && !IsAlreadyMarkedNegate(card))
                    return true;
            }
            return false;
        }

        public int RedBounceStat(ClientCard card)
        {
            if (card == null) return 0;
            int atk = card.Attack;
            int def = card.GetDefensePower();
            if (atk > def) return atk;
            return def;
        }

        public List<ClientCard> GetRedBounceableMonsters()
        {
            List<ClientCard> result = new List<ClientCard>();
            foreach (ClientCard card in Enemy.GetMonsters())
            {
                if (card == null || !card.IsFaceup()) continue;
                if (card.Level < 1 || card.Level > 6) continue;
                if (IsAlreadyMarkedNegate(card)) continue;
                result.Add(card);
            }
            return result;
        }

        /// <summary>
        /// True if bouncing this body leaves the opponent without both a tuner and a non-tuner.
        /// </summary>
        public bool RedBounceBreaksSynchroPair(ClientCard bounce)
        {
            if (bounce == null) return false;
            bool hadTuner = false;
            bool hadNonTuner = false;
            bool tunerLeft = false;
            bool nonTunerLeft = false;
            foreach (ClientCard card in Enemy.GetMonsters())
            {
                if (card == null || !card.IsFaceup()) continue;
                bool tuner = card.IsTuner();
                if (tuner) hadTuner = true;
                else hadNonTuner = true;
                if (card == bounce) continue;
                if (tuner) tunerLeft = true;
                else nonTunerLeft = true;
            }
            return hadTuner && hadNonTuner && !(tunerLeft && nonTunerLeft);
        }

        public bool ShouldActivateRedSwap()
        {
            List<ClientCard> bounceable = GetRedBounceableMonsters();
            if (bounceable.Count == 0) return false;
            if ((Duel.Phase == DuelPhase.Draw || Duel.Phase == DuelPhase.Standby)
                && GetProblematicEnemyMonster() == null
                && !DefaultOnBecomeTarget())
                return false;

            List<ClientCard> problems = GetProblematicEnemyCardList(false, false, CardType.Monster);
            foreach (ClientCard card in bounceable)
            {
                if (problems.Contains(card))
                    return true;
            }

            foreach (ClientCard a in bounceable)
            {
                foreach (ClientCard b in bounceable)
                {
                    if (a != b && a.Level == b.Level)
                        return true;
                }
            }

            foreach (ClientCard card in bounceable)
            {
                if (RedBounceBreaksSynchroPair(card))
                    return true;
            }

            bool hasLink = Enemy.GetMonsters().Any(c => c != null && c.IsFaceup() && c.HasType(CardType.Link));
            if (hasLink)
                return true;

            if (DefaultOnBecomeTarget())
                return true;
            return false;
        }

        public IList<ClientCard> SelectRedBounceTarget(IList<ClientCard> cards, int min, int max)
        {
            List<ClientCard> bounceable = new List<ClientCard>();
            foreach (ClientCard card in cards)
            {
                if (card == null || card.Controller != 1) continue;
                if (!card.IsFaceup() || card.Level < 1 || card.Level > 6) continue;
                if (IsAlreadyMarkedNegate(card)) continue;
                bounceable.Add(card);
            }

            List<ClientCard> ranked = new List<ClientCard>();
            List<ClientCard> problems = GetProblematicEnemyCardList(false, false, CardType.Monster);
            foreach (ClientCard card in problems)
            {
                if (bounceable.Contains(card) && !ranked.Contains(card))
                    ranked.Add(card);
            }

            List<ClientCard> breakers = new List<ClientCard>();
            foreach (ClientCard card in bounceable)
            {
                if (RedBounceBreaksSynchroPair(card) && !ranked.Contains(card))
                    breakers.Add(card);
            }
            bool canBreakTuner = false;
            bool canBreakNonTuner = false;
            foreach (ClientCard card in breakers)
            {
                if (card.IsTuner()) canBreakTuner = true;
                else canBreakNonTuner = true;
            }
            if (canBreakTuner && canBreakNonTuner)
            {
                foreach (ClientCard card in breakers)
                {
                    if (card.IsTuner() && !ranked.Contains(card))
                        ranked.Add(card);
                }
            }
            foreach (ClientCard card in breakers)
            {
                if (!ranked.Contains(card))
                    ranked.Add(card);
            }

            List<ClientCard> sameLevel = new List<ClientCard>();
            foreach (ClientCard card in bounceable)
            {
                if (ranked.Contains(card)) continue;
                int count = 0;
                foreach (ClientCard other in bounceable)
                {
                    if (other.Level == card.Level) count++;
                }
                if (count >= 2)
                    sameLevel.Add(card);
            }
            sameLevel = sameLevel.OrderByDescending(c => RedBounceStat(c)).ToList();
            foreach (ClientCard card in sameLevel)
            {
                if (!ranked.Contains(card))
                    ranked.Add(card);
            }

            List<ClientCard> rest = bounceable.Where(c => !ranked.Contains(c))
                .OrderByDescending(c => RedBounceStat(c)).ToList();
            ranked.AddRange(rest);

            foreach (ClientCard card in cards)
            {
                if (card != null && !ranked.Contains(card))
                    ranked.Add(card);
            }
            if (ranked.Count > 0)
            {
                string bounceStr = "";
                foreach (ClientCard card in bounceable)
                {
                    bounceStr += " " + CardStr(card) + " lv=" + card.Level
                        + " stat=" + RedBounceStat(card)
                        + " problem=" + problems.Contains(card);
                }
                ElfnoteLog("Red bounce pick=" + CardStr(ranked[0]) + " bounceable=" + bounceStr);
            }
            return Util.CheckSelectCount(ranked, cards, min, max);
        }

        public bool WantGreenFieldSummonWind()
        {
            // Wind ② is once per name. A second Wind from Green Field cannot search if ② already fired.
            if (activatedCardIdList.Contains(CardId.ElvenotesWind + 1))
                return false;
            return Bot.HasInDeck(CardId.ElvenotesWind)
                && !Bot.HasInHand(CardId.ElvenotesWind)
                && !Bot.HasInMonstersZone(CardId.ElvenotesWind);
        }

        /// <summary>
        /// Green Field ② must send the center body so Wind can enter z2.
        /// Used Blue / used Red are fodder. Do not send Yellow that just placed Red Field.
        /// </summary>
        public bool CanGreenFieldSendCenterForWind(ClientCard center)
        {
            if (center == null || center.Controller != 0) return false;
            if (!IsUsedElvenotesBody(center)) return false;
            if (center.IsCode(CardId.ElvenotesYellow)) return false;
            if (CheckWhetherBotWillBeBanished(center)) return false;
            return IsGreenFieldAcceptableCost(center, CardAttribute.Wind);
        }

        /// <summary>
        /// Opponent chained a targeting effect on our Red/Blue/Yellow deck-search (②).
        /// Return that monster so Wind can send it as cost and dodge the negate.
        /// </summary>
        public ClientCard GetWindDodgeTarget()
        {
            if (Duel.CurrentChainInfo == null || Duel.CurrentChainInfo.Count < 2) return null;

            ClientCard searchMonster = null;
            int searchIndex = -1;
            for (int i = 0; i < Duel.CurrentChainInfo.Count; i++)
            {
                ChainInfo info = Duel.CurrentChainInfo[i];
                if (info == null || info.ActivatePlayer != 0) continue;
                if (!info.IsActivateCode(CardId.ElvenotesRed, CardId.ElvenotesBlue, CardId.ElvenotesYellow))
                    continue;
                if (!info.HasLocation(CardLocation.MonsterZone)) continue;
                int searchDesc = Util.GetStringId(info.ActivateId, 0);
                if (info.ActivateDescription != searchDesc && info.ActivateDescription != -1)
                    continue;

                ClientCard monster = info.RelatedCard;
                if (monster == null || monster.Controller != 0 || monster.Location != CardLocation.MonsterZone)
                {
                    if (info.ActivateSequence >= 0 && info.ActivateSequence <= 4)
                        monster = Bot.MonsterZone[info.ActivateSequence];
                }
                if (monster == null || monster.Controller != 0 || monster.Location != CardLocation.MonsterZone)
                    continue;
                if (!monster.IsCode(CardId.ElvenotesRed, CardId.ElvenotesBlue, CardId.ElvenotesYellow))
                    continue;
                searchMonster = monster;
                searchIndex = i;
                break;
            }
            if (searchMonster == null) return null;

            // Wind is already on the chain when its cost is selected, so LastChainPlayer
            // is us and LastChainTargets is empty. Look at opponent links after the ②.
            bool targeted = false;
            if (Duel.LastChainTargets != null)
            {
                foreach (ClientCard t in Duel.LastChainTargets)
                {
                    if (SameFieldCard(t, searchMonster))
                    {
                        targeted = true;
                        break;
                    }
                }
            }
            if (!targeted && Util.IsChainTarget(searchMonster))
                targeted = true;
            if (!targeted && Duel.ChainTargets != null)
            {
                foreach (ClientCard t in Duel.ChainTargets)
                {
                    if (SameFieldCard(t, searchMonster))
                    {
                        targeted = true;
                        break;
                    }
                }
            }
            if (!targeted && searchIndex >= 0)
            {
                for (int i = searchIndex + 1; i < Duel.CurrentChainInfo.Count; i++)
                {
                    ChainInfo info = Duel.CurrentChainInfo[i];
                    if (info == null || info.ActivatePlayer != 1 || info.Targets == null) continue;
                    foreach (ClientCard t in info.Targets)
                    {
                        if (SameFieldCard(t, searchMonster))
                        {
                            targeted = true;
                            break;
                        }
                    }
                    if (targeted) break;
                }
            }
            if (!targeted) return null;

            ClientCard center = GetCenterMonster();
            if (!IsCenterEmpty() && center != null && !SameFieldCard(center, searchMonster))
                return null;
            if (CheckWhetherBotWillBeBanished(searchMonster)) return null;
            return searchMonster;
        }

        public ClientCard GetWindCostCard(ClientCard exclude = null)
        {
            ClientCard dodge = GetWindDodgeTarget();
            if (dodge != null) return dodge;

            List<ClientCard> candidates = new List<ClientCard>();
            candidates.AddRange(Bot.GetMonsters());
            candidates.AddRange(Bot.Hand.Where(c => c != null && c.IsMonster()));
            candidates.AddRange(Bot.GetSpells().Where(c => c != null && c.HasSetcode(SetcodeElvenotes)));

            List<ClientCard> legal = new List<ClientCard>();
            foreach (ClientCard card in candidates)
            {
                if (card == null) continue;
                if (exclude != null && card == exclude) continue;
                if (card.IsCode(CardId.ElvenotesWind))
                {
                    // Script excludes the copy being summoned. A second Wind is legal Elvenotes fodder.
                    int windCopies = Bot.Hand.Count(c => c != null && c.IsCode(CardId.ElvenotesWind))
                        + Bot.GetMonsters().Count(c => c != null && c.IsCode(CardId.ElvenotesWind) && c.IsFaceup());
                    if (windCopies < 2) continue;
                }
                if (!card.HasSetcode(SetcodeElvenotes)) continue;
                if (IsPendulumZoneCard(card)) continue;
                // Face-down on field is not FaceupEx; script cannot take it as Wind cost.
                if ((card.Location == CardLocation.SpellZone || card.Location == CardLocation.MonsterZone)
                    && card.IsFacedown())
                    continue;
                // Keep Red Field for the opponent turn; do not dump it as Wind cost on own turn.
                // Opponent turn: used Red Field is a late fallback (see below).
                if (card.IsCode(CardId.RedField) && Duel.Player == 0) continue;
                if (card.IsCode(CardId.RedField) && Duel.Player == 1
                    && !activatedCardIdList.Contains(CardId.RedField))
                    continue;
                if (!card.IsCode(CardId.ElvenotesRed, CardId.ElvenotesBlue, CardId.ElvenotesYellow, CardId.ElvenotesWind,
                    CardId.WhitePendulum, CardId.GreenField, CardId.InnocentArt, CardId.White7, CardId.White10, CardId.JailChicken))
                {
                    if (!card.HasSetcode(SetcodeElvenotes)) continue;
                }
                if (card.IsCode(CardId.JailChicken)) continue;
                // Keep White7 / White10 / Synchro unless already negated.
                if (card.HasType(CardType.Synchro) && !card.IsDisabled()) continue;
                if (card.IsCode(CardId.White7, CardId.White10) && !card.IsDisabled()) continue;
                if (Duel.CurrentChain.Contains(card)) continue;
                if (card.IsCode(CardId.WhitePendulum) && IsPendulumZoneCard(card)) continue;
                // Own turn: unused Green Field ② still SS from deck (chicken if Wind is already in hand).
                // Dump it as Wind cost only after ②, or when ② has no monster cost.
                // Opponent turn Green ② is ignition and cannot fire; Green stays a legal cost.
                if (card.IsCode(CardId.GreenField) && !activatedCardIdList.Contains(CardId.GreenField)
                    && Duel.Player == 0 && GetGreenFieldCostMonster() != null)
                    continue;
                if (card.IsCode(CardId.ElvenotesRed) && CanElvenotesRedSearch()) continue;
                if (card.IsCode(CardId.ElvenotesBlue) && !activatedCardIdList.Contains(CardId.ElvenotesBlue)) continue;
                if (card.IsCode(CardId.ElvenotesYellow) && !activatedCardIdList.Contains(CardId.ElvenotesYellow)) continue;
                if (card.IsCode(CardId.InnocentArt) && !artSearched) continue;
                if (CheckWhetherBotWillBeBanished(card)) continue;
                legal.Add(card);
            }

            // Opponent turn cost order (Q5): center Elvenotes → used Art → hand duplicate 6
            // → Green → hand other Elvenotes → field other Elvenotes → used Red Field.
            if (Duel.Player == 1)
            {
                ClientCard centerElvenotes = legal.FirstOrDefault(c => c.Location == CardLocation.MonsterZone
                    && c.Sequence == 2 && c.HasSetcode(SetcodeElvenotes));
                if (centerElvenotes != null) return centerElvenotes;

                ClientCard usedArt = legal.FirstOrDefault(c => c.IsCode(CardId.InnocentArt) && artSearched);
                if (usedArt != null) return usedArt;

                List<int> handDupOrder = new List<int>
                {
                    CardId.ElvenotesBlue, CardId.ElvenotesRed, CardId.ElvenotesYellow, CardId.WhitePendulum
                };
                foreach (int id in handDupOrder)
                {
                    ClientCard dup = legal.FirstOrDefault(c => c.Location == CardLocation.Hand && c.IsCode(id)
                        && Bot.Hand.Count(h => h != null && h.IsCode(id)) > 1);
                    if (dup != null) return dup;
                }

                ClientCard green = legal.FirstOrDefault(c => c.IsCode(CardId.GreenField));
                if (green != null) return green;

                ClientCard handOther = legal.FirstOrDefault(c => c.Location == CardLocation.Hand
                    && c.HasSetcode(SetcodeElvenotes) && !c.IsCode(CardId.ElvenotesWind));
                if (handOther != null) return handOther;

                ClientCard fieldOther = legal.FirstOrDefault(c =>
                    (c.Location == CardLocation.MonsterZone || c.Location == CardLocation.SpellZone)
                    && c.HasSetcode(SetcodeElvenotes) && !c.IsCode(CardId.RedField));
                if (fieldOther != null) return fieldOther;

                ClientCard usedRedField = legal.FirstOrDefault(c => c.IsCode(CardId.RedField)
                    && activatedCardIdList.Contains(CardId.RedField)
                    && !Duel.CurrentChain.Contains(c));
                if (usedRedField != null) return usedRedField;

                return legal.FirstOrDefault();
            }

            ClientCard usedBlue = legal.FirstOrDefault(c => c.Location == CardLocation.MonsterZone && c.IsCode(CardId.ElvenotesBlue));
            if (usedBlue != null) return usedBlue;

            List<ClientCard> extraSix = legal.Where(c => c.Location == CardLocation.Hand && IsElvenotesSixStar(c)
                && Bot.Hand.Count(h => h != null && h.IsCode(c.Id)) > 1).ToList();
            if (extraSix.Count > 0) return extraSix[0];

            ClientCard usedArtOwn = legal.FirstOrDefault(c => c.IsCode(CardId.InnocentArt) && artSearched);
            if (usedArtOwn != null) return usedArtOwn;

            ClientCard usedGreen = legal.FirstOrDefault(c => c.IsCode(CardId.GreenField) && activatedCardIdList.Contains(CardId.GreenField)
                && (Bot.HasInMonstersZone(CardId.ElvenotesWind) || Bot.HasInHand(CardId.ElvenotesWind)));
            if (usedGreen != null) return usedGreen;

            // Yellow ② places Red Field from hand or deck. Do not send Yellow while a copy is still only in hand.
            if (Bot.HasInSpellZone(CardId.RedField) || (!Bot.HasInDeck(CardId.RedField) && !Bot.HasInHand(CardId.RedField)))
            {
                ClientCard yellow = legal.FirstOrDefault(c => c.IsCode(CardId.ElvenotesYellow));
                if (yellow != null) return yellow;
            }

            ClientCard greenOwn = legal.FirstOrDefault(c => c.IsCode(CardId.GreenField));
            if (greenOwn != null) return greenOwn;

            return legal.FirstOrDefault();
        }

        public bool CanSynchroEightWithWhite7()
        {
            // Opponent turn has no idle synchro. 7+1 must not block chicken +3 White 10.
            if (Duel.Player == 1) return false;
            if (!Bot.HasInMonstersZone(CardId.White7)) return false;
            if (!Bot.HasInMonstersZone(CardId.JailChicken)) return false;
            if (CheckShouldNoMoreSpSummon(CardLocation.Extra)) return false;
            // Crystal still in Extra: real 7+1 line.
            if (Bot.HasInExtra(CardId.CrystalWing)) return true;
            // Accel alone: AccelSpSummon skips when Crystal already sits (unless librarian line).
            if (Bot.HasInExtra(CardId.AccelSynchroStardust)
                && !Bot.HasInMonstersZone(CardId.CrystalWing))
                return true;
            // Omega is the leftover 8 after Crystal Wing already sits (Red opening step 7).
            if (Bot.HasInExtra(CardId.PSYFramelordOmega))
                return true;
            return false;
        }

        /// <summary>
        /// Face-up 6-star + chicken can still make Formula (7) after White 7 / Crystal are already out.
        /// </summary>
        public bool CanSynchroSevenWithChicken()
        {
            if (!HasChickenAndLevel6OnField()) return false;
            if (CheckShouldNoMoreSpSummon(CardLocation.Extra)) return false;
            return Bot.HasInExtra(CardId.FormulaAthleteLightning);
        }

        /// <summary>
        /// Two face-up chickens + a leftover 6-star is 8 (Omega) after Crystal Wing already sits.
        /// </summary>
        public bool CanSynchroEightWithTwoChickens()
        {
            if (CountFaceupChickenOnField() < 2) return false;
            if (CheckShouldNoMoreSpSummon(CardLocation.Extra)) return false;
            if (!Bot.HasInExtra(CardId.PSYFramelordOmega) && !Bot.HasInExtra(CardId.CrystalWing)
                && !(Bot.HasInExtra(CardId.AccelSynchroStardust) && !Bot.HasInMonstersZone(CardId.CrystalWing)))
                return false;
            return Bot.GetMonsters().Any(c => c != null && c.IsFaceup() && !c.IsTuner() && c.Level == 6
                && !c.IsCode(CardId.CrystalWing, CardId.FormulaAthleteLightning, CardId.BaronneDeFleur, CardId.SuperLibrarian));
        }

        /// <summary>
        /// Librarian then 8-synchro is only legal when a tuner remains after 4+1:
        /// chicken + chicken + Level 4 (Medius) + White 7. One chicken would be eaten
        /// and 7+1 would die. Crystal already out: Accel is the leftover 8.
        /// </summary>
        public bool CanLibrarianThenEightSynchro()
        {
            if (!Bot.HasInMonstersZone(CardId.White7)) return false;
            if (CountFaceupChickenOnField() < 2) return false;
            if (!Bot.GetMonsters().Any(c => c != null && c.IsFaceup() && c.Level == 4))
                return false;
            if (!Bot.HasInMonstersZone(CardId.CrystalWing)) return false;
            if (Bot.HasInMonstersZone(CardId.SuperLibrarian, true, false, true)) return false;
            if (!Bot.HasInExtra(CardId.AccelSynchroStardust))
                return false;
            if (CheckShouldNoMoreSpSummon(CardLocation.Extra)) return false;
            return true;
        }

        public bool CanChickenLevelForWhite10()
        {
            if (!White10EffectStillAvailable())
                return false;
            // +3 is once per turn. After it already made White 10, leftover 6+1 should go to FA,
            // not wait for a second +3 that JailChickenActivate will never accept.
            if (activatedCardIdList.Contains(CardId.JailChicken + 1)) return false;
            ClientCard chicken = Bot.GetMonsters().FirstOrDefault(c => c != null && c.IsCode(CardId.JailChicken) && c.IsFaceup());
            ClientCard center = GetCenterMonster();
            if (chicken == null || center == null || center.Level < 1) return false;
            if (center.IsCode(CardId.White7)) return false;
            return center.Level + 3 + chicken.Level == 10;
        }

        /// <summary>
        /// White 7 / White 10 are Quick Effects. If the current chain is only our cards
        /// (typically our own search), wait for it to finish so the new card can be used.
        /// White 7 does not wait for White P: extra recycle can steal GY chicken.
        /// White 10 does wait for White P: recycle can put a 6-star in hand for the SS.
        /// </summary>
        public bool ShouldWaitOwnChainToResolve(bool waitForWhiteP = false)
        {
            if (Duel.CurrentChain == null || Duel.CurrentChain.Count == 0)
                return false;
            if (Duel.LastChainPlayer == 1)
                return false;
            bool hasWaitable = false;
            foreach (ClientCard card in Duel.CurrentChain)
            {
                if (card != null && card.Controller == 1)
                    return false;
                if (card != null && card.IsCode(CardId.WhitePendulum) && !waitForWhiteP)
                    continue;
                if (card != null && card.Controller == 0)
                    hasWaitable = true;
            }
            return hasWaitable;
        }

        /// <summary>
        /// Opponent turn: if the chain is resolving our White 7 ② / Red Field ② / White 10 ② /
        /// chicken ③ / Wind ① and the opponent has not inserted, wait for it to finish first.
        /// </summary>
        public bool ShouldWaitOwnEnemyTurnDisruption(bool waitForWind, bool waitForChicken)
        {
            if (Duel.CurrentChain == null || Duel.CurrentChain.Count == 0)
                return false;
            if (Duel.LastChainPlayer == 1)
                return false;
            bool hasWaitable = false;
            foreach (ClientCard card in Duel.CurrentChain)
            {
                if (card == null) continue;
                if (card.Controller == 1)
                    return false;
                if (card.Controller != 0) continue;
                if (card.IsCode(CardId.White7, CardId.RedField, CardId.White10))
                    hasWaitable = true;
                else if (waitForChicken && card.IsCode(CardId.JailChicken))
                    hasWaitable = true;
                else if (waitForWind && card.IsCode(CardId.ElvenotesWind))
                    hasWaitable = true;
            }
            return hasWaitable;
        }

        /// <summary>
        /// Accel ② skips if our chicken / White 10 / Wind is already on the chain and the
        /// opponent has not chained anything after that link. Opponent opening the chain
        /// (e.g. Mixed Hell God CL1, chicken CL2) still counts as wait; an insert after
        /// those cards does not.
        /// </summary>
        public bool Accel2OwnWaitableOnChain()
        {
            if (Duel.CurrentChain == null || Duel.CurrentChain.Count == 0)
                return false;
            int lastWaitable = -1;
            for (int i = 0; i < Duel.CurrentChain.Count; ++i)
            {
                ClientCard card = Duel.CurrentChain[i];
                if (card == null || card.Controller != 0) continue;
                if (card.IsCode(CardId.JailChicken, CardId.White10, CardId.ElvenotesWind))
                    lastWaitable = i;
            }
            if (lastWaitable < 0)
                return false;
            for (int i = lastWaitable + 1; i < Duel.CurrentChain.Count; ++i)
            {
                ClientCard card = Duel.CurrentChain[i];
                if (card != null && card.Controller == 1)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Opponent-turn Accel ②: chicken +3 → White 10 → 解体 → unused center Wind ② first.
        /// Accel ② pays tribute as cost; stacking it with chicken eats the materials before
        /// the follow-up synchro, and firing it before Wind ② skips the missing-color ③.
        /// </summary>
        public bool ShouldWaitChickenWhite10BeforeAccel2()
        {
            if (Duel.Player != 1) return false;
            if (!activatedCardIdList.Contains(CardId.ElvenotesWind + 1)
                && !CheckShouldNoMoreSpSummon(CardLocation.Deck))
            {
                ClientCard windCenter = GetCenterMonster();
                if (windCenter != null && windCenter.IsFaceup() && windCenter.IsCode(CardId.ElvenotesWind))
                {
                    ElfnoteLog("skip Accel ②: Wind ② first");
                    return true;
                }
            }
            if (Bot.HasInMonstersZone(CardId.White10, true, false, true)
                && !activatedCardIdList.Contains(CardId.White10))
            {
                ElfnoteLog("skip Accel ②: White10 ② first");
                return true;
            }
            if (activatedCardIdList.Contains(CardId.JailChicken + 1)) return false;
            if (!White10EffectStillAvailable()) return false;
            if (CanSynchroEightWithWhite7()) return false;
            if (CenterElvenotesStillNeedsIgnition()) return false;
            ClientCard chicken = null;
            foreach (ClientCard card in Bot.GetMonsters())
            {
                if (card != null && card.IsFaceup() && card.IsCode(CardId.JailChicken))
                {
                    chicken = card;
                    break;
                }
            }
            if (chicken == null) return false;
            ClientCard center = GetCenterMonster();
            if (center == null || center == chicken || center.Level < 1) return false;
            if (center.Level + 3 + chicken.Level != 10) return false;
            ElfnoteLog("skip Accel ②: chicken +3 White10 first center=" + CardStr(center));
            return true;
        }

        /// <summary>
        /// White 7 must not stack on our Red Field ② unless the opponent inserted after it.
        /// Red Field SS needs a zone at resolution; White 7 resolving first can fill it and drop the negate.
        /// </summary>
        public bool ShouldWaitRedFieldBeforeWhite7()
        {
            if (Duel.CurrentChain == null || Duel.CurrentChain.Count == 0)
                return false;
            if (Duel.LastChainPlayer == 1)
                return false;
            foreach (ClientCard card in Duel.CurrentChain)
            {
                if (card != null && card.Controller == 0 && card.IsCode(CardId.RedField))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Floodgate / dangerous / invincible monsters, plus face-up floodgate S/T.
        /// Sitting Extra and generic continuous S/T are not tight danger.
        /// </summary>
        public bool EnemyHasTightDangerCard()
        {
            foreach (ClientCard monster in Enemy.GetMonsters())
            {
                if (monster == null || monster.Data == null || !monster.IsFaceup()) continue;
                if (monster.IsFloodgate() || monster.IsMonsterDangerous() || monster.IsMonsterInvincible())
                    return true;
            }
            foreach (ClientCard spell in Enemy.GetSpells())
            {
                if (spell == null || spell.Data == null || !spell.IsFaceup()) continue;
                if (spell.IsFloodgate())
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Jail Gate "danger monster": floodgate / dangerous / invincible,
        /// sitting Extra (Fusion/Ritual/Synchro/Xyz, Link-2+), and attack-position monsters we cannot beat.
        /// </summary>
        public bool EnemyHasTightDangerMonster()
        {
            foreach (ClientCard monster in Enemy.GetMonsters())
            {
                if (monster == null || monster.Data == null || !monster.IsFaceup()) continue;
                if (monster.IsFloodgate() || monster.IsMonsterDangerous() || monster.IsMonsterInvincible())
                    return true;
                if (monster.HasType(CardType.Fusion | CardType.Ritual | CardType.Synchro | CardType.Xyz)
                    || (monster.HasType(CardType.Link) && monster.LinkCount >= 2))
                    return true;
            }
            int bestBotAtk = Util.GetBestAttack(Bot);
            foreach (ClientCard monster in Enemy.GetMonsters())
            {
                if (monster == null || !monster.IsFaceup() || !monster.IsAttack()) continue;
                int power = monster.GetDefensePower();
                if (power > 0 && power >= bestBotAtk)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Keep on-field Thousand Spear for opponent-turn Red Field cost (③ only on the opponent turn).
        /// </summary>
        public bool ShouldKeepThousandSpearForRedFieldCost()
        {
            return Bot.GetMonsters().Any(c => c != null && c.IsFaceup() && c.IsCode(CardId.ThousandSpearDragon))
                && Bot.HasInSpellZone(CardId.RedField, true);
        }

        /// <summary>
        /// Baronne may eat Thousand Spear in MP2, or in MP1 when a tight danger card is up and Baronne would not be negated.
        /// </summary>
        public bool ShouldAllowThousandSpearAsBaronneMaterial()
        {
            if (!Bot.HasInExtra(CardId.BaronneDeFleur))
                return false;
            if (CheckWhetherNegated(true, true, CardType.Monster))
                return false;
            if (Duel.Phase == DuelPhase.Main2)
                return true;
            return Duel.Phase == DuelPhase.Main1 && EnemyHasTightDangerCard();
        }

        public bool HasJailGodMonsterInDeck()
        {
            return Bot.HasInDeck(CardId.JailChicken) || Bot.HasInDeck(CardId.MixedHellGod)
                || Bot.HasInDeck(CardId.WhitePendulum) || Bot.HasInDeck(CardId.MediusTheInnocent);
        }

        /// <summary>
        /// ① can still fire after GY ② SS: not used this turn, and deck still has a Jail God monster.
        /// </summary>
        public bool MediusEffect1CanFireAfterGySs()
        {
            return !activatedCardIdList.Contains(CardId.MediusTheInnocent) && HasJailGodMonsterInDeck();
        }

        public bool CanFieldMakeExtraSynchro()
        {
            if (Bot.HasInExtra(CardId.SuperLibrarian) && CanSynchroLevel(5)) return true;
            if ((Bot.HasInExtra(CardId.White7) || Bot.HasInExtra(CardId.FormulaAthleteLightning)
                || Bot.HasInExtra(CardId.BlackRoseDragon) || Bot.HasInExtra(CardId.AssaultBlackwing))
                && CanSynchroLevel(7))
                return true;
            if ((Bot.HasInExtra(CardId.CrystalWing) || Bot.HasInExtra(CardId.AccelSynchroStardust)
                || Bot.HasInExtra(CardId.PSYFramelordOmega) || Bot.HasInExtra(CardId.ChaosAngel))
                && CanSynchroLevel(8))
                return true;
            if ((Bot.HasInExtra(CardId.AncientFishDragon) || Bot.HasInExtra(CardId.ThousandSpearDragon))
                && CanSynchroLevel(9))
                return true;
            if (CanMakeWhite10Synchro()) return true;
            if (Bot.HasInExtra(CardId.BaronneDeFleur) && CanSynchroLevel(10)) return true;
            return false;
        }

        /// <summary>
        /// ① already used: GY ② only if the only tuner is chicken, chicken cannot synchro with
        /// current non-tuners, and bouncing a non-chicken monster would make Librarian.
        /// </summary>
        public bool CanMediusGySsForLibrarianAfterEffect1Used()
        {
            if (!Bot.HasInExtra(CardId.SuperLibrarian))
                return false;
            List<ClientCard> faceup = Bot.GetMonsters().Where(c => c != null && c.IsFaceup()).ToList();
            List<ClientCard> tuners = faceup.Where(c => c.IsTuner()).ToList();
            if (tuners.Count == 0) return false;
            foreach (ClientCard tuner in tuners)
            {
                if (tuner == null || !tuner.IsCode(CardId.JailChicken))
                    return false;
            }
            if (CanFieldMakeExtraSynchro()) return false;
            bool chickenOnField = faceup.Any(c => c.IsCode(CardId.JailChicken));
            if (!chickenOnField) return false;
            bool fieldBounce = faceup.Any(c => !c.IsCode(CardId.JailChicken) && !c.HasType(CardType.Synchro));
            bool handBounce = Bot.Hand.Any(c => c != null && c.IsMonster() && !c.IsCode(CardId.JailChicken))
                && Bot.GetMonstersInMainZone().Count < 5;
            return fieldBounce || handBounce;
        }

        /// <summary>
        /// Jail Gate locks non-Jail attacks this turn. Skip in an attack-capable Main 1 when
        /// we already have board advantage (can battle over sitting Extra) and can still extend
        /// or win the battle. Floodgate / invincible / unbeatable Extra still allow Jail Gate.
        /// </summary>
        public bool EnemyHasUnbeatableJailGateDanger()
        {
            int bestBotAtk = Util.GetBestAttack(Bot);
            foreach (ClientCard monster in Enemy.GetMonsters())
            {
                if (monster == null || monster.Data == null || !monster.IsFaceup()) continue;
                if (monster.IsFloodgate() || monster.IsMonsterDangerous() || monster.IsMonsterInvincible())
                    return true;
                bool extraBody = monster.HasType(CardType.Fusion | CardType.Ritual | CardType.Synchro | CardType.Xyz)
                    || (monster.HasType(CardType.Link) && monster.LinkCount >= 2);
                if (extraBody && monster.GetDefensePower() >= bestBotAtk)
                    return true;
            }
            foreach (ClientCard monster in Enemy.GetMonsters())
            {
                if (monster == null || !monster.IsFaceup() || !monster.IsAttack()) continue;
                int power = monster.GetDefensePower();
                if (power > 0 && power >= bestBotAtk)
                    return true;
            }
            return false;
        }

        public bool ShouldSkipJailGodGateForAttack()
        {
            if (Duel.Player != 0 || Duel.Phase != DuelPhase.Main1)
                return false;
            bool canAttackThisTurn = !(Duel.Turn == 1 && Duel.IsFirst);
            if (!canAttackThisTurn) return false;
            if (EnemyHasUnbeatableJailGateDanger()) return false;
            if (CanFieldMakeExtraSynchro() || HandHasSixStarWaitingToSs())
                return true;
            int bestBotAtk = 0;
            foreach (ClientCard monster in Bot.GetMonsters())
            {
                if (monster != null && monster.IsFaceup() && monster.Attack > bestBotAtk)
                    bestBotAtk = monster.Attack;
            }
            if (bestBotAtk <= 0) return false;
            if (Enemy.LifePoints <= bestBotAtk) return true;
            if (Enemy.GetMonsterCount() == 0) return true;
            int bestEnemy = 0;
            foreach (ClientCard monster in Enemy.GetMonsters())
            {
                if (monster == null) continue;
                int power = monster.GetDefensePower();
                if (power > bestEnemy) bestEnemy = power;
            }
            return bestBotAtk > bestEnemy;
        }

        public bool HasEightSynchroInExtra()
        {
            return Bot.HasInExtra(new List<int>
            {
                CardId.CrystalWing, CardId.AccelSynchroStardust, CardId.PSYFramelordOmega
            });
        }

        /// <summary>
        /// Whether Red/Blue/Yellow ③ swap is worth activating on the opponent turn at this timing.
        /// assumeCenterFilled: White 10 SS of multiple bodies will fill center this batch,
        /// so do not judge from the field at the call site alone.
        /// </summary>
        public bool CanRbySwapNow(int cardId, bool assumeCenterFilled)
        {
            if (Duel.Player != 1) return false;
            if (!assumeCenterFilled && GetCenterMonster() == null) return false;
            if (cardId == CardId.ElvenotesRed)
            {
                if (activatedCardIdList.Contains(CardId.ElvenotesRed + 3)) return false;
                return ShouldActivateRedSwap();
            }
            if (cardId == CardId.ElvenotesBlue)
            {
                if (activatedCardIdList.Contains(CardId.ElvenotesBlue + 3)) return false;
                if (Duel.Phase == DuelPhase.End) return false;
                return Enemy.Hand.Count > 0;
            }
            if (cardId == CardId.ElvenotesYellow)
            {
                if (activatedCardIdList.Contains(CardId.ElvenotesYellow + 3)) return false;
                return EnemyHasYellowBounceTarget();
            }
            return false;
        }

        public bool CanRbySwapNow(ClientCard card, bool assumeCenterFilled)
        {
            if (card == null) return false;
            return CanRbySwapNow(card.Id, assumeCenterFilled);
        }

        /// <summary>
        /// Face-up Red/Blue/Yellow in a main side zone can already ③ (sequence&lt;5, not center).
        /// ③ is once per name; pulling another copy from GY does not get a second swap.
        /// </summary>
        public bool HasFaceupRbyReadyToSwap(int cardId)
        {
            foreach (ClientCard card in Bot.GetMonsters())
            {
                if (card == null || !card.IsFaceup() || !card.IsCode(cardId)) continue;
                if (card.Sequence >= 5 || card.Sequence == 2) continue;
                return true;
            }
            return false;
        }

        public bool GyRbyWorthWhite7Pull(int cardId)
        {
            if (!Bot.HasInGraveyard(cardId)) return false;
            if (!CanRbySwapNow(cardId, true)) return false;
            if (HasFaceupRbyReadyToSwap(cardId)) return false;
            return true;
        }

        public bool White10EffectStillAvailable()
        {
            return Bot.HasInExtra(CardId.White10) && !activatedCardIdList.Contains(CardId.White10);
        }

        public bool White7EffectStillAvailable()
        {
            return Bot.HasInExtra(CardId.White7) && !activatedCardIdList.Contains(CardId.White7);
        }

        /// <summary>
        /// Synchro ② after chicken +3 would be negated on the field: Skill Drain, or Number 41
        /// while the summoned body would sit in Defense (White 7 original ATK 0 always Defense).
        /// </summary>
        public bool ElvenotesSynchroSecondWouldBeNegatedOnSummon(int synchroId)
        {
            if (Enemy.HasInSpellZone(_CardId.SkillDrain, true, true)) return true;
            if (!DefaultCheckWhetherNumber41IsActive()) return false;
            NamedCard data = NamedCard.Get(synchroId);
            if (data == null) return true;
            if (data.Attack == 0) return true;
            if ((Duel.Turn == 1 || Duel.Phase >= DuelPhase.Main2) && data.Attack <= data.Defense) return true;
            if (Duel.Player == 1 && (data.Defense >= data.Attack || data.Attack < 2000)) return true;
            return false;
        }

        /// <summary>
        /// After chicken +3 on the current center body: (center Level + 3) + chicken Level.
        /// Uses live Levels; do not assume the Elvenotes is still 6.
        /// </summary>
        public int GetChickenPlus3SynchroLevel()
        {
            ClientCard center = GetCenterMonster();
            ClientCard chicken = Bot.GetMonsters().FirstOrDefault(c => c != null && c.IsCode(CardId.JailChicken) && c.IsFaceup());
            if (center == null || chicken == null || center.Level < 1) return 0;
            return center.Level + 3 + chicken.Level;
        }

        /// <summary>
        /// Extra synchro chicken +3 would make right now: White 10 (10) or White 7 (7), if ② is still usable.
        /// </summary>
        public int GetChickenDodgeSynchroId()
        {
            int lv = GetChickenPlus3SynchroLevel();
            if (lv == 10 && White10EffectStillAvailable()
                && !ElvenotesSynchroSecondWouldBeNegatedOnSummon(CardId.White10))
                return CardId.White10;
            if (lv == 7 && White7EffectStillAvailable()
                && !ElvenotesSynchroSecondWouldBeNegatedOnSummon(CardId.White7))
                return CardId.White7;
            return 0;
        }

        /// <summary>
        /// Last two links: our center Elvenotes (Red/Blue/Yellow/Wind by code) then opponent
        /// targeting it, or chaining Ghost Ogre (does not target; destroys the activating card).
        /// +3 must make White 10 or White 7 from current Levels.
        /// </summary>
        public bool ShouldChickenPlus3DodgeNegate()
        {
            if (activatedCardIdList.Contains(CardId.JailChicken + 1)) return false;
            if (!Bot.GetMonsters().Any(c => c != null && c.IsCode(CardId.JailChicken) && c.IsFaceup()))
                return false;
            if (Duel.CurrentChain == null || Duel.CurrentChain.Count < 2) return false;
            if (Duel.LastChainPlayer != 1) return false;

            ClientCard prev = Duel.CurrentChain[Duel.CurrentChain.Count - 2];
            if (prev == null || prev.Controller != 0) return false;
            if (!IsElvenotesSixStar(prev)) return false;
            if (prev.Location != CardLocation.MonsterZone || prev.Sequence != 2) return false;

            bool targeted = Util.IsChainTarget(prev)
                || (Duel.LastChainTargets != null && Duel.LastChainTargets.Any(t => t != null && t.Equals(prev)));
            ClientCard last = Duel.CurrentChain[Duel.CurrentChain.Count - 1];
            bool ghostOgre = last != null && last.IsCode(_CardId.GhostOgreAndSnowRabbit);
            if (!targeted && !ghostOgre) return false;

            int synchroId = GetChickenDodgeSynchroId();
            if (synchroId == 0) return false;
            ElfnoteLog("Chicken +3 dodge last-1=" + CardStr(prev) + " last=" + CardStr(last)
                + " targeted=" + targeted + " ghostOgre=" + ghostOgre + " synchro=" + synchroId
                + " plus3Lv=" + GetChickenPlus3SynchroLevel());
            return true;
        }

        /// <summary>
        /// Face-up center White 10 whose ② has not been used yet (Main Phase only).
        /// </summary>
        public bool White10OnFieldSecondEffectReady()
        {
            if (activatedCardIdList.Contains(CardId.White10)) return false;
            if (Duel.Phase != DuelPhase.Main1 && Duel.Phase != DuelPhase.Main2) return false;
            ClientCard white10 = Bot.GetMonsters().FirstOrDefault(c => c != null && c.IsCode(CardId.White10)
                && c.IsFaceup() && c.Sequence == 2 && !c.IsDisabled());
            return white10 != null;
        }

        /// <summary>
        /// Our MP1 when Battle Phase is still available: keep a live Assault Blackwing
        /// (attack-all / pierce / ATK from Synchro materials) instead of eating it.
        /// </summary>
        public bool ShouldKeepAssaultBlackwingForBattle()
        {
            if (Duel.Player != 0 || Duel.Phase != DuelPhase.Main1 || Duel.Turn <= 1)
                return false;
            return Bot.GetMonsters().Any(c => c != null && c.IsCode(CardId.AssaultBlackwing)
                && c.IsFaceup() && !c.IsDisabled());
        }

        /// <summary>
        /// Opponent-turn Wind ① White 10 line: Wind into center, ② SS chicken, then +3 synchro White 10 and dismantle.
        /// </summary>
        public bool CanWindEnemyTurnWhite10Line()
        {
            if (!White10EffectStillAvailable())
            {
                ElfnoteLog("Wind enemy White10 line fail: extraW10=" + Bot.HasInExtra(CardId.White10)
                    + " w10Used=" + activatedCardIdList.Contains(CardId.White10));
                return false;
            }
            if (!Bot.HasInDeck(CardId.JailChicken))
            {
                ElfnoteLog("Wind enemy White10 line fail: no deck chicken");
                return false;
            }
            if (activatedCardIdList.Contains(CardId.JailChicken + 1))
            {
                ElfnoteLog("Wind enemy White10 line fail: chicken +3 already used");
                return false;
            }
            if (activatedCardIdList.Contains(CardId.ElvenotesWind + 1))
            {
                ElfnoteLog("Wind enemy White10 line fail: Wind ② already used");
                return false;
            }
            // After Wind takes center, keep one zone for chicken
            ClientCard cost = GetWindCostCard();
            int freeAfter = CountFreeMainMonsterZones();
            if (cost != null && cost.Location == CardLocation.MonsterZone)
                freeAfter += 1;
            if (IsCenterEmpty() || (cost != null && GetCenterMonster() == cost))
            {
                // Center empty or the cost is the center monster: after SS Wind occupies center, SS chicken needs at least 1 free main zone
                if (freeAfter < 1)
                {
                    ElfnoteLog("Wind enemy White10 line fail: no zone for chicken freeAfter=" + freeAfter);
                    return false;
                }
            }
            else
            {
                ElfnoteLog("Wind enemy White10 line fail: cannot enter center cost=" + CardStr(cost));
                return false;
            }
            return true;
        }

        /// <summary>
        /// Opponent-turn Wind ②: whether the deck has a Red/Blue/Yellow that passes the ③ swap check.
        /// </summary>
        public bool DeckHasRbySwapTarget()
        {
            List<int> order = new List<int> { CardId.ElvenotesRed, CardId.ElvenotesBlue, CardId.ElvenotesYellow };
            foreach (int id in order)
            {
                if (!Bot.HasInDeck(id)) continue;
                // Wind is already in center; SS Red/Blue/Yellow into a side zone is enough to swap
                if (CanRbySwapNow(id, true))
                    return true;
            }
            return false;
        }

        public bool IsBattlePhaseNow()
        {
            return Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2;
        }

        public bool EnemyHasMonsterThatBeatsChicken()
        {
            NamedCard chickenData = NamedCard.Get(CardId.JailChicken);
            int chickenAtk = chickenData != null ? chickenData.Attack : 300;
            return Enemy.GetMonsters().Any(c => c != null && c.IsAttack() && c.Attack > chickenAtk);
        }

        /// <summary>
        /// Opponent-turn Wind ②: whether to take the chicken → White 10 line.
        /// </summary>
        public bool WantWindEnemyTurnSsChicken()
        {
            if (IsBattlePhaseNow()) return false;
            if (!Bot.HasInDeck(CardId.JailChicken)) return false;
            if (!White10EffectStillAvailable()) return false;
            if (activatedCardIdList.Contains(CardId.JailChicken + 1)) return false;
            return true;
        }

        public bool CanSynchroLevel(int level)
        {
            List<ClientCard> monsters = Bot.GetMonsters().Where(c => c != null && c.IsFaceup()).ToList();
            List<ClientCard> tuners = monsters.Where(c => c.IsTuner()).ToList();
            List<ClientCard> nonTuners = monsters.Where(c => !c.IsTuner()).ToList();
            foreach (ClientCard tuner in tuners)
            {
                foreach (ClientCard nonTuner in nonTuners)
                {
                    if (tuner.Level + nonTuner.Level == level)
                        return true;
                }
            }
            foreach (ClientCard tuner in tuners)
            {
                for (int i = 0; i < nonTuners.Count; ++i)
                {
                    for (int j = i + 1; j < nonTuners.Count; ++j)
                    {
                        if (tuner.Level + nonTuners[i].Level + nonTuners[j].Level == level)
                            return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// White 10's non-tuner must be Spellcaster. Ancient Fish (9) + chicken (1) is level 10
        /// but cannot make White 10; that pair should go to Baronne.
        /// </summary>
        public bool CanMakeWhite10Synchro()
        {
            if (!White10EffectStillAvailable())
                return false;
            List<ClientCard> monsters = Bot.GetMonsters().Where(c => c != null && c.IsFaceup()).ToList();
            List<ClientCard> tuners = monsters.Where(c => c.IsTuner()).ToList();
            List<ClientCard> spellNonTuners = monsters.Where(c => !c.IsTuner() && c.HasRace(CardRace.SpellCaster)).ToList();
            foreach (ClientCard tuner in tuners)
            {
                foreach (ClientCard nonTuner in spellNonTuners)
                {
                    if (tuner.Level + nonTuner.Level == 10)
                        return true;
                }
            }
            foreach (ClientCard tuner in tuners)
            {
                for (int i = 0; i < spellNonTuners.Count; ++i)
                {
                    for (int j = i + 1; j < spellNonTuners.Count; ++j)
                    {
                        if (tuner.Level + spellNonTuners[i].Level + spellNonTuners[j].Level == 10)
                            return true;
                    }
                }
            }
            return false;
        }

        public bool ShouldPreferWhite10Synchro()
        {
            if (ShouldRerouteChickenSixUnderBanish()) return false;
            if (ShouldFuwalosCompromiseIdleBaronne()) return false;
            return CanMakeWhite10Synchro();
        }

        public bool IsLightOrDark(ClientCard card)
        {
            if (card == null) return false;
            return card.HasAttribute(CardAttribute.Light | CardAttribute.Dark);
        }

        public bool IsChaosAngelTunerSlot(ClientCard card)
        {
            return card != null && (card.IsTuner() || IsLightOrDark(card));
        }

        public bool IsChaosAngelNonTunerLd(ClientCard card)
        {
            return card != null && !card.IsTuner() && IsLightOrDark(card);
        }

        public bool ChaosAngelPairOk(ClientCard a, ClientCard b)
        {
            if (a == null || b == null || a == b) return false;
            if (a.Level + b.Level != 10) return false;
            return (IsChaosAngelTunerSlot(a) && IsChaosAngelNonTunerLd(b))
                || (IsChaosAngelTunerSlot(b) && IsChaosAngelNonTunerLd(a));
        }

        public List<ClientCard> GetChaosAngelMaterialPool(bool allowFaLine)
        {
            List<ClientCard> monsters = Bot.GetMonsters().Where(c => c != null && c.IsFaceup()).ToList();
            monsters = monsters.Where(c => !c.IsCode(CardId.CrystalWing, CardId.BaronneDeFleur, CardId.White10)).ToList();
            if (ShouldKeepAssaultBlackwingForBattle())
                monsters = monsters.Where(c => !c.IsCode(CardId.AssaultBlackwing) || c.IsDisabled()).ToList();
            if (!allowFaLine)
                monsters = monsters.Where(c => !c.IsCode(CardId.FormulaAthleteLightning)).ToList();
            else
                monsters = monsters.Where(c => !c.IsCode(CardId.FormulaAthleteLightning) || c.Level == 5).ToList();
            return monsters;
        }

        /// <summary>
        /// Chaos Angel: 10-star. Tuner slot is a tuner or Light/Dark; extra materials are Light/Dark non-tuners.
        /// Prefer Yellow+Medius, then other non-FA pairs, then Librarian+FA at 5 (after ④).
        /// </summary>
        public List<ClientCard> FindChaosAngelMaterials(bool allowFaLine)
        {
            List<ClientCard> monsters = GetChaosAngelMaterialPool(allowFaLine);
            ClientCard yellow = monsters.FirstOrDefault(c => c.IsCode(CardId.ElvenotesYellow));
            ClientCard medius = monsters.FirstOrDefault(c => c.IsCode(CardId.MediusTheInnocent));
            if (ChaosAngelPairOk(yellow, medius))
                return new List<ClientCard> { yellow, medius };

            for (int i = 0; i < monsters.Count; ++i)
            {
                for (int j = i + 1; j < monsters.Count; ++j)
                {
                    if (monsters[i].IsCode(CardId.FormulaAthleteLightning) || monsters[j].IsCode(CardId.FormulaAthleteLightning))
                        continue;
                    if (ChaosAngelPairOk(monsters[i], monsters[j]))
                        return new List<ClientCard> { monsters[i], monsters[j] };
                }
            }

            if (allowFaLine)
            {
                ClientCard librarian = monsters.FirstOrDefault(c => c.IsCode(CardId.SuperLibrarian));
                ClientCard fa = monsters.FirstOrDefault(c => c.IsCode(CardId.FormulaAthleteLightning) && c.Level == 5);
                if (ChaosAngelPairOk(librarian, fa))
                    return new List<ClientCard> { librarian, fa };
            }

            for (int i = 0; i < monsters.Count; ++i)
            {
                for (int j = i + 1; j < monsters.Count; ++j)
                {
                    for (int k = j + 1; k < monsters.Count; ++k)
                    {
                        ClientCard a = monsters[i];
                        ClientCard b = monsters[j];
                        ClientCard c = monsters[k];
                        if (a.Level + b.Level + c.Level != 10) continue;
                        if (!allowFaLine && (a.IsCode(CardId.FormulaAthleteLightning)
                            || b.IsCode(CardId.FormulaAthleteLightning) || c.IsCode(CardId.FormulaAthleteLightning)))
                            continue;
                        bool ok = (IsChaosAngelTunerSlot(a) && IsChaosAngelNonTunerLd(b) && IsChaosAngelNonTunerLd(c))
                            || (IsChaosAngelTunerSlot(b) && IsChaosAngelNonTunerLd(a) && IsChaosAngelNonTunerLd(c))
                            || (IsChaosAngelTunerSlot(c) && IsChaosAngelNonTunerLd(a) && IsChaosAngelNonTunerLd(b));
                        if (ok)
                            return new List<ClientCard> { a, b, c };
                    }
                }
            }
            return null;
        }

        public bool EnemyHasChaosAngelBanishTarget()
        {
            if (BanishRedirectPrevented()) return false;
            foreach (ClientCard card in Enemy.GetMonsters())
            {
                if (card == null || card.HasType(CardType.Token)) continue;
                if (CheckCanBeTargeted(card, true, CardType.Monster))
                    return true;
            }
            foreach (ClientCard card in Enemy.GetSpells())
            {
                if (card == null) continue;
                if (CheckCanBeTargeted(card, true, CardType.Monster))
                    return true;
            }
            return false;
        }

        public bool ShouldSummonChaosAngel()
        {
            if (!Bot.HasInExtra(CardId.ChaosAngel)) return false;
            bool banish = EnemyHasChaosAngelBanishTarget();
            List<ClientCard> mats = FindChaosAngelMaterials(banish);
            if (mats == null) return false;
            if (CanMakeWhite10Synchro() && !ShouldRerouteChickenSixUnderBanish()) return false;
            if (banish) return true;
            if (CanSynchroLevel(10)) return false;
            // No banish: only convert if the materials are weaker than Chaos Angel, or already Main 2.
            if (Duel.Phase == DuelPhase.Main2) return true;
            int atk = 0;
            foreach (ClientCard mat in mats)
            {
                if (mat != null)
                    atk += mat.Attack;
            }
            return atk < 3500;
        }

        public bool NeedCenterCard(int cardId)
        {
            return cardId == CardId.ElvenotesRed || cardId == CardId.ElvenotesBlue || cardId == CardId.ElvenotesYellow
                || cardId == CardId.White7 || cardId == CardId.White10;
        }

        public int CountHandNegates()
        {
            int count = 0;
            count += Bot.Hand.Count(c => c != null && c.IsCode(_CardId.MaxxC));
            count += Bot.Hand.Count(c => c != null && c.IsCode(_CardId.AshBlossom));
            count += Bot.Hand.Count(c => c != null && c.IsCode(_CardId.LockBird));
            count += Bot.Hand.Count(c => c != null && c.IsCode(_CardId.EffectVeiler));
            return count;
        }

        public bool CheckThousandSpearGoingFirstLine()
        {
            // Librarian still live + 4 cards: ① has discard fodder after Accel draws.
            if (Bot.Hand.Count >= 4
                && Bot.GetMonsters().Any(c => c != null && c.IsFaceup() && c.IsCode(CardId.SuperLibrarian) && !c.IsDisabled()))
                return true;
            if (CountHandNegates() < 2) return false;
            if (!(Bot.HasInSpellZone(CardId.RedField, true, true) || Bot.HasInHand(CardId.RedField) || Bot.HasInDeck(CardId.RedField)
                || Bot.GetMonsters().Any(c => c != null && c.IsCode(CardId.ElvenotesYellow))))
                return false;
            return true;
        }

        /// <summary>
        /// Going-second ① needs a card worth banishing: extra (including Link-1), face-up monsters,
        /// face-up Continuous/Equip/Field/Pendulum S/T (not a resolving Normal Spell), GY count ≥ 2,
        /// or the existing danger / problematic lists.
        /// </summary>
        public bool EnemyHasThousandSpearBanishWorth()
        {
            if (Enemy.Graveyard.Count >= 2) return true;
            if (GetDangerousCardinEnemyGrave(false).Count > 0) return true;
            if (GetProblematicEnemyCardList(false, false, 0).Count > 0) return true;
            foreach (ClientCard card in Enemy.GetMonsters())
            {
                if (card == null) continue;
                if (card.HasType(CardType.Fusion | CardType.Ritual | CardType.Synchro | CardType.Xyz | CardType.Link))
                    return true;
                if (card.IsFaceup())
                    return true;
            }
            foreach (ClientCard card in Enemy.GetSpells())
            {
                if (card == null || !card.IsFaceup()) continue;
                if (card.HasType(CardType.Continuous | CardType.Equip | CardType.Field | CardType.Pendulum))
                    return true;
            }
            return false;
        }

        public bool CheckThousandSpearGoingSecondLine()
        {
            // Going-second line exists to fire ①. Empty hand cannot pay the discard cost.
            if (!Bot.Hand.Any(c => c != null))
                return false;
            int banisable = 0;
            foreach (ClientCard card in Enemy.GetMonsters())
            {
                if (card != null) banisable++;
            }
            foreach (ClientCard card in Enemy.GetSpells())
            {
                if (card != null) banisable++;
            }
            banisable += Enemy.Graveyard.Count;
            if (banisable < 2) return false;
            return EnemyHasThousandSpearBanishWorth();
        }

        public List<ClientCard> GetDangerousCardinEnemyGrave(bool onlyMonster = false)
        {
            List<ClientCard> result = Enemy.Graveyard.GetMatchingCards(card =>
                (!onlyMonster || card.IsMonster()) && (card.HasSetcode(SetcodeOrcust) || card.HasSetcode(SetcodePhantomKnight) || card.HasSetcode(SetcodeHorus))).ToList();
            List<int> dangerMonsterIdList = new List<int>
            {
                99937011, 63542003, 9411399, 28954097, 30680659
            };
            result.AddRange(Enemy.Graveyard.GetMatchingCards(card => dangerMonsterIdList.Contains(card.Id)));
            return result;
        }

        public ClientCard GetProblematicEnemyMonster(int attack = 0, bool canBeTarget = false, bool ignoreCurrentDestroy = false, CardType selfType = 0)
        {
            ClientCard floodagateCard = Enemy.GetMonsters().Where(c => c != null && c.Data != null && (ignoreCurrentDestroy || !currentDestroyCardList.Contains(c))
                && c.IsFloodgate() && c.IsFaceup()
                && CheckCanBeTargeted(c, canBeTarget, selfType)).OrderByDescending(card => card.Attack).FirstOrDefault();
            if (floodagateCard != null) return floodagateCard;

            ClientCard dangerCard = Enemy.MonsterZone.Where(c => c != null && c.Data != null && (ignoreCurrentDestroy || !currentDestroyCardList.Contains(c))
                && c.IsMonsterDangerous() && c.IsFaceup() && CheckCanBeTargeted(c, canBeTarget, selfType)).OrderByDescending(card => card.Attack).FirstOrDefault();
            if (dangerCard != null) return dangerCard;

            ClientCard invincibleCard = Enemy.MonsterZone.Where(c => c != null && c.Data != null && (ignoreCurrentDestroy || !currentDestroyCardList.Contains(c))
                && c.IsMonsterInvincible() && c.IsFaceup() && CheckCanBeTargeted(c, canBeTarget, selfType)).OrderByDescending(card => card.Attack).FirstOrDefault();
            if (invincibleCard != null) return invincibleCard;

            ClientCard equippedCard = Enemy.MonsterZone.Where(c => c != null && c.Data != null && (ignoreCurrentDestroy || !currentDestroyCardList.Contains(c))
                && c.EquipCards.Count > 0 && CheckCanBeTargeted(c, canBeTarget, selfType)).OrderByDescending(card => card.Attack).FirstOrDefault();
            if (equippedCard != null) return equippedCard;

            ClientCard enemyExtraMonster = Enemy.MonsterZone.Where(c => c != null && (ignoreCurrentDestroy || !currentDestroyCardList.Contains(c))
                && (c.HasType(CardType.Fusion | CardType.Ritual | CardType.Synchro | CardType.Xyz) || (c.HasType(CardType.Link) && c.LinkCount >= 2))
                && CheckCanBeTargeted(c, canBeTarget, selfType)).OrderByDescending(card => card.Attack).FirstOrDefault();
            if (enemyExtraMonster != null) return enemyExtraMonster;

            if (attack >= 0)
            {
                if (attack == 0)
                    attack = Util.GetBestAttack(Bot);
                ClientCard betterCard = Enemy.MonsterZone.Where(card => card != null
                    && card.GetDefensePower() >= attack && card.GetDefensePower() > 0 && card.IsAttack() && CheckCanBeTargeted(card, canBeTarget, selfType)
                    && (ignoreCurrentDestroy || !currentDestroyCardList.Contains(card))).OrderByDescending(card => card.Attack).FirstOrDefault();
                if (betterCard != null) return betterCard;
            }
            return null;
        }

        public List<ClientCard> GetProblematicEnemyCardList(bool canBeTarget = false, bool ignoreSpells = false, CardType selfType = 0)
        {
            List<ClientCard> resultList = new List<ClientCard>();

            List<ClientCard> floodagateList = Enemy.MonsterZone.Where(c => c != null && c.Data != null && !currentDestroyCardList.Contains(c)
                && c.IsFloodgate() && c.IsFaceup() && CheckCanBeTargeted(c, canBeTarget, selfType)).OrderByDescending(card => card.Attack).ToList();
            if (floodagateList.Count > 0) resultList.AddRange(floodagateList);

            List<ClientCard> problemEnemySpellList = Enemy.SpellZone.Where(c => c != null && c.Data != null && !resultList.Contains(c) && !currentDestroyCardList.Contains(c)
                && c.IsFloodgate() && c.IsFaceup() && CheckCanBeTargeted(c, canBeTarget, selfType)).ToList();
            if (problemEnemySpellList.Count > 0) resultList.AddRange(problemEnemySpellList);

            List<ClientCard> dangerList = Enemy.MonsterZone.Where(c => c != null && c.Data != null && !resultList.Contains(c) && !currentDestroyCardList.Contains(c)
                && c.IsMonsterDangerous() && c.IsFaceup() && CheckCanBeTargeted(c, canBeTarget, selfType)).OrderByDescending(card => card.Attack).ToList();
            if (dangerList.Count > 0
                && (Duel.Player == 0 || (Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2))) resultList.AddRange(dangerList);

            List<ClientCard> invincibleList = Enemy.MonsterZone.Where(c => c != null && c.Data != null && !resultList.Contains(c) && !currentDestroyCardList.Contains(c)
                && c.IsMonsterInvincible() && c.IsFaceup() && CheckCanBeTargeted(c, canBeTarget, selfType)).OrderByDescending(card => card.Attack).ToList();
            if (invincibleList.Count > 0) resultList.AddRange(invincibleList);

            List<ClientCard> enemyMonsters = Enemy.GetMonsters().Where(c => !currentDestroyCardList.Contains(c)).OrderByDescending(card => card.Attack).ToList();
            foreach (ClientCard target in enemyMonsters)
            {
                if ((target.HasType(CardType.Fusion | CardType.Ritual | CardType.Synchro | CardType.Xyz)
                        || (target.HasType(CardType.Link) && target.LinkCount >= 2))
                    && !resultList.Contains(target) && CheckCanBeTargeted(target, canBeTarget, selfType))
                {
                    resultList.Add(target);
                }
            }

            List<ClientCard> spells = Enemy.GetSpells().Where(c => c.IsFaceup() && !currentDestroyCardList.Contains(c)
                && c.HasType(CardType.Equip | CardType.Pendulum | CardType.Field | CardType.Continuous) && CheckCanBeTargeted(c, canBeTarget, selfType)
                && !NotToDestroySpellTrap.Contains(c.Id)).ToList();
            if (spells.Count > 0 && !ignoreSpells) resultList.AddRange(spells);

            return resultList;
        }

        /// <summary>
        /// Baronne ① destroy order. Same ranking as Apophis / Swordsoul GetNormalEnemyTargetList.
        /// </summary>
        public List<ClientCard> GetNormalEnemyTargetList(bool canBeTarget = true, bool ignoreCurrentDestroy = true, CardType selfType = 0)
        {
            List<ClientCard> targetList = GetProblematicEnemyCardList(canBeTarget, false, selfType);
            List<ClientCard> enemyMonster = Enemy.GetMonsters().Where(card => card != null && card.IsFaceup() && !targetList.Contains(card)
                && (!ignoreCurrentDestroy || !currentNegateCardList.Contains(card))
                && !currentDestroyCardList.Contains(card)).ToList();
            enemyMonster.Sort(CardContainer.CompareCardAttack);
            enemyMonster.Reverse();
            targetList.AddRange(enemyMonster);
            targetList.AddRange(Util.ShuffleList(Enemy.GetSpells().Where(card =>
                card != null && (!ignoreCurrentDestroy || !currentNegateCardList.Contains(card))
                && !currentDestroyCardList.Contains(card)
                && enemyPlaceThisTurn.Contains(card) && card.IsFacedown()).ToList()));
            targetList.AddRange(Util.ShuffleList(Enemy.GetSpells().Where(card =>
                card != null && (!ignoreCurrentDestroy || !currentNegateCardList.Contains(card))
                && !currentDestroyCardList.Contains(card)
                && !enemyPlaceThisTurn.Contains(card) && card.IsFacedown()).ToList()));
            targetList.AddRange(Util.ShuffleList(Enemy.GetMonsters().Where(card => card != null && card.IsFacedown()
                && (!ignoreCurrentDestroy || !currentNegateCardList.Contains(card))
                && !currentDestroyCardList.Contains(card)).ToList()));
            return targetList;
        }

        public List<ClientCard> GetMonsterListForTargetNegate(bool canBeTarget = false, CardType selfType = 0)
        {
            // Match Ryzeal / Albaz / Apophis: only pre-negate floodgates, or monsters currently chaining.
            // Do not Veiler every face-up Effect monster (e.g. freshly summoned Yellow).
            List<ClientCard> resultList = new List<ClientCard>();
            if (CheckWhetherNegated())
                return resultList;

            ClientCard target = Enemy.MonsterZone.FirstOrDefault(card => card != null && card.Data != null
                    && card.IsMonsterShouldBeDisabledBeforeItUseEffect() && card.IsFaceup() && !card.IsShouldNotBeTarget()
                    && CheckCanBeTargeted(card, canBeTarget, selfType)
                    && !IsAlreadyMarkedNegate(card));
            if (target != null)
                resultList.Add(target);

            foreach (ClientCard chainingCard in Duel.CurrentChain)
            {
                if (chainingCard == null || chainingCard.Controller != 1) continue;
                if (chainingCard.Location != CardLocation.MonsterZone || chainingCard.IsDisabled()) continue;
                if (!CheckCanBeTargeted(chainingCard, canBeTarget, selfType)) continue;
                if (IsAlreadyMarkedNegate(chainingCard)) continue;
                if (chainingCard.HasPosition(CardPosition.Defence) && DefaultCheckWhetherNumber41IsActive()) continue;
                if (!CheckCardShouldNegate(chainingCard)) continue;
                resultList.Add(chainingCard);
            }

            return resultList;
        }

        public bool IsAlreadyMarkedNegate(ClientCard card)
        {
            if (card == null) return false;
            if (preferNegateCard != null)
            {
                if (preferNegateCard == card) return true;
                if (preferNegateCard.Id == card.Id && preferNegateCard.Controller == card.Controller
                    && preferNegateCard.Location == card.Location && preferNegateCard.Sequence == card.Sequence)
                    return true;
            }
            foreach (ClientCard marked in currentNegateCardList)
            {
                if (marked == null) continue;
                if (marked == card) return true;
                if (marked.Id == card.Id && marked.Controller == card.Controller
                    && marked.Location == card.Location && marked.Sequence == card.Sequence)
                    return true;
            }
            return false;
        }

        public bool ShouldSkipAncientFishDestroy(ClientCard card)
        {
            if (card == null) return true;
            if (card.IsDisabled()) return true;
            if (IsAlreadyMarkedNegate(card)) return true;
            if (currentDestroyCardList.Contains(card)) return true;
            if (redFieldPendingNegateTarget != null)
            {
                if (redFieldPendingNegateTarget == card) return true;
                if (redFieldPendingNegateTarget.Id == card.Id
                    && redFieldPendingNegateTarget.Controller == card.Controller
                    && redFieldPendingNegateTarget.Location == card.Location
                    && redFieldPendingNegateTarget.Sequence == card.Sequence)
                    return true;
            }
            return false;
        }

        public List<ClientCard> GetAncientFishDestroyTargets()
        {
            List<ClientCard> result = new List<ClientCard>();
            List<ClientCard> problems = GetProblematicEnemyCardList(true, false, 0);
            foreach (ClientCard card in problems)
            {
                if (ShouldSkipAncientFishDestroy(card)) continue;
                result.Add(card);
            }
            return result;
        }

        /// <summary>
        /// Same as Apophis swamp: only opponent cards that are face-up on the field on the current chain.
        /// Hand activations (Maxx C, etc.) cannot be hit by Red Field ② (NegateAnyFilter / LOCATION_ONFIELD).
        /// </summary>
        public bool EnemyHasFaceupCardOnChain()
        {
            if (Duel.CurrentChain == null || Duel.CurrentChain.Count == 0) return false;
            foreach (ClientCard card in Duel.CurrentChain)
            {
                if (card == null || card.Controller != 1) continue;
                if (!card.IsOnField() || !card.IsFaceup()) continue;
                if (!CheckCardShouldNegate(card)) continue;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Decide whether Red Field ② should spend the negate.
        /// Chain face-up on-field cards first (Apophis swamp);
        /// sitting floodgate / dangerous / invincible / continuous S/T via GetProblematicEnemyCardList.
        /// Generic sitting Extra (e.g. freshly summoned enemy White 7) is not enough — wait for their activation on chain
        /// (Crystal Wing / Baronne / Red Field can answer then). Do not burn Red Field on summon alone.
        /// </summary>
        public bool EnemyHasWorthNegate()
        {
            if (EnemyHasFaceupCardOnChain())
                return true;
            List<ClientCard> problems = GetProblematicEnemyCardList(false, false, CardType.Monster | CardType.Spell | CardType.Trap);
            foreach (ClientCard card in problems)
            {
                if (card == null || !card.IsFaceup()) continue;
                if (!CheckCardShouldNegate(card)) continue;
                bool serious = card.IsFloodgate() || card.IsMonsterDangerous() || card.IsMonsterInvincible()
                    || (card.IsSpell() || card.IsTrap());
                bool onlyGenericExtra = card.IsMonster()
                    && (card.HasType(CardType.Fusion | CardType.Ritual | CardType.Synchro | CardType.Xyz)
                        || (card.HasType(CardType.Link) && card.LinkCount >= 2))
                    && !serious;
                // Sitting Extra without floodgate: wait for the activation chain (Apophis-style).
                if (onlyGenericExtra)
                    continue;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Red Field disable pick order: chain (newest first), then problematic, then other face-up candidates.
        /// </summary>
        public List<ClientCard> GetRedFieldDisableOrder(IList<ClientCard> cards)
        {
            List<ClientCard> picked = new List<ClientCard>();
            if (Duel.CurrentChain != null)
            {
                for (int i = Duel.CurrentChain.Count - 1; i >= 0; i--)
                {
                    ClientCard chainCard = Duel.CurrentChain[i];
                    if (chainCard == null || chainCard.Controller != 1) continue;
                    if (!chainCard.IsOnField() || !chainCard.IsFaceup()) continue;
                    if (!cards.Contains(chainCard) || picked.Contains(chainCard)) continue;
                    if (!CheckCardShouldNegate(chainCard)) continue;
                    picked.Add(chainCard);
                }
            }
            List<ClientCard> problems = GetProblematicEnemyCardList(false, false, CardType.Monster | CardType.Spell | CardType.Trap);
            foreach (ClientCard card in problems)
            {
                if (card != null && cards.Contains(card) && !picked.Contains(card) && CheckCardShouldNegate(card))
                    picked.Add(card);
            }
            foreach (ClientCard card in cards)
            {
                if (card != null && card.IsFaceup() && card.Controller == 1 && !picked.Contains(card)
                    && CheckCardShouldNegate(card))
                    picked.Add(card);
            }
            foreach (ClientCard card in cards)
            {
                if (card != null && card.IsFaceup() && !picked.Contains(card))
                    picked.Add(card);
            }
            return picked;
        }

        public void SetRedFieldPendingNegate()
        {
            List<ClientCard> negateCandidates = new List<ClientCard>();
            foreach (ClientCard monster in Enemy.GetMonsters())
            {
                if (monster != null && monster.IsFaceup())
                    negateCandidates.Add(monster);
            }
            foreach (ClientCard spell in Enemy.GetSpells())
            {
                if (spell != null && spell.IsFaceup())
                    negateCandidates.Add(spell);
            }
            List<ClientCard> negateOrder = GetRedFieldDisableOrder(negateCandidates);
            if (negateOrder.Count > 0)
            {
                redFieldPendingNegateTarget = negateOrder[0];
                ElfnoteLog("Red Field pending negate " + CardStr(redFieldPendingNegateTarget));
            }
        }

        public ClientCard GetFieldCostMonster(bool forWhite7, bool forWhite10, bool preferThousandSpear, CardAttribute avoidAttribute = 0, bool forNegate = false, bool allowHandTriggers = false)
        {
            if (preferThousandSpear)
            {
                ClientCard spear = Bot.GetMonsters().FirstOrDefault(c => c != null && c.IsCode(CardId.ThousandSpearDragon));
                if (spear != null && CostAttributeOk(spear, forWhite7, forWhite10, avoidAttribute))
                    return spear;
            }

            // Our turn: unused Medius (① and ② still available) before Mulcharmy, so GY ② can continue.
            // Enemy turn Red Field must not dump a Medius that can still GY ②.
            if (Duel.Player == 0)
            {
                ClientCard unusedMediusFirst = Bot.Hand.FirstOrDefault(c => c != null && c.IsCode(CardId.MediusTheInnocent)
                    && !activatedCardIdList.Contains(CardId.MediusTheInnocent)
                    && !activatedCardIdList.Contains(CardId.MediusTheInnocent + 1)
                    && CostAttributeOk(c, forWhite7, forWhite10, avoidAttribute));
                if (unusedMediusFirst != null) return unusedMediusFirst;
            }

            ClientCard mulcharmy = Bot.Hand.FirstOrDefault(c => c != null && c.IsCode(_CardId.MulcharmyFuwalos, _CardId.MulcharmyPurulia, _CardId.MulcharmyNyalus));
            if (mulcharmy != null && CostAttributeOk(mulcharmy, forWhite7, forWhite10, avoidAttribute)) return mulcharmy;

            if (Bot.GetMonsterCount() > 0)
            {
                ClientCard thief = Bot.Hand.FirstOrDefault(c => c != null && c.IsCode(CardId.GreatRighteousThief));
                if (thief != null && CostAttributeOk(thief, forWhite7, forWhite10, avoidAttribute)) return thief;
            }

            ClientCard usedBlue = Bot.GetMonsters().FirstOrDefault(c => c != null && c.IsCode(CardId.ElvenotesBlue) && activatedCardIdList.Contains(CardId.ElvenotesBlue));
            if (usedBlue != null && CostAttributeOk(usedBlue, forWhite7, forWhite10, avoidAttribute)) return usedBlue;

            ClientCard usedRed = Bot.GetMonsters().FirstOrDefault(c => c != null && c.IsCode(CardId.ElvenotesRed) && IsUsedElvenotesBody(c));
            if (usedRed != null && CostAttributeOk(usedRed, forWhite7, forWhite10, avoidAttribute)) return usedRed;

            ClientCard usedYellow = Bot.GetMonsters().FirstOrDefault(c => c != null && c.IsCode(CardId.ElvenotesYellow) && activatedCardIdList.Contains(CardId.ElvenotesYellow));
            if (usedYellow != null && CostAttributeOk(usedYellow, forWhite7, forWhite10, avoidAttribute)
                && !ShouldKeepSixStarForChickenPlus3())
                return usedYellow;

            ClientCard unusedMedius = Bot.Hand.FirstOrDefault(c => c != null && c.IsCode(CardId.MediusTheInnocent)
                && !activatedCardIdList.Contains(CardId.MediusTheInnocent + 1)
                && CostAttributeOk(c, forWhite7, forWhite10, avoidAttribute));
            if (unusedMedius != null) return unusedMedius;

            // Wind ② already used: field Wind is fodder. WIND attr still allows FIRE chicken from Green Field.
            if (activatedCardIdList.Contains(CardId.ElvenotesWind + 1))
            {
                ClientCard usedWind = Bot.GetMonsters().FirstOrDefault(c => c != null
                    && c.IsCode(CardId.ElvenotesWind)
                    && CostAttributeOk(c, forWhite7, forWhite10, avoidAttribute));
                if (usedWind != null) return usedWind;
            }

            if (forNegate)
            {
                ClientCard wind = Bot.Hand.Concat(Bot.GetMonsters()).FirstOrDefault(c => c != null
                    && c.IsCode(CardId.ElvenotesWind) && CostAttributeOk(c, forWhite7, forWhite10, avoidAttribute));
                if (wind != null) return wind;
                ClientCard unusedSix = Bot.Hand.Concat(Bot.GetMonsters()).FirstOrDefault(c => c != null
                    && c.IsCode(CardId.ElvenotesBlue, CardId.ElvenotesYellow, CardId.ElvenotesRed)
                    && CostAttributeOk(c, forWhite7, forWhite10, avoidAttribute));
                if (unusedSix != null) return unusedSix;
            }

            foreach (ClientCard monster in Bot.Hand.Concat(Bot.GetMonsters()))
            {
                if (monster == null || !monster.IsMonster()) continue;
                if (monster.IsCode(CardId.ElvenotesWind, CardId.JailChicken, CardId.ElvenotesRed) && !IsUsedElvenotesBody(monster)
                    && !forNegate)
                    continue;
                if (monster.IsCode(CardId.MediusTheInnocent) && monster.Location == CardLocation.MonsterZone
                    && !activatedCardIdList.Contains(CardId.MediusTheInnocent + 1))
                    continue;
                if (monster.IsCode(_CardId.MaxxC, _CardId.AshBlossom, _CardId.LockBird, _CardId.EffectVeiler)
                    && !allowHandTriggers)
                    continue;
                if (monster.IsCode(CardId.White7, CardId.White10) || monster.HasType(CardType.Synchro))
                    continue;
                if (ShouldKeepSixStarForChickenPlus3() && monster.Location == CardLocation.MonsterZone
                    && IsElvenotesSixStar(monster))
                    continue;
                if (CostAttributeOk(monster, forWhite7, forWhite10, avoidAttribute))
                    return monster;
            }
            return null;
        }

        public bool CostAttributeOk(ClientCard cost, bool forWhite7, bool forWhite10, CardAttribute avoidAttribute = 0)
        {
            if (cost == null) return false;
            if (CheckWhetherBotWillBeBanished(cost)) return false;
            if (avoidAttribute != 0 && cost.HasAttribute(avoidAttribute)) return false;
            if (forWhite7 && cost.HasAttribute(CardAttribute.Fire)) return false;
            if (forWhite10 && cost.HasAttribute(CardAttribute.Light)) return false;
            return true;
        }

        /// <summary>
        /// Red Field cost original attribute must differ from the chosen GY Elvenotes
        /// (script c24092792: GetOriginalAttribute). Do not lock Fire and Light together:
        /// White 7 is FIRE, White 10 is LIGHT, Wind / Great Righteous Thief are WIND and can revive either.
        /// </summary>
        public ClientCard GetRedFieldCostMonster(bool canWhite7, bool canWhite10, bool preferSpear, bool forNegate, CardAttribute avoidAttribute = 0)
        {
            if (forNegate)
            {
                if (canWhite10)
                {
                    ClientCard cost10 = GetFieldCostMonster(false, true, preferSpear, avoidAttribute, forNegate);
                    if (cost10 != null) return cost10;
                }
                if (canWhite7)
                {
                    ClientCard cost7 = GetFieldCostMonster(true, false, preferSpear, avoidAttribute, forNegate);
                    if (cost7 != null) return cost7;
                }
                return GetFieldCostMonster(false, false, preferSpear, avoidAttribute, forNegate);
            }

            if (canWhite7)
            {
                ClientCard cost7 = GetFieldCostMonster(true, false, preferSpear, avoidAttribute, forNegate);
                if (cost7 != null) return cost7;
            }
            if (canWhite10)
            {
                ClientCard cost10 = GetFieldCostMonster(false, true, preferSpear, avoidAttribute, forNegate);
                if (cost10 != null) return cost10;
            }
            return GetFieldCostMonster(false, false, preferSpear, avoidAttribute, forNegate);
        }

        public bool IsGreenFieldEndBoardCost(ClientCard cost)
        {
            if (cost == null) return true;
            if (cost.HasType(CardType.Synchro)) return true;
            return cost.IsCode(CardId.White7, CardId.White10, CardId.ThousandSpearDragon, CardId.CrystalWing,
                CardId.PSYFramelordOmega, CardId.StardustDragon, CardId.SuperLibrarian, CardId.AccelSynchroStardust,
                CardId.FormulaAthleteLightning, CardId.BaronneDeFleur);
        }

        public bool GreenFieldDeckHasSsForCost(ClientCard cost)
        {
            if (cost == null || cost.Data == null) return false;
            int costAttr = cost.Data.Attribute;
            int[] ids = new int[]
            {
                CardId.ElvenotesWind, CardId.JailChicken, CardId.ElvenotesYellow, CardId.ElvenotesBlue, CardId.ElvenotesRed
            };
            foreach (int id in ids)
            {
                if (!Bot.HasInDeck(id)) continue;
                NamedCard data = NamedCard.Get(id);
                if (data == null) continue;
                if ((data.Attribute & costAttr) == 0)
                    return true;
            }
            return false;
        }

        public bool IsGreenFieldAcceptableCost(ClientCard cost, CardAttribute avoidAttribute)
        {
            if (cost == null || !cost.IsMonster()) return false;
            if (IsGreenFieldEndBoardCost(cost)) return false;
            if (!CostAttributeOk(cost, false, false, avoidAttribute)) return false;
            if (!GreenFieldDeckHasSsForCost(cost)) return false;
            // Script GetMZoneCount(tp,c)>0: a hand cost does not free a zone.
            if (cost.Location == CardLocation.Hand && CountFreeMainMonsterZones() < 1)
                return false;
            return true;
        }

        public ClientCard GetGreenFieldCostMonster(CardAttribute avoidAttribute = 0)
        {
            ClientCard preferred = GetFieldCostMonster(false, false, false, avoidAttribute, false, true);
            if (IsGreenFieldAcceptableCost(preferred, avoidAttribute))
                return preferred;

            // Wind ② already used: leftover Wind cannot ② again, so it is legal Green Field fodder
            // (e.g. WIND cost to SS FIRE Red / chicken when that is all the deck has left).
            if (activatedCardIdList.Contains(CardId.ElvenotesWind + 1))
            {
                foreach (ClientCard card in Bot.Hand.Concat(Bot.GetMonsters()))
                {
                    if (card != null && card.IsCode(CardId.ElvenotesWind)
                        && IsGreenFieldAcceptableCost(card, avoidAttribute))
                        return card;
                }
            }

            foreach (ClientCard monster in Bot.Hand.Concat(Bot.GetMonsters()))
            {
                if (!IsGreenFieldAcceptableCost(monster, avoidAttribute)) continue;
                if (monster.IsCode(_CardId.MaxxC, _CardId.AshBlossom, _CardId.LockBird, _CardId.EffectVeiler)
                    && Bot.Hand.Count(c => c != null && c.IsCode(monster.Id)) <= 1)
                    continue;
                return monster;
            }
            return null;
        }

        public bool ShouldKeepInHand(ClientCard card)
        {
            if (card == null) return false;
            if (card.IsCode(_CardId.MaxxC, _CardId.AshBlossom, _CardId.LockBird)) return true;
            if (card.IsCode(CardId.ElvenotesWind)) return true;
            if (card.IsCode(_CardId.EffectVeiler) && Bot.Hand.Count(c => c != null && c.IsCode(_CardId.EffectVeiler)) <= 1)
                return true;
            if (card.IsCode(CardId.ElvenotesRed) && CanElvenotesRedSearch()
                && Bot.Hand.Count(c => c != null && c.IsCode(CardId.ElvenotesRed)) <= 1 && !Bot.HasInMonstersZone(CardId.ElvenotesRed))
                return true;
            if (card.IsCode(CardId.MediusTheInnocent) && !activatedCardIdList.Contains(CardId.MediusTheInnocent + 1)
                && Bot.Hand.Count(c => c != null && c.IsCode(CardId.MediusTheInnocent)) <= 1)
                return true;
            if (card.IsCode(CardId.GreenField) && !Bot.HasInSpellZone(CardId.GreenField) && !activatedCardIdList.Contains(CardId.GreenField))
                return true;
            if (card.IsCode(CardId.RedField) && !Bot.HasInSpellZone(CardId.RedField)) return true;
            if (card.IsCode(CardId.JailChicken) && !Bot.HasInMonstersZone(CardId.JailChicken) && !Bot.HasInGraveyard(CardId.JailChicken)
                && Bot.Hand.Count(c => c != null && c.IsCode(CardId.JailChicken)) <= 1)
                return true;
            return false;
        }

        public IList<ClientCard> SelectElfnoteTributes(IList<ClientCard> cards, int min, int max)
        {
            if (cards == null || cards.Count == 0)
                return null;

            List<ClientCard> ranked = new List<ClientCard>();
            List<ClientCard> problems = GetProblematicEnemyCardList(false, true, CardType.Monster);
            foreach (ClientCard card in problems)
            {
                if (card != null && cards.Contains(card) && !ranked.Contains(card))
                    ranked.Add(card);
            }
            foreach (ClientCard card in cards)
            {
                if (card != null && card.Controller == 1 && !ranked.Contains(card))
                    ranked.Add(card);
            }

            ClientCard unusedMedius = null;
            foreach (ClientCard card in cards)
            {
                if (card != null && card.Controller == 0 && card.IsCode(CardId.MediusTheInnocent)
                    && !activatedCardIdList.Contains(CardId.MediusTheInnocent) && !ranked.Contains(card))
                {
                    unusedMedius = card;
                    break;
                }
            }
            if (unusedMedius != null)
                ranked.Add(unusedMedius);

            foreach (ClientCard card in cards)
            {
                if (card != null && card.Controller == 0
                    && card.IsCode(_CardId.MulcharmyFuwalos, _CardId.MulcharmyPurulia, _CardId.MulcharmyNyalus)
                    && !ranked.Contains(card))
                    ranked.Add(card);
            }

            foreach (ClientCard card in cards)
            {
                if (card != null && card.Controller == 0 && card.IsCode(CardId.MediusTheInnocent) && !ranked.Contains(card))
                    ranked.Add(card);
            }

            foreach (ClientCard card in cards)
            {
                if (card == null || ranked.Contains(card)) continue;
                if (card.Location == CardLocation.Hand && ShouldKeepInHand(card)) continue;
                ranked.Add(card);
            }

            foreach (ClientCard card in cards)
            {
                if (card != null && !ranked.Contains(card))
                    ranked.Add(card);
            }

            IList<ClientCard> picked = Util.CheckSelectCount(ranked, cards, min, max);
            if (picked != null && picked.Count > 0)
            {
                List<string> names = new List<string>();
                foreach (ClientCard card in picked)
                    names.Add(CardStr(card));
                ElfnoteLog("tribute " + string.Join(",", names.ToArray()));
            }
            return picked;
        }

        public bool CanMediusGySsAfterThiefTribute()
        {
            if (activatedCardIdList.Contains(CardId.MediusTheInnocent + 1))
                return false;
            if (CheckShouldNoMoreSpSummon(CardLocation.Grave))
                return false;

            List<ClientCard> medius = new List<ClientCard>();
            List<ClientCard> otherMonsters = new List<ClientCard>();
            foreach (ClientCard card in Bot.Hand)
            {
                if (card == null || !card.IsMonster()) continue;
                if (card.IsCode(CardId.GreatRighteousThief)) continue;
                if (card.IsCode(CardId.MediusTheInnocent))
                    medius.Add(card);
                else
                    otherMonsters.Add(card);
            }
            if (medius.Count == 0)
                return false;
            if (!medius.Any(c => c != null && !CheckWhetherBotWillBeBanished(c)))
                return false;

            int need = 2;
            int enemy = Enemy.GetMonsterCount();
            if (enemy > need)
                enemy = need;
            need -= enemy;
            if (need <= 0)
                return false;

            need -= 1;
            int extraMedius = medius.Count - 1;
            int dumps = otherMonsters.Count;
            if (need > dumps + extraMedius)
                return false;

            int dumpsLeft = dumps;
            int extraLeft = extraMedius;
            if (need > 0)
            {
                int useDump = need < dumpsLeft ? need : dumpsLeft;
                dumpsLeft -= useDump;
                need -= useDump;
            }
            if (need > 0)
                extraLeft -= need;
            return dumpsLeft + extraLeft > 0;
        }

        public IList<ClientCard> SelectMediusToDeck(IList<ClientCard> cards, int min, int max)
        {
            bool keepWhite10Mat = CanChickenLevelForWhite10() || ShouldPreferWhite10Synchro()
                || (ShouldRerouteChickenSixUnderBanish() && CanChickenPlus3Baronne());
            ClientCard center = GetCenterMonster();
            List<ClientCard> ranked = new List<ClientCard>();
            // NS already spent: bounce the hand chicken so ① / White 10 / Wind ② can SS it from deck.
            // Never bounce a chicken that is already on the field.
            ClientCard handChicken = null;
            foreach (ClientCard card in cards)
            {
                if (card != null && card.Location == CardLocation.Hand && card.IsCode(CardId.JailChicken))
                {
                    handChicken = card;
                    break;
                }
            }
            if (!CheckWhetherCanSummon() && handChicken != null)
            {
                ranked.Add(handChicken);
                ElfnoteLog("Medius GY bounce hand chicken, NS already used " + CardStr(handChicken));
                return Util.CheckSelectCount(ranked, cards, min, max);
            }
            foreach (ClientCard card in cards)
            {
                if (card != null && card.Location == CardLocation.MonsterZone && IsUsedElvenotesBody(card) && !ranked.Contains(card))
                {
                    if (keepWhite10Mat && center != null && card.Equals(center)) continue;
                    ranked.Add(card);
                }
            }
            foreach (ClientCard card in cards)
            {
                if (card != null && card.Location == CardLocation.Hand && card.IsCode(CardId.MediusTheInnocent) && !ranked.Contains(card))
                    ranked.Add(card);
            }
            foreach (ClientCard card in cards)
            {
                if (card != null && card.IsCode(_CardId.MulcharmyFuwalos, _CardId.MulcharmyPurulia, _CardId.MulcharmyNyalus) && !ranked.Contains(card))
                    ranked.Add(card);
            }
            foreach (ClientCard card in cards)
            {
                if (card == null || ranked.Contains(card)) continue;
                if (card.IsCode(CardId.JailChicken, CardId.ElvenotesWind, CardId.GreatRighteousThief)) continue;
                if (keepWhite10Mat && center != null && card.Equals(center)) continue;
                ranked.Add(card);
            }
            foreach (ClientCard card in cards)
            {
                if (card != null && !ranked.Contains(card))
                    ranked.Add(card);
            }
            if (ranked.Count > 0)
                ElfnoteLog("Medius GY bounce " + CardStr(ranked[0]));
            return Util.CheckSelectCount(ranked, cards, min, max);
        }

        public IList<ClientCard> SelectHandDiscard(IList<ClientCard> cards, int min, int max)
        {
            List<int> dumpFirst = new List<int>
            {
                _CardId.MulcharmyFuwalos, _CardId.MulcharmyPurulia, _CardId.MulcharmyNyalus,
                CardId.ElvenotesBlue, CardId.ElvenotesYellow, CardId.GreatRighteousThief, CardId.InnocentArt, CardId.GreenField
            };
            List<ClientCard> ranked = new List<ClientCard>();
            foreach (int id in dumpFirst)
            {
                foreach (ClientCard card in cards)
                {
                    if (card != null && card.IsCode(id) && !ShouldKeepInHand(card) && !ranked.Contains(card))
                        ranked.Add(card);
                }
            }
            foreach (ClientCard card in cards)
            {
                if (card != null && !ShouldKeepInHand(card) && !ranked.Contains(card))
                    ranked.Add(card);
            }
            foreach (ClientCard card in cards)
            {
                if (card != null && !ranked.Contains(card))
                    ranked.Add(card);
            }
            return Util.CheckSelectCount(ranked, cards, min, max);
        }

        public bool ComboIsStuck()
        {
            bool hasSix = Bot.HasInHand(CardId.ElvenotesRed) || Bot.HasInHand(CardId.ElvenotesBlue) || Bot.HasInHand(CardId.ElvenotesYellow)
                || Bot.HasInMonstersZone(CardId.ElvenotesRed) || Bot.HasInMonstersZone(CardId.ElvenotesBlue) || Bot.HasInMonstersZone(CardId.ElvenotesYellow)
                || Bot.HasInMonstersZone(CardId.ElvenotesWind);
            bool hasChicken = Bot.HasInHand(CardId.JailChicken) || Bot.HasInMonstersZone(CardId.JailChicken) || Bot.HasInGraveyard(CardId.JailChicken)
                || Bot.HasInDeck(CardId.JailChicken);
            if (!hasSix || !hasChicken) return true;
            if (!DefaultCheckWhetherBotCanSearch() || enemyResolvedEffectIdList.Contains(_CardId.AshBlossom))
            {
                if (!Bot.GetMonsters().Any(c => c != null && c.HasType(CardType.Synchro)))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Wind ③ recycle (synchro material → hand) is once per turn; track with Wind+2.
        /// </summary>
        public bool WindGyRecycleAvailable()
        {
            return !activatedCardIdList.Contains(CardId.ElvenotesWind + 2);
        }

        /// <summary>
        /// Accel ② second extra pick: unused Wind ③ and chicken+Wind can make a 7-star that eats Wind.
        /// </summary>
        public bool CanAccelSecondSynchroUseWindForSeven()
        {
            if (!WindGyRecycleAvailable()) return false;
            ClientCard wind = Bot.GetMonsters().FirstOrDefault(c => c != null && c.IsFaceup() && c.IsCode(CardId.ElvenotesWind));
            if (wind == null) return false;
            ClientCard chicken = Bot.GetMonsters().FirstOrDefault(c => c != null && c.IsFaceup() && c.IsCode(CardId.JailChicken));
            if (chicken == null) return false;
            return chicken.Level + wind.Level == 7;
        }

        /// <summary>
        /// Accel ② Formula/White 7 materials: chicken + Wind from this candidate list (by code).
        /// Do not queue Bot.GetMonsters() objects — CardSelector.Contains misses them and fills Red.
        /// </summary>
        public IList<ClientCard> SelectAccel2WindChickenMaterials(IList<ClientCard> cards, int min, int max)
        {
            if (!accel2PreferWindChicken || cards == null || cards.Count == 0)
                return null;
            List<ClientCard> ranked = new List<ClientCard>();
            foreach (ClientCard card in cards)
            {
                if (card != null && card.IsCode(CardId.ElvenotesWind) && !ranked.Contains(card))
                    ranked.Add(card);
            }
            foreach (ClientCard card in cards)
            {
                if (card != null && card.IsCode(CardId.JailChicken) && !ranked.Contains(card))
                    ranked.Add(card);
            }
            if (ranked.Count == 0)
                return null;
            foreach (ClientCard card in cards)
            {
                if (card == null || ranked.Contains(card)) continue;
                if (card.IsCode(CardId.ElvenotesRed, CardId.ElvenotesBlue, CardId.ElvenotesYellow))
                    continue;
                ranked.Add(card);
            }
            foreach (ClientCard card in cards)
            {
                if (card != null && !ranked.Contains(card))
                    ranked.Add(card);
            }
            ElfnoteLog("Accel ② synchro mats prefer Wind+chicken first=" + CardStr(ranked[0])
                + " min=" + min + " max=" + max);
            return Util.CheckSelectCount(ranked, cards, min, max);
        }

        /// <summary>
        /// Prefer unused Wind as a 6-star synchro material so its GY recycle can fire.
        /// </summary>
        public List<ClientCard> PreferWindAmongNonTuners(List<ClientCard> nonTuners)
        {
            if (!WindGyRecycleAvailable()) return nonTuners;
            List<ClientCard> preferred = new List<ClientCard>();
            foreach (ClientCard card in nonTuners)
            {
                if (card != null && card.IsCode(CardId.ElvenotesWind))
                    preferred.Add(card);
            }
            foreach (ClientCard card in nonTuners)
            {
                if (card != null && !preferred.Contains(card))
                    preferred.Add(card);
            }
            return preferred;
        }

        public List<ClientCard> FindSynchroMaterials(int level, bool avoidCrystalWing, bool forBaronne = false)
        {
            List<ClientCard> monsters = Bot.GetMonsters().Where(c => c != null && c.IsFaceup()).ToList();
            if (avoidCrystalWing)
                monsters = monsters.Where(c => !c.IsCode(CardId.CrystalWing)).ToList();
            // Formula / Baronne are end-board bodies; do not eat them for further synchros.
            // Do not also drop Omega: Assault Blackwing (9) is often chicken + Omega.
            // Chaos Angel uses FindChaosAngelMaterials, not this pool.
            monsters = monsters.Where(c => !c.IsCode(CardId.FormulaAthleteLightning, CardId.BaronneDeFleur)).ToList();
            if (ShouldKeepAssaultBlackwingForBattle())
            {
                monsters = monsters.Where(c => !c.IsCode(CardId.AssaultBlackwing) || c.IsDisabled()).ToList();
                ElfnoteLog("keep Assault Blackwing for battle, not synchro material");
            }
            if (ShouldKeepThousandSpearForRedFieldCost()
                && !(forBaronne && ShouldAllowThousandSpearAsBaronneMaterial()))
            {
                monsters = monsters.Where(c => !c.IsCode(CardId.ThousandSpearDragon)).ToList();
                ElfnoteLog("keep Thousand Spear for opponent-turn Red Field ③, forBaronne=" + forBaronne);
            }
            List<ClientCard> tuners = monsters.Where(c => c.IsTuner()).ToList();
            List<ClientCard> nonTuners = PreferWindAmongNonTuners(monsters.Where(c => !c.IsTuner()).ToList());
            // Accel ② follow-up 9-star: Stardust + chicken. Keep Omega on board.
            if (level == 9)
            {
                ClientCard stardust = monsters.FirstOrDefault(c => c != null && c.IsCode(CardId.StardustDragon));
                ClientCard chicken = monsters.FirstOrDefault(c => c != null && c.IsCode(CardId.JailChicken) && c.IsTuner());
                if (stardust != null && chicken != null && chicken.Level + stardust.Level == 9)
                {
                    ElfnoteLog("synchro 9 prefer Stardust+chicken, keep Omega");
                    return new List<ClientCard> { chicken, stardust };
                }
            }
            foreach (ClientCard tuner in tuners)
            {
                foreach (ClientCard nonTuner in nonTuners)
                {
                    if (tuner.Level + nonTuner.Level == level)
                    {
                        List<ClientCard> mats = new List<ClientCard>();
                        mats.Add(tuner);
                        mats.Add(nonTuner);
                        return mats;
                    }
                }
            }
            foreach (ClientCard tuner in tuners)
            {
                for (int i = 0; i < nonTuners.Count; ++i)
                {
                    for (int j = i + 1; j < nonTuners.Count; ++j)
                    {
                        if (tuner.Level + nonTuners[i].Level + nonTuners[j].Level == level)
                        {
                            List<ClientCard> mats = new List<ClientCard>();
                            mats.Add(tuner);
                            mats.Add(nonTuners[i]);
                            mats.Add(nonTuners[j]);
                            return mats;
                        }
                    }
                }
            }
            // Two tuners + one non-tuner (chicken + chicken + 6-star = 8 Omega).
            for (int i = 0; i < tuners.Count; ++i)
            {
                for (int j = i + 1; j < tuners.Count; ++j)
                {
                    foreach (ClientCard nonTuner in nonTuners)
                    {
                        if (tuners[i].Level + tuners[j].Level + nonTuner.Level == level)
                        {
                            List<ClientCard> mats = new List<ClientCard>();
                            mats.Add(tuners[i]);
                            mats.Add(tuners[j]);
                            mats.Add(nonTuner);
                            return mats;
                        }
                    }
                }
            }
            return null;
        }

        public bool SelectSynchroMaterials(int level, bool avoidCrystalWing, bool forBaronne = false)
        {
            List<ClientCard> mats = FindSynchroMaterials(level, avoidCrystalWing, forBaronne);
            if (mats == null) return false;
            AI.SelectMaterials(mats);
            return true;
        }

        /// <summary>
        /// Original level used by Helldive Bomber damage (script GetOriginalLevel). Xyz/Link contribute 0.
        /// </summary>
        public int BomberDestroyLevel(ClientCard card)
        {
            if (card == null || !card.IsFaceup()) return 0;
            if (card.HasType(CardType.Xyz | CardType.Link)) return 0;
            if (card.Data != null && card.Data.Level > 0) return card.Data.Level;
            return card.Level > 0 ? card.Level : 0;
        }

        /// <summary>
        /// Green Field makes our Sequence 2 monster cannot be destroyed by bomber.
        /// </summary>
        public bool BomberEffectWouldDestroy(ClientCard card)
        {
            if (card == null || !card.IsFaceup()) return false;
            if (card.Controller == 0 && card.Sequence == 2 && Bot.HasInSpellZone(CardId.GreenField, true, true))
                return false;
            return true;
        }

        public int EstimateBomberDamage(IList<ClientCard> leaving = null, bool includeBomber = false)
        {
            int levelSum = 0;
            foreach (ClientCard card in Bot.GetMonsters())
            {
                if (card == null) continue;
                if (leaving != null && leaving.Contains(card)) continue;
                if (!BomberEffectWouldDestroy(card)) continue;
                levelSum += BomberDestroyLevel(card);
            }
            foreach (ClientCard card in Enemy.GetMonsters())
            {
                if (card == null) continue;
                if (leaving != null && leaving.Contains(card)) continue;
                if (!BomberEffectWouldDestroy(card)) continue;
                levelSum += BomberDestroyLevel(card);
            }
            if (includeBomber)
            {
                NamedCard bomber = NamedCard.Get(CardId.HelldiveBomber);
                levelSum += bomber != null && bomber.Level > 0 ? bomber.Level : 7;
            }
            return levelSum * 200;
        }

        /// <summary>
        /// Skip Battle to Main 2 when Bomber ① would reduce enemy LP to 0.
        /// Gate resolved: non-Jail cannot attack, skip if lethal. Gate not resolved (or negated):
        /// skip only if the opponent still has a monster to convert into Bomber damage.
        /// </summary>
        public bool ShouldSkipBattleForBomber()
        {
            if (!HelldiveBomberDestroyEffectCanApply()) return false;
            foreach (ClientCard monster in Enemy.GetMonsters())
            {
                if (monster == null || monster.Data == null || !monster.IsFaceup()) continue;
                if (monster.IsFloodgate())
                    return false;
            }

            int dmg;
            bool bomberOnField = Bot.GetMonsters().Any(c => c != null && c.IsFaceup()
                && c.IsCode(CardId.HelldiveBomber) && !c.IsDisabled());
            if (bomberOnField)
                dmg = EstimateBomberDamage();
            else if (HasChickenAndLevel6OnField() && Bot.HasInExtra(CardId.HelldiveBomber))
            {
                List<ClientCard> mats = FindSynchroMaterials(7, true);
                if (mats == null) return false;
                dmg = EstimateBomberDamage(mats, true);
            }
            else
                return false;

            if (dmg < Enemy.LifePoints) return false;
            if (jailGateResolvedThisTurn) return true;
            return Enemy.GetMonsterCount() > 0;
        }

        /// <summary>
        /// Helldive Bomber ① is once per turn. Skill Drain / Number 41 in Defense would stop it.
        /// </summary>
        public bool HelldiveBomberDestroyEffectCanApply()
        {
            if (activatedCardIdList.Contains(CardId.HelldiveBomber)) return false;
            if (ElvenotesSynchroSecondWouldBeNegatedOnSummon(CardId.HelldiveBomber)) return false;
            return true;
        }

        public bool ShouldDeferSynchroForHelldiveBomber()
        {
            if (Duel.Turn == 1) return false;
            if (!HelldiveBomberDestroyEffectCanApply()) return false;
            if (!Bot.HasInExtra(CardId.HelldiveBomber)) return false;
            if (Bot.GetMonsters().Any(c => c != null && c.IsFaceup() && c.IsCode(CardId.HelldiveBomber) && !c.IsDisabled()))
                return false;
            List<ClientCard> mats = FindSynchroMaterials(7, true);
            if (mats == null) return false;
            return EstimateBomberDamage(mats, true) > Enemy.LifePoints;
        }

        public bool JailGateCannotRemove()
        {
            return BanishRedirectPrevented();
        }

        public bool JailGateGyBanishReady()
        {
            if (activatedCardIdList.Contains(CardId.JailGodGate + 1)) return false;
            if (!Bot.HasInGraveyard(CardId.JailGodGate)) return false;
            if (JailGateCannotRemove()) return false;
            return true;
        }

        public bool EnemyMonsterAcceptsJailGateBanish(ClientCard defender)
        {
            if (defender == null || !defender.IsMonster()) return false;
            if (defender.HasType(CardType.Token)) return false;
            if (JailGateCannotRemove()) return false;
            return true;
        }

        public ClientCard PickJailGateBanishTarget(IList<ClientCard> defenders)
        {
            if (defenders == null) return null;
            ClientCard best = null;
            int bestPower = -1;
            foreach (ClientCard defender in defenders)
            {
                if (!EnemyMonsterAcceptsJailGateBanish(defender)) continue;
                int power = defender.GetDefensePower();
                if (best == null || power > bestPower)
                {
                    best = defender;
                    bestPower = power;
                }
            }
            return best;
        }

        public IList<ClientCard> PreferCards(IList<ClientCard> cards, IList<int> idOrder, int min, int max)
        {
            List<ClientCard> selected = new List<ClientCard>();
            foreach (int id in idOrder)
            {
                foreach (ClientCard card in cards)
                {
                    if (card != null && card.IsCode(id) && !selected.Contains(card))
                    {
                        selected.Add(card);
                        if (selected.Count >= max) return Util.CheckSelectCount(selected, cards, min, max);
                    }
                }
            }
            // SelectUnselect finish (min=0): keep preferred matches only; do not pad with leftovers.
            if (min == 0)
            {
                if (selected.Count == 0)
                    return new List<ClientCard>();
                return Util.CheckSelectCount(selected, cards, min, max);
            }
            foreach (ClientCard card in cards)
            {
                if (!selected.Contains(card))
                {
                    selected.Add(card);
                    if (selected.Count >= max) break;
                }
            }
            return Util.CheckSelectCount(selected, cards, min, max);
        }

        public bool HasUsableOneStar()
        {
            return Bot.HasInHand(CardId.JailChicken) || Bot.HasInMonstersZone(CardId.JailChicken)
                || Bot.HasInGraveyard(CardId.JailChicken);
        }

        public bool AlreadyHasWhitePendulum()
        {
            return Bot.HasInHand(CardId.WhitePendulum) || Bot.HasInSpellZone(CardId.WhitePendulum);
        }

        /// <summary>
        /// Red ② may search White P only when the draw can fire after placing it, from an
        /// imminent 0x1d8/0x1ce SS that does not depend on this search. Do not use
        /// CanTriggerWhitePendulumThisTurn (that predicts Green Field / Wind ②).
        /// </summary>
        public bool RedSearchWhitePCanTriggerDraw()
        {
            if (!DefaultCheckWhetherBotCanDraw()) return false;
            if (!NeedPendulumScale()) return false;
            if (AlreadyHasWhitePendulum()) return false;

            bool skipHandSs = CheckShouldNoMoreSpSummon(CardLocation.Hand) && CheckHasStopBoard();
            bool skipWhite10 = CheckShouldNoMoreSpSummon(CardLocation.Deck) && CheckHasStopBoard();

            if (White10OnFieldSecondEffectReady() && !skipWhite10)
                return true;
            if (HasChickenAndLevel6OnField() && Bot.HasInExtra(CardId.White7))
                return true;
            bool white7Ready = Bot.GetMonsters().Any(c => c != null && c.IsCode(CardId.White7)
                && c.IsFaceup() && !c.IsDisabled())
                && !activatedCardIdList.Contains(CardId.White7)
                && (Duel.Phase == DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2);
            if (white7Ready)
            {
                bool hasPull = Bot.Hand.Any(c => c != null && c.HasSetcode(SetcodeElvenotes) && c.Level <= 6)
                    || Bot.Graveyard.Any(c => c != null && c.HasSetcode(SetcodeElvenotes) && c.Level <= 6);
                if (hasPull) return true;
            }
            if (!skipHandSs && IsCenterEmpty())
            {
                if (ChickenGySearchWillAddHandSixStar()) return true;
                if (HandHasSixStarWaitingToSs()) return true;
            }
            return false;
        }

        public int CountOwnedWhitePendulum()
        {
            int count = 0;
            foreach (ClientCard card in Bot.Hand)
            {
                if (card != null && card.IsCode(CardId.WhitePendulum))
                    count++;
            }
            foreach (ClientCard card in Bot.GetSpells())
            {
                if (card != null && card.IsCode(CardId.WhitePendulum))
                    count++;
            }
            return count;
        }

        public bool CanTriggerWhitePendulumThisTurn()
        {
            if (!DefaultCheckWhetherBotCanDraw()) return false;
            if (activatedCardIdList.Contains(CardId.WhitePendulum)) return false;
            if (!NeedPendulumScale() && !Bot.HasInSpellZone(CardId.WhitePendulum) && !Bot.HasInHand(CardId.WhitePendulum))
                return false;

            if (WantGreenFieldSummonWind() && !CheckShouldNoMoreSpSummon(CardLocation.Deck))
            {
                bool haveGreen = Bot.HasInSpellZone(CardId.GreenField) || Bot.HasInHand(CardId.GreenField);
                bool canPlaceGreen = Bot.HasInDeck(CardId.GreenField)
                    && (Bot.HasInHand(CardId.ElvenotesBlue) || Bot.HasInMonstersZone(CardId.ElvenotesBlue)
                        || Bot.HasInDeck(CardId.ElvenotesBlue));
                if (haveGreen || canPlaceGreen) return true;
            }
            if (Bot.HasInMonstersZone(CardId.ElvenotesWind) && Bot.HasInDeck(CardId.ElvenotesYellow)
                && !CheckShouldNoMoreSpSummon(CardLocation.Deck))
                return true;
            if (Bot.HasInMonstersZone(CardId.White7))
            {
                if (Bot.HasInHand(CardId.JailChicken) || Bot.HasInGraveyard(CardId.JailChicken))
                    return true;
                if (Bot.Hand.Any(c => c != null && c.HasSetcode(SetcodeElvenotes) && c.Level <= 6))
                    return true;
                if (Bot.Graveyard.Any(c => c != null && c.HasSetcode(SetcodeElvenotes) && c.Level <= 6))
                    return true;
            }
            if (Bot.HasInSpellZone(CardId.RedField, true, true)
                && Bot.Graveyard.Any(c => c != null && (c.HasSetcode(SetcodeElvenotes) || c.IsCode(CardId.JailChicken))))
                return true;
            return false;
        }

        public IList<ClientCard> SelectWhite7SpSummon(IList<ClientCard> cards, int min, int max)
        {
            List<ClientCard> ranked = new List<ClientCard>();
            bool canEight = HasEightSynchroInExtra();
            List<int> sixStars = new List<int> { CardId.ElvenotesBlue, CardId.ElvenotesRed, CardId.ElvenotesWind, CardId.ElvenotesYellow };
            // Opponent turn: Wind → RBY swap → chicken synchro → hand RBY → MainEnd body.
            if (Duel.Player == 1)
            {
                bool centerFilled = GetCenterMonster() != null;
                // 1. GY Wind for ②
                if (IsCenterEmpty() && !activatedCardIdList.Contains(CardId.ElvenotesWind + 1)
                    && (WantWindEnemyTurnSsChicken() || DeckHasRbySwapTarget()))
                {
                    foreach (ClientCard card in cards)
                    {
                        if (card != null && card.IsCode(CardId.ElvenotesWind) && card.Location == CardLocation.Grave
                            && !ranked.Contains(card))
                            ranked.Add(card);
                    }
                }
                // 2. GY Red-Blue-Yellow that can swap (need center filled). Skip a name whose
                // face-up copy will already spend the once-per-turn ③.
                if (centerFilled)
                {
                    List<int> rbyOrder = new List<int> { CardId.ElvenotesRed, CardId.ElvenotesBlue, CardId.ElvenotesYellow };
                    foreach (int id in rbyOrder)
                    {
                        if (!GyRbyWorthWhite7Pull(id)) continue;
                        foreach (ClientCard card in cards)
                        {
                            if (card != null && card.IsCode(id) && card.Location == CardLocation.Grave && !ranked.Contains(card))
                                ranked.Add(card);
                        }
                    }
                }
                // 3. GY chicken for synchro follow-up
                if (!activatedCardIdList.Contains(CardId.JailChicken + 1)
                    && (canEight || White10EffectStillAvailable()))
                {
                    foreach (ClientCard card in cards)
                    {
                        if (card != null && card.IsCode(CardId.JailChicken) && card.Location == CardLocation.Grave
                            && !ranked.Contains(card))
                            ranked.Add(card);
                    }
                }
                // 4. Hand RBY only (not chicken, not Wind) when GY had no hit
                if (ranked.Count == 0 && centerFilled)
                {
                    List<int> rbyOrder = new List<int> { CardId.ElvenotesRed, CardId.ElvenotesBlue, CardId.ElvenotesYellow };
                    foreach (int id in rbyOrder)
                    {
                        if (!CanRbySwapNow(id, true) || HasFaceupRbyReadyToSwap(id)) continue;
                        foreach (ClientCard card in cards)
                        {
                            if (card != null && card.IsCode(id) && card.Location == CardLocation.Hand && !ranked.Contains(card))
                                ranked.Add(card);
                        }
                    }
                }
                // 5. MainEnd fallback: chicken if safe for next 7+1, else highest ATK GY
                if (ranked.Count == 0 && (CurrentTiming & HintTimingMainEnd) != 0)
                {
                    bool main2End = Duel.Phase == DuelPhase.Main2;
                    bool pullChicken = canEight && (main2End || !EnemyHasMonsterThatBeatsChicken());
                    if (pullChicken)
                    {
                        foreach (ClientCard card in cards)
                        {
                            if (card != null && card.IsCode(CardId.JailChicken) && card.Location == CardLocation.Grave
                                && !ranked.Contains(card))
                                ranked.Add(card);
                        }
                    }
                    List<ClientCard> gyBodies = cards.Where(c => c != null && c.Location == CardLocation.Grave
                        && c.HasSetcode(SetcodeElvenotes) && c.Level <= 6 && !ranked.Contains(c))
                        .OrderByDescending(c => c.Attack).ToList();
                    ranked.AddRange(gyBodies);
                }
                // Never pad hand Wind / hand chicken. Spec: hand is RBY only, and only if GY missed.
                foreach (ClientCard card in cards)
                {
                    if (card == null || ranked.Contains(card)) continue;
                    if (card.Location != CardLocation.Grave) continue;
                    if (!card.HasSetcode(SetcodeElvenotes) || card.Level > 6) continue;
                    if (card.IsCode(CardId.ElvenotesWind)) continue;
                    ranked.Add(card);
                }
                if (ranked.Count > 0)
                    ElfnoteLog("White7 SS pick enemyTurn " + CardStr(ranked[0]));
                else
                    ElfnoteLog("White7 SS pick enemyTurn empty ranked, CheckSelectCount will pad");
                return Util.CheckSelectCount(ranked, cards, min, max);
            }
            // Own turn: No leftover 8-star → SS a GY 6-star to continue ②, not a chicken that cannot 7+1.
            if (!canEight)
            {
                foreach (int id in sixStars)
                {
                    foreach (ClientCard card in cards)
                    {
                        if (card != null && card.IsCode(id) && card.Location == CardLocation.Grave && !ranked.Contains(card))
                            ranked.Add(card);
                    }
                }
                foreach (int id in sixStars)
                {
                    foreach (ClientCard card in cards)
                    {
                        if (card != null && card.IsCode(id) && card.Location == CardLocation.Hand && !ranked.Contains(card))
                            ranked.Add(card);
                    }
                }
            }
            foreach (ClientCard card in cards)
            {
                if (card != null && card.IsCode(CardId.JailChicken) && card.Location == CardLocation.Grave && !ranked.Contains(card))
                    ranked.Add(card);
            }
            foreach (ClientCard card in cards)
            {
                if (card != null && card.Location == CardLocation.Grave && card.Level <= 1 && !ranked.Contains(card))
                    ranked.Add(card);
            }
            foreach (ClientCard card in cards)
            {
                if (card != null && card.IsCode(CardId.JailChicken) && card.Location == CardLocation.Hand && !ranked.Contains(card))
                    ranked.Add(card);
            }
            foreach (ClientCard card in cards)
            {
                if (card != null && card.Location == CardLocation.Hand && card.Level <= 1 && !ranked.Contains(card))
                    ranked.Add(card);
            }
            if (canEight)
            {
                foreach (int id in sixStars)
                {
                    foreach (ClientCard card in cards)
                    {
                        if (card != null && card.IsCode(id) && card.Location == CardLocation.Grave && !ranked.Contains(card))
                            ranked.Add(card);
                    }
                }
                foreach (int id in sixStars)
                {
                    foreach (ClientCard card in cards)
                    {
                        if (card != null && card.IsCode(id) && card.Location == CardLocation.Hand && !ranked.Contains(card))
                            ranked.Add(card);
                    }
                }
            }
            foreach (ClientCard card in cards)
            {
                if (card != null && !ranked.Contains(card))
                    ranked.Add(card);
            }
            if (ranked.Count > 0)
                ElfnoteLog("White7 SS pick " + CardStr(ranked[0]) + " canEight=" + canEight);
            return Util.CheckSelectCount(ranked, cards, min, max);
        }

        /// <summary>
        /// Wind ② SS from deck. Own turn: if there is no tuner, always call Chicken for 6+1 White 7.
        /// Do not call a second Blue after Blue ② is already used (once per turn).
        /// If Blue is already face-up and ② is still free, that Blue will place the Continuous —
        /// call Chicken instead of a second Blue that cannot place.
        /// Yellow is for placing Red Field face-up; a copy in hand is still a trap and cannot be activated.
        /// With a tuner and Red Field already on the field, unused Red ② is the extender.
        /// Opponent turn: chicken for White 10 line → Red-Blue-Yellow that passes ③ → highest ATK.
        /// </summary>
        public IList<ClientCard> SelectWindDeckSpSummon(IList<ClientCard> cards, int min, int max)
        {
            if (Duel.Player == 1)
            {
                List<ClientCard> ranked = new List<ClientCard>();
                bool wantChicken = WantWindEnemyTurnSsChicken();
                bool w10Avail = White10EffectStillAvailable();
                bool w10Used = activatedCardIdList.Contains(CardId.White10);
                bool redSwap = CanRbySwapNow(CardId.ElvenotesRed, true);
                bool blueSwap = CanRbySwapNow(CardId.ElvenotesBlue, true);
                bool yellowSwap = CanRbySwapNow(CardId.ElvenotesYellow, true);
                if (wantChicken)
                {
                    foreach (ClientCard card in cards)
                    {
                        if (card != null && card.IsCode(CardId.JailChicken) && !ranked.Contains(card))
                            ranked.Add(card);
                    }
                }
                // 4-body Wind line stored the missing color; pull it even if ③ has no target yet.
                if (white10EnemyWindMissingDeckId != 0)
                {
                    foreach (ClientCard card in cards)
                    {
                        if (card != null && card.IsCode(white10EnemyWindMissingDeckId) && !ranked.Contains(card))
                            ranked.Add(card);
                    }
                }
                List<int> rbyOrder = new List<int> { CardId.ElvenotesRed, CardId.ElvenotesBlue, CardId.ElvenotesYellow };
                foreach (int id in rbyOrder)
                {
                    if (!CanRbySwapNow(id, true)) continue;
                    foreach (ClientCard card in cards)
                    {
                        if (card != null && card.IsCode(id) && !ranked.Contains(card))
                            ranked.Add(card);
                    }
                }
                // After White 10 ②, unused ③ RBY still sit as disruption even with no current target.
                if (w10Used)
                {
                    foreach (int id in rbyOrder)
                    {
                        if (id == CardId.ElvenotesRed && activatedCardIdList.Contains(CardId.ElvenotesRed + 3))
                            continue;
                        if (id == CardId.ElvenotesBlue && activatedCardIdList.Contains(CardId.ElvenotesBlue + 3))
                            continue;
                        if (id == CardId.ElvenotesYellow && activatedCardIdList.Contains(CardId.ElvenotesYellow + 3))
                            continue;
                        if (HasFaceupRbyReadyToSwap(id)) continue;
                        foreach (ClientCard card in cards)
                        {
                            if (card != null && card.IsCode(id) && !ranked.Contains(card))
                                ranked.Add(card);
                        }
                    }
                }
                // Fallback: highest ATK among remaining Elvenotes (exclude Wind copy).
                List<ClientCard> byAtk = cards.Where(c => c != null && c.HasSetcode(SetcodeElvenotes)
                    && !c.IsCode(CardId.ElvenotesWind) && !ranked.Contains(c))
                    .OrderByDescending(c => c.Attack).ToList();
                ranked.AddRange(byAtk);
                foreach (ClientCard card in cards)
                {
                    if (card != null && !ranked.Contains(card))
                        ranked.Add(card);
                }
                if (ranked.Count > 0)
                    ElfnoteLog("Wind SS enemyTurn first=" + CardStr(ranked[0])
                        + " wantChicken=" + wantChicken
                        + " w10Avail=" + w10Avail
                        + " w10Used=" + w10Used
                        + " missingDeck=" + white10EnemyWindMissingDeckId
                        + " redSwap=" + redSwap
                        + " blueSwap=" + blueSwap
                        + " yellowSwap=" + yellowSwap);
                return Util.CheckSelectCount(ranked, cards, min, max);
            }

            bool hasTuner = Bot.GetMonsters().Any(c => c != null && c.IsFaceup() && c.IsTuner());
            bool blue2Used = activatedCardIdList.Contains(CardId.ElvenotesBlue);
            bool blueOnFieldFor2 = !blue2Used
                && Bot.GetMonsters().Any(c => c != null && c.IsCode(CardId.ElvenotesBlue) && c.IsFaceup());
            bool red2Unused = CanElvenotesRedSearch();
            bool needYellowForRed = !Bot.HasInSpellZone(CardId.RedField)
                && (Bot.HasInDeck(CardId.RedField) || Bot.HasInHand(CardId.RedField))
                && !Bot.HasInHand(CardId.ElvenotesYellow) && !Bot.HasInMonstersZone(CardId.ElvenotesYellow);
            List<int> order;
            // After Crystal Wing, Red opening step 6: Wind ② SS Yellow to place Red Field, then Red Field
            // revives GY chicken for White 10. Do not grab a second deck chicken just because
            // there is no tuner — GY chicken is the tuner.
            if (IsFuwalosOnlyOpeningCompromise() && !hasTuner)
                order = new List<int> { CardId.JailChicken, CardId.ElvenotesYellow, CardId.ElvenotesBlue, CardId.ElvenotesRed };
            else if (needYellowForRed && HasEightSynchroOnField() && Bot.HasInGraveyard(CardId.JailChicken))
                order = new List<int> { CardId.ElvenotesYellow, CardId.JailChicken, CardId.ElvenotesBlue, CardId.ElvenotesRed };
            else if (!hasTuner)
                order = new List<int> { CardId.JailChicken, CardId.ElvenotesYellow, CardId.ElvenotesBlue, CardId.ElvenotesRed };
            else if (needYellowForRed)
                order = new List<int> { CardId.ElvenotesYellow, CardId.ElvenotesBlue, CardId.JailChicken, CardId.ElvenotesRed };
            else if (red2Unused)
                order = new List<int> { CardId.ElvenotesRed, CardId.JailChicken, CardId.ElvenotesBlue, CardId.ElvenotesYellow };
            else if (!blue2Used && !blueOnFieldFor2)
                order = new List<int> { CardId.ElvenotesBlue, CardId.JailChicken, CardId.ElvenotesYellow, CardId.ElvenotesRed };
            else
                order = new List<int> { CardId.JailChicken, CardId.ElvenotesYellow, CardId.ElvenotesBlue, CardId.ElvenotesRed };
            ElfnoteLog("Wind SS hasTuner=" + hasTuner + " blue2Used=" + blue2Used
                + " blueOnField=" + blueOnFieldFor2 + " red2Unused=" + red2Unused + " needYellow=" + needYellowForRed);
            return PreferCards(cards, order, min, max);
        }

        /// <summary>
        /// Field synchro materials excluding the White 10 that is leaving.
        /// Crystal Wing, Formula, Baronne, and Librarian are end-board bodies, not leftover materials.
        /// </summary>
        public void CountWhite10FollowupSynchroBodies(out int tuners, out int nonTuners)
        {
            tuners = 0;
            nonTuners = 0;
            foreach (ClientCard card in Bot.GetMonsters())
            {
                if (card == null || !card.IsFaceup()) continue;
                if (card.IsCode(CardId.White10, CardId.CrystalWing, CardId.FormulaAthleteLightning,
                    CardId.BaronneDeFleur, CardId.SuperLibrarian)) continue;
                if (card.Level <= 0) continue;
                if (card.HasType(CardType.Xyz | CardType.Link)) continue;
                if (card.IsTuner()) tuners++;
                else nonTuners++;
            }
        }

        public bool IsWhite10HandSixStar(ClientCard card)
        {
            return card != null && card.Location == CardLocation.Hand
                && card.IsCode(CardId.ElvenotesRed, CardId.ElvenotesBlue, CardId.ElvenotesYellow);
        }

        public int CountHandCode(int cardId)
        {
            int count = 0;
            foreach (ClientCard card in Bot.Hand)
            {
                if (card != null && card.IsCode(cardId))
                    count++;
            }
            return count;
        }

        /// <summary>
        /// Hand 6-star for White 10: unused ② first, then a duplicate copy in hand.
        /// </summary>
        public ClientCard PickWhite10HandSixStar(IList<ClientCard> cards)
        {
            foreach (ClientCard card in cards)
            {
                if (IsWhite10HandSixStar(card) && White10Body2StillAvailable(card.Id))
                    return card;
            }
            foreach (ClientCard card in cards)
            {
                if (IsWhite10HandSixStar(card) && CountHandCode(card.Id) >= 2)
                    return card;
            }
            foreach (ClientCard card in cards)
            {
                if (IsWhite10HandSixStar(card))
                    return card;
            }
            return null;
        }

        public void AddWhite10HandSixStars(List<ClientCard> ranked, IList<ClientCard> cards)
        {
            foreach (ClientCard card in cards)
            {
                if (IsWhite10HandSixStar(card) && White10Body2StillAvailable(card.Id) && !ranked.Contains(card))
                    ranked.Add(card);
            }
            foreach (ClientCard card in cards)
            {
                if (IsWhite10HandSixStar(card) && CountHandCode(card.Id) >= 2 && !ranked.Contains(card))
                    ranked.Add(card);
            }
            foreach (ClientCard card in cards)
            {
                if (IsWhite10HandSixStar(card) && !ranked.Contains(card))
                    ranked.Add(card);
            }
        }

        public bool White10Body2StillAvailable(int cardId)
        {
            if (cardId == CardId.ElvenotesRed) return CanElvenotesRedSearch();
            if (cardId == CardId.ElvenotesYellow) return !activatedCardIdList.Contains(CardId.ElvenotesYellow);
            if (cardId == CardId.ElvenotesBlue) return !activatedCardIdList.Contains(CardId.ElvenotesBlue);
            return false;
        }

        public ClientCard PickWhite10DeckSixStar(IList<ClientCard> cards)
        {
            List<int> bodyOrder = new List<int> { CardId.ElvenotesRed, CardId.ElvenotesYellow, CardId.ElvenotesBlue };
            foreach (int id in bodyOrder)
            {
                if (Bot.HasInHand(id)) continue;
                if (!White10Body2StillAvailable(id)) continue;
                foreach (ClientCard card in cards)
                {
                    if (card != null && card.IsCode(id) && card.Location == CardLocation.Deck)
                        return card;
                }
            }
            return null;
        }

        /// <summary>
        /// Unused Wind ②: White 10's deck slot should SS Wind into z2 so ② can call unused Red.
        /// </summary>
        public bool CanWhite10DeckSsWind(IList<ClientCard> cards)
        {
            if (activatedCardIdList.Contains(CardId.ElvenotesWind + 1))
                return false;
            ClientCard center = GetCenterMonster();
            if (center != null && center.IsCode(CardId.ElvenotesWind) && center.Sequence == 2)
                return false;
            foreach (ClientCard card in cards)
            {
                if (card != null && card.IsCode(CardId.ElvenotesWind) && card.Location == CardLocation.Deck)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Deck slot: if leftover field has more non-tuners than tuners, SS chicken to feed later synchro
        /// (our turn needs 2+ chickens in deck, or 1 deck + 1 hand; opponent turn needs 1).
        /// GY chicken is always taken first on the default path, so count it as the leftover tuner.
        /// Hand + deck chicken is treated as 2 deck chickens; the caller SS the hand copy and a deck 6-star.
        /// Else unused Wind ②, else unused ② Red/Yellow/Blue that is not in hand.
        /// </summary>
        public bool ShouldWhite10DeckSsChicken(IList<ClientCard> cards)
        {
            int tuners;
            int nonTuners;
            CountWhite10FollowupSynchroBodies(out tuners, out nonTuners);
            bool gyChickenCounted = white10PickedGyChickenThisChain;
            if (!gyChickenCounted)
            {
                foreach (ClientCard card in cards)
                {
                    if (card != null && card.IsCode(CardId.JailChicken) && card.Location == CardLocation.Grave)
                    {
                        gyChickenCounted = true;
                        break;
                    }
                }
            }
            if (gyChickenCounted)
                tuners++;
            if (nonTuners <= tuners) return false;
            int deckChicken = 0;
            bool hasHandChicken = false;
            foreach (ClientCard card in cards)
            {
                if (card == null) continue;
                if (card.IsCode(CardId.JailChicken) && card.Location == CardLocation.Deck)
                    deckChicken++;
                if (card.IsCode(CardId.JailChicken) && card.Location == CardLocation.Hand)
                    hasHandChicken = true;
            }
            if (deckChicken <= 0)
                deckChicken = Bot.GetCardCountInDeck(CardId.JailChicken);
            if (Duel.Player == 0) return deckChicken >= 2 || (hasHandChicken && deckChicken >= 1);
            return deckChicken >= 1;
        }

        /// <summary>
        /// White 10 SS: at most 1 from each of GY / hand / deck (script gcheck).
        /// Opponent turn: 4-body White7/Wind lines requiring Red+Blue+Yellow, else fill 3 with RBY, chicken last.
        /// Own turn: Only 2 free main zones, GY White 7 is legal, and 2+ chickens off the field:
        /// SS GY White 7 + one chicken (hand first, else deck while leaving 1 in deck) for 8-synchro.
        /// Our turn, 2+ deck chickens (or 1 deck + 1 hand chicken), leftover non-tuners &lt;= tuners
        /// (Crystal Wing / Formula / Baronne / Librarian ignored), and GY White 7 is legal:
        /// if hand has chicken, SS GY White 7 + hand chicken + deck 6-star; else GY White 7 + deck chicken + hand 6-star.
        /// Otherwise GY chicken + hand 6-star (or hand chicken when keeping 1 in deck), then fill the deck slot
        /// (unused Wind ② first, else unused ② Red/Yellow/Blue, else chicken).
        /// SelectUnselect asks one card at a time; do not confirm (min=0) while a useful deck card remains.
        /// </summary>
        public IList<ClientCard> SelectWhite10SpSummon(IList<ClientCard> cards, int min, int max)
        {
            if (Duel.Player == 1)
                return SelectWhite10SpSummonEnemyTurn(cards, min, max);

            if (white10MissingPlaceLineThisChain)
            {
                List<ClientCard> forced = new List<ClientCard>();
                ClientCard gyChicken = null;
                foreach (ClientCard card in cards)
                {
                    if (card != null && card.IsCode(CardId.JailChicken) && card.Location == CardLocation.Grave)
                    {
                        gyChicken = card;
                        break;
                    }
                }
                int deckId = OpeningYellowPlaceMissing() ? CardId.ElvenotesYellow : CardId.ElvenotesBlue;
                ClientCard deckColor = null;
                foreach (ClientCard card in cards)
                {
                    if (card != null && card.IsCode(deckId) && card.Location == CardLocation.Deck)
                    {
                        deckColor = card;
                        break;
                    }
                }
                if (gyChicken == null || deckColor == null)
                {
                    ElfnoteLog("White10 missing-place fallback gyChicken=" + (gyChicken != null)
                        + " deckId=" + deckId + " deckColor=" + (deckColor != null));
                }
                else
                {
                    forced.Add(gyChicken);
                    forced.Add(deckColor);
                    ClientCard handSix = PickWhite10HandSixStar(cards);
                    if (handSix != null) forced.Add(handSix);
                    ElfnoteLog("White10 missing-place SS " + CardStr(gyChicken) + " + " + CardStr(deckColor)
                        + (handSix != null ? " + " + CardStr(handSix) : ""));
                    return FinishWhite10OwnSelect(forced, cards, min, max);
                }
            }

            List<ClientCard> ranked = new List<ClientCard>();
            int tuners;
            int nonTuners;
            CountWhite10FollowupSynchroBodies(out tuners, out nonTuners);
            int deckChicken = 0;
            bool hasGyWhite7 = false;
            bool hasHandElvenotes = false;
            bool hasDeckChickenCard = false;
            bool hasHandChicken = false;
            foreach (ClientCard card in cards)
            {
                if (card == null) continue;
                if (card.IsCode(CardId.JailChicken) && card.Location == CardLocation.Deck)
                {
                    deckChicken++;
                    hasDeckChickenCard = true;
                }
                if (card.IsCode(CardId.JailChicken) && card.Location == CardLocation.Hand)
                    hasHandChicken = true;
                if (card.IsCode(CardId.White7) && card.Location == CardLocation.Grave)
                    hasGyWhite7 = true;
                if (IsWhite10HandSixStar(card))
                    hasHandElvenotes = true;
            }
            if (deckChicken <= 0)
                deckChicken = Bot.GetCardCountInDeck(CardId.JailChicken);
            int freeMain = CountFreeMainMonsterZones();
            int chickenOffField = CountChickenInHandDeckGrave();
            bool canEight = HasEightSynchroInExtra();
            bool twoZoneWhite7Chicken = white10TwoZoneWhite7ChickenThisChain
                || (Duel.Player == 0 && freeMain == 2 && hasGyWhite7 && chickenOffField >= 2
                    && (hasHandChicken || hasDeckChickenCard)
                    && canEight
                    && !CheckShouldNoMoreSpSummon(CardLocation.Grave));
            if (twoZoneWhite7Chicken)
            {
                white10TwoZoneWhite7ChickenThisChain = true;
                ElfnoteLog("White10 two-zone White7+chicken freeMain=" + freeMain + " offFieldChicken=" + chickenOffField);
                ClientCard gyWhite7 = null;
                ClientCard handChicken = null;
                ClientCard deckChickenCard = null;
                foreach (ClientCard card in cards)
                {
                    if (card == null) continue;
                    if (gyWhite7 == null && card.IsCode(CardId.White7) && card.Location == CardLocation.Grave)
                        gyWhite7 = card;
                    if (handChicken == null && card.IsCode(CardId.JailChicken) && card.Location == CardLocation.Hand)
                        handChicken = card;
                    if (deckChickenCard == null && card.IsCode(CardId.JailChicken) && card.Location == CardLocation.Deck)
                        deckChickenCard = card;
                }
                if (gyWhite7 != null) ranked.Add(gyWhite7);
                // Prefer a hand chicken so at least 1 chicken stays in deck.
                if (handChicken != null)
                    ranked.Add(handChicken);
                else if (deckChickenCard != null)
                    ranked.Add(deckChickenCard);
                if (ranked.Count > 0)
                    ElfnoteLog("White10 SS first=" + CardStr(ranked[0]) + (ranked.Count > 1 ? " second=" + CardStr(ranked[1]) : ""));
                return FinishWhite10OwnSelect(ranked, cards, min, max);
            }
            bool twoChickensKeepOneInDeck = deckChicken >= 2 || (hasHandChicken && hasDeckChickenCard);
            bool gyWhite7HandChicken = hasHandChicken && hasDeckChickenCard;
            // After Crystal Wing, White 10 ② should pull GY White 7 + a chicken (deck/hand), then 7+1 Omega.
            // GY/hand/deck are 1 each, so GY chicken + GY White 7 is illegal; take White 7 from GY.
            bool gyWhite7AfterEight = Duel.Player == 0 && HasEightSynchroOnField() && hasGyWhite7
                && (hasDeckChickenCard || hasHandChicken)
                && HasEightSynchroInExtra()
                && !CheckShouldNoMoreSpSummon(CardLocation.Grave)
                && !CheckShouldNoMoreSpSummon(CardLocation.Deck);
            bool gyWhite7Line = white10GyWhite7LineThisChain || gyWhite7AfterEight
                || (Duel.Player == 0 && twoChickensKeepOneInDeck && nonTuners <= tuners
                && hasGyWhite7 && (gyWhite7HandChicken || (hasHandElvenotes && hasDeckChickenCard))
                && !CheckShouldNoMoreSpSummon(CardLocation.Deck));
            if (gyWhite7Line)
            {
                white10GyWhite7LineThisChain = true;
                ClientCard gyWhite7 = null;
                ClientCard deckChickenCard = null;
                ClientCard handChicken = null;
                foreach (ClientCard card in cards)
                {
                    if (card == null) continue;
                    if (gyWhite7 == null && card.IsCode(CardId.White7) && card.Location == CardLocation.Grave)
                        gyWhite7 = card;
                    if (deckChickenCard == null && card.IsCode(CardId.JailChicken) && card.Location == CardLocation.Deck)
                        deckChickenCard = card;
                    if (handChicken == null && card.IsCode(CardId.JailChicken) && card.Location == CardLocation.Hand)
                        handChicken = card;
                }
                if (gyWhite7 != null) ranked.Add(gyWhite7);
                if (handChicken != null)
                {
                    ElfnoteLog("White10 GY White7 hand-chicken line tuners=" + tuners + " nonTuners=" + nonTuners);
                    ranked.Add(handChicken);
                    ClientCard deckSixStar = PickWhite10DeckSixStar(cards);
                    if (deckSixStar != null) ranked.Add(deckSixStar);
                }
                else
                {
                    ElfnoteLog("White10 GY White7 line tuners=" + tuners + " nonTuners=" + nonTuners
                        + " afterEight=" + gyWhite7AfterEight);
                    if (deckChickenCard != null) ranked.Add(deckChickenCard);
                    ClientCard handElvenotes = PickWhite10HandSixStar(cards);
                    if (handElvenotes != null) ranked.Add(handElvenotes);
                }
                if (ranked.Count > 0)
                    ElfnoteLog("White10 SS first=" + CardStr(ranked[0]) + (ranked.Count > 1 ? " second=" + CardStr(ranked[1]) : ""));
                return FinishWhite10OwnSelect(ranked, cards, min, max);
            }

            foreach (ClientCard card in cards)
            {
                if (card != null && card.IsCode(CardId.JailChicken) && card.Location == CardLocation.Grave && !ranked.Contains(card))
                {
                    ranked.Add(card);
                    white10PickedGyChickenThisChain = true;
                }
            }
            bool wantFollowupChicken = ShouldWhite10DeckSsChicken(cards);
            bool takeHandChickenKeepDeck = hasHandChicken && hasDeckChickenCard && wantFollowupChicken;
            if (takeHandChickenKeepDeck)
            {
                foreach (ClientCard card in cards)
                {
                    if (card != null && card.IsCode(CardId.JailChicken) && card.Location == CardLocation.Hand && !ranked.Contains(card))
                        ranked.Add(card);
                }
            }
            else
                AddWhite10HandSixStars(ranked, cards);
            if (!CheckShouldNoMoreSpSummon(CardLocation.Deck))
            {
                bool wantDeckChicken = wantFollowupChicken && !takeHandChickenKeepDeck;
                bool wantDeckWind = !wantDeckChicken && CanWhite10DeckSsWind(cards);
                ElfnoteLog("White10 deck slot tuners=" + tuners + " nonTuners=" + nonTuners
                    + " wantChicken=" + wantDeckChicken + " wantWind=" + wantDeckWind
                    + " handChickenKeepDeck=" + takeHandChickenKeepDeck);
                if (wantDeckWind)
                {
                    foreach (ClientCard card in cards)
                    {
                        if (card != null && card.IsCode(CardId.ElvenotesWind) && card.Location == CardLocation.Deck && !ranked.Contains(card))
                            ranked.Add(card);
                    }
                }
                if (wantDeckChicken)
                {
                    foreach (ClientCard card in cards)
                    {
                        if (card != null && card.IsCode(CardId.JailChicken) && card.Location == CardLocation.Deck && !ranked.Contains(card))
                            ranked.Add(card);
                    }
                }
                List<int> bodyOrder = new List<int> { CardId.ElvenotesRed, CardId.ElvenotesYellow, CardId.ElvenotesBlue };
                foreach (int id in bodyOrder)
                {
                    if (Bot.HasInHand(id)) continue;
                    if (!White10Body2StillAvailable(id)) continue;
                    foreach (ClientCard card in cards)
                    {
                        if (card != null && card.IsCode(id) && card.Location == CardLocation.Deck && !ranked.Contains(card))
                            ranked.Add(card);
                    }
                }
                if (!wantDeckWind)
                {
                    foreach (ClientCard card in cards)
                    {
                        if (card != null && card.IsCode(CardId.ElvenotesWind) && card.Location == CardLocation.Deck && !ranked.Contains(card))
                            ranked.Add(card);
                    }
                }
            }
            foreach (ClientCard card in cards)
            {
                if (card != null && card.IsCode(CardId.White7) && card.Location == CardLocation.Grave && !ranked.Contains(card))
                    ranked.Add(card);
            }
            // GY 6-star is a non-tuner. Skip when field leftover + planned hand/deck (1 each) already
            // balance tuners (e.g. hand Blue + deck Wind with Chicken+White7 on field).
            int plannedTuners = tuners;
            int plannedNonTuners = nonTuners;
            bool countedHand = false;
            bool countedDeck = false;
            bool countedGy = false;
            foreach (ClientCard card in ranked)
            {
                if (card == null) continue;
                bool isTunerMat = card.IsCode(CardId.JailChicken);
                bool isNonTunerMat = card.IsCode(CardId.White7) || IsElvenotesSixStar(card);
                if (!isTunerMat && !isNonTunerMat) continue;
                if (card.Location == CardLocation.Hand)
                {
                    if (countedHand) continue;
                    countedHand = true;
                }
                else if (card.Location == CardLocation.Deck)
                {
                    if (countedDeck) continue;
                    countedDeck = true;
                }
                else if (card.Location == CardLocation.Grave)
                {
                    if (countedGy) continue;
                    countedGy = true;
                }
                else continue;
                if (isTunerMat) plannedTuners++;
                else plannedNonTuners++;
            }
            bool skipGySixStarFlood = plannedNonTuners >= plannedTuners;
            if (!skipGySixStarFlood)
            {
                foreach (ClientCard card in cards)
                {
                    if (card != null && IsElvenotesSixStar(card) && card.Location == CardLocation.Grave && !ranked.Contains(card))
                        ranked.Add(card);
                }
            }
            else
                ElfnoteLog("White10 skip GY 6-star flood plannedT=" + plannedTuners + " plannedNT=" + plannedNonTuners);
            foreach (ClientCard card in cards)
            {
                if (card != null && card.IsCode(CardId.JailChicken) && card.Location == CardLocation.Hand && !ranked.Contains(card))
                    ranked.Add(card);
            }
            if (ranked.Count > 0)
                ElfnoteLog("White10 SS first=" + CardStr(ranked[0]) + (ranked.Count > 1 ? " second=" + CardStr(ranked[1]) : ""));
            return FinishWhite10OwnSelect(ranked, cards, min, max);
        }

        public IList<ClientCard> FinishWhite10OwnSelect(List<ClientCard> ranked, IList<ClientCard> cards, int min, int max)
        {
            IList<ClientCard> picked;
            if (ranked.Count == 0 && min == 0)
                picked = new List<ClientCard>();
            else
                picked = Util.CheckSelectCount(ranked, cards, min, max);
            int count = picked != null ? picked.Count : 0;
            List<string> parts = new List<string>();
            if (picked != null)
            {
                foreach (ClientCard card in picked)
                {
                    if (card == null) continue;
                    parts.Add(CardStr(card));
                }
            }
            ElfnoteLog("White10 SS result min=" + min + " max=" + max + " count=" + count
                + (parts.Count > 0 ? " cards=" + string.Join(",", parts.ToArray()) : ""));
            return picked;
        }

        public ClientCard FindWhite10Candidate(IList<ClientCard> cards, int cardId, CardLocation loc)
        {
            if (white10EnemyPickedCodesThisChain.Contains(cardId))
                return null;
            foreach (ClientCard card in cards)
            {
                if (card != null && card.IsCode(cardId) && card.Location == loc)
                    return card;
            }
            return null;
        }

        public bool White10EnemySkipCode(List<ClientCard> picked, int cardId)
        {
            if (white10EnemyPickedCodesThisChain.Contains(cardId)) return true;
            if (picked != null && picked.Any(p => p != null && p.IsCode(cardId))) return true;
            if (Bot.GetMonsters().Any(c => c != null && c.IsFaceup() && c.IsCode(cardId))) return true;
            return false;
        }

        public IList<ClientCard> FinishWhite10EnemySelect(List<ClientCard> ranked, IList<ClientCard> cards, int min, int max)
        {
            if (ranked.Count == 0 && min == 0)
                return new List<ClientCard>();
            IList<ClientCard> result = Util.CheckSelectCount(ranked, cards, min, max);
            if (result != null)
            {
                foreach (ClientCard card in result)
                {
                    if (card != null && !white10EnemyPickedCodesThisChain.Contains(card.Id))
                        white10EnemyPickedCodesThisChain.Add(card.Id);
                }
            }
            return result;
        }

        /// <summary>
        /// Opponent-turn White 10 ②: who sits in z2. Unused Wind ② needs center.
        /// Else chicken, so Red/Blue/Yellow can ③ from a side main zone (the z2 body cannot ③).
        /// Else Yellow-Red-Blue.
        /// </summary>
        public int White10EnemyTurnCenterCardId()
        {
            bool wind2 = !activatedCardIdList.Contains(CardId.ElvenotesWind + 1);
            if (wind2 && white10EnemyPickedCodesThisChain.Contains(CardId.ElvenotesWind))
                return CardId.ElvenotesWind;
            if (white10EnemyPickedCodesThisChain.Contains(CardId.JailChicken))
                return CardId.JailChicken;
            if (white10EnemyPickedCodesThisChain.Contains(CardId.ElvenotesYellow))
                return CardId.ElvenotesYellow;
            if (white10EnemyPickedCodesThisChain.Contains(CardId.ElvenotesRed))
                return CardId.ElvenotesRed;
            if (white10EnemyPickedCodesThisChain.Contains(CardId.ElvenotesBlue))
                return CardId.ElvenotesBlue;
            if (white10EnemyPickedCodesThisChain.Contains(CardId.ElvenotesWind))
                return CardId.ElvenotesWind;
            return 0;
        }

        public bool White10EnemyTurnWantsThisCardInCenter(int cardId)
        {
            int want = White10EnemyTurnCenterCardId();
            if (want != 0)
                return cardId == want;
            if (cardId == CardId.ElvenotesWind && !activatedCardIdList.Contains(CardId.ElvenotesWind + 1))
                return true;
            if (cardId == CardId.JailChicken)
                return true;
            if (cardId == CardId.ElvenotesYellow || cardId == CardId.ElvenotesRed || cardId == CardId.ElvenotesBlue)
                return true;
            return false;
        }

        /// <summary>
        /// Opponent-turn White 10 dismantle picks: prefer a 4-body three-color line (White 7 / Wind),
        /// otherwise fill 3 with Red-Blue-Yellow, chicken as fallback.
        /// </summary>
        public IList<ClientCard> SelectWhite10SpSummonEnemyTurn(IList<ClientCard> cards, int min, int max)
        {
            // Script: GY / hand / deck at most 1 each. SelectUnselect must not pick a 4th body.
            if (min == 0 && white10EnemyPickedCodesThisChain.Count >= 3)
                return new List<ClientCard>();

            List<ClientCard> ranked = new List<ClientCard>();
            // White 10 returning to Extra frees one zone
            int freeAfter = CountFreeMainMonsterZones() + 1;
            if (Bot.MonsterZone[2] != null && Bot.MonsterZone[2].IsCode(CardId.White10))
            {
                // White10 itself is in main zone; already counted in GetMonstersInMainZone
            }

            ClientCard gyWhite7 = FindWhite10Candidate(cards, CardId.White7, CardLocation.Grave);
            ClientCard windGy = FindWhite10Candidate(cards, CardId.ElvenotesWind, CardLocation.Grave);
            ClientCard windDeck = FindWhite10Candidate(cards, CardId.ElvenotesWind, CardLocation.Deck);
            ClientCard windHand = FindWhite10Candidate(cards, CardId.ElvenotesWind, CardLocation.Hand);
            ClientCard windPick = windGy ?? windDeck ?? windHand;
            bool alreadyWind = white10EnemyPickedCodesThisChain.Contains(CardId.ElvenotesWind);

            List<int> rbyIds = new List<int> { CardId.ElvenotesRed, CardId.ElvenotesBlue, CardId.ElvenotesYellow };

            // Line 1: GY White7 + hand 1 + deck 1, White7 ② pulls the missing color. Need 4 zones.
            if (freeAfter >= 4 && gyWhite7 != null && !activatedCardIdList.Contains(CardId.White7)
                && !CheckShouldNoMoreSpSummon(CardLocation.Grave))
            {
                ClientCard handPick = null;
                ClientCard deckPick = null;
                List<int> have = new List<int>();
                // Prefer RBY that pass swap (batch fills center)
                foreach (int id in rbyIds)
                {
                    if (handPick == null)
                    {
                        ClientCard h = FindWhite10Candidate(cards, id, CardLocation.Hand);
                        if (h != null) { handPick = h; have.Add(id); }
                    }
                }
                foreach (int id in rbyIds)
                {
                    if (have.Contains(id)) continue;
                    if (deckPick == null)
                    {
                        ClientCard d = FindWhite10Candidate(cards, id, CardLocation.Deck);
                        if (d != null) { deckPick = d; have.Add(id); }
                    }
                }
                // Missing color must be in GY for White7 ② (not the ones already picked)
                bool thirdInGy = false;
                foreach (int id in rbyIds)
                {
                    if (have.Contains(id)) continue;
                    if (Bot.HasInGraveyard(id))
                    {
                        thirdInGy = true;
                        have.Add(id);
                        break;
                    }
                }
                bool threeColors = thirdInGy && have.Contains(CardId.ElvenotesRed) && have.Contains(CardId.ElvenotesBlue)
                    && have.Contains(CardId.ElvenotesYellow);
                if (threeColors && handPick != null && deckPick != null)
                {
                    // Placement order: Yellow first among RBY, then White7
                    ClientCard yellow = null;
                    if (handPick.IsCode(CardId.ElvenotesYellow)) yellow = handPick;
                    else if (deckPick.IsCode(CardId.ElvenotesYellow)) yellow = deckPick;
                    if (yellow != null) ranked.Add(yellow);
                    ranked.Add(gyWhite7);
                    if (!ranked.Contains(handPick)) ranked.Add(handPick);
                    if (!ranked.Contains(deckPick)) ranked.Add(deckPick);
                    ElfnoteLog("White10 enemy 4-body White7 line");
                    return FinishWhite10EnemySelect(ranked, cards, min, max);
                }
            }

            // Line 2: one Wind (GY-deck-hand) + GY 1 + hand 1, Wind ② pulls missing from deck.
            // SelectUnselect: after the Wind slot is taken, do not pick a second Wind from hand/deck.
            if (freeAfter >= 4 && (windPick != null || alreadyWind) && !activatedCardIdList.Contains(CardId.ElvenotesWind + 1)
                && !CheckShouldNoMoreSpSummon(CardLocation.Deck))
            {
                ClientCard gyPick = null;
                ClientCard handPick = null;
                List<int> have = new List<int>();
                foreach (int id in rbyIds)
                {
                    if (gyPick == null)
                    {
                        ClientCard g = FindWhite10Candidate(cards, id, CardLocation.Grave);
                        if (g != null) { gyPick = g; have.Add(id); }
                    }
                }
                foreach (int id in rbyIds)
                {
                    if (have.Contains(id)) continue;
                    if (handPick == null)
                    {
                        ClientCard h = FindWhite10Candidate(cards, id, CardLocation.Hand);
                        if (h != null) { handPick = h; have.Add(id); }
                    }
                }
                bool thirdInDeck = false;
                int missingId = 0;
                foreach (int id in rbyIds)
                {
                    if (have.Contains(id)) continue;
                    if (Bot.HasInDeck(id))
                    {
                        thirdInDeck = true;
                        missingId = id;
                        have.Add(id);
                        break;
                    }
                }
                bool threeColors = have.Contains(CardId.ElvenotesRed) && have.Contains(CardId.ElvenotesBlue)
                    && have.Contains(CardId.ElvenotesYellow);
                // Wind must be able to enter center (White10 leaves z2 empty)
                if (threeColors && gyPick != null && handPick != null && thirdInDeck && missingId != 0)
                {
                    if (windPick != null)
                        ranked.Add(windPick);
                    if (!ranked.Contains(gyPick)) ranked.Add(gyPick);
                    if (!ranked.Contains(handPick)) ranked.Add(handPick);
                    white10EnemyWindMissingDeckId = missingId;
                    ElfnoteLog("White10 enemy 4-body Wind line missingDeck=" + missingId
                        + (alreadyWind ? " windAlreadyPicked" : ""));
                    return FinishWhite10EnemySelect(ranked, cards, min, max);
                }
            }

            // Fallback: fill up to 3 unique Red-Blue-Yellow (pass swap first), then ATK.
            // Same name only once (do not SS hand Blue + GY Blue). Chicken/Wind pad if colors < 3.
            List<ClientCard> picked = new List<ClientCard>();
            bool usedGy = false, usedHand = false, usedDeck = false;
            foreach (int id in rbyIds)
            {
                if (picked.Count >= 3) break;
                if (White10EnemySkipCode(picked, id)) continue;
                if (!CanRbySwapNow(id, true)) continue;
                ClientCard pick = null;
                if (!usedGy) pick = FindWhite10Candidate(cards, id, CardLocation.Grave);
                if (pick != null) { picked.Add(pick); usedGy = true; continue; }
                if (!usedHand) pick = FindWhite10Candidate(cards, id, CardLocation.Hand);
                if (pick != null) { picked.Add(pick); usedHand = true; continue; }
                if (!usedDeck) pick = FindWhite10Candidate(cards, id, CardLocation.Deck);
                if (pick != null) { picked.Add(pick); usedDeck = true; continue; }
            }
            if (picked.Count < 3)
            {
                List<ClientCard> rbyAll = cards.Where(c => c != null
                    && (c.IsCode(CardId.ElvenotesRed) || c.IsCode(CardId.ElvenotesBlue) || c.IsCode(CardId.ElvenotesYellow))
                    && !picked.Contains(c)).OrderByDescending(c => c.Attack).ToList();
                foreach (ClientCard card in rbyAll)
                {
                    if (picked.Count >= 3) break;
                    if (White10EnemySkipCode(picked, card.Id)) continue;
                    if (card.Location == CardLocation.Grave && usedGy) continue;
                    if (card.Location == CardLocation.Hand && usedHand) continue;
                    if (card.Location == CardLocation.Deck && usedDeck) continue;
                    picked.Add(card);
                    if (card.Location == CardLocation.Grave) usedGy = true;
                    else if (card.Location == CardLocation.Hand) usedHand = true;
                    else if (card.Location == CardLocation.Deck) usedDeck = true;
                }
            }
            if (picked.Count < 3)
            {
                ClientCard chicken = null;
                if (!White10EnemySkipCode(picked, CardId.JailChicken))
                {
                    if (!usedGy) chicken = FindWhite10Candidate(cards, CardId.JailChicken, CardLocation.Grave);
                    if (chicken == null && !usedHand) chicken = FindWhite10Candidate(cards, CardId.JailChicken, CardLocation.Hand);
                    if (chicken == null && !usedDeck) chicken = FindWhite10Candidate(cards, CardId.JailChicken, CardLocation.Deck);
                }
                if (chicken != null)
                {
                    picked.Add(chicken);
                    if (chicken.Location == CardLocation.Grave) usedGy = true;
                    else if (chicken.Location == CardLocation.Hand) usedHand = true;
                    else if (chicken.Location == CardLocation.Deck) usedDeck = true;
                }
            }
            if (picked.Count < 3)
            {
                List<ClientCard> others = cards.Where(c => c != null && c.HasSetcode(SetcodeElvenotes)
                    && !picked.Contains(c)).OrderByDescending(c => c.Attack).ToList();
                foreach (ClientCard card in others)
                {
                    if (picked.Count >= 3) break;
                    if (White10EnemySkipCode(picked, card.Id)) continue;
                    if (card.Location == CardLocation.Grave && usedGy) continue;
                    if (card.Location == CardLocation.Hand && usedHand) continue;
                    if (card.Location == CardLocation.Deck && usedDeck) continue;
                    picked.Add(card);
                    if (card.Location == CardLocation.Grave) usedGy = true;
                    else if (card.Location == CardLocation.Hand) usedHand = true;
                    else if (card.Location == CardLocation.Deck) usedDeck = true;
                }
            }

            // Rank for SelectUnselect: Yellow first (center), then Red, Blue, rest
            foreach (ClientCard card in picked)
            {
                if (card != null && card.IsCode(CardId.ElvenotesYellow) && !ranked.Contains(card))
                    ranked.Add(card);
            }
            foreach (ClientCard card in picked)
            {
                if (card != null && card.IsCode(CardId.ElvenotesRed) && !ranked.Contains(card))
                    ranked.Add(card);
            }
            foreach (ClientCard card in picked)
            {
                if (card != null && card.IsCode(CardId.ElvenotesBlue) && !ranked.Contains(card))
                    ranked.Add(card);
            }
            foreach (ClientCard card in picked)
            {
                if (card != null && !ranked.Contains(card))
                    ranked.Add(card);
            }
            if (ranked.Count > 0)
            {
                string pickedStr = "";
                foreach (ClientCard card in ranked)
                    pickedStr += " " + CardStr(card);
                ElfnoteLog("White10 enemy fallback min=" + min + " max=" + max + " count=" + ranked.Count + pickedStr);
            }
            return FinishWhite10EnemySelect(ranked, cards, min, max);
        }

        /// <summary>
        /// Jail Gate mill: chicken first; if chicken is already on field/GY, mill extra White P, not the deck copy.
        /// </summary>
        public IList<ClientCard> SelectJailGateMill(IList<ClientCard> cards, int min, int max)
        {
            bool chickenAlready = Bot.HasInMonstersZone(CardId.JailChicken) || Bot.HasInGraveyard(CardId.JailChicken);
            List<ClientCard> gyOk = new List<ClientCard>();
            foreach (ClientCard card in cards)
            {
                if (card != null && !CheckWhetherBotWillBeBanished(card) && !gyOk.Contains(card))
                    gyOk.Add(card);
            }
            IList<ClientCard> millPool = gyOk.Count > 0 ? gyOk : cards;
            ElfnoteLog("Jail Gate mill, chicken already=" + chickenAlready + " gyOk=" + gyOk.Count);
            if (!chickenAlready && millPool.Any(c => c != null && c.IsCode(CardId.JailChicken)
                && !CheckWhetherBotWillBeBanished(c)))
                return PreferCards(millPool, new List<int> { CardId.JailChicken, CardId.MixedHellGod, CardId.WhitePendulum }, min, max);

            List<ClientCard> millRanked = new List<ClientCard>();
            foreach (ClientCard card in millPool)
            {
                if (card != null && card.IsCode(CardId.WhitePendulum) && card.Location == CardLocation.Extra && !millRanked.Contains(card))
                    millRanked.Add(card);
            }
            foreach (ClientCard card in millPool)
            {
                if (card != null && card.IsCode(CardId.MixedHellGod) && card.Location == CardLocation.Extra && !millRanked.Contains(card))
                    millRanked.Add(card);
            }
            foreach (ClientCard card in millPool)
            {
                if (card != null && card.IsCode(CardId.WhitePendulum) && !millRanked.Contains(card))
                    millRanked.Add(card);
            }
            foreach (ClientCard card in millPool)
            {
                if (card != null && card.IsCode(CardId.MixedHellGod) && !millRanked.Contains(card))
                    millRanked.Add(card);
            }
            foreach (ClientCard card in millPool)
            {
                if (card != null && card.IsCode(CardId.JailChicken) && !millRanked.Contains(card))
                    millRanked.Add(card);
            }
            if (millRanked.Count > 0)
                ElfnoteLog("Jail Gate mill pick " + CardStr(millRanked[0]));
            return Util.CheckSelectCount(millRanked, millPool, min, max);
        }

        public IList<ClientCard> SelectMixedHellGodSearch(IList<ClientCard> cards, int min, int max)
        {
            // SelectUnselect finish step: min=0 means "OK / no more cards", not a second search.
            if (min == 0)
                return new List<ClientCard>();

            bool needChicken = !HasUsableOneStar();
            bool wantWhiteP = cards.Any(c => c != null && c.IsCode(CardId.WhitePendulum)) && CountOwnedWhitePendulum() < 2;
            bool canTrigger = CanTriggerWhitePendulumThisTurn();
            // Gate is for NS Medius. Skip if Medius is already in hand/field or ① already used.
            bool wantGate = cards.Any(c => c != null && c.IsCode(CardId.JailGodGate))
                && !Bot.HasInHand(CardId.JailGodGate) && !Bot.HasInSpellZone(CardId.JailGodGate)
                && !activatedCardIdList.Contains(CardId.JailGodGate)
                && Duel.Player == 0
                && DefaultCheckWhetherBotCanSearch()
                && CheckWhetherCanSummon()
                && !Bot.HasInHand(CardId.MediusTheInnocent)
                && !Bot.HasInMonstersZone(CardId.MediusTheInnocent)
                && !activatedCardIdList.Contains(CardId.MediusTheInnocent)
                && !Bot.GetMonsters().Any(c => c != null && IsElvenotesSixStar(c) && c.IsFaceup())
                && !HandHasSixStarWaitingToSs();
            List<int> order;
            bool p2Gate = activatedCardIdList.Contains(CardId.MixedHellGod + 1)
                && cards.Any(c => c != null && c.IsCode(CardId.JailGodGate));
            if (p2Gate)
            {
                order = new List<int> { CardId.JailGodGate, CardId.JailChicken, CardId.WhitePendulum, CardId.MixedHellGod };
            }
            else if (needChicken)
                order = new List<int> { CardId.JailChicken, CardId.JailGodGate, CardId.WhitePendulum, CardId.MixedHellGod };
            else if (wantGate)
                order = new List<int> { CardId.JailGodGate, CardId.WhitePendulum, CardId.MixedHellGod, CardId.JailChicken };
            else if (wantWhiteP && canTrigger)
                order = new List<int> { CardId.WhitePendulum, CardId.JailChicken, CardId.MixedHellGod, CardId.JailGodGate };
            else if (wantWhiteP)
                order = new List<int> { CardId.WhitePendulum, CardId.MixedHellGod, CardId.JailChicken, CardId.JailGodGate };
            else
                order = new List<int> { CardId.MixedHellGod, CardId.WhitePendulum, CardId.JailChicken, CardId.JailGodGate };
            ElfnoteLog("Mixed extra search needChicken=" + needChicken + " canTriggerWhiteP=" + canTrigger
                + " wantWhiteP=" + wantWhiteP + " wantGate=" + wantGate + " p2Gate=" + p2Gate);
            return PreferCards(cards, order, min, max);
        }

        public int PickMixedHellGodPendulumOption(IList<int> options)
        {
            int ssIdx = options.IndexOf(Util.GetStringId(CardId.MixedHellGod, 1));
            int atkIdx = options.IndexOf(Util.GetStringId(CardId.MixedHellGod, 2));
            if (ssIdx < 0 && atkIdx < 0)
                return -1;

            bool gyJail = Bot.HasInGraveyard(CardId.JailChicken);
            bool handJail = Bot.HasInHand(CardId.JailChicken);
            bool fieldJail = Bot.HasInMonstersZone(CardId.JailChicken);
            bool hasZone = CountFreeMainMonsterZones() >= 1;
            int pick = atkIdx >= 0 ? atkIdx : ssIdx;
            if (ssIdx >= 0 && hasZone && gyJail && !CheckShouldNoMoreSpSummon(CardLocation.Grave))
                pick = ssIdx;
            else if (ssIdx >= 0 && hasZone && handJail && !fieldJail && !CheckShouldNoMoreSpSummon(CardLocation.Hand)
                && !ShouldNsChickenInsteadOfMixedHandSs())
                pick = ssIdx;
            else if (atkIdx >= 0 && CanMixedHellGodPendulumAtkBuff())
            {
                pick = atkIdx;
                // MixedHellGod + 1: P option 2, Extra ① should search Jail Gate.
                if (!activatedCardIdList.Contains(CardId.MixedHellGod + 1))
                    activatedCardIdList.Add(CardId.MixedHellGod + 1);
            }
            ElfnoteLog("Mixed P option=" + pick + " gyChicken=" + gyJail + " handChicken=" + handJail
                + " fieldChicken=" + fieldJail + " hasZone=" + hasZone
                + " searchGate=" + activatedCardIdList.Contains(CardId.MixedHellGod + 1));
            return pick;
        }

        #endregion

        #region Lifecycle

        /// <summary>
        /// Always go first.
        /// </summary>
        public override bool OnSelectHand()
        {
            return true;
        }

        public override void OnNewTurn()
        {
            summonCount = 1;
            activatedCardIdList.Clear();
            spSummonedCardIdList.Clear();
            enemyPlaceThisTurn.Clear();
            currentNegateCardList.Clear();
            currentDestroyCardList.Clear();
            white10GyWhite7LineThisChain = false;
            white10TwoZoneWhite7ChickenThisChain = false;
            white10PickedGyChickenThisChain = false;
            white10EnemyPickedCodesThisChain.Clear();
            white10EnemyWindMissingDeckId = 0;
            white10MissingPlaceLineThisChain = false;
            redFieldPendingNegateTarget = null;
            preferNegateCard = null;
            nsChickenOffCenterForWhite10 = false;
            chickenDodgeSynchroThisChain = false;
            chickenDodgeSynchroCardId = 0;
            accel2PreferWindChicken = false;
            jailGateResolvedThisTurn = false;
            base.OnNewTurn();
        }

        public override void OnChainSolved(int chainIndex)
        {
            ChainInfo currentChain = Duel.GetCurrentSolvingChainInfo();
            if (currentChain != null && currentChain.ActivatePlayer == 0
                && currentChain.IsActivateCode(CardId.JailGodGate)
                && (Duel.Phase == DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2)
                && !Duel.IsCurrentSolvingChainNegated())
            {
                jailGateResolvedThisTurn = true;
                ElfnoteLog("Jail Gate resolved, non-Jail attack lock");
            }
            base.OnChainSolved(chainIndex);
        }

        public override void OnChainEnd()
        {
            currentNegateCardList.Clear();
            currentDestroyCardList.Clear();
            white10GyWhite7LineThisChain = false;
            white10TwoZoneWhite7ChickenThisChain = false;
            white10PickedGyChickenThisChain = false;
            white10EnemyPickedCodesThisChain.Clear();
            redFieldPendingNegateTarget = null;
            preferNegateCard = null;
            chickenDodgeSynchroThisChain = false;
            chickenDodgeSynchroCardId = 0;
            accel2PreferWindChicken = false;
            for (int idx = enemyPlaceThisTurn.Count - 1; idx >= 0; idx--)
            {
                ClientCard checkTarget = enemyPlaceThisTurn[idx];
                if (checkTarget == null || !checkTarget.IsOnField())
                    enemyPlaceThisTurn.RemoveAt(idx);
            }
            base.OnChainEnd();
        }

        public override void OnMove(ClientCard card, int previousControler, int previousLocation, int currentControler, int currentLocation)
        {
            if (card != null && card.IsCode(CardId.MediusTheInnocent))
            {
                // GY ② is aux.Stringid(id, 1). ① SS from deck is offset 0 and is not this move.
                if (previousControler == 0 && previousLocation == (int)CardLocation.Grave
                    && currentControler == 0 && currentLocation == (int)CardLocation.MonsterZone)
                {
                    ChainInfo solving = Duel.GetCurrentSolvingChainInfo();
                    if (solving != null && solving.ActivatePlayer == 0 && solving.IsActivateCode(CardId.MediusTheInnocent)
                        && solving.ActivateDescription == Util.GetStringId(CardId.MediusTheInnocent, 1))
                    {
                        if (!mediusSsByOwnGyEffect.Contains(card))
                            mediusSsByOwnGyEffect.Add(card);
                        ElfnoteLog("Medius GY ② SS tracked " + CardStr(card));
                    }
                }
                if (mediusSsByOwnGyEffect.Contains(card)
                    && (currentLocation & (int)CardLocation.MonsterZone) == 0)
                {
                    mediusSsByOwnGyEffect.Remove(card);
                    ElfnoteLog("Medius GY ② leave-field untrack " + CardStr(card));
                }
            }
            if (card != null && card.IsCode(CardId.InnocentArt)
                && (previousControler == 0 || currentControler == 0))
            {
                artSearched = false;
                ElfnoteLog("Innocent Art moved, reset artSearched prevLoc=" + previousLocation
                    + " curLoc=" + currentLocation);
            }
            if (card != null && previousControler == 1)
            {
                if (currentControler == 1 && (currentLocation == (int)CardLocation.MonsterZone || currentLocation == (int)CardLocation.SpellZone))
                {
                    if (!enemyPlaceThisTurn.Contains(card))
                        enemyPlaceThisTurn.Add(card);
                }
            }
            base.OnMove(card, previousControler, previousLocation, currentControler, currentLocation);
        }

        public override void OnPosChange(ClientCard card, int previousPosition, int currentPosition)
        {
            if (card != null && mediusSsByOwnGyEffect.Contains(card))
            {
                mediusSsByOwnGyEffect.Remove(card);
                ElfnoteLog("Medius GY ② pos-change untrack " + CardStr(card));
            }
            base.OnPosChange(card, previousPosition, currentPosition);
        }

        public override ClientCard OnSelectAttacker(IList<ClientCard> attackers, IList<ClientCard> defenders)
        {
            if (JailGateGyBanishReady() && PickJailGateBanishTarget(defenders) != null)
            {
                ClientCard jail = attackers.FirstOrDefault(c => c != null && c.HasSetcode(SetcodeJailGod));
                if (jail != null) return jail;
            }
            return base.OnSelectAttacker(attackers, defenders);
        }

        public override BattlePhaseAction OnSelectAttackTarget(ClientCard attacker, IList<ClientCard> defenders)
        {
            if (attacker != null && attacker.HasSetcode(SetcodeJailGod) && JailGateGyBanishReady())
            {
                ClientCard best = PickJailGateBanishTarget(defenders);
                if (best != null)
                {
                    ElfnoteLog("Jail God attack for Gate GY banish " + CardStr(best));
                    return AI.Attack(attacker, best);
                }
            }
            return base.OnSelectAttackTarget(attacker, defenders);
        }

        public override BattlePhaseAction OnBattle(IList<ClientCard> attackers, IList<ClientCard> defenders)
        {
            if (ShouldSkipBattleForBomber() && Duel.BattlePhase.CanMainPhaseTwo)
                return new BattlePhaseAction(BattlePhaseAction.BattleAction.ToMainPhaseTwo);
            if (JailGateGyBanishReady())
            {
                ClientCard jail = attackers.FirstOrDefault(c => c != null && c.HasSetcode(SetcodeJailGod));
                if (jail != null && defenders != null && defenders.Count > 0)
                {
                    BattlePhaseAction hit = OnSelectAttackTarget(jail, defenders);
                    if (hit != null) return hit;
                }
            }
            return base.OnBattle(attackers, defenders);
        }

        #endregion

        #region Selects

        /// <summary>
        /// Our S/T sequence faces the opponent's 4-seq column. A facedown there may be Imperm.
        /// </summary>
        public bool SpellZoneFacesEnemySet(int zoneId)
        {
            if (zoneId < 0 || zoneId > 4) return false;
            ClientCard enemySpell = Enemy.SpellZone[4 - zoneId];
            return enemySpell != null && enemySpell.IsFacedown();
        }

        public string EnemyFacedownSpellSeqStr()
        {
            List<string> seqs = new List<string>();
            for (int i = 0; i < 5; i++)
            {
                ClientCard spell = Enemy.SpellZone[i];
                if (spell != null && spell.IsFacedown())
                    seqs.Add(i.ToString());
            }
            return string.Join(",", seqs.ToArray());
        }

        public override int OnSelectPlace(int cardId, int player, CardLocation location, int available)
        {
            if (player != 0) return base.OnSelectPlace(cardId, player, location, available);

            if (location == CardLocation.SpellZone || location == CardLocation.PendulumZone)
            {
                string enemySet = EnemyFacedownSpellSeqStr();
                if (cardId == CardId.WhitePendulum || cardId == CardId.MixedHellGod)
                {
                    bool z0Avail = (available & Zones.z0) != 0;
                    bool z4Avail = (available & Zones.z4) != 0;
                    bool z0Safe = z0Avail && !SpellZoneFacesEnemySet(0)
                        && !infiniteImpermanenceNegatedColumns.Contains(0);
                    bool z4Safe = z4Avail && !SpellZoneFacesEnemySet(4)
                        && !infiniteImpermanenceNegatedColumns.Contains(4);
                    int picked = 0;
                    int pickedId = -1;
                    if (z0Safe)
                    {
                        picked = Zones.z0;
                        pickedId = 0;
                    }
                    else if (z4Safe)
                    {
                        picked = Zones.z4;
                        pickedId = 4;
                    }
                    else if (z0Avail)
                    {
                        picked = Zones.z0;
                        pickedId = 0;
                    }
                    else if (z4Avail)
                    {
                        picked = Zones.z4;
                        pickedId = 4;
                    }
                    if (picked != 0)
                    {
                        ElfnoteLog("OnSelectPlace S/T " + cardId + " zone=" + pickedId
                            + " available=" + available + " enemySet=" + enemySet);
                        return picked;
                    }
                }
                else
                {
                    // Prefer 1/2/3, then pendulum, skipping resolved Imperm columns and columns
                    // facing an enemy set S/T (Imperm column lock). Still place if none are safe.
                    List<int> spellZones = Util.ShuffleList(new List<int> { 1, 2, 3 });
                    spellZones.AddRange(Util.ShuffleList(new List<int> { 0, 4 }));
                    int fallbackSet = 0;
                    int fallbackSetId = -1;
                    int fallbackImperm = 0;
                    int fallbackImpermId = -1;
                    foreach (int zoneId in spellZones)
                    {
                        int zone = (int)Math.Pow(2, zoneId);
                        if ((available & zone) == 0 || Bot.SpellZone[zoneId] != null)
                            continue;
                        if (infiniteImpermanenceNegatedColumns.Contains(zoneId))
                        {
                            if (fallbackImperm == 0)
                            {
                                fallbackImperm = zone;
                                fallbackImpermId = zoneId;
                            }
                            continue;
                        }
                        if (SpellZoneFacesEnemySet(zoneId))
                        {
                            if (fallbackSet == 0)
                            {
                                fallbackSet = zone;
                                fallbackSetId = zoneId;
                            }
                            continue;
                        }
                        ElfnoteLog("OnSelectPlace S/T " + cardId + " zone=" + zoneId
                            + " available=" + available + " enemySet=" + enemySet);
                        return zone;
                    }
                    if (fallbackSet != 0)
                    {
                        ElfnoteLog("OnSelectPlace S/T " + cardId + " zone=" + fallbackSetId
                            + " available=" + available + " enemySet=" + enemySet
                            + " no safe column, use set column");
                        return fallbackSet;
                    }
                    if (fallbackImperm != 0)
                    {
                        ElfnoteLog("OnSelectPlace S/T " + cardId + " zone=" + fallbackImpermId
                            + " available=" + available + " enemySet=" + enemySet
                            + " no safe column, use Imperm column");
                        return fallbackImperm;
                    }
                }
            }

            if (location == CardLocation.MonsterZone)
            {
                if (cardId == CardId.ElvenotesWind)
                {
                    bool canCenter = (available & Zones.z2) != 0 && Bot.MonsterZone[2] == null;
                    ElfnoteLog("OnSelectPlace Wind canCenter=" + canCenter + " available=" + available);
                }
                // Rule 1 NS chicken: never take z2; Red/Blue/Yellow can only SS into center.
                if (cardId == CardId.JailChicken && nsChickenOffCenterForWhite10)
                {
                    ElfnoteLog("OnSelectPlace Chicken: keep off z2 for White10 dodge NS");
                    int extraDodge = SelectExtraMonsterZone(available);
                    if (extraDodge != 0) return extraDodge;
                    List<int> sideDodge = Util.ShuffleList(new List<int> { 0, 4, 1, 3 });
                    foreach (int zoneId in sideDodge)
                    {
                        int zone = (int)Math.Pow(2, zoneId);
                        if ((available & zone) != 0 && Bot.MonsterZone[zoneId] == null)
                            return zone;
                    }
                }
                // Opening combo: Medius ① / Mixed P SS chicken into z2 so a late Fuwalos
                // still has a +3 target. NS Medius stays off-center. Accel ① chicken
                // stays off z2 for Yellow.
                if (IsOpeningComboTurn() && cardId == CardId.JailChicken)
                {
                    ChainInfo chickenSolving = Duel.GetCurrentSolvingChainInfo();
                    bool ownSolving = chickenSolving != null && chickenSolving.ActivatePlayer == 0;
                    bool accelReviveChicken = ownSolving && chickenSolving.IsActivateCode(CardId.AccelSynchroStardust);
                    bool mediusOrMixedChicken = ownSolving
                        && chickenSolving.IsActivateCode(CardId.MediusTheInnocent, CardId.MixedHellGod);
                    if (accelReviveChicken)
                    {
                        ElfnoteLog("OnSelectPlace Accel ① chicken off z2");
                        int extraAccel = SelectExtraMonsterZone(available);
                        if (extraAccel != 0) return extraAccel;
                        List<int> sideAccel = Util.ShuffleList(new List<int> { 0, 4, 1, 3 });
                        foreach (int zoneId in sideAccel)
                        {
                            int zone = (int)Math.Pow(2, zoneId);
                            if ((available & zone) != 0 && Bot.MonsterZone[zoneId] == null)
                                return zone;
                        }
                    }
                    else if (mediusOrMixedChicken && (available & Zones.z2) != 0 && Bot.MonsterZone[2] == null)
                    {
                        ElfnoteLog("OnSelectPlace opening Medius/Mixed chicken to z2");
                        return Zones.z2;
                    }
                }
                bool mustCenter = NeedCenterCard(cardId) || cardId == CardId.ElvenotesWind;
                ChainInfo solving = Duel.GetCurrentSolvingChainInfo();
                bool white10Solving = solving != null && solving.ActivatePlayer == 0 && solving.IsActivateCode(CardId.White10);
                bool white7Solving = solving != null && solving.ActivatePlayer == 0 && solving.IsActivateCode(CardId.White7);
                if (white10Solving && Duel.Player == 1)
                {
                    // Unused Wind ② needs z2. Else chicken occupies z2 so Red/Blue/Yellow
                    // sit in a side main zone and can ③ immediately. Else Yellow-Red-Blue.
                    if ((available & Zones.z2) != 0 && Bot.MonsterZone[2] == null
                        && White10EnemyTurnWantsThisCardInCenter(cardId))
                    {
                        ElfnoteLog("OnSelectPlace White10 enemy z2 card=" + cardId
                            + " want=" + White10EnemyTurnCenterCardId());
                        return Zones.z2;
                    }
                    // Red/Blue/Yellow must enter a main-zone side slot for ③ swap (sequence<5)
                    if (cardId == CardId.ElvenotesRed || cardId == CardId.ElvenotesBlue || cardId == CardId.ElvenotesYellow)
                    {
                        List<int> sideRby = Util.ShuffleList(new List<int> { 0, 4, 1, 3 });
                        foreach (int zoneId in sideRby)
                        {
                            int zone = (int)Math.Pow(2, zoneId);
                            if ((available & zone) != 0 && Bot.MonsterZone[zoneId] == null)
                                return zone;
                        }
                    }
                    int extraPlace = SelectExtraMonsterZone(available);
                    if (extraPlace != 0) return extraPlace;
                    List<int> side = Util.ShuffleList(new List<int> { 0, 4, 1, 3 });
                    foreach (int zoneId in side)
                    {
                        int zone = (int)Math.Pow(2, zoneId);
                        if ((available & zone) != 0 && Bot.MonsterZone[zoneId] == null)
                            return zone;
                    }
                }

                // After our White 10 bounces: chicken into z2 only when +3 can still make White 10.
                // Unused Wind ② takes z2 next. Otherwise leave z2 empty for unused Yellow/Red/Blue hand-SS.
                if (white10Solving && Duel.Player == 0)
                {
                    bool chickenWantsCenter = cardId == CardId.JailChicken
                        && !activatedCardIdList.Contains(CardId.JailChicken + 1)
                        && Bot.HasInExtra(CardId.White10)
                        && (available & Zones.z2) != 0
                        && Bot.MonsterZone[2] == null;
                    if (chickenWantsCenter)
                        return Zones.z2;
                    if (cardId == CardId.JailChicken && activatedCardIdList.Contains(CardId.JailChicken + 1))
                        ElfnoteLog("OnSelectPlace White10: keep chicken off z2, +3 already used");
                    // Unused Wind ② only works in z2. After +3, chicken stays off-center so Wind can take it.
                    bool windWantsCenter = cardId == CardId.ElvenotesWind
                        && !activatedCardIdList.Contains(CardId.ElvenotesWind + 1)
                        && (available & Zones.z2) != 0
                        && Bot.MonsterZone[2] == null;
                    if (windWantsCenter)
                    {
                        ElfnoteLog("OnSelectPlace White10: Wind to z2 for unused ②");
                        return Zones.z2;
                    }
                    int extraOur = SelectExtraMonsterZone(available);
                    if (extraOur != 0) return extraOur;
                    List<int> sideOur = Util.ShuffleList(new List<int> { 0, 4, 1, 3 });
                    foreach (int zoneId in sideOur)
                    {
                        int zone = (int)Math.Pow(2, zoneId);
                        if ((available & zone) != 0 && Bot.MonsterZone[zoneId] == null)
                            return zone;
                    }
                }

                // Own-turn White 7: leave z2 for unused hand Red/Blue/Yellow, including a 6-star
                // chicken GY ③ is about to search (place happens before that trigger).
                if (cardId == CardId.White7 && Duel.Player == 0
                    && (HandHasSixStarWaitingToSs() || ChickenGySearchWillAddHandSixStar()))
                {
                    ElfnoteLog("OnSelectPlace White7: leave z2 for hand 6-star gySearch="
                        + ChickenGySearchWillAddHandSixStar());
                    int extraWhite7 = SelectExtraMonsterZone(available);
                    if (extraWhite7 != 0) return extraWhite7;
                    List<int> sideWhite7 = Util.ShuffleList(new List<int> { 0, 4, 1, 3 });
                    foreach (int zoneId in sideWhite7)
                    {
                        int zone = (int)Math.Pow(2, zoneId);
                        if ((available & zone) != 0 && Bot.MonsterZone[zoneId] == null)
                            return zone;
                    }
                }

                // Own-turn White 7 ② SS is effect SS (no inherent z2 lock). If a hand 6-star
                // still needs center, put this Red/Blue/Yellow in a main side slot (sequence<5).
                if (white7Solving && Duel.Player == 0
                    && (cardId == CardId.ElvenotesRed || cardId == CardId.ElvenotesBlue || cardId == CardId.ElvenotesYellow)
                    && (HandHasSixStarWaitingToSs() || ChickenGySearchWillAddHandSixStar()))
                {
                    ElfnoteLog("OnSelectPlace White7 ② RBY off z2 for hand 6-star card=" + cardId);
                    List<int> sideWhite7Rby = Util.ShuffleList(new List<int> { 0, 4, 1, 3 });
                    foreach (int zoneId in sideWhite7Rby)
                    {
                        int zone = (int)Math.Pow(2, zoneId);
                        if ((available & zone) != 0 && Bot.MonsterZone[zoneId] == null)
                            return zone;
                    }
                }

                // Opponent turn White 7: ② SS works off-center; ATK boost needs z2 but we usually want disruption SS, not a 3000 beater.
                // Prefer Extra/side so z2 stays free for Yellow / Wind / other center-locked bodies.
                // If center empty and no Wind/RBY swap intent, White 7 may take center.
                if (cardId == CardId.White7 && Duel.Player == 1)
                {
                    bool keepCenterForWindOrRby = WantWindEnemyTurnSsChicken()
                        || DeckHasRbySwapTarget()
                        || (Bot.HasInGraveyard(CardId.ElvenotesWind) && !activatedCardIdList.Contains(CardId.ElvenotesWind + 1))
                        || Bot.Graveyard.Any(c => c != null && CanRbySwapNow(c.Id, true));
                    if (keepCenterForWindOrRby || Bot.MonsterZone[2] != null)
                    {
                        ElfnoteLog("OnSelectPlace White7: enemy turn off-center");
                        int extraEnemy = SelectExtraMonsterZone(available);
                        if (extraEnemy != 0) return extraEnemy;
                        List<int> sideEnemy = Util.ShuffleList(new List<int> { 0, 4, 1, 3 });
                        foreach (int zoneId in sideEnemy)
                        {
                            int zone = (int)Math.Pow(2, zoneId);
                            if ((available & zone) != 0 && Bot.MonsterZone[zoneId] == null)
                                return zone;
                        }
                    }
                }

                // Opponent turn: RBY for ③ must sit in main side (sequence<5), never EMZ.
                if (Duel.Player == 1 && Bot.MonsterZone[2] != null
                    && (cardId == CardId.ElvenotesRed || cardId == CardId.ElvenotesBlue || cardId == CardId.ElvenotesYellow))
                {
                    List<int> sideRby = Util.ShuffleList(new List<int> { 0, 4, 1, 3 });
                    foreach (int zoneId in sideRby)
                    {
                        int zone = (int)Math.Pow(2, zoneId);
                        if ((available & zone) != 0 && Bot.MonsterZone[zoneId] == null)
                            return zone;
                    }
                }

                if (mustCenter)
                {
                    if ((available & Zones.z2) != 0 && Bot.MonsterZone[2] == null)
                        return Zones.z2;
                }
                else
                {
                    int extraPlace = SelectExtraMonsterZone(available);
                    if (extraPlace != 0) return extraPlace;
                    List<int> preferSide = Util.ShuffleList(new List<int> { 0, 4, 1, 3 });
                    foreach (int zoneId in preferSide)
                    {
                        int zone = (int)Math.Pow(2, zoneId);
                        if ((available & zone) != 0 && Bot.MonsterZone[zoneId] == null)
                            return zone;
                    }
                }

                if ((available & Zones.z2) != 0 && Bot.MonsterZone[2] == null && mustCenter)
                    return Zones.z2;
            }
            return base.OnSelectPlace(cardId, player, location, available);
        }

        public int SelectExtraMonsterZone(int available)
        {
            List<int> extra = Util.ShuffleList(new List<int> { 5, 6 });
            foreach (int zoneId in extra)
            {
                int zone = (int)Math.Pow(2, zoneId);
                if ((available & zone) != 0 && Bot.MonsterZone[zoneId] == null)
                    return zone;
            }
            return 0;
        }

        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            if (AI.HaveSelectedPosition())
                return 0;

            NamedCard cardData = NamedCard.Get(cardId);
            if (cardData != null)
            {
                int cardAttack = cardData.Attack;
                // FA printed ATK is 0; on-field ATK is Level*300 (7 → 2100).
                if (cardId == CardId.FormulaAthleteLightning)
                {
                    cardAttack = 2100;
                    if (Util.IsTurn1OrMain2())
                    {
                        if (positions.Contains(CardPosition.FaceUpDefence))
                            return CardPosition.FaceUpDefence;
                        if (positions.Contains(CardPosition.FaceDownDefence))
                            return CardPosition.FaceDownDefence;
                    }
                }
                // Follow Apophis: turn 1 / Main 2 onward, ATK <= DEF → Defense
                if (Duel.Turn == 1 || Duel.Phase >= DuelPhase.Main2)
                {
                    if (cardAttack <= cardData.Defense)
                    {
                        if (positions.Contains(CardPosition.FaceUpDefence))
                            return CardPosition.FaceUpDefence;
                        if (positions.Contains(CardPosition.FaceDownDefence))
                            return CardPosition.FaceDownDefence;
                    }
                }
                // Opponent turn: higher DEF, or an enemy monster that beats this ATK → Defense
                if (Duel.Player == 1)
                {
                    if (cardData.Defense >= cardAttack || Util.IsOneEnemyBetterThanValue(cardAttack, true) || cardAttack < 2000)
                    {
                        if (positions.Contains(CardPosition.FaceUpDefence))
                            return CardPosition.FaceUpDefence;
                        if (positions.Contains(CardPosition.FaceDownDefence))
                            return CardPosition.FaceDownDefence;
                    }
                }
                int bestBotAttack = Math.Max(Util.GetBestPower(Bot, true), cardAttack);
                if (Util.IsAllEnemyBetterThanValue(bestBotAttack, true))
                {
                    if (positions.Contains(CardPosition.FaceUpDefence))
                        return CardPosition.FaceUpDefence;
                    if (positions.Contains(CardPosition.FaceDownDefence))
                        return CardPosition.FaceDownDefence;
                }
            }

            if (cardData != null && cardData.Attack == 0 && cardId != CardId.FormulaAthleteLightning
                && positions.Contains(CardPosition.FaceUpDefence))
                return CardPosition.FaceUpDefence;

            if (cardId == CardId.FormulaAthleteLightning)
                return 0;

            return base.OnSelectPosition(cardId, positions);
        }

        public bool ElfnoteMonsterRepos()
        {
            if (Card == null || !Card.IsFaceup()) return false;

            ClientCard center = Bot.MonsterZone[2];
            bool lockedByWhite10 = center != null && center.IsCode(CardId.White10) && center.IsFaceup() && !center.IsDisabled()
                && Card.Sequence != 2;
            bool white7OffCenter = Card.IsCode(CardId.White7) && Card.Sequence != 2;

            // White 10 locks attacks from non-center: flip to Defense
            if (lockedByWhite10)
                return Card.IsAttack();
            // White 7 ATK boost is off when not in center: flip to Defense
            if (white7OffCenter && Card.IsAttack())
                return true;

            // Match OnSelectPosition: opponent turn / Main 2 / turn 1, stay Defense if that is the correct position
            if (Duel.Player == 1 || Util.IsTurn1OrMain2())
            {
                if (Card.Defense >= Card.Attack && Card.IsAttack())
                    return true;
                if (Duel.Player == 1 && Util.IsOneEnemyBetterThanValue(Card.Attack, true) && Card.IsAttack())
                    return true;
                int bestBotAttack = Math.Max(Util.GetBestPower(Bot, true), Card.Attack);
                if (Util.IsAllEnemyBetterThanValue(bestBotAttack, true) && Card.IsAttack())
                    return true;
            }
            return false;
        }

        public override bool OnSelectYesNo(int desc)
        {
            ChainInfo yesNoSolving = Duel.GetCurrentSolvingChainInfo();
            // L1: opponent turn after Chicken +3. Script only asks synchro YesNo if a matching Extra synchro is summonable.
            if (Duel.Player == 1 && activatedCardIdList.Contains(CardId.JailChicken + 1))
            {
                int solvingId = yesNoSolving != null ? yesNoSolving.ActivateId : 0;
                ElfnoteLog("YesNo after Chicken +3 enemyTurn askedSynchro="
                    + (desc == Util.GetStringId(CardId.JailChicken, 2))
                    + " desc=" + desc
                    + " expectSynchro=" + Util.GetStringId(CardId.JailChicken, 2)
                    + " solvingId=" + solvingId
                    + " extraWhite10=" + Bot.HasInExtra(CardId.White10)
                    + " phase=" + Duel.Phase);
            }
            if (yesNoSolving != null && yesNoSolving.IsActivateCode(CardId.JailChicken))
            {
                ElfnoteLog("YesNo during Chicken desc=" + desc
                    + " expectSynchro=" + Util.GetStringId(CardId.JailChicken, 2)
                    + " player=" + Duel.Player + " phase=" + Duel.Phase
                    + " extraWhite10=" + Bot.HasInExtra(CardId.White10));
            }

            if (desc == Util.GetStringId(CardId.White7, 1))
            {
                // Own turn: keep levels for 7+1 Crystal / Accel / Omega.
                if (Duel.Player == 0 && Bot.HasInMonstersZone(CardId.White7) && Bot.HasInMonstersZone(CardId.JailChicken)
                    && HasEightSynchroInExtra())
                    return false;
                // Librarian 4+4 only when that line is real.
                if (Bot.HasInExtra(CardId.SuperLibrarian) && Bot.HasInMonstersZone(CardId.JailChicken)
                    && Bot.GetMonsters().Any(c => c != null && c.IsCode(CardId.White7)))
                {
                    if (Bot.HasInMonstersZone(CardId.MediusTheInnocent) || Bot.GetMonsters().Count(c => c != null && c.Level == 4) > 0)
                        return true;
                    if (!Bot.HasInExtra(CardId.CrystalWing) && !Bot.HasInExtra(CardId.AccelSynchroStardust)
                        && !Bot.HasInMonstersZone(CardId.CrystalWing) && !Bot.HasInMonstersZone(CardId.AccelSynchroStardust))
                        return true;
                }
                // Opponent turn: script lowers BOTH fields. Do not drop our unused Wind ② (6→3)
                // and do not strip an enemy that is already negated.
                if (Duel.Player == 1)
                {
                    bool keepWindLevel = Bot.GetMonsters().Any(c => c != null && c.IsCode(CardId.ElvenotesWind)
                        && c.IsFaceup() && c.Level >= 4)
                        && !activatedCardIdList.Contains(CardId.ElvenotesWind + 1);
                    if (keepWindLevel)
                    {
                        ElfnoteLog("White7 level-down YesNo enemyTurn: keep Wind level for ②");
                        return false;
                    }
                    bool enemyHigh = Enemy.GetMonsters().Any(c => c != null && c.IsFaceup() && c.Level >= 4
                        && !c.IsDisabled() && !IsAlreadyMarkedNegate(c));
                    ElfnoteLog("White7 level-down YesNo enemyTurn enemyHigh=" + enemyHigh);
                    return enemyHigh;
                }
                return false;
            }

            if (desc == Util.GetStringId(CardId.JailGodGate, 2))
            {
                if (!DefaultCheckWhetherBotCanSearch())
                {
                    ElfnoteLog("Jail Gate search YesNo: cannot search");
                    return false;
                }
                bool canSearch = Bot.HasInDeck(CardId.MediusTheInnocent) || Bot.HasInDeck(CardId.MixedHellGod);
                ElfnoteLog("Jail Gate search YesNo=" + canSearch);
                return canSearch;
            }

            if (desc == Util.GetStringId(CardId.JailChicken, 2))
            {
                // Script asks this after RegisterEffect(+3). Client Level often still shows the old value
                // (own-turn log: centerLv=6 here, then 9 only after the chain ends). Treat +3 as applied.
                if (chickenDodgeSynchroThisChain)
                {
                    ElfnoteLog("Chicken synchro YesNo dodge synchro=" + chickenDodgeSynchroCardId
                        + " extraWhite10=" + Bot.HasInExtra(CardId.White10) + " answer=True");
                    return true;
                }
                if (white10MissingPlaceLineThisChain || ShouldOpeningWhite10ForMissingPlace())
                {
                    bool extraWhite10Missing = Bot.HasInExtra(CardId.White10);
                    ElfnoteLog("Chicken synchro YesNo: opening missing-place White10 extra="
                        + extraWhite10Missing + " answer=" + extraWhite10Missing);
                    return extraWhite10Missing;
                }
                // Chicken's YesNo synchro is Elfnote-only (White 7 / White 10). Under monster GY
                // redirect or Fuwalos Baronne short line, +3 is for idle Baronne 9+1; refuse so
                // White 10 is not made here.
                if ((ShouldRerouteChickenSixUnderBanish() || IsFuwalosOnlyOpeningCompromise())
                    && Bot.HasInExtra(CardId.BaronneDeFleur))
                {
                    ElfnoteLog("Chicken synchro YesNo: idle Baronne not White10 fuwalos="
                        + IsFuwalosOnlyOpeningCompromise());
                    return false;
                }
                ClientCard center = GetCenterMonster();
                ClientCard chicken = Bot.GetMonsters().FirstOrDefault(c => c != null && c.IsCode(CardId.JailChicken) && c.IsFaceup());
                if (center != null && center.IsCode(CardId.JailChicken))
                {
                    bool selfPlus3 = Bot.HasInExtra(CardId.White10)
                        && Bot.GetMonsters().Any(c => c != null && c != center && IsElvenotesSixStar(c) && c.IsFaceup());
                    ElfnoteLog("Chicken synchro YesNo self-center lv=" + center.Level + " extraWhite10=" + Bot.HasInExtra(CardId.White10)
                        + " sixStar=" + selfPlus3 + " answer=" + selfPlus3
                        + " player=" + Duel.Player + " phase=" + Duel.Phase);
                    return selfPlus3;
                }
                int chickenLv = chicken != null ? chicken.Level : 1;
                int centerLv = center != null ? center.Level : 0;
                bool extraWhite10 = Bot.HasInExtra(CardId.White10);
                bool alreadyTen = extraWhite10 && center != null && centerLv + chickenLv == 10;
                bool plus3MakesTen = extraWhite10 && center != null && centerLv + 3 + chickenLv == 10;
                bool answer = alreadyTen || plus3MakesTen;
                if (answer && CenterYellowStillNeedsToPlaceRedField() && !EnemyHasFloodgateMonster())
                {
                    ElfnoteLog("Chicken synchro YesNo: unused Yellow should place Red Field first"
                        + " extraWhite10=" + extraWhite10 + " answer=False");
                    return false;
                }
                ElfnoteLog("Chicken synchro YesNo center=" + CardStr(center) + " centerLv=" + centerLv
                    + " chickenLv=" + chickenLv + " extraWhite10=" + extraWhite10
                    + " alreadyTen=" + alreadyTen + " plus3MakesTen=" + plus3MakesTen
                    + " answer=" + answer
                    + " player=" + Duel.Player + " phase=" + Duel.Phase);
                return answer;
            }

            if (desc == Util.GetStringId(CardId.RedField, 1))
                return EnemyHasWorthNegate();

            if (desc == Util.GetStringId(CardId.ElvenotesRed, 1))
                return Duel.Player == 1 && Bot.GetMonsters().Any(c => c != null && c.Level <= 6 && c.IsFaceup() && c.HasSetcode(SetcodeElvenotes));

            // Bounce after swap. Skip monsters already marked for Veiler/Imperm.
            if (desc == Util.GetStringId(CardId.ElvenotesRed, 2))
                return GetRedBounceableMonsters().Count > 0;

            if (desc == Util.GetStringId(CardId.ElvenotesBlue, 2))
                return Enemy.Hand.Count > 0 && Duel.Player == 1;

            if (desc == Util.GetStringId(CardId.ElvenotesYellow, 2))
                return Duel.Player == 1 && EnemyHasYellowBounceTarget();

            if (desc == Util.GetStringId(CardId.BlackRoseDragon, 0))
                return ShouldBlackRoseWipe();

            if (desc == Util.GetStringId(CardId.ThousandSpearDragon, 3))
                return Enemy.Hand.Count > 0;

            if (desc == Util.GetStringId(CardId.AccelSynchroStardust, 1)
                || desc == Util.GetStringId(CardId.AccelSynchroStardust, 2))
            {
                return Bot.HasInMonstersZone(CardId.JailChicken) || Bot.HasInGraveyard(CardId.JailChicken)
                    || Bot.GetMonsters().Any(c => c != null && c.IsTuner());
            }

            return base.OnSelectYesNo(desc);
        }

        public override int OnSelectOption(IList<int> options)
        {
            if (options.Count == 2 && options.Contains(1190) && options.Contains(1152))
            {
                ChainInfo solving = Duel.GetCurrentSolvingChainInfo();
                if (solving != null && solving.IsActivateCode(CardId.MediusTheInnocent))
                {
                    bool maxx = enemyResolvedEffectIdList.Contains(_CardId.MaxxC);
                    bool noZone = Bot.GetMonstersInMainZone().Count >= 5;
                    ClientCard selectedHint = null;
                    bool mixedPUsed = activatedCardIdList.Contains(CardId.MixedHellGod);
                    if (!DefaultCheckWhetherBotCanSearch())
                        return options.IndexOf(1152);
                    if (ShouldFuwalosMediusSsChickenFromDeck())
                        return options.IndexOf(1152);
                    if (Bot.HasInGraveyard(CardId.JailChicken) && !mixedPUsed)
                        return options.IndexOf(1190);
                    if (maxx || noZone)
                        return options.IndexOf(1190);
                    if (selectedHint != null && selectedHint.IsCode(CardId.WhitePendulum))
                        return options.IndexOf(1190);
                    if (selectedHint != null && selectedHint.IsCode(CardId.MixedHellGod))
                        return options.IndexOf(1190);
                    if (CheckShouldNoMoreSpSummon(CardLocation.Deck))
                        return options.IndexOf(1190);
                    return options.IndexOf(1152);
                }
            }

            ChainInfo current = Duel.GetCurrentChainCard() != null ? null : Duel.GetCurrentSolvingChainInfo();
            ClientCard chainCard = Duel.GetCurrentChainCard();
            if (chainCard != null && chainCard.IsCode(CardId.MixedHellGod) && chainCard.Location == CardLocation.SpellZone)
            {
                int mixedPick = PickMixedHellGodPendulumOption(options);
                if (mixedPick >= 0)
                    return mixedPick;
            }
            if (current != null && current.IsActivateCode(CardId.MixedHellGod))
            {
                int mixedPick = PickMixedHellGodPendulumOption(options);
                if (mixedPick >= 0)
                    return mixedPick;
            }

            return base.OnSelectOption(options);
        }

        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, int hint, bool cancelable)
        {
            if (AI.HaveSelectedCards())
                return null;

            if (hint == HintMsg.SynchroMaterial || hint == HintMsg.SYNCHRO)
            {
                IList<ClientCard> accelMats = SelectAccel2WindChickenMaterials(cards, min, max);
                if (accelMats != null)
                    return accelMats;
                return base.OnSelectCard(cards, min, max, hint, cancelable);
            }

            ClientCard activating = Duel.GetCurrentChainCard();
            ChainInfo solving = Duel.GetCurrentSolvingChainInfo();

            if (hint == HintMsg.Tribute || hint == HintMsg.Release)
                return SelectElfnoteTributes(cards, min, max);

            if (activating != null)
            {
                IList<ClientCard> costPick = SelectForActivatingCard(activating, cards, min, max, hint);
                if (costPick != null) return costPick;
            }

            if (solving != null)
            {
                IList<ClientCard> solvePick = SelectForSolvingCard(solving, cards, min, max, hint);
                if (solvePick != null) return solvePick;
            }

            if (hint == HintMsg.Disable || hint == HintMsg.Target || hint == HintMsg.Destroy || hint == HintMsg.Remove)
            {
                List<ClientCard> problems = GetProblematicEnemyCardList(true, false, 0);
                List<ClientCard> picked = new List<ClientCard>();
                foreach (ClientCard card in problems)
                {
                    if (cards.Contains(card) && !currentNegateCardList.Contains(card) && !currentDestroyCardList.Contains(card))
                        picked.Add(card);
                }
                foreach (ClientCard card in cards)
                {
                    if (card != null && card.Controller == 1 && !picked.Contains(card)
                        && !currentNegateCardList.Contains(card) && !currentDestroyCardList.Contains(card))
                        picked.Add(card);
                }
                if (picked.Count > 0)
                    return Util.CheckSelectCount(picked, cards, min, max);
            }

            if ((hint == HintMsg.Discard || hint == HintMsg.ToGrave)
                && cards != null && cards.Count > 0
                && cards.All(c => c != null && c.Controller == 0 && c.Location == CardLocation.Hand))
            {
                ElfnoteLog("hand discard/overflow by priority min=" + min + " max=" + max);
                return SelectHandDiscard(cards, min, max);
            }

            return base.OnSelectCard(cards, min, max, hint, cancelable);
        }

        public override IList<ClientCard> OnSelectTribute(IList<ClientCard> cards, int min, int max, int hint, bool cancelable)
        {
            IList<ClientCard> picked = SelectElfnoteTributes(cards, min, max);
            if (picked != null)
                return picked;
            return base.OnSelectTribute(cards, min, max, hint, cancelable);
        }

        public override IList<ClientCard> OnSelectSynchroMaterial(IList<ClientCard> cards,
            IList<ClientCard> mandatoryCards, int sum, int min, int max)
        {
            IList<ClientCard> accelMats = SelectAccel2WindChickenMaterials(cards, min, max);
            if (accelMats != null)
                return accelMats;
            return null;
        }

        public IList<ClientCard> SelectForActivatingCard(ClientCard activating, IList<ClientCard> cards, int min, int max, int hint)
        {
            if (activating.IsCode(CardId.BaronneDeFleur) && (hint == HintMsg.Destroy || hint == HintMsg.Target))
            {
                List<ClientCard> ranked = GetNormalEnemyTargetList(true, false);
                List<ClientCard> picked = new List<ClientCard>();
                foreach (ClientCard card in ranked)
                {
                    if (cards.Contains(card) && !picked.Contains(card))
                        picked.Add(card);
                }
                if (picked.Count > 0)
                {
                    if (!currentDestroyCardList.Contains(picked[0]))
                        currentDestroyCardList.Add(picked[0]);
                    ElfnoteLog("Baronne ① select " + CardStr(picked[0]));
                    return Util.CheckSelectCount(picked, cards, min, max);
                }
            }

            if (activating.IsCode(_CardId.EffectVeiler) && (hint == HintMsg.Disable || hint == HintMsg.Target))
            {
                ClientCard veilerPick = null;
                if (preferNegateCard != null)
                {
                    foreach (ClientCard card in cards)
                    {
                        if (card == null) continue;
                        if (card == preferNegateCard
                            || (card.Id == preferNegateCard.Id && card.Controller == preferNegateCard.Controller
                                && card.Location == preferNegateCard.Location && card.Sequence == preferNegateCard.Sequence))
                        {
                            veilerPick = card;
                            break;
                        }
                    }
                }
                if (veilerPick == null)
                {
                    if (Duel.CurrentChain != null)
                    {
                        for (int i = Duel.CurrentChain.Count - 1; i >= 0; i--)
                        {
                            ClientCard chainingCard = Duel.CurrentChain[i];
                            if (chainingCard == null || chainingCard == activating) continue;
                            if (chainingCard.Controller != 1) continue;
                            if (chainingCard.Location != CardLocation.MonsterZone) continue;
                            if (!cards.Contains(chainingCard)) continue;
                            veilerPick = chainingCard;
                            break;
                        }
                    }
                    if (veilerPick == null)
                    {
                        List<ClientCard> shouldNegate = GetMonsterListForTargetNegate(true, CardType.Monster);
                        foreach (ClientCard card in shouldNegate)
                        {
                            if (card != null && cards.Contains(card))
                            {
                                veilerPick = card;
                                break;
                            }
                        }
                    }
                }
                preferNegateCard = null;
                if (veilerPick != null)
                {
                    if (!currentNegateCardList.Contains(veilerPick))
                        currentNegateCardList.Add(veilerPick);
                    ElfnoteLog("Veiler target " + CardStr(veilerPick));
                    return Util.CheckSelectCount(new List<ClientCard> { veilerPick }, cards, min, max);
                }
            }

            if (activating.IsCode(CardId.ElvenotesWind) && hint == HintMsg.ToGrave)
            {
                ClientCard dodge = GetWindDodgeTarget();
                ClientCard cost = GetWindCostCard(activating);
                string candStr = "";
                if (cards != null)
                {
                    foreach (ClientCard card in cards)
                    {
                        if (card == null) continue;
                        if (candStr.Length > 0) candStr += ",";
                        candStr += CardStr(card);
                    }
                }
                ClientCard matched = null;
                ClientCard want = dodge != null ? dodge : cost;
                if (want != null && cards != null)
                {
                    foreach (ClientCard card in cards)
                    {
                        if (SameFieldCard(card, want))
                        {
                            matched = card;
                            break;
                        }
                    }
                    if (matched == null && dodge != null)
                    {
                        foreach (ClientCard card in cards)
                        {
                            if (card == null || card.Controller != 0) continue;
                            if (card.Location != CardLocation.MonsterZone) continue;
                            if (!card.IsCode(dodge.Id)) continue;
                            matched = card;
                            break;
                        }
                    }
                }
                bool containsCost = cost != null && cards != null && cards.Contains(cost);
                ElfnoteLog("Wind cost hint=" + hint
                    + " dodge=" + CardStr(dodge)
                    + " getCost=" + CardStr(cost)
                    + " contains=" + containsCost
                    + " pick=" + CardStr(matched)
                    + " candidates=" + candStr);
                if (matched != null)
                    return Util.CheckSelectCount(new List<ClientCard> { matched }, cards, min, max);
                if (dodge != null)
                {
                    ElfnoteLog("Wind cost dodge not in candidates, cancel if possible");
                    if (min == 0)
                        return new List<ClientCard>();
                    return null;
                }
                List<ClientCard> fallback = cards.Where(c => c != null && !IsPendulumZoneCard(c)
                    && c.HasSetcode(SetcodeElvenotes) && !c.HasType(CardType.Synchro)
                    && !c.IsCode(CardId.JailChicken, CardId.RedField)
                    && !CheckWhetherBotWillBeBanished(c)
                    && !(c.IsFacedown() && (c.Location == CardLocation.SpellZone || c.Location == CardLocation.MonsterZone)))
                    .ToList();
                ClientCard green = fallback.FirstOrDefault(c => c.IsCode(CardId.GreenField));
                if (green != null)
                    return Util.CheckSelectCount(new List<ClientCard> { green }, cards, min, max);
                if (fallback.Count > 0)
                    return Util.CheckSelectCount(fallback, cards, min, max);
                ElfnoteLog("Wind cost: no legal card in candidates, cancel if possible");
                if (min == 0)
                    return new List<ClientCard>();
            }

            if (activating.IsCode(CardId.GreenField, CardId.RedField) && hint == HintMsg.ToGrave)
            {
                string candStr = "";
                if (cards != null)
                {
                    foreach (ClientCard card in cards)
                    {
                        if (card == null) continue;
                        if (candStr.Length > 0) candStr += ",";
                        candStr += CardStr(card);
                    }
                }
                if (activating.IsCode(CardId.GreenField) && ShouldFuwalosGreenFieldSummonRed() && !IsCenterEmpty())
                {
                    ClientCard centerRed = GetCenterMonster();
                    if (centerRed != null && cards.Contains(centerRed) && centerRed.IsCode(CardId.ElvenotesBlue)
                        && IsGreenFieldAcceptableCost(centerRed, CardAttribute.Fire))
                    {
                        ElfnoteLog("Green Field cost Fuwalos center Blue " + CardStr(centerRed)
                            + " hint=" + hint + " min=" + min + " max=" + max + " candidates=" + candStr);
                        return Util.CheckSelectCount(new List<ClientCard> { centerRed }, cards, min, max);
                    }
                }
                if (activating.IsCode(CardId.GreenField) && WantGreenFieldSummonWind() && !IsCenterEmpty())
                {
                    ClientCard center = GetCenterMonster();
                    // Spec: send used Blue / used Red fodder so Wind can enter z2. Do not dump Yellow that just placed Red Field.
                    if (center != null && cards.Contains(center) && CanGreenFieldSendCenterForWind(center))
                    {
                        ElfnoteLog("Green Field cost center " + CardStr(center)
                            + " hint=" + hint + " min=" + min + " max=" + max + " candidates=" + candStr);
                        return Util.CheckSelectCount(new List<ClientCard> { center }, cards, min, max);
                    }
                }
                bool canWhite7 = activating.IsCode(CardId.RedField) && Bot.HasInGraveyard(CardId.White7);
                bool canWhite10 = activating.IsCode(CardId.RedField) && Bot.HasInGraveyard(CardId.White10);
                bool preferSpear = Duel.Player == 1 && Bot.HasInMonstersZone(CardId.ThousandSpearDragon) && EnemyHasWorthNegate();
                bool forNegate = activating.IsCode(CardId.RedField) && Duel.Player == 1 && EnemyHasWorthNegate();
                CardAttribute avoidAttr = 0;
                if (activating.IsCode(CardId.GreenField))
                {
                    if (WantGreenFieldSummonWind())
                        avoidAttr = CardAttribute.Wind;
                    else if (Bot.HasInDeck(CardId.JailChicken))
                        avoidAttr = CardAttribute.Fire;
                }
                if (activating.IsCode(CardId.GreenField))
                {
                    ClientCard greenCost = GetGreenFieldCostMonster(avoidAttr);
                    ElfnoteLog("Green Field cost activatePick=" + CardStr(greenCost)
                        + " hint=" + hint + " min=" + min + " max=" + max
                        + " avoidAttr=" + avoidAttr + " candidates=" + candStr);
                    if (greenCost != null && cards.Contains(greenCost))
                    {
                        ElfnoteLog("Green Field cost " + CardStr(greenCost));
                        return Util.CheckSelectCount(new List<ClientCard> { greenCost }, cards, min, max);
                    }
                    ClientCard legal = null;
                    foreach (ClientCard card in cards)
                    {
                        if (IsGreenFieldAcceptableCost(card, avoidAttr))
                        {
                            legal = card;
                            break;
                        }
                    }
                    if (legal != null)
                    {
                        ElfnoteLog("Green Field cost fallback legal " + CardStr(legal));
                        return Util.CheckSelectCount(new List<ClientCard> { legal }, cards, min, max);
                    }
                    ElfnoteLog("Green Field cost no legal candidate, skip synchro pad");
                    if (min == 0)
                        return new List<ClientCard>();
                    List<ClientCard> nonEnd = new List<ClientCard>();
                    foreach (ClientCard card in cards)
                    {
                        if (card != null && !IsGreenFieldEndBoardCost(card))
                            nonEnd.Add(card);
                    }
                    if (nonEnd.Count > 0)
                        return Util.CheckSelectCount(nonEnd, cards, min, max);
                    return Util.CheckSelectCount(cards, cards, min, max);
                }
                ClientCard cost = GetRedFieldCostMonster(canWhite7, canWhite10, preferSpear, forNegate, avoidAttr);
                if (cost == null || !cards.Contains(cost))
                    cost = GetRedFieldCostMonster(canWhite7, canWhite10, preferSpear, forNegate, 0);
                if (cost != null && cards.Contains(cost))
                {
                    ElfnoteLog("Red Field cost " + CardStr(cost));
                    return Util.CheckSelectCount(new List<ClientCard> { cost }, cards, min, max);
                }
                List<int> order = new List<int> { CardId.MediusTheInnocent, _CardId.MulcharmyFuwalos, _CardId.MulcharmyPurulia, CardId.GreatRighteousThief, CardId.ElvenotesWind, CardId.ElvenotesBlue, CardId.ElvenotesYellow };
                return PreferCards(cards, order, min, max);
            }

            if (activating.IsCode(CardId.RedField) && (hint == HintMsg.SpSummon || hint == HintMsg.Target))
            {
                bool chickenOnField = Bot.GetMonsters().Any(c => c != null && c.IsCode(CardId.JailChicken) && c.IsFaceup());
                if (Duel.Player == 1)
                {
                    // White 10 then White 7. Yellow ② cannot place another Red Field while this
                    // effect is resolving; do not rank it above Wind / Red / Blue.
                    List<int> order = new List<int> { CardId.White10, CardId.White7 };
                    if (!chickenOnField)
                        order.Add(CardId.JailChicken);
                    order.Add(CardId.ElvenotesWind);
                    order.Add(CardId.ElvenotesRed);
                    order.Add(CardId.ElvenotesBlue);
                    order.Add(CardId.ElvenotesYellow);
                    if (chickenOnField)
                        order.Add(CardId.JailChicken);
                    IList<ClientCard> enemyPick = PreferCards(cards, order, min, max);
                    if (enemyPick != null && enemyPick.Count > 0)
                        ElfnoteLog("Red Field GY SS enemyTurn " + CardStr(enemyPick[0])
                            + " chickenOnField=" + chickenOnField);
                    return enemyPick;
                }
                bool canEight = Bot.HasInExtra(CardId.CrystalWing) || Bot.HasInExtra(CardId.AccelSynchroStardust);
                bool gyWhite7 = cards.Any(c => c != null && c.IsCode(CardId.White7) && c.Location == CardLocation.Grave);
                bool hasSixSelect = Bot.GetMonsters().Any(c => c != null && IsElvenotesSixStar(c) && c.IsFaceup());
                bool preferWhite7Select = ShouldPreferRedFieldGyWhite7();
                ElfnoteLog("Red Field GY SS select hasSix=" + hasSixSelect
                    + " chickenOnField=" + chickenOnField
                    + " chickenEnables=" + RedFieldChickenEnablesSynchro()
                    + " preferWhite7=" + preferWhite7Select
                    + " center=" + CardStr(GetCenterMonster()));
                // Empty field / no synchro from chicken: SS White 7, then White 7 ② SS other Elvenotes.
                // Tuner already on field: SS White 7 for 7+1 instead of a second chicken.
                if (gyWhite7 && preferWhite7Select)
                {
                    ElfnoteLog("Red Field GY SS White7 chickenOnField=" + chickenOnField + " canEight=" + canEight
                        + " chickenEnables=" + RedFieldChickenEnablesSynchro());
                    List<int> white7First = new List<int> { CardId.White7, CardId.White10, CardId.JailChicken, CardId.ElvenotesYellow, CardId.ElvenotesBlue };
                    return PreferCards(cards, white7First, min, max);
                }
                List<int> selfOrder = new List<int> { CardId.JailChicken, CardId.White10, CardId.White7, CardId.ElvenotesYellow, CardId.ElvenotesBlue };
                return PreferCards(cards, selfOrder, min, max);
            }

            if (activating.IsCode(CardId.ThousandSpearDragon) && hint == HintMsg.Discard)
            {
                List<int> order = new List<int> { _CardId.MulcharmyFuwalos, _CardId.MulcharmyPurulia, CardId.ElvenotesBlue, CardId.GreatRighteousThief };
                return PreferCards(cards, order, min, max);
            }

            if (activating.IsCode(CardId.AncientFishDragon) && hint == HintMsg.Discard)
                return SelectHandDiscard(cards, min, max);

            if (activating.IsCode(CardId.AncientFishDragon) && (hint == HintMsg.Destroy || hint == HintMsg.Target))
            {
                List<ClientCard> problems = GetAncientFishDestroyTargets();
                List<ClientCard> picked = new List<ClientCard>();
                foreach (ClientCard card in problems)
                {
                    if (cards.Contains(card)) picked.Add(card);
                }
                foreach (ClientCard card in cards)
                {
                    if (card != null && card.Controller == 1 && !picked.Contains(card)
                        && !ShouldSkipAncientFishDestroy(card))
                        picked.Add(card);
                }
                if (picked.Count > 0)
                    return Util.CheckSelectCount(picked, cards, min, max);
            }

            if (activating.IsCode(CardId.ThousandSpearDragon) && hint == HintMsg.Remove)
            {
                return SelectThousandSpearBanish(cards, min, max);
            }

            if (activating.IsCode(CardId.JailChicken) && hint == HintMsg.Target)
            {
                ClientCard center = cards.FirstOrDefault(c => c != null && c.Sequence == 2 && c.Controller == 0);
                if (center != null)
                    return Util.CheckSelectCount(new List<ClientCard> { center }, cards, min, max);
            }

            if (activating.IsCode(CardId.MediusTheInnocent) && hint == HintMsg.ToDeck)
                return SelectMediusToDeck(cards, min, max);

            if (activating.IsCode(CardId.White7) && hint == HintMsg.SpSummon)
                return SelectWhite7SpSummon(cards, min, max);

            if (activating.Controller == 0 && activating.IsCode(CardId.PSYFramelordOmega)
                && activating.Location == CardLocation.Grave && hint == HintMsg.ToDeck)
                return SelectOmegaGyToDeck(cards, min, max);

            return null;
        }

        public IList<ClientCard> SelectForSolvingCard(ChainInfo solving, IList<ClientCard> cards, int min, int max, int hint)
        {
            if (solving.IsActivateCode(CardId.ElvenotesRed) && hint == HintMsg.AddToHand)
            {
                bool chickenFirst = !Bot.HasInHand(CardId.JailChicken)
                    && !enemyResolvedEffectIdList.Contains(_CardId.LockBird)
                    && !Bot.HasInMonstersZone(CardId.JailChicken)
                    && CheckWhetherCanSummon();
                bool needYellowForRedField = !chickenFirst
                    && !activatedCardIdList.Contains(CardId.ElvenotesYellow)
                    && !Bot.HasInSpellZone(CardId.RedField, true, true)
                    && (Bot.HasInDeck(CardId.RedField) || Bot.HasInHand(CardId.RedField))
                    && !Bot.HasInHand(CardId.ElvenotesYellow)
                    && !Bot.HasInMonstersZone(CardId.ElvenotesYellow)
                    && !spSummonedCardIdList.Contains(CardId.ElvenotesYellow)
                    && IsCenterEmpty();
                bool wantWhiteP = RedSearchWhitePCanTriggerDraw();
                List<int> order = new List<int>();
                if (chickenFirst)
                    order.Add(CardId.JailChicken);
                else if (needYellowForRedField)
                    order.Add(CardId.ElvenotesYellow);
                if (wantWhiteP)
                    order.Add(CardId.WhitePendulum);
                order.Add(CardId.ElvenotesWind);
                order.Add(CardId.ElvenotesBlue);
                order.Add(CardId.ElvenotesYellow);
                if (CountOwnedWhitePendulum() < 2 && !order.Contains(CardId.WhitePendulum))
                    order.Add(CardId.WhitePendulum);
                if (!chickenFirst)
                    order.Add(CardId.JailChicken);
                if (needYellowForRedField)
                    ElfnoteLog("Red search: Yellow to SS and place Red Field");
                if (wantWhiteP)
                    ElfnoteLog("Red search: White P can trigger draw");
                bool inWhiteP = false;
                bool inWind = false;
                bool inBlue = false;
                if (cards != null)
                {
                    foreach (ClientCard card in cards)
                    {
                        if (card == null) continue;
                        if (card.IsCode(CardId.WhitePendulum)) inWhiteP = true;
                        if (card.IsCode(CardId.ElvenotesWind)) inWind = true;
                        if (card.IsCode(CardId.ElvenotesBlue)) inBlue = true;
                    }
                }
                IList<ClientCard> redPick = PreferCards(cards, order, min, max);
                string pickStr = (redPick != null && redPick.Count > 0) ? CardStr(redPick[0]) : "none";
                ElfnoteLog("Red search pick=" + pickStr
                    + " inWhiteP=" + inWhiteP + " inWind=" + inWind + " inBlue=" + inBlue);
                return redPick;
            }

            if (solving.IsActivateCode(CardId.ElvenotesRed) && hint == HintMsg.ReturnToHand)
                return SelectRedBounceTarget(cards, min, max);

            if (solving.IsActivateCode(CardId.ElvenotesBlue) && hint == HintMsg.ToField)
            {
                if (!DefaultCheckWhetherBotCanSearch())
                    return PreferCards(cards, new List<int> { CardId.GreenField, CardId.InnocentArt }, min, max);
                bool greenInDeck = cards.Any(c => c != null && c.IsCode(CardId.GreenField));
                bool artInDeck = cards.Any(c => c != null && c.IsCode(CardId.InnocentArt));
                bool needGreen = greenInDeck && !Bot.HasInSpellZone(CardId.GreenField) && !Bot.HasInHand(CardId.GreenField);
                bool needArt = artInDeck && !Bot.HasInHand(CardId.MediusTheInnocent) && !needGreen;
                ElfnoteLog("Blue place needGreen=" + needGreen + " needArt=" + needArt);
                if (needGreen)
                    return PreferCards(cards, new List<int> { CardId.GreenField, CardId.InnocentArt }, min, max);
                if (needArt)
                    return PreferCards(cards, new List<int> { CardId.InnocentArt, CardId.GreenField }, min, max);
                if (greenInDeck)
                    return PreferCards(cards, new List<int> { CardId.GreenField, CardId.InnocentArt }, min, max);
                return PreferCards(cards, new List<int> { CardId.InnocentArt, CardId.GreenField }, min, max);
            }

            if (solving.IsActivateCode(CardId.ElvenotesYellow) && hint == HintMsg.ToField)
                return PreferCards(cards, new List<int> { CardId.RedField }, min, max);

            if (solving.IsActivateCode(CardId.ElvenotesYellow) && hint == HintMsg.ReturnToHand)
            {
                List<ClientCard> ranked = new List<ClientCard>();
                foreach (ClientCard card in cards)
                {
                    if (card != null && card.Controller == 1 && CanYellowBounceToHand(card)
                        && !IsAlreadyMarkedNegate(card) && !ranked.Contains(card))
                        ranked.Add(card);
                }
                foreach (ClientCard card in cards)
                {
                    if (card == null || ranked.Contains(card)) continue;
                    if (IsAlreadyMarkedNegate(card)) continue;
                    if (card.IsCode(CardId.RedField, CardId.GreenField) && card.Controller == 0) continue;
                    if (CanYellowBounceToHand(card))
                        ranked.Add(card);
                }
                if (ranked.Count > 0)
                    ElfnoteLog("Yellow bounce " + CardStr(ranked[0]));
                if (ranked.Count == 0)
                    return new List<ClientCard>();
                return Util.CheckSelectCount(ranked, cards, min, max);
            }

            if (solving.IsActivateCode(CardId.JailChicken) && hint == HintMsg.SpSummon)
            {
                List<int> synchroOrder = new List<int> { CardId.White10 };
                if (chickenDodgeSynchroThisChain && chickenDodgeSynchroCardId == CardId.White7)
                    synchroOrder = new List<int> { CardId.White7, CardId.White10 };
                else if (chickenDodgeSynchroThisChain)
                    synchroOrder = new List<int> { CardId.White10, CardId.White7 };
                IList<ClientCard> synchroPick = PreferCards(cards, synchroOrder, min, max);
                if (synchroPick != null && synchroPick.Count > 0)
                    ElfnoteLog("Chicken synchro pick " + CardStr(synchroPick[0]));
                return synchroPick;
            }

            if (solving.IsActivateCode(CardId.JailChicken) && hint == HintMsg.AddToHand)
            {
                // Blue ② must still have a continuous to place. Otherwise skip Blue.
                bool blueGreen = Bot.HasInDeck(CardId.GreenField)
                    && !activatedCardIdList.Contains(CardId.GreenField);
                bool blueArt = Bot.HasInDeck(CardId.InnocentArt)
                    && Bot.HasInDeck(CardId.MediusTheInnocent)
                    && !activatedCardIdList.Contains(CardId.MediusTheInnocent)
                    && CheckWhetherCanSummon();
                bool pickBlue = blueGreen || blueArt;
                List<int> order;
                // Fuwalos short line ends on Yellow placing Red Field. Do not search Blue
                // just because Green Field is still in deck (that would resume the full axis).
                if (IsFuwalosOnlyOpeningCompromise())
                    return PreferCards(cards, new List<int> { CardId.ElvenotesYellow, CardId.ElvenotesWind, CardId.ElvenotesRed, CardId.ElvenotesBlue }, min, max);
                if (pickBlue)
                    order = new List<int> { CardId.ElvenotesBlue, CardId.ElvenotesYellow, CardId.ElvenotesWind, CardId.ElvenotesRed };
                else
                    order = new List<int> { CardId.ElvenotesYellow, CardId.ElvenotesWind, CardId.ElvenotesRed, CardId.ElvenotesBlue };
                if (!DefaultCheckWhetherBotCanSearch())
                {
                    if (pickBlue)
                        order = new List<int> { CardId.ElvenotesBlue, CardId.ElvenotesYellow, CardId.ElvenotesWind };
                    else
                        order = new List<int> { CardId.ElvenotesYellow, CardId.ElvenotesWind, CardId.ElvenotesBlue };
                }
                else if (Bot.HasInHand(CardId.ElvenotesBlue) || Bot.HasInMonstersZone(CardId.ElvenotesBlue) || activatedCardIdList.Contains(CardId.ElvenotesBlue))
                    order = new List<int> { CardId.ElvenotesYellow, CardId.ElvenotesWind, CardId.ElvenotesRed, CardId.ElvenotesBlue };
                if (pickBlue && DefaultCheckWhetherBotCanSearch() && Bot.HasInSpellZone(CardId.RedField))
                {
                    if (!Bot.HasInHand(CardId.ElvenotesBlue) && !Bot.HasInMonstersZone(CardId.ElvenotesBlue))
                        order = new List<int> { CardId.ElvenotesBlue, CardId.ElvenotesWind, CardId.ElvenotesRed };
                }
                return PreferCards(cards, order, min, max);
            }

            if (solving.IsActivateCode(CardId.MediusTheInnocent) && hint == HintMsg.ToDeck)
                return SelectMediusToDeck(cards, min, max);

            if (solving.IsActivateCode(CardId.MediusTheInnocent) && hint == HintMsg.OperateCard)
            {
                bool maxx = enemyResolvedEffectIdList.Contains(_CardId.MaxxC);
                bool noZone = Bot.GetMonstersInMainZone().Count >= 4;
                bool mixedPUsed = activatedCardIdList.Contains(CardId.MixedHellGod);
                if (ShouldFuwalosMediusSsChickenFromDeck())
                    return PreferCards(cards, new List<int> { CardId.JailChicken, CardId.MixedHellGod, CardId.WhitePendulum }, min, max);
                if (!DefaultCheckWhetherBotCanSearch())
                    return PreferCards(cards, new List<int> { CardId.JailChicken }, min, max);
                if (!mixedPUsed && (maxx || noZone))
                    return PreferCards(cards, new List<int> { CardId.MixedHellGod, CardId.WhitePendulum, CardId.JailChicken }, min, max);
                if (mixedPUsed && (maxx || Bot.GetMonstersInMainZone().Count >= 5))
                    return PreferCards(cards, new List<int> { CardId.WhitePendulum, CardId.JailChicken, CardId.MixedHellGod }, min, max);
                if (Bot.HasInGraveyard(CardId.JailChicken) && !mixedPUsed)
                    return PreferCards(cards, new List<int> { CardId.MixedHellGod, CardId.WhitePendulum, CardId.JailChicken }, min, max);
                if (!Bot.HasInGraveyard(CardId.JailChicken) && Bot.HasInExtra(CardId.SuperLibrarian) && !noZone)
                    return PreferCards(cards, new List<int> { CardId.JailChicken, CardId.MixedHellGod, CardId.WhitePendulum }, min, max);
                if (NeedPendulumScale() && !mixedPUsed)
                    return PreferCards(cards, new List<int> { CardId.WhitePendulum, CardId.JailChicken, CardId.MixedHellGod }, min, max);
                return PreferCards(cards, new List<int> { CardId.JailChicken, CardId.MixedHellGod, CardId.WhitePendulum }, min, max);
            }

            if (solving.IsActivateCode(CardId.JailGodGate) && hint == HintMsg.ToGrave)
                return SelectJailGateMill(cards, min, max);

            if (solving.IsActivateCode(CardId.JailGodGate) && hint == HintMsg.AddToHand)
            {
                bool hasMedius = Bot.HasInHand(CardId.MediusTheInnocent) || Bot.HasInMonstersZone(CardId.MediusTheInnocent);
                ElfnoteLog("Jail Gate search hasHandMedius=" + Bot.HasInHand(CardId.MediusTheInnocent)
                    + " hasFieldMedius=" + Bot.HasInMonstersZone(CardId.MediusTheInnocent)
                    + " pick=" + (hasMedius ? "Mixed" : "Medius"));
                if (!hasMedius && cards.Any(c => c != null && c.IsCode(CardId.MediusTheInnocent)))
                    return PreferCards(cards, new List<int> { CardId.MediusTheInnocent, CardId.MixedHellGod }, min, max);
                return PreferCards(cards, new List<int> { CardId.MixedHellGod, CardId.MediusTheInnocent }, min, max);
            }

            if (solving.IsActivateCode(CardId.GreenField) && hint == HintMsg.SpSummon)
            {
                List<int> order;
                if (ShouldFuwalosGreenFieldSummonRed())
                    order = new List<int> { CardId.ElvenotesRed, CardId.JailChicken, CardId.ElvenotesYellow, CardId.ElvenotesBlue, CardId.ElvenotesWind };
                else if (WantGreenFieldSummonWind())
                    order = new List<int> { CardId.ElvenotesWind, CardId.JailChicken, CardId.ElvenotesYellow, CardId.ElvenotesBlue, CardId.ElvenotesRed };
                else if (CountFaceupChickenOnField() == 0)
                    order = new List<int> { CardId.JailChicken, CardId.ElvenotesYellow, CardId.ElvenotesBlue, CardId.ElvenotesRed };
                else
                    order = new List<int> { CardId.ElvenotesYellow, CardId.ElvenotesBlue, CardId.ElvenotesRed, CardId.JailChicken };
                IList<ClientCard> picked = PreferCards(cards, order, min, max);
                if (picked != null && picked.Count > 0)
                    ElfnoteLog("Green Field SS " + CardStr(picked[0]));
                return picked;
            }

            if (solving.IsActivateCode(CardId.ElvenotesWind) && hint == HintMsg.SpSummon)
            {
                IList<ClientCard> picked = SelectWindDeckSpSummon(cards, min, max);
                if (picked != null && picked.Count > 0)
                    ElfnoteLog("Wind SS " + CardStr(picked[0]));
                return picked;
            }

            if (solving.IsActivateCode(CardId.White7) && hint == HintMsg.SpSummon)
                return SelectWhite7SpSummon(cards, min, max);

            if (solving.IsActivateCode(CardId.White10) && hint == HintMsg.SpSummon)
                return SelectWhite10SpSummon(cards, min, max);

            if (solving.ActivatePlayer == 0 && solving.IsActivateCode(CardId.PSYFramelordOmega)
                && hint == HintMsg.ToDeck)
                return SelectOmegaGyToDeck(cards, min, max);

            if (solving.IsActivateCode(CardId.WhitePendulum) && hint == HintMsg.Discard)
            {
                List<int> order = new List<int> { _CardId.MulcharmyFuwalos, _CardId.MulcharmyPurulia, CardId.ElvenotesBlue, CardId.ElvenotesRed };
                return PreferCards(cards, order, min, max);
            }

            if (solving.IsActivateCode(CardId.WhitePendulum) && hint == HintMsg.AddToHand)
            {
                List<ClientCard> ranked = new List<ClientCard>();
                ClientCard green = cards.FirstOrDefault(c => c != null && c.IsCode(CardId.GreenField) && c.Location == CardLocation.Grave);
                if (green != null && !Bot.HasInHand(CardId.GreenField) && !Bot.HasInSpellZone(CardId.GreenField))
                    ranked.Add(green);
                ClientCard unusedRed = cards.FirstOrDefault(c => c != null && c.IsCode(CardId.ElvenotesRed) && c.Location == CardLocation.Grave && CanElvenotesRedSearch());
                ClientCard unusedYellow = cards.FirstOrDefault(c => c != null && c.IsCode(CardId.ElvenotesYellow) && c.Location == CardLocation.Grave && !activatedCardIdList.Contains(CardId.ElvenotesYellow));
                if (unusedRed != null) ranked.Add(unusedRed);
                if (unusedYellow != null) ranked.Add(unusedYellow);
                ClientCard field = cards.FirstOrDefault(c => c != null && (c.IsCode(CardId.RedField) || c.IsCode(CardId.GreenField)));
                if (field != null && !ranked.Contains(field)) ranked.Add(field);
                bool mixedPUsedThisTurn = activatedCardIdList.Contains(CardId.MixedHellGod);
                List<ClientCard> usedMixed = new List<ClientCard>();
                foreach (ClientCard card in cards)
                {
                    if (card == null || !card.IsCode(CardId.MixedHellGod) || ranked.Contains(card)) continue;
                    // Extra Mixed after this turn's pendulum is the spent copy; GY mill / leftover Extra is still unused.
                    if (mixedPUsedThisTurn && card.Location == CardLocation.Extra)
                        usedMixed.Add(card);
                    else
                        ranked.Add(card);
                }
                bool keepChickenForWhite7 = White7StillWantsGyChicken();
                List<ClientCard> remaining = new List<ClientCard>();
                List<ClientCard> extraMonsters = new List<ClientCard>();
                foreach (ClientCard card in cards)
                {
                    if (card == null || ranked.Contains(card) || usedMixed.Contains(card)) continue;
                    if (card.HasType(CardType.Fusion | CardType.Ritual | CardType.Synchro | CardType.Xyz | CardType.Link))
                    {
                        extraMonsters.Add(card);
                        continue;
                    }
                    if (keepChickenForWhite7 && card.IsCode(CardId.JailChicken) && card.Location == CardLocation.Grave)
                        continue;
                    remaining.Add(card);
                }
                if (keepChickenForWhite7)
                {
                    foreach (ClientCard card in cards)
                    {
                        if (card != null && card.IsCode(CardId.JailChicken) && card.Location == CardLocation.Grave
                            && !ranked.Contains(card))
                            ranked.Add(card);
                    }
                }
                foreach (ClientCard mixed in usedMixed)
                {
                    if (!ranked.Contains(mixed))
                        ranked.Add(mixed);
                }
                foreach (ClientCard card in remaining)
                {
                    if (!ranked.Contains(card))
                        ranked.Add(card);
                }
                foreach (ClientCard extra in extraMonsters)
                {
                    if (!ranked.Contains(extra))
                        ranked.Add(extra);
                }
                if (ranked.Count > 0)
                    ElfnoteLog("White P recycle pick " + CardStr(ranked[0])
                        + " order=" + string.Join(" > ", ranked.ConvertAll(CardStr).ToArray())
                        + (keepChickenForWhite7 ? " keepGyChickenForWhite7" : ""));
                return Util.CheckSelectCount(ranked, cards, min, max);
            }

            if (solving.IsActivateCode(CardId.InnocentArt) && hint == HintMsg.AddToHand)
                return PreferCards(cards, new List<int> { CardId.MediusTheInnocent }, min, max);

            if (solving.IsActivateCode(CardId.AccelSynchroStardust) && hint == HintMsg.SpSummon)
            {
                if (cards.Any(c => c != null && c.IsCode(CardId.JailChicken) && c.Location == CardLocation.Grave))
                    return PreferCards(cards, new List<int> { CardId.JailChicken }, min, max);
                if (cards.Any(c => c != null && c.IsCode(CardId.StardustDragon)))
                    return PreferCards(cards, new List<int> { CardId.StardustDragon, CardId.AncientFishDragon, CardId.ThousandSpearDragon }, min, max);
                // Second extra pick (Stardust already on field). Do not pick Black Rose here;
                // idle wipe still uses ShouldBlackRoseWipe (floodgate / invincible exceptions unchanged).
                List<int> secondOrder = new List<int>();
                if (CanAccelSecondSynchroUseWindForSeven())
                {
                    bool hasFormula = cards.Any(c => c != null && c.IsCode(CardId.FormulaAthleteLightning));
                    bool hasWhite7 = White7EffectStillAvailable()
                        && cards.Any(c => c != null && c.IsCode(CardId.White7));
                    if (hasFormula || hasWhite7)
                    {
                        if (hasFormula)
                            secondOrder.Add(CardId.FormulaAthleteLightning);
                        else
                            secondOrder.Add(CardId.White7);
                        accel2PreferWindChicken = true;
                        ElfnoteLog("Accel ② second synchro Wind ③ " + (hasFormula ? "Formula" : "White7"));
                        return PreferCards(cards, secondOrder, min, max);
                    }
                }
                if (CheckThousandSpearGoingFirstLine() || CheckThousandSpearGoingSecondLine())
                {
                    if (SelectSynchroMaterials(9, true))
                        ElfnoteLog("Accel ② second synchro 9-star mats queued");
                    else
                        ElfnoteLog("Accel ② second synchro 9-star no mats");
                    secondOrder.Add(CardId.ThousandSpearDragon);
                    secondOrder.Add(CardId.AncientFishDragon);
                    return PreferCards(cards, secondOrder, min, max);
                }
                if (SelectSynchroMaterials(9, true))
                    ElfnoteLog("Accel ② second synchro 9-star mats queued");
                secondOrder.Add(CardId.AncientFishDragon);
                secondOrder.Add(CardId.ThousandSpearDragon);
                secondOrder.Add(CardId.StardustDragon);
                return PreferCards(cards, secondOrder, min, max);
            }

            if (solving.IsActivateCode(CardId.ThousandSpearDragon) && (hint == HintMsg.Remove || hint == HintMsg.ReturnToHand))
                return SelectThousandSpearBanish(cards, min, max);

            if (solving.IsActivateCode(CardId.MixedHellGod) && hint == HintMsg.SpSummon)
            {
                ClientCard gyChicken = cards.FirstOrDefault(c => c != null && c.IsCode(CardId.JailChicken) && c.Location == CardLocation.Grave);
                if (gyChicken != null)
                {
                    ElfnoteLog("Mixed SS GY chicken");
                    return Util.CheckSelectCount(new List<ClientCard> { gyChicken }, cards, min, max);
                }
                ElfnoteLog("Mixed SS no GY chicken, PreferCards");
                return PreferCards(cards, new List<int> { CardId.JailChicken }, min, max);
            }

            if (solving.IsActivateCode(CardId.MixedHellGod) && hint == HintMsg.AddToHand)
                return SelectMixedHellGodSearch(cards, min, max);

            if (solving.IsActivateCode(CardId.RedField) && hint == HintMsg.Disable)
            {
                List<ClientCard> picked = GetRedFieldDisableOrder(cards);
                if (picked.Count > 0)
                {
                    currentNegateCardList.Add(picked[0]);
                    ElfnoteLog("Red Field disable " + CardStr(picked[0]));
                    return Util.CheckSelectCount(picked, cards, min, max);
                }
            }

            return null;
        }

        public IList<ClientCard> SelectThousandSpearBanish(IList<ClientCard> cards, int min, int max)
        {
            List<ClientCard> ranked = new List<ClientCard>();
            List<ClientCard> dangerGy = GetDangerousCardinEnemyGrave(false);
            List<ClientCard> problems = GetProblematicEnemyCardList(false, false, 0);
            foreach (ClientCard card in dangerGy)
            {
                if (cards.Contains(card) && !ranked.Contains(card) && !currentNegateCardList.Contains(card) && !currentDestroyCardList.Contains(card))
                    ranked.Add(card);
            }
            foreach (ClientCard card in problems)
            {
                if (cards.Contains(card) && !ranked.Contains(card) && !currentNegateCardList.Contains(card) && !currentDestroyCardList.Contains(card))
                    ranked.Add(card);
            }
            foreach (ClientCard card in cards)
            {
                if (card != null && card.Controller == 1 && !ranked.Contains(card))
                    ranked.Add(card);
            }
            return Util.CheckSelectCount(ranked, cards, min, max);
        }

        public bool NeedPendulumScale()
        {
            bool left = Bot.SpellZone[0] != null && Bot.SpellZone[0].HasType(CardType.Pendulum);
            bool right = Bot.SpellZone[4] != null && Bot.SpellZone[4].HasType(CardType.Pendulum);
            return !left || !right;
        }

        #endregion

        #region Counters and hand traps

        public bool EffectVeilerActivate()
        {
            if (CheckWhetherNegated(true)) return false;
            List<ClientCard> shouldNegateList = GetMonsterListForTargetNegate(true, CardType.Monster);
            if (shouldNegateList.Count > 0)
            {
                ClientCard last = Util.GetLastChainCard();
                if (last != null && last.IsCode(CardId.ElvenotesWind) && last.Controller == 1 && shouldNegateList.Count == 1)
                    return false;
                ClientCard target = shouldNegateList[0];
                if (last != null && last.Controller == 1 && last.Location == CardLocation.MonsterZone
                    && shouldNegateList.Contains(last))
                    target = last;
                preferNegateCard = target;
                ElfnoteLog("Veiler prefer " + CardStr(target));
                return true;
            }
            return false;
        }

        public bool AshBlossomActivate()
        {
            if (CheckWhetherNegated(true) || !CheckLastChainShouldNegated()) return false;
            ClientCard last = Util.GetLastChainCard();
            if (DefaultAshBlossomAndJoyousSpring())
            {
                if (last != null)
                    currentNegateCardList.Add(last);
                return true;
            }
            return false;
        }

        public bool BaronneNegateActivate()
        {
            if (ActivateDescription != Util.GetStringId(CardId.BaronneDeFleur, 1) && ActivateDescription != -1)
                return false;
            if (CheckWhetherNegated(true) || !CheckLastChainShouldNegated()) return false;
            ClientCard last = Util.GetLastChainCard();
            if (last != null) currentNegateCardList.Add(last);
            return true;
        }

        public bool BaronneDestroyActivate()
        {
            if (ActivateDescription == Util.GetStringId(CardId.BaronneDeFleur, 1))
                return false;
            if (Duel.Phase == DuelPhase.Standby || ActivateDescription == Util.GetStringId(CardId.BaronneDeFleur, 2))
                return BaronneStandbyReviveActivate();
            if (CheckWhetherNegated(true)) return false;
            List<ClientCard> targetList = GetNormalEnemyTargetList(true, false);
            if (targetList.Count == 0) return false;
            currentDestroyCardList.Add(targetList[0]);
            ElfnoteLog("Baronne ① destroy " + CardStr(targetList[0]));
            return true;
        }

        public bool BaronneStandbyReviveActivate()
        {
            // TODO: Baronne ③ Standby — return this card to Extra, SS a GY monster Level 9 or lower.
            return false;
        }

        public bool CrystalWingActivate()
        {
            if (CheckWhetherNegated(true) || !CheckLastChainShouldNegated()) return false;
            ClientCard last = Util.GetLastChainCard();
            if (last == null || !last.IsMonster()) return false;
            if (last != null) currentNegateCardList.Add(last);
            return true;
        }

        public bool FormulaAthleteActivate()
        {
            if (CheckWhetherNegated(true) || !CheckLastChainShouldNegated()) return false;
            ClientCard last = Util.GetLastChainCard();
            if (last == null || (!last.IsSpell() && !last.IsTrap())) return false;
            currentNegateCardList.Add(last);
            return true;
        }

        public bool GreatRighteousThiefNegateActivate()
        {
            if (CheckWhetherNegated(true) || !CheckLastChainShouldNegated()) return false;
            if (ActivateDescription == Util.GetStringId(CardId.GreatRighteousThief, 2)) return false;
            ClientCard last = Util.GetLastChainCard();
            if (last == null) return false;
            if (last.Location != CardLocation.Hand && last.Location != CardLocation.Grave && last.Location != CardLocation.Removed)
                return false;
            currentNegateCardList.Add(last);
            return true;
        }

        public bool GreatRighteousThiefBattleActivate()
        {
            if (ActivateDescription != Util.GetStringId(CardId.GreatRighteousThief, 2) && ActivateDescription != -1)
                return false;
            if (Duel.Phase < DuelPhase.BattleStart || Duel.Phase > DuelPhase.Battle) return false;
            return Enemy.GetMonsters().Any(c => c != null && c.IsAttack());
        }

        public bool AccelStardustActivate()
        {
            if (CheckWhetherNegated(true)) return false;
            if (Card.Location != CardLocation.MonsterZone) return false;
            if (ActivateDescription == Util.GetStringId(CardId.AccelSynchroStardust, 0) || ActivateDescription == -1)
            {
                if (Bot.HasInGraveyard(CardId.JailChicken) && !CheckShouldNoMoreSpSummon(CardLocation.Grave))
                {
                    activatedCardIdList.Add(CardId.AccelSynchroStardust);
                    return true;
                }
                if (ActivateDescription == Util.GetStringId(CardId.AccelSynchroStardust, 0))
                    return Bot.Graveyard.Any(c => c != null && c.IsTuner() && c.Level <= 2);
            }
            if (Duel.Phase != DuelPhase.Main1 && Duel.Phase != DuelPhase.Main2) return false;
            bool librarian = Bot.HasInMonstersZone(CardId.SuperLibrarian, true, false, true);
            if (Duel.Player == 0 && librarian)
            {
                activatedCardIdList.Add(CardId.AccelSynchroStardust + 1);
                return true;
            }
            if (Duel.Player == 1)
            {
                if (Accel2OwnWaitableOnChain())
                {
                    ElfnoteLog("skip Accel ②: own chicken/White10/Wind on chain, opponent has not inserted after");
                    return false;
                }
                if (ShouldWaitChickenWhite10BeforeAccel2())
                    return false;
                activatedCardIdList.Add(CardId.AccelSynchroStardust + 1);
                return true;
            }
            return false;
        }

        public bool StardustDragonActivate()
        {
            if (CheckWhetherNegated(true)) return false;
            ClientCard last = Util.GetLastChainCard();
            if (last == null) return true;
            return Duel.LastChainPlayer == 1;
        }

        public bool IsExtraDeckMonster(ClientCard card)
        {
            if (card == null) return false;
            return card.HasType(CardType.Fusion | CardType.Synchro | CardType.Xyz | CardType.Link);
        }

        public bool IsOwnUsedUpOmegaRecycleCard(ClientCard card)
        {
            if (card == null || card.Controller != 0) return false;
            if (card.IsCode(CardId.ElvenotesWind))
                return activatedCardIdList.Contains(CardId.ElvenotesWind)
                    || activatedCardIdList.Contains(CardId.ElvenotesWind + 1);
            if (card.IsCode(CardId.JailChicken))
                return activatedCardIdList.Contains(CardId.JailChicken)
                    || activatedCardIdList.Contains(CardId.JailChicken + 1);
            if (card.IsCode(CardId.MediusTheInnocent))
                return activatedCardIdList.Contains(CardId.MediusTheInnocent);
            if (card.IsCode(CardId.InnocentArt))
                return artSearched;
            return activatedCardIdList.Contains(card.Id);
        }

        /// <summary>
        /// Omega GY shuffle: Red/Green Field, enemy dangerous GY, own extra except
        /// White 10 / White 7, then used-up main, then White 10 / White 7.
        /// Return null so default OnSelectCard continues when none of those match.
        /// </summary>
        public IList<ClientCard> SelectOmegaGyToDeck(IList<ClientCard> cards, int min, int max)
        {
            List<string> candParts = new List<string>();
            if (cards != null)
            {
                foreach (ClientCard card in cards)
                {
                    if (card == null) continue;
                    candParts.Add(CardStr(card) + " p" + card.Controller);
                }
            }
            ElfnoteLog("PSY Omega GY ToDeck min=" + min + " max=" + max
                + " candidates=" + string.Join(",", candParts.ToArray()));

            if (cards == null)
                return null;

            ClientCard omegaSelf = Duel.GetCurrentChainCard();
            List<ClientCard> pool = new List<ClientCard>();
            foreach (ClientCard card in cards)
            {
                if (card == null) continue;
                if (omegaSelf != null && card == omegaSelf) continue;
                if (card.Controller == 0 && card.Location == CardLocation.Grave
                    && card.IsCode(CardId.PSYFramelordOmega))
                    continue;
                pool.Add(card);
            }

            List<ClientCard> ranked = new List<ClientCard>();
            string reason = null;
            int[] fieldIds = new int[] { CardId.RedField, CardId.GreenField };
            foreach (int fieldId in fieldIds)
            {
                foreach (ClientCard card in pool)
                {
                    if (ranked.Contains(card)) continue;
                    if (card.Controller != 0 || card.Location != CardLocation.Grave) continue;
                    if (!card.IsCode(fieldId)) continue;
                    if (reason == null) reason = card.IsCode(CardId.RedField) ? "red field" : "green field";
                    ranked.Add(card);
                }
            }

            List<ClientCard> dangerGy = GetDangerousCardinEnemyGrave(false);
            foreach (ClientCard danger in dangerGy)
            {
                if (danger == null || !pool.Contains(danger) || ranked.Contains(danger)) continue;
                if (reason == null) reason = "enemy danger GY";
                ranked.Add(danger);
            }

            foreach (ClientCard card in pool)
            {
                if (ranked.Contains(card)) continue;
                if (card.Location != CardLocation.Grave || card.Controller != 0) continue;
                if (!IsExtraDeckMonster(card)) continue;
                if (card.IsCode(CardId.White10, CardId.White7)) continue;
                if (reason == null) reason = "extra";
                ranked.Add(card);
            }
            foreach (ClientCard card in pool)
            {
                if (ranked.Contains(card)) continue;
                if (card.Location != CardLocation.Grave || card.Controller != 0) continue;
                if (IsExtraDeckMonster(card) || !IsOwnUsedUpOmegaRecycleCard(card)) continue;
                if (reason == null) reason = "used main";
                ranked.Add(card);
            }
            foreach (ClientCard card in pool)
            {
                if (ranked.Contains(card)) continue;
                if (card.Location != CardLocation.Grave || card.Controller != 0) continue;
                if (!card.IsCode(CardId.White10)) continue;
                if (reason == null) reason = "extra White10";
                ranked.Add(card);
            }
            foreach (ClientCard card in pool)
            {
                if (ranked.Contains(card)) continue;
                if (card.Location != CardLocation.Grave || card.Controller != 0) continue;
                if (!card.IsCode(CardId.White7)) continue;
                if (reason == null) reason = "extra White7";
                ranked.Add(card);
            }

            if (ranked.Count == 0)
            {
                ElfnoteLog("PSY Omega GY ToDeck default");
                return null;
            }

            IList<ClientCard> picked = Util.CheckSelectCount(ranked, cards, min, max);
            string pickStr = (picked != null && picked.Count > 0) ? CardStr(picked[0]) + " p" + picked[0].Controller : "none";
            ElfnoteLog("PSY Omega GY ToDeck pick=" + pickStr + " reason=" + reason);
            return picked;
        }

        public bool PSYOmegaActivate()
        {
            if (CheckWhetherNegated(true)) return false;

            if (Card.Location == CardLocation.Grave)
            {
                int dangerGy = GetDangerousCardinEnemyGrave(false).Count;
                bool redFieldGy = Bot.HasInGraveyard(CardId.RedField);
                ElfnoteLog("PSY Omega GY recycle activate dangerGy=" + dangerGy
                    + " redFieldGy=" + redFieldGy + " enemyGy=" + Enemy.Graveyard.Count);
                if (dangerGy > 0 || redFieldGy)
                    return true;
                return Enemy.Graveyard.Count > 0;
            }

            if (Card.Location != CardLocation.MonsterZone)
                return false;

            // ② opponent Standby: send 1 banished card to GY.
            if (Duel.Phase == DuelPhase.Standby)
                return Duel.Player == 1 && (Bot.Banished.Any(c => c != null) || Enemy.Banished.Any(c => c != null));

            // ① Quick: script allows Main1 or Main2. Strategy: first turn or Main 2
            // (do not banish again in later-turn Main 1 right after returning from banished).
            if (Duel.Phase != DuelPhase.Main1 && Duel.Phase != DuelPhase.Main2)
                return false;
            if (Enemy.Hand.Count <= 0)
                return false;
            if (Duel.CurrentChain.Count > 0 && Duel.LastChainPlayer == 0)
                return false;
            if (Duel.Player == 0 && Duel.Turn != 1 && Duel.Phase != DuelPhase.Main2)
                return false;
            ElfnoteLog("PSY Omega ① banish hand player=" + Duel.Player + " phase=" + Duel.Phase
                + " turn=" + Duel.Turn + " enemyHand=" + Enemy.Hand.Count);
            return true;
        }

        public bool ChaosAngelActivate()
        {
            if (CheckWhetherNegated(true)) return false;
            return EnemyHasChaosAngelBanishTarget();
        }

        public bool MaxxCActivate()
        {
            if (CheckWhetherNegated(true) || Duel.LastChainPlayer == 0 || !DefaultCheckWhetherBotCanDraw()) return false;
            return DefaultMaxxC();
        }

        public bool MulcharmyFuwalosActivate()
        {
            if (CheckWhetherNegated(true) || Duel.Player == 0) return false;
            if (!DefaultCheckWhetherBotCanDraw() || Duel.CurrentChain.Any(c => c.IsCode(_CardId.LockBird))) return false;
            if (Duel.Phase > DuelPhase.Main1) return false;
            if (CountActivatedMulcharmy() >= 2) return false;
            activatedCardIdList.Add(_CardId.MulcharmyFuwalos);
            return true;
        }

        public bool MulcharmyPuruliaActivate()
        {
            if (CheckWhetherNegated(true) || Duel.Player == 0) return false;
            if (!DefaultCheckWhetherBotCanDraw() || Duel.CurrentChain.Any(c => c.IsCode(_CardId.LockBird))) return false;
            if (Duel.Phase > DuelPhase.Main1) return false;
            // Script flag 84192580 <= 1: Fuwalos + Purulia (and Nyalus) may fire twice in total.
            if (CountActivatedMulcharmy() >= 2) return false;
            activatedCardIdList.Add(_CardId.MulcharmyPurulia);
            return true;
        }

        public int CountActivatedMulcharmy()
        {
            int n = 0;
            foreach (int id in activatedCardIdList)
            {
                if (id == _CardId.MulcharmyFuwalos || id == _CardId.MulcharmyPurulia || id == _CardId.MulcharmyNyalus)
                    n++;
            }
            return n;
        }

        public bool LockBirdActivate()
        {
            if (CheckWhetherNegated(true)) return false;
            if (Duel.Player == 0) return false;
            return true;
        }

        #endregion

        #region Starters

        public bool ElvenotesRedSpSummon()
        {
            if (CheckWhetherNegated(true, true, CardType.Monster)) return false;
            // Same as Blue/Yellow: Purulia only skips extra hand SS after a stop board.
            // Empty going-first still starts Red. Keep the Fuwalos+empty-center exception.
            if (CheckShouldNoMoreSpSummon(CardLocation.Hand) && CheckHasStopBoard())
            {
                if (!(enemyResolvedEffectIdList.Contains(_CardId.MulcharmyFuwalos) && IsCenterEmpty()))
                {
                    ElfnoteLog("skip Red SS: no more hand SS after stop board");
                    return false;
                }
            }
            if (CheckShouldNoMoreSpSummon() && CheckHasStopBoard()) return false;
            if (!IsCenterEmpty() && Card.Location == CardLocation.Hand) return false;
            if (ShouldSsYellowFirstAfterEightSynchro())
            {
                ElfnoteLog("skip Red SS: Yellow first after 8-synchro to place Red Field");
                return false;
            }
            if (ShouldSsYellowFirstToPlaceRedField())
            {
                ElfnoteLog("skip Red SS: Yellow first to place Red Field");
                return false;
            }
            // White7 ② chicken: do 7+1 Crystal/Accel before hand-SS Red/Blue/Yellow.
            if (CanSynchroEightWithWhite7())
            {
                ElfnoteLog("skip Red SS: White7+chicken for 8 first");
                return false;
            }
            if (CanSynchroSevenWithChicken())
            {
                ElfnoteLog("skip Red SS: 6+1 Formula first");
                return false;
            }
            if (!DefaultCheckWhetherBotCanSearch()
                && !Bot.HasInHand(CardId.JailChicken) && !Bot.HasInMonstersZone(CardId.JailChicken)
                && !Bot.HasInGraveyard(CardId.JailChicken))
            {
                ElfnoteLog("skip Red SS: cannot search and no chicken for 6+1");
                return false;
            }
            spSummonedCardIdList.Add(CardId.ElvenotesRed);
            return true;
        }

        public bool ElvenotesBlueSpSummon()
        {
            if (CheckWhetherNegated(true, true, CardType.Monster)) return false;
            if (CheckShouldNoMoreSpSummon(CardLocation.Hand) && CheckHasStopBoard()) return false;
            if (!IsCenterEmpty()) return false;
            if (ShouldSsYellowFirstAfterEightSynchro())
            {
                ElfnoteLog("skip Blue SS: Yellow first after 8-synchro to place Red Field");
                return false;
            }
            if (ShouldSsYellowFirstToPlaceRedField())
            {
                ElfnoteLog("skip Blue SS: Yellow first to place Red Field");
                return false;
            }
            if (CanSynchroEightWithWhite7())
            {
                ElfnoteLog("skip Blue SS: White7+chicken for 8 first");
                return false;
            }
            if (CanSynchroSevenWithChicken())
            {
                ElfnoteLog("skip Blue SS: 6+1 Formula first");
                return false;
            }
            if (Duel.Player == 0 && Bot.HasInHand(CardId.ElvenotesWind) && !activatedCardIdList.Contains(CardId.ElvenotesBlue)
                && Bot.HasInHand(CardId.ElvenotesRed) && IsCenterEmpty())
            {
                bool redStillUseful = DefaultCheckWhetherBotCanSearch()
                    || Bot.HasInHand(CardId.JailChicken) || Bot.HasInMonstersZone(CardId.JailChicken)
                    || Bot.HasInGraveyard(CardId.JailChicken);
                if (redStillUseful) return false;
            }
            spSummonedCardIdList.Add(CardId.ElvenotesBlue);
            return true;
        }

        public bool ElvenotesYellowSpSummon()
        {
            if (CheckWhetherNegated(true, true, CardType.Monster)) return false;
            if (CheckShouldNoMoreSpSummon(CardLocation.Hand) && CheckHasStopBoard()) return false;
            if (!IsCenterEmpty()) return false;
            if (ShouldKeepGyChickenForMixedHellGod())
            {
                ElfnoteLog("skip Yellow SS: keep GY chicken for Mixed Hell God pendulum");
                return false;
            }
            // After White7 SS chicken, 7+1 Crystal/Accel first. Yellow places Red Field only after the 8.
            if (CanSynchroEightWithWhite7())
            {
                ElfnoteLog("skip Yellow SS: White7+chicken for 8 first");
                return false;
            }
            if (CanSynchroSevenWithChicken())
            {
                ElfnoteLog("skip Yellow SS: 6+1 Formula first");
                return false;
            }
            ElfnoteLog("Yellow SS to center");
            spSummonedCardIdList.Add(CardId.ElvenotesYellow);
            return true;
        }

        public bool ElvenotesRedActivate()
        {
            if (CheckWhetherNegated(true)) return false;
            if (Duel.Player == 0 && (ActivateDescription == Util.GetStringId(CardId.ElvenotesRed, 0) || ActivateDescription == -1))
            {
                if (Card.Location != CardLocation.MonsterZone)
                {
                    ElfnoteLog("skip Red ②: desc=" + ActivateDescription + " loc=" + Card.Location
                        + " canSearch=" + DefaultCheckWhetherBotCanSearch()
                        + " alreadyUsed=" + activatedCardIdList.Contains(CardId.ElvenotesRed)
                        + " deckTarget=" + HasElvenotesSearchTargetInDeck());
                    return false;
                }
                if (!DefaultCheckWhetherBotCanSearch())
                {
                    ElfnoteLog("skip Red ②: cannot search desc=" + ActivateDescription + " loc=" + Card.Location
                        + " canSearch=False alreadyUsed=" + activatedCardIdList.Contains(CardId.ElvenotesRed)
                        + " deckTarget=" + HasElvenotesSearchTargetInDeck());
                    return false;
                }
                if (!HasElvenotesSearchTargetInDeck())
                {
                    ElfnoteLog("skip Red ②: no deck Elvenotes desc=" + ActivateDescription + " loc=" + Card.Location
                        + " canSearch=True alreadyUsed=" + activatedCardIdList.Contains(CardId.ElvenotesRed)
                        + " deckTarget=False");
                    return false;
                }
                activatedCardIdList.Add(CardId.ElvenotesRed);
                return true;
            }
            if (Duel.Player == 1 && Card.Location == CardLocation.MonsterZone && Card.Sequence != 2)
            {
                ClientCard center = GetCenterMonster();
                if (center == null || center.Sequence != 2) return false;
                // Bounce resolves first. Do not stack on Veiler/Imperm or the negate target leaves the field.
                if (Duel.CurrentChain != null)
                {
                    foreach (ClientCard chainCard in Duel.CurrentChain)
                    {
                        if (chainCard == null || chainCard.Controller != 0) continue;
                        if (targetNegateIdList.Contains(chainCard.Id))
                        {
                            ElfnoteLog("skip Red swap: already chained a targeting negate");
                            return false;
                        }
                    }
                }
                if (!ShouldActivateRedSwap())
                    return false;
                activatedCardIdList.Add(CardId.ElvenotesRed + 3);
                return true;
            }
            if (Duel.Player == 0)
            {
                ElfnoteLog("skip Red ②: desc=" + ActivateDescription + " loc=" + Card.Location
                    + " canSearch=" + DefaultCheckWhetherBotCanSearch()
                    + " alreadyUsed=" + activatedCardIdList.Contains(CardId.ElvenotesRed)
                    + " deckTarget=" + HasElvenotesSearchTargetInDeck());
            }
            return false;
        }

        public bool ElvenotesBlueActivate()
        {
            if (CheckWhetherNegated(true)) return false;
            if (Duel.Player == 0 && Card.Location == CardLocation.MonsterZone
                && (ActivateDescription == Util.GetStringId(CardId.ElvenotesBlue, 0) || ActivateDescription == -1)
                && Bot.GetSpellCountWithoutField() < 5)
            {
                bool needGreen = Bot.HasInDeck(CardId.GreenField) && !Bot.HasInSpellZone(CardId.GreenField) && !Bot.HasInHand(CardId.GreenField);
                bool hasMedius = Bot.HasInHand(CardId.MediusTheInnocent);
                bool artInDeck = Bot.HasInDeck(CardId.InnocentArt);
                bool greenInDeck = Bot.HasInDeck(CardId.GreenField);
                ElfnoteLog("Blue ② activate needGreen=" + needGreen + " hasMedius=" + hasMedius
                    + " artInDeck=" + artInDeck + " greenInDeck=" + greenInDeck);
                if (!DefaultCheckWhetherBotCanSearch() && !needGreen)
                {
                    ElfnoteLog("skip Blue place: cannot search Art and no Green Field need");
                    return false;
                }
                if (!Bot.HasInDeck(CardId.GreenField) && !Bot.HasInDeck(CardId.InnocentArt))
                    return false;
                activatedCardIdList.Add(CardId.ElvenotesBlue);
                return true;
            }
            if (Duel.Player == 1 && Card.Location == CardLocation.MonsterZone && Card.Sequence != 2 && GetCenterMonster() != null)
            {
                if (Enemy.Hand.Count <= 0) return false;
                activatedCardIdList.Add(CardId.ElvenotesBlue + 3);
                return true;
            }
            return false;
        }

        public bool ElvenotesYellowActivate()
        {
            if (CheckWhetherNegated(true)) return false;
            if (Duel.Player == 0 && Card.Location == CardLocation.MonsterZone)
            {
                if (Bot.HasInSpellZone(CardId.RedField, true, true)) return false;
                if (!Bot.HasInDeck(CardId.RedField) && !Bot.HasInHand(CardId.RedField)) return false;
                activatedCardIdList.Add(CardId.ElvenotesYellow);
                return true;
            }
            if (Duel.Player == 1 && Card.Location == CardLocation.MonsterZone && Card.Sequence != 2 && GetCenterMonster() != null)
            {
                bool bounce = EnemyHasYellowBounceTarget();
                if (!bounce)
                {
                    ElfnoteLog("skip Yellow swap: no face-up S/T that can return to hand");
                    return false;
                }
                activatedCardIdList.Add(CardId.ElvenotesYellow + 3);
                return true;
            }
            return false;
        }

        public bool ElvenotesWindActivate()
        {
            if (CheckWhetherNegated(true)) return false;
            if (Card.Location == CardLocation.Hand)
            {
                ClientCard dodge = GetWindDodgeTarget();
                if (dodge != null)
                {
                    ElfnoteLog("Wind dodge negate, send " + CardStr(dodge));
                    activatedCardIdList.Add(CardId.ElvenotesWind);
                    return true;
                }
                if (CheckShouldNoMoreSpSummon(CardLocation.Deck) || CheckShouldNoMoreSpSummon())
                {
                    bool fuwalosWind = IsFuwalosOnlyOpeningCompromise() && Duel.Player == 0
                        && (Bot.HasInDeck(CardId.JailChicken)
                            || (Bot.HasInHand(CardId.JailChicken) && CheckWhetherCanSummon()));
                    if (!fuwalosWind)
                        return false;
                }
                if (Duel.Phase == DuelPhase.Draw || Duel.Phase == DuelPhase.Standby)
                    return false;
                if (Duel.CurrentChain.Count > 0 && Duel.LastChainPlayer == 0)
                    return false;
                ClientCard cost = GetWindCostCard(Card);
                if (cost == null) return false;
                if (cost.HasType(CardType.Synchro)) return false;
                if (!IsCenterEmpty() && GetCenterMonster() != cost)
                    return false;
                // Place then ② first. Wind is registered before on-field Green Field, so skip here.
                if (Duel.Player == 0
                    && Bot.HasInSpellZone(CardId.GreenField)
                    && !activatedCardIdList.Contains(CardId.GreenField)
                    && GetGreenFieldCostMonster() != null)
                {
                    ElfnoteLog("skip Wind hand: unused Green Field ② first");
                    return false;
                }
                if (Duel.Player == 0 && CanSynchroEightWithWhite7())
                {
                    ElfnoteLog("skip Wind hand: keep White7+chicken for 8");
                    return false;
                }
                // Red + Wind: treat Wind as absent until the Red 6+1 line is done.
                // After NS chicken the old "chicken still in hand" skip expires; do not dump
                // the used Red to put Wind in z2 (White 7 never synchros, Wind stays center).
                if (Duel.Player == 0 && HasChickenAndLevel6OnField())
                {
                    ElfnoteLog("skip Wind hand: 6+1 on field, treat as no Wind");
                    return false;
                }
                // Red searched chicken: NS chicken for 6+1. Do not dump the 6-star to SS Wind.
                if (CheckWhetherCanSummon() && Bot.HasInHand(CardId.JailChicken)
                    && Bot.GetMonsters().Any(c => c != null && IsElvenotesSixStar(c) && c.IsFaceup()))
                {
                    ElfnoteLog("skip Wind hand: NS chicken for 6+1, treat as no Wind");
                    return false;
                }
                ClientCard centerNow = GetCenterMonster();
                if (Duel.Player == 0 && centerNow != null && centerNow.IsCode(CardId.JailChicken))
                {
                    ElfnoteLog("skip Wind hand: keep chicken in center");
                    return false;
                }
                if (Duel.Player == 0)
                {
                    if (cost.IsCode(CardId.ElvenotesRed, CardId.ElvenotesBlue, CardId.ElvenotesYellow) && cost.Location == CardLocation.Hand)
                        return false;
                }
                // Wind ② is once per name. After recycling the used Wind, a second copy from hand
                // cannot search and would dump Green Field for a vanilla body. Keep it for the opponent turn.
                if (Duel.Player == 0 && activatedCardIdList.Contains(CardId.ElvenotesWind + 1))
                {
                    ElfnoteLog("skip Wind hand: ② already used, keep for opponent turn");
                    return false;
                }
                if (Duel.Player == 1)
                {
                    if (ShouldWaitOwnEnemyTurnDisruption(false, true))
                    {
                        ElfnoteLog("skip Wind hand: wait own White7/RedField/White10/Chicken");
                        return false;
                    }
                    bool inMain = Duel.Phase == DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2;
                    bool white10Line = CanWindEnemyTurnWhite10Line();
                    if (inMain)
                    {
                        if (!white10Line)
                        {
                            ElfnoteLog("skip Wind hand enemy Main: White10 line unavailable");
                            return false;
                        }
                        ElfnoteLog("Wind hand enemy Main: White10 line");
                        activatedCardIdList.Add(CardId.ElvenotesWind);
                        return true;
                    }
                    // Battle: block direct attack when empty board.
                    if (IsBattlePhaseNow() && Bot.GetMonsterCount() == 0 && Bot.UnderAttack
                        && Bot.BattlingMonster == null && Enemy.BattlingMonster != null)
                    {
                        NamedCard windData = NamedCard.Get(CardId.ElvenotesWind);
                        int windAtk = windData != null ? windData.Attack : 2000;
                        int enemyAtk = Enemy.BattlingMonster.Attack;
                        if (enemyAtk < windAtk || enemyAtk >= Bot.LifePoints)
                        {
                            ElfnoteLog("Wind hand enemy Battle: block direct atk=" + enemyAtk);
                            activatedCardIdList.Add(CardId.ElvenotesWind);
                            return true;
                        }
                        return false;
                    }
                    // Outside Main: RBY disruption via Wind ② if Wind can enter center.
                    if (!white10Line
                        && !activatedCardIdList.Contains(CardId.ElvenotesWind + 1)
                        && (IsCenterEmpty() || GetCenterMonster() == cost)
                        && DeckHasRbySwapTarget())
                    {
                        ElfnoteLog("Wind hand enemy outside Main: RBY swap line");
                        activatedCardIdList.Add(CardId.ElvenotesWind);
                        return true;
                    }
                    ElfnoteLog("skip Wind hand enemy: no White10/Battle/RBY line");
                    return false;
                }
                activatedCardIdList.Add(CardId.ElvenotesWind);
                return true;
            }
            if (Card.Location == CardLocation.MonsterZone && Card.Sequence == 2)
            {
                if (CheckShouldNoMoreSpSummon(CardLocation.Deck))
                {
                    bool fuwalosWind2 = IsFuwalosOnlyOpeningCompromise()
                        && Bot.HasInDeck(CardId.JailChicken)
                        && !Bot.HasInMonstersZone(CardId.JailChicken)
                        && !Bot.HasInHand(CardId.JailChicken);
                    if (!fuwalosWind2)
                    {
                        ElfnoteLog("skip Wind ②: no more deck SS");
                        return false;
                    }
                    ElfnoteLog("Wind ② Fuwalos: SS chicken from deck");
                }
                activatedCardIdList.Add(CardId.ElvenotesWind + 1);
                return true;
            }
            return false;
        }

        public bool ElvenotesWindGyRecycleActivate()
        {
            if (CheckWhetherNegated(true)) return false;
            if (Card.Location != CardLocation.Grave) return false;
            ElfnoteLog("Wind GY recycle after synchro material");
            activatedCardIdList.Add(CardId.ElvenotesWind + 2);
            return true;
        }

        public bool GreenFieldActivateFromHand()
        {
            if (Card.Location != CardLocation.Hand) return false;
            if (CheckWhetherNegated(true, true, CardType.Spell)) return false;
            if (Bot.HasInSpellZone(CardId.GreenField)) return false;
            if (!activatedCardIdList.Contains(CardId.ElvenotesBlue))
                return false;
            ElfnoteLog("activate Green Field from hand after Blue ②");
            return true;
        }

        public bool GreenFieldActivate()
        {
            if (CheckWhetherNegated(true, true, CardType.Spell)) return false;
            if (Card.Location == CardLocation.Hand)
            {
                if (Bot.HasInSpellZone(CardId.GreenField)) return false;
                ElfnoteLog("activate Green Field from hand");
                return true;
            }
            if (CheckShouldNoMoreSpSummon(CardLocation.Deck) && !ShouldFuwalosGreenFieldSummonRed())
                return false;
            if (ShouldFuwalosGreenFieldSummonRed())
            {
                ElfnoteLog("Green Field ② Fuwalos: send center Blue, SS Red");
                activatedCardIdList.Add(CardId.GreenField);
                return true;
            }
            if (ShouldKeepSixStarForChickenPlus3())
            {
                ElfnoteLog("skip Green Field ②: keep 6-star for chicken +3");
                return false;
            }
            if (CountFaceupChickenOnField() >= 2 && !WantGreenFieldSummonWind())
            {
                bool canSsSix = Bot.HasInDeck(CardId.ElvenotesYellow) || Bot.HasInDeck(CardId.ElvenotesBlue)
                    || Bot.HasInDeck(CardId.ElvenotesRed);
                if (!canSsSix)
                {
                    ElfnoteLog("skip Green Field ②: already have enough chickens");
                    return false;
                }
            }
            CardAttribute greenAvoid = 0;
            if (WantGreenFieldSummonWind())
                greenAvoid = CardAttribute.Wind;
            else if (Bot.HasInDeck(CardId.JailChicken))
                greenAvoid = CardAttribute.Fire;
            ClientCard greenCost = GetGreenFieldCostMonster(greenAvoid);
            if (greenCost == null)
            {
                string remain = "";
                foreach (ClientCard monster in Bot.Hand.Concat(Bot.GetMonsters()))
                {
                    if (monster != null && monster.IsMonster())
                        remain += " " + CardStr(monster);
                }
                ElfnoteLog("skip Green Field ②: no cost" + remain);
                return false;
            }
            ElfnoteLog("Green Field ② will cost " + CardStr(greenCost)
                + " deckSsOk=" + GreenFieldDeckHasSsForCost(greenCost));
            if (greenCost.IsCode(CardId.ElvenotesYellow) && activatedCardIdList.Contains(CardId.ElvenotesYellow))
            {
                ElfnoteLog("skip Green Field ②: do not send Yellow after placing Red Field");
                return false;
            }
            if (!Bot.HasInDeck(CardId.ElvenotesWind) && !Bot.HasInDeck(CardId.JailChicken)
                && !Bot.HasInDeck(CardId.ElvenotesYellow) && !Bot.HasInDeck(CardId.ElvenotesBlue)
                && !Bot.HasInDeck(CardId.ElvenotesRed))
                return false;
            if (WantGreenFieldSummonWind())
            {
                if (!IsCenterEmpty())
                {
                    ClientCard center = GetCenterMonster();
                    // Spec: dump used Blue / used Red fodder so Wind can enter z2. Yellow that placed Red Field stays.
                    if (!CanGreenFieldSendCenterForWind(center))
                    {
                        ElfnoteLog("skip Green Field: Wind cannot go to center without sending used fodder, center=" + CardStr(center));
                        return false;
                    }
                    ElfnoteLog("Green Field: send center " + CardStr(center) + " so Wind can go to z2");
                }
            }
            activatedCardIdList.Add(CardId.GreenField);
            return true;
        }

        public bool RedFieldActivate()
        {
            if (CheckWhetherNegated(true, true, CardType.Trap)) return false;
            // Continuous trap: cannot activate from hand. Yellow ② places it face-up, otherwise Set it.
            if (Card.Location == CardLocation.Hand)
                return false;

            // e1 (flip only): no SetDescription → desc 0.
            // e4 (② SS/negate): aux.Stringid(id, 0) → GetStringId(RedField, 0).
            int effect2Desc = Util.GetStringId(CardId.RedField, 0);
            ElfnoteLog("Red Field ActivateDescription=" + ActivateDescription
                + " effect2Desc=" + effect2Desc
                + " facedown=" + Card.IsFacedown()
                + " loc=" + Card.Location);

            if (ActivateDescription == 0)
            {
                // Attack lock only matters in Battle while a monster sits in z2.
                if (Card.IsFacedown() && Card.Location == CardLocation.SpellZone)
                {
                    bool inBattle = Duel.Phase >= DuelPhase.BattleStart && Duel.Phase < DuelPhase.Main2;
                    if (inBattle && GetCenterMonster() != null)
                    {
                        ElfnoteLog("flip set Red Field: Battle, center occupied desc=" + ActivateDescription);
                        return true;
                    }
                }
                ElfnoteLog("skip Red Field flip desc=" + ActivateDescription);
                return false;
            }
            if (ActivateDescription != effect2Desc)
            {
                ElfnoteLog("skip Red Field: unknown ActivateDescription=" + ActivateDescription);
                return false;
            }

            if (Duel.Player == 1)
            {
                if (Duel.Phase == DuelPhase.Draw || Duel.Phase == DuelPhase.Standby)
                    return false;
                bool wantNegate = EnemyHasWorthNegate();
                if (!wantNegate)
                {
                    ElfnoteLog("skip Red Field enemy turn: nothing to negate, do not revive");
                    return false;
                }
                bool gyWhite7 = Bot.HasInGraveyard(CardId.White7);
                bool gyWhite10 = Bot.HasInGraveyard(CardId.White10);
                bool gyBoss = gyWhite7 || gyWhite10;
                if (!gyBoss)
                {
                    bool canRevive = Bot.Graveyard.Any(c => c != null && c.HasSetcode(SetcodeElvenotes));
                    if (canRevive)
                    {
                        ElfnoteLog("Red Field ② enemy turn revive no-boss desc=" + ActivateDescription);
                        SetRedFieldPendingNegate();
                    }
                    return canRevive;
                }
                bool preferSpear = Bot.HasInMonstersZone(CardId.ThousandSpearDragon) && wantNegate;
                ClientCard cost = GetRedFieldCostMonster(gyWhite7, gyWhite10, preferSpear, wantNegate);
                if (cost == null)
                {
                    ElfnoteLog("skip Red Field enemy turn: no cost wantNegate=" + wantNegate);
                    return false;
                }
                if (cost.IsCode(CardId.ThousandSpearDragon) && !wantNegate)
                    return false;
                ElfnoteLog("Red Field ② enemy turn cost " + CardStr(cost)
                    + " wantNegate=" + wantNegate + " desc=" + ActivateDescription);
                SetRedFieldPendingNegate();
                activatedCardIdList.Add(CardId.RedField);
                return true;
            }
            if (ShouldKeepGyChickenForMixedHellGod())
            {
                ElfnoteLog("skip Red Field: keep GY chicken for Mixed Hell God pendulum");
                return false;
            }
            if (ShouldWaitOwnChainToResolve())
            {
                ElfnoteLog("skip Red Field: own chain, opponent has not inserted, do not revive");
                return false;
            }
            if (!ShouldActivateRedFieldOnOurTurn())
            {
                if (White10OnFieldSecondEffectReady())
                    ElfnoteLog("skip Red Field: White 10 ② first");
                else
                    ElfnoteLog("skip Red Field: wait to revive chicken for extra synchro");
                return false;
            }
            bool reviveWhite7 = Bot.HasInGraveyard(CardId.White7) && !Bot.HasInMonstersZone(CardId.White7);
            bool reviveWhite10 = Bot.HasInGraveyard(CardId.White10) && !Bot.HasInMonstersZone(CardId.White10);
            bool preferWhite7 = ShouldPreferRedFieldGyWhite7();
            ClientCard selfCost = preferWhite7
                ? GetRedFieldCostMonster(true, false, false, false)
                : GetRedFieldCostMonster(reviveWhite7, reviveWhite10, false, false);
            if (selfCost == null) return false;
            bool hasSixNow = Bot.GetMonsters().Any(c => c != null && IsElvenotesSixStar(c) && c.IsFaceup());
            ElfnoteLog("Red Field ② our turn cost " + CardStr(selfCost)
                + " hasSix=" + hasSixNow
                + " chickenOnField=" + Bot.GetMonsters().Any(c => c != null && c.IsCode(CardId.JailChicken) && c.IsFaceup())
                + " chickenEnables=" + RedFieldChickenEnablesSynchro()
                + " preferWhite7=" + preferWhite7
                + " center=" + CardStr(GetCenterMonster())
                + " desc=" + ActivateDescription);
            activatedCardIdList.Add(CardId.RedField);
            return true;
        }

        public bool JailGodGateActivate()
        {
            if (CheckWhetherNegated(true, true, CardType.Spell)) return false;
            if (Card.Location == CardLocation.Grave)
            {
                if (!EnemyMonsterAcceptsJailGateBanish(Enemy.BattlingMonster)) return false;
                activatedCardIdList.Add(CardId.JailGodGate + 1);
                return true;
            }
            if (!DefaultCheckWhetherBotCanSearch())
            {
                ElfnoteLog("skip Jail Gate: cannot search");
                return false;
            }
            if (JailGatePredictedMillWouldBeBanished())
            {
                ElfnoteLog("skip Jail Gate: mill would be banished, search cannot resolve");
                return false;
            }
            if (Bot.HasInHand(CardId.InnocentArt) && !artSearched && Bot.HasInDeck(CardId.MediusTheInnocent))
            {
                ElfnoteLog("skip Jail Gate: let Innocent Art search first");
                return false;
            }
            if (CheckShouldNoMoreSpSummon() && CheckHasStopBoard()) return false;
            // After turn 1 Main 1: a face-up synchro is already a board; Gate would lock non-Jail attacks.
            if (!Util.IsTurn1OrMain2()
                && Bot.GetMonsters().Any(c => c != null && c.IsFaceup() && c.HasType(CardType.Synchro)))
            {
                ElfnoteLog("skip Jail Gate: Main1 after turn 1, face-up synchro on field");
                return false;
            }
            if (ShouldSkipJailGodGateForAttack())
            {
                ElfnoteLog("skip Jail Gate: attack-capable Main 1, no danger, can extend or battle");
                return false;
            }
            if (!Bot.HasInDeck(CardId.JailChicken) && !Bot.HasInDeck(CardId.MixedHellGod) && !Bot.HasInDeck(CardId.WhitePendulum)
                && !Bot.HasInExtra(CardId.MixedHellGod) && !Bot.HasInExtra(CardId.WhitePendulum))
                return false;
            activatedCardIdList.Add(CardId.JailGodGate);
            return true;
        }

        public bool InnocentArtActivate()
        {
            if (CheckWhetherNegated(true, true, CardType.Spell)) return false;
            if (Card.Location == CardLocation.Hand)
            {
                if (!activatedCardIdList.Contains(CardId.ElvenotesBlue) && Bot.GetMonsters().Any(c => c != null && c.IsCode(CardId.ElvenotesBlue) && c.IsFaceup()))
                {
                    ElfnoteLog("skip Innocent Art from hand: wait for Blue ②");
                    return false;
                }
                return DefaultCheckWhetherBotCanSearch();
            }
            if (!DefaultCheckWhetherBotCanSearch())
            {
                ElfnoteLog("skip Innocent Art: cannot search");
                return false;
            }
            if (!Bot.HasInDeck(CardId.MediusTheInnocent)) return false;
            artSearched = true;
            activatedCardIdList.Add(CardId.InnocentArt);
            return true;
        }

        public bool MediusSummon()
        {
            if (!CheckWhetherCanSummon()) return false;
            if (activatedCardIdList.Contains(CardId.MediusTheInnocent))
            {
                ElfnoteLog("skip Medius NS: ① already used");
                return false;
            }
            if (CheckShouldNoMoreSpSummon() && CheckHasStopBoard()) return false;
            // Red (or any 6-star) already on field + chicken in hand: NS chicken for 6+1.
            // NS Medius instead spends the Normal Summon; if ① is negated we stall with chicken stuck in hand.
            bool chickenInHand = Bot.HasInHand(CardId.JailChicken);
            bool sixOnField = Bot.GetMonsters().Any(c => c != null && IsElvenotesSixStar(c) && c.IsFaceup());
            if (chickenInHand && sixOnField)
            {
                ElfnoteLog("skip Medius NS: NS chicken for 6+1, keep Medius in hand");
                return false;
            }
            summonCount -= 1;
            return true;
        }

        public bool MediusActivate()
        {
            if (CheckWhetherNegated(true)) return false;
            if (Card.Location == CardLocation.Grave)
            {
                if (CheckShouldNoMoreSpSummon(CardLocation.Grave)) return false;
                if (!ShouldRerouteChickenSixUnderBanish()
                    && (CanChickenLevelForWhite10() || ShouldPreferWhite10Synchro()))
                {
                    ElfnoteLog("skip Medius GY: prefer White10 after chicken +3");
                    return false;
                }
                bool effect1CanFire = MediusEffect1CanFireAfterGySs();
                bool librarianLine = CanMediusGySsForLibrarianAfterEffect1Used();
                if (!effect1CanFire && !librarianLine)
                {
                    ElfnoteLog("skip Medius GY: ① cannot fire after SS and no Librarian line");
                    return false;
                }
                ElfnoteLog("Medius GY SS effect1CanFire=" + effect1CanFire + " librarianLine=" + librarianLine);
                activatedCardIdList.Add(CardId.MediusTheInnocent + 1);
                return true;
            }
            if (activatedCardIdList.Contains(CardId.MediusTheInnocent))
            {
                ElfnoteLog("skip Medius ①: already used this turn");
                return false;
            }
            if (!DefaultCheckWhetherBotCanSearch())
            {
                if (CheckShouldNoMoreSpSummon(CardLocation.Deck) || !Bot.HasInDeck(CardId.JailChicken)
                    || Bot.GetMonstersInMainZone().Count >= 5)
                {
                    ElfnoteLog("skip Medius ①: cannot search and cannot SS chicken");
                    return false;
                }
                ElfnoteLog("Medius ①: cannot search, SS chicken instead");
            }
            activatedCardIdList.Add(CardId.MediusTheInnocent);
            return true;
        }

        public bool MixedHellGodActivate()
        {
            if (CheckWhetherNegated(true)) return false;
            if (Card.Location == CardLocation.Hand)
            {
                if (activatedCardIdList.Contains(CardId.MixedHellGod))
                {
                    ElfnoteLog("skip Mixed place: pendulum already used this turn");
                    return false;
                }
                if (Bot.HasInSpellZone(CardId.MixedHellGod))
                {
                    ElfnoteLog("skip Mixed place: already in pendulum zone");
                    return false;
                }
                bool canSsPlace = CanMixedHellGodPendulumSs();
                bool canBuffPlace = CanMixedHellGodPendulumAtkBuff();
                ElfnoteLog("Mixed P2 check handMedius=" + Bot.HasInHand(CardId.MediusTheInnocent)
                    + " fieldMedius=" + Bot.HasInMonstersZone(CardId.MediusTheInnocent)
                    + " effect1Used=" + activatedCardIdList.Contains(CardId.MediusTheInnocent)
                    + " deckMedius=" + Bot.HasInDeck(CardId.MediusTheInnocent)
                    + " deckGate=" + Bot.HasInDeck(CardId.JailGodGate)
                    + " ss=" + canSsPlace + " p2=" + canBuffPlace);
                if (!canSsPlace && !canBuffPlace)
                {
                    ElfnoteLog("skip Mixed place: no jail SS target and no Gate-search P2");
                    return false;
                }
                bool need = NeedPendulumScale();
                ElfnoteLog("Mixed place from hand needScale=" + need
                    + " ss=" + canSsPlace + " p2=" + canBuffPlace);
                return need;
            }
            if (IsPendulumZoneCard(Card))
            {
                bool canSs = CanMixedHellGodPendulumSs();
                bool canBuff = CanMixedHellGodPendulumAtkBuff();
                ElfnoteLog("Mixed P2 check handMedius=" + Bot.HasInHand(CardId.MediusTheInnocent)
                    + " fieldMedius=" + Bot.HasInMonstersZone(CardId.MediusTheInnocent)
                    + " effect1Used=" + activatedCardIdList.Contains(CardId.MediusTheInnocent)
                    + " deckMedius=" + Bot.HasInDeck(CardId.MediusTheInnocent)
                    + " deckGate=" + Bot.HasInDeck(CardId.JailGodGate)
                    + " ss=" + canSs + " p2=" + canBuff);
                if (!canSs && !canBuff)
                {
                    ElfnoteLog("skip Mixed P: cannot SS jail and no Gate-search P2");
                    return false;
                }
                if (canSs && CountFreeMainMonsterZones() < 1 && CanSynchroToFreeMainZone())
                {
                    ElfnoteLog("skip Mixed P: no main zone, wait for synchro");
                    return false;
                }
                ElfnoteLog("Mixed P activate gyChicken=" + Bot.HasInGraveyard(CardId.JailChicken)
                    + " handChicken=" + Bot.HasInHand(CardId.JailChicken)
                    + " fieldChicken=" + Bot.HasInMonstersZone(CardId.JailChicken)
                    + " freeMain=" + CountFreeMainMonsterZones()
                    + " p2=" + canBuff);
                activatedCardIdList.Add(CardId.MixedHellGod);
                return true;
            }
            if (Card.Location == CardLocation.Extra)
            {
                if (!DefaultCheckWhetherBotCanSearch())
                {
                    ElfnoteLog("skip Mixed extra search: cannot search");
                    return false;
                }
                return true;
            }
            return Card.Location == CardLocation.Grave;
        }

        public bool WhitePendulumActivate()
        {
            if (CheckWhetherNegated(true)) return false;
            if (Card.Location == CardLocation.Hand)
            {
                if (Duel.Player != 0) return false;
                if (Bot.HasInSpellZone(CardId.WhitePendulum) && Bot.Hand.Count <= 7)
                {
                    ElfnoteLog("skip White P place: already on field, hand=" + Bot.Hand.Count);
                    return false;
                }
                return NeedPendulumScale();
            }
            return false;
        }

        public bool WhitePendulumDrawActivate()
        {
            if (CheckWhetherNegated(true)) return false;
            if (!IsPendulumZoneCard(Card)) return false;
            return DefaultCheckWhetherBotCanDraw();
        }

        public bool WhitePendulumRecycleActivate()
        {
            if (CheckWhetherNegated(true)) return false;
            if (Card.Location != CardLocation.Extra || !Card.IsFaceup()) return false;
            return true;
        }

        public bool JailChickenGySearchActivate()
        {
            if (Card.Location != CardLocation.Grave) return false;
            return JailChickenActivate();
        }

        public bool JailChickenActivate()
        {
            if (CheckWhetherNegated(true)) return false;
            if (Card.Location == CardLocation.Grave)
            {
                if (!DefaultCheckWhetherBotCanSearch())
                {
                    ElfnoteLog("skip Chicken GY: cannot search");
                    return false;
                }
                activatedCardIdList.Add(CardId.JailChicken);
                return true;
            }
            if (Card.Location == CardLocation.MonsterZone)
            {
                if (activatedCardIdList.Contains(CardId.JailChicken + 1)) return false;
                if (ShouldChickenPlus3DodgeNegate())
                {
                    chickenDodgeSynchroThisChain = true;
                    chickenDodgeSynchroCardId = GetChickenDodgeSynchroId();
                    ElfnoteLog("Chicken +3 dodge accept synchro=" + chickenDodgeSynchroCardId
                        + " plus3Lv=" + GetChickenPlus3SynchroLevel());
                    activatedCardIdList.Add(CardId.JailChicken + 1);
                    return true;
                }
                if (ShouldWaitOwnEnemyTurnDisruption(true, false))
                {
                    ElfnoteLog("skip Chicken +3: wait own White7/RedField/White10/Wind");
                    return false;
                }
                // Before the first 6+1 White 7, do not +3. Field/GY White 7, or ② already used, means 6+1 is done.
                // Same on going-second first turn if opponent passed. Monster GY redirect: +3 Baronne.
                // Fuwalos opening short line: +3 Baronne (6-star) or Accel (Medius).
                if (IsOpeningComboTurn() && !activatedCardIdList.Contains(CardId.White7)
                    && !Bot.HasInMonstersZone(CardId.White7) && !Bot.HasInGraveyard(CardId.White7)
                    && !enemyResolvedEffectIdList.Contains(_CardId.LockBird))
                {
                    bool fuwalosPlus3 = IsFuwalosOnlyOpeningCompromise()
                        && (CanChickenPlus3Baronne() || CanChickenPlus3Accel());
                    bool missingPlacePlus3 = ShouldOpeningWhite10ForMissingPlace();
                    if (!fuwalosPlus3 && !missingPlacePlus3
                        && (!ShouldRerouteChickenSixUnderBanish() || !CanChickenPlus3Baronne()))
                        return false;
                }
                ClientCard center = GetCenterMonster();
                if (center == null || center.Level < 1) return false;
                // White7 ② just revived chicken: 7+1 first. Do not +3 a hand-SS'd Yellow into White 10.
                if (CanSynchroEightWithWhite7())
                {
                    ElfnoteLog("skip Chicken +3: White7+chicken for 8 first");
                    return false;
                }
                // Chicken +3 is Quick; Red/Blue/Yellow ignition is not. After hand-SS the
                // check-monster window lets Chicken fire first and YesNo synchro eats unused ②.
                if (CenterElvenotesStillNeedsIgnition() && !EnemyHasFloodgateMonster())
                {
                    ElfnoteLog("skip Chicken +3: unused center ignition ② first center=" + CardStr(center));
                    return false;
                }
                // After +3, synchro level is (center + 3) + chicken. White 10 needs this == 10 (6-star in center).
                // White 7 in center is 7+3+1 = 11; keep 7+1 for Crystal Wing / Accel Stardust.
                bool plus3TenExtra = (ShouldRerouteChickenSixUnderBanish() || IsFuwalosOnlyOpeningCompromise())
                    ? Bot.HasInExtra(CardId.BaronneDeFleur)
                    : Bot.HasInExtra(CardId.White10);
                if (IsFuwalosOnlyOpeningCompromise() && CanChickenPlus3Accel())
                {
                    ElfnoteLog("Chicken +3 Accel accept target=" + CardStr(center) + " centerLv=" + center.Level
                        + " chickenLv=" + Card.Level);
                    activatedCardIdList.Add(CardId.JailChicken + 1);
                    return true;
                }
                if (center.Level + 3 + Card.Level == 10 && plus3TenExtra)
                {
                    // Opponent turn hard gate (Q20): after White 10 ② already resolved this turn,
                    // do not +3 again for White 10. Own turn keeps Baronne 10-star line.
                    if (Duel.Player == 1 && activatedCardIdList.Contains(CardId.White10))
                    {
                        ElfnoteLog("skip Chicken +3 enemyTurn: White10 ② already used");
                        return false;
                    }
                    ElfnoteLog("Chicken +3 accept target=" + CardStr(center) + " centerLv=" + center.Level
                        + " chickenLv=" + Card.Level + " player=" + Duel.Player + " phase=" + Duel.Phase);
                    if (ShouldOpeningWhite10ForMissingPlace())
                    {
                        white10MissingPlaceLineThisChain = true;
                        ElfnoteLog("Chicken +3 opening missing-place White10");
                    }
                    activatedCardIdList.Add(CardId.JailChicken + 1);
                    return true;
                }
                // Chicken in center: +3 itself to 4, then 4+6 with a leftover Yellow/Blue/Red.
                if (center == Card && plus3TenExtra
                    && Bot.GetMonsters().Any(c => c != null && c != Card && IsElvenotesSixStar(c) && c.IsFaceup()))
                {
                    if (Duel.Player == 1 && activatedCardIdList.Contains(CardId.White10))
                    {
                        ElfnoteLog("skip Chicken +3 self enemyTurn: White10 ② already used");
                        return false;
                    }
                    ElfnoteLog("Chicken +3 self in center, keep the 6-star on the side");
                    activatedCardIdList.Add(CardId.JailChicken + 1);
                    return true;
                }
            }
            return false;
        }

        public bool JailChickenSummonForWhite10Dodge()
        {
            if (!CheckWhetherCanSummon()) return false;
            if (Bot.GetMonsters().Any(c => c != null && c.IsCode(CardId.JailChicken) && c.IsFaceup()))
                return false;
            if (activatedCardIdList.Contains(CardId.JailChicken + 1)) return false;
            if (!White10EffectStillAvailable()) return false;
            if (ElvenotesSynchroSecondWouldBeNegatedOnSummon(CardId.White10)) return false;
            if (Duel.MainPhase == null || Duel.MainPhase.SpecialSummonableCards == null) return false;
            bool canSsRby = Duel.MainPhase.SpecialSummonableCards.Any(c => c != null
                && c.IsCode(CardId.ElvenotesRed, CardId.ElvenotesBlue, CardId.ElvenotesYellow));
            if (!canSsRby) return false;
            ElfnoteLog("NS Chicken first for White10 dodge line");
            nsChickenOffCenterForWhite10 = true;
            summonCount -= 1;
            return true;
        }

        public bool JailChickenSummon()
        {
            if (!CheckWhetherCanSummon()) return false;
            if (CheckShouldNoMoreSpSummon() && CheckHasStopBoard() && !HasChickenAndLevel6OnField())
            {
                if (Bot.GetMonsters().Any(c => c != null && IsElvenotesSixStar(c)))
                {
                    summonCount -= 1;
                    return true;
                }
                return false;
            }
            if (Bot.GetMonsters().Any(c => c != null && (IsElvenotesSixStar(c) || c.Level == 4)))
            {
                summonCount -= 1;
                return true;
            }
            return false;
        }

        public bool EffectVeilerSummon()
        {
            if (!ShouldNormalSummonVeilerForWhite7()) return false;
            ElfnoteLog("NS Veiler for 6+1 White 7 whiteP=" + WhitePendulumWaitingForElvenotesSs()
                + " blockedSix=" + (HandHasSixStarWaitingToSs() && GetCenterMonster() != null
                    && IsElvenotesSixStar(GetCenterMonster())));
            summonCount -= 1;
            return true;
        }

        public bool GreatRighteousThiefSummon()
        {
            if (!CheckWhetherCanSummon()) return false;
            if (Duel.Turn == 1)
            {
                if (Bot.GetMonsterCount() > 0) return false;
                if (Bot.HasInHand(CardId.JailChicken) && HandHasSixStarWaitingToSs())
                {
                    ElfnoteLog("skip Thief: Red line NS chicken first");
                    return false;
                }
                bool mediusGyLine = CanMediusGySsAfterThiefTribute();
                if (Bot.HasInHand(CardId.MediusTheInnocent)
                    && !activatedCardIdList.Contains(CardId.MediusTheInnocent + 1)
                    && !mediusGyLine)
                {
                    ElfnoteLog("skip Thief: Medius GY would bounce Thief");
                    return false;
                }
                int fodder = Bot.Hand.Count(c => c != null && c.IsCode(_CardId.MulcharmyFuwalos, _CardId.MulcharmyPurulia, _CardId.MulcharmyNyalus, CardId.MediusTheInnocent)
                    && !c.IsCode(CardId.GreatRighteousThief));
                fodder += Enemy.GetMonsterCount();
                if (!mediusGyLine && fodder < 2) return false;
                ElfnoteLog("Thief summon turn1 mediusGyLine=" + mediusGyLine + " fodder=" + fodder);
                summonCount -= 1;
                return true;
            }
            if (Bot.GetMonsterCount() == 0 && Enemy.GetMonsterCount() > 0)
            {
                ElfnoteLog("Thief tribute summon going-second enemyMon=" + Enemy.GetMonsterCount());
                summonCount -= 1;
                return true;
            }
            return false;
        }

        public bool White7Activate()
        {
            if (CheckWhetherNegated(true)) return false;
            if (ShouldWaitOwnChainToResolve())
            {
                ElfnoteLog("skip White7: wait own chain/search to finish");
                return false;
            }
            if (ShouldWaitRedFieldBeforeWhite7())
            {
                ElfnoteLog("skip White7: wait own Red Field ② to resolve");
                return false;
            }
            if (Duel.Phase != DuelPhase.Main1 && Duel.Phase != DuelPhase.Main2) return false;
            if (CheckShouldNoMoreSpSummon(CardLocation.Grave) && CheckShouldNoMoreSpSummon(CardLocation.Hand)) return false;
            if (Duel.Player == 0)
            {
                bool hasChicken = Bot.HasInGraveyard(CardId.JailChicken) || Bot.HasInHand(CardId.JailChicken);
                bool hasOtherElvenotes = Bot.Hand.Any(c => c != null && c.HasSetcode(SetcodeElvenotes) && c.Level <= 6)
                    || Bot.Graveyard.Any(c => c != null && c.HasSetcode(SetcodeElvenotes) && c.Level <= 6);
                if (!hasChicken && !hasOtherElvenotes)
                    return false;
                activatedCardIdList.Add(CardId.White7);
                return true;
            }
            // Opponent turn: only activate when a real target line exists.
            bool centerFilled = GetCenterMonster() != null;
            bool gyWind = Bot.HasInGraveyard(CardId.ElvenotesWind) && IsCenterEmpty()
                && !activatedCardIdList.Contains(CardId.ElvenotesWind + 1)
                && (WantWindEnemyTurnSsChicken() || DeckHasRbySwapTarget());
            bool gyRby = centerFilled && (
                GyRbyWorthWhite7Pull(CardId.ElvenotesRed)
                || GyRbyWorthWhite7Pull(CardId.ElvenotesBlue)
                || GyRbyWorthWhite7Pull(CardId.ElvenotesYellow));
            bool gyChicken = Bot.HasInGraveyard(CardId.JailChicken) && !activatedCardIdList.Contains(CardId.JailChicken + 1)
                && (HasEightSynchroInExtra() || White10EffectStillAvailable());
            bool handRby = centerFilled && (
                (Bot.HasInHand(CardId.ElvenotesRed) && CanRbySwapNow(CardId.ElvenotesRed, true)
                    && !HasFaceupRbyReadyToSwap(CardId.ElvenotesRed))
                || (Bot.HasInHand(CardId.ElvenotesBlue) && CanRbySwapNow(CardId.ElvenotesBlue, true)
                    && !HasFaceupRbyReadyToSwap(CardId.ElvenotesBlue))
                || (Bot.HasInHand(CardId.ElvenotesYellow) && CanRbySwapNow(CardId.ElvenotesYellow, true)
                    && !HasFaceupRbyReadyToSwap(CardId.ElvenotesYellow)));
            bool mainEndBody = (CurrentTiming & HintTimingMainEnd) != 0
                && Bot.Graveyard.Any(c => c != null && c.HasSetcode(SetcodeElvenotes) && c.Level <= 6
                    && !CheckShouldNoMoreSpSummon(CardLocation.Grave));
            if (!gyWind && !gyRby && !gyChicken && !handRby && !mainEndBody)
            {
                ElfnoteLog("skip White7 enemyTurn: no Wind/RBY/Chicken/MainEnd target");
                return false;
            }
            ElfnoteLog("White7 activate enemyTurn gyWind=" + gyWind + " gyRby=" + gyRby
                + " gyChicken=" + gyChicken + " handRby=" + handRby + " mainEnd=" + mainEndBody);
            activatedCardIdList.Add(CardId.White7);
            return true;
        }

        public bool White10Activate()
        {
            if (CheckWhetherNegated(true)) return false;
            if (ShouldWaitOwnChainToResolve(true))
            {
                ElfnoteLog("skip White10: wait own chain/White P recycle so hand 6-star can be SS'd");
                return false;
            }
            // White 10 is registered before White P place. Mixed extra search of White P only
            // draw-filters this turn if ② SS happens after the scale is down.
            if (Duel.Player == 0
                && Bot.HasInHand(CardId.WhitePendulum)
                && NeedPendulumScale()
                && !Bot.HasInSpellZone(CardId.WhitePendulum)
                && !activatedCardIdList.Contains(CardId.WhitePendulum)
                && DefaultCheckWhetherBotCanDraw())
            {
                ElfnoteLog("skip White10: place White P first so ② SS can draw-filter");
                return false;
            }
            if (Card.Sequence != 2) return false;
            if (Duel.Phase != DuelPhase.Main1 && Duel.Phase != DuelPhase.Main2) return false;
            if (CheckShouldNoMoreSpSummon(CardLocation.Deck) && Duel.Player == 0 && CheckHasStopBoard()) return false;
            activatedCardIdList.Add(CardId.White10);
            return true;
        }

        #endregion

        #region Synchro

        public bool White7SpSummon()
        {
            if (ShouldSkipExtraSynchroForFuwalosCompromise()) return false;
            if (CheckShouldNoMoreSpSummon(CardLocation.Extra)) return false;
            if (ShouldDeferSynchroForHelldiveBomber())
            {
                ElfnoteLog("skip White7: Helldive Bomber first");
                return false;
            }
            if (ShouldRerouteChickenSixUnderBanish())
            {
                ElfnoteLog("skip White7: monster GY redirect, chicken+6 reroute");
                return false;
            }
            if (ShouldOpeningWhite10ForMissingPlace())
            {
                ElfnoteLog("skip White7: opening missing-place White10 first");
                return false;
            }
            // White P may recycle White 7 to Extra after ②. A second copy this turn eats the leftover
            // tuner (chicken + Wind = 7) and blocks Ancient Fish + chicken = Baronne.
            // Do not use spSummoned: bounced back before ② should still remake White 7.
            if (activatedCardIdList.Contains(CardId.White7))
            {
                ElfnoteLog("skip White7: ② already used this turn, leftover mats for Baronne/FA");
                return false;
            }
            if (SelectSynchroMaterials(7, false))
                return true;
            return false;
        }

        public bool SuperLibrarianSpSummon()
        {
            if (ShouldSkipExtraSynchroForFuwalosCompromise()) return false;
            if (CheckShouldNoMoreSpSummon(CardLocation.Extra)) return false;
            if (ShouldDeferSynchroForHelldiveBomber())
            {
                ElfnoteLog("skip Librarian: Helldive Bomber first");
                return false;
            }
            if (ShouldPreferWhite10Synchro())
            {
                ElfnoteLog("skip Librarian: prefer White10 after chicken +3");
                return false;
            }
            // White 7 + one chicken + Medius: 5-star eats the last tuner and 7+1 dies.
            // Librarian first only with chicken+chicken+Level4+White7 (leftover tuner for Accel).
            if (CanSynchroEightWithWhite7() && !CanLibrarianThenEightSynchro())
            {
                ElfnoteLog("skip Librarian: White7+chicken for 8, not enough tuners left after 4+1");
                return false;
            }
            // One chicken + Medius + a leftover 6-star (Wind): 4+1 eats the last tuner,
            // leftover 6 cannot make Accel. Keep 6+1 (FA) instead.
            if (!CanLibrarianThenEightSynchro() && CountFaceupChickenOnField() < 2)
            {
                bool sixForSeven = Bot.GetMonsters().Any(c => c != null && c.IsFaceup() && !c.IsTuner() && c.Level == 6
                    && !c.IsCode(CardId.CrystalWing, CardId.FormulaAthleteLightning, CardId.BaronneDeFleur, CardId.SuperLibrarian));
                if (sixForSeven)
                {
                    ElfnoteLog("skip Librarian: only one chicken, keep 6+1");
                    return false;
                }
            }
            if (CanLibrarianThenEightSynchro())
                ElfnoteLog("Librarian then Accel: two chickens + 4-star + White7");
            if (SelectSynchroMaterials(5, false))
                return true;
            return false;
        }

        public bool AccelStardustSpSummon()
        {
            if (ShouldFuwalosCompromiseIdleAccel())
            {
                if (SelectSynchroMaterials(8, false))
                {
                    ElfnoteLog("Accel Fuwalos compromise idle 8");
                    return true;
                }
                return false;
            }
            if (ShouldSkipExtraSynchroForFuwalosCompromise()) return false;
            if (CheckShouldNoMoreSpSummon(CardLocation.Extra)) return false;
            if (ShouldDeferSynchroForHelldiveBomber())
            {
                ElfnoteLog("skip Accel: Helldive Bomber first");
                return false;
            }
            if (ShouldRerouteChickenSixUnderBanish())
            {
                ElfnoteLog("skip Accel: monster GY redirect, chicken+6 reroute");
                return false;
            }
            bool librarian = Bot.HasInMonstersZone(CardId.SuperLibrarian, true, false, true);
            bool wantNegate = Duel.Player == 0 && !Bot.HasInMonstersZone(CardId.CrystalWing) && !librarian
                && Bot.HasInExtra(CardId.CrystalWing);
            if (wantNegate && !librarian) return false;
            if (!librarian && Bot.HasInMonstersZone(CardId.CrystalWing))
            {
                ElfnoteLog("skip Accel: Crystal Wing already on field");
                return false;
            }
            if (!librarian && ShouldPreferWhite10Synchro())
            {
                ElfnoteLog("skip Accel: prefer White10 after chicken +3");
                return false;
            }
            if (SelectSynchroMaterials(8, false))
                return true;
            return false;
        }

        public bool CrystalWingSpSummon()
        {
            if (ShouldSkipExtraSynchroForFuwalosCompromise()) return false;
            if (CheckShouldNoMoreSpSummon(CardLocation.Extra)) return false;
            if (ShouldDeferSynchroForHelldiveBomber())
            {
                ElfnoteLog("skip Crystal Wing: Helldive Bomber first");
                return false;
            }
            if (ShouldRerouteChickenSixUnderBanish())
            {
                ElfnoteLog("skip Crystal Wing: monster GY redirect, chicken+6 reroute");
                return false;
            }
            if (Bot.HasInMonstersZone(CardId.SuperLibrarian, true, false, true) && Bot.HasInExtra(CardId.AccelSynchroStardust))
                return false;
            if (ShouldPreferWhite10Synchro())
            {
                ElfnoteLog("skip Crystal Wing: prefer White10 after chicken +3");
                return false;
            }
            if (SelectSynchroMaterials(8, false))
                return true;
            return false;
        }

        public bool White10SpSummon()
        {
            if (ShouldSkipExtraSynchroForFuwalosCompromise()) return false;
            if (CheckShouldNoMoreSpSummon(CardLocation.Extra)) return false;
            if (ShouldDeferSynchroForHelldiveBomber())
            {
                ElfnoteLog("skip White10: Helldive Bomber first");
                return false;
            }
            if (!White10EffectStillAvailable())
            {
                ElfnoteLog("skip White10 idle synchro: extra or ② already used");
                return false;
            }
            if (ShouldRerouteChickenSixUnderBanish())
            {
                ElfnoteLog("skip White10: monster GY redirect, chicken+6 reroute");
                return false;
            }
            ClientCard center = GetCenterMonster();
            ClientCard chicken = Bot.GetMonsters().FirstOrDefault(c => c != null && c.IsCode(CardId.JailChicken) && c.IsFaceup());
            if (SelectSynchroMaterials(10, false))
            {
                ElfnoteLog("White10 idle synchro player=" + Duel.Player + " phase=" + Duel.Phase
                    + " center=" + CardStr(center) + " centerLv=" + (center != null ? center.Level : 0)
                    + " chickenLv=" + (chicken != null ? chicken.Level : 0));
                return true;
            }
            ElfnoteLog("skip White10 idle synchro player=" + Duel.Player + " phase=" + Duel.Phase
                + " center=" + CardStr(center) + " centerLv=" + (center != null ? center.Level : 0)
                + " chickenLv=" + (chicken != null ? chicken.Level : 0)
                + " extraWhite10=" + Bot.HasInExtra(CardId.White10));
            return false;
        }

        public bool BaronneSpSummon()
        {
            if (ShouldFuwalosCompromiseIdleBaronne())
            {
                if (SelectSynchroMaterials(10, false, true))
                {
                    ElfnoteLog("Baronne Fuwalos compromise idle 10");
                    return true;
                }
                return false;
            }
            if (ShouldSkipExtraSynchroForFuwalosCompromise()) return false;
            if (CheckShouldNoMoreSpSummon(CardLocation.Extra)) return false;
            if (ShouldDeferSynchroForHelldiveBomber())
            {
                ElfnoteLog("skip Baronne: Helldive Bomber first");
                return false;
            }
            if (ShouldKeepSixStarForChickenPlus3())
            {
                ElfnoteLog("skip Baronne: chicken in center, +3 then synchro with the 6-star");
                return false;
            }
            if (!ShouldRerouteChickenSixUnderBanish()
                && CanMakeWhite10Synchro() && Bot.GetMonsters().Any(c => c != null && c.HasSetcode(SetcodeElvenotes)))
            {
                ElfnoteLog("skip Baronne: White 10 is actually makeable");
                return false;
            }
            if (ShouldSummonChaosAngel())
            {
                ElfnoteLog("skip Baronne: prefer Chaos Angel");
                return false;
            }
            if (SelectSynchroMaterials(10, false, true))
                return true;
            return false;
        }

        public bool FormulaAthleteSpSummon()
        {
            if (ShouldSkipExtraSynchroForFuwalosCompromise()) return false;
            if (CheckShouldNoMoreSpSummon(CardLocation.Extra)) return false;
            if (ShouldDeferSynchroForHelldiveBomber())
            {
                ElfnoteLog("skip Formula: Helldive Bomber first");
                return false;
            }
            if (ShouldKeepSixStarForChickenPlus3())
            {
                ElfnoteLog("skip Formula: chicken in center, +3 then synchro with the 6-star");
                return false;
            }
            if (ShouldOpeningWhite10ForMissingPlace())
            {
                ElfnoteLog("skip Formula: opening missing-place White10");
                return false;
            }
            if (ShouldRerouteChickenSixUnderBanish() && Duel.Turn >= 2
                && Bot.HasInExtra(CardId.BlackRoseDragon)
                && ShouldBlackRoseWipe(true))
            {
                ElfnoteLog("skip Formula: monster GY redirect, Black Rose first");
                return false;
            }
            if (!ShouldRerouteChickenSixUnderBanish()
                && (CanChickenLevelForWhite10() || ShouldPreferWhite10Synchro()))
            {
                ElfnoteLog("skip Formula: prefer chicken +3 into White10");
                return false;
            }
            // White 7 + chicken, or two chickens + a 6-star, should make Omega (8) before FA (7).
            if (CanSynchroEightWithWhite7() || CanSynchroEightWithTwoChickens())
            {
                ElfnoteLog("skip Formula: materials exist for 8 first");
                return false;
            }
            if (SelectSynchroMaterials(7, false))
                return true;
            return false;
        }

        public bool PSYOmegaSpSummon()
        {
            if (ShouldSkipExtraSynchroForFuwalosCompromise()) return false;
            if (CheckShouldNoMoreSpSummon(CardLocation.Extra)) return false;
            if (ShouldDeferSynchroForHelldiveBomber())
            {
                ElfnoteLog("skip Omega: Helldive Bomber first");
                return false;
            }
            // White 7 + chicken should go to Accel after Librarian only when 4+1 leaves a tuner.
            if (CanLibrarianThenEightSynchro())
            {
                ElfnoteLog("skip Omega: two chickens + 4-star + White7, Librarian then Accel");
                return false;
            }
            if (SelectSynchroMaterials(8, false))
                return true;
            return false;
        }

        public bool ChaosAngelSpSummon()
        {
            if (ShouldSkipExtraSynchroForFuwalosCompromise()) return false;
            if (CheckShouldNoMoreSpSummon(CardLocation.Extra)) return false;
            if (!ShouldSummonChaosAngel()) return false;
            bool allowFa = EnemyHasChaosAngelBanishTarget();
            List<ClientCard> mats = FindChaosAngelMaterials(allowFa);
            if (mats == null || mats.Count == 0) return false;
            AI.SelectMaterials(mats);
            string matStr = "";
            foreach (ClientCard mat in mats)
                matStr += " " + CardStr(mat);
            ElfnoteLog("Chaos Angel materials" + matStr + " allowFa=" + allowFa);
            return true;
        }

        /// <summary>
        /// Black Rose ① is a going-second reset. Do not blow up Crystal Wing / Omega / FA
        /// for a face-up pendulum scale (e.g. enemy White P).
        /// Own terminals stay unless the opponent has a live floodgate or invincible monster
        /// worth the reset. Enemy Red Field is only an attack restriction (it is listed as
        /// Floodgate); disabled floodgates/invincibles also do not justify wiping.
        /// A floodgate monster is only a hard threat if we have no face-up monster with
        /// higher ATK (run over it instead of wiping). Floodgate S/T still count.
        /// Hard threats (live floodgate / invincible / dangerous) wipe even with leftover
        /// own cards. Extra bodies and non-floodgate face-up S/T are soft: same leftover
        /// bar as enemy field count >= 2. forSummon applies leftover; ① does not.
        /// </summary>
        public bool ShouldBlackRoseWipe(bool forSummon = false)
        {
            if (Duel.Turn == 1) return false;

            List<ClientCard> problems = GetProblematicEnemyCardList(false, false, 0);
            bool enemyFloodgate = false;
            bool enemyInvincible = false;
            bool enemyDangerous = false;
            foreach (ClientCard card in problems)
            {
                if (card == null || card.IsDisabled()) continue;
                if (card.IsCode(CardId.RedField)) continue;
                if (card.IsFloodgate())
                {
                    if (!card.IsMonster()
                        || !Bot.GetMonsters().Any(m => m != null && m.IsFaceup() && m.Attack > card.Attack))
                        enemyFloodgate = true;
                }
                if (card.IsMonsterInvincible()) enemyInvincible = true;
                if (card.IsMonsterDangerous()) enemyDangerous = true;
            }

            bool ownTerminal = false;
            foreach (ClientCard monster in Bot.GetMonsters())
            {
                if (monster == null || !monster.IsFaceup() || monster.IsCode(CardId.BlackRoseDragon))
                    continue;
                if (monster.IsCode(CardId.CrystalWing, CardId.AccelSynchroStardust, CardId.PSYFramelordOmega,
                    CardId.FormulaAthleteLightning, CardId.BaronneDeFleur, CardId.SuperLibrarian))
                {
                    ownTerminal = true;
                    break;
                }
            }

            if (ownTerminal && !enemyFloodgate && !enemyInvincible)
                return false;
            bool hardThreat = enemyFloodgate || enemyInvincible || enemyDangerous;
            if (hardThreat)
                return true;
            if (problems.Count == 0 && Enemy.GetFieldCount() < 2)
                return false;
            if (!forSummon)
                return true;

            List<ClientCard> mats = FindSynchroMaterials(7, false);
            int own = Bot.GetFieldCount();
            int enemy = Enemy.GetFieldCount();
            int leftover = mats != null ? own - mats.Count : own;
            if (mats == null || leftover >= 2 || own > enemy)
            {
                ElfnoteLog("skip Black Rose soft/count: leftover=" + leftover
                    + " own=" + own + " enemy=" + enemy + " mats=" + (mats != null ? mats.Count : 0)
                    + " problems=" + problems.Count);
                return false;
            }
            return true;
        }

        public bool BlackRoseSpSummon()
        {
            if (Duel.Turn == 1) return false;
            if (ShouldSkipExtraSynchroForFuwalosCompromise()) return false;
            if (!ShouldBlackRoseWipe(true))
            {
                ElfnoteLog("skip Black Rose: no board worth wiping");
                return false;
            }
            if (CheckShouldNoMoreSpSummon(CardLocation.Extra) && GetProblematicEnemyCardList().Count == 0) return false;
            if (Duel.Player == 0 && Duel.Turn > 1)
            {
                if (SelectSynchroMaterials(7, false))
                    return true;
            }
            return false;
        }

        public bool AncientFishSpSummon()
        {
            if (ShouldSkipExtraSynchroForFuwalosCompromise()) return false;
            if (CheckShouldNoMoreSpSummon(CardLocation.Extra)) return false;
            if (ShouldDeferSynchroForHelldiveBomber())
            {
                ElfnoteLog("skip Ancient Fish: Helldive Bomber first");
                return false;
            }
            if (CheckThousandSpearGoingFirstLine() || CheckThousandSpearGoingSecondLine())
                return false;
            if (SelectSynchroMaterials(9, true))
                return true;
            return false;
        }

        public bool ThousandSpearSpSummon()
        {
            if (ShouldSkipExtraSynchroForFuwalosCompromise()) return false;
            if (CheckShouldNoMoreSpSummon(CardLocation.Extra)) return false;
            if (ShouldDeferSynchroForHelldiveBomber())
            {
                ElfnoteLog("skip Thousand Spear: Helldive Bomber first");
                return false;
            }
            if (!(CheckThousandSpearGoingFirstLine() || CheckThousandSpearGoingSecondLine()))
                return false;
            if (SelectSynchroMaterials(9, true))
                return true;
            return false;
        }

        public bool AssaultBlackwingSpSummon()
        {
            if (Duel.Turn == 1) return false;
            if (ShouldSkipExtraSynchroForFuwalosCompromise()) return false;
            if (ShouldDeferSynchroForHelldiveBomber())
            {
                ElfnoteLog("skip Assault Blackwing: Helldive Bomber first");
                return false;
            }
            if (CheckShouldNoMoreSpSummon(CardLocation.Extra) && Enemy.LifePoints > 4000) return false;
            int atk = 0;
            foreach (ClientCard card in Bot.GetMonsters())
            {
                if (card != null && card.HasType(CardType.Synchro))
                    atk += card.Attack;
            }
            if (atk + 3000 < Enemy.LifePoints && Enemy.GetMonsterCount() == 0) return false;
            if (SelectSynchroMaterials(9, true))
                return true;
            return false;
        }

        public bool HelldiveBomberSpSummon()
        {
            if (Duel.Turn == 1) return false;
            if (ShouldSkipExtraSynchroForFuwalosCompromise()) return false;
            if (CheckShouldNoMoreSpSummon(CardLocation.Extra)) return false;
            if (!HelldiveBomberDestroyEffectCanApply())
            {
                ElfnoteLog("skip Helldive Bomber: ① used or cannot apply");
                return false;
            }
            if (Bot.GetMonsters().Any(c => c != null && c.IsFaceup() && c.IsCode(CardId.HelldiveBomber) && !c.IsDisabled()))
            {
                ElfnoteLog("skip Helldive Bomber: ① already on field");
                return false;
            }
            List<ClientCard> mats = FindSynchroMaterials(7, true);
            if (mats == null)
            {
                ElfnoteLog("skip Helldive Bomber: no 7 mats");
                return false;
            }
            int dmg = EstimateBomberDamage(mats, true);
            if (dmg <= Enemy.LifePoints)
            {
                ElfnoteLog("skip Helldive Bomber: damage=" + dmg + " enemyLP=" + Enemy.LifePoints);
                return false;
            }
            AI.SelectMaterials(mats);
            ElfnoteLog("Helldive Bomber SS damage=" + dmg + " enemyLP=" + Enemy.LifePoints);
            return true;
        }

        public bool ThousandSpearActivate()
        {
            if (CheckWhetherNegated(true)) return false;
            bool hasDanger = GetDangerousCardinEnemyGrave(false).Count > 0 || GetProblematicEnemyCardList(false, false, 0).Count > 0;
            if (ActivateDescription == Util.GetStringId(CardId.ThousandSpearDragon, 0) || (ActivateDescription == -1 && Duel.LastChainPlayer != 1))
            {
                if (!EnemyHasThousandSpearBanishWorth()) return false;
                int removable = Enemy.GetMonsterCount() + Enemy.GetSpellCount() + Enemy.Graveyard.Count;
                if (removable < 2) return false;
                activatedCardIdList.Add(CardId.ThousandSpearDragon);
                return true;
            }
            if (ActivateDescription == Util.GetStringId(CardId.ThousandSpearDragon, 1))
            {
                if (!hasDanger) return false;
                if (Bot.Hand.Count(c => c != null && !c.IsCode(_CardId.AshBlossom, _CardId.MaxxC, _CardId.LockBird, _CardId.EffectVeiler)) == 0)
                    return false;
                activatedCardIdList.Add(CardId.ThousandSpearDragon + 1);
                return true;
            }
            if (ActivateDescription == Util.GetStringId(CardId.ThousandSpearDragon, 2) || Card.Location == CardLocation.Grave)
            {
                activatedCardIdList.Add(CardId.ThousandSpearDragon + 2);
                return true;
            }
            return false;
        }

        public bool HelldiveBomberActivate()
        {
            if (CheckWhetherNegated(true)) return false;
            if (Duel.Phase != DuelPhase.Main2) return false;
            if (activatedCardIdList.Contains(CardId.HelldiveBomber)) return false;
            if (EstimateBomberDamage() <= 0) return false;
            activatedCardIdList.Add(CardId.HelldiveBomber);
            return true;
        }

        public bool BlackRoseActivate()
        {
            if (CheckWhetherNegated(true)) return false;
            int destroyDesc = Util.GetStringId(CardId.BlackRoseDragon, 0);
            if (ActivateDescription != destroyDesc && ActivateDescription != -1)
                return false;
            if (!ShouldBlackRoseWipe())
            {
                ElfnoteLog("skip Black Rose ①: would destroy own terminal board");
                return false;
            }
            return true;
        }

        public bool AncientFishActivate()
        {
            if (CheckWhetherNegated(true)) return false;
            int drawDesc = Util.GetStringId(CardId.AncientFishDragon, 0);
            int destroyDesc = Util.GetStringId(CardId.AncientFishDragon, 1);
            if (ActivateDescription == drawDesc
                || (ActivateDescription == -1 && ActivateDescription != destroyDesc && !activatedCardIdList.Contains(CardId.AncientFishDragon)))
            {
                activatedCardIdList.Add(CardId.AncientFishDragon);
                return true;
            }
            List<ClientCard> destroyTargets = GetAncientFishDestroyTargets();
            if (destroyTargets.Count == 0)
            {
                ElfnoteLog("skip Ancient Fish ②: no destroy target besides pending negate/disabled");
                return false;
            }
            return true;
        }

        public bool SuperLibrarianActivate()
        {
            return true;
        }

        public bool SpellSetCheck()
        {
            // Continuous spells (Green Field, Innocent Art, Jail God Gate) must be activated face-up.
            // Red Field is a continuous trap: Set it unless Yellow can still place it face-up this turn.
            if (Card.IsCode(CardId.GreenField, CardId.InnocentArt, CardId.JailGodGate))
                return false;
            if (Card.IsCode(CardId.RedField))
            {
                if (Bot.HasInSpellZone(CardId.RedField)) return false;
                // Yellow places Red Field face-up. Set only when Yellow can no longer place it this turn.
                // Turn 1 has no Main2: if Yellow cannot SS (center occupied), Set in Main1 before End.
                if (!activatedCardIdList.Contains(CardId.ElvenotesYellow))
                {
                    bool yellowOnField = Bot.GetMonsters().Any(c => c != null && c.IsCode(CardId.ElvenotesYellow) && c.IsFaceup());
                    if (yellowOnField)
                    {
                        ElfnoteLog("skip set Red Field: Yellow on field can still place");
                        return false;
                    }
                    if (Bot.HasInHand(CardId.ElvenotesYellow) && IsCenterEmpty())
                    {
                        ElfnoteLog("skip set Red Field: Yellow in hand can SS to center");
                        return false;
                    }
                }
                // Main 2 after Battle. No attacking monster → Main 1 goes to End, set now.
                if (Duel.Phase == DuelPhase.Main1 && Bot.HasAttackingMonster() && Duel.Turn > 1)
                    return false;
                ElfnoteLog("set Red Field");
                return true;
            }
            if (Duel.Phase == DuelPhase.Main1 && Bot.HasAttackingMonster() && Duel.Turn > 1)
                return false;
            if (Card.IsTrap() || Card.HasType(CardType.QuickPlay))
            {
                ElfnoteLog("set " + CardStr(Card));
                return true;
            }
            return false;
        }

        #endregion
    }
}
