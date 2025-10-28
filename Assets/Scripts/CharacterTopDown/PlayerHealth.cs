using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem.Processors;
using System;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth;
    public float baseHealth = 100;
    [HideInInspector] public float currentHealth;
    [HideInInspector] public Image healthBarFill;

    private Animator animator;
    public float deathAnimationTime = 0.1f;
    [HideInInspector]
    public TextMeshProUGUI healthText; // drag the health ui text here in the inspector

    // this runs when the game starts and sets up the player's health and health regen
    void Start()
    {
        UpdateHealthFromStats();
        GameObject textObj = GameObject.FindWithTag("HealthText");
        GameObject fillObj = GameObject.FindWithTag("HealthFill");
        if (textObj != null && textObj.activeInHierarchy)
        {
            healthText = textObj.GetComponent<TextMeshProUGUI>();
        }
        if(fillObj != null && fillObj.activeInHierarchy)
        {
            Debug.LogWarning("Fill found");
            healthBarFill = fillObj.GetComponent<Image>();
        }
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
    public void TakeDamage(float amount)
    {
        currentHealth -= CalculateDamage(amount);
        
        //        currentHealth = Mathf.Max(0, currentHealth); // prevent negative health
        Debug.Log($"Player HP: {currentHealth}");

        if (currentHealth > -99999 && currentHealth <= 0)
        {
            SoundManager.PlaySound(SoundTypeEffects.WARRIOR_DEATH);
            currentHealth = -99999; // flag to say this has already triggered - no more death sounds
            animator.SetTrigger("DeathTrigger");

            // SoundManager.PlaySound(SoundTypeEffects.PLAYER_BARBARIAN_DEATH, 1);
            Debug.Log("Player has died.");
            StartCoroutine(DeathSequence());
        }
        else
            SoundManager.PlaySound(SoundTypeEffects.WARRIOR_TAKES_DAMAGE);
    }


    private IEnumerator DeathSequence()
    {

        // SoundManager.PlaySound(SoundTypeEffects.PLAYER_BARBARIAN_DEATH, 1);

        yield return new WaitForSeconds(deathAnimationTime);

        GameOverManager gameOver = FindAnyObjectByType<GameOverManager>();
        if (gameOver != null)
            gameOver.ShowGameOver();
        else
            Debug.LogWarning("GameOverManager not found in scene!");
    }

    // this returns the player's current health as a percentage (0.0 to 1.0)
    public float GetHealthPercent()
    {
        return Mathf.Clamp01(currentHealth / maxHealth);
    }

    // this slowly heals the player over time, up to their max health
    void RegenerateHealth()
    {
        if (currentHealth < maxHealth && currentHealth > 0)
        {
            float healAmount = Mathf.Min(2, maxHealth - currentHealth);
            currentHealth += healAmount;
            Debug.Log($"Regenerated {healAmount} HP. Current HP: {currentHealth}");
        }
    }

    // this updates the health text UI to show the current health
    private void UpdateHealthText()
    {
        if (healthText != null)
            healthText.text = $"{currentHealth} / {maxHealth}";
        if(healthBarFill!=null)
            healthBarFill.fillAmount = currentHealth/maxHealth;
    }
    public void UpdateHealthFromStats()
    {
        // use base health so it doesn’t shrink if player is damaged
        maxHealth = baseHealth * StatManager.instance.GetHealthAmount();

        // optionally refill current health
        currentHealth = maxHealth;

        UpdateHealthText(); // update the UI
    }
    public float GetHealth()
    {
        return currentHealth;
    }
    float CalculateDamage(float amount)
    {
        float calculatedDamage = amount * StatManager.instance.GetDefenseAmount();
        Debug.Log(calculatedDamage);
        return calculatedDamage;
    }
}
