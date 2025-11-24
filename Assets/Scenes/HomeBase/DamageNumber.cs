using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    // How fast damage numbers float upwards
    public float floatSpeed = 1f;       

    // How long before numbers fade out
    public float fadeDuration = 0.5f;   

    // Reference to the TMP text component
    private TextMeshProUGUI textMesh;

    // Used to control fading without altering color manually
    private CanvasGroup canvasGroup;

    //Time that has passed since number spawned, to be checked for fade out
    private float elapsed = 0f;

    void Awake()
    {
        // Get required components on the same GameObject
        textMesh = GetComponent<TextMeshProUGUI>();
        canvasGroup = GetComponent<CanvasGroup>();
    }


    // Called by script spawns damage numbers
    public void SetText(string text)
    {
        textMesh.text = text; //sets damage value
    }

    void Update()
    {
        // logic for text to move upwards
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        // logic for fading out over time
        elapsed += Time.deltaTime;
        canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);

        // Destroy object when fully faded from scene
        if (elapsed >= fadeDuration)
            Destroy(gameObject);
    }
}
