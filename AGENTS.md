# AGENTS.md

## 项目定位

- WindBot 是面向 YGOPro/YGOSharp/SRVPro 协议的 C# 决斗机器人，本质上是一个自动操作的 YGOPro 客户端。
- 主程序基于 .NET Framework 4.8，解决方案平台为 x86；项目是旧式非 SDK `.csproj`，不要默认使用仅适用于现代 .NET/SDK 项目的工具和 API。
- 机器人只能使用服务器发给当前客户端的信息。对方手牌、卡组、盖卡等未公开信息通常只有数量或 `Id == 0` 的占位对象，不能读取、推断或硬编码其真实内容。
- 服务器在要求客户端响应时已经给出了合法操作候选。牌组 AI 的职责主要是判断“现在是否值得这样做”以及“选哪个目标”，不要重复实现完整的规则引擎。

## 解决方案与运行链路

解决方案包含两个项目：

- `WindBot.csproj`：控制台主程序，包含网络协议、客户端状态和全部决斗 AI；输出 `WindBot.exe`。
- `BotWrapper/BotWrapper.csproj`：供 YGOPro 人机模式调用的轻量启动器，读取 `bot.conf` 并启动 `WindBot.exe`；输出 `Bot.exe`。

主程序的调用链如下：

1. `Program.cs` 读取命令行或配置文件，初始化牌组注册表和 `cards.cdb`，然后以单实例模式或 HTTP server 模式启动机器人。
2. `Game/GameClient.cs` 建立连接、进入房间并把收到的数据包交给 `GameBehavior`。
3. `Game/GameBehavior.cs` 按 `StocMessage`/`GameMessage` 解包，更新 `Duel`、`ClientField`、`ClientCard` 等客户端可见状态，并在需要响应时调用 `GameAI`。
4. `Game/GameAI.cs` 整理服务端给出的合法候选，按优先级查询当前牌组的 `Executor`，把最终选择编码后发回服务器。
5. `Game/AI/DecksManager.cs` 通过反射发现带 `[Deck]` 的执行器，并为每局实例化对应牌组 AI。
6. `Game/AI/Executor.cs` 定义公共回调和有序的 `CardExecutor` 列表；各牌组通常继承 `DefaultExecutor`。

server 模式会为每个 HTTP 请求创建独立线程和独立的 `GameClient`/`Duel`/`Executor`。不要把单局可变状态放进静态字段，也不要让不同机器人实例共享未同步的牌组状态。

## 目录职责

- `Game/`：决斗状态、消息处理、主阶段/战斗阶段动作模型。
- `Game/AI/`：AI 调度、选择队列、通用决策、卡片扩展方法和领域常量。
- `Game/AI/Decks/`：各牌组的专用执行器，是新增或调整牌组逻辑的首选位置。
- `Game/AI/Enums/`：跨牌组共享的已知卡分类，例如压制卡、危险怪兽、不可取对象等。
- `YGOSharp.Network/`：TCP 数据帧和 CTOS/STOC 协议层。
- `YGOSharp.OCGWrapper/`、`YGOSharp.OCGWrapper.Enums/`：卡片数据库模型和协议/规则枚举。
- `Decks/`：运行时 `.ydk` 牌组文件。
- `Dialogs/`：运行时 JSON 对话资源。
- `BotWrapper/`：外部 bot 启动包装器及其配置。

层次边界应保持清晰：

- 协议解析和客户端状态同步放在 `GameBehavior`/网络层。
- 通用、确实能被多个牌组复用的决策放在 `DefaultExecutor`、`AIUtil`、`CardExtension` 或共享枚举中。
- 单一牌组的展开路线、卡片优先级和临时标志留在对应牌组执行器中。
- 不要为了一个牌组的特例污染协议层或全局默认逻辑。

## AI 决策模型

### `AddExecutor` 顺序就是优先级

`GameAI` 会按注册顺序遍历 `Executor.Executors`，再遍历服务器给出的候选卡；第一个匹配且返回 `true` 的执行器立即胜出。因此：

- 构造函数中的 `AddExecutor` 顺序具有决定性影响，调整顺序属于行为修改，不是格式整理。
- 更具体、更紧急的响应放在前面，通用处理和兜底放在后面。
- 同一张卡的不同效果或不同局面可以注册多次；需要用 `Card`、`ActivateDescription`、`CurrentTiming` 和当前连锁状态区分。
- `AddExecutor(type, cardId)` 表示匹配后无条件接受；只应在所有合法出现时都适合执行的场景使用。
- `GoToBattlePhase`、`GoToEndPhase`、`Surrender` 等没有卡片上下文，其条件函数不能依赖 `Card`。

服务器给出的候选已经满足“能否发动/召唤”的基本规则，所以执行器条件应回答“是否适合发动/召唤”。只有在策略确实需要时，才检查会被无效、资源不足、后续路线冲突、每回合次数等因素。

执行器条件可能在一次响应过程中被多次查询。避免在返回 `false` 的路径上消耗资源、写入已发动标志或排入残留选择；只有确定接受动作时才提交与该动作绑定的选择和状态。

### 已知架构局限

- 机器人无法得知自己或对方发动某个效果后，游戏引擎和卡片脚本接下来会如何处理。它只能依据服务器当前已发送的状态，以及编写者对卡片效果和规则的知识做基本判断；这是本项目最主要的局限之一。不要把策略写成能够预演脚本、完整结算效果或读取未来状态。
- 卡片逻辑中的选卡是预先选择：执行器接受动作时通过 `AI.Select*` 排入选择，等处理效果的选择消息到达后再消费。目前预选项与具体效果、连锁环节之间没有稳定的一一对应关系，同一连锁包含多次发动或选择流程与预期不一致时可能消费错位；这是另一项主要局限。
- 预先选择的顺序必须与卡片脚本实际发出选择请求的顺序一致。卡片脚本可参考 `https://raw.githubusercontent.com/Fluorohydride/ygopro-scripts/refs/heads/master/c{id}.lua`，其中 `{id}` 替换为卡片密码；较复杂的效果不能仅凭效果文本猜测选目标、选cost、选素材等请求的先后顺序。

### 上下文、状态与选择

- `Executor.SetCard` 会在查询前设置当前 `Type`、`Card`、`ActivateDescription`、`CurrentTiming`。
- `Bot` 和 `Enemy` 分别是本机视角的 `Duel.Fields[0]` 与 `Duel.Fields[1]`；协议玩家编号应通过现有本地化逻辑转换，不要自行假定座位编号。
- 优先使用 `ClientField`、`ClientCard`、`AIUtil`、`CardExtension` 的现有查询方法，避免重复遍历和散落的区域位掩码。
- 未知卡的 `Id` 可能为 `0`，`Data`/`Name` 可能为 `null`。对隐藏区域只能依赖客户端实际知道的数量、位置和已公开历史。
- `Bot.Deck` 只表示客户端可见的牌堆槽位，不是可按卡号查询的剩余卡组：决斗开始时其中的卡通常为 `Id == 0`，洗牌后也会被重置为 `Id == 0`。因此禁止用 `Bot.Deck.Any(card => card.IsCode(...))` 或等价写法判断某卡是否仍在卡组。检索、送墓等效果应排入卡号优先级，再由服务器提供的实际候选集过滤并决定选择。
- 牌组执行器的回合、阶段、连锁和使用次数标志应在 `OnNewTurn`、`OnNewPhase`、`OnChainEnd`、`OnMove` 等正确生命周期回调中维护和重置。
- `Duel.CurrentChain`、`CurrentChainInfo`、`ChainTargets`、`LastSummonedCards` 等状态由消息流维护；使用前注意它表示当前客户端已收到的时点，而不是完整规则模拟。

动作和后续选卡通常分两步发生：

- 在接受动作前用 `AI.SelectCard`、`AI.SelectNextCard`、`AI.SelectThirdCard`、`AI.SelectMaterials`、`AI.SelectPlace`、`AI.SelectPosition`、`AI.SelectOption` 等预设后续响应。
- `SelectCard` 是第一个选择；必须先调用它，再调用 `SelectNextCard`/`SelectThirdCard`。
- 选择器会在后续服务端选择消息到达时消费，并可能跨越多个连续选择。不要排入与实际效果流程不一致的额外选择。
- 复杂的、依赖 `hint`/`min`/`max`/候选集合的选择应覆盖 `OnSelectCard` 或素材选择回调；无法处理时返回 `base`/`null`，让通用逻辑继续。
- 返回的卡片数量必须满足 `min`/`max`，并且对象必须来自服务器传入的候选集合。

必发效果的发动和选卡需要特别处理：

- 必发效果可能由服务器直接强制发动，尤其是只有一个强制候选时，牌组注册的 `Activate` 执行器及其条件函数不会被调用。因此不要依赖发动条件函数为必发效果排入 `AI.Select*`、设置状态或完成其他副作用；通常只需让服务器发动，并在实际选择回调中处理选卡。
- 也就是说，`AddExecutor(ExecutorType.Activate, CardId.Sangan, SanganActivate);` 中在 `SanganActivate` 调用 `AI.SelectCard` 等方法基本是无意义的；必发效果的 `ExecutorType.Activate` 的意义应仅限于多个同时发动候选的优先级。
- 卡片发动时选择支付代价或选择指定目标，发生在连锁建立阶段。此时该卡已经加入 `Duel.CurrentChain`，但尚未进入连锁处理，应在 `OnSelectCard` 中用 `Duel.GetCurrentChainCard()` 识别最新连锁卡；同时检查控制者、卡号和 `hint`，再从服务器给出的候选中返回对象。
- 效果处理时才进行的选卡，例如从卡组检索、特殊召唤或效果处理中的丢弃，发生在连锁处理阶段，应在 `OnSelectCard` 中用 `Duel.GetCurrentSolvingChainCard()` 识别正在处理的连锁卡。该方法在发动、支付代价和指定目标时会返回 `null`。
- 注意，以上问题仅限于必发效果，即满足条件必定强制发动的 `EFFECT_TYPE_TRIGGER_F` 的效果。效果文本中写“〇〇的场合才能发动”通常不是必发效果，写“〇〇的场合发动”通常是必发效果。普通可选发动的效果应正常使用 `AI.SelectCard` 等方法。
- `GetCurrentChainCard()` 只表示尚未开始处理时的最新连锁卡，连锁开始处理后返回 `null`；`GetCurrentSolvingChainCard()` 只表示当前正在处理的连锁卡。不要用 `CurrentChain.LastOrDefault()` 或 `AIUtil.GetLastChainCard()` 代替这一区分，否则多段连锁倒序处理时可能把选卡归给错误的连锁卡。
- 如果同一张卡同时具有发动时目标、处理时选卡或多个不同效果，必须结合 `hint`、候选区域和必要的效果描述进一步区分。不要让 `OnSelectCard` 返回选择的同时还保留同一流程的预选队列，否则残留选择可能污染下一次选卡。

## 新增或修改牌组

新增牌组时通常需要同时完成：

1. 在 `Game/AI/Decks/` 添加继承 `DefaultExecutor` 的执行器。
2. 添加唯一的 `[Deck("外部名称", "AI_牌组文件名", "级别")]`。外部名称用于 `Deck=...`；文件名对应 `Decks/<文件名>.ydk`，不带扩展名。
3. 在 `Decks/` 添加匹配的 `.ydk`。`DeckFile` 配置可以覆盖特性声明的默认文件。
4. 从高到低注册 `AddExecutor`，并实现必要的目标、素材、选项、位置以及生命周期回调。
5. 只有在需要向 BotWrapper 暴露该牌组或新增对话时，才同步修改 `BotWrapper/bot.conf`、`Dialogs/` 或用户文档。

注意：

- `[Deck]` 名称重复会在 `DecksManager.Init()` 初始化字典时失败。
- 未指定牌组时，随机选择只接受 `Level == "Normal"` 的执行器；`Easy`、`NotFinished`、`Test` 不会进入普通随机池。
- `Game/AI/Decks/*.cs`、`Game/AI/Enums/*.cs`、`Decks/*.ydk` 和 `Dialogs/*.json` 已由项目文件通配包含。
- 旧式项目不会自动包含其他目录中的新 `.cs` 文件；在核心目录新增源码时必须同步检查 `WindBot.csproj` 的 `<Compile Include=...>`。
- 卡片静态数据来自运行时 `cards.cdb`。不要仅为某张卡复制数据库字段到代码；卡号常量和确有必要的策略分类除外。

修改已有牌组时，先阅读它的构造函数注册顺序、相关覆盖回调和邻近的同类牌组实现。尽量做局部修改，避免顺手重排整个执行器列表或大范围统一旧代码风格。

## 协议和核心状态修改

- `GameBehavior` 的读取顺序、整数宽度、控制者转换和响应格式必须与协议完全一致；不要在没有协议依据时增删字段。
- 新增 `GameMessage`/`StocMessage` 处理时，同时检查枚举、消息注册、状态更新和响应路径。
- 状态应先在 `Duel`/`ClientField`/`ClientCard` 中正确落地，再供 AI 使用；不要让牌组执行器直接解析网络包。
- 区域、位置、卡片类型和查询标志使用 `YGOSharp.OCGWrapper.Enums` 中的枚举。位标志判断保留按位语义，不要改成普通相等判断。
- 公共行为修改会影响全部牌组。修改 `DefaultExecutor`、`AIUtil`、`GameAI` 或选择器时，要快速检查多个调用点以及兜底行为。

## 构建与验证

- CI 在 Windows 上使用 MSBuild 构建 `WindBot.sln` 的 Release 配置；本地对应命令为：

  ```powershell
  msbuild WindBot.sln /t:Build /p:Configuration=Release /p:Platform=x86
  ```

- 这是旧式 .NET Framework 解决方案，优先使用 Visual Studio/MSBuild；不要把 `dotnet build` 当作默认验证方式。
- 仓库目前没有自动化测试项目。不要声称“测试通过”来代替实际构建或对局验证。
- 文档、对话或单纯 `.ydk` 修改通常无需编译。修改 C# 时按影响范围决定是否构建。
- 协议、状态同步、选择队列和公共 AI 修改应至少做 Release x86 构建。
- 运行 `WindBot.exe` 需要可访问的 `cards.cdb`；项目输出还依赖随仓库提供的 SQLite 组件以及复制到输出目录的 `Decks`/`Dialogs` 资源。
- 调试行为问题时可使用 `Debug=True` 查看移动日志，并记录触发消息、当前阶段、连锁、候选列表及最终命中的执行器。

## 代码与协作约定

- 保持与现有代码一致的 C# 风格：4 空格缩进、Allman 大括号、清晰的显式控制流；不要为无关文件做批量格式化。
- 所有代码文件使用 UTF-8 和 CRLF。新文件建议无 BOM；已有 BOM 不做无关调整。
- 避免引入 .NET Framework 4.8 或当前编译方式不支持的 API/语法。
- 不要提取只在单个函数中调用一次、且没有明显复用价值的小函数。
- 注释可以复述单个函数整体行为。
- 保留用户已有的未提交和暂存更改。除非用户明确要求，不要暂存、提交、切换分支或改动暂存区。
- 除非用户明确要求，不要新增独立文档文件；必要说明优先更新现有文档。
- 审阅更改时，除列出问题外，还应简述改动内容，评价充分性、必要性、优缺点和明显优化点，并快速搜索项目内是否存在同类问题。

## 本地 AGENTS.md

- 本地可能存在 `AGENTS.local.md` 或 `AGENTS.user.md`，并被 `.gitignore` 或 `.git/info/exclude` 忽略。
- 它们可以包含本地开发者的特殊要求、ygopro 和 ygopro-scripts 的本地路径等。
- 这些文件不会被提交到仓库，但应与此 AGENTS.md 文件同时读取。
