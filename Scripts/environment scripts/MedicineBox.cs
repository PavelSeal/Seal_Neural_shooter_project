using UnityEngine;

public class MedicineBox : MonoBehaviour
{
    public int healthToRestore = 20;

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {

            PlayerController player = other.GetComponent<PlayerController>();

            if (player.currentHealth < player.maxHealth)
            {
                player.currentHealth = Mathf.Min(player.currentHealth + healthToRestore, player.maxHealth);
                Destroy(gameObject);
            }
        }
    }
}