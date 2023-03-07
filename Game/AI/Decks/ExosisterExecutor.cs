using YGOSharp.OCGWrapper;
using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using System.Linq;


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
            public const int PotofExtravagance = 4408198;
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
            // _CardId.EvilswarmExcitonKnight = 46772449;

            public const int NaturalExterio = 99916754;
            public const int NaturalBeast = 33198837;
            public const int ImperialOrder = 61740673;
            public const int SwordsmanLV7 = 37267041;
            public const int RoyalDecree = 51452091;
            public const int Number41BagooskatheTerriblyTiredTapir = 90590303;
            public const int InspectorBoarder = 15397015;
        }

        public ExosisterExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // counter
            AddExecutor(ExecutorType.Activate, CardId.StellarknightConstellarDiamond);
            AddExecutor(ExecutorType.Activate, _CardId.AshBlossom, AshBlossomActivate);
            AddExecutor(ExecutorType.Activate, CardId.ExosistersMagnifica, ExosistersMagnificaActivateTrigger);

            AddExecutor(ExecutorType.Activate, _CardId.InfiniteImpermanence, InfiniteImpermanenceActivate);
            AddExecutor(ExecutorType.Activate, _CardId.CalledByTheGrave, CalledbytheGraveActivate);

            // quick effect
            AddExecutor(ExecutorType.Activate, DefaultExosisterTransform);
            AddExecutor(ExecutorType.Activate, CardId.ExosistersMagnifica, ExosistersMagnificaActivateBanish);
            AddExecutor(ExecutorType.Activate, CardId.ExosisterArment,   ExosisterArmentActivate);
            AddExecutor(ExecutorType.Activate, CardId.ExosisterVadis,    ExosisterVadisActivate);
            AddExecutor(ExecutorType.Activate, CardId.ExosisterReturnia, ExosisterReturniaActivate);

            // free chain
            AddExecutor(ExecutorType.Activate, _CardId.MaxxC, MaxxCActivate);

            // field effect
            AddExecutor(ExecutorType.Activate, CardId.Aratama);
            AddExecutor(ExecutorType.Activate, CardId.ExosisterMikailis,  ExosisterMikailisActivate);
            AddExecutor(ExecutorType.Activate, CardId.ExosisterKaspitell, ExosisterKaspitellActivate);
            AddExecutor(ExecutorType.Activate, CardId.ExosisterGibrine,   ExosisterGibrineActivate);
            AddExecutor(ExecutorType.Activate, CardId.ExosisterAsophiel,  ExosisterAsophielActivate);

            AddExecutor(ExecutorType.Activate, CardId.ExosisterSophia, ExosisterSophiaActivate);
            AddExecutor(ExecutorType.Activate, CardId.ExosisterIrene,  ExosisterIreneActivate);
            AddExecutor(ExecutorType.Activate, CardId.ExosisterStella, ExosisterStellaActivate);

            // addition monster summmon
            AddExecutor(ExecutorType.Activate, CardId.ExosisterElis, ExosisterElisActivate);
            AddExecutor(ExecutorType.Activate, CardId.Sakitama, SakitamaActivate);
            AddExecutor(ExecutorType.Activate, CardId.ExosisterPax,  ExosisterPaxActivate);

            // xyz summon
            AddExecutor(ExecutorType.SpSummon, CardId.StellarknightConstellarDiamond);
            AddExecutor(ExecutorType.SpSummon, CardId.ExosisterMikailis, ExosisterMikailisAdvancedSpSummonCheck);
            AddExecutor(ExecutorType.SpSummon, CardId.ExosisterKaspitell, ExosisterKaspitellAdvancedSpSummonCheck);

            AddExecutor(ExecutorType.SpSummon, CardId.ExosisterKaspitell, ExosisterKaspitellSpSummonCheck);
            AddExecutor(ExecutorType.SpSummon, CardId.ExosisterMikailis, ExosisterMikailisSpSummonCheck);
            AddExecutor(ExecutorType.SpSummon, CardId.TellarknightConstellarCaduceus, TellarknightConstellarCaduceusSpSummonCheck);

            AddExecutor(ExecutorType.SpSummon, CardId.ExosistersMagnifica, ExosistersMagnificaSpSummonCheck);

            // normal summon for xyz(avoiding MaxxC)
            AddExecutor(ExecutorType.Summon, CardId.ExosisterSophia, ExosisterAvoidMaxxCSummonCheck);
            AddExecutor(ExecutorType.Summon, CardId.ExosisterIrene,  ExosisterAvoidMaxxCSummonCheck);
            AddExecutor(ExecutorType.Summon, CardId.ExosisterStella, ExosisterAvoidMaxxCSummonCheck);
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
        List<int> transformDestList = new List<int>();

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

        public ClientCard GetProblematicEnemyCard(bool canBeTarget = false)
        {
            ClientCard card = Enemy.MonsterZone.GetFloodgate(canBeTarget);
            if (card != null)
                return card;

            card = Enemy.MonsterZone.GetDangerousMonster(canBeTarget);
            if (card != null
                && (Duel.Player == 0 || (Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2)))
                return card;

            card = Enemy.MonsterZone.GetInvincibleMonster(canBeTarget);
            if (card != null)
                return card;
            List<ClientCard> enemyMonsters = Enemy.GetMonsters();
            enemyMonsters.Sort(CardContainer.CompareCardAttack);
            enemyMonsters.Reverse();
            foreach(ClientCard target in enemyMonsters)
            {
                if (target.HasType(CardType.Fusion) || target.HasType(CardType.Ritual) || target.HasType(CardType.Synchro) || target.HasType(CardType.Xyz) || (target.HasType(CardType.Link) && target.LinkCount >= 2) )
                {
                    if (!canBeTarget || !(target.IsShouldNotBeTarget() || target.IsShouldNotBeMonsterTarget())) return target;
                }
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
        /// Check whether negate MaxxC
        /// </summary>
        public void CheckDeactiveFlag()
        {
            if (Util.GetLastChainCard() != null && Util.GetLastChainCard().IsCode(_CardId.MaxxC)  && Duel.LastChainPlayer == 1)
            {
                enemyActivateMaxxC = false;
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
            if (Util.GetProblematicEnemyMonster() == null && Bot.GetMonsters().Any(card => card.IsFaceup()))
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
            return !marthaEffect1Activated && CheckCalledbytheGrave(CardId.ExosisterMartha) == 0
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
        /// <param name="avoid_list">Whether need to avoid set in this place</param>
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
            List<ClientCard> selectedList = new List<ClientCard>();
            
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

            if (player == 1 && card.IsCode(_CardId.MaxxC) && CheckCalledbytheGrave(_CardId.MaxxC) == 0)
            {
                enemyActivateMaxxC = true;
            }
            if (player == 1 && card.IsCode(_CardId.InfiniteImpermanence))
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
            if (player == 1 && card.IsCode(_CardId.CalledByTheGrave))
            {
                foreach (ClientCard targetCard in Duel.LastChainTargets) {
                    calledbytheGraveCount[targetCard.Id] = 2;
                }
            }
            if (player == 1)
            {
                if (Duel.LastChainLocation == CardLocation.Grave && card.Location == CardLocation.Grave)
                {
                    Logger.WriteLine("Exosister: enemy activate effect from GY.");
                    enemyMoveGrave = true;
                } else 
                {
                    foreach (ClientCard targetCard in Duel.LastChainTargets) {
                        if (targetCard.Location == CardLocation.Grave)
                        {
                            Logger.WriteLine("Exosister: enemy target cards of GY.");
                            enemyMoveGrave = true;
                            break;
                        }
                    }
                }
            }
            base.OnChaining(player, card);
        }

        /// <summary>
        /// clear chain information
        /// </summary>
        public override void OnChainEnd()
        {
            enemyMoveGrave = false;
            paxCallToField = false;
            transformDestList.Clear();
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
            base.OnChainEnd();
        }

        public override void OnNewTurn()
        {
            enemyActivateMaxxC = false;
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
        }

        /// <summary>
        /// override for exosister's transform
        /// </summary>
        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, int hint, bool cancelable)
        {
            bool beginTransformCheck = false;
            if (hint == HintMsg.SpSummon && min == 1 && max == 1 && transformDestList.Count() > 0)
            {
                // check whether for transform
                if (cards.All(card => card.Location == CardLocation.Extra && card.Rank == 4 && card.HasSetcode(SetcodeExosister)))
                {
                    beginTransformCheck = true;
                }
            }
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
                        return Util.CheckSelectCount(result, cards, min, max);
                    }
                }
            }
            return base.OnSelectCard(cards, min, max, hint, cancelable);
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
                // TODO
            }

            return base.OnSelectOption(options);
        }

        public bool AshBlossomActivate()
        {
            if (CheckWhetherNegated(true) || !CheckLastChainShouldNegated()) return false;
            CheckDeactiveFlag();
            return DefaultAshBlossomAndJoyousSpring();
        }

        public bool MaxxCActivate()
        {
            if (CheckWhetherNegated(true)) return false;
            return DefaultMaxxC();
        }
    
        public bool InfiniteImpermanenceActivate()
        {
            if (!SpellNegatable()) return false;
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
                    List<ClientCard> enemy_monsters = Enemy.GetMonsters();
                    enemy_monsters.Sort(CardContainer.CompareCardAttack);
                    enemy_monsters.Reverse();
                    foreach(ClientCard card in enemy_monsters)
                    {
                        if (card.IsFaceup() && !card.IsShouldNotBeTarget() && !card.IsShouldNotBeSpellTrapTarget())
                        {
                            AI.SelectCard(card);
                            infiniteImpermanenceList.Add(this_seq);
                            return true;
                        }
                    }
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
                List<ClientCard> enemy_monsters = Enemy.GetMonsters();
                enemy_monsters.Sort(CardContainer.CompareCardAttack);
                enemy_monsters.Reverse();
                foreach (ClientCard card in enemy_monsters)
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
                    List<ClientCard> enemy_monsters = Enemy.Graveyard.GetMatchingCards(card => card.IsMonster()).ToList();
                    if (enemy_monsters.Count > 0)
                    {
                        enemy_monsters.Sort(CardContainer.CompareCardAttack);
                        enemy_monsters.Reverse();
                        int code = enemy_monsters[0].Id;
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

        public bool SakitamaActivate()
        {
            // summon
            if (Card.Location == CardLocation.Hand)
            {
                if (Bot.GetMonsters().Count(card => CheckAbleForXyz(card)) == 1)
                {
                    AI.SelectCard(CardId.Aratama, CardId.Sakitama);
                    sakitamaEffect1Activated = true;
                    return true;
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
            if (ActivateDescription != Util.GetStringId(CardId.ExosisterStella, 0) || CheckWhetherNegated(true))
            {
                return false;
            }

            bool ableToXyz = Bot.GetMonsters().Count(card => CheckAbleForXyz(card)) >= 2;

            if (CheckLessOperation() && ableToXyz) 
            {
                return false;
            }
            if (Bot.HasInHand(CardId.ExosisterMartha) && ableToXyz && Bot.Hand.Count(card => card.IsCode(CardId.ExosisterMartha)) == 1)
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
                if (oncePerTurnEffectActivatedList.Contains(cardId) || Bot.Hand.Count(card => card.IsCode(cardId)) > 1)
                {
                    shuffleList.Add(cardId);
                }
            }
            
            if (shuffleList.Count() > 0)
            {
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
            List<int> checkTransformCode = new List<int>{
                Util.GetStringId(CardId.ExosisterElis, 1),
                Util.GetStringId(CardId.ExosisterStella, 1),
                Util.GetStringId(CardId.ExosisterIrene, 1),
                Util.GetStringId(CardId.ExosisterSophia, 1),
                Util.GetStringId(CardId.ExosisterMartha, 1)
            };
            if (!checkTransformCode.Contains(ActivateDescription))
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
                if (Duel.Player == 0 && !mikailisEffect3Activated && Duel.Phase < DuelPhase.End)
                {
                    return false;
                }

                // banish problem card
                ClientCard target = GetProblematicEnemyCard(true);
                if (target != null && !Util.ChainContainPlayer(0))
                {
                    mikailisEffect1Activated = true;
                    AI.SelectCard(target);
                    return true;
                }

                // banish GY target
                if (Duel.LastChainPlayer == 1)
                {
                    List<ClientCard> graveTarget = Duel.LastChainTargets.Where(card => card.Controller == 1 && card.Location == CardLocation.Grave).ToList();
                    if (graveTarget.Count() > 0)
                    {
                        mikailisEffect1Activated = true;
                        AI.SelectCard(ShuffleCardList(graveTarget));
                        return true;
                    }
                }

                // dump banish
                target = Util.GetBestEnemyCard(false, true);
                if (DefaultOnBecomeTarget() || Bot.UnderAttack || Duel.Phase == DuelPhase.End
                    || (Duel.Player == 0 && Bot.GetMonsters().Count(card => card.HasType(CardType.Xyz) && card.Rank == 4 && card.HasSetcode(SetcodeExosister)) == 2)
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
            if (ActivateDescription == Util.GetStringId(CardId.ExosisterKaspitell, 0))
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
            if (CheckMarthaActivatable() && CheckRemainInDeck(CardId.ExosisterMartha) > 0 && CheckRemainInDeck(CardId.ExosisterElis) > 0 && !Bot.HasInHand(CardId.ExosisterMartha))
            {
                kaspitellEffect3Activated = true;
                SelectDetachMaterial(Card);
                AI.SelectNextCard(CardId.ExosisterMartha);
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
            // search sophia for draw
            if (!summoned && Bot.GetMonsters().Count(card => CheckAbleForXyz(card)) == 1 && !sophiaEffect1Activated && CheckCalledbytheGrave(CardId.ExosisterSophia) == 0
                && !Bot.HasInHand(CardId.ExosisterSophia))
            {
                kaspitellEffect3Activated = true;
                SelectDetachMaterial(Card);
                AI.SelectNextCard(CardId.ExosisterSophia);
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
            if (Bot.GetMonsters().Count(card => CheckAbleForXyz(card)) >= 2)
            {
                // wait for xyz summon
                return false;
            }
            gibrineEffect3Activated = true;
            SelectDetachMaterial(Card);
            return true;
        }

        public bool ExosisterAsophielActivate()
        {
            // block activate from GY
            if (ActivateDescription == Util.GetStringId(CardId.ExosisterAsophiel, 0))
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
                if (DefaultOnBecomeTarget())
                {
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

                // dump banish
                if (target == null)
                {
                    target = Util.GetBestEnemyCard(false, true);
                    if (!DefaultOnBecomeTarget() && !Bot.UnderAttack)
                    {
                        target = null;
                    }
                }

                if (target != null && Duel.LastChainPlayer != 0)
                {
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
            List<int> checkListForSpSummon = new List<int>{
                CardId.ExosisterSophia, CardId.ExosisterIrene, CardId.ExosisterStella, CardId.ExosisterMartha, CardId.ExosisterElis
            };
            List<int> checkListForSearch = new List<int>{
                CardId.ExosisterMartha, CardId.ExosisterStella, CardId.ExosisterSophia, CardId.ExosisterIrene, CardId.ExosisterElis
            };
            if (Duel.Player == 0)
            {
                // search martha for activate
                if (CheckMarthaActivatable() && CheckRemainInDeck(CardId.ExosisterMartha) > 0 && CheckRemainInDeck(CardId.ExosisterElis) > 0 && !Bot.HasInHand(CardId.ExosisterMartha))
                {
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
                            oncePerTurnEffectActivatedList.Add(Card.Id);
                            AI.SelectCard(CardId.ExosisterStella);
                            paxCallToField = shouldSpSummon;
                            return true;
                        }
                    }

                    // search monster for stella to summon
                    if (Bot.HasInHandOrHasInMonstersZone(CardId.ExosisterStella) &&
                        Bot.Hand.Count(card => card?.Data != null && card.IsMonster() && card.HasSetcode(SetcodeExosister)) == 0)
                    {
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
                                oncePerTurnEffectActivatedList.Add(Card.Id);
                                AI.SelectCard(checkId);
                                paxCallToField = true;
                                return true;
                            }
                        }
                    }
                }
            }

            bool inDanger = CheckInDanger();

            // trigger transform
            // become target
            if (enemyMoveGrave || DefaultOnBecomeTarget() || inDanger)
            {
                List<int> checkList = checkListForSpSummon;
                bool shouldSpSummon = enemyMoveGrave || inDanger;
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
                    int checkTarget = CheckExosisterMentionCard(checkId);
                    if (checkTarget > 0 && Bot.HasInMonstersZoneOrInGraveyard(checkId) && CheckRemainInDeck(checkTarget) > 0
                        && !exosisterTransformEffectList.Contains(checkId) && !Bot.HasInMonstersZone(checkId))
                    {
                        oncePerTurnEffectActivatedList.Add(Card.Id);
                        AI.SelectCard(checkId);
                        paxCallToField = shouldSpSummon;
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
                    targetList.Sort();
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
            // TODO
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
            if (enemyMoveGrave)
            {
                decideToActivate = true;
                checkTransform = true;
            }

            if (CheckInDanger() || DefaultOnBecomeTarget())
            {
                decideToActivate = true;
            }

            if (decideToActivate)
            {
                foreach (int checkId in checkListForSpSummon)
                {
                    int checkTarget = CheckExosisterMentionCard(checkId);
                    if (checkTarget > 0 && Bot.HasInMonstersZoneOrInGraveyard(checkId) && CheckRemainInDeck(checkTarget) > 0)
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
            if (target != null && !Util.ChainContainPlayer(0))
            {
                oncePerTurnEffectActivatedList.Add(Card.Id);
                AI.SelectCard(target);
                return true;
            }

            // banish GY target
            if (Duel.LastChainPlayer == 1)
            {
                List<ClientCard> graveTarget = Duel.LastChainTargets.Where(card => card.Controller == 1 && card.Location == CardLocation.Grave).ToList();
                if (graveTarget.Count() > 0)
                {
                    oncePerTurnEffectActivatedList.Add(Card.Id);
                    AI.SelectCard(ShuffleCardList(graveTarget));
                    return true;
                }
            }

            // dump banish
            target = Util.GetBestEnemyCard(false, true);
            if (DefaultOnBecomeTarget() || Bot.UnderAttack || Duel.Phase == DuelPhase.End
                || (Duel.Player == 1 && Enemy.GetMonsterCount() >= 2))
            {
                oncePerTurnEffectActivatedList.Add(Card.Id);
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
            if (stellaEffect1Activated || Bot.HasInMonstersZone(CardId.ExosisterStella, true) || CheckWhetherNegated(true))
            {
                return false;
            }

            int summonableCount = Bot.Hand.Count(card => card != Card && card?.Data != null && card.IsMonster()
                && !card.IsCode(CardId.ExosisterMartha) && card.HasSetcode(SetcodeExosister));
            
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
            if (irenaEffect1Activated || CheckWhetherNegated(true) || CheckCalledbytheGrave(CardId.ExosisterElis) > 0 || CheckCalledbytheGrave(CardId.ExosisterIrene) > 0)
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
            if (elisEffect1Activated || CheckCalledbytheGrave(CardId.ExosisterElis) > 0)
            {
                return false;
            }
            if (Card?.Data == null)
            {
                return false;
            }
            if (!Card.HasSetcode(SetcodeExosister) || Card.IsCode(CardId.ExosisterMartha))
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
            if (CheckRemainInDeck(CardId.Sakitama) > 0)
            {
                summoned = true;
                return true;
            }
            return false;
        }

        public bool ForSakitamaSummonCheck()
        {
            if (sakitamaEffect1Activated || CheckCalledbytheGrave(CardId.Sakitama) > 0)
            {
                return false;
            }
            if (Bot.Hand.Count(card => card?.Data != null && Card != card && card.IsCode(CardId.Sakitama)) == 0)
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

        public bool ExosisterMikailisSpSummonCheck()
        {
            if (Bot.HasInMonstersZone(CardId.ExosisterMikailis) || mikailisEffect3Activated)
            {
                return false;
            }

            // check searched spell/trap
            foreach (int cardId in ExosisterSpellTrapList)
            {
                if (!Bot.HasInHandOrInSpellZone(cardId))
                {
                    SelectXyzMaterial(2);
                    return true;
                }
            }

            return false;
        }

        public bool ExosisterMikailisAdvancedSpSummonCheck()
        {
            if (!CheckLessOperation())
            {
                return false;
            }

            return ExosisterMikailisSpSummonCheck();
        }

        public bool ExosisterKaspitellSpSummonCheck()
        {
            if (Bot.HasInMonstersZone(CardId.ExosisterKaspitell) || kaspitellEffect3Activated)
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

        public bool ExosisterKaspitellAdvancedSpSummonCheck()
        {
            if (!CheckLessOperation())
            {
                return false;
            }
            return ExosisterKaspitellSpSummonCheck();
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

        public bool SpellSetCheck()
        {
            if (Duel.Phase == DuelPhase.Main1 && Bot.HasAttackingMonster() && Duel.Turn > 1) return false;
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