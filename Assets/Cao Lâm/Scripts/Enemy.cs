using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform target; // Mục tiêu để theo dõi
    [SerializeField] private float detectionRadius = 10f; // Bán kính phát hiện mục tiêu
    [SerializeField] private float attackRadius = 1.5f; // Bán kính để tấn công
    [SerializeField] private float maxPatrolDistance = 50f; // Khoảng cách tối đa enemy có thể di chuyển
    [SerializeField] private Vector3 originalPosition; // Vị trí ban đầu
    [SerializeField] private Animator animator;

    public enum EnemyState
    {
        Idle,       // Trạng thái nghỉ ngơi
        Chase,      // Đuổi theo
        Attack,     // Tấn công
        Die         // Chết
    }

    public EnemyState currentState = EnemyState.Idle;
    public int maxHP = 50;
    public int currentHP;

    private bool canAttack = true;
    private float attackCooldown = 1.5f;
    private float lastAttackTime;

    void Start()
    {
        currentHP = maxHP;
        originalPosition = transform.position;
        agent.stoppingDistance = attackRadius;
    }

    void Update()
    {
        if (currentState == EnemyState.Die)
            return;

        float distanceToTarget = Vector3.Distance(target.position, transform.position);
        float distanceToOriginal = Vector3.Distance(originalPosition, transform.position);

        UpdateState(distanceToTarget, distanceToOriginal);
    }

    void UpdateState(float distanceToTarget, float distanceToOriginal)
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                HandleIdleState(distanceToTarget);
                break;
            case EnemyState.Chase:
                HandleChaseState(distanceToTarget);
                break;
            case EnemyState.Attack:
                HandleAttackState(distanceToTarget);
                break;
        }
    }

    void HandleIdleState(float distanceToTarget)
    {
        // Đảm bảo set giá trị float chính xác
        animator.SetFloat("run", 0f); // Chú ý chữ hoa

        if (distanceToTarget <= detectionRadius)
        {
            ChangeState(EnemyState.Chase);
        }
    }

    void HandleChaseState(float distanceToTarget)
    {
        if (distanceToTarget > maxPatrolDistance)
        {
            agent.SetDestination(originalPosition);
            animator.SetFloat("run", 1f); // Chú ý chữ hoa
        }
    }

    void HandleAttackState(float distanceToTarget)
    {
        agent.ResetPath();
        animator.SetFloat("run", 0);

        if (distanceToTarget > attackRadius)
        {
            ChangeState(EnemyState.Chase);
            return;
        }

        if (CanPerformAttack())
        {
            PerformAttack();
        }
    }

    bool CanPerformAttack()
    {
        return Time.time >= lastAttackTime + attackCooldown;
    }

    void PerformAttack()
    {
        animator.SetTrigger("attack");
        lastAttackTime = Time.time;
    }

    public void NewEvent()
    {
        // Được gọi từ animation event để reset trạng thái attack
        if (currentState == EnemyState.Attack)
        {
            ChangeState(EnemyState.Chase);
        }
    }

    public void ChangeState(EnemyState newState)
    {
        if (currentState == EnemyState.Die)
            return;

        currentState = newState;

        switch (newState)
        {
            case EnemyState.Die:
                animator.SetTrigger("die");
                agent.ResetPath();
                Destroy(gameObject, 2f);
                break;
        }
    }

    public void TakeDamage(int damage)
    {
        if (currentState == EnemyState.Die)
            return;

        currentHP -= damage;
        currentHP = Mathf.Max(0, currentHP);

        if (currentHP <= 0)
        {
            ChangeState(EnemyState.Die);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
           TakeDamage(20);
        }
    }
}