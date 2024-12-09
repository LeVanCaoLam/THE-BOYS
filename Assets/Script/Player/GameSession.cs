using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    // Các thuộc tính của người chơi
    [SerializeField] private float maxHP = 100f;
    [SerializeField] private float currentHP;
    [SerializeField] private float maxMP = 100f;
    [SerializeField] private float currentMP;

    // Cấu hình hồi phục MP
    [SerializeField] private float mpRecoveryRate = 30f; // Số MP hồi phục mỗi giây
    [SerializeField] private float mpRecoveryDelay = 0.7f; // Thời gian chờ trước khi bắt đầu hồi phục

    // Tham chiếu đến UI Image
    public Image HP_Player;
    public Image MP_Player;

    // Coroutine để quản lý việc hồi phục MP
    private Coroutine mpRecoveryCoroutine;
    private Animator animatorP;

    [Header("<------ Audio ------>")]
    [SerializeField] AudioSource hurtPlayerSource;
    [SerializeField] AudioSource playerLoudHurt;
    [SerializeField] AudioSource healSound;
    [SerializeField] AudioSource coinSound;

    [Header("--- HUD Game Over ---")]
    [SerializeField] GameObject gameOver;
    [SerializeField] Image backgroundGameOver;
    [SerializeField] TextMeshProUGUI textGameOver;

    [Header("---- HUD Collect Coin ----")]
    [SerializeField] TextMeshProUGUI coinText;
    private int coinCount = 0; // Thêm biến để theo dõi số lượng coin

    void Start()
    {
        // Ẩn trỏ chuột
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // Khởi tạo HP và MP ban đầu
        InitializePlayerStats();
        // Lấy Animator
        animatorP = GetComponent<Animator>();
    }

    void InitializePlayerStats()
    {
        // Đặt HP và MP về giá trị tối đa
        currentHP = maxHP;
        currentMP = maxMP;

        // Cập nhật UI fill amount
        UpdateHPUI();
        UpdateMPUI();
    }

    // Phương thức bắt đầu tấn công (gọi từ PlayerControll)
    public bool StartAttack(float mpCost)
    {
        // Kiểm tra xem có đủ MP không
        if (currentMP >= mpCost)
        {
            // Trừ MP ngay lập tức
            currentMP -= mpCost;
            UpdateMPUI();

            // Dừng bất kỳ coroutine hồi phục MP nào đang chạy
            if (mpRecoveryCoroutine != null)
            {
                StopCoroutine(mpRecoveryCoroutine);
            }

            return true;
        }

        Debug.Log("Không đủ MP để tấn công");
        return false;
    }

    // Phương thức kết thúc tấn công (gọi từ PlayerControll)
    public void EndAttack()
    {
        // Bắt đầu coroutine hồi phục MP
        if (mpRecoveryCoroutine != null)
        {
            StopCoroutine(mpRecoveryCoroutine);
        }
        mpRecoveryCoroutine = StartCoroutine(RecoverMPOverTime());
    }

    // Coroutine hồi phục MP theo thời gian
    private IEnumerator RecoverMPOverTime()
    {
        // Đợi delay trước khi bắt đầu hồi phục
        yield return new WaitForSeconds(mpRecoveryDelay);

        // Hồi phục MP cho đến khi đầy
        while (currentMP < maxMP)
        {
            // Tăng MP
            currentMP = Mathf.Min(maxMP, currentMP + mpRecoveryRate * Time.deltaTime);

            // Cập nhật UI
            UpdateMPUI();

            // Đợi 1 frame
            yield return null;
        }
    }

    // Cập nhật UI HP
    void UpdateHPUI()
    {
        if (HP_Player != null)
        {
            HP_Player.fillAmount = currentHP / maxHP;
        }
    }

    // Cập nhật UI MP
    void UpdateMPUI()
    {
        if (MP_Player != null)
        {
            MP_Player.fillAmount = currentMP / maxMP;
        }
    }

    // Phương thức để sử dụng MP
    public void UseMP(float amount)
    {
        currentMP = Mathf.Max(0, currentMP - amount);
        UpdateMPUI();

        if (mpRecoveryCoroutine != null)
        {
            StopCoroutine(mpRecoveryCoroutine);
        }
        mpRecoveryCoroutine = StartCoroutine(RecoverMPOverTime());
    }

    // Phương thức để hồi phục MP
    public void RestoreMP(float amount)
    {
        currentMP = Mathf.Min(maxMP, currentMP + amount);
        UpdateMPUI();
    }

    public void HealHP(float amounts)
    {
        currentHP = Mathf.Min(maxHP, currentHP + amounts);
        UpdateHPUI();
    }

    public float CurrentMP
    {
        get { return currentMP; }
    }

    public void TakeDamaged(float damage)
    {
        currentHP = Mathf.Max(0, currentHP - damage);
        animatorP.SetTrigger("Hurt");
        UpdateHPUI();

        PlayerControll playerController = GetComponent<PlayerControll>();
        if (playerController != null)
        {
            if (playerController != null)
            {
                playerController.SetHurtState(true);
            }
            StartCoroutine(EnablePlayerAfterHurt());
        }

        if (currentHP <= 0)
        {
            Debug.Log("Player đã died");
            animatorP.SetBool("Dead", true);

            TriggerGameOverScreen(); // hiệu ứng rõ dần của UI game over

            // Tắt script và collider
            CharacterController characterController = GetComponent<CharacterController>();
            if (characterController != null)
            {
                characterController.enabled = false;
            }

            // Tắt hẳn PlayerControll
            playerController.StopAllActions();
            playerController.enabled = false;

            // Tắt Rigidbody và Capsule Collider
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true; // Ngăn mọi tương tác vật lý
            }

            CapsuleCollider collider = GetComponent<CapsuleCollider>();
            if (collider != null)
            {
                collider.enabled = false; // Tắt collider
            }

            enabled = false; // Tắt GameSession
        }
        else
        {
            // Kích hoạt lại sau hoạt ảnh bị thương
            StartCoroutine(EnablePlayerAfterHurt());
        }
    }

    private IEnumerator EnablePlayerAfterHurt()
    {
        yield return new WaitForSeconds(animatorP.GetCurrentAnimatorStateInfo(0).length);

        PlayerControll playerController = GetComponent<PlayerControll>();

        if (playerController != null)
        {
            playerController.SetHurtState(false); // Cho phép điều khiển lại
            playerController.EnableControll();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyAttack"))
        {
            hurtPlayerSource.PlayOneShot(hurtPlayerSource.clip);
            playerLoudHurt.PlayOneShot(playerLoudHurt.clip);
            TakeDamaged(5f);
        }

        if (other.CompareTag("Heal"))
        {
            HealHP(20f);
            healSound.PlayOneShot(healSound.clip);
            Destroy(other.gameObject, 0.015f);
        }

        if (other.CompareTag("Coin"))
        {
            coinCount += 10;
            coinSound.PlayOneShot(coinSound.clip);
            coinText.text = coinCount.ToString();
            Destroy(other.gameObject, 0.015f);
        }

        if (other.CompareTag("Emerald"))
        {
            coinCount += 50;
            coinSound.PlayOneShot(coinSound.clip);
            coinText.text = coinCount.ToString();
            Destroy(other.gameObject, 0.015f);
        }

        if (other.CompareTag("Diamond"))
        {
            coinCount += 100;
            coinSound.PlayOneShot(coinSound.clip);
            coinText.text = coinCount.ToString();
            Destroy(other.gameObject, 0.015f);
        }
    }

    // hiệu ứng UI khi player chết
    public void TriggerGameOverScreen()
    {
        gameOver.SetActive(true); // Bật màn hình Game Over
        StartCoroutine(FadeInGameOverElements());
    }

    private IEnumerator FadeInGameOverElements()
    {
        float duration = 2f; // Thời gian hiệu ứng (2 giây)
        float elapsedTime = 0f;

        Color backgroundColor = backgroundGameOver.color;
        Color textColor = textGameOver.color;

        // Đặt alpha ban đầu về 0
        backgroundColor.a = 0f;
        textColor.a = 0f;

        backgroundGameOver.color = backgroundColor;
        textGameOver.color = textColor;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / duration);

            // Cập nhật alpha
            backgroundColor.a = alpha;
            textColor.a = alpha;

            backgroundGameOver.color = backgroundColor;
            textGameOver.color = textColor;

            yield return null;
        }

        // Đảm bảo alpha đạt 1 sau khi hiệu ứng kết thúc
        backgroundColor.a = 1f;
        textColor.a = 1f;

        backgroundGameOver.color = backgroundColor;
        textGameOver.color = textColor;

        // Chờ thêm 2 giây trước khi tải lại scene
        yield return new WaitForSeconds(2f);
        ReloadCurrentScene();
    }
    private void ReloadCurrentScene()
    {
        // Tải lại scene hiện tại
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}