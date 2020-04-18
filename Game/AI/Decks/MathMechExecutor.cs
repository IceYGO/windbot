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
        public class CardID
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
            AddExecutor(ExecutorType.Activate, CardID.raigeki ,when_raigeki);
            AddExecutor(ExecutorType.Activate, CardID.upstartGoblin);
            AddExecutor(ExecutorType.Activate, CardID.OneTimePasscode);
            AddExecutor(ExecutorType.SpellSet, CardID.threanteningRoar);
            AddExecutor(ExecutorType.Activate,CardID.cosmicCyclone , when_cosmic);
            AddExecutor(ExecutorType.Activate,CardID.lightningStorm ,lightstorm_target);
            AddExecutor(ExecutorType.Activate,CardID.foolishBurial,foolish_burial_target);
            AddExecutor(ExecutorType.Activate,CardID.mathmechEquation,mathmech_equation_target);
            AddExecutor(ExecutorType.Activate,CardID.PotOfDesires);


            AddExecutor(ExecutorType.Summon, CardID.MathmechNebla);
            AddExecutor(ExecutorType.Summon,CardID.BalancerLord );
            AddExecutor(ExecutorType.Summon, CardID.Mathmechdouble);
            AddExecutor(ExecutorType.Summon, CardID.MathmechSubtra);
            AddExecutor(ExecutorType.Summon, CardID.MathmechAddition);
            AddExecutor(ExecutorType.Summon, CardID.MathmechDivision);
            AddExecutor(ExecutorType.Summon, CardID.MathmechDivision);
            AddExecutor(ExecutorType.Activate, CardID.MathmechSigma);
            AddExecutor(ExecutorType.Activate,CardID.threanteningRoar);

            //xyz summons
            AddExecutor(ExecutorType.SpSummon, CardID.Mathmechalem, when_Mathmechalem);
            //xyz effects
            AddExecutor(ExecutorType.Activate, CardID.Mathmechalem, mathchalenEffect);
            //Synchro
            AddExecutor(ExecutorType.SpSummon, CardID.MathmechFinalSigma , FinalSigmaSummon);
            
            AddExecutor(ExecutorType.Activate, CardID.Mathmechdouble, doubleEffect);

            //normal effects
            AddExecutor(ExecutorType.Activate, CardID.MathmechNebla, NeblaEffect);
            AddExecutor(ExecutorType.Activate,CardID.MathmechDivision , divisionEffect);
            AddExecutor(ExecutorType.Activate,CardID.BalancerLord , active_balancer);
            AddExecutor(ExecutorType.Activate, CardID.MathmechSubtra , whom_subtra);
            AddExecutor(ExecutorType.Activate, CardID.MathmechAddition , whom_addition);
            //spell effects
            AddExecutor(ExecutorType.Activate, CardID.cynetmining , how_to_cynet_mine);
            AddExecutor(ExecutorType.SpSummon, CardID.MathmechMagma, MagmaSummon);
            AddExecutor(ExecutorType.Activate,CardID.MathmechFinalSigma);
            AddExecutor(ExecutorType.Activate,CardID.MathmechMagma);
            

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
            if (Bot.HasInHand(CardID.MathmechNebla))
            {
                AI.SelectCard(CardID.MathmechNebla);
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
            if (Bot.HasInGraveyard(CardID.MathmechNebla))
            {
                AI.SelectCard(CardID.MathmechNebla);
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
            AI.SelectCard(CardID.MathmechNebla);
            return true;
        }
        private bool how_to_cynet_mine()
        {
            AI.SelectCard(Util.GetWorstBotMonster());
            if (!Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardID.MathmechSigma))
            {
                AI.SelectNextCard(CardID.MathmechSigma);
                return true;
            }
            return true;
        }
        private bool when_Mathmechalem()
        {
            if (Bot.HasInMonstersZone(CardID.MathmechNebla)){
                return false;
            }
            else if(Bot.HasInMonstersZone(CardID.MathmechSigma) && Bot.HasInMonstersZone(CardID.Mathmechdouble))
            {
                return false;
            }
            else if (Bot.HasInMonstersZone(CardID.Mathmechalem))
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
            if ((Bot.HasInMonstersZone(CardID.Mathmechdouble) && (( Bot.HasInMonstersZone(CardID.MathmechSigma)) || Bot.HasInMonstersZone(CardID.MathmechNebla))))
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
            bool a = Bot.HasInMonstersZone(CardID.MathmechSubtra) || Bot.HasInMonstersZone(CardID.securitytoken) || Bot.HasInMonstersZone(CardID.MathmechSigma) || Bot.HasInMonstersZone(CardID.MathmechAddition) || Bot.HasInMonstersZone(CardID.Mathmechalem) || Bot.HasInMonstersZone(CardID.MathmechDivision);
            if (a)
            {
                List<int> cards = new List<int>();
                cards.Add(CardID.MathmechSigma);
                cards.Add(CardID.MathmechSubtra);
                cards.Add(CardID.MathmechAddition);
                cards.Add(item:CardID.MathmechDivision);
                cards.Add(item:CardID.Mathmechalem);
                cards.Add(CardID.securitytoken);
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
                        u = CardID.securitytoken;
                    }
                }
                AI.SelectCard(CardID.securitytoken);
                AI.SelectNextCard(CardID.Mathmechdouble);
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
            if (Bot.HasInMonstersZone(CardID.MathmechNebla) || Bot.HasInMonstersZone(CardID.MathmechSigma))
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
            if ( (Bot.HasInHandOrInGraveyard(CardID.MathmechNebla) &&  !Bot.HasInMonstersZone(CardID.MathmechNebla)) && (Card.Location == CardLocation.FieldZone && Card.HasXyzMaterial(0)) )
            {
                AI.SelectCard(CardID.Mathmechalem);
                AI.SelectNextCard(CardID.MathmechNebla);
                return true;
            }

            if (Bot.HasInHandOrInGraveyard(CardID.Mathmechdouble) &&
                (Bot.HasInMonstersZone(CardID.MathmechNebla) || Bot.HasInMonstersZone(CardID.MathmechSigma)) &&
                Card.Location == CardLocation.FieldZone && Card.HasXyzMaterial(0))
            {
                AI.SelectCard(CardID.Mathmechalem);
                AI.SelectNextCard(CardID.Mathmechdouble);
                return true;
            }
            if (!Bot.HasInHandOrInGraveyard(CardID.MathmechNebla) && Card.HasXyzMaterial(2))
            {
                AI.SelectCard(CardID.MathmechNebla);
                AI.SelectThirdCard(CardID.MathmechNebla);
                return true;
            }

            if (!Bot.HasInHandOrInGraveyard(CardID.MathmechSigma) && Card.HasXyzMaterial(2))
            {
                AI.SelectCard(CardID.MathmechSigma);
                AI.SelectThirdCard(CardID.MathmechSigma);
                return true;
            }
            else
            {
                return false;
            };
            
            
        }

        private bool MagmaSummon()
        {
            if (Bot.HasInMonstersZone(CardID.MathmechNebla))
            {
                return false;
            }

            if (Bot.HasInMonstersZone(CardID.MathmechSigma) && Bot.HasInMonstersZone(CardID.Mathmechdouble))
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
            if (cardId == CardID.MathmechFinalSigma)
            {
                if ((Zones.z5 & available) > 0) return Zones.z5;
                if ((Zones.z6 & available) > 0) return Zones.z6;
            }
            return base.OnSelectPlace(cardId, player, location, available);
        }

    }

}