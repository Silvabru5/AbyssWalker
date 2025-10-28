using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int baseHealth; // how much health the enemy starts with
    public float healthGrowth = 0.15f; // how much health the enemy starts with
    [SerializeField]private float currentHealth; // current health value
    private float maxHealth; // current health value
    private Animator animator; // reference to the animator
    [HideInInspector]
    private bool isDead = false; // tracks if enemy is already dead
    [HideInInspector]
    private bool isHurting = false; // tracks if enemy is currently playing hurt animation
    private string enemyType;
    EnemyAI ai;
    EnemyLevel level;
    Rigidbody2D rb;
    void Start()
    {
        level = GetComponent<EnemyLevel>();
        UpdateHealth();
        animator = GetComponent<Animator>(); // get animator attached to this object
        enemyType = SoundManager.GetPlayerOrEnemyType(this.gameObject);
        ai = GetComponent<EnemyAI>();
        rb = GetComponent<Rigidbody2D>();
    }



    // called whenever the enemy takes damage
    public void TakeDamage(float amount)
    {
        // if already dead or still flinching, ignore new damage
        if (isDead || isHurting) return;

        // subtract damage from health
        currentHealth -= amount;

        // if enemy is currently attacking, cancel the attack
        if (ai != null && ai.isAttacking)
        {
            ai.InterruptAttack();
        }
        
        // if health hits zero or below, kill the enemy
        if (currentHealth <= 0)
        {
            // if a death sound is defined in the enum, play it
            if (Enum.TryParse(enemyType + "_DEATH", out SoundTypeEffects key))
                SoundManager.PlaySound(key);

            ExperienceManager.instance.AddExperience(level.CalculateExp());
            Die();
        }
        else
        {
            // if a takes damage sound is defined in the enum, play it
            if (Enum.TryParse(enemyType + "_TAKES_DAMAGE", out SoundTypeEffects key))
                SoundManager.PlaySound(key);
            // play the hurt animation
            PlayHurtAnimation();
        }
    }

    // handles hurt animation and brief flinch state
    void PlayHurtAnimation()
    {
        isHurting = true; // prevents more damage or re-triggering

        if (animator != null)
        {
            animator.SetTrigger("isHit"); // play hurt animation
        }

        // cancel current attack (if attacking)
        
        if (ai != null) ai.InterruptAttack();

        // wait a bit before resetting the hurt state
        Invoke(nameof(ResetHurtState), 0.3f); // change this delay to match your animation
    }

    // called after hurt delay to allow the enemy to move again
    void ResetHurtState()
    {
        isHurting = false;

        // allow enemy to attack again if still alive
        if (ai != null)
        {
            ai.canAttack = true;
        }

        // don’t reset animation if already dead
        if (isDead) return;

        // reset any lingering animation flags
        animator.SetBool("isAttacking", false);
        animator.SetBool("isWalking", false);
    }

    // handles death logic and animation
    void Die()
    {
        if (isDead) return;
        isDead = true;

        // increment through the spawner when a death happens
        EnemySpawner spawner = FindAnyObjectByType<EnemySpawner>();
        if (spawner != null)
        {
            spawner.EnemyDied();  // shared counter increases
            // handles the UI for the boss meter
            BossMeterUI bossMeter = FindAnyObjectByType<BossMeterUI>();
            if (bossMeter != null)
            {
                bossMeter.RegisterKill();
            }
        }

        // play the death animation
        if (animator != null)
        {
            animator.SetTrigger("isDead");
        }

        // stop any enemy behavior
        if (ai != null) ai.enabled = false;

        // stop all movement
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        // turn off all colliders to avoid future interactions
        foreach (Collider2D col in GetComponents<Collider2D>())
        {
            col.enabled = false;
        }
        
        // object will get destroyed in DestroySelf when an event is triggered in the end of the death animation
    }

    // this function is called from the end of the death animation (set in animation event)
    public void DestroySelf()
    { 
        Destroy(gameObject); // remove the enemy from the scene
    }
    public float GetHealthPercent()
    {
        return (float)currentHealth / maxHealth;
    }
    private void UpdateHealth()
    {
        int enemyLevel = 1;
        if (level != null)
        enemyLevel = level.GetEnemyLevel();
        maxHealth = Mathf.RoundToInt(baseHealth * Mathf.Pow(1 + healthGrowth, enemyLevel - 1));
        currentHealth = maxHealth;
        Debug.Log($"[UpdateHealth] Level: {enemyLevel}, Base: {baseHealth}, Growth: {healthGrowth}, Max: {maxHealth}");

    }
}
