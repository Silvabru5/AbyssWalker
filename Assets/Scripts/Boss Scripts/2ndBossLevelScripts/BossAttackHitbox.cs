using UnityEngine;

public class BossAttackHitbox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerHitEffect player = collision.GetComponent<PlayerHitEffect>();
        if (player != null)
        {
            player.TriggerBlink();
        }
    }
}
