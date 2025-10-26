using System.Collections;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossMonsterDracula : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    private bool isDead;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D bossCollider;
    [SerializeField] private HealthBar healthBar; // Custom HealthBar script
    [SerializeField] private TextMeshProUGUI percentText; // MainBoss Text (TMP)
    [SerializeField] private GameObject returnPortal;
    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (bossCollider == null) bossCollider = GetComponent<Collider2D>();

        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        UpdateHealthUI();
    }

    // Called when the player deals damage
    public void TakeDamage(float damage)
    {
        Debug.Log("[Boss] TakeDamage called! Damage: " + damage);
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log("[Boss] New HP: " + currentHealth);
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Update visuals
        healthBar.SetHealth(currentHealth);
        UpdateHealthUI();

        // Play hit animation
        animator.SetTrigger("Hit");

        // Check for death
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthUI()
    {
        float percent = (currentHealth / maxHealth) * 100f;
        percent = Mathf.Max(0, percent);
        if (percentText != null)
            percentText.text = percent.ToString("F1") + "%";
    }


    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("[Boss] Dracula Die() called!");

        // Disable movement and physics
        if (TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        if (bossCollider != null)
            bossCollider.enabled = false;

        // Tell controller to stop chasing and attacking
        var controller = GetComponent<BossControllerHybrid>();
        if (controller != null)
            controller.OnDeath();

        // Play the Death animation once
        if (animator != null)
        {
            animator.ResetTrigger("Hit");
            animator.SetTrigger("Die");
            Debug.Log("[Boss] Animator Trigger 'Die' set!");
        }

        StartCoroutine(DeathSequence());
    }


    private IEnumerator DeathSequence()
    {
        Debug.Log("[Boss] Waiting for death animation...");
        yield return new WaitForSeconds(4.3f); // your Death animation length

        // Activate portal after animation finishes
        if (returnPortal != null)
        {
            returnPortal.SetActive(true);
            Debug.Log("[Boss] Portal activated!");
        }
        else
        {
            Debug.LogWarning("[Boss] Portal reference missing in Inspector!");
        }

        // Wait a short moment before removing the boss
        yield return new WaitForSeconds(1f);
        Debug.Log("[Boss] Destroying Dracula after death animation.");
        Destroy(gameObject);
    }

}
