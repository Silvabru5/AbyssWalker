using UnityEngine;
using UnityEngine.SceneManagement;


//checks to see if the player has entered the boss portal and loads the boss scene
public class BossPortal : MonoBehaviour
{
    public string bossSceneName = "HomeBase";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(bossSceneName);
        }
    }
}
