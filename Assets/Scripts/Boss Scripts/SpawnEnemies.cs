using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.XR;
/*
 * Author: Adrian Agius
 * File: SpawnEnemies.cs
 * Description: this script creates a reanimated enemy. It respawns enemies after they have been killed, meant for boss use only
 * can be implemented else where.
 * 
 */
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
        //Grab all components of prefab
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

    public float getHealth() // get enemy health
    {
        return _currentHealth;
    }
    public void TakeDamage(float damage) // function to damage this enemy
    {
        _currentHealth -= damage;
        _anim.SetTrigger("Hurt");
        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    void Attack() // set the animations
    {
        _anim.SetTrigger("Attack");
        moveSpeed = 0;
    }

    void DealDamage() // deal damage to player
    {
        Collider2D playerCol = Physics2D.OverlapCircle(attackPtn.position, attackRange, Player);
        if (playerCol != null)
        {
            // PlayerSS player = playerCol.GetComponent<PlayerSS>();
            WarriorController2D player = playerCol.GetComponent<WarriorController2D>();
            player.TakeDamage(attackDmg);
        }
    }

    void Die() // function for death to play the animation and start the respawn effect
    {
        isDead = true;
        moveSpeed = 0f;
        _anim.SetTrigger("Death");
        _col.enabled = false;
        StartCoroutine(RespawnEnemy());

    }

    // Update is called once per frame
    void Update()
    {
        _anim.SetFloat("CurrentHP", (_currentHealth / _maxHealth) * 100); //HP for enemy

        if(Time.time >= attackTime && !isDead)
        {
            Collider2D playerCol = Physics2D.OverlapCircle(attackPtn.position, attackRange, Player); // check for overlap and if player is in range
            if (playerCol != null)
            {
                Attack();
                attackTime = Time.time + 1f / attackSpeed; // attack time is how often they can attack
            }
            else
            {
                moveSpeed = 2f;
            }
        }
        if(boss.GetComponent<BossMonster>().getDead() == true) // despawn game object if the bass is dead
        {
            Destroy(this.gameObject);
        }

        if (!isDead && canChase && player != null) // player collision/overlap with other enemies
        {
            if (!IsTooCloseToOtherEnemy())
            {
                ChasePlayer();
            }
        }
    }

    private IEnumerator RespawnEnemy() // starting the delay to respawn the enemy
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
    private void ChasePlayer() // function to start the movement towards the player, updating the animations as well
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
    private void Flip() // Flip character based on direction facing
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private bool IsTooCloseToOtherEnemy() // check for overlap with other enemies
    {
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, 0.8f, _enemyLayers);
        return nearbyEnemies.Length > 1; // If more than one (itself), it's too close
    }
}
