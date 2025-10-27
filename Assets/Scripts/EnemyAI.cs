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
// END OF OLD SCRIPT REMOVE WHEN NEW SCRIPT CONFIRMED WORKING













// UPDATED SCRIPT TO AVOID OBSTACLES BETTER (Uses Raycasts)
// enemyai.cs
// this script controls basic enemy behavior in a 2d game. it makes the enemy chase the player, avoid obstacles using raycasts, and keeps enemies from stacking on top of each other.
// if the enemy gets close enough to the player, it starts an attack animation and deals damage. if an obstacle is in the way, the enemy picks a side to go around it and remembers that direction until the path is clear.
// the script also prevents enemies from getting stuck together by pushing them apart, and adds a little random movement if two enemies are blocking each other to break deadlocks.
// all movement uses physics (rigidbody2d), and the sprite flips to face the player. attack logic is handled with cooldowns and animation events.
// basically, this makes enemies feel smart and not get stuck or clump up, while still being simple and efficient for 2d games.
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyAI : MonoBehaviour
{
    // reference to the player object
    public GameObject player;
    PlayerHealth playerHealth;
    // how fast the enemy moves
    public float moveSpeed = 2f;
    // how much damage the enemy deals to the player
    public float damage = 10;
    // time between each attack
    public float attackCooldown = 1.5f;
    // how close enemies can be before pushing apart
    public float separationDistance = 0.7f;

    // animator handles animation transitions
    private Animator animator;
    // rigidbody2d handles movement with physics
    private Rigidbody2D rb;
    // lets us flip the sprite left/right
    private SpriteRenderer spriteRenderer;

    // these flags control attack and death logic
    [HideInInspector] public bool isAttacking = false;
    [HideInInspector] public bool canAttack = true;
    [HideInInspector] public bool isDead = false;

    // used to prevent multiple hits in one attack
    private bool attackRegistered = false;
    // true when player is inside attack trigger zone
    private bool playerInRange = false;
    // prevents flipping during attack
    private bool lockFlip = false;
    // true if we're currently avoiding an obstacle
    private bool isAvoiding = false;

    // stores the direction we're using to avoid obstacles
    private Vector2 avoidanceDirection = Vector2.zero;
    private string enemyType;

    void Start()
    {
        // find the player in the scene (by name)
        player = GameObject.FindWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
        // get all required components
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        enemyType = SoundManager.GetPlayerOrEnemyType(this.gameObject);

    }

    void Update()
    {
        // bail out if player is gone or we're dead
        if (player == null || isDead) return;

        // flip the enemy toward the player unless we're attacking
        if (!lockFlip) FlipTowardsPlayer();

        // don't do anything if we're attacking or can't attack yet
        if (isAttacking || !canAttack) return;

        // if player is close, start attack, otherwise chase them
        if (playerInRange)
        {
            StartAttack();
        }
        else
        {
            MoveTowardsPlayer();
        }
    }

    // handles all movement logic for the enemy
    // 1. only moves if not attacking, not dead, and not in attack range
    // 2. calculates direction to the player
    // 3. uses a circlecast to check for obstacles directly ahead
    // 4. if blocked, tries to pick a side (left/right) to go around the obstacle
    // 5. if both sides are blocked, tries diagonal directions (up-left, down-left, up-right, down-right)
    // 6. if all directions are blocked, picks a random direction to try and escape
    // 7. remembers the chosen avoidance direction until the path is clear
    // 8. also calculates an avoidance vector to keep enemies from stacking
    // 9. blends movement smoothly, moving more decisively when avoiding obstacles
    // 10. stops and plays idle animation if unable to move
    void MoveTowardsPlayer()
    {
        // 1. only moves if not attacking, not dead, and not in attack range
        if (!playerInRange && !isAttacking && canAttack)
        {
            animator.SetBool("isWalking", true);

            // 2. calculates direction to the player
            Vector2 direction = (player.transform.position - transform.position).normalized;
            float rayDistance = 1f;
            float radius = 0.3f;
            LayerMask obstacleMask = LayerMask.GetMask("Obstacle", "Border");

            // 3. uses a circlecast to check for obstacles directly ahead
            RaycastHit2D hitCenter = Physics2D.CircleCast(transform.position, radius, direction, rayDistance, obstacleMask);

            if (hitCenter.collider != null)
            {
                // 4. if blocked, tries to pick a side (left/right) to go around the obstacle
                if (!isAvoiding)
                {
                    Vector2 perpLeft = Vector2.Perpendicular(direction).normalized;
                    Vector2 perpRight = -perpLeft;

                    RaycastHit2D hitLeft = Physics2D.CircleCast(transform.position, radius, perpLeft, rayDistance, obstacleMask);
                    RaycastHit2D hitRight = Physics2D.CircleCast(transform.position, radius, perpRight, rayDistance, obstacleMask);

                    if (hitLeft.collider == null)
                    {
                        avoidanceDirection = perpLeft; // 4. left side clear
                    }
                    else if (hitRight.collider == null)
                    {
                        avoidanceDirection = perpRight; // 4. right side clear
                    }
                    else
                    {
                        // 5. if both sides are blocked, tries diagonal directions
                        Vector2 diagUpLeft = (perpLeft + Vector2.up).normalized;
                        Vector2 diagDownLeft = (perpLeft + Vector2.down).normalized;
                        Vector2 diagUpRight = (perpRight + Vector2.up).normalized;
                        Vector2 diagDownRight = (perpRight + Vector2.down).normalized;

                        RaycastHit2D hitDiagUpLeft = Physics2D.CircleCast(transform.position, radius, diagUpLeft, rayDistance, obstacleMask);
                        RaycastHit2D hitDiagDownLeft = Physics2D.CircleCast(transform.position, radius, diagDownLeft, rayDistance, obstacleMask);
                        RaycastHit2D hitDiagUpRight = Physics2D.CircleCast(transform.position, radius, diagUpRight, rayDistance, obstacleMask);
                        RaycastHit2D hitDiagDownRight = Physics2D.CircleCast(transform.position, radius, diagDownRight, rayDistance, obstacleMask);

                        if (hitDiagUpLeft.collider == null)
                        {
                            avoidanceDirection = diagUpLeft;
                        }
                        else if (hitDiagDownLeft.collider == null)
                        {
                            avoidanceDirection = diagDownLeft;
                        }
                        else if (hitDiagUpRight.collider == null)
                        {
                            avoidanceDirection = diagUpRight;
                        }
                        else if (hitDiagDownRight.collider == null)
                        {
                            avoidanceDirection = diagDownRight;
                        }
                        else
                        {
                            // 6. if all directions are blocked, picks a random direction to try and escape
                            avoidanceDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
                        }
                    }
                    isAvoiding = true; // 7. remembers the chosen avoidance direction until the path is clear
                }
                // 7. keep moving in the avoidance direction until the path is clear
                direction = avoidanceDirection;
            }
            else
            {
                // 7. path is clear, go straight toward the player
                isAvoiding = false;
                avoidanceDirection = Vector2.zero;
            }

            // 8. also calculates an avoidance vector to keep enemies from stacking
            Vector2 avoidance = AvoidStacking(isAvoiding);

            // 9. blends movement smoothly, moving more decisively when avoiding obstacles
            float lerpFactor = isAvoiding ? 0.5f : 0.2f;
            Vector2 targetVelocity = (direction + avoidance).normalized * moveSpeed;
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity, lerpFactor);
        }
        else
        {
            // 10. stops and plays idle animation if unable to move
            animator.SetBool("isWalking", false);
            rb.linearVelocity = Vector2.zero;
        }
    }

    // keeps enemies from clumping together, and adds a little randomness if they're stuck
    // 1. finds all nearby colliders within separationDistance
    // 2. for each nearby enemy, calculates a push-away vector
    // 3. combines all push vectors and scales down the strength
    // 4. if blocked and push is tiny, adds a little random wiggle to break deadlock
    // 5. returns the final avoidance vector
    Vector2 AvoidStacking(bool blocked)
    {
        Vector2 avoidanceVector = Vector2.zero;

        // 1. finds all nearby colliders within separationDistance
        Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, separationDistance);

        foreach (Collider2D col in nearby)
        {
            // 2. for each nearby enemy, calculates a push-away vector
            if (col != null && col.gameObject != gameObject && col.GetComponent<EnemyAI>())
            {
                Vector2 diff = (Vector2)transform.position - (Vector2)col.transform.position;
                avoidanceVector += diff.normalized;
            }
        }

        // 3. combines all push vectors and scales down the strength
        avoidanceVector *= 0.5f;

        // 4. if blocked and push is tiny, adds a little random wiggle to break deadlock
        if (blocked && avoidanceVector.magnitude < 0.1f)
        {
            avoidanceVector += new Vector2(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f));
        }

        // 5. returns the final avoidance vector
        return avoidanceVector;
    }

    // starts the attack if the enemy can attack and isn't already doing so
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

            // if an attack sound is defined in the enum, play it
            if (Enum.TryParse(enemyType + "_ATTACK", out SoundTypeEffects key))
                SoundManager.PlaySound(key);
        }
    }

    // runs when the player enters the enemy's trigger zone
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerHealth>() != null)
        {
            playerInRange = true;
            // rb.linearVelocity = Vector2.zero;
        }
    }

    // runs when the player leaves the enemy's trigger zone
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

        if (playerInRange)
        {
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log($"Player Hit! Damage: {damage}, Health: {playerHealth.currentHealth}");
            }
            else
            {
                Debug.LogWarning("PlayerHealth reference is null!");
            }
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
        // if the player was killed after this call but before it is complete, this action no longer needs to happen
        if (player == null) return; else isAttacking = false;
        if (player == null) return; else lockFlip = false;
        if (player == null) return; else canAttack = false;
        if (player == null) return; else animator.SetBool("isAttacking", false);
        if (player == null) return; else rb.linearVelocity = Vector2.zero;
    }

    // flips the enemy to face the player
    void FlipTowardsPlayer()
    {
        if (player == null || lockFlip) return;
        if (player == null) return; else spriteRenderer.flipX = player.transform.position.x < transform.position.x;
    }
}