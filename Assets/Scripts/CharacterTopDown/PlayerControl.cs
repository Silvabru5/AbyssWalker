using System.Collections;
using UnityEngine;


public class PlayerControl : MonoBehaviour
{
    private Animator animator;
    public float movSpeed = 5f;
    private Rigidbody2D rb;

    private Vector2 input;
    private Vector2 movement;

    //direction tracker used by attack system
    public Vector2 LastMoveDirection { get; private set; } = Vector2.down; //THIS SETS WHICH DIRECTION CHARACTER LOOKS AT AFTER STARTING GAME

    // variables to add leeway for diagonal input (else player needs to let go of keys at the exact same time for diag idles)
    private float lastDiagonalTime = 0f;
    public float diagonalLeeway = 0.2f;

    //variables for dashing
    public float dashingPower = 20f;
    public float dashingTime = 0.2f;
    public float dashingCooldown = 1f;
    public bool canDash = true;
    public bool isDashing = false;
    public TrailRenderer _trailRenderer;

    // used to disable movement after death
    private PlayerHealth playerHealth;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        _trailRenderer = transform.Find("Trail").GetComponent<TrailRenderer>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        // if dead, allow no movements
        if (playerHealth.currentHealth <= 0) return;

        //if not dashing do normal movement
        if (!isDashing)
        {
            // Get raw input (WASD or arrows)
            // Get WASD/Arrow key input
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            input = new Vector2(horizontal, vertical).normalized;
            movement = input * movSpeed;

            //if multiple directions are active (diagonal) update the last moved direction and get the time since they were in that state
            if (Mathf.Abs(horizontal) > 0.1f && Mathf.Abs(vertical) > 0.1f)
            {
                LastMoveDirection = input.normalized;
                lastDiagonalTime = Time.time;
            }
            else if (input != Vector2.zero)
            {
                // If only one direction is active, check to see if player is within the leeway so directions dont get jumbled up
                if (Time.time - lastDiagonalTime < diagonalLeeway)
                {
                    // let it be diagonal
                }
                else
                {
                    // updates it normally.
                    LastMoveDirection = input.normalized;
                }
            }

            // Update animator parameters:
            /*   // Use current input if moving and remembers last direction when stopped
               if (input != Vector2.zero)
               {
                   animator.SetFloat("Horizontal", input.x);
                   animator.SetFloat("Vertical", input.y);
               }
               else
               {
                   animator.SetFloat("Horizontal", LastMoveDirection.x);
                   animator.SetFloat("Vertical", LastMoveDirection.y);
               }

                   */
            animator.SetBool("IsMoving", input != Vector2.zero);
        }

        //when dashing idnore the movement inpiut
        movement = Vector2.zero;

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }

        // Calculate movement vector
        // movement = input.normalized * movSpeed;
    }

    void FixedUpdate()
    {
        if (!isDashing)
        {
            rb.linearVelocity = input * movSpeed;
        }
    }


    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;

        //use last movement direction as dash dir
        Vector2 dashDirection = LastMoveDirection;
        if (dashDirection == Vector2.zero)
        {
            dashDirection = Vector2.down;

        }
        rb.linearVelocity = dashDirection.normalized * dashingPower;

        if (_trailRenderer != null)
        {
            _trailRenderer.emitting = true;
        }

        yield return new WaitForSeconds(dashingTime);
        if (_trailRenderer != null)
        {
            _trailRenderer.emitting = false;
        }
        rb.gravityScale = originalGravity;
        isDashing = false;
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}
