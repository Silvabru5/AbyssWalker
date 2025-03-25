using UnityEngine;
using System.Collections.Generic;

public class AttackHitbox : MonoBehaviour
{
    public int damage = 10;
    private HashSet<EnemyHealth> enemiesHit = new HashSet<EnemyHealth>();

    void OnEnable()
    {
        enemiesHit.Clear(); // Reset hit memory on attack activation
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        if (enemy != null && !enemiesHit.Contains(enemy))
        {
            enemy.TakeDamage(damage);
            enemiesHit.Add(enemy); // Prevent re-hitting this enemy
        }
    }
}
