using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Attack1State : StateMachineBehaviour
{
    public float meleeAtkDistance = 4f;      // Khoảng cách để bắn

    private NavMeshAgent navMeshAsuna;
    private Transform player;
    private Transform enemy;
    private Enemy_Health enemy_Health;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemy = animator.transform;

        navMeshAsuna = enemy.GetComponent<NavMeshAgent>();
        enemy_Health = enemy.GetComponent<Enemy_Health>();
        navMeshAsuna.isStopped = true; // Dừng di chuyển
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player != null && navMeshAsuna != null || enemy_Health != null)
        {
            float distanceToPlayer = Vector3.Distance(enemy.position, player.position);

            // Luôn quay về phía player
            Vector3 directionToPlayer = (player.position - enemy.position).normalized;
            enemy.rotation = Quaternion.LookRotation(directionToPlayer);

            // Kiểm tra có khoá tấn công ko
            if (enemy_Health.GetLockAttack())
            {
                animator.SetBool("isAtkPlayer", false);
                animator.SetBool("isChase", false);
                return;
            }

            // Nếu player còn trong phạm vi bắn
            if (distanceToPlayer <= meleeAtkDistance)
            {
                navMeshAsuna.isStopped = true;
                animator.SetBool("isAtkPlayer", true);
                animator.SetBool("isChase", false);
            }
            else
            {
                // Nếu ra ngoài phạm vi bắn thì đuổi theo
                animator.SetBool("isAtkPlayer", false);
                animator.SetBool("isChase", true);
                navMeshAsuna.isStopped = false;
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
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
