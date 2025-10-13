using UnityEngine;
using System.Collections;

public class PlayerHitEffect : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private bool isBlinking;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TriggerBlink()
    {
        if (!isBlinking)
            StartCoroutine(BlinkCoroutine());
    }

    private IEnumerator BlinkCoroutine()
    {
        isBlinking = true;
        float blinkDuration = 0.5f;
        float blinkInterval = 0.1f;

        for (float t = 0; t < blinkDuration; t += blinkInterval)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(blinkInterval);
        }

        spriteRenderer.enabled = true;
        isBlinking = false;
    }
}
