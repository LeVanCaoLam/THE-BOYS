using System.Collections;
using UnityEngine;

public class Enemy_Attack : MonoBehaviour
{
    [SerializeField] GameObject attackTrigger;
    [SerializeField] float attackStart = 0.35f;
    [SerializeField] float attackDuring = 0.4f;

    private bool is_Attacking = false;

    void Start()
    {
        attackTrigger.SetActive(false);
    }
    public void AttackTrigger()
    {
        if (!is_Attacking)
        {
            is_Attacking = true;
            StartCoroutine(EnableTrigger());
        }
    }
    private IEnumerator EnableTrigger()
    {
        yield return new WaitForSeconds(attackStart);
        attackTrigger.SetActive(true);

        yield return new WaitForSeconds(attackDuring);
        attackTrigger.SetActive(false);

        StopCoroutine(EnableTrigger());
    }
    public void ResetAttackTrigger()
    {
        is_Attacking = false;
    }
}
