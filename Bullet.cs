using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 10;
    public float lifetime = 3f;
    public string enemyTag = "Enemy"; // Тег врага (можно изменить в инспекторе)

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(enemyTag))
        {
            // Пытаемся вызвать TakeDamage любым способом
            collision.gameObject.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
        }
        Destroy(gameObject);
    }
}

// Интерфейс для объектов, которые могут получать урон
public interface ITakeDamage
{
    void TakeDamage(int damage);
}