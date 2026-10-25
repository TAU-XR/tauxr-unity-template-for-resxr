using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The only script communicating directly with the headset sdk, exposing its functionality to other in project classes.
// Created: 28.01.2024 - Tal. Updated: 28.01.2024 - Tal.
public class ResXRHeadsetServices : ResXRSingleton<ResXRHeadsetServices>
{
    [SerializeField] private OVRManager _ovrManager;

    // The eye camera (CenterEyeAnchor's Camera). Its background must become transparent while
    // passthrough is on, otherwise the opaque clear occludes the Underlay passthrough layer and
    // the room shows as black. Optional: if unwired, only the passthrough flag is toggled.
    [SerializeField] private Camera _eyeCamera;

    private CameraClearFlags _cachedClearFlags;
    private Color _cachedBackgroundColor;
    private bool _hasCachedCameraClear;

    // Use the ResXRSingleton hook, NOT a private Awake(). A private Awake() here would
    // hide ResXRSingleton's own Awake() (which assigns Instance), leaving Instance null
    // and causing a NullReferenceException the first time anything calls SetPassthrough.
    protected override void DoInAwake()
    {
        if(_ovrManager == null)
        {
            Debug.LogError("No OVRManager found in ResXR Headset Services. Make sure to drag it in Base Scene");
        }
    }

    public void SetPassthrough(bool state)
    {
        _ovrManager.isInsightPassthroughEnabled = state;
        SetCameraClearForPassthrough(state);
    }

    // The Underlay OVRPassthroughLayer only shows where the eye buffer is transparent. The base
    // scene camera clears to opaque black (for the experiment's void), which would paint over
    // passthrough. So while passthrough is on we clear to fully-transparent black, and restore
    // the original opaque clear when passthrough is turned off.
    private void SetCameraClearForPassthrough(bool passthroughOn)
    {
        if (_eyeCamera == null)
        {
            return;
        }

        if (passthroughOn)
        {
            if (!_hasCachedCameraClear)
            {
                _cachedClearFlags = _eyeCamera.clearFlags;
                _cachedBackgroundColor = _eyeCamera.backgroundColor;
                _hasCachedCameraClear = true;
            }
            _eyeCamera.clearFlags = CameraClearFlags.SolidColor;
            _eyeCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
        }
        else if (_hasCachedCameraClear)
        {
            _eyeCamera.clearFlags = _cachedClearFlags;
            _eyeCamera.backgroundColor = _cachedBackgroundColor;
        }
    }
}
