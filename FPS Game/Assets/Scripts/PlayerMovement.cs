using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float shiftSpeedBoost = 3.0f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public float mouseSensitivity = 200f;

    public float fireForce = 10f;
    public float fireRange = 1000f;
    public float fireRate = 0.2f;
    public LayerMask fireLayerMask = -1;

    private bool freeLook = false;
    private float xRotation;
    private float yRotation;
    private float verticalVelocity;
    private CharacterController controller;
    private Transform cam;

    private float nextFireTime = 0f;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = GetComponentInChildren<Camera>().transform;

        yRotation = transform.eulerAngles.y;
        xRotation = cam.localEulerAngles.x;

        if (!Application.isEditor)
        {
            freeLook = true;
            SetCursor(true);
        }
    }

    private void Update()
    {
        if (freeLook && Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Fire();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            freeLook = false;
            SetCursor(false);
        }

        if (Input.GetMouseButtonDown(0) && !freeLook)
        {
            freeLook = true;
            SetCursor(true);
        }

        if (freeLook)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            yRotation += mouseX;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
            cam.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }

        Vector3 move = Vector3.zero;

        if (freeLook)
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            float speed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? shiftSpeedBoost : 1f);
            move = (transform.right * x + transform.forward * z) * speed;
        }

        if (controller.isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;

        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

        verticalVelocity += gravity * Time.deltaTime;
        move += Vector3.up * verticalVelocity;

        controller.Move(move * Time.deltaTime);
    }

    private void Fire()
    {
        Ray ray = new Ray(cam.position, cam.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, fireRange, fireLayerMask))
        {
            Debug.DrawLine(cam.position, hit.point, Color.red, 0.5f);

            Rigidbody rb = hit.rigidbody;
            if (rb != null)
            {
                rb.AddForceAtPosition(ray.direction * fireForce, hit.point, ForceMode.Impulse);
            }
        }
        else
        {
            Debug.DrawRay(cam.position, cam.forward * fireRange, Color.white, 0.2f);
        }
    }

    private void SetCursor(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}