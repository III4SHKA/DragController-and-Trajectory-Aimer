using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DragController : MonoBehaviour
{
    [SerializeField] private TrajectoryAimer2D trajectoryAimer; // Reference to trajectory visualization

    [Header("Settings")]
    [SerializeField] private float dragLimit = 3f;      // Maximum allowed drag distance
    [SerializeField] private float forceToAdd = 10f;     // Multiplier for launch force

    private Rigidbody2D rb;
    private Camera cam;
    private bool isDragging;
    private Vector3 dragStartPos;

    // Gets the current mouse position in world coordinates
    private Vector3 MousePosition
    {
        get
        {
            Vector3 pos = cam.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0; // Zero out z-axis since we're in 2D
            return pos;
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
    }

    private void Start()
    {
        // Set up Rigidbody2D physics properties
        rb.mass = 5f;
        rb.gravityScale = 0.65f;
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    private void Update()
    {
        // Start dragging on mouse press
        if (Input.GetMouseButtonDown(0) && !isDragging)
        {
            DragStart();
        }

        // Update drag if currently dragging
        if (isDragging)
        {
            Drag();
        }

        // Release drag on mouse release
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            DragEnd();
        }
    }

    // Called when the player starts dragging
    private void DragStart()
    {
        isDragging = true;
        dragStartPos = MousePosition;

        // Stop any existing movement
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    // Called while the player is dragging
    private void Drag()
    {
        Vector3 currentPos = MousePosition;
        Vector3 distance = dragStartPos - currentPos;

        // Limit the drag distance
        if (distance.magnitude > dragLimit)
            distance = distance.normalized * dragLimit;

        Vector3 finalForce = distance * forceToAdd;

        // Calculate expected velocity from force and mass
        Vector2 initialVelocity = (Vector2)(finalForce / rb.mass);

        // Visualize the trajectory
        trajectoryAimer.ShowTrajectory(rb.position, initialVelocity);
    }

    // Called when the player releases the mouse button
    private void DragEnd()
    {
        isDragging = false;

        Vector3 currentPos = MousePosition;
        Vector3 distance = dragStartPos - currentPos;

        // Limit the drag distance
        if (distance.magnitude > dragLimit)
            distance = distance.normalized * dragLimit;

        Vector3 finalForce = distance * forceToAdd;

        // Apply impulse force to Rigidbody2D
        rb.AddForce(finalForce, ForceMode2D.Impulse);

        // Hide trajectory after release
        trajectoryAimer.Hide();
    }
}
