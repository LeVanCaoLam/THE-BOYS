using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent agent;
    [SerializeField]
    private Transform target; // Mục tiêu để theo dõi
    [SerializeField]
    private float radius = 10f; // Bán kính phát hiện mục tiêu
    [SerializeField]
    private float attackradius = 1f; // Bán kính để tấn công
    [SerializeField]
    private float maxDistance = 50f; // Khoảng cách tối đa enemy có thể di chuyển
    [SerializeField]
    private Vector3 originalPosition; // Vị trí ban đầu
    [SerializeField]
    private Animator animator;
    private bool Is_attack = false;
    public enum enemystate
    {
        Normal, Attck, Die
    }
    public enemystate currentstate;
    public int maxHP, currentHP;

    void Start()
    {
        currentHP = maxHP;
        originalPosition = transform.position;
    }
    private void FixedUpdate()
    {

    }
    void Update()
    {
        if (currentstate == enemystate.Die)
        {
            return;
        }
        var distanceOriginal = Vector3.Distance(originalPosition, transform.position);
        var distance = Vector3.Distance(target.position, transform.position);
        if (distance <= radius && distanceOriginal <= maxDistance)
        {
            agent.SetDestination(target.position);
            Is_attack = false;
            animator.SetFloat("run", 1);
        }
        else if (distanceOriginal > maxDistance || distance > radius)
        {
            agent.SetDestination(originalPosition);
            if (Vector3.Distance(transform.position, originalPosition) < 0.1f) // Nếu đã quay về vị trí ban đầu
            {
                agent.ResetPath();
                animator.SetFloat("run", 0);
            }

        }
        if (distance < 1f && !Is_attack)
        {
            attack();
            return;
        }
        //Debug.Log($"Distance: {distance}, Attack Radius: {attackradius}, Is_Attack: {Is_attack}");
    }

    void attack()
    {
        Is_attack = true;
        agent.ResetPath();
        animator.SetFloat("run", 0);
        animator.SetTrigger("attack");
        //Debug.Log("Enemy is attacking!");
    }
    public void NewEvent()
    {
        Is_attack = false;
        Debug.Log("Attack end");
    }
    public void ChangeState(enemystate newState)
    {
        switch (currentstate)
        {
            case enemystate.Normal: break;
            case enemystate.Attck: break;
            case enemystate.Die: break;
        }
        switch (newState)
        {
            case enemystate.Normal: break;
            case enemystate.Attck: break;
            case enemystate.Die:
                animator.SetTrigger("die");
                Destroy(gameObject, 2f);
                break;
        }
        currentstate = newState;
    }
    public void TakeDame(int dame)
    {
        currentHP -= dame;
        currentHP = Mathf.Max(0, currentHP);
        if (currentHP <= 0)
        {
            ChangeState(enemystate.Die);
        }
    }
}