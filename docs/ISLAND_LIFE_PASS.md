# Island-life and interface pass — 2 October 2026

The creator's latest feedback supersedes fixed journal/bag hotbar buttons and instant travel. Preserve the existing creature designs, fishing and three-stage cooking while building the everyday loop around tools, item placement and spatial interactions.

## Implemented

- Ten clickable item hotbar slots (1–0/scroll), backed by thirty backpack slots. Tab opens the grid; drag or click two slots to exchange items. Gathered stacks occupy the first free hotbar slot before overflowing into the backpack. Tools moved between slots keep their behavior. Missions and settings are menu pages.
- Inventory, Map, Recipes, Journal, Requests and Options tabs in a parchment/burgundy/gold interface. Recipe cards show ingredient icons, pantry quantities and selling prices. M opens the current island map, with scenery, player marker and important locations. Options includes volume, mute, display and challenge settings.
- Mouse aiming within two world units for tools; left click/Space uses equipment, right click/E interacts. Existing keyboard movement and cooking alternatives remain.
- The shovel creates one-unit soil tiles on clear land on either island. Seeds consume finite packets and appear on the ground. The can holds twenty water units and refills beside water. Watered crops grow overnight and are harvested after two watered nights. New plots require new seeds after harvest; legacy garden beds retain their previous regrowth behavior. Up to 256 plots persist in this slice.
- Axe: three hits harvest Cinnamonwood; pickaxe: three hits harvest Saltstone; scythe: a broad sweep harvests Noodlegrass fiber. These enter item slots and remain with the player when food is deposited. The workshop trades materials for shells. Sources recover each morning in this fantasy prototype.
- Forage visibly tugs, lifts and arcs toward the player before awarding ingredients. Esc cancels without an award. Harvested plants disappear until tomorrow instead of leaving faded copies.
- The skiff carries the player through a continuous sea route. Both physical island roots are visible in temporary sea coordinates, then the destination is rebased after docking. No purchase gates. Mistwake has a different coastline, trails, terraces, architecture and resource layout.
- Footprint collision for trees, fountains, benches, solid buildings, kitchen fixtures, tables, lamps, mailboxes and mineral nodes. Walkable flowers, grass and arches remain open. Trees avoid water and Iona's house footprint.
- Restaurant tables have original detailed place settings, clear numbering and wider aisles. Kitchen props, herbs, plants, steam, walking guests and Nori remain integrated.
- Original forty-second music compositions for Saltleaf, Mistwake and the restaurant, synthesized locally without samples or third-party music. Interaction sound effects accompany tool impacts and watering.
- World sprite imports remain 32 PPU, point-filtered, uncompressed and without mipmaps. Fractional scaling was removed from held dishes, visitor order icons, particles and butterflies. Differently sized objects use different native canvas dimensions, with the same physical pixel density.

## Controls to review

WASD/arrows walk; click a hotbar slot, 1–0 or scroll selects items; left click/Space uses tools; right click/E interacts; Tab opens inventory; M opens the map; Esc closes or cancels. Cooking remains alternating cuts, heat/stirring and drag-to-plate.

Try the new soil flow beside home: select shovel, click clear ground within reach, select Pepperbell seeds, click the same tile, select the can, click it again. Refill beside shore or pond. The initial hotbar contains the rod, can, seeds, knife, shovel, axe, scythe and pickaxe; empty spaces receive discoveries. Rearrange this layout freely in inventory.

## Limits

This is a two-island prototype. The resource economy is material trading, not a crafting tree. The field knife remains visual; Brothback collection still follows its charge/cooldown behavior. NPC movement uses authored routes rather than general navigation. Music is original synthesized loop music, not a recorded orchestral score. Keyboard/mouse is the verified target; physical gamepad completeness and human comprehension remain playtest work.
