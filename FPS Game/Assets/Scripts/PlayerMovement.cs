using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float shiftSpeedBoost = 3.0f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    [Header("Mouse Settings")]
    public float mouseSensitivity = 200f;

    private bool freeLook = false;
    private float xRotation = 0f;      // camera pitch
    private float yRotation = 0f;      // player yaw
    private float verticalVelocity = 0f;

    private CharacterController controller;
    private Transform cameraTransform;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        // Find the child camera (assumes first child with Camera component)
        cameraTransform = GetComponentInChildren<Camera>().transform;

        // Start with cursor locked and freeLook active? 
        // To match typical FPS, we set freeLook = true and lock cursor at start.
        // But if you prefer holding right-click, comment the lines below.
        // freeLook = true;
        // SetCursor(true);

        // For now, follow original behaviour: cursor unlocked until right-click held.
        SetCursor(false);
        freeLook = false;

        // Sync angles with current transform to avoid sudden snap when first holding right-click
        yRotation = transform.eulerAngles.y;
        if (cameraTransform != null)
            xRotation = cameraTransform.localEulerAngles.x;
    }

    void Update()
    {
        HandleFreeLookToggle();

        if (freeLook)
        {
            HandleMouseLook();
            HandleMovement();
        }
    }

    void HandleFreeLookToggle()
    {
        if (Input.GetMouseButtonDown(1)) // Right click pressed
        {
            freeLook = true;
            SetCursor(true);

            // Sync current angles to avoid any snap when entering free look
            yRotation = transform.eulerAngles.y;
            if (cameraTransform != null)
                xRotation = cameraTransform.localEulerAngles.x;
        }

        if (Input.GetMouseButtonUp(1)) // Right click released
        {
            freeLook = false;
            SetCursor(false);
        }
    }

    void SetCursor(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Apply yaw rotation to the player body (only Y axis)
        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
        // Apply pitch rotation to the child camera
        if (cameraTransform != null)
            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        float speedBoost = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) ? shiftSpeedBoost : 1f;
        float currentSpeed = moveSpeed * speedBoost;

        // Movement relative to player's forward/right (horizontal only)
        Vector3 move = transform.right * x + transform.forward * z;
        move *= currentSpeed;

        // Apply gravity
        if (controller.isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f; // small downward force to keep grounded

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

        verticalVelocity += gravity * Time.deltaTime;
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);
    }
}