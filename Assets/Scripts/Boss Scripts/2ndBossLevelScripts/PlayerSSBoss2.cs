using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSSBoss2 : MonoBehaviour
{
    [SerializeField] private CharacterController2DBoss controller;
    [SerializeField] private float _horizontalMove = 0;
    [SerializeField] private float _runSpeed = 10f; 
    [SerializeField] private Animator _anim;
    private Collider2D _collider;
    private Rigidbody2D _rb;
    private bool jump = false;

    [SerializeField] private Transform _attkPnt;
    [SerializeField] private float _attackRange = 0.75f;
    [SerializeField] private float _attackSpeed = 2f;
    [SerializeField] private float _attackDamage = 30f;
    [SerializeField] private LayerMask _enemyLayers;
    private bool isAttacking = false;
    private float nextAttkTime = 0f;

    [SerializeField] private TrailRenderer _trailRenderer;
    [SerializeField] private float dashingPower = 10f;
    private bool canDash = true;
    private bool isDashing;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;

    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _currentHealth = 0f;
    [SerializeField] private float iFrameDuration = 1.5f;
    [SerializeField] private float blinkDuration = 0.1f;
    private bool isDead = false;
    private bool iFrame = false;
    private SpriteRenderer _spriteRenderer;

    private GameObject _gameManager;

    private void Start()
    {
        // Step 1: Initialize references and variables
        _anim = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        _rb = GetComponent<Rigidbody2D>();
        _currentHealth = _maxHealth;
        _gameManager = GameObject.Find("UIManager");
        StartCoroutine(StartTimer());

    }

    private void Update()
    {
        // Step 1: Skip logic if dead or dashing
        if (isDead || isDashing)
            return;

        // Step 2: Handle horizontal input
        _horizontalMove = Input.GetAxisRaw("Horizontal") * _runSpeed;


        // Step 3: Handle jump input
        if (Input.GetButtonDown("Jump") && !isAttacking)
        {
            jump = true;
            _anim.SetTrigger("Jump");
        }

        // Step 4: Handle attack input
        if (Time.time >= nextAttkTime)
        {
            if (Input.GetButtonDown("Fire1") && controller.m_Grounded)
            {
                jump = false;
                Attack();
                nextAttkTime = Time.time + 1f / _attackSpeed;
                isAttacking = false;
            }
        }

        // Step 5: Handle dash input
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            _anim.SetTrigger("Dash");
            StartCoroutine(Dash());
        }
    }

    private void FixedUpdate()
    {

        // Step 1: Skip logic if dashing, attacking, or dead
        if (isDead || isAttacking || isDashing)
            return;

        // Step 2: Handle movement and slope correction
        PlayerInput();

        // Step 3: Prevent small sliding on slopes
        HandleSlopeSliding();
    }

    private void PlayerInput()
    {
        // Step 1: Handle animation state
        if (_horizontalMove != 0)
            _anim.SetInteger("AnimState", 2);
        else
            _anim.SetInteger("AnimState", 0);

        // Step 2: Apply movement
        float moveInput = _horizontalMove * Time.fixedDeltaTime;

        // Step 3: Add slope compensation boost
        if (controller.m_Grounded)
            moveInput *= 1.3f;

        controller.Move(moveInput, false, jump);
        jump = false;
    }

    private void HandleSlopeSliding()
    {
        // Step 1: Stop micro-sliding when idle on slopes
        if (controller.m_Grounded && Mathf.Abs(_rb.linearVelocity.x) < 0.05f)
            _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y);
    }

    private void Attack()
    {
        
        // Step 1: Play animation and set flags
        _anim.SetTrigger("Attack");
        isAttacking = true;
        _runSpeed = 0;
        SoundManager.PlaySound(SoundTypeEffects.WARRIOR_ATTACK);
        StartCoroutine(AttackCD());
        
    }

    //private void DealDamage()
    //{
    //    Debug.Log("[Player] DealDamage() called!");
    //    // Step 1: Detect enemies in range
    //    Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(_attkPnt.position, _attackRange, _enemyLayers);
    //    Debug.Log("[Player] HitEnemies count: " + hitEnemies.Length);
        

    //    // Step 2: Apply damage to each target type
    //    foreach (Collider2D enemy in hitEnemies)
    //    {
    //        if (!enemy.gameObject.activeInHierarchy)
    //            continue;

    //        BossMonster dracula = enemy.GetComponent<BossMonster>();
    //        if (dracula != null)
    //        {
    //            dracula.TakeDamage(10f); // Dracula loses 10 HP per hit
    //            Debug.Log("Hit Dracula! HP reduced by 10");
    //        }
    //    }
    //}
    private void DealDamage()
    {
        Debug.Log("[Player] DealDamage() called!");

        // Step 1: Find all enemies in range
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(_attkPnt.position, _attackRange, _enemyLayers);
        Debug.Log("[Player] HitEnemies count: " + hitEnemies.Length);

        foreach (Collider2D enemy in hitEnemies)
        {
            if (!enemy.gameObject.activeInHierarchy)
                continue;

            Debug.Log("[Player] Found collider: " + enemy.name);

            // Step 2: Try to get any boss-type component
            var bossA = enemy.GetComponent<BossMonster>();
            var bossB = enemy.GetComponent<BossMonsterDracula>();

            if (bossA != null)
            {
                Debug.Log("[Player] Damaging BossMonster component on: " + enemy.name);
                bossA.TakeDamage(10f);
                SoundManager.PlaySound(SoundTypeEffects.VAMPIRE_TAKES_DAMAGE);
            }
            else if (bossB != null)
            {
                Debug.Log("[Player] Damaging BossMonsterDracula component on: " + enemy.name);
                bossB.TakeDamage(10f);
                SoundManager.PlaySound(SoundTypeEffects.VAMPIRE_TAKES_DAMAGE);
            }
            else
            {
                Debug.LogWarning("[Player] Enemy in range but no boss script found: " + enemy.name);
            }
        }
    }

    private void Dead()
    {
        // Step 1: Play death animation and disable input
        _anim.SetTrigger("Death");
        SoundManager.PlaySound(SoundTypeEffects.WARRIOR_DEATH);
        _runSpeed = 0;
        controller.rb.linearVelocity = new Vector2(0f, 0f);
        _gameManager.GetComponent<GameOverManager>().ShowGameOver();
        canDash = false;
    }

    public void TakeDamage(float damage)
    {
        // Step 1: Handle invulnerability
        if (iFrame)
            return;

        // Step 2: Apply damage
        _currentHealth -= damage;
        _anim.SetTrigger("Hurt");

        // Step 3: Handle death
        if (_currentHealth <= 0)
        {
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
        // Step 1: Set dash flags
        canDash = false;
        isDashing = true;
        isAttacking = true;
        iFrame = true;

        // Step 2: Disable collider during dash
        _collider.enabled = false;

        // Step 3: Perform dash
        float originalGravity = controller.rb.gravityScale;
        controller.rb.gravityScale = 0;
        controller.rb.linearVelocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        _trailRenderer.emitting = true;
        SoundManager.PlaySound(SoundTypeEffects.WARRIOR_DASH_PORTAL);
        yield return new WaitForSeconds(dashingTime);

        // Step 4: End dash and reset
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
        // Step 1: Wait and restore run speed
        yield return new WaitForSeconds(0.67f);
        _runSpeed = 10f;
        isAttacking = false;
    }

    private IEnumerator IFrames()
    {
        // Step 1: Blink and disable damage for a short time
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
        // Step 1: Small delay at start before player can move
        isDead = true;
        _anim.SetTrigger("Recover");
        yield return new WaitForSeconds(1f);
        isDead = false;
    }

    private void OnDrawGizmos()
    {
        // Step 1: Draw attack range gizmo
        if (_attkPnt == null)
            return;
        Gizmos.DrawWireSphere(_attkPnt.position, _attackRange);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Step 1: Take damage on enemy or fire collision
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
