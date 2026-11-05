# Idle Restaurant Tycoon — free family edition

Unity **6000.3.26f1**, portrait Android **11+**, ARM64. Active scene: `Assets/Restaurant/Production/RestaurantGame.unity`. Version **0.3.0**.

This edition has **no purchases or ads**. Guests queue, order, collect food, reserve chairs, eat and return their plates. Earn coins for upgrades and new restaurants; earn tickets for teammates and useful skills. A short guided first service introduces one step at a time. There is no energy meter, guest patience penalty or daily service-ticket limit.

The game includes 40 restaurants in five themed districts, 71 collectible teammates, research, offline earnings, repeatable restaurant stars and rolling goal books. The 40 environments are finite authored content; continued play repeats and develops those restaurants.

See [the current game guide and balance](Design/FamilyGameGuide.html). The free family balance intentionally differs from Traffic Flow. Historical source files and exact legacy arithmetic fixtures are retained separately, and do not certify active-game parity.

## Play and edit

Open the production scene and press Play. Green BUY cards are affordable. Grey NEED cards need more coins; SOON cards unlock through onboarding. The guide can be skipped and Help remains available. Mouse/keyboard testing: 1/2/3 buy visible upgrades, M opens the map, Esc goes back.

- `Assets/Restaurant/Production/Data/FamilyBalance.asset`: active prices, growth, rewards, timers, service and offline tuning.
- `Assets/Restaurant/Production/Data/RestaurantCatalog.asset`: outlets, food/environment slots, teammates, research and audio.
- [Asset replacement and editor tools](Design/AssetReplacementAndTools.html).
- **Restaurant → Family Game**: configuration, economy forecast and actual gameplay tests.
- **Restaurant → Production → Build Android APK** creates `Builds/IdleRestaurant-Android.apk`, a normal, debug-signed installable test build without development diagnostics.

## Verification

Current reports: `Verification/FamilyPlay.json`, `FamilyBusyServices.json`, `FamilyEconomy.json` and `FamilyProgressionForecast.json`. The forecast is analytical, not measured total playtime. The current APK is 0.3.0/code 3, normal non-development, debug-signed for testing; signature and absence of billing/advertising-ID permission are verified. The current APK is 0.3.0/code 3, normal non-development, debug-signed for testing; signature and absence of billing/advertising-ID permission are verified. The current APK is 0.3.0/code 3, normal non-development, debug-signed for testing; signature and absence of billing/advertising-ID permission are verified. Historical `CoreChecks.json` checks the retained reference model separately.

Physical-phone performance/input and child usability testing remain necessary. This is a runnable testing candidate, not a claim of a store-approved or child-tested release. See [the current handoff](Design/DevelopmentHandoff.html) for exact artifact and validation status.

Repository: `mahnul-playnoob/idle-restaurant`, branch `main`.
