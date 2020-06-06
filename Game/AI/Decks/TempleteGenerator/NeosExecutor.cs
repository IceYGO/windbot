using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using YGOSharp.OCGWrapper.Enums;
namespace WindBot.Game.AI.Decks
{
   
   [Deck("Neos", "Neo Jaden")]
    public class NeosExecutor: DefaultExecutor
    {
        public class CardId
        {
            // Initialize all normal monsters

            // Create all methods needed for ElementalHERONeos to work
            public const int ElementalHERONeos = 89943723;
            // Initialize all effect monsters

            // Create all methods needed for ElementalHEROHonestNeos to work
            public const int ElementalHEROHonestNeos = 14124483;

            // Create all methods needed for ElementalHERONeosAlius to work
            public const int ElementalHERONeosAlius = 69884162;

            // Create all methods needed for NeoSpacePathFinder to work
            public const int NeoSpacePathFinder = 19594506;

            // Create all methods needed for ElementalHEROStratos to work
            public const int ElementalHEROStratos = 40044918;

            // Create all methods needed for ElementalHEROPrisma to work
            public const int ElementalHEROPrisma = 89312388;

            // Create all methods needed for ElementalHEROBlazeman to work
            public const int ElementalHEROBlazeman = 63060238;

            // Create all methods needed for NeoSpaceConnector to work
            public const int NeoSpaceConnector = 85840608;

            // Create all methods needed for NeoSpacienGrandMole to work
            public const int NeoSpacienGrandMole = 80344569;

            // Create all methods needed for NeoSpacianAirHummingbird to work
            public const int NeoSpacianAirHummingbird = 54959865;

            // Create all methods needed for NeoSpacianAquaDolphin to work
            public const int NeoSpacianAquaDolphin = 17955766;

            // Create all methods needed for NeoSpacianFlareScarab to work
            public const int NeoSpacianFlareScarab = 89621922;
            // Initialize all special summonable effect monsters
            // Initialize all pure special summonable effect monsters

            // Create all methods needed for ElementalHEROStormNeos to work
            public const int ElementalHEROStormNeos = 49352945;

            // Create all methods needed for ElementalHeroMagmaNeos to work
            public const int ElementalHeroMagmaNeos = 78512663;

            // Create all methods needed for ElementalHeroAirNeos to work
            public const int ElementalHeroAirNeos = 11502550;

            // Create all methods needed for ElementalHeroGrandNeos to work
            public const int ElementalHeroGrandNeos = 48996569;

            // Create all methods needed for ElementalHeroAquaNeos to work
            public const int ElementalHeroAquaNeos = 55171412;

            // Create all methods needed for ElementalHeroBraveNeos to work
            public const int ElementalHeroBraveNeos = 64655485;

            // Create all methods needed for ElementalHeroFlareNeos to work
            public const int ElementalHeroFlareNeos = 81566151;

            // Create all methods needed for ElementalHeroSunrise to work
            public const int ElementalHeroSunrise = 22908820;

            // Create all methods needed for ElementalHeroNeosKnight to work
            public const int ElementalHeroNeosKnight = 72926163;
            // Initialize all spell and trap cards

            // Create all methods needed for MonsterReborn to work
            public const int MonsterReborn = 83764718;

            // Create all methods needed for NeosFusion to work
            public const int NeosFusion = 14088859;

            // Create all methods needed for Polymerization to work
            public const int Polymerization = 24094653;

            // Create all methods needed for MiracleContact to work
            public const int MiracleContact = 35255456;

            // Create all methods needed for InstantNeoSpace to work
            public const int InstantNeoSpace = 11913700;

            // Create all methods needed for NeoSpace to work
            public const int NeoSpace = 42015635;

            // Create all methods needed for DrowningMirrorForce to work
            public const int DrowningMirrorForce = 47475363;

            // Create all methods needed for NEXT to work
            public const int NEXT = 74414885;

            // Create all methods needed for CallOfTheHaunted to work
            public const int CallOfTheHaunted = 97077563;
            // Initialize all useless cards

            // Create all methods needed for AshBlossomJoyusSpring to work
            public const int AshBlossomJoyusSpring = 14558127;

            // Create all methods needed for GhostReaperWinterCherries to work
            public const int GhostReaperWinterCherries = 62015408;

            // Create all methods needed for PsyFrameGearGamma to work
            public const int PsyFrameGearGamma = 38814750;

            // Create all methods needed for DrollLockBird to work
            public const int DrollLockBird = 94145021;

            // Create all methods needed for ArtifactLancea to work
            public const int ArtifactLancea = 34267821;

            // Create all methods needed for GhostOgreSnowRabbit to work
            public const int GhostOgreSnowRabbit = 59438930;

            // Create all methods needed for NimbuThePrimeBeing to work
            public const int NimbuThePrimeBeing = 27204311;

            // Create all methods needed for EffectVailer to work
            public const int EffectVailer = 97268402;

            // Create all methods needed for DDCrow to work
            public const int DDCrow = 24508238;

            // Create all methods needed for Honest to work
            public const int Honest = 37742478;

            // Create all methods needed for Tragodia to work
            public const int Tragodia = 9877036;

            // Create all methods needed for GorzTheEmmisaryOfDarkness to work
            public const int GorzTheEmmisaryOfDarkness = 44330098;

         }
        public NeosExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // Add Executors to all normal monsters

            // Create all methods needed for ElementalHERONeos to work
            AddExecutor(ExecutorType.Summon, CardId.ElementalHERONeos, ElementalHERONeosNormalSummon);
            AddExecutor(ExecutorType.MonsterSet, CardId.ElementalHERONeos, ElementalHERONeosMonsterSet);
            AddExecutor(ExecutorType.Repos, CardId.ElementalHERONeos, ElementalHERONeosRepos);
            // Add Executors to all effect monsters

            // Create all methods needed for ElementalHEROHonestNeos to work
            AddExecutor(ExecutorType.Summon, CardId.ElementalHEROHonestNeos, ElementalHEROHonestNeosNormalSummon);
            AddExecutor(ExecutorType.MonsterSet, CardId.ElementalHEROHonestNeos, ElementalHEROHonestNeosMonsterSet);
            AddExecutor(ExecutorType.Repos, CardId.ElementalHEROHonestNeos, ElementalHEROHonestNeosRepos);
            AddExecutor(ExecutorType.Activate, CardId.ElementalHEROHonestNeos, ElementalHEROHonestNeosActivate);

            // Create all methods needed for ElementalHERONeosAlius to work
            AddExecutor(ExecutorType.Summon, CardId.ElementalHERONeosAlius, ElementalHERONeosAliusNormalSummon);
            AddExecutor(ExecutorType.MonsterSet, CardId.ElementalHERONeosAlius, ElementalHERONeosAliusMonsterSet);
            AddExecutor(ExecutorType.Repos, CardId.ElementalHERONeosAlius, ElementalHERONeosAliusRepos);
            AddExecutor(ExecutorType.Activate, CardId.ElementalHERONeosAlius, ElementalHERONeosAliusActivate);

            // Create all methods needed for NeoSpacePathFinder to work
            AddExecutor(ExecutorType.Summon, CardId.NeoSpacePathFinder, NeoSpacePathFinderNormalSummon);
            AddExecutor(ExecutorType.MonsterSet, CardId.NeoSpacePathFinder, NeoSpacePathFinderMonsterSet);
            AddExecutor(ExecutorType.Repos, CardId.NeoSpacePathFinder, NeoSpacePathFinderRepos);
            AddExecutor(ExecutorType.Activate, CardId.NeoSpacePathFinder, NeoSpacePathFinderActivate);

            // Create all methods needed for ElementalHEROStratos to work
            AddExecutor(ExecutorType.Summon, CardId.ElementalHEROStratos, ElementalHEROStratosNormalSummon);
            AddExecutor(ExecutorType.MonsterSet, CardId.ElementalHEROStratos, ElementalHEROStratosMonsterSet);
            AddExecutor(ExecutorType.Repos, CardId.ElementalHEROStratos, ElementalHEROStratosRepos);
            AddExecutor(ExecutorType.Activate, CardId.ElementalHEROStratos, ElementalHEROStratosActivate);

            // Create all methods needed for ElementalHEROPrisma to work
            AddExecutor(ExecutorType.Summon, CardId.ElementalHEROPrisma, ElementalHEROPrismaNormalSummon);
            AddExecutor(ExecutorType.MonsterSet, CardId.ElementalHEROPrisma, ElementalHEROPrismaMonsterSet);
            AddExecutor(ExecutorType.Repos, CardId.ElementalHEROPrisma, ElementalHEROPrismaRepos);
            AddExecutor(ExecutorType.Activate, CardId.ElementalHEROPrisma, ElementalHEROPrismaActivate);

            // Create all methods needed for ElementalHEROBlazeman to work
            AddExecutor(ExecutorType.Summon, CardId.ElementalHEROBlazeman, ElementalHEROBlazemanNormalSummon);
            AddExecutor(ExecutorType.MonsterSet, CardId.ElementalHEROBlazeman, ElementalHEROBlazemanMonsterSet);
            AddExecutor(ExecutorType.Repos, CardId.ElementalHEROBlazeman, ElementalHEROBlazemanRepos);
            AddExecutor(ExecutorType.Activate, CardId.ElementalHEROBlazeman, ElementalHEROBlazemanActivate);

            // Create all methods needed for NeoSpaceConnector to work
            AddExecutor(ExecutorType.Summon, CardId.NeoSpaceConnector, NeoSpaceConnectorNormalSummon);
            AddExecutor(ExecutorType.MonsterSet, CardId.NeoSpaceConnector, NeoSpaceConnectorMonsterSet);
            AddExecutor(ExecutorType.Repos, CardId.NeoSpaceConnector, NeoSpaceConnectorRepos);
            AddExecutor(ExecutorType.Activate, CardId.NeoSpaceConnector, NeoSpaceConnectorActivate);

            // Create all methods needed for NeoSpacienGrandMole to work
            AddExecutor(ExecutorType.Summon, CardId.NeoSpacienGrandMole, NeoSpacienGrandMoleNormalSummon);
            AddExecutor(ExecutorType.MonsterSet, CardId.NeoSpacienGrandMole, NeoSpacienGrandMoleMonsterSet);
            AddExecutor(ExecutorType.Repos, CardId.NeoSpacienGrandMole, NeoSpacienGrandMoleRepos);
            AddExecutor(ExecutorType.Activate, CardId.NeoSpacienGrandMole, NeoSpacienGrandMoleActivate);

            // Create all methods needed for NeoSpacianAirHummingbird to work
            AddExecutor(ExecutorType.Summon, CardId.NeoSpacianAirHummingbird, NeoSpacianAirHummingbirdNormalSummon);
            AddExecutor(ExecutorType.MonsterSet, CardId.NeoSpacianAirHummingbird, NeoSpacianAirHummingbirdMonsterSet);
            AddExecutor(ExecutorType.Repos, CardId.NeoSpacianAirHummingbird, NeoSpacianAirHummingbirdRepos);
            AddExecutor(ExecutorType.Activate, CardId.NeoSpacianAirHummingbird, NeoSpacianAirHummingbirdActivate);

            // Create all methods needed for NeoSpacianAquaDolphin to work
            AddExecutor(ExecutorType.Summon, CardId.NeoSpacianAquaDolphin, NeoSpacianAquaDolphinNormalSummon);
            AddExecutor(ExecutorType.MonsterSet, CardId.NeoSpacianAquaDolphin, NeoSpacianAquaDolphinMonsterSet);
            AddExecutor(ExecutorType.Repos, CardId.NeoSpacianAquaDolphin, NeoSpacianAquaDolphinRepos);
            AddExecutor(ExecutorType.Activate, CardId.NeoSpacianAquaDolphin, NeoSpacianAquaDolphinActivate);

            // Create all methods needed for NeoSpacianFlareScarab to work
            AddExecutor(ExecutorType.Summon, CardId.NeoSpacianFlareScarab, NeoSpacianFlareScarabNormalSummon);
            AddExecutor(ExecutorType.MonsterSet, CardId.NeoSpacianFlareScarab, NeoSpacianFlareScarabMonsterSet);
            AddExecutor(ExecutorType.Repos, CardId.NeoSpacianFlareScarab, NeoSpacianFlareScarabRepos);
            AddExecutor(ExecutorType.Activate, CardId.NeoSpacianFlareScarab, NeoSpacianFlareScarabActivate);
            //  Add Executors to all special summonable effect monsters
            //  Add Executors to all summonable effect monsters

            // Create all methods needed for ElementalHEROStormNeos to work
            AddExecutor(ExecutorType.Repos, CardId.ElementalHEROStormNeos, ElementalHEROStormNeosRepos);
            AddExecutor(ExecutorType.Activate, CardId.ElementalHEROStormNeos, ElementalHEROStormNeosActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.ElementalHEROStormNeos, ElementalHEROStormNeosSpSummon);

            // Create all methods needed for ElementalHeroMagmaNeos to work
            AddExecutor(ExecutorType.Repos, CardId.ElementalHeroMagmaNeos, ElementalHeroMagmaNeosRepos);
            AddExecutor(ExecutorType.Activate, CardId.ElementalHeroMagmaNeos, ElementalHeroMagmaNeosActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.ElementalHeroMagmaNeos, ElementalHeroMagmaNeosSpSummon);

            // Create all methods needed for ElementalHeroAirNeos to work
            AddExecutor(ExecutorType.Repos, CardId.ElementalHeroAirNeos, ElementalHeroAirNeosRepos);
            AddExecutor(ExecutorType.Activate, CardId.ElementalHeroAirNeos, ElementalHeroAirNeosActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.ElementalHeroAirNeos, ElementalHeroAirNeosSpSummon);

            // Create all methods needed for ElementalHeroGrandNeos to work
            AddExecutor(ExecutorType.Repos, CardId.ElementalHeroGrandNeos, ElementalHeroGrandNeosRepos);
            AddExecutor(ExecutorType.Activate, CardId.ElementalHeroGrandNeos, ElementalHeroGrandNeosActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.ElementalHeroGrandNeos, ElementalHeroGrandNeosSpSummon);

            // Create all methods needed for ElementalHeroAquaNeos to work
            AddExecutor(ExecutorType.Repos, CardId.ElementalHeroAquaNeos, ElementalHeroAquaNeosRepos);
            AddExecutor(ExecutorType.Activate, CardId.ElementalHeroAquaNeos, ElementalHeroAquaNeosActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.ElementalHeroAquaNeos, ElementalHeroAquaNeosSpSummon);

            // Create all methods needed for ElementalHeroBraveNeos to work
            AddExecutor(ExecutorType.Repos, CardId.ElementalHeroBraveNeos, ElementalHeroBraveNeosRepos);
            AddExecutor(ExecutorType.Activate, CardId.ElementalHeroBraveNeos, ElementalHeroBraveNeosActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.ElementalHeroBraveNeos, ElementalHeroBraveNeosSpSummon);

            // Create all methods needed for ElementalHeroFlareNeos to work
            AddExecutor(ExecutorType.Repos, CardId.ElementalHeroFlareNeos, ElementalHeroFlareNeosRepos);
            AddExecutor(ExecutorType.Activate, CardId.ElementalHeroFlareNeos, ElementalHeroFlareNeosActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.ElementalHeroFlareNeos, ElementalHeroFlareNeosSpSummon);

            // Create all methods needed for ElementalHeroSunrise to work
            AddExecutor(ExecutorType.Repos, CardId.ElementalHeroSunrise, ElementalHeroSunriseRepos);
            AddExecutor(ExecutorType.Activate, CardId.ElementalHeroSunrise, ElementalHeroSunriseActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.ElementalHeroSunrise, ElementalHeroSunriseSpSummon);

            // Create all methods needed for ElementalHeroNeosKnight to work
            AddExecutor(ExecutorType.Repos, CardId.ElementalHeroNeosKnight, ElementalHeroNeosKnightRepos);
            AddExecutor(ExecutorType.Activate, CardId.ElementalHeroNeosKnight, ElementalHeroNeosKnightActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.ElementalHeroNeosKnight, ElementalHeroNeosKnightSpSummon);
            //  Add Executors to all spell and trap cards

            // Create all methods needed for MonsterReborn to work
            AddExecutor(ExecutorType.SpellSet, CardId.MonsterReborn, MonsterRebornSpellSet);
            AddExecutor(ExecutorType.Activate, CardId.MonsterReborn, MonsterRebornActivate);

            // Create all methods needed for NeosFusion to work
            AddExecutor(ExecutorType.SpellSet, CardId.NeosFusion, NeosFusionSpellSet);
            AddExecutor(ExecutorType.Activate, CardId.NeosFusion, NeosFusionActivate);

            // Create all methods needed for Polymerization to work
            AddExecutor(ExecutorType.SpellSet, CardId.Polymerization, PolymerizationSpellSet);
            AddExecutor(ExecutorType.Activate, CardId.Polymerization, PolymerizationActivate);

            // Create all methods needed for MiracleContact to work
            AddExecutor(ExecutorType.SpellSet, CardId.MiracleContact, MiracleContactSpellSet);
            AddExecutor(ExecutorType.Activate, CardId.MiracleContact, MiracleContactActivate);

            // Create all methods needed for InstantNeoSpace to work
            AddExecutor(ExecutorType.SpellSet, CardId.InstantNeoSpace, InstantNeoSpaceSpellSet);
            AddExecutor(ExecutorType.Activate, CardId.InstantNeoSpace, InstantNeoSpaceActivate);

            // Create all methods needed for NeoSpace to work
            AddExecutor(ExecutorType.SpellSet, CardId.NeoSpace, NeoSpaceSpellSet);
            AddExecutor(ExecutorType.Activate, CardId.NeoSpace, NeoSpaceActivate);

            // Create all methods needed for DrowningMirrorForce to work
            AddExecutor(ExecutorType.SpellSet, CardId.DrowningMirrorForce, DrowningMirrorForceSpellSet);
            AddExecutor(ExecutorType.Activate, CardId.DrowningMirrorForce, DrowningMirrorForceActivate);

            // Create all methods needed for NEXT to work
            AddExecutor(ExecutorType.SpellSet, CardId.NEXT, NEXTSpellSet);
            AddExecutor(ExecutorType.Activate, CardId.NEXT, NEXTActivate);

            // Create all methods needed for CallOfTheHaunted to work
            AddExecutor(ExecutorType.SpellSet, CardId.CallOfTheHaunted, CallOfTheHauntedSpellSet);
            AddExecutor(ExecutorType.Activate, CardId.CallOfTheHaunted, CallOfTheHauntedActivate);

         }

            // All Normal Monster Methods

            // Create all methods needed for ElementalHERONeos to work

        private bool ElementalHERONeosNormalSummon()
        {

            return true;
        }

        private bool ElementalHERONeosMonsterSet()
        {

            return true;
        }

        private bool ElementalHERONeosRepos()
        {

            return DefaultMonsterRepos;
        }

            // All Effect Monster Methods

            // Create all methods needed for ElementalHEROHonestNeos to work

        private bool ElementalHEROHonestNeosNormalSummon()
        {

            return true;
        }

        private bool ElementalHEROHonestNeosMonsterSet()
        {

            return true;
        }

        private bool ElementalHEROHonestNeosRepos()
        {

            return DefaultMonsterRepos;
        }

        private bool ElementalHEROHonestNeosActivate()
        {

            return true;
        }

            // Create all methods needed for ElementalHERONeosAlius to work

        private bool ElementalHERONeosAliusNormalSummon()
        {

            return true;
        }

        private bool ElementalHERONeosAliusMonsterSet()
        {

            return true;
        }

        private bool ElementalHERONeosAliusRepos()
        {

            return DefaultMonsterRepos;
        }

        private bool ElementalHERONeosAliusActivate()
        {

            return true;
        }

            // Create all methods needed for NeoSpacePathFinder to work

        private bool NeoSpacePathFinderNormalSummon()
        {

            return true;
        }

        private bool NeoSpacePathFinderMonsterSet()
        {

            return true;
        }

        private bool NeoSpacePathFinderRepos()
        {

            return DefaultMonsterRepos;
        }

        private bool NeoSpacePathFinderActivate()
        {

            return true;
        }

            // Create all methods needed for ElementalHEROStratos to work

        private bool ElementalHEROStratosNormalSummon()
        {

            return true;
        }

        private bool ElementalHEROStratosMonsterSet()
        {

            return true;
        }

        private bool ElementalHEROStratosRepos()
        {

            return DefaultMonsterRepos;
        }

        private bool ElementalHEROStratosActivate()
        {

            return true;
        }

            // Create all methods needed for ElementalHEROPrisma to work

        private bool ElementalHEROPrismaNormalSummon()
        {

            return true;
        }

        private bool ElementalHEROPrismaMonsterSet()
        {

            return true;
        }

        private bool ElementalHEROPrismaRepos()
        {

            return DefaultMonsterRepos;
        }

        private bool ElementalHEROPrismaActivate()
        {

            return true;
        }

            // Create all methods needed for ElementalHEROBlazeman to work

        private bool ElementalHEROBlazemanNormalSummon()
        {

            return true;
        }

        private bool ElementalHEROBlazemanMonsterSet()
        {

            return true;
        }

        private bool ElementalHEROBlazemanRepos()
        {

            return DefaultMonsterRepos;
        }

        private bool ElementalHEROBlazemanActivate()
        {

            return true;
        }

            // Create all methods needed for NeoSpaceConnector to work

        private bool NeoSpaceConnectorNormalSummon()
        {

            return true;
        }

        private bool NeoSpaceConnectorMonsterSet()
        {

            return true;
        }

        private bool NeoSpaceConnectorRepos()
        {

            return DefaultMonsterRepos;
        }

        private bool NeoSpaceConnectorActivate()
        {

            return true;
        }

            // Create all methods needed for NeoSpacienGrandMole to work

        private bool NeoSpacienGrandMoleNormalSummon()
        {

            return true;
        }

        private bool NeoSpacienGrandMoleMonsterSet()
        {

            return true;
        }

        private bool NeoSpacienGrandMoleRepos()
        {

            return DefaultMonsterRepos;
        }

        private bool NeoSpacienGrandMoleActivate()
        {

            return true;
        }

            // Create all methods needed for NeoSpacianAirHummingbird to work

        private bool NeoSpacianAirHummingbirdNormalSummon()
        {

            return true;
        }

        private bool NeoSpacianAirHummingbirdMonsterSet()
        {

            return true;
        }

        private bool NeoSpacianAirHummingbirdRepos()
        {

            return DefaultMonsterRepos;
        }

        private bool NeoSpacianAirHummingbirdActivate()
        {

            return true;
        }

            // Create all methods needed for NeoSpacianAquaDolphin to work

        private bool NeoSpacianAquaDolphinNormalSummon()
        {

            return true;
        }

        private bool NeoSpacianAquaDolphinMonsterSet()
        {

            return true;
        }

        private bool NeoSpacianAquaDolphinRepos()
        {

            return DefaultMonsterRepos;
        }

        private bool NeoSpacianAquaDolphinActivate()
        {

            return true;
        }

            // Create all methods needed for NeoSpacianFlareScarab to work

        private bool NeoSpacianFlareScarabNormalSummon()
        {

            return true;
        }

        private bool NeoSpacianFlareScarabMonsterSet()
        {

            return true;
        }

        private bool NeoSpacianFlareScarabRepos()
        {

            return DefaultMonsterRepos;
        }

        private bool NeoSpacianFlareScarabActivate()
        {

            return true;
        }

            // All Special Summonable Effect Monster Methods

            // All Pure Special Summonable Effect Monster Methods

            // Create all methods needed for ElementalHEROStormNeos to work

        private bool ElementalHEROStormNeosRepos()
        {

            return DefaultMonsterRepos;
        }

        private bool ElementalHEROStormNeosActivate()
        {

            return true;
        }

        private bool ElementalHEROStormNeosSpSummon()
        {

            return true;
        }

            // Create all methods needed for ElementalHeroMagmaNeos to work

        private bool ElementalHeroMagmaNeosRepos()
        {

            return DefaultMonsterRepos;
        }

        private bool ElementalHeroMagmaNeosActivate()
        {

            return true;
        }

        private bool ElementalHeroMagmaNeosSpSummon()
        {

            return true;
        }

            // Create all methods needed for ElementalHeroAirNeos to work

        private bool ElementalHeroAirNeosRepos()
        {

            return DefaultMonsterRepos;
        }

        private bool ElementalHeroAirNeosActivate()
        {

            return true;
        }

        private bool ElementalHeroAirNeosSpSummon()
        {

            return true;
        }

            // Create all methods needed for ElementalHeroGrandNeos to work

        private bool ElementalHeroGrandNeosRepos()
        {

            return DefaultMonsterRepos;
        }

        private bool ElementalHeroGrandNeosActivate()
        {

            return true;
        }

        private bool ElementalHeroGrandNeosSpSummon()
        {

            return true;
        }

            // Create all methods needed for ElementalHeroAquaNeos to work

        private bool ElementalHeroAquaNeosRepos()
        {

            return DefaultMonsterRepos;
        }

        private bool ElementalHeroAquaNeosActivate()
        {

            return true;
        }

        private bool ElementalHeroAquaNeosSpSummon()
        {

            return true;
        }

            // Create all methods needed for ElementalHeroBraveNeos to work

        private bool ElementalHeroBraveNeosRepos()
        {

            return DefaultMonsterRepos;
        }

        private bool ElementalHeroBraveNeosActivate()
        {

            return true;
        }

        private bool ElementalHeroBraveNeosSpSummon()
        {

            return true;
        }

            // Create all methods needed for ElementalHeroFlareNeos to work

        private bool ElementalHeroFlareNeosRepos()
        {

            return DefaultMonsterRepos;
        }

        private bool ElementalHeroFlareNeosActivate()
        {

            return true;
        }

        private bool ElementalHeroFlareNeosSpSummon()
        {

            return true;
        }

            // Create all methods needed for ElementalHeroSunrise to work

        private bool ElementalHeroSunriseRepos()
        {

            return DefaultMonsterRepos;
        }

        private bool ElementalHeroSunriseActivate()
        {

            return true;
        }

        private bool ElementalHeroSunriseSpSummon()
        {

            return true;
        }

            // Create all methods needed for ElementalHeroNeosKnight to work

        private bool ElementalHeroNeosKnightRepos()
        {

            return DefaultMonsterRepos;
        }

        private bool ElementalHeroNeosKnightActivate()
        {

            return true;
        }

        private bool ElementalHeroNeosKnightSpSummon()
        {

            return true;
        }

            // All Spell and Trap Card Methods

            // Create all methods needed for MonsterReborn to work

        private bool MonsterRebornSpellSet()
        {

            return DefaultSpellSet;
        }

        private bool MonsterRebornActivate()
        {

            return true;
        }

            // Create all methods needed for NeosFusion to work

        private bool NeosFusionSpellSet()
        {

            return DefaultSpellSet;
        }

        private bool NeosFusionActivate()
        {

            return true;
        }

            // Create all methods needed for Polymerization to work

        private bool PolymerizationSpellSet()
        {

            return DefaultSpellSet;
        }

        private bool PolymerizationActivate()
        {

            return true;
        }

            // Create all methods needed for MiracleContact to work

        private bool MiracleContactSpellSet()
        {

            return DefaultSpellSet;
        }

        private bool MiracleContactActivate()
        {

            return true;
        }

            // Create all methods needed for InstantNeoSpace to work

        private bool InstantNeoSpaceSpellSet()
        {

            return DefaultSpellSet;
        }

        private bool InstantNeoSpaceActivate()
        {

            return true;
        }

            // Create all methods needed for NeoSpace to work

        private bool NeoSpaceSpellSet()
        {

            return DefaultSpellSet;
        }

        private bool NeoSpaceActivate()
        {

            return true;
        }

            // Create all methods needed for DrowningMirrorForce to work

        private bool DrowningMirrorForceSpellSet()
        {

            return DefaultSpellSet;
        }

        private bool DrowningMirrorForceActivate()
        {

            return true;
        }

            // Create all methods needed for NEXT to work

        private bool NEXTSpellSet()
        {

            return DefaultSpellSet;
        }

        private bool NEXTActivate()
        {

            return true;
        }

            // Create all methods needed for CallOfTheHaunted to work

        private bool CallOfTheHauntedSpellSet()
        {

            return DefaultSpellSet;
        }

        private bool CallOfTheHauntedActivate()
        {

            return true;
        }

    }
}