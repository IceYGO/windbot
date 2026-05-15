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
            public const int TimeThiefBezelShip = 82496079;
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
            AddExecutor(ExecutorType.Activate, Spells.Raigeki, DefaultDarkHole);
            AddExecutor(ExecutorType.Activate, Spells.FoolishBurial, FoolishBurialTarget);
            AddExecutor(ExecutorType.Activate, Spells.TimeThiefStartup, TimeThiefStartupEffect);
            AddExecutor(ExecutorType.Activate, Spells.TimeThiefHack);
            AddExecutor(ExecutorType.Activate, Spells.PotofExtravagance, PotofExtravaganceActivate);
            AddExecutor(ExecutorType.Activate, Spells.HarpieFeatherDuster, DefaultHarpiesFeatherDusterFirst);
            AddExecutor(ExecutorType.Activate, Spells.PotOfDesires, PotOfDesireseff);
            // trap executors set
            AddExecutor(ExecutorType.SpellSet, Traps.PhantomKnightsShade);
            AddExecutor(ExecutorType.SpellSet, Traps.TimeThiefRetrograte);
            AddExecutor(ExecutorType.SpellSet, Traps.TimeThiefFlyBack);
            AddExecutor(ExecutorType.SpellSet, Traps.SolemnWarning);
            AddExecutor(ExecutorType.SpellSet, Traps.SolemStrike);
            AddExecutor(ExecutorType.SpellSet, Traps.SolemnJudgment);
            AddExecutor(ExecutorType.SpellSet, Traps.Crackdown);
            //normal summons
            AddExecutor(ExecutorType.Summon, Monsters.TimeThiefRegulator);
            AddExecutor(ExecutorType.SpSummon, Monsters.PhotonTrasher, SummonToDef);
            AddExecutor(ExecutorType.Summon, Monsters.TimeThiefWinder);
            AddExecutor(ExecutorType.Summon, Monsters.TimeThiefBezelShip);
            AddExecutor(ExecutorType.Summon, Monsters.PerformTrickClown);
            AddExecutor(ExecutorType.Summon, Monsters.TimeThiefCronocorder);
            AddExecutor(ExecutorType.Summon, Monsters.ThunderKingRaiOh, ThunderKingRaiOhsummon);
            //xyz summons
            AddExecutor(ExecutorType.SpSummon, XYZs.TimeThiefRedoer);
            AddExecutor(ExecutorType.SpSummon, XYZs.TimeThiefPerpetua);
            AddExecutor(ExecutorType.SpSummon, XYZs.EvilswarmExcitonKnight, DefaultEvilswarmExcitonKnightSummon);
            AddExecutor(ExecutorType.SpSummon, XYZs.GagagaCowboy, GagagaCowboySummon);
            AddExecutor(ExecutorType.SpSummon, XYZs.Number39Utopia, DefaultNumberS39UtopiaTheLightningSummon);
            AddExecutor(ExecutorType.SpSummon, XYZs.NumberS39UtopiaOne);
            AddExecutor(ExecutorType.SpSummon, XYZs.NumberS39UtopiatheLightning);
            AddExecutor(ExecutorType.SpSummon, XYZs.DarkRebellionXyzDragon, DarkRebellionXyzDragonSummon);
            //activate trap
            AddExecutor(ExecutorType.Activate, Traps.PhantomKnightsShade);
            AddExecutor(ExecutorType.Activate, Traps.TimeThiefRetrograte, RetrograteEffect);
            AddExecutor(ExecutorType.Activate, Traps.TimeThiefFlyBack);
            AddExecutor(ExecutorType.Activate, Traps.SolemnWarning, DefaultSolemnWarning);
            AddExecutor(ExecutorType.Activate, Traps.SolemStrike, DefaultSolemnStrike);
            AddExecutor(ExecutorType.Activate, Traps.SolemnJudgment, DefaultSolemnJudgment);
            AddExecutor(ExecutorType.Activate, Traps.Crackdown, Crackdowneff);
            //xyz effects
            AddExecutor(ExecutorType.Activate, XYZs.TimeThiefRedoer, RedoerEffect);
            AddExecutor(ExecutorType.Activate, XYZs.TimeThiefPerpetua, PerpertuaEffect);
            AddExecutor(ExecutorType.Activate, XYZs.EvilswarmExcitonKnight, DefaultEvilswarmExcitonKnightEffect);
            AddExecutor(ExecutorType.Activate, XYZs.GagagaCowboy);
            AddExecutor(ExecutorType.Activate, XYZs.NumberS39UtopiatheLightning, DefaultNumberS39UtopiaTheLightningEffect);
            AddExecutor(ExecutorType.Activate, XYZs.DarkRebellionXyzDragon, DarkRebellionXyzDragonEffect);

            //monster effects
            AddExecutor(ExecutorType.Activate, Monsters.TimeThiefRegulator, RegulatorEffect);
            AddExecutor(ExecutorType.Activate, Monsters.TimeThiefWinder);
            AddExecutor(ExecutorType.Activate, Monsters.TimeThiefCronocorder);
            AddExecutor(ExecutorType.Activate, Monsters.PerformTrickClown, TrickClownEffect);
            AddExecutor(ExecutorType.Activate, Monsters.TimeThiefBezelShip);
            AddExecutor(ExecutorType.Activate, Monsters.ThunderKingRaiOh, ThunderKingRaiOheff);
            AddExecutor(ExecutorType.Activate, Monsters.AshBlossomAndJoyousSpring, DefaultAshBlossomAndJoyousSpring);
            AddExecutor(ExecutorType.Activate, Monsters.MaxxC, DefaultMaxxC);
        }

        public void SelectSTPlace(ClientCard card = null, bool avoid_Impermanence = false, List<int> avoid_list = null)
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
                    if (card != null && card.Location == CardLocation.Hand && avoid_Impermanence) continue;
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
                AI.SelectCard(Monsters.TimeThiefCronocorder);
                AI.SelectCard(Monsters.TimeThiefWinder);
                return true;
            }

            if (Card.Location == CardLocation.Grave)
            {
                return true;
            }

            return false;
        }

        private bool PerpertuaEffect()
        {
            if (Bot.HasInGraveyard(XYZs.TimeThiefRedoer))
            {
                AI.SelectCard(XYZs.TimeThiefRedoer);
                return true;
            }

            if (Bot.HasInMonstersZone(XYZs.TimeThiefRedoer))
            {
                AI.SelectCard(Monsters.TimeThiefBezelShip);
                AI.SelectNextCard(XYZs.TimeThiefRedoer);
                return true;
            }

            return false;
        }

        private int _totalAttack;
        private int _totalBotAttack;
        private bool RedoerEffect()
        {

            List<ClientCard> enemy = Enemy.GetMonstersInMainZone();
            List<int> units = Card.Overlays;
            if (Duel.Phase == DuelPhase.Standby && (AI.Executor.Util.GetStringId(XYZs.TimeThiefRedoer, 0) ==
                                                    ActivateDescription))
            {

                return true;
            }

            try
            {
                for (int i = 0; i < enemy.Count; i++)
                {
                    _totalAttack += enemy[i].Attack;
                }

                foreach (var t in Bot.GetMonsters())
                {
                    _totalBotAttack += t.Attack;
                }

                if (_totalAttack > Bot.LifePoints + _totalBotAttack)
                {
                    return false;
                }



                foreach (var t in enemy)
                {
                    if (t.Attack < 2400 || !t.IsAttack()) continue;
                    try
                    {
                        AI.SelectCard(t.Id);
                        AI.SelectCard(t.Id);
                    }
                    catch { }

                    return true;
                }
            }
            catch { }

            if (Bot.UnderAttack)
            {
                //AI.SelectCard(Util.GetBestEnemyMonster());
                return true;
            }

            return false;

        }
        private bool RetrograteEffect()
        {
            if (Card.Owner == 1)
            {
                return true;
            }
            return false;

        }
        private bool TimeThiefStartupEffect()
        {
            if (Card.Location == CardLocation.Hand)
            {
                if (Bot.HasInHand(Monsters.TimeThiefRegulator) && !(Bot.GetMonsterCount() > 0))
                {
                    AI.SelectCard(Monsters.TimeThiefRegulator);
                    return true;
                }
                if (Bot.HasInHand(Monsters.TimeThiefWinder) && Bot.GetMonsterCount() > 1)
                {
                    AI.SelectCard(Monsters.TimeThiefWinder);
                    return true;
                }
                return true;

            }
            if (Card.Location == CardLocation.Grave)
            {
                AI.SelectCard(Monsters.TimeThiefCronocorder);
                AI.SelectCard(Spells.TimeThiefHack);
                AI.SelectCard(Traps.TimeThiefFlyBack);
                return true;
            }

            return false;

        }
        private bool FoolishBurialTarget()
        {
            AI.SelectCard(Monsters.PerformTrickClown);
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
            int oppoBestAttack = Util.GetBestAttack(Enemy);
            ClientCard target = Util.GetOneEnemyBetterThanValue(oppoBestAttack, true);
            if (target != null)
            {
                AI.SelectCard(0);
                AI.SelectNextCard(target);
            }
            return true;
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
            if (Duel.SummoningCards.Count > 0)
            {
                foreach (ClientCard m in Duel.SummoningCards)
                {
                    if (m.Attack >= 1900)
                        return true;
                }
            }
            return false;
        }
        private bool Crackdowneff()
        {
            if (Util.GetOneEnemyBetterThanMyBest(true, true) != null && Bot.UnderAttack)
                AI.SelectCard(Util.GetOneEnemyBetterThanMyBest(true, true));
            return Util.GetOneEnemyBetterThanMyBest(true, true) != null && Bot.UnderAttack;
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
