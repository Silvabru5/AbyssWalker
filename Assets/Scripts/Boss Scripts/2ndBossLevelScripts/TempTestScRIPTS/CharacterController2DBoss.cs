using UnityEngine;
using UnityEngine.Events;

public class CharacterController2DBoss : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 400f;
    [Range(0, 1)][SerializeField] private float m_CrouchSpeed = .36f;
    [Range(0, .3f)][SerializeField] private float m_MovementSmoothing = .05f;
    [SerializeField] private bool m_AirControl = false;
    [SerializeField] private LayerMask m_WhatIsGround;
    [SerializeField] private Transform m_GroundCheck;
    [SerializeField] private Transform m_CeilingCheck;
    [SerializeField] private Collider2D m_CrouchDisableCollider;

    private const float k_GroundedRadius = .2f;
    public bool m_Grounded;
    private const float k_CeilingRadius = .2f;

    [Header("Slope Settings")]
    [SerializeField] private float slopeRayLength = 0.5f;     // how far to probe below feet
    [SerializeField] private float maxSlopeAngle = 45f;        // ignore walls
    [SerializeField] private float uphillAssist = 25f;         // small push to overcome friction
    [SerializeField] private float moveSpeedScalar = 10f;      // same factor used before

    public Rigidbody2D rb;
    private bool m_FacingRight = true;
    private Vector2 m_Velocity = Vector2.zero;


    [Header("Events")]
    [Space]
    public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }
    public BoolEvent OnCrouchEvent;
    private bool m_wasCrouching = false;

    private void Awake()
    {
        // Step 1: cache rigidbody and init events
        rb = GetComponent<Rigidbody2D>();
        if (OnLandEvent == null) OnLandEvent = new UnityEvent();
        if (OnCrouchEvent == null) OnCrouchEvent = new BoolEvent();
    }

    private void FixedUpdate()
    {
        // Step 1: refresh grounded flag via overlap
        bool wasGrounded = m_Grounded;
        m_Grounded = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;
                if (!wasGrounded) OnLandEvent.Invoke();
            }
        }
    }

    public void Move(float move, bool crouch, bool jump)
    {
        // Step 1: ceiling check for crouch
        if (!crouch)
        {
            if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
                crouch = true;
        }

        // Step 2: crouch logic
        if (m_Grounded || m_AirControl)
        {
            if (crouch)
            {
                if (!m_wasCrouching)
                {
                    m_wasCrouching = true;
                    OnCrouchEvent.Invoke(true);
                }
                move *= m_CrouchSpeed;
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = false;
            }
            else
            {
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = true;

                if (m_wasCrouching)
                {
                    m_wasCrouching = false;
                    OnCrouchEvent.Invoke(false);
                }
            }

            // Step 3: detect slope under feet
            Vector2 groundNormal;
            float slopeAngle;
            bool onSlope = TryGetSlope(out groundNormal, out slopeAngle);

            // Step 4: calculate target velocity
            Vector2 current = rb.linearVelocity;
            Vector2 target;

            if (m_Grounded && onSlope && slopeAngle > 0.1f && slopeAngle < maxSlopeAngle)
            {
                // Step 4a: move along the slope tangent
                Vector2 tangent = new Vector2(groundNormal.y, -groundNormal.x).normalized;
                float dir = Mathf.Sign(move);
                float mag = Mathf.Abs(move) * moveSpeedScalar;

                Vector2 along = tangent * dir * mag;

                // Small uphill assist to overcome friction (fixed CS1503 line)
                if (Mathf.Abs(move) > 0.01f)
                {
                    Vector2 push = new Vector2(along.x, Mathf.Max(0f, along.y)) * (uphillAssist / 100f);
                    rb.AddForce(push); // Removed ForceMode2D.Force
                }

                target = new Vector2(along.x, onSlope ? along.y : current.y);
            }
            else
            {
                // Step 4b: flat ground or in-air motion
                target = new Vector2(move * moveSpeedScalar, current.y);
            }

            // Step 5: smooth velocity
            Vector2 smoothed = Vector2.SmoothDamp(current, target, ref m_Velocity, m_MovementSmoothing);
            rb.linearVelocity = smoothed;

            // Step 6: stop sliding when idle
            if (m_Grounded && Mathf.Abs(move) < 0.01f)
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

            // Step 7: flip character if needed
            if (move > 0 && !m_FacingRight)
                Flip();
            else if (move < 0 && m_FacingRight)
                Flip();
        }

        // Step 8: jump logic
        if (m_Grounded && jump)
        {
            m_Grounded = false;
            rb.AddForce(new Vector2(0f, m_JumpForce), ForceMode2D.Impulse);
        }
    }


    private bool TryGetSlope(out Vector2 groundNormal, out float slopeAngle)
    {
        // Step 1: raycast down from ground check to read surface normal
        RaycastHit2D hit = Physics2D.Raycast(m_GroundCheck.position, Vector2.down, slopeRayLength, m_WhatIsGround);
        if (hit.collider != null)
        {
            groundNormal = hit.normal;
            slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            return true;
        }
        groundNormal = Vector2.up;
        slopeAngle = 0f;
        return false;
    }

    private void Flip()
    {
        // Step 1: flip sprite horizontally
        m_FacingRight = !m_FacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1f;
        transform.localScale = theScale;
    }
}
