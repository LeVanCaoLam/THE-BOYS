using UnityEngine;
using UnityEngine.AI;

public class Boss_Chase : MonoBehaviour
{
    [Header("Player References")]
    public Transform playerTransform;
    public Animator bossAnimator;

    [Header("Chase Parameters")]
    [SerializeField] private float chaseDistance = 55f;  // Khoảng cách đuổi theo
    [SerializeField] private float attackDistance = 2.5f;  // Khoảng cách tấn công
    [SerializeField] float stopDistance = 2.6f;
    [SerializeField] float speeds = 7.5f;

    private NavMeshAgent bossAgent;
    private Vector3 originPosition;  // Vị trí ban đầu

    private bool isHurt = false;

    private bool isStoppedByAttack = false; // Trạng thái dừng do tấn công

    void Start()
    {
        // Lưu vị trí ban đầu
        originPosition = transform.position;

        // Gắn NavMeshAgent
        bossAgent = GetComponent<NavMeshAgent>();
        bossAgent.speed = speeds;  // Tốc độ di chuyển
        bossAgent.stoppingDistance = stopDistance;  // Khoảng cách dừng
    }

    void Update()
    {
        // Kiểm tra xem boss còn sống không trước khi thực hiện bất kỳ hành động nào
        if (isHurt || isStoppedByAttack) return;

        if (playerTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Logic đuổi theo player
        if (distanceToPlayer <= chaseDistance && distanceToPlayer > attackDistance)
        {
            bossAgent.isStopped = false;
            bossAgent.speed = 7.5f;
            bossAgent.SetDestination(playerTransform.position);

            bossAnimator.SetBool("ChasePlayer", true);
            bossAnimator.SetBool("AttackPlayer", false);
        }
        // Logic tấn công
        else if (distanceToPlayer <= attackDistance)
        {
            // THÊM ĐIỀU KIỆN KIỂM TRA BOSS CÒN SỐNG
            if (!isHurt)
            {
                bossAgent.isStopped = true;
                bossAgent.speed = 0f;
                transform.LookAt(playerTransform);

                bossAnimator.SetBool("ChasePlayer", false);
                bossAnimator.SetBool("AttackPlayer", true);

                Boss_Attack boss_Attack = GetComponent<Boss_Attack>();

                if (boss_Attack != null && boss_Attack.CanAttack())
                {
                    boss_Attack.EnableTrigger();
                }
            }
        }
        // Quay về trạng thái Idle
        else
        {
            bossAgent.isStopped = false;
            bossAgent.speed = 7.5f;
            bossAgent.SetDestination(originPosition);

            bossAnimator.SetBool("ChasePlayer", false);
            bossAnimator.SetBool("AttackPlayer", false);
        }
    }

    // Hàm dừng di chuyển
    public void StopMovement()
    {
        if (bossAgent != null && bossAgent.isActiveAndEnabled) // Kiểm tra xem bossAgent có phải là active và được kích hoạt không
        {
            isStoppedByAttack = true;
            bossAgent.isStopped = true; // Ngăn NavMeshAgent di chuyển
            bossAgent.speed = 0f;
        }
    }

    // Hàm tiếp tục di chuyển
    public void ResumeMovement()
    {
        if (bossAgent != null && bossAgent.isActiveAndEnabled) // Kiểm tra xem bossAgent có phải là active và được kích hoạt không
        {
            isStoppedByAttack = false;
            bossAgent.isStopped = false; // Ngăn NavMeshAgent di chuyển
            bossAgent.speed = speeds;
        }
    }

    public void TakeDamageBoss()
    {
        isHurt = true;
        bossAgent.isStopped = true;
        bossAgent.speed = 0f;

        bossAnimator.SetTrigger("isInjured");

        Invoke("RecoverFromHurt", 1.5f);  // Hồi phục sau 1.5s
    }

    private void RecoverFromHurt()
    {
        isHurt = false;
        bossAgent.speed = 7.5f;
    }

    public void DieState()
    {
        isHurt = true;

        // Kiểm tra và xử lý NavMeshAgent an toàn
        if (bossAgent != null && bossAgent.isActiveAndEnabled && bossAgent.enabled)
        {
            try
            {
                bossAgent.speed = 0f;
                bossAgent.isStopped = true;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Lỗi khi dừng NavMeshAgent: " + e.Message);
            }
        }

        // Đảm bảo chắc chắn Death animation được kích hoạt
        if (bossAnimator != null)
        {
            bossAnimator.SetBool("Death", true);

            // Tắt tất cả các trạng thái khác
            bossAnimator.SetBool("AttackPlayer", false);
            bossAnimator.SetBool("ChasePlayer", false);

            // Nếu cần, có thể thêm trigger để chắc chắn
            bossAnimator.SetTrigger("Die");
        }

        this.enabled = false; // Vô hiệu hóa script để dừng logic đuổi và tấn công
    }
}
