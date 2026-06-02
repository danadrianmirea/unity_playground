using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float shiftSpeedBoost = 3.0f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public float mouseSensitivity = 200f;

    private bool freeLook = true;
    private bool skipFirstMouseFrame = false;
    private float xRotation;
    private float yRotation;
    private float verticalVelocity;
    private CharacterController controller;
    private Transform cam;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = GetComponentInChildren<Camera>().transform;

        yRotation = transform.eulerAngles.y;
        xRotation = cam.localEulerAngles.x;

        SetCursor(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            freeLook = false;
            SetCursor(false);
        }

        if (Input.GetMouseButtonDown(0) && !freeLook)
        {
            freeLook = true;
            SetCursor(true);
            skipFirstMouseFrame = true;
        }

        if (freeLook)
        {
            if (!skipFirstMouseFrame)
            {
                float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
                float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

                yRotation += mouseX;
                xRotation -= mouseY;
                xRotation = Mathf.Clamp(xRotation, -90f, 90f);

                transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
                cam.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            }
            else
            {
                skipFirstMouseFrame = false;
            }
        }

        Vector3 move = Vector3.zero;

        if (freeLook)
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            float speed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? shiftSpeedBoost : 1f);
            move = (transform.right * x + transform.forward * z) * speed;

            if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        if (controller.isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;
        move += Vector3.up * verticalVelocity;

        controller.Move(move * Time.deltaTime);
    }

    private void SetCursor(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}