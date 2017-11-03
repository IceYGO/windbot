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
        public enum CardId
        {
            JizukirutheStarDestroyingKaiju = 63941210,
            GadarlatheMysteryDustKaiju = 36956512,
            GamecieltheSeaTurtleKaiju = 55063751,
            RadiantheMultidimensionalKaiju = 28674152,
            KumongoustheStickyStringKaiju = 29726552,
            PhotonThrasher = 65367484,
            Thoroughblade = 77150143,
            Whiptail = 31755044,
            Ratpier = 78872731,
            AleisterTheInvoker = 86120751,

            HarpiesFeatherDuster = 18144506,
            DarkHole = 53129443,
            Terraforming = 73628505,
            Invocation = 74063034,
            MonsterReborn = 83764718,
            InterruptedKaijuSlumber = 99330325,
            ZoodiacBarrage = 46060017,
            FireFormationTenki = 57103969,
            MagicalMeltdown = 47679935,
            ZoodiacCombo = 73881652,

            InvokedMechaba = 75286621,
            InvokedMagellanica = 48791583,
            NumberS39UtopiatheLightning = 56832966,
            Number39Utopia = 84013237,
            DaigustoEmeral = 581014,
            Tigermortar = 11510448,
            Chakanine = 41375811,
            Drident = 48905153,
            Broadbull = 85115440
        }

        bool TigermortarSpsummoned = false;
        bool ChakanineSpsummoned = false;
        bool BroadbullSpsummoned = false;
        int WhiptailEffectCount = 0;

        public ZoodiacExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // Quick spells
            AddExecutor(ExecutorType.Activate, (int)CardId.HarpiesFeatherDuster);
            AddExecutor(ExecutorType.Activate, (int)CardId.InterruptedKaijuSlumber, DefaultInterruptedKaijuSlumber);
            AddExecutor(ExecutorType.Activate, (int)CardId.DarkHole, DefaultDarkHole);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.GamecieltheSeaTurtleKaiju, DefaultKaijuSpsummon);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.KumongoustheStickyStringKaiju, DefaultKaijuSpsummon);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.GadarlatheMysteryDustKaiju, DefaultKaijuSpsummon);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.RadiantheMultidimensionalKaiju, DefaultKaijuSpsummon);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.JizukirutheStarDestroyingKaiju, DefaultKaijuSpsummon);

            AddExecutor(ExecutorType.Activate, (int)CardId.Terraforming);
            AddExecutor(ExecutorType.Activate, (int)CardId.MagicalMeltdown);
            AddExecutor(ExecutorType.Activate, (int)CardId.FireFormationTenki, FireFormationTenkiEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.ZoodiacBarrage, ZoodiacBarrageEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.DaigustoEmeral, DaigustoEmeralEffect);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.PhotonThrasher, PhotonThrasherSummon);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.Number39Utopia, DefaultNumberS39UtopiaTheLightningSummon);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.NumberS39UtopiatheLightning);
            AddExecutor(ExecutorType.Activate, (int)CardId.NumberS39UtopiatheLightning);

            AddExecutor(ExecutorType.Activate, (int)CardId.InvokedMechaba, DefaultTrap);

            AddExecutor(ExecutorType.Activate, RatpierMaterialEffect);

            AddExecutor(ExecutorType.Activate, (int)CardId.Drident, DridentEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.Broadbull, BroadbullEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.Tigermortar, TigermortarEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.Chakanine, ChakanineEffect);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.Chakanine, ChakanineSummon);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.Tigermortar, TigermortarSummon);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.Broadbull, BroadbullSummon);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.Drident, DridentSummon);

            AddExecutor(ExecutorType.Summon, (int)CardId.Ratpier);
            AddExecutor(ExecutorType.Activate, (int)CardId.Ratpier, RatpierEffect);
            AddExecutor(ExecutorType.Summon, (int)CardId.Thoroughblade);
            AddExecutor(ExecutorType.Activate, (int)CardId.Thoroughblade, RatpierEffect);
            AddExecutor(ExecutorType.Summon, (int)CardId.AleisterTheInvoker);
            AddExecutor(ExecutorType.Activate, (int)CardId.AleisterTheInvoker, AleisterTheInvokerEffect);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.DaigustoEmeral, DaigustoEmeralSummon);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.Broadbull, BroadbullXYZSummon);

            AddExecutor(ExecutorType.Activate, (int)CardId.MonsterReborn, MonsterRebornEffect);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.PhotonThrasher);
            AddExecutor(ExecutorType.Summon, (int)CardId.Whiptail);

            AddExecutor(ExecutorType.Activate, (int)CardId.Invocation, InvocationEffect);

            AddExecutor(ExecutorType.Activate, (int)CardId.Whiptail, WhiptailEffect);

            AddExecutor(ExecutorType.Activate, (int)CardId.ZoodiacCombo, ZoodiacComboEffect);

            AddExecutor(ExecutorType.SpellSet, (int)CardId.ZoodiacCombo);

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
            if (!(defender.Id == (int)CardId.NumberS39UtopiatheLightning))
            {
                //if (attacker.HasType(CardType.Fusion) && Bot.HasInHand((int)CardId.AleisterTheInvoker))
                //    attacker.RealPower = attacker.RealPower + 1000;
                if (attacker.Id == (int)CardId.NumberS39UtopiatheLightning && !attacker.IsDisabled() && attacker.HasXyzMaterial(2, (int)CardId.Number39Utopia))
                    attacker.RealPower = 5000;
            }
            return attacker.RealPower > defender.GetDefensePower();
        }

        private bool PhotonThrasherSummon()
        {
            return Bot.HasInHand((int)CardId.AleisterTheInvoker)
                && !Bot.HasInHand((int)CardId.Ratpier)
                && !Bot.HasInHand((int)CardId.Thoroughblade);
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
                AI.SelectCard((int)CardId.InvokedMechaba);
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
                AI.SelectCard((int)CardId.InvokedMagellanica);
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
                if (card.Id == (int)CardId.AleisterTheInvoker)
                {
                    AI.SelectNextCard(card);
                    return;
                }
            }
            foreach (ClientCard card in materials0)
            {
                if (card.Id == (int)CardId.AleisterTheInvoker)
                {
                    AI.SelectNextCard(card);
                    return;
                }
            }
            AI.SelectNextCard((int)CardId.AleisterTheInvoker);
        }

        private bool ChakanineSummon()
        {
            if (Bot.HasInMonstersZone((int)CardId.Ratpier) && !ChakanineSpsummoned)
            {
                AI.SelectCard((int)CardId.Ratpier);
                AI.SelectYesNo(true);
                AI.SelectPosition(CardPosition.FaceUpDefence);
                ChakanineSpsummoned = true;
                return true;
            }
            if (Bot.HasInMonstersZone((int)CardId.Broadbull) && !ChakanineSpsummoned)
            {
                AI.SelectCard((int)CardId.Broadbull);
                AI.SelectYesNo(true);
                AI.SelectPosition(CardPosition.FaceUpDefence);
                ChakanineSpsummoned = true;
                return true;
            }
            return false;
        }

        private bool ChakanineEffect()
        {
            if (Bot.HasInGraveyard((int)CardId.Whiptail) || Bot.HasInGraveyard((int)CardId.Thoroughblade))
            {
                AI.SelectCard(new[]
                {
                    (int)CardId.Broadbull,
                    (int)CardId.Tigermortar,
                    (int)CardId.Chakanine,
                    (int)CardId.Thoroughblade,
                    (int)CardId.Ratpier,
                    (int)CardId.Whiptail
                });
                AI.SelectNextCard(new[]
                {
                    (int)CardId.Whiptail,
                    (int)CardId.Thoroughblade
                });
                return true;
            }
            return false;
        }

        private bool TigermortarSummon()
        {
            if (Bot.HasInMonstersZone((int)CardId.Chakanine) && !TigermortarSpsummoned)
            {
                AI.SelectCard((int)CardId.Chakanine);
                AI.SelectYesNo(true);
                AI.SelectPosition(CardPosition.FaceUpDefence);
                TigermortarSpsummoned = true;
                return true;
            }
            if (Bot.HasInMonstersZone((int)CardId.Ratpier) && !TigermortarSpsummoned)
            {
                AI.SelectCard((int)CardId.Ratpier);
                AI.SelectYesNo(true);
                AI.SelectPosition(CardPosition.FaceUpDefence);
                TigermortarSpsummoned = true;
                return true;
            }
            if (Bot.HasInMonstersZone((int)CardId.Thoroughblade) && !TigermortarSpsummoned
                && Bot.HasInGraveyard(new List<int>
                {
                    (int)CardId.Whiptail,
                    (int)CardId.Ratpier
                }))
            {
                AI.SelectCard((int)CardId.Thoroughblade);
                AI.SelectYesNo(true);
                TigermortarSpsummoned = true;
                return true;
            }
            if (Bot.HasInMonstersZone((int)CardId.Whiptail) && !TigermortarSpsummoned
                && Bot.HasInGraveyard((int)CardId.Ratpier))
            {
                AI.SelectCard((int)CardId.Whiptail);
                AI.SelectYesNo(true);
                TigermortarSpsummoned = true;
                return true;
            }
            return false;
        }

        private bool TigermortarEffect()
        {
            //if (Card.HasXyzMaterial((int)CardId.Ratpier) || !Bot.HasInGraveyard((int)CardId.Ratpier))
            //    return false;
            AI.SelectCard((int)CardId.Chakanine);
            AI.SelectNextCard((int)CardId.Tigermortar);
            AI.SelectThirdCard(new[]
                {
                    (int)CardId.Ratpier,
                    (int)CardId.Whiptail,
                    (int)CardId.Thoroughblade
                });
            return true;
        }

        private bool BroadbullSummon()
        {
            if (Bot.HasInMonstersZone((int)CardId.Tigermortar) && !BroadbullSpsummoned)
            {
                AI.SelectCard((int)CardId.Tigermortar);
                AI.SelectYesNo(true);
                AI.SelectPosition(CardPosition.FaceUpDefence);
                BroadbullSpsummoned = true;
                return true;
            }
            if (Bot.HasInMonstersZone((int)CardId.Chakanine) && !BroadbullSpsummoned)
            {
                AI.SelectCard((int)CardId.Chakanine);
                AI.SelectYesNo(true);
                AI.SelectPosition(CardPosition.FaceUpDefence);
                BroadbullSpsummoned = true;
                return true;
            }
            if (Bot.HasInMonstersZone((int)CardId.Ratpier) && !BroadbullSpsummoned)
            {
                AI.SelectCard((int)CardId.Ratpier);
                AI.SelectYesNo(true);
                AI.SelectPosition(CardPosition.FaceUpDefence);
                BroadbullSpsummoned = true;
                return true;
            }
            if (Bot.HasInMonstersZone((int)CardId.Thoroughblade) && !BroadbullSpsummoned)
            {
                AI.SelectCard((int)CardId.Thoroughblade);
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
                    (int)CardId.Tigermortar,
                    (int)CardId.Chakanine,
                    (int)CardId.Drident,
                    (int)CardId.AleisterTheInvoker,
                    (int)CardId.PhotonThrasher
                });
            if (Bot.HasInHand((int)CardId.Whiptail) && !Bot.HasInHand((int)CardId.Ratpier))
                AI.SelectNextCard((int)CardId.Ratpier);
            else
                AI.SelectNextCard((int)CardId.Whiptail);
            return true;
        }

        private bool BroadbullXYZSummon()
        {
            AI.SelectYesNo(false);
            AI.SelectPosition(CardPosition.FaceUpDefence);
            AI.SelectCard(new[]
                {
                    (int)CardId.Ratpier,
                    (int)CardId.PhotonThrasher,
                    (int)CardId.Whiptail,
                    (int)CardId.AleisterTheInvoker
                });
            return true;
        }

        private bool DridentSummon()
        {
            AI.SelectCard(new[]
                {
                    (int)CardId.Broadbull,
                    (int)CardId.Tigermortar,
                    (int)CardId.Chakanine,
                    (int)CardId.Thoroughblade
                });
            return true;
        }

        private bool RatpierMaterialEffect()
        {
            if (ActivateDescription == AI.Utils.GetStringId((int)CardId.Ratpier, 1))
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
                if (monster.IsFaceup() && monster.Id == (int)CardId.Drident && !monster.HasXyzMaterial())
                {
                    target = monster;
                    break;
                }
            }
            /*if (target == null)
            {
                foreach (ClientCard monster in monsters)
                {
                    if (monster.IsFaceup() && monster.Type == (int)CardType.Xyz && monster.Id != (int)CardId.DaigustoEmeral && !monster.HasXyzMaterial())
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
                        (int)CardId.Drident
                    });
            }
            WhiptailEffectCount++;
            return true;
        }

        private bool RatpierEffect()
        {
            AI.SelectCard(new[]
                {
                    (int)CardId.ZoodiacCombo,
                    (int)CardId.Thoroughblade,
                    (int)CardId.ZoodiacBarrage
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
                    (int)CardId.Broadbull,
                    (int)CardId.Tigermortar,
                    (int)CardId.Chakanine,
                    (int)CardId.Thoroughblade,
                    (int)CardId.Ratpier,
                    (int)CardId.Whiptail
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
                    (int)CardId.Ratpier,
                    (int)CardId.AleisterTheInvoker,
                    (int)CardId.Whiptail
                });
            AI.SelectNextCard(new[]
                {
                    (int)CardId.Ratpier,
                    (int)CardId.DaigustoEmeral
                });
            return true;
        }

        private bool FireFormationTenkiEffect()
        {
            if (Bot.HasInHand((int)CardId.ZoodiacBarrage)
               || Bot.HasInSpellZone((int)CardId.ZoodiacBarrage)
               || Bot.HasInHand((int)CardId.Ratpier))
            {
                AI.SelectCard((int)CardId.Whiptail);
            }
            else
            {
                AI.SelectCard((int)CardId.Ratpier);
            }
            AI.SelectYesNo(true);
            return true;
        }

        private bool ZoodiacBarrageEffect()
        {
            IList<ClientCard> spells = Bot.GetSpells();
            foreach (ClientCard spell in spells)
            {
                if (spell.Id == (int)CardId.ZoodiacBarrage && !Card.Equals(spell))
                    return false;
            }
            AI.SelectCard(new[]
                {
                    (int)CardId.FireFormationTenki,
                    (int)CardId.MagicalMeltdown,
                    (int)CardId.ZoodiacBarrage
                });
            AI.SelectNextCard(new[]
                {
                    (int)CardId.Ratpier,
                    (int)CardId.Whiptail,
                    (int)CardId.Thoroughblade
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
                AI.SelectCard((int)CardId.Drident);
                AI.SelectNextCard(new[]
                {
                    (int)CardId.Whiptail,
                    (int)CardId.Ratpier,
                    (int)CardId.Thoroughblade
                });
            }
            return true;
        }

        private bool MonsterRebornEffect()
        {
            AI.SelectCard(new[]
                {
                    (int)CardId.Ratpier,
                    (int)CardId.Whiptail,
                    (int)CardId.InvokedMechaba,
                    (int)CardId.JizukirutheStarDestroyingKaiju,
                    (int)CardId.InvokedMagellanica,
                    (int)CardId.Tigermortar,
                    (int)CardId.Chakanine,
                    (int)CardId.Broadbull
                });
            return true;
        }

        private bool MonsterRepos()
        {
            if (Card.Id == (int)CardId.NumberS39UtopiatheLightning)
                return false;
            return base.DefaultMonsterRepos();
        }
    }
}
