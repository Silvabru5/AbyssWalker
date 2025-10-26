using UnityEngine;
using System.Collections;

// controls the dracula boss movement, jumping, and melee attack logic
// designed for 2d side-scrolling boss fights
public class BossControllerHybrid : MonoBehaviour
{
    [Header("movement settings")]
    [Tooltip("horizontal move speed when chasing the player")]
    public float moveSpeed = 2f;

    [Tooltip("maximum range within which the boss starts chasing")]
    public float chaseRange = 8f;

    [Tooltip("attack range within which the boss performs melee attacks")]
    public float attackRange = 2f;

    [Header("attack settings")]
    [Tooltip("cooldown time between melee attacks")]
    public float meleeCooldown = 2f;

    [Tooltip("reference to the melee hitbox gameobject")]
    public GameObject meleeHitbox;

    [Header("jump settings")]
    [Tooltip("desired jump height in unity units")]
    public float jumpHeight = 3f;

    [Tooltip("horizontal push force applied during a jump")]
    public float horizontalJumpForce = 4f;

    [Tooltip("how much higher the player must be for dracula to attempt a jump")]
    public float playerAboveThreshold = 1.5f;

    [Tooltip("maximum horizontal distance before considering a jump")]
    public float horizontalJumpRange = 4f;

    [Tooltip("distance below dracula used for ground detection")]
    public float groundCheckDistance = 0.2f;

    [Tooltip("layer mask used for detecting ground/platform surfaces")]
    public LayerMask groundLayer;

    [Header("jump tuning")]
    [SerializeField] private float jumpCooldown = 1.0f;              // seconds between jumps
    [SerializeField] private BossPlatformZone platformZone;          // where both must be inside to allow jumping
    [SerializeField] private BossPlatformZone dropZone;              // where player-only triggers “drop down”

    private bool isGrounded;
    private bool isJumping;
    private bool isAttacking;
    private bool facingRight = true;
    private bool isDead = false;

    private Transform player;
    private Rigidbody2D rb;
    private Animator anim;
    private float lastMeleeTime;
    private float spawnDelay = 1.0f;
    private float spawnTimer;
    private float lastJumpTime = 0f;

    private void Start()
    {
        player = GameObject.FindFirstObjectByType<PlayerSSBoss2>()?.transform;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        meleeHitbox?.SetActive(false);
        spawnTimer = spawnDelay;
    }

    private void Update()
    {
        if (isDead || player == null) return;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer > 0) return;

        CheckGrounded();

        if (isAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        // flip to face the player
        if ((player.position.x > transform.position.x && !facingRight) ||
            (player.position.x < transform.position.x && facingRight))
            Flip();

        // too far: stop moving
        if (distance > chaseRange)
        {
            anim.SetBool("isMoving", false);
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // within chase range: move or attack
        if (distance > attackRange)
            HandleMovement();
        else
            TryAttack();
    }

    // handles horizontal chasing movement and jump logic
    private void HandleMovement()
    {
        float verticalDiff = player.position.y - transform.position.y;

        // stop if player is too high with no platform path
        if (verticalDiff > playerAboveThreshold)
        {
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

    // checks if dracula is grounded using a downward raycast
    private void CheckGrounded()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        anim.SetBool("isGrounded", isGrounded);
    }

    // decides whether the boss should jump toward the player
    private void TryJumpToPlayer()
    {
        if (!isGrounded || isJumping || isAttacking || isDead)
            return;

        float vDiff = player.position.y - transform.position.y;
        float hDiff = Mathf.Abs(player.position.x - transform.position.x);
        bool playerRight = player.position.x > transform.position.x;

        bool jzP = platformZone != null && platformZone.playerInZone;
        bool jzB = platformZone != null && platformZone.bossInZone;

        // jump only if both in jump zone and player above
        if (platformZone != null && jzP && jzB)
        {
            if (vDiff > playerAboveThreshold && hDiff < horizontalJumpRange)
            {
                if (Time.time - lastJumpTime < jumpCooldown)
                    return;

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

        // default chase movement
        rb.linearVelocity = new Vector2((playerRight ? 1 : -1) * moveSpeed, rb.linearVelocity.y);
    }

    // performs the actual jump motion
    private IEnumerator JumpRoutine()
    {
        isJumping = true;
        anim.SetBool("isMoving", false);
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(0.15f);

        float gravity = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);
        float verticalVelocity = Mathf.Sqrt(2 * gravity * jumpHeight);
        float dirX = player.position.x > transform.position.x ? 1f : -1f;

        rb.linearVelocity = new Vector2(dirX * horizontalJumpForce, verticalVelocity);

        yield return new WaitUntil(() => Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer));

        isJumping = false;
        anim.SetBool("isMoving", true);
    }

    // checks if there’s a platform above
    private bool IsPlatformAboveReachable()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, jumpHeight * 2f, groundLayer);
        return hit.collider != null;
    }

    // tries to perform melee attack
    private void TryAttack()
    {
        if (isAttacking || Time.time < lastMeleeTime + meleeCooldown) return;
        StartCoroutine(DoMeleeAttack());
        lastMeleeTime = Time.time;
    }

    // performs the melee attack sequence
    private IEnumerator DoMeleeAttack()
    {
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;

        anim.ResetTrigger("attack");
        anim.SetTrigger("attack");

        DisableHitbox();
        yield return new WaitForSeconds(0.1f);

        EnableHitbox();

        float attackDuration = 0.8f;
        float elapsed = 0f;

        while (elapsed < attackDuration)
        {
            rb.linearVelocity = Vector2.zero;
            elapsed += Time.deltaTime;
            yield return null;
        }

        DisableHitbox();
        anim.ResetTrigger("attack");
        anim.Play("Boss_Idle", 0, 0f);
        anim.SetBool("isMoving", true);

        rb.linearVelocity = Vector2.zero;
        isAttacking = false;
    }

    // enables the melee hitbox
    public void EnableHitbox()
    {
        if (meleeHitbox != null)
        {
            meleeHitbox.SetActive(true);
            meleeHitbox.GetComponent<BossAttackHitbox>()?.ActivateHitbox();
        }
    }

    // disables the melee hitbox
    public void DisableHitbox()
    {
        if (meleeHitbox != null)
        {
            meleeHitbox.GetComponent<BossAttackHitbox>()?.DeactivateHitbox();
            meleeHitbox.SetActive(false);
        }
    }

    // called when dracula dies
    public void OnDeath()
    {
        isDead = true;
        isAttacking = false;
        isJumping = false;

        rb.linearVelocity = Vector2.zero;
        DisableHitbox();

        anim.ResetTrigger("attack");
        anim.SetBool("isMoving", false);
        enabled = false;
    }

    // flips dracula’s sprite when changing direction
    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 s = transform.localScale;
        s.x *= -1;
        transform.localScale = s;
        UpdateHitboxDirection();
    }

    // updates hitbox direction after flipping
    private void UpdateHitboxDirection()
    {
        if (meleeHitbox == null) return;

        Vector3 pos = meleeHitbox.transform.localPosition;
        pos.x = Mathf.Abs(pos.x) * (facingRight ? 1 : -1);
        meleeHitbox.transform.localPosition = pos;
    }
}
