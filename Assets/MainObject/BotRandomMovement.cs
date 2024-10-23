using UnityEngine;

public class BotBoatMovement : MonoBehaviour
{
    public Transform playerBoat;            // Player's boat to follow
    public float moveSpeed = 4f;            // Movement speed for the bot boat
    public float rotationSpeed = 40f;       // Speed of rotation for the bot boat
    public float followDistance = 6f;       // Distance to maintain from the player boat

    private void Update()
    {
        FollowPlayerBoat();
    }

    private void FollowPlayerBoat()
    {
        // Calculate the target position behind the player boat
        Vector3 offset = -playerBoat.forward * followDistance;
        Vector3 targetPosition = playerBoat.position + offset;

        // Calculate the direction towards the target position
        Vector3 direction = (targetPosition - transform.position).normalized;

        // Move the bot boat towards the target position with the same movement logic as the player boat
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);

        // Smoothly rotate the bot boat towards the target position
        Quaternion targetRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180, 0);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
