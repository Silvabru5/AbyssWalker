using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI")]
    public Slider healthBar;          
    public Animator animator;         

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    // Called when the player hits the boss
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();

        // Optional: play a hit animation or flash
        animator.SetTrigger("hurt");

        Debug.Log("[Boss] Took " + damage + " damage. Current HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthUI()
    {
        if (healthBar != null)
            healthBar.value = (float)currentHealth / maxHealth;
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("[Boss] Dracula defeated!");
        animator.SetTrigger("die");

        // Stop all movement and attacks
        var controller = GetComponent<BossControllerHybrid>();
        if (controller != null)
        {
            controller.enabled = false;
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }

        // Optionally disable collider after death animation ends
        GetComponent<Collider2D>().enabled = false;
    }
}
