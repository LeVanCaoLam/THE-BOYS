using UnityEngine;
using System.Collections;

public class Boss_Attack : MonoBehaviour
{
    [Header("<---- Trigger Attack Player ---->")]
    public MeshCollider meshCollider;

    [Header("<---- Attack Configuration ---->")]
    [SerializeField] private float triggerStart = 0.2f;
    [SerializeField] private float triggerActiveTime = 0.3f; // Thời gian kích hoạt trigger mặc định
    [SerializeField] private float cooldownTime = 0.5f; // Thời gian hồi chiêu

    private bool isAttacking = false;
    private bool isOnCooldown = false;

    void Start()
    {
        // Đảm bảo trigger ban đầu bị tắt
        if (meshCollider != null)
        {
            meshCollider.enabled = false;
        }
    }

    // Phương thức public để gọi từ Animator
    public void EnableTrigger()
    {
        // Kiểm tra nếu đang trong trạng thái hồi chiêu hoặc đang tấn công
        // HOẶC boss đã chết
        if (isOnCooldown || isAttacking) return;

        StartCoroutine(ManageTrigger());
    }

    private IEnumerator ManageTrigger()
    {
        yield return new WaitForSeconds(triggerStart);

        // Bắt đầu trạng thái tấn công
        isAttacking = true;

        // Bật trigger
        if (meshCollider != null)
        {
            meshCollider.enabled = true;
        }

        // Đợi thời gian trigger hoạt động
        yield return new WaitForSeconds(triggerActiveTime);

        // Tắt trigger
        if (meshCollider != null)
        {
            meshCollider.enabled = false;
        }

        // Bắt đầu thời gian hồi chiêu
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldownTime);

        // Kết thúc hồi chiêu
        isOnCooldown = false;
        isAttacking = false;
    }

    // Phương thức ngắt trigger khẩn cấp (ví dụ như khi chết)
    public void DisableTrigger()
    {
        // Dừng mọi coroutine đang chạy
        StopAllCoroutines();

        // Tắt trigger
        if (meshCollider != null)
        {
            meshCollider.enabled = false;
        }

        // Đặt lại trạng thái
        isAttacking = false;
        isOnCooldown = false;
    }

    // Phương thức cho phép tuỳ chỉnh thời gian trigger
    public void SetTriggerTiming(float startTime, float activeTime, float cooldown)
    {
        startTime = triggerStart;
        triggerActiveTime = activeTime;
        cooldownTime = cooldown;
    }

    // Kiểm tra xem có thể tấn công không
    public bool CanAttack()
    {
        return !isAttacking && !isOnCooldown;
    }
}