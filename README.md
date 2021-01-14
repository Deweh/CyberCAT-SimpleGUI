# CyberCAT-SimpleGUI

A simplified offshoot of SirBitesalot's CyberCAT.

## Features

**Feature**                                                                     | **Stability**
------------------------------------------------------------------------------- | -----------
Save and load presets for your character's appearance.                          | Stable
Edit the quantity, flags, & mod tree of items in your inventory.                | Stable
Edit quest facts.                                                               | Stable
Quick actions - dedicated controls for changing money & making items legendary. | Semi-Stable

## Usage

1. Run **CP2077SaveEditor.exe**
2. Click **"Load Save"**
3. Make changes to your save.
    - Double-click items in your inventory to edit them.
    - Double-click nodes in an item's mod tree to edit them
4. Click **"Save Changes"**

## Todo

- Refactor, refactor, refactor.
- Eat some of the spaghetti.
- Improve the functionality of the appearance tab.
- Move contribution section from readme to github wiki

## Credits

[CyberCAT by SirBitesalot and other contributors](https://github.com/WolvenKit/CyberCAT)

## Contribution

### VSCodium

**Task** | **Info**
-------- | ----------------------------------
Requires | .NET 5.0 SDK
Setup    | None
Build    | Terminal > Run Build Task...
Debug    | Unsupported

### Visual Studio 2019

**Task** | **Info**
-------- | ----------------------------------
Requires | .NET 5.0 SDK, C# Desktop workspace
Setup    | Solution > Restore NuGet Packages
Build    | Build > Rebuild Solution
Debug    | Debug > Start Debugging
