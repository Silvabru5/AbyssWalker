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
    [SerializeField] private GameObject player;
    [SerializeField] private LayerMask Player;
    [SerializeField] private LayerMask _enemyLayers;
    private GameObject boss;

    [SerializeField] private float moveSpeed = 0f;
    private bool canChase = false;
    private bool m_FacingRight = false;
    void Start()
    {
        _anim = GetComponent<Animator>();
        _currentHealth = _maxHealth;
        _anim.SetFloat("CurrentHP", 100);
        _col = GetComponent<Collider2D>();
        _anim.SetTrigger("Death");
        _col.enabled = false;
        boss = GameObject.Find("Boss");
        player = GameObject.FindWithTag("Player");
        isDead= true;   
        StartCoroutine(RespawnEnemy());
    }

    public float getHealth()
    {
        return _currentHealth;
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
        moveSpeed = 0;
    }

    void DealDamage()
    {
        Collider2D playerCol = Physics2D.OverlapCircle(attackPtn.position, attackRange, Player);
        if (playerCol != null)
        {
            // PlayerSS player = playerCol.GetComponent<PlayerSS>();
            WarriorController2D player = playerCol.GetComponent<WarriorController2D>();
            player.TakeDamage(attackDmg);
        }
    }

    void Die()
    {
        isDead = true;
        moveSpeed = 0f;
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
            else
            {
                moveSpeed = 2f;
            }
        }
        if(boss.GetComponent<BossMonster>().getDead() == true)
        {
            Destroy(this.gameObject);
        }

        if (!isDead && canChase && player != null)
        {
            if (!IsTooCloseToOtherEnemy())
            {
                ChasePlayer();
            }
        }
    }

    private IEnumerator RespawnEnemy()
    {
        yield return new WaitForSeconds(8f);
        isDead = false;
        canChase = true;
        _anim.SetTrigger("Recover");
        yield return new WaitForSeconds(2f);
        moveSpeed = 2f;
        _col.enabled = true;
        _currentHealth = _maxHealth;

    }
    private void ChasePlayer()
    {
        if (player != null)
        {
            _anim.SetFloat("Run", moveSpeed);
            if ((player.transform.position.x > transform.position.x && !m_FacingRight) ||
             (player.transform.position.x < transform.position.x && m_FacingRight))
            {
                Flip();
            }
            Vector2 targetPosition = new Vector2(player.transform.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }
    private void OnDrawGizmos()
    {
        if (attackPtn == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackPtn.position, attackRange);
    }
    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private bool IsTooCloseToOtherEnemy()
    {
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, 0.8f, _enemyLayers);
        return nearbyEnemies.Length > 1; // If more than one (itself), it's too close
    }
}
