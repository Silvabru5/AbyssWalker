using UnityEngine;
using UnityEngine.InputSystem;

/*
 * Author: Adrian Agius
 * File: InteractableDetecor.cs
 * Description: Detector script for the player Game Object in heirarchy. This allows for the key popup as well as checking interactable objects
 * if they're available. 
 */
public class InteractableDetector : MonoBehaviour
{
    private IInteractable interactableInRange = null; // Find the closest interactable to the player
    [SerializeField] private GameObject interactableIcon;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       interactableIcon = GameObject.Find("InteractIcon"); // Set the icon
       interactableIcon.SetActive(false); // ensure the icon is off on game start
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E)) // Set the key to be the interactable key
        {
            interactableInRange?.Interact(); // check if the object is in range
        }    
    }
    private void OnTriggerEnter2D(Collider2D collision) // Using a trigger collider to not interfere with collisions in game
    {
        if(collision.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
        {
            interactableInRange = interactable; // if in range set true; else false
            interactableIcon.SetActive(true); // turn icon on if near interactable object
        }
    }

    private void OnTriggerExit2D(Collider2D collision) 
    {
            if(collision.TryGetComponent(out IInteractable interactable) && interactable == interactableInRange)
            {
                interactableInRange = null; // object not in range
                interactableIcon.SetActive(false); // turn icon off
            }
    }
}
