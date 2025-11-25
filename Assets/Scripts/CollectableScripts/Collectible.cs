using UnityEngine;

/*
Author: Tristan Ung
File: Collectible.cs
Description:
Script for collectible items that the player can interact with and collect to proceed to boss room

*/
public class Collectible : MonoBehaviour, IInteractable
{
    // Type of collectible (e.g., "Skull", "Blood", etc.)
    [SerializeField] private string collectibleType;

    // Track if the collectible has been collected 
    private bool collected = false;

    // Reference to animator for collection effect (not implemented yet)
    private Animator anim;

    void Start()
    {
    }

    // Check if the collectible can be interacted with
    public bool CanInteract()
    {
        return !collected;
    }

    // Handle interaction with the collectible
    public void Interact()
    {
        // Prevent multiple collections of the same item
        if (collected) return;
        collected = true;

        // Add the collectible to the CollectibleManager to be tracked
        if (CollectibleManager.Instance != null)
        {
            CollectibleManager.Instance.AddCollectible(collectibleType);
            // Play collection sound effect
            SoundManager.PlaySound(SoundTypeEffects.TOKEN_PICKED_UP); 
        }
            



        Debug.Log($"Collected a {collectibleType}");

        // Destroy after short delay
        Destroy(gameObject, 0.2f);
    }
}
