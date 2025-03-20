using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 5f; // �������� �����
    public int attackDamage = 10; // ���� �� �����
    public float attackRange = 2f; // ������ �����
    public float attackCooldown = 2f; // ����� ����� �������

    private Transform player; // ���� ������
    private bool canAttack = true; // ���� ����������� �����

    void Start()
    {
        // ����� ������ � �����
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return; // ���� ����� �� ������, ������� �� ������

        // �������� � ������
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // �������� �� ����������� �����
        if (canAttack && Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            Attack();
        }
    }

    void Attack()
    {
        canAttack = false; // ��������� ��������� ����� �� ��������� ��oldown
        Invoke("ResetAttack", attackCooldown); // ���������� ���� ����� ��oldown

    }

    void ResetAttack()
    {
        canAttack = true; // ��������� ��������� �����
    }
}

