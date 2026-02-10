using System;
using System.Collections.Generic;
using System.Linq;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using YGOSharp.Network.Enums;
using YGOSharp.OCGWrapper;
using YGOSharp.OCGWrapper.Enums;

namespace WindBot.Game.AI.Decks
{
    [Deck("Maliss", "AI_Maliss")]
    public class MalissExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int DominusImpulse = 40366667;
            public const int TERRAFORMING = 73628505;
            //public const int AllureOfDarkness = 1475311;
            public const int GoldSarcophagus = 75500286;

            // Cyberse / utility
            public const int BackupIgnister = 30118811;
            public const int WizardIgnister = 3723262;

            // Main Maliss pieces
            public const int MalissP_Dormouse = 32061192;              // Maliss <P> Dormouse
            public const int MalissP_WhiteRabbit = 69272449;           // Maliss <P> White Rabbit
            public const int MalissP_ChessyCat = 96676583;             // Maliss <P> Chessy Cat
            public const int MalissP_MarchHare = 20938824;             // Maliss <P> March Hare
            public const int MalissC_GWC06 = 20726052;                 // Maliss <C> GWC-06
            //public const int MalissC_TB11 = 57111661;                  // Maliss <C> TB-11
            public const int MalissC_MTP07 = 94722358;                 // Maliss <C> MTP-07
            public const int MalissQ_RedRansom = 68059897;             // Maliss <Q> Red Ransom
            public const int MalissQ_WhiteBinder = 95454996;           // Maliss <Q> White Binder
            public const int MalissQ_HeartsCrypter = 21848500;         // Maliss <Q> Hearts Crypter
            public const int MalissInTheMirror = 93453053;             // Maliss in the Mirror (Spell)
            public const int MalissInUnderground = 68337209;           // Maliss in Underground


            // === Extra Deck ===
            public const int Linguriboh = 24842059;
            public const int LinkDecoder = 30342076;
            public const int SP_LITTLE_KNIGHT = 29301450;
            public const int SALAMANGREAT_ALMIRAJ = 60303245;
            public const int SplashMage = 59859086;                    // Splash Mage
            public const int CyberseWicckid = 52698008;                // Cyberse Wicckid
            public const int TranscodeTalker = 46947713;               // Transcode Talker
            public const int AlliedCodeTalkerIgnister = 39138610;      // Allied Code Talker @Ignister
            public const int FirewallDragon = 5043010;                 // Firewall Dragon
            public const int LinkSpider = 98978921;                     // Link Spider
            public const int HaggardLizardose = 9763474;                 // Haggard Lizardose
            public const int AccesscodeTalker = 86066372;              // Accesscode Talker
            public const int Apollousa = 4280258;

            // === Handtraps / Others (blacklist/targets etc.) ===
            public const int Lancea = 34267821;
            public const int Fuwalos = 42141493;
            public const int NaturalExterio = 99916754;
            public const int NaturalBeast = 33198837;
            public const int ImperialOrder = 61740673;
            public const int SwordsmanLV7 = 37267041;
            public const int RoyalDecree = 51452091;
            public const int Number41BagooskatheTerriblyTiredTapir = 90590303;
            public const int InspectorBoarder = 15397015;
            public const int SkillDrain = 82732705;
            public const int DivineArsenalAAZEUS_SkyThunder = 90448279;
            public const int DimensionShifter = 91800273;
            public const int MacroCosmos = 30241314;
            public const int DimensionalFissure = 81674782;
            public const int BanisheroftheRadiance = 94853057;
            public const int BanisheroftheLight = 61528025;
            public const int KashtiraAriseHeart = 48626373;
            public const int GhostMournerMoonlitChill = 52038441;
            public const int NibiruThePrimalBeing = 27204311;
        }
        const int SetcodeMaliss = 0x1bf;
        const int SetcodeTimeLord = 0x4a;
        const int SetcodePhantom = 0xdb;
        const int SetcodeOrcust = 0x11b;
        const int SetcodeHorus = 0x19d;
        const int SetcodeDarkWorld = 0x6;
        const int SetcodeSkyStriker = 0x115;

        Dictionary<int, List<int>> DeckCountTable = new Dictionary<int, List<int>>{
            {3, new List<int> { CardId.MalissP_ChessyCat, CardId.MalissP_MarchHare,CardId.MalissP_WhiteRabbit,CardId.MalissInUnderground,
                                CardId.BackupIgnister, CardId.MalissP_Dormouse,
                                 _CardId.AshBlossom,_CardId.InfiniteImpermanence,CardId.DominusImpulse } },
            {2, new List<int> { _CardId.MaxxC, _CardId.CalledByTheGrave}},
            {1, new List<int> { CardId.GoldSarcophagus, CardId.TERRAFORMING,
                                CardId.MalissC_GWC06, CardId.Lancea, CardId.MalissC_MTP07,
                                _CardId.CrossoutDesignator, CardId.MalissInTheMirror, CardId.WizardIgnister,
                                CardId.NibiruThePrimalBeing }}
        };

        List<int> notToNegateIdList = new List<int> { 58699500, 20343502, 19403423 };
        List<int> notToDestroySpellTrap = new List<int> { 50005218, 6767771 };
        List<int> targetNegateIdList = new List<int> {
            _CardId.EffectVeiler, _CardId.InfiniteImpermanence, CardId.GhostMournerMoonlitChill, _CardId.BreakthroughSkill, 74003290, 67037924,
            9753964, 66192538, 23204029, 73445448, 35103106, 30286474, 45002991, 5795980, 38511382, 53742162, 30430448
        };

        public MalissExecutor(GameAI ai, Duel duel) : base(ai, duel)
        {
            // Must Set First
            AddExecutor(ExecutorType.SpellSet, CardId.MalissC_GWC06, SpellSetCheck);
            AddExecutor(ExecutorType.SpellSet, CardId.MalissC_MTP07, SpellSetCheck);


            AddExecutor(ExecutorType.Activate, CardId.AccesscodeTalker, Accesscode_OnSummon_AtkUp);
            AddExecutor(ExecutorType.Activate, CardId.AccesscodeTalker, Accesscode_Destroy_Ignition);

            // ===== Generic counters =====
            AddExecutor(ExecutorType.Activate, _CardId.MaxxC, MaxxCActivate);
            AddExecutor(ExecutorType.Activate, _CardId.AshBlossom, AshBlossomActivate);
            AddExecutor(ExecutorType.Activate, _CardId.CalledByTheGrave, CalledbytheGraveActivate);
            AddExecutor(ExecutorType.Activate, _CardId.CrossoutDesignator, CrossoutDesignatorActivate);
            AddExecutor(ExecutorType.Activate, _CardId.InfiniteImpermanence, InfiniteImpermanenceActivate);
            AddExecutor(ExecutorType.Activate, CardId.Apollousa, DontSelfNG);
            AddExecutor(ExecutorType.Activate, CardId.DominusImpulse, DontSelfNG);
            AddExecutor(ExecutorType.Activate, CardId.AlliedCodeTalkerIgnister, Allied_NegateBanish);
            AddExecutor(ExecutorType.Activate, CardId.FirewallDragon, FirewallBounce_OnOppSummon);
            AddExecutor(ExecutorType.Activate, CardId.MalissC_MTP07, MTP07_OppTurn_RemoveEnemyOnly);
            AddExecutor(ExecutorType.Activate, CardId.MalissQ_HeartsCrypter, HC_Quick_ReturnBanished_AndBanishField);
            AddExecutor(ExecutorType.Activate, CardId.MalissC_GWC06, GWC06_OppTurn_ReviveWB_HC);
            AddExecutor(ExecutorType.Activate, CardId.SP_LITTLE_KNIGHT, ActLittleKnight);


            AddExecutor(ExecutorType.Activate, CardId.MalissQ_RedRansom, RR_SS_FromBanished);
            AddExecutor(ExecutorType.Activate, CardId.MalissQ_HeartsCrypter, HC_OnBanished_SpecialSummon);
            AddExecutor(ExecutorType.Activate, CardId.MalissInTheMirror, Mirror_Banish);

            // Plan#1
            // --- Start with Dormouse ---
            AddExecutor(ExecutorType.Summon, CardId.MalissP_Dormouse, Step1_Dormouse_NormalSummon);
            AddExecutor(ExecutorType.Summon, CardId.MalissP_WhiteRabbit, Step1_WhiteRabbit_NormalSummon);
            AddExecutor(ExecutorType.Summon, CardId.MalissP_ChessyCat, TwoCC_NormalSummon);
            AddExecutor(ExecutorType.Summon, CardId.MalissP_ChessyCat, Emergency_NormalCat);
            AddExecutor(ExecutorType.Summon, CardId.MalissP_MarchHare, NSMH);
            AddExecutor(ExecutorType.Summon, CardId.BackupIgnister, NSBackup_L);
            AddExecutor(ExecutorType.Summon, CardId.BackupIgnister, NSBackup);
            AddExecutor(ExecutorType.Activate, CardId.MalissP_Dormouse, Dormouse_SS_FromBanished);
            AddExecutor(ExecutorType.Activate, CardId.MalissP_Dormouse, Dormouse_ForMH);
            AddExecutor(ExecutorType.Activate, CardId.MalissP_Dormouse, Dormouse_Banish_Anytime);
            AddExecutor(ExecutorType.Activate, CardId.MalissP_WhiteRabbit, Step1_WhiteRabbit_SS_FromBanished);
            AddExecutor(ExecutorType.Activate, CardId.MalissP_WhiteRabbit, Step1_WhiteRabbit_SetTrapOnSummon);
            AddExecutor(ExecutorType.Activate, CardId.MalissP_MarchHare, Step1_MH_FromHand);
            AddExecutor(ExecutorType.Activate, CardId.MalissP_MarchHare, returnFromBanish);
            AddExecutor(ExecutorType.SpSummon, CardId.Linguriboh, LinguribohMHLine);
            AddExecutor(ExecutorType.SpSummon, CardId.LinkDecoder, Step1_SSLinkDecoder);
            AddExecutor(ExecutorType.SpSummon, CardId.MalissQ_RedRansom, Step2N_LinkSummon_RedRansom);
            AddExecutor(ExecutorType.SpSummon, CardId.MalissQ_RedRansom, Step2_LinkSummon_RedRansom);
            AddExecutor(ExecutorType.Activate, CardId.MalissQ_RedRansom, Step2_RedRansom_Search);
            AddExecutor(ExecutorType.Activate, CardId.MalissP_ChessyCat, AnyDraw);
            AddExecutor(ExecutorType.SpSummon, CardId.Apollousa, Link_Apo);
            AddExecutor(ExecutorType.SpSummon, CardId.CyberseWicckid, Step2N_RRtoWicckid);
            AddExecutor(ExecutorType.SpSummon, CardId.LinkDecoder, Step_SummonLinkDecoderToWicckid);
            AddExecutor(ExecutorType.Activate, CardId.CyberseWicckid, Wicckid_SearchTuner);
            AddExecutor(ExecutorType.SpSummon, CardId.SplashMage, Step_SplashToWB);
            AddExecutor(ExecutorType.Activate, CardId.SplashMage, Step2N_SplashMage_ReviveP);
            AddExecutor(ExecutorType.Activate, CardId.BackupIgnister, Flow3_BackupIgnister_AfterMakeIt3);
            AddExecutor(ExecutorType.Activate, CardId.BackupIgnister, OneBody_Backup_SearchWizard);
            AddExecutor(ExecutorType.SpSummon, CardId.MalissQ_WhiteBinder, Step2N_LinkSummon_WB);
            AddExecutor(ExecutorType.Activate, CardId.MalissQ_WhiteBinder, WB_OnSummon_BanishGY);
            AddExecutor(ExecutorType.Activate, CardId.MalissC_GWC06, GWC06_MyTurn_Extend);
            AddExecutor(ExecutorType.Activate, CardId.MalissQ_WhiteBinder, WB_SetMalissTrap);

            // === STEP2: 2 bodies -> Splash Mage -> revive P -> Red Ransom -> search ===
            AddExecutor(ExecutorType.Activate, CardId.WizardIgnister, Step2_Fallback_Wizard_AfterSplashNegated);
            AddExecutor(ExecutorType.Activate, CardId.BackupIgnister, Step2_Fallback_Backup_AfterSplashNegated);

            AddExecutor(ExecutorType.Activate, CardId.WizardIgnister, Flow3_WizardIgnister_AfterMakeIt3);
            AddExecutor(ExecutorType.Activate, CardId.MalissQ_WhiteBinder, WB_OnBanished_SelfSS);
            AddExecutor(ExecutorType.Activate, CardId.MalissP_MarchHare, ssFromHandMH);
            AddExecutor(ExecutorType.SpSummon, CardId.MalissQ_HeartsCrypter, Step_LinkSummon_HeartsCrypter);
            AddExecutor(ExecutorType.SpSummon, CardId.AlliedCodeTalkerIgnister, Flow3_Link_Allied);
            AddExecutor(ExecutorType.Activate, CardId.AlliedCodeTalkerIgnister, Allied_OnSummonTrigger);

            // Plan#2 White Rabbit ---
            AddExecutor(ExecutorType.Activate, CardId.MalissC_MTP07, MTP07_ForMH);
            AddExecutor(ExecutorType.SpSummon, CardId.FirewallDragon, Flow3_Link_Firewall);
            AddExecutor(ExecutorType.SpSummon, CardId.MalissQ_WhiteBinder, Step_WicckidPlusOneToWB);
            AddExecutor(ExecutorType.Activate, CardId.MalissP_ChessyCat, ChessyCat_SS_FromBanished);


            AddExecutor(ExecutorType.SpSummon, CardId.CyberseWicckid, Step_RRtoWicckid);
            AddExecutor(ExecutorType.Activate, CardId.MalissInUnderground, Flow3_UnderGround_Available_SSAnyPawn);
            AddExecutor(ExecutorType.Activate, CardId.LinkDecoder, LinkDecoder_ReviveFromGY);

            // Fallback Lancea
            AddExecutor(ExecutorType.SpSummon, CardId.TranscodeTalker, SummonTranscode);
            AddExecutor(ExecutorType.Activate, CardId.TranscodeTalker, Transcode_ReviveLink3OrLower);

            // Emergency Start
            AddExecutor(ExecutorType.Activate, CardId.TERRAFORMING, Terra_GrabUnderground);
            AddExecutor(ExecutorType.Activate, CardId.GoldSarcophagus, GoldSarc_StartPiece);
            AddExecutor(ExecutorType.Activate, CardId.MalissInUnderground, Underground_ActivateStarter);

            AddExecutor(ExecutorType.SpSummon, CardId.SplashMage, Step_SplashToRR);
            AddExecutor(ExecutorType.Activate, CardId.SplashMage, Step2_SplashMage_ReviveP);

            // Fallback: Try to Start
            AddExecutor(ExecutorType.SpSummon, CardId.LinkSpider);
            AddExecutor(ExecutorType.Summon, _CardId.AshBlossom, Emergency_NS);
            AddExecutor(ExecutorType.Summon, _CardId.MaxxC, Emergency_NS);
            AddExecutor(ExecutorType.SpSummon, CardId.Linguriboh, OneBody_Link1_Linguriboh);
            AddExecutor(ExecutorType.SpSummon, CardId.SALAMANGREAT_ALMIRAJ, OneBody_Link1_Almiraj);
            AddExecutor(ExecutorType.Activate, CardId.BackupIgnister, OneBody_Backup_SS);
            AddExecutor(ExecutorType.Activate, CardId.BackupIgnister, OneBody_Backup_SearchWizard);
            AddExecutor(ExecutorType.Activate, CardId.WizardIgnister, OneBody_Wizard_SS);


            //Finisher
            AddExecutor(ExecutorType.SpSummon, CardId.AccesscodeTalker, Flow3_Link_Accesscode);

            // Turn3 or More
            AddExecutor(ExecutorType.SpSummon, CardId.Linguriboh, T3Allow);
            AddExecutor(ExecutorType.SpSummon, CardId.SALAMANGREAT_ALMIRAJ, T3Allow);
            AddExecutor(ExecutorType.SpSummon, CardId.TranscodeTalker, EmerTranscode);
            AddExecutor(ExecutorType.SpSummon, CardId.AlliedCodeTalkerIgnister, Emer_Allied);
            AddExecutor(ExecutorType.SpSummon, CardId.AlliedCodeTalkerIgnister, Emer_Allied2);

            // >>> Fallback: S:P Little Knight
            AddExecutor(ExecutorType.SpSummon, CardId.SP_LITTLE_KNIGHT, SummonLittleKnightFast);
            AddExecutor(ExecutorType.SpSummon, CardId.SP_LITTLE_KNIGHT, SPEmer);

            AddExecutor(ExecutorType.SpellSet, SpellSetCheck);
            AddExecutor(ExecutorType.Repos, MonsterRepos);

        }
        // Maliss Flags
        bool usedNormalSummon = false;
        bool ssDormouse = false;
        bool ssWhiteRabbit = false;
        bool ssChessyCat = false;
        bool ssMarchHare = false;
        bool ActiveMarchHare = false;
        bool ssRRThisTurn = false;
        bool ssWBThisTurn = false;
        bool ssHCThisTurn = false;
        bool enemyActivateLancea = false;
        bool enemyActivateFuwalos = false;
        bool ActiveUnderground = false;
        bool blockWicckid = false;
        bool mtp07SetThisTurn = false;
        bool gwc06SetThisTurn = false;
        bool splashNegatedThisTurn = false;
        bool Allied_End = false;
        bool fullBoard1 = false;
        bool goldstart = false;
        bool undergroundstart = false;
        bool nsplan = false;
        bool nsBackupplan = false;
        bool NSDorMouse = false;
        bool nsLanceaplan = false;

        int myTurnCount = 0;
        bool avoidLinkedZones = false;
        bool wantLinkedToWicckid = false;
        private int? _wicckidEmzIndex = null;
        private int _transcodeZoneMask = 0;

        const int MZ0 = 1 << 0;
        const int MZ1 = 1 << 1;
        const int MZ2 = 1 << 2;
        const int MZ3 = 1 << 3;
        const int MZ4 = 1 << 4;
        const int EMZ_L = (1 << 5);
        const int EMZ_R = (1 << 6);
        const int EMZ_ALL = EMZ_L | EMZ_R;
        int _wicckidEmzBit = 0;
        int _forceTranscodeBit = 0;

        // Step Flag
        bool step1Done = false;
        bool step2Done = false;
        int lastRevivedIdBySplash = 0;
        bool coreSetupComplete = false;
        bool madeIt3 = false;
        bool resultSuccessFlag = false;
        private bool _didSplashToRR;
        private bool _didRRtoWicckid;
        private bool _didSummonToWicckidArrow;
        private bool _didWBFromWicckid;
        private bool _finishPlanDecided;
        private bool _preferWicckidArrows;
        private bool _rrSelfSSPlacing = false;
        private enum FinishPlan { FW_HC_Allied, HC_Allied, AlliedOnly }
        private FinishPlan _finishPlan;
        static bool IsEmzSeq(int seq) => seq >= 5;
        static int BitOfSeq(int seq) => (1 << seq);
        static int LowestBit(int m) => m & -m;

        private bool _oppJustActivatedPersistentSpell;
        private bool _oppJustSummoned;
        private bool _oppJustSet;
        private int _prefWindowTTL;

        private int _enemyMonsterCountSnap;
        private int _enemyFacedownSTSnap;

        //==================== Default code ====================
        #region Default Code Start Here
        private int _totalAttack;
        private int _totalBotAttack;
        bool enemyActivateMaxxC = false;
        bool enemyActivateLockBird = false;
        int dimensionShifterCount = 0;
        bool enemyActivateInfiniteImpermanenceFromHand = false;
        List<int> infiniteImpermanenceList = new List<int>();
        List<ClientCard> currentNegateCardList = new List<ClientCard>();
        List<ClientCard> currentDestroyCardList = new List<ClientCard>();
        List<ClientCard> sendToGYThisTurn = new List<ClientCard>();
        List<int> activatedCardIdList = new List<int>();
        List<ClientCard> enemyPlaceThisTurn = new List<ClientCard>();
        List<ClientCard> escapeTargetList = new List<ClientCard>();
        List<ClientCard> summonThisTurn = new List<ClientCard>();


        public List<T> ShuffleList<T>(List<T> list)
        {
            List<T> result = list;
            int n = result.Count;
            while (n-- > 1)
            {
                int index = Program.Rand.Next(result.Count);
                int nextIndex = (index + Program.Rand.Next(result.Count - 1)) % result.Count;
                T tempCard = result[index];
                result[index] = result[nextIndex];
                result[nextIndex] = tempCard;
            }
            return result;
        }

        public override bool OnSelectHand() { return true; }

        public List<ClientCard> ShuffleCardList(List<ClientCard> list)
        {
            List<ClientCard> result = list;
            int n = result.Count;
            while (n-- > 1)
            {
                int index = Program.Rand.Next(n + 1);
                ClientCard temp = result[index];
                result[index] = result[n];
                result[n] = temp;
            }
            return result;
        }

        public int CheckRemainInDeck(int id)
        {
            for (int count = 1; count < 4; ++count)
            {
                if (DeckCountTable[count].Contains(id))
                {
                    return Bot.GetRemainingCount(id, count);
                }
            }
            return 0;
        }
        public bool MonsterRepos()
        {
            int selfAttack = Card.Attack + 1;

            if (selfAttack <= 1)
                return !Card.IsDefense();

            int bestAttack = 0;
            foreach (ClientCard card in Bot.GetMonsters())
            {
                int attack = card.Attack;
                if (attack >= bestAttack)
                {
                    bestAttack = attack;
                }
            }

            bool enemyBetter = Util.IsAllEnemyBetterThanValue(bestAttack, true);

            if (Card.IsAttack() && enemyBetter)
                return true;
            if (Card.IsDefense() && !enemyBetter)
                return true;
            return false;
        }

        public bool CheckAtAdvantage()
        {
            if (GetProblematicEnemyMonster() == null && Bot.GetMonsters().Any(card => card.IsFaceup()))
            {
                return true;
            }
            return false;
        }

        public bool CheckInDanger()
        {
            if (Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2)
            {
                int totalAtk = 0;
                foreach (ClientCard m in Enemy.GetMonsters())
                {
                    if (m.IsAttack() && !m.Attacked) totalAtk += m.Attack;
                }
                if (totalAtk >= Bot.LifePoints) return true;
            }
            return false;
        }
        private int GetMyLinkedMMZMask()
        {
            int mask = 0;
            foreach (var m in Bot.GetMonsters())
            {
                if (m == null || !m.IsFaceup() || !m.HasType(CardType.Link)) continue;
                mask |= m.GetLinkedZones();
            }
            mask &= 0x1F;
            return mask;
        }
        private bool IsPawnId(int id)
        {
            return id == CardId.MalissP_Dormouse
                || id == CardId.MalissP_WhiteRabbit
                || id == CardId.MalissP_ChessyCat
                || id == CardId.MalissP_MarchHare;
        }
        private int GetQueenLinkedMMZMask()
        {
            int mask = 0;
            foreach (var m in Bot.GetMonsters())
            {
                if (m == null || !m.IsFaceup()) continue;

                if (m.IsCode(CardId.MalissQ_RedRansom)
                 || m.IsCode(CardId.MalissQ_WhiteBinder)
                 || m.IsCode(CardId.MalissQ_HeartsCrypter))
                {
                    mask |= m.GetLinkedZones();
                }
            }
            mask &= 0x1F;
            return mask;
        }

        private int LinkValOf(ClientCard c)
        {
            return c.HasType(CardType.Link) ? Math.Max(1, c.LinkCount) : 1;
        }

        private bool IsOneVal(ClientCard c)
        {
            return !c.HasType(CardType.Link) || Math.Max(1, c.LinkCount) == 1;
        }

        private bool IsMaliss(ClientCard c)
        {
            return c.HasSetcode(0x1bf);
        }

        private int ScoreForBanishedMaliss(ClientCard c)
        {
            if (c.IsCode(CardId.MalissInTheMirror)) return 100;
            if (c.IsCode(CardId.MalissC_MTP07)) return 95;
            if (c.IsCode(CardId.MalissC_GWC06)) return 90;
            if (c.IsCode(CardId.MalissInUnderground)) return 85;
            if (c.IsCode(CardId.MalissP_MarchHare)) return 80;
            if (c.IsCode(CardId.MalissP_ChessyCat)) return 75;
            if (c.IsCode(CardId.MalissP_WhiteRabbit)) return 70;
            if (c.IsCode(CardId.MalissP_Dormouse)) return 65;
            return 50;
        }

        public override int OnSelectPlace(int cardId, int player, CardLocation location, int available)
        {
            if (player == 0 && location == CardLocation.MonsterZone)
            {
                int MAIN_MASK =
                                (int)Zones.z0 |
                                (int)Zones.z1 |
                                (int)Zones.z2 |
                                (int)Zones.z3 |
                                (int)Zones.z4;

                int emzMask = available & ~MAIN_MASK;
                int mainMask = available & MAIN_MASK;
                if (IsPawnId(cardId))
                {
                    int queenMask = GetQueenLinkedMMZMask();
                    int queenChoices = queenMask & available & MAIN_MASK;

                    if (queenChoices != 0)
                    {
                        int pick = FirstBitFromOrder(
                            queenChoices,
                            new[] { (int)Zones.z2, (int)Zones.z1, (int)Zones.z3, (int)Zones.z0, (int)Zones.z4 }
                        );
                        AI.SelectPlace(pick);
                        return pick;
                    }
                }
                if (cardId == CardId.AlliedCodeTalkerIgnister)
                {
                    var fw = Bot.MonsterZone.GetFirstMatchingCard(m => m != null && m.IsCode(CardId.FirewallDragon));
                    int emzAvail = available & EMZ_ALL;

                    if (fw != null && fw.IsFaceup())
                    {
                        bool firewallCenter = fw.Controller == 0 && fw.Location == CardLocation.MonsterZone && fw.Sequence == 2;

                        int linkedAll = fw.GetLinkedZones();
                        int linkedChoices = linkedAll & available;

                        int linkedEmzChoices = linkedChoices & EMZ_ALL;
                        int pick;

                        if (linkedEmzChoices != 0)
                        {
                            if (firewallCenter && (linkedEmzChoices & EMZ_L) != 0 && (linkedEmzChoices & EMZ_R) != 0)
                            {
                                int leftFree = 0;
                                if ((available & (int)Zones.z0) != 0) leftFree++;
                                if ((available & (int)Zones.z1) != 0) leftFree++;

                                int rightFree = 0;
                                if ((available & (int)Zones.z3) != 0) rightFree++;
                                if ((available & (int)Zones.z4) != 0) rightFree++;

                                if (leftFree > rightFree)
                                    pick = EMZ_L;
                                else if (rightFree > leftFree)
                                    pick = EMZ_R;
                                else
                                    pick = FirstBitFromOrder(linkedEmzChoices, new[] { EMZ_L, EMZ_R });
                            }
                            else
                            {
                                pick = FirstBitFromOrder(linkedEmzChoices, new[] { EMZ_L, EMZ_R });
                            }
                            AI.SelectPlace(pick);
                            return pick;
                        }
                        if (emzAvail != 0)
                        {
                            if (firewallCenter && (emzAvail & EMZ_L) != 0 && (emzAvail & EMZ_R) != 0)
                            {
                                int leftFree = 0;
                                if ((available & (int)Zones.z0) != 0) leftFree++;
                                if ((available & (int)Zones.z1) != 0) leftFree++;

                                int rightFree = 0;
                                if ((available & (int)Zones.z3) != 0) rightFree++;
                                if ((available & (int)Zones.z4) != 0) rightFree++;

                                if (leftFree > rightFree)
                                    pick = EMZ_L;
                                else if (rightFree > leftFree)
                                    pick = EMZ_R;
                                else
                                    pick = FirstBitFromOrder(emzAvail, new[] { EMZ_L, EMZ_R });
                            }
                            else
                            {
                                pick = FirstBitFromOrder(emzAvail, new[] { EMZ_L, EMZ_R });
                            }

                            AI.SelectPlace(pick);
                            return pick;
                        }
                        if (linkedChoices != 0)
                        {
                            pick = FirstBitFromOrder(
                                linkedChoices,
                                new[] { (int)Zones.z2, (int)Zones.z1, (int)Zones.z3, (int)Zones.z0, (int)Zones.z4 }
                            );
                            AI.SelectPlace(pick);
                            return pick;
                        }
                    }
                    int emzOnly = available & EMZ_ALL;
                    if (emzOnly != 0)
                    {
                        int pick = FirstBitFromOrder(emzOnly, new[] { EMZ_L, EMZ_R });
                        AI.SelectPlace(pick);
                        return pick;
                    }
                    return PreferSafeSummonZones(available);
                }
                if (cardId == CardId.CyberseWicckid)
                {
                    int picked = ChooseAndRememberWicckidEmz(available);
                    if (picked != 0) return picked;
                    return 0;
                }
                if (cardId == CardId.TranscodeTalker)
                {
                    int wanted = _forceTranscodeBit != 0 ? _forceTranscodeBit : _wicckidEmzBit;

                    if (wanted != 0 && (available & wanted) != 0)
                        return wanted;

                    int anyEmz = available & EMZ_ALL;
                    if (anyEmz != 0)
                        return (anyEmz & EMZ_L) != 0 ? EMZ_L : EMZ_R;

                    return 0;
                }
                if (cardId == CardId.MalissQ_RedRansom && _rrSelfSSPlacing)
                {
                    int prefer = (int)Zones.z1 | (int)Zones.z3;
                    int wmask = GetLinkedMaskFor(GetWicckid());
                    int choices = (available & prefer) & ~wmask;
                    if (choices != 0)
                    {
                        int pick = FirstBitFromOrder(choices, new[] { (int)Zones.z1, (int)Zones.z3 });
                        AI.SelectPlace(pick);
                        _rrSelfSSPlacing = false;
                        return pick;
                    }
                }
                if (cardId == CardId.LinkDecoder)
                {
                    var trans = Bot.MonsterZone.GetFirstMatchingCard(m => m != null && m.IsCode(CardId.TranscodeTalker));
                    int tmask = GetLinkedMaskFor(trans) & 0x1F;
                    int safe = (available & 0x1F) & ~tmask;
                    if (safe != 0)
                    {
                        int pick = FirstBitFromOrder(safe, new[] { (int)Zones.z2, (int)Zones.z1, (int)Zones.z3, (int)Zones.z0, (int)Zones.z4 });
                        AI.SelectPlace(pick);
                        return pick;
                    }
                }
                if (cardId == CardId.FirewallDragon)
                {
                    var trans = Bot.MonsterZone.GetFirstMatchingCard(m => m != null && m.IsCode(CardId.TranscodeTalker));
                    int underTrans = 0;
                    if (trans != null)
                    {
                        int tmask = GetLinkedMaskFor(trans) & 0x1F;
                        if ((tmask & (int)Zones.z1) != 0) underTrans |= (int)Zones.z1;
                        if ((tmask & (int)Zones.z3) != 0) underTrans |= (int)Zones.z3;
                    }
                    int choices = (available & underTrans);
                    if (choices != 0)
                    {
                        int pick = FirstBitFromOrder(choices, new[] { (int)Zones.z1, (int)Zones.z3 });
                        AI.SelectPlace(pick);
                        return pick;
                    }
                }

                if (cardId == CardId.TranscodeTalker ||
                    cardId == CardId.AccesscodeTalker ||
                    cardId == CardId.AlliedCodeTalkerIgnister ||
                    cardId == CardId.MalissQ_WhiteBinder ||
                    cardId == CardId.MalissQ_HeartsCrypter)
                {
                    return PreferSafeSummonZones(available);
                }
                int linked = (GetMyLinkedMMZMask() & available) & 0x1F;
                int unlinked = (available & 0x1F) & ~linked;

                int choose;
                if (avoidLinkedZones && unlinked != 0)
                    choose = LowestBit(unlinked);     // Not linked zone
                else if (linked != 0)
                    choose = LowestBit(linked);       // Default
                else
                    choose = LowestBit(available & 0x1F);

                AI.SelectPlace(choose);
                return choose;
            }
            SelectSTPlace(Card, true);
            return base.OnSelectPlace(cardId, player, location, available);
        }

        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            YGOSharp.OCGWrapper.NamedCard cardData = YGOSharp.OCGWrapper.NamedCard.Get(cardId);
            if (cardData != null)
            {
                if (Duel.Turn == 1 || Duel.Phase >= DuelPhase.Main2)
                {
                    bool turnDefense = false;
                    if (cardData.Attack <= cardData.Defense)
                    {
                        turnDefense = true;
                    }
                    if (turnDefense)
                    {
                        return CardPosition.FaceUpDefence;
                    }
                }
                if (Duel.Player == 1)
                {
                    if (cardData.Defense >= cardData.Attack || Util.IsOneEnemyBetterThanValue(cardData.Attack, true))
                    {
                        return CardPosition.FaceUpDefence;
                    }
                }
                int cardAttack = cardData.Attack;
                int bestBotAttack = Math.Max(Util.GetBestAttack(Bot), cardAttack);
                if (Util.IsAllEnemyBetterThanValue(bestBotAttack, true))
                {
                    return CardPosition.FaceUpDefence;
                }
            }
            return base.OnSelectPosition(cardId, positions);
        }

        public bool AshBlossomActivate()
        {
            if (CheckWhetherNegated(true) || !CheckLastChainShouldNegated()) return false;
            if (Duel.LastChainPlayer == 1 && Util.GetLastChainCard().IsCode(_CardId.MaxxC))
            {
                if (CheckAtAdvantage() && Duel.Turn > 1)
                {
                    return false;
                }
            }
            return DefaultAshBlossomAndJoyousSpring();
        }

        public bool MaxxCActivate()
        {
            if (CheckWhetherNegated(true) || Duel.LastChainPlayer == 0) return false;
            return DefaultMaxxC();
        }

        public bool InfiniteImpermanenceActivate()
        {
            if (CheckWhetherNegated()) return false;
            foreach (ClientCard m in Enemy.GetMonsters())
            {
                if (m.IsMonsterShouldBeDisabledBeforeItUseEffect() && !m.IsDisabled() && Duel.LastChainPlayer != 0)
                {
                    if (Card.Location == CardLocation.SpellZone)
                    {
                        for (int i = 0; i < 5; ++i)
                        {
                            if (Bot.SpellZone[i] == Card)
                            {
                                infiniteImpermanenceList.Add(i);
                                break;
                            }
                        }
                    }
                    if (Card.Location == CardLocation.Hand)
                    {
                        SelectSTPlace(Card, true);
                    }
                    AI.SelectCard(m);
                    return true;
                }
            }

            ClientCard LastChainCard = Util.GetLastChainCard();

            if (Card.Location == CardLocation.SpellZone)
            {
                int this_seq = -1;
                int that_seq = -1;
                for (int i = 0; i < 5; ++i)
                {
                    if (Bot.SpellZone[i] == Card) this_seq = i;
                    if (LastChainCard != null
                        && LastChainCard.Controller == 1 && LastChainCard.Location == CardLocation.SpellZone && Enemy.SpellZone[i] == LastChainCard) that_seq = i;
                    else if (Duel.Player == 0 && Util.GetProblematicEnemySpell() != null
                        && Enemy.SpellZone[i] != null && Enemy.SpellZone[i].IsFloodgate()) that_seq = i;
                }
                if ((this_seq * that_seq >= 0 && this_seq + that_seq == 4)
                    || (Util.IsChainTarget(Card))
                    || (LastChainCard != null && LastChainCard.Controller == 1 && LastChainCard.IsCode(_CardId.HarpiesFeatherDuster)))
                {
                    ClientCard target = GetProblematicEnemyMonster(canBeTarget: true);
                    List<ClientCard> enemyMonsters = Enemy.GetMonsters();
                    AI.SelectCard(target);
                    infiniteImpermanenceList.Add(this_seq);
                    return true;
                }
            }
            if ((LastChainCard == null || LastChainCard.Controller != 1 || LastChainCard.Location != CardLocation.MonsterZone
                || LastChainCard.IsDisabled() || LastChainCard.IsShouldNotBeTarget() || LastChainCard.IsShouldNotBeSpellTrapTarget()))
                return false;

            if (Card.Location == CardLocation.SpellZone)
            {
                for (int i = 0; i < 5; ++i)
                {
                    if (Bot.SpellZone[i] == Card)
                    {
                        infiniteImpermanenceList.Add(i);
                        break;
                    }
                }
            }
            if (Card.Location == CardLocation.Hand)
            {
                SelectSTPlace(Card, true);
            }
            if (LastChainCard != null) AI.SelectCard(LastChainCard);
            else
            {
                List<ClientCard> enemyMonsters = Enemy.GetMonsters();
                enemyMonsters.Sort(CardContainer.CompareCardAttack);
                enemyMonsters.Reverse();
                foreach (ClientCard card in enemyMonsters)
                {
                    if (card.IsFaceup() && !card.IsShouldNotBeTarget() && !card.IsShouldNotBeSpellTrapTarget())
                    {
                        AI.SelectCard(card);
                        return true;
                    }
                }
            }
            return true;
        }

        public bool CrossoutDesignatorActivate()
        {
            if (CheckWhetherNegated() || !CheckLastChainShouldNegated()) return false;
            if (Duel.LastChainPlayer == 1 && Util.GetLastChainCard() != null)
            {
                int code = Util.GetLastChainCard().Id;
                int alias = Util.GetLastChainCard().Alias;
                if (alias != 0 && alias - code < 10) code = alias;
                if (code == 0) return false;
                if (DefaultCheckWhetherCardIdIsNegated(code)) return false;
                if (CheckRemainInDeck(code) > 0)
                {
                    if (!(Card.Location == CardLocation.SpellZone))
                    {
                        SelectSTPlace(null, true);
                    }
                    AI.SelectAnnounceID(code);
                    currentNegateCardList.AddRange(Enemy.MonsterZone.Where(c => c != null && c.IsFaceup() && c.IsCode(code)));
                    return true;
                }
            }
            return false;
        }

        public bool CalledbytheGraveActivate()
        {
            if (CheckWhetherNegated() || !CheckLastChainShouldNegated())
            {
                return false;
            }
            if (Duel.LastChainPlayer == 1)
            {
                if (Util.GetLastChainCard().IsMonster())
                {
                    int code = Util.GetLastChainCard().GetOriginCode();
                    if (code == 0) return false;
                    if (DefaultCheckWhetherCardIdIsNegated(code)) return false;
                    if (Util.GetLastChainCard().IsCode(_CardId.MaxxC) && CheckAtAdvantage() && Duel.Turn > 1)
                    {
                        return false;
                    }
                    ClientCard graveTarget = Enemy.Graveyard.GetFirstMatchingCard(card => card.IsMonster() && card.GetOriginCode() == code);
                    if (graveTarget != null)
                    {
                        if (!(Card.Location == CardLocation.SpellZone))
                        {
                            SelectSTPlace(null, true);
                        }
                        AI.SelectCard(graveTarget);
                        currentDestroyCardList.Add(graveTarget);
                        return true;
                    }
                }

                foreach (ClientCard graveCard in Enemy.Graveyard)
                {
                    if (Duel.ChainTargets.Contains(graveCard) && graveCard.IsMonster())
                    {
                        if (!(Card.Location == CardLocation.SpellZone))
                        {
                            SelectSTPlace(null, true);
                        }
                        int code = graveCard.Id;
                        AI.SelectCard(graveCard);
                        currentDestroyCardList.Add(graveCard);
                        return true;
                    }
                }

                if (Duel.ChainTargets.Contains(Card))
                {
                    List<ClientCard> enemyMonsters = Enemy.Graveyard.GetMatchingCards(card => card.IsMonster()).ToList();
                    if (enemyMonsters.Count > 0)
                    {
                        enemyMonsters.Sort(CardContainer.CompareCardAttack);
                        enemyMonsters.Reverse();
                        int code = enemyMonsters[0].Id;
                        AI.SelectCard(code);
                        currentDestroyCardList.Add(enemyMonsters[0]);
                        return true;
                    }
                }
            }

            if (Duel.LastChainPlayer == 1) return false;
            List<ClientCard> targets = GetDangerousCardinEnemyGrave(true);
            if (targets.Count > 0)
            {
                int code = targets[0].Id;
                if (!(Card.Location == CardLocation.SpellZone))
                {
                    SelectSTPlace(null, true);
                }
                AI.SelectCard(code);
                currentDestroyCardList.Add(targets[0]);
                return true;
            }

            return false;
        }

        public bool SpellSetCheck()
        {
            if (Duel.Phase == DuelPhase.Main1 && Bot.HasAttackingMonster() && Duel.Turn > 1) return false;
            List<int> onlyOneSetList = new List<int> { };
            if (onlyOneSetList.Contains(Card.Id) && Bot.HasInSpellZone(Card.Id))
            {
                return false;
            }

            if ((Card.IsTrap() || Card.HasType(CardType.QuickPlay)))
            {

                List<int> avoid_list = new List<int>();
                int setFornfiniteImpermanence = 0;
                for (int i = 0; i < 5; ++i)
                {
                    if (Enemy.SpellZone[i] != null && Enemy.SpellZone[i].IsFaceup() && Bot.SpellZone[4 - i] == null)
                    {
                        avoid_list.Add(4 - i);
                        setFornfiniteImpermanence += (int)System.Math.Pow(2, 4 - i);
                    }
                }
                if (Bot.HasInHand(_CardId.InfiniteImpermanence))
                {
                    if (Card.IsCode(_CardId.InfiniteImpermanence))
                    {
                        AI.SelectPlace(setFornfiniteImpermanence);
                        return true;
                    }
                    else
                    {
                        SelectSTPlace(Card, false, avoid_list);
                        return true;
                    }
                }
                else
                {
                    SelectSTPlace();
                }
                if (Card.IsCode(CardId.MalissC_MTP07)) mtp07SetThisTurn = true;
                if (Card.IsCode(CardId.MalissC_GWC06)) gwc06SetThisTurn = true;
                return true;
            }

            return false;
        }

        public List<ClientCard> GetDangerousCardinEnemyGrave(bool onlyMonster = false)
        {
            List<ClientCard> result = Enemy.Graveyard.GetMatchingCards(card =>
                (!onlyMonster || card.IsMonster()) && (card.HasSetcode(SetcodeOrcust) || card.HasSetcode(SetcodePhantom) || card.HasSetcode(SetcodeHorus) || card.HasSetcode(SetcodeDarkWorld) || card.HasSetcode(SetcodeSkyStriker))).ToList();
            List<int> dangerMonsterIdList = new List<int> { 99937011, 63542003, 9411399, 28954097, 30680659, 32731036 };
            result.AddRange(Enemy.Graveyard.GetMatchingCards(card => dangerMonsterIdList.Contains(card.Id)));
            return result;
        }

        public bool CheckWhetherNegated(bool disablecheck = true, bool toFieldCheck = false, CardType type = 0)
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
                if ((toFieldCheck && (((int)type & (int)CardType.Link) != 0)) || Card.IsDefense())
                {
                    if (Enemy.MonsterZone.Any(card => CheckNumber41(card)) || Bot.MonsterZone.Any(card => CheckNumber41(card))) return true;
                }
                if (Enemy.HasInSpellZone(CardId.SkillDrain, true)) return true;
            }
            if (disablecheck) return (Card.Location == CardLocation.MonsterZone || Card.Location == CardLocation.SpellZone) && Card.IsDisabled() && Card.IsFaceup();
            return false;
        }

        public bool CheckNumber41(ClientCard card)
        {
            return card != null && card.IsFaceup() && card.IsCode(CardId.Number41BagooskatheTerriblyTiredTapir) && card.IsDefense() && !card.IsDisabled();
        }

        public void SelectSTPlace(ClientCard card = null, bool avoidImpermanence = false, List<int> avoidList = null)
        {
            if (card == null) card = Card;
            List<int> list = new List<int>();
            for (int seq = 0; seq < 5; ++seq)
            {
                if (Bot.SpellZone[seq] == null)
                {
                    if (card != null && card.Location == CardLocation.Hand && avoidImpermanence && infiniteImpermanenceList.Contains(seq)) continue;
                    if (avoidList != null && avoidList.Contains(seq)) continue;
                    list.Add(seq);
                }
            }
            int n = list.Count;
            while (n-- > 1)
            {
                int index = Program.Rand.Next(list.Count);
                int nextIndex = (index + Program.Rand.Next(list.Count - 1)) % list.Count;
                int tempInt = list[index];
                list[index] = list[nextIndex];
                list[nextIndex] = tempInt;
            }
            if (avoidImpermanence && Bot.GetMonsters().Any(c => c.IsFaceup() && !c.IsDisabled()))
            {
                foreach (int seq in list)
                {
                    ClientCard enemySpell = Enemy.SpellZone[4 - seq];
                    if (enemySpell != null && enemySpell.IsFacedown()) continue;
                    int zone = (int)System.Math.Pow(2, seq);
                    AI.SelectPlace(zone);
                    return;
                }
            }
            foreach (int seq in list)
            {
                int zone = (int)System.Math.Pow(2, seq);
                AI.SelectPlace(zone);
                return;
            }
            AI.SelectPlace(0);
        }

        public bool CheckSpellWillBeNegate(bool isCounter = false, ClientCard target = null)
        {
            if (target == null) target = Card;
            if (target.Location != CardLocation.SpellZone && target.Location != CardLocation.Hand) return false;

            if (Enemy.HasInMonstersZone(CardId.NaturalExterio, true) && !isCounter) return true;
            if (target.IsSpell())
            {
                if (Enemy.HasInMonstersZone(CardId.NaturalBeast, true)) return true;
                if (Enemy.HasInSpellZone(CardId.ImperialOrder, true) || Bot.HasInSpellZone(CardId.ImperialOrder, true)) return true;
                if (Enemy.HasInMonstersZone(CardId.SwordsmanLV7, true) || Bot.HasInMonstersZone(CardId.SwordsmanLV7, true)) return true;
            }
            if (target.IsTrap() && (Enemy.HasInSpellZone(CardId.RoyalDecree, true) || Bot.HasInSpellZone(CardId.RoyalDecree, true))) return true;
            if (target.Location == CardLocation.SpellZone && (target.IsSpell() || target.IsTrap()))
            {
                int selfSeq = -1;
                for (int i = 0; i < 5; ++i)
                {
                    if (Bot.SpellZone[i] == Card) selfSeq = i;
                }
                if (infiniteImpermanenceList.Contains(selfSeq)) return true;
            }
            return false;
        }

        public bool CheckLastChainShouldNegated()
        {
            ClientCard lastcard = Util.GetLastChainCard();
            if (lastcard == null || lastcard.Controller != 1) return false;
            if (lastcard.IsMonster() && lastcard.HasSetcode(SetcodeTimeLord) && Duel.Phase == DuelPhase.Standby) return false;
            if (notToNegateIdList.Contains(lastcard.Id)) return false;
            if (DefaultCheckWhetherCardIsNegated(lastcard)) return false;
            if (Duel.Turn == 1 && lastcard.IsCode(_CardId.MaxxC)) return false;

            return true;
        }

        public ClientCard GetProblematicEnemyMonster(int attack = 0, bool canBeTarget = false, bool ignoreCurrentDestroy = false, CardType selfType = 0)
        {
            ClientCard floodagateCard = Enemy.GetMonsters().Where(c => c?.Data != null && (ignoreCurrentDestroy || !currentDestroyCardList.Contains(c))
                && c.IsFloodgate() && c.IsFaceup()
                && CheckCanBeTargeted(c, canBeTarget, selfType)
                && CheckShouldNotIgnore(c)).OrderByDescending(card => card.Attack).FirstOrDefault();
            if (floodagateCard != null) return floodagateCard;

            ClientCard dangerCard = Enemy.MonsterZone.Where(c => c?.Data != null && (ignoreCurrentDestroy || !currentDestroyCardList.Contains(c))
                && c.IsMonsterDangerous() && c.IsFaceup() && CheckCanBeTargeted(c, canBeTarget, selfType)
                && CheckShouldNotIgnore(c)).OrderByDescending(card => card.Attack).FirstOrDefault();
            if (dangerCard != null) return dangerCard;

            ClientCard invincibleCard = Enemy.MonsterZone.Where(c => c?.Data != null && (ignoreCurrentDestroy || !currentDestroyCardList.Contains(c))
                && c.IsMonsterInvincible() && c.IsFaceup() && CheckCanBeTargeted(c, canBeTarget, selfType)
                && CheckShouldNotIgnore(c)).OrderByDescending(card => card.Attack).FirstOrDefault();
            if (invincibleCard != null) return invincibleCard;

            ClientCard equippedCard = Enemy.MonsterZone.Where(c => c?.Data != null && (ignoreCurrentDestroy || !currentDestroyCardList.Contains(c))
                && c.EquipCards.Count > 0 && CheckCanBeTargeted(c, canBeTarget, selfType)
                && CheckShouldNotIgnore(c)).OrderByDescending(card => card.Attack).FirstOrDefault();
            if (equippedCard != null) return equippedCard;

            ClientCard enemyExtraMonster = Enemy.MonsterZone.Where(c => c != null && (ignoreCurrentDestroy || !currentDestroyCardList.Contains(c))
                && (c.HasType(CardType.Fusion | CardType.Ritual | CardType.Synchro | CardType.Xyz) || (c.HasType(CardType.Link) && c.LinkCount >= 2))
                && CheckCanBeTargeted(c, canBeTarget, selfType) && CheckShouldNotIgnore(c)).OrderByDescending(card => card.Attack).FirstOrDefault();
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

        public bool CheckCanBeTargeted(ClientCard card, bool canBeTarget, CardType selfType)
        {
            if (card == null) return true;
            if (canBeTarget)
            {
                if (card.IsShouldNotBeTarget()) return false;
                if (((int)selfType & (int)CardType.Monster) > 0 && card.IsShouldNotBeMonsterTarget()) return false;
                if (((int)selfType & (int)CardType.Spell) > 0 && card.IsShouldNotBeSpellTrapTarget()) return false;
                if (((int)selfType & (int)CardType.Trap) > 0 && (card.IsShouldNotBeSpellTrapTarget() && !card.IsDisabled())) return false;
            }
            return true;
        }

        public bool CheckShouldNotIgnore(ClientCard cards, bool ignore = false)
        {
            return !ignore || (!currentDestroyCardList.Contains(cards) && !currentNegateCardList.Contains(cards));
        }

        public List<ClientCard> GetProblematicEnemyCardList(bool canBeTarget = false, bool ignoreSpells = false, CardType selfType = 0)
        {
            List<ClientCard> resultList = new List<ClientCard>();

            List<ClientCard> floodagateList = Enemy.MonsterZone.Where(c => c?.Data != null && !currentDestroyCardList.Contains(c)
                && c.IsFloodgate() && c.IsFaceup() && CheckCanBeTargeted(c, canBeTarget, selfType)).OrderByDescending(card => card.Attack).ToList();
            if (floodagateList.Count > 0) resultList.AddRange(floodagateList);

            List<ClientCard> problemEnemySpellList = Enemy.SpellZone.Where(c => c?.Data != null && !resultList.Contains(c) && !currentDestroyCardList.Contains(c)
                && c.IsFloodgate() && c.IsFaceup() && CheckCanBeTargeted(c, canBeTarget, selfType)).ToList();
            if (problemEnemySpellList.Count > 0) resultList.AddRange(ShuffleList(problemEnemySpellList));

            List<ClientCard> dangerList = Enemy.MonsterZone.Where(c => c?.Data != null && !resultList.Contains(c) && !currentDestroyCardList.Contains(c)
                && c.IsMonsterDangerous() && c.IsFaceup() && CheckCanBeTargeted(c, canBeTarget, selfType)).OrderByDescending(card => card.Attack).ToList();
            if (dangerList.Count > 0 && (Duel.Player == 0 || (Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2))) resultList.AddRange(dangerList);

            List<ClientCard> invincibleList = Enemy.MonsterZone.Where(c => c?.Data != null && !resultList.Contains(c) && !currentDestroyCardList.Contains(c)
                && c.IsMonsterInvincible() && c.IsFaceup() && CheckCanBeTargeted(c, canBeTarget, selfType)).OrderByDescending(card => card.Attack).ToList();
            if (invincibleList.Count > 0) resultList.AddRange(invincibleList);

            List<ClientCard> enemyMonsters = Enemy.GetMonsters().Where(c => !currentDestroyCardList.Contains(c)).OrderByDescending(card => card.Attack).ToList();
            if (enemyMonsters.Count > 0)
            {
                foreach (ClientCard target in enemyMonsters)
                {
                    if ((target.HasType(CardType.Fusion | CardType.Ritual | CardType.Synchro | CardType.Xyz)
                            || (target.HasType(CardType.Link) && target.LinkCount >= 2))
                        && !resultList.Contains(target) && CheckCanBeTargeted(target, canBeTarget, selfType))
                    {
                        resultList.Add(target);
                    }
                }
            }

            List<ClientCard> spells = Enemy.GetSpells().Where(c => c.IsFaceup() && !currentDestroyCardList.Contains(c)
                && c.HasType(CardType.Equip | CardType.Pendulum | CardType.Field | CardType.Continuous) && CheckCanBeTargeted(c, canBeTarget, selfType)
                && !notToDestroySpellTrap.Contains(c.Id)).ToList();
            if (spells.Count > 0 && !ignoreSpells) resultList.AddRange(ShuffleList(spells));

            return resultList;
        }

        public List<ClientCard> GetNormalEnemyTargetList(bool canBeTarget = true, bool ignoreCurrentDestroy = false, CardType selfType = 0)
        {
            List<ClientCard> targetList = GetProblematicEnemyCardList(canBeTarget, selfType: selfType);
            List<ClientCard> enemyMonster = Enemy.GetMonsters().Where(card => card.IsFaceup() && !targetList.Contains(card)
                && (!ignoreCurrentDestroy || !currentDestroyCardList.Contains(card))).ToList();
            enemyMonster.Sort(CardContainer.CompareCardAttack);
            enemyMonster.Reverse();
            targetList.AddRange(enemyMonster);
            targetList.AddRange(ShuffleList(Enemy.GetSpells().Where(card => (!ignoreCurrentDestroy || !currentDestroyCardList.Contains(card)) && enemyPlaceThisTurn.Contains(card)).ToList()));
            targetList.AddRange(ShuffleList(Enemy.GetSpells().Where(card => (!ignoreCurrentDestroy || !currentDestroyCardList.Contains(card)) && !enemyPlaceThisTurn.Contains(card)).ToList()));
            targetList.AddRange(ShuffleList(Enemy.GetMonsters().Where(card => card.IsFacedown() && (!ignoreCurrentDestroy || !currentDestroyCardList.Contains(card))).ToList()));

            return targetList;
        }

        public List<ClientCard> GetMonsterListForTargetNegate(bool canBeTarget = false, CardType selfType = 0)
        {
            List<ClientCard> resultList = new List<ClientCard>();
            if (CheckWhetherNegated())
            {
                return resultList;
            }

            ClientCard target = Enemy.MonsterZone.FirstOrDefault(card => card?.Data != null
                    && card.IsMonsterShouldBeDisabledBeforeItUseEffect() && card.IsFaceup() && !card.IsShouldNotBeTarget()
                    && CheckCanBeTargeted(card, canBeTarget, selfType)
                    && !currentNegateCardList.Contains(card));
            if (target != null)
            {
                resultList.Add(target);
            }

            foreach (ClientCard chainingCard in Duel.CurrentChain)
            {
                if (chainingCard.Location == CardLocation.MonsterZone && chainingCard.Controller == 1 && !chainingCard.IsDisabled()
                && CheckCanBeTargeted(chainingCard, canBeTarget, selfType) && !currentNegateCardList.Contains(chainingCard))
                {
                    resultList.Add(chainingCard);
                }
            }

            return resultList;
        }

        public ClientCard GetBestEnemyMonster(bool onlyFaceup = false, bool canBeTarget = false)
        {
            ClientCard card = GetProblematicEnemyMonster(0, canBeTarget);
            if (card != null) return card;
            card = Enemy.MonsterZone.GetHighestAttackMonster(canBeTarget);
            if (card != null) return card;
            List<ClientCard> monsters = Enemy.GetMonsters();
            if (monsters.Count > 0 && !onlyFaceup) return ShuffleCardList(monsters)[0];
            return null;
        }

        public ClientCard GetBestEnemySpell(bool onlyFaceup = false, bool canBeTarget = false)
        {
            List<ClientCard> problemEnemySpellList = Enemy.SpellZone.Where(c => c?.Data != null
                && c.IsFloodgate() && c.IsFaceup() && (!canBeTarget || !c.IsShouldNotBeTarget())).ToList();
            if (problemEnemySpellList.Count > 0)
            {
                return ShuffleCardList(problemEnemySpellList)[0];
            }

            List<ClientCard> spells = Enemy.GetSpells().Where(card => !(card.IsFaceup() && card.IsCode(_CardId.EvenlyMatched))).ToList();

            List<ClientCard> faceUpList = spells.Where(ecard => ecard.IsFaceup() && (ecard.HasType(CardType.Continuous) || ecard.HasType(CardType.Field) || ecard.HasType(CardType.Pendulum))).ToList();
            if (faceUpList.Count > 0)
            {
                return ShuffleCardList(faceUpList)[0];
            }

            if (spells.Count > 0 && !onlyFaceup)
            {
                return ShuffleCardList(spells)[0];
            }

            return null;
        }

        public ClientCard GetBestEnemyCard(bool onlyFaceup = false, bool canBeTarget = false, bool checkGrave = false)
        {
            ClientCard card = GetBestEnemyMonster(onlyFaceup, canBeTarget);
            if (card != null) return card;

            card = GetBestEnemySpell(onlyFaceup, canBeTarget);
            if (card != null) return card;

            if (checkGrave && Enemy.Graveyard.Count > 0)
            {
                List<ClientCard> graveMonsterList = Enemy.Graveyard.GetMatchingCards(c => c.IsMonster()).ToList();
                if (graveMonsterList.Count > 0)
                {
                    graveMonsterList.Sort(CardContainer.CompareCardAttack);
                    graveMonsterList.Reverse();
                    return graveMonsterList[0];
                }
                return ShuffleCardList(Enemy.Graveyard.ToList())[0];
            }

            return null;
        }

        private int LinkVal(ClientCard c) => (c != null && c.HasType(CardType.Link)) ? Math.Max(1, c.LinkCount) : 1;
        private static readonly int EMZ_LEFT = 5;
        private static readonly int EMZ_RIGHT = 6;
        private bool IsInEMZ(ClientCard c)
        {
            var mz = Bot.MonsterZone;
            return (mz.Length > 5 && mz[5] == c) || (mz.Length > 6 && mz[6] == c);
        }

        private bool HasFreeEMZ()
        {
            var mz = Bot.MonsterZone;
            bool slot5Free = mz.Length > 5 && mz[5] == null;
            bool slot6Free = mz.Length > 6 && mz[6] == null;
            return slot5Free || slot6Free;
        }
        #endregion

        #region work space #1
        public override void OnChainSolved(int chainIndex)
        {
            ClientCard currentCard = Duel.GetCurrentSolvingChainCard();
            var solving = Duel.GetCurrentSolvingChainCard();
            bool neg = Duel.IsCurrentSolvingChainNegated();
            if (currentCard != null && !Duel.IsCurrentSolvingChainNegated() && currentCard.Controller == 1)
            {
                if (currentCard.IsCode(CardId.Lancea)) enemyActivateLancea = true;
                if (currentCard.IsCode(_CardId.MaxxC)) enemyActivateMaxxC = true;
                if (currentCard.IsCode(CardId.Fuwalos)) enemyActivateFuwalos = true;
                if (currentCard.IsCode(_CardId.LockBird)) enemyActivateLockBird = true;
                if (currentCard.IsCode(_CardId.InfiniteImpermanence))
                {
                    for (int i = 0; i < 5; ++i)
                    {
                        if (Enemy.SpellZone[i] == currentCard)
                        {
                            infiniteImpermanenceList.Add(4 - i);
                            break;
                        }
                    }
                }
                var last = Duel.GetCurrentSolvingChainCard();
                if (last != null)
                {
                    if (last.IsSpell() && (last.HasType(CardType.Field) || last.HasType(CardType.Continuous) || last.HasType(CardType.Equip)))
                        _oppJustActivatedPersistentSpell = true;
                    _prefWindowTTL = Math.Max(_prefWindowTTL, 2);
                }
            }
            if (currentCard != null && currentCard.Controller == 0 && currentCard.IsCode(CardId.SplashMage))
            {
                if (Duel.IsCurrentSolvingChainNegated())
                    splashNegatedThisTurn = true;
            }
        }
        public override void OnChainEnd()
        {
            escapeTargetList.Clear();
            currentNegateCardList.Clear();
            currentDestroyCardList.Clear();
            enemyActivateInfiniteImpermanenceFromHand = false;
            _oppJustActivatedPersistentSpell = false;
            int curMon = Enemy.GetMonsterCount();
            if (curMon > _enemyMonsterCountSnap) _oppJustSummoned = true;
            _enemyMonsterCountSnap = curMon;
            int curFD = Enemy.SpellZone.Count(c => c != null && c.IsFacedown());
            if (curFD > _enemyFacedownSTSnap) _oppJustSet = true;
            _enemyFacedownSTSnap = curFD;
            for (int idx = enemyPlaceThisTurn.Count - 1; idx >= 0; idx--)
            {
                ClientCard checkTarget = enemyPlaceThisTurn[idx];
                if (checkTarget == null || (checkTarget.Location != CardLocation.SpellZone && checkTarget.Location != CardLocation.MonsterZone))
                {
                    enemyPlaceThisTurn.RemoveAt(idx);
                }
            }
            base.OnChainEnd();
        }
        public override void OnNewTurn()
        {
            if (Duel.Player == 0)
            {
                myTurnCount++;
            }
            enemyActivateLancea = false; //added for Maliss
            enemyActivateFuwalos = false; //added
            enemyActivateMaxxC = false;
            enemyActivateLockBird = false;
            enemyActivateInfiniteImpermanenceFromHand = false;
            if (dimensionShifterCount > 0) dimensionShifterCount--;
            infiniteImpermanenceList.Clear();
            currentNegateCardList.Clear();
            currentDestroyCardList.Clear();
            sendToGYThisTurn.Clear();
            activatedCardIdList.Clear();
            enemyPlaceThisTurn.Clear();
            summonThisTurn.Clear();

            // reset Maliss flags
            usedNormalSummon = false;
            ssChessyCat = false;
            ssDormouse = false;
            ssMarchHare = false;
            ssWhiteRabbit = false;
            ActiveMarchHare = false;
            ActiveUnderground = false;
            step1Done = false; 
            step2Done = false;
            lastRevivedIdBySplash = 0;
            mtp07SetThisTurn = false;
            gwc06SetThisTurn = false;
            splashNegatedThisTurn = false;
            ssRRThisTurn = false;
            ssWBThisTurn = false;
            ssHCThisTurn = false;
            _didSplashToRR = _didRRtoWicckid = _didSummonToWicckidArrow = _didWBFromWicckid = false;
            _finishPlanDecided = false;
            _preferWicckidArrows = false;
            _rrSelfSSPlacing = false;
            _forceTranscodeBit = 0;
            _oppJustActivatedPersistentSpell = false;
            _oppJustSummoned = false;
            _oppJustSet = false;
            _enemyMonsterCountSnap = Enemy.GetMonsterCount();
            _enemyFacedownSTSnap = Enemy.SpellZone.Count(c => c != null && c.IsFacedown());
            _prefWindowTTL = 0;
            fullBoard1 = false;
            Allied_End = false;
            nsplan = false;
            nsBackupplan = false;
            NSDorMouse = false;
            nsLanceaplan = false;
            base.OnNewTurn();
        }
        public override bool OnSelectYesNo(int desc)
        {
            if (desc == Util.GetStringId(CardId.MalissQ_WhiteBinder, 0))
            {
                bool anyGY = (Bot.Graveyard.Count > 0) || (Enemy.Graveyard.Count > 0);
                return anyGY;
            }
            return base.OnSelectYesNo(desc);
        }
        private bool DontSelfNG() { return Duel.LastChainPlayer != 0; }

        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, int hint, bool cancelable)
        {
            var solving = Duel.GetCurrentSolvingChainCard();
            if (cards != null && cards.Count > 0 && solving != null)
            {
                if (solving.IsCode(CardId.MalissQ_RedRansom))
                {
                    var searchPool = cards
                        .Where(c => c != null &&
                                    (c.IsCode(CardId.MalissInUnderground) ||
                                     c.IsCode(CardId.MalissInTheMirror)))
                        .ToList();

                    if (searchPool.Count > 0)
                    {
                        bool shouldUG = ShouldSearchUnderground();
                        int chooseId = 0;
                        if (shouldUG && searchPool.Any(c => c.IsCode(CardId.MalissInUnderground)))
                        {
                            chooseId = CardId.MalissInUnderground;
                        }
                        else if (searchPool.Any(c => c.IsCode(CardId.MalissInTheMirror)))
                        {
                            chooseId = CardId.MalissInTheMirror;
                        }
                        if (chooseId != 0)
                        {
                            var pick = searchPool.First(c => c.IsCode(chooseId));
                            return new List<ClientCard> { pick };
                        }
                    }
                }
                if (hint == HintMsg.Set && solving.IsCode(CardId.MalissQ_WhiteBinder))
                {
                    ClientCard pick = null;
                    pick = cards.FirstOrDefault(c => c.Id == CardId.MalissC_GWC06 && c.Location == CardLocation.Deck);
                    if (pick == null)
                    {
                        pick = cards.FirstOrDefault(c => c.Id == CardId.MalissC_GWC06);
                    }
                    if (pick == null)
                    {
                        pick = cards.FirstOrDefault(c => c.Id == CardId.MalissC_MTP07);
                    }
                    if (pick != null)
                    {
                        if (pick.Id == CardId.MalissC_GWC06)
                            gwc06SetThisTurn = true;
                        else if (pick.Id == CardId.MalissC_MTP07)
                            mtp07SetThisTurn = true;

                        return new List<ClientCard> { pick };
                    }

                }
            }
            return base.OnSelectCard(cards, min, max, hint, cancelable);
        }

        #endregion


        #region work space #2
        private int GetMMZCount() => Bot.MonsterZone.Take(5).Count(c => c != null);
        private bool HasFreeMMZ() => GetMMZCount() < 5;
        private bool HaveTwoBodies() => Bot.GetMonsterCount() >= 2;
        private bool ShouldFastEndToSPLK() =>
            enemyActivateMaxxC || enemyActivateFuwalos;
        private bool Step1Complete()
        {
            return Bot.HasInMonstersZone(CardId.MalissP_Dormouse)
                && Bot.HasInMonstersZone(CardId.MalissP_WhiteRabbit);
        }
        private bool CanStartStep1()
        {
            if (enemyActivateLancea || ShouldFastEndToSPLK()) return false;
            if (HaveTwoBodies()) return false;
            if (!HasFreeMMZ()) return false;
            bool haveStarterInHand =
                                    Bot.HasInHand(CardId.MalissP_Dormouse) ||
                                    Bot.HasInHand(CardId.MalissP_WhiteRabbit) ||
                                    Bot.HasInHand(CardId.GoldSarcophagus) ||
                                    Bot.HasInHand(CardId.TERRAFORMING) ||
                                    Bot.HasInHand(CardId.MalissInUnderground);
            return haveStarterInHand;
        }
        private bool CanContinueStep1()
        {
            if (enemyActivateLancea) return false;
            return !HaveTwoBodies() && HasFreeMMZ();
        }
        private int PickMalissTrapToSet()
        {
            int pref;
            if (Duel.Player == 0 && Bot.HasInMonstersZone(CardId.MalissP_Dormouse) && Bot.HasInMonstersZone(CardId.MalissP_WhiteRabbit))
            {
                pref = CardId.MalissC_GWC06;
            }
            else if (Duel.Player == 0 && (Bot.HasInMonstersZoneOrInGraveyard(CardId.MalissQ_RedRansom) || Bot.HasInBanished(CardId.MalissQ_RedRansom)))
            {
                pref = CardId.MalissC_GWC06;
            }
            else if (Duel.Player == 0 && Bot.HasInHand(CardId.MalissP_MarchHare) && !ActiveMarchHare && !ssWhiteRabbit)
            {
                pref = CardId.MalissC_GWC06;
            }
            else if (Duel.Player == 0 && nsBackupplan)
            {
                pref = CardId.MalissC_GWC06;
            }
            else if (Duel.Player == 1)
            {
                pref = CardId.MalissC_MTP07;
            }
            else
            {
                pref = CardId.MalissC_MTP07;
            }
            if (CheckRemainInDeck(pref) > 0)
            {
                return pref;
            }

            return 0;
        }

        private bool ActLittleKnight()
        {
            if (ActivateDescription == -1 || ActivateDescription == Util.GetStringId(CardId.SP_LITTLE_KNIGHT, 0))
            {
                List<ClientCard> problemCardList = GetProblematicEnemyCardList(true, selfType: CardType.Monster);
                problemCardList.AddRange(GetDangerousCardinEnemyGrave(false));
                problemCardList.AddRange(GetNormalEnemyTargetList(true, true, CardType.Monster));
                problemCardList.AddRange(Enemy.Graveyard.Where(card => card.HasType(CardType.Monster)).OrderByDescending(card => card.Attack));
                problemCardList.AddRange(Enemy.Graveyard.Where(card => !card.HasType(CardType.Monster)));
                if (problemCardList.Count() > 0)
                {
                    AI.SelectCard(problemCardList);
                    activatedCardIdList.Add(Card.Id);
                    return true;
                }
            }
            else if (ActivateDescription == Util.GetStringId(CardId.SP_LITTLE_KNIGHT, 1))
            {
                ClientCard selfMonster = null;
                foreach (ClientCard target in Bot.GetMonsters())
                {
                    if (Duel.ChainTargets.Contains(target) && !escapeTargetList.Contains(target))
                    {
                        selfMonster = target;
                        break;
                    }
                }
                if (selfMonster == null)
                {
                    if (Duel.Player == 1)
                    {
                        selfMonster = Bot.GetMonsters().Where(card => card.IsAttack()).OrderBy(card => card.Attack).FirstOrDefault();
                        if (!Util.IsOneEnemyBetterThanValue(selfMonster.Attack, true)) selfMonster = null;
                    }
                }
                if (selfMonster != null)
                {
                    ClientCard nextMonster = null;
                    List<ClientCard> selfTargetList = Bot.GetMonsters().Where(card => card != selfMonster).ToList();
                    if (Enemy.GetMonsterCount() == 0 && selfTargetList.Count() > 0)
                    {
                        selfTargetList.Sort(CompareUsableAttack);
                        nextMonster = selfTargetList[0];
                        escapeTargetList.Add(nextMonster);
                    }
                    if (Enemy.GetMonsterCount() > 0)
                    {
                        nextMonster = GetBestEnemyMonster(true, true);
                        currentDestroyCardList.Add(nextMonster);
                    }
                    if (nextMonster != null)
                    {
                        AI.SelectCard(selfMonster);
                        AI.SelectNextCard(nextMonster);
                        escapeTargetList.Add(selfMonster);
                        activatedCardIdList.Add(Card.Id + 1);
                        return true;
                    }
                }
            }

            return false;
        }
        public int CompareUsableAttack(ClientCard cardA, ClientCard cardB)
        {
            if (cardA == null && cardB == null)
                return 0;
            if (cardA == null)
                return -1;
            if (cardB == null)
                return 1;
            int powerA = (cardA.IsDefense() && summonThisTurn.Contains(cardA)) ? 0 : cardA.Attack;
            int powerB = (cardB.IsDefense() && summonThisTurn.Contains(cardB)) ? 0 : cardB.Attack;
            if (powerA < powerB)
                return -1;
            if (powerA == powerB)
                return CardContainer.CompareCardLevel(cardA, cardB);
            return 1;
        }

        private bool Step1_Dormouse_NormalSummon()
        {
            if (!CanStartStep1()) return false;
            if (usedNormalSummon) return false;
            usedNormalSummon = true;
            NSDorMouse = true;
            return true;
        }
        private bool Dormouse_ForMH()
        {
            if (DefaultCheckWhetherCardIdIsNegated(Card.Id)) return false;
            if (!CanContinueStep1()) return false;
            if (Card.Location != CardLocation.MonsterZone) return false;
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            int pick = 0;
            if (goldstart || undergroundstart)
            {
                pick = (CheckRemainInDeck(CardId.MalissP_WhiteRabbit) > 0)
                    ? CardId.MalissP_WhiteRabbit
                    : CardId.MalissP_ChessyCat;
            }
            else
            {
                pick = (CheckRemainInDeck(CardId.MalissP_MarchHare) > 0)
                    ? CardId.MalissP_MarchHare
                    : (CheckRemainInDeck(CardId.MalissP_WhiteRabbit) > 0)
                    ? CardId.MalissP_WhiteRabbit
                    : CardId.MalissP_ChessyCat;
            }
            if (pick == 0) return false;
            AI.SelectCard(pick);
            return true;
        }

        private bool Step1_WhiteRabbit_SS_FromBanished()
        {
            if (DefaultCheckWhetherCardIdIsNegated(Card.Id)) return false;
            if (enemyActivateLancea) return false;
            if (Card.Id != CardId.MalissP_WhiteRabbit) return false;
            if (Card.Location != CardLocation.Removed) return false;
            if (Bot.LifePoints <= 300)
            {
                return false;
            }
            ssWhiteRabbit = true;
            return true;
        }

        private bool Step1_WhiteRabbit_SetTrapOnSummon()
        {
            if (DefaultCheckWhetherCardIdIsNegated(Card.Id)) return false;
            if (Card.Id != CardId.MalissP_WhiteRabbit) return false;
            if (Card.Location != CardLocation.MonsterZone) return false;
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;

            int trapToSet = PickMalissTrapToSet();
            if (trapToSet == 0) return false;

            if (trapToSet == CardId.MalissC_GWC06)
                gwc06SetThisTurn = true;

            if (trapToSet == CardId.MalissC_MTP07)
                mtp07SetThisTurn = true;

            AI.SelectCard(trapToSet);
            SelectSafeSTZoneAwayFromImperm();
            if (Step1Complete()) step1Done = true;
            return true;
        }

        private bool Step1_WhiteRabbit_NormalSummon()
        {
            if (!CanStartStep1()) return false;
            if (Bot.HasInHand(CardId.MalissP_Dormouse)) return false;
            if (usedNormalSummon) return false;
            usedNormalSummon = true;
            return true;
        }

        private bool Dormouse_SS_FromBanished()
        {
            if (DefaultCheckWhetherCardIdIsNegated(Card.Id)) return false;
            if (Card.Id != CardId.MalissP_Dormouse) return false;
            if (Card.Location != CardLocation.Removed) return false;
            if (enemyActivateLancea) return false;
            if (Bot.LifePoints <= 300)
            {
                return false;
            }
            ssDormouse = true;
            return true;
        }
        private bool ChessyCat_SS_FromBanished()
        {
            if (DefaultCheckWhetherCardIdIsNegated(Card.Id)) return false;
            if (Card.Id != CardId.MalissP_ChessyCat) return false;
            if (Card.Location != CardLocation.Removed) return false;
            if (enemyActivateLancea) return false;
            if (Bot.LifePoints <= 300)
            {
                return false;
            }
            ssChessyCat = true;
            return true;
        }

        private bool SummonLittleKnightFast()
        {
            if (!(Bot.HasInMonstersZone(CardId.LinkSpider)|| Bot.HasInMonstersZone(CardId.Linguriboh))) return false;
            if (!HaveTwoBodies()) return false;

            var mats = Bot.GetMonsters()
                .Where(c => c != null && c.IsFaceup() && c.HasType(CardType.Effect))
                .OrderBy(c => c.Attack)
                .Take(2).ToList();

            if (mats.Count < 2) return false;

            AI.SelectMaterials(mats);
            step1Done = true;
            return true;
        }
        private bool SPEmer()
        {
            if (Bot.HasInMonstersZone(CardId.FirewallDragon) ||
                Bot.HasInMonstersZone(CardId.AlliedCodeTalkerIgnister) ||
                Bot.HasInMonstersZone(CardId.Apollousa) ||
                Bot.HasInMonstersZone(CardId.AccesscodeTalker))
            {
                return false;
            }
            if (!HaveTwoBodies()) return false;

            var mats = PickLinkMatsMinCount(
                    targetLink: 2,
                    isEligible: m => m.HasType(CardType.Effect),
                    minCount: 2,
                    maxCount: 2,
                    avoidIds: new[] { CardId.Apollousa, CardId.AlliedCodeTalkerIgnister, CardId.AccesscodeTalker }
                );
            if (mats.Count != 2)
            {
                return false;
            }

            AI.SelectMaterials(mats);
            return true;
        }

        private bool GoldSarc_StartPiece()
        {
            if (CheckSpellWillBeNegate()) return false;
            if (enemyActivateLancea) { return false; }
            if (Bot.HasInHand(CardId.MalissP_Dormouse) || Bot.HasInHand(CardId.MalissP_WhiteRabbit)) { return false; }
            int pick = 0;
            if (!Bot.HasInMonstersZone(CardId.MalissP_Dormouse) && CheckRemainInDeck(CardId.MalissP_Dormouse) > 0 && !ssDormouse)
            { pick = CardId.MalissP_Dormouse; }
            else if (!Bot.HasInMonstersZone(CardId.MalissP_WhiteRabbit) && CheckRemainInDeck(CardId.MalissP_WhiteRabbit) > 0 && !ssWhiteRabbit)
            { pick = CardId.MalissP_WhiteRabbit; }
            else if (madeIt3 && !ssChessyCat)
            { pick = CardId.MalissP_ChessyCat; }
            else { return false; }
            if (pick == 0) return false;


            AI.SelectCard(pick);
            if (Card.Location == CardLocation.Hand)
            {
                SelectSTPlace(Card, true);
            }
            goldstart = true;
            return true;
        }
        private bool ExistsForUnderground(int id)
        {
            return CheckRemainInDeck(id) > 0
                || Bot.HasInHand(id)
                || Bot.HasInGraveyard(id);
        }
        private bool Underground_ActivateStarter()
        {
            if (enemyActivateLancea) return false;
            if (Bot.GetMonsterCount() != 0) return false;
            if (step1Done) return false;

            int pick = 0;
            if (ExistsForUnderground(CardId.MalissP_Dormouse))
                pick = CardId.MalissP_Dormouse;
            else if (ExistsForUnderground(CardId.MalissP_WhiteRabbit))
                pick = CardId.MalissP_WhiteRabbit;
            else if (ExistsForUnderground(CardId.MalissP_ChessyCat))
                pick = CardId.MalissP_ChessyCat;

            if (pick == 0) return false;

            AI.SelectYesNo(true);
            AI.SelectCard(pick);
            ActiveUnderground = true;
            undergroundstart = true;
            return true;
        }

        private bool Terra_GrabUnderground()
        {
            if (DefaultCheckWhetherCardIdIsNegated(Card.Id)) return false;
            if (CheckSpellWillBeNegate()) return false;
            if (ActiveUnderground) return false;
            if (Bot.HasInHand(CardId.MalissInUnderground) || Bot.HasInSpellZone(CardId.MalissInUnderground))
                return false;
            AI.SelectCard(CardId.MalissInUnderground);
            if (Card.Location == CardLocation.Hand)
            {
                SelectSTPlace(Card, true);
            }
            return true;
        }
        private bool HaveBackupOrWizardInHand()
        {
            return Bot.HasInHand(CardId.BackupIgnister) || Bot.HasInHand(CardId.WizardIgnister);
        }
        private bool HaveMHInHand()
        {
            return Bot.HasInHand(CardId.MalissP_MarchHare);
        }
        private bool IsMalissBody(ClientCard c)
        {
            return c != null && c.IsFaceup() && c.HasSetcode(SetcodeMaliss)
                               && c.IsCode(CardId.MalissP_Dormouse,
                                           CardId.MalissP_WhiteRabbit,
                                           CardId.MalissP_ChessyCat,
                                           CardId.MalissP_MarchHare);
        }
        private bool Emergency_NormalCat()
        {
            if (Bot.GetMonsterCount() != 0) return false;
            if (usedNormalSummon) return false;
            if (Bot.HasInHand(CardId.MalissP_Dormouse) || Bot.HasInHand(CardId.MalissP_WhiteRabbit) ||
                Bot.HasInHand(CardId.MalissInUnderground) || Bot.HasInHand(CardId.TERRAFORMING) ||
                Bot.HasInHand(CardId.GoldSarcophagus)) return false;
            if (!HaveMHInHand())
            {
                return false;
            }

            usedNormalSummon = true;
            return true;
        }
        private bool OneBody_Link1_Linguriboh()
        {
            if (!HaveBackupOrWizardInHand()) return false;
            if (Bot.HasInMonstersZone(CardId.Linguriboh)) return false;
            if (Bot.GetMonsterCount() != 1) return false;
            return true;
        }
        private bool OneBody_Link1_Almiraj()
        {
            if (!HaveBackupOrWizardInHand()) return false;
            if (Bot.HasInMonstersZone(CardId.SALAMANGREAT_ALMIRAJ)) return false;
            if (Bot.GetMonsterCount() != 1) return false;
            return true;
        }
        private bool OneBody_Backup_SS()
        {
            if (DefaultCheckWhetherCardIdIsNegated(Card.Id)) return false;
            bool haveLinkAnchor = Bot.HasInMonstersZone(CardId.Linguriboh) || Bot.HasInMonstersZone(CardId.SplashMage);
            if (!haveLinkAnchor) return false;
            if (Card.Location != CardLocation.Hand) return false;

            avoidLinkedZones = true;
            return true;
        }
        private bool OneBody_Backup_SearchWizard()
        {
            if (DefaultCheckWhetherCardIdIsNegated(Card.Id)) return false;
            if (!Card.IsCode(CardId.BackupIgnister)) return false;

            if (Bot.Hand.Count == 0) return false;

            bool haveWizard = Bot.HasInHand(CardId.WizardIgnister);

            int searchId = 0;
            if (haveWizard && CheckRemainInDeck(CardId.MalissP_MarchHare) > 0 && Bot.Hand.Count > 0)
                searchId = CardId.MalissP_MarchHare;
            else if (CheckRemainInDeck(CardId.MalissP_Dormouse) > 0 && nsplan && Bot.HasInMonstersZone(CardId.SALAMANGREAT_ALMIRAJ))
                searchId = CardId.MalissP_Dormouse;
            else if (CheckRemainInDeck(CardId.MalissP_WhiteRabbit) > 0 && nsBackupplan)
                searchId = CardId.MalissP_WhiteRabbit;
            else if (!haveWizard && CheckRemainInDeck(CardId.WizardIgnister) > 0 && Bot.Hand.Count > 0)
                searchId = CardId.WizardIgnister;
            else if (CheckRemainInDeck(CardId.MalissP_WhiteRabbit) > 0)
                searchId = CardId.MalissP_WhiteRabbit;
            else
                return false;

            AI.SelectCard(searchId);

            var hand = Bot.Hand.Where(h => h != null).ToList();
            var candidates = hand.Where(h => h.Id != CardId.WizardIgnister).ToList();
            var discards = new List<ClientCard>(hand.Count);
            if (searchId == CardId.MalissP_Dormouse)
            {
                discards.AddRange(candidates.Where(c => c.Id == CardId.MalissP_Dormouse));
            }
            Func<IEnumerable<ClientCard>> OthersExcludingTarget = () =>
            {
                if (searchId == CardId.MalissP_MarchHare)
                    return candidates.Where(c => c.Id != CardId.MalissP_MarchHare);
                return candidates;
            };
            var othersExcludingTarget = OthersExcludingTarget().ToList();
            var dupGroups = othersExcludingTarget.GroupBy(c => c.Id)
                                                 .Where(g => g.Count() >= 2);

            discards.AddRange(dupGroups.SelectMany(g => g));

            int[] lowValueSinglesOrder = {  CardId.NibiruThePrimalBeing,  
                                            CardId.Lancea,        
                                            CardId.TERRAFORMING,
                                            CardId.GoldSarcophagus
                                         };

            foreach (var id in lowValueSinglesOrder)
            {
                discards.AddRange(othersExcludingTarget
                    .Where(c => c.Id == id && dupGroups.All(g => g.Key != id)));
            }

            var already = new HashSet<ClientCard>(discards);
            discards.AddRange(othersExcludingTarget.Where(c => !already.Contains(c)));

            discards = discards.Where(c => c != null).Distinct().ToList();

            if (searchId == CardId.MalissP_Dormouse)
            {
                AI.SelectNextCard(searchId);
            }
            if (searchId == CardId.MalissP_WhiteRabbit)
            {
                AI.SelectNextCard(searchId);
            }
            else if (discards != null)
            {
                AI.SelectNextCard(discards);
            }
            else
            {
                return false;
            }

            avoidLinkedZones = true;
            if (Bot.HasInMonstersZone(CardId.Linguriboh)) blockWicckid = true;
            if (GetMMZCount() >= 5 && Bot.HasInHand(CardId.WizardIgnister)) { fullBoard1 = true; }
            return true;
        }
        private bool OneBody_Wizard_SS()
        {
            if (DefaultCheckWhetherCardIdIsNegated(Card.Id)) return false;
            if (!Bot.HasInMonstersZone(CardId.Linguriboh) && !Bot.HasInMonstersZone(CardId.SALAMANGREAT_ALMIRAJ)) return false;
            if (Card.Location != CardLocation.Hand) return false;

            var revive = PickGYCybersePriority();
            if (revive == null) return false;

            avoidLinkedZones = true;
            blockWicckid = true;

            AI.SelectCard(revive);
            return true;
        }
        private ClientCard PickGYCybersePriority()
        {
            var m = PickGYMalissPriority();
            if (m != null) return m;

            var list = Bot.Graveyard.GetMatchingCards(c =>
                c != null && c.IsMonster() && c.HasRace(CardRace.Cyberse) && c.Level <= 4).ToList();
            return list.FirstOrDefault();
        }
        private ClientCard PickGYMalissPriority()
        {
            int[] pref = {
                            CardId.MalissP_Dormouse,
                            CardId.MalissP_WhiteRabbit,
                            CardId.MalissP_ChessyCat,
                            CardId.MalissP_MarchHare
                        };
            foreach (var id in pref)
            {
                var c = Bot.Graveyard.GetFirstMatchingCard(g => g.IsCode(id) && g.IsMonster());
                if (c != null) return c;
            }
            return null;
        }
        private bool TwoCC_NormalSummon()
        {
            if (usedNormalSummon) return false;
            if (Bot.GetMonsterCount() != 0) return false;

            bool haveOtherStarter =
                Bot.HasInHand(CardId.MalissP_Dormouse) ||
                Bot.HasInHand(CardId.MalissP_WhiteRabbit) ||
                Bot.HasInHand(CardId.GoldSarcophagus) ||
                Bot.HasInHand(CardId.MalissInUnderground);

            if (haveOtherStarter) return false;

            if (Bot.Hand.GetMatchingCards(c => c != null && c.IsCode(CardId.MalissP_ChessyCat)).Count >= 2)
            {
                usedNormalSummon = true;
                return true;
            }
            return false;
        }
        private bool IsMalissCost(ClientCard card)
        {
            if (card == null) return false;

            if (card.IsCode(CardId.MalissP_ChessyCat))
                return true;
            if (card.IsCode(CardId.MalissP_WhiteRabbit) && NSDorMouse && !ssWhiteRabbit)
                return true;
            if (card.IsCode(CardId.MalissP_WhiteRabbit) && ssWhiteRabbit)
                return true;
            if (card.IsCode(CardId.MalissP_Dormouse) && ssDormouse)
                return true;
            if (card.IsCode(CardId.MalissP_MarchHare) && ActiveMarchHare)
                return true;

            return false;
        }
        private bool IsMalissCost2(ClientCard card)
        {
            if (card == null) return false;

            return
                card.IsCode(CardId.MalissP_ChessyCat) ||
                card.IsCode(CardId.MalissP_WhiteRabbit) ||
                card.IsCode(CardId.MalissP_MarchHare) ||
                card.IsCode(CardId.MalissInUnderground);
        }
        private bool AnyDraw()
        {
            if (DefaultCheckWhetherCardIdIsNegated(Card.Id)) return false;
            if (Card.Id != CardId.MalissP_ChessyCat) return false;
            if (Card.Location != CardLocation.MonsterZone) return false;
            if (CheckWhetherNegated()) return false;

            ClientCard target = null;

            target = Bot.Hand.FirstOrDefault(c => c != null && c.IsCode(CardId.MalissInTheMirror));
            if (target == null)
            {
                target = Bot.Hand.FirstOrDefault(IsMalissCost);
            }
            if (target == null)
            {
                var malissDupGroup = Bot.Hand
                    .Where(IsMalissCost2)
                    .GroupBy(c => c.Id)
                    .FirstOrDefault(g => g.Count() >= 2);

                if (malissDupGroup != null)
                {
                    target = malissDupGroup.First();
                }
            }
            if (target == null)
            {
                return false;
            }
            AI.SelectCard(target);
            return true;
        }
        private int PickTB11CostCandidateId()
        {
            var field = Bot.GetMonsters()
                .Where(c => c != null && c.IsFaceup() && c.HasSetcode(SetcodeMaliss))
                .ToList();

            field = field.Where(c =>
                !(c.IsCode(CardId.MalissP_WhiteRabbit) && ssWhiteRabbit) &&
                !(c.IsCode(CardId.MalissP_ChessyCat) && ssChessyCat) &&
                !(c.IsCode(CardId.MalissP_MarchHare) && ssMarchHare) &&
                !(c.IsCode(CardId.MalissP_Dormouse) && ssDormouse) &&
                !(c.IsCode(CardId.MalissQ_RedRansom) && ssRRThisTurn) &&
                !(c.IsCode(CardId.MalissQ_WhiteBinder) && ssWBThisTurn) &&
                !(c.IsCode(CardId.MalissQ_HeartsCrypter) && ssHCThisTurn)
            ).ToList();

            int[] pref = {
                                CardId.MalissP_WhiteRabbit,
                                CardId.MalissP_ChessyCat,
                                CardId.MalissP_MarchHare,
                                CardId.MalissP_Dormouse,
                                CardId.MalissQ_RedRansom,
                                CardId.MalissQ_WhiteBinder,
                                CardId.MalissQ_HeartsCrypter
                            };
            foreach (var id in pref)
                if (field.Any(c => c.IsCode(id))) return id;

            return 0;
        }
        private int PickPFromGYForSplash()
        {
            if (enemyActivateLancea)
            {
                int[] pref2 = {
                            CardId.MalissP_Dormouse,
                            CardId.MalissP_WhiteRabbit,
                            CardId.MalissP_ChessyCat,
                            CardId.MalissP_MarchHare,
                            CardId.BackupIgnister,
                            CardId.WizardIgnister
                        };
                foreach (var id in pref2)
                    if (Bot.HasInGraveyard(id)) return id;
            }
            int[] pref = {
                            CardId.MalissP_Dormouse,
                            CardId.MalissP_WhiteRabbit,
                            CardId.MalissP_ChessyCat,
                            CardId.MalissP_MarchHare
                        };
            foreach (var id in pref)
                if (Bot.HasInGraveyard(id)) return id;
            return 0;
        }
        private bool HaveUndergroundOnHandOrField()
        {
            return Bot.HasInHand(CardId.MalissInUnderground) || Bot.HasInSpellZone(CardId.MalissInUnderground);
        }
        private bool ShouldSearchUnderground()
        {
            if (ActiveUnderground) return false;
            if (HaveUndergroundOnHandOrField()) return false;
            return CheckRemainInDeck(CardId.MalissInUnderground) > 0;
        }
        private bool Step2_RedRansom_Search()
        {
            if (DefaultCheckWhetherCardIdIsNegated(Card.Id)) return false;
            int chooseId = 0;
            bool shouldUG = ShouldSearchUnderground();
            if (shouldUG)
            {
                chooseId = CardId.MalissInUnderground;
            }
            else if (CheckRemainInDeck(CardId.MalissInTheMirror) > 0)
            {
                chooseId = CardId.MalissInTheMirror;
            }
            if (chooseId == 0) return false;

            //AI.SelectCard(chooseId); go to onselectcard
            step2Done = true;
            avoidLinkedZones = false;
            coreSetupComplete = true;
            return true;
        }
        private List<ClientCard> PickMaterialsForRedRansom()
        {
            var splash = Bot.MonsterZone.FirstOrDefault(c => c != null && c.IsFaceup() && c.IsCode(CardId.SplashMage));
            if (splash == null) return new List<ClientCard>();

            ClientCard revived = null;
            if (lastRevivedIdBySplash != 0)
                revived = Bot.MonsterZone.FirstOrDefault(c => c != null && c.IsFaceup() && c.IsCode(lastRevivedIdBySplash));

            if (revived == null)
                revived = Bot.MonsterZone.FirstOrDefault(c =>
                    c != null && c.IsFaceup() && c.HasSetcode(0x1bf) && c != splash && !c.HasType(CardType.Link));

            if (revived == null) return new List<ClientCard>();
            return new List<ClientCard> { splash, revived };
        }
        private bool Step2_SplashMage_ReviveP()
        {
            if (DefaultCheckWhetherCardIdIsNegated(Card.Id)) return false;
            if (step2Done) return false;

            if (!Bot.HasInMonstersZone(CardId.SplashMage)) return false;

            int pick = PickPFromGYForSplash();
            if (pick == 0) return false;

            AI.SelectCard(pick);
            lastRevivedIdBySplash = pick;
            return true;
        }
        private bool Step2N_SplashMage_ReviveP()
        {
            if (DefaultCheckWhetherCardIdIsNegated(Card.Id)) return false;
            if (!Bot.HasInMonstersZone(CardId.SplashMage)) return false;

            int pick = PickPFromGYForSplash();
            if (pick == 0) return false;

            AI.SelectCard(pick);
            lastRevivedIdBySplash = pick;
            return true;
        }
        private bool Step2_LinkSummon_RedRansom()
        {
            if (step2Done) return false;

            var mats = PickMaterialsForRedRansom();
            if (mats.Count != 2) return false;

            if (Util.GetBotAvailZonesFromExtraDeck(mats) == 0) return false;

            AI.SelectMaterials(mats);
            madeIt3 = true;
            return true;
        }
        private bool Step2N_LinkSummon_WB()
        {
            var mats = PickMaterialsForRedRansom();
            if (mats.Count != 2) return false;

            if (Util.GetBotAvailZonesFromExtraDeck(mats) == 0) return false;

            AI.SelectMaterials(mats);
            return true;
        }
        private bool Step2N_LinkSummon_RedRansom()
        {
            if (!Bot.HasInMonstersZone(CardId.MalissP_MarchHare) || Bot.GetMonsterCount() < 3)
            {
                return false;
            }
            madeIt3 = true;
            return true;
        }
        private bool Step2_Fallback_Wizard_AfterSplashNegated()
        {
            if (DefaultCheckWhetherCardIdIsNegated(Card.Id)) return false;
            if (!splashNegatedThisTurn) return false;
            if (Card.Location != CardLocation.Hand) return false;

            var revive = PickGYMalissPriority();
            if (revive == null) return false;

            avoidLinkedZones = true;
            blockWicckid = true;
            AI.SelectCard(revive);
            return true;
        }
        private bool Step2_Fallback_Backup_AfterSplashNegated()
        {
            if (DefaultCheckWhetherCardIdIsNegated(Card.Id)) return false;
            if (!splashNegatedThisTurn) return false;
            if (Card.Location != CardLocation.Hand) return false;

            if (GetMMZCount() >= 5) return false;

            int want = !Bot.HasInHand(CardId.WizardIgnister) && CheckRemainInDeck(CardId.WizardIgnister) > 0
                       ? CardId.WizardIgnister
                       : (CheckRemainInDeck(CardId.MalissP_MarchHare) > 0 ? CardId.MalissP_MarchHare : 0);
            if (want == 0) return false;

            AI.SelectCard(want);

            var discard = Bot.Hand.FirstOrDefault(h => h != null && h.Id != CardId.WizardIgnister && h != Card);
            if (discard != null) AI.SelectNextCard(discard);

            avoidLinkedZones = true;
            blockWicckid = true;
            return true;
        }
        #endregion

        #region FLOW 3: Core → Results (UG / Mirror / TP11 → Make it 3!! → Finishers)
        private bool HaveUG() => Bot.HasInHand(CardId.MalissInUnderground) || Bot.HasInSpellZone(CardId.MalissInUnderground);
        private bool HasSelfSSAvailable(int id)
        {
            if (id == CardId.MalissP_Dormouse) return !ssDormouse;
            if (id == CardId.MalissP_WhiteRabbit) return !ssWhiteRabbit;
            if (id == CardId.MalissP_ChessyCat) return !ssChessyCat;
            if (id == CardId.MalissP_MarchHare) return !ActiveMarchHare;
            return true;
        }
        private int PickUG_DHG_DormouseFirst()
        {
            int[] pref = {
                            CardId.MalissP_Dormouse,
                            CardId.MalissP_WhiteRabbit,
                            CardId.MalissP_ChessyCat,
                            CardId.MalissP_MarchHare
                        };

            foreach (var id in pref)
            {
                if (!HasSelfSSAvailable(id)) continue;
                if (ExistsForUnderground(id))
                    return id;
            }
            return 0;
        }

        private bool Flow3_UnderGround_Available_SSAnyPawn()
        {
            if (!step2Done) return false;
            if (!HaveUG()) return false;
            if (Card.Id != CardId.MalissInUnderground) return false;
            if (resultSuccessFlag) return false;
            if (GetMMZCount() >= 5)
            {
                AI.SelectYesNo(false);
                return true;
            }
            if (enemyActivateLancea)
            {
                AI.SelectYesNo(false);
                return true;
            }

            int pick = 0;

            if (CheckRemainInDeck(CardId.MalissInTheMirror) > 0 && nsplan)
            {
                pick = CardId.MalissInTheMirror;
            }
            else if (CheckRemainInDeck(CardId.MalissInTheMirror) > 0 && NSDorMouse)
            {
                pick = CardId.MalissInTheMirror;
            }
            else
            {
                pick = PickUG_DHG_DormouseFirst();
            }

            if (pick == 0)
            {
                AI.SelectYesNo(false);
                return true;
            }

            AI.SelectYesNo(true);
            AI.SelectCard(pick);
            resultSuccessFlag = true;
            return true;
        }
        private bool Mirror_Banish()
        {
            if (DescIs(CardId.MalissInTheMirror, 1))
            {
                var gy = PickMirrorGYTargetForSearch();
                if (gy == null) return false;

                AI.SelectCard(gy);

                var wants = Mirror_SearchOrderForType(false, gy.IsMonster());
                AI.SelectNextCard(wants);
                return true;
            }

            if (CheckSpellWillBeNegate()) return false;
            if (CheckWhetherNegated()) return false;
            var cost = PickMirrorCostCandidate();
            if (cost == null) return false;
            foreach (ClientCard m in Enemy.GetMonsters())
            {
                if (m.IsMonsterShouldBeDisabledBeforeItUseEffect() && !m.IsDisabled() && Duel.LastChainPlayer != 0)
                {
                    if (Card.Location == CardLocation.Hand)
                    {
                        SelectSTPlace(Card, true);
                    }
                    AI.SelectCard(m);
                    AI.SelectNextCard(cost);
                    return true;
                }
            }
            ClientCard LastChainCard = Util.GetLastChainCard();
            if ((LastChainCard == null || LastChainCard.Controller != 1 || LastChainCard.Location != CardLocation.MonsterZone
                || LastChainCard.IsDisabled() || LastChainCard.IsShouldNotBeTarget() || LastChainCard.IsShouldNotBeSpellTrapTarget()))
                return false;

            if (Card.Location == CardLocation.Hand)
            {
                SelectSTPlace(Card, true);
            }
            if (LastChainCard != null) AI.SelectCard(LastChainCard);
            else
            {
                List<ClientCard> enemyMonsters = Enemy.GetMonsters();
                enemyMonsters.Sort(CardContainer.CompareCardAttack);
                enemyMonsters.Reverse();
                foreach (ClientCard card in enemyMonsters)
                {
                    if (card.IsFaceup() && !card.IsShouldNotBeTarget() && !card.IsShouldNotBeSpellTrapTarget())
                    {
                        AI.SelectCard(card);
                    }
                }
            }
            AI.SelectNextCard(cost);
            return true;
        }
        private bool Flow3_Link_Accesscode()
        {
            if (Bot.HasInExtra(CardId.AlliedCodeTalkerIgnister)) return false;
            if (BlockAccesscodeOnT1()) return false;

            var avoid = new[] { CardId.AlliedCodeTalkerIgnister, CardId.FirewallDragon };

            var mats = PickLinkMatsMinCount(
                                                targetLink: 4,
                                                isEligible: m => m.HasType(CardType.Effect),
                                                minCount: 2,
                                                maxCount: 4,
                                                avoidIds: avoid
                                            );

            if (mats.Count < 2) return false;
            AI.SelectMaterials(mats);
            return true;
        }
        private bool Flow3_BackupIgnister_AfterMakeIt3()
        {
            if (DefaultCheckWhetherCardIdIsNegated(Card.Id)) return false;
            if (!madeIt3) return false;
            if (Card.Location != CardLocation.Hand) return false;

            avoidLinkedZones = true;
            return true;
        }
        private bool Flow3_WizardIgnister_AfterMakeIt3()
        {
            if (DefaultCheckWhetherCardIdIsNegated(Card.Id)) return false;
            if (!madeIt3) return false;
            if (Card.Location != CardLocation.Hand) return false;

            var revive = PickGYMalissPriority();
            if (revive == null) return false;

            avoidLinkedZones = true;
            AI.SelectCard(revive);
            return true;
        }
        private bool RR_SS_FromBanished()
        {
            if (DefaultCheckWhetherCardIdIsNegated(Card.Id)) return false;
            if (Card.Id != CardId.MalissQ_RedRansom) return false;
            if (Card.Location != CardLocation.Removed) return false;
            if (GetMMZCount() >= 5) return false;
            if (Bot.LifePoints <= 900)
            {
                return false;
            }

            int pickId = 0;

            int[] pawnOrder = {
                                        CardId.MalissP_ChessyCat,
                                        CardId.MalissP_MarchHare,
                                        CardId.MalissP_WhiteRabbit,
                                        CardId.MalissP_Dormouse
                               };

            foreach (var id in pawnOrder)
            {
                if (CheckRemainInDeck(id) > 0 && PawnSelfSS_AvailableId(id))
                {
                    pickId = id;
                    break;
                }
            }


            bool canBanish = (pickId != 0);
            AI.SelectYesNo(canBanish);
            if (canBanish)
            {
                AI.SelectCard(pickId);
            }
            _rrSelfSSPlacing = true;
            ssRRThisTurn = true;
            return true;
        }
        private bool Wicckid_SearchTuner()
        {
            if (DefaultCheckWhetherCardIdIsNegated(Card.Id)) return false;
            if (CheckRemainInDeck(CardId.BackupIgnister) <= 0) return false;
            var cost = PickGYCyberseForWicckidCost_Safe();
            if (cost == null)
            {
                return false;
            }

            AI.SelectCard(cost);
            AI.SelectNextCard(CardId.BackupIgnister);
            avoidLinkedZones = false;
            return true;
        }
        private bool LinkDecoder_ReviveFromGY()
        {
            if (DefaultCheckWhetherCardIdIsNegated(Card.Id)) return false;
            if (Card.Location != CardLocation.Grave) { return false; }
            if (Allied_End) { return false; }
            return true;
        }
        private bool Transcode_ReviveLink3OrLower()
        {
            if (DefaultCheckWhetherCardIdIsNegated(Card.Id)) return false;
            if (Card.Location != CardLocation.MonsterZone) return false;

            var prefer = Bot.Graveyard.GetFirstMatchingCard(c => c.IsCode(CardId.CyberseWicckid))
                      ?? Bot.Graveyard.GetFirstMatchingCard(c => c.IsCode(CardId.SplashMage))
                      ?? Bot.Graveyard.GetFirstMatchingCard(c => c.IsCode(CardId.MalissQ_RedRansom))
                      ?? Bot.Graveyard.GetFirstMatchingCard(c => c.IsCode(CardId.MalissQ_WhiteBinder))
                      ?? Bot.Graveyard
                            .GetMatchingCards(c => c.IsMonster() && c.HasType(CardType.Link) && c.LinkCount <= 3 && !c.IsCode(CardId.TranscodeTalker))
                            .OrderByDescending(c => c.Attack).FirstOrDefault();

            if (prefer == null) return false;
            AI.SelectCard(prefer);
            avoidLinkedZones = false;
            return true;
        }
        private bool Allied_NegateBanish()
        {
            if (DefaultCheckWhetherCardIdIsNegated(Card.Id)) return false;
            if (CheckWhetherNegated(true) || !CheckLastChainShouldNegated()) return false;
            var allied = Bot.MonsterZone.GetFirstMatchingCard(m => m != null && m.IsCode(CardId.AlliedCodeTalkerIgnister));
            if (allied == null || allied.IsDisabled()) return false;
            bool haveAnyLink = Bot.GetMonsters().Any(m => m != null && m.HasType(CardType.Link) && !m.IsCode(CardId.AlliedCodeTalkerIgnister));
            if (!haveAnyLink) return false;

            var cheapLinks = new List<int> {
                                                CardId.CyberseWicckid,
                                                CardId.MalissQ_WhiteBinder,
                                                CardId.TranscodeTalker,
                                                CardId.MalissQ_RedRansom
                                            };
            AI.SelectCard(cheapLinks.ToArray());
            return true;
        }
        private ClientCard GetWicckid()
        {
            return Bot.MonsterZone.GetFirstMatchingFaceupCard(c => c != null && c.IsCode(CardId.CyberseWicckid));
        }
        private int GetLinkedMaskFor(ClientCard link)
        {
            if (link == null || !link.IsFaceup() || !link.HasType(CardType.Link)) return 0;
            return link.GetLinkedZones() & 0x1F;
        }
        private bool PawnSelfSS_AvailableId(int id)
        {
            if (id == CardId.MalissP_Dormouse) return !ssDormouse;
            if (id == CardId.MalissP_WhiteRabbit) return !ssWhiteRabbit;
            if (id == CardId.MalissP_ChessyCat) return !ssChessyCat;
            if (id == CardId.MalissP_MarchHare) return !ActiveMarchHare;
            return false;
        }
        private bool QueenSelfSS_AvailableId(int id)
        {
            if (id == CardId.MalissQ_HeartsCrypter) return !ssHCThisTurn;
            if (id == CardId.MalissQ_WhiteBinder) return !ssWBThisTurn;
            if (id == CardId.MalissQ_RedRansom) return !ssRRThisTurn;
            return false;
        }
        private bool Dormouse_Banish_Anytime()
        {
            if (DefaultCheckWhetherCardIdIsNegated(Card.Id)) return false;
            if (Card.Id != CardId.MalissP_Dormouse) return false;
            if (Card.Location != CardLocation.MonsterZone) return false;
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            if (enemyActivateLancea) return false;
            if (!HasFreeMMZ()) return false;

            int pick = 0;

            if (CheckRemainInDeck(CardId.MalissP_WhiteRabbit) > 0 && PawnSelfSS_AvailableId(CardId.MalissP_WhiteRabbit))
                pick = CardId.MalissP_WhiteRabbit;
            else if (CheckRemainInDeck(CardId.MalissP_ChessyCat) > 0 && PawnSelfSS_AvailableId(CardId.MalissP_ChessyCat))
                pick = CardId.MalissP_ChessyCat;

            if (pick == 0) return false;

            AI.SelectCard(pick);
            return true;
        }
        private bool IsCyberse(ClientCard c) => c != null && c.HasType(CardType.Monster) && c.HasRace(CardRace.Cyberse);
        private bool RR_HOPT_Spent_ThisTurn() => ssRRThisTurn;
        private bool RR_CanStillSS_ThisTurn()
        {
            if (RR_HOPT_Spent_ThisTurn()) return false;
            return true;
        }
        private int Score_WicckidCost(ClientCard c)
        {
            if (c == null) return int.MinValue;
            if (c.IsCode(CardId.MalissQ_RedRansom) && RR_HOPT_Spent_ThisTurn()) return -999;
            if (c.IsCode(CardId.MalissQ_RedRansom) && RR_CanStillSS_ThisTurn()) return 1000;

            if (c.IsCode(CardId.Linguriboh) || c.IsCode(CardId.SALAMANGREAT_ALMIRAJ)) return 120;

            if (c.IsCode(CardId.MalissP_Dormouse) && ssDormouse) return 90;
            if (c.IsCode(CardId.MalissP_WhiteRabbit) && ssWhiteRabbit) return 70;
            if (c.IsCode(CardId.MalissP_ChessyCat) && ssChessyCat) return 60;
            if (c.IsCode(CardId.MalissP_MarchHare) && ssMarchHare) return 60;

            if (c.IsCode(CardId.BackupIgnister)) return 20;
            if (c.IsCode(CardId.WizardIgnister)) return 20;
            return 30;
        }
        private ClientCard PickGYCyberseForWicckidCost_Safe()
        {
            var gy = (Bot.Graveyard ?? new List<ClientCard>())
                     .Where(IsCyberse)
                     .ToList();
            if (gy.Count == 0) return null;

            ClientCard best = null;
            int bestScore = int.MinValue;
            foreach (var c in gy)
            {
                int sc = Score_WicckidCost(c);
                if (sc > bestScore) { best = c; bestScore = sc; }
            }
            return best;
        }
        private bool BlockAccesscodeOnT1()
        {
            bool blocked = (Duel.Player == 0 && Duel.Turn == 1);
            return blocked;
        }
        private bool Allied_OnSummonTrigger()
        {
            if (DefaultCheckWhetherCardIdIsNegated(Card.Id)) return false;
            var me = Bot.MonsterZone.GetFirstMatchingCard(m => m != null && m.IsCode(CardId.AlliedCodeTalkerIgnister));
            if (me == null) return false;

            bool has2300 = Bot.Graveyard.Any(c => c != null && c.IsMonster() && c.HasRace(CardRace.Cyberse) && c.Attack == 2300);
            if (!has2300) return false;

            var prefer = new List<int> {
                                            CardId.MalissQ_RedRansom,
                                            CardId.MalissQ_WhiteBinder,
                                            CardId.TranscodeTalker
                                        };
            AI.SelectCard(prefer.ToArray());
            return true;
        }
        private bool ShouldSummonTranscode()
        {
            bool haveSplashGY = Bot.HasInGraveyard(CardId.SplashMage);
            bool haveRR = Bot.HasInMonstersZone(CardId.MalissQ_RedRansom);
            bool haveL1 = Bot.HasInMonstersZone(CardId.LinkDecoder) || Bot.HasInGraveyard(CardId.LinkDecoder);

            return haveSplashGY && (haveRR || haveL1);
        }
        private bool Step_SplashToRR()
        {
            if (_didSplashToRR) return false;
            if (Bot.HasInMonstersZone(CardId.LinkDecoder)) return false;

            if (!Bot.HasInExtra(CardId.MalissQ_RedRansom)) return false;

            var mats = PickLinkMatsMinCount(
                targetLink: 2,
                isEligible: m => m.HasType(CardType.Effect),
                minCount: 2,
                maxCount: 2,
                avoidIds: new[] { CardId.CyberseWicckid }
            );
            if (mats.Count == 0)
            {
                return false;
            }

            AI.SelectMaterials(mats);

            _didSplashToRR = true;
            return true;
        }
        private bool Step_SplashToWB()
        {
            if (_didSplashToRR) return false;

            if (!Bot.HasInExtra(CardId.MalissQ_WhiteBinder)) return false;

            var mats = PickLinkMatsMinCount(
                targetLink: 2,
                isEligible: m => m.HasType(CardType.Effect),
                minCount: 2,
                maxCount: 2,
                avoidIds: new[] { CardId.CyberseWicckid }
            );
            if (mats.Count == 0)
            {
                return false;
            }

            AI.SelectMaterials(mats);

            _didSplashToRR = true;
            return true;
        }
        private bool Step_RRtoWicckid()
        {
            if(enemyActivateLancea) return false;
            if (blockWicckid) return true;
            if (!_didSplashToRR || _didRRtoWicckid) return false;
            if (!Bot.HasInExtra(CardId.CyberseWicckid)) return false;

            var rr = Bot.MonsterZone.GetFirstMatchingCard(m => m != null && m.IsCode(CardId.MalissQ_RedRansom));
            if (rr == null) return false;
            var emzOccupant = Bot.GetMonsters().FirstOrDefault(m => m != null && m != rr && IsInEMZ(m));
            var buddy = emzOccupant ?? Bot.MonsterZone.FirstOrDefault(m => m != null && m != rr);
            if (buddy == null) return false;
            bool emzWillBeFree = HasFreeEMZ() || IsInEMZ(rr) || IsInEMZ(buddy);
            if (!emzWillBeFree) return false;
            AI.SelectCard(CardId.CyberseWicckid);
            AI.SelectMaterials(new List<ClientCard> { rr, buddy });
            wantLinkedToWicckid = true;
            _preferWicckidArrows = true;
            _didRRtoWicckid = true;
            return true;
        }
        private bool Step2N_RRtoWicckid()
        {
            if (enemyActivateLancea) return false;
            if (blockWicckid) return false;
            var rr = Bot.MonsterZone.GetFirstMatchingCard(m => m != null && m.IsCode(CardId.MalissQ_RedRansom));
            if (rr == null) return false;
            var emzOccupant = Bot.GetMonsters().FirstOrDefault(m => m != null && m != rr && IsInEMZ(m));
            var buddy = emzOccupant ?? Bot.MonsterZone.FirstOrDefault(m => m != null && m != rr);
            if (buddy == null) return false;
            bool emzWillBeFree = HasFreeEMZ() || IsInEMZ(rr) || IsInEMZ(buddy);
            if (!emzWillBeFree) return false;
            AI.SelectCard(CardId.CyberseWicckid);
            AI.SelectMaterials(new List<ClientCard> { rr, buddy });
            wantLinkedToWicckid = true;
            _preferWicckidArrows = true;
            _didRRtoWicckid = true;
            return true;
        }
        private bool Step_SummonLinkDecoderToWicckid()
        {
            if (!_didRRtoWicckid || _didSummonToWicckidArrow) return false;

            var wic = Bot.MonsterZone.GetFirstMatchingCard(m => m != null && m.IsCode(CardId.CyberseWicckid));
            if (wic == null) return false;

            _preferWicckidArrows = true;
            avoidLinkedZones = false;

            return true;
        }
        private bool Step1_SSLinkDecoder()
        {
            if ((Bot.HasInMonstersZone(CardId.MalissP_Dormouse) ||
                Bot.HasInMonstersZone(CardId.MalissP_WhiteRabbit) ||
                Bot.HasInMonstersZone(CardId.MalissP_ChessyCat) ||
                Bot.HasInMonstersZone(CardId.BackupIgnister)) && Bot.GetMonsterCount() == 1 && Bot.HasInHand(CardId.MalissP_MarchHare))
            {
                return true;
            }
            return false;
        }
        private bool IsMalissMonster(ClientCard c) => c != null && c.IsMonster() && c.HasSetcode(0x1bf);
        private bool CanMakeLinkNWithFlexibleTwo(ClientCard a, ClientCard b, int target)
        {
            int a1 = 1, aL = (a != null && a.HasType(CardType.Link)) ? Math.Max(1, a.LinkCount) : 1;
            int b1 = 1, bL = (b != null && b.HasType(CardType.Link)) ? Math.Max(1, b.LinkCount) : 1;
            return (a1 + b1 == target) || (a1 + bL == target) || (aL + b1 == target) || (aL + bL == target);
        }
        private bool Step_WicckidPlusOneToWB()
        {
            const int WB_LINK = 3;
            if (!Bot.HasInExtra(CardId.MalissQ_WhiteBinder)) return false;

            var wic = GetWicckid();
            if (wic != null)
            {
                var partners = Bot.GetMonsters()
                    .Where(m => m != null && m.IsFaceup() && m != wic && m.HasType(CardType.Effect))
                    .ToList();

                Func<ClientCard, bool> okPair = p => (IsMalissMonster(p) || IsMalissMonster(wic)) && CanMakeLinkNWithFlexibleTwo(wic, p, WB_LINK);

                var p2 = partners.FirstOrDefault(p => !p.IsCode(CardId.MalissQ_RedRansom) && okPair(p))
                                                      ?? partners.FirstOrDefault(p => p.IsCode(CardId.MalissQ_RedRansom) && okPair(p));

                if (p2 != null)
                {
                    AI.SelectMaterials(new List<ClientCard> { wic, p2 });
                    _didWBFromWicckid = true;
                    EnsureFinishPlanAfterWB();
                    return true;
                }
            }

            var mats = PickLinkMatsMinCount(
                                                targetLink: WB_LINK,
                                                isEligible: m => m.HasType(CardType.Effect),
                                                minCount: 2,
                                                maxCount: 3,
                                                avoidIds: new[] { CardId.Apollousa, CardId.FirewallDragon, CardId.AlliedCodeTalkerIgnister },
                                                requireMaliss: true
                                            );
            if (mats.Count == 0) return false;

            AI.SelectMaterials(mats);
            _didWBFromWicckid = true;
            EnsureFinishPlanAfterWB();
            return true;
        }
        private void EnsureFinishPlanAfterWB()
        {
            if (_finishPlanDecided) return;

            int free = Bot.MonsterZone.Count(m => m != null && !m.IsCode(CardId.MalissQ_RedRansom));
            bool canFW = Bot.HasInExtra(CardId.FirewallDragon);
            bool canHC = Bot.HasInExtra(CardId.MalissQ_HeartsCrypter);
            bool canAll = Bot.HasInExtra(CardId.AlliedCodeTalkerIgnister);
            bool reachAllied = CanReachAlliedNow();

            if (canFW && canHC && canAll && free >= 3 && reachAllied)
                _finishPlan = FinishPlan.FW_HC_Allied;
            else if (canHC && canAll && free >= 2 && reachAllied)
                _finishPlan = FinishPlan.HC_Allied;
            else
                _finishPlan = FinishPlan.AlliedOnly;

            _finishPlanDecided = true;
        }
        private bool ssFromHandMH()
        {
            if (DefaultCheckWhetherCardIdIsNegated(Card.Id)) return false;
            if (Card.Location != CardLocation.Hand) return false;
            if (GetMMZCount() > 3) return false;
            if (enemyActivateLancea) return false;

            if (Duel.Player == 0)
            {
                var gy = Bot.Graveyard.GetMatchingCards(c => c != null && c.HasSetcode(0x1bf) && c != Card).ToList();

                Func<IEnumerable<ClientCard>, ClientCard> pickP1 = (src) =>
                {
                    int[] pref = {
                                    CardId.MalissQ_WhiteBinder,
                                    CardId.MalissQ_HeartsCrypter,
                                    CardId.MalissQ_RedRansom
                                 };
                    foreach (var id in pref)
                    {
                        var c = src.FirstOrDefault(x => x.HasType(CardType.Monster) && x.Id == id && QueenSelfSS_AvailableId(x.Id));
                        if (c != null) return c;
                    }
                    return src.FirstOrDefault(x => x.HasType(CardType.Monster) && QueenSelfSS_AvailableId(x.Id));
                };
                ClientCard pick = pickP1(gy);
                if (pick == null) return false;
                AI.SelectCard(pick);
                ssMarchHare = true;
                return DontSelfNG();
            }
            else
            {
                if (Bot.HasInGraveyard(CardId.MalissQ_WhiteBinder) && !ssWBThisTurn)
                {
                    var Target = Bot.Graveyard.GetFirstMatchingCard(c => c != null && c.IsCode(CardId.MalissQ_WhiteBinder));
                    if (Target == null) return false;
                    AI.SelectCard(Target);
                }
                else if (Bot.HasInGraveyard(CardId.MalissQ_HeartsCrypter) && !ssHCThisTurn)
                {
                    var Target = Bot.Graveyard.GetFirstMatchingCard(c => c != null && c.IsCode(CardId.MalissQ_HeartsCrypter));
                    if (Target == null) return false;
                    AI.SelectCard(Target);
                }
                else if (Bot.HasInGraveyard(CardId.MalissQ_RedRansom) && !ssRRThisTurn)
                {
                    var Target = Bot.Graveyard.GetFirstMatchingCard(c => c != null && c.IsCode(CardId.MalissQ_RedRansom));
                    if (Target == null) return false;
                    AI.SelectCard(Target);
                }
                else
                {
                    return false;
                }

                ssMarchHare = true;
                return DontSelfNG();
            }
        }
        private bool Step1_MH_FromHand()
        {
            if (DefaultCheckWhetherCardIdIsNegated(Card.Id)) return false;
            if (Card.Location != CardLocation.Hand) return false;
            if (Bot.GetMonsterCount() > 1) return false;
            if (enemyActivateLancea) return false;

            if (Bot.HasInMonstersZone(CardId.MalissP_WhiteRabbit) && Bot.HasInGraveyard(CardId.MalissC_MTP07))
            {
                var target = Bot.Graveyard.GetFirstMatchingCard(c => c != null && c.IsCode(CardId.MalissC_MTP07));
                if (target == null) return false;
                AI.SelectCard(target);
                ssMarchHare = true;
                return DontSelfNG();
            }
            if (Bot.HasInMonstersZone(CardId.LinkDecoder) && Bot.HasInGraveyard(CardId.MalissP_Dormouse))
            {
                var target = Bot.Graveyard.GetFirstMatchingCard(c => c != null && c.IsCode(CardId.MalissP_Dormouse));
                if (target == null) return false;
                AI.SelectCard(target);
                ssMarchHare = true;
                return DontSelfNG();
            }
            if (Bot.HasInMonstersZone(CardId.LinkDecoder) && Bot.HasInGraveyard(CardId.MalissP_WhiteRabbit))
            {
                var target = Bot.Graveyard.GetFirstMatchingCard(c => c != null && c.IsCode(CardId.MalissP_WhiteRabbit));
                if (target == null) return false;
                AI.SelectCard(target);
                ssMarchHare = true;
                return DontSelfNG();
            }
            if (Bot.HasInMonstersZone(CardId.LinkDecoder) && Bot.HasInGraveyard(CardId.MalissP_ChessyCat))
            {
                var target = Bot.Graveyard.GetFirstMatchingCard(c => c != null && c.IsCode(CardId.MalissP_ChessyCat));
                if (target == null) return false;
                AI.SelectCard(target);
                ssMarchHare = true;
                return DontSelfNG();
            }
            if (Bot.HasInMonstersZone(CardId.Linguriboh) && Bot.HasInGraveyard(CardId.MalissP_MarchHare))
            {
                var target = Bot.Graveyard.GetFirstMatchingCard(c => c != null && c.IsCode(CardId.MalissP_MarchHare));
                if (target == null) return false;
                AI.SelectCard(target);
                ssMarchHare = true;
                return DontSelfNG();
            }

            return false;
        }
        private bool returnFromBanish()
        {
            if (DefaultCheckWhetherCardIdIsNegated(Card.Id)) return false;
            if (Card.Location != CardLocation.Removed) return false;

            var mh = Bot.Banished.GetFirstMatchingCard(
                c => c.IsFaceup() && c.IsCode(CardId.MalissP_MarchHare));

            if (mh == null) return false;

            AI.SelectCard(mh);
            ActiveMarchHare = true;
            return true;
        }
        private bool WB_OnSummon_BanishGY()
        {
            if (DefaultCheckWhetherCardIdIsNegated(Card.Id)) return false;
            if (Card.Location != CardLocation.MonsterZone) return false;

            const int MAX_PICKS = 3;
            var picks = new List<ClientCard>();

            if (Duel.Player == 0)
            {
                int freeMMZ = Math.Max(0, 5 - GetMMZCount());
                int myPawnCount = Bot.Graveyard
                                     .GetMatchingCards(g => IsMalissPawn(g))
                                     .Count;
                bool canUseOwnPawn = myPawnCount > 1;

                if (!ActiveMarchHare && canUseOwnPawn)
                {
                    var mh = Bot.Graveyard.GetFirstMatchingCard(g => g.IsCode(CardId.MalissP_MarchHare));
                    if (mh != null) picks.Add(mh);
                }

                if (freeMMZ > 0 && !ssRRThisTurn && !Allied_End)
                {
                    var rr = Bot.Graveyard.GetFirstMatchingCard(g => g.IsCode(CardId.MalissQ_RedRansom));
                    if (rr != null && !picks.Contains(rr) && !ShouldSkipBanishing(rr))
                    { picks.Add(rr); freeMMZ--; }
                }

                if (freeMMZ > 0 && !ssHCThisTurn && !Allied_End)
                {
                    var hc = Bot.Graveyard.GetFirstMatchingCard(g => g.IsCode(CardId.MalissQ_HeartsCrypter));
                    if (hc != null && !picks.Contains(hc) && !ShouldSkipBanishing(hc))
                    { picks.Add(hc); freeMMZ--; }
                }

                if (freeMMZ > 0 && !ssWhiteRabbit && !Allied_End && canUseOwnPawn)
                {
                    var wr = Bot.Graveyard.GetFirstMatchingCard(g => g.IsCode(CardId.MalissP_WhiteRabbit));
                    if (wr != null && !picks.Contains(wr) && !ShouldSkipBanishing(wr))
                    { picks.Add(wr); freeMMZ--; }
                }

                if (freeMMZ > 0 && !ssDormouse && !Allied_End && canUseOwnPawn)
                {
                    var dm = Bot.Graveyard.GetFirstMatchingCard(g => g.IsCode(CardId.MalissP_Dormouse));
                    if (dm != null && !picks.Contains(dm) && !ShouldSkipBanishing(dm))
                    { picks.Add(dm); freeMMZ--; }
                }

                if (freeMMZ > 0 && !ssChessyCat && !Allied_End && canUseOwnPawn)
                {
                    var cc = Bot.Graveyard.GetFirstMatchingCard(g => g.IsCode(CardId.MalissP_ChessyCat));
                    if (cc != null && !picks.Contains(cc) && !ShouldSkipBanishing(cc))
                    { picks.Add(cc); freeMMZ--; }
                }

                if (freeMMZ >= 0 && canUseOwnPawn)
                {
                    var target = Bot.Graveyard.GetFirstMatchingCard(g => g.IsCode(CardId.MalissP_ChessyCat) || g.IsCode(CardId.MalissP_Dormouse));
                    picks.Add(target);
                }

                picks = picks.Where(c => c != null).Distinct().Take(MAX_PICKS).ToList();

                var threats = PickEnemyGYThreats(MAX_PICKS - picks.Count);
                foreach (var t in threats)
                {
                    if (picks.Count >= MAX_PICKS) break;
                    if (!picks.Contains(t)) picks.Add(t);
                }
            }
            else
            {
                int freeMMZ = Math.Max(0, 5 - GetMMZCount());

                if (!Bot.HasInSpellZone(CardId.MalissC_MTP07) && freeMMZ > 0 && !ssWhiteRabbit)
                {
                    var wr = Bot.Graveyard.GetFirstMatchingCard(g => g.IsCode(CardId.MalissP_WhiteRabbit));
                    if (wr != null) { picks.Add(wr); freeMMZ--; }
                }

                if (picks.Count >= 0 && !Bot.HasInHand(CardId.MalissP_MarchHare) && !ActiveMarchHare)
                {
                    var mh = Bot.Graveyard.GetFirstMatchingCard(g => g.IsCode(CardId.MalissP_MarchHare));
                    if (mh != null) picks.Add(mh);
                }

                if (freeMMZ > 0 && !ssRRThisTurn)
                {
                    var rr = Bot.Graveyard.GetFirstMatchingCard(g => g.IsCode(CardId.MalissQ_RedRansom));
                    if (rr != null && !picks.Contains(rr) && !ShouldSkipBanishing(rr))
                    { picks.Add(rr); freeMMZ--; }
                }

                if (freeMMZ > 0 && !ssHCThisTurn)
                {
                    var hc = Bot.Graveyard.GetFirstMatchingCard(g => g.IsCode(CardId.MalissQ_HeartsCrypter));
                    if (hc != null && !picks.Contains(hc) && !ShouldSkipBanishing(hc))
                    { picks.Add(hc); freeMMZ--; }
                }

                var threats = PickEnemyGYThreats(MAX_PICKS - picks.Count);
                foreach (var t in threats)
                {
                    if (picks.Count >= MAX_PICKS) break;
                    if (!picks.Contains(t)) picks.Add(t);
                }
            }

            if (picks.Count == 0) return false;

            if (picks.Count > MAX_PICKS) picks = picks.Take(MAX_PICKS).ToList();
            if (picks.Count < MAX_PICKS)
            {
                var more = PickEnemyGYThreats(MAX_PICKS - picks.Count);
                foreach (var t in more)
                    if (!picks.Contains(t)) picks.Add(t);
            }
            AI.SelectCard(picks);
            return true;
        }
        private bool IsMalissPawn(ClientCard c)
        {
            return c != null && (
                c.IsCode(CardId.MalissP_ChessyCat) ||
                c.IsCode(CardId.MalissP_WhiteRabbit) ||
                c.IsCode(CardId.MalissP_Dormouse) ||
                c.IsCode(CardId.MalissP_MarchHare)
            );
        }
        private List<ClientCard> PickEnemyGYThreats(int need)
        {
            var result = new List<ClientCard>();
            if (need <= 0) return result;

            var danger = GetDangerousCardinEnemyGrave(false);
            foreach (var c in danger)
            {
                if (result.Count >= need) break;
                if (!result.Contains(c)) result.Add(c);
            }

            if (result.Count < need)
            {
                for (int i = Enemy.Graveyard.Count - 1; i >= 0 && result.Count < need; i--)
                {
                    var c = Enemy.Graveyard[i];
                    if (c == null) continue;
                    if (!result.Contains(c)) result.Add(c);
                }
            }

            if (result.Count < need)
            {
                foreach (var c in Enemy.Graveyard)
                {
                    if (result.Count >= need) break;
                    if (!result.Contains(c)) result.Add(c);
                }
            }

            return result;
        }
        private bool WB_OnBanished_SelfSS()
        {
            if (DefaultCheckWhetherCardIdIsNegated(Card.Id)) return false;
            if (Card.Location != CardLocation.Removed) return false;
            if (GetMMZCount() >= 5) return false;
            if (Bot.LifePoints <= 900)
            {
                return false;
            }
            AI.SelectYesNo(true);
            ssWBThisTurn = true;
            return true;
        }
        private int PickMalissTrapForWB()
        {
            if (CheckRemainInDeck(CardId.MalissC_GWC06) > 0 || Bot.HasInGraveyard(CardId.MalissC_GWC06))
                return CardId.MalissC_GWC06;

            if (CheckRemainInDeck(CardId.MalissC_MTP07) > 0 || Bot.HasInGraveyard(CardId.MalissC_MTP07))
                return CardId.MalissC_MTP07;
            
            return 0;
        }
        private bool WB_SetMalissTrap()
        {
            if (DefaultCheckWhetherCardIdIsNegated(Card.Id)) return false;
            if (ActivateDescription != Util.GetStringId(CardId.MalissQ_WhiteBinder, 1)) return false;
            if (Card.Location != CardLocation.MonsterZone) return false;


            SelectSafeSTZoneAwayFromImperm();
            return true;
        }
        private List<ClientCard> PickLinkMatsMinCount(int targetLink,
                                                        Func<ClientCard, bool> isEligible,
                                                        int minCount,
                                                        int maxCount,
                                                        IEnumerable<int> avoidIds = null,
                                                        bool requireMaliss = false)
        {
            var all = Bot.GetMonsters()
                         .Where(m => m != null && m.IsFaceup() && isEligible(m))
                         .ToList();
            if (all.Count < minCount) return new List<ClientCard>();

            var avoid = new HashSet<int>(avoidIds ?? Enumerable.Empty<int>());

            Func<IEnumerable<ClientCard>, List<ClientCard>> OrderForFirst = (src) => src.OrderByDescending(LinkValOf)
                                                                              .ThenBy(m => avoid.Contains(m.Id) ? 1 : 0)
                                                                              .ThenBy(m => m.Attack)
                                                                              .ToList();

            Func<IEnumerable<ClientCard>, List<ClientCard>> OrderForLater = (src) => src.OrderBy(m => IsOneVal(m) ? 0 : 1)
                                                                               .ThenBy(m => m.HasType(CardType.Link) ? (Math.Max(1, m.LinkCount) == 1 ? 0 : 1) : -1)
                                                                               .ThenBy(LinkValOf)
                                                                               .ThenBy(m => avoid.Contains(m.Id) ? 1 : 0)
                                                                               .ThenBy(m => m.Attack)
                                                                               .ToList();

            var poolPreferred = all.Where(m => !avoid.Contains(m.Id)).ToList();
            var poolFallback = all.ToList();

            int firstMaxAllowed = targetLink - Math.Max(0, minCount - 1);

            Func<List<ClientCard>, List<ClientCard>> TryPick = (pool) =>
            {
                var chosen = new List<ClientCard>();
                Func<List<ClientCard>, int, bool> Dfs = null;
                Dfs = (avail, sum) =>
                {
                    if (sum > targetLink || chosen.Count > maxCount) return false;

                    if (chosen.Count >= minCount && sum == targetLink)
                    {
                        if (requireMaliss && !chosen.Any(IsMalissBody)) return false;
                        if (Util.GetBotAvailZonesFromExtraDeck(chosen) == 0) return false;
                        return true;
                    }

                    int need = Math.Max(0, minCount - chosen.Count);
                    if (sum + need > targetLink) return false;

                    var ordered = (chosen.Count == 0)
                        ? OrderForFirst(avail)
                        : OrderForLater(avail);

                    for (int i = 0; i < ordered.Count; i++)
                    {
                        var m = ordered[i];
                        var nextAvail = ordered.Where((x, idx) => idx != i).ToList();
                        int lv = LinkValOf(m);

                        int[] values = (chosen.Count == 0)
                            ? (lv > 1 ? new[] { Math.Min(lv, firstMaxAllowed), 1 } : new[] { 1 })
                            : (lv > 1 ? new[] { 1, lv } : new[] { 1 });

                        foreach (var v in values.Distinct())
                        {
                            int newSum = sum + v;
                            if (newSum > targetLink) continue;

                            int remMin = Math.Max(0, minCount - (chosen.Count + 1));
                            if (newSum + remMin > targetLink) continue;

                            chosen.Add(m);
                            if (Dfs(nextAvail, newSum)) return true;
                            chosen.RemoveAt(chosen.Count - 1);
                        }
                    }
                    return false;
                };

                if (Dfs(OrderForFirst(pool), 0)) return chosen;
                return null;
            };

            var pick = TryPick(poolPreferred);
            if (pick != null && pick.Count > 0) return pick;

            pick = TryPick(poolFallback);
            return pick ?? new List<ClientCard>();
        }
        private bool Link_Apo()
        {
            if (!(Bot.HasInMonstersZone(CardId.MalissQ_RedRansom) &&
                  Bot.HasInMonstersZone(CardId.LinkDecoder))) return false;
            

            var mats = PickLinkMatsMinCount(
                targetLink: 4,
                isEligible: m => m.HasType(CardType.Monster),
                minCount: 2,
                maxCount: 2,
                avoidIds: new[] { CardId.TranscodeTalker, CardId.AlliedCodeTalkerIgnister, CardId.AccesscodeTalker }
            );
            if (mats.Count == 0) return false;
            AI.SelectMaterials(mats);
            blockWicckid = true;
            return true;
        }
        private bool Flow3_Link_Firewall()
        {
            if (Bot.HasInMonstersZone(CardId.MalissQ_HeartsCrypter) && Bot.HasInMonstersZone(CardId.LinkDecoder) && Bot.HasInMonstersZone(CardId.CyberseWicckid))
            {
                var mats = PickLinkMatsMinCount(
                    targetLink: 4,
                    isEligible: m => m.HasType(CardType.Monster),
                    minCount: 2,
                    maxCount: 2,
                    avoidIds: new[] { CardId.CyberseWicckid, CardId.Apollousa, CardId.TranscodeTalker, CardId.AlliedCodeTalkerIgnister, CardId.AccesscodeTalker }
                );
                if (mats.Count == 0) return false;
                AI.SelectMaterials(mats);
                return true;
            }
            return false;
        }
        private bool Step_LinkSummon_HeartsCrypter()
        {
            if((Bot.HasInMonstersZone(CardId.MalissQ_WhiteBinder) && Bot.HasInMonstersZone(CardId.MalissQ_RedRansom) &&
                Bot.HasInMonstersZone(CardId.Apollousa) && Bot.GetMonsterCount() < 5)) return false;
            var cand = Bot.GetMonsters()
                          .Where(c => c != null && c.IsFaceup() && c.HasType(CardType.Effect))
                          .ToList();
            if (cand.Count < 3) return false;

            var avoid = new HashSet<int> { CardId.CyberseWicckid, CardId.Apollousa, CardId.AlliedCodeTalkerIgnister, CardId.AccesscodeTalker, CardId.FirewallDragon, CardId.TranscodeTalker };

            var ordered = cand
                .OrderBy(m => avoid.Contains(m.Id) ? 2 : 0)
                .ThenBy(m => m.HasType(CardType.Link) ? 1 : 0)
                .ThenBy(m => m.Attack)
                .ToList();

            List<ClientCard> mats = ordered.Take(3).ToList();
            if (!mats.Any(IsMaliss))
            {
                var maliss = ordered.FirstOrDefault(IsMaliss);
                if (maliss == null) return false;
                mats[2] = maliss;
            }

            if (Util.GetBotAvailZonesFromExtraDeck(mats) == 0) return false;
            AI.SelectMaterials(mats);
            return true;
        }
        private bool HC_Quick_ReturnBanished_AndBanishField()
        {
            if (DefaultCheckWhetherCardIdIsNegated(Card.Id)) return false;
            if (ActivateDescription != Util.GetStringId(CardId.MalissQ_HeartsCrypter, 0)) return false;
            if (Card.Location != CardLocation.MonsterZone) return false;
            if (CheckWhetherNegated()) return false;

            RefreshNoChainWindows();

            var banishedMaliss = GetBanishedMaliss();
            if (banishedMaliss.Count == 0) return false;
            bool haveReturn = banishedMaliss.Count > 0;

            bool mustNow = GetProblematicEnemyCardList(
                        canBeTarget: true,
                        selfType: CardType.Monster | CardType.Spell | CardType.Trap
                   ).Count > 0;

            if (Duel.Player == 0)
            {
                if (!haveReturn && !mustNow) return false;
            }
            else
            {
                if (!mustNow && !IsPreferredRemovalTiming()) return false;
            }

            if (!haveReturn) return false;
            var ret = PickBanishedMalissForHC(banishedMaliss);
            if (ret == null) return false;

            var fieldTargets = GetProblematicEnemyCardList(true, selfType: CardType.Monster | CardType.Spell | CardType.Trap);
            if (fieldTargets.Count == 0)
            {
                var any = GetBestEnemyCard(false, true);
                if (any != null) fieldTargets.Add(any);
            }
            if (fieldTargets.Count == 0) return false;

            AI.SelectCard(ret);
            AI.SelectNextCard(fieldTargets);
            ConsumePreferredWindow();
            return DontSelfNG();
        }
        private List<ClientCard> GetBanishedMaliss()
        {
            return Bot.Banished.GetMatchingCards(c =>
                c != null && c.IsFaceup() && c.HasSetcode(0x1bf)).ToList();
        }
        private ClientCard PickBanishedMalissForHC(List<ClientCard> cand)
        {
            cand = cand
                        .Where(c => !c.IsCode(CardId.MalissQ_WhiteBinder) && !c.IsCode(CardId.MalissQ_RedRansom))
                        .ToList();
            if (cand.Count == 0) return null;

            return cand.OrderByDescending(ScoreForBanishedMaliss).First();
        }
        private bool HC_OnBanished_SpecialSummon()
        {
            if (DefaultCheckWhetherCardIdIsNegated(Card.Id)) return false;
            if (Card.Location != CardLocation.Removed) return false;

            if (GetMMZCount() >= 5)
            {
                return false;
            }

            if (Bot.LifePoints <= 900)
            {
                return false;
            }

            AI.SelectYesNo(true);
            ssHCThisTurn = true;
            return true;
        }
        private bool HasMalissLinkFaceup()
        {
            return Bot.GetMonsters().Any(c => c != null && c.IsFaceup() && c.HasSetcode(0x1bf) && c.HasType(CardType.Link));
        }
        private bool CanReachAlliedNow()
        {
            var mats = PickLinkMatsMinCount(
                targetLink: 5,
                isEligible: m => m.HasType(CardType.Effect),
                minCount: 3,
                maxCount: 5,
                avoidIds: new[] { CardId.Apollousa, CardId.FirewallDragon, CardId.AccesscodeTalker }
            );
            return mats.Count > 0;
        }
        private bool Flow3_Link_Allied()
        {
            if (!(Bot.HasInMonstersZoneOrInGraveyard(CardId.MalissQ_RedRansom) && Bot.HasInMonstersZoneOrInGraveyard(CardId.MalissQ_WhiteBinder))) return false;
            var mats = PickLinkMatsMinCount(
                targetLink: 5,
                isEligible: m => m.HasType(CardType.Effect),
                minCount: 3,
                maxCount: 5,
                avoidIds: new[] { CardId.Apollousa, CardId.FirewallDragon, CardId.AccesscodeTalker }
            );
            if (mats.Count == 0)
            {
                return false;
            }

            AI.SelectMaterials(mats);
            Allied_End = true;
            return true;
        }
        private bool Emer_Allied()
        {
            if(!enemyActivateLancea) return false;
            if (!(Bot.HasInMonstersZone(CardId.TranscodeTalker) && Bot.HasInMonstersZone(CardId.SplashMage))) return false;
            var mats = PickLinkMatsMinCount(
                targetLink: 5,
                isEligible: m => m.HasType(CardType.Effect),
                minCount: 3,
                maxCount: 3
            );
            if (mats.Count == 0)
            {
                return false;
            }

            AI.SelectMaterials(mats);
            Allied_End = true;
            return true;
        }
        private bool Emer_Allied2()
        {
            var myMonsters = Bot.GetMonsters().Where(m => m != null).ToList();
            if (myMonsters.Count != 3) return false;

            var link3List = myMonsters
                .Where(m => m.HasType(CardType.Link) && m.LinkCount == 3)
                .ToList();

            if (link3List.Count != 1) return false;

            bool haveKeyInGY =
                Bot.HasInGraveyard(CardId.MalissQ_RedRansom) ||
                Bot.HasInGraveyard(CardId.MalissQ_WhiteBinder) ||
                Bot.HasInGraveyard(CardId.TranscodeTalker);

            if (!haveKeyInGY) return false;

            var mats = PickLinkMatsMinCount(
                targetLink: 5,
                isEligible: m => m.HasType(CardType.Effect),
                minCount: 3,
                maxCount: 3
            );
            if (mats.Count == 0)
            {
                return false;
            }

            AI.SelectMaterials(mats);
            Allied_End = true;
            return true;
        }
        private ClientCard FindGWC06TargetByOrder(params int[] ids)
        {
            foreach (var id in ids)
            {
                var gy = Bot.Graveyard.GetFirstMatchingCard(c => c != null && c.IsCode(id));
                if (gy != null) return gy;

                var ban = Bot.Banished.GetFirstMatchingCard(c => c != null && c.IsCode(id) && c.IsFaceup());
                if (ban != null) return ban;
            }
            return null;
        }
        private int PickGWC06CostCandidateId()
        {
            if (Bot.HasInMonstersZone(CardId.MalissQ_WhiteBinder) && !ssWBThisTurn)
                return CardId.MalissQ_WhiteBinder;
            if (Bot.HasInMonstersZone(CardId.MalissQ_RedRansom) && !ssRRThisTurn)
                return CardId.MalissQ_RedRansom;

            int card = PickTB11CostCandidateId();
            if (card != 0) return card;

            return 0;
        }
        private ClientCard PickGWC06TargetExtend()
        {
            if (Duel.Turn > 2)
            {
                return FindGWC06TargetByOrder(
                    CardId.MalissQ_HeartsCrypter,
                    CardId.MalissQ_RedRansom,
                    CardId.MalissQ_WhiteBinder
                );
            }
            else {
                return FindGWC06TargetByOrder(
                    CardId.MalissQ_WhiteBinder,
                    CardId.MalissQ_RedRansom,
                    CardId.MalissQ_HeartsCrypter,
                    CardId.MalissP_WhiteRabbit,
                    CardId.MalissP_Dormouse,
                    CardId.MalissP_ChessyCat,
                    CardId.MalissP_MarchHare
                );
            }
        }
        private bool GWC06_MyTurn_Extend()
        {
            if (DefaultCheckWhetherCardIdIsNegated(Card.Id)) return false;
            if (Duel.Player == 0 &&
                GetMMZCount() >= 4 &&
                Bot.HasInMonstersZone(CardId.FirewallDragon) &&
                (Bot.HasInMonstersZoneOrInGraveyard(CardId.MalissQ_RedRansom) ||
                Bot.HasInMonstersZoneOrInGraveyard(CardId.MalissQ_WhiteBinder) ||
                Bot.HasInMonstersZoneOrInGraveyard(CardId.MalissQ_HeartsCrypter))) return false;
            if (Duel.Player != 0) return false;
            if (!(Duel.Phase == DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2)) return false;
            if (CheckSpellWillBeNegate()) return false;
            if (GetMMZCount() >= 5) return false;
            if (!Bot.HasInMonstersZone(CardId.MalissQ_WhiteBinder) && !(Bot.HasInMonstersZone(CardId.MalissQ_RedRansom) && Bot.GetMonsterCount() == 1)) return false;
            var target = PickGWC06TargetExtend();
            if (target == null) return false;
            if (gwc06SetThisTurn)
            {
                int costId = PickGWC06CostCandidateId();
                if (costId == 0) return false;
                AI.SelectCard(costId);
                AI.SelectNextCard(target);
                return DontSelfNG();
            }
            AI.SelectCard(target);
            return DontSelfNG();
        }
        private bool GWC06_OppTurn_ReviveWB_HC()
        {
            if (Duel.Player != 1) return false;
            if (!Bot.HasInSpellZone(CardId.MalissC_GWC06)) return false;
            if (CheckSpellWillBeNegate()) return false;
            if (GetMMZCount() >= 5) return false;

            var target = FindGWC06TargetByOrder(
                CardId.MalissQ_WhiteBinder,
                CardId.MalissQ_HeartsCrypter
            );
            if (target == null) return false;

            if (gwc06SetThisTurn)
            {
                int costId = PickGWC06CostCandidateId();
                if (costId == 0) return false;
                AI.SelectCard(costId);
                AI.SelectNextCard(target);
                return DontSelfNG();
            }

            AI.SelectCard(target);
            return DontSelfNG();
        }
        private static int FirstBit(int mask)
        {
            for (int i = 0; i < 32; i++)
            {
                int b = 1 << i;
                if ((mask & b) != 0) return b;
            }
            return 0;
        }
        private static int FirstBitFromOrder(int mask, int[] order)
        {
            foreach (var b in order)
                if ((mask & b) != 0) return b;
            return FirstBit(mask);
        }
        private int PreferSafeSummonZones(int available)
        {
            int MAIN_MASK =
                (int)Zones.z0 |
                (int)Zones.z1 |
                (int)Zones.z2 |
                (int)Zones.z3 |
                (int)Zones.z4;

            int emzMask = available & ~MAIN_MASK;

            if (emzMask != 0)
                return FirstBit(emzMask);

            int enemyPointed = 0;
            try
            {
                enemyPointed = Enemy.GetLinkedZones();
            }
            catch { }

            int safeMain = (available & MAIN_MASK) & ~enemyPointed;

            if (safeMain != 0)
            {
                return FirstBitFromOrder(
                    safeMain,
                    new[] { (int)Zones.z2, (int)Zones.z1, (int)Zones.z3, (int)Zones.z0, (int)Zones.z4 }
                );
            }

            return FirstBit(available);
        }
        private int PickMTP07CostCandidateId()
        {
            return PickTB11CostCandidateId();
        }
        private ClientCard PickMTP07EnemyRemovalTarget()
        {
            var list = GetProblematicEnemyCardList(canBeTarget: true, ignoreSpells: false, selfType: CardType.Trap);
            if (list.Count > 0) return list[0];

            var m = GetBestEnemyMonster(onlyFaceup: false, canBeTarget: true);
            if (m != null) return m;

            var s = GetBestEnemySpell(onlyFaceup: false, canBeTarget: true);
            if (s != null) return s;

            return Enemy.GetMonsters().FirstOrDefault(c => c != null)
                ?? Enemy.GetSpells().FirstOrDefault(c => c != null);
        }
        private bool MTP07_ForMH()
        {
            if (!(Bot.GetMonsterCount() == 1 && Bot.HasInMonstersZone(CardId.MalissP_WhiteRabbit))) return false;
            if (CheckSpellWillBeNegate()) return false;
            if (CheckWhetherNegated()) return false;

            int searchId = PickMTP07SearchId();
            if (searchId == 0) return false;

            if (mtp07SetThisTurn)
            {
                int costId = PickMTP07CostCandidateId();
                if (costId == 0) return false;
                AI.SelectCard(costId);
            }
            AI.SelectNextCard(searchId);
            return DontSelfNG();
        }
        private bool MTP07_OppTurn_RemoveEnemyOnly()
        {
            if (Duel.Player != 1) return false;
            if (!HasMalissLinkFaceup()) return false;
            if (CheckSpellWillBeNegate()) return false;
            if (CheckWhetherNegated()) return false;
            var urgent = GetProblematicEnemyCardList(canBeTarget: true, ignoreSpells: false, selfType: CardType.Trap);
            if (urgent.Count == 0 && !IsPreferredRemovalTiming()) return false;
            bool preBattle = Duel.Phase == DuelPhase.Main1 && Enemy.GetMonsterCount() > 0;
            if (urgent.Count == 0 && !(IsPreferredRemovalTiming() || preBattle)) return false;
            int searchId = PickMTP07SearchId();
            if (searchId == 0) return false;
            ClientCard target = null;
            if (urgent.Count > 0)
            {
                target = urgent[0];
            }
            else if (preBattle)
            {
                target = Enemy.MonsterZone
                    .Where(c => c != null && c.IsFaceup())
                    .OrderByDescending(c => c.Attack)
                    .FirstOrDefault();
            }
            else
            {
                target = PickMTP07EnemyRemovalTarget();
            }
            if (target == null) return false;
            if (mtp07SetThisTurn)
            {
                int costId = PickMTP07CostCandidateId();
                if (costId == 0) return false;
                AI.SelectCard(costId);
                AI.SelectNextCard(searchId);
                AI.SelectNextCard(target);
                ConsumePreferredWindow();
                return DontSelfNG();
            }
            AI.SelectCard(searchId);
            AI.SelectNextCard(target);
            ConsumePreferredWindow();
            return DontSelfNG();
        }
        private bool Emergency_NS()
        {
            if (usedNormalSummon) return false;
            if (Bot.GetMonsterCount() != 0) return false;
            if (Bot.HasInHand(CardId.MalissP_Dormouse) || Bot.HasInHand(CardId.MalissP_WhiteRabbit)
                || Bot.HasInHand(CardId.MalissP_ChessyCat)
                || Bot.HasInHand(CardId.GoldSarcophagus) || Bot.HasInHand(CardId.MalissInUnderground)) return false;
            if (!Bot.HasInHand(CardId.BackupIgnister)) return false;
            usedNormalSummon = true;
            nsplan = true;
            return true;
        }
        private bool IsWicInEMZ(ClientCard wic)
        {
            return wic != null
                && wic.Location == CardLocation.MonsterZone
                && (wic.Sequence == EMZ_LEFT || wic.Sequence == EMZ_RIGHT);
        }
        private IEnumerable<int> GetWicDownSeq(ClientCard wic)
        {
            if (!IsWicInEMZ(wic)) yield break;
            if (wic.Sequence == EMZ_LEFT) { yield return 1; yield return 2; }
            if (wic.Sequence == EMZ_RIGHT) { yield return 3; yield return 4; }
        }
        private bool IsUnderWic(ClientCard wic, ClientCard m)
        {
            if (wic == null || m == null) return false;
            if (m.Controller != wic.Controller) return false;
            if (m.Location != CardLocation.MonsterZone) return false;
            if (m.Sequence < 0 || m.Sequence > 4) return false;
            return GetWicDownSeq(wic).Contains(m.Sequence);
        }
        private ClientCard PickUnderlingForTranscode(ClientCard wic, System.Collections.Generic.IList<ClientCard> pool)
        {
            if (!IsWicInEMZ(wic)) return null;
            var underlings = pool.Where(x => x != null && x != wic && x.IsFaceup()
                                                                && x.Location == CardLocation.MonsterZone
                                                                && x.Controller == wic.Controller
                                                                && x.Sequence >= 0 && x.Sequence <= 4
                                                                && IsUnderWic(wic, x)
                                                                && (x.HasType(CardType.Link) || x.HasType(CardType.Effect))
                                                                && !x.HasType(CardType.Token))
                                                                .ToList();

            if (underlings.Count == 0) return null;

            var preferLD = underlings.FirstOrDefault(x => x.IsCode(CardId.LinkDecoder));
            if (preferLD != null) return preferLD;
            return underlings
                            .OrderByDescending(x => x.HasType(CardType.Link) ? 2 : 1)
                            .ThenByDescending(x => x.Attack)
                            .FirstOrDefault();
        }
        private static readonly int[] PreferCenterMainSeq = new[] { 2, 1, 3, 0, 4 };

        private bool SummonTranscode()
        {
            if (!enemyActivateLancea) return false;
            var wic = Bot.GetMonsters().FirstOrDefault(
                x => x != null && x.IsFaceup() && x.IsCode(CardId.CyberseWicckid));

            if (!IsWicInEMZ(wic)) return false;

            var under = PickUnderlingForTranscode(wic, Bot.GetMonsters());
            if (under == null) return false;

            SelectLinkMaterialsPair(wic, under);

            int emz = EmzBitFor(wic);
            if (emz != 0) AI.SelectPlace(emz);

            return true;
        }
        private void SelectLinkMaterialsPair(ClientCard a, ClientCard b)
        {
            try
            {
                var mats = new System.Collections.Generic.List<ClientCard> { a, b };
                AI.SelectMaterials(mats);
                return;
            }
            catch { }

            AI.SelectCard(a);
            AI.SelectNextCard(b);
        }
        private int EmzBitFor(ClientCard link)
        {
            if (link == null || link.Location != CardLocation.MonsterZone) return 0;
            if (link.Sequence == EMZ_LEFT) return 1 << EMZ_LEFT; // 1<<5
            if (link.Sequence == EMZ_RIGHT) return 1 << EMZ_RIGHT; // 1<<6
            return 0;
        }
        int ChooseAndRememberWicckidEmz(int available)
        {
            int emzAvail = available & EMZ_ALL;
            if (emzAvail == 0) return 0;

            int best = 0;
            int bestScore = int.MinValue;

            foreach (int emz in new[] { EMZ_L, EMZ_R })
            {
                if ((emzAvail & emz) == 0) continue;

                int score = 0;
                int down = DownBitOfEmz(emz);

                if (down == MZ1 && IsMainFreeSeq(1)) score += 10;
                if (down == MZ3 && IsMainFreeSeq(3)) score += 10;

                if (score > bestScore)
                {
                    bestScore = score;
                    best = emz;
                }
            }

            if (best == 0)
                best = (emzAvail & EMZ_L) != 0 ? EMZ_L : EMZ_R;

            _wicckidEmzBit = best;
            return best;
        }
        bool IsMainFreeSeq(int seq)
        {
            var ms = Bot.MonsterZone.GetMonsters();
            var occupied = ms.Any(m => m != null && m.Controller == 0 && m.Sequence == seq);
            return !occupied;
        }
        int DownBitOfEmz(int emzBit)
        {
            if (emzBit == EMZ_L) return MZ1;
            if (emzBit == EMZ_R) return MZ3;
            return 0;
        }
        private bool IsPreferredRemovalTiming()
        {
            if (Duel.Player != 1) return false;
            if (_prefWindowTTL > 0) return true;
            if (_oppJustActivatedPersistentSpell || _oppJustSummoned || _oppJustSet) return true;

            return false;
        }
        private void ConsumePreferredWindow()
        {
            _prefWindowTTL = 0;
            _oppJustActivatedPersistentSpell = false;
            _oppJustSummoned = false;
            _oppJustSet = false;
        }
        private void RefreshNoChainWindows()
        {
            bool oppMain = (Duel.Player == 1) && (Duel.Phase == DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2);

            int curMon = Enemy.GetMonsterCount();
            if (curMon > _enemyMonsterCountSnap)
            {
                _oppJustSummoned = true;
                if (oppMain) _prefWindowTTL = Math.Max(_prefWindowTTL, 2);
            }
            _enemyMonsterCountSnap = curMon;

            int curFD = Enemy.SpellZone.Count(c => c != null && c.IsFacedown());
            if (curFD > _enemyFacedownSTSnap)
            {
                _oppJustSet = true;
                if (oppMain) _prefWindowTTL = Math.Max(_prefWindowTTL, 2);
            }
            _enemyFacedownSTSnap = curFD;

            if (oppMain)
            {
                if (_prefWindowTTL > 0) _prefWindowTTL--;
                if (_prefWindowTTL == 0)
                {
                    _oppJustActivatedPersistentSpell = false;
                    _oppJustSummoned = false;
                    _oppJustSet = false;
                }
            }
            else
            {
                _prefWindowTTL = 0;
                _oppJustActivatedPersistentSpell = false;
                _oppJustSummoned = false;
                _oppJustSet = false;
            }
        }
        private bool FirewallBounce_OnOppSummon()
        {
            if (DefaultCheckWhetherCardIdIsNegated(Card.Id)) return false;
            if (ActivateDescription != Util.GetStringId(CardId.FirewallDragon, 0))
                return false;

            if (Duel.LastSummonPlayer != 1)
                return false;

            var picks = new List<ClientCard>();

            var negateList = GetMonsterListForTargetNegate();
            if (negateList != null)
            {
                foreach (var c in negateList)
                {
                    if (c == null) continue;
                    if (c.Controller != 1) continue;
                    if (!c.IsMonster()) continue;
                    if (!c.IsFaceup()) continue;
                    if (!picks.Contains(c))
                        picks.Add(c);
                }
            }
            foreach (var m in Enemy.GetMonsters().OrderByDescending(x => x.Attack))
            {
                if (m == null) continue;
                if (!m.IsMonster()) continue;
                if (!m.IsFaceup()) continue;
                if (!picks.Contains(m))
                    picks.Add(m);
            }

            if (picks.Count == 0)
                return false;

            AI.SelectCard(picks);
            return true;
        }
        private bool Accesscode_OnSummon_AtkUp()
        {
            var list = new List<ClientCard>();
            int[] prefer = {
                                CardId.MalissQ_HeartsCrypter,
                                CardId.MalissQ_WhiteBinder,
                                CardId.MalissQ_RedRansom,
                                CardId.TranscodeTalker
                            };
            foreach (var id in prefer)
            {
                var hit = Bot.Graveyard.GetFirstMatchingCard(c => c != null && c.IsCode(id));
                if (hit != null) list.Add(hit);
            }

            AI.SelectCard(list);
            return true;
        }

        private bool Accesscode_Destroy_Ignition()
        {
            if (ActivateDescription != Util.GetStringId(CardId.AccesscodeTalker, 1))
                return false;

            if (Enemy.GetFieldCount() == 0)
                return false;

            IEnumerable<ClientCard> costPool =
                (Bot.GetGraveyardMonsters() ?? Enumerable.Empty<ClientCard>())
                    .Where(c => c.HasType(CardType.Link))
                .Concat((Bot.GetMonsters() ?? Enumerable.Empty<ClientCard>())
                    .Where(c => c.HasType(CardType.Link) && c != Card));

            var costList = costPool
                .OrderBy(c => c.Location != CardLocation.Grave)
                .ThenBy(c => c.LinkCount)
                .ThenBy(c => c.Attack)
                .ToList();

            if (costList.Count == 0)
                return false;

            var targets = new List<ClientCard>();

            targets.AddRange(Enemy.GetSpells()
                .Where(s => s != null && s.IsFacedown()));
            targets.AddRange(Enemy.GetSpells()
                .Where(s => s != null && !s.IsFacedown()));
            targets.AddRange(Enemy.GetMonsters()
                .OrderByDescending(m => m.Attack));

            if (targets.Count == 0)
                return false;

            AI.SelectCard(costList);
            AI.SelectNextCard(targets);

            return true;
        }
        private bool AlreadySSFromBanishThisTurn(ClientCard c)
        {
            if (c == null) return false;
            if (c.Controller != 0) return false;

            if (c.IsCode(CardId.MalissP_Dormouse)) return ssDormouse;
            if (c.IsCode(CardId.MalissP_WhiteRabbit)) return ssWhiteRabbit;
            if (c.IsCode(CardId.MalissP_ChessyCat)) return ssChessyCat;

            if (c.IsCode(CardId.MalissQ_RedRansom)) return ssRRThisTurn;
            if (c.IsCode(CardId.MalissQ_HeartsCrypter)) return ssHCThisTurn;
            if (c.IsCode(CardId.MalissQ_WhiteBinder)) return ssWBThisTurn;

            return false;
        }
        private bool ShouldSkipBanishing(ClientCard c)
        {
            if (c == null) return true;
            if (c.Controller != 0) return false;
            if (AlreadySSFromBanishThisTurn(c)) return true;
            if (c.IsCode(CardId.MalissP_MarchHare) && ActiveMarchHare) return true;
            return false;
        }
        private bool DescIs(int cardId, params int[] idx)
        {
            if (ActivateDescription == -1) return true;
            foreach (var i in idx)
                if (ActivateDescription == Util.GetStringId(cardId, i)) return true;
            return false;
        }

        private ClientCard PickMirrorCostCandidate()
        {
            int[] handPref = {
                                CardId.MalissP_WhiteRabbit,
                                CardId.MalissP_ChessyCat,
                                CardId.MalissP_Dormouse,
                                CardId.MalissP_MarchHare
                            };
            foreach (var id in handPref)
            {
                var h = Bot.Hand.GetFirstMatchingCard(c => c != null && c.IsCode(id));
                if (h != null) return h;
            }

            var fieldP = Bot.GetMonsters()
                .Where(c => c != null && c.HasSetcode(0x1bf) && !c.HasType(CardType.Link))
                .OrderBy(c => c.Attack).FirstOrDefault();
            if (fieldP != null) return fieldP;

            int[] avoid = {
                                CardId.MalissQ_HeartsCrypter, CardId.MalissQ_WhiteBinder, CardId.MalissQ_RedRansom
                            };
            var weakLink = Bot.GetMonsters()
                .Where(c => c != null && c.HasSetcode(0x1bf) && c.HasType(CardType.Link) && !avoid.Contains(c.Id))
                .OrderBy(c => c.Attack).FirstOrDefault();
            return weakLink;
        }

        private int[] Mirror_SearchOrderForType(bool isTrap, bool isMon)
        {
            if (isTrap)
                return new[] { CardId.MalissC_GWC06, CardId.MalissC_MTP07 };
            if (isMon)
            {
                if (!Bot.HasInHand(CardId.MalissP_WhiteRabbit) && NSDorMouse)
                {
                    return new[] { CardId.MalissP_WhiteRabbit, CardId.MalissP_MarchHare, CardId.MalissP_Dormouse, CardId.MalissP_ChessyCat };
                }
                else
                {
                    return new[] { CardId.MalissP_MarchHare, CardId.MalissP_Dormouse, CardId.MalissP_WhiteRabbit, CardId.MalissP_ChessyCat };
                }
            }
            return new[] { CardId.MalissP_MarchHare, CardId.MalissP_Dormouse, CardId.MalissP_WhiteRabbit, CardId.MalissP_ChessyCat };
        }

        private ClientCard PickMirrorGYTargetForSearch()
        {
            if (NSDorMouse && !ssRRThisTurn)
            {
                var rr = Bot.Graveyard.GetFirstMatchingCard(
                    c => c != null && c.IsCode(CardId.MalissQ_RedRansom));

                if (rr != null)
                    return rr;
            }
            int[] monPref = { CardId.MalissP_MarchHare, CardId.MalissP_Dormouse, CardId.MalissP_WhiteRabbit, CardId.MalissP_ChessyCat };
            foreach (var id in monPref)
            {
                var m = Bot.Graveyard.GetFirstMatchingCard(c => c != null && c.IsCode(id));
                if (m != null && CheckRemainInDeck(id) > 0) return m;
            }
            return null;
        }
        private int PickMTP07SearchId()
        {
            if (CheckRemainInDeck(CardId.MalissP_MarchHare) > 0)
                return CardId.MalissP_MarchHare;

            int[] pawnPref = {
                                CardId.MalissP_Dormouse,
                                CardId.MalissP_WhiteRabbit,
                                CardId.MalissP_ChessyCat
                            };
            foreach (var id in pawnPref)
                if (CheckRemainInDeck(id) > 0) return id;

            return 0;
        }
        private void SelectSafeSTZoneAwayFromImperm()
        {
            var safeCols = Enumerable.Range(0, 5)
                .Where(seq =>
                {
                    if (Bot.SpellZone[seq] != null) return false;
                    if (infiniteImpermanenceList.Contains(seq)) return false;
                    return true;
                })
                .ToList();

            if (safeCols.Count == 0)
            {
                safeCols = Enumerable.Range(0, 5)
                    .Where(seq => Bot.SpellZone[seq] == null)
                    .ToList();
            }

            int mask = 0;
            foreach (int seq in safeCols) mask |= (1 << seq);
            AI.SelectPlace(mask);
        }
        private bool T3Allow()
        {
            if (Duel.Player != 0) return false;
            if (myTurnCount < 2) return false;
            if (!HaveBackupOrWizardInHand() || !Bot.HasInHand(CardId.MalissP_MarchHare)) return false;
            return true;
        }
        private bool NSBackup()
        {
            if (Bot.GetMonsterCount() != 0) return false;
            if (usedNormalSummon) return false;
            if (Bot.HasInHand(CardId.MalissP_Dormouse) || Bot.HasInHand(CardId.MalissP_WhiteRabbit) || Bot.HasInHand(CardId.MalissP_ChessyCat) ||
                Bot.HasInHand(CardId.MalissInUnderground) || Bot.HasInHand(CardId.TERRAFORMING) ||
                Bot.HasInHand(CardId.GoldSarcophagus)) return false;
            nsBackupplan = true;
            return true;
        }
        private bool NSBackup_L()
        {
            if (Bot.GetMonsterCount() != 0) return false;
            if (usedNormalSummon) return false;
            if (Bot.HasInHand(CardId.MalissP_Dormouse) || Bot.HasInHand(CardId.MalissP_WhiteRabbit) || Bot.HasInHand(CardId.MalissP_ChessyCat) ||
                Bot.HasInHand(CardId.MalissP_MarchHare) || Bot.HasInHand(CardId.MalissInUnderground) || Bot.HasInHand(CardId.TERRAFORMING) ||
                Bot.HasInHand(CardId.GoldSarcophagus)) return false;
            nsLanceaplan = true;
            return true;
        }

        private bool NSMH()
        {
            int mhCount = Bot.Hand.GetMatchingCards(c => c != null && c.IsCode(CardId.MalissP_MarchHare)).Count;
            if (mhCount <2 ) return false;
            if (Bot.GetMonsterCount() != 0) return false;
            if (usedNormalSummon) return false;
            if (Bot.HasInHand(CardId.MalissP_Dormouse) || Bot.HasInHand(CardId.MalissP_WhiteRabbit) || Bot.HasInHand(CardId.MalissP_ChessyCat) ||
                Bot.HasInHand(CardId.MalissInUnderground) || Bot.HasInHand(CardId.TERRAFORMING) ||
                Bot.HasInHand(CardId.GoldSarcophagus)) return false;
            return true;
        }
        private bool LinguribohMHLine()
        {
            if (!(Bot.HasInHand(CardId.MalissP_MarchHare) && Bot.HasInMonstersZone(CardId.MalissP_MarchHare))) return false;
            if (Bot.GetMonsterCount() != 1) return false;
            return true;
        }
        private bool EmerTranscode()
        {
            if (!enemyActivateLancea) return false;
            if (Bot.GetMonsterCount() < 3) return false;
            var mats = PickLinkMatsMinCount(
                targetLink: 3,
                isEligible: m => m.HasType(CardType.Effect),
                minCount: 2,
                maxCount: 2
            );
            if (mats.Count == 0)
            {
                return false;
            }

            AI.SelectMaterials(mats);
            return true;
        }
        #endregion

        // ======================= END OF LIFE ====================
    }
}
