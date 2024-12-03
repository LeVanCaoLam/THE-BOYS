using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Thư viện để làm việc với UI

public class MainMenu : MonoBehaviour
{
    // Tham chiếu đến AudioSource để phát nhạc nền
    public AudioSource backgroundMusic;

    // Tham chiếu đến Slider để điều chỉnh âm lượng
    public Slider volumeSlider;

    void Start()
    {
        // Phát nhạc nền nếu đã được gán
        if (backgroundMusic != null && !backgroundMusic.isPlaying)
        {
            backgroundMusic.loop = true; // Lặp lại nhạc
            backgroundMusic.Play();
        }
        else
        {
            Debug.LogWarning("Background music chưa được gán hoặc đã phát.");
        }

        // Gán giá trị khởi tạo cho slider (nếu tồn tại)
        if (volumeSlider != null)
        {
            volumeSlider.value = backgroundMusic.volume; // Đặt giá trị ban đầu cho slider
            volumeSlider.onValueChanged.AddListener(AdjustVolume); // Lắng nghe sự kiện thay đổi
        }
        else
        {
            Debug.LogWarning("Volume slider chưa được gán.");
        }
    }

    // Phương thức để điều chỉnh âm lượng
    public void AdjustVolume(float volume)
    {
        if (backgroundMusic != null)
        {
            backgroundMusic.volume = volume;
        }
    }

    // Phương thức để bắt đầu trò chơi
    public void PlayGame()
    {
        // Kiểm tra và tải cảnh "Game"
        if (Application.CanStreamedLevelBeLoaded("Game"))
        {
            SceneManager.LoadScene("Game");
        }
        else
        {
            Debug.LogError("Cảnh 'Game' chưa được thêm vào Build Settings.");
        }
    }

    // Phương thức để thoát trò chơi
    public void QuitGame()
    {
        Debug.Log("Thoát trò chơi");
        Application.Quit();
    }
}
