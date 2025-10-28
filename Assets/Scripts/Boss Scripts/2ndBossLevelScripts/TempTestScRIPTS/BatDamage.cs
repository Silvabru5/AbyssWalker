using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BatDamage : MonoBehaviour
{
    [SerializeField] private int damageAmount = 5;
    [SerializeField] private float damageCooldown = 2f;
    private float lastHitTime = -999f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        

        // Damage the player if they have the PlayerSSBoss2 component
        PlayerSSBoss2 player = other.GetComponent<PlayerSSBoss2>();
        if (player != null)
        {


            if (Time.time < lastHitTime + damageCooldown)
            {
                Debug.Log("[Bat] Skipping hit — cooldown active.");
                return;
            }

            lastHitTime = Time.time;
            Debug.Log("[Bat] Hit player — dealing " + damageAmount + " damage.");
            player.TakeDamage(damageAmount);
            return;
        }

    }
}
