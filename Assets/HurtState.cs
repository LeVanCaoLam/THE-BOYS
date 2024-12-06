using UnityEngine;
using UnityEngine.AI;

public class HurtState : StateMachineBehaviour
{
    private NavMeshAgent navMeshAgent;
    private Enemy_Health enemy_Health;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        navMeshAgent = animator.GetComponent<NavMeshAgent>();
        enemy_Health = animator.GetComponent<Enemy_Health>();

        if (navMeshAgent != null)
        {
            navMeshAgent.isStopped = true;
        }

        animator.ResetTrigger("isHurt");
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (navMeshAgent != null)
        {
            navMeshAgent.isStopped = false;
        }

        // Mở khoá tấn công
        if (enemy_Health != null)
        {
            enemy_Health.UnlockAttack();
        }
    }
}
