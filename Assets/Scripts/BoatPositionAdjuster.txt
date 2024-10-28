using UnityEngine;

public class BoatPositionAdjustmentWithCompass : MonoBehaviour
{
    public float distanceSouthWest = 10.0f;  // Смещение на юго-запад в метрах
    public float dropHeight = 2.0f;          // Смещение вниз в метрах

    void Start()
    {
        // Убедимся, что компас включен
        Input.compass.enabled = true;

        // Сдвигаем лодку на 10 метров на юго-запад с учётом ориентации устройства
        ShiftPositionSouthWestWithCompass();

        // Опускаем лодку на 2 метра вниз
        LowerBoat();
    }

    void ShiftPositionSouthWestWithCompass()
    {
        // Юго-западное направление — это 225 градусов относительно севера
        float targetAngleSW = 225.0f;

        // Получаем текущее направление устройства (компас указывает на север)
        float currentHeading = Input.compass.trueHeading;

        // Рассчитываем угол отклонения между севером и юго-западом
        float adjustedAngle = targetAngleSW - currentHeading;

        // Преобразуем этот угол в вектор направления
        Vector3 directionSouthWest = Quaternion.Euler(0, adjustedAngle, 0) * Vector3.forward;

        // Учитываем ориентацию камеры относительно мира
        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0;  // Убираем наклон по оси Y (высоте), чтобы работало только по плоскости
        cameraForward.Normalize();

        // Смещаем позицию лодки на заданное расстояние в этом направлении
        transform.position += directionSouthWest * distanceSouthWest;
    }

    void LowerBoat()
    {
        // Опускаем лодку на dropHeight вниз
        transform.position = new Vector3(transform.position.x, transform.position.y - dropHeight, transform.position.z);
    }

    void OnDestroy()
    {
        // Отключаем компас, когда лодка уничтожена
        Input.compass.enabled = false;
    }
}
