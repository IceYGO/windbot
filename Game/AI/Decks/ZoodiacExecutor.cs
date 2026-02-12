using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("Zoodiac", "AI_Zoodiac")]
    class ZoodiacExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int JizukirutheStarDestroyingKaiju = 63941210;
            public const int GadarlatheMysteryDustKaiju = 36956512;
            public const int GamecieltheSeaTurtleKaiju = 55063751;
            public const int RadiantheMultidimensionalKaiju = 28674152;
            public const int KumongoustheStickyStringKaiju = 29726552;
            public const int PhotonThrasher = 65367484;
            public const int Thoroughblade = 77150143;
            public const int Whiptail = 31755044;
            public const int Ratpier = 78872731;
            public const int AleisterTheInvoker = 86120751;

            public const int HarpiesFeatherDuster = 18144506;
            public const int DarkHole = 53129443;
            public const int Terraforming = 73628505;
            public const int Invocation = 74063034;
            public const int MonsterReborn = 83764718;
            public const int InterruptedKaijuSlumber = 99330325;
            public const int ZoodiacBarrage = 46060017;
            public const int FireFormationTenki = 57103969;
            public const int MagicalMeltdown = 47679935;
            public const int ZoodiacCombo = 73881652;

            public const int InvokedMechaba = 75286621;
            public const int InvokedMagellanica = 48791583;
            public const int NumberS39UtopiatheLightning = 56832966;
            public const int Number39Utopia = 84013237;
            public const int DaigustoEmeral = 581014;
            public const int Tigermortar = 11510448;
            public const int Chakanine = 41375811;
            public const int Drident = 48905153;
            public const int Broadbull = 85115440;
        }

        bool TigermortarSpsummoned = false;
        bool ChakanineSpsummoned = false;
        bool BroadbullSpsummoned = false;

        public ZoodiacExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // Quick spells
            AddExecutor(ExecutorType.Activate, CardId.HarpiesFeatherDuster);
            AddExecutor(ExecutorType.Activate, CardId.InterruptedKaijuSlumber, DefaultInterruptedKaijuSlumber);
            AddExecutor(ExecutorType.Activate, CardId.DarkHole, DefaultDarkHole);

            AddExecutor(ExecutorType.SpSummon, CardId.GamecieltheSeaTurtleKaiju, DefaultKaijuSpsummon);
            AddExecutor(ExecutorType.SpSummon, CardId.KumongoustheStickyStringKaiju, DefaultKaijuSpsummon);
            AddExecutor(ExecutorType.SpSummon, CardId.GadarlatheMysteryDustKaiju, DefaultKaijuSpsummon);
            AddExecutor(ExecutorType.SpSummon, CardId.RadiantheMultidimensionalKaiju, DefaultKaijuSpsummon);
            AddExecutor(ExecutorType.SpSummon, CardId.JizukirutheStarDestroyingKaiju, DefaultKaijuSpsummon);

            AddExecutor(ExecutorType.Activate, CardId.Terraforming);
            AddExecutor(ExecutorType.Activate, CardId.MagicalMeltdown);
            AddExecutor(ExecutorType.Activate, CardId.FireFormationTenki, FireFormationTenkiEffect);
            AddExecutor(ExecutorType.Activate, CardId.ZoodiacBarrage, ZoodiacBarrageEffect);
            AddExecutor(ExecutorType.Activate, CardId.DaigustoEmeral, DaigustoEmeralEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.PhotonThrasher, PhotonThrasherSummon);

            AddExecutor(ExecutorType.SpSummon, CardId.Number39Utopia, DefaultNumberS39UtopiaTheLightningSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.NumberS39UtopiatheLightning);
            AddExecutor(ExecutorType.Activate, CardId.NumberS39UtopiatheLightning, DefaultNumberS39UtopiaTheLightningEffect);

            AddExecutor(ExecutorType.Activate, CardId.InvokedMechaba, DefaultTrap);

            AddExecutor(ExecutorType.Activate, RatpierMaterialEffect);

            AddExecutor(ExecutorType.Activate, CardId.Drident, DridentEffect);
            AddExecutor(ExecutorType.Activate, CardId.Broadbull, BroadbullEffect);
            AddExecutor(ExecutorType.Activate, CardId.Tigermortar, TigermortarEffect);
            AddExecutor(ExecutorType.Activate, CardId.Chakanine, ChakanineEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.Chakanine, ChakanineSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.Tigermortar, TigermortarSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.Broadbull, BroadbullSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.Drident, DridentSummon);

            AddExecutor(ExecutorType.Summon, CardId.Ratpier);
            AddExecutor(ExecutorType.Activate, CardId.Ratpier, RatpierEffect);
            AddExecutor(ExecutorType.Summon, CardId.Thoroughblade);
            AddExecutor(ExecutorType.Activate, CardId.Thoroughblade, RatpierEffect);
            AddExecutor(ExecutorType.Summon, CardId.AleisterTheInvoker);
            AddExecutor(ExecutorType.Activate, CardId.AleisterTheInvoker, AleisterTheInvokerEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.DaigustoEmeral, DaigustoEmeralSummon);

            AddExecutor(ExecutorType.SpSummon, CardId.Broadbull, BroadbullXYZSummon);

            AddExecutor(ExecutorType.Activate, CardId.MonsterReborn, MonsterRebornEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.PhotonThrasher);
            AddExecutor(ExecutorType.Summon, CardId.Whiptail);

            AddExecutor(ExecutorType.Activate, CardId.Invocation, InvocationEffect);

            AddExecutor(ExecutorType.Activate, CardId.Whiptail, WhiptailEffect);

            AddExecutor(ExecutorType.Activate, CardId.ZoodiacCombo, ZoodiacComboEffect);

            AddExecutor(ExecutorType.SpellSet, CardId.ZoodiacCombo);

            AddExecutor(ExecutorType.Repos, MonsterRepos);
        }

        public override bool OnSelectHand()
        {
            // go first
            return true;
        }

        public override void OnNewTurn()
        {
            // reset
            TigermortarSpsummoned = false;
            ChakanineSpsummoned = false;
            BroadbullSpsummoned = false;
            base.OnNewTurn();
        }

        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            if (!defender.IsMonsterHasPreventActivationEffectInBattle())
            {
                if (attacker.HasType(CardType.Fusion) && Bot.HasInHand(CardId.AleisterTheInvoker))
                    attacker.RealPower = attacker.RealPower + 1000;
            }
            return base.OnPreBattleBetween(attacker, defender);
        }

        private bool PhotonThrasherSummon()
        {
            return Bot.HasInHand(CardId.AleisterTheInvoker)
                && !Bot.HasInHand(CardId.Ratpier)
                && !Bot.HasInHand(CardId.Thoroughblade);
        }

        private bool AleisterTheInvokerEffect()
        {
            if (Card.Location == CardLocation.Hand)
            {
                if (DefaultCheckWhetherCardIsNegated(Card)) return false;
                if (!(Duel.Phase == DuelPhase.BattleStep
                    || Duel.Phase == DuelPhase.BattleStart
                    || Duel.Phase == DuelPhase.Damage))
                    return false;
                return Duel.Player==0
                    || Util.IsOneEnemyBetter();
            }
            return true;
        }

        private bool InvocationEffect()
        {
            if (Card.Location == CardLocation.Grave)
                return true;
            IList<ClientCard> materials0 = Bot.Graveyard;
            IList<ClientCard> materials1 = Enemy.Graveyard;
            IList<ClientCard> mats = new List<ClientCard>();
            ClientCard aleister = GetAleisterInGrave();
            if (aleister != null)
            {
                mats.Add(aleister);
            }
            ClientCard mat = null;
            foreach (ClientCard card in materials0)
            {
                if (card.HasAttribute(CardAttribute.Light))
                {
                    mat = card;
                    break;
                }
            }
            foreach (ClientCard card in materials1)
            {
                if (card.HasAttribute(CardAttribute.Light))
                {
                    mat = card;
                    break;
                }
            }
            if (mat != null)
            {
                mats.Add(mat);
                AI.SelectCard(CardId.InvokedMechaba);
                AI.SelectMaterials(mats);
                AI.SelectPosition(CardPosition.FaceUpAttack);
                return true;
            }
            foreach (ClientCard card in materials0)
            {
                if (card.HasAttribute(CardAttribute.Earth))
                {
                    mat = card;
                    break;
                }
            }
            foreach (ClientCard card in materials1)
            {
                if (card.HasAttribute(CardAttribute.Earth))
                {
                    mat = card;
                    break;
                }
            }
            if (mat != null)
            {
                mats.Add(mat);
                AI.SelectCard(CardId.InvokedMagellanica);
                AI.SelectMaterials(mats);
                AI.SelectPosition(CardPosition.FaceUpAttack);
                return true;
            }
            return false;
        }

        private ClientCard GetAleisterInGrave()
        {
            foreach (ClientCard card in Enemy.Graveyard)
            {
                if (card.IsCode(CardId.AleisterTheInvoker))
                {
                    return card;
                }
            }
            foreach (ClientCard card in Bot.Graveyard)
            {
                if (card.IsCode(CardId.AleisterTheInvoker))
                {
                    return card;
                }
            }
            return null;
        }

        private bool ChakanineSummon()
        {
            if (Bot.HasInMonstersZone(CardId.Ratpier) && !ChakanineSpsummoned)
            {
                AI.SelectMaterials(CardId.Ratpier);
                AI.SelectYesNo(true);
                AI.SelectPosition(CardPosition.FaceUpDefence);
                ChakanineSpsummoned = true;
                return true;
            }
            if (Bot.HasInMonstersZone(CardId.Broadbull) && !ChakanineSpsummoned)
            {
                AI.SelectMaterials(CardId.Broadbull);
                AI.SelectYesNo(true);
                AI.SelectPosition(CardPosition.FaceUpDefence);
                ChakanineSpsummoned = true;
                return true;
            }
            return false;
        }

        private bool ChakanineEffect()
        {
            if (Bot.HasInGraveyard(CardId.Whiptail) || Bot.HasInGraveyard(CardId.Thoroughblade))
            {
                AI.SelectCard(
                    CardId.Broadbull,
                    CardId.Tigermortar,
                    CardId.Chakanine,
                    CardId.Thoroughblade,
                    CardId.Ratpier,
                    CardId.Whiptail
                    );
                AI.SelectNextCard(
                    CardId.Whiptail,
                    CardId.Thoroughblade
                    );
                return true;
            }
            return false;
        }

        private bool TigermortarSummon()
        {
            if (Bot.HasInMonstersZone(CardId.Chakanine) && !TigermortarSpsummoned)
            {
                AI.SelectMaterials(CardId.Chakanine);
                AI.SelectYesNo(true);
                AI.SelectPosition(CardPosition.FaceUpDefence);
                TigermortarSpsummoned = true;
                return true;
            }
            if (Bot.HasInMonstersZone(CardId.Ratpier) && !TigermortarSpsummoned)
            {
                AI.SelectMaterials(CardId.Ratpier);
                AI.SelectYesNo(true);
                AI.SelectPosition(CardPosition.FaceUpDefence);
                TigermortarSpsummoned = true;
                return true;
            }
            if (Bot.HasInMonstersZone(CardId.Thoroughblade) && !TigermortarSpsummoned
                && Bot.HasInGraveyard(new[]
                {
                    CardId.Whiptail,
                    CardId.Ratpier
                }))
            {
                AI.SelectMaterials(CardId.Thoroughblade);
                AI.SelectYesNo(true);
                TigermortarSpsummoned = true;
                return true;
            }
            if (Bot.HasInMonstersZone(CardId.Whiptail) && !TigermortarSpsummoned
                && Bot.HasInGraveyard(CardId.Ratpier))
            {
                AI.SelectMaterials(CardId.Whiptail);
                AI.SelectYesNo(true);
                TigermortarSpsummoned = true;
                return true;
            }
            return false;
        }

        private bool TigermortarEffect()
        {
            //if (Card.HasXyzMaterial(CardId.Ratpier) || !Bot.HasInGraveyard(CardId.Ratpier))
            //    return false;
            AI.SelectCard(CardId.Chakanine);
            AI.SelectNextCard(CardId.Tigermortar);
            AI.SelectThirdCard(CardId.Ratpier, CardId.Whiptail, CardId.Thoroughblade);
            return true;
        }

        private bool BroadbullSummon()
        {
            if (Bot.HasInMonstersZone(CardId.Tigermortar) && !BroadbullSpsummoned)
            {
                AI.SelectMaterials(CardId.Tigermortar);
                AI.SelectYesNo(true);
                AI.SelectPosition(CardPosition.FaceUpDefence);
                BroadbullSpsummoned = true;
                return true;
            }
            if (Bot.HasInMonstersZone(CardId.Chakanine) && !BroadbullSpsummoned)
            {
                AI.SelectMaterials(CardId.Chakanine);
                AI.SelectYesNo(true);
                AI.SelectPosition(CardPosition.FaceUpDefence);
                BroadbullSpsummoned = true;
                return true;
            }
            if (Bot.HasInMonstersZone(CardId.Ratpier) && !BroadbullSpsummoned)
            {
                AI.SelectMaterials(CardId.Ratpier);
                AI.SelectYesNo(true);
                AI.SelectPosition(CardPosition.FaceUpDefence);
                BroadbullSpsummoned = true;
                return true;
            }
            if (Bot.HasInMonstersZone(CardId.Thoroughblade) && !BroadbullSpsummoned)
            {
                AI.SelectMaterials(CardId.Thoroughblade);
                AI.SelectYesNo(true);
                AI.SelectPosition(CardPosition.FaceUpDefence);
                BroadbullSpsummoned = true;
                return true;
            }
            return false;
        }

        private bool BroadbullEffect()
        {
            AI.SelectCard(
                CardId.Tigermortar,
                CardId.Chakanine,
                CardId.Drident,
                CardId.AleisterTheInvoker,
                CardId.PhotonThrasher
                );
            if (Bot.HasInHand(CardId.Whiptail) && !Bot.HasInHand(CardId.Ratpier))
                AI.SelectNextCard(CardId.Ratpier);
            else
                AI.SelectNextCard(CardId.Whiptail);
            return true;
        }

        private bool BroadbullXYZSummon()
        {
            AI.SelectYesNo(false);
            AI.SelectPosition(CardPosition.FaceUpDefence);
            AI.SelectMaterials(new[]
                {
                    CardId.Ratpier,
                    CardId.PhotonThrasher,
                    CardId.Whiptail,
                    CardId.AleisterTheInvoker
                });
            return true;
        }

        private bool DridentSummon()
        {
            AI.SelectMaterials(new[]
                {
                    CardId.Broadbull,
                    CardId.Tigermortar,
                    CardId.Chakanine,
                    CardId.Thoroughblade
                });
            return true;
        }

        private bool RatpierMaterialEffect()
        {
            if (ActivateDescription == Util.GetStringId(CardId.Ratpier, 1))
            {
                AI.SelectPosition(CardPosition.FaceUpDefence);
                return true;
            }
            return false;
        }

        private bool WhiptailEffect()
        {
            if (Duel.Phase == DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2)
                return false;
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            ClientCard target = null;
            List<ClientCard> monsters = Bot.GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                if (monster.IsFaceup() && monster.IsCode(CardId.Drident) && !monster.HasXyzMaterial())
                {
                    target = monster;
                    break;
                }
            }
            /*if (target == null)
            {
                foreach (ClientCard monster in monsters)
                {
                    if (monster.IsFaceup() && monster.Type == (int)CardType.Xyz && !monster.IsCode(CardId.DaigustoEmeral) && !monster.HasXyzMaterial())
                    {
                        target = monster;
                        break;
                    }
                }
            }*/
            if (target == null)
            {
                AI.SelectCard(new[]
                    {
                        CardId.Drident
                    });
            }
            return true;
        }

        private bool RatpierEffect()
        {
            AI.SelectCard(
                CardId.ZoodiacCombo,
                CardId.Thoroughblade,
                CardId.ZoodiacBarrage
                );
            return true;
        }

        private bool DridentEffect()
        {
            if (Duel.LastChainPlayer == 0)
                return false;
            ClientCard target = Util.GetBestEnemyCard(true);
            if (target == null)
                return false;
            AI.SelectCard(
                CardId.Broadbull,
                CardId.Tigermortar,
                CardId.Chakanine,
                CardId.Thoroughblade,
                CardId.Ratpier,
                CardId.Whiptail
                );
            AI.SelectNextCard(target);
            return true;
        }

        private bool DaigustoEmeralSummon()
        {
            AI.SelectMaterials(new[]
                {
                    CardId.PhotonThrasher,
                    CardId.AleisterTheInvoker
                });
            return Bot.GetGraveyardMonsters().Count >= 3;
        }

        private bool DaigustoEmeralEffect()
        {
            AI.SelectCard(
                CardId.Ratpier,
                CardId.AleisterTheInvoker,
                CardId.Whiptail
                );
            AI.SelectNextCard(
                CardId.Ratpier,
                CardId.DaigustoEmeral
                );
            return true;
        }

        private bool FireFormationTenkiEffect()
        {
            if (Bot.HasInHand(CardId.ZoodiacBarrage)
               || Bot.HasInSpellZone(CardId.ZoodiacBarrage)
               || Bot.HasInHand(CardId.Ratpier))
            {
                AI.SelectCard(CardId.Whiptail);
            }
            else
            {
                AI.SelectCard(CardId.Ratpier);
            }
            AI.SelectYesNo(true);
            return true;
        }

        private bool ZoodiacBarrageEffect()
        {
            foreach (ClientCard spell in Bot.GetSpells())
            {
                if (spell.IsCode(CardId.ZoodiacBarrage) && !Card.Equals(spell))
                    return false;
            }
            AI.SelectCard(
                CardId.FireFormationTenki,
                CardId.MagicalMeltdown,
                CardId.ZoodiacBarrage
                );
            AI.SelectNextCard(
                CardId.Ratpier,
                CardId.Whiptail,
                CardId.Thoroughblade
                );
            AI.SelectPosition(CardPosition.FaceUpDefence);
            return true;
        }

        private bool ZoodiacComboEffect()
        {
            if (Duel.CurrentChain.Count > 0)
                return false;
            if (Card.Location != CardLocation.Grave)
            {
                AI.SelectCard(CardId.Drident);
                AI.SelectNextCard(
                    CardId.Whiptail,
                    CardId.Ratpier,
                    CardId.Thoroughblade
                    );
            }
            return true;
        }

        private bool MonsterRebornEffect()
        {
            AI.SelectCard(
                CardId.Ratpier,
                CardId.Whiptail,
                CardId.InvokedMechaba,
                CardId.JizukirutheStarDestroyingKaiju,
                CardId.InvokedMagellanica,
                CardId.Tigermortar,
                CardId.Chakanine,
                CardId.Broadbull
                );
            return true;
        }

        private bool MonsterRepos()
        {
            if (Card.IsCode(CardId.NumberS39UtopiatheLightning) && Card.IsAttack())
                return false;
            return base.DefaultMonsterRepos();
        }
    }
}
