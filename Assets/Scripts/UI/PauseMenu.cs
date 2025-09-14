using SuperTiled2Unity;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // drag the pause menu panel from the canvas here
    public GameObject soundMenuUI; // drag the sound menu panel from the canvas here
    public GameObject configManager;

    private bool isPaused = false; // tracks whether the game is currently paused

    // on load, read the values from the singleton that reads the config file and saves values into a dictionary then set the volumes
    private void Start()
    {
        GameObject.Find("BackgroundMusic").GetComponent<AudioSource>().volume = ConfigManager.Instance.GetInt("musicVolume") / 100f;
        GameObject.Find("SoundEffects").GetComponent<AudioSource>().volume = ConfigManager.Instance.GetInt("effectsVolume") / 100f;
    }

    // this checks for the escape key each frame to toggle pause
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                if (soundMenuUI.activeSelf)
                    SoundMenuBack();
                else
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

    public void SoundMenu()
    {
        // get the volumes from the components
        int backgroundVolume = Mathf.RoundToInt(GameObject.Find("BackgroundMusic").GetComponent<AudioSource>().volume * 100);
        int effectsVolume = Mathf.RoundToInt(GameObject.Find("SoundEffects").GetComponent<AudioSource>().volume * 100);

        // swap menus
        pauseMenuUI.SetActive(false);
        soundMenuUI.SetActive(true);

        // update the values in the sliders before showing the menus
        GameObject.Find("MusicVolumeSlider").gameObject.GetComponent<Slider>().value = backgroundVolume;
        GameObject.Find("EffectsVolumeSlider").gameObject.GetComponent<Slider>().value = effectsVolume;
        UpdateVolumes();

    }
    public void SoundMenuBack()
    {
        // swap menus
        soundMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }

    public void UpdateVolumes()
    {
        // save the values from the sliders
        string musicVolume = GameObject.Find("MusicVolumeSlider").gameObject.GetComponent<Slider>().value.ToString();
        string effectsVolume = GameObject.Find("EffectsVolumeSlider").gameObject.GetComponent<Slider>().value.ToString();

        // update text
        GameObject.Find("MusicVolumeValue").GetComponent<TextMeshProUGUI>().text = musicVolume;
        GameObject.Find("EffectsVolumeValue").GetComponent<TextMeshProUGUI>().text = effectsVolume;

        // update component values (volumes)
        GameObject.Find("BackgroundMusic").GetComponent<AudioSource>().volume = musicVolume.ToFloat() / 100;
        GameObject.Find("SoundEffects").GetComponent<AudioSource>().volume = effectsVolume.ToFloat() / 100;

        // save the changes to the config file using the singleton pattern
        ConfigManager.Instance.SetInt("musicVolume", musicVolume.ToInt());
        ConfigManager.Instance.SetInt("effectsVolume", effectsVolume.ToInt());
        ConfigManager.Instance.SaveConfigFile();
    }
}
