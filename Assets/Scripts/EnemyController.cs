using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 5f; // Скорость врага
    public int attackDamage = 10; // Урон от атаки
    public float attackRange = 2f; // Радиус атаки
    public float attackCooldown = 2f; // Время между атаками

    private Transform player; // Цель игрока
    private bool canAttack = true; // Флаг возможности атаки

    void Start()
    {
        // Поиск игрока в сцене
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return; // Если игрок не найден, выходим из метода

        // Движение к игроку
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // Проверка на возможность атаки
        if (canAttack && Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            Attack();
        }
    }

    void Attack()
    {
        canAttack = false; // Запрещаем атаковать снова до окончания коoldown
        Invoke("ResetAttack", attackCooldown); // Сбрасываем флаг после коoldown

    }

    void ResetAttack()
    {
        canAttack = true; // Разрешаем атаковать снова
    }
}

