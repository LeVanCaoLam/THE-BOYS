using UnityEngine;
using System.Collections;
using TMPro;

public class VictoryHUD_UI : MonoBehaviour
{
    [SerializeField] GameObject victoryUI;

    [Header("<<<--- TextMeshPro --->>>")]
    [SerializeField] TextMeshProUGUI collectText;
    [SerializeField] TextMeshProUGUI timeText;

    [Header("<<<----- Audio ----->>>")]
    [SerializeField] AudioSource backgroundMusic;
    [SerializeField] AudioSource victoryMusic;

    private GameSession gameSession;
    private float startTimer;
    private ChestThrowItems chest;

    private void Awake()
    {
        if (victoryUI != null)
        {
            victoryUI.SetActive(false);
        }

        startTimer = Time.time;
    }

    private void Start()
    {
        // Đăng ký sự kiện khi chest được tạo
        ChestThrowItems.OnChestSpawned += RegisterChestEvent;

        gameSession = FindObjectOfType<GameSession>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Thử tìm chest ngay lập tức
        TryFindChest();
    }

    private void TryFindChest()
    {
        chest = FindObjectOfType<ChestThrowItems>();
        if (chest != null)
        {
            RegisterChestEvent(chest);
        }
    }

    private void RegisterChestEvent(ChestThrowItems spawnedChest)
    {
        if (spawnedChest != null)
        {
            Debug.Log("Chest spawned, registering event: " + spawnedChest.name);
            chest = spawnedChest;
            chest.OnChestOpened += HandleChestOpened;
        }
    }

    private void HandleChestOpened()
    {
        Debug.Log("HandleChestOpened called!");

        if (victoryUI != null)
        {
            Debug.Log("Invoking ShowVictoryUI after ... seconds");
            Invoke(nameof(ShowVictoryUI), 6f);
        }
    }

    private void ShowVictoryUI()
    {
        if (victoryUI != null)
        {
            victoryUI.SetActive(true);
            Time.timeScale = 0f;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            ShowInformationVictory();

            Debug.Log("Victory UI activated");
        }
    }

    void ShowInformationVictory()
    {
        backgroundMusic.Stop();

        if (victoryMusic != null)
        {
            victoryMusic.loop = true; // Bật chế độ lặp
            victoryMusic.Play();
        }

        if (gameSession != null && collectText != null)
        {
            collectText.text = "Collected:" + " " + gameSession.CoinCount.ToString() + " Coins";
        }

        if (timeText != null)
        {
            float playTime = Time.time - startTimer;
            timeText.text = FormatTime(playTime);
        }
    }

    private string FormatTime(float totalTimer)
    {
        int minutes = Mathf.FloorToInt(totalTimer / 60);
        int seconds = Mathf.FloorToInt(totalTimer % 60);
        return string.Format("Time: {0:00}:{1:00}", minutes, seconds);
    }

    // Hủy đăng ký sự kiện khi object bị hủy
    private void OnDestroy()
    {
        // Hủy đăng ký sự kiện
        ChestThrowItems.OnChestSpawned -= RegisterChestEvent;

        if (chest != null)
        {
            chest.OnChestOpened -= HandleChestOpened;
        }
    }
}