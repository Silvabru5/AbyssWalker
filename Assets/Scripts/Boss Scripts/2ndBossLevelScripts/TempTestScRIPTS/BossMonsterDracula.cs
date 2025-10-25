using System.Collections;
using TMPro;
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

    //private void Die()
    //{
    //    isDead = true;
    //    animator.SetBool("Dead", true);
    //    bossCollider.enabled = false;
    //    StartCoroutine(DeathSequence());
    //    Debug.Log("[Boss] Dracula defeated!");
    //}
    //private void Die()
    //{
    //    if (isDead) return;
    //    isDead = true;

    //    Debug.Log("[Boss] Dracula defeated!");

    //    // Freeze movement (stops gravity / falling)
    //    Rigidbody2D rb = GetComponent<Rigidbody2D>();
    //    if (rb != null)
    //    {
    //        rb.linearVelocity = Vector2.zero;
    //        rb.bodyType = RigidbodyType2D.Kinematic; // freezes in place
    //        rb.simulated = false; // disables physics while animation plays
    //    }

    //    // Trigger death animation
    //    animator.SetTrigger("Die");

    //    // Lock collider so boss stays visible on platform
    //    if (bossCollider != null)
    //        bossCollider.enabled = true;

    //    // Set health bar to 0
    //    healthBar.SetHealth(0);
    //    UpdateHealthUI();

    //    // Begin fade / despawn
    //    StartCoroutine(DeathSequence());
    //}

    //private IEnumerator DeathSequence()
    //{

    //    yield return new WaitForSeconds(8f);

    //    // Disable collision after animation finishes
    //    if (bossCollider != null)
    //        bossCollider.enabled = false;

    //    // Finally, remove boss
    //    gameObject.SetActive(false);
    //    SceneManager.LoadScene("HomeBase");
    //    Debug.Log("[Boss] Death sequence complete.");
    //}


    //private IEnumerator DeathSequence()
    //{
    //    // Wait for the death animation to finish
    //    yield return new WaitForSeconds(3f);

    //    Debug.Log("[Boss] Dracula defeated — activating portal!");

    //    if (returnPortal != null)
    //        returnPortal.SetActive(true); // enable portal for player interaction

    //    // Optionally disable boss object
    //    GetComponent<Collider2D>().enabled = false;
    //    GetComponent<Rigidbody2D>().simulated = false;
    //}

    // BossMonsterDracula.cs (only replace these two methods)

    //private void Die()
    //{
    //    if (isDead) return; // Prevent multiple triggers
    //    isDead = true;

    //    Debug.Log("[Boss] Dracula defeated!");
    //    animator.SetBool("Dead", true); // Trigger death animation once

    //    // Stop movement completely
    //    if (TryGetComponent<Rigidbody2D>(out var rb))
    //    {
    //        rb.linearVelocity = Vector2.zero;
    //        rb.gravityScale = 0f; // Prevent falling through floor
    //    }

    //    // Optionally disable attack logic or AI scripts here
    //    bossCollider.enabled = false;

    //    StartCoroutine(DeathSequence());
    //}

    //private IEnumerator DeathSequence()
    //{
    //    // Wait for death animation duration
    //    yield return new WaitForSeconds(5.5f);

    //    // Reactivate the return portal
    //    GameObject portal = GameObject.Find("HomeBasePortal");
    //    if (portal != null)
    //    {
    //        portal.SetActive(true);
    //        Debug.Log("[Boss] Activating return portal...");
    //    }
    //    else
    //    {
    //        Debug.LogWarning("[Boss] No portal found named 'HomeBasePortal'!");
    //    }

    //    // Wait one more second for dramatic pause
    //    yield return new WaitForSeconds(1f);

    //    Debug.Log("[Boss] Destroying Dracula object after animation.");
    //    Destroy(gameObject);
    //}


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

        // Play the Death animation once
        if (animator != null)
        {
            animator.ResetTrigger("Hit");
            animator.SetTrigger("Die");  // this matches the trigger parameter name
            Debug.Log("[Boss] Animator Trigger 'Die' set!");
        }

        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        Debug.Log("[Boss] Waiting for death animation...");
        yield return new WaitForSeconds(4.3f); // your Death animation length

        // Activate portal after animation finishes
        GameObject portal = GameObject.Find("HomeBasePortal");
        if (portal != null)
        {
            portal.SetActive(true);
            Debug.Log("[Boss] Portal activated!");
        }
        else
        {
            Debug.LogWarning("[Boss] Portal not found!");
        }

        // Wait a short moment before removing the boss
        yield return new WaitForSeconds(1f);
        Debug.Log("[Boss] Destroying Dracula after death animation.");
        Destroy(gameObject);
    }

}
