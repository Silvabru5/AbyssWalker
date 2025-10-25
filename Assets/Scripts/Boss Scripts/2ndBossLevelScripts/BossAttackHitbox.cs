using UnityEngine;

public class BossAttackHitbox : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    private bool active;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!active) return;

        var player = other.GetComponent<PlayerSSBoss2>();
        if (player != null)
        {
            player.TakeDamage(damage);

            // Optional blink / flash if you have PlayerHitEffect attached
            var blink = player.GetComponent<PlayerHitEffect>();
            if (blink != null)
                blink.TriggerBlink();
        }
    }

    // Called by animation events or boss script
    public void ActivateHitbox() => active = true;
    public void DeactivateHitbox() => active = false;
}
