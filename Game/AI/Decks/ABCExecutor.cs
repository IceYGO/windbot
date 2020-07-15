using System;
using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using System.Linq;
using YGOSharp.Network.Enums;
using YGOSharp.OCGWrapper;

namespace WindBot.Game.AI.Decks
{
    [Deck("ABC", "AI_ABC")]
    class ABCExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int UnionDriver = 99249638;
            public const int GalaxySoldier = 46659709;
            public const int PhotonThrasher = 65367484;
            public const int AAssaultCore = 30012506;
            public const int BBusterDrake = 77411244;
            public const int CCrushWyvern = 3405259;
            public const int PhotonOrbital = 89132148;
            public const int PhotonVanisher = 43147039;
            public const int HeavyMechSupportArmor = 39890958;
            public const int AshBlossomAndJoyousSpring = 14558128;

            public const int ReinforcementOfTheArmy = 32807846;
            public const int Terraforming = 73628505;
            public const int UnauthorizedReactivation = 12524259;
            public const int CalledbyTheGrave = 24224830;
            public const int CrossOutDesignator = 65681983;
            public const int UnionHangar = 66399653;
            public const int InfiniteImpermanence = 10045474;
            public const int SolemnStrike = 40605147;

            public const int ABCDragonBuster = 1561110;
            public const int InvokedMechaba = 75286621;
            public const int CyberDragonInfinity = 10443957;
            public const int CyberDragonNova = 58069384;
            public const int CrystronHalqifibrax = 50588353;
            public const int UnionCarrier = 83152482;
            public const int IPMasquerena = 65741786;
            public const int CrusadiaAvramax = 21887175;
            public const int ApollousaBOG = 4280258;
            public const int KnightmareUnicorn = 38342335;
            public const int KnightmarePhoenix = 2857636;
            public const int KnightmareCerberus = 75452921;
        }

        public ABCExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            //#1 Interactions
            AddExecutor(ExecutorType.Activate, CardId.ApollousaBOG, ApollousaNegate);
            AddExecutor(ExecutorType.Activate, CardId.AshBlossomAndJoyousSpring, DefaultAshBlossomAndJoyousSpring);
            AddExecutor(ExecutorType.Activate, CardId.CalledbyTheGrave, DefaultCalledByTheGrave);
            AddExecutor(ExecutorType.Activate, CardId.InfiniteImpermanence, DefaultInfiniteImpermanence);
            AddExecutor(ExecutorType.Activate, CardId.SolemnStrike, DefaultSolemnStrike);
            AddExecutor(ExecutorType.Activate, CardId.CrossOutDesignator, CrossOutNegate);
            AddExecutor(ExecutorType.Activate, CardId.InvokedMechaba, CyberDragonInfinityNegate);
            AddExecutor(ExecutorType.Activate, CardId.CyberDragonInfinity, CyberDragonInfinityNegate);
            AddExecutor(ExecutorType.Activate, CardId.CyberDragonInfinity, CyberDragonInfinityAttach);
            AddExecutor(ExecutorType.Activate, CardId.CyberDragonNova, CyberDragonNovaFloat);
            AddExecutor(ExecutorType.Activate, CardId.IPMasquerena, IPMasquerenaEffect);
            AddExecutor(ExecutorType.Activate, CardId.ABCDragonBuster, ABCBanish);
            AddExecutor(ExecutorType.Activate, CardId.ABCDragonBuster, ABCUnionSummon);

            //#2 1st Searches, Equips and Photon summons
            AddExecutor(ExecutorType.Activate, CardId.ReinforcementOfTheArmy, ROTAEffect);
            AddExecutor(ExecutorType.Activate, CardId.UnionHangar, UnionHangarActivate);
            AddExecutor(ExecutorType.Activate, CardId.UnionHangar, UnionHangarEquip);
            AddExecutor(ExecutorType.SpSummon, CardId.PhotonThrasher, PhotonThrasherSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.PhotonVanisher, PhotonVanisherSummon);
            AddExecutor(ExecutorType.Activate, CardId.PhotonOrbital, PhotonOrbitalEquip);
            AddExecutor(ExecutorType.Activate, CardId.PhotonOrbital, PhotonOrbitalEffect);
            AddExecutor(ExecutorType.Activate, CardId.Terraforming, TerraformingEffect);
            AddExecutor(ExecutorType.Activate, CardId.UnauthorizedReactivation, UnauthorizedReactivationEffect);
            AddExecutor(ExecutorType.Activate, CardId.UnionDriver, UnionDriverEffect);

            //#3 Monster sets
            AddExecutor(ExecutorType.MonsterSet, CardId.CCrushWyvern, MonsterSet);
            AddExecutor(ExecutorType.MonsterSet, CardId.BBusterDrake, MonsterSet);
            AddExecutor(ExecutorType.MonsterSet, CardId.AAssaultCore, MonsterSet);

            //#4 Prioritized Summons and effects
            AddExecutor(ExecutorType.SpSummon, CardId.CyberDragonNova, CyberDragonNovaSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.CyberDragonInfinity, CyberDragonInfinitySummon);
            AddExecutor(ExecutorType.Summon, CardId.HeavyMechSupportArmor, HMSArmorSummon);
            AddExecutor(ExecutorType.Summon, CardId.AAssaultCore, AAssaultCoreSummon);
            AddExecutor(ExecutorType.Summon, CardId.BBusterDrake, BBusterDrakeSummon);
            AddExecutor(ExecutorType.Summon, CardId.CCrushWyvern, CCrushWyvernSummon);
            AddExecutor(ExecutorType.Activate, CardId.AAssaultCore, UnionSpSummon);
            AddExecutor(ExecutorType.Activate, CardId.BBusterDrake, UnionSpSummon);
            AddExecutor(ExecutorType.Activate, CardId.CCrushWyvern, UnionSpSummon);

            //#5 Knightmare Summons and effects (Turn 2+)
            AddExecutor(ExecutorType.SpSummon, CardId.KnightmareUnicorn, KnightmareUnicornSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.KnightmarePhoenix, KnightmarePhoenixSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.KnightmareCerberus, KnightmareCerberusSummon);
            AddExecutor(ExecutorType.Activate, CardId.KnightmareUnicorn, KnightmareUnicornEffect);
            AddExecutor(ExecutorType.Activate, CardId.KnightmarePhoenix, KnightmarePhoenixEffect);
            AddExecutor(ExecutorType.Activate, CardId.KnightmareCerberus, KnightmareCerberusEffect);

            //#6 Default Plays
            AddExecutor(ExecutorType.SpSummon, CardId.UnionCarrier, UnionCarrierSummon);
            AddExecutor(ExecutorType.Activate, CardId.UnionCarrier, UnionCarrierEffect);
            AddExecutor(ExecutorType.Activate, CardId.GalaxySoldier, GalaxySoldierSpSummon);
            AddExecutor(ExecutorType.Activate, CardId.GalaxySoldier, GalaxySoldierEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.ApollousaBOG, ApollousaSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.CrusadiaAvramax, CrusadiaAvramaxSummon);
            AddExecutor(ExecutorType.Activate, CardId.CrusadiaAvramax, CrusadiaAvramaxEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.IPMasquerena, IPMasquerenaSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.ABCDragonBuster, ABCDragonBusterSummon);

            //#7 Unions Effects
            AddExecutor(ExecutorType.Activate, CardId.CCrushWyvern, CCrushWyvernEffect);
            AddExecutor(ExecutorType.Activate, CardId.BBusterDrake, BBusterDrakeEffect);
            AddExecutor(ExecutorType.Activate, CardId.AAssaultCore, AAssaultCoreEffect);
            AddExecutor(ExecutorType.Activate, CardId.HeavyMechSupportArmor, HMSArmorEffect);
            AddExecutor(ExecutorType.Activate, CardId.HeavyMechSupportArmor, HMSArmorEquip);
            AddExecutor(ExecutorType.Activate, CardId.AAssaultCore, UnionEquip);
            AddExecutor(ExecutorType.Activate, CardId.BBusterDrake, UnionEquip);
            AddExecutor(ExecutorType.Activate, CardId.CCrushWyvern, UnionEquip);

            AddExecutor(ExecutorType.Repos, MonsterRepos);

            //#8 Spell/Trap sets
            AddExecutor(ExecutorType.SpellSet, CardId.CalledbyTheGrave, TrapSet);
            AddExecutor(ExecutorType.SpellSet, CardId.InfiniteImpermanence, TrapSet);
            AddExecutor(ExecutorType.SpellSet, CardId.CrossOutDesignator, TrapSet);
            AddExecutor(ExecutorType.SpellSet, CardId.SolemnStrike, TrapSet);


        }
        private bool UnionHangarActivated = false;
        private bool UnionHangarEquipped = false;
        private bool UnionDriverUsed = false;
        private bool PhotonOrbitalUsed = false;
        private bool GalaxySoldierUsed = false;
        private bool ABCBanishUsed = false;
        private bool ABCUnionSummonUsed = false;
        private bool UnionCarrierSummonTurn = false;
        private bool NormalSummonUsed = false;
        private bool HMSNormalUsed = false;

        public override bool OnSelectHand()
        {
            // go first
            return true;
        }

        public override void OnNewTurn()
        {
            UnionHangarActivated = false;
            UnionHangarEquipped = false;
            UnionDriverUsed = false;
            PhotonOrbitalUsed = false;
            GalaxySoldierUsed = false;
            ABCBanishUsed = false;
            ABCUnionSummonUsed = false;
            UnionCarrierSummonTurn = false;
            NormalSummonUsed = false;
            HMSNormalUsed = false;
        }

        private readonly int[] MonsterMassRemoval =
        {
            12580477,   //Raigeki
            53129443,   //Dark Hole
            14532163,   //Lightning Storm
            99330325,   //Interrupted Kaiju Slumber
            69162969,   //Lightning Vortex
            15693423,   //Evenly Matched
            53582587,   //Torrential Tribute
            8251996,    //Ojama Delta Hurricane!!
            44883830,   //Des Croaking
        };

        private readonly int[] ABCUnion =
        {
            CardId.AAssaultCore,
            CardId.BBusterDrake,
            CardId.CCrushWyvern
        };

        private readonly int[] discards =
        {
            CardId.AAssaultCore,
            CardId.BBusterDrake,
            CardId.CCrushWyvern,
            CardId.PhotonThrasher,
            CardId.UnionDriver,
        };

        private readonly int[] ExtraEquip =
        {
            CardId.CyberDragonInfinity,
            CardId.InvokedMechaba,
            CardId.ABCDragonBuster,
        };

        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, long hint, bool cancelable)
        {
            //Extra Deck millers
            int[] extradecksend =
            {
                87602890,   //Zaborg, The Mega Monarch
                95679145,   //Maximus Dragma
                82734805,   //Infernoid Tierra
                86062400,   //Xyz Avenger
                63737050,   //Ryu Okami

            };
            if (Util.GetLastChainCard() != null && Util.GetLastChainCard().IsCode(extradecksend) && Duel.LastChainPlayer != 0)
            {
                List<int> list = new List<int>();
                if (min >= 1)
                    list.Add(CardId.CyberDragonNova);
                if (min >= 2)
                    list.Add(CardId.CyberDragonInfinity);
                if (min >= 3)
                {
                    list.Add(CardId.ABCDragonBuster);
                    list.Add(CardId.CrusadiaAvramax);
                    list.Add(CardId.IPMasquerena);
                    list.Add(CardId.KnightmareCerberus);
                    list.Add(CardId.KnightmarePhoenix);
                    list.Add(CardId.KnightmareUnicorn);
                }
                int todrop = min;
                IList<ClientCard> result = new List<ClientCard>();
                IList<ClientCard> ToRemove = new List<ClientCard>(cards);
                List<int> record = new List<int>();
                foreach (ClientCard card in ToRemove)
                {
                    if (card?.Id != 0 && card.IsCode(list) && (!record.Contains(card.Id)))
                    {
                        record.Add(card.Id);
                        result.Add(card);
                        if (--todrop <= 0) break;
                    }
                }
                if (todrop <= 0) return result;
            }
            //Evenly Matched
            if (Util.GetLastChainCard() != null && Util.GetLastChainCard().IsCode(15693423) && Duel.LastChainPlayer != 0)
            {
                List<int> list = new List<int>();
                if (Bot.HasInMonstersZone(CardId.CrusadiaAvramax))
                    list.Add(CardId.CrusadiaAvramax);
                if (Bot.HasInMonstersZone(CardId.ApollousaBOG) && !Bot.HasInMonstersZone(CardId.CrusadiaAvramax))
                    list.Add(CardId.ApollousaBOG);
                if (Bot.HasInMonstersZone(CardId.ABCDragonBuster) && !Bot.HasInMonstersZone(CardId.ApollousaBOG) && !Bot.HasInMonstersZone(CardId.CrusadiaAvramax))
                    list.Add(CardId.ABCDragonBuster);
                if (Bot.HasInMonstersZone(CardId.CrusadiaAvramax) && !Bot.HasInMonstersZone(CardId.ABCDragonBuster) && !Bot.HasInMonstersZone(CardId.ApollousaBOG) && !Bot.HasInMonstersZone(CardId.CrusadiaAvramax))
                    list.Add(CardId.CyberDragonInfinity);
                int todrop = min;
                IList<ClientCard> result = new List<ClientCard>();
                IList<ClientCard> ToRemove = new List<ClientCard>(cards);
                List<int> record = new List<int>();
                foreach (ClientCard card in ToRemove)
                {
                    if (card?.Id != 0 && !card.IsCode(list))
                    {
                        result.Add(card);
                        if (--todrop <= 0) break;
                    }
                }
                if (todrop <= 0) return result;
            }
            return null;
        }

        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            YGOSharp.OCGWrapper.NamedCard cardData = YGOSharp.OCGWrapper.NamedCard.Get(cardId);
            if (cardData != null)
            {
                if (cardData.Attack <= 1000)
                    return CardPosition.FaceUpDefence;
                if (cardData.HasType(CardType.Union) && Duel.Player == 1)
                    return CardPosition.FaceUpDefence;
            }
            return 0;
        }

        public override int OnSelectPlace(long cardId, int player, CardLocation location, int available)
        {
            if (location == CardLocation.MonsterZone)
            {
                return available & ~Bot.GetLinkedZones();
            }
            return 0;
        }

        // update stats for battle prediction based on effects
        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            if (!defender.IsMonsterHasPreventActivationEffectInBattle())
            {
                if (attacker.IsCode(CardId.CrusadiaAvramax) && !attacker.IsDisabled() && defender.IsSpecialSummoned)
                    attacker.RealPower += defender.Attack;
            }
            return base.OnPreBattleBetween(attacker, defender);
        }

        private bool CrossOutNegate()
        {
            if (Duel.LastChainPlayer == 1)
            {
                ClientCard LastChainCard = Util.GetLastChainCard();
                if (LastChainCard.IsCode(CardId.InfiniteImpermanence))
                {
                    AI.SelectAnnounceID(CardId.InfiniteImpermanence);
                    return true;
                }
                if (LastChainCard.IsCode(CardId.AshBlossomAndJoyousSpring))
                {
                    AI.SelectAnnounceID(CardId.AshBlossomAndJoyousSpring);
                    return true;
                }
                if (LastChainCard.IsCode(CardId.CalledbyTheGrave))
                {
                    AI.SelectAnnounceID(CardId.CalledbyTheGrave);
                    return true;
                }
            }
            return false;
        }

        private bool ABCBanish()
        {
            if (ActivateDescription == Util.GetStringId(CardId.ABCDragonBuster, 1))
                return false;
            if (Duel.LastChainPlayer == 0 || ABCBanishUsed)
                return false;
            ClientCard target = Util.GetBestEnemyCard(canBeTarget: true);
            if (target == null || (target.IsFacedown() && Duel.Phase != DuelPhase.End && Duel.Player == 1) || ((target.HasType(CardType.Spell) || target.HasType(CardType.Trap)) && (!target.HasType(CardType.Equip) && !target.HasType(CardType.Continuous) && !target.HasType(CardType.Field))))
                return false;
            AI.SelectOption(0);
            if (Bot.HasInHand(ABCUnion))
                AI.SelectCard(ABCUnion);
            else AI.SelectCard(Card.Location = CardLocation.Hand);
            AI.SelectNextCard(target);
            ABCBanishUsed = true;
            return true;
        }

        private bool ABCUnionSummon()
        {
            ClientCard LastChainCard = Util.GetLastChainCard();
            if (ActivateDescription == Util.GetStringId(CardId.ABCDragonBuster, 0))
                return false;
            if (LastChainCard != null && LastChainCard.IsCode(CardId.ABCDragonBuster) && Duel.LastChainPlayer == 0)
                return false;
            if ((ABCBanishUsed && Duel.LastChainPlayer != 0) || (Duel.Phase == DuelPhase.End) || (Duel.Phase == DuelPhase.Battle && Bot.GetHandCount() == 0 && Enemy.GetMonsterCount() >= 1))
            {
                if (!Bot.HasInExtra(CardId.ABCDragonBuster) && Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(CardId.ABCDragonBuster)) == 1)
                    return false;
                ABCUnionSummonUsed = true;
                return true;
            }
            if (Duel.LastChainPlayer == 0 || !ABCBanishUsed)
            {
                if (LastChainCard != null && LastChainCard.IsCode(CardId.IPMasquerena) && Util.ChainContainsCard(MonsterMassRemoval))
                {
                    ABCUnionSummonUsed = true;
                    return true;
                }
                if (LastChainCard != null && LastChainCard.IsCode(MonsterMassRemoval) && !LastChainCard.IsCode(15693423) && !Bot.HasInMonstersZone(CardId.IPMasquerena, notDisabled: true))
                {
                    ABCUnionSummonUsed = true;
                    return true;
                }
                if (Util.IsChainTarget(Card) && Duel.LastChainPlayer == 1)//TODO: add a blacklist
                {
                    ABCUnionSummonUsed = true;
                    return true;
                }
                return false;
            }
            return false;
        }

        private bool CyberDragonInfinityNegate()
        {
            if (Duel.LastChainPlayer != 1)
                return false;
            int[] Blacklist =
            {
                74117290,   //Dark World Dealings
                74519184,   //Hand Destruction
                70368879,   //Upstart Goblin
                93946239,   //Into the Void
                73628505,   //Terraforming
                55010259,   //Gold Gadget
                29021114,   //Silver Gadget
                34408491,   //Beelze of the Diabolic Dragons
                8763963,    //Beelzeus of the Diabolic Dragons
            };
            ClientCard LastChainCard = Util.GetLastChainCard();
            if (LastChainCard.IsCode(Blacklist))
                return false;
            if (LastChainCard.IsCode(423585) && ActivateDescription == Util.GetStringId(423585, 0))
                return false;
            return true;
        }

        private bool CyberDragonInfinityAttach()
        {
            if (Duel.LastChainPlayer != -1)
                return false;
            ClientCard target = Util.GetBestEnemyMonster(true, true);
            if (target != null && target.IsAttack())
            {
                AI.SelectCard(target);
                return true;
            }
            return false;
        }

        private bool CyberDragonNovaFloat()
        {
            if (Card.Location != CardLocation.Grave)
                return false;
            return true;
        }

        private bool ApollousaNegate()
        {
            int[] Blacklist =
            {
                64734921,   //The Agent of Creation - Venus
                34408491,   //Beelze of the Diabolic Dragons
                8763963,    //Beelzeus of the Diabolic Dragons
                74586817,   //PSY-Framelord Omega
                29353756,   //ZW - Eagle Claw
            };
            int[] HandMZone =
            {
                93969023,   //Black Metal Dragon
                81471108,   //ZW - Tornado Bringer
                45082499,   //ZW - Lightning Blade
                2648201,    //ZW - Sleipnir Mail
                40941889,   //ZW - Asura Strike
                6330307,    //DZW - Chimera Clad
                14235211,   //Rider of the Storm Winds
                38210374,   //Explossum
                38601126,   //Robot Buster Destruction Sword
                2602411,    //Wizard Buster Destruction Sword
                76218313,   //Dragon Buster Destruction Sword
            };
            int[] Hand =
            {
                94573223,   //Inzektor Giga-Mantis
                21977828,   //Inzektor Giga-Weevil
                89132148,   //Photon Orbital
                76080032,   //ZW - Unicorn Spear
                87008374,   //ZW - Phoenix Bow
                12927849,   //SZW - Fenrir Sword
            };
            ClientCard LastChainCard = Util.GetLastChainCard();
            if (LastChainCard.IsCode(57774843) && (Duel.Phase == DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2))
                return false;   //Judgment Dragon
            if (LastChainCard.IsCode(423585) && ActivateDescription == Util.GetStringId(423585, 0))
                return false;   //Summoner Monk
            if (LastChainCard.IsCode(43218406) && (LastChainCard.Location == CardLocation.MonsterZone))
                return false;   //Water Gizmek
            if (LastChainCard.HasSetcode(0x11e) && (LastChainCard.Location == CardLocation.Hand))
                return false;   //Danger!
            if (LastChainCard.HasSetcode(0x109a) && (LastChainCard.Location == CardLocation.Hand || LastChainCard.Location == CardLocation.MonsterZone) && (Duel.Phase == DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2))
                return false;   //Superheavy Samurai Soul
            if (LastChainCard.IsCode(HandMZone) && (LastChainCard.Location == CardLocation.Hand || LastChainCard.Location == CardLocation.MonsterZone))
                return false;   //Equip effects from hand or monster zone
            if (LastChainCard.IsCode(Hand) && LastChainCard.Location == CardLocation.Hand)
                return false;   //Equip effects from hand
            if (LastChainCard.IsCode(Blacklist))
                return false;
            return true;
        }

        private bool KnightmarePhoenixSummon()
        {
            if (Duel.Turn == 1) return false;
            if (Enemy.GetSpellCount() == 0) return false;
            if (Enemy.GetSpellCount() < Enemy.GetMonsterCount() && Bot.HasInExtra(CardId.KnightmareCerberus)) return false;
            int[] materials = new[] {
                CardId.CCrushWyvern,
                CardId.BBusterDrake,
                CardId.AAssaultCore,
                CardId.PhotonThrasher,
                CardId.PhotonVanisher,
                CardId.GalaxySoldier,
                CardId.UnionDriver,
                CardId.HeavyMechSupportArmor,
            };
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials)) >= 2)
            {
                AI.SelectMaterials(materials);
                return true;
            }
            return false;
        }

        private bool KnightmareCerberusSummon()
        {
            if (Duel.Turn == 1) return false;
            if (Enemy.GetMonsterCount() == 0) return false;
            if (Enemy.GetMonsterCount() < Enemy.GetSpellCount() && Bot.HasInExtra(CardId.KnightmarePhoenix)) return false;
            ClientCard target = Util.GetBestEnemyMonster(true, true);
            int[] materials = new[] {
                CardId.CCrushWyvern,
                CardId.BBusterDrake,
                CardId.AAssaultCore,
                CardId.PhotonThrasher,
                CardId.PhotonVanisher,
                CardId.GalaxySoldier,
                CardId.UnionDriver,
                CardId.HeavyMechSupportArmor,
            };
            if (target != null && target.IsSpecialSummoned && target.Sequence < 5)
            {
                if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials)) >= 2)
                {
                    AI.SelectMaterials(materials);
                    return true;
                }
                return false;
            }
            return false;
        }

        private bool KnightmareUnicornSummon()
        {
            if (Duel.Turn == 1) return false;
            int[] materials = new[] {
                CardId.KnightmareCerberus,
                CardId.KnightmarePhoenix,
                CardId.CCrushWyvern,
                CardId.BBusterDrake,
                CardId.AAssaultCore,
                CardId.PhotonThrasher,
                CardId.PhotonVanisher,
                CardId.GalaxySoldier,
                CardId.UnionDriver,
                CardId.HeavyMechSupportArmor,
            };
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials)) >= 2 && (Bot.HasInMonstersZone(CardId.KnightmareCerberus) || Bot.HasInMonstersZone(CardId.KnightmarePhoenix)))
            {
                List<ClientCard> materials2 = new List<ClientCard>();
                List<ClientCard> bot_monster = Bot.GetMonsters();
                bot_monster.Sort(CardContainer.CompareCardLevel);
                int link_count = 0;
                foreach (ClientCard card in bot_monster)
                {
                    if (card.IsFacedown()) continue;
                    if (!materials2.Contains(card) && card.LinkCount <= 2 && card.IsCode(materials))
                    {
                        materials2.Add(card);
                        link_count += (card.HasType(CardType.Link)) ? card.LinkCount : 1;
                        if (link_count >= 3) break;
                    }
                }
                if (link_count >= 3)
                {
                    AI.SelectMaterials(materials2);
                    return true;
                }
            }
            return false;
        }

        private bool KnightmarePhoenixEffect()
        {
            ClientCard target = Util.GetBestEnemySpell();
            if (target == null)
                return false;
            if (Bot.HasInGraveyard(CardId.AAssaultCore) && Bot.HasInGraveyard(CardId.BBusterDrake) && Bot.HasInHand(CardId.CCrushWyvern))
                AI.SelectCard(CardId.CCrushWyvern);
            if (Bot.HasInGraveyard(CardId.BBusterDrake) && Bot.HasInGraveyard(CardId.CCrushWyvern) && Bot.HasInHand(CardId.AAssaultCore))
                AI.SelectCard(CardId.AAssaultCore);
            if (Bot.HasInGraveyard(CardId.CCrushWyvern) && Bot.HasInGraveyard(CardId.AAssaultCore) && Bot.HasInHand(CardId.BBusterDrake))
                AI.SelectCard(CardId.BBusterDrake);
            if (Bot.HasInHand(discards))
                AI.SelectCard(discards);
            else AI.SelectCard(Card.Location = CardLocation.Hand);
            AI.SelectNextCard(target);
            return true;
        }

        private bool KnightmareCerberusEffect()
        {
            ClientCard target = Util.GetBestEnemyMonster(true, true);
            if (target == null && !target.IsSpecialSummoned && target.Sequence >= 5)
                return false;
            if (Bot.HasInGraveyard(CardId.AAssaultCore) && Bot.HasInGraveyard(CardId.BBusterDrake) && Bot.HasInHand(CardId.CCrushWyvern))
                AI.SelectCard(CardId.CCrushWyvern);
            if (Bot.HasInGraveyard(CardId.BBusterDrake) && Bot.HasInGraveyard(CardId.CCrushWyvern) && Bot.HasInHand(CardId.AAssaultCore))
                AI.SelectCard(CardId.AAssaultCore);
            if (Bot.HasInGraveyard(CardId.CCrushWyvern) && Bot.HasInGraveyard(CardId.AAssaultCore) && Bot.HasInHand(CardId.BBusterDrake))
                AI.SelectCard(CardId.BBusterDrake);
            if (Bot.HasInHand(discards))
                AI.SelectCard(discards);
            else AI.SelectCard(Card.Location = CardLocation.Hand);
            AI.SelectNextCard(target);
            return true;
        }

        private bool KnightmareUnicornEffect()
        {
            ClientCard target = Util.GetBestEnemyCard(canBeTarget: true);
            if (target == null)
                return false;
            if (Bot.HasInGraveyard(CardId.AAssaultCore) && Bot.HasInGraveyard(CardId.BBusterDrake) && Bot.HasInHand(CardId.CCrushWyvern))
                AI.SelectCard(CardId.CCrushWyvern);
            if (Bot.HasInGraveyard(CardId.BBusterDrake) && Bot.HasInGraveyard(CardId.CCrushWyvern) && Bot.HasInHand(CardId.AAssaultCore))
                AI.SelectCard(CardId.AAssaultCore);
            if (Bot.HasInGraveyard(CardId.CCrushWyvern) && Bot.HasInGraveyard(CardId.AAssaultCore) && Bot.HasInHand(CardId.BBusterDrake))
                AI.SelectCard(CardId.BBusterDrake);
            if (Bot.HasInHand(discards))
                AI.SelectCard(discards);
            else AI.SelectCard(Card.Location = CardLocation.Hand);
            AI.SelectNextCard(target);
            return true;
        }

        private bool ROTAEffect()
        {
            if (Bot.HasInHandOrHasInMonstersZone(CardId.PhotonThrasher))
            {
                AI.SelectCard(CardId.PhotonVanisher);
                return true;
            }
            else AI.SelectCard(CardId.PhotonThrasher);
            return true;
        }


        private bool TerraformingEffect()
        {
            if (UnionHangarActivated)
                return false;
            AI.SelectCard(CardId.UnionHangar);
            return true;
        }

        private bool UnionHangarActivate()
        {
            if (ActivateDescription != Util.GetStringId(CardId.UnionHangar, 0))
                return false;
            if (UnionHangarActivated)
                return false;
            if (Bot.HasInGraveyard(ABCUnion) && !NormalSummonUsed && Duel.Player == 0)
                AI.SelectCard(CardId.HeavyMechSupportArmor);
            if (Bot.HasInHand(CardId.BBusterDrake) || Bot.HasInHand(CardId.AAssaultCore))
                AI.SelectCard(CardId.CCrushWyvern);
            else
                AI.SelectCard(CardId.BBusterDrake);
            UnionHangarActivated = true;
            return true;
        }

        //TODO: add when to equip other abc instead
        private bool UnionHangarEquip()
        {
            if (ActivateDescription == Util.GetStringId(CardId.UnionHangar, 0))
                return false;
            if (UnionHangarEquipped)
                return false;
            if (Duel.Player == 1)
                AI.SelectCard(ABCUnion);
            if (!ABCUnionSummonUsed && !UnionDriverUsed && Duel.Player == 0)
                AI.SelectCard(CardId.UnionDriver);
            UnionHangarEquipped = true;
            return true;
        }

        private bool PhotonThrasherSummon()
        {
            int[] cards =
            {
                CardId.AAssaultCore,
                CardId.BBusterDrake,
                CardId.CCrushWyvern,
                CardId.UnionHangar,
                CardId.Terraforming,
            };
            if (!Bot.HasInHand(cards) && Bot.HasInHand(CardId.GalaxySoldier))
                return false;
            return true;
        }

        private bool PhotonVanisherSummon()
        {
            int[] mats =
            {
                CardId.AAssaultCore,
                CardId.BBusterDrake,
                CardId.CCrushWyvern,
                CardId.PhotonThrasher,
            };
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(mats)) >= 2 || Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(mats)) == 0 || Bot.HasInMonstersZone(CardId.IPMasquerena))
                return false;
            return true;
        }

        private bool PhotonOrbitalEquip()
        {
            if (Card.Location == CardLocation.Hand)
                return true;
            return false;
        }

        private bool HMSArmorSummon()
        {
            if (!Bot.HasInHandOrInGraveyard(ABCUnion) && !Bot.HasInHand(CardId.GalaxySoldier) && (Bot.HasInMonstersZone(CardId.PhotonThrasher) || Bot.HasInHand(CardId.UnauthorizedReactivation) || Bot.HasInSpellZone(CardId.UnionHangar, notDisabled: true)))
            {
                NormalSummonUsed = true;
                return true;
            }
            if (Bot.HasInGraveyard(ABCUnion) && Bot.GetMonstersInMainZone().Count() < 4)
            {
                NormalSummonUsed = true;
                return true;
            }
            return false;
        }

        private bool CCrushWyvernSummon()
        {
            NormalSummonUsed = true;
            return true;
        }

        private bool BBusterDrakeSummon()
        {
            if (Bot.HasInHand(CardId.CCrushWyvern))
                return false;
            NormalSummonUsed = true;
            return true;
        }

        private bool AAssaultCoreSummon()
        {
            if (Bot.HasInHand(CardId.CCrushWyvern) || Bot.HasInHand(CardId.BBusterDrake))
                return false;
            NormalSummonUsed = true;
            return true;
        }

        private bool UnauthorizedReactivationEffect()
        {
            //TODO: add when to equip other abc instead
            if (Duel.LastChainPlayer == 0)
                return false;
            if (!UnionDriverUsed && !Bot.HasInHandOrInSpellZone(CardId.UnionHangar))
            {
                AI.SelectCard(CardLocation.MonsterZone);
                AI.SelectNextCard(CardId.UnionDriver);
                return true;
            }
            if (Bot.HasInMonstersZone(CardId.CyberDragonInfinity))
            {
                AI.SelectCard(CardId.CyberDragonInfinity);
                {
                    if (Bot.GetRemainingCount(CardId.HeavyMechSupportArmor, 1) != 0)
                        AI.SelectNextCard(CardId.HeavyMechSupportArmor);
                    else AI.SelectNextCard(CardId.AAssaultCore);
                }
                return true;
            }
            return false;
        }

        //NOTE: needs even better logic
        private bool UnionDriverEffect()
        {
            if (UnionDriverUsed || Card.Location != CardLocation.SpellZone)
                return false;
            if (Bot.HasInHand(CardId.AAssaultCore) || Bot.HasInHand(CardId.BBusterDrake))
                AI.SelectCard(CardId.CCrushWyvern);
            if (Bot.HasInMonstersZone(CardId.CCrushWyvern) && Bot.HasInHand(CardId.BBusterDrake) && Bot.HasInHand(CardId.AAssaultCore))
                AI.SelectCard(CardId.CCrushWyvern);
            if (Bot.HasInMonstersZone(CardId.CCrushWyvern))
                AI.SelectCard(CardId.BBusterDrake);
            if (!Bot.HasInMonstersZoneOrInGraveyard(CardId.AAssaultCore) && (Bot.HasInMonstersZone(CardId.BBusterDrake) && !Bot.HasInHand(CardId.AAssaultCore)))
                AI.SelectCard(CardId.AAssaultCore);
            if (!Bot.HasInHandOrHasInMonstersZone(CardId.BBusterDrake))
                AI.SelectCard(CardId.BBusterDrake);
            UnionDriverUsed = true;
            return true;
        }

        private bool UnionSpSummon()
        {
            if (Card.Location == CardLocation.SpellZone)
                return true;
            return false;
        }

        private bool UnionCarrierSummon()
        {
            if (Bot.HasInMonstersZone(CardId.UnionCarrier, true))
                return false;

            int[] materials = new[] {
                CardId.CCrushWyvern,
                CardId.BBusterDrake,
                CardId.AAssaultCore,
                CardId.PhotonThrasher,
                CardId.PhotonVanisher,
                CardId.HeavyMechSupportArmor,
                CardId.UnionDriver,
                CardId.GalaxySoldier,
            };
            //NOTE: need to allow them if they are Equipped by effects other than union driver
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials)) >= 2)
            {
                AI.SelectMaterials(materials);
                UnionCarrierSummonTurn = true;
                return true;
            }
            return false;
        }

        private bool CrusadiaAvramaxSummon()
        {
            int[] materials = new[] {
                CardId.IPMasquerena,
                CardId.UnionCarrier,
                CardId.KnightmareCerberus,
                CardId.KnightmarePhoenix,
                CardId.KnightmareUnicorn,
            };
            if ((UnionCarrierSummonTurn && Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials)) == 2) || Duel.Turn == 1)
                return false;
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials)) >= 2)
            {
                List<ClientCard> materials2 = new List<ClientCard>();
                List<ClientCard> bot_monster = Bot.GetMonsters();
                bot_monster.Sort(CardContainer.CompareCardLevel);
                int link_count = 0;
                foreach (ClientCard card in bot_monster)
                {
                    if (card.IsFacedown()) continue;
                    if (!materials2.Contains(card) && card.LinkCount <= 3 && card.IsCode(materials))
                    {
                        materials2.Add(card);
                        link_count += (card.HasType(CardType.Link)) ? card.LinkCount : 1;
                        if (link_count >= 4) break;
                    }
                }
                if (link_count >= 4)
                {
                    AI.SelectMaterials(materials2);
                    return true;
                }
            }
            return false;
        }

        private bool CrusadiaAvramaxEffect()
        {
            ClientCard target = Util.GetBestEnemyCard();
            if (target == null)
                return false;
            AI.SelectNextCard(target);
            return true;
        }

        private bool ApollousaSummon()
        {
            if (Duel.Turn == 1)
                return false;
            int[] materials = new[] {
                CardId.CCrushWyvern,
                CardId.BBusterDrake,
                CardId.AAssaultCore,
                CardId.IPMasquerena,
                CardId.UnionCarrier,
                CardId.PhotonThrasher,
                CardId.PhotonVanisher,
                CardId.GalaxySoldier,
                CardId.UnionDriver,
                CardId.HeavyMechSupportArmor,
            };
            if ((Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials)) >= 3 && Bot.HasInMonstersZone(CardId.IPMasquerena)) || Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials)) >= 4)//Needs to check for different names?
            {
                AI.SelectMaterials(materials);
                return true;
            }
            return false;
        }

        private bool IPMasquerenaSummon()
        {
            if (!UnionCarrierSummonTurn && Duel.Turn == 1)
                return false;
            int[] materials = new[] {
                CardId.CCrushWyvern,
                CardId.BBusterDrake,
                CardId.AAssaultCore,
                CardId.PhotonThrasher,
                CardId.PhotonVanisher,
                CardId.GalaxySoldier,
                CardId.UnionDriver,
                CardId.HeavyMechSupportArmor,
            };
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials)) >= 2)
            {
                AI.SelectMaterials(materials);
                return true;
            }
            return false;
        }

        private bool IPMasquerenaEffect()
        {
            int[] materials1 = new[] {
                CardId.CCrushWyvern,
                CardId.BBusterDrake,
                CardId.AAssaultCore,
                CardId.IPMasquerena,
            };
            int[] materials2 = new[] {
                CardId.IPMasquerena,
                CardId.UnionCarrier,
                CardId.KnightmareCerberus,
                CardId.KnightmarePhoenix,
                CardId.KnightmareUnicorn,
            };
            if (Duel.LastChainPlayer == 0)
                return false;
            ClientCard LastChainCard = Util.GetLastChainCard();
            if (LastChainCard != null && LastChainCard.IsCode(MonsterMassRemoval))
            {
                if (Bot.HasInMonstersZone(CardId.ABCDragonBuster))
                {
                    AI.SelectCard(CardId.ApollousaBOG);
                    AI.SelectMaterials(materials1);
                    return true;
                }
                if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials2)) >= 2 && !Bot.HasInMonstersZone(CardId.ABCDragonBuster) && !ABCUnionSummonUsed)
                {
                    AI.SelectCard(CardId.CrusadiaAvramax);
                    AI.SelectMaterials(materials2);
                    return true;
                }
                return false;
            }
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials1)) >= 3 && ABCUnionSummonUsed)
            {
                AI.SelectCard(CardId.ApollousaBOG);
                AI.SelectMaterials(materials1);
                return true;
            }
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials2)) >= 2 && ((!Bot.HasInMonstersZone(CardId.ABCDragonBuster) && !ABCUnionSummonUsed) || (Util.IsChainTarget(Card) && Duel.LastChainPlayer == 1)))
            {
                AI.SelectCard(CardId.CrusadiaAvramax);
                AI.SelectMaterials(materials2);
                return true;
            }
            return false;
        }

        private bool CyberDragonNovaSummon()
        {
            int[] materials = new[] {
                CardId.GalaxySoldier,
                CardId.UnionDriver
            };
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials)) >= 2)
            {
                AI.SelectMaterials(materials);
                return true;
            }
            return false;
        }

        private bool CyberDragonInfinitySummon()
        {
            AI.SelectMaterials(CardId.CyberDragonNova);
            return true;
        }

        private bool ABCDragonBusterSummon()
        {
            if (Bot.Graveyard.GetMatchingCardsCount(card => card.IsCode(ABCUnion)) >= 3)
                AI.SelectMaterials(CardLocation.Grave);
            else AI.SelectMaterials(CardLocation.MonsterZone);
            return true;
        }

        private bool AAssaultCoreEffect()
        {
            if (Card.Location != CardLocation.Grave)
                return false;
            if (!NormalSummonUsed && Bot.HasInGraveyard(CardId.HeavyMechSupportArmor))
            {
                AI.SelectCard(CardId.HeavyMechSupportArmor);
                return true;
            }
            if (Bot.HasInMonstersZone(CardId.IPMasquerena) && (!Bot.HasInHand(CardId.GalaxySoldier) || GalaxySoldierUsed))
            {
                if (Bot.Graveyard.GetCardCount(CardId.CCrushWyvern) >= 2)
                {
                    AI.SelectCard(CardId.CCrushWyvern);
                    return true;
                }
                if (Bot.Graveyard.GetCardCount(CardId.BBusterDrake) >= 2)
                {
                    AI.SelectCard(CardId.BBusterDrake);
                    return true;
                }
                if (Bot.Graveyard.GetCardCount(CardId.AAssaultCore) >= 2)
                {
                    AI.SelectCard(CardId.AAssaultCore);
                    return true;
                }
                return false;
            }
            AI.SelectCard(CardId.CCrushWyvern);
            AI.SelectCard(CardId.BBusterDrake);
            return true;
        }

        private bool BBusterDrakeEffect()
        {
            if (Card.Location == CardLocation.Grave)
            {
                if (!NormalSummonUsed)
                    AI.SelectCard(CardId.HeavyMechSupportArmor);
                if (!Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.CCrushWyvern))
                    AI.SelectCard(CardId.CCrushWyvern);
                if (!Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.AAssaultCore))
                    AI.SelectCard(CardId.AAssaultCore);
                if (Bot.HasInGraveyard(CardId.CCrushWyvern) && Bot.HasInGraveyard(CardId.AAssaultCore))
                    AI.SelectCard(CardId.BBusterDrake);
                return true;
            }
            return false;
        }

        private bool CCrushWyvernEffect()
        {
            if (Card.Location == CardLocation.Grave)
            {
                if (!Bot.HasInMonstersZoneOrInGraveyard(CardId.BBusterDrake) && Bot.HasInHand(CardId.BBusterDrake))
                    AI.SelectCard(CardId.BBusterDrake);
                if (!Bot.HasInMonstersZoneOrInGraveyard(CardId.AAssaultCore) && Bot.HasInHand(CardId.AAssaultCore))
                    AI.SelectCard(CardId.AAssaultCore);
                if (Bot.HasInMonstersZoneOrInGraveyard(CardId.BBusterDrake) && Bot.HasInMonstersZoneOrInGraveyard(CardId.AAssaultCore))
                    AI.SelectCard(CardId.CCrushWyvern);
                else AI.SelectCard(ABCUnion);
                return true;
            }
            return false;
        }

        private bool HMSArmorEffect()
        {
            if (Card.Location == CardLocation.MonsterZone && !HMSNormalUsed)
            {
                if ((Bot.HasInHand(CardId.BBusterDrake) || Bot.HasInHand(CardId.AAssaultCore)) && Bot.HasInGraveyard(CardId.CCrushWyvern))
                    AI.SelectCard(CardId.CCrushWyvern);
                if (Bot.HasInGraveyard(CardId.BBusterDrake))
                    AI.SelectCard(CardId.BBusterDrake);
                if (Bot.HasInGraveyard(CardId.AAssaultCore))
                    AI.SelectCard(CardId.AAssaultCore);
                else AI.SelectCard(ABCUnion);
                HMSNormalUsed = true;
                return true;
            }
            return false;
        }

        private bool HMSArmorEquip()
        {
            if (Card.Location == CardLocation.MonsterZone && HMSNormalUsed && Bot.HasInMonstersZone(ExtraEquip))
            {
                if (Bot.HasInMonstersZone(CardId.CyberDragonInfinity))
                    AI.SelectCard(CardId.CyberDragonInfinity);
                if (Bot.HasInMonstersZone(CardId.InvokedMechaba))
                    AI.SelectCard(CardId.InvokedMechaba);
                if (Bot.HasInMonstersZone(CardId.ABCDragonBuster))
                    AI.SelectCard(CardId.ABCDragonBuster);
                return true;
            }
            return false;
        }

        private bool UnionEquip()
        {
            if (Card.Location == CardLocation.MonsterZone && Util.IsTurn1OrMain2() && Bot.HasInMonstersZone(ExtraEquip))
            {
                if (Bot.HasInMonstersZone(CardId.CyberDragonInfinity))
                    AI.SelectCard(CardId.CyberDragonInfinity);
                if (Bot.HasInMonstersZone(CardId.InvokedMechaba))
                    AI.SelectCard(CardId.InvokedMechaba);
                if (Bot.HasInMonstersZone(CardId.ABCDragonBuster))
                    AI.SelectCard(CardId.ABCDragonBuster);
                return true;
            }
            return false;
        }

        private bool UnionCarrierEffect()
        {
            if (Bot.HasInMonstersZone(CardId.CyberDragonInfinity))
            {
                AI.SelectCard(CardId.CyberDragonInfinity);
                {
                    if (Bot.GetRemainingCount(CardId.HeavyMechSupportArmor, 1) != 0)
                        AI.SelectNextCard(CardId.HeavyMechSupportArmor);
                    else AI.SelectNextCard(CardId.AAssaultCore);
                }
                return true;
            }
            if (!UnionDriverUsed || !PhotonOrbitalUsed)
            {
                int[] mats =
                {
                    CardId.AAssaultCore,
                    CardId.BBusterDrake,
                    CardId.CCrushWyvern,
                    CardId.PhotonThrasher,
                };
                AI.SelectCard(CardId.UnionCarrier);
                if (!UnionDriverUsed)
                {
                    AI.SelectNextCard(CardId.UnionDriver);
                    return true;
                }
                if (!PhotonOrbitalUsed)
                {
                    if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(mats)) >= 2 && (Bot.HasInHand(CardId.GalaxySoldier) || GalaxySoldierUsed))
                        return false;
                    AI.SelectNextCard(CardId.PhotonOrbital);
                    return true;
                }
                return false;
            }
            return false;
        }

        private bool PhotonOrbitalEffect()
        {
            if (Card.Location == CardLocation.SpellZone)
            {
                if (Bot.HasInHand(CardId.GalaxySoldier))
                    AI.SelectCard(CardId.PhotonVanisher);
                else AI.SelectCard(CardId.GalaxySoldier);
                PhotonOrbitalUsed = true;
                return true;
            }
            return false;
        }

        private bool GalaxySoldierSpSummon()
        {
            if (GalaxySoldierUsed && !Bot.HasInMonstersZone(CardId.GalaxySoldier) && Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(discards)) != 1)
                return false;
            if (Card.Location == CardLocation.Hand)
            {
                if (Bot.HasInGraveyard(CardId.AAssaultCore) && Bot.HasInGraveyard(CardId.BBusterDrake) && Bot.HasInHand(CardId.CCrushWyvern))
                {
                    AI.SelectCard(CardId.CCrushWyvern);
                    return true;
                }
                if (Bot.HasInGraveyard(CardId.BBusterDrake) && Bot.HasInGraveyard(CardId.CCrushWyvern) && Bot.HasInHand(CardId.AAssaultCore))
                {
                    AI.SelectCard(CardId.AAssaultCore);
                    return true;
                }
                if (Bot.HasInGraveyard(CardId.CCrushWyvern) && Bot.HasInGraveyard(CardId.AAssaultCore) && Bot.HasInHand(CardId.BBusterDrake))
                {
                    AI.SelectCard(CardId.BBusterDrake);
                    return true;
                }
                if (Bot.HasInHand(CardId.AAssaultCore) || Bot.HasInHand(CardId.BBusterDrake) || Bot.HasInHand(CardId.CCrushWyvern) || Bot.HasInHand(CardId.PhotonThrasher) || Bot.HasInHand(CardId.UnionDriver))
                {
                    AI.SelectCard(discards);
                    return true;
                }
                if (!Bot.HasInHandOrHasInMonstersZone(ABCUnion) && (Bot.Hand.GetMatchingCardsCount(card => card.HasAttribute(CardAttribute.Light)) >= 3 || (Bot.Hand.GetMatchingCardsCount(card => card.HasAttribute(CardAttribute.Light)) >= 2 && Bot.HasInMonstersZone(CardId.GalaxySoldier))))
                {
                    return true;
                }
            }
            return false;
        }

        private bool GalaxySoldierEffect()
        {
            if (Card.Location == CardLocation.MonsterZone && !GalaxySoldierUsed)
            {
                AI.SelectCard(CardId.GalaxySoldier);
                GalaxySoldierUsed = true;
                return true;
            }
            return false;
        }

        private bool MonsterSet()
        {
            if (!Bot.HasInHandOrInSpellZone(CardId.UnionHangar) && !Bot.HasInHand(CardId.UnauthorizedReactivation) && !Bot.HasInHand(CardId.GalaxySoldier) && Bot.GetMonsterCount() == 0 && (Duel.Turn == 1 || (Duel.Turn != 1 && Enemy.GetMonsterCount() >= 1)))
                return true;
            return false;
        }

        private bool TrapSet()
        {
            if ((Bot.HasInMonstersZone(CardId.ABCDragonBuster) || Bot.HasInMonstersZone(CardId.InvokedMechaba)) && Bot.GetHandCount() == 1)
                return false;
            if (Util.IsTurn1OrMain2() || Bot.GetMonsterCount() == 0)
            {
                AI.SelectPlace(Zones.z0 + Zones.z1 + Zones.z3 + Zones.z4);
                return true;
            }
            return false;
        }

        private bool MonsterRepos()
        {
            if (Card.IsFacedown())
                return true;
            return DefaultMonsterRepos();
        }
    }
}
