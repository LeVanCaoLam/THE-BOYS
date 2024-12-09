using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPCScript : MonoBehaviour
{
    public GameObject NPCpanel;
    public TextMeshProUGUI NPCcontent;
    public List<HoiThoai> hoiThoais = new List<HoiThoai>();
    private Coroutine currentCoroutine;
    public Button skip;

    private int currentIndex = 0;
    private bool isCompleteDialogue = false;

    void Start()
    {
        NPCpanel.SetActive(false);
        NPCcontent.text = "";
    }

    private void Update()
    {
        if (NPCpanel.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            if (isCompleteDialogue)
            {
                ContinueToNextDialogue();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            NPCpanel.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            currentIndex = 0;
            currentCoroutine = StartCoroutine(ReadContent());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            NPCpanel.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }

            currentIndex = 0;
        }
    }

    private void ContinueToNextDialogue()
    {
        currentIndex++;
        if (currentIndex < hoiThoais.Count)
        {
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            currentCoroutine = StartCoroutine(ReadContent());
        }
        else
        {
            NPCpanel.SetActive(false);
        }
    }

    private IEnumerator ReadContent()
    {
        isCompleteDialogue = false;
        var currentHoiThoai = hoiThoais[currentIndex];

        // Sử dụng Rich Text để đảm bảo font chữ
        string colorTag = currentHoiThoai.doiTuong == "Player" ? "green" : "white";
        NPCcontent.text = $"<color={colorTag}><b>{currentHoiThoai.doiTuong}:</b> ";

        // Đọc từng ký tự của nội dung
        string fullContent = currentHoiThoai.noiDung;
        foreach (var character in fullContent)
        {
            NPCcontent.text += character;
            yield return new WaitForSeconds(0.05f);
        }
        NPCcontent.text += "</color>";

        isCompleteDialogue = true;
    }

    public void End()
    {
        NPCpanel.SetActive(false);
        currentIndex = 0;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
    }

    [Serializable]
    public class HoiThoai
    {
        public string doiTuong;
        [TextArea] public string noiDung;
    }
}