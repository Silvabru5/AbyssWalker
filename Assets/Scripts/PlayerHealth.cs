using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    private Animator animator;
    public  float deathAnimationTime = 0.1f;

    public TextMeshProUGUI healthText; // drag the health ui text here in the inspector

    // this runs when the game starts and sets up the player's health and health regen
    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthText(); // update the UI with initial health
        InvokeRepeating(nameof(RegenerateHealth), 3f, 3f); // heal 2 hp every 3 seconds
        animator = GetComponent<Animator>(); //get death anim if health depletes
    }

    // this updates the health text every frame to match the current health
    void Update()
    {
        UpdateHealthText();
    }

    // this handles the player taking damage and checks if they die
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth); // prevent negative health
        Debug.Log($"Player HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            animator.SetTrigger("DeathTrigger");

            // SoundManager.PlaySound(SoundTypeEffects.PLAYER_BARBARIAN_DEATH, 1);
            Debug.Log("Player has died.");
            StartCoroutine(DeathSequence());

            // find the game over manager in the scene and show the game over screen
            // GameOverManager gameOver = FindAnyObjectByType<GameOverManager>();
            // if (gameOver != null)
            // {
                // gameOver.ShowGameOver();
            // }
            // else
            // {
                // Debug.LogWarning("GameOverManager not found in scene!");
            // }
        }
        else
        {
            SoundManager.PlaySound(SoundTypeEffects.PLAYER_BARBARIAN_TAKES_DAMAGE, 1);
        }
    }


    private IEnumerator DeathSequence()
    {
        
        SoundManager.PlaySound(SoundTypeEffects.PLAYER_BARBARIAN_DEATH, 1);

        yield return new WaitForSeconds(deathAnimationTime);

        GameOverManager gameOver = FindAnyObjectByType<GameOverManager>();
         if (gameOver != null)
        {
            gameOver.ShowGameOver();
        }
        else
        {
            Debug.LogWarning("GameOverManager not found in scene!");
        }
    }

    // this returns the player's current health as a percentage (0.0 to 1.0)
    public float GetHealthPercent()
    {
        return Mathf.Clamp01((float)currentHealth / maxHealth);
    }

    // this slowly heals the player over time, up to their max health
    void RegenerateHealth()
    {
        if (currentHealth < maxHealth)
        {
            int healAmount = Mathf.Min(2, maxHealth - currentHealth);
            currentHealth += healAmount;
            Debug.Log($"Regenerated {healAmount} HP. Current HP: {currentHealth}");
        }
    }

    // this updates the health text UI to show the current health
    private void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = $"{currentHealth} / {maxHealth}";
        }
    }
}
