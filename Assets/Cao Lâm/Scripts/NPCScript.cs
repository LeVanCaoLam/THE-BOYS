using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NPCScript : MonoBehaviour
{
    public GameObject npcPanel;
    public TextMeshProUGUI npcKnight;
    public TextMeshProUGUI npcMage;
    public TextMeshProUGUI npcRouge;
    public TextMeshProUGUI npcRougeHood;

    [System.Serializable]
    public class DialogueSet
    {
        public string tag;
        public string[] dialogueTexts;
    }

    public DialogueSet[] dialogueSets;

    private Coroutine currentCoroutine;
    public Button skip;
    public Button close; // Thêm nút đóng

    private int currentIndex = 0;
    private bool isCompleteDialogue = false;
    private TextMeshProUGUI currentTextMesh;
    private string currentTag; // Lưu tag NPC hiện tại

    void Start()
    {
        npcPanel.SetActive(false);
        ResetAllTextMeshes();

        if (skip != null)
        {
            skip.onClick.AddListener(SkipDialogue);
        }

        // Thêm sự kiện đóng cho nút close
        if (close != null)
        {
            close.onClick.AddListener(CloseDialogue);
        }
    }

    void ResetAllTextMeshes()
    {
        npcKnight.text = "";
        npcMage.text = "";
        npcRouge.text = "";
        npcRougeHood.text = "";
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DialogueSet matchedSet = System.Array.Find(dialogueSets, set => set.tag == transform.tag);

            if (matchedSet != null)
            {
                StartDialogue(transform.tag, matchedSet.dialogueTexts);
            }
        }
    }

    // Thêm phương thức OnTriggerExit để đóng dialogue
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CloseDialogue();
        }
    }

    void StartDialogue(string tag, string[] texts)
    {
        // Trước khi bắt đầu, reset các text mesh khác
        ResetAllTextMeshes();

        currentTag = tag;
        currentTextMesh = GetTextMeshByTag(tag);

        if (currentTextMesh != null)
        {
            npcPanel.SetActive(true);
            currentIndex = 0;
            isCompleteDialogue = false;

            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }

            currentCoroutine = StartCoroutine(TypeDialogue(texts));
        }
    }

    IEnumerator TypeDialogue(string[] texts)
    {
        while (currentIndex < texts.Length)
        {
            currentTextMesh.text = "";
            foreach (char letter in texts[currentIndex])
            {
                if (isCompleteDialogue) break;

                currentTextMesh.text += letter;
                yield return new WaitForSeconds(0.05f);
            }

            yield return new WaitUntil(() => isCompleteDialogue);

            currentIndex++;
            isCompleteDialogue = false;
        }

        EndDialogue();
    }

    void SkipDialogue()
    {
        if (!isCompleteDialogue)
        {
            if (currentTextMesh != null)
            {
                currentTextMesh.text = dialogueSets[System.Array.FindIndex(dialogueSets, set => set.tag == currentTag)].dialogueTexts[currentIndex];
            }
            isCompleteDialogue = true;
        }
        else
        {
            isCompleteDialogue = true;
        }
    }

    // Thêm phương thức CloseDialogue
    void CloseDialogue()
    {
        npcPanel.SetActive(false);
        ResetAllTextMeshes();

        // Dừng coroutine nếu đang chạy
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
    }

    void EndDialogue()
    {
        npcPanel.SetActive(false);
        ResetAllTextMeshes();
    }

    TextMeshProUGUI GetTextMeshByTag(string tag)
    {
        switch (tag)
        {
            case "Knight": return npcKnight;
            case "Mage": return npcMage;
            case "Rouge": return npcRouge;
            case "RougeHood": return npcRougeHood;
            default: return null;
        }
    }
}