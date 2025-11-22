using UnityEngine;
using System.Collections;

/*
    Author(s): Bruno Silva
    Description: handles the visual blink effect that plays when the player is hit.
                 this briefly toggles the sprite renderer on and off to create a
                 flashing effect without interfering with gameplay or movement.
    Date (last modification): 11/22/2025
*/

public class PlayerHitEffect : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    // reference to the player's sprite renderer

    private bool isBlinking;
    // ensures the blink effect cannot run multiple times at once

    void Start()
    {
        // cache the sprite renderer component on startup
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // called by other scripts to play the blinking effect
    public void TriggerBlink()
    {
        // prevent overlapping blink coroutines
        if (!isBlinking)
            StartCoroutine(BlinkCoroutine());
    }

    // handles the timed flashing of the sprite renderer
    private IEnumerator BlinkCoroutine()
    {
        isBlinking = true;

        float blinkDuration = 0.5f;   // total length of the effect
        float blinkInterval = 0.1f;   // how fast the sprite toggles on and off

        // loop through the duration and toggle the sprite each interval
        for (float t = 0; t < blinkDuration; t += blinkInterval)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(blinkInterval);
        }

        // ensure sprite is visible once blinking is done
        spriteRenderer.enabled = true;
        isBlinking = false;
    }
}
