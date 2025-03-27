using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private Animator animator;
    public float movSpeed = 5f;
    private Rigidbody2D rb;

    private Vector2 input;
    private Vector2 movement;

    // Public direction tracker — used by attack system
    public Vector2 LastMoveDirection { get; private set; } = Vector2.right;

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

        // Update facing direction for attack logic
        if (input != Vector2.zero)
        {
            LastMoveDirection = input.normalized;
        }

        // Update animator parameters
        animator.SetFloat("Horizontal", input.x);
        animator.SetFloat("Vertical", input.y);
        animator.SetBool("IsMoving", input != Vector2.zero);
    }

    void FixedUpdate()
    {
        rb.linearVelocity = movement;
    }
}

