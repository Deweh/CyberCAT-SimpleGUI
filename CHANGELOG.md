# Changelog

## 0.30.1

- **Patch 2.5 stats are saved correctly.** V's attribute and skill values (and other "forced" modifiers) were merged into the regular modifiers when saving, even without edits, and edits to them were lost. Stats that mention a stat type the editor didn't know (from a newer game version or a mod) made the whole entry read-only, so for example extra cyberware capacity wasn't saved. Each kind of stat data is now written back to where it came from, and unchanged stats stay byte for byte the same.
- **Opening item stats on some patch 2.5 saves crashed** with "Index was out of range". Fixed.
- **Raising cyberware capacity crashed on some patch 2.0 saves** whose player stats had no modifiers yet. Fixed.
- **Saves using mods load.** Saves with stat types or enum values added by mods (for example Weapon Conditioning) failed with `CEnum "…" could not be found`. Those values are now kept and written back unchanged. Data that script mods add to V and NPCs is read and kept too, so changing body gender works on those saves.
- **Changing body gender works on patch 2.5.** It used to leave V with a mix of both body types and the old gender's head, and the game could crash at a mirror. The new body gets a complete default appearance made for patch 2.5, the voice stays the same, and a change that can't be made leaves the save as it was. Loading an appearance preset for the other body gender only applies it if the change worked.
- **Saves using lifepath mods load.** Saves from mods like Fresh Start failed with "Specified argument was out of the range of valid values". Their lifepath now shows as "Modded" and is kept.
- **Large saves load and save.** Saves with a compression layout the editor didn't know failed with "Exception of type 'System.Exception' was thrown", and saves that grew while editing couldn't be saved.
- **Clearer messages** when `kraken.dll` can't be loaded (usually an incomplete extraction or antivirus), and on what "Make all characters romanceable" can do (some romances also need a mod).
- **Fewer files in the download.** Texture, 3D model, DirectX and database libraries the editor never used are left out, which should also mean fewer antivirus false positives.
- **License files are included.** The release contains GPL-3.0 components (WolvenKit, Kraken), see THIRD-PARTY-NOTICES.md.
