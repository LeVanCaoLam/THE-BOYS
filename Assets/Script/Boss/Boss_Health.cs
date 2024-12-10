using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Boss_Health : MonoBehaviour
{
    private float maxHealth = 150f;
    private float currentHealth;

    [Header("UI Components")]
    [SerializeField] Image hp_Boss;

    [Header("<<----- Audio ----->>")]
    [SerializeField] AudioSource bossLoudHit;
    [SerializeField] AudioSource bossHit;
    [SerializeField] AudioSource bossLoudDeath;

    [Header("Boss Components")]
    [SerializeField] Boss_Chase bossChase;  // Tham chiếu đến script Boss_Chase

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHP();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            bossHit.PlayOneShot(bossHit.clip);
            TakeDamages(15f);
        }
    }

    public void TakeDamages(float damage)
    {
        currentHealth -= damage;
        UpdateHP();

        if (currentHealth > 0)
        {
            bossLoudHit.PlayOneShot(bossLoudHit.clip);
            bossChase.TakeDamageBoss();  // Gọi logic bị thương trong Boss_Chase
        }
        else
        {
            Die();
        }
    }

    private void Die()
    {
        // Dừng tất cả các hoạt động của boss ngay lập tức

        bossLoudDeath.PlayOneShot(bossLoudDeath.clip);

        Debug.Log("Boss đã chết.");

        // Vô hiệu hóa NavMeshAgent một cách an toàn
        NavMeshAgent navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent != null && navMeshAgent.isActiveAndEnabled && navMeshAgent.enabled)
        {
            try
            {
                navMeshAgent.enabled = false;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Lỗi khi vô hiệu hóa NavMeshAgent: " + e.Message);
            }
        }

        // Vô hiệu hóa BoxCollider
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            boxCollider.enabled = false;
        }

        // Vô hiệu hóa script Boss_Attack và ngắt trigger
        Boss_Attack bossAttack = GetComponent<Boss_Attack>();
        if (bossAttack != null)
        {
            bossAttack.DisableTrigger(); // Ngắt trigger ngay lập tức
            bossAttack.enabled = false; // Vô hiệu hóa script Attack
        }

        // Dừng mọi Coroutine đang chạy ở Boss_Attack nếu cần
        StopAllCoroutines();

        // Dừng trạng thái của Boss_Chase
        bossChase.DieState();
        bossChase.enabled = false;  // Vô hiệu hóa AI

        // Đảm bảo không thể tấn công
        Animator bossAnimator = GetComponent<Animator>();
        if (bossAnimator != null)
        {
            // Set cả boolean và trigger
            bossAnimator.SetBool("Death", true);
            bossAnimator.SetTrigger("Die");

            // Tắt các trạng thái Attack và Chase
            bossAnimator.SetBool("AttackPlayer", false);
            bossAnimator.SetBool("ChasePlayer", false);

            // Log để kiểm tra trạng thái
            Debug.Log("Current Animator state: " + bossAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash);
        }

        // Dừng trạng thái của Boss_Chase
        if (bossChase != null)
        {
            try
            {
                bossChase.DieState();
                bossChase.enabled = false;  // Vô hiệu hóa AI
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Lỗi khi dừng Boss_Chase: " + e.Message);
            }
        }

        // Đặt thanh máu về 0
        hp_Boss.fillAmount = 0;
    }

    private void UpdateHP()
    {
        hp_Boss.fillAmount = Mathf.Clamp(currentHealth / maxHealth, 0f, 1f);
    }
}
