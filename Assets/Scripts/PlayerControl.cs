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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Get raw input (WASD or arrows)
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        input = new Vector2(horizontal, vertical);
        movement = input.normalized * movSpeed;

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
        // Use current input if moving and remembers last direction when stopped
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
        
        animator.SetBool("IsMoving", input != Vector2.zero);

        // Calculate movement vector
        movement = input.normalized * movSpeed;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = movement;
    }
}
