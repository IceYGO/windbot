using YGOSharp.OCGWrapper;
using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using System.Linq;


namespace WindBot.Game.AI.Decks
{
    [Deck("Witchcraft", "AI_Witchcraft")]

    class WitchcraftExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int PSYDriver = 49036338;
            public const int GolemAruru = 71074418;
            public const int MadameVerre = 21522601;
            public const int Haine = 84523092;
            public const int Schmietta = 21744288;
            public const int Pittore = 95245544;
            public const int PSYGamma = 38814750;
            public const int Potterie = 59851535;
            public const int Genni = 64756282;
            public const int Collaboration = 10805153;
            public const int ThatGrassLooksGreener = 11110587;
            public const int PotofExtravagance = 49238328;
            public const int DarkRulerNoMore = 54693926;
            public const int Creation = 57916305;
            public const int Reasoning = 58577036;
            public const int MetalfoesFusion = 73594093;
            public const int Holiday = 83301414;
            public const int Draping = 56894757;
            public const int Unveiling = 70226289;
            public const int MagiciansLeftHand = 13758665;
            public const int Scroll = 19673561;
            public const int MagiciansRestage = 40252269;
            public const int WitchcrafterBystreet = 83289866;
            public const int MagicianRightHand = 87769556;
            public const int Masterpiece = 55072170;
            public const int Patronus = 94553671;
            public const int BorreloadSavageDragon = 27548199;
            public const int DracoBerserkeroftheTenyi = 5041348;
            public const int PSYOmega = 74586817;
            public const int TGWonderMagician = 98558751;
            public const int BorrelswordDragon = 85289965;
            public const int KnightmareUnicorn = 38342335;
            public const int KnightmarePhoenix = 2857636;
            public const int PSYLambda = 8802510;
            public const int CrystronHalqifibrax = 50588353;
            public const int SalamangreatAlmiraj = 60303245;
            public const int RelinquishedAnima = 94259633;

            public const int NaturalExterio = 99916754;
            public const int SwordsmanLV7 = 37267041;
            public const int Anti_Spell = 58921041;
            public const int Numbe41BagooskatheTerriblyTiredTapir = 90590303;
            public const int PerformapalFive_RainbowMagician = 19619755;

            public const int DimensionShifter = 91800273;
            public const int MacroCosmos = 30241314;
            public const int DimensionalFissure = 81674782;
            public const int BanisheroftheRadiance = 94853057;
            public const int BanisheroftheLight = 61528025;
        }

        public WitchcraftExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // do first
            AddExecutor(ExecutorType.Activate, CardId.PotofExtravagance, PotofExtravaganceActivate);
            AddExecutor(ExecutorType.SpellSet, SpellSetForFiveRainbow);

            // clear
            AddExecutor(ExecutorType.Activate, CardId.DarkRulerNoMore, DarkRulerNoMoreActivate);
            AddExecutor(ExecutorType.Activate, _CardId.LightningStorm, LightningStormActivate);
            AddExecutor(ExecutorType.Activate, CardId.RelinquishedAnima);

            // counter & quick effect
            AddExecutor(ExecutorType.Activate, CardId.Schmietta, DeckSSWitchcraft);
            AddExecutor(ExecutorType.Activate, CardId.Pittore, DeckSSWitchcraft);
            AddExecutor(ExecutorType.Activate, CardId.Potterie, DeckSSWitchcraft);
            AddExecutor(ExecutorType.Activate, CardId.Genni, DeckSSWitchcraft);
            AddExecutor(ExecutorType.Activate, CardId.PSYGamma, PSYGammaActivate);
            AddExecutor(ExecutorType.Activate, _CardId.MaxxC, MaxxCActivate);
            AddExecutor(ExecutorType.Activate, CardId.GolemAruru, GolemAruruActivate);
            AddExecutor(ExecutorType.Activate, CardId.BorreloadSavageDragon, BorreloadSavageDragonActivate);
            AddExecutor(ExecutorType.Activate, _CardId.InfiniteImpermanence, InfiniteImpermanenceActivate);
            AddExecutor(ExecutorType.Activate, _CardId.AshBlossom, AshBlossom_JoyousSpringActivate);
            AddExecutor(ExecutorType.Activate, _CardId.CalledByTheGrave, CalledbytheGraveActivate);
            AddExecutor(ExecutorType.Activate, _CardId.CrossoutDesignator, CrossoutDesignatorActivate);
            AddExecutor(ExecutorType.Activate, CardId.MagicianRightHand, SpellsActivate);
            AddExecutor(ExecutorType.Activate, CardId.MagiciansLeftHand, SpellsActivate);
            AddExecutor(ExecutorType.Activate, CardId.Unveiling, UnveilingActivate);
            AddExecutor(ExecutorType.Activate, CardId.Draping, DrapingActivate);
            AddExecutor(ExecutorType.Activate, CardId.PSYOmega, PSYOmegaActivate);
            AddExecutor(ExecutorType.Activate, CardId.DracoBerserkeroftheTenyi, DracoBerserkeroftheTenyiActivate);
            AddExecutor(ExecutorType.Activate, CardId.MadameVerre, MadameVerreActivate);
            AddExecutor(ExecutorType.Activate, CardId.Haine, HaineActivate);
            AddExecutor(ExecutorType.Activate, CardId.SalamangreatAlmiraj, SalamangreatAlmirajActivate);

            // PSY auto
            AddExecutor(ExecutorType.Activate, CardId.PSYLambda);
            AddExecutor(ExecutorType.SpSummon, CardId.PSYLambda, PSYLambdaSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.PSYOmega, Lv8Summon);
            AddExecutor(ExecutorType.SpSummon, CardId.BorreloadSavageDragon, BorreloadSavageDragonSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.DracoBerserkeroftheTenyi, Lv8Summon);
            AddExecutor(ExecutorType.SpSummon, CardId.BorreloadSavageDragon, Lv8Summon);

            // auto
            AddExecutor(ExecutorType.Activate, CardId.WitchcrafterBystreet, WitchcraftRecycle);
            AddExecutor(ExecutorType.Activate, WitchcraftRecycle);
            AddExecutor(ExecutorType.Activate, CardId.MetalfoesFusion);
            AddExecutor(ExecutorType.Activate, CardId.TGWonderMagician, TGWonderMagicianActivate);
            AddExecutor(ExecutorType.Activate, CardId.KnightmareUnicorn, KnightmareUnicornActivate);
            AddExecutor(ExecutorType.Activate, CardId.KnightmarePhoenix, KnightmarePhoenixActivate);
            AddExecutor(ExecutorType.Activate, CardId.CrystronHalqifibrax, CrystronHalqifibraxActivate);

            // activate with counter
            AddExecutor(ExecutorType.Activate, CardId.ThatGrassLooksGreener, SpellsActivatewithCounter);
            AddExecutor(ExecutorType.Activate, CardId.Reasoning, SpellsActivatewithCounter);

            // witchcraft summon
            AddExecutor(ExecutorType.Activate, CardId.Masterpiece, MasterpieceActivate);
            AddExecutor(ExecutorType.Activate, CardId.Patronus, PatronusActivate);
            AddExecutor(ExecutorType.Activate, CardId.MagiciansRestage, MagiciansRestageActivate);
            AddExecutor(ExecutorType.Activate, CardId.Holiday, HolidayActivate);

            // summon
            AddExecutor(ExecutorType.Summon, CardId.Schmietta, WitchcraftSummon);
            AddExecutor(ExecutorType.Summon, CardId.Pittore, WitchcraftSummon);
            AddExecutor(ExecutorType.Summon, CardId.Potterie, WitchcraftSummon);
            AddExecutor(ExecutorType.Summon, CardId.Genni, WitchcraftSummon);
            AddExecutor(ExecutorType.Activate, CardId.Creation, CreationActivate);

            // witchcraft resources
            AddExecutor(ExecutorType.Activate, CardId.Pittore, PittoreActivate);
            AddExecutor(ExecutorType.Activate, CardId.Schmietta, SchmiettaActivate);
            AddExecutor(ExecutorType.Activate, CardId.Genni, GenniActivate);
            AddExecutor(ExecutorType.Activate, CardId.Potterie, PotterieActivate);

            // extra calling
            AddExecutor(ExecutorType.SpSummon, CardId.KnightmarePhoenix, KnightmarePhoenixSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.RelinquishedAnima, RelinquishedAnimaSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.CrystronHalqifibrax, CrystronHalqifibraxSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.BorrelswordDragon, BorrelswordDragonSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.KnightmareUnicorn, KnightmareUnicornSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.SalamangreatAlmiraj, SalamangreatAlmirajSummon);
            AddExecutor(ExecutorType.Summon, SummonForLink);

            // activate spells normally
            AddExecutor(ExecutorType.Activate, CardId.ThatGrassLooksGreener, SpellsActivateNoCost);
            AddExecutor(ExecutorType.Activate, CardId.Reasoning, SpellsActivateNoCost);
            AddExecutor(ExecutorType.Activate, CardId.MagicianRightHand, SpellsActivateNoCost);
            AddExecutor(ExecutorType.Activate, CardId.MagiciansLeftHand, SpellsActivateNoCost);

            //AddExecutor(ExecutorType.SummonOrSet);

            // rest
            AddExecutor(ExecutorType.Summon, WitchcraftSummonForRecycle);
            AddExecutor(ExecutorType.Repos, MonsterRepos);
            AddExecutor(ExecutorType.Activate, CardId.WitchcrafterBystreet, WitchcrafterBystreetActivate);
            AddExecutor(ExecutorType.Activate, CardId.Scroll, ScrollActivate);
            AddExecutor(ExecutorType.SpellSet, SpellSet);
        }

        int Witchcraft_setcode = 0x128;
        int TimeLord_setcode = 0x4a;
        int[] important_witchcraft = { CardId.Haine, CardId.MadameVerre };
        Dictionary<int, int> witchcraft_level = new Dictionary<int, int> {
            {CardId.GolemAruru, 8}, {CardId.MadameVerre, 7}, {CardId.Haine, 7}, {CardId.Schmietta, 4},
            {CardId.Pittore, 3}, {CardId.Potterie, 2}, {CardId.Genni, 1}
        };

        List<int> Impermanence_list = new List<int>();
        List<int> FirstCheckSS = new List<int>();
        List<int> UseSSEffect = new List<int>();
        List<int> ActivatedCards = new List<int>();
        List<int> currentNegatingIdList = new List<int>();
        bool MadameVerreGainedATK = false;
        bool summoned = false;
        bool enemy_activate_MaxxC = false;
        bool enemy_activate_DimensionShifter = false;
        bool MagiciansLeftHand_used = false;
        bool MagicianRightHand_used = false;

        // go first
        public override bool OnSelectHand()
        {
            return true;
        }

        // reset the negated card in case of activated again
        public override void OnChainEnd()
        {
            currentNegatingIdList.Clear();
            base.OnChainEnd();
        }

        public override void OnChainSolved(int chainIndex)
        {
            ChainInfo currentCard = Duel.GetCurrentSolvingChainInfo();
            if (currentCard != null && currentCard.ActivatePlayer == 1)
            {
                if (Duel.IsCurrentSolvingChainNegated())
                {
                    // MagiciansLeftHand / MagicianRightHand
                    if (!MagicianRightHand_used && currentCard.IsSpell())
                    {
                        if (Bot.MonsterZone.GetFirstMatchingCard(c => c.HasRace(CardRace.SpellCaster) && c.IsFaceup()) != null
                            && Bot.HasInSpellZone(CardId.MagicianRightHand, true))
                        {
                            Logger.DebugWriteLine("MagicianRightHand negate: " + currentCard.RelatedCard.Name ?? "???");
                            MagicianRightHand_used = true;
                        }
                    }
                    if (!MagiciansLeftHand_used && currentCard.IsTrap() && currentCard.ActivatePlayer == 1)
                    {
                        if (Bot.MonsterZone.GetFirstMatchingCard(c => c.HasRace(CardRace.SpellCaster) && c.IsFaceup()) != null
                            && Bot.HasInSpellZone(CardId.MagiciansLeftHand, true))
                        {
                            Logger.DebugWriteLine("MagiciansLeftHand negate: " + currentCard.RelatedCard.Name ?? "???");
                            MagiciansLeftHand_used = true;
                        }
                    }
                }
                if (!Duel.IsCurrentSolvingChainNegated())
                {
                    if (currentCard.IsCode(_CardId.MaxxC))
                        enemy_activate_MaxxC = true;
                    if (currentCard.IsCode(CardId.DimensionShifter))
                        enemy_activate_DimensionShifter = true;
                    if (currentCard.IsCode(_CardId.InfiniteImpermanence))
                    {
                        for (int i = 0; i < 5; ++i)
                        {
                            if (Enemy.SpellZone[i] == currentCard.RelatedCard)
                            {
                                Impermanence_list.Add(4 - i);
                                break;
                            }
                        }
                    }
                }
            }
        }

        // new turn reset
        public override void OnNewTurn()
        {
            MadameVerreGainedATK = false;
            summoned = false;
            enemy_activate_MaxxC = false;
            enemy_activate_DimensionShifter = false;
            MagiciansLeftHand_used = false;
            MagicianRightHand_used = false;
            Impermanence_list.Clear();
            FirstCheckSS.Clear();
            UseSSEffect.Clear();
            ActivatedCards.Clear();
            currentNegatingIdList.Clear();
            base.OnNewTurn();
        }

        // power fix
        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            if (!defender.IsMonsterHasPreventActivationEffectInBattle())
            {
                if (!MadameVerreGainedATK && Bot.HasInMonstersZone(CardId.MadameVerre, true, false, true) && attacker.HasSetcode(Witchcraft_setcode)) 
                {
                    attacker.RealPower += CheckPlusAttackforMadameVerre();
                }
            }
            return base.OnPreBattleBetween(attacker, defender);
        }

        // overwrite OnSelectCard to act normally in SelectUnselect
        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, int hint, bool cancelable)
        {
            // Patronus
            if (hint == HintMsg.AddToHand)
            {
                bool flag = true;
                foreach(ClientCard card in cards)
                {
                    if (!card.HasSetcode(Witchcraft_setcode) || card.Location != CardLocation.Removed || !card.IsSpell())
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    Logger.DebugWriteLine("** Patronus recycle.");
                    // select all
                    IList<ClientCard> selected = new List<ClientCard>();
                    for (int i = 1; i <= max; ++i)
                    {
                        selected.Add(cards[cards.Count - i]);
                        Logger.DebugWriteLine("** Select " + cards[cards.Count - i].Name ?? "???");
                    }
                    return selected;
                }
            }
            // MaxxC solution
            if (hint == HintMsg.SpSummon && enemy_activate_MaxxC)
            {
                // check whether SS from deck while using effect
                bool flag = true;
                List<int> levels = new List<int>();
                List<int> check_cardid = new List<int> { CardId.Haine, CardId.MadameVerre, CardId.GolemAruru };
                List<ClientCard> checked_card = new List<ClientCard> { null, null, null };
                foreach (ClientCard card in cards)
                {
                    if (card != null && card.Location == CardLocation.Deck && card.Controller == 0 && card.HasSetcode(Witchcraft_setcode))
                    {
                        for (int i = 0; i < 3; ++i)
                        {
                            if (card.Id == check_cardid[i])
                            {
                                checked_card[i] = card;
                            }
                        }
                        // Patronus also special summon from deck
                        if (!levels.Contains(card.Level))
                        {
                            levels.Add(card.Level);
                        }
                    }
                    else
                    {
                        flag = false;
                        break;
                    }
                }
                
                // only special summon advance monster
                if (flag && levels.Count > 1)
                {
                    Logger.DebugWriteLine("SS with MaxxC.");
                    IList<ClientCard> result = new List<ClientCard>();
                    // check MadameVerre
                    int extra_attack = CheckPlusAttackforMadameVerre(true, true, true);
                    int bot_best = Util.GetBestAttack(Bot);
                    if (CheckProblematicCards() != null && Util.IsAllEnemyBetterThanValue(bot_best + extra_attack, true) == false)
                    {
                        if (!Bot.HasInMonstersZone(CardId.MadameVerre) && checked_card[1] != null)
                        {
                            result.Add(checked_card[1]);
                            return result;
                        } 
                    }
                    for (int i = 0; i < 3; ++i)
                    {
                        if (checked_card[i] != null)
                        {
                            result.Add(checked_card[i]);
                            return result;
                        }
                    }
                }
            }
            // MadameVerre
            if (hint == HintMsg.Confirm)
            {
                Logger.DebugWriteLine("** min-max: " + min.ToString() + " / " + max.ToString());
                foreach (ClientCard card in cards)
                {
                    Logger.DebugWriteLine(card.Name ?? "???");
                }

                // select all
                IList<ClientCard> selected = new List<ClientCard>();
                for (int i = 1; i <= max; ++i)
                {
                    selected.Add(cards[cards.Count - i]);
                    Logger.DebugWriteLine("** Select " + cards[cards.Count - i].Name ?? "???");
                }
                return selected;
            }

            return base.OnSelectCard(cards, min, max, hint, cancelable);
        }

        // position select
        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            NamedCard Data = NamedCard.Get(cardId);
            if (Data == null)
            {
                return base.OnSelectPosition(cardId, positions);
            }
            if (!Enemy.HasInMonstersZone(_CardId.BlueEyesChaosMAXDragon) 
                && (Duel.Player == 1 && (cardId == CardId.MadameVerre ||
                Util.GetOneEnemyBetterThanValue(Data.Attack + 1) != null))
                || cardId == _CardId.MaxxC || cardId == _CardId.AshBlossom)
            {
                return CardPosition.FaceUpDefence;
            }
            if (cardId == CardId.MadameVerre && Util.IsTurn1OrMain2())
            {
                return CardPosition.FaceUpDefence;
            }
            return base.OnSelectPosition(cardId, positions);
        }

        // shuffle List<ClientCard>
        public List<ClientCard> CardListShuffle(List<ClientCard> list)
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

        // check negated time count of id
        public int CheckCalledbytheGrave(int id)
        {
            if (currentNegatingIdList.Contains(id)) return 1;
            if (DefaultCheckWhetherCardIdIsNegated(id)) return 1;
            return 0;
        }

        // check enemy's dangerous card in grave
        public List<ClientCard> CheckDangerousCardinEnemyGrave(bool onlyMonster = false)
        {
            List<ClientCard> result = Enemy.Graveyard.GetMatchingCards(card => 
            (!onlyMonster || card.IsMonster()) && card.HasSetcode(0x11b)).ToList();
            return result;
        }

        /// <summary>
        /// Check count of discardable spells for witchcraft monsters.
        /// </summary>
        /// <param name="except">Card that prepared to use and can't discard.</param>
        public int CheckDiscardableSpellCount(ClientCard except = null)
        {
            int discardable_hands = 0;
            int count_witchcraftspell = Bot.Hand.GetMatchingCardsCount(card => (card.IsSpell() && (card.HasSetcode(Witchcraft_setcode)) && card != except));
            int count_remainhands = CheckRemainInDeck(CardId.MagiciansLeftHand, CardId.MagicianRightHand);
            int count_MagiciansRestage = Bot.Hand.GetMatchingCardsCount(card => card.Id == CardId.MagiciansRestage && card != except);
            int count_MetalfoesFusion = Bot.Hand.GetCardCount(CardId.MetalfoesFusion);
            int count_WitchcrafterBystreet = Bot.SpellZone.GetMatchingCardsCount(card => card.IsFaceup() && card.Id == CardId.WitchcrafterBystreet && !card.IsDisabled());
            if (count_MagiciansRestage > 0)
            {
                discardable_hands += (count_MagiciansRestage > count_remainhands ? count_remainhands : count_MagiciansRestage);
            }
            if (!ActivatedCards.Contains(CardId.WitchcrafterBystreet) && (count_WitchcrafterBystreet >= 2 || (count_WitchcrafterBystreet >= 1 && Duel.Phase > DuelPhase.Battle)))
            {
                discardable_hands += 1;
            }
            discardable_hands += count_witchcraftspell + count_MetalfoesFusion;
            return discardable_hands;
        }

        /// <summary>
        /// Check whether last chain card should be disabled.
        /// </summary>
        public bool CheckLastChainNegated()
        {
            ClientCard lastcard = Util.GetLastChainCard();
            if (lastcard == null || lastcard.Controller != 1) return false;
            if (lastcard.IsMonster() && lastcard.HasSetcode(TimeLord_setcode) && Duel.Phase == DuelPhase.Standby) return false;
            if (DefaultCheckWhetherCardIdIsNegated(lastcard.GetOriginCode())) return false;

            // MagiciansLeftHand / MagicianRightHand
            if (!MagicianRightHand_used && lastcard.IsSpell())
            {
                if (Bot.MonsterZone.GetFirstMatchingCard(c => c.HasRace(CardRace.SpellCaster) && c.IsFaceup()) != null
                    && Bot.HasInSpellZone(CardId.MagicianRightHand, true))
                {
                    return true;
                }
            }
            if (!MagiciansLeftHand_used && lastcard.IsTrap())
            {
                if (Bot.MonsterZone.GetFirstMatchingCard(c => c.HasRace(CardRace.SpellCaster) && c.IsFaceup()) != null
                    && Bot.HasInSpellZone(CardId.MagiciansLeftHand, true))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Check whether match link condition.
        /// </summary>
        /// <param name="LinkCount">Min Link count</param>
        /// <param name="MaterialCount">Min material count</param>
        /// <param name="list">materails list</param>
        /// <param name="need_tune">whether need tuner</param>
        /// <returns></returns>
        public bool CheckLinkMaterialsMatch(int LinkCount, int MaterialCount, List<ClientCard> list, bool need_tune = false)
        {
            // material count check
            if (list.Count < MaterialCount) return false;
            
            // link marker check
            int linkcount = 0;
            foreach(ClientCard card in list)
            {
                linkcount += (card.HasType(CardType.Link) ? card.LinkCount : 1);
            }
            if (linkcount != LinkCount)
            {
                foreach (ClientCard card in list)
                {
                    linkcount += 1;
                }
                if (linkcount != LinkCount) return false;
            }

            // tuner check
            if (need_tune && list.GetFirstMatchingCard(card => card.IsTuner()) == null) return false;
            return true;
        }

        /// <summary>
        /// Check link summon materials. If not enough, return an empty list.
        /// </summary>
        /// <param name="LinkCount">Link monster's link count.</param>
        /// <param name="MaterialCount">Link monster's least material count.</param>
        /// <param name="need_tuner">Whether materials need tuner(use for CrystronHalqifibrax)</param>
        /// <param name="extra">Extra monster use for material check.</param>
        public List<ClientCard> CheckLinkMaterials(int LinkCount, int MaterialCount, bool need_tuner = false, List<ClientCard> extra = null)
        {
            List<int> psy_cardids = new List<int> { CardId.PSYGamma, CardId.PSYDriver };
            List<ClientCard> result = Bot.MonsterZone.GetMatchingCards(card => card.IsFaceup() && psy_cardids.Contains(card.Id)).ToList();
            if (CheckLinkMaterialsMatch(LinkCount, MaterialCount, result, need_tuner)) return result;

            List<ClientCard> bot_monsters = Enemy.MonsterZone.GetMatchingCards(c => c.IsFaceup()).ToList();
            if (extra != null) bot_monsters = bot_monsters.Union(extra).ToList();
            bot_monsters.Sort(CardContainer.CompareCardAttack);

            int remaindiscard = CheckDiscardableSpellCount();
            int enemybest = Util.GetBestAttack(Enemy);
            foreach (ClientCard card in bot_monsters)
            {
                if ((card.HasSetcode(Witchcraft_setcode) && (card.Level >= 5 || remaindiscard >= 2))
                    || (card.Attack >= enemybest)
                    || (card.HasType(CardType.Link) && card.LinkMarker > 2))
                {
                    continue;
                }
                result.Add(card);
                if (CheckLinkMaterialsMatch(LinkCount, MaterialCount, result, need_tuner)) return result;
            }
            if (!CheckLinkMaterialsMatch(LinkCount, MaterialCount, result, need_tuner)) result.Clear();

            return result;
        }

        /// <summary>
        /// Check how many attack MadameVerre can provide
        /// </summary>
        /// <param name="ignore_activated">whether ignore the activate of MadameVerre</param>
        /// <param name="check_recycle">check prerecycle spells in grave</param>
        /// <param name="force">force check whether have MadameVerre</param>
        public int CheckPlusAttackforMadameVerre(bool ignore_activated = false, bool check_recycle = false, bool force = false)
        {
            // not MadameVerre on field
            if (!force && Bot.MonsterZone.GetFirstMatchingCard(card => card.Id == CardId.MadameVerre && !card.IsDisabled()) == null) return 0;
            if (!ignore_activated && MadameVerreGainedATK) return 0;

            HashSet<int> spells_id = new HashSet<int>();
            foreach(ClientCard card in Bot.Hand)
            {
                if (card.IsSpell())
                {
                    spells_id.Add(card.Id);
                }
            }
            if (check_recycle && Bot.MonsterZone.GetFirstMatchingCard(card => card.IsFaceup() && card.HasSetcode(Witchcraft_setcode)) != null)
            {
                List<int> spell_checklist = new List<int> { CardId.Holiday, CardId.Creation, CardId.Draping, CardId.Unveiling, CardId.Collaboration };
                foreach (int cardid in spell_checklist)
                {
                    if (Bot.HasInGraveyard(cardid) && !ActivatedCards.Contains(cardid))
                    {
                        spells_id.Add(Card.Id);
                    }
                }
            }
            int max_hand = spells_id.Count() >= 6 ? 6 : spells_id.Count();
            return max_hand * 1000;
            
        }

        /// <summary>
        /// Check problematic cards on enemy's field.
        /// </summary>
        /// <param name="canBeTarget">whether can be targeted</param>
        /// <param name="OnlyDanger">only check danger monsters</param>
        public ClientCard CheckProblematicCards(bool canBeTarget = false, bool OnlyDanger = false)
        {
            ClientCard card = Enemy.MonsterZone.GetFloodgate(canBeTarget);
            if (card != null)
                return card;

            card = Enemy.MonsterZone.GetDangerousMonster(canBeTarget);
            if (card != null
                && (Duel.Player == 0 || (Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2)))
                return card;

            card = Enemy.MonsterZone.GetInvincibleMonster(canBeTarget);
            if (card != null
                && (Duel.Player == 0 || (Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2)))
                return card;

            List<ClientCard> enemy_monsters = Enemy.MonsterZone.GetMatchingCards(c => c.IsFaceup()).ToList();
            enemy_monsters.Sort(CardContainer.CompareCardAttack);
            enemy_monsters.Reverse();
            foreach (ClientCard target in enemy_monsters)
            {
                if (target.HasType(CardType.Fusion) || target.HasType(CardType.Ritual) || target.HasType(CardType.Synchro) || target.HasType(CardType.Xyz) || (target.HasType(CardType.Link) && target.LinkCount >= 2))
                {
                    if (!canBeTarget || !(target.IsShouldNotBeTarget() || target.IsShouldNotBeMonsterTarget())) return target;
                }
            }

            if (OnlyDanger) return null;

            int highest_self = Util.GetBestPower(Bot);
            if (!MadameVerreGainedATK && Bot.HasInMonstersZone(CardId.MadameVerre, true, false, true))
            {
                highest_self += CheckPlusAttackforMadameVerre();
            }
            return Util.GetProblematicEnemyCard(highest_self, canBeTarget);
        }

        /// <summary>
        /// Check how many spells can be recylced to hand.
        /// </summary>
        public int CheckRecyclableCount(bool tohand = false, bool ignore_monster = false)
        {
            if (!ignore_monster && Bot.MonsterZone.GetFirstMatchingCard(card => card.IsFaceup() && card.HasSetcode(Witchcraft_setcode)) == null) return 0;
            int result = 0;
            List<int> spell_checklist = new List<int> { CardId.Holiday, CardId.Creation, CardId.Draping, CardId.Unveiling, CardId.Collaboration };
            if (!tohand)
            {
                spell_checklist.Add(CardId.WitchcrafterBystreet);
                spell_checklist.Add(CardId.Scroll);
            }
            foreach (int cardid in spell_checklist)
            {
                if (Bot.HasInGraveyard(cardid) && !ActivatedCards.Contains(cardid))
                {
                    result++;
                }
            }
            return result;
        }

        /// <summary>
        /// Check remain cards in deck
        /// </summary>
        /// <param name="id">Card's ID</param>
        public int CheckRemainInDeck(int id)
        {
            switch (id)
            {
                case CardId.PSYDriver:
                    return Bot.GetRemainingCount(CardId.PSYDriver, 1);
                case CardId.GolemAruru:
                    return Bot.GetRemainingCount(CardId.GolemAruru, 1);
                case CardId.MadameVerre:
                    return Bot.GetRemainingCount(CardId.MadameVerre, 1);
                case CardId.Haine:
                    return Bot.GetRemainingCount(CardId.Haine, 2);
                case CardId.Schmietta:
                    return Bot.GetRemainingCount(CardId.Schmietta, 3);
                case CardId.Pittore:
                    return Bot.GetRemainingCount(CardId.Pittore, 3);
                case _CardId.AshBlossom:
                    return Bot.GetRemainingCount(_CardId.AshBlossom, 1);
                case CardId.PSYGamma:
                    return Bot.GetRemainingCount(CardId.PSYGamma, 3);
                case _CardId.MaxxC:
                    return Bot.GetRemainingCount(_CardId.MaxxC, 1);
                case CardId.Potterie:
                    return Bot.GetRemainingCount(CardId.Potterie, 1);
                case CardId.Genni:
                    return Bot.GetRemainingCount(CardId.Genni, 2);
                case CardId.Collaboration:
                    return Bot.GetRemainingCount(CardId.Collaboration, 1);
                case CardId.ThatGrassLooksGreener:
                    return Bot.GetRemainingCount(CardId.ThatGrassLooksGreener, 2);
                case _CardId.LightningStorm:
                    return Bot.GetRemainingCount(_CardId.LightningStorm, 2);
                case CardId.PotofExtravagance:
                    return Bot.GetRemainingCount(CardId.PotofExtravagance, 3);
                case CardId.DarkRulerNoMore:
                    return Bot.GetRemainingCount(CardId.DarkRulerNoMore, 2);
                case CardId.Creation:
                    return Bot.GetRemainingCount(CardId.Creation, 3);
                case CardId.Reasoning:
                    return Bot.GetRemainingCount(CardId.Reasoning, 3);
                case CardId.MetalfoesFusion:
                    return Bot.GetRemainingCount(CardId.MetalfoesFusion, 1);
                case CardId.Holiday:
                    return Bot.GetRemainingCount(CardId.Holiday, 3);
                case _CardId.CalledByTheGrave:
                    return Bot.GetRemainingCount(_CardId.CalledByTheGrave, 3);
                case CardId.Draping:
                    return Bot.GetRemainingCount(CardId.Draping, 1);
                case _CardId.CrossoutDesignator:
                    return Bot.GetRemainingCount(_CardId.CrossoutDesignator, 2);
                case CardId.Unveiling:
                    return Bot.GetRemainingCount(CardId.Unveiling, 1);
                case CardId.MagiciansLeftHand:
                    return Bot.GetRemainingCount(CardId.MagiciansLeftHand, 1);
                case CardId.Scroll:
                    return Bot.GetRemainingCount(CardId.Scroll, 1);
                case CardId.MagiciansRestage:
                    return Bot.GetRemainingCount(CardId.MagiciansRestage, 2);
                case CardId.WitchcrafterBystreet:
                    return Bot.GetRemainingCount(CardId.WitchcrafterBystreet, 3);
                case CardId.MagicianRightHand:
                    return Bot.GetRemainingCount(CardId.MagicianRightHand, 1);
                case _CardId.InfiniteImpermanence:
                    return Bot.GetRemainingCount(_CardId.InfiniteImpermanence, 3);
                case CardId.Masterpiece:
                    return Bot.GetRemainingCount(CardId.Masterpiece, 1);
                case CardId.Patronus:
                    return Bot.GetRemainingCount(CardId.Patronus, 2);
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Check remain cards in deck
        /// </summary>
        /// <param name="ids">Card's ID list</param>
        public int CheckRemainInDeck(params int[] ids)
        {
            int result = 0;
            foreach (int cardid in ids)
            {
                result += CheckRemainInDeck(cardid);
            }
            return result;
        }

        /// <summary>
        /// Check whether cards will be removed. If so, do not send cards to grave.
        /// </summary>
        public bool CheckWhetherWillbeRemoved()
        {
            if (enemy_activate_DimensionShifter) return true;
            List<int> check_card = new List<int> { CardId.BanisheroftheRadiance, CardId.BanisheroftheLight, CardId.MacroCosmos, CardId.DimensionalFissure };
            foreach(int cardid in check_card)
            {
                List<ClientField> fields = new List<ClientField> { Bot, Enemy };
                foreach (ClientField cf in fields)
                {
                    if (cf.HasInMonstersZone(cardid, true) || cf.HasInSpellZone(cardid, true))
                    {
                        return true;
                    }
                }
            }
            return false;
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
            if (CheckCalledbytheGrave(target.GetOriginCode()) > 0) return true;
            // won't negate if not on field
            if (target.Location != CardLocation.SpellZone && target.Location != CardLocation.Hand) return false;

            // negate judge
            if (Enemy.HasInMonstersZone(CardId.NaturalExterio, true) && !isCounter) return true;
            if (target.IsSpell())
            {
                if (Enemy.HasInMonstersZone(_CardId.NaturiaBeast, true)) return true;
                if (Enemy.HasInSpellZone(_CardId.ImperialOrder, true) || Bot.HasInSpellZone(_CardId.ImperialOrder, true)) return true;
                if (Enemy.HasInMonstersZone(CardId.SwordsmanLV7, true) || Bot.HasInMonstersZone(CardId.SwordsmanLV7, true)) return true;
            }
            if (target.IsTrap())
            {
                if (Enemy.HasInSpellZone(_CardId.RoyalDecreel, true) || Bot.HasInSpellZone(_CardId.RoyalDecreel, true)) return true;
            }
            // how to get here?
            return false;
        }

        /// <summary>
        /// Check whether'll be negated
        /// </summary>
        public bool NegatedCheck(bool disablecheck = true){
            if (Card.IsSpell() || Card.IsTrap()){
                if (SpellNegatable()) return true;
            }
            if (CheckCalledbytheGrave(Card.GetOriginCode()) > 0){
                return true;
            }
            if (Card.IsMonster() && Card.Location == CardLocation.MonsterZone && Card.IsDefense())
            {
                if (Enemy.MonsterZone.GetFirstMatchingFaceupCard(card => card.Id == CardId.Numbe41BagooskatheTerriblyTiredTapir && card.IsDefense() && !card.IsDisabled()) != null
                    || Bot.MonsterZone.GetFirstMatchingFaceupCard(card => card.Id == CardId.Numbe41BagooskatheTerriblyTiredTapir && card.IsDefense() && !card.IsDisabled()) != null)
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
        /// <param name="avoid_Impermanence">Whether need to avoid InfiniteImpermanence</param>
        /// <param name="avoid_list">Whether need to avoid set in this place</param>
        public void SelectSTPlace(ClientCard card = null, bool avoid_Impermanence = false, List<int> avoid_list = null)
        {
            if (card == null) card = Card;
            List<int> list = new List<int>();
            for (int seq = 0; seq < 5; ++seq)
            {
                if (Bot.SpellZone[seq] == null)
                {
                    if (card != null && card.Location == CardLocation.Hand && avoid_Impermanence && Impermanence_list.Contains(seq)) continue;
                    if (avoid_list != null && avoid_list.Contains(seq)) continue;
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
            if (avoid_Impermanence && Bot.GetMonsters().Any(c => c.IsFaceup() && !c.IsDisabled()))
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

        // Spell&trap's set
        public bool SpellSet(){
            if (Duel.Phase == DuelPhase.Main1 && Bot.HasAttackingMonster() && Duel.Turn > 1) return false;
            if (Card.Id == _CardId.CrossoutDesignator && Duel.Turn >= 5) return false;

            // set condition
            int[] activate_with_condition = { CardId.Masterpiece, CardId.Draping };
            if (activate_with_condition.Contains(Card.Id))
            {
                if (Bot.MonsterZone.GetFirstMatchingCard(card => card.HasSetcode(Witchcraft_setcode)) == null)
                {
                    return false;
                }
            }
            if (Card.Id == CardId.Unveiling)
            {
                return false;
            }
            if (Card.Id == CardId.Patronus)
            {
                int count = Bot.Banished.GetMatchingCardsCount(card => card.HasSetcode(Witchcraft_setcode));
                if (count == 0)
                {
                    count += Bot.Graveyard.GetMatchingCardsCount(card => card.HasSetcode(Witchcraft_setcode));
                }
                if (count == 0)
                {
                    return false;
                }
            }

            // prepare spells to discard
            if (Card.IsSpell()){
                int spells_todiscard = CheckRecyclableCount() + Bot.Hand.GetMatchingCardsCount(card => card.IsSpell());
                int will_discard = 0;
                if (Bot.HasInMonstersZone(CardId.Haine)) will_discard ++;
                if (Bot.HasInMonstersZone(CardId.MadameVerre)) will_discard ++;

                if (will_discard >= spells_todiscard){
                    return false;
                }
            }

            // select place
            if ((Card.IsTrap() || Card.HasType(CardType.QuickPlay)))
            {
                List<int> avoid_list = new List<int>();
                int Impermanence_set = 0;
                for (int i = 0; i < 5; ++i)
                {
                    if (Enemy.SpellZone[i] != null && Enemy.SpellZone[i].IsFaceup() && Bot.SpellZone[4 - i] == null)
                    {
                        avoid_list.Add(4 - i);
                        Impermanence_set += (int)System.Math.Pow(2, 4 - i);
                    }
                }
                if (Bot.HasInHand(_CardId.InfiniteImpermanence))
                {
                    if (Card.IsCode(_CardId.InfiniteImpermanence))
                    {
                        AI.SelectPlace(Impermanence_set);
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
            // anti-spell relevant
            else if (Enemy.HasInSpellZone(CardId.Anti_Spell, true) || Bot.HasInSpellZone(CardId.Anti_Spell, true))
            {
                if (Card.IsSpell() && Card.Id != CardId.MetalfoesFusion)
                {
                    SelectSTPlace();
                    return true;
                }
            }
            return false;
        }

        // Spell&trap's set for Performapal Five-Rainbow Magician
        public bool SpellSetForFiveRainbow()
        {
            // check
            bool have_FiveRainbow = false;
            List<ClientCard> list = new List<ClientCard>();
            if (Duel.IsNewRule || Duel.IsNewRule2020)
            {
                list.Add(Enemy.SpellZone[0]);
                list.Add(Enemy.SpellZone[4]);
            }
            else
            {
                list.Add(Enemy.SpellZone[6]);
                list.Add(Enemy.SpellZone[7]);
            }
            foreach(ClientCard card in list)
            {
                if (card != null && card.Id == CardId.PerformapalFive_RainbowMagician)
                {
                    have_FiveRainbow = true;
                    break;
                }
            }

            if (!have_FiveRainbow) return false;
            if (Bot.GetMonsterCount() == 0 || Bot.SpellZone.GetFirstMatchingCard(card => card.IsFacedown()) != null) return false;
            if (Card.IsSpell())
            {
                SelectSTPlace(null, true);
                return true;
            }

            return false;
        }

        // use for repos
        public bool MonsterRepos()
        {
            int self_attack = Card.Attack + 1;
            int extra_attack = CheckPlusAttackforMadameVerre(true, true);
            Logger.DebugWriteLine("self_attack of " + (Card.Name ?? "X") + ": " + self_attack.ToString());
            if (Card.HasSetcode(Witchcraft_setcode))
            {
                self_attack += extra_attack;
            }

            if (Card.IsFaceup() && Card.IsDefense() && self_attack <= 1)
                return false;

            int best_attack = 0;
            foreach (ClientCard card in Bot.GetMonsters())
            {
                int attack = card.Attack;
                if (card.HasSetcode(Witchcraft_setcode))
                {
                    attack += extra_attack;
                }
                if (attack >= best_attack)
                {
                    best_attack = attack;
                }
            }

            bool enemyBetter = Util.IsAllEnemyBetterThanValue(best_attack, true);

            if (Card.IsAttack() && enemyBetter)
                return true;
            if (Card.IsDefense() && !enemyBetter && self_attack >= Card.Defense)
                return true;
            return false;
        }

        /// <summary>
        /// Select spell cost for Witchcraft.
        /// </summary>
        public void SelectDiscardSpell()
        {
            int count_remainhands = CheckRemainInDeck(CardId.MagiciansLeftHand, CardId.MagicianRightHand);
            int count_witchcraftspell = Bot.Hand.GetMatchingCardsCount(card => (card.IsSpell() && (card.HasSetcode(Witchcraft_setcode))));
            int WitchcrafterBystreet_count = Bot.SpellZone.GetMatchingCardsCount(card => card.IsFaceup() && card.Id == CardId.WitchcrafterBystreet);
            if (Bot.HasInHand(CardId.MagiciansRestage) && count_remainhands > 0)
            {
                AI.SelectCard(CardId.MagiciansRestage);
            }
            else if (Bot.HasInHand(CardId.MetalfoesFusion))
            {
                AI.SelectCard(CardId.MetalfoesFusion);
            }
            else if (!ActivatedCards.Contains(CardId.Scroll) && Bot.SpellZone.GetCardCount(CardId.Scroll) > 0)
            {
                AI.SelectCard(Bot.SpellZone.GetFirstMatchingFaceupCard(card => card.Id == CardId.Scroll));
                ActivatedCards.Add(CardId.Scroll);
            }
            else if (!ActivatedCards.Contains(CardId.WitchcrafterBystreet) && WitchcrafterBystreet_count >= 2)
            {
                AI.SelectCard(Bot.SpellZone.GetFirstMatchingFaceupCard(card => card.Id == CardId.WitchcrafterBystreet));
                ActivatedCards.Add(CardId.WitchcrafterBystreet);
            }
            else if (count_witchcraftspell > 0)
            {
                List<int> cost_list = new List<int>{ CardId.Scroll, CardId.WitchcrafterBystreet, CardId.Collaboration, CardId.Unveiling, CardId.Draping };
                if (Duel.Player == 1)
                {
                    cost_list.Add(CardId.Creation);
                    cost_list.Add(CardId.Holiday);
                } else
                {
                    cost_list.Add(CardId.Holiday);
                    cost_list.Add(CardId.Creation);
                }
                foreach (int cardid in cost_list)
                {
                    IList<ClientCard> targets = Bot.Hand.GetMatchingCards(card => card.Id == cardid);
                    if (targets.Count() > 0)
                    {
                        AI.SelectCard(targets);
                        return;
                    }
                }
                AI.SelectCard(CardId.Scroll, CardId.WitchcrafterBystreet);
            }
            else if (Bot.HasInHand(CardId.PotofExtravagance) && Bot.ExtraDeck.Count < 6)
            {
                AI.SelectCard(CardId.PotofExtravagance);
            }
            else if (WitchcrafterBystreet_count >= 1)
            {
                AI.SelectCard(Bot.SpellZone.GetFirstMatchingFaceupCard(card => card.Id == CardId.WitchcrafterBystreet));
                ActivatedCards.Add(CardId.WitchcrafterBystreet);
            }
            else
            {
                AI.SelectCard(CardId.ThatGrassLooksGreener, _CardId.LightningStorm, CardId.PotofExtravagance, CardId.MagiciansLeftHand, CardId.MagicianRightHand, _CardId.CrossoutDesignator, _CardId.CalledByTheGrave);
            }
        }

        /// <summary>
        /// For normal spells activate
        /// </summary>
        public bool SpellsActivate()
        {
            if (SpellNegatable()) return false;
            if (CheckDiscardableSpellCount() <= 1) return false;
            if ((Card.Id == CardId.ThatGrassLooksGreener || Card.Id == CardId.Reasoning) && CheckWhetherWillbeRemoved()) return false;
            if (Card.Id == CardId.MagiciansLeftHand || Card.Id == CardId.MagicianRightHand)
            {
                if (Bot.MonsterZone.GetFirstMatchingCard(card => card.HasRace(CardRace.SpellCaster)) == null
                    && (summoned || Bot.Hand.GetFirstMatchingCard(card => card.HasRace(CardRace.SpellCaster) && card.Level <= 4) == null))
                {
                    return false;
                }
            }
            SelectSTPlace(Card, true);
            return true;
        }

        /// <summary>
        /// For normal spells activate without cost
        /// </summary>
        public bool SpellsActivateNoCost()
        {
            if (SpellNegatable()) return false;
            if ((Card.Id == CardId.ThatGrassLooksGreener || Card.Id == CardId.Reasoning) && CheckWhetherWillbeRemoved()) return false;
            if (Card.Id == CardId.MagiciansLeftHand || Card.Id == CardId.MagicianRightHand)
            {
                if (Bot.MonsterZone.GetFirstMatchingCard(card => card.HasRace(CardRace.SpellCaster)) == null
                    && (summoned || Bot.Hand.GetFirstMatchingCard(card => card.HasRace(CardRace.SpellCaster) && card.Level <= 4) == null))
                {
                    return false;
                }
            }
            SelectSTPlace(Card, true);
            return true;
        }

        /// <summary>
        /// Check wheter have enough counter to care for important spells. if not, delay it.
        /// </summary>
        public bool SpellsActivatewithCounter()
        {
            if (SpellNegatable()) return false;
            if ((Card.Id == CardId.ThatGrassLooksGreener || Card.Id == CardId.Reasoning) && CheckWhetherWillbeRemoved()) return false;
            int[] counter_cards = { CardId.PSYGamma, _CardId.CalledByTheGrave, _CardId.CrossoutDesignator };
            int count = Bot.Hand.GetMatchingCardsCount(card => counter_cards.Contains(card.Id));
            count += Bot.SpellZone.GetMatchingCardsCount(card => counter_cards.Contains(card.Id));
            if (count > 0 || Bot.Hand.GetCardCount(Card.Id) >= 2)
            {
                SelectSTPlace(Card, true);
                return true;
            }
            return Program.Rand.Next(2) > 0;
        }

        /// <summary>
        /// Summon Witchcraft for special summoning from deck.
        /// </summary>
        public bool WitchcraftSummon()
        {
            if (UseSSEffect.Contains(Card.Id)) return false;
            int count_spell = Bot.Hand.GetMatchingCardsCount(card => (card.IsSpell()));
            int count_target = CheckRemainInDeck(CardId.MadameVerre, CardId.Haine, CardId.GolemAruru);
            if (count_spell > 0 && count_target > 0)
            {
                summoned = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Summon Witchcraft for recycling spells
        /// </summary>
        public bool WitchcraftSummonForRecycle()
        {
            if (!Card.HasSetcode(Witchcraft_setcode) || Card.Level > 4)
            {
                return false;
            }
            if (CheckRecyclableCount(false, true) > 0 && Bot.MonsterZone.GetFirstMatchingFaceupCard(card => card.HasSetcode(Witchcraft_setcode)) == null)
            {
                summoned = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check whether summon monster for link summon.
        /// </summary>
        public bool SummonForLink()
        {
            // reject advance summon
            if (Card.Level >= 5) return false;
            summoned = true;

            if (BorrelswordDragonSummonCheck(Card).Count >= 3)
            {
                Logger.DebugWriteLine("Summon for BorrelswordDragon.");
                List<ClientCard> list = BorrelswordDragonSummonCheck(Card);
                foreach( ClientCard c in list)
                {
                    Logger.DebugWriteLine(c.Name ?? "???");
                }
                return true;
            }
            if (KnightmareUnicornSummonCheck(Card).Count >= 2)
            {
                Logger.DebugWriteLine("Summon for KnightmareUnicorn.");
                return true;
            }
            if (KnightmarePhoenixSummonCheck(Card).Count >= 2)
            {
                Logger.DebugWriteLine("Summon for KnightmarePhoenix.");
                return true;
            }
            if (RelinquishedAnimaSummonCheck(Card) != -1)
            {
                Logger.DebugWriteLine("Summon for RelinquishedAnima.");
                return true;
            }
            if (SalamangreatAlmirajSummonCheck(Card))
            {
                Logger.DebugWriteLine("Summon for SalamangreatAlmiraj.");
                return true;
            }

            summoned = false;
            return false;
        }

        /// <summary>
        /// Special Witchcraft from deck for all monsters, except spells/traps.
        /// </summary>
        /// <param name="level">max level can be special summoned.</param>
        public bool DeckSSWitchcraft()
        {
            if (Card.Location != CardLocation.MonsterZone) return false;
            if (Duel.LastChainPlayer == 0) return false;
            if (NegatedCheck(false)) return false;
            if (Duel.Player == 0 && !FirstCheckSS.Contains(Card.Id))
            {
                // activate when ask twice
                FirstCheckSS.Add(Card.Id);
                return false;
            }

            // get discardable count
            int discardable_hands = CheckDiscardableSpellCount();

            // not must SS
            if (discardable_hands == 0 && Bot.MonsterZone.GetFirstMatchingCard(card => card.HasSetcode(Witchcraft_setcode) && card.Level >= 6) != null)
            {
                return false;
            }

            SelectDiscardSpell();

            // check whether should call MadameVerre for destroying monster
            bool lesssummon = false;
            int extra_attack = CheckPlusAttackforMadameVerre(true, false, true);
            int best_power = Util.GetBestAttack(Bot);
            if (CheckRemainInDeck(CardId.Haine) > 0 && best_power < 2400) best_power = 2400;
            Logger.DebugWriteLine("less summon check: " + (best_power + extra_attack - 1000).ToString() + " to " + (best_power + extra_attack).ToString());
            if (Util.GetOneEnemyBetterThanValue(best_power) != null 
                && Util.GetOneEnemyBetterThanValue(best_power + extra_attack) == null
                && Util.GetOneEnemyBetterThanValue(best_power + extra_attack - 1000) != null)
            {
                lesssummon = true;
            }
            
            // SS lower 4
            if (!enemy_activate_MaxxC && !lesssummon && discardable_hands >= 2 && Duel.Player == 0)
            {
                int[] SS_priority = { CardId.Schmietta, CardId.Pittore, CardId.Genni, CardId.Potterie };
                foreach (int cardid in SS_priority)
                {
                    if (!UseSSEffect.Contains(cardid) && Card.Id != cardid && CheckRemainInDeck(cardid) > 0
                        && Bot.MonsterZone.GetFirstMatchingCard(card => card.Id == cardid && card.IsFaceup()) == null)
                    {
                        UseSSEffect.Add(Card.Id);
                        AI.SelectNextCard(cardid);
                        return true;
                    }
                }
            }

            // check whether continue to ss
            bool should_attack = Util.GetOneEnemyBetterThanValue(Card.Attack) == null;
            if ((should_attack ^ Card.IsDefense()) && Duel.Player == 1) return false;
            if (CheckRemainInDeck(CardId.Haine, CardId.MadameVerre, CardId.GolemAruru) == 0) return false;

            // SS higer level
            if (Bot.HasInMonstersZone(CardId.Haine) || (lesssummon && !Bot.HasInMonstersZone(CardId.MadameVerre, true)))
            {
                AI.SelectNextCard(CardId.MadameVerre, CardId.Haine, CardId.GolemAruru);
            }
            else
            {
                AI.SelectNextCard(CardId.Haine, CardId.MadameVerre, CardId.GolemAruru);
            }
            UseSSEffect.Add(Card.Id);
            return true;
        }

        // recycle witchcraft spells in grave
        public bool WitchcraftRecycle()
        {
            if (Card.IsSpell() && Card.HasSetcode(Witchcraft_setcode) && Card.Location == CardLocation.Grave) {
                ActivatedCards.Add(Card.Id);
                if (Card.HasType(CardType.Continuous))
                {
                    SelectSTPlace(Card);
                }
                return true;
            }
            return false;
        }

        // activate of GolemAruru
        public bool GolemAruruActivate()
        {
            if (ActivateDescription == Util.GetStringId(CardId.GolemAruru, 2))
            {
                return true;
            }
            if (NegatedCheck()) return false;
            ClientCard targetcard = CheckProblematicCards(true);
            if (targetcard != null)
            {
                AI.SelectCard(targetcard);
                return true;
            }
            AI.SelectCard(CardId.Holiday, CardId.Creation, CardId.Draping, CardId.Scroll, CardId.WitchcrafterBystreet, CardId.Unveiling, CardId.Collaboration );
            return true;
        }

        // activate of MadameVerre
        public bool MadameVerreActivate()
        {
            if (NegatedCheck(true)) return false;
            // negate
            if (ActivateDescription == Util.GetStringId(CardId.MadameVerre, 1))
            {
                if (Card.IsDisabled()) return false;
                if (CheckLastChainNegated()) return false;

                // negate before activate
                if (Enemy.MonsterZone.GetFirstMatchingCard(card => card.IsMonsterShouldBeDisabledBeforeItUseEffect() && !card.IsDisabled()) != null)
                {
                    SelectDiscardSpell();
                    return true;
                }

                // chain check
                ClientCard LastChainCard = Util.GetLastChainCard();
                if ((LastChainCard != null && LastChainCard.Controller == 1 && LastChainCard.Location == CardLocation.MonsterZone))
                {
                    // negate monsters' activate
                    SelectDiscardSpell();
                    return true;
                }

                // negate battle related effect
                if (Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2)
                {
                    if (Enemy.MonsterZone.GetFirstMatchingCard(card => 
                        card.IsMonsterDangerous() || (Duel.Player == 0) && card.IsMonsterInvincible()) != null)
                    {
                        SelectDiscardSpell();
                        return true;
                    }
                }

                return false;
            }
            // gain ATK
            else
            {
                ClientCard self_card = Bot.BattlingMonster;
                ClientCard enemy_card = Enemy.BattlingMonster;
                if (self_card != null && enemy_card != null)
                {
                    int power_cangain = CheckPlusAttackforMadameVerre();
                    int diff = enemy_card.GetDefensePower() - self_card.GetDefensePower();
                    Logger.DebugWriteLine("power: " + power_cangain.ToString());
                    Logger.DebugWriteLine("diff: " + diff.ToString());
                    if (diff > 0)
                    {
                        // avoid useless effect
                        if (self_card.IsDefense() && power_cangain < diff)
                        {
                            return false;
                        }
                        AI.SelectCard(Bot.Hand.GetMatchingCards(card => card.IsSpell()));
                        MadameVerreGainedATK = true;
                        return true;
                    }
                    else if (Enemy.GetMonsterCount() == 1 || (enemy_card.IsAttack() && Enemy.LifePoints <= diff + power_cangain))
                    {
                        AI.SelectCard(Bot.Hand.GetMatchingCards(card => card.IsSpell()));
                        MadameVerreGainedATK = true;
                        return true;
                    }
                }
            }
            return false;
        }

        // activate of Haine
        public bool HaineActivate()
        {
            if (NegatedCheck(true) || Duel.LastChainPlayer == 0) return false;
            // danger check
            ClientCard targetcard = Enemy.MonsterZone.GetFloodgate(true);
            if (targetcard == null)
            {
                Logger.DebugWriteLine("*** Haine 2nd check.");
                targetcard = Enemy.SpellZone.FirstOrDefault(card => card?.Data != null && card.IsFloodgate() && card.IsFaceup() && (!card.IsShouldNotBeTarget() || !Duel.ChainTargets.Contains(card)));
                // GetFloodgate(true);
            }
            if (targetcard == null)
            {
                Logger.DebugWriteLine("*** Haine 3rd check.");
                targetcard = CheckProblematicCards(true, (Duel.Phase <= DuelPhase.Main1 || Duel.Phase >= DuelPhase.Main2));
                if (targetcard != null && targetcard.HasSetcode(TimeLord_setcode) && !targetcard.IsDisabled()) targetcard = null;
            }
            if (targetcard == null && Duel.LastChainPlayer == 1)
            {
                Logger.DebugWriteLine("*** Haine 4th check.");
                ClientCard lastcard = Util.GetLastChainCard();
                if (lastcard != null && !lastcard.IsDisabled() && !CheckLastChainNegated()
                    && (lastcard.HasType(CardType.Continuous) || lastcard.HasType(CardType.Equip) || lastcard.HasType(CardType.Field))
                    && (lastcard.Location == CardLocation.SpellZone || lastcard.Location == CardLocation.FieldZone))
                {
                    targetcard = lastcard;
                }
            }
            if (targetcard != null)
            {
                Logger.DebugWriteLine("*** Haine target: "+ targetcard.Name ?? "???");
                SelectDiscardSpell();
                AI.SelectNextCard(targetcard);
                return true;
            }

            // pendulum check
            if (!CheckLastChainNegated())
            {
                ClientCard l = null;
                ClientCard r = null;
                if (Duel.IsNewRule || Duel.IsNewRule2020)
                {
                    l = Enemy.SpellZone[0];
                    r = Enemy.SpellZone[4];
                }
                else
                {
                    l = Enemy.SpellZone[6];
                    r = Enemy.SpellZone[7];
                }
                if (l != null && r != null && l.LScale != r.RScale)
                {
                    Logger.DebugWriteLine("*** Haine pendulum destroy");
                    SelectDiscardSpell();
                    AI.SelectNextCard(Program.Rand.Next(2) == 1 ? l : r);
                    return true;
                }
            }
            

            // end check
            if (Duel.Player == 0 && Duel.Phase == DuelPhase.End)
            {
                Logger.DebugWriteLine("*** Haine self check");
                int selected_cost = 0;
                // spare spell check
                int[] checklist = { CardId.Collaboration, CardId.Unveiling, CardId.Scroll, CardId.Holiday, CardId.Creation, CardId.Draping };
                foreach (int cardid in checklist)
                {
                    if (!ActivatedCards.Contains(cardid) && Bot.HasInHand(cardid))
                    {
                        selected_cost = cardid;
                        break;
                    }
                }

                if (selected_cost == 0) return false;
                IList<ClientCard> target_1 = Enemy.SpellZone.GetMatchingCards(card => card.IsFaceup());
                IList<ClientCard> target_2 = Enemy.MonsterZone.GetMatchingCards(card => card.IsFaceup());
                List<ClientCard> targets = target_1.Union(target_2).ToList();
                if (targets.Count == 0)
                {
                    return false;
                }
                // shuffle and select randomly
                targets = CardListShuffle(targets);
                AI.SelectCard(selected_cost);
                AI.SelectNextCard(targets);
                return true;
            }
            return false;
        }

        // activate of Schmietta
        public bool SchmiettaActivate()
        {
            if (Card.Location != CardLocation.Grave) return false;
            if (NegatedCheck(false) || CheckWhetherWillbeRemoved()) return false;
            // spell check
            bool can_recycle = Bot.MonsterZone.GetFirstMatchingCard(
                card => card.IsFaceup() && card.HasSetcode(Witchcraft_setcode) && card.Id != CardId.GolemAruru
                ) != null;
            if (can_recycle)
            {
                int[] spell_checklist = { CardId.WitchcrafterBystreet, CardId.Holiday, CardId.Creation, CardId.Draping, CardId.Scroll, CardId.Unveiling, CardId.Collaboration };
                foreach (int cardid in spell_checklist)
                {
                    if (CheckRemainInDeck(cardid) > 0 && !Bot.HasInHandOrInSpellZone(cardid) && !Bot.HasInGraveyard(cardid) && !ActivatedCards.Contains(cardid))
                    {
                        AI.SelectCard(cardid);
                        ActivatedCards.Add(CardId.Schmietta);
                        return true;
                    }
                }
            }

            bool can_find_Holiday = Bot.HasInHandOrInSpellZone(CardId.Holiday) || (can_recycle && Bot.HasInGraveyard(CardId.Holiday) && !(ActivatedCards.Contains(CardId.Holiday)));
            // monster check
            if (Bot.HasInHand(important_witchcraft)  && !Bot.HasInGraveyard(CardId.Pittore) 
                && !ActivatedCards.Contains(CardId.Pittore) && CheckRemainInDeck(CardId.Pittore) > 0 && can_find_Holiday){
                AI.SelectCard(CardId.Pittore);
                ActivatedCards.Add(CardId.Schmietta);
                return true;
            }

            // ss check
            if (Bot.HasInHand(CardId.Holiday) && !ActivatedCards.Contains(CardId.Holiday) && !Bot.HasInGraveyard(important_witchcraft))
            {
                AI.SelectCard(important_witchcraft);
                ActivatedCards.Add(CardId.Schmietta);
                return true;
            }

            // copy check
            if (!ActivatedCards.Contains(CardId.Genni))
            {
                int has_Genni = Bot.HasInGraveyard(CardId.Genni) ? 1 : 0;
                int has_Holiday = Bot.HasInGraveyard(CardId.Holiday) ? 1 : 0;
                int has_important = Bot.HasInGraveyard(important_witchcraft) ? 1 : 0;
                // lack one of them
                if (has_Genni + has_Holiday + has_important == 2)
                {
                    if (has_Genni == 0)
                    {
                        AI.SelectCard(CardId.Genni);
                        ActivatedCards.Add(CardId.Schmietta);
                        return true;
                    }
                    if (has_Holiday == 0)
                    {
                        AI.SelectCard(CardId.Holiday);
                        ActivatedCards.Add(CardId.Schmietta);
                        return true;
                    }
                    if (has_important == 0)
                    {
                        AI.SelectCard(important_witchcraft);
                        ActivatedCards.Add(CardId.Schmietta);
                        return true;
                    }
                }
            }

            // Pittore check
            if (!ActivatedCards.Contains(CardId.Pittore) && !Bot.HasInGraveyard(CardId.Pittore))
            {
                if (PittoreActivate())
                {
                    AI.SelectCard(CardId.Pittore);
                    ActivatedCards.Add(CardId.Schmietta);
                    return true;
                }
            }

            // trap check
            if (CheckRemainInDeck(CardId.Masterpiece) >= 2){
                AI.SelectCard(CardId.Masterpiece);
                ActivatedCards.Add(CardId.Schmietta);
                return true;
            }

            return false;
        }

        // activate of Pittore
        public bool PittoreActivate()
        {
            if (Card.Location != CardLocation.Grave) return false;
            if (NegatedCheck(false) || CheckWhetherWillbeRemoved()) return false;
            if (Bot.Hand.GetFirstMatchingCard(card => card.HasSetcode(Witchcraft_setcode)) == null) return false;

            // discard advance
            if (Bot.Hand.GetFirstMatchingCard(card => card.Id == CardId.MadameVerre || card.Id == CardId.Haine) != null)
            {
                AI.SelectCard(CardId.MadameVerre, CardId.Haine);
                ActivatedCards.Add(CardId.Pittore);
                return true;
            }

            // spell check
            int[] spell_checklist = { CardId.Scroll, CardId.Unveiling, CardId.Collaboration, CardId.Draping, CardId.WitchcrafterBystreet, CardId.Holiday, CardId.Creation };
            foreach (int cardid in spell_checklist)
            {
                if (Bot.HasInHand(cardid) && !ActivatedCards.Contains(cardid)){
                    AI.SelectCard(cardid);
                    ActivatedCards.Add(CardId.Pittore);
                    return true;
                }
            }

            // monster check
            if ((Bot.HasInHand(CardId.Schmietta) && !ActivatedCards.Contains(CardId.Schmietta))
                ||Bot.Hand.GetMatchingCardsCount(card => card.HasSetcode(Witchcraft_setcode) && card.Level <= 4) >= 2){
                int[] monster_checklist = { CardId.Schmietta, CardId.Pittore, CardId.Genni, CardId.Potterie};
                foreach (int cardid in spell_checklist)
                {
                    if (Bot.HasInHand(cardid)){
                        AI.SelectCard(cardid);
                        ActivatedCards.Add(CardId.Pittore);
                        return true;
                    }
                }
            }

            return false;
        }

        // activate of AshBlossom_JoyousSpring
        public bool AshBlossom_JoyousSpringActivate()
        {
            if (NegatedCheck(true) || CheckLastChainNegated()) return false;
            return DefaultAshBlossomAndJoyousSpring();
        }

        // activate of PSYGamma
        public bool PSYGammaActivate()
        {
            if (NegatedCheck(true)) return false;
            return true;
        }

        // activate of MaxxC
        public bool MaxxCActivate()
        {
            if (NegatedCheck(true)) return false;
            return DefaultMaxxC();
        }

        // activate of Potterie
        public bool PotterieActivate()
        {
            if (Card.Location != CardLocation.Grave) return false;
            if (NegatedCheck(true)) return false;

            // Holiday check
            if (!ActivatedCards.Contains(CardId.Holiday) && Bot.HasInGraveyard(CardId.Holiday)){
                if (Bot.HasInGraveyard(important_witchcraft)){
                    AI.SelectCard(CardId.Holiday);
                    ActivatedCards.Add(CardId.Potterie);
                    return true;
                }
            }
            
            // safe check
            if (CheckProblematicCards() == null){
                int[] checklist = {CardId.Patronus, CardId.GolemAruru};
                foreach (int cardid in checklist){
                    if (Bot.HasInGraveyard(cardid)){
                        AI.SelectCard(cardid);
                        ActivatedCards.Add(CardId.Potterie);
                        return true;
                    }
                }
            }
            return false;
        }

        // activate of Genni
        public bool GenniActivate()
        {
            if (Card.Location != CardLocation.Grave) return false;
            if (NegatedCheck(true)) return false;

            // Holiday check
            int HolidayCount = Bot.Graveyard.GetMatchingCardsCount(card => card.Id == CardId.Holiday);
            int SS_id = HolidayCheck(Card);
            if (HolidayCount > 0 && SS_id > 0){
                AI.SelectCard(CardId.Holiday);
                AI.SelectNextCard(SS_id);
                ActivatedCards.Add(CardId.Genni);
                return true;
            }

            // Draping check
            if (Bot.HasInGraveyard(CardId.Draping)){
                if (Enemy.GetMonsterCount() == 0 && Duel.Phase == DuelPhase.Main1){
                    int total_attack = 0;
                    foreach (ClientCard card in Bot.GetMonsters()){
                        total_attack += card.Attack;
                    }
                    // otk confirm
                    if (total_attack >= Enemy.LifePoints){
                        int bot_count = Bot.MonsterZone.GetMatchingCardsCount(card => card.IsFaceup() && card.HasSetcode(Witchcraft_setcode));
                        IList<ClientCard> enemy_cards = Enemy.GetSpells();
                        if (bot_count >= enemy_cards.Count()){
                            AI.SelectCard(CardId.Draping);
                            AI.SelectNextCard(enemy_cards);
                            ActivatedCards.Add(CardId.Genni);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        // activate of Collaboration
        public bool CollaborationActivate()
        {
            if (Card.Location == CardLocation.Grave) return false;
            if (NegatedCheck(true)) return false;
            ClientCard target = Util.GetBestBotMonster(true);
            if (Util.GetOneEnemyBetterThanMyBest() == null){
                if (Enemy.SpellZone.GetFirstMatchingCard(card => card.IsFacedown()) != null
                    || Enemy.MonsterZone.GetMatchingCardsCount(card => card.GetDefensePower() < target.Attack) >= 2){
                    AI.SelectCard(target);
                    SelectSTPlace(null, true);
                    ActivatedCards.Add(CardId.Collaboration);
                    return true;
                }
            }
            return false;
        }

        // activate of LightningStorm
        public bool LightningStormActivate()
        {
            int bestPower = 0;
            foreach (ClientCard hand in Bot.Hand)
            {
                if (hand.IsMonster() && hand.Level <= 4 && hand.Attack > bestPower) bestPower = hand.Attack;
            }

            int opt = -1;
            // destroy monster
            if (Enemy.MonsterZone.GetFirstMatchingCard(card => card.IsFloodgate() && card.IsAttack()) != null
                || Enemy.MonsterZone.GetMatchingCardsCount(card => card.IsAttack() && card.Attack >= bestPower) >= 2) opt = 0;
            // destroy spell/trap
            else if (Enemy.GetSpellCount() >= 2 || Util.GetProblematicEnemySpell() != null) opt = 1;

            if (opt == -1) return false;

            // only one selection
            if (Enemy.MonsterZone.GetFirstMatchingCard(card => card.IsAttack()) == null 
                || Enemy.GetSpellCount() == 0)
            {
                AI.SelectOption(0);
                SelectSTPlace(null, true);
                return true;
            }
            AI.SelectOption(opt);
            SelectSTPlace(null, true);
            return true;
        }

        // activate of PotofExtravagance
        public bool PotofExtravaganceActivate()
        {
            // won't activate if it'll be negate
            if (SpellNegatable()) return false;

            SelectSTPlace(Card, true);
            AI.SelectOption(1);
            return true;
        }

        // activate of DarkRulerNoMore
        public bool DarkRulerNoMoreActivate()
        {
            if (SpellNegatable()) return false;
            if (Enemy.MonsterZone.GetFirstMatchingCard(card => card.IsFloodgate() && !card.IsDisabled()) != null)
            {
                SelectSTPlace(null, true);
                return true;
            }
            return false;
        }

        // activate of Creation
        public bool CreationActivate()
        {
            if (Card.Location == CardLocation.Grave) return false;
            if (NegatedCheck(true)) return false;

            // discard cost ensure
            int least_cost = (Bot.HasInMonstersZone(CardId.Haine) ? 1 : 0) + (Bot.HasInMonstersZone(CardId.MadameVerre) ? 1 : 0);
            int discardable = Bot.Hand.GetMatchingCardsCount(card => card != Card && card.IsSpell()) + CheckRecyclableCount() -1;
            if (discardable < least_cost) return false;

            // search monster to summon
            bool need_lower = (!summoned || (
                Bot.MonsterZone.GetFirstMatchingCard(card => card.HasSetcode(Witchcraft_setcode)) == null
                && Bot.Hand.GetFirstMatchingCard(card => card.IsMonster() && card.HasSetcode(Witchcraft_setcode) && card.Level <= 4) == null));
            if (need_lower)
            {
                AI.SelectCard(CardId.Schmietta, CardId.Pittore, CardId.Genni, CardId.Potterie, CardId.GolemAruru);
                SelectSTPlace(null, true);
                ActivatedCards.Add(CardId.Creation);
                return true;
            }
            // search GolemAruru
            else
            {
                if (Bot.HasInHand(CardId.GolemAruru)) return false;
                if (Bot.MonsterZone.GetFirstMatchingCard(card => card.IsFaceup() && card.HasSetcode(Witchcraft_setcode)) == null)
                {
                    AI.SelectCard(CardId.GolemAruru, CardId.Schmietta, CardId.Pittore, CardId.Genni, CardId.Potterie);
                    SelectSTPlace(null, true);
                    ActivatedCards.Add(CardId.Creation);
                    return true;
                } else
                {
                    AI.SelectCard(CardId.Schmietta, CardId.Pittore, CardId.Genni, CardId.Potterie, CardId.GolemAruru);
                    SelectSTPlace(null, true);
                    ActivatedCards.Add(CardId.Creation);
                    return true;
                }
            }
        }

        /// <summary>
        /// Check Holiday's target. If nothing should be SS, return 0.
        /// </summary>
        /// <param name="except_card"></param>
        /// <returns></returns>
        public int HolidayCheck(ClientCard except_card = null){
            // SS important first
            List<int> check_list = new List<int> { CardId.Haine, CardId.MadameVerre, CardId.GolemAruru};
            foreach (int cardid in check_list)
            {
                if (Bot.HasInGraveyard(cardid) && Bot.MonsterZone.GetFirstMatchingCard(card => card.IsFaceup() && card.Id == cardid) == null)
                {
                    Logger.DebugWriteLine("*** Holiday check 1st: " + cardid.ToString());
                    return cardid;
                }
            }
            check_list.Clear();
            if (CheckProblematicCards() == null)
            {
                if (Bot.HasInGraveyard(CardId.GolemAruru) && Bot.MonsterZone.GetFirstMatchingCard(card => card.IsFaceup() && card.HasSetcode(Witchcraft_setcode)) != null)
                {
                    Logger.DebugWriteLine("*** Holiday check 2nd: GolemAruru");
                    return CardId.GolemAruru;
                }
                check_list.Add(CardId.Schmietta);
                check_list.Add(CardId.Pittore);
                check_list.Add(CardId.Genni);
                check_list.Add(CardId.Potterie);
                foreach (int cardid in check_list)
                {
                    if (!UseSSEffect.Contains(cardid) && Bot.Graveyard.GetFirstMatchingCard(card => card.Id == cardid && card != except_card) != null && CheckDiscardableSpellCount(Card) > 0)
                    {
                        Logger.DebugWriteLine("*** Holiday check 3rd: " + cardid.ToString());
                        return cardid;
                    }
                }
            }
            else
            {
                check_list.Add(CardId.Haine);
                check_list.Add(CardId.MadameVerre);
                check_list.Add(CardId.GolemAruru);
                foreach (int cardid in check_list)
                {
                    if (Bot.Graveyard.GetFirstMatchingCard(card => card.Id == cardid && card != except_card) != null)
                    {
                        return cardid;
                    }
                }
            }
            return 0;
        }

        // activate of Holiday
        public bool HolidayActivate()
        {
            if (Card.Location == CardLocation.Grave) return false;
            if (NegatedCheck(true)) return false;
            int target = HolidayCheck();
            if (target != 0)
            {
                AI.SelectCard(target);
                SelectSTPlace(null, true);
                ActivatedCards.Add(CardId.Holiday);
                return true;
            }
            return false;
        }

        // activate of CalledbytheGrave
        public bool CalledbytheGraveActivate()
        {
            if (NegatedCheck(true)) return false;
            if (Duel.LastChainPlayer == 1)
            {
                // negate
                if (Util.GetLastChainCard().IsMonster())
                {
                    int code = Util.GetLastChainCard().GetOriginCode();
                    if (code == 0) return false;
                    if (CheckCalledbytheGrave(code) > 0) return false;
                    ClientCard target = Enemy.Graveyard.GetFirstMatchingCard(card => card.IsMonster() && card.IsOriginalCode(code));
                    if (target != null)
                    {
                        if (!(Card.Location == CardLocation.SpellZone))
                        {
                            SelectSTPlace(null, true);
                        }
                        AI.SelectCard(target);
                        currentNegatingIdList.Add(code);
                        return true;
                    }
                }
                
                // banish target
                foreach (ClientCard cards in Enemy.Graveyard)
                {
                    if (Duel.ChainTargets.Contains(cards))
                    {
                        int code = cards.GetOriginCode();
                        AI.SelectCard(cards);
                        currentNegatingIdList.Add(code);
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
                        int code = enemy_monsters[0].GetOriginCode();
                        AI.SelectCard(enemy_monsters);
                        currentNegatingIdList.Add(code);
                        return true;
                    }
                }
            }

            // avoid danger monster in grave
            if (Duel.LastChainPlayer == 1) return false;
            List<ClientCard> targets = CheckDangerousCardinEnemyGrave(true);
            if (targets.Count() > 0) {
                int code = targets[0].GetOriginCode();
                if (!(Card.Location == CardLocation.SpellZone))
                {
                    SelectSTPlace(null, true);
                }
                AI.SelectCard(targets);
                currentNegatingIdList.Add(code);
                return true;
            }

            return false;
        }

        // activate of Draping
        public bool DrapingActivate()
        {
            if (Card.Location == CardLocation.Grave) return false;
            if (NegatedCheck(true)) return false;

            IList<ClientCard> dangerours_spells = Enemy.SpellZone.GetMatchingCards(card => card.IsFloodgate() && !card.IsDisabled() && card.IsSpell());
            IList<ClientCard> dangerours_traps = Enemy.SpellZone.GetMatchingCards(card => card.IsFloodgate() && !card.IsDisabled() && card.IsTrap());
            List<ClientCard> faceup_spells = CardListShuffle(Enemy.SpellZone.GetMatchingCards(card => card.IsFaceup() && card.IsSpell()).ToList());
            List<ClientCard> faceup_traps = CardListShuffle(Enemy.SpellZone.GetMatchingCards(card => card.IsFaceup() && card.IsTrap()).ToList());
            List<ClientCard> setcards = CardListShuffle(Enemy.SpellZone.GetMatchingCards(card => card.IsFacedown()).ToList());
            if (Duel.Player == 0 || Duel.Phase == DuelPhase.End)
            {
                IList<ClientCard> targets_1 = dangerours_spells.Union(dangerours_traps).Union(faceup_spells).Union(faceup_traps).Union(setcards).ToList();
                if (targets_1.Count() == 0) return false;
                AI.SelectCard(targets_1);
                SelectSTPlace(null, true);
                ActivatedCards.Add(CardId.Draping);
                return true;
            }
            IList<ClientCard> targets_2 = dangerours_traps.Union(faceup_traps).ToList();
            if (targets_2.Count() == 0) return false;
            targets_2 = targets_2.Union(dangerours_spells).Union(faceup_spells).Union(setcards).ToList();
            AI.SelectCard(targets_2);
            SelectSTPlace(null, true);
            ActivatedCards.Add(CardId.Draping);
            return true;
        }

        // activate of CrossoutDesignator
        public bool CrossoutDesignatorActivate()
        {
            if (NegatedCheck(true) || CheckLastChainNegated()) return false;
            // negate 
            if (Duel.LastChainPlayer == 1 && Util.GetLastChainCard() != null)
            {
                int code = Util.GetLastChainCard().GetOriginCode();
                if (code == 0) return false;
                if (CheckCalledbytheGrave(code) > 0) return false;
                if (CheckRemainInDeck(code) > 0)
                {
                    if (!(Card.Location == CardLocation.SpellZone))
                    {
                        SelectSTPlace(null, true);
                    }
                    AI.SelectAnnounceID(code);
                    currentNegatingIdList.Add(code);
                    return true;
                }
            }
            return false;
        }

        // activate of Unveiling
        public bool UnveilingActivate()
        {
            if (Card.Location == CardLocation.Grave) return false;
            if (NegatedCheck(true)) return false;

            // LightningStorm check
            if (Bot.HasInHandOrInSpellZone(_CardId.LightningStorm))
            {
                int faceup_count = Bot.SpellZone.GetMatchingCardsCount(card => card.IsFaceup());
                faceup_count += Bot.MonsterZone.GetMatchingCardsCount(card => card.IsFaceup());
                if (faceup_count == 0 && LightningStormActivate())
                {
                    return false;
                }
            }

            if (Bot.HasInHand(important_witchcraft))
            {
                AI.SelectCard(important_witchcraft);
                SelectSTPlace(null, true);
                ActivatedCards.Add(CardId.Unveiling);
                return true;
            }
            return false;
        }

        // activate of Scroll
        public bool ScrollActivate()
        {
            if (SpellNegatable() || Card.Location == CardLocation.Grave || Duel.Phase == DuelPhase.Main2)
            {
                return false;
            }
            if (Bot.MonsterZone.GetFirstMatchingCard(card => card.HasRace(CardRace.SpellCaster)) == null)
            {
                return false;
            }
            SelectSTPlace(null, true);
            return true;
        }

        // activate of MagiciansRestage
        public bool MagiciansRestageActivate()
        {
            // search
            if (Card.Location == CardLocation.Grave)
            {
                if (Enemy.SpellZone.GetFirstMatchingCard(card => card.IsFacedown()) != null)
                {
                    AI.SelectCard(CardId.MagiciansLeftHand, CardId.MagicianRightHand);
                }
                else
                {
                    AI.SelectCard(CardId.MagicianRightHand, CardId.MagiciansLeftHand);
                }
                return true;
            }

            if (SpellNegatable())
            {
                return false;
            }

            // find target
            if (CheckDiscardableSpellCount(Card) < 1) return false;
            int target = 0;
            int[] target_list = { CardId.Genni, CardId.Pittore, CardId.Potterie };
            foreach (int cardid in target_list)
            {
                if (!UseSSEffect.Contains(cardid) && Bot.HasInGraveyard(cardid))
                {
                    target = cardid;
                    break;
                }
            }
            if (target == 0) return false;
            if (Card.Location == CardLocation.Hand)
            {
                SelectSTPlace(null, true);
                return true;
            }
            AI.SelectCard(target);
            return true;
        }

        // activate of WitchcrafterBystreet
        public bool WitchcrafterBystreetActivate()
        {
            if (SpellNegatable() || Card.Location == CardLocation.Grave)
            {
                return false;
            }
            if (Bot.HasInSpellZone(CardId.WitchcrafterBystreet, true) || Bot.MonsterZone.GetFirstMatchingCard(card => card.HasSetcode(Witchcraft_setcode) && card.IsFaceup()) == null)
            {
                return false;
            }
            SelectSTPlace(null, true);
            return true;
        }

        // activate of Impermanence
        public bool InfiniteImpermanenceActivate()
        {
            if (SpellNegatable()) return false;
            if (CheckLastChainNegated()) return false;
            // negate before monster's effect's used
            foreach (ClientCard m in Enemy.GetMonsters())
            {
                if (!m.IsDisabled() && Duel.LastChainPlayer != 0 && 
                    ((m.IsMonsterShouldBeDisabledBeforeItUseEffect() || m.IsFloodgate())
                    || (Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2 && 
                        (m.IsMonsterDangerous() || m.IsMonsterInvincible() 
                        || (m.IsMonsterHasPreventActivationEffectInBattle() && Bot.HasInMonstersZone(CardId.MadameVerre)))
                     )))
                {
                    if (Card.Location == CardLocation.SpellZone)
                    {
                        for (int i = 0; i < 5; ++i)
                        {
                            if (Bot.SpellZone[i] == Card)
                            {
                                Impermanence_list.Add(i);
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
                if ((this_seq * that_seq >= 0 && this_seq + that_seq == 4)
                    || (Util.IsChainTarget(Card))
                    || (LastChainCard != null && LastChainCard.Controller == 1 && LastChainCard.IsCode(_CardId.HarpiesFeatherDuster)))
                {
                    List<ClientCard> enemy_monsters = Enemy.GetMonsters();
                    enemy_monsters.Sort(CardContainer.CompareCardAttack);
                    enemy_monsters.Reverse();
                    foreach (ClientCard card in enemy_monsters)
                    {
                        if (card.IsFaceup() && !card.IsShouldNotBeTarget() && !card.IsShouldNotBeSpellTrapTarget())
                        {
                            AI.SelectCard(card);
                            Impermanence_list.Add(this_seq);
                            return true;
                        }
                    }
                }
            }

            // negate monsters
            if ((LastChainCard == null || LastChainCard.Controller != 1 || LastChainCard.Location != CardLocation.MonsterZone
                || CheckLastChainNegated() || LastChainCard.IsShouldNotBeTarget() || LastChainCard.IsShouldNotBeSpellTrapTarget()))
                return false;
            if (Card.Location == CardLocation.SpellZone)
            {
                for (int i = 0; i < 5; ++i)
                {
                    if (Bot.SpellZone[i] == Card)
                    {
                        Impermanence_list.Add(i);
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

        // activate of Masterpiece
        public bool MasterpieceActivate()
        {
            // search effect
            if (Card.Location == CardLocation.SpellZone)
            {
                if (NegatedCheck(true)) return false;
                // select randomly (TODO)
                IList<ClientCard> target_1 = Bot.Graveyard.GetMatchingCards(card => card.IsSpell() && CheckRemainInDeck(card.Id) > 0);
                IList<ClientCard> target_2 = Enemy.Graveyard.GetMatchingCards(card => card.IsSpell() && CheckRemainInDeck(card.Id) > 0);
                List<ClientCard> targets = CardListShuffle(target_1.Union(target_2).ToList());
                AI.SelectCard(targets);
                return true;
            }
            else
            // ss effect
            {
                // LightningStorm check
                if (Bot.HasInHandOrInSpellZone(_CardId.LightningStorm))
                {
                    int faceup_count = Bot.SpellZone.GetMatchingCardsCount(card => card.IsFaceup());
                    faceup_count += Bot.MonsterZone.GetMatchingCardsCount(card => card.IsFaceup());
                    if (faceup_count == 0 && LightningStormActivate())
                    {
                        return false;
                    }
                }

                List<ClientCard> tobanish_spells = CardListShuffle(Bot.Graveyard.GetMatchingCards(card => card.IsSpell() && !card.HasSetcode(Witchcraft_setcode) && card.Id != CardId.MetalfoesFusion).ToList());
                if (Bot.HasInGraveyard(CardId.Patronus))
                {
                    List<ClientCard> witchcraft_spells = CardListShuffle(Bot.Graveyard.GetMatchingCards(card => card.IsSpell() && card.HasSetcode(Witchcraft_setcode)).ToList());
                    tobanish_spells = witchcraft_spells.Union(tobanish_spells).ToList();
                }
                int max_level = tobanish_spells.Count();

                //check discardable count
                int discardable_hands = CheckDiscardableSpellCount();
                List<int> will_discard_list = new List<int> { CardId.Haine, CardId.MadameVerre, CardId.Schmietta, CardId.Pittore, CardId.Potterie, CardId.Genni };
                foreach (int cardid in will_discard_list)
                {
                    if (Bot.HasInMonstersZone(cardid))
                    {
                        discardable_hands--;
                    }
                }

                // SS lower 4
                if (discardable_hands >= 1 && Duel.Player == 0)
                {
                    int[] SS_priority = { CardId.Schmietta, CardId.Pittore, CardId.Genni, CardId.Potterie };
                    foreach (int cardid in SS_priority)
                    {
                        int level = witchcraft_level[cardid];
                        if (!UseSSEffect.Contains(cardid) & CheckRemainInDeck(cardid) > 0 && level <= max_level)
                        {
                            AI.SelectNumber(level);
                            AI.SelectCard(tobanish_spells);
                            AI.SelectNextCard(cardid);
                            return true;
                        }
                    }
                }

                // SS higer level
                List<int> ss_priority = new List<int>();
                if (Bot.HasInMonstersZone(CardId.Haine))
                {
                    ss_priority.Add(CardId.MadameVerre);
                    ss_priority.Add(CardId.Haine);
                }
                else
                {
                    ss_priority.Add(CardId.Haine);
                    ss_priority.Add(CardId.MadameVerre);
                }
                ss_priority.Add(CardId.GolemAruru);
                foreach (int cardid in ss_priority)
                {
                    int level = witchcraft_level[cardid];
                    if (CheckRemainInDeck(cardid) > 0 && level <= max_level)
                    {
                        AI.SelectNumber(level);
                        AI.SelectCard(tobanish_spells);
                        AI.SelectNextCard(cardid);
                        return true;
                    }
                }

            }
            return false;
        }

        // activate of Patronus
        public bool PatronusActivate()
        {
            // activate immediately
            if (ActivateDescription == 94)
            {
                return true;
            }
            // search
            if (Card.Location == CardLocation.SpellZone)
            {
                if (NegatedCheck(true) || Duel.LastChainPlayer == 0) return false;
                // find lack of spells
                int lack_spells = 0;
                int[] spell_checklist = { CardId.WitchcrafterBystreet, CardId.Holiday, CardId.Creation, CardId.Draping, CardId.Scroll, CardId.Unveiling, CardId.Collaboration };
                foreach (int cardid in spell_checklist)
                {
                    if (!Bot.HasInHandOrInSpellZone(cardid) && !Bot.HasInGraveyard(cardid))
                    {
                        lack_spells = cardid;
                        break;
                    }
                }

                // banish check
                List<int> banish_checklist = new List<int>{ CardId.Haine, CardId.MadameVerre, CardId.GolemAruru, CardId.Schmietta, CardId.Pittore};
                if (lack_spells == 0)
                {
                    List<int> new_list = new List<int> { CardId.Pittore, CardId.Genni, CardId.Schmietta, CardId.Potterie };
                    banish_checklist = banish_checklist.Union(new_list).ToList();
                }
                else
                {
                    List<int> new_list = new List<int> { CardId.Schmietta, CardId.Pittore, CardId.Genni, CardId.Potterie };
                    banish_checklist = banish_checklist.Union(new_list).ToList();
                }
                foreach(int cardid in banish_checklist)
                {
                    ClientCard target = Bot.Banished.GetFirstMatchingCard(card => card.Id == cardid);
                    if (target != null)
                    {
                        AI.SelectCard(target);
                        AI.SelectNextCard(lack_spells);
                        return true;
                    }
                }
            }

            // recycle
            if (Card.Location == CardLocation.Grave)
            {
                if (Bot.HasInHandOrInSpellZoneOrInGraveyard(CardId.Masterpiece))
                {
                    return false;
                }
                IList<ClientCard> targets = Bot.Banished.GetMatchingCards(card => card.IsSpell() && card.HasSetcode(Witchcraft_setcode));
                AI.SelectCard(targets);
                return true;
            }
            return false;
        }

        // summmon process of Level 8 Synchro Monster
        public bool Lv8Summon()
        {
            if (Bot.HasInMonstersZone(CardId.PSYGamma) && Bot.HasInMonstersZone(CardId.PSYDriver))
            {
                List<int> targets = new List<int> { CardId.PSYDriver, CardId.PSYGamma };
                AI.SelectMaterials(targets);
                return true;
            }
            return false;
        }

        // summon process of BorreloadSavageDragon
        public bool BorreloadSavageDragonSummon()
        {
            if (Bot.Graveyard.GetFirstMatchingCard(card => card.HasType(CardType.Link)) == null)
            {
                return false;
            }
            return Lv8Summon();
        }

        // equip target comparer for BorreloadSavageDragon
        public static int BorreloadSavageDragonEquipCompare(ClientCard cardA, ClientCard cardB)
        {
            if (cardA.LinkCount > cardB.LinkCount)
                return -1;
            if (cardA.LinkCount < cardB.LinkCount)
                return -1;
            if (cardA.Attack > cardB.Attack)
                return 1;
            if (cardA.Attack < cardB.Attack)
                return -1;
            return 0;
        }

        // activate of BorreloadSavageDragon
        public bool BorreloadSavageDragonActivate()
        {
            // equip
            if (ActivateDescription == Util.GetStringId(CardId.BorreloadSavageDragon, 0))
            {
                List<ClientCard> links = Bot.Graveyard.GetMatchingCards(card => card.HasType(CardType.Link)).ToList();
                links.Sort(BorreloadSavageDragonEquipCompare);
                AI.SelectCard(links);
                return true;
            }
            // negate
            if (NegatedCheck(true) || Duel.LastChainPlayer != 1) return false;
            if (Util.GetLastChainCard().HasSetcode(0x11e) && Util.GetLastChainCard().Location == CardLocation.Hand) return false;
            return false;
        }

        // activate of DracoBerserkeroftheTenyi(TODO)
        public bool DracoBerserkeroftheTenyiActivate()
        {
            Logger.DebugWriteLine("DracoBerserkeroftheTenyi's Effect: " + ActivateDescription.ToString());
            return true;
        }

        // activate of PSYOmega
        public bool PSYOmegaActivate()
        {
            // recycle
            if (Duel.Phase == DuelPhase.Standby)
            {
                if (Bot.Banished.Count == 0)
                {
                    return false;
                }
                List<ClientCard> targets = CardListShuffle(Bot.Banished.GetMatchingCards(card => card.HasSetcode(Witchcraft_setcode)).ToList());
                AI.SelectCard(targets);
                return true;
            }
            // banish hands
            if (Card.Location == CardLocation.MonsterZone)
            {
                if (Duel.Player == 1 || Bot.HasInMonstersZone(CardId.PSYLambda) || (Util.IsChainTarget(Card)) )
                {
                    return true;
                } else
                {
                    return Util.IsAllEnemyBetterThanValue(Card.Attack, true);
                }
            }
            // recycle from grave
            if (Card.Location == CardLocation.Grave)
            {
                List<ClientCard> enemy_danger = CheckDangerousCardinEnemyGrave();
                if (enemy_danger.Count > 0)
                {
                    AI.SelectCard(enemy_danger);
                    return true;
                }
                if (!Bot.HasInHandOrInSpellZoneOrInGraveyard(CardId.Holiday) && Bot.HasInGraveyard(important_witchcraft))
                {
                    AI.SelectCard(important_witchcraft);
                    return true;
                }
                if (CheckProblematicCards() == null)
                {
                    AI.SelectCard(_CardId.CalledByTheGrave, _CardId.CrossoutDesignator,
                        _CardId.MaxxC, _CardId.AshBlossom,
                        CardId.MagicianRightHand, CardId.MagiciansLeftHand, CardId.MagiciansRestage, CardId.Patronus, 
                        _CardId.LightningStorm, CardId.Reasoning);
                    return true;
                }
            }
            return false;
        }

        // activate of TGWonderMagician
        public bool TGWonderMagicianActivate()
        {
            if (Card.Location != CardLocation.MonsterZone) return true;
            Logger.DebugWriteLine("TGWonderMagician: " + ActivateDescription.ToString());
            List<ClientCard> problem_cards = Enemy.SpellZone.GetMatchingCards(card => card.IsFloodgate()).ToList();
            List<ClientCard> faceup_cards = Enemy.SpellZone.GetMatchingCards(card => card.IsFaceup()).ToList();
            List<ClientCard> facedown_cards = Enemy.SpellZone.GetMatchingCards(card => card.IsFacedown()).ToList();
            List<ClientCard> result = problem_cards.Union(faceup_cards).ToList().Union(facedown_cards).ToList();
            AI.SelectCard(result);
            return true;
        }

        // check whether summon BorrelswordDragon
        public List<ClientCard> BorrelswordDragonSummonCheck(ClientCard included = null)
        {
            List<ClientCard> empty_list = new List<ClientCard>();
            List<ClientCard> extra_list = new List<ClientCard>();
            if (included != null) extra_list.Add(included);
            List<ClientCard> materials = CheckLinkMaterials(4, 3, false, extra_list);
            if (materials.Count < 3) return empty_list;

            // need BorrelswordDragon?
            // for problem monster
            ClientCard flag = Util.GetOneEnemyBetterThanMyBest();
            if (flag != null)
            {
                return materials;
            }
            // for higher attack
            int total_attack = 0;
            foreach (ClientCard card in materials)
            {
                total_attack += card.Attack;
            }
            if (total_attack >= 3000) return empty_list;

            return materials;
        }

        // summon process of BorrelswordDragon
        public bool BorrelswordDragonSummon()
        {
            List<ClientCard> materials = BorrelswordDragonSummonCheck();
            if (materials.Count < 3) return false;
            AI.SelectMaterials(materials);
            return true;
        }

        // activate of BorrelswordDragon
        public bool BorrelswordDragonActivate()
        {
            if (ActivateDescription == -1)
            {
                ClientCard enemy_monster = Enemy.BattlingMonster;
                if (enemy_monster != null && enemy_monster.HasPosition(CardPosition.Attack))
                {
                    return (Card.Attack - enemy_monster.Attack < Enemy.LifePoints);
                }
                return true;
            };
            ClientCard BestEnemy = Util.GetBestEnemyMonster(true);
            ClientCard WorstBot = Bot.GetMonsters().GetLowestAttackMonster();
            if (BestEnemy == null || BestEnemy.HasPosition(CardPosition.FaceDown)) return false;
            if (WorstBot == null || WorstBot.HasPosition(CardPosition.FaceDown)) return false;
            if (BestEnemy.Attack >= WorstBot.RealPower)
            {
                AI.SelectCard(BestEnemy);
                return true;
            }
            return false;
        }

        // check whether summon KnightmareUnicorn
        public List<ClientCard> KnightmareUnicornSummonCheck(ClientCard included = null)
        {
            List<ClientCard> empty_list = new List<ClientCard>();
            List<ClientCard> extra_list = new List<ClientCard>();
            if (included != null) extra_list.Add(included);
            List<ClientCard> materials = CheckLinkMaterials(3, 2, false, extra_list);
            if (materials.Count < 2) return empty_list;

            // need KnightmareUnicorn?
            // for clear spells
            ClientCard flag = CheckProblematicCards(true, true);
            if (flag != null)
            {
                if (Bot.Hand.GetMatchingCardsCount(card => card != Card) == 0)
                {
                    return empty_list;
                }
                else
                {
                    return materials;
                }
            }
            // for higher attack
            int total_attack = 0;
            foreach(ClientCard card in materials)
            {
                total_attack += card.Attack;
            }
            if (total_attack >= 2200) return empty_list;

            return materials;
        }

        // summon process of KnightmareUnicorn
        public bool KnightmareUnicornSummon()
        {
            List<ClientCard> materials = KnightmareUnicornSummonCheck();
            if (materials.Count < 2) return false;
            AI.SelectMaterials(materials);
            return true;
        }

        // activate of KnightmareUnicorn
        public bool KnightmareUnicornActivate()
        {
            ClientCard card = CheckProblematicCards(true);
            if (card == null) return false;
            // avoid cards that cannot target.
            IList<ClientCard> enemy_list = new List<ClientCard>();
            if (!card.IsShouldNotBeMonsterTarget() && !card.IsShouldNotBeTarget()) enemy_list.Add(card);
            foreach (ClientCard enemy in Enemy.GetMonstersInExtraZone())
            {
                if (enemy != null && !enemy_list.Contains(enemy) && !enemy.IsShouldNotBeMonsterTarget() && !enemy.IsShouldNotBeTarget()) enemy_list.Add(enemy);
            }
            foreach (ClientCard enemy in Enemy.GetMonstersInMainZone())
            {
                if (enemy != null && !enemy_list.Contains(enemy) && !enemy.IsShouldNotBeMonsterTarget() && !enemy.IsShouldNotBeTarget()) enemy_list.Add(enemy);
            }
            foreach (ClientCard enemy in Enemy.GetSpells())
            {
                if (enemy != null && !enemy_list.Contains(enemy) && !enemy.IsShouldNotBeMonsterTarget() && !enemy.IsShouldNotBeTarget()) enemy_list.Add(enemy);
            }
            if (enemy_list.Count > 0)
            {
                SelectDiscardSpell();
                AI.SelectNextCard(enemy_list);
                return true;
            }
            return false;
        }
        
        // check whether summon KnightmarePhoenix
        public List<ClientCard> KnightmarePhoenixSummonCheck(ClientCard included = null)
        {
            List<ClientCard> empty_list = new List<ClientCard>();
            List<ClientCard> extra_list = new List<ClientCard>();
            if (included != null) extra_list.Add(included);
            List<ClientCard> materials = CheckLinkMaterials(2, 2, true, extra_list);
            if (materials.Count < 2) return empty_list;

            // need KnightmarePhoenix?
            // for clear spells
            ClientCard flag = Util.GetProblematicEnemySpell();
            if (flag != null)
            {
                if (Bot.Hand.GetMatchingCardsCount(card => card != Card) == 0)
                {
                    return empty_list;
                } else
                {
                    return materials;
                }
            }
            // for higher attack
            if (materials[0].Attack + materials[1].Attack >= 1900) return empty_list;

            return materials;
        }

        // summon process of KnightmarePhoenix
        public bool KnightmarePhoenixSummon()
        {
            List<ClientCard> materials = KnightmarePhoenixSummonCheck();
            if (materials.Count < 2) return false;
            AI.SelectMaterials(materials);
            return true;
        }

        // activate of KnightmarePhoenix
        public bool KnightmarePhoenixActivate()
        {
            List<ClientCard> targets = new List<ClientCard>();
            targets.Add(Util.GetProblematicEnemySpell());
            List<ClientCard> spells = Enemy.GetSpells();
            List<ClientCard> faceups = new List<ClientCard>();
            List<ClientCard> facedowns = new List<ClientCard>();
            CardListShuffle(spells);
            foreach (ClientCard card in spells)
            {
                if (card.HasPosition(CardPosition.FaceUp) && !(card.IsShouldNotBeTarget() || card.IsShouldNotBeMonsterTarget())) faceups.Add(card);
                else if (card.HasPosition(CardPosition.FaceDown)) facedowns.Add(card);
            }
            targets = targets.Union(faceups).Union(facedowns).ToList();
            if (targets.Count == 0) return false;
            SelectDiscardSpell();
            AI.SelectNextCard(targets);
            return true;
        }

        // check whether summon CrystronHalqifibrax
        public List<ClientCard> CrystronHalqifibraxSummonCheck(ClientCard included = null)
        {
            List<ClientCard> empty_list = new List<ClientCard>();
            List<ClientCard> extra_list = new List<ClientCard>();
            if (included != null) extra_list.Add(included);
            List<ClientCard> materials = CheckLinkMaterials(2, 2, true, extra_list);
            if (materials.Count < 2) return empty_list;

            // need CrystronHalqifibrax?
            if (CheckRemainInDeck(CardId.PSYGamma, _CardId.AshBlossom) == 0) return empty_list;


            return empty_list;
        }

        // summon process of CrystronHalqifibrax
        public bool CrystronHalqifibraxSummon()
        {
            List<ClientCard> materials = CrystronHalqifibraxSummonCheck();
            if (materials.Count < 2) return false;
            AI.SelectMaterials(materials);
            return true;
        }

        // activate of CrystronHalqifibrax
        public bool CrystronHalqifibraxActivate()
        {
            if (Duel.Player == 0)
            {
                return true;
            }
            else if (Util.IsChainTarget(Card) || Util.GetProblematicEnemySpell() != null) return true;
            else if (Duel.Player == 1 && Duel.Phase == DuelPhase.BattleStart && Util.IsOneEnemyBetterThanValue(1500, true))
            {
                if (Util.IsOneEnemyBetterThanValue(1900, true))
                {
                    AI.SelectPosition(CardPosition.FaceUpDefence);
                }
                else
                {
                    AI.SelectPosition(CardPosition.FaceUpAttack);
                }
                return true;
            }
            return false;
        }

        // check whether summon SalamangreatAlmiraj
        public bool SalamangreatAlmirajSummonCheck(ClientCard included = null)
        {
            // use witchcraft first
            if (CheckDiscardableSpellCount() >= 2) return false;
            List<ClientCard> materials = Bot.GetMonsters();
            if (included != null) materials.Add(included);
            if (materials.GetCardCount(CardId.Pittore) + materials.GetCardCount(CardId.Genni) == 0) return false;
            if (Bot.HasInHand(important_witchcraft)) return true;

            return false;
        }

        // summmon process of SalamangreatAlmiraj
        public bool SalamangreatAlmirajSummon()
        {
            if (!SalamangreatAlmirajSummonCheck()) return false;
            List<int> material = new List<int> { CardId.Pittore, CardId.Genni };
            AI.SelectMaterials(material);
            return true;
        }

        // activate of SalamangreatAlmiraj
        public bool SalamangreatAlmirajActivate()
        {
            if (Card.Location == CardLocation.Grave) return true;
            if (Duel.Player == 1)
            {
                AI.SelectCard(Util.GetBestBotMonster());
                return true;
            }
            return false;
        }

        // summmon process of PSYLambda
        public bool PSYLambdaSummon()
        {
            if (Bot.HasInMonstersZone(CardId.PSYGamma) && Bot.HasInMonstersZone(CardId.PSYDriver))
            {
                if (Bot.HasInHand(CardId.PSYGamma) || Bot.HasInMonstersZone(CardId.PSYOmega)) {
                    List<int> targets = new List<int>{CardId.PSYDriver, CardId.PSYGamma};
                    AI.SelectMaterials(targets);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// return place to summon RelinquishedAnima.
        /// if no need to summon, return -1
        /// </summary>
        /// <param name="included">Cards included into account</param>
        public int RelinquishedAnimaSummonCheck(ClientCard included = null)
        {
            // use witchcraft first
            if (CheckDiscardableSpellCount() >= 2) return -1;
            List<ClientCard> materials = Bot.GetMonsters();
            if (included != null) materials.Add(included);

            int place = -1;
            int attack = Util.GetBestAttack(Bot);
            // select place

            List<ClientCard> checklist = new List<ClientCard> { Enemy.MonsterZone[6], Enemy.MonsterZone[5] };
            List<int> placelist = new List<int> { 1, 3 };
            for (int i = 0; i < 2; ++i)
            {
                ClientCard card = checklist[i];
                int _place = placelist[i];
                if (card != null && card.HasLinkMarker((int)CardLinkMarker.Top) && card.Attack > attack &&
                    !card.IsShouldNotBeMonsterTarget() && !card.IsShouldNotBeTarget())
                {
                    ClientCard self_card = Bot.MonsterZone[_place];
                    if (self_card == null || self_card.Level == 1)
                    {
                        place = _place;
                        attack = card.Attack;
                    }
                }
            }
            checklist = new List<ClientCard> { Enemy.MonsterZone[3], Enemy.MonsterZone[1] };
            placelist = new List<int> { 5, 6 };
            for (int i = 0; i < 2; ++i)
            {
                ClientCard card = checklist[i];
                int _place = placelist[i];
                if (card != null && card.Attack > attack &&
                    !card.IsShouldNotBeMonsterTarget() && !card.IsShouldNotBeTarget())
                {
                    ClientCard enemy_card = Enemy.MonsterZone[11 - _place];
                    if (enemy_card != null) continue;
                    ClientCard self_card = Bot.MonsterZone[_place];
                    if (self_card == null || self_card.Level == 1)
                    {
                        place = _place;
                        attack = card.Attack;
                    }
                }
            }

            return place;
        }

        // summmon process of RelinquishedAnima
        public bool RelinquishedAnimaSummon()
        {
            int place = RelinquishedAnimaSummonCheck();
            Logger.DebugWriteLine("RelinquishedAnima summon check: " + place.ToString());
            if (place != -1)
            {
                int zone = (int)System.Math.Pow(2, place);
                AI.SelectPlace(zone);
                if (Bot.MonsterZone[place] != null && Bot.MonsterZone[place].Level == 1)
                {
                    AI.SelectMaterials(Bot.MonsterZone[place]);
                } else
                {
                    AI.SelectMaterials(CardId.Genni);
                }
                return true;
            }
            return false;
        }

        // default Chicken game
        public bool ChickenGame()
        {
            if (SpellNegatable()) return false;
            if (Bot.LifePoints <= 1000)
                return false;
            if (Bot.LifePoints - 1000 <= Enemy.LifePoints && ActivateDescription == Util.GetStringId(_CardId.ChickenGame, 0))
            {
                return true;
            }
            if (Bot.LifePoints - 1000 > Enemy.LifePoints && ActivateDescription == Util.GetStringId(_CardId.ChickenGame, 1))
            {
                return true;
            }
            return false;
        }

        protected override bool DefaultSetForDiabellze()
        {
            if (base.DefaultSetForDiabellze())
            {
                SelectSTPlace(null, true);
                return true;
            }
            return false;
        }
    }
}