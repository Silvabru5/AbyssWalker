using TMPro;
using UnityEngine;

/*
Author: Tristan Ung
File: CollectibleProgressUI.cs
Description: Script to update UI text showing collectible progress towards opening the boss arena.
*/
public class CollectibleProgressUI : MonoBehaviour
{
    // Type of collectible to track
    public string collectibleType = "Skull"; 

    // Required amount to open boss arena
    public int requiredAmount = 5;

    // Reference to the TextMeshProUGUI component
    private TextMeshProUGUI label;

    void Awake()
    {
        // auto-grab the TMP component on the same object
        label = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        // Update the UI text with current collectible count
        if (CollectibleManager.Instance == null || label == null) return;

        // Get current count of the specified collectible type
        int count = CollectibleManager.Instance.GetCount(collectibleType);

        // Update the label text to show progress, and indicate if boss arena is open
        label.text = $"Collectables gathered {count}/{requiredAmount}"
                   + (count >= requiredAmount ? " Boss Arena Open" : "");
    }
}
