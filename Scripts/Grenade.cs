using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float throwForce = 10f; // ���� ������
    public int damage = 50; // ����, ������� ������� �������
    public float explosionRadius = 5f; // ������ ������
    public float delay = 3f; // �������� ����� �������
    public string enemyTag = "Enemy"; // ��� ����� (����� �������� � ����������)
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
        // ������� ���� ���� �������� � ����� "Enemy" � ������� ������
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearbyObject in colliders)
        {
            if (nearbyObject.CompareTag(enemyTag))
            {
                // ���������� SendMessage ��� � Bullet.cs ��� �������������
                nearbyObject.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
            }
        }

        // ��������� ������
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePosition;
    }
}