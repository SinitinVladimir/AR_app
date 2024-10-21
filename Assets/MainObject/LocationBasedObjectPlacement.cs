using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections;
using System.Collections.Generic;


[RequireComponent(typeof(ARRaycastManager))]
public class LocationBasedObjectPlacement : MonoBehaviour
{
    public GameObject placeablePrefab; // Object to be placed once location is validated
    private ARRaycastManager raycastManager; // Reference to AR raycasting manager
    private bool canPlaceObject = false; // Boolean to control object placement availability

    // Define the GPS target coordinates
    private double targetLatitude = 49.909471503057965;
    private double targetLongitude = 10.915336876204577;
    private bool gpsLocationConfirmed = false; // Tracks if the user is in the allowed location

    private void Start()
    {
        raycastManager = GetComponent<ARRaycastManager>();

        // Start GPS coroutine
        StartCoroutine(StartGPS());
    }

    private IEnumerator StartGPS()
    {
        // Ensure GPS permissions are granted
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("GPS not enabled. Please enable location services.");
            yield break;
        }

        // Start the GPS service
        Input.location.Start();

        // Wait until GPS service is initialized
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Handle GPS initialization failure
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Unable to determine device location.");
            yield break;
        }
        else
        {
            Debug.Log("GPS location acquired.");

            // Confirm if the user is within the allowed location range
            while (!gpsLocationConfirmed)
            {
                double currentLatitude = Input.location.lastData.latitude;
                double currentLongitude = Input.location.lastData.longitude;

                // Calculate distance to the target location
                double distance = CalculateDistance(currentLatitude, currentLongitude, targetLatitude, targetLongitude);

                if (distance <= 10.0)
                {
                    Debug.Log("User is within the allowed location range.");
                    gpsLocationConfirmed = true;
                    canPlaceObject = true;
                }
                else
                {
                    Debug.Log($"User is {distance} meters away from the target location.");
                }

                // Recheck location every 5 seconds
                yield return new WaitForSeconds(5);
            }
        }

        Input.location.Stop();
    }

    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        // Haversine formula to calculate the distance between two latitude and longitude coordinates
        double R = 6371e3;  // Earth's radius in meters
        double dLat = (lat2 - lat1) * Mathf.Deg2Rad;
        double dLon = (lon2 - lon1) * Mathf.Deg2Rad;
        double a = Mathf.Sin((float)dLat / 2) * Mathf.Sin((float)dLat / 2) +
                   Mathf.Cos((float)lat1 * Mathf.Deg2Rad) * Mathf.Cos((float)lat2 * Mathf.Deg2Rad) *
                   Mathf.Sin((float)dLon / 2) * Mathf.Sin((float)dLon / 2);
        double c = 2 * Mathf.Atan2(Mathf.Sqrt((float)a), Mathf.Sqrt(1 - (float)a));
        return R * c;
    }

    private void Update()
    {
        if (canPlaceObject && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Vector2 touchPosition = Input.GetTouch(0).position;

            if (raycastManager.Raycast(touchPosition, new List<ARRaycastHit>(), UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
            {
                // Place object
                Pose hitPose = new List<ARRaycastHit>()[0].pose;
                Instantiate(placeablePrefab, hitPose.position, hitPose.rotation);

                // Disable object placement after placement is completed
                canPlaceObject = false;
                Debug.Log("Object placed successfully.");
            }
        }
    }
}
