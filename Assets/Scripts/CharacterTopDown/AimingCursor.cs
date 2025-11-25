using UnityEngine;

// filename:    AimingCursor.ca
// author:      Carey Cunningham
// description: make a player target cursor in the direction of the mouse then aim the player and attack towards the mouse

public class AimingCursor : MonoBehaviour
{
    [SerializeField] private Transform player;         // Drag your player here
    [SerializeField] private GameObject cursorMarker;   // Drag the crosshair/hash object
    private const float RADIUS = 3f;       // Distance from player
    private Animator animator;

    public Vector3 direction; // direction between player and aiming target, also used in PlayerAttack

    private void Start()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<Transform>();
        cursorMarker = GameObject.Find("Target");
        Cursor.visible = false; 
        Cursor.lockState = CursorLockMode.Confined;
    }        

    void FixedUpdate()
    {
        // Get mouse position in world space
        Vector3 mousePos = Input.mousePosition;
        // Set the mouse Z coordinate to the same as the camera so the Z plane is constant
        mousePos.z = Mathf.Abs(Camera.main.transform.position.z - player.position.z);
        // Find the mouse position
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        // Find the direction from player to mouse as a unit vector
        direction = (mousePos - player.position).normalized;

        // Place the marker at fixed radius (constant set above)
        cursorMarker.transform.position = player.position + direction * RADIUS;

        // Rotate the target marker -- no longer needed, originally the target was an ellipse that needed this
        //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // cursorMarker.transform.rotation = Quaternion.Euler(0, 0, angle);

        // Rotate player to face marker
        animator.SetFloat("Vertical", direction.y);
        animator.SetFloat("Horizontal", direction.x);
    }
}
