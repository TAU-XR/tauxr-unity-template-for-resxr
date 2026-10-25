# New project checklist

Follow this every time you start a new TAUXR project from the studio template.

## 1. Create the repository
- [ ] On GitHub, open `TAU-XR/tauxr-unity-template-for-resxr` and click **Use this template → Create a new repository**.
- [ ] Owner: `TAU-XR`. Name it after the project (e.g. `FlipMazeResXR`). Private unless there's a reason not to be.
- [ ] Clone it locally.

> The new repository is a **snapshot** of the template on the day you create it. Later improvements to the template
> do not reach existing projects automatically — copy them over by hand if a project needs them.

## 2. Open in Unity
- [ ] Unity Hub → **Add project from disk** → select the folder.
- [ ] Editor **6000.3.24f1** with **Android Build Support** (incl. OpenJDK and Android SDK & NDK).
- [ ] Wait for the first import to finish (it downloads the Meta XR SDK and other packages).

## 3. Name the project
In **Edit → Project Settings → Player**:
- [ ] **Product Name** — the app name shown on the headset.
- [ ] **Package Name** (Android → Other Settings) — `com.tauxr.<projectname>`, lowercase, letters and digits only
      (e.g. `com.tauxr.flipmaze`). Two projects with the same package name overwrite each other on the headset.
- [ ] **Version** — start at `0.1.0`.

## 4. Set up the scenes
- [ ] Your project's own files go in `Assets/Project Folder/` (or a folder named after the project).
      Start from `Assets/Project Folder/Main Scene.unity`.
- [ ] **File → Build Profiles**: the Base Scene must be first, your first scene second. Remove the demo scenes.
- [ ] **Room calibration** — only if the virtual scene must line up with a real room (see FlipMazeResXR for a
      working example). It is off by default and the calibrator is not in the Base Scene:
      1. Drag `Assets/ResXR/Base Scene/ResXR_RoomCalibrator/ResXR_RoomCalibrator.prefab` into the Base Scene.
      2. Place `VirtualReferencePoint1` (centre) and `VirtualReferencePoint2` (forward) on the two physical marks you
         will touch in the room.
      3. On `ResXR_ProjectInitializer`, tick **Should Project Use Calibration** (and **Should Calibrate On Editor** to
         test it over Quest Link).

      Everything else wires itself at runtime: the hand pivots follow the player's hands, and passthrough works even if
      the Base Scene has no `ResXRHeadsetServices` (the calibrator adds one).

      On the headset, text in front of you says what to do next: touch each mark with the LEFT index fingertip and
      pinch-HOLD the RIGHT hand until it reaches 100%. The two recorded points appear green and cyan — check they sit on
      the guides, then press Confirm (or Redo to mark again).

## 5. Choose what to keep
- [ ] **Recording data or not** — see [Recording on / off](#recording-on--off) below.
- [ ] **Demo experiments** (`Assets/ResXR/Demo Experiments`): you may delete the Binary Choice, Maze and Museum folders
      once you no longer need them as examples. **Keep `Shared Utils`** — panels, buttons, slider and position marks
      used across projects live there.
- [ ] **Palm toucher**: on the `ResXR Hand` prefab, untick **Use Palm Toucher** if palms cause accidental button
      presses in this project.
- [ ] **Quest-style UI** (optional): Meta's UI Set is available as an add-on — see `Assets/TAUXR/UI Set/README.md`.
      It needs the `Base Scene With Meta Interactions`.
- [ ] **Foveated rendering**: `TAUXR_QuestFoveation` in the Base Scene is set to High + dynamic. Lower it if the edges
      of the view look blurry for this project.

## 6. Replace the README
- [ ] Replace `README.md` with the contents of `PROJECT_README_TEMPLATE.md` and fill it in.
- [ ] Delete `STUDIO_SETUP.md`, `UPSTREAM_UPDATES.md` and `PROJECT_README_TEMPLATE.md` from the project — they
      describe the template, not your project.

## 7. First build
- [ ] Connect a Quest, **File → Build And Run**.
- [ ] Check: hands tracked, pinch works, buttons react to fingertip and palm, fade in/out has no flicker.
- [ ] Commit and push.

---

## Recording on / off

The template ships with recording **on** (ResXR's default). One switch controls it.

### Turning recording OFF (projects that don't collect data)

1. **Base Scene → `ResXR_DataManager` → Recording → untick `Recording Enabled`.**
   With it off, nothing is written to the headset: no session folder, no CSV or JSON files. Calls to
   `ReportEvent`, `LogCustom`, `TrialsData.Log` and `LogLineToFile` are silently ignored, so the flow scripts and demo
   code don't need changes. The Recording Options below the switch have no effect while it's off.
2. Switch off what only exists for recording, unless the project uses it for interaction:

   | Where | Setting | Turn off when |
   | ----- | ------- | ------------- |
   | **Meta XR project settings** (Edit → Project Settings → Meta XR, or `Assets/Oculus/OculusProjectConfig.asset`) | Body Tracking Support, Face Tracking Support, Eye Tracking Support → **None** | the project doesn't use them. Otherwise the app asks the participant for those permissions at startup. |
   | **Base Scene → `ResXR_Player`** | `Is Eye Tracking Enabled`, `Is Face Tracking Enabled` | no gaze-based interaction / no face expressions in the experience. Saves processing every frame. |
   | **Instructions panels** (`Shared Utils/Instructions Panel`) | `Collect Eye Gaze` | always safe to untick when not recording — it only adds a gaze collider for logging. |
   | **File → Build Profiles** | the Live Monitor scene (`Assets/ResXR/Base Scene/ResXRDataManager/Live Monitor/...`) | always — it only displays recorded data. |

   Keep `ResXR_DataManager` itself in the Base Scene — other scripts expect it to exist.

### Turning recording ON

1. **`ResXR_DataManager` → tick `Recording Enabled`**, then choose what to record under **Recording Options**
   (head/nodes, eyes, hands, body, face, system status, custom transforms).
2. Turn on the matching tracking: Meta XR project settings (Eye / Face / Body Tracking Support → Supported or
   Required) and `ResXR_Player` → `Is Eye Tracking Enabled` / `Is Face Tracking Enabled`. Eye and face tracking need a
   Quest Pro.
3. Data is written to `Application.persistentDataPath` on the headset (`/sdcard/Android/data/<package name>/files/`),
   one folder per session. In the editor it goes to Unity's persistent data path, or tick `Export In Editor` and set
   `Save File Path` to choose a folder (left empty, it uses `<system temp>/ResXR_EditorLogs`).
4. Full reference: [ResXR recording docs](https://docs.resxr.org/unity/recording/) and
   [data output](https://docs.resxr.org/unity/data-output/).
