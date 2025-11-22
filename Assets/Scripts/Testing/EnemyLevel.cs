using UnityEngine;

public class EnemyLevel : MonoBehaviour
{
    public string enemyType;

    [SerializeField] private int baseExp;
    [SerializeField] private int enemyLevel;
    [SerializeField] private float growthFactor;

    private void Awake()
    {
        int playerLevel = ExperienceManager.instance.GetCurrentLevel();
        // Assign base exp values that will increase base on players level
        baseExp = enemyType switch
        {
            "spider" => 2,
            "zombie" => 4,
            "skeleton" => 7,
            "bat" => 5,
            "buff zombie" => 12,
            "rust skeleton" => 18,
            "sword skeleton" => 22,
            _ => 1
        };

        //We can assign more creatures to this switch and change levels based on player.
        enemyLevel = enemyType switch
        {
            "bat" => Mathf.Max(1, playerLevel - 2),
            "spider" => Mathf.Max(1, playerLevel - 2), // 2 levels below
            "zombie" => playerLevel,                  // same level
            "skeleton" => playerLevel + 2,            // 2 above
            "buff zombie" => playerLevel + 2,            // 2 above
            "rust skeleton" => playerLevel + 3,
            "sword skeleton" => playerLevel + 4,
            _ => playerLevel
        };
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    
    }

    public int CalculateExp() // Calculating the amount of exp distributed based on enemy type
    {
        int exp = Mathf.RoundToInt(baseExp * Mathf.Pow(growthFactor, enemyLevel - 1));
        Debug.Log($"{enemyType} (Lvl {enemyLevel}) → Base: {baseExp}, Exp: {exp}");
        return exp; 
    }

    public int GetEnemyLevel()
        { return enemyLevel; }
}
