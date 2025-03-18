using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    // Reference to the enemy's Rigidbody2D component
    private Rigidbody2D rb;

    void Start()
    {
        // Get the Rigidbody2D component attached to the enemy
        rb = GetComponent<Rigidbody2D>();
    }

    // Detects when the enemy collides with another object
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collision is with the player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Stop enemy movement when colliding with the player
            rb.velocity = Vector2.zero;

            // Debug message to confirm collision detection
            Debug.Log("Player collided with the enemy!");
        }
    }
}
