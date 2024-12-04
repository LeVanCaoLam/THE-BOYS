using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : StateMachineBehaviour
{
    public float detectionRadius = 10f; // Bán kính phát hiện lớn
    public float chaseDistance = 40f;  // Khoảng cách tối đa để đuổi theo

    private Transform player;
    private Transform enemy;
    private Vector3 originalPosition;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Lấy Transform của player và enemy
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemy = animator.transform;

        // Lưu vị trí ban đầu
        originalPosition = enemy.position;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player != null)
        {
            // Tính khoảng cách giữa enemy và player
            float distance = Vector3.Distance(enemy.position, player.position);

            // Nếu player trong bán kính và khoảng cách cho phép, chuyển sang trạng thái "Chase"
            if (distance <= detectionRadius && distance <= chaseDistance)
            {
                animator.SetBool("isChase", true);
            }
            else
            {
                animator.SetBool("isChase", false);
                animator.SetBool("isAtkPlayer", false);
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{

    //}

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
