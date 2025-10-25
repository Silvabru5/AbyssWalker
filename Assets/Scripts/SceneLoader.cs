using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class SceneLoader : MonoBehaviour
{
    [SerializeField] private GameObject crossfade;
    [SerializeField] private float transitionTime;
    [SerializeField] private Canvas gameCanvas;
    public static SceneLoader instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        DontDestroyOnLoad(gameCanvas);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            LoadNextLevel();
    }

    public void LoadNextLevel()
    {
        LoadSpecificLevel(SceneManager.GetActiveScene().buildIndex + 1);
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        crossfade = GameObject.Find("Crossfade");
        crossfade.GetComponent<Animator>().SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelIndex);
    }

    public void LoadSpecificLevel(int buildIndex)
    {
        StartCoroutine(LoadLevel(buildIndex));
        ChangeBackgroundMusic(buildIndex);
        PlaySceneEntrySound(buildIndex);
    }

    // change the background sound on scene change
    public void ChangeBackgroundMusic(int sceneIndex)
    {
        switch (sceneIndex)
        {
            case 0: SoundManager.PlayBackgroundMusic(SoundTypeBackground.MAIN_MENU); break;
            case 1: SoundManager.PlayBackgroundMusic(SoundTypeBackground.CHARACTER_SELECT); break;
            case 2: SoundManager.PlayBackgroundMusic(SoundTypeBackground.HOME_BASE); break;
            case 3: SoundManager.PlayBackgroundMusic(SoundTypeBackground.CEMETERY); break;
            case 4: SoundManager.PlayBackgroundMusic(SoundTypeBackground.CEMETERY_BOSS); break;
            case 5: SoundManager.PlayBackgroundMusic(SoundTypeBackground.CAVE); break;
            case 6: SoundManager.PlayBackgroundMusic(SoundTypeBackground.CAVE_BOSS); break;
            default: Debug.Log("No background sound associated with scene index " + sceneIndex); break;
        }
    }

    // play a zone entry sound each time a level is loaded
    public void PlaySceneEntrySound(int sceneIndex)
    {
        switch (sceneIndex)
        {
            case 0: SoundManager.PlaySound(SoundTypeEffects.TITLE_SCREEN_LOAD); break;
            case 1: break;
            case 2: break;
            case 3: SoundManager.PlaySound(SoundTypeEffects.CEMETERY_LOAD); break;
            case 4: SoundManager.PlaySound(SoundTypeEffects.CEMETERY_BOSS_LOAD); break;
            case 5: SoundManager.PlaySound(SoundTypeEffects.CAVE_LOAD); break;
            case 6: SoundManager.PlaySound(SoundTypeEffects.CAVE_BOSS_LOAD); break;
            default: Debug.Log("No zone entry sound associated with scene index " + sceneIndex); break;
        }
    }
}
