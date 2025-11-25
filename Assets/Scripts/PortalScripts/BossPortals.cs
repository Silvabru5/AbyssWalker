using UnityEngine;
using UnityEngine.SceneManagement;

/*
Author: Tristan Ung
File: BossPortals.cs
Description: Manages boss portals that unlock based on collectible counts.
*/

public class BossPortals : MonoBehaviour, IInteractable
{
    // Serialized fields for scene and unlock conditions
    [Header("Scene Settings")]
    [SerializeField] private string targetScene; // Target scene to load on interaction

    [Header("Unlock Condition")]
    [SerializeField] private string requiredCollectibleType; // Type of collectible required to unlock
    [SerializeField] private int requiredAmount = 5; // Amount of collectibles needed to unlock

    [SerializeField] private Animator anim; // Animator for portal animations
    private bool isUnlocked = false; // Tracks if the portal is unlocked

    void Start()
    {
        CheckUnlockStatus(); 
    }

    void Update()
    {
        // Continuously check collections
        CheckUnlockStatus();
    }

    //checks if the portal should be unlocked based on collectibles
    private void CheckUnlockStatus()
{
    if (isUnlocked) return;

    if (CollectibleManager.Instance == null)
    {
        Debug.LogWarning("CollectibleManager missing");
        return;
    }
    // Check if enough collectibles have been collected
    bool hasEnough = CollectibleManager.Instance.HasEnough(requiredCollectibleType, requiredAmount);
    // Log current collectible count for debugging
    int count = CollectibleManager.Instance.GetCount(requiredCollectibleType);
    Debug.Log($"Portal Checking unlock: {count}/{requiredAmount} {requiredCollectibleType}");

    // Unlock the portal if enough collectibles are present
    if (hasEnough)
    {
        isUnlocked = true;
       
        
        anim.SetBool("isUnlocked", isUnlocked);
        
        // else
        // {
        //     Debug.LogWarning("no animator found");
        // }
    }
}

    // IInteractable implementation
    public bool CanInteract()
    {
        return isUnlocked;
    }

    // Teleports player to target scene if portal is unlocked
    public void Interact()
    {
        if (CanInteract())
        {
            Debug.Log($"tp'ing to {targetScene}");
            if (CollectibleManager.Instance != null)
            CollectibleManager.Instance.ResetText(); // Reset collectible UI text on scene change
            SceneManager.LoadScene(targetScene); // Load the target scene
        }
        else
        {
            Debug.Log("more collectibles to activate this portal");
        }
    }
}
