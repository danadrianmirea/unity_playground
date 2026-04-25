using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float mouseSensitivity = 200f;
    public float moveSpeed = 10f;
    public float shiftSpeedBoost = 3.0f;

    bool freeLook = false;
    private float xRotation = 0f;
    private float yRotation = 0f;

    void Start()
    {
        SetCursor(false);
    }

    void Update()
    {
        HandleFreeLookToggle();
        HandleMouseLook();
        HandleMovement();
    }

    void HandleFreeLookToggle()
    {
        if (Input.GetMouseButtonDown(1)) // Right click pressed
        {
            freeLook = true;
            SetCursor(true);
            // Sync internal angles to the current camera rotation to avoid any snap
            Vector3 euler = transform.eulerAngles;
            xRotation = euler.x;
            yRotation = euler.y;
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
        if (!freeLook) return;

        // Using raw mouse delta – remove Time.deltaTime for frame‑rate independent look
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity; 
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
            
        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // prevent flipping

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }

    void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        float speedBoost = 1.0f;

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            speedBoost = shiftSpeedBoost;

        Vector3 move = transform.right * x + transform.forward * z;

        if (Input.GetKey(KeyCode.Space))  move += Vector3.up;
        if (Input.GetKey(KeyCode.C))      move -= Vector3.up;

        transform.position += move * moveSpeed * speedBoost * Time.deltaTime;
    }
}