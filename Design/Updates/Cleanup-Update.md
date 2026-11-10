# Idle Restaurant v0.5.3 — people and optional cleanup

## Player behavior

Guests enter, order, collect meals, sit, eat and leave empty-handed through the exit. Articulated upper legs and knees lift before sliding onto the chair. Calves and shoes stay in front of the cushion. Existing chairs retain their colliders.

Finished meals leave a small stack of used dishes near the back of the table, leaving the dining spots free for automatic service. Stacks are visually capped at three dishes per table. Occasional crumpled tissues and fallen paper cups appear in the open floor lanes. Ignoring all dirt is supported: seats and service rewards remain automatic.

Tap a dirty plate, tissue or cup to call one janitor. The janitor walks through the doorway, visits every mess present when called, wipes tables or sweeps the floor, then exits. One completed visit pays exactly **1.5 × the network income/min displayed when the visit starts**. New messes created during that visit wait for the next visit. Cash is granted once after the janitor leaves, without multiplying meal or service rewards.

The next rewarded visit is available 90 seconds after completion. Tapping during that interval queues a visit; it starts automatically when the interval ends. Tapping during an active visit cannot create another worker or payout. One crew and interval are shared across restaurants. Active jobs, fixed rewards and queued requests are saved. Offline earnings do not grant cleaning bonuses.

## Editable content

`Assets/Restaurant/Production/Data/FamilyBalance.asset` exposes cleanup reward minutes (1.5), repeat interval (90 s), litter interval (36 s), floor litter limit (4), janitor walking speed (3 units/s), and cleaning duration per mess (1.1 s).

`RestaurantCatalog.asset` exposes guest, chef, host and janitor variant arrays. Twelve guests provide six feminine and six masculine presentations, with a range of skin colors, hair and clothing. Six chefs, six hosts and two janitors are authored as editable prefabs under `Art/People`. Appearance has no effect on income or progression.

Replace meshes and materials inside a prefab while retaining `RestaurantActorRig` references. Upper legs have child knees; keep both knee references for seated animation. The original guest/chef/host fallback slots still work. Add variants to the arrays to change casting without editing service or economy code. The People authoring menu regenerates the built-in collection; avoid regenerating over custom replacements without backing them up.

Cleanup visuals live in `RestaurantCleanupView.BuildModel`; janitor movement and saved visit state live in `RestaurantCleanupGame`; animation is in `RestaurantAnimation`. Payment logic is independent of visual appearance. Currency-only reward feedback and a soft brushing sound respect the existing sound setting. Reduced motion removes cosmetic pulsing and sweeping oscillation.

## Balance scope

Cleanup is a user-approved additional cash source, not verified Traffic Flow parity. Restaurant prices, upgrade formulas and the two-service manager invitation gap retain the preceding build's rules. Active cleanup can shorten cash-saving waits; completed-service and upgrade requirements still gate expansion. See the recorded fresh-save session for measured pacing.

## Release scope

This is a debug-signed Android test build for Android 11+ (ARM64). Live Unity verification and package checks do not establish physical-device performance or store readiness. No in-app purchases are included.
