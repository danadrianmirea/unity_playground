using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float mouseSensitivity = 200f;
    public float moveSpeed = 10f;

    bool freeLook = false;

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
        }

        if (Input.GetMouseButtonUp(1)) // Right click released
        {
            freeLook = false;
            SetCursor(false);
        }
    }

    void SetCursor(bool locked)
    {
        if (locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void HandleMouseLook()
    {
        if (!freeLook) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX, Space.World);
        transform.Rotate(Vector3.right * -mouseY, Space.Self);
    }

    void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move =
            transform.right * x +
            transform.forward * z;

        if (Input.GetKey(KeyCode.Space))
            move += Vector3.up;

        if (Input.GetKey(KeyCode.C))
            move -= Vector3.up;

        transform.position += move * moveSpeed * Time.deltaTime;
    }
}