using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PositionMarkTriggerZone : MonoBehaviour
{
    [Header("Facing Direction Constraint (in degrees)")]
    [Tooltip("Yaw = Horizontal rotation (left/right). Pitch = Vertical tilt (up/down).")]
    [Range(-180f, 180f)] public float desiredYaw = 0f;
    [Range(-89f, 89f)] public float desiredPitch = 0f;

    [Tooltip("Allowed angular deviation from the target direction.")]
    [Range(0f, 180f)] public float allowedAngle = 30f;


    private float gizmoLength = 2f;
    private Color gizmoColor = new Color(1f, 0.5f, 0f, 0.4f);

    public event Action<GameObject> OnPlayerEnter;

    private bool hasTriggered = false;
    private Collider _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    // Physics-independent check: is `head` inside this trigger's collider AND facing within
    // allowedAngle of the target direction? Unity does NOT reliably fire OnTriggerStay for a
    // collider that is already overlapping when this trigger is enabled/teleported around a
    // stationary head (and the one-shot `hasTriggered` latch can also persist from a prior use),
    // so a caller that places the zone around the player — rather than having the player walk in —
    // should POLL this every frame instead of subscribing to OnPlayerEnter.
    public bool IsHeadInPositionAndFacing(Transform head)
    {
        if (_collider == null)
        {
            return false;
        }

        Vector3 headPos = head.position;
        if ((_collider.ClosestPoint(headPos) - headPos).sqrMagnitude > 0.0001f)
        {
            return false; // head is outside the trigger volume
        }

        return Vector3.Angle(head.forward, GetTargetDirection()) <= allowedAngle;
    }

    private void OnTriggerStay(Collider other)
    {
        if (hasTriggered || !other.CompareTag("PlayerHead"))
            return;

        Vector3 playerForward = other.transform.forward;
        Vector3 worldTargetDir = GetTargetDirection();

        float angle = Vector3.Angle(playerForward, worldTargetDir);

        if (angle <= allowedAngle)
        {
            hasTriggered = true;
            OnPlayerEnter?.Invoke(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerHead"))
        {
            hasTriggered = false;
        }
    }

    private Vector3 GetTargetDirection()
    {
        // Create a direction from pitch and yaw relative to this transform
        Quaternion rotation = Quaternion.Euler(desiredPitch, desiredYaw, 0f);
        return transform.rotation * rotation * Vector3.forward;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        Vector3 origin = transform.position;
        Vector3 direction = GetTargetDirection().normalized;

        // Compute left and right edge of the triangle
        Quaternion leftRot = Quaternion.AngleAxis(-allowedAngle, Vector3.up);
        Quaternion rightRot = Quaternion.AngleAxis(allowedAngle, Vector3.up);

        Vector3 left = origin + (leftRot * direction) * gizmoLength;
        Vector3 right = origin + (rightRot * direction) * gizmoLength;
        Vector3 tip = origin + direction * gizmoLength;

        // Draw triangle outline
        Gizmos.DrawLine(origin, left);
        Gizmos.DrawLine(origin, right);
        Gizmos.DrawLine(left, right);
    }
}
