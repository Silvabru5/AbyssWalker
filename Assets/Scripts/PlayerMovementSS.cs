using UnityEngine;

public class PlayerMovementSS : MonoBehaviour
{
    [SerializeField] private CharacterController2D controller;
    [SerializeField] private float _horizontalMove = 0;
    [SerializeField] private float _runSpeed = 0;
    [SerializeField] private Animator _anim;
    [SerializeField] private Transform _attkPnt;
    [SerializeField] private float _attackRange;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private float _attackDamage;
    [SerializeField] private LayerMask _enemyLayers;
    bool jump = false;


    private void Update()
    {
        _horizontalMove = Input.GetAxisRaw("Horizontal") * _runSpeed;
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
            _anim.SetTrigger("Jump");
        }

        if(Input.GetButtonDown("Fire1"))
        {
            Attack();
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
}
