# Current First Light guidance

Read docs/FIRST_LIGHT.md and docs/FIRST_LIGHT_PLAYTEST.md first. Normal startup opens JourneyFrontEnd; only --smoke-test bypasses it. --frontend-test exercises the actual title/creation/intro/slots flow with isolated --save-path and --test-output. Preserve five independent slot files and the legacy save. Avatar, introSeen and landmarks are additive state; slot creation initializes inventory before writing. CharacterLook changes actual native walk/action frames, keeping 32 PPU and point filtering. Its temporary textures require safe runtime/editor cleanup.

Run tools/first_light_art.py AFTER wonder_art.py, preferably with Python -X utf8. It enlarges outer landforms, adds five grounded responsive discoveries and water habitats, and appends original Pearlfin/chowder content (20 ingredients, 21 recipes). Terrain, collision, maps and arrivals use the shared catalog. Author outdoors with ArchipelagoBuilder.Author after backing up the scene; preserve room/UI. Chef/action textures must remain readable for customization, large island textures use a 4096 cap. C# source stays valid UTF-8.

FishingChallenge is a moving fish and inertial catch zone, with gain inside and slow loss outside. Preserve cast/wait/track/retrieve stages, one reward after retrieval, and safe cancellation. World input must remain blocked during menu/intro. New never overwrites an occupied slot. Keep model changes and discovery rewards/water/recipe unlocks atomic. Previous Wonder diagnostics still apply.

# Wildfeast project guidance

Read docs/WONDER_REDESIGN.md and docs/WONDER_PLAYTEST.md first for current art/geography. Run tools/wonder_art.py LAST after archipelago_art.py. It preserves IDs while replacing terrain, creatures, characters and sign lettering. Island dimensions/docks/arrivals differ: consume catalog size/dock/arrival in camera, map, ferry and save startup. Soil is centred and its entire footprint must avoid roads/water. Preserve body targeting, calm/lure grace, visible ready cues and persistent boar stock. Re-run body-click, dry-docking and isolated saved-island relaunch diagnostics when changing these flows.

Read `docs/HANDOFF.md`, `docs/DECISIONS.md`, and `docs/VERIFICATION.md` before extending the game. The original vision is in `docs/WILDFEAST_PLAN.md`. Work in the Unity project at `Game/`; the repository root is not the Unity project root.

## Direction

Modern top-down pixel art. Offline single-player Windows game, free without ads or purchases. Exploration and restaurant growth must remain connected through original food ecology. The creator expanded the scope to five designed islands, 19 ingredients and 20 recipes; preserve that coherent food ecology and the restaurant loop.

## Implementation

- Unity 6000.3.7f1, URP 2D, Input System, uGUI/TMP.
- Play scene: `Game/Assets/Wildfeast/Scenes/Wildfeast.unity`.
- Preserve .meta files. Never commit Library, Temp, logs, generated projects, or build output.
- Every serialized MonoBehaviour belongs in a file with its class name.
- Domain state and transactions live in `GameData.cs` and `ItemInventory.cs`; external tuning definitions live in `Resources/Content.json`. Scene and UI integration are real serialized references.
- Keep quantity changes, crop resets, payouts, and purchases atomic with respect to save checkpoints.
- Use supported Editor tooling for scene/prefab/asset edits. Detect the pipeline before changing pixel rendering. Do not hand-edit scene YAML or the package manifest.
- Ordinary scene edits should preserve manual work. `ProjectBuilder.Assemble` intentionally reconstructs the scene; do not invoke it on top of someone else's manual edits without first preserving them.
- Regenerate original art/audio in order: `tools/make_art.py`, `tools/polish_art.py`, `tools/creature_art.py`, then `tools/island_life_art.py`, then `tools/cozy_polish_art.py`, then `tools/archipelago_art.py` last; `tools/archipelago_music.py` generates the three new music loops, requiring Python, Pillow and numpy. Intentional scene assembly rebakes illustrated maps. Keep bundled TMP font license files.
- Read docs/ARCHIPELAGO_EXPANSION.md first. Preserve the five maps, shared terrain catalog/road rules, persistent ecology contacts, new seed identities, paged books and independent sound/zoom settings. Use catalog island IDs (0, 1, 3, 4, 5); area 2 remains the restaurant. Verify road walking clearance after layout changes.
- Read docs/COZY_POLISH_PASS.md. Preserve the Pixelify font/license, pixel widgets, compact held props, directional chef actions, contact/recovery cooldowns, furniture depth/collision and clear restaurant aisles.
- Read docs/ISLAND_LIFE_PASS.md. Preserve item hotbar/backpack placement, runtime pointer callbacks, Tab categories, M maps, spatial farming, finite water, resources, original music and continuous ship travel.
- Read docs/LIVING_WORLD_PASS.md. Preserve the creator's free ferry travel, visible tools, seed/can actions, quiet HUD, proximity creature behavior and distinct prep/cook/plate cooking. Do not revert to the initial prototype's modal activation interactions or travel purchase gates.
- Do not present blockouts, automated test inputs, or offscreen performance samples as finished art, human playtesting, or actual display FPS.

## Checks

Run relevant Unity tests. The economy/save suite is in `Assets/Wildfeast/Tests/Editor/`. Run the standalone integrated journey when changing the end-to-end flow, using `--smoke-test --save-path <isolated-file> --test-output <directory>`; diagnostic mode uses a virtual Input System keyboard and does not alter an ordinary save. Check its result, runtime-error log, and captured frames, not just the executable's presence.

`Wildfeast.Editor.ProjectBuilder.Build` builds the authored scene. `-wildfeastBuild <absolute-executable-path>` selects an output folder. For a fresh bootstrap only, `ImportFontsAndExit` must run without `-quit` and confirm completion before scene assembly. Required font resources are already checked in.

Keep verification and handoff documents current. State what actually passed and what remains untested, especially physical controller support and fresh-player comprehension. Keep changes small and preserve unrelated edits.
