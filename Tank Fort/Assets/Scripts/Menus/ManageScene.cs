using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManageScene : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;



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
