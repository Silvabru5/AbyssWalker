using UnityEngine;
using UnityEngine.Events;

/*
    Author(s): Bruno Silva
    Description: controls a 2D boss character and provides grounded checks,
                 slope handling, crouching, smooth horizontal movement,
                 and jumping. this controller is designed for platform-based
                 boss encounters where slopes and terrain shape affect
                 movement behavior.
    Date (last modification): 11/22/2025
*/

public class CharacterController2DBoss : MonoBehaviour
{
    [Header("movement settings")]
    [SerializeField] private float m_JumpForce = 400f;
    // amount of upward force applied when the boss jumps

    [Range(0, 1)]
    [SerializeField] private float m_CrouchSpeed = .36f;
    // speed reduction applied while crouching
    // 1 means normal speed, 0 means no movement

    [Range(0, .3f)]
    [SerializeField] private float m_MovementSmoothing = .05f;
    // smoothing factor used to gradually adjust movement velocity

    [SerializeField] private bool m_AirControl = false;
    // determines whether movement input affects the boss while in the air

    [SerializeField] private LayerMask m_WhatIsGround;
    // layer(s) considered ground for grounded checks and slope detection

    [SerializeField] private Transform m_GroundCheck;
    // position below the boss used to test for ground using an overlap circle

    [SerializeField] private Transform m_CeilingCheck;
    // position above the boss used to prevent uncrouching under low ceilings

    [SerializeField] private Collider2D m_CrouchDisableCollider;
    // optional collider that is disabled while crouching to fit under low areas

    private const float k_GroundedRadius = .2f;
    // radius of the overlap circle used for ground detection

    public bool m_Grounded;
    // indicates whether the boss is currently touching the ground

    private const float k_CeilingRadius = .2f;
    // radius used to check whether a low ceiling prevents standing up

    [Header("slope settings")]
    [SerializeField] private float slopeRayLength = 0.5f;
    // distance for a downward raycast used to determine slope angle

    [SerializeField] private float maxSlopeAngle = 45f;
    // maximum slope angle the boss is allowed to walk on

    [SerializeField] private float uphillAssist = 25f;
    // amount of additional upward force applied when moving uphill

    [SerializeField] private float moveSpeedScalar = 10f;
    // multiplier used to scale the horizontal input into movement velocity

    public Rigidbody2D rb;
    // main rigidbody controlling boss physics

    private bool m_FacingRight = true;
    // tracks which direction the boss is facing for sprite flipping

    private Vector2 m_Velocity = Vector2.zero;
    // used by SmoothDamp to gradually adjust the velocity

    [Header("events")]
    public UnityEvent OnLandEvent;
    // invoked when the boss transitions from being airborne to grounded

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    public BoolEvent OnCrouchEvent;
    // invoked when the boss starts crouching or stops crouching

    private bool m_wasCrouching = false;
    // tracks the previous crouch state to ensure events fire correctly

    private void Awake()
    {
        // cache the rigidbody reference if not set
        rb = GetComponent<Rigidbody2D>();

        // ensure events are initialized
        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        if (OnCrouchEvent == null)
            OnCrouchEvent = new BoolEvent();
    }

    private void FixedUpdate()
    {
        // store grounded value from last frame to detect landing transitions
        bool wasGrounded = m_Grounded;
        m_Grounded = false;

        // check for ground contact by overlapping a circle at the ground check position
        Collider2D[] colliders = Physics2D.OverlapCircleAll(
            m_GroundCheck.position,
            k_GroundedRadius,
            m_WhatIsGround
        );

        // loop through all detected colliders
        for (int i = 0; i < colliders.Length; i++)
        {
            // make sure the collider detected is not part of the boss itself
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;

                // trigger landing event if we were not grounded in the previous frame
                if (!wasGrounded)
                    OnLandEvent.Invoke();
            }
        }
    }

    public void Move(float move, bool crouch, bool jump)
    {
        // check if there is a low ceiling directly above; if so, force crouching
        if (!crouch)
        {
            if (Physics2D.OverlapCircle(
                m_CeilingCheck.position,
                k_CeilingRadius,
                m_WhatIsGround))
            {
                crouch = true;
            }
        }

        // handle crouching and optional collider disabling
        if (m_Grounded || m_AirControl)
        {
            if (crouch)
            {
                // fire event on crouch start
                if (!m_wasCrouching)
                {
                    m_wasCrouching = true;
                    OnCrouchEvent.Invoke(true);
                }

                // reduce movement speed while crouched
                move *= m_CrouchSpeed;

                // disable crouch collider when crouched if assigned
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = false;
            }
            else
            {
                // re-enable crouch collider when standing up
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = true;

                // fire event when leaving crouch state
                if (m_wasCrouching)
                {
                    m_wasCrouching = false;
                    OnCrouchEvent.Invoke(false);
                }
            }

            // determine slope angle and ground normal under the boss
            Vector2 groundNormal;
            float slopeAngle;
            bool onSlope = TryGetSlope(out groundNormal, out slopeAngle);

            Vector2 currentVelocity = rb.linearVelocity;
            Vector2 targetVelocity;

            // if standing on a valid slope, adjust movement so the boss walks along the slope
            if (m_Grounded && onSlope && slopeAngle > 0.1f && slopeAngle < maxSlopeAngle)
            {
                // calculate tangent direction of the slope
                Vector2 slopeTangent = new Vector2(groundNormal.y, -groundNormal.x).normalized;

                // determine movement direction relative to slope
                float direction = Mathf.Sign(move);
                float magnitude = Mathf.Abs(move) * moveSpeedScalar;

                // calculate final movement along slope
                Vector2 alongSlope = slopeTangent * direction * magnitude;

                // add extra upward push when climbing uphill
                if (Mathf.Abs(move) > 0.01f)
                {
                    Vector2 assist = new Vector2(
                        alongSlope.x,
                        Mathf.Max(0f, alongSlope.y)
                    ) * (uphillAssist / 100f);

                    rb.AddForce(assist);
                }

                // apply slope-adjusted velocity  
                targetVelocity = new Vector2(
                    alongSlope.x,
                    onSlope ? alongSlope.y : currentVelocity.y
                );
            }
            else
            {
                // normal horizontal movement on flat ground or in the air
                targetVelocity = new Vector2(
                    move * moveSpeedScalar,
                    currentVelocity.y
                );
            }

            // apply smoothed velocity to avoid sudden movement changes
            Vector2 smoothedVelocity = Vector2.SmoothDamp(
                currentVelocity,
                targetVelocity,
                ref m_Velocity,
                m_MovementSmoothing
            );

            rb.linearVelocity = smoothedVelocity;

            // prevent sliding when no horizontal input is given
            if (m_Grounded && Mathf.Abs(move) < 0.01f)
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

            // flip the boss horizontally when changing direction
            if (move > 0 && !m_FacingRight)
                Flip();
            else if (move < 0 && m_FacingRight)
                Flip();
        }

        // handle jumping when grounded
        if (m_Grounded && jump)
        {
            m_Grounded = false;
            rb.AddForce(new Vector2(0f, m_JumpForce), ForceMode2D.Impulse);
        }
    }

    // raycast downward to determine slope normal and angle
    private bool TryGetSlope(out Vector2 groundNormal, out float slopeAngle)
    {
        RaycastHit2D hit = Physics2D.Raycast(
            m_GroundCheck.position,
            Vector2.down,
            slopeRayLength,
            m_WhatIsGround
        );

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

    // flips the boss horizontally by inverting its x scale
    private void Flip()
    {
        m_FacingRight = !m_FacingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }
}
