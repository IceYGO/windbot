# WindBot Ignite

A fork of [IceYGO's WindBot](https://github.com/IceYGO/windbot), ported to the
[Project Ignis: EDOPro](https://github.com/edo9300/edopro) network protocol.

This is a simple, deterministic artificial intelligence that connects as a
virtual player to the YGOPro room system. Decks for this bot player **must** be
specifically prepared and compiled as individual executors.

Written in C# targeting .NET Framework 4. Use Visual Studio 2015 or newer.

## Available decks and executors
* ABC
* Altergeist
* Blue-Eyes
* Blue-Eyes Ritual
* Burn
* Chain Burn
* Cyberse
* Dark Magician
* Dragma
* Dragunity
* Dragun of Red-Eyes
* Frog
* Gren Maju Stun
* Horus
* Lightsworn Shaddoll Dino
* Mathmech
* Normal Monster Mash
* Normal Monster Mash II
* Orcust
* Qliphort
* R5NK
* Rainbow
* Rose Scrap Synchro
* Salamangreat
* Sky Striker
* Time Thief
* Toadally Awesome
* Trickstar
* Windwitch Gusto
* Witchcrafter Grass
* Yosenju
* ZEXAL Weapon
* Zoodiac

## Contributing

Pull requests are welcome for fixes and new additions! It might take some time
for them to be evaluated since we are pretty swamped with a lot work to be done.

Please keep bug reports on Discord so we can verify them first.

For new additions, please make sure you add new code files to both the WindBot
and libWindbot projects. You need only worry about testing the WindBot project.

## Other architectural changes from upstream
[Old README](https://github.com/ProjectIgnis/windbot/tree/master/README-old.md),
including some command-line documentation.

The Visual Studio project has been merged with
[libWindbot](https://github.com/mercury233/libWindbot), meant for use as an
Android aar. Most of the code is shared with the main WindBot project, minus
the few specific bindings to call the bot as a library instead of a separate
process. The repository structure has been improved to keep the sources for
YGOSharp around as a result, but sqlite3 DLLs are still sitting around.

ExecutorBase is a refactor to experiment with loading additional executors
from DLLs in an executor folder. See SampleExecutor for an example project using
this experimental feature.

### libWindbot

To actually compile libWindbot including the post-build task that produces the
Android aar artifact, you will need the following EXACT setup. You _will_ have a
bad day otherwise and this has been kept concise.

- The postbuild event runs on Windows only.
- You must use Visual Studio 2017 or Visual Studio 2019.
- You need Visual Studio workloads for Android (Xamarin and native development).
- You must install the 32-bit Mono SDK. The 64-bit version does not work.
- In the Visual Studio 2017 `Tools > Options > Xamarin > Android Settings`,
  ensure the SDK, NDK, and JDK all point to valid paths. They should be set
  correctly by default. You can use Microsoft-provided installations or share
  these with Android Studio.
  - In addition to the default Android SDK tools, install Platform 24
    (Android 7.0). No newer platform works.
  - The NDK path must point to an r15c installation. Visual Studio 2017 should
    already have installed it somewhere but you can download this unsupported
    old version from the Android developer site. No newer NDK works.

These are all quirks of the 0.4.0 NuGet version of
[Embeddinator-4000](https://github.com/mono/Embeddinator-4000), used to
transform the .NET DLL into a native library for Android.

## License

WindBot Ignite is free/libree and open source software licensed under the GNU
Affero General Public License, version 3 or later. Please see
[LICENSE](https://github.com/ProjectIgnis/windbot/blob/master/LICENSE) and
[COPYING](https://github.com/ProjectIgnis/windbot/blob/master/COPYING) for more
details.
