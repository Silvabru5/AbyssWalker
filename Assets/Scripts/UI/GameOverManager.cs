using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverUI; // drag the game over panel from the canvas here
    private CanvasGroup canvasGroup; // used to control the fade-in transparency
    public float fadeDuration = 1f; // how long the fade takes

    // this runs when the script loads and sets the canvas group to fully transparent
    private void Awake()
    {
        canvasGroup = gameOverUI.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
            canvasGroup.alpha = 0f; // make sure it's invisible at the start
    }

    // this shows the game over screen and fades it in
    public void ShowGameOver()
    {
        gameOverUI.SetActive(true); // make the UI active
        Time.timeScale = 0f; // pause the game
        StartCoroutine(FadeIn()); // start the fade-in animation
    }

    // this gradually fades in the canvas group to full opacity
    private System.Collections.IEnumerator FadeIn()
    {
        float timeElapsed = 0f;

        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.unscaledDeltaTime; // keep fading even when paused
            float alpha = Mathf.Clamp01(timeElapsed / fadeDuration);
            if (canvasGroup != null)
                canvasGroup.alpha = alpha;
            yield return null;
        }

        if (canvasGroup != null)
            canvasGroup.alpha = 1f; // make sure it's fully visible at the end
    }

    // this reloads the current scene and resumes the game
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // this loads the main menu scene and resumes the game
    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0); // replace with your actual main menu scene name
    }
}
