using UnityEngine;
using UnityEngine.AI;

public class Attack1State : StateMachineBehaviour
{
    public float meleeAtkDistance = 2.6f;

    private NavMeshAgent navMeshAsuna;
    private Transform player;
    private Transform enemy;
    private Enemy_Health enemy_Health;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        enemy = animator.transform;
        navMeshAsuna = enemy.GetComponent<NavMeshAgent>();
        enemy_Health = enemy.GetComponent<Enemy_Health>();

        if (navMeshAsuna != null)
        {
            navMeshAsuna.isStopped = true;
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player != null && enemy_Health != null)
        {
            float distanceToPlayer = Vector3.Distance(enemy.position, player.position);

            // Kiểm tra trạng thái khoá tấn công
            if (enemy_Health.GetLockAttack())
            {
                animator.SetBool("isAtkPlayer", false);
                animator.SetBool("isChase", true); // Quay lại trạng thái đuổi theo
                return;
            }

            // Giữ Enemy xoay mặt về phía Player
            Vector3 directionToPlayer = player.position - enemy.position;
            directionToPlayer.y = 0; // Cố định trục X
            enemy.rotation = Quaternion.LookRotation(directionToPlayer);

            if (distanceToPlayer > meleeAtkDistance)
            {
                animator.SetBool("isAtkPlayer", false);
                animator.SetBool("isChase", true);
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("isAtkPlayer", false);

        if (navMeshAsuna != null)
        {
            navMeshAsuna.isStopped = false;
            navMeshAsuna.ResetPath();
        }
    }
}
