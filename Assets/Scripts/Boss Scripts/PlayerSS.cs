using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class PlayerSS : MonoBehaviour
{
    //Character Controller variables
    [SerializeField] private CharacterController2D controller;
    [SerializeField] private float _horizontalMove = 0;
    [SerializeField] private float _runSpeed = 0;
    [SerializeField] private Animator _anim;
    private Collider2D _collider;
    bool jump = false;

    //Attack related variables
    [SerializeField] private Transform _attkPnt;
    [SerializeField] private float _attackRange;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private float baseDamage = 10f;
    [SerializeField] private float scaleFactor = 0.15f;
    [SerializeField] private LayerMask _enemyLayers;
    bool isAttacking = false;
    float nextAttkTime = 0f;

    //Dashing variables
    [SerializeField] private TrailRenderer _trailRenderer;
    [SerializeField] private float dashingPower = 10f;
    private bool canDash = true;
    private bool isDashing;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;

    //Player Health
    [SerializeField] private float _baseHealth = 100f;
    [SerializeField] private float _maxHealth = 0f;
    [SerializeField] private float _currentHealth = 0f;
    [SerializeField] private float iFrameDuration = 1.5f;
    [SerializeField] private float blinkDuration = 0.1f;
    [SerializeField] private HealthBar _healthBar;
    private bool isDead = false;
    private bool iFrame = false;
    private SpriteRenderer _spriteRenderer;

    //MISC
    private GameObject _gameManager;
    private void Start()
    {
        _maxHealth = _baseHealth * StatManager.instance.GetHealthAmount();
        _anim = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _currentHealth = _maxHealth;
        _healthBar.SetMaxHealth(_maxHealth);
        _collider = GetComponent<Collider2D>();
        _gameManager = GameObject.Find("UIManager");
        StartCoroutine(StartTimer());

    }
    private void Update()
    {

        if (!isDead)
        {
            if (isDashing)
            {
                return;
            }
            _horizontalMove = Input.GetAxisRaw("Horizontal") * _runSpeed;
            if (Input.GetButtonDown("Jump") && !isAttacking)
            {
                jump = true;
                _anim.SetTrigger("Jump");
            }
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

            if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
            {
                _anim.SetTrigger("Dash");
                StartCoroutine(Dash());
            }
        }
    }

    void Attack()
    {
        _anim.SetTrigger("Attack");
        isAttacking = true;
        _runSpeed = 0;
        StartCoroutine(AttackCD());
    }

    void DealDamage()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(_attkPnt.position, _attackRange, _enemyLayers);
        float damage = CalculateDamage();
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.gameObject.activeInHierarchy) // Check if spawned
            {
                BossSmallEnemies smallEnemy = enemy.GetComponent<BossSmallEnemies>();
                if (smallEnemy != null)
                {
                    if (smallEnemy.GetCurrenthealth() <= damage)
                        SoundManager.PlaySound(SoundTypeEffects.NECROMANCER_VATS_DESTROYED); // play a sound
                    else
                        SoundManager.PlaySound(SoundTypeEffects.NECROMANCER_VATS_TAKES_DAMAGE); // play a sound
                    smallEnemy.TakeDamage(damage);
                    Debug.Log("Hit " + enemy.name);
                }

                BossMonster bossEnemy = enemy.GetComponent<BossMonster>();
                if (bossEnemy != null)
                {
                    if (bossEnemy.getHealth() <= damage)
                        SoundManager.PlaySound(SoundTypeEffects.NECROMANCER_DEATH); // play a sound
                    else
                        SoundManager.PlaySound(SoundTypeEffects.NECROMANCER_TAKES_DAMAGE); // play a sound
                    bossEnemy.TakeDamage(damage);
                    Debug.Log("HIT: " + bossEnemy.name);
                }

                SpawnEnemies spawnEnemy = enemy.GetComponent<SpawnEnemies>();
                if (spawnEnemy != null)
                {
                    if (spawnEnemy.getHealth() <= damage)
                        SoundManager.PlaySound(SoundTypeEffects.NECROMANCER_MINION_DEATH); // play a sound
                    else
                        SoundManager.PlaySound(SoundTypeEffects.NECROMANCER_MINION_TAKES_DAMAGE); // play a sound
                    spawnEnemy.TakeDamage(damage);
                    Debug.Log("HIT: " + spawnEnemy.name);
                }

                isEnemy skullEnemy = enemy.GetComponent<isEnemy>();
                if (skullEnemy != null)
                {
                    if (skullEnemy.getHealth() <= damage)
                        SoundManager.PlaySound(SoundTypeEffects.NECROMANCER_SKULLS_DEATH); // play a sound
                    else
                        SoundManager.PlaySound(SoundTypeEffects.NECROMANCER_SKULLS_TAKES_DAMAGE); // play a sound
                    skullEnemy.TakeDamage(damage);
                    Debug.Log("HIT: " + skullEnemy.name);
                }
            }
        }
    }
    void Dead()
    {
        _anim.SetTrigger("Death");
        _runSpeed = 0;
        controller.rb.linearVelocity = new Vector2(0f, 0f);
        SceneLoader.instance.LoadSpecificLevel(2);
        canDash = false;
    }
    public void TakeDamage(float damage)
    {
        if (iFrame)
        {
            return;
        }
        _currentHealth -= damage;
        _healthBar.SetHealth(_currentHealth);
        _anim.SetTrigger("Hurt");

        if (_currentHealth <= 0)
        {
            SoundManager.PlaySound(SoundTypeEffects.WARRIOR_DEATH); // play a sound
            isDead = true;
            Dead();
        }
        else
        {
            SoundManager.PlaySound(SoundTypeEffects.WARRIOR_TAKES_DAMAGE); // play a sound
            StartCoroutine(IFrames());
        }
    }
    void PlayerInput()
    {
        if (_horizontalMove >= 1 || _horizontalMove <= -1)
        {
            _anim.SetInteger("AnimState", 2);
        }
        else
        {
            _anim.SetInteger("AnimState", 0);
        }

        controller.Move(_horizontalMove * Time.fixedDeltaTime, false, jump);

        jump = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (isDashing)
        {
            return;
        }
        if (!isAttacking && !isDead)
        {
            PlayerInput();
        }
    }

    private void OnDrawGizmos()
    {
        if (_attkPnt == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(_attkPnt.position, _attackRange);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
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

    private IEnumerator Dash()
    {
        SoundManager.PlaySound(SoundTypeEffects.WARRIOR_DASH_PORTAL);
        canDash = false;
        isDashing = true;
        isAttacking = true;
        iFrame = true;
        _collider.enabled = false;
        float originalGravity = controller.rb.gravityScale;
        controller.rb.gravityScale = 0;
        controller.rb.linearVelocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        _trailRenderer.emitting = true;
        yield return new WaitForSeconds(dashingTime);
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
        yield return new WaitForSeconds(0.67f);
        _runSpeed = 25;
        isAttacking = false;
    }

    private IEnumerator IFrames()
    {
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
        isDead = true;
        _anim.SetTrigger("Recover");
        yield return new WaitForSeconds(1f);
        isDead = false;
    }

    private float CalculateDamage()
    {
        int level = ExperienceManager.instance != null ? ExperienceManager.instance.GetCurrentLevel() : 1;
        float damage = (baseDamage * (1f + scaleFactor * level)) * StatManager.instance.GetDamageIncrease();

        // Crit check
        if (Random.value < StatManager.instance.GetCritChance())
        {
            damage *= StatManager.instance.GetCritDamage();
            Debug.Log($"Critical hit! {damage:F1}");
        }

        return damage;
    }
}
