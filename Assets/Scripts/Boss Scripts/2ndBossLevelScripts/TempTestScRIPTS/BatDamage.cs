using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BatDamage : MonoBehaviour
{
    [SerializeField] private int damageAmount = 5;
    [SerializeField] private float damageCooldown = 2f;
    private float lastHitTime = -999f;

    private GameObject playerobj;

    private void Start()
    {
        playerobj = GameObject.FindWithTag("Player");
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        

        // Damage the player if they have the PlayerSSBoss2 component
        WarriorController2D player = other.GetComponent<WarriorController2D>();
        // var player = playerobj.GetComponent<WarriorController2D>();
        if (player != null)
        {


            if (Time.time < lastHitTime + damageCooldown)
            {
                Debug.Log("[Bat] Skipping hit � cooldown active.");
                return;
            }

            lastHitTime = Time.time;
            Debug.Log("[Bat] Hit player � dealing " + damageAmount + " damage.");
            player.TakeDamage(damageAmount);
            return;
        }

    }
}
