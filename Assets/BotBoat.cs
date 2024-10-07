using UnityEngine;

public class BotBoat : MonoBehaviour
{
    public float moveSpeed = 550f;  // Speed of the second boat
    public float rotationSpeed = 100f;  // Speed of the rotation for the second boat
    public Vector3 movementAreaMin;  // Minimum boundary for random movement
    public Vector3 movementAreaMax;  // Maximum boundary for random movement
    public float stuckThreshold = 2f;  // Minimum distance before changing target
    public float avoidStuckDistance = 0.5f;  // Distance to ensure the new target is far enough away

    private Vector3 randomTargetPosition;  // The target position the boat moves to
    private float stuckCheckTime = 0.1f;  // Time to recheck for getting stuck
    private float stuckCheckTimer;

    private Rigidbody rb;  // Rigidbody for the boat's movement

    private void Start()
    {
        // Assign the Rigidbody
        rb = GetComponent<Rigidbody>();

        // Set Rigidbody's collision detection mode to Continuous
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Set an initial random target position within the movement area
        SetRandomTargetPosition();
        stuckCheckTimer = stuckCheckTime;  // Initialize timer
    }

    private void SetRandomTargetPosition()
    {
        // Generate a random target position within the defined movement area and ensure it's far enough away
        Vector3 newTarget;
        do
        {
            newTarget = new Vector3(
                Random.Range(movementAreaMin.x, movementAreaMax.x),
                0,
                Random.Range(movementAreaMin.z, movementAreaMax.z)
            );
        } while (Vector3.Distance(transform.position, newTarget) < avoidStuckDistance);  // Ensure it's far enough away

        randomTargetPosition = newTarget;
    }

    private void Update()
    {
        MoveRandomly();
    }

    private void MoveRandomly()
    {
        // Move toward the random target position
        Vector3 direction = (randomTargetPosition - transform.position).normalized;
        rb.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);  // Use Rigidbody's MovePosition for physics-based movement

        // Rotate the boat toward the movement direction
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime));
        }

        // If the boat reaches the target position, set a new random target
        if (Vector3.Distance(transform.position, randomTargetPosition) < stuckThreshold)
        {
            SetRandomTargetPosition();
        }

        // Periodically recheck if the boat is getting stuck
        stuckCheckTimer -= Time.deltaTime;
        if (stuckCheckTimer <= 0)
        {
            stuckCheckTimer = stuckCheckTime;  // Reset timer
            if (Vector3.Distance(transform.position, randomTargetPosition) < stuckThreshold)
            {
                SetRandomTargetPosition();  // Set a new random target if it's stuck
            }
        }
    }

    // // Detect collisions with other objects
    // private void OnCollisionEnter(Collision collision)
    // {
    //     if (collision.gameObject.CompareTag("MyBoat"))
    //     {
    //         // Handle collision with the player boat if needed (e.g., bounce back or change target)
    //         SetRandomTargetPosition();  // Choose a new random position to avoid getting stuck
    //     }
    // }
}
