using System.Collections;
using UnityEngine;

public class PlayerMovementSS : MonoBehaviour
{
    //Character Controller variables
    [SerializeField] private CharacterController2D controller;
    [SerializeField] private float _horizontalMove = 0;
    [SerializeField] private float _runSpeed = 0;
    [SerializeField] private Animator _anim;
    bool jump = false;

    //Attack related variables
    [SerializeField] private Transform _attkPnt;
    [SerializeField] private float _attackRange;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private float _attackDamage;
    [SerializeField] private LayerMask _enemyLayers;
    float nextAttkTime = 0f;

    //Dashing variables
    [SerializeField] private TrailRenderer _trailRenderer;
    [SerializeField] private float dashingPower = 10f;
    private bool canDash = true;
    private bool isDashing;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;



    private void Update()
    {
        if (isDashing)
        {
            return;
        }
        _horizontalMove = Input.GetAxisRaw("Horizontal") * _runSpeed;
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
            _anim.SetTrigger("Jump");
        }
        if (Time.time >= nextAttkTime)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Attack();
                nextAttkTime = Time.time + _attackSpeed;
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            _anim.SetTrigger("Dash");
            StartCoroutine(Dash());
        }
    }

    void Attack()
    {
        _anim.SetTrigger("Attack");
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(_attkPnt.position, _attackRange, _enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<BossSmallEnemies>().TakeDamage(_attackDamage);
            Debug.Log("Hit" + enemy.name);
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
        PlayerInput();
    }

    private void OnDrawGizmos()
    {
        if(_attkPnt == null)
        {
            return; 
        }
        Gizmos.DrawWireSphere(_attkPnt.position, _attackRange);
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = controller.rb.gravityScale;
        controller.rb.gravityScale = 0;
        controller.rb.linearVelocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        _trailRenderer.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        _trailRenderer.emitting = false;
        controller.rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;

    }
}
