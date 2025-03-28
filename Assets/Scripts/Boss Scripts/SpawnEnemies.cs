using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class SpawnEnemies : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _currentHealth;
    [SerializeField] private Animator _anim;
    private Collider2D _col;
    public bool isDead = false;
    public Transform attackPtn;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float attackDmg;
    private float attackTime;
    [SerializeField] private LayerMask Player;
    void Start()
    {
        _anim = GetComponent<Animator>();
        _currentHealth = _maxHealth;
        _anim.SetFloat("CurrentHP", 100);
        _col = GetComponent<Collider2D>();
        _anim.SetTrigger("Death");
        _col.enabled = false;
        isDead= true;   
        StartCoroutine(RespawnEnemy());
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        _anim.SetTrigger("Hurt");
        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    void Attack()
    {
        _anim.SetTrigger("Attack");
  
    }

    void DealDamage()
    {
        Collider2D playerCol = Physics2D.OverlapCircle(attackPtn.position, attackRange, Player);
        if (playerCol != null)
        {
            PlayerSS player = playerCol.GetComponent<PlayerSS>();
            player.TakeDamage(attackDmg);
        }
    }

    void Die()
    {
        isDead = true;
        _anim.SetTrigger("Death");
        _col.enabled = false;
        Debug.Log("Enemy Died");
        StartCoroutine(RespawnEnemy());

    }

    // Update is called once per frame
    void Update()
    {
        _anim.SetFloat("CurrentHP", (_currentHealth / _maxHealth) * 100);

        if(Time.time >= attackTime && !isDead)
        {
            Collider2D playerCol = Physics2D.OverlapCircle(attackPtn.position, attackRange, Player);
            if (playerCol != null)
            {
                Attack();
                attackTime = Time.time + 1f / attackSpeed;
            }
        }
    }

    private IEnumerator RespawnEnemy()
    {
        yield return new WaitForSeconds(8f);
        isDead = false;
        _anim.SetTrigger("Recover");
        _col.enabled = true;
        _currentHealth = _maxHealth;
    }

    private void OnDrawGizmos()
    {
        if (attackPtn == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackPtn.position, attackRange);
    }
}
