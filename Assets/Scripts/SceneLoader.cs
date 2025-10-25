using System.Collections;
using UnityEngine;
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
        {
            instance = this;
        }
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
        {
            LoadNextLevel();
        }
    }

    public void LoadNextLevel()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void LoadSpecificLevel(int buildIndex)
    {
        StartCoroutine(LoadLevel(buildIndex));
        
    }
    
    IEnumerator LoadLevel(int levelIndex)
    {
        crossfade = GameObject.Find("Crossfade");
        crossfade.GetComponent<Animator>().SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelIndex);
    }

 

}
