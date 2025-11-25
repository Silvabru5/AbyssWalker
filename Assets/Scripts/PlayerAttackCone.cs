using System.Collections;
using UnityEngine;

/*
author: Tristan ung
file: PlayerAttackCone.cs
description:
This script handles player attack mechanics using a cone-shaped area in front of the player | FOR WARRIOR CLASS

*/ 
public class PlayerAttackCone : MonoBehaviour
{
    [Header("References")]
    public Animator animator; // Reference to the player's animator
    private AimingCursor aimingCursor; // Reference to aiming cursor script

    [Header("Attack Settings")]
    public float attackCooldown = 0.8f; // Time between attacks
    public float attackRange = 2f;       // How far the attack reaches
    public float attackAngle = 60f;      // Cone angle in degrees
    public LayerMask enemyLayers;      // Layers considered as enemies

    [Header("Damage Stats")]
    public float baseDamage = 10f; // Base damage of the attack
    public float scaleFactor = 0.15f; // Damage scaling per player level

    private bool canAttack = true; // Track if player can attack & prevent spamming left click
    private bool isCrit = false; // Track if the current attack is a critical hit

    void Start()
    {
        // Get reference to aiming cursor
        aimingCursor = GetComponent<AimingCursor>();

        // Get reference to animator if not set
        if (animator == null)
            animator = GetComponentInParent<Animator>();
    }

    void Update()
    {

        // Check for attack input, cant attack while on cool down or game is paused.
        if (!canAttack || Time.timeScale == 0f) return;

        if (Input.GetMouseButtonDown(0)) // Left click
        {
            // Determine aim direction
            Vector2 aimDir = aimingCursor != null ? aimingCursor.direction.normalized : Vector2.right;

            // Trigger attack animation based on aim direction
            animator.SetFloat("AttackHorizontal", aimDir.x);
            animator.SetFloat("AttackVertical", aimDir.y);
            animator.SetTrigger("BasicAttack");

            // Start attack coroutine
            StartCoroutine(PerformAttack(aimDir));
        }
    }

    private IEnumerator PerformAttack(Vector2 aimDir)
    {
        canAttack = false; // Disable further attacks until cooldown is over

        // Detect enemies within range
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayers);

        //Calculate damage delt
        int level = ExperienceManager.instance != null ? ExperienceManager.instance.GetCurrentLevel() : 1;
        float damage = (baseDamage * (1f + scaleFactor * level)) * StatManager.instance.GetDamageIncrease();
        isCrit = Random.value < StatManager.instance.GetCritChance();
        // Crit check
        if (isCrit)
        {
            damage *= StatManager.instance.GetCritDamage();
        }

        // Apply damage to enemies within cone
        foreach (Collider2D enemy in hits)
        {
            if (enemy == null) continue;

            // Check if within cone angle
            Vector2 dirToEnemy = (enemy.transform.position - transform.position).normalized;
            float angleToEnemy = Vector2.Angle(aimDir, dirToEnemy);
            if (angleToEnemy <= attackAngle / 2f)
            {
                // Enemy is within cone, apply damage
                enemy.GetComponent<EnemyHealth>().TakeDamage(damage);
                
                if (isCrit) // Spawn critical hit damage number
                {
                    DamageNumberSpawner.instance.SpawnCritNumber(enemy.transform.position, damage);
                }
                else // Spawn regular damage number
                {
                    DamageNumberSpawner.instance.SpawnDamageNumber(enemy.transform.position, damage);
                }

            }
        }

        // Draw red debug ray for attack direction
        Debug.DrawRay(transform.position, aimDir * attackRange, Color.red, 0.5f);

        // Cooldown
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    //used for debugging attack cone in unity
    private void OnDrawGizmosSelected()
    {
        // Draw attack range circle
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Draw cone lines
        if (aimingCursor != null)
        {
            Vector2 dir = aimingCursor.direction.normalized;

            // Calculate left and right directions of the cone
            Quaternion leftRot = Quaternion.Euler(0, 0, -attackAngle / 2f);
            Quaternion rightRot = Quaternion.Euler(0, 0, attackAngle / 2f);

            Vector2 leftDir = leftRot * dir;
            Vector2 rightDir = rightRot * dir;

            Gizmos.color = Color.red;

            // Draw cone lines
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + leftDir * attackRange);
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + rightDir * attackRange);
        }
    }
}
