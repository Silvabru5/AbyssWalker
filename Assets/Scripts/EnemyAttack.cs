using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    // Amount of damage the enemy deals to the player per attack
    public int damageAmount = 10;

    // Controls whether the enemy can currently attack
    private bool canAttack = true;

    // Time (in seconds) before the enemy can attack again after landing a hit
    public float attackCooldown = 1.5f;

    // Detects when the enemy enters the player's hitbox
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object collided with has the "Player" tag and the enemy can attack
        if (other.CompareTag("Player") && canAttack)
        {
            // Try to access the PlayerHealth script attached to the player
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                // Apply damage to the player
                playerHealth.TakeDamage(damageAmount);

                // Prevent immediate re-attacks by setting canAttack to false
                canAttack = false;

                // Start cooldown before the enemy can attack again
                Invoke("ResetAttack", attackCooldown);
            }
        }
    }

    // Resets the attack cooldown, allowing the enemy to attack again
    private void ResetAttack()
    {
        canAttack = true;
    }
}
