using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float throwForce = 10f; // Сила броска
    public int damage = 50; // Урон, который граната наносит
    public float explosionRadius = 5f; // Радиус взрыва
    public float delay = 3f; // Задержка перед взрывом
    public string enemyTag = "Enemy"; // Тег врага (можно изменить в инспекторе)
    private bool hasExploded = false;

    void Start()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * throwForce, ForceMode.Impulse);
        Destroy(gameObject, delay);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!hasExploded)
        {
            Explode();
            hasExploded = true;
        }
    }

    void Explode()
    {
        // Наносим урон всем объектам с тегом "Enemy" в радиусе взрыва
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearbyObject in colliders)
        {
            if (nearbyObject.CompareTag(enemyTag))
            {
                // Используем SendMessage как в Bullet.cs для совместимости
                nearbyObject.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
            }
        }

        // Добавляем отдачу
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePosition;
    }
}