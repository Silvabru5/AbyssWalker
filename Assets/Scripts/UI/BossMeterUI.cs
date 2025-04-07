using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossMeterUI : MonoBehaviour
{
    public Image bossMeterFill;         // drag the boss meter fill image here
    public int killsNeeded = 10;        // number of kills required to fill the bar
    private int currentKills = 0;       // tracks how many enemies have been killed

    public TextMeshProUGUI killCountText; // drag the boss meter text UI here

    private bool bossTriggered = false; // tracks whether the boss meter is full
    private bool isFlashing = false;    // controls if flashing is active
    private float flashTimer = 0f;      // timer for flashing interval
    private float flashInterval = 0.2f; // how fast the text flashes
    private int flashCount = 0;         // number of times flashed so far
    private int maxFlashes = 30;        // total number of flashes

    private Color originalColor;        // stores the original text color
    public Color flashColor = Color.red; // color to flash when boss is ready

    private GameObject enemySpawner;

    // this runs when the game starts and sets the initial UI state
    void Start()
    {
        bossMeterFill.fillAmount = 0f;
        enemySpawner = GameObject.Find("EnemySpawner");
        //killCountText.text = enemySpawner.GetComponent<EnemySpawner>().maxEnemies.ToString();
        if (enemySpawner != null)
        {
            killsNeeded = enemySpawner.GetComponent<EnemySpawner>().maxEnemies;

            if (killCountText != null)
            {
                killCountText.text = $"0 / {killsNeeded} enemies killed";
            }
        }
        else
        {
            Debug.LogWarning("EnemySpawner not assigned to BossMeterUI.");
        }
        //if (killCountText != null)
        //{
        //    killCountText.text = $"0 / {killsNeeded} enemies killed";
        //}
    }

    // this handles flashing the boss meter text when the meter is full
    void Update()
    {
        if (isFlashing)
        {
            flashTimer += Time.deltaTime;

            if (flashTimer >= flashInterval)
            {
                flashTimer = 0f;
                flashCount++;

                // switch between flash color and original color
                killCountText.color = (flashCount % 2 == 0) ? flashColor : originalColor;

                // stop flashing after reaching the max number of flashes
                if (flashCount >= maxFlashes)
                {
                    isFlashing = false;
                    killCountText.color = originalColor;
                }
            }
        }
    }

    // this is called when an enemy is killed and updates the bar and text
    public void RegisterKill()
    {
        if (bossTriggered) return;

        currentKills++;

        float percent = Mathf.Clamp01((float)currentKills / killsNeeded);
        bossMeterFill.fillAmount = percent;

        if (currentKills >= killsNeeded)
        {
            bossTriggered = true;
            killCountText.text = "BOSS READY";

            // begin flashing effect
            isFlashing = true;
            flashTimer = 0f;
            flashCount = 0;
            originalColor = killCountText.color;
        }
        else
        {
            killCountText.text = $"{currentKills} / {killsNeeded} enemies killed";
        }
    }
}
