using UnityEngine;
using System.Collections;

/*
    Author(s): Bruno Silva
    Description: controls the dracula boss during the 2d side-scrolling fight. 
                 handles chasing, jumping, melee attacks, hitbox activation, 
                 ground checks, and AI decision-making. designed for hybrid 
                 movement that mixes platforming with close-range combat.
    Date (last modification): 11/22/2025
*/

public class BossControllerHybrid : MonoBehaviour
{
    [Header("movement settings")]
    [Tooltip("horizontal move speed when chasing the player")]
    public float moveSpeed = 2f;

    [Tooltip("distance from player before the boss starts chasing")]
    public float chaseRange = 8f;

    [Tooltip("range at which the boss will stop moving and attempt a melee attack")]
    public float attackRange = 2f;

    [Header("attack settings")]
    [Tooltip("cooldown time between each melee attack")]
    public float meleeCooldown = 2f;

    [Tooltip("melee hitbox object to enable during attacks")]
    public GameObject meleeHitbox;

    [Header("jump settings")]
    [Tooltip("how high the boss jumps in unity units")]
    public float jumpHeight = 3f;

    [Tooltip("horizontal force applied when jumping toward the player")]
    public float horizontalJumpForce = 4f;

    [Tooltip("how much higher the player must be before the boss considers jumping")]
    public float playerAboveThreshold = 1.5f;

    [Tooltip("maximum horizontal distance before the boss considers jumping")]
    public float horizontalJumpRange = 4f;

    [Tooltip("raycast distance below the boss to check for ground")]
    public float groundCheckDistance = 0.2f;

    [Tooltip("layer mask for ground/platform detection")]
    public LayerMask groundLayer;

    [Header("jump tuning")]
    [SerializeField] private float jumpCooldown = 1.0f;             // time between allowed jumps
    [SerializeField] private BossPlatformZone platformZone;         // area where both player + boss must be to allow jumps
    [SerializeField] private BossPlatformZone dropZone;             // not used here, but intended for platform drop logic

    private bool isGrounded;
    private bool isJumping;
    private bool isAttacking;
    private bool facingRight = true;
    private bool isDead = false;

    private GameObject player;                                      // reference to player object
    private Rigidbody2D rb;
    private Animator anim;

    private float lastMeleeTime;
    private float spawnDelay = 1.0f;
    private float spawnTimer;
    private float lastJumpTime = 0f;

    private void Start()
    {
        // find the player by tag (this scene uses tag-based detection)
        player = GameObject.FindWithTag("Player");

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // ensure hitbox starts disabled
        meleeHitbox?.SetActive(false);

        // delay boss behavior slightly after spawning
        spawnTimer = spawnDelay;
    }

    private void Update()
    {
        if (isDead || player == null)
            return;

        // wait for spawn delay before activating full behavior
        spawnTimer -= Time.deltaTime;
        if (spawnTimer > 0)
            return;

        CheckGrounded();

        // do not move when attacking
        if (isAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float distance = Vector2.Distance(transform.position, player.transform.position);

        // ensure boss faces the player at all times
        if ((player.transform.position.x > transform.position.x && !facingRight) ||
            (player.transform.position.x < transform.position.x && facingRight))
        {
            Flip();
        }

        // outside chase range → idle
        if (distance > chaseRange)
        {
            anim.SetBool("isMoving", false);
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // within chase range → move or attack depending on distance
        if (distance > attackRange && !isDead)
            HandleMovement();
        else
            TryAttack();
    }

    // handles horizontal movement logic while tracking the player
    private void HandleMovement()
    {
        float verticalDiff = player.transform.position.y - transform.position.y;

        // if player is higher and a wall is in the way, stop instead of walking uselessly into it
        if (verticalDiff > playerAboveThreshold)
        {
            bool wallAhead = Physics2D.Raycast(transform.position,
                                               facingRight ? Vector2.right : Vector2.left,
                                               0.8f,
                                               groundLayer);

            if (wallAhead)
            {
                rb.linearVelocity = Vector2.zero;
                anim.SetBool("isMoving", false);
                return;
            }
        }

        // always ensure hitbox is disabled while moving
        if (meleeHitbox.activeSelf)
            DisableHitbox();

        anim.SetBool("isMoving", true);

        // move toward the player
        Vector2 dir = (player.transform.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(dir.x * moveSpeed, rb.linearVelocity.y);

        // decide whether to attempt a jump
        TryJumpToPlayer();
    }

    // raycast to check if the boss is grounded
    private void CheckGrounded()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        anim.SetBool("isGrounded", isGrounded);
    }

    // decides when to jump toward the player based on height and distance
    private void TryJumpToPlayer()
    {
        if (!isGrounded || isJumping || isAttacking || isDead)
            return;

        float vDiff = player.transform.position.y - transform.position.y;
        float hDiff = Mathf.Abs(player.transform.position.x - transform.position.x);
        bool playerRight = player.transform.position.x > transform.position.x;

        bool jzP = platformZone != null && platformZone.playerInZone;
        bool jzB = platformZone != null && platformZone.bossInZone;

        // only jump if both are in a dedicated “jump zone”
        if (platformZone != null && jzP && jzB)
        {
            // player must be above, and within horizontal range, and jump cooldown must be ready
            if (vDiff > playerAboveThreshold && hDiff < horizontalJumpRange)
            {
                if (Time.time - lastJumpTime < jumpCooldown)
                    return;

                // small horizontal adjustment before jumping
                if (hDiff < 0.8f)
                {
                    float side = playerRight ? -1f : 1f;
                    rb.linearVelocity = new Vector2(side * moveSpeed * 0.6f, rb.linearVelocity.y);
                    return;
                }

                if (IsPlatformAboveReachable())
                {
                    StartCoroutine(JumpRoutine());
                    lastJumpTime = Time.time;
                    return;
                }
            }
        }

        // fallback movement (normal chase)
        rb.linearVelocity = new Vector2((playerRight ? 1 : -1) * moveSpeed, rb.linearVelocity.y);
    }

    // performs the actual jump arc toward the player
    private IEnumerator JumpRoutine()
    {
        isJumping = true;
        anim.SetBool("isMoving", false);
        rb.linearVelocity = Vector2.zero;

        // brief delay before launch
        yield return new WaitForSeconds(0.15f);

        float gravity = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);

        // calculate vertical speed needed to reach desired height
        float verticalVelocity = Mathf.Sqrt(2 * gravity * jumpHeight);
        float dirX = player.transform.position.x > transform.position.x ? 1f : -1f;

        rb.linearVelocity = new Vector2(dirX * horizontalJumpForce, verticalVelocity);

        // wait until boss lands
        yield return new WaitUntil(() =>
            Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer)
        );

        isJumping = false;
        anim.SetBool("isMoving", true);
    }

    // quick upward raycast to confirm if a platform above is reachable by jump
    private bool IsPlatformAboveReachable()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, jumpHeight * 2f, groundLayer);
        return hit.collider != null;
    }

    // attempts to activate melee attack if ready
    private void TryAttack()
    {
        if (isAttacking || Time.time < lastMeleeTime + meleeCooldown)
            return;

        StartCoroutine(DoMeleeAttack());
        lastMeleeTime = Time.time;
    }

    // performs the full melee attack animation and hitbox timing
    private IEnumerator DoMeleeAttack()
    {
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;

        anim.ResetTrigger("attack");
        anim.SetTrigger("attack");
        anim.SetBool("isAttacking", true);

        DisableHitbox();
        yield return new WaitForSeconds(0.1f);     // wait for wind-up before hitbox

        EnableHitbox();

        float attackDuration = 0.8f;
        float elapsed = 0f;

        // keep boss still while attacking
        while (elapsed < attackDuration)
        {
            rb.linearVelocity = Vector2.zero;
            elapsed += Time.deltaTime;
            yield return null;
        }

        DisableHitbox();
        anim.ResetTrigger("attack");
        anim.SetBool("isMoving", false);

        rb.linearVelocity = Vector2.zero;
        isAttacking = false;
        anim.SetBool("isAttacking", false);
    }

    // enables the melee hitbox + script logic
    public void EnableHitbox()
    {
        if (meleeHitbox != null)
        {
            meleeHitbox.SetActive(true);
            meleeHitbox.GetComponent<BossAttackHitbox>()?.ActivateHitbox();
        }
    }

    // disables melee hitbox + script logic
    public void DisableHitbox()
    {
        if (meleeHitbox != null)
        {
            meleeHitbox.GetComponent<BossAttackHitbox>()?.DeactivateHitbox();
            meleeHitbox.SetActive(false);
        }
    }

    // called when the boss dies (from BossHealth)
    public void OnDeath()
    {
        isDead = true;
        isAttacking = false;
        isJumping = false;

        rb.linearVelocity = Vector2.zero;
        DisableHitbox();

        anim.SetBool("isMoving", false);
        enabled = false;
    }

    // flips sprite direction and updates hitbox position
    private void Flip()
    {
        facingRight = !facingRight;

        Vector3 s = transform.localScale;
        s.x *= -1;
        transform.localScale = s;

        UpdateHitboxDirection();
    }

    // mirrors the melee hitbox position when flipping
    private void UpdateHitboxDirection()
    {
        if (meleeHitbox == null)
            return;

        Vector3 pos = meleeHitbox.transform.localPosition;
        pos.x = Mathf.Abs(pos.x) * (facingRight ? 1 : -1);
        meleeHitbox.transform.localPosition = pos;
    }
}
