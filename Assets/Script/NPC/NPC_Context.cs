using TMPro;
using UnityEngine;

public class NPC_Context : MonoBehaviour
{
    [SerializeField] string[] Context;
    [SerializeField] GameObject npcCanvas;
    [SerializeField] TextMeshProUGUI contentText;
    [SerializeField] TextMeshProUGUI button_E_notice;

    [SerializeField] Transform player; // Transform của Player
    [SerializeField] float interactionDistance = 3f; // Khoảng cách để kích hoạt hội thoại

    private int currentIndex = 0; // Chỉ số câu thoại hiện tại
    private bool isDialogueActive = false; // Kiểm tra trạng thái hội thoại

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (npcCanvas != null)
        {
            npcCanvas.SetActive(false); // Ẩn Canvas ban đầu
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null || npcCanvas == null || contentText == null || button_E_notice == null) return;

        // Tính khoảng cách giữa NPC và Player
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= interactionDistance) // Nếu Player ở trong khoảng cách trò chuyện
        {
            // Hiển thị thông báo nhấn E
            button_E_notice.gameObject.SetActive(true);
            button_E_notice.text = "Nhấn E để trò chuyện";

            if (Input.GetKeyDown(KeyCode.E)) // Nhấn phím "E" để bật hội thoại
            {
                if (!isDialogueActive)
                {
                    StartDialogue(); // Bắt đầu hội thoại
                }
            }
        }
        else
        {
            // Ẩn thông báo nhấn E khi Player ở xa
            button_E_notice.gameObject.SetActive(false);
        }

        // Khi hội thoại đang bật và nhấn phím Space
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            NextDialogue(); // Hiển thị câu tiếp theo
        }
    }

    void StartDialogue()
    {
        isDialogueActive = true; // Bật trạng thái hội thoại
        npcCanvas.SetActive(true); // Hiển thị Canvas
        button_E_notice.gameObject.SetActive(false); // Ẩn thông báo nhấn E khi hội thoại bắt đầu
        currentIndex = 0; // Reset về câu đầu tiên
        ShowDialogue(); // Hiển thị câu thoại đầu tiên
    }

    void ShowDialogue()
    {
        if (Context.Length > 0 && currentIndex < Context.Length)
        {
            contentText.text = Context[currentIndex]; // Hiển thị nội dung câu thoại hiện tại
        }
    }

    void NextDialogue()
    {
        currentIndex++;

        if (currentIndex < Context.Length)
        {
            ShowDialogue(); // Hiển thị câu thoại tiếp theo
        }
        else
        {
            EndDialogue(); // Kết thúc hội thoại nếu hết câu
        }
    }

    void EndDialogue()
    {
        isDialogueActive = false; // Tắt trạng thái hội thoại
        npcCanvas.SetActive(false); // Ẩn Canvas
        currentIndex = 0; // Reset về câu đầu tiên
    }
}
