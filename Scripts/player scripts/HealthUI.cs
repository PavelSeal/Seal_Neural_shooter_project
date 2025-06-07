using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public PlayerController playerController; 
    public Text healthText; 
    public Slider healthSlider; 

    void Start()
    {
        
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
            
            if (healthText != null)
            {
                healthText.text = $"{playerController.currentHealth}/{playerController.maxHealth}";
            }

           
            if (healthSlider != null)
            {
                healthSlider.maxValue = playerController.maxHealth;
                healthSlider.value = playerController.currentHealth;
            }
        }
    }
}