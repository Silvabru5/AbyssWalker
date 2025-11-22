using UnityEngine;

/*
    Author(s): bruno silva
    Description: handles damage dealt by the bat enemy when it touches the player.
                 uses a simple cooldown system so the bat cannot damage the player every frame.
    Date (Last Modification): 11/22/2025
*/

[RequireComponent(typeof(Collider2D))]
public class BatDamage : MonoBehaviour
{
    [Header("damage settings")]
    [SerializeField] private int damageAmount = 5;          // amount of health removed from the player per hit
    [SerializeField] private float damageCooldown = 2f;      // minimum time between damage events

    private float lastHitTime = -999f;                       // tracks when the last hit happened; negative so the bat can hit immediately

    private void OnTriggerEnter2D(Collider2D other)
    {
        // check if the object we hit contains the player's main controller script
        // this avoids using tags or object names and depends only on component presence
        WarriorController2D player = other.GetComponent<WarriorController2D>();

        // if 'player' is not null, it means we collided with the player
        if (player != null)
        {
            // check if the cooldown is still active
            if (Time.time < lastHitTime + damageCooldown)
            {
                Debug.Log("[bat] hit ignored — cooldown active");
                return;
            }

            // record the time of this hit
            lastHitTime = Time.time;

            // apply damage to the player
            Debug.Log("[bat] damaged player for " + damageAmount + " health");
            player.TakeDamage(damageAmount);
        }
    }
}

