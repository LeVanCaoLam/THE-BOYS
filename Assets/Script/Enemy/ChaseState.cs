using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseState : StateMachineBehaviour
{
    public float maxChaseDistance = 40f;  // Khoảng cách tối đa đuổi theo
    public float stopChaseDistance = 8f;  // Khoảng cách dừng đuổi và chuyển sang bắn

    private Transform player;
    private Transform enemy;
    public Vector3 originalPosition;
    private NavMeshAgent navMeshAsuna;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemy = animator.transform;
        originalPosition = enemy.position;

        navMeshAsuna = enemy.GetComponent<NavMeshAgent>();
        navMeshAsuna.isStopped = false;
        navMeshAsuna.updateRotation = true; // Cho phép NavMesh tự xoay hướng
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player != null && navMeshAsuna != null)
        {
            float distanceToPlayer = Vector3.Distance(enemy.position, player.position);
            float distanceToOrigin = Vector3.Distance(enemy.position, originalPosition);

            // Nếu player còn trong phạm vi đuổi
            if (distanceToPlayer <= maxChaseDistance)
            {
                // Nếu đến gần đủ để bắn
                if (distanceToPlayer <= stopChaseDistance)
                {
                    navMeshAsuna.isStopped = true;
                    animator.SetBool("isChase", false);
                    animator.SetBool("isAtkPlayer", true);
                }
                else
                {
                    // Còn xa thì tiếp tục đuổi
                    navMeshAsuna.isStopped = false;
                    navMeshAsuna.SetDestination(player.position); // NavMesh sẽ tự xoay hướng về phía player
                    animator.SetBool("isChase", true);
                    animator.SetBool("isAtkPlayer", false);
                }
            }
            else
            {
                // Quay về vị trí ban đầu nếu đuổi quá xa
                navMeshAsuna.SetDestination(originalPosition); // NavMesh sẽ tự xoay hướng về vị trí ban đầu

                if (distanceToOrigin <= 0.0000000001f)
                {
                    navMeshAsuna.isStopped = true;
                    animator.SetBool("isChase", false);
                }
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (navMeshAsuna != null)
        {
            navMeshAsuna.ResetPath();
            navMeshAsuna.isStopped = false;
        }

        animator.SetBool("isChase", false);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
