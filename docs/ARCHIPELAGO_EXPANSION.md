Current redesign: see WONDER_REDESIGN.md and WONDER_PLAYTEST.md. Map sizes, docking positions, cast and whole-tile road protection now supersede the earlier layouts below.

# Wildfeast: the five-island expansion

The creator requested a larger, deliberately designed food world after the cozy polish pass. This release replaces the two outdoor layouts with **five actual explorable islands**, while preserving the restaurant, cooking stages, item inventory, chef animations and economy. Each island is 40 Ã— 26 world units; this is an expanded playable prototype, with room for further environmental storytelling and bespoke art.

| Island | Layout and food ecology | New discoveries |
| --- | --- | --- |
| Saltleaf Shore | A dock road leads to the village, restaurant, workshop and kitchen garden. A west trail enters Glowgrove; an east branch reaches the Steamstone habitat and fishing bank. | Existing Leafgill, Pepperbell, Lanternroot and Brothback remain. |
| Mistwake Isle | Orchard terraces, a tidekeeper cottage, custard meadow, a small western pool and a separate coastal fishing approach. | Bubblecarp, Custardpetal and Custardram cream. |
| Emberfold Cay | Toasted groves, a mineral quarry, warm pools, spice terraces and a sap stand. | Emberbulb, Lavafin, Spiceclaw spice and Cinnamon sap. |
| Moonfen Hollow | Mushroom groves, a pollen glade and multiple marsh pockets, with clear routes around the water. | Mooncap, Jellyray, Mochimoth pollen and Dew nectar. |
| Pearltide Atoll | Coral-colored groves around a central pool, pearlroot beds, a kelp habitat and reef fishing bank. | Pearlsprout and Kelp jelly; Jellyray and Cloudfruit also live here. |

All five destinations are available from the skiff without upgrades or purchases. Sailing physically moves the ship and player away from the departure island and into the destination dock before returning to local coordinates. The voyage takes about ten seconds. The maps are separate authored scenes within the play scene, rather than alternate names for one layout.

## Ground, vegetation and collision

The same `Resources/Archipelago.json` defines coastlines, pools, roads, habitat regions, resource placements and landmarks for art generation and gameplay. Roads are visibly edged packed trails; they cannot be tilled. Clear green ground can be tilled on every island. Water, tree trunks, mineral outcrops, furniture and solid landmarks reject shovel use. A planted tile must fit beside the protected road edge.

Trees have detailed native 64 Ã— 96 foliage frames, with a one-pixel canopy change and stationary trunks/roots. Low 24 Ã— 16 ground cover grows in clusters; its small frame changes keep it anchored. Stone patches sit at biome-specific quarry edges. Native silhouettes remain at 32 pixels per world unit, with point filtering and no mipmaps. Roads connect actual destinations, and their junctions have continuous interiors. Ground-level habitat flowers, marsh reeds, coral growth and terrace edging belong to the terrain. The restaurant's existing furniture and aisles are preserved.

Creature, sap and nectar collection moves a compact ingredient into the player’s hands while leaving the animal/tree/bloom rooted in its habitat. Ordinary forage retains its root-pulling animation. Ground creatures and the sap tree have physical footprints. The hovering Mochimoth can fly over the player. Food creatures remain animated after shedding an ingredient and recover their harvest the next morning.

## Play the new interactions

- **Custardram:** enter Mistwake's meadow, stand still nearby for two seconds, then press E or right-click to collect cream. Footsteps startle it.
- **Spicepangolin:** use the pickaxe on the Emberfold creature three times. Each contact follows the chef's windup/recovery animation. The cracked shell becomes visible; collect its shed spice with E.
- **Mochimoth:** place a gathered Lanternroot ingredient on your hotbar and hold it. Approach the Moonfen moth; it follows the light. Collect pollen when the hint says it is ready.
- **Kelpsnail:** use the watering can on the Pearltide creature, spending one unit of water. Collect the jelly it sheds.
- **Dewblossom:** water Moonfen's closed bud, then collect nectar from its visibly opened flower.
- **Cinnamon sap:** use the field knife at Emberfold's tap tree. The harvest uses the existing action and pull animations.

Shell contacts and watered readiness persist in the save. Actions apply once through domain transactions; repeated contact cannot spend extra water on an already opened source. Wild harvests recover overnight.

## Ingredients, farming and the restaurant

There are **19 ingredients, 20 dishes, five food-creature species and six growable crops**. Four new seed packets join Pepperbell and Lanternroot: Custardpetal, Emberbulb, Mooncap and Pearlsprout. Gathering their wild sources yields seeds. Put a packet on the hotbar, shovel clear ground, plant, water and grow it over two watered nights. Seed stacks, plots and inventory placement survive loading.

New dishes include Bubblecarp Dumplings, Cloud Cream Pavlova, Spiceclaw Bisque, Cinnamon Sap Pancakes, Mooncap Risotto, Mochi Mooncakes, Kelp Jelly Parfait and the three-island Archipelago Hotpot. Each has actual ingredient requirements, discovery unlocks, a distinct dish icon, a compact carried serving and a service payout. Recipes unlock when their ingredients are discovered; the original starter recipes and original three discovery unlocks remain compatible. The satchel and provision-locker upgrades still increase food portion capacity.

Recipe and journal books use five entries per page with mouse-operable Previous/Next buttons. M shows the current island's baked chart, your position and its important destinations. The ferry lists all five ecosystems. The hotbar remains items and tools.

## Options and sound

Options provides separate **music** and **sound-effect sliders**, mute, three world zoom steps and reduced ambient motion. Zoom keeps the HUD size and native world pixel grid. A second Options page provides relaxed/standard challenge, fullscreen/windowed display, a control guide, an archived new journey and Save/Quit. Audio, zoom, motion and challenge settings save immediately. Muting preserves the chosen slider values.

Each island has an original seamless synthesized music composition; the restaurant has a sixth theme. The three new pieces use no recordings or sampled songs. Pixelify Sans and the existing pixel widgets remain consistent throughout the UI.

## Older progress and reproducibility

Area 2 remains the restaurant; islands use IDs 0, 1, 3, 4 and 5. This retains existing island and scene meanings. Older combined-volume saves migrate into both audio sliders. Older crops that now overlap a redesigned road or pool move to nearby valid ground with their item, growth and watering day intact. Money, upgrades, pantry, discoveries and seed quantities are retained. Diagnostic runs always use separate saves.

Run the original art scripts in their documented order, then `tools/archipelago_art.py` last. Run `tools/archipelago_music.py` for the three new music loops. `ArchipelagoBuilder.Author` replaces outdoor island roots and rebakes all five maps, preserving the authored restaurant and UI. Back up manual scene work before this deliberate outdoor reconstruction. `ProjectBuilder.Assemble` remains the complete bootstrap. The prior scene is preserved outside the repository as `work/fifth-pass-scene-backup.unity`.

## What to test by hand

1. Walk from the Saltleaf dock to the restaurant, garden, workshop and spring using the paths. Try walking into trees, rocks and the fountain; check front/behind depth. Inspect the low grass and quiet tree animation.
2. Shovel a visible road, then a clear green tile beside it. Only the green tile should become soil. Plant and water a seed. Refill the can at a pool or shore.
3. Sail to all four other islands. Watch the boat depart, cross the sea and park. Walk each route and open M; the chart and player pin should match your island.
4. Try all six interactions above. Leave and return to a watered blossom or partially cracked shell; save/relaunch and verify that progress remains.
5. Grow a newly discovered crop, bring new ingredients home, page through the recipe book, put a stocked new dish on the menu and serve it using prep/cook/plate.
6. Drag each audio slider independently. Change zoom and reduced motion, then reopen the game. Check the smaller window, inventory dragging and page buttons for readability.

Automated test evidence and release paths are recorded in `VERIFICATION.md`. Automated inputs do not replace a fresh player's judgement of map readability, art quality, balance or comfort.
