//using UnityEngine;

//public class BasicMovement : MonoBehaviour
//{
//    public float movSpeed;
//    float speedX, speedY;

//    Rigidbody2D rb;
//    // Start is called once before the first execution of Update after the MonoBehaviour is created
//    void Start()
//    {
//        rb = GetComponent<Rigidbody2D>();
//        rb.gravityScale = 0;
//        rb.freezeRotation = true; // Prevents unwanted rotation
//        rb.bodyType = RigidbodyType2D.Kinematic; // Ensures player doesn�t push objects
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        // ADD RIGIDBODY2D BEFORE APPLYING SCRIPT
//        speedX = Input.GetAxisRaw("Horizontal") * movSpeed;
//        speedY = Input.GetAxisRaw("Vertical") * movSpeed;

//        rb.linearVelocity = new Vector2(speedX, speedY);
//    }
//    void FixedUpdate()
//    {
//        // Move using transform.position (prevents physics-based pushing)
//        transform.position += new Vector3(speedX, speedY, 0).normalized * movSpeed * Time.fixedDeltaTime;
//    }
//}\
using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    public float movSpeed;
    float speedX, speedY;
    private Rigidbody2D rb;
    private bool canMove = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true; // Prevents unwanted rotation
    }

    void Update()
    {
        // Get movement input
        speedX = Input.GetAxisRaw("Horizontal");
        speedY = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        if (canMove)
        {
            rb.linearVelocity = new Vector2(speedX, speedY).normalized * movSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero; // Stop movement on collision
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            canMove = false; // Stop player movement when touching enemy
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            canMove = true; // Allow movement again if no longer touching enemy
        }
    }
}
