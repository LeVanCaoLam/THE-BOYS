using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuGame : MonoBehaviour
{
    public Button playButton;
    public Button exitButton;
    public AudioSource menuAudio;
    public AudioClip menuClip;
    // Start is called before the first frame update
    void Start()
    {
        menuAudio = GetComponent<AudioSource>();
        menuClip = GetComponent<AudioClip>();
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
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Map1");
        Debug.Log("Vào game thành công");
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Đã thoát game thành công");
    }
}
