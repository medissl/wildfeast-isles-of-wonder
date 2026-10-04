# Current restored-art guidance

Read docs/HARBOR_REST.md first. The creator explicitly asked to roll back the generated-art experiments to the established First Light design, then improve around it. Canonical building, character, creature, furniture, icon, original title background and UI sprites were restored from e08d0de. The pre-experiment scene snapshot restored the existing room/UI. Preserve this design. Do not invoke image generation, replace the visual direction, or rerun experimental global art generators. Existing tree bases are canonical; animate them without replacing their design. OriginalPresentation only rebinds the existing UI sprites/colors and cleans up unused experiment assets.

Keep the invisible Grid/Tilemap, connected road protection and existing gameplay. The three raised beds are gone. Old planted crops migrate into spatial plots without losing growth. Area 6 is a separate private bedroom; area 2 remains dining. Sleep stages the next morning in a separate model, saves atomically, then applies it; failure preserves the active day. Actions, quit and title return do not checkpoint the active day. New-slot creation, intro flag metadata, and options-only writes are separate. Energy is 100; tools cost 2 per existing animation, forage 3, valid casts 6, cooking up to 5. Exhaustion blocks world work; allow restaurant meals to finish at zero energy to avoid service deadlock. Sleep restores energy and grows watered crops. Verify isolated --rest-test, --smoke-test and --frontend-test. No packaging.

The historical generated-menu and Pixel Harbor art directions below are superseded.

# Current tile world guidance

Read docs/TILE_WORLD.md first. Terrain now uses actual Grid/Tilemap layers, not the old island background PNGs. Resources/Archipelago.json gridLand/gridRoad/gridDeck are the shared authority for drawing, water collision, road protection and planting. Integer garden coordinates remain cell centres; Grid roots offset (-.5,-.5), unit cells, centre-anchored 32px sprites. Old field saves are validated/relocated without losing crops. Resource placement.source retains old harvest IDs after snapping feet to cells.

Do not run older raster world generators over this pass. tools/tile_world_art.py generates native tile art and preserves catalog layouts once tileWorld is set. tools/plan_tile_routes.py explicitly reconnects authored destinations around solid footprints and dock cells; tools/foliage_tiles.py generates native rooted canopy/grass frames. The menu uses the restored native First Light title-horizon background and original text plaque. Do not generate replacement imagery.

ArchipelagoBuilder.Author detects URP, imports sprites, creates persistent tile assets, reauthors outdoors, adds restaurant floor Tilemaps while retaining furniture/UI, and rebakes island maps and optional guide screenshots. Back up the scene first. TidalTile is a TileBase with six frames and Grid collision; reduced motion pauses its Tilemap. VisualGuide is optional in Options; ordinary HUD/menus should contain brief labels and contextual actions. Docking and introduction use fades. The creator explicitly asked for no packaging in this pass; build an unpacked verification player only.

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
