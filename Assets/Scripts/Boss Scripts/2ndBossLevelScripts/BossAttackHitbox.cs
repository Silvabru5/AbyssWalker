using UnityEngine;

public class BossAttackHitbox : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    private bool active;

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[BossHitbox] Triggered with {other.name}, Active={active}");
        if (!active) return;

        var player = other.GetComponent<PlayerSSBoss2>();
        if (player != null)
        {
            Debug.Log("[BossHitbox] Player detected – applying damage!");
            player.TakeDamage(damage);

            var blink = player.GetComponent<PlayerHitEffect>();
            if (blink != null)
                blink.TriggerBlink();
        }
    }


    // Called by animation events or boss script
    public void ActivateHitbox() => active = true;
    public void DeactivateHitbox() => active = false;
}
