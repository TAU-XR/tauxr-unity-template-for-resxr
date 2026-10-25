using NaughtyAttributes;
using UnityEngine;

// Enables Fixed Foveated Rendering at startup.
//
// In Meta XR Core SDK, foveation is a code-only static API
// (OVRManager.foveatedRenderingLevel / OVRManager.useDynamicFoveatedRendering) with no
// serialized Inspector field, so it can't be set from the OVRManager inspector. This
// component re-surfaces it so the level can be tuned in the Inspector and re-applied
// live (the button) for A/B GPU testing with OVR Metrics.
//
// Put it on any GameObject in the persistent Base Scene. Requires the Vulkan graphics API.
public class QuestFoveationSetup : MonoBehaviour
{
    [SerializeField] private OVRManager.FoveatedRenderingLevel foveationLevel = OVRManager.FoveatedRenderingLevel.High;

    [Tooltip("Let the system ease the foveation level down when the GPU has headroom.")]
    [SerializeField] private bool useDynamicFoveation = true;

    private void Start()
    {
        Apply();
    }

    // Also exposed as an Inspector button so the level can be changed and re-applied
    // during Play mode while watching GPU utilisation in OVR Metrics.
    [Button("Apply Foveation Now")]
    private void Apply()
    {
        OVRManager.foveatedRenderingLevel = foveationLevel;
        OVRManager.useDynamicFoveatedRendering = useDynamicFoveation;
        Debug.Log($"[QuestFoveationSetup] Foveation set to {foveationLevel}, dynamic = {useDynamicFoveation}.");
    }
}
