using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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

    void Start()
    {
        // Khởi tạo HP và MP ban đầu
        InitializePlayerStats();
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

    // Gửi sát thương cho Enemy
    public void DealDamageToEnemy(float damage)
    {
        Enemy_Health enemy_Health = GetComponent<Enemy_Health>();
        if (enemy_Health != null)
        {
            enemy_Health.TakeDamage(damage);
        }
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
    }

    // Phương thức để hồi phục MP
    public void RestoreMP(float amount)
    {
        currentMP = Mathf.Min(maxMP, currentMP + amount);
        UpdateMPUI();
    }

    public float CurrentMP
    {
        get { return currentMP; }
    }
}