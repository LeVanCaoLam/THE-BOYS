using UnityEngine;
using UnityEngine.UI;

public class Enemy_Health : MonoBehaviour
{
    [SerializeField] float currentHealth;
    [SerializeField] float maxHealth = 40f;
    [SerializeField] Image healthImage;
    [SerializeField] Image backgroundImage;
    [SerializeField] GameObject enemyMany;

    [SerializeField] AudioSource hitFromPlayer;

    private Animator animator;
    private bool isLockAttack = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        UpdateHealhHUD();
    }

    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);
        UpdateHealhHUD();

        isLockAttack = true;

        if (currentHealth > 0)
        {
            if (animator != null)
            {
                animator.SetBool("isAtkPlayer", false);

                animator.SetTrigger("isHurt");
            }
        }
        else if (currentHealth <= 0)
        {
            animator.SetBool("isDead", true);
            Destroy(gameObject, 1f);
            Destroy(enemyMany, 1f);
        }
    }
    void UpdateHealhHUD()
    {
        if (healthImage != null)
        {
            healthImage.fillAmount = currentHealth / maxHealth;
        }
    }

    void LateUpdate()
    {
        if (backgroundImage != null)
        {
            backgroundImage.transform.LookAt(Camera.main.transform);
            backgroundImage.transform.Rotate(0f, 180f, 0f);
        }

        if (healthImage != null)
        {
            healthImage.transform.LookAt(Camera.main.transform);
            healthImage.transform.Rotate(0f, 180f, 0f);
        }
    }

    // Nhận sát thương khi Player va chạm
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            hitFromPlayer.PlayOneShot(hitFromPlayer.clip);
            TakeDamage(15f); // Gây 10 sát thương, có thể thay đổi giá trị này
        }
    }

    public void UnlockAttack()
    {
        isLockAttack = false;
    }
    public bool GetLockAttack()
    {
        return isLockAttack; 
    }
}
