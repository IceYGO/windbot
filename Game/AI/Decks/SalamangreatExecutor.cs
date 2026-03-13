using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YGOSharp.OCGWrapper.Enums;

namespace WindBot.Game.AI.Decks
{
    [Deck("Salamangreat", "AI_Salamangreat")]
    class SalamangreatExecutor : DefaultExecutor
    {
        bool foxyPopEnemySpell = false;
        bool wasGazelleSummonedThisTurn = false;
        bool wasFieldspellUsedThisTurn = false;
        bool wasWolfSummonedUsingItself = false;
        int sunlightPosition = 0;
        bool wasVeilynxSummonedThisTurn = false;
        bool falcoHitGY = false;
        List<int> CombosInHand;


        List<int> Impermanence_list = new List<int>();
        public class CardId
        {
            public const int JackJaguar = 56003780;
            public const int EffectVeiler = 97268402;
            public const int LadyDebug = 16188701;
            public const int Foxy = 94620082;
            public const int Gazelle = 26889158;
            public const int Fowl = 89662401;
            public const int Falco = 20618081;
            public const int Spinny = 52277807;
            public const int MaxxC = 23434538;
            public const int AshBlossom = 14558127;

            public const int FusionOfFire = 25800447;
            public const int Circle = 52155219;
            public const int HarpieFeatherDuster = 18144507;
            public const int FoolishBurial = 81439174;
            public const int Sanctuary = 1295111;
            public const int CalledByTheGrave = 24224830;
            public const int SalamangreatRage = 14934922;
            public const int SalamangreatRoar = 51339637;
            public const int Impermanence = 10045474;
            public const int SolemnJudgment = 41420027;
            public const int SolemnStrike = 40605147;

            public const int SalamangreatVioletChimera = 37261776;
            public const int ExcitionKnight = 46772449;
            public const int MirageStallio = 87327776;
            public const int SunlightWolf = 87871125;
            public const int Borrelload = 31833038;
            public const int HeatLeo = 41463181;
            public const int Veilynx = 14812471;
            public const int Charmer = 48815792;
            public const int KnightmarePheonix = 2857636;
            public const int Borrelsword = 85289965;

            public const int GO_SR = 59438930;
            public const int DarkHole = 53129443;
            public const int NaturalBeast = 33198837;
            public const int SwordsmanLV7 = 37267041;
            public const int RoyalDecreel = 51452091;
            public const int Anti_Spell = 58921041;
            public const int Hayate = 8491308;
            public const int Raye = 26077387;
            public const int Drones_Token = 52340445;
            public const int Iblee = 10158145;
            public const int ImperialOrder = 61740673;
            public const int TornadoDragon = 6983839;
        }

        List<int> Combo_cards = new List<int>()
        {
            CardId.Spinny,
            CardId.JackJaguar,
            CardId.Fowl,
            CardId.Foxy,
            CardId.Falco,
            CardId.Circle,
            CardId.Gazelle,
            CardId.FoolishBurial
        };

        List<int> normal_counter = new List<int>
        {
            53262004, 98338152, 32617464, 45041488, CardId.SolemnStrike,
            61257789, 23440231, 27354732, 12408276, 82419869, CardId.Impermanence,
            49680980, 18621798, 38814750, 17266660, 94689635,CardId.AshBlossom,
            74762582, 75286651, 4810828,  44665365, 21123811, _CardId.CrystalWingSynchroDragon,
            82044279, 82044280, 79606837, 10443957, 1621413,
            90809975, 8165596,  9753964,  53347303, 88307361, _CardId.GamecieltheSeaTurtleKaiju,
            5818294,  2948263,  6150044,  26268488, 51447164, _CardId.JizukirutheStarDestroyingKaiju,
            97268402
        };

        List<int> should_not_negate = new List<int>
        {
            81275020, 28985331
        };

        List<int> salamangreat_links = new List<int>
        {
            CardId.HeatLeo,
            CardId.SunlightWolf,
            CardId.Veilynx
        };

        List<int> JackJaguarTargets = new List<int>
        {
            CardId.SunlightWolf,
            CardId.MirageStallio,
            CardId.HeatLeo
        };


        List<int> salamangreat_combopieces = new List<int>
        {
            CardId.Gazelle,
            CardId.Spinny,
            CardId.JackJaguar,
            CardId.Foxy,
            CardId.Circle,
            CardId.Falco
        };

        List<int> WolfMaterials = new List<int>
        {
            CardId.Veilynx,
            CardId.JackJaguar,
            CardId.Falco,
            CardId.Foxy,
            CardId.MirageStallio,
            CardId.Gazelle
        };

        List<int> salamangreat_spellTrap = new List<int>
        {
            CardId.SalamangreatRoar,
            CardId.SalamangreatRage,
            CardId.Circle,
            CardId.Sanctuary
        };
        private bool falcoUsedReturnST;
        private bool wasStallioActivated;
        private bool wasWolfActivatedThisTurn;

        bool JackJaguarActivatedThisTurn = false;
        bool FoxyActivatedThisTurn = false;

        public SalamangreatExecutor(GameAI ai, Duel duel) : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, CardId.HarpieFeatherDuster);
            AddExecutor(ExecutorType.Activate, CardId.MaxxC, G_activate);
            AddExecutor(ExecutorType.Activate, CardId.CalledByTheGrave, Called_activate);
            AddExecutor(ExecutorType.Activate, CardId.AshBlossom, Hand_act_eff);
            AddExecutor(ExecutorType.Activate, CardId.EffectVeiler, DefaultBreakthroughSkill);
            AddExecutor(ExecutorType.Activate, CardId.Impermanence, Impermanence_activate);
            AddExecutor(ExecutorType.Activate, CardId.SalamangreatRoar, SolemnJudgment_activate);
            AddExecutor(ExecutorType.Activate, CardId.SolemnStrike, SolemnStrike_activate);
            AddExecutor(ExecutorType.Activate, CardId.SolemnJudgment, SolemnJudgment_activate);
            AddExecutor(ExecutorType.Activate, CardId.Sanctuary, Sanctuary_activate);

            AddExecutor(ExecutorType.Activate, CardId.Charmer);
            AddExecutor(ExecutorType.Activate, CardId.SunlightWolf, Wolf_activate);
            AddExecutor(ExecutorType.Activate, CardId.LadyDebug, Fadydebug_activate);
            AddExecutor(ExecutorType.Activate, CardId.Foxy, Foxy_activate);
            AddExecutor(ExecutorType.Activate, CardId.Falco, Falco_activate);
            AddExecutor(ExecutorType.Activate, CardId.Circle, Circle_activate);
            AddExecutor(ExecutorType.Activate, CardId.Borrelsword, Borrelsword_eff);
            AddExecutor(ExecutorType.Activate, CardId.Gazelle, Gazelle_activate);
            AddExecutor(ExecutorType.Activate, CardId.Spinny, Spinny_activate);
            AddExecutor(ExecutorType.Activate, CardId.MirageStallio, Stallio_activate);
            AddExecutor(ExecutorType.Activate, CardId.Veilynx);

            AddExecutor(ExecutorType.Activate, CardId.JackJaguar, JackJaguar_activate);
            AddExecutor(ExecutorType.Summon, CardId.LadyDebug);
            AddExecutor(ExecutorType.Summon, CardId.Foxy);
            AddExecutor(ExecutorType.Summon, CardId.Spinny);
            AddExecutor(ExecutorType.Summon, CardId.JackJaguar);
            AddExecutor(ExecutorType.Summon, CardId.Gazelle);
            AddExecutor(ExecutorType.Summon, CardId.Fowl);
            AddExecutor(ExecutorType.Activate, CardId.Spinny, Spinny_activate);
            AddExecutor(ExecutorType.Activate, CardId.HeatLeo, DefaultMysticalSpaceTyphoon);

            AddExecutor(ExecutorType.SpSummon, CardId.Borrelsword, Borrelsword_ss);
            AddExecutor(ExecutorType.SpSummon, CardId.Veilynx, Veilynx_summon);
            AddExecutor(ExecutorType.SpSummon, CardId.MirageStallio, Stallio_summon);
            AddExecutor(ExecutorType.Activate, CardId.MirageStallio, Stallio_activate);
            AddExecutor(ExecutorType.SpSummon, CardId.Charmer, Charmer_summon);
            AddExecutor(ExecutorType.SpSummon, CardId.SunlightWolf, SunlightWolf_summon);
            AddExecutor(ExecutorType.SpSummon, CardId.HeatLeo, HeatLeo_summon);

            AddExecutor(ExecutorType.SpSummon, CardId.TornadoDragon);
            AddExecutor(ExecutorType.Activate, CardId.TornadoDragon, DefaultMysticalSpaceTyphoon);

            AddExecutor(ExecutorType.Activate, CardId.SalamangreatRage, Rage_activate);
            AddExecutor(ExecutorType.Activate, CardId.Fowl, Fowl_activate);
            AddExecutor(ExecutorType.Activate, CardId.SunlightWolf, Wolf_activate);
            AddExecutor(ExecutorType.Activate, CardId.Gazelle, Gazelle_activate);
            AddExecutor(ExecutorType.Activate, CardId.FoolishBurial, FoolishBurial_activate);

            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
            AddExecutor(ExecutorType.SpellSet, SpellSet);

        }

        public int get_Wolf_linkzone()
        {
            ClientCard WolfInExtra = Bot.GetMonstersInExtraZone().Where(x => x.Id == CardId.SunlightWolf).ToList().FirstOrDefault(x => x.Id == CardId.SunlightWolf);
            if (WolfInExtra != null)
            {
                int zone = WolfInExtra.Position;
                if (zone == 5) return 1;
                if (zone == 6) return 3;
            }
            return -1;
        }

        private bool Charmer_summon()
        {
            if (Duel.Phase != DuelPhase.Main1) return false;
            if (Duel.Turn == 1) return false;
            if (Enemy.Graveyard.Where(x => x.Attribute == (int)CardAttribute.Fire).Count() > 0
                && (Bot.GetMonstersInExtraZone().Count == 0
                || Bot.GetMonstersInExtraZone().Where(x =>
                (x.Id == CardId.Veilynx
                || x.Id == CardId.MirageStallio)
                && x.Owner == 0).Count() == 1))
            {
                List<ClientCard> material_list = new List<ClientCard>();
                List<ClientCard> bot_monster = Bot.GetMonsters();
                bot_monster.Sort(CardContainer.CompareCardAttack);
                //bot_monster.Reverse();
                int link_count = 0;
                foreach (ClientCard card in bot_monster)
                {
                    if (card.IsFacedown()) continue;
                    if (!material_list.Contains(card) && card.LinkCount < 2)
                    {
                        material_list.Add(card);
                        link_count += (card.HasType(CardType.Link)) ? card.LinkCount : 1;
                        if (link_count >= 4) break;
                    }
                }
                if (link_count >= 3)
                {
                    AI.SelectCard(CardId.Veilynx);
                    return true;
                }
            }
            return false;
        }

        private bool HeatLeo_summon()
        {
            if (Duel.Turn == 1) return false;
            if (Duel.Phase != DuelPhase.Main1) return false;
            if (wasWolfSummonedUsingItself && Bot.GetMonsters().Count() <= 3) return false;

            ClientCard self_best = Util.GetBestBotMonster(true);
            int self_power = (self_best != null) ? self_best.Attack : 0;
            ClientCard enemy_best = Util.GetBestEnemyMonster(true);
            int enemy_power = (enemy_best != null) ? enemy_best.GetDefensePower() : 0;
            if (enemy_power < self_power) return false;

            if (Enemy.GetSpells().Where(x => x.IsFloodgate()).Count() > 0)
            {
                List<ClientCard> material_list = new List<ClientCard>();
                List<ClientCard> bot_monster = Bot.GetMonsters();
                bot_monster.Sort(CardContainer.CompareCardAttack);
                //bot_monster.Reverse();
                int link_count = 0;

                foreach (ClientCard card in bot_monster)
                {
                    if (card.IsFacedown()) continue;
                    if (!material_list.Contains(card) && card.LinkCount < 2)
                    {
                        material_list.Add(card);
                        link_count += (card.HasType(CardType.Link)) ? card.LinkCount : 1;
                        if (link_count >= 3) break;
                    }
                }
                if (link_count >= 3)
                {
                    AI.SelectMaterials(material_list);
                    return true;
                }
            }
            return false;
        }

        private bool Stallio_summon()
        {
            if (!wasStallioActivated)
            {
                AI.SelectMaterials(CardId.Spinny);
                return true;
            }
            return false;
        }

        private bool SunlightWolf_summon()
        {
            if (Bot.HasInMonstersZone(CardId.SunlightWolf))
            {
                if (wasWolfSummonedUsingItself)
                {
                    return false;
                }

                if (!wasFieldspellUsedThisTurn && Bot.HasInGraveyard(salamangreat_spellTrap) || Bot.HasInHandOrInSpellZone(CardId.SalamangreatRage))
                {
                    AI.SelectOption(1);
                    AI.SelectMaterials(new List<int>() {
                        CardId.SunlightWolf,
                        CardId.Veilynx,
                        CardId.JackJaguar,
                        CardId.Gazelle
                    });
                    wasWolfSummonedUsingItself = true;
                    AI.SelectPlace(sunlightPosition);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            wasWolfSummonedUsingItself = false;
            if (Bot.HasInMonstersZone(CardId.Veilynx))
            {

                if (Bot.HasInMonstersZone(CardId.MirageStallio)
                    && Bot.HasInMonstersZone(CardId.Veilynx)
                    && Bot.HasInMonstersZone(CardId.Gazelle))
                {
                    AI.SelectCard(CardId.Veilynx);
                    AI.SelectNextCard(CardId.MirageStallio);
                }
                else
                {
                    AI.SelectCard(WolfMaterials);
                    AI.SelectNextCard(WolfMaterials);
                }
                sunlightPosition = SelectSetPlace(new List<int>() { CardId.Veilynx }, true);

                AI.SelectPlace(sunlightPosition);
            }
            return true;
        }

        private bool Wolf_activate()
        {
            wasWolfActivatedThisTurn = true;
            AI.SelectCard(new List<int>() {
                CardId.Gazelle,
                CardId.SalamangreatRoar,
                CardId.SalamangreatRage,
                CardId.Foxy,
                CardId.AshBlossom,
                CardId.Fowl,
                CardId.SunlightWolf,
                CardId.Veilynx,
                CardId.HeatLeo,
                CardId.Spinny
            });
            return true;
        }

        private bool Stallio_activate()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {
                wasStallioActivated = true;
                if (!wasGazelleSummonedThisTurn)
                {
                    AI.SelectCard(CardId.Gazelle, CardId.Spinny);
                    AI.SelectNextCard(CardId.Gazelle);
                    return true;
                }
                if (!Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.JackJaguar))
                {
                    AI.SelectCard(CardId.Gazelle);
                    AI.SelectNextCard(CardId.JackJaguar);
                    return true;
                }
                if (!Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.Falco) && FalcoToGY(true))
                {
                    AI.SelectCard(CardId.Gazelle);
                    AI.SelectNextCard(CardId.Falco);
                    return true;
                }
                AI.SelectCard(CardId.Gazelle);
                return true;
            }
            else
            {
                if (Util.GetBestEnemyMonster(canBeTarget: true) != null)
                {
                    AI.SelectCard(Util.GetBestEnemyMonster(canBeTarget: true));
                    return true;
                }
            }
            return false;
        }

        private bool Veilynx_summon()
        {
            if (wasStallioActivated && wasWolfActivatedThisTurn)
            {
                return false;
            }
            if ((wasStallioActivated
                && !wasWolfActivatedThisTurn)
                ||
                (!wasStallioActivated
                && wasWolfActivatedThisTurn))
            {
                return false;
            }
            if (Bot.HasInHand(CardId.Gazelle)
                && !wasGazelleSummonedThisTurn
                && !Bot.HasInGraveyard(CardId.JackJaguar)
                && Bot.GetMonstersInMainZone().Where(x => x.Level == 3).Count() <= 1
                || (Bot.HasInMonstersZone(CardId.SunlightWolf)
                && !Bot.HasInSpellZoneOrInGraveyard(CardId.Sanctuary)
                && !wasWolfSummonedUsingItself))
            {

                var monsters = Bot.GetMonstersInMainZone();
                if (Bot.HasInMonstersZone(CardId.Veilynx) && monsters.Count == 2)
                {
                    return false;
                }

                monsters.Sort(CardContainer.CompareCardLevel);
                monsters.Reverse();
                AI.SelectMaterials(monsters);
                return true;
            }
            if (!Bot.HasInMonstersZone(CardId.Veilynx)
                &&
                Bot.GetMonstersInMainZone().Count >= 3
                &&
                (Bot.GetMonstersInExtraZone().Where(x => x.Owner == 0).Count() == 0))
            {
                var monsters = Bot.GetMonstersInMainZone();
                monsters.Sort(CardContainer.CompareCardLevel);
                monsters.Reverse();
                AI.SelectMaterials(monsters);
                return true;
            }


            if (CombosInHand.Where(x => x != CardId.Foxy).Where(x => x != CardId.Spinny).Count() == 0
                && Bot.HasInHand(CardId.Spinny))
            {
                if (Bot.HasInMonstersZone(CardId.Gazelle) && Bot.HasInMonstersZone(CardId.SunlightWolf))
                {
                    AI.SelectMaterials(CardId.Gazelle);
                    return true;
                }
                if (!wasVeilynxSummonedThisTurn)
                {
                    wasVeilynxSummonedThisTurn = true;
                    return true;
                }
            }

            return false;
        }

        private bool JackJaguar_activate()
        {
            if (Card.Location == CardLocation.Grave)
            {
                if (Bot.HasInGraveyard(JackJaguarTargets)
                    || Bot.Graveyard.Where(x => x.Id == CardId.Veilynx).Count() >= 2
                    || (!Bot.HasInGraveyard(salamangreat_spellTrap)
                    && Bot.HasInMonstersZone(CardId.SunlightWolf)
                    && Bot.HasInGraveyard(CardId.Gazelle)
                    && !Bot.HasInHand(CardId.Gazelle)))
                {
                    JackJaguarActivatedThisTurn = true;
                    if (Bot.Graveyard.Where(x => x.Id == CardId.Veilynx).Count() >= 2
                        && Bot.Graveyard.Select(x => x.Id).Intersect(JackJaguarTargets).Count() == 0)
                    {
                        AI.SelectCard(CardId.Veilynx);
                        return true;
                    }
                    AI.SelectCard(JackJaguarTargets);
                    return true;
                }
            }
            return false;
        }

        private bool Fowl_activate()
        {
            if (Card.Location == CardLocation.Hand)
            {
                return Bot.HasInMonstersZone(CardId.JackJaguar) && JackJaguarActivatedThisTurn;
            }
            return false;
        }

        private bool Spinny_activate()
        {
            if (Card.Location == CardLocation.Hand)
            {
                if (Bot.HasInGraveyard(CardId.Foxy) && !FoxyActivatedThisTurn) return false;
                if (CombosInHand.Where(x => x != CardId.Foxy).Where(x => x != CardId.Spinny).Count() == 0)
                {
                    return false;
                }

                if (!Bot.HasInMonstersZoneOrInGraveyard(CardId.Spinny)
                    && Util.GetBestBotMonster(true) != null
                    && !(Bot.GetMonsters().Count == 1
                    && Bot.HasInMonstersZone(CardId.Spinny)))
                {
                    AI.SelectCard(Util.GetBestBotMonster(true));
                    return true;
                }
            }
            return true;
        }

        private bool Falco_activate()
        {
            if (!falcoUsedReturnST && falcoHitGY)
            {
                if (Bot.HasInGraveyard(salamangreat_spellTrap))
                {
                    falcoUsedReturnST = true;
                    AI.SelectCard(salamangreat_spellTrap);
                    return true;
                }
            }
            return false;
        }

        private bool Gazelle_activate()
        {
            wasGazelleSummonedThisTurn = true;
            if (!Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.Spinny))
            {
                AI.SelectCard(CardId.Spinny);
                return true;
            }
            if (!Bot.HasInSpellZoneOrInGraveyard(CardId.SalamangreatRoar))
            {
                AI.SelectCard(CardId.SalamangreatRoar);
                return true;
            }
            if (!Bot.HasInSpellZoneOrInGraveyard(CardId.SalamangreatRage))
            {
                AI.SelectCard(CardId.SalamangreatRage);
                return true;
            }
            if (!Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.JackJaguar))
            {
                AI.SelectCard(CardId.JackJaguar);
                return true;
            }
            if (!Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.Foxy))
            {
                AI.SelectCard(CardId.Foxy);
                return true;
            }
            if (!Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.Falco))
            {
                AI.SelectCard(CardId.Falco);
                return true;
            }
            return true;
        }

        private bool Foxy_activate()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {

                if (CombosInHand.Where(x => x != CardId.Foxy).Where(x => x != CardId.Spinny).Count() == 0 && Bot.HasInHand(CardId.Spinny))
                {
                    return false;
                }
                AI.SelectCard(salamangreat_combopieces);
                FoxyActivatedThisTurn = true;
                return true;
            }
            else
            {
                if (DefaultCheckWhetherCardIsNegated(Card)) return false;
                if (Bot.HasInHand(CardId.Spinny) || FalcoToGY(false))
                {
                    if (Bot.HasInHand(CardId.Spinny) && !Bot.HasInGraveyard(CardId.Spinny))
                    {
                        AI.SelectCard(CardId.Spinny);
                    }
                    else
                    {
                        if (FalcoToGY(false))
                        {
                            AI.SelectCard(CardId.Falco);
                        }
                        else
                        {
                            return false;
                        }
                    }
                    if (Util.GetBestEnemySpell(true) != null)
                    {
                        AI.SelectNextCard(Util.GetBestEnemySpell(true));
                        foxyPopEnemySpell = true;
                    }
                    FoxyActivatedThisTurn = true;
                    return true;
                }
                return false;
            }
        }

        private bool FalcoToGY(bool FromDeck)
        {
            if (FromDeck && Bot.Deck.ContainsCardWithId(CardId.Falco))
            {
                if (Bot.HasInGraveyard(salamangreat_spellTrap))
                {
                    return true;
                }
                return false;
            }
            else
            {
                if (Bot.HasInHand(CardId.Falco) && Bot.HasInGraveyard(salamangreat_spellTrap))
                {
                    return true;
                }
                return false;
            }
        }
        private bool Fadydebug_activate()
        {
            if (!Bot.HasInHand(CardId.Gazelle))
            {
                AI.SelectCard(CardId.Gazelle);
                return true;
            }
            if (!Bot.HasInHandOrInGraveyard(CardId.Spinny))
            {
                AI.SelectCard(CardId.Spinny);
                return true;
            }
            if (!Bot.HasInHand(CardId.Foxy))
            {
                AI.SelectCard(CardId.Foxy);
                return true;
            }
            return true;
        }

        private bool Circle_activate()
        {
            var x = ActivateDescription;
            if (ActivateDescription == Util.GetStringId(CardId.Circle, 0) || ActivateDescription == 0)
            {
                AI.SelectOption(0);
                if (!Bot.HasInHand(CardId.Gazelle))
                {
                    AI.SelectCard(CardId.Gazelle);
                    return true;
                }
                if (!Bot.HasInHandOrInGraveyard(CardId.Spinny))
                {
                    AI.SelectCard(CardId.Spinny);
                    return true;
                }
                if (!Bot.HasInHand(CardId.Foxy))
                {
                    AI.SelectCard(CardId.Foxy);
                    return true;
                }
                if (!Bot.HasInHand(CardId.Fowl))
                {
                    AI.SelectCard(CardId.Fowl);
                    return true;
                }
                if (!Bot.HasInHand(CardId.JackJaguar))
                {
                    AI.SelectCard(CardId.JackJaguar);
                    return true;
                }
                if (!Bot.HasInHand(CardId.Falco))
                {
                    AI.SelectCard(CardId.Falco);
                    return true;
                }
                return false;
            }

            return false;
        }

        private bool FoolishBurial_activate()
        {
            if (FalcoToGY(true) && Bot.HasInHandOrInGraveyard(CardId.Spinny))
            {
                AI.SelectCard(CardId.Falco);
                return true;
            }
            AI.SelectCard(CardId.Spinny, CardId.JackJaguar, CardId.Foxy);
            return true;
        }

        private bool Sanctuary_activate()
        {
            if (Card.Location == CardLocation.Hand)
            {
                return true;
            }
            return false;
        }

        private bool Rage_activate()
        {
            if (ActivateDescription == Util.GetStringId(CardId.SalamangreatRage, 1))
            {
                AI.SelectCard(salamangreat_links);
                AI.SelectOption(1);
                IList<ClientCard> targets = new List<ClientCard>();

                ClientCard target1 = Util.GetBestEnemyMonster(canBeTarget: true);
                if (target1 != null)
                    targets.Add(target1);
                ClientCard target2 = Util.GetBestEnemySpell();
                if (target2 != null)
                    targets.Add(target2);

                foreach (ClientCard target in Enemy.GetMonsters())
                {
                    if (targets.Count >= 2)
                        break;
                    if (!targets.Contains(target))
                        targets.Add(target);
                }
                foreach (ClientCard target in Enemy.GetSpells())
                {
                    if (targets.Count >= 2)
                        break;
                    if (!targets.Contains(target))
                        targets.Add(target);
                }
                if (targets.Count == 0)
                    return false;
                AI.SelectNextCard(targets);
                return true;
            }
            else
            {
                if (Util.GetProblematicEnemyCard(canBeTarget: true) != null)
                {
                    if (Util.GetBestBotMonster(true) != null)
                    {
                        AI.SelectCard(Util.GetProblematicEnemyCard(Util.GetBestBotMonster(true).Attack, canBeTarget: true));
                    }
                    else
                    {
                        AI.SelectCard(Util.GetProblematicEnemyCard(canBeTarget: true));
                    }

                    return true;
                }
            }
            return false;
        }

        public bool G_activate()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            return (Duel.Player == 1);
        }
        public bool Hand_act_eff()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            return (Duel.LastChainPlayer == 1);
        }

        public bool Impermanence_activate()
        {
            if (!Should_counter()) return false;
            if (!spell_trap_activate()) return false;
            // negate before effect used
            foreach (ClientCard m in Enemy.GetMonsters())
            {
                if (m.IsMonsterShouldBeDisabledBeforeItUseEffect() && !m.IsDisabled() && Duel.LastChainPlayer != 0)
                {
                    if (Card.Location == CardLocation.SpellZone)
                    {
                        for (int i = 0; i < 5; ++i)
                        {
                            if (Bot.SpellZone[i] == Card)
                            {
                                Impermanence_list.Add(i);
                                break;
                            }
                        }
                    }
                    if (Card.Location == CardLocation.Hand)
                    {
                        AI.SelectPlace(SelectSTPlace(Card, true));
                    }
                    AI.SelectCard(m);
                    return true;
                }
            }

            ClientCard LastChainCard = Util.GetLastChainCard();

            // negate spells
            if (Card.Location == CardLocation.SpellZone)
            {
                int this_seq = -1;
                int that_seq = -1;
                for (int i = 0; i < 5; ++i)
                {
                    if (Bot.SpellZone[i] == Card) this_seq = i;
                    if (LastChainCard != null
                        && LastChainCard.Controller == 1 && LastChainCard.Location == CardLocation.SpellZone && Enemy.SpellZone[i] == LastChainCard) that_seq = i;
                    else if (Duel.Player == 0 && Util.GetProblematicEnemySpell() != null
                        && Enemy.SpellZone[i] != null && Enemy.SpellZone[i].IsFloodgate()) that_seq = i;
                }
                if ((this_seq * that_seq >= 0 && this_seq + that_seq == 4)
                    || (Util.IsChainTarget(Card))
                    || (LastChainCard != null && LastChainCard.Controller == 1 && LastChainCard.IsCode(_CardId.HarpiesFeatherDuster)))
                {
                    List<ClientCard> enemy_monsters = Enemy.GetMonsters();
                    enemy_monsters.Sort(CardContainer.CompareCardAttack);
                    enemy_monsters.Reverse();
                    foreach (ClientCard card in enemy_monsters)
                    {
                        if (card.IsFaceup() && !card.IsShouldNotBeTarget() && !card.IsShouldNotBeSpellTrapTarget())
                        {
                            AI.SelectCard(card);
                            Impermanence_list.Add(this_seq);
                            return true;
                        }
                    }
                }
            }
            if ((LastChainCard == null || LastChainCard.Controller != 1 || LastChainCard.Location != CardLocation.MonsterZone
                || LastChainCard.IsDisabled() || LastChainCard.IsShouldNotBeTarget() || LastChainCard.IsShouldNotBeSpellTrapTarget()))
                return false;
            // negate monsters
            if (is_should_not_negate() && LastChainCard.Location == CardLocation.MonsterZone) return false;
            if (Card.Location == CardLocation.SpellZone)
            {
                for (int i = 0; i < 5; ++i)
                {
                    if (Bot.SpellZone[i] == Card)
                    {
                        Impermanence_list.Add(i);
                        break;
                    }
                }
            }
            if (Card.Location == CardLocation.Hand)
            {
                AI.SelectPlace(SelectSTPlace(Card, true));
            }
            if (LastChainCard != null) AI.SelectCard(LastChainCard);
            else
            {
                List<ClientCard> enemy_monsters = Enemy.GetMonsters();
                enemy_monsters.Sort(CardContainer.CompareCardAttack);
                enemy_monsters.Reverse();
                foreach (ClientCard card in enemy_monsters)
                {
                    if (card.IsFaceup() && !card.IsShouldNotBeTarget() && !card.IsShouldNotBeSpellTrapTarget())
                    {
                        AI.SelectCard(card);
                        return true;
                    }
                }
            }
            return true;
        }

        public bool is_should_not_negate()
        {
            ClientCard last_card = Util.GetLastChainCard();
            if (last_card != null
                && last_card.Controller == 1 && last_card.IsCode(should_not_negate))
                return true;
            return false;
        }

        public bool SolemnStrike_activate()
        {
            if (!Should_counter()) return false;
            return (DefaultSolemnStrike() && spell_trap_activate(true));
        }

        public bool SolemnJudgment_activate()
        {
            return !Util.IsChainTargetOnly(Card)
                    &&
                    !(Duel.Player == 0
                    && Duel.LastChainPlayer == -1)
                    && DefaultTrap() && spell_trap_activate(true);
        }

        public bool spell_trap_activate(bool isCounter = false, ClientCard target = null)
        {
            if (target == null) target = Card;
            if (target.Location != CardLocation.SpellZone && target.Location != CardLocation.Hand) return true;
            if (target.IsSpell())
            {
                if (Enemy.HasInMonstersZone(CardId.NaturalBeast, true) && !Bot.HasInHandOrHasInMonstersZone(CardId.GO_SR) && !isCounter && !Bot.HasInSpellZone(CardId.SolemnStrike)) return false;
                if (Enemy.HasInSpellZone(CardId.ImperialOrder, true) || Bot.HasInSpellZone(CardId.ImperialOrder, true)) return false;
                if (Enemy.HasInMonstersZone(CardId.SwordsmanLV7, true) || Bot.HasInMonstersZone(CardId.SwordsmanLV7, true)) return false;
                return true;
            }
            if (target.IsTrap())
            {
                if (Enemy.HasInSpellZone(CardId.RoyalDecreel, true) || Bot.HasInSpellZone(CardId.RoyalDecreel, true)) return false;
                return true;
            }
            // how to get here?
            return false;
        }

        public bool Should_counter()
        {
            if (Duel.CurrentChain.Count < 2) return true;
            ClientCard self_card = Duel.CurrentChain[Duel.CurrentChain.Count - 2];
            if (self_card?.Controller != 0
                || !(self_card.Location == CardLocation.MonsterZone || self_card.Location == CardLocation.SpellZone)) return true;
            ClientCard enemy_card = Duel.CurrentChain[Duel.CurrentChain.Count - 1];
            if (enemy_card?.Controller != 1
                || !enemy_card.IsCode(normal_counter)) return true;
            return false;
        }
        public int SelectSTPlace(ClientCard card = null, bool avoid_Impermanence = false)
        {
            List<int> list = new List<int>();
            list.Add(0);
            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.Add(4);
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
                    if (card != null && card.Location == CardLocation.Hand && avoid_Impermanence && Impermanence_list.Contains(seq)) continue;
                    return zone;
                };
            }
            return 0;
        }

        public override bool OnSelectYesNo(int desc)
        {
            if (desc == Util.GetStringId(CardId.Sanctuary, 0))
            {
                wasFieldspellUsedThisTurn = true;
            }
            if (desc == Util.GetStringId(CardId.Foxy, 3))
            {
                return foxyPopEnemySpell;
            }
            return base.OnSelectYesNo(desc);
        }

        public override void OnNewTurn()
        {
            FoxyActivatedThisTurn = false;
            JackJaguarActivatedThisTurn = false;
            wasWolfActivatedThisTurn = false;
            wasStallioActivated = false;
            falcoUsedReturnST = false;
            CombosInHand = Bot.Hand.Select(h => h.Id).Intersect(Combo_cards).ToList();
            wasFieldspellUsedThisTurn = false;
            wasGazelleSummonedThisTurn = false;
            base.OnNewTurn();
        }

        public override bool OnSelectHand()
        {
            return true;
        }

        public bool SpellSet()
        {
            if (Card.Id == CardId.Circle)
            {
                return false;
            }
            if (Duel.Phase == DuelPhase.Main1 && Bot.HasAttackingMonster() && Duel.Turn > 1) return false;
            if (Card.IsCode(CardId.SolemnStrike) && Bot.LifePoints <= 1500) return false;
            if ((Card.IsTrap() || Card.HasType(CardType.QuickPlay)))
            {
                List<int> avoid_list = new List<int>();
                int Impermanence_set = 0;
                for (int i = 0; i < 5; ++i)
                {
                    if (Enemy.SpellZone[i] != null && Enemy.SpellZone[i].IsFaceup() && Bot.SpellZone[4 - i] == null)
                    {
                        avoid_list.Add(4 - i);
                        Impermanence_set += (int)System.Math.Pow(2, 4 - i);
                    }
                }
                if (Bot.HasInHand(CardId.Impermanence))
                {
                    if (Card.IsCode(CardId.Impermanence))
                    {
                        AI.SelectPlace(Impermanence_set);
                        return true;
                    }
                    else
                    {
                        AI.SelectPlace(SelectSetPlace(avoid_list));
                        return true;
                    }
                }
                else
                {
                    AI.SelectPlace(SelectSTPlace());
                }
                return true;
            }
            return false;
        }

        public bool Called_activate()
        {
            if (!DefaultUniqueTrap())
                return false;

            if (Duel.Player == 1)
            {
                ClientCard target = Enemy.MonsterZone.GetShouldBeDisabledBeforeItUseEffectMonster();
                if (target != null && Enemy.HasInGraveyard(target.Id))
                {
                    AI.SelectCard(target.Id);
                    return true;
                }
            }

            ClientCard LastChainCard = Util.GetLastChainCard();

            if (LastChainCard != null
                && LastChainCard.Controller == 1
                && (LastChainCard.Location == CardLocation.Grave
                || LastChainCard.Location == CardLocation.Hand
                || LastChainCard.Location == CardLocation.MonsterZone
                || LastChainCard.Location == CardLocation.Removed)
                && !LastChainCard.IsDisabled() && !LastChainCard.IsShouldNotBeTarget()
                && !LastChainCard.IsShouldNotBeSpellTrapTarget()
                && Enemy.HasInGraveyard(LastChainCard.Id))
            {
                AI.SelectCard(LastChainCard.Id);
                return true;
            }

            if (Bot.BattlingMonster != null && Enemy.BattlingMonster != null)
            {
                if (!Enemy.BattlingMonster.IsDisabled() && Enemy.BattlingMonster.IsCode(_CardId.EaterOfMillions) && Enemy.HasInGraveyard(_CardId.EaterOfMillions))
                {
                    AI.SelectCard(Enemy.BattlingMonster.Id);
                    return true;
                }
            }

            if (Duel.Phase == DuelPhase.BattleStart && Duel.Player == 1 &&
                Enemy.HasInMonstersZone(_CardId.NumberS39UtopiaTheLightning, true) && Enemy.HasInGraveyard(_CardId.NumberS39UtopiaTheLightning))
            {
                AI.SelectCard(_CardId.NumberS39UtopiaTheLightning);
                return true;
            }

            return false;
        }

        public bool Borrelsword_ss()
        {
            if (Duel.Phase != DuelPhase.Main1) return false;
            if (Duel.Turn == 1) return false;
            if (wasStallioActivated) return false;

            List<ClientCard> material_list = new List<ClientCard>();
            List<ClientCard> bot_monster = Bot.GetMonsters();
            bot_monster.Sort(CardContainer.CompareCardAttack);
            //bot_monster.Reverse();
            int link_count = 0;
            foreach (ClientCard card in bot_monster)
            {
                if (card.IsFacedown()) continue;
                if (!material_list.Contains(card) && card.LinkCount < 3)
                {
                    material_list.Add(card);
                    link_count += (card.HasType(CardType.Link)) ? card.LinkCount : 1;
                }
            }
            if (link_count >= 4)
            {
                if (link_count > 4 && material_list.Where(x => x.Id == CardId.MirageStallio).Count() > 0)
                {
                    material_list.Remove(material_list.First(x => x.Id == CardId.MirageStallio));
                }
                AI.SelectMaterials(material_list);
                return true;
            }
            return false;
        }

        public bool Borrelsword_eff()
        {
            if (ActivateDescription == -1) return true;
            else if ((Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2) || Util.IsChainTarget(Card))
            {
                List<ClientCard> enemy_list = Enemy.GetMonsters();
                enemy_list.Sort(CardContainer.CompareCardAttack);
                enemy_list.Reverse();
                foreach (ClientCard card in enemy_list)
                {
                    if (card.HasPosition(CardPosition.Attack) && !card.HasType(CardType.Link))
                    {
                        AI.SelectCard(card);
                        return true;
                    }
                }
                List<ClientCard> bot_list = Bot.GetMonsters();
                bot_list.Sort(CardContainer.CompareCardAttack);
                //bot_list.Reverse();
                foreach (ClientCard card in bot_list)
                {
                    if (card.HasPosition(CardPosition.Attack) && !card.HasType(CardType.Link))
                    {
                        AI.SelectCard(card);
                        return true;
                    }
                }
            }
            return false;
        }
        public override void OnChainEnd()
        {
            if (!falcoHitGY && !falcoUsedReturnST && Bot.HasInGraveyard(CardId.Falco))
            {
                falcoHitGY = true;
            }
            else if (!Bot.HasInGraveyard(CardId.Falco))
            {
                falcoHitGY = false;
            }
            base.OnChainEnd();
        }

        public override int OnSelectPlace(int cardId, int player, CardLocation location, int available)
        {
            if (player == 0)
            {
                if (location == CardLocation.MonsterZone)
                {
                    if (Bot.GetMonstersInExtraZone().Where(x => x.Id == CardId.SunlightWolf).Count() > 1)
                    {
                        for (int i = 0; i < 7; ++i)
                        {
                            if (Bot.MonsterZone[i] != null && Bot.MonsterZone[i].IsCode(CardId.SunlightWolf))
                            {
                                int next_index = get_Wolf_linkzone();
                                if (next_index != -1 && (available & (int)(System.Math.Pow(2, next_index))) > 0)
                                {
                                    return (int)(System.Math.Pow(2, next_index));
                                }
                            }
                        }
                    }
                }
            }
            return base.OnSelectPlace(cardId, player, location, available);
        }
        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            if (Util.IsTurn1OrMain2()
                &&
                (cardId == CardId.Gazelle
                || cardId == CardId.Spinny
                || cardId == CardId.Foxy))
            {
                return CardPosition.FaceUpDefence;
            }
            return 0;
        }

        public int SelectSetPlace(List<int> avoid_list = null, bool avoid = true)
        {
            List<int> list = new List<int>();
            list.Add(5);
            list.Add(6);
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

                if (Bot.MonsterZone[seq] == null || !avoid)
                {
                    if (avoid)
                    {
                        if (avoid_list != null && avoid_list.Contains(seq)) continue;
                        return zone;
                    }
                    else
                    {
                        if (avoid_list != null && avoid_list.Contains(seq))
                        {
                            return list.First(x => x == seq);
                        }
                        continue;
                    }

                };
            }
            return 0;
        }
        public override BattlePhaseAction OnSelectAttackTarget(ClientCard attacker, IList<ClientCard> defenders)
        {
            foreach (ClientCard defender in defenders)
            {
                attacker.RealPower = attacker.Attack;
                defender.RealPower = defender.GetDefensePower();
                if (attacker.IsCode(CardId.Borrelsword) && !attacker.IsDisabled())
                    return AI.Attack(attacker, defender);
                if (!OnPreBattleBetween(attacker, defender))
                    continue;

                if (attacker.RealPower > defender.RealPower || (attacker.RealPower > defender.RealPower && attacker.IsLastAttacker && defender.IsAttack()))
                    return AI.Attack(attacker, defender);
            }

            if (attacker.CanDirectAttack)
                return AI.Attack(attacker, null);

            return null;
        }
    }
}
