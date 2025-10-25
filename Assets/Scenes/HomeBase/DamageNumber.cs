using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    public float floatSpeed = 1f;       // How fast it floats up
    public float fadeDuration = 0.5f;   // How long before disappearing

    private TextMeshProUGUI textMesh;
    private CanvasGroup canvasGroup;
    private float elapsed = 0f;

    void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetText(string text)
    {
        textMesh.text = text;
    }

    void Update()
    {
        // Move up
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        // Fade out
        elapsed += Time.deltaTime;
        canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);

        // Destroy when fully faded
        if (elapsed >= fadeDuration)
            Destroy(gameObject);
    }
}
