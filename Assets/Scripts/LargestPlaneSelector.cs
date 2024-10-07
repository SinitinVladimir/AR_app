using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class LargestPlaneSelector : MonoBehaviour
{
    public ARPlaneManager planeManager;

    void Update()
    {
        ARPlane largestPlane = null;
        float largestArea = 0f;

        // Проходим по всем обнаруженным плоскостям
        foreach (var plane in planeManager.trackables)
        {
            float planeArea = plane.size.x * plane.size.y;

            // Если плоскость больше предыдущей найденной, запоминаем её
            if (planeArea > largestArea)
            {
                largestArea = planeArea;
                largestPlane = plane;
            }
        }

        // Отключаем все плоскости, кроме самой большой
        foreach (var plane in planeManager.trackables)
        {
            if (plane != largestPlane)
            {
                plane.gameObject.SetActive(false);
            }
        }
    }
}
