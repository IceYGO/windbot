using System;
using System.Collections.Generic;
using System.Linq;
using YGOSharp.OCGWrapper.Enums;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("HeroBeat1103", "AI_HeroBeat1103")]
    class HeroBeat1103Executor : DefaultExecutor
    {
        private const int HeroSetcode = 0x8;
        private const int ElementalHeroSetcode = 0x3008;

        public class CardId
        {
            public const int GorzTheEmissaryOfDarkness = 44330098; // 冥府之使者 格斯
            public const int ElementalHERONeosAlius = 69884162; // 元素英雄 次新宇侠
            public const int ThunderKingRaiOh = 71564252; // 雷王
            public const int ElementalHEROStratos = 40044918; // 元素英雄 天空侠
            public const int Honest = 37742478; // 欧尼斯特
            public const int ElementalHEROBubbleman = 79979666; // 元素英雄 水泡侠
            public const int EEmergencyCall = 213326; // E-紧急呼唤
            public const int ReinforcementOfTheArmy = 32807846; // 增援
            public const int MiracleFusion = 45906428; // 奇迹融合
            public const int DarkHole = 53129443; // 黑洞
            public const int MonsterReborn = 83764718; // 死者苏生
            public const int PotOfDuality = 98645731; // 强欲而谦虚之壶
            public const int MysticalSpaceTyphoon = 5318639; // 旋风
            public const int BookOfMoon = 14087893; // 月之书
            public const int ForbiddenLance = 27243130; // 禁忌的圣枪
            public const int GeminiSpark = 33846209; // 二重电光
            public const int SuperPolymerization = 48130397; // 超融合
            public const int BottomlessTrapHole = 29401950; // 奈落的落穴
            public const int HeroBlast = 37412656; // 英雄爆破
            public const int MirrorForce = 44095762; // 神圣防护罩 -反射镜力-
            public const int TorrentialTribute = 53582587; // 激流葬
            public const int DimensionalPrison = 70342110; // 次元幽闭
            public const int SolemnJudgment = 41420027; // 神之宣告
            public const int SolemnWarning = 84749824; // 神之警告
            public const int CyberDragon = 70095154; // 电子龙
            public const int DragonKnightDracoEquiste = 14017402; // 波动龙骑士
            public const int ChimeratechOverdragon = 64599569; // 嵌合超载龙
            public const int ElementalHEROGreatTornado = 3642509; // 元素英雄 大龙卷侠
            public const int ElementalHERONovaMaster = 1945387; // 元素英雄 新星主
            public const int ElementalHEROTheShining = 22061412; // 元素英雄 闪光侠
            public const int ElementalHEROEscuridao = 33574806; // 元素英雄 幽冥女郎
            public const int ElementalHEROAbsoluteZero = 40854197; // 元素英雄 绝对零度侠
            public const int ChimeratechFortressDragon = 79229522; // 嵌合要塞龙
            public const int ElementalHEROGaia = 16304628; // 元素英雄 大地侠
            public const int TrishulaDragonOfTheIceBarrier = 52687916; // 冰结界之龙 三叉龙
            public const int BrionacDragonOfTheIceBarrier = 50321796; // 冰结界之龙 光枪龙
            public const int GemKnightPearl = 71594310; // 宝石骑士·珍珠
            public const int Number39Utopia = 84013237; // No.39 希望皇 霍普
            public const int EvigishkiMerrowgeist = 76372778; // 邪遗式人鱼风灵
        }

        public HeroBeat1103Executor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, CardId.GorzTheEmissaryOfDarkness);
            AddExecutor(ExecutorType.Activate, CardId.Honest, DefaultHonestEffect);
            AddExecutor(ExecutorType.Activate, CardId.ThunderKingRaiOh, ThunderKingRaiOhActivate);

            AddExecutor(ExecutorType.Activate, CardId.DarkHole, DefaultDarkHole);
            AddExecutor(ExecutorType.Activate, CardId.EEmergencyCall, SearchHeroActivate);
            AddExecutor(ExecutorType.Activate, CardId.ReinforcementOfTheArmy, SearchHeroActivate);
            AddExecutor(ExecutorType.Activate, CardId.MonsterReborn, MonsterRebornActivate);
            AddExecutor(ExecutorType.Activate, CardId.MiracleFusion, MiracleFusionActivate);

            AddExecutor(ExecutorType.SpSummon, CardId.ElementalHEROBubbleman, BubblemanSpSummon);

            AddExecutor(ExecutorType.Summon, CardId.ElementalHEROStratos);
            AddExecutor(ExecutorType.Summon, CardId.ThunderKingRaiOh);
            AddExecutor(ExecutorType.Summon, CardId.ElementalHERONeosAlius, NeosAliusSummon);
            AddExecutor(ExecutorType.Summon, CardId.ElementalHEROBubbleman, BubblemanSummon);
            AddExecutor(ExecutorType.MonsterSet, CardId.ElementalHEROBubbleman);

            AddExecutor(ExecutorType.Activate, CardId.ElementalHEROStratos, StratosEffect);
            AddExecutor(ExecutorType.Activate, CardId.ElementalHEROBubbleman);
            AddExecutor(ExecutorType.Activate, CardId.ElementalHEROGreatTornado);
            AddExecutor(ExecutorType.Activate, CardId.ElementalHERONovaMaster);
            AddExecutor(ExecutorType.Activate, CardId.ElementalHEROTheShining, ElementalHEROTheShiningActivate);
            AddExecutor(ExecutorType.Activate, CardId.ElementalHEROGaia);
            AddExecutor(ExecutorType.Activate, CardId.Number39Utopia, Number39UtopiaActivate);
            AddExecutor(ExecutorType.Activate, CardId.EvigishkiMerrowgeist);

            AddExecutor(ExecutorType.SpSummon, CardId.ChimeratechFortressDragon);
            AddExecutor(ExecutorType.SpSummon, CardId.GemKnightPearl, GemKnightPearlSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.EvigishkiMerrowgeist, EvigishkiMerrowgeistSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.Number39Utopia, Number39UtopiaSummon);

            AddExecutor(ExecutorType.Activate, CardId.GeminiSpark, GeminiSparkActivate);
            AddExecutor(ExecutorType.Activate, CardId.MysticalSpaceTyphoon, DefaultMysticalSpaceTyphoon);
            AddExecutor(ExecutorType.Activate, CardId.BookOfMoon, DefaultBookOfMoon);
            AddExecutor(ExecutorType.Activate, CardId.ForbiddenLance, ForbiddenLanceActivate);
            AddExecutor(ExecutorType.Activate, CardId.SuperPolymerization, SuperPolymerizationActivate);
            AddExecutor(ExecutorType.Activate, CardId.PotOfDuality, PotOfDualityActivate);

            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);

            AddExecutor(ExecutorType.Activate, CardId.HeroBlast, HeroBlastActivate);
            AddExecutor(ExecutorType.Activate, CardId.BottomlessTrapHole, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, CardId.MirrorForce, MirrorForceActivate);
            AddExecutor(ExecutorType.Activate, CardId.DimensionalPrison, DefaultTrap);
            AddExecutor(ExecutorType.Activate, CardId.TorrentialTribute, DefaultTorrentialTribute);
            AddExecutor(ExecutorType.Activate, CardId.SolemnJudgment, DefaultSolemnJudgment);
            AddExecutor(ExecutorType.Activate, CardId.SolemnWarning, DefaultSolemnWarning);

            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
        }

        public override IList<ClientCard> OnSelectCard(
            IList<ClientCard> cards, int min, int max, int hint, bool cancelable)
        {
            ClientCard currentChainCard = Duel.GetCurrentChainCard();
            if (currentChainCard != null &&
                currentChainCard.Controller == 0 &&
                currentChainCard.IsCode(CardId.ElementalHEROGaia) &&
                hint == HintMsg.Faceup)
            {
                ClientCard target = Util.GetBestEnemyMonster(true, true);
                List<ClientCard> targets = new List<ClientCard>();
                if (target != null && cards.Contains(target))
                    targets.Add(target);
                targets.AddRange(cards
                    .Where(card => !targets.Contains(card))
                    .OrderByDescending(card => card.GetDefensePower()));
                return Util.CheckSelectCount(targets, cards, min, max);
            }

            return base.OnSelectCard(cards, min, max, hint, cancelable);
        }

        public override bool OnSelectHand()
        {
            return true;
        }

        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            if (!defender.IsMonsterHasPreventActivationEffectInBattle() &&
                attacker.HasAttribute(CardAttribute.Light) &&
                Bot.HasInHand(CardId.Honest))
            {
                attacker.RealPower += defender.Attack;
            }
            return base.OnPreBattleBetween(attacker, defender);
        }

        private bool IsElementalHero(ClientCard card)
        {
            return card != null && card.IsMonster() && card.HasSetcode(ElementalHeroSetcode);
        }

        private List<int> GetHeroSearchPriority()
        {
            List<int> priority = new List<int>();
            int stratosCount = Bot.GetRemainingCount(CardId.ElementalHEROStratos, 1);
            int neosAliusCount = Bot.GetRemainingCount(CardId.ElementalHERONeosAlius, 3);
            int bubblemanCount = Bot.GetRemainingCount(CardId.ElementalHEROBubbleman, 1);

            if (stratosCount > 0 &&
                !Bot.HasInHand(CardId.ElementalHEROStratos) &&
                !Bot.HasInMonstersZone(CardId.ElementalHEROStratos))
                priority.Add(CardId.ElementalHEROStratos);
            if (neosAliusCount > 0 &&
                !Bot.HasInHand(CardId.ElementalHERONeosAlius) &&
                !Bot.HasInMonstersZone(CardId.ElementalHERONeosAlius))
                priority.Add(CardId.ElementalHERONeosAlius);
            if (bubblemanCount > 0 && !Bot.HasInHand(CardId.ElementalHEROBubbleman))
                priority.Add(CardId.ElementalHEROBubbleman);

            if (neosAliusCount > 0 && !priority.Contains(CardId.ElementalHERONeosAlius))
                priority.Add(CardId.ElementalHERONeosAlius);
            if (bubblemanCount > 0 && !priority.Contains(CardId.ElementalHEROBubbleman))
                priority.Add(CardId.ElementalHEROBubbleman);
            if (stratosCount > 0 && !priority.Contains(CardId.ElementalHEROStratos))
                priority.Add(CardId.ElementalHEROStratos);
            return priority;
        }

        private bool TryGetFusion(IList<ClientCard> materials, bool requireEnemyMaterial,
            out int fusionId, out IList<ClientCard> fusionMaterials)
        {
            fusionId = 0;
            fusionMaterials = null;

            List<ClientCard> available = materials
                .Where(card => card != null && card.IsMonster())
                .ToList();
            if (requireEnemyMaterial && Bot.HasInExtra(CardId.DragonKnightDracoEquiste))
            {
                foreach (ClientCard dragonSynchro in available
                    .Where(card => card.HasType(CardType.Synchro) && card.HasRace(CardRace.Dragon))
                    .OrderByDescending(card => card.Controller))
                {
                    ClientCard warrior = available
                        .Where(card =>
                            !card.Equals(dragonSynchro) &&
                            card.HasRace(CardRace.Warrior) &&
                            (dragonSynchro.Controller == 1 || card.Controller == 1))
                        .OrderByDescending(card => card.Controller)
                        .FirstOrDefault();
                    if (warrior == null)
                        continue;

                    fusionId = CardId.DragonKnightDracoEquiste;
                    fusionMaterials = new List<ClientCard> { dragonSynchro, warrior };
                    return true;
                }
            }

            // 嵌合超载龙会在融合召唤后把自己的其他场上卡送墓，只在不会亏掉己方场面时使用。
            if (requireEnemyMaterial &&
                Bot.HasInExtra(CardId.ChimeratechOverdragon) &&
                Bot.GetMonsters().Count == 0 &&
                Bot.GetSpells().All(card => card.Equals(Card)))
            {
                ClientCard cyberDragon = available
                    .Where(card => card.Controller == 1 && card.IsCode(CardId.CyberDragon))
                    .FirstOrDefault();
                if (cyberDragon != null)
                {
                    List<ClientCard> machineMaterials = available
                        .Where(card =>
                            card.Controller == 1 &&
                            !card.Equals(cyberDragon) &&
                            card.HasRace(CardRace.Machine))
                        .ToList();
                    if (machineMaterials.Count > 0)
                    {
                        fusionId = CardId.ChimeratechOverdragon;
                        List<ClientCard> selectedMaterials = new List<ClientCard> { cyberDragon };
                        selectedMaterials.AddRange(machineMaterials);
                        fusionMaterials = selectedMaterials;
                        return true;
                    }
                }
            }

            int[] fusionPriority;
            CardAttribute[] requiredAttributes;
            if (Enemy.GetMonsters().Count(card => card.IsFaceup()) >= 2)
            {
                fusionPriority = new[]
                {
                    CardId.ElementalHEROGreatTornado,
                    CardId.ElementalHEROGaia,
                    CardId.ElementalHEROTheShining,
                    CardId.ElementalHEROAbsoluteZero,
                    CardId.ElementalHERONovaMaster,
                    CardId.ElementalHEROEscuridao
                };
                requiredAttributes = new[]
                {
                    CardAttribute.Wind,
                    CardAttribute.Earth,
                    CardAttribute.Light,
                    CardAttribute.Water,
                    CardAttribute.Fire,
                    CardAttribute.Dark
                };
            }
            else
            {
                fusionPriority = new[]
                {
                    CardId.ElementalHEROTheShining,
                    CardId.ElementalHEROAbsoluteZero,
                    CardId.ElementalHEROGreatTornado,
                    CardId.ElementalHEROGaia,
                    CardId.ElementalHERONovaMaster,
                    CardId.ElementalHEROEscuridao
                };
                requiredAttributes = new[]
                {
                    CardAttribute.Light,
                    CardAttribute.Water,
                    CardAttribute.Wind,
                    CardAttribute.Earth,
                    CardAttribute.Fire,
                    CardAttribute.Dark
                };
            }

            for (int i = 0; i < fusionPriority.Length; ++i)
            {
                if (!Bot.HasInExtra(fusionPriority[i]))
                    continue;

                foreach (ClientCard hero in available
                    .Where(IsElementalHero)
                    .OrderByDescending(card => requireEnemyMaterial && card.Controller == 1))
                {
                    ClientCard attributeMaterial = available
                        .Where(card =>
                            !card.Equals(hero) &&
                            card.HasAttribute(requiredAttributes[i]) &&
                            (!requireEnemyMaterial || hero.Controller == 1 || card.Controller == 1))
                        .OrderByDescending(card => requireEnemyMaterial && card.Controller == 1)
                        .FirstOrDefault();
                    if (attributeMaterial == null)
                        continue;

                    fusionId = fusionPriority[i];
                    fusionMaterials = new List<ClientCard> { hero, attributeMaterial };
                    return true;
                }
            }
            return false;
        }

        private bool ThunderKingRaiOhActivate()
        {
            ClientCard summonedCard = Duel.SummoningCards
                .FirstOrDefault(card => card.Controller == 1);
            return summonedCard != null &&
                (summonedCard.IsExtraCard() ||
                    summonedCard.IsMonsterDangerous() ||
                    summonedCard.Attack >= Card.Attack);
        }

        private bool SearchHeroActivate()
        {
            List<int> priority = GetHeroSearchPriority();
            if (priority.Count == 0)
                return false;
            AI.SelectCard(priority);
            return true;
        }

        private bool NeosAliusSummon()
        {
            // 再度召唤只会把卡名变成「元素英雄 新宇侠」，不会解锁二重电光。
            return Card.Location == CardLocation.Hand;
        }

        private bool StratosEffect()
        {
            List<ClientCard> enemySpells = Enemy.GetSpells();
            ClientCard floodgate = Enemy.SpellZone.GetFloodgate();
            List<int> searchPriority = GetHeroSearchPriority();
            int otherHeroCount = Bot.GetMonsters().Count(card =>
                !card.Equals(Card) && card.IsFaceup() && card.HasSetcode(HeroSetcode));
            if (otherHeroCount > 0 && enemySpells.Count > 0 &&
                (searchPriority.Count == 0 || floodgate != null || enemySpells.Count >= 2))
            {
                AI.SelectOption(0);
                List<ClientCard> targets = new List<ClientCard>();
                if (floodgate != null)
                    targets.Add(floodgate);
                targets.AddRange(enemySpells.Where(card => card.IsFacedown() && !targets.Contains(card)));
                targets.AddRange(enemySpells.Where(card => !targets.Contains(card)));
                AI.SelectCard(targets);
                return true;
            }

            if (searchPriority.Count == 0)
                return false;

            AI.SelectOption(1);
            AI.SelectCard(searchPriority);
            return true;
        }

        private bool BubblemanSpSummon()
        {
            return Bot.IsFieldEmpty() ||
                Enemy.GetMonsterCount() == 0 ||
                Bot.GetMonsters().Any(card =>
                    card.IsFaceup() &&
                    card.Level == 4 &&
                    !card.IsCode(CardId.ThunderKingRaiOh));
        }

        private bool BubblemanSummon()
        {
            return Bot.Hand.Count == 1 && Bot.IsFieldEmpty() ||
                Enemy.GetMonsterCount() == 0 ||
                Bot.GetMonsters().Any(card =>
                    card.IsFaceup() &&
                    card.Level == 4 &&
                    !card.IsCode(CardId.ThunderKingRaiOh));
        }

        private bool ElementalHEROTheShiningActivate()
        {
            AI.SelectCard(
                CardId.ElementalHEROStratos,
                CardId.ElementalHERONeosAlius,
                CardId.ElementalHEROBubbleman,
                CardId.ElementalHEROAbsoluteZero,
                CardId.ElementalHEROGreatTornado,
                CardId.ElementalHERONovaMaster,
                CardId.ElementalHEROGaia,
                CardId.ElementalHEROEscuridao,
                CardId.ElementalHEROTheShining);
            return true;
        }

        private bool Number39UtopiaActivate()
        {
            if (Duel.Player != 1 || Enemy.BattlingMonster == null)
                return false;

            return Bot.BattlingMonster == null ||
                Enemy.BattlingMonster.Attack >= Bot.BattlingMonster.GetDefensePower();
        }

        private bool GeminiSparkActivate()
        {
            ClientCard targetedNeosAlius = null;
            if (Duel.LastChainPlayer == 1)
            {
                targetedNeosAlius = Duel.LastChainTargets.FirstOrDefault(card =>
                    card.Controller == 0 &&
                    card.Location == CardLocation.MonsterZone &&
                    card.IsFaceup() &&
                    card.IsCode(CardId.ElementalHERONeosAlius));
            }

            ClientCard target = null;

            if (targetedNeosAlius != null)
            {
                ClientCard lastChainCard = Util.GetLastChainCard();
                if (lastChainCard != null &&
                    lastChainCard.Controller == 1 &&
                    (lastChainCard.Location == CardLocation.MonsterZone ||
                     lastChainCard.Location == CardLocation.SpellZone) &&
                    (lastChainCard.HasType(CardType.Continuous) ||
                     lastChainCard.HasType(CardType.Equip)) &&
                    !lastChainCard.IsShouldNotBeTarget() &&
                    !lastChainCard.IsShouldNotBeSpellTrapTarget())
                {
                    target = lastChainCard;
                }
                else
                {
                    IList<ClientCard> otherTargets = Enemy.GetMonsters()
                        .Concat(Enemy.GetSpells())
                        .Where(card =>
                            card != lastChainCard &&
                            !card.IsShouldNotBeTarget() &&
                            !card.IsShouldNotBeSpellTrapTarget())
                        .ToList();

                    target = Util.GetProblematicEnemyCard(0, true);
                    if (target == null || !otherTargets.Contains(target))
                    {
                        target = otherTargets
                            .OrderByDescending(card => card.IsFloodgate() || card.IsMonsterDangerous())
                            .ThenByDescending(card => card.IsFaceup())
                            .ThenByDescending(card => card.GetDefensePower())
                            .FirstOrDefault();
                    }
                }

                if (target == null)
                    return false;

                AI.SelectCard(targetedNeosAlius);
                AI.SelectNextCard(target);
                return true;
            }

            target = Util.GetProblematicEnemyCard(1900, true);
            if (target == null)
                return false;

            AI.SelectCard(CardId.ElementalHERONeosAlius);
            AI.SelectNextCard(target);
            return true;
        }

        private bool MiracleFusionActivate()
        {
            // 奇迹融合只能使用自己场上和墓地的素材，不能使用已经除外的怪兽。
            IList<ClientCard> materials = Bot.Graveyard
                .Concat(Bot.GetMonsters())
                .ToList();
            int fusionId;
            IList<ClientCard> fusionMaterials;
            if (!TryGetFusion(materials, false, out fusionId, out fusionMaterials))
                return false;

            AI.SelectCard(fusionId);
            AI.SelectMaterials(fusionMaterials);
            return true;
        }

        private bool SuperPolymerizationActivate()
        {
            IList<ClientCard> materials = Bot.GetMonsters()
                .Concat(Enemy.GetMonsters())
                .Where(card => card.IsFaceup())
                .ToList();
            int fusionId;
            IList<ClientCard> fusionMaterials;
            if (!TryGetFusion(materials, true, out fusionId, out fusionMaterials))
                return false;

            AI.SelectCard(
                CardId.PotOfDuality,
                CardId.EEmergencyCall,
                CardId.ReinforcementOfTheArmy,
                CardId.ElementalHEROBubbleman,
                CardId.HeroBlast,
                CardId.BottomlessTrapHole,
                CardId.DimensionalPrison);
            AI.SelectNextCard(fusionId);
            AI.SelectMaterials(fusionMaterials);
            return true;
        }

        private bool GemKnightPearlSummon()
        {
            int bestBotPower = Util.GetBestPower(Bot);
            bool shouldSummon = Enemy.GetMonsters().Any(card =>
                card.IsFaceup() &&
                card.GetDefensePower() >= bestBotPower &&
                card.GetDefensePower() < 2600);
            if (shouldSummon)
                SelectRank4Materials();
            return shouldSummon;
        }

        private bool EvigishkiMerrowgeistSummon()
        {
            bool shouldSummon = Duel.Phase == DuelPhase.Main2 &&
                Bot.GetMonsters().Count(card =>
                    card.IsFaceup() &&
                    card.Level == 4 &&
                    !card.IsCode(CardId.ThunderKingRaiOh)) >= 2 &&
                Enemy.GetMonsters().Any(card =>
                    card.IsFaceup() &&
                    card.IsMonsterDangerous() &&
                    card.GetDefensePower() < 2100);
            if (shouldSummon)
                SelectRank4Materials();
            return shouldSummon;
        }

        private bool Number39UtopiaSummon()
        {
            bool shouldSummon = Util.IsTurn1OrMain2() &&
                Bot.GetMonsters().Count(card =>
                    card.IsFaceup() &&
                    card.Level == 4 &&
                    !card.IsCode(CardId.ThunderKingRaiOh)) >= 2 &&
                (Enemy.GetMonsterCount() == 0 || Util.GetBestPower(Enemy) <= 2500);
            if (shouldSummon)
                SelectRank4Materials();
            return shouldSummon;
        }

        private void SelectRank4Materials()
        {
            AI.SelectMaterials(new[]
            {
                CardId.ElementalHEROBubbleman,
                CardId.ElementalHEROStratos,
                CardId.ElementalHERONeosAlius,
                CardId.Honest,
                CardId.ThunderKingRaiOh
            });
        }

        private bool MonsterRebornActivate()
        {
            List<ClientCard> targets = Bot.Graveyard
                .Concat(Enemy.Graveyard)
                .Where(card => card.IsMonster() && card.IsCanRevive())
                .OrderByDescending(card =>
                    card.Attack + (card.IsCode(CardId.ElementalHEROStratos) ? 1500 : 0))
                .ToList();
            if (targets.Count == 0)
                return false;

            AI.SelectCard(targets);
            return true;
        }

        private bool PotOfDualityActivate()
        {
            List<int> priority = new List<int>();
            if (Enemy.SpellZone.GetFloodgate() != null)
                priority.Add(CardId.MysticalSpaceTyphoon);
            if (Util.IsOneEnemyBetter())
            {
                priority.Add(CardId.DarkHole);
                priority.Add(CardId.BookOfMoon);
                priority.Add(CardId.DimensionalPrison);
            }
            priority.AddRange(GetHeroSearchPriority());
            priority.AddRange(new[]
            {
                CardId.EEmergencyCall,
                CardId.ReinforcementOfTheArmy,
                CardId.ThunderKingRaiOh,
                CardId.ElementalHERONeosAlius,
                CardId.Honest,
                CardId.GeminiSpark,
                CardId.MiracleFusion,
                CardId.ForbiddenLance,
                CardId.SolemnJudgment,
                CardId.SolemnWarning,
                CardId.BottomlessTrapHole,
                CardId.HeroBlast,
                CardId.MirrorForce,
                CardId.DimensionalPrison,
                CardId.TorrentialTribute,
                CardId.MysticalSpaceTyphoon,
                CardId.BookOfMoon,
                CardId.DarkHole,
                CardId.MonsterReborn,
                CardId.SuperPolymerization,
                CardId.ElementalHEROBubbleman,
                CardId.GorzTheEmissaryOfDarkness,
                CardId.PotOfDuality
            });
            AI.SelectCard(priority);
            return true;
        }

        private bool ForbiddenLanceActivate()
        {
            ClientCard lastChainCard = Util.GetLastChainCard();
            if (lastChainCard != null && lastChainCard.Controller == 1 &&
                (lastChainCard.IsSpell() || lastChainCard.IsTrap()))
            {
                ClientCard protectedMonster = Duel.ChainTargets.FirstOrDefault(card =>
                    card.Controller == 0 &&
                    card.Location == CardLocation.MonsterZone &&
                    card.IsFaceup());
                if (protectedMonster == null &&
                    lastChainCard.IsCode(
                        CardId.DarkHole,
                        CardId.BottomlessTrapHole,
                        CardId.MirrorForce,
                        CardId.TorrentialTribute))
                {
                    protectedMonster = Bot.GetMonsters()
                        .Where(card => card.IsFaceup())
                        .OrderByDescending(card => card.Attack)
                        .FirstOrDefault();
                }
                if (protectedMonster != null)
                {
                    AI.SelectCard(protectedMonster);
                    return true;
                }
            }

            if (Duel.Phase == DuelPhase.BattleStep || Duel.Phase == DuelPhase.Damage)
            {
                if (Bot.BattlingMonster != null &&
                    Enemy.BattlingMonster != null &&
                    Enemy.BattlingMonster.IsAttack() &&
                    Enemy.BattlingMonster.Attack >= Bot.BattlingMonster.GetDefensePower() &&
                    Enemy.BattlingMonster.Attack - 800 < Bot.BattlingMonster.GetDefensePower() &&
                    !Enemy.BattlingMonster.IsShouldNotBeTarget() &&
                    !Enemy.BattlingMonster.IsShouldNotBeSpellTrapTarget())
                {
                    AI.SelectCard(Enemy.BattlingMonster);
                    return true;
                }
            }
            return false;
        }

        private bool HeroBlastActivate()
        {
            ClientCard target = Enemy.GetMonsters()
                .Where(card => card.IsFaceup() && card.Attack <= 1900)
                .OrderByDescending(card => card.IsMonsterDangerous())
                .ThenByDescending(card => card.Attack)
                .FirstOrDefault();
            if (target == null && Duel.Player == 1 && Duel.Phase != DuelPhase.End)
                return false;

            AI.SelectCard(CardId.ElementalHERONeosAlius);
            if (target != null)
                AI.SelectNextCard(target);
            return true;
        }

        private bool MirrorForceActivate()
        {
            if (!DefaultTrap())
                return false;

            int attackerCount = Enemy.GetMonsters().Count(card => card.IsAttack());
            return attackerCount >= 2 || !Bot.HasInSpellZone(CardId.DimensionalPrison);
        }

    }
}
