using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
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