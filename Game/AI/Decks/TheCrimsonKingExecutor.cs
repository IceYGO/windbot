// dotnet build WindBot.csproj -c Debug
// WindBot.exe Deck=TheCrimsonKing Name=The_Crimson_King Dialog=thecrimsonking.en Mode=fast

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using YGOSharp.OCGWrapper.Enums;

namespace WindBot.Game.AI.Decks
{
    // =====================================================================================================
    // THE CRIMSON KING
    //
    // Este arquivo tem tudo que o bot usa, em três partes:
    //
    //   PARTE 1 - LISTA E VERIFICAÇÃO DO DECK
    //   PARTE 2 - EXECUÇÃO: lê o duelo, pede um plano e transforma cada passo em respostas para o jogo
    //   PARTE 3 - PLANEJADOR: modelo das cartas + busca da melhor sequência (porta do handbook/planner)
    //
    // Fluxo de um turno nosso:
    //   1. No menu da Main Phase (idle), PrepareIdlePrompt lê o estado real (ReadState).
    //   2. Se o estado é o previsto pelo plano, segue o próximo passo; senão calcula um plano novo.
    //   3. Os executores PlanActivate / PlanSpecialSummon / PlanNormalSummon só aceitam a carta do passo.
    //   4. Os prompts seguintes (escolher carta, opção, zona, materiais) usam as escolhas guardadas no passo.
    //   5. Gatilhos (Soul, Vision, King...) aparecem na chain; DecideTrigger procura a resposta no plano.
    //   6. Quando o plano acaba, as armadilhas são baixadas e o turno termina.
    //
    // Nesta fase o bot só planeja o PRÓPRIO turno. No turno do oponente ele usa apenas as handtraps
    // padrão (Ash, Impermanence, Maxx "C"). Quetzacoatl, Hypernova, Red Zone, Abyss etc. ficam para a fase 2.
    // =====================================================================================================
    [Deck("TheCrimsonKing", "AI_TheCrimsonKing")]
    public class TheCrimsonKingExecutor : DefaultExecutor
    {
        public class CardId
        {
            // ===== Main Deck =====
            // Monstros
            public const int PowerViceDragon = 19434243;                 // DARK Dragon lv5
            public const int FiendPieceGolem = 56838842;                 // DARK Fiend lv5
            public const int EarthboundPrisonerStoneSweeper = 72323266;  // DARK Fiend lv5
            public const int BoneArchfiend = 25784595;                   // DARK Fiend lv4
            public const int DarknessResonator = 83445539;               // DARK Fiend Tuner lv3
            public const int SoulResonator = 62991792;                   // FIRE Fiend Tuner lv3
            public const int CrimsonResonator = 34761841;                // DARK Fiend Tuner lv2
            public const int VisionResonator = 98396890;                 // DARK Fiend Tuner lv2
            public const int ChainResonator = 13764881;                  // LIGHT Fiend Tuner lv1
            public const int SynkronResonator = 77360173;                // DARK Fiend Tuner lv1
            public const int TheBystialLubellion = 32731036;             // LIGHT Dragon lv8
            public const int BystialMagnamhut = 33854624;                // DARK Dragon lv6
            public const int FydraulisHarmonia = 70088809;               // DARK Dragon Tuner lv7
            public const int AshBlossomJoyousSpring = 14558127;          // FIRE Zombie Tuner lv3
            public const int MaxxC = 23434538;                           // EARTH Insect lv2

            // Magias
            public const int FoolishBurial = 81439173;                   // Normal
            public const int CrimsonCall = 99398682;
            public const int ResonatorCall = 23008320;
            public const int CrimsonGaia = 98173209;                     // Contínua

            // Armadilhas
            public const int RedZone = 50056656;                         // Contínua
            public const int EtudeOfTheBranded = 45675980;               // Contínua
            public const int KingsResonance = 17269895;                  // Normal
            public const int RedDragonArchfiendsChain = 92936365;        // Contínua
            public const int InfiniteImpermanence = 10045474;            // Normal
            public const int DominusImpulse = 40366668;                  // Normal

            // ===== Extra Deck (todos DARK Dragon Synchro) =====
            public const int CrimsonDragonQuetzacoatl = 29053657;        // lv12
            public const int RedHypernovaDragon = 30698243;              // lv12
            public const int RedSupernovaDragon = 99585851;              // lv12
            public const int RedNovaDragonBurningSoul = 65541656;        // lv12
            public const int StormBaneDragonDestorbim = 94641726;        // lv11
            public const int BystialDisPater = 27572350;                 // lv10
            public const int HotRedDragonArchfiendAbyss = 9753964;       // lv9
            public const int RedDragonArchfiend = 70902743;              // lv8
            public const int TheCrimsonKing = 67809530;                  // lv8
            public const int ScarredDragonArchfiend = 87451661;          // lv8
            public const int CrimsonBladeDragon = 3294539;               // lv7
            public const int ZalenTheShackledDragon = 4891376;           // lv7 Tuner
            public const int RedRisingDragon = 66141736;                 // lv6

            // ===== Cartas do oponente que mudam as decisões (não estão no nosso deck)
            public const int DrollLockBird = 94145021;        // depois da 1ª adição do Deck: ninguém adiciona do Deck
            public const int MulcharmyFuwalos = 42141493;     // compra a cada Invocação-Especial do Deck/Extra
            public const int NibiruThePrimalBeing = 27204311; // 5+ invocações: libera todos os monstros do campo
        }

        // PURPOSE: quantidade de cópias de cada carta na lista atual (AI_TheCrimsonKing.ydk).
        // TRUSTS: precisa ser atualizada junto com o .ydk; ValidateDeckList avisa quando divergir.
        // RESET: nenhum (dados fixos).
        private static readonly Dictionary<int, int> MainDeckList = new Dictionary<int, int>
        {
            { CardId.PowerViceDragon, 3 },
            { CardId.FiendPieceGolem, 1 },
            { CardId.EarthboundPrisonerStoneSweeper, 1 },
            { CardId.BoneArchfiend, 3 },
            { CardId.DarknessResonator, 1 },
            { CardId.SoulResonator, 3 },
            { CardId.CrimsonResonator, 1 },
            { CardId.VisionResonator, 2 },
            { CardId.ChainResonator, 1 },
            { CardId.SynkronResonator, 1 },
            { CardId.FoolishBurial, 1 },
            { CardId.CrimsonCall, 1 },
            { CardId.ResonatorCall, 1 },
            { CardId.CrimsonGaia, 3 },
            { CardId.RedZone, 1 },
            { CardId.EtudeOfTheBranded, 1 },
            { CardId.KingsResonance, 1 },
            { CardId.RedDragonArchfiendsChain, 1 },
            { CardId.TheBystialLubellion, 1 },
            { CardId.BystialMagnamhut, 1 },
            { CardId.FydraulisHarmonia, 3 },
            { CardId.AshBlossomJoyousSpring, 2 },
            { CardId.MaxxC, 1 },
            { CardId.InfiniteImpermanence, 3 },
            { CardId.DominusImpulse, 2 },
        };

        private static readonly Dictionary<int, int> ExtraDeckList = new Dictionary<int, int>
        {
            { CardId.CrimsonDragonQuetzacoatl, 1 },
            { CardId.RedHypernovaDragon, 1 },
            { CardId.RedSupernovaDragon, 1 },
            { CardId.RedNovaDragonBurningSoul, 1 },
            { CardId.StormBaneDragonDestorbim, 1 },
            { CardId.BystialDisPater, 1 },
            { CardId.HotRedDragonArchfiendAbyss, 1 },
            { CardId.RedDragonArchfiend, 2 },
            { CardId.TheCrimsonKing, 1 },
            { CardId.ScarredDragonArchfiend, 1 },
            { CardId.CrimsonBladeDragon, 1 },
            { CardId.ZalenTheShackledDragon, 1 },
            { CardId.RedRisingDragon, 2 },
        };

        // ---- Configuração da busca do plano (validada em handbook/planner/experiments, baterias 1-11)
        // Primeiro plano do turno: carteira de 3 buscas em paralelo; fica a melhor mesa (camada, depois nota de seleção).
        // Nas 25 mãos de teste essa carteira chegou à mesa ideal com Hypernova (ou à mesa do jogador) em 22 mãos,
        // o mesmo teto de todas as buscas somadas. Tempo estimado no C#: 6-7 s com as 3 buscas em paralelo.
        //   base_w1500         busca simples (sem livro): cobre mãos em que o livro puxa para a rota errada
        //   livro_w3000        diversidade por rota + livro de rotas, largura 3000
        //   livro_w3000_protegida  igual, com ruído e sem aceitar Hypernova/Supernova sem proteção no campo
        //                          (teste do jogador: o bot fez Hypernova logo de cara, sem Zalen/Abyss/Dis Pater/King)
        //   protegida_s1/s2/s5     três sementes de ruído: o Hypernova protegido existe em mãos difíceis, mas cada
        //                          semente só acha em algumas (experimento: s1 → mão 17, s2 → 16 e 18, s5 → 17 e 18)
        // Carteira do primeiro plano do turno. Com 5 buscas em paralelo todas batiam no limite de 25 s e as mesas
        // pioravam (teste 5, 2026-09-14), então só as PortfolioCount primeiras rodam. A semente 4 protegida achou o
        // Hypernova protegido na mão do teste 5 (Bone, Soul, Foolish, Darkness, Vision) que as outras não acharam.
        private static readonly SearchOptions[] PortfolioSearches =
        {
            new SearchOptions { Name = "book_w3000", Width = 3000, PerRoute = 80, BookWeight = 15, CollectSeeds = false }, // sementes: testadas na mão do Albaz 23:19 sem ganho (desligadas)
            new SearchOptions { Name = "book_w3000_protected_s1", Width = 3000, PerRoute = 80, BookWeight = 15, Noise = 10, Seed = 1, ForbidUnprotectedNova = true },
            new SearchOptions { Name = "book_w3000_protected_s4", Width = 3000, PerRoute = 80, BookWeight = 15, Noise = 10, Seed = 4, ForbidUnprotectedNova = true },
            new SearchOptions { Name = "base_w1500", Width = 1500 },
        };
        private static int PortfolioCount = 3; // static para teste offline por reflexão (3 = s4 no lugar da base, 4 = todas)
        // Replanejamentos no meio do combo (estado adiantado, busca curta). Teste do jogador: depois de uma interrupção
        // (Dominus Impulse na Magnamhut) o replanejamento fez Hypernova sem proteção. Por isso também há uma busca protegida.
        private static readonly SearchOptions ReplanSearch =
            // Largura 1000 (medido em 13 negações de passo crítico, mãos Soul e PV): 25-30% mais rápido que 1500,
            // nenhuma camada perdida e nota média -1. Largura 700 perdia camada.
            new SearchOptions { Name = "book_w1000", Width = 1000, PerRoute = 30, BookWeight = 15, TimeLimitMs = 8000 };
        private static readonly SearchOptions[] ReplanSearches =
        {
            ReplanSearch,
            new SearchOptions { Name = "book_w1000_protected", Width = 1000, PerRoute = 30, BookWeight = 15, TimeLimitMs = 8000, ForbidUnprotectedNova = true },
        };
        // Proteção contra laço: se o duelo diverge do modelo muitas vezes, o bot para de planejar no turno.
        private const int MaxReplansPerTurn = 25;

        // ---- Opções de efeito confirmadas em cards.cdb (str1..str16 = índice 0..15)
        private const int BoneOptionIncrease = 1;          // "Increase Level"
        private const int BoneOptionDecrease = 2;          // "Decrease Level"
        private const int FiendPieceOptionReduce1 = 3;     // "Decrease by 1 Level"
        private const int FiendPieceOptionReduce2 = 4;     // "Decrease by 2 Levels"
        private const int DarknessOptionExtraSummon = 3;   // "Summoned by the effect of Darkness Resonator"
        // Textos do sistema usados por aux.ToHandOrElse (adicionar à mão / Invocar por Invocação-Especial).
        private const int SystemStringAddToHand = 1190;
        private const int SystemStringSpecialSummon = 1152;
        private const int SystemStringSynchroSummon = 1164;   // "Invocação-Sincro" (escolha de procedimento)

        // Armadilhas que o bot baixa depois do combo (as de interação ficam para a fase 2).
        private static readonly int[] TrapsToSet =
        {
            CardId.RedZone, CardId.KingsResonance, CardId.RedDragonArchfiendsChain,
            CardId.InfiniteImpermanence, CardId.DominusImpulse, CardId.EtudeOfTheBranded
        };

        // Efeitos do modelo que NÃO são "uma vez por turno" (não entram em _usedThisTurn).
        private static readonly HashSet<RdaKey> NotOncePerTurn = new HashSet<RdaKey>
        {
            RdaKey.None, RdaKey.ChainSummon, RdaKey.SynkronAdd, RdaKey.RedRisingRevive
        };

        // Em que tipo de prompt o bot está. Os mesmos executores servem para o menu (idle) e para a chain.
        private enum PromptKind { None, Idle, Chain }

        // Reset Matrix
        // Per-turn reset (OnNewTurn):
        //   - _usedThisTurn, _normalSummonUsed, _extraNormalSummonAvailable, _noSpecialSummon, _turnFlags
        //   - _plan, _nextStep, _doneSteps, _replansThisTurn, _plannerDisabledThisTurn, _comboDone
        //   - _actionBySource, _remainingPicks, _currentAction, _idleStep
        // Per-chain reset (OnChainEnd):
        //   - _declinedSources
        // Per-phase reset (OnNewPhase):
        //   - _prompt
        // Persistente:
        //   - _deckListValidated (uma vez por duelo)
        private bool _deckListValidated;
        // "durante um Duelo em que você Invocou por Sincronia Red Dragon Archfiend" (condição do Burning Soul): vale o duelo todo.
        private bool _rdaSynchroThisDuel;
        // Burning Soul: o efeito de adicionar é "uma vez por Duelo". Se ele voltar ao Extra (custo da Quetzacoatl) e for invocado
        // de novo em outro turno, o efeito não existe mais.
        private bool _burningSoulAddUsedThisDuel;

        // ---- O que já aconteceu no turno (o jogo não informa diretamente, então o executor anota)
        private readonly HashSet<RdaKey> _usedThisTurn = new HashSet<RdaKey>();
        private bool _normalSummonUsed;
        private bool _extraNormalSummonAvailable;   // concedida pelo gatilho do Darkness Resonator
        private bool _noSpecialSummon;              // trava da Crimson Dragon Quetzacoatl, ou o jogo não oferecer nenhuma
        private int _specialSummonBlockedCount;     // vezes que um passo de Invocação-Especial não foi oferecido neste turno
        private int _turnFlags;                     // RdaState.FlagMagnamhutEndPhase / FlagRdaSynchro

        // ---- Plano atual
        private RdaPlan _plan;
        private int _nextStep;                                   // índice do próximo passo ainda não feito
        private readonly HashSet<int> _doneSteps = new HashSet<int>();  // gatilhos respondidos (podem vir fora de ordem)
        private PlanStep _idleStep;                              // passo liberado para o menu atual
        private int _idleStepIndex;
        private int _replansThisTurn;
        private bool _plannerDisabledThisTurn;
        private bool _comboDone;                                 // plano terminou: não replaneja mais no turno
        private bool _portfolioUsedThisTurn;                     // a carteira (3 buscas) roda só no primeiro plano

        // ---- Execução do passo
        private PromptKind _prompt = PromptKind.None;
        private PlanAction _currentAction;                                     // última ação iniciada
        private readonly Dictionary<ClientCard, PlanAction> _actionBySource = new Dictionary<ClientCard, PlanAction>();
        private readonly Dictionary<PlanAction, List<RdaPick>> _remainingPicks = new Dictionary<PlanAction, List<RdaPick>>();
        private readonly HashSet<ClientCard> _declinedSources = new HashSet<ClientCard>();
        // Cartas oferecidas na janela de chain atual (para ordenar gatilhos simultâneos).
        private IList<ClientCard> _chainCandidates = new List<ClientCard>();
        // Monstros que entraram no nosso campo neste turno (risco de Nibiru a partir do 5º).
        private int _summonedThisTurn;
        // Isca resolveu sem o oponente responder: segue a rota mais segura sem gastar outra isca (regra do jogador).
        private bool _baitResolvedThisTurn;
        // Um passo do combo que não é isca já resolveu: daqui em diante nenhuma ativação conta como isca.
        private bool _comboStartedThisTurn;
        // Efeitos críticos nossos que resolveram sem o oponente negar neste turno: cada um reduz a chance de negação
        // usada na escolha por resistência (o oponente teve a chance de responder e deixou passar).
        private int _unansweredCriticalsThisTurn;
        // Ações que o menu não ofereceu, válidas enquanto o estado real não muda (por texto e pelo efeito inteiro).
        private readonly HashSet<string> _blockedActions = new HashSet<string>();
        private RdaState _blockedState;

        // ---- Respostas rápidas a efeitos do oponente (Abyss, Dis Pater, Zalen, Quetzacoatl, King ②, Red Zone)
        private enum ResponseKind { None, Abyss, DisPaterNegate, DisPaterDestroy, ZalenFirst, ZalenSecond, Quetzacoatl, King, RedZone, Dominus, Harmonia }
        private ResponseKind _responseKind = ResponseKind.None;
        private ClientCard _responseSource;   // nossa carta que respondeu
        private ClientCard _responseTarget;   // carta do oponente que motivou a resposta
        private bool _responseFresh;          // ativada agora: alvos/custos ainda podem ser pedidos antes do elo aparecer
        private int _opponentActivationsThisTurn; // ativações do oponente no turno dele (sinal de combo para a Harmonia)
        private bool _novaBanishThisTurn;     // já usamos o banimento de um nova neste turno (nosso turno: limpar antes da batalha)
        private bool _novaBanishInWindow;     // um nova já foi ativado nesta janela de chain (nunca os dois juntos)
        private int _opponentSummonsSinceNova;      // monstros que o oponente colocou no campo desde o último banimento do nova
        private int _opponentExtraSummonsSinceNova; // desses, os que vieram do Extra Deck
        private readonly HashSet<int> _negateUsedThisTurn = new HashSet<int>();  // cartas nossas que já gastaram a negação neste turno
        private bool _crimsonCallChainAttack;         // Crimson Call deu ataque extra ao RDA e ele ainda não foi usado
        private bool _harmoniaUsedThisTurn;   // Fydraulis Harmonia já ativada neste turno (1 por turno)
        private readonly HashSet<ClientCard> _extraToGraveUnsummoned = new HashSet<ClientCard>(); // foi do Extra ao GY sem ser invocado
        private readonly HashSet<ClientCard> _kingRdaCards = new HashSet<ClientCard>();          // RDA que veio do efeito do Crimson King
        private readonly HashSet<ClientCard> _gaiaRdaCards = new HashSet<ClientCard>();          // RDA que veio do efeito da Crimson Gaia
        private readonly Dictionary<ClientCard, int> _enemyEntryOrder = new Dictionary<ClientCard, int>(); // ordem de entrada no campo
        private int _enemyEntryCounter;
        private MainPhase _preparedMenu;      // menu da Main Phase já preparado (IdleGuard / PrepareIdlePrompt)
        private readonly Dictionary<ClientCard, int> _knownEnemyIds = new Dictionary<ClientCard, int>(); // id visto antes de virar
        private readonly HashSet<int> _deckDriftReported = new HashSet<int>();   // divergências de deck já avisadas
        private readonly HashSet<int> _profiledCardIds = new HashSet<int>();      // cartas já perfiladas no log
        private readonly HashSet<int> _reportedEnemyHandIds = new HashSet<int>();   // cartas da mão dele já avisadas no log
        private ClientCard _novaReviveSource; // Dis Pater/Red Zone trazendo o nova banido: escolhe o nova no prompt
        // ---- Efeitos fora do plano (auditoria carta por carta, 2026-09-14)
        private int _opponentSpecialSummonsThisTurn;  // monstros que o oponente invocou por Invocação-Especial (King's Resonance)
        private ClientCard _bestReviveSource;         // King's Resonance / Storm-Bane ②: escolhe o melhor monstro para voltar
        private bool _rdaChainActivating;             // RDA's Chain ativando: revelar e escolher os alvos
        private List<ClientCard> _rdaChainTargets = new List<ClientCard>();
        private bool _rdaChainBeforeComboDone;        // RDA's Chain antes do combo já foi resolvida (ou não dá) neste turno
        private int _rdaChainBeforeComboChecks;       // menus esperando a RDA's Chain antes de planejar (limite contra laço)
        private bool _stormBaneBanishing;             // Storm-Bane ① ativando: custo do GY e cartas do oponente
        private int _stormBaneCount;
        private List<ClientCard> _stormBaneTargets = new List<ClientCard>();
        private bool _abyssRevive;                    // Abyss ② revivendo um Tuner
        private int _abyssReviveCode;                 // Tuner escolhido pela simulação da Main Phase 2
        private ClientCard _magnamhutTarget;          // Magnamhut em resposta: monstro do GY que ela bane
        private bool _magnamhutEndPhaseSearch;        // Magnamhut invocada no turno do oponente: busca na End Phase
        private int _redRisingToGraveThisTurn;        // Red Rising que foi ao GY neste turno (efeito do GY só no turno seguinte)
        private int _etudeTarget;                     // Etude of the Branded: Synchro escolhido

        // =====================================================================================================
        // MODO DE DECISÃO — escolhido FORA do código, sem recompilar
        //
        // Lido por Config, que aceita três fontes (a primeira que definir vence):
        //   linha de comando   WindBot.exe Deck=TheCrimsonKing Mode=fast
        //   arquivo            WindBot.exe Config=bot.txt   com a linha "Mode=fast" dentro
        //   App.config         <add key="Mode" value="fast"/>
        //
        //   fast (PADRÃO)   tetos menores e menos concorrentes na resistência. É o padrão desde 2026-09-18, por
        //                   escolha do jogador: nas medições abaixo ele corta o tempo à metade ou mais sem derrubar
        //                   nenhuma camada, e o custo aparece só na nota de seleção.
        //   logic           comportamento completo: buscas com Width 1000 e teto de 4000 ms, e a escolha por
        //                   resistência comparando até 4 aberturas. Use quando a mesa importar mais que o tempo.
        //   turbo           mais agressivo que o fast. Só vale quando o tempo importa mais que a mesa.
        //
        // MEDIDO EM 2026-09-18 (handbook/tests/teste_modo_rapido.ps1 e teste_modo_rapido_turno2.ps1).
        // As camadas ficaram intactas nos três modos; o que muda é a nota de seleção.
        //
        //            turno 1 (20 mãos)              turno 2+ (12 raízes reais)
        //            total    maior   nota          total    maior   nota
        //   logic    101 s   10,3 s      -           66 s   10,5 s      -
        //   fast      36 s    3,8 s    -17           26 s    4,5 s    +26
        //   turbo     28 s    2,8 s    -70           19 s    2,9 s    -62
        //
        // O turbo usa Width 300 (e não 250, que também foi medido): com 250 o tempo é praticamente o mesmo e a
        // perda de nota é maior (-83 no turno 1, -89 no turno 2+). Largura 300 é o ponto melhor dos dois.
        //
        // POR QUE ISSO É UM MODO E NÃO O PADRÃO: baixar estes tetos já foi medido em 2026-09-16 e rejeitado como
        // padrão porque custa mesa. Aqui a troca é explícita e reversível por configuração.
        //
        // Os três valores de cada modo estão em ModoRapido/ModoLogico abaixo; os testes offline varrem outros por
        // reflexão chamando RdaPlanner.AplicarModo diretamente.
        // =====================================================================================================
        private static bool _modeApplied;
        private static string _modeName = ModeDefault;
        private const string ModeDefault = "fast";
        // largura das buscas, teto por busca (ms), concorrentes na resistência, largura do plano B, teto do plano B
        private static readonly int[] ModeLogic = { 1000, 4000, 4, 400, 1500 };
        private static readonly int[] ModeFast  = {  400, 1500, 2, 200,  600 };
        private static readonly int[] ModeTurbo = {  300, 1000, 2, 200,  400 };

        private static void ApplyDecisionMode()
        {
            if (_modeApplied) return;
            _modeApplied = true;
            string configured;
            try { configured = Config.GetString("Mode", ModeDefault); }
            catch { configured = ModeDefault; }   // Config ainda não carregado: mantém o padrão
            _modeName = string.IsNullOrEmpty(configured) ? ModeDefault : configured.Trim().ToLowerInvariant();
            bool turbo = _modeName == "turbo";
            // Qualquer valor que não seja "logic" nem "turbo" cai no padrão, que é o fast. Um erro de digitação
            // vira fast em silêncio, por isso o nome é normalizado abaixo: o log mostra o modo APLICADO, não o
            // que foi digitado, senão um "Mode=logci" apareceria como se fosse logic rodando com tetos de fast.
            bool fast = _modeName != "logic" && !turbo;
            _modeName = turbo ? "turbo" : fast ? "fast" : "logic";
            int[] values = turbo ? ModeTurbo : fast ? ModeFast : ModeLogic;
            RdaPlanner.ApplyMode(values[0], values[1], values[2], values[3], values[4]);
            // A carteira pesada quase nunca roda, mas QUANDO roda ela domina: medido em partida, dois planos do modo
            // rápido passaram de 5 s com "buscas" em 4.833 e 5.554 ms, porque nenhuma rota encaixou, o atalho não
            // disparou e o caminho caro (Width 3000, teto 25 s) executou inteiro. O modo rápido também a encolhe.
            if (fast || turbo)
            {
                // Tudo segue o MESMO teto do modo, inclusive a carteira pesada. Dar folga extra a ela tornava a
                // garantia difícil de raciocinar; assim "fast" quer dizer, sem exceção, que nenhuma busca passa do
                // teto do modo. Como a carteira quase nunca roda, o custo dessa uniformidade é baixo.
                foreach (SearchOptions search in PortfolioSearches)
                {
                    if (search == null) continue;
                    search.Width = values[0];
                    search.TimeLimitMs = values[1];
                }
                // O REPLANEJAMENTO usa outro conjunto, com teto de 8000 ms. Sem isto o modo rápido não o alcançava:
                // partida de 2026-09-18 às 05:36 teve um plano de 9.036 ms em modo fast, com "buscas 8.066 ms" —
                // exatamente o teto de 8 s sendo atingido por uma busca que o preset não tocava.
                foreach (SearchOptions search in ReplanSearches)
                {
                    if (search == null) continue;
                    search.Width = values[0];
                    search.TimeLimitMs = values[1];
                }
                // Busca da Main Phase 2 com o Abyss no campo: mesma regra, sem nunca aumentar o que já era menor.
                AbyssReviveSearch.Width = Math.Min(AbyssReviveSearch.Width, values[0]);
                AbyssReviveSearch.TimeLimitMs = Math.Min(AbyssReviveSearch.TimeLimitMs, values[1]);
            }
        }

        public TheCrimsonKingExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            ApplyDecisionMode();
            HookConnectionEnd();
            // A ordem dos AddExecutor é a prioridade.

            // 0) Preparação do menu: roda antes de tudo no idle e sempre retorna false (nunca termina o turno).
            //    GoToEndPhase é o único tipo avaliado antes das ativações sem precisar de carta.
            AddExecutor(ExecutorType.GoToEndPhase, PrepareIdlePrompt);
            // O WindBot só avalia o GoToEndPhase quando o menu permite ir direto para a End Phase. Estes guardas (sempre false)
            // garantem a preparação do plano em qualquer menu da Main Phase, antes de todas as outras regras.
            AddExecutor(ExecutorType.Activate, IdleGuard);
            AddExecutor(ExecutorType.SpSummon, IdleGuard);
            AddExecutor(ExecutorType.Summon, IdleGuard);
            AddExecutor(ExecutorType.MonsterSet, IdleGuard);
            AddExecutor(ExecutorType.SpellSet, IdleGuard);
            AddExecutor(ExecutorType.Repos, IdleGuard);

            // 1) Interação padrão no turno do oponente
            AddExecutor(ExecutorType.Activate, CardId.AshBlossomJoyousSpring, AshResponse);
            AddExecutor(ExecutorType.Activate, CardId.InfiniteImpermanence, ImpermanenceResponse);
            AddExecutor(ExecutorType.Activate, CardId.MaxxC, DefaultMaxxC);

            // 2) Respostas rápidas a efeitos do oponente (antes do plano: numa janela com elo do oponente o plano não age)
            AddExecutor(ExecutorType.Activate, NovaVsWipe);
            AddExecutor(ExecutorType.Activate, NovaVsOpponentPlay);
            AddExecutor(ExecutorType.Activate, QuickResponse);

            // 2b) Efeitos fora do plano (auditoria carta por carta): RDA's Chain (antes do combo ou contra ameaça),
            //     King's Resonance, Storm-Bane, Scarred e Crimson Gaia no turno do oponente, gatilhos de batalha.
            AddExecutor(ExecutorType.SpellSet, RdaChainSetBeforeCombo);
            AddExecutor(ExecutorType.Activate, RdaChainActivate);
            AddExecutor(ExecutorType.Activate, KingsResonanceActivate);
            AddExecutor(ExecutorType.Activate, StormBaneBanish);
            AddExecutor(ExecutorType.Activate, StormBaneRevive);
            AddExecutor(ExecutorType.Activate, ScarredOpponentTurn);
            AddExecutor(ExecutorType.Activate, GaiaReviveOpponentTurn);
            AddExecutor(ExecutorType.Activate, BattleTriggers);
            AddExecutor(ExecutorType.Activate, MagnamhutResponse);
            AddExecutor(ExecutorType.Activate, EtudeResponse);

            // 3) Execução do plano (combo do próprio turno + respostas aos gatilhos)
            AddExecutor(ExecutorType.Activate, PlanActivate);
            AddExecutor(ExecutorType.SpSummon, PlanSpecialSummon);
            AddExecutor(ExecutorType.Summon, PlanNormalSummon);

            // 3b) Banimento dos novas: evitar ataques do oponente, limpar a mesa dele antes da nossa batalha e trazer o nova de volta.
            //     Crimson Gaia na nossa batalha (linha RDA + Gaia) e Soul Resonator protegendo pelo GY.
            AddExecutor(ExecutorType.Activate, SoulGraveProtection);
            AddExecutor(ExecutorType.Activate, GaiaBattleEffect);
            AddExecutor(ExecutorType.Activate, NovaVsAttack);
            AddExecutor(ExecutorType.Activate, NovaClearBoard);
            AddExecutor(ExecutorType.Activate, ReviveBanishedNova);

            // 4) Depois do combo
            AddExecutor(ExecutorType.SpellSet, SetTrapAfterCombo);
            AddExecutor(ExecutorType.GoToMainPhase2, SkipBattlePhase);
            AddExecutor(ExecutorType.Repos, ReposGuard);
        }

        // =====================================================================================================
        // PARTE 1 - VERIFICAÇÃO DO DECK
        // =====================================================================================================

        // PURPOSE: quantas cópias da carta ainda estão no deck.
        // TRUSTS: rastreio de deck do framework (ClientField.GetCardCountInDeck).
        public int CheckRemainInDeck(int cardId)
        {
            return Bot.GetCardCountInDeck(cardId);
        }

        public int CheckRemainInDeck(params int[] cardIds)
        {
            return cardIds.Sum(id => Bot.GetCardCountInDeck(id));
        }

        // PURPOSE: confirmar, no início do duelo, que o .ydk carregado bate com MainDeckList/ExtraDeckList.
        // TRUSTS: chamado no primeiro OnNewTurn, quando só existem cartas no deck e na mão inicial.
        // RESET: roda uma vez por duelo (_deckListValidated).
        private void ValidateDeckList()
        {
            if (!Bot.DeckTrackingActive)
            {
                Report("deck_check", "deck tracking inactive (duel tag?), check skipped");
                return;
            }

            var problems = new List<string>();

            foreach (KeyValuePair<int, int> entry in MainDeckList)
            {
                int actual = Bot.GetCardCountInDeck(entry.Key) + Bot.Hand.Count(card => card != null && card.IsOriginalCode(entry.Key));
                if (actual != entry.Value)
                    problems.Add(string.Format("main {0}: expected {1}, found {2}", CardName(entry.Key), entry.Value, actual));
            }

            int expectedMainCount = MainDeckList.Values.Sum();
            int actualMainCount = Bot.Deck.Count + Bot.Hand.Count;
            if (actualMainCount != expectedMainCount)
                problems.Add(string.Format("main total: expected {0}, found {1} (there are cards outside the executor list)", expectedMainCount, actualMainCount));

            if (Bot.ExtraDeck.Any(card => card != null && card.Id == 0))
            {
                Report("deck_check", "Extra Deck ids still unknown, Extra not checked");
            }
            else
            {
                foreach (KeyValuePair<int, int> entry in ExtraDeckList)
                {
                    int actual = Bot.ExtraDeck.Count(card => card != null && card.IsOriginalCode(entry.Key));
                    if (actual != entry.Value)
                        problems.Add(string.Format("extra {0}: expected {1}, found {2}", CardName(entry.Key), entry.Value, actual));
                }
                foreach (ClientCard card in Bot.ExtraDeck.Where(card => card != null && !ExtraDeckList.Keys.Any(id => card.IsOriginalCode(id))))
                    problems.Add(string.Format("extra {0}: card outside the executor list", card.Name ?? card.Id.ToString()));
            }

            if (problems.Count == 0)
            {
                Report("deck_check", string.Format("deck list matches: main {0}, extra {1}", expectedMainCount, ExtraDeckList.Values.Sum()));
            // Sai sempre, mesmo com a lista divergente: saber em que modo o bot está é o primeiro dado de qualquer
            // diagnóstico de tempo.
            Report("deck_check", string.Format("decision mode: {0} (width {1}, ceiling {2} ms, contenders {3})",
                _modeName, RdaPlanner.RouteVerifySearch.Width, RdaPlanner.RouteVerifySearch.TimeLimitMs,
                RdaPlanner.ResilienceContenderCount));
                return;
            }

            foreach (string problem in problems)
            {
                Logger.WriteErrorLine("[TheCrimsonKing] deck mismatch: " + problem);
                LogNote("deck_check", problem);
            }
        }

        // Só Debug: em Release a chamada some na compilação (com o string.Format dos argumentos), sem custo.
        [System.Diagnostics.Conditional("DEBUG")]
        private void Report(string tag, string message)
        {
            Logger.WriteLine("[TheCrimsonKing] " + message);
            LogNote(tag, message);
        }

        // Log estruturado (.jsonl): existe só no nosso WindBot modificado (GameAI.Log / Game\DuelLog.cs). O acesso é por reflexão
        // para o executor compilar e rodar no WindBot oficial, onde a propriedade não existe e a nota é simplesmente ignorada.
        private object _duelLog;
        private System.Reflection.MethodInfo _duelLogNote;
        private bool _duelLogResolved;

        [System.Diagnostics.Conditional("DEBUG")]
        private void LogNote(string tag, string message)
        {
            if (!_duelLogResolved)
            {
                _duelLogResolved = true;
                try
                {
                    System.Reflection.PropertyInfo property = AI != null ? AI.GetType().GetProperty("Log") : null;
                    _duelLog = property != null ? property.GetValue(AI, null) : null;
                    _duelLogNote = _duelLog != null ? _duelLog.GetType().GetMethod("Note", new[] { typeof(string), typeof(string) }) : null;
                }
                catch (Exception)
                {
                    _duelLogNote = null;
                }
            }
            if (_duelLogNote == null)
                return;
            try
            {
                _duelLogNote.Invoke(_duelLog, new object[] { tag, message });
            }
            catch (Exception)
            {
                // O log nunca pode quebrar o duelo.
            }
        }

        private static string CardName(int cardId)
        {
            YGOSharp.OCGWrapper.NamedCard card = YGOSharp.OCGWrapper.NamedCard.Get(cardId);
            return card != null ? card.Name + " (" + cardId + ")" : cardId.ToString();
        }

        // =====================================================================================================
        // PARTE 2 - EXECUÇÃO DO PLANO
        // =====================================================================================================

        // -----------------------------------------------------------------------------------------------------
        // 2.1 Ciclo de vida: zera o que vale só por turno, por fase ou por chain
        // -----------------------------------------------------------------------------------------------------

        public override void OnNewTurn()
        {
            KnowledgeCheckLifeLoss();
            // Duelo novo: o turno ANDOU PARA TRÁS. Testar "Turn <= 1" não serve — jogando em segundo, o nosso
            // primeiro OnNewTurn já vê turno 2, o reset nunca roda e o processo, que joga várias partidas
            // seguidas, carrega a lista do duelo anterior. Foi o que corrompeu a coleta de 2026-09-23: a linha
            // "seen" era escrita uma vez só (a carta já estava na lista) enquanto "bad" era escrita a cada duelo,
            // e a taxa passou de 100%.
            if (Duel.Turn < _lastSeenTurn)
                KnowledgeNewDuel();
            _lastSeenTurn = Duel.Turn;
            if (!_deckListValidated)
            {
                _deckListValidated = true;
                ValidateDeckList();
            }

            _usedThisTurn.Clear();
            _normalSummonUsed = false;
            _extraNormalSummonAvailable = false;
            _noSpecialSummon = false;
            _specialSummonBlockedCount = 0;
            _turnFlags = 0;
            _plan = null;
            _nextStep = 0;
            _doneSteps.Clear();
            _idleStep = null;
            _replansThisTurn = 0;
            _plannerDisabledThisTurn = false;
            _comboDone = false;
            _portfolioUsedThisTurn = false;
            _currentAction = null;
            _actionBySource.Clear();
            _remainingPicks.Clear();
            _declinedSources.Clear();
            _summonedThisTurn = 0;
            _baitResolvedThisTurn = false;
            _comboStartedThisTurn = false;
            _unansweredCriticalsThisTurn = 0;
            _opponentActivationsThisTurn = 0;
            _novaBanishThisTurn = false;
            _novaBanishInWindow = false;
            _opponentSummonsSinceNova = 0;
            _opponentExtraSummonsSinceNova = 0;
            _harmoniaUsedThisTurn = false;
            _crimsonCallChainAttack = false;
            _negateUsedThisTurn.Clear();
            _novaReviveSource = null;
            _opponentSpecialSummonsThisTurn = 0;
            _rdaChainBeforeComboDone = false;
            _rdaChainBeforeComboChecks = 0;
            _magnamhutEndPhaseSearch = false;
            _redRisingToGraveThisTurn = 0;
            ClearAuditSelections();
            _blockedActions.Clear();
            _blockedState = null;
            ClearResponse();
            // Plano em segundo plano de outro turno não vale mais.
            _backgroundStartedThisTurn = false;
            _backgroundPlan = null;
            _backgroundRoot = null;
            CancelBackgroundPlan();
            base.OnNewTurn();
        }

        // Fim do duelo sem hook dedicado: o Executor do WindBot oficial não declara OnDuelEnd (nem o chamaria), então
        // o cancelamento observa o fim da conexão, que existe nas duas versões. O GameBehavior fecha a conexão ao
        // receber o resultado do duelo e o evento é disparado pelo próprio Tick logo em seguida — mesmo instante em
        // que o hook da versão modificada agia. Qualquer busca em segundo plano ainda rodando não tem mais sentido e
        // só ocuparia CPU até terminar o orçamento (P4).
        private void HookConnectionEnd()
        {
            try
            {
                if (AI != null && AI.Game != null && AI.Game.Connection != null)
                    AI.Game.Connection.Disconnected += OnConnectionDisconnected;
            }
            catch (Exception)
            {
                // Sem conexão observável o cancelamento continua valendo nos outros pontos
                // (OnNewTurn, limite de replan, descarte do plano em TakeBackgroundPlan...).
            }
        }

        private void OnConnectionDisconnected(Exception error)
        {
            CancelBackgroundPlan();
        }

        public override void OnNewPhase()
        {
            // Um prompt de efeito sim/não na End Phase não pode ser confundido com o menu da Main Phase.
            _prompt = PromptKind.None;
            _novaBanishInWindow = false;
            // Turno 1 (sem compra): a mão já está completa na Draw Phase. Nos outros turnos o cálculo começa na compra (OnDraw);
            // a Standby é a garantia se a compra não disparou.
            if (Duel.Player == 0 && ((Duel.Phase == DuelPhase.Draw && Duel.Turn == 1) || Duel.Phase == DuelPhase.Standby))
                StartBackgroundPlan(Duel.Phase == DuelPhase.Draw ? "opening hand, before the Main Phase" : "Standby Phase");
            base.OnNewPhase();
        }

        public override void OnSelectChain(IList<ClientCard> cards)
        {
            // Chamado no início de toda janela de chain: a partir daqui PlanActivate trata gatilhos.
            _prompt = PromptKind.Chain;
            _chainCandidates = cards;
            base.OnSelectChain(cards);
        }

        public override void OnChainEnd()
        {
            _declinedSources.Clear();
            _novaBanishInWindow = false;
            ClearResponse();
            ClearAuditSelections();
            base.OnChainEnd();
        }

        // PURPOSE: avisar no log quando uma carta do oponente muda as regras do turno.
        // TRUSTS: DefaultExecutor.OnChainSolved guarda Droll (resolvedEffectIdList) e Maxx "C"/Fuwalos do
        //         oponente (enemyResolvedEffectIdList); ReadState lê essas listas e o plano é refeito sozinho,
        //         porque as flags fazem o estado real deixar de bater com o plano.
        public override void OnChainSolved(int chainIndex)
        {
            base.OnChainSolved(chainIndex);
            ChainInfo info = Duel.GetCurrentSolvingChainInfo();
            if (info == null)
                return;
            // Quem foi negado. O Duel.NegatedChainIndexList é preenchido pelo core em ChainNegated/ChainDisabled e
            // só um executor no repositório o lia. Saber que o NOSSO efeito foi parado muda o replanejamento; saber
            // que o DELE foi parado diz que a nossa resposta funcionou.
            if (Duel.IsCurrentSolvingChainNegated())
            {
                string name = info.RelatedCard != null ? (info.RelatedCard.Name ?? info.ActivateId.ToString())
                    : info.ActivateId.ToString();
                Report("opponent", "negated: " + (info.ActivatePlayer == 0 ? "our " : "their ") + name
                    + " (chain link " + (chainIndex + 1) + ")");
                // Memória: efeito NOSSO negado é resultado ruim. Marca todas as cartas que ele já resolveu neste
                // duelo, porque não sabemos qual delas montou a resposta — a estatística separa isso ao longo das
                // partidas, e é ela que sabe que ele tem mais de uma rota.
                if (info.ActivatePlayer == 0)
                    KnowledgeMarkOutcome();
            }
            else
            {
                ReportResolvedOpponentEffect(info, chainIndex);
                if (info.ActivatePlayer == 1 && info.ActivateId != 0 && !_opponentResolvedThisDuel.Contains(info.ActivateId))
                {
                    // Antes de somar a carta à lista: se ELA é uma das destacadas, credita quem veio logo antes.
                    KnowledgeMarkLeadUp(info.ActivateId);
                    _opponentResolvedThisDuel.Add(info.ActivateId);
                    KnowledgeWriteSeen();
                }
            }
            // Nosso nova baniu (efeito resolvido): zera a contagem do que o oponente jogou. O nova pode voltar (Harmonia +
            // Storm-Bane, Red Zone) e o oponente pode continuar jogando; a contagem recomeça daqui (jogador, 2026-09-15).
            if (info.ActivatePlayer == 0 && !Duel.IsCurrentSolvingChainNegated()
                && (info.IsActivateCode(CardId.RedHypernovaDragon) || info.IsActivateCode(CardId.RedSupernovaDragon)))
            {
                _opponentSummonsSinceNova = 0;
                _opponentExtraSummonsSinceNova = 0;
            }
            // Nossa isca resolvendo: negada = pode usar outra isca (se não fizer falta); sem resposta = rota segura, sem outra.
            if (info.ActivatePlayer == 0 && info.RelatedCard != null)
            {
                PlanAction action;
                if (!_actionBySource.TryGetValue(info.RelatedCard, out action))
                    action = _actionBySource.Values.LastOrDefault(item => item.CardId == CardCode(info.RelatedCard));
                if (action != null && RdaPlanner.IsCriticalAction(action) && !Duel.IsCurrentSolvingChainNegated())
                    _unansweredCriticalsThisTurn++;
                if (action != null && !RdaPlanner.IsBaitAction(action))
                    _comboStartedThisTurn = true;
                else if (action != null && !_comboStartedThisTurn)
                {
                    if (Duel.IsCurrentSolvingChainNegated())
                        Report("interaction", "bait negated by the opponent: " + action.Text + " (another bait may come before the starter)");
                    else if (!_baitResolvedThisTurn)
                    {
                        _baitResolvedThisTurn = true;
                        Report("interaction", "bait resolved with no answer: " + action.Text + " (takes the safest route, with no other bait)");
                    }
                }
                return;
            }
            if (Duel.IsCurrentSolvingChainNegated() || info.ActivatePlayer != 1)
                return;
            if (info.IsActivateCode(CardId.DrollLockBird))
                Report("interaction", "Droll & Lock Bird resolved: nothing else leaves the Deck for the hand this turn");
            else if (info.IsActivateCode(CardId.MaxxC))
                Report("interaction", "opponent Maxx \"C\": every Special Summon gives them a card");
            else if (info.IsActivateCode(CardId.MulcharmyFuwalos))
                Report("interaction", "opponent Mulcharmy Fuwalos: a Special Summon from the Deck/Extra gives them a card");
        }

        // PURPOSE: contar os monstros que o oponente invocou por Invocação-Especial no turno (King's Resonance: 1+/3+/5+/10+).
        public override void OnSpSummoned()
        {
            _opponentSpecialSummonsThisTurn += Duel.LastSummonedCards.Count(card => card != null && card.Controller == 1);
            base.OnSpSummoned();
        }

        // PURPOSE: contar monstros que entram no nosso campo no nosso turno (condição do Nibiru: 5 ou mais).
        public override void OnMove(ClientCard card, int previousControler, int previousLocation, int currentControler, int currentLocation)
        {
            if (card != null && currentControler == 0 && currentLocation == (int)CardLocation.Grave && previousLocation != (int)CardLocation.Grave
                && CardCode(card) == CardId.RedRisingDragon)
                _redRisingToGraveThisTurn++;
            if (Duel.Player == 0 && currentControler == 0 && currentLocation == (int)CardLocation.MonsterZone
                && previousLocation != (int)CardLocation.MonsterZone)
                _summonedThisTurn++;
            RememberEnemyCard(card);
            // Nosso monstro do Extra que vai direto do Extra Deck para o GY (Harmonia, custo...) nunca foi invocado: não pode ser
            // revivido do GY. Sai da marca quando deixa o GY (ex.: banido e trazido de volta).
            if (card != null && currentControler == 0)
            {
                if (currentLocation == (int)CardLocation.Grave && previousLocation == (int)CardLocation.Extra)
                    _extraToGraveUnsummoned.Add(card);
                else if (currentLocation != (int)CardLocation.Grave)
                    _extraToGraveUnsummoned.Remove(card);
                // RDA que entra no campo pelo efeito do Crimson King: já é imune aos efeitos do oponente, então o King's
                // Resonance (3+) não faz falta para ele (jogador, 2026-09-15).
                if (currentLocation == (int)CardLocation.MonsterZone && previousLocation == (int)CardLocation.Extra
                    && CardCode(card) == CardId.RedDragonArchfiend && _responseKind == ResponseKind.King)
                    _kingRdaCards.Add(card);
                else if (currentLocation != (int)CardLocation.MonsterZone)
                    _kingRdaCards.Remove(card);
                // RDA trazido pelo efeito da Crimson Gaia: é a salvação do deck (jogador, 2026-09-15) e nunca vira custo.
                if (currentLocation == (int)CardLocation.MonsterZone && previousLocation != (int)CardLocation.MonsterZone
                    && CardCode(card) == CardId.RedDragonArchfiend && GaiaLinkResolving())
                    _gaiaRdaCards.Add(card);
                else if (currentLocation != (int)CardLocation.MonsterZone)
                    _gaiaRdaCards.Remove(card);
            }
            // Ordem em que os monstros do oponente entram no campo (qualquer turno): desempate "último invocado" na escolha de alvo.
            if (card != null && currentControler == 1 && currentLocation == (int)CardLocation.MonsterZone
                && previousLocation != (int)CardLocation.MonsterZone)
                _enemyEntryOrder[card] = ++_enemyEntryCounter;
            // Turno do oponente: monstros que ele coloca no campo desde o último banimento do nova (Invocação-Normal ou Especial).
            if (Duel.Player == 1 && currentControler == 1 && currentLocation == (int)CardLocation.MonsterZone
                && previousLocation != (int)CardLocation.MonsterZone)
            {
                _opponentSummonsSinceNova++;
                if (previousLocation == (int)CardLocation.Extra)
                    _opponentExtraSummonsSinceNova++;
            }
            base.OnMove(card, previousControler, previousLocation, currentControler, currentLocation);
        }

        // PURPOSE: a partir do momento em que o elo da nossa resposta aparece, alvos/custos já foram escolhidos.
        public override void OnChaining(int player, ClientCard card)
        {
            if (player == 0 && card != null && card == _responseSource)
                _responseFresh = false;
            if (player == 1 && Duel.Player == 1)
                _opponentActivationsThisTurn++;
            base.OnChaining(player, card);
        }

        // -----------------------------------------------------------------------------------------------------
        // Coleta de dados — SÓ LOG. Quatro sinais que o WindBot já entrega e que ninguém usava.
        // Servem para medir, antes de ligar qualquer decisão, se dá mesmo para saber "qual efeito vale interromper".
        // -----------------------------------------------------------------------------------------------------

        /// <summary>
        /// Qual efeito do oponente resolveu. Tem que ser lido na RESOLUÇÃO, não no OnChaining: o GameBehavior chama
        /// o nosso OnChaining ANTES de adicionar o elo ao CurrentChainInfo, então ali a descrição ainda não existe —
        /// foi por isso que a primeira coleta saiu quase toda sem o índice do efeito (partidas de 2026-09-22 22:xx).
        /// O ActivateDescription do elo empacota carta + índice, e o índice casa com o nosso perfil de efeito.
        /// </summary>
        private void ReportResolvedOpponentEffect(ChainInfo info, int chainIndex)
        {
            if (info == null || info.ActivatePlayer != 1)
                return;
            int id = info.ActivateId != 0 ? info.ActivateId : info.ActivateAlias;
            if (id == 0)
                return;
            var named = YGOSharp.OCGWrapper.NamedCard.Get(id);
            string name = named != null ? named.Name : id.ToString();
            int desc = info.ActivateDescription;
            int index = desc > 0 && desc / 16 == id ? desc & 0xF : -1;
            string label = "resolved: " + name + (index >= 0 ? " effect #" + index : " (effect index unknown)")
                + " from " + info.ActivateLocation;
            List<EffectProfile> profiles = EffectProfilesOf(id);
            EffectProfile exact = index >= 0 ? profiles.FirstOrDefault(p => p.DescIndex == index) : null;
            if (exact != null && exact.Does.Count > 0)
                label += " | " + string.Join(" + ", exact.Does);
            else if (profiles.Count > 0)
                label += " | card does: " + string.Join(" / ",
                    profiles.Where(p => p.Does.Count > 0).Select(p => string.Join("+", p.Does)));
            Report("opponent", label);
        }

        /// <summary>
        /// O jogo avisa qual opção foi escolhida quando a carta tem vários efeitos (HINT_OPSELECTED). O WindBot já
        /// grava em ChainInfo.Announces e nenhum executor lia. É a peça que desfaz a ambiguidade quando o
        /// ActivateDescription não basta.
        /// </summary>
        public override void OnReceivingAnnouce(int player, int data)
        {
            base.OnReceivingAnnouce(player, data);
            if (player != 1 || data <= 0)
                return;
            int id = data / 16, index = data & 0xF;
            var named = YGOSharp.OCGWrapper.NamedCard.Get(id);
            if (named != null)
                Report("opponent", "chose effect #" + index + " of " + named.Name);
        }

        /// <summary>
        /// Efeito contínuo que passou a (ou deixou de) afetar um jogador. O core manda DescAdd/DescRemove e o
        /// DefaultExecutor já mantém o conjunto; ninguém lia. Cobertura parcial: só vale para efeitos cujo script
        /// declara descrição — o Dark Contract que travou o bot, por exemplo, não declara.
        /// </summary>
        public override void OnPlayerHint(int player, int hintType, int description)
        {
            base.OnPlayerHint(player, hintType, description);
            if (description <= 0 || (player != 0 && player != 1))
                return;
            int id = description / 16, index = description & 0xF;
            var named = YGOSharp.OCGWrapper.NamedCard.Get(id);
            if (named == null)
                return;
            string who = player == 0 ? "us" : "the opponent";
            string verb = hintType == (int)PlayerHintType.DescAdd ? "now affects " : "stopped affecting ";
            Report("opponent", "continuous: " + named.Name + " effect #" + index + " " + verb + who);
        }

        // -----------------------------------------------------------------------------------------------------
        // 2.2 Leitura do estado real do duelo no formato do planejador
        // -----------------------------------------------------------------------------------------------------

        // Código usado pelo modelo.
        // ATENÇÃO: alguns ids da lista são impressões de arte alternativa (cards.cdb):
        //   Quetzacoatl 29053657 (original 29053656), Supernova 99585851 (99585850),
        //   Burning Soul 65541656 (65541655), RDA's Chain 92936365 (92936364), Dominus 40366668 (40366667).
        // GetNonAltartCode() devolve o original, que não está no modelo; no teste isso fez Quetzacoatl e
        // Supernova sumirem do Extra lido e o plano caiu de 321 para 228. Aqui qualquer código
        // (impressão ou original) é convertido para o id que o modelo usa.
        private static Dictionary<int, int> _modelCodes;

        private static int CardCode(ClientCard card)
        {
            if (_modelCodes == null)
            {
                var codes = new Dictionary<int, int>();
                foreach (int id in RdaCards.All.Keys)
                {
                    codes[id] = id;
                    YGOSharp.OCGWrapper.NamedCard data = YGOSharp.OCGWrapper.NamedCard.Get(id);
                    if (data != null && YGOSharp.OCGWrapper.NamedCard.IsAltartAlias(id, data.Alias) && !codes.ContainsKey(data.Alias))
                        codes[data.Alias] = id;
                }
                _modelCodes = codes;
            }

            int model;
            if (_modelCodes.TryGetValue(card.Id, out model))
                return model;
            if (card.Alias != 0 && _modelCodes.TryGetValue(card.Alias, out model))
                return model;
            return card.Id;
        }

        private static bool IsModeled(int code)
        {
            return RdaCards.All.ContainsKey(code);
        }

        // Cartas conhecidas de uma lista, ordenadas (o estado usa arrays ordenados).
        private static int[] KnownCodes(IEnumerable<ClientCard> cards)
        {
            return cards.Where(card => card != null)
                .Select(CardCode)
                .Where(IsModeled)
                .OrderBy(code => code)
                .ToArray();
        }

        // Monstros LIGHT/DARK de uma lista do oponente (GY/banidas), registrados no modelo como cartas de fora.
        // Os valores vivos de cada carta entram no overlay 'foreign' da duel (lido pelas regras), nunca no cache estático.
        private static int[] EnemyLightDarkMonsters(IEnumerable<ClientCard> cards, Dictionary<int, ForeignLive> foreign)
        {
            var result = new List<int>();
            foreach (ClientCard card in cards)
            {
                if (card == null || card.Id == 0 || !card.IsMonster() || card.HasType(CardType.Xyz) || card.HasType(CardType.Link) || card.HasType(CardType.Token))
                    continue;
                if ((card.Attribute & ((int)CardAttribute.Light | (int)CardAttribute.Dark)) == 0)
                    continue;
                int code = CardCode(card);
                if (!IsModeled(code))
                    RdaCards.RegisterForeign(code, card.Name ?? ("#" + code));
                RememberForeignLive(foreign, code, card);
                result.Add(code);
            }
            result.Sort();
            return result.ToArray();
        }

        // Overlay da duel para uma carta de fora: só entra se o modelo marcou a carta como de fora (as nossas ficam
        // com o modelo estático, cujos valores de impressão são imutáveis e valem para sempre).
        private static void RememberForeignLive(Dictionary<int, ForeignLive> foreign, int code, ClientCard card)
        {
            RdaCardInfo info = RdaCards.Get(code);
            if (info != null && info.Foreign)
                foreign[code] = new ForeignLive(card.Level, (CardAttribute)card.Attribute, (CardRace)card.Race, card.IsTuner());
        }

        // PURPOSE: montar o RdaState do nosso lado agora.
        // TRUSTS: _usedThisTurn/_normalSummonUsed/etc. anotados pelo executor, porque o jogo não os expõe.
        //         Cartas fora do modelo são ignoradas; zonas de magia com carta desconhecida viram marcador.
        private RdaState ReadState()
        {
            var deck = new List<int>();
            foreach (KeyValuePair<int, int> entry in MainDeckList)
            {
                int copies = Bot.DeckTrackingActive ? Bot.GetCardCountInDeck(entry.Key) : entry.Value;
                for (int i = 0; i < copies; ++i)
                    deck.Add(entry.Key);
            }

            // Monstros: nível atual, se está na Zona de Monstros Extra (sequência 5 ou 6) e se está negado.
            // Overlay com os valores vivos das cartas de fora desta leitura (consultado pelas regras; ver ForeignDynamic).
            var foreignDynamic = new Dictionary<int, ForeignLive>();
            var field = new List<long>();
            for (int seq = 0; seq < Bot.MonsterZone.Length; ++seq)
            {
                ClientCard card = Bot.MonsterZone[seq];
                if (card == null || !card.IsFaceup())
                    continue;
                int code = CardCode(card);
                if (!IsModeled(code))
                {
                    // Monstro que não é do nosso deck no nosso campo (token do Nibiru, monstro que o oponente deixou):
                    // ocupa zona, conta nas condições de campo (Crimson Resonator, Power Vice...) e pode ser material ou
                    // custo (Bone). Antes ele era ignorado e o plano tentava ações que o jogo não oferecia.
                    if (RdaCards.RegisterForeign(code, card.Name ?? ("#" + code)))
                        Report("interaction", string.Format("monster outside our deck on the field joins the plan: {0} (Level {1})", card.Name ?? ("#" + code), card.Level));
                }
                RememberForeignLive(foreignDynamic, code, card);
                field.Add(RdaField.Encode(code, card.Level, seq >= 5, card.IsDisabled()));
            }

            // Magias/armadilhas: só as 5 zonas normais (a Field Zone não conta para o modelo).
            var spells = new List<int>();
            for (int seq = 0; seq < 5; ++seq)
            {
                ClientCard card = Bot.SpellZone[seq];
                if (card == null)
                    continue;
                // Carta nossa numa coluna negada pela Impermanence ou com os efeitos negados por outra carta (teste contra o
                // Albaz: Brightest, Blazing, Branded King negou a Crimson Gaia, que ficou no campo; o bot ativou a busca dela
                // mesmo assim e perdeu o Darkness Resonator): ocupa a zona, mas não tem efeito neste turno.
                bool negatedColumn = infiniteImpermanenceNegatedColumns.Contains(seq);
                bool disabled = card.IsFaceup() && card.IsDisabled();
                spells.Add(IsModeled(CardCode(card)) && !negatedColumn && !disabled ? CardCode(card) : RdaCards.FaceDownCard);
            }
            int blockedSpellZones = 0;
            for (int seq = 0; seq < 5; ++seq)
                if (Bot.SpellZone[seq] == null && infiniteImpermanenceNegatedColumns.Contains(seq))
                    blockedSpellZones++;

            // Lado do oponente: condições "no campo" dos dois lados e alvos da Magnamhut e da Dis Pater ①.
            List<ClientCard> enemyMonsters = Enemy.GetMonsters().Where(card => card != null && card.IsFaceup()).ToList();
            bool enemySynchro = enemyMonsters.Any(card => card.HasType(CardType.Synchro));
            bool enemyDarkLevel5 = enemyMonsters.Any(card => !card.HasType(CardType.Xyz) && !card.HasType(CardType.Link)
                && card.Level >= 5 && (card.Attribute & (int)CardAttribute.Dark) != 0);
            bool fieldZoneCard = Bot.SpellZone[5] != null || Enemy.SpellZone[5] != null;
            int[] enemyGrave = EnemyLightDarkMonsters(Enemy.Graveyard, foreignDynamic);
            int[] enemyBanished = EnemyLightDarkMonsters(Enemy.Banished.Where(card => card != null && card.IsFaceup()), foreignDynamic);
            int enemyBanishedOther = Math.Max(0, Enemy.Banished.Count(card => card != null) - enemyBanished.Length);
            ClientCard bestGraveTarget = Enemy.Graveyard.Where(card => card != null && Array.IndexOf(enemyGrave, CardCode(card)) >= 0)
                .OrderByDescending(MagnamhutTargetValue).FirstOrDefault();
            int enemyGravePriority = bestGraveTarget != null ? CardCode(bestGraveTarget) : 0;
            bool redZoneReady = Bot.SpellZone.Take(5).Any(card => card != null && card.IsFaceup() && !card.IsDisabled()
                && CardCode(card) == CardId.RedZone && !infiniteImpermanenceNegatedColumns.Contains(card.Sequence));
            int redRisingGraveReady = Math.Max(0, Bot.Graveyard.Count(card => card != null && CardCode(card) == CardId.RedRisingDragon)
                - _redRisingToGraveThisTurn);
            // 1 monstro do oponente já basta para querer o RDA na mesa (jogador, 2026-09-17: o RDA é a diferença entre
            // ganhar e perder). Antes eram 2, o que deixava o bot escolher o King numa mesa contra 1 monstro só.
            bool battleWipeReady = Duel.Turn > 1 && Duel.Player == 0 && Enemy.GetMonsters().Count(card => card != null) >= 1;
            // A Gaia só serve à linha do RDA se o efeito dela estiver valendo. Mesma verificação da Red Zone.
            bool gaiaNegated = Bot.SpellZone.Any(card => card != null && card.IsFaceup()
                && CardCode(card) == CardId.CrimsonGaia
                && (card.IsDisabled() || infiniteImpermanenceNegatedColumns.Contains(card.Sequence)));
            bool secondTurnOrLater = Duel.Turn > 1 && Duel.Player == 0;
            // RDA imune no campo (efeito do Crimson King ou da Crimson Gaia). Jogador, replay de 2026-09-16: o bot deu o RDA
            // protegido como custo do Bone, o Bone foi negado e a mesa ficou sem a peça que não podia ser destruída.
            bool protectedRda = Bot.GetMonsters().Any(card => card != null && CardCode(card) == CardId.RedDragonArchfiend
                && (_kingRdaCards.Contains(card) || _gaiaRdaCards.Contains(card)));

            return new RdaState
            {
                Hand = KnownCodes(Bot.Hand),
                Deck = ReadDeckChecked(deck),
                // Monstro do Extra que foi ao GY sem ter sido invocado (ex.: Storm-Bane mandado pela Harmonia) não pode voltar
                // ao campo: fica fora do GY do modelo, senão o plano conta com um revive que o jogo não oferece (partida contra
                // Yubel, 2026-09-15: "escolha não encontrada" no revive da Quetzacoatl).
                Grave = KnownCodes(Bot.Graveyard.Where(card => card == null || !_extraToGraveUnsummoned.Contains(card))),
                Banished = KnownCodes(Bot.Banished),
                Extra = KnownCodes(Bot.ExtraDeck),
                Field = field.OrderBy(value => value).ToArray(),
                Spells = spells.OrderBy(code => code).ToArray(),
                Used = _usedThisTurn.Concat(_burningSoulAddUsedThisDuel ? new[] { RdaKey.BurningSoulAdd } : new RdaKey[0])
                    .Select(key => (int)key).Distinct().OrderBy(key => key).ToArray(),
                NormalSummon = _normalSummonUsed ? 0 : 1,
                ExtraNormalSummon = _extraNormalSummonAvailable ? 1 : 0,
                NoSpecial = _noSpecialSummon,
                Flags = _turnFlags | (_rdaSynchroThisDuel ? RdaState.FlagRdaSynchro : 0) | OpponentEffectFlags()
                    | (_baitResolvedThisTurn ? RdaState.FlagBaitDone : 0) | (protectedRda ? RdaState.FlagProtectedRda : 0),
                SummonedThisTurn = _summonedThisTurn,
                UnansweredCriticals = _unansweredCriticalsThisTurn,
                BlockedSpellZones = blockedSpellZones,
                EnemySynchro = enemySynchro,
                EnemyDarkLevel5 = enemyDarkLevel5,
                FieldZoneCard = fieldZoneCard,
                ForcedRace = FieldForcedRace(),
                ForcedAttribute = FieldForcedAttribute(),
                EnemyHand = ReadKnownEnemyHand(),
                EnemyGrave = enemyGrave,
                EnemyBanished = enemyBanished,
                EnemyBanishedOther = enemyBanishedOther,
                EnemyGravePriority = enemyGravePriority,
                RedZoneReady = redZoneReady,
                RedRisingGraveReady = redRisingGraveReady,
                BattleWipeReady = battleWipeReady,
                GaiaNegated = gaiaNegated,
                SecondTurnOrLater = secondTurnOrLater,
                ForeignDynamic = foreignDynamic.Count > 0 ? foreignDynamic : null
            };
        }

        // PURPOSE: efeitos do oponente que já resolveram neste turno e mudam o que o plano pode fazer.
        private int OpponentEffectFlags()
        {
            int flags = 0;
            if (resolvedEffectIdList.Contains(CardId.DrollLockBird))
                flags |= RdaState.FlagNoDeckAdd;
            if (Duel.Player == 0 && enemyResolvedEffectIdList.Contains(CardId.MaxxC))
                flags |= RdaState.FlagMaxxC;
            if (Duel.Player == 0 && enemyResolvedEffectIdList.Contains(CardId.MulcharmyFuwalos))
                flags |= RdaState.FlagFuwalos;
            return flags;
        }

        // -----------------------------------------------------------------------------------------------------
        // 2.3 Plano: sincronizar, recalcular e escolher o passo do menu
        // -----------------------------------------------------------------------------------------------------

        private static bool IsMainPhase(DuelPhase phase)
        {
            return phase == DuelPhase.Main1 || phase == DuelPhase.Main2;
        }

        // PURPOSE: roda no começo de cada menu da Main Phase. Decide qual passo do plano pode ser feito agora
        //          e guarda em _idleStep. Nunca termina o turno (sempre retorna false).
        // TRUSTS: Duel.MainPhase já preenchido pelo GameBehavior antes do OnSelectIdleCmd.
        // Guarda dos menus da Main Phase: prepara o plano uma vez por menu e nunca escolhe ação.
        private bool IdleGuard()
        {
            if (Duel.MainPhase != null && !ReferenceEquals(_preparedMenu, Duel.MainPhase) && IsMainPhase(Duel.Phase))
                PrepareIdlePrompt();
            return false;
        }

        private bool PrepareIdlePrompt()
        {
            // Uma vez por menu (jogador, 2026-09-15, 3 partidas contra Yubel): com o menu sem "ir para a End Phase" o WindBot não
            // avaliava esta preparação, nenhum passo do plano era escolhido e a regra padrão dele ia para a batalha no meio do
            // combo (7 de 10 entradas na batalha). Agora os guardas (IdleGuard) também chamam esta preparação.
            if (Duel.MainPhase == null || ReferenceEquals(_preparedMenu, Duel.MainPhase))
                return false;
            _preparedMenu = Duel.MainPhase;
            _prompt = PromptKind.Idle;
            _idleStep = null;
            _currentAction = null;
            // Recusas valem só para a janela em que foram feitas. Um gatilho recusado pode não abrir
            // chain nenhuma (então OnChainEnd não roda) e a mesma carta pode ter outro gatilho depois
            // (ex.: Fiend Piece recusa o nível e mais tarde revive como material do King).
            _declinedSources.Clear();

            if (Duel.Player != 0 || !IsMainPhase(Duel.Phase) || _plannerDisabledThisTurn || _comboDone)
                return false;

            // RDA's Chain antes do combo (jogador): baixar e ativar primeiro; o plano vem depois, com a zona já ocupada.
            // No máximo 3 menus esperando (baixar, ativar, resolver): se o jogo não oferecer, segue com o plano.
            // Teste contra o Albaz (turno 3): a RDA's Chain baixada não estava entre as ativações do menu; esperar sem agir
            // encerrou a Main Phase sem combo. Agora só espera se o menu oferece ativar (baixada) ou baixar (mão).
            if (RdaChainBeforeComboWanted())
            {
                MainPhase menu = Duel.MainPhase;
                bool canActivate = menu != null && menu.ActivableCards.Any(card => card != null && CardCode(card) == CardId.RedDragonArchfiendsChain
                    && card.Location == CardLocation.SpellZone);
                bool canSet = menu != null && menu.SpellSetableCards.Any(card => card != null && CardCode(card) == CardId.RedDragonArchfiendsChain
                    && card.Location == CardLocation.Hand);
                if ((canActivate || canSet) && ++_rdaChainBeforeComboChecks <= 3)
                    return false;
                _rdaChainBeforeComboDone = true;
            }

            RdaState real = ReadState();
            // Os bloqueios valem enquanto o estado real não muda. Antes eram refeitos a cada menu e o plano voltava a tentar
            // variantes da mesma ação indisponível (teste do Nibiru: 18 replanejamentos em laço com o Crimson Resonator).
            if (_blockedState == null || !_blockedState.Equals(real))
            {
                _blockedActions.Clear();
                _blockedState = real;
            }
            var blocked = _blockedActions;

            // Até 6 tentativas: se o passo escolhido não está disponível no menu, ele é bloqueado e o plano refeito.
            for (int attempt = 0; attempt < 6; ++attempt)
            {
                if (!SyncPlan(real) && !Replan(real, blocked))
                    return false;

                int index;
                PlanStep step = NextIdleStep(out index);
                if (step == null)
                {
                    // O plano acabou: o combo está pronto. Daqui em diante só baixa armadilhas.
                    _comboDone = true;
                    Report("plan", "combo finished, predicted score " + _plan.Score.ToString("0.#"));
                    return false;
                }

                if (IsAvailableNow(step.Action))
                {
                    _idleStep = step;
                    _idleStepIndex = index;
                    return false;
                }

                Report("plan", "step unavailable in the menu, replanning without it: " + step.Action.Text);
                // O jogo é a única fonte completa do que é legal: o core não manda metadado de restrição, ele
                // simplesmente não oferece a ação. Quando um passo de Invocação-Especial não é oferecido E o menu
                // não tem NENHUMA Invocação-Especial disponível, alguma coisa está proibindo — e replanejar para
                // outra Synchro só queima o turno. Jogador, 2026-09-22 (log 203848): o Dark Contract with the
                // Eternal Darkness registra EFFECT_CANNOT_BE_SYNCHRO_MATERIAL, o menu ficou sem specialSummon e o
                // bot tentou rota atrás de rota até perder. Duas ocorrências para não cair num estado passageiro.
                bool wantedSpecial = step.Action.Kind == PlanKind.Synchro || step.Action.Kind == PlanKind.SpecialProc;
                if (wantedSpecial && Duel.MainPhase != null && Duel.MainPhase.SpecialSummonableCards.Count == 0)
                {
                    if (++_specialSummonBlockedCount >= 2 && !_noSpecialSummon)
                    {
                        _noSpecialSummon = true;
                        Report("plan", "no Special Summon is being offered: planning the rest of the turn without any");
                    }
                }
                blocked.Add(step.Action.Text);
                // Ativação que o menu não oferece não depende dos alvos: bloqueia o efeito inteiro (todas as variantes).
                string effectKey = EffectBlockKey(step.Action);
                if (effectKey != null)
                    blocked.Add(effectKey);
                _plan = null;
            }
            return false;
        }

        // PURPOSE: descobrir em que ponto do plano o duelo está, comparando o estado real com o previsto.
        // TRUSTS: RdaState.Equals ignora Deck e Extra (o rastreio do deck pode ter pequenas diferenças).
        // Retorna false quando o duelo saiu do plano (negação, carta diferente, efeito fora do modelo...).
        private bool SyncPlan(RdaState real)
        {
            if (_plan == null)
                return false;

            // Procura do fim para o começo: o estado mais adiantado que bate com o real.
            // A marca "isca resolveu" (FlagBaitDone) não muda o duelo: sem ela aqui, a isca resolvendo forçava replanejar no meio
            // do combo (teste: plano de Hypernova trocado por mesa camada 3 depois da busca da Lubellion).
            RdaState comparable = real;
            if (real.HasFlag(RdaState.FlagBaitDone) && _plan.Root != null && !_plan.Root.HasFlag(RdaState.FlagBaitDone))
            {
                comparable = real.Copy();
                comparable.Flags &= ~RdaState.FlagBaitDone;
            }
            for (int j = _plan.Steps.Count - 1; j >= Math.Max(0, _nextStep - 1); --j)
            {
                RdaState after = _plan.Steps[j].After;
                if (after.Pending.Length == 0 && after.Equals(comparable))
                {
                    _nextStep = j + 1;
                    return true;
                }
            }
            return _nextStep == 0 && _plan.Root.Equals(comparable);
        }

        // -----------------------------------------------------------------------------------------------------
        // 2.3b Primeiro plano do turno em segundo plano (jogador, 2026-09-15)
        //   O cálculo começa assim que a mão do turno é conhecida (Draw Phase), enquanto o jogo ainda passa pelas fases
        //   iniciais e animações. Na Main Phase o resultado só é usado se o estado real for idêntico ao usado no cálculo
        //   (mão, Deck, Extra, campo, GY, lado do oponente...). Se o oponente fez algo antes (ex.: Maxx "C" na Draw Phase),
        //   o resultado é descartado e o plano é calculado normalmente: a decisão nunca muda por causa do adiantamento.
        // -----------------------------------------------------------------------------------------------------
        private System.Threading.Tasks.Task<RdaPlan> _backgroundPlan;
        private RdaState _backgroundRoot;
        private System.Diagnostics.Stopwatch _backgroundWatch;
        private bool _backgroundStartedThisTurn;
        // Cancelamento da busca em segundo plano (P4): o token só é observado pelos Plan da tarefa; quando a busca
        // não serve mais (estado mudou, turno virou, replan limit, fim do duelo) o cancelamento faz os Plan pararem
        // no próximo ponto de checagem em vez de gastar o orçamento inteiro de CPU atoa.
        private CancellationTokenSource _backgroundCts;

        public override void OnDraw(int player)
        {
            base.OnDraw(player);
            if (player == 0 && Duel.Player == 0 && Duel.Phase == DuelPhase.Draw)
                StartBackgroundPlan("card drawn in the Draw Phase");
        }

        private void StartBackgroundPlan(string reason)
        {
            if (_backgroundStartedThisTurn || Duel.Player != 0 || _plannerDisabledThisTurn)
                return;
            _backgroundStartedThisTurn = true;
            RdaState root = ReadState();
            if (root.Pending.Length != 0)
                return;
            _backgroundRoot = root;
            _backgroundWatch = System.Diagnostics.Stopwatch.StartNew();
            _backgroundCts = new CancellationTokenSource();
            CancellationToken token = _backgroundCts.Token;
            // A carteira roda com as opções clonadas carregando o token desta execução; as SearchOptions estáticas e
            // compartilhadas ficam intactas (ver SearchOptions.Token / WithToken).
            List<SearchOptions> searches = PortfolioSearches.Take(PortfolioCount).Select(o => o.WithToken(token)).ToList();
            _backgroundPlan = System.Threading.Tasks.Task.Run(() => RdaPlanner.PlanPortfolio(root, searches, null));
            Report("plan", "first plan of the turn started calculating in the background (" + reason + ")");
        }

        // Cancela a busca em segundo plano ainda ativa (se houver). O CTS é zerado antes do Cancel para que uma nova
        // busca iniciada em seguida não conviva com o cancelamento da anterior; Cancel numa source já cancelada é inócuo.
        private void CancelBackgroundPlan()
        {
            CancellationTokenSource source = _backgroundCts;
            _backgroundCts = null;
            if (source != null)
                source.Cancel();
        }

        // O estado usado no cálculo é o mesmo da Main Phase? Compara também o que fica fora do Equals.
        private static bool SameRoot(RdaState a, RdaState b)
        {
            return a.Equals(b)
                && RdaArray.Same(a.Deck, b.Deck) && RdaArray.Same(a.Extra, b.Extra)
                && a.BlockedSpellZones == b.BlockedSpellZones && a.SummonedThisTurn == b.SummonedThisTurn
                && a.UnansweredCriticals == b.UnansweredCriticals
                && a.EnemySynchro == b.EnemySynchro && a.EnemyDarkLevel5 == b.EnemyDarkLevel5 && a.FieldZoneCard == b.FieldZoneCard
                && RdaArray.Same(a.EnemyGrave, b.EnemyGrave) && RdaArray.Same(a.EnemyBanished, b.EnemyBanished)
                && a.EnemyBanishedOther == b.EnemyBanishedOther && a.EnemyGravePriority == b.EnemyGravePriority
                && a.RedZoneReady == b.RedZoneReady && a.RedRisingGraveReady == b.RedRisingGraveReady
                && a.BattleWipeReady == b.BattleWipeReady && a.GaiaNegated == b.GaiaNegated
                && a.SecondTurnOrLater == b.SecondTurnOrLater
                && SameForeignDynamic(a.ForeignDynamic, b.ForeignDynamic);
        }

        // Overlay vivo de cartas de fora idêntico entre duas leituras (mesmas cartas, mesmos valores): se um efeito
        // mudou nível/atributo/raça/tuner entre as leituras, a raiz antiga não serve mais e precisa replanejar.
        private static bool SameForeignDynamic(Dictionary<int, ForeignLive> a, Dictionary<int, ForeignLive> b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a == null || b == null || a.Count != b.Count) return false;
            foreach (KeyValuePair<int, ForeignLive> entry in a)
            {
                ForeignLive other;
                if (!b.TryGetValue(entry.Key, out other)) return false;
                ForeignLive live = entry.Value;
                if (live.Level != other.Level || live.Attribute != other.Attribute
                    || live.Race != other.Race || live.Tuner != other.Tuner)
                    return false;
            }
            return true;
        }

        private RdaPlan TakeBackgroundPlan(RdaState real, ICollection<string> blocked)
        {
            System.Threading.Tasks.Task<RdaPlan> task = _backgroundPlan;
            RdaState root = _backgroundRoot;
            _backgroundPlan = null;
            _backgroundRoot = null;
            if (task == null || root == null)
                return null;
            if ((blocked != null && blocked.Count > 0) || !SameRoot(root, real))
            {
                Report("plan", "background plan discarded: the state changed after the calculation started");
                CancelBackgroundPlan();
                return null;
            }
            long elapsedAtMenu = _backgroundWatch.ElapsedMilliseconds;
            try
            {
                task.Wait();
            }
            catch (Exception error)
            {
                Report("plan", "background plan failed, calculating again: " + error.GetBaseException().Message);
                return null;
            }
            Report("plan", string.Format("background plan reused: it had been running for {0} ms when the menu opened, waited {1} ms more",
                elapsedAtMenu, Math.Max(0, _backgroundWatch.ElapsedMilliseconds - elapsedAtMenu)));
            return task.Result;
        }

        // PURPOSE: calcular um plano novo a partir do estado real.
        // TRUSTS: RdaPlanner (PARTE 3). blocked = ações que o jogo não ofereceu neste estado.
        private bool Replan(RdaState real, ICollection<string> blocked)
        {
            if (++_replansThisTurn > MaxReplansPerTurn)
            {
                _plannerDisabledThisTurn = true;
                _backgroundPlan = null;
                _backgroundRoot = null;
                CancelBackgroundPlan();
                Report("plan", "replan limit reached, planner disabled for this turn");
                return false;
            }

            // Primeiro plano do turno (sem gatilho pendente): carteira de 3 buscas em paralelo.
            // Depois: uma busca só, porque o estado já está adiantado e a busca é curta.
            if (!_portfolioUsedThisTurn && real.Pending.Length == 0)
            {
                _portfolioUsedThisTurn = true;
                _plan = TakeBackgroundPlan(real, blocked) ?? RdaPlanner.PlanPortfolio(real, PortfolioSearches.Take(PortfolioCount).ToList(), blocked);
            }
            else
            {
                // Recálculo rápido: tenta reaproveitar o que faltava do plano anterior antes de buscar de novo.
                RdaPlan repaired = _plan != null ? RdaPlanner.RepairPlan(real, _plan, Math.Max(0, _nextStep - 1), blocked) : null;
                _plan = repaired ?? RdaPlanner.PlanPortfolio(real, ReplanSearches, blocked);
            }
            _nextStep = 0;
            _doneSteps.Clear();

            KnowledgeCheckPlanCollapse();
            Report("plan", string.Format("new plan #{0}: {1} steps, tier {2}, score {3:0.#}, search {4}, {5} ms | risks: exposure {6:0.#}, cards given {7}, nibiru {8}{9} | {10}",
                _replansThisTurn, _plan.Steps.Count, _plan.Tier, _plan.Score, _plan.Search, _plan.ElapsedMs,
                _plan.Exposure, _plan.DrawEvents, _plan.NibiruPenalty.ToString("0.#") + (_plan.NibiruExposed ? " (unprotected)" : ""),
                DescribeFlags(real.Flags), real.Describe()));
            // Perfil da busca: em que etapa o tempo foi gasto, somado sobre todas as buscas da carteira. Medido fora do
            // jogo a divisão era successors 49%, evaluate 27%, nodes 21%, sort 6% — esta linha existe para confirmar (ou
            // desmentir) isso em partida de verdade, onde o estado é outro. Os ms somam mais que o tempo de parede porque
            // as buscas rodam em paralelo.
            if (_plan.PoolCount > 0 && _plan.PoolGenerated > 0)
            {
                long cpu = _plan.PoolSuccessorsMs + _plan.PoolNodesMs + _plan.PoolEvaluateMs + _plan.PoolSortMs;
                if (cpu <= 0) cpu = 1;
                Report("plan", string.Format(
                    "profile: {0} searches, {1:N0} states, {2:N0} moves | successors {3}% evaluate {4}% nodes {5}% sort {6}%"
                    + " | cpu total {7} ms, slowest search {8} ms, wall {9} ms, outside the search ~{10} ms",
                    _plan.PoolCount, _plan.PoolExpanded, _plan.PoolGenerated,
                    100 * _plan.PoolSuccessorsMs / cpu, 100 * _plan.PoolEvaluateMs / cpu,
                    100 * _plan.PoolNodesMs / cpu, 100 * _plan.PoolSortMs / cpu,
                    cpu, _plan.PoolMaxSearchMs, _plan.ElapsedMs,
                    Math.Max(0, _plan.ElapsedMs - _plan.PoolMaxSearchMs)));
                // As quatro fases rodam em SEQUÊNCIA, então aqui os ms somam para o tempo de parede. "outros" é o que
                // sobrou sem cronômetro (avaliação do atalho, semeadura, isca, atraso de invocações).
                long phaseTotal = _plan.PhaseGuidedMs + _plan.PhaseSearchMs + _plan.PhaseForcedMs + _plan.PhaseResilientMs
                    + _plan.PhaseShortcutMs + _plan.PhaseSeedsMs;
                Report("plan", string.Format(
                    "phases: routes {0} ms, shortcut {1} ms, searches {2} ms, seeds {3} ms, alternative {4} ms,"
                    + " resilience {5} ms, others {6} ms (wall {7} ms)",
                    _plan.PhaseGuidedMs, _plan.PhaseShortcutMs, _plan.PhaseSearchMs, _plan.PhaseSeedsMs,
                    _plan.PhaseForcedMs, _plan.PhaseResilientMs,
                    Math.Max(0, _plan.ElapsedMs - phaseTotal), _plan.ElapsedMs));
            }
            if (_plan.ResilienceNote != null)
                Report("plan", "resilience: " + _plan.ResilienceNote);
            if (_plan.BaitNote != null)
                Report("plan", "bait: " + _plan.BaitNote);
            // Estado completo usado pelo plano: permite reproduzir a busca fora do jogo com os mesmos dados.
            LogNote("plan_root", string.Format("hand={0} deck={1} grave={2} banished={3} extra={4} field={5} spells={6} used={7} ns={8} ens={9} nospecial={10} flags={11} pending={12} summoned={13}",
                string.Join(",", real.Hand), string.Join(",", real.Deck), string.Join(",", real.Grave), string.Join(",", real.Banished),
                string.Join(",", real.Extra), string.Join(",", real.Field), string.Join(",", real.Spells), string.Join(",", real.Used),
                real.NormalSummon, real.ExtraNormalSummon, real.NoSpecial, real.Flags, string.Join(",", real.Pending), real.SummonedThisTurn)
                + (real.EnemyHand.Length > 0 ? " enemyhand=" + string.Join(",", real.EnemyHand) : "")
                + (real.ForcedRace != 0 ? " forcedrace=" + real.ForcedRace : "")
                + (real.ForcedAttribute != 0 ? " forcedattr=" + real.ForcedAttribute : ""));
            for (int i = 0; i < _plan.Steps.Count; ++i)
                LogNote("plan", string.Format("{0,2}. {1}", i + 1, _plan.Steps[i].Action.Text));
            return true;
        }

        private static string DescribeFlags(int flags)
        {
            var parts = new List<string>();
            if ((flags & RdaState.FlagNoDeckAdd) != 0) parts.Add("Droll");
            if ((flags & RdaState.FlagMaxxC) != 0) parts.Add("Maxx C");
            if ((flags & RdaState.FlagFuwalos) != 0) parts.Add("Fuwalos");
            return parts.Count == 0 ? "" : " | opponent actives: " + string.Join(", ", parts);
        }

        // Chave de bloqueio do efeito inteiro (carta + efeito), independente dos alvos escolhidos.
        private static string EffectBlockKey(PlanAction action)
        {
            if (action == null || action.Kind != PlanKind.Activate || action.Effect == RdaKey.None)
                return null;
            return "effect:" + action.CardId + ":" + action.Effect;
        }

        // Próximo passo que o menu pode executar. Gatilhos são pulados: eles só aparecem na chain.
        private PlanStep NextIdleStep(out int index)
        {
            for (index = _nextStep; index < _plan.Steps.Count; ++index)
            {
                PlanKind kind = _plan.Steps[index].Action.Kind;
                if (_doneSteps.Contains(index) || kind == PlanKind.TriggerAccept || kind == PlanKind.TriggerDecline)
                    continue;
                return _plan.Steps[index];
            }
            return null;
        }

        // PURPOSE: conferir no menu atual se o jogo oferece a ação do passo.
        private bool IsAvailableNow(PlanAction action)
        {
            MainPhase main = Duel.MainPhase;
            switch (action.Kind)
            {
                case PlanKind.Activate:
                    for (int i = 0; i < main.ActivableCards.Count; ++i)
                        if (MatchesAction(main.ActivableCards[i], main.ActivableDescs[i], action))
                            return true;
                    return false;
                case PlanKind.SpecialProc:
                    return main.SpecialSummonableCards.Any(card => CardCode(card) == action.CardId && card.Location == action.From);
                case PlanKind.Synchro:
                    return main.SpecialSummonableCards.Any(card => CardCode(card) == action.CardId && card.Location == CardLocation.Extra)
                        && FindMaterials(action.Materials) != null;
                case PlanKind.NormalSummon:
                case PlanKind.ExtraNormalSummon:
                    return main.SummonableCards.Any(card => CardCode(card) == action.CardId);
            }
            return false;
        }

        // -----------------------------------------------------------------------------------------------------
        // 2.4 Identificação de efeitos: carta + local + descrição
        // -----------------------------------------------------------------------------------------------------

        // Índice do texto (str) de cada efeito, quando a carta tem mais de um efeito no mesmo local.
        // -1 = sem descrição conhecida (aceita qualquer uma).
        private static int ExpectedDescriptionIndex(RdaKey key)
        {
            switch (key)
            {
                case RdaKey.PowerViceSS: return 0;
                case RdaKey.PowerViceSearch: return 1;
                case RdaKey.StoneSweeperSearch: return 1;
                case RdaKey.DarknessSS: return 0;
                case RdaKey.DarknessExtraNs: return 1;
                case RdaKey.DarknessLevel: return 2;
                case RdaKey.FiendPieceSS: return 0;
                case RdaKey.FiendPieceLevel: return 1;
                case RdaKey.FiendPieceRevive: return 2;
                case RdaKey.SynkronAdd: return 0;          // confirmado nos logs (o 1 do cdb não é o gatilho)
                case RdaKey.QuetzacoatlRevive: return 0;   // índice 1 é a negação
                case RdaKey.BladeTake: return 0;           // índice 1 é a destruição
                case RdaKey.CrimsonCall: return 0;
                case RdaKey.KingSearch: return 0;          // índice 1 é o efeito rápido (banir e invocar RDA)
                case RdaKey.RedZoneRevive: return 1;       // índice 0 é a destruição (resposta ao oponente)
                case RdaKey.StormBaneGraveSummon: return 2; // 0 = não pode ser alvo, 1 = banir o campo dele
            }
            return -1;
        }

        private bool DescriptionMatches(RdaKey key, ClientCard card, int desc)
        {
            int expected = ExpectedDescriptionIndex(key);
            if (expected < 0 || desc <= 0)
                return true;
            // Descrições de sistema (ex.: 221, gatilho genérico) não pertencem à carta: aceita.
            if (desc / 16 != CardCode(card))
                return true;
            return desc == Util.GetStringId(CardCode(card), expected);
        }

        private bool MatchesAction(ClientCard card, int desc, PlanAction action)
        {
            return card != null && CardCode(card) == action.CardId && card.Location == action.From
                && DescriptionMatches(action.Effect, card, desc);
        }

        // Qual gatilho do modelo esta carta está oferecendo na chain.
        private RdaKey IdentifyTrigger(ClientCard card, int desc)
        {
            foreach (KeyValuePair<RdaKey, RdaPick> entry in RdaRules.TriggerSource)
            {
                if (entry.Value.Id == CardCode(card) && entry.Value.Location == card.Location && DescriptionMatches(entry.Key, card, desc))
                    return entry.Key;
            }
            return RdaKey.None;
        }

        // -----------------------------------------------------------------------------------------------------
        // 2.5 Executores do plano
        // -----------------------------------------------------------------------------------------------------

        // PURPOSE: efeitos ativados. No menu, aceita só a carta do passo atual; na chain, responde gatilhos.
        private bool PlanActivate()
        {
            if (Duel.Player != 0)
                return false; // turno do oponente: fase 2

            if (_prompt == PromptKind.Idle)
            {
                if (_idleStep == null || _idleStep.Action.Kind != PlanKind.Activate || !MatchesAction(Card, ActivateDescription, _idleStep.Action))
                    return false;
                return CommitIdleStep();
            }

            return DecideTrigger(Card, ActivateDescription);
        }

        // PURPOSE: procedimentos de Invocação-Especial (Vision, Synkron, Lubellion) e Synchro.
        private bool PlanSpecialSummon()
        {
            if (_prompt != PromptKind.Idle || _idleStep == null)
                return false;

            PlanAction action = _idleStep.Action;
            if (action.Kind == PlanKind.SpecialProc)
            {
                if (CardCode(Card) != action.CardId || Card.Location != action.From)
                    return false;
                return CommitIdleStep();
            }

            if (action.Kind == PlanKind.Synchro)
            {
                if (CardCode(Card) != action.CardId || Card.Location != CardLocation.Extra)
                    return false;
                List<ClientCard> materials = FindMaterials(action.Materials);
                if (materials == null)
                    return false;
                // Os prompts de material (SelectCard/SelectUnselect/SelectSum com hint SynchroMaterial)
                // escolhem exatamente estas cartas.
                AI.SelectMaterials(materials);
                return CommitIdleStep();
            }
            return false;
        }

        // PURPOSE: Invocação-Normal e a Invocação-Normal extra do Darkness Resonator.
        private bool PlanNormalSummon()
        {
            if (_prompt != PromptKind.Idle || _idleStep == null)
                return false;
            PlanAction action = _idleStep.Action;
            if ((action.Kind != PlanKind.NormalSummon && action.Kind != PlanKind.ExtraNormalSummon) || CardCode(Card) != action.CardId)
                return false;
            return CommitIdleStep();
        }

        // -----------------------------------------------------------------------------------------------------
        // 2.6c Banimento dos novas (jogador, 2026-09-14)
        //   Hypernova: efeito rápido a qualquer momento; bane a si mesmo e o campo + GY do oponente.
        //   Supernova: só quando o oponente ativa efeito de monstro ou declara ataque; bane a si mesmo e o campo dele;
        //              volta sozinho no nosso End Phase seguinte.
        //   Dis Pater (1) e Red Zone (2) trazem o nova banido de volta. Nunca gastar os dois novas no mesmo turno.
        //   Mirrorjade the Iceblade Dragon: se sai do campo por carta nossa (banido também), destrói todos os nossos
        //   monstros no End Phase: só tirar se for letal.
        // -----------------------------------------------------------------------------------------------------
        private static readonly int[] MirrorjadeIds = { 44146295, 44146296 };
        private const int NovaOriginalAttack = 4500; // ATK original do Red Hypernova Dragon (cards.cdb)

        // Mirrorjade só é ameaça se não tivermos como parar o efeito do End Phase dele (jogador): negação da Quetzacoatl
        // (com outro Dragão Synchro para devolver), negação da Dis Pater, Zalen com Crimson King (o King (2) abre a chain
        // para o Zalen negar o efeito anterior) ou Soul Resonator no GY (bane no lugar da destruição; precisa controlar
        // RDA ou um Synchro que o mencione).
        private bool MirrorjadeIsThreat()
        {
            if (!Enemy.GetMonsters().Any(card => card != null && card.IsFaceup() && MirrorjadeIds.Contains(card.Id)))
                return false;
            var monsters = Bot.GetMonsters().Where(card => card != null && card.IsFaceup() && !card.IsDisabled()).ToList();
            bool quetzacoatl = monsters.Any(card => CardCode(card) == CardId.CrimsonDragonQuetzacoatl)
                && monsters.Count(card => card.HasType(CardType.Synchro) && card.HasRace(CardRace.Dragon)) >= 2;
            bool disPater = monsters.Any(card => CardCode(card) == CardId.BystialDisPater);
            bool zalenKing = monsters.Any(card => CardCode(card) == CardId.ZalenTheShackledDragon)
                && monsters.Any(card => CardCode(card) == CardId.TheCrimsonKing);
            bool soul = Bot.Graveyard.Any(card => card != null && CardCode(card) == CardId.SoulResonator)
                && monsters.Any(card => CardCode(card) == CardId.RedDragonArchfiend || CardCode(card) == CardId.ScarredDragonArchfiend
                    || (card.HasType(CardType.Synchro) && RdaCards.Get(CardCode(card)) != null && RdaCards.Get(CardCode(card)).MentionsRda));
            return !(quetzacoatl || disPater || zalenKing || soul);
        }

        // Linha RDA + Crimson Gaia (jogador): com a Gaia com a face para cima e 2+ monstros do oponente, o RDA ataca
        // primeiro; a Gaia vira os monstros dele com a face para baixo em defesa e, depois do cálculo de dano, o RDA destrói
        // todos os monstros em defesa. Não vale no 1º turno (sem batalha) nem com Mirrorjade ameaçando.
        private bool GaiaRdaLineReady()
        {
            // 1 monstro do oponente basta, igual ao battleWipeReady (que passou de 2 para 1 em 2026-09-17). Os dois
            // estavam fora de sincronia: contra UM monstro só a nota já queria o RDA na mesa e este portão continuava
            // barrando a troca que o traz de volta (jogador, 2026-09-22).
            if (Duel.Turn == 1 || Enemy.GetMonsters().Count < 1 || MirrorjadeIsThreat())
                return false;
            return Bot.SpellZone.Any(card => card != null && card.IsFaceup() && !card.IsDisabled() && CardCode(card) == CardId.CrimsonGaia
                && !infiniteImpermanenceNegatedColumns.Contains(card.Sequence));
        }

        private bool CanReviveBanishedNova()
        {
            bool disPater = Bot.GetMonsters().Any(card => card != null && card.IsFaceup() && !card.IsDisabled() && CardCode(card) == CardId.BystialDisPater)
                && !_usedThisTurn.Contains(RdaKey.DisPaterEffect);
            bool redZone = !_usedThisTurn.Contains(RdaKey.RedZoneRevive) && Bot.SpellZone.Any(card => card != null && card.IsFaceup() && !card.IsDisabled() && CardCode(card) == CardId.RedZone
                && !infiniteImpermanenceNegatedColumns.Contains(card.Sequence));
            return disPater || redZone;
        }

        // PURPOSE: nosso turno, combo pronto, antes da batalha: Hypernova limpa o campo e o GY do oponente se isso dá letal,
        //          ou se ele tem 2+ cartas e o Hypernova pode voltar (fica disponível de novo no turno dele).
        private bool NovaClearBoard()
        {
            if (Duel.Player != 0 || Duel.Phase != DuelPhase.Main1 || Duel.Turn == 1 || _novaBanishThisTurn || !(_comboDone || _plannerDisabledThisTurn))
                return false;
            if (CardCode(Card) != CardId.RedHypernovaDragon || Card.Location != CardLocation.MonsterZone || Card.IsDisabled() || !IsEffectDescription(Card, ActivateDescription, 0))
                return false;
            int enemyCards = Enemy.GetMonsters().Count + Enemy.GetSpells().Count;
            if (enemyCards == 0)
                return false;
            bool revive = CanReviveBanishedNova();
            long damage = 0;
            foreach (ClientCard monster in Bot.GetMonsters())
            {
                if (monster == null || monster == Card || !monster.IsFaceup() || !monster.IsAttack())
                    continue;
                int attack = monster.Attack;
                if (CardCode(monster) == CardId.CrimsonDragonQuetzacoatl && !revive)
                    attack -= NovaOriginalAttack; // Quetzacoatl soma o ATK original dos outros Synchros
                damage += Math.Max(0, attack);
            }
            if (revive)
                damage += Card.Attack; // volta e ataca também
            bool lethal = damage >= Enemy.LifePoints;
            if (MirrorjadeIsThreat() && !lethal)
                return false;
            // Monstros que refletem o dano ou não saem em batalha travam os ataques (jogador): o banimento sem alvo resolve, se
            // não houver outra resposta (linha RDA + Gaia, Impermanence) ou se o Hypernova puder voltar.
            bool blockers = BattleBlockers().Count > 0 && (revive || !HasOtherBattleBlockerAnswer());
            if (!lethal && !(enemyCards >= 2 && revive) && !blockers)
                return false;
            _novaBanishThisTurn = true;
            Report("interaction", string.Format("Hypernova banishes the opponent field and GY before the battle ({0}, possible damage {1} against {2} LP)",
                lethal ? "lethal" : blockers ? "monsters that reflect damage or survive battle" : "comes back with Dis Pater/Red Zone", damage, Enemy.LifePoints));
            return true;
        }

        // PURPOSE: Soul Resonator no GY: quando uma carta nossa seria destruída por efeito (Raigeki, Heavy Storm, Mirrorjade,
        //          até o End Phase do nosso RDA), bane o Soul no lugar. O jogo só oferece nessa situação. Poupa as negações e
        //          o Soul banido vira alvo para a Dis Pater (2) embaralhar.
        private bool SoulGraveProtection()
        {
            if (CardCode(Card) != CardId.SoulResonator || Card.Location != CardLocation.Grave)
                return false;
            Report("interaction", "Soul Resonator (GY) is banished instead of our cards being destroyed");
            return true;
        }

        // PURPOSE: Crimson Gaia na nossa batalha: quando o RDA declara ataque (vira os monstros do oponente para defesa com a
        //          face para baixo) ou quando um monstro é destruído (revive RDA do GY). Nos dois casos é vantagem usar.
        private bool GaiaBattleEffect()
        {
            if (Duel.Player != 0 || !(Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2))
                return false;
            if (CardCode(Card) != CardId.CrimsonGaia || Card.Location != CardLocation.SpellZone || !Card.IsFaceup() || Card.IsDisabled())
                return false;
            Report("interaction", "Crimson Gaia in the battle (RDA + Gaia line)");
            return true;
        }

        // PURPOSE: depois do combo, trazer o nova banido de volta (Dis Pater (1) ou Red Zone (2) com a face para cima).
        private bool ReviveBanishedNova()
        {
            if (Duel.Player == 1)
            {
                // Turno do oponente (jogador, 2026-09-15): depois que o nova baniu, a Red Zone ② traz o nova de volta para outro
                // uso. A Harmonia vem primeiro (precisa de efeito de monstro dele e de 2 zonas livres); a Red Zone espera enquanto
                // a Harmonia ainda puder entrar, exceto na End Phase.
                if (!Bot.Banished.Any(card => card != null && IsNova(card)))
                    return false;
                bool harmoniaPending = !_harmoniaUsedThisTurn && FreeMainMonsterZones() >= 2
                    && Bot.Hand.Any(card => card != null && CardCode(card) == CardId.FydraulisHarmonia)
                    && Bot.ExtraDeck.Any(card => card != null && CardCode(card) == CardId.StormBaneDragonDestorbim);
                if (harmoniaPending && Duel.Phase != DuelPhase.End)
                    return false;
            }
            else if (Duel.Player != 0 || !IsMainPhase(Duel.Phase) || !(_comboDone || _plannerDisabledThisTurn))
                return false;
            // Nova banido tem prioridade; sem ele, a Red Zone ② traz o melhor Synchro Dragão DARK banido (jogador), se houver zona.
            bool novaBanished = Bot.Banished.Any(card => card != null && (CardCode(card) == CardId.RedHypernovaDragon || CardCode(card) == CardId.RedSupernovaDragon));
            bool synchroBanished = Bot.Banished.Any(card => card != null && card.HasType(CardType.Synchro) && card.HasRace(CardRace.Dragon)
                && (card.Attribute & (int)CardAttribute.Dark) != 0);
            if ((!novaBanished && !synchroBanished) || !MainMonsterZoneFree())
                return false;
            int code = CardCode(Card);
            if (code == CardId.BystialDisPater && novaBanished && Card.Location == CardLocation.MonsterZone && !Card.IsDisabled() && !_usedThisTurn.Contains(RdaKey.DisPaterEffect))
            {
                _usedThisTurn.Add(RdaKey.DisPaterEffect);
                _novaReviveSource = Card;
                Report("interaction", "Dis Pater brings the banished nova back to the field");
                return true;
            }
            if (code == CardId.RedZone && Card.Location == CardLocation.SpellZone && Card.IsFaceup() && !Card.IsDisabled() && IsEffectDescription(Card, ActivateDescription, 1)
                && !_usedThisTurn.Contains(RdaKey.RedZoneRevive))
            {
                _usedThisTurn.Add(RdaKey.RedZoneRevive);
                _novaReviveSource = Card;
                Report("interaction", novaBanished ? "Red Zone brings the banished nova back to the field" : "Red Zone brings the best banished Synchro back to the field");
                return true;
            }
            return false;
        }

        // PURPOSE: turno do oponente, ataque declarado: um só nova bane para evitar o ataque (Supernova primeiro, volta sozinho).
        // Efeitos do oponente que limpam, banem ou anulam a nossa mesa sem alvo. Raigeki/Dark Hole ficam de fora: o Hypernova não
        // é destruído por eles e banir em resposta não salva os outros monstros.
        private static readonly HashSet<int> OpponentWipeCodes = new HashSet<int>
        {
            CardId.CrimsonGaia,          // efeito 1: vira nossos monstros para defesa com a face para baixo (o RDA destrói depois)
            CardId.RedDragonArchfiend,   // destrói todos os nossos monstros em defesa depois do cálculo de dano
            54693926,                    // Dark Ruler No More: nega os efeitos dos nossos monstros
            15693423,                    // Evenly Matched: bane nossas cartas com a face para baixo
            24299458,                    // Forbidden Droplet: nega efeitos e corta ATK
            CardId.KingsResonance,       // 5+: vira nossos monstros para defesa com a face para baixo
            CardId.RedHypernovaDragon,   // bane nosso campo e GY
            CardId.RedSupernovaDragon, 99585850, // bane todas as nossas cartas
            CardId.StormBaneDragonDestorbim,     // bane nossas cartas
        };

        // PURPOSE: um nova bane em resposta a um efeito do oponente que limparia ou anularia a nossa mesa, em qualquer fase
        //          (Hypernova: a qualquer momento; Supernova: só contra efeito de monstro, o jogo decide). Um nova por turno.
        private bool NovaVsWipe()
        {
            if (_novaBanishInWindow || _prompt != PromptKind.Chain || Duel.CurrentChainInfo == null || Duel.CurrentChainInfo.Count == 0)
                return false;
            int code = CardCode(Card);
            if ((code != CardId.RedHypernovaDragon && code != CardId.RedSupernovaDragon) || Card.Location != CardLocation.MonsterZone
                || !Card.IsFaceup() || Card.IsDisabled())
                return false;
            if (code == CardId.RedHypernovaDragon && !IsEffectDescription(Card, ActivateDescription, 0))
                return false;
            ChainInfo last = Duel.CurrentChainInfo[Duel.CurrentChainInfo.Count - 1];
            if (last.ActivatePlayer != 1 || !(OpponentWipeCodes.Contains(last.ActivateId) || OpponentWipeCodes.Contains(last.ActivateAlias)))
                return false;
            // Crimson Gaia: só o efeito de virar para defesa (buscar e reviver não limpam a mesa).
            if (last.IsActivateCode(CardId.CrimsonGaia) && last.ActivateDescription != Util.GetStringId(CardId.CrimsonGaia, 1))
                return false;
            // RDA: só se temos monstro em defesa para ser destruído.
            if (last.IsActivateCode(CardId.RedDragonArchfiend) && !Bot.GetMonsters().Any(card => card != null && card.IsDefense()))
                return false;
            if (code == CardId.RedHypernovaDragon && OfferedNow(CardId.RedSupernovaDragon))
                return false;
            _novaBanishInWindow = true;
            Report("interaction", string.Format("{0} banishes in response to {1} (it would clear or negate our board)",
                code == CardId.RedSupernovaDragon ? "Supernova" : "Hypernova",
                last.RelatedCard != null ? (last.RelatedCard.Name ?? last.ActivateId.ToString()) : last.ActivateId.ToString()));
            return true;
        }

        // Regra do jogador (2026-09-15): se o oponente entra na Battle Phase com pelo menos 1 carta no campo, o nova bane, não
        // importa o ataque, o alvo ou a conta de ATK (teste: RDA + Crimson Gaia limparam a nossa mesa com o Hypernova parado).
        // Hypernova a qualquer momento da batalha; Supernova quando o jogo oferecer (ataque declarado ou efeito de monstro).
        // Um nova por janela; com os dois oferecidos, Supernova primeiro (volta sozinho no nosso End Phase). O limite de 1 por
        // turno é do próprio card: o nova que volta do banimento é outra carta para o jogo e pode usar de novo.
        private bool NovaVsAttack()
        {
            if (Duel.Player != 1 || !(Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2) || _novaBanishInWindow)
                return false;
            if (Card.Location != CardLocation.MonsterZone || !Card.IsFaceup() || Card.IsDisabled())
                return false;
            int code = CardCode(Card);
            if (code != CardId.RedHypernovaDragon && code != CardId.RedSupernovaDragon)
                return false;
            if (code == CardId.RedHypernovaDragon && !IsEffectDescription(Card, ActivateDescription, 0))
                return false;
            if (code == CardId.RedHypernovaDragon && OfferedNow(CardId.RedSupernovaDragon))
                return false;
            int enemyCards = Enemy.GetMonsters().Count(card => card != null) + Enemy.GetSpells().Count(card => card != null);
            if (enemyCards == 0)
                return false;
            _novaBanishInWindow = true;
            Report("interaction", string.Format("{0} banishes the opponent field during their Battle Phase ({1} card(s) on their field)",
                code == CardId.RedSupernovaDragon ? "Supernova" : "Hypernova", enemyCards));
            return true;
        }

        // Regra do jogador (2026-09-15, replay contra Daru): no turno do oponente o Hypernova ficou parado fora da Battle Phase
        // e o oponente montou a mesa inteira. Conta o que ele coloca no campo desde o último banimento do nova e bane quando:
        //   3+ monstros do Extra Deck, 6+ monstros no total, ou na End Phase com 2+ monstros ou 2+ magias/armadilhas.
        // Os contadores zeram quando o efeito do nova resolve (OnChainSolved): o nova pode voltar (Harmonia + Storm-Bane,
        // Red Zone) e o oponente pode continuar jogando. Hypernova a qualquer momento; Supernova só quando o jogo oferecer
        // (efeito de monstro ou ataque). Um nova por janela; Supernova primeiro.
        private bool NovaVsOpponentPlay()
        {
            if (Duel.Player != 1 || _novaBanishInWindow)
                return false;
            int code = CardCode(Card);
            if ((code != CardId.RedHypernovaDragon && code != CardId.RedSupernovaDragon) || Card.Location != CardLocation.MonsterZone
                || !Card.IsFaceup() || Card.IsDisabled())
                return false;
            if (code == CardId.RedHypernovaDragon && !IsEffectDescription(Card, ActivateDescription, 0))
                return false;
            if (code == CardId.RedHypernovaDragon && OfferedNow(CardId.RedSupernovaDragon))
                return false;
            int monsters = Enemy.GetMonsters().Count(card => card != null);
            int spells = Enemy.GetSpells().Count(card => card != null);
            if (monsters + spells == 0)
                return false;
            string reason;
            if (_opponentExtraSummonsSinceNova >= 3)
                reason = _opponentExtraSummonsSinceNova + " Extra Deck monsters";
            else if (_opponentSummonsSinceNova >= 6)
                reason = _opponentSummonsSinceNova + " monsters summoned";
            else if (Duel.Phase == DuelPhase.End && (monsters >= 2 || spells >= 2))
                reason = string.Format("End Phase with {0} monster(s) and {1} spell(s)/trap(s)", monsters, spells);
            else
                return false;
            _novaBanishInWindow = true;
            Report("interaction", string.Format("{0} banishes the opponent field on their turn ({1} since the last banish)",
                code == CardId.RedSupernovaDragon ? "Supernova" : "Hypernova", reason));
            return true;
        }

        // -----------------------------------------------------------------------------------------------------
        // 2.6d Efeitos fora do plano (auditoria carta por carta, 2026-09-14)
        //   Red Dragon Archfiend's Chain: como a Impermanence; no nosso turno antes do combo contra ameaças do campo dele
        //     (da mão pode baixar e ativar no mesmo turno revelando o RDA do Extra); no turno dele para parar o combo ou
        //     contra efeito de monstro que mira nossas cartas. Alvos = Tuners revelados + 1.
        //   King's Resonance (jogador): espera 5+ Invocações-Especiais do oponente (vira os monstros dele na batalha); 10+
        //     ativa na hora (ninguém responde); com 3 a 4 só como proteção do RDA/Scarred no campo, em resposta a um efeito
        //     dele. O revive sozinho (1+) não justifica a ativação.
        //   Storm-Bane ①: contra ameaça (combo dele ou efeito mirando nossas cartas) ou para dar letal; ② sempre.
        //   Scarred ② e Crimson Gaia ③ no turno do oponente: sempre trazem o RDA (jogador).
        //   Batalha: Crimson Call ② (segundo ataque), Crimson Blade ② (destrói nível 5+), Abyss ② (revive Tuner só se o
        //     planejador achar um Synchro na Main Phase 2 que melhora a mesa; o plano é refeito na Main Phase 2).
        // -----------------------------------------------------------------------------------------------------
        private void ClearAuditSelections()
        {
            _bestReviveSource = null;
            _rdaChainActivating = false;
            _stormBaneBanishing = false;
            _abyssRevive = false;
            _magnamhutTarget = null;
            _etudeTarget = 0;
        }

        // Monstro do oponente que a RDA's Chain pode negar: com a face para cima, de efeito, não negado e que aceita alvo.
        private static bool RdaChainCanTarget(ClientCard card)
        {
            return card != null && card.Controller == 1 && card.Location == CardLocation.MonsterZone && card.IsFaceup()
                && card.HasType(CardType.Effect) && !card.IsDisabled() && !card.IsShouldNotBeTarget() && !card.IsShouldNotBeSpellTrapTarget();
        }

        // Ameaças em ordem: listas conhecidas (negar antes de usar, floodgate, perigoso), monstros do Extra (costumam ter
        // negação), maior ATK.
        private List<ClientCard> RdaChainThreats()
        {
            return Enemy.GetMonsters().Where(RdaChainCanTarget)
                .OrderByDescending(EnemyTargetRank)
                .ToList();
        }

        private bool RdaChainBeforeComboWanted()
        {
            if (_rdaChainBeforeComboDone || Duel.Player != 0 || Duel.Phase != DuelPhase.Main1 || Duel.Turn == 1 || _plan != null)
                return false;
            if (RdaChainThreats().Count == 0)
                return false;
            if (Bot.SpellZone.Any(card => card != null && card.IsFacedown() && CardCode(card) == CardId.RedDragonArchfiendsChain
                && !infiniteImpermanenceNegatedColumns.Contains(card.Sequence)))
                return true;
            return Bot.Hand.Any(card => card != null && CardCode(card) == CardId.RedDragonArchfiendsChain)
                && Bot.ExtraDeck.Any(card => card != null && CardCode(card) == CardId.RedDragonArchfiend)
                && Bot.SpellZone.Take(5).Count(card => card != null) < 5;
        }

        // PURPOSE: baixar a RDA's Chain da mão para ativar no mesmo turno, antes do combo.
        private bool RdaChainSetBeforeCombo()
        {
            if (CardCode(Card) != CardId.RedDragonArchfiendsChain || Card.Location != CardLocation.Hand || !RdaChainBeforeComboWanted())
                return false;
            if (Bot.SpellZone.Any(card => card != null && card.IsFacedown() && CardCode(card) == CardId.RedDragonArchfiendsChain))
                return false;
            Report("interaction", "sets RDA's Chain to negate threats before the combo");
            return true;
        }

        // PURPOSE: ativar a RDA's Chain antes do combo ou contra o efeito de um monstro do oponente.
        private bool RdaChainActivate()
        {
            if (CardCode(Card) != CardId.RedDragonArchfiendsChain || Card.Location != CardLocation.SpellZone
                || Card.IsDisabled() || infiniteImpermanenceNegatedColumns.Contains(Card.Sequence))
                return false;
            List<ClientCard> threats = RdaChainThreats();
            if (threats.Count == 0)
                return false;

            string reason;
            if (Duel.Player == 0 && _prompt == PromptKind.Idle && RdaChainBeforeComboWanted())
            {
                _rdaChainBeforeComboDone = true;
                reason = "before the combo";
            }
            else if (_prompt == PromptKind.Chain && Duel.CurrentChainInfo != null && Duel.CurrentChainInfo.Count > 0)
            {
                ChainInfo last = Duel.CurrentChainInfo[Duel.CurrentChainInfo.Count - 1];
                ClientCard source = last.RelatedCard;
                // RdaChainCanTarget já exige monstro no campo, então aqui o tipo sempre veio do servidor; usa o mesmo
                // teste do ChooseResponse só para não haver duas formas do mesmo idioma no arquivo.
                if (last.ActivatePlayer != 1 || !IsMonsterActivation(last) || !RdaChainCanTarget(source))
                    return false;
                bool targetsOurs = Duel.ChainTargets.Any(card => card != null && card.Controller == 0);
                bool combo = Duel.Player == 1 && _opponentActivationsThisTurn >= 2;
                if (!targetsOurs && !combo && !source.IsMonsterShouldBeDisabledBeforeItUseEffect())
                    return false;
                threats.Remove(source);
                threats.Insert(0, source);
                reason = targetsOurs ? "against an effect aimed at our cards" : "against the opponent combo";
            }
            else
                return false;

            _rdaChainActivating = true;
            _rdaChainTargets = threats;
            Report("interaction", string.Format("RDA's Chain negates {0} ({1})", string.Join(", ", threats.Select(card => card.Name ?? card.Id.ToString())), reason));
            return true;
        }

        // Melhor monstro nosso para voltar ao campo: Hypernova, Supernova, depois o de maior valor de mesa.
        private static double ReviveRank(ClientCard card)
        {
            int code = CardCode(card);
            if (code == CardId.RedHypernovaDragon) return 1000;
            if (code == CardId.RedSupernovaDragon) return 900;
            return RdaEvaluator.BoardValue(code);
        }

        private bool MainMonsterZoneFree()
        {
            return Bot.MonsterZone.Take(5).Count(card => card != null) < 5;
        }

        private int FreeMainMonsterZones()
        {
            return 5 - Bot.MonsterZone.Take(5).Count(card => card != null);
        }

        private static bool IsNova(ClientCard card)
        {
            int code = CardCode(card);
            return code == CardId.RedHypernovaDragon || code == CardId.RedSupernovaDragon;
        }

        // PURPOSE: King's Resonance baixada, no turno do oponente, conforme o número de Invocações-Especiais dele.
        private bool KingsResonanceActivate()
        {
            if (CardCode(Card) != CardId.KingsResonance || Card.Location != CardLocation.SpellZone || Duel.Player != 1
                || infiniteImpermanenceNegatedColumns.Contains(Card.Sequence))
                return false;
            // Regra do jogador (2026-09-15, partida contra Yubel: foi ativada com o oponente quase sem monstros): o King's
            // Resonance serve para interromper combo de 5+ ou 10+ Invocações-Especiais. Fora disso, só num caso de desespero.
            int summons = _opponentSpecialSummonsThisTurn;
            if (summons < 3)
                return false;

            string reason;
            if (summons >= 10)
                reason = "10+ summons: nobody can respond";
            else if (summons >= 5)
                reason = "5+ summons: flips their monsters face-down";
            else
            {
                // Desespero (3+ invocações): o oponente está ativando algo que destrói todos os monstros do campo, não temos o
                // Soul Resonator no GY para substituir a destruição, temos um RDA que NÃO veio do efeito do Crimson King (esse já
                // é imune) e a Crimson Gaia não está no campo. Só então o 3+ (RDA imune aos efeitos dele) vale a carta.
                if (Bot.Graveyard.Any(card => card != null && CardCode(card) == CardId.SoulResonator))
                    return false;
                if (Bot.SpellZone.Any(card => card != null && card.IsFaceup() && CardCode(card) == CardId.CrimsonGaia))
                    return false;
                bool ownRda = Bot.GetMonsters().Any(card => card != null && card.IsFaceup() && !_kingRdaCards.Contains(card)
                    && (CardCode(card) == CardId.RedDragonArchfiend || CardCode(card) == CardId.ScarredDragonArchfiend));
                if (!ownRda || !OpponentMassDestructionNow())
                    return false;
                reason = "3+ summons and an effect that would destroy every monster: protects the RDA";
            }
            _bestReviveSource = Card;
            Report("interaction", string.Format("King's Resonance ({0} opponent Special Summons, {1})", summons, reason));
            return true;
        }

        // O elo do oponente em ativação destrói (ou bane) todos os monstros do campo? Lido do texto no cards.cdb.
        private static readonly System.Text.RegularExpressions.Regex MassDestructionText = new System.Text.RegularExpressions.Regex(
            @"destroy all (?:other )?(?:face-up |face-down )?(?:attack position |defense position )?monsters|destroy as many (?:monsters|cards) (?:your opponent controls|on the field)|banish all (?:cards|monsters) your opponent controls|destroy all cards (?:your opponent controls|on the field)",
            System.Text.RegularExpressions.RegexOptions.Compiled);

        private bool OpponentMassDestructionNow()
        {
            if (_prompt != PromptKind.Chain || Duel.CurrentChainInfo == null || Duel.CurrentChainInfo.Count == 0)
                return false;
            ChainInfo last = Duel.CurrentChainInfo[Duel.CurrentChainInfo.Count - 1];
            if (last.ActivatePlayer != 1)
                return false;
            int id = last.ActivateId != 0 ? last.ActivateId : last.ActivateAlias;
            if (id == 0)
                return false;
            YGOSharp.OCGWrapper.NamedCard data = YGOSharp.OCGWrapper.NamedCard.Get(id);
            return data != null && data.Description != null && MassDestructionText.IsMatch(data.Description.ToLowerInvariant());
        }

        private List<ClientCard> StormBaneFodder()
        {
            // Monstros DARK que não podem ser Invocados-Normal no nosso GY (os Synchros). RDA/Scarred por último:
            // são parte do Burning Soul pelo GY.
            return Bot.Graveyard.Where(card => card != null && card.HasType(CardType.Synchro) && (card.Attribute & (int)CardAttribute.Dark) != 0)
                .OrderBy(card => CardCode(card) == CardId.RedDragonArchfiend || CardCode(card) == CardId.ScarredDragonArchfiend ? 1 : 0)
                .ThenBy(card => RdaEvaluator.BoardValue(CardCode(card)))
                .ToList();
        }

        // PURPOSE: Storm-Bane ① contra ameaça (turno do oponente, efeito rápido) ou para abrir letal (nosso turno).
        private bool StormBaneBanish()
        {
            if (CardCode(Card) != CardId.StormBaneDragonDestorbim || Card.Location != CardLocation.MonsterZone || Card.IsDisabled() || !IsEffectDescription(Card, ActivateDescription, 1))
                return false;
            List<ClientCard> fodder = StormBaneFodder();
            if (fodder.Count == 0)
                return false;

            var targets = new List<ClientCard>();
            string reason;
            if (_prompt == PromptKind.Chain && Duel.CurrentChainInfo != null && Duel.CurrentChainInfo.Count > 0)
            {
                ChainInfo last = Duel.CurrentChainInfo[Duel.CurrentChainInfo.Count - 1];
                if (last.ActivatePlayer != 1)
                    return false;
                bool targetsOurs = Duel.ChainTargets.Any(card => card != null && card.Controller == 0);
                bool combo = Duel.Player == 1 && _opponentActivationsThisTurn >= 2;
                if (!targetsOurs && !combo && Duel.Player != 0)
                    return false;
                // Como a Red Zone: a carta que ativa e sai do campo não vale o custo; mira o melhor monstro que fica.
                ClientCard source = last.RelatedCard;
                ClientCard best = source != null && source.Controller == 1 && source.Location == CardLocation.MonsterZone
                    && (!BenefitsFromRemoval(source, Removal.Banish) || IsListedThreat(source))
                    ? source : BestEnemyMonster(TargetKind.NoTarget, Removal.Banish);
                if (best == null)
                    return false;
                targets.Add(best);
                reason = targetsOurs ? "against an effect aimed at our cards" : "against the opponent combo";
            }
            else if (_prompt == PromptKind.Idle && Duel.Player == 0 && Duel.Phase == DuelPhase.Main1 && Duel.Turn > 1 && (_comboDone || _plannerDisabledThisTurn))
            {
                // Hypernova limpa primeiro (bane campo e GY e pode voltar).
                if (!_novaBanishThisTurn && Bot.GetMonsters().Any(card => card != null && card.IsFaceup() && CardCode(card) == CardId.RedHypernovaDragon))
                    return false;
                var blockers = Enemy.GetMonsters().Where(card => card != null).ToList();
                if (blockers.Count == 0 || blockers.Count > fodder.Count)
                    return false;
                long damage = Bot.GetMonsters().Where(card => card != null && card.IsFaceup() && card.IsAttack()).Sum(card => (long)card.Attack);
                if (damage < Enemy.LifePoints)
                    return false;
                targets.AddRange(blockers.OrderByDescending(card => card.Attack));
                reason = string.Format("opens lethal ({0} damage against {1} LP)", damage, Enemy.LifePoints);
            }
            else
                return false;

            _stormBaneBanishing = true;
            _stormBaneCount = targets.Count;
            _stormBaneTargets = targets;
            Report("interaction", string.Format("Storm-Bane banishes {0} opponent card(s) ({1})", targets.Count, reason));
            return true;
        }

        // PURPOSE: Storm-Bane ② (foi ao GY, ex.: Harmonia): sempre traz o melhor Dragão banido (Hypernova primeiro).
        private bool StormBaneRevive()
        {
            if (CardCode(Card) != CardId.StormBaneDragonDestorbim || Card.Location != CardLocation.Grave || !IsEffectDescription(Card, ActivateDescription, 2))
                return false;
            if (!Bot.Banished.Any(card => card != null && card.HasRace(CardRace.Dragon)))
                return false;
            // O planejador agora modela este gatilho (RdaKey.StormBaneGraveSummon). Havendo passo no plano, quem decide
            // o alvo e a ordem da cadeia é ele — e o DecideTrigger já segura este elo quando outro gatilho nosso precisa
            // entrar antes. Esta regra vira reserva: só age sem plano, e aí pega o melhor banido por ReviveRank.
            if (FindTriggerStep(RdaKey.StormBaneGraveSummon) >= 0)
                return false;
            if (!StormBaneWorthTheZone())
                return false;
            _bestReviveSource = Card;
            Report("interaction", "Storm-Bane (GY) brings the best banished Dragon back (no plan step)");
            return true;
        }

        // Disputa de zona entre dois revives nossos que disparam na MESMA cadeia. O Quetzacoatl invoca "as many Dragon
        // Synchro Monsters from your GY as possible": ele enche TODA zona livre, então cada zona que o Storm-Bane gastar
        // é uma peça a menos que o Quetzacoatl coloca. Como o elo do Storm-Bane resolve antes, ele escolhe primeiro e o
        // Quetzacoatl perde a peça marginal — a de menor valor entre as que ainda caberiam.
        //
        // Duelo contra Branded em 2026-09-23 (log 002140, seq 621-640): com 2 zonas livres, o Quetzacoatl pegaria o
        // Crimson King (25) e o próprio Storm-Bane (15) do GY; o Storm-Bane gastou uma zona trazendo o Scarred (10) do
        // banimento, que naquele ponto já tinha usado o efeito dele e era só um corpo de 3000. A mesa saiu pior. O
        // jogador apontou o conflito no mesmo dia: a partida foi ganha ali, mas numa de resistência isso custa.
        //
        // Não é regra de carta: é a conta de recursos. Só gasta a zona se o banido que o Storm-Bane traz valer mais do
        // que a peça que o Quetzacoatl perde por causa dela.
        private bool StormBaneWorthTheZone()
        {
            if (FindTriggerStep(RdaKey.QuetzacoatlRevive) < 0)
                return true;
            int freeZones = FreeMainMonsterZones();
            if (freeZones <= 0)
                return true;   // sem zona livre o Quetzacoatl não coloca nada mesmo; nada a disputar

            double best = Bot.Banished.Where(card => card != null && card.HasRace(CardRace.Dragon))
                .Select(ReviveRank).DefaultIfEmpty(0).Max();
            // O que o Quetzacoatl colocaria: Synchros de Dragão no GY, na mesma ordem de valor que ele usa.
            List<double> queue = Bot.Graveyard
                .Where(card => card != null && card.HasType(CardType.Synchro) && card.HasRace(CardRace.Dragon))
                .Select(ReviveRank).OrderByDescending(value => value).ToList();
            if (queue.Count < freeZones)
                return true;   // sobra zona para os dois
            double marginal = queue[freeZones - 1];
            if (best > marginal)
                return true;
            Report("interaction", string.Format(
                "Storm-Bane holds its revive: the zone is worth more to the Quetzacoatl ({0:N0} against {1:N0})",
                marginal, best));
            return false;
        }

        // PURPOSE: Scarred ② no turno do oponente: sempre invoca o RDA do Extra (o RDA só destrói no nosso End Phase).
        private bool ScarredOpponentTurn()
        {
            if (Duel.Player != 1 || CardCode(Card) != CardId.ScarredDragonArchfiend || Card.Location != CardLocation.Grave)
                return false;
            Report("interaction", "Scarred (GY) summons the RDA from the Extra Deck on the opponent turn");
            return true;
        }

        // PURPOSE: Crimson Gaia ③ no turno do oponente (único efeito ativável fora do nosso turno): revive o RDA do GY.
        private bool GaiaReviveOpponentTurn()
        {
            if (Duel.Player != 1 || CardCode(Card) != CardId.CrimsonGaia || Card.Location != CardLocation.SpellZone || !Card.IsFaceup()
                || Card.IsDisabled() || infiniteImpermanenceNegatedColumns.Contains(Card.Sequence))
                return false;
            Report("interaction", "Crimson Gaia revives the RDA on the opponent turn");
            return true;
        }

        // PURPOSE: gatilhos de batalha que só dão vantagem.
        private bool BattleTriggers()
        {
            if (!(Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2))
                return false;
            int code = CardCode(Card);
            int desc = ActivateDescription;
            // O "desc > 0 &&" que ficava aqui matava as três regras. Nestes avisos o servidor manda desc = -1 (logs de
            // 2026-09-22: Crimson Blade em 042939 seq 418 e Crimson Call em 043140 seq 499, ambos recusados com
            // rule = null), e a condição virava "não sei qual efeito é, então recusa". O IsEffectDescription já trata
            // desc <= 0 como "serve", e a carta + a fase já identificam o efeito: na Battle Phase o Blade no campo só
            // tem a destruição, a Crimson Call no GY só tem o ataque extra e o Abyss só tem o revive.
            if (code == CardId.CrimsonCall && Card.Location == CardLocation.Grave && Duel.Player == 0 && IsEffectDescription(Card, desc, 1))
            {
                // Marca o ataque extra como pendente: ele é DO RDA e o próximo ataque tem que ser dele, senão a Call é
                // banida sem dar ataque nenhum (jogador, 2026-09-22: logs 191047 seq 726-733 e 191638 seq 831-838).
                _crimsonCallChainAttack = true;
                Report("interaction", "Crimson Call (GY): the RDA attacks again");
                return true;
            }
            if (code == CardId.CrimsonBladeDragon && Card.Location == CardLocation.MonsterZone && !Card.IsDisabled() && IsEffectDescription(Card, desc, 1))
            {
                Report("interaction", "Crimson Blade destroys the Level 5+ monster in battle");
                return true;
            }
            if (code == CardId.HotRedDragonArchfiendAbyss && Card.Location == CardLocation.MonsterZone && !Card.IsDisabled() && IsEffectDescription(Card, desc, 1))
            {
                int tuner = AbyssReviveChoice();
                if (tuner == 0)
                    return false;
                _abyssRevive = true;
                _abyssReviveCode = tuner;
                // O combo continua na Main Phase 2 com o Tuner revivido.
                _comboDone = false;
                _plan = null;
                Report("interaction", "Abyss revives " + RdaCards.Name(tuner) + " for a Synchro in Main Phase 2");
                return true;
            }
            return false;
        }

        private static readonly SearchOptions AbyssReviveSearch =
            new SearchOptions { Name = "abyss_mp2_w500", Width = 500, PerRoute = 20, BookWeight = 15, TimeLimitMs = 2500 };

        // PURPOSE: Abyss ② (jogador): só revive um Tuner se ele for usado depois da batalha num Synchro que melhora a mesa.
        //          Simula a Main Phase 2 com cada Tuner diferente do GY no campo e compara com a mesa sem ele.
        private int AbyssReviveChoice()
        {
            if (_noSpecialSummon || _plannerDisabledThisTurn || !MainMonsterZoneFree())
                return 0;
            RdaState root = ReadState();
            root.Pending = new int[0];
            var tuners = Bot.Graveyard.Where(card => card != null && card.HasType(CardType.Tuner))
                .Select(CardCode).Where(IsModeled).Distinct().ToList();
            if (tuners.Count == 0)
                return 0;

            var states = new List<RdaState> { root };
            foreach (int id in tuners)
            {
                RdaState withTuner = root.Copy();
                withTuner.Grave = RdaArray.Remove(root.Grave, id);
                withTuner.Field = RdaArray.Add(root.Field, RdaField.Encode(id, RdaCards.Get(id).Level, false, false));
                states.Add(withTuner);
            }
            var plans = new RdaPlan[states.Count];
            System.Threading.Tasks.Parallel.For(0, states.Count, i => plans[i] = RdaPlanner.Plan(states[i], AbyssReviveSearch, null));

            double baseline = plans[0].Score;
            int best = 0;
            double bestScore = baseline + 10; // ganho mínimo para valer o Tuner exposto
            for (int i = 1; i < plans.Length; ++i)
            {
                bool synchro = plans[i].Steps.Any(step => step.Action.Kind == PlanKind.Synchro);
                if (synchro && plans[i].Score >= bestScore)
                {
                    bestScore = plans[i].Score;
                    best = tuners[i - 1];
                }
            }
            Report("interaction", best == 0
                ? string.Format("Abyss declines the revive: no Tuner improves the board in Main Phase 2 (score {0:0.#})", baseline)
                : string.Format("Abyss: {0} takes the board from {1:0.#} to {2:0.#} in Main Phase 2", RdaCards.Name(best), baseline, bestScore));
            return best;
        }

        // Synchros de proteção que o Etude pode fazer no turno do oponente, com o valor de cada um.
        private double EtudeTargetValue(int code)
        {
            switch (code)
            {
                case CardId.HotRedDragonArchfiendAbyss: return 40;
                case CardId.BystialDisPater: return Enemy.Banished.Any(card => card != null && card.IsFaceup()) ? 38 : 15;
                case CardId.ZalenTheShackledDragon: return 30;
                case CardId.CrimsonDragonQuetzacoatl: return 28;
                case CardId.TheCrimsonKing: return 20;
            }
            return 0;
        }

        // Monstros que já são proteção ou boss: nunca viram material do Etude.
        private static readonly HashSet<int> EtudeKeepOnField = new HashSet<int>
        {
            CardId.RedHypernovaDragon, CardId.RedSupernovaDragon, CardId.CrimsonDragonQuetzacoatl, CardId.RedNovaDragonBurningSoul,
            CardId.BystialDisPater, CardId.HotRedDragonArchfiendAbyss, CardId.ZalenTheShackledDragon, CardId.TheCrimsonKing
        };

        // PURPOSE: Etude of the Branded no turno do oponente (proteção): baixada, ativa a carta na Standby dele; com a face para
        //          cima, em resposta a um efeito dele e sem negação nossa no campo, faz um Synchro de proteção com monstros que não
        //          são proteção (incluindo um Dragão), para ter resposta aos próximos efeitos.
        private bool EtudeResponse()
        {
            if (Duel.Player != 1 || CardCode(Card) != CardId.EtudeOfTheBranded || Card.Location != CardLocation.SpellZone
                || infiniteImpermanenceNegatedColumns.Contains(Card.Sequence))
                return false;
            if (Card.IsFacedown())
            {
                if (Duel.Phase != DuelPhase.Standby || Duel.CurrentChain.Count > 0)
                    return false;
                Report("interaction", "activates the set Etude of the Branded in the opponent Standby Phase");
                return true;
            }
            if (Card.IsDisabled() || _prompt != PromptKind.Chain || Duel.CurrentChainInfo == null || Duel.CurrentChainInfo.Count == 0
                || Duel.CurrentChainInfo[Duel.CurrentChainInfo.Count - 1].ActivatePlayer != 1)
                return false;
            if (Bot.GetMonsters().Any(card => card != null && card.IsFaceup() && !card.IsDisabled() && EtudeKeepOnField.Contains(CardCode(card))
                && CardCode(card) != CardId.RedHypernovaDragon && CardCode(card) != CardId.RedSupernovaDragon && CardCode(card) != CardId.RedNovaDragonBurningSoul))
                return false;

            RdaState state = ReadState();
            state.Pending = new int[0];
            state.NoSpecial = false;
            int bestTarget = 0;
            long[] bestMaterials = null;
            double bestValue = 0;
            foreach (RdaMove move in RdaRules.Successors(state))
            {
                PlanAction action = move.Action;
                if (action.Kind != PlanKind.Synchro || action.Materials == null)
                    continue;
                if (!action.Materials.Any(m => RdaCards.Get(RdaField.Id(m)) != null && RdaCards.Get(RdaField.Id(m)).Race == CardRace.Dragon))
                    continue;
                if (action.Materials.Any(m => EtudeKeepOnField.Contains(RdaField.Id(m))))
                    continue;
                double value = EtudeTargetValue(action.CardId);
                if (value > bestValue)
                {
                    bestValue = value;
                    bestTarget = action.CardId;
                    bestMaterials = action.Materials;
                }
            }
            if (bestTarget == 0)
                return false;
            List<ClientCard> materials = FindMaterials(bestMaterials);
            if (materials == null)
                return false;
            AI.SelectMaterials(materials);
            _etudeTarget = bestTarget;
            Report("interaction", string.Format("Etude of the Branded does {0} on the opponent turn ({1})", RdaCards.Name(bestTarget),
                string.Join(" + ", materials.Select(card => card.Name ?? card.Id.ToString()))));
            return true;
        }

        private static bool IsMagnamhutTarget(ClientCard card)
        {
            return card != null && card.Location == CardLocation.Grave && card.IsMonster()
                && (card.Attribute & ((int)CardAttribute.Light | (int)CardAttribute.Dark)) != 0;
        }

        // Quanto banir este monstro do GY do oponente tira dele (jogador): efeito a partir do GY ou revive vale mais que ATK;
        // monstro do Extra (boss que volta, material de Fusão/Synchro) também. Banir o que tem efeito "se for banido" ajuda ele.
        private static int MagnamhutTargetValue(ClientCard card)
        {
            int value = card.Level;
            YGOSharp.OCGWrapper.NamedCard data = YGOSharp.OCGWrapper.NamedCard.Get(card.Id);
            string text = data != null && data.Description != null ? data.Description : "";
            if (text.Contains("this card is in your GY") || text.Contains("this card from your GY") || text.Contains("this card in your GY")
                || text.Contains("this card is in the GY"))
                value += 100;
            else if (text.Contains("from your GY") || text.Contains("in your GY"))
                value += 40;
            if (card.HasType(CardType.Fusion) || card.HasType(CardType.Synchro))
                value += 30;
            if (text.Contains("If this card is banished"))
                value -= 200;
            return value;
        }

        // PURPOSE: Magnamhut fora do nosso combo (jogador): quando o efeito do oponente mira um monstro em qualquer GY (revive,
        //          custo, recuperar peça) ou vem de um monstro dele no GY, a Magnamhut bane esse monstro (efeito rápido se ele
        //          controla monstro). Invocada assim, também aceita a busca da End Phase.
        private bool MagnamhutResponse()
        {
            if (CardCode(Card) != CardId.BystialMagnamhut)
                return false;
            if (Duel.Player == 1 && Duel.Phase == DuelPhase.End && _magnamhutEndPhaseSearch)
                return true;
            if (Duel.Player == 1 && Card.Location == CardLocation.MonsterZone && _prompt == PromptKind.Chain
                && !Duel.CurrentChain.Any(link => link.Controller == 1))
            {
                _magnamhutEndPhaseSearch = true;
                Report("interaction", "Magnamhut schedules the End Phase search (opponent turn)");
                return true;
            }
            if (Card.Location != CardLocation.Hand || _prompt != PromptKind.Chain || !MainMonsterZoneFree())
                return false;
            if (Duel.Player == 0 && !(_comboDone || _plannerDisabledThisTurn))
                return false;
            if (Duel.CurrentChainInfo == null || Duel.CurrentChainInfo.Count == 0)
                return false;
            ChainInfo last = Duel.CurrentChainInfo[Duel.CurrentChainInfo.Count - 1];
            if (last.ActivatePlayer != 1)
                return false;
            ClientCard target = Duel.ChainTargets.FirstOrDefault(IsMagnamhutTarget);
            string reason = "target of the opponent effect in the GY";
            if (target == null && last.RelatedCard != null && last.RelatedCard.Controller == 1 && IsMagnamhutTarget(last.RelatedCard))
            {
                target = last.RelatedCard;
                reason = "opponent monster activating an effect from the GY";
            }
            if (target == null)
                return false;
            _magnamhutTarget = target;
            Report("interaction", string.Format("Magnamhut banishes {0} ({1})", target.Name ?? target.Id.ToString(), reason));
            return true;
        }

        // PURPOSE: prompts de seleção dos efeitos acima.
        private IList<ClientCard> SelectForAuditEffects(IList<ClientCard> cards, int min, int max)
        {
            if (cards == null || cards.Count == 0)
                return null;

            if (_rdaChainActivating)
            {
                // Ativação no turno em que foi baixada: revela o RDA do Extra.
                if (cards.All(card => card != null && card.Location == CardLocation.Extra))
                {
                    ClientCard rda = cards.FirstOrDefault(card => card.Controller == 0 && CardCode(card) == CardId.RedDragonArchfiend);
                    return rda != null ? new List<ClientCard> { rda } : null;
                }
                // Revela Tuners da mão só o suficiente para mirar todas as ameaças (alvos = Tuners revelados + 1).
                if (cards.All(card => card != null && card.Controller == 0 && card.Location == CardLocation.Hand))
                {
                    int wanted = Math.Min(max, Math.Max(0, _rdaChainTargets.Count - 1));
                    var reveal = cards.Where(card => card.HasType(CardType.Tuner)).Take(wanted).ToList();
                    foreach (ClientCard card in cards)
                    {
                        if (reveal.Count >= min) break;
                        if (!reveal.Contains(card)) reveal.Add(card);
                    }
                    return reveal.Count > 0 ? reveal : null;
                }
                if (cards.All(card => card != null && card.Controller == 1))
                {
                    var picks = _rdaChainTargets.Where(cards.Contains).ToList();
                    foreach (ClientCard card in cards.OrderByDescending(EnemyTargetRank))
                        if (!picks.Contains(card)) picks.Add(card);
                    _rdaChainActivating = false;
                    return picks.Take(Math.Max(min, Math.Min(max, _rdaChainTargets.Count))).ToList();
                }
            }

            if (_stormBaneBanishing)
            {
                if (cards.All(card => card != null && card.Controller == 0 && card.Location == CardLocation.Grave))
                {
                    var order = StormBaneFodder().Where(cards.Contains).ToList();
                    foreach (ClientCard card in cards)
                        if (!order.Contains(card)) order.Add(card);
                    return order.Take(Math.Max(min, Math.Min(max, _stormBaneCount))).ToList();
                }
                if (cards.All(card => card != null && card.Controller == 1))
                {
                    var picks = _stormBaneTargets.Where(cards.Contains).ToList();
                    foreach (ClientCard card in cards.OrderByDescending(card => card.IsMonster() ? card.Attack : -1))
                        if (!picks.Contains(card)) picks.Add(card);
                    _stormBaneBanishing = false;
                    return picks.Take(Math.Max(min, Math.Min(max, _stormBaneCount))).ToList();
                }
            }

            // Os blocos abaixo devolvem UMA carta, então só podem responder a um prompt que aceite uma: daí o "min <= 1".
            // Sem essa guarda eles atendiam prompts de várias cartas e o framework recusava a escolha inteira
            // ("Invalid card selection returned by executor"). Jogador, 2026-09-22 (log 153454 seq 774): o Storm-Bane ②
            // armou _bestReviveSource, a cadeia resolveu de trás para frente e o revive da Quetzacoatl (min=max=4, todos
            // os alvos no GY) caiu neste bloco, que devolveu 1 carta. Pior: a flag era consumida pelo prompt errado, então
            // o Storm-Bane depois escolhia pelo padrão do framework.
            if (min <= 1 && _bestReviveSource != null && cards.All(card => card != null && card.Controller == 0
                && (card.Location == CardLocation.Grave || card.Location == CardLocation.Removed)))
            {
                _bestReviveSource = null;
                return new List<ClientCard> { cards.OrderByDescending(ReviveRank).First() };
            }

            if (min <= 1 && _etudeTarget != 0 && cards.All(card => card != null && card.Controller == 0 && card.Location == CardLocation.Extra))
            {
                ClientCard synchro = cards.FirstOrDefault(card => CardCode(card) == _etudeTarget);
                if (synchro != null)
                {
                    _etudeTarget = 0;
                    return new List<ClientCard> { synchro };
                }
            }
            if (min <= 1 && _magnamhutTarget != null && cards.Contains(_magnamhutTarget))
            {
                ClientCard target = _magnamhutTarget;
                _magnamhutTarget = null;
                return new List<ClientCard> { target };
            }
            if (min <= 1 && _magnamhutEndPhaseSearch && Duel.Player == 1 && Duel.Phase == DuelPhase.End
                && cards.All(card => card != null && card.Controller == 0 && (card.Location == CardLocation.Deck || card.Location == CardLocation.Grave)))
            {
                _magnamhutEndPhaseSearch = false;
                ClientCard pick = cards.FirstOrDefault(card => CardCode(card) == CardId.FydraulisHarmonia)
                    ?? cards.FirstOrDefault(card => CardCode(card) == CardId.PowerViceDragon) ?? cards.First();
                Report("interaction", "Magnamhut searches " + (pick.Name ?? pick.Id.ToString()) + " in the opponent End Phase");
                return new List<ClientCard> { pick };
            }

            if (min <= 1 && _abyssRevive && cards.All(card => card != null && card.Controller == 0 && card.Location == CardLocation.Grave))
            {
                _abyssRevive = false;
                ClientCard tuner = cards.FirstOrDefault(card => CardCode(card) == _abyssReviveCode);
                return tuner != null ? new List<ClientCard> { tuner } : null;
            }
            return null;
        }

        // PURPOSE: depois do combo, baixar as armadilhas da mão.
        private bool SetTrapAfterCombo()
        {
            if (Duel.Player != 0 || !IsMainPhase(Duel.Phase) || _idleStep != null)
                return false;
            if (!_comboDone && !_plannerDisabledThisTurn)
                return false;
            // Baixar só antes de encerrar o turno (jogador): se ainda dá para ir à batalha, espera a Main Phase 2.
            if (Duel.Phase == DuelPhase.Main1 && Duel.MainPhase != null && Duel.MainPhase.CanBattlePhase)
                return false;
            if (!Card.IsTrap() || !TrapsToSet.Contains(CardCode(Card)))
                return false;
            // Baixa todas, menos a 2ª cópia de armadilha que só pode ser ativada 1 vez por turno (jogador, 2026-09-14):
            // ela ocuparia uma zona sem poder ser usada no mesmo turno que a outra. A Impermanence não tem essa limitação.
            int code = CardCode(Card);
            if (OncePerTurnTraps.Contains(code) && Bot.SpellZone.Any(card => card != null && CardCode(card) == code))
                return false;
            return true;
        }

        // Armadilhas com "só pode ativar 1 por turno" (cards.cdb). Red Zone: cada efeito 1 vez por turno.
        private static readonly HashSet<int> OncePerTurnTraps = new HashSet<int>
        {
            CardId.DominusImpulse, CardId.KingsResonance, CardId.RedDragonArchfiendsChain, CardId.RedZone, CardId.EtudeOfTheBranded
        };

        // PURPOSE: materiais reais para os materiais do plano (mesmo código e nível; prefere a mesma zona).
        private List<ClientCard> FindMaterials(long[] materials)
        {
            if (materials == null)
                return null;
            var result = new List<ClientCard>();
            foreach (long material in materials)
            {
                ClientCard match = null;
                for (int seq = 0; seq < Bot.MonsterZone.Length; ++seq)
                {
                    ClientCard card = Bot.MonsterZone[seq];
                    if (card == null || result.Contains(card) || !card.IsFaceup()
                        || CardCode(card) != RdaField.Id(material) || card.Level != RdaField.Level(material))
                        continue;
                    if (match == null || (seq >= 5) == RdaField.Emz(material))
                        match = card;
                }
                if (match == null)
                    return null;
                result.Add(match);
            }
            return result;
        }

        // PURPOSE: registrar que o passo do menu foi iniciado.
        private bool CommitIdleStep()
        {
            _nextStep = _idleStepIndex + 1;
            _declinedSources.Clear();
            StartAction(_idleStep.Action, Card);
            _idleStep = null;
            _prompt = PromptKind.None;
            return true;
        }

        // PURPOSE: guardar as escolhas da ação e anotar os efeitos do turno que o jogo não informa.
        private void StartAction(PlanAction action, ClientCard source)
        {
            _currentAction = action;
            _remainingPicks[action] = new List<RdaPick>(action.Picks);
            if (source != null)
                _actionBySource[source] = action;

            if (!NotOncePerTurn.Contains(action.Effect) && action.Kind != PlanKind.TriggerDecline)
                _usedThisTurn.Add(action.Effect);

            switch (action.Kind)
            {
                case PlanKind.NormalSummon:
                    _normalSummonUsed = true;
                    break;
                case PlanKind.ExtraNormalSummon:
                    _extraNormalSummonAvailable = false;
                    break;
                case PlanKind.Synchro:
                    // Scarred conta como "Red Dragon Archfiend" invocado por Synchro (condição do Burning Soul).
                    if (RdaCards.RdaLike.Contains(action.CardId))
                        _rdaSynchroThisDuel = true;
                    break;
            }
            switch (action.Effect)
            {
                case RdaKey.DarknessExtraNs:
                    _extraNormalSummonAvailable = true;
                    break;
                case RdaKey.MagnamhutEndPhase:
                    _turnFlags |= RdaState.FlagMagnamhutEndPhase;
                    break;
                case RdaKey.QuetzacoatlRevive:
                    _noSpecialSummon = true;
                    break;
                case RdaKey.ScarredSummon:
                    _rdaSynchroThisDuel = true;
                    break;
                case RdaKey.BurningSoulAdd:
                    _burningSoulAddUsedThisDuel = true;
                    break;
            }

            Report("plan_step", action.Text);
        }

        // -----------------------------------------------------------------------------------------------------
        // 2.6 Gatilhos na chain
        // -----------------------------------------------------------------------------------------------------

        // PURPOSE: decidir se ativa um gatilho nosso.
        //   1. Magnamhut na End Phase: sempre busca (a Harmonia é escolhida em OnSelectCard).
        //   2. Gatilho conhecido: procura a resposta nos passos de gatilho logo à frente do plano.
        //   3. Se o plano não previu esse gatilho, replaneja com ele pendente e segue a resposta nova.
        private bool DecideTrigger(ClientCard card, int desc)
        {
            if (card == null || _declinedSources.Contains(card))
                return false;

            if (Duel.Phase == DuelPhase.End)
            {
                if (CardCode(card) != CardId.BystialMagnamhut)
                    return false;
                var search = new PlanAction { Kind = PlanKind.TriggerAccept, CardId = card.Id, From = card.Location, Text = "Magnamhut searches Fydraulis Harmonia in the End Phase" };
                search.Picks.Add(new RdaPick(CardId.FydraulisHarmonia, CardLocation.Deck));
                StartAction(search, card);
                return true;
            }

            // Gatilhos nossos abrem a chain ou entram junto com outros gatilhos nossos (SEGOC).
            // Se o oponente já tem elo na chain, é uma janela de resposta (efeito rápido), não gatilho.
            // Foi o que aconteceu no teste: o King ② (banir e invocar RDA) foi tratado como a busca.
            if (Duel.CurrentChain.Any(link => link.Controller != 0))
                return false;

            RdaKey key = IdentifyTrigger(card, desc);
            if (key == RdaKey.None || !IsMainPhase(Duel.Phase) || _plannerDisabledThisTurn)
                return false;

            int index = FindTriggerStep(key);
            if (index < 0)
            {
                RdaState real = ReadState();
                real.Pending = new[] { (int)key };
                if (!Replan(real, null))
                    return false;
                index = FindTriggerStep(key);
                if (index < 0)
                    return false;
            }

            PlanStep step = _plan.Steps[index];

            // Ordem da chain (top-down): o último elo resolve primeiro. O plano lista os gatilhos na ordem em
            // que devem RESOLVER, então, entre gatilhos oferecidos juntos, ativa primeiro o que o plano resolve
            // por último. Este fica para a próxima janela, que o jogo abre logo em seguida.
            if (step.Action.Kind == PlanKind.TriggerAccept && HasLaterSimultaneousTrigger(card, index))
                return false;

            _doneSteps.Add(index);
            if (step.Action.Kind == PlanKind.TriggerDecline)
            {
                _declinedSources.Add(card);
                Report("plan_step", step.Action.Text);
                return false;
            }
            StartAction(step.Action, card);
            return true;
        }

        // PURPOSE: existe outro gatilho aceito pelo plano, oferecido nesta mesma janela, que deve resolver depois?
        private bool HasLaterSimultaneousTrigger(ClientCard card, int index)
        {
            foreach (ClientCard other in _chainCandidates)
            {
                if (other == null || other == card || _declinedSources.Contains(other))
                    continue;
                RdaKey otherKey = IdentifyTrigger(other, 0);
                if (otherKey == RdaKey.None)
                    continue;
                for (int i = index + 1; i < _plan.Steps.Count; ++i)
                {
                    PlanAction action = _plan.Steps[i].Action;
                    if (action.Kind != PlanKind.TriggerAccept && action.Kind != PlanKind.TriggerDecline)
                        break;
                    if (!_doneSteps.Contains(i) && action.Kind == PlanKind.TriggerAccept && action.Effect == otherKey)
                        return true;
                }
            }
            return false;
        }

        // Os gatilhos ficam juntos logo depois da ação que os causou; vários podem vir em qualquer ordem.
        private int FindTriggerStep(RdaKey key)
        {
            if (_plan == null)
                return -1;
            for (int i = _nextStep; i < _plan.Steps.Count; ++i)
            {
                PlanAction action = _plan.Steps[i].Action;
                bool isTrigger = action.Kind == PlanKind.TriggerAccept || action.Kind == PlanKind.TriggerDecline;
                if (!isTrigger)
                    break;
                if (!_doneSteps.Contains(i) && action.Effect == key)
                    return i;
            }
            return -1;
        }

        // -----------------------------------------------------------------------------------------------------
        // 2.6b Respostas rápidas a efeitos do oponente
        //
        // Quando o último elo da chain é do oponente (handtrap no nosso combo, Nibiru, efeito no turno dele),
        // escolhe UMA resposta, na ordem do que custa menos para a mesa:
        //   1. Abyss        carta do oponente com a face para cima no campo: nega os efeitos dela (sem custo)
        //   2. Dis Pater    efeito de monstro: embaralha carta banida do oponente (nega) ou nossa (destrói o monstro no campo)
        //   3. Zalen        chain de 2+: nega o efeito do oponente (o encadeado ou o anterior)
        //   4. Quetzacoatl  efeito de monstro: devolve nosso Synchro de menor valor ao Extra e nega
        //   5. King ②       Nibiru, efeito que parou a busca do King ou que tem o King como alvo: bane o King e
        //                   invoca RDA do Extra (ATK dobrado, tratado como Invocação-Synchro, abre o Zalen no Nibiru)
        //   6. Red Zone     carta do oponente no campo: destrói
        // Depois da resposta o estado muda e o plano é refeito normalmente.
        // -----------------------------------------------------------------------------------------------------

        // Cartas nossas cujo elo já NEGA o efeito do oponente: nesses casos o Zalen não precisa negar o elo anterior de novo.
        // Jogador (2026-09-15, partida contra o Labrynth): a Red Zone só destrói e a Harmonia (6) só destrói, então o efeito do
        // oponente resolve do mesmo jeito. Elas saíram da lista: depois delas o Zalen ainda deve negar o efeito dele.
        private static readonly HashSet<int> OurNegationCards = new HashSet<int>
        {
            CardId.AshBlossomJoyousSpring, CardId.InfiniteImpermanence, CardId.DominusImpulse,
            CardId.ZalenTheShackledDragon, CardId.BystialDisPater, CardId.CrimsonDragonQuetzacoatl, 29053656,
            CardId.HotRedDragonArchfiendAbyss, 40366667
        };

        private void ClearResponse()
        {
            _responseKind = ResponseKind.None;
            _responseSource = null;
            _responseTarget = null;
            _responseFresh = false;
        }

        // Descrição do efeito pelo índice do texto (confere id, alias e código do modelo por causa das artes alternativas).
        private bool IsEffectDescription(ClientCard card, int desc, int index)
        {
            if (desc <= 0)
                return true;
            foreach (int code in new[] { card.Id, card.Alias, CardCode(card) })
                if (code > 0 && desc == Util.GetStringId(code, index))
                    return true;
            return false;
        }

        private bool OfferedNow(int code)
        {
            return _chainCandidates != null && _chainCandidates.Any(card => card != null && CardCode(card) == code
                && !((card.Location == CardLocation.MonsterZone || card.Location == CardLocation.SpellZone) && card.IsFaceup() && card.IsDisabled()));
        }

        private int OtherDragonSynchros(ClientCard except)
        {
            return Bot.GetMonsters().Count(card => card != null && card != except && card.IsFaceup()
                && card.HasType(CardType.Synchro) && card.HasRace(CardRace.Dragon));
        }

        // PURPOSE: o elo é a ativação de um efeito de MONSTRO?
        // ChainInfo.ActivateType copia ClientCard.Type, e o framework só preenche esse campo em ClientCard.Update, quando o
        // servidor manda os dados da carta. Carta ativada da MÃO do oponente é oculta: o bot descobre o id pela mensagem de
        // chaining e SetId preenche nome e alias, nunca o tipo. Então ActivateType fica 0 e toda regra de "efeito de monstro"
        // falhava contra handtrap da mão (jogador, 2026-09-16: Nibiru com o King na mesa, o King ② não ativou; o log mostra o
        // jogo oferecendo o King e o bot passando). Quando o tipo não veio, consulta o cards.cdb pelo id.
        private static bool IsMonsterActivation(ChainInfo info)
        {
            if (info == null)
                return false;
            if (info.ActivateType != 0)
                return (info.ActivateType & (int)CardType.Monster) != 0;
            int id = info.ActivateId != 0 ? info.ActivateId : info.ActivateAlias;
            if (id == 0)
                return false;
            YGOSharp.OCGWrapper.NamedCard data = YGOSharp.OCGWrapper.NamedCard.Get(id);
            return data != null && (data.Type & (int)CardType.Monster) != 0;
        }

        // PURPOSE: qual resposta usar agora (ou nenhuma), olhando só o último elo.
        private ResponseKind ChooseResponse(out ClientCard target)
        {
            target = null;
            IList<ChainInfo> chain = Duel.CurrentChainInfo;
            if (chain == null || chain.Count == 0)
                return ResponseKind.None;
            ChainInfo last = chain[chain.Count - 1];

            // Nosso elo por último em cima de um elo do oponente (ex.: Nibiru <- King ②): o Zalen nega o anterior.
            if (last.ActivatePlayer != 1)
            {
                if (chain.Count >= 2 && chain[chain.Count - 2].ActivatePlayer == 1 && OfferedNow(CardId.ZalenTheShackledDragon)
                    && !OurNegationCards.Contains(last.ActivateId) && !OurNegationCards.Contains(last.ActivateAlias))
                {
                    target = chain[chain.Count - 2].RelatedCard;
                    return ResponseKind.ZalenFirst;
                }
                return ResponseKind.None;
            }

            ClientCard source = last.RelatedCard;
            bool monsterEffect = IsMonsterActivation(last);
            bool onField = source != null && source.Controller == 1 && source.IsFaceup()
                && (source.Location == CardLocation.MonsterZone || source.Location == CardLocation.SpellZone);
            bool nibiru = last.IsActivateCode(CardId.NibiruThePrimalBeing);
            target = source;

            // Dominus Impulse BAIXADA (nunca da mão: trava os efeitos de LIGHT/EARTH/WIND no duelo). O jogo só oferece
            // quando o efeito invoca por Invocação-Especial; nega e ainda destrói se há armadilha no nosso GY.
            // Teste: o bot passou nas 3 ofertas (Bone, Red Rising revivendo, Crimson Resonator invocando 2 do Deck).
            if (_chainCandidates != null && _chainCandidates.Any(card => card != null && CardCode(card) == CardId.DominusImpulse
                && card.Location == CardLocation.SpellZone && !infiniteImpermanenceNegatedColumns.Contains(card.Sequence)))
                return ResponseKind.Dominus;
            if (onField && OfferedNow(CardId.HotRedDragonArchfiendAbyss))
                return ResponseKind.Abyss;
            if (monsterEffect && OfferedNow(CardId.BystialDisPater))
            {
                // Dis Pater ② (jogador): embaralhar carta banida do oponente NEGA; embaralhar carta banida nossa só DESTRÓI.
                //   efeito vindo da mão: destruir já resolve (a carta sai da mão), sem gastar banida do oponente;
                //   efeito de monstro no campo: nega se houver banida do oponente, senão destrói;
                //   efeito vindo do GY/banidas: não dá para destruir; só nega, e sem banida do oponente não usa (gastaria à toa).
                bool enemyBanished = Enemy.Banished.Any(card => card != null && card.IsFaceup());
                bool ourBanished = Bot.Banished.Any(card => card != null);
                CardLocation from = source != null ? source.Location : CardLocation.Grave;
                if (from == CardLocation.Hand)
                {
                    if (ourBanished) return ResponseKind.DisPaterDestroy;
                    if (enemyBanished) return ResponseKind.DisPaterNegate;
                }
                else if (from == CardLocation.MonsterZone)
                {
                    if (enemyBanished) return ResponseKind.DisPaterNegate;
                    if (onField && ourBanished) return ResponseKind.DisPaterDestroy;
                }
                else if (enemyBanished)
                    return ResponseKind.DisPaterNegate;
            }
            if (chain.Count >= 2 && OfferedNow(CardId.ZalenTheShackledDragon))
                return ResponseKind.ZalenSecond;
            // King ② (jogador, 2026-09-15): quando um monstro do oponente ativa efeito, bane o King e traz o RDA com ATK dobrado
            // que o oponente não destrói por efeito. Com o Zalen pronto, o nosso elo em cima do dele abre o Zalen, que nega e
            // destrói a carta dele (King + Zalen, ou Red Zone + Zalen). Por isso o King vem antes da Quetzacoatl, que gasta um
            // Synchro Dragão de volta ao Extra.
            if (monsterEffect && OfferedNow(CardId.TheCrimsonKing)
                && Bot.ExtraDeck.Any(card => card != null && CardCode(card) == CardId.RedDragonArchfiend))
                return ResponseKind.King;
            if (monsterEffect && OfferedNow(CardId.CrimsonDragonQuetzacoatl)
                && OtherDragonSynchros(Bot.GetMonsters().FirstOrDefault(card => card != null && CardCode(card) == CardId.CrimsonDragonQuetzacoatl)) > 0)
                return ResponseKind.Quetzacoatl;
            // Fydraulis Harmonia, no turno do oponente, contra efeito de monstro do campo dele. Revelando 6: se invoca, manda
            // Storm-Bane ao GY (traz o nova banido) e destrói 1 monstro. A trava só pega monstros do Extra que não são Synchro.
            // Jogador (2026-09-15): com nova nosso, a Harmonia espera o nova banir e só entra com 2 zonas principais livres
            // (Harmonia + o nova que volta), para o nova voltar e continuar pressionando. Sem nova nosso: contra o combo dele.
            if (Duel.Player == 1 && monsterEffect && source != null && source.Controller == 1 && source.Location == CardLocation.MonsterZone
                && OfferedNow(CardId.FydraulisHarmonia))
            {
                bool novaBanished = Bot.Banished.Any(card => card != null && IsNova(card));
                bool novaOnField = Bot.GetMonsters().Any(card => card != null && IsNova(card));
                bool stormBane = Bot.ExtraDeck.Any(card => card != null && CardCode(card) == CardId.StormBaneDragonDestorbim);
                if (novaBanished && stormBane && FreeMainMonsterZones() >= 2)
                    return ResponseKind.Harmonia;
                if (!novaBanished && !novaOnField && _opponentActivationsThisTurn >= 2)
                    return ResponseKind.Harmonia;
            }
            // Red Zone só destrói: não impede o efeito que já está na chain (teste: destruir a Dominus Impulse e a
            // Infinite Impermanence não evitou as negações). Usa só para tirar o melhor monstro do oponente do campo.
            // Red Zone destrói qualquer carta do campo: magias/armadilhas do oponente também entram na disputa pelo alvo.
            ClientCard redZoneTarget = BestEnemyMonster(TargetKind.SpellTrap, Removal.Destroy, includeSpells: true);
            // Destruir monstro que se aproveita disso (flutuador) só vale se ele for ameaça das listas do WindBot.
            if (redZoneTarget != null && BenefitsFromRemoval(redZoneTarget, Removal.Destroy) && !IsListedThreat(redZoneTarget))
                redZoneTarget = null;
            // Nem a ameaça das listas justifica: se ele não pode ser destruído por efeito, a Red Zone não faz nada nele.
            if (redZoneTarget != null && WastedOnTarget(redZoneTarget, Removal.Destroy))
                redZoneTarget = null;
            if (redZoneTarget != null && OfferedNow(CardId.RedZone))
            {
                target = redZoneTarget;
                return ResponseKind.RedZone;
            }
            return ResponseKind.None;
        }

        // Como o efeito alcança a carta do oponente: sem alvo, alvo de efeito de monstro ou alvo de magia/armadilha.
        private enum TargetKind { NoTarget, MonsterEffect, SpellTrap }

        // Alvo no campo do oponente (jogador, 2026-09-15: "listas do WindBot + as nossas regras"; maior ATK nem sempre é o certo).
        //   1. Floodgate (trava o nosso jogo)                        — lista Floodgate do WindBot
        //   2. Monstro a negar antes de usar o efeito ou perigoso     — ShouldBeDisabledBeforeItUseEffectMonster / DangerousMonster
        //   3. Monstro difícil de tirar em batalha                     — InvincibleMonster
        //   4. O monstro que está ativando efeito no elo atual
        //   5. O último monstro invocado (regra do jogador para cartas desconhecidas)
        //   6. Monstro do Extra Deck (costuma ter negação ou ser o boss); ATK só como desempate final.
        // Cartas que não aceitam o tipo de alvo do efeito ficam de fora (ShouldNotBeTarget / MonsterTarget / SpellTrapTarget).
        private long EnemyTargetRank(ClientCard card)
        {
            long rank = 0;
            if (card.IsFloodgate())
                rank += 16000000000L;
            if (card.IsMonsterShouldBeDisabledBeforeItUseEffect() || card.IsMonsterDangerous())
                rank += 8000000000L;
            if (card.IsMonsterInvincible())
                rank += 4000000000L;
            ClientCard chainCard = Duel.CurrentChain.Count > 0 ? Duel.CurrentChain[Duel.CurrentChain.Count - 1] : null;
            if (chainCard != null && chainCard == card)
                rank += 2000000000L;
            int order;
            if (_enemyEntryOrder.TryGetValue(card, out order))
                rank += Math.Min(order, 999) * 1000L;
            // Magia/armadilha que fica no campo costuma ser o problema maior (jogador, 2026-09-15: "existem magias/armadilhas que
            // ficam em campo e são o grande problema"). Regra ampla, sem citar cartas: Field e Contínuas valem mais que um monstro
            // comum, porque seguem aplicando efeito; carta virada é incógnita e vale menos; Equip vale pelo que sustenta.
            if (card.Location == CardLocation.SpellZone)
            {
                if (card.IsFacedown())
                    rank += 300000;
                else if (card.HasType(CardType.Field) || card.HasType(CardType.Continuous))
                    rank += 1200000;
                else if (card.HasType(CardType.Equip))
                    rank += 500000;
                else
                    rank += 200000;
            }
            // Monstro do Extra Deck costuma ser bom alvo (jogador, 2026-09-15): fica acima de equipamento e de carta virada, e
            // abaixo de uma magia de campo/contínua com a face para cima.
            if (card.HasType(CardType.Fusion) || card.HasType(CardType.Synchro) || card.HasType(CardType.Xyz) || card.HasType(CardType.Link))
                rank += 600000;
            rank += Math.Max(0, Math.Min(99999, card.Attack));
            return rank;
        }

        private static bool CanBeTargetedBy(ClientCard card, TargetKind kind)
        {
            if (kind == TargetKind.NoTarget)
                return true;
            if (card.IsShouldNotBeTarget())
                return false;
            return kind == TargetKind.MonsterEffect ? !card.IsShouldNotBeMonsterTarget() : !card.IsShouldNotBeSpellTrapTarget();
        }

        // Como o efeito tira a carta do campo: nada (negar), destruir ou banir.
        private enum Removal { None, Destroy, Banish }

        private const int BenefitDestroy = 1; // ganha algo ao ser destruído, ir ao GY ou sair do campo
        private const int BenefitBanish = 2;  // ganha algo ao ser banido ou sair do campo
        private static readonly Dictionary<int, int> RemovalBenefitCache = new Dictionary<int, int>();
        // Cobre as variações dos textos: "this card", "this Fusion Summoned card", "this card in its owner's control",
        // "is/was destroyed", "is sent (from the field) to the GY", "leaves the field", "is/was banished".
        private static readonly System.Text.RegularExpressions.Regex DestroyBenefitText = new System.Text.RegularExpressions.Regex(
            @"\bthis [^.:;]{0,60}?card[^.:;]{0,40}? (?:is|was) (?:destroyed|sent (?:from the field )?to (?:the|your) gy)|\bthis [^.:;]{0,60}?card[^.:;]{0,40}? leaves the field",
            System.Text.RegularExpressions.RegexOptions.Compiled);
        private static readonly System.Text.RegularExpressions.Regex BanishBenefitText = new System.Text.RegularExpressions.Regex(
            @"\bthis [^.:;]{0,60}?card[^.:;]{0,40}? (?:is|was) banished|\bthis [^.:;]{0,60}?card[^.:;]{0,40}? leaves the field",
            System.Text.RegularExpressions.RegexOptions.Compiled);

        // Monstro que se aproveita de sair do campo (jogador, 2026-09-15: "evitar destruir o que se aproveita disso"). Lido do
        // texto em inglês do cards.cdb, como o valor de alvo da Magnamhut. "Destroyed by battle" não conta (nossos efeitos não
        // são batalha). Carta desconhecida (face para baixo, sem texto): não se aproveita.
        private static bool BenefitsFromRemoval(ClientCard card, Removal removal)
        {
            if (card == null || card.Id == 0 || removal == Removal.None)
                return false;
            int flags;
            lock (RemovalBenefitCache)
            {
                if (!RemovalBenefitCache.TryGetValue(card.Id, out flags))
                {
                    YGOSharp.OCGWrapper.NamedCard data = YGOSharp.OCGWrapper.NamedCard.Get(card.Id);
                    // "Banish it when it leaves the field" é restrição (desvantagem), não ganho: sai antes da comparação.
                    string text = data != null && data.Description != null
                        ? data.Description.ToLowerInvariant().Replace("destroyed by battle", "")
                            .Replace("banish it when it leaves the field", "").Replace("banish it if it leaves the field", "")
                        : "";
                    if (DestroyBenefitText.IsMatch(text))
                        flags |= BenefitDestroy;
                    if (BanishBenefitText.IsMatch(text))
                        flags |= BenefitBanish;
                    RemovalBenefitCache[card.Id] = flags;
                }
            }
            return (flags & (removal == Removal.Destroy ? BenefitDestroy : BenefitBanish)) != 0;
        }

        // Ameaça das listas do WindBot: vale tirar mesmo que ela se aproveite de sair do campo.
        private static bool IsListedThreat(ClientCard card)
        {
            return card.IsFloodgate() || card.IsMonsterShouldBeDisabledBeforeItUseEffect() || card.IsMonsterDangerous() || card.IsMonsterInvincible();
        }

        // Melhor alvo entre as cartas oferecidas (ou o campo do oponente): monstros e magias/armadilhas floodgate. Quem se
        // aproveita da remoção (destruir/banir) desce abaixo dos outros; ameaças das listas continuam no topo.
        // includeSpells: efeitos que alcançam qualquer carta do campo (Red Zone: "escolha 1 card no campo; destrua"). Sem ele só
        // entram monstros e as magias/armadilhas floodgate.
        private ClientCard BestEnemyTarget(IEnumerable<ClientCard> candidates, TargetKind kind, Removal removal = Removal.None, bool includeSpells = false)
        {
            return candidates.Where(card => card != null && card.Controller == 1 && CanBeTargetedBy(card, kind)
                    && (card.Location == CardLocation.MonsterZone || card.IsFloodgate()
                        || (includeSpells && card.Location == CardLocation.SpellZone)))
                .OrderByDescending(card => EnemyTargetRank(card)
                    - (BenefitsFromRemoval(card, removal) ? 3000000000L : 0)
                    - (WastedOnTarget(card, removal) ? 6000000000L : 0))
                .FirstOrDefault();
        }

        // Destruir quem não pode ser destruído por efeito não faz absolutamente nada — é pior do que destruir um flutuador,
        // que ao menos sai do campo. Por isso o desconto aqui é o dobro do de BenefitsFromRemoval. Jogador, 2026-09-17:
        // replay contra Yubel em que a Harmonia foi gasta tentando destruir um monstro protegido.
        // Vale só para DESTRUIÇÃO: banir (Storm-Bane ①) passa por cima dessa proteção.
        // Usa TraitsOf, que cai no id lembrado quando a carta está virada para baixo.
        private bool WastedOnTarget(ClientCard card, Removal removal)
        {
            BattleTrait traits = TraitsOf(card);
            // NÃO AFETADO por efeitos: mirar nele joga fora o efeito inteiro, não importa se ele destrói, bane ou nega.
            // Por isso vale para qualquer tipo de remoção, inclusive Removal.None (alvo de negação).
            // Jogador, 2026-09-18: replay em que a Harmonia foi gasta tentando destruir um Yubel não afetado por efeitos.
            if ((traits & BattleTrait.ImmuneEffect) != 0)
                return true;
            return removal == Removal.Destroy && (traits & BattleTrait.EffectIndestructible) != 0;
        }

        private ClientCard BestEnemyMonster(TargetKind kind = TargetKind.NoTarget, Removal removal = Removal.None, bool includeSpells = false)
        {
            return BestEnemyTarget(Enemy.GetMonsters().Concat(Enemy.GetSpells()), kind, removal, includeSpells);
        }

        // Existe monstro do oponente que vale destruir (não se aproveita disso, ou é ameaça das listas)?
        private bool HasWorthwhileDestroyTarget()
        {
            return Enemy.GetMonsters().Any(card => card != null && !WastedOnTarget(card, Removal.Destroy)
                && (!BenefitsFromRemoval(card, Removal.Destroy) || IsListedThreat(card)));
        }

        // Ash Blossom e Infinite Impermanence com as nossas regras (jogador, 2026-09-14/15), no lugar do padrão do WindBot:
        //   Turno do oponente: deixa o 1º efeito dele passar e nega quando ele se compromete (2ª ativação no turno em diante),
        //   a não ser que o efeito mire as nossas cartas ou venha de carta das listas de ameaça do WindBot (floodgate, negar antes
        //   de usar, perigoso). No nosso turno: nega o efeito do oponente que responde ao combo (padrão do WindBot).
        /// <summary>
        /// O monstro está numa batalha AGORA? Devolver/remover o atacante nessa janela cancela o ataque e vira replay
        /// (foi o que derrubou o bot em 2026-09-15). Depois que a batalha acaba, tirá-lo do campo não custa nada.
        /// </summary>
        private bool InBattleNow(ClientCard card)
        {
            if (card == null)
                return false;
            if (ReferenceEquals(Bot.BattlingMonster, card))
                return true;
            return card.IsLastAttacker
                && (Duel.Phase == DuelPhase.BattleStep || Duel.Phase == DuelPhase.Damage || Duel.Phase == DuelPhase.DamageCal);
        }

        /// <summary>
        /// RDA comum que já atacou: o melhor custo para a devolução da Quetzacoatl. Não vale para o RDA protegido pelo
        /// King ② nem para o trazido pela Crimson Gaia — esses são a salvação do deck e ficam na mesa.
        /// </summary>
        /// <summary>
        /// O monstro ainda tem negação disponível? Abyss, Dis Pater, Zalen e Quetzacoatl interrompem enquanto estão na
        /// mesa; devolver um deles com a negação intacta é entregar a nossa resposta. Se já gastou neste turno, ou está
        /// negado, sair do campo não custa nada. Jogador, 2026-09-22.
        /// </summary>
        private bool StillHoldsNegate(ClientCard card)
        {
            if (card == null || card.IsDisabled())
                return false;
            int code = CardCode(card);
            return RdaPlanner.IsNegate(code) && !_negateUsedThisTurn.Contains(code);
        }

        private bool PreferredQuetzacoatlCost(ClientCard card)
        {
            return card != null && CardCode(card) == CardId.RedDragonArchfiend && card.Attacked
                && !_gaiaRdaCards.Contains(card) && !_kingRdaCards.Contains(card) && !InBattleNow(card);
        }

        // -----------------------------------------------------------------------------------------------------
        // Quanto custa deixar o efeito do oponente resolver. Prioridades do jogador (2026-09-22), na ordem que ele
        // deu: (1) destruir/banir nossas cartas; (2) trocar tipo/atributo ou proibir material, que quebra as nossas
        // receitas de Synchro; (3) o mesmo contra as nossas magias/armadilhas; (4) impedir mudança de posição;
        // (5) negar os nossos efeitos; (6) por último, impedir que ele compre/busque e chegue no boss.
        // Não é nota por carta: é a categoria do efeito, que o perfil já classifica, aplicada agora.
        // -----------------------------------------------------------------------------------------------------
        private const int ThreatHigh = 100;   // tira nossas cartas ou quebra a nossa receita
        private const int ThreatLock = 80;    // trava alguma coisa nossa
        private const int ThreatNegate = 70;  // nega os nossos efeitos
        private const int ThreatBoss = 50;    // adianta a mesa dele
        private const int ThreatSearch = 30;  // busca/compra
        private const int ThreatOther = 20;
        // A partir daqui vale gastar interação já na PRIMEIRA ativação do turno dele.
        private const int ThreatWorthEarly = ThreatNegate;

        private static readonly string[] HighThreatWords =
        {
            // Descartar a nossa mão tira nossas cartas igual destruir ou banir: mesma faixa (jogador, prioridade 1).
            "destroy", "banish", "discards our hand",
            "changes Type on the field", "changes Attribute on the field",
            "our monsters cannot be Synchro material", "cannot be Fusion material",
            "cannot be Xyz material", "cannot be Link material"
        };

        /// <summary>Nota de ameaça do efeito que está entrando na cadeia, pelo perfil dele.</summary>
        private int OpponentEffectThreat(ClientCard source)
        {
            if (source == null)
                return ThreatOther;
            int id = CardCode(source);
            List<EffectProfile> profiles = EffectProfilesOf(id);
            if (profiles.Count == 0)
                return ThreatOther;

            // O índice do efeito vem do elo. Sem ele, considera o pior que a carta sabe fazer — errar para o lado
            // de negar é melhor do que deixar passar uma destruição por não saber qual efeito é.
            int index = -1;
            IList<ChainInfo> chain = Duel.CurrentChainInfo;
            if (chain != null && chain.Count > 0)
            {
                ChainInfo last = chain[chain.Count - 1];
                int desc = last.ActivateDescription;
                if (desc > 0 && desc / 16 == id)
                    index = desc & 0xF;
            }
            EffectProfile exact = index >= 0 ? profiles.FirstOrDefault(p => p.DescIndex == index) : null;
            var looked = new List<EffectProfile>();
            if (exact != null) looked.Add(exact); else looked.AddRange(profiles);

            int worst = ThreatOther;
            foreach (EffectProfile profile in looked)
            {
                foreach (string does in profile.Does)
                {
                    int value = ThreatOther;
                    if (HighThreatWords.Any(word => does == word)) value = ThreatHigh;
                    else if (does.StartsWith("locks") || does.StartsWith("cannot")) value = ThreatLock;
                    else if (does == "disables monsters" || does == "negates effects"
                        || does == "disable" || does == "negate") value = ThreatNegate;
                    else if (does == "special summon") value = ThreatBoss;
                    else if (does == "search" || does == "to hand" || does == "draw") value = ThreatSearch;
                    if (value > worst) worst = value;
                }
            }

            // Monstro protegido (não é destruído ou não pode ser alvo): depois que ele está na mesa as nossas saídas
            // são poucas — a linha RDA + Crimson Gaia virando tudo para baixo, ou banir com Hypernova/Supernova, e
            // vários têm ATK bem acima do nosso (jogador, 2026-09-22). Então negar o efeito ENQUANTO dá é uma das
            // poucas janelas: qualquer efeito vindo de um monstro assim sobe para a faixa alta.
            if (source.Location == CardLocation.MonsterZone && source.IsFaceup())
            {
                BattleTrait traits = TraitsOf(source);
                if ((traits & (BattleTrait.EffectIndestructible | BattleTrait.ImmuneEffect | BattleTrait.BattleIndestructible)) != 0)
                    worst = Math.Max(worst, ThreatHigh);
            }
            return worst;
        }

        // -----------------------------------------------------------------------------------------------------
        // Ameaça que PERMANECE. Regra do jogador (2026-09-23): negar a ativação de uma carta que FICA no campo não
        // resolve nada. A parte contínua dela nunca entra em cadeia, então nenhuma negação de ativação a alcança —
        // ou a carta sai do campo, ou a ameaça continua de pé.
        //
        // Visto no duelo contra Rosas (log 002832): o Ash negou o elo 1 do Black Rose Garden, que é a ATIVAÇÃO do
        // campo ("add 1 Rose Dragon"). A segunda linha da carta, "All face-up monsters on the field become Plant
        // monsters", é contínua e ficou valendo — a carta estava na Spell Zone dele no duel_end. Não foi desperdício
        // (a busca foi negada), mas a ameaça de pé seguiu intacta.
        //
        // O contrário também é verdade e o jogador apontou: contra D/D/D a carta perigosa aparece e sai rápido, então
        // enxergar já basta e não vale gastar remoção.
        //
        // Só classifica e escreve no log. Nenhuma decisão usa isto ainda.
        private bool StandingThreat(ClientCard source, out string part)
        {
            part = null;
            if (source == null || !source.IsFaceup())
                return false;
            if (source.Location != CardLocation.MonsterZone && source.Location != CardLocation.SpellZone)
                return false;
            foreach (EffectProfile profile in EffectProfilesOf(CardCode(source)))
            {
                // Contínuo sem gatilho: é o que aplica sozinho enquanto a carta estiver ali.
                bool continuous = profile.When == "continuous" && string.IsNullOrEmpty(profile.Event);
                string floodgate = profile.Does.FirstOrDefault(does =>
                    does.StartsWith("locks") || does.StartsWith("cannot") || does.StartsWith("blocks")
                    || does.StartsWith("changes") || does == "negates effects" || does == "disables monsters");
                if (floodgate == null && !continuous)
                    continue;
                part = floodgate ?? "continuous effect";
                return true;
            }
            return LockAppliedByOperation(CardCode(source), out part);
        }

        // -----------------------------------------------------------------------------------------------------
        // A trava que o extrator não vê. O initial_effect registra um efeito de CAMPO CONTÍNUO e a proibição é
        // criada lá dentro da função de operation, então não há SetCode(EFFECT_CANNOT_*) para ler. É a forma do
        // Branded Lost (e3/e4 chamando limop/limop2).
        //
        // Medido no script.zip do cliente em 2026-09-23: 13.526 scripts, 3.208 com alguma trava, 2.115 com a trava
        // só fora do initial_effect. Mas desses, 1.966 aplicam a trava NA RESOLUÇÃO a um alvo — a Infinite
        // Impermanence é um deles — e aí negar a ativação resolve mesmo, não é ameaça de pé. Sobram 149 com efeito
        // de campo contínuo registrado, que é a classe que perdíamos. 149 de 3.208 é 4,6%.
        //
        // A regra, então, é por FORMA e não por carta: campo contínuo no initial_effect + algum código de trava em
        // qualquer lugar do arquivo. Sem script (cópia oficial, cliente ausente) simplesmente devolve false, como
        // todo o resto deste módulo.
        private static bool LockAppliedByOperation(int id, out string part)
        {
            part = null;
            if (id == 0)
                return false;
            var data = YGOSharp.OCGWrapper.NamedCard.Get(id);
            if (data == null)
                return false;
            // Magia/armadilha NORMAL resolve e vai para o GY: ela não fica, então não é ameaça de pé por definição.
            // Sem esta checagem o "Brightest, Blazing, Branded King" (armadilha normal) era marcado, porque o script
            // dele tem campo contínuo e EFFECT_DISABLE_EFFECT — ele nega tudo até o fim do turno e sai. O tipo vem da
            // coluna do cards.cdb, não de texto.
            if ((data.Type & (SpellType | TrapType)) != 0 && (data.Type & StaysOnFieldTypes) == 0)
                return false;
            string script = RdaScripts.Read(id, data != null ? data.Alias : 0);
            if (string.IsNullOrEmpty(script))
                return false;
            string initial = InitialEffectBlock(script);
            if (initial.IndexOf("EFFECT_TYPE_FIELD", StringComparison.Ordinal) < 0
                || initial.IndexOf("EFFECT_TYPE_CONTINUOUS", StringComparison.Ordinal) < 0)
                return false;
            foreach (string[] pair in FloodgateWords)
            {
                if (script.IndexOf(pair[0], StringComparison.Ordinal) < 0)
                    continue;
                part = pair[1];
                return true;
            }
            return false;
        }

        private void ReportStandingThreat(ClientCard source)
        {
            string part;
            if (source == null || !StandingThreat(source, out part))
                return;
            Report("interaction", string.Format(
                "standing threat: {0} keeps \"{1}\" while it is on the field — negating an activation does not remove it",
                source.Name ?? source.Id.ToString(), part));
        }

        // =====================================================================================================
        // CAMADA 5 — a reserva. Só log, por enquanto: ela escreve o que TERIA segurado e não muda a resposta.
        //
        // O problema, com as duas partidas que o mostram lado a lado (Branded, 2026-09-23):
        //   011938 (vitória)  a resposta sobrou e o Quetzacoatl negou o Mirrorjade quando ele chegou
        //   012308 (derrota)  o Harmonia foi gasto no turno 1 num buscador; o Mirrorjade chegou sem resposta
        // Mesmo deck, mesmo boss. A diferença foi para quem a interação foi gasta e quando.
        //
        // A conta usa as três peças que já existem e estão testadas: o CUSTO do pior boss ainda alcançável, o
        // APOIO (quantos caminhos ele tem, ou seja, se dá para impedir) e a AMEAÇA DE PÉ (se negar resolve ou
        // se só remoção resolve).
        // =====================================================================================================
        private List<OpponentBoss> _projectedBosses;
        // Com mais respostas que isto na mão/mesa, não há o que racionar.
        private const int ReserveScarceAnswers = 1;

        // Cartas nossas que servem de resposta agora: as da mão mais os monstros de negação já na mesa.
        private int CountAvailableAnswers()
        {
            int inHand = Bot.Hand.Count(card => card != null && OurNegationCards.Contains(CardCode(card)));
            int onField = Bot.GetMonsters().Count(card => card != null && card.IsFaceup() && !card.IsDisabled()
                && OurNegationCards.Contains(CardCode(card)));
            return inHand + onField;
        }

        // O pior boss que ainda PODE chegar: já na mesa não conta, porque para esse a hora de guardar passou.
        private OpponentBoss WorstBossStillComing()
        {
            if (_projectedBosses == null)
                return null;
            return _projectedBosses.Where(boss => !boss.OnField)
                .OrderByDescending(boss => boss.Difficulty).FirstOrDefault();
        }

        private void ReportReserveDecision(ClientCard source, int threat)
        {
            OpponentBoss worst = WorstBossStillComing();
            if (worst == null || source == null)
                return;
            int answers = CountAvailableAnswers();
            // Sem resposta nenhuma não há o que racionar: a única linha que a regra produziu nas partidas de
            // 2026-09-23 dizia "would hold ... and we have 0 answer(s)", que é conselho vazio.
            if (answers < 1 || answers > ReserveScarceAnswers)
                return;
            if (worst.Difficulty <= threat)
                return;
            // Guardar só faz sentido se a resposta ainda servir quando ele chegar. Contra ameaça de pé não serve:
            // ali o que resolve é remoção, então não adianta racionar negação por causa dela.
            string part;
            bool standing = StandingThreat(source, out part);
            Report("interaction", string.Format(
                "reserve (log only): would hold against {0} (threat {1}); {2} is worth {3} with {4} path(s) seen, and we have {5} answer(s){6}",
                source.Name ?? source.Id.ToString(), threat,
                worst.Name, worst.Difficulty, worst.Support.Count, answers,
                standing ? " — but this one stays on the field, so holding does not fix it" : ""));
        }

        // Guardar a resposta porque a memória diz que vem coisa pior. Exige três coisas ao mesmo tempo, para não
        // virar passividade: resposta escassa, a carta da vez não destacada, e uma carta destacada ainda de fora.
        private bool KnowledgeShouldHold(ClientCard source)
        {
            if (_knowledgeCounts == null || string.IsNullOrEmpty(_knowledgeArchetype))
                return false;
            int answers = CountAvailableAnswers();
            if (answers < 1 || answers > ReserveScarceAnswers)
                return false;
            int sample;
            double lift = KnowledgeLift(CardCode(source), out sample);
            if (lift >= KnowledgeLiftBar)
                return false;   // esta já é a carta que costuma nos quebrar: gasta agora
            int waitingId = KnowledgeWorstStillUnseen();
            if (waitingId == 0)
                return false;
            var data = YGOSharp.OCGWrapper.NamedCard.Get(waitingId);
            Report("interaction", string.Format(
                "memory says hold: {0} is not the one that hurts us here; {1} still has not shown up and we have {2} answer(s)",
                source.Name ?? source.Id.ToString(), data != null ? data.Name : waitingId.ToString(), answers));
            return true;
        }

        // A carta de maior lift neste arquétipo que AINDA não resolveu neste duelo. 0 se não há nenhuma.
        private int KnowledgeWorstStillUnseen()
        {
            string prefix = _knowledgeArchetype + "|";
            int best = 0;
            double bestLift = KnowledgeLiftBar;
            foreach (var entry in _knowledgeCounts)
            {
                if (!entry.Key.StartsWith(prefix, StringComparison.Ordinal)
                    || !entry.Key.EndsWith("|" + OutcomeSeen, StringComparison.Ordinal))
                    continue;
                string middle = entry.Key.Substring(prefix.Length, entry.Key.Length - prefix.Length - OutcomeSeen.Length - 1);
                int cardId;
                if (!int.TryParse(middle, out cardId) || _opponentResolvedThisDuel.Contains(cardId))
                    continue;
                int sample;
                double lift = KnowledgeLift(cardId, out sample);
                if (lift >= bestLift) { bestLift = lift; best = cardId; }
            }
            return best;
        }

        private bool OpponentEffectWorthNegating(ClientCard source)
        {
            if (Duel.Player != 1)
                return true;
            // Efeito que tira nossas cartas, quebra a nossa receita, trava ou nega: não espera a segunda ativação.
            int threat = OpponentEffectThreat(source);
            ReportReserveDecision(source, threat);
            // MEMÓRIA LIGADA NA DECISÃO. O perfil de efeito diz o que a carta FAZ; a memória diz o que costuma
            // acontecer com a gente depois dela neste arquétipo. Quando o histórico destoa da média do deck, ele
            // vale mais que a categoria: o Called by the Grave é "banish" e o Brightest Blazing Branded King é
            // "disable", categorias que sozinhas não chegam ao limiar, mas são as duas cartas que mais derrubam
            // o nosso plano nos 10 duelos medidos contra Branded.
            if (source != null && KnowledgeDecides)
            {
                int sample;
                double lift = KnowledgeLift(CardCode(source), out sample);
                if (lift >= KnowledgeLiftBar)
                {
                    Report("interaction", string.Format(
                        "memory says answer now: {0} (lift {1:0.0} over {2} duel(s) against {3})",
                        source.Name ?? source.Id.ToString(), lift, sample, _knowledgeArchetype));
                    return true;
                }
                // Interceptar mais cedo: esta carta não é a que nos quebra, mas é a que costuma vir logo antes
                // dela. Parar aqui custa menos do que parar depois, quando ele já tem a peça na mão.
                double lead = KnowledgeLiftFor(CardCode(source), OutcomePre, out sample);
                if (lead >= KnowledgeLiftBar)
                {
                    Report("interaction", string.Format(
                        "memory says cut it early: {0} usually comes right before what breaks us (lead lift {1:0.0} over {2} duel(s))",
                        source.Name ?? source.Id.ToString(), lead, sample));
                    return true;
                }
            }
            if (threat >= ThreatWorthEarly)
            {
                Report("interaction", string.Format("worth answering now: {0} (threat {1})",
                    source != null ? (source.Name ?? source.Id.ToString()) : "opponent effect", threat));
                return true;
            }
            // O lado oposto da mesma memória: se a carta da vez NÃO destoa, e ainda existe no arquétipo uma que
            // destoa e que não apareceu neste duelo, a resposta escassa vale mais guardada. É a regra que teria
            // ganho o duelo 012308 — lá o Harmonia foi gasto num buscador e o que faltou foi resposta depois.
            if (source != null && KnowledgeDecides && KnowledgeShouldHold(source))
                return false;
            if (Duel.ChainTargets.Any(card => card != null && card.Controller == 0))
                return true;
            if (source != null && (source.IsFloodgate() || source.IsMonsterShouldBeDisabledBeforeItUseEffect() || source.IsMonsterDangerous()))
                return true;
            return _opponentActivationsThisTurn >= 2;
        }

        // PURPOSE: Ash Blossom: condições do WindBot (efeito do oponente que busca, invoca do Deck, manda ao GY…, ignorando as
        //          cartas da lista dele) + a regra do 1º efeito no turno do oponente.
        private bool AshResponse()
        {
            if (!DefaultAshBlossomAndJoyousSpring())
                return false;
            ClientCard source = Util.GetLastChainCard();
            string name = source != null ? (source.Name ?? source.Id.ToString()) : "opponent effect";
            if (!OpponentEffectWorthNegating(source))
            {
                Report("interaction", "Ash stops it: 1st opponent effect this turn (" + name + ")");
                return false;
            }
            Report("interaction", "Ash negates " + name);
            return true;
        }

        // PURPOSE: Infinite Impermanence: nega o monstro do oponente que está ativando (regra do 1º efeito no turno dele) ou, sem
        //          chain no turno dele, o monstro da lista "negar antes de usar" do WindBot. Sem alvo nosso: casos do padrão
        //          (Eater of Millions na batalha, Utopia the Lightning). O alvo segue o ranking de ameaça.
        private bool ImpermanenceResponse()
        {
            if (DefaultCheckWhetherCardIsNegated(Card))
                return false;
            ClientCard last = Util.GetLastChainCard();
            if (last != null && ((last.IsCode(_CardId.GalaxySoldier) && Enemy.Hand.Count >= 3) || last.IsCode(_CardId.EffectVeiler, _CardId.InfiniteImpermanence)))
                return false;
            ClientCard target = null;
            string reason = null;
            if (last != null && last.Controller == 1 && last.Location == CardLocation.MonsterZone && Duel.LastChainPlayer == 1
                && !last.IsDisabled() && CanBeTargetedBy(last, TargetKind.SpellTrap))
            {
                if (!OpponentEffectWorthNegating(last))
                {
                    Report("interaction", "Impermanence stops it: 1st opponent effect this turn (" + (last.Name ?? last.Id.ToString()) + ")");
                    return false;
                }
                target = last;
                reason = "negates the effect on activation";
            }
            else if (Duel.Player == 1)
            {
                target = Enemy.GetMonsters().Where(card => card != null && card.IsFaceup() && !card.IsDisabled()
                        && card.IsMonsterShouldBeDisabledBeforeItUseEffect() && CanBeTargetedBy(card, TargetKind.SpellTrap))
                    .OrderByDescending(EnemyTargetRank).FirstOrDefault();
                reason = "negates before the effect is used (WindBot list)";
            }
            else if (Duel.Player == 0 && Duel.Phase == DuelPhase.Main1 && (_comboDone || _plannerDisabledThisTurn) && Duel.CurrentChain.Count == 0
                && !GaiaRdaLineReady() && Bot.GetMonsters().Any(card => card != null && card.IsFaceup() && card.IsAttack()))
            {
                // Nosso turno, antes da batalha (jogador): nega o monstro que reflete dano ou não sai em batalha para liberar o ataque.
                target = BattleBlockers().Where(card => card.IsFaceup() && !card.IsDisabled() && CanBeTargetedBy(card, TargetKind.SpellTrap))
                    .OrderByDescending(EnemyTargetRank).FirstOrDefault();
                reason = "removes the battle protection / reflect before the attack";
            }
            if (target == null && (last == null || last.Controller != 1))
            {
                target = DefaultGetDisableMonsterTarget();
                reason = "WindBot default case";
            }
            if (target == null)
                return false;
            AI.SelectCard(target);
            // O AI.SelectCard sozinho não bastou: no duelo contra Labrynth de 2026-09-23 (log 041150 t3) a regra
            // escolheu a Lady Labrynth, que tinha acabado de ativar, e o alvo que saiu foi a Lovely Labrynth —
            // o SelectForResponse sequestrou a escolha porque um _responseKind de antes ainda estava armado.
            // Negar um monstro que não está ativando nada desperdiça a carta, então o alvo fica guardado aqui e
            // o OnSelectCard o respeita antes de qualquer outra regra de seleção.
            _impermanenceTarget = target;
            Report("interaction", string.Format("Impermanence on {0} ({1})", target.Name ?? target.Id.ToString(), reason));
            return true;
        }

        // PURPOSE: executor das respostas rápidas (ativa só a carta escolhida, com a descrição certa).
        private bool QuickResponse()
        {
            // Pergunta "usar o efeito?" logo depois de ativar a resposta (ex.: Red Zone baixada: a 1ª ativação só vira a
            // carta, e o jogo pergunta se usa o efeito; no teste a resposta padrão "não" desperdiçou a Red Zone).
            if (_responseFresh && Card == _responseSource)
                return true;
            if (_prompt != PromptKind.Chain || Duel.CurrentChain.Count == 0)
                return false;

            ClientCard target;
            ResponseKind kind = ChooseResponse(out target);
            int code = CardCode(Card);
            int desc = ActivateDescription;
            bool match;
            switch (kind)
            {
                case ResponseKind.Abyss: match = code == CardId.HotRedDragonArchfiendAbyss && IsEffectDescription(Card, desc, 0); break;
                case ResponseKind.DisPaterNegate:
                case ResponseKind.DisPaterDestroy:
                    {
                        // Os dois modos podem vir como ativações separadas (descrição 0 = banida do oponente, 1 = nossa)
                        // ou como uma ativação só com a escolha depois (OnSelectOption). Na primeira forma, só aceita o modo certo.
                        bool isNegate = desc > 0 && IsEffectDescription(Card, desc, 0);
                        bool isDestroy = desc > 0 && IsEffectDescription(Card, desc, 1);
                        bool wanted = kind == ResponseKind.DisPaterNegate ? isNegate : isDestroy;
                        match = code == CardId.BystialDisPater && (wanted || (!isNegate && !isDestroy));
                        break;
                    }
                case ResponseKind.ZalenFirst:
                case ResponseKind.ZalenSecond: match = code == CardId.ZalenTheShackledDragon; break;
                case ResponseKind.Quetzacoatl: match = code == CardId.CrimsonDragonQuetzacoatl && IsEffectDescription(Card, desc, 1); break;
                case ResponseKind.King: match = code == CardId.TheCrimsonKing && IsEffectDescription(Card, desc, 1); break;
                case ResponseKind.Dominus:
                    match = code == CardId.DominusImpulse && Card.Location == CardLocation.SpellZone;
                    break;
                case ResponseKind.Harmonia:
                    match = code == CardId.FydraulisHarmonia && Card.Location == CardLocation.Hand;
                    break;
                case ResponseKind.RedZone:
                    // Red Zone numa coluna negada pela Impermanence: o efeito seria negado.
                    match = code == CardId.RedZone && IsEffectDescription(Card, desc, 0)
                        && !(Card.Location == CardLocation.SpellZone && infiniteImpermanenceNegatedColumns.Contains(Card.Sequence));
                    break;
                default: match = false; break;
            }
            if (!match)
                return false;

            _responseKind = kind;
            _responseSource = Card;
            _responseTarget = target;
            _responseFresh = true;
            if (kind == ResponseKind.Harmonia)
                _harmoniaUsedThisTurn = true;
            // Guarda qual carta gastou a negação dela neste turno. Nas RESPOSTAS isto não faria falta (o jogo só oferece
            // o que dá para usar, e o OfferedNow confia nisso), mas no CUSTO da Quetzacoatl não existe esse sinal: a
            // lista de Synchros chega sem dizer quem ainda tem negação na manga. Jogador, 2026-09-22.
            if (kind == ResponseKind.Abyss || kind == ResponseKind.DisPaterNegate || kind == ResponseKind.DisPaterDestroy
                || kind == ResponseKind.ZalenFirst || kind == ResponseKind.ZalenSecond || kind == ResponseKind.Quetzacoatl)
                _negateUsedThisTurn.Add(CardCode(Card));
            if (kind == ResponseKind.King)
                _rdaSynchroThisDuel = true; // o RDA do King é tratado como Invocação-Synchro (libera o Burning Soul)
            Report("interaction", string.Format("response: {0} against {1}", kind, target != null ? (target.Name ?? target.Id.ToString()) : "opponent effect"));
            return true;
        }

        // A seleção atual pertence à nossa resposta?
        private bool SelectionIsForResponse()
        {
            if (_responseKind == ResponseKind.None || _responseSource == null)
                return false;
            return _responseFresh || Duel.GetCurrentChainCard() == _responseSource || Duel.GetCurrentSolvingChainCard() == _responseSource;
        }

        // Ordem de envio ao GY pela Harmonia. 0 = Storm-Bane quando há Dragão banido (ele volta e traz o banido);
        // 1 = RDA/Scarred, que trabalham do GY (Crimson Gaia ③ revive o RDA, Scarred ② invoca o RDA no turno do
        // oponente); 2 = o resto; 3 = Storm-Bane sem alvo banido, que no GY não faz nada e não volta.
        private int HarmoniaSendPriority(ClientCard card, bool stormBaneUsable)
        {
            int code = CardCode(card);
            if (code == CardId.StormBaneDragonDestorbim) return stormBaneUsable ? 0 : 3;
            if (code == CardId.RedDragonArchfiend || code == CardId.ScarredDragonArchfiend) return 1;
            return 2;
        }

        private IList<ClientCard> SelectForResponse(IList<ClientCard> cards, int min, int max)
        {
            if (_responseKind == ResponseKind.Harmonia)
            {
                var ours = cards.Where(card => card != null && card.Controller == 0 && card.Location == CardLocation.Extra).ToList();
                if (ours.Count > 0)
                {
                    // Revelar o máximo (6 = se invoca + manda + destrói) e mandar ao GY. O Storm-Bane vinha sempre em
                    // primeiro, mas o efeito ② dele exige um Dragão nosso banido: sem alvo banido ele só vai ao GY e morre
                    // lá, porque monstro do Extra que foi direto ao GY sem ser invocado não volta ao campo (jogador,
                    // 2026-09-22). Sem alvo, mandamos RDA/Scarred, que têm efeito próprio no GY.
                    bool stormBaneUsable = Bot.Banished.Any(card => card != null && card.HasRace(CardRace.Dragon));
                    var ordered = ours.OrderBy(card => HarmoniaSendPriority(card, stormBaneUsable))
                        .ThenBy(card => RdaEvaluator.BoardValue(CardCode(card))).ToList();
                    int count = max <= 1 ? 1 : Math.Min(max, ordered.Count);
                    // Revelar 6 obriga a destruir 1 monstro. Se todos se aproveitam de ser destruídos (e nenhum é ameaça das
                    // listas), revela até 4 Synchros: o Storm-Bane ainda vai ao GY, mas nada é destruído.
                    if (count > 4 && !HasWorthwhileDestroyTarget())
                        count = Math.Max(min, 4);
                    if (count < min)
                        return null;
                    var selected = ordered.Take(count).ToList();
                    // O servidor chama esta escolha mais de uma vez na mesma resolução (revelar, depois mandar ao GY).
                    // A mensagem antiga dizia "reveals 1" em todas elas, o que fazia parecer que só uma carta tinha
                    // sido revelada quando na verdade aquele aviso era o da escolha de UMA para o GY. Confundiu a
                    // leitura do log 012308 em 2026-09-23; agora o texto diz qual das duas perguntas é.
                    Report("interaction", count > 1
                        ? string.Format("Harmonia reveals {0}, first to the GY is {1} ({2})",
                            count, RdaCards.Name(CardCode(selected[0])),
                            stormBaneUsable ? "there is a banished Dragon for Storm-Bane" : "no banished Dragon: Storm-Bane stays in the Extra Deck")
                        : string.Format("Harmonia sends {0} to the GY ({1})",
                            RdaCards.Name(CardCode(selected[0])),
                            stormBaneUsable ? "there is a banished Dragon for Storm-Bane" : "no banished Dragon: Storm-Bane stays in the Extra Deck"));
                    return selected;
                }
                // Destrói sem alvo (6 reveladas): o melhor monstro pelo ranking de ameaça.
                ClientCard victim = cards.FirstOrDefault(card => card == _responseTarget)
                    ?? BestEnemyTarget(cards, TargetKind.NoTarget, Removal.Destroy);
                return victim != null ? new List<ClientCard> { victim } : null;
            }
            ClientCard pick = null;
            switch (_responseKind)
            {
                case ResponseKind.DisPaterNegate:
                    pick = cards.FirstOrDefault(card => card != null && card.Controller == 1);
                    break;
                case ResponseKind.DisPaterDestroy:
                    // Nossa carta banida que menos falta faz: não-monstro primeiro, depois o monstro de menor nível.
                    pick = cards.Where(card => card != null && card.Controller == 0)
                        .OrderBy(card => card.IsMonster() ? 1 : 0).ThenBy(card => card.Level).FirstOrDefault();
                    break;
                case ResponseKind.Quetzacoatl:
                    {
                        // Custo: devolve ao Extra o Synchro que menos faz falta. Ficam de fora o RDA trazido pela Crimson Gaia
                        // (é a salvação do deck) e o RDA protegido pelo King ②.
                        //
                        // A penalidade antiga era "já atacou vai para o fim", por causa do replay de 2026-09-15. Mas aquilo vale
                        // para o monstro que está EM BATALHA agora, não para quem já terminou de atacar. Com a penalidade errada o
                        // RDA levava dois castigos (atacou + ser RDA) e nunca era escolhido: nos logs de 2026-09-22 o bot devolveu
                        // o Burning Soul (191047 seq 722) e o Crimson King (191638 seq 828) tendo o RDA comum disponível.
                        // O RDA comum que já atacou é o MELHOR custo (jogador): o ataque dele já foi feito, do Extra ele volta, e
                        // sair do campo ainda cancela a destruição dos NOSSOS monstros na End Phase.
                        var options = cards.Where(card => card != null && card.Controller == 0).ToList();
                        var allowed = options.Where(card => !_gaiaRdaCards.Contains(card) && !_kingRdaCards.Contains(card)).ToList();
                        pick = (allowed.Count > 0 ? allowed : options)
                            .OrderBy(card => InBattleNow(card) ? 1 : 0)
                            .ThenBy(card => StillHoldsNegate(card) ? 1 : 0)   // negação na manga fica na mesa
                            .ThenBy(card => PreferredQuetzacoatlCost(card) ? 0 : 1)
                            .ThenBy(card => CardCode(card) == CardId.RedHypernovaDragon || CardCode(card) == CardId.RedSupernovaDragon ? 1 : 0)
                            .ThenBy(card => RdaEvaluator.BoardValue(CardCode(card))).FirstOrDefault();
                        break;
                    }
                case ResponseKind.Abyss:
                    pick = cards.FirstOrDefault(card => card == _responseTarget)
                        ?? BestEnemyTarget(cards, TargetKind.MonsterEffect)
                        ?? cards.FirstOrDefault(card => card != null && card.Controller == 1);
                    break;
                case ResponseKind.RedZone:
                    // Nunca um card nosso: o melhor monstro do oponente (o alvo guardado, se ainda estiver disponível).
                    pick = cards.FirstOrDefault(card => card == _responseTarget)
                        ?? BestEnemyTarget(cards, TargetKind.SpellTrap, Removal.Destroy, includeSpells: true)
                        ?? cards.FirstOrDefault(card => card != null && card.Controller == 1);
                    break;
            }
            if (pick == null || min > 1)
                return null;
            return new List<ClientCard> { pick };
        }

        // -----------------------------------------------------------------------------------------------------
        // 2.7 Prompts de seleção: cartas, opções e zonas
        // -----------------------------------------------------------------------------------------------------

        // Ação dona do prompt atual: o elo da chain sendo ativado/resolvido, ou a última ação sem chain.
        private PlanAction CurrentSelectionAction()
        {
            ClientCard source = Duel.GetCurrentChainCard() ?? Duel.GetCurrentSolvingChainCard();
            if (source == null)
                return _currentAction;

            PlanAction action;
            if (_actionBySource.TryGetValue(source, out action))
                return action;
            // O objeto da carta pode mudar ao trocar de zona; tenta pelo código.
            return _actionBySource.Values.LastOrDefault(item => item.CardId == CardCode(source));
        }

        private ClientCard _impermanenceTarget;

        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, int hint, bool cancelable)
        {
            IList<ClientCard> handled = base.OnSelectCard(cards, min, max, hint, cancelable);
            if (handled != null)
                return handled;
            IList<ClientCard> audit = SelectForAuditEffects(cards, min, max);
            if (audit != null)
                return audit;
            // Alvo já decidido pela regra da Impermanence: tem precedência sobre as outras seleções.
            if (min <= 1 && _impermanenceTarget != null)
            {
                ClientCard wanted = cards.FirstOrDefault(card => card == _impermanenceTarget);
                _impermanenceTarget = null;
                if (wanted != null)
                    return new List<ClientCard> { wanted };
            }
            // Mesma guarda do SelectForAuditEffects: este bloco devolve UMA carta.
            if (min <= 1 && _novaReviveSource != null)
            {
                ClientCard nova = cards.FirstOrDefault(card => card != null && card.Controller == 0 && CardCode(card) == CardId.RedHypernovaDragon)
                    ?? cards.FirstOrDefault(card => card != null && card.Controller == 0 && CardCode(card) == CardId.RedSupernovaDragon)
                    ?? cards.Where(card => card != null && card.Controller == 0 && card.HasType(CardType.Synchro)).OrderByDescending(ReviveRank).FirstOrDefault();
                if (nova != null)
                {
                    _novaReviveSource = null;
                    return new List<ClientCard> { nova };
                }
            }
            if (SelectionIsForResponse())
            {
                IList<ClientCard> response = SelectForResponse(cards, min, max);
                if (response != null)
                    return response;
            }
            // Materiais de Synchro: o seletor de AI.SelectMaterials cuida.
            if (Duel.Player != 0 || hint == HintMsg.SynchroMaterial)
                return null;

            // Busca da Magnamhut na End Phase: resolve sem prompt de ativação, só pede a carta.
            // No teste a escolha padrão pegou Power Vice Dragon; o alvo é a Fydraulis Harmonia.
            if (Duel.Phase == DuelPhase.End && hint == HintMsg.AddToHand && (_turnFlags & RdaState.FlagMagnamhutEndPhase) != 0)
            {
                ClientCard harmonia = cards.FirstOrDefault(card => card.IsCode(CardId.FydraulisHarmonia));
                if (harmonia != null)
                {
                    return new List<ClientCard> { harmonia };
                }
            }
            return SelectByPlan(cards, min, max);
        }

        public override IList<ClientCard> OnSelectTribute(IList<ClientCard> cards, int min, int max, int hint, bool cancelable)
        {
            // Lubellion (procedimento) pode pedir o monstro liberado por aqui.
            if (Duel.Player != 0)
                return null;
            return SelectByPlan(cards, min, max);
        }

        // PURPOSE: escolher as cartas guardadas no passo, em qualquer ordem.
        //          Casa pelo CardId (IsCode, como os outros executors); o local só desempata
        //          (ex.: Lubellion no Deck x no GY, Crimson Gaia buscando do Deck x do GY).
        // TRUSTS: null = deixa a escolha padrão do framework (e registra os candidatos no log).
        private IList<ClientCard> SelectByPlan(IList<ClientCard> cards, int min, int max)
        {
            PlanAction action = CurrentSelectionAction();
            List<RdaPick> remaining;
            if (action == null || !_remainingPicks.TryGetValue(action, out remaining))
                return null;

            // Quetzacoatl revive: com a linha RDA + Crimson Gaia pronta e o RDA no GY, o RDA entra no lugar da peça de menor
            // valor entre as escolhidas. Antes a troca só valia para o Crimson King, então um plano que revivia a Crimson Blade
            // (valor 12, menor que os 25 do King) deixava o RDA no GY e perdia a limpeza da mesa (jogador, replay de 2026-09-16).
            // O planejador ordena os revives por BoardValue decrescente, e o RDA vale 8: sem esta troca ele quase nunca volta.
            // Só troca peça da classe dos extensores (valor até o da Blade): Abyss, Zalen, King, Dis Pater, Burning Soul e os
            // novas ficam na mesa, porque o RDA só tem prioridade quando já temos proteção suficiente.
            if (action.Effect == RdaKey.QuetzacoatlRevive && GaiaRdaLineReady()
                && !remaining.Any(pick => pick.Id == CardId.RedDragonArchfiend)
                && cards.Any(card => card != null && CardCode(card) == CardId.RedDragonArchfiend))
            {
                double limit = RdaEvaluator.BoardValue(CardId.CrimsonBladeDragon);
                int worst = -1;
                for (int i = 0; i < remaining.Count; ++i)
                {
                    if (remaining[i].Location != CardLocation.Grave || RdaEvaluator.BoardValue(remaining[i].Id) > limit)
                        continue;
                    if (worst < 0 || RdaEvaluator.BoardValue(remaining[i].Id) < RdaEvaluator.BoardValue(remaining[worst].Id))
                        worst = i;
                }
                if (worst >= 0)
                {
                    Report("interaction", string.Format("Quetzacoatl revives the RDA in place of {0} (RDA + Crimson Gaia line)",
                        CardName(remaining[worst].Id)));
                    remaining = new List<RdaPick>(remaining);
                    remaining[worst] = new RdaPick(CardId.RedDragonArchfiend, CardLocation.Grave);
                    _remainingPicks[action] = remaining;
                }
            }

            var pending = new List<RdaPick>(remaining);
            var result = new List<ClientCard>();
            bool progress = true;
            while (result.Count < max && progress)
            {
                progress = false;
                for (int p = 0; p < pending.Count; ++p)
                {
                    RdaPick pick = pending[p];
                    var sameId = cards.Where(card => card != null && !result.Contains(card) && card.IsCode(pick.Id)).ToList();
                    ClientCard match = sameId.FirstOrDefault(card => card.Location == pick.Location) ?? sameId.FirstOrDefault();
                    if (match == null)
                        continue;
                    result.Add(match);
                    pending.RemoveAt(p);
                    progress = true;
                    break;
                }
            }

            if (result.Count < min)
            {
                if (remaining.Count > 0)
                    LogNote("plan_select", string.Format("pick not found for \"{0}\" | wanted [{1}] | candidates [{2}]",
                        action.Text,
                        string.Join(", ", remaining.Select(pick => pick.Id + "@" + pick.Location)),
                        string.Join(", ", cards.Select(card => card.Id + "/alias " + card.Alias + "@" + card.Location))));
                return null;
            }

            _remainingPicks[action] = pending;
            return result;
        }

        // PURPOSE: opções de efeito que o plano já decidiu (valores confirmados em cards.cdb).
        public override int OnSelectOption(IList<int> options)
        {
            // Opções das respostas rápidas: Dis Pater (banida do oponente = nega / nossa = destrói) e Zalen.
            if (SelectionIsForResponse())
            {
                int wantedResponse = -1;
                switch (_responseKind)
                {
                    case ResponseKind.DisPaterNegate: wantedResponse = Util.GetStringId(CardId.BystialDisPater, 0); break;
                    case ResponseKind.DisPaterDestroy: wantedResponse = Util.GetStringId(CardId.BystialDisPater, 1); break;
                    case ResponseKind.ZalenSecond: wantedResponse = Util.GetStringId(CardId.ZalenTheShackledDragon, 2); break;
                    case ResponseKind.ZalenFirst: wantedResponse = Util.GetStringId(CardId.ZalenTheShackledDragon, 3); break;
                }
                int responseIndex = options.IndexOf(wantedResponse);
                if (responseIndex >= 0)
                    return responseIndex;
                if (wantedResponse >= 0)
                    LogNote("plan_option", string.Format("response option {0} not found in [{1}]", _responseKind, string.Join(",", options)));
            }

            PlanAction action = CurrentSelectionAction();

            // Invocação-Normal com o Darkness Resonator ativo: escolher entre a normal e a extra.
            int extraSummonOption = Util.GetStringId(CardId.DarknessResonator, DarknessOptionExtraSummon);
            int extraIndex = options.IndexOf(extraSummonOption);
            if (extraIndex >= 0 && action != null && (action.Kind == PlanKind.NormalSummon || action.Kind == PlanKind.ExtraNormalSummon))
            {
                if (action.Kind == PlanKind.ExtraNormalSummon)
                    return extraIndex;
                for (int i = 0; i < options.Count; ++i)
                    if (i != extraIndex)
                        return i;
            }

            // Burning Soul tem dois procedimentos (Synchro e pelo GY): o jogo pergunta qual usar.
            if (action != null && action.CardId == RdaCards.BurningSoul
                && (action.Kind == PlanKind.SpecialProc || action.Kind == PlanKind.Synchro) && options.Count > 1)
            {
                int synchroIndex = options.IndexOf(SystemStringSynchroSummon);
                if (synchroIndex >= 0)
                {
                    if (action.Kind == PlanKind.Synchro)
                        return synchroIndex;
                    for (int i = 0; i < options.Count; ++i)
                        if (i != synchroIndex)
                            return i;
                }
                LogNote("plan_option", "Burning Soul options: [" + string.Join(",", options) + "] for: " + action.Text);
            }

            if (action != null)
            {
                int wanted = -1;
                switch (action.Effect)
                {
                    case RdaKey.BoneLevel:
                        wanted = Util.GetStringId(CardId.BoneArchfiend, action.LevelDelta > 0 ? BoneOptionIncrease : BoneOptionDecrease);
                        break;
                    case RdaKey.FiendPieceLevel:
                        wanted = Util.GetStringId(CardId.FiendPieceGolem, action.LevelDelta == -1 ? FiendPieceOptionReduce1 : FiendPieceOptionReduce2);
                        break;
                    case RdaKey.BladeTake:
                        // A ação do plano diz se é Invocar (negado) ou adicionar à mão. Antes a regra era "Synchro = Invocar", e a
                        // Lubellion que o plano queria no campo foi para a mão (teste contra o Albaz, 23:19), quebrando o combo.
                        bool summon = action.Summon;
                        wanted = summon ? SystemStringSpecialSummon : SystemStringAddToHand;
                        break;
                }
                int index = options.IndexOf(wanted);
                if (wanted >= 0 && index >= 0)
                    return index;
                if (wanted >= 0)
                    LogNote("plan_option", string.Format("option {0} not found in [{1}] for: {2}", wanted, string.Join(",", options), action.Text));
            }
            return base.OnSelectOption(options);
        }

        // PURPOSE: Synchros vão para a Zona de Monstros Extra quando possível, como o modelo assume.
        // Zonas (jogador): o padrão do WindBot sempre começa pelo meio, previsível. Aqui a zona é sorteada entre as livres,
        // respeitando o que o combo precisa:
        //   - magias/armadilhas: fora das colunas negadas pela Infinite Impermanence baixada;
        //   - Hypernova/Supernova: zona principal quando houver (são banidos e voltam; a zona extra fica para outro Synchro);
        //   - outros monstros do Extra: zona extra primeiro (deixa as principais livres para os próximos Synchros);
        //   - monstros do Main Deck: zona principal sorteada.
        // Obs.: alguns cards do oponente usam o monstro da zona extra; mesmo assim ela é necessária para o combo.
        private readonly Random _placeRandom = new Random();

        private int RandomZone(int mask)
        {
            var bits = new List<int>();
            for (int i = 0; i < 7; ++i)
                if ((mask & (1 << i)) != 0)
                    bits.Add(1 << i);
            return bits.Count == 0 ? 0 : bits[_placeRandom.Next(bits.Count)];
        }

        private static bool IsNovaCode(int cardId)
        {
            return cardId == CardId.RedHypernovaDragon || cardId == CardId.RedSupernovaDragon || cardId == 99585850;
        }

        public override int OnSelectPlace(int cardId, int player, CardLocation location, int available)
        {
            if (player == 0 && location == CardLocation.SpellZone)
            {
                int zones = available & 0x1F;
                int safe = 0;
                for (int i = 0; i < 5; ++i)
                    if ((zones & (1 << i)) != 0 && !infiniteImpermanenceNegatedColumns.Contains(i))
                        safe |= 1 << i;
                int pick = RandomZone(safe != 0 ? safe : zones);
                if (pick != 0)
                    return pick;
            }
            if (player == 0 && location == CardLocation.MonsterZone)
            {
                int main = available & 0x1F;
                int extra = available & (Zones.z5 | Zones.z6);
                int pick;
                if (IsNovaCode(cardId) && main != 0)
                    pick = RandomZone(main);
                else if (extra != 0)
                    pick = RandomZone(extra);
                else
                    pick = RandomZone(main);
                if (pick != 0)
                    return pick;
            }
            return base.OnSelectPlace(cardId, player, location, available);
        }

        // PURPOSE: posição dos nossos monstros.
        // O DefaultExecutor coloca em defesa todo monstro com ATK base 0. A Crimson Dragon Quetzacoatl tem
        // 0 de ATK/DEF impressos e ganha ATK pelo próprio efeito: em defesa ela fica com 0 e é destruída fácil.
        // Regra do deck: Synchros e monstros do combo sempre em ataque (NUNCA Quetzacoatl em defesa).
        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            if (RdaCards.All.ContainsKey(cardId) && positions.Contains(CardPosition.FaceUpAttack))
                return CardPosition.FaceUpAttack;
            return base.OnSelectPosition(cardId, positions);
        }

        // PURPOSE: reposicionamento. Só a Crimson Dragon Quetzacoatl (jogador, 2026-09-16: "não quero que ele, somente
        // ele mude porque podemos perdê-lo de graça"). As outras cartas seguem o padrão do WindBot.
        // O DefaultMonsterRepos vira para defesa todo monstro com ATK 0 e, uma vez em defesa, só volta ao ataque se
        // ATK > DEF. A Quetzacoatl tira o ATK do próprio efeito: negada (ou sob um floodgate que só trava efeitos) o
        // ATK dela lê 0, o padrão a vira para defesa — onde a DEF também é 0 — e ela nunca volta sozinha, porque
        // 0 > 0 é falso. Foi o que aconteceu uma vez em jogo: negaram a Quetzacoatl e o bot a virou logo em seguida.
        // A negação costuma durar só até o fim do turno; o ATK não muda. Então ela fica sempre em ataque, e se um
        // efeito do oponente a forçou para defesa, volta ao ataque assim que o jogo permitir.
        private bool ReposGuard()
        {
            if (Card != null && CardCode(Card) == CardId.CrimsonDragonQuetzacoatl)
                return Card.IsFaceup() && Card.IsDefense();
            return DefaultMonsterRepos();
        }

        // PURPOSE: ordem de ataque.
        // Ataque direto: primeiro o monstro de maior ATK atual (Quetzacoatl, Hypernova/Supernova com Tuners no GY,
        // RDA do Crimson King, Burning Soul depois de adicionar carta...). Se o oponente interromper a batalha,
        // o dano maior já entrou. O padrão do WindBot atacava do mais fraco para o mais forte.
        // Com monstros do oponente no campo, mantém a lógica padrão (escolha de alvos).
        // Com monstros do oponente (regra do jogador, teste com Fydraulis Harmonia em defesa: a Quetzacoatl de 16600
        // gastou o ataque nela): cada alvo é atacado pelo nosso monstro MAIS FRACO que passa o ATK (ataque) ou a DEF
        // (defesa); os fortes ficam para o dano direto. Exceções:
        //   - Red Dragon Archfiend ataca primeiro um monstro em defesa que ele passa (destrói todos em defesa depois).
        //   - Letal na mesa e alvo é monstro de efeito em Posição de Ataque (pode destruir/baixar o atacante): o mais forte.
        //   - Monstro virado para baixo (DEF desconhecida): por último, com o mais fraco que sobrar.
        public override BattlePhaseAction OnBattle(IList<ClientCard> attackers, IList<ClientCard> defenders)
        {
            if (attackers.Count == 0)
                return base.OnBattle(attackers, defenders);
            foreach (ClientCard defender in defenders)
                RememberEnemyCard(defender);

            // Combo antes da batalha (jogador, 2026-09-15): com o combo pendente não ataca; vai para a Main Phase 2 terminar.
            // Única exceção: Abyss no campo com 1 zona principal livre (pode trazer um Resonator na batalha e melhorar a mesa).
            if (Duel.Player == 0 && !_comboDone && !_plannerDisabledThisTurn && !AbyssBattleWindow())
                return SkipBattleAction(attackers, defenders, "combo not finished yet");

            // Ataque extra da Crimson Call: ele pertence ao RDA. Antes daqui o bot escolhia o atacante mais forte que
            // ainda não tinha atacado (a Quetzacoatl) e perdia o efeito. Além do ataque em si, atacar de novo com o RDA
            // pune quem invocar monstro de surpresa (jogador, 2026-09-22).
            if (_crimsonCallChainAttack)
            {
                ClientCard callAttacker = attackers.FirstOrDefault(card => CardCode(card) == CardId.RedDragonArchfiend);
                _crimsonCallChainAttack = false;
                if (callAttacker != null)
                {
                    if (defenders.Count == 0)
                    {
                        Report("battle", "Crimson Call extra attack: Red Dragon Archfiend attacks directly");
                        return AI.Attack(callAttacker, null);
                    }
                    ClientCard callTarget = defenders.Where(card => !AvoidBattleTarget(card))
                        .Where(card => callAttacker.Attack > RequiredPower(card))
                        .OrderBy(card => RequiredPower(card)).FirstOrDefault();
                    if (callTarget != null)
                    {
                        Report("battle", "Crimson Call extra attack: Red Dragon Archfiend attacks " + (callTarget.Name ?? callTarget.Id.ToString()));
                        return AI.Attack(callAttacker, callTarget);
                    }
                }
            }

            if (defenders.Count == 0)
                return AI.Attack(attackers.OrderByDescending(card => card.Attack).First(), null);

            ClientCard rda = attackers.FirstOrDefault(card => CardCode(card) == CardId.RedDragonArchfiend);
            // Linha RDA + Crimson Gaia: o RDA declara ataque primeiro. A Gaia vira todos os monstros do oponente para defesa
            // com a face para baixo (reflexo e proteções param de valer) e, depois do cálculo de dano, o RDA destrói todos os
            // que estão em defesa. O alvo do ataque vira para cima e volta a ter os efeitos: nunca mirar um que não pode ser
            // destruído por efeito (jogador); entre os outros, o de menor DEF.
            if (rda != null && GaiaRdaLineReady())
            {
                ClientCard lineTarget = defenders.Where(card => (TraitsOf(card) & BattleTrait.EffectIndestructible) == 0)
                    .OrderBy(card => card.Defense).FirstOrDefault();
                if (lineTarget != null)
                    return AI.Attack(rda, lineTarget);
            }

            // Monstros que refletem o dano ou não saem em batalha (jogador): não atacar. Negação (Impermanence), banimento sem
            // alvo (Hypernova antes da batalha) ou a linha RDA + Gaia resolvem antes; aqui só ficam de fora dos alvos.
            var blocked = defenders.Where(AvoidBattleTarget).ToList();
            if (blocked.Count > 0)
            {
                Report("battle", "avoids attacking (reflects damage or is not destroyed by battle): " + string.Join(", ", blocked.Select(card => DescribeKnown(card))));
                var open = defenders.Where(card => !blocked.Contains(card)).ToList();
                if (open.Count == 0)
                    return SkipBattleAction(attackers, defenders, "no target worth attacking");
                defenders = open;
            }

            // Distribuição econômica: alvos do mais fácil ao mais difícil, cada um com o atacante mais fraco que o passa.
            var free = attackers.OrderBy(card => card.Attack).ToList();
            // Alvos: quem se aproveita de ser destruído fica por último (jogador, 2026-09-15), a não ser que seja ameaça das
            // listas do WindBot. Entre iguais, do mais fácil de passar ao mais difícil.
            // Monstro da Main Monster Zone SEMPRE antes do da Extra Monster Zone (jogador, 2026-09-17): não interessa a
            // posição nem o ATK do que está na Extra Zone, os alvos da zona principal vêm primeiro. As zonas 5 e 6 são a
            // Extra Monster Zone.
            var known = defenders.Where(card => !card.IsFacedown())
                .OrderBy(card => card.Sequence > 4 ? 1 : 0)
                .ThenBy(card => BenefitsFromRemoval(card, Removal.Destroy) && !IsListedThreat(card) ? 1 : 0)
                .ThenBy(RequiredPower).ToList();
            // Duas coisas diferentes, resolvidas separado (jogador, 2026-09-17):
            //   QUEM ataca QUEM  -> os alvos em defesa são servidos primeiro, então ficam com os atacantes mais fracos que
            //                       passam a DEF deles. Bater num monstro em defesa não causa dano, então gastar o
            //                       Quetzacoatl (ou outro de ATK alto) ali desperdiça o ataque dele.
            //   ORDEM dos ataques -> continua a de `known`: Main Monster Zone antes da Extra Monster Zone.
            var chosen = new Dictionary<ClientCard, ClientCard>(); // alvo -> atacante
            foreach (ClientCard target in known.OrderBy(card => card.IsDefense() ? 0 : 1).ThenBy(RequiredPower))
            {
                // Primeiro o caminho normal: o atacante mais fraco que SUPERA o alvo.
                ClientCard attacker = free.FirstOrDefault(card => card.Attack > RequiredPower(target));
                // Só se ninguém passar é que a Crimson Blade entra, para não gastá-la num alvo que outro resolve.
                if (attacker == null)
                    attacker = free.FirstOrDefault(card => BladeCanDestroy(card, target));
                if (attacker == null)
                    continue;
                chosen[target] = attacker;
                free.Remove(attacker);
            }
            var pairs = known.Where(chosen.ContainsKey)
                .Select(target => new KeyValuePair<ClientCard, ClientCard>(chosen[target], target)).ToList();
            bool allKnownCleared = pairs.Count == known.Count && known.Count == defenders.Count;
            bool lethal = allKnownCleared && free.Sum(card => (long)card.Attack) >= Enemy.LifePoints;

            if (rda != null)
            {
                // Sem a Gaia: o RDA mira um monstro em defesa; os outros em defesa são destruídos pelo efeito (menos quem não pode
                // ser destruído por efeito).
                ClientCard defenseTarget = defenders.Where(card => card.IsFaceup() && card.IsDefense() && rda.Attack > card.Defense
                        && (TraitsOf(card) & BattleTrait.EffectIndestructible) == 0)
                    .OrderByDescending(card => card.Defense).FirstOrDefault();
                if (defenseTarget != null)
                    return AI.Attack(rda, defenseTarget);
            }

            if (lethal)
            {
                ClientCard risky = defenders.Where(card => card.IsFaceup() && card.IsAttack() && card.HasType(CardType.Effect))
                    .OrderByDescending(card => card.Attack).FirstOrDefault();
                if (risky != null)
                    return AI.Attack(attackers.OrderByDescending(card => card.Attack).First(), risky);
            }

            if (pairs.Count > 0)
                return AI.Attack(pairs[0].Key, pairs[0].Value);

            // Carta virada para baixo: usa o id lembrado de antes de ela virar (KnownId). Quem é lembrado como imune a
            // destruição por efeito fica por último, para o RDA não gastar o ataque nele (jogador, 2026-09-17).
            ClientCard faceDown = defenders.Where(card => card.IsFacedown())
                .OrderBy(card => (TraitsOf(card) & BattleTrait.EffectIndestructible) != 0 ? 1 : 0).FirstOrDefault();
            if (faceDown != null && known.Count == 0)
                return AI.Attack(attackers.OrderBy(card => card.Attack).First(), faceDown);

            // Nenhum atacante SUPERA um alvo (o emparelhamento acima exige ATK estritamente maior).
            // Aqui não se devolve o controle ao WindBot: o OnBattle base retorna null e a rotina padrão dele
            // (DefaultExecutor.OnSelectAttackTarget) troca de propósito com ATK igual quando é o último atacante
            // (`attacker.RealPower >= defender.RealPower && attacker.IsLastAttacker`). Foi assim que o King (2500)
            // trocou de graça com o Albion (2500). Sem alvo que a gente passe, não ataca.
            return SkipBattleAction(attackers, defenders, "no attacker beats the opponent monsters");
        }

        // -----------------------------------------------------------------------------------------------------
        // Proteções do oponente (jogador, 2026-09-15 e 2026-09-18, partidas contra Yubel)
        //   Reflect              ao ser atacado, causa dano a nós (Yubel: dano igual ao ATK do atacante).
        //   BattleIndestructible não é destruído em batalha.
        //   EffectIndestructible não é destruído por efeito.
        //   ImmuneEffect         NÃO É AFETADO por efeitos: mirar nele desperdiça o efeito inteiro, não só a destruição.
        //
        // DUAS FONTES, nesta ordem:
        //   1. Script Lua do cliente (RdaScripts). É exato: a proteção é uma constante declarada, sem ambiguidade de
        //      redação, e o tipo do efeito diz se vale para a própria carta (EFFECT_TYPE_SINGLE) ou para outras
        //      (EFFECT_TYPE_FIELD). Medido em 2026-09-18 contra o texto em inglês: o texto errava 155 cartas para mais
        //      e 32 para menos só na proteção por efeito, quase sempre porque confundia a cláusula que protege OUTRAS
        //      cartas com auto-proteção ("Zombie monsters you control cannot be destroyed by card effects").
        //   2. Texto em inglês do cards.cdb, quando não há script. É o comportamento antigo, com as taxas de erro acima.
        // O reflexo continua vindo do texto nos dois casos: não existe constante única para ele (o Yubel faz por
        // gatilho, EVENT_BATTLE_CONFIRM com CATEGORY_DAMAGE).
        //
        // Com a face para baixo o texto não vale; o id visto antes de virar fica em _knownEnemyIds.
        // -----------------------------------------------------------------------------------------------------
        [Flags]
        private enum BattleTrait { None = 0, Reflect = 1, BattleIndestructible = 2, EffectIndestructible = 4, ImmuneEffect = 8 }

        private static readonly Dictionary<int, BattleTrait> BattleTraitCache = new Dictionary<int, BattleTrait>();
        private static readonly System.Text.RegularExpressions.Regex ReflectText = new System.Text.RegularExpressions.Regex(
            @"damage (?:to|inflicted to) (?:your opponent|the attacking player)[^.]{0,60}?equal to (?:the |that )?(?:attacking monster's|its|that monster's) atk|(?:your opponent|the opponent) takes? (?:the )?battle damage instead|battle damage[^.]{0,40}?is (?:also )?inflicted to your opponent|also inflicts? (?:the same amount of )?damage to your opponent",
            System.Text.RegularExpressions.RegexOptions.Compiled);
        // Medido no cards.cdb em 2026-09-17: a versão anterior pegava 385 cartas, esta pega 414. As 29 que faltavam vinham
        // de três formulações comuns e a mais frequente era "battle or YOUR OPPONENT'S card effects" — a expressão antiga
        // aceitava "battle or card effects" e "battle or by card effects", mas não com o possessivo no meio (Code Talker,
        // Follow Wing, Evil Eye of Selene, K9-ØØ "Hound"). As outras duas: "an opponent's" no lugar de "your opponent's"
        // (El Shaddoll Winda) e a vírgula de "destroyed, or banished, by card effects" (Icejade Gymir Aegirine).
        private static readonly System.Text.RegularExpressions.Regex EffectIndestructibleText = new System.Text.RegularExpressions.Regex(
            @"cannot be destroyed(?:, or banished,)? by (?:(?:your|an) opponent's )?card effects"
            + @"|cannot be destroyed by battle or (?:by )?(?:(?:your|an) opponent's )?card effects",
            System.Text.RegularExpressions.RegexOptions.Compiled);

        // Um efeito nosso da Crimson Gaia está ativando ou resolvendo agora?
        private bool GaiaLinkResolving()
        {
            ClientCard solving = Duel.GetCurrentSolvingChainCard();
            ClientCard activating = Duel.GetCurrentChainCard();
            return (solving != null && solving.Controller == 0 && CardCode(solving) == CardId.CrimsonGaia)
                || (activating != null && activating.Controller == 0 && CardCode(activating) == CardId.CrimsonGaia);
        }

        private void RememberEnemyCard(ClientCard card)
        {
            if (card != null && card.Controller == 1 && card.Id != 0)
                _knownEnemyIds[card] = card.Id;
        }

        /// <summary>
        /// Cartas da mão do oponente que nós JÁ VIMOS. Normalmente a mão dele é só uma contagem, mas quando uma carta
        /// é revelada (busca que revela, custo revelado, efeito que mostra a mão) o framework grava o id no objeto e
        /// ele fica: GameBehavior.OnConfirmCards faz "if (cardId > 0) card.SetId(cardId)". Ou seja, a informação
        /// continua disponível depois da revelação — e até agora o executor não usava nada disso.
        /// Exemplo: log 20260922-153454 seq 50, o oponente revelou Fallen of Albaz na mão.
        /// </summary>
        /// <summary>
        /// Raça que o jogo está impondo a TODOS os nossos monstros do campo, ou 0 se nada foi trocado. O ClientCard
        /// traz raça, atributo, tipo e nível já atualizados pelo servidor (ClientCard.Update lê Query.Race etc.), então
        /// a verdade está aqui — a tabela do deck guarda só o que está impresso.
        /// Só devolve valor quando a troca é do campo inteiro, que é como essas cartas funcionam (Black Rose Garden:
        /// "all face-up monsters become Plant monsters"). Mudança em um monstro só não é modelada: nesses casos o menu
        /// é quem barra, e o replanejamento cuida.
        /// </summary>
        /// <summary>
        /// Confere o deck do nosso modelo contra o rastreamento que o próprio framework mantém (ClientField guarda
        /// as contagens ao vivo e expõe GetCardCountInDeck). Se divergir, o modelo está mentindo e todo plano feito
        /// em cima dele é suspeito — é a mesma família de bug que custou as duas partidas de 2026-09-22.
        /// Só registra no log, e só a primeira divergência de cada carta por duelo.
        /// </summary>
        private int[] ReadDeckChecked(List<int> deck)
        {
            int[] ordered = deck.OrderBy(code => code).ToArray();
            CheckDeckModel(ordered);
            return ordered;
        }

        private void CheckDeckModel(int[] modelDeck)
        {
            // CanQueryDeck é interno ao framework; DeckTrackingActive é a mesma condição, pública.
            if (!Bot.DeckTrackingActive)
                return;
            var counted = new Dictionary<int, int>();
            foreach (int id in modelDeck)
            {
                int already;
                counted.TryGetValue(id, out already);
                counted[id] = already + 1;
            }
            foreach (KeyValuePair<int, int> entry in counted)
            {
                int real = Bot.GetCardCountInDeck(entry.Key);
                if (real == entry.Value || !_deckDriftReported.Add(entry.Key))
                    continue;
                Report("deck_check", string.Format("deck model drift: {0} model {1} x tracked {2}",
                    CardName(entry.Key), entry.Value, real));
            }
        }

        // Troca de característica no campo, do tipo "all face-up monsters on the field become X".
        //
        // Duas falhas que custaram o duelo 042750 de 2026-09-25, contra o deck das Rosas:
        //
        // 1. Só olhava os NOSSOS monstros. O efeito do Black Rose Garden alcança o campo inteiro, e no começo do
        //    nosso turno a nossa mesa estava vazia — então não havia o que amostrar e o planejador montou um plano
        //    de 39 passos sem saber que tudo viraria Planta. A mesa DELE estava cheia de Plantas, ou seja, o dado
        //    existia e nós não olhávamos. Agora amostra os dois lados.
        //
        // 2. Desistia se UM monstro estivesse com a raça impressa. Num deck de Plantas isso é comum (o Glow-Up Bulb
        //    já é Planta de origem), e a checagem virava um falso negativo. O certo é: todos com a MESMA raça e ao
        //    menos um DIFERENTE da impressa — aí a troca é global, e quem já era daquela raça não atrapalha.
        //
        // A raça impressa vem do NamedCard, não do nosso registro de deck, porque agora as cartas dele também
        // entram na conta.
        // Nomes do script Lua para os valores do enum. RACE_* e ATTRIBUTE_* são constantes do próprio jogo, não
        // texto de carta, então isto é leitura e não adivinhação.
        private static readonly string[] ScriptRaceNames =
        {
            "RACE_WARRIOR", "RACE_SPELLCASTER", "RACE_FAIRY", "RACE_FIEND", "RACE_ZOMBIE", "RACE_MACHINE",
            "RACE_AQUA", "RACE_PYRO", "RACE_ROCK", "RACE_WINDBEAST", "RACE_PLANT", "RACE_INSECT",
            "RACE_THUNDER", "RACE_DRAGON", "RACE_BEAST", "RACE_BEASTWARRIOR", "RACE_DINOSAUR", "RACE_FISH",
            "RACE_SEASERPENT", "RACE_REPTILE", "RACE_PSYCHO", "RACE_DIVINE", "RACE_CREATORGOD", "RACE_WYRM",
            "RACE_CYBERSE", "RACE_ILLUSION"
        };
        private static readonly CardRace[] ScriptRaceValues =
        {
            CardRace.Warrior, CardRace.SpellCaster, CardRace.Fairy, CardRace.Fiend, CardRace.Zombie,
            CardRace.Machine, CardRace.Aqua, CardRace.Pyro, CardRace.Rock, CardRace.WindBeast, CardRace.Plant,
            CardRace.Insect, CardRace.Thunder, CardRace.Dragon, CardRace.Beast, CardRace.BeastWarrior,
            CardRace.Dinosaur, CardRace.Fish, CardRace.SeaSerpent, CardRace.Reptile, CardRace.Psycho,
            CardRace.DivineBeast, CardRace.CreatorGod, CardRace.Wyrm, CardRace.Cyberse, CardRace.Illusion
        };
        private static readonly string[] ScriptAttributeNames =
        {
            "ATTRIBUTE_EARTH", "ATTRIBUTE_WATER", "ATTRIBUTE_FIRE", "ATTRIBUTE_WIND",
            "ATTRIBUTE_LIGHT", "ATTRIBUTE_DARK", "ATTRIBUTE_DIVINE"
        };
        private static readonly CardAttribute[] ScriptAttributeValues =
        {
            CardAttribute.Earth, CardAttribute.Water, CardAttribute.Fire, CardAttribute.Wind,
            CardAttribute.Light, CardAttribute.Dark, CardAttribute.Divine
        };

        // Lê a troca DIRETO da carta que a impõe, em vez de deduzir do campo. Fecha o buraco que sobrou: com o
        // campo dos dois lados vazio não há o que amostrar, e mesmo assim a trava já está valendo. O Black Rose
        // Garden traz o valor explícito no script — "e2:SetCode(EFFECT_CHANGE_RACE)" seguido de
        // "e2:SetValue(RACE_PLANT)" — então é consulta, não palpite.
        private static int ForcedFromCardScript(int cardId, string effectCode, string[] names, int[] values)
        {
            string script = RdaScripts.Read(cardId, 0);
            if (string.IsNullOrEmpty(script) || script.IndexOf(effectCode, StringComparison.Ordinal) < 0)
                return 0;
            // Só vale a troca que alcança O CAMPO INTEIRO. O Predaplant Verte Anaconda também usa
            // EFFECT_CHANGE_ATTRIBUTE, mas em UM monstro alvo (EFFECT_TYPE_SINGLE); tratar isso como troca global
            // seria pior que o defeito original, porque quebraria receitas de Synchro sem motivo.
            // O que separa os dois é a forma do efeito, na MESMA variável:
            //   Black Rose Garden  e2:SetType(EFFECT_TYPE_FIELD) + e2:SetTargetRange(LOCATION_MZONE,LOCATION_MZONE)
            //   Verte Anaconda     e1:SetType(EFFECT_TYPE_SINGLE)
            foreach (var match in System.Text.RegularExpressions.Regex.Matches(script,
                @"(?m)^\s*(\w+):SetCode\(" + effectCode + @"\)").Cast<System.Text.RegularExpressions.Match>())
            {
                string variable = match.Groups[1].Value;
                string byVariable = @"(?m)^\s*" + System.Text.RegularExpressions.Regex.Escape(variable) + ":Set";
                bool fieldWide = System.Text.RegularExpressions.Regex.IsMatch(script,
                        byVariable + @"Type\([^)]*EFFECT_TYPE_FIELD")
                    && System.Text.RegularExpressions.Regex.IsMatch(script,
                        byVariable + @"TargetRange\(LOCATION_MZONE\s*,\s*LOCATION_MZONE\)");
                if (!fieldWide)
                    continue;
                for (int i = 0; i < names.Length; ++i)
                    if (System.Text.RegularExpressions.Regex.IsMatch(script,
                            byVariable + @"Value\(" + names[i] + @"\)"))
                        return values[i];
            }
            return 0;
        }

        // A trava pode vir de qualquer carta virada para cima, dos dois lados.
        private int ForcedOnFieldFromScripts(string effectCode, string[] names, int[] values)
        {
            foreach (ClientCard card in Bot.GetMonsters().Concat(Bot.GetSpells())
                .Concat(Enemy.GetMonsters()).Concat(Enemy.GetSpells()))
            {
                if (card == null || !card.IsFaceup())
                    continue;
                int id = CardCode(card);
                if (id == 0)
                    continue;
                int forced = ForcedFromCardScript(id, effectCode, names, values);
                if (forced != 0)
                    return forced;
            }
            return 0;
        }

        private CardRace FieldForcedRace()
        {
            int fromScript = ForcedOnFieldFromScripts("EFFECT_CHANGE_RACE", ScriptRaceNames,
                ScriptRaceValues.Select(value => (int)value).ToArray());
            if (fromScript != 0)
                return (CardRace)fromScript;
            var faceUp = Bot.GetMonsters().Concat(Enemy.GetMonsters())
                .Where(card => card != null && card.IsFaceup() && CardCode(card) != 0).ToList();
            if (faceUp.Count == 0)
                return 0;
            int race = faceUp[0].Race;
            bool anyChanged = false;
            foreach (ClientCard card in faceUp)
            {
                if (card.Race != race)
                    return 0;   // não é uniforme: não há troca global
                var data = YGOSharp.OCGWrapper.NamedCard.Get(CardCode(card));
                if (data != null && data.Race != race)
                    anyChanged = true;
            }
            return anyChanged ? (CardRace)race : 0;
        }

        private CardAttribute FieldForcedAttribute()
        {
            int fromScript = ForcedOnFieldFromScripts("EFFECT_CHANGE_ATTRIBUTE", ScriptAttributeNames,
                ScriptAttributeValues.Select(value => (int)value).ToArray());
            if (fromScript != 0)
                return (CardAttribute)fromScript;
            var faceUp = Bot.GetMonsters().Concat(Enemy.GetMonsters())
                .Where(card => card != null && card.IsFaceup() && CardCode(card) != 0).ToList();
            if (faceUp.Count == 0)
                return 0;
            int attribute = faceUp[0].Attribute;
            bool anyChanged = false;
            foreach (ClientCard card in faceUp)
            {
                if (card.Attribute != attribute)
                    return 0;
                var data = YGOSharp.OCGWrapper.NamedCard.Get(CardCode(card));
                if (data != null && data.Attribute != attribute)
                    anyChanged = true;
            }
            return anyChanged ? (CardAttribute)attribute : 0;
        }

        private List<ClientCard> KnownEnemyHand()
        {
            return Enemy.Hand.Where(card => card != null && card.Id != 0).ToList();
        }

        // -----------------------------------------------------------------------------------------------------
        // Dossiê do oponente — SÓ OBSERVAÇÃO. Monta o que dá para saber e escreve no log; não muda decisão nenhuma.
        //
        // O texto do jogo segue uma convenção rígida: toda referência a outra carta ou a um arquétipo vem entre
        // aspas. Então não é preciso interpretar texto livre — basta pegar o que está entre aspas e consultar o
        // índice de nomes. Medição no cards.cdb (2026-09-22): 22.868 trechos entre aspas, 11.976 (52,4%) são nome
        // exato de carta e o resto são nomes de arquétipo. Nenhum dos dois é palpite, é consulta.
        //
        // Limite conhecido e aceito (jogador): o conjunto é "o que existe impresso citando esta carta", não "o que
        // ele tem no deck". O bot superestima as opções dele — e deve mesmo, porque ele pode ter outra carta para
        // chegar no mesmo lugar. Por isso listamos TODOS os bosses alcançáveis, não um só.
        // -----------------------------------------------------------------------------------------------------
        private static Dictionary<string, List<int>> _referenceIndex;   // nome citado -> ids que o citam
        private static HashSet<string> _cardNameSet;                    // nomes exatos de carta
        private static readonly Dictionary<string, int> _cardIdByName = new Dictionary<string, int>(StringComparer.Ordinal);
        private static readonly Dictionary<long, List<int>> _setcodeIndex = new Dictionary<long, List<int>>();

        // Os setcodes vêm empacotados em quatro campos de 16 bits. Os 12 bits baixos são o arquétipo base e os
        // 4 altos o subtipo — "D/D/D" é subtipo de "D/D", então a base liga os dois, que é o que queremos.
        private static IEnumerable<long> BaseSetcodes(long packed)
        {
            while (packed > 0)
            {
                long one = packed & 0xffff;
                packed >>= 16;
                if (one != 0)
                    yield return one & 0xfff;
            }
        }
        private static bool _referenceIndexReady;
        private static readonly object _referenceIndexLock = new object();
        private string _opponentProfileSignature;

        private const int TypeFusion = 0x40, TypeSynchro = 0x2000, TypeXyz = 0x800000, TypeLink = 0x4000000;
        private const int SpellType = 0x2, TrapType = 0x4;
        // Tipos que FICAM no campo depois de resolver: Contínua, Campo e Equipamento.
        private const int StaysOnFieldTypes = 0x10000 | 0x20000 | 0x40000;
        private const int ExtraDeckTypes = TypeFusion | TypeSynchro | TypeXyz | TypeLink;

        // Trechos entre aspas. Varredura simples de caracteres: a convenção do texto torna isso suficiente e
        // evita depender de expressão regular, que é imprecisa para este trabalho.
        private static List<string> QuotedTokens(string text)
        {
            var found = new List<string>();
            if (string.IsNullOrEmpty(text))
                return found;
            int open = -1;
            for (int i = 0; i < text.Length; ++i)
            {
                if (text[i] != '"')
                    continue;
                if (open < 0) { open = i + 1; continue; }
                int length = i - open;
                if (length >= 2 && length <= 60)
                    found.Add(text.Substring(open, length));
                open = -1;
            }
            return found;
        }

        // Índice reverso montado uma vez por processo. O banco inteiro já está em memória no NamedCardsManager;
        // chegamos nele pelo assembly do NamedCard, sem nome de assembly em texto (que já falhou antes aqui).
        private void BuildReferenceIndex()
        {
            lock (_referenceIndexLock)
            {
                if (_referenceIndexReady)
                    return;
                _referenceIndexReady = true;
                _referenceIndex = new Dictionary<string, List<int>>(StringComparer.Ordinal);
                _cardNameSet = new HashSet<string>(StringComparer.Ordinal);
                try
                {
                    System.Type manager = typeof(YGOSharp.OCGWrapper.NamedCard).Assembly
                        .GetType("YGOSharp.OCGWrapper.NamedCardsManager");
                    if (manager == null)
                        return;
                    var field = manager.GetField("_cards", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                    var all = field == null ? null : field.GetValue(null) as System.Collections.IEnumerable;
                    if (all == null)
                        return;
                    var cards = new List<YGOSharp.OCGWrapper.NamedCard>();
                    foreach (object entry in all)
                    {
                        var pair = entry.GetType().GetProperty("Value");
                        var card = (pair == null ? entry : pair.GetValue(entry, null)) as YGOSharp.OCGWrapper.NamedCard;
                        if (card != null && !string.IsNullOrEmpty(card.Name))
                            cards.Add(card);
                    }
                    foreach (var card in cards)
                    {
                        _cardNameSet.Add(card.Name);
                        _cardIdByName[card.Name] = card.Id;
                        // Índice por setcode: a coluna estruturada do cards.cdb, não texto. É o que liga um deck de
                        // arquétipo ao boss dele quando a linha de materiais diz 2+ "D/D" monsters — um rótulo de
                        // arquétipo, que o índice por NOME descarta de propósito. Medido em 2026-09-23: nos três
                        // logs o índice por nome previu 1 boss e o setcode previu a família inteira.
                        if ((card.Type & ExtraDeckTypes) == 0)
                            continue;
                        foreach (long baseSet in BaseSetcodes(card.Setcode))
                        {
                            List<int> family;
                            if (!_setcodeIndex.TryGetValue(baseSet, out family))
                                _setcodeIndex[baseSet] = family = new List<int>();
                            family.Add(card.Id);
                        }
                    }
                    foreach (var card in cards)
                    {
                        foreach (string token in QuotedTokens(card.Description))
                        {
                            List<int> list;
                            if (!_referenceIndex.TryGetValue(token, out list))
                                _referenceIndex[token] = list = new List<int>();
                            if (!list.Contains(card.Id))
                                list.Add(card.Id);
                        }
                    }
                    Report("opponent", string.Format("reference index built: {0} card names, {1} quoted references",
                        _cardNameSet.Count, _referenceIndex.Count));
                }
                catch (Exception error)
                {
                    Report("opponent", "reference index unavailable: " + error.Message);
                }
            }
        }

        // Tudo que já vimos das cartas dele: campo, magias/armadilhas viradas para cima, GY, banidas, e o que foi
        // revelado da mão e do Extra (o id fica gravado depois da revelação).
        private List<ClientCard> SeenOpponentCards()
        {
            var seen = new List<ClientCard>();
            seen.AddRange(Enemy.GetMonsters().Where(card => card != null && card.IsFaceup()));
            seen.AddRange(Enemy.GetSpells().Where(card => card != null && card.IsFaceup()));
            seen.AddRange(Enemy.Graveyard.Where(card => card != null));
            seen.AddRange(Enemy.Banished.Where(card => card != null));
            seen.AddRange(Enemy.Hand.Where(card => card != null && card.Id != 0));
            seen.AddRange(Enemy.ExtraDeck.Where(card => card != null && card.Id != 0));
            return seen.Where(card => card.Id != 0).ToList();
        }

        // Um boss alcançável: monstro de Extra que cita uma carta que já vimos na mesa dele.
        private sealed class OpponentBoss
        {
            public int Id;
            public string Name;
            public int Attack;
            public string Materials;      // primeira linha do texto: a linha de materiais
            public List<string> Needed;   // nomes citados na linha de materiais
            public List<string> Have;     // desses, os que já vimos
            public List<string> Support;  // cartas JÁ VISTAS que levam a este boss (sem contar ele mesmo)
            // ATENÇÃO à diferença, que já custou uma partida. "Revealed" é "esta carta existe e nós a vimos" —
            // e o SeenOpponentCards inclui o EXTRA DECK revelado e o GY, então um Mirrorjade revelado no Extra
            // cai aqui sem nunca ter tocado o campo. "OnField" é o que de fato já está posto. Só o segundo
            // significa "a hora de impedir passou". No duelo 012308 de 2026-09-23 o Mirrorjade estava entre as 8
            // cartas vistas quando o bot gastou o Harmonia, e a reserva o descartou como se já estivesse na mesa.
            public bool Revealed;         // vimos a carta em algum lugar (campo, GY, Extra revelado, mão revelada)
            public bool OnField;          // está no campo dele agora: não há mais o que impedir
            public bool FromArchetype;    // veio da família por setcode, não de um nome citado: fonte menos precisa
            public int Difficulty;        // quanto custa lidar com ele depois que ele estiver pronto
        }

        // -----------------------------------------------------------------------------------------------------
        // CAMADA 1 — quanto custa lidar com o boss DEPOIS de pronto.
        //
        // Não é nota por carta e não conhece arquétipo nenhum: lê ATK, o perfil de efeito que já extraímos do
        // script, e os traços de proteção. O Baronne de Fleur e o Bystial Dis Pater pontuam alto sozinhos porque o
        // perfil deles diz "destroy + negate | quick" e "to deck + disable | quick" — não porque alguém escreveu o
        // nome deles aqui. É o mesmo cálculo para um boss de Rose, de D/D/D ou de Branded.
        //
        // A escala é a mesma faixa de ThreatHigh/ThreatLock/... para as duas notas poderem ser comparadas depois.
        // -----------------------------------------------------------------------------------------------------
        private const int OurBattleAttack = 3000;     // ATK do Red Dragon Archfiend: o que a nossa mesa derruba em batalha
        private const int BossAttackFarAbove = 40;    // ATK acima do que qualquer atacante nosso alcança
        private const int BossAttackAbove = 25;       // ATK acima do RDA
        private const int BossAttackTrades = 10;      // ATK que troca com a nossa mesa
        private const int BossRemoves = 25;           // destrói ou bane as nossas cartas
        private const int BossNegates = 30;           // nega os nossos efeitos
        private const int BossLocks = 30;             // trava alguma coisa nossa
        private const int BossExtends = 10;           // só adianta a mesa dele
        private const int BossQuick = 15;             // efeito rápido: alcança o NOSSO turno
        private const int BossPunishesRemoval = 20;   // dispara ao sair do campo: tirar a carta cobra um preço
        private const int BossProtected = 30;         // protegido: as nossas saídas contra ele são poucas
        private static readonly Dictionary<int, int> BossDifficultyCache = new Dictionary<int, int>();

        private static int BossDifficulty(int id, int attack)
        {
            lock (BossDifficultyCache)
            {
                int cached;
                if (BossDifficultyCache.TryGetValue(id, out cached))
                    return cached;

                int score = 0;
                // Empate de ATK conta como "acima": o nosso melhor atacante é o RDA com 3000, e um boss de 3000 não é
                // atropelado em batalha. Com "> OurBattleAttack" o Mirrorjade (3000) caía na faixa de troca e valia 10.
                if (attack >= NovaOriginalAttack) score += BossAttackFarAbove;
                else if (attack >= OurBattleAttack) score += BossAttackAbove;
                else if (attack >= 2500) score += BossAttackTrades;

                bool anyQuick = false;
                foreach (EffectProfile profile in EffectProfilesOf(id))
                {
                    foreach (string does in profile.Does)
                    {
                        if (does == "destroy" || does == "banish" || does == "to deck") score += BossRemoves;
                        else if (does == "disable" || does == "negate") score += BossNegates;
                        else if (does.StartsWith("locks") || does.StartsWith("cannot")) score += BossLocks;
                        else if (does == "special summon") score += BossExtends;
                    }
                    // "Ser rápido" é uma propriedade da carta, não de cada efeito. Somando por perfil, o Rindbrumm
                    // (dois efeitos rápidos) ganhava +30 e passava na frente do Mirrorjade, que é a carta que de fato
                    // matou o nosso plano no duelo contra Branded de 2026-09-23 (log 010139).
                    if (profile.When == "quick") anyQuick = true;
                    // Punição por sair do campo: o efeito dispara QUANDO a carta deixa o campo, então removê-la cobra
                    // um preço. O Mirrorjade destrói a nossa mesa ao sair. Isso torna a carta mais difícil, não menos,
                    // e o cálculo antigo tratava como uma destruição qualquer.
                    if (profile.Event == "EVENT_LEAVE_FIELD" && profile.Does.Count > 0)
                        score += BossPunishesRemoval;
                }
                if (anyQuick) score += BossQuick;

                // Proteção: depois que ele está na mesa só saímos pela linha RDA + Crimson Gaia ou banindo com
                // Hypernova/Supernova (regra do jogador, 2026-09-22). Impedir que ele chegue vale muito mais.
                if ((TraitsOfId(id) & (BattleTrait.EffectIndestructible | BattleTrait.ImmuneEffect | BattleTrait.BattleIndestructible)) != 0)
                    score += BossProtected;

                BossDifficultyCache[id] = score;
                return score;
            }
        }

        private List<OpponentBoss> ReachableOpponentBosses(List<string> seenNames)
        {
            return ReachableOpponentBosses(seenNames, OpponentFieldNames());
        }

        // Nomes do que está virado para cima no campo dele AGORA. Offline (testes) não há campo, e a sobrecarga
        // de um argumento só passa um conjunto vazio — o que é o certo ali: nada está posto.
        private HashSet<string> OpponentFieldNames()
        {
            var names = new HashSet<string>(StringComparer.Ordinal);
            foreach (ClientCard card in Enemy.GetMonsters().Concat(Enemy.GetSpells()))
                if (card != null && card.IsFaceup() && !string.IsNullOrEmpty(card.Name))
                    names.Add(card.Name);
            return names;
        }

        private List<OpponentBoss> ReachableOpponentBosses(List<string> seenNames, HashSet<string> onFieldNames)
        {
            var bosses = new List<OpponentBoss>();
            if (_referenceIndex == null)
                return bosses;
            // CAMADA 2 — "apoio": quantas cartas DIFERENTES que já vimos levam a este mesmo boss. Muitas = cortar
            // uma não impede nada, guarde a resposta. Uma só = aquela carta é o gargalo, e vale gastar ali.
            // Sai de contagem no índice reverso, sem ler o sentido de texto nenhum.
            var byName = new Dictionary<string, OpponentBoss>(StringComparer.Ordinal);
            foreach (string name in seenNames)
            {
                List<int> referencing;
                if (!_referenceIndex.TryGetValue(name, out referencing))
                    continue;
                foreach (int id in referencing)
                {
                    var data = YGOSharp.OCGWrapper.NamedCard.Get(id);
                    if (data == null || (data.Type & ExtraDeckTypes) == 0)
                        continue;
                    OpponentBoss boss;
                    if (!byName.TryGetValue(data.Name, out boss))
                    {
                        string first = (data.Description ?? "").Split('\n')[0].Trim();
                        var needed = QuotedTokens(first).Where(token => _cardNameSet.Contains(token)).ToList();
                        byName[data.Name] = boss = new OpponentBoss
                        {
                            Id = id,
                            Name = data.Name,
                            Attack = data.Attack,
                            Materials = first,
                            Needed = needed,
                            Have = needed.Where(seenNames.Contains).ToList(),
                            Support = new List<string>(),
                            OnField = onFieldNames != null && onFieldNames.Contains(data.Name),
                            Difficulty = BossDifficulty(id, data.Attack)
                        };
                    }
                    // A carta cita o PRÓPRIO nome na cláusula "You can only use this effect of X once per turn".
                    // Sem esta linha, todo boss que apareceu no campo dele vira "alcançável por si mesmo" e o
                    // apoio dá 1 para tudo — foi o que os três logs mostraram na primeira medição.
                    if (string.Equals(name, data.Name, StringComparison.Ordinal))
                    {
                        boss.Revealed = true;
                        continue;
                    }
                    if (!boss.Support.Contains(name))
                        boss.Support.Add(name);
                }
            }
            // Segunda fonte: a família do arquétipo, pelo setcode. Menos precisa que o nome citado — traz primos
            // que ele pode nem ter — e por isso fica marcada como tal. Vale porque para deck de arquétipo o índice
            // por nome não prevê nada: o material é um rótulo ("D/D"), não um nome de carta.
            foreach (string name in seenNames)
            {
                int seenId;
                if (!_cardIdByName.TryGetValue(name, out seenId))
                    continue;
                var seenData = YGOSharp.OCGWrapper.NamedCard.Get(seenId);
                if (seenData == null)
                    continue;
                foreach (long baseSet in BaseSetcodes(seenData.Setcode))
                {
                    List<int> family;
                    if (!_setcodeIndex.TryGetValue(baseSet, out family))
                        continue;
                    foreach (int id in family)
                    {
                        var data = YGOSharp.OCGWrapper.NamedCard.Get(id);
                        if (data == null)
                            continue;
                        OpponentBoss boss;
                        if (!byName.TryGetValue(data.Name, out boss))
                        {
                            string first = (data.Description ?? "").Split('\n')[0].Trim();
                            var needed = QuotedTokens(first).Where(token => _cardNameSet.Contains(token)).ToList();
                            byName[data.Name] = boss = new OpponentBoss
                            {
                                Id = id,
                                Name = data.Name,
                                Attack = data.Attack,
                                Materials = first,
                                Needed = needed,
                                Have = needed.Where(seenNames.Contains).ToList(),
                                Support = new List<string>(),
                                OnField = onFieldNames != null && onFieldNames.Contains(data.Name),
                                FromArchetype = true,
                                Difficulty = BossDifficulty(id, data.Attack)
                            };
                        }
                        if (string.Equals(name, data.Name, StringComparison.Ordinal))
                        {
                            boss.Revealed = true;
                            continue;
                        }
                        if (!boss.Support.Contains(name))
                            boss.Support.Add(name);
                    }
                }
            }
            // Filtro de precisão sobre a fonte larga. O setcode liga famílias que se cruzam de verdade nos dados:
            // o D/D Savant Kepler carrega 0x6E junto com o Red-Eyes Dark Dragoon (medido em 2026-09-23), e sem
            // isto o Dragoon aparecia como a maior ameaça de um deck D/D/D. Mas o Dragoon pede materiais com NOME
            // ("Dark Magician" + 1 Red-Eyes) e não vimos nenhum deles. Regra: boss cuja linha de materiais cita
            // nomes e nenhum deles apareceu não está ao alcance. Bosses de arquétipo não citam nome nenhum, então
            // não são afetados — é exatamente onde queremos manter o alcance largo.
            // Segundo filtro sobre a fonte larga: o arquétipo que ele está jogando é aquele a que a MAIORIA das cartas
            // vistas pertence. No duelo contra D/D/D em 2026-09-23 (log 003531) o setcode 0xAF cobria 12 das 16 cartas
            // vistas e o 0x6E cobria 2 — e era o 0x6E que trazia o Gilti-Gearfried, o único "gargalo" que a projeção
            // nomeou naquela partida, apontando o bot para a carta errada. Um boss sustentado por uma fração pequena
            // do que vimos é evidência fraca. Continua sendo contagem: nenhum nome de carta entra nesta decisão.
            int seenTotal = seenNames.Count;
            bosses.AddRange(byName.Values.Where(boss =>
                (boss.Revealed || boss.Needed.Count == 0 || boss.Have.Count > 0)
                && (!boss.FromArchetype || boss.Revealed || seenTotal == 0
                    || boss.Support.Count * ArchetypeShareDivisor >= seenTotal)));
            // Ordem: o que mais custa primeiro. O ATK desempata.
            return bosses.OrderByDescending(boss => boss.Difficulty).ThenByDescending(boss => boss.Attack).ToList();
        }

        // Escreve o dossiê no log quando o que sabemos muda. Nenhuma decisão usa isto ainda.
        // =====================================================================================================
        // MEMÓRIA ENTRE PARTIDAS
        //
        // O processo morre no fim de cada duelo, então o que ele aprende tem que ir para arquivo. Ele mora em
        // WindBot_knowledge\, pasta propria ao lado do executavel (antes vivia escondido dentro de Dialogs
        // com nome de arquivo de traducao), e o nome vem do PRIMEIRO nome do [Deck(...)] mais "_knowledge":
        // TheCrimsonKing -> TheCrimsonKing_knowledge.json. O formato é uma
        // linha JSON por registro, o que dá três coisas de que precisamos: dá para APENDAR sem reescrever nada
        // (não se perde dado se o processo morrer no meio), dá para ler sem biblioteca nenhuma (o esquema é fixo
        // e nosso, então os campos saem por busca de texto — o JavaScriptSerializer existe mas gastou 137 ms num
        // arquivo de 212 KB e exigiria referência nova no .csproj), e o arquivo SATURA em vez de crescer sem fim.
        //
        // Tamanho medido em 2026-09-23, simulando 400 duelos com pools de carta realistas:
        //     duelos    linhas   pares distintos   compactado
        //         50     2.194             1.061      57,0 KB
        //        100     4.394             1.224      65,7 KB
        //        400    17.571             1.248      67,0 KB
        // Satura porque o pool de cartas de um arquétipo é finito: passa a só incrementar contador. Ler e agregar
        // um arquivo de 488 KB sem compactar levou 38 ms, contra os ~3 s que o planejador gasta por turno.
        //
        // O QUE se registra (jogador, 2026-09-23): não é "qual carta leva ao boss". Muito deck não termina em
        // monstro de Extra — no Labrynth o problema são as armadilhas, e o Droll e o Maxx "C" derrubam o combo da
        // mão. Então o consequente é O QUE DEU ERRADO PARA NÓS, que o executor já sabe calcular: o plano desabou,
        // ou um efeito nosso foi negado. Assim a mesma conta cobre armadilha, trava, handtrap e boss.
        //
        // E não se procura UM culpado: guarda-se a nota de todas as cartas, porque ele costuma ter mais de uma
        // rota para o mesmo lugar. Quantas cartas passam da nota é a própria resposta para "dá para impedir?".
        // =====================================================================================================
        // ---------------------------------------------------------------------------------------------------
        // LIGA/DESLIGA. Duas chaves independentes, porque um dia isto pode atrapalhar e tem que sair do caminho
        // sem recompilar nada:
        //   1. Arquivos marcadores, ao lado do proprio arquivo de memoria. Criar ou apagar e o liga/desliga
        //      do dia a dia, sem recompilar:
        //        WindBot_knowledge\TheCrimsonKing_knowledge.json.off        memoria INTEIRA desligada: nao le e nao escreve
        //        WindBot_knowledge\TheCrimsonKing_knowledge.json.nodecide   continua COLETANDO, mas nenhuma regra consulta
        //        WindBot_knowledge\TheCrimsonKing_knowledge.json.decide     forca a consulta, mesmo com a constante em false
        //      O ".off" vence os outros dois. O ".decide" existe para o caso inverso do ".nodecide": sem ele,
        //      uma constante compilada em false nao teria como ser religada de fora.
        //   2. As duas constantes abaixo, que sao apenas o PADRAO quando nao ha arquivo marcador.
        // Com a memoria desligada o bot decide como antes dela: perfil de efeito, projecao e reserva continuam
        // funcionando, porque as tres regras que a consultam caem por si (lift -1 nunca passa da barra). Nao ha
        // caminho em que desligar quebre alguma coisa; o pior caso e perder o ganho.
        private const bool KnowledgeCompiledOn = true;
        // COLETAR e DECIDIR sao coisas separadas. Medido em 2026-09-23, nos 22 duelos depois de ligar a
        // decisao, a memoria influenciou exatamente 1 — e nesse um ela errou (segurou contra a Branded Fusion).
        // Coletar custa 65 KB e milissegundos; decidir sem evidencia nao se paga. Este e so o padrao: o
        // arquivo ".nodecide" desliga e o ".decide" liga, sem mexer no codigo.
        private const bool KnowledgeDecidesOn = true;

        private const string KnowledgeOffMarker = ".off";
        private const string KnowledgeNoDecideMarker = ".nodecide";
        private const string KnowledgeDecideMarker = ".decide";
        private static bool? _knowledgeEnabled;
        private static bool? _knowledgeDecides;

        private static bool Marcador(string sufixo)
        {
            try { return System.IO.File.Exists(KnowledgePath() + sufixo); }
            catch { return false; }
        }

        private bool KnowledgeEnabled
        {
            get
            {
                if (_knowledgeEnabled.HasValue)
                    return _knowledgeEnabled.Value;
                bool on = KnowledgeCompiledOn && !Marcador(KnowledgeOffMarker);
                _knowledgeEnabled = on;
                if (!on)
                    Report("opponent", "memory: disabled, deciding without it");
                return on;
            }
        }

        /// <summary>A memoria pode CONSULTAR? Exige estar ligada; os marcadores vencem a constante.</summary>
        private bool KnowledgeDecides
        {
            get
            {
                if (_knowledgeDecides.HasValue)
                    return _knowledgeDecides.Value;
                bool on;
                if (!KnowledgeEnabled) on = false;
                else if (Marcador(KnowledgeNoDecideMarker)) on = false;
                else if (Marcador(KnowledgeDecideMarker)) on = true;
                else on = KnowledgeDecidesOn;
                _knowledgeDecides = on;
                Report("opponent", on ? "memory: collecting and deciding"
                                      : "memory: collecting only, no rule consults it");
                return on;
            }
        }

        // Pasta propria ao lado do executavel; a memoria nao se esconde mais em Dialogs. O nome do arquivo e o
        // PRIMEIRO nome do [Deck(...)] (o mesmo que o Deck=... aceita) mais o sufixo "_knowledge" — se o deck
        // for renomeado, o arquivo acompanha. Os marcadores ".off"/".nodecide"/".decide" ficam ao lado, com o
        // mesmo nome acrescido do sufixo.
        private const string KnowledgeFolder = "WindBot_knowledge";
        private static readonly string KnowledgeName = KnowledgeDeckName() + "_knowledge";

        private static string KnowledgeDeckName()
        {
            object[] attributes = typeof(TheCrimsonKingExecutor).GetCustomAttributes(typeof(DeckAttribute), false);
            if (attributes.Length > 0 && !string.IsNullOrEmpty(((DeckAttribute)attributes[0]).Name))
                return ((DeckAttribute)attributes[0]).Name;
            return "TheCrimsonKing";
        }
        // Resultado ruim para nós. Fica curto porque vai em toda linha do arquivo.
        private const string OutcomeSeen = "-";        // denominador: a carta resolveu neste duelo
        // Numerador: UM rótulo só para "deu ruim para nós", seja plano desabado ou efeito nosso negado. Dois
        // rótulos separados somavam no mesmo duelo quando as duas coisas aconteciam, e a taxa passava de 100%
        // (visto na semeadura de 2026-09-23: 233%). A medida é "em que fração dos duelos esta carta apareceu
        // antes do estrago", então é uma marca por duelo, não uma por tipo de estrago.
        private const string OutcomeBad = "bad";
        // Segundo consequente (jogador, 2026-09-23): não esperar o estrago. Quando uma carta que JÁ sabemos que
        // nos quebra entra em jogo, as jogadas que vieram logo antes dela também são ponto de interceptação — e
        // mais cedo. Mesma máquina, outro alvo: em vez de "o que veio antes do estrago", "o que veio antes da
        // carta que causa o estrago". Só começa a render quando "bad" já tem amostra, porque é ele que define
        // quais cartas contam como destacadas.
        private const string OutcomePre = "pre";
        // Chave GLOBAL, além da do arquétipo. Engine é compartilhada — Bystial aparece em Branded, em Rose Dragon
        // e no nosso próprio deck (jogador, 2026-09-23) — e uma chave por deck fatia a amostra da mesma carta em
        // vários baldes. Medido nos 240 duelos: com chave global o Ash Blossom tem 66 duelos de amostra e lift
        // 4,4, contra 24 duelos quando dividido por deck. Cada registro sai nas duas chaves; a consulta usa a do
        // arquétipo quando ela tem amostra própria e cai na global quando não tem.
        private const string KnowledgeGlobalKey = "*";
        private const int KnowledgeArchetypeSample = 5;
        // Terceiro nível: a CLASSE do efeito, não a carta. Deck tem variantes e cartas diferentes que chegam ao
        // mesmo lugar (jogador, 2026-09-23), então carta nova nunca teria amostra. A classe resolve isso porque
        // ela já existe antes de a carta aparecer. Medido nos 240 duelos, com baseline 7%:
        //     lift 6,1  disable+to hand |quick      (a classe do Brightest Blazing Branded King)
        //     lift 4,4  disable+draw |quick         (a classe do Ash Blossom)
        //     lift 2,8  banish+destroy |quick
        //     lift 2,2  destroy+negate |quick
        // Uma carta inédita que seja "disable | quick" já entra valendo, sem nunca ter sido vista.
        private const string KnowledgeClassPrefix = "#";
        private const int KnowledgeClassSample = 15;

        private readonly List<int> _opponentResolvedThisDuel = new List<int>();
        private string _knowledgeArchetype;
        private readonly List<string> _knowledgePending = new List<string>();

        // Assinatura de efeito da carta: os dois primeiros rótulos em ordem, mais a marca de rápido. Tem que ser
        // calculada igual na gravação e na leitura, por isso fica aqui e não espalhada.
        private static readonly Dictionary<int, string> KnowledgeClassCache = new Dictionary<int, string>();

        private static string KnowledgeClassOf(int cardId)
        {
            lock (KnowledgeClassCache)
            {
                string cached;
                if (KnowledgeClassCache.TryGetValue(cardId, out cached))
                    return cached;
                var labels = new List<string>();
                bool quick = false;
                foreach (EffectProfile profile in EffectProfilesOf(cardId))
                {
                    foreach (string does in profile.Does)
                        if (!labels.Contains(does))
                            labels.Add(does);
                    if (profile.When == "quick")
                        quick = true;
                }
                labels.Sort(StringComparer.Ordinal);
                string result = labels.Count == 0 ? null
                    : KnowledgeClassPrefix + string.Join("+", labels.Take(2)) + (quick ? " |quick" : "");
                KnowledgeClassCache[cardId] = result;
                return result;
            }
        }

        private static string KnowledgePath()
        {
            string dir = System.IO.Path.Combine(
                System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? ".",
                KnowledgeFolder);
            return System.IO.Path.Combine(dir, KnowledgeName + ".json");
        }

        // Teto da fila de pendentes. Enquanto a escrita falha, os lotes são RETIDOS para o próximo flush (nada
        // se perde mais em silêncio), mas uma falha prolongada cresceria sem limite — passado o teto, descartam-
        // se os registros mais antigos (de duelos mais velhos) com um único aviso por duelo.
        private const int KnowledgePendingMax = 4096;

        // Coordenação de escrita. O MESMO arquivo é apendado por várias instâncias no mesmo processo (modo server,
        // um executor por thread) e por vários processos na mesma pasta. Mutex nomeado, derivado de um hash
        // estável do path — Mutex não aceita '\' no nome, e string.GetHashCode varia entre runtimes. Um segurador
        // travado não pode congelar os outros: timeout, lote retido, retry no próximo flush. AbandonedMutexException
        // (detentor morreu no meio) significa que o mutex foi liberado pelo sistema e tomamos na chamada.
        //
        // Os dois mutáveis (e não const) são como SearchOptions no template: existem para os testes ajustarem por
        // reflexão, sem trocar o comportamento em produção.
        private static int KnowledgeLockTimeoutMs = 5000;
        private static long KnowledgeCompactBytes = 4L * 1024 * 1024;
        private static readonly string KnowledgeLockName = KnowledgeMutexNameFor(KnowledgePath());
        private static readonly System.Threading.Mutex KnowledgeMutex = CreateKnowledgeMutex();

        private static string KnowledgeMutexNameFor(string path)
        {
            // FNV-1a de 64 bits: determinístico entre processos e entre runtimes (.NET Framework e Mono).
            unchecked
            {
                ulong hash = 14695981039346656037UL;
                for (int i = 0; i < path.Length; ++i)
                {
                    hash ^= path[i];
                    hash *= 1099511628211UL;
                }
                return "WindBot_knowledge_" + hash.ToString("x16");
            }
        }

        private static System.Threading.Mutex CreateKnowledgeMutex()
        {
            try { return new System.Threading.Mutex(false, KnowledgeLockName); }
            catch { return null; }   // ambiente sem mutex nomeado: segue sem coordenação cross-processo
        }

        // true = segurou (ou não há mutex para segurar); false = outro escritor não soltou em KnowledgeLockTimeoutMs.
        private static bool KnowledgeLock()
        {
            if (KnowledgeMutex == null) return true;
            try { return KnowledgeMutex.WaitOne(KnowledgeLockTimeoutMs); }
            catch (System.Threading.AbandonedMutexException) { return true; }
            catch { return false; }
        }

        private static void KnowledgeUnlock()
        {
            if (KnowledgeMutex == null) return;
            try { KnowledgeMutex.ReleaseMutex(); }
            catch { }   // liberar demais é bug nosso, não motivo para derrubar o duelo
        }

        // Memória é melhoria, nunca requisito: falha de gravação não derruba o duelo. MAS a falha também não pode
        // DESCARTAR o lote — o catch vazio somado ao Clear() perdia os registros para sempre, e os flags de "uma
        // vez por duelo" já consumidos impediam que o lote fosse regenerado. Agora o lote fica pendente até um
        // flush bem-sucedido; o aviso é único por duelo para o log não virar ruído.
        private bool _knowledgeWriteNotice;

        private void KnowledgeWriteNotice(string message)
        {
            if (_knowledgeWriteNotice) return;
            _knowledgeWriteNotice = true;
            Report("opponent", message);
        }

        private void KnowledgeAppend(string outcome, int cardId)
        {
            if (string.IsNullOrEmpty(_knowledgeArchetype) || cardId == 0 || !KnowledgeEnabled)
                return;
            _knowledgePending.Add(string.Format(
                "{{\"a\":\"{0}\",\"o\":\"{1}\",\"c\":{2},\"n\":1}}",
                _knowledgeArchetype.Replace("\"", ""), outcome, cardId));
            _knowledgePending.Add(string.Format(
                "{{\"a\":\"{0}\",\"o\":\"{1}\",\"c\":{2},\"n\":1}}",
                KnowledgeGlobalKey, outcome, cardId));
            // A classe usa a própria assinatura como "carta": assim o contador de uma classe soma todas as cartas
            // dela, que é o ponto.
            string signature = KnowledgeClassOf(cardId);
            if (signature != null)
                _knowledgePending.Add(string.Format(
                    "{{\"a\":\"{0}\",\"o\":\"{1}\",\"c\":0,\"n\":1}}", signature, outcome));
            if (_knowledgePending.Count > KnowledgePendingMax)
            {
                _knowledgePending.RemoveRange(0, _knowledgePending.Count - KnowledgePendingMax);
                KnowledgeWriteNotice("pendentes no teto de " + KnowledgePendingMax
                    + " linha(s): descartados os registros mais antigos");
            }
            if (_knowledgePending.Count < KnowledgeFlushAt)
                return;
            KnowledgeFlush();
        }

        private const int KnowledgeFlushAt = 16;
        private const int KnowledgeBlameWindow = 2;

        // Deu ruim para nós: credita o resultado a TODAS as cartas que ele resolveu antes, neste duelo. Uma
        // partida só não distingue quem foi o culpado — e nem deveria, porque muitas vezes não há um culpado só.
        // Ao longo de dezenas de duelos a proporção separa quem realmente aparece antes do estrago de quem estava
        // junto por acaso.
        private readonly HashSet<string> _knowledgeOutcomesThisDuel = new HashSet<string>(StringComparer.Ordinal);

        // Denominador. Só sai quando o arquétipo já é conhecido — e quando ele é descoberto no meio do duelo,
        // esta função escreve de uma vez as cartas que já tinham resolvido antes disso. Sem isso a carta ganhava
        // "bad" sem nunca ter ganhado "seen", que é a outra metade da taxa acima de 100%.
        private readonly HashSet<int> _knowledgeSeenWritten = new HashSet<int>();

        private void KnowledgeWriteSeen()
        {
            if (string.IsNullOrEmpty(_knowledgeArchetype))
                return;
            foreach (int cardId in _opponentResolvedThisDuel)
                if (_knowledgeSeenWritten.Add(cardId))
                    KnowledgeAppend(OutcomeSeen, cardId);
        }

        // A carta que acabou de entrar é uma das que nos quebram? Então o que veio logo antes dela é o ponto
        // de corte mais cedo. Mesma janela e mesma regra de uma marca por duelo.
        private void KnowledgeMarkLeadUp(int arrivingId)
        {
            if (_knowledgeCounts == null || _opponentResolvedThisDuel.Count == 0)
                return;
            int sample;
            if (KnowledgeLift(arrivingId, out sample) < KnowledgeLiftBar)
                return;
            KnowledgeWriteSeen();
            int from = Math.Max(0, _opponentResolvedThisDuel.Count - KnowledgeBlameWindow);
            for (int i = from; i < _opponentResolvedThisDuel.Count; ++i)
            {
                int cardId = _opponentResolvedThisDuel[i];
                if (_knowledgeOutcomesThisDuel.Add(OutcomePre + "|" + cardId))
                    KnowledgeAppend(OutcomePre, cardId);
            }
        }

        private void KnowledgeMarkOutcome()
        {
            KnowledgeWriteSeen();   // garante o denominador antes de creditar o numerador
            const string outcome = OutcomeBad;
            // Uma vez por duelo e por carta. A medida é "em que fração dos duelos esta carta apareceu antes do
            // estrago" — se um duelo com três efeitos nossos negados creditasse três vezes, o numerador passaria
            // o denominador. Foi o que aconteceu na primeira semeadura: taxas de 233%.
            // Só as ÚLTIMAS cartas antes do estrago. Creditar todas as do duelo não distingue nada: medido em
            // 2026-09-23, com todas o baseline ia a 78% e 16 de 17 cartas passavam da barra, incluindo Gold
            // Sarcophagus e Ash Blossom. Com janela 2 o baseline cai para 32% e sobram as que realmente quebram
            // o nosso plano — no Branded, Called by the Grave, Brightest Blazing Branded King e Infinite
            // Impermanence, que são as respostas DELE. Janela 1 perdia a Welcome Labrynth; janela 3 diluía.
            int from = Math.Max(0, _opponentResolvedThisDuel.Count - KnowledgeBlameWindow);
            for (int i = from; i < _opponentResolvedThisDuel.Count; ++i)
            {
                int cardId = _opponentResolvedThisDuel[i];
                if (!_knowledgeOutcomesThisDuel.Add(outcome + "|" + cardId))
                    continue;
                KnowledgeAppend(outcome, cardId);
            }
            KnowledgeFlush();
        }

        // O processo costuma morrer junto com o duelo, mas ele também pode jogar várias partidas seguidas. Sem
        // zerar, o duelo seguinte herdaria as cartas do anterior e creditaria resultado a quem não estava lá.
        private int _lastSeenTurn;

        // PONTO CEGO que os logs de 2026-09-23 mostraram: o consequente era só "plano desabou" ou "efeito nosso
        // negado", então o bot só aprendia sobre a INTERAÇÃO dele, nunca sobre a mesa dele. Resultado prático: a
        // memória disse "segura contra Branded Fusion, ela não é a que nos machuca" — e a Branded Fusion é
        // justamente o que monta a mesa que nos mata na batalha, sem tocar no nosso plano.
        // Levar dano grande é o terceiro consequente, e é o que faltava para a conta enxergar isso.
        private const int KnowledgeLifeDrop = 2000;
        private int _lastLifePoints;

        private void KnowledgeCheckLifeLoss()
        {
            int now = Bot.LifePoints;
            if (_lastLifePoints > 0 && (now <= 0 || _lastLifePoints - now >= KnowledgeLifeDrop))
            {
                Report("opponent", string.Format("memory: we lost {0} LP — crediting {1} card(s) of theirs",
                    _lastLifePoints - now, _opponentResolvedThisDuel.Count));
                KnowledgeMarkOutcome();
            }
            _lastLifePoints = now;
        }

        private void KnowledgeNewDuel()
        {
            _lastLifePoints = 0;
            KnowledgeFlush();   // retry natural: os pendentes de falhas do duelo anterior tentam gravar aqui
            _opponentResolvedThisDuel.Clear();
            _knowledgeOutcomesThisDuel.Clear();
            _knowledgeSeenWritten.Clear();
            _knowledgeWriteNotice = false;
            _planCollapseRecorded = false;
            _lastPlanTier = -1;
        }

        // Plano desabou: a camada piorou e sobrou pouca coisa. É o sinal que o log já mostrava nas derrotas —
        // "new plan #3: 0 steps, tier 3" logo depois de um efeito dele. Registra uma vez por duelo, senão um
        // turno ruim conta várias vezes e distorce a proporção.
        private const int PlanCollapseSteps = 3;
        private int _lastPlanTier = -1;
        private bool _planCollapseRecorded;

        private void KnowledgeCheckPlanCollapse()
        {
            if (_plan == null)
                return;
            int tier = _plan.Tier;
            bool worse = _lastPlanTier >= 0 && tier > _lastPlanTier;
            _lastPlanTier = tier;
            if (_planCollapseRecorded || !worse || _plan.Steps.Count > PlanCollapseSteps)
                return;
            if (_opponentResolvedThisDuel.Count == 0)
                return;   // sem nada dele resolvido, o plano caiu por conta própria: não é aprendizado sobre ele
            _planCollapseRecorded = true;
            Report("opponent", string.Format("memory: our plan collapsed to tier {0} with {1} step(s) — crediting {2} card(s) of theirs",
                tier, _plan.Steps.Count, _opponentResolvedThisDuel.Count));
            KnowledgeMarkOutcome();
        }

        // O que a memória já sabe sobre este arquétipo: nota por carta e quantas rotas passam da nota. Só log.
        // Acima disto a carta destoa da média do arquétipo o bastante para mudar decisão. Medido em 2026-09-23:
        // com lift >= 2,0 sobram 4 de 17 no Branded (Called by the Grave, Brightest Blazing Branded King,
        // Infinite Impermanence, Mirrorjade) e 2 de 10 no Labrynth (Ash Blossom, Welcome Labrynth) — as respostas
        // deles, que é exatamente o que quebra o nosso plano.
        private const double KnowledgeLiftBar = 2.0;
        private const int KnowledgeCardsLogged = 6;

        private void ReportKnowledge()
        {
            if (_knowledgeCounts == null || string.IsNullOrEmpty(_knowledgeArchetype))
                return;
            string prefix = _knowledgeArchetype + "|";
            var rates = new List<string>();
            int routes = 0;
            foreach (var entry in _knowledgeCounts)
            {
                if (!entry.Key.StartsWith(prefix, StringComparison.Ordinal) || !entry.Key.EndsWith("|" + OutcomeSeen, StringComparison.Ordinal))
                    continue;
                string middle = entry.Key.Substring(prefix.Length, entry.Key.Length - prefix.Length - OutcomeSeen.Length - 1);
                int cardId;
                if (!int.TryParse(middle, out cardId))
                    continue;
                int sample;
                double lift = KnowledgeLift(cardId, out sample);
                if (lift < KnowledgeLiftBar)
                    continue;
                routes++;
                var data = YGOSharp.OCGWrapper.NamedCard.Get(cardId);
                rates.Add(string.Format("{0} lift {1:0.0} of {2}", data != null ? data.Name : cardId.ToString(), lift, sample));
            }
            if (rates.Count == 0)
            {
                Report("opponent", "memory: nothing stands out about " + _knowledgeArchetype + " yet");
                return;
            }
            rates.Sort(StringComparer.Ordinal);
            Report("opponent", string.Format("memory on {0}: {1} card(s) above lift {2:0.0} (baseline {3:0}%) — {4}",
                _knowledgeArchetype, routes, KnowledgeLiftBar, KnowledgeBaseline() * 100,
                string.Join(", ", rates.Take(KnowledgeCardsLogged))));
            if (routes > 1)
                Report("opponent", string.Format(
                    "memory: {0} different route(s) lead to our trouble here, so cutting one does not stop it", routes));
        }

        // Escreve os pendentes sob o mutex cross-processo. A fila só limpa em SUCESSO: antes, exceção engolida +
        // Clear() perdia o lote inteiro para sempre. Em falha os pendentes ficam e o próximo flush (a cada 16
        // registros, em cada desfecho e no início do duelo seguinte) tenta de novo. A compactação por tamanho roda
        // aqui porque um processo que vive muitos duelos sem morrer só cresce por apends — o load cobre só o
        // startup. Pasta ausente agora é criada: antes pular a escrita e limpar a fila assim mesmo era perda total.
        private void KnowledgeFlush()
        {
            if (_knowledgePending.Count == 0)
                return;
            if (_knowledgePending.Count > KnowledgePendingMax)
            {
                _knowledgePending.RemoveRange(0, _knowledgePending.Count - KnowledgePendingMax);
                KnowledgeWriteNotice("pendentes no teto de " + KnowledgePendingMax
                    + " linha(s): descartados os registros mais antigos");
            }
            if (!KnowledgeLock())
            {
                KnowledgeWriteNotice("outro escritor segura o arquivo — " + _knowledgePending.Count
                    + " linha(s) retidas para o próximo flush");
                return;
            }
            bool written = false;
            try
            {
                string path = KnowledgePath();
                try
                {
                    string dir = System.IO.Path.GetDirectoryName(path);
                    if (!string.IsNullOrEmpty(dir))
                        System.IO.Directory.CreateDirectory(dir);
                    System.IO.File.AppendAllLines(path, _knowledgePending);
                    written = true;
                }
                catch (Exception e)
                {
                    KnowledgeWriteNotice("falha ao gravar " + _knowledgePending.Count + " linha(s) ("
                        + e.GetType().Name + ") — retidas para o próximo flush");
                }
                if (written)
                {
                    try
                    {
                        if (new System.IO.FileInfo(path).Length >= KnowledgeCompactBytes)
                            KnowledgeCompact(-1);
                    }
                    catch { }   // o apend já gravou: compactação é melhor esforço e falha mantê-lo-ia intacto
                }
            }
            finally
            {
                KnowledgeUnlock();
            }
            if (written)
                _knowledgePending.Clear();
        }

        // Agregado carregado uma vez por processo: (arquétipo|carta) -> vezes vista, e (arquétipo|carta|resultado)
        // -> vezes que aquele resultado veio depois dela.
        private static Dictionary<string, int> _knowledgeCounts;
        private static readonly object _knowledgeLock = new object();

        private static string Field(string line, string key)
        {
            int at = line.IndexOf("\"" + key + "\":", StringComparison.Ordinal);
            if (at < 0)
                return null;
            at += key.Length + 3;
            if (at < line.Length && line[at] == '"')
            {
                int end = line.IndexOf('"', at + 1);
                return end < 0 ? null : line.Substring(at + 1, end - at - 1);
            }
            int stop = at;
            while (stop < line.Length && (char.IsDigit(line[stop]) || line[stop] == '-')) stop++;
            return stop > at ? line.Substring(at, stop - at) : null;
        }

        // Uma linha do arquivo -> uma entrada agregada. Usada no load e na releitura da compactação; os dois
        // caminhos precisam calcular a chave IGUAL, senão a taxa de repetição e os totais erram.
        private static bool KnowledgeCountLine(Dictionary<string, int> counts, string line)
        {
            if (line.Length < 10)
                return false;
            string archetype = Field(line, "a"), outcome = Field(line, "o"), card = Field(line, "c"), count = Field(line, "n");
            if (archetype == null || outcome == null || card == null)
                return false;
            int add;
            if (!int.TryParse(count, out add))
                add = 1;
            string key = archetype + "|" + card + "|" + outcome;
            int current;
            counts.TryGetValue(key, out current);
            counts[key] = current + add;
            return true;
        }

        private void KnowledgeLoad()
        {
            lock (_knowledgeLock)
            {
                if (_knowledgeCounts != null)
                    return;
                if (!KnowledgeEnabled)
                {
                    _knowledgeCounts = new Dictionary<string, int>(StringComparer.Ordinal);
                    return;
                }
                // Ler e compactar no MESMO mutex: soltar no meio deixaria outro processo apendear entre a
                // leitura e a reescrita. A guarda "!= null" é uma vez por processo, então o timeout não pode
                // simplesmente devolver sem publicar — senão a memória ficaria desligada para o processo todo
                // (a próxima chamada pularia). Publicar vazio mantém o comportamento do catch de leitura.
                if (!KnowledgeLock())
                {
                    _knowledgeCounts = new Dictionary<string, int>(StringComparer.Ordinal);
                    return;
                }
                try
                {
                    var counts = new Dictionary<string, int>(StringComparer.Ordinal);
                    int lines = 0;
                    try
                    {
                        string path = KnowledgePath();
                        if (System.IO.File.Exists(path))
                            foreach (string line in System.IO.File.ReadLines(path))
                                if (KnowledgeCountLine(counts, line))
                                    lines++;
                    }
                    catch { }
                    _knowledgeCounts = counts;
                    if (lines > 0)
                        Report("opponent", string.Format("memory: {0} record(s) read, {1} distinct key(s)",
                            lines, counts.Count));
                    KnowledgeCompact(lines);
                }
                finally
                {
                    KnowledgeUnlock();
                }
            }
        }

        // O arquivo é apendável de propósito — é isso que o torna à prova de o processo morrer no meio do duelo.
        // O preço é que a mesma chave se repete a cada partida. A compactação reescreve uma linha por chave, com
        // o total no campo "n".
        //
        // Isto não existia até 2026-09-23: o arquivo tinha chegado a 9.793 linhas e 423 KB com 1.478 chaves
        // distintas, ou seja, mais de 6 linhas por chave. Depois de compactar são 1.478 linhas.
        //
        // O CHAMADOR segura o mutex (load ou flush). Duas regras contra as perdas silenciosas conhecidas:
        //   1. a escrita parte de uma RELEITURA do arquivo, nunca do snapshot do load — reescrever a partir da
        //      memória apagava tudo que outros processos gravaram desde o nosso load;
        //   2. grava em .tmp e troca com File.Replace: o arquivo nunca fica truncado no meio, e falha na troca
        //      mantê-lo-ia intacto. A memória passa a ser exatamente essa releitura — o "recarregar" custa zero
        //      (são os bytes já lidos) e é o que faz o processo enxergar o que os outros gravaram.
        //
        // lines < 0 = disparo por tamanho (flush): aí a razão de repetição não serve, porque lines seria o
        // número do load, não o do arquivo atual.
        private const int KnowledgeCompactRatio = 3;

        private void KnowledgeCompact(int lines)
        {
            string path = KnowledgePath();
            try
            {
                if (lines >= 0)
                {
                    int distinct = _knowledgeCounts != null ? _knowledgeCounts.Count : 0;
                    if (lines <= distinct * KnowledgeCompactRatio)
                        return;
                }
                if (!System.IO.File.Exists(path))
                    return;
                var fresh = new Dictionary<string, int>(StringComparer.Ordinal);
                int freshLines = 0;
                foreach (string line in System.IO.File.ReadLines(path))
                    if (KnowledgeCountLine(fresh, line))
                        freshLines++;
                if (freshLines == 0)
                    return;   // arquivo sumiu ou virou lixo sob nós: aborta, memória e original intactos
                var compacted = new List<string>(fresh.Count);
                foreach (var entry in fresh)
                {
                    // A chave é "arquétipo|carta|resultado". O arquétipo pode ter qualquer coisa dentro, então o
                    // corte é feito da DIREITA para a esquerda, onde os dois campos são conhecidos.
                    int lastBar = entry.Key.LastIndexOf('|');
                    if (lastBar <= 0) continue;
                    int firstOfPair = entry.Key.LastIndexOf('|', lastBar - 1);
                    if (firstOfPair < 0) continue;
                    string archetype = entry.Key.Substring(0, firstOfPair);
                    string card = entry.Key.Substring(firstOfPair + 1, lastBar - firstOfPair - 1);
                    string outcome = entry.Key.Substring(lastBar + 1);
                    compacted.Add(string.Format("{{\"a\":\"{0}\",\"o\":\"{1}\",\"c\":{2},\"n\":{3}}}",
                        archetype, outcome, card, entry.Value));
                }
                if (compacted.Count == 0)
                    return;
                string temp = path + ".tmp";
                System.IO.File.WriteAllLines(temp, compacted);
                try
                {
                    System.IO.File.Replace(temp, path, null);
                }
                catch
                {
                    try { System.IO.File.Delete(temp); }
                    catch { }
                    return;
                }
                _knowledgeCounts = fresh;
                Report("opponent", string.Format("memory: compacted {0} line(s) into {1}", freshLines, fresh.Count));
            }
            catch { }
        }

        private int KnowledgeCount(string archetype, int cardId, string outcome)
        {
            if (_knowledgeCounts == null || string.IsNullOrEmpty(archetype))
                return 0;
            int value;
            _knowledgeCounts.TryGetValue(archetype + "|" + cardId + "|" + outcome, out value);
            return value;
        }

        // Nota da carta: das vezes que ela resolveu contra nós neste arquétipo, em quantas veio coisa ruim depois.
        // Devolve -1 quando não há amostra suficiente para opinar.
        private const int KnowledgeMinSample = 3;

        // Taxa crua: das vezes que esta carta resolveu contra nós neste arquétipo, em quantas veio estrago logo
        // depois. -1 quando não há amostra para opinar.
        private double KnowledgeBadRate(int cardId, out int sample)
        {
            sample = KnowledgeCount(_knowledgeArchetype, cardId, OutcomeSeen);
            if (sample < KnowledgeMinSample)
                return -1;
            int bad = KnowledgeCount(_knowledgeArchetype, cardId, OutcomeBad);
            return (double)bad / sample;
        }

        // Quanto a carta pesa ACIMA da média do arquétipo. A taxa crua sozinha não serve: num deck que nos
        // desmonta quase sempre, tudo dá alto. O lift divide pelo baseline e só sobra quem destoa.
        private double KnowledgeBaselineFor(string outcome) { return KnowledgeBaselineIn(_knowledgeArchetype, outcome); }

        private double KnowledgeBaselineIn(string key, string outcome)
        {
            if (_knowledgeCounts == null || string.IsNullOrEmpty(key))
                return 0;
            string prefix = key + "|";
            int seen = 0, bad = 0;
            foreach (var entry in _knowledgeCounts)
            {
                if (!entry.Key.StartsWith(prefix, StringComparison.Ordinal))
                    continue;
                if (entry.Key.EndsWith("|" + OutcomeSeen, StringComparison.Ordinal)) seen += entry.Value;
                else if (entry.Key.EndsWith("|" + outcome, StringComparison.Ordinal)) bad += entry.Value;
            }
            return seen > 0 ? (double)bad / seen : 0;
        }

        private double KnowledgeLift(int cardId, out int sample)
        {
            return KnowledgeLiftFor(cardId, OutcomeBad, out sample);
        }

        private double KnowledgeLiftFor(int cardId, string outcome, out int sample)
        {
            // Primeiro a chave do arquétipo, se ela tiver amostra própria: ali a carta é medida no contexto em
            // que está. Sem amostra, a global, que junta todos os decks em que a carta apareceu.
            sample = KnowledgeCount(_knowledgeArchetype, cardId, OutcomeSeen);
            if (sample >= KnowledgeArchetypeSample)
                return LiftIn(_knowledgeArchetype, cardId, outcome, sample);
            sample = KnowledgeCount(KnowledgeGlobalKey, cardId, OutcomeSeen);
            if (sample >= KnowledgeMinSample)
                return LiftIn(KnowledgeGlobalKey, cardId, outcome, sample);
            // Carta sem histórico próprio: cai na classe do efeito dela.
            string signature = KnowledgeClassOf(cardId);
            if (signature == null)
                return -1;
            sample = KnowledgeCount(signature, 0, OutcomeSeen);
            if (sample < KnowledgeClassSample)
                return -1;
            return LiftIn(signature, 0, outcome, sample);
        }

        private double LiftIn(string key, int cardId, string outcome, int sample)
        {
            double rate = (double)KnowledgeCount(key, cardId, outcome) / sample;
            double baseline = KnowledgeBaselineIn(key, outcome);
            return baseline > 0 ? rate / baseline : -1;
        }

        private double KnowledgeBaseline() { return KnowledgeBaselineFor(OutcomeBad); }

        private void ReportOpponentProfile()
        {
            List<ClientCard> seen = SeenOpponentCards();
            if (seen.Count == 0)
                return;
            string signature = string.Join(",", seen.Select(card => card.Id).Distinct().OrderBy(id => id));
            if (signature == _opponentProfileSignature)
                return;
            _opponentProfileSignature = signature;
            BuildReferenceIndex();
            if (_referenceIndex == null || _referenceIndex.Count == 0)
                return;

            ReportStandingThreatsOnField();
            var seenNames = seen.Select(card => card.Name).Where(name => !string.IsNullOrEmpty(name)).Distinct().ToList();
            // Arquétipos: trechos entre aspas dos textos dele que NÃO são nome de carta.
            var archetypes = new List<string>();
            // Conta quantas cartas dele citam cada rótulo. A ordem importa para a memória: o rótulo escolhido
            // vira a chave do arquivo, e pegar "o primeiro que apareceu" fazia o MESMO deck cair em três baldes
            // diferentes — "Bystial", "Dogmatika" e "Branded" eram o mesmo Branded, com a amostra dividida por
            // três (visto em 2026-09-23). O mais citado é estável entre partidas.
            var archetypeHits = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (ClientCard card in seen)
            {
                var data = YGOSharp.OCGWrapper.NamedCard.Get(CardCode(card));
                if (data == null) continue;
                foreach (string token in QuotedTokens(data.Description).Distinct())
                {
                    if (_cardNameSet.Contains(token))
                        continue;
                    if (!archetypes.Contains(token))
                        archetypes.Add(token);
                    int hits;
                    archetypeHits.TryGetValue(token, out hits);
                    archetypeHits[token] = hits + 1;
                }
            }
            archetypes = archetypes.OrderByDescending(token => archetypeHits[token]).ToList();
            foreach (ClientCard card in seen)
                ReportEffectProfile(CardCode(card), card.Name);
            Report("opponent", string.Format("profile: {0} card(s) seen{1}", seenNames.Count,
                archetypes.Count > 0 ? ", archetypes: " + string.Join(", ", archetypes.Take(6)) : ""));

            // Assinatura do arquétipo para a memória: o rótulo mais citado nos textos dele. Um só, estável entre
            // partidas — "Branded", "Labrynth". Rótulo de uma letra ou vazio não serve de chave.
            string dominant = archetypes.FirstOrDefault(token => token.Length > 1);
            if (dominant != null && dominant != _knowledgeArchetype)
            {
                _knowledgeArchetype = dominant;
                KnowledgeLoad();
                KnowledgeWriteSeen();   // o arquétipo pode só ficar claro depois de ele já ter jogado
                ReportKnowledge();
            }

            List<OpponentBoss> bosses = ReachableOpponentBosses(seenNames);
            _projectedBosses = bosses;
            if (bosses.Count == 0)
                return;
            Report("opponent", string.Format("{0} Extra Deck boss(es) reachable from what we have seen", bosses.Count));
            foreach (OpponentBoss boss in bosses.Take(OpponentBossesLogged))
            {
                Report("opponent", string.Format("  boss: {0} (ATK {1}, cost {2}, support {3}, by {4}{5}) | materials: {6}{7}",
                    boss.Name, boss.Attack, boss.Difficulty, boss.Support.Count,
                    boss.FromArchetype ? "archetype" : "name",
                    boss.OnField ? ", already on the field" : boss.Revealed ? ", revealed" : "", boss.Materials,
                    boss.Have.Count > 0 ? " | already seen: " + string.Join(", ", boss.Have) : ""));
            }
            ReportOpponentChokePoints(bosses);
        }

        // Varredura do que está DE PÉ no campo dele agora. Pega inclusive a carta que nunca ativa nada e por isso
        // nunca passa pelo OnChainSolved — o Dark Contract with the Eternal Darkness é desse tipo, e foi ele que
        // travou o bot no duelo contra D/D/D. Só log.
        private void ReportStandingThreatsOnField()
        {
            var standing = Enemy.GetMonsters().Concat(Enemy.GetSpells())
                .Where(card => card != null && card.IsFaceup()).ToList();
            foreach (ClientCard card in standing)
            {
                int id = CardCode(card);
                if (id == 0 || !_reportedStandingThreats.Add(id))
                    continue;
                ReportStandingThreat(card);
            }
        }

        private readonly HashSet<int> _reportedStandingThreats = new HashSet<int>();

        private const int OpponentBossesLogged = 6;
        // Um boss vindo do setcode só conta se o arquétipo dele cobrir pelo menos 1/3 do que já vimos da mesa dele.
        private const int ArchetypeShareDivisor = 3;
        // Abaixo disto o boss não justifica gastar interação para impedir que ele chegue.
        private const int BossWorthPreventing = 60;
        // Com mais apoio que isto, cortar uma carta não impede nada: guarde a resposta para a mesa pronta.
        private const int ChokeMaxSupport = 2;
        private const int ChokePointsLogged = 4;

        // -----------------------------------------------------------------------------------------------------
        // CAMADA 2 (relatório) — o gargalo. Não é uma carta com nome no código: é a carta vista que sustenta um
        // boss caro e tem POUCO apoio, ou seja, o ponto onde o caminho dele é mais estreito. É o que o jogador
        // chamou de "elo fraco" (2026-09-23). No Branded isso cai no Albion; no Rose cai onde o Tuner aperta.
        // Só log: nenhuma decisão usa isto ainda.
        // -----------------------------------------------------------------------------------------------------
        private void ReportOpponentChokePoints(List<OpponentBoss> bosses)
        {
            var costByCard = new Dictionary<string, int>(StringComparer.Ordinal);
            var bossByCard = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (OpponentBoss boss in bosses)
            {
                // Boss que já está na mesa dele não tem gargalo: a hora de impedir passou, e a resposta agora é
                // remover, não prevenir. E sem apoio nenhum não há por onde cortar.
                if (boss.OnField || boss.Support.Count == 0)
                    continue;
                if (boss.Difficulty < BossWorthPreventing || boss.Support.Count > ChokeMaxSupport)
                    continue;
                foreach (string card in boss.Support)
                {
                    int current;
                    if (costByCard.TryGetValue(card, out current) && current >= boss.Difficulty)
                        continue;
                    costByCard[card] = boss.Difficulty;
                    bossByCard[card] = boss.Name;
                }
            }
            if (costByCard.Count == 0)
            {
                // Silêncio aqui também é resultado: ou nada caro está ao alcance, ou tudo que está tem caminho
                // demais e a resposta deve ser guardada para a mesa pronta.
                var expensive = bosses.Where(boss => boss.Difficulty >= BossWorthPreventing).ToList();
                int already = expensive.Count(boss => boss.OnField);
                int wide = expensive.Count(boss => !boss.OnField && boss.Support.Count > ChokeMaxSupport);
                Report("opponent", string.Format(
                    "no choke point: {0} expensive boss(es) — {1} already on the field, {2} with more than {3} path(s) seen",
                    expensive.Count, already, wide, ChokeMaxSupport));
                return;
            }
            foreach (var entry in costByCard.OrderByDescending(pair => pair.Value).Take(ChokePointsLogged))
            {
                Report("opponent", string.Format("  choke point: {0} -> {1} (cost {2})",
                    entry.Key, bossByCard[entry.Key], entry.Value));
            }
        }

        // Monta a lista para o estado do planejador e, de passagem, avisa no log cada carta nova (uma vez por duelo).
        private int[] ReadKnownEnemyHand()
        {
            ReportOpponentProfile();
            List<ClientCard> known = KnownEnemyHand();
            foreach (ClientCard card in known)
            {
                if (_reportedEnemyHandIds.Contains(card.Id))
                    continue;
                _reportedEnemyHandIds.Add(card.Id);
                Report("interaction", "opponent hand revealed: " + (card.Name ?? card.Id.ToString()));
            }
            return known.Select(card => CardCode(card)).Distinct().OrderBy(code => code).ToArray();
        }

        private int KnownId(ClientCard card)
        {
            if (card == null)
                return 0;
            if (card.Id != 0)
                return card.Id;
            int id;
            return _knownEnemyIds.TryGetValue(card, out id) ? id : 0;
        }

        private string DescribeKnown(ClientCard card)
        {
            int id = KnownId(card);
            return id != 0 ? RdaCards.Name(id) : "unknown face-down card";
        }

        private BattleTrait TraitsOf(ClientCard card)
        {
            return TraitsOfId(KnownId(card));
        }

        // Mesma leitura, a partir do id. A projeção precisa disto para um boss que AINDA não está no campo.
        private static BattleTrait TraitsOfId(int id)
        {
            if (id == 0)
                return BattleTrait.None;
            BattleTrait traits;
            lock (BattleTraitCache)
            {
                if (!BattleTraitCache.TryGetValue(id, out traits))
                {
                    YGOSharp.OCGWrapper.NamedCard data = YGOSharp.OCGWrapper.NamedCard.Get(id);
                    string text = data != null && data.Description != null ? data.Description.ToLowerInvariant() : "";
                    // Reflexo: sempre do texto (não existe constante única para ele no script).
                    if (ReflectText.IsMatch(text))
                        traits |= BattleTrait.Reflect;
                    // Impressão de arte alternativa não tem script próprio (c29053657.lua não existe, o do Quetzacoatl
                    // está sob 29053656): o alias leva ao id original.
                    string script = RdaScripts.Read(id, data != null ? data.Alias : 0);
                    if (script != null)
                    {
                        if (RdaScripts.HasOwnEffect(script, "EFFECT_INDESTRUCTABLE_BATTLE"))
                            traits |= BattleTrait.BattleIndestructible;
                        if (RdaScripts.HasOwnEffect(script, "EFFECT_INDESTRUCTABLE_EFFECT"))
                            traits |= BattleTrait.EffectIndestructible;
                        if (RdaScripts.HasOwnEffect(script, "EFFECT_IMMUNE_EFFECT"))
                            traits |= BattleTrait.ImmuneEffect;
                    }
                    else
                    {
                        // Sem script: comportamento antigo, só o texto em inglês.
                        if (text.Contains("cannot be destroyed by battle"))
                            traits |= BattleTrait.BattleIndestructible;
                        if (EffectIndestructibleText.IsMatch(text))
                            traits |= BattleTrait.EffectIndestructible;
                    }
                    BattleTraitCache[id] = traits;
                }
            }
            return traits;
        }

        // =====================================================================================================
        // Leitura dos scripts Lua do cliente (opcional, nunca obrigatória)
        //
        // ONDE DEIXAR A PASTA DO BOT (importante para isto funcionar):
        //   O ideal é a pasta do WindBot ficar AO LADO da pasta do cliente, sob um diretório comum. Exemplo real
        //   desta máquina, que a busca resolve de primeira:
        //       E:\GAMES\PC\Yugioh\MDPro3\Data\script.zip      <- cliente
        //       E:\GAMES\PC\Yugioh\windbot-master\bin\Debug\   <- bot roda daqui
        //   A busca sobe a partir da pasta do executável e, em cada nível, procura "script.zip", "Data\script.zip"
        //   e "<subpasta>\Data\script.zip" — um nível de subpasta só. Medido: acha em ~140 ms; quando não existe,
        //   desiste em ~200 ms. NÃO varre o disco recursivamente: isso levaria minutos e travaria a partida.
        //
        // SE NÃO ENCONTRAR, NADA QUEBRA: TraitsOf volta a ler o texto em inglês do cards.cdb, exatamente como antes.
        //   Vale para a cópia oficial em outra máquina, para o cliente desinstalado e para uma atualização que mova
        //   o arquivo. Toda falha aqui é engolida de propósito; este módulo nunca lança.
        // =====================================================================================================
        // -----------------------------------------------------------------------------------------------------
        // Perfil de efeito — SÓ OBSERVAÇÃO. Classifica cada efeito de uma carta e escreve no log.
        //
        // Fonte principal: o script Lua, onde cada efeito declara o que faz (SetCategory), quando pode ser usado
        // (SetType), em cima de que evento (SetCode) e se é uma-vez-por-turno (SetCountLimit). Levantamento nos
        // 13.523 scripts (2026-09-22) para escolher o vocabulário: SPECIAL_SUMMON 11.601, TOHAND 5.918,
        // DESTROY 4.743, SEARCH 1.911, DRAW 1.576, REMOVE 1.527, DAMAGE 1.451, NEGATE 777, DISABLE 709.
        // Trava (floodgate) não é categoria do jogo: é derivada de um efeito de campo contínuo com um código
        // EFFECT_CANNOT_* — os mais comuns são CANNOT_SPECIAL_SUMMON 914, CANNOT_ATTACK 330, CANNOT_ACTIVATE 319.
        //
        // Fallback sem script: o texto do cards.cdb, com a mesma lista de palavras. Menos preciso, e marcado como
        // tal no log para não confundirmos as duas coisas.
        // -----------------------------------------------------------------------------------------------------
        private sealed class EffectProfile
        {
            public string Variable;       // e1, e2... como o script chama
            // Índice da DESCRIÇÃO do efeito, de aux.Stringid(id, N). É esse N que chega no ActivateDescription do
            // elo, e ele NÃO é a ordem de registro no script: casar por posição na lista erra a carta (visto nos
            // logs de 2026-09-22, ex.: Labrynth Stovie Torbie efeito #0 caindo no efeito errado). -1 = sem descrição.
            public int DescIndex = -1;
            public List<string> Does;     // o que faz: destroy, banish, search...
            public string When;           // quick, ignition, trigger, continuous, activate
            public string Event;          // evento do gatilho, quando houver
            public bool OncePerTurn;
            public bool HitsOpponent;     // efeito de campo que alcança o outro jogador
        }

        private static readonly Dictionary<int, List<EffectProfile>> EffectProfileCache = new Dictionary<int, List<EffectProfile>>();

        // Categoria do script -> palavra que usamos. Só o que importa para decidir interação.
        private static readonly string[][] CategoryWords =
        {
            new[] { "CATEGORY_DESTROY", "destroy" },
            new[] { "CATEGORY_REMOVE", "banish" },
            new[] { "CATEGORY_TOHAND", "to hand" },
            new[] { "CATEGORY_SEARCH", "search" },
            new[] { "CATEGORY_TOGRAVE", "to grave" },
            new[] { "CATEGORY_TODECK", "to deck" },
            new[] { "CATEGORY_SPECIAL_SUMMON", "special summon" },
            new[] { "CATEGORY_DRAW", "draw" },
            // Descarte da NOSSA mão: 96 cartas no banco e nenhuma pontuava como ameaça até aqui (auditoria de
            // 2026-09-23). O HANDES_SELF é o descarte da mão dele, que não nos custa nada, então fica de fora.
            new[] { "CATEGORY_HANDES_OPPO", "discards our hand" },
            new[] { "CATEGORY_DAMAGE", "burn" },
            new[] { "CATEGORY_RECOVER", "gain life" },
            new[] { "CATEGORY_NEGATE", "negate" },
            new[] { "CATEGORY_DISABLE", "disable" },
            new[] { "CATEGORY_ATKCHANGE", "atk change" },
            new[] { "CATEGORY_POSITION", "position" },
            new[] { "CATEGORY_CONTROL", "take control" }
        };

        // Códigos que, num efeito de campo contínuo, significam trava. Lista tirada da contagem nos 13.523 scripts
        // (2026-09-22). A família "não pode ser material" faltava e foi exatamente ela que travou o bot contra D/D/D:
        // Dark Contract with the Eternal Darkness registra EFFECT_CANNOT_BE_SYNCHRO_MATERIAL, o menu parou de oferecer
        // qualquer Invocação-Especial e o bot passou o turno tentando Synchros ilegais até perder.
        // As mais longas vêm primeiro: CANNOT_ATTACK_ANNOUNCE contém CANNOT_ATTACK, e a primeira que casar vence.
        private static readonly string[][] FloodgateWords =
        {
            new[] { "EFFECT_CANNOT_BE_SYNCHRO_MATERIAL", "our monsters cannot be Synchro material" },   // 101 cartas
            new[] { "EFFECT_CANNOT_BE_FUSION_MATERIAL", "cannot be Fusion material" },                  // 50
            new[] { "EFFECT_CANNOT_BE_XYZ_MATERIAL", "cannot be Xyz material" },                        // 75
            new[] { "EFFECT_CANNOT_BE_LINK_MATERIAL", "cannot be Link material" },                      // 68
            new[] { "EFFECT_CANNOT_BE_BATTLE_TARGET", "cannot be attacked" },                           // 57
            new[] { "EFFECT_CANNOT_BE_EFFECT_TARGET", "cannot be targeted" },                           // 321
            new[] { "EFFECT_CANNOT_SELECT_BATTLE_TARGET", "locks battle targets" },                     // 134
            new[] { "EFFECT_CANNOT_ATTACK_ANNOUNCE", "locks attack declaration" },                      // 104
            new[] { "EFFECT_CANNOT_DIRECT_ATTACK", "locks direct attacks" },                            // 113
            new[] { "EFFECT_CANNOT_CHANGE_POSITION", "locks position change" },                         // 75
            new[] { "EFFECT_CANNOT_SPECIAL_SUMMON", "locks Special Summons" },                          // 914
            new[] { "EFFECT_CANNOT_FLIP_SUMMON", "locks Flip Summons" },                                // 34
            new[] { "EFFECT_CANNOT_SUMMON", "locks Normal Summons" },                                   // 137
            new[] { "EFFECT_CANNOT_ACTIVATE", "locks activations" },                                    // 319
            // Impede que a gente NEGUE as ativações dele. Faltava no vocabulário, e por isso o Branded Lost saía
            // sem trava nenhuma: o e2 dele (EFFECT_TYPE_FIELD + EFFECT_CANNOT_INACTIVATE, alcance LOCATION_SZONE)
            // ficava com Does vazio e era descartado pelo filtro do fim de ProfilesFromScript. Visto no duelo contra
            // Branded de 2026-09-23 (log 012308): a carta ficou no campo dele a partida inteira e a varredura de
            // ameaça de pé não apontou nada.
            new[] { "EFFECT_CANNOT_INACTIVATE", "locks our negations" },
            // Droll & Lock Bird. Foi a carta que mais derrubou combo nosso nos logs e estava fora do vocabulário:
            // o script dela registra os dois códigos DENTRO de função, então nem a varredura do initial_effect
            // nem o rótulo existiam. Medido em 2026-09-23: 14 e 10 cartas respectivamente, mas o impacto é alto.
            new[] { "EFFECT_CANNOT_TO_HAND", "locks our adds to the hand" },
            new[] { "EFFECT_CANNOT_DRAW", "locks our draws" },
            new[] { "EFFECT_CANNOT_DISEFFECT", "locks our effect negations" },
            new[] { "EFFECT_CANNOT_TRIGGER", "locks triggers" },                                        // 114
            new[] { "EFFECT_CANNOT_ATTACK", "locks attacks" },                                          // 330
            new[] { "EFFECT_CANNOT_MSET", "locks monster set" },                                        // 46
            new[] { "EFFECT_UNRELEASABLE_NONSUM", "cannot be tributed for effects" },                   // 49
            new[] { "EFFECT_UNRELEASABLE_SUM", "cannot be tributed for a Tribute Summon" },              // 68
            new[] { "EFFECT_NO_EFFECT_DAMAGE", "blocks effect damage" },                                // 57
            new[] { "EFFECT_NO_BATTLE_DAMAGE", "blocks battle damage" },                                // 31
            new[] { "EFFECT_CANNOT_BP", "skips the Battle Phase" },                                     // 50
            new[] { "EFFECT_DISABLE_EFFECT", "negates effects" },                                       // 599
            new[] { "EFFECT_DISABLE", "disables monsters" },                                            // 725
            // Mudança de característica: não proíbe nada, mas quebra receita de Synchro do mesmo jeito. Foi o
            // Black Rose Garden (EFFECT_CHANGE_RACE, "all face-up monsters become Plant") que derrubou o bot na
            // partida das Rosas — o Red Rising exige Tuner Fiend e o nosso Soul Resonator tinha virado Planta.
            new[] { "EFFECT_CHANGE_ATTRIBUTE", "changes Attribute on the field" },                      // 87
            new[] { "EFFECT_CHANGE_RACE", "changes Type on the field" },                                // 70
            new[] { "EFFECT_CHANGE_LEVEL", "changes Level on the field" },                              // 239
            new[] { "EFFECT_CHANGE_CODE", "changes card name on the field" },                           // 64
            new[] { "EFFECT_ADD_TYPE", "adds a card Type on the field" },                               // 132
            new[] { "EFFECT_ADD_ATTRIBUTE", "adds an Attribute on the field" }                          // 21
        };

        /// <summary>
        /// Só o corpo da initial_effect. Precisa ser recortado porque os scripts reusam nomes de variável entre
        /// funções: um "e1" da initial_effect e outro "e1" dentro de uma operação são efeitos diferentes, e agrupar
        /// por nome sem recortar o bloco mistura os dois (foi o que aconteceu na primeira leitura do Mirrorjade).
        /// </summary>
        private static string InitialEffectBlock(string script)
        {
            if (string.IsNullOrEmpty(script))
                return "";
            int start = script.IndexOf("initial_effect", StringComparison.Ordinal);
            if (start < 0)
                return "";
            // O bloco vai até a primeira linha que seja exatamente "end" começando na coluna 0.
            int i = script.IndexOf('\n', start);
            if (i < 0) return "";
            int line = i + 1;
            while (line < script.Length)
            {
                int next = script.IndexOf('\n', line);
                if (next < 0) next = script.Length;
                string content = script.Substring(line, next - line).TrimEnd('\r');
                if (content == "end")
                    return script.Substring(start, line - start);
                line = next + 1;
            }
            return script.Substring(start);
        }

        // Estático de propósito: assim dá para exercitar a classificação offline, sem montar um duelo.
        private static List<EffectProfile> EffectProfilesOf(int id)
        {
            lock (EffectProfileCache)
            {
                List<EffectProfile> cached;
                if (EffectProfileCache.TryGetValue(id, out cached))
                    return cached;
                var profiles = new List<EffectProfile>();
                // O banco pode não estar carregado (teste offline); o script sozinho já cobre a maior parte.
                YGOSharp.OCGWrapper.NamedCard data = null;
                try { data = YGOSharp.OCGWrapper.NamedCard.Get(id); } catch { }
                string script = RdaScripts.Read(id, data != null ? data.Alias : 0);
                string block = InitialEffectBlock(script);
                if (block.Length > 0)
                    profiles = ProfilesFromScript(block);
                // Terceira fonte, antes de cair no texto: varrer o ARQUIVO inteiro. Muita carta registra a
                // habilidade dentro da função de operation/target, e não no initial_effect — o Droll & Lock Bird,
                // o Predaplant Verte Anaconda, o Triple Tactics Talent e o Albion the Shrouded são assim.
                // Medido em 2026-09-23 sobre os 13.526 scripts do cliente: das 2.287 cartas sem rótulo no
                // initial_effect, 602 (26,3%) têm rótulo em algum lugar do arquivo. Nas cartas que realmente
                // enfrentamos, resolve 7 de 12.
                // O preço é perder a atribuição por efeito: sai um perfil só, sem índice de descrição. Isso já
                // encaixa no que existe, porque sem índice o OpponentEffectThreat considera o pior que a carta
                // sabe fazer — errar para o lado de negar é melhor do que deixar passar.
                if (!profiles.Any(profile => profile.Does.Count > 0) && !string.IsNullOrEmpty(script))
                {
                    EffectProfile wholeFile = ProfileFromWholeScript(script);
                    if (wholeFile != null)
                        profiles.Add(wholeFile);
                }
                if (profiles.Count == 0 && data != null)
                    profiles = ProfilesFromText(data.Description);
                EffectProfileCache[id] = profiles;
                return profiles;
            }
        }

        // Um perfil só, de nível de CARTA, juntando tudo que o arquivo cita. Marcado no Variable para o log
        // deixar claro que veio daqui e não de um efeito identificado.
        private static EffectProfile ProfileFromWholeScript(string script)
        {
            var profile = new EffectProfile { Variable = "file", Does = new List<string>(), When = "from the whole script" };
            foreach (string[] pair in CategoryWords)
                if (script.IndexOf(pair[0], StringComparison.Ordinal) >= 0 && !profile.Does.Contains(pair[1]))
                    profile.Does.Add(pair[1]);
            foreach (string[] pair in FloodgateWords)
                if (script.IndexOf(pair[0], StringComparison.Ordinal) >= 0 && !profile.Does.Contains(pair[1]))
                    profile.Does.Add(pair[1]);
            return profile.Does.Count > 0 ? profile : null;
        }

        private static List<EffectProfile> ProfilesFromScript(string block)
        {
            var order = new List<string>();
            var calls = new Dictionary<string, List<string[]>>(StringComparer.Ordinal);
            foreach (var match in System.Text.RegularExpressions.Regex.Matches(block,
                @"(?m)^\s*(\w+):Set(\w+)\(([^)]*)\)").Cast<System.Text.RegularExpressions.Match>())
            {
                string variable = match.Groups[1].Value;
                if (variable == "c") continue;              // c:RegisterEffect e afins não são um efeito
                if (!calls.ContainsKey(variable)) { calls[variable] = new List<string[]>(); order.Add(variable); }
                calls[variable].Add(new[] { match.Groups[2].Value, match.Groups[3].Value });
            }
            var profiles = new List<EffectProfile>();
            foreach (string variable in order)
            {
                var profile = new EffectProfile { Variable = variable, Does = new List<string>() };
                string type = "", code = "", range = "";
                foreach (string[] call in calls[variable])
                {
                    // Chaves obrigatórias: sem elas o "else" gruda no if de dentro do foreach (dangling else) e
                    // Type/Code/CountLimit nunca são lidos — foi exatamente o que aconteceu na primeira versão.
                    if (call[0] == "Category")
                    {
                        foreach (string[] pair in CategoryWords)
                        {
                            if (call[1].Contains(pair[0]) && !profile.Does.Contains(pair[1]))
                                profile.Does.Add(pair[1]);
                        }
                    }
                    else if (call[0] == "Type") { type = call[1]; }
                    else if (call[0] == "Code") { code = call[1]; }
                    else if (call[0] == "CountLimit") { profile.OncePerTurn = call[1].StartsWith("1"); }
                    else if (call[0] == "TargetRange") { range = call[1]; }
                    else if (call[0] == "Description")
                    {
                        // aux.Stringid(id, N) -> N é o índice que chega no ActivateDescription.
                        // Sem exigir o ")" final: o recorte da chamada para no primeiro parêntese fechado, então
                        // o conteúdo chega truncado como "aux.Stringid(id,0".
                        var m = System.Text.RegularExpressions.Regex.Match(call[1], @"Stringid\([^,]+,\s*(\d+)");
                        if (m.Success) profile.DescIndex = int.Parse(m.Groups[1].Value);
                    }
                }
                profile.When = type.Contains("QUICK_O") ? "quick"
                    : type.Contains("IGNITION") ? "ignition"
                    : type.Contains("TRIGGER") ? "trigger"
                    : type.Contains("ACTIVATE") ? "activate"
                    : type.Contains("CONTINUOUS") ? "continuous" : "";
                // Trava: efeito de campo contínuo cujo código proíbe alguma coisa.
                bool fieldContinuous = type.Contains("FIELD") || type.Contains("CONTINUOUS") || type == "";
                if (fieldContinuous)
                {
                    // Só a PRIMEIRA que casar: a lista está da mais longa para a mais curta, senão
                    // EFFECT_CANNOT_ATTACK casaria dentro de EFFECT_CANNOT_ATTACK_ANNOUNCE e sairiam dois rótulos.
                    foreach (string[] pair in FloodgateWords)
                    {
                        if (!code.Contains(pair[0]))
                            continue;
                        if (!profile.Does.Contains(pair[1]))
                            profile.Does.Add(pair[1]);
                        break;
                    }
                }
                if (code.Length > 0 && code.StartsWith("EVENT_"))
                    profile.Event = code;
                // SetTargetRange(x,y): o segundo número diferente de 0 alcança o outro jogador.
                string[] parts = range.Split(',');
                profile.HitsOpponent = parts.Length >= 2 && parts[1].Trim() != "0";
                if (profile.Does.Count > 0 || profile.Event != null)
                    profiles.Add(profile);
            }
            return profiles;
        }

        // Fallback sem script: o texto do cards.cdb. Um perfil só, e marcado como vindo do texto.
        private static List<EffectProfile> ProfilesFromText(string description)
        {
            var profiles = new List<EffectProfile>();
            if (string.IsNullOrEmpty(description))
                return profiles;
            string text = description.ToLowerInvariant();
            var profile = new EffectProfile { Variable = "text", Does = new List<string>(), When = "from card text" };
            if (text.Contains("destroy")) profile.Does.Add("destroy");
            if (text.Contains("banish")) profile.Does.Add("banish");
            if (text.Contains("add") && text.Contains("to your hand")) profile.Does.Add("search");
            if (text.Contains("special summon")) profile.Does.Add("special summon");
            if (text.Contains("draw")) profile.Does.Add("draw");
            if (text.Contains("damage")) profile.Does.Add("burn");
            if (text.Contains("negate")) profile.Does.Add("negate");
            if (text.Contains("cannot")) profile.Does.Add("restricts something");
            if (profile.Does.Count > 0)
                profiles.Add(profile);
            return profiles;
        }

        // Escreve o perfil de uma carta no log, uma vez por carta e por duelo.
        private void ReportEffectProfile(int id, string name)
        {
            if (id == 0 || !_profiledCardIds.Add(id))
                return;
            List<EffectProfile> profiles = EffectProfilesOf(id);
            if (profiles.Count == 0)
                return;
            Report("opponent", "effect profile: " + (name ?? id.ToString()));
            foreach (EffectProfile profile in profiles)
            {
                var parts = new List<string>();
                if (profile.Does.Count > 0) parts.Add(string.Join(" + ", profile.Does));
                if (!string.IsNullOrEmpty(profile.When)) parts.Add(profile.When);
                if (!string.IsNullOrEmpty(profile.Event)) parts.Add("on " + profile.Event);
                if (profile.OncePerTurn) parts.Add("once per turn");
                if (profile.HitsOpponent) parts.Add("reaches the other player");
                if (parts.Count > 0)
                    Report("opponent", "   " + profile.Variable + ": " + string.Join(" | ", parts));
            }
        }

        private static class RdaScripts
        {
            // Limites contra ZIP malicioso (zip bomb). Script de carta real é poucos KB — o maior c*.lua medido em
            // MDPro3/script.zip (2026-10-07) tem 8 KB e o maior arquivo do archive, procedure.lua, 87 KB — então
            // 512 KB já é folga enorme. O limite é DUAS camadas: a primeira confia no tamanho declarado no
            // cabeçalho do ZIP (recusa barata, mas o cabeçalho pode mentir), a segunda mede os bytes que a stream
            // entrega de fato (a garantia real). Sem a segunda, um ZIP com cabeçalho falso passa pela primeira.
            private const int MaxEntryBytes = 512 * 1024;
            // Tamanho do script.zip comprimido no disco: só sanidade (o real tem 13 MB); zip bomb cabe em arquivo
            // pequeno, quem segura o bomb é MaxEntryBytes.
            private const long MaxArchiveBytes = 256L * 1024 * 1024;
            // Máximo de entradas indexadas (o real tem 13527): impede Prepare de varrer archive absurdo sob o Gate.
            private const int MaxEntries = 65536;

            private static readonly object Gate = new object();
            private static bool _prepared;
            private static object _archive;                        // System.IO.Compression.ZipArchive
            private static Dictionary<string, object> _entries;    // "c123.lua" -> ZipArchiveEntry
            private static System.Reflection.MethodInfo _openEntry;
            private static System.Reflection.PropertyInfo _entryLength; // ZipArchiveEntry.Length (tamanho declarado)

            // Caminho explícito e confiável, se configurado. Config.Load pode não ter sido chamado (teste por
            // reflexão carrega só o assembly) — daí o try/catch: sem config, quem manda é a busca legada.
            // isConfigured distingue "chave ausente" (busca legada vale) de "chave presente mas arquivo não
            // existe" (local explícito é exclusivo: não volta para a busca ampla).
            private static string ConfiguredPath(out bool isConfigured)
            {
                isConfigured = false;
                string configured;
                try { configured = WindBot.Config.GetString("SCRIPTZIP"); }
                catch { return null; }
                if (string.IsNullOrEmpty(configured))
                    return null;
                isConfigured = true;
                try
                {
                    if (!System.IO.Path.IsPathRooted(configured))
                        configured = System.IO.Path.Combine(AssemblyDir(), configured);
                    return System.IO.File.Exists(configured) ? configured : null;
                }
                catch { return null; }
            }

            private static string AssemblyDir()
            {
                // Âncora na pasta do PRÓPRIO assembly, não em AppDomain.BaseDirectory: quando outro processo carrega o
                // WindBot (teste por reflexão, host embutido), o BaseDirectory é o do hospedeiro e a busca partiria do
                // lugar errado.
                string dir = null;
                try { dir = System.IO.Path.GetDirectoryName(typeof(TheCrimsonKingExecutor).Assembly.Location); }
                catch { }
                if (string.IsNullOrEmpty(dir))
                    dir = AppDomain.CurrentDomain.BaseDirectory;
                return dir;
            }

            // Busca legada, usada só quando SCRIPTZIP não está configurado: sobe no máximo 6 níveis a partir da
            // pasta do executável procurando o script.zip do cliente.
            private static string Locate()
            {
                bool isConfigured;
                string configured = ConfiguredPath(out isConfigured);
                // Local explícito configurado é EXCLUSIVO: se não existir, não cai na busca ampla.
                if (isConfigured)
                    return configured;
                string dir = AssemblyDir();
                for (int level = 0; level < 6 && !string.IsNullOrEmpty(dir); ++level)
                {
                    string direct = System.IO.Path.Combine(dir, "script.zip");
                    if (System.IO.File.Exists(direct)) return direct;
                    string inDataFolder = System.IO.Path.Combine(dir, System.IO.Path.Combine("Data", "script.zip"));
                    if (System.IO.File.Exists(inDataFolder)) return inDataFolder;
                    try
                    {
                        foreach (string sub in System.IO.Directory.GetDirectories(dir))
                        {
                            string candidate = System.IO.Path.Combine(sub, System.IO.Path.Combine("Data", "script.zip"));
                            if (System.IO.File.Exists(candidate)) return candidate;
                        }
                    }
                    catch { }
                    System.IO.DirectoryInfo parent = System.IO.Directory.GetParent(
                        dir.TrimEnd(System.IO.Path.DirectorySeparatorChar));
                    dir = parent == null ? null : parent.FullName;
                }
                return null;
            }

            // Reflexão em vez de referência de projeto: assim nenhum dos dois .csproj muda e a cópia oficial compila
            // sem alteração. Mesmo recurso já usado no DuelLog.
            private static void Prepare()
            {
                if (_prepared) return;
                _prepared = true;
                try
                {
                    string path = Locate();
                    if (path == null) return;
                    // Teto do arquivo no disco: recusa barata, antes de abrir (zip bomb não precisa ser grande,
                    // por isso o teto de arquivo é só sanidade — quem segura o bomb é o limite por entrada).
                    if (new System.IO.FileInfo(path).Length > MaxArchiveBytes) return;
                    // System.Type qualificado: "Type" sozinho resolveria para a propriedade Executor.Type da classe base.
                    // O nome do assembly precisa ser COMPLETO: com o nome curto, Type.GetType devolve null e
                    // Assembly.Load falha (medido em 2026-09-18). ZipFile/ZipArchive não estão em System.dll, por isso
                    // a reflexão — assim nenhum dos dois .csproj precisa de referência nova.
                    System.Type zipFile = System.Type.GetType(
                        "System.IO.Compression.ZipFile, System.IO.Compression.FileSystem,"
                        + " Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")
                        ?? System.Type.GetType("System.IO.Compression.ZipFile, System.IO.Compression.FileSystem");
                    if (zipFile == null) return;
                    System.Reflection.MethodInfo openRead = zipFile.GetMethod("OpenRead", new[] { typeof(string) });
                    if (openRead == null) return;
                    _archive = openRead.Invoke(null, new object[] { path });
                    object entries = _archive.GetType().GetProperty("Entries").GetValue(_archive, null);
                    var map = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                    int seen = 0;
                    bool overflow = false;
                    foreach (object entry in (System.Collections.IEnumerable)entries)
                    {
                        if (++seen > MaxEntries) { overflow = true; break; }
                        if (_openEntry == null)
                        {
                            _openEntry = entry.GetType().GetMethod("Open", System.Type.EmptyTypes);
                            _entryLength = entry.GetType().GetProperty("Length");
                        }
                        string name = (string)entry.GetType().GetProperty("Name").GetValue(entry, null);
                        if (!string.IsNullOrEmpty(name) && name[0] == 'c' && name.EndsWith(".lua", StringComparison.OrdinalIgnoreCase))
                            map[name] = entry;
                    }
                    // Archive com contagem absurda é recusado por inteiro: não indexa nada.
                    _entries = (_openEntry != null && !overflow) ? map : null;
                }
                catch
                {
                    _archive = null;
                    _entries = null;
                }
            }

            /// <summary>Texto do script da carta, ou null se não houver script (aí o chamador usa o texto em inglês).</summary>
            public static string Read(int id, int alias)
            {
                lock (Gate)
                {
                    Prepare();
                    if (_entries == null) return null;
                    object entry;
                    if (!_entries.TryGetValue("c" + id + ".lua", out entry) && alias != 0)
                        _entries.TryGetValue("c" + alias + ".lua", out entry);
                    if (entry == null) return null;
                    try
                    {
                        // Camada 1: tamanho declarado no cabeçalho do ZIP. Barato, mas confiável só se o cabeçalho
                        // for honesto — por isso a camada 2 abaixo.
                        if (_entryLength != null)
                        {
                            long declared = (long)_entryLength.GetValue(entry, null);
                            if (declared > MaxEntryBytes) return null;
                        }
                        using (var stream = (System.IO.Stream)_openEntry.Invoke(entry, null))
                        using (var payload = new System.IO.MemoryStream())
                        {
                            // Camada 2: mede os bytes que a stream entrega de fato. Um ZIP com cabeçalho mentindo
                            // (declara pouco, entrega muito) estoura aqui e devolve null: o chamador cai no texto
                            // em inglês, que é o mesmo caminho de "sem script". Nunca estoura MaxEntryBytes+o
                            // tamanho do buffer por muito, então o tempo sob o Gate fica limitado.
                            byte[] buffer = new byte[8192];
                            int read;
                            int total = 0;
                            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                total += read;
                                if (total > MaxEntryBytes) return null;
                                payload.Write(buffer, 0, read);
                            }
                            // MemoryStream em vez de ler a stream direto: o teto garante que ReadToEnd é seguro.
                            payload.Position = 0;
                            using (var reader = new System.IO.StreamReader(payload))
                                return reader.ReadToEnd();
                        }
                    }
                    catch { return null; }
                }
            }

            /// <summary>
            /// O script registra este efeito para a PRÓPRIA carta? EFFECT_TYPE_SINGLE vale para ela mesma;
            /// EFFECT_TYPE_FIELD vale para um alcance de cartas (é o concedente, não o protegido). Sem esta
            /// distinção, uma carta como o Archfiend Zombie-Skull ("Zombie monsters you control cannot be
            /// destroyed by card effects") era lida como se ela própria fosse indestrutível.
            /// </summary>
            public static bool HasOwnEffect(string script, string effectCode)
            {
                if (string.IsNullOrEmpty(script)) return false;
                string marker = "SetCode(" + effectCode + ")";
                int pos = script.IndexOf(marker, StringComparison.Ordinal);
                while (pos >= 0)
                {
                    int start = Math.Max(0, pos - 500);
                    string preceding = script.Substring(start, pos - start);
                    bool singleType = preceding.LastIndexOf("EFFECT_TYPE_SINGLE", StringComparison.Ordinal)
                        > preceding.LastIndexOf("EFFECT_TYPE_FIELD", StringComparison.Ordinal);
                    if (singleType && RegisteredOnOwnCard(script, pos + marker.Length))
                        return true;
                    pos = script.IndexOf(marker, pos + marker.Length, StringComparison.Ordinal);
                }
                return false;
            }

            /// <summary>
            /// O efeito declarado aqui é registrado na própria carta? O dono do registro é quem vem antes do
            /// ":RegisterEffect(" — "c" é a carta, qualquer outra coisa ("tc", "sc"...) é outra carta, e
            /// "Duel.RegisterEffect" é efeito de campo. Sem isto, o nosso Crimson King era lido como indestrutível
            /// por efeito: o script dele registra EFFECT_INDESTRUCTABLE_EFFECT com EFFECT_TYPE_SINGLE, mas em "tc",
            /// o monstro que ELE protege (medido em 2026-09-18).
            /// </summary>
            private static bool RegisteredOnOwnCard(string script, int afterCode)
            {
                int reg = script.IndexOf("RegisterEffect(", afterCode, StringComparison.Ordinal);
                if (reg < 0) return false;
                // recua sobre ":" ou "." e lê o identificador do dono
                int i = reg - 1;
                if (i < 0 || (script[i] != ':' && script[i] != '.')) return false;
                int end = i;
                --i;
                while (i >= 0 && (char.IsLetterOrDigit(script[i]) || script[i] == '_')) --i;
                string owner = script.Substring(i + 1, end - i - 1);
                return owner == "c";
            }
        }

        // Atacar este monstro não serve: não sai em batalha, ou reflete o dano (reflexo só vale com a face para cima em ataque;
        // virado para baixo ele vira ao ser atacado). Negado (face para cima e desabilitado): pode atacar.
        private bool AvoidBattleTarget(ClientCard card)
        {
            BattleTrait traits = TraitsOf(card);
            if (traits == BattleTrait.None || (card.IsFaceup() && card.IsDisabled()))
                return false;
            // Não destruído em batalha: só é alvo ruim em DEFESA (nem dano nem destruição) ou virado para baixo (DEF
            // desconhecida). Em Posição de Ataque ele continua valendo, porque o dano de batalha acontece do mesmo jeito:
            // a diferença de ATK vai na vida do oponente mesmo sem o monstro ser destruído (jogador, 2026-09-17).
            // Quem decide se dá para passar é o emparelhamento, que exige ATK estritamente maior.
            if ((traits & BattleTrait.BattleIndestructible) != 0 && !card.IsAttack())
                return true;
            return (traits & BattleTrait.Reflect) != 0 && (card.IsAttack() || card.IsFacedown());
        }

        private List<ClientCard> BattleBlockers()
        {
            return Enemy.GetMonsters().Where(card => card != null && AvoidBattleTarget(card)).ToList();
        }

        // PURPOSE: sair da batalha sem atacar, só com a opção que o menu oferece.
        // O jogo recusa a ação e derruba o bot (MSG_RETRY, 2026-09-15: o RDA atacou, a Quetzacoatl devolveu o próprio RDA ao
        // Extra como custo e o ataque virou replay; nesse menu não havia Main Phase 2 nem End Phase). No replay forçado o jogo
        // exige uma declaração de ataque: escolhe o alvo menos ruim.
        private BattlePhaseAction SkipBattleAction(IList<ClientCard> attackers, IList<ClientCard> defenders, string reason)
        {
            BattlePhase battle = Duel.BattlePhase;
            if (battle != null && battle.CanMainPhaseTwo)
            {
                Report("battle", reason + ": going to Main Phase 2 without attacking");
                return new BattlePhaseAction(BattlePhaseAction.BattleAction.ToMainPhaseTwo);
            }
            if (battle != null && battle.CanEndPhase)
            {
                Report("battle", reason + ": ending the battle without attacking");
                return new BattlePhaseAction(BattlePhaseAction.BattleAction.ToEndPhase);
            }
            if (attackers.Count == 0)
                return base.OnBattle(attackers, defenders);
            if (defenders.Count == 0)
            {
                Report("battle", reason + ", but the game requires an attack: direct attack");
                return AI.Attack(attackers.OrderByDescending(card => card.Attack).First(), null);
            }
            // Alvo: primeiro um que não devolva dano nem sobreviva de graça. Só se não houver nenhum, um dos bloqueados.
            ClientCard target = defenders.Where(card => !AvoidBattleTarget(card)).OrderBy(RequiredPower).FirstOrDefault();
            bool forcedIntoBlocked = target == null;
            if (forcedIntoBlocked)
                target = defenders.OrderBy(RequiredPower).First();
            // Obrigado a bater num monstro que devolve dano igual ao ATK do atacante (Yubel), manda o MAIS FRACO: o dano que
            // volta é o ATK de quem ataca. Jogador, 2026-09-17: há carta no deck do Yubel que obriga o ataque, e nessa partida
            // o bot mandou o mais forte duas vezes e perdeu vida à toa. Com alvo livre, segue o mais forte.
            ClientCard attacker = forcedIntoBlocked
                ? attackers.OrderBy(card => card.Attack).First()
                : attackers.OrderByDescending(card => card.Attack).First();
            Report("battle", reason + ", but the game requires an attack: "
                + (forcedIntoBlocked ? "only a damage-reflecting target is left, attacking with the weakest " : "attacks ")
                + DescribeKnown(target));
            return AI.Attack(attacker, target);
        }

        // PURPOSE: com o combo pendente, ir para a Main Phase 2 para terminá-lo. O WindBot só chama isto quando o menu da
        // batalha oferece a Main Phase 2, então nunca gera ação recusada.
        private bool SkipBattlePhase()
        {
            if (Duel.Player != 0 || _comboDone || _plannerDisabledThisTurn || AbyssBattleWindow())
                return false;
            Report("battle", "combo not finished yet: going to Main Phase 2 without attacking");
            return true;
        }

        // Abyss no campo com 1 zona principal livre: única situação em que atacar antes de terminar o combo compensa (jogador).
        private bool AbyssBattleWindow()
        {
            return Bot.GetMonsters().Any(card => card != null && card.IsFaceup() && !card.IsDisabled() && CardCode(card) == CardId.HotRedDragonArchfiendAbyss)
                && MainMonsterZoneFree();
        }

        // Outra resposta aos bloqueios de batalha, antes de gastar o Hypernova: linha RDA + Gaia pronta ou Impermanence baixada.
        private bool HasOtherBattleBlockerAnswer()
        {
            bool rdaLine = GaiaRdaLineReady() && Bot.GetMonsters().Any(card => card != null && card.IsFaceup() && CardCode(card) == CardId.RedDragonArchfiend);
            bool impermanence = Bot.SpellZone.Any(card => card != null && card.IsFacedown() && CardCode(card) == CardId.InfiniteImpermanence
                && !infiniteImpermanenceNegatedColumns.Contains(card.Sequence));
            return rdaLine || impermanence;
        }

        public override void OnPosChange(ClientCard card, int previousPosition, int currentPosition)
        {
            RememberEnemyCard(card);
            base.OnPosChange(card, previousPosition, currentPosition);
        }

        // Poder que o atacante precisa superar: ATK se o alvo está em ataque, DEF se está em defesa.
        private static int RequiredPower(ClientCard defender)
        {
            return defender.IsAttack() ? defender.Attack : defender.Defense;
        }

        /// <summary>
        /// A Crimson Blade Dragon pode destruir este alvo pelo efeito dela, sem depender de ATK?
        ///
        /// O efeito dispara em EVENT_BATTLE_START — ANTES do cálculo de dano — e exige que o alvo da batalha esteja
        /// com a face para cima, seja nível 5 ou mais e seja do oponente (lido do script c3294539.lua em 2026-09-18).
        /// Como o monstro sai antes do cálculo, a Blade derruba monstro MAIS FORTE que ela sem sofrer nada, e é
        /// justamente para isso que ela serve na batalha (jogador).
        ///
        /// Vale nos dois sentidos: GetBattleTarget() funciona atacando ou sendo atacado. O lado "sendo atacado" já
        /// estava tratado na aceitação do gatilho; o que faltava era ESCOLHER o alvo no nosso turno, porque o
        /// emparelhamento exigia ATK maior e assim o jogo nunca chegava a oferecer o efeito.
        ///
        /// A destruição é por EFEITO: alvo que não é destruído por efeito, ou que não é afetado, fica de fora.
        /// </summary>
        private bool BladeCanDestroy(ClientCard attacker, ClientCard target)
        {
            if (attacker == null || target == null) return false;
            if (CardCode(attacker) != CardId.CrimsonBladeDragon) return false;
            if (!attacker.IsFaceup() || attacker.IsDisabled()) return false;
            if (!target.IsFaceup() || target.Level < 5) return false;
            return (TraitsOf(target) & (BattleTrait.EffectIndestructible | BattleTrait.ImmuneEffect)) == 0;
        }

        // =====================================================================================================
        // PARTE 3 - PLANEJADOR
        //
        // Porta para C# do planejador validado em handbook/planner/rda_planner.py (mesmas notas nos cenários
        // dos replays: Soul 324, Bone 328, Power Vice 302, Branded 01 295).
        //
        //   RdaCards     dados das cartas (nível, atributo, tipo, tuner...) e grupos usados pelas regras
        //   RdaState     estado do nosso lado (mão, deck, GY, banidas, Extra, campo, efeitos usados...)
        //   RdaRules     todas as ações legais a partir de um estado, seguindo o texto das cartas
        //   RdaEvaluator nota do campo final (bosses, recursão do Hypernova, handtraps, armadilhas)
        //   RdaPlanner   busca em feixe: mantém os N estados mais promissores a cada passo
        //
        // Simplificações: interação do oponente não é simulada; gatilhos resolvem um por vez.
        // =====================================================================================================

        /// <summary>Dados de uma carta usados pelo modelo.</summary>
        private sealed class RdaCardInfo
        {
            public readonly int Id;
            public readonly string Name;
            public readonly bool IsMonster, IsSpell, IsTrap;
            public readonly int Level;
            public readonly CardAttribute Attribute;
            public readonly CardRace Race;
            public readonly bool Tuner, Synchro, Resonator, MentionsRda, Summonable;
            public readonly bool Foreign;   // não é do nosso deck (token, monstro deixado pelo oponente): nunca vai ao nosso GY

            public RdaCardInfo(int id, string name, char kind, int level = 0, CardAttribute attribute = 0, CardRace race = 0,
                bool tuner = false, bool synchro = false, bool resonator = false, bool mentionsRda = false, bool summonable = true, bool foreign = false)
            {
                Id = id;
                Name = name;
                IsMonster = kind == 'M';
                IsSpell = kind == 'S';
                IsTrap = kind == 'T';
                Level = level;
                Attribute = attribute;
                Race = race;
                Tuner = tuner;
                Synchro = synchro;
                Resonator = resonator;
                MentionsRda = mentionsRda;
                Foreign = foreign;
                Summonable = summonable;
            }
        }

        /// <summary>
        /// Valores vivos de uma carta de fora durante a duel (nível/atributo/raça/tuner podem mudar por efeito).
        /// Construído na leitura do estado e nunca mutado depois (as cópias do MemberwiseClone do RdaState compartilham
        /// a referência só para leitura). Fica em RdaState.ForeignDynamic, nunca no cache estático RdaCards.All:
        /// é uma tabela por-duel, então um efeito numa duel não vaza para a próxima.
        /// </summary>
        private sealed class ForeignLive
        {
            public readonly int Level;
            public readonly CardAttribute Attribute;
            public readonly CardRace Race;
            public readonly bool Tuner;

            public ForeignLive(int level, CardAttribute attribute, CardRace race, bool tuner)
            {
                Level = level;
                Attribute = attribute;
                Race = race;
                Tuner = tuner;
            }
        }

        /// <summary>Lista de cartas conhecidas pelo modelo e grupos usados pelas regras.</summary>
        private static class RdaCards
        {
            // Main Deck
            public const int PowerVice = 19434243;
            public const int FiendPiece = 56838842;
            public const int StoneSweeper = 72323266;
            public const int Bone = 25784595;
            public const int Darkness = 83445539;
            public const int Soul = 62991792;
            public const int Crimson = 34761841;
            public const int Vision = 98396890;
            public const int Chain = 13764881;
            public const int Synkron = 77360173;
            public const int Lubellion = 32731036;
            public const int Magnamhut = 33854624;
            public const int Harmonia = 70088809;
            public const int Ash = 14558127;
            public const int MaxxC = 23434538;
            public const int Foolish = 81439173;
            public const int CrimsonCall = 99398682;
            public const int ResonatorCall = 23008320;
            public const int Gaia = 98173209;
            public const int RedZone = 50056656;
            public const int Etude = 45675980;
            public const int KingsResonance = 17269895;
            public const int RdaChain = 92936365;
            public const int Impermanence = 10045474;
            public const int Dominus = 40366668;

            // Extra Deck
            public const int Quetzacoatl = 29053657;
            public const int Hypernova = 30698243;
            public const int Supernova = 99585851;
            public const int BurningSoul = 65541656;
            public const int StormBane = 94641726;
            public const int DisPater = 27572350;
            public const int Abyss = 9753964;
            public const int Rda = 70902743;
            public const int King = 67809530;
            public const int Scarred = 87451661;
            public const int Blade = 3294539;
            public const int Zalen = 4891376;
            public const int RedRising = 66141736;

            // Marcador de carta baixada virada para baixo (só ocupa zona de magia/armadilha).
            public const int FaceDownCard = 1;

            // Concorrente: o plano em segundo plano lê a tabela enquanto a leitura do estado pode registrar cartas de fora.
            public static readonly System.Collections.Concurrent.ConcurrentDictionary<int, RdaCardInfo> All =
                new System.Collections.Concurrent.ConcurrentDictionary<int, RdaCardInfo>();
            public static readonly HashSet<int> Level4Fiends = new HashSet<int>();
            public static readonly HashSet<int> FiendTuners = new HashSet<int>();
            public static readonly HashSet<int> Resonators = new HashSet<int>();
            public static readonly HashSet<int> MentionsMain = new HashSet<int>();
            public static readonly HashSet<int> MentionsSpellTrap = new HashSet<int>();
            // Scarred se chama "Red Dragon Archfiend" no campo e no GY.
            public static readonly HashSet<int> RdaLike = new HashSet<int> { Rda, Scarred };
            // Synchros que ativam o efeito de GY do Fiend Piece Golem quando ele é material.
            public static readonly HashSet<int> FiendPieceRdaSynchros = new HashSet<int> { Rda, Scarred, King, BurningSoul };
            // Synchros que seguram a mesa (jogador, 2026-09-17): todo o Extra MENOS o Red Rising, que não tem utilidade
            // nenhuma além de extender o combo. Usado para contar as 4 peças mínimas de uma mesa cheia.
            public static readonly HashSet<int> UsefulSynchros = new HashSet<int>
                { Quetzacoatl, Hypernova, Supernova, BurningSoul, StormBane, DisPater, Abyss, Rda, King, Scarred, Blade, Zalen };
            // Não-Synchros que aparecem na mesa: numa mesa cheia o jogador aceita no máximo 1 deles.
            public static readonly HashSet<int> FieldNonSynchros = new HashSet<int> { Lubellion, Magnamhut, Harmonia };

            static RdaCards()
            {
                Add(new RdaCardInfo(PowerVice, "Power Vice Dragon", 'M', 5, CardAttribute.Dark, CardRace.Dragon, mentionsRda: true));
                Add(new RdaCardInfo(FiendPiece, "Fiend Piece Golem", 'M', 5, CardAttribute.Dark, CardRace.Fiend, mentionsRda: true));
                Add(new RdaCardInfo(StoneSweeper, "Earthbound Prisoner Stone Sweeper", 'M', 5, CardAttribute.Dark, CardRace.Fiend));
                Add(new RdaCardInfo(Bone, "Bone Archfiend", 'M', 4, CardAttribute.Dark, CardRace.Fiend));
                Add(new RdaCardInfo(Darkness, "Darkness Resonator", 'M', 3, CardAttribute.Dark, CardRace.Fiend, tuner: true, resonator: true, mentionsRda: true));
                Add(new RdaCardInfo(Soul, "Soul Resonator", 'M', 3, CardAttribute.Fire, CardRace.Fiend, tuner: true, resonator: true, mentionsRda: true));
                Add(new RdaCardInfo(Crimson, "Crimson Resonator", 'M', 2, CardAttribute.Dark, CardRace.Fiend, tuner: true, resonator: true));
                Add(new RdaCardInfo(Vision, "Vision Resonator", 'M', 2, CardAttribute.Dark, CardRace.Fiend, tuner: true, resonator: true, mentionsRda: true));
                Add(new RdaCardInfo(Chain, "Chain Resonator", 'M', 1, CardAttribute.Light, CardRace.Fiend, tuner: true, resonator: true));
                Add(new RdaCardInfo(Synkron, "Synkron Resonator", 'M', 1, CardAttribute.Dark, CardRace.Fiend, tuner: true, resonator: true));
                Add(new RdaCardInfo(Lubellion, "The Bystial Lubellion", 'M', 8, CardAttribute.Light, CardRace.Dragon, summonable: false));
                Add(new RdaCardInfo(Magnamhut, "Bystial Magnamhut", 'M', 6, CardAttribute.Dark, CardRace.Dragon));
                Add(new RdaCardInfo(Harmonia, "Fydraulis Harmonia", 'M', 7, CardAttribute.Dark, CardRace.Dragon, tuner: true));
                Add(new RdaCardInfo(Ash, "Ash Blossom & Joyous Spring", 'M', 3, CardAttribute.Fire, CardRace.Zombie, tuner: true));
                Add(new RdaCardInfo(MaxxC, "Maxx \"C\"", 'M', 2, CardAttribute.Earth, CardRace.Insect));
                Add(new RdaCardInfo(Foolish, "Foolish Burial", 'S'));
                Add(new RdaCardInfo(CrimsonCall, "Crimson Call", 'S', mentionsRda: true));
                Add(new RdaCardInfo(ResonatorCall, "Resonator Call", 'S'));
                Add(new RdaCardInfo(Gaia, "Crimson Gaia", 'S', mentionsRda: true));
                Add(new RdaCardInfo(RedZone, "Red Zone", 'T', mentionsRda: true));
                Add(new RdaCardInfo(Etude, "Etude of the Branded", 'T'));
                Add(new RdaCardInfo(KingsResonance, "King's Resonance", 'T', mentionsRda: true));
                Add(new RdaCardInfo(RdaChain, "Red Dragon Archfiend's Chain", 'T', mentionsRda: true));
                Add(new RdaCardInfo(Impermanence, "Infinite Impermanence", 'T'));
                Add(new RdaCardInfo(Dominus, "Dominus Impulse", 'T'));
                Add(Extra(Quetzacoatl, "Crimson Dragon Quetzacoatl", 12));
                Add(Extra(Hypernova, "Red Hypernova Dragon", 12));
                Add(Extra(Supernova, "Red Supernova Dragon", 12));
                Add(Extra(BurningSoul, "Red Nova Dragon - Burning Soul", 12, mentionsRda: true));
                Add(Extra(StormBane, "Storm-Bane Dragon Destorbim", 11));
                Add(Extra(DisPater, "Bystial Dis Pater", 10));
                Add(Extra(Abyss, "Hot Red Dragon Archfiend Abyss", 9));
                Add(Extra(Rda, "Red Dragon Archfiend", 8));
                Add(Extra(King, "The Crimson King", 8, mentionsRda: true));
                Add(Extra(Scarred, "Scarred Dragon Archfiend", 8));
                Add(Extra(Blade, "Crimson Blade Dragon", 7));
                Add(Extra(Zalen, "Zalen the Shackled Dragon", 7, tuner: true));
                Add(Extra(RedRising, "Red Rising Dragon", 6));

                foreach (RdaCardInfo card in All.Values)
                {
                    if (card.IsMonster && !card.Synchro && card.Race == CardRace.Fiend && card.Level <= 4)
                        Level4Fiends.Add(card.Id);
                    if (card.IsMonster && !card.Synchro && card.Race == CardRace.Fiend && card.Tuner)
                        FiendTuners.Add(card.Id);
                    if (card.Resonator)
                        Resonators.Add(card.Id);
                    if (card.MentionsRda && !card.Synchro)
                        MentionsMain.Add(card.Id);
                    if (card.MentionsRda && (card.IsSpell || card.IsTrap))
                        MentionsSpellTrap.Add(card.Id);
                }
            }

            private static void Add(RdaCardInfo card)
            {
                All[card.Id] = card;
                _lookup = new Dictionary<int, RdaCardInfo>(All);
            }

            // Cópia fixa para leitura (Get é chamado milhões de vezes por busca). Dictionary comum é seguro para leitura em
            // paralelo enquanto ninguém o altera: registrar carta nova troca a referência por uma cópia nova (escrita rara,
            // só na linha principal), então as buscas em andamento continuam lendo a cópia antiga sem conflito.
            private static volatile Dictionary<int, RdaCardInfo> _lookup = new Dictionary<int, RdaCardInfo>();

            // Checar, inserir e publicar a nova cópia do _lookup acontecem sob um único lock: sem ele, dois registros
            // simultâneos podiam ler a mesma tabela e o segundo republished uma cópia perdendo o primeiro. Add() do
            // construtor estático fica fora do lock (o type initializer do CLR já serializa aquelas chamadas).
            private static readonly object SyncLock = new object();

            /// <summary>
            /// Monstro fora do nosso deck no nosso campo (ex.: Primal Being Token do Nibiru). Registrado na leitura do
            /// estado, antes do planejamento (a busca em paralelo só lê). A base (nível/atributo/raça/tuner/synchro)
            /// vem do cards.cdb — mesma fonte de Name(), valores de impressão imutáveis; os valores vivos da duel
            /// ficam em RdaState.ForeignDynamic, nunca aqui. Retorna true se foi registrado agora.
            /// </summary>
            public static bool RegisterForeign(int id, string name)
            {
                lock (SyncLock)
                {
                    if (All.ContainsKey(id))
                        return false;
                    int level = 0;
                    CardAttribute attribute = 0;
                    CardRace race = 0;
                    bool tuner = false, synchro = false;
                    try
                    {
                        YGOSharp.OCGWrapper.NamedCard data = YGOSharp.OCGWrapper.NamedCard.Get(id);
                        if (data != null)
                        {
                            level = data.Level & 0xFF;
                            attribute = (CardAttribute)data.Attribute;
                            race = (CardRace)data.Race;
                            tuner = data.HasType(CardType.Tuner);
                            synchro = data.HasType(CardType.Synchro);
                        }
                    }
                    catch (Exception)
                    {
                        // Sem cards.cdb carregado (teste offline): fica a base vazia (nível 0), como antes.
                    }
                    All[id] = new RdaCardInfo(id, name, 'M', level, attribute, race, tuner: tuner, synchro: synchro, summonable: false, foreign: true);
                    _lookup = new Dictionary<int, RdaCardInfo>(All);
                    return true;
                }
            }

            private static RdaCardInfo Extra(int id, string name, int level, bool tuner = false, bool mentionsRda = false)
            {
                return new RdaCardInfo(id, name, 'M', level, CardAttribute.Dark, CardRace.Dragon, tuner: tuner, synchro: true,
                    mentionsRda: mentionsRda, summonable: false);
            }

            public static RdaCardInfo Get(int id)
            {
                RdaCardInfo card;
                return _lookup.TryGetValue(id, out card) ? card : null;
            }

            // Leituras por estado de uma carta possivelmente de fora: consultam o overlay vivo da duel (ForeignDynamic)
            // e voltam ao modelo (base do cards.cdb, imutável) quando não há overlay. As cartas nossas nunca têm overlay,
            // então o resultado é o mesmo de Get(...).<prop> — só a carta de fora ganha o valor vivo desta duel.
            public static int Level(RdaState state, int id)
            {
                ForeignLive live;
                if (state.ForeignDynamic != null && state.ForeignDynamic.TryGetValue(id, out live))
                    return live.Level;
                RdaCardInfo card = Get(id);
                return card != null ? card.Level : 0;
            }

            public static CardAttribute Attribute(RdaState state, int id)
            {
                ForeignLive live;
                if (state.ForeignDynamic != null && state.ForeignDynamic.TryGetValue(id, out live))
                    return live.Attribute;
                RdaCardInfo card = Get(id);
                return card != null ? card.Attribute : 0;
            }

            public static CardRace Race(RdaState state, int id)
            {
                ForeignLive live;
                if (state.ForeignDynamic != null && state.ForeignDynamic.TryGetValue(id, out live))
                    return live.Race;
                RdaCardInfo card = Get(id);
                return card != null ? card.Race : 0;
            }

            public static bool Tuner(RdaState state, int id)
            {
                ForeignLive live;
                if (state.ForeignDynamic != null && state.ForeignDynamic.TryGetValue(id, out live))
                    return live.Tuner;
                RdaCardInfo card = Get(id);
                return card != null && card.Tuner;
            }

            public static string Name(int id)
            {
                RdaCardInfo card = Get(id);
                if (card != null)
                    return card.Name;
                // Carta fora do modelo (ex.: do oponente): nome do cards.cdb que acompanha o WindBot.
                try
                {
                    YGOSharp.OCGWrapper.NamedCard data = YGOSharp.OCGWrapper.NamedCard.Get(id);
                    if (data != null && !string.IsNullOrEmpty(data.Name))
                        return data.Name;
                }
                catch (Exception)
                {
                    // Sem cards.cdb carregado (teste offline): fica o número.
                }
                return "#" + id;
            }
        }

        /// <summary>
        /// Chaves dos efeitos "uma vez por turno" e dos gatilhos. A mesma chave marca o uso no estado
        /// e identifica o gatilho pendente.
        /// </summary>
        private enum RdaKey
        {
            None = 0,
            SoulSearch, PowerViceSS, PowerViceSearch, DarknessSS, DarknessExtraNs, DarknessLevel,
            CrimsonSS, CrimsonEffect, VisionProc, VisionSearch, SynkronProc, SynkronAdd, ChainSummon,
            BoneSS, BoneLevel, FiendPieceSS, FiendPieceLevel, FiendPieceRevive, StoneSweeperSearch,
            LubellionSS, LubellionSearch, LubellionPlace, MagnamhutSS, MagnamhutEndPhase, KingSearch,
            BladeTake, RedRisingRevive, QuetzacoatlRevive, DisPaterEffect, ScarredSummon, CrimsonCall,
            GaiaSearch, BurningSoulAdd, StoneSweeperSS, RedZoneRevive,
            // Storm-Bane ③ (c94641726.lua, e4): EVENT_TO_GRAVE sem condição de local anterior, uma vez por turno,
            // invoca 1 Dragão banido e exige zona principal livre. Novo no fim do enum para não mexer nos valores
            // já existentes.
            StormBaneGraveSummon
        }

        /// <summary>Monstro no campo codificado num long: id, nível atual, se está na EMZ e se está negado.</summary>
        private static class RdaField
        {
            public static long Encode(int id, int level, bool emz, bool negated)
            {
                return ((long)id << 12) | ((long)level << 2) | (emz ? 2L : 0L) | (negated ? 1L : 0L);
            }

            public static int Id(long monster) { return (int)(monster >> 12); }
            public static int Level(long monster) { return (int)((monster >> 2) & 0x3FF); }
            public static bool Emz(long monster) { return (monster & 2L) != 0; }
            public static bool Negated(long monster) { return (monster & 1L) != 0; }

            public static long WithLevel(long monster, int level)
            {
                return Encode(Id(monster), level, Emz(monster), Negated(monster));
            }
        }

        /// <summary>Operações em arrays ordenados usados como multiconjuntos imutáveis.</summary>
        internal static class RdaArray
        {
            public static int[] Add(int[] items, int value)
            {
                int[] result = new int[items.Length + 1];
                int index = 0;
                while (index < items.Length && items[index] <= value)
                {
                    result[index] = items[index];
                    index++;
                }
                result[index] = value;
                Array.Copy(items, index, result, index + 1, items.Length - index);
                return result;
            }

            public static long[] Add(long[] items, long value)
            {
                long[] result = new long[items.Length + 1];
                int index = 0;
                while (index < items.Length && items[index] <= value)
                {
                    result[index] = items[index];
                    index++;
                }
                result[index] = value;
                Array.Copy(items, index, result, index + 1, items.Length - index);
                return result;
            }

            public static int[] Remove(int[] items, int value)
            {
                int position = Array.IndexOf(items, value);
                if (position < 0)
                    return items;
                int[] result = new int[items.Length - 1];
                Array.Copy(items, 0, result, 0, position);
                Array.Copy(items, position + 1, result, position, items.Length - position - 1);
                return result;
            }

            public static long[] Remove(long[] items, long value)
            {
                int position = Array.IndexOf(items, value);
                if (position < 0)
                    return items;
                long[] result = new long[items.Length - 1];
                Array.Copy(items, 0, result, 0, position);
                Array.Copy(items, position + 1, result, position, items.Length - position - 1);
                return result;
            }

            public static int[] Append(int[] items, int value)
            {
                int[] result = new int[items.Length + 1];
                Array.Copy(items, result, items.Length);
                result[items.Length] = value;
                return result;
            }

            public static bool Same(int[] a, int[] b)
            {
                if (a.Length != b.Length) return false;
                for (int i = 0; i < a.Length; ++i)
                    if (a[i] != b[i]) return false;
                return true;
            }

            public static bool Same(long[] a, long[] b)
            {
                if (a.Length != b.Length) return false;
                for (int i = 0; i < a.Length; ++i)
                    if (a[i] != b[i]) return false;
                return true;
            }

            public static IEnumerable<int> Distinct(int[] sorted, Func<int, bool> condition)
            {
                for (int i = 0; i < sorted.Length; ++i)
                {
                    if (i > 0 && sorted[i] == sorted[i - 1]) continue;
                    if (condition(sorted[i])) yield return sorted[i];
                }
            }
        }

        /// <summary>
        /// Estado do nosso lado do duelo. Imutável por convenção: toda alteração cria uma cópia com arrays novos.
        /// Deck e Extra não entram na comparação (são consequência do resto).
        /// </summary>
        private sealed class RdaState : IEquatable<RdaState>
        {
            public const int FlagMagnamhutEndPhase = 1;
            public const int FlagRdaSynchro = 2;
            public const int FlagNoDeckAdd = 4;   // Droll & Lock Bird resolveu: nada sai do Deck para a mão
            public const int FlagMaxxC = 8;       // Maxx "C" do oponente: cada Invocação-Especial (evento) dá 1 carta
            public const int FlagFuwalos = 16;    // Mulcharmy Fuwalos: só Invocações-Especiais do Deck/Extra dão carta
            public const int FlagBaitDone = 32;   // uma isca já resolveu sem resposta neste turno: não gastar outra
            // Temos no campo um RDA imune aos efeitos do oponente (veio do Crimson King ② ou da Crimson Gaia): ele não
            // pode virar custo. O modelo não distingue as cópias, então a flag protege qualquer RDA nosso no campo.
            public const int FlagProtectedRda = 64;

            public int[] Hand = new int[0];
            public int[] Deck = new int[0];
            public int[] Grave = new int[0];
            public int[] Banished = new int[0];
            public int[] Extra = new int[0];
            public int[] Spells = new int[0];
            public long[] Field = new long[0];
            public int[] Used = new int[0];
            public int[] Pending = new int[0];
            public int NormalSummon;
            public int ExtraNormalSummon;
            // Raça/atributo forçados no campo por um efeito contínuo do jogo (ex.: Black Rose Garden vira tudo
            // em Planta). 0 = nada forçado. Lidos ao vivo do ClientCard no ReadState.
            public CardRace ForcedRace;
            public CardAttribute ForcedAttribute;
            public bool NoSpecial;
            public int Flags;
            // Monstros invocados no turno antes deste estado (só dado de entrada para o risco de Nibiru; fora do Equals).
            public int SummonedThisTurn;
            // Efeitos críticos nossos que resolveram sem negação no turno (entrada da escolha por resistência; fora do Equals).
            public int UnansweredCriticals;
            // Lado do oponente (entrada do plano, fora do Equals: o GY dele mudando durante o nosso combo, por exemplo uma
            // handtrap, não pode forçar replanejar). Condições "no campo" que valem para os dois lados e alvos da Magnamhut
            // e da Dis Pater (auditoria carta por carta, 2026-09-14).
            public bool EnemySynchro;                 // Chain Resonator (gatilho) e Synkron Resonator (procedimento)
            public bool EnemyDarkLevel5;              // Vision Resonator (procedimento)
            public bool FieldZoneCard;                // Stone Sweeper (procedimento): carta em qualquer Field Zone
            // Cartas da mão do oponente que já vimos reveladas. Fora do Equals/GetHashCode de propósito: não muda
            // durante a nossa busca, então compará-la em cada estado só gastaria tempo.
            public int[] EnemyHand = new int[0];
            public int[] EnemyGrave = new int[0];     // monstros LIGHT/DARK no GY do oponente (alvos da Magnamhut)
            public int[] EnemyBanished = new int[0];  // monstros LIGHT/DARK banidos do oponente (alvos da Dis Pater ①)
            public int EnemyBanishedOther;            // outras cartas banidas do oponente (negação da Dis Pater ②)
            public int EnemyGravePriority;            // melhor alvo da Magnamhut no GY do oponente (efeito no GY, Extra, nível)
            // Valores vivos das cartas de fora nesta leitura (ForeignLive por id): consultados pelas regras via
            // RdaCards.Level/Attribute/Race/Tuner em vez do modelo estático. Null quando não há nenhuma. Fora do
            // Equals/GetHashCode (mesmo motivo do lado inimigo: dentro de uma busca todos os estados derivam da mesma
            // leitura) e nunca mutado depois de construído — as cópias do MemberwiseClone compartilham a referência
            // apenas para leitura.
            public Dictionary<int, ForeignLive> ForeignDynamic;
            // Entradas do turno (fora do Equals): Red Zone com a face para cima e sem negação (efeito ② disponível);
            // cópias de Red Rising Dragon no GY que não foram para lá neste turno (efeito do GY disponível).
            public bool RedZoneReady;
            public int RedRisingGraveReady;
            // Turno com batalha e 1+ monstro do oponente: RDA + Crimson Gaia limpam a mesa dele (entrada, fora do Equals).
            public bool BattleWipeReady;
            // Crimson Gaia no campo porém negada (Impermanence na coluna ou efeito desligado): a linha do RDA não funciona,
            // então ela não vale como montada (jogador, 2026-09-17). Entrada do turno, fora do Equals.
            public bool GaiaNegated;
            // Nosso turno a partir do 2º (entrada, fora do Equals). A Crimson Blade é só extensor: ajuda muito a montar o
            // combo no turno 1, mas depois pode ser trocada por peça melhor (jogador, 2026-09-16).
            public bool SecondTurnOrLater;

            private int _hash;
            private bool _hasHash;

            public RdaState Copy()
            {
                RdaState copy = (RdaState)MemberwiseClone();
                copy._hasHash = false;
                return copy;
            }

            public bool Uses(RdaKey key) { return Array.IndexOf(Used, (int)key) >= 0; }

            public RdaState Mark(RdaKey key)
            {
                RdaState copy = Copy();
                if (!Uses(key))
                    copy.Used = RdaArray.Add(Used, (int)key);
                return copy;
            }

            public bool HasFlag(int flag) { return (Flags & flag) != 0; }

            public int MainZonesFree
            {
                get
                {
                    int used = 0;
                    foreach (long monster in Field)
                        if (!RdaField.Emz(monster)) used++;
                    return 5 - used;
                }
            }

            public int EmzFree
            {
                get
                {
                    foreach (long monster in Field)
                        if (RdaField.Emz(monster)) return 0;
                    return 1;
                }
            }

            // Zonas vazias em coluna negada pela Infinite Impermanence (baixada) neste turno: magia/armadilha ali é negada.
            public int BlockedSpellZones;
            public int SpellZonesFree { get { return Math.Max(0, 5 - Spells.Length - BlockedSpellZones); } }

            public bool HasSynchro
            {
                get
                {
                    foreach (long monster in Field)
                        if (RdaCards.Get(RdaField.Id(monster)).Synchro) return true;
                    return false;
                }
            }

            public bool Controls(int id)
            {
                foreach (long monster in Field)
                    if (RdaField.Id(monster) == id) return true;
                return false;
            }

            public bool Equals(RdaState other)
            {
                if (other == null) return false;
                if (ReferenceEquals(this, other)) return true;
                return NormalSummon == other.NormalSummon && ExtraNormalSummon == other.ExtraNormalSummon
                    && NoSpecial == other.NoSpecial && Flags == other.Flags
                    && RdaArray.Same(Hand, other.Hand) && RdaArray.Same(Grave, other.Grave)
                    && RdaArray.Same(Banished, other.Banished) && RdaArray.Same(Field, other.Field)
                    && RdaArray.Same(Spells, other.Spells) && RdaArray.Same(Used, other.Used)
                    && RdaArray.Same(Pending, other.Pending);
            }

            public override bool Equals(object obj) { return Equals(obj as RdaState); }

            public override int GetHashCode()
            {
                if (_hasHash) return _hash;
                unchecked
                {
                    int hash = 17;
                    foreach (int value in Hand) hash = hash * 31 + value;
                    hash = hash * 31 + 1;
                    foreach (int value in Grave) hash = hash * 31 + value;
                    hash = hash * 31 + 2;
                    foreach (int value in Banished) hash = hash * 31 + value;
                    hash = hash * 31 + 3;
                    foreach (long value in Field) hash = hash * 31 + value.GetHashCode();
                    hash = hash * 31 + 4;
                    foreach (int value in Spells) hash = hash * 31 + value;
                    foreach (int value in Used) hash = hash * 31 + value;
                    hash = hash * 31 + 5;
                    foreach (int value in Pending) hash = hash * 31 + value;
                    hash = hash * 31 + NormalSummon * 7 + ExtraNormalSummon * 11 + (NoSpecial ? 13 : 0) + Flags * 17;
                    _hash = hash;
                }
                _hasHash = true;
                return _hash;
            }

            public string Describe()
            {
                return "hand=[" + string.Join(", ", Hand.Select(RdaCards.Name)) + "] field=[" +
                       string.Join(", ", Field.Select(m => RdaCards.Name(RdaField.Id(m)) + " nv" + RdaField.Level(m))) +
                       "] GY=[" + string.Join(", ", Grave.Select(RdaCards.Name)) + "]";
            }
        }

        /// <summary>Tipo de ação planejada. Cada tipo corresponde a um ExecutorType do WindBot.</summary>
        private enum PlanKind
        {
            NormalSummon,       // ExecutorType.Summon
            ExtraNormalSummon,  // ExecutorType.Summon + opção "Summoned by the effect of Darkness Resonator"
            SpecialProc,        // ExecutorType.SpSummon (procedimento: Vision, Synkron, Lubellion)
            Synchro,            // ExecutorType.SpSummon de um monstro do Extra, com materiais pré-selecionados
            Activate,           // ExecutorType.Activate de um efeito que não é gatilho
            TriggerAccept,      // gatilho aceito (ExecutorType.Activate no prompt de chain)
            TriggerDecline      // gatilho recusado
        }

        /// <summary>Carta a escolher num prompt de seleção (id + onde ela está).</summary>
        private struct RdaPick
        {
            public readonly int Id;
            public readonly CardLocation Location;

            public RdaPick(int id, CardLocation location)
            {
                Id = id;
                Location = location;
            }
        }

        /// <summary>Uma ação do plano com tudo que o executor precisa para realizá-la no jogo.</summary>
        private sealed class PlanAction
        {
            public PlanKind Kind;
            public int CardId;                 // carta que realiza a ação (quem ativa ou é invocada)
            public CardLocation From;          // onde ela está no momento da ação
            public RdaKey Effect;              // efeito usado (para marcar "uma vez por turno")
            public int LevelDelta;             // Bone +1/-1, Fiend Piece -1/-2
            public bool Summon;                // Crimson Blade: Invocar (efeitos negados) em vez de adicionar à mão
            public long[] Materials;           // materiais de synchro (codificados como RdaField)
            public readonly List<RdaPick> Picks = new List<RdaPick>(); // escolhas nos prompts, em ordem
            // Texto do passo, montado só quando alguém lê: a busca gera ~700 mil ações por plano e só ~40 viram passos.
            private string _text;
            public Func<string> TextFactory;
            public string Text
            {
                get
                {
                    if (_text == null && TextFactory != null)
                    {
                        _text = TextFactory();
                        TextFactory = null;
                    }
                    return _text;
                }
                set
                {
                    _text = value;
                    TextFactory = null;
                }
            }

            public override string ToString() { return Text; }
        }

        /// <summary>Par ação + estado resultante.</summary>
        private struct RdaMove
        {
            public readonly PlanAction Action;
            public readonly RdaState State;

            public RdaMove(PlanAction action, RdaState state)
            {
                Action = action;
                State = state;
            }
        }

        /// <summary>Regras: gera as ações legais a partir de um estado.</summary>
        private static class RdaRules
        {
            // Gatilhos que só adicionam recursos: recusar nunca é melhor.
            private static readonly HashSet<RdaKey> AlwaysTake = new HashSet<RdaKey>
            {
                RdaKey.SoulSearch, RdaKey.PowerViceSearch, RdaKey.DarknessExtraNs, RdaKey.MagnamhutEndPhase,
                RdaKey.VisionSearch, RdaKey.SynkronAdd, RdaKey.KingSearch
            };

            // Carta e local de origem de cada gatilho (para o executor reconhecer o prompt).
            public static readonly Dictionary<RdaKey, RdaPick> TriggerSource = new Dictionary<RdaKey, RdaPick>
            {
                { RdaKey.SoulSearch, new RdaPick(RdaCards.Soul, CardLocation.MonsterZone) },
                { RdaKey.PowerViceSearch, new RdaPick(RdaCards.PowerVice, CardLocation.MonsterZone) },
                { RdaKey.DarknessExtraNs, new RdaPick(RdaCards.Darkness, CardLocation.MonsterZone) },
                { RdaKey.FiendPieceLevel, new RdaPick(RdaCards.FiendPiece, CardLocation.MonsterZone) },
                { RdaKey.FiendPieceRevive, new RdaPick(RdaCards.FiendPiece, CardLocation.Grave) },
                { RdaKey.MagnamhutEndPhase, new RdaPick(RdaCards.Magnamhut, CardLocation.MonsterZone) },
                { RdaKey.BurningSoulAdd, new RdaPick(RdaCards.BurningSoul, CardLocation.MonsterZone) },
                { RdaKey.ChainSummon, new RdaPick(RdaCards.Chain, CardLocation.MonsterZone) },
                { RdaKey.VisionSearch, new RdaPick(RdaCards.Vision, CardLocation.Grave) },
                { RdaKey.SynkronAdd, new RdaPick(RdaCards.Synkron, CardLocation.Grave) },
                { RdaKey.KingSearch, new RdaPick(RdaCards.King, CardLocation.MonsterZone) },
                { RdaKey.BladeTake, new RdaPick(RdaCards.Blade, CardLocation.MonsterZone) },
                { RdaKey.RedRisingRevive, new RdaPick(RdaCards.RedRising, CardLocation.MonsterZone) },
                { RdaKey.StormBaneGraveSummon, new RdaPick(RdaCards.StormBane, CardLocation.Grave) },
                { RdaKey.QuetzacoatlRevive, new RdaPick(RdaCards.Quetzacoatl, CardLocation.MonsterZone) },
                { RdaKey.ScarredSummon, new RdaPick(RdaCards.Scarred, CardLocation.Grave) },
            };

            // ------------------------------------------------------------------ movimentos básicos

            private static RdaState Place(RdaState state, int id, int level, bool fromExtra, bool negated = false)
            {
                if (state.NoSpecial)
                    return null;
                bool emz;
                // Hypernova/Supernova ficam numa zona principal quando há espaço (jogador): são banidos e voltam, e a zona extra
                // fica para outro Synchro. Os outros Synchros usam a zona extra primeiro (deixa as principais livres).
                bool nova = id == RdaCards.Hypernova || id == RdaCards.Supernova;
                if (fromExtra && state.EmzFree > 0 && !(nova && state.MainZonesFree > 0))
                    emz = true;
                else if (state.MainZonesFree > 0)
                    emz = false;
                else
                    return null;
                RdaState result = state.Copy();
                result.Field = RdaArray.Add(state.Field, RdaField.Encode(id, level, emz, negated));
                return result;
            }

            private static RdaState Place(RdaState state, int id)
            {
                return Place(state, id, RdaCards.Get(id).Level, false);
            }

            private static RdaState AddPending(RdaState state, List<RdaKey> triggers)
            {
                if (triggers.Count == 0)
                    return state;
                RdaState result = state.Copy();
                foreach (RdaKey trigger in triggers)
                    result.Pending = RdaArray.Append(result.Pending, (int)trigger);
                return result;
            }

            // Um gatilho pendente só (roda para toda jogada da busca: sem lista por chamada).
            private static RdaState AddPending(RdaState state, RdaKey trigger)
            {
                RdaState result = state.Copy();
                result.Pending = RdaArray.Append(state.Pending, (int)trigger);
                return result;
            }

            private static RdaState OnSummoned(RdaState state, int id, bool special = true, bool normal = false, bool synchro = false)
            {
                // Cada carta dispara no máximo um destes gatilhos (ids diferentes), então basta uma variável.
                RdaKey trigger = RdaKey.None;
                if (id == RdaCards.Soul) trigger = RdaKey.SoulSearch;
                else if (special && id == RdaCards.PowerVice) trigger = RdaKey.PowerViceSearch;
                else if (special && id == RdaCards.Darkness) trigger = RdaKey.DarknessExtraNs;
                else if (special && id == RdaCards.FiendPiece) trigger = RdaKey.FiendPieceLevel;
                else if (special && id == RdaCards.Magnamhut) trigger = RdaKey.MagnamhutEndPhase;
                else if (special && id == RdaCards.BurningSoul) trigger = RdaKey.BurningSoulAdd;
                else if (normal && id == RdaCards.Chain && (state.HasSynchro || state.EnemySynchro)) trigger = RdaKey.ChainSummon;
                else if (synchro)
                {
                    if (id == RdaCards.King) trigger = RdaKey.KingSearch;
                    else if (id == RdaCards.Blade) trigger = RdaKey.BladeTake;
                    else if (id == RdaCards.RedRising) trigger = RdaKey.RedRisingRevive;
                    else if (id == RdaCards.Quetzacoatl) trigger = RdaKey.QuetzacoatlRevive;
                }
                return trigger == RdaKey.None ? state : AddPending(state, trigger);
            }

            private static RdaState SendToGrave(RdaState state, int id, bool fromField = false, int materialFor = 0)
            {
                // Token ou monstro do oponente usado como material/custo: sai do campo e não vem para o nosso GY.
                RdaCardInfo info = C(id);
                if (info != null && info.Foreign)
                    return state.Copy();
                RdaState result = state.Copy();
                result.Grave = RdaArray.Add(state.Grave, id);
                // No máximo um gatilho (ids diferentes): anexa direto, sem lista e sem segunda cópia do estado.
                RdaKey trigger = RdaKey.None;
                if (id == RdaCards.Vision) trigger = RdaKey.VisionSearch;
                else if (id == RdaCards.Synkron && fromField) trigger = RdaKey.SynkronAdd;
                else if (id == RdaCards.Scarred && fromField) trigger = RdaKey.ScarredSummon;
                else if (id == RdaCards.FiendPiece && RdaCards.FiendPieceRdaSynchros.Contains(materialFor)) trigger = RdaKey.FiendPieceRevive;
                // Storm-Bane ③ dispara indo ao GY de QUALQUER lugar: como material, destruído no campo ou mandado
                // direto do Extra pela Harmonia. O script não tem condição de local anterior.
                else if (id == RdaCards.StormBane) trigger = RdaKey.StormBaneGraveSummon;
                if (trigger != RdaKey.None)
                    result.Pending = RdaArray.Append(state.Pending, (int)trigger);
                return result;
            }

            private static RdaState RemoveField(RdaState state, long monster)
            {
                RdaState result = state.Copy();
                result.Field = RdaArray.Remove(state.Field, monster);
                // A proteção do RDA (efeito ② do Crimson King / Crimson Gaia) pertence àquela cópia no campo: se ela sai,
                // a flag tem que sair junto, senão um plano que usa o RDA protegido como material e depois o revive ainda
                // ganharia o bônus de "RDA protegido" sem ter a proteção. Quem só troca o nível do monstro (Bone, Darkness,
                // Fiend Piece) remove e readiciona: esses pontos devolvem a flag.
                if (RdaField.Id(monster) == RdaCards.Rda)
                    result.Flags &= ~RdaState.FlagProtectedRda;
                return result;
            }

            private static RdaState DeckToHand(RdaState state, int id)
            {
                // Droll & Lock Bird: a adição não acontece (movimento inválido, filtrado em Successors).
                // Mandar do Deck ao GY e invocar do Deck continuam valendo.
                if (state.HasFlag(RdaState.FlagNoDeckAdd))
                    return null;
                RdaState result = state.Copy();
                result.Deck = RdaArray.Remove(state.Deck, id);
                result.Hand = RdaArray.Add(state.Hand, id);
                return result;
            }

            private static RdaState GraveToHand(RdaState state, int id)
            {
                RdaState result = state.Copy();
                result.Grave = RdaArray.Remove(state.Grave, id);
                result.Hand = RdaArray.Add(state.Hand, id);
                return result;
            }

            private static PlanAction Action(PlanKind kind, int cardId, CardLocation from, RdaKey effect, string text, params RdaPick[] picks)
            {
                var action = new PlanAction { Kind = kind, CardId = cardId, From = from, Effect = effect, Text = text };
                action.Picks.AddRange(picks);
                return action;
            }

            private static string N(int id) { return RdaCards.Name(id); }

            // Tuner Fiend no campo AGORA: se há troca global de raça e ela não é Fiend, não existe nenhum.
            private static bool FiendTunerOnField(RdaState state)
            {
                if (state.ForcedRace != 0 && state.ForcedRace != CardRace.Fiend)
                    return false;
                return state.Field.Any(m => RdaCards.FiendTuners.Contains(RdaField.Id(m)));
            }

            private static RdaCardInfo C(int id) { return RdaCards.Get(id); }

            // ------------------------------------------------------------------ gatilhos

            /// <summary>Formas de resolver o gatilho. 'state' já está sem o gatilho na fila.</summary>
            private static IEnumerable<RdaMove> ResolveTrigger(RdaState state, RdaKey trigger)
            {
                RdaPick source = TriggerSource.ContainsKey(trigger) ? TriggerSource[trigger] : new RdaPick(0, CardLocation.MonsterZone);
                Func<string, RdaPick[], PlanAction> accept = (text, picks) =>
                    Action(PlanKind.TriggerAccept, source.Id, source.Location, trigger, text, picks);

                switch (trigger)
                {
                    case RdaKey.SoulSearch:
                        if (state.Uses(trigger)) yield break;
                        foreach (int id in RdaArray.Distinct(state.Deck, n => RdaCards.Level4Fiends.Contains(n) && n != RdaCards.Soul))
                            yield return new RdaMove(accept("Soul searches " + N(id), new[] { new RdaPick(id, CardLocation.Deck) }), DeckToHand(state.Mark(trigger), id));
                        break;

                    case RdaKey.PowerViceSearch:
                        if (state.Uses(trigger)) yield break;
                        foreach (int id in RdaArray.Distinct(state.Deck, n => RdaCards.Resonators.Contains(n)))
                            yield return new RdaMove(accept("Power Vice searches " + N(id), new[] { new RdaPick(id, CardLocation.Deck) }), DeckToHand(state.Mark(trigger), id));
                        break;

                    case RdaKey.DarknessExtraNs:
                        if (state.Uses(trigger)) yield break;
                        {
                            RdaState result = state.Mark(trigger);
                            result.ExtraNormalSummon = 1;
                            yield return new RdaMove(accept("Darkness grants an extra Resonator Normal Summon", new RdaPick[0]), result);
                        }
                        break;

                    case RdaKey.FiendPieceLevel:
                        if (state.Uses(trigger)) yield break;
                        foreach (long monster in state.Field)
                        {
                            if (RdaField.Id(monster) != RdaCards.FiendPiece || RdaField.Negated(monster)) continue;
                            for (int reduce = 1; reduce <= 2; ++reduce)
                            {
                                if (RdaField.Level(monster) - reduce < 1) continue;
                                RdaState result = RemoveField(state.Mark(trigger), monster);
                                result.Field = RdaArray.Add(result.Field, RdaField.WithLevel(monster, RdaField.Level(monster) - reduce));
                                PlanAction action = accept("Fiend Piece reduces the Level by " + reduce, new RdaPick[0]);
                                action.LevelDelta = -reduce;
                                yield return new RdaMove(action, result);
                            }
                            break;
                        }
                        break;

                    case RdaKey.MagnamhutEndPhase:
                        if (state.Uses(trigger)) yield break;
                        {
                            RdaState result = state.Mark(trigger);
                            result.Flags |= RdaState.FlagMagnamhutEndPhase;
                            yield return new RdaMove(accept("Magnamhut schedules the End Phase search", new RdaPick[0]), result);
                        }
                        break;

                    case RdaKey.BurningSoulAdd:
                        if (state.Uses(trigger) || !state.HasFlag(RdaState.FlagRdaSynchro)) yield break;
                        // Só vai para a mão o que não é Synchro. Para não inundar a busca (o gatilho aparece muito
                        // desde que o Synchro do Scarred conta), testa só os Resonators (podem estender o combo)
                        // e a melhor carta que não é Resonator (a que mais vale na mão).
                        {
                            int bestOther = 0;
                            double bestValue = double.MinValue;
                            foreach (int id in RdaArray.Distinct(state.Grave, n => !C(n).Synchro))
                            {
                                if (C(id).Resonator)
                                {
                                    yield return new RdaMove(accept("Burning Soul adds " + N(id) + " from the GY", new[] { new RdaPick(id, CardLocation.Grave) }), GraveToHand(state.Mark(trigger), id));
                                    continue;
                                }
                                double value = RdaEvaluator.HandCardValue(id);
                                if (value > bestValue) { bestValue = value; bestOther = id; }
                            }
                            if (bestOther != 0)
                                yield return new RdaMove(accept("Burning Soul adds " + N(bestOther) + " from the GY", new[] { new RdaPick(bestOther, CardLocation.Grave) }), GraveToHand(state.Mark(trigger), bestOther));
                        }
                        break;

                    case RdaKey.ChainSummon:
                        foreach (int id in RdaArray.Distinct(state.Deck, n => RdaCards.Resonators.Contains(n) && n != RdaCards.Chain))
                        {
                            RdaState removed = state.Copy();
                            removed.Deck = RdaArray.Remove(state.Deck, id);
                            RdaState placed = Place(removed, id);
                            if (placed != null)
                                yield return new RdaMove(accept("Chain summons " + N(id) + " from the Deck", new[] { new RdaPick(id, CardLocation.Deck) }), OnSummoned(placed, id));
                        }
                        break;

                    case RdaKey.VisionSearch:
                        if (state.Uses(trigger)) yield break;
                        foreach (int id in RdaArray.Distinct(state.Deck, n => RdaCards.MentionsSpellTrap.Contains(n)))
                            yield return new RdaMove(accept("Vision (GY) searches " + N(id), new[] { new RdaPick(id, CardLocation.Deck) }), DeckToHand(state.Mark(trigger), id));
                        break;

                    case RdaKey.SynkronAdd:
                        foreach (int id in RdaArray.Distinct(state.Grave, n => RdaCards.Resonators.Contains(n) && n != RdaCards.Synkron))
                            yield return new RdaMove(accept("Synkron (GY) returns " + N(id) + " to the hand", new[] { new RdaPick(id, CardLocation.Grave) }), GraveToHand(state, id));
                        break;

                    case RdaKey.KingSearch:
                        if (state.Uses(trigger)) yield break;
                        foreach (int id in RdaArray.Distinct(state.Deck, n => RdaCards.MentionsMain.Contains(n)))
                            yield return new RdaMove(accept("King searches " + N(id), new[] { new RdaPick(id, CardLocation.Deck) }), DeckToHand(state.Mark(trigger), id));
                        break;

                    case RdaKey.BladeTake:
                        if (state.Uses(trigger)) yield break;
                        {
                            RdaState marked = state.Mark(trigger);
                            if (Array.IndexOf(state.Deck, RdaCards.Lubellion) >= 0)
                                yield return new RdaMove(accept("Crimson Blade searches The Bystial Lubellion (Deck)", new[] { new RdaPick(RdaCards.Lubellion, CardLocation.Deck) }), DeckToHand(marked, RdaCards.Lubellion));
                            if (Array.IndexOf(state.Grave, RdaCards.Lubellion) >= 0)
                                yield return new RdaMove(accept("Crimson Blade searches The Bystial Lubellion (GY)", new[] { new RdaPick(RdaCards.Lubellion, CardLocation.Grave) }), GraveToHand(marked, RdaCards.Lubellion));
                            // Texto da Blade: também pode Invocar o monstro (efeitos negados). Lubellion negada
                            // no campo serve de nível 8 para o Dis Pater, mas não coloca o Etude.
                            foreach (CardLocation origin in new[] { CardLocation.Deck, CardLocation.Grave })
                            {
                                int[] pile = origin == CardLocation.Deck ? marked.Deck : marked.Grave;
                                if (Array.IndexOf(pile, RdaCards.Lubellion) < 0) continue;
                                RdaState removed = marked.Copy();
                                if (origin == CardLocation.Deck) removed.Deck = RdaArray.Remove(marked.Deck, RdaCards.Lubellion);
                                else removed.Grave = RdaArray.Remove(marked.Grave, RdaCards.Lubellion);
                                RdaState placed = Place(removed, RdaCards.Lubellion, C(RdaCards.Lubellion).Level, false, true);
                                if (placed != null)
                                {
                                    PlanAction summonAction = accept("Crimson Blade summons The Bystial Lubellion (" + (origin == CardLocation.Deck ? "Deck" : "GY") + ", negated)",
                                        new[] { new RdaPick(RdaCards.Lubellion, origin) });
                                    summonAction.Summon = true;
                                    yield return new RdaMove(summonAction, placed);
                                }
                            }
                            foreach (int id in RdaArray.Distinct(state.Grave, n => C(n).Synchro && C(n).Level >= 8))
                            {
                                RdaState removed = marked.Copy();
                                removed.Grave = RdaArray.Remove(marked.Grave, id);
                                RdaState placed = Place(removed, id, C(id).Level, false, true);
                                if (placed != null)
                                {
                                    PlanAction summonAction = accept("Crimson Blade summons " + N(id) + " from the GY (negated)", new[] { new RdaPick(id, CardLocation.Grave) });
                                    summonAction.Summon = true;
                                    yield return new RdaMove(summonAction, placed);
                                }
                            }
                        }
                        break;

                    case RdaKey.RedRisingRevive:
                        // O texto do Red Rising não tem "uma vez por turno".
                        foreach (int id in RdaArray.Distinct(state.Grave, n => RdaCards.Resonators.Contains(n)))
                        {
                            RdaState removed = state.Copy();
                            removed.Grave = RdaArray.Remove(state.Grave, id);
                            RdaState placed = Place(removed, id);
                            if (placed != null)
                                yield return new RdaMove(accept("Red Rising revives " + N(id), new[] { new RdaPick(id, CardLocation.Grave) }), OnSummoned(placed, id));
                        }
                        break;

                    case RdaKey.QuetzacoatlRevive:
                        if (state.Uses(trigger)) yield break;
                        {
                            // O revive do Quetzacoatl exige DRAGÃO e alcança só o GY (script c29053656.lua:
                            // "c:IsRace(RACE_DRAGON) and c:IsType(TYPE_SYNCHRO)", LOCATION_GRAVE). Antes o modelo
                            // aceitava qualquer Synchro; hoje todos os nossos são Dragões, então isto alinha o modelo
                            // ao jogo sem mudar comportamento — e evita um erro silencioso se entrar um não-Dragão.
                            //
                            // Antes o revive era UM lance guloso: enchia as zonas com os Dragões de maior valor de mesa.
                            // Isso tirava do planejador a escolha de quem volta (jogador, 2026-09-22) — às vezes vale
                            // deixar zona livre para o que vem depois, ou trazer um monstro mais barato que a rota usa
                            // como material. Agora cada subconjunto vira um lance e a busca decide. A lista de candidatos
                            // é limitada porque o número de lances dobra a cada candidato a mais.
                            const int QuetzacoatlReviveCandidates = 5;
                            // Ordem por valor sensível ao contexto: com a linha da Gaia viva o RDA vale 45 e entra na
                            // lista dos candidatos. Com o valor fixo de 8 ele ficava em penúltimo e, num GY com cinco
                            // Synchros melhores, nem chegava a ser oferecido (jogador, 2026-09-22).
                            var candidates = RdaArray.Distinct(state.Grave, n => C(n).Synchro && C(n).Race == CardRace.Dragon)
                                .OrderByDescending(n => RdaEvaluator.BoardValue(n, state)).Take(QuetzacoatlReviveCandidates).ToList();
                            int freeZones = state.MainZonesFree;
                            // O script não deixa escolher QUANTOS: "g=tg:Select(tp,ft,ft,nil)" com ft = zonas principais
                            // livres, e se houver menos alvos que zonas ele invoca todos sem perguntar. Então a única
                            // escolha real é QUAIS, e só quando sobram alvos. Um lance por combinação de tamanho exato.
                            int pickCount = Math.Min(freeZones, candidates.Count);
                            if (freeZones <= 0 || candidates.Count == 0) yield break;
                            for (int mask = 0; mask < (1 << candidates.Count); ++mask)
                            {
                                int selectedCount = 0;
                                for (int i = 0; i < candidates.Count; ++i)
                                    if ((mask & (1 << i)) != 0) selectedCount++;
                                if (selectedCount != pickCount) continue;
                                RdaState result = state.Mark(trigger);
                                var picks = new List<RdaPick>();
                                bool placedAll = true;
                                for (int i = 0; i < candidates.Count; ++i)
                                {
                                    if ((mask & (1 << i)) == 0) continue;
                                    int id = candidates[i];
                                    RdaState removed = result.Copy();
                                    removed.Grave = RdaArray.Remove(result.Grave, id);
                                    RdaState placed = Place(removed, id, C(id).Level, false);
                                    if (placed == null) { placedAll = false; break; }
                                    result = placed;
                                    picks.Add(new RdaPick(id, CardLocation.Grave));
                                }
                                if (!placedAll) continue;
                                result = result.Copy();
                                result.NoSpecial = true;
                                yield return new RdaMove(accept("Quetzacoatl revives " + (picks.Count == 0 ? "nothing" : string.Join(", ", picks.Select(p => N(p.Id)))), picks.ToArray()), result);
                            }
                        }
                        break;

                    case RdaKey.ScarredSummon:
                        if (state.Uses(trigger) || Array.IndexOf(state.Extra, RdaCards.Rda) < 0) yield break;
                        {
                            RdaState removed = state.Mark(trigger);
                            removed.Extra = RdaArray.Remove(state.Extra, RdaCards.Rda);
                            RdaState placed = Place(removed, RdaCards.Rda, 8, true);
                            if (placed != null)
                            {
                                placed.Flags |= RdaState.FlagRdaSynchro;
                                yield return new RdaMove(accept("Scarred summons Red Dragon Archfiend from the Extra Deck", new[] { new RdaPick(RdaCards.Rda, CardLocation.Extra) }), placed);
                            }
                        }
                        break;

                    case RdaKey.StormBaneGraveSummon:
                        // Um lance por Dragão banido. Quem decide qual volta (ou se vale a pena) é a nota da mesa,
                        // não uma regra fixa: com um alvo ruim o plano que recusa, ou que resolve o Quetzacoatl antes
                        // e deixa este sem zona, pontua mais (jogador, 2026-09-22).
                        if (state.Uses(trigger)) yield break;
                        foreach (int id in RdaArray.Distinct(state.Banished, n => C(n) != null && C(n).Race == CardRace.Dragon))
                        {
                            RdaState removed = state.Mark(trigger);
                            removed.Banished = RdaArray.Remove(removed.Banished, id);
                            RdaState placed = Place(removed, id, C(id).Level, false);
                            if (placed != null)
                                yield return new RdaMove(accept("Storm-Bane (GY) summons " + N(id) + " from the banished zone",
                                    new[] { new RdaPick(id, CardLocation.Removed) }), OnSummoned(placed, id));
                        }
                        break;

                    case RdaKey.FiendPieceRevive:
                        if (state.Uses(trigger)) yield break;
                        foreach (int id in RdaArray.Distinct(state.Grave, n => RdaCards.Level4Fiends.Contains(n)))
                        {
                            RdaState removed = state.Mark(trigger);
                            removed.Grave = RdaArray.Remove(removed.Grave, id);
                            RdaState placed = Place(removed, id);
                            if (placed != null)
                                yield return new RdaMove(accept("Fiend Piece (GY) revives " + N(id), new[] { new RdaPick(id, CardLocation.Grave) }), OnSummoned(placed, id));
                        }
                        break;
                }
            }

            // ------------------------------------------------------------------ synchro

            /// <summary>
            /// Receita do Synchro. Raça e atributo vêm da tabela do deck, MAS o jogo pode trocá-los no campo: o
            /// Black Rose Garden deixa "all face-up monsters become Plant monsters" e o nosso Soul Resonator deixa
            /// de ser Fiend, quebrando o Red Rising. Jogador, 2026-09-22 (log 203229): o planejador montou esse
            /// Synchro, o menu não ofereceu e o replanejamento devolveu 0 passos.
            /// forcedRace/forcedAttribute vêm do estado, lidos AO VIVO do ClientCard, e valem para todos os monstros
            /// do campo — que é como essas cartas funcionam.
            /// </summary>
            private static bool SynchroRequirement(RdaState state, int target, List<long> tuners, List<long> nonTuners,
                CardRace forcedRace = 0, CardAttribute forcedAttribute = 0)
            {
                Func<long, RdaCardInfo> card = m => C(RdaField.Id(m));
                Func<long, CardRace> raceOf = m => forcedRace != 0 ? forcedRace : RdaCards.Race(state, RdaField.Id(m));
                Func<long, CardAttribute> attrOf = m => forcedAttribute != 0 ? forcedAttribute : RdaCards.Attribute(state, RdaField.Id(m));
                switch (target)
                {
                    case RdaCards.Rda:
                    case RdaCards.StormBane:
                    case RdaCards.Quetzacoatl:
                        return tuners.Count == 1 && nonTuners.Count >= 1;
                    case RdaCards.King:
                    case RdaCards.Scarred:
                        return tuners.Count == 1 && nonTuners.Count >= 1 && nonTuners.All(m => attrOf(m) == CardAttribute.Dark);
                    case RdaCards.Blade:
                        return tuners.Count == 1 && card(tuners[0]).Resonator && nonTuners.Count >= 1;
                    case RdaCards.RedRising:
                        return tuners.Count == 1 && raceOf(tuners[0]) == CardRace.Fiend && nonTuners.Count >= 1;
                    case RdaCards.DisPater:
                        return tuners.Count == 1 && nonTuners.Count >= 1 && nonTuners.All(m => raceOf(m) == CardRace.Dragon);
                    case RdaCards.Abyss:
                        return tuners.Count == 1 && nonTuners.Count == 1 && card(nonTuners[0]).Synchro
                            && attrOf(nonTuners[0]) == CardAttribute.Dark && raceOf(nonTuners[0]) == CardRace.Dragon;
                    case RdaCards.Zalen:
                        return tuners.Count == 1 && nonTuners.Count >= 1 && nonTuners.All(m => card(m).Synchro);
                    case RdaCards.Hypernova:
                        return tuners.Count == 4 && nonTuners.Count >= 1 && nonTuners.All(m => card(m).Synchro);
                    case RdaCards.Supernova:
                        return tuners.Count == 3 && nonTuners.Count >= 1 && nonTuners.All(m => card(m).Synchro);
                    case RdaCards.BurningSoul:
                        return tuners.Count == 2 && nonTuners.Count == 1;
                }
                return false;
            }

            /// <summary>Synchros possíveis com os monstros do campo (todas as combinações cuja soma de nível fecha).</summary>
            private static IEnumerable<KeyValuePair<int, long[]>> SynchroOptions(RdaState state)
            {
                // Mesmas combinações e na mesma ordem de antes (medido: o bloco de Synchro era 56% do tempo de gerar jogadas).
                // A soma de níveis e o tamanho de cada combinação são calculados uma vez por estado (antes: uma vez para cada
                // monstro do Extra) e os monstros do Extra cujo nível nenhuma combinação alcança são pulados.
                // A repetição (materiais iguais em zonas diferentes) é detectada por uma chave numérica (FNV-1a).
                if (state.NoSpecial) yield break;
                long[] field = state.Field;
                int count = field.Length;
                if (count < 2) yield break;
                int[] extra = state.Extra;
                int masks = 1 << count;
                var sums = new int[masks];
                var sizes = new int[masks];
                long reachable = 0;
                for (int mask = 1; mask < masks; ++mask)
                {
                    int lowBit = 0;
                    while ((mask & (1 << lowBit)) == 0) lowBit++;
                    int previous = mask & ~(1 << lowBit);
                    sums[mask] = sums[previous] + RdaField.Level(field[lowBit]);
                    sizes[mask] = sizes[previous] + 1;
                    if (sizes[mask] >= 2 && sums[mask] < 64)
                        reachable |= 1L << sums[mask];
                }
                HashSet<ulong> seen = null;
                var tuners = new List<long>(count);
                var nonTuners = new List<long>(count);
                for (int e = 0; e < extra.Length; ++e)
                {
                    int target = extra[e];
                    if (e > 0 && extra[e - 1] == target) continue;
                    int level = C(target).Level;
                    if (level >= 64 || (reachable & (1L << level)) == 0) continue;
                    for (int mask = 1; mask < masks; ++mask)
                    {
                        if (sizes[mask] < 2 || sums[mask] != level) continue;
                        int size = sizes[mask];

                        ulong key = 14695981039346656037UL;
                        int flexibleCount = 0;
                        unchecked
                        {
                            key = (key ^ (ulong)target) * 1099511628211UL;
                            for (int i = 0; i < count; ++i)
                            {
                                if ((mask & (1 << i)) == 0) continue;
                                key = (key ^ (ulong)field[i]) * 1099511628211UL;
                                if (RdaField.Id(field[i]) == RdaCards.Zalen) flexibleCount++;
                            }
                        }
                        if (seen == null) seen = new HashSet<ulong>();
                        if (seen.Contains(key)) continue;

                        // Zalen pode ser tratado como não-tuner quando usado como material.
                        bool valid = false;
                        for (int roles = 0; roles < (1 << flexibleCount) && !valid; ++roles)
                        {
                            tuners.Clear();
                            nonTuners.Clear();
                            int flexibleIndex = 0;
                            for (int i = 0; i < count; ++i)
                            {
                                if ((mask & (1 << i)) == 0) continue;
                                long material = field[i];
                                bool isTuner = RdaCards.Tuner(state, RdaField.Id(material));
                                if (RdaField.Id(material) == RdaCards.Zalen)
                                {
                                    isTuner = (roles & (1 << flexibleIndex)) != 0;
                                    flexibleIndex++;
                                }
                                (isTuner ? tuners : nonTuners).Add(material);
                            }
                            valid = SynchroRequirement(state, target, tuners, nonTuners, state.ForcedRace, state.ForcedAttribute);
                        }
                        if (!valid) continue;
                        seen.Add(key);
                        var materials = new long[size];
                        for (int i = 0, k = 0; i < count; ++i)
                            if ((mask & (1 << i)) != 0) materials[k++] = field[i];
                        yield return new KeyValuePair<int, long[]>(target, materials);
                    }
                }
            }

            /// <summary>
            /// Peça do campo que NUNCA vira custo nem material, por decisão do jogador (2026-09-22).
            ///
            ///   Burning Soul  o combo inteiro existe para chegar nele, e o avaliador o tratava como moeda pequena
            ///                 (valor de campo 20, o segundo mais barato). Nos logs de 20 e 22/09 ele foi entregue
            ///                 três vezes — duas à Lubellion, uma ao Bone — e as mesas terminaram em camada 2 com
            ///                 nota 306 e 317, e camada 1 com 370. É regra dura, não preço: o jogador pediu
            ///                 explicitamente que o planejador não tenha a opção.
            ///   RDA imune     veio do Crimson King ② ou da Crimson Gaia. Ali ele tem proteção contra destruição por
            ///                 efeito do oponente e ATK maior, então é a peça que sobrevive à interrupção.
            ///
            /// O RDA COMUM continua disponível: o jogador quis que ele siga decidido pelo planejador, porque em
            /// campo sem o Soul Resonator no GY ele destrói os nossos monstros na End Phase.
            /// </summary>
            /// <summary>
            /// Cartas de interação forte na mão: traps do deck e handtraps. São as que respondem ao turno do oponente,
            /// então só viram custo quando não há alternativa nenhuma.
            /// </summary>
            private static bool IsStrongInteraction(int id)
            {
                return id == RdaCards.KingsResonance || id == RdaCards.RdaChain || id == RdaCards.RedZone
                    || id == RdaCards.Ash || id == RdaCards.Impermanence || id == RdaCards.Dominus
                    || id == RdaCards.MaxxC || id == RdaCards.Harmonia;
            }

            private static bool NeverBecomesCost(RdaState state, long monster)
            {
                int id = RdaField.Id(monster);
                if (id == RdaCards.BurningSoul) return true;
                return id == RdaCards.Rda && state.HasFlag(RdaState.FlagProtectedRda);
            }

            // Monstros do campo sem repetição, na mesma ordem de Distinct(): o array do campo é ordenado, então os repetidos são
            // vizinhos (evita criar um HashSet a cada estado).
            private static IEnumerable<long> DistinctField(long[] field)
            {
                for (int i = 0; i < field.Length; ++i)
                {
                    if (i > 0 && field[i] == field[i - 1]) continue;
                    yield return field[i];
                }
            }

            private static bool ScarredCountsAsRda = true; // static para teste offline por reflexão

            // Teste offline (por reflexão): tempo gasto em cada bloco de RawSuccessors. Desligado no jogo (uma checagem booleana).
            public static bool ProfileSections = false; // ligado só pelos testes offline (reflexão)
            public static readonly long[] SectionTicks = new long[32];

            private static void ProfileMark(ref long tick, int section)
            {
                if (!ProfileSections)
                    return;
                long now = Stopwatch.GetTimestamp();
                System.Threading.Interlocked.Add(ref SectionTicks[section], now - tick);
                tick = now;
            }

            private static RdaState DoSynchro(RdaState state, int target, long[] materials)
            {
                RdaState result = state;
                foreach (long material in materials)
                    result = RemoveField(result, material);
                foreach (long material in materials)
                    result = SendToGrave(result, RdaField.Id(material), true, target);
                RdaState removed = result.Copy();
                removed.Extra = RdaArray.Remove(result.Extra, target);
                result = Place(removed, target, C(target).Level, true);
                if (result == null)
                    return null;
                // Scarred vira "Red Dragon Archfiend" no campo, então a Invocação-Synchro dele também libera o Burning Soul.
                if (target == RdaCards.Rda || (ScarredCountsAsRda && target == RdaCards.Scarred))
                    result.Flags |= RdaState.FlagRdaSynchro;
                return OnSummoned(result, target, synchro: true);
            }

            // ------------------------------------------------------------------ ações

            public static List<RdaMove> Successors(RdaState state)
            {
                List<RdaMove> result = RawSuccessors(state);
                result.RemoveAll(move => move.State == null);
                return result;
            }

            private static List<RdaMove> RawSuccessors(RdaState state)
            {
                var moves = new List<RdaMove>();
                long profileTick = ProfileSections ? Stopwatch.GetTimestamp() : 0;

                // Com gatilho pendente, a única decisão possível é sobre a fila de gatilhos.
                // Antes só o Pending[0] era resolvido, numa ordem fixa. Como gatilhos nossos simultâneos têm a ordem
                // escolhida por nós (a cadeia resolve de trás para frente), a ORDEM é uma decisão de jogo e agora cada
                // gatilho da fila gera seu próprio ramo. Jogador, 2026-09-22: resolver o Storm-Bane ③ antes do revive
                // da Quetzacoatl tira uma zona dela e traz um Dragão do banimento; resolver depois deixa o Storm-Bane
                // sem zona. As duas mesas são diferentes, então a nota separa as duas em vez de uma regra fixa.
                // Custo: ramifica só onde há mais de um gatilho pendente — hoje isso só acontece com o Storm-Bane.
                if (state.Pending.Length > 0)
                {
                    var jaVistos = new List<int>();
                    for (int slot = 0; slot < state.Pending.Length; ++slot)
                    {
                        if (jaVistos.Contains(state.Pending[slot])) continue;   // gatilhos iguais dão o mesmo ramo
                        jaVistos.Add(state.Pending[slot]);
                        RdaKey trigger = (RdaKey)state.Pending[slot];
                        RdaState rest = state.Copy();
                        var remaining = new int[state.Pending.Length - 1];
                        Array.Copy(state.Pending, 0, remaining, 0, slot);
                        Array.Copy(state.Pending, slot + 1, remaining, slot, state.Pending.Length - slot - 1);
                        rest.Pending = remaining;
                        var options = new List<RdaMove>();
                        foreach (RdaMove option in ResolveTrigger(rest, trigger))
                        {
                            if (option.State != null)
                                options.Add(option);
                        }
                        if (!AlwaysTake.Contains(trigger) || options.Count == 0)
                        {
                            RdaPick source = TriggerSource.ContainsKey(trigger) ? TriggerSource[trigger] : new RdaPick(0, CardLocation.MonsterZone);
                            moves.Add(new RdaMove(Action(PlanKind.TriggerDecline, source.Id, source.Location, trigger, "declines " + trigger), rest));
                        }
                        moves.AddRange(options);
                    }
                    ProfileMark(ref profileTick, 31);
                    return moves;
                }

                ProfileMark(ref profileTick, 0);
                // ---- Invocação-Normal (nível 4 ou menor)
                foreach (int id in RdaArray.Distinct(state.Hand, n => C(n).IsMonster && !C(n).Synchro && C(n).Summonable && C(n).Level <= 4))
                {
                    if (state.NormalSummon > 0)
                        AddNormalSummon(moves, state, id, false);
                    if (state.ExtraNormalSummon > 0 && C(id).Resonator)
                        AddNormalSummon(moves, state, id, true);
                }

                ProfileMark(ref profileTick, 1);
                // ---- Invocações-Especiais pela mão
                if (Array.IndexOf(state.Hand, RdaCards.PowerVice) >= 0 && !state.Uses(RdaKey.PowerViceSS)
                    && state.Field.All(m => C(RdaField.Id(m)).Synchro && C(RdaField.Id(m)).Attribute == CardAttribute.Dark))
                    AddHandSummon(moves, state, RdaCards.PowerVice, RdaKey.PowerViceSS, PlanKind.Activate, "Power Vice summons itself from the hand");

                if (Array.IndexOf(state.Hand, RdaCards.Vision) >= 0 && !state.Uses(RdaKey.VisionProc)
                    && (state.EnemyDarkLevel5 || state.Field.Any(m => RdaField.Level(m) >= 5 && C(RdaField.Id(m)).Attribute == CardAttribute.Dark)))
                    AddHandSummon(moves, state, RdaCards.Vision, RdaKey.VisionProc, PlanKind.SpecialProc, "Vision summons itself from the hand");

                if (Array.IndexOf(state.Hand, RdaCards.Synkron) >= 0 && !state.Uses(RdaKey.SynkronProc) && (state.HasSynchro || state.EnemySynchro))
                    AddHandSummon(moves, state, RdaCards.Synkron, RdaKey.SynkronProc, PlanKind.SpecialProc, "Synkron summons itself from the hand");

                if (Array.IndexOf(state.Hand, RdaCards.Crimson) >= 0 && !state.Uses(RdaKey.CrimsonSS) && state.Field.Length == 0)
                    AddHandSummon(moves, state, RdaCards.Crimson, RdaKey.CrimsonSS, PlanKind.Activate, "Crimson Resonator summons itself from the hand");

                if (Array.IndexOf(state.Hand, RdaCards.Darkness) >= 0 && !state.Uses(RdaKey.DarknessSS) && Array.IndexOf(state.Extra, RdaCards.Rda) >= 0)
                    AddHandSummon(moves, state, RdaCards.Darkness, RdaKey.DarknessSS, PlanKind.Activate, "Darkness reveals RDA and summons itself", new RdaPick(RdaCards.Rda, CardLocation.Extra));

                // O Fiend Piece exige Tuner FIEND NO CAMPO, e a troca global de raça alcança o campo. Esta
                // checagem olhava só a lista de ids e ignorava o ForcedRace: no duelo 042750 de 2026-09-25 o
                // Black Rose Garden virou tudo em Planta, o plano foi montado com o Fiend Piece de qualquer jeito
                // e desabou no passo 4 — depois de o passo 3 já ter baixado o nível do Bone, que é irreversível.
                // Bone 3 + Vision 2 = 5, e não existe Synchro de nível 5 no nosso Extra: mesa perdida.
                // As buscas na mão e no Deck (StoneSweeperSearch, BoneLevel) não entram aqui, porque a troca só
                // vale para carta virada para cima no campo.
                if (Array.IndexOf(state.Hand, RdaCards.FiendPiece) >= 0 && !state.Uses(RdaKey.FiendPieceSS)
                    && FiendTunerOnField(state))
                    AddHandSummon(moves, state, RdaCards.FiendPiece, RdaKey.FiendPieceSS, PlanKind.Activate, "Fiend Piece summons itself from the hand");

                if (Array.IndexOf(state.Hand, RdaCards.Magnamhut) >= 0 && !state.Uses(RdaKey.MagnamhutSS))
                {
                    foreach (int target in RdaArray.Distinct(state.Grave, n => C(n).IsMonster && (C(n).Attribute == CardAttribute.Light || C(n).Attribute == CardAttribute.Dark)))
                    {
                        RdaState paid = state.Mark(RdaKey.MagnamhutSS);
                        paid.Hand = RdaArray.Remove(state.Hand, RdaCards.Magnamhut);
                        paid.Grave = RdaArray.Remove(state.Grave, target);
                        paid.Banished = RdaArray.Add(state.Banished, target);
                        RdaState placed = Place(paid, RdaCards.Magnamhut);
                        if (placed != null)
                            moves.Add(new RdaMove(Action(PlanKind.Activate, RdaCards.Magnamhut, CardLocation.Hand, RdaKey.MagnamhutSS,
                                "Magnamhut banishes " + N(target) + " and summons itself", new RdaPick(target, CardLocation.Grave)), OnSummoned(placed, RdaCards.Magnamhut)));
                    }
                    // GY do oponente: tira recurso dele e a carta banida vira alvo da Dis Pater. Um alvo só (o que mais tira dele:
                    // efeito a partir do GY, monstro do Extra, nível): para o nosso lado todos dão o mesmo estado.
                    if (state.EnemyGrave.Length > 0)
                    {
                        int target = Array.IndexOf(state.EnemyGrave, state.EnemyGravePriority) >= 0
                            ? state.EnemyGravePriority : state.EnemyGrave.OrderByDescending(n => RdaCards.Level(state, n)).First();
                        RdaState paid = state.Mark(RdaKey.MagnamhutSS);
                        paid.Hand = RdaArray.Remove(state.Hand, RdaCards.Magnamhut);
                        paid.EnemyGrave = RdaArray.Remove(state.EnemyGrave, target);
                        paid.EnemyBanished = RdaArray.Add(state.EnemyBanished, target);
                        RdaState placed = Place(paid, RdaCards.Magnamhut);
                        if (placed != null)
                            moves.Add(new RdaMove(Action(PlanKind.Activate, RdaCards.Magnamhut, CardLocation.Hand, RdaKey.MagnamhutSS,
                                "Magnamhut banishes " + N(target) + " from the opponent GY and summons itself", new RdaPick(target, CardLocation.Grave)), OnSummoned(placed, RdaCards.Magnamhut)));
                    }
                }

                ProfileMark(ref profileTick, 2);
                // ---- Bone Archfiend (mão ou GY): manda 1 outra carta da mão ou do campo ao GY e se invoca
                if (!state.Uses(RdaKey.BoneSS))
                {
                    foreach (CardLocation origin in new[] { CardLocation.Hand, CardLocation.Grave })
                    {
                        int[] pile = origin == CardLocation.Hand ? state.Hand : state.Grave;
                        if (Array.IndexOf(pile, RdaCards.Bone) < 0) continue;
                        RdaState taken = state.Mark(RdaKey.BoneSS);
                        if (origin == CardLocation.Hand) taken.Hand = RdaArray.Remove(state.Hand, RdaCards.Bone);
                        else taken.Grave = RdaArray.Remove(state.Grave, RdaCards.Bone);
                        string where = origin == CardLocation.Hand ? "hand" : "GY";

                        // Custo da mão: as cartas de interação forte (King's Resonance, RDA's Chain, Red Zone, Ash,
                        // Impermanence, Dominus, Maxx "C", Harmonia) são ÚLTIMO RECURSO — só entram quando não sobra
                        // mais nada na mão. Jogador, 2026-09-22 (log 042939 seq 136): o Bone mandou a King's Resonance,
                        // uma das traps mais fortes do deck, porque o plano previa recuperá-la com o Burning Soul; o
                        // Bone foi negado por Solemn Strike e a trap ficou morta no GY. Apostar a interação num passo
                        // que ainda pode ser negado não compensa.
                        var ordinaryCosts = RdaArray.Distinct(taken.Hand, n => !IsStrongInteraction(n)).ToList();
                        var costs = ordinaryCosts.Count > 0 ? ordinaryCosts : RdaArray.Distinct(taken.Hand, n => true).ToList();
                        foreach (int cost in costs)
                        {
                            RdaState removed = taken.Copy();
                            removed.Hand = RdaArray.Remove(taken.Hand, cost);
                            RdaState placed = Place(SendToGrave(removed, cost), RdaCards.Bone);
                            if (placed != null)
                                moves.Add(new RdaMove(Action(PlanKind.Activate, RdaCards.Bone, origin, RdaKey.BoneSS,
                                    "Bone (" + where + ") sends " + N(cost) + " from the hand and summons itself", new RdaPick(cost, CardLocation.Hand)), OnSummoned(placed, RdaCards.Bone)));
                        }
                        foreach (long monster in DistinctField(taken.Field))
                        {
                            if (NeverBecomesCost(taken, monster)) continue;
                            RdaState placed = Place(SendToGrave(RemoveField(taken, monster), RdaField.Id(monster), true), RdaCards.Bone);
                            if (placed != null)
                                moves.Add(new RdaMove(Action(PlanKind.Activate, RdaCards.Bone, origin, RdaKey.BoneSS,
                                    "Bone (" + where + ") sends " + N(RdaField.Id(monster)) + " from the field and summons itself", new RdaPick(RdaField.Id(monster), CardLocation.MonsterZone)), OnSummoned(placed, RdaCards.Bone)));
                        }
                    }
                }

                ProfileMark(ref profileTick, 3);
                // ---- The Bystial Lubellion: invocação por tributo (procedimento), busca e colocar Etude
                if (!state.Uses(RdaKey.LubellionSS))
                {
                    foreach (CardLocation origin in new[] { CardLocation.Hand, CardLocation.Grave })
                    {
                        int[] pile = origin == CardLocation.Hand ? state.Hand : state.Grave;
                        if (Array.IndexOf(pile, RdaCards.Lubellion) < 0) continue;
                        foreach (long monster in DistinctField(state.Field))
                        {
                            RdaCardInfo card = C(RdaField.Id(monster));
                            if (card.Attribute != CardAttribute.Dark || card.Race != CardRace.Dragon || RdaField.Level(monster) < 6) continue;
                            if (NeverBecomesCost(state, monster)) continue;
                            RdaState taken = state.Mark(RdaKey.LubellionSS);
                            if (origin == CardLocation.Hand) taken.Hand = RdaArray.Remove(state.Hand, RdaCards.Lubellion);
                            else taken.Grave = RdaArray.Remove(state.Grave, RdaCards.Lubellion);
                            RdaState placed = Place(SendToGrave(RemoveField(taken, monster), RdaField.Id(monster), true), RdaCards.Lubellion);
                            if (placed != null)
                                moves.Add(new RdaMove(Action(PlanKind.SpecialProc, RdaCards.Lubellion, origin, RdaKey.LubellionSS,
                                    "Lubellion (" + (origin == CardLocation.Hand ? "hand" : "GY") + ") releases " + N(card.Id) + " and summons itself",
                                    new RdaPick(card.Id, CardLocation.MonsterZone)), placed));
                        }
                    }
                }
                if (Array.IndexOf(state.Hand, RdaCards.Lubellion) >= 0 && !state.Uses(RdaKey.LubellionSearch) && Array.IndexOf(state.Deck, RdaCards.Magnamhut) >= 0)
                {
                    RdaState result = state.Mark(RdaKey.LubellionSearch);
                    result.Hand = RdaArray.Remove(state.Hand, RdaCards.Lubellion);
                    result.Grave = RdaArray.Add(state.Grave, RdaCards.Lubellion);
                    moves.Add(new RdaMove(Action(PlanKind.Activate, RdaCards.Lubellion, CardLocation.Hand, RdaKey.LubellionSearch,
                        "Lubellion discards and searches Magnamhut", new RdaPick(RdaCards.Magnamhut, CardLocation.Deck)), DeckToHand(result, RdaCards.Magnamhut)));
                }
                if (state.Field.Any(m => RdaField.Id(m) == RdaCards.Lubellion && !RdaField.Negated(m)) && !state.Uses(RdaKey.LubellionPlace)
                    && Array.IndexOf(state.Deck, RdaCards.Etude) >= 0 && state.SpellZonesFree > 0)
                {
                    RdaState result = state.Mark(RdaKey.LubellionPlace);
                    result.Deck = RdaArray.Remove(state.Deck, RdaCards.Etude);
                    result.Spells = RdaArray.Add(state.Spells, RdaCards.Etude);
                    moves.Add(new RdaMove(Action(PlanKind.Activate, RdaCards.Lubellion, CardLocation.MonsterZone, RdaKey.LubellionPlace,
                        "Lubellion places Etude of the Branded", new RdaPick(RdaCards.Etude, CardLocation.Deck)), result));
                }

                ProfileMark(ref profileTick, 4);
                // ---- Stone Sweeper: a invocação da mão saiu do modelo (jogador, 2026-09-15: "nunca deve ser invocado; neste deck
                // a única função dele é descartar e buscar"). Ele não entra em nenhuma rota e ocupava zona à toa.

                ProfileMark(ref profileTick, 5);
                // ---- Stone Sweeper: descarta e busca Fiend Tuner nível 3 ou menor
                if (Array.IndexOf(state.Hand, RdaCards.StoneSweeper) >= 0 && !state.Uses(RdaKey.StoneSweeperSearch))
                {
                    RdaState paid = state.Mark(RdaKey.StoneSweeperSearch);
                    paid.Hand = RdaArray.Remove(state.Hand, RdaCards.StoneSweeper);
                    paid.Grave = RdaArray.Add(state.Grave, RdaCards.StoneSweeper);
                    foreach (int id in RdaArray.Distinct(state.Deck, n => RdaCards.FiendTuners.Contains(n) && C(n).Level <= 3))
                        moves.Add(new RdaMove(Action(PlanKind.Activate, RdaCards.StoneSweeper, CardLocation.Hand, RdaKey.StoneSweeperSearch,
                            "Stone Sweeper discards and searches " + N(id), new RdaPick(id, CardLocation.Deck)), DeckToHand(paid, id)));
                }

                ProfileMark(ref profileTick, 6);
                // ---- Bone no campo: alvo 1 monstro, manda Fiend Tuner da mão/Deck ao GY e muda o nível em ±1
                if (!state.Uses(RdaKey.BoneLevel) && state.Field.Any(m => RdaField.Id(m) == RdaCards.Bone && !RdaField.Negated(m)))
                {
                    foreach (CardLocation origin in new[] { CardLocation.Hand, CardLocation.Deck })
                    {
                        int[] pile = origin == CardLocation.Hand ? state.Hand : state.Deck;
                        foreach (int tuner in RdaArray.Distinct(pile, n => RdaCards.FiendTuners.Contains(n)))
                        {
                            RdaState paid = state.Mark(RdaKey.BoneLevel);
                            if (origin == CardLocation.Hand) paid.Hand = RdaArray.Remove(state.Hand, tuner);
                            else paid.Deck = RdaArray.Remove(state.Deck, tuner);
                            foreach (long monster in DistinctField(paid.Field))
                            {
                                foreach (int delta in new[] { 1, -1 })
                                {
                                    int level = RdaField.Level(monster) + delta;
                                    if (level < 1) continue;
                                    RdaState changed = RemoveField(paid, monster);
                                    changed.Field = RdaArray.Add(changed.Field, RdaField.WithLevel(monster, level));
                                    // O monstro continua no campo: só mudou de nível, então a proteção do RDA continua valendo.
                                    changed.Flags |= paid.Flags & RdaState.FlagProtectedRda;
                                    PlanAction action = Action(PlanKind.Activate, RdaCards.Bone, CardLocation.MonsterZone, RdaKey.BoneLevel,
                                        "Bone sends " + N(tuner) + " (" + (origin == CardLocation.Hand ? "hand" : "deck") + ") and " + (delta > 0 ? "raises" : "lowers") + " the Level of " + N(RdaField.Id(monster)),
                                        new RdaPick(RdaField.Id(monster), CardLocation.MonsterZone), new RdaPick(tuner, origin));
                                    action.LevelDelta = delta;
                                    moves.Add(new RdaMove(action, SendToGrave(changed, tuner)));
                                }
                            }
                        }
                    }
                }

                ProfileMark(ref profileTick, 7);
                // ---- Darkness no campo: tuners escolhidos viram nível 1
                if (!state.Uses(RdaKey.DarknessLevel) && state.Field.Any(m => RdaField.Id(m) == RdaCards.Darkness && !RdaField.Negated(m)))
                {
                    var tuners = state.Field.Where(m => RdaCards.Tuner(state, RdaField.Id(m)) && RdaField.Level(m) > 1).ToList();
                    for (int mask = 1; mask < (1 << tuners.Count); ++mask)
                    {
                        RdaState changed = state.Mark(RdaKey.DarknessLevel);
                        var picks = new List<RdaPick>();
                        for (int i = 0; i < tuners.Count; ++i)
                        {
                            if ((mask & (1 << i)) == 0) continue;
                            changed = RemoveField(changed, tuners[i]);
                            changed.Field = RdaArray.Add(changed.Field, RdaField.WithLevel(tuners[i], 1));
                            picks.Add(new RdaPick(RdaField.Id(tuners[i]), CardLocation.MonsterZone));
                        }
                        RdaPick[] levelPicks = picks.ToArray();
                        var levelAction = Action(PlanKind.Activate, RdaCards.Darkness, CardLocation.MonsterZone, RdaKey.DarknessLevel, null, levelPicks);
                        levelAction.TextFactory = () => "Darkness changes to Level 1: " + string.Join(", ", levelPicks.Select(p => N(p.Id)));
                        moves.Add(new RdaMove(levelAction, changed));
                    }
                }

                ProfileMark(ref profileTick, 8);
                // ---- Crimson Resonator no campo: o único outro monstro é 1 Synchro Dragon DARK → invoca até 2 Resonators
                if (!state.Uses(RdaKey.CrimsonEffect))
                {
                    long crimson = state.Field.FirstOrDefault(m => RdaField.Id(m) == RdaCards.Crimson && !RdaField.Negated(m));
                    if (crimson != 0)
                    {
                        var others = state.Field.Where(m => m != crimson).ToList();
                        if (others.Count == 1)
                        {
                            int otherId = RdaField.Id(others[0]);
                            if (C(otherId).Synchro && RdaCards.Attribute(state, otherId) == CardAttribute.Dark
                                && RdaCards.Race(state, otherId) == CardRace.Dragon)
                                AddCrimsonEffect(moves, state);
                        }
                    }
                }

                ProfileMark(ref profileTick, 9);
                // ---- Bystial Dis Pater: invoca 1 monstro LIGHT/DARK banido
                if (!state.Uses(RdaKey.DisPaterEffect) && state.Field.Any(m => RdaField.Id(m) == RdaCards.DisPater && !RdaField.Negated(m)))
                {
                    foreach (int target in RdaArray.Distinct(state.Banished, n => C(n).IsMonster && (C(n).Attribute == CardAttribute.Light || C(n).Attribute == CardAttribute.Dark)))
                    {
                        RdaState removed = state.Mark(RdaKey.DisPaterEffect);
                        removed.Banished = RdaArray.Remove(state.Banished, target);
                        RdaState placed = Place(removed, target, C(target).Level, false);
                        if (placed != null)
                            moves.Add(new RdaMove(Action(PlanKind.Activate, RdaCards.DisPater, CardLocation.MonsterZone, RdaKey.DisPaterEffect,
                                "Dis Pater summons " + N(target) + " from the banished zone", new RdaPick(target, CardLocation.Removed)), OnSummoned(placed, target)));
                    }
                    // Monstro banido do oponente: vem para o nosso campo (sem gatilhos nossos).
                    foreach (int target in RdaArray.Distinct(state.EnemyBanished, n => RdaCards.Level(state, n) > 0))
                    {
                        RdaState removed = state.Mark(RdaKey.DisPaterEffect);
                        removed.EnemyBanished = RdaArray.Remove(state.EnemyBanished, target);
                        RdaState placed = Place(removed, target, RdaCards.Level(state, target), false);
                        if (placed != null)
                            moves.Add(new RdaMove(Action(PlanKind.Activate, RdaCards.DisPater, CardLocation.MonsterZone, RdaKey.DisPaterEffect,
                                "Dis Pater summons " + N(target) + " banished from the opponent", new RdaPick(target, CardLocation.Removed)), placed));
                    }
                }

                ProfileMark(ref profileTick, 10);
                // ---- Red Zone ②: invoca 1 Synchro Dragão DARK nosso banido (ex.: RDA banido pelo Burning Soul, nova banido).
                if (state.RedZoneReady && !state.Uses(RdaKey.RedZoneRevive))
                {
                    foreach (int target in RdaArray.Distinct(state.Banished, n => C(n).Synchro && C(n).Attribute == CardAttribute.Dark && C(n).Race == CardRace.Dragon))
                    {
                        RdaState removed = state.Mark(RdaKey.RedZoneRevive);
                        removed.Banished = RdaArray.Remove(state.Banished, target);
                        RdaState placed = Place(removed, target, C(target).Level, false);
                        if (placed != null)
                            moves.Add(new RdaMove(Action(PlanKind.Activate, RdaCards.RedZone, CardLocation.SpellZone, RdaKey.RedZoneRevive,
                                "Red Zone summons " + N(target) + " from the banished zone", new RdaPick(target, CardLocation.Removed)), placed));
                    }
                }

                ProfileMark(ref profileTick, 11);
                // ---- Red Rising Dragon (GY): na Main Phase, exceto no turno em que foi ao GY: bane a si mesmo e invoca
                //      2 Resonators nível 1 do GY (Chain e Synkron). Sem "uma vez por turno": o limite são as cópias prontas.
                if (state.RedRisingGraveReady > 0 && Array.IndexOf(state.Grave, RdaCards.RedRising) >= 0 && state.MainZonesFree >= 2)
                {
                    var levelOne = RdaArray.Distinct(state.Grave, n => C(n).Resonator && C(n).Level == 1).ToList();
                    for (int a = 0; a < levelOne.Count; ++a)
                    {
                        for (int b = a; b < levelOne.Count; ++b)
                        {
                            int first = levelOne[a], second = levelOne[b];
                            if (a == b && state.Grave.Count(x => x == first) < 2)
                                continue;
                            RdaState paid = state.Copy();
                            paid.Grave = RdaArray.Remove(RdaArray.Remove(RdaArray.Remove(state.Grave, RdaCards.RedRising), first), second);
                            paid.Banished = RdaArray.Add(state.Banished, RdaCards.RedRising);
                            paid.RedRisingGraveReady = state.RedRisingGraveReady - 1;
                            RdaState placed = Place(paid, first);
                            if (placed == null) continue;
                            placed = Place(placed, second);
                            if (placed == null) continue;
                            placed = OnSummoned(OnSummoned(placed, first), second);
                            moves.Add(new RdaMove(Action(PlanKind.Activate, RdaCards.RedRising, CardLocation.Grave, RdaKey.None,
                                "Red Rising (GY) banishes itself and summons " + N(first) + " + " + N(second),
                                new RdaPick(first, CardLocation.Grave), new RdaPick(second, CardLocation.Grave)), placed));
                        }
                    }
                }

                ProfileMark(ref profileTick, 12);
                // ---- Magias
                if (state.SpellZonesFree > 0)
                {
                    if (Array.IndexOf(state.Hand, RdaCards.Foolish) >= 0)
                    {
                        RdaState paid = state.Copy();
                        paid.Hand = RdaArray.Remove(state.Hand, RdaCards.Foolish);
                        paid.Grave = RdaArray.Add(state.Grave, RdaCards.Foolish);
                        foreach (int id in RdaArray.Distinct(state.Deck, n => C(n).IsMonster))
                        {
                            RdaState removed = paid.Copy();
                            removed.Deck = RdaArray.Remove(paid.Deck, id);
                            moves.Add(new RdaMove(Action(PlanKind.Activate, RdaCards.Foolish, CardLocation.Hand, RdaKey.None,
                                "Foolish Burial sends " + N(id) + " to the GY", new RdaPick(id, CardLocation.Deck)), SendToGrave(removed, id)));
                        }
                    }
                    if (Array.IndexOf(state.Hand, RdaCards.CrimsonCall) >= 0 && !state.Uses(RdaKey.CrimsonCall))
                    {
                        RdaState paid = state.Mark(RdaKey.CrimsonCall);
                        paid.Hand = RdaArray.Remove(state.Hand, RdaCards.CrimsonCall);
                        paid.Grave = RdaArray.Add(state.Grave, RdaCards.CrimsonCall);
                        foreach (int id in RdaArray.Distinct(state.Grave, n => RdaCards.Level4Fiends.Contains(n)))
                            moves.Add(new RdaMove(Action(PlanKind.Activate, RdaCards.CrimsonCall, CardLocation.Hand, RdaKey.CrimsonCall,
                                "Crimson Call takes " + N(id) + " from the GY", new RdaPick(id, CardLocation.Grave)), GraveToHand(paid, id)));
                        bool controlsRda = state.Field.Any(m => RdaCards.RdaLike.Contains(RdaField.Id(m)) || (C(RdaField.Id(m)).Synchro && C(RdaField.Id(m)).MentionsRda));
                        if (controlsRda)
                            foreach (int id in RdaArray.Distinct(state.Deck, n => RdaCards.Level4Fiends.Contains(n)))
                                moves.Add(new RdaMove(Action(PlanKind.Activate, RdaCards.CrimsonCall, CardLocation.Hand, RdaKey.CrimsonCall,
                                    "Crimson Call searches " + N(id) + " from the Deck", new RdaPick(id, CardLocation.Deck)), DeckToHand(paid, id)));
                    }
                    if (Array.IndexOf(state.Hand, RdaCards.ResonatorCall) >= 0)
                    {
                        RdaState paid = state.Copy();
                        paid.Hand = RdaArray.Remove(state.Hand, RdaCards.ResonatorCall);
                        paid.Grave = RdaArray.Add(state.Grave, RdaCards.ResonatorCall);
                        foreach (int id in RdaArray.Distinct(state.Deck, n => RdaCards.Resonators.Contains(n)))
                            moves.Add(new RdaMove(Action(PlanKind.Activate, RdaCards.ResonatorCall, CardLocation.Hand, RdaKey.None,
                                "Resonator Call searches " + N(id), new RdaPick(id, CardLocation.Deck)), DeckToHand(paid, id)));
                    }
                    // Só 1 Crimson Gaia no campo: os efeitos são 1 vez por turno pelo nome, então uma segunda não traz
                    // vantagem. As outras ficam na mão como reserva caso a do campo seja removida (jogador, 2026-09-14).
                    if (Array.IndexOf(state.Hand, RdaCards.Gaia) >= 0 && Array.IndexOf(state.Spells, RdaCards.Gaia) < 0)
                    {
                        RdaState result = state.Copy();
                        result.Hand = RdaArray.Remove(state.Hand, RdaCards.Gaia);
                        result.Spells = RdaArray.Add(state.Spells, RdaCards.Gaia);
                        moves.Add(new RdaMove(Action(PlanKind.Activate, RdaCards.Gaia, CardLocation.Hand, RdaKey.None, "Activate Crimson Gaia"), result));
                    }
                }
                if (Array.IndexOf(state.Spells, RdaCards.Gaia) >= 0 && !state.Uses(RdaKey.GaiaSearch))
                {
                    RdaState marked = state.Mark(RdaKey.GaiaSearch);
                    foreach (int id in RdaArray.Distinct(state.Deck, n => RdaCards.MentionsMain.Contains(n) && n != RdaCards.Gaia))
                        moves.Add(new RdaMove(Action(PlanKind.Activate, RdaCards.Gaia, CardLocation.SpellZone, RdaKey.GaiaSearch,
                            "Crimson Gaia searches " + N(id) + " from the Deck", new RdaPick(id, CardLocation.Deck)), DeckToHand(marked, id)));
                    foreach (int id in RdaArray.Distinct(state.Grave, n => RdaCards.MentionsMain.Contains(n) && n != RdaCards.Gaia))
                        moves.Add(new RdaMove(Action(PlanKind.Activate, RdaCards.Gaia, CardLocation.SpellZone, RdaKey.GaiaSearch,
                            "Crimson Gaia takes " + N(id) + " from the GY", new RdaPick(id, CardLocation.Grave)), GraveToHand(marked, id)));
                }

                ProfileMark(ref profileTick, 13);
                // ---- Red Nova Dragon - Burning Soul pelo GY: banir 2 Tuners + 1 "Red Dragon Archfiend" (RDA ou Scarred)
                // do GY. É a forma preferida: não gasta monstros do campo (a Synchro fica para quando fecha o duelo).
                if (!state.NoSpecial && Array.IndexOf(state.Extra, RdaCards.BurningSoul) >= 0)
                {
                    var graveTuners = RdaArray.Distinct(state.Grave, n => C(n).Tuner).ToList();
                    foreach (int rdaId in RdaArray.Distinct(state.Grave, n => RdaCards.RdaLike.Contains(n)))
                    {
                        for (int a = 0; a < graveTuners.Count; ++a)
                        {
                            for (int b = a; b < graveTuners.Count; ++b)
                            {
                                int t1 = graveTuners[a], t2 = graveTuners[b];
                                if (a == b && state.Grave.Count(x => x == t1) < 2)
                                    continue;
                                RdaState paid = state.Copy();
                                paid.Grave = RdaArray.Remove(RdaArray.Remove(RdaArray.Remove(state.Grave, rdaId), t1), t2);
                                paid.Banished = RdaArray.Add(RdaArray.Add(RdaArray.Add(state.Banished, rdaId), t1), t2);
                                paid.Extra = RdaArray.Remove(state.Extra, RdaCards.BurningSoul);
                                RdaState placed = Place(paid, RdaCards.BurningSoul, C(RdaCards.BurningSoul).Level, true);
                                if (placed == null)
                                    continue;
                                moves.Add(new RdaMove(Action(PlanKind.SpecialProc, RdaCards.BurningSoul, CardLocation.Extra, RdaKey.None,
                                    "Burning Soul summons itself from the GY banishing " + N(t1) + ", " + N(t2) + " and " + N(rdaId),
                                    new RdaPick(t1, CardLocation.Grave), new RdaPick(t2, CardLocation.Grave), new RdaPick(rdaId, CardLocation.Grave)),
                                    OnSummoned(placed, RdaCards.BurningSoul)));
                            }
                        }
                    }
                }

                ProfileMark(ref profileTick, 14);
                // ---- Synchro
                var synchroOptions = SynchroOptions(state).ToList();
                // RDA protegido pelo King ② como material (jogador, 2026-09-22; log 043140 seq 352: virou material do
                // Abyss com o Crimson King nível 8 parado ao lado, que servia igual). Regra: ele só pode ser material
                // quando NÃO existir outra receita para o MESMO Synchro sem ele. Havendo substituto, a versão que gasta
                // o RDA protegido nem entra na lista, em vez de depender do bônus de nota ganhar da rota.
                // A flag diz que UM RDA no campo está protegido; com duas cópias no campo isto restringe as duas, o que
                // erra para o lado seguro.
                if (state.HasFlag(RdaState.FlagProtectedRda))
                {
                    var targetsWithSubstitute = new HashSet<int>(synchroOptions
                        .Where(o => !o.Value.Any(m => RdaField.Id(m) == RdaCards.Rda))
                        .Select(o => o.Key));
                    synchroOptions.RemoveAll(o => targetsWithSubstitute.Contains(o.Key)
                        && o.Value.Any(m => RdaField.Id(m) == RdaCards.Rda));
                }
                foreach (KeyValuePair<int, long[]> option in synchroOptions)
                {
                    RdaState result = DoSynchro(state, option.Key, option.Value);
                    if (result == null) continue;
                    int synchroTarget = option.Key;
                    long[] synchroMaterials = option.Value;
                    var action = Action(PlanKind.Synchro, synchroTarget, CardLocation.Extra, RdaKey.None, null);
                    action.TextFactory = () => "Synchro Summon " + N(synchroTarget) + " ("
                        + string.Join(" + ", synchroMaterials.Select(m => N(RdaField.Id(m)) + " " + RdaField.Level(m))) + ")";
                    action.Materials = option.Value;
                    moves.Add(new RdaMove(action, result));
                }

                ProfileMark(ref profileTick, 15);
                return moves;
            }

            private static void AddNormalSummon(List<RdaMove> moves, RdaState state, int id, bool extra)
            {
                RdaState removed = state.Copy();
                removed.Hand = RdaArray.Remove(state.Hand, id);
                if (extra) removed.ExtraNormalSummon = 0;
                else removed.NormalSummon = 0;
                RdaState placed;
                if (state.NoSpecial)
                {
                    // A trava da Quetzacoatl só impede Invocação-Especial.
                    if (removed.MainZonesFree <= 0) return;
                    placed = removed.Copy();
                    placed.Field = RdaArray.Add(removed.Field, RdaField.Encode(id, C(id).Level, false, false));
                }
                else
                {
                    placed = Place(removed, id);
                }
                if (placed == null) return;
                moves.Add(new RdaMove(Action(extra ? PlanKind.ExtraNormalSummon : PlanKind.NormalSummon, id, CardLocation.Hand, RdaKey.None,
                    (extra ? "extra Normal Summon (Darkness) of " : "Normal Summon ") + N(id)), OnSummoned(placed, id, false, true)));
            }

            private static void AddHandSummon(List<RdaMove> moves, RdaState state, int id, RdaKey key, PlanKind kind, string text, params RdaPick[] picks)
            {
                RdaState removed = state.Mark(key);
                removed.Hand = RdaArray.Remove(state.Hand, id);
                RdaState placed = Place(removed, id);
                if (placed != null)
                    moves.Add(new RdaMove(Action(kind, id, CardLocation.Hand, key, text, picks), OnSummoned(placed, id)));
            }

            private static void AddCrimsonEffect(List<RdaMove> moves, RdaState state)
            {
                RdaState marked = state.Mark(RdaKey.CrimsonEffect);
                var pool = new List<RdaPick>();
                foreach (int id in state.Hand.Where(n => RdaCards.Resonators.Contains(n) && n != RdaCards.Crimson))
                    pool.Add(new RdaPick(id, CardLocation.Hand));
                foreach (int id in state.Deck.Where(n => RdaCards.Resonators.Contains(n) && n != RdaCards.Crimson))
                    pool.Add(new RdaPick(id, CardLocation.Deck));

                var seen = new HashSet<string>();
                var combos = new List<RdaPick[]>();
                for (int i = 0; i < pool.Count; ++i)
                {
                    string single = pool[i].Id + "@" + pool[i].Location;
                    if (seen.Add(single))
                        combos.Add(new[] { pool[i] });
                    for (int j = i + 1; j < pool.Count; ++j)
                    {
                        RdaPick a = pool[i], b = pool[j];
                        string pair = string.CompareOrdinal(a.Id + "@" + a.Location, b.Id + "@" + b.Location) <= 0
                            ? a.Id + "@" + a.Location + "|" + b.Id + "@" + b.Location
                            : b.Id + "@" + b.Location + "|" + a.Id + "@" + a.Location;
                        if (seen.Add(pair))
                            combos.Add(new[] { a, b });
                    }
                }

                foreach (RdaPick[] combo in combos)
                {
                    RdaState result = marked;
                    bool ok = true;
                    foreach (RdaPick pick in combo)
                    {
                        RdaState removed = result.Copy();
                        if (pick.Location == CardLocation.Hand) removed.Hand = RdaArray.Remove(result.Hand, pick.Id);
                        else removed.Deck = RdaArray.Remove(result.Deck, pick.Id);
                        result = Place(removed, pick.Id);
                        if (result == null) { ok = false; break; }
                    }
                    if (!ok) continue;
                    foreach (RdaPick pick in combo)
                        result = OnSummoned(result, pick.Id);
                    RdaPick[] crimsonCombo = combo;
                    var crimsonAction = Action(PlanKind.Activate, RdaCards.Crimson, CardLocation.MonsterZone, RdaKey.CrimsonEffect, null, crimsonCombo);
                    crimsonAction.TextFactory = () => "Crimson Resonator summons "
                        + string.Join(" + ", crimsonCombo.Select(p => N(p.Id) + " (" + (p.Location == CardLocation.Hand ? "hand" : "deck") + ")"));
                    moves.Add(new RdaMove(crimsonAction, result));
                }
            }
        }

        /// <summary>Nota do campo final e potencial de um estado intermediário.</summary>
        private static class RdaEvaluator
        {
            // Valores da mesa (regras do jogador, 14/09/2026). A BUSCA usa estes valores "neutros".
            // Ordem: Hypernova > Supernova > Quetzacoatl > Dis Pater/Abyss > Zalen > King > Burning Soul > Storm-Bane >
            // Blade > Scarred > RDA > Red Rising. Storm-Bane vale pouco no campo: o uso ideal é ir ao GY pela
            // Fydraulis Harmonia e trazer o Hypernova de volta. O campo deve ser o maior possível (+10 por monstro).
            private static readonly Dictionary<int, double> FieldValue = new Dictionary<int, double>
            {
                { RdaCards.Hypernova, 100 }, { RdaCards.Supernova, 70 }, { RdaCards.Quetzacoatl, 40 }, { RdaCards.DisPater, 35 },
                // Burning Soul subiu de 20 para 28 em 2026-09-22 (jogador): acima do Crimson King, abaixo do Zalen.
                // A 20 ele era a segunda peça mais barata da mesa e o avaliador o trocava por qualquer coisa. O valor
                // também define a fila do revive do Quetzacoatl, que é ordenada por este número.
                { RdaCards.Abyss, 35 }, { RdaCards.Zalen, 30 }, { RdaCards.BurningSoul, 28 }, { RdaCards.King, 25 },
                { RdaCards.StormBane, 15 }, { RdaCards.Blade, 12 }, { RdaCards.Scarred, 10 }, { RdaCards.Rda, 8 },
                { RdaCards.RedRising, 6 }
            };
            private const double NonSynchroFieldValue = 3;
            private const double BurningSoulAddBonus = 15;      // efeito do Burning Soul usado (ATK + carta de volta)
            private static double NeutralBurningSoulAddBonus = 0; // static para teste offline por reflexão
            private const double SoulGraveProtection = 12;      // Soul Resonator no GY protege o campo de destruição
            private const double SynchroDefaultValue = 5;
            private const double FieldSizeBonus = 10;
            // Bônus do RDA que veio do efeito ② do Crimson King (ou da Gaia): ele soma ao valor base 8, ficando acima do
            // King (25) que se baniu para trazê-lo — o jogador trata essa troca como ganho, não como perda.
            private const double ProtectedRdaBonus = 25;
            // Segundo ataque do RDA pela Crimson Call no GY: menos que a limpeza da Gaia (60), mais que uma peça comum.
            private const double CrimsonCallSecondAttack = 20;
            private const double RdaGaiaWipeBonus = 60;          // RDA + Crimson Gaia no campo com batalha e 2+ monstros do oponente

            // Preferências usadas SÓ na escolha da mesa final. Dentro da busca elas desviam rotas de Hypernova
            // (bateria 11: camada 0 caiu de 22 para 18 mãos em 25).
            private const double SelectionAbyss = 33;
            private const double SelectionZalen = 25;
            private const double ZalenChainBonus = 15;          // o Zalen precisa de chain: King ② ou Red Zone ativam antes
            private const double LubellionNotAccessed = -30;    // a Crimson Blade busca a Lubellion (pacote Bystial)
            private const double ExposedNonSynchro = -10;       // Bone, Magnamhut, Resonators... não devem ficar expostos na mesa

            private static readonly Dictionary<int, double> HandValue = new Dictionary<int, double>
            {
                { RdaCards.Harmonia, 25 }, { RdaCards.Ash, 15 }, { RdaCards.Impermanence, 15 }, { RdaCards.Dominus, 10 }, { RdaCards.MaxxC, 6 }
            };

            private static readonly Dictionary<int, double> HandPotential = new Dictionary<int, double>
            {
                { RdaCards.PowerVice, 8 }, { RdaCards.Bone, 7 }, { RdaCards.Lubellion, 8 }, { RdaCards.Magnamhut, 8 },
                { RdaCards.StoneSweeper, 6 }, { RdaCards.Foolish, 6 }, { RdaCards.CrimsonCall, 7 }, { RdaCards.ResonatorCall, 7 },
                { RdaCards.Gaia, 7 }, { RdaCards.FiendPiece, 4 }
            };

            public static double BoardValue(int id)
            {
                double value;
                if (FieldValue.TryGetValue(id, out value))
                    return value;
                RdaCardInfo card = RdaCards.Get(id);
                return card != null && card.Synchro ? SynchroDefaultValue : NonSynchroFieldValue;
            }

            // Valor do RDA com a linha da Crimson Gaia viva: 8 + 37 = 45, acima da Quetzacoatl (40) e abaixo do
            // Supernova (70). Jogador, 2026-09-22: nesse turno o RDA + Gaia viram a mesa do oponente para baixo e
            // destroem tudo, é quase ganhar o duelo. Fora dessa janela o RDA continua valendo 8, porque destrói os
            // nossos monstros na End Phase.
            private const double RdaGaiaLineFieldBonus = 37;

            /// <summary>
            /// A linha RDA + Crimson Gaia está viva neste estado: é turno de ataque nosso com monstro do oponente no
            /// campo (BattleWipeReady), a Gaia não está negada e nós já temos a Gaia — ativa no campo ou na mão, que é
            /// um passo garantido de distância (o planejador tem o lance "Ativa Crimson Gaia").
            ///
            /// A Gaia só no Deck NÃO conta. Medição dos 8 duelos de 2026-09-22: ela chegou ao campo em 5 e nunca saiu
            /// do Deck em 3. Valorizar o RDA na esperança de buscá-la deixaria, em 4 de 10 partidas, um RDA comum na
            /// mesa destruindo os nossos monstros na End Phase. Quando o plano REALMENTE busca a Gaia, todo estado
            /// depois desse passo já a tem na mão ou no campo, então a conta fecha sozinha — e nos mesmos logs a Gaia
            /// desceu cedo (turno 1 seq 60 e 110, turno 2 seq 88 e 93), antes das decisões sobre o RDA.
            /// </summary>
            public static bool GaiaLineLive(RdaState state)
            {
                return state.BattleWipeReady && !state.GaiaNegated
                    && (Array.IndexOf(state.Spells, RdaCards.Gaia) >= 0 || Array.IndexOf(state.Hand, RdaCards.Gaia) >= 0);
            }

            /// <summary>Valor de campo sensível ao contexto. Hoje só o RDA muda: ver <see cref="LinhaGaiaViva"/>.</summary>
            public static double BoardValue(int id, RdaState state)
            {
                double value = BoardValue(id);
                if (id == RdaCards.Rda && GaiaLineLive(state))
                    value += RdaGaiaLineFieldBonus;
                return value;
            }

            // As funções abaixo rodam para dezenas de milhares de estados por busca: sem LINQ, sem listas e sem montar
            // textos (o texto de detalhe só é montado quando 'details' é pedido, para depuração).
            private static void Detail(List<string> details, string label, double value)
            {
                details.Add(label + " " + value.ToString("0.#"));
            }

            private static int Count(int[] items, int id)
            {
                int count = 0;
                for (int i = 0; i < items.Length; ++i)
                    if (items[i] == id) count++;
                return count;
            }

            // A i-ésima carta da mão final foi ganha durante o combo? (multiconjunto: cópias além das da mão inicial)
            private static bool IsGained(int[] finalHand, int index, int[] rootHand)
            {
                int id = finalHand[index];
                int copiesBefore = 0;
                for (int i = 0; i < index; ++i)
                    if (finalHand[i] == id) copiesBefore++;
                return copiesBefore >= Count(rootHand, id);
            }

            /// <summary>
            /// Camada da mesa (especificação do jogador, 2026-09-17; menor é melhor):
            ///   0 = nova (Hypernova ou Supernova) + Quetzacoatl + Dis Pater + (Abyss ou Zalen) + 2 Synchros úteis quaisquer;
            ///   1 = a MESMA mesa sem chegar no nova: Burning Soul ou Storm-Bane ocupa o lugar dele;
            ///   2 = nem isso, mas ainda 2+ negates e mesa cheia (5+ monstros, 4+ Synchros fora o Red Rising,
            ///       no máximo 1 entre Lubellion/Magnamhut/Fydraulis);
            ///   3 = o resto. Menos de 4 Synchros úteis: mesmo com os melhores monstros a mesa fica vulnerável.
            /// Negates = Quetzacoatl, Abyss, Zalen, Dis Pater.
            /// Só 4 cartas são papel fixo (jogador, 2026-09-17). Crimson King e Crimson Blade NÃO são exigidos: os dois
            /// lugares restantes aceitam QUALQUER Synchro que não seja o Red Rising. A Blade só destrói um monstro de
            /// nível 5+ em batalha e não vale contra Xyz nem Link, e a mesa sem o King não é pior por isso — a mesa ideal
            /// é a que chega no nova e no Quetzacoatl, que são os trunfos.
            /// </summary>
            public static int Tier(RdaState state)
            {
                long[] field = state.Field;
                bool hypernova = false, supernova = false, quetzacoatl = false, disPater = false;
                bool abyss = false, zalen = false;
                int soulBane = 0;    // Burning Soul / Storm-Bane: ocupam o lugar do nova na camada 1
                int synchros = 0;    // Synchros úteis: todo o Extra menos o Red Rising
                int nonSynchros = 0; // Lubellion / Magnamhut / Fydraulis
                for (int i = 0; i < field.Length; ++i)
                {
                    int id = RdaField.Id(field[i]);
                    if (id == RdaCards.Hypernova) hypernova = true;
                    else if (id == RdaCards.Supernova) supernova = true;
                    else if (id == RdaCards.Quetzacoatl) quetzacoatl = true;
                    else if (id == RdaCards.DisPater) disPater = true;
                    else if (id == RdaCards.Abyss) abyss = true;
                    else if (id == RdaCards.Zalen) zalen = true;
                    else if (id == RdaCards.BurningSoul || id == RdaCards.StormBane) ++soulBane;
                    if (RdaCards.UsefulSynchros.Contains(id)) ++synchros;
                    else if (RdaCards.FieldNonSynchros.Contains(id)) ++nonSynchros;
                }
                // Papéis obrigatórios: nova + Quetzacoatl + Dis Pater + (Abyss ou Zalen). O Crimson King NÃO é obrigatório
                // (jogador, 2026-09-17: "não é porque não tem o king que não é boa e entra em outra camada") — ele é um dos
                // Synchros que preenchem os dois lugares livres, igual à Crimson Blade. Os dois lugares livres aceitam
                // QUALQUER Synchro útil, isto é, qualquer um menos o Red Rising; por isso a exigência é só ter 6 deles.
                // Camadas 0 e 1 são a mesma mesa: muda só quem ocupa o lugar do nova.
                bool core = quetzacoatl && disPater && (abyss || zalen) && synchros >= 6;
                if (core && (hypernova || supernova))
                    return 0;
                if (core && soulBane >= 1)
                    return 1;
                int negates = (quetzacoatl ? 1 : 0) + (abyss ? 1 : 0) + (zalen ? 1 : 0) + (disPater ? 1 : 0);
                if (negates >= 2 && field.Length >= 5 && synchros >= 4 && nonSynchros <= 1)
                    return 2;
                return 3;
            }

            /// <summary>Valor de uma carta que não é Resonator voltando para a mão (usado para escolher o alvo do Burning Soul).</summary>
            public static double HandCardValue(int id)
            {
                double value;
                if (HandValue.TryGetValue(id, out value)) return value;
                return Math.Max(TrapValue(id, false), SpellValue(id));
            }

            private static double SpellValue(int spell)
            {
                return spell == RdaCards.Etude ? 12 : spell == RdaCards.Gaia ? 8 : 2;
            }

            private static double TrapValue(int id, bool hypernova)
            {
                if (id == RdaCards.RedZone) return hypernova ? 20 : 8;
                if (id == RdaCards.KingsResonance) return hypernova ? 8 : 15;
                if (id == RdaCards.RdaChain) return 10;
                if (id == RdaCards.Impermanence || id == RdaCards.Dominus) return HandValue[id];
                return -1; // não é armadilha
            }

            /// <summary>
            /// Nota de seleção da mesa final (usada para escolher entre as mesas encontradas, nunca para guiar a busca).
            /// Não conta cartas que ficaram na mão do início ao fim; aplica as preferências do jogador
            /// (Abyss 33; Zalen 25, ou 40 com The Crimson King no campo ou Red Zone disponível; Lubellion no Deck -30).
            /// </summary>
            public static double SelectionScore(RdaState state, int[] rootHand, List<string> details = null)
            {
                double score = 0;
                long[] field = state.Field;
                bool rda = false, hypernova = false, supernova = false, disPater = false, king = false;
                for (int i = 0; i < field.Length; ++i)
                {
                    int id = RdaField.Id(field[i]);
                    if (id == RdaCards.Rda) rda = true;
                    else if (id == RdaCards.Hypernova) hypernova = true;
                    else if (id == RdaCards.Supernova) supernova = true;
                    else if (id == RdaCards.DisPater) disPater = true;
                    else if (id == RdaCards.King) king = true;
                }
                bool redZoneInHand = Array.IndexOf(state.Hand, RdaCards.RedZone) >= 0;
                bool redZoneOnField = Array.IndexOf(state.Spells, RdaCards.RedZone) >= 0;
                bool zalenEnabled = king || redZoneInHand || redZoneOnField;

                int synchroCount = 0;
                for (int i = 0; i < field.Length; ++i)
                {
                    int id = RdaField.Id(field[i]);
                    RdaCardInfo info = RdaCards.Get(id);
                    bool isSynchro = info != null && info.Synchro;
                    double value = !isSynchro ? ExposedNonSynchro
                                 : id == RdaCards.Abyss ? SelectionAbyss
                                 : id == RdaCards.Zalen ? SelectionZalen + (zalenEnabled ? ZalenChainBonus : 0)
                                 : BoardValue(id);
                    if (isSynchro)
                    {
                        synchroCount++;
                        if (RdaField.Negated(field[i])) value *= 0.3;
                    }
                    score += value;
                    if (details != null) Detail(details, RdaCards.Name(id), value);
                }
                // O tamanho da mesa conta só Synchros (monstros comuns expostos não fazem a mesa melhor).
                if (synchroCount > 0)
                {
                    score += FieldSizeBonus * synchroCount;
                    if (details != null) Detail(details, synchroCount + " Synchros on the field", FieldSizeBonus * synchroCount);
                }
                if (Array.IndexOf(state.Deck, RdaCards.Lubellion) >= 0)
                {
                    score += LubellionNotAccessed;
                    if (details != null) Detail(details, "Lubellion not accessed", LubellionNotAccessed);
                }
                // Linha RDA + Crimson Gaia (jogador): com batalha e 2+ monstros do oponente, o RDA ataca, a Gaia vira a mesa dele para
                // defesa e o RDA destrói todos; os outros monstros atacam depois e não somem no End Phase. Trocar o RDA pela Lubellion
                // nesse turno não compensa (teste de 2026-09-15).
                bool gaiaWipe = rda && state.BattleWipeReady && !state.GaiaNegated
                    && Array.IndexOf(state.Spells, RdaCards.Gaia) >= 0
                    && field.Any(m => RdaField.Id(m) == RdaCards.Rda && !RdaField.Negated(m));
                if (gaiaWipe)
                {
                    score += RdaGaiaWipeBonus;
                    if (details != null) Detail(details, "RDA + Crimson Gaia wipe the opponent board in the battle", RdaGaiaWipeBonus);
                }
                // Soul Resonator no GY bane no lugar da destruição (protege todos de uma vez): sem perda.
                if (!gaiaWipe && rda && field.Length > 1 && Array.IndexOf(state.Grave, RdaCards.Soul) < 0)
                {
                    score -= 40;
                    if (details != null) Detail(details, "RDA destroys the other monsters in the End Phase", -40);
                }
                // RDA protegido (jogador, 2026-09-22): "o RDA sendo invocado pelo efeito 2 do crimson king também acaba
                // valendo mais do que invocado de outras formas, pois ali ele tem proteção de destruição dos efeitos do
                // oponente e ainda mais força". O valor base do RDA continua 8 de propósito — é o RDA comum, que ainda
                // destrói os nossos monstros na End Phase. O bônus é desta cópia específica, e a flag cai junto com ela
                // (ver RemoveField), então gastar o RDA protegido como material perde o bônus.
                bool protectedRdaOnField = state.HasFlag(RdaState.FlagProtectedRda)
                    && field.Any(m => RdaField.Id(m) == RdaCards.Rda && !RdaField.Negated(m));
                if (protectedRdaOnField)
                {
                    score += ProtectedRdaBonus;
                    if (details != null) Detail(details, "RDA protected by King ② (immune to destruction by effect)", ProtectedRdaBonus);
                }
                // Crimson Call no GY com o RDA no campo (jogador): quando o RDA ataca, a Call dá um segundo ataque a ele —
                // dano dobrado, e letal se o oponente estava sem monstros. Só vale num turno em que conseguimos atacar.
                if (state.SecondTurnOrLater && Array.IndexOf(state.Grave, RdaCards.CrimsonCall) >= 0
                    && field.Any(m => RdaField.Id(m) == RdaCards.Rda && !RdaField.Negated(m)))
                {
                    score += CrimsonCallSecondAttack;
                    if (details != null) Detail(details, "Crimson Call in the GY gives the RDA a second attack", CrimsonCallSecondAttack);
                }
                // RDA invocado por Synchro (ou tratado como) em qualquer momento do duelo libera o efeito do Burning Soul,
                // que pode vir do GY a qualquer momento (jogador): vale como recurso enquanto o Burning Soul está no Extra.
                if (state.HasFlag(RdaState.FlagRdaSynchro) && Array.IndexOf(state.Extra, RdaCards.BurningSoul) >= 0)
                {
                    score += 15;
                    if (details != null) Detail(details, "Burning Soul unlocked (RDA summoned this duel)", 15);
                }
                foreach (int spell in state.Spells)
                {
                    score += SpellValue(spell);
                    if (details != null) Detail(details, RdaCards.Name(spell) + " (on field)", SpellValue(spell));
                }
                // Etude of the Branded com um Bystial no campo (jogador: é o efeito que realmente importa): tributos e materiais
                // de Fusão/Synchro/Link do oponente são banidos em vez de irem ao GY.
                if (Array.IndexOf(state.Spells, RdaCards.Etude) >= 0 && state.Field.Any(m => !RdaField.Negated(m)
                    && (RdaField.Id(m) == RdaCards.DisPater || RdaField.Id(m) == RdaCards.Lubellion || RdaField.Id(m) == RdaCards.Magnamhut)))
                {
                    score += 20;
                    if (details != null) Detail(details, "Etude with a Bystial on the field (banishes opponent materials)", 20);
                }

                bool redZoneSeenInHand = false, harmoniaSeenInHand = false;
                int[] hand = state.Hand;
                var countedCopies = new Dictionary<int, int>();
                for (int i = 0; i < hand.Length; ++i)
                {
                    int id = hand[i];
                    bool gained = IsGained(hand, i, rootHand);
                    double value = TrapValue(id, hypernova);
                    if (value < 0)
                    {
                        double handValue;
                        if (HandValue.TryGetValue(id, out handValue)) value = handValue;
                    }
                    if (value < 0)
                    {
                        // Peça de combo que sobrou na mão: só conta se o plano a ganhou. O que veio da mão inicial e
                        // ficou lá não separa um plano do outro.
                        if (!gained) continue;
                        value = 1;
                    }
                    else
                    {
                        // Carta de interação (handtrap ou armadilha): conta esteja ela na mão desde o início ou não.
                        // Antes só contavam as cartas GANHAS, então um plano que queimava a Ash da mão inicial como
                        // custo tirava a mesma nota de um que a guardava — a interação saía de graça (jogador,
                        // 2026-09-22). Cópias repetidas valem menos: a segunda Ash quase nunca é usada no mesmo turno.
                        int alreadyCounted;
                        countedCopies.TryGetValue(id, out alreadyCounted);
                        countedCopies[id] = alreadyCounted + 1;
                        value *= alreadyCounted == 0 ? 1.0 : alreadyCounted == 1 ? 0.5 : 0.25;
                    }
                    if (id == RdaCards.RedZone) redZoneSeenInHand = true;
                    if (id == RdaCards.Harmonia) harmoniaSeenInHand = true;
                    score += value;
                    if (details != null) Detail(details, RdaCards.Name(id) + (gained ? " (gained)" : " (kept)"), value);
                }

                bool endPhaseHarmonia = state.HasFlag(RdaState.FlagMagnamhutEndPhase) && Array.IndexOf(state.Deck, RdaCards.Harmonia) >= 0
                    && !state.HasFlag(RdaState.FlagNoDeckAdd); // com Droll a busca do End Phase (mesmo turno) não acontece
                bool harmonia = harmoniaSeenInHand || endPhaseHarmonia;
                bool stormBaneInExtra = Array.IndexOf(state.Extra, RdaCards.StormBane) >= 0;
                // As três vias de retorno valem para os DOIS novas (jogador, 2026-09-16): Red Zone revive, Dis Pater traz do
                // banimento e o Storm-Bane devolve Dragão banido — nada disso é exclusivo do Hypernova. Antes só ele recebia,
                // o que tirava até 120 pontos das mesas de Supernova e explicava por que elas ficavam em 275-323 contra
                // 396-457. A preferência pelo Hypernova continua, e vem de onde deve: 100 contra 70 no FieldValue.
                bool nova = hypernova || supernova;
                string novaName = hypernova ? "Hypernova" : "Supernova";
                if (nova)
                {
                    if (redZoneSeenInHand || redZoneOnField) { score += 40; if (details != null) Detail(details, novaName + " comes back with Red Zone", 40); }
                    if (disPater) { score += 40; if (details != null) Detail(details, novaName + " comes back with Dis Pater", 40); }
                    if (harmonia && stormBaneInExtra) { score += 40; if (details != null) Detail(details, novaName + " comes back with Harmonia + Storm-Bane", 40); }
                }
                else if (harmonia && stormBaneInExtra)
                {
                    score += 10;
                    if (details != null) Detail(details, "Harmonia + Storm-Bane", 10);
                }
                if (endPhaseHarmonia)
                {
                    score += 25;
                    if (details != null) Detail(details, "Magnamhut searches Harmonia in the End Phase", 25);
                }
                // Burning Soul que adicionou carta do GY: +2000 de ATK e um recurso para o próximo turno.
                if (state.Uses(RdaKey.BurningSoulAdd))
                {
                    score += BurningSoulAddBonus;
                    if (details != null) Detail(details, "Burning Soul added a card (+2000 ATK)", BurningSoulAddBonus);
                }
                // Soul Resonator no GY: se algo nosso for destruído por efeito, bane o Soul no lugar, desde que
                // controlemos RDA ou um Synchro que o mencione. Só vale nota se essa condição existir no campo final.
                if (Array.IndexOf(state.Grave, RdaCards.Soul) >= 0 &&
                    state.Field.Any(m => RdaCards.RdaLike.Contains(RdaField.Id(m)) || (RdaCards.Get(RdaField.Id(m)).Synchro && RdaCards.Get(RdaField.Id(m)).MentionsRda)))
                {
                    score += SoulGraveProtection;
                    if (details != null) Detail(details, "Soul Resonator in the GY protects the field", SoulGraveProtection);
                }
                return score;
            }

            /// <summary>Nota neutra do campo como está (usada pela busca): bosses, recursão, magias/armadilhas, mão.</summary>
            public static double Evaluate(RdaState state, List<string> details = null)
            {
                double score = 0;
                long[] field = state.Field;
                bool rda = false, hypernova = false, supernova = false, disPater = false;
                // Uma vez só, fora do laço: esta função roda para dezenas de milhares de estados por busca.
                // Valor sensível ao contexto: sem isto a busca poda os ramos com o RDA no campo antes de a linha da
                // Gaia se formar, e a mesa boa nunca é encontrada (jogador, 2026-09-22).
                double bonusRda = GaiaLineLive(state) ? RdaGaiaLineFieldBonus : 0;
                for (int i = 0; i < field.Length; ++i)
                {
                    int id = RdaField.Id(field[i]);
                    double value = BoardValue(id);
                    if (id == RdaCards.Rda) value += bonusRda;
                    value *= RdaField.Negated(field[i]) ? 0.3 : 1;
                    score += value;
                    if (details != null) Detail(details, RdaCards.Name(id), value);
                    if (id == RdaCards.Rda) rda = true;
                    else if (id == RdaCards.Hypernova) hypernova = true;
                    else if (id == RdaCards.Supernova) supernova = true;
                    else if (id == RdaCards.DisPater) disPater = true;
                }
                if (field.Length > 0)
                {
                    score += FieldSizeBonus * field.Length;
                    if (details != null) Detail(details, field.Length + " monsters on the field", FieldSizeBonus * field.Length);
                }
                if (rda && field.Length > 1)
                {
                    score -= 40;
                    if (details != null) Detail(details, "RDA destroys the other monsters in the End Phase", -40);
                }

                // Red Hypernova pode banir o campo e o GY do oponente mais de uma vez se puder voltar das banidas.
                bool endPhaseHarmonia = state.HasFlag(RdaState.FlagMagnamhutEndPhase) && Array.IndexOf(state.Deck, RdaCards.Harmonia) >= 0
                    && !state.HasFlag(RdaState.FlagNoDeckAdd); // com Droll a busca do End Phase (mesmo turno) não acontece
                bool harmoniaAvailable = Array.IndexOf(state.Hand, RdaCards.Harmonia) >= 0 || endPhaseHarmonia;
                bool stormBaneInExtra = Array.IndexOf(state.Extra, RdaCards.StormBane) >= 0;
                // Mesma correção do SelectionScore: as vias de retorno valem para o Supernova também, senão a busca desvia
                // das linhas dele antes mesmo de a mesa ser avaliada.
                bool novaOnField = hypernova || supernova;
                string novaLabel = hypernova ? "Hypernova" : "Supernova";
                if (novaOnField)
                {
                    if (Array.IndexOf(state.Hand, RdaCards.RedZone) >= 0 || Array.IndexOf(state.Spells, RdaCards.RedZone) >= 0)
                    { score += 40; if (details != null) Detail(details, novaLabel + " can come back with Red Zone", 40); }
                    if (disPater) { score += 40; if (details != null) Detail(details, novaLabel + " can come back with Dis Pater", 40); }
                    if (harmoniaAvailable && stormBaneInExtra) { score += 40; if (details != null) Detail(details, novaLabel + " can come back with Harmonia + Storm-Bane", 40); }
                }
                else if (harmoniaAvailable && stormBaneInExtra)
                {
                    score += 10;
                    if (details != null) Detail(details, "Harmonia + Storm-Bane available", 10);
                }

                foreach (int spell in state.Spells)
                {
                    score += SpellValue(spell);
                    if (details != null) Detail(details, RdaCards.Name(spell), SpellValue(spell));
                }

                // Armadilhas da mão que dá para baixar (as de maior valor, limitadas pelas zonas livres).
                int[] hand = state.Hand;
                int trapCount = 0;
                double trapSum = 0;
                for (int i = 0; i < hand.Length; ++i)
                {
                    double value = TrapValue(hand[i], hypernova);
                    if (value >= 0) { trapCount++; trapSum += value; }
                }
                int free = state.SpellZonesFree;
                if (trapCount <= free && details == null)
                {
                    score += trapSum;
                }
                else if (trapCount > 0)
                {
                    var values = new double[trapCount];
                    var ids = new int[trapCount];
                    int k = 0;
                    for (int i = 0; i < hand.Length; ++i)
                    {
                        double value = TrapValue(hand[i], hypernova);
                        if (value >= 0) { values[k] = value; ids[k] = hand[i]; k++; }
                    }
                    Array.Sort(values, ids);
                    for (int i = trapCount - 1, taken = 0; i >= 0 && taken < free; --i, ++taken)
                    {
                        score += values[i];
                        if (details != null) Detail(details, RdaCards.Name(ids[i]) + " (set)", values[i]);
                    }
                }

                for (int i = 0; i < hand.Length; ++i)
                {
                    int id = hand[i];
                    if (TrapValue(id, hypernova) >= 0)
                        continue;
                    double value;
                    if (!HandValue.TryGetValue(id, out value)) value = 1;
                    score += value;
                    if (details != null) Detail(details, RdaCards.Name(id) + " (hand)", value);
                }
                if (endPhaseHarmonia)
                {
                    score += 25;
                    if (details != null) Detail(details, "Magnamhut searches Harmonia in the End Phase", 25);
                }
                // Burning Soul que adicionou carta do GY: +2000 de ATK e um recurso para o próximo turno.
                // Na nota neutra (que guia a busca) o bônus fica em 0: desde que o Synchro do Scarred libera o efeito,
                // o bônus aqui enchia o feixe de linhas com Burning Soul e escondia o Hypernova. Ele vale só na seleção.
                if (state.Uses(RdaKey.BurningSoulAdd) && NeutralBurningSoulAddBonus != 0)
                {
                    score += NeutralBurningSoulAddBonus;
                    if (details != null) Detail(details, "Burning Soul added a card (+2000 ATK)", NeutralBurningSoulAddBonus);
                }
                return score;
            }

            /// <summary>Quanto o estado ainda pode render (só para ordenar a busca em feixe).</summary>
            public static double Potential(RdaState state)
            {
                if (state.NoSpecial) return 0;
                double score = 6 * state.NormalSummon + 6 * state.ExtraNormalSummon;
                foreach (int id in state.Hand)
                {
                    double value;
                    score += RdaCards.Resonators.Contains(id) ? 6 : HandPotential.TryGetValue(id, out value) ? value : 0;
                }
                foreach (long monster in state.Field)
                {
                    RdaCardInfo card = RdaCards.Get(RdaField.Id(monster));
                    if (card.Synchro) score += card.Level <= 10 ? 5 : 0;
                    else score += RdaCards.Tuner(state, RdaField.Id(monster)) ? 7 : 5;
                }
                if (Array.IndexOf(state.Grave, RdaCards.Bone) >= 0 && !state.Uses(RdaKey.BoneSS)) score += 6;
                if (Array.IndexOf(state.Grave, RdaCards.Lubellion) >= 0 && !state.Uses(RdaKey.LubellionSS)) score += 5;
                foreach (int id in state.Grave)
                    if (RdaCards.Resonators.Contains(id)) score += 2;
                if (state.Controls(RdaCards.Crimson) && !state.Uses(RdaKey.CrimsonEffect)) score += 12;
                if (state.Controls(RdaCards.Darkness) && !state.Uses(RdaKey.DarknessLevel)) score += 6;
                if (state.Controls(RdaCards.Bone) && !state.Uses(RdaKey.BoneLevel)) score += 6;
                if (state.Controls(RdaCards.DisPater) && !state.Uses(RdaKey.DisPaterEffect)) score += 8;
                if (state.Controls(RdaCards.Lubellion) && !state.Uses(RdaKey.LubellionPlace)) score += 6;
                if (Array.IndexOf(state.Spells, RdaCards.Gaia) >= 0 && !state.Uses(RdaKey.GaiaSearch)) score += 8;
                // HandSynergy roda para cada nó da busca (LINQ com lambdas no laço mais quente) e hoje o peso é 0, então o
                // produto é sempre zero. O campo continua existindo para calibrar fora do jogo por reflexão; com peso 0 a
                // chamada é pulada. Medição de 2026-09-16: a etapa 'evaluate' consome 27% da CPU da busca.
                if (PotentialSynergyWeight > 0)
                    score += PotentialSynergyWeight * HandSynergy(state);
                return score;
            }

            // Peso das combinações da mão que abrem o combo (static para calibrar fora do jogo por reflexão).
            // Teste contra o Albaz (23:19): a linha "Gaia busca Soul" até o Hypernova ficava parada em nota 75 entre as
            // profundidades 9 e 13 (Magnamhut + Lubellion, Darkness e Power Vice guardados na mão) e saía do feixe.
            public static double PotentialSynergyWeight = 0.0; // 0 = desligado (testes com 1-3 não seguraram a linha do Hypernova)

            private static double HandSynergy(RdaState state)
            {
                double bonus = 0;
                // Darkness Resonator na mão com RDA no Extra: se invoca e ainda dá uma Invocação-Normal extra de Resonator.
                if (Array.IndexOf(state.Hand, RdaCards.Darkness) >= 0 && !state.Uses(RdaKey.DarknessSS) && Array.IndexOf(state.Extra, RdaCards.Rda) >= 0)
                    bonus += 10;
                // Power Vice na mão com a condição de invocação já cumprida (campo vazio ou só Synchros DARK).
                if (Array.IndexOf(state.Hand, RdaCards.PowerVice) >= 0 && !state.Uses(RdaKey.PowerViceSS) && state.Field.Length > 0
                    && state.Field.All(m => RdaCards.Get(RdaField.Id(m)).Synchro && RdaCards.Get(RdaField.Id(m)).Attribute == CardAttribute.Dark))
                    bonus += 8;
                // Magnamhut na mão com a Lubellion na mão/GY: Magnamhut se invoca, a Lubellion libera ela, coloca o Etude e vira
                // material nível 8 (Dis Pater).
                if (Array.IndexOf(state.Hand, RdaCards.Magnamhut) >= 0 && !state.Uses(RdaKey.MagnamhutSS) && !state.Uses(RdaKey.LubellionSS)
                    && (Array.IndexOf(state.Hand, RdaCards.Lubellion) >= 0 || Array.IndexOf(state.Grave, RdaCards.Lubellion) >= 0))
                    bonus += 10;
                return bonus;
            }
        }

        /// <summary>Um passo do plano: a ação e o estado previsto logo depois dela.</summary>
        private sealed class PlanStep
        {
            public PlanAction Action;
            public RdaState After;
        }

        /// <summary>Resultado do planejamento.</summary>
        private sealed class RdaPlan
        {
            public double Score;          // nota de seleção da mesa final
            public int Tier;              // camada da mesa final (0 = ideal com Hypernova ... 3 = outra)
            public bool RouteComplete;    // rota de referência seguida até o último passo (mesa que o jogador montou)
            public string Search;         // busca que encontrou o plano
            public double Exposure;       // monstros soltos no campo somados ao longo da linha (menor = menos frágil)
            public int DrawEvents;        // Invocações-Especiais que dão carta ao oponente (Maxx "C"/Fuwalos ativos)
            public int RouteDrawEvents;   // dessas, as que vêm de uma rota de referência: o jogador já aceitou o custo
            public double NibiruPenalty;  // invocações a partir do 5º monstro feitas sem resposta ao Nibiru no campo
            public string BaitNote;       // isca escolhida por BaitFirst (null = nenhuma)
            public long BestFoundMs;      // medição: quando a melhor mesa desta busca apareceu
            public int BestFoundDepth;
            public long SuccessorsMs, NodesMs, EvaluateMs, SortMs, Expanded, Generated; // medição por etapa da busca
            // Mesma medição somada sobre TODAS as buscas da carteira, não só a que venceu. É isto que sai no log da
            // partida: a busca vencedora sozinha não explica o tempo, porque elas rodam em paralelo e o tempo de parede
            // é o da mais lenta. Preenchido em PlanPortfolio, logo antes do retorno.
            public long PoolSuccessorsMs, PoolNodesMs, PoolEvaluateMs, PoolSortMs, PoolExpanded, PoolGenerated;
            public int PoolCount;
            // Maior CPU de uma única busca do pool. Como as buscas rodam em paralelo, o tempo de parede não pode ser
            // menor que esta: o que passar disso foi gasto FORA da busca em feixe (encaixe de rotas, escolha por
            // resistência, abertura alternativa). É essa diferença que diz se vale otimizar a busca ou o resto.
            public long PoolMaxSearchMs;
            // Fases da carteira, em tempo de PAREDE. Medido em partida real (18/09): nos planos acima de 6 s, metade do
            // tempo estava FORA da busca em feixe, e estas fases são o que há lá. Elas rodam em sequência.
            public long PhaseGuidedMs;     // encaixe das rotas de referência + busca curta que as completa
            public long PhaseSearchMs;    // as buscas da carteira (em paralelo entre si)
            public long PhaseForcedMs;    // abertura alternativa
            public long PhaseResilientMs; // escolha por resistência
            public long PhaseShortcutMs;  // decidir se o atalho vale (plano B de cada rota ideal)
            public long PhaseSeedsMs;     // busca a partir das sementes por abertura
            public bool NovaUnprotected;  // Hypernova/Supernova invocado sem proteção (não disputa a escolha por resistência)
            public string ResilienceNote; // escolha por resistência do starter (null = não houve disputa)
            public readonly List<RdaPlan> Alternatives = new List<RdaPlan>(); // outros finais bons da mesma busca
            public readonly List<RdaPlan> Seeds = new List<RdaPlan>();        // começos de linha por abertura (sementes)
            public bool NibiruExposed;    // alguma dessas invocações sem resposta nem reconstrução
            public RdaState Root;
            public RdaState Final;
            public readonly List<PlanStep> Steps = new List<PlanStep>();
            public long ElapsedMs;
            public int Depth;
        }

        /// <summary>Configuração de uma busca em feixe (validada nas baterias de handbook/planner/experiments).</summary>
        private sealed class SearchOptions
        {
            public string Name;
            public int Width;              // estados mantidos por passo
            public int PerRoute;           // vagas reservadas por rota (0 = sem diversidade); rota = cartas restantes no Extra
            // Vagas por rota separadas também pela abertura (1º efeito crítico e a carta escolhida). Teste contra o Albaz (23:19):
            // a linha "Gaia busca Soul" chega ao Hypernova (camada 0), mas gasta 6 passos antes do 1º Synchro e saía do feixe
            // para as linhas do Power Vice, que tinham a mesma assinatura de Extra.
            public bool OpeningDiversity = false; // experimento: ligado só pelos testes offline (reflexão)
            // Fração do feixe reservada à diversidade por rota (o resto é completado pela prioridade). Com 1,0 a primeira passada
            // podia encher o feixe inteiro com estados fracos de rotas raras e expulsar estados bons de uma rota cheia (teste
            // contra o Albaz, 23:19: a linha do Hypernova estava na posição 908 de 15775 e ficou fora do feixe de 3000).
            public double DiversityShare = 1.0;
            // Guarda, por abertura, o estado de maior prioridade que acabou de fazer o SeedSynchroCount-ésimo Synchro
            // (sementes para buscas curtas depois; ver SeededPlans).
            public bool CollectSeeds;
            public double BookWeight;      // bônus por Synchro que segue uma rota do livro (combos do jogador)
            public double Noise;           // sorteio [0, Noise) somado à prioridade (explora outros caminhos)
            public int Seed;
            // Não aceita Hypernova/Supernova sem proteção (Zalen, Abyss, Dis Pater ou King) no campo.
            // Só uma busca da carteira usa isso: restringir todas desviaria rotas (lição da bateria 11).
            public bool ForbidUnprotectedNova;
            public bool ForbidNibiruExposed = false;  // descarta caminhos que invocam do 5º monstro em diante sem resposta ao Nibiru
            public int MaxDepth = 90;
            // Limite de segurança: medido fora do jogo, a carteira leva 10-15 s em paralelo; cortar antes piora a mesa.
            public int TimeLimitMs = 25000;
            // Token de cancelamento do chamador (P4): a busca em segundo plano do turno clona as opções com um token próprio;
            // quando a tarefa é cancelada, os Plan conferem Token nos pontos de checagem e param cedo. Buscas síncronas
            // (replanejamento, testes offline) deixam CancellationToken.None e nunca são canceladas. As opções da carteira
            // são estáticas e compartilhadas entre busca de fundo e replanejamento, então o Token nunca é gravado nelas:
            // quem precisa dele clona com WithToken. Não usar Token como identidade da busca.
            public CancellationToken Token;

            // Cópia rasa com token de cancelamento desta chamada, sem tocar nos SearchOptions estáticos compartilhados.
            // Ao adicionar um campo novo em SearchOptions, atualizar este clone.
            public SearchOptions WithToken(CancellationToken value)
            {
                var copy = new SearchOptions
                {
                    Name = Name, Width = Width, PerRoute = PerRoute, OpeningDiversity = OpeningDiversity,
                    DiversityShare = DiversityShare, CollectSeeds = CollectSeeds, BookWeight = BookWeight,
                    Noise = Noise, Seed = Seed, ForbidUnprotectedNova = ForbidUnprotectedNova,
                    ForbidNibiruExposed = ForbidNibiruExposed, MaxDepth = MaxDepth, TimeLimitMs = TimeLimitMs,
                    Token = value
                };
                return copy;
            }
        }

        /// <summary>Busca em feixe sobre as regras do modelo.</summary>
        private static class RdaPlanner
        {
            // Livro de rotas: sequências de Synchro dos combos do jogador (replays Soul, Bone e Power Vice).
            // O bônus só evita que essas rotas saiam do feixe cedo demais; a busca continua livre para sair delas.
            private static readonly int[][] RouteBook =
            {
                new[] { RdaCards.RedRising, RdaCards.Blade, RdaCards.King, RdaCards.DisPater, RdaCards.RedRising, RdaCards.Zalen, RdaCards.Hypernova, RdaCards.Quetzacoatl },
                new[] { RdaCards.King, RdaCards.Abyss, RdaCards.RedRising, RdaCards.Blade, RdaCards.DisPater, RdaCards.Supernova, RdaCards.Quetzacoatl },
                new[] { RdaCards.King, RdaCards.Abyss, RdaCards.RedRising, RdaCards.Blade, RdaCards.DisPater, RdaCards.Hypernova, RdaCards.Quetzacoatl },
            };
            private const int AllRoutes = (1 << 3) - 1;
            // Margem (nota neutra) para calcular a nota de seleção de um estado da melhor camada. As duas notas diferem
            // por cartas intocadas na mão e pelas preferências (Zalen/Abyss/Lubellion), bem menos que isso.
            private const double SelectionMargin = 150;

            private sealed class Node
            {
                public RdaState State;
                public Node Parent;
                public PlanAction Action;
                public int SynchroCount;   // Synchros feitos na linha até aqui
                public int RouteMask;      // rotas do livro que a linha ainda segue (bit por rota)
                public bool NovaUnprotected; // Hypernova/Supernova invocado sem proteção (Zalen, Abyss, Dis Pater, King) no campo
                public double Exposure;      // soma, passo a passo, dos monstros "soltos" no campo (ver StepExposure)
                public int Summoned;         // monstros invocados no turno até aqui (NS + SS)
                public int DrawEvents;       // eventos de Invocação-Especial que dão carta ao oponente
                public double NibiruPenalty; // risco de Nibiru somado nas invocações a partir do 5º monstro
                public bool NibiruExposed;   // invocou do 5º monstro em diante sem resposta nem reconstrução (perde 1 camada)
                public bool CriticalSeen;    // a linha já ativou um efeito crítico que handtrap para (busca/invocação do Deck)
                public bool BaitLate;        // Resonator Call (isca) ativada depois de um efeito crítico
                public PlanAction FirstCritical; // starter da linha: 1º efeito crítico que não é isca
            }

            // Maxx "C"/Fuwalos: cada evento de Invocação-Especial dá 1 carta ao oponente. Nesse modo a camada não
            // manda mais (a mesa ideal pode custar muitas cartas): vale só a nota, e parar mais cedo é permitido.
            // Calibrado na mão do Soul com Maxx "C" desde o início: 20 fazia o combo inteiro (16 cartas dadas),
            // 50 não fazia nada; 35 fica com Hypernova + King dando 8 cartas.
            private static double DrawPenalty = 35;
            // Nibiru: a partir do 5º monstro invocado o oponente pode usá-lo a qualquer momento (o jogo dá prioridade a
            // ele depois de cada invocação). Cada invocação feita sem resposta no campo (Dis Pater com banida,
            // Quetzacoatl com outro Dragão Synchro, Zalen com King) custa esse peso; só o King (traz o RDA) vale metade.
            // Com proteção no campo o Nibiru é fácil de lidar, então a linha é empurrada a montar a proteção cedo.
            private static double NibiruPenaltyWeight = 8;
            // Maxx "C"/Fuwalos: quanto menos cartas o oponente recebe, maior a chance de vitória (jogador). Uma mesa mínima
            // com proteção (ex.: Crimson King + King's Resonance baixada + handtraps na mão) vale mais que o combo grande.
            private const double MaxxProtectionBonus = 40;
            private const double MaxxLubellionRefund = 30;   // sem buscar a Lubellion não é defeito quando o objetivo é invocar pouco
            // Custo extra por carta dada ao oponente sob Maxx "C"/Fuwalos, somado aos 35 do PathCost (static para calibrar fora do
            // jogo com handbook\tests\teste_maxxc.ps1: 0 reproduz o comportamento antigo).
            // Calibrado em 2026-09-15 com as mãos carregadas e com o estado real do turno 12 da partida perdida:
            //   0  = linha gulosa: todas as mãos davam 2 cartas ao oponente;
            //   30 = só passa a linha de maior valor por carta (Foolish, 10 passos); as outras param de invocar;
            //   50 ou 85 = nenhuma linha invoca (proibição total).
            public static double MaxxExtraDrawPenalty = 30;

            private static double NibiruRisk(RdaState state)
            {
                bool disPater = false, quetzacoatl = false, zalen = false, king = false;
                int synchros = 0;
                foreach (long monster in state.Field)
                {
                    if (RdaField.Negated(monster))
                        continue;
                    int id = RdaField.Id(monster);
                    if (RdaCards.Get(id).Synchro) synchros++;
                    if (id == RdaCards.DisPater) disPater = true;
                    else if (id == RdaCards.Quetzacoatl) quetzacoatl = true;
                    else if (id == RdaCards.Zalen) zalen = true;
                    else if (id == RdaCards.King) king = true;
                }
                bool anyBanished = state.Banished.Length > 0 || state.EnemyBanished.Length > 0 || state.EnemyBanishedOther > 0;
                if ((disPater && anyBanished) || (quetzacoatl && synchros >= 2) || (zalen && king))
                    return 0;
                // Reconstrução depois da limpeza: King ② traz o RDA; ou Burning Soul pelo GY (2 Tuners + RDA no GY).
                bool rdaInGrave = false;
                int tunersInGrave = 0;
                foreach (int id in state.Grave)
                {
                    if (RdaCards.RdaLike.Contains(id)) rdaInGrave = true;
                    RdaCardInfo info = RdaCards.Get(id);
                    if (info != null && info.Tuner) tunersInGrave++;
                }
                bool burningSoulRebuild = rdaInGrave && tunersInGrave >= 2 && Array.IndexOf(state.Extra, RdaCards.BurningSoul) >= 0;
                return king || burningSoulRebuild ? NibiruPenaltyWeight * 0.5 : NibiruPenaltyWeight;
            }

            // Quantos monstros novos entraram no campo neste passo e quantos vieram do Deck/Extra.
            // Roda para todo estado da busca: sem LINQ nem alocação (a versão com LINQ fazia a busca bater no limite de tempo).
            private static void CountNewMonsters(RdaState before, RdaState after, out int total, out int fromDeckOrExtra)
            {
                // Maxx "C" dá carta em TODA Invocação-Especial, e o Synchro faz o campo encolher (2 materiais saem, 1 entra).
                // O atalho antigo saía cedo quando o campo não crescia, então Synchros e invocações que trocam monstro não eram
                // contadas (jogador, 2026-09-15: "ele sempre vai dar cartas ao oponente"). Agora só o campo vazio sai cedo.
                total = 0;
                fromDeckOrExtra = 0;
                long[] field = after.Field;
                if (field.Length == 0 || ReferenceEquals(field, before.Field))
                    return;
                for (int i = 0; i < field.Length; ++i)
                {
                    int id = RdaField.Id(field[i]);
                    // Cada id é contado uma vez: só na primeira posição em que aparece.
                    bool seen = false;
                    for (int j = 0; j < i && !seen; ++j)
                        seen = RdaField.Id(field[j]) == id;
                    if (seen)
                        continue;
                    int gained = CountId(field, id) - CountId(before.Field, id);
                    if (gained <= 0)
                        continue;
                    total += gained;
                    if (CountId(after.Extra, id) < CountId(before.Extra, id) || CountId(after.Deck, id) < CountId(before.Deck, id))
                        fromDeckOrExtra += gained;
                }
            }

            private static int CountId(long[] field, int id)
            {
                int count = 0;
                for (int i = 0; i < field.Length; ++i)
                    if (RdaField.Id(field[i]) == id) count++;
                return count;
            }

            private static int CountId(int[] cards, int id)
            {
                int count = 0;
                for (int i = 0; i < cards.Length; ++i)
                    if (cards[i] == id) count++;
                return count;
            }

            // Abertura da linha para a diversidade: efeito crítico inicial + carta escolhida (Gaia→Soul ≠ Gaia→Power Vice).
            private static int OpeningKey(Node node)
            {
                PlanAction first = node.FirstCritical;
                if (first == null)
                    return 0;
                int pick = first.Picks.Count > 0 ? first.Picks[0].Id : first.CardId;
                return unchecked((int)first.Effect * 31 + pick + 1);
            }

            // Teste offline (por reflexão): estados de uma linha esperada. A busca anota em que posição cada um ficou, a nota de
            // corte do feixe e se foi mantido. Nulo no jogo (sem custo).
            public static HashSet<RdaState> TraceStates = null; // teste offline (reflexão)
            public static readonly List<string> TraceLog = new List<string>();

            // Teste offline (por reflexão): conta quantas vezes cada estado teve as jogadas geradas, somando as buscas paralelas.
            // Mede quanto um cache compartilhado de jogadas economizaria. Nulo no jogo.
            public static System.Collections.Concurrent.ConcurrentDictionary<RdaState, int> ExpansionCounter = null; // teste offline (reflexão)

            // Teto global de buscas Plan concorrentes (P4). Antes, um replanejamento síncrono durante a primeira busca do
            // turno em segundo plano podia deixar a carteira (3 buscas) + rotas + iscas + resistência rodando ao mesmo tempo:
            // até 10+ Plans competindo por CPU, cada um gastando o orçamento inteiro. O gate limita quantos Plans rodam de
            // verdade; quem chega depois espera no Monitor e leave faz PulseAll. Não há deadlock por reentrada porque Plan
            // é folha (não chama outro Plan dentro dele); os helpers paralelos chamam Plan e cada chamada entra/sai do
            // gate por conta própria. Uma busca já cancelada que espera na fila é inofensiva: ao entrar, o primeiro ponto
            // de checagem aborta em seguida e devolve a vaga.
            private static readonly object SearchGateLock = new object();
            private static int SearchGatePermits = System.Environment.ProcessorCount; // teste offline: reduz por reflexão
            private static int SearchGateActive;
            public static int SearchGateInUse
            {
                get { lock (SearchGateLock) { return SearchGateActive; } }
            }

            // Espera por uma vaga (quem cancelou a busca só sai esperando: ao entrar, o primeiro ponto de checagem do Plan
            // vê o Token cancelado e devolve o melhor-so-far em seguida). Espera em blocos de 50 ms; leave faz PulseAll.
            private static void SearchGateEnter()
            {
                lock (SearchGateLock)
                {
                    while (SearchGateActive >= SearchGatePermits)
                        Monitor.Wait(SearchGateLock, 50);
                    SearchGateActive++;
                }
            }

            private static void SearchGateLeave()
            {
                lock (SearchGateLock)
                {
                    SearchGateActive--;
                    Monitor.PulseAll(SearchGateLock);
                }
            }

            private static double PathCost(Node node)
            {
                return ExposureWeight * node.Exposure + node.NibiruPenalty + DrawPenalty * node.DrawEvents
                    + (node.BaitLate ? BaitLatePenalty : 0);
            }

            // Isca primeiro (regra do jogador): Resonator Call é ativada antes de comprometer a linha, para gastar a
            // handtrap do oponente numa busca que não segura o combo. Efeitos críticos = os que uma Ash/Droll/Veiler para
            // e que sustentam a rota. Só muda a ordem entre caminhos que chegam ao mesmo estado e a escolha final.
            // Testado com 15: não mudou a ordem (mão RC + Soul + PV continuava abrindo com o Soul) e poderia favorecer linhas
            // sem a isca. Zerada; a regra da isca é aplicada por BaitFirst depois que o plano é escolhido.
            private static double BaitLatePenalty = 0; // static para teste offline por reflexão
            private static readonly HashSet<RdaKey> CriticalEffects = new HashSet<RdaKey>
            {
                RdaKey.SoulSearch, RdaKey.PowerViceSearch, RdaKey.CrimsonEffect, RdaKey.ChainSummon, RdaKey.BoneLevel,
                RdaKey.KingSearch, RdaKey.BladeTake, RdaKey.CrimsonCall, RdaKey.GaiaSearch, RdaKey.LubellionSearch,
                RdaKey.StoneSweeperSearch
            };

            // ------------------------------------------------------------------ isca primeiro
            // Regra do jogador: cartas que buscam/mandam algo para iniciar o combo (Stone Sweeper, Resonator Call, Lubellion,
            // Crimson Gaia se não fizer falta, Foolish Burial) servem de isca. Usa UMA delas antes do primeiro efeito crítico
            // para forçar a handtrap do oponente; não adianta gastar todas antes. A escolhida é a que, se for negada,
            // deixa a melhor mesa possível (replanejamento rápido já contando com a negação).
            private static readonly SearchOptions BaitFallbackSearch =
                new SearchOptions { Name = "bait_fast", Width = 400, PerRoute = 20, BookWeight = 15, TimeLimitMs = 1500 };

            public static bool IsBaitAction(PlanAction action)
            {
                if (action == null || action.Kind != PlanKind.Activate)
                    return false;
                // Só conta como isca o que uma handtrap tipo Ash para: buscar/mandar/invocar do Deck. No teste a Crimson Gaia
                // pegando carta do GY foi tratada como isca "resolvida sem resposta", mas o oponente nem podia responder a ela.
                if (!action.Picks.Any(pick => pick.Location == CardLocation.Deck))
                    return false;
                return action.CardId == RdaCards.ResonatorCall || action.CardId == RdaCards.Foolish
                    || action.Effect == RdaKey.StoneSweeperSearch || action.Effect == RdaKey.LubellionSearch || action.Effect == RdaKey.GaiaSearch;
            }

            public static bool IsCriticalAction(PlanAction action)
            {
                return (action.Kind == PlanKind.Activate || action.Kind == PlanKind.TriggerAccept) && CriticalEffects.Contains(action.Effect)
                    && !IsBaitAction(action);
            }

            // Estado depois de um efeito ser negado: custo pago, efeito marcado como usado, gatilhos que ele causaria somem,
            // e o que ele buscaria/mandaria/invocaria volta para onde estava (Deck ou mão).
            private static RdaState NegateBait(RdaState resolved, PlanAction action)
            {
                RdaState negated = resolved.Copy();
                negated.Pending = new int[0];
                foreach (RdaPick pick in action.Picks)
                {
                    if (pick.Location != CardLocation.Deck && pick.Location != CardLocation.Hand)
                        continue;
                    bool moved = false;
                    if (pick.Location == CardLocation.Deck && Array.IndexOf(negated.Hand, pick.Id) >= 0)
                    {
                        negated.Hand = RdaArray.Remove(negated.Hand, pick.Id);
                        moved = true;
                    }
                    else if (pick.Location == CardLocation.Deck && Array.IndexOf(negated.Grave, pick.Id) >= 0)
                    {
                        negated.Grave = RdaArray.Remove(negated.Grave, pick.Id);
                        moved = true;
                    }
                    else
                    {
                        long onField = negated.Field.FirstOrDefault(m => RdaField.Id(m) == pick.Id);
                        if (onField != 0)
                        {
                            negated.Field = RdaArray.Remove(negated.Field, onField);
                            moved = true;
                        }
                    }
                    if (!moved)
                        continue;
                    if (pick.Location == CardLocation.Deck)
                        negated.Deck = RdaArray.Add(negated.Deck, pick.Id);
                    else
                        negated.Hand = RdaArray.Add(negated.Hand, pick.Id);
                }
                return negated;
            }

            // ------------------------------------------------------------------ resistência do starter
            // Regra do jogador: entre rotas de mesas próximas (mesma camada), prefere a que continua boa se o starter for
            // negado — reserva na mão, Invocação-Normal ainda livre e Extra Deck sem trava aparecem sozinhos no
            // replanejamento depois da negação. Espera-se ~4 negações do oponente (Ash, Impermanence, Dominus...).
            // A mesa ideal continua vencendo uma mesa pior (só disputa quem está na mesma camada e perto da melhor nota).
            // Valor esperado (jogador, 2026-09-14): o oponente costuma ter ~4 negações (Ash, Impermanence, até 2 Dominus, mais
            // se jogamos em segundo). Se o Soul é negado ele fica no campo e impede o Power Vice, travando tudo; se o Power
            // Vice é negado, o Soul ainda chega ao Crimson King. Por isso a comparação vale ENTRE camadas:
            //   valor = (1 - chance) x nota da mesa sem negação + chance x nota do que sobra com o starter negado.
            private static int ResilienceContenders = 4; // static: ajustado pelo modo rápido e pelos testes por reflexão
            internal static int ResilienceContenderCount { get { return ResilienceContenders; } }
            private static double ResilienceWeight = 0.5;       // static para teste offline; 0 desliga a escolha
            private static double StarterNegationChance = 0.5;  // chance de o starter (1º efeito crítico depois da isca) ser negado

            private static readonly SearchOptions StarterAlternativeSearch = new SearchOptions
            {
                // Largura 700 / 2,5 s: nas 3 mãos de teste de starter (Soul+PV sem Bone, Soul+Bone+PV, replay do PV) a escolha
                // de abertura foi a mesma que com 1000 / 4 s, gastando menos. Com 1500 / 6 s a decisão chegava a 34 s.
                Name = "alternative_opening", Width = 700, PerRoute = 20, BookWeight = 15, TimeLimitMs = 2500, ForbidUnprotectedNova = true
            };

            // Replaneja proibindo a abertura do melhor plano (a jogada que leva ao starter), para achar uma rota com outro starter.
            // Obs. (P4): esta fase não é cancelável — a assinatura é chamada por testes offline via reflexão com 3 argumentos,
            // então não pode ganhar um parâmetro de token; a busca interna fica limitada ao próprio TimeLimitMs (2,5 s).
            private static RdaPlan ForcedAlternative(RdaState root, RdaPlan best, ICollection<string> blocked)
            {
                if (root.Pending.Length != 0 || best.Steps.Count == 0 || ResilienceWeight <= 0
                    || root.HasFlag(RdaState.FlagMaxxC) || root.HasFlag(RdaState.FlagFuwalos))
                    return null;
                int critical = best.Steps.FindIndex(step => IsCriticalAction(step.Action));
                if (critical < 0)
                    return null;
                PlanStep opening = best.Steps.Take(critical + 1).FirstOrDefault(step => !IsBaitAction(step.Action)
                    && step.Action.Kind != PlanKind.TriggerAccept && step.Action.Kind != PlanKind.TriggerDecline);
                if (opening == null)
                    return null;
                int openingIndex = best.Steps.IndexOf(opening);
                // Iscas antes da abertura: a rota alternativa parte do estado depois delas (o bloqueio só vale na raiz).
                for (int i = 0; i < openingIndex; ++i)
                    if (!IsBaitAction(best.Steps[i].Action))
                        return null;
                RdaState start = openingIndex == 0 ? root : best.Steps[openingIndex - 1].After;
                if (start.Pending.Length != 0)
                    return null;
                var blockedMore = new HashSet<string>(blocked ?? new string[0]) { opening.Action.Text };
                RdaPlan alternative = Plan(start, StarterAlternativeSearch, blockedMore);
                if (alternative.Steps.Count == 0)
                    return null;
                if (openingIndex == 0)
                    return alternative;
                var joined = new RdaPlan { Score = alternative.Score, Tier = alternative.Tier, Search = alternative.Search, Root = root,
                    Final = alternative.Final, Depth = alternative.Depth, Exposure = alternative.Exposure, DrawEvents = alternative.DrawEvents,
                    NibiruPenalty = alternative.NibiruPenalty, NibiruExposed = alternative.NibiruExposed, NovaUnprotected = alternative.NovaUnprotected };
                joined.Steps.AddRange(best.Steps.Take(openingIndex));
                joined.Steps.AddRange(alternative.Steps);
                return joined;
            }

            private static RdaPlan ChooseResilient(RdaState root, IList<RdaPlan> plans, RdaPlan best,
                CancellationToken token = default(CancellationToken))
            {
                if (root.Pending.Length != 0 || root.HasFlag(RdaState.FlagMaxxC) || root.HasFlag(RdaState.FlagFuwalos) || ResilienceWeight <= 0)
                    return best;
                var pool = new List<RdaPlan>();
                foreach (RdaPlan plan in plans)
                {
                    pool.Add(plan);
                    pool.AddRange(plan.Alternatives);
                }
                // Combo do jogador seguido até o fim (jogador, 2026-09-15: "quero manter o meu combo pois ele é efetivo", sobre o
                // Bone sozinho): a resistência compara linhas da mesma camada, mas não troca a rota por uma mesa de camada pior.
                int routeTier = pool.Where(p => p.RouteComplete && p.Tier <= 1 && !p.NovaUnprotected)
                    .Select(p => p.Tier).DefaultIfEmpty(int.MaxValue).Min();
                if (best.Tier > routeTier)
                    best = pool.Where(p => p.RouteComplete && p.Tier == routeTier && !p.NovaUnprotected).OrderByDescending(p => p.Score).First();
                pool.RemoveAll(p => p.Tier > routeTier);
                var contenders = new List<RdaPlan>();
                var starters = new HashSet<string>();
                // A melhor mesa entra primeiro; depois as outras aberturas, da melhor nota para a pior.
                // Nova sem proteção nunca disputa (teste: sorteio_05 trocou para um Hypernova sem proteção).
                foreach (RdaPlan plan in new[] { best }.Concat(pool.Where(p => !ReferenceEquals(p, best) && !p.NovaUnprotected).OrderByDescending(p => p.Score)))
                {
                    int index = plan.Steps.FindIndex(step => IsCriticalAction(step.Action));
                    string key = index < 0 ? "-" : plan.Steps[index].Action.Text;
                    if (!starters.Add(key))
                        continue;
                    contenders.Add(plan);
                    if (contenders.Count >= ResilienceContenders)
                        break;
                }
                if (contenders.Count < 2)
                    return best;

                // O oponente já deixou passar efeitos críticos: cada um corta a chance pela metade (50%, 25%, 12,5%...).
                double chance = StarterNegationChance * Math.Pow(0.5, Math.Max(0, root.UnansweredCriticals));
                var fallbackTier = new int[contenders.Count];
                var fallbackScore = new double[contenders.Count];
                Parallel.For(0, contenders.Count, i =>
                {
                    RdaPlan plan = contenders[i];
                    int index = plan.Steps.FindIndex(step => IsCriticalAction(step.Action));
                    if (index < 0)
                    {
                        fallbackTier[i] = plan.Tier;
                        fallbackScore[i] = plan.Score;
                        return;
                    }
                    RdaPlan fallback = Plan(NegateBait(plan.Steps[index].After, plan.Steps[index].Action), BaitFallbackSearch.WithToken(token), null);
                    fallbackTier[i] = fallback.Tier;
                    fallbackScore[i] = fallback.Score;
                });

                int chosen = 0;
                double bestValue = double.MinValue;
                for (int i = 0; i < contenders.Count; ++i)
                {
                    double value = (1 - chance) * contenders[i].Score + chance * fallbackScore[i];
                    if (value > bestValue)
                    {
                        bestValue = value;
                        chosen = i;
                    }
                }
                RdaPlan result = contenders[chosen];
                int starter = result.Steps.FindIndex(step => IsCriticalAction(step.Action));
                result.ResilienceNote = string.Format("starter \"{0}\": if negated, tier {1} score {2:0.#} ({3} routes compared{4})",
                    starter < 0 ? "-" : result.Steps[starter].Action.Text, fallbackTier[chosen], fallbackScore[chosen], contenders.Count,
                    ReferenceEquals(result, best) ? ", the best-scoring one" : string.Format(", replaced the one scoring {0:0.#}", best.Score));
                return result;
            }

            private sealed class BaitOption
            {
                public List<PlanStep> Steps;
                public RdaState Final;
                public RdaState Negated;
                public int FallbackTier = 4;
                public double FallbackScore = double.MinValue;
            }

            private static void BaitFirst(RdaPlan plan, CancellationToken token = default(CancellationToken))
            {
                if (plan == null || plan.Root == null || plan.Root.Pending.Length != 0 || plan.Steps.Count == 0
                    || plan.Root.HasFlag(RdaState.FlagBaitDone))
                    return;
                List<PlanAction> actions = plan.Steps.Select(step => step.Action).ToList();
                if (!actions.Any(IsCriticalAction))
                    return;

                var options = new List<BaitOption>();
                // 1) Iscas que o plano já usa: mover a escolhida para o primeiro passo (mesma mesa final).
                for (int i = 0; i < actions.Count; ++i)
                {
                    if (!IsBaitAction(actions[i]))
                        continue;
                    var order = new List<PlanAction> { actions[i] };
                    order.AddRange(actions.Where((action, index) => index != i));
                    List<PlanStep> simulated = SimulateActions(plan.Root, order);
                    if (simulated != null && simulated[simulated.Count - 1].After.Equals(plan.Final))
                        options.Add(new BaitOption { Steps = simulated, Final = simulated[simulated.Count - 1].After });
                }
                // 2) Plano sem isca: ativar uma da mão antes de tudo, sem mudar o campo final.
                if (options.Count == 0)
                {
                    foreach (RdaMove move in RdaRules.Successors(plan.Root))
                    {
                        if (!IsBaitAction(move.Action) || move.Action.From != CardLocation.Hand)
                            continue;
                        var order = new List<PlanAction> { move.Action };
                        order.AddRange(actions);
                        List<PlanStep> simulated = SimulateActions(plan.Root, order);
                        if (simulated != null && RdaArray.Same(simulated[simulated.Count - 1].After.Field, plan.Final.Field))
                            options.Add(new BaitOption { Steps = simulated, Final = simulated[simulated.Count - 1].After });
                    }
                }
                if (options.Count == 0)
                    return;

                // Qual isca dói menos se for negada: replanejamento rápido, em paralelo, a partir da negação.
                foreach (BaitOption option in options)
                    option.Negated = NegateBait(option.Steps[0].After, option.Steps[0].Action);
                Parallel.For(0, options.Count, i =>
                {
                    RdaPlan fallback = Plan(options[i].Negated, BaitFallbackSearch.WithToken(token), null);
                    options[i].FallbackTier = fallback.Tier;
                    options[i].FallbackScore = fallback.Score;
                });
                BaitOption best = options.OrderBy(option => option.FallbackTier).ThenByDescending(option => option.FallbackScore).First();

                if (ReferenceEquals(best.Steps[0].Action, plan.Steps[0].Action) || best.Steps[0].Action.Text == plan.Steps[0].Action.Text)
                {
                    plan.BaitNote = best.Steps[0].Action.Text + " (it was already the first step)";
                    return;
                }
                plan.Steps.Clear();
                plan.Steps.AddRange(best.Steps);
                plan.Final = best.Final;
                plan.BaitNote = string.Format("{0} (if negated: tier {1}, score {2:0.#}; {3} options)", best.Steps[0].Action.Text,
                    best.FallbackTier, best.FallbackScore, options.Count);
            }

            // ------------------------------------------------------------------ reparo do plano (recálculo rápido)
            // Quando o duelo sai do plano (efeito negado, alvo diferente), refaz os passos que faltavam a partir do estado real,
            // pulando os que deixaram de ser possíveis. Aceita só se a mesa continuar na mesma camada, sem nova desprotegida e
            // com nota perto da prevista; senão a busca normal roda. Poupa os 4-10 s de busca na maioria dos desvios pequenos.
            private const double RepairMargin = 40;

            public static RdaPlan RepairPlan(RdaState root, RdaPlan previous, int fromStep, ICollection<string> blocked)
            {
                if (previous == null || previous.Root == null || root.Pending.Length != 0 || fromStep >= previous.Steps.Count
                    || root.Flags != previous.Root.Flags)
                    return null;
                var watch = Stopwatch.StartNew();
                var steps = new List<PlanStep>();
                RdaState state = root;
                bool novaUnprotected = false;
                int index = Math.Max(0, fromStep);
                int guard = 0;
                while (index < previous.Steps.Count && guard++ < 200)
                {
                    PlanAction wanted = previous.Steps[index].Action;
                    RdaMove match = default(RdaMove);
                    RdaMove decline = default(RdaMove);
                    bool found = false, canDecline = false;
                    foreach (RdaMove move in RdaRules.Successors(state))
                    {
                        if (move.Action.Kind == PlanKind.TriggerDecline && !canDecline)
                        {
                            decline = move;
                            canDecline = true;
                        }
                        if (move.Action.Kind != wanted.Kind || move.Action.Text != wanted.Text)
                            continue;
                        if (steps.Count == 0 && blocked != null
                            && (blocked.Contains(move.Action.Text) || blocked.Contains(EffectBlockKey(move.Action))))
                            continue;
                        match = move;
                        found = true;
                        break;
                    }
                    if (!found)
                    {
                        // Gatilho pendente que o plano antigo não previa: recusa e tenta o mesmo passo de novo.
                        if (state.Pending.Length > 0 && canDecline)
                        {
                            steps.Add(new PlanStep { Action = decline.Action, After = decline.State });
                            state = decline.State;
                            continue;
                        }
                        index++;   // passo que deixou de ser possível
                        continue;
                    }
                    if (match.Action.Kind == PlanKind.Synchro && (match.Action.CardId == RdaCards.Hypernova || match.Action.CardId == RdaCards.Supernova)
                        && !HasProtection(state))
                        novaUnprotected = true;
                    steps.Add(new PlanStep { Action = match.Action, After = match.State });
                    state = match.State;
                    index++;
                }
                if (steps.Count == 0 || state.Pending.Length > 0 || novaUnprotected)
                    return null;
                int tier = RdaEvaluator.Tier(state);
                double score = RdaEvaluator.SelectionScore(state, root.Hand);
                if (tier > previous.Tier || score < previous.Score - RepairMargin)
                    return null;
                var plan = new RdaPlan { Score = score, Tier = tier, Search = "plan repair", Root = root, Final = state, Depth = steps.Count };
                plan.Steps.AddRange(steps);
                plan.ElapsedMs = watch.ElapsedMilliseconds;
                return plan;
            }

            // =================================================================================================
            // Invocar só quando for usar (jogador, 2026-09-15): "é pra ele jogar no campo os monstros somente quando for usar".
            // Depois de escolher o plano, cada bloco que coloca monstro no campo (a jogada e os gatilhos logo depois dela) é adiado
            // para logo antes do primeiro passo que usa aquele monstro. Cada troca é refeita pelas regras e só vale se todos os
            // passos continuam possíveis e a mesa final é idêntica. O começo até o starter (isca e 1º efeito crítico) não muda.
            // Menos monstros parados no campo = menos exposição a negação, Nibiru e remoção no meio do combo.
            // =================================================================================================
            private const int DelaySummonsMaxReplays = 80;

            private static List<List<PlanStep>> SplitBlocks(List<PlanStep> steps)
            {
                var blocks = new List<List<PlanStep>>();
                foreach (PlanStep step in steps)
                {
                    bool trigger = step.Action.Kind == PlanKind.TriggerAccept || step.Action.Kind == PlanKind.TriggerDecline;
                    if (!trigger || blocks.Count == 0)
                        blocks.Add(new List<PlanStep>());
                    blocks[blocks.Count - 1].Add(step);
                }
                return blocks;
            }

            private static void DelaySummons(RdaPlan plan)
            {
                if (plan == null || plan.Root == null || plan.Steps.Count < 3)
                    return;
                int replays = 0;
                bool moved = true;
                while (moved && replays < DelaySummonsMaxReplays)
                {
                    moved = false;
                    List<List<PlanStep>> blocks = SplitBlocks(plan.Steps);
                    int fixedBlocks = blocks.FindIndex(block => block.Any(step => IsCriticalAction(step.Action))) + 1;
                    RdaState before = plan.Root;
                    for (int b = 0; b < blocks.Count && !moved && replays < DelaySummonsMaxReplays; ++b)
                    {
                        RdaState after = blocks[b][blocks[b].Count - 1].After;
                        List<PlanStep> block = blocks[b];
                        RdaState blockBefore = before;
                        before = after;
                        if (b < fixedBlocks || block[0].Action.Kind == PlanKind.Synchro)
                            continue;
                        // Monstros que este bloco coloca no campo.
                        List<string> summoned = after.Field.Select(RdaField.Id).Except(blockBefore.Field.Select(RdaField.Id))
                            .Select(RdaCards.Name).ToList();
                        if (summoned.Count == 0)
                            continue;
                        // Primeiro bloco depois dele que usa algum desses monstros (material, custo, alvo).
                        int consumer = -1;
                        for (int c = b + 1; c < blocks.Count && consumer < 0; ++c)
                        {
                            if (blocks[c].Any(step => step.Action.Text != null && summoned.Any(name => step.Action.Text.Contains(name))))
                                consumer = c;
                        }
                        if (consumer < 0)
                            consumer = blocks.Count; // fica no campo até o fim: vai para o final
                        // Tenta o mais tarde possível: logo antes do consumidor, depois uma posição antes, até b + 2.
                        for (int target = consumer; target >= b + 2 && !moved && replays < DelaySummonsMaxReplays; --target)
                        {
                            var order = new List<List<PlanStep>>(blocks);
                            order.RemoveAt(b);
                            order.Insert(target - 1, block);
                            List<PlanAction> actions = order.SelectMany(item => item.Select(step => step.Action)).ToList();
                            replays++;
                            List<PlanStep> simulated = SimulateActions(plan.Root, actions);
                            if (simulated == null || !simulated[simulated.Count - 1].After.Equals(plan.Final))
                                continue;
                            plan.Steps.Clear();
                            plan.Steps.AddRange(simulated);
                            plan.Final = simulated[simulated.Count - 1].After;
                            moved = true;
                        }
                    }
                }
                double exposure = 0;
                foreach (PlanStep step in plan.Steps)
                    exposure += StepExposure(step.After);
                plan.Exposure = exposure;
            }

            // Refaz uma sequência de ações pelas regras (casando pelo texto). null se algum passo não for possível.
            private static List<PlanStep> SimulateActions(RdaState root, List<PlanAction> actions)
            {
                var result = new List<PlanStep>();
                RdaState state = root;
                foreach (PlanAction action in actions)
                {
                    RdaMove match = default(RdaMove);
                    bool found = false;
                    foreach (RdaMove move in RdaRules.Successors(state))
                    {
                        if (move.Action.Kind == action.Kind && move.Action.Text == action.Text)
                        {
                            match = move;
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                        return null;
                    result.Add(new PlanStep { Action = match.Action, After = match.State });
                    state = match.State;
                }
                return result;
            }

            // Custo usado só para escolher entre caminhos que chegam ao mesmo estado. Com o risco de Nibiru cobrado a
            // cada invocação, ele muda muito a ordem da busca; NibiruInDedupe = false deixa o Nibiru só na nota final.
            // 25 mãos: com o Nibiru na escolha de caminho 14/6/2/3 (Soul caiu para camada 1); só na nota final 13/7/3/2
            // (Soul e PV na camada 0, menos mesas na camada 3). Diferença pequena, fica fora da escolha de caminho.
            private static bool NibiruInDedupe = false; // static para teste offline por reflexão

            private static double DedupeCost(Node node)
            {
                return PathCost(node) - (NibiruInDedupe ? 0 : node.NibiruPenalty);
            }

            // Nibiru: perder camada por linha exposta foi testado e DESLIGADO por decisão do jogador (2026-09-14). As linhas
            // protegidas perdem o Hypernova (Soul 396 -> 303, PV 468 -> 253); o certo é buscar a mesa ideal, porque a proteção
            // chega poucos passos depois do 5º monstro e a mesa pode ser remontada (RDA + Burning Soul, Synkron do GY, token
            // do Nibiru como material da Quetzacoatl). O risco fica só como desempate na nota (NibiruPenaltyWeight).
            private static bool NibiruTierBump = false; // static para teste offline por reflexão

            // Mesmo estado por dois caminhos: fica o que protegeu a nova, depois o que não se expôs ao Nibiru, depois o mais barato.
            private static bool KeepExisting(Node existing, Node child)
            {
                if (!ExposureDedupe)
                    return true;
                if (existing.NovaUnprotected != child.NovaUnprotected)
                    return !existing.NovaUnprotected;
                if (NibiruTierBump && existing.NibiruExposed != child.NibiruExposed)
                    return !existing.NibiruExposed;
                return DedupeCost(existing) <= DedupeCost(child);
            }

            // Exposição da linha: monstros que ficam parados no campo esperando uso. Uma interrupção nesse momento
            // deixa a mesa cheia de peças que não chegam a lugar nenhum, e ocupar as zonas trava as próximas invocações.
            // Por passo: monstros não-Synchro além de 2 (um par Tuner + não-Tuner é o normal) + monstros além de 5.
            // Entra só na escolha da mesa final e para decidir, entre caminhos que chegam ao mesmo estado, qual ordem usar.
            // static (não const) para os testes offline poderem trocar os valores por reflexão.
            private static double ExposureWeight = 1.0;
            private static bool ExposureDedupe = true;

            private static double StepExposure(RdaState state)
            {
                int nonSynchro = 0;
                foreach (long monster in state.Field)
                {
                    if (!RdaCards.Get(RdaField.Id(monster)).Synchro)
                        ++nonSynchro;
                }
                return Math.Max(0, nonSynchro - 2) + Math.Max(0, state.Field.Length - 5);
            }

            // Hypernova/Supernova sem proteção prévia na mesa: uma interrupção pega o combo sem resposta.
            // Entra só na escolha da mesa final (não guia a busca).
            private const double UnprotectedNovaPenalty = 60;

            // Categorias das cartas do Extra Deck (jogador, 2026-09-16). A lista antiga misturava as três e deixava
            // o Quetzacoatl de fora; separá-las torna explícito o que cada mesa oferece.
            //   negação: interrompe o efeito do oponente enquanto está na mesa.
            //   proteção própria: não nega nada, mas impede que nossos monstros sejam destruídos.
            //   proteção indireta: o King não nega; se virar alvo, ele se troca por um RDA que já está protegido.
            internal static bool IsNegate(int id)
            {
                return id == RdaCards.Zalen || id == RdaCards.Abyss || id == RdaCards.DisPater || id == RdaCards.Quetzacoatl;
            }

            private static bool IsSelfProtection(int id)
            {
                // 99585850: segunda impressão da Supernova, que aparece em decks importados.
                return id == RdaCards.Hypernova || id == RdaCards.Supernova || id == 99585850
                    || id == RdaCards.BurningSoul || id == RdaCards.StormBane;
            }

            private static bool IsIndirectProtection(int id)
            {
                return id == RdaCards.King;
            }

            /// <summary>Conta, na mesa, quantas negações, proteções próprias e proteções indiretas estão ativas.</summary>
            private static void CountInterruptions(RdaState state, out int negates, out int selfProtection, out int indirectProtection)
            {
                negates = 0;
                selfProtection = 0;
                indirectProtection = 0;
                foreach (long monster in state.Field)
                {
                    if (RdaField.Negated(monster))
                        continue;
                    int id = RdaField.Id(monster);
                    if (IsNegate(id))
                        ++negates;
                    if (IsSelfProtection(id))
                        ++selfProtection;
                    if (IsIndirectProtection(id))
                        ++indirectProtection;
                }
            }

            // Uma Nova está "coberta" se a mesa já responde a uma interrupção: negação, ou o King, que se troca pelo RDA.
            // A proteção própria não entra aqui: ela impede destruição, mas não impede que o efeito do combo seja negado.
            private static bool HasProtection(RdaState state)
            {
                int negates, selfProtection, indirectProtection;
                CountInterruptions(state, out negates, out selfProtection, out indirectProtection);
                return negates > 0 || indirectProtection > 0;
            }

            // Assinatura da rota para a diversidade: quais cartas ainda estão no Extra Deck.
            private static int ExtraSignature(RdaState state)
            {
                unchecked
                {
                    int hash = 17;
                    foreach (int id in state.Extra)
                        hash = hash * 31 + id;
                    return hash;
                }
            }

            /// <summary>Carteira: roda as buscas em paralelo e fica com a melhor mesa (camada primeiro, nota de seleção depois).</summary>
            public static RdaPlan PlanPortfolio(RdaState root, IList<SearchOptions> searches, ICollection<string> blockedRootActions)
            {
                // Token de cancelamento desta execução da carteira: a busca de fundo do turno passa as opções já clonadas
                // com o token (ver StartBackgroundPlan); o replanejamento síncrono usa as opções estáticas sem token (None).
                CancellationToken token = searches.Count > 0 ? searches[0].Token : default(CancellationToken);
                var watch = Stopwatch.StartNew();
                // Rotas de referência (combos do jogador) só no primeiro plano do turno: encaixadas na mão pelas regras e
                // completadas por uma busca curta. Se alguma chega à mesa ideal com o nova protegido, a carteira pesada é trocada
                // por uma busca de verificação (economiza tempo); senão a carteira roda normalmente. Tudo disputa junto.
                bool firstPlan = searches.Count >= 3;
                bool maxx = root.HasFlag(RdaState.FlagMaxxC) || root.HasFlag(RdaState.FlagFuwalos);
                // No primeiro plano a rota é encaixada desde o começo e completada por uma busca curta. Nos replanejamentos (depois
                // de uma negação) ela é encaixada a partir de qualquer passo e só entra se for até o fim, sem busca extra: o custo
                // é só o encaixe (milissegundos), então o tempo de espera não muda.
                // Com Maxx "C"/Fuwalos ativos entram só as rotas escritas para essa situação (jogador, 2026-09-15); fora delas, só as
                // rotas normais. Antes as rotas eram simplesmente desligadas sob Maxx "C".
                var phase = Stopwatch.StartNew();
                List<RdaPlan> guided = (blockedRootActions == null || blockedRootActions.Count == 0) && root.Pending.Length == 0
                    ? GuidedPlans(root, firstPlan, maxx, token) : new List<RdaPlan>();
                long msGuided = phase.ElapsedMilliseconds;
                long msSearch = 0, msForced = 0, msResilient = 0, msShortcut = 0, msSeeds = 0;
                RdaPlan bestGuided = guided.OrderBy(item => item.Tier).ThenByDescending(item => item.Score).FirstOrDefault();
                // Atalho só com início seguro (jogador, 2026-09-15): "antes uma mesa mais fraca com menos risco de ser parada do que
                // uma mesa muito forte com início muito frágil". Se negar o starter da rota trava o combo (camada 3), a carteira
                // completa roda para achar rotas mais seguras (ex.: Gaia busca Power Vice com Darkness na mão, em vez do Soul).
                // Qualquer rota que chega à mesa ideal com o nova protegido serve, desde que o início aguente: com o starter negado
                // ainda sobra camada 2 ou nota >= RouteSafeFallbackScore (o Soul negado sobrava ~0; o Power Vice negado, 267).
                bool shortcut = false;
                // Camada 1 também conta quando a rota foi seguida inteira: é a mesa de Supernova que o jogador montou (Bone, Foolish).
                phase.Restart();
                List<RdaPlan> idealRoutes = guided.Where(plan => (plan.Tier == 0 || (plan.Tier == 1 && plan.RouteComplete)) && !plan.NovaUnprotected).ToList();
                if (firstPlan && idealRoutes.Count > 0)
                {
                    var safe = new bool[idealRoutes.Count];
                    Parallel.For(0, idealRoutes.Count, i =>
                    {
                        RdaPlan starterNegated = StarterFallback(idealRoutes[i], token);
                        safe[i] = starterNegated == null || starterNegated.Tier <= 2 || starterNegated.Score >= RouteSafeFallbackScore;
                    });
                    shortcut = safe.Any(value => value);
                    // Mãos jogáveis (jogador, 2026-09-15): se todo iniciante da mão abre alguma rota que encaixou, a carteira
                    // completa não tem outra abertura para achar (bateria: Soul sozinho, Power Vice sozinho, sorteio_04/06 ficavam
                    // 12-17 s para confirmar a mesma rota). A rota frágil é aceita; a resistência ainda compara as rotas encaixadas
                    // com as buscas de verificação e a abertura alternativa.
                    if (!shortcut && StartersCovered(root, guided))
                        shortcut = true;
                }
                else if (firstPlan)
                {
                    // Nenhuma rota ideal encaixou: a mão não chega à mesa ideal, e medir mostrou que a carteira pesada não
                    // paga o que cobra nesse caso. Fora do turno 1 isso é a regra, não a exceção: em 12 raízes reais de turno
                    // 2+ colhidas dos logs, 7 caíam aqui (no turno 1 o atalho já dispara em 21 de 23 mãos).
                    // Medição de 2026-09-16 (handbook\tests\teste_atalho_sempre.ps1), comparando a carteira pesada com
                    // verificação + rotas + resistência nas mesmas raízes: 3 mesas melhores, 9 iguais, nenhuma pior, e
                    // 88.527 ms caindo para 34.854 ms nas 7 raízes sem atalho. Os dois piores casos vistos em jogo (26,6 s e
                    // 27,0 s) foram para 9,3 s e 12,3 s, e um deles ainda subiu de camada 3 nota 41 para camada 2 nota 259:
                    // a busca larga se perde no espaço grande e as linhas boas saem do feixe cedo, enquanto as rotas
                    // encaixadas já entregam a mesa melhor.
                    shortcut = true;
                }
                // Com rota segura: verificação normal e protegida em paralelo (mesmo limite de tempo, a espera não aumenta) para
                // ainda comparar com linhas fora das rotas.
                msShortcut = phase.ElapsedMilliseconds;
                IList<SearchOptions> effective = shortcut ? (IList<SearchOptions>)new[] { RouteVerifySearch, RouteVerifySearchProtected } : searches;
                var plans = new RdaPlan[effective.Count];
                phase.Restart();
                Parallel.For(0, effective.Count, i => plans[i] = Plan(root, effective[i].WithToken(token), blockedRootActions));
                msSearch = phase.ElapsedMilliseconds;
                var pool = new List<RdaPlan>(plans);
                pool.AddRange(guided);
                RdaPlan best = pool.OrderBy(item => item.Tier).ThenByDescending(item => item.Score).First();
                // Sementes por abertura (só no primeiro plano do turno): linhas longas, como "Gaia busca Soul" até o Hypernova,
                // passam por uma fase de mesa fraca e saem do feixe (teste contra o Albaz, 23:19: caiu na profundidade 13; buscando
                // a partir do passo 11 a mesma busca achava a camada 0 em 6 s). A busca continua a partir delas e o resultado
                // entra na disputa junto com a carteira.
                if (firstPlan && plans.Any(plan => plan.Seeds.Count > 0))
                {
                    phase.Restart();
                    List<RdaPlan> seeded = SeededPlans(root, plans, best, token);
                    msSeeds = phase.ElapsedMilliseconds;
                    pool.AddRange(seeded);
                    best = pool.OrderBy(item => item.Tier).ThenByDescending(item => item.Score).First();
                }
                // Só no primeiro plano do turno (carteira de 3+ buscas): também busca uma rota com outra abertura, para a
                // escolha por resistência ter com o que comparar (a abertura alternativa costuma sair do feixe cedo).
                // Com o atalho ligado ela não entra: o pool já vem das rotas encaixadas, que por construção cobrem os
                // iniciantes da mão (StartersCovered), então a abertura forçada é redundante. Medido em 2026-09-16 com
                // handbook\tests\teste_forced_redundante.ps1: nas 21 mãos com atalho ela nunca mudou a escolha do
                // ChooseResilient, e custava de 0,8 a 2,7 s no caminho crítico.
                if (searches.Count >= 3 && !shortcut)
                {
                    phase.Restart();
                    RdaPlan forced = ForcedAlternative(root, best, blockedRootActions);
                    msForced = phase.ElapsedMilliseconds;
                    if (forced != null)
                        pool.Add(forced);
                }
                phase.Restart();
                best = ChooseResilient(root, pool, best, token);
                msResilient = phase.ElapsedMilliseconds;
                // Droll & Lock Bird resolvido (jogador, 2026-09-16): a rota de referência tem preferência. Sob Droll sobram poucas
                // cartas jogáveis, as mesas empatam em negações e a alternativa da busca só aguentava melhor a negação porque
                // gastava a Ash Blossom como material, o que troca uma interrupção por quase nada. A rota também deixa o RDA com o
                // Soul no GY, o que abre a linha da Crimson Gaia.
                if (root.HasFlag(RdaState.FlagNoDeckAdd))
                {
                    RdaPlan drollRoute = guided.Where(plan => plan.Tier <= best.Tier)
                        .OrderBy(plan => plan.Tier).ThenByDescending(plan => plan.Score).FirstOrDefault();
                    if (drollRoute != null && !ReferenceEquals(drollRoute, best))
                    {
                        drollRoute.ResilienceNote = string.Format("Droll: the player route wins (route score {0:0.#}, search line {1:0.#})",
                            drollRoute.Score, best.Score);
                        best = drollRoute;
                    }
                }
                // Isca só no primeiro plano do turno (jogador): é para puxar a negação enquanto a mesa está sendo montada.
                // Nos recálculos do meio do combo a isca é dispensada; no turno seguinte o primeiro plano volta a considerar.
                if (searches.Count >= 3)
                    BaitFirst(best, token);
                DelaySummons(best);
                best.ElapsedMs = watch.ElapsedMilliseconds;
                // Perfil somado de todas as buscas que rodaram nesta carteira (inclui as rotas encaixadas). Serve para
                // saber, numa partida de verdade, em que etapa o tempo foi gasto — e não só quanto tempo foi.
                foreach (RdaPlan candidate in pool)
                {
                    best.PoolSuccessorsMs += candidate.SuccessorsMs;
                    best.PoolNodesMs += candidate.NodesMs;
                    best.PoolEvaluateMs += candidate.EvaluateMs;
                    best.PoolSortMs += candidate.SortMs;
                    best.PoolExpanded += candidate.Expanded;
                    best.PoolGenerated += candidate.Generated;
                    best.PoolCount++;
                    long searchCpuMs = candidate.SuccessorsMs + candidate.NodesMs + candidate.EvaluateMs + candidate.SortMs;
                    if (searchCpuMs > best.PoolMaxSearchMs) best.PoolMaxSearchMs = searchCpuMs;
                }
                best.PhaseGuidedMs = msGuided;
                best.PhaseSearchMs = msSearch;
                best.PhaseForcedMs = msForced;
                best.PhaseResilientMs = msResilient;
                best.PhaseShortcutMs = msShortcut;
                best.PhaseSeedsMs = msSeeds;
                return best;
            }

            // =================================================================================================
            // Rotas de referência (jogador, 2026-09-15): os 3 combos dos replays (Soul, Bone Archfiend, Power Vice) como guias.
            // Cada passo é um trecho do texto da ação do planejador: alternativas separadas por " | ", trechos obrigatórios
            // por " & ", "?" no começo = passo opcional (a mão pode já ter a carta ou a rota pode seguir sem ele).
            // O encaixe usa as regras do planejador (só jogadas legais), aceita até RouteMaxSkips passos que a mão não permite e
            // recusa gatilhos fora da rota. Não são seguidas 1:1: o começo encaixado é completado por uma busca curta e disputa
            // com as buscas normais pela mesma nota.
            // =================================================================================================
            private sealed class RouteStep
            {
                public readonly string[][] Alternatives;
                public readonly bool Optional;

                public RouteStep(string text)
                {
                    Optional = text.StartsWith("?");
                    if (Optional)
                        text = text.Substring(1);
                    Alternatives = text.Split(new[] { " | " }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(alternative => alternative.Split(new[] { " & " }, StringSplitOptions.RemoveEmptyEntries).Select(token => token.Trim()).ToArray())
                        .ToArray();
                }

                public bool Matches(string actionText)
                {
                    if (actionText == null)
                        return false;
                    foreach (string[] tokens in Alternatives)
                    {
                        bool all = true;
                        foreach (string token in tokens)
                        {
                            if (!actionText.Contains(token)) { all = false; break; }
                        }
                        if (all)
                            return true;
                    }
                    return false;
                }
            }

            private sealed class RouteGuide
            {
                public readonly string Name;
                public readonly RouteStep[] Steps;
                public readonly bool ForMaxx;   // rota escrita para Maxx "C"/Fuwalos ativos (dá poucas cartas ao oponente)

                public RouteGuide(string name, params string[] steps)
                    : this(false, name, steps)
                {
                }

                public RouteGuide(bool forMaxx, string name, params string[] steps)
                {
                    Name = name;
                    ForMaxx = forMaxx;
                    Steps = steps.Select(step => new RouteStep(step)).ToArray();
                }
            }

            private static readonly RouteGuide[] RouteGuides =
            {
                // Replay "RDA Soul Combo": Hypernova (Vision, Synkron, Zalen e Darkness no nível 1 + King) e Quetzacoatl.
                new RouteGuide("Soul",
                    // Foolish Burial manda o Vision para buscar a Crimson Gaia, que busca o Soul (jogador, 2026-09-15).
                    "?Foolish Burial sends Vision Resonator to the GY",
                    "?Vision (GY) searches Crimson Gaia",
                    "?Activate Crimson Gaia",
                    "?Stone Sweeper discards and searches Soul Resonator | Crimson Gaia searches Soul Resonator | Resonator Call searches Soul Resonator",
                    "Normal Summon Soul Resonator",
                    "Soul searches Bone Archfiend",
                    "Bone (hand) sends",
                    "Bone sends Crimson Resonator & lowers the Level of Bone Archfiend",
                    "Synchro Summon Red Rising Dragon & Bone Archfiend & Soul Resonator",
                    "Red Rising revives Crimson Resonator",
                    "Crimson Resonator summons & Synkron Resonator & Chain Resonator",
                    "Synchro Summon Crimson Blade Dragon & Chain Resonator & Red Rising Dragon",
                    "?Crimson Blade searches The Bystial Lubellion",
                    "?Lubellion discards and searches Magnamhut",
                    "Synchro Summon The Crimson King & Synkron Resonator & Crimson Blade Dragon",
                    "?King searches Power Vice Dragon | Synkron (GY) returns Chain Resonator",
                    "?King searches Power Vice Dragon | Synkron (GY) returns Chain Resonator",
                    "Magnamhut banishes Synkron Resonator",
                    "Magnamhut schedules",
                    "Lubellion ( & releases Bystial Magnamhut",
                    "Lubellion places Etude",
                    "Synchro Summon Bystial Dis Pater & The Bystial Lubellion & Crimson Resonator",
                    "Power Vice summons itself from the hand",
                    "Power Vice searches Darkness Resonator | Power Vice searches Vision Resonator",
                    "Darkness reveals RDA",
                    "Darkness grants",
                    "extra Normal Summon (Darkness) of Chain Resonator",
                    "Chain summons Vision Resonator from the Deck",
                    "Synchro Summon Red Rising Dragon & Chain Resonator & Power Vice Dragon",
                    "Red Rising revives Chain Resonator",
                    "Synchro Summon Zalen the Shackled Dragon & Chain Resonator & Red Rising Dragon",
                    "Dis Pater summons Synkron Resonator from the banished zone",
                    "Darkness changes to Level 1 & Zalen the Shackled Dragon & Darkness Resonator & Vision Resonator",
                    "Synchro Summon Red Hypernova Dragon",
                    "?Synkron (GY) returns Vision Resonator | Vision (GY) searches",
                    "?Synkron (GY) returns Vision Resonator | Vision (GY) searches",
                    "Vision summons itself from the hand",
                    "Synchro Summon Crimson Dragon Quetzacoatl & Bystial Dis Pater & Vision Resonator",
                    "Quetzacoatl revives"),
                // Replay "RDA Bone Archfiend Combo": Supernova (só 3 Tuners) e Quetzacoatl revivendo Dis Pater, Blade, Abyss e King.
                new RouteGuide("Bone",
                    "Normal Summon Bone Archfiend",
                    "Bone sends Vision Resonator & raises the Level of Bone Archfiend",
                    "Vision (GY) searches Crimson Gaia",
                    "Activate Crimson Gaia",
                    "Crimson Gaia searches Darkness Resonator",
                    "Darkness reveals RDA",
                    "Darkness grants",
                    "Synchro Summon The Crimson King & Bone Archfiend & Darkness Resonator",
                    "King searches Crimson Call",
                    "Crimson Call searches Chain Resonator",
                    "extra Normal Summon (Darkness) of Chain Resonator | Normal Summon Chain Resonator",
                    "Chain summons Crimson Resonator from the Deck",
                    "Synchro Summon Hot Red Dragon Archfiend Abyss & Chain Resonator & The Crimson King",
                    "Crimson Resonator summons & Vision Resonator & Soul Resonator",
                    "Soul searches Synkron Resonator",
                    "Bone (GY) sends Hot Red Dragon Archfiend Abyss from the field",
                    "Synchro Summon Red Rising Dragon & Vision Resonator & Bone Archfiend",
                    "Red Rising revives Darkness Resonator",
                    "Synkron summons itself from the hand",
                    "Synchro Summon Crimson Blade Dragon & Synkron Resonator & Red Rising Dragon",
                    "?Synkron (GY) returns Vision Resonator | Crimson Blade searches The Bystial Lubellion",
                    "?Synkron (GY) returns Vision Resonator | Crimson Blade searches The Bystial Lubellion",
                    "Lubellion discards and searches Magnamhut",
                    "Magnamhut banishes Synkron Resonator",
                    "Magnamhut schedules",
                    "Lubellion ( & releases Bystial Magnamhut",
                    "Lubellion places Etude",
                    "Synchro Summon Bystial Dis Pater & The Bystial Lubellion & Crimson Resonator",
                    "Dis Pater summons Synkron Resonator from the banished zone",
                    "?Darkness changes to Level 1 & Darkness Resonator",
                    "Synchro Summon Red Supernova Dragon",
                    "?Synkron (GY) returns Vision Resonator",
                    "Vision summons itself from the hand",
                    "Synchro Summon Crimson Dragon Quetzacoatl & Bystial Dis Pater & Vision Resonator",
                    "Quetzacoatl revives"),
                // Replay "RDA Power Vice Dragon Combo": Hypernova (Darkness, Soul, Chain e Vision + Blade) e Quetzacoatl.
                new RouteGuide("Power Vice",
                    "?Foolish Burial sends Vision Resonator to the GY",
                    "?Vision (GY) searches Crimson Gaia",
                    "?Activate Crimson Gaia",
                    "?Crimson Gaia searches Power Vice Dragon",
                    "Power Vice summons itself from the hand",
                    // Darkness já na mão (parceiro do Power Vice): a busca vai para outro Resonator da rota.
                    "Power Vice searches Darkness Resonator | Power Vice searches Chain Resonator | Power Vice searches Crimson Resonator",
                    "Darkness reveals RDA",
                    "Darkness grants",
                    "Synchro Summon The Crimson King & Power Vice Dragon & Darkness Resonator",
                    "King searches Crimson Gaia | King searches Crimson Call",
                    "Crimson Call searches Chain Resonator",
                    "extra Normal Summon (Darkness) of Chain Resonator | Normal Summon Chain Resonator",
                    "Chain summons Crimson Resonator from the Deck",
                    "Synchro Summon Hot Red Dragon Archfiend Abyss & Chain Resonator & The Crimson King",
                    "Crimson Resonator summons & Synkron Resonator & Soul Resonator",
                    "Soul searches Bone Archfiend",
                    "Bone (hand) sends Hot Red Dragon Archfiend Abyss from the field",
                    "Bone sends Vision Resonator & raises the Level of Bone Archfiend",
                    "?Vision (GY) searches",
                    "Synchro Summon Red Rising Dragon & Synkron Resonator & Bone Archfiend",
                    "Red Rising revives Synkron Resonator",
                    "?Synkron (GY) returns Chain Resonator",
                    "Synchro Summon Crimson Blade Dragon & Synkron Resonator & Red Rising Dragon",
                    "?Synkron (GY) returns Vision Resonator | Crimson Blade searches The Bystial Lubellion",
                    "?Synkron (GY) returns Vision Resonator | Crimson Blade searches The Bystial Lubellion",
                    "Lubellion discards and searches Magnamhut",
                    "Magnamhut banishes Darkness Resonator",
                    "Magnamhut schedules",
                    "Lubellion ( & releases Bystial Magnamhut",
                    "Lubellion places Etude",
                    "Synchro Summon Bystial Dis Pater & The Bystial Lubellion & Crimson Resonator",
                    "Normal Summon Chain Resonator | extra Normal Summon (Darkness) of Chain Resonator",
                    "Chain summons Vision Resonator from the Deck",
                    "Dis Pater summons Darkness Resonator from the banished zone",
                    "Darkness changes to Level 1 & Darkness Resonator & Soul Resonator",
                    "Synchro Summon Red Hypernova Dragon",
                    "?Synkron (GY) returns Vision Resonator | Vision (GY) searches",
                    "Vision summons itself from the hand",
                    "Synchro Summon Crimson Dragon Quetzacoatl & Bystial Dis Pater & Vision Resonator",
                    "Quetzacoatl revives"),
                // Combo descrito pelo jogador (2026-09-15) para a mão que só tem Foolish Burial como starter:
                // Supernova, Quetzacoatl, Abyss, Dis Pater, Crimson King e Crimson Blade.
                new RouteGuide("Foolish Burial",
                    "Foolish Burial sends Bone Archfiend to the GY",
                    "Bone (GY) sends & from the hand and summons itself",
                    "Bone sends Vision Resonator & raises the Level of Bone Archfiend",
                    "Vision (GY) searches Crimson Gaia",
                    "Activate Crimson Gaia",
                    "Crimson Gaia searches Darkness Resonator",
                    "Darkness reveals RDA",
                    "Darkness grants",
                    "Synchro Summon The Crimson King & Bone Archfiend & Darkness Resonator",
                    "King searches Power Vice Dragon",
                    "Power Vice summons itself from the hand",
                    "Power Vice searches Chain Resonator",
                    // O Chain entra primeiro pela Invocação-Normal extra do Darkness; a normal fica para o fim do combo.
                    "extra Normal Summon (Darkness) of Chain Resonator",
                    "Chain summons Soul Resonator from the Deck",
                    "Soul searches Synkron Resonator",
                    "Synkron summons itself from the hand",
                    "Synchro Summon Red Rising Dragon & Power Vice Dragon & Synkron Resonator",
                    "Red Rising revives Synkron Resonator | Synkron (GY) returns Vision Resonator",
                    "Red Rising revives Synkron Resonator | Synkron (GY) returns Vision Resonator",
                    "Synchro Summon Crimson Blade Dragon & Red Rising Dragon & Synkron Resonator",
                    "Synkron (GY) returns Darkness Resonator | Crimson Blade searches The Bystial Lubellion",
                    "Synkron (GY) returns Darkness Resonator | Crimson Blade searches The Bystial Lubellion",
                    "Lubellion discards and searches Magnamhut",
                    "Magnamhut banishes Synkron Resonator",
                    "Magnamhut schedules",
                    "Lubellion ( & releases Bystial Magnamhut",
                    "?Lubellion places Etude",
                    "Vision summons itself from the hand",
                    "Synchro Summon Bystial Dis Pater & The Bystial Lubellion & Vision Resonator",
                    "Dis Pater summons Synkron Resonator from the banished zone",
                    "Synchro Summon Red Supernova Dragon & Crimson Blade Dragon",
                    "Synkron (GY) returns Chain Resonator",
                    "Normal Summon Chain Resonator | extra Normal Summon (Darkness) of Chain Resonator",
                    "Chain summons Crimson Resonator from the Deck",
                    "Synchro Summon Hot Red Dragon Archfiend Abyss & The Crimson King & Chain Resonator",
                    "Synchro Summon Crimson Dragon Quetzacoatl & Bystial Dis Pater & Crimson Resonator",
                    "Quetzacoatl revives"),
                // Rotas tiradas dos planos das partidas (logs de 2026-09-15), escritas por papéis. Terminam no Hypernova já
                // protegido (King ou Dis Pater no campo); a busca curta completa o resto da mesa.
                // Crimson Resonator se invoca (Stone Sweeper pode buscá-lo) e Magnamhut + Vision fazem o King ou a Dis Pater.
                new RouteGuide("Crimson Resonator + Magnamhut",
                    "?Stone Sweeper discards and searches Crimson Resonator",
                    "Crimson Resonator summons itself from the hand",
                    "?Lubellion discards and searches Magnamhut",
                    "?Stone Sweeper discards and searches Vision Resonator",
                    "Magnamhut banishes & and summons itself",
                    "Magnamhut schedules",
                    "Vision summons itself from the hand",
                    "?Lubellion (GY) releases Bystial Magnamhut",
                    "?Lubellion places Etude",
                    "Synchro Summon The Crimson King & Bystial Magnamhut & Vision Resonator | Synchro Summon Bystial Dis Pater & The Bystial Lubellion & Vision Resonator",
                    "?Vision (GY) searches",
                    "?King searches",
                    "Crimson Resonator summons & Soul Resonator & Darkness Resonator",
                    "Soul searches Chain Resonator",
                    "Darkness grants",
                    "?Dis Pater summons & from the banished zone",
                    "?Synchro Summon The Crimson King & Soul Resonator",
                    "?King searches",
                    "extra Normal Summon (Darkness) of Chain Resonator | Normal Summon Chain Resonator",
                    "Chain summons & from the Deck",
                    "?Activate Crimson Gaia",
                    "?Crimson Gaia searches",
                    "Darkness changes to Level 1 & Darkness Resonator",
                    "Synchro Summon Red Hypernova Dragon"),
                // Crimson Resonator se invoca, Soul (Resonator Call ou Gaia) busca Bone, Blade busca Lubellion e o King protege.
                new RouteGuide("Crimson Resonator + Soul",
                    "?Stone Sweeper discards and searches Crimson Resonator",
                    "Crimson Resonator summons itself from the hand",
                    "?Activate Crimson Gaia",
                    "Resonator Call searches Soul Resonator | Crimson Gaia searches Soul Resonator",
                    "Normal Summon Soul Resonator",
                    "Soul searches Bone Archfiend",
                    "Bone (hand) sends & from the hand and summons itself",
                    "Synchro Summon Crimson Blade Dragon & Bone Archfiend & Soul Resonator",
                    "Crimson Blade searches The Bystial Lubellion",
                    "Lubellion discards and searches Magnamhut",
                    "Crimson Resonator summons & Darkness Resonator & Vision Resonator",
                    "Darkness grants",
                    "Magnamhut banishes & and summons itself",
                    "Magnamhut schedules",
                    "Synchro Summon The Crimson King & Bystial Magnamhut & Vision Resonator",
                    "?Vision (GY) searches Crimson Gaia",
                    "?King searches Crimson Call",
                    "?Activate Crimson Gaia",
                    "Crimson Call searches Chain Resonator",
                    "extra Normal Summon (Darkness) of Chain Resonator",
                    "Chain summons Vision Resonator from the Deck",
                    "Darkness changes to Level 1 & Darkness Resonator & Vision Resonator",
                    "Synchro Summon Red Hypernova Dragon"),
                // Linha segura escolhida pela carteira completa (mão lenta sorteio_01, 30 s): Crimson Resonator + Fiend Piece
                // (da mão ou buscado pela Crimson Gaia) fazem o Red Rising, que revive o Crimson para invocar Soul + Darkness.
                // Com o Crimson Resonator negado ainda sobra mesa (camada 3, nota ~316), então conta como início seguro.
                new RouteGuide("Crimson Resonator + Fiend Piece",
                    "?Stone Sweeper discards and searches Crimson Resonator",
                    "Crimson Resonator summons itself from the hand",
                    "?Activate Crimson Gaia",
                    "?Crimson Gaia searches Fiend Piece Golem",
                    // Bone Archfiend na mão faz o mesmo papel (jogador: Crimson Resonator + Bone é combo), gastando a Invocação-Normal;
                    // o Chain então entra pela Invocação-Normal extra do Darkness.
                    "Fiend Piece summons itself from the hand | Normal Summon Bone Archfiend",
                    "?Fiend Piece reduces the Level",
                    "Synchro Summon Red Rising Dragon & Crimson Resonator & Fiend Piece Golem | Synchro Summon Red Rising Dragon & Crimson Resonator & Bone Archfiend",
                    "Red Rising revives Crimson Resonator",
                    "Crimson Resonator summons & Soul Resonator & Darkness Resonator",
                    "Soul searches Chain Resonator",
                    "Darkness grants",
                    "Normal Summon Chain Resonator | extra Normal Summon (Darkness) of Chain Resonator",
                    "Chain summons Synkron Resonator",
                    "Synchro Summon Crimson Blade Dragon & Chain Resonator & Red Rising Dragon",
                    "Crimson Blade searches The Bystial Lubellion",
                    "Lubellion discards and searches Magnamhut",
                    "Magnamhut banishes Red Rising Dragon",
                    "Magnamhut schedules",
                    "Synchro Summon The Crimson King & Bystial Magnamhut & Crimson Resonator",
                    "King searches Vision Resonator",
                    "Darkness changes to Level 1 & Soul Resonator & Darkness Resonator",
                    "Vision summons itself from the hand",
                    "Synchro Summon Red Hypernova Dragon"),
                // Caso raro (jogador + bateria, 2026-09-15): com Synchro do oponente no campo, o Chain (Invocação-Normal) traz o Soul
                // do Deck. O Bone se invoca mandando o próprio Chain do campo, o Synkron devolve o Chain à mão e ele volta pela
                // Invocação-Normal extra do Darkness para buscar o Vision; Hypernova protegido pela Dis Pater.
                new RouteGuide("Chain (opponent Synchro)",
                    "Normal Summon Chain Resonator",
                    "Chain summons Soul Resonator from the Deck",
                    "Soul searches Bone Archfiend",
                    "Bone (hand) sends & and summons itself",
                    "Bone sends Crimson Resonator & lowers the Level of Bone Archfiend",
                    "Synchro Summon Red Rising Dragon & Bone Archfiend & Soul Resonator",
                    "Red Rising revives Crimson Resonator",
                    "Crimson Resonator summons & Synkron Resonator & Darkness Resonator",
                    "Darkness grants",
                    "Synchro Summon Crimson Blade Dragon & Red Rising Dragon & Synkron Resonator",
                    "Synkron (GY) returns Chain Resonator",
                    "Crimson Blade searches The Bystial Lubellion",
                    "Lubellion discards and searches Magnamhut",
                    "Magnamhut banishes Synkron Resonator",
                    "Magnamhut schedules",
                    "Lubellion ( & releases Bystial Magnamhut",
                    "?Lubellion places Etude",
                    "extra Normal Summon (Darkness) of Chain Resonator",
                    "Chain summons Vision Resonator from the Deck",
                    "Synchro Summon Bystial Dis Pater & The Bystial Lubellion & Vision Resonator",
                    "?Vision (GY) searches Crimson Gaia",
                    "?Activate Crimson Gaia",
                    "Darkness changes to Level 1 & Darkness Resonator",
                    "Dis Pater summons Synkron Resonator from the banished zone",
                    "Synchro Summon Red Hypernova Dragon & Crimson Blade Dragon & Chain Resonator"),
                // Rota do jogador (2026-09-15) para Lubellion + Tuner nível 2: Lubellion vai ao GY e busca o Magnamhut, que bane o
                // Lubellion (LIGHT no GY) e se invoca; Vision (se invoca) ou Crimson (Invocação-Normal) fazem o Crimson King.
                // Lubellion + Chain não funciona (nível 9 é o Abyss, que pede Dragon DARK Synchro). Daí: King busca a Gaia ou a
                // Crimson Call, a Call busca do Deck (o King é Synchro que menciona RDA) e o Crimson invoca Soul + Darkness ao lado
                // do King para o Hypernova; a busca curta completa a mesa.
                new RouteGuide("Lubellion + Level 2 Tuner",
                    "Lubellion discards and searches Magnamhut",
                    "Magnamhut banishes The Bystial Lubellion",
                    "?Magnamhut schedules",
                    "Vision summons itself from the hand | Normal Summon Crimson Resonator",
                    "Synchro Summon The Crimson King & Bystial Magnamhut & Vision Resonator | Synchro Summon The Crimson King & Bystial Magnamhut & Crimson Resonator",
                    "?Vision (GY) searches Crimson Call",
                    "King searches Crimson Gaia | King searches Crimson Call",
                    "Crimson Call searches Crimson Resonator",
                    "?Activate Crimson Gaia",
                    "Normal Summon Crimson Resonator",
                    "Crimson Resonator summons & Soul Resonator & Darkness Resonator",
                    "Soul searches Chain Resonator | Soul searches Bone Archfiend",
                    "Darkness grants",
                    "Darkness changes to Level 1",
                    "extra Normal Summon (Darkness) of Chain Resonator",
                    "Chain summons Synkron Resonator",
                    "Synchro Summon Red Hypernova Dragon"),
                // =========================================================================================================
                // Rotas para Maxx "C" / Fuwalos ativos (jogador, 2026-09-15). Objetivo: dar o mínimo de cartas ao oponente e
                // ainda montar interrupção. Ordem das armadilhas em todas elas: King's Resonance, depois RDA's Chain, depois
                // Red Zone (a que ainda não estiver na mão). Com todas na mão, a busca serve para preparar o turno seguinte,
                // e o fim segue a regra normal de baixar as armadilhas e encerrar.
                // =========================================================================================================
                new RouteGuide(true, "Maxx C - Power Vice",
                    "Power Vice summons itself from the hand",
                    "?Power Vice searches Soul Resonator",
                    "Normal Summon Soul Resonator",
                    "?Soul searches Bone Archfiend",
                    "Bone (hand) sends & Power Vice Dragon from the field",
                    "Bone sends Vision Resonator & raises the Level of Bone Archfiend",
                    "Vision (GY) searches Crimson Gaia",
                    "Activate Crimson Gaia",
                    "Crimson Gaia searches King's Resonance | Crimson Gaia searches Red Dragon Archfiend's Chain | Crimson Gaia searches Red Zone",
                    "Synchro Summon The Crimson King & Bone Archfiend & Soul Resonator",
                    "King searches King's Resonance | King searches Red Dragon Archfiend's Chain | King searches Red Zone | King searches"),
                // Bone já na mão: o Vision vira a busca da Gaia, que traz o Darkness para o King.
                new RouteGuide(true, "Maxx C - Bone",
                    "Normal Summon Bone Archfiend | Bone (hand) sends",
                    "Bone sends Vision Resonator & raises the Level of Bone Archfiend",
                    "Vision (GY) searches Crimson Gaia",
                    "Activate Crimson Gaia",
                    "Crimson Gaia searches Darkness Resonator",
                    "Darkness reveals RDA",
                    "Darkness grants",
                    "Synchro Summon The Crimson King & Bone Archfiend & Darkness Resonator",
                    "King searches King's Resonance | King searches Red Dragon Archfiend's Chain | King searches Red Zone | King searches"),
                // Só o Soul: busca o Bone, que se invoca descartando, e a Gaia pega a armadilha que falta.
                new RouteGuide(true, "Maxx C - Soul",
                    "Normal Summon Soul Resonator",
                    "Soul searches Bone Archfiend",
                    "Bone (hand) sends",
                    "Bone sends Vision Resonator & raises the Level of Bone Archfiend",
                    "Vision (GY) searches Crimson Gaia",
                    "Activate Crimson Gaia",
                    "Crimson Gaia searches King's Resonance | Crimson Gaia searches Red Dragon Archfiend's Chain | Crimson Gaia searches Red Zone",
                    "?Synchro Summon The Crimson King & Bone Archfiend & Soul Resonator",
                    "?King searches"),
                // Fuwalos: só Invocação-Especial do Deck/Extra dá carta, então dá para ir até o Hypernova.
                new RouteGuide(true, "Fuwalos - Soul + Resonator",
                    "Normal Summon Soul Resonator",
                    "Soul searches Bone Archfiend",
                    "Bone (hand) sends",
                    "Bone sends Crimson Resonator & lowers the Level of Bone Archfiend",
                    "Synchro Summon Red Rising Dragon & Bone Archfiend & Soul Resonator",
                    "Red Rising revives Crimson Resonator",
                    "Crimson Resonator summons & Vision Resonator & Darkness Resonator",
                    "Darkness grants",
                    "extra Normal Summon (Darkness) of",
                    "Vision (GY) searches Crimson Gaia",
                    "Activate Crimson Gaia",
                    "Crimson Gaia searches King's Resonance | Crimson Gaia searches Red Dragon Archfiend's Chain | Crimson Gaia searches Red Zone",
                    "?Darkness changes to Level 1",
                    "?Synchro Summon Red Hypernova Dragon"),
                // Fuwalos, linha mais arriscada e de melhor resultado: King cedo, Crimson traz Synkron + Soul e o Hypernova fecha.
                new RouteGuide(true, "Fuwalos - Power Vice",
                    "Power Vice summons itself from the hand",
                    "Power Vice searches Darkness Resonator",
                    "Darkness reveals RDA",
                    "Darkness grants",
                    "Synchro Summon The Crimson King & Power Vice Dragon & Darkness Resonator",
                    "King searches Crimson Resonator",
                    "Crimson Resonator summons & Synkron Resonator & Soul Resonator",
                    "Soul searches Bone Archfiend",
                    "Normal Summon Bone Archfiend",
                    "Bone sends Vision Resonator & raises the Level of Bone Archfiend",
                    "Vision (GY) searches Crimson Gaia",
                    "Synchro Summon Red Rising Dragon & Bone Archfiend & Synkron Resonator",
                    "Red Rising revives Darkness Resonator",
                    "Synkron (GY) returns Vision Resonator",
                    "Vision summons itself from the hand",
                    "Darkness changes to Level 1",
                    "Synchro Summon Red Hypernova Dragon",
                    "Activate Crimson Gaia",
                    "Crimson Gaia searches King's Resonance | Crimson Gaia searches Red Dragon Archfiend's Chain | Crimson Gaia searches Red Zone"),
                // =========================================================================================================
                // Rotas para Droll & Lock Bird resolvido (jogador, 2026-09-15). Nada mais sai do Deck para a mão, mas efeitos que
                // invocam direto do Deck e que mandam ao GY continuam funcionando, e a linha termina no Burning Soul + Quetzacoatl.
                // =========================================================================================================
                // Sob Droll nada sai do Deck para a mão: os passos de busca ficam opcionais e a rota segue com o que já está na mão.
                new RouteGuide("Droll - Soul",
                    "Normal Summon Soul Resonator",
                    "?Soul searches Bone Archfiend",
                    "Bone (hand) sends",
                    "Bone sends Crimson Resonator & lowers the Level of Bone Archfiend",
                    "Synchro Summon Red Rising Dragon & Bone Archfiend & Soul Resonator",
                    "Red Rising revives Crimson Resonator",
                    "Crimson Resonator summons & Chain Resonator & Darkness Resonator",
                    "Darkness grants",
                    "Synchro Summon Zalen the Shackled Dragon & Red Rising Dragon & Chain Resonator",
                    "Darkness changes to Level 1 & Darkness Resonator",
                    "Synchro Summon Scarred Dragon Archfiend & Zalen the Shackled Dragon & Darkness Resonator",
                    "Synchro Summon Bystial Dis Pater & Scarred Dragon Archfiend & Crimson Resonator",
                    "Scarred summons Red Dragon Archfiend from the Extra Deck",
                    "Burning Soul summons itself from the GY banishing",
                    "Burning Soul adds Chain Resonator",
                    "extra Normal Summon (Darkness) of Chain Resonator",
                    "Chain summons Synkron Resonator from the Deck",
                    "Dis Pater summons Zalen the Shackled Dragon from the banished zone",
                    "Synchro Summon The Crimson King & Zalen the Shackled Dragon & Synkron Resonator",
                    "Synkron (GY) returns Darkness Resonator",
                    "Synchro Summon Hot Red Dragon Archfiend Abyss & Red Dragon Archfiend & Chain Resonator",
                    "Darkness reveals RDA",
                    "Synchro Summon Crimson Dragon Quetzacoatl & Hot Red Dragon Archfiend Abyss & Darkness Resonator",
                    "Quetzacoatl revives"),
                // Power Vice + Darkness ou Chain na mão (um busca o outro).
                new RouteGuide("Droll - Power Vice + Resonator",
                    "Power Vice summons itself from the hand",
                    "?Power Vice searches Darkness Resonator | Power Vice searches Chain Resonator",
                    "Darkness reveals RDA",
                    "Darkness grants",
                    "Darkness changes to Level 1 & Darkness Resonator",
                    "Synchro Summon Red Rising Dragon & Power Vice Dragon & Darkness Resonator",
                    "Red Rising revives Darkness Resonator",
                    "Normal Summon Chain Resonator | extra Normal Summon (Darkness) of Chain Resonator",
                    "Chain summons Synkron Resonator from the Deck",
                    "Synchro Summon Zalen the Shackled Dragon & Red Rising Dragon & Chain Resonator",
                    "Synchro Summon Scarred Dragon Archfiend & Zalen the Shackled Dragon & Synkron Resonator",
                    "Synkron (GY) returns Chain Resonator",
                    "extra Normal Summon (Darkness) of Chain Resonator | Normal Summon Chain Resonator",
                    "Chain summons Vision Resonator from the Deck",
                    "Synchro Summon Hot Red Dragon Archfiend Abyss & Scarred Dragon Archfiend & Chain Resonator",
                    "Scarred summons Red Dragon Archfiend from the Extra Deck",
                    "Synchro Summon Bystial Dis Pater & Red Dragon Archfiend & Vision Resonator",
                    "Burning Soul summons itself from the GY banishing",
                    "Burning Soul adds Synkron Resonator",
                    "Dis Pater summons Zalen the Shackled Dragon from the banished zone",
                    "Synkron summons itself from the hand",
                    "Synchro Summon The Crimson King & Zalen the Shackled Dragon & Synkron Resonator",
                    "Synkron (GY) returns Vision Resonator",
                    "Synchro Summon Crimson Dragon Quetzacoatl & Hot Red Dragon Archfiend Abyss & Darkness Resonator",
                    "Quetzacoatl revives"),
                // Power Vice + Bone: o Bone usa o Power Vice como custo e a linha fecha no Supernova + Quetzacoatl.
                new RouteGuide("Droll - Power Vice + Bone",
                    "Power Vice summons itself from the hand",
                    "?Power Vice searches Darkness Resonator",
                    "Darkness reveals RDA",
                    "Darkness grants",
                    "Bone (hand) sends & Power Vice Dragon from the field",
                    "Bone sends Crimson Resonator & lowers the Level of Bone Archfiend",
                    "Synchro Summon Red Rising Dragon & Bone Archfiend & Darkness Resonator",
                    "Red Rising revives Crimson Resonator",
                    "Crimson Resonator summons & Chain Resonator & Synkron Resonator",
                    "Synchro Summon Zalen the Shackled Dragon & Red Rising Dragon & Chain Resonator",
                    "Synchro Summon Scarred Dragon Archfiend & Zalen the Shackled Dragon & Synkron Resonator",
                    "Synkron (GY) returns Chain Resonator",
                    "extra Normal Summon (Darkness) of Chain Resonator | Normal Summon Chain Resonator",
                    "Chain summons Vision Resonator from the Deck",
                    "Synchro Summon Hot Red Dragon Archfiend Abyss & Scarred Dragon Archfiend & Chain Resonator",
                    "Scarred summons Red Dragon Archfiend from the Extra Deck",
                    "Synchro Summon Bystial Dis Pater & Red Dragon Archfiend & Crimson Resonator",
                    "Burning Soul summons itself from the GY banishing",
                    "Burning Soul adds Synkron Resonator",
                    "Synkron summons itself from the hand",
                    "Dis Pater summons Darkness Resonator from the banished zone",
                    "Darkness changes to Level 1 & Darkness Resonator & Vision Resonator",
                    "Synchro Summon Red Supernova Dragon",
                    "Synkron (GY) returns Vision Resonator",
                    "Vision summons itself from the hand",
                    "Synchro Summon Crimson Dragon Quetzacoatl & Bystial Dis Pater & Vision Resonator",
                    "Quetzacoatl revives"),
                // Bateria "mãos jogáveis" (2026-09-15): linha segura da carteira completa para Crimson Resonator + Bone (com o
                // Crimson negado ainda sobra nota ~270). Blade busca Lubellion, King busca a Gaia, a Gaia busca a Crimson Call e a
                // Dis Pater traz o Red Rising banido para o Supernova; a busca curta completa com Zalen e Quetzacoatl.
                new RouteGuide("Crimson Resonator + Bone",
                    "Crimson Resonator summons itself from the hand",
                    "Normal Summon Bone Archfiend",
                    "Synchro Summon Red Rising Dragon & Bone Archfiend & Crimson Resonator",
                    "Red Rising revives Crimson Resonator",
                    "Crimson Resonator summons & Soul Resonator & Darkness Resonator",
                    "Soul searches Chain Resonator",
                    "Darkness grants",
                    "extra Normal Summon (Darkness) of Chain Resonator",
                    "Chain summons Vision Resonator from the Deck",
                    "Synchro Summon Crimson Blade Dragon & Chain Resonator & Red Rising Dragon",
                    "Crimson Blade searches The Bystial Lubellion",
                    "Lubellion discards and searches Magnamhut",
                    "Magnamhut banishes Red Rising Dragon",
                    "Magnamhut schedules",
                    "Synchro Summon The Crimson King & Bystial Magnamhut & Vision Resonator",
                    "?Vision (GY) searches",
                    "King searches Crimson Gaia",
                    "Synchro Summon Bystial Dis Pater & Crimson Blade Dragon & Soul Resonator",
                    "Activate Crimson Gaia",
                    "Crimson Gaia searches Crimson Call",
                    "Crimson Call searches Synkron Resonator",
                    "Dis Pater summons Red Rising Dragon from the banished zone",
                    "Synkron summons itself from the hand",
                    "Synchro Summon Red Supernova Dragon & Crimson Resonator & Red Rising Dragon & Synkron Resonator & Darkness Resonator"),
                // Bateria "mãos jogáveis" (2026-09-15): Darkness se invoca e dá ao Fiend Piece o Tuner Fiend no campo; o King
                // busca a Crimson Call (que agora pode buscar do Deck) e Chain + Soul + Vision fecham o Hypernova com o King.
                new RouteGuide("Darkness + Fiend Piece",
                    "?Stone Sweeper discards and searches Darkness Resonator",
                    "Darkness reveals RDA",
                    "Darkness grants",
                    "Fiend Piece summons itself from the hand",
                    "Synchro Summon The Crimson King & Fiend Piece Golem & Darkness Resonator",
                    "?Fiend Piece (GY) revives Darkness Resonator",
                    "King searches Crimson Call",
                    "Crimson Call searches Chain Resonator",
                    "Normal Summon Chain Resonator | extra Normal Summon (Darkness) of Chain Resonator",
                    "Chain summons Soul Resonator from the Deck",
                    "Soul searches Vision Resonator",
                    "Vision summons itself from the hand",
                    "Darkness changes to Level 1 & Soul Resonator & Darkness Resonator & Vision Resonator",
                    "Synchro Summon Red Hypernova Dragon & The Crimson King"),
            };

            private const int RouteBudget = 4000;       // estados visitados por rota (encaixe com retrocesso)
            private const int RouteMaxSkips = 3;        // passos obrigatórios que a mão pode não permitir
            private const int RouteMinStepsMatched = 6; // abaixo disso a rota não serve para esta mão
            private const double RouteSafeFallbackScore = 150; // mesa mínima com o starter negado para a rota contar como segura
            // Estas três são as buscas que REALMENTE rodam em partida (a carteira pesada quase nunca dispara, medido em
            // 2026-09-18). São objetos mutáveis de propósito: o modo rápido ajusta Width e TimeLimitMs sem recompilar,
            // e os testes offline varrem os valores por reflexão.
            internal static readonly SearchOptions RouteVerifySearch =
                new SearchOptions { Name = "verify_w1000", Width = 1000, PerRoute = 30, BookWeight = 15, TimeLimitMs = 4000 };
            internal static readonly SearchOptions RouteVerifySearchProtected =
                new SearchOptions { Name = "verify_w1000_protected", Width = 1000, PerRoute = 30, BookWeight = 15, TimeLimitMs = 4000, Noise = 10, Seed = 1, ForbidUnprotectedNova = true };
            private const int RouteMinStepsReplan = 4;  // replanejamento: trecho mínimo encaixado (a rota precisa ir até o fim)
            internal static readonly SearchOptions RouteFinishSearch =
                new SearchOptions { Name = "route_w1000", Width = 1000, PerRoute = 30, BookWeight = 15, TimeLimitMs = 4000 };

            /// <summary>
            /// Aplica um preset de modo às buscas que rodam de verdade. Ver o bloco MODO DE DECISÃO no executor.
            /// Os dois últimos parâmetros são a busca do plano B (BaitFallbackSearch), que roda uma vez por rota
            /// ideal na fase "atalho" e uma vez por concorrente na "resistência" — por isso ela pesa duas vezes.
            /// Os valores padrão preservam o comportamento de hoje para quem chama com 3 argumentos.
            /// </summary>
            internal static void ApplyMode(int width, int timeLimitMs, int contenderCount,
                int fallbackWidth = 400, int fallbackTimeLimitMs = 1500)
            {
                BaitFallbackSearch.Width = fallbackWidth;
                BaitFallbackSearch.TimeLimitMs = fallbackTimeLimitMs;
                // Todas as outras buscas do planejador seguem a largura e o teto do modo. O inventário completo está
                // na tabela do bloco MODO DE DECISÃO; deixar uma de fora é o erro que já aconteceu duas vezes.
                SeedSearch.Width = width;
                SeedSearch.TimeLimitMs = timeLimitMs;
                StarterAlternativeSearch.Width = Math.Min(StarterAlternativeSearch.Width, width);
                StarterAlternativeSearch.TimeLimitMs = Math.Min(StarterAlternativeSearch.TimeLimitMs, timeLimitMs);
                RouteVerifySearch.Width = width;
                RouteVerifySearch.TimeLimitMs = timeLimitMs;
                RouteVerifySearchProtected.Width = width;
                RouteVerifySearchProtected.TimeLimitMs = timeLimitMs;
                RouteFinishSearch.Width = width;
                RouteFinishSearch.TimeLimitMs = timeLimitMs;
                ResilienceContenders = contenderCount;
            }

            private sealed class RouteFollower
            {
                public RouteGuide Route;
                public int Budget = RouteBudget;
                public int BestIndex = -1;
                public int BestMatched;
                public bool BestEnd;          // o melhor encaixe chegou ao último passo da rota
                public bool Perfect;          // rota completa sem pular passo obrigatório: para de testar alternativas
                public List<RdaMove> BestPath = new List<RdaMove>();
                private readonly List<RdaMove> _path = new List<RdaMove>();
                private int _matched;

                public void Follow(RdaState state, int index, int skips)
                {
                    bool end = index >= Route.Steps.Length;
                    if ((end && !BestEnd) || (end == BestEnd && (_matched > BestMatched || (_matched == BestMatched && index > BestIndex))))
                    {
                        BestEnd = end;
                        BestIndex = index;
                        BestMatched = _matched;
                        BestPath = new List<RdaMove>(_path);
                    }
                    if (index >= Route.Steps.Length)
                    {
                        if (skips == 0)
                            Perfect = true;
                        return;
                    }
                    if (--Budget <= 0)
                        return;
                    RouteStep step = Route.Steps[index];
                    List<RdaMove> moves = RdaRules.Successors(state);
                    int tried = 0;
                    foreach (RdaMove move in moves)
                    {
                        if (move.Action.Kind == PlanKind.TriggerDecline || !step.Matches(move.Action.Text))
                            continue;
                        _path.Add(move);
                        _matched++;
                        Follow(move.State, index + 1, skips);
                        _matched--;
                        _path.RemoveAt(_path.Count - 1);
                        if (Perfect || ++tried >= 6 || Budget <= 0)
                            return;
                    }
                    if (tried > 0)
                        return;
                    if (state.Pending.Length > 0)
                    {
                        // Gatilho fora da rota: recusa se der; senão aceita a primeira opção (gatilho que só adiciona recurso).
                        RdaMove chosen = default(RdaMove);
                        bool found = false;
                        foreach (RdaMove move in moves)
                        {
                            if (move.Action.Kind == PlanKind.TriggerDecline) { chosen = move; found = true; break; }
                        }
                        if (!found && moves.Count > 0) { chosen = moves[0]; found = true; }
                        if (!found)
                            return;
                        _path.Add(chosen);
                        Follow(chosen.State, index, skips);
                        _path.RemoveAt(_path.Count - 1);
                        return;
                    }
                    if (step.Optional)
                        Follow(state, index + 1, skips);
                    else if (skips < RouteMaxSkips)
                        Follow(state, index + 1, skips + 1);
                }
            }

            private static RdaPlan StitchPlan(RdaState root, string name, List<PlanStep> prefix, double prefixExposure, bool prefixUnprotected, RdaPlan suffix)
            {
                var plan = new RdaPlan { Search = name, Root = root };
                plan.Steps.AddRange(prefix);
                RdaState final = prefix.Count > 0 ? prefix[prefix.Count - 1].After : root;
                plan.Exposure = prefixExposure;
                plan.NovaUnprotected = prefixUnprotected;
                // Cartas que os passos da própria rota dão ao oponente (jogador, 2026-09-15: "ele sempre vai dar cartas").
                // Antes só a busca do sufixo contava, então uma rota encaixada 100% saía com 0 cartas dadas.
                RdaState previous = root;
                foreach (PlanStep step in prefix)
                {
                    int newMonsters, fromDeckOrExtra;
                    CountNewMonsters(previous, step.After, out newMonsters, out fromDeckOrExtra);
                    bool normalSummon = step.Action.Kind == PlanKind.NormalSummon || step.Action.Kind == PlanKind.ExtraNormalSummon;
                    if (newMonsters > 0 && !normalSummon
                        && (step.After.HasFlag(RdaState.FlagMaxxC) || (step.After.HasFlag(RdaState.FlagFuwalos) && fromDeckOrExtra > 0)))
                        plan.DrawEvents++;
                    previous = step.After;
                }
                // Vale para a rota completa também (sem busca de sufixo): o custo dos passos da rota é aceito pelo jogador.
                plan.RouteDrawEvents = plan.DrawEvents;
                if (suffix != null && suffix.Steps.Count > 0)
                {
                    plan.Steps.AddRange(suffix.Steps);
                    final = suffix.Final;
                    plan.Exposure += suffix.Exposure;
                    plan.DrawEvents += suffix.DrawEvents;   // soma: o prefixo da rota já contou as cartas dadas pelos passos dele
                    plan.RouteDrawEvents = plan.DrawEvents; // rota de referência: as cartas dadas por ela são aceitas pelo jogador
                    plan.NibiruPenalty = suffix.NibiruPenalty;
                    plan.NibiruExposed = suffix.NibiruExposed;
                    plan.NovaUnprotected |= suffix.NovaUnprotected;
                }
                plan.Final = final;
                plan.Depth = plan.Steps.Count;
                int tier = RdaEvaluator.Tier(final);
                if (plan.NovaUnprotected)
                    tier = Math.Min(3, tier + 2);
                plan.Tier = tier;
                // As cartas dadas pelos passos da própria rota não descontam nota: a rota é a referência escrita pelo jogador para
                // essa situação (Maxx "C"/Fuwalos), e ele já aceitou esse custo. Só o que a busca do sufixo inventa é penalizado.
                plan.Score = RdaEvaluator.SelectionScore(final, root.Hand)
                    - (plan.NovaUnprotected ? UnprotectedNovaPenalty : 0)
                    - (ExposureWeight * plan.Exposure + plan.NibiruPenalty
                        + DrawPenalty * Math.Max(0, plan.DrawEvents - plan.RouteDrawEvents));
                return plan;
            }

            private static RdaPlan RoutePlan(RdaState root, RouteGuide route, bool fromStart,
                CancellationToken token = default(CancellationToken))
            {
                var follower = new RouteFollower { Route = route };
                // Primeiro plano: a rota começa do primeiro passo. Replanejamento: o combo já andou, então tenta começar de cada passo.
                int starts = fromStart ? 1 : route.Steps.Length;
                for (int start = 0; start < starts && !follower.Perfect && follower.Budget > 0; ++start)
                    follower.Follow(root, start, 0);
                if (follower.BestPath.Count == 0 || follower.BestMatched < (fromStart ? RouteMinStepsMatched : RouteMinStepsReplan))
                    return null;
                if (!fromStart && !follower.BestEnd)
                    return null;
                var prefix = new List<PlanStep>();
                RdaState before = root;
                double exposure = 0;
                bool unprotected = false;
                foreach (RdaMove move in follower.BestPath)
                {
                    if (move.Action.Kind == PlanKind.Synchro && (move.Action.CardId == RdaCards.Hypernova || move.Action.CardId == RdaCards.Supernova)
                        && !HasProtection(before))
                        unprotected = true;
                    exposure += StepExposure(move.State);
                    prefix.Add(new PlanStep { Action = move.Action, After = move.State });
                    before = move.State;
                }
                string name = string.Format("route {0} ({1} of {2} steps)", route.Name, follower.BestMatched, route.Steps.Length);
                RdaPlan onlyRoute = StitchPlan(root, name, prefix, exposure, unprotected, null);
                onlyRoute.RouteComplete = follower.BestMatched == route.Steps.Length;
                if (!fromStart)
                    return onlyRoute;
                RdaPlan suffix = Plan(before, RouteFinishSearch.WithToken(token), null);
                RdaPlan finished = StitchPlan(root, name + " + search", prefix, exposure, unprotected, suffix);
                finished.RouteComplete = onlyRoute.RouteComplete;
                return finished.Tier < onlyRoute.Tier || (finished.Tier == onlyRoute.Tier && finished.Score > onlyRoute.Score) ? finished : onlyRoute;
            }

            // Iniciantes (jogador, 2026-09-15): Power Vice, Soul, Bone, Stone Sweeper, Foolish Burial, Resonator Call e Crimson Gaia
            // abrem combo sozinhos; Crimson Resonator e Fiend Piece precisam de parceiro; Lubellion (busca Magnamhut) abre com
            // Tuner nível 2 na mão (Magnamhut + Vision/Crimson = Crimson King). Crimson Call não entra: sem RDA no campo ela só
            // pega do GY. Sem o parceiro a carta não conta como iniciante (não impede o atalho de outra rota).
            // Casos raros com monstro do oponente (jogador, 2026-09-15): Chain (Synchro no campo) invoca um Resonator do Deck, como o
            // Soul; Synkron (Synchro no campo) e Vision (DARK nível 5+ no campo) precisam de Bone ou Fiend Piece na mão.
            private static bool StarterInHand(RdaState state, int starter)
            {
                int[] hand = state.Hand;
                if (Array.IndexOf(hand, starter) < 0)
                    return false;
                bool fiendPartner = Array.IndexOf(hand, RdaCards.Bone) >= 0 || Array.IndexOf(hand, RdaCards.FiendPiece) >= 0;
                if (starter == RdaCards.Chain)
                    return state.EnemySynchro;
                if (starter == RdaCards.Synkron)
                    return state.EnemySynchro && fiendPartner;
                if (starter == RdaCards.Vision)
                    return state.EnemyDarkLevel5 && fiendPartner;
                if (starter == RdaCards.Crimson)
                    return Array.IndexOf(hand, RdaCards.FiendPiece) >= 0 || Array.IndexOf(hand, RdaCards.Bone) >= 0;
                if (starter == RdaCards.FiendPiece)
                    return Array.IndexOf(hand, RdaCards.Darkness) >= 0 || Array.IndexOf(hand, RdaCards.Crimson) >= 0;
                if (starter == RdaCards.Lubellion)
                    return Array.IndexOf(hand, RdaCards.Vision) >= 0 || Array.IndexOf(hand, RdaCards.Crimson) >= 0;
                return true;
            }
            private static readonly int[] StarterCards =
            {
                RdaCards.PowerVice, RdaCards.Soul, RdaCards.Bone, RdaCards.StoneSweeper, RdaCards.Foolish, RdaCards.ResonatorCall,
                RdaCards.Gaia, RdaCards.Crimson, RdaCards.FiendPiece, RdaCards.Lubellion, RdaCards.Chain, RdaCards.Synkron, RdaCards.Vision
            };
            private const int StarterOpeningSteps = 4; // o iniciante precisa aparecer nos primeiros passos da rota

            // Todo iniciante da mão aparece na abertura de alguma rota encaixada com mesa boa (camada 0 ou 1, nova protegido).
            private static bool StartersCovered(RdaState root, List<RdaPlan> guided)
            {
                bool any = false;
                foreach (int starter in StarterCards)
                {
                    if (!StarterInHand(root, starter))
                        continue;
                    any = true;
                    bool covered = guided.Any(plan => plan.Tier <= 1 && !plan.NovaUnprotected &&
                        plan.Steps.Take(StarterOpeningSteps).Any(step => step.Action.CardId == starter));
                    if (!covered)
                        return false;
                }
                return any;
            }

            // Mesa que sobra se o starter (1º efeito crítico) do plano for negado.
            private static RdaPlan StarterFallback(RdaPlan plan, CancellationToken token = default(CancellationToken))
            {
                int index = plan.Steps.FindIndex(step => IsCriticalAction(step.Action));
                if (index < 0)
                    return null;
                return Plan(NegateBait(plan.Steps[index].After, plan.Steps[index].Action), BaitFallbackSearch.WithToken(token), null);
            }

            private static List<RdaPlan> GuidedPlans(RdaState root, bool fromStart, bool maxx = false,
                CancellationToken token = default(CancellationToken))
            {
                RouteGuide[] routes = RouteGuides.Where(route => route.ForMaxx == maxx).ToArray();
                var results = new RdaPlan[routes.Length];
                Parallel.For(0, routes.Length, i => results[i] = RoutePlan(root, routes[i], fromStart, token));
                return results.Where(plan => plan != null && plan.Steps.Count > 0).ToList();
            }

            private static int SeedSynchroCount = 3;  // semente: a linha acabou de fazer o N-ésimo Synchro (static para calibrar fora do jogo)
            private static int SeedCount = 4;         // sementes continuadas (rotas do livro primeiro, depois prioridade)

            private static int LowestBit(int mask)
            {
                for (int bit = 0; bit < 31; ++bit)
                    if ((mask & (1 << bit)) != 0)
                        return bit;
                return 0;
            }
            private static readonly SearchOptions SeedSearch =
                new SearchOptions { Name = "seed_w1000", Width = 1000, PerRoute = 30, BookWeight = 15, TimeLimitMs = 4000 };

            private static string OpeningText(RdaPlan plan)
            {
                int index = plan.Steps.FindIndex(step => IsCriticalAction(step.Action));
                return index < 0 ? "-" : plan.Steps[index].Action.Text;
            }

            private static RdaKey OpeningEffect(RdaPlan plan)
            {
                int index = plan.Steps.FindIndex(step => IsCriticalAction(step.Action));
                return index < 0 ? RdaKey.None : plan.Steps[index].Action.Effect;
            }

            private static List<RdaPlan> SeededPlans(RdaState root, IList<RdaPlan> plans, RdaPlan best,
                CancellationToken token = default(CancellationToken))
            {
                // Uma semente por starter + rota do livro; fora as que já estão no caminho do melhor plano. Rotas do livro (combos do
                // jogador) primeiro, depois a prioridade.
                var bestStates = new HashSet<RdaState>(best.Steps.Select(step => step.After));
                List<RdaPlan> prefixes = plans.SelectMany(plan => plan.Seeds)
                    .Where(prefix => !bestStates.Contains(prefix.Final))
                    .GroupBy(prefix => prefix.Search)
                    .Select(group => group.OrderByDescending(prefix => prefix.Score).First())
                    .OrderByDescending(prefix => prefix.Search.Contains("/route") ? 1 : 0)
                    .ThenByDescending(prefix => prefix.Score)
                    .Take(SeedCount)
                    .ToList();
                var results = new RdaPlan[prefixes.Count];
                Parallel.For(0, prefixes.Count, i =>
                {
                    RdaPlan prefix = prefixes[i];
                    RdaPlan suffix = Plan(prefix.Final, SeedSearch.WithToken(token), null);
                    if (suffix.Steps.Count == 0)
                        return;
                    var stitched = new RdaPlan
                    {
                        Search = "seed(" + prefix.Search + ")", Root = root, Final = suffix.Final, Tier = suffix.Tier,
                        Depth = prefix.Steps.Count + suffix.Depth, Exposure = prefix.Exposure + suffix.Exposure,
                        DrawEvents = prefix.DrawEvents + suffix.DrawEvents, NibiruPenalty = prefix.NibiruPenalty + suffix.NibiruPenalty,
                        NibiruExposed = prefix.NibiruExposed || suffix.NibiruExposed, NovaUnprotected = prefix.NovaUnprotected || suffix.NovaUnprotected
                    };
                    stitched.Steps.AddRange(prefix.Steps);
                    stitched.Steps.AddRange(suffix.Steps);
                    // Nota refeita com a mão do começo do turno (a da busca curta usava a mão do meio da linha).
                    stitched.RouteDrawEvents = prefix.RouteDrawEvents;
                    stitched.Score = RdaEvaluator.SelectionScore(stitched.Final, root.Hand)
                        - (stitched.NovaUnprotected ? UnprotectedNovaPenalty : 0)
                        - (ExposureWeight * stitched.Exposure + stitched.NibiruPenalty + DrawPenalty * stitched.DrawEvents);
                    results[i] = stitched;
                });
                return results.Where(plan => plan != null).ToList();
            }

            // blockedRootActions: textos de ações que o jogo recusou no estado atual (não aparecem no menu).
            // Só valem para o primeiro passo; depois dele o estado já é outro.
            public static RdaPlan Plan(RdaState root, SearchOptions options, ICollection<string> blockedRootActions)
            {
                SearchGateEnter();
                try
                {
                    return PlanGated(root, options, blockedRootActions);
                }
                finally
                {
                    SearchGateLeave();
                }
            }

            private static RdaPlan PlanGated(RdaState root, SearchOptions options, ICollection<string> blockedRootActions)
            {
                var watch = Stopwatch.StartNew();
                var random = new Random(options.Seed);
                var rootNode = new Node { State = root, RouteMask = AllRoutes, Summoned = root.SummonedThisTurn };
                var seeds = options.CollectSeeds ? new Dictionary<int, KeyValuePair<double, Node>>() : null;
                bool maxxMode = root.HasFlag(RdaState.FlagMaxxC) || root.HasFlag(RdaState.FlagFuwalos);

                // Melhor mesa: camada primeiro, nota de seleção depois (as preferências do jogador só entram aqui).
                Node bestNode = rootNode;
                // Melhores finais diferentes desta busca (para a escolha por resistência na carteira).
                var finalCandidates = new List<Candidate>();
                // Medição de tempo: quando (ms e profundidade) a melhor mesa final desta busca apareceu.
                long bestFoundMs = 0;
                int bestFoundDepth = 0;
                // Medição por etapa (ticks acumulados): gerar jogadas, montar/deduplicar nós, avaliar, ordenar/cortar o feixe.
                var phase = new Stopwatch();
                long ticksSuccessors = 0, ticksNodes = 0, ticksEvaluate = 0, ticksSort = 0, expanded = 0, generated = 0;
                int bestTier = root.Pending.Length == 0 ? (maxxMode ? 3 : RdaEvaluator.Tier(root)) : 4;
                double bestScore = root.Pending.Length == 0 ? RdaEvaluator.SelectionScore(root, root.Hand) : double.MinValue;
                // Melhor nota neutra vista em cada camada: a nota de seleção (mais cara) só é calculada para estados
                // da melhor camada que estão perto dessa referência.
                var bestNeutral = new double[5];
                for (int t = 0; t < bestNeutral.Length; ++t)
                    bestNeutral[t] = double.MinValue;

                var frontier = new List<Node> { rootNode };
                int depth = 0;
                // Contador para a checagem de tempo/cancelamento a cada bloco de avaliações (P4): o limite antigo só era
                // conferido no fim da camada, então uma profundidade com feixe grande estourava o orçamento em segundos.
                int timeCheckCount = 0;
                for (; depth < options.MaxDepth; ++depth)
                {
                    var candidates = new Dictionary<RdaState, Node>();
                    foreach (Node node in frontier)
                    {
                        // Granularidade por nó na geração de jogadas: sair aqui para o mais cedo possível quando a busca
                        // foi cancelada ou o limite estourou (a avaliação dos candidatos já colhidos ainda roda abaixo).
                        if (options.Token.IsCancellationRequested || watch.ElapsedMilliseconds > options.TimeLimitMs)
                            break;
                        phase.Restart();
                        List<RdaMove> successors = RdaRules.Successors(node.State);
                        if (ExpansionCounter != null)
                            ExpansionCounter.AddOrUpdate(node.State, 1, (key, value) => value + 1);
                        ticksSuccessors += phase.ElapsedTicks;
                        expanded++;
                        generated += successors.Count;
                        phase.Restart();
                        foreach (RdaMove move in successors)
                        {
                            if (node == rootNode && blockedRootActions != null
                                && (blockedRootActions.Contains(move.Action.Text) || blockedRootActions.Contains(EffectBlockKey(move.Action))))
                                continue;
                            var child = new Node
                            {
                                State = move.State, Parent = node, Action = move.Action,
                                SynchroCount = node.SynchroCount, RouteMask = node.RouteMask, NovaUnprotected = node.NovaUnprotected,
                                Exposure = node.Exposure + StepExposure(move.State),
                                DrawEvents = node.DrawEvents, NibiruPenalty = node.NibiruPenalty, NibiruExposed = node.NibiruExposed,
                                CriticalSeen = node.CriticalSeen, BaitLate = node.BaitLate, FirstCritical = node.FirstCritical
                            };
                            bool accepted = move.Action.Kind == PlanKind.Activate || move.Action.Kind == PlanKind.TriggerAccept;
                            if (accepted && move.Action.CardId == RdaCards.ResonatorCall && move.Action.Kind == PlanKind.Activate && node.CriticalSeen)
                                child.BaitLate = true;
                            if (accepted && CriticalEffects.Contains(move.Action.Effect))
                                child.CriticalSeen = true;
                            if (child.FirstCritical == null && IsCriticalAction(move.Action))
                                child.FirstCritical = move.Action;
                            int newMonsters, fromDeckOrExtra;
                            CountNewMonsters(node.State, move.State, out newMonsters, out fromDeckOrExtra);
                            child.Summoned = node.Summoned + newMonsters;
                            bool normalSummon = move.Action.Kind == PlanKind.NormalSummon || move.Action.Kind == PlanKind.ExtraNormalSummon;
                            if (newMonsters > 0 && !normalSummon
                                && (move.State.HasFlag(RdaState.FlagMaxxC) || (move.State.HasFlag(RdaState.FlagFuwalos) && fromDeckOrExtra > 0)))
                                child.DrawEvents++;
                            if (newMonsters > 0 && child.Summoned >= 5)
                            {
                                double risk = NibiruRisk(move.State);
                                child.NibiruPenalty += risk;
                                if (risk >= NibiruPenaltyWeight)
                                    child.NibiruExposed = true;
                            }
                            // Busca protegida contra o Nibiru: só segue linhas que chegam ao 5º monstro com resposta ou reconstrução.
                            if (options.ForbidNibiruExposed && child.NibiruExposed)
                                continue;
                            if (move.Action.Kind == PlanKind.Synchro)
                            {
                                if ((move.Action.CardId == RdaCards.Hypernova || move.Action.CardId == RdaCards.Supernova) && !HasProtection(node.State))
                                {
                                    if (options.ForbidUnprotectedNova)
                                        continue;
                                    child.NovaUnprotected = true;
                                }
                                int mask = 0;
                                for (int r = 0; r < RouteBook.Length; ++r)
                                {
                                    if ((node.RouteMask & (1 << r)) != 0 && node.SynchroCount < RouteBook[r].Length
                                        && RouteBook[r][node.SynchroCount] == move.Action.CardId)
                                        mask |= 1 << r;
                                }
                                child.RouteMask = mask;
                                child.SynchroCount = node.SynchroCount + 1;
                            }
                            // Mesmo estado por outro caminho: fica o que protegeu a nova e, depois, o que expôs menos monstros.
                            Node existing;
                            if (candidates.TryGetValue(move.State, out existing)
                                && KeepExisting(existing, child))
                                continue;
                            candidates[move.State] = child;
                        }
                        ticksNodes += phase.ElapsedTicks;
                    }
                    if (candidates.Count == 0)
                        break;
                    phase.Restart();

                    var scored = new List<KeyValuePair<double, Node>>(candidates.Count);
                    foreach (Node node in candidates.Values)
                    {
                        // Granularidade na avaliação: a cada 256 nós confere cancelamento/limite. Um feixe de 3000 estados
                        // leva ~meio segundo para avaliar; dividir por blocos impede que isso vire segundos de estouro.
                        if ((++timeCheckCount & 255) == 0
                            && (options.Token.IsCancellationRequested || watch.ElapsedMilliseconds > options.TimeLimitMs))
                            break;
                        double evaluation = RdaEvaluator.Evaluate(node.State);
                        if (node.State.Pending.Length == 0)
                        {
                            int tier = RdaEvaluator.Tier(node.State);
                            // Nova sem proteção prévia cai duas camadas: o combo é frágil e quebra inteiro se for interrompido,
                            // então a mesa ideal com Supernova protegida (camada 1) vence o Hypernova sem proteção.
                            if (node.NovaUnprotected)
                                tier = Math.Min(3, tier + 2);
                            if (NibiruTierBump && node.NibiruExposed)
                                tier = Math.Min(3, tier + 1);
                            if (maxxMode)
                                tier = 3;
                            if (tier < bestTier || (tier == bestTier && (maxxMode || evaluation >= bestNeutral[tier] - SelectionMargin)))
                            {
                                double selection = RdaEvaluator.SelectionScore(node.State, root.Hand)
                                    - (node.NovaUnprotected ? UnprotectedNovaPenalty : 0)
                                    - PathCost(node);
                                if (maxxMode)
                                {
                                    if (HasProtection(node.State))
                                        selection += MaxxProtectionBonus;
                                    if (Array.IndexOf(node.State.Deck, RdaCards.Lubellion) >= 0)
                                        selection += MaxxLubellionRefund;
                                    // Disciplina sob Maxx "C"/Fuwalos (jogador, 2026-09-15, partida perdida contra Labrynth: o bot
                                    // deu 2 cartas e mesmo assim seguiu o combo). Cada carta dada custa MaxxExtraDrawPenalty além
                                    // dos 35 do PathCost, então só uma mesa bem melhor justifica continuar invocando.
                                    selection -= MaxxExtraDrawPenalty * node.DrawEvents;
                                }
                                OfferCandidate(finalCandidates, node, tier, selection);
                                if (tier < bestTier || selection > bestScore)
                                {
                                    bestTier = tier;
                                    bestScore = selection;
                                    bestNode = node;
                                    bestFoundMs = watch.ElapsedMilliseconds;
                                    bestFoundDepth = depth;
                                }
                            }
                            if (evaluation > bestNeutral[tier])
                                bestNeutral[tier] = evaluation;
                        }
                        // Prioridade da busca: nota neutra + potencial + livro de rotas + ruído.
                        double priority = evaluation + RdaEvaluator.Potential(node.State);
                        if (options.BookWeight > 0 && node.RouteMask != 0)
                            priority += options.BookWeight * node.SynchroCount;
                        if (options.Noise > 0)
                            priority += random.NextDouble() * options.Noise;
                        if (seeds != null && node.FirstCritical != null && node.SynchroCount == SeedSynchroCount
                            && node.Parent != null && node.Parent.SynchroCount < SeedSynchroCount)
                        {
                            int openingKey = (int)node.FirstCritical.Effect * 64 + (node.RouteMask == 0 ? 0 : LowestBit(node.RouteMask) + 1);
                            KeyValuePair<double, Node> current;
                            if (!seeds.TryGetValue(openingKey, out current) || priority > current.Key)
                                seeds[openingKey] = new KeyValuePair<double, Node>(priority, node);
                        }
                        scored.Add(new KeyValuePair<double, Node>(priority, node));
                    }
                    ticksEvaluate += phase.ElapsedTicks;
                    phase.Restart();
                    scored.Sort((a, b) => b.Key.CompareTo(a.Key));

                    if (options.PerRoute > 0)
                    {
                        // Diversidade: primeiro garante até PerRoute estados de cada rota, depois completa pela prioridade.
                        var chosen = new List<Node>(options.Width);
                        var taken = new bool[scored.Count];
                        var counts = new Dictionary<int, int>();
                        int diversityLimit = Math.Max(1, (int)(options.Width * Math.Min(1.0, Math.Max(0.0, options.DiversityShare))));
                        for (int i = 0; i < scored.Count && chosen.Count < diversityLimit; ++i)
                        {
                            int signature = ExtraSignature(scored[i].Value.State);
                            if (options.OpeningDiversity)
                                signature = unchecked(signature * 486187739 + OpeningKey(scored[i].Value));
                            int count;
                            counts.TryGetValue(signature, out count);
                            if (count >= options.PerRoute)
                                continue;
                            counts[signature] = count + 1;
                            chosen.Add(scored[i].Value);
                            taken[i] = true;
                        }
                        for (int i = 0; i < scored.Count && chosen.Count < options.Width; ++i)
                        {
                            if (!taken[i])
                                chosen.Add(scored[i].Value);
                        }
                        frontier = chosen;
                    }
                    else
                    {
                        frontier = scored.Take(options.Width).Select(item => item.Value).ToList();
                    }
                    if (TraceStates != null)
                    {
                        var kept = new HashSet<Node>(frontier);
                        double cutoff = scored.Count > 0 ? scored[Math.Min(options.Width, scored.Count) - 1].Key : 0;
                        for (int i = 0; i < scored.Count; ++i)
                        {
                            if (!TraceStates.Contains(scored[i].Value.State))
                                continue;
                            Node traced = scored[i].Value;
                            lock (TraceLog)
                                TraceLog.Add(string.Format("{0}: depth {1} position {2}/{3} priority {4:0.#} (eval {5:0.#} pot {6:0.#} book {7}) cut {8:0.#} kept {9} | {10}",
                                    options.Name, depth, i, scored.Count, scored[i].Key, RdaEvaluator.Evaluate(traced.State), RdaEvaluator.Potential(traced.State),
                                    traced.RouteMask != 0 ? traced.SynchroCount : 0, cutoff, kept.Contains(traced), traced.Action != null ? traced.Action.Text : "-"));
                        }
                    }
                    ticksSort += phase.ElapsedTicks;
                    if (options.Token.IsCancellationRequested || watch.ElapsedMilliseconds > options.TimeLimitMs)
                        break;
                }

                RdaPlan plan = MakePlan(root, options, bestNode, bestTier, bestScore, depth);
                plan.BestFoundMs = bestFoundMs;
                plan.BestFoundDepth = bestFoundDepth;
                plan.SuccessorsMs = ticksSuccessors * 1000 / Stopwatch.Frequency;
                plan.NodesMs = ticksNodes * 1000 / Stopwatch.Frequency;
                plan.EvaluateMs = ticksEvaluate * 1000 / Stopwatch.Frequency;
                plan.SortMs = ticksSort * 1000 / Stopwatch.Frequency;
                plan.Expanded = expanded;
                plan.Generated = generated;
                foreach (Candidate candidate in finalCandidates)
                {
                    if (candidate.Node != bestNode && candidate.Node.Parent != null)
                        plan.Alternatives.Add(MakePlan(root, options, candidate.Node, candidate.Tier, candidate.Score, depth));
                }
                if (seeds != null)
                {
                    foreach (KeyValuePair<int, KeyValuePair<double, Node>> entry in seeds)
                    {
                        RdaPlan seedPlan = MakePlan(root, options, entry.Value.Value, 3, entry.Value.Key, depth);
                        int route = entry.Key % 64;
                        seedPlan.Search = entry.Value.Value.FirstCritical.Effect + (route > 0 ? "/route" + route : "");
                        plan.Seeds.Add(seedPlan);
                    }
                }
                plan.ElapsedMs = watch.ElapsedMilliseconds;
                return plan;
            }

            private sealed class Candidate
            {
                public Node Node;
                public int Tier;
                public double Score;
            }

            private const int CandidatesPerSearch = 3;

            private static void OfferCandidate(List<Candidate> list, Node node, int tier, double score)
            {
                // Um candidato por starter (1º efeito crítico): a disputa por resistência precisa de aberturas diferentes.
                string starter = node.FirstCritical != null ? node.FirstCritical.Text : "-";
                for (int i = 0; i < list.Count; ++i)
                {
                    string other = list[i].Node.FirstCritical != null ? list[i].Node.FirstCritical.Text : "-";
                    if (other != starter)
                        continue;
                    if (tier < list[i].Tier || (tier == list[i].Tier && score > list[i].Score))
                    {
                        list[i].Node = node;
                        list[i].Tier = tier;
                        list[i].Score = score;
                    }
                    return;
                }
                if (list.Count >= CandidatesPerSearch)
                {
                    Candidate worst = list[list.Count - 1];
                    if (tier > worst.Tier || (tier == worst.Tier && score <= worst.Score))
                        return;
                }
                list.Add(new Candidate { Node = node, Tier = tier, Score = score });
                list.Sort((a, b) => a.Tier != b.Tier ? a.Tier.CompareTo(b.Tier) : b.Score.CompareTo(a.Score));
                if (list.Count > CandidatesPerSearch)
                    list.RemoveAt(list.Count - 1);
            }

            private static RdaPlan MakePlan(RdaState root, SearchOptions options, Node best, int tier, double score, int depth)
            {
                var plan = new RdaPlan { Score = score, Tier = tier, Search = options.Name, Root = root, Final = best.State, Depth = depth, Exposure = best.Exposure,
                    DrawEvents = best.DrawEvents, NibiruPenalty = best.NibiruPenalty, NibiruExposed = best.NibiruExposed,
                    NovaUnprotected = best.NovaUnprotected };
                var chain = new List<Node>();
                for (Node node = best; node.Parent != null; node = node.Parent)
                    chain.Add(node);
                chain.Reverse();
                foreach (Node node in chain)
                    plan.Steps.Add(new PlanStep { Action = node.Action, After = node.State });
                return plan;
            }
        }
    }
}
