using UnityEngine;

public class MageAttack : MonoBehaviour
{

    public Transform firePoint;
    public GameObject projectilePrefab;

    public float projectileSpeed = 10f; // how fast firebolt goes
    public float fireRate = 0.5f; // refresh cast time (NEED TO ADJUST TO BE SLOWER)
    private float nextFireTime = 0f; // timer to check when player can cast again (use when anim is imported)

    private AimingCursor aimingDirection; //get direction from Carey's Mouse Aim Script



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        aimingDirection = Object.FindFirstObjectByType<AimingCursor>();

    }

    // Update is called once per frame
    void Update()
    {
        //if user left clicks, and enough time passes for recast, cast
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }

    }

    void Shoot()
    {
        //Spawn firebolt after L click from player hands, projectile has no rotation
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        //Get direction from mouse aim script 
        Vector2 dir = aimingDirection.direction.normalized;

        //adding velocity
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = dir * projectileSpeed;
        }

        //rotating projectile to match direction its going
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg; //math atan gets angled fired in radians * by Rad2Deg for degree fired
        proj.transform.rotation = Quaternion.Euler(0, 0, angle); //projectile shoots out in angle appropriate for direction aimed

        Debug.Log("Fire bolt!!");
    }
}
