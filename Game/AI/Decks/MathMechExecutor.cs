using System;
using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using System.Diagnostics;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using System.Linq;

namespace WindBot.Game.AI.Decks
{
    [Deck("MathMech", "AI_Mathmech")]
    public class MathmechExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int MathmechNebla = 53577438;
            public const int MathmechSigma = 27182739;
            public const int MathmechDivision = 89743495;
            public const int MathmechAddition = 80965043;
            public const int MathmechSubtra = 16360142;
            public const int Mathmechdouble = 52354896;
            public const int MathmechFinalSigma = 42632209;
            public const int Mathmechalem = 85692042;
            public const int MathmechMagma = 15248594;
            public const int BalancerLord = 08567955;
            public const int LightDragon = 61399402;

            // spells
            public const int upstartGoblin = 70368879;
            public const int raigeki = 12580477;
            public const int cynetmining = 57160136;
            public const int  PotOfDesires= 35261759;
            public const int lightningStorm =  14532163;
            public const int cosmicCyclone = 08267140;
            public const int foolishBurial = 81439173;
            public const int OneTimePasscode = 93104632;
            public const int mathmechEquation = 14025912;
            //traps
            public const int threanteningRoar = 36361633;
            //tokens
            public const int securitytoken = 93104633;

        }
        public MathmechExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, CardId.raigeki ,when_raigeki);
            AddExecutor(ExecutorType.Activate, CardId.upstartGoblin);
            AddExecutor(ExecutorType.Activate, CardId.OneTimePasscode);
            AddExecutor(ExecutorType.SpellSet, CardId.threanteningRoar);
            AddExecutor(ExecutorType.Activate,CardId.cosmicCyclone , when_cosmic);
            AddExecutor(ExecutorType.Activate,CardId.lightningStorm ,lightstorm_target);
            AddExecutor(ExecutorType.Activate,CardId.foolishBurial,foolish_burial_target);
            AddExecutor(ExecutorType.Activate,CardId.mathmechEquation,mathmech_equation_target);
            AddExecutor(ExecutorType.Activate,CardId.PotOfDesires);


            AddExecutor(ExecutorType.Summon, CardId.MathmechNebla);
            AddExecutor(ExecutorType.Summon,CardId.BalancerLord );
            AddExecutor(ExecutorType.Summon, CardId.Mathmechdouble);
            AddExecutor(ExecutorType.Summon, CardId.MathmechSubtra);
            AddExecutor(ExecutorType.Summon, CardId.MathmechAddition);
            AddExecutor(ExecutorType.Summon, CardId.MathmechDivision);
            AddExecutor(ExecutorType.Summon, CardId.MathmechDivision);
            AddExecutor(ExecutorType.Activate, CardId.MathmechSigma);
            AddExecutor(ExecutorType.Activate,CardId.threanteningRoar);

            //xyz summons
            AddExecutor(ExecutorType.SpSummon, CardId.Mathmechalem, when_Mathmechalem);
            //xyz effects
            AddExecutor(ExecutorType.Activate, CardId.Mathmechalem, mathchalenEffect);
            //Synchro
            AddExecutor(ExecutorType.SpSummon, CardId.MathmechFinalSigma , FinalSigmaSummon);
            
            AddExecutor(ExecutorType.Activate, CardId.Mathmechdouble, doubleEffect);

            //normal effects
            AddExecutor(ExecutorType.Activate, CardId.MathmechNebla, NeblaEffect);
            AddExecutor(ExecutorType.Activate,CardId.MathmechDivision , divisionEffect);
            AddExecutor(ExecutorType.Activate,CardId.BalancerLord , active_balancer);
            AddExecutor(ExecutorType.Activate, CardId.MathmechSubtra , whom_subtra);
            AddExecutor(ExecutorType.Activate, CardId.MathmechAddition , whom_addition);
            //spell effects
            AddExecutor(ExecutorType.Activate, CardId.cynetmining , how_to_cynet_mine);
            AddExecutor(ExecutorType.SpSummon, CardId.MathmechMagma, MagmaSummon);
            AddExecutor(ExecutorType.Activate,CardId.MathmechFinalSigma);
            AddExecutor(ExecutorType.Activate,CardId.MathmechMagma);
            

            //function
            
        }

        public override bool OnSelectHand()
        {
            return false;
        }
        private bool when_cosmic()
        {
            
            if (Enemy.GetSpellCount() > 1)
            {
                AI.SelectCard(Util.GetBestEnemySpell());
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool divisionEffect()
        {
            if (Enemy.GetMonsterCount() > 0)
            {
                AI.SelectCard(Util.GetBestEnemyMonster(canBeTarget:true,onlyFaceup:true));
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool when_raigeki()
        {
            if (Enemy.GetMonsterCount() > 3)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool whom_addition()
        {
            AI.SelectCard(Util.GetBestBotMonster(onlyATK:true));
            return true;
        }

        private bool whom_subtra()
        {
            try
            {
                AI.SelectCard(Util.GetBestEnemyMonster(onlyFaceup: true, canBeTarget: true));
                return true;
            }
            catch (Exception e)
            {
                return true;
            }
        }

        private bool active_balancer()
        {
            if (Bot.HasInHand(CardId.MathmechNebla))
            {
                AI.SelectCard(CardId.MathmechNebla);
                return true;
            }
            else
            {
                return true;
            }
        }
        private bool lightstorm_target()
        {
            if ((Enemy.MonsterZone.ToList().Count > Enemy.SpellZone.ToList().Count ) && Enemy.MonsterZone.ToList().Count>3)
            {
                AI.SelectPlace(Zones.MonsterZones);
                return true;
            }
            else
            {
                AI.SelectPlace(Zones.SpellZones);
                return true;
            }

        }

        private bool mathmech_equation_target()
        {
            if (Bot.HasInGraveyard(CardId.MathmechNebla))
            {
                AI.SelectCard(CardId.MathmechNebla);
                return true;
            }
            else
            {
                AI.SelectCard((Util.GetBestBotMonster(onlyATK: true)));
                return true;
            }
        }

        private bool foolish_burial_target()
        {
            AI.SelectCard(CardId.MathmechNebla);
            return true;
        }
        private bool how_to_cynet_mine()
        {
            AI.SelectCard(Util.GetWorstBotMonster());
            if (!Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.MathmechSigma))
            {
                AI.SelectNextCard(CardId.MathmechSigma);
                return true;
            }
            return true;
        }
        private bool when_Mathmechalem()
        {
            if (Bot.HasInMonstersZone(CardId.MathmechNebla)){
                return false;
            }
            else if(Bot.HasInMonstersZone(CardId.MathmechSigma) && Bot.HasInMonstersZone(CardId.Mathmechdouble))
            {
                return false;
            }
            else if (Bot.HasInMonstersZone(CardId.Mathmechalem))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private bool FinalSigmaSummon()
        {
            if (Duel.Turn < 1)
            {
                return false;
            }
            if ((Bot.HasInMonstersZone(CardId.Mathmechdouble) && (( Bot.HasInMonstersZone(CardId.MathmechSigma)) || Bot.HasInMonstersZone(CardId.MathmechNebla))))
            {
                AI.SelectPosition(CardPosition.Attack);
                try { AI.SelectPlace(Zones.ExtraMonsterZones);  }
                catch { }
                
                return true;
            }
            else
            {
                return true;
            }

        }
        private bool NeblaEffect()
        {
            bool a = Bot.HasInMonstersZone(CardId.MathmechSubtra) || Bot.HasInMonstersZone(CardId.securitytoken) || Bot.HasInMonstersZone(CardId.MathmechSigma) || Bot.HasInMonstersZone(CardId.MathmechAddition) || Bot.HasInMonstersZone(CardId.Mathmechalem) || Bot.HasInMonstersZone(CardId.MathmechDivision);
            if (a)
            {
                List<int> cards = new List<int>();
                cards.Add(CardId.MathmechSigma);
                cards.Add(CardId.MathmechSubtra);
                cards.Add(CardId.MathmechAddition);
                cards.Add(item:CardId.MathmechDivision);
                cards.Add(item:CardId.Mathmechalem);
                cards.Add(CardId.securitytoken);
                int u = 0;
                List<ClientCard> monsters = Bot.GetMonstersInMainZone();
                for (int i = 0; i < monsters.Count; i++)
                {
                    if (cards.Contains(monsters[i].Id))
                    {
                        u = monsters[i].Id;
                        break;
                    }
                    else
                    {
                        u = CardId.securitytoken;
                    }
                }
                AI.SelectCard(CardId.securitytoken);
                AI.SelectNextCard(CardId.Mathmechdouble);
                return true;


            }
            if (Card.Location == CardLocation.Grave)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool doubleEffect()
        {
            if (Bot.HasInMonstersZone(CardId.MathmechNebla) || Bot.HasInMonstersZone(CardId.MathmechSigma))
            {
                return true;
            };
            if (Card.Location == CardLocation.Grave )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        
        private bool mathchalenEffect()

        {
            if (Duel.Turn < 1)
            {
                return false;
            }
            if ( (Bot.HasInHandOrInGraveyard(CardId.MathmechNebla) &&  !Bot.HasInMonstersZone(CardId.MathmechNebla)) && (Card.Location == CardLocation.FieldZone && Card.HasXyzMaterial(0)) )
            {
                AI.SelectCard(CardId.Mathmechalem);
                AI.SelectNextCard(CardId.MathmechNebla);
                return true;
            }

            if (Bot.HasInHandOrInGraveyard(CardId.Mathmechdouble) &&
                (Bot.HasInMonstersZone(CardId.MathmechNebla) || Bot.HasInMonstersZone(CardId.MathmechSigma)) &&
                Card.Location == CardLocation.FieldZone && Card.HasXyzMaterial(0))
            {
                AI.SelectCard(CardId.Mathmechalem);
                AI.SelectNextCard(CardId.Mathmechdouble);
                return true;
            }
            if (!Bot.HasInHandOrInGraveyard(CardId.MathmechNebla) && Card.HasXyzMaterial(2))
            {
                AI.SelectCard(CardId.MathmechNebla);
                AI.SelectThirdCard(CardId.MathmechNebla);
                return true;
            }

            if (!Bot.HasInHandOrInGraveyard(CardId.MathmechSigma) && Card.HasXyzMaterial(2))
            {
                AI.SelectCard(CardId.MathmechSigma);
                AI.SelectThirdCard(CardId.MathmechSigma);
                return true;
            }
            else
            {
                return false;
            };
            
            
        }

        private bool MagmaSummon()
        {
            if (Bot.HasInMonstersZone(CardId.MathmechNebla))
            {
                return false;
            }

            if (Bot.HasInMonstersZone(CardId.MathmechSigma) && Bot.HasInMonstersZone(CardId.Mathmechdouble))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public override int OnSelectPlace(int cardId, int player, CardLocation location, int available)
        {
            if (cardId == CardId.MathmechFinalSigma)
            {
                if ((Zones.z5 & available) > 0) return Zones.z5;
                if ((Zones.z6 & available) > 0) return Zones.z6;
            }
            return base.OnSelectPlace(cardId, player, location, available);
        }

    }

}