using UnityEngine;
using UnityEngine.SceneManagement;

public class BossPortals : MonoBehaviour, IInteractable
{
    [Header("Scene Settings")]
    [SerializeField] private string targetScene;

    [Header("Unlock Condition")]
    [SerializeField] private string requiredCollectibleType;
    [SerializeField] private int requiredAmount = 5;

    private Animator anim;
    private bool isUnlocked = false;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        CheckUnlockStatus(); 
    }

    void Update()
    {
        // Continuously check collections
        CheckUnlockStatus();
    }

    // private void CheckUnlockStatus()
    // {
    //     if (isUnlocked) return; // don’t re-trigger

    //     if (CollectibleManager.Instance != null &&
    //         CollectibleManager.Instance.HasEnough(requiredCollectibleType, requiredAmount))
    //     {
    //         isUnlocked = true;

    //         // Play activation animation if animator exists
    //         if (anim != null)
    //         {
    //             anim.SetBool("isUnlocked", true);
    //             Debug.Log($"{name} portal unlocked!");
    //         }
    //     }
    // }

    private void CheckUnlockStatus()
{
    if (isUnlocked) return;

    if (CollectibleManager.Instance == null)
    {
        Debug.LogWarning("CollectibleManager missing");
        return;
    }

    bool hasEnough = CollectibleManager.Instance.HasEnough(requiredCollectibleType, requiredAmount);
    int count = CollectibleManager.Instance.GetCount(requiredCollectibleType);
    Debug.Log($"Portal Checking unlock: {count}/{requiredAmount} {requiredCollectibleType}");

    if (hasEnough)
    {
        isUnlocked = true;
        if (anim != null)
        {
            anim.SetBool("isUnlocked", true);
        }
        else
        {
            Debug.LogWarning("no animator found");
        }
    }
}


    public bool CanInteract()
    {
        return isUnlocked;
    }

    public void Interact()
    {
        if (CanInteract())
        {
            Debug.Log($"tp'ing to {targetScene}");
            SceneManager.LoadScene(targetScene);
        }
        else
        {
            Debug.Log("more collectibles to activate this portal");
        }
    }
}
