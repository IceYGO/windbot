using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    // NOT FINISHED YET
    [Deck("Lightsworn", "AI_Lightsworn")]
    public class LightswornExecutor : DefaultExecutor
    {
        public enum CardId
        {
            JudgmentDragon = 57774843,
            Wulf = 58996430,
            Garoth = 59019082,
            Raiden = 77558536,
            Lyla = 22624373,
            Felis = 73176465,
            Lumina = 95503687,
            Minerva = 40164421,
            Ryko = 21502796,
            PerformageTrickClown = 67696066,
            Goblindbergh = 25259669,
            ThousandBlades = 1833916,
            Honest = 37742478,
            GlowUpBulb = 67441435,

            SolarRecharge = 691925,
            GalaxyCyclone = 5133471,
            HarpiesFeatherDuster = 18144506,
            ReinforcementOfTheArmy = 32807846,
            MetalfoesFusion = 73594093,
            ChargeOfTheLightBrigade = 94886282,

            Michael = 4779823,
            MinervaTheExalted = 30100551,
            TrishulaDragonOfTheIceBarrier = 52687916,
            ScarlightRedDragonArchfiend = 80666118,
            PSYFramelordOmega = 74586817,
            PSYFramelordZeta = 37192109,
            NumberS39UtopiatheLightning = 56832966,
            Number39Utopia = 84013237,
            CastelTheSkyblasterMusketeer = 82633039,
            EvilswarmExcitonKnight = 46772449,
            DanteTravelerOfTheBurningAbyss = 83531441,
            DecodeTalker = 1861629,
            MissusRadiant = 3987233

        }

        bool ClownUsed = false;

        public LightswornExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, (int)CardId.HarpiesFeatherDuster, DefaultHarpiesFeatherDusterFirst);
            AddExecutor(ExecutorType.Activate, (int)CardId.GalaxyCyclone, DefaultGalaxyCyclone);
            AddExecutor(ExecutorType.Activate, (int)CardId.HarpiesFeatherDuster);

            AddExecutor(ExecutorType.Activate, (int)CardId.MetalfoesFusion);
            AddExecutor(ExecutorType.Activate, (int)CardId.GlowUpBulb);

            AddExecutor(ExecutorType.Activate, (int)CardId.JudgmentDragon, DefaultDarkHole);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.JudgmentDragon);

            AddExecutor(ExecutorType.Activate, (int)CardId.ReinforcementOfTheArmy, ReinforcementOfTheArmyEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.ChargeOfTheLightBrigade, ChargeOfTheLightBrigadeEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.SolarRecharge, SolarRechargeEffect);

            AddExecutor(ExecutorType.Summon, (int)CardId.Goblindbergh, GoblindberghSummon);
            AddExecutor(ExecutorType.Activate, (int)CardId.Goblindbergh, GoblindberghEffect);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.EvilswarmExcitonKnight, EvilswarmExcitonKnightSummon);
            AddExecutor(ExecutorType.Activate, (int)CardId.EvilswarmExcitonKnight, EvilswarmExcitonKnightEffect);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.CastelTheSkyblasterMusketeer, CastelTheSkyblasterMusketeerSummon);
            AddExecutor(ExecutorType.Activate, (int)CardId.CastelTheSkyblasterMusketeer, CastelTheSkyblasterMusketeerEffect);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.ScarlightRedDragonArchfiend, ScarlightRedDragonArchfiendSummon);
            AddExecutor(ExecutorType.Activate, (int)CardId.ScarlightRedDragonArchfiend, ScarlightRedDragonArchfiendEffect);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.Number39Utopia, NumberS39UtopiatheLightningSummon);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.NumberS39UtopiatheLightning);
            AddExecutor(ExecutorType.Activate, (int)CardId.NumberS39UtopiatheLightning);

            AddExecutor(ExecutorType.Activate, (int)CardId.PerformageTrickClown, PerformageTrickClownEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.ThousandBlades);
            AddExecutor(ExecutorType.Activate, (int)CardId.Honest, HonestEffect);

            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
        }

        public override void OnNewTurn()
        {
            ClownUsed = false;
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
                if (attacker.Attribute == (int)CardAttribute.Light && Bot.HasInHand((int)CardId.Honest))
                    attacker.RealPower = attacker.RealPower + defender.Attack;
                if (attacker.Id == (int)CardId.NumberS39UtopiatheLightning && !attacker.IsDisabled() && attacker.HasXyzMaterial(2, (int)CardId.Number39Utopia))
                    attacker.RealPower = 5000;
            }
            return attacker.RealPower > defender.GetDefensePower();
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
                    if (!result.Contains(card) && (!ClownUsed || card.Id != (int)CardId.PerformageTrickClown))
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
            if (!Bot.HasInHand((int)CardId.Goblindbergh))
                AI.SelectCard((int)CardId.Goblindbergh);
            else if (!Bot.HasInHand((int)CardId.Raiden))
                AI.SelectCard((int)CardId.Raiden);
            return true;
        }

        private bool ChargeOfTheLightBrigadeEffect()
        {
            if (!Bot.HasInHand((int)CardId.Lumina))
                AI.SelectCard((int)CardId.Lumina);
            else
                AI.SelectCard(new[]
                {
                    (int)CardId.Raiden,
                    (int)CardId.Lumina,
                    (int)CardId.Minerva,
                    (int)CardId.Lyla
                });
            return true;
        }

        private bool SolarRechargeEffect()
        {
            AI.SelectCard(new[]
                {
                    (int)CardId.Wulf,
                    (int)CardId.Felis,
                    (int)CardId.Minerva,
                    (int)CardId.Lyla,
                    (int)CardId.Raiden
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
                    (int)CardId.Felis,
                    (int)CardId.Wulf,
                    (int)CardId.Raiden,
                    (int)CardId.PerformageTrickClown,
                    (int)CardId.ThousandBlades
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

        private bool EvilswarmExcitonKnightSummon()
        {
            int selfCount = Bot.GetMonsterCount() + Bot.GetSpellCount() + Bot.GetHandCount();
            int oppoCount = Enemy.GetMonsterCount() + Enemy.GetSpellCount() + Enemy.GetHandCount();
            return (selfCount - 1 < oppoCount) && EvilswarmExcitonKnightEffect();
        }

        private bool EvilswarmExcitonKnightEffect()
        {
            int selfCount = Bot.GetMonsterCount() + Bot.GetSpellCount();
            int oppoCount = Enemy.GetMonsterCount() + Enemy.GetSpellCount();

            int selfAttack = 0;
            List<ClientCard> monsters = Bot.GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                selfAttack += monster.GetDefensePower();
            }

            int oppoAttack = 0;
            monsters = Enemy.GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                oppoAttack += monster.GetDefensePower();
            }

            return (selfCount < oppoCount) || (selfAttack < oppoAttack);
        }

        private bool ScarlightRedDragonArchfiendSummon()
        {
            int selfBestAttack = AI.Utils.GetBestAttack(Bot);
            int oppoBestAttack = AI.Utils.GetBestPower(Enemy);
            return (selfBestAttack <= oppoBestAttack && oppoBestAttack <= 3000) || ScarlightRedDragonArchfiendEffect();
        }

        private bool ScarlightRedDragonArchfiendEffect()
        {
            int selfCount = 0;
            List<ClientCard> monsters = Bot.GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                if (!monster.Equals(Card) && monster.HasType(CardType.Effect) && monster.Attack <= Card.Attack)
                    selfCount++;
            }

            int oppoCount = 0;
            monsters = Enemy.GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                if (monster.HasType(CardType.Effect) && monster.Attack <= Card.Attack)
                    oppoCount++;
            }

            return (oppoCount > 0 && selfCount <= oppoCount) || oppoCount > 2;
        }

        private bool CastelTheSkyblasterMusketeerSummon()
        {
            return AI.Utils.GetProblematicEnemyCard() != null;
        }

        private bool CastelTheSkyblasterMusketeerEffect()
        {
            if (ActivateDescription == AI.Utils.GetStringId((int)CardId.CastelTheSkyblasterMusketeer, 0))
                return false;
            AI.SelectNextCard(AI.Utils.GetProblematicEnemyCard());
            return true;
        }

        private bool NumberS39UtopiatheLightningSummon()
        {
            return AI.Utils.IsOneEnemyBetter();
        }
    }
}