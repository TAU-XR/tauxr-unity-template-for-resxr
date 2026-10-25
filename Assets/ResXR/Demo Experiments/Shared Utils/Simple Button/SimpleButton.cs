using UnityEngine;
using UnityEngine.Events;

public class SimpleButton : MonoBehaviour
{
    public UnityEvent SimpleButtonPressed = new UnityEvent();

    [SerializeField] private Material disabledMaterial;
    [SerializeField] private Renderer buttonGraphics;

    private Material originalMaterial;
    private bool disabled = false;

    // number of Toucher colliders currently inside the button. A hand has several touchers (fingertip, palm),
    // so the button only fires when the first one enters - otherwise one push presses it twice.
    private int touchersInside = 0;

    private void Awake()
    {
        if (buttonGraphics == null)
        {
            Debug.LogWarning($"[SimpleButton] Missing buttonGraphics on {name}", this);
            return;
        }

        // Read the default material without instantiating a runtime copy in edit mode.
        originalMaterial = buttonGraphics.sharedMaterial;
    }

    public void SetButtonEnabled(bool enabled)
    {
        if (buttonGraphics == null)
            return;

        // Prevent editor-time scene mutation.
        if (!Application.isPlaying)
            return;

        if (enabled)
        {
            buttonGraphics.material = originalMaterial;
            disabled = false;
        }
        else
        {
            buttonGraphics.material = disabledMaterial;
            disabled = true;
        }
    }

    // reset when disabled - OnTriggerExit is not called for colliders inside a disabled button
    private void OnDisable()
    {
        touchersInside = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!Application.isPlaying)
            return;

        if (!other.gameObject.CompareTag("Toucher"))
            return;

        touchersInside++;
        if (touchersInside > 1 || disabled)
            return;

        SimpleButtonPressed.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Toucher"))
            return;

        touchersInside = Mathf.Max(0, touchersInside - 1);
    }
}
