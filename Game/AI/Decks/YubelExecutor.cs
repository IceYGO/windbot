using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using YGOSharp.Network.Enums;
using YGOSharp.OCGWrapper;
using YGOSharp.OCGWrapper.Enums;

namespace WindBot.Game.AI.Decks
{
    [Deck("Yubel", "AI_Yubel")]
    public class YubelExecutor : DefaultExecutor
    {
        public class CardId
        {
            // === Core Yubel ===
            public const int YUBEL = 78371393;
            public const int YUBEL_TERROR_INCARNATE = 4779091;
            public const int SPIRIT_OF_YUBEL = 90829280;
            public const int PHANTOM_OF_YUBEL = 80453041; // (extra)

            // === Starters / Engine ===
            public const int SAMSARA_D_LOTUS = 62318994;
            public const int GRUESOME_GRAVE_SQUIRMER = 24215921;
            public const int FABLED_LURRIE = 97651498;

            // === Fiendsmith / Fiend package ===
            public const int SHARVARA = 41165831;
            public const int FIENDSMITH_ENGRAVER = 60764609;
            public const int LACRIMA_CT = 28803166;
            public const int DARK_BECKONING_BEAST = 81034083;
            public const int CHAOS_SUMMONING_BEAST = 27439792;

            // === Spells / Traps ===
            public const int NIGHTMARE_PAIN = 65261141;
            public const int NIGHTMARE_THRONE = 93729896;
            public const int SPIRIT_GATES = 80312545;
            public const int FIENDSMITH_TRACT = 98567237;
            public const int ABOMINABLE_CHAMBER = 80801743;
            public const int FIENDSMITHS_PARADISE = 99989863;
            public const int TERRAFORMING = 73628505;

            // === Extra Deck / อื่น ๆ ===
            public const int FIENDSMITHS_DESIRAE = 82135803;
            //public const int FIENDSMITHS_LACRIMA = 46640168;
            public const int VARUDASN_FINAL_BRINGER = 70636044;
            public const int DDD_WAVE_HIGH_KING_CAESAR = 79559912;
            public const int UNCHAINED_LORD_OF_YAMA = 24269961;
            public const int UNCHAINED_SOUL_OF_RAGE = 67680512;
            public const int SP_LITTLE_KNIGHT = 29301450;
            public const int MOON_OF_THE_CLOSED_HEAVEN = 71818935;
            //public const int FIENDSMITHS_SEQUENCE = 49867899;
            //public const int MUCKRAKER_UNDERWORLD = 71607202;
            public const int FIENDSMITHS_REQUIEM = 2463794;
            public const int SALAMANGREAT_ALMIRAJ = 60303245;
            public const int NECROQUIP = 93860227;
            //public const int ARIALEATER = 28143384;
            public const int GORGONOFZIL = 12067160;
            public const int GUSTAVMAX = 56910167;
            public const int JUGGERNAUT = 26096328;
            public const int UNCHAINDEDABOMINATION = 29479265;

            // === Handtraps / Others (blacklist/targets etc.) ===
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
            public const int AccesscodeTalker = 86066372;
            public const int GhostMournerMoonlitChill = 52038441;
            public const int NibiruThePrimalBeing = 27204311;
        }

        const int SetcodeTimeLord = 0x4a;
        const int SetcodePhantom = 0xdb;
        const int SetcodeOrcust = 0x11b;
        const int SetcodeHorus = 0x19d;

        Dictionary<int, List<int>> DeckCountTable = new Dictionary<int, List<int>>{
            {3, new List<int> { CardId.SPIRIT_OF_YUBEL, CardId.SAMSARA_D_LOTUS,CardId.NIGHTMARE_THRONE, CardId.SPIRIT_GATES,
                                 _CardId.AshBlossom, CardId.DARK_BECKONING_BEAST,_CardId.InfiniteImpermanence } },
            {2, new List<int> { _CardId.MaxxC, _CardId.CalledByTheGrave,CardId.GRUESOME_GRAVE_SQUIRMER}},
            {1, new List<int> { CardId.FIENDSMITH_ENGRAVER, CardId.FIENDSMITHS_PARADISE,
                                CardId.YUBEL, CardId.YUBEL_TERROR_INCARNATE, CardId.ABOMINABLE_CHAMBER,
                                _CardId.CrossoutDesignator, CardId.GRUESOME_GRAVE_SQUIRMER, CardId.FABLED_LURRIE,
                                CardId.NIGHTMARE_PAIN, CardId.TERRAFORMING, CardId.FIENDSMITH_TRACT,CardId.SHARVARA,
                                CardId.CHAOS_SUMMONING_BEAST, CardId.LACRIMA_CT 
                                } }
        };

        List<int> notToNegateIdList = new List<int> { 58699500, 20343502, 19403423 };
        List<int> notToDestroySpellTrap = new List<int> { 50005218, 6767771 };
        List<int> targetNegateIdList = new List<int> {
            _CardId.EffectVeiler, _CardId.InfiniteImpermanence, CardId.GhostMournerMoonlitChill, _CardId.BreakthroughSkill, 74003290, 67037924,
            9753964, 66192538, 23204029, 73445448, 35103106, 30286474, 45002991, 5795980, 38511382, 53742162, 30430448
        };

        private static readonly int[] LinkFodderPriority = new[]
        {
            CardId.SHARVARA,
            CardId.YUBEL_TERROR_INCARNATE,
            CardId.UNCHAINED_SOUL_OF_RAGE,
            CardId.UNCHAINED_LORD_OF_YAMA,
            CardId.GRUESOME_GRAVE_SQUIRMER,
            CardId.FABLED_LURRIE,
            CardId.LACRIMA_CT,
            CardId.SALAMANGREAT_ALMIRAJ,
            CardId.DARK_BECKONING_BEAST,
            CardId.CHAOS_SUMMONING_BEAST,
            CardId.SAMSARA_D_LOTUS,
            CardId.SPIRIT_OF_YUBEL,
            CardId.YUBEL,
            CardId.FIENDSMITH_ENGRAVER,
        };

        private static readonly HashSet<int> YUBEL_SET = new HashSet<int> { CardId.YUBEL, CardId.YUBEL_TERROR_INCARNATE, CardId.SPIRIT_OF_YUBEL, CardId.PHANTOM_OF_YUBEL };
        
        public YubelExecutor(GameAI ai, Duel duel) : base(ai, duel)
        {

            AddExecutor(ExecutorType.Activate, CardId.NIGHTMARE_THRONE, ActNightmareThroneSearch);
            // ===== Generic counters =====
            AddExecutor(ExecutorType.Activate, _CardId.CalledByTheGrave, CalledbytheGraveActivate);
            AddExecutor(ExecutorType.Activate, _CardId.CrossoutDesignator, CrossoutDesignatorActivate);
            AddExecutor(ExecutorType.Activate, _CardId.InfiniteImpermanence, InfiniteImpermanenceActivate);
            AddExecutor(ExecutorType.Activate, _CardId.AshBlossom, AshBlossomActivate);
            AddExecutor(ExecutorType.Activate, _CardId.MaxxC, MaxxCActivate);
            
            // ===== Yubel-related =====
            AddExecutor(ExecutorType.Activate, CardId.SAMSARA_D_LOTUS, ActSamsaraDLotusGY);
            AddExecutor(ExecutorType.Activate, CardId.YUBEL);
            AddExecutor(ExecutorType.Activate, CardId.UNCHAINDEDABOMINATION, UnchainedAbominationActivate);
            AddExecutor(ExecutorType.Activate, CardId.PHANTOM_OF_YUBEL, DontSelfNG);


            // ===== Engine pieces =====
            AddExecutor(ExecutorType.Activate, CardId.ABOMINABLE_CHAMBER, ActAbo);
            //AddExecutor(ExecutorType.Activate, CardId.SP_LITTLE_KNIGHT, ActLittleKnightSM);
            AddExecutor(ExecutorType.Activate, CardId.SP_LITTLE_KNIGHT, ActLittleKnight);
            AddExecutor(ExecutorType.Activate, CardId.DDD_WAVE_HIGH_KING_CAESAR, DontSelfNG);
            AddExecutor(ExecutorType.Activate, CardId.FIENDSMITHS_DESIRAE, ActDesirae);
            AddExecutor(ExecutorType.Activate, CardId.VARUDASN_FINAL_BRINGER, ActVarudras);
            AddExecutor(ExecutorType.Activate, CardId.UNCHAINED_SOUL_OF_RAGE, ActRageQuickLink);
            AddExecutor(ExecutorType.Activate, CardId.FIENDSMITHS_PARADISE, ActParadise);

            AddExecutor(ExecutorType.Activate, CardId.FIENDSMITH_ENGRAVER, ActEngraverHand);
            AddExecutor(ExecutorType.Activate, CardId.FIENDSMITH_TRACT, ActTract);
            AddExecutor(ExecutorType.SpSummon, CardId.FABLED_LURRIE);
            AddExecutor(ExecutorType.SpSummon, CardId.FIENDSMITHS_REQUIEM, SSRequiem);
            AddExecutor(ExecutorType.Activate, CardId.FIENDSMITHS_REQUIEM, ActRequiemMZ);
            AddExecutor(ExecutorType.Activate, CardId.LACRIMA_CT, ActLacimaCT);
            AddExecutor(ExecutorType.Activate, CardId.FIENDSMITHS_REQUIEM, ActRequiemEQ);
            AddExecutor(ExecutorType.SpSummon, CardId.NECROQUIP, SSNecroquip);
            AddExecutor(ExecutorType.Activate, CardId.FIENDSMITH_ENGRAVER, ActEngraverGY);
            AddExecutor(ExecutorType.SpSummon, CardId.DDD_WAVE_HIGH_KING_CAESAR);

            AddExecutor(ExecutorType.Activate, CardId.LACRIMA_CT, ActLacimaCTGY);

            // Field & search line
            AddExecutor(ExecutorType.Activate, S1_ActivateTerraformingForThrone);
            AddExecutor(ExecutorType.Activate, CardId.NIGHTMARE_THRONE, S6_ChainThroneFollowUp);

            // Normal Summon engine
            AddExecutor(ExecutorType.Summon, CardId.DARK_BECKONING_BEAST, NSDarkBeckoningBeast);
            AddExecutor(ExecutorType.Activate, CardId.DARK_BECKONING_BEAST, ActDarkBeckoningBeast);

            // === SPIRIT GATES ===
            AddExecutor(ExecutorType.Activate, CardId.SPIRIT_GATES, S4_ActivateSpiritGates);   // ใช้จากมือเพื่อค้น
            AddExecutor(ExecutorType.Activate, CardId.SPIRIT_GATES, Gate_RecycleContinuous);   // e3: เก็บ Continuous (มีเลเวล 10)


            AddExecutor(ExecutorType.Summon, CardId.SAMSARA_D_LOTUS, NSSamsaraDLotus);
            AddExecutor(ExecutorType.Activate, CardId.SAMSARA_D_LOTUS, ActSamsaraDLotus);

            // Follow-ups
            AddExecutor(ExecutorType.SpSummon, CardId.SPIRIT_OF_YUBEL);
            AddExecutor(ExecutorType.Activate, CardId.SPIRIT_OF_YUBEL);
            AddExecutor(ExecutorType.Activate, CardId.NIGHTMARE_PAIN, ActNightmarePainHand);
            AddExecutor(ExecutorType.Activate, CardId.NIGHTMARE_PAIN, ActNightmarePainEffect);
            AddExecutor(ExecutorType.Activate, CardId.GRUESOME_GRAVE_SQUIRMER, SSGGS);
            AddExecutor(ExecutorType.Activate, CardId.GRUESOME_GRAVE_SQUIRMER, ActGGSGY);

            AddExecutor(ExecutorType.SpSummon, CardId.MOON_OF_THE_CLOSED_HEAVEN, SSMoon);
            AddExecutor(ExecutorType.SpSummon, CardId.UNCHAINED_LORD_OF_YAMA, L2YamaSetup);
            AddExecutor(ExecutorType.Activate, CardId.UNCHAINED_LORD_OF_YAMA, ActYamaMZ);
            AddExecutor(ExecutorType.Activate, CardId.UNCHAINED_LORD_OF_YAMA, ActYamaGY);
            AddExecutor(ExecutorType.SpSummon, CardId.UNCHAINED_SOUL_OF_RAGE, L2RageKeepYama);
            AddExecutor(ExecutorType.SpSummon, CardId.UNCHAINDEDABOMINATION, L4ABOSS);
            AddExecutor(ExecutorType.Activate, CardId.SHARVARA, ActSharvara);
            AddExecutor(ExecutorType.Activate, CardId.SHARVARA, ActSharvaraGY);
            //AddExecutor(ExecutorType.SpSummon, CardId.MUCKRAKER_UNDERWORLD, L2NoBrain);



            // Panic line
            //AddExecutor(ExecutorType.SpSummon, CardId.VARUDASN_FINAL_BRINGER, SSVarudras);
            AddExecutor(ExecutorType.SpSummon, CardId.VARUDASN_FINAL_BRINGER);
            AddExecutor(ExecutorType.SpSummon, CardId.SALAMANGREAT_ALMIRAJ, AlmirajSummon);
            AddExecutor(ExecutorType.Activate, CardId.SPIRIT_GATES, Gate_Revive00Fiend);       // e2: ทิ้งมือ ชุบ Fiend 0/0
            AddExecutor(ExecutorType.SpSummon, CardId.PHANTOM_OF_YUBEL, SSPhantom);
            AddExecutor(ExecutorType.SpSummon, CardId.GUSTAVMAX);
            AddExecutor(ExecutorType.Activate, CardId.GUSTAVMAX);
            AddExecutor(ExecutorType.SpSummon, CardId.JUGGERNAUT);
            AddExecutor(ExecutorType.Activate, CardId.JUGGERNAUT);
            //AddExecutor(ExecutorType.Activate, CardId.MUCKRAKER_UNDERWORLD, NeverUseMuckrakerRevive);

            // other
            AddExecutor(ExecutorType.SpellSet, SpellSetCheck);
            AddExecutor(ExecutorType.Repos, MonsterRepos);
        }
        //======================Default code
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

        private enum ThroneStage { None, Searching, AwaitDestroyPrompt }
        private bool _yubelWantsTribute = false;
        private ThroneStage _throneStage = ThroneStage.None;
        // === Spirit Gates state ===
        int _gateReviveTargetId = 0;      // จะชุบตัวไหน
        int _gateDiscardPreferredId = 0;  // จะทิ้งใบไหนเป็น cost
        bool _gateWantsRecycle = false;   // กำลังจะกดโหมดเก็บ Continuous
        bool _spQuickMode = false;
        bool moonSummoned = false;
        bool requiemSummoned = false;
        bool thronePending = false;      // we're in a Throne activation flow
        bool throneSearched = false;     // after we chose the monster to search
        int throneDesiredPick = 0;       // preferred monster id to search

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

        private bool MonsterRepos()
        {
            bool isYubelFam =
                Card.Id == CardId.SPIRIT_OF_YUBEL ||
                Card.Id == CardId.YUBEL ||
                Card.Id == CardId.YUBEL_TERROR_INCARNATE ||
                Card.Id == CardId.PHANTOM_OF_YUBEL;

            if (isYubelFam)
            {
                if (Card.IsDefense())
                {
                    AI.SelectPosition(CardPosition.Attack);
                    return true;
                }
                return false;
            }
            if (Card.IsFacedown())
                return true;
            if (CheckInDanger() && (_totalAttack > _totalBotAttack))
                return Card.IsDefense();
            return DefaultMonsterRepos();
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

        public override int OnSelectPlace(int cardId, int player, CardLocation location, int available)
        {
            if (player == 0 && location == CardLocation.MonsterZone && cardId == CardId.UNCHAINED_SOUL_OF_RAGE)
            {
                int prefer = (GetMyLinkedMMZMask() & available) & 0x1F;

                int choose = (prefer != 0) ? LowestBit(prefer)
                                           : LowestBit(available & 0x1F); // fallback

                AI.SelectPlace(choose);
                return choose;
            }
            SelectSTPlace(Card, true);
            return base.OnSelectPlace(cardId, player, location, available);
        }

        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            if (positions == null || positions.Count == 0)
                return base.OnSelectPosition(cardId, positions);

            bool isYubelFamily =
                YUBEL_SET.Contains(cardId) ||
                (Card != null && YUBEL_SET.Contains(Card.Id)) ||
                (Card != null && (Card.Name?.Contains("Yubel") ?? false));

            if(!isYubelFamily)
                return base.OnSelectPosition(cardId, positions);

            CardPosition atkPref =
                positions.Contains(CardPosition.FaceUpAttack) ? CardPosition.FaceUpAttack :
                positions.Contains(CardPosition.Attack) ? CardPosition.Attack :
                (CardPosition)0;

            if (isYubelFamily && atkPref != 0)
            {
                AI.SelectPosition(atkPref);
                return atkPref;
            }

            var chosen = positions[0];
            AI.SelectPosition(chosen);
            return chosen;
        }

        public bool AshBlossomActivate()
        {
            if (BlockIfThrone("AshBlossom")) return false;//added
            if (InThroneFlow) return false;//added
            if (CheckWhetherNegated(true) || !CheckLastChainShouldNegated()) return false;
            if (Duel.LastChainPlayer == 1 && Util.GetLastChainCard().IsCode(_CardId.MaxxC))
            {
                if (CheckAtAdvantage())
                {
                    return false;
                }
            }
            return DefaultAshBlossomAndJoyousSpring();
        }

        public bool MaxxCActivate()
        {
            if (BlockIfThrone("MaxxC")) return false;
            if (InThroneFlow) return false;//added
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
            if (CheckWhetherNegated() || !CheckLastChainShouldNegated()) return false;
            if (CheckAtAdvantage() && Duel.LastChainPlayer == 1 && Util.GetLastChainCard().IsCode(_CardId.MaxxC))
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
                    if (Util.GetLastChainCard().IsCode(_CardId.MaxxC) && CheckAtAdvantage())
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
            if (Card.Id == CardId.FIENDSMITHS_PARADISE) return false;

            if (Duel.Phase == DuelPhase.Main1 && Bot.HasAttackingMonster() && Duel.Turn > 1) return false;
            List<int> onlyOneSetList = new List<int> { CardId.ABOMINABLE_CHAMBER };
            if (onlyOneSetList.Contains(Card.Id) && Bot.HasInSpellZone(Card.Id))
            {
                return false;
            }

            if ((Card.IsTrap() || Card.HasType(CardType.QuickPlay)))//added
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
                return true;
            }

            return false;
        }

        public List<ClientCard> GetDangerousCardinEnemyGrave(bool onlyMonster = false)
        {
            List<ClientCard> result = Enemy.Graveyard.GetMatchingCards(card =>
                (!onlyMonster || card.IsMonster()) && (card.HasSetcode(SetcodeOrcust) || card.HasSetcode(SetcodePhantom) || card.HasSetcode(SetcodeHorus))).ToList();
            List<int> dangerMonsterIdList = new List<int> { 99937011, 63542003, 9411399, 28954097, 30680659 };
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
        #endregion

        public override void OnChainSolved(int chainIndex)
        {
            ClientCard currentCard = Duel.GetCurrentSolvingChainCard();
            var solving = Duel.GetCurrentSolvingChainCard();
            bool neg = Duel.IsCurrentSolvingChainNegated();
            Logger.DebugWriteLine($"[CHAIN] Solved idx={chainIndex} negated={neg} solving={CardStr(solving)}");
            if (currentCard != null && !Duel.IsCurrentSolvingChainNegated() && currentCard.Controller == 1)
            {
                if (currentCard.IsCode(_CardId.MaxxC)) enemyActivateMaxxC = true;
                if (currentCard.IsCode(CardId.Fuwalos)) enemyActivateMaxxC = true;
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
            }
        }
        public override void OnChainEnd()
        {
            escapeTargetList.Clear();
            currentNegateCardList.Clear();
            currentDestroyCardList.Clear();
            enemyActivateInfiniteImpermanenceFromHand = false;
            for (int idx = enemyPlaceThisTurn.Count - 1; idx >= 0; idx--)
            {
                ClientCard checkTarget = enemyPlaceThisTurn[idx];
                if (checkTarget == null || (checkTarget.Location != CardLocation.SpellZone && checkTarget.Location != CardLocation.MonsterZone))
                {
                    enemyPlaceThisTurn.RemoveAt(idx);
                }
            }
            if (thronePending && _throneStage == ThroneStage.None)
            {    thronePending = false; }
            ResetThroneFlow();
            Logger.DebugWriteLine("[CHAIN] OnChainEnd");
            base.OnChainEnd();
        }
        private void ResetThroneFlow()
        {
            Logger.DebugWriteLine($"[THRONE] Reset flow (was stage={_throneStage}, pending={thronePending})");
            thronePending = false;
            throneSearched = false;
            throneDesiredPick = 0;
            _throneStage = ThroneStage.None;
        }
        public override void OnNewTurn()
        {
            if (Duel.Turn <= 1) { dimensionShifterCount = 0; }

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

            // reset Throne flow
            thronePending = false;
            throneSearched = false;
            throneDesiredPick = 0;
            _gateReviveTargetId = 0;
            _gateDiscardPreferredId = 0;
            _gateWantsRecycle = false;
            _spQuickMode = false;
            
            base.OnNewTurn();
        }

        // ===== Material safety logic (inspired by windbot patterns) =====
        private static readonly HashSet<int> NEVER_SAC = new HashSet<int>
        {
            CardId.PHANTOM_OF_YUBEL,
            CardId.DDD_WAVE_HIGH_KING_CAESAR,
            CardId.VARUDASN_FINAL_BRINGER,
            CardId.SP_LITTLE_KNIGHT,
            CardId.GORGONOFZIL,
            CardId.UNCHAINDEDABOMINATION
        };
        private bool InThroneFlow => thronePending || _throneStage != ThroneStage.None;
        private int PriorityIndex(int id)
        {
            int idx = Array.IndexOf(LinkFodderPriority, id);
            return idx >= 0 ? idx : 999;
        }
        private bool IsProtectedMaterial(ClientCard c, bool allowUseYubelForLink = false)
        {
            if (c == null) return true;

            if (NEVER_SAC.Contains(c.Id)) return true;

            if (c.EquipCards != null && c.EquipCards.Count > 0) return true;
            if (c.HasType(CardType.Link) && c.LinkCount >= 2) return true;
            if (c.HasType(CardType.Fusion | CardType.Synchro | CardType.Xyz)) return true;

            return false;
        }
        private ClientCard[] GetSafeMaterials(int need)
        {
            bool allowYubel = ShouldUseYubelForLink();

            var list = Bot.GetMonsters()
                .Where(m => !IsProtectedMaterial(m, allowYubel))
                .OrderBy(m => PriorityIndex(m.Id))
                .ThenBy(m => m.Attack)
                .Take(need)
                .ToArray();

            return list;
        }
        private bool ShouldUseYubelForLink()
        {
            if (Duel.Player != 0) return false; // ทำเฉพาะเทิร์นเรา
            var mons = Bot.GetMonsters().Where(m => m != null).ToList();
            if (mons.Count == 0) return false;

            bool twoOrLess = mons.Count <= 2;

            // มีลิงก์ที่อัปเกรดบอร์ดได้
            bool canMakeBetter;

            if (HasInExtra(CardId.FIENDSMITHS_REQUIEM))
            {
                canMakeBetter =
                HasInExtra(CardId.MOON_OF_THE_CLOSED_HEAVEN) ||
                HasInExtra(CardId.UNCHAINED_SOUL_OF_RAGE) ||
                HasInExtra(CardId.UNCHAINED_LORD_OF_YAMA) ||

                //HasInExtra(CardId.MUCKRAKER_UNDERWORLD) ||
                //HasInExtra(CardId.FIENDSMITHS_SEQUENCE) ||
                HasInExtra(CardId.SP_LITTLE_KNIGHT);
                //HasInExtra(CardId.GORGONOFZIL) ;
            }
            else
            {
                canMakeBetter =
                HasInExtra(CardId.UNCHAINED_SOUL_OF_RAGE) ||
                HasInExtra(CardId.UNCHAINED_LORD_OF_YAMA) ||
                //HasInExtra(CardId.MUCKRAKER_UNDERWORLD) ||
                //HasInExtra(CardId.FIENDSMITHS_SEQUENCE) ||
                HasInExtra(CardId.SP_LITTLE_KNIGHT);
                //HasInExtra(CardId.GORGONOFZIL);
            }

            // ถ้าบนสนามเหลือแต่ชิ้น Yubel 1-2 ใบ และสามารถอัปเกรดเป็นลิงก์ที่ให้ interrupt/มูฟได้ ⇒ ใช้ไปลิงก์เถอะ
            return twoOrLess && canMakeBetter;
        }
        private static readonly int[] YubelCostPriority = new[]
        {
            CardId.SAMSARA_D_LOTUS,
            CardId.FABLED_LURRIE,
            CardId.DARK_BECKONING_BEAST,
            CardId.CHAOS_SUMMONING_BEAST,
            CardId.GRUESOME_GRAVE_SQUIRMER,
            CardId.SALAMANGREAT_ALMIRAJ,
            //CardId.FIENDSMITHS_SEQUENCE
        };
        private bool HasInExtra(int id)
        {
            return Bot.ExtraDeck.Any(c => c != null && c.Id == id);
        }
        // --- State for Nightmare Throne prompt flow ---
        

        #region Work Space #1
        private bool DontSelfNG() { return Duel.LastChainPlayer != 0; }
        private int LowestBit(int mask) => mask & -mask;

        // เอา mask ของช่องมอนสเตอร์ที่ลิงก์เราชี้ (เฉพาะ Main Monster Zone 0..4)
        private int GetMyLinkedMMZMask()
        {
            int mask = 0;
            foreach (var m in Bot.GetMonsters())
            {
                if (m == null || !m.IsFaceup() || !m.HasType(CardType.Link)) continue;
                mask |= m.GetLinkedZones();
            }
            // เอาเฉพาะ 5 โซนหลัก (บิต 0..4)
            mask &= 0x1F;
            return mask;
        }
        private bool S1_ActivateTerraformingForThrone()
        {
            if (Type != ExecutorType.Activate) return false;
            if (Card.Id != CardId.TERRAFORMING) return false;
            if (Bot.HasInHandOrInSpellZone(CardId.NIGHTMARE_THRONE)) { return false; }
            AI.SelectCard(CardId.NIGHTMARE_THRONE);
            return true;
        }

        private bool ActNightmareThroneSearch()
        {
            if (Type != ExecutorType.Activate) return false;
            if (Card.Location == CardLocation.Hand && Bot.HasInSpellZone(CardId.NIGHTMARE_THRONE)) return false;

            int pick = 0;
            if (!Bot.HasInHand(CardId.SAMSARA_D_LOTUS) && CheckRemainInDeck(CardId.SAMSARA_D_LOTUS) > 0)
                pick = CardId.SAMSARA_D_LOTUS;
            else if (Bot.HasInHand(CardId.SAMSARA_D_LOTUS) && !Bot.HasInHand(CardId.DARK_BECKONING_BEAST) && CheckRemainInDeck(CardId.DARK_BECKONING_BEAST) > 0)
                pick = CardId.DARK_BECKONING_BEAST;
            else if (Bot.HasInHand(CardId.SAMSARA_D_LOTUS) && Bot.HasInHand(CardId.DARK_BECKONING_BEAST) && !Bot.HasInHand(CardId.CHAOS_SUMMONING_BEAST) && CheckRemainInDeck(CardId.CHAOS_SUMMONING_BEAST) > 0)
                pick = CardId.CHAOS_SUMMONING_BEAST;

            thronePending = true;
            throneSearched = false;
            throneDesiredPick = pick;

            Logger.DebugWriteLine($"[THRONE] Activate search; desiredPick={(pick == 0 ? "(auto)" : pick.ToString())}");
            DumpChain("ThroneActivate");

            return true;
        }

        private bool NSDarkBeckoningBeast()
        {
            if (Duel.Phase != DuelPhase.Main1) return false;
            if (Bot.HasInMonstersZone(CardId.DARK_BECKONING_BEAST))
            {
                if (Bot.HasInHand(CardId.SAMSARA_D_LOTUS))
                {
                    return false;
                }
            }
            return true;
        }

        private bool ActDarkBeckoningBeast()
        {
            if (Duel.Phase != DuelPhase.Main1) return false;
            if ((CheckRemainInDeck(CardId.SPIRIT_GATES) > 0) && !Bot.HasInSpellZone(CardId.SPIRIT_GATES))
            { AI.SelectCard(CardId.SPIRIT_GATES); return true; }
            else if (CheckRemainInDeck(CardId.CHAOS_SUMMONING_BEAST) > 0)
            { AI.SelectCard(CardId.CHAOS_SUMMONING_BEAST); return true; }
            else { return false; }
        }

        private bool S4_ActivateSpiritGates()
        {
            // ใช้จาก "มือ" เพื่อค้น DBB/CSB
            if (Type != ExecutorType.Activate) return false;
            if (Card.Location != CardLocation.Hand) return false;
            if (Bot.HasInSpellZone(CardId.SPIRIT_GATES, true, true)) return false;

            // กฎ: ถ้ามี DBB อยู่ "บนสนาม" ให้ค้น CSB
            int pick = 0;
            bool dbbOnField = Bot.HasInMonstersZone(CardId.DARK_BECKONING_BEAST, false, false, true);

            if (dbbOnField && CheckRemainInDeck(CardId.CHAOS_SUMMONING_BEAST) > 0)
                pick = CardId.CHAOS_SUMMONING_BEAST;
            else if (CheckRemainInDeck(CardId.DARK_BECKONING_BEAST) > 0)
                pick = CardId.DARK_BECKONING_BEAST;
            else if (CheckRemainInDeck(CardId.CHAOS_SUMMONING_BEAST) > 0)
                pick = CardId.CHAOS_SUMMONING_BEAST;

            if (pick == 0) return false;

            AI.SelectCard(pick);
            return true;
        }
        private bool Gate_RecycleContinuous()
        {
            // e3: มีเลเวล 10 -> เก็บ Continuous Spell จากสุสาน (เน้นเก็บ NIGHTMARE_PAIN)
            if (Card.Location != CardLocation.SpellZone) return false;
            if (!HaveFaceupLevel10()) return false;

            if (!Bot.HasInGraveyard(CardId.NIGHTMARE_PAIN))
                return false;

            _gateWantsRecycle = true;
            // เลือก Pain ก่อน ถ้าไม่มีค่อยเลือก Paradise ตอน select card
            return true;
        }
        private bool Is00FiendId(int id)
        {
            // รายชื่อ 0/0 Fiend ที่เราใช้ในเด็คนี้
            return id == CardId.YUBEL
                || id == CardId.SPIRIT_OF_YUBEL
                || id == CardId.DARK_BECKONING_BEAST
                || id == CardId.CHAOS_SUMMONING_BEAST
                || id == CardId.SAMSARA_D_LOTUS;
        }

        // เลือกเป้าหมายที่จะชุบ ด้วยลำดับความสำคัญตาม 2.2.x
        private int PlanSpiritGatesReviveTarget()
        {
            // Emergency Case
            if (Bot.HasInMonstersZone(CardId.SALAMANGREAT_ALMIRAJ)&&Bot.HasInGraveyard(CardId.DARK_BECKONING_BEAST))
            { return CardId.DARK_BECKONING_BEAST; }

            // 2.2.2: ถ้าขาด spirit → SS spirit (จากสุสาน หรือทิ้งจากมือ)
            bool spiritOnBoard = Bot.HasInMonstersZone(CardId.SPIRIT_OF_YUBEL, true);
            bool spiritInGY = Bot.HasInGraveyard(CardId.SPIRIT_OF_YUBEL);
            bool spiritInHand = Bot.HasInHand(CardId.SPIRIT_OF_YUBEL);
            if (!spiritOnBoard && (spiritInGY || spiritInHand))
                return CardId.SPIRIT_OF_YUBEL;

            // 2.2.3: ถ้า DBB โดนขัด/ตันเกมแล้วมี Almiraj → ชุบ DBB เพื่อไต่ไป Moon
            if (Bot.HasInMonstersZone(CardId.SALAMANGREAT_ALMIRAJ, true)
                && Bot.HasInGraveyard(CardId.DARK_BECKONING_BEAST)
                && HasInExtra(CardId.MOON_OF_THE_CLOSED_HEAVEN))
                return CardId.DARK_BECKONING_BEAST;

            // 2.2.1: ถ้าขาด lotus → SS lotus
            if (!Bot.HasInMonstersZone(CardId.SAMSARA_D_LOTUS, true) && Bot.HasInGraveyard(CardId.SAMSARA_D_LOTUS))
                return CardId.SAMSARA_D_LOTUS;

            // ทางเลือกทั่วไป: DBB > CSB > Yubel
            if (Bot.HasInGraveyard(CardId.DARK_BECKONING_BEAST)) return CardId.DARK_BECKONING_BEAST;
            if (Bot.HasInGraveyard(CardId.CHAOS_SUMMONING_BEAST)) return CardId.CHAOS_SUMMONING_BEAST;
            if (Bot.HasInGraveyard(CardId.YUBEL)) return CardId.YUBEL;

            // ถ้าไม่มีอะไรชุบได้เลย อาจวางแผน discard ให้ Spirit ลงสุสานแล้วค่อยชุบ
            if (!spiritOnBoard && spiritInHand) return CardId.SPIRIT_OF_YUBEL;

            return 0;
        }

        // เลือกใบจะทิ้ง: CSB > Paradise > Terror > (บางกรณี) Spirit
        private int PickSpiritGatesDiscard(int reviveTargetId)
        {
            // เคสอยากชุบ Spirit แต่ยังอยู่ในมือ → ทิ้ง Spirit เป็น cost แล้วชุบมันเอง
            if (reviveTargetId == CardId.SPIRIT_OF_YUBEL && Bot.HasInHand(CardId.SPIRIT_OF_YUBEL))
                return CardId.SPIRIT_OF_YUBEL;

            if (Bot.HasInHand(CardId.FIENDSMITHS_PARADISE)) return CardId.FIENDSMITHS_PARADISE;
            if (Bot.HasInHand(CardId.CHAOS_SUMMONING_BEAST)) return CardId.CHAOS_SUMMONING_BEAST;
            if (Bot.HasInHand(CardId.YUBEL_TERROR_INCARNATE)) return CardId.YUBEL_TERROR_INCARNATE;

            // fallback: เลือกใบที่ "ทิ้งแล้วเจ็บน้อยสุด"
            var hand = Bot.Hand.ToList();
            hand.Sort((a, b) => ScoreOwnCardForCost(a).CompareTo(ScoreOwnCardForCost(b)));
            return hand.Count > 0 ? hand[0].Id : 0;
        }

        private bool Gate_Revive00Fiend()
        {
            // e2: ทิ้งมือ 1 → SS Fiend 0/0 จากสุสาน
            if (Card.Location != CardLocation.SpellZone) return false;
            
            if (requiemSummoned)
            {
                if (!Bot.HasInHandOrInGraveyard(CardId.SPIRIT_OF_YUBEL)) return false; // ต้องมีในหลุม
                if (Bot.Hand.Count <= 0) return false;                          // ต้องมีใบทิ้ง
                _gateReviveTargetId = CardId.SPIRIT_OF_YUBEL;
                _gateDiscardPreferredId = PickSpiritGatesDiscard(_gateReviveTargetId);
                _gateWantsRecycle = false;
                return true;
            }

            // วางแผนก่อนว่าจะชุบตัวไหน
            int target = PlanSpiritGatesReviveTarget();
            if (target == 0) return false;

            // ต้องมีการ์ดให้ทิ้งอย่างน้อย 1 ใบ
            if (Bot.Hand.Count <= 0) return false;

            int discard = PickSpiritGatesDiscard(target);
            if (discard == 0) return false;

            _gateReviveTargetId = target;
            _gateDiscardPreferredId = discard;
            _gateWantsRecycle = false;

            // ไม่เลือกอะไรตรงนี้ ปล่อยให้ OnSelectCard จิ้ม cost/เป้าหมายให้
            return true;
        }

        private bool HaveFaceupLevel10()
        {
            return Bot.MonsterZone.Any(m => m != null && m.IsFaceup() && m.Level == 10);
        }
        private bool ActNightmarePainHand()
        {
            if (Bot.HasInSpellZone(CardId.NIGHTMARE_PAIN, true, true)) return false;
            if (Card.Location == CardLocation.Hand) return true;
            return false;
        }

        private bool ActNightmarePainEffect()
        {
            if (Card.Location == CardLocation.SpellZone)
            {
                if(CheckRemainInDeck(CardId.GRUESOME_GRAVE_SQUIRMER)==0)return false;

                if (Bot.HasInMonstersZone(CardId.SPIRIT_OF_YUBEL) || Bot.HasInHand(CardId.SPIRIT_OF_YUBEL))
                {
                    AI.SelectCard(CardId.SPIRIT_OF_YUBEL);
                    AI.SelectNextCard(CardId.GRUESOME_GRAVE_SQUIRMER);
                    return true;
                }
            }
            return false;
        }

        private bool S6_ChainThroneFollowUp()
        {
            if (Type != ExecutorType.Activate) return false;
            if (sendToGYThisTurn.Any(c => c != null && YUBEL_SET.Contains(c.Id)) && Bot.HasInHand(CardId.NIGHTMARE_THRONE))
            {
                AI.SelectYesNo(true);
                return true;
            }
            return false;
        }

        private bool NSSamsaraDLotus()
        {
            if (Bot.HasInMonstersZone(CardId.SPIRIT_OF_YUBEL)) return false;
            return true;
        }

        private bool ActSamsaraDLotus()
        {
            if (Duel.Player == 0)
            {
                if (Card.Location == CardLocation.MonsterZone)
                {
                    AI.SelectCard(CardId.SPIRIT_OF_YUBEL);
                    return true;
                }
            }
            if (Duel.Player == 1)
            {
                if (Bot.HasInMonstersZone(CardId.YUBEL) || Bot.HasInMonstersZone(CardId.SPIRIT_OF_YUBEL))
                {
                    return true;
                }
            }
            return false;
        }
        private bool ActSamsaraDLotusGY()
        {
            if (Card.Location == CardLocation.Grave)
            {
                if (Bot.HasInMonstersZone(CardId.YUBEL))
                { 
                    AI.SelectOption(1);
                    return true;
                }
            }
            return false;
        }

        private bool ActTract()
        {
            if (Card.Location != CardLocation.Hand) return false;

            // ถ้าเข้า Fiendsmith line แล้ว -> ไม่ใช้ Tract
            if (requiemSummoned) return false;
            AI.SelectCard(CardId.FABLED_LURRIE);
            AI.SelectNextCard(CardId.FABLED_LURRIE);
            return true;
            /*if (Card.Location == CardLocation.Hand)
            {
                if (CheckRemainInDeck(CardId.FIENDSMITH_ENGRAVER) == 0 &&
                    CheckRemainInDeck(CardId.LACRIMA_CT) == 0)
                {
                    return false;
                }
                if(CheckRemainInDeck(CardId.FABLED_LURRIE) == 0){ return false; }
                AI.SelectCard(CardId.FABLED_LURRIE);
                AI.SelectNextCard(CardId.FABLED_LURRIE);
                return true;
            }
            if (Card.LastLocation == CardLocation.Grave) { return false; }
            return false;*/
        }

        private bool ActParadise()
        {
            if (Card.Location != CardLocation.Grave) return false;
            if (Bot.HasInMonstersZoneOrInGraveyard(CardId.FIENDSMITHS_DESIRAE) || Bot.HasInBanished(CardId.FIENDSMITHS_DESIRAE)) return false;
            AI.SelectCard(CardId.FIENDSMITHS_DESIRAE);
            return DontSelfNG();
        }

        private bool ActDesirae()
        {
            if (Card.Location != CardLocation.Grave) {return false; }
            ClientCard target = GetBestEnemyCard(onlyFaceup: true, canBeTarget: true, checkGrave: false);
            if (target == null) return false;
            if (Bot.HasInGraveyard(CardId.FIENDSMITHS_REQUIEM))
            {
                AI.SelectCard(CardId.FIENDSMITHS_REQUIEM);
                AI.SelectNextCard(target);
                return true;
            }
            AI.SelectCard(target);
            return true;
        }

        private bool ActRequiemMZ()
        {
            if (Card.Location != CardLocation.MonsterZone) { return false; }
            if (Bot.HasInHand(CardId.LACRIMA_CT) || CheckRemainInDeck(CardId.LACRIMA_CT) > 0)
            { 
                AI.SelectCard(CardId.LACRIMA_CT);
                AI.SelectPosition(CardPosition.FaceUpDefence);
                return true; 
            }
            else if (Bot.HasInHand(CardId.FIENDSMITH_ENGRAVER) || CheckRemainInDeck(CardId.FIENDSMITH_ENGRAVER) > 0)
            {
                AI.SelectCard(CardId.FIENDSMITH_ENGRAVER);
                AI.SelectPosition(CardPosition.FaceUpDefence);
                return true;
            }
            return false;
        }

        private bool ActRequiemEQ()
        {
            if (!HasInExtra(CardId.NECROQUIP)) { return false; }
            if (Card.Location != CardLocation.Grave) { return false; }
            if (Bot.HasInMonstersZone(CardId.LACRIMA_CT))
            {   
                AI.SelectCard(CardId.LACRIMA_CT);
                return true; 
            }
            else if (Bot.HasInMonstersZone(CardId.FIENDSMITH_ENGRAVER))
            {
                AI.SelectCard(CardId.FIENDSMITH_ENGRAVER);
                return true;
            }
            return false;
        }

        private bool SSNecroquip()
        {
            if (Bot.HasInSpellZone(CardId.FIENDSMITHS_REQUIEM) && Bot.HasInMonstersZone(CardId.LACRIMA_CT))
            {
                AI.SelectCard(CardId.FIENDSMITHS_REQUIEM);
                AI.SelectNextCard(CardId.LACRIMA_CT);
                return true;
            }
            else if (Bot.HasInSpellZone(CardId.FIENDSMITHS_REQUIEM) && Bot.HasInMonstersZone(CardId.FIENDSMITH_ENGRAVER))
            {
                AI.SelectCard(CardId.FIENDSMITHS_REQUIEM);
                AI.SelectNextCard(CardId.FIENDSMITH_ENGRAVER);
                return true;
            }
            return false;
        }

        private bool ActLacimaCT()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {
                if (!Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.FIENDSMITH_ENGRAVER) && !Bot.HasInBanished(CardId.FIENDSMITH_ENGRAVER))
                { AI.SelectCard(CardId.FIENDSMITH_ENGRAVER); return true; }
                else if (!Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.FIENDSMITHS_PARADISE) && !Bot.HasInBanished(CardId.FIENDSMITHS_PARADISE))
                { AI.SelectCard(CardId.FIENDSMITHS_PARADISE); return true; }
                return false;
            }
            return false;
        }

        private bool ActLacimaCTGY()
        {
            if (Card.Location == CardLocation.Grave)
            {
                if (Bot.HasInBanished(CardId.FIENDSMITHS_PARADISE) || Bot.HasInHandOrInGraveyard(CardId.FIENDSMITHS_PARADISE))
                { return false; }
                else { return DontSelfNG(); }
            }
            return false;
        }

        /*private bool SSVarudras()
        {
            if (Bot.HasInMonstersZone(CardId.YUBEL) && Bot.HasInMonstersZone(CardId.SPIRIT_OF_YUBEL))
            {
                AI.SelectCard(CardId.YUBEL);
                AI.SelectNextCard(CardId.SPIRIT_OF_YUBEL);
                return true;
            }
            return true;
        }*/

        private bool ActEngraverHand()
        {
            if (Card.Location != CardLocation.Hand) return false;
            if (!Bot.HasInHandOrInSpellZoneOrInGraveyard(CardId.FIENDSMITH_TRACT) && !Bot.HasInBanished(CardId.FIENDSMITH_TRACT))
            {
                AI.SelectCard(CardId.FIENDSMITH_TRACT);
                return true;
            }
            return false;
        }

        private bool ActEngraverGY()
        {
            if (Card.Location != CardLocation.Grave) return false;
            if (Bot.HasInGraveyard(CardId.FABLED_LURRIE)) { AI.SelectCard(CardId.FABLED_LURRIE); return true; }
            else if (Bot.HasInGraveyard(CardId.MOON_OF_THE_CLOSED_HEAVEN)) { AI.SelectCard(CardId.MOON_OF_THE_CLOSED_HEAVEN); return true; }
            return false;
        }

        private bool SSMoon()
        {
            if (moonSummoned){ return false; }
            if (requiemSummoned) { return false; }
            if (!HasInExtra(CardId.FIENDSMITHS_REQUIEM))
                return false;
            var mats = GetSafeMaterials(2);
            if (mats.Length < 2) return false;
            AI.SelectMaterials(mats);
            moonSummoned = true;
            return true;
        }

        private bool ActAbo()
        {
            if (Bot.HasInGraveyard(CardId.UNCHAINED_SOUL_OF_RAGE))
            {
                AI.SelectCard(CardId.UNCHAINED_SOUL_OF_RAGE);
                return true;
            }
            return false;
        }

        private bool AlmirajSummon()
        {
            if (Bot.GetMonsterCount() > 1) return false;
            ClientCard mat = Bot.GetMonsters().First();
            if (mat.IsCode(new[] { CardId.DARK_BECKONING_BEAST }))
            {
                AI.SelectMaterials(mat);
                return true;
            }
            return false;
        }

        private bool SSGGS()
        {
            if (!DontSelfNG()) { return false; }
            if (BlockIfThrone("GGS")) return false;
            if (Duel.Player == 1) { return false; }
            if (InThroneFlow) { return false; }
            if (Card.Location != CardLocation.Hand) { return false; }
            if (Bot.HasInMonstersZone(CardId.SPIRIT_OF_YUBEL))
            {
                AI.SelectYesNo(true);
                AI.SelectCard(CardId.SPIRIT_OF_YUBEL);
                return true; 
            }
            AI.SelectYesNo(false);
            return true;
        }

        private bool ActGGSGY()
        {
            if (Card.Location != CardLocation.Grave) { return false; }
            if (Bot.HasInGraveyard(CardId.SPIRIT_OF_YUBEL))
            {
                AI.SelectCard(CardId.SPIRIT_OF_YUBEL);
                return true;
            }
            else { return false; }
        }
        private bool ActLittleKnight()
        {
            if (ActivateDescription == -1 || ActivateDescription == Util.GetStringId(CardId.SP_LITTLE_KNIGHT, 0))
            {
                // banish card
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
        private bool ActRageQuickLink()
        {
            if (Card.Location != CardLocation.MonsterZone) return false;
            // Quick Link เฉพาะเทิร์นศัตรูช่วง Main เพื่อสร้าง 1 interrupt
            if (Duel.Player != 1) return false;
            if (Duel.Phase < DuelPhase.Main1 || Duel.Phase > DuelPhase.Main2) return false;
            if (!HasValidRageLinkCandidate()) return false;

            var target = GetBestEnemyMonster(onlyFaceup: true, canBeTarget: true);
            if (target == null) return false;

            // ส่วนใหญ่เอนจินจะถามเลือกการ์ดฝั่งตรงข้ามที่จะใช้เป็นวัสดุ/หรือไล่
            AI.SelectCard(target);
            return DontSelfNG();
        }
        private bool SSPhantom()
        {
            var gyMat2Codes = new List<int> { CardId.YUBEL_TERROR_INCARNATE, CardId.YUBEL, CardId.DARK_BECKONING_BEAST, CardId.CHAOS_SUMMONING_BEAST, CardId.SPIRIT_OF_YUBEL };
            if (!Bot.HasInGraveyard(gyMat2Codes))
            {
                return false;
            }
            // select mat 1
            if (Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.YUBEL_TERROR_INCARNATE))
            {
                AI.SelectCard(CardId.YUBEL_TERROR_INCARNATE);
            }
            else if (Bot.HasInMonstersZoneOrInGraveyard(CardId.YUBEL))
            {
                AI.SelectCard(CardId.YUBEL);
            }
            else if (Bot.HasInGraveyard(CardId.SPIRIT_OF_YUBEL))
            {
                AI.SelectCard(CardId.SPIRIT_OF_YUBEL);
            }
            else
            {
                return false;
            }
            // วัตถุดิบใบที่ 2 จากสุสานตามที่จัดไว้
            AI.SelectNextCard(gyMat2Codes);
            return true;
        }

        private bool ActSharvara()
        {
            if (BlockIfThrone("Sharvara")) return false;
            if (Duel.Player == 1) { return false; }
            if (InThroneFlow) return false;
            if (Card.Location != CardLocation.Hand) return false;
            if (Bot.HasInMonstersZone(CardId.SPIRIT_OF_YUBEL))
            {
                AI.SelectCard(CardId.SPIRIT_OF_YUBEL);
                return true;
            }
            else if (Bot.HasInMonstersZone(CardId.YUBEL))
            {
                AI.SelectCard(CardId.YUBEL);
                return true;
            }

            return false;
        }
        private bool ActSharvaraGY()
        {
            if (Card.Location != CardLocation.Grave) return false;
            return true;
        }

        /*private bool L2NoBrain()
        {
            var mats = GetSafeMaterials(2);
            if (mats.Length < 2) return false;

            AI.SelectMaterials(mats);
            return true;
        }*/
        private bool ActVarudras()
        {
            if (CheckWhetherNegated()) return false;

            // รายการเป้า (ศัตรูก่อน ถ้าไม่มีค่อย fallback)
            List<ClientCard> targetList = GetNormalEnemyTargetList(true, true);
            int desc = ActivateDescription;
            int d1 = Util.GetStringId(CardId.VARUDASN_FINAL_BRINGER, 1); // ใช้ทั้ง e1 (Negate) และ e2 (Battle Start destroy)
            int d2 = Util.GetStringId(CardId.VARUDASN_FINAL_BRINGER, 2); // e3 (Destroyed -> destroy 1)

            Logger.DebugWriteLine("[Varudras] desc: " + desc + ", timing = " + CurrentTiming);


            var enemyPick = targetList.FirstOrDefault(c => c != null && c.Controller == 1);

            // e1: Quick effect Negate (ฝั่งคู่ต่อสู้กดเอฟเฟกต์)
            if (desc == d1 && Duel.LastChainPlayer == 1 && Duel.CurrentChain.Count > 0)
            {
                if (!CheckLastChainShouldNegated()) return false;
                activatedCardIdList.Add(Card.Id); // แท็กว่าเป็น e1
                return true; // เอนจินจะจัดการ detach เป็น cost ให้เอง
            }

            // e2: Battle Start ทำลาย 1 / e3: ถูกทำลายแล้วทำลาย 1
            if (desc == d1 || desc == d2 || desc == -1)
            {
                if (targetList.Count == 0) return false;

                // พยายามให้เลือกฝั่งศัตรูก่อน
                if (enemyPick != null)
                    targetList.Insert(0, enemyPick);
                else
                {
                    // ถ้าไม่มีการ์ดฝั่งศัตรูให้เลือกเลย -> แทรกฝั่งเราที่ "เสียหายน้อยสุด" ไว้หัวลิสต์
                    var selfBest =
                        Bot.GetMonsters().Concat(Bot.GetSpells())
                           .Where(c => c != null)
                           .OrderBy(ScoreOwnCardForCost)
                           .FirstOrDefault();
                    if (selfBest != null) targetList.Insert(0, selfBest);
                }

                // log / tag effect ย่อย
                if (desc == d1 && Duel.CurrentChain.Count == 0) activatedCardIdList.Add(Card.Id + 1); // e2
                if (desc == d2) activatedCardIdList.Add(Card.Id + 2); // e3

                AI.SelectCard(targetList);
                return true;
            }

            return false;
        }

        private bool ShouldVarudrasDetachForPop(ClientCard target)
        {
            if (target == null) return false;
            if (target.IsFloodgate() || target.IsMonsterDangerous() || target.IsMonsterInvincible()) return true;
            // ใช้สกอร์เดิม ๆ ที่เรามีเพื่อประเมินความ "คุ้ม" ของการถอดวัตถุดิบแลกกับ 1 ทำลาย
            return ScoreEnemyCardForRemoval(target) >= 3000;
        }
        private bool ActYamaGY()
        {
            if (Card.Location != CardLocation.Grave) return false;
            if (Bot.HasInGraveyard(CardId.SPIRIT_OF_YUBEL)) return false;
            AI.SelectCard(CardId.SPIRIT_OF_YUBEL);
            AI.SelectYesNo(false);
            return true;
        }
        private bool ActYamaMZ()
        {
            if (CheckRemainInDeck(CardId.SHARVARA) == 0 ) return false;
            AI.SelectCard(CardId.SHARVARA);
            return true;
        }
        private bool SSRequiem()
        {
            if (CheckRemainInDeck(CardId.LACRIMA_CT) == 0 && !Bot.HasInHand(CardId.LACRIMA_CT)) { return false; }
            requiemSummoned = true;
            return true;
        }
        private bool L4ABOSS()
        {
            if (!HasInExtra(CardId.UNCHAINDEDABOMINATION)) return false;

            var mons = Bot.GetMonsters();
            var yama = mons.FirstOrDefault(m => m != null && m.Id == CardId.UNCHAINED_LORD_OF_YAMA);
            var rage = mons.FirstOrDefault(m => m != null && m.Id == CardId.UNCHAINED_SOUL_OF_RAGE);
            var yubel = mons.FirstOrDefault(m => m != null && m.Id == CardId.YUBEL);
            var terror = mons.FirstOrDefault(m => m != null && m.Id == CardId.YUBEL_TERROR_INCARNATE);

            if (yama != null && rage != null && IsInEMZ(yama))
            {
                AI.SelectMaterials(new[] { yama, rage });
                return true;
            }

            if (rage != null && yubel != null && terror != null && IsInEMZ(rage))
            {
                AI.SelectMaterials(new[] { rage, yubel, terror });
                return true;
            }

            // 3) greedy: เลือกลิงก์ที่มีอยู่ก่อน (prefer Rage > Yama > อื่น ๆ) แล้วเติมการ์ดจนแต้มรวมครบ 4
            ClientCard firstLink =
                rage ??
                yama ??
                mons.Where(m => m.HasType(CardType.Link)).OrderByDescending(m => m.LinkCount).FirstOrDefault();

            if (firstLink != null)
            {
                // ไม่ใช้ของที่ไม่ควรสังเวย (NEVER_SAC) แต่ยอมใช้ Yubel/Terror ได้
                var pool = mons
                    .Where(m => m != firstLink && !NEVER_SAC.Contains(m.Id))
                    .OrderBy(ScoreOwnCardForCost) // เสียน้อยสุดมาก่อน
                    .ToList();

                var pick = new List<ClientCard> { firstLink };
                int need = 4 - LinkValue(firstLink);

                foreach (var m in pool)
                {
                    pick.Add(m);
                    need -= LinkValue(m);
                    if (need <= 0) break;
                }

                if (need <= 0)
                {
                    AI.SelectMaterials(pick.ToArray());
                    return true;
                }

            }
            return false;
        }
        private int LinkValue(ClientCard c) => (c != null && c.HasType(CardType.Link)) ? Math.Max(1, c.LinkCount) : 1;

        private bool IsInEMZ(ClientCard c)
        {
            var mz = Bot.MonsterZone;
            return (mz.Length > 5 && mz[5] == c) || (mz.Length > 6 && mz[6] == c);
        }
        public bool UnchainedAbominationActivate()
        {
            if (CheckWhetherNegated()) return false;
            List<ClientCard> targetList = GetNormalEnemyTargetList(true, true, CardType.Monster);
            if (targetList.Count() == 0) return false;
            int logDesc = ActivateDescription;
            if (logDesc >= Util.GetStringId(CardId.UNCHAINDEDABOMINATION, 0))
            {
                logDesc = Util.GetStringId(CardId.UNCHAINDEDABOMINATION, 0) - 10;
            }
            Logger.DebugWriteLine("[UnchainedAbomination]desc: " + logDesc + ", timing = " + CurrentTiming);
            if (ActivateDescription == Util.GetStringId(CardId.UNCHAINDEDABOMINATION, 0)) activatedCardIdList.Add(Card.Id);
            if (ActivateDescription == Util.GetStringId(CardId.UNCHAINDEDABOMINATION, 1) || ActivateDescription == -1) activatedCardIdList.Add(Card.Id + 1);
            if (ActivateDescription == Util.GetStringId(CardId.UNCHAINDEDABOMINATION, 2)) activatedCardIdList.Add(Card.Id + 2);
            AI.SelectCard(targetList);

            return true;
        }
        private ClientCard[] GetSafeMaterialsExcluding(HashSet<int> excludeIds, int need)
        {
            return Bot.GetMonsters()
                .Where(m => m != null
                            && (excludeIds == null || !excludeIds.Contains(m.Id))
                            && !IsProtectedMaterial(m)      // ไม่ยอมแลกของสำคัญ (Link≥2 / Extra ฯลฯ)
                       )
                .OrderBy(m => PriorityIndex(m.Id))
                .ThenBy(m => m.Attack)
                .Take(need)
                .ToArray();
        }

        private bool CanMakeRageWithoutYama()
        {
            var mats = GetSafeMaterialsExcluding(new HashSet<int> { CardId.UNCHAINED_LORD_OF_YAMA }, 2);
            return mats.Length >= 2;
        }
        private bool L2YamaSetup()
        {
            var mats = GetSafeMaterials(2);
            if (mats.Length < 2) return false;
            AI.SelectMaterials(mats);
            return true;
        }
        private bool L2RageKeepYama()
        {
            // ต้องมี Yama อยู่ก่อน และต้องมีวัตถุดิบอื่น 2 ใบ (ไม่นับ Yama)
            if (!Bot.HasInMonstersZone(CardId.UNCHAINED_LORD_OF_YAMA, true)) return false;
            if (!CanMakeRageWithoutYama()) return false;

            var mats = GetSafeMaterialsExcluding(new HashSet<int> { CardId.UNCHAINED_LORD_OF_YAMA }, 2);
            if (mats.Length < 2) return false;  // ยังไม่พอ → รอก่อน อย่าฝืนใช้ Yama

            AI.SelectMaterials(mats);
            return true;
        }
        private bool HasFreeEMZ()
        {
            // ปกติ MonsterZone มี 7 ช่อง (0..4 = MMZ, 5..6 = EMZ)
            var mz = Bot.MonsterZone;
            bool slot5Free = mz.Length > 5 && mz[5] == null;
            bool slot6Free = mz.Length > 6 && mz[6] == null;
            return slot5Free || slot6Free;
        }

        private bool HasValidRageLinkCandidate()
        {
            bool hasSP = HasInExtra(CardId.SP_LITTLE_KNIGHT);
            bool hasGorgon = HasInExtra(CardId.GORGONOFZIL);
            hasGorgon = HasInExtra(CardId.GORGONOFZIL);

            if (hasSP) return true;
            if (hasGorgon && HasFreeEMZ()) return true;
            return false;
        }

        #endregion

        // ======================= On Select Somethings ====================
        #region Work Space #2
        private bool YesNoFor(int desc, int cardId, int idx)
        {
            var info = Duel.GetCurrentSolvingChainInfo();
            var card = Duel.GetCurrentSolvingChainCard();
            // ต้องทั้ง: คำอธิบายตรง + การ์ดบน chain ตอนนี้ตรง
            return desc == Util.GetStringId(cardId, idx)
                   && ((info != null && info.IsCode(cardId)) || (card != null && card.IsCode(cardId)));
        }
        public override bool OnSelectYesNo(int desc)
        {
            Logger.DebugWriteLine($"[DEBUG] OnSelectYesNo: desc={desc}");
            var info = Duel.GetCurrentSolvingChainInfo();
            var solving = Duel.GetCurrentSolvingChainCard();
            DumpChain("OnSelectYesNo");
            Logger.DebugWriteLine($"[THRONE] OnSelectYesNo desc={desc} stage={_throneStage} solving={CardStr(solving)}");
            if (info != null && info.ActivatePlayer == 1)
            { return false; }
            // --- Nightmare Throne ---
            // idx อาจต่างกันตามสคริปต์ แต่แนวคิดคือ anchor กับ desc+solving เสมอ
            if (solving != null && solving.IsCode(CardId.NIGHTMARE_THRONE))
            {
                // เปิด map ช่วย debug ให้เห็นว่า desc ตรง index ไหนจริง ๆ
                DebugThroneDescMap(desc);

                // ยังไม่ได้เริ่มค้น -> ตอบ YES เพื่อเข้าโหมดค้น
                if (_throneStage == ThroneStage.None && !throneSearched)
                {
                    _throneStage = ThroneStage.Searching;
                    return true; // YES เริ่มค้น
                }

                // ค้นเสร็จแล้ว และกำลังถาม "จะทำลายไหม?" -> default = NO
                if (_throneStage == ThroneStage.AwaitDestroyPrompt || throneSearched)
                {
                    _throneStage = ThroneStage.None;
                    return false; // ไม่ทำลาย
                }

                // กันเคส engine ถามซ้ำก่อนโชว์เด็ค: ตอบ YES ไป (จะไปเข้า OnSelectCard)
                if (_throneStage == ThroneStage.Searching && !throneSearched)
                    return true;
            }
            // --- Muckraker ---
            /*if (YesNoFor(desc, CardId.MUCKRAKER_UNDERWORLD, 0))
            {
                if (Duel.ChainTargets.Contains(Card)) return false; // ตัวเองกำลังโดนเล็งทำลาย -> ไม่ใช้
                bool protectAlly = Duel.ChainTargets.Any(t => t != null && t.Controller == 0
                                        && t.Location == CardLocation.MonsterZone && !YUBEL_SET.Contains(t.Id));
                return protectAlly;
            }*/

            // --- Varudras: ถามถอดวัตถุดิบอีก 1 เพื่อทำลาย ---
            if (YesNoFor(desc, CardId.VARUDASN_FINAL_BRINGER, 1))
            {
                var best = GetBestEnemyCard();
                return best != null && ShouldVarudrasDetachForPop(best);
            }
            if (solving != null
                && solving.IsCode(CardId.VARUDASN_FINAL_BRINGER)
                && Duel.CurrentChain.Count > 0) // แปลว่าอยู่ใน e1 ไม่ใช่ e2
            {
                // มีเป้าศัตรูให้ทำลายไหม?
                var t = GetNormalEnemyTargetList(true, true).FirstOrDefault(c => c.Controller == 1);
                if (t == null) return false;                // ไม่มีเป้า → ไม่ถอด
                return ShouldVarudrasDetachForPop(t);       // มีเป้า → ใช้เกณฑ์เดิมตัดสิน
            }

            // aux.Stringid(78371393,2) -> คำถาม "จะสังเวยไหม?"
            if (YesNoFor(desc, CardId.YUBEL, 2))
            {
                // มี Lotus "บนสนามเรา" ไหม
                bool haveLotusOnField = Bot.GetMonsters().Any(m => m != null && m.Id == CardId.SAMSARA_D_LOTUS);

                // ถ้ามี จะตอบ YES และตั้งธงว่ากำลังจะเลือกตัวสังเวยให้ Yubel
                _yubelWantsTribute = haveLotusOnField;
                return haveLotusOnField; // YES ถ้ามี Lotus, NO ถ้าไม่มี => Yubel ระเบิดตัวเอง
            }


            return base.OnSelectYesNo(desc);
        }

        // Safety net for any selection the specific executors didn't pre-select
        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, int hint, bool cancelable)
        {
            Logger.DebugWriteLine($"[DEBUG] OnSelectCard: hint={hint} (0x{hint:X}), min={min}, max={max}, cancelable={cancelable}, candidates={cards?.Count ?? 0}");
            
            bool isReleasePrompt =
            hint == (long)HintMsg.Release ||
            hint.ToString().ToLower().Contains("release"); // กันเหนียว
            var solving = Duel.GetCurrentSolvingChainCard();
            if (cards != null && cards.Count > 0)
            {
                // === Throne ===
                if (_throneStage == ThroneStage.Searching && solving != null && solving.IsCode(CardId.NIGHTMARE_THRONE) && !throneSearched && cards != null && cards.Count > 0)
                {
                    throneSearched = true;
                    _throneStage = ThroneStage.AwaitDestroyPrompt;

                    ClientCard chosen = null;
                    if (throneDesiredPick != 0)
                        chosen = cards.FirstOrDefault(c => c != null && c.Id == throneDesiredPick);

                    // fallback: ถ้าเลือกตามใจไม่ได้ ให้เลือกใบ Yubel ที่มีค่าที่สุด/หรือใบแรก
                    if (chosen == null)
                        chosen = cards.FirstOrDefault(c => c != null && YUBEL_SET.Contains(c.Id)) ?? cards[0];

                    Logger.DebugWriteLine($"[THRONE] Search pick => {CardStr(chosen)}");
                    return new[] { chosen };
                }
                // === SPIRIT GATES selections ===
                if (Card != null && Card.Id == CardId.SPIRIT_GATES && cards != null && cards.Count > 0)
                {
                    // 2.1: เลือก Continuous Spell จากสุสาน (Recycle)
                    if (_gateWantsRecycle)
                    {
                        var pain = cards.FirstOrDefault(c => c != null && c.Id == CardId.NIGHTMARE_PAIN);
                        if (pain != null) return new[] { pain };

                        // อะไรต่อมิอะไรที่เป็น Continuous Spell ถ้ามี
                        var anyCont = cards.FirstOrDefault(c => c != null && c.IsSpell() && c.HasType(CardType.Continuous));
                        if (anyCont != null) return new[] { anyCont };
                    }

                    // เลือกทิ้งมือเป็น cost (ลิสต์ทั้งหมดมาจากมือเรา)
                    bool selectingDiscard = cards.All(c => c != null && c.Controller == 0 && c.Location == CardLocation.Hand);
                    if (selectingDiscard && _gateDiscardPreferredId != 0)
                    {
                        var want = cards.FirstOrDefault(c => c.Id == _gateDiscardPreferredId);
                        if (want != null) return new[] { want };

                        // fallback: ใช้สกอร์เดิม
                        var sorted = cards.OrderBy(ScoreOwnCardForCost).ToList();
                        return new[] { sorted[0] };
                    }

                    // เลือกเป้าหมายชุบจากสุสาน (Fiend 0/0)
                    bool selectingGYTarget = cards.Any(c => c != null && c.Location == CardLocation.Grave);
                    
                    if (selectingGYTarget)
                    {
                        if (requiemSummoned)
                        {
                            var sp = cards.FirstOrDefault(c => c != null && c.Id == CardId.SPIRIT_OF_YUBEL);
                            if (sp != null) return new[] { sp };
                        }
                        if (_gateReviveTargetId != 0)
                        {
                            var t = cards.FirstOrDefault(c => c != null && c.Id == _gateReviveTargetId);
                            if (t != null) return new[] { t };
                        }
                        if (moonSummoned)
                        {
                            int[] prio = {
                                        CardId.SPIRIT_OF_YUBEL,
                                        CardId.YUBEL
                                    };
                            foreach (var id in prio)
                            {
                                var pick = cards.FirstOrDefault(c => c != null && c.Id == id);
                                if (pick != null) return new[] { pick };
                            }
                        }
                        else
                        {
                            int[] prio = {
                                        CardId.SPIRIT_OF_YUBEL,
                                        CardId.SAMSARA_D_LOTUS,
                                        CardId.DARK_BECKONING_BEAST,
                                        CardId.CHAOS_SUMMONING_BEAST,
                                        CardId.YUBEL
                                    };
                            foreach (var id in prio)
                            {
                                var pick = cards.FirstOrDefault(c => c != null && c.Id == id);
                                if (pick != null) return new[] { pick };
                            }
                        }

                        var any00 = cards.FirstOrDefault(c => c != null && Is00FiendId(c.Id));
                        if (any00 != null) return new[] { any00 };
                    }
                }
                // === Throne: เลือกการ์ดที่ค้นเจอ ===
                if (Card.Id == CardId.NIGHTMARE_THRONE && _throneStage == ThroneStage.Searching && thronePending && !throneSearched)
                {
                    throneSearched = true;
                    _throneStage = ThroneStage.AwaitDestroyPrompt;
                    ClientCard chosen = null;
                    if (throneDesiredPick != 0) chosen = cards.FirstOrDefault(c => c.Id == throneDesiredPick);
                    if (chosen == null) chosen = cards[0];

                    Logger.DebugWriteLine($"[THRONE] Search pick => {CardStr(chosen)}");
                    return new[] { chosen };
                }
                // --- Yubel is asking us to choose the tribute target ---
                if (_yubelWantsTribute && isReleasePrompt && cards != null && cards.Count > 0)
                {
                    var lotus = cards.FirstOrDefault(c => c != null && c.Id == CardId.SAMSARA_D_LOTUS);
                    if (lotus != null)
                    {
                        _yubelWantsTribute = false; // เคลียร์ธง
                        return new[] { lotus };     // เลือก Lotus สังเวยตามที่ต้องการ
                    }
                    // เผื่อกรณีเอนจินส่งลิสต์มาแต่ดันไม่มี Lotus (ไม่ควรเกิด เพราะเราตอบ YES เมื่อมี Lotus)
                    _yubelWantsTribute = false;
                    // ปล่อยให้ base ตัดสินใจ หรือจะ return null ก็ได้ตามฐานของคุณ
                }
                // --- Varudras: เลือกเป้าหมายทำลาย ---
                if (Card.Id == CardId.VARUDASN_FINAL_BRINGER && hint == 502 && cards != null && cards.Count > 0)
                {
                    // พยายามเลือกฝั่งศัตรูก่อน (คัดใบที่อันตราย/ป่วนที่สุด)
                    var enemyPick = cards
                        .Where(c => c != null && c.Controller == 1)
                        .OrderByDescending(c => ScoreEnemyCardForRemoval(c))
                        .FirstOrDefault();
                    if (enemyPick != null) return new[] { enemyPick };

                    return new[] { cards[0] }; // fallback
                }
                // --- Abomination: เลือกเป้าหมายทำลาย ---
                if (Card.Id == CardId.UNCHAINDEDABOMINATION && hint == 502 && cards != null && cards.Count > 0)
                {
                    // พยายามเลือกฝั่งศัตรูก่อน (คัดใบที่อันตราย/ป่วนที่สุด)
                    var enemyPick = cards
                        .Where(c => c != null && c.Controller == 1)
                        .OrderByDescending(c => ScoreEnemyCardForRemoval(c))
                        .FirstOrDefault();
                    if (enemyPick != null) return new[] { enemyPick };

                    return new[] { cards[0] }; // fallback
                }
                // --- Rage Quick Link ---
                if (solving != null && solving.IsCode(CardId.UNCHAINED_SOUL_OF_RAGE) && cards.Any(c => c != null && c.Location == CardLocation.Extra))
                {
                    // 1) เลือก S:P Little Knight ก่อนเสมอ ถ้ามี
                    var pickSP = cards.FirstOrDefault(c => c != null && c.Id == CardId.SP_LITTLE_KNIGHT);
                    if (pickSP != null) return new List<ClientCard> { pickSP };

                    // 2) เลือก Gorgon เฉพาะเมื่อ EMZ ว่างเท่านั้น
                    ClientCard pickGorgon = null;
                    pickGorgon = cards.FirstOrDefault(c => c != null && c.Id == CardId.GORGONOFZIL);

                    if (pickGorgon != null)
                    {
                        if (HasFreeEMZ())
                            return new List<ClientCard> { pickGorgon };
                        // ถ้า EMZ ไม่ว่าง ห้ามเลือก Gorgon -> ไปหาใบอื่นต่อ
                    }

                    // 3) fallback: ถ้ามีลิงก์ตัวอื่นที่ generic ก็เลือกใบแรกไปก่อน
                    return new List<ClientCard> { cards[0] };
                }
            }
            

            return base.OnSelectCard(cards, min, max, hint, cancelable);
        }
        
        // --- Scoring helpers -------------------------------------------------
        private int ScoreOwnCardForCost(ClientCard c)
        {
            if (c == null) return int.MaxValue;
            int score = 5000; // lower is more expendable
            if (NEVER_SAC.Contains(c.Id)) return int.MaxValue;
            if (YUBEL_SET.Contains(c.Id)) return 9000;
            if ((c.HasType(CardType.Link) && c.LinkCount >= 2) || c.HasType(CardType.Fusion | CardType.Synchro | CardType.Xyz)) score += 2000;
            if (c.EquipCards != null && c.EquipCards.Count > 0) score += 1000;
            int idx = Array.IndexOf(YubelCostPriority, c.Id);
            if (idx >= 0) score = 10 + idx;
            score += Math.Max(0, c.Attack / 100);
            return score;
        }

        private int ScoreEnemyCardForRemoval(ClientCard c)
        {
            if (c == null) return -1;
            int s = 0;
            if (c.IsFloodgate()) s += 6000;
            if (c.IsMonsterDangerous()) s += 4000;
            if (c.IsMonsterInvincible()) s += 3500;
            if (c.EquipCards != null && c.EquipCards.Count > 0) s += 800;
            if (c.HasType(CardType.Fusion | CardType.Ritual | CardType.Synchro | CardType.Xyz)) s += 700;
            if (c.HasType(CardType.Link) && c.LinkCount >= 2) s += 700;
            s += Math.Max(0, c.Attack);
            return s;
        }

        #endregion

        #region DEBUG

        private string CardStr(ClientCard c)
        {
            if (c == null) return "null";
            string loc = c.Location.ToString();
            string face = c.IsFaceup() ? "FU" : "FD";
            return $"{c.Name}#{c.Id} [{loc}] P{c.Controller} {face}";
        }
        private void DumpChain(string tag = "")
        {
            Logger.DebugWriteLine($"[CHAIN]{(tag == "" ? "" : $" ({tag})")} turn={Duel.Turn} player={Duel.Player} phase={Duel.Phase} chainCount={Duel.CurrentChain.Count}");
            for (int i = 0; i < Duel.CurrentChain.Count; i++)
            {
                var c = Duel.CurrentChain[i];
                Logger.DebugWriteLine($"  [{i}] {CardStr(c)}");
            }
            var solving = Duel.GetCurrentSolvingChainCard();
            if (solving != null)
            {
                Logger.DebugWriteLine($"  -> Solving: {CardStr(solving)}  ActivateDescription={ActivateDescription}");
            }
            if (Duel.ChainTargets != null && Duel.ChainTargets.Count > 0)
            {
                var tg = Duel.ChainTargets.Where(t => t != null).Select(CardStr);
                Logger.DebugWriteLine($"  targets: {string.Join(" | ", tg)}");
            }
        }

        private void DebugThroneDescMap(int incomingDesc)
        {
            for (int i = 0; i < 5; i++)
            {
                int sid = Util.GetStringId(CardId.NIGHTMARE_THRONE, i);
                Logger.DebugWriteLine($"[THRONE] desc map i={i} strId={sid} match={(sid == incomingDesc)}");
            }
        }
        private bool BlockIfThrone(string who)
        {
            if (InThroneFlow)
            {
                Logger.DebugWriteLine($"[THRONE] BLOCKED {who} during Throne flow");
                return true;
            }
            return false;
        }

        #endregion

        // ======================= END OF LIFE ====================
    }
}
