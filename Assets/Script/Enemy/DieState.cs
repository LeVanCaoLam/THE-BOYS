using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class DieState : StateMachineBehaviour
{
    private NavMeshAgent navMeshAgent;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        navMeshAgent = animator.GetComponent<NavMeshAgent>();

        if (navMeshAgent != null )
        {
            navMeshAgent.isStopped = true;
        }

        // Đảm bảo huỷ mọi trạng thái trong Animator
        animator.SetBool("isChase", false);
        animator.SetBool("isAtkPlayer", false);

        Collider collider = animator.GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        // Huỷ gameObject sau thời gian đặt trước
        animator.GetComponent<Enemy_Health>()
            .StartCoroutine(DestroyEnemyAfterTime(animator.gameObject, 1.2f));
    }

    private IEnumerator DestroyEnemyAfterTime(GameObject enemy, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(enemy);
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
