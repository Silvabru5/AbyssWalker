using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    //  public Transform player; // reference to the player's transform
    public GameObject player;
    public float moveSpeed = 2f; // how fast the enemy moves
    public int damage = 10; // how much damage the enemy deals to the player
    public float attackCooldown = 1.5f; // time between each attack
    public float separationDistance = 0.7f; // how close enemies can be before pushing apart

    private Animator animator; // handles animation transitions
    private Rigidbody2D rb; // handles movement with physics
    private SpriteRenderer spriteRenderer; // allows flipping the sprite left/right

    [HideInInspector] public bool isAttacking = false; // true if currently attacking
    [HideInInspector] public bool canAttack = true; // true if allowed to attack
    [HideInInspector] public bool isDead = false; // true if enemy is dead and should stop logic

    private bool attackRegistered = false; // prevents multiple hits in one attack
    private bool playerInRange = false; // true when player is inside attack trigger zone
    private bool lockFlip = false; // prevents flipping during attack

    void Start()
    {
        player = GameObject.Find("Character");
        // get all required components
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        // find the player in the scene using their health component (no tags used)
        PlayerHealth foundPlayer = player.GetComponent<PlayerHealth>();
            //Object.FindFirstObjectByType<PlayerHealth>();

        //if (foundPlayer != null)
        //{
        //    player = foundPlayer.transform;
        //}
    }

    void Update()
    {
        // stop logic if player is gone or enemy is dead
        if (player == null || isDead) return;

        // flip the enemy toward the player unless locked
        if (!lockFlip) FlipTowardsPlayer();

        // don't act if already attacking or cooling down
        if (isAttacking || !canAttack) return;

        // attack if close, otherwise move toward player
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
        if (!playerInRange && !isAttacking && canAttack)
        {
            animator.SetBool("isWalking", true);

            // move toward player with slight push to avoid overlapping with others
            Vector2 direction = (player.transform.position - transform.position).normalized;
            Vector2 avoidance = AvoidStacking();
            rb.linearVelocity = (direction + avoidance) * moveSpeed;
        }
        else
        {
            // stop moving and walking animation
            animator.SetBool("isWalking", false);
            rb.linearVelocity = Vector2.zero;
        }
    }

    Vector2 AvoidStacking()
    {
        // prevents enemies from standing in the exact same spot
        Vector2 avoidanceVector = Vector2.zero;

        Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, separationDistance);
        foreach (Collider2D col in nearby)
        {
            if (col != null && col.gameObject != gameObject && col.GetComponent<EnemyAI>())
            {
                Vector2 diff = (Vector2)transform.position - (Vector2)col.transform.position;
                avoidanceVector += diff.normalized;
            }
        }

        return avoidanceVector * 0.5f; // scale down how strong the push is
    }
    // starts the attack if the enemy can attack and isn't already doing so
    void StartAttack()
    {
        if (!isAttacking && canAttack)
        {
            if (transform.GetComponent<isSkeleton>())    SoundManager.PlaySound(SoundTypeEffects.ENEMY_ATTACK_SKELETON, 1);
            else if (transform.GetComponent<isSpider>()) SoundManager.PlaySound(SoundTypeEffects.ENEMY_ATTACK_SPIDER, 1);
            else if (transform.GetComponent<isZombie>()) SoundManager.PlaySound(SoundTypeEffects.ENEMY_ATTACK_ZOMBIE, 1);
            isAttacking = true;
            canAttack = false;
            attackRegistered = false;
            lockFlip = true;

            animator.SetBool("isWalking", false);
            animator.SetBool("isAttacking", true);
            rb.linearVelocity = Vector2.zero;
        }
    }
    // runs when the player enters the enemy’s trigger zone
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerHealth>() != null)
        {
            playerInRange = true;
            rb.linearVelocity = Vector2.zero;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<PlayerHealth>() != null)
        {
            playerInRange = false;
            CancelAttack();
        }
    }

    // called from animation event mid-swing
    public void RegisterHit()
    {
        if (attackRegistered) return;

        if (playerInRange && player != null)
        {
            PlayerHealth health = player.GetComponent<PlayerHealth>();
            if (health != null)
                health.TakeDamage(damage);

            attackRegistered = true;
        }
        else
        {
            CancelAttack();
        }
    }

    // called from animation event at end of attack
    public void EndAttack()
    {
        isAttacking = false;
        lockFlip = false;
        animator.SetBool("isAttacking", false);
        Invoke(nameof(ResetAttackCooldown), attackCooldown);
    }
    // stops the current attack and resets animations/movement
    void CancelAttack()
    {
        isAttacking = false;
        lockFlip = false;
        animator.SetBool("isAttacking", false);
        animator.SetBool("isWalking", false);
        rb.linearVelocity = Vector2.zero;
        Invoke(nameof(ResetAttackCooldown), attackCooldown);
    }
    // lets the enemy attack again after cooldown
    void ResetAttackCooldown()
    {
        canAttack = true;
    }

    // interrupts the attack immediately (like when hit)
    public void InterruptAttack()
    {
        isAttacking = false;
        lockFlip = false;
        canAttack = false;
        animator.SetBool("isAttacking", false);
        rb.linearVelocity = Vector2.zero;
    }
    // flips the enemy to face the player
    void FlipTowardsPlayer()
    {
        if (player == null || lockFlip) return;

        // flip sprite depending on whether player is to the left or right
        spriteRenderer.flipX = player.transform.position.x < transform.position.x;
    }
}
