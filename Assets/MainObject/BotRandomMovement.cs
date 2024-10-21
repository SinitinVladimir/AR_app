using UnityEngine;

public class BotBoatMovement : MonoBehaviour
{
    public Transform playerBoat;            // Player's boat to follow
    public float followDistance = 1f;     // Desired distance to maintain from the player's boat
    public float moveSpeed = 4f;          // Movement speed for the AI boat
    public float rotationSpeed = 40f;      // Speed of rotation for the AI boat

    private Rigidbody rb;                   // Rigidbody component for physics-based movement
    private Vector3 targetPosition;         // The target position for the bot to move towards

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        SetTargetPosition(); // Set the initial target position
    }

    private void Update()
    {
        FollowPlayerBoat();
    }

    private void FollowPlayerBoat()
    {
        // Calculate the target position behind the player boat
        Vector3 offset = -playerBoat.forward * followDistance;
        targetPosition = playerBoat.position + offset;

        // Smoothly move towards the target position
        Vector3 direction = (targetPosition - transform.position).normalized;
        Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        rb.MovePosition(newPosition);

        // Smoothly rotate towards the player boat
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            Quaternion smoothRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            rb.MoveRotation(smoothRotation);
        }
    }

    private void SetTargetPosition()
    {
        // Calculate the initial target position behind the player boat
        Vector3 offset = -playerBoat.forward * followDistance;
        targetPosition = playerBoat.position + offset;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerBoat"))
        {
            Debug.Log("Collision detected with PlayerBoat.");

            // Apply a gentle force to separate the boats upon collision
            Vector3 collisionDirection = (transform.position - collision.transform.position).normalized;
            rb.AddForce(collisionDirection * moveSpeed, ForceMode.Impulse);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerBoat"))
        {
            Debug.Log("Staying in collision with PlayerBoat.");

            // Reduce velocity during collision to avoid erratic behavior
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
