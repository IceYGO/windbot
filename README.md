# WindBot

A C# bot for ygopro, compatible with the [YGOSharp](https://github.com/IceYGO/ygosharp) and [SRVPro](https://github.com/moecube/srvpro) server.

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

* Blue-Eyes

* Dragunity

* Qliphort

* Rainbow

* Rank V

* ST1732

* Toadally Awesome (old lflist)

* Yosenju

* Zexal Weapons

* Zoodiac (old lflist, master rule 3 only)

### Unfinished decks

* Blackwing

* CyberDragon

* Evilswarm

* Gravekeeper

* Graydle

* Lightsworn

* Nekroz

### Server mode

WindBot can run as a "server", provide a http interface to create bot.

eg. `http://127.0.0.1:2399/?name=%E2%91%A8&deck=Blue-Eyes&host=127.0.0.1&port=7911&dialog=cirno.zh-CN&version=4928`

In this situation, it will be multi-threaded. This can be useful for servers, since it don't use large amount memory.

The parameters are same as commandlines, but low cased.

### Known issues

* The bot will attack synchro monsters next to _Ultimaya Tzolkin_ because it don't know _Ultimaya Tzolkin_ can't be attacked.

* The attack won't be canceled when battle replay happens.

* If one chain includes two activation that use `AI.SelectCard`, the second one won't select correctly.

### TODO list

* More decks

* Documents for creating AI

* `AI.SelectZone`

* `AI.SelectMaterials` which select a set of cards for F/S/X/L summon

* `AI.SelectTribute`

* Better new master rule support

* Update the known card enums

* More default common cards executor
