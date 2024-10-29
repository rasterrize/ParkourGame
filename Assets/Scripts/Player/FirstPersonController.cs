using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonController : MonoBehaviour
{
    private const float DEFAULT_MOUSE_SENSITIVITY = 0.25f;

    [SerializeField] private Camera playerCamera;
    private Transform playerTransform;
    private float mouseSensitivityHorizontal = DEFAULT_MOUSE_SENSITIVITY;
    private float mouseSensitivityVertical = DEFAULT_MOUSE_SENSITIVITY;
    private float xRotation;

    private bool allowLooking = true;

    // Start is called before the first frame update
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerTransform = GetComponent<Transform>();
    }

    public void OnLookAction(InputAction.CallbackContext context)
    {
        if (!allowLooking)
            return;

        // Get the mouse delta from the input context
        var mouseDelta = context.ReadValue<Vector2>();

        // Handle vertical rotation (camera only)
        xRotation -= mouseDelta.y * mouseSensitivityVertical;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Handle horizontal rotation (player & camera)
        playerTransform.Rotate(0, mouseDelta.x * mouseSensitivityHorizontal, 0);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
    }
}