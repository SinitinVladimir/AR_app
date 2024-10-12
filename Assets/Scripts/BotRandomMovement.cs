using UnityEngine;

public class BoatAIMovement : MonoBehaviour
{
    public Transform playerBoat;       // Player's boat
    public float minSpeed = 0.1f;      // Minimum movement speed for the AI boat
    public float maxSpeed = 2.0f;      // Maximum movement speed for the AI boat
    public float distanceFactor = 0.5f; // Factor by which distance affects speed
    public float rightOffset = 6.0f;    // Offset for the AI boat to stay on the player's right side
    public float positionTolerance = 0.1f; // Allowed tolerance before updating the target position

    private Vector3 targetPos;         // Target position for the AI boat to move towards

    void Start()
    {
        // Set the initial target position for the AI boat
        SetTargetPosition();
    }

    void Update()
    {
        // Calculate the distance between the AI boat and the player's boat
        float distance = Vector3.Distance(transform.position, playerBoat.position);

        // Increase speed based on distance, but do not exceed maxSpeed
        float speed = Mathf.Min(minSpeed + distance * distanceFactor, maxSpeed);

        // Move the AI boat towards the target position at the calculated speed
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        // If the AI boat reaches the target position, update the target position
        if (Vector3.Distance(transform.position, targetPos) < positionTolerance)
        {
            SetTargetPosition();
        }
    }

    void SetTargetPosition()
    {
        // Calculate the target position to the right of the player's boat (with an offset on the X-axis)
        Vector3 offset = playerBoat.right * rightOffset;
        targetPos = playerBoat.position + offset;
    }
}
