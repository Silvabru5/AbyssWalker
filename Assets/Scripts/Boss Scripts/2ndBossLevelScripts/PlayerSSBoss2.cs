using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// handles player control, movement, combat, and health during the dracula boss fight
public class PlayerSSBoss2 : MonoBehaviour
{
    [Header("movement settings")]
    [SerializeField] private CharacterController2DBoss controller; // reference to custom character controller
    [SerializeField] private float _runSpeed = 10f;                // player run speed
    private float _horizontalMove = 0f;                            // current horizontal input
    private bool jump = false;                                     // jump flag

    [Header("animation and physics")]
    [SerializeField] private Animator _anim;                       // animator reference
    private Collider2D _collider;                                  // collider reference
    private Rigidbody2D _rb;                                       // rigidbody reference
    private SpriteRenderer _spriteRenderer;                        // sprite renderer reference

    [Header("attack settings")]
    [SerializeField] private Transform _attkPnt;                   // attack point transform
    [SerializeField] private float _attackRange = 0.75f;           // radius of attack range
    [SerializeField] private float _attackSpeed = 2f;              // attack speed (attacks per second)
    [SerializeField] private float _attackDamage = 30f;            // attack damage amount
    [SerializeField] private LayerMask _enemyLayers;               // layers the player can attack
    private bool isAttacking = false;                              // attack flag
    private float nextAttkTime = 0f;                               // cooldown timer

    [Header("dash settings")]
    [SerializeField] private TrailRenderer _trailRenderer;         // trail renderer for dash effect
    [SerializeField] private float dashingPower = 10f;             // dash force
    private bool canDash = true;                                   // dash availability flag
    private bool isDashing;                                        // dash state flag
    private float dashingTime = 0.2f;                              // duration of dash
    private float dashingCooldown = 1f;                            // dash cooldown delay

    [Header("health settings")]
    [SerializeField] private float _maxHealth = 100f;              // maximum player health
    [SerializeField] private float _currentHealth = 0f;            // current health
    [SerializeField] private float iFrameDuration = 1.5f;          // invulnerability duration after damage
    [SerializeField] private float blinkDuration = 0.1f;           // blink rate while invulnerable
    private bool isDead = false;                                   // death flag
    private bool iFrame = false;                                   // invulnerability flag

    [Header("ui and game references")]
    private GameObject _gameManager;                               // ui manager reference

    private void Start()
    {
        // initialize core components
        _anim = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        _rb = GetComponent<Rigidbody2D>();
        _currentHealth = _maxHealth;
        _gameManager = GameObject.Find("UIManager");

        // short intro recovery animation before control starts
        StartCoroutine(StartTimer());
    }

    private void Update()
    {
        // skip input handling when dead or dashing
        if (isDead || isDashing) return;

        // handle horizontal input
        _horizontalMove = Input.GetAxisRaw("Horizontal") * _runSpeed;

        // handle jump input
        if (Input.GetButtonDown("Jump") && !isAttacking)
        {
            jump = true;
            _anim.SetTrigger("Jump");
        }

        // handle attack input with cooldown
        if (Time.time >= nextAttkTime)
        {
            if (Input.GetButtonDown("Fire1") && controller.m_Grounded)
            {
                SoundManager.PlaySound(SoundTypeEffects.WARRIOR_ATTACK);
                jump = false;
                Attack();
                nextAttkTime = Time.time + 1f / _attackSpeed;
                isAttacking = false;
            }
        }

        // handle dash input
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            SoundManager.PlaySound(SoundTypeEffects.WARRIOR_DASH_PORTAL);
            _anim.SetTrigger("Dash");
            StartCoroutine(Dash());
        }
    }

    private void FixedUpdate()
    {
        // skip movement logic if dead, attacking, or dashing
        if (isDead || isAttacking || isDashing)
            return;

        // apply input-based movement
        PlayerInput();

        // correct for slope sliding when idle
        HandleSlopeSliding();
    }

    private void PlayerInput()
    {
        // set animation based on movement state
        if (_horizontalMove != 0)
            _anim.SetInteger("AnimState", 2); // moving
        else
            _anim.SetInteger("AnimState", 0); // idle

        // convert movement input to fixed delta movement
        float moveInput = _horizontalMove * Time.fixedDeltaTime;

        // apply small ground boost for smoother control
        if (controller.m_Grounded)
            moveInput *= 1.3f;

        controller.Move(moveInput, false, jump);
        jump = false;
    }

    private void HandleSlopeSliding()
    {
        // stops unwanted sliding when standing still on slopes
        if (controller.m_Grounded && Mathf.Abs(_rb.linearVelocity.x) < 0.05f)
            _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y);
    }

    private void Attack()
    {
        // triggers attack animation and locks movement
        _anim.SetTrigger("Attack");
        isAttacking = true;
        _runSpeed = 0f;

        StartCoroutine(AttackCD());
    }

    private void DealDamage()
    {
        // detects and damages all enemies in attack range
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(_attkPnt.position, _attackRange, _enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            if (!enemy.gameObject.activeInHierarchy)
                continue;

            // attempt to find valid boss scripts on the enemy
            var bossA = enemy.GetComponent<BossMonster>() ?? enemy.GetComponentInParent<BossMonster>();
            var bossB = enemy.GetComponent<BossMonsterDracula>() ?? enemy.GetComponentInParent<BossMonsterDracula>();

            if (bossA != null)
            {
                bossA.TakeDamage(_attackDamage);
                SoundManager.PlaySound(SoundTypeEffects.VAMPIRE_TAKES_DAMAGE);
            }
            else if (bossB != null)
            {
                bossB.TakeDamage(_attackDamage);
                if (bossB.getHealth() <= _attackDamage)
                    SoundManager.PlaySound(SoundTypeEffects.VAMPIRE_DEATH);
                else
                    SoundManager.PlaySound(SoundTypeEffects.VAMPIRE_TAKES_DAMAGE);
            }
        }
    }

    private void Dead()
    {
        // plays death animation and stops all control
        _anim.SetTrigger("Death");
        SoundManager.PlaySound(SoundTypeEffects.WARRIOR_DEATH);
        _runSpeed = 0f;
        controller.rb.linearVelocity = Vector2.zero;

        // shows game over screen via manager
        _gameManager.GetComponent<GameOverManager>().ShowGameOver();
        canDash = false;
    }

    public void TakeDamage(float damage)
    {
        // prevents repeated damage during invulnerability
        if (iFrame)
            return;

        // apply damage and trigger hurt animation
        _currentHealth -= damage;
        _anim.SetTrigger("Hurt");

        // if health is depleted, trigger death
        if (_currentHealth <= 0)
        {
            SoundManager.PlaySound(SoundTypeEffects.WARRIOR_DEATH);
            isDead = true;
            Dead();
        }
        else
        {
            SoundManager.PlaySound(SoundTypeEffects.WARRIOR_TAKES_DAMAGE);
            StartCoroutine(IFrames());
        }
    }

    private IEnumerator Dash()
    {
        // starts dash movement and temporary invulnerability
        canDash = false;
        isDashing = true;
        isAttacking = true;
        iFrame = true;

        _collider.enabled = false;

        // apply dash force
        float originalGravity = controller.rb.gravityScale;
        controller.rb.gravityScale = 0;
        controller.rb.linearVelocity = new Vector2(transform.localScale.x * dashingPower, 0f);

        _trailRenderer.emitting = true;
        SoundManager.PlaySound(SoundTypeEffects.WARRIOR_DASH_PORTAL);

        yield return new WaitForSeconds(dashingTime);

        // stop dash and reset state
        _trailRenderer.emitting = false;
        controller.rb.gravityScale = originalGravity;
        isDashing = false;
        isAttacking = false;
        iFrame = false;
        _collider.enabled = true;

        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    private IEnumerator AttackCD()
    {
        // small delay after attack before resuming movement
        yield return new WaitForSeconds(0.67f);
        _runSpeed = 10f;
        isAttacking = false;
    }

    private IEnumerator IFrames()
    {
        // temporary invulnerability with sprite blinking
        iFrame = true;
        float elapsedTime = 0f;

        while (elapsedTime < iFrameDuration)
        {
            _spriteRenderer.enabled = !_spriteRenderer.enabled;
            yield return new WaitForSeconds(blinkDuration);
            elapsedTime += blinkDuration;
        }

        _spriteRenderer.enabled = true;
        iFrame = false;
    }

    private IEnumerator StartTimer()
    {
        // small startup recovery animation before control resumes
        isDead = true;
        _anim.SetTrigger("Recover");
        yield return new WaitForSeconds(1f);
        isDead = false;
    }

    private void OnDrawGizmos()
    {
        // draws attack radius in editor for clarity
        if (_attkPnt == null) return;
        Gizmos.DrawWireSphere(_attkPnt.position, _attackRange);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // checks for collisions with enemies or hazards
        if (collision.gameObject.GetComponent<isEnemy>())
        {
            if (!iFrame)
            {
                TakeDamage(10f);
                Destroy(collision.gameObject);
            }
        }

        if (collision.gameObject.GetComponent<isFire>())
        {
            TakeDamage(15f);
        }
    }
}
