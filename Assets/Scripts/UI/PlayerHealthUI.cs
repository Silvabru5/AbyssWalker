using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
        // drag the player object with the PlayerHealth script here
    public Image healthBarFill;           // drag the HealthBarFill image (the red/green bar) here

    private float currentFill = 1f;       // stores the current fill amount for smooth transition
    public float smoothSpeed = 5f;        // controls how fast the bar fills or depletes
    private GameObject player;
    private void Start()
    {
        player = GameObject.Find("Character"); 
    }

    // this updates the health bar's fill based on the player's current health
    void Update()
    {
        if (player.GetComponent<PlayerHealth>() != null && healthBarFill != null)
        {
            float targetFill = player.GetComponent<PlayerHealth>().GetHealthPercent();  // get current health as percent
            currentFill = Mathf.Lerp(currentFill, targetFill, Time.deltaTime * smoothSpeed); // smoothly animate
            healthBarFill.fillAmount = currentFill; // apply fill to the UI image
        }
    }
}
