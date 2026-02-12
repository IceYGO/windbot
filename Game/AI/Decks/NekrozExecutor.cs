using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    // NOT FINISHED YET
    [Deck("Nekroz", "AI_Nekroz", "NotFinished")]
    public class NekrozExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int DancePrincess = 52738610;
            public const int ThousandHands = 23401839;
            public const int TenThousandHands = 95492061;
            public const int Shurit = 90307777;
            public const int MaxxC = 23434538;
            public const int DecisiveArmor = 88240999;
            public const int Trishula = 52068432;
            public const int Valkyrus = 25857246;
            public const int Gungnir = 74122412;
            public const int Brionac = 26674724;
            public const int Unicore = 89463537;
            public const int Clausolas = 99185129;
            public const int PhantomOfChaos = 30312361;

            public const int DarkHole = 53129443;
            public const int ReinforcementOfTheArmy = 32807846;
            public const int TradeIn = 38120068;
            public const int PreparationOfRites = 96729612;
            public const int Mirror = 14735698;
            public const int Kaleidoscope = 51124303;
            public const int Cycle = 97211663;
            public const int MysticalSpaceTyphoon = 5318639;
            public const int RoyalDecree = 51452091;
            public const int EvilswarmExcitonKnight = 46772449;
            public const int HeraldOfTheArcLight = 79606837;
        }

        List<int> NekrozRituelCard = new List<int>();
        List<int> NekrozSpellCard = new List<int>();

        public NekrozExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            NekrozRituelCard.Add(CardId.Clausolas);
            NekrozRituelCard.Add(CardId.Unicore);
            NekrozRituelCard.Add(CardId.DecisiveArmor);
            NekrozRituelCard.Add(CardId.Brionac);
            NekrozRituelCard.Add(CardId.Trishula);
            NekrozRituelCard.Add(CardId.Gungnir);
            NekrozRituelCard.Add(CardId.Valkyrus);

            NekrozSpellCard.Add(CardId.Mirror);
            NekrozSpellCard.Add(CardId.Kaleidoscope);
            NekrozSpellCard.Add(CardId.Cycle);

            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);
            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);

            AddExecutor(ExecutorType.Activate, CardId.DarkHole, DefaultDarkHole);
            AddExecutor(ExecutorType.Activate, CardId.ReinforcementOfTheArmy, ReinforcementOfTheArmyEffect);
            AddExecutor(ExecutorType.Activate, CardId.TradeIn);
            AddExecutor(ExecutorType.Activate, CardId.PreparationOfRites);
            AddExecutor(ExecutorType.Activate, CardId.Mirror);
            AddExecutor(ExecutorType.Activate, CardId.Kaleidoscope);
            AddExecutor(ExecutorType.Activate, CardId.Cycle);
            AddExecutor(ExecutorType.Activate, CardId.MysticalSpaceTyphoon, DefaultMysticalSpaceTyphoon);
            AddExecutor(ExecutorType.Activate, CardId.RoyalDecree);

            AddExecutor(ExecutorType.SummonOrSet, CardId.DancePrincess, DancePrincessSummon);
            AddExecutor(ExecutorType.MonsterSet, CardId.Shurit, ShuritSet);
            AddExecutor(ExecutorType.Summon, CardId.ThousandHands, ThousandHandsSummon);
            AddExecutor(ExecutorType.Summon, CardId.TenThousandHands, TenThousandHandsSummon);
            AddExecutor(ExecutorType.Summon, CardId.PhantomOfChaos, PhantomOfChaosSummon);

            AddExecutor(ExecutorType.Activate, CardId.Unicore, UnicoreEffect);
            AddExecutor(ExecutorType.Activate, CardId.DecisiveArmor, DecisiveArmorEffect);
            AddExecutor(ExecutorType.Activate, CardId.Valkyrus, ValkyrusEffect);
            AddExecutor(ExecutorType.Activate, CardId.Gungnir, GungnirEffect);
            AddExecutor(ExecutorType.Activate, CardId.Brionac, BrionacEffect);
            AddExecutor(ExecutorType.Activate, CardId.Clausolas, ClausolasEffect);
            AddExecutor(ExecutorType.Activate, CardId.Trishula);
            AddExecutor(ExecutorType.Activate, CardId.EvilswarmExcitonKnight, DefaultEvilswarmExcitonKnightEffect);
            AddExecutor(ExecutorType.Activate, CardId.PhantomOfChaos, PhantomOfChaosEffect);
            AddExecutor(ExecutorType.Activate, CardId.MaxxC);
            AddExecutor(ExecutorType.Activate, CardId.ThousandHands, ThousandHandsEffect);
            AddExecutor(ExecutorType.Activate, CardId.TenThousandHands, BrionacEffect);
            AddExecutor(ExecutorType.Activate, CardId.HeraldOfTheArcLight);
            AddExecutor(ExecutorType.Activate, CardId.Shurit);

            AddExecutor(ExecutorType.SpSummon, CardId.Trishula);
            AddExecutor(ExecutorType.SpSummon, CardId.DecisiveArmor);
            AddExecutor(ExecutorType.SpSummon, CardId.Valkyrus);
            AddExecutor(ExecutorType.SpSummon, CardId.Gungnir);
            AddExecutor(ExecutorType.SpSummon, CardId.Brionac);
            AddExecutor(ExecutorType.SpSummon, CardId.Unicore);
            AddExecutor(ExecutorType.SpSummon, CardId.Clausolas);
            AddExecutor(ExecutorType.SpSummon, CardId.EvilswarmExcitonKnight, DefaultEvilswarmExcitonKnightSummon);
        }

        private bool ThousandHandsSummon()
        {
            if (!Bot.HasInHand(NekrozRituelCard) || Bot.HasInHand(CardId.Shurit) || !Bot.HasInHand(NekrozSpellCard))
                return true;
            foreach (ClientCard Card in Bot.Hand)
                if (Card != null && Card.IsCode(CardId.Kaleidoscope) && !Bot.HasInHand(CardId.Unicore))
                    return true;
                else if (Card.IsCode(CardId.Trishula) || Card.IsCode(CardId.DecisiveArmor) && !Bot.HasInHand(CardId.Mirror) || !Bot.HasInHand(CardId.Shurit))
                    return true;
            return false;
        }

        private bool ReinforcementOfTheArmyEffect()
        {
            if (!Bot.HasInGraveyard(CardId.Shurit) && !Bot.HasInHand(CardId.Shurit))
            {
                AI.SelectCard(CardId.Shurit);
                return true;
            }
            return false;
        }

        private bool TenThousandHandsSummon()
        {
                if (!Bot.HasInHand(CardId.ThousandHands) || !Bot.HasInHand(CardId.Shurit))
                return true;
            return false;
        }

        private bool DancePrincessSummon()
        {
            if (!Bot.HasInHand(CardId.ThousandHands) && !Bot.HasInHand(CardId.TenThousandHands))
                return true;
            return false;
        }

        private bool PhantomOfChaosSummon()
        {
            if (Bot.HasInGraveyard(CardId.Shurit) && Bot.HasInHand(NekrozSpellCard) && Bot.HasInHand(NekrozRituelCard))
                return true;
            return false;
        }

        private bool PhantomOfChaosEffect()
        {
            AI.SelectCard(CardId.Shurit);
            return true;
        }

        private bool ShuritSet()
        {
            if (!Bot.HasInHand(CardId.ThousandHands) && !Bot.HasInHand(CardId.TenThousandHands) && !Bot.HasInHand(CardId.DancePrincess))
                return true;
            return false;
        }

        private bool DecisiveArmorEffect()
        {
            if (Util.IsAllEnemyBetterThanValue(3300, true))
            {
                AI.SelectCard(CardId.DecisiveArmor);
                return true;
            }
            return false;
        }

        private bool ValkyrusEffect()
        {
            if (Duel.Phase == DuelPhase.Battle)
                return true;
            return false;
        }

        private bool GungnirEffect()
        {
            if (Util.IsOneEnemyBetter(true) && Duel.Phase == DuelPhase.Main1)
            {
                AI.SelectCard(Enemy.GetMonsters().GetHighestAttackMonster());
                return true;
            }
            return false;
        }

        private bool BrionacEffect()
        {
            if (!Bot.HasInHand(CardId.Shurit))
            {
                AI.SelectCard(CardId.Shurit);
                return true;
            }
            else if (!Bot.HasInHand(NekrozSpellCard))
            {
                AI.SelectCard(CardId.Mirror);
                return true;
            }
            else if (Util.IsOneEnemyBetterThanValue(3300, true) && !Bot.HasInHand(CardId.Trishula))
            {
                AI.SelectCard(CardId.Trishula);
                return true;
            }
            else if (Util.IsAllEnemyBetterThanValue(2700,true) && !Bot.HasInHand(CardId.DecisiveArmor))
            {
                AI.SelectCard(CardId.DecisiveArmor);
                return true;
            }
            else if (Bot.HasInHand(CardId.Unicore) && !Bot.HasInHand(CardId.Kaleidoscope))
            {
                AI.SelectCard(CardId.Kaleidoscope);
                return true;
            }
            else if (!Bot.HasInHand(CardId.Unicore) && Bot.HasInHand(CardId.Kaleidoscope))
            {
                AI.SelectCard(CardId.Unicore);
                return true;
            }
            return true;
        }

        private bool ThousandHandsEffect()
        {
            if (Util.IsOneEnemyBetterThanValue(3300, true) && !Bot.HasInHand(CardId.Trishula))
            {
                AI.SelectCard(CardId.Trishula);
                return true;
            }
            else if (Util.IsAllEnemyBetterThanValue(2700, true) && !Bot.HasInHand(CardId.DecisiveArmor))
            {
                AI.SelectCard(CardId.DecisiveArmor);
                return true;
            }
            else if (!Bot.HasInHand(CardId.Unicore) && Bot.HasInHand(CardId.Kaleidoscope))
            {
                AI.SelectCard(CardId.Unicore);
                return true;
            }
            return true;
        }

        private bool UnicoreEffect()
        {
            if (Bot.HasInGraveyard(CardId.Shurit))
            {
                AI.SelectCard(CardId.Shurit);
                return true;
            }
            return false;
        }

        private bool ClausolasEffect()
        {
            if (!Bot.HasInHand(NekrozSpellCard))
            {
                AI.SelectCard(CardId.Mirror);
                return true;
            }
            return false;
        }

        private bool IsTheLastPossibility()
        {
            if (!Bot.HasInHand(CardId.DecisiveArmor) && !Bot.HasInHand(CardId.Trishula))
                return true;
            return false;
        }

        private bool SelectNekrozWhoInvoke()
        {
            List<int> NekrozCard = new List<int>();
            try
            {
                foreach (ClientCard card in Bot.Hand)
                    if (card != null && card.IsCode(NekrozRituelCard))
                        NekrozCard.Add(card.Id);

                foreach (int Id in NekrozCard)
                {
                    if (Id == CardId.Trishula && Util.IsAllEnemyBetterThanValue(2700, true) && Bot.HasInHand(CardId.DecisiveArmor))
                    {
                        AI.SelectCard(CardId.Trishula);
                        return true;
                    }
                    else if (Id == CardId.DecisiveArmor)
                    {
                        AI.SelectCard(CardId.DecisiveArmor);
                        return true;
                    }
                    else if (Id == CardId.Unicore && Bot.HasInHand(CardId.Kaleidoscope) && !Bot.HasInGraveyard(CardId.Shurit))
                    {
                        AI.SelectCard(CardId.Unicore);
                        return true;
                    }
                    else if (Id == CardId.Valkyrus)
                    {
                        if (IsTheLastPossibility())
                        {
                            AI.SelectCard(CardId.Valkyrus);
                            return true;
                        }
                    }
                    else if (Id == CardId.Gungnir)
                    {
                        if (IsTheLastPossibility())
                        {
                            AI.SelectCard(CardId.Gungnir);
                            return true;
                        }
                    }
                    else if (Id == CardId.Clausolas)
                    {
                        if (IsTheLastPossibility())
                        {
                            AI.SelectCard(CardId.Clausolas);
                            return true;
                        }
                    }
                }
                return false;
            }
            catch
            { return false; }
        }
    }
}
