using YGOSharp.OCGWrapper;
using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using System.Linq;

namespace WindBot.Game.AI.Decks
{
    [Deck("TimeThief", "AI_Timethief")]
    public class TimeThiefExecutor : DefaultExecutor
    {
        public class Monsters
        {
            //monsters
            public const int TimeThiefWinder = 56308388;
            public const int TimeThiefBezelShip = 82496097;
            public const int TimeThiefCronocorder = 74578720;
            public const int TimeThiefRegulator = 19891131;
            public const int PhotonTrasher = 65367484;
            public const int PerformTrickClown = 67696066;
            public const int ThunderKingRaiOh = 71564252;
            public const int MaxxC = 23434538;
            public const int AshBlossomAndJoyousSpring = 14558127;
        }

        public class CardId
        {
            public const int ImperialOrder = 61740673;
            public const int NaturalExterio = 99916754;
            public const int NaturalBeast = 33198837;
            public const int SwordsmanLV7 = 37267041;
            public const int RoyalDecreel = 51452091;
        }

        public class Spells
        {
            // spells
            public const int Raigeki = 12580477;
            public const int FoolishBurial = 81439173;
            public const int TimeThiefStartup = 10877309;
            public const int TimeThiefHack = 81670445;
            public const int HarpieFeatherDuster = 18144506;
            public const int PotOfDesires = 35261759;
            public const int PotofExtravagance = 49238328;
        }
        public class Traps
        {
            //traps
            public const int SolemnWarning = 84749824;
            public const int SolemStrike = 40605147;
            public const int SolemnJudgment = 41420027;
            public const int TimeThiefRetrograte = 76587747;
            public const int PhantomKnightsShade = 98827725;
            public const int TimeThiefFlyBack = 18678554;
            public const int Crackdown = 36975314;
        }
        public class XYZs
        {
            //xyz
            public const int TimeThiefRedoer = 55285840;
            public const int TimeThiefPerpetua = 59208943;
            public const int CrazyBox = 42421606;
            public const int GagagaCowboy = 12014404;
            public const int Number39Utopia = 84013237;
            public const int NumberS39UtopiatheLightning = 56832966;
            public const int NumberS39UtopiaOne = 86532744;
            public const int DarkRebellionXyzDragon = 16195942;
            public const int EvilswarmExcitonKnight = 46772449;
        }



        public TimeThiefExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // executors
            //Spell activate
            AddExecutor(ExecutorType.Activate, Spells.PotofExtravagance, PotofExtravaganceActivate);
            AddExecutor(ExecutorType.Activate, Spells.Raigeki, DefaultDarkHole);
            AddExecutor(ExecutorType.Activate, Spells.FoolishBurial, FoolishBurialTarget);
            AddExecutor(ExecutorType.Activate, Spells.TimeThiefStartup, TimeThiefStartupEffect);
            AddExecutor(ExecutorType.Activate, Spells.TimeThiefHack, TimeThiefHackEffect);
            AddExecutor(ExecutorType.Activate, Spells.HarpieFeatherDuster, DefaultHarpiesFeatherDusterFirst);
            AddExecutor(ExecutorType.Activate, Spells.PotOfDesires, PotOfDesireseff);
            // trap executors set
            AddExecutor(ExecutorType.SpellSet, Traps.PhantomKnightsShade, DefaultSpellSet);
            AddExecutor(ExecutorType.SpellSet, Traps.TimeThiefRetrograte, DefaultSpellSet);
            AddExecutor(ExecutorType.SpellSet, Traps.TimeThiefFlyBack, DefaultSpellSet);
            AddExecutor(ExecutorType.SpellSet, Traps.SolemnWarning, DefaultSpellSet);
            AddExecutor(ExecutorType.SpellSet, Traps.SolemStrike, DefaultSpellSet);
            AddExecutor(ExecutorType.SpellSet, Traps.SolemnJudgment, DefaultSpellSet);
            AddExecutor(ExecutorType.SpellSet, Traps.Crackdown, DefaultSpellSet);
            //normal summons
            AddExecutor(ExecutorType.Summon, Monsters.TimeThiefRegulator);
            AddExecutor(ExecutorType.SpSummon, Monsters.PhotonTrasher, SummonToDef);
            AddExecutor(ExecutorType.Summon, Monsters.TimeThiefWinder);
            AddExecutor(ExecutorType.Summon, Monsters.TimeThiefBezelShip);
            AddExecutor(ExecutorType.Summon, Monsters.PerformTrickClown);
            AddExecutor(ExecutorType.Summon, Monsters.TimeThiefCronocorder);
            AddExecutor(ExecutorType.Summon, Monsters.ThunderKingRaiOh, ThunderKingRaiOhsummon);
            //xyz summons
            AddExecutor(ExecutorType.SpSummon, XYZs.GagagaCowboy, GagagaCowboySummon);
            AddExecutor(ExecutorType.SpSummon, XYZs.EvilswarmExcitonKnight, DefaultEvilswarmExcitonKnightSummon);
            AddExecutor(ExecutorType.SpSummon, XYZs.DarkRebellionXyzDragon, DarkRebellionXyzDragonSummon);
            AddExecutor(ExecutorType.SpSummon, XYZs.Number39Utopia, DefaultNumberS39UtopiaTheLightningSummon);
            AddExecutor(ExecutorType.SpSummon, XYZs.NumberS39UtopiatheLightning);
            AddExecutor(ExecutorType.SpSummon, XYZs.NumberS39UtopiaOne);
            AddExecutor(ExecutorType.SpSummon, XYZs.TimeThiefPerpetua, TimeThiefPerpetuaSummon);
            AddExecutor(ExecutorType.SpSummon, XYZs.TimeThiefRedoer, TimeThiefRedoerSummon);
            AddExecutor(ExecutorType.SpSummon, XYZs.TimeThiefPerpetua);
            //activate trap
            AddExecutor(ExecutorType.Activate, Traps.PhantomKnightsShade);
            AddExecutor(ExecutorType.Activate, Traps.TimeThiefRetrograte, RetrograteEffect);
            AddExecutor(ExecutorType.Activate, Traps.TimeThiefFlyBack, TimeThiefFlyBackEffect);
            AddExecutor(ExecutorType.Activate, Traps.SolemnWarning, DefaultSolemnWarning);
            AddExecutor(ExecutorType.Activate, Traps.SolemStrike, DefaultSolemnStrike);
            AddExecutor(ExecutorType.Activate, Traps.SolemnJudgment, DefaultSolemnJudgment);
            AddExecutor(ExecutorType.Activate, Traps.Crackdown, Crackdowneff);
            //xyz effects
            AddExecutor(ExecutorType.Activate, XYZs.TimeThiefRedoer, RedoerEffect);
            AddExecutor(ExecutorType.Activate, XYZs.TimeThiefPerpetua, PerpetuaEffect);
            AddExecutor(ExecutorType.Activate, XYZs.EvilswarmExcitonKnight, DefaultEvilswarmExcitonKnightEffect);
            AddExecutor(ExecutorType.Activate, XYZs.GagagaCowboy);
            AddExecutor(ExecutorType.Activate, XYZs.NumberS39UtopiatheLightning, DefaultNumberS39UtopiaTheLightningEffect);
            AddExecutor(ExecutorType.Activate, XYZs.DarkRebellionXyzDragon, DarkRebellionXyzDragonEffect);

            //monster effects
            AddExecutor(ExecutorType.Activate, Monsters.TimeThiefRegulator, RegulatorEffect);
            AddExecutor(ExecutorType.Activate, Monsters.TimeThiefWinder, TimeThiefWinderEffect);
            AddExecutor(ExecutorType.Activate, Monsters.TimeThiefCronocorder, TimeThiefCronocorderEffect);
            AddExecutor(ExecutorType.Activate, Monsters.PerformTrickClown, TrickClownEffect);
            AddExecutor(ExecutorType.Activate, Monsters.TimeThiefBezelShip, TimeThiefBezelShipEffect);
            AddExecutor(ExecutorType.Activate, Monsters.ThunderKingRaiOh, ThunderKingRaiOheff);
            AddExecutor(ExecutorType.Activate, Monsters.AshBlossomAndJoyousSpring, DefaultAshBlossomAndJoyousSpring);
            AddExecutor(ExecutorType.Activate, Monsters.MaxxC, DefaultMaxxC);
        }

        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, int hint, bool cancelable)
        {
            ClientCard currentChainCard = Duel.GetCurrentChainCard();
            ChainInfo solvingChain = Duel.GetCurrentSolvingChainInfo();

            if (currentChainCard != null && currentChainCard.Controller == 0)
            {
                if (currentChainCard.IsCode(XYZs.DarkRebellionXyzDragon) &&
                    cards.Any(card => card.Controller == 1 && card.Location == CardLocation.MonsterZone))
                {
                    ClientCard target = Util.GetProblematicEnemyMonster(0, true);
                    if (target == null || !cards.Contains(target))
                    {
                        target = cards
                            .Where(card => card.Controller == 1 && card.IsMonster())
                            .OrderByDescending(card => card.Attack)
                            .FirstOrDefault();
                    }
                    return SelectOneCard(target, cards, min, max);
                }

                if (hint == HintMsg.RemoveXyz && currentChainCard.IsCode(
                    Monsters.TimeThiefWinder,
                    Monsters.TimeThiefBezelShip,
                    XYZs.TimeThiefPerpetua,
                    XYZs.DarkRebellionXyzDragon))
                {
                    return SelectXyzDetachMaterial(cards, min, max);
                }

                if (hint == HintMsg.Target)
                {
                    if (currentChainCard.IsCode(
                        Spells.TimeThiefStartup,
                        Traps.TimeThiefFlyBack,
                        Monsters.TimeThiefBezelShip))
                    {
                        return SelectXyzToReceiveMaterial(cards, min, max);
                    }

                    if (currentChainCard.IsCode(XYZs.TimeThiefPerpetua))
                    {
                        if (cards.Any(card => card.Location == CardLocation.Grave))
                        {
                            int[] reviveOrder =
                            {
                                XYZs.TimeThiefRedoer,
                                Monsters.TimeThiefRegulator,
                                Monsters.TimeThiefWinder,
                                Monsters.TimeThiefBezelShip,
                                Monsters.TimeThiefCronocorder
                            };
                            List<ClientCard> selected = new List<ClientCard>();
                            foreach (int id in reviveOrder)
                            {
                                ClientCard target = cards.FirstOrDefault(card => card.IsCode(id));
                                if (target != null)
                                {
                                    selected.Add(target);
                                    break;
                                }
                            }
                            return Util.CheckSelectCount(selected, cards, min, max);
                        }

                        return SelectXyzToReceiveMaterial(cards, min, max);
                    }

                }

                if (hint == HintMsg.Faceup && currentChainCard.IsCode(Spells.TimeThiefHack))
                {
                    ClientCard target = cards
                        .Where(card => card.Controller == 0 && card.HasType(CardType.Xyz))
                        .OrderByDescending(card => card.Attack + card.Overlays.Count * 300)
                        .FirstOrDefault();
                    return SelectOneCard(target, cards, min, max);
                }
            }

            if (solvingChain != null && solvingChain.ActivatePlayer == 0)
            {
                if (solvingChain.IsActivateCode(Monsters.TimeThiefRegulator) &&
                    hint == HintMsg.SpSummon && min == 2)
                {
                    int[] summonOrder =
                    {
                        Monsters.TimeThiefWinder,
                        Monsters.TimeThiefBezelShip,
                        Monsters.TimeThiefCronocorder
                    };
                    List<ClientCard> selected = new List<ClientCard>();
                    foreach (int id in summonOrder)
                    {
                        ClientCard target = cards.FirstOrDefault(card =>
                            card.IsCode(id) && selected.All(selectedCard => selectedCard.Id != card.Id));
                        if (target != null)
                            selected.Add(target);
                        if (selected.Count >= max)
                            break;
                    }
                    foreach (ClientCard target in cards)
                    {
                        if (selected.Count >= max)
                            break;
                        if (selected.All(selectedCard => selectedCard.Id != target.Id))
                            selected.Add(target);
                    }
                    return Util.CheckSelectCount(selected, cards, min, max);
                }

                if (solvingChain.IsActivateCode(Monsters.TimeThiefWinder) && hint == HintMsg.AddToHand)
                {
                    int[] searchOrder = Bot.HasInMonstersZone(XYZs.TimeThiefRedoer)
                        ? new[]
                        {
                            Traps.TimeThiefRetrograte,
                            Traps.TimeThiefFlyBack,
                            Spells.TimeThiefStartup,
                            Monsters.TimeThiefBezelShip,
                            Monsters.TimeThiefCronocorder,
                            Spells.TimeThiefHack
                        }
                        : new[]
                        {
                            Spells.TimeThiefStartup,
                            Monsters.TimeThiefBezelShip,
                            Monsters.TimeThiefCronocorder,
                            Traps.TimeThiefRetrograte,
                            Traps.TimeThiefFlyBack,
                            Spells.TimeThiefHack
                        };
                    ClientCard target = searchOrder
                        .Select(id => cards.FirstOrDefault(card => card.IsCode(id)))
                        .FirstOrDefault(card => card != null);
                    return SelectOneCard(target, cards, min, max);
                }

                if (solvingChain.IsActivateCode(Spells.TimeThiefStartup))
                {
                    if (hint == HintMsg.SpSummon)
                    {
                        int[] summonOrder = Bot.GetMonsterCount() == 0
                            ? new[]
                            {
                                Monsters.TimeThiefRegulator,
                                Monsters.TimeThiefWinder,
                                Monsters.TimeThiefBezelShip,
                                Monsters.TimeThiefCronocorder
                            }
                            : new[]
                            {
                                Monsters.TimeThiefWinder,
                                Monsters.TimeThiefBezelShip,
                                Monsters.TimeThiefCronocorder,
                                Monsters.TimeThiefRegulator
                            };
                        ClientCard target = summonOrder
                            .Select(id => cards.FirstOrDefault(card => card.IsCode(id)))
                            .FirstOrDefault(card => card != null);
                        return SelectOneCard(target, cards, min, max);
                    }

                    if (hint == HintMsg.XyzMaterial && min == 3)
                    {
                        List<ClientCard> selected = new List<ClientCard>();
                        int[] monsterOrder =
                        {
                            Monsters.TimeThiefBezelShip,
                            Monsters.TimeThiefCronocorder,
                            Monsters.TimeThiefWinder,
                            Monsters.TimeThiefRegulator
                        };
                        ClientCard monster = monsterOrder
                            .Select(id => cards.FirstOrDefault(card => card.IsCode(id)))
                            .FirstOrDefault(card => card != null) ??
                            cards.FirstOrDefault(card => card.IsMonster());
                        ClientCard spell = cards.FirstOrDefault(card => card.IsCode(Spells.TimeThiefHack)) ??
                            cards.FirstOrDefault(card => card.IsSpell());
                        ClientCard trap = cards.FirstOrDefault(card => card.IsCode(Traps.TimeThiefRetrograte)) ??
                            cards.FirstOrDefault(card => card.IsCode(Traps.TimeThiefFlyBack)) ??
                            cards.FirstOrDefault(card => card.IsTrap());
                        if (monster != null) selected.Add(monster);
                        if (spell != null) selected.Add(spell);
                        if (trap != null) selected.Add(trap);
                        return Util.CheckSelectCount(selected, cards, min, max);
                    }
                }

                if (solvingChain.IsActivateCode(XYZs.TimeThiefRedoer))
                {
                    if (hint == HintMsg.RemoveXyz)
                    {
                        List<ClientCard> selected = new List<ClientCard>();
                        ClientCard trap = cards.FirstOrDefault(card => card.IsTrap());
                        ClientCard spell = cards.FirstOrDefault(card => card.IsSpell());
                        ClientCard monster = cards.FirstOrDefault(card => card.IsMonster());
                        bool needsProtection = ShouldUseRedoerMonsterMaterial(solvingChain.RelatedCard);
                        if (trap != null)
                            selected.Add(trap);
                        if (spell != null)
                            selected.Add(spell);
                        if (monster != null && needsProtection)
                            selected.Add(monster);
                        return Util.CheckSelectCount(selected, cards, min, max);
                    }

                    if (hint == HintMsg.ToDeck)
                    {
                        ClientCard target = Util.GetProblematicEnemyCard();
                        if (target == null || !cards.Contains(target))
                        {
                            target = cards
                                .OrderByDescending(card => card.IsMonster() ? card.GetDefensePower() : 0)
                                .FirstOrDefault();
                        }
                        return SelectOneCard(target, cards, min, max);
                    }
                }

                if (hint == HintMsg.XyzMaterial && solvingChain.IsActivateCode(
                    XYZs.TimeThiefPerpetua,
                    Traps.TimeThiefFlyBack,
                    Monsters.TimeThiefBezelShip))
                {
                    if (cards.Any(card => card.Controller == 1))
                    {
                        ClientCard target = Bot.HasInMonstersZone(XYZs.TimeThiefRedoer)
                            ? cards
                                .OrderByDescending(card => card.IsTrap())
                                .ThenByDescending(card => card.IsSpell())
                                .ThenByDescending(card => card.IsMonster())
                                .ThenByDescending(card => card.GetDefensePower())
                                .FirstOrDefault()
                            : cards
                                .OrderByDescending(card => card.IsMonsterDangerous())
                                .ThenByDescending(card => card.IsExtraCard())
                                .ThenByDescending(card => card.IsMonster())
                                .ThenByDescending(card => card.GetDefensePower())
                                .FirstOrDefault();
                        return SelectOneCard(target, cards, min, max);
                    }

                    return SelectMaterialToAttach(cards, min, max);
                }

                if (solvingChain.IsActivateCode(Traps.TimeThiefRetrograte) && hint == HintMsg.Faceup)
                {
                    return SelectXyzToReceiveMaterial(cards, min, max);
                }
            }

            return base.OnSelectCard(cards, min, max, hint, cancelable);
        }

        private IList<ClientCard> SelectXyzToReceiveMaterial(IList<ClientCard> cards, int min, int max)
        {
            ClientCard target = cards
                .Where(card => card.Controller == 0 && card.IsFaceup() && card.HasType(CardType.Xyz))
                .OrderByDescending(card => card.IsCode(XYZs.TimeThiefRedoer))
                .ThenByDescending(card => card.HasSetcode(0x126))
                .ThenBy(card => card.Overlays.Count)
                .FirstOrDefault();
            return SelectOneCard(target, cards, min, max);
        }

        public override IList<ClientCard> OnSelectXyzMaterial(IList<ClientCard> cards, int min, int max)
        {
            int[] materialOrder =
            {
                Monsters.PerformTrickClown,
                Monsters.TimeThiefBezelShip,
                Monsters.TimeThiefCronocorder,
                Monsters.TimeThiefWinder,
                Monsters.TimeThiefRegulator,
                Monsters.PhotonTrasher,
                Monsters.ThunderKingRaiOh
            };
            List<ClientCard> selected = materialOrder
                .Select(id => cards.FirstOrDefault(card => card.IsCode(id)))
                .Where(card => card != null)
                .Distinct()
                .ToList();
            return Util.CheckSelectCount(selected, cards, min, max);
        }

        private IList<ClientCard> SelectXyzDetachMaterial(IList<ClientCard> cards, int min, int max)
        {
            int[] detachOrder =
            {
                Monsters.TimeThiefBezelShip,
                Monsters.PerformTrickClown,
                Monsters.TimeThiefCronocorder,
                Monsters.TimeThiefRegulator,
                Monsters.TimeThiefWinder
            };
            List<ClientCard> selected = detachOrder
                .Select(id => cards.FirstOrDefault(card => card.IsCode(id)))
                .Where(card => card != null)
                .Distinct()
                .ToList();
            selected.AddRange(cards.Where(card =>
                !selected.Contains(card) && card.Owner == 0 && card.IsMonster()));
            selected.AddRange(cards.Where(card =>
                !selected.Contains(card) && card.IsMonster()));
            selected.AddRange(cards.Where(card =>
                !selected.Contains(card) && card.IsSpell()));
            selected.AddRange(cards.Where(card => !selected.Contains(card)));
            return Util.CheckSelectCount(selected, cards, min, max);
        }

        private IList<ClientCard> SelectMaterialToAttach(IList<ClientCard> cards, int min, int max)
        {
            int[] materialOrder =
            {
                Traps.TimeThiefFlyBack,
                Traps.TimeThiefRetrograte,
                Spells.TimeThiefStartup,
                Spells.TimeThiefHack,
                Monsters.TimeThiefBezelShip,
                Monsters.TimeThiefWinder,
                Monsters.TimeThiefCronocorder,
                Monsters.TimeThiefRegulator
            };
            ClientCard target = materialOrder
                .Select(id => cards.FirstOrDefault(card => card.IsCode(id)))
                .FirstOrDefault(card => card != null) ??
                cards.FirstOrDefault(card => card.IsTrap()) ??
                cards.FirstOrDefault(card => card.IsSpell()) ??
                cards.FirstOrDefault(card => card.IsMonster()) ??
                cards.FirstOrDefault();
            return SelectOneCard(target, cards, min, max);
        }

        private IList<ClientCard> SelectOneCard(ClientCard target, IList<ClientCard> cards, int min, int max)
        {
            List<ClientCard> selected = new List<ClientCard>();
            if (target != null)
                selected.Add(target);
            return Util.CheckSelectCount(selected, cards, min, max);
        }

        public void SelectSTPlace(ClientCard card = null, bool avoid_Impermanence = false, List<int> avoid_list = null)
        {
            List<int> list = new List<int> { 0, 1, 2, 3, 4 };
            Util.ShuffleListInPlace(list);
            foreach (int seq in list)
            {
                int zone = (int)System.Math.Pow(2, seq);
                if (Bot.SpellZone[seq] == null)
                {
                    if (card != null && card.Location == CardLocation.Hand &&
                        avoid_Impermanence && infiniteImpermanenceNegatedColumns.Contains(seq)) continue;
                    if (avoid_list != null && avoid_list.Contains(seq)) continue;
                    AI.SelectPlace(zone);
                    return;
                };
            }
            AI.SelectPlace(0);
        }

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
                if (Enemy.HasInSpellZone(CardId.RoyalDecreel, true) || Bot.HasInSpellZone(CardId.RoyalDecreel, true)) return true;
            }
            // how to get here?
            return false;
        }
        private bool SummonToDef()
        {
            AI.SelectPosition(CardPosition.Defence);
            return true;
        }
        private bool RegulatorEffect()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {
                if (DefaultCheckWhetherCardIsNegated(Card)) return false;
                return true;
            }

            if (Card.Location == CardLocation.Grave)
            {
                return true;
            }

            return false;
        }

        private bool TimeThiefPerpetuaSummon()
        {
            return Bot.HasInMonstersZone(XYZs.TimeThiefRedoer);
        }

        private bool TimeThiefRedoerSummon()
        {
            return !Bot.HasInMonstersZone(XYZs.TimeThiefRedoer);
        }

        private bool ShouldUseRedoerMonsterMaterial(ClientCard redoer)
        {
            if (redoer == null)
                return false;

            if (DefaultOnBecomeTarget(redoer) ||
                Duel.Player == 0 && Duel.Phase == DuelPhase.Main2)
            {
                return true;
            }

            ClientCard opponent = Enemy.BattlingMonster;
            if (Bot.BattlingMonster != redoer || opponent == null)
                return false;

            bool willBeDestroyed = Duel.Player == 0
                ? opponent.IsAttack() && opponent.Attack >= redoer.Attack
                : redoer.IsAttack()
                    ? opponent.Attack >= redoer.Attack
                    : opponent.Attack > redoer.Defense;
            if (!willBeDestroyed)
                return false;

            return Bot.GetMonsters().Any(card => card != redoer) ||
                Bot.LifePoints > opponent.Attack;
        }

        private bool PerpetuaEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card))
                return false;

            if (ActivateDescription == Util.GetStringId(XYZs.TimeThiefPerpetua, 0) ||
                Duel.Phase == DuelPhase.Standby)
            {
                return true;
            }

            if (ActivateDescription == Util.GetStringId(XYZs.TimeThiefPerpetua, 1) ||
                ActivateDescription == -1)
            {
                return Bot.GetMonsters().Any(card =>
                    card != Card &&
                    card.IsFaceup() &&
                    card.HasType(CardType.Xyz));
            }

            return false;
        }

        private bool RedoerEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card))
                return false;

            if (Duel.Phase == DuelPhase.Standby &&
                Util.GetStringId(XYZs.TimeThiefRedoer, 0) == ActivateDescription)
            {
                return true;
            }

            List<NamedCard> materials = Card.Overlays
                .Select(NamedCard.Get)
                .Where(material => material != null)
                .ToList();
            bool hasMonsterMaterial = materials.Any(material => material.HasType(CardType.Monster));
            bool hasSpellMaterial = materials.Any(material => material.HasType(CardType.Spell));
            bool hasTrapMaterial = materials.Any(material => material.HasType(CardType.Trap));

            if (hasMonsterMaterial && ShouldUseRedoerMonsterMaterial(Card))
                return true;

            if (hasTrapMaterial && Util.GetProblematicEnemyCard() != null)
                return true;

            if (hasSpellMaterial && Duel.Player == 0 &&
                (Duel.Phase == DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2))
                return true;

            return Duel.Player == 1 && Duel.Phase == DuelPhase.End &&
                (hasSpellMaterial || hasTrapMaterial);
        }

        private bool RetrograteEffect()
        {
            ClientCard lastChainCard = Util.GetLastChainCard();
            return Duel.LastChainPlayer == 1 &&
                lastChainCard != null &&
                (lastChainCard.IsSpell() || lastChainCard.IsTrap()) &&
                !DefaultTrapWillBeNegated();
        }

        private bool TimeThiefStartupEffect()
        {
            if (Card.Location == CardLocation.Hand)
            {
                return !DefaultSpellWillBeNegated();
            }

            if (Card.Location == CardLocation.Grave)
            {
                return true;
            }

            return false;
        }

        private bool TimeThiefHackEffect()
        {
            if (Card.Location == CardLocation.SpellZone && Card.IsFaceup())
            {
                return Duel.Player == 0 &&
                    Duel.Phase == DuelPhase.Main1 &&
                    Bot.GetMonsters().Any(card => card.IsFaceup() &&
                        card.HasType(CardType.Xyz) && card.Overlays.Count > 0);
            }

            return !DefaultSpellWillBeNegated() &&
                !Bot.HasInSpellZone(Spells.TimeThiefHack, true);
        }

        private bool TimeThiefFlyBackEffect()
        {
            if (Card.Location == CardLocation.Grave)
                return Enemy.Graveyard.Count > 0;

            return Card.Location == CardLocation.SpellZone &&
                !DefaultTrapWillBeNegated();
        }

        private bool TimeThiefWinderEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card))
                return false;

            if (Card.Location == CardLocation.Hand)
            {
                return Bot.GetMonsters().Any(card =>
                    card.IsFaceup() && card.HasType(CardType.Xyz) && card.Overlays.Count > 0);
            }

            return Card.Location == CardLocation.MonsterZone;
        }

        private bool TimeThiefBezelShipEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card))
                return false;

            if (Card.Location == CardLocation.Grave)
                return true;

            if (Card.Location == CardLocation.MonsterZone)
            {
                return DefaultOnBecomeTarget() ||
                    Duel.Player == 1 ||
                    Duel.Phase >= DuelPhase.Main2;
            }

            return false;
        }

        private bool TimeThiefCronocorderEffect()
        {
            if (Card.Location == CardLocation.Grave)
                return true;

            return Card.Location == CardLocation.MonsterZone &&
                !DefaultCheckWhetherCardIsNegated(Card) &&
                Bot.UnderAttack;
        }

        private bool FoolishBurialTarget()
        {
            if (Bot.LifePoints > 1000)
            {
                AI.SelectCard(
                    Monsters.PerformTrickClown,
                    Monsters.TimeThiefBezelShip,
                    Monsters.TimeThiefCronocorder,
                    Monsters.TimeThiefRegulator);
            }
            else
            {
                AI.SelectCard(
                    Monsters.TimeThiefBezelShip,
                    Monsters.TimeThiefCronocorder,
                    Monsters.TimeThiefRegulator);
            }
            return true;
        }

        private bool TrickClownEffect()
        {
            if (Bot.LifePoints <= 1000)
            {
                return false;
            }
            AI.SelectPosition(CardPosition.FaceUpDefence);
            return true;
        }
        private bool GagagaCowboySummon()
        {
            if (Enemy.LifePoints <= 800 || (Bot.GetMonsterCount() >= 4 && Enemy.LifePoints <= 1600))
            {
                AI.SelectPosition(CardPosition.FaceUpDefence);
                return true;
            }
            return false;
        }

        private bool DarkRebellionXyzDragonSummon()
        {
            int selfBestAttack = Util.GetBestAttack(Bot);
            int oppoBestAttack = Util.GetBestAttack(Enemy);
            return selfBestAttack <= oppoBestAttack;
        }

        private bool DarkRebellionXyzDragonEffect()
        {
            return Enemy.GetMonsters().Any(card =>
                card.IsFaceup() && !card.IsShouldNotBeTarget());
        }
        private bool ThunderKingRaiOhsummon()
        {
            if (Bot.MonsterZone[0] == null)
                AI.SelectPlace(Zones.z0);
            else
                AI.SelectPlace(Zones.z4);
            return true;
        }
        private bool ThunderKingRaiOheff()
        {
            if (DefaultOnlyHorusSpSummoning()) return false;
            ClientCard summonedCard = Duel.SummoningCards
                .FirstOrDefault(card => card.Controller == 1);
            return summonedCard != null &&
                (summonedCard.IsExtraCard() ||
                    summonedCard.IsMonsterDangerous() ||
                    summonedCard.Attack >= Card.Attack);
        }
        private bool Crackdowneff()
        {
            ClientCard target = Enemy.MonsterZone.GetFloodgate(true) ??
                Enemy.MonsterZone.GetDangerousMonster(true);
            if (target == null && (Bot.UnderAttack || DefaultOnBecomeTarget()))
                target = Util.GetOneEnemyBetterThanMyBest(true, true);
            if (target == null)
                return false;

            AI.SelectCard(target);
            return true;
        }
        private bool PotOfDesireseff()
        {
            return Bot.Deck.Count > 14 && !DefaultSpellWillBeNegated();
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


    }

}
