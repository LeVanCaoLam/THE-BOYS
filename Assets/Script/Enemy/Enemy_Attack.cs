using System.Collections;
using UnityEngine;

public class Enemy_Attack : MonoBehaviour
{
    [SerializeField] GameObject attackTrigger;
    [SerializeField] Transform target;
    [SerializeField] float distanceAtk = 2.7f;

    private GameSession playerSession;
    private Animator animator;
    private bool isAttacking;

    void Start()
    {
        attackTrigger.SetActive(false);
        isAttacking = false;

        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }

        playerSession = target.GetComponent<GameSession>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (target != null)
        {
            // Chỉ điều chỉnh trục Y để quay mặt về phía Player
            Vector3 directionToPlayer = target.position - transform.position;
            directionToPlayer.y = 0; // Giữ cố định trục X
            transform.rotation = Quaternion.LookRotation(directionToPlayer);

            float distanceToPlayer = Vector3.Distance(transform.position, target.position);

            if (distanceToPlayer <= distanceAtk && !isAttacking)
            {
                StartCoroutine(EnableAtkTrigger());
            }
        }
    }

    private IEnumerator EnableAtkTrigger()
    {
        isAttacking = true;

        // Chạy animation Attack
        if (animator != null)
        {
            animator.SetTrigger("isAtkPlayer");
        }

        yield return new WaitForSeconds(0.6f); // Đợi trước khi bật trigger
        attackTrigger.SetActive(true);

        yield return new WaitForSeconds(0.6f); // Kích hoạt trigger trong khoảng thời gian
        attackTrigger.SetActive(false);

        yield return new WaitForSeconds(0.6f); // Đợi sau khi tấn công xong
        isAttacking = false;
    }
}
