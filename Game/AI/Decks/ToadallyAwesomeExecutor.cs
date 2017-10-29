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
        public enum CardId
        {
            CryomancerOfTheIceBarrier = 23950192,
            DewdarkOfTheIceBarrier = 90311614,
            SwapFrog = 9126351,
            PriorOfTheIceBarrier = 50088247,
            Ronintoadin = 1357146,
            DupeFrog = 46239604,
            GraydleSlimeJr = 80250319,

            GalaxyCyclone = 5133471,
            HarpiesFeatherDuster = 18144506,
            Surface = 33057951,
            DarkHole = 53129443,
            CardDestruction = 72892473,
            FoolishBurial = 81439173,
            MonsterReborn = 83764718,
            MedallionOfTheIceBarrier = 84206435,
            Salvage = 96947648,
            AquariumStage = 29047353,

            HeraldOfTheArcLight = 79606837,
            ToadallyAwesome = 90809975,
            SkyCavalryCentaurea = 36776089,
            DaigustoPhoenix = 2766877,
            CatShark = 84224627,

            MysticalSpaceTyphoon = 5318639,
            BookOfMoon = 14087893,
            CallOfTheHaunted = 97077563,
            TorrentialTribute = 53582587,

            NumberS39UtopiatheLightning = 56832966
        }

        public ToadallyAwesomeExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, (int)CardId.HarpiesFeatherDuster, DefaultHarpiesFeatherDusterFirst);
            AddExecutor(ExecutorType.Activate, (int)CardId.GalaxyCyclone, DefaultGalaxyCyclone);
            AddExecutor(ExecutorType.Activate, (int)CardId.HarpiesFeatherDuster);
            AddExecutor(ExecutorType.Activate, (int)CardId.DarkHole, DefaultDarkHole);

            AddExecutor(ExecutorType.Activate, (int)CardId.AquariumStage, AquariumStageEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.MedallionOfTheIceBarrier, MedallionOfTheIceBarrierEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.FoolishBurial, FoolishBurialEffect);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.PriorOfTheIceBarrier);
            AddExecutor(ExecutorType.Summon, (int)CardId.GraydleSlimeJr, GraydleSlimeJrSummon);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.SwapFrog, SwapFrogSpsummon);

            AddExecutor(ExecutorType.Activate, (int)CardId.SwapFrog, SwapFrogEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.GraydleSlimeJr, GraydleSlimeJrEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.Ronintoadin, RonintoadinEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.PriorOfTheIceBarrier);
            AddExecutor(ExecutorType.Activate, (int)CardId.DupeFrog);

            AddExecutor(ExecutorType.Activate, (int)CardId.Surface, SurfaceEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.MonsterReborn, SurfaceEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.Salvage, SalvageEffect);

            AddExecutor(ExecutorType.Summon, (int)CardId.SwapFrog);
            AddExecutor(ExecutorType.Summon, (int)CardId.DewdarkOfTheIceBarrier, IceBarrierSummon);
            AddExecutor(ExecutorType.Summon, (int)CardId.CryomancerOfTheIceBarrier, IceBarrierSummon);

            AddExecutor(ExecutorType.Activate, (int)CardId.CardDestruction);

            AddExecutor(ExecutorType.Summon, (int)CardId.GraydleSlimeJr, NormalSummon);
            AddExecutor(ExecutorType.Summon, (int)CardId.PriorOfTheIceBarrier, NormalSummon);
            AddExecutor(ExecutorType.Summon, (int)CardId.Ronintoadin, NormalSummon);
            AddExecutor(ExecutorType.Summon, (int)CardId.DupeFrog, NormalSummon);
            AddExecutor(ExecutorType.Summon, (int)CardId.PriorOfTheIceBarrier, PriorOfTheIceBarrierSummon);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.CatShark, CatSharkSummon);
            AddExecutor(ExecutorType.Activate, (int)CardId.CatShark, CatSharkEffect);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.SkyCavalryCentaurea, SkyCavalryCentaureaSummon);
            AddExecutor(ExecutorType.Activate, (int)CardId.SkyCavalryCentaurea);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.DaigustoPhoenix, DaigustoPhoenixSummon);
            AddExecutor(ExecutorType.Activate, (int)CardId.DaigustoPhoenix);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.ToadallyAwesome);
            AddExecutor(ExecutorType.Activate, (int)CardId.ToadallyAwesome, ToadallyAwesomeEffect);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.HeraldOfTheArcLight, HeraldOfTheArcLightSummon);
            AddExecutor(ExecutorType.Activate, (int)CardId.HeraldOfTheArcLight);

            AddExecutor(ExecutorType.MonsterSet, (int)CardId.GraydleSlimeJr);
            AddExecutor(ExecutorType.MonsterSet, (int)CardId.DupeFrog);
            AddExecutor(ExecutorType.MonsterSet, (int)CardId.Ronintoadin);

            AddExecutor(ExecutorType.Repos, Repos);

            // cards got by Toadall yAwesome
            AddExecutor(ExecutorType.Activate, (int)CardId.MysticalSpaceTyphoon, DefaultMysticalSpaceTyphoon);
            AddExecutor(ExecutorType.Activate, (int)CardId.BookOfMoon, DefaultBookOfMoon);
            AddExecutor(ExecutorType.Activate, (int)CardId.CallOfTheHaunted, SurfaceEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.TorrentialTribute, DefaultTorrentialTribute);
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
            if (defender.IsMonsterInvincible())
            {
                if (defender.IsMonsterDangerous() || defender.IsDefense())
                    return false;
            }
            if (!(defender.Id == (int)CardId.NumberS39UtopiatheLightning))
            {
                if (attacker.Id == (int)CardId.SkyCavalryCentaurea && !attacker.IsDisabled() && attacker.HasXyzMaterial())
                    attacker.RealPower = Duel.LifePoints[0] + attacker.Attack;
            }
            return attacker.RealPower >= defender.GetDefensePower();
        }

        private bool MedallionOfTheIceBarrierEffect()
        {
            if (Bot.HasInHand(new List<int>
                {
                    (int)CardId.CryomancerOfTheIceBarrier,
                    (int)CardId.DewdarkOfTheIceBarrier
                }) || Bot.HasInMonstersZone(new List<int>
                {
                    (int)CardId.CryomancerOfTheIceBarrier,
                    (int)CardId.DewdarkOfTheIceBarrier
                }))
            {
                AI.SelectCard((int)CardId.PriorOfTheIceBarrier);
            }
            else
            {
                AI.SelectCard(new[]
                    {
                    (int)CardId.CryomancerOfTheIceBarrier,
                    (int)CardId.DewdarkOfTheIceBarrier
                });
            }
            return true;
        }

        private bool SurfaceEffect()
        {
            AI.SelectCard(new[]
                {
                    (int)CardId.ToadallyAwesome,
                    (int)CardId.HeraldOfTheArcLight,
                    (int)CardId.SwapFrog,
                    (int)CardId.DewdarkOfTheIceBarrier,
                    (int)CardId.CryomancerOfTheIceBarrier,
                    (int)CardId.DupeFrog,
                    (int)CardId.Ronintoadin,
                    (int)CardId.GraydleSlimeJr
                });
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
            if (Bot.HasInHand((int)CardId.GraydleSlimeJr) && !Bot.HasInGraveyard((int)CardId.GraydleSlimeJr))
                AI.SelectCard((int)CardId.GraydleSlimeJr);
            else if (Bot.HasInGraveyard((int)CardId.Ronintoadin) && !Bot.HasInGraveyard((int)CardId.DupeFrog))
                AI.SelectCard((int)CardId.DupeFrog);
            else if (Bot.HasInGraveyard((int)CardId.DupeFrog) && !Bot.HasInGraveyard((int)CardId.Ronintoadin))
                AI.SelectCard((int)CardId.Ronintoadin);
            else
                AI.SelectCard(new[]
                    {
                        (int)CardId.GraydleSlimeJr,
                        (int)CardId.Ronintoadin,
                        (int)CardId.DupeFrog,
                        (int)CardId.CryomancerOfTheIceBarrier,
                        (int)CardId.DewdarkOfTheIceBarrier,
                        (int)CardId.PriorOfTheIceBarrier,
                        (int)CardId.SwapFrog
                    });
            return true;
        }

        private bool SalvageEffect()
        {
            AI.SelectCard(new[]
                {
                    (int)CardId.SwapFrog,
                    (int)CardId.PriorOfTheIceBarrier,
                    (int)CardId.GraydleSlimeJr
                });
            return true;
        }

        private bool SwapFrogSpsummon()
        {
            if (Bot.GetCountCardInZone(Bot.Hand, (int)CardId.GraydleSlimeJr)>=2 && !Bot.HasInGraveyard((int)CardId.GraydleSlimeJr))
                AI.SelectCard((int)CardId.GraydleSlimeJr);
            else if (Bot.HasInGraveyard((int)CardId.Ronintoadin) && !Bot.HasInGraveyard((int)CardId.DupeFrog))
                AI.SelectCard((int)CardId.DupeFrog);
            else if (Bot.HasInGraveyard((int)CardId.DupeFrog) && !Bot.HasInGraveyard((int)CardId.Ronintoadin))
                AI.SelectCard((int)CardId.Ronintoadin);
            else
                AI.SelectCard(new[]
                    {
                        (int)CardId.Ronintoadin,
                        (int)CardId.DupeFrog,
                        (int)CardId.CryomancerOfTheIceBarrier,
                        (int)CardId.DewdarkOfTheIceBarrier,
                        (int)CardId.PriorOfTheIceBarrier,
                        (int)CardId.GraydleSlimeJr,
                        (int)CardId.SwapFrog
                    });
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
                if (Bot.HasInHand((int)CardId.DupeFrog))
                {
                    AI.SelectCard(new[]
                        {
                            (int)CardId.PriorOfTheIceBarrier,
                            (int)CardId.GraydleSlimeJr,
                            (int)CardId.SwapFrog
                        });
                    return true;
                }
            }
            return false;
        }

        private bool GraydleSlimeJrSummon()
        {
            return Bot.HasInGraveyard((int)CardId.GraydleSlimeJr);
        }

        private bool GraydleSlimeJrEffect()
        {
            AI.SelectCard((int)CardId.GraydleSlimeJr);
            AI.SelectPosition(CardPosition.FaceUpDefence);
            AI.SelectNextCard(new[]
                {
                    (int)CardId.SwapFrog,
                    (int)CardId.CryomancerOfTheIceBarrier,
                    (int)CardId.DewdarkOfTheIceBarrier,
                    (int)CardId.Ronintoadin,
                    (int)CardId.DupeFrog,
                    (int)CardId.PriorOfTheIceBarrier,
                    (int)CardId.GraydleSlimeJr
                });
            return true;
        }

        private bool RonintoadinEffect()
        {
            AI.SelectPosition(CardPosition.FaceUpDefence);
            return true;
        }

        private bool NormalSummon()
        {
            List<ClientCard> monsters = Bot.GetMonsters();
            foreach (ClientCard monster in monsters)
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
            return Bot.GetCountCardInZone(Bot.Hand, (int)CardId.PriorOfTheIceBarrier) > 0;
        }

        private bool PriorOfTheIceBarrierSummon()
        {
            return Bot.GetCountCardInZone(Bot.Hand, (int)CardId.PriorOfTheIceBarrier) >= 2;
        }

        private bool ToadallyAwesomeEffect()
        {
            if (CurrentChain.Count > 0)
            {
                // negate effect, select a cost for it
                List<ClientCard> monsters = Bot.GetMonsters();
                List<int> suitableCost = new List<int> {
                    (int)CardId.SwapFrog,
                    (int)CardId.Ronintoadin,
                    (int)CardId.GraydleSlimeJr,
                    (int)CardId.CryomancerOfTheIceBarrier,
                    (int)CardId.DewdarkOfTheIceBarrier
                };
                foreach (ClientCard monster in monsters)
                {
                    if (suitableCost.Contains(monster.Id))
                    {
                        AI.SelectCard(monster);
                        return true;
                    }
                }
                bool haveAquariumStage = Bot.HasInSpellZone((int)CardId.AquariumStage, true);
                foreach (ClientCard monster in monsters)
                {
                    if (monster.Id == (int)CardId.DupeFrog && !haveAquariumStage)
                    {
                        AI.SelectCard(monster);
                        return true;
                    }
                }
                monsters = (List<ClientCard>)Bot.Hand;
                bool HaveTwoGraydleSlimeJrInHand = Bot.GetCountCardInZone(Bot.Hand, (int)CardId.GraydleSlimeJr) >= 2;
                foreach (ClientCard monster in monsters)
                {
                    if (monster.Id == (int)CardId.GraydleSlimeJr && HaveTwoGraydleSlimeJrInHand)
                    {
                        AI.SelectCard(monster);
                        return true;
                    }
                }
                bool NeedDupeFrogInGrave = Bot.HasInGraveyard((int)CardId.Ronintoadin) && !Bot.HasInGraveyard((int)CardId.DupeFrog) && !Bot.HasInGraveyard((int)CardId.SwapFrog);
                foreach (ClientCard monster in monsters)
                {
                    if (monster.Id == (int)CardId.DupeFrog && NeedDupeFrogInGrave)
                    {
                        AI.SelectCard(monster);
                        return true;
                    }
                }
                foreach (ClientCard monster in monsters)
                {
                    if (monster.Id == (int)CardId.Ronintoadin || monster.Id == (int)CardId.DupeFrog)
                    {
                        AI.SelectCard(monster);
                        return true;
                    }
                }
                foreach (ClientCard monster in monsters)
                {
                    AI.SelectCard(monster);
                    return true;
                }
                return true;
            }
            else if (Card.Location == CardLocation.Grave)
            {
                if (!Bot.HasInExtra((int)CardId.ToadallyAwesome))
                {
                    AI.SelectCard((int)CardId.ToadallyAwesome);
                }
                else
                {
                    AI.SelectCard(new[]
                        {
                            (int)CardId.SwapFrog,
                            (int)CardId.PriorOfTheIceBarrier,
                            (int)CardId.GraydleSlimeJr
                        });
                }
                return true;
            }
            else if (Duel.Phase == DuelPhase.Standby)
            {
                SelectXYZDetach(Card.Overlays);
                if (Duel.Player == 0)
                {
                    AI.SelectNextCard(new[]
                        {
                            (int)CardId.SwapFrog,
                            (int)CardId.CryomancerOfTheIceBarrier,
                            (int)CardId.DewdarkOfTheIceBarrier,
                            (int)CardId.Ronintoadin,
                            (int)CardId.DupeFrog,
                            (int)CardId.GraydleSlimeJr
                        });
                }
                else
                {
                    AI.SelectNextCard(new[]
                        {
                            (int)CardId.DupeFrog,
                            (int)CardId.SwapFrog,
                            (int)CardId.Ronintoadin,
                            (int)CardId.GraydleSlimeJr,
                            (int)CardId.CryomancerOfTheIceBarrier,
                            (int)CardId.DewdarkOfTheIceBarrier
                        });
                    AI.SelectPosition(CardPosition.FaceUpDefence);
                }
                return true;
            }
            return true;
        }

        private bool CatSharkSummon()
        {
            bool should = Bot.HasInMonstersZone((int)CardId.ToadallyAwesome)
                        && ((AI.Utils.IsEnemyBetter(true, false)
                            && !Bot.HasInMonstersZone(new List<int>
                                {
                                    (int)CardId.CatShark,
                                    (int)CardId.SkyCavalryCentaurea
                                }, true, true))
                        || !Bot.HasInExtra((int)CardId.ToadallyAwesome));
            if (should)
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
                if (monster.Id == (int)CardId.ToadallyAwesome && monster.Attack <= 2200)
                {
                    SelectXYZDetach(Card.Overlays);
                    AI.SelectNextCard(monster);
                    return true;
                }
            }
            foreach (ClientCard monster in monsters)
            {
                if (monster.Id == (int)CardId.SkyCavalryCentaurea && monster.Attack <= 2000)
                {
                    SelectXYZDetach(Card.Overlays);
                    AI.SelectNextCard(monster);
                    return true;
                }
            }
            foreach (ClientCard monster in monsters)
            {
                if (monster.Id == (int)CardId.DaigustoPhoenix && monster.Attack <= 1500)
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
            List<ClientCard> monsters = Bot.GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                if (monster.Level ==2)
                {
                    num++;
                }
            }
            return AI.Utils.IsEnemyBetter(true, false)
                   && AI.Utils.GetBestAttack(Enemy, true) > 2200
                   && num < 4
                   && !Bot.HasInMonstersZone(new List<int>
                        {
                            (int)CardId.SkyCavalryCentaurea
                        }, true, true);
        }

        private bool DaigustoPhoenixSummon()
        {
            if (Duel.Turn != 1)
            {
                int attack = 0;
                int defence = 0;
                List<ClientCard> monsters = Bot.GetMonsters();
                foreach (ClientCard monster in monsters)
                {
                    if (!monster.IsDefense())
                    {
                        attack += monster.Attack;
                    }
                }
                monsters = Enemy.GetMonsters();
                foreach (ClientCard monster in monsters)
                {
                    defence += monster.GetDefensePower();
                }
                if (attack - 2000 - defence > Duel.LifePoints[1] && !AI.Utils.IsEnemyBetter(true, false))
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
            bool enemyBetter = AI.Utils.IsEnemyBetter(true, true);

            if (Card.IsFacedown())
                return true;
            if (Card.IsDefense() && !enemyBetter && Card.Attack >= Card.Defense)
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
            if (Overlays.Contains((int)CardId.GraydleSlimeJr) && Bot.HasInHand((int)CardId.GraydleSlimeJr) && !Bot.HasInGraveyard((int)CardId.GraydleSlimeJr))
                AI.SelectCard((int)CardId.GraydleSlimeJr);
            else if (Overlays.Contains((int)CardId.DupeFrog) && Bot.HasInGraveyard((int)CardId.Ronintoadin) && !Bot.HasInGraveyard((int)CardId.DupeFrog))
                AI.SelectCard((int)CardId.DupeFrog);
            else if (Overlays.Contains((int)CardId.Ronintoadin) && Bot.HasInGraveyard((int)CardId.DupeFrog) && !Bot.HasInGraveyard((int)CardId.Ronintoadin))
                AI.SelectCard((int)CardId.Ronintoadin);
            else
                AI.SelectCard(new[]
                    {
                        (int)CardId.GraydleSlimeJr,
                        (int)CardId.Ronintoadin,
                        (int)CardId.DupeFrog,
                        (int)CardId.CryomancerOfTheIceBarrier,
                        (int)CardId.DewdarkOfTheIceBarrier,
                        (int)CardId.PriorOfTheIceBarrier,
                        (int)CardId.SwapFrog
                    });
        }
    }
}
