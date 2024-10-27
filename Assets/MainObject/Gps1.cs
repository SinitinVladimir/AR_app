using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.Android;

public class PlaceBoatOnShoreWithCompass : MonoBehaviour
{
    [SerializeField] private GameObject placedPrefab;  // Prefab that will be placed on the ground
    private GameObject spawnedObject;  // Instance of the placed prefab

    // Target GPS coordinates for object placement
    private float targetLatitude = 49.892611339570124f;
    private float targetLongitude = 10.886399591111331f;

    // Initial GPS coordinates when the app starts
    private float initialLatitude;
    private float initialLongitude;

    private bool isInitialPositionSet = false;

    private ARRaycastManager raycastManager;  // AR Raycast Manager for AR functionalities

    private void Awake()
    {
        // Initialize the AR Raycast Manager
        raycastManager = GetComponent<ARRaycastManager>();

        // Request GPS permission if not granted
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }

        // Enable compass and start GPS service
        Input.compass.enabled = true;
        StartCoroutine(StartGPS());
    }

    // Coroutine to start GPS service
    IEnumerator StartGPS()
    {
        // If GPS is not enabled, stop the function
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("GPS is not enabled by the user.");
            yield break;
        }

        // Start the GPS service
        Input.location.Start();

        int maxWait = 20; // Wait up to 20 seconds for GPS initialization
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // If GPS initialization takes too long, exit
        if (maxWait < 1)
        {
            Debug.Log("Timed out waiting for GPS initialization.");
            yield break;
        }

        // If GPS initialization fails, exit
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Unable to determine device location.");
            yield break;
        }
        else
        {
            // Save initial GPS coordinates after successful initialization
            initialLatitude = Input.location.lastData.latitude;
            initialLongitude = Input.location.lastData.longitude;
            isInitialPositionSet = true;
        }
    }

    private void Update()
    {
        // If initial GPS is set and object not spawned, calculate position and place object
        if (isInitialPositionSet && spawnedObject == null)
        {
            Vector3 shorePosition = CalculatePositionFromGPS(targetLatitude, targetLongitude);
            PlaceObject(shorePosition);
        }
    }

    // Calculate the position of the target based on GPS coordinates
    private Vector3 CalculatePositionFromGPS(float targetLat, float targetLon)
    {
        const float EarthRadius = 6371000;  // Earth's radius in meters

        // Calculate the distance in latitude and longitude from the initial position
        float dLat = Mathf.Deg2Rad * (targetLat - initialLatitude);
        float dLon = Mathf.Deg2Rad * (targetLon - initialLongitude);

        // Convert latitude and longitude differences to distances in meters
        float latDistance = dLat * EarthRadius;
        float lonDistance = dLon * EarthRadius * Mathf.Cos(Mathf.Deg2Rad * initialLatitude);

        // Get the device's heading to orient the placement properly
        float heading = Input.compass.trueHeading;

        // Calculate the final position
        Vector3 position = new Vector3(lonDistance, 0, latDistance);
        Quaternion rotation = Quaternion.Euler(0, -heading, 0);

        // Apply rotation to position to align with the compass direction
        return rotation * position;
    }

    // Instantiate the prefab at the calculated position
    private void PlaceObject(Vector3 position)
    {
        // Adjust height so the object appears slightly above the ground
        position.y -= 2f;

        // Instantiate the prefab at the given position with no rotation
        spawnedObject = Instantiate(placedPrefab, position, Quaternion.identity);
    }

    // Disable the compass when the object is destroyed to save battery
    private void OnDestroy()
    {
        Input.compass.enabled = false;
    }
}
