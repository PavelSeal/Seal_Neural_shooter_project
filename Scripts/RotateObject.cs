using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed = 30f; // Скорость вращения в градусах в секунду

    void Update()
    {
        // Вращаем объект по всем осям с одинаковой скоростью
        transform.Rotate(
            rotationSpeed * Time.deltaTime, // X
            rotationSpeed * Time.deltaTime, // Y
            rotationSpeed * Time.deltaTime  // Z
        );
    }
}