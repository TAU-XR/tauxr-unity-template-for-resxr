using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class EnvironmentCalibrator : ResXRSingleton<EnvironmentCalibrator>
{
    [SerializeField] private GameObject centerModel;
    [SerializeField] private Transform virtualReferencePointPosition;
    [SerializeField] private Transform virtualReferencePointRotation;
    [SerializeField] private GameObject calibrationMark;
    [SerializeField] private GameObject _btnConfirm;
    [SerializeField] private GameObject _btnRedo;

    // The ResXR_RoomCalibrator root. Disabled when calibration confirms, so its guide
    // markers / centre model don't stay visible in the Base Scene and overlap the maze.
    // Optional: if left unwired, the guide markers are hidden individually instead.
    [SerializeField] private GameObject calibratorRoot;

    [SerializeField]
    private FollowTransform
        _playerMarkedPosition; // a sphere following the exact position players mark when they pinch. Changes every frame according to players' hand.

    // pivots that follow the hands, for hand-attached instructions. If their target is left empty they follow
    // the player's hand anchors, so the prefab works without scene wiring.
    [SerializeField] private FollowTransform _instructionsPivotLeft;
    [SerializeField] private FollowTransform _instructionsPivotRight;

    [Header("Status text")]
    // in-headset text telling the operator what to do next and showing pinch-hold progress.
    [SerializeField] private TextMeshPro _statusText;
    [SerializeField] private float _statusTextDistance = 0.6f;     // metres in front of the head
    [SerializeField] private float _statusTextHeightOffset = 0.15f; // metres below head height
    private const float STATUS_TEXT_FOLLOW_SPEED = 4f;
    private string _lastLoggedStatus;

    private float PINCH_HOLD_DURATION = 1f;

    // gets 2 points where player touches during the calibration action.
    private Transform realWorldReferencePointPosition;
    private Transform realWorldReferencePointRotation;
    private Transform _player;
    private bool _shouldRedo, _shouldConfirm;

    public async UniTask CalibrateRoom()
    {
        Init();

        // a Redo starts the marking again; loops until the operator confirms.
        while (true)
        {
            await MarkAndAlign();

            DisplayConfirmationButtons();

            while (!_shouldRedo && !_shouldConfirm)
            {
                await UniTask.Yield();
            }

            if (_shouldConfirm) break;

            // Clear this pass's marks before re-marking. Without this each Redo abandons its
            // predecessors (they're parented to the rig), leaving a trail of stray spheres.
            _shouldRedo = false;
            _btnConfirm.gameObject.SetActive(false);
            _btnRedo.gameObject.SetActive(false);
            DestroyReferenceMarks();
        }

        _shouldConfirm = false;
        EndCalibration();
    }

    private async UniTask MarkAndAlign()
    {
        EnterCalibrationMode();
        SetStatus("Calibration started.\nLEFT fingertip on the CENTRE mark, then pinch-HOLD the RIGHT hand.");

        // wait until getting 1st point
        await WaitForRightPinchHoldWithFeedback("Point 1 (centre)");
        realWorldReferencePointPosition = CreateReferenceMark(_playerMarkedPosition.Position, Color.green);
        SetStatus("Point 1 recorded.\nLEFT fingertip on the FORWARD mark, then pinch-HOLD the RIGHT hand.");

        // wait until getting 2st point
        await WaitForRightPinchHoldWithFeedback("Point 2 (forward)");
        realWorldReferencePointRotation = CreateReferenceMark(_playerMarkedPosition.Position, Color.cyan);
        SetStatus("Point 2 recorded. Aligning…");

        // once we have 2 real world reference points - we can calibrate.
        AlignVirtualToPhysicalRoom();
        SetStatus("Aligned. Confirm or Redo.");

        // disable passthrough & show model
        ExitCalibrationMode();
        SetStatus("Check: green should sit on the centre guide, cyan on the forward guide.\nConfirm, or Redo if they are off.");
    }

    private void Init()
    {
        _btnConfirm.gameObject.SetActive(false);
        _btnRedo.gameObject.SetActive(false);

        // TODO: Apply offsets
        _playerMarkedPosition.Init(ResXRPlayer.Instance.GetHandFingerCollider(HandType.Left, FingerType.Index));

        _shouldConfirm = false;
        _shouldRedo = false;

        _player = ResXRPlayer.Instance.transform;
        //    WaitForRightPinchHold = ResXRPlayer.Instance.WaitForPinchHold(HandType.Right, PINCH_HOLD_DURATION, true);

        if (_instructionsPivotLeft != null && !_instructionsPivotLeft.HasTarget)
            _instructionsPivotLeft.Init(ResXRPlayer.Instance.LeftHand);
        if (_instructionsPivotRight != null && !_instructionsPivotRight.HasTarget)
            _instructionsPivotRight.Init(ResXRPlayer.Instance.RightHand);

        // passthrough is switched through ResXRHeadsetServices. If the Base Scene doesn't have one,
        // add it here (it finds the OVRManager and eye camera by itself).
        if (ResXRHeadsetServices.Instance == null)
        {
            gameObject.AddComponent<ResXRHeadsetServices>();
        }

        if (_statusText != null)
        {
            _statusText.gameObject.SetActive(true);
            PlaceStatusText(true);
        }
    }

    private void Update()
    {
        if (_statusText != null && _statusText.gameObject.activeInHierarchy)
        {
            PlaceStatusText(false);
        }
    }

    // keeps the status text in front of the operator's head, facing them. Follows lazily so it doesn't jitter.
    private void PlaceStatusText(bool snap)
    {
        Transform head = ResXRPlayer.Instance != null ? ResXRPlayer.Instance.PlayerHead : null;
        if (head == null) return;

        Vector3 forwardFlat = Vector3.ProjectOnPlane(head.forward, Vector3.up).normalized;
        if (forwardFlat.sqrMagnitude < 0.0001f) forwardFlat = head.forward;

        Vector3 targetPosition = head.position + forwardFlat * _statusTextDistance - Vector3.up * _statusTextHeightOffset;
        Quaternion targetRotation = Quaternion.LookRotation(targetPosition - head.position, Vector3.up);

        Transform text = _statusText.transform;
        float t = snap ? 1f : 1f - Mathf.Exp(-STATUS_TEXT_FOLLOW_SPEED * Time.deltaTime);
        text.SetPositionAndRotation(Vector3.Lerp(text.position, targetPosition, t),
                                    Quaternion.Slerp(text.rotation, targetRotation, t));
    }

    private void EnterCalibrationMode()
    {
        ResXRPlayer.Instance.SetPassthrough(true);

        // display reference point in hand (so user have accurate place for point-markdown)
        _playerMarkedPosition.gameObject.SetActive(true);
    }

    private void DisplayConfirmationButtons()
    {
        // show buttons
        _btnConfirm.gameObject.SetActive(true);
        _btnRedo.gameObject.SetActive(true);
    }

    private void ExitCalibrationMode()
    {
        ResXRPlayer.Instance.SetPassthrough(false);

        // The two recorded marks are deliberately LEFT VISIBLE here.
        //
        // They're parented to the rig, so AlignVirtualToPhysicalRoom has just carried them onto
        // the virtual guide points — which makes them the operator's check that the alignment
        // actually worked: green should sit on the centre guide, cyan on the forward guide. If
        // either is visibly offset, press Redo. Hiding them made Confirm a blind guess.
        //
        // Hiding them here also meant the cyan Point-2 mark was created and deactivated inside
        // the same frame (nothing between CreateReferenceMark and here ever yields), so it was
        // never drawn at all.
        //
        // They're destroyed by DestroyReferenceMarks() — on Confirm, and before a Redo pass.
    }

    private void EndCalibration()
    {
        DestroyReferenceMarks();

        // hide marker + buttons (also covered by the root disable, harmless if redundant)
        _playerMarkedPosition.gameObject.SetActive(false);
        _btnConfirm.gameObject.SetActive(false);
        _btnRedo.gameObject.SetActive(false);
        if (_statusText != null) _statusText.gameObject.SetActive(false);

        // Hide the whole calibrator so its guide points / centre don't overlap the experiment.
        // Safe to disable the hosting GameObject here: this is the last statement of the flow,
        // there are no further awaits, and the CalibrateRoom UniTask has already completed its
        // await loop — the continuation back to ProjectInitializer runs regardless of this
        // object's active state.
        if (calibratorRoot != null)
        {
            calibratorRoot.SetActive(false);
        }
        else
        {
            // Fallback when the root isn't wired: hide the guide markers individually.
            if (virtualReferencePointPosition != null)
            {
                virtualReferencePointPosition.gameObject.SetActive(false);
            }
            if (virtualReferencePointRotation != null)
            {
                virtualReferencePointRotation.gameObject.SetActive(false);
            }
            if (centerModel != null)
            {
                centerModel.SetActive(false);
            }
        }
    }

    // Destroys the two recorded reference marks. They're parented to the player rig (so they
    // stay put through the rig realign), which means disabling the calibrator root would NOT
    // hide them — they have to go explicitly. Called on Confirm, and before a Redo pass so
    // repeated redos don't leave abandoned marks under the rig.
    private void DestroyReferenceMarks()
    {
        if (realWorldReferencePointPosition != null)
        {
            Destroy(realWorldReferencePointPosition.gameObject);
            realWorldReferencePointPosition = null;
        }
        if (realWorldReferencePointRotation != null)
        {
            Destroy(realWorldReferencePointRotation.gameObject);
            realWorldReferencePointRotation = null;
        }
    }

    // Creates a visible, brightly-colored reference mark at a recorded point — the
    // "point recorded" feedback the operator sees. The colour distinguishes the two
    // points (Point 1 green, Point 2 cyan). Uses the calibrationMark prefab if assigned
    // (and still tints it the given colour); otherwise builds an unlit sphere so the mark
    // stays clearly visible in the dark void and in passthrough, regardless of scene lighting.
    private Transform CreateReferenceMark(Vector3 position, Color color)
    {
        if (calibrationMark != null)
        {
            GameObject mark = Instantiate(calibrationMark, position, Quaternion.identity, _player);
            TintUnlit(mark, color);
            return mark.transform;
        }

        Debug.LogWarning("[Calibration] calibrationMark prefab is not assigned — using a fallback sphere. Wire it on EnvironmentCalibrator for a proper mark.");
        GameObject fallback = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        fallback.transform.SetParent(_player);
        fallback.transform.position = position;
        fallback.transform.localScale = Vector3.one * 0.05f;
        Destroy(fallback.GetComponent<Collider>());
        TintUnlit(fallback, color);
        return fallback.transform;
    }

    // Forces the mark to render a flat, bright colour independent of lighting, so it's
    // unmistakable against the black void. Falls back to the standard material tint if the
    // Unlit shader isn't available in the build.
    private static void TintUnlit(GameObject go, Color color)
    {
        Renderer markRenderer = go.GetComponentInChildren<Renderer>();
        if (markRenderer == null)
        {
            return;
        }

        Shader unlit = Shader.Find("Universal Render Pipeline/Unlit");
        if (unlit == null)
        {
            unlit = Shader.Find("Unlit/Color");
        }
        if (unlit != null)
        {
            markRenderer.material = new Material(unlit);
        }
        markRenderer.material.color = color;
    }

    // Waits for a full CONTINUOUS right-hand pinch-hold, showing live hold progress so the
    // operator knows to keep holding (the base WaitForHoldAndRelease gives no feedback, which
    // is why a quick pinch looked like nothing happened). Same pinch detection underneath.
    private async UniTask WaitForRightPinchHoldWithFeedback(string label)
    {
        PinchingInputManager pinch = ResXRPlayer.Instance.PinchingInputManager;

        // if already pinching when we arrive, wait for a release first
        while (pinch.IsRightHeld())
        {
            await UniTask.Yield();
        }

        float held = 0f;
        while (held < PINCH_HOLD_DURATION)
        {
            held = pinch.IsRightHeld() ? held + Time.deltaTime : 0f;
            int percent = Mathf.RoundToInt(Mathf.Clamp01(held / PINCH_HOLD_DURATION) * 100f);
            SetStatus($"{label}\nHOLD the pinch closed…  {percent}%");
            await UniTask.Yield();
        }

        SetStatus($"{label}\nrecorded — release the pinch.");
        while (pinch.IsRightHeld())
        {
            await UniTask.Yield();
        }
    }

    private void SetStatus(string message)
    {
        if (_statusText != null) _statusText.text = message;

        // the hold progress updates every frame - log only when the step changes
        string firstLine = message.Split('\n')[0];
        if (firstLine != _lastLoggedStatus)
        {
            Debug.Log("[Calibration] " + message.Replace("\n", " | "));
            _lastLoggedStatus = firstLine;
        }
    }

    // called when the redo calibration hold button is touched
    public void OnRedoCalibration()
    {
        _shouldRedo = true;
    }

    // called when the confirm calibration hold button is touched
    public void OnConfirmCalibration()
    {
        _shouldConfirm = true;
    }

    public void AlignVirtualToPhysicalRoom()
    {
        // ignore differences on height
        realWorldReferencePointRotation.position = new Vector3(realWorldReferencePointRotation.position.x,
            realWorldReferencePointPosition.position.y, realWorldReferencePointRotation.position.z);

        Vector3 realDirection = (realWorldReferencePointRotation.position - realWorldReferencePointPosition.position)
            .normalized;
        Vector3 virtualDirection = (virtualReferencePointRotation.position - virtualReferencePointPosition.position)
            .normalized;

        float angle = Vector3.SignedAngle(realDirection, virtualDirection, _player.up);
        _player.Rotate(_player.up, angle);

        Vector3 positionOffset = virtualReferencePointPosition.position - realWorldReferencePointPosition.position;
        _player.position += positionOffset;
    }
}