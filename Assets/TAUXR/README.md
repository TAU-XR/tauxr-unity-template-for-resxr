# Assets/TAUXR

Studio code and assets that are shared by all TAUXR projects. Everything that is ours (not ResXR's) lives here,
so `Assets/ResXR` stays as close as possible to upstream ResXR and is easier to update.

| Folder | What goes here |
| ------ | -------------- |
| `Scripts/` | Reusable runtime scripts (e.g. `QuestFoveationSetup`) |
| `Editor/` | Editor-only tools and import defaults (e.g. `TAUXRTextureImportDefaults`) |
| `Meta components/` | Examples of Meta Interaction SDK components (grab, poke button, slider) — moved here from `Assets/ResXR`. Need `Base Scene With Meta Interactions` |
| `UI Set/` | Optional add-on for Meta's Quest-style UI Set — see [UI Set/README.md](UI%20Set/README.md) |

Add `Prefabs/`, `Materials/`, etc. when there is something to put in them.

Project-specific code does **not** go here - put it in `Assets/Project Folder/` (or your own project folder).
Only move something into `Assets/TAUXR` once it is useful for every project.

There is intentionally no assembly definition (.asmdef) here: ResXR's scripts are compiled into the default
assembly, and an asmdef would prevent these scripts from using ResXR classes such as `ResXRPlayer`.
