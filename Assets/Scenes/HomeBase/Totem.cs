using UnityEngine;
using UnityEngine.UI;

public class Totem : MonoBehaviour
{
    [Header("Totem Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("UI")]
    public Image healthBarFill; // Reference to the health bar fill image

    public delegate void TotemDestroyed();
    public static event TotemDestroyed OnTotemDestroyed; // Event to notify manager

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            DestroyTotem();
        }
    }

    private void UpdateHealthUI()
    {
        if (healthBarFill != null)
            healthBarFill.fillAmount = currentHealth / maxHealth;
    }

    private void DestroyTotem()
    {
        // Notify the manager that the totem has been destroyed
        OnTotemDestroyed?.Invoke();
        Destroy(gameObject);
    }
}
