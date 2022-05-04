using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManaging : MonoBehaviour
{
    public Animator transition;
    public float transitionTIme = 1f;

    public void PlayGame() {
        StartCoroutine(Loading(SceneManager.GetActiveScene().buildIndex + 1));
    }


    IEnumerator Loading(int levelIndex) {
        // Play Animation
        transition.SetTrigger("Start");
        // Wait
        yield return new WaitForSeconds(1);
        // Load Scene
        SceneManager.LoadScene(levelIndex);
    }

    public void QuitGame() {
        Debug.Log("Quitted Game");
        Application.Quit();
    }
}
