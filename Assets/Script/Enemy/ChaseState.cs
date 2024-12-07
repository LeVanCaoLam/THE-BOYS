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

    private bool hasReturnedToOrigin = false;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemy = animator.transform;
        originalPosition = enemy.position;

        navMeshAsuna = enemy.GetComponent<NavMeshAgent>();
        if (navMeshAsuna != null)
        {
            navMeshAsuna.isStopped = false;
            navMeshAsuna.updateRotation = true;
        }

        animator.SetBool("isChase", false); // Reset trạng thái ở đầu
        hasReturnedToOrigin = false;
    }


    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player != null && navMeshAsuna != null)
        {
            float distanceToPlayer = Vector3.Distance(enemy.position, player.position);

            if (distanceToPlayer <= maxChaseDistance)
            {
                hasReturnedToOrigin = false;

                if (distanceToPlayer >= stopChaseDistance)
                {
                    Vector3 directionToPlayer = player.position - enemy.position;
                    directionToPlayer.y = 0;
                    enemy.rotation = Quaternion.LookRotation(directionToPlayer);

                    navMeshAsuna.isStopped = false;
                    navMeshAsuna.SetDestination(player.position);
                }
                else
                {
                    navMeshAsuna.isStopped = true;
                    animator.SetBool("isChase", false);
                    animator.SetBool("isAtkPlayer", true);
                }
            }
            else
            {
                Debug.Log($"Remaining Distance: {navMeshAsuna.remainingDistance}, PathPending: {navMeshAsuna.pathPending}");
                Debug.Log($"Has Returned To Origin: {hasReturnedToOrigin}");

                if (!navMeshAsuna.pathPending && navMeshAsuna.remainingDistance <= 2.1f
                        && Vector3.Distance(enemy.position, originalPosition) <= 2.1f)
                {
                    if (!hasReturnedToOrigin)
                    {
                        navMeshAsuna.isStopped = true;
                        animator.SetBool("isChase", false);
                        hasReturnedToOrigin = true;

                        Debug.Log("Enemy has returned to origin. Setting isChase to false.");
                        enemy.rotation = Quaternion.identity;
                    }
                }
                else
                {
                    navMeshAsuna.SetDestination(originalPosition);
                }
            }
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
