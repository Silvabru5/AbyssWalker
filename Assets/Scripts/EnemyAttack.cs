using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public int damageAmount = 10;  // Amount of damage dealt to the player
    private bool canAttack = true;
    public float attackCooldown = 1.5f; // Delay between attacks

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && canAttack)
        {
            // Try to get the PlayerHealth script and deal damage
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
                canAttack = false;
                Invoke("ResetAttack", attackCooldown); // Delay before next attack
            }
        }
    }

    private void ResetAttack()
    {
        canAttack = true;
    }
}
