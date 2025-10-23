using UnityEngine;

public class BatMovement : MonoBehaviour
{
    [SerializeField] private Transform _target1;
    [SerializeField] private Transform _target2;
    [SerializeField] private float moveFreq;
    [SerializeField] private float moveAmplitude;
    [SerializeField] private float moveSpeed;
    private Vector2 movePos;
    private Vector2 endPos;
    private SpriteRenderer spriteRenderer;
    private float lastTValue;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movePos = _target1.position;
        endPos = _target2.position; 
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    void Movement()
    {
        // Horizontal movement using PingPong
        float t = Mathf.PingPong(Time.time * moveSpeed, 1f);
        Vector2 basePos = Vector2.Lerp(movePos, endPos, t);

        // Vertical sine-wave motion
        float sineOffset = Mathf.Sin(Time.time * moveFreq) * moveAmplitude;

        // Combine both
        transform.position = new Vector2(basePos.x, basePos.y + sineOffset);

        // Determine direction (for flipping)
        bool movingRight = t > lastTValue; 
        spriteRenderer.flipX = !movingRight;

        lastTValue = t;
    }
}
