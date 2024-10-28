using UnityEngine;

public class BoatWaveMovementAndPhysics : MonoBehaviour
{
    public float waveHeight = 0.5f;      // Wave height for vertical motion
    public float waveFrequency = 1.0f;   // Frequency of the wave oscillation
    public float waveSpeed = 1.0f;       // Speed at which the wave travels
    public float tiltAngle = 5.0f;       // Maximum tilt angle of the boat

    private Vector3 startPos;            // Initial position of the boat

    private PhysicMaterial boatPhysicMaterial; // Physics material for boat

    void Start()
    {
        // Save the initial position of the boat for wave calculations
        startPos = transform.position;

        // Create a new Physic Material for the boat physics
        boatPhysicMaterial = new PhysicMaterial("BoatPhysics");

        // Set bounciness to 0 (no bouncing on collision)
        boatPhysicMaterial.bounciness = 0f;

        // Set friction for smooth sliding during collisions
        boatPhysicMaterial.dynamicFriction = 0.5f;
        boatPhysicMaterial.staticFriction = 0.5f;

        // Apply this material to all parts of the boat with colliders
        ApplyMaterialToBoatParts(transform);
    }

    void Update()
    {
        // Calculate vertical wave movement
        float waveOffset = Mathf.Sin(Time.time * waveFrequency + transform.position.x * waveSpeed) * waveHeight;

        // Calculate roll tilt (side-to-side movement)
        float rollTilt = Mathf.Sin(Time.time * waveFrequency * 0.5f + transform.position.x * waveSpeed) * tiltAngle;

        // Calculate pitch tilt (forward-backward movement)
        float pitchTilt = Mathf.Sin(Time.time * waveFrequency * 0.8f + transform.position.z * waveSpeed) * tiltAngle;

        // Update the boat's position for vertical movement (Y axis)
        transform.position = new Vector3(transform.position.x, startPos.y + waveOffset, transform.position.z);

        // Update the boat's rotation for tilt
        transform.rotation = Quaternion.Euler(pitchTilt, transform.rotation.eulerAngles.y, rollTilt);
    }

    // Recursively apply the physics material to all parts of the boat with colliders
    void ApplyMaterialToBoatParts(Transform parent)
    {
        Collider[] colliders = parent.GetComponentsInChildren<Collider>();
        foreach (var col in colliders)
        {
            col.material = boatPhysicMaterial; // Assign the physics material to each collider
        }
    }
}
