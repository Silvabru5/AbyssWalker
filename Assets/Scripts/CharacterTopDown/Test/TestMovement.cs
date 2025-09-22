using UnityEngine;

public class TestMovement : MonoBehaviour
{
    public float movSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 input;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0; // no falling in top-down
    }

    void Update()
    {
        // Get WASD/Arrow key input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        input = new Vector2(horizontal, vertical).normalized;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = input * movSpeed;
    }
}