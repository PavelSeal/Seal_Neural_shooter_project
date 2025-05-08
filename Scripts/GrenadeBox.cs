using UnityEngine;

public class GrenadeBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, что объект - игрок
        if (other.CompareTag("Player"))
        {
            // Получаем компонент GrenadeThrower у игрока
            GrenadeThrower grenadeThrower = other.GetComponentInChildren<GrenadeThrower>();

            if (grenadeThrower.currentGrenades < grenadeThrower.maxGrenades)
            {
                grenadeThrower.currentGrenades = grenadeThrower.maxGrenades;
                Destroy(gameObject);
            }
        }
    }
}