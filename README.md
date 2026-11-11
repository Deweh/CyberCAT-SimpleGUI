# CyberCAT-SimpleGUI

A save editor for Cyberpunk 2077, including patch 2.5. A simplified offshoot of SirBitesalot's CyberCAT, by Deweh and contributors.

**Download:** [Nexus Mods](https://www.nexusmods.com/cyberpunk2077/mods/718) or the [latest GitHub release](https://github.com/Deweh/CyberCAT-SimpleGUI/releases/latest). Extract the whole zip into a folder before running the editor.

**What's new:** see [CHANGELOG.md](CHANGELOG.md).

## Features

- **Saves browser:** your saves with screenshot, level, life path, play time and patch; double click to open.
- **Safe saving:** timestamped backups (last 5), the file is verified before replacing `sav.dat`, warning on unsaved changes, `Ctrl+S`.
- **Undo/redo:** `Ctrl+Z` / `Ctrl+Y` (outside text fields), up to 10 steps. Edits made in quick succession count as one step.
- **Inventory:** in-game item names (English or Brazilian Portuguese), search with filters (`type:`, `slot:`, `quantity:`...), add any item, copy/paste, move between inventory and stash, bulk *Make Tier 5++* and *Set Quantity*, crafting components, money.
- **Items:** quantity, flags, mod tree and stats of each item.
- **Player:** level, attributes, skills, perk/attribute/relic points.
- **Perks:** the 2.0 skill trees and Relic perks, with or without spending points.
- **Cyberware:** equip any cyberware in any slot of its body area and raise the cyberware capacity.
- **Vehicles:** unlock or lock any player vehicle.
- **Appearance:** presets, voice, and body gender (with a complete default appearance for the new body).
- **Quest facts, fast travel** and support for several mods' saved data.
- **Modded saves:** stat types and other values added by mods, data that script mods add to V and NPCs, and lifepath mods (shown as "Modded") are kept and written back unchanged.

Saves from game versions newer than the editor supports open read-only, so they can't be damaged.

## Game Version Support

**Game Version**    | **Support** | **CyberCAT-SimpleGUI Version**
--------------------| ------------|-----------------------------
Patch 2.5           | Full        | >=v0.29.0
Patch 2.0 - 2.3     | Full        | >=v0.28c
Patch 1.5X          | Full        | >=v0.20a
Patch 1.3           | Partial     | >=v0.10a_r1
Patch 1.23          | Full        | >=v0.10a

## Usage

Requires the [.NET 10 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/10.0) (x64).

1. Run **CP2077SaveEditor.exe** and double click a save (or use **Load Save** for one somewhere else).
2. Make your changes. Double click items in the inventory to edit them.
3. Click **Save** (`Ctrl+S`) to overwrite the loaded save, or **Save As...** to write it into another save folder.

Errors are written to `logs\` next to the executable, attach that file to bug reports.

**Always keep a copy of your saves.** The editor keeps backups too, but edits to quest facts and item flags can break things hours later in the game.

If loading fails with a message about `kraken.dll`, the zip wasn't fully extracted or an antivirus quarantined the file. Antivirus false positives happen with unsigned tools like this one; the full source is in this repository.

The editor follows the Windows light/dark setting. To force one, create a `config.json` next to the executable with `{ "Theme": 1 }` (light) or `{ "Theme": 2 }` (dark).

Other `config.json` options: `"UndoSteps"` (default 10, `0` turns undo off; each step keeps ~4 MB in memory) and `"CheckForUpdates": 0` to skip the update check on startup.

Planned work is tracked in [TODO.md](TODO.md).

## Credits

- [Deweh (DeweySalt)](https://github.com/Deweh), the original CyberCAT-SimpleGUI, and Papayu for later releases
- [bernardomcma](https://github.com/bernardomcma), patch 2.5 support and the v0.29/v0.30 features
- [topagae](https://github.com/topagae), patch 2.5 fixes merged in v0.30.1 (stats, modded saves, body gender, large saves, licensing), written about 95% by Claude (Anthropic's AI model, via Claude Code) and directed and tested by topagae
- [CyberCAT by SirBitesalot and other contributors](https://github.com/WolvenKit/CyberCAT)
- [WolvenKit by the WolvenKit team](https://github.com/WolvenKit/WolvenKit)

## License

The editor's source code is MIT licensed, see [LICENSE](LICENSE). The release also contains WolvenKit and the Kraken
decompressor, which are licensed under the GNU GPL v3.0, so the program as distributed is covered by the GPL v3.0.
Anyone who redistributes it must also make the corresponding source available: this repository at the release's tag,
including the `WolvenKit` submodule commit and `patches/WolvenKit.patch`.
[THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md) lists every component in the release with its license, and the release
zip includes these files.

## Contribution

Clone with submodules (`git clone --recursive`, or `git submodule update --init` on an existing clone).
Newer game patches usually need the `WolvenKit` submodule updated to a commit that supports them.

### Changes to WolvenKit

The `WolvenKit` submodule points at upstream [WolvenKit](https://github.com/WolvenKit/WolvenKit). This repository's
changes to it are kept in [patches/WolvenKit.patch](patches/WolvenKit.patch), and
[Directory.Build.targets](Directory.Build.targets) applies the patch before WolvenKit builds (and skips it when it's
already applied). Each modified WolvenKit file is marked in its first line, as the GPL requires.

To change WolvenKit code: edit the files in `WolvenKit/`, then run `tools/update-wolvenkit-patch.ps1` and commit the
updated patch. After updating the submodule to a newer WolvenKit, reset it (`git submodule update --init --force`),
check that the patch still applies, and fix it where it doesn't.

### Tests

Round-trip tests (load, edit, save, reload) run against a real save, which is not part of the repository.
Point `CP2077_TEST_SAVE` to a `sav.dat` (ideally a copy) and run:

```
dotnet test --project CP2077SaveEditor.Tests/CP2077SaveEditor.Tests.csproj -p:Platform=x64
```

Without the variable the tests are skipped.

### Releases

Pushing a version tag (`v0.30.1`) builds the release, attaches the zip to a GitHub release, and optionally:

- **Uploads it to Nexus Mods** as a new version of the main file, with the matching `CHANGELOG.md` section as the
  changelog. Set the repository variables `NEXUS_MOD_ID` and `NEXUS_FILE_ID` and the secret `NEXUS_API_KEY`
  ([API keys](https://www.nexusmods.com/settings/api-keys)). Both IDs are shown on the mod page's Files tab with
  "Advanced" checked.
- **Scans it with VirusTotal** first: set the secret `VIRUSTOTAL_API_KEY` (a free key works). Results are in the run
  summary. `VIRUSTOTAL_MAX_DETECTIONS` holds the Nexus upload when more engines flag the zip, since Nexus quarantines
  uploads with too many detections.

Before tagging, bump `<Version>` in `CP2077SaveEditor/CP2077SaveEditor.csproj` and add a section to `CHANGELOG.md`.

### Game data

Item classes, names, perks, vehicles and cyberware come from the game files. After a game patch, regenerate them with:

```
dotnet run --project ResourceGenerator -p:Platform=x64 -- "<game folder>" all --out CP2077SaveEditor/Resources
```

### VSCodium

**Task** | **Info**
-------- | ----------------------------------
Requires | .NET 10 SDK
Setup    | None
Build    | Terminal > Run Build Task...
Debug    | Unsupported

### Visual Studio

**Task** | **Info**
-------- | ----------------------------------
Requires | A version with .NET 10 support, C# Desktop workspace
Setup    | Solution > Restore NuGet Packages
Build    | Build > Rebuild Solution
Debug    | Debug > Start Debugging
