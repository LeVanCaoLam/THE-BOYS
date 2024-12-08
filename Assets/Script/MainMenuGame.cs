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
    public AudioSource selectSource;
    public AudioSource exitSource;
    public AudioSource choosingSource;

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
        selectSource.PlayOneShot(selectSource.clip);

        Invoke(nameof(PlayThisGame), 1f);
    }

    void PlayThisGame()
    {
        SceneManager.LoadScene(1);
        Debug.Log("Vào được game 1");
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
}
