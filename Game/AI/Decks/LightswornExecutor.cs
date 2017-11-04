using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    // NOT FINISHED YET
    [Deck("Lightsworn", "AI_Lightsworn", "NotFinished")]
    public class LightswornExecutor : DefaultExecutor
    {
        public class CardId
        {
            public static int JudgmentDragon = 57774843;
            public static int Wulf = 58996430;
            public static int Garoth = 59019082;
            public static int Raiden = 77558536;
            public static int Lyla = 22624373;
            public static int Felis = 73176465;
            public static int Lumina = 95503687;
            public static int Minerva = 40164421;
            public static int Ryko = 21502796;
            public static int PerformageTrickClown = 67696066;
            public static int Goblindbergh = 25259669;
            public static int ThousandBlades = 1833916;
            public static int Honest = 37742478;
            public static int GlowUpBulb = 67441435;

            public static int SolarRecharge = 691925;
            public static int GalaxyCyclone = 5133471;
            public static int HarpiesFeatherDuster = 18144506;
            public static int ReinforcementOfTheArmy = 32807846;
            public static int MetalfoesFusion = 73594093;
            public static int ChargeOfTheLightBrigade = 94886282;

            public static int Michael = 4779823;
            public static int MinervaTheExalted = 30100551;
            public static int TrishulaDragonOfTheIceBarrier = 52687916;
            public static int ScarlightRedDragonArchfiend = 80666118;
            public static int PSYFramelordOmega = 74586817;
            public static int PSYFramelordZeta = 37192109;
            public static int NumberS39UtopiatheLightning = 56832966;
            public static int Number39Utopia = 84013237;
            public static int CastelTheSkyblasterMusketeer = 82633039;
            public static int EvilswarmExcitonKnight = 46772449;
            public static int DanteTravelerOfTheBurningAbyss = 83531441;
            public static int DecodeTalker = 1861629;
            public static int MissusRadiant = 3987233;
        }

        bool ClownUsed = false;

        public LightswornExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, CardId.HarpiesFeatherDuster, DefaultHarpiesFeatherDusterFirst);
            AddExecutor(ExecutorType.Activate, CardId.GalaxyCyclone, DefaultGalaxyCyclone);
            AddExecutor(ExecutorType.Activate, CardId.HarpiesFeatherDuster);

            AddExecutor(ExecutorType.Activate, CardId.MetalfoesFusion);
            AddExecutor(ExecutorType.Activate, CardId.GlowUpBulb);

            AddExecutor(ExecutorType.Activate, CardId.JudgmentDragon, DefaultDarkHole);
            AddExecutor(ExecutorType.SpSummon, CardId.JudgmentDragon);

            AddExecutor(ExecutorType.Activate, CardId.ReinforcementOfTheArmy, ReinforcementOfTheArmyEffect);
            AddExecutor(ExecutorType.Activate, CardId.ChargeOfTheLightBrigade, ChargeOfTheLightBrigadeEffect);
            AddExecutor(ExecutorType.Activate, CardId.SolarRecharge, SolarRechargeEffect);

            AddExecutor(ExecutorType.Summon, CardId.Goblindbergh, GoblindberghSummon);
            AddExecutor(ExecutorType.Activate, CardId.Goblindbergh, GoblindberghEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.EvilswarmExcitonKnight, DefaultEvilswarmExcitonKnightSummon);
            AddExecutor(ExecutorType.Activate, CardId.EvilswarmExcitonKnight, DefaultEvilswarmExcitonKnightEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.CastelTheSkyblasterMusketeer, DefaultCastelTheSkyblasterMusketeerSummon);
            AddExecutor(ExecutorType.Activate, CardId.CastelTheSkyblasterMusketeer, DefaultCastelTheSkyblasterMusketeerEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.ScarlightRedDragonArchfiend, DefaultScarlightRedDragonArchfiendSummon);
            AddExecutor(ExecutorType.Activate, CardId.ScarlightRedDragonArchfiend, DefaultScarlightRedDragonArchfiendEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.Number39Utopia, DefaultNumberS39UtopiaTheLightningSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.NumberS39UtopiatheLightning);
            AddExecutor(ExecutorType.Activate, CardId.NumberS39UtopiatheLightning);

            AddExecutor(ExecutorType.Activate, CardId.PerformageTrickClown, PerformageTrickClownEffect);
            AddExecutor(ExecutorType.Activate, CardId.ThousandBlades);
            AddExecutor(ExecutorType.Activate, CardId.Honest, HonestEffect);

            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
        }

        public override void OnNewTurn()
        {
            ClownUsed = false;
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

        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, bool cancelable)
        {
            if (max == 2 && min == 2 && cards[0].Location == CardLocation.MonsterZone)
            {
                Logger.DebugWriteLine("OnSelectCard XYZ");
                IList<ClientCard> avail = new List<ClientCard>();
                foreach (ClientCard card in cards)
                {
                    // clone
                    avail.Add(card);
                }
                IList<ClientCard> result = new List<ClientCard>();
                foreach (ClientCard card in cards)
                {
                    if (!result.Contains(card) && (!ClownUsed || card.Id != CardId.PerformageTrickClown))
                        result.Add(card);
                    if (result.Count >= 2)
                        break;
                }
                if (result.Count < 2)
                {
                    foreach (ClientCard card in cards)
                    {
                        if (!result.Contains(card))
                            result.Add(card);
                        if (result.Count >= 2)
                            break;
                    }
                }
                return result;
            }
            Logger.DebugWriteLine("Use default.");
            return null;
        }

        private bool ReinforcementOfTheArmyEffect()
        {
            if (!Bot.HasInHand(CardId.Goblindbergh))
                AI.SelectCard(CardId.Goblindbergh);
            else if (!Bot.HasInHand(CardId.Raiden))
                AI.SelectCard(CardId.Raiden);
            return true;
        }

        private bool ChargeOfTheLightBrigadeEffect()
        {
            if (!Bot.HasInHand(CardId.Lumina))
                AI.SelectCard(CardId.Lumina);
            else
                AI.SelectCard(new[]
                {
                    CardId.Raiden,
                    CardId.Lumina,
                    CardId.Minerva,
                    CardId.Lyla
                });
            return true;
        }

        private bool SolarRechargeEffect()
        {
            AI.SelectCard(new[]
                {
                    CardId.Wulf,
                    CardId.Felis,
                    CardId.Minerva,
                    CardId.Lyla,
                    CardId.Raiden
                });
            return true;
        }

        private bool GoblindberghSummon()
        {
            foreach (ClientCard card in Bot.Hand)
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
                    CardId.Felis,
                    CardId.Wulf,
                    CardId.Raiden,
                    CardId.PerformageTrickClown,
                    CardId.ThousandBlades
                });
            return true;
        }

        private bool PerformageTrickClownEffect()
        {
            ClownUsed = true;
            AI.SelectPosition(CardPosition.FaceUpDefence);
            return true;
        }

        private bool HonestEffect()
        {
            return Duel.Phase != DuelPhase.Main1;
        }
    }
}