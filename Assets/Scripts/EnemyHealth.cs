using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 20;
    private int currentHealth;
    private Animator animator;
    private bool isHurting = false;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (animator != null)
        {
            var currentState = animator.GetCurrentAnimatorStateInfo(0);
            string stateName = GetStateName(currentState.fullPathHash);
        }
    }

    public void TakeDamage(int amount)
    {

        currentHealth -= amount;

        if (!isHurting && animator != null)
        {
            animator.SetTrigger("isHit");
            isHurting = true;

            // Use a fallback delay to reset isHurting if the animation can't be measured
            float fallbackResetTime = 0.5f;

            float animLength = animator.GetCurrentAnimatorStateInfo(0).length;
            float delay = animLength > 0.01f ? animLength : fallbackResetTime;

            Invoke("ResetHurt", delay);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void ResetHurt()
    {
        isHurting = false;

    }

    void Die()
    {
        Debug.Log($"{gameObject.name} died.");

        // Force play death animation immediately
        animator.Play("skeleton_death", 0, 0);

        // Optional: also disable further animation changes
        animator.SetBool("isWalking", false);
        animator.SetBool("isAttacking", false);

        // Disable AI
        EnemyAI ai = GetComponent<EnemyAI>();
        if (ai != null) ai.enabled = false;

        // Stop movement
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        // Disable collision
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // Destroy after delay
        Destroy(gameObject, 5f);
    }



    // Optional: Converts hash to string for readability
    private string GetStateName(int fullPathHash)
    {
        AnimatorClipInfo[] clips = animator.GetCurrentAnimatorClipInfo(0);
        if (clips.Length > 0)
        {
            return clips[0].clip.name;
        }
        return $"HASH: {fullPathHash}";
    }
}
