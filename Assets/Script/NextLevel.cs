using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class NextLevel : MonoBehaviour
{
    [SerializeField] private int nextSceneIndex;
    [SerializeField] private float transitionDelay = 1f; // Thời gian delay trước khi chuyển scene
    [SerializeField] AudioSource portalSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            portalSound.PlayOneShot(portalSound.clip);
            StartCoroutine(LoadNextSceneWithDelay());
        }
    }

    private IEnumerator LoadNextSceneWithDelay()
    {
        // Có thể thêm hiệu ứng fade out ở đây nếu muốn
        yield return new WaitForSeconds(transitionDelay);
        SceneManager.LoadScene(nextSceneIndex);
    }
}
