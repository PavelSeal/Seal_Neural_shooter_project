using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    [Tooltip("Процент пополнения боеприпасов (от максимального запаса)")]
    [Range(0, 100)]
    public float ammoRefillPercent = 20f;

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, что объект - игрок (можно добавить проверку тега)
        if (other.CompareTag("Player"))
        {
            // Получаем компонент Shooting у игрока
            Shooting playerShooting = other.GetComponentInChildren<Shooting>();

            if (playerShooting != null)
            {
                // Рассчитываем количество пополняемых боеприпасов
                int ammoToAdd = Mathf.RoundToInt(playerShooting.maxAmmoInReserve * ammoRefillPercent / 100f);

                // Пополняем только если текущий запас не полный
                if (playerShooting.currentAmmoInReserve < playerShooting.maxAmmoInReserve)
                {
                    playerShooting.currentAmmoInReserve += ammoToAdd;

                    // Не даем превысить максимальный запас
                    if (playerShooting.currentAmmoInReserve > playerShooting.maxAmmoInReserve)
                    {
                        playerShooting.currentAmmoInReserve = playerShooting.maxAmmoInReserve;
                    }

                    Debug.Log($"Пополнено {ammoToAdd} патронов. Теперь в запасе: {playerShooting.currentAmmoInReserve}");

                    // Уничтожаем объект с боеприпасами
                    Destroy(gameObject);
                }
            }
        }
    }
}