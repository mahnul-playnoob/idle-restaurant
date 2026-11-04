# Idle Restaurant Tycoon

A cozy 3D restaurant concept built around the Traffic Flow 0.8.1 progression baseline.

## Deliverables

- `Design/Idle_Restaurant_Tycoon_Plan.html`: complete design plan, economy mapping, food progression, animation and sound direction, validation criteria and production sequence.
- `Design/RestaurantBalanceBaseline.json`: all 40 proposed restaurants mapped to source route values, including the starter override rule.
- `Design/Source/`: the supplied reference GDD and balance workbook, retained without edits.
- `Media/Idle_Restaurant_Tycoon_Concept_15s.mp4`: rendered portrait concept film with original synchronized sound.
- `Media/SoundDesign.json`: timestamped sound cues and measured audio levels.
- `Assets/Restaurant/RestaurantConcept.unity`: editable scene entry point.

## Demo scope

This is a scripted 3D concept presentation, not the playable restaurant game. The customer, chef and server motions, food assembly, coin collection, upgrade selection and recipe reveal follow a 15-second timeline. UI cash and rewards are illustrative. Preview timing is accelerated. The production economy, player input, saving, managers, research, prestige, ads and billing have not been implemented here. No phone performance or playtime parity is claimed.

Models, animation code, UI and synthesized audio are original. Reference screenshots and original game assets are not included. The scene uses Unity-provided TextMeshPro resources with their included licences.

## Preview in Unity

Use Unity **6000.3.26f1** and open `Assets/Restaurant/RestaurantConcept.unity`. Play mode creates the procedural restaurant and loops the animated scene with its soundtrack. Use the `Restaurant/Prepare Concept Film` menu to regenerate the scene entry point. Dimensions, colours, character poses and recipe geometry are editable in the scripts under `Assets/Restaurant/Runtime`.

To render frames, run `RestaurantPreview.RestaurantFilm.Instance.StartCoroutine(RestaurantPreview.RestaurantFilm.Instance.Record("<absolute output directory>"))` through the editor, after entering Play mode. The recorder writes 450 frames at 30 fps. `Tools/EncodeFrames.swift` produces the silent MP4 and `Tools/AddSoundtrack.swift` combines it with `Assets/Restaurant/Resources/RestaurantSoundtrack.wav`. The audio generation script requires Python and NumPy. Intermediate frame sequences are excluded from Git.

Target repository: `mahnul-playnoob/idle-restaurant`, branch `main`.
