using UnityEngine;
using System.Collections;


// Controls the Dracula boss movement, jumping, and melee attack logic.
// Designed for 2D side-scrolling boss fights.

public class BossControllerHybrid : MonoBehaviour
{ 

    [Header("Movement Settings")]
    [Tooltip("Horizontal move speed when chasing the player.")]
    public float moveSpeed = 2f;

    [Tooltip("Maximum range within which the boss starts chasing.")]
    public float chaseRange = 8f;

    [Tooltip("Attack range within which the boss performs melee attacks.")]
    public float attackRange = 2f;


    [Header("Attack Settings")]
    [Tooltip("Cooldown time between melee attacks.")]
    public float meleeCooldown = 2f;

    [Tooltip("Reference to the melee hitbox GameObject.")]
    public GameObject meleeHitbox;


    [Header("Jump Settings")]
    [Tooltip("Desired jump height in Unity units.")]
    public float jumpHeight = 3f;

    [Tooltip("Horizontal push force applied during a jump.")]
    public float horizontalJumpForce = 4f;

    [Tooltip("How much higher the player must be for Dracula to attempt a jump.")]
    public float playerAboveThreshold = 1.5f;

    [Tooltip("Maximum horizontal distance before considering a jump.")]
    public float horizontalJumpRange = 4f;

    [Tooltip("Distance below Dracula used for ground detection.")]
    public float groundCheckDistance = 0.2f;

    [Tooltip("Layer mask used for detecting ground/platform surfaces.")]
    public LayerMask groundLayer;
    [Header("Jump Tuning")]
    [SerializeField] private float minHorizontalForJump = 0.9f;   // how far left/right from Dracula before a jump is allowed
    [SerializeField] private float jumpAnticipation = 0.08f;       // small wind-up before jump
    [SerializeField] private float maxRepositionTime = 0.5f;       // how long to sidestep before giving up and jumping
    [SerializeField] private float jumpCooldown = 1.0f;            // seconds between jumps
    [SerializeField] private BossPlatformZone platformZone; // where both must be inside to allow jumping up
    [SerializeField] private BossPlatformZone dropZone; // where player-only triggers “drop down”
                                                        
    [SerializeField] private float stuckTimeThreshold = 1.5f;  // Time boss must be stuck before picking a new direction
    [SerializeField] private float ledgeCheckLength = 1.0f;    // How far ahead to check for a ledge
    [SerializeField] private float dropSpeed = -8f;            // Vertical speed when dropping

    // Add these new fields at the top of your class
    [SerializeField] private float ledgeCheckDistance = 1.5f;
    [SerializeField] private float ledgeCheckOffset = 0.6f;

    private bool isGrounded;
    private bool isJumping;
    private bool isAttacking;
    private bool facingRight = true;
    private bool isDead = false;
    // Debug tracking of zone flags to log only on changes
    private bool lastSeenPlayerInZone = false;
    private bool lastSeenBossInZone = false;

    private Transform player;
    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D bossCollider;
    private Collider2D playerCollider;

    private float lastMeleeTime;
    private float spawnDelay = 1.0f;
    private float spawnTimer;
    private float lastJumpTime = 0f;
    


    private void Start()
    {
        player = GameObject.FindFirstObjectByType<PlayerSSBoss2>()?.transform;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        bossCollider = GetComponent<Collider2D>();

        if (player != null)
            playerCollider = player.GetComponent<Collider2D>();
        
        if (platformZone == null)
            Debug.LogWarning("[Boss] platformZone reference is NULL. Boss will not be able to enforce zone-based jumping.");
        else
            Debug.Log("[Boss] platformZone hooked up: " + platformZone.name);
        
        meleeHitbox?.SetActive(false);
        spawnTimer = spawnDelay;
    }

    private void TraceZoneFlagsChange()
    {
        if (platformZone == null) return;

        if (platformZone.playerInZone != lastSeenPlayerInZone)
        {
            Debug.Log("[Boss] Zone flag change: playerInZone=" + platformZone.playerInZone);
            lastSeenPlayerInZone = platformZone.playerInZone;
        }

        if (platformZone.bossInZone != lastSeenBossInZone)
        {
            Debug.Log("[Boss] Zone flag change: bossInZone=" + platformZone.bossInZone);
            lastSeenBossInZone = platformZone.bossInZone;
        }
    }

    private void Update()
    {
        if (isDead) return;
        if (player == null) return;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer > 0) return;
        // track zone flag transitions
        TraceZoneFlagsChange();

        CheckGrounded();

        if (isAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        // Flip to face the player
        if ((player.position.x > transform.position.x && !facingRight) ||
            (player.position.x < transform.position.x && facingRight))
            Flip();

        // Too far: stop moving
        if (distance > chaseRange)
        {
            anim.SetBool("isMoving", false);
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Within chase range: move or attack
        if (distance > attackRange)
            HandleMovement();
        else
            TryAttack();
    }



    // Handles basic horizontal chasing movement and jump logic.
    private void HandleMovement()
    {
        // Prevent moving if player is directly above with no platform path
        float verticalDiff = player.position.y - transform.position.y;
        if (verticalDiff > playerAboveThreshold)
        {
            // Check if there’s no nearby slope or stair to climb
            bool wallAhead = Physics2D.Raycast(transform.position, facingRight ? Vector2.right : Vector2.left, 0.8f, groundLayer);
            if (wallAhead)
            {
                rb.linearVelocity = Vector2.zero;
                anim.SetBool("isMoving", false);
                return;
            }
        }

        if (meleeHitbox.activeSelf)
            DisableHitbox();
        
        anim.SetBool("isMoving", true);
        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(dir.x * moveSpeed, rb.linearVelocity.y);

        TryJumpToPlayer();
    }

    // Checks if Dracula is currently grounded using a downward raycast.
    private void CheckGrounded()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        anim.SetBool("isGrounded", isGrounded);
    }


    private void TryJumpToPlayer()
    {
        if (!isGrounded || isJumping || isAttacking || isDead)
            return;

        float vDiff = player.position.y - transform.position.y;
        float hDiff = Mathf.Abs(player.position.x - transform.position.x);
        bool playerRight = player.position.x > transform.position.x;

        bool jzP = platformZone != null && platformZone.playerInZone;
        bool jzB = platformZone != null && platformZone.bossInZone;
        bool dzP = dropZone != null && dropZone.playerInZone;
        bool dzB = dropZone != null && dropZone.bossInZone;

        Debug.Log($"[BossZones] jumpZone(P,B)=({jzP},{jzB}) dropZone(P,B)=({dzP},{dzB}) vDiff={vDiff:F2} hDiff={hDiff:F2}");



        // JUMP UP 
        if (platformZone != null && jzP && jzB)
        {
            // Only jump if player is above and within horizontal reach
            if (vDiff > playerAboveThreshold && hDiff < horizontalJumpRange)
            {
                if (Time.time - lastJumpTime < jumpCooldown)
                    return;

                // Prevent jumping directly under player
                if (hDiff < 0.8f)
                {
                    float side = playerRight ? -1f : 1f;
                    Debug.Log("[BossJump] Player above & too close — sidestep instead of jump.");
                    rb.linearVelocity = new Vector2(side * moveSpeed * 0.6f, rb.linearVelocity.y);
                    return;
                }

                Vector2 _;
                if (IsPlatformAboveReachable(out _))
                {
                    Debug.Log("[BossJump] Both in jump zone — jumping toward player/platform.");
                    StartCoroutine(JumpRoutine());
                    lastJumpTime = Time.time;
                    return;
                }
                else
                {
                    Debug.Log("[BossJump] Platform above not detected — skipping jump.");
                }
            }
        }
        // DEFAULT CHASE
        rb.linearVelocity = new Vector2((playerRight ? 1 : -1) * moveSpeed, rb.linearVelocity.y);
        Debug.Log("[BossChase] Normal chase.");
    }

    // Executes the jump toward the player's platform.
    private IEnumerator JumpRoutine()
    {
        isJumping = true;
        anim.SetBool("isMoving", false);
        rb.linearVelocity = Vector2.zero;

        // Re-validate zone right before takeoff
        if (platformZone != null && (!platformZone.playerInZone || !platformZone.bossInZone))
        {
            Debug.Log("[BossJump] Abort at takeoff: zone invalid now. playerInZone=" + platformZone.playerInZone + ", bossInZone=" + platformZone.bossInZone);
            isJumping = false;
            anim.SetBool("isMoving", true);
            yield break;
        }

        yield return new WaitForSeconds(0.15f);

        // One more guard before the actual impulse
        if (platformZone != null && (!platformZone.playerInZone || !platformZone.bossInZone))
        {
            Debug.Log("[BossJump] Abort right before impulse: zone invalid. playerInZone=" + platformZone.playerInZone + ", bossInZone=" + platformZone.bossInZone);
            isJumping = false;
            anim.SetBool("isMoving", true);
            yield break;
        }

        float gravity = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);
        float verticalVelocity = Mathf.Sqrt(2 * gravity * jumpHeight);
        float dirX = player.position.x > transform.position.x ? 1f : -1f;

        rb.linearVelocity = new Vector2(dirX * horizontalJumpForce, verticalVelocity);
        Debug.Log("[BossJump] Jump started. vel=" + rb.linearVelocity + " time=" + Time.time.ToString("F2"));

        yield return new WaitUntil(() => Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer));

        isJumping = false;
        anim.SetBool("isMoving", true);
        Debug.Log("[BossJump] Landed. time=" + Time.time.ToString("F2"));
    }


    // Detect if the player is above and reachable via platform
    private bool IsPlatformAboveReachable(out Vector2 platformPos)
    {
        platformPos = Vector2.zero;

        // Cast upward to detect a ground layer (a platform) above
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, jumpHeight * 2f, groundLayer);

        if (hit.collider != null)
        {
            platformPos = hit.point;
            Debug.Log("[Boss] Found platform above at " + platformPos);
            return true;
        }

        return false;
    }



    // Attempts a melee attack if cooldown is over.

    private void TryAttack()
    {
        if (isAttacking || Time.time < lastMeleeTime + meleeCooldown) return;
        StartCoroutine(DoMeleeAttack());
        lastMeleeTime = Time.time;
    }


    // Handles the melee attack coroutine 
    private IEnumerator DoMeleeAttack()
    {
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;

        anim.ResetTrigger("attack");
        anim.SetTrigger("attack");

        // Safety: always disable before enabling (prevents leftover states)
        DisableHitbox();

        yield return new WaitForSeconds(0.1f); // short wind-up before hit

        // Enable hitbox during the swing
        EnableHitbox();

        float attackDuration = 0.8f;   // total time the swing is active
        float cancelDistance = attackRange + 1.0f; // distance at which boss cancels attack
        float elapsed = 0f;

        while (elapsed < attackDuration)
        {
            // Cancel attack if player moves too far away
            float dx = Mathf.Abs(player.position.x - transform.position.x);
            if (dx > cancelDistance)
            {
                
                DisableHitbox();

                // Force cancel attack animation immediately
                anim.ResetTrigger("attack");
                anim.Play("Boss_Idle", 0, 0f); // change "Idle" to the name of your movement or idle anim
                rb.linearVelocity = Vector2.zero;
                anim.SetBool("isMoving", true);

                isAttacking = false;
                yield break; // stop attack early
            }

            // Keep boss frozen while attacking
            rb.linearVelocity = Vector2.zero;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Attack finished normally
        DisableHitbox();

        // Return to moving/chasing animation
        anim.ResetTrigger("attack");
        anim.Play("Boss_Idle", 0, 0f); // change "Idle" if your move anim has a different name
        anim.SetBool("isMoving", true);
        rb.linearVelocity = Vector2.zero;

        isAttacking = false;
    }




    public void EnableHitbox()
    {
        if (meleeHitbox != null)
        {
            meleeHitbox.SetActive(true); // make object visible/active in hierarchy
            meleeHitbox.GetComponent<BossAttackHitbox>()?.ActivateHitbox();
            
        }
    }

    public void DisableHitbox()
    {
        if (meleeHitbox != null)
        {
            meleeHitbox.GetComponent<BossAttackHitbox>()?.DeactivateHitbox();
            meleeHitbox.SetActive(false);
           
        }
    }


    public void OnDeath()
    {
        isDead = true;
        isAttacking = false;
        rb.linearVelocity = Vector2.zero;
        DisableHitbox();

        anim.ResetTrigger("attack");
        anim.SetBool("isMoving", false);
        anim.Play("Death", 0, 0f);
        enabled = false;

    }



    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 s = transform.localScale;
        s.x *= -1;
        transform.localScale = s;
        // Adjust hitbox side when flipping
        UpdateHitboxDirection();
    }


    private void UpdateHitboxDirection()
    {
        if (meleeHitbox == null) return;

        // Assume the hitbox starts on the right side of Dracula by default
        Vector3 pos = meleeHitbox.transform.localPosition;
        pos.x = Mathf.Abs(pos.x) * (facingRight ? 1 : -1);
        meleeHitbox.transform.localPosition = pos;
    }
}

