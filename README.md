# Wildfeast: Isles of Wonder

A free, offline single-player Unity game prototype for Windows. Explore strange islands, bring impossible ingredients home, and grow a harbor restaurant in modern top-down pixel art.

## Play

Open `Wildfeast.exe` from the Windows build folder. Keep its accompanying data folder and DLLs together. First launch shows the controls and daily loop.

- WASD or arrow keys: move.
- E: interact with the nearest fishing spot, plant, creature, door, or table.
- Space: reel while held; release to give slack. During cooking, press once to finish.
- Tab: ingredient journal.
- Escape: close a panel, cancel a challenge, or open pause.
- Mouse: menus and cooking controls. Fishing can also be reeled while holding the mouse in its panel.

Basic gamepad movement, interaction, and UI navigation are implemented but have not been physically tested. Keyboard and mouse are the verified target.

## Your first day

Walk southeast to the Leafgill fishing spot. Catch three fish and pick Pepperbell from the shore path. Enter your restaurant to transfer ingredients into storage. At the menu table, select stocked recipes; open the restaurant from its welcome table. Cook each order at the stove and carry it to the guest's numbered table. Use your shells at the harbor workshop, then begin the next day from the closing screen or the closing-day station.

If ingredients run out, add Harbor Porridge to the menu. The pantry restores three grain portions each morning. Wild gathering sources recover daily; fishing can be repeated while your bag has room. Progress saves after each meaningful change.

## Beyond the first supper

At the northeast springs, observe Brothback's steam warning, step aside from its charge, and approach with E during its cooling phase. Search the northwest grove for Lanternroot. Plant discovered Pepperbell or Lanternroot at home and water on successive days. Restore the terrace, hire Nori to deliver dishes, restore the boat, and take a reach tool to Mistwake Isle. Find floating Cloudfruit and the tidekeeper's letter. Four harbor requests connect the habitats to dishes served at your table.

## Open the Unity project

Use Unity Hub to open `Game/` with **Unity 6000.3.7f1**. Open `Game/Assets/Wildfeast/Scenes/Wildfeast.unity` and enter Play mode. The repository includes the serialized, integrated scene and all required original art. Unity will regenerate its ignored Library folder on a fresh checkout.

From the repository root, `tools/build.ps1` builds a Windows player and `tools/test.ps1` runs the Unity rule tests. Both accept `-UnityEditor` if your editor executable is in a different location.

Editor and standalone saves use separate files. See `docs/HANDOFF.md` for their location and project tooling. Rebuild the scene only intentionally via **Wildfeast → Assemble playable scene**; this reconstructs the authored scene and replaces manual edits to it. Regular gameplay iteration should edit the scene normally.

## Documentation

- `docs/WILDFEAST_PLAN.md`: original vision and milestone definitions.
- `docs/WORK_MODE_PROMPT.md`: development instructions.
- `docs/DECISIONS.md`: settled choices and tradeoffs.
- `docs/PRODUCTION_PLAN.md`: content budget, release scope, accessibility, and performance goals.
- `docs/VERIFICATION.md`: actual checks and limitations.
- `docs/HANDOFF.md`: implementation status and next work.
- `docs/ASSET_PROVENANCE.md`: original artwork and audio sources.

This is an implemented small-world prototype, not a finished commercial-scale game. Human playtesting should establish whether the loop and art direction warrant a larger production.
