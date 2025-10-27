using System.Collections;
using TMPro;
using UnityEngine;

public class BossMonsterDracula : MonoBehaviour
{
    [Header("health settings")]
    [SerializeField] private float maxHealth = 100f;       // maximum health value for the boss
    [SerializeField] private float currentHealth;          // current health value that changes during the fight
    private bool isDead;                                   // flag to prevent multiple deaths

    [Header("references")]
    [SerializeField] private Animator animator;            // reference to the animator controlling animations
    [SerializeField] private Collider2D bossCollider;      // boss collider for hit detection
    [SerializeField] private HealthBar healthBar;          // ui health bar that displays boss health
    [SerializeField] private TextMeshProUGUI percentText;  // ui text showing health percentage
    [SerializeField] private GameObject returnPortal;      // portal object that activates after death

    public float getHealth()
    {
        return currentHealth;
    }
    private void Start()
    {
        // get component references if not already assigned
        if (animator == null) animator = GetComponent<Animator>();
        if (bossCollider == null) bossCollider = GetComponent<Collider2D>();

        // set health to full at start
        currentHealth = maxHealth;

        // set up the health bar and display initial values
        healthBar.SetMaxHealth(maxHealth);
        UpdateHealthUI();
    }

    // called whenever the boss takes damage from the player
    //public void TakeDamage(float damage)
    //{
    //    // ignore any hits if already dead
    //    if (isDead) return;

    //    // subtract damage and make sure it stays within valid range
    //    currentHealth -= damage;
    //    currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

    //    // update ui elements to reflect new health
    //    healthBar.SetHealth(currentHealth);
    //    UpdateHealthUI();

    //    // if boss still has health, play hit animation
    //    if (currentHealth > 0)
    //    {
    //        animator.SetTrigger("Hit");
    //    }
    //    else
    //    {
    //        // if health reaches zero or less, start death process
    //        Die();
    //    }
    //}
    private IEnumerator EnableControllerAfterHit()
    {
        yield return new WaitForSeconds(0.6f); // length of hit animation
        var controller = GetComponent<BossControllerHybrid>();
        if (controller != null)
            controller.enabled = true;
    }
    public void TakeDamage(float damage)
    {
        // ignore any hits if already dead
        if (isDead) return;

        // subtract damage and make sure it stays within valid range
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // update ui elements to reflect new health
        healthBar.SetHealth(currentHealth);
        UpdateHealthUI();

        // if boss still has health, play hit animation
        if (currentHealth > 0)
        {
            // temporarily disable controller to prevent run/attack cancelling the hit animation
            var controller = GetComponent<BossControllerHybrid>();
            if (controller != null)
                controller.enabled = false;

            // reset and trigger the hit animation
            animator.ResetTrigger("Hit");
            animator.SetTrigger("Hit");

            // re-enable controller after the hit animation finishes
            StartCoroutine(EnableControllerAfterHit());
        }
        else
        {
            // if health reaches zero or less, start death process
            Die();
        }
    }

    // updates the on-screen text to show current health percentage
    private void UpdateHealthUI()
    {
        float percent = (currentHealth / maxHealth) * 100f; // calculate percentage
        percent = Mathf.Max(0, percent);                    // ensure it doesn’t go below 0

        if (percentText != null)
            percentText.text = percent.ToString("F1") + "%"; // display value with one decimal place
    }

    // handles all logic when the boss dies
    private void Die()
    {
        // prevent multiple death triggers
        if (isDead) return;
        isDead = true;

        // disable boss ai movement logic
        var controller = GetComponent<BossControllerHybrid>();
        if (controller != null)
            controller.enabled = false;

        // stop all physics movement and freeze body in place
        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;              // stop moving
            rb.gravityScale = 0f;                          // prevent falling
            rb.constraints = RigidbodyConstraints2D.FreezeAll; // lock body
        }

        // play death animation
        if (animator != null)
        {
            animator.ResetTrigger("Hit");                  // clear any hit triggers
            animator.SetTrigger("die");                    // play death animation
        }

        // start coroutine to wait for animation and cleanup
        StartCoroutine(DeathSequence());
    }

    // waits for death animation to finish before activating portal and destroying boss
    private IEnumerator DeathSequence()
    {
        // wait for the length of the death animation
        yield return new WaitForSeconds(4.3f);

        // enable the portal so the player can interact with it
        if (returnPortal != null)
            returnPortal.SetActive(true);

        // wait a bit before removing the boss from the scene
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
