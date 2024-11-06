using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.Android;

public class SequentialARPlacement : MonoBehaviour
{
    [SerializeField] private GameObject placedPrefab;  // Prefab to place on the plane
    private GameObject spawnedObject;

    private ARTrackedImageManager imageManager;        // Manager for image tracking
    private ARRaycastManager raycastManager;           // Manager for plane detection
    private Vector2 targetLocation = new Vector2(49.909466496245116f, 10.915336912735034f); // Target GPS coordinates

    private bool isLocationVerified = false;           // Location verified flag
    private bool isImageRecognized = false;            // Image recognition flag
    private bool isPlaneDetected = false;              // Plane detection flag
    private float currentLatitude, currentLongitude;   // Current user location

    private bool isGpsInitialized = false;             // Flag to check if GPS has been initialized
    private float checkInterval = 15f;                 // Interval to check distance (in seconds)
    private float nextCheckTime = 0f;                  // Next check time
    private bool lightAdjusted = false;                // Flag to ensure light adjustment happens only once

    private void Awake()
    {
        // Initialize AR components
        imageManager = FindObjectOfType<ARTrackedImageManager>();
        raycastManager = FindObjectOfType<ARRaycastManager>();

        // Set up image tracking event
        imageManager.trackedImagesChanged += OnTrackedImagesChanged;

        // Request GPS permissions
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }

        Input.compass.enabled = true; // Enable compass
        StartCoroutine(StartGPS());   // Start GPS coroutine
    }

    private IEnumerator StartGPS()
    {
        Debug.Log("Starting GPS...");

        // Initialize GPS service
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("GPS is not enabled by the user.");
            yield break;
        }

        Input.location.Start();
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            Debug.Log("Initializing GPS...");
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait < 1 || Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Failed to determine device location.");
            yield break;
        }

        isGpsInitialized = true;
        Debug.Log("GPS initialized successfully.");
    }

    private void Update()
    {
        // Step 1: Update location periodically and check verification
        if (Time.time >= nextCheckTime && isGpsInitialized && !isLocationVerified)
        {
            UpdateCurrentLocation();
            VerifyLocation();
            nextCheckTime = Time.time + checkInterval;
        }

        // Step 2: Enable image tracking only after location verification and if not already enabled
        if (isLocationVerified && !isImageRecognized && !imageManager.enabled)
        {
            Debug.Log("Enabling image tracking...");
            EnableImageTracking();
        }

        // Step 3: Enable plane detection only after image recognition
        if (isImageRecognized && !isPlaneDetected)
        {
            Debug.Log("Attempting to detect a plane...");
            DetectPlane();
        }
    }

    private void UpdateCurrentLocation()
    {
        // Continuously update the user's current latitude and longitude
        currentLatitude = Input.location.lastData.latitude;
        currentLongitude = Input.location.lastData.longitude;
        Debug.Log($"Updated Location: Latitude {currentLatitude}, Longitude {currentLongitude}");
    }

    private void VerifyLocation()
    {
        // Check the user's distance from the target location using updated coordinates
        Vector2 currentLocation = new Vector2(currentLatitude, currentLongitude);
        float distanceToTarget = HaversineDistance(currentLocation, targetLocation);

        Debug.Log("Distance to target: " + distanceToTarget + " meters");

        if (distanceToTarget <= 100f)
        {
            Debug.Log("Location verified, enabling image tracking.");
            isLocationVerified = true;
        }
    }

    private float HaversineDistance(Vector2 point1, Vector2 point2)
    {
        const float EarthRadius = 6371000; // in meters
        float dLat = Mathf.Deg2Rad * (point2.x - point1.x);
        float dLon = Mathf.Deg2Rad * (point2.y - point1.y);

        float a = Mathf.Sin(dLat / 2) * Mathf.Sin(dLat / 2) +
                  Mathf.Cos(Mathf.Deg2Rad * point1.x) * Mathf.Cos(Mathf.Deg2Rad * point2.x) *
                  Mathf.Sin(dLon / 2) * Mathf.Sin(dLon / 2);

        float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
        return EarthRadius * c; // Distance in meters
    }

    private void EnableImageTracking()
    {
        if (!lightAdjusted) // Ensure light adjustment happens only once
        {
            Debug.Log("Adjusting light sensitivity...");
            AdjustLightSensitivity();
            lightAdjusted = true;
        }

        // Enable image tracking only if it is not already enabled
        if (!imageManager.enabled)
        {
            Debug.Log("ImageManager is being enabled...");
            imageManager.enabled = true;  // Enable image tracking
        }
        else
        {
            Debug.LogWarning("ImageManager was already enabled!");
        }
    }

    private void AdjustLightSensitivity()
    {
        RenderSettings.ambientIntensity = 0.5f; // Adjust light for better image recognition in low light
        Debug.Log("Light sensitivity adjusted for low-light conditions.");
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        // Verify that the target image is detected
        foreach (var trackedImage in eventArgs.added)
        {
            Debug.Log($"Processing tracked image with name: {trackedImage.referenceImage.name}, state: {trackedImage.trackingState}");

            if (trackedImage.referenceImage.name == "YourImageName" && trackedImage.trackingState == TrackingState.Tracking && !isImageRecognized && isLocationVerified)
            {
                Debug.Log("Image recognized! Enabling plane detection.");
                isImageRecognized = true; // Mark image as recognized

                // Move directly to plane detection
                DetectPlane();

                // Disable ImageManager to avoid multiple recognitions
                imageManager.enabled = false;

                return; // Exit from the loop immediately once an image is recognized
            }
        }
    }

    private void DetectPlane()
    {
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (raycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.Planes))
        {
            Debug.Log("Plane detected, tap to place object.");
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                PlaceObject(hits[0].pose.position); // Place the object at the detected plane's position
                DisableAllTracking(); // Turn off tracking features to prevent interference
            }
        }
    }

    private void PlaceObject(Vector3 position)
    {
        position.y = -2f; // Set desired altitude
        spawnedObject = Instantiate(placedPrefab, position, Quaternion.identity);
        Debug.Log("Object placed on plane.");
    }

    private void DisableAllTracking()
    {
        // Disable tracking components after placement
        Input.compass.enabled = false;
        raycastManager.enabled = false;
        Debug.Log("Tracking disabled to enable object interaction.");
    }

    private void OnDestroy()
    {
        // Clean up image tracking event and compass
        imageManager.trackedImagesChanged -= OnTrackedImagesChanged;
        Input.compass.enabled = false;
    }
}
