using System.Collections;
using UnityEngine;

public class WarriorController2D : MonoBehaviour
{


    // private SpriteRenderer _spriteRenderer; 
    private Collider2D _collider;


    [Header("Movement")]
    public float moveSpeed = 3f;
    public float jumpForce = 7f;

    [Header("Combat")]
    public Transform attackPoint;           
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;           
    public float attackCooldown = 0.5f;

    private bool canAttack = true;

    [SerializeField] private float dashingPower = 10f;
    [SerializeField] private TrailRenderer _trailRenderer;
    private bool canDash = true;
    private bool isDashing;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;

    [Header("References")]
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;





    //attack stuf
    [SerializeField] private float baseDamage = 10f;
    [SerializeField] private float scaleFactor = 0.15f;

    //Player Health
    [SerializeField] private float _baseHealth = 100f;
    [SerializeField] private float _maxHealth = 0f;
    [SerializeField] private float _currentHealth = 0f;
    [SerializeField] private float iFrameDuration = 1.5f;
    [SerializeField] private float blinkDuration = 0.1f;
    [SerializeField] private HealthBar _healthBar;
    private bool isDead = false;
    private bool iFrame = false;

    private void Start()
    {
        _maxHealth = _baseHealth * StatManager.instance.GetHealthAmount();
         _currentHealth = _maxHealth;
        _healthBar.SetMaxHealth(_maxHealth);
        _collider = GetComponent<Collider2D>();
        



        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {

        if (!isDead)
        {
            if (isDashing)
            {
                return;
            }

            float move = Input.GetAxisRaw("Horizontal");
            rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);

            if (move > 0) sr.flipX = false;
            else if (move < 0) sr.flipX = true;

            anim.SetFloat("Speed", Mathf.Abs(move));
            anim.SetBool("isJumping", !IsGrounded());

            if (Input.GetButtonDown("Jump") && IsGrounded())
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }

            if (Input.GetMouseButtonDown(0) && canAttack) // Left click to attack
            {
                Attack();
            }
            if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
            {
                // _anim.SetTrigger("Dash");
                StartCoroutine(Dash());
            }
        }






    }
    

    void DealDamage()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
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


    private void Attack()
    {
        float damage = CalculateDamage();
        SoundManager.PlaySound(SoundTypeEffects.WARRIOR_ATTACK);
        anim.SetTrigger("Attack");
        canAttack = false;
        Invoke(nameof(ResetAttack), attackCooldown);

        // detect enemies
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Hit: " + enemy.name);
            enemy.GetComponent<BossMonsterDracula>()?.TakeDamage(damage);

        }
        DealDamage();
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




    private void ResetAttack()
    {
        canAttack = true;
    }

    private bool IsGrounded()
    {
        return rb.linearVelocity.y == 0;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
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


    public void TakeDamage(float damage)
    {
        if (iFrame)
        {
            return;
        }
        _currentHealth -= damage;
        _healthBar.SetHealth(_currentHealth);


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


    void Dead()
    {
        // _anim.SetTrigger("Death");
        moveSpeed = 0;
        rb.linearVelocity = new Vector2(0f, 0f);
        SceneLoader.instance.LoadSpecificLevel(2);
        canDash = false;
    }
    

        private IEnumerator Dash()
    {
        SoundManager.PlaySound(SoundTypeEffects.WARRIOR_DASH_PORTAL);
        canDash = false;
        isDashing = true;
        iFrame = true;
        _collider.enabled = false;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;
        rb.linearVelocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        _trailRenderer.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        _trailRenderer.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        iFrame = false;
        _collider.enabled = true;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }


    private IEnumerator IFrames()
    {
        iFrame = true;
        float elapsedTime = 0f;

        while (elapsedTime < iFrameDuration)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(blinkDuration);
            elapsedTime += blinkDuration;
        }

        sr.enabled = true;
        iFrame = false;

    }
}
