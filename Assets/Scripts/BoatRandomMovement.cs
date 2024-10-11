using UnityEngine;

public class BoatRandomMovement : MonoBehaviour
{
    public float moveSpeed = 0.1f;       // Скорость движения лодки
    public float maxHorizontalXOffset = 2.0f; // Максимальное отклонение от начальной точки по X
    public float maxHorizontalZOffset = 1.0f; // Максимальное отклонение от начальной точки по Z

    private Vector3 startPos;            // Начальная позиция лодки
    private Vector3 targetPos;           // Целевая позиция для движения

    void Start()
    {
        // Сохраняем начальную позицию
        startPos = transform.position;
        // Устанавливаем первую случайную точку
        SetRandomTargetPosition();
    }

    void Update()
    {
        // Двигаем лодку к целевой позиции
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        // Если лодка достигла целевой позиции, устанавливаем новую
        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            SetRandomTargetPosition();
        }
    }

    void SetRandomTargetPosition()
    {
        // Рассчитываем случайное отклонение по оси X
        float randomX = Random.Range(-maxHorizontalXOffset, maxHorizontalXOffset);
        float randomZ = Random.Range(-maxHorizontalZOffset, maxHorizontalZOffset);
        // Новая целевая позиция — это смещение от начальной позиции по оси X
        targetPos = new Vector3(startPos.x + randomX, startPos.y, startPos.z + randomZ);
    }
}
