using UnityEngine;

public class MedicineBox : MonoBehaviour
{
    public int healthToRestore = 20; // Количество восстанавливаемого здоровья

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, что объект - игрок
        if (other.CompareTag("Player"))
        {
            // Получаем компонент PlayerController у игрока
            PlayerController player = other.GetComponent<PlayerController>();

            if (player.currentHealth < player.maxHealth)
            {
                // Пополняем здоровье, но не превышаем максимум
                player.currentHealth = Mathf.Min(player.currentHealth + healthToRestore, player.maxHealth);

                // Уничтожаем объект с аптечкой
                Destroy(gameObject);
            }
        }
    }
}