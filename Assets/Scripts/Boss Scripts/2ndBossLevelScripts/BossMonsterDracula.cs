using System.Collections;
using TMPro;
using UnityEngine;

/*
    Author(s): Bruno Silva
    Description: manages dracula's health, damage intake, hit reactions,
                 death logic, UI updates, and the activation of the exit portal
                 once the boss is defeated. works alongside BossControllerHybrid 
                 to coordinate movement and animation flow.
    Date (last modification): 11/22/2025
*/

public class BossMonsterDracula : MonoBehaviour
{
    [Header("health settings")]
    [SerializeField] private float maxHealth = 100f;        // maximum possible health
    [SerializeField] private float currentHealth;           // current health value
    private bool isDead;                                    // used to prevent duplicate death handling

    [Header("references")]
    [SerializeField] private Animator animator;             // boss animator for hit / death animations
    [SerializeField] private Collider2D bossCollider;       // main collider for hit detection
    [SerializeField] private HealthBar healthBar;           // UI health bar showing boss health
    [SerializeField] private TextMeshProUGUI percentText;   // UI text that displays health percentage
    [SerializeField] private GameObject returnPortal;       // portal enabled after death
    [SerializeField] private GameObject attackPoint;        // reference to attack hitbox (disabled on death)

    // public getter for other scripts
    public float getHealth()
    {
        return currentHealth;
    }

    private void Start()
    {
        // auto-assign components if not set in inspector
        if (animator == null) animator = GetComponent<Animator>();
        if (bossCollider == null) bossCollider = GetComponent<Collider2D>();

        // initialize health
        currentHealth = maxHealth;

        // initialize UI
        healthBar.SetMaxHealth(maxHealth);
        UpdateHealthUI();
    }

    // waits for hit animation to finish before re-enabling movement ai
    private IEnumerator EnableControllerAfterHit()
    {
        yield return new WaitForSeconds(0.6f);  // match hit animation timing

        var controller = GetComponent<BossControllerHybrid>();
        if (controller != null)
            controller.enabled = true;
    }

    // main method called when the boss takes damage
    public void TakeDamage(float damage)
    {
        if (isDead) return;   // ignore if already dead

        // subtract damage and clamp value
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // update UI values
        healthBar.SetHealth(currentHealth);
        UpdateHealthUI();

        // still alive → play hit reaction
        if (currentHealth > 0)
        {
            // disable controller temporarily so hit animation is not interrupted
            var controller = GetComponent<BossControllerHybrid>();
            if (controller != null)
                controller.enabled = false;

            // trigger hit animation
            animator.ResetTrigger("Hit");
            animator.SetTrigger("Hit");

            // re-enable ai movement after hit animation delay
            StartCoroutine(EnableControllerAfterHit());
        }
        else
        {
            // health hit zero → die
            Die();
        }
    }

    // updates the percentage text next to the health bar
    private void UpdateHealthUI()
    {
        float percent = (currentHealth / maxHealth) * 100f;
        percent = Mathf.Max(0, percent);  // ensure no negative

        if (percentText != null)
            percentText.text = percent.ToString("F1") + "%";  // one decimal place
    }

    // handles the entire death logic flow
    private void Die()
    {
        if (isDead) return;
        isDead = true;

        // disable movement/attack ai
        var controller = GetComponent<BossControllerHybrid>();
        if (controller != null)
            controller.enabled = false;

        // freeze physics and movement
        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        // play death animation
        if (animator != null)
        {
            animator.SetTrigger("die");
        }

        // disable attack hitbox
        attackPoint.SetActive(false);

        // begin final sequence
        StartCoroutine(DeathSequence());
    }

    // waits for animation and enables portal before removing boss
    private IEnumerator DeathSequence()
    {
        animator.SetBool("isDead", true);

        // wait for full death animation window
        yield return new WaitForSeconds(3.5f);

        // enable exit portal so player can leave arena
        if (returnPortal != null)
            returnPortal.SetActive(true);

        // remove boss from scene
        Destroy(gameObject);
    }
}
