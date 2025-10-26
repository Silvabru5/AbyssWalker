using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour, IInteractable
{
    [Header("Portal Settings")]
    [SerializeField] private int targetScene; // The name of the scene to load

    public bool CanInteract()
    {
        return true;
    }

    public void Interact()
    {
        if (!CanInteract()) return;
        SceneLoader.instance.LoadSpecificLevel(targetScene);
    }
}
