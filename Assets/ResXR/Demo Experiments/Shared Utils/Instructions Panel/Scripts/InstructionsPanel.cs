using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class InstructionsPanel : MonoBehaviour
{
    [Header("references")]
    public GameObject backPanel;
    public TextMeshPro title;
    public TextMeshPro text;
    public TMPBackPanelResizer backPanelResizer;
    // optional "pop" played whenever the panel appears. leave empty for a silent panel.
    // put the AudioSource on the panel root, not on backPanel/title/text — HideInstant
    // deactivates those, which would cut the clip off mid-play.
    public AudioSource showAudio;

    [Header("settings")]
    public bool hideOnAwake = true;
    public bool useAnimations = true;
    public bool collectEyeGaze = true;
    // Seconds to wait after Show() is called before the panel starts appearing, so it doesn't
    // pop the instant a trigger/collider fires — a short beat feels much less abrupt. Tunable
    // per panel; 0 = instant (original behaviour).
    public float showDelaySeconds = 0.2f;

    [Tooltip("Delay before the pop sound plays, measured from Show() — INDEPENDENT of showDelaySeconds. " +
        "Lower than showDelaySeconds = snappy audio feedback while the panel still eases in; 0 = immediate.")]
    public float soundDelaySeconds = 0f;

    [Tooltip("Grow-in length (seconds). Longer = gentler.")]
    public float showAnimationSeconds = 0.3f;
    [Range(0f, 1f)]
    [Tooltip("Scale the panel grows FROM, as a fraction of full size. ~0.9 = a subtle settle; " +
        "1 = no motion. Growing from 0 is the cheap 'balloon' look, so keep this high.")]
    public float showStartScale = 0.9f;

    private Vector3 initialScale;
    private Collider eyeGazeCollider;

    protected virtual void Awake()
    {
        initialScale = transform.localScale;

        // setup eye gaze collider
        eyeGazeCollider = backPanel.GetComponent<Collider>();

        if (collectEyeGaze && eyeGazeCollider == null)
        {
            Debug.LogWarning("[InstructionsPanel] No collider found on backPanel for eye gaze collection. Adding a BoxCollider. Please check its dimensions or add your own.");
            eyeGazeCollider = backPanel.AddComponent<BoxCollider>();
        }
        else if (!collectEyeGaze && eyeGazeCollider != null)
        {
            Destroy(eyeGazeCollider);
        }

        if (hideOnAwake)
        {
            HideInstant();
        }

    }

    public virtual async UniTask Show(bool doResizeBackPanel = true)
    {
        // Pop sound on its OWN timeline (soundDelaySeconds), decoupled from the visual delay, so it can
        // respond quickly to the trigger while the panel still eases in after showDelaySeconds. Started
        // fire-and-forget from Show() only (not from ShowInstant — AnimateHide calls that too and would
        // play the sound on hide).
        PlayShowAudio().Forget();

        // Small beat before the panel appears so it doesn't pop the instant a trigger fires.
        // Every appearance routes through Show(), so this covers them all.
        if (showDelaySeconds > 0f)
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(showDelaySeconds));
        }

        // Ensure geometry exists and active BEFORE measuring (else TMP may not update correctly)
        ShowInstant();

        if (doResizeBackPanel)
        {
            // Measure the text and fit the back panel to it — fully SYNCHRONOUSLY. ForceMeshUpdate
            // gives TMP a fresh mesh without waiting a frame, so the panel is never presented at full
            // scale before the grow-in. (The old code set full scale and then `await UniTask.Yield()`,
            // which rendered one frame at full size — a flash/pop right before the animation.)
            Vector3 prevScale = transform.localScale;
            transform.localScale = initialScale;   // measure at full scale; no yield, so never rendered here
            title.ForceMeshUpdate();
            text.ForceMeshUpdate();
            backPanelResizer.ResizeBackPanel();
            transform.localScale = prevScale;
        }

        if (useAnimations)
        {
            await AnimateShow();
        }
        else
        {
            ShowInstant();
        }


        Debug.Log($"[InstructionsPanel] {gameObject.name} was shown.");
    }

    // Plays the pop sound after its own soundDelaySeconds, independent of the visual show delay.
    private async UniTaskVoid PlayShowAudio()
    {
        if (showAudio == null)
        {
            return;
        }
        if (soundDelaySeconds > 0f)
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(soundDelaySeconds));
        }
        showAudio.Play();
    }

    public virtual async UniTask Hide()
    {
        if (useAnimations)
        {
            await AnimateHide();
        }
        else
        {
            HideInstant();
        }
        Debug.Log($"[InstructionsPanel] {gameObject.name} was hidden.");
    }


    public void SetTitle(string newTitle)
    {
        title.text = newTitle;
    }

    public void SetText(string newText)
    {
        text.text = newText;
    }

    public async UniTask ShowForSeconds(float seconds, bool doResizeBackPanel = true)
    {
        await Show(doResizeBackPanel);
        await UniTask.Delay(System.TimeSpan.FromSeconds(seconds));
        await Hide();
    }

    protected virtual void ShowInstant()
    {
        backPanel.SetActive(true);
        title.gameObject.SetActive(true);
        text.gameObject.SetActive(true);
    }

    protected virtual void HideInstant()
    {
        backPanel.SetActive(false);
        title.gameObject.SetActive(false);
        text.gameObject.SetActive(false);
    }

    private async UniTask AnimateShow()
    {
        // Gentle grow-in: scale from `showStartScale` (a fraction of full, e.g. 0.9) up to full —
        // NOT from zero, which reads as a cheap balloon/pop. A small range + smooth ease-out lets
        // the panel settle in place rather than inflate. No fade (opaque panel), so this is the main
        // lever on how "considered" it feels; tune showStartScale / showAnimationSeconds on device.
        float duration = Mathf.Max(0.0001f, showAnimationSeconds);
        transform.localScale = initialScale * showStartScale;
        ShowInstant();
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = Mathf.Clamp01(t / duration);

            // ease-out (fast → slow) so it arrives and settles gently
            float ease = 1f - Mathf.Pow(1f - normalized, 3f);
            float scaleFactor = Mathf.Lerp(showStartScale, 1f, ease);

            transform.localScale = initialScale * scaleFactor;
            await UniTask.Yield();
        }

        transform.localScale = initialScale;
    }

    private async UniTask AnimateHide(float duration = 0.25f)
    {
        // Make sure it's visible before shrinking
        ShowInstant();

        float t = 0f;
        Vector3 startScale = initialScale;

        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = Mathf.Clamp01(t / duration);

            // ease-in (mirrors the ease-out of AnimateShow)
            float ease = Mathf.Pow(normalized, 3);

            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, ease);

            await UniTask.Yield();
        }

        transform.localScale = Vector3.zero;

        // now hide objects
        HideInstant();
    }


}
