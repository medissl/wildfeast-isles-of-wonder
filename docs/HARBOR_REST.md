# Harbor Rest: preserve the established design

The creator explicitly rejected the generated-image pass and then asked to return to the already completed design. Original First Light art at e08d0de is the reference, not the abandoned replacement direction. Building, furniture, character, creature, icon, title background and rustic UI sprites are restored from that revision. The pre-experiment scene snapshot restored the existing restaurant and Canvas before authoring the targeted additions. No image generation is used by the current art pipeline.

## Implementation brief

Improve around the existing native pixel art and licensed Pixelify font. Keep the original palette, creature silhouettes, restaurant, title background, and UI frames/buttons/slots. Preserve the invisible one-unit grid, 32 PPU, point filtering, no mipmaps, connected dirt trails and authoritative land/road/water cells. Original tree bases retain their design with anchored one-pixel animation; grass has native 28x16 frames. The ordinary title omits the rejected marketing lines. Menus retain brief labels, optional picture guides, responsive mouse input and established keyboard controls. OriginalPresentation binds the original assets into the existing Canvas using supported Editor APIs. Avoid rebuilding the entire art direction.

The three raised garden boxes are removed. Existing occupied legacy beds migrate into ordinary field cells, retaining item, growth and watering day. Native Tilemaps display each soil cell independently and never modify roads.

## Sleep and saves

The back-right door in the restaurant enters a separate upstairs bedroom. Its bed offers Sleep and save; stairs return to dining. Room walls and furniture have footprints and depth sorting. Sleep fades out, stages the next morning separately, writes it atomically, then applies that model and wakes the player at home. A disk-write failure preserves the active day. Sleep restores 100 energy, advances watered crops and refreshes wild resources. Finish all guests before resting.

The right-side meter shows energy out of 100. An animated world tool attempt costs 2, forage 3, a valid shoreline cast 6 and a cooking session up to 5. Existing cooldowns prevent repeated clicks charging multiple times during one action. Walking, menus, travel and serving are free. Exhaustion blocks world tools; cooking can finish at zero energy to prevent service trapping the player. Values are initial tuning and need human playtesting.

Active progress checkpoints on sleep. Quitting or returning to title resumes the last sleep checkpoint. New slots have an initial checkpoint; intro completion writes only its flag on that checkpoint; options writes copy only preferences onto the existing checkpoint.

## Verification

Use isolated saves for Unity EditMode, --smoke-test --rest-test, full --smoke-test and --frontend-test. Inspect actual player captures at desktop resolutions. Build an unpacked Windows player only. Automated checks prove covered behavior, not visual preference, gameplay balance or display FPS.
