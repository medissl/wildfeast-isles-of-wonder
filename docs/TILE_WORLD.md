> The later creator correction supersedes the generated-menu direction in this historical document. See HARBOR_REST.md: keep the established First Light art, original title background and rustic UI.

# Tile world art direction

The world is an invisible square grid, with one cell per world unit and original 32 × 32 pixel terrain tiles. Old integer garden coordinates remain tile centres. Land, trails, tidal water and cultivated soil are separate editable Unity Tilemaps. Ground classification, fishing, shore collision and tilling consume the same persisted cell plan. Each soil cell owns its full square; a shovel never paints over neighboring paving. Shore-bank cells remain protected.

## Design brief and implementation guide

Build a vivid food archipelago with readable land use. Connect arrival, restaurant door, garden, workshop and inhabited groves; connect remote discoveries through walkable forest trails. Keep clear approaches around solid objects. Use narrow dirt trails outside the village and wider harbor approaches. Fray trail edges into grass with irregular pixel clusters; avoid embossed oval slabs, outlined tile seams and isolated paths. Give each island its existing distinct size, shoreline, biome and destinations. Snap resource feet to cells, preserve resource save identities, and keep wild areas available for player gardens.

Texture ground with restrained clustered blades and tonal variation. Separate the earthy bank from shallow turquoise water, broken foam and moving horizontal wavelets. Use six original water animation frames. Render all terrain with point filtering, no mipmaps and 32 pixels per unit. Preserve directional chef actions, anchored foliage animation and gameplay cooldowns. Keep exploration and cooking connected through original food ecology.

Use the established First Light native pixel background with its original rustic title plaque and buttons. Remove marketing slogans and repeated control paragraphs from ordinary menus/HUD. Offer optional screenshot cards, one title and one short instruction per page, through the field guide in Options. Use fades around cinematic cuts and docking. Preserve five save slots, character creation, introduction, restaurant, ecology and all existing content. Build only an unpacked verification player; do not create release archives for this pass.

## Research

- [Unity 6.3 Tilemaps](https://docs.unity3d.com/6000.3/Documentation/Manual/tilemaps/tilemaps-landing.html): native grid painting, custom tiles, and tilemap collision.
- [SLYNYRD, Top Down Tiles Part 2](https://www.slynyrd.com/blog/2023/3/26/pixelblog-43-top-down-tiles-part-2): sand/water transitions and cohesive top-down pixel tile construction. The original tiles here apply those principles; no tutorial sprites are copied.

## Restored menu artwork

The generated sign and panorama were rejected and removed. The canonical menu uses the manually authored First Light title-horizon.png from e08d0de and the original rustic plaque/buttons. See HARBOR_REST.md for the current direction. Do not reproduce the abandoned generation pass.

Native terrain tools preserve the authored cell plan, positions and resource identities once tileWorld is recorded. Further layout work edits that plan deliberately, then reauthors through ArchipelagoBuilder after a scene backup. Do not run older raster terrain generators over this pass. foliage_tiles.py derives motion from canonical tree bases; it does not redesign them.
