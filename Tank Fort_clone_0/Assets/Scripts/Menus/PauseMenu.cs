using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool isGamePaused = false;        // Boolean whether game is paused
    public static bool onOtherMenu = false;         // So we do not duplicate pause menu by pressing esc when on other menus
    public GameObject pauseMenuUI;                  // Reference to Pause Menu's UI

    void Update(){
        if (Input.GetKeyDown(KeyCode.Escape) && !onOtherMenu) {
            MenuControl();
        }
    }

    void MenuControl() {
        pauseMenuUI.SetActive(!isGamePaused);
        isGamePaused = !isGamePaused;
        Time.timeScale = isGamePaused ? 0f : 1f;
    }

    // Function of ResumeButton, resumes game
    public void Resume() {
        pauseMenuUI.SetActive(false);
        isGamePaused = false;
        Time.timeScale = 1f;
    }

    public void AnotherMenu() {
        onOtherMenu = !onOtherMenu;
    }

    // Function of TitleButton, loads MainMenu Scene
    public void Title() {
        Resume();
        SceneManager.LoadScene("Menu");
    }

    // Function of QuitButton, quits game 
    public void QuitGame() {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}
