using UnityEngine;
using UnityEngine.Events;

// handles 2d boss character movement with slopes, crouching, and jumping
public class CharacterController2DBoss : MonoBehaviour
{
    [Header("movement settings")]
    [SerializeField] private float m_JumpForce = 400f;                   // upward jump force
    [Range(0, 1)][SerializeField] private float m_CrouchSpeed = .36f;    // speed multiplier while crouching
    [Range(0, .3f)][SerializeField] private float m_MovementSmoothing = .05f; // smooth transition for velocity
    [SerializeField] private bool m_AirControl = false;                  // allows mid-air movement
    [SerializeField] private LayerMask m_WhatIsGround;                   // identifies ground layers
    [SerializeField] private Transform m_GroundCheck;                    // position used to detect ground
    [SerializeField] private Transform m_CeilingCheck;                   // position used to detect low ceiling
    [SerializeField] private Collider2D m_CrouchDisableCollider;         // collider to disable when crouching

    private const float k_GroundedRadius = .2f;                          // radius for ground check circle
    public bool m_Grounded;                                              // is boss currently on ground
    private const float k_CeilingRadius = .2f;                           // radius for ceiling check

    [Header("slope settings")]
    [SerializeField] private float slopeRayLength = 0.5f;                // ray length to detect slope angle
    [SerializeField] private float maxSlopeAngle = 45f;                  // maximum slope considered valid
    [SerializeField] private float uphillAssist = 25f;                   // boost to climb slopes smoothly
    [SerializeField] private float moveSpeedScalar = 10f;                // horizontal movement speed multiplier

    public Rigidbody2D rb;                                               // rigidbody reference
    private bool m_FacingRight = true;                                   // facing direction flag
    private Vector2 m_Velocity = Vector2.zero;                           // current velocity smoothing reference

    [Header("events")]
    public UnityEvent OnLandEvent;                                       // called when boss lands on ground

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }
    public BoolEvent OnCrouchEvent;                                      // called when boss crouches or stands
    private bool m_wasCrouching = false;                                 // tracks crouch state for event control

    private void Awake()
    {
        // cache rigidbody and ensure event references exist
        rb = GetComponent<Rigidbody2D>();
        if (OnLandEvent == null) OnLandEvent = new UnityEvent();
        if (OnCrouchEvent == null) OnCrouchEvent = new BoolEvent();
    }

    private void FixedUpdate()
    {
        // update grounded status by checking overlap below feet
        bool wasGrounded = m_Grounded;
        m_Grounded = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;
                if (!wasGrounded)
                    OnLandEvent.Invoke();
            }
        }
    }

    public void Move(float move, bool crouch, bool jump)
    {
        // step 1: check if ceiling is too low to stand
        if (!crouch)
        {
            if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
                crouch = true;
        }

        // step 2: crouch logic and collider toggling
        if (m_Grounded || m_AirControl)
        {
            if (crouch)
            {
                if (!m_wasCrouching)
                {
                    m_wasCrouching = true;
                    OnCrouchEvent.Invoke(true);
                }

                move *= m_CrouchSpeed; // reduce speed when crouched
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

            // step 3: detect slope under the boss
            Vector2 groundNormal;
            float slopeAngle;
            bool onSlope = TryGetSlope(out groundNormal, out slopeAngle);

            // step 4: calculate target velocity
            Vector2 current = rb.linearVelocity;
            Vector2 target;

            if (m_Grounded && onSlope && slopeAngle > 0.1f && slopeAngle < maxSlopeAngle)
            {
                // step 4a: move along slope direction
                Vector2 tangent = new Vector2(groundNormal.y, -groundNormal.x).normalized;
                float dir = Mathf.Sign(move);
                float mag = Mathf.Abs(move) * moveSpeedScalar;

                Vector2 along = tangent * dir * mag;

                // add a small push to help climb slopes
                if (Mathf.Abs(move) > 0.01f)
                {
                    Vector2 push = new Vector2(along.x, Mathf.Max(0f, along.y)) * (uphillAssist / 100f);
                    rb.AddForce(push);
                }

                target = new Vector2(along.x, onSlope ? along.y : current.y);
            }
            else
            {
                // step 4b: handle flat ground or air movement
                target = new Vector2(move * moveSpeedScalar, current.y);
            }

            // step 5: smooth velocity transitions
            Vector2 smoothed = Vector2.SmoothDamp(current, target, ref m_Velocity, m_MovementSmoothing);
            rb.linearVelocity = smoothed;

            // step 6: prevent sliding when idle
            if (m_Grounded && Mathf.Abs(move) < 0.01f)
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

            // step 7: handle sprite flipping
            if (move > 0 && !m_FacingRight)
                Flip();
            else if (move < 0 && m_FacingRight)
                Flip();
        }

        // step 8: apply jump force if grounded
        if (m_Grounded && jump)
        {
            m_Grounded = false;
            rb.AddForce(new Vector2(0f, m_JumpForce), ForceMode2D.Impulse);
        }
    }

    // detects slope angle and normal below the boss
    private bool TryGetSlope(out Vector2 groundNormal, out float slopeAngle)
    {
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

    // flips the sprite when changing movement direction
    private void Flip()
    {
        m_FacingRight = !m_FacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1f;
        transform.localScale = theScale;
    }
}
