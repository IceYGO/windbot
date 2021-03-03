using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("FamiliarPossessed", "AI_FamiliarPossessed")]
    public class FamiliarPossessedExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int MetalSnake = 71197066;
            public const int InspectBoarder = 15397015;
            public const int AshBlossomAndJoyousSpring =14558127;
            public const int GrenMajuDaEizo = 36584821;
            public const int MaxxC = 23434538;
			
			public const int Aussa = 31887906;
			public const int Eria = 68881650;
			public const int Wynn = 31764354;
			public const int Hiita = 4376659;
			public const int Lyna = 40542825;
			public const int Awakening = 62256492;
			public const int Unpossessed = 25704359;

            public const int HarpieFeatherDuster = 18144506;
            public const int PotOfDesires = 35261759;
			public const int PotofExtravagance = 49238328;
            public const int Scapegoat = 73915051;
            public const int MacroCosmos = 30241314;
            public const int Crackdown = 36975314;
            public const int ImperialOrder = 61740673;
            public const int SolemnWarning = 84749824;
            public const int SolemStrike= 40605147;
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
			AddExecutor(ExecutorType.Activate, CardId.Awakening, DefaultUniqueSpell);
			AddExecutor(ExecutorType.Activate, CardId.Unpossessed, UnpossessedEffect);
            //first do
            AddExecutor(ExecutorType.Activate, CardId.HarpieFeatherDuster, DefaultHarpiesFeatherDusterFirst);
            AddExecutor(ExecutorType.Activate, CardId.PotOfDesires, PotOfDesireseff);
            //sp
            AddExecutor(ExecutorType.Activate, CardId.Linkuriboh, Linkuriboheff);
            AddExecutor(ExecutorType.SpSummon, CardId.Linkuriboh, Linkuribohsp);
            AddExecutor(ExecutorType.SpSummon, CardId.KnightmareCerberus,Knightmaresp);
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
            AddExecutor(ExecutorType.Activate, CardId.LynaP, LynaP);
            
            AddExecutor(ExecutorType.SpSummon, CardId.Linkuriboh, Linkuribohsp);
            AddExecutor(ExecutorType.SpSummon, CardId.LinkSpider);
            AddExecutor(ExecutorType.SpSummon, CardId.BorreloadDragon, BorreloadDragonsp);
            AddExecutor(ExecutorType.Activate, CardId.BorreloadDragon, BorreloadDragoneff);            
            // normal summon
            AddExecutor(ExecutorType.Summon, CardId.InspectBoarder, InspectBoardersummon);
            AddExecutor(ExecutorType.Summon, CardId.GrenMajuDaEizo, GrenMajuDaEizosummon);
            AddExecutor(ExecutorType.Summon, CardId.ThunderKingRaiOh, ThunderKingRaiOhsummon);
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
            AddExecutor(ExecutorType.Repos, MonsterRepos);
            //set
            AddExecutor(ExecutorType.SpellSet, SpellSet);
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
                foreach(ClientCard check in Enemy.GetSpells())
                {
                    if (Util.GetLastChainCard() == check)
                        return true;
                }
            }
            return false;
        }

        private bool PotOfDesireseff()
        {
            if (CardOfDemiseeff_used) return false;          
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
			if (!(Bot.HasInMonstersZone(CardId.InspectBoarder) || !(Bot.HasInMonstersZone(CardId.GrenMajuDaEizo))))
            return (Bot.LifePoints > 1000) && DefaultUniqueTrap();
        }
		
		private bool UnpossessedEffectEffect()
        {
			AI.SelectCard(CardId.Lyna, CardId.Hiita, CardId.Wynn, CardId.Eria, CardId.Aussa);
            return ture;
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
            if (!(Bot.HasInMonstersZone(CardId.LynaP, CardId.HiitaP, CardId.WynnP, CardId.EriaP, CardId.AussaP) || Bot.HasInMonstersZone(new[] { CardId.KnightmareCerberus, CardId.KnightmarePhoenix }))) return false;
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
            if (!(Bot.HasInMonstersZone(CardId.LynaP, CardId.HiitaP, CardId.WynnP, CardId.EriaP, CardId.AussaP) || Bot.HasInMonstersZone(new[] { CardId.KnightmareCerberus,CardId.KnightmarePhoenix }))) return false;
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
            if (ActivateDescription == -1 && (Duel.Phase==DuelPhase.BattleStart||Duel.Phase==DuelPhase.End))
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

        private bool MetalSnakesp()
        {
            if (ActivateDescription == Util.GetStringId(CardId.MetalSnake, 0) && !Bot.HasInMonstersZone(CardId.MetalSnake))
            {
                if(Duel.Player == 1 && Duel.Phase >= DuelPhase.BattleStart )
                    return Bot.Deck.Count >= 12;
                if(Duel.Player == 0 && Duel.Phase >= DuelPhase.Main1)
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
                if (monster.HasAttribute(CardAttribute.Earth) && monster.Level==1 && !Card.IsCode(CardId.KnightmareCerberus, CardId.InspectBoarder, CardId.GrenMajuDaEizo, CardId.KnightmarePhoenix, CardId.LynaP, CardId.HiitaP, CardId.WynnP, CardId.EriaP, CardId.AussaP, CardId.KnightmareUnicorn, CardId.KnightmareGryphon, CardId.BorreloadDragon, CardId.BirrelswordDragon))
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
                if (monster.HasAttribute(CardAttribute.Water) && !Card.IsCode(CardId.KnightmareCerberus, CardId.InspectBoarder, CardId.GrenMajuDaEizo, CardId.KnightmarePhoenix, CardId.LynaP, CardId.HiitaP, CardId.WynnP, CardId.EriaP, CardId.AussaP, CardId.KnightmareUnicorn, CardId.KnightmareGryphon, CardId.BorreloadDragon, CardId.BirrelswordDragon))
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
                if (monster.HasAttribute(CardAttribute.Wind) && !Card.IsCode(CardId.KnightmareCerberus, CardId.InspectBoarder, CardId.GrenMajuDaEizo, CardId.KnightmarePhoenix, CardId.LynaP, CardId.HiitaP, CardId.WynnP, CardId.EriaP, CardId.AussaP, CardId.KnightmareUnicorn, CardId.KnightmareGryphon, CardId.BorreloadDragon, CardId.BirrelswordDragon))
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
                if (monster.HasAttribute(CardAttribute.Fire) && !Card.IsCode(CardId.KnightmareCerberus, CardId.InspectBoarder, CardId.GrenMajuDaEizo, CardId.KnightmarePhoenix, CardId.LynaP, CardId.HiitaP, CardId.WynnP, CardId.EriaP, CardId.AussaP, CardId.KnightmareUnicorn, CardId.KnightmareGryphon, CardId.BorreloadDragon, CardId.BirrelswordDragon))
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

        private bool HittaPeff()
        {
            AI.SelectCard(CardId.Hiita);
            return true;
        }
		
		private bool LynaPsp()
        {                       
            IList<ClientCard> material_list = new List<ClientCard>();
            foreach (ClientCard monster in Bot.GetMonsters())
            {
                if (monster.HasAttribute(CardAttribute.Light) && !Card.IsCode(CardId.KnightmareCerberus, CardId.InspectBoarder, CardId.GrenMajuDaEizo, CardId.KnightmarePhoenix, CardId.LynaP, CardId.HiitaP, CardId.WynnP, CardId.EriaP, CardId.AussaP, CardId.KnightmareUnicorn, CardId.KnightmareGryphon, CardId.BorreloadDragon, CardId.BirrelswordDragon))
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
                if (c.Level==1)
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
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(firstMats)) >= 1)return false;
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
        private bool MonsterRepos()
        {
            if (Card.IsAttack()) return false;
            return DefaultMonsterRepos();
        }
		
		private bool GagagaCowboySummon()
        {
            if (Enemy.LifePoints <= 800 || (Bot.GetMonsterCount()>=4 && Enemy.LifePoints <= 1600))
            {
                AI.SelectPosition(CardPosition.FaceUpDefence);
                return true;
            }
            return false;
        }

        private bool SpellSet()
        {
            int count = 0;
            foreach(ClientCard check in Bot.Hand)
            {
                if (check.IsCode(CardId.CardOfDemise))
                    count++;
            }
            if (count == 2 && Bot.Hand.Count == 2 && Bot.GetSpellCountWithoutField() <= 2)
                return true;            
            if (Card.IsCode(CardId.MacroCosmos) && Bot.HasInSpellZone(CardId.MacroCosmos)) return false;
			if (Card.IsCode(CardId.Unpossessed) && Bot.HasInSpellZone(CardId.Unpossessed)) return false;
			if (Card.IsCode(CardId.Crackdown) && Bot.HasInSpellZone(CardId.Crackdown)) return false;
			if (Card.IsCode(CardId.SkillDrain) && Bot.HasInSpellZone(CardId.SkillDrain)) return false;
			if (Card.IsCode(CardId.Mistake) && Bot.HasInSpellZone(CardId.Mistake)) return false;
            if (Card.IsCode(CardId.Scapegoat))
                return true;
            if (Card.HasType(CardType.Trap))
                return Bot.GetSpellCountWithoutField() < 4;
        }
        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            if (Bot.HasInMonstersZone(CardId.InspectBoarder) && !attacker.IsDisabled())
            {
                attacker.RealPower = 9999;
                return true;
            }
            if (!Bot.HasInMonstersZone(CardId.InspectBoarder) && !attacker.IsDisabled())
            {
                attacker.RealPower = 9999;
                return true;
            }            
            return base.OnPreBattleBetween(attacker, defender);
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