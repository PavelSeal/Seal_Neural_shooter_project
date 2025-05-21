using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed = 30f; // �������� �������� � �������� � �������

    void Update()
    {
        // ������� ������ �� ���� ���� � ���������� ���������
        transform.Rotate(
            rotationSpeed * Time.deltaTime, // X
            rotationSpeed * Time.deltaTime, // Y
            rotationSpeed * Time.deltaTime  // Z
        );
    }
}