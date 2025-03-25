using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject narrowCone;
    public GameObject wideCone;
    public float showDuration = 0.2f;
    public float offsetDistance = 1f;

    private PlayerControl playerControl;

    void Start()
    {
        playerControl = GetComponent<PlayerControl>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TriggerAttack(narrowCone);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            TriggerAttack(wideCone);
        }
    }

    void TriggerAttack(GameObject cone)
    {
        if (cone == null || playerControl == null)
        {
            Debug.LogWarning("Missing reference in PlayerAttack script.");
            return;
        }

        Vector2 aimDir = playerControl.LastMoveDirection;
        if (aimDir != Vector2.zero)
        {
            float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

            // Only rotate the narrow cone
            if (cone == narrowCone)
            {
                cone.transform.localRotation = Quaternion.Euler(0, 0, angle - 90f); // Adjusted with offset
            }
            else
            {
                // Don't rotate wide cone
                cone.transform.localRotation = Quaternion.identity;
            }

            // Move both cones in the direction the player is aiming
            cone.transform.localPosition = aimDir.normalized * offsetDistance;
        }

        cone.SetActive(true);
        Invoke(nameof(HideAllCones), showDuration);
    }


    void HideAllCones()
    {
        narrowCone.SetActive(false);
        wideCone.SetActive(false);
    }
}
