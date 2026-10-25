using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using System.Threading;

public class ResXRPlayer : ResXRSingleton<ResXRPlayer>
{
    [Header("Player Trackables")]
    [SerializeField]
    private Transform ovrRig;

    [SerializeField] private Transform playerHead;
    [SerializeField] private Transform rightHandAnchor;
    [SerializeField] private Transform leftHandAnchor;
    public Transform PlayerHead => playerHead;
    public Transform RightHand => rightHandAnchor;
    public Transform LeftHand => leftHandAnchor;

    [Header("Hand Tracking")] public ResXRHand HandLeft;
    public ResXRHand HandRight;

    [Header("Eye Tracking")] public ResXREyeTracker EyeTracker;
    public bool IsEyeTrackingEnabled;
    public Transform FocusedObject => EyeTracker.FocusedObject;
    public Vector3 EyeGazeHitPosition => EyeTracker.EyeGazeHitPosition;
    public Transform RightEye => EyeTracker.RightEye;
    public Transform LeftEye => EyeTracker.LeftEye;


    [Header("Face Tracking")]
    [SerializeField]
    public OVRFaceExpressions ovrFace;

    public OVRFaceExpressions OVRFace => ovrFace;
    public bool IsFaceTrackingEnabled;

    // Color overlay
    [SerializeField] private MeshRenderer colorOverlayMR;
    private CancellationTokenSource _fadeCts;

    public ControllersInputManager ControllersInputManager;
    public PinchingInputManager PinchingInputManager;

    protected override void DoInAwake()
    {
        HandRight.Init();
        HandLeft.Init();

        ControllersInputManager = new ControllersInputManager();
        PinchingInputManager = new PinchingInputManager(HandLeft, HandRight);

        if (IsEyeTrackingEnabled)
            EyeTracker.Init();
        if (IsFaceTrackingEnabled)
        {
            ovrRig.gameObject.AddComponent<OVRFaceExpressions>();
        }
    }


    void Update()
    {
        HandRight.UpdateHand();
        HandLeft.UpdateHand();

        if (IsEyeTrackingEnabled)
        {
            EyeTracker.UpdateEyeTracker();
        }
    }

    // Covers player's view with a solid color, fading over duration seconds.
    // Cancels any in-progress fade — last caller always wins.
    public async UniTask FadeViewToColor(Color targetColor, float duration)
    {
        // Cancel any in-progress fade and start a fresh token for this one.
        _fadeCts?.Cancel();
        _fadeCts?.Dispose();
        _fadeCts = new CancellationTokenSource();
        CancellationToken token = _fadeCts.Token;

        if (duration == 0)
        {
            colorOverlayMR.material.color = targetColor;
            Debug.Log($"[ResXRPlayer] Fade started: {targetColor} instant");
            Debug.Log($"[ResXRPlayer] Fade finished: {targetColor}");
            return;
        }

        Color currentColor = colorOverlayMR.material.color;
        if (currentColor == targetColor)
        {
            Debug.Log($"[ResXRPlayer] Fade skipped: view is already {targetColor}");
            return;
        }

        Debug.Log($"[ResXRPlayer] Fade started: {targetColor} over {duration}s");

        try
        {
            float lerpTime = 0;
            while (lerpTime < duration)
            {
                lerpTime += Time.deltaTime;
                float t = lerpTime / duration;
                colorOverlayMR.material.color = Color.Lerp(currentColor, targetColor, t);

                await UniTask.Yield(token);
            }

            Debug.Log($"[ResXRPlayer] Fade finished: {targetColor}");
        }
        catch (OperationCanceledException)
        {
            Debug.Log($"[ResXRPlayer] Fade cancelled (was heading to {targetColor} over {duration}s)");
        }
    }

    public void RecenterView()
    {
        // IMPLEMENT
    }

    public void CalibrateFloorHeight()
    {
        // IMPLEMENT
    }

    public void SetPassthrough(bool state)
    {
        ResXRHeadsetServices.Instance.SetPassthrough(state);
    }

    #region Hands

    public Transform GetHandFingerCollider(HandType handType, FingerType fingerType)
    {
        switch (handType)
        {
            case HandType.Left:
                return HandLeft.GetFingerCollider(fingerType);
            case HandType.Right:
                return HandRight.GetFingerCollider(fingerType);
            case HandType.Any:
                return HandLeft.GetFingerCollider(fingerType);
            case HandType.None:
                return null;
            default: return null;
        }
    }

    #endregion
}