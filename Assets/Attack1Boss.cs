using UnityEngine;

public class Attack1Boss : StateMachineBehaviour
{
    // OnStateEnter: Khi bắt đầu animation Attack1
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Dừng di chuyển khi bắt đầu tấn công
        Boss_Chase bossChase = animator.GetComponent<Boss_Chase>();
        if (bossChase != null)
        {
            bossChase.StopMovement();
        }

        Boss_Attack bossAttack = animator.GetComponent<Boss_Attack>();
        if (bossAttack != null)
        {
            bossAttack.EnableTrigger(); // Bật trigger khi bắt đầu tấn công
        }
    }

    // OnStateExit: Khi kết thúc animation Attack1
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Cho phép di chuyển lại sau khi tấn công
        Boss_Chase bossChase = animator.GetComponent<Boss_Chase>();
        if (bossChase != null)
        {
            bossChase.ResumeMovement();
        }

        Boss_Attack bossAttack = animator.GetComponent<Boss_Attack>();
        if (bossAttack != null)
        {
            bossAttack.DisableTrigger(); // Tắt trigger khi kết thúc tấn công
        }
    }
}
