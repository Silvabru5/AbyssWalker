/*
 * Author: Adrian Agius
 * File: IInteractable.cs
 * Description: Interface for the Interactable scripts. We use Interact() for logic, and CanInteract() to check if its able to be interacted with.
 */

public interface IInteractable
{ 
    void Interact();
    bool CanInteract();
}
