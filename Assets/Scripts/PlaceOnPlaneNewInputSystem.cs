using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class PlaceOnExtendedPlane : MonoBehaviour
{
    [SerializeField] private GameObject placedPrefab;
    private GameObject spawnedObject;
    private ARRaycastManager raycastManager;
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private Vector3 lastPlanePosition;
    private bool isPlaneFound = false;

    private TouchControls controls;
    private bool isPressed;

    private bool objectPlaced = false; // New flag to track if object has been placed

    private void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();

        controls = new TouchControls();
        controls.control.touch.performed += _ => isPressed = true;
        controls.control.touch.canceled += _ => isPressed = false;
    }

    private void Update()
    {
        if (objectPlaced)  // Skip placement logic if the object is already placed
            return;

        if (Pointer.current == null || !isPressed)
            return;

        var touchPosition = Pointer.current.position.ReadValue();

        // Check for plane hit
        if (raycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;
            lastPlanePosition = hitPose.position; // Store last plane position
            isPlaneFound = true;

            PlaceObject(hitPose);
        }
        else if (isPlaneFound)
        {
            // Extend the plane over water if the plane was found previously
            Vector3 extendedPosition = ExtendPlaneOverWater(touchPosition);
            PlaceObject(new Pose(extendedPosition, Quaternion.identity));
        }
    }

    private Vector3 ExtendPlaneOverWater(Vector2 touchPosition)
    {
        // Extend the position based on water surface
        Ray ray = Camera.main.ScreenPointToRay(touchPosition);
        Vector3 waterPlaneNormal = Vector3.up; // Flat water plane normal

        // Project a plane at last known plane position
        Plane waterPlane = new Plane(waterPlaneNormal, lastPlanePosition);
        waterPlane.Raycast(ray, out float distanceToWater);

        return ray.GetPoint(distanceToWater);
    }

    private void PlaceObject(Pose pose)
    {
        if (spawnedObject == null)
        {
            // Place the object for the first time
            spawnedObject = Instantiate(placedPrefab, pose.position, pose.rotation);

            // Disable plane detection after placing the object
            DisablePlaneDetection();
        }
        else
        {
            // Update object position and rotation if already placed
            spawnedObject.transform.position = pose.position;
            spawnedObject.transform.rotation = pose.rotation;
        }

        // Make the object look at the camera
        Vector3 lookPos = Camera.main.transform.position - spawnedObject.transform.position;
        lookPos.y = 0;
        spawnedObject.transform.rotation = Quaternion.LookRotation(lookPos);
    }

    private void DisablePlaneDetection()
    {
        // Disable further plane detection and object placement
        objectPlaced = true;
        raycastManager.enabled = false;

        // Optionally disable all existing planes (invisible)
        ARPlaneManager planeManager = GetComponent<ARPlaneManager>();
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
        controls.control.Enable();
    }

    private void OnDisable()
    {
        controls.control.Disable();
    }
}
