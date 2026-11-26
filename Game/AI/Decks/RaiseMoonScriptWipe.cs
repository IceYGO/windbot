using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace WindBot.Game.AI.Decks
{

// 「不取对象的群体清场」检测：直接读这张卡的 lua 脚本，判断它会不会一次把整个区域拆掉。
//
// 为什么必须自己做这件事：
//   取对象的效果（旋风 / 强脱 / 次元幽闭）在以我们为目标时，服务端会发 MSG_BECOME_TARGET，
//   进 Duel.ChainTargets，用 Util.IsChainTarget 就能提前知道；
//   但「破坏场上所有卡」这类**不取对象**的清场根本不发这个包 —— ChainTargets 是空的，
//   卡被拆掉之前一点信号都没有。只能反过来问：「现在连锁上的这张卡，会不会清场？」
//
// 判据（逐函数体，只看真正喂给 Duel.Destroy / Remove / Sendto* 的那个组）：
//   1. 函数体里没有 Duel.SelectTarget —— 有就是取对象，另有路径覆盖，不算清场
//   2. Duel.Destroy / Remove / SendtoGrave / SendtoDeck / SendtoHand 的实参是个组，
//      且该组来自 local g = Duel.GetFieldGroup / GetMatchingGroup(...)
//   3. 区域：LOCATION_ONFIELD / LOCATION_ALL → 怪兽+魔陷；LOCATION_MZONE → 怪兽；LOCATION_SZONE → 魔陷
//      若组带过滤函数（例：羽毛扫的 filter 限定 TYPE_SPELL+TYPE_TRAP），以过滤函数为准
//   4. 落点：组取函数的 (自己, 对手) 两个区域参数里，对手那一格是 0 → 只拆自己的场，
//      威胁不到我们，直接不算（这条砍掉约 1/4 的「自爆型」误报）
//
// 实测分类（样本见施工单）：
//   黑洞 / 雷击 / 激流葬 / 圣防 → 怪兽
//   大岚 / 羽毛扫             → 魔陷
//   天霆号 阿宙斯              → 全场
//   旋风 / 强脱 / 次元幽闭 / 双龙卷 / 沙尘 → 0（取对象，走 ChainTargets）
public static class RaiseMoonScriptWipe
{
	public const int None = 0;
	public const int Monster = 1;     // 会拆怪兽区
	public const int SpellTrap = 2;   // 会拆魔陷区

	// 打包：低 2 位 = 拆「发动者自己的场」，高 2 位 = 拆「发动者的对手的场」
	private const int ShiftOpp = 2;

	private static readonly Dictionary<int, int> Cache = new Dictionary<int, int>();
	private static string _scriptDir;
	private static bool _dirResolved;

	private static readonly Regex AssignRx = new Regex(
		@"local\s+(\w+)\s*=\s*(Duel\.Get\w+\((?:[^()]|\([^()]*\))*\))",
		RegexOptions.Singleline | RegexOptions.Compiled);

	// 会让卡「离开场上」的操作：破坏 / 除外 / 送墓 / 回卡组 / 回手牌
	private static readonly Regex LeaveRx = new Regex(
		@"Duel\.(?:Destroy|Remove|SendtoGrave|SendtoDeck|SendtoHand)\s*\(\s*([\w:\.]+)",
		RegexOptions.Singleline | RegexOptions.Compiled);

	private static readonly Regex FirstArgRx = new Regex(
		@"Duel\.Get\w+\(\s*([\w\.]+)\s*,", RegexOptions.Compiled);

	private static readonly Regex CallRx = new Regex(
		@"^Duel\.Get\w+\((.*)\)$", RegexOptions.Singleline | RegexOptions.Compiled);

	private static readonly Regex FuncRx = new Regex(
		@"function\s+([\w\.]+)\s*\([^)]*\)(.*?)\nend", RegexOptions.Singleline | RegexOptions.Compiled);

	// 「不会被效果破坏」类效果的注册码。全部是**自身**破坏抗性，跟幽鬼兔要对的那一下直接相关：
	//   EFFECT_INDESTRUCTABLE_EFFECT  —— 不会被效果破坏
	//   EFFECT_INDESTRUCTABLE_COUNT   —— 不会被破坏 N 次
	//   EFFECT_INDESTRUCTABLE         —— 旧式（不带后缀）= 战斗 + 效果都免
	//   EFFECT_DESTROY_SUBSTITUTE     —— 可以用别的卡代替破坏
	//   EFFECT_DESTROY_REPLACE        —— 代替破坏（黑智天至那种「拔一个素材代替」）
	// ★ 明确排除 EFFECT_INDESTRUCTABLE_BATTLE（只免战斗破坏，效果破坏照样吃，不影响幽鬼兔）。
	//   `(?![_\w])` 这条负向前瞻就是干这个的：`_BATTLE` 里 `EFFECT_INDESTRUCTABLE` 后面紧跟下划线，匹配失败。
	private static readonly Regex DestroyImmuneRx = new Regex(
		@"EFFECT_INDESTRUCTABLE_EFFECT|EFFECT_INDESTRUCTABLE_COUNT|EFFECT_INDESTRUCTABLE(?![_\w])|EFFECT_DESTROY_SUBSTITUTE|EFFECT_DESTROY_REPLACE",
		RegexOptions.Compiled);

	private static readonly Dictionary<int, bool> ImmuneCache = new Dictionary<int, bool>();

	// 这张卡是不是「不会被效果破坏」—— 幽鬼兔的效果是「把发动效果的那张卡破坏」，
	// 砸在这种卡上等于白丢一张手牌（对方效果照常处理，卡还在场上），所以先问一句再决定发不发。
	public static bool CannotBeDestroyedByEffect(int cardId)
	{
		bool cached;
		if (ImmuneCache.TryGetValue(cardId, out cached)) return cached;

		bool res = false;
		try
		{
			string path = ScriptPath(cardId);
			if (path != null) res = DestroyImmuneRx.IsMatch(File.ReadAllText(path));
		}
		catch
		{
			res = false;   // 读不到就当「可以破坏」，保持原来的行为
		}

		ImmuneCache[cardId] = res;
		return res;
	}

	public static string ResolvedDir
	{
		get
		{
			ScriptPath(0);
			return _scriptDir;
		}
	}

	// ===== 「会不会被战斗破坏」=====
	// 重铠光子龙 ② 要拔 1 素材炸一张表侧卡 —— 炸**场地 / 永续**是纯赚，但炸「战破抗性怪」要看场合：
	//   ★ v5.27 用户口径：对方场上有这种怪时，宁可先降 500 打点（5300 → 4800）也要抢在战阶前炸掉它，
	//     否则战阶啃不动、还会被它继续干扰。
	// 判据只认「免战斗破坏」这一类：
	//   EFFECT_INDESTRUCTABLE_BATTLE   —— 不会被战斗破坏
	//   EFFECT_INDESTRUCTABLE(?![_\w]) —— 旧式（战斗 + 效果都免）
	//   EFFECT_INDESTRUCTABLE_COUNT    —— 不会被破坏 N 次（含战斗）
	//   EFFECT_BATTLE_DESTROY_REDIRECT —— 把战斗破坏转移给别人
	// ★ 与 `DestroyImmuneRx` 那一组恰好互补（那边明确排除 `_BATTLE`），别混用：
	//   幽鬼兔问的是「效果破坏能不能成」，重铠问的是「战阶能不能打死」。
	private static readonly Regex BattleImmuneRx = new Regex(
		@"EFFECT_INDESTRUCTABLE_BATTLE|EFFECT_INDESTRUCTABLE(?![_\w])|EFFECT_INDESTRUCTABLE_COUNT|EFFECT_BATTLE_DESTROY_REDIRECT",
		RegexOptions.Compiled);

	private static readonly Dictionary<int, bool> BattleImmuneCache = new Dictionary<int, bool>();

	public static bool CannotBeDestroyedByBattle(int cardId)
	{
		bool cached;
		if (BattleImmuneCache.TryGetValue(cardId, out cached)) return cached;

		// 读不到脚本 → 当「能战破」，保持原来的行为（不因为 DIY 卡缺脚本就乱抢节奏）。
		bool res = false;
		try
		{
			string path = ScriptPath(cardId);
			if (path != null) res = BattleImmuneRx.IsMatch(File.ReadAllText(path));
		}
		catch
		{
			res = false;
		}

		BattleImmuneCache[cardId] = res;
		return res;
	}

	// ===== 「会不会从卡组把卡加入手卡」=====
	// 朔·斯奎兹的「持续抽卡」（op=1）注册的是 EVENT_TO_HAND + 过滤「对方的卡从卡组加入手卡且不是抽卡」，
	// 只有对方真的**检索**才会触发。对方从卡组**堆墓**（愚蠢的副葬、各类送墓）根本不会触发，
	// 提前把它当 cost 丢掉就是白扔一张手牌。
	// AI 拿不到对方效果的 category，只能读它的 lua 脚本找痕迹：
	//   CATEGORY_SEARCH          —— 声明了「检索」类别
	//   HINTMSG_ATOHAND          —— 选择时要「加入手卡」
	//   Card.IsAbleToHand        —— 过滤条件里筛「能加入手卡」的卡（壶 / 检索魔法常这么写）
	//   SendtoHand + LOCATION_DECK —— 直接往手卡送，且脚本里动到卡组
	private static readonly Regex SearchRx = new Regex(
		@"CATEGORY_SEARCH|HINTMSG_ATOHAND|Card\.IsAbleToHand",
		RegexOptions.Compiled);

	private static readonly Dictionary<int, bool> SearchCache = new Dictionary<int, bool>();

	public static bool TakesFromDeckToHand(int cardId)
	{
		bool cached;
		if (SearchCache.TryGetValue(cardId, out cached)) return cached;

		// ★ 读不到脚本 → 按「可能是检索」处理，等于保持改动前的宽松行为。
		//   宁可多挂一次（多抽一张），也不要因为读不到 DIY 卡脚本就漏掉对方的检索。
		bool res = true;
		try
		{
			string path = ScriptPath(cardId);
			if (path != null)
			{
				string t = File.ReadAllText(path);
				res = SearchRx.IsMatch(t)
					|| (t.IndexOf("SendtoHand", StringComparison.Ordinal) >= 0
						&& t.IndexOf("LOCATION_DECK", StringComparison.Ordinal) >= 0);
			}
		}
		catch
		{
			res = true;
		}

		SearchCache[cardId] = res;
		return res;
	}

	// ===== 「能不能无效『效果的发动』」=====
	// ★ v6.36 用户口径（replay「2026-10-30 02-54-42」）：
	//   「当对方场上有能无效怪兽效果发动（包括能无效卡的效果的发动的）的卡时，
	//     陨石不要发动解放效果，会被白白破坏掉。」
	//   陨石（27204311）是**手牌发动的怪兽效果**，会被这类卡直接无效并破坏/送去墓地：
	//     · 神之通告（40605147）、朱光 / 绿光之宣告者（17266660 / 21074344）……
	//   共同特征是 `Duel.NegateActivation(ev)`（无效**发动**，不是无效效果）。
	//   `EFFECT_NEGATE_ACTIVATE` 是同一件事的另一种写法，一起认。
	//   不认 `EFFECT_NEGATE_EFFECT`（无限泡影那种只无效效果、不停发动，而且只能指场上怪，
	//   陨石在手牌发动它够不到）。
	private static readonly Regex NegateActivationRx = new Regex(
		@"Duel\.NegateActivation\s*\(|EFFECT_NEGATE_ACTIVATE", RegexOptions.Compiled);

	private static readonly Dictionary<int, bool> NegateActivateCache = new Dictionary<int, bool>();

	public static bool NegatesActivation(int cardId)
	{
		bool cached;
		if (NegateActivateCache.TryGetValue(cardId, out cached)) return cached;

		// 读不到脚本 → 按「不会无效」处理。这条判据是**压制我方动作**的闸门，
		// 读不到就放行比读不到就拦住更安全（后者会让陨石永远不敢发）。
		bool res = false;
		try
		{
			string path = ScriptPath(cardId);
			if (path != null)
				res = NegateActivationRx.IsMatch(File.ReadAllText(path));
		}
		catch
		{
			res = false;
		}

		NegateActivateCache[cardId] = res;
		return res;
	}

	// ===== 「会不会从手牌特殊召唤」=====
	// 辉煌 ① 拉千金大奖的前提之一（v5.76）：对方这一手是从手牌特招怪兽。
	// 千金 ②（c9484285 的 e3 / `s.tdcon2`）判的是
	//   `eg:IsExists(Card.IsSummonLocation,1,nil,LOCATION_HAND)`
	// —— **被特招的那只怪**来自手牌，不分敌我，所以对方特招落地那一刻千金 ② 就能拔素材弹掉它。
	// AI 看不到对方的 category，只能读 lua 找痕迹：
	//   · 脚本里出现 `Duel.SpecialSummon(`（确实会特招）；
	//   · 并且某个「选卡 / 取卡」调用把 LOCATION_HAND 当成了候选来源，
	//     或者 filter 里直接写 `IsLocation(LOCATION_HAND)` / `IsPreviousLocation(LOCATION_HAND)`。
	// 只筛手牌用于丢弃的卡（灰流丽、增殖的 G、幽鬼兔…）没有 Duel.SpecialSummon，不会命中。
	private static readonly Regex SpecialSummonRx = new Regex(
		@"Duel\.SpecialSummon\s*\(", RegexOptions.Compiled);

	private static readonly Regex SelectCallRx = new Regex(
		@"Duel\.(?:SelectMatchingCard|SelectCard|SelectTarget|SelectWithSummon|GetMatchingCard|IsExistingMatchingCard)\s*\(",
		RegexOptions.Compiled);

	private static readonly Regex HandLocationRx = new Regex(
		@"Is(?:Previous)?Location\s*\(\s*LOCATION_HAND", RegexOptions.Compiled);

	private static readonly Dictionary<int, bool> HandSummonCache = new Dictionary<int, bool>();

	public static bool SpecialSummonsFromHand(int cardId)
	{
		bool cached;
		if (HandSummonCache.TryGetValue(cardId, out cached)) return cached;

		// 读不到脚本 → 当「不会手牌特招」：宁可不发，也不要为了一个猜不准的时机把
		// 本回合唯一的一张辉煌浪费在「拉一只没事干的千金」上。
		bool res = false;
		try
		{
			string path = ScriptPath(cardId);
			if (path != null)
			{
				string t = File.ReadAllText(path);
				if (SpecialSummonRx.IsMatch(t))
					res = HandLocationRx.IsMatch(t) || SelectArgsMentionHand(t);
			}
		}
		catch
		{
			res = false;
		}

		HandSummonCache[cardId] = res;
		return res;
	}

	// 把每个 `Duel.SelectXxx(` 的括号内容取出来，看位置参数里有没有 LOCATION_HAND
	private static bool SelectArgsMentionHand(string text)
	{
		foreach (Match m in SelectCallRx.Matches(text))
		{
			int i = m.Index + m.Length;      // 已经越过 '('
			int depth = 1;
			int start = i;
			while (i < text.Length && depth > 0)
			{
				char ch = text[i];
				if (ch == '(') depth++;
				else if (ch == ')') depth--;
				if (depth == 0) break;
				i++;
			}
			if (i <= text.Length && i > start)
			{
				string args = text.Substring(start, i - start);
				if (args.IndexOf("LOCATION_HAND", StringComparison.Ordinal) >= 0) return true;
			}
		}
		return false;
	}

	// 这张卡是不是「吃墓地」的效果 —— 屋敷童（73642296）的打击面：
	//   ● 从墓地把卡加入手卡 / 卡组 / 额外卡组  → SendtoHand / SendtoDeck / SendtoExtraP
	//   ● 从墓地把怪兽特殊召唤                  → SpecialSummon
	//   ● 从墓地把卡除外                        → Remove
	// 判据 =「脚本里既提到 LOCATION_GRAVE，又有上面任一操作」。
	// ★ 这是**整张卡**粒度的粗筛（AI 看不到对方这一手具体发动的是哪个效果），
	//   所以宁可用「这张卡有没有墓地互动」当门槛 —— 没有墓地互动的卡（检索 / 手坑 / 纯打点）
	//   一律不吃屋敷童，只有 1 张的屋敷童才不会被随手浪费。
	// ★ 读不到脚本 → 返回 false（不发）。宁可少交，也不要把唯一的屋敷童丢在一张猜不准的卡上。
	private static readonly Regex GraveOpRx = new Regex(
		@"Duel\.(?:SendtoHand|SendtoDeck|SendtoExtraP|SpecialSummon|Remove)\s*\(",
		RegexOptions.Compiled);

	private static readonly Dictionary<int, bool> GraveCache = new Dictionary<int, bool>();

	public static bool GraveRelated(int cardId)
	{
		bool cached;
		if (GraveCache.TryGetValue(cardId, out cached)) return cached;

		bool res = false;
		try
		{
			string path = ScriptPath(cardId);
			if (path != null)
			{
				string t = File.ReadAllText(path);
				res = t.IndexOf("LOCATION_GRAVE", StringComparison.Ordinal) >= 0 && GraveOpRx.IsMatch(t);
			}
		}
		catch
		{
			res = false;
		}

		GraveCache[cardId] = res;
		return res;
	}

	// 这张卡（由 casterIsEnemy 一方发动）的效果，会不会把 want 指定的区域整个拆掉
	public static bool Wipes(int cardId, int want, bool casterIsEnemy)
	{
		int packed = PackedFlags(cardId);
		int mine = casterIsEnemy ? (packed >> ShiftOpp) : packed;
		return (mine & want) != 0;
	}

	// 调试用：返回「拆我方」的原始区域标记（0–3）
	public static int FlagsAgainstMe(int cardId, bool casterIsEnemy)
	{
		int packed = PackedFlags(cardId);
		return (casterIsEnemy ? (packed >> ShiftOpp) : packed) & 3;
	}

	private static int PackedFlags(int cardId)
	{
		int cached;
		if (Cache.TryGetValue(cardId, out cached)) return cached;

		int flags = None;
		try
		{
			string path = ScriptPath(cardId);
			if (path != null) flags = Classify(File.ReadAllText(path));
		}
		catch
		{
			flags = None;   // 读不到就当作「不是清场」，保持原有保守行为
		}

		Cache[cardId] = flags;
		return flags;
	}

	// ★ v6.26：这张卡（任意一个效果）会不会「把怪兽特殊召唤」。
	//   只给「自己回合能不能跟增殖的G」做粗筛用 —— 用户口径是「自己回合只有对方发动特招效果才连锁」。
	//   整卡粒度：分不出这一环具体是哪个效果，同一张卡既有检索又有特招时会误判（已知短板）。
	private static readonly Dictionary<int, bool> AnySsCache = new Dictionary<int, bool>();

	public static bool HasSpecialSummonEffect(int cardId)
	{
		bool cached;
		if (AnySsCache.TryGetValue(cardId, out cached)) return cached;

		bool res = false;
		try
		{
			string path = ScriptPath(cardId);
			if (path != null) res = SpecialSummonRx.IsMatch(File.ReadAllText(path));
		}
		catch
		{
			res = false;
		}

		AnySsCache[cardId] = res;
		return res;
	}

	private static string ScriptPath(int cardId)
	{
		if (!_dirResolved)
		{
			_dirResolved = true;

			// 允许用环境变量直接指定（Bot 从别处启动 / 多份 ygopro 并存时用）
			string env = Environment.GetEnvironmentVariable("SHENGYUE_SCRIPT_DIR");
			if (!string.IsNullOrEmpty(env) && Directory.Exists(env))
			{
				_scriptDir = env;
			}
			else
			{
				string b = AppDomain.CurrentDomain.BaseDirectory;
				string[] candidates =
				{
					Path.Combine(b, "script"),
					Path.Combine(Path.GetFullPath(Path.Combine(b, "..")), "script"),
					Path.Combine(Path.GetFullPath(Path.Combine(b, "..", "..")), "script")
				};
				foreach (string c in candidates)
				{
					try
					{
						if (Directory.Exists(c))
						{
							_scriptDir = c;
							break;
						}
					}
					catch
					{
						// 路径非法就试下一个
					}
				}
			}
		}

		if (_scriptDir == null) return null;
		string p = Path.Combine(_scriptDir, "c" + cardId + ".lua");
		return File.Exists(p) ? p : null;
	}

	private static int Classify(string raw)
	{
		string text = raw.Replace("\r\n", "\n").Replace('\r', '\n');
		int flags = None;
		foreach (string body in Blocks(text))
			flags |= ClassifyBlock(body, text);
		return flags;
	}

	// 按 function 边界切块：一个块 = 一个效果函数
	private static IEnumerable<string> Blocks(string text)
	{
		List<string> cur = new List<string>();
		bool started = false;
		foreach (string raw in text.Split('\n'))
		{
			string s = raw.Trim();
			if (s.StartsWith("function ", StringComparison.Ordinal)
				|| s.StartsWith("local function ", StringComparison.Ordinal))
			{
				if (started) yield return string.Join("\n", cur);
				cur = new List<string>();
				started = true;
			}
			cur.Add(s);
		}
		if (started) yield return string.Join("\n", cur);
	}

	private static int ClassifyBlock(string body, string whole)
	{
		if (body.IndexOf("Duel.SelectTarget", StringComparison.Ordinal) >= 0) return None;
		if (!LeaveRx.IsMatch(body)) return None;

		Dictionary<string, string> vars = new Dictionary<string, string>();
		foreach (Match m in AssignRx.Matches(body))
			vars[m.Groups[1].Value] = m.Groups[2].Value;

		int flags = None;
		foreach (Match m in LeaveRx.Matches(body))
		{
			string arg = m.Groups[1].Value;
			string expr;
			if (!vars.TryGetValue(arg, out expr))
			{
				if (!arg.StartsWith("Duel.Get", StringComparison.Ordinal)) continue;
				expr = arg;   // 内联写法：Duel.Destroy(Duel.GetFieldGroup(...), ...)
			}

			int z = ZoneFlags(expr);
			if (z == None) continue;                       // 组里没有区域 → 不是场上清场

			int byFilter = FilterFlags(expr, whole);
			if (byFilter != None) z = byFilter;

			switch (SideOf(expr))
			{
				case 0: flags |= z; break;                             // 只拆发动者自己 → 低 2 位
				case 1: flags |= z << ShiftOpp; break;                  // 只拆对手 → 高 2 位
				default: flags |= z | (z << ShiftOpp); break;           // 两边都拆
			}
		}
		return flags;
	}

	private static int ZoneFlags(string expr)
	{
		bool onfield = expr.IndexOf("LOCATION_ONFIELD", StringComparison.Ordinal) >= 0
			|| expr.IndexOf("LOCATION_ALL", StringComparison.Ordinal) >= 0;
		int f = None;
		if (onfield
			|| expr.IndexOf("LOCATION_MZONE", StringComparison.Ordinal) >= 0
			|| expr.IndexOf("LOCATION_MONSTERZONE", StringComparison.Ordinal) >= 0)
			f |= Monster;
		if (onfield || expr.IndexOf("LOCATION_SZONE", StringComparison.Ordinal) >= 0)
			f |= SpellTrap;
		return f;
	}

	// 组取函数的 (自己, 对手) 两个区域参数：0 = 只拆自己，1 = 只拆对手，2 = 两边/说不清
	private static int SideOf(string expr)
	{
		string[] a = ArgsOf(expr);
		if (a == null) return 2;

		string self, opp;
		if (expr.StartsWith("Duel.GetFieldGroup", StringComparison.Ordinal))
		{
			if (a.Length < 3) return 2;
			self = a[1];
			opp = a[2];
		}
		else
		{
			if (a.Length < 4) return 2;
			self = a[2];
			opp = a[3];
		}

		bool sZero = self == "0";
		bool oZero = opp == "0";
		if (oZero && !sZero) return 0;
		if (sZero && !oZero) return 1;
		return 2;
	}

	private static string[] ArgsOf(string expr)
	{
		Match m = CallRx.Match(expr);
		if (!m.Success) return null;

		string body = m.Groups[1].Value;
		List<string> list = new List<string>();
		int depth = 0;
		StringBuilder sb = new StringBuilder();
		foreach (char ch in body)
		{
			if (ch == '(') depth++;
			else if (ch == ')') depth--;
			if (ch == ',' && depth == 0)
			{
				list.Add(sb.ToString().Trim());
				sb.Length = 0;
			}
			else
			{
				sb.Append(ch);
			}
		}
		list.Add(sb.ToString().Trim());
		return list.ToArray();
	}

	// 组带过滤函数时以过滤函数为准：羽毛扫的 c18144506.filter 只放 TYPE_SPELL+TYPE_TRAP，
	// 虽然区域写的是 LOCATION_ONFIELD，它其实只拆魔陷。
	private static int FilterFlags(string expr, string whole)
	{
		Match m = FirstArgRx.Match(expr);
		if (!m.Success) return None;
		string fn = m.Groups[1].Value;
		if (!fn.StartsWith("c", StringComparison.Ordinal)) return None;

		foreach (Match fm in FuncRx.Matches(whole))
		{
			if (fm.Groups[1].Value != fn) continue;
			string b = fm.Groups[2].Value;
			bool sp = b.IndexOf("TYPE_SPELL", StringComparison.Ordinal) >= 0
				|| b.IndexOf("TYPE_TRAP", StringComparison.Ordinal) >= 0;
			bool mo = b.IndexOf("TYPE_MONSTER", StringComparison.Ordinal) >= 0;
			if (sp && !mo) return SpellTrap;
			if (mo && !sp) return Monster;
			return None;
		}
		return None;
	}
}
}
