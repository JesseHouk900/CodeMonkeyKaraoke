using UnityEngine;
using Cinemachine;
// CameraManager handles _camera changes and interactions
public class CameraManager : MonoBehaviour
{
    // Assign the virtual cameras from the Unity Inspector
    [SerializeField] private CinemachineVirtualCamera playerVirtualCamera;

    // Boolean to keep track of the current _camera.
    private bool isPlayerCameraActive;

    private void Start()
    {
        isPlayerCameraActive = true;
    }

    // Function to switch the _camera
    public void SwitchCamera(CinemachineVirtualCamera virtualCamera)
    {
        Debug.Log("Switching camera");
        isPlayerCameraActive = !isPlayerCameraActive;

        playerVirtualCamera.enabled = isPlayerCameraActive;
        virtualCamera.enabled = !isPlayerCameraActive;
    }
}