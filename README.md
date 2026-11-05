# Idle Restaurant Tycoon

Playable Unity development project for portrait Android 11+ (API 30), ARM64. Unity version: **6000.3.26f1**. Production scene: `Assets/Restaurant/Production/RestaurantGame.unity`.

## Play and edit

Open the production scene and press Play. Guests automatically order, receive food and dine; each stage earns cash. Use the nine upgrade cards, grouped into Orders, Kitchen and Dining. Green BUY cards are affordable; grey NEED cards show the required cash. The map contains 40 outlets in five themed districts. Managers unlock with the second outlet. Research, licences and restaurant stars provide longer progression.

Editor shortcuts: **1/2/3** buy the visible upgrades, **M** opens the map, **Esc** goes back. Mouse and touch share the uGUI input path.

The editable balance/content catalog is `Assets/Restaurant/Production/Data/RestaurantCatalog.asset`. Art includes 40 replaceable environment prefabs, 40 signature food prefabs, original character rigs, themed map emblems, and replaceable audio clips. See [asset replacement and editor tools](Design/AssetReplacementAndTools.html).

## Evidence and limits

- [Acceptance and parity register](Design/AcceptanceAndParity.json): verified, reconstructed, proposed and unknown behavior.
- [Human-readable specification](Design/ProductionSpecification.html).
- [Core checks](Verification/CoreChecks.json): 17 groups passed, including 21,600 exact price comparisons and 279 exact effect comparisons against independently generated Java fixtures.
- [Actual UI-path checks](Verification/PlayUI.json): rendered GraphicRaycaster purchases, map navigation, settings and demo checkout. This is Editor evidence; physical Android input is not yet verified.
- [Full progression simulation](Verification/ProgressionSimulation.csv) and [pacing comparison](Verification/PacingComparison.json): conditional analytical estimates, not measured completion times.
- `Design/Source/` retains the supplied GDD and balance workbook without edits.

The customer timing/path model is reconstructed. Exact Traffic Flow playtime parity is **not certified**. Source upgrade prices, raw effects, capacities, fares and stage weights are preserved and tested; overall progression still needs reference-session and physical-device validation.

## Demo monetization

The shop uses explicit `demo.*` product IDs, as requested. Demo confirmations grant local test entitlements/gems without charges. Rewarded-ad tests are also visibly labelled. Test grants compile only in the Editor and development builds. Unity Purchasing 5.4.4 and LevelPlay 9.6.0 are installed; live billing, ads, consent, receipt validation and store configuration are not connected. Local development analytics write a bounded diagnostic log; no analytics backend is configured.

## Build and authoring tools

Use **Restaurant → Production → Content and Economy Workbench** for catalog inspection, progression simulation and development controls. Use **Build Android Development APK** to create `Builds/IdleRestaurant-Android.apk`. This is a debug-signed testing build, not a signed store bundle.

Art bake commands regenerate the supplied procedural assets; commit custom artist work before using them. The source catalog importer replaces numerical content definitions while resolving existing art slots.

## Recording and release status

See [the development handoff](Design/DevelopmentHandoff.html) for the built APK, verification results and precise remaining gaps.

`Media/IdleRestaurant_Playable_15s.mp4` is reserved for the running-game recording. Recording verification is documented in `Verification/`. The earlier `Media/Idle_Restaurant_Tycoon_Concept_15s.mp4` and `Assets/Restaurant/RestaurantConcept.unity` remain separate scripted concept artifacts.

Release acceptance remains incomplete: target-phone performance/input, exact timing parity, remaining manager/offer behavior, publisher services, app/store presentation and release signing require further validation or configuration. See the parity register for precise gaps. Do not describe this development build as a completed store-ready game.

Target repository: `mahnul-playnoob/idle-restaurant`, branch `main`.
