using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    [SerializeField] int currentLevel;
    [SerializeField] int skillPoints;
    // Player movement speed (set in Unity Inspector)
    public float movSpeed;

    // Stores input movement values
    float speedX, speedY;

    // Rigidbody2D component for physics-based movement
    private Rigidbody2D rb;

    // track if player is alive/dead
    private PlayerHealth playerHealth;

    // Prevents movement when colliding with enemies
    private bool canMove = true;

    public Vector2 LastMoveDirection { get; private set; } = Vector2.right;
    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();

        // Get the Rigidbody2D component attached to the player
        rb = GetComponent<Rigidbody2D>();

        // Prevents the Rigidbody from rotating when colliding with objects
        rb.freezeRotation = true;

        
    }

    void Update()
    {
        if (canMove && playerHealth.currentHealth > 0)
        {
            // Get player input (WASD / Arrow keys / Controller)
            speedX = Input.GetAxisRaw("Horizontal"); // Left (-1) / Right (1)
            speedY = Input.GetAxisRaw("Vertical");   // Down (-1) / Up (1)

            Vector2 inputDir = new Vector2(speedX, speedY);
            if (inputDir != Vector2.zero)
            {
                LastMoveDirection = inputDir.normalized;
            }
        }

        UpdateLevel();
    }

    void FixedUpdate()
    {
        // Only allow movement if the player is not colliding with an enemy
        if (canMove && playerHealth.currentHealth > 0)
        {
            // Normalize input to ensure diagonal movement isn't faster
            rb.linearVelocity = new Vector2(speedX, speedY).normalized * movSpeed;
        }
        else
        {
            // Stop player movement when colliding with an enemy
            rb.linearVelocity = Vector2.zero;
        }
    }

    // **Detect when the player collides with an enemy**
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            canMove = false; // Stop player movement while touching the enemy
        }
    }

    // **Detect when the player stops colliding with an enemy**
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            canMove = true; // Allow movement again when no longer touching the enemy
        }
    }

    void UpdateLevel()
    {
        currentLevel = ExperienceManager.instance.GetCurrentLevel();
    }
}
