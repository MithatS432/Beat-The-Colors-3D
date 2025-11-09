using UnityEngine;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

public class PlayerMovement : MonoBehaviour
{
    [Header("Camera Settings")]
    public Camera playerCamera;
    public Camera secondaryCamera;
    public bool isUsingSecondaryCamera = false;
    void Start()
    {
        if (isUsingSecondaryCamera)
        {
            playerCamera.enabled = false;
            secondaryCamera.enabled = true;
        }
        else
        {
            playerCamera.enabled = true;
            secondaryCamera.enabled = false;
        }
    }

    void Update()
    {
#if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleCameras();
        }
#else
        if (Keyboard.current != null && Keyboard.current.cKey.wasPressedThisFrame)
        {
            ToggleCameras();
        }
#endif
    }

    private void ToggleCameras()
    {
        isUsingSecondaryCamera = !isUsingSecondaryCamera;
        playerCamera.enabled = !isUsingSecondaryCamera;
        secondaryCamera.enabled = isUsingSecondaryCamera;
    }
}
