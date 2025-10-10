using UnityEngine;

public class cameraMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 10f;         // Normal movement speed
    public float fastSpeed = 20f;         // When holding Shift
    public float lookSensitivity = 2f;    // Mouse look sensitivity

    private float rotationX = 0f;
    private float rotationY = 0f;
    private bool isLooking = false;

    void Start()
    {
        // Initialize rotation from current transform
        Vector3 euler = transform.rotation.eulerAngles;
        rotationX = euler.y;
        rotationY = euler.x;
    }

    void Update()
    {
        HandleLook();
        HandleMovement();
    }

    void HandleLook()
    {
        if (Input.GetMouseButtonDown(1)) // Right mouse pressed
        {
            isLooking = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (Input.GetMouseButtonUp(1)) // Right mouse released
        {
            isLooking = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (isLooking)
        {
            float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

            rotationX += mouseX;
            rotationY -= mouseY;
            rotationY = Mathf.Clamp(rotationY, -90f, 90f); // Prevent flipping

            transform.rotation = Quaternion.Euler(rotationY, rotationX, 0f);
        }
    }

    void HandleMovement()
    {
        float speed = Input.GetKey(KeyCode.LeftShift) ? fastSpeed : moveSpeed;

        Vector3 move = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) move += transform.forward;
        if (Input.GetKey(KeyCode.S)) move -= transform.forward;
        if (Input.GetKey(KeyCode.A)) move -= transform.right;
        if (Input.GetKey(KeyCode.D)) move += transform.right;

        if (Input.GetKey(KeyCode.E)) move += transform.up;   // Up
        if (Input.GetKey(KeyCode.Q)) move -= transform.up;   // Down

        transform.position += move.normalized * speed * Time.deltaTime;
    }
}
