using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float throwForce = 10f;         
    public int damage = 50;                
    public float explosionRadius = 5f;     
    public float delay = 3f;               
    public string enemyTag = "Enemy";      
    public ParticleSystem explosionEffect; 

  
    public AudioSource audioSource;      
    public AudioClip explosionSound;      

    private bool hasExploded = false;
    private float destroyTime;             

    void Start()
    {
       
        GetComponent<Rigidbody>().AddForce(transform.forward * throwForce, ForceMode.Impulse);

        destroyTime = Time.time + delay;
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

        if (!hasExploded && Time.time >= destroyTime)
        {
            Explode();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
  
        if (!hasExploded)
        {
            Explode();
        }
    }

    void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.constraints = RigidbodyConstraints.FreezeAll;

        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        if (audioSource != null && explosionSound != null)
        {
            audioSource.PlayOneShot(explosionSound);
        }

        if (explosionEffect != null)
        {
            ParticleSystem explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            explosion.Play();
            Destroy(explosion.gameObject, explosion.main.duration);
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearbyObject in colliders)
        {
            if (nearbyObject.CompareTag(enemyTag))
            {
                nearbyObject.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
            }
        }

        Destroy(gameObject, explosionSound != null ? explosionSound.length : 0f);
    }
}