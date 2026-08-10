# WindBot

A C# bot for [YGOPro](https://github.com/Fluorohydride/ygopro), compatible with the [YGOSharp](https://github.com/IceYGO/ygosharp) and [SRVPro](https://github.com/mycard/srvpro) server.

### How to use:

* Compile `WindBot.sln` using Visual Studio or Mono.

* Put `cards.cdb` next to the compiled `WindBot.exe`.

* Run YGOPro, create a host.

* Run WindBot and observe.

### Supported commandlines

`Name`
The nickname for the bot.

`Deck`
The deck to be used by the bot. Available decks are listed below. Keep empty to use random deck.

`DeckFile`
The deck file (.ydk) to be used by the bot. Will be set by `Deck` automatically, but you can override it.

Note: Most cards not in the original deck are unknown to the bot, and won't be summoned or activated in the duel.

`Dialog`
The dialog texts to be used by the bot. See Dialogs folder for list.

`Host`
The IP of the host to be connected to.

`Port`
The port of the host to be connected to.

`HostInfo`
The host info (password) to be used.

`Version`
The version of YGOPro.

`Hand`
If you are testing deck, you may want to make sure the bot go first or second. `Hand=1` will make the bot always show Scissors, 2 for Rock, 3 for Paper.

`Chat`
False to turn the chat of bot off.

`Debug`
Print verbose log of card movement info. False at default. (May be updated in future)

`ServerMode` and `ServerPort`
WindBot can run as a "server", provide a http interface to create bot.

### Available decks

**Easy**:

* Burn

* Frog

* Horus

* MokeyMokey

* MokeyMokeyKing

* OldSchool

**Normal**:

* Altergeist

* Blue-Eyes

* BlueEyesMaxDragon

* Brave

* ChainBurn

* DarkMagician

* Dragun

* Dragunity

* GrenMajuThunderBoarder

* Level VIII

* LightswornShaddoldinosour

* Orcust

* Phantasm

* Qliphort

* Rainbow

* Rank V

* Salamangreat

* SkyStriker

* ST1732

* ThunderDragon

* Toadally Awesome

* Trickstar

* Yosenju

* Zexal Weapons

* Zoodiac

### Unfinished decks

* Blackwing

* CyberDragon

* Evilswarm

* Gravekeeper

* Graydle

* Lightsworn

* Nekroz

### Template Generator

A tool which generates a WindBot deck code template from a YGOPro deck file.

You can use it to create a new deck for WindBot quickly.

https://mercury233.me/windbot/gen.html

### Server mode

WindBot can run as a "server", provide a http interface to create bot.

eg. `http://127.0.0.1:2399/?name=%E2%91%A8&deck=Blue-Eyes&host=127.0.0.1&port=7911&dialog=cirno.zh-CN`

In this situation, it will be multi-threaded. This can be useful for servers, since it don't use large amount memory.

The parameters are same as commandlines, but low cased.

Note: Currently the server bind to all interfaces, so it requires elevated privileges to run. You can otherwise use the following command to add a URL ACL for your port (2399 for example), which allows all users to access it:
```
netsh http add urlacl url=http://+:2399/ user=Everyone
```

### Changelog

View [CHANGELOG.MD](CHANGELOG.md) for information regarding the changes made during updates

### TODO list

* More decks

* Documents for creating AI

* `AI.SelectPlace` for linked zones or not linked zones

* `AI.SelectTribute`

* Get equip of card.

* Better new master rule support

* Update the known card enums

* More default common cards executor
