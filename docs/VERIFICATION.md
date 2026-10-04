# Living Districts verification — 4 October 2026

Unpacked Windows player: outputs/Wildfeast-Living-Districts/Wildfeast.exe, outside Git. Evidence: outputs/living-districts-verification. No ZIP/release packaging. The original restored pixel direction is preserved; new assets are explicit native-grid drawings.

Passed: full EditMode suite 95/95 (final-editmode.xml), plus the added whole-native-map regression 1/1 (map-regression.xml). Real standalone player at 1366×768: LivingRunner 67/67 (final-living), FirstLightRunner 36/36 (final-front), sleep/energy 15 checks (rest-final). LivingRunner also passed 67/67 at 1920×1080 (living1920); its framed dialogue capture was inspected. The final full standalone gameplay journey passed 218 integrated checks (clear-retained-flow), including remote ecology ingredients, full cooking/service, all six district charts, roads, ferries, options and save/load. These cover contacts/air swings, soil/plant/water, falling trees and stump removal, walking passages/no bounce, five residents/portrait changes/events, actual pointer seed purchases, homes, three ferry destinations, clear water/dry arrival, morning checkpoint persistence, directional customization, full introduction/skip/fade and save slots. Captures inspected include title, avatar creation, fishing/cooking introduction, cultivated ground, town and framed resident dialogue.

The legacy full diagnostic initially assumed every district entrance had nearby water, then sampled a culled offscreen water tile. It now finds real water and samples it in view. It also exposed the new town chart being auto-imported as a sheet: map baking/builds now normalize whole single sprites. The old creature fixture stood inside a new solid flower border and was pushed outside collection range. It now finds an unblocked approach after activating the district, clicks the visible creature body, and requires a real pull activity. Failures remain in their isolated evidence folders for traceability; clear-retained-flow is the final successful journey run.

See LIVING_DISTRICTS_PLAYTEST.md for human checks. Scripted positioning and virtual input diagnostics are not human playtesting. Density, exploration pacing, customization readability and balance remain subjective playtest work. Residents have one ingredient event each and daily dialogue; shops currently share the seed economy, not a complete bespoke shop/catalog system. The expansion is a prototype, not a completed open-world campaign.

# Latest verification: restored First Light design and Harbor Rest — 4 October 2026

The original native art/UI at e08d0de and pre-experiment scene presentation were restored. The generated menu imagery and abandoned replacement UI are removed. The retained work is the invisible Tilemap terrain plus targeted room, energy and sleep checkpoints. The unpacked Windows player is `outputs/Wildfeast-Harbor-Rest/Wildfeast.exe`; no release archive was created.

- Unity EditMode: **86/86 passed**, zero failures (`restored-editmode.xml`). Includes energy transactions, morning crop growth, unfinished-service protection, old-save energy defaults and occupied legacy bed migration.
- Windows title/creation/intro/slot journey: **33/33 passed at 1366×768**, exit 0 (`restored-front1366/result.txt`). Captures show the original native night background, rustic plaque and buttons.
- Windows sleep/energy journey: **14 checks passed at 1920×1080**, exit 0 (`restored-rest1080/rest-result.txt`). Actual tools deplete energy, exhaustion blocks extra mining, restaurant door enters the room, sleep advances and saves a full-energy morning, Load recovers it, stairs return to dining.
- A separate player launch restored that checkpoint: **4/4 checks passed at 1920×1080**, exit 0 (`restored-final-reload1080/reload-result.txt`). Day 2, 19 shells, 100 energy, only the bedroom active.
- Full integrated journey: **202 checks passed** at 1366×768, exit 0 (`restored-final-journey1366/smoke-result.txt`), no runtime-error file. Covers native terrain/water, pointer hotbar and inventory, road protection, action cooldowns, fishing, spatial gardening, cooking/service/economy, all five continuous voyages, expanded ecology and sleep checkpoint recovery. The fixture exits the new room before travel rather than assuming an outdoor wake-up.
- Inspected real player title, outdoor, bedroom and sleep-modal captures. Canonical tree bases remain unchanged; all 20 derived frames match the reproducible one-pixel motion tool. The restored final build completed successfully.

Evidence is under `outputs/lush-life-verification/restored-*` outside Git. Earlier failed or abandoned art experiments are not final-build evidence. These automated checks do not establish human visual preference, balance, fresh-player comprehension, physical controller support, disk-full UI recovery or display FPS. A harmless Unity shutdown ComputeBuffer disposal warning remains in the full player log; no game runtime errors were logged.

---

# Latest verification: First Light — 4 October 2026

Unity EditMode passed **75/75 tests**, including moving-bar fishing, progress loss, all species, slot isolation/full slots, additive old-save defaults, complete avatar/intro round trips, glasses alignment and palette changes in action frames, atomic discovery rewards and reachable outer routes. Process exit 0.

The shipped Windows build passed **33/33 new-game checks at 1920×1080 and 33/33 at 1366×768**, each process exiting 0. Actual virtual mouse input exercises Title → Options → New → Character → Introduction → Play, another new slot, then Load of the first slot. Tests check the first slot remains byte-identical while creating another, all customization values persist, the 24-second montage completes, tools stay locked during it, physics resumes, every island casts and lands exactly one native fish, all five fish differ, and the rain kettle rewards its recipe/ingredient/water exactly once. The native captures were inspected: title, character preview, options/load, restaurant montage, Moonfen fishing and the outer orchard. No runtime error log was produced.

The existing complete Windows journey passed **188/188 checks at each resolution** on the preceding gameplay-equivalent build. The only subsequent changes were source whitespace, the introduction's open sign and fishing hint text; the shipped front-end runs cover both visual changes. Coverage retains actual hotbar/mouse actions, full fishing/cooking/service/staff/economy, body collection, plant growth, all five voyages, road rejection, dishes, settings and saves. A separate shipped process passed **3/3 saved-Pearltide identity/dry-arrival checks**. Its isolated setup explicitly changes the fixture's island to 5; the integrated journey naturally finishes on Mistwake (1). An initial setup using that Mistwake save with expected island 5 correctly failed, then the correctly prepared Pearltide fixture passed.

Evidence lives in the workspace `outputs/first-light-verification/`: shipped-front1080, shipped-front1366, release-journey1080, release-journey1366, saved-pearltide, copied editmode.xml and build.log. Earlier candidate failure directories are retained as diagnostic history, not release evidence. Initial tests caught inventory initialization before slot creation, path/tree overlap, visitor artwork child lookup, two inland fish markers and appearance eye alignment; these were corrected before the passing runs.

All game runs use isolated saves and scripted positioning/domain setup where appropriate. They are automated functional checks and offscreen captures, not human playtesting or display-FPS evidence. A previously observed URP ComputeBuffer disposal warning occurs after PASS during shutdown; no runtime exception/error log is produced by passing runs. Human fishing feel, character art, exploration density, cinematic pacing, progression balance and physical controller support still require playtesting. Five independent saves currently have no in-game delete button. This remains an expanded five-island prototype.

# Latest verification: Wonder redesign — 4 October 2026

The final Windows build passed **188 integrated checks at 1920×1080 and 188 at 1366×768**, each process exiting 0. Unity EditMode passed **62/62 tests**, 0 failures, process exit 0. A separate fresh Windows process passed **3 saved-Pearltide relaunch checks**, restoring island 5 on its actual dry landing rather than the old common origin. No runtime-errors.txt was produced by these three game runs.

Coverage includes all five voyages, distinct dry disembarkation, matching map assets, soil road/water footprint protection, sampled authored route clearance against water/solid footprints, body targeting, pangolin body clicks through actual virtual mouse input, a step before collecting settled ram cream, watering/luring, complete existing fishing/cooking/service/staff/economy flows, new recipes and disk save loading. Domain tests cover full-satchel reward preservation and repeated readiness contacts. Two former test garden positions overlapped the newly protected paving margin; their setup moved to clear farmland while retaining harvest/save assertions.

Reviewed final baked map shapes, native cast/sign preview, creature ready cues, habitat captures, maps and the fresh-process Pearltide landing. Scripted setup positioning is part of the diagnostic journey; these captures are not a claim of fresh-player comprehension or normal manual playthroughs. Scenes were authored through supported Unity Editor tooling; restaurant/UI references round-tripped. The pre-pass scene is backed up outside the repo at work/wonder-before.unity. Unrelated preexisting PackageManagerSettings.asset remains excluded.

Supported-editor shipping build: 159,747,307 bytes. Hidden-window offscreen URP samples on Intel Iris Xe: 1080p mean 1.68 ms/P95 2.07 ms/89.4 MiB allocated; 1366×768 mean 1.13 ms/P95 1.34 ms/89.1 MiB. Samples are render-request diagnostics, not display FPS or minimum-hardware claims. The preexisting shutdown ComputeBuffer disposal warning remains after PASS; no runtime exception or failing check resulted.

Deliverables: Wildfeast-Wonder-Redesign-Windows.zip, matching Unity source archive, WONDER_REDESIGN.md (research/design/execution prompt), WONDER_PLAYTEST.md, native cast/map previews and wonder-verification evidence. Package CRC and executable/managed-assembly hashes are checked. Source commit and archive checksum are recorded with release evidence.

Still needs human judgement: overall art density, fresh-player navigation/creature comprehension, long sessions, progression balance and physical controller behavior. This is an expanded prototype, not finished production art.

---

# Latest verification: five-island archipelago

Verified 2 October 2026 with Unity 6000.3.7f1, the final authored five-island scene and the exact Windows build distributed as Wildfeast-Archipelago-Windows.zip. Both Windows diagnostic processes and the Unity test process exited with code 0. Ordinary player saves were not opened or overwritten.

- **56/56 Unity EditMode tests passed**, zero failures. Original economy, save, inventory, furniture depth, font, routes and action-clock checks remain. New tests cover five free destinations, road/water planting restrictions, native foliage frames, authored island references/physical resources, seed growth, all recipe requirements, settings/seed/ecology disk persistence, older combined-volume migration, atomic watering, preserved legacy crops relocated off new roads and physical walking clearance on every authored road.
- **177 integrated checks passed at 1920×1080**, and **177 at 1366×768**, without a runtime-errors file or logged exceptions/errors. These use virtual keyboard/mouse inputs and isolated saves. They retain the full original fishing, animated forage, garden, mining spam protection, prep/cook/plate, guest service, earnings, upgrades, Nori delivery and story journey.
- The expansion journey physically sails to all five destinations; checks one arrived root, its actual M chart/player pin and protected road; exercises independent mouse-adjusted music/SFX sliders, saved camera zoom and reduced foliage motion; collects cream after quiet approach, cracks the spice shell through three timed pickaxe contacts, taps sap with the knife, gathers new seeds, waters/opens a blossom, attracts a moth with actual held Lanternroot and waters/collects Kelpsnail jelly.
- Creature collection verifies a compact ingredient moving to the player while the animal artwork stays in its habitat. Journal/recipe pagination is exercised by pointer; the gathered Spiceclaw Bisque ingredients are deposited, cooked, served and paid through the existing restaurant economy. Final island/settings save loading is checked.
- Map inspection and a sampled physical road test caught a trail crossing a marsh pool, obstructed cottage/landmark approaches and a fishing trail too close to a coastline. The delivered layouts fix these; the road test samples terrain and circle/footprint collision along every segment of every island road.
- Native captures inspected include all five baked maps, their live charts/markers, the actual 1080p and smaller-screen Options, recipe pages, inventory, chef actions, room depth, customer layout and island scenery. New generated map/art assets use 32 PPU, point filtering and no mipmaps. Scene/texture edits use public Editor APIs; the outdoor reconstruction preserves the restaurant and UI.

Hidden-window 180-frame URP render-request samples on Intel Iris Xe (including UI setup):

| Resolution | Mean | P95 | Unity allocated |
| --- | --- | --- | --- |
| 1920 × 1080 | 1.39 ms | 1.58 ms | 120.6 MiB |
| 1366 × 768 | 1.14 ms | 1.46 ms | 120.4 MiB |

These are offscreen diagnostic samples, not actual display FPS or human playtesting. Both standalone runs emit the existing ComputeBuffer disposal warning during shutdown after PASS; it is not a runtime error or a failed process.

The final supported-Editor build succeeds at 149,266,471 reported build bytes. Release packaging checks ZIP CRCs and hashes of the shipped executable/managed game assembly against the verified build. The matching source archive is exported from the committed repository; generated Library, build output, diagnostics and the preexisting untracked PackageManagerSettings.asset are excluded.

Delivery evidence is in outputs/archipelago-verification (56-test XML, both 177-check reports and native captures), alongside the Windows/source archives, SHA256 sums, updated plan/work-mode/guide and five-island preview. Earlier verification entries below describe prior releases. Physical controller support, a fresh player's comprehension, art judgement, comfortable real-time play and broader progression balance remain for human playtesting. The original three authored garden beds keep their original two seed types; all six crops use spatial plots. This is an expanded playable prototype, not a complete production-scale world.

# Latest verification: cozy polish

Verified 2 October 2026, using Unity 6000.3.7f1 and the exact final Windows build. No ordinary save was used or replaced.

- **45/45 Unity EditMode tests passed**, with zero failures. Existing economy/save/scene tests remain, plus contact/recovery spam protection, cancelled windup, timing for each ordinary tool, static font/glyph coverage, authored room footprints, all 120 chef action sprites and clear restaurant aisle routes. The visitor restart test now verifies the entrance queue rather than overlapping every guest at one exact coordinate.
- **122 integrated checks passed at 1920×1080** and **122 at 1366×768**, both Windows processes exiting successfully without logged runtime errors. Inputs use isolated virtual keyboard/mouse devices; some progression and positioning are scripted.
- The player checks actual clickable hotbar and inventory dragging, M maps, till/seed/water/refill, resource tools, one mining contact under rapid clicks, planted feet and stable equipment during the chef action, free continuous ferry voyages, real fishing, animated forage, kitchen/table collision, front/behind furniture depth, wall-hanging depth, consistent fonts, distinct tool icons, small pickup sprites, cleanup on entering the restaurant and hidden icons for guests still queued outside.
- The complete journey also checks restaurant opening by mouse, timed chopping and stirring, pointer plating, serving/payouts, requests, crop growth, natural creature charge/collection, all upgrades, Nori delivery, Mistwake story/fruit and disk save round trip.
- Captures inspected: inventory/font, pickaxe windup/contact, kitchen front/behind, dining-table footprint, service layout, expanded room, cooking/plating and the smaller-screen versions. All world art imports use 32 PPU, point filtering and no mipmaps; small held assets use native smaller canvases rather than fractional scaling of whole creatures.
- The Windows build used the repaired checked-in scene. Scene, font and sprite changes were authored through public Unity Editor APIs; the scene was preserved before repair. Font source and OFL are included.

Hidden-window 180-frame URP render-request samples on Intel Iris Xe, including UI setup:

```
Resolution: 1920 x 1080
Renderer: Intel(R) Iris(R) Xe Graphics
180-frame offscreen URP render-request sample on Mistwake, including UI setup. Hidden-window diagnostics; not a display FPS claim.
Mean frame: 1.23 ms
P95 frame: 1.49 ms
Unity allocated: 109.2 MiB

Resolution: 1366 x 768
Renderer: Intel(R) Iris(R) Xe Graphics
180-frame offscreen URP render-request sample on Mistwake, including UI setup. Hidden-window diagnostics; not a display FPS claim.
Mean frame: 1.12 ms
P95 frame: 1.53 ms
Unity allocated: 109.2 MiB
```

These samples are not display FPS or a performance benchmark. Both processes still emit Unity's previously observed ComputeBuffer disposal warning during shutdown, after successful checks; no gameplay error or exception was logged.

Human animation feel, cozy presentation, fresh-player comprehension, accessibility and physical controller support remain unverified. See COZY_POLISH_PASS.md for the playtest sequence. The pre-existing untracked PackageManagerSettings.asset is excluded from the commit and source archive.

Previous verification sections below describe historical builds.


# Current verification — island-life pass, 2 October 2026

- Unity 6000.3.7f1 compiled and built the Windows x64 delivery player successfully: **124,483,503 bytes** in the build report.
- **39 EditMode tests passed**, none failed/skipped. Existing economy, crops, service, saving and scene checks remain; new cases cover hotbar overflow, rearranged tool identity, material persistence/trading, free plots, finite water, watered-night growth, atomic field harvest, invalid actions, state round trips, different shorelines and malformed tool-stack backup recovery.
- **66 integrated checks passed at both 1920×1080 and 1366×768**, exit 0. No recorded runtime errors/exceptions. Separate diagnostic saves were used; ordinary progress files were not modified.
- Actual Input System/UI events exercise serialized hotbar clicks, rapid clicks without old-tool casting, mouse-wheel and number selection, Tab inventory, M map, dragging tools to/from the backpack, mouse-aimed till/seed/water, empty-can refill, axe/scythe/pickaxe rewards, fountain collision, ship departure/docking/return, fishing, chopping, heat/stirring, pointer plating and hunt collection. Domain setup/progression and scripted positioning are explicitly part of the harness.
- The same journey verifies opening service, all payouts, garden growth, upgrades, five-table expansion, walking Nori delivery, second-island story, fruit discovery/yield, soundtrack inclusion and save reload. It does not constitute an uninterrupted human playthrough.
- Engine-rendered captures were inspected for inventory, illustrated map at the smaller resolution, gardening, destination arrival, restaurant and cooking. The modal's pale-tab contrast and stale navigation-bar issue were corrected before the delivery build. Final cooking hides menu tabs and previous HUD notifications.
- Original asset set: **151 PNGs and 7 WAVs**, including three forty-second original music compositions. World sprites use 32 PPU, point filtering, no mipmaps/compression; fractional scaling on carried dishes, order icons, butterflies and particles was removed.
- Scene reconstruction was preceded by a backup of the clean tracked scene. Supported Editor authoring saved/reopened serialized references and baked the island maps. Source/document diffs pass whitespace checks; Unity's generated scene formatting is retained.

## Bugs addressed and evidence limits

The previous hotbar stored Editor-created runtime lambdas which did not survive scene serialization. Init now binds the actual button callbacks at runtime. UI pointer hit-testing uses the mouse's current position, avoiding old-tool activation when clicking rapidly. The first rapid-click test asserted before mouse release; that harness error was corrected, and the complete-click regression passes in both delivery runs.

New saves retain schema 1 through additive fields. Existing money, orders, ingredients, upgrades, seed counters and three legacy beds remain. The food pouch retains its portion limit (8 initially, up to 20); tools, seed packets and material stacks occupy their own slots. The first ordinary launch of this pass restores native fullscreen so diagnostic window preferences do not dictate presentation; Options then allows windowed/fullscreen choice.

The diagnostic player's shutdown still reports the existing URP ComputeBuffer disposal warning. Neither run logged game errors or exceptions. Editor licensing/cloud-configuration messages do not represent a game failure; compilation, test results and build completion were checked directly.

Concurrent hidden-window, forced-render diagnostic samples on Intel Iris Xe: 1080p mean 1.75ms / P95 2.22ms, 107.8 MiB Unity allocated; 1366×768 mean 1.70ms / P95 2.03ms, 107.5 MiB. These are **not displayed FPS or minimum-hardware measurements**.

Still requires creator/human review: control comfort, travel pacing, whether terrain/props feel integrated, restaurant comprehension, music preference, long sessions and another-machine import. Physical gamepad completeness is not claimed. Two authored islands, basic NPC routes, material trading and a visual-only knife remain the bounded prototype scope. Packaging CRC and source/remote status are checked separately at delivery.

## Historical verification below

Earlier counts and interactions refer to preceding builds.

# Verification — 2 October 2026

## Living-world pass: current evidence

- Unity 6000.3.7f1 compiled and built the revised Windows x64 player. Final build report: 113,360,335 bytes.
- **29 EditMode tests passed, none failed or skipped.** New checks cover free travel, finite seed consumption, dry planting, separate watering/harvest checkpoints, forage seed uniqueness, seed/tool persistence, legacy saves and restarting visitors across consecutive services.
- **47 integrated checks passed at 1920×1080 and 1366×768**, exit 0, no recorded runtime errors/exceptions. The current journey adds number-key equipment, mouse-wheel selection, full camera viewport, actual outdoor sign/OPEN-button service flow, separate seed and watering-can input, chopping/heat/plating, pointer dragging, proximity-triggered Brothback, walking delivery, free travel and upgraded fruit yield.
- The opt-in journey creates virtual keyboard and mouse devices. It exercises the actual Input System/UI events, while setup positions and repeated earning cycles remain scripted. It does not constitute a human playthrough.
- Offscreen Unity frames of both resolutions were inspected: village, shoreline casting, garden watering, restaurant, stove and plating. The ordinary window and welcome screen were also inspected through Windows Computer Use. Physical keyboard usability and an uninterrupted manual loop are not claimed from that inspection.
- Original artwork now contains 133 PNGs, including chef/visitor/creature animation, held tools, richer foliage/architecture, distinct interaction props, water effects and cooking visuals. Sprite imports remain point-filtered, uncompressed, without mipmaps, at 32 PPU.
- The native borderless fullscreen launch was inspected through Windows Computer Use, with no window titlebar or boxed viewport. An isolated save was used for this display check.
- The pre-pass scene was backed up before supported Editor authoring reconstructed the clean tracked scene. Scene references and font resources were reopened and checked. Source/document diffs have no whitespace errors; Unity-generated scene whitespace is retained as emitted by its serializer.

## Bugs found and corrected in this pass

- A reel input could be reused as a cast on the same frame that a catch resolved. Busy-frame input is now consumed once.
- The initial diagnostic mouse could retain an unrelated physical click. Verification now uses its own virtual mouse.
- Garden beds initially competed with the doorway prompt. They were moved clear of the entrance.
- Pixel frame borders were initially oversized in Canvas units; their border scale was corrected.
- Consecutive services now restart guests who had not finished walking out the previous night.
- Legacy tool/seed fields, separate watering, and changed upgrade effects preserve the existing save/economy.
- Replaced an unsupported checkmark glyph and corrected the final food icons to show cooked dishes rather than the raw Leafgill.

## Current performance observation and remaining checks

Concurrent hidden-window diagnostic samples on Intel Iris Xe, 180 stationary Mistwake frames with forced offscreen rendering: 1080p mean 2.14ms / P95 2.78ms; 1366×768 mean 2.72ms / P95 4.29ms; about 98.5 MiB Unity allocated. These are diagnostic intervals, **not display FPS**, and concurrent execution makes them unsuitable for a hardware claim.

Still needs creator review: whether the original visual treatment is close enough to the desired direction, manual navigation and comprehension, cooking enjoyment after repeated services, fishing feel, guest pacing, long sessions, audio, physical controller completeness, minimum hardware and another-machine import. The knife is visual only, NPC routes are simple, cooking remains order-based, and the world remains two authored islands.

## Historical first-prototype evidence

The evidence below concerns the preceding build and its superseded interactions/travel gates.

## Final evidence

- Unity 6000.3.7f1 compiled the game and built a Windows x64 Mono player successfully. Final build report: 111,868,995 bytes.
- The Editor assembled, saved, reopened, and checked the scene's gameplay references, MonoScript bindings, and font references.
- **21 EditMode tests passed, zero failed or skipped**, including the crop checkpoint regression and serialized-scene/font checks. The repository's `tools/test.ps1` was also executed successfully.
- **35 integrated checks passed in the final Windows player at 1920 × 1080 and 1366 × 768**, with exit code 0 and no logged runtime errors/exceptions. The earlier 1280 × 720 journey passed 34 checks before the final visible-five-guest check was added.
- Rendered shore, restaurant, cooking, expanded-restaurant, and Mistwake frames were captured through Unity URP render requests. Shore/cooking/restaurant frames and non-integer scaling were visually inspected. These are engine-rendered offscreen frames, not desktop screenshots.

## Integrated check coverage

Movement from virtual keyboard events through Unity Input System; three catches with exactly one reward each; daily forage uniqueness; transfer to pantry; stocked service; cooking resolution and delivery; closing; useful satchel effect; one-time request reward; two watered nights and crop yield; Brothback telegraph, charge, one bounded knockback, and cooling-phase harvest through interaction input; recipe discovery; earnings-funded upgrades; five guests and an active employee; Nori's timed delivery; boat travel; island story; reach-gated Cloudfruit/recipe; disk save round trip; runtime error detection.

The scripted journey uses test setup positions and domain calls for repeated service earnings and some progression. It is not a human walking every route, and does not establish physical keyboard/gamepad usability or player enjoyment. Ordinary gameplay does not activate the diagnostic keyboard or offscreen captures.

## Meaningful fixes found during verification

- Changed save replacement to the API available in Unity's runtime.
- Moved WorldPoint to its own correctly named MonoBehaviour source file; the initial player had failed during scene loading.
- Completed asynchronous TMP Essential Resources import before building and added a required-resource guard.
- Isolated diagnostic input from hidden-window focus and used the game's own positioning setup.
- Fixed repeated frame-based charge knockback by allowing one bounded push per charge.
- Made crop harvesting save its reward and growth reset as one checkpoint.
- Corrected title text bounds, restaurant framing, customer silhouettes, shoreline fishing placement, and offscreen UI sorting.

## Performance observation

Test renderer: Intel Iris Xe Graphics. The 180-frame stationary Mistwake sample forces URP offscreen rendering and includes UI setup; it is **not display FPS** or a minimum hardware qualification.

| Resolution | Mean interval | P95 interval | Unity allocated memory |
|---|---:|---:|---:|
| 1920 × 1080 | 1.11 ms | 1.30 ms | 95.9 MiB |
| 1366 × 768 | 0.95 ms | 1.18 ms | 95.6 MiB |

The production target remains stable 60 FPS on an agreed reference PC. Actual foreground frame pacing, extended sessions, and lower-end hardware still need profiling.

## Unverified / production gates

Fresh-player comprehension and enjoyment, a fully manual first-day journey, long-session regression, actual audio listening/mixing, physical controller navigation, key rebinding, text-size preferences, minimum hardware, and a clean import on another machine. No claim of release readiness or final art polish is made.

Machine-readable test reports and result summaries are retained with the local deliverables. Git source excludes generated logs, caches, and build files.
