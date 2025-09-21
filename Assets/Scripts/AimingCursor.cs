using UnityEngine;

public class AimingCursor : MonoBehaviour
{
    public Transform player;         // Drag your player here
    public Transform cursorMarker;   // Drag the crosshair/hash object
    private const float RADIUS = 3f;       // Distance from player
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Get mouse position in world space
        Vector3 mousePos = Input.mousePosition;
        // Set the mouse Z coordinate to the same as the camera so the Z plane is constant
        mousePos.z = Mathf.Abs(Camera.main.transform.position.z - player.position.z);
        // Find the mouse position
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        // Find the direction from player to mouse as a unit vector
        Vector3 direction = (mousePos - player.position).normalized;

        // Place the marker at fixed radius (constant set above)
        cursorMarker.position = player.position + direction * RADIUS;

        // Rotate the target marker
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        cursorMarker.rotation = Quaternion.Euler(0, 0, angle);

        // Rotate player to face marker
        animator.SetFloat("Vertical", direction.y);
        animator.SetFloat("Horizontal", direction.x);

        // Rotate the player aim with the way they face

    }
}
