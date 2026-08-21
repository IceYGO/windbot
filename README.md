# WindBot

WindBot is a C# duel bot for [YGOPro](https://github.com/Fluorohydride/ygopro). It is compatible with [YGOSharp](https://github.com/IceYGO/ygosharp) and [SRVPro](https://github.com/mycard/srvpro) servers.

## Usage

1. Build `WindBot.sln` with Visual Studio/MSBuild or Mono.
2. Place `cards.cdb` next to the compiled `WindBot.exe`.
3. Start a YGOPro host.
4. Run WindBot with options in `Key=Value` format, for example:
   ```text
   WindBot.exe Name=WindBot Deck=Blue-Eyes Host=127.0.0.1 Port=7911
   ```
5. The bot will connect to the host and start dueling.
6. Refer to `BotWrapper/bot.conf` or the `[Deck]` declarations of each file in `Game/AI/Decks` for available decks.

Contributor guidance for creating and modifying deck AIs is available in `AGENTS.md`.

## Command-line options

- `Name`: The bot's nickname.
- `Deck`: The built-in deck AI to use. Leave this empty to select a random deck with the `Normal` level. Deck names are defined by the executors in `Game/AI/Decks`.
- `DeckFile`: The `.ydk` deck file to use. It is selected automatically by `Deck`, but can be overridden. Cards not supported by the selected deck AI may not be used correctly.
- `Dialog`: The dialog set to use. Available dialog files are in `Dialogs`.
- `Host`: The host name or IP address of the YGOPro server.
- `Port`: The server port.
- `HostInfo`: The room password.
- `Version`: The YGOPro protocol version.
- `Hand`: For testing your deck, forces the bot's rock-paper-scissors choice: `1` for Scissors, `2` for Rock, or `3` for Paper. The default behavior is random for most deck AIs.
- `Chat`: Set to `False` to disable bot chat. The default is `True`.
- `Debug`: Set to `True` to print detailed card-movement logs. The default is `False`.
- `Config`: Loads additional options from a configuration file. Command-line options take precedence.
- `DbPath`: Specifies the path to `cards.cdb`. The default is `cards.cdb`. The program also looks for `cards.cdb` in the parent directory of the working directory, which is the typical location for YGOPro.
- `ServerMode` and `ServerPort`: Enable the HTTP server mode and set its port. The default port is `2399`.

## Server mode

Server mode exposes an HTTP endpoint that starts a bot for each valid request. Example:

```text
http://127.0.0.1:2399/?name=%E2%91%A8&deck=Blue-Eyes&host=127.0.0.1&port=7911&dialog=cirno.zh-CN
```

Query parameter names are lowercase. Supported parameters are `name`, `deck`, `host`, `port`, `dialog`, `version`, `password`, `hand`, `debug`, and `chat`. The `name`, `host`, and `port` parameters are required. `deckfile` is not supported.

The HTTP listener binds to all interfaces. On Windows, run with sufficient privileges or reserve the URL for the selected port (using `2399` here):

```text
netsh http add urlacl url=http://+:2399/ user=Everyone
```

## Known issue

- When a chain contains multiple activations that rely on preselected `AI.SelectCard` choices, a later activation may consume the wrong selection.

## TODO list

- Add and update deck AIs.
- Add linked-zone and non-linked-zone preferences to `AI.SelectPlace`.
- Add an `AI.SelectTribute` preselection interface.
- Keep the known-card enums up to date.
- Add more executors for commonly used cards.

## Related projects

### BotWrapper

`BotWrapper` is a lightweight launcher for YGOPro's built-in bot mode. It converts the arguments supplied by YGOPro into WindBot options, supports random bot selection, and starts `WindBot.exe`.

`BotWrapper/bot.conf` is in the format that YGOPro expects for its bot mode.

### Template generator

The [WindBot template generator](https://mercury233.me/windbot/gen.html) creates a deck AI template from a YGOPro `.ydk` file.

### WindBot Arena

[WindBot Arena](https://github.com/mercury233/windbot-arena) is a self-hosted automated duel experimentation console for running regression tests, deck challenges, smoke tests, and win-rate rankings with WindBot.
