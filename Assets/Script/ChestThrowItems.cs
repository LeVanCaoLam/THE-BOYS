using UnityEngine;
using System.Collections;
using System;

public class ChestThrowItems : MonoBehaviour
{
    [Header("<<---- Item can be collected ---->>")]
    [SerializeField] GameObject[] items;

    [Header("<<---- Chest Components ---->>")]
    [SerializeField] Animator chestAnimator;
    [SerializeField] GameObject chestOpened;

    [Header("<<---- Throw Settings ---->>")]
    [SerializeField] float throwForce = 5f;
    [SerializeField] float throwHeight = 2f;
    [SerializeField] float itemSpreadRadius = 1f;

    [Header("<<--- Audio --->>")]
    [SerializeField] AudioSource chestSound;


    // Sự kiện static để theo dõi chest được tạo
    public static event Action<ChestThrowItems> OnChestSpawned;

    // Sự kiện khi chest được mở
    public event Action OnChestOpened;

    private bool isOpened = false;

    private void Awake()
    {
        // Kích hoạt sự kiện khi chest được tạo
        OnChestSpawned?.Invoke(this);
    }
    private void Start()
    {
        // Đảm bảo chest được đăng ký
        Debug.Log("Chest Spawned: " + gameObject.name);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Log để kiểm tra chi tiết
        Debug.Log("Trigger entered by: " + other.gameObject.name + " with tag: " + other.tag);

        // Thử sử dụng kiểm tra gameObject thay vì chỉ tag
        if (!isOpened && (other.CompareTag("PlayerAttack") || other.gameObject.CompareTag("Player")))
        {
            OpenChest();
        }
    }

    // Phương thức để mở chest
    public void OpenChests()
    {
        if (!isOpened)
        {
            isOpened = true;
            Debug.Log("Chest Opened!");

            // Kích hoạt sự kiện chest được mở
            OnChestOpened?.Invoke();
        }
    }

    public void TriggerChestOpen()
    {
        OpenChests();
    }

    private void OpenChest()
    {
        Debug.Log("OpenChest method called");

        isOpened = true;

        // Log chi tiết về sự kiện
        Debug.Log("OnChestOpened event listeners count: " +
            (OnChestOpened != null ? OnChestOpened.GetInvocationList().Length : 0));

        chestSound.PlayOneShot(chestSound.clip);

        // Animate chest opening
        if (chestAnimator != null)
        {
            chestAnimator.SetTrigger("OpenChest");
        }

        // Swap chest models
        if (chestOpened != null) chestOpened.SetActive(true);

        // Kích hoạt sự kiện OnChestOpened
        try
        {
            OnChestOpened?.Invoke();
            Debug.Log("OnChestOpened event invoked successfully");
        }
        catch (Exception e)
        {
            Debug.LogError("Error invoking OnChestOpened event: " + e.Message);
        }

        // Throw items
        StartCoroutine(ThrowItems());
    }

    private IEnumerator ThrowItems()
    {
        // Wait a short moment after opening
        yield return new WaitForSeconds(0.5f);

        // Throw each item
        foreach (GameObject item in items)
        {
            if (item != null)
            {
                // Instantiate item
                GameObject thrownItem = Instantiate(item, transform.position + Vector3.up, Quaternion.identity);

                // Calculate random throw direction
                Vector3 throwDirection = UnityEngine.Random.insideUnitSphere;
                throwDirection.y = Mathf.Abs(throwDirection.y) + 0.5f; // Ensure upward trajectory
                throwDirection = throwDirection.normalized;

                // Add Rigidbody if not exists
                Rigidbody rb = thrownItem.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    rb = thrownItem.AddComponent<Rigidbody>();
                }

                // Configure Rigidbody
                rb.useGravity = true;
                rb.isKinematic = false;

                // Apply throw force
                rb.AddForce(
                    throwDirection * throwForce,
                    ForceMode.Impulse
                );

                // Optional: After a short time, make item kinematic
                StartCoroutine(MakeItemKinematicAfterDelay(rb, 0.92f));
            }
        }
    }

    private IEnumerator MakeItemKinematicAfterDelay(Rigidbody rb, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        Invoke(nameof(ChestDestroy), 2f);
    }

    void ChestDestroy()
    {
        Destroy(gameObject);
    }
}