using System;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using YGOSharp.OCGWrapper.Enums;

namespace WindBot.Game.AI.Decks
{
    [Deck("Zexal Weapons", "AI_ZexalWeapons")]
    class ZexalWeaponsExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int CyberDragon = 70095155;
            public const int ZwTornadoBringer = 81471108;
            public const int ZwLightningBlade = 45082499;
            public const int ZwAsuraStrike = 40941889;
            public const int SolarWindJammer = 33911264;
            public const int PhotonTrasher = 65367484;
            public const int StarDrawing = 24610207;
            public const int SacredCrane = 30914564;
            public const int Goblindbergh = 25259669;
            public const int Honest = 37742478;
            public const int Kagetokage = 94656263;
            public const int HeroicChallengerExtraSword = 34143852;
            public const int TinGoldfish = 18063928;
            public const int SummonerMonk = 423585;
            public const int InstantFusion = 1845204;
            public const int Raigeki = 12580477;
            public const int ReinforcementOfTheArmy = 32807846;
            public const int DarkHole = 53129443;
            public const int MysticalSpaceTyphoon = 5318639;
            public const int BreakthroughSkill = 78474168;
            public const int SolemnWarning = 84749824;
            public const int SolemnStrike = 40605147;
            public const int XyzChangeTactics = 11705261;

            public const int FlameSwordsman = 45231177;
            public const int DarkfireDragon = 17881964;
            public const int GaiaDragonTheThunderCharger = 91949988;
            public const int ZwLionArms = 60992364;
            public const int AdreusKeeperOfArmageddon = 94119480;
            public const int Number61Volcasaurus = 29669359;
            public const int GemKnightPearl = 71594310;
            public const int Number39Utopia = 84013237;
            public const int NumberS39UtopiaOne = 86532744;
            public const int NumberS39UtopiatheLightning = 56832966;
            public const int MaestrokeTheSymphonyDjinn = 25341652;
            public const int GagagaCowboy = 12014404;
        }

        public ZexalWeaponsExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // Quick spells
            AddExecutor(ExecutorType.Activate, CardId.MysticalSpaceTyphoon, DefaultMysticalSpaceTyphoon);

            // Spell cards
            AddExecutor(ExecutorType.Activate, CardId.DarkHole, DefaultDarkHole);
            AddExecutor(ExecutorType.Activate, CardId.Raigeki, DefaultRaigeki);
            AddExecutor(ExecutorType.Activate, CardId.ReinforcementOfTheArmy, ReinforcementOfTheArmy);
            AddExecutor(ExecutorType.Activate, CardId.XyzChangeTactics, XyzChangeTactics);

            // XYZ summons
            AddExecutor(ExecutorType.SpSummon, CardId.Number39Utopia);
            AddExecutor(ExecutorType.SpSummon, CardId.NumberS39UtopiaOne);
            AddExecutor(ExecutorType.SpSummon, CardId.NumberS39UtopiatheLightning);
            AddExecutor(ExecutorType.SpSummon, CardId.Number61Volcasaurus, Number61Volcasaurus);
            AddExecutor(ExecutorType.SpSummon, CardId.ZwLionArms);
            AddExecutor(ExecutorType.SpSummon, CardId.AdreusKeeperOfArmageddon);

            // XYZ effects
            AddExecutor(ExecutorType.Activate, CardId.Number39Utopia, Number39Utopia);
            AddExecutor(ExecutorType.Activate, CardId.NumberS39UtopiaOne);
            AddExecutor(ExecutorType.Activate, CardId.NumberS39UtopiatheLightning, DefaultNumberS39UtopiaTheLightningEffect);
            AddExecutor(ExecutorType.Activate, CardId.ZwLionArms, ZwLionArms);
            AddExecutor(ExecutorType.Activate, CardId.AdreusKeeperOfArmageddon);
            AddExecutor(ExecutorType.Activate, CardId.Number61Volcasaurus);

            // Weapons
            AddExecutor(ExecutorType.Activate, CardId.ZwTornadoBringer, ZwWeapon);
            AddExecutor(ExecutorType.Activate, CardId.ZwLightningBlade, ZwWeapon);
            AddExecutor(ExecutorType.Activate, CardId.ZwAsuraStrike, ZwWeapon);


            // Special summons
            AddExecutor(ExecutorType.SpSummon, CardId.PhotonTrasher);
            AddExecutor(ExecutorType.SpSummon, CardId.CyberDragon);
            AddExecutor(ExecutorType.SpSummon, CardId.SolarWindJammer, SolarWindJammer);

            AddExecutor(ExecutorType.Activate, CardId.InstantFusion, InstantFusion);

            // Normal summons
            AddExecutor(ExecutorType.Summon, CardId.Goblindbergh, GoblindberghFirst);
            AddExecutor(ExecutorType.Summon, CardId.TinGoldfish, GoblindberghFirst);
            AddExecutor(ExecutorType.Summon, CardId.StarDrawing);
            AddExecutor(ExecutorType.Summon, CardId.SacredCrane);
            AddExecutor(ExecutorType.Summon, CardId.HeroicChallengerExtraSword);
            AddExecutor(ExecutorType.Summon, CardId.Goblindbergh);
            AddExecutor(ExecutorType.Summon, CardId.TinGoldfish);
            AddExecutor(ExecutorType.Summon, CardId.SummonerMonk);
            AddExecutor(ExecutorType.Summon, CardId.Honest);

            // Summons: Effects
            AddExecutor(ExecutorType.Activate, CardId.Goblindbergh, GoblindberghEffect);
            AddExecutor(ExecutorType.Activate, CardId.TinGoldfish, GoblindberghEffect);
            AddExecutor(ExecutorType.Activate, CardId.Kagetokage, KagetokageEffect);
            AddExecutor(ExecutorType.Activate, CardId.SummonerMonk, SummonerMonkEffect);
            AddExecutor(ExecutorType.Activate, CardId.Honest, DefaultHonestEffect);

            // Reposition
            AddExecutor(ExecutorType.Repos, MonsterRepos);

            // Spummon GaiaDragonTheThunderCharger if Volcasaurus or ZwLionArms had been used
            AddExecutor(ExecutorType.SpSummon, CardId.GaiaDragonTheThunderCharger);

            // Set and activate traps
            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);

            AddExecutor(ExecutorType.Activate, CardId.BreakthroughSkill, DefaultBreakthroughSkill);
            AddExecutor(ExecutorType.Activate, CardId.SolemnWarning, DefaultSolemnWarning);
            AddExecutor(ExecutorType.Activate, CardId.SolemnStrike, DefaultSolemnStrike);
        }

        public override bool OnSelectHand()
        {
            return false;
        }

        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            if (!defender.IsMonsterHasPreventActivationEffectInBattle())
            {
                if (attacker.Attribute == (int)CardAttribute.Light && Bot.HasInHand(CardId.Honest))
                    attacker.RealPower = attacker.RealPower + defender.Attack;
            }
            return base.OnPreBattleBetween(attacker, defender);
        }

        public override IList<ClientCard> OnSelectXyzMaterial(IList<ClientCard> cards, int min, int max)
        {
            IList<ClientCard> result = Util.SelectPreferredCards(new[] {
                CardId.StarDrawing,
                CardId.SolarWindJammer,
                CardId.Goblindbergh
            }, cards, min, max);
            return Util.CheckSelectCount(result, cards, min, max);
        }

        private bool Number39Utopia()
        {
            if (!Util.HasChainedTrap(0) && Duel.Player == 1 && Duel.Phase == DuelPhase.BattleStart && Card.HasXyzMaterial(2))
                return true;
            return false;
        }

        private bool Number61Volcasaurus()
        {
            return Util.IsOneEnemyBetterThanValue(2000, false);
        }

        private bool ZwLionArms()
        {
            if (ActivateDescription == Util.GetStringId(CardId.ZwLionArms, 0))
                return true;
            if (ActivateDescription == Util.GetStringId(CardId.ZwLionArms, 1))
                return !Card.IsDisabled() && ZwWeapon();
            return false;
        }

        private bool ZwWeapon()
        {
            return true;
        }

        private bool ReinforcementOfTheArmy()
        {
            AI.SelectCard(
                CardId.Goblindbergh,
                CardId.TinGoldfish,
                CardId.StarDrawing,
                CardId.Kagetokage,
                CardId.SacredCrane
                );
            return true;
        }

        private bool InstantFusion()
        {
            if (Bot.LifePoints <= 1000)
                return false;
            int count4 = 0;
            int count5 = 0;
            foreach (ClientCard card in Bot.GetMonsters())
            {
                if (card.Level == 5)
                    ++count5;
                if (card.Level == 4)
                    ++count4;
            }
            if (count5 == 1)
            {
                AI.SelectCard(CardId.FlameSwordsman);
                return true;
            }
            else if (count4 == 1)
            {
                AI.SelectCard(CardId.DarkfireDragon);
                return true;
            }
            return false;
        }

        private bool XyzChangeTactics()
        {
            return Bot.LifePoints > 500;
        }

        private bool GoblindberghFirst()
        {
            foreach (ClientCard card in Bot.Hand.GetMonsters())
            {
                if (!card.Equals(Card) && card.Level == 4)
                    return true;
            }
            return false;
        }

        private bool GoblindberghEffect()
        {
            AI.SelectCard(
                CardId.SacredCrane,
                CardId.HeroicChallengerExtraSword,
                CardId.StarDrawing,
                CardId.SummonerMonk
                );
            return true;
        }

        private bool KagetokageEffect()
        {
            var lastChainCard = Util.GetLastChainCard();
            if (lastChainCard == null) return true;
            return !lastChainCard.IsCode(CardId.Goblindbergh, CardId.TinGoldfish);
        }

        private bool SummonerMonkEffect()
        {
            IList<int> costs = new[]
                {
                    CardId.XyzChangeTactics,
                    CardId.DarkHole,
                    CardId.MysticalSpaceTyphoon,
                    CardId.InstantFusion
                };
            if (Bot.HasInHand(costs))
            {
                AI.SelectCard(costs);
                AI.SelectNextCard(
                    CardId.SacredCrane,
                    CardId.StarDrawing,
                    CardId.Goblindbergh,
                    CardId.TinGoldfish
                    );
                AI.SelectPosition(CardPosition.FaceUpDefence);
                return true;
            }
            return false;
        }

        private bool SolarWindJammer()
        {
            if (!Bot.HasInHand(new[] {
                    CardId.StarDrawing,
                    CardId.InstantFusion
                }))
                return false;
            AI.SelectPosition(CardPosition.FaceUpDefence);
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
