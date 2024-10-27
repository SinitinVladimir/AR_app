using UnityEngine;

public class BoatPositionAdjustmentWithCompass : MonoBehaviour
{
    public float distanceSouthWest = 10.0f;  // Distance to move southwest
    public float dropHeight = 2.0f;          // Height to lower the boat

    void Start()
    {
        // Enable the compass at the start
        Input.compass.enabled = true;

        // Move the boat southwest by the specified distance
        ShiftPositionSouthWestWithCompass();

        // Lower the boat by the specified height
        LowerBoat();
    }

    void ShiftPositionSouthWestWithCompass()
    {
        float targetAngleSW = 225.0f; // Target southwest angle in degrees

        // Get the current compass heading of the device
        float currentHeading = Input.compass.trueHeading;

        // Adjust the angle to point southwest based on the device's heading
        float adjustedAngle = targetAngleSW - currentHeading;

        // Calculate the direction vector to the southwest
        Vector3 directionSouthWest = Quaternion.Euler(0, adjustedAngle, 0) * Vector3.forward;

        // Get the forward direction of the camera and set y to 0 to ignore height
        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize(); // Normalize the vector to ensure consistent movement

        // Move the boat position southwest by the specified distance
        transform.position += directionSouthWest * distanceSouthWest;
    }

    void LowerBoat()
    {
        // Adjust the boat's height downward by the dropHeight value
        transform.position = new Vector3(transform.position.x, transform.position.y - dropHeight, transform.position.z);
    }

    void OnDestroy()
    {
        // Disable the compass when the object is destroyed to save power
        Input.compass.enabled = false;
    }
}
