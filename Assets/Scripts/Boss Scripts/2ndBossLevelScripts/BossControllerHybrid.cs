using UnityEngine;
using System.Collections;

public class BossControllerHybrid : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float chaseRange = 8f;
    public float attackRange = 2f;

    [Header("Attack Settings")]
    public float meleeCooldown = 2f;
    public float rangedCooldown = 4f;
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public GameObject meleeHitbox;

    [Header("Phase 2 Settings")]
    public bool phase2Unlocked = false;
    public float phase2SpeedMultiplier = 1.5f;
    public float phase2AttackMultiplier = 1.5f;

    private Transform player;
    private Rigidbody2D rb;
    private Animator anim;
    private bool facingRight = true;
    private float lastMeleeTime;
    private float lastRangedTime;
    private bool isAttacking;

    void Start()
    {
        //player = GameObject.FindObjectOfType<CharacterController2D>().transform;
        player = GameObject.FindFirstObjectByType<PlayerSS>().transform;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        meleeHitbox.SetActive(false);
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > chaseRange)
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetBool("isMoving", false);
            return;
        }

        // Flip boss toward player
        if ((player.position.x > transform.position.x && !facingRight) ||
            (player.position.x < transform.position.x && facingRight))
        {
            Flip();
        }

        // Phase 2 logic placeholder (you can later trigger this by HP)
        float currentMoveSpeed = phase2Unlocked ? moveSpeed * phase2SpeedMultiplier : moveSpeed;

        if (!isAttacking)
        {
            if (distance > attackRange)
            {
                MoveTowardPlayer(currentMoveSpeed);
            }
            else
            {
                TryAttack(distance);
            }
        }
    }

    void MoveTowardPlayer(float speed)
    {
        anim.SetBool("isMoving", true);
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y);
    }

    void TryAttack(float distance)
    {
        rb.linearVelocity = Vector2.zero;
        anim.SetBool("isMoving", false);

        // Alternate between melee and ranged if available
        if (Time.time > lastMeleeTime + meleeCooldown)
        {
            StartCoroutine(DoMeleeAttack());
            lastMeleeTime = Time.time;
        }
        else if (projectilePrefab != null && Time.time > lastRangedTime + rangedCooldown)
        {
            StartCoroutine(DoRangedAttack());
            lastRangedTime = Time.time;
        }
    }

    IEnumerator DoMeleeAttack()
    {
        isAttacking = true;
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(0.3f); // small delay before hit
        meleeHitbox.SetActive(true);
        yield return new WaitForSeconds(0.4f); // duration active
        meleeHitbox.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        isAttacking = false;
    }

    IEnumerator DoRangedAttack()
    {
        isAttacking = true;
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(0.5f);

        // Fire projectile
        if (shootPoint != null && projectilePrefab != null)
        {
            GameObject proj = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
            Vector2 dir = (player.position - shootPoint.position).normalized;
            proj.GetComponent<Rigidbody2D>().linearVelocity = dir * 5f;
        }

        yield return new WaitForSeconds(0.3f);
        isAttacking = false;
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 s = transform.localScale;
        s.x *= -1;
        transform.localScale = s;
    }

    // For animation events (optional)
    public void EnableHitbox() => meleeHitbox.SetActive(true);
    public void DisableHitbox() => meleeHitbox.SetActive(false);
}
