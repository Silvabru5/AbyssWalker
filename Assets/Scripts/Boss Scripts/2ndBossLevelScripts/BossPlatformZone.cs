using UnityEngine;

/*
    Author(s): Bruno Silva
    Description: tracks whether the player or the boss are inside a specific
                 platform zone. this is used by the dracula boss logic to
                 decide when jumping is allowed (both characters must be in 
                 the same zone for certain jump conditions to be valid).
    Date (last modification): 11/22/2025
*/

public class BossPlatformZone : MonoBehaviour
{
    public bool playerInZone;    // true while the player is inside this zone
    public bool bossInZone;      // true while the boss is inside this zone

    private void OnTriggerEnter2D(Collider2D other)
    {
        // check if the player entered this zone
        // the game uses `isHero` instead of PlayerSSBoss2 for player identification
        if (other.GetComponent<isHero>() != null)
            playerInZone = true;

        // check if the boss entered this zone
        if (other.GetComponent<BossControllerHybrid>() != null)
            bossInZone = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // player left the zone
        if (other.GetComponent<isHero>() != null)
            playerInZone = false;

        // boss left the zone
        if (other.GetComponent<BossControllerHybrid>() != null)
            bossInZone = false;
    }
}
