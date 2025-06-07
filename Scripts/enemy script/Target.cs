using UnityEngine;

public class Target : MonoBehaviour
{
    int health = 10;

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}