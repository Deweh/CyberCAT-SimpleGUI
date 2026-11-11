# CyberCAT Reloaded by bernardomcma

A Cyberpunk 2077 save editor with support for patch 2.5 and Phantom Liberty. CyberCAT Reloaded continues [CyberCAT-SimpleGUI by Deweh](https://github.com/Deweh/CyberCAT-SimpleGUI), itself a simplified offshoot of SirBitesalot's CyberCAT.

**Download:** [latest release](https://github.com/bernardomcma/CyberCAT-SimpleGUI/releases/latest)

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
- **Cloud saves:** download a save from the CD PROJEKT RED cross-save cloud, edit it and upload it back, without the game installed on the PC (see [Cloud saves](#cloud-saves)).
- **Quest facts, fast travel, appearance presets** and support for several mods' saved data.

Saves from game versions newer than the editor supports open read-only, so they can't be damaged.

## Game Version Support

**Game Version**    | **Support** | **Editor Version**
--------------------| ------------|-----------------------------
Patch 2.5           | Full        | >=v0.29.0
Patch 2.0 - 2.3     | Full        | >=v0.28c
Patch 1.5X          | Full        | >=v0.20a
Patch 1.3           | Partial     | >=v0.10a_r1
Patch 1.23          | Full        | >=v0.10a

## Usage

Each release has two downloads:

- **CyberCAT-Reloaded-Standalone.zip** (recommended): a single `CP2077SaveEditor.exe` with .NET included, nothing else to install.
- **CyberCAT-Reloaded.zip**: smaller, but needs the [.NET 10 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/10.0) (x64).

1. Run **CP2077SaveEditor.exe** and double click a save (or use **Load Save** for one somewhere else).
2. Make your changes. Double click items in the inventory to edit them.
3. Click **Save** (`Ctrl+S`) to overwrite the loaded save, or **Save As...** to write it into another save folder.

Errors are written to `logs\` next to the executable, attach that file to bug reports.

The editor uses a dark, Cyberpunk-styled theme by default. For the classic light look, create a `config.json` next to the executable with `{ "Theme": 1 }`, or `{ "Theme": 0 }` to follow the Windows light/dark setting.

Other `config.json` options: `"UndoSteps"` (default 10, `0` turns undo off; each step keeps ~4 MB in memory) and `"CheckForUpdates": 0` to skip the update check on startup.

Planned work is tracked in [TODO.md](TODO.md).

## Cloud saves

The **Cloud...** button works directly with the CD PROJEKT RED cross-save cloud, the same one the game uses. You don't need Cyberpunk 2077 installed on the PC, which is useful for saves from consoles such as the PS5 (the feature was developed and tested with a PS5 save) that would otherwise be hard to reach.

1. Turn on cross-save in the game with your CD PROJEKT RED account, and make sure the save you want is synced to the cloud.
2. Click **Cloud...** and **Sign in**. Your browser opens the CD PROJEKT RED sign-in page; the password is only ever typed there. The editor stays signed in until you close it and never writes the access token to disk.
3. Select a slot and click **Download and open** to edit it like any other save.
4. To send it back, open the save, click **Cloud...**, select the slot and click **Upload open save to slot**. The editor saves the open save first (with the usual backup and verification), keeps a copy of the previous cloud version in `%LOCALAPPDATA%\CyberCAT\cloud-backups`, uploads and reads the result back to check it.

Notes:

- Make your own backup of your cloud saves first. Uploading overwrites the slot.
- This uses the cross-save API the game itself talks to. It is not publicly documented, so CD PROJEKT RED can change it at any time and the feature may stop working.
- Downloaded saves are kept in `%LOCALAPPDATA%\CyberCAT\cloud-downloads`.

## Credits

- [CyberCAT-SimpleGUI by Deweh](https://github.com/Deweh/CyberCAT-SimpleGUI), the original editor this fork continues
- [CyberCAT by SirBitesalot and other contributors](https://github.com/WolvenKit/CyberCAT)
- [WolvenKit by the WolvenKit team](https://github.com/WolvenKit/WolvenKit)

## Contribution

Clone with submodules (`git clone --recursive`, or `git submodule update --init` on an existing clone).
Newer game patches usually need the `WolvenKit` submodule updated to a commit that supports them.

### Tests

Round-trip tests (load, edit, save, reload) run against a real save, which is not part of the repository.
Point `CP2077_TEST_SAVE` to a `sav.dat` (ideally a copy) and run:

```
dotnet test --project CP2077SaveEditor.Tests/CP2077SaveEditor.Tests.csproj -p:Platform=x64
```

Without the variable the tests are skipped.

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
