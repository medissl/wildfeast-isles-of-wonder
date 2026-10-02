# Living world and tool interaction pass

The creator found the initial prototype lifeless, visually plain, difficult to read, too small on screen, and dominated by identical interaction boxes. Their reference is a densely planted fantasy village with colorful trees, distinct buildings, organized paths and gardens, and water. This pass uses those broad qualities for original assets without copying reference sprites or branding.

## Changes

- Borderless fullscreen default and a pause-menu window toggle. The URP camera fills its viewport, retaining point-filtered sprites and pixel snapping. Aspect ratios reveal more world instead of distorting it.
- An eight-slot belt: hands, rod, can, two seed packets, knife, journal, bag. Number keys, scrolling, and clicking select tools. Tools appear in the chef's hands; four-direction walk frames replace the one-direction character.
- Quiet exploration HUD: purse/day, one short objective, contextual prompt, tool belt. Removed the permanent logo card, objective paragraph card, controls card, and three large navigation buttons.
- Original layered foliage, purple/gold/teal trees, planted borders, mushroom architecture, glowing windows, arch, fountain, lanterns, herbs, distinct props, and textured paths. Ripple frames, fish shadows, butterflies, foliage sway, wandering villagers, crop stages, and floating fruit provide motion.
- Fishing from actual shoreline/pond geometry. Cast animation, rod/line/float, splash, bite, tension play in a small HUD, and an arcing catch animation. No visible fish object must be activated to start.
- Brothback roams and detects proximity; warning, charge and cooling are natural creature states. It has four movement frames and one bounded push per charge. Cooling yields stock. The knife currently has a visible swing but no additional combat damage system.
- Real seed quantities: starter Pepperbell packet, seed rewards from forage, planting consumption, dry seed, watering can, wet soil, sprout, mature crop, harvest. Growth requires two watered nights. Untended plants pause growth.
- Cooking is chopping (alternate cuts), stove management (heat and stirring), then plating (drag/place ingredients). Pans, soup pots and whisking bowls differ by recipe. Quality follows heat control and stirring. Seasoning/herb garnish art is presentation; recipe inventory costs are unchanged. Cooking remains order-based.
- Recognizable pantry, menu board, OPEN/CLOSED sign, stove, bed and letterbox. The outdoor sign offers a direct way to open service and enters the restaurant. Stocked recovery porridge is added when the selected menu is completely unavailable.
- Guests enter from the doorway, walk to numbered tables, display their dish orders, and walk out after serving. A held dish must match the guest. Nori walks over with a visible dish before delivery.
- Free ferry destination selection in both directions. Mistwake and its fruit are available immediately. Old `boat` and `reach` upgrade IDs are retained for save compatibility, now provision capacity and botanical yield improvements.
- Existing schema-1 saves are retained. Missing tool/seed fields receive a starter packet without discarding progress. New seed, tool and watering states persist.

## Next human review

Judge world composition, walk readability, casting feel, crop cues, the clarity of the opening sign and stove, cooking enjoyment across several services, customer pacing, and screen framing. The art is a significant original development pass, not a claim to match the reference's finished hand-authored detail.

Still outside this pass: full combat, a useful knife progression system, open-world streaming, additional islands, deep farming, customer personality/dialogue, free-form cooking, distinct recipe mechanics beyond the shared prep/cook/plate sequence, rich audio/music, key rebinding, and complete controller support. Walking staff and guest routes are simple authored routes rather than obstacle-aware navigation.
