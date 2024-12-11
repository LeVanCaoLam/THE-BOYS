using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseGame : MonoBehaviour
{
    [Header("<---------- Pause Game ----------->")]
    [SerializeField] GameObject pauseCanvas;
    [SerializeField] Button resumeGame;
    [SerializeField] Button reloadGame;
    [SerializeField] Button returnMenuGame;
    [SerializeField] Button quitGame;

    [Header("------ Audio Button ------")]
    [SerializeField] AudioSource blueButton;
    [SerializeField] AudioSource redButton;
    [SerializeField] AudioSource pauseSound;

    private bool isPause = false;

    // biến kiểm tra nhấn nút
    private bool isReturning = false;
    private bool isReload = false;
    private bool isQuit = false;
    private float returnStartTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pauseCanvas.SetActive(false);
        Time.timeScale = 1.0f;

        // Ẩn và khóa trỏ chuột khi bắt đầu game
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPause) 
                ResumeGame();
            else 
                Pause();
        }

        // Xử lý hàm trở về Menu
        if (isReturning)
        {
            // So sánh thời gian thực
            if (Time.realtimeSinceStartup - returnStartTime >= 1f)
            {
                isReturning = false; // Ngừng trạng thái chuyển
                Time.timeScale = 1f; // Reset timeScale về bình thường
                SceneManager.LoadScene(0); // Chuyển về Scene 0
            }
        }

        // Xử lý hàm tải lại scene hiện tại
        if (isReload)
        {
            if (Time.realtimeSinceStartup - returnStartTime >= 1f)
            {
                isReload = false; // Ngừng trạng thái chuyển
                Time.timeScale = 1f; // Reset timeScale về bình thường
                Scene currentScene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(currentScene.name);
            }
        }

        // Xử lý hàm thoát game
        if (isQuit)
        {
            if (Time.realtimeSinceStartup - returnStartTime >= 1f)
            {
                isQuit = false; // Ngừng trạng thái chuyển
                Time.timeScale = 1f; // Reset timeScale về bình thường
                Debug.Log("Thoát game hoàn tất");
                Application.Quit();
            }
        }
    }

    private void Pause()
    {
        pauseCanvas.SetActive(true);
        Time.timeScale = 0.0f; // Dừng game
        isPause = true;

        pauseSound.PlayOneShot(pauseSound.clip);

        // Hiển thị và mở khóa trỏ chuột khi tạm dừng
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        pauseCanvas.SetActive(false);
        Time.timeScale = 1.0f;
        isPause = false;

        blueButton.PlayOneShot(blueButton.clip);

        // Ẩn và khóa trỏ chuột khi tiếp tục game
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ReloadGame()
    {
        // Lưu giá trị coin
        SaveCoinCount();

        blueButton.PlayOneShot(blueButton.clip);

        isReload = true;
        returnStartTime = Time.realtimeSinceStartup;
    }

    private void SaveCoinCount()
    {
        // Lấy giá trị coin từ GameSession
        GameSession gameSession = FindFirstObjectByType<GameSession>();
        if (gameSession != null)
        {
            int currentCoinCount = gameSession.CoinCount; // Sử dụng một thuộc tính CoinCount trong GameSession
            PlayerPrefs.SetInt("CoinCount", currentCoinCount);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.LogWarning("GameSession không được tìm thấy. Không thể lưu coin.");
        }
    }

    public void ReturnToMenuGame()
    {
        redButton.PlayOneShot(redButton.clip);

        isReturning = true;
        returnStartTime = Time.realtimeSinceStartup;
    }

    public void QuitGame()
    {
        redButton.PlayOneShot(redButton.clip);

        isQuit = true;
        returnStartTime= Time.realtimeSinceStartup;
        Debug.Log("Sau 1s sẽ thoát game");
    }
}
