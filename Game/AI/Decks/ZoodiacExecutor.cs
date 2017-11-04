using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("Zoodiac", "AI_Zoodiac", "OutDated")]
    class ZoodiacExecutor : DefaultExecutor
    {
        public class CardId
        {
            public static int JizukirutheStarDestroyingKaiju = 63941210;
            public static int GadarlatheMysteryDustKaiju = 36956512;
            public static int GamecieltheSeaTurtleKaiju = 55063751;
            public static int RadiantheMultidimensionalKaiju = 28674152;
            public static int KumongoustheStickyStringKaiju = 29726552;
            public static int PhotonThrasher = 65367484;
            public static int Thoroughblade = 77150143;
            public static int Whiptail = 31755044;
            public static int Ratpier = 78872731;
            public static int AleisterTheInvoker = 86120751;

            public static int HarpiesFeatherDuster = 18144506;
            public static int DarkHole = 53129443;
            public static int Terraforming = 73628505;
            public static int Invocation = 74063034;
            public static int MonsterReborn = 83764718;
            public static int InterruptedKaijuSlumber = 99330325;
            public static int ZoodiacBarrage = 46060017;
            public static int FireFormationTenki = 57103969;
            public static int MagicalMeltdown = 47679935;
            public static int ZoodiacCombo = 73881652;

            public static int InvokedMechaba = 75286621;
            public static int InvokedMagellanica = 48791583;
            public static int NumberS39UtopiatheLightning = 56832966;
            public static int Number39Utopia = 84013237;
            public static int DaigustoEmeral = 581014;
            public static int Tigermortar = 11510448;
            public static int Chakanine = 41375811;
            public static int Drident = 48905153;
            public static int Broadbull = 85115440;
        }

        bool TigermortarSpsummoned = false;
        bool ChakanineSpsummoned = false;
        bool BroadbullSpsummoned = false;
        int WhiptailEffectCount = 0;

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
            AddExecutor(ExecutorType.Activate, CardId.NumberS39UtopiatheLightning);

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
            WhiptailEffectCount = 0;
        }

        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            if (defender.IsMonsterInvincible())
            {
                if (defender.IsMonsterDangerous() || defender.IsDefense())
                    return false;
            }
            if (!(defender.Id == CardId.NumberS39UtopiatheLightning))
            {
                //if (attacker.HasType(CardType.Fusion) && Bot.HasInHand(CardId.AleisterTheInvoker))
                //    attacker.RealPower = attacker.RealPower + 1000;
                if (attacker.Id == CardId.NumberS39UtopiatheLightning && !attacker.IsDisabled() && attacker.HasXyzMaterial(2, CardId.Number39Utopia))
                    attacker.RealPower = 5000;
            }
            return attacker.RealPower > defender.GetDefensePower();
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
                if (!(Duel.Phase == DuelPhase.BattleStep
                    || Duel.Phase == DuelPhase.BattleStart
                    || Duel.Phase == DuelPhase.Damage))
                    return false;
                return Duel.Player==0
                    || AI.Utils.IsOneEnemyBetter();
            }
            return true;
        }

        private bool InvocationEffect()
        {
            if (Card.Location == CardLocation.Grave)
                return true;
            IList<ClientCard> materials0 = Bot.Graveyard;
            IList<ClientCard> materials1 = Enemy.Graveyard;
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
                AI.SelectCard(CardId.InvokedMechaba);
                SelectAleisterInGrave();
                AI.SelectThirdCard(mat);
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
                AI.SelectCard(CardId.InvokedMagellanica);
                SelectAleisterInGrave();
                AI.SelectThirdCard(mat);
                AI.SelectPosition(CardPosition.FaceUpAttack);
                return true;
            }
            return false;
        }

        private void SelectAleisterInGrave()
        {
            IList<ClientCard> materials0 = Bot.Graveyard;
            IList<ClientCard> materials1 = Enemy.Graveyard;
            foreach (ClientCard card in materials1)
            {
                if (card.Id == CardId.AleisterTheInvoker)
                {
                    AI.SelectNextCard(card);
                    return;
                }
            }
            foreach (ClientCard card in materials0)
            {
                if (card.Id == CardId.AleisterTheInvoker)
                {
                    AI.SelectNextCard(card);
                    return;
                }
            }
            AI.SelectNextCard(CardId.AleisterTheInvoker);
        }

        private bool ChakanineSummon()
        {
            if (Bot.HasInMonstersZone(CardId.Ratpier) && !ChakanineSpsummoned)
            {
                AI.SelectCard(CardId.Ratpier);
                AI.SelectYesNo(true);
                AI.SelectPosition(CardPosition.FaceUpDefence);
                ChakanineSpsummoned = true;
                return true;
            }
            if (Bot.HasInMonstersZone(CardId.Broadbull) && !ChakanineSpsummoned)
            {
                AI.SelectCard(CardId.Broadbull);
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
                AI.SelectCard(new[]
                {
                    CardId.Broadbull,
                    CardId.Tigermortar,
                    CardId.Chakanine,
                    CardId.Thoroughblade,
                    CardId.Ratpier,
                    CardId.Whiptail
                });
                AI.SelectNextCard(new[]
                {
                    CardId.Whiptail,
                    CardId.Thoroughblade
                });
                return true;
            }
            return false;
        }

        private bool TigermortarSummon()
        {
            if (Bot.HasInMonstersZone(CardId.Chakanine) && !TigermortarSpsummoned)
            {
                AI.SelectCard(CardId.Chakanine);
                AI.SelectYesNo(true);
                AI.SelectPosition(CardPosition.FaceUpDefence);
                TigermortarSpsummoned = true;
                return true;
            }
            if (Bot.HasInMonstersZone(CardId.Ratpier) && !TigermortarSpsummoned)
            {
                AI.SelectCard(CardId.Ratpier);
                AI.SelectYesNo(true);
                AI.SelectPosition(CardPosition.FaceUpDefence);
                TigermortarSpsummoned = true;
                return true;
            }
            if (Bot.HasInMonstersZone(CardId.Thoroughblade) && !TigermortarSpsummoned
                && Bot.HasInGraveyard(new List<int>
                {
                    CardId.Whiptail,
                    CardId.Ratpier
                }))
            {
                AI.SelectCard(CardId.Thoroughblade);
                AI.SelectYesNo(true);
                TigermortarSpsummoned = true;
                return true;
            }
            if (Bot.HasInMonstersZone(CardId.Whiptail) && !TigermortarSpsummoned
                && Bot.HasInGraveyard(CardId.Ratpier))
            {
                AI.SelectCard(CardId.Whiptail);
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
            AI.SelectThirdCard(new[]
                {
                    CardId.Ratpier,
                    CardId.Whiptail,
                    CardId.Thoroughblade
                });
            return true;
        }

        private bool BroadbullSummon()
        {
            if (Bot.HasInMonstersZone(CardId.Tigermortar) && !BroadbullSpsummoned)
            {
                AI.SelectCard(CardId.Tigermortar);
                AI.SelectYesNo(true);
                AI.SelectPosition(CardPosition.FaceUpDefence);
                BroadbullSpsummoned = true;
                return true;
            }
            if (Bot.HasInMonstersZone(CardId.Chakanine) && !BroadbullSpsummoned)
            {
                AI.SelectCard(CardId.Chakanine);
                AI.SelectYesNo(true);
                AI.SelectPosition(CardPosition.FaceUpDefence);
                BroadbullSpsummoned = true;
                return true;
            }
            if (Bot.HasInMonstersZone(CardId.Ratpier) && !BroadbullSpsummoned)
            {
                AI.SelectCard(CardId.Ratpier);
                AI.SelectYesNo(true);
                AI.SelectPosition(CardPosition.FaceUpDefence);
                BroadbullSpsummoned = true;
                return true;
            }
            if (Bot.HasInMonstersZone(CardId.Thoroughblade) && !BroadbullSpsummoned)
            {
                AI.SelectCard(CardId.Thoroughblade);
                AI.SelectYesNo(true);
                AI.SelectPosition(CardPosition.FaceUpDefence);
                BroadbullSpsummoned = true;
                return true;
            }
            return false;
        }

        private bool BroadbullEffect()
        {
            AI.SelectCard(new[]
                {
                    CardId.Tigermortar,
                    CardId.Chakanine,
                    CardId.Drident,
                    CardId.AleisterTheInvoker,
                    CardId.PhotonThrasher
                });
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
            AI.SelectCard(new[]
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
            AI.SelectCard(new[]
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
            if (ActivateDescription == AI.Utils.GetStringId(CardId.Ratpier, 1))
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
            if (Card.IsDisabled() || WhiptailEffectCount >= 3)
                return false;
            ClientCard target = null;
            List<ClientCard> monsters = Bot.GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                if (monster.IsFaceup() && monster.Id == CardId.Drident && !monster.HasXyzMaterial())
                {
                    target = monster;
                    break;
                }
            }
            /*if (target == null)
            {
                foreach (ClientCard monster in monsters)
                {
                    if (monster.IsFaceup() && monster.Type == (int)CardType.Xyz && monster.Id != CardId.DaigustoEmeral && !monster.HasXyzMaterial())
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
            WhiptailEffectCount++;
            return true;
        }

        private bool RatpierEffect()
        {
            AI.SelectCard(new[]
                {
                    CardId.ZoodiacCombo,
                    CardId.Thoroughblade,
                    CardId.ZoodiacBarrage
                });
            return true;
        }

        private bool DridentEffect()
        {
            if (LastChainPlayer == 0)
                return false;
            ClientCard target = AI.Utils.GetBestEnemyCard(true);
            if (target == null)
                return false;
            AI.SelectCard(new[]
                {
                    CardId.Broadbull,
                    CardId.Tigermortar,
                    CardId.Chakanine,
                    CardId.Thoroughblade,
                    CardId.Ratpier,
                    CardId.Whiptail
                });
            AI.SelectNextCard(target);
            return true;
        }

        private bool DaigustoEmeralSummon()
        {
            return Bot.GetGraveyardMonsters().Count >= 3;
        }

        private bool DaigustoEmeralEffect()
        {
            AI.SelectCard(new[]
                {
                    CardId.Ratpier,
                    CardId.AleisterTheInvoker,
                    CardId.Whiptail
                });
            AI.SelectNextCard(new[]
                {
                    CardId.Ratpier,
                    CardId.DaigustoEmeral
                });
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
            IList<ClientCard> spells = Bot.GetSpells();
            foreach (ClientCard spell in spells)
            {
                if (spell.Id == CardId.ZoodiacBarrage && !Card.Equals(spell))
                    return false;
            }
            AI.SelectCard(new[]
                {
                    CardId.FireFormationTenki,
                    CardId.MagicalMeltdown,
                    CardId.ZoodiacBarrage
                });
            AI.SelectNextCard(new[]
                {
                    CardId.Ratpier,
                    CardId.Whiptail,
                    CardId.Thoroughblade
                });
            AI.SelectPosition(CardPosition.FaceUpDefence);
            return true;
        }

        private bool ZoodiacComboEffect()
        {
            if (CurrentChain.Count > 0)
                return false;
            if (Card.Location != CardLocation.Grave)
            {
                AI.SelectCard(CardId.Drident);
                AI.SelectNextCard(new[]
                {
                    CardId.Whiptail,
                    CardId.Ratpier,
                    CardId.Thoroughblade
                });
            }
            return true;
        }

        private bool MonsterRebornEffect()
        {
            AI.SelectCard(new[]
                {
                    CardId.Ratpier,
                    CardId.Whiptail,
                    CardId.InvokedMechaba,
                    CardId.JizukirutheStarDestroyingKaiju,
                    CardId.InvokedMagellanica,
                    CardId.Tigermortar,
                    CardId.Chakanine,
                    CardId.Broadbull
                });
            return true;
        }

        private bool MonsterRepos()
        {
            if (Card.Id == CardId.NumberS39UtopiatheLightning)
                return false;
            return base.DefaultMonsterRepos();
        }
    }
}
