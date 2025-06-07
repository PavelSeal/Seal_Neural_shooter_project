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

    public float searchDuration = 5f; 
    [Header("References")]
    public Animator animator;
    public Transform player;
    public LayerMask obstacleMask;

    [Header("Sound Settings")]
    public AudioClip idleSound;
    public AudioClip detectionSound;
    public AudioClip attackSound;
    public float idleSoundInterval = 5f;

    private NavMeshAgent agent;
    private float lastAttackTime;
    private bool isDead = false;
    private bool isAttacking = false;
    private float originalSpeed;
    private bool damageApplied = false;
    private Rigidbody[] ragdollRigidbodies;
    private Collider[] ragdollColliders;
    private AudioSource audioSource;
    private float nextIdleSoundTime;
    private bool playerDetected = false;
    private Vector3 lastKnownPlayerPosition;
    private float searchTimer = 0f;
    private bool isSearching = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        lastAttackTime = -attackCooldown;
        originalSpeed = agent.speed;

        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();
        SetRagdoll(false);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        nextIdleSoundTime = Time.time + idleSoundInterval;
    }

    void Update()
    {
        if (isDead) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool canSeePlayer = distanceToPlayer <= detectionRadius &&
                          !Physics.Linecast(transform.position, player.position, obstacleMask);


        if (canSeePlayer)
        {
            lastKnownPlayerPosition = player.position;
            searchTimer = 0f;
            isSearching = true;

            if (!playerDetected)
            {
                PlaySound(detectionSound);
                playerDetected = true;
            }
        }
        else if (playerDetected)
        {
            playerDetected = false;
        }

        if (!playerDetected && Time.time >= nextIdleSoundTime)
        {
            PlaySound(idleSound);
            nextIdleSoundTime = Time.time + idleSoundInterval;
        }

        if (canSeePlayer)
        {
            HandleCombat(distanceToPlayer);
        }
        else if (isSearching)
        {
            SearchForPlayer();
        }
        else
        {
            StopMovement();
            isAttacking = false;
            agent.speed = originalSpeed;
            damageApplied = false;
        }

        UpdateAnimations();
    }

    void HandleCombat(float distanceToPlayer)
    {
        bool isInAttackCooldown = Time.time - lastAttackTime < attackCooldown;

        if (isInAttackCooldown)
        {
            StopMovement();
            isAttacking = true;
            agent.speed = 0;

            if (Time.time - lastAttackTime >= attackCooldown * 0.8f && !damageApplied)
            {
                TryApplyDamage();
            }
        }
        else if (distanceToPlayer <= attackRange)
        {
            StartAttack();
        }
        else
        {
            ChasePlayer();
        }

        FacePlayer();
    }

    void SearchForPlayer()
    {

        agent.isStopped = false;
        agent.speed = originalSpeed * 0.7f; 
        agent.SetDestination(lastKnownPlayerPosition);

        if (Vector3.Distance(transform.position, lastKnownPlayerPosition) < 1f || searchTimer >= searchDuration)
        {
            isSearching = false;
            searchTimer = 0f;
        }
        else
        {
            searchTimer += Time.deltaTime;
        }

        Vector3 direction = (lastKnownPlayerPosition - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    void SetRagdoll(bool state)
    {
 
        animator.enabled = !state;

   
        foreach (var rb in ragdollRigidbodies)
        {
            rb.isKinematic = !state;
            if (rb != GetComponent<Rigidbody>())
            {
                rb.detectCollisions = state;
            }
        }

        foreach (var col in ragdollColliders)
        {
  
            if (col != GetComponent<Collider>())
            {
                col.enabled = state;
            }
        }
        GetComponent<Collider>().enabled = !state;
    }

    void StartAttack()
    {
        agent.isStopped = true;
        agent.speed = 0;
        isAttacking = true;
        lastAttackTime = Time.time;
        damageApplied = false;

        PlaySound(attackSound);
    }

    void TryApplyDamage()
    {
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            player.GetComponent<PlayerController>().TakeDamage(attackDamage);
            damageApplied = true;
        }
    }

    void ChasePlayer()
    {
        agent.isStopped = false;
        agent.speed = originalSpeed;
        agent.SetDestination(player.position);
        isAttacking = false;
        damageApplied = false;
    }

    void StopMovement()
    {
        agent.isStopped = true;
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

        SetRagdoll(true);
        foreach (var rb in ragdollRigidbodies)
        {
            rb.AddForce(Vector3.up * 2f, ForceMode.Impulse);
        }

        agent.enabled = false;
        GetComponent<Collider>().enabled = false;

        Destroy(gameObject, 5f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}