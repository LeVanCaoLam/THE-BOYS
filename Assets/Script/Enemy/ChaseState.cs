using UnityEngine;
using UnityEngine.AI;

public class ChaseState : StateMachineBehaviour
{
    public float maxChaseDistance = 40f;
    public float stopChaseDistance = 2.6f;

    private Transform player;
    private Transform enemy;
    private Vector3 originalPosition;
    private NavMeshAgent navMeshAsuna;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemy = animator.transform;
        originalPosition = enemy.position;

        navMeshAsuna = enemy.GetComponent<NavMeshAgent>();
        if (navMeshAsuna != null)
        {
            navMeshAsuna.isStopped = false;
            navMeshAsuna.updateRotation = true; // Cho phép NavMesh tự xoay hướng
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player != null && navMeshAsuna != null)
        {
            float distanceToPlayer = Vector3.Distance(enemy.position, player.position);

            if (distanceToPlayer <= maxChaseDistance)
            {
                if (distanceToPlayer <= stopChaseDistance)
                {
                    navMeshAsuna.isStopped = true;
                    animator.SetBool("isChase", false);
                    animator.SetBool("isAtkPlayer", true);
                }
                else
                {
                    // Đuổi theo Player
                    navMeshAsuna.SetDestination(player.position);
                }
            }
            else
            {
                // Quay lại vị trí ban đầu nếu Player thoát khỏi phạm vi
                navMeshAsuna.SetDestination(originalPosition);

                if (Vector3.Distance(enemy.position, originalPosition) <= 0.1f)
                {
                    navMeshAsuna.isStopped = true;
                    animator.SetBool("isChase", false);
                }
            }

            // Giữ Enemy xoay mặt về phía Player nhưng không thay đổi trục X
            Vector3 directionToPlayer = player.position - enemy.position;
            directionToPlayer.y = 0; // Cố định trục X
            enemy.rotation = Quaternion.LookRotation(directionToPlayer);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (navMeshAsuna != null)
        {
            navMeshAsuna.ResetPath();
            navMeshAsuna.isStopped = false;
        }
    }
}
