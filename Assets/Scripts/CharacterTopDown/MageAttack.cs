using UnityEngine;

public class MageAttack : MonoBehaviour
{

    public Transform firePoint;
    public GameObject projectilePrefab;

    public float projectileSpeed = 10f;
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;

    private AimingCursor aimingDirection; //get direction from Carey's Mouse Aim Script



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        aimingDirection = Object.FindFirstObjectByType<AimingCursor>();

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }

    }

    void Shoot()
    {
        //Spawn firebolt after L click
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
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        proj.transform.rotation = Quaternion.Euler(0, 0, angle);

        Debug.Log("Fire bolt!!");
    }
}
