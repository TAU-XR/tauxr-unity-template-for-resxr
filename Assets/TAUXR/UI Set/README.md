# TAUXR UI Set add-on

Optional Quest-style UI (buttons, toggles, sliders, dialogs, menus) using **Meta's UI Set**, which ships inside the
Meta Interaction SDK that is already installed. Nothing here replaces ResXR's own UI — `SimpleButton`, the Game Button,
the Instructions Panels and the ResXR Slider keep working exactly as before. Use whichever fits the project.

Meta's prefabs are used **straight from the package** (not copied), so they stay licensed and updated by Meta.
Browse them in the Project window under:
`Packages/Meta XR Interaction SDK Essentials/Runtime/Sample/Objects/UISet/Prefabs/`
(Button, Slider, Dialog, DropDown, ContextMenu, TextInputField, Tooltip, Backplate, Patterns).

## What's in this folder

| Asset | What it is |
| ----- | ---------- |
| `TAUXR UI EventSystem.prefab` | An EventSystem with Meta's `PointableCanvasModule`. Every scene using UI Set panels needs exactly **one**. |
| `TAUXR UI Set Example.unity` | Example scene: two ready-made Meta panels and an empty backplate. |

## Requirements

UI Set controls are uGUI canvases pressed by **Meta's interaction rig** (poke with the fingertip, ray from the hand or
controller) — not by ResXR's `Toucher` colliders. So:

1. Use **`Assets/ResXR/Base Scene/Base Scene With Meta Interactions.unity`** as the Base Scene (it contains Meta's
   `OVRComprehensiveInteractionRig`). The plain `Base Scene` has no Meta rig, and UI Set panels won't react there.
2. Put **one** `TAUXR UI EventSystem` in the scene. The ResXR Live Monitor scene has its own EventSystem — if you load it
   at the same time, remove one of them.

## Try the example

1. Open `Base Scene With Meta Interactions.unity`.
2. Drag `TAUXR UI Set Example.unity` into the Hierarchy to open it additively.
3. Press Play (Quest Link) — poke the panels with your fingertip or point at them and pinch.

## Build your own panel

1. Drag `Backplate/EmptyUIBackplateWithCanvas` into your scene — it's a panel that already has the canvas and Meta's
   poke/ray components.
2. Put controls (e.g. `Button/UnityUIButtonBased/PrimaryButton_IconAndLabel_UnityUIButton`) **inside its Canvas**.
   Controls only work under such a canvas.
3. Hook up the button: in the Inspector (`Button → On Click`), or from a flow script with UniTask (already in the
   project), in the same await style as the ResXR flow:

   ```csharp
   using Cysharp.Threading.Tasks;
   using UnityEngine.UI;

   [SerializeField] private Button confirmButton;

   await confirmButton.OnClickAsync();   // waits until the participant presses it
   ```

4. Colours: the backplate has a `UIThemeManager` — pick one of Meta's themes (Quest Dark / Quest Light) or make your own
   theme asset.

Meta's full reference panels are in `Prefabs/Patterns/` and Meta's own demo scene is
`Packages/Meta XR Interaction SDK Essentials/Runtime/Sample/Objects/UISet/Scenes/UISet.unity`.

## Notes

- Upgrading the Meta SDK can move or change these prefabs (they're in Meta's "Sample" folder). After an SDK upgrade,
  open the example scene and check for missing prefabs.
- Meta's SDK is licensed under the Oculus/Meta SDK License Agreement, not Apache 2.0.
