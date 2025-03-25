using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private Animator animator;
    public float movSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;
    public Vector2 LastMoveDirection { get; private set; } = Vector2.right;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // getting movement input (wasd)
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        movement = new Vector2(horizontal, vertical).normalized * movSpeed;



        // updating animator params
        animator.SetFloat("Horizontal", horizontal);
        animator.SetFloat("Vertical", vertical);
        animator.SetBool("IsMoving", movement != Vector2.zero);

        if (movement != Vector2.zero)
        {
            LastMoveDirection = movement.normalized;
        }

    }

    void FixedUpdate()
    {
        
        rb.linearVelocity = movement;
    }
}
