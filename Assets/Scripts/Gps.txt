using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.Android;

public class PlaceBoatOnShoreWithCompass : MonoBehaviour
{
    [SerializeField] private GameObject placedPrefab;
    private GameObject spawnedObject;

    // GPS координаты берега
    private float targetLatitude = 49.892611339570124f;
    private float targetLongitude = 10.886399591111331f;

    // Начальная точка GPS
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
            Debug.Log("GPS не включен на устройстве.");
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
            Debug.Log("Не удалось получить местоположение.");
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Не удалось определить местоположение.");
            yield break;
        }
        else
        {
            initialLatitude = Input.location.lastData.latitude;
            initialLongitude = Input.location.lastData.longitude;
            isInitialPositionSet = true;
        }
    }

    private void Update()
    {
        if (isInitialPositionSet && spawnedObject == null)
        {
            Vector3 shorePosition = CalculatePositionFromGPS(targetLatitude, targetLongitude);
            PlaceObject(shorePosition);
        }
    }

    private Vector3 CalculatePositionFromGPS(float targetLat, float targetLon)
    {
        const float EarthRadius = 6371000;
        float dLat = Mathf.Deg2Rad * (targetLat - initialLatitude);
        float dLon = Mathf.Deg2Rad * (targetLon - initialLongitude);

        float latDistance = dLat * EarthRadius;
        float lonDistance = dLon * EarthRadius * Mathf.Cos(Mathf.Deg2Rad * initialLatitude);

        float heading = Input.compass.trueHeading;

        Vector3 position = new Vector3((float)lonDistance, 0, (float)latDistance);
        Quaternion rotation = Quaternion.Euler(0, -heading, 0);

        return rotation * position;
    }

    private void PlaceObject(Vector3 position)
    {
        // Опускаем лодку на 2 метра вниз
        position.y -= 2f;

        spawnedObject = Instantiate(placedPrefab, position, Quaternion.identity);
    }

    private void OnDestroy()
    {
        Input.compass.enabled = false;
    }
}