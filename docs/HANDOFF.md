# Development handoff

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
