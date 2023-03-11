using YGOSharp.OCGWrapper;
using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using System.Linq;
using System;

namespace WindBot.Game.AI.Decks
{
    [Deck("Exosister", "AI_Exosister")]

    class ExosisterExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int ExosisterElis = 16474916;
            public const int ExosisterStella = 43863925;
            public const int ExosisterIrene = 79858629;
            public const int ExosisterSophia = 5352328;
            public const int ExosisterMartha = 37343995;
            public const int Aratama = 16889337;
            public const int Sakitama = 67972302;
            // _CardId.MaxxC = 23434538;
            // _CardId.AshBlossom = 14558127;

            public const int ExosisterPax = 77913594;
            public const int ExosisterArment = 4408198;
            public const int PotofExtravagance = 84211599;
            // _CardId.CalledByTheGrave = 24224830;

            public const int ExosisterVadis = 77891946;
            public const int ExosisterReturnia = 197042;
            // _CardId.InfiniteImpermanence = 10045474;
            
            public const int ExosisterMikailis = 42741437;
            public const int ExosisterKaspitell = 78135071;
            public const int ExosisterGibrine = 5530780;
            public const int ExosisterAsophiel = 41524885;
            public const int ExosistersMagnifica = 59242457;
            public const int TellarknightConstellarCaduceus = 58858807;
            public const int StellarknightConstellarDiamond = 9272381;
            public const int DivineArsenalAAZEUS_SkyThunder = 90448279;
            public const int DonnerDaggerFurHire = 8728498;
            // _CardId.EvilswarmExcitonKnight = 46772449;

            public const int NaturalExterio = 99916754;
            public const int NaturalBeast = 33198837;
            public const int ImperialOrder = 61740673;
            public const int SwordsmanLV7 = 37267041;
            public const int RoyalDecree = 51452091;
            public const int Number41BagooskatheTerriblyTiredTapir = 90590303;
            public const int InspectorBoarder = 15397015;
            public const int DimensionShifter = 91800273;
        }

        public ExosisterExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // trigger
            AddExecutor(ExecutorType.Activate, CardId.ExosistersMagnifica, ExosistersMagnificaActivateTrigger);

            // quick effect
            AddExecutor(ExecutorType.Activate, CardId.ExosisterMikailis,     ExosisterMikailisActivate);
            AddExecutor(ExecutorType.Activate, CardId.ExosistersMagnifica,   ExosistersMagnificaActivateBanish);
            AddExecutor(ExecutorType.Activate, CardId.ExosisterReturnia,     ExosisterReturniaActivate);
            AddExecutor(ExecutorType.Activate, CardId.ExosisterVadis,        ExosisterVadisActivate);
            AddExecutor(ExecutorType.Activate, _CardId.InfiniteImpermanence, InfiniteImpermanenceActivate);
            AddExecutor(ExecutorType.Activate, CardId.StellarknightConstellarDiamond);
            AddExecutor(ExecutorType.Activate, _CardId.AshBlossom,           AshBlossomActivate);
            AddExecutor(ExecutorType.Activate, _CardId.CalledByTheGrave,     CalledbytheGraveActivate);
            AddExecutor(ExecutorType.Activate, DefaultExosisterTransform);
            AddExecutor(ExecutorType.Activate, CardId.ExosisterArment,       ExosisterArmentActivate);

            // free chain
            AddExecutor(ExecutorType.Activate, _CardId.MaxxC, MaxxCActivate);

            // search
            AddExecutor(ExecutorType.Activate, CardId.PotofExtravagance, PotofExtravaganceActivate);

            // field effect
            AddExecutor(ExecutorType.Activate, CardId.Aratama);
            AddExecutor(ExecutorType.Activate, CardId.DonnerDaggerFurHire, DonnerDaggerFurHireActivate);
            AddExecutor(ExecutorType.Activate, CardId.ExosisterKaspitell,  ExosisterKaspitellActivate);
            AddExecutor(ExecutorType.Activate, CardId.ExosisterGibrine,    ExosisterGibrineActivate);
            AddExecutor(ExecutorType.Activate, CardId.ExosisterAsophiel,   ExosisterAsophielActivate);

            AddExecutor(ExecutorType.Activate, CardId.ExosisterSophia, ExosisterSophiaActivate);
            AddExecutor(ExecutorType.Activate, CardId.ExosisterIrene,  ExosisterIreneActivate);
            AddExecutor(ExecutorType.Activate, CardId.ExosisterStella, ExosisterStellaActivate);

            // addition monster summmon
            AddExecutor(ExecutorType.Activate, CardId.ExosisterElis, ExosisterElisActivate);
            AddExecutor(ExecutorType.Activate, CardId.Sakitama, SakitamaActivate);
            AddExecutor(ExecutorType.Activate, CardId.ExosisterPax,  ExosisterPaxActivate);
            AddExecutor(ExecutorType.Activate, CardId.ExosisterStella, ExosisterStellaSecondActivate);

            // xyz summon
            AddExecutor(ExecutorType.SpSummon, CardId.StellarknightConstellarDiamond);
            AddExecutor(ExecutorType.SpSummon, CardId.DonnerDaggerFurHire, DonnerDaggerFurHireSpSummonCheck);
            AddExecutor(ExecutorType.SpSummon, CardId.ExosisterMikailis, ExosisterMikailisAdvancedSpSummonCheck);
            AddExecutor(ExecutorType.SpSummon, CardId.ExosisterKaspitell, ExosisterKaspitellAdvancedSpSummonCheck);

            AddExecutor(ExecutorType.SpSummon, CardId.ExosisterKaspitell, ExosisterKaspitellSpSummonCheck);
            AddExecutor(ExecutorType.SpSummon, CardId.ExosisterMikailis, ExosisterMikailisSpSummonCheck);
            AddExecutor(ExecutorType.SpSummon, CardId.TellarknightConstellarCaduceus, TellarknightConstellarCaduceusSpSummonCheck);

            AddExecutor(ExecutorType.SpSummon, CardId.ExosistersMagnifica, ExosistersMagnificaSpSummonCheck);

            AddExecutor(ExecutorType.SpSummon, _CardId.EvilswarmExcitonKnight, DefaultEvilswarmExcitonKnightSummon);
            AddExecutor(ExecutorType.Activate, _CardId.EvilswarmExcitonKnight, DefaultEvilswarmExcitonKnightEffect);

            // normal summon for xyz(avoiding MaxxC)
            AddExecutor(ExecutorType.Summon, CardId.ExosisterStella, ExosisterAvoidMaxxCSummonCheck);
            AddExecutor(ExecutorType.Summon, CardId.ExosisterSophia, ExosisterAvoidMaxxCSummonCheck);
            AddExecutor(ExecutorType.Summon, CardId.ExosisterIrene,  ExosisterAvoidMaxxCSummonCheck);
            AddExecutor(ExecutorType.Summon, CardId.ExosisterElis,   ExosisterAvoidMaxxCSummonCheck);

            // activate martha
            AddExecutor(ExecutorType.Activate, CardId.ExosisterMartha, ExosisterMarthaActivate);

            // normal summon for xyz
            AddExecutor(ExecutorType.Summon, CardId.ExosisterStella, ExosisterStellaSummonCheck);
            AddExecutor(ExecutorType.Summon, CardId.Aratama, AratamaSummonCheck);
            AddExecutor(ExecutorType.Summon, ExosisterForElisSummonCheck);
            AddExecutor(ExecutorType.Summon, ForSakitamaSummonCheck);
            AddExecutor(ExecutorType.Summon, CardId.ExosisterIrene, ExosisterIreneSummonCheck);
            AddExecutor(ExecutorType.Summon, Level4SummonCheck);
            AddExecutor(ExecutorType.Summon, ExosisterForArmentSummonCheck);
            AddExecutor(ExecutorType.Summon, ForDonnerSummonCheck);

            AddExecutor(ExecutorType.Activate, CardId.ExosisterPax,  ExosisterPaxActivateForEndSearch);

            // other
            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
            AddExecutor(ExecutorType.SpellSet, SpellSetCheck);
        }

        const int SetcodeTimeLord = 0x4a;
        const int SetcodeShadoll = 0x9d;
        const int SetcodeInferoid = 0xbb;
        const int SetcodeOrcust = 0x11b;
        const int SetcodeExosister = 0x172;
        const int SetcodeTearlaments = 0x181;
        List<int> SetcodeForDiamond = new List<int>{SetcodeShadoll, SetcodeInferoid, SetcodeTearlaments};

        List<int> affectGraveCardIdList = new List<int>{
            71344451, 40975243, 87746184, 70534340, 45906428, 71490127, 3659803, 12071500, 6077601, 11827244, 95238394, 81223446, 40003819,
            72490637, 21011044, 59419719, 14735698, 45410988
        };

        Dictionary<int, List<int>> DeckCountTable = new Dictionary<int, List<int>>{
            {3, new List<int> { CardId.ExosisterElis, CardId.ExosisterStella, CardId.ExosisterMartha, CardId.Aratama, CardId.Sakitama,
                                _CardId.MaxxC, _CardId.AshBlossom, CardId.ExosisterPax, CardId.ExosisterVadis }},
            {2, new List<int> { CardId.ExosisterIrene, CardId.ExosisterSophia, CardId.PotofExtravagance, _CardId.CalledByTheGrave,
                                CardId.ExosisterReturnia, _CardId.InfiniteImpermanence }},
            {1, new List<int> { CardId.ExosisterArment }},
        };
        Dictionary<int, int> ExosisterMentionTable = new Dictionary<int, int>{
            {CardId.ExosisterElis, CardId.ExosisterStella}, {CardId.ExosisterStella, CardId.ExosisterElis},
            {CardId.ExosisterIrene, CardId.ExosisterSophia}, {CardId.ExosisterSophia, CardId.ExosisterIrene},
            {CardId.ExosisterMartha, CardId.ExosisterElis}
        };
        List<int> ExosisterSpellTrapList = new List<int>{CardId.ExosisterPax, CardId.ExosisterArment, CardId.ExosisterVadis, CardId.ExosisterReturnia};

        Dictionary<int, int> calledbytheGraveCount = new Dictionary<int, int>();
        bool enemyActivateMaxxC = false;
        bool enemyActivateLockBird = false;
        bool enemyMoveGrave = false;
        bool paxCallToField = false;
        List<int> infiniteImpermanenceList = new List<int>();

        bool summoned = false;
        bool elisEffect1Activated = false;
        bool stellaEffect1Activated = false;
        bool irenaEffect1Activated = false;
        bool sophiaEffect1Activated = false;
        bool marthaEffect1Activated = false;
        bool mikailisEffect1Activated = false;
        bool mikailisEffect3Activated = false;
        bool kaspitellEffect1Activated = false;
        bool kaspitellEffect3Activated = false;
        bool gibrineEffect1Activated = false;
        bool gibrineEffect3Activated = false;
        bool asophielEffect1Activated = false;
        bool asophielEffect3Activated = false;
        bool sakitamaEffect1Activated = false;
        List<int> exosisterTransformEffectList = new List<int>();
        List<int> oncePerTurnEffectActivatedList = new List<int>();
        List<ClientCard> activatedMagnificaList = new List<ClientCard>();
        List<ClientCard> targetedMagnificaList = new List<ClientCard>();
        List<int> transformDestList = new List<int>();
        List<ClientCard> spSummonThisTurn = new List<ClientCard>();
        bool potActivate = false;
        List<ClientCard> removeChosenList = new List<ClientCard>();

        /// <summary>
        /// Shuffle List<ClientCard> and return a random-order card list
        /// </summary>
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

        public ClientCard GetProblematicEnemyMonster(int attack = 0, bool canBeTarget = false)
        {
            List<ClientCard> floodagateList = Enemy.GetMonsters().Where(c => c?.Data != null &&
                c.IsFloodgate() && c.IsFaceup() && (!canBeTarget || !c.IsShouldNotBeTarget())).ToList();
            if (floodagateList.Count > 0)
            {
                floodagateList.Sort(CardContainer.CompareCardAttack);
                floodagateList.Reverse();
                return floodagateList[0];
            }

            List<ClientCard> dangerList = Enemy.MonsterZone.Where(c => c?.Data != null &&
                c.IsMonsterDangerous() && c.IsFaceup() && (!canBeTarget || !c.IsShouldNotBeTarget())).ToList();
            if (dangerList.Count > 0)
            {
                dangerList.Sort(CardContainer.CompareCardAttack);
                dangerList.Reverse();
                return dangerList[0];
            }

            List<ClientCard> invincibleList = Enemy.MonsterZone.Where(c => c?.Data != null &&
                c.IsMonsterInvincible() && c.IsFaceup() && (!canBeTarget || !c.IsShouldNotBeTarget())).ToList();
            if (invincibleList.Count > 0)
            {
                invincibleList.Sort(CardContainer.CompareCardAttack);
                invincibleList.Reverse();
                return invincibleList[0];
            }

            if (attack == 0)
                attack = Util.GetBestAttack(Bot);
            List<ClientCard> betterList = Enemy.MonsterZone.GetMonsters()
                .Where(card => card.GetDefensePower() >= attack && card.IsAttack() && (!canBeTarget || !card.IsShouldNotBeTarget())).ToList();
            if (betterList.Count > 0)
            {
                betterList.Sort(CardContainer.CompareCardAttack);
                betterList.Reverse();
                return betterList[0];
            }
            return null;
        }

        public ClientCard GetProblematicEnemyCard(bool canBeTarget = false)
        {
            List<ClientCard> floodagateList = Enemy.MonsterZone.Where(c => c?.Data != null && !removeChosenList.Contains(c) &&
                c.IsFloodgate() && c.IsFaceup() && (!canBeTarget || !c.IsShouldNotBeTarget())).ToList();
            if (floodagateList.Count > 0)
            {
                floodagateList.Sort(CardContainer.CompareCardAttack);
                floodagateList.Reverse();
                return floodagateList[0];
            }
            
            List<ClientCard> problemEnemySpellList = Enemy.SpellZone.Where(c => c?.Data != null && !removeChosenList.Contains(c)
                && c.IsFloodgate() && c.IsFaceup() && (!canBeTarget || !c.IsShouldNotBeTarget())).ToList();
            if (problemEnemySpellList.Count > 0)
            {
                return ShuffleCardList(problemEnemySpellList)[0];
            }

            List<ClientCard> dangerList = Enemy.MonsterZone.Where(c => c?.Data != null && !removeChosenList.Contains(c)
                && c.IsMonsterDangerous() && c.IsFaceup() && (!canBeTarget || !c.IsShouldNotBeTarget())).ToList();
            if (dangerList.Count > 0
                && (Duel.Player == 0 || (Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2)))
            {
                dangerList.Sort(CardContainer.CompareCardAttack);
                dangerList.Reverse();
                return dangerList[0];
            }

            List<ClientCard> invincibleList = Enemy.MonsterZone.Where(c => c?.Data != null && !removeChosenList.Contains(c)
                && c.IsMonsterInvincible() && c.IsFaceup() && (!canBeTarget || !c.IsShouldNotBeTarget())).ToList();
            if (invincibleList.Count > 0)
            {
                invincibleList.Sort(CardContainer.CompareCardAttack);
                invincibleList.Reverse();
                return invincibleList[0];
            }

            List<ClientCard> enemyMonsters = Enemy.GetMonsters().Where(c => !removeChosenList.Contains(c)).ToList();
            if (enemyMonsters.Count > 0)
            {
                enemyMonsters.Sort(CardContainer.CompareCardAttack);
                enemyMonsters.Reverse();
                foreach(ClientCard target in enemyMonsters)
                {
                    if (target.HasType(CardType.Fusion) || target.HasType(CardType.Ritual) || target.HasType(CardType.Synchro) || target.HasType(CardType.Xyz) || (target.HasType(CardType.Link) && target.LinkCount >= 2) )
                    {
                        if (!canBeTarget || !(target.IsShouldNotBeTarget() || target.IsShouldNotBeMonsterTarget())) return target;
                    }
                }
            }

            List<ClientCard> spells = Enemy.GetSpells().Where(c => c.IsFaceup() && !removeChosenList.Contains(c)
                && (c.HasType(CardType.Equip) || c.HasType(CardType.Pendulum) || c.HasType(CardType.Field) || c.HasType(CardType.Continuous)))
                .ToList();
            if (spells.Count > 0)
            {
                return ShuffleCardList(spells)[0];
            }

            return null;
        }
        
        public ClientCard GetBestEnemyMonster(bool onlyFaceup = false, bool canBeTarget = false)
        {
            ClientCard card = GetProblematicEnemyMonster(0, canBeTarget);
            if (card != null)
                return card;

            card = Enemy.MonsterZone.GetHighestAttackMonster(canBeTarget);
            if (card != null)
                return card;

            List<ClientCard> monsters = Enemy.GetMonsters();

            // after GetHighestAttackMonster, the left monsters must be face-down.
            if (monsters.Count > 0 && !onlyFaceup)
                return ShuffleCardList(monsters)[0];

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

            List<ClientCard> faceUpList = spells.Where(ecard => ecard.IsFaceup() &&
                (ecard.HasType(CardType.Continuous) || ecard.HasType(CardType.Field) || ecard.HasType(CardType.Pendulum))).ToList();
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
            if (card != null)
            {
                return card;
            }

            card = GetBestEnemySpell(onlyFaceup, canBeTarget);
            if (card != null)
            {
                return card;
            }
            
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

        /// <summary>
        /// Check remain cards in deck
        /// </summary>
        /// <param name="id">Card's ID</param>
        public int CheckRemainInDeck(int id)
        {
            for (int count = 1; count < 4; ++count)
            {
                if (DeckCountTable[count].Contains(id)) {
                    return Bot.GetRemainingCount(id, count);
                }
            }
            return 0;
        }

        /// <summary>
        /// Check negated turn count of id
        /// </summary>
        public int CheckCalledbytheGrave(int id)
        {
            if (!calledbytheGraveCount.ContainsKey(id))
            {
                return 0;
            }
            return calledbytheGraveCount[id];
        }

        public void CheckEnemyMoveGrave()
        {
            if (Duel.LastChainPlayer == 1)
            {
                ClientCard card = Util.GetLastChainCard();
                if (Duel.LastChainLocation == CardLocation.Grave && card.Location == CardLocation.Grave)
                {
                    Logger.DebugWriteLine("===Exosister: enemy activate effect from GY.");
                    enemyMoveGrave = true;
                }
                else if (affectGraveCardIdList.Contains(card.Id))
                {
                    Logger.DebugWriteLine("===Exosister: enemy activate effect that affect GY.");
                    enemyMoveGrave = true;
                }
                else
                {
                    foreach (ClientCard targetCard in Duel.LastChainTargets)
                    {
                        if (targetCard.Location == CardLocation.Grave)
                        {
                            Logger.DebugWriteLine("===Exosister: enemy target cards of GY.");
                            enemyMoveGrave = true;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Check exosister's relative card. 0 for error.
        /// </summary>
        public int CheckExosisterMentionCard(int id)
        {
            if (!ExosisterMentionTable.ContainsKey(id))
            {
                return 0;
            }
            return ExosisterMentionTable[id];
        }

        /// <summary>
        /// Check whether last chain card should be disabled.
        /// </summary>
        public bool CheckLastChainShouldNegated()
        {
            ClientCard lastcard = Util.GetLastChainCard();
            if (lastcard == null || lastcard.Controller != 1) return false;
            if (lastcard.IsMonster() && lastcard.HasSetcode(SetcodeTimeLord) && Duel.Phase == DuelPhase.Standby) return false;
            return true;
        }

        /// <summary>
        /// Check whether negate opposite's effect and clear flag
        /// </summary>
        public void CheckDeactiveFlag()
        {
            if (Util.GetLastChainCard() != null && Duel.LastChainPlayer == 1)
            {
                if (Util.GetLastChainCard().IsCode(_CardId.MaxxC))
                {
                    enemyActivateMaxxC = false;
                }
                if (Util.GetLastChainCard().IsCode(_CardId.LockBird))
                {
                    enemyActivateLockBird = false;
                }
            }
        }

        /// <summary>
        /// Check whether opposite use Maxx-C, and thus make less operation.
        /// </summary>
        public bool CheckLessOperation()
        {
            if (!enemyActivateMaxxC)
            {
                return false;
            }
            return CheckAtAdvantage();
        }

        /// <summary>
        /// Check whether bot is at advantage.
        /// </summary>
        public bool CheckAtAdvantage()
        {
            if (GetProblematicEnemyMonster() == null && Bot.GetMonsters().Any(card => card.IsFaceup()))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check whether bot is in danger and need to summon monster to defense.
        /// </summary> 
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

        /// <summary>
        /// Check whether can be used for xyz summon.
        /// </summary>
        public bool CheckAbleForXyz(ClientCard card)
        {
            return card.IsFaceup() && !card.HasType(CardType.Xyz) && !card.HasType(CardType.Link) && !card.HasType(CardType.Token) && card.Level == 4;
        }

        /// <summary>
        /// Check whether bot can activate martha.
        /// </summary>
        public bool CheckMarthaActivatable()
        {
            return !marthaEffect1Activated && CheckCalledbytheGrave(CardId.ExosisterMartha) == 0 && CheckRemainInDeck(CardId.ExosisterElis) > 0
                && !Bot.GetMonsters().Any(card => card.IsFacedown() || !card.HasType(CardType.Xyz));
        }

        /// <summary>
        /// check enemy's dangerous card in grave
        /// </summary>
        public List<ClientCard> CheckDangerousCardinEnemyGrave(bool onlyMonster = false)
        {
            List<ClientCard> result = Enemy.Graveyard.GetMatchingCards(card => 
            (!onlyMonster || card.IsMonster()) && card.HasSetcode(SetcodeOrcust)).ToList();
            return result;
        }

        /// <summary>
        /// Whether spell or trap will be negate. If so, return true.
        /// </summary>
        /// <param name="isCounter">is counter trap</param>
        /// <param name="target">check target</param>
        /// <returns></returns>
        public bool SpellNegatable(bool isCounter = false, ClientCard target = null)
        {
            // target default set
            if (target == null) target = Card;
            // won't negate if not on field
            if (target.Location != CardLocation.SpellZone && target.Location != CardLocation.Hand) return false;

            // negate judge
            if (Enemy.HasInMonstersZone(CardId.NaturalExterio, true) && !isCounter) return true;
            if (target.IsSpell())
            {
                if (Enemy.HasInMonstersZone(CardId.NaturalBeast, true)) return true;
                if (Enemy.HasInSpellZone(CardId.ImperialOrder, true) || Bot.HasInSpellZone(CardId.ImperialOrder, true)) return true;
                if (Enemy.HasInMonstersZone(CardId.SwordsmanLV7, true) || Bot.HasInMonstersZone(CardId.SwordsmanLV7, true)) return true;
            }
            if (target.IsTrap())
            {
                if (Enemy.HasInSpellZone(CardId.RoyalDecree, true) || Bot.HasInSpellZone(CardId.RoyalDecree, true)) return true;
            }
            if (target.Location == CardLocation.SpellZone && (target.IsSpell() || target.IsTrap()))
            {
                int selfSeq = -1;
                for (int i = 0; i < 5; ++i)
                {
                    if (Bot.SpellZone[i] == Card) selfSeq = i;
                }
                if (infiniteImpermanenceList.Contains(selfSeq)) {
                    return true;
                }
            }
            // how to get here?
            return false;
        }

        /// <summary>
        /// Check whether'll be negated
        /// </summary>
        /// <param name="isCounter">check whether card itself is disabled.</param>
        public bool CheckWhetherNegated(bool disablecheck = true){
            if (Card.IsSpell() || Card.IsTrap()){
                if (SpellNegatable()) return true;
            }
            if (CheckCalledbytheGrave(Card.Id) > 0){
                return true;
            }
            if (Card.IsMonster() && Card.Location == CardLocation.MonsterZone && Card.IsDefense())
            {
                if (Enemy.MonsterZone.GetFirstMatchingFaceupCard(card => card.IsCode(CardId.Number41BagooskatheTerriblyTiredTapir) && card.IsDefense() && !card.IsDisabled()) != null
                    || Bot.MonsterZone.GetFirstMatchingFaceupCard(card => card.IsCode(CardId.Number41BagooskatheTerriblyTiredTapir) && card.IsDefense() && !card.IsDisabled()) != null)
                {
                    return true;
                }
            }
            if (disablecheck){
                return Card.IsDisabled();
            }
            return false;
        }

        /// <summary>
        /// Select spell/trap's place randomly to avoid InfiniteImpermanence and so on.
        /// </summary>
        /// <param name="card">Card to set(default current card)</param>
        /// <param name="avoidImpermanence">Whether need to avoid InfiniteImpermanence</param>
        /// <param name="avoidList">Whether need to avoid set in this place</param>
        public void SelectSTPlace(ClientCard card = null, bool avoidImpermanence = false, List<int> avoidList = null)
        {
            List<int> list = new List<int> { 0, 1, 2, 3, 4 };
            int n = list.Count;
            while (n-- > 1)
            {
                int index = Program.Rand.Next(n + 1);
                int temp = list[index];
                list[index] = list[n];
                list[n] = temp;
            }
            foreach (int seq in list)
            {
                int zone = (int)System.Math.Pow(2, seq);
                if (Bot.SpellZone[seq] == null)
                {
                    if (card != null && card.Location == CardLocation.Hand && avoidImpermanence && infiniteImpermanenceList.Contains(seq)) continue;
                    if (avoidList != null && avoidList.Contains(seq)) continue;
                    AI.SelectPlace(zone);
                    return;
                };
            }
            AI.SelectPlace(0);
        }

        public void SelectXyzMaterial(int num = 2, bool needExosister = false)
        {
            List<ClientCard> materialList = Bot.GetMonsters().Where(card => CheckAbleForXyz(card)).ToList();
            if (materialList?.Count() < num)
            {
                return;
            }
            if (needExosister && !materialList.Any(card => card.HasSetcode(SetcodeExosister)))
            {
                return;
            }
            List<ClientCard> selectedList = new List<ClientCard>();

            // if needed, select exosister with less atk first
            if (needExosister)
            {
                List<ClientCard> exosisterList = materialList.Where(card => card.HasSetcode(SetcodeExosister)).ToList();
                exosisterList.Sort(CardContainer.CompareCardAttack);
                ClientCard firstSelect = exosisterList[0];
                selectedList.Add(firstSelect);
                materialList.Remove(firstSelect);
            }
            
            // select non-exosister or effecte used's exosister first
            // never use martha first
            List<ClientCard> sortMaterialList = materialList.Where(card => 
                (card?.Data != null && !card.HasSetcode(SetcodeExosister)) || (exosisterTransformEffectList.Contains(card.Id) && card.Id != CardId.ExosisterMartha)).ToList();
            sortMaterialList.Sort(CardContainer.CompareCardAttack);
            foreach (ClientCard card in sortMaterialList)
            {
                selectedList.Add(card);
                if (selectedList.Count() >= num)
                {
                    AI.SelectMaterials(selectedList);
                    return;
                }
            }

            List<ClientCard> valuableMaterialList = materialList.Where(card => card.Id == CardId.ExosisterMartha || !exosisterTransformEffectList.Contains(card.Id)).ToList();
            valuableMaterialList.Sort(CardContainer.CompareCardAttack);
            foreach (ClientCard card in valuableMaterialList)
            {
                selectedList.Add(card);
                if (selectedList.Count() >= num)
                {
                    AI.SelectMaterials(selectedList);
                    return;
                }
            }
        }

        public void SelectDetachMaterial(ClientCard activateCard)
        {
            // TODO
            AI.SelectCard(0);
        }

        /// <summary>
        /// go first
        /// </summary>
        public override bool OnSelectHand()
        {
            return true;
        }

        /// <summary>
        /// check whether enemy activate important card
        /// </summary>
        public override void OnChaining(int player, ClientCard card)
        {
            if (card == null) return;

            if (player == 1)
            {
                if (card.IsCode(_CardId.MaxxC) && CheckCalledbytheGrave(_CardId.MaxxC) == 0)
                {
                    enemyActivateMaxxC = true;
                }
                if (card.IsCode(_CardId.LockBird) && CheckCalledbytheGrave(_CardId.LockBird) == 0)
                {
                    enemyActivateLockBird = true;
                }
                if (card.IsCode(_CardId.InfiniteImpermanence))
                {
                    for (int i = 0; i < 5; ++i)
                    {
                        if (Enemy.SpellZone[i] == card)
                        {
                            infiniteImpermanenceList.Add(4-i);
                            break;
                        }
                    }
                }
                if (Duel.LastChainLocation == CardLocation.Grave && card.Location == CardLocation.Grave)
                {
                    Logger.DebugWriteLine("===Exosister: enemy activate effect from GY.");
                    enemyMoveGrave = true;
                }
            }
            base.OnChaining(player, card);
        }

        public override void OnSelectChain(IList<ClientCard> cards)
        {
            int player = Duel.LastChainPlayer;
            ClientCard card = Util.GetLastChainCard();
            if (player == 1)
            {
                if (card != null && card.IsCode(_CardId.CalledByTheGrave))
                {
                    foreach (ClientCard targetCard in Duel.LastChainTargets) {
                        Logger.DebugWriteLine("===Exosister: " + targetCard?.Name + " is targeted by called by the grave.");
                        calledbytheGraveCount[targetCard.Id] = 2;
                    }
                }
                foreach (ClientCard targetCard in Duel.LastChainTargets) {
                    if (targetCard.Location == CardLocation.Grave)
                    {
                        Logger.DebugWriteLine("===Exosister: enemy target cards of GY.");
                        enemyMoveGrave = true;
                        break;
                    }
                }
            }
            base.OnSelectChain(cards);
        }

        /// <summary>
        /// clear chain information
        /// </summary>
        public override void OnChainEnd()
        {
            enemyMoveGrave = false;
            paxCallToField = false;
            potActivate = false;
            transformDestList.Clear();
            targetedMagnificaList.Clear();
            if (activatedMagnificaList.Count() > 0)
            {
                for (int idx = activatedMagnificaList.Count() - 1; idx >= 0; -- idx)
                {
                    ClientCard checkTarget = activatedMagnificaList[idx];
                    if (checkTarget == null || checkTarget.IsFacedown() || checkTarget.Location != CardLocation.MonsterZone)
                    {
                        activatedMagnificaList.RemoveAt(idx);
                    }
                }
            }
            if (spSummonThisTurn.Count() > 0)
            {
                for (int idx = spSummonThisTurn.Count() - 1; idx >= 0; -- idx)
                {
                    ClientCard checkTarget = spSummonThisTurn[idx];
                    if (checkTarget == null || checkTarget.IsFacedown() || checkTarget.Location != CardLocation.MonsterZone)
                    {
                        spSummonThisTurn.RemoveAt(idx);
                    }
                }
            }
            base.OnChainEnd();
        }

        public override void OnNewTurn()
        {
            enemyActivateMaxxC = false;
            enemyActivateLockBird = false;
            infiniteImpermanenceList.Clear();
            // CalledbytheGrave refresh
            List<int> key_list = calledbytheGraveCount.Keys.ToList();
            foreach (int dic in key_list)
            {
                if (calledbytheGraveCount[dic] > 1)
                {
                    calledbytheGraveCount[dic] -= 1;
                }
            }

            summoned = false;
            elisEffect1Activated = false;
            stellaEffect1Activated = false;
            irenaEffect1Activated = false;
            sophiaEffect1Activated = false;
            marthaEffect1Activated = false;
            mikailisEffect1Activated = false;
            mikailisEffect3Activated = false;
            kaspitellEffect1Activated = false;
            kaspitellEffect3Activated = false;
            gibrineEffect1Activated = false;
            gibrineEffect3Activated = false;
            asophielEffect1Activated = false;
            asophielEffect3Activated = false;
            sakitamaEffect1Activated = false;
            exosisterTransformEffectList.Clear();
            oncePerTurnEffectActivatedList.Clear();
            activatedMagnificaList.Clear();
            spSummonThisTurn.Clear();
        }

        /// <summary>
        /// override for exosister's transform
        /// </summary>
        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, int hint, bool cancelable)
        {
            bool beginTransformCheck = false;
            // transform for main monster
            if (hint == HintMsg.SpSummon && min == 1 && max == 1 && transformDestList.Count() > 0)
            {
                // check whether for transform
                if (cards.All(card => card.Location == CardLocation.Extra && card.Rank == 4 && card.HasSetcode(SetcodeExosister)))
                {
                    beginTransformCheck = true;
                }
            }
            // transform for magnifica
            if (hint == HintMsg.ToDeck && min == 1 && max == 1 && transformDestList.Count() > 0)
            {
                if (cards.All(card => card.Location == CardLocation.Overlay))
                {
                    beginTransformCheck = true;
                }
            }
            if (beginTransformCheck)
            {
                for (int idx = 0; idx < transformDestList.Count(); ++ idx)
                {
                    int targetId = transformDestList[idx];
                    ClientCard targetCard = cards.FirstOrDefault(card => card.IsCode(targetId));
                    if (targetCard != null)
                    {
                        List<ClientCard> result = new List<ClientCard>();
                        result.Add(targetCard);
                        transformDestList.RemoveAt(idx);
                        spSummonThisTurn.AddRange(result);
                        return Util.CheckSelectCount(result, cards, min, max);
                    }
                }
            }
            
            if (Util.ChainContainsCard(_CardId.EvenlyMatched) && Util.ChainContainPlayer(1) && hint == HintMsg.Remove)
            {
                int botCount = Bot.GetMonsterCount() + Bot.GetSpellCount();
                int oppositeCount = Enemy.GetMonsterCount() + Enemy.GetSpellCount();
                if (botCount - oppositeCount == min && min == max)
                {
                    Logger.DebugWriteLine("===Exosister: Evenly Matched activated.");
                    List<ClientCard> allBotCards = new List<ClientCard>();
                    allBotCards.AddRange(Bot.GetMonsters());
                    allBotCards.AddRange(Bot.GetSpells());
                    List<ClientCard> importantList = new List<ClientCard>();

                    List<ClientCard> magnificaList = allBotCards.Where(card => card.IsCode(CardId.ExosistersMagnifica)).ToList();
                    if (magnificaList.Count > 0)
                    {
                        allBotCards.RemoveAll(c => magnificaList.Contains(c));
                        importantList.AddRange(magnificaList);
                    }
                    if (!mikailisEffect1Activated)
                    {
                        List<ClientCard> mikailisList = allBotCards.Where(card => spSummonThisTurn.Contains(card)
                            && card.IsCode(CardId.ExosisterMikailis) && card.IsFaceup()).ToList();
                        if (mikailisList.Count > 0)
                        {
                            allBotCards.RemoveAll(c => mikailisList.Contains(c));
                            importantList.AddRange(mikailisList);
                        }
                    }
                    if (!gibrineEffect1Activated)
                    {
                        List<ClientCard> gibrineList = allBotCards.Where(card => spSummonThisTurn.Contains(card)
                            && card.IsCode(CardId.ExosisterGibrine) && card.IsFaceup()).ToList();
                        if (gibrineList.Count > 0)
                        {
                            allBotCards.RemoveAll(c => gibrineList.Contains(c));
                            importantList.AddRange(gibrineList);
                        }
                    }
                    if (!oncePerTurnEffectActivatedList.Contains(CardId.ExosisterVadis))
                    {
                        List<ClientCard> vadisList = allBotCards.Where(card => card.IsCode(CardId.ExosisterVadis) && card.IsFacedown()).ToList();
                        if (vadisList.Count > 0)
                        {
                            allBotCards.RemoveAll(c => vadisList.Contains(c));
                            importantList.AddRange(vadisList);
                        }
                    }
                    List<ClientCard> xyzList = allBotCards.Where(card => card.IsMonster() && card.HasType(CardType.Xyz)).ToList();
                    if (xyzList.Count > 0)
                    {
                        xyzList.Sort(CardContainer.CompareCardAttack);
                        xyzList.Reverse();
                        allBotCards.RemoveAll(c => xyzList.Contains(c));
                        importantList.AddRange(xyzList);
                    }
                    List<ClientCard> monsterList = allBotCards.Where(card => card.IsMonster()).ToList();
                    if (monsterList.Count > 0)
                    {
                        monsterList.Sort(CardContainer.CompareCardAttack);
                        monsterList.Reverse();
                        allBotCards.RemoveAll(c => monsterList.Contains(c));
                        importantList.AddRange(monsterList);
                    }
                    List<ClientCard> faceDownList = allBotCards.Where(card => card.IsFacedown()).ToList();
                    if (faceDownList.Count > 0)
                    {
                        allBotCards.RemoveAll(c => faceDownList.Contains(c));
                        importantList.AddRange(ShuffleCardList(faceDownList));
                    }
                    
                    importantList.Reverse();
                    return Util.CheckSelectCount(importantList, cards, min, max);
                }
            }

            return base.OnSelectCard(cards, min, max, hint, cancelable);
        }

        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            YGOSharp.OCGWrapper.NamedCard cardData = YGOSharp.OCGWrapper.NamedCard.Get(cardId);
            if (cardData != null)
            {
                if (Util.IsTurn1OrMain2())
                {
                    bool turnDefense = false;
                    if (cardId == CardId.DivineArsenalAAZEUS_SkyThunder || cardId == CardId.ExosistersMagnifica)
                    {
                        turnDefense = true;
                    }
                    if (!cardData.HasType(CardType.Xyz))
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
                    if (!cardData.HasType(CardType.Xyz) || cardData.Defense >= cardData.Attack || Util.IsOneEnemyBetterThanValue(cardData.Attack, true))
                    {
                        return CardPosition.FaceUpDefence;
                    }
                }
                int bestBotAttack = Math.Max(Util.GetBestAttack(Bot), cardData.Attack);
                if (Util.IsAllEnemyBetterThanValue(bestBotAttack, true))
                {
                    return CardPosition.FaceUpDefence;
                }
            }
            return base.OnSelectPosition(cardId, positions);
        }

        /// <summary>
        /// override for magnifica's spsummon
        /// </summary>
        public override bool OnSelectYesNo(int desc)
        {
            // magnifica spsummon
            if (desc == Util.GetStringId(CardId.ExosistersMagnifica, 2))
            {
                return true;
            }
            // pax spsummon
            if (desc == Util.GetStringId(CardId.ExosisterPax, 1))
            {
                return paxCallToField;
            }

            return base.OnSelectYesNo(desc);
        }

        /// <summary>
        /// override for returnia's option
        /// </summary>
        public override int OnSelectOption(IList<int> options)
        {
            // check retunia
            int spSummonOption = -1;
            int banishOption = -1;
            int doNothingOption = -1;
            for (int idx = 0; idx < options.Count(); ++ idx)
            {
                int option = options[idx];
                if (option == Util.GetStringId(CardId.ExosisterReturnia, 0))
                {
                    spSummonOption = idx;
                } else if (option == Util.GetStringId(CardId.ExosisterReturnia, 1))
                {
                    banishOption = idx;
                } else if (option == Util.GetStringId(CardId.ExosisterReturnia, 2))
                {
                    doNothingOption = idx;
                }
            }

            if (spSummonOption >= 0 || banishOption >= 0 || doNothingOption >= 0)
            {
                if (spSummonOption < 0 && banishOption < 0)
                {
                    return doNothingOption;
                }
                if (banishOption >= 0)
                {
                    // banish problem card
                    ClientCard target = GetProblematicEnemyCard(true);
                    if (target != null)
                    {
                        AI.SelectCard(target);
                        return banishOption;
                    }

                    // dump banish
                    target = GetBestEnemyCard(false, false);
                    if (target != null)
                    {
                        AI.SelectCard(target);
                        return banishOption;
                    }
                }
                if (spSummonOption >= 0)
                {
                    // TODO
                }
            }

            // check pot
            int potBanish6Option = -1;
            int potBanish3Option = -1;
            for (int idx = 0; idx < options.Count(); ++idx)
            {
                int option = options[idx];
                if (option == Util.GetStringId(CardId.PotofExtravagance, 0))
                {
                    potBanish3Option = idx;
                }
                else if (option == Util.GetStringId(CardId.PotofExtravagance, 1))
                {
                    potBanish6Option = idx;
                }
            }
            if (potBanish3Option >= 0 || potBanish6Option >= 0)
            {
                if (Bot.ExtraDeck.Count() > 9 && potBanish6Option >= 0)
                {
                    return potBanish6Option;
                }
                return potBanish3Option;
            }

            return base.OnSelectOption(options);
        }

        public bool AshBlossomActivate()
        {
            if (CheckWhetherNegated(true) || !CheckLastChainShouldNegated()) return false;
            if (Duel.LastChainPlayer == 1 && Util.GetLastChainCard().IsCode(_CardId.MaxxC))
            {
                if (CheckAtAdvantage())
                {
                    return false;
                }
            }
            CheckDeactiveFlag();
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
            // negate before effect used
            foreach(ClientCard m in Enemy.GetMonsters())
            {
                if (m.IsMonsterShouldBeDisabledBeforeItUseEffect() && !m.IsDisabled() && Duel.LastChainPlayer != 0)
                {
                    if (Card.Location == CardLocation.SpellZone)
                    {
                        for (int i = 0; i < 5; ++ i)
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

            // negate spells
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
                if ( (this_seq * that_seq >= 0 && this_seq + that_seq == 4)
                    || (Util.IsChainTarget(Card))
                    || (LastChainCard != null && LastChainCard.Controller == 1 && LastChainCard.IsCode(_CardId.HarpiesFeatherDuster)))
                {
                    ClientCard target = GetProblematicEnemyMonster(canBeTarget: true);
                    List<ClientCard> enemyMonsters = Enemy.GetMonsters();
                    CheckDeactiveFlag();
                    AI.SelectCard(target);
                    infiniteImpermanenceList.Add(this_seq);
                    return true;
                }
            }
            if ( (LastChainCard == null || LastChainCard.Controller != 1 || LastChainCard.Location != CardLocation.MonsterZone
                || LastChainCard.IsDisabled() || LastChainCard.IsShouldNotBeTarget() || LastChainCard.IsShouldNotBeSpellTrapTarget()) )
                return false;
            // negate monsters
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
                        CheckDeactiveFlag();
                        AI.SelectCard(card);
                        return true;
                    }
                }
            }
            return true;
        }
        
        public bool CalledbytheGraveActivate()
        {
            if (CheckWhetherNegated(true)) return false;
            if (Duel.LastChainPlayer == 1)
            {
                // negate
                if (Util.GetLastChainCard().IsMonster())
                {
                    int code = Util.GetLastChainCard().Id;
                    if (code == 0) return false;
                    if (CheckCalledbytheGrave(code) > 0) return false;
                    if (Util.GetLastChainCard().IsCode(_CardId.MaxxC) && CheckAtAdvantage())
                    {
                         return false;
                    }
                    if (code == CardId.DimensionShifter)
                    {
                        return false;
                    }
                    if (Enemy.Graveyard.GetFirstMatchingCard(card => card.IsMonster() && card.IsOriginalCode(code)) != null)
                    {
                        if (!(Card.Location == CardLocation.SpellZone))
                        {
                            SelectSTPlace(null, true);
                        }
                        AI.SelectCard(code);
                        calledbytheGraveCount[code] = 2;
                        CheckDeactiveFlag();
                        return true;
                    }
                }
                
                // banish target
                foreach (ClientCard cards in Enemy.Graveyard)
                {
                    if (Duel.ChainTargets.Contains(cards))
                    {
                        int code = cards.Id;
                        AI.SelectCard(cards);
                        calledbytheGraveCount[code] = 2;
                        return true;
                    }
                }

                // become targets
                if (Duel.ChainTargets.Contains(Card))
                {
                    List<ClientCard> enemyMonsters = Enemy.Graveyard.GetMatchingCards(card => card.IsMonster()).ToList();
                    if (enemyMonsters.Count > 0)
                    {
                        enemyMonsters.Sort(CardContainer.CompareCardAttack);
                        enemyMonsters.Reverse();
                        int code = enemyMonsters[0].Id;
                        AI.SelectCard(code);
                        calledbytheGraveCount[code] = 2;
                        return true;
                    }
                }
            }

            // avoid danger monster in grave
            if (Duel.LastChainPlayer == 1) return false;
            List<ClientCard> targets = CheckDangerousCardinEnemyGrave(true);
            if (targets.Count() > 0) {
                int code = targets[0].Id;
                if (!(Card.Location == CardLocation.SpellZone))
                {
                    SelectSTPlace(null, true);
                }
                AI.SelectCard(code);
                calledbytheGraveCount[code] = 2;
                return true;
            }

            return false;
        }

        public List<ClientCard> GetPotofExtravaganceBanish()
        {
            List<ClientCard> banishList = new List<ClientCard>();
            ClientCard aaZeus = Bot.ExtraDeck.FirstOrDefault(card => card.IsCode(CardId.DivineArsenalAAZEUS_SkyThunder));
            if (aaZeus != null)
            {
                banishList.Add(aaZeus);
            }

            ClientCard diamond = Bot.ExtraDeck.FirstOrDefault(card => card.IsCode(CardId.StellarknightConstellarDiamond));
            if (diamond != null)
            {
                banishList.Add(diamond);
            }

            ClientCard caduceus = Bot.ExtraDeck.FirstOrDefault(card => card.IsCode(CardId.TellarknightConstellarCaduceus));
            if (caduceus != null)
            {
                banishList.Add(caduceus);
            }

            ClientCard evilswarm = Bot.ExtraDeck.FirstOrDefault(card => card.IsCode(_CardId.EvilswarmExcitonKnight));
            if (evilswarm != null)
            {
                banishList.Add(evilswarm);
            }

            // second asophiel
            if (Bot.ExtraDeck.Count(card => card.IsCode(CardId.ExosisterAsophiel)) > 1)
            {
                ClientCard asophiel2 = Bot.ExtraDeck.FirstOrDefault(card => card.IsCode(CardId.ExosisterAsophiel));
                banishList.Add(asophiel2);
            }

            ClientCard gibrine = Bot.ExtraDeck.FirstOrDefault(card => card.IsCode(CardId.ExosisterGibrine));
            if (gibrine != null)
            {
                banishList.Add(gibrine);
            }

            // 6 done

            // third mikailis
            if (Bot.ExtraDeck.Count(card => card.IsCode(CardId.ExosisterMikailis)) > 2)
            {
                ClientCard mikailis3 = Bot.ExtraDeck.FirstOrDefault(card => card.IsCode(CardId.ExosisterMikailis));
                banishList.Add(mikailis3);
            }

            // first asophiel
            ClientCard asophiel1 = Bot.ExtraDeck.FirstOrDefault(card => card.IsCode(CardId.ExosisterAsophiel) && !banishList.Contains(card));
            if (asophiel1 != null)
            {
                banishList.Add(asophiel1);
            }

            ClientCard donner = Bot.ExtraDeck.FirstOrDefault(card => card.IsCode(CardId.DonnerDaggerFurHire));
            if (donner != null)
            {
                banishList.Add(donner);
            }
            
            // 9 done

            // second kaspitell
            if (Bot.ExtraDeck.Count(card => card.IsCode(CardId.ExosisterKaspitell)) > 1)
            {
                ClientCard kaspitell = Bot.ExtraDeck.FirstOrDefault(card => card.IsCode(CardId.ExosisterKaspitell));
                banishList.Add(kaspitell);
            }

            // second magnifica
            if (Bot.ExtraDeck.Count(card => card.IsCode(CardId.ExosistersMagnifica)) > 1)
            {
                ClientCard magnifica2 = Bot.ExtraDeck.FirstOrDefault(card => card.IsCode(CardId.ExosistersMagnifica));
                banishList.Add(magnifica2);
            }

            // second mikailis
            if (Bot.ExtraDeck.Count(card => card.IsCode(CardId.ExosisterMikailis)) > 1)
            {
                ClientCard mikailis2 = Bot.ExtraDeck.FirstOrDefault(card => card.IsCode(CardId.ExosisterMikailis) && !banishList.Contains(card));
                banishList.Add(mikailis2);
            }

            // first magnifica
            ClientCard magnifica1 = Bot.ExtraDeck.FirstOrDefault(card => card.IsCode(CardId.ExosistersMagnifica) && !banishList.Contains(card));
            if (magnifica1 != null)
            {
                banishList.Add(magnifica1);
            }

            // first kaspitell
            ClientCard kaspitell1 = Bot.ExtraDeck.FirstOrDefault(card => card.IsCode(CardId.ExosisterKaspitell) && !banishList.Contains(card));
            if (kaspitell1 != null)
            {
                banishList.Add(kaspitell1);
            }

            // first mikailis1
            ClientCard mikailis1 = Bot.ExtraDeck.FirstOrDefault(card => card.IsCode(CardId.ExosisterMikailis) && !banishList.Contains(card));
            if (mikailis1 != null)
            {
                banishList.Add(mikailis1);
            }

            return banishList;
        }

        public bool PotofExtravaganceActivate()
        {
            if (CheckWhetherNegated())
            {
                return false;
            }
            List<ClientCard> banishList = GetPotofExtravaganceBanish();

            List<int> addToHandOrderList = new List<int>();

            bool marthaActivatable = CheckMarthaActivatable();
            if (marthaActivatable)
            {
                if (!Bot.HasInHand(CardId.ExosisterMartha))
                {
                    addToHandOrderList.Add(CardId.ExosisterMartha);
                }
                if (Bot.HasInHand(CardId.ExosisterMartha) && !Bot.HasInHandOrInSpellZone(_CardId.CalledByTheGrave))
                {
                    addToHandOrderList.Add(_CardId.CalledByTheGrave);
                }
            }
            int exosisterCount = Bot.Hand.Count(card => card?.Data != null && card.HasSetcode(SetcodeExosister));
            if (!stellaEffect1Activated && CheckCalledbytheGrave(CardId.ExosisterStella) == 0)
            {
                if (!Bot.HasInHand(CardId.ExosisterStella) && exosisterCount > 0)
                {
                    addToHandOrderList.Add(CardId.ExosisterStella);
                }
                if (Bot.HasInHand(CardId.ExosisterStella) && exosisterCount == 0)
                {
                    addToHandOrderList.AddRange(new List<int>{
                        CardId.ExosisterSophia, CardId.ExosisterIrene, CardId.ExosisterStella, CardId.ExosisterMartha, CardId.ExosisterElis});
                }
            }
            if (exosisterCount >= 0 && !Bot.HasInHandOrInSpellZone(CardId.ExosisterReturnia))
            {
                addToHandOrderList.Add(CardId.ExosisterReturnia);
            }
            List<int> remainOrderList = new List<int>{
                CardId.Aratama, CardId.Sakitama, _CardId.MaxxC, _CardId.AshBlossom, _CardId.InfiniteImpermanence,
                _CardId.CalledByTheGrave, CardId.ExosisterVadis, CardId.ExosisterReturnia, CardId.ExosisterPax
            };
            addToHandOrderList.AddRange(remainOrderList);

            AI.SelectCard(banishList);
            AI.SelectNextCard(addToHandOrderList);
            if (!(Card.Location == CardLocation.SpellZone))
            {
                SelectSTPlace(null, true);
            }

            potActivate = true;
            return true;
        }

        public bool SakitamaActivate()
        {
            // summon
            if (Card.Location == CardLocation.Hand)
            {
                // summon for xyz
                if (Bot.GetMonsters().Count(card => CheckAbleForXyz(card)) == 1)
                {
                    AI.SelectCard(CardId.Aratama, CardId.Sakitama);
                    sakitamaEffect1Activated = true;
                    return true;
                }

                // summon for summon donner
                if (!CheckLessOperation() && Bot.HasInExtra(CardId.DonnerDaggerFurHire) &&
                    !Bot.HasInHand(CardId.ExosisterMartha) || Bot.HasInHandOrInSpellZone(CardId.ExosisterReturnia))
                {
                    List<ClientCard> illegalList = Bot.GetMonsters().Where(card => card.IsFaceup() && !card.HasType(CardType.Xyz) && card.Level != 4
                        && (card.Data == null || !card.HasSetcode(SetcodeExosister))).ToList();
                    if (illegalList.Count() > 0)
                    {
                        if (illegalList.Count() == 1)
                        {
                            List<ClientCard> otherMaterialList = Bot.GetMonsters().Where(card => card.IsFaceup() && !card.HasType(CardType.Xyz)).ToList();
                            otherMaterialList.Sort(CardContainer.CompareCardAttack);
                            illegalList.AddRange(otherMaterialList);
                        }
                        if (illegalList.Count() == 1)
                        {
                            Logger.DebugWriteLine("===Exosister: activate sakitama for donner");
                            AI.SelectCard(CardId.Aratama, CardId.Sakitama);
                            sakitamaEffect1Activated = true;
                            return true;
                        }
                    }
                }
                return false;
            }
            // add to hand
            if (Card.Location == CardLocation.Grave)
            {
                AI.SelectCard(CardId.Sakitama, CardId.Aratama);
                return true;
            }
            return true;
        }

        public bool DonnerDaggerFurHireActivate()
        {
            if (CheckAtAdvantage() && !Bot.HasInHand(CardId.ExosisterMartha) && !Bot.HasInHandOrInSpellZone(CardId.ExosisterReturnia))
            {
                return false;
            }

            ClientCard targetCard = Util.GetProblematicEnemyMonster(canBeTarget: true);
            if (targetCard == null)
            {
                List<ClientCard> enemyMonsters = Enemy.GetMonsters();
                if (enemyMonsters.Count() > 0)
                {
                    enemyMonsters.Sort(CardContainer.CompareCardAttack);
                    enemyMonsters.Reverse();
                    targetCard = enemyMonsters[0];
                }
            }

            if (targetCard != null)
            {
                AI.SelectCard(Card);
                AI.SelectNextCard(targetCard);
                return true;
            }

            return false;
        }

        public bool ExosisterElisActivate()
        {
            if (ActivateDescription != Util.GetStringId(CardId.ExosisterElis, 0))
            {
                return false;
            }

            if (Bot.GetMonsters().Count(card => CheckAbleForXyz(card)) == 1)
            {
                elisEffect1Activated = true;
                return true;
            }

            return false;
        }

        public bool ExosisterStellaActivate()
        {
            return ExosisterStellaActivateInner(true);
        }

        public bool ExosisterStellaSecondActivate()
        {
            return ExosisterStellaActivateInner(false);
        }

        public bool ExosisterStellaActivateInner(bool checkMartha = false)
        {
            if (ActivateDescription != Util.GetStringId(CardId.ExosisterStella, 0) || CheckWhetherNegated(true))
            {
                return false;
            }

            bool ableToXyz = Bot.GetMonsters().Count(card => CheckAbleForXyz(card)) >= 2;

            if (CheckLessOperation() && ableToXyz)
            {
                return false;
            }
            if (checkMartha && Bot.HasInHand(CardId.ExosisterMartha) && ableToXyz
                   && Bot.Hand.Count(card => card.IsMonster() && card.HasSetcode(CardId.ExosisterMartha)) == 1)
            {
                return false;
            }

            AI.SelectCard(CardId.ExosisterSophia, CardId.ExosisterIrene, CardId.ExosisterStella, CardId.ExosisterElis);
            stellaEffect1Activated = true;
            return true;
        }

        public bool ExosisterIreneActivate()
        {
            if (ActivateDescription != Util.GetStringId(CardId.ExosisterIrene, 0) || CheckWhetherNegated(true))
            {
                return false;
            }

            List<int> shuffleList = new List<int>();
            foreach (int cardId in new List<int>{CardId.ExosisterIrene, CardId.ExosisterSophia, CardId.ExosisterArment})
            {
                if (Bot.HasInHand(cardId))
                {
                    shuffleList.Add(cardId);
                }
            }
            if (elisEffect1Activated || Bot.Hand.Count(card => card.IsCode(CardId.ExosisterElis)) > 1) 
            {
                shuffleList.Add(CardId.ExosisterElis);
            }
            foreach (int cardId in new List<int>{CardId.ExosisterPax, CardId.ExosisterReturnia, CardId.ExosisterVadis})
            {
                if ((oncePerTurnEffectActivatedList.Contains(cardId) && Bot.HasInHand(cardId)) || Bot.Hand.Count(card => card.IsCode(cardId)) > 1)
                {
                    shuffleList.Add(cardId);
                }
            }
            
            if (shuffleList.Count() > 0)
            {
                Logger.DebugWriteLine("===Exosister: irene return " + shuffleList[0]);
                AI.SelectCard(shuffleList);
                return true;
            }
            return false;
        }

        public bool ExosisterSophiaActivate()
        {
            if (ActivateDescription == Util.GetStringId(CardId.ExosisterSophia, 0) && !CheckWhetherNegated(true))
            {
                sophiaEffect1Activated = true;
                return true;
            }
            return false;
        }

        public bool ExosisterMarthaActivate()
        {
            if (ActivateDescription != Util.GetStringId(CardId.ExosisterMartha, 0)) {
                return false;
            }
            if (CheckLessOperation() && Bot.GetMonsterCount() > 0)
            {
                return false;
            }

            marthaEffect1Activated = true;
            return true;
        }

        public bool DefaultExosisterTransform()
        {
            List<int> canTransformList = new List<int>
            {
                CardId.ExosisterElis, CardId.ExosisterStella, CardId.ExosisterIrene, CardId.ExosisterSophia, CardId.ExosisterMartha
            };
            if (Card.IsDisabled() || !canTransformList.Contains(Card.Id))
            {
                return false;
            }
            List<int> checkTransformCode = new List<int>{
                Util.GetStringId(CardId.ExosisterElis, 1),
                Util.GetStringId(CardId.ExosisterStella, 1),
                Util.GetStringId(CardId.ExosisterIrene, 1),
                Util.GetStringId(CardId.ExosisterSophia, 1),
                Util.GetStringId(CardId.ExosisterMartha, 1)
            };
            if (!checkTransformCode.Contains(ActivateDescription) && ActivateDescription != -1)
            {
                return false;
            }
            
            // mikailis
            if (!Bot.HasInMonstersZone(CardId.ExosisterMikailis) && !mikailisEffect1Activated && (Duel.Player == 1 || !mikailisEffect3Activated)
                && !transformDestList.Contains(CardId.ExosisterMikailis) && Bot.HasInExtra(CardId.ExosisterMikailis))
            {
                exosisterTransformEffectList.Add(Card.Id);
                transformDestList.Add(CardId.ExosisterMikailis);
                return true;
            }

            // kaspitell on bot's turn
            if (!Bot.HasInMonstersZone(CardId.ExosisterKaspitell) && !kaspitellEffect3Activated && Duel.Player == 0
                && !transformDestList.Contains(CardId.ExosisterKaspitell) && Bot.HasInExtra(CardId.ExosisterKaspitell))
            {
                exosisterTransformEffectList.Add(Card.Id);
                transformDestList.Add(CardId.ExosisterKaspitell);
                return true;
            }

            // gibrine
            if (!Bot.HasInMonstersZone(CardId.ExosisterGibrine) && !gibrineEffect1Activated
                && !transformDestList.Contains(CardId.ExosisterGibrine) && Bot.HasInExtra(CardId.ExosisterGibrine))
            {
                exosisterTransformEffectList.Add(Card.Id);
                transformDestList.Add(CardId.ExosisterGibrine);
                return true;
            }

            // asophiel
            if (!Bot.HasInMonstersZone(CardId.ExosisterAsophiel) && !asophielEffect1Activated
                && !transformDestList.Contains(CardId.ExosisterAsophiel) && Bot.HasInExtra(CardId.ExosisterAsophiel))
            {
                exosisterTransformEffectList.Add(Card.Id);
                transformDestList.Add(CardId.ExosisterAsophiel);
                return true;
            }

            // kaspitell on bot's turn
            if (!Bot.HasInMonstersZone(CardId.ExosisterKaspitell) && !kaspitellEffect1Activated
                && !transformDestList.Contains(CardId.ExosisterKaspitell) && Bot.HasInExtra(CardId.ExosisterKaspitell))
            {
                exosisterTransformEffectList.Add(Card.Id);
                transformDestList.Add(CardId.ExosisterKaspitell);
                return true;
            }

            return false;
        }

        public bool ExosisterMikailisActivate()
        {
            // banish
            if (ActivateDescription == Util.GetStringId(CardId.ExosisterMikailis, 0))
            {
                // activate after search
                if (Duel.Player == 0 && !mikailisEffect3Activated && Duel.Phase < DuelPhase.End && !DefaultOnBecomeTarget())
                {
                    return false;
                }

                // banish problem card
                ClientCard target = GetProblematicEnemyCard(true);
                if (target != null && Duel.LastChainPlayer != 0)
                {
                    removeChosenList.Add(target);
                    mikailisEffect1Activated = true;
                    AI.SelectCard(target);
                    return true;
                }

                // banish target
                if (Duel.LastChainPlayer == 1)
                {
                    List<ClientCard> targetList = Duel.LastChainTargets.Where(card => card.Controller == 1 &&
                        (card.Location == CardLocation.Grave || card.Location == CardLocation.MonsterZone || card.Location == CardLocation.SpellZone || card.Location == CardLocation.FieldZone)).ToList();
                    if (targetList.Count() > 0)
                    {
                        mikailisEffect1Activated = true;
                        AI.SelectCard(ShuffleCardList(targetList));
                        return true;
                    }
                }

                // dump banish
                target = GetBestEnemyCard(false, true, true);
                if ((DefaultOnBecomeTarget() && !Util.ChainContainsCard(_CardId.EvenlyMatched)) || Bot.UnderAttack || (Duel.Phase == DuelPhase.End && Duel.LastChainPlayer != 0)
                    || (Duel.Player == 0 && Bot.GetMonsters().Count(card => card.HasType(CardType.Xyz) && card.Rank == 4 && card.HasSetcode(SetcodeExosister)) == 2 && Duel.LastChainPlayer != 0)
                    || (Duel.Player == 1 && Enemy.GetMonsterCount() >= 2))
                {
                    mikailisEffect1Activated = true;
                    AI.SelectCard(target);
                    return true;
                }
                return false;
            }
            
            // search
            if (CheckWhetherNegated(true))
            {
                return false;
            }

            List<int> searchTarget = new List<int>{
                CardId.ExosisterReturnia,
                CardId.ExosisterVadis,
                CardId.ExosisterPax,
                CardId.ExosisterArment
            };
            List<int> firstSearchList = new List<int>();
            List<int> lastSearchList = new List<int>();
            foreach (int cardId in searchTarget)
            {
                if (Bot.HasInHandOrInSpellZone(cardId) || CheckRemainInDeck(cardId) == 0)
                {
                    lastSearchList.Add(cardId);
                    continue;
                }
                if (cardId == CardId.ExosisterReturnia && Bot.GetMonsters().Any(card => card.IsFacedown() || !card.HasSetcode(SetcodeExosister)))
                {
                    lastSearchList.Add(cardId);
                    continue;
                }
                firstSearchList.Add(cardId);
            }
            firstSearchList.AddRange(lastSearchList);

            mikailisEffect3Activated = true;
            SelectDetachMaterial(Card);
            AI.SelectNextCard(firstSearchList);
            return true;
        }

        public bool ExosisterKaspitellActivate()
        {
            // block spsummon from GY
            if (ActivateDescription == Util.GetStringId(CardId.ExosisterKaspitell, 0) || ActivateDescription == -1)
            {
                if (Enemy.HasInMonstersZone(CardId.InspectorBoarder, true))
                {
                    return false;
                }
                kaspitellEffect1Activated = true;
                return true;
            }

            // search
            if (CheckWhetherNegated(true))
            {
                return false;
            }

            // search martha for activate
            if (CheckMarthaActivatable() && CheckRemainInDeck(CardId.ExosisterMartha) > 0 && !Bot.HasInHand(CardId.ExosisterMartha))
            {
                kaspitellEffect3Activated = true;
                SelectDetachMaterial(Card);
                AI.SelectNextCard(CardId.ExosisterMartha);
                return true;
            }
            // search sophia for draw
            if (!summoned && !sophiaEffect1Activated && CheckCalledbytheGrave(CardId.ExosisterSophia) == 0 && !Bot.HasInHand(CardId.ExosisterSophia)
                 && (Bot.GetMonsters().Count(card => CheckAbleForXyz(card)) == 1 || (Bot.HasInHand(CardId.ExosisterElis) && !elisEffect1Activated)))
            {
                kaspitellEffect3Activated = true;
                SelectDetachMaterial(Card);
                AI.SelectNextCard(CardId.ExosisterSophia);
                return true;
            }
            // search stella for next xyz
            if (!summoned && !Bot.HasInHand(CardId.ExosisterStella) && !stellaEffect1Activated && CheckCalledbytheGrave(CardId.ExosisterStella) == 0
                && CheckRemainInDeck(CardId.ExosisterStella) > 0 && Bot.Hand.Any(card => card?.Data != null && card.IsMonster() && card.HasSetcode(SetcodeExosister)))
            {
                kaspitellEffect3Activated = true;
                SelectDetachMaterial(Card);
                AI.SelectNextCard(CardId.ExosisterStella);
                return true;
            }
            kaspitellEffect3Activated = true;
            SelectDetachMaterial(Card);
            AI.SelectNextCard(CardId.ExosisterMartha, CardId.ExosisterStella, CardId.ExosisterElis, CardId.ExosisterSophia, CardId.ExosisterIrene);
            return true;
        }

        public bool ExosisterGibrineActivate()
        {
            // negate
            if (ActivateDescription == Util.GetStringId(CardId.ExosisterGibrine, 0))
            {
                if (Duel.Player == 1)
                {
                    ClientCard target = Enemy.MonsterZone.GetShouldBeDisabledBeforeItUseEffectMonster();
                    if (target != null)
                    {
                        gibrineEffect1Activated = true;
                        AI.SelectCard(target);
                        return true;
                    }
                }

                ClientCard LastChainCard = Util.GetLastChainCard();
                if (LastChainCard != null && LastChainCard.Controller == 1 && LastChainCard.Location == CardLocation.MonsterZone &&
                !LastChainCard.IsDisabled() && !LastChainCard.IsShouldNotBeTarget() && !LastChainCard.IsShouldNotBeMonsterTarget())
                {
                    gibrineEffect1Activated = true;
                    AI.SelectCard(LastChainCard);
                    return true;
                }

                return false;
            }

            // gain atk
            if (CheckWhetherNegated(true))
            {
                return false;
            }
            gibrineEffect3Activated = true;
            SelectDetachMaterial(Card);
            return true;
        }

        public bool ExosisterAsophielActivate()
        {
            // block activate from GY
            if (ActivateDescription == Util.GetStringId(CardId.ExosisterAsophiel, 0) || ActivateDescription == -1)
            {
                if (Enemy.HasInMonstersZone(CardId.InspectorBoarder, true))
                {
                    return false;
                }
                asophielEffect1Activated = true;
                return true;
            }

            // return hand
            if (CheckWhetherNegated(true))
            {
                return false;
            }
            ClientCard targetCard = Util.GetProblematicEnemyMonster(0, true);
            if (targetCard != null)
            {
                asophielEffect3Activated = true;
                SelectDetachMaterial(Card);
                AI.SelectNextCard(targetCard);
                return true;
            }

            return false;
        }

        public bool ExosistersMagnificaActivateTrigger()
        {
            // sp summon
            if (ActivateDescription == Util.GetStringId(CardId.ExosistersMagnifica, 1))
            {
                // return after effect used
                if (activatedMagnificaList.Contains(Card))
                {
                    // return to Mikailis for danger card
                    if (Card.Overlays.Contains(CardId.ExosisterMikailis) && !mikailisEffect1Activated)
                    {
                        ClientCard target = GetProblematicEnemyCard(true);
                        if (target != null && !Duel.CurrentChain.Any(card => card == Card))
                        {
                            transformDestList.Add(CardId.ExosisterMikailis);
                            return true;
                        }
                    }

                    // negate important card
                    if (Card.Overlays.Contains(CardId.ExosisterGibrine) && !gibrineEffect1Activated)
                    {
                        ClientCard target = Enemy.MonsterZone.GetShouldBeDisabledBeforeItUseEffectMonster();
                        if (target != null)
                        {
                            transformDestList.Add(CardId.ExosisterGibrine);
                            return true;
                        }
                    }
                }
                
                // become target
                if (DefaultOnBecomeTarget() || (Duel.CurrentChain.Any(c => c == Card) && Duel.LastChainPlayer != 0))
                {
                    targetedMagnificaList.Add(Card);
                    transformDestList.AddRange(new List<int>{CardId.ExosistersMagnifica, CardId.ExosisterMikailis, CardId.ExosisterGibrine, CardId.ExosisterKaspitell, CardId.ExosisterAsophiel});
                    return true;
                }
            }
            return false;
        }
        public bool ExosistersMagnificaActivateBanish()
        {
            // banish
            if (ActivateDescription == Util.GetStringId(CardId.ExosistersMagnifica, 0))
            {
                if (CheckWhetherNegated())
                {
                    return false;
                }
                // banish problem card
                ClientCard target = GetProblematicEnemyCard();
                bool isProblemCard = false;
                if (target != null)
                {
                    isProblemCard = true;
                    Logger.DebugWriteLine("===Exosister: magnifica target 1: " + target?.Name);
                }

                // banish target
                if (Duel.LastChainPlayer == 1 && target == null)
                {
                    List<ClientCard> currentTargetList = Duel.LastChainTargets.Where(card => card.Controller == 1 &&
                        (card.Location == CardLocation.MonsterZone || card.Location == CardLocation.SpellZone || card.Location == CardLocation.FieldZone)).ToList();
                    if (currentTargetList.Count() > 0)
                    {
                        target = ShuffleCardList(currentTargetList)[0];
                        Logger.DebugWriteLine("===Exosister: magnifica target 2: " + target?.Name);
                    }
                }

                // dump banish
                if (target == null)
                {
                    target = GetBestEnemyCard(false, false);
                    bool check1 = !DefaultOnBecomeTarget() || Util.ChainContainsCard(_CardId.EvenlyMatched);
                    bool check2 = !targetedMagnificaList.Contains(Card);
                    bool check3 = !Bot.UnderAttack;
                    bool check4 = Duel.Phase != DuelPhase.End;
                    bool check5 = Duel.Player == 0 || Enemy.GetMonsterCount() < 2;
                    Logger.DebugWriteLine("===Exosister: magnifica check flag: " + check1 + " " + check2 + " " + check3 + " " + check4 + " " + check5);
                    if (check1 && check2 && check3 && check4 && check5)
                    {
                        target = null;
                    }
                }

                if (target != null && (Duel.LastChainPlayer != 0 || Util.GetLastChainCard() == Card))
                {
                    if (isProblemCard)
                    {
                        removeChosenList.Add(target);
                    }
                    Logger.DebugWriteLine("===Exosister: magnifica target final: " + target?.Name);
                    activatedMagnificaList.Add(Card);
                    AI.SelectCard(CardId.ExosisterGibrine, CardId.ExosisterAsophiel, CardId.ExosisterKaspitell, CardId.ExosisterMikailis);
                    AI.SelectNextCard(target);
                    return true;
                }
                
                return false;
            }
            return false;
        }

        public bool ExosisterPaxActivate()
        {
            if (potActivate)
            {
                return false;
            }

            List<int> checkListForSpSummon = new List<int>{
                CardId.ExosisterSophia, CardId.ExosisterIrene, CardId.ExosisterStella, CardId.ExosisterMartha, CardId.ExosisterElis
            };
            List<int> checkListForSearch = new List<int>{
                CardId.ExosisterMartha, CardId.ExosisterStella, CardId.ExosisterVadis, CardId.ExosisterReturnia, CardId.ExosisterSophia,
                CardId.ExosisterIrene, CardId.ExosisterArment, CardId.ExosisterElis
            };
            if (Duel.Player == 0 && Duel.LastChainPlayer != 0)
            {
                // search returnia for banish
                if (CheckAtAdvantage() && GetProblematicEnemyCard(true) != null && CheckRemainInDeck(CardId.ExosisterReturnia) > 0 && !Bot.HasInHandOrInSpellZone(CardId.ExosisterReturnia))
                {
                    if (Bot.GetMonsterCount() > 0 && Bot.GetMonsters().All(card => card.IsFaceup() && card.HasSetcode(SetcodeExosister)))
                    {
                        if (!(Card.Location == CardLocation.SpellZone))
                        {
                            SelectSTPlace(null, true);
                        }
                        oncePerTurnEffectActivatedList.Add(Card.Id);
                        AI.SelectCard(CardId.ExosisterReturnia);
                        paxCallToField = false;
                        return true;
                    }
                }

                // search martha for activate
                if (CheckMarthaActivatable() && CheckRemainInDeck(CardId.ExosisterMartha) > 0 && !Bot.HasInHand(CardId.ExosisterMartha))
                {
                    if (!(Card.Location == CardLocation.SpellZone))
                    {
                        SelectSTPlace(null, true);
                    }
                    oncePerTurnEffectActivatedList.Add(Card.Id);
                    AI.SelectCard(CardId.ExosisterMartha);
                    paxCallToField = false;
                    return true;
                }

                // stella relative
                if (!stellaEffect1Activated && CheckCalledbytheGrave(CardId.ExosisterStella) == 0)
                {
                    // try to search stella
                    if (Bot.Hand.Count(card => card.IsCode(CardId.ExosisterStella)) == 0 && CheckRemainInDeck(CardId.ExosisterStella) > 0)
                    {
                        bool shouldSpSummon = !CheckLessOperation() && summoned && Bot.HasInMonstersZoneOrInGraveyard(CardId.ExosisterElis);
                        if (Bot.Hand.Any(card => card?.Data != null && card.IsMonster() && card.HasSetcode(SetcodeExosister)))
                        {
                            if (!(Card.Location == CardLocation.SpellZone))
                            {
                                SelectSTPlace(null, true);
                            }
                            oncePerTurnEffectActivatedList.Add(Card.Id);
                            AI.SelectCard(CardId.ExosisterStella);
                            paxCallToField = shouldSpSummon;
                            return true;
                        }
                    }

                    // search monster for stella to summon
                    bool searchExosisterMonster = false;
                    if (Bot.HasInHand(CardId.ExosisterStella) && Bot.Hand.Count(card => card?.Data != null && card.IsMonster() && card.HasSetcode(SetcodeExosister)) == 1)
                    {
                        searchExosisterMonster = true;
                    }
                    if (Bot.HasInMonstersZone(CardId.ExosisterStella) && Bot.Hand.Count(card => card?.Data != null && card.IsMonster() && card.HasSetcode(SetcodeExosister)) == 0)
                    {
                        searchExosisterMonster = true;
                    }
                    if (searchExosisterMonster)
                    {
                        if (!(Card.Location == CardLocation.SpellZone))
                        {
                            SelectSTPlace(null, true);
                        }
                        oncePerTurnEffectActivatedList.Add(Card.Id);
                        AI.SelectCard(CardId.ExosisterSophia, CardId.ExosisterIrene, CardId.ExosisterMartha, CardId.ExosisterStella, CardId.ExosisterElis);
                        paxCallToField = false;
                        return true;
                    }
                }

                // addition summon
                if (Bot.GetMonsters().Count(card => CheckAbleForXyz(card)) == 1 && summoned && !CheckLessOperation())
                {
                    if (    (sakitamaEffect1Activated || !Bot.HasInHand(CardId.Sakitama))
                        &&  (stellaEffect1Activated   || !Bot.HasInMonstersZone(CardId.ExosisterStella))
                        &&  (elisEffect1Activated     || !Bot.HasInHand(CardId.ExosisterElis))
                    )
                    {
                        foreach (int checkId in checkListForSpSummon)
                        {
                            int checkTarget = CheckExosisterMentionCard(checkId);
                            if (checkTarget > 0 && Bot.HasInMonstersZoneOrInGraveyard(checkId) && CheckRemainInDeck(checkTarget) > 0)
                            {
                                if (!(Card.Location == CardLocation.SpellZone))
                                {
                                    SelectSTPlace(null, true);
                                }
                                oncePerTurnEffectActivatedList.Add(Card.Id);
                                AI.SelectCard(checkId);
                                paxCallToField = true;
                                return true;
                            }
                        }
                    }
                }
            }

            // in danger
            bool inDanger = CheckInDanger();

            // trigger transform
            CheckEnemyMoveGrave();

            bool forReturnia = false;
            if (!oncePerTurnEffectActivatedList.Contains(CardId.ExosisterReturnia) && Bot.HasInSpellZone(CardId.ExosisterReturnia) && Bot.GetMonsters().Count() == 0)
            {
                forReturnia = true;
            }

            // become target
            if (enemyMoveGrave || DefaultOnBecomeTarget() || inDanger || forReturnia)
            {
                List<int> checkList = checkListForSpSummon;
                bool shouldSpSummon = enemyMoveGrave || inDanger || forReturnia;
                if (!shouldSpSummon && !Bot.HasInMonstersZone(new List<int>{
                    CardId.ExosisterElis, CardId.ExosisterStella, CardId.ExosisterIrene, CardId.ExosisterSophia, CardId.ExosisterMartha}))
                {
                    shouldSpSummon = true;
                }
                if (CheckAtAdvantage() && !enemyMoveGrave)
                {
                    shouldSpSummon = false;
                    checkList = checkListForSearch;
                }
                foreach (int checkId in checkList)
                {
                    bool checkSuccessFlag = false;

                    if (shouldSpSummon)
                    {
                        int checkTarget = CheckExosisterMentionCard(checkId);
                        checkSuccessFlag = checkTarget > 0 && Bot.HasInMonstersZoneOrInGraveyard(checkTarget) && CheckRemainInDeck(checkId) > 0
                                && !exosisterTransformEffectList.Contains(checkId) && !Bot.HasInMonstersZone(checkId);
                    } else 
                    {
                        checkSuccessFlag = !Bot.HasInHandOrHasInMonstersZone(checkId) && !Bot.HasInSpellZone(checkId) && CheckRemainInDeck(checkId) > 0;
                    }

                    if (checkSuccessFlag)
                    {
                        if (!(Card.Location == CardLocation.SpellZone))
                        {
                            SelectSTPlace(null, true);
                        }
                        oncePerTurnEffectActivatedList.Add(Card.Id);
                        AI.SelectCard(checkId);
                        paxCallToField = shouldSpSummon;
                        return true;
                    }
                }
            }

            return false;
        }

        public bool ExosisterPaxActivateForEndSearch()
        {
            if (potActivate)
            {
                return false;
            }

            if (Duel.Player == 0 || Duel.Phase >= DuelPhase.End)
            {
                // search spell/trap
                List<int> checkSpellTrapListForSearch = new List<int>{
                    CardId.ExosisterVadis, CardId.ExosisterMartha, CardId.ExosisterReturnia, CardId.ExosisterStella, CardId.ExosisterSophia,
                    CardId.ExosisterIrene, CardId.ExosisterArment, CardId.ExosisterElis
                };
                foreach (int checkId in checkSpellTrapListForSearch)
                {
                    if (!Bot.HasInHandOrHasInMonstersZone(checkId) && !Bot.HasInSpellZone(checkId) && CheckRemainInDeck(checkId) > 0)
                    {
                        if (!(Card.Location == CardLocation.SpellZone))
                        {
                            SelectSTPlace(null, true);
                        }
                        oncePerTurnEffectActivatedList.Add(Card.Id);
                        AI.SelectCard(CardId.ExosisterSophia, CardId.ExosisterIrene, CardId.ExosisterMartha, CardId.ExosisterStella, CardId.ExosisterElis);
                        paxCallToField = false;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool ExosisterArmentActivate()
        {
            ClientCard activateTarget = null;

            if (Duel.Player == 0)
            {
                bool decided = false;

                // addition summon
                if (Bot.GetMonsters().Count(card => CheckAbleForXyz(card)) == 1 && summoned && !CheckLessOperation())
                {
                    if (    (sakitamaEffect1Activated || !Bot.HasInHand(CardId.Sakitama))
                        &&  (stellaEffect1Activated   || !Bot.HasInMonstersZone(CardId.ExosisterStella))
                        &&  (elisEffect1Activated     || !Bot.HasInHand(CardId.ExosisterElis))
                    )
                    {
                        decided = true;
                    }
                }

                if (Duel.LastChainPlayer == 1)
                {
                    foreach (ClientCard target in Duel.LastChainTargets)
                    {
                        if (target.Controller == 0 && target.Location == CardLocation.MonsterZone && target.IsFaceup() && target.HasSetcode(SetcodeExosister))
                        {
                            activateTarget = target;
                            decided = true;
                            break;
                        }
                    }
                }

                if (!decided)
                {
                    return false;
                }
            }

            if (activateTarget == null && Duel.LastChainPlayer == 1)
            {
                {
                    foreach (ClientCard target in Duel.LastChainTargets)
                    {
                        if (target.Controller == 0 && target.Location == CardLocation.MonsterZone && target.IsFaceup() && target.HasSetcode(SetcodeExosister))
                        {
                            activateTarget = target;
                            break;
                        }
                    }
                }
            }

            if (activateTarget == null)
            {
                List<ClientCard> targetList = Bot.GetMonsters().Where(card => card.IsFaceup() && card.HasSetcode(SetcodeExosister) && !card.HasType(CardType.Xyz)).ToList();
                if (targetList.Count() > 0)
                {
                    targetList.Sort(CardContainer.CompareCardAttack);
                    activateTarget = targetList[0];
                }
            }

            if (activateTarget == null)
            {
                return false;
            }

            // mikailis
            if (!Bot.HasInMonstersZone(CardId.ExosisterMikailis) && !mikailisEffect1Activated && (Duel.Player == 1 || !mikailisEffect3Activated)
                && !transformDestList.Contains(CardId.ExosisterMikailis) && Bot.HasInExtra(CardId.ExosisterMikailis))
            {
                if (!(Card.Location == CardLocation.SpellZone))
                {
                    SelectSTPlace(null, true);
                }
                AI.SelectCard(activateTarget);
                transformDestList.Add(CardId.ExosisterMikailis);
                return true;
            }

            // kaspitell on bot's turn
            if (!Bot.HasInMonstersZone(CardId.ExosisterKaspitell) && !kaspitellEffect3Activated && Duel.Player == 0
                && !transformDestList.Contains(CardId.ExosisterKaspitell) && Bot.HasInExtra(CardId.ExosisterKaspitell))
            {
                if (!(Card.Location == CardLocation.SpellZone))
                {
                    SelectSTPlace(null, true);
                }
                AI.SelectCard(activateTarget);
                transformDestList.Add(CardId.ExosisterKaspitell);
                return true;
            }

            // gibrine
            if (!Bot.HasInMonstersZone(CardId.ExosisterGibrine) && !gibrineEffect1Activated
                && !transformDestList.Contains(CardId.ExosisterGibrine) && Bot.HasInExtra(CardId.ExosisterGibrine))
            {
                if (!(Card.Location == CardLocation.SpellZone))
                {
                    SelectSTPlace(null, true);
                }
                AI.SelectCard(activateTarget);
                transformDestList.Add(CardId.ExosisterGibrine);
                return true;
            }

            // asophiel
            if (!Bot.HasInMonstersZone(CardId.ExosisterAsophiel) && !asophielEffect1Activated
                && !transformDestList.Contains(CardId.ExosisterAsophiel) && Bot.HasInExtra(CardId.ExosisterAsophiel))
            {
                if (!(Card.Location == CardLocation.SpellZone))
                {
                    SelectSTPlace(null, true);
                }
                AI.SelectCard(activateTarget);
                transformDestList.Add(CardId.ExosisterAsophiel);
                return true;
            }

            // kaspitell on bot's turn
            if (!Bot.HasInMonstersZone(CardId.ExosisterKaspitell) && !kaspitellEffect1Activated
                && !transformDestList.Contains(CardId.ExosisterKaspitell) && Bot.HasInExtra(CardId.ExosisterKaspitell))
            {
                if (!(Card.Location == CardLocation.SpellZone))
                {
                    SelectSTPlace(null, true);
                }
                AI.SelectCard(activateTarget);
                transformDestList.Add(CardId.ExosisterKaspitell);
                return true;
            }

            return false;
        }

        public bool ExosisterVadisActivate()
        {
            List<int> checkListForSpSummon = new List<int>{
                CardId.ExosisterSophia, CardId.ExosisterIrene, CardId.ExosisterStella, CardId.ExosisterMartha, CardId.ExosisterElis
            };

            bool decideToActivate = false;
            bool checkTransform = false;

            // special summon for xyz
            if (Duel.Player == 0 && Duel.Phase > DuelPhase.Draw && !CheckLessOperation())
            {
                decideToActivate = true;
            }

            // move grave
            CheckEnemyMoveGrave();
            if (enemyMoveGrave)
            {
                decideToActivate = true;
                checkTransform = true;
            }
            
            // for returia
            if (!oncePerTurnEffectActivatedList.Contains(CardId.ExosisterReturnia) && Bot.HasInSpellZone(CardId.ExosisterReturnia) && Bot.GetMonsters().Count() == 0)
            {
                decideToActivate = true;
            }

            if (CheckInDanger() || (DefaultOnBecomeTarget() && !Util.ChainContainsCard(_CardId.EvenlyMatched)))
            {
                decideToActivate = true;
            }

            if (decideToActivate)
            {
                foreach (int checkId in checkListForSpSummon)
                {
                    int checkTarget = CheckExosisterMentionCard(checkId);
                    if (checkTarget > 0 && CheckRemainInDeck(checkId) > 0 && CheckRemainInDeck(checkTarget) > 0)
                    {
                        if (checkTransform)
                        {
                            int canTransformCount = 0;
                            foreach (int transformCheckId in new List<int>{checkId, checkTarget})
                            {
                                if (!Bot.HasInMonstersZone(checkId) && !exosisterTransformEffectList.Contains(checkId))
                                {
                                    canTransformCount ++;
                                }
                            }
                            
                            if (canTransformCount == 0)
                            {
                                continue;
                            }
                        }
                        oncePerTurnEffectActivatedList.Add(Card.Id);
                        Logger.DebugWriteLine("Exosiseter Vadis decide: " + checkId);
                        AI.SelectCard(checkId);
                        AI.SelectNextCard(checkTarget);
                        return true;
                    }
                }
            }

            return false;
        }

        public bool ExosisterReturniaActivate()
        {
            // banish problem card
            ClientCard target = GetProblematicEnemyCard(true);
            if (target != null && Duel.LastChainPlayer != 0)
            {
                Logger.DebugWriteLine("===Exosister: returnia target 1: " + target?.Name);
                removeChosenList.Add(target);
                oncePerTurnEffectActivatedList.Add(Card.Id);
                AI.SelectCard(target);
                return true;
            }

            // banish target
            if (Duel.LastChainPlayer == 1)
            {
                List<ClientCard> targetList = Duel.LastChainTargets.Where(card => card.Controller == 1 &&
                    (card.Location == CardLocation.Grave || card.Location == CardLocation.MonsterZone || card.Location == CardLocation.SpellZone || card.Location == CardLocation.FieldZone)).ToList();
                if (targetList.Count() > 0)
                {
                    oncePerTurnEffectActivatedList.Add(Card.Id);
                    List<ClientCard> shuffleTargetList = ShuffleCardList(targetList);
                    Logger.DebugWriteLine("===Exosister: returnia target 2: " + shuffleTargetList[0]?.Name);
                    AI.SelectCard(shuffleTargetList);
                    return true;
                }
            }

            // dump banish
            target = GetBestEnemyCard(false, true, true);
            bool check1 = DefaultOnBecomeTarget() && target.Id != _CardId.EvenlyMatched;
            bool check2 = Bot.UnderAttack;
            bool check3 = (Duel.Player == 1 && Duel.Phase == DuelPhase.End && Duel.LastChainPlayer != 0 && target != null && target.Location != CardLocation.Grave);
            bool check4 = (Duel.Player == 1 && Enemy.GetMonsterCount() >= 2 && Duel.LastChainPlayer != 0);
            Logger.DebugWriteLine("===Exosister: returnia check flag: " + check1 + " " + check2 + " " + check3 + " " + check4);
            if (check1 || check2 || check3 || check4)
            {
                oncePerTurnEffectActivatedList.Add(Card.Id);
                Logger.DebugWriteLine("===Exosister: returnia target 3: " + target?.Name);
                AI.SelectCard(target);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check hand like exosister + elis + martha
        /// </summary>
        public bool ExosisterAvoidMaxxCSummonCheck()
        {
            if (!Bot.HasInHand(CardId.ExosisterMartha) || !Bot.HasInHand(CardId.ExosisterElis) || elisEffect1Activated || marthaEffect1Activated)
            {
                return false;
            }
            if (enemyActivateLockBird && CheckAtAdvantage())
            {
                return false;
            }
            // normal summon non-elis exosister
            if (Card.Id != CardId.ExosisterElis && Card.Id != CardId.ExosisterMartha)
            {
                summoned = true;
                return true;
            }
            // normal summon elis
            if (Card.IsCode(CardId.ExosisterElis))
            {
                int otherExosisterCount = Bot.Hand.Count(card => card?.Data != null && !card.IsCode(CardId.ExosisterElis) && !card.IsCode(CardId.ExosisterMartha)
                    && card.IsMonster() && card.HasSetcode(SetcodeExosister));
                if (otherExosisterCount > 0)
                {
                    return false;
                }
                if (Bot.Hand.Count(card => card?.Data != null && card.IsCode(CardId.ExosisterElis)) > 1)
                {
                    summoned = true;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check hand like exosister + stella
        /// </summary>
        public bool ExosisterStellaSummonCheck()
        {
            if (stellaEffect1Activated || Bot.HasInMonstersZone(CardId.ExosisterStella, true) || CheckWhetherNegated(true) || CheckLessOperation())
            {
                return false;
            }
            if (enemyActivateLockBird && CheckAtAdvantage())
            {
                return false;
            }

            int summonableCount = Bot.Hand.Count(card => card != Card && card?.Data != null && card.IsMonster()
                && card.HasSetcode(SetcodeExosister));
            
            if (summonableCount > 0)
            {
                summoned = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check whether need Irene's redraw effect to search elis for xyz
        /// </summary>
        public bool ExosisterIreneSummonCheck()
        {
            if (irenaEffect1Activated || CheckLessOperation()
                || CheckWhetherNegated(true) || CheckCalledbytheGrave(CardId.ExosisterElis) > 0 || CheckCalledbytheGrave(CardId.ExosisterIrene) > 0)
            {
                return false;
            }
            if (enemyActivateLockBird && CheckAtAdvantage())
            {
                return false;
            }

            if (CheckRemainInDeck(CardId.ExosisterElis) > 0)
            {
                summoned = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check hand like exosister + elis
        /// </summary>
        public bool ExosisterForElisSummonCheck()
        {
            if (elisEffect1Activated || CheckCalledbytheGrave(CardId.ExosisterElis) > 0 || CheckLessOperation())
            {
                return false;
            }
            if (Card?.Data == null)
            {
                return false;
            }
            if (!Card.HasSetcode(SetcodeExosister) || (Card.IsCode(CardId.ExosisterMartha) && CheckRemainInDeck(CardId.ExosisterElis) > 0))
            {
                return false;
            }
            if (enemyActivateLockBird && CheckAtAdvantage())
            {
                return false;
            }

            if (Bot.Hand.Count(card => card != Card && card?.Data != null && card.IsCode(CardId.ExosisterElis)) > 0)
            {
                summoned = true;
                return true;
            }

            return false;
        }

        public bool AratamaSummonCheck()
        {
            if (sakitamaEffect1Activated || CheckCalledbytheGrave(CardId.Aratama) > 0 || CheckCalledbytheGrave(CardId.Sakitama) > 0)
            {
                return false;
            }
            if (enemyActivateLockBird && CheckAtAdvantage())
            {
                return false;
            }
            if (CheckRemainInDeck(CardId.Sakitama) > 0)
            {
                summoned = true;
                return true;
            }
            return false;
        }

        public bool ForSakitamaSummonCheck()
        {
            if (sakitamaEffect1Activated || CheckCalledbytheGrave(CardId.Sakitama) > 0 || CheckLessOperation())
            {
                return false;
            }
            if (Bot.Hand.Count(card => card?.Data != null && Card != card && card.IsCode(CardId.Sakitama)) == 0)
            {
                return false;
            }
            if (enemyActivateLockBird && CheckAtAdvantage())
            {
                return false;
            }
            if (Card?.Data != null && !Card.IsCode(CardId.ExosisterMartha) && Card.Level == 4)
            {
                summoned = true;
                return true;
            }

            return false;
        }
    
        public bool Level4SummonCheck()
        {
            if (Card.Id == CardId.ExosisterMartha)
            {
                return false;
            }
            if (Bot.GetMonsters().Count(card => CheckAbleForXyz(card)) == 1)
            {
                summoned = true;
                return true;
            }
            return false;
        }

        public bool ForDonnerSummonCheck()
        {
            if (!Bot.HasInExtra(CardId.DonnerDaggerFurHire) || (!Bot.HasInHand(CardId.ExosisterMartha) && !Bot.HasInHandOrInSpellZone(CardId.ExosisterReturnia)))
            {
                return false;
            }
            if (CheckLessOperation())
            {
                return false;
            }

            List<ClientCard> illegalList = Bot.GetMonsters().Where(card => card.IsFaceup() && !card.HasType(CardType.Xyz) && card.Level != 4
                && (card.Data == null || !card.HasSetcode(SetcodeExosister))).ToList();
            if (illegalList.Count() == 0)
            {
                return false;
            }
            
            if (illegalList.Count() == 1)
            {
                List<ClientCard> otherMaterialList = Bot.GetMonsters().Where(card => card.IsFaceup() && !card.HasType(CardType.Xyz)).ToList();
                otherMaterialList.Sort(CardContainer.CompareCardAttack);
                illegalList.AddRange(otherMaterialList);
            }
            if (illegalList.Count() == 1)
            {
                List<ClientCard> hands = Bot.Hand.Where(card => card?.Data != null && card.IsMonster()).ToList();
                if (hands.Count() > 0)
                {
                    hands.Sort(CardContainer.CompareCardAttack);
                    if (Card != hands[0])
                    {
                        return false;
                    }
                }
                Logger.DebugWriteLine("===Exosister: summon for donner");
                summoned = true;
                return true;
            }

            return false;
        }

        public bool ExosisterForArmentSummonCheck()
        {
            if (!Bot.HasInHandOrInSpellZone(CardId.ExosisterArment))
            {
                return false;
            }
            if (Card?.Data == null)
            {
                return false;
            }
            if (!Card.HasSetcode(SetcodeExosister))
            {
                return false;
            }

            if (!Bot.GetMonsters().Any(card => card?.Data != null && card.IsFaceup() && card.HasSetcode(SetcodeExosister)))
            {
                summoned = true;
                return true;
            }

            return false;
        }

        public bool ExosisterMikailisSpSummonCheck()
        {
            return ExosisterMikailisSpSummonCheckInner(true);
        }

        public bool ExosisterMikailisAdvancedSpSummonCheck()
        {
            if (!CheckLessOperation() || enemyActivateLockBird)
            {
                return false;
            }

            return ExosisterMikailisSpSummonCheckInner(false);
        }

        public bool ExosisterMikailisSpSummonCheckInner(bool shouldCheckLessOperation = true)
        {
            if (Bot.HasInMonstersZone(CardId.ExosisterMikailis) || mikailisEffect3Activated || (CheckLessOperation() && shouldCheckLessOperation))
            {
                return false;
            }

            // check searched spell/trap
            if (!enemyActivateLockBird)
            {
                foreach (int cardId in ExosisterSpellTrapList)
                {
                    if (!Bot.HasInHandOrInSpellZone(cardId))
                    {
                        SelectXyzMaterial(2);
                        return true;
                    }
                }
            }

            // clear enemy card
            if (!mikailisEffect1Activated && !Bot.HasInMonstersZone(CardId.ExosisterMikailis))
            {
                ClientCard target = GetProblematicEnemyCard(true);
                if (target != null)
                {
                    List<ClientCard> exosisterMaterialList = Bot.GetMonsters().Where(card => CheckAbleForXyz(card) && card.HasSetcode(SetcodeExosister)).ToList();
                    if (exosisterMaterialList?.Count() > 0)
                    {
                        SelectXyzMaterial(2, true);
                        return true;
                    }
                }
            }

            return false;
        }

        public bool ExosisterKaspitellSpSummonCheck()
        {
            return ExosisterKaspitellSpSummonCheckInner(true);
        }

        public bool ExosisterKaspitellAdvancedSpSummonCheck()
        {
            if (!CheckLessOperation() || enemyActivateLockBird)
            {
                return false;
            }
            return ExosisterKaspitellSpSummonCheckInner(false);
        }

        public bool ExosisterKaspitellSpSummonCheckInner(bool shouldCheckLessOperation = true)
        {
            if (Bot.HasInMonstersZone(CardId.ExosisterKaspitell) || kaspitellEffect3Activated || (shouldCheckLessOperation && CheckLessOperation()))
            {
                return false;
            }

            bool searchMartha = true;
            bool searchStella = true;
            bool forMagnifica = false;
            if (marthaEffect1Activated || CheckCalledbytheGrave(CardId.ExosisterMartha) > 0
                || CheckRemainInDeck(CardId.ExosisterMartha) == 0 || CheckRemainInDeck(CardId.ExosisterElis) == 0)
            {
                searchMartha = false;
            }
            if (Bot.GetMonsters().Any(card => card.HasType(CardType.Link) || card.HasType(CardType.Token)))
            {
                searchMartha = false;
            }
            if (stellaEffect1Activated || CheckCalledbytheGrave(CardId.ExosisterStella) > 0 || CheckRemainInDeck(CardId.ExosisterStella) == 0
                || !Bot.Hand.Any(card => card?.Data != null && card.IsMonster() && card.HasSetcode(SetcodeExosister)))
            {
                searchStella = false;
            }
            if (Bot.GetMonsters().Count(card => card?.Data != null
                && card.HasType(CardType.Xyz) && card.HasType(CardType.Xyz) && !card.IsCode(CardId.ExosistersMagnifica)) == 1)
            {
                forMagnifica = true;
            }
            if (enemyActivateLockBird)
            {
                searchMartha = false;
                searchStella = false;
            }

            if (!searchMartha && !searchStella && !forMagnifica)
            {
                return false;
            }

            List<ClientCard> materialCheckList = Bot.GetMonsters().Where(card =>
                !card.HasType(CardType.Xyz) && !card.HasType(CardType.Token) && !card.HasType(CardType.Link)).ToList();
            if (materialCheckList.Count() == 2 && materialCheckList.All(card => card.Level == 4))
            {
                SelectXyzMaterial(2);
                return true;
            }

            return false;
        }

        public bool ExosistersMagnificaSpSummonCheck()
        {
            if (CheckLessOperation())
            {
                return false;
            }

            List<ClientCard> materialList = Bot.GetMonsters().Where(card => card.IsFaceup() && card.HasType(CardType.Xyz)
                && card.Rank == 4 && card.HasSetcode(SetcodeExosister)).ToList();
            materialList.Sort(CardContainer.CompareCardAttack);

            AI.SelectMaterials(materialList);
            return true;
        }

        public bool CheckCaduceusInner(ClientCard card)
        {
            if (card?.Data == null)
            {
                return false;
            }
            foreach (int setcode in SetcodeForDiamond)
            {
                if (card.HasSetcode(setcode))
                {
                    return true;
                }
            }
            return false;
        }

        public bool TellarknightConstellarCaduceusSpSummonCheck()
        {
            if (Duel.Turn == 1 || !Bot.HasInExtra(CardId.StellarknightConstellarDiamond))
            {
                return false;
            }

            // check whether need to call Diamond
            if (Enemy.Graveyard.Any(card => CheckCaduceusInner(card)))
            {
                SelectXyzMaterial(2);
                return true;
            }

            return false;
        }

        public bool DonnerDaggerFurHireSpSummonCheck()
        {
            if (!Bot.HasInHand(CardId.ExosisterMartha) && !Bot.HasInHandOrInSpellZone(CardId.ExosisterReturnia))
            {
                return false;
            }

            if (CheckLessOperation())
            {
                return false;
            }

            List<ClientCard> illegalList = Bot.GetMonsters().Where(card => card.IsFaceup() && !card.HasType(CardType.Xyz) && card.Level != 4
                && (card.Data == null || !card.HasSetcode(SetcodeExosister))).ToList();
            
            if (illegalList.Count() == 1)
            {
                
                List<ClientCard> otherMaterialList = Bot.GetMonsters().Where(card => card.IsFaceup() && !card.HasType(CardType.Xyz)).ToList();
                otherMaterialList.Sort(CardContainer.CompareCardAttack);
                illegalList.AddRange(otherMaterialList);
            }
            if (illegalList.Count() > 1)
            {
                AI.SelectMaterials(illegalList);
                return true;
            }

            return false;
        }

        public bool SpellSetCheck()
        {
            if (Duel.Phase == DuelPhase.Main1 && Bot.HasAttackingMonster() && Duel.Turn > 1) return false;
            List<int> onlyOneSetList = new List<int>{
                CardId.ExosisterPax, CardId.ExosisterArment, CardId.ExosisterVadis, CardId.ExosisterReturnia
            };
            if (onlyOneSetList.Contains(Card.Id) && Bot.HasInSpellZone(Card.Id))
            {
                return false;
            }

            // select place
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
                    } else
                    {
                        SelectSTPlace(Card, false, avoid_list);
                        return true;
                    }
                } else
                {
                    SelectSTPlace();
                }
                return true;
            }

            return false;
        }
    }
}