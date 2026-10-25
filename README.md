# TAUXR Studio Unity Template

The starting point for every TAUXR Unity XR project for Meta Quest.

It is built on the [ResXR Unity Research Template](https://github.com/ResXR/resxr-unity-research-template)
and adds what the studio needs: newer Unity and Meta SDK versions, fixes from our own projects, and shared studio
tools. Projects that record data use ResXR's recording pipeline as usual; projects that don't simply leave it unused.

ResXR itself is not modified — it stays as it is and available for everyone. All studio changes live in this repo.

| | |
| --- | --- |
| **Unity** | 6000.3.24f1 (Unity 6.3 LTS) with Android Build Support |
| **Meta XR SDK** | 205.0.0 (Core, MR Utility Kit, Haptics, Interaction OVR, Platform) · Audio 85.0.0 · Voice 85.0.1 |
| **Headsets** | Meta Quest 2 / 3 / 3S / Pro (eye and face tracking need Quest Pro) |
| **Based on** | ResXR `main` @ `6c3f0f8` (after v0.2.0) |

## Starting a new project

1. On GitHub click **Use this template → Create a new repository** (owner `TAU-XR`).
2. Clone it and open it in Unity Hub with **6000.3.24f1**.
3. Follow the checklist in **[STUDIO_SETUP.md](STUDIO_SETUP.md)** (naming, scenes, what to keep or delete).

A new project is a **snapshot** of this template on the day you create it: it gets every version upgrade, fix and
studio tool that is here at that moment. Changes made to the template later do not reach existing projects.

## What's in the template

| Path | Contents |
| ---- | -------- |
| `Assets/ResXR/` | ResXR framework: Base Scene, `ResXRPlayer` (hands, pinch, fade), flow management (Session → Task → Trial), room calibrator, data recording, `Shared Utils` (panels, buttons, slider, position marks), demo experiments |
| `Assets/TAUXR/` | Studio scripts shared by all projects, the Meta UI Set add-on and the Meta components examples (grab, poke button, slider) — see [Assets/TAUXR/README.md](Assets/TAUXR/README.md) |
| `Assets/Project Folder/` | Your project's own content. Start from `Main Scene.unity` |
| `STUDIO_SETUP.md` | New-project checklist |
| `PROJECT_README_TEMPLATE.md` | README to fill in for each new project |
| `UPSTREAM_UPDATES.md` | Maintaining this template: pulling ResXR updates, upgrading Unity/SDK |
| `ResXR_Template_Documentation.md` | ResXR's own full documentation (also online at https://docs.resxr.org/unity/) |

To run: open `Assets/ResXR/Base Scene/Base Scene.unity`, make sure the Base Scene is first in **File → Build
Profiles**, then press Play (Quest Link) or **Build And Run**.

### Where things are

| What | Path |
| ---- | ---- |
| Base Scene (ResXR hands only) | `Assets/ResXR/Base Scene/Base Scene.unity` |
| Base Scene + Meta interaction rig | `Assets/ResXR/Base Scene/Base Scene With Meta Interactions.unity` |
| Player (hands, pinch, fade, palm toucher) | `Assets/ResXR/Base Scene/ResXRPlayer/ResXR Player.prefab` |
| Room calibrator | `Assets/ResXR/Base Scene/ResXR_RoomCalibrator/ResXR_RoomCalibrator.prefab` |
| Calibration on/off | `ResXR_ProjectInitializer` in the Base Scene |
| Recording on/off | `ResXR_DataManager` in the Base Scene |
| Panels, buttons, slider, position marks | `Assets/ResXR/Demo Experiments/Shared Utils/` |
| Flow scripts (Session → Task → Trial) | `Assets/ResXR/Flow Management/` |
| Meta interaction examples | `Assets/TAUXR/Meta components/` |
| Meta UI Set add-on | `Assets/TAUXR/UI Set/` |

**Which Base Scene?** Both have ResXR's own hand tracking (hands, pinch, `Toucher` fingertip/palm colliders), which is
all that ResXR's buttons, panels, slider, pinch-grab and the calibrator need — use the plain **Base Scene** by default.
Use **Base Scene With Meta Interactions** only when the project uses Meta's interaction system: the UI Set add-on,
Meta poke/ray canvases, Meta grab interactables, the `Meta components` examples or the Live Monitor. In that scene both
hand systems run side by side.

Room calibration (lining the virtual scene up with a physical room) is opt-in, as in ResXR — the setup steps are in
[STUDIO_SETUP.md](STUDIO_SETUP.md).

## Studio additions

- **`QuestFoveationSetup`** (in the Base Scene as `TAUXR_QuestFoveation`) — turns on fixed foveated rendering
  (High, dynamic) for a cheap GPU saving. Tune per project.
- **Mipmaps by default** — every image in the project generates mipmaps, and `TAUXRTextureImportDefaults` turns them on
  for every newly imported image (less shimmer at a distance in VR).
- **Palm toucher** — buttons and other `Toucher` targets react to the palm as well as the index fingertip.
  Switch off per project with **Use Palm Toucher** on the `ResXR Hand` prefab.
- **Recording on/off switch** — `Recording Enabled` on `ResXR_DataManager`. Off = nothing written to disk and all
  logging calls ignored. What else to switch off is in [STUDIO_SETUP.md](STUDIO_SETUP.md#recording-on--off).
- **Room calibrator** — wires itself (hand pivots, passthrough), shows in-headset instructions and pinch-hold
  progress, and lets you check the alignment before confirming. Still opt-in, as in ResXR.
- **Meta UI Set add-on** (optional) — Quest-style buttons, sliders, dialogs and menus from Meta's Interaction SDK,
  alongside ResXR's own UI. See [Assets/TAUXR/UI Set/README.md](Assets/TAUXR/UI%20Set/README.md).
- **Quest render settings** — `URP-Quest` shadow distance 25 m with 2 cascades (was 150 m / 4); unused URP pipeline
  assets removed so only the active one is left.
- **Git setup** — `.gitattributes` keeps line endings consistent across machines and lets Unity's Smart Merge handle
  scenes and prefabs (see [UPSTREAM_UPDATES.md](UPSTREAM_UPDATES.md)); `build_info.json` (rewritten on every build) is
  no longer tracked.
- **Package name** `com.tauxr.studiotemplate` — change it per project (see the checklist).
- Removed packages a Quest project doesn't use: Ads, In-App Purchasing, Analytics, Version Control, Multiplayer
  Center, Visual Scripting, 2D Tilemap. Removed the Meta XR Simulator package (no version works with SDK 205 —
  use Quest Link or Meta's standalone simulator).

## Changes from ResXR

ResXR is licensed under Apache 2.0, which asks that modified files are listed. Files under `Assets/ResXR` that this
template changes:

| File | Change |
| ---- | ------ |
| `Base Scene/ResXR_RoomCalibrator/EnvironmentCalibrator.cs`, `ResXR_RoomCalibrator.prefab` | Hides the calibrator after Confirm; Redo clears old marks and loops instead of recursing; recorded points shown green/cyan so alignment can be checked before confirming; in-headset status text with pinch-hold progress; hand pivots follow the hands when unwired; adds `ResXRHeadsetServices` if the scene has none |
| `Base Scene/ProjectInitializer.cs` | Clear error instead of a crash when calibration is on but there is no calibrator in the Base Scene |
| `Base Scene/ResXRDataManager/ResXRDataManager.cs` | `Recording Enabled` master switch |
| `Utilities/General Scripts/FollowTransform.cs` | Adds `HasTarget` |
| `Utilities/General Scripts/ResXRSingleton.cs` | A duplicate singleton stops after destroying itself (no longer runs its setup) |
| `Flow Management/SessionManager.cs` | `BetweenTasksFlow` no longer throws `NotImplementedException` (crashed after the first task) |
| `Demo Experiments/Shared Utils/Simple Button/SimpleButton.cs` | Counts touchers, so one push with fingertip + palm (or two hands) presses once |
| `Utilities/General Scripts/ResXRHeadsetServices.cs` | Fixes `Instance` staying null (private `Awake` hid the singleton's), which crashed the first passthrough toggle; clears the eye camera to transparent while passthrough is on; finds OVRManager and the eye camera when not assigned |
| `Demo Experiments/Shared Utils/Instructions Panel/Scripts/InstructionsPanel.cs` | Fixes the one-frame full-size flash before the show animation; gentler grow-in (from 90%); optional show delay and sound |
| `Demo Experiments/Shared Utils/Instructions Panel/Scripts/InstructionsPanelWithConfirmation.cs` | Adds `ShowUntilDismissed` (hide on button press *or* by code) |
| `Demo Experiments/Shared Utils/Player Position Mark/Scripts/PositionMarkTriggerZone.cs` | Adds `IsHeadInPositionAndFacing` — reliable check when the mark is shown where the participant already stands |
| `Demo Experiments/Shared Utils/Slider/Scripts/ResXRSlider.cs` | Removes `[SerializeField]` from a property (compile error in Unity 6.3) |
| `Base Scene/SceneManagement/ResXRSceneManager.cs`, `ResXRPlayer/ResXRPlayer.cs`, `ProjectInitializer.cs` | Player Repositioner removed (script, prefab and its use in scenes) |
| `ResXRPlayer/ResXRPlayer.cs` | `RecenterView()` implemented (it was empty); `CalibrateFloorHeight()` logs that it's not implemented instead of silently doing nothing |
| `ResXRPlayer/ResXR Hand/ResXRHand.cs`, `ResXR Hand.prefab` | Palm toucher |
| `ResXRPlayer/ResXR Player.prefab` | Fade overlay quad moved from 0.10 to 0.11 m (it sat on the camera's near clip plane and flickered) |
| `Pincher.cs`, `ResXRPlayer.cs`, `ImagesRating.cs`, `ListExtensions.cs` | No longer depend on the Visual Scripting package (removed `using` lines; Visual Scripting helpers replaced with the standard Unity calls) |
| `Meta components/` (folder) | Moved to `Assets/TAUXR/Meta components/` (`ResXR_Template_Documentation.md` still shows the old path) |
| Base Scenes, demo scenes, image import settings | Wiring for the above; mipmaps on |

## Updating from ResXR

Not usually needed. If ResXR releases something worth having, see [UPSTREAM_UPDATES.md](UPSTREAM_UPDATES.md).

## License

Apache License 2.0 — see [LICENSE](LICENSE) and [NOTICE](NOTICE). Original work © ResXR; modifications © TAUXR.

The Meta XR SDK (installed via the Package Manager) is provided by Meta Platform Technologies, LLC under the Meta SDK
License Agreement and is not covered by Apache 2.0. Other bundled libraries (UniTask, NaughtyAttributes, DOTween)
are listed in [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md).

## Acknowledgments

ResXR builds on the early work of the [TAUXR Research Template](https://github.com/TAU-XR/TAUXR-Research-Template)
by TAU-XR Studio and [talmzip](https://github.com/talmzip). Please cite ResXR when publishing research made with it:

```bibtex
@software{resxr,
  title = {ResXR: XR Experiment Recording Template},
  year  = {2026},
  url   = {https://github.com/ResXR/resxr-unity-research-template}
}
```
