using UnityEngine;

public class DamageNumberSpawner : MonoBehaviour
{
    public static DamageNumberSpawner instance;

    // Prefab for critical hit damage numbers
    public GameObject critNumberPrefab; 

    // Prefab for regular damage numbers
    public GameObject damageNumberPrefab; 

    //Reference to the canvas where damage numbers will be spawned
    public Canvas canvas;                 

    void Awake()
    {

        // Singleton pattern to ensure only one instance exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); //this spawner persists across scenes to be reused
        }
        else
            Destroy(gameObject); //destroy duplicates
    }

    // Spawns a damage number at the specified world position
    public void SpawnDamageNumber(Vector3 worldPos, float damage)
    {
        // Convert world to canvas position
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);


        //Create damage number object in the canvas
        GameObject dmgNum = Instantiate(damageNumberPrefab, canvas.transform);

        //Set its position to the calculated screen position
        dmgNum.transform.position = screenPos;

        //set the damage value on the damage number script
        DamageNumber dmgScript = dmgNum.GetComponent<DamageNumber>();
        dmgScript.SetText(damage.ToString("F0")); // whole numbers only to be displayed
    }



    // Spawns a critical hit damage number at the specified world position, different prefab because of colour
    public void SpawnCritNumber(Vector3 worldPos, float damage)
    {
        // Convert world to canvas position
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        // Instantiate critical hit damage number prefab in the canvas
        GameObject dmgNum = Instantiate(critNumberPrefab, canvas.transform);

        // Set its position to the calculated screen position
        dmgNum.transform.position = screenPos;

        // Set the damage value on the damage number script
        DamageNumber dmgScript = dmgNum.GetComponent<DamageNumber>();

        dmgScript.SetText(damage.ToString("F0")); // whole numbers only to be displayed
    }
}
