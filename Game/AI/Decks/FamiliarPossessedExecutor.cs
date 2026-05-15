using YGOSharp.OCGWrapper;
using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using System.Linq;

namespace WindBot.Game.AI.Decks
{
    [Deck("FamiliarPossessed", "AI_FamiliarPossessed")]
    public class FamiliarPossessedExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int MetalSnake = 71197066;
            public const int InspectBoarder = 15397015;
            public const int AshBlossomAndJoyousSpring = 14558127;
            public const int GrenMajuDaEizo = 36584821;
            public const int MaxxC = 23434538;

            public const int Aussa = 31887906;
            public const int Eria = 68881650;
            public const int Wynn = 31764354;
            public const int Hiita = 4376659;
            public const int Lyna = 40542825;
            public const int Awakening = 62256492;
            public const int Unpossessed = 25704359;

            public const int NaturalExterio = 99916754;
            public const int NaturalBeast = 33198837;
            public const int SwordsmanLV7 = 37267041;
            public const int RoyalDecreel = 51452091;

            public const int HarpieFeatherDuster = 18144506;
            public const int PotOfDesires = 35261759;
            public const int PotofExtravagance = 49238328;
            public const int Scapegoat = 73915051;
            public const int MacroCosmos = 30241314;
            public const int Crackdown = 36975314;
            public const int ImperialOrder = 61740673;
            public const int SolemnWarning = 84749824;
            public const int SolemStrike = 40605147;
            public const int SolemnJudgment = 41420027;
            public const int SkillDrain = 82732705;
            public const int Mistake = 59305593;

            public const int BorreloadDragon = 31833038;
            public const int BirrelswordDragon = 85289965;
            public const int KnightmareGryphon = 65330383;
            public const int KnightmareUnicorn = 38342335;
            public const int KnightmarePhoenix = 2857636;
            public const int KnightmareCerberus = 75452921;
            public const int LinkSpider = 98978921;
            public const int Linkuriboh = 41999284;
            public const int GagagaCowboy = 12014404;

            public const int AussaP = 97661969;
            public const int EriaP = 73309655;
            public const int WynnP = 30674956;
            public const int HiitaP = 48815792;
            public const int LynaP = 9839945;

        }

        public FamiliarPossessedExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // do first
            AddExecutor(ExecutorType.Activate, CardId.PotofExtravagance, PotofExtravaganceActivate);
            // burn if enemy's LP is below 800
            AddExecutor(ExecutorType.SpSummon, CardId.GagagaCowboy, GagagaCowboySummon);
            AddExecutor(ExecutorType.Activate, CardId.GagagaCowboy);
            //Sticker
            AddExecutor(ExecutorType.Activate, CardId.MacroCosmos, MacroCosmoseff);
            //counter
            AddExecutor(ExecutorType.Activate, CardId.AshBlossomAndJoyousSpring, DefaultAshBlossomAndJoyousSpring);
            AddExecutor(ExecutorType.Activate, CardId.MaxxC, DefaultMaxxC);
            AddExecutor(ExecutorType.Activate, CardId.SolemnWarning, DefaultSolemnWarning);
            AddExecutor(ExecutorType.Activate, CardId.SolemStrike, DefaultSolemnStrike);
            AddExecutor(ExecutorType.Activate, CardId.ImperialOrder, ImperialOrderfirst);
            AddExecutor(ExecutorType.Activate, CardId.ImperialOrder, ImperialOrdereff);
            AddExecutor(ExecutorType.Activate, CardId.SolemnJudgment, DefaultSolemnJudgment);
            AddExecutor(ExecutorType.Activate, CardId.SkillDrain, SkillDrainEffect);
            AddExecutor(ExecutorType.Activate, CardId.Mistake, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, CardId.Awakening);
            AddExecutor(ExecutorType.Activate, CardId.Unpossessed, UnpossessedEffect);
            //first do
            AddExecutor(ExecutorType.Activate, CardId.HarpieFeatherDuster, DefaultHarpiesFeatherDusterFirst);
            AddExecutor(ExecutorType.Activate, CardId.PotOfDesires, PotOfDesireseff);
            //sp
            AddExecutor(ExecutorType.Activate, CardId.Linkuriboh, Linkuriboheff);
            AddExecutor(ExecutorType.SpSummon, CardId.Linkuriboh, Linkuribohsp);
            AddExecutor(ExecutorType.SpSummon, CardId.KnightmareCerberus, Knightmaresp);
            AddExecutor(ExecutorType.SpSummon, CardId.KnightmarePhoenix, Knightmaresp);
            AddExecutor(ExecutorType.SpSummon, CardId.AussaP, AussaPsp);
            AddExecutor(ExecutorType.Activate, CardId.AussaP, AussaPeff);
            AddExecutor(ExecutorType.SpSummon, CardId.EriaP, EriaPsp);
            AddExecutor(ExecutorType.Activate, CardId.EriaP, EriaPeff);
            AddExecutor(ExecutorType.SpSummon, CardId.WynnP, WynnPsp);
            AddExecutor(ExecutorType.Activate, CardId.WynnP, WynnPeff);
            AddExecutor(ExecutorType.SpSummon, CardId.HiitaP, HiitaPsp);
            AddExecutor(ExecutorType.Activate, CardId.HiitaP, HiitaPeff);
            AddExecutor(ExecutorType.SpSummon, CardId.LynaP, LynaPsp);
            AddExecutor(ExecutorType.Activate, CardId.LynaP, LynaPeff);

            AddExecutor(ExecutorType.SpSummon, CardId.Linkuriboh, Linkuribohsp);
            AddExecutor(ExecutorType.SpSummon, CardId.LinkSpider);
            AddExecutor(ExecutorType.SpSummon, CardId.BorreloadDragon, BorreloadDragonsp);
            AddExecutor(ExecutorType.Activate, CardId.BorreloadDragon, BorreloadDragoneff);
            AddExecutor(ExecutorType.SpSummon, CardId.BirrelswordDragon, BirrelswordDragonsp);
            AddExecutor(ExecutorType.Activate, CardId.BirrelswordDragon, BirrelswordDragoneff);
            // normal summon
            AddExecutor(ExecutorType.Summon, CardId.InspectBoarder, InspectBoardersummon);
            AddExecutor(ExecutorType.Summon, CardId.GrenMajuDaEizo, GrenMajuDaEizosummon);
            AddExecutor(ExecutorType.SpSummon, CardId.BorreloadDragon, BorreloadDragonspsecond);

            AddExecutor(ExecutorType.Summon, CardId.Aussa, FamiliarPossessedsummon);
            AddExecutor(ExecutorType.Summon, CardId.Eria, FamiliarPossessedsummon);
            AddExecutor(ExecutorType.Summon, CardId.Wynn, FamiliarPossessedsummon);
            AddExecutor(ExecutorType.Summon, CardId.Hiita, FamiliarPossessedsummon);
            AddExecutor(ExecutorType.Summon, CardId.Lyna, FamiliarPossessedsummon);

            AddExecutor(ExecutorType.Activate, CardId.MetalSnake, MetalSnakesp);
            AddExecutor(ExecutorType.Activate, CardId.MetalSnake, MetalSnakeeff);
            //spell
            AddExecutor(ExecutorType.Activate, CardId.Crackdown, Crackdowneff);
            AddExecutor(ExecutorType.Activate, CardId.Scapegoat, DefaultScapegoat);
            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
            //set
            AddExecutor(ExecutorType.SpellSet, SpellSet);
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

        private bool MacroCosmoseff()
        {

            return (Duel.LastChainPlayer == 1 || Duel.LastSummonPlayer == 1 || Duel.Player == 0) && UniqueFaceupSpell();
        }

        private bool ImperialOrderfirst()
        {
            if (Util.GetLastChainCard() != null && Util.GetLastChainCard().IsCode(CardId.PotOfDesires))
                return false;
            return DefaultOnBecomeTarget() && Util.GetLastChainCard().HasType(CardType.Spell);
        }

        private bool ImperialOrdereff()
        {
            if (Util.GetLastChainCard() != null && Util.GetLastChainCard().IsCode(CardId.PotOfDesires))
                return false;
            if (Duel.LastChainPlayer == 1)
            {
                foreach (ClientCard check in Enemy.GetSpells())
                {
                    if (Util.GetLastChainCard() == check)
                        return true;
                }
            }
            return false;
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

        private bool Crackdowneff()
        {
            if (Util.GetOneEnemyBetterThanMyBest(true, true) != null && Bot.UnderAttack)
                AI.SelectCard(Util.GetOneEnemyBetterThanMyBest(true, true));
            return Util.GetOneEnemyBetterThanMyBest(true, true) != null && Bot.UnderAttack;
        }

        private bool SkillDrainEffect()
        {
            return (Bot.LifePoints > 1000) && DefaultUniqueTrap();
        }

        private bool UnpossessedEffect()
        {
            AI.SelectCard(new List<int>() {
                CardId.Lyna,
                CardId.Hiita,
                CardId.Wynn,
                CardId.Eria,
                CardId.Aussa
            });
            return true;
        }

        private bool InspectBoardersummon()
        {
            if (Bot.MonsterZone[0] == null)
                AI.SelectPlace(Zones.z0);
            else
                AI.SelectPlace(Zones.z4);
            return true;
        }

        private bool GrenMajuDaEizosummon()
        {
            if (Duel.Turn == 1) return false;
            if (Bot.HasInSpellZone(CardId.SkillDrain) || Enemy.HasInSpellZone(CardId.SkillDrain)) return false;
            if (Bot.MonsterZone[0] == null)
                AI.SelectPlace(Zones.z0);
            else
                AI.SelectPlace(Zones.z4);
            return Bot.Banished.Count >= 6;
        }

        private bool FamiliarPossessedsummon()
        {
            if (Bot.MonsterZone[0] == null)
                AI.SelectPlace(Zones.z0);
            else
                AI.SelectPlace(Zones.z4);
            return true;
        }

        private bool BorreloadDragonsp()
        {
            if (!(Bot.HasInMonstersZone(new[] { CardId.KnightmareCerberus, CardId.KnightmarePhoenix, CardId.LynaP, CardId.HiitaP, CardId.WynnP, CardId.EriaP, CardId.AussaP }))) return false;
            IList<ClientCard> material_list = new List<ClientCard>();
            foreach (ClientCard monster in Bot.GetMonsters())
            {
                if (monster.IsCode(CardId.LynaP, CardId.HiitaP, CardId.WynnP, CardId.EriaP, CardId.AussaP, CardId.KnightmareCerberus, CardId.KnightmarePhoenix, CardId.LinkSpider, CardId.Linkuriboh))
                    material_list.Add(monster);
                if (material_list.Count == 3) break;
            }
            if (material_list.Count >= 3)
            {
                AI.SelectMaterials(material_list);
                return true;
            }
            return false;
        }
        private bool BorreloadDragonspsecond()
        {
            if (!(Bot.HasInMonstersZone(new[] { CardId.KnightmareCerberus, CardId.KnightmarePhoenix, CardId.LynaP, CardId.HiitaP, CardId.WynnP, CardId.EriaP, CardId.AussaP }))) return false;
            IList<ClientCard> material_list = new List<ClientCard>();
            foreach (ClientCard monster in Bot.GetMonsters())
            {
                if (monster.IsCode(CardId.LynaP, CardId.HiitaP, CardId.WynnP, CardId.EriaP, CardId.AussaP, CardId.KnightmareCerberus, CardId.KnightmarePhoenix, CardId.LinkSpider, CardId.Linkuriboh))
                    material_list.Add(monster);
                if (material_list.Count == 3) break;
            }
            if (material_list.Count >= 3)
            {
                AI.SelectMaterials(material_list);
                return true;
            }
            return false;
        }
        public bool BorreloadDragoneff()
        {
            if (ActivateDescription == -1 && (Duel.Phase == DuelPhase.BattleStart || Duel.Phase == DuelPhase.End))
            {
                ClientCard enemy_monster = Enemy.BattlingMonster;
                if (enemy_monster != null && enemy_monster.HasPosition(CardPosition.Attack))
                {
                    return (Card.Attack - enemy_monster.Attack < Enemy.LifePoints);
                }
                return true;
            };
            ClientCard BestEnemy = Util.GetBestEnemyMonster(true);
            ClientCard WorstBot = Bot.GetMonsters().GetLowestAttackMonster();
            if (BestEnemy == null || BestEnemy.HasPosition(CardPosition.FaceDown)) return false;
            if (WorstBot == null || WorstBot.HasPosition(CardPosition.FaceDown)) return false;
            if (BestEnemy.Attack >= WorstBot.RealPower)
            {
                AI.SelectCard(BestEnemy);
                return true;
            }
            return false;
        }

        private bool BirrelswordDragonsp()
        {
            IList<ClientCard> material_list = new List<ClientCard>();
            foreach (ClientCard m in Bot.GetMonsters())
            {
                if (m.IsCode(CardId.KnightmareCerberus, CardId.KnightmarePhoenix, CardId.LynaP, CardId.HiitaP, CardId.WynnP, CardId.EriaP, CardId.AussaP))
                {
                    material_list.Add(m);
                    break;
                }
            }
            foreach (ClientCard m in Bot.GetMonsters())
            {
                if (m.IsCode(CardId.Linkuriboh) || m.Level == 1)
                {
                    material_list.Add(m);
                    if (material_list.Count == 3)
                        break;
                }
            }
            if (material_list.Count == 3)
            {
                AI.SelectMaterials(material_list);
                return true;
            }
            return false;
        }

        private bool BirrelswordDragoneff()
        {
            if (ActivateDescription == Util.GetStringId(CardId.BirrelswordDragon, 0))
            {
                if (Util.IsChainTarget(Card) && Util.GetBestEnemyMonster(true, true) != null)
                {
                    AI.SelectCard(Util.GetBestEnemyMonster(true, true));
                    return true;
                }
                if (Duel.Player == 1 && Bot.BattlingMonster == Card)
                {
                    AI.SelectCard(Enemy.BattlingMonster);
                    return true;
                }
                if (Duel.Player == 1 && Bot.BattlingMonster != null &&
                    (Enemy.BattlingMonster.Attack - Bot.BattlingMonster.Attack) >= Bot.LifePoints)
                {
                    AI.SelectCard(Enemy.BattlingMonster);
                    return true;
                }
                if (Duel.Player == 0 && Duel.Phase == DuelPhase.BattleStart)
                {
                    foreach (ClientCard check in Enemy.GetMonsters())
                    {
                        if (check.IsAttack())
                        {
                            AI.SelectCard(check);
                            return true;
                        }
                    }
                }
                return false;
            }
            return true;
        }

        private bool MetalSnakesp()
        {
            if (ActivateDescription == Util.GetStringId(CardId.MetalSnake, 0) && !Bot.HasInMonstersZone(CardId.MetalSnake))
            {
                if (Duel.Player == 1 && Duel.Phase >= DuelPhase.BattleStart)
                    return Bot.Deck.Count >= 12;
                if (Duel.Player == 0 && Duel.Phase >= DuelPhase.Main1)
                    return Bot.Deck.Count >= 12;
            }
            return false;
        }

        private bool MetalSnakeeff()
        {
            ClientCard target = Util.GetOneEnemyBetterThanMyBest(true, true);
            if (ActivateDescription == Util.GetStringId(CardId.MetalSnake, 1) && target != null)
            {
                AI.SelectCard(new[]
                {
                CardId.LynaP,
                CardId.HiitaP,
                CardId.WynnP,
                CardId.EriaP,
                CardId.KnightmareGryphon
                });
                AI.SelectNextCard(target);
                return true;
            }
            return false;

        }

        private bool AussaPsp()
        {
            IList<ClientCard> material_list = new List<ClientCard>();
            foreach (ClientCard monster in Bot.GetMonsters())
            {
                if (monster.HasAttribute(CardAttribute.Earth) && !monster.IsCode(CardId.KnightmareCerberus, CardId.InspectBoarder, CardId.GrenMajuDaEizo, CardId.KnightmarePhoenix, CardId.LynaP, CardId.HiitaP, CardId.WynnP, CardId.EriaP, CardId.AussaP, CardId.KnightmareUnicorn, CardId.KnightmareGryphon, CardId.BorreloadDragon, CardId.BirrelswordDragon))
                    material_list.Add(monster);
                if (material_list.Count == 2) break;
            }
            if (material_list.Count < 2) return false;
            if (Bot.HasInMonstersZone(CardId.AussaP)) return false;
            AI.SelectMaterials(material_list);
            if (Bot.MonsterZone[0] == null && Bot.MonsterZone[2] == null && Bot.MonsterZone[5] == null)
                AI.SelectPlace(Zones.z5);
            else
                AI.SelectPlace(Zones.z6);
            return true;
        }

        private bool AussaPeff()
        {
            AI.SelectCard(CardId.MaxxC, CardId.Aussa);
            return true;
        }

        private bool EriaPsp()
        {
            IList<ClientCard> material_list = new List<ClientCard>();
            foreach (ClientCard monster in Bot.GetMonsters())
            {
                if (monster.HasAttribute(CardAttribute.Water) && !monster.IsCode(CardId.KnightmareCerberus, CardId.InspectBoarder, CardId.GrenMajuDaEizo, CardId.KnightmarePhoenix, CardId.LynaP, CardId.HiitaP, CardId.WynnP, CardId.EriaP, CardId.AussaP, CardId.KnightmareUnicorn, CardId.KnightmareGryphon, CardId.BorreloadDragon, CardId.BirrelswordDragon))
                    material_list.Add(monster);
                if (material_list.Count == 2) break;
            }
            if (material_list.Count < 2) return false;
            if (Bot.HasInMonstersZone(CardId.EriaP)) return false;
            AI.SelectMaterials(material_list);
            if (Bot.MonsterZone[0] == null && Bot.MonsterZone[2] == null && Bot.MonsterZone[5] == null)
                AI.SelectPlace(Zones.z5);
            else
                AI.SelectPlace(Zones.z6);
            return true;
        }

        private bool EriaPeff()
        {
            AI.SelectCard(CardId.Eria);
            return true;
        }

        private bool WynnPsp()
        {
            IList<ClientCard> material_list = new List<ClientCard>();
            foreach (ClientCard monster in Bot.GetMonsters())
            {
                if (monster.HasAttribute(CardAttribute.Wind) && !monster.IsCode(CardId.KnightmareCerberus, CardId.InspectBoarder, CardId.GrenMajuDaEizo, CardId.KnightmarePhoenix, CardId.LynaP, CardId.HiitaP, CardId.WynnP, CardId.EriaP, CardId.AussaP, CardId.KnightmareUnicorn, CardId.KnightmareGryphon, CardId.BorreloadDragon, CardId.BirrelswordDragon))
                    material_list.Add(monster);
                if (material_list.Count == 2) break;
            }
            if (material_list.Count < 2) return false;
            if (Bot.HasInMonstersZone(CardId.WynnP)) return false;
            AI.SelectMaterials(material_list);
            if (Bot.MonsterZone[0] == null && Bot.MonsterZone[2] == null && Bot.MonsterZone[5] == null)
                AI.SelectPlace(Zones.z5);
            else
                AI.SelectPlace(Zones.z6);
            return true;
        }

        private bool WynnPeff()
        {
            AI.SelectCard(CardId.Wynn);
            return true;
        }

        private bool HiitaPsp()
        {
            IList<ClientCard> material_list = new List<ClientCard>();
            foreach (ClientCard monster in Bot.GetMonsters())
            {
                if (monster.HasAttribute(CardAttribute.Fire) && !monster.IsCode(CardId.KnightmareCerberus, CardId.InspectBoarder, CardId.GrenMajuDaEizo, CardId.KnightmarePhoenix, CardId.LynaP, CardId.HiitaP, CardId.WynnP, CardId.EriaP, CardId.AussaP, CardId.KnightmareUnicorn, CardId.KnightmareGryphon, CardId.BorreloadDragon, CardId.BirrelswordDragon))
                    material_list.Add(monster);
                if (material_list.Count == 2) break;
            }
            if (material_list.Count < 2) return false;
            if (Bot.HasInMonstersZone(CardId.HiitaP)) return false;
            AI.SelectMaterials(material_list);
            if (Bot.MonsterZone[0] == null && Bot.MonsterZone[2] == null && Bot.MonsterZone[5] == null)
                AI.SelectPlace(Zones.z5);
            else
                AI.SelectPlace(Zones.z6);
            return true;
        }

        private bool HiitaPeff()
        {
            AI.SelectCard(CardId.Hiita);
            return true;
        }

        private bool LynaPsp()
        {
            IList<ClientCard> material_list = new List<ClientCard>();
            foreach (ClientCard monster in Bot.GetMonsters())
            {
                if (monster.HasAttribute(CardAttribute.Light) && !monster.IsCode(CardId.KnightmareCerberus, CardId.InspectBoarder, CardId.GrenMajuDaEizo, CardId.KnightmarePhoenix, CardId.LynaP, CardId.HiitaP, CardId.WynnP, CardId.EriaP, CardId.AussaP, CardId.KnightmareUnicorn, CardId.KnightmareGryphon, CardId.BorreloadDragon, CardId.BirrelswordDragon))
                    material_list.Add(monster);
                if (material_list.Count == 2) break;
            }
            if (material_list.Count < 2) return false;
            if (Bot.HasInMonstersZone(CardId.LynaP)) return false;
            AI.SelectMaterials(material_list);
            if (Bot.MonsterZone[0] == null && Bot.MonsterZone[2] == null && Bot.MonsterZone[5] == null)
                AI.SelectPlace(Zones.z5);
            else
                AI.SelectPlace(Zones.z6);
            return true;
        }

        private bool LynaPeff()
        {
            AI.SelectCard(CardId.Lyna);
            return true;
        }

        private bool Linkuribohsp()
        {

            foreach (ClientCard c in Bot.GetMonsters())
            {
                if (c.Level == 1)
                {
                    AI.SelectMaterials(c);
                    return true;
                }
            }
            return false;
        }

        private bool Knightmaresp()
        {
            int[] firstMats = new[] {
              CardId.KnightmareCerberus,
              CardId.KnightmarePhoenix
            };
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(firstMats)) >= 1) return false;
            foreach (ClientCard c in Bot.GetMonsters())
            {
                if (c.Level == 1)
                {
                    AI.SelectMaterials(c);
                    return true;
                }
            }
            return false;
        }
        private bool Linkuriboheff()
        {
            if (Duel.LastChainPlayer == 0 && Util.GetLastChainCard().IsCode(CardId.Linkuriboh)) return false;
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
        private bool SpellSet()
        {
            if (Card.IsCode(CardId.MacroCosmos) && Bot.HasInSpellZone(CardId.MacroCosmos)) return false;
            if (Card.IsCode(CardId.Unpossessed) && Bot.HasInSpellZone(CardId.Unpossessed)) return false;
            if (Card.IsCode(CardId.Crackdown) && Bot.HasInSpellZone(CardId.Crackdown)) return false;
            if (Card.IsCode(CardId.SkillDrain) && Bot.HasInSpellZone(CardId.SkillDrain)) return false;
            if (Card.IsCode(CardId.Mistake) && Bot.HasInSpellZone(CardId.Mistake)) return false;
            if (Card.IsCode(CardId.Scapegoat))
                return true;
            if (Card.HasType(CardType.Trap))
                return Bot.GetSpellCountWithoutField() < 4;
            return false;
        }
        public override ClientCard OnSelectAttacker(IList<ClientCard> attackers, IList<ClientCard> defenders)
        {
            for (int i = 0; i < attackers.Count; ++i)
            {
                ClientCard attacker = attackers[i];
                if (attacker.IsCode(CardId.BirrelswordDragon, CardId.BorreloadDragon)) return attacker;
            }
            return null;
        }
        public override bool OnSelectHand()
        {
            return true;
        }
    }
}