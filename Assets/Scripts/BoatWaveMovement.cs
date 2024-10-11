using UnityEngine;

public class BoatPositionAdjuster : MonoBehaviour
{
    public float downwardShift = 2.0f;  // Насколько опустить лодку вниз
    public float southwestShiftDistance = 10.0f;  // Насколько сдвинуть лодку на юго-запад

    private void Start()
    {
        // Смещаем лодку вниз на заданную величину
        Vector3 adjustedPosition = transform.position;
        adjustedPosition.y -= downwardShift;

        // Смещаем лодку на юго-запад
        Vector3 directionSouthWest = new Vector3(-1, 0, -1).normalized;
        adjustedPosition += directionSouthWest * southwestShiftDistance;

        // Обновляем позицию лодки
        transform.position = adjustedPosition;
    }
}
