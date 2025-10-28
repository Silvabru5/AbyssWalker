using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class SceneLoader : MonoBehaviour
{
    [SerializeField] private GameObject crossfade;
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private float transitionTime;
    [SerializeField] private Canvas gameCanvas;
    [SerializeField] private GameObject UICanvas;
    public static SceneLoader instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(this.gameObject);
            DontDestroyOnLoad(gameCanvas);
            // Subscribe to sceneLoaded event
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject); // Prevent duplicate persistent objects
        }       
    }
    private void Start()
    {
       UICanvas.SetActive(true);
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.P))
        //    LoadNextLevel();
    }

    public void LoadNextLevel()
    {
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        LoadSpecificLevel(nextIndex);
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        if (crossfade == null)
            crossfade = GameObject.Find("Crossfade");

        if (crossfade != null)
            crossfade.GetComponent<Animator>().SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        // LoadSceneMode.Single ensures the old scene is removed
        SceneManager.LoadScene(levelIndex,LoadSceneMode.Single);
    }

    public void LoadSpecificLevel(int buildIndex)
    {
        StartCoroutine(LoadLevel(buildIndex));
        ChangeBackgroundMusic(buildIndex);
        PlaySceneEntrySound(buildIndex);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Enable in-game UI just for scenes 3+5 save game on scene 2
        
        switch (scene.buildIndex)
        {
            case 2: SaveAndLoadManager.instance.SaveGame(); break;
            case 3: inGameUI.SetActive(true); break;
            case 4: inGameUI.SetActive(false); break;
            case 5: inGameUI.SetActive(true); break;
            case 6: inGameUI.SetActive(false); break;
            default: break;
        }
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
