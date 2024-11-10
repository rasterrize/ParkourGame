using Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonController : MonoBehaviour
{
    private const float DEFAULT_MOUSE_SENSITIVITY = 0.25f;
    private const float DEFAULT_XROTATION_MIN_CLAMP = -90.0f;
    private const float DEFAULT_XROTATION_MAX_CLAMP = 90.0f;

    [SerializeField] private Camera playerCamera;
    private Transform playerTransform;
    [SerializeField] private float mouseSensitivityHorizontal = DEFAULT_MOUSE_SENSITIVITY;
    [SerializeField] private float mouseSensitivityVertical = DEFAULT_MOUSE_SENSITIVITY;

    private Vector2 mouseDelta;
    
    private float xRotation;
    public float XRotationMinClamp = DEFAULT_XROTATION_MIN_CLAMP;
    public float XRotationMaxClamp = DEFAULT_XROTATION_MAX_CLAMP;

    private bool allowLooking = true;
    
    private PlayerMovement playerMovement;
    
    public Camera GetCamera() => playerCamera;

    // Start is called before the first frame update
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerTransform = GetComponent<Transform>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    public void OnLookAction(InputAction.CallbackContext context)
    {
        if (!allowLooking)
            return;

        // Get the mouse delta from the input context
        mouseDelta = context.ReadValue<Vector2>();

        Look();
    }

    private void Look()
    {
        // Handle vertical rotation (camera only)
        xRotation -= mouseDelta.y * mouseSensitivityVertical;
        xRotation = Mathf.Clamp(xRotation, XRotationMinClamp, XRotationMaxClamp);
        
        // Handle horizontal rotation (player & camera)
        playerTransform.Rotate(0, mouseDelta.x * mouseSensitivityHorizontal, 0);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        playerMovement.OnLook();
    }

    public void ResetXRotationClamps()
    {
        XRotationMinClamp = DEFAULT_XROTATION_MIN_CLAMP;
        XRotationMaxClamp = DEFAULT_XROTATION_MAX_CLAMP;
    }
}