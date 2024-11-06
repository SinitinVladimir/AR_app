using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.Android;

public class PlaceBoatOnShoreWithCompass : MonoBehaviour
{
    [SerializeField] private GameObject placedPrefab;
    private GameObject spawnedObject;

    // Target GPS point and radius
    private Vector2 targetPoint = new Vector2(49.89238915414971f, 10.88632968426753f);
    public float spawnRadius = 100f;
    public float checkRadius = 100f;

    private float initialLatitude;
    private float initialLongitude;

    private bool isInitialPositionSet = false;
    private ARRaycastManager raycastManager;

    private void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();

        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }

        Input.compass.enabled = true;
        StartCoroutine(StartGPS());
    }

    IEnumerator StartGPS()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("GPS is not enabled on the device.");
            yield break;
        }

        Input.location.Start();

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait < 1)
        {
            Debug.Log("Unable to get location.");
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Failed to determine location.");
            yield break;
        }
        else
        {

            initialLatitude = Input.location.lastData.latitude;
            initialLongitude = Input.location.lastData.longitude;
            isInitialPositionSet = true;
            StartCoroutine(CheckDistanceToTarget());
        }
    }

    IEnumerator CheckDistanceToTarget()
    {
        while (spawnedObject == null)
        {
            if (isInitialPositionSet)
            {
                initialLatitude = Input.location.lastData.latitude;
                initialLongitude = Input.location.lastData.longitude;

                Vector2 currentLocation = new Vector2(initialLatitude, initialLongitude);
                float distanceToTarget = CalculateDistance(currentLocation, targetPoint);

                Debug.Log("Distance to target: " + distanceToTarget + " meters.");

                if (distanceToTarget <= spawnRadius)
                {
                    Vector3 position = new Vector3(0, -2f, 0);
                    PlaceObject(position);
                    yield break;
                }
                else if (distanceToTarget <= checkRadius)
                {
                    yield return new WaitForSeconds(3f);
                }
                else
                {
                    yield break;
                }
            }
            else
            {
                yield return null;
            }
        }
    }

    private float CalculateDistance(Vector2 point1, Vector2 point2)
    {
        const float EarthRadius = 6371000;
        float dLat = Mathf.Deg2Rad * (point2.x - point1.x);
        float dLon = Mathf.Deg2Rad * (point2.y - point1.y);

        float lat1Rad = Mathf.Deg2Rad * point1.x;
        float lat2Rad = Mathf.Deg2Rad * point2.x;

        float a = Mathf.Sin(dLat / 2) * Mathf.Sin(dLat / 2) +
                  Mathf.Sin(dLon / 2) * Mathf.Sin(dLon / 2) * Mathf.Cos(lat1Rad) * Mathf.Cos(lat2Rad);
        float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));

        return EarthRadius * c;
    }

    private void PlaceObject(Vector3 position)
    {
        spawnedObject = Instantiate(placedPrefab, position, Quaternion.identity);
        Debug.Log("Object placed successfully.");
    }

    private void OnDestroy()
    {
        Input.compass.enabled = false;
    }
}
