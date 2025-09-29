//using UnityEngine;

//public class EnemyAI : MonoBehaviour
//{
//    //  public Transform player; // reference to the player's transform
//    public GameObject player;
//    public float moveSpeed = 2f; // how fast the enemy moves
//    public int damage = 10; // how much damage the enemy deals to the player
//    public float attackCooldown = 1.5f; // time between each attack
//    public float separationDistance = 0.7f; // how close enemies can be before pushing apart

//    private Animator animator; // handles animation transitions
//    private Rigidbody2D rb; // handles movement with physics
//    private SpriteRenderer spriteRenderer; // allows flipping the sprite left/right

//    [HideInInspector] public bool isAttacking = false; // true if currently attacking
//    [HideInInspector] public bool canAttack = true; // true if allowed to attack
//    [HideInInspector] public bool isDead = false; // true if enemy is dead and should stop logic

//    private bool attackRegistered = false; // prevents multiple hits in one attack
//    private bool playerInRange = false; // true when player is inside attack trigger zone
//    private bool lockFlip = false; // prevents flipping during attack

//    void Start()
//    {
//        player = GameObject.Find("Character");
//        // get all required components
//        animator = GetComponent<Animator>();
//        spriteRenderer = GetComponent<SpriteRenderer>();
//        rb = GetComponent<Rigidbody2D>();

//        // find the player in the scene using their health component (no tags used)
//        PlayerHealth foundPlayer = player.GetComponent<PlayerHealth>();
//            //Object.FindFirstObjectByType<PlayerHealth>();

//        //if (foundPlayer != null)
//        //{
//        //    player = foundPlayer.transform;
//        //}
//    }

//    void Update()
//    {
//        // stop logic if player is gone or enemy is dead
//        if (player == null || isDead) return;

//        // flip the enemy toward the player unless locked
//        if (!lockFlip) FlipTowardsPlayer();

//        // don't act if already attacking or cooling down
//        if (isAttacking || !canAttack) return;

//        // attack if close, otherwise move toward player
//        if (playerInRange)
//        {
//            StartAttack();
//        }
//        else
//        {
//            MoveTowardsPlayer();
//        }
//    }

//    void MoveTowardsPlayer()
//    {
//        if (!playerInRange && !isAttacking && canAttack)
//        {
//            animator.SetBool("isWalking", true);

//            Vector2 pos = rb.position;
//            Vector2 direction = ((Vector2)player.transform.position - pos).normalized;

//            // Check for obstacles ahead
//            Vector2 avoidance = AvoidObstacles(direction) + AvoidStacking();

//            Vector2 finalDir = (direction + avoidance).normalized;

//            rb.linearVelocity = finalDir * moveSpeed;
//        }
//        else
//        {
//            animator.SetBool("isWalking", false);
//            rb.linearVelocity = Vector2.zero;
//        }
//    }

//    Vector2 AvoidObstacles(Vector2 forwardDir)
//    {
//        float avoidDistance = 1.0f;    // how far to check ahead
//        float sideStep = 0.5f;         // how hard to sidestep if blocked
//        LayerMask obstacleMask = LayerMask.GetMask("Obstacles");

//        RaycastHit2D hit = Physics2D.Raycast(rb.position, forwardDir, avoidDistance, obstacleMask);

//        if (hit.collider != null)
//        {
//            // If we hit a wall, steer perpendicular to the surface normal
//            Vector2 perp = Vector2.Perpendicular(hit.normal).normalized;
//            return perp * sideStep;
//        }

//        return Vector2.zero;
//    }

//    Vector2 AvoidStacking()
//    {
//        // prevents enemies from standing in the exact same spot
//        Vector2 avoidanceVector = Vector2.zero;

//        Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, separationDistance);
//        foreach (Collider2D col in nearby)
//        {
//            if (col != null && col.gameObject != gameObject && col.GetComponent<EnemyAI>())
//            {
//                Vector2 diff = (Vector2)transform.position - (Vector2)col.transform.position;
//                avoidanceVector += diff.normalized;
//            }
//        }

//        return avoidanceVector * 0.5f; // scale down how strong the push is
//    }
//    // starts the attack if the enemy can attack and isn't already doing so
//    void StartAttack()
//    {
//        if (!isAttacking && canAttack)
//        {
//            //if (transform.GetComponent<isSkeleton>())    SoundManager.PlaySound(SoundTypeEffects.ENEMY_ATTACK_SKELETON, 1);
//            //else if (transform.GetComponent<isSpider>()) SoundManager.PlaySound(SoundTypeEffects.ENEMY_ATTACK_SPIDER, 1);
//            //else if (transform.GetComponent<isZombie>()) SoundManager.PlaySound(SoundTypeEffects.ENEMY_ATTACK_ZOMBIE, 1);
//            isAttacking = true;
//            canAttack = false;
//            attackRegistered = false;
//            lockFlip = true;

//            animator.SetBool("isWalking", false);
//            animator.SetBool("isAttacking", true);
//            rb.linearVelocity = Vector2.zero;
//        }
//    }
//    // runs when the player enters the enemy�s trigger zone
//    void OnTriggerEnter2D(Collider2D other)
//    {
//        if (other.GetComponent<PlayerHealth>() != null)
//        {
//            playerInRange = true;
//            rb.linearVelocity = Vector2.zero;
//        }
//    }

//    void OnTriggerExit2D(Collider2D other)
//    {
//        if (other.GetComponent<PlayerHealth>() != null)
//        {
//            playerInRange = false;
//            CancelAttack();
//        }
//    }

//    // called from animation event mid-swing
//    public void RegisterHit()
//    {
//        if (attackRegistered) return;

//        if (playerInRange && player != null)
//        {
//            PlayerHealth health = player.GetComponent<PlayerHealth>();
//            if (health != null)
//                health.TakeDamage(damage);

//            attackRegistered = true;
//        }
//        else
//        {
//            CancelAttack();
//        }
//    }

//    // called from animation event at end of attack
//    public void EndAttack()
//    {
//        isAttacking = false;
//        lockFlip = false;
//        animator.SetBool("isAttacking", false);
//        Invoke(nameof(ResetAttackCooldown), attackCooldown);
//    }
//    // stops the current attack and resets animations/movement
//    void CancelAttack()
//    {
//        isAttacking = false;
//        lockFlip = false;
//        animator.SetBool("isAttacking", false);
//        animator.SetBool("isWalking", false);
//        rb.linearVelocity = Vector2.zero;
//        Invoke(nameof(ResetAttackCooldown), attackCooldown);
//    }
//    // lets the enemy attack again after cooldown
//    void ResetAttackCooldown()
//    {
//        canAttack = true;
//    }

//    // interrupts the attack immediately (like when hit)
//    public void InterruptAttack()
//    {
//        isAttacking = false;
//        lockFlip = false;
//        canAttack = false;
//        animator.SetBool("isAttacking", false);
//        rb.linearVelocity = Vector2.zero;
//    }
//    // flips the enemy to face the player
//    void FlipTowardsPlayer()
//    {
//        if (player == null || lockFlip) return;

//        // flip sprite depending on whether player is to the left or right
//        spriteRenderer.flipX = player.transform.position.x < transform.position.x;
//    }
//}


//// This script controls basic enemy AI with the raycast addition to prevent enemies from walking into objects endlessly
//using System.Security.Cryptography;
//using UnityEngine;

//public class EnemyAI : MonoBehaviour
//{
//    public GameObject player;
//    public float moveSpeed = 2f;
//    public int damage = 10;
//    public float attackCooldown = 1.5f;
//    public float separationDistance = 0.7f;

//    private Vector2 avoidanceDirection = Vector2.zero;
//    private float avoidanceTimer = 0f;
//    private const float avoidanceDuration = 3f; // seconds to keep avoiding

//    private Animator animator;
//    private Rigidbody2D rb;
//    private SpriteRenderer spriteRenderer;

//    [HideInInspector] public bool isAttacking = false;
//    [HideInInspector] public bool canAttack = true;
//    [HideInInspector] public bool isDead = false;

//    private bool attackRegistered = false;
//    private bool playerInRange = false;
//    private bool lockFlip = false;

//    void Start()
//    {
//        player = GameObject.Find("Character");
//        animator = GetComponent<Animator>();
//        spriteRenderer = GetComponent<SpriteRenderer>();
//        rb = GetComponent<Rigidbody2D>();
//    }

//    void Update()
//    {
//        if (player == null || isDead) return;

//        if (!lockFlip) FlipTowardsPlayer();

//        if (isAttacking || !canAttack) return;

//        if (playerInRange)
//        {
//            StartAttack();
//        }
//        else
//        {
//            MoveTowardsPlayer();
//        }
//    }

//    void MoveTowardsPlayer()
//    {
//        if (!playerInRange && !isAttacking && canAttack)
//        {
//            animator.SetBool("isWalking", true);

//            Vector2 direction = (player.transform.position - transform.position).normalized;
//            float rayDistance = 1.6f;
//            LayerMask obstacleMask = LayerMask.GetMask("Obstacle");

//            // Check direct path
//            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, rayDistance, obstacleMask);
//            Debug.DrawRay(transform.position, direction * rayDistance, Color.red);

//            if (hit.collider != null)
//            {
//                // Try left and right perpendicular directions
//                Vector2 perpLeft = Vector2.Perpendicular(direction).normalized;
//                Vector2 perpRight = -perpLeft;

//                RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, perpLeft, rayDistance, obstacleMask);
//                RaycastHit2D hitRight = Physics2D.Raycast(transform.position, perpRight, rayDistance, obstacleMask);

//                // Prefer the side that is not blocked
//                if (hitLeft.collider == null)
//                {
//                    direction = perpLeft;
//                    Debug.DrawRay(transform.position, direction * rayDistance, Color.yellow);
//                }
//                else if (hitRight.collider == null)
//                {
//                    direction = perpRight;
//                    Debug.DrawRay(transform.position, direction * rayDistance, Color.yellow);
//                }
//                else
//                {
//                    // Both sides blocked, stop
//                    direction = Vector2.zero;
//                }
//            }

//            Vector2 avoidance = AvoidStacking();
//            Vector2 targetVelocity = (direction + avoidance).normalized * moveSpeed;
//            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity, 0.2f);
//        }
//        else
//        {
//            animator.SetBool("isWalking", false);
//            rb.linearVelocity = Vector2.zero;
//        }
//    }

//    Vector2 AvoidStacking()
//    {
//        Vector2 avoidanceVector = Vector2.zero;
//        Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, separationDistance);

//        foreach (Collider2D col in nearby)
//        {
//            if (col != null && col.gameObject != gameObject && col.GetComponent<EnemyAI>())
//            {
//                Vector2 diff = (Vector2)transform.position - (Vector2)col.transform.position;
//                avoidanceVector += diff.normalized;
//            }
//        }

//        return avoidanceVector * 0.5f;
//    }

//    void StartAttack()
//    {
//        if (!isAttacking && canAttack)
//        {
//            isAttacking = true;
//            canAttack = false;
//            attackRegistered = false;
//            lockFlip = true;

//            animator.SetBool("isWalking", false);
//            animator.SetBool("isAttacking", true);
//            rb.linearVelocity = Vector2.zero;
//        }
//    }

//    void OnTriggerEnter2D(Collider2D other)
//    {
//        if (other.GetComponent<PlayerHealth>() != null)
//        {
//            playerInRange = true;
//            rb.linearVelocity = Vector2.zero;
//        }
//    }

//    void OnTriggerExit2D(Collider2D other)
//    {
//        if (other.GetComponent<PlayerHealth>() != null)
//        {
//            playerInRange = false;
//            CancelAttack();
//        }
//    }

//    public void RegisterHit()
//    {
//        if (attackRegistered) return;

//        if (playerInRange && player != null)
//        {
//            PlayerHealth health = player.GetComponent<PlayerHealth>();
//            if (health != null)
//                health.TakeDamage(damage);

//            attackRegistered = true;
//        }
//        else
//        {
//            CancelAttack();
//        }
//    }

//    public void EndAttack()
//    {
//        isAttacking = false;
//        lockFlip = false;
//        animator.SetBool("isAttacking", false);
//        Invoke(nameof(ResetAttackCooldown), attackCooldown);
//    }

//    void CancelAttack()
//    {
//        isAttacking = false;
//        lockFlip = false;
//        animator.SetBool("isAttacking", false);
//        animator.SetBool("isWalking", false);
//        rb.linearVelocity = Vector2.zero;
//        Invoke(nameof(ResetAttackCooldown), attackCooldown);
//    }

//    void ResetAttackCooldown()
//    {
//        canAttack = true;
//    }

//    public void InterruptAttack()
//    {
//        isAttacking = false;
//        lockFlip = false;
//        canAttack = false;
//        animator.SetBool("isAttacking", false);
//        rb.linearVelocity = Vector2.zero;
//    }

//    void FlipTowardsPlayer()
//    {
//        if (player == null || lockFlip) return;
//        spriteRenderer.flipX = player.transform.position.x < transform.position.x;
//    }
//}


using UnityEngine;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    public GameObject player;
    public float moveSpeed = 2f;
    public int damage = 10;
    public float attackCooldown = 1.5f;
    public float separationDistance = 0.7f;

    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    [HideInInspector] public bool isAttacking = false;
    [HideInInspector] public bool canAttack = true;
    [HideInInspector] public bool isDead = false;

    private bool attackRegistered = false;
    private bool playerInRange = false;
    private bool lockFlip = false;

    // Avoidance persistence fields
    private Vector2 avoidanceDirection = Vector2.zero;
    private float avoidanceTimer = 0f;

    private float stuckTimer = 0f;
    private const float stuckThreshold = 2.0f; // seconds before forced escape
    private const float avoidanceDuration = 2.0f; // seconds to keep avoiding

    // Path memory fields
    private List<Vector2> recentBlockedDirections = new List<Vector2>();
    private const int maxMemory = 3; // Number of directions to remember

    void Start()
    {
        player = GameObject.Find("Character");
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (player == null || isDead) return;

        if (!lockFlip) FlipTowardsPlayer();

        if (isAttacking || !canAttack) return;

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

            Vector2 direction = (player.transform.position - transform.position).normalized;
            float rayDistance = 1f;
            float radius = 0.3f;
            LayerMask obstacleMask = LayerMask.GetMask("Obstacle");

            RaycastHit2D hitCenter = Physics2D.CircleCast(transform.position, radius, direction, rayDistance, obstacleMask);
            RaycastHit2D hitUp = Physics2D.CircleCast(transform.position, radius, direction + Vector2.up * 0.2f, rayDistance, obstacleMask);
            RaycastHit2D hitDown = Physics2D.CircleCast(transform.position, radius, direction + Vector2.down * 0.2f, rayDistance, obstacleMask);

            bool blocked = hitCenter.collider != null || hitUp.collider != null || hitDown.collider != null;

            Debug.DrawRay(transform.position, direction * rayDistance, Color.red);

            // Stuck detection
            if (blocked)
            {
                stuckTimer += Time.deltaTime;
                if (avoidanceTimer <= 0f)
                {
                    // Add blocked direction to memory
                    AddBlockedDirection(direction);

                    Vector2 perpLeft = Vector2.Perpendicular(direction).normalized;
                    Vector2 perpRight = -perpLeft;

                    RaycastHit2D hitLeft = Physics2D.CircleCast(transform.position, radius, perpLeft, rayDistance, obstacleMask);
                    RaycastHit2D hitRight = Physics2D.CircleCast(transform.position, radius, perpRight, rayDistance, obstacleMask);

                    // Prefer directions not recently blocked
                    if (hitLeft.collider == null && !IsDirectionBlocked(perpLeft))
                    {
                        direction = perpLeft;
                        Debug.DrawRay(transform.position, direction * rayDistance, Color.yellow);
                    }
                    else if (hitRight.collider == null && !IsDirectionBlocked(perpRight))
                    {
                        direction = perpRight;
                        Debug.DrawRay(transform.position, direction * rayDistance, Color.yellow);
                    }
                    else
                    {
                        Vector2 diagUpLeft = (perpLeft + Vector2.up).normalized;
                        Vector2 diagDownLeft = (perpLeft + Vector2.down).normalized;
                        Vector2 diagUpRight = (perpRight + Vector2.up).normalized;
                        Vector2 diagDownRight = (perpRight + Vector2.down).normalized;

                        RaycastHit2D hitDiagUpLeft = Physics2D.CircleCast(transform.position, radius, diagUpLeft, rayDistance, obstacleMask);
                        RaycastHit2D hitDiagDownLeft = Physics2D.CircleCast(transform.position, radius, diagDownLeft, rayDistance, obstacleMask);
                        RaycastHit2D hitDiagUpRight = Physics2D.CircleCast(transform.position, radius, diagUpRight, rayDistance, obstacleMask);
                        RaycastHit2D hitDiagDownRight = Physics2D.CircleCast(transform.position, radius, diagDownRight, rayDistance, obstacleMask);

                        if (hitDiagUpLeft.collider == null && !IsDirectionBlocked(diagUpLeft))
                        {
                            direction = diagUpLeft;
                            Debug.DrawRay(transform.position, direction * rayDistance, Color.magenta);
                        }
                        else if (hitDiagDownLeft.collider == null && !IsDirectionBlocked(diagDownLeft))
                        {
                            direction = diagDownLeft;
                            Debug.DrawRay(transform.position, direction * rayDistance, Color.magenta);
                        }
                        else if (hitDiagUpRight.collider == null && !IsDirectionBlocked(diagUpRight))
                        {
                            direction = diagUpRight;
                            Debug.DrawRay(transform.position, direction * rayDistance, Color.magenta);
                        }
                        else if (hitDiagDownRight.collider == null && !IsDirectionBlocked(diagDownRight))
                        {
                            direction = diagDownRight;
                            Debug.DrawRay(transform.position, direction * rayDistance, Color.magenta);
                        }
                        else
                        {
                            direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * moveSpeed;
                        }
                    }
                    avoidanceDirection = direction;
                    avoidanceTimer = avoidanceDuration;
                }
                else
                {
                    direction = avoidanceDirection;
                    avoidanceTimer -= Time.deltaTime;
                }

                // If stuck for too long, force random escape
                if (stuckTimer > stuckThreshold)
                {
                    direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * moveSpeed;
                    avoidanceDirection = direction;
                    avoidanceTimer = avoidanceDuration;
                    stuckTimer = 0f;
                }
            }
            else
            {
                avoidanceTimer = 0f;
                stuckTimer = 0f;
            }

            Vector2 avoidance = AvoidStacking(blocked);
            float lerpFactor = blocked ? 0.5f : 0.2f;
            Vector2 targetVelocity = (direction + avoidance).normalized * moveSpeed;
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity, lerpFactor);
        }
        else
        {
            animator.SetBool("isWalking", false);
            rb.linearVelocity = Vector2.zero;
        }
    }

    // Path memory helpers
    private void AddBlockedDirection(Vector2 dir)
    {
        // Store normalized direction
        dir = dir.normalized;
        if (!recentBlockedDirections.Exists(d => Vector2.Dot(d, dir) > 0.95f))
        {
            recentBlockedDirections.Add(dir);
            if (recentBlockedDirections.Count > maxMemory)
                recentBlockedDirections.RemoveAt(0);
        }
    }

    private bool IsDirectionBlocked(Vector2 dir)
    {
        dir = dir.normalized;
        return recentBlockedDirections.Exists(d => Vector2.Dot(d, dir) > 0.95f);
    }

    //Vector2 AvoidStacking()
    //{
    //    Vector2 avoidanceVector = Vector2.zero;
    //    Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, separationDistance);

    //    foreach (Collider2D col in nearby)
    //    {
    //        if (col != null && col.gameObject != gameObject && col.GetComponent<EnemyAI>())
    //        {
    //            Vector2 diff = (Vector2)transform.position - (Vector2)col.transform.position;
    //            avoidanceVector += diff.normalized;
    //        }
    //    }

    //    return avoidanceVector * 0.5f;
    //}
    Vector2 AvoidStacking(bool blocked)
    {
        Vector2 avoidanceVector = Vector2.zero;
        Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, separationDistance);

        foreach (Collider2D col in nearby)
        {
            if (col != null && col.gameObject != gameObject && col.GetComponent<EnemyAI>())
            {
                Vector2 diff = (Vector2)transform.position - (Vector2)col.transform.position;
                float dist = diff.magnitude;
                if (dist < separationDistance * 0.7f) // Only push if very close
                {
                    avoidanceVector += diff.normalized * (1.0f - dist / separationDistance);
                }
            }
        }

        // Smooth and scale avoidance
        float avoidanceScale = blocked ? 0.2f : 0.5f; // Less avoidance when blocked
        return avoidanceVector * avoidanceScale;
    }

    void StartAttack()
    {
        if (!isAttacking && canAttack)
        {
            isAttacking = true;
            canAttack = false;
            attackRegistered = false;
            lockFlip = true;

            animator.SetBool("isWalking", false);
            animator.SetBool("isAttacking", true);
            rb.linearVelocity = Vector2.zero;
        }
    }

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

    public void EndAttack()
    {
        isAttacking = false;
        lockFlip = false;
        animator.SetBool("isAttacking", false);
        Invoke(nameof(ResetAttackCooldown), attackCooldown);
    }

    void CancelAttack()
    {
        isAttacking = false;
        lockFlip = false;
        animator.SetBool("isAttacking", false);
        animator.SetBool("isWalking", false);
        rb.linearVelocity = Vector2.zero;
        Invoke(nameof(ResetAttackCooldown), attackCooldown);
    }

    void ResetAttackCooldown()
    {
        canAttack = true;
    }

    public void InterruptAttack()
    {
        isAttacking = false;
        lockFlip = false;
        canAttack = false;
        animator.SetBool("isAttacking", false);
        rb.linearVelocity = Vector2.zero;
    }

    void FlipTowardsPlayer()
    {
        if (player == null || lockFlip) return;
        spriteRenderer.flipX = player.transform.position.x < transform.position.x;
    }
}