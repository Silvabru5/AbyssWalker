using System.Collections;
// using System.Numerics;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject basicAttackHitbox;   // the triangle hitbox for the basic attack
    public GameObject strongAttackHitbox;  // the rectangle hitbox for the strong attack

    public float attackCooldown = 1f;      // how long between attacks
    public float hitboxShowTime = 0.2f;    // how long the hitbox stays active
    public float offsetDistance = 1f;      // how far in front of the player the hitbox appears

    private bool canAttack = true;         // controls if player is allowed to attack
    private PlayerControl playerControl;   // reference to the movement script to get aim direction
    
    private Animator animator; //to use animator
    private bool isFacingRight;

    void Start()
    {
        // get a reference to the player movement script
        playerControl = GetComponent<PlayerControl>();
        animator = GetComponentInParent<Animator>();
    }

    void Update()
    {
        // don�t do anything if player cant attack or reference is missing
        if (!canAttack || playerControl == null || Time.timeScale == 0f) return;

        // left mouse button = basic attack (triangle)
        if (Input.GetMouseButtonDown(0))
        {
            //Tristan Mod
            //getting where the player last faced
            Vector2 aimDir = playerControl.LastMoveDirection;


            //using animation blend tree params to decide attack direction
            animator.SetFloat("AttackHorizontal", aimDir.x);
            animator.SetFloat("AttackVertical", aimDir.y);

            //trigger basic attack
            animator.SetTrigger("BasicAttack");





            StartCoroutine(PerformAttack(basicAttackHitbox, true));
        }
        // right mouse button = strong attack (rectangle)
        else if (Input.GetMouseButtonDown(1))
        {

            //getting where the player last faced
            Vector2 aimDir = playerControl.LastMoveDirection;

            animator.SetFloat("Horizontal", aimDir.x);
            animator.SetFloat("Vertical", aimDir.y);


            animator.SetTrigger("StrongTrigger");
            StartCoroutine(PerformAttack(strongAttackHitbox, false));
        }
    }

    IEnumerator PerformAttack(GameObject hitbox, bool rotateWithDirection)
    {
        canAttack = false;
        SoundManager.PlaySound(SoundTypeEffects.PLAYER_BARBARIAN_ATTACK, 1);

        // get the direction the player is facing
        Vector2 aimDir = playerControl.LastMoveDirection;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

        // move the hitbox in front of the player
        hitbox.transform.localPosition = aimDir * offsetDistance;

        // rotate the hitbox if needed
        if (rotateWithDirection)
        {
            // triangle faces the aim direction
            hitbox.transform.localRotation = Quaternion.Euler(0, 0, angle - 90f);
        }
        else
        {
            // rectangle only rotates if facing up/down
            if (Mathf.Abs(aimDir.y) > Mathf.Abs(aimDir.x))
            {
                hitbox.transform.localRotation = Quaternion.Euler(0, 0, 90f);
            }
            else
            {
                hitbox.transform.localRotation = Quaternion.identity;
            }
        }

        // show the hitbox
        hitbox.SetActive(true);
        yield return new WaitForSeconds(hitboxShowTime);

        // hide the hitbox after it hits
        hitbox.SetActive(false);

        // wait for cooldown to finish before next attack is allowed
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    public void PlayScreenShake()
    {
        ScreenShakeControllerTopDown.instance.StartShake(0.2f, 0.4f);
    }
}
