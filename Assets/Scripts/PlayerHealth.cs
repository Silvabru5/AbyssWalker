using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"Player HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Debug.Log("Player has died.");
        }
    }
}
