using System;
using System.Collections.Generic;
using System.Linq;
using YGOSharp.OCGWrapper.Enums;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("Lightsworn", "AI_Lightsworn", "Normal")]
    public class LightswornExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int JudgmentDragon = 57774843;
            public const int Wulf = 58996430;
            public const int Garoth = 59019082;
            public const int Raiden = 77558536;
            public const int Lyla = 22624373;
            public const int Felis = 73176465;
            public const int Lumina = 95503687;
            public const int Minerva = 40164421;
            public const int Ryko = 21502796;
            public const int PerformageTrickClown = 67696066;
            public const int Goblindbergh = 25259669;
            public const int ThousandBlades = 1833916;
            public const int Honest = 37742478;
            public const int GlowUpBulb = 67441435;

            public const int SolarRecharge = 691925;
            public const int GalaxyCyclone = 5133471;
            public const int HarpiesFeatherDuster = 18144506;
            public const int ReinforcementOfTheArmy = 32807846;
            public const int MetalfoesFusion = 73594093;
            public const int ChargeOfTheLightBrigade = 94886282;

            public const int Michael = 4779823;
            public const int MinervaTheExalted = 30100551;
            public const int TrishulaDragonOfTheIceBarrier = 52687916;
            public const int ScarlightRedDragonArchfiend = 80666118;
            public const int PSYFramelordOmega = 74586817;
            public const int PSYFramelordZeta = 37192109;
            public const int NaturiaBeast = 33198837;
            public const int NumberS39UtopiatheLightning = 56832966;
            public const int Number39Utopia = 84013237;
            public const int Number101SilentHonorARK = 48739166;
            public const int CastelTheSkyblasterMusketeer = 82633039;
            public const int EvilswarmExcitonKnight = 46772449;
            public const int DanteTravelerOfTheBurningAbyss = 83531441;
        }

        private const int LightswornSetcode = 0x38;
        private const int MillDeckCountThreshold = 5;

        private bool _clownUsed;
        private bool _minervaTheExaltedUsed;
        private ClientCard _omegaBanishedEnemyCard;

        public LightswornExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, CardId.NaturiaBeast, NaturiaBeastEffect);
            AddExecutor(ExecutorType.Activate, CardId.PSYFramelordOmega, PSYFramelordOmegaEffect);
            AddExecutor(ExecutorType.Activate, CardId.PSYFramelordZeta, PSYFramelordZetaEffect);
            AddExecutor(ExecutorType.Activate, CardId.Number39Utopia, Number39UtopiaEffect);
            AddExecutor(ExecutorType.Activate, CardId.NumberS39UtopiatheLightning, NumberS39UtopiaTheLightningEffect);
            AddExecutor(ExecutorType.Activate, CardId.Honest, DefaultHonestEffect);
            AddExecutor(ExecutorType.Activate, CardId.TrishulaDragonOfTheIceBarrier, TrishulaEffect);

            AddExecutor(ExecutorType.Activate, CardId.Wulf);
            AddExecutor(ExecutorType.Activate, CardId.Felis, FelisSpecialSummonEffect);
            AddExecutor(ExecutorType.Activate, CardId.Garoth, GarothEffect);
            AddExecutor(ExecutorType.Activate, CardId.Minerva, MinervaEffect);
            AddExecutor(ExecutorType.Activate, CardId.Ryko);
            AddExecutor(ExecutorType.Activate, CardId.PerformageTrickClown, PerformageTrickClownEffect);
            AddExecutor(ExecutorType.Activate, CardId.ThousandBlades, ThousandBladesEffect);

            AddExecutor(ExecutorType.Activate, CardId.HarpiesFeatherDuster, DefaultHarpiesFeatherDusterFirst);
            AddExecutor(ExecutorType.Activate, CardId.GalaxyCyclone, DefaultGalaxyCyclone);
            AddExecutor(ExecutorType.Activate, CardId.MetalfoesFusion, MetalfoesFusionEffect);
            AddExecutor(ExecutorType.Activate, CardId.SolarRecharge, SolarRechargeEffect);
            AddExecutor(ExecutorType.Activate, CardId.ChargeOfTheLightBrigade, ChargeOfTheLightBrigadeEffect);
            AddExecutor(ExecutorType.Activate, CardId.ReinforcementOfTheArmy, ReinforcementOfTheArmyEffect);
            AddExecutor(ExecutorType.Activate, CardId.HarpiesFeatherDuster);

            AddExecutor(ExecutorType.Activate, CardId.JudgmentDragon, JudgmentDragonEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.JudgmentDragon, JudgmentDragonSummon);

            AddExecutor(ExecutorType.Activate, CardId.Goblindbergh, GoblindberghEffect);
            AddExecutor(ExecutorType.Activate, CardId.Felis, FelisDestroyEffect);
            AddExecutor(ExecutorType.Activate, CardId.Lyla, LylaEffect);
            AddExecutor(ExecutorType.Activate, CardId.Raiden, RaidenEffect);
            AddExecutor(ExecutorType.Activate, CardId.Lumina, LuminaEffect);
            AddExecutor(ExecutorType.Activate, CardId.GlowUpBulb, GlowUpBulbEffect);
            AddExecutor(ExecutorType.Activate, CardId.MinervaTheExalted, MinervaTheExaltedEffect);
            AddExecutor(ExecutorType.Activate, CardId.Michael, MichaelEffect);
            AddExecutor(ExecutorType.Activate, CardId.EvilswarmExcitonKnight, EvilswarmExcitonKnightEffect);
            AddExecutor(ExecutorType.Activate, CardId.Number101SilentHonorARK, Number101SilentHonorARKEffect);
            AddExecutor(ExecutorType.Activate, CardId.CastelTheSkyblasterMusketeer, CastelEffect);
            AddExecutor(ExecutorType.Activate, CardId.ScarlightRedDragonArchfiend, DefaultScarlightRedDragonArchfiendEffect);
            AddExecutor(ExecutorType.Activate, CardId.DanteTravelerOfTheBurningAbyss, DanteEffect);

            AddExecutor(ExecutorType.Summon, CardId.Goblindbergh, GoblindberghSummon);
            AddExecutor(ExecutorType.Summon, CardId.Lumina, LuminaSummon);
            AddExecutor(ExecutorType.Summon, CardId.Lyla, LylaSummon);
            AddExecutor(ExecutorType.Summon, CardId.Raiden, RaidenSummon);
            AddExecutor(ExecutorType.Summon, CardId.Minerva, MinervaSummon);
            AddExecutor(ExecutorType.Summon, CardId.Garoth);
            AddExecutor(ExecutorType.Summon, CardId.PerformageTrickClown, Level4ExtenderSummon);
            AddExecutor(ExecutorType.Summon, CardId.ThousandBlades, Level4ExtenderSummon);

            AddExecutor(ExecutorType.SpSummon, CardId.EvilswarmExcitonKnight, EvilswarmExcitonKnightSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.TrishulaDragonOfTheIceBarrier, TrishulaSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.Number101SilentHonorARK, Number101SilentHonorARKSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.CastelTheSkyblasterMusketeer, CastelSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.Michael, MichaelSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.ScarlightRedDragonArchfiend, ScarlightRedDragonArchfiendSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.NaturiaBeast, NaturiaBeastSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.MinervaTheExalted, MinervaTheExaltedSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.Number39Utopia, Number39UtopiaSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.NumberS39UtopiatheLightning);
            AddExecutor(ExecutorType.SpSummon, CardId.PSYFramelordZeta, PSYFramelordZetaSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.PSYFramelordOmega, PSYFramelordOmegaSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.DanteTravelerOfTheBurningAbyss, DanteSummon);

            AddExecutor(ExecutorType.MonsterSet, CardId.Ryko, RykoSet);
            AddExecutor(ExecutorType.MonsterSet, CardId.PerformageTrickClown);
            AddExecutor(ExecutorType.MonsterSet, CardId.ThousandBlades);
            AddExecutor(ExecutorType.MonsterSet, CardId.Raiden);

            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
        }

        public override bool OnSelectHand()
        {
            return true;
        }

        public override void OnNewTurn()
        {
            _clownUsed = false;
            _minervaTheExaltedUsed = false;
            base.OnNewTurn();
        }

        public override void OnMove(
            ClientCard card,
            int previousControler,
            int previousLocation,
            int currentControler,
            int currentLocation)
        {
            if (card != null)
            {
                ChainInfo solvingChain = Duel.GetCurrentSolvingChainInfo();
                if (previousControler == 1 &&
                    previousLocation == (int)CardLocation.Hand &&
                    currentControler == 1 &&
                    currentLocation == (int)CardLocation.Removed &&
                    solvingChain != null &&
                    solvingChain.ActivatePlayer == 0 &&
                    solvingChain.IsActivateCode(CardId.PSYFramelordOmega))
                {
                    _omegaBanishedEnemyCard = card;
                }
                else if (card.Equals(_omegaBanishedEnemyCard) &&
                    currentLocation != (int)CardLocation.Removed)
                {
                    _omegaBanishedEnemyCard = null;
                }
            }

            base.OnMove(card, previousControler, previousLocation, currentControler, currentLocation);
        }

        public override IList<ClientCard> OnSelectCard(
            IList<ClientCard> cards, int min, int max, int hint, bool cancelable)
        {
            ChainInfo solvingChain = Duel.GetCurrentSolvingChainInfo();
            if (solvingChain != null && solvingChain.ActivatePlayer == 0)
            {
                if (solvingChain.IsActivateCode(CardId.MinervaTheExalted) && hint == HintMsg.Destroy ||
                    solvingChain.IsActivateCode(CardId.Ryko) && hint == HintMsg.Destroy ||
                    solvingChain.IsActivateCode(CardId.TrishulaDragonOfTheIceBarrier) && hint == HintMsg.Remove)
                {
                    List<ClientCard> targets = GetEnemyTargetPriority(cards);
                    if (targets.Count > 0)
                        return Util.CheckSelectCount(targets, cards, min, max);
                }
            }

            return base.OnSelectCard(cards, min, max, hint, cancelable);
        }

        public override bool OnSelectYesNo(int desc)
        {
            ChainInfo solvingChain = Duel.GetCurrentSolvingChainInfo();
            if (solvingChain != null && solvingChain.ActivatePlayer == 0 &&
                solvingChain.IsActivateCode(CardId.MinervaTheExalted, CardId.Ryko))
            {
                return Enemy.GetFieldCount() > 0;
            }

            return base.OnSelectYesNo(desc);
        }

        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            if ((cardId == CardId.PerformageTrickClown ||
                 cardId == CardId.Felis ||
                 cardId == CardId.Lumina ||
                 cardId == CardId.Minerva ||
                 cardId == CardId.Ryko ||
                 cardId == CardId.GlowUpBulb) &&
                positions.Contains(CardPosition.FaceUpDefence))
            {
                return CardPosition.FaceUpDefence;
            }

            return base.OnSelectPosition(cardId, positions);
        }

        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            if (!defender.IsMonsterHasPreventActivationEffectInBattle() &&
                attacker.HasAttribute(CardAttribute.Light) &&
                Bot.HasInHand(CardId.Honest) &&
                !DefaultCheckWhetherCardIdIsNegated(CardId.Honest))
            {
                attacker.RealPower += defender.Attack;
            }

            return base.OnPreBattleBetween(attacker, defender);
        }

        public override IList<ClientCard> OnSelectXyzMaterial(IList<ClientCard> cards, int min, int max)
        {
            List<ClientCard> result = cards
                .OrderBy(GetMaterialPriority)
                .ThenBy(card => card.Attack)
                .Take(min)
                .ToList();
            return Util.CheckSelectCount(result, cards, min, max);
        }

        private bool IsLightsworn(ClientCard card)
        {
            return card != null && card.IsMonster() && card.HasSetcode(LightswornSetcode);
        }

        private bool HasLuminaReviveTarget()
        {
            return Bot.GetGraveyardMonsters()
                .Any(card => IsLightsworn(card) && card.Level <= 4);
        }

        // 卡组数量不足时不应继续堆墓
        private bool ShouldMillCards()
        {
            return Bot.Deck.Count > MillDeckCountThreshold;
        }

        private List<ClientCard> GetEnemyTargetPriority(IEnumerable<ClientCard> candidates)
        {
            List<ClientCard> available = candidates
                .Where(card => card != null && card.Controller == 1)
                .ToList();
            List<ClientCard> result = new List<ClientCard>();

            ClientCard target = Util.GetProblematicEnemyCard();
            if (target != null && available.Contains(target))
                result.Add(target);

            target = Util.GetBestEnemyMonster();
            if (target != null && available.Contains(target) && !result.Contains(target))
                result.Add(target);

            target = Util.GetBestEnemySpell();
            if (target != null && available.Contains(target) && !result.Contains(target))
                result.Add(target);

            result.AddRange(available
                .Where(card => !result.Contains(card))
                .OrderByDescending(card => card.IsFaceup())
                .ThenByDescending(card => card.IsMonster())
                .ThenByDescending(card => card.GetDefensePower()));
            return result;
        }

        private int GetMaterialPriority(ClientCard card)
        {
            if (card.IsCode(CardId.PerformageTrickClown))
                return 0;
            if (card.IsCode(CardId.Wulf))
                return 1;
            if (card.IsCode(CardId.ThousandBlades))
                return 2;
            if (card.IsCode(CardId.Goblindbergh))
                return 3;
            if (card.IsCode(CardId.Felis))
                return 4;
            if (card.IsCode(CardId.Garoth))
                return 5;
            if (card.IsCode(CardId.Lyla))
                return 6;
            if (card.IsCode(CardId.Honest))
                return 7;
            if (card.IsCode(CardId.Minerva))
                return 8;
            if (card.IsCode(CardId.Lumina))
                return 10;
            if (card.IsCode(CardId.Raiden))
                return 11;
            if (card.IsCode(CardId.GlowUpBulb))
                return 12;
            return 9;
        }

        private int GetDiscardPriority(ClientCard card)
        {
            if (card.IsCode(CardId.MetalfoesFusion))
                return 0;
            if (card.IsCode(CardId.PerformageTrickClown) && !_clownUsed && Bot.LifePoints > 1000)
                return 1;
            if (card.IsCode(CardId.Minerva))
                return 2;
            if (card.IsCode(CardId.Wulf))
                return 3;
            if (card.IsCode(CardId.Felis))
                return 4;
            if (card.IsCode(CardId.PerformageTrickClown))
                return 5;
            if (card.IsCode(CardId.ThousandBlades))
                return 6;
            if (card.IsCode(CardId.GalaxyCyclone))
                return Enemy.GetSpells().Any(other => other.IsFacedown()) ? 14 : 7;
            if (card.IsCode(CardId.Ryko))
                return 8;
            if (card.IsCode(CardId.Garoth))
                return 9;
            if (card.IsCode(CardId.Lyla))
                return 10;
            if (card.IsCode(CardId.JudgmentDragon))
                return Bot.Hand.Count(c => c.IsCode(CardId.JudgmentDragon)) > 1 ? 11 : 18;
            if (card.IsCode(CardId.Lumina))
                return Bot.Hand.Count(c => c.IsCode(CardId.Lumina)) > 1 ? 12 : 16;
            if (card.IsCode(CardId.Raiden))
                return Bot.Hand.Count(c => c.IsCode(CardId.Raiden)) > 1 ? 13 : 17;
            if (card.IsCode(CardId.Honest))
                return 19;
            return 15;
        }

        private List<ClientCard> GetDiscardPriority(bool lightswornOnly)
        {
            return Bot.Hand
                .Where(card => card != null && !card.Equals(Card) && (!lightswornOnly || IsLightsworn(card)))
                .OrderBy(GetDiscardPriority)
                .ThenBy(card => card.Attack)
                .ToList();
        }

        private bool SelectXyzMaterials(int level)
        {
            List<List<ClientCard>> materialLists = Util.GetXyzMaterials(
                Bot.GetMonsters(), level, 2);
            List<ClientCard> materials = materialLists
                .OrderBy(list => list.Sum(GetMaterialPriority))
                .ThenBy(list => list.Sum(card => card.Attack))
                .FirstOrDefault();
            if (materials == null)
                return false;

            AI.SelectMaterials(materials);
            return true;
        }

        private bool SelectSynchroMaterials(
            int level,
            int nonTunerCount,
            Func<ClientCard, bool> tunerFilter = null,
            Func<ClientCard, bool> nonTunerFilter = null)
        {
            List<List<ClientCard>> materialLists = Util.GetSynchroMaterials(
                Bot.GetMonsters(),
                level,
                1,
                nonTunerCount,
                false,
                true,
                tunerFilter,
                nonTunerFilter);
            List<ClientCard> materials = materialLists
                .OrderBy(list => list.Count)
                .ThenBy(list => list.Sum(GetMaterialPriority))
                .ThenBy(list => list.Sum(card => card.Attack))
                .FirstOrDefault();
            if (materials == null)
                return false;

            AI.SelectMaterials(materials);
            return true;
        }

        private void SelectXyzDetachMaterial()
        {
            AI.SelectCard(
                CardId.PerformageTrickClown,
                CardId.Wulf,
                CardId.Felis,
                CardId.ThousandBlades,
                CardId.Goblindbergh,
                CardId.Garoth,
                CardId.Lyla,
                CardId.Honest,
                CardId.Minerva,
                CardId.Lumina,
                CardId.Raiden);
        }

        private bool MetalfoesFusionEffect()
        {
            return Card.Location == CardLocation.Grave;
        }

        private bool SolarRechargeEffect()
        {
            if (!ShouldMillCards() || DefaultSpellWillBeNegated())
                return false;

            List<ClientCard> discards = GetDiscardPriority(true);
            AI.SelectCard(discards);
            return true;
        }

        private bool ChargeOfTheLightBrigadeEffect()
        {
            if (!ShouldMillCards() || DefaultSpellWillBeNegated())
                return false;

            List<int> priority = new List<int>();
            if (Enemy.GetSpellCount() > 0)
                priority.Add(CardId.Lyla);
            if (HasLuminaReviveTarget() &&
                Bot.Hand.Any(card => card != null && !card.Equals(Card)))
            {
                priority.Add(CardId.Lumina);
            }
            if (!Bot.HasInHandOrHasInMonstersZone(CardId.Raiden))
                priority.Add(CardId.Raiden);
            priority.Add(CardId.Lumina);
            priority.Add(CardId.Raiden);
            priority.Add(CardId.Lyla);
            priority.Add(CardId.Minerva);
            priority.Add(CardId.Garoth);
            priority.Add(CardId.Ryko);

            AI.SelectCard(priority);
            return true;
        }

        private bool ReinforcementOfTheArmyEffect()
        {
            if (DefaultSpellWillBeNegated())
                return false;

            bool hasGoblindberghTarget = Bot.Hand.Any(card =>
                card != null &&
                !card.Equals(Card) &&
                card.IsMonster() &&
                card.Level <= 4);
            if (hasGoblindberghTarget && !Bot.HasInHand(CardId.Goblindbergh))
                AI.SelectCard(CardId.Goblindbergh, CardId.Raiden, CardId.Garoth);
            else
                AI.SelectCard(CardId.Raiden, CardId.Goblindbergh, CardId.Garoth);
            return true;
        }

        private bool JudgmentDragonSummon()
        {
            int possibleDamage = 3000 + Bot.GetMonsters()
                .Where(card => card.IsFaceup() && card.IsAttack())
                .Sum(card => card.Attack);
            int endPhaseMills = 4 + Bot.GetMonsters().Sum(card =>
                card.IsCode(CardId.JudgmentDragon) ? 4 :
                card.IsCode(CardId.Lumina, CardId.Lyla, CardId.Michael) ? 3 :
                card.IsCode(CardId.Raiden, CardId.Minerva) ? 2 : 0);
            return Bot.Deck.Count > endPhaseMills || possibleDamage >= Enemy.LifePoints;
        }

        private bool JudgmentDragonEffect()
        {
            if (ActivateDescription == Util.GetStringId(CardId.JudgmentDragon, 2))
                return true;
            if (ActivateDescription != Util.GetStringId(CardId.JudgmentDragon, 1))
                return false;
            if (Bot.LifePoints <= 1000 || DefaultCheckWhetherCardIsNegated(Card))
                return false;

            int enemyCards = Enemy.GetFieldCount();
            if (enemyCards == 0)
                return false;

            int ownCards = Bot.GetFieldCount() - 1;
            ClientCard problem = Util.GetProblematicEnemyCard(3000);
            bool canAttackForGame = Enemy.GetMonsterCount() > 0 &&
                Enemy.LifePoints <= Card.Attack;
            return problem != null ||
                canAttackForGame ||
                enemyCards >= 2 && enemyCards > ownCards;
        }

        private bool GoblindberghSummon()
        {
            return Bot.Hand.Any(card =>
                card != null &&
                !card.Equals(Card) &&
                card.IsMonster() &&
                card.Level <= 4);
        }

        private bool GoblindberghEffect()
        {
            AI.SelectCard(
                CardId.PerformageTrickClown,
                CardId.Wulf,
                CardId.Felis,
                CardId.ThousandBlades,
                CardId.Raiden,
                CardId.Lyla,
                CardId.Garoth,
                CardId.Honest,
                CardId.Lumina,
                CardId.Minerva,
                CardId.Ryko,
                CardId.GlowUpBulb);
            return true;
        }

        private bool LuminaSummon()
        {
            bool hasDiscard = Bot.Hand.Any(card => card != null && !card.Equals(Card));
            return hasDiscard && HasLuminaReviveTarget() ||
                Bot.HasInExtra(CardId.DanteTravelerOfTheBurningAbyss) &&
                Bot.GetMonsters().Any(card => card.IsFaceup() && card.Level == 3);
        }

        private bool LuminaEffect()
        {
            if (ActivateDescription != Util.GetStringId(CardId.Lumina, 0))
                return true;
            if (DefaultCheckWhetherCardIsNegated(Card))
                return false;

            List<ClientCard> discards = GetDiscardPriority(false);
            List<int> revivePriority = new List<int>();
            if (Enemy.GetSpellCount() > 0)
                revivePriority.Add(CardId.Lyla);
            if (Enemy.GetMonsterCount() > 0)
                revivePriority.Add(CardId.Felis);
            revivePriority.Add(CardId.Raiden);
            revivePriority.Add(CardId.Felis);
            revivePriority.Add(CardId.Lyla);
            revivePriority.Add(CardId.Garoth);
            revivePriority.Add(CardId.Wulf);
            revivePriority.Add(CardId.Lumina);
            revivePriority.Add(CardId.Minerva);
            revivePriority.Add(CardId.Ryko);

            AI.SelectCard(discards);
            AI.SelectNextCard(revivePriority);
            return true;
        }

        private bool LylaSummon()
        {
            return Enemy.GetSpellCount() > 0 ||
                Bot.GetMonsters().Any(card => card.IsFaceup() && card.Level == 4);
        }

        private bool LylaEffect()
        {
            if (ActivateDescription != Util.GetStringId(CardId.Lyla, 0))
                return true;
            if (DefaultCheckWhetherCardIsNegated(Card))
                return false;

            ClientCard target = GetEnemyTargetPriority(Enemy.GetSpells()).FirstOrDefault();
            if (target == null)
                return false;

            AI.SelectCard(target);
            return true;
        }

        private bool RaidenSummon()
        {
            return Bot.Deck.Count >= 2;
        }

        private bool RaidenEffect()
        {
            if (ActivateDescription != Util.GetStringId(CardId.Raiden, 0))
                return true;
            return ShouldMillCards() && !DefaultCheckWhetherCardIsNegated(Card);
        }

        private bool MinervaSummon()
        {
            int lightswornNames = Bot.GetGraveyardMonsters()
                .Where(IsLightsworn)
                .Select(card => card.Alias > 0 ? card.Alias : card.Id)
                .Where(id => id != 0)
                .Distinct()
                .Count();
            if (lightswornNames >= 8)
                return true;

            if (Bot.HasInExtra(CardId.DanteTravelerOfTheBurningAbyss) &&
                Bot.GetMonsters().Any(card => card.IsFaceup() && card.Level == 3))
            {
                return true;
            }

            bool hasLevel4NonTuner = Bot.GetMonsters().Any(card =>
                card.IsFaceup() &&
                card.Level == 4 &&
                !card.HasType(CardType.Tuner));
            if (!hasLevel4NonTuner)
                return false;

            bool canMakeZeta = Bot.HasInExtra(CardId.PSYFramelordZeta) &&
                Enemy.GetMonsters().Any(card =>
                    card.IsFaceup() &&
                    card.IsAttack() &&
                    card.IsSpecialSummoned);
            bool canMakeMichael = Bot.HasInExtra(CardId.Michael) &&
                Bot.LifePoints > 1000 &&
                Enemy.GetFieldCount() > 0 &&
                Bot.GetMonsters().Any(card =>
                    card.IsFaceup() &&
                    card.Level == 4 &&
                    !card.HasType(CardType.Tuner) &&
                    card.HasAttribute(CardAttribute.Light));
            return canMakeZeta || canMakeMichael;
        }

        private bool MinervaEffect()
        {
            if (ActivateDescription == Util.GetStringId(CardId.Minerva, 0))
                AI.SelectCard(CardId.JudgmentDragon);
            return true;
        }

        private bool Level4ExtenderSummon()
        {
            return Bot.GetMonsters().Any(card => card.IsFaceup() && card.Level == 4);
        }

        private bool ThousandBladesEffect()
        {
            return Card.Location == CardLocation.Grave &&
                ActivateDescription == Util.GetStringId(CardId.ThousandBlades, 1) &&
                !DefaultCheckWhetherCardIsNegated(Card);
        }

        private bool RykoSet()
        {
            return Enemy.GetFieldCount() > 0 || Bot.GetMonsterCount() == 0;
        }

        private bool GarothEffect()
        {
            return ShouldMillCards();
        }

        private bool FelisSpecialSummonEffect()
        {
            return Card.Location == CardLocation.Grave ||
                ActivateDescription == Util.GetStringId(CardId.Felis, 0);
        }

        private bool FelisDestroyEffect()
        {
            if (Card.Location != CardLocation.MonsterZone ||
                ActivateDescription != Util.GetStringId(CardId.Felis, 1) ||
                !ShouldMillCards() ||
                DefaultCheckWhetherCardIsNegated(Card))
            {
                return false;
            }

            ClientCard target = GetEnemyTargetPriority(Enemy.GetMonsters()).FirstOrDefault();
            if (target == null)
                return false;
            if (Util.GetProblematicEnemyMonster() == null &&
                target.GetDefensePower() < 1800 &&
                Enemy.GetMonsterCount() < 2)
            {
                return false;
            }

            AI.SelectCard(target);
            return true;
        }

        private bool PerformageTrickClownEffect()
        {
            if (_clownUsed || Bot.LifePoints <= 1000 || DefaultCheckWhetherCardIsNegated(Card))
                return false;

            AI.SelectCard(CardId.PerformageTrickClown);
            AI.SelectPosition(CardPosition.FaceUpDefence);
            _clownUsed = true;
            return true;
        }

        private bool GlowUpBulbEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card))
                return false;

            List<ClientCard> level4NonTuners = Bot.GetMonsters()
                .Where(card =>
                    card.IsFaceup() &&
                    card.Level == 4 &&
                    !card.HasType(CardType.Tuner) &&
                    !card.IsMonsterNotBeSynchroMaterial())
                .ToList();
            bool canMakeNaturiaBeast =
                Bot.HasInExtra(CardId.NaturiaBeast) &&
                // 鳞茎先送墓 1 张，之后自然兽仍需 2 张卡支付无效费用。
                Bot.Deck.Count >= 3 &&
                level4NonTuners.Any(card =>
                    card.HasAttribute(CardAttribute.Earth));
            int enemyZones = (Enemy.GetFieldCount() > 0 ? 1 : 0) +
                (Enemy.GetHandCount() > 0 ? 1 : 0) +
                (Enemy.Graveyard.Count > 0 ? 1 : 0);
            bool canMakeTrishula =
                Bot.HasInExtra(CardId.TrishulaDragonOfTheIceBarrier) &&
                enemyZones >= 2 &&
                level4NonTuners.Count >= 2;
            return canMakeNaturiaBeast || canMakeTrishula;
        }

        private bool MinervaTheExaltedEffect()
        {
            if (!ShouldMillCards())
                return false;

            if (Card.Location == CardLocation.MonsterZone)
            {
                if (DefaultCheckWhetherCardIsNegated(Card))
                    return false;

                SelectXyzDetachMaterial();
                _minervaTheExaltedUsed = true;
                return true;
            }

            return Card.Location == CardLocation.Grave &&
                !DefaultCheckWhetherCardIsNegated(Card);
        }

        private bool MichaelEffect()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {
                if (ActivateDescription == Util.GetStringId(CardId.Michael, 2))
                    return true;
                if (ActivateDescription != Util.GetStringId(CardId.Michael, 0) ||
                    Bot.LifePoints <= 1000 ||
                    DefaultCheckWhetherCardIsNegated(Card))
                {
                    return false;
                }

                ClientCard target = GetEnemyTargetPriority(
                    Enemy.GetMonsters().Concat(Enemy.GetSpells())).FirstOrDefault();
                if (target == null)
                    return false;

                AI.SelectCard(target);
                return true;
            }

            if (Card.Location != CardLocation.Grave || DefaultCheckWhetherCardIsNegated(Card))
                return false;

            List<ClientCard> targets = Bot.GetGraveyardMonsters()
                .Where(card => IsLightsworn(card) && !card.Equals(Card))
                .OrderBy(card => Bot.Graveyard.Count(other => other.IsCode(card.Id)) > 1 ? 0 : 1)
                .ThenBy(GetMaterialPriority)
                .ToList();
            if (Bot.Deck.Count > 10 && Bot.LifePoints > 3000 && targets.Count <= 4)
                return false;

            int count = Bot.Deck.Count <= 10 ? Math.Min(3, targets.Count) : 1;
            AI.SelectCard(targets.Take(count).ToList());
            return true;
        }

        private bool NaturiaBeastEffect()
        {
            return Duel.LastChainPlayer == 1 &&
                ShouldMillCards() &&
                !DefaultCheckWhetherCardIsNegated(Card);
        }

        private bool PSYFramelordOmegaEffect()
        {
            if (Card.Location == CardLocation.Grave)
            {
                ClientCard target = Enemy.Graveyard
                    .Where(card => card != null)
                    .OrderByDescending(card => card.IsMonster())
                    .ThenByDescending(card => card.GetDefensePower())
                    .FirstOrDefault();
                if (target == null && Bot.Deck.Count <= 8)
                {
                    target = Bot.Graveyard
                        .Where(card =>
                            card != null &&
                            !card.Equals(Card) &&
                            !card.IsCode(CardId.MetalfoesFusion, CardId.GalaxyCyclone))
                        .OrderBy(GetMaterialPriority)
                        .FirstOrDefault();
                }
                if (target == null)
                    return false;

                AI.SelectCard(target);
                return true;
            }

            if (Card.Location != CardLocation.MonsterZone || DefaultCheckWhetherCardIsNegated(Card))
                return false;

            if (ActivateDescription == Util.GetStringId(CardId.PSYFramelordOmega, 1) ||
                Duel.Phase == DuelPhase.Standby)
            {
                ClientCard target = _omegaBanishedEnemyCard;
                if (target == null ||
                    !Enemy.Banished.Contains(target) ||
                    !target.IsFaceup())
                {
                    target = Bot.Banished
                        .Where(card => card != null && card.IsFaceup())
                        .OrderBy(GetMaterialPriority)
                        .FirstOrDefault();
                }
                if (target == null)
                    return false;

                AI.SelectCard(target);
                return true;
            }

            return Enemy.GetHandCount() > 0 &&
                (Duel.Player == 1 ||
                 Duel.Turn == 1 ||
                 Duel.Phase == DuelPhase.Main2 ||
                 DefaultOnBecomeTarget());
        }

        private bool PSYFramelordZetaEffect()
        {
            if (Card.Location != CardLocation.MonsterZone || DefaultCheckWhetherCardIsNegated(Card))
                return false;

            List<ClientCard> candidates = Enemy.GetMonsters()
                .Where(card => card.IsFaceup() && card.IsAttack() && card.IsSpecialSummoned)
                .ToList();
            ClientCard target = GetEnemyTargetPriority(candidates).FirstOrDefault();
            if (target == null)
                return false;
            if (Duel.Player == 0 &&
                Duel.Phase != DuelPhase.Main2 &&
                !DefaultOnBecomeTarget() &&
                target.GetDefensePower() < Card.Attack &&
                !target.Equals(Util.GetProblematicEnemyMonster()))
            {
                return false;
            }

            AI.SelectCard(target);
            return true;
        }

        private bool Number39UtopiaEffect()
        {
            if (Duel.Player == 0)
                return false;

            SelectXyzDetachMaterial();
            return true;
        }

        private bool NumberS39UtopiaTheLightningEffect()
        {
            if (!DefaultNumberS39UtopiaTheLightningEffect())
                return false;

            SelectXyzDetachMaterial();
            return true;
        }

        private bool TrishulaEffect()
        {
            return !DefaultCheckWhetherCardIsNegated(Card);
        }

        private bool EvilswarmExcitonKnightEffect()
        {
            if (!DefaultEvilswarmExcitonKnightEffect() || DefaultCheckWhetherCardIsNegated(Card))
                return false;

            SelectXyzDetachMaterial();
            return true;
        }

        private bool Number101SilentHonorARKEffect()
        {
            if (ActivateDescription == 96) return true;

            if (DefaultCheckWhetherCardIsNegated(Card))
                return false;

            List<ClientCard> candidates = Enemy.GetMonsters()
                .Where(card => card.IsFaceup() && card.IsAttack() && card.IsSpecialSummoned)
                .ToList();
            ClientCard target = GetEnemyTargetPriority(candidates).FirstOrDefault();
            if (target == null)
                return false;

            SelectXyzDetachMaterial();
            AI.SelectNextCard(target);
            return true;
        }

        private bool CastelEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card))
                return false;

            ClientCard target;
            if (ActivateDescription == Util.GetStringId(CardId.CastelTheSkyblasterMusketeer, 1))
            {
                target = GetEnemyTargetPriority(
                    Enemy.GetMonsters().Concat(Enemy.GetSpells()).Where(card => card.IsFaceup()))
                    .FirstOrDefault();
            }
            else
            {
                target = GetEnemyTargetPriority(
                    Enemy.GetMonsters().Where(card => card.IsFaceup() && !card.HasType(CardType.Link)))
                    .FirstOrDefault();
                if (target != null && Util.GetProblematicEnemyMonster() == null)
                    return false;
            }
            if (target == null)
                return false;

            SelectXyzDetachMaterial();
            AI.SelectNextCard(target);
            return true;
        }

        private bool DanteEffect()
        {
            if (Card.Location != CardLocation.MonsterZone ||
                !ShouldMillCards() ||
                DefaultCheckWhetherCardIsNegated(Card))
            {
                return false;
            }

            SelectXyzDetachMaterial();
            AI.SelectNumber(Math.Min(3, Bot.Deck.Count));
            return true;
        }

        private bool EvilswarmExcitonKnightSummon()
        {
            return DefaultEvilswarmExcitonKnightSummon() && SelectXyzMaterials(4);
        }

        private bool TrishulaSummon()
        {
            int enemyZones = (Enemy.GetFieldCount() > 0 ? 1 : 0) +
                (Enemy.GetHandCount() > 0 ? 1 : 0) +
                (Enemy.Graveyard.Count > 0 ? 1 : 0);
            return enemyZones >= 2 && SelectSynchroMaterials(9, 2);
        }

        private bool Number101SilentHonorARKSummon()
        {
            bool hasTarget = Enemy.GetMonsters()
                .Any(card => card.IsFaceup() && card.IsAttack() && card.IsSpecialSummoned);
            return hasTarget && SelectXyzMaterials(4);
        }

        private bool CastelSummon()
        {
            return Util.GetProblematicEnemyCard() != null && SelectXyzMaterials(4);
        }

        private bool MichaelSummon()
        {
            if (Bot.LifePoints <= 1000 || Enemy.GetFieldCount() == 0)
                return false;

            return SelectSynchroMaterials(
                7,
                1,
                null,
                card => card.HasAttribute(CardAttribute.Light));
        }

        private bool ScarlightRedDragonArchfiendSummon()
        {
            return DefaultScarlightRedDragonArchfiendSummon() &&
                SelectSynchroMaterials(8, 1);
        }

        private bool Number39UtopiaSummon()
        {
            if (DefaultNumberS39UtopiaTheLightningSummon())
                return SelectXyzMaterials(4);

            List<ClientCard> defenceMaterials = Util.GetXyzMaterials(
                    Bot.GetMonsters(), 4, 2)
                .Where(materials =>
                    materials.All(card => card.IsDefense()))
                .OrderBy(materials =>
                    materials.Sum(GetMaterialPriority))
                .ThenBy(materials =>
                    materials.Sum(card => card.Attack))
                .FirstOrDefault();
            if (defenceMaterials == null)
                return false;

            AI.SelectMaterials(defenceMaterials);
            return true;
        }

        private bool NaturiaBeastSummon()
        {
            if (!ShouldMillCards())
                return false;

            return SelectSynchroMaterials(
                5,
                1,
                card => card.HasAttribute(CardAttribute.Earth),
                card => card.HasAttribute(CardAttribute.Earth));
        }

        private bool PSYFramelordZetaSummon()
        {
            bool hasTarget = Enemy.GetMonsters()
                .Any(card => card.IsFaceup() && card.IsAttack() && card.IsSpecialSummoned);
            return hasTarget && SelectSynchroMaterials(7, 1);
        }

        private bool PSYFramelordOmegaSummon()
        {
            return (Util.IsTurn1OrMain2() || Enemy.GetHandCount() >= 3) &&
                SelectSynchroMaterials(8, 1);
        }

        private bool MinervaTheExaltedSummon()
        {
            return !_minervaTheExaltedUsed &&
                !Bot.HasInMonstersZone(CardId.MinervaTheExalted) &&
                ShouldMillCards() &&
                SelectXyzMaterials(4);
        }

        private bool DanteSummon()
        {
            return ShouldMillCards() &&
                SelectXyzMaterials(3);
        }
    }
}
