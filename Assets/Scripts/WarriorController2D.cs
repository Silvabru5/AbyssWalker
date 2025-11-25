/*
    Author(s): Bruno Silva, Carey Cunninghum, Adrian Agius, Tristan Ung
    Description: Handles full 2D Warrior player control including movement, jumping, 
                 melee combat, dashing with invulnerability frames, taking damage, 
                 blinking immunity, death logic, and interaction with various enemy types. 
                 Works with StatManager, ExperienceManager, SoundManager, and SceneLoader 
                 to support dynamic stat scaling, sound effects, and scene transitions.
    Date (last modification): 11/24/2025
*/

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

    // called when the object is first created
    private void Start()
    {
        // calculate player max health using stat bonuses
        _maxHealth = _baseHealth * StatManager.instance.GetHealthAmount();
        _currentHealth = _maxHealth;

        // update the health bar ui
        _healthBar.SetMaxHealth(_maxHealth);

        // get component references
        _collider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    // called every frame
    private void Update()
    {
        // do nothing if the player is dead
        if (!isDead)
        {
            // stop all other movement while dashing
            if (isDashing)
            {
                return;
            }

            // read horizontal movement input
            float move = Input.GetAxisRaw("Horizontal");

            // apply movement velocity
            rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);

            // flip the sprite based on direction
            if (move > 0) sr.flipX = false;
            else if (move < 0) sr.flipX = true;

            // update animation values
            anim.SetFloat("Speed", Mathf.Abs(move));
            anim.SetBool("isJumping", !IsGrounded());

            // jump if grounded
            if (Input.GetButtonDown("Jump") && IsGrounded())
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }

            // attack on left click if not on cooldown
            if (Input.GetMouseButtonDown(0) && canAttack)
            {
                Attack();
            }

            // dash when left shift is pressed and dash is available
            if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
            {
                StartCoroutine(Dash());
            }
        }
    }



    // handles dealing damage to all enemy types hit by the attack
    void DealDamage()
    {
        // detect all enemies inside the attack radius
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        float damage = CalculateDamage();

        // loop through every enemy that was hit
        foreach (Collider2D enemy in hitEnemies)
        {
            // make sure the enemy is active in the scene
            if (enemy.gameObject.activeInHierarchy)
            {
                // check for small enemy type
                BossSmallEnemies smallEnemy = enemy.GetComponent<BossSmallEnemies>();
                if (smallEnemy != null)
                {
                    // choose death or damage sound based on remaining health
                    if (smallEnemy.GetCurrenthealth() <= damage)
                        SoundManager.PlaySound(SoundTypeEffects.NECROMANCER_VATS_DESTROYED);
                    else
                        SoundManager.PlaySound(SoundTypeEffects.NECROMANCER_VATS_TAKES_DAMAGE);

                    smallEnemy.TakeDamage(damage);
                   
                }

                // check for boss enemy type
                BossMonster bossEnemy = enemy.GetComponent<BossMonster>();
                if (bossEnemy != null)
                {
                    if (bossEnemy.getHealth() <= damage)
                        SoundManager.PlaySound(SoundTypeEffects.NECROMANCER_DEATH);
                    else
                        SoundManager.PlaySound(SoundTypeEffects.NECROMANCER_TAKES_DAMAGE);

                    bossEnemy.TakeDamage(damage);
                    
                }

                // check for spawned minion enemy type
                SpawnEnemies spawnEnemy = enemy.GetComponent<SpawnEnemies>();
                if (spawnEnemy != null)
                {
                    if (spawnEnemy.getHealth() <= damage)
                        SoundManager.PlaySound(SoundTypeEffects.NECROMANCER_MINION_DEATH);
                    else
                        SoundManager.PlaySound(SoundTypeEffects.NECROMANCER_MINION_TAKES_DAMAGE);

                    spawnEnemy.TakeDamage(damage);
                    
                }

                // check for skull enemy type
                isEnemy skullEnemy = enemy.GetComponent<isEnemy>();
                if (skullEnemy != null)
                {
                    if (skullEnemy.getHealth() <= damage)
                        SoundManager.PlaySound(SoundTypeEffects.NECROMANCER_SKULLS_DEATH);
                    else
                        SoundManager.PlaySound(SoundTypeEffects.NECROMANCER_SKULLS_TAKES_DAMAGE);

                    skullEnemy.TakeDamage(damage);
                    
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


    // handles player taking damage
    public void TakeDamage(float damage)
    {
        // if the player is currently invincible, ignore damage
        if (iFrame)
        {
            return;
        }

        // reduce health and update the health bar ui
        _currentHealth -= damage;
        _healthBar.SetHealth(_currentHealth);

        // check if the player has died
        if (_currentHealth <= 0)
        {
            // play death sound and mark player as dead
            SoundManager.PlaySound(SoundTypeEffects.WARRIOR_DEATH);
            isDead = true;

            // run death logic (scene change, stop movement, etc.)
            Dead();
        }
        else
        {
            // play hurt sound and start temporary invincibility frames
            SoundManager.PlaySound(SoundTypeEffects.WARRIOR_TAKES_DAMAGE);
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

        // Determine dash direction
        float moveInput = Input.GetAxisRaw("Horizontal");
        float dashDirection;

        // Use input direction if given, otherwise use facing direction (flipX)
        if (moveInput != 0)
            dashDirection = moveInput;
        else
            dashDirection = sr.flipX ? -1f : 1f;

        // Apply dash force
        rb.linearVelocity = new Vector2(dashDirection * dashingPower, 0f);

        _trailRenderer.emitting = true;
        yield return new WaitForSeconds(dashingTime);

        // End dash
        _trailRenderer.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        iFrame = false;
        _collider.enabled = true;

        // Small pause before dash can happen again
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
