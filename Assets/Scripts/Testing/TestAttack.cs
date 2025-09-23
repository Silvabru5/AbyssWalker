using System.Collections;
using UnityEngine;

public class TestAttack : MonoBehaviour
{
    [SerializeField] private Animator _anim;
    private Collider2D _collider;
    //Attack related variables
    [SerializeField] private Transform _attkPnt;
    [SerializeField] private float _attackRange;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private int _attackDamage;
    [SerializeField] private LayerMask _enemyLayers;
    private bool isAttacking = false;
    float nextAttkTime = 0f;

    //Player Level Variables
    [SerializeField] private int currentLevel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextAttkTime)
        {
            if (Input.GetButtonDown("Fire1"))
            { 
                Attack();
                nextAttkTime = Time.time + 1f / _attackSpeed;
                isAttacking = false;
            }
        }
    }

    void DealDamage()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(_attkPnt.position, _attackRange, _enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            if(enemy.GetComponent<isSpider>())
            {
                enemy.GetComponent<EnemyHealth>().TakeDamage(_attackDamage);
                    
            }
            if (enemy.GetComponent<isSkeleton>())
            {
                enemy.GetComponent<EnemyHealth>().TakeDamage(_attackDamage);
                
            }
            if (enemy.GetComponent<isZombie>())
            {
                enemy.GetComponent<EnemyHealth>().TakeDamage(_attackDamage);
            }
        }

    }            
    void Attack()
    {
        _anim.SetTrigger("Attack");
        isAttacking = true;
        DealDamage();
        StartCoroutine(AttackCD());
    }
    private IEnumerator AttackCD()
    {
        yield return new WaitForSeconds(0.67f);
        isAttacking = false;
    }
    private void OnDrawGizmos()
    {
        if (_attkPnt == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(_attkPnt.position, _attackRange);
    }
}
