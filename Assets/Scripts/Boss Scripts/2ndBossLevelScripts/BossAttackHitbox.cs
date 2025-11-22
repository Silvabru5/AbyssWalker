using UnityEngine;

/*
    Author(s): Bruno Silva
    Description: controls the boss attack hitbox. this hitbox is turned on and off by animation
                 events or the boss controller, and deals damage to the player when active.
                 relies only on component checks instead of tags or object names.
    Date (Last Modification): 11/22/2025
*/

public class BossAttackHitbox : MonoBehaviour
{
    [Header("damage settings")]
    [SerializeField] private int damage = 10;     // how much damage this attack deals to the player

    private bool active = false;                  // determines if the hitbox is currently allowed to deal damage

    private void OnTriggerStay2D(Collider2D other)
    {
        // if the hitbox isn't active, do nothing
        if (!active) return;

        // check if the collider belongs to the player by looking for their main controller script
        WarriorController2D player = other.GetComponent<WarriorController2D>();

        // if the collider has the player controller, apply damage
        if (player != null)
        {
            player.TakeDamage(damage);
        }
    }

    // called by animation events or the boss script to enable the hitbox
    public void ActivateHitbox() => active = true;

    // called by animation events or the boss script to disable the hitbox
    public void DeactivateHitbox() => active = false;
}
