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
      2. Add a **ResXRHeadsetServices** component to it; drag in the **OVRManager** and the **CenterEyeAnchor** camera
         (needed for passthrough during calibration).
      3. On the prefab's `InstructionsPivot_RightHand` / `_LeftHand`, set the follow targets to the player's hand anchors.
      4. Place `VirtualReferencePoint1` (centre) and `VirtualReferencePoint2` (forward) on the two physical marks you
         will touch in the room.
      5. On `ResXR_ProjectInitializer`, tick **Should Project Use Calibration**.

      On the headset: touch each mark with the LEFT index fingertip and pinch-hold the RIGHT hand. The two recorded
      points appear green and cyan — check they sit on the guides, then Confirm (or Redo).

## 5. Choose what to keep
- [ ] **Not recording data?** In the Base Scene, on `ResXR_DataManager`, untick the recording options you don't need
      (eyes, hands, body, face). ResXR still writes a small session folder; it does no harm.
- [ ] **Recording data?** See the ResXR docs: https://docs.resxr.org/unity/recording/
- [ ] **Demo experiments** (`Assets/ResXR/Demo Experiments`): you may delete the Binary Choice, Maze and Museum folders
      once you no longer need them as examples. **Keep `Shared Utils`** — panels, buttons, slider and position marks
      used across projects live there.
- [ ] **Palm toucher**: on the `ResXR Hand` prefab, untick **Use Palm Toucher** if palms cause accidental button
      presses in this project.
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
