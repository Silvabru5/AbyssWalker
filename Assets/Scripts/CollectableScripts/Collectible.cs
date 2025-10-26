using UnityEngine;

public class Collectible : MonoBehaviour, IInteractable
{
    [SerializeField] private string collectibleType; 
    private bool collected = false;
    private Animator anim;

    void Start()
    {
    }

    public bool CanInteract()
    {
        return !collected;
    }

    public void Interact()
    {
        if (collected) return;

        collected = true;

        
        if (CollectibleManager.Instance != null)
            CollectibleManager.Instance.AddCollectible(collectibleType);



        Debug.Log($"Collected a {collectibleType}");

        // Destroy after short delay for effect
        Destroy(gameObject, 0.2f);
    }
}
