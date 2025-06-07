using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 10;
    public float lifetime = 3f;
    public string enemyTag = "Enemy"; 

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(enemyTag))
        {
  
            collision.gameObject.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
        }
        Destroy(gameObject);
    }
}

public interface ITakeDamage
{
    void TakeDamage(int damage);
}