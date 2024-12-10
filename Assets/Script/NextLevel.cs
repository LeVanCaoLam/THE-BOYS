using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    [SerializeField] private int indexLevel;
    [SerializeField] AudioSource teleportSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerControll playerControll = FindFirstObjectByType<PlayerControll>();

            if (playerControll != null)
            {
                playerControll.SetHurtState(true);
            }

            teleportSource.PlayOneShot(teleportSource.clip);
            // Lưu giá trị coin
            SaveCoinCount();

            Invoke(nameof(NextLevelBelongTime), 1f);
        }
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

    void NextLevelBelongTime()
    {
        SceneManager.LoadScene(indexLevel);
    }
}
