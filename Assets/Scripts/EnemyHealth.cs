using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 20; // how much health the enemy starts with
    private int currentHealth; // current health value
    private Animator animator; // reference to the animator
    [HideInInspector]
    private bool isDead = false; // tracks if enemy is already dead
    [HideInInspector]
    private bool isHurting = false; // tracks if enemy is currently playing hurt animation

    private string enemyType; 

    void Start()
    {
        currentHealth = maxHealth; // set health at start
        animator = GetComponent<Animator>(); // get animator attached to this object
        if (this.GetComponentInParent<isSpider>() != null) enemyType = "spider";
        else if (this.GetComponentInParent<isZombie>() != null) enemyType = "zombie";
        else if (this.GetComponentInParent<isSkeleton>() != null) enemyType = "skeleton";
    }

    // called whenever the enemy takes damage
    public void TakeDamage(int amount)
    {
        // if already dead or still flinching, ignore new damage
        if (isDead || isHurting) return;

        // subtract damage from health
        currentHealth -= amount;

        // if enemy is currently attacking, cancel the attack
        EnemyAI ai = GetComponent<EnemyAI>();
        if (ai != null && ai.isAttacking)
        {
            ai.InterruptAttack();
        }
        
        // if health hits zero or below, kill the enemy
        if (currentHealth <= 0)
        { /*
            if (enemyType == "spider") SoundManager.PlaySound(SoundTypeEffects.ENEMY_DEATH_SPIDER);
            else if (enemyType == "zombie") SoundManager.PlaySound(SoundTypeEffects.ENEMY_DEATH_ZOMBIE);
            else if (enemyType == "skeleton") SoundManager.PlaySound(SoundTypeEffects.ENEMY_DEATH_SKELETON); */
            if(enemyType == "spider") { ExperienceManager.instance.AddExperience(2); }
            else if(enemyType == "zombie") { ExperienceManager.instance.AddExperience(5); }
            else if(enemyType == "skeleton") { ExperienceManager.instance.AddExperience(7); }
            Die();
        }
        else
        {
            /*
            if (enemyType == "spider") SoundManager.PlaySound(SoundTypeEffects.ENEMY_TAKES_DAMAGE_SPIDER);
            else if (enemyType == "zombie") SoundManager.PlaySound(SoundTypeEffects.ENEMY_TAKES_DAMAGE_ZOMBIE);
            else if (enemyType == "skeleton") SoundManager.PlaySound(SoundTypeEffects.ENEMY_TAKES_DAMAGE_SKELETON); */
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
        EnemyAI ai = GetComponent<EnemyAI>();
        if (ai != null) ai.InterruptAttack();

        // wait a bit before resetting the hurt state
        Invoke(nameof(ResetHurtState), 0.3f); // change this delay to match your animation
    }

    // called after hurt delay to allow the enemy to move again
    void ResetHurtState()
    {
        isHurting = false;

        // allow enemy to attack again if still alive
        EnemyAI ai = GetComponent<EnemyAI>();
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
        EnemyAI ai = GetComponent<EnemyAI>();
        if (ai != null) ai.enabled = false;

        // stop all movement
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
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

}
