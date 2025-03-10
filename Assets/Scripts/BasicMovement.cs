using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    public float movSpeed;
    float speedX, speedY;
    
    Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // ADD RIGIDBODY2D BEFORE APPLYING SCRIPT
        speedX = Input.GetAxisRaw("Horizontal") * movSpeed;
        speedY = Input.GetAxisRaw("Vertical") * movSpeed;
    
        rb.linearVelocity = new Vector2(speedX, speedY);
    }
}
