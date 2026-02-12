using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("Toadally Awesome", "AI_ToadallyAwesome")]
    public class ToadallyAwesomeExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int CryomancerOfTheIceBarrier = 23950192;
            public const int DewdarkOfTheIceBarrier = 90311614;
            public const int SwapFrog = 9126351;
            public const int PriorOfTheIceBarrier = 50088247;
            public const int Ronintoadin = 1357146;
            public const int DupeFrog = 46239604;
            public const int GraydleSlimeJr = 80250319;

            public const int GalaxyCyclone = 5133471;
            public const int HarpiesFeatherDuster = 18144506;
            public const int Surface = 33057951;
            public const int DarkHole = 53129443;
            public const int CardDestruction = 72892473;
            public const int FoolishBurial = 81439173;
            public const int MonsterReborn = 83764718;
            public const int MedallionOfTheIceBarrier = 84206435;
            public const int Salvage = 96947648;
            public const int AquariumStage = 29047353;

            public const int HeraldOfTheArcLight = 79606837;
            public const int ToadallyAwesome = 90809975;
            public const int SkyCavalryCentaurea = 36776089;
            public const int DaigustoPhoenix = 2766877;
            public const int CatShark = 84224627;

            public const int MysticalSpaceTyphoon = 5318639;
            public const int BookOfMoon = 14087893;
            public const int CallOfTheHaunted = 97077563;
            public const int TorrentialTribute = 53582587;
        }

        public ToadallyAwesomeExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, CardId.HarpiesFeatherDuster, DefaultHarpiesFeatherDusterFirst);
            AddExecutor(ExecutorType.Activate, CardId.GalaxyCyclone, DefaultGalaxyCyclone);
            AddExecutor(ExecutorType.Activate, CardId.HarpiesFeatherDuster);
            AddExecutor(ExecutorType.Activate, CardId.DarkHole, DefaultDarkHole);

            AddExecutor(ExecutorType.Activate, CardId.AquariumStage, AquariumStageEffect);
            AddExecutor(ExecutorType.Activate, CardId.MedallionOfTheIceBarrier, MedallionOfTheIceBarrierEffect);
            AddExecutor(ExecutorType.Activate, CardId.FoolishBurial, FoolishBurialEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.PriorOfTheIceBarrier);
            AddExecutor(ExecutorType.Summon, CardId.GraydleSlimeJr, GraydleSlimeJrSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.SwapFrog, SwapFrogSpsummon);

            AddExecutor(ExecutorType.Activate, CardId.SwapFrog, SwapFrogEffect);
            AddExecutor(ExecutorType.Activate, CardId.GraydleSlimeJr, GraydleSlimeJrEffect);
            AddExecutor(ExecutorType.Activate, CardId.Ronintoadin, RonintoadinEffect);
            AddExecutor(ExecutorType.Activate, CardId.PriorOfTheIceBarrier);
            AddExecutor(ExecutorType.Activate, CardId.DupeFrog);

            AddExecutor(ExecutorType.Activate, CardId.Surface, SurfaceEffect);
            AddExecutor(ExecutorType.Activate, CardId.MonsterReborn, SurfaceEffect);
            AddExecutor(ExecutorType.Activate, CardId.Salvage, SalvageEffect);

            AddExecutor(ExecutorType.Summon, CardId.SwapFrog);
            AddExecutor(ExecutorType.Summon, CardId.DewdarkOfTheIceBarrier, IceBarrierSummon);
            AddExecutor(ExecutorType.Summon, CardId.CryomancerOfTheIceBarrier, IceBarrierSummon);

            AddExecutor(ExecutorType.Activate, CardId.CardDestruction);

            AddExecutor(ExecutorType.Summon, CardId.GraydleSlimeJr, NormalSummon);
            AddExecutor(ExecutorType.Summon, CardId.PriorOfTheIceBarrier, NormalSummon);
            AddExecutor(ExecutorType.Summon, CardId.Ronintoadin, NormalSummon);
            AddExecutor(ExecutorType.Summon, CardId.DupeFrog, NormalSummon);
            AddExecutor(ExecutorType.Summon, CardId.PriorOfTheIceBarrier, PriorOfTheIceBarrierSummon);

            AddExecutor(ExecutorType.SpSummon, CardId.CatShark, CatSharkSummon);
            AddExecutor(ExecutorType.Activate, CardId.CatShark, CatSharkEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.SkyCavalryCentaurea, SkyCavalryCentaureaSummon);
            AddExecutor(ExecutorType.Activate, CardId.SkyCavalryCentaurea);
            AddExecutor(ExecutorType.SpSummon, CardId.DaigustoPhoenix, DaigustoPhoenixSummon);
            AddExecutor(ExecutorType.Activate, CardId.DaigustoPhoenix);
            AddExecutor(ExecutorType.SpSummon, CardId.ToadallyAwesome);
            AddExecutor(ExecutorType.Activate, CardId.ToadallyAwesome, ToadallyAwesomeEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.HeraldOfTheArcLight, HeraldOfTheArcLightSummon);
            AddExecutor(ExecutorType.Activate, CardId.HeraldOfTheArcLight);

            AddExecutor(ExecutorType.MonsterSet, CardId.GraydleSlimeJr);
            AddExecutor(ExecutorType.MonsterSet, CardId.DupeFrog);
            AddExecutor(ExecutorType.MonsterSet, CardId.Ronintoadin);

            AddExecutor(ExecutorType.Repos, Repos);

            // cards got by Toadally Awesome
            AddExecutor(ExecutorType.Activate, CardId.MysticalSpaceTyphoon, DefaultMysticalSpaceTyphoon);
            AddExecutor(ExecutorType.Activate, CardId.BookOfMoon, DefaultBookOfMoon);
            AddExecutor(ExecutorType.Activate, CardId.CallOfTheHaunted, SurfaceEffect);
            AddExecutor(ExecutorType.Activate, CardId.TorrentialTribute, DefaultTorrentialTribute);
            AddExecutor(ExecutorType.Activate, OtherSpellEffect);
            AddExecutor(ExecutorType.Activate, OtherTrapEffect);
            AddExecutor(ExecutorType.Activate, OtherMonsterEffect);
        }

        public override bool OnSelectHand()
        {
            return true;
        }

        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            if (!defender.IsMonsterHasPreventActivationEffectInBattle())
            {
                if (attacker.IsCode(CardId.SkyCavalryCentaurea) && !attacker.IsDisabled() && attacker.HasXyzMaterial())
                    attacker.RealPower = Bot.LifePoints + attacker.Attack;
            }
            return base.OnPreBattleBetween(attacker, defender);
        }

        private bool MedallionOfTheIceBarrierEffect()
        {
            if (Bot.HasInHand(new[]
                {
                    CardId.CryomancerOfTheIceBarrier,
                    CardId.DewdarkOfTheIceBarrier
                }) || Bot.HasInMonstersZone(new[]
                {
                    CardId.CryomancerOfTheIceBarrier,
                    CardId.DewdarkOfTheIceBarrier
                }))
            {
                AI.SelectCard(CardId.PriorOfTheIceBarrier);
            }
            else
            {
                AI.SelectCard(
                    CardId.CryomancerOfTheIceBarrier,
                    CardId.DewdarkOfTheIceBarrier
                    );
            }
            return true;
        }

        private bool SurfaceEffect()
        {
            AI.SelectCard(
                CardId.ToadallyAwesome,
                CardId.HeraldOfTheArcLight,
                CardId.SwapFrog,
                CardId.DewdarkOfTheIceBarrier,
                CardId.CryomancerOfTheIceBarrier,
                CardId.DupeFrog,
                CardId.Ronintoadin,
                CardId.GraydleSlimeJr
                );
            return true;
        }

        private bool AquariumStageEffect()
        {
            if (Card.Location == CardLocation.Grave)
            {
                AI.SelectPosition(CardPosition.FaceUpDefence);
                return SurfaceEffect();
            }
            return true;
        }

        private bool FoolishBurialEffect()
        {
            if (Bot.HasInHand(CardId.GraydleSlimeJr) && !Bot.HasInGraveyard(CardId.GraydleSlimeJr))
                AI.SelectCard(CardId.GraydleSlimeJr);
            else if (Bot.HasInGraveyard(CardId.Ronintoadin) && !Bot.HasInGraveyard(CardId.DupeFrog))
                AI.SelectCard(CardId.DupeFrog);
            else if (Bot.HasInGraveyard(CardId.DupeFrog) && !Bot.HasInGraveyard(CardId.Ronintoadin))
                AI.SelectCard(CardId.Ronintoadin);
            else
                AI.SelectCard(
                    CardId.GraydleSlimeJr,
                    CardId.Ronintoadin,
                    CardId.DupeFrog,
                    CardId.CryomancerOfTheIceBarrier,
                    CardId.DewdarkOfTheIceBarrier,
                    CardId.PriorOfTheIceBarrier,
                    CardId.SwapFrog
                    );
            return true;
        }

        private bool SalvageEffect()
        {
            AI.SelectCard(
                CardId.SwapFrog,
                CardId.PriorOfTheIceBarrier,
                CardId.GraydleSlimeJr
                );
            return true;
        }

        private bool SwapFrogSpsummon()
        {
            if (Bot.GetCountCardInZone(Bot.Hand, CardId.GraydleSlimeJr)>=2 && !Bot.HasInGraveyard(CardId.GraydleSlimeJr))
                AI.SelectCard(CardId.GraydleSlimeJr);
            else if (Bot.HasInGraveyard(CardId.Ronintoadin) && !Bot.HasInGraveyard(CardId.DupeFrog))
                AI.SelectCard(CardId.DupeFrog);
            else if (Bot.HasInGraveyard(CardId.DupeFrog) && !Bot.HasInGraveyard(CardId.Ronintoadin))
                AI.SelectCard(CardId.Ronintoadin);
            else
                AI.SelectCard(
                    CardId.Ronintoadin,
                    CardId.DupeFrog,
                    CardId.CryomancerOfTheIceBarrier,
                    CardId.DewdarkOfTheIceBarrier,
                    CardId.PriorOfTheIceBarrier,
                    CardId.GraydleSlimeJr,
                    CardId.SwapFrog
                    );
            return true;
        }

        private bool SwapFrogEffect()
        {
            if (ActivateDescription == -1)
            {
                return FoolishBurialEffect();
            }
            else
            {
                if (DefaultCheckWhetherCardIsNegated(Card)) return false;
                if (Bot.HasInHand(CardId.DupeFrog))
                {
                    AI.SelectCard(
                        CardId.PriorOfTheIceBarrier,
                        CardId.GraydleSlimeJr,
                        CardId.SwapFrog
                        );
                    return true;
                }
            }
            return false;
        }

        private bool GraydleSlimeJrSummon()
        {
            return Bot.HasInGraveyard(CardId.GraydleSlimeJr);
        }

        private bool GraydleSlimeJrEffect()
        {
            AI.SelectCard(CardId.GraydleSlimeJr);
            AI.SelectPosition(CardPosition.FaceUpDefence);
            AI.SelectNextCard(
                CardId.SwapFrog,
                CardId.CryomancerOfTheIceBarrier,
                CardId.DewdarkOfTheIceBarrier,
                CardId.Ronintoadin,
                CardId.DupeFrog,
                CardId.PriorOfTheIceBarrier,
                CardId.GraydleSlimeJr
                );
            return true;
        }

        private bool RonintoadinEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            AI.SelectPosition(CardPosition.FaceUpDefence);
            return true;
        }

        private bool NormalSummon()
        {
            foreach (ClientCard monster in Bot.GetMonsters())
            {
                if (monster.Level==2)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IceBarrierSummon()
        {
            return Bot.GetCountCardInZone(Bot.Hand, CardId.PriorOfTheIceBarrier) > 0;
        }

        private bool PriorOfTheIceBarrierSummon()
        {
            return Bot.GetCountCardInZone(Bot.Hand, CardId.PriorOfTheIceBarrier) >= 2;
        }

        private bool ToadallyAwesomeEffect()
        {
            if (Duel.CurrentChain.Count > 0)
            {
                if (DefaultCheckWhetherCardIsNegated(Card)) return false;
                // negate effect, select a cost for it
                List<ClientCard> monsters = Bot.GetMonsters();
                IList<int> suitableCost = new[] {
                    CardId.SwapFrog,
                    CardId.Ronintoadin,
                    CardId.GraydleSlimeJr,
                    CardId.CryomancerOfTheIceBarrier,
                    CardId.DewdarkOfTheIceBarrier
                };
                foreach (ClientCard monster in monsters)
                {
                    if (monster.IsCode(suitableCost))
                    {
                        AI.SelectCard(monster);
                        return true;
                    }
                }
                if (!Bot.HasInSpellZone(CardId.AquariumStage, true))
                {
                    foreach (ClientCard monster in monsters)
                    {
                        if (monster.IsCode(CardId.DupeFrog))
                        {
                            AI.SelectCard(monster);
                            return true;
                        }
                    }
                }
                List<ClientCard> hands = Bot.Hand.GetMonsters();
                if (Bot.GetCountCardInZone(Bot.Hand, CardId.GraydleSlimeJr) >= 2)
                {
                    foreach (ClientCard monster in hands)
                    {
                        if (monster.IsCode(CardId.GraydleSlimeJr))
                        {
                            AI.SelectCard(monster);
                            return true;
                        }
                    }
                }
                if (Bot.HasInGraveyard(CardId.Ronintoadin) && !Bot.HasInGraveyard(CardId.DupeFrog) && !Bot.HasInGraveyard(CardId.SwapFrog))
                {
                    foreach (ClientCard monster in hands)
                    {
                        if (monster.IsCode(CardId.DupeFrog))
                        {
                            AI.SelectCard(monster);
                            return true;
                        }
                    }
                }
                foreach (ClientCard monster in hands)
                {
                    if (monster.IsCode(CardId.Ronintoadin, CardId.DupeFrog))
                    {
                        AI.SelectCard(monster);
                        return true;
                    }
                }
                foreach (ClientCard monster in hands)
                {
                    AI.SelectCard(monster);
                    return true;
                }
                return true;
            }
            else if (Card.Location == CardLocation.Grave)
            {
                if (!Bot.HasInExtra(CardId.ToadallyAwesome))
                {
                    AI.SelectCard(CardId.ToadallyAwesome);
                }
                else
                {
                    AI.SelectCard(
                        CardId.SwapFrog,
                        CardId.PriorOfTheIceBarrier,
                        CardId.GraydleSlimeJr
                        );
                }
                return true;
            }
            else if (Duel.Phase == DuelPhase.Standby)
            {
                if (DefaultCheckWhetherCardIsNegated(Card)) return false;
                SelectXYZDetach(Card.Overlays);
                if (Duel.Player == 0)
                {
                    AI.SelectNextCard(
                        CardId.SwapFrog,
                        CardId.CryomancerOfTheIceBarrier,
                        CardId.DewdarkOfTheIceBarrier,
                        CardId.Ronintoadin,
                        CardId.DupeFrog,
                        CardId.GraydleSlimeJr
                        );
                }
                else
                {
                    AI.SelectNextCard(
                        CardId.DupeFrog,
                        CardId.SwapFrog,
                        CardId.Ronintoadin,
                        CardId.GraydleSlimeJr,
                        CardId.CryomancerOfTheIceBarrier,
                        CardId.DewdarkOfTheIceBarrier
                        );
                    AI.SelectPosition(CardPosition.FaceUpDefence);
                }
                return true;
            }
            return true;
        }

        private bool CatSharkSummon()
        {
            if (Bot.HasInMonstersZone(CardId.ToadallyAwesome)
                && ((Util.IsOneEnemyBetter(true)
                    && !Bot.HasInMonstersZone(new[]
                        {
                            CardId.CatShark,
                            CardId.SkyCavalryCentaurea
                        }, true, true))
                    || !Bot.HasInExtra(CardId.ToadallyAwesome)))
            {
                AI.SelectPosition(CardPosition.FaceUpDefence);
                return true;
            }
            return false;
        }

        private bool CatSharkEffect()
        {
            List<ClientCard> monsters = Bot.GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                if (monster.IsCode(CardId.ToadallyAwesome) && monster.Attack <= 2200)
                {
                    SelectXYZDetach(Card.Overlays);
                    AI.SelectNextCard(monster);
                    return true;
                }
            }
            foreach (ClientCard monster in monsters)
            {
                if (monster.IsCode(CardId.SkyCavalryCentaurea) && monster.Attack <= 2000)
                {
                    SelectXYZDetach(Card.Overlays);
                    AI.SelectNextCard(monster);
                    return true;
                }
            }
            foreach (ClientCard monster in monsters)
            {
                if (monster.IsCode(CardId.DaigustoPhoenix) && monster.Attack <= 1500)
                {
                    SelectXYZDetach(Card.Overlays);
                    AI.SelectNextCard(monster);
                    return true;
                }
            }
            return false;
        }

        private bool SkyCavalryCentaureaSummon()
        {
            int num = 0;
            foreach (ClientCard monster in Bot.GetMonsters())
            {
                if (monster.Level ==2)
                {
                    num++;
                }
            }
            return Util.IsOneEnemyBetter(true)
                   && Util.GetBestAttack(Enemy) > 2200
                   && num < 4
                   && !Bot.HasInMonstersZone(new[]
                        {
                            CardId.SkyCavalryCentaurea
                        }, true, true);
        }

        private bool DaigustoPhoenixSummon()
        {
            if (Duel.Turn != 1)
            {
                int attack = 0;
                int defence = 0;
                foreach (ClientCard monster in Bot.GetMonsters())
                {
                    if (!monster.IsDefense())
                    {
                        attack += monster.Attack;
                    }
                }
                foreach (ClientCard monster in Enemy.GetMonsters())
                {
                    defence += monster.GetDefensePower();
                }
                if (attack - 2000 - defence > Enemy.LifePoints && !Util.IsOneEnemyBetter(true))
                    return true;
            }
            return false;
        }

        private bool HeraldOfTheArcLightSummon()
        {
            AI.SelectPosition(CardPosition.FaceUpDefence);
            return true;
        }

        private bool Repos()
        {
            if (Card.IsFacedown())
                return true;
            if (Card.IsDefense() && !Util.IsAllEnemyBetter(true) && Card.Attack >= Card.Defense)
                return true;
            return false;
        }

        private bool OtherSpellEffect()
        {
            foreach (CardExecutor exec in Executors)
            {
                if (exec.Type == Type && exec.CardId == Card.Id)
                    return false;
            }
            return Card.IsSpell();
        }

        private bool OtherTrapEffect()
        {
            foreach (CardExecutor exec in Executors)
            {
                if (exec.Type == Type && exec.CardId == Card.Id)
                    return false;
            }
            return Card.IsTrap() && DefaultTrap();
        }

        private bool OtherMonsterEffect()
        {
            foreach (CardExecutor exec in Executors)
            {
                if (exec.Type == Type && exec.CardId == Card.Id)
                    return false;
            }
            return Card.IsMonster();
        }

        private void SelectXYZDetach(List<int> Overlays)
        {
            if (Overlays.Contains(CardId.GraydleSlimeJr) && Bot.HasInHand(CardId.GraydleSlimeJr) && !Bot.HasInGraveyard(CardId.GraydleSlimeJr))
                AI.SelectCard(CardId.GraydleSlimeJr);
            else if (Overlays.Contains(CardId.DupeFrog) && Bot.HasInGraveyard(CardId.Ronintoadin) && !Bot.HasInGraveyard(CardId.DupeFrog))
                AI.SelectCard(CardId.DupeFrog);
            else if (Overlays.Contains(CardId.Ronintoadin) && Bot.HasInGraveyard(CardId.DupeFrog) && !Bot.HasInGraveyard(CardId.Ronintoadin))
                AI.SelectCard(CardId.Ronintoadin);
            else
                AI.SelectCard(
                    CardId.GraydleSlimeJr,
                    CardId.Ronintoadin,
                    CardId.DupeFrog,
                    CardId.CryomancerOfTheIceBarrier,
                    CardId.DewdarkOfTheIceBarrier,
                    CardId.PriorOfTheIceBarrier,
                    CardId.SwapFrog
                    );
        }
    }
}
