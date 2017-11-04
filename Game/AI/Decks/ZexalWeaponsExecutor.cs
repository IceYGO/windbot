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
            public static int CyberDragon = 70095155;
            public static int ZwTornadoBringer = 81471108;
            public static int ZwLightningBlade = 45082499;
            public static int ZwAsuraStrike = 40941889;
            public static int SolarWindJammer = 33911264;
            public static int PhotonTrasher = 65367484;
            public static int StarDrawing = 24610207;
            public static int SacredCrane = 30914564;
            public static int Goblindbergh = 25259669;
            public static int Honest = 37742478;
            public static int Kagetokage = 94656263;
            public static int HeroicChallengerExtraSword = 34143852;
            public static int TinGoldfish = 18063928;
            public static int SummonerMonk = 423585;
            public static int InstantFusion = 1845204;
            public static int Raigeki = 12580477;
            public static int ReinforcementOfTheArmy = 32807846;
            public static int DarkHole = 53129443;
            public static int MysticalSpaceTyphoon = 5318639;
            public static int BreakthroughSkill = 78474168;
            public static int SolemnWarning = 84749824;
            public static int SolemnStrike = 40605147;
            public static int XyzChangeTactics = 11705261;

            public static int FlameSwordsman = 45231177;
            public static int DarkfireDragon = 17881964;
            public static int GaiaDragonTheThunderCharger = 91949988;
            public static int ZwLionArms = 60992364;
            public static int AdreusKeeperOfArmageddon = 94119480;
            public static int Number61Volcasaurus = 29669359;
            public static int GemKnightPearl = 71594310;
            public static int Number39Utopia = 84013237;
            public static int NumberS39UtopiaOne = 86532744;
            public static int NumberS39UtopiatheLightning = 56832966;
            public static int MaestrokeTheSymphonyDjinn = 25341652;
            public static int GagagaCowboy = 12014404;
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
            AddExecutor(ExecutorType.Activate, CardId.NumberS39UtopiatheLightning, NumberS39UtopiatheLightning);
            AddExecutor(ExecutorType.Activate, CardId.ZwLionArms, ZwLionArms);
            AddExecutor(ExecutorType.Activate, CardId.AdreusKeeperOfArmageddon);
            AddExecutor(ExecutorType.Activate, CardId.Number61Volcasaurus);

            // Weapons
            AddExecutor(ExecutorType.Activate, CardId.ZwTornadoBringer);
            AddExecutor(ExecutorType.Activate, CardId.ZwLightningBlade);
            AddExecutor(ExecutorType.Activate, CardId.ZwAsuraStrike);


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

            // Summons: Effects
            AddExecutor(ExecutorType.Activate, CardId.Goblindbergh, GoblindberghEffect);
            AddExecutor(ExecutorType.Activate, CardId.TinGoldfish, GoblindberghEffect);
            AddExecutor(ExecutorType.Activate, CardId.Kagetokage);
            AddExecutor(ExecutorType.Activate, CardId.SummonerMonk, SummonerMonkEffect);
            AddExecutor(ExecutorType.Activate, CardId.Honest, Honest);

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

        private bool Number39Utopia()
        {
            if (!HasChainedTrap(0) && Duel.Player == 1 && Duel.Phase == DuelPhase.BattleStart && Card.HasXyzMaterial(2))
                return true;
            return false;
        }

        private bool Number61Volcasaurus()
        {
            return AI.Utils.IsOneEnemyBetterThanValue(2000, false);
        }

        private bool ZwLionArms()
        {
            if (ActivateDescription == AI.Utils.GetStringId(CardId.ZwLionArms, 0))
                return true;
            if (ActivateDescription == AI.Utils.GetStringId(CardId.ZwLionArms, 1))
                return !Card.IsDisabled();
            return false;
        }

        private bool ReinforcementOfTheArmy()
        {
            AI.SelectCard(new[]
                {
                    CardId.Goblindbergh,
                    CardId.TinGoldfish,
                    CardId.StarDrawing,
                    CardId.Kagetokage,
                    CardId.SacredCrane
                });
            return true;
        }

        private bool InstantFusion()
        {
            if (Duel.LifePoints[0] <= 1000)
                return false;
            List<ClientCard> monsters = Bot.GetMonsters();
            int count4 = 0;
            int count5 = 0;
            foreach (ClientCard card in monsters)
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
            return Duel.LifePoints[0] > 500;
        }

        private bool NumberS39UtopiatheLightning()
        {
            return Card.Attack < 5000;
        }

        private bool Honest()
        {
            return Duel.Phase != DuelPhase.Main1 || Duel.Turn == 1;
        }

        private bool GoblindberghFirst()
        {
            IList<ClientCard> hand = Bot.Hand;
            foreach (ClientCard card in hand)
            {
                if (card != Card && card.IsMonster() && card.Level == 4)
                    return true;
            }
            return false;
        }

        private bool GoblindberghEffect()
        {
            AI.SelectCard(new[]
                {
                    CardId.SacredCrane,
                    CardId.HeroicChallengerExtraSword,
                    CardId.StarDrawing,
                    CardId.SummonerMonk
                });
            return true;
        }

        private bool SummonerMonkEffect()
        {
            if (Bot.HasInHand(CardId.InstantFusion) ||
                Bot.HasInHand(CardId.MysticalSpaceTyphoon))
            {
                AI.SelectCard(new[]
                    {
                        CardId.InstantFusion,
                        CardId.MysticalSpaceTyphoon
                    });
                return true;
            }
            AI.SelectNextCard(new[]
                {
                    CardId.Goblindbergh,
                    CardId.TinGoldfish,
                    CardId.StarDrawing,
                    CardId.Kagetokage,
                    CardId.SacredCrane
                });
            return false;
        }

        private bool SolarWindJammer()
        {
            if (!Bot.HasInHand(new List<int> {
                    CardId.StarDrawing,
                    CardId.InstantFusion
                }))
                return false;
            AI.SelectPosition(CardPosition.FaceUpDefence);
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
