

////using UnityEngine;
////using System.Collections;

////public class BossControllerHybrid : MonoBehaviour
////{
////    [Header("Movement Settings")]
////    public float moveSpeed = 2f;
////    public float chaseRange = 8f;
////    public float attackRange = 2f;

////    [Header("Attack Settings")]
////    public float meleeCooldown = 2f;
////    public GameObject meleeHitbox;

////    [Header("Jump Settings")]
////    public float jumpForce = 10f;               // Adjust this for Dracula’s jump height
////    public float jumpCheckDistance = 1.5f;      // How far ahead to check for walls
////    public float groundCheckDistance = 0.2f;
////    public LayerMask groundLayer;               // Assign to “Ground” layer in Inspector

////    private bool isGrounded;
////    private bool isJumping;

////    private Transform player;
////    private Rigidbody2D rb;
////    private Animator anim;

////    private bool facingRight = true;
////    private bool isAttacking;
////    private bool isMoving;
////    private float lastMeleeTime;
////    private float spawnDelay = 1.0f;
////    private float spawnTimer;

////    void Start()
////    {
////        player = GameObject.FindFirstObjectByType<PlayerSSBoss2>()?.transform;
////        rb = GetComponent<Rigidbody2D>();
////        anim = GetComponent<Animator>();
////        meleeHitbox?.SetActive(false);
////        spawnTimer = spawnDelay;
////    }

////    void Update()
////    {
////        if (player == null) return;

////        spawnTimer -= Time.deltaTime;
////        if (spawnTimer > 0) return;

////        if (isAttacking)
////        {
////            rb.linearVelocity = Vector2.zero;
////            return;
////        }

////        float distance = Vector2.Distance(transform.position, player.position);

////        // Face the player
////        if ((player.position.x > transform.position.x && !facingRight) ||
////            (player.position.x < transform.position.x && facingRight))
////        {
////            Flip();
////        }

////        // Stop if too far
////        if (distance > chaseRange)
////        {
////            if (!isAttacking)
////                anim.SetBool("isMoving", false);

////            rb.linearVelocity = Vector2.zero;
////            return;
////        }

////        // Try to attack or move
////        if (distance > attackRange)
////        {
////            MoveTowardPlayer();
////        }
////        else
////        {
////            TryAttack();
////        }
////    }

////    private void MoveTowardPlayer()
////    {
////        isMoving = true;
////        anim.SetBool("isMoving", true);


////        Vector2 direction = (player.position - transform.position).normalized;

////        // Handle ground + jump detection
////        CheckGrounded();
////        DetectAndJump(direction);

////        // Move horizontally (ignore y when jumping)
////        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
////    }

////    private void CheckGrounded()
////    {
////        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
////        isGrounded = hit.collider != null;
////    }

////    private void DetectAndJump(Vector2 direction)
////    {
////        // Don’t spam jump if already in air
////        if (!isGrounded || isJumping) return;

////        // Check for obstacle in front
////        Vector2 rayDir = facingRight ? Vector2.right : Vector2.left;
////        RaycastHit2D wallCheck = Physics2D.Raycast(transform.position, rayDir, jumpCheckDistance, groundLayer);

////        // Check if player is above boss by a certain height
////        bool playerAbove = player.position.y - transform.position.y > 1.0f;

////        if (wallCheck.collider != null && playerAbove)
////        {
////            StartCoroutine(JumpRoutine());
////        }
////    }

////    private IEnumerator JumpRoutine()
////    {
////        isJumping = true;
////        anim.SetTrigger("jump");

////        yield return new WaitForSeconds(0.05f);

////        // Manual jump override (works despite 999 mass)
////        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

////        // Wait until grounded again
////        yield return new WaitUntil(() => isGrounded);
////        isJumping = false;
////    }

////    private void TryAttack()
////    {
////        if (isAttacking) return;
////        if (Time.time < lastMeleeTime + meleeCooldown) return;

////        Debug.Log("Boss trying to attack");
////        StartCoroutine(DoMeleeAttack());
////        lastMeleeTime = Time.time;
////    }

////    private IEnumerator DoMeleeAttack()
////    {
////        isAttacking = true;
////        rb.linearVelocity = Vector2.zero;

////        anim.ResetTrigger("attack");
////        anim.SetTrigger("attack");
////        Debug.Log("Boss attack triggered");

////        yield return new WaitForSeconds(0.05f);

////        float attackDuration = 1.0f;
////        float cancelDelay = 0.3f;
////        float elapsed = 0f;

////        while (elapsed < attackDuration)
////        {
////            rb.linearVelocity = Vector2.zero;

////            float dx = Mathf.Abs(player.position.x - transform.position.x);
////            if (elapsed > cancelDelay && dx > attackRange + 1.0f)
////            {
////                Debug.Log("Player escaped attack range — cancelling attack early");
////                anim.ResetTrigger("attack");
////                anim.SetBool("isMoving", true);
////                isAttacking = false;
////                anim.CrossFade("Running", 0.05f);
////                yield break;
////            }

////            elapsed += Time.deltaTime;
////            yield return null;
////        }

////        anim.ResetTrigger("attack");
////        anim.SetBool("isMoving", true);
////        isAttacking = false;
////    }

////    public void EnableHitbox()
////    {
////        if (meleeHitbox == null) return;
////        var bossHitbox = meleeHitbox.GetComponent<BossAttackHitbox>();
////        if (bossHitbox != null) bossHitbox.ActivateHitbox();
////    }

////    public void DisableHitbox()
////    {
////        if (meleeHitbox == null) return;
////        var bossHitbox = meleeHitbox.GetComponent<BossAttackHitbox>();
////        if (bossHitbox != null) bossHitbox.DeactivateHitbox();
////    }

////    private void Flip()
////    {
////        facingRight = !facingRight;
////        Vector3 s = transform.localScale;
////        s.x *= -1;
////        transform.localScale = s;
////    }
////}
////using UnityEngine;
////using System.Collections;

////public class BossControllerHybrid : MonoBehaviour
////{
////    [Header("Movement Settings")]
////    public float moveSpeed = 2f;
////    public float chaseRange = 8f;
////    public float attackRange = 2f;

////    [Header("Attack Settings")]
////    public float meleeCooldown = 2f;
////    public GameObject meleeHitbox;

////    [Header("Jump Settings")]
////    public float jumpForce = 10f;                 // Adjust this for Dracula’s jump height
////    public float jumpCheckDistance = 1.5f;        // Distance to check for obstacles in front
////    public float groundCheckDistance = 0.2f;      // Distance for checking ground under boss
////    public float playerAboveThreshold = 1.5f;     // How much higher the player must be
////    public float horizontalJumpRange = 3f;        // Max horizontal distance before jumping
////    public LayerMask groundLayer;                 // Assign to “Ground” and “Platform” layers
////    public LayerMask ceilingLayer;
////    private bool isGrounded;
////    private bool isJumping;

////    private Transform player;
////    private Rigidbody2D rb;
////    private Animator anim;

////    private bool facingRight = true;
////    private bool isAttacking;
////    private bool isMoving;
////    private float lastMeleeTime;
////    private float spawnDelay = 1.0f;
////    private float spawnTimer;
////    private Collider2D bossCollider;
////    private Collider2D playerCollider;
////    void Start()
////    {
////        //player = GameObject.FindFirstObjectByType<PlayerSSBoss2>()?.transform;
////        //rb = GetComponent<Rigidbody2D>();
////        //anim = GetComponent<Animator>();
////        //meleeHitbox?.SetActive(false);
////        //spawnTimer = spawnDelay;

////        player = GameObject.FindFirstObjectByType<PlayerSSBoss2>()?.transform;
////        rb = GetComponent<Rigidbody2D>();
////        anim = GetComponent<Animator>();
////        bossCollider = GetComponent<Collider2D>();

////        if (player != null)
////            playerCollider = player.GetComponent<Collider2D>();

////        meleeHitbox?.SetActive(false);
////        spawnTimer = spawnDelay;
////    }

////    void Update()
////    {
////        if (player == null) return;

////        spawnTimer -= Time.deltaTime;
////        if (spawnTimer > 0) return;

////        if (isAttacking)
////        {
////            rb.linearVelocity = Vector2.zero;
////            return;
////        }

////        float distance = Vector2.Distance(transform.position, player.position);

////        // Face the player
////        if ((player.position.x > transform.position.x && !facingRight) ||
////            (player.position.x < transform.position.x && facingRight))
////        {
////            Flip();
////        }

////        // Stop if too far
////        if (distance > chaseRange)
////        {
////            if (!isAttacking)
////                anim.SetBool("isMoving", false);

////            rb.linearVelocity = Vector2.zero;
////            return;
////        }

////        // Try to attack or move
////        if (distance > attackRange)
////        {
////            MoveTowardPlayer();
////        }
////        else
////        {
////            TryAttack();
////        }
////    }

////    private void MoveTowardPlayer()
////    {
////        isMoving = true;
////        anim.SetBool("isMoving", true);

////        Vector2 direction = (player.position - transform.position).normalized;

////        // Handle ground + jump detection
////        CheckGrounded();
////        DetectAndJump(direction);

////        // Move horizontally (ignore y when jumping)
////        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
////    }

////    private void CheckGrounded()
////    {
////        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
////        isGrounded = hit.collider != null;
////    }



////    private void DetectAndJump(Vector2 direction)
////    {
////        if (!isGrounded || isJumping) return;

////        float verticalDiff = player.position.y - transform.position.y;
////        float horizontalDiff = Mathf.Abs(player.position.x - transform.position.x);

////        // Player must be somewhat above
////        if (verticalDiff < playerAboveThreshold) return;

////        // Check for headroom directly above Dracula
////        RaycastHit2D ceilingCheck = Physics2D.Raycast(transform.position, Vector2.up, verticalDiff, groundLayer);

////        if (ceilingCheck.collider != null)
////        {
////            // There's something blocking above — start repositioning
////            Debug.Log("[Boss Jump] Path blocked above by: " + ceilingCheck.collider.name);
////            StartCoroutine(RepositionForJump());
////            return;
////        }

////        // Make sure player has ground/platform under them
////        Vector2 playerFeet = player.position + Vector3.down * 0.5f;
////        RaycastHit2D platformCheck = Physics2D.Raycast(playerFeet, Vector2.down, 3f, groundLayer);

////        if (platformCheck.collider == null)
////        {
////            Debug.Log("[Boss Jump] No valid platform under player — skip jump");
////            return;
////        }

////        // All conditions met, jump
////        Debug.Log("[Boss Jump] Jumping toward player!");
////        StartCoroutine(JumpRoutine());
////    }

////    private IEnumerator JumpRoutine()
////    {
////        isJumping = true;
////        anim.SetTrigger("jump");

////        yield return new WaitForSeconds(0.05f);

////        // --- temporarily ignore collision with the player ---
////        if (bossCollider != null && playerCollider != null)
////        {
////            Physics2D.IgnoreCollision(bossCollider, playerCollider, true);
////            Debug.Log("[Boss Jump] Collision with player disabled");
////        }

////        // Apply jump velocity (diagonal if you wish)
////        float horizontalDirection = player.position.x > transform.position.x ? 1f : -1f;
////        float horizontalJumpForce = 4f;
////        rb.linearVelocity = new Vector2(horizontalDirection * horizontalJumpForce, jumpForce);

////        // Wait until grounded again
////        yield return new WaitUntil(() =>
////            Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer));

////        // Re-enable collision
////        if (bossCollider != null && playerCollider != null)
////        {
////            Physics2D.IgnoreCollision(bossCollider, playerCollider, false);
////            Debug.Log("[Boss Jump] Collision with player re-enabled");
////        }

////        isJumping = false;
////        anim.SetBool("isMoving", true);
////        Debug.Log("[Boss Jump] Landed and resumed running");
////    }

////    private IEnumerator RepositionForJump()
////    {
////        isMoving = true;
////        anim.SetBool("isMoving", true);

////        // Choose a random side or the side opposite to the player’s x position
////        float moveDir = player.position.x > transform.position.x ? -1f : 1f;

////        float moveDuration = 1.0f;   // move for 1 second
////        float timer = 0f;

////        Debug.Log("[Boss AI] Repositioning sideways to find jump gap");

////        while (timer < moveDuration)
////        {
////            rb.linearVelocity = new Vector2(moveDir * moveSpeed, rb.linearVelocity.y);

////            // Check periodically if there's now room to jump
////            RaycastHit2D ceilingCheck = Physics2D.Raycast(transform.position, Vector2.up, 2f, groundLayer);
////            if (ceilingCheck.collider == null)
////            {
////                Debug.Log("[Boss AI] Found open space — jumping now!");
////                StartCoroutine(JumpRoutine());
////                yield break;
////            }

////            timer += Time.deltaTime;
////            yield return null;
////        }

////        // stop moving if no gap found
////        rb.linearVelocity = Vector2.zero;
////        anim.SetBool("isMoving", false);
////        Debug.Log("[Boss AI] Could not find open jump space, stopping reposition");
////    }

////    private void TryAttack()
////    {
////        if (isAttacking) return;
////        if (Time.time < lastMeleeTime + meleeCooldown) return;

////        Debug.Log("Boss trying to attack");
////        StartCoroutine(DoMeleeAttack());
////        lastMeleeTime = Time.time;
////    }

////    private IEnumerator DoMeleeAttack()
////    {
////        isAttacking = true;
////        rb.linearVelocity = Vector2.zero;

////        anim.ResetTrigger("attack");
////        anim.SetTrigger("attack");
////        Debug.Log("Boss attack triggered");

////        yield return new WaitForSeconds(0.05f);

////        float attackDuration = 1.0f;
////        float cancelDelay = 0.3f;
////        float elapsed = 0f;

////        while (elapsed < attackDuration)
////        {
////            rb.linearVelocity = Vector2.zero;

////            float dx = Mathf.Abs(player.position.x - transform.position.x);
////            if (elapsed > cancelDelay && dx > attackRange + 1.0f)
////            {
////                Debug.Log("Player escaped attack range — cancelling attack early");
////                anim.ResetTrigger("attack");
////                anim.SetBool("isMoving", true);
////                isAttacking = false;
////                anim.CrossFade("Running", 0.05f);
////                yield break;
////            }

////            elapsed += Time.deltaTime;
////            yield return null;
////        }

////        anim.ResetTrigger("attack");
////        anim.SetBool("isMoving", true);
////        isAttacking = false;
////    }

////    public void EnableHitbox()
////    {
////        if (meleeHitbox == null) return;
////        var bossHitbox = meleeHitbox.GetComponent<BossAttackHitbox>();
////        if (bossHitbox != null) bossHitbox.ActivateHitbox();
////    }

////    public void DisableHitbox()
////    {
////        if (meleeHitbox == null) return;
////        var bossHitbox = meleeHitbox.GetComponent<BossAttackHitbox>();
////        if (bossHitbox != null) bossHitbox.DeactivateHitbox();
////    }

////    private void Flip()
////    {
////        facingRight = !facingRight;
////        Vector3 s = transform.localScale;
////        s.x *= -1;
////        transform.localScale = s;
////    }
////}


//using UnityEngine;
//using System.Collections;

//public class BossControllerHybrid : MonoBehaviour
//{
//    [Header("Movement Settings")]
//    public float moveSpeed = 2f;
//    public float chaseRange = 8f;
//    public float attackRange = 2f;

//    [Header("Attack Settings")]
//    public float meleeCooldown = 2f;
//    public GameObject meleeHitbox;

//    [Header("Jump Settings")]
//    public float jumpHeight = 3f;                 // desired jump height in world units
//    public float horizontalJumpForce = 4f;        // side push during jump
//    public float playerAboveThreshold = 1.5f;     // player must be this much higher
//    public float horizontalJumpRange = 4f;        // jump only if horizontally close
//    public float groundCheckDistance = 0.2f;      // how far down to check for ground
//    public LayerMask groundLayer;                

//    private bool isGrounded;
//    private bool isJumping;
//    private bool isAttacking;
//    private bool facingRight = true;
//    private bool isMoving;

//    private Transform player;
//    private Rigidbody2D rb;
//    private Animator anim;
//    private Collider2D bossCollider;
//    private Collider2D playerCollider;

//    private float lastMeleeTime;
//    private float spawnDelay = 1.0f;
//    private float spawnTimer;

//    void Start()
//    {
//        player = GameObject.FindFirstObjectByType<PlayerSSBoss2>()?.transform;
//        rb = GetComponent<Rigidbody2D>();
//        anim = GetComponent<Animator>();
//        bossCollider = GetComponent<Collider2D>();

//        if (player != null)
//            playerCollider = player.GetComponent<Collider2D>();

//        meleeHitbox?.SetActive(false);
//        spawnTimer = spawnDelay;
//    }

//    void Update()
//    {
//        if (player == null) return;

//        spawnTimer -= Time.deltaTime;
//        if (spawnTimer > 0) return;

//        CheckGrounded();

//        if (isAttacking)
//        {
//            rb.linearVelocity = Vector2.zero;
//            return;
//        }

//        float distance = Vector2.Distance(transform.position, player.position);

//        // Face the player
//        if ((player.position.x > transform.position.x && !facingRight) ||
//            (player.position.x < transform.position.x && facingRight))
//            Flip();

//        // Too far, idle
//        if (distance > chaseRange)
//        {
//            anim.SetBool("isMoving", false);
//            rb.linearVelocity = Vector2.zero;
//            return;
//        }

//        // Within chase range
//        if (distance > attackRange)
//            HandleMovement();
//        else
//            TryAttack();
//    }

//    // -----------------------------------------------------------------------
//    // MOVEMENT AND JUMPING
//    // -----------------------------------------------------------------------
//    private void HandleMovement()
//    {
//        isMoving = true;
//        anim.SetBool("isMoving", true);

//        Vector2 dir = (player.position - transform.position).normalized;
//        rb.linearVelocity = new Vector2(dir.x * moveSpeed, rb.linearVelocity.y);

//        // Jump if the player is above and reachable
//        TryJumpToPlayer();
//    }

//    private void CheckGrounded()
//    {
//        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
//        anim.SetBool("isGrounded", isGrounded);
//    }

//    private void TryJumpToPlayer()
//    {
//        if (!isGrounded || isJumping) return;

//        float verticalDiff = player.position.y - transform.position.y;
//        float horizontalDiff = Mathf.Abs(player.position.x - transform.position.x);

//        // Player not high enough or too far horizontally
//        if (verticalDiff < playerAboveThreshold || horizontalDiff > horizontalJumpRange)
//            return;

//        // Check if there is ground under the player (a platform)
//        Vector2 playerFeet = player.position + Vector3.down * 0.6f;
//        bool platformUnderPlayer = Physics2D.Raycast(playerFeet, Vector2.down, 2f, groundLayer);
//        if (!platformUnderPlayer)
//            return;

//        // Check if there's headroom for jump
//        bool blockedAbove = Physics2D.Raycast(transform.position, Vector2.up, 1.0f, groundLayer);
//        if (blockedAbove)
//        {
//            // Try to reposition sideways before jumping
//            StartCoroutine(RepositionThenJump());
//            return;
//        }

//        // Jump directly if path is clear
//        StartCoroutine(JumpRoutine());
//    }

//    private IEnumerator JumpRoutine()
//    {
//        isJumping = true;
//        anim.SetTrigger("jump");

//        // short delay for animation start
//        yield return new WaitForSeconds(0.05f);

//        // Compute physics-based vertical velocity
//        float gravity = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);
//        float verticalVelocity = Mathf.Sqrt(2 * gravity * jumpHeight);

//        // Temporarily ignore collision with player
//        if (bossCollider != null && playerCollider != null)
//            Physics2D.IgnoreCollision(bossCollider, playerCollider, true);

//        // Apply diagonal jump
//        float horizontalDir = player.position.x > transform.position.x ? 1f : -1f;
//        rb.linearVelocity = new Vector2(horizontalDir * horizontalJumpForce, verticalVelocity);

//        // Wait until grounded again
//        yield return new WaitUntil(() => Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer));

//        // Restore collision
//        if (bossCollider != null && playerCollider != null)
//            Physics2D.IgnoreCollision(bossCollider, playerCollider, false);

//        isJumping = false;
//        anim.SetBool("isMoving", true);
//    }

//    // If blocked above, move sideways until open space, then jump
//    private IEnumerator RepositionThenJump()
//    {
//        Debug.Log("[Boss AI] Repositioning for jump...");
//        float side = player.position.x > transform.position.x ? -1f : 1f;
//        float timer = 0f;
//        float duration = 1f;

//        while (timer < duration)
//        {
//            rb.linearVelocity = new Vector2(side * moveSpeed, rb.linearVelocity.y);

//            // check if headroom opened
//            bool blocked = Physics2D.Raycast(transform.position, Vector2.up, 1.0f, groundLayer);
//            if (!blocked)
//            {
//                StartCoroutine(JumpRoutine());
//                yield break;
//            }

//            timer += Time.deltaTime;
//            yield return null;
//        }

//        rb.linearVelocity = Vector2.zero;
//        anim.SetBool("isMoving", false);
//        Debug.Log("[Boss AI] Could not find open jump space.");
//    }

//    // -----------------------------------------------------------------------
//    // ATTACKING
//    // -----------------------------------------------------------------------
//    private void TryAttack()
//    {
//        if (isAttacking) return;
//        if (Time.time < lastMeleeTime + meleeCooldown) return;

//        StartCoroutine(DoMeleeAttack());
//        lastMeleeTime = Time.time;
//    }

//    private IEnumerator DoMeleeAttack()
//    {
//        isAttacking = true;
//        rb.linearVelocity = Vector2.zero;

//        anim.ResetTrigger("attack");
//        anim.SetTrigger("attack");

//        yield return new WaitForSeconds(0.05f);

//        float attackDuration = 1.0f;
//        float elapsed = 0f;

//        while (elapsed < attackDuration)
//        {
//            rb.linearVelocity = Vector2.zero;
//            float dx = Mathf.Abs(player.position.x - transform.position.x);

//            // Cancel attack if player moved out of range
//            if (dx > attackRange + 1.0f)
//            {
//                anim.SetBool("isMoving", true);
//                isAttacking = false;
//                anim.CrossFade("Running", 0.05f);
//                yield break;
//            }

//            elapsed += Time.deltaTime;
//            yield return null;
//        }

//        anim.SetBool("isMoving", true);
//        isAttacking = false;
//    }

//    // -----------------------------------------------------------------------
//    // HITBOX
//    // -----------------------------------------------------------------------
//    public void EnableHitbox()
//    {
//        if (meleeHitbox == null) return;
//        meleeHitbox.GetComponent<BossAttackHitbox>()?.ActivateHitbox();
//    }

//    public void DisableHitbox()
//    {
//        if (meleeHitbox == null) return;
//        meleeHitbox.GetComponent<BossAttackHitbox>()?.DeactivateHitbox();
//    }
//    private void OnCollisionEnter2D(Collision2D collision)
//    {
//        var player = collision.collider.GetComponent<PlayerSSBoss2>();
//        if (player == null) return;

//        // Check if player is above boss (landing on head)
//        foreach (ContactPoint2D contact in collision.contacts)
//        {
//            if (contact.point.y > transform.position.y + 1.0f)
//            {
//                Debug.Log("[Boss] Player landed on head – deal damage!");
//                // player.TakeDamage(1);
//                // Optional bounce-back
//                Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
//                if (rb != null)
//                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, 8f);
//                break;
//            }
//        }
//    }
//    // -----------------------------------------------------------------------
//    // UTILS
//    // -----------------------------------------------------------------------
//    private void Flip()
//    {
//        facingRight = !facingRight;
//        Vector3 s = transform.localScale;
//        s.x *= -1;
//        transform.localScale = s;
//    }

//    // -----------------------------------------------------------------------
//    // DEBUG GIZMOS
//    // -----------------------------------------------------------------------
//    private void OnDrawGizmos()
//    {
//        if (!Application.isPlaying) return;

//        // Colors




//        Color groundColor = Color.green;
//        Color headColor = Color.red;
//        Color playerColor = Color.cyan;
//        Color jumpColor = Color.yellow;

//        // === Ground Check Ray ===
//        Gizmos.color = groundColor;
//        Gizmos.DrawLine(transform.position,
//                        transform.position + Vector3.down * groundCheckDistance);

//        // === Headroom Check Ray ===
//        Gizmos.color = headColor;
//        Gizmos.DrawLine(transform.position,
//                        transform.position + Vector3.up * 1.0f);

//        // === Player Feet Ray (for platform detection) ===
//        if (player != null)
//        {
//            Vector2 playerFeet = player.position + Vector3.down * 0.6f;
//            Gizmos.color = playerColor;
//            Gizmos.DrawLine(playerFeet,
//                            playerFeet + Vector2.down * 2f);

//            // Draw height and horizontal relation lines
//            Gizmos.color = jumpColor;
//            Gizmos.DrawLine(transform.position, player.position);
//        }

//        // === Optional: Show jump trajectory ===
//        if (isJumping)
//        {
//            Vector3 start = transform.position;
//            Vector3 velocity = rb.linearVelocity;
//            Vector3 gravity = Physics2D.gravity * rb.gravityScale;

//            Gizmos.color = new Color(1f, 0.5f, 0f, 0.7f); // orange arc

//            // Simulate a few frames of jump path
//            Vector3 pos = start;
//            for (int i = 0; i < 20; i++)
//            {
//                Vector3 next = pos + velocity * 0.05f + 0.5f * gravity * 0.05f * 0.05f;
//                Gizmos.DrawLine(pos, next);
//                pos = next;
//                velocity += gravity * 0.05f;
//            }
//        }
//    }

//}


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


    private bool isGrounded;
    private bool isJumping;
    private bool isAttacking;
    private bool facingRight = true;

    private Transform player;
    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D bossCollider;
    private Collider2D playerCollider;

    private float lastMeleeTime;
    private float spawnDelay = 1.0f;
    private float spawnTimer;



    private void Start()
    {
        player = GameObject.FindFirstObjectByType<PlayerSSBoss2>()?.transform;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        bossCollider = GetComponent<Collider2D>();

        if (player != null)
            playerCollider = player.GetComponent<Collider2D>();

        meleeHitbox?.SetActive(false);
        spawnTimer = spawnDelay;
    }


    private void Update()
    {
        if (player == null) return;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer > 0) return;

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


    // Attempts to jump toward the player if they are above and reachable.

    private void TryJumpToPlayer()
    {
        if (!isGrounded || isJumping) return;

        float verticalDiff = player.position.y - transform.position.y;
        float horizontalDiff = Mathf.Abs(player.position.x - transform.position.x);

        // Player not high enough or too far horizontally
        if (verticalDiff < playerAboveThreshold || horizontalDiff > horizontalJumpRange)
            return;

        // Ensure the player is standing on a valid platform
        Vector2 playerFeet = player.position + Vector3.down * 0.6f;
        bool platformUnderPlayer = Physics2D.Raycast(playerFeet, Vector2.down, 2f, groundLayer);
        if (!platformUnderPlayer)
            return;

        // Check headroom
        bool blockedAbove = Physics2D.Raycast(transform.position, Vector2.up, 1.0f, groundLayer);
        if (blockedAbove)
        {
            StartCoroutine(RepositionThenJump());
            return;
        }

        StartCoroutine(JumpRoutine());
    }

    // Executes the jump toward the player's platform.

    private IEnumerator JumpRoutine()
    {
        isJumping = true;
        // add when i figure out how to fix animation
        //anim.SetTrigger("jump");

        yield return new WaitForSeconds(0.05f);

        // Compute velocity from desired jump height
        float gravity = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);
        float verticalVelocity = Mathf.Sqrt(2 * gravity * jumpHeight);

        // Temporarily disable collision with player to avoid bumping
        if (bossCollider && playerCollider)
            Physics2D.IgnoreCollision(bossCollider, playerCollider, true);

        // Launch diagonally toward the player
        float dirX = player.position.x > transform.position.x ? 1f : -1f;
        rb.linearVelocity = new Vector2(dirX * horizontalJumpForce, verticalVelocity);

        // Wait until boss lands
        yield return new WaitUntil(() => Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer));

        // Restore collision
        if (bossCollider && playerCollider)
            Physics2D.IgnoreCollision(bossCollider, playerCollider, false);

        isJumping = false;
        anim.SetBool("isMoving", true);
    }

    // If there's no space above, moves sideways until clear, then jumps.

    private IEnumerator RepositionThenJump()
    {
        Debug.Log("[Boss AI] Repositioning to find jump space...");
        float sideDir = player.position.x > transform.position.x ? -1f : 1f;
        float timer = 0f;
        const float duration = 1f;

        while (timer < duration)
        {
            rb.linearVelocity = new Vector2(sideDir * moveSpeed, rb.linearVelocity.y);

            bool blocked = Physics2D.Raycast(transform.position, Vector2.up, 1.0f, groundLayer);
            if (!blocked)
            {
                StartCoroutine(JumpRoutine());
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        anim.SetBool("isMoving", false);
        Debug.Log("[Boss AI] Could not find clear jump path.");
    }



    // Attempts a melee attack if cooldown is over.

    private void TryAttack()
    {
        if (isAttacking || Time.time < lastMeleeTime + meleeCooldown) return;

        StartCoroutine(DoMeleeAttack());
        lastMeleeTime = Time.time;
    }

    // Handles the melee attack coroutine.

    private IEnumerator DoMeleeAttack()
    {
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;

        anim.ResetTrigger("attack");
        anim.SetTrigger("attack");

        yield return new WaitForSeconds(0.05f);

        float attackDuration = 1.0f;
        float elapsed = 0f;

        while (elapsed < attackDuration)
        {
            rb.linearVelocity = Vector2.zero;
            float dx = Mathf.Abs(player.position.x - transform.position.x);

            // Cancel attack if player moves out of range
            if (dx > attackRange + 1.0f)
            {
                anim.SetBool("isMoving", true);
                isAttacking = false;
                anim.CrossFade("Running", 0.05f);
                yield break;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        anim.SetBool("isMoving", true);
        isAttacking = false;
    }


    public void EnableHitbox()
    {
        meleeHitbox?.GetComponent<BossAttackHitbox>()?.ActivateHitbox();
    }

    public void DisableHitbox()
    {
        meleeHitbox?.GetComponent<BossAttackHitbox>()?.DeactivateHitbox();
    }




    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 s = transform.localScale;
        s.x *= -1;
        transform.localScale = s;
    }
}

