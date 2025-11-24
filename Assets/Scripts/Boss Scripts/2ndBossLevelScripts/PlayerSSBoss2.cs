using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/*
    Author(s): Bruno Silva
    Description: controls player movement, jumping, attacking, dashing, health,
                 damage handling, and basic interactions during the dracula boss
                 fight. this script is tied to the boss version of the warrior
                 and works with CharacterController2DBoss and animation states.
    Date (last modification): 11/22/2025
*/

public class PlayerSSBoss2 : MonoBehaviour
{
    [Header("movement settings")]
    [SerializeField] private CharacterController2DBoss controller;
    // reference to the custom character controller that handles grounded checks, slopes, and physics-friendly movement

    [SerializeField] private float _runSpeed = 10f;
    // base horizontal movement speed when the player is running

    private float _horizontalMove = 0f;
    // current horizontal input value (left or right), updated every frame

    private bool jump = false;
    // flag that tells the controller to perform a jump during the next fixed update

    [Header("animation and physics")]
    [SerializeField] private Animator _anim;
    // animator that drives the player animations such as idle, run, jump, attack, hurt, and death

    private Collider2D _collider;
    // main collider for hit detection and environment interaction

    private Rigidbody2D _rb;
    // rigidbody used for physics movement and velocity checks (especially for slope fix)

    private SpriteRenderer _spriteRenderer;
    // handles visibility and color of the player sprite (used for blinking during i-frames)

    [Header("attack settings")]
    [SerializeField] private Transform _attkPnt;
    // transform that represents the center point of the melee attack area

    [SerializeField] private float _attackRange = 0.75f;
    // radius of the circular area used to detect hit enemies during an attack

    [SerializeField] private float _attackSpeed = 2f;
    // number of attacks allowed per second (used to calculate attack cooldown)

    [SerializeField] private float _attackDamage = 30f;
    // amount of damage dealt to boss enemies when an attack connects

    [SerializeField] private LayerMask _enemyLayers;
    // layer mask used to filter which colliders are treated as valid attack targets

    private bool isAttacking = false;
    // tracks if the player is currently in an attack state

    private float nextAttkTime = 0f;
    // time at which the next attack is allowed, based on attack speed

    [Header("dash settings")]
    [SerializeField] private TrailRenderer _trailRenderer;
    // trail renderer used to visually show the dash effect

    [SerializeField] private float dashingPower = 10f;
    // strength of the dash force applied horizontally

    private bool canDash = true;
    // indicates whether the player is currently allowed to start a dash

    private bool isDashing;
    // indicates whether the player is currently dashing

    private float dashingTime = 0.2f;
    // duration of the active dash phase

    private float dashingCooldown = 1f;
    // time to wait after a dash before another dash can be triggered

    [Header("health settings")]
    [SerializeField] private float _maxHealth = 100f;
    // maximum player health for this fight

    [SerializeField] private float _currentHealth = 0f;
    // current health value that goes down when taking damage

    [SerializeField] private float iFrameDuration = 1.5f;
    // total length of the invulnerability period after taking damage

    [SerializeField] private float blinkDuration = 0.1f;
    // how quickly the sprite toggles visibility during invulnerability

    private bool isDead = false;
    // true once the player has died and gameplay should no longer accept input

    private bool iFrame = false;
    // true while the player is invulnerable to damage

    [Header("ui and game references")]
    private GameObject _gameManager;
    // reference to the ui or game manager used to trigger the game over screen

    private void Start()
    {
        // cache core components required for movement, animation, and visuals
        _anim = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        _rb = GetComponent<Rigidbody2D>();

        // set current health to full at the start of the fight
        _currentHealth = _maxHealth;

        // find the UI manager object that shows the game over screen
        _gameManager = GameObject.Find("UIManager");

        // play a small recovery animation at the start and delay control
        StartCoroutine(StartTimer());
    }

    private void Update()
    {
        // ignore player input when dead or during dash, so states cannot be interrupted
        if (isDead || isDashing)
            return;

        // read raw horizontal input (A/D or left/right arrows) and scale by run speed
        _horizontalMove = Input.GetAxisRaw("Horizontal") * _runSpeed;

        // handle jump input only if not currently attacking, to avoid jump-canceling attacks
        if (Input.GetButtonDown("Jump") && !isAttacking)
        {
            jump = true;
            _anim.SetTrigger("Jump");
        }

        // handle normal attack input, limited by attack speed cooldown and requiring grounded state
        if (Time.time >= nextAttkTime)
        {
            if (Input.GetButtonDown("Fire1") && controller.m_Grounded)
            {
                SoundManager.PlaySound(SoundTypeEffects.WARRIOR_ATTACK);
                jump = false;               // prevent jump from mixing with the attack
                Attack();
                nextAttkTime = Time.time + 1f / _attackSpeed;
                isAttacking = false;        // reset attack flag after scheduling attack
            }
        }

        // handle dash input; dash is only allowed when not on cooldown
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            SoundManager.PlaySound(SoundTypeEffects.WARRIOR_DASH_PORTAL);
            _anim.SetTrigger("Dash");
            StartCoroutine(Dash());
        }
    }

    private void FixedUpdate()
    {
        // skip applying movement if the player is dead, attacking, or in the middle of a dash
        if (isDead || isAttacking || isDashing)
            return;

        // process movement based on current input
        PlayerInput();

        // apply a small correction to reduce sliding on slopes when idle
        HandleSlopeSliding();
    }

    private void PlayerInput()
    {
        // update animator with movement state: running if moving, idle if not
        if (_horizontalMove != 0)
            _anim.SetInteger("AnimState", 2); // running animation
        else
            _anim.SetInteger("AnimState", 0); // idle animation

        // scale horizontal movement by fixed delta time since controller uses physics movement
        float moveInput = _horizontalMove * Time.fixedDeltaTime;

        // slightly boost movement when grounded to improve control responsiveness
        if (controller.m_Grounded)
            moveInput *= 1.3f;

        // send movement and jump flags to the custom controller
        controller.Move(moveInput, false, jump);

        // reset jump so we do not keep jumping every frame
        jump = false;
    }

    private void HandleSlopeSliding()
    {
        // if grounded and horizontal velocity is very small, lock x velocity to zero
        // this helps avoid slowly sliding down slopes when not pressing any movement keys
        if (controller.m_Grounded && Mathf.Abs(_rb.linearVelocity.x) < 0.05f)
            _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y);
    }

    private void Attack()
    {
        // trigger attack animation and mark player as currently attacking
        _anim.SetTrigger("Attack");
        isAttacking = true;

        // temporarily set run speed to zero so the character does not slide during the attack
        _runSpeed = 0f;

        // after a short delay, restore run speed and allow movement again
        StartCoroutine(AttackCD());
    }

    // called by animation event or timed logic to apply damage to enemies in range
    private void DealDamage()
    {
        // get all enemy colliders within attack range from the attack point
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            _attkPnt.position,
            _attackRange,
            _enemyLayers
        );

        foreach (Collider2D enemy in hitEnemies)
        {
            // ignore any enemy objects that are inactive
            if (!enemy.gameObject.activeInHierarchy)
                continue;

            // try to find a standard boss enemy on the object or its parent
            var bossA = enemy.GetComponent<BossMonster>() ?? enemy.GetComponentInParent<BossMonster>();

            // try to find the dracula boss on the object or its parent
            var bossB = enemy.GetComponent<BossMonsterDracula>() ?? enemy.GetComponentInParent<BossMonsterDracula>();

            // handle normal boss damage
            if (bossA != null)
            {
                bossA.TakeDamage(_attackDamage);
                SoundManager.PlaySound(SoundTypeEffects.VAMPIRE_TAKES_DAMAGE);
            }
            // handle dracula-specific boss damage and audio
            else if (bossB != null)
            {
                bossB.TakeDamage(_attackDamage);

                // pick death or hurt sound depending on remaining health
                if (bossB.getHealth() <= _attackDamage)
                    SoundManager.PlaySound(SoundTypeEffects.VAMPIRE_DEATH);
                else
                    SoundManager.PlaySound(SoundTypeEffects.VAMPIRE_TAKES_DAMAGE);
            }
        }
    }

    private void Dead()
    {
        // trigger death animation and sound, and stop all player control
        _anim.SetTrigger("Death");
        SoundManager.PlaySound(SoundTypeEffects.WARRIOR_DEATH);
        _runSpeed = 0f;

        // stop character movement immediately
        controller.rb.linearVelocity = Vector2.zero;

        // display the game over screen through the ui manager
        _gameManager.GetComponent<GameOverManager>().ShowGameOver();

        // prevent any further dashing
        canDash = false;
    }

    public void TakeDamage(float damage)
    {
        // prevent taking damage while in invulnerability state
        if (iFrame)
            return;

        // reduce current health by incoming damage
        _currentHealth -= damage;

        // play hurt animation to show feedback
        _anim.SetTrigger("Hurt");

        // if health is depleted, run death logic
        if (_currentHealth <= 0)
        {
            SoundManager.PlaySound(SoundTypeEffects.WARRIOR_DEATH);
            isDead = true;
            Dead();
        }
        else
        {
            // still alive, play damage sound and start invulnerability window
            SoundManager.PlaySound(SoundTypeEffects.WARRIOR_TAKES_DAMAGE);
            StartCoroutine(IFrames());
        }
    }

    private IEnumerator Dash()
    {
        // begin dash: lock state, enable invulnerability, and disable regular collision
        canDash = false;
        isDashing = true;
        isAttacking = true;
        iFrame = true;

        _collider.enabled = false;

        // temporarily remove gravity so the dash is purely horizontal
        float originalGravity = controller.rb.gravityScale;
        controller.rb.gravityScale = 0;

        // apply a horizontal velocity in the direction the player is facing
        controller.rb.linearVelocity = new Vector2(transform.localScale.x * dashingPower, 0f);

        // turn on the dash trail visual and play sound
        _trailRenderer.emitting = true;
        SoundManager.PlaySound(SoundTypeEffects.WARRIOR_DASH_PORTAL);

        // wait for the dash duration to complete
        yield return new WaitForSeconds(dashingTime);

        // turn off trail and restore original physics settings
        _trailRenderer.emitting = false;
        controller.rb.gravityScale = originalGravity;

        // leave dash state and restore attack and invulnerability flags
        isDashing = false;
        isAttacking = false;
        iFrame = false;

        // re-enable collider now that dash is done
        _collider.enabled = true;

        // wait for cooldown before dash can be used again
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    private IEnumerator AttackCD()
    {
        // short delay to keep player locked in attack animation
        yield return new WaitForSeconds(0.67f);

        // restore run speed and allow movement and actions again
        _runSpeed = 10f;
        isAttacking = false;
    }

    private IEnumerator IFrames()
    {
        // start invulnerability and blink the sprite to indicate the state
        iFrame = true;
        float elapsedTime = 0f;

        while (elapsedTime < iFrameDuration)
        {
            _spriteRenderer.enabled = !_spriteRenderer.enabled;
            yield return new WaitForSeconds(blinkDuration);
            elapsedTime += blinkDuration;
        }

        // make sure sprite is visible when invulnerability ends
        _spriteRenderer.enabled = true;
        iFrame = false;
    }

    private IEnumerator StartTimer()
    {
        // at the beginning of the scene, briefly lock the player in a recover animation
        // this gives a small intro before the player can move
        isDead = true;
        _anim.SetTrigger("Recover");
        yield return new WaitForSeconds(1f);
        isDead = false;
    }

    private void OnDrawGizmos()
    {
        // draw a wire sphere in the editor to visualize attack range around the attack point
        if (_attkPnt == null)
            return;

        Gizmos.DrawWireSphere(_attkPnt.position, _attackRange);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // handle collision with enemy projectiles or bodies that implement isEnemy
        if (collision.gameObject.GetComponent<isEnemy>())
        {
            // only apply damage if not currently invulnerable
            if (!iFrame)
            {
                TakeDamage(10f);
                Destroy(collision.gameObject);
            }
        }

        // handle collision with fire hazards that implement isFire
        if (collision.gameObject.GetComponent<isFire>())
        {
            TakeDamage(15f);
        }
    }
}
