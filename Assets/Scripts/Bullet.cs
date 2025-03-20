using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 10;
        public float lifetime = 3f; // ����� ����� ���� � ��������

    void Start()
    {
        // ���������� ���� ����� �������� �����
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        Target target = collision.gameObject.GetComponent<Target>();
        if (target != null)
        {
            target.TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}