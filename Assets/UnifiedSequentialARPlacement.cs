using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.Android;

public class UnifiedSequentialARPlacement : MonoBehaviour
{
    [SerializeField] private GameObject placedPrefab; // Prefab to place on the plane
    [SerializeField] private XRReferenceImageLibrary referenceLibrary; // Reference image library

    private GameObject spawnedObject;
    private ARTrackedImageManager imageManager;
    private ARRaycastManager raycastManager;
    private Vector2 targetLocation = new Vector2(49.909466496245116f, 10.915336912735034f); // Target GPS coordinates

    private bool isLocationVerified = false; // Location verified flag
    private bool isImageRecognized = false; // Image recognition flag
    private bool isPlaneDetected = false; // Plane detection flag
    private float currentLatitude, currentLongitude; // Current user location
    private bool isGpsInitialized = false; // GPS initialized flag
    private float checkInterval = 15f; // Interval to check distance (in seconds)
    private float nextCheckTime = 1f; // Next check time
    private bool lightAdjusted = false; // Ensure light adjustment happens only once

    private void Awake()
    {
        // Initialize AR components
        imageManager = gameObject.AddComponent<ARTrackedImageManager>();
        imageManager.referenceLibrary = referenceLibrary;
        imageManager.maxNumberOfMovingImages = 1; // Set the number of images to track
        imageManager.trackedImagesChanged += OnTrackedImagesChanged;

        raycastManager = FindObjectOfType<ARRaycastManager>();

        // Request GPS permissions
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }

        Input.compass.enabled = true; // Enable compass
        StartCoroutine(StartGPS()); // Start GPS coroutine
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
        // Step 1: Update location periodically and verify
        if (Time.time >= nextCheckTime && isGpsInitialized && !isLocationVerified)
        {
            UpdateCurrentLocation();
            VerifyLocation();
            nextCheckTime = Time.time + checkInterval;
        }

        // Step 2: Enable image tracking after location verification
        if (isLocationVerified && !isImageRecognized)
        {
            EnableImageTracking();
        }

        // Step 3: Enable plane detection after image recognition
        if (isImageRecognized && !isPlaneDetected)
        {
            DetectPlane();
        }
    }

    private void UpdateCurrentLocation()
    {
        // Continuously update user's current latitude and longitude
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

        if (distanceToTarget <= 15f)
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
            Debug.Log("Image tracking enabled!");
            imageManager.enabled = true; // Enable image tracking component
            AdjustLightSensitivity();
            lightAdjusted = true;
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
            if (trackedImage.referenceImage.name == "YourImageName" && !isImageRecognized && isLocationVerified)
            {
                Debug.Log("Image recognized! Enabling plane detection.");
                isImageRecognized = true; // Mark image as recognized
            }
        }

        // Check updated images (important if an image moves or updates)
        foreach (var trackedImage in eventArgs.updated)
        {
            if (trackedImage.referenceImage.name == "YourImageName" && !isImageRecognized && isLocationVerified)
            {
                Debug.Log("Image recognized! Enabling plane detection.");
                isImageRecognized = true; // Mark image as recognized
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
        imageManager.enabled = false;
        raycastManager.enabled = false;
        Debug.Log("Tracking disabled to enable object interaction.");
    }

    private void OnDestroy()
    {
        // Clean up image tracking event and compass
        if (imageManager != null)
        {
            imageManager.trackedImagesChanged -= OnTrackedImagesChanged;
        }
        Input.compass.enabled = false;
    }
}

