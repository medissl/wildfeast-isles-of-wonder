# Latest: restored First Light design and Harbor Rest

The creator explicitly requests preserving the existing design before the generated-art experiments. See HARBOR_REST.md and the top of AGENTS.md. Original art/UI were restored from e08d0de; preserve the grid and targeted bedroom, energy and sleep checkpoint additions. Earlier generated-menu directions are superseded.

# Current handoff: tile world

Read TILE_WORLD.md and the newest VERIFICATION entry first. Five outdoor maps now use native Unity Grid/Tilemap layers: land, connected trails, animated collision water and cultivated soil. The restaurant has timber/kitchen/rug Tilemaps. Catalog integer cell arrays are shared with domain tilling and spatial water checks. Centres remain at integer coordinates for old saves. Footprint clearance and one connected trail network per island are tested; docks use protected wooden cells. Resources snap to cells with preserved source identities. The initial scene backup is outputs/tile-world-scene-before.unity outside Git.

Original ground/path/bank/deck/soil textures use 32px tiles, point filtering and no compression or mipmaps. The canopy and grass art has richer clustered shading and rooted frames. TidalTile derives directly from TileBase and supplies six frames plus Grid collision. Reduced motion pauses water animation. Island maps and the picture guide are actual URP captures. Normal player startup retains five slots, creation, the skippable intro and all existing game content. The restored original pixel title background and rustic plaque replace the rejected generated imagery; clouds/glints animate behind it. Optional VisualGuide cards replace repeated control paragraphs. Docking and intro cuts fade.

Use tile_world_art.py, plan_tile_routes.py and foliage_tiles.py for this art direction. Preserve the original First Light title background when regenerating native art. Reauthor through ArchipelagoBuilder.Author, retaining room/UI; do not run Assemble or older raster generators. The creator requested no packaging: the current verification player is outputs/Wildfeast-Tile-World/Wildfeast.exe, without new ZIP archives. Human judgement of texture density, path readability and guide comprehension still needs playtesting.

# Current handoff: First Light

Read FIRST_LIGHT.md, FIRST_LIGHT_PLAYTEST.md and the newest VERIFICATION entry. Normal boot now shows the animated title. JourneyFrontEnd owns New/Load/Options/Exit, five JourneySlots, legacy continue, real sprite customization and a skippable 24-second in-world introduction. GameController switches model/save subscriptions and IslandLife clears old plot views when changing journeys. Avatar choices affect all native direction/action frames through cached temporary CharacterLook textures; the original assets remain unchanged. SaveStore normalizes fresh inventory before writing, validates additive avatar/intro/landmark fields, and preserves legacy saves.

FishingChallenge tracks moving fish with an inertial catch zone and slow recoverable progress loss. WorldView animates the cast float/line, player poses, retrieval and flying ingredient; reward follows retrieval. All island habitats sit in water; schools inhabit springs and shore habitats. Original Pearlfin and Pearlfin Chowder raise the preserved catalog to 20 ingredients and 21 dishes. The five enlarged areas measure 44×38, 40×58, 76×30, 44×68 and 78×42 world units. Existing arrivals and docks retain their positions.

Each outer region has a unique one-time interactive discovery, story, native awake sprite and recipe/food reward. Dinner bell and compass use contact animations, kettle uses water, quarry uses pickaxe, harp uses scythe. Rewards are one atomic model checkpoint. The journal includes the five landmarks. Paths remain protected and physically clear; bases have collision beside their approaches.

Run first_light_art.py after wonder_art.py, using Python -X utf8. It reuses the original terrain painter without re-executing the preceding redesign. Author via ArchipelagoBuilder.Author; large textures use 4096, other textures keep 2048, chef/action frames are readable. Preserve UTF-8 C# text. The pre-pass scene backup is outputs/first-light-scene-before.unity outside the repo. --frontend-test is separate from the existing --smoke-test journey, both require isolated directories. FirstLightRunner uses public URP render requests to capture hidden windows; ScreenCapture alone produces black images in batch mode.

Human judgement of fishing feel, exploration density, character art and introduction pacing remains necessary. Five slot files have no in-game delete UI; full slots are never silently replaced. Title preferences apply to new journeys; loaded journeys retain their own sound/zoom/motion choices. This is still a five-island prototype rather than a completed open-world campaign.

# Current handoff: Wonder redesign

Read WONDER_REDESIGN.md and WONDER_PLAYTEST.md first. The five islands now have different size/dock/arrival vectors. ProjectBuilder.BakeIslandMaps renders their native dimensions, GameUI.Map fits without distortion, WorldView clamps camera to each map, and sailing/save startup use their real landing points. Road protection checks the centred soil footprint rather than its former bottom-pivot centre. Old fields relocate inside current bounds without losing crops. Body clicks resolve FoodEcology origins; calm/lure grace lets players approach to collect, ready ingredients are visible, and Brothback stock no longer expires while approaching. Preserve existing IDs (including spicecrab art/crab-ember source) despite the new Spicepangolin presentation.

The final original art generator is tools/wonder_art.py, run after archipelago_art.py. It redraws geography, independent creatures, chef/walk/action frames, visitor outfits, grounded forage, material props and measured signs. Author with ArchipelagoBuilder.Author; outdoor roots/maps change while room and UI references remain. The pre-pass scene backup is work/wonder-before.unity outside the repo. Standalone diagnostics add --verify-island-spawn --expected-island <id> alongside --smoke-test and isolated --save-path/--test-output for a fresh-process landing check.

# Current handoff: five-island archipelago

Read ARCHIPELAGO_EXPANSION.md and the newest verification first. There are five actual island roots: Saltleaf 0, Mistwake 1, Emberfold 3, Moonfen 4 and Pearltide 5; **2 remains the restaurant** to retain legacy scene/save meaning. WorldView.islands stores roots in catalog order. Use IslandRoot(area), Archipelago.Valid/Get and the catalog instead of assuming every non-home area is Mistwake.

Resources/Archipelago.json is the shared layout source: coast, main/secondary pools, road polylines, habitat regions, props and interaction points. archipelago_art.py authors it with original terrain, native foliage/grass frames, resources, four creatures, forage/seed icons and distinct dishes. Run it AFTER cozy_polish_art.py. archipelago_music.py generates three additional original music themes; the six music clips are selected by area key. Preserve the Pixelify font, actual chef contact/recovery actions and restaurant furniture/aisles.

ArchipelagoBuilder.Author replaces only the outdoor roots, retaining restaurant and UI objects, then bakes all five maps through public URP render requests. The preceding scene was backed up to work/fifth-pass-scene-backup.unity outside the repo. Assemble remains a whole-scene bootstrap. No scene YAML was edited manually. The preexisting untracked PackageManagerSettings.asset remains excluded.

FoodEcology presents ram quietness, a cracked spice shell, Lanternroot attraction, watering reactions and knife sap tapping. ItemInventory.EcologyAction owns atomic persistent contacts/water transactions. Progress adds independent music/effect levels, zoom, reducedMotion, seeds and ecology; no schema reset or new economy. Old combined volumes migrate. Old crop plots that now hit roads/water relocate while preserving growth and quantities. Six crops can grow in spatial plots; the original three authored beds keep their two original seed types.

GameUI uses five-row pagination for the 19-entry journal and 20-recipe book, five ferry choices, current-island M charts, two actual audio sliders and sound/camera/comfort Options. Terrain is green/tillable versus visibly protected roads; physical placement checks still reject solid objects. World motion swaps native foliage frames, keeping roots and trunks stationary. Ground creature bodies have footprints; the hovering moth does not.

Release: Wildfeast-Archipelago-Windows.zip and matching Unity source. Verification captures cover both supported screen sizes, all five maps, Options and recipe pages. Human art judgement, progression balance, fresh-player comprehension and physical controller support still need playtesting.

# Current handoff: cozy polish

Read COZY_POLISH_PASS.md and the latest verification section first. Typography now uses one static Pixelify Sans font throughout authored and runtime UI, with original pixel slot/button borders. Large minigame images were removed from the restaurant and replaced with native world furniture, corrected footprints, ground depth and immediate centered framing. Compact held ingredients and cleanup of area effects prevent oversized pickup images from covering the room. Guests and Nori use clear aisle routes.

ToolActionClock owns a windup/contact/recovery sequence; use its Busy state to gate movement, equipment and repeated tool input. Capture the target at windup, apply the model operation once at contact, and retain recovery even after contact. Forage/fishing remain activity locks. Cooking cuts and stirring also use the clock. WorldView selects actual directional chef action frames, and PropDepth sorts solid furniture from its feet. Save schema and persistent quantities remain unchanged.

Preserve the original scale: all world sprites are 32 PPU. Small held assets have smaller native canvases; they are not scaled-down full-size monsters. Add cozy_polish_art.py after the earlier generation scripts. The Pixelify font's OFL license is included; retain it in builds and source distributions.

CozyPolish.Typography / ArtAndRoom are targeted Editor repairs; they preserve the scene hierarchy. Back up manual work before Assemble, which intentionally reconstructs it. A fourth-pass original scene backup is outside the repo in work/fourth-pass-scene-backup.unity. The unrelated PackageManagerSettings.asset remains untracked and excluded.

Latest delivery is Wildfeast-Cozy-Polish-Windows.zip, with source and verification captures. Previous Island-Life archives are historical.

# Current handoff: island-life pass

Read ISLAND_LIFE_PASS.md and the current section of VERIFICATION.md first. The entries below describe earlier builds where they differ.

New runtime modules: ItemInventory.cs owns item layout, resources and persistent field transactions; IslandLife.cs handles spatial tools, forage animation, continuous sailing and resource presentation; InventoryDrag.cs supplies actual pointer dragging; HarvestNode.cs tracks visible tool-hit state. GameUI restores serialized hotbar button listeners during Init — Editor-created runtime lambdas were not persisted, which caused the old click bug.

Progress schema 1 is extended additively with slots[40], fields, resources and water. Existing bag/pantry, seed counts, legacy three beds, orders, upgrades and money remain authoritative and preserved. Slots synchronize quantities while retaining placement. Do not create a second inventory economy or replace existing save files during tests. Food capacity still counts portions (8 initially, up to 20); tools/seeds/materials use their own slots.

ProjectBuilder.Assemble now also bakes scenery maps through URP into map-saltleaf/map-mistwake textures. It reconstructs the scene; preserve manual scene work first. Normal Build uses the checked-in scene. Regenerate art in order make_art → polish_art → creature_art → island_life_art, then intentionally assemble to rebake maps. island_life_art additionally requires numpy for original music synthesis.

Sailing temporarily places the destination root at (40,-35), enables both islands and ocean tiles, disables player physics, carries the player aboard, docks, restores root coordinates and resumes normal camera/physics. Keep inventory and escape input locked during a voyage. Never award travel or forage halfway through an animation.

Latest delivery: outputs/Wildfeast-Island-Life-Windows.zip; source archive and verification captures accompany it. Older Living-World archives remain historical.


# Development handoff

## Current version: living-world pass

The creator's feedback supersedes the first prototype's presentation and interaction choices. Read LIVING_WORLD_PASS.md and the updated README first. The existing Unity scene is still the play scene; a separate Windows build contains the revised game.

The world now has textured multicolor foliage, fantasy buildings and planted borders, ambient motion, four-direction chef frames, visitor and creature frames, visible held tools, and a quiet eight-slot HUD. The ordinary player defaults to borderless fullscreen on the first launch of this pass, including when Unity retained the earlier prototype's window preference. Pause switches fullscreen/windowed.

Fishing uses a rod at shoreline/pond geometry with a cast, line, float and landing animation. Gardening spends finite seeds, then separately waters and harvests staged crops. Cooking uses alternating cuts, heat/stirring and drag/keyboard plating with recipe-appropriate pans, pots or bowls. Brothback roams and detects proximity. Recognizable opening signs, pantry, menu board, stove, bed, letters, workshop and ferry replace ambiguous repeated props. Customers walk to numbered tables and display dish icons; Nori physically carries deliveries.

Both islands and Cloudfruit are accessible immediately. Saved upgrade IDs remain valid: `boat` is now a provision locker (+4 carrying slots); `reach` is botanical gloves (three fruit per source). Existing progress is preserved; saves missing the new seed/tool fields receive a starter Pepperbell packet. Current saves persist seed quantities, selected tools and dry/watered crops.

For artwork, run make_art.py, polish_art.py, creature_art.py in order. Scene authoring remains supported Editor C# tooling. A backup of the pre-pass scene was preserved locally before reconstructing the clean, tracked scene. The pre-existing untracked PackageManagerSettings.asset was left outside this change.

The next priority is creator playtesting of visual direction and game feel. The current art is substantially richer but does not claim the reference's finished detail. The knife has a visible swing but no extra combat effect. Cooking remains order-based with a shared three-stage structure; controller completeness, deeper recipe mechanics, obstacle-aware NPC routes, rich sound/music, broader farming/combat, and larger worlds remain future work.

## Historical initial prototype handoff

The sections below record the first prototype before the living-world pass. Travel gates, instant gardening, timing-only cooking, placed guests and art limitations described there have been superseded as stated above.

## What exists

A playable Unity two-island prototype with an integrated serialized scene at `Game/Assets/Wildfeast/Scenes/Wildfeast.unity`. The project opens in Unity 6000.3.7f1. All initial plan milestones have implementations within the small-world scope:

| Milestone | Implementation |
|---|---|
| 0 — Foundation | Actual Universal 2D template, URP camera, crisp imports, collision, input, original art, and repository conventions |
| 1 — First supper | Shore fishing/foraging, bag and pantry, menu, cooking, three guests, payment, satchel, explicit day phases and save/load |
| 2 — Ecosystem | Springs with a telegraphed Brothback hunt, Glowgrove with Lanternroot, discovery-driven recipes, and three crop plots |
| 3 — Restaurant | Terrace with two extra guests, Nori's automatic dish delivery, customer tastes/tips, four harbor requests |
| 4 — Second island | Restored skiff, Mistwake, reach-gated floating fruit, new dessert recipe, tidekeeper's letter |
| 5 — Production planning | Content budget, performance target/sample, accessibility defaults, synthesized audio, onboarding and release gate in PRODUCTION_PLAN.md |

## Launch and build

Open the existing scene and enter Play mode. The standalone build is produced separately and is not checked into source control. From the repository root, `tools/build.ps1` invokes the installed Windows Unity editor and builds into ignored `Builds/Windows/`. Supply `-UnityEditor` if the editor is elsewhere, and `-Destination` to override the output executable. `tools/test.ps1` runs the EditMode suite and writes ignored `artifacts/editmode.xml`.

The source includes all required font resources, original PNG/WAV assets, definitions and .meta files. A fresh checkout needs normal Unity package restoration/import. Its manifest and lock file record the required versions. The local Pipeline package is optional editor tooling; ordinary play does not depend on a running CLI.

## Saves

Standalone: `%USERPROFILE%/AppData/LocalLow/Wildfeast/Wildfeast - Isles of Wonder/wildfeast.json`. Editor uses `wildfeast-editor.json` in the same application's persistent-data directory. UI reset archives an old save before starting fresh. Writes use an atomic replacement and retain a previous checkpoint. Unreadable originals are copied aside before fallback. Schema version 1 rejects unsupported versions rather than guessing a migration.

Interrupted services keep cooked and paid flags, ingredient quantities, money and current orders. Reopening returns the player to the restaurant so remaining guests can be served. Exact walking position and active fishing/cooking/hunting timing are not saved; unfinished timing challenges restart without consuming ingredients or granting rewards.

The opt-in Windows diagnostic runs with `--smoke-test --save-path <isolated-save> --test-output <directory>`. It uses virtual Input System events and scripted setup positions, records offscreen URP frames, and quits with a result. Ordinary gameplay never creates a test keyboard or uses diagnostic rendering.

## Important implementation boundaries

- The player cooks for restaurant orders; free-form cooking outside service is not implemented.
- Guests are placed at tables when service begins. Arrival and departure path animation is future polish.
- Hunting is one short behavior sequence with shed-stock collection; there is no broader combat system.
- The first crop system supports discovered Pepperbell and Lanternroot with two watered nights per harvest.
- Recipe discovery is authored, rather than a general ingredient-combination solver.
- Original pixel art establishes a direction, but does not have full directional animation or production-grade creature animation.
- Island maps are authored backgrounds with serialized props/colliders and interaction points, rather than a general tilemap authoring tool.
- Physical gamepad behavior, long sessions, hardware minimums, and fresh-player usability remain to be tested.

## Next useful work

Have the creator play the current journey before expanding content. Observe navigation to fishing, recognition of plant/hunt cues, cooking instructions, guest order matching, overnight crop understanding, and motivation to invest in the restaurant. Refine those findings and the visual style, then measure production time for a polished habitat. The production plan lists the remaining demo/release gates; no release date is asserted.
