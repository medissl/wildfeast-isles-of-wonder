# Wonder redesign — analysis and execution brief

## What went wrong

The previous five maps repeated the same central fork, southwest pier, oval pool and tree bands inside the same 40×26 envelope. Palette swaps did not create new places. Uniform wide smooth paths, noisy oval tree canopies, giant isolated flowers and empty clearings weakened scale and composition. Four food creatures shared a rounded body/leg template. The chef and visitors had box-shaped heads and little identity. Building and service signs used an unmeasured default font, spilling beyond their boards.

Collection depended on hidden readiness and distance to a sprite's bottom pivot. Clicking a visible torso could miss the interaction origin. Rams immediately lost calm when the player took a step to collect; Brothback's reward window expired while the player approached. Tilling checked a centre point, not the visible soil footprint. Scripted tests standing at exact pivots missed these usability failures.

## Reference study (4 October 2026)

- Stardew Valley official media: https://www.stardewvalley.net/media/ — readable settlement routes, crop space, boundaries and points of interest. Adopt functional land use and clear entrances.
- Eastward publisher media: https://chucklefish.org/blog/eastward-launches-today-on-xbox-game-pass/ — layered inhabited places and distinctive people. Adopt coherent clusters, shade, small local stories and occupied edges.
- Eastward combat design discussion: https://chucklefish.org/blog/cast-iron-combat/ — interaction rhythm needs different spatial decisions. Adopt visible creature responses instead of another activation panel.
- Sun Haven official screenshots: https://store.steampowered.com/app/1432860/Sun_Haven/ — fantasy habitats and varied creature scale/silhouettes. Adopt an original food ecosystem with recognizable animal anatomy.

These are composition and interaction references only. Do not copy or download their assets. This pass uses project-native original pixel authoring, 32 PPU, point sampling, no compressed world textures.

## Island composition

1. Saltleaf: compact harbor settlement, cobbled square, planted kitchen garden, looping woodland trail and spring. Preserve the home entrances and starter garden coordinates.
2. Mistwake: tall stepped orchard, winding hillside route, cottage court, meadow with custard sheep. A narrow, vertical island instead of another horizontal clearing.
3. Emberfold: broad broken volcanic shelf, switchback quarry, cinnamon grove and warm-water inlet. Ochre stone paving and mineral seams; long horizontal exploration.
4. Moonfen: tall indented wetland, separate pools, timber marsh paths, mushroom enclosure and pollen sanctuary. Vegetation frames passageways without obstructing travel.
5. Pearltide: wide crescent around a large lagoon, curved coral orchard route and southeast jetty. Reef arcs and shore habitat give the water a structural role.

Use different actual terrain sizes, coast polygons, dock positions, pool placement and route graphs. Maps, camera bounds, ferry docking, physics and tilling must consume the same definitions. All roads remain traversable; a soil tile's entire visible square must avoid paving and water. Tree roots/grass belong to habitat edges; do not fill every open area with noise.

## Cast and ecology

- Brothback: stock-pot wild boar, four hooves, tusks, spout and steam. Telegraph a charge, then retain collectable stock until collected or the player leaves.
- Custardram: horned custard sheep, fluffy scalloped fleece, hoofed legs and cream muzzle. Quiet proximity settles it; grace period permits approaching to collect.
- Spicepangolin: low plated cinnamon animal with curled tail and spice plates. Three pickaxe contacts loosen its shed spice; preserve the legacy spiceclaw/source IDs.
- Mochimoth: airborne winged dumpling insect, antennae and luminous wing eyes. Lanternroot attracts it; readiness persists briefly after an equipment change.
- Kelpsnail: elongated soft body, visibly spiral kelp shell, eye stalks. Water unfurls the body and reveals jelly.

No common recolored body function. Each sprite has a different outline, pose, animation and readable ready state. Put a compact ingredient cue over ready creatures; right-click their body or E within reach to collect. Explain a full satchel explicitly without losing the reward. Retain daily regeneration and save compatibility.

Chef: expressive face, layered hair, soft chef hat, teal jacket, red neckerchief, apron and boots. Draw four-direction walk and full-body action frames in the same native scale. Visitors/staff each get distinct headwear, hairstyle, outfit and face; retain aisle navigation and carried dishes.

## Work-mode execution prompt

Act as Wildfeast's game developer and art director. Implement the above design in the existing Unity project. First repair body targeting, stable readiness and whole-tile road protection. Then author five different landforms/routes and original sprites, integrate them with supported Unity Editor tooling, and correct measured sign typography. Preserve .meta files, restaurant work, inventory/economy, music/settings, saves and nonlethal food collection. Back up the scene before outdoor reconstruction. Keep IDs stable. Test real body clicks, movement before collection, repeated input, satchel-full recovery, water/road edges, docks and map pins, all five voyages, cooking/service, save loading and two screen sizes. Inspect actual captured frames. Record evidence and remaining limitations. Package the exact tested build and source, then commit and push the reviewed changes. Do not call automated checks human playtesting or claim finished production art.

## Acceptance

- Different island dimensions and unmistakable grayscale silhouettes; coherent walkable routes and visible habitat landmarks.
- Every creature obtainable from ordinary E/body-click use, with original art and informative cues.
- No visible soil overlaps protected road/water; nearby clear ground still farms.
- Signs fully inside their frames; all characters and animations share the pixel grid.
- Relevant Unity tests and isolated Windows journey pass; visual captures reviewed at both resolutions.
