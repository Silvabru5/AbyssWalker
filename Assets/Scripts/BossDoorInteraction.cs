using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class BossDoorInteraction : MonoBehaviour
{
    private bool bossDoorOpen = false;
    private int killCount; // = enemySpawner.numOfEnemiesDead; // temporary variable - replace with reference from the game
    private int threshold; //= GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>().maxEnemies; // temporary variable - replace with reference from the game
    private const float messageDuration = 3.0f;
    private float messageShowingTimer = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    //    enemySpawner = GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>();
        threshold = GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>().maxEnemies;
        killCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        killCount = GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>().numOfEnemiesDead;
        if (!bossDoorOpen && killCount >= threshold)
        {
            // SoundManager.PlaySound(SoundTypeEffects.BOSS_DOOR_OPENS);
            bossDoorOpen = true;
            GameObject.Find("MessageTextBox").GetComponent<TextMeshProUGUI>().text = "You have vanquished enough enemies to face the boss. A mysterious portal has now opened.";// display a message
        }

        if (messageShowingTimer > 0)
        {
            messageShowingTimer -= Time.deltaTime;
            if (messageShowingTimer <= 0)
            {
                messageShowingTimer = 0;
                GameObject.Find("MessageTextBox").GetComponent<TextMeshProUGUI>().text = string.Empty;
            }
        }
    }

// if the player interacts with the door to the boss
//   if kill count is high enough, allow new scene to load
//   otherwise make a sound and display a message
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<isHero>())
        {
            Debug.Log("Hero interacted with boss door");
            if (killCount >= threshold)
            {
                //SoundManager.PlaySound(SoundTypeEffects.BOSS_DOOR_ENTER);
                //SoundManager.PlayBackgroundMusic(SoundTypeBackground.BACKGROUND_BOSS);
                //SoundManager.PlaySoundWaitForCompletion(SoundTypeEffects.BOSS_DOOR_ENTER);
                SceneManager.LoadScene(2); // load scene with build index 2 (boss scene) - File->Build Profiles->Scene List 
            }
            else if (messageShowingTimer <= 0 && killCount < threshold) // only interact here if the message timer is not depleted
            {
                //SoundManager.PlaySound(SoundTypeEffects.BOSS_DOOR_BLOCKED); // play a sound
                GameObject.Find("MessageTextBox").GetComponent<TextMeshProUGUI>().text = "You have not vanquished enough enemies to proceed to the boss level";// display a message
                messageShowingTimer = messageDuration;
            }
        }
    }
}
