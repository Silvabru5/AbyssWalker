using UnityEngine;

public class BossMovement : MonoBehaviour
{
    public float moveSpeed = 0.1f; // Movement speed
    private Animator animator;
    private Rigidbody2D rb;
    private float moveInput;
    private float lastMoveInput; // remembers previous direction
    private bool wasMoving; // tracks previous moving state

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0; // Prevents falling
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    void Update()
    {
        // Get input (-1 = left, 1 = right, 0 = idle)
        moveInput = Input.GetAxisRaw("Horizontal");
        bool isMoving = moveInput != 0;

        // Normal moving/idle toggle
        animator.SetBool("isMoving", isMoving);

        // Detect direction change while moving
        if (isMoving && Mathf.Sign(moveInput) != Mathf.Sign(lastMoveInput) && lastMoveInput != 0)
        {
            animator.SetTrigger("restartRun"); // restart StartRun animation
        }

        // Play StopRun only when going from moving to idle
        if (wasMoving && !isMoving)
        {
            animator.SetTrigger("stopRun");
        }

        // Flip sprite
        if (moveInput > 0)
            transform.localScale = new Vector3(1, 1, 1); // facing right
        else if (moveInput < 0)
            transform.localScale = new Vector3(-1, 1, 1); // facing left

        // Store current direction and moving state for next frame
        lastMoveInput = moveInput;
        wasMoving = isMoving;
    }

    void FixedUpdate()
    {
        // Apply movement
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }
}