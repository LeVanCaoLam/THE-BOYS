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
            Invoke(nameof(NextLevelBelongTime), 0.3f);
        }
    }
    void NextLevelBelongTime()
    {
        SceneManager.LoadScene(indexLevel);
    }
}
