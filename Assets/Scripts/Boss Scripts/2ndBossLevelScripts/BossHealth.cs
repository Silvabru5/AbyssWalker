using UnityEngine;
using UnityEngine.UI;

/*
    Author(s): Bruno Silva
    Description: manages the boss’s health, damage intake, death state, and updates the on-screen
                 health bar. also plays hurt and death animations and disables boss behaviour once
                 defeated.
    Date (Last Modification): 11/22/2025
*/

public class BossHealth : MonoBehaviour
{
    [Header("health settings")]
    public int maxHealth = 100;              // total health the boss starts with
    public int currentHealth;                // runtime health stored here

    [Header("ui")]
    public Slider healthBar;                 // reference to the boss health slider
    public Animator animator;                // animator controlling boss animations

    private bool isDead = false;             // prevents taking damage or dying twice

    void Start()
    {
        // initialize health and update ui
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    // called when the player deals damage to the boss
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        // apply damage and clamp to valid range
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();

        // play hurt animation
        animator.SetTrigger("hurt");


        // check for death
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // updates the health bar value (0 to 1)
    private void UpdateHealthUI()
    {
        if (healthBar != null)
            healthBar.value = (float)currentHealth / maxHealth;
    }

    // triggers when the boss health reaches zero
    private void Die()
    {
        if (isDead) return;
        isDead = true;

        animator.SetTrigger("die");

        // stop boss movement and behaviour
        var controller = GetComponent<BossControllerHybrid>();
        if (controller != null)
        {
            controller.enabled = false;

            // stop any existing velocity
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }

        // disable collisions after death
        GetComponent<Collider2D>().enabled = false;
    }
}
