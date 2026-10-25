
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ProjectInitializer : MonoBehaviour
{
    [SerializeField] private bool _shouldProjectUseCalibration;     // true if project should be calibrated into a physical space.
    [SerializeField] private bool _shouldCalibrateOnEditor;

    private void Start()
    {
        StartResXRExperience().Forget();
    }

    private async UniTask StartResXRExperience()
    {
        bool shouldCalibrateOnBuild = !Application.isEditor && _shouldProjectUseCalibration;
        bool shouldCalibrateOnEditor = _shouldCalibrateOnEditor && Application.isEditor && _shouldProjectUseCalibration;

        if (shouldCalibrateOnBuild || shouldCalibrateOnEditor)
        {
            if (EnvironmentCalibrator.Instance == null)
            {
                Debug.LogError("[ProjectInitializer] Calibration is on, but there is no ResXR_RoomCalibrator in the Base Scene - skipping calibration.");
            }
            else
            {
                // trigger calibration on BaseScene
                await EnvironmentCalibrator.Instance.CalibrateRoom();
            }
        }

        // after environment was calibrated- load first scene
        ResXRSceneManager.Instance.Init();
    }

}
