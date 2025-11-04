using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class cameraMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 10f;         // Normal movement speed
    public float fastSpeed = 20f;         // When holding Shift
    public float lookSensitivity = 2f;    // Mouse look sensitivity

    [Header("Camera Boundaries")]
    public bool useBoundaries = true;     // Toggle bounds on/off
    public float minX = -50f;
    public float maxX = 50f;
    public float minY = 2f;
    public float maxY = 20f;
    public float minZ = -50f;
    public float maxZ = 50f;

    private float rotationX = 0f;
    private float rotationY = 0f;
    private bool isLooking = false;

    private GraphicRaycaster[] raycasters; // All UI raycasters in the scene

    void Start()
    {
        // Initialize rotation from current transform
        Vector3 euler = transform.rotation.eulerAngles;
        rotationX = euler.y;
        rotationY = euler.x;

        // Cache all UI raycasters (e.g. Canvas)
        raycasters = FindObjectsOfType<GraphicRaycaster>();
    }

    void Update()
    {
        HandleLook();
        HandleMovement();
        ClampPosition();
    }

    void HandleLook()
    {
        if (Input.GetMouseButtonDown(1)) // Right mouse pressed
        {
            isLooking = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Disable UI raycasts so mouse ignores buttons
            SetUIRaycastState(false);
        }
        else if (Input.GetMouseButtonUp(1)) // Right mouse released
        {
            isLooking = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Re-enable UI raycasts
            SetUIRaycastState(true);
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

    void ClampPosition()
    {
        if (!useBoundaries) return;

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        pos.z = Mathf.Clamp(pos.z, minZ, maxZ);
        transform.position = pos;
    }

    void SetUIRaycastState(bool enabled)
    {
        if (raycasters == null) return;
        foreach (var r in raycasters)
        {
            if (r != null)
                r.enabled = enabled;
        }
    }
}
