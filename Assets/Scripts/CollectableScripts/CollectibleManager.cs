using System.Collections.Generic;
using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    public static CollectibleManager Instance;

    private Dictionary<string, int> collected = new Dictionary<string, int>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddCollectible(string type)
    {
        if (!collected.ContainsKey(type))
            collected[type] = 0;

        collected[type]++;
        Debug.Log($"Collected {collected[type]} {type}(s)");
    }

    public bool HasEnough(string type, int required)
    {
        return collected.ContainsKey(type) && collected[type] >= required;
    }

    // Optional debug helper
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
