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
| `Assets/TAUXR/` | Studio scripts shared by all projects — see [Assets/TAUXR/README.md](Assets/TAUXR/README.md) |
| `Assets/Project Folder/` | Your project's own content. Start from `Main Scene.unity` |
| `STUDIO_SETUP.md` | New-project checklist |
| `PROJECT_README_TEMPLATE.md` | README to fill in for each new project |
| `UPSTREAM_UPDATES.md` | Maintaining this template: pulling ResXR updates, upgrading Unity/SDK |
| `ResXR_Template_Documentation.md` | ResXR's own full documentation (also online at https://docs.resxr.org/unity/) |

To run: open `Assets/ResXR/Base Scene/Base Scene.unity`, make sure the Base Scene is first in **File → Build
Profiles**, then press Play (Quest Link) or **Build And Run**.

Room calibration (lining the virtual scene up with a physical room) is opt-in, as in ResXR — the setup steps are in
[STUDIO_SETUP.md](STUDIO_SETUP.md).

## Studio additions

- **`QuestFoveationSetup`** (in the Base Scene as `TAUXR_QuestFoveation`) — turns on fixed foveated rendering
  (High, dynamic) for a cheap GPU saving. Tune per project.
- **Mipmaps by default** — every image in the project generates mipmaps, and `TAUXRTextureImportDefaults` turns them on
  for every newly imported image (less shimmer at a distance in VR).
- **Palm toucher** — buttons and other `Toucher` targets react to the palm as well as the index fingertip.
  Switch off per project with **Use Palm Toucher** on the `ResXR Hand` prefab.
- **Package name** `com.tauxr.studiotemplate` — change it per project (see the checklist).
- Removed packages a Quest project doesn't use: Ads, In-App Purchasing, Analytics, Version Control, Multiplayer
  Center, Visual Scripting, 2D Tilemap. Removed the Meta XR Simulator package (no version works with SDK 205 —
  use Quest Link or Meta's standalone simulator).

## Changes from ResXR

ResXR is licensed under Apache 2.0, which asks that modified files are listed. Files under `Assets/ResXR` that this
template changes:

| File | Change |
| ---- | ------ |
| `Base Scene/ResXR_RoomCalibrator/EnvironmentCalibrator.cs`, `ResXR_RoomCalibrator.prefab` | Hides the calibrator after Confirm; Redo clears old marks; recorded points shown green/cyan so alignment can be checked before confirming; pinch-hold progress in the log |
| `Utilities/General Scripts/ResXRHeadsetServices.cs` | Fixes `Instance` staying null (private `Awake` hid the singleton's), which crashed the first passthrough toggle; clears the eye camera to transparent while passthrough is on |
| `Demo Experiments/Shared Utils/Instructions Panel/Scripts/InstructionsPanel.cs` | Fixes the one-frame full-size flash before the show animation; gentler grow-in (from 90%); optional show delay and sound |
| `Demo Experiments/Shared Utils/Instructions Panel/Scripts/InstructionsPanelWithConfirmation.cs` | Adds `ShowUntilDismissed` (hide on button press *or* by code) |
| `Demo Experiments/Shared Utils/Player Position Mark/Scripts/PositionMarkTriggerZone.cs` | Adds `IsHeadInPositionAndFacing` — reliable check when the mark is shown where the participant already stands |
| `Demo Experiments/Shared Utils/Slider/Scripts/ResXRSlider.cs` | Removes `[SerializeField]` from a property (compile error in Unity 6.3) |
| `Base Scene/SceneManagement/ResXRSceneManager.cs`, `ResXRPlayer/ResXRPlayer.cs`, `ProjectInitializer.cs` | Player Repositioner removed (script, prefab and its use in scenes) |
| `ResXRPlayer/ResXR Hand/ResXRHand.cs`, `ResXR Hand.prefab` | Palm toucher |
| `ResXRPlayer/ResXR Player.prefab` | Fade overlay quad moved from 0.10 to 0.11 m (it sat on the camera's near clip plane and flickered) |
| `Pincher.cs`, `ResXRPlayer.cs`, `ImagesRating.cs`, `ListExtensions.cs` | No longer depend on the Visual Scripting package (removed `using` lines; Visual Scripting helpers replaced with the standard Unity calls) |
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
