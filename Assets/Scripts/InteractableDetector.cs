using UnityEngine;
using UnityEngine.InputSystem;

public class InteractableDetector : MonoBehaviour
{
    private IInteractable interactableInRange = null; // Find the closest interactable to the player
    [SerializeField] private GameObject interactableIcon;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       interactableIcon.SetActive(false);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            interactableInRange?.Interact();
        }    
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
        {
            interactableInRange = interactable;
            interactableIcon.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) 
    {
            if(collision.TryGetComponent(out IInteractable interactable) && interactable == interactableInRange)
            {
                interactableInRange = null;
                interactableIcon.SetActive(false);
            }
    }
}
