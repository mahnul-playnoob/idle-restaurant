> Version 0.5.1: centred Street Kitchen shades, lighting at all three counters, six night-only table candles and earlier earned starter access. Readable growth targets and Map/Team readiness markers guide the longer journey. [Fresh-save playtest and editing notes](Design/Updates/WarmServiceAndFreshSession.html).

> Version 0.5: longer earned progression, one-at-a-time managers with portraits, distinct outlet icons, straight-on restaurants, service-driven day/night lighting, improved animations and visible upgrades. [Balance, editing and validation](Design/Updates/ProgressionAndAtmosphere.html).

> Version 0.4.0: five progressive district chapters, themed vector icons, distinct scenery and varied repeatable goal books. [Changes, editing instructions and live verification](Design/Updates/DistrictJourneyAndGoals.html). Existing progressed goal books finish before switching to varied goals.

> Version 0.3.2 experience pass: contextual guest tutorial, non-blocking spotlight, larger playfield and food, green upgrade feedback and automatic service rewards. [Changes and 24 live checks](Design/Updates/ExperiencePolish.html). Historical screenshots and reports below describe their respective earlier builds.

# Idle Restaurant Tycoon — free family edition

Unity **6000.3.26f1**, portrait Android **11+**, ARM64. Active scene: `Assets/Restaurant/Production/RestaurantGame.unity`. Version **0.3.1**.

This edition has **no purchases or ads**. Guests queue, order, collect food, reserve chairs, eat and return their plates. Earn coins for upgrades and new restaurants; earn tickets for teammates and useful skills. A short guided first service introduces one step at a time. There is no energy meter, guest patience penalty or daily service-ticket limit.

The game includes 40 restaurants in five themed districts, 71 collectible teammates, research, offline earnings, repeatable restaurant stars and rolling goal books. The 40 environments are finite authored content; continued play repeats and develops those restaurants.

See [the current game guide and balance](Design/FamilyGameGuide.html). The free family balance intentionally differs from Traffic Flow. Historical source files and exact legacy arithmetic fixtures are retained separately, and do not certify active-game parity.

## Play and edit

Open the production scene and press Play. Green BUY cards are affordable, with a separate darker price bar. Grey price cards need more coins; SOON cards unlock through onboarding. Yellow guidance dismisses between completed steps and reappears for the next target. The guide can be skipped and Help remains available. Mouse/keyboard testing: 1/2/3 buy visible upgrades, M opens the map, Esc goes back.

- `Assets/Restaurant/Production/Data/FamilyBalance.asset`: active prices, growth, rewards, timers, service and offline tuning.
- `Assets/Restaurant/Production/Data/RestaurantCatalog.asset`: outlets, food/environment slots, teammates, research and audio.
- [Asset replacement and editor tools](Design/AssetReplacementAndTools.html).
- **Restaurant → Family Game**: configuration, economy forecast and actual gameplay tests.
- **Restaurant → Production → Build Android APK** creates `Builds/IdleRestaurant-Android.apk`, a normal, debug-signed installable test build without development diagnostics.

## Verification

Current reports: `Verification/FamilyPlay.json`, `FamilyBusyServices.json`, `FamilyEconomy.json` and `FamilyProgressionForecast.json`. The forecast is analytical, not measured total playtime. Historical `CoreChecks.json` checks the retained reference model separately.

Physical-phone performance/input and child usability testing remain necessary. This is a runnable testing candidate, not a claim of a store-approved or child-tested release. See [the current handoff](Design/DevelopmentHandoff.html) for exact artifact and validation status.

Repository: `mahnul-playnoob/idle-restaurant`, branch `main`.

## Presentation patch 0.3.1

Visible ENTER/EXIT gateways align with customer paths. Customers return their plate before leaving, and the waiting queue stays within its lane. The cream/herb UI uses green affordable upgrades, grey unavailable cards, separate price strips and yellow tutorial targets. Navigation now reflects the active page.

[Implementation and backlog mapping](Design/Updates/PresentationAndGuestFlow.html) · [Live verification](Verification/Presentation/PresentationVerification.json). All 14 Editor checks passed, including the six-guest tutorial and a 22-guest service. The new APK is `Builds/IdleRestaurant-Android-v0.3.1.apk` (also copied to the latest APK filename), version 0.3.1/code 4. ZIP integrity and signature v2 are verified; it is debug-signed for testing. Physical Android touch/performance remains unverified. Original save files were restored after validation.
