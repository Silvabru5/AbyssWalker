using System.Collections.Generic;
using UnityEngine;

// Script is a manager to track collected items across scenes (level 1 and 2)
public class CollectibleManager : MonoBehaviour
{
    // Singleton instance
    public static CollectibleManager Instance;

    // Dictionary to track counts of each collectible type
    private Dictionary<string, int> collected = new Dictionary<string, int>();

    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Add a collectible of a specific type
    public void AddCollectible(string type)
    {
        // Initialize count if type not present
        if (!collected.ContainsKey(type))
            collected[type] = 0;
        // Increment the count
        collected[type]++;
        Debug.Log($"Collected {collected[type]} {type}(s)");
    }

    // Check if enough collectibles of a specific type have been collected to proceed to boss room
    public bool HasEnough(string type, int required)
    {
        // Return true if the count meets or exceeds the required amount
        return collected.ContainsKey(type) && collected[type] >= required;
    }

    //Get current count of a specific collectible type
    public int GetCount(string type)
    {
        return collected.ContainsKey(type) ? collected[type] : 0;
    }


    //Reset counts after each scene
    public void ResetText()
    {
        collected.Clear();
        Debug.Log("Collectible counts reset.");
    }
}
