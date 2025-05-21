using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public PlayerController playerController; // Ссылка на скрипт игрока
    public Text healthText; // Ссылка на Text элемент
    public Slider healthSlider; // Опционально: добавь слайдер для визуализации

    void Start()
    {
        // Инициализация при старте
        UpdateHealthUI();
    }

    void Update()
    {
        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        if (playerController != null)
        {
            // Обновление текста
            if (healthText != null)
            {
                healthText.text = $"{playerController.currentHealth}/{playerController.maxHealth}";
            }

            // Обновление слайдера (если используется)
            if (healthSlider != null)
            {
                healthSlider.maxValue = playerController.maxHealth;
                healthSlider.value = playerController.currentHealth;
            }
        }
    }
}