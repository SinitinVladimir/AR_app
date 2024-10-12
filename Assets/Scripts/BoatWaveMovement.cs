using UnityEngine;

public class BoatWaveMovement : MonoBehaviour
{
    public float waveHeight = 0.5f;      // Высота волны
    public float waveFrequency = 1.0f;   // Частота волны (скорость колебания)
    public float waveSpeed = 1.0f;       // Скорость движения волны
    public float tiltAngle = 5.0f;      // Максимальный угол наклона лодки

    private Vector3 startPos;            // Начальная позиция лодки

    void Start()
    {
        // Сохраняем начальную позицию лодки
        startPos = transform.position;
    }

    void Update()
    {
        // Рассчитываем текущее покачивание по вертикали
        float waveOffset = Mathf.Sin(Time.time * waveFrequency + transform.position.x * waveSpeed) * waveHeight;

        // Рассчитываем угловое покачивание по оси Z (влево-вправо)
        float rollTilt = Mathf.Sin(Time.time * waveFrequency * 0.5f + transform.position.x * waveSpeed) * tiltAngle;

        // Рассчитываем угловое покачивание по оси X (вперед-назад)
        float pitchTilt = Mathf.Sin(Time.time * waveFrequency * 0.8f + transform.position.z * waveSpeed) * tiltAngle;

        // Обновляем позицию лодки для покачивания по оси Y (вверх-вниз)
        transform.position = new Vector3(transform.position.x, startPos.y + waveOffset, transform.position.z);

        // Обновляем вращение лодки (угловое покачивание)
        transform.rotation = Quaternion.Euler(pitchTilt, transform.rotation.eulerAngles.y, rollTilt);
    }
}
