using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 2f;
    public int damage = 10;
    public float attackCooldown = 1.5f;
    public float separationDistance = 0.7f; // Minimum distance between skeletons

    private Animator animator;
    private bool isAttacking = false;
    private bool canAttack = true;
    private bool attackRegistered = false;
    private bool playerInRange = false;
    private bool lockFlip = false; // Prevents flipping direction mid-attack

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        if (!lockFlip) // Prevent flipping during attack animations
        {
            FlipTowardsPlayer();
        }

        if (isAttacking || !canAttack) return; // Prevent movement if attacking

        if (playerInRange)
        {
            StartAttack();
        }
        else
        {
            MoveTowardsPlayer();
        }
    }

    void MoveTowardsPlayer()
    {
        if (!playerInRange) // Only move if not in range
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isAttacking", false);

            Vector2 direction = (player.position - transform.position).normalized;
            Vector2 avoidance = AvoidStacking(); // Prevent perfect stacking

            rb.linearVelocity = (direction + avoidance) * moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero; // Stop movement when in range
        }
    }

    // **Function to prevent stacking**
    Vector2 AvoidStacking()
    {
        Vector2 avoidanceVector = Vector2.zero;
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, separationDistance);

        foreach (Collider2D col in nearbyEnemies)
        {
            if (col.gameObject != gameObject && col.CompareTag("Enemy")) // Check if it's another skeleton
            {
                Vector2 diff = (Vector2)transform.position - (Vector2)col.transform.position;
                avoidanceVector += diff.normalized; // Move slightly away
            }
        }

        return avoidanceVector * 0.5f; // Scale down the avoidance effect
    }

    void StartAttack()
    {
        if (!isAttacking && canAttack)
        {
            isAttacking = true;
            canAttack = false;
            attackRegistered = false;
            lockFlip = true; // Lock facing direction

            animator.SetBool("isWalking", false);
            animator.SetBool("isAttacking", true);

            rb.linearVelocity = Vector2.zero; // Stop movement while attacking
        }
    }

    // **Detect when the player enters attack range**
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            rb.linearVelocity = Vector2.zero; // Stop moving when player is in range
        }
    }

    // **Detect when the player leaves attack range**
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            CancelAttack();
        }
    }

    // **Called from Animation Event when the skeleton swings its weapon**
    public void RegisterHit()
    {
        if (attackRegistered) return;

        if (playerInRange)
        {
            player.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            Debug.Log("Enemy hit the player!");
            attackRegistered = true;
        }
        else
        {
            Debug.Log("Player dodged the attack!");
            CancelAttack();
        }
    }

    // **Called at the end of the attack animation (set as Animation Event)**
    public void EndAttack()
    {
        if (!playerInRange)
        {
            CancelAttack();
        }
        else
        {
            isAttacking = false;
            lockFlip = false; // Allow flipping again
            animator.SetBool("isAttacking", false);
            Invoke("ResetAttackCooldown", attackCooldown);
        }
    }

    void CancelAttack()
    {
        isAttacking = false;
        lockFlip = false; // Allow flipping again
        animator.SetBool("isAttacking", false);
        animator.SetBool("isWalking", true);
        rb.linearVelocity = Vector2.zero;

        Invoke("ResetAttackCooldown", attackCooldown);
    }

    void ResetAttackCooldown()
    {
        canAttack = true;
    }

    void FlipTowardsPlayer()
    {
        if (player == null) return;
        if (lockFlip) return; // Prevent flipping mid-attack

        if (player.position.x < transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }
}