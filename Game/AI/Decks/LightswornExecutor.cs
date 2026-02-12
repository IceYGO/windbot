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
            public const int NumberS39UtopiatheLightning = 56832966;
            public const int Number39Utopia = 84013237;
            public const int CastelTheSkyblasterMusketeer = 82633039;
            public const int EvilswarmExcitonKnight = 46772449;
            public const int DanteTravelerOfTheBurningAbyss = 83531441;
            public const int DecodeTalker = 1861629;
            public const int MissusRadiant = 3987233;
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
            AddExecutor(ExecutorType.Activate, CardId.NumberS39UtopiatheLightning, DefaultNumberS39UtopiaTheLightningEffect);

            AddExecutor(ExecutorType.Activate, CardId.PerformageTrickClown, PerformageTrickClownEffect);
            AddExecutor(ExecutorType.Activate, CardId.ThousandBlades);
            AddExecutor(ExecutorType.Activate, CardId.Honest, DefaultHonestEffect);

            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
        }

        public override void OnNewTurn()
        {
            ClownUsed = false;
            base.OnNewTurn();
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
            Logger.DebugWriteLine("OnSelectXyzMaterial " + cards.Count + " " + min + " " + max);
            IList<ClientCard> result = new List<ClientCard>();
            foreach (ClientCard card in cards)
            {
                if (!result.Contains(card) && (!ClownUsed || !card.IsCode(CardId.PerformageTrickClown)))
                    result.Add(card);
                if (result.Count >= max)
                    break;
            }
            
            return Util.CheckSelectCount(result, cards, min, max);
        }

        private bool ReinforcementOfTheArmyEffect()
        {
            if (!Bot.HasInHand(CardId.Raiden))
            {
                AI.SelectCard(CardId.Raiden);
                return true;
            }
            else if (!Bot.HasInHand(CardId.Goblindbergh))
            {
                AI.SelectCard(CardId.Goblindbergh);
                return true;
            }
            return false;
        }

        private bool ChargeOfTheLightBrigadeEffect()
        {
            if (!Bot.HasInHand(CardId.Lumina))
                AI.SelectCard(CardId.Lumina);
            else
                AI.SelectCard(
                    CardId.Raiden,
                    CardId.Lumina,
                    CardId.Minerva,
                    CardId.Lyla
                    );
            return true;
        }

        private bool SolarRechargeEffect()
        {
            AI.SelectCard(
                CardId.Wulf,
                CardId.Felis,
                CardId.Minerva,
                CardId.Lyla,
                CardId.Raiden
                );
            return true;
        }

        private bool GoblindberghSummon()
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
                CardId.Felis,
                CardId.Wulf,
                CardId.Raiden,
                CardId.PerformageTrickClown,
                CardId.ThousandBlades
                );
            return true;
        }

        private bool LuminaEffect()
        {
            if (!Bot.HasInGraveyard(CardId.Raiden) && Bot.HasInHand(CardId.Raiden))
            {
                AI.SelectCard(CardId.Raiden);
            }
            else if (!ClownUsed && Bot.HasInHand(CardId.PerformageTrickClown))
            {
                AI.SelectCard(CardId.PerformageTrickClown);
            }
            else
            {
                AI.SelectCard(
                    CardId.Wulf,
                    CardId.Felis,
                    CardId.Minerva,
                    CardId.ThousandBlades
                    );
            }
            AI.SelectNextCard(CardId.Raiden, CardId.Felis);
            return true;
        }

        private bool PerformageTrickClownEffect()
        {
            ClownUsed = true;
            AI.SelectPosition(CardPosition.FaceUpDefence);
            return true;
        }

        private bool MinervaTheExaltedEffect()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {
                return true;
            }
            else
            {
                IList<ClientCard> targets = new List<ClientCard>();

                ClientCard target1 = Util.GetBestEnemyMonster();
                if (target1 != null)
                    targets.Add(target1);
                ClientCard target2 = Util.GetBestEnemySpell();
                if (target2 != null)
                    targets.Add(target2);

                foreach (ClientCard target in Enemy.GetMonsters())
                {
                    if (targets.Count >= 3)
                        break;
                    if (!targets.Contains(target))
                        targets.Add(target);
                }
                foreach (ClientCard target in Enemy.GetSpells())
                {
                    if (targets.Count >= 3)
                        break;
                    if (!targets.Contains(target))
                        targets.Add(target);
                }
                if (targets.Count == 0)
                    return false;

                AI.SelectNextCard(targets);
                return true;
            }
        }

    }
}