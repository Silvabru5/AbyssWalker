using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    // Maximum health of the player
    public int maxHealth = 100;

    // Tracks the player's current health
    private int currentHealth;
    
    void Start()
    {
        // Set the player's health to max when the game starts
        currentHealth = maxHealth;
    }

    // Called when the player takes damage
    public void TakeDamage(int damage)
    {
        // Subtract the received damage from the current health
        currentHealth -= damage;

        // Display the updated health in the Unity Console (for debugging)
        Debug.Log("Player Health: " + currentHealth);

        // Check if the player's health reaches zero or below
        if (currentHealth <= 0)
        {
            Die(); // Trigger death behavior
        }
    }

    // Handles the player's death (will need to add more ofc once I find the time)
    void Die()
    {
        Debug.Log("Player Died!");

        
    }
}
