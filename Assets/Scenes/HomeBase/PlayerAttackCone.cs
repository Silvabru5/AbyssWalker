using System.Collections;
using UnityEngine;

public class PlayerAttackCone : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    private AimingCursor aimingCursor;

    [Header("Attack Settings")]
    public float attackCooldown = 0.8f;
    public float attackRange = 2f;       // How far the attack reaches
    public float attackAngle = 60f;      // Cone angle in degrees
    public LayerMask enemyLayers;

    [Header("Damage Stats")]
    public float baseDamage = 10f;
    public float scaleFactor = 0.15f;

    private bool canAttack = true;

    void Start()
    {
        aimingCursor = GetComponent<AimingCursor>();
        if (animator == null)
            animator = GetComponentInParent<Animator>();
    }

    void Update()
    {
        if (!canAttack || Time.timeScale == 0f) return;

        if (Input.GetMouseButtonDown(0)) // Left click
        {
            Vector2 aimDir = aimingCursor != null ? aimingCursor.direction.normalized : Vector2.right;

            animator.SetFloat("AttackHorizontal", aimDir.x);
            animator.SetFloat("AttackVertical", aimDir.y);
            animator.SetTrigger("BasicAttack");

            StartCoroutine(PerformAttack(aimDir));
        }
    }

    private IEnumerator PerformAttack(Vector2 aimDir)
    {
        canAttack = false;

        // Detect enemies within range
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hits)
        {
            if (enemy == null) continue;

            // Check if within cone angle
            Vector2 dirToEnemy = (enemy.transform.position - transform.position).normalized;
            float angleToEnemy = Vector2.Angle(aimDir, dirToEnemy);
            if (angleToEnemy <= attackAngle / 2f)
            {
                EnemyHealth enemyHealth = enemy.GetComponentInParent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    float damage = CalculateDamage();
                    enemyHealth.TakeDamage(damage);
                    Debug.Log($"Hit {enemy.name} for {damage:F1} damage");
                }
            }
        }

        // Optional: debug draw
        Debug.DrawRay(transform.position, aimDir * attackRange, Color.red, 0.5f);

        // Cooldown
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
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

    private void OnDrawGizmosSelected()
    {
        // Draw attack range circle
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Draw cone lines
        if (aimingCursor != null)
        {
            Vector2 dir = aimingCursor.direction.normalized;
            Quaternion leftRot = Quaternion.Euler(0, 0, -attackAngle / 2f);
            Quaternion rightRot = Quaternion.Euler(0, 0, attackAngle / 2f);

            Vector2 leftDir = leftRot * dir;
            Vector2 rightDir = rightRot * dir;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + leftDir * attackRange);
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + rightDir * attackRange);
        }
    }
}
