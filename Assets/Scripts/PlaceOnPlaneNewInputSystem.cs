using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARSubsystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Android;

[RequireComponent(typeof(ARRaycastManager))]
public class PlaceOnExtendedPlane : MonoBehaviour
{
    [SerializeField] private GameObject placedPrefab; // Prefab to place on detected plane
    private GameObject spawnedObject; // Reference to the spawned object
    private ARRaycastManager raycastManager; // AR Raycast Manager to detect planes
    private ARPlaneManager planeManager; // AR Plane Manager
    private ARAnchorManager anchorManager; // For anchoring the object to the detected plane
    private ARAnchor boatAnchor; // AR Anchor for the placed object

    // Defined GPS coordinates for the target water location
    // private double targetLatitude = 49.892595385461846;
    // private double targetLongitude = 10.8861954279113;

    // Dorm coordinates -- For testing purposes only -- 
    private double targetLatitude = 49.90947577042178; 
    private double targetLongitude = 10.915333022124926;

    private bool objectPlaced = false; // Track whether the object has already been placed
    private bool gpsLocationConfirmed = false; // Track whether the user is within range
    private bool isPressed; // Track touch press state
    private bool planeDetected = false; // Track whether a plane is detected
    private Vector3 lastDetectedAltitude; // Store altitude of the detected plane

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>(); // Store raycast hit results
    private TouchControls controls; // Handle touch inputs

    private void Awake()
    {
        // Initialize the AR components
        raycastManager = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();
        anchorManager = GetComponent<ARAnchorManager>();

        // Set up touch controls
        controls = new TouchControls();
        controls.control.touch.performed += _ => isPressed = true;
        controls.control.touch.canceled += _ => isPressed = false;
    }

    private IEnumerator Start()
    {
        // Start GPS service to check user location
        yield return StartCoroutine(StartGPS());
    }

    private IEnumerator StartGPS()
    {
        // Ensure GPS permissions are granted
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Requesting GPS permission");
            
            // Request GPS permission
            yield return RequestLocationPermission();

            // Check again if location is enabled after permission request
            if (!Input.location.isEnabledByUser)
            {
                Debug.Log("GPS still not enabled after requesting permission");
                yield break;
            }
        }

        // Start the GPS service
        Input.location.Start();

        int maxWait = 30; // Extend wait time for GPS initialization
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            Debug.Log("Waiting for GPS initialization...");
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Handle GPS initialization failure
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Failed to determine device location");
            yield break;
        }
        else
        {
            Debug.Log($"GPS location acquired: Latitude {Input.location.lastData.latitude}, Longitude {Input.location.lastData.longitude}");

            // Check if the user is near the target location (within 20 meters)
            gpsLocationConfirmed = IsNearTargetLocation(Input.location.lastData.latitude, Input.location.lastData.longitude);

            if (gpsLocationConfirmed)
            {
                Debug.Log("User is near the target location, object will be placed at the exact coordinates.");
            }
            else
            {
                Debug.Log("User is too far from the target location.");
            }
        }

        // Stop the GPS service if no longer needed
        Input.location.Stop();
    }

    private IEnumerator RequestLocationPermission()
    {
        // Android-specific permission request
        if (Application.platform == RuntimePlatform.Android)
        {
            // If the location permission is not granted, request it
            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                Permission.RequestUserPermission(Permission.FineLocation);

                // Wait for the permission dialog to finish
                yield return new WaitForSeconds(2);
            }
        }
    }

    private bool IsNearTargetLocation(double currentLatitude, double currentLongitude)
    {
        // Calculate the distance between the current location and the target location
        double distance = CalculateDistance(currentLatitude, currentLongitude, targetLatitude, targetLongitude);
        return distance < 20.0; // Consider the user "near" if they are within 20 meters
    }

    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        // Use the Haversine formula to calculate the distance between two GPS coordinates
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
        // Ensure the object hasn't been placed, the user is near the target, and they are interacting with the screen
        if (objectPlaced || !gpsLocationConfirmed || Pointer.current == null || !isPressed) return;

        Vector2 touchPosition = Pointer.current.position.ReadValue();

        // Use AR raycasting to detect planes
        if (raycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;
            lastDetectedAltitude = hitPose.position;  // Store the altitude of the detected plane
            PlaceObject(hitPose); // Place object on the detected plane
            planeDetected = true;
        }
        else if (!planeDetected)
        {
            // Simulate a virtual plane if no plane is detected
            Vector3 extendedPosition = SimulatePlaneOverWater(touchPosition);
            PlaceObject(new Pose(extendedPosition, Quaternion.identity));
        }
    }

    private Vector3 SimulatePlaneOverWater(Vector2 touchPosition)
    {
        // Simulate a virtual plane based on the last detected altitude
        Ray ray = Camera.main.ScreenPointToRay(touchPosition);
        Vector3 waterPlaneNormal = Vector3.up;
        Plane waterPlane = new Plane(waterPlaneNormal, lastDetectedAltitude);

        waterPlane.Raycast(ray, out float distanceToWater);
        return ray.GetPoint(distanceToWater);
    }

    private void PlaceObject(Pose pose)
    {
        if (spawnedObject == null)
        {
            // Place the object on the detected or simulated plane and move it to the exact coordinates
            spawnedObject = Instantiate(placedPrefab, pose.position, pose.rotation);
            MoveToExactCoordinates();
            DisablePlaneDetection();
        }
        else
        {
            // Update object position and rotation
            spawnedObject.transform.position = pose.position;
            spawnedObject.transform.rotation = pose.rotation;
        }

        // Ensure the object faces the camera for user interaction
        Vector3 lookPos = Camera.main.transform.position - spawnedObject.transform.position;
        lookPos.y = 0;
        spawnedObject.transform.rotation = Quaternion.LookRotation(lookPos);
    }

    private void MoveToExactCoordinates()
    {
        // Convert GPS coordinates to Unity's world space and move the object there
        Vector3 precisePosition = ConvertGPSToUnityCoords(targetLatitude, targetLongitude);
        spawnedObject.transform.position = precisePosition;

        // Anchor the object to prevent it from drifting
        boatAnchor = anchorManager.AttachAnchor(spawnedObject.GetComponent<ARPlane>(), new Pose(precisePosition, Quaternion.identity));
    }

    private Vector3 ConvertGPSToUnityCoords(double latitude, double longitude)
    {
        // // Reference coordinates for the river
        // double originLatitude = 49.892595385461846;
        // double originLongitude = 10.8861954279113;

        // Dorm coordinates -- For testing purposes only -- 
        double originLatitude = 49.90947577042178;  // Latitude for testing
        double originLongitude = 10.915333022124926;  // Longitude for testing

        double earthRadius = 6371000;  // Earth's radius in meters

        // Calculate the difference in meters between the current position and the reference position
        double latDiff = latitude - originLatitude;
        double lonDiff = longitude - originLongitude;

        double latMeters = latDiff * (Mathf.PI / 180) * earthRadius;
        double lonMeters = lonDiff * (Mathf.PI / 180) * earthRadius * Mathf.Cos((float)latitude * Mathf.Deg2Rad);

        float scale = 0.1f;  // Scale factor for Unity's world space

        return new Vector3((float)lonMeters * scale, 0, (float)latMeters * scale); // Return Unity world position
    }

    private void DisablePlaneDetection()
    {
        // Disable further plane detection once the object has been placed
        objectPlaced = true;
        raycastManager.enabled = false;

        if (planeManager != null)
        {
            planeManager.enabled = false;
            foreach (var plane in planeManager.trackables)
            {
                plane.gameObject.SetActive(false);
            }
        }
    }

    private void OnEnable()
    {
        controls.control.Enable(); // Enable touch controls
    }

    private void OnDisable()
    {
        controls.control.Disable(); // Disable touch controls
    }
}
