using UnityEngine;
using UnityEngine.SceneManagement;

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
