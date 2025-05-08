using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemy : MonoBehaviour
{
    [Header("Combat Settings")]
    public float detectionRadius = 10f;
    public float attackRange = 2f;
    public int attackDamage = 20;
    public float attackCooldown = 2f;
    public float rotationSpeed = 5f;
    public int Health = 50;

    [Header("References")]
    public Animator animator;
    public Transform player;
    public LayerMask obstacleMask;

    private NavMeshAgent agent;
    private float lastAttackTime;
    private bool isDead = false;
    private bool isAttacking = false;
    private float originalSpeed; // Храним оригинальную скорость

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        lastAttackTime = -attackCooldown;
        originalSpeed = agent.speed; // Сохраняем оригинальную скорость
    }

    void Update()
    {
        if (isDead) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Проверка видимости игрока
        bool canSeePlayer = distanceToPlayer <= detectionRadius &&
                            !Physics.Linecast(transform.position, player.position, obstacleMask);

        if (canSeePlayer)
        {
            // Проверяем, идет ли сейчас атака (включая время кулдауна)
            bool isInAttackCooldown = Time.time - lastAttackTime < attackCooldown;

            if (isInAttackCooldown)
            {
                // Во время атаки и кулдауна останавливаемся
                StopMovement();
                isAttacking = true;
                agent.speed = 0; // Устанавливаем скорость в 0
            }
            else if (distanceToPlayer <= attackRange)
            {
                // Атака, если в радиусе и кулдаун прошел
                Attack();
            }
            else
            {
                // Преследование игрока
                ChasePlayer();
            }

            // Поворот к игроку (даже во время атаки)
            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
        else
        {
            // Игрок не виден - idle
            StopMovement();
            isAttacking = false;
            agent.speed = originalSpeed; // Восстанавливаем скорость
        }

        UpdateAnimations();
    }

    void ChasePlayer()
    {
        agent.isStopped = false;
        agent.speed = originalSpeed; // Восстанавливаем оригинальную скорость
        agent.SetDestination(player.position);
        isAttacking = false;
    }

    void StopMovement()
    {
        agent.isStopped = true;
    }

    void Attack()
    {
        agent.isStopped = true;
        agent.speed = 0; // Устанавливаем скорость в 0

        isAttacking = true;
        lastAttackTime = Time.time;

        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            player.GetComponent<PlayerController>().TakeDamage(attackDamage);
        }
    }

    void UpdateAnimations()
    {
        animator.SetBool("IsRunning", agent.velocity.magnitude > 0.1f && !isAttacking);
        animator.SetBool("IsAttacking", isAttacking);
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        agent.isStopped = true;
        animator.SetTrigger("Die");

        // Отключаем коллайдер и NavMeshAgent
        GetComponent<Collider>().enabled = false;
        agent.enabled = false;

        // Уничтожаем объект через после смерти
        Destroy(gameObject, 5f);
    }

    void OnDrawGizmosSelected()
    {
        // Визуализация радиуса обнаружения в редакторе
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}