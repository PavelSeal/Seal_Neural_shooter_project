using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float throwForce = 10f;         // Сила броска
    public int damage = 50;                // Урон
    public float explosionRadius = 5f;     // Радиус взрыва
    public float delay = 3f;               // Задержка перед взрывом
    public string enemyTag = "Enemy";      // Тег врага
    public ParticleSystem explosionEffect; // Эффект взрыва (частицы)

    // Звуковые компоненты
    public AudioSource audioSource;        // Источник звука
    public AudioClip explosionSound;       // Звук взрыва

    private bool hasExploded = false;
    private float destroyTime;             // Время, когда граната должна исчезнуть

    void Start()
    {
        // Бросок гранаты
        GetComponent<Rigidbody>().AddForce(transform.forward * throwForce, ForceMode.Impulse);

        // Запоминаем, когда граната должна исчезнуть (если не взорвётся раньше)
        destroyTime = Time.time + delay;

        // Проверка и инициализация AudioSource
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }

    void Update()
    {
        // Если граната не взорвалась, но время вышло — взрываем
        if (!hasExploded && Time.time >= destroyTime)
        {
            Explode();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Взрыв при столкновении (если ещё не взорвалась)
        if (!hasExploded)
        {
            Explode();
        }
    }

    void Explode()
    {
        if (hasExploded) return;  // Защита от повторного вызова
        hasExploded = true;

        // Отключаем физику и визуал перед удалением
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.constraints = RigidbodyConstraints.FreezeAll;

        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        // Воспроизведение звука взрыва
        if (audioSource != null && explosionSound != null)
        {
            audioSource.PlayOneShot(explosionSound);
        }

        // Создаём эффект взрыва (если есть префаб)
        if (explosionEffect != null)
        {
            ParticleSystem explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            explosion.Play();
            Destroy(explosion.gameObject, explosion.main.duration);
        }

        // Наносим урон врагам в радиусе
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearbyObject in colliders)
        {
            if (nearbyObject.CompareTag(enemyTag))
            {
                nearbyObject.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
            }
        }

        // Удаляем гранату после завершения звука взрыва
        Destroy(gameObject, explosionSound != null ? explosionSound.length : 0f);
    }
}