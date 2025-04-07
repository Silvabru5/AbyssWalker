using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // drag the pause menu panel from the canvas here
    private bool isPaused = false; // tracks whether the game is currently paused

    // this checks for the escape key each frame to toggle pause
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    // this hides the pause menu and resumes normal gameplay
    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    // this shows the pause menu and stops time in the game
    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    // this reloads the current scene and resumes time
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // this loads the main menu scene and resumes time
    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0); // replace with your actual main menu scene name
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }
}
