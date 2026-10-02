# Wildfeast: Isles of Wonder

A free, offline single-player Unity game prototype for Windows. Explore strange islands, bring impossible ingredients home, and grow a harbor restaurant in modern top-down pixel art.

## Play

Open `Wildfeast.exe` from the Windows build folder. Keep its accompanying data folder and DLLs together. First launch shows the controls and daily loop.

- WASD or arrow keys: move.
- E: doors, opening signs, menu boards, plants, guests, and harvesting.
- 1–8 or mouse wheel: tool belt (hands, rod, watering can, Pepperbell seeds, Lanternroot seeds, field knife, journal, bag). Click a slot to select it.
- Space or left click: use the held tool. At a bank, the rod casts into water. Hold Space or the mouse to reel; release for slack.
- Cooking: alternate A/D to chop; adjust heat with A/D and stir with Space; drag ingredients onto the plate or place them with 1/2/3.
- Gardening: equip a seed packet and use it at a bed; then equip the watering can and water separately. Foraging supplies more seeds.
- Tab: ingredient journal.
- Escape: close a panel, cancel a challenge, or open pause.
- B: bag and pantry. Escape opens settings, including the fullscreen/windowed toggle.

The player starts in borderless fullscreen; windowed mode is available in pause. Keyboard and mouse are the supported target for this pass. Complete gamepad tool selection and cooking controls remain future work.

## Your first day

Walk to the shore or spring bank. Equip the rod with 2 and cast with Space; catches animate out of the water. Gather Pepperbell from the path. Enter your restaurant to store ingredients. Flip the clearly marked CLOSED sign beside the front door (E) and choose OPEN in its short confirmation. Customers walk in and show dish icons above their heads. At the stove, choose an order, prepare/cook/plate it, and carry it to the numbered table. The separate menu board changes the offered dishes. Rest at the bed to begin tomorrow.

If ingredients run out, add Harbor Porridge to the menu. The pantry restores three grain portions each morning. Wild gathering sources recover daily; fishing can be repeated while your bag has room. Progress saves after each meaningful change.

## Beyond the first supper

Brothback patrols the northeast springs and reacts when you approach. Dodge its steam charge and collect stock with E while it cools. Search the northwest grove for Lanternroot and its seeds. Plant seeds at the beds and water on successive days. The ferry sails to either island from the start; no equipment purchase unlocks exploration or Cloudfruit. Restore the terrace and hire Nori, who walks cooked meals to guests. The provision locker increases bag capacity, and botanical gloves increase fruit yield. Four harbor letters connect discoveries to served dishes.

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
- `docs/LIVING_WORLD_PASS.md`: creator feedback, interaction redesign, controls, and remaining polish targets.

This is an implemented small-world prototype, not a finished commercial-scale game. Human playtesting should establish whether the loop and art direction warrant a larger production.
