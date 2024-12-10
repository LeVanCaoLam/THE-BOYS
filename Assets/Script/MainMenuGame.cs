using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuGame : MonoBehaviour
{
    [Header("<----- Button ----->")]
    public Button playButton;
    public Button exitButton;
    public Button clearCache;

    [Header("<----- Audio ----->")]
    public AudioSource menuAudio;
    public AudioClip menuClip;
    public AudioSource selectSource;
    public AudioSource exitSource;
    public AudioSource choosingSource;

    [SerializeField] private GameObject clearNotice;

    // Start is called before the first frame update
    void Start()
    {
        menuAudio = GetComponent<AudioSource>();
        menuClip = GetComponent<AudioClip>();

        clearNotice.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if (playButton != null)
        {
            playButton = GetComponent<Button>();
        }

        if (exitButton != null)
        {
            exitButton = GetComponent<Button>();
        }

        if (clearCache != null)
        {
            clearCache = GetComponent<Button>();
        }
    }

    public void PlayGame()
    {
        selectSource.PlayOneShot(selectSource.clip);

        Invoke(nameof(PlayThisGame), 1f);
    }

    void PlayThisGame()
    {
        // Lưu lại coin trước khi vào game
        SaveCoinCount();

        SceneManager.LoadScene(1);
        Debug.Log("Vào được game 1");
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

    public void ExitGame()
    {
        exitSource.PlayOneShot(exitSource.clip);

        Invoke(nameof(ExitThisGame), 1f);
    }

    void ExitThisGame()
    {
        Application.Quit();
        Debug.Log("Thoát game được rồi");
    }

    public void ClearAllInformation()
    {
        exitSource.PlayOneShot(exitSource.clip);
        Invoke(nameof(ClearComplete), 1f);
    }

    void ClearComplete()
    {
        PlayerPrefs.DeleteAll(); // Xóa toàn bộ dữ liệu trong PlayerPrefs
        PlayerPrefs.Save();
        Debug.Log("Đã clear tất cả dữ liệu người chơi");
        clearNotice.SetActive(true);

        StartCoroutine(DisableClearNotice());
    }

    IEnumerator DisableClearNotice()
    {
        yield return new WaitForSeconds(2f);
        clearNotice.SetActive(false);

        StopCoroutine(DisableClearNotice());
    }
}
