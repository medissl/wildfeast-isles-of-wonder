# Wildfeast: Isles of Wonder

A free, offline single-player Unity game prototype for Windows. Explore strange islands, bring impossible ingredients home, and grow a harbor restaurant in modern top-down pixel art.

The latest cozy polish adds consistent Pixelify typography, pixel menu borders, connected kitchen furniture, corrected prop depth/collision and customer/staff aisles. Tools and held ingredients use compact native sprites. Actual chef poses and a windup/contact/recovery cooldown prevent tool spam. See [the polish test guide](docs/COZY_POLISH_PASS.md).

## Play

Open `Wildfeast.exe` from the Windows build folder. Keep its accompanying data folder and DLLs together. First launch shows the controls and daily loop.

- WASD / arrows: walk.
- Click hotbar, 1–0 or scroll: select an item/tool. The first inventory row is the hotbar.
- Left click / Space: use a tool; allow its windup and recovery to finish. Mouse targets must be within reach. Hold and release to reel.
- Right click / E: doors, signs, forage, guests and other world interactions.
- Tab / B: inventory; drag or click two slots to rearrange. Includes Recipes, Journal, Requests and Options tabs.
- M: island map with player and landmark markers.
- Shovel: till clear land; seed packet: plant; can: water, holding 20 units. Refill beside water.
- Axe: chop Cinnamonwood; scythe: sweep Noodlegrass; pickaxe: mine Saltstone. Trade materials at the workshop.
- Cooking: alternating A/D cuts at the chopping rhythm, A/D heat and Space stirring, then drag garnishes onto the plate (or 1/2/3).
- Escape: close/cancel or open Options. Voyages finish at the destination dock.

The player starts in borderless fullscreen; windowed mode is available in pause. Keyboard and mouse are the supported target for this pass. Complete gamepad tool selection and cooking controls remain future work.

## Your first day

Walk to the shore or spring bank. Equip the rod with 2 and cast with Space; catches animate out of the water. Gather Pepperbell from the path. Enter your restaurant to store ingredients. Flip the clearly marked CLOSED sign beside the front door (E) and choose OPEN in its short confirmation. Customers walk in and show dish icons above their heads. At the stove, choose an order, prepare/cook/plate it, and carry it to the numbered table. The separate menu board changes the offered dishes. Rest at the bed to begin tomorrow.

If ingredients run out, add Harbor Porridge to the menu. The pantry restores three grain portions each morning. Wild gathering sources recover daily; fishing can be repeated while your bag has room. Progress saves after each meaningful change.

## Beyond the first supper

Brothback patrols the northeast springs and reacts when you approach. Dodge its steam charge and collect stock with E while it cools. Search the northwest grove for Lanternroot and its seeds. Till clear ground on either island, plant seeds and water on successive days. The ferry visibly carries you along a sea route to either island from the start; no equipment purchase unlocks exploration or Cloudfruit. Restore the terrace and hire Nori, who walks cooked meals to guests. The provision locker increases bag capacity, and botanical gloves increase fruit yield. Four harbor letters connect discoveries to served dishes.

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
- `docs/ISLAND_LIFE_PASS.md`: current tools, item inventory, menus, map, sailing and audio.
- `docs/LIVING_WORLD_PASS.md`: creator feedback, interaction redesign, controls, and remaining polish targets.

This is an implemented small-world prototype, not a finished commercial-scale game. Human playtesting should establish whether the loop and art direction warrant a larger production.
