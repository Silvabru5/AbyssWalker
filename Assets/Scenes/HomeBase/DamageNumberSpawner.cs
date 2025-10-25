using UnityEngine;

public class DamageNumberSpawner : MonoBehaviour
{
    public static DamageNumberSpawner instance;

    public GameObject damageNumberPrefab; 
    public Canvas canvas;                 

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
            Destroy(gameObject);
    }

    public void SpawnDamageNumber(Vector3 worldPos, float damage)
    {
        // Convert world to canvas position
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        GameObject dmgNum = Instantiate(damageNumberPrefab, canvas.transform);
        dmgNum.transform.position = screenPos;

        DamageNumber dmgScript = dmgNum.GetComponent<DamageNumber>();
        dmgScript.SetText(damage.ToString("F0")); // whole numbers
    }
}
